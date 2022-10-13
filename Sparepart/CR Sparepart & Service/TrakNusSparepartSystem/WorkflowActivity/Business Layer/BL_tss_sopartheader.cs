using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrakNusSparepartSystem.DataLayer;
using TrakNusSparepartSystem.Helper;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;

namespace TrakNusSparepartSystem.WorkflowActivity.BusinessLayer
{
    public class BL_tss_sopartheader
    {
        #region Constant

        #endregion

        #region Dependencies
        private DL_tss_totalpartconsumpmarketsize _DL_tss_totalpartconsumpmarketsize = new DL_tss_totalpartconsumpmarketsize();
        private DL_tss_salesorderpartheader _DL_tss_salesorderpartheader = new DL_tss_salesorderpartheader();
        private DL_tss_salesorderpartlines _DL_tss_salesorderpartlines = new DL_tss_salesorderpartlines();
        private DL_tss_potentialprospectpart _DL_tss_potentialprospectpart = new DL_tss_potentialprospectpart();
        private DL_tss_potentialprospectpartlines _DL_tss_potentialprospectpartlines = new DL_tss_potentialprospectpartlines();
        private DL_tss_potentialprospectpartsublines _DL_tss_potentialprospectpartsublines = new DL_tss_potentialprospectpartsublines();
        private DL_tss_prospectpartheader _DL_tss_prospectpartheader = new DL_tss_prospectpartheader();
        private DL_tss_prospectpartlines _DL_tss_prospectpartlines = new DL_tss_prospectpartlines();
        private DL_tss_quotationpartheader _DL_tss_quotationpartheader = new DL_tss_quotationpartheader();


        #endregion

        public void SalesOrderPartInvoiced(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {
            Entity EN_SalesOrderPart = _DL_tss_salesorderpartheader.Select(organizationService, context.PrimaryEntityId);
            
            //If Invoiced
            if(EN_SalesOrderPart.GetAttributeValue<OptionSetValue>("tss_statecode").Value == 865920004) { 

            #region get sales order part lines
            FilterExpression fSOPL = new FilterExpression(LogicalOperator.And);
            fSOPL.AddCondition("tss_sopartheaderid", ConditionOperator.Equal, context.PrimaryEntityId);

            QueryExpression qSOPL = new QueryExpression(_DL_tss_salesorderpartlines.EntityName);
            qSOPL.Criteria.AddFilter(fSOPL);
            qSOPL.ColumnSet = new ColumnSet(true);
            EntityCollection ENC_SalesOrderPartLine = _DL_tss_salesorderpartlines.Select(organizationService, qSOPL);
                #endregion

                foreach (Entity SOPL in ENC_SalesOrderPartLine.Entities)
                {
                    FilterExpression fTPC = new FilterExpression(LogicalOperator.And);
                    fTPC.AddCondition("tss_pss", ConditionOperator.Equal, EN_SalesOrderPart.GetAttributeValue<EntityReference>("tss_pss").Id);
                    fTPC.AddCondition("tss_customer", ConditionOperator.Equal, EN_SalesOrderPart.GetAttributeValue<EntityReference>("tss_customer").Id);
                    fTPC.AddCondition("tss_partnumber", ConditionOperator.Equal, SOPL.GetAttributeValue<EntityReference>("tss_partnumber").Id);

                    QueryExpression qTPC = new QueryExpression(_DL_tss_totalpartconsumpmarketsize.EntityName);
                    qTPC.Criteria.AddFilter(fTPC);
                    qTPC.ColumnSet = new ColumnSet(true);
                    EntityCollection ENC_TotalPartConsump = _DL_tss_totalpartconsumpmarketsize.Select(organizationService, qTPC);

                    int qtyTotalPartConsump = ENC_TotalPartConsump.Entities[0].GetAttributeValue<int>("tss_qty");
                    int qtySalesOrderPartLines = SOPL.GetAttributeValue<int>("tss_qtyrequest");

                    if (qtyTotalPartConsump == 0)
                    {
                        EN_SalesOrderPart["tss_sourcetype"] = new OptionSetValue(865920000);
                        organizationService.Update(EN_SalesOrderPart);
                    }
                    else if (qtySalesOrderPartLines <= qtyTotalPartConsump)
                    {
                        ENC_TotalPartConsump.Entities[0]["tss_qtyrequest"] = qtyTotalPartConsump - qtySalesOrderPartLines;
                        organizationService.Update(ENC_TotalPartConsump.Entities[0]);

                        #region get Quotation
                        FilterExpression fQ = new FilterExpression(LogicalOperator.And);
                        fQ.AddCondition("tss_salesorderno", ConditionOperator.Equal, context.PrimaryEntityId);

                        QueryExpression qQ = new QueryExpression(_DL_tss_quotationpartheader.EntityName);
                        qQ.Criteria.AddFilter(fQ);
                        qQ.ColumnSet = new ColumnSet(true);
                        EntityCollection ENC_Quotation = _DL_tss_quotationpartheader.Select(organizationService, qQ);
                        #endregion

                        #region get Prospect Part
                        FilterExpression fPP = new FilterExpression(LogicalOperator.And);
                        fPP.AddCondition("tss_prospectpartheaderid", ConditionOperator.Equal, ENC_Quotation.Entities[0].GetAttributeValue<EntityReference>("tss_prospectlink").Id);

                        QueryExpression qPP = new QueryExpression(_DL_tss_prospectpartheader.EntityName);
                        qPP.Criteria.AddFilter(fPP);
                        qPP.ColumnSet = new ColumnSet(true);
                        EntityCollection ENC_ProspectPart = _DL_tss_prospectpartheader.Select(organizationService, qQ);

                        #endregion

                        #region get Prospect Part Line 
                        FilterExpression fPPL = new FilterExpression(LogicalOperator.And);
                        fPPL.AddCondition("tss_prospectpartheader", ConditionOperator.Equal, ENC_ProspectPart.Entities[0].GetAttributeValue<EntityReference>("tss_prospectpartheaderid").Id);

                        QueryExpression qPPL = new QueryExpression(_DL_tss_prospectpartlines.EntityName);
                        qPPL.Criteria.AddFilter(fPPL);
                        qPPL.ColumnSet = new ColumnSet(true);
                        EntityCollection ENC_ProspectPartLines = _DL_tss_prospectpartlines.Select(organizationService, qPPL);

                        #endregion

                        //Check Market Size Reference and Update Qty
                        #region Potential Prospect Part
                        foreach (Entity en_ProspectPartLines in ENC_ProspectPartLines.Entities)
                        {
                            //if Market Size not empty 
                            if (en_ProspectPartLines.GetAttributeValue<EntityReference>("tss_refmarketsize") != null && en_ProspectPartLines.GetAttributeValue<EntityReference>("tss_refmarketsizebypmperiod") != null)
                            {
                                FilterExpression fPPP = new FilterExpression(LogicalOperator.And);
                                fPPP.AddCondition("tss_marketsizeid", ConditionOperator.Equal, en_ProspectPartLines.GetAttributeValue<EntityReference>("tss_refmarketsize").Id);

                                QueryExpression qPPP = new QueryExpression(_DL_tss_potentialprospectpart.EntityName);
                                qPPP.Criteria.AddFilter(fPPP);
                                qPPP.ColumnSet = new ColumnSet(true);
                                EntityCollection ENC_PotentialProspectPart = _DL_tss_potentialprospectpart.Select(organizationService, qPPP);

                                foreach (Entity en_PotentialProspectPart in ENC_PotentialProspectPart.Entities)
                                {
                                    FilterExpression fPPPL = new FilterExpression(LogicalOperator.And);
                                    fPPPL.AddCondition("tss_potentialprospectpart", ConditionOperator.Equal, en_PotentialProspectPart.GetAttributeValue<EntityReference>("tss_potentialprospectpartid").Id);

                                    QueryExpression qPPPL = new QueryExpression(_DL_tss_potentialprospectpartlines.EntityName);
                                    qPPPL.Criteria.AddFilter(fPPPL);
                                    qPPPL.ColumnSet = new ColumnSet(true);
                                    EntityCollection ENC_PotentialProspectPartLines = _DL_tss_potentialprospectpartlines.Select(organizationService, qPPPL);

                                    foreach (Entity en_PotentialProspectPartLines in ENC_PotentialProspectPartLines.Entities)
                                    {
                                        FilterExpression fPPPSL = new FilterExpression(LogicalOperator.And);
                                        fPPPSL.AddCondition("tss_potentialprospectpartlinesref", ConditionOperator.Equal, en_PotentialProspectPartLines.GetAttributeValue<EntityReference>("tss_potentialprospectpartlinesid").Id);
                                        fPPPSL.AddCondition("tss_partnumber", ConditionOperator.Equal, SOPL.GetAttributeValue<EntityReference>("tss_partnumber").Id);

                                        QueryExpression qPPPSL = new QueryExpression(_DL_tss_potentialprospectpartsublines.EntityName);
                                        qPPPSL.Criteria.AddFilter(fPPPSL);
                                        qPPPSL.ColumnSet = new ColumnSet(true);
                                        EntityCollection ENC_PotentialProspectPartSubLines = _DL_tss_potentialprospectpartsublines.Select(organizationService, qPPPSL);

                                        foreach (Entity en_PotentialProspectPartSubLines in ENC_PotentialProspectPartSubLines.Entities)
                                        {
                                            en_PotentialProspectPartSubLines["tss_remainingqty"] = en_PotentialProspectPart.GetAttributeValue<int>("tss_remainingqty") - qtySalesOrderPartLines;
                                            organizationService.Update(en_PotentialProspectPartSubLines);
                                        }
                                    }

                                }
                            }
                        }

                        #endregion
                    }
                    else if (qtySalesOrderPartLines > qtyTotalPartConsump)
                    {
                        ENC_TotalPartConsump.Entities[0]["tss_qtyrequest"] = qtyTotalPartConsump - qtySalesOrderPartLines;
                        organizationService.Update(ENC_TotalPartConsump.Entities[0]);

                        #region get Quotation
                        FilterExpression fQ = new FilterExpression(LogicalOperator.And);
                        fQ.AddCondition("tss_salesorderno", ConditionOperator.Equal, context.PrimaryEntityId);

                        QueryExpression qQ = new QueryExpression(_DL_tss_quotationpartheader.EntityName);
                        qQ.Criteria.AddFilter(fQ);
                        qQ.ColumnSet = new ColumnSet(true);
                        EntityCollection ENC_Quotation = _DL_tss_quotationpartheader.Select(organizationService, qQ);
                        #endregion

                        #region get Prospect Part
                        FilterExpression fPP = new FilterExpression(LogicalOperator.And);
                        fPP.AddCondition("tss_prospectpartheaderid", ConditionOperator.Equal, ENC_Quotation.Entities[0].GetAttributeValue<EntityReference>("tss_prospectlink").Id);

                        QueryExpression qPP = new QueryExpression(_DL_tss_prospectpartheader.EntityName);
                        qPP.Criteria.AddFilter(fPP);
                        qPP.ColumnSet = new ColumnSet(true);
                        EntityCollection ENC_ProspectPart = _DL_tss_prospectpartheader.Select(organizationService, qQ);

                        #endregion

                        #region get Prospect Part Line 
                        FilterExpression fPPL = new FilterExpression(LogicalOperator.And);
                        fPPL.AddCondition("tss_prospectpartheader", ConditionOperator.Equal, ENC_ProspectPart.Entities[0].GetAttributeValue<EntityReference>("tss_prospectpartheaderid").Id);

                        QueryExpression qPPL = new QueryExpression(_DL_tss_prospectpartlines.EntityName);
                        qPPL.Criteria.AddFilter(fPPL);
                        qPPL.ColumnSet = new ColumnSet(true);
                        EntityCollection ENC_ProspectPartLines = _DL_tss_prospectpartlines.Select(organizationService, qPPL);

                        #endregion

                        //Check Market Size Reference and Update Qty
                        #region Potential Prospect Part
                        foreach (Entity en_ProspectPartLines in ENC_ProspectPartLines.Entities)
                        {
                            //if Market Size not empty 
                            if (en_ProspectPartLines.GetAttributeValue<EntityReference>("tss_refmarketsize") != null && en_ProspectPartLines.GetAttributeValue<EntityReference>("tss_refmarketsizebypmperiod") != null)
                            {
                                FilterExpression fPPP = new FilterExpression(LogicalOperator.And);
                                fPPP.AddCondition("tss_marketsizeid", ConditionOperator.Equal, en_ProspectPartLines.GetAttributeValue<EntityReference>("tss_refmarketsize").Id);

                                QueryExpression qPPP = new QueryExpression(_DL_tss_potentialprospectpart.EntityName);
                                qPPP.Criteria.AddFilter(fPPP);
                                qPPP.ColumnSet = new ColumnSet(true);
                                EntityCollection ENC_PotentialProspectPart = _DL_tss_potentialprospectpart.Select(organizationService, qPPP);

                                foreach (Entity en_PotentialProspectPart in ENC_PotentialProspectPart.Entities)
                                {
                                    FilterExpression fPPPL = new FilterExpression(LogicalOperator.And);
                                    fPPPL.AddCondition("tss_potentialprospectpart", ConditionOperator.Equal, en_PotentialProspectPart.GetAttributeValue<EntityReference>("tss_potentialprospectpartid").Id);

                                    QueryExpression qPPPL = new QueryExpression(_DL_tss_potentialprospectpartlines.EntityName);
                                    qPPPL.Criteria.AddFilter(fPPPL);
                                    qPPPL.ColumnSet = new ColumnSet(true);
                                    EntityCollection ENC_PotentialProspectPartLines = _DL_tss_potentialprospectpartlines.Select(organizationService, qPPPL);

                                    foreach (Entity en_PotentialProspectPartLines in ENC_PotentialProspectPartLines.Entities)
                                    {
                                        FilterExpression fPPPSL = new FilterExpression(LogicalOperator.And);
                                        fPPPSL.AddCondition("tss_potentialprospectpartlinesref", ConditionOperator.Equal, en_PotentialProspectPartLines.GetAttributeValue<EntityReference>("tss_potentialprospectpartlinesid").Id);
                                        fPPPSL.AddCondition("tss_partnumber", ConditionOperator.Equal, SOPL.GetAttributeValue<EntityReference>("tss_partnumber").Id);

                                        QueryExpression qPPPSL = new QueryExpression(_DL_tss_potentialprospectpartsublines.EntityName);
                                        qPPPSL.Criteria.AddFilter(fPPPSL);
                                        qPPPSL.ColumnSet = new ColumnSet(true);
                                        EntityCollection ENC_PotentialProspectPartSubLines = _DL_tss_potentialprospectpartsublines.Select(organizationService, qPPPSL);

                                        foreach (Entity en_PotentialProspectPartSubLines in ENC_PotentialProspectPartSubLines.Entities)
                                        {
                                            en_PotentialProspectPartSubLines["tss_remainingqty"] = 0;
                                            organizationService.Update(en_PotentialProspectPartSubLines);
                                        }
                                    }

                                }
                            }
                        }

                        #endregion

                    }
                }
            }
        }
    }
}
