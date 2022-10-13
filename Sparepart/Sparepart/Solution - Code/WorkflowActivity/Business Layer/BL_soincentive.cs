using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusSparepartSystem.WorkflowActivity.Business_Layer
{
    public class BL_soincentive : BaseCustomeWofkflow

    {

        private const int INVOICETYPE_PARTDIRECTSALES = 865920000;
        private const int INVOICETYPE_PTSLABOR = 865920001;
        private const int INVOICETYPE_LABOR = 865920002;
        private const int INVOICETYPE_PTS = 865920003;
        private const int SO_SOURCETYPE_SERVICE = 865920005;
        public BL_soincentive(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context) :
        base(organizationService, tracingService, context)
        {
        }
        public void InsertSOIncentiveFromSO()
        {
            
            FilterExpression fe = new FilterExpression(LogicalOperator.And);
            fe.AddCondition("tss_sopartlinesid", ConditionOperator.Equal, CRMContext.PrimaryEntityId);
            fe.AddCondition("tss_status", ConditionOperator.In, new string[] { "865920003", "865920004" });
            LinkEntity linkSoPartLine = new LinkEntity
            {
                LinkFromEntityName = "tss_sopartlines",
                LinkToEntityName = "trs_masterpart",
                LinkFromAttributeName = "tss_partnumber",
                LinkToAttributeName = "trs_masterpartid",
                Columns = new ColumnSet(new string[] { "trs_product" }),
                JoinOperator = JoinOperator.Inner,
                EntityAlias = "sopartline"
            };

            LinkEntity linkPartMaster = new LinkEntity
            {
                LinkFromEntityName = "trs_masterpart",
                LinkToEntityName = "product",
                LinkFromAttributeName = "trs_product",
                LinkToAttributeName = "productid",
                Columns = new ColumnSet(new string[] { "new_unitgroup","defaultuomscheduleid" }),
                JoinOperator = JoinOperator.Inner,
                EntityAlias = "product"
            };

            CRMQExp = new QueryExpression("tss_sopartlines");
            CRMQExp.LinkEntities.Add(linkSoPartLine);
            CRMQExp.LinkEntities[0].LinkEntities.Add(linkPartMaster);
            CRMQExp.ColumnSet = new ColumnSet(new string[] {"tss_sopartheaderid", "tss_partnumber", "createdon" });
            CRMQExp.Criteria.AddFilter(fe);
            EntityCollection joinsopartline = CRMOrganizationService.RetrieveMultiple(CRMQExp);

            var unitGroup = joinsopartline.Entities.Select(x => ((EntityReference)x.GetAttributeValue<AliasedValue>("product.defaultuomscheduleid").Value).Id).Distinct();

            EntityReference sopartheaderRef = joinsopartline.Entities.FirstOrDefault().GetAttributeValue<EntityReference>("tss_sopartheaderid");
            Entity sopartheader = CRMOrganizationService.Retrieve(sopartheaderRef.LogicalName, sopartheaderRef.Id,new ColumnSet(true));

            CRMFilterExp = new FilterExpression(LogicalOperator.And);
            CRMFilterExp.AddCondition("tss_sonumber", ConditionOperator.Equal, sopartheader?.Id);
            CRMQExp = new QueryExpression("tss_soincentive");
            CRMQExp.Criteria.AddFilter(CRMFilterExp);
            EntityMultyRetrieve = CRMOrganizationService.RetrieveMultiple(CRMQExp);
            if (EntityMultyRetrieve.Entities.Count() > 0)
            {
                if (sopartheader.GetAttributeValue<OptionSetValue>("tss_statecode").Value == 865920005)
                {
                    EntitySingleRetrieve = EntityMultyRetrieve.Entities.FirstOrDefault();
                    Entity entityToUpdate = new Entity("tss_soincentive");
                    entityToUpdate.Id = EntitySingleRetrieve.Id;
                    //untuk f5 di skip dulu
                    //entityToUpdate.Attributes["tss_f5"] = 0;
                    CRMOrganizationService.Update(entityToUpdate);
                }
            }
            else
            {
                Entity soincentiveToInsert = new Entity("tss_soincentive");
                decimal?
                    f1 = 0,
                    f2 = 0;

                OptionSetValue optValue = null,
                    sourcetyesoparttype = sopartheader.GetAttributeValue<OptionSetValue>("tss_sourcetype");
                #region F1 Factor
                CRMFilterExp = new FilterExpression(LogicalOperator.And);
                CRMFilterExp.AddCondition("tss_product", ConditionOperator.Equal, unitGroup.FirstOrDefault());
                CRMQExp = new QueryExpression("tss_incentivef1productfactor");                
                CRMQExp.ColumnSet = new ColumnSet(new string[] { "tss_factor","tss_product" });
                CRMQExp.Criteria.AddFilter(CRMFilterExp);
                f1 = CRMOrganizationService.RetrieveMultiple(CRMQExp).Entities.FirstOrDefault()?.GetAttributeValue<decimal>("tss_factor");
                #endregion

                #region F2 Factor
                /*
                 Invoice Type = Part Direct Sales, jika Source Type SO adalah Direct Sales, Market Size, Dealer, Counter
                 Invoice Type = PTS + Labor, jika Source Type SO adalah Service dan terdapat data pada SO Part Lines dan SO Lines Service
                 Invoice Type = PTS Only, jika Source Type SO adalah Service dan terdapat data pada SO Part Lines dan tidak ada data pada SO Lines Service
                 */
                bool isPartLine = false,
                     isPartLineService = false;
                decimal? totalamount = 0;
                if (sourcetyesoparttype.Value == SO_SOURCETYPE_SERVICE)
                {
                    optValue = new OptionSetValue(INVOICETYPE_PARTDIRECTSALES);
                    //CRMQExp = new QueryExpression("tss_sopartlines")
                    //{
                    //    Criteria =
                    //    {
                    //        Conditions = { new ConditionExpression("tss_sopartheaderid",ConditionOperator.Equal,sopartheader.Id)}
                    //    }
                    //};
                    //EntitySingleRetrieve = CRMOrganizationService.RetrieveMultiple(CRMQExp).Entities.FirstOrDefault();
                    totalamount = sopartheader?.GetAttributeValue<Money>("tss_totalamount").Value;

                }
                else
                {
                    CRMQExp = new QueryExpression("tss_sopartlines")
                    {
                        Criteria =
                        {
                            Conditions = { new ConditionExpression("tss_sopartheaderid",ConditionOperator.Equal,sopartheader.Id)}
                        }
                    };

                    if (CRMOrganizationService.RetrieveMultiple(CRMQExp).Entities.Count() > 0)
                        isPartLine = true;

                    CRMQExp = new QueryExpression("tss_sopartlinesservice")
                    {
                        Criteria =
                        {
                            Conditions={new ConditionExpression("tss_sopartheaderid",ConditionOperator.Equal,sopartheader.Id)}
                        }
                    };

                    if (CRMOrganizationService.RetrieveMultiple(CRMQExp).Entities.Count() > 0)
                        isPartLineService = true;

                    if (isPartLine && isPartLineService)
                        optValue = new OptionSetValue(INVOICETYPE_PTSLABOR);
                    else
                        optValue = new OptionSetValue(INVOICETYPE_PTS);
                    totalamount = sopartheader?.GetAttributeValue<Money>("tss_totalamount").Value;

                }

                CRMFilterExp = new FilterExpression(LogicalOperator.And);
                CRMFilterExp.AddCondition("tss_invoicetype", ConditionOperator.Equal, optValue.Value);
                CRMQExp = new QueryExpression("tss_incentivef2invoicetype");
                CRMQExp.ColumnSet = new ColumnSet(new string[] { "tss_factor" });
                CRMQExp.Criteria.AddFilter(CRMFilterExp);
                f2 = CRMOrganizationService.RetrieveMultiple(CRMQExp).Entities.FirstOrDefault()?.GetAttributeValue<decimal>("tss_factor");
                #endregion

                //soincentiveToInsert.Attributes["tss_wonumber"] = 0;
                soincentiveToInsert.Attributes["tss_type"] = new OptionSetValue(865920000);//Sales Order
                soincentiveToInsert.Attributes["tss_sonumber"] = sopartheaderRef;
                soincentiveToInsert.Attributes["tss_sodate"] = sopartheader?.GetAttributeValue<DateTime>("createdon");
                soincentiveToInsert.Attributes["tss_salesamount"] = totalamount;
                soincentiveToInsert.Attributes["tss_pss"] = sopartheader?.GetAttributeValue<EntityReference>("tss_pss");
                soincentiveToInsert.Attributes["tss_branch"] = sopartheader?.GetAttributeValue<EntityReference>("tss_branch");
                soincentiveToInsert.Attributes["tss_f1"] = f1;
                soincentiveToInsert.Attributes["tss_f2"] = f2;
                CRMOrganizationService.Create(soincentiveToInsert);
            }
        }

        //public void InsertSOIncentiveFromWO()
        //{
        //    Entity woheader = CRMOrganizationService.Retrieve("serviceappointment", CRMContext.PrimaryEntityId, new ColumnSet(true));
        //    FilterExpression fe = new FilterExpression(LogicalOperator.And);
        //    fe.AddCondition("serviceappointmentid", ConditionOperator.In, CRMContext.PrimaryEntityId);
        //    // fe.AddCondition("tss_status", ConditionOperator.Equal, INVOICED);

        //    //var unitGroup = joinsopartline.Entities.Select(x => ((EntityReference)x.GetAttributeValue<AliasedValue>("product.new_unitgroup").Value).Id).Distinct();

        //    CRMFilterExp = new FilterExpression(LogicalOperator.And);
        //    CRMFilterExp.AddCondition("tss_wonumber", ConditionOperator.Equal, woheader?.GetAttributeValue<string>("new_nowo"));
        //    CRMQExp = new QueryExpression("tss_soincentive");
        //    CRMQExp.Criteria.AddFilter(CRMFilterExp);
        //    EntityMultyRetrieve = CRMOrganizationService.RetrieveMultiple(CRMQExp);
        //    if (EntityMultyRetrieve.Entities.Count() > 0)
        //    {
        //        EntitySingleRetrieve = EntityMultyRetrieve.Entities.FirstOrDefault();
        //        Entity entityToUpdate = new Entity("tss_soincentive");
        //        entityToUpdate.Id = EntitySingleRetrieve.Id;
        //        entityToUpdate.Attributes["tss_f5"] = 0;
        //        CRMOrganizationService.Update(entityToUpdate);
        //    }
        //    else
        //    {
        //        Entity soincentiveToInsert = new Entity("tss_soincentive");
        //        decimal?
        //            f1 = 0,
        //            f2 = 0;
        //        OptionSetValue optValue = null,
        //            sourcetyesoparttype = woheader.GetAttributeValue<OptionSetValue>("tss_sourcetype");
        //        #region F1 Factor
        //        CRMFilterExp = new FilterExpression(LogicalOperator.And);
        //        CRMFilterExp.AddCondition("tss_product", ConditionOperator.Equal, unitGroup.FirstOrDefault());
        //        CRMQExp = new QueryExpression("tss_incentivef1productfactor");
        //        CRMQExp.ColumnSet = new ColumnSet(new string[] { "tss_factor" });
        //        CRMQExp.Criteria.AddFilter(CRMFilterExp);
        //        f1 = CRMOrganizationService.RetrieveMultiple(CRMQExp).Entities.FirstOrDefault()?.GetAttributeValue<decimal>("tss_fatore");
        //        #endregion

        //        #region F2 Factor
        //        /*
        //         Invoice Type = Part Direct Sales, jika Source Type SO adalah Direct Sales, Market Size, Dealer, Counter
        //         Invoice Type = PTS + Labor, jika Source Type SO adalah Service dan terdapat data pada SO Part Lines dan SO Lines Service
        //         Invoice Type = PTS Only, jika Source Type SO adalah Service dan terdapat data pada SO Part Lines dan tidak ada data pada SO Lines Service
        //         */
        //        bool isPartLine = false,
        //             isPartLineService = false;
        //        if (sourcetyesoparttype.Value == SO_SOURCETYPE_SERVICE)
        //            optValue = new OptionSetValue(INVOICETYPE_PARTDIRECTSALES);
        //        else
        //        {
        //            CRMQExp = new QueryExpression("tss_sopartlines")
        //            {
        //                Criteria =
        //                {
        //                    Conditions = { new ConditionExpression("tss_sopartheaderid",ConditionOperator.Equal,woheader.Id)}
        //                }
        //            };

        //            if (CRMOrganizationService.RetrieveMultiple(CRMQExp).Entities.Count() > 0)
        //                isPartLine = true;

        //            CRMQExp = new QueryExpression("tss_sopartlinesservice")
        //            {
        //                Criteria =
        //                {
        //                    Conditions={new ConditionExpression("tss_sopartheaderid",ConditionOperator.Equal,woheader.Id)}
        //                }
        //            };

        //            if (CRMOrganizationService.RetrieveMultiple(CRMQExp).Entities.Count() > 0)
        //                isPartLineService = true;

        //            if (isPartLine && isPartLineService)
        //                optValue = new OptionSetValue(INVOICETYPE_PTSLABOR);
        //            else
        //                optValue = new OptionSetValue(INVOICETYPE_PTS);


        //        }

        //        CRMFilterExp = new FilterExpression(LogicalOperator.And);
        //        CRMFilterExp.AddCondition("tss_invoicetype", ConditionOperator.Equal, optValue.Value);
        //        CRMQExp = new QueryExpression("tss_incentivef2invoicetype");
        //        CRMQExp.ColumnSet = new ColumnSet(new string[] { "tss_factor" });
        //        CRMQExp.Criteria.AddFilter(CRMFilterExp);
        //        f2 = CRMOrganizationService.RetrieveMultiple(CRMQExp).Entities.FirstOrDefault()?.GetAttributeValue<decimal>("tss_fatore");
        //        #endregion



        //        //soincentiveToInsert.Attributes["tss_wonumber"] = 0;
        //        soincentiveToInsert.Attributes["tss_type"] = 0;
        //        soincentiveToInsert.Attributes["tss_sonumber"] = woheader?.GetAttributeValue<string>("tss_sonumber");
        //        soincentiveToInsert.Attributes["tss_sodate"] = woheader?.GetAttributeValue<DateTime>("createdon").ToLocalTime();
        //        soincentiveToInsert.Attributes["tss-salesamount"] = woheader?.GetAttributeValue<Money>("tss_totalamount");
        //        soincentiveToInsert.Attributes["tss_pss"] = woheader?.GetAttributeValue<EntityReference>("tss_pss");
        //        soincentiveToInsert.Attributes["tss_branch"] = woheader?.GetAttributeValue<EntityReference>("tss_branch");
        //        soincentiveToInsert.Attributes["tss_f1"] = f1;
        //        soincentiveToInsert.Attributes["tss_f2"] = f2;
        //        CRMOrganizationService.Create(soincentiveToInsert);
        //    }
        //}
    }
}
