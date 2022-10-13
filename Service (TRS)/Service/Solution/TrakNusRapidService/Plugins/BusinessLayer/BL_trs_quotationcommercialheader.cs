using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using TrakNusRapidService.DataLayer;

namespace TrakNusRapidService.Plugins.BusinessLayer
{
    class BL_trs_quotationcommercialheader
    {
        #region Constants
        private const string _classname = "BL_trs_quotationcommercialheader";
        #endregion

        #region Depedencies
        private DL_trs_quotationcommercialheader _DL_trs_quotationcommercialheader = new DL_trs_quotationcommercialheader();
        private DL_trs_quotationcommercialdetail _DL_trs_quotationcommercialdetail = new DL_trs_quotationcommercialdetail();
        private DL_trs_sectionfactorrate _DL_trs_sectionfactorrate = new DL_trs_sectionfactorrate();
        private DL_trs_mechanicgrade _DL_trs_mechanicgrade = new DL_trs_mechanicgrade();
        private DL_trs_tasklist _DL_trs_tasklist = new DL_trs_tasklist();
        private DL_trs_quotationpartdetail _DL_trs_quotationpartdetail = new DL_trs_quotationpartdetail();
        private DL_trs_commercialtask _DL_trs_commercialtask = new DL_trs_commercialtask();
        #endregion

        #region Privates
        private decimal FillCommercialDetail(IOrganizationService organizationService, Guid id, Guid quotationId, Guid taskListHeaderId)
        {
            decimal rtg = 0;

            QueryExpression queryExpression = new QueryExpression(_DL_trs_commercialtask.EntityName);
            queryExpression.ColumnSet = new ColumnSet(true);

            FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
            filterExpression.AddCondition("trs_tasklistheaderid", ConditionOperator.Equal, taskListHeaderId);

            EntityCollection entityCollection = _DL_trs_commercialtask.Select(organizationService, queryExpression);
            foreach (Entity entity in entityCollection.Entities)
            {
                _DL_trs_quotationcommercialdetail = new DL_trs_quotationcommercialdetail();
                _DL_trs_quotationcommercialdetail.trs_commercialheader = id;
                _DL_trs_quotationcommercialdetail.trs_taskcode = entity.GetAttributeValue<string>("trs_taskcode");
                _DL_trs_quotationcommercialdetail.trs_mechanicgrade = entity.GetAttributeValue<EntityReference>("trs_mechanicgrade").Id;
                _DL_trs_quotationcommercialdetail.trs_tasklistdetail = entity.GetAttributeValue<Guid>("trs_commercialtaskid");
                _DL_trs_quotationcommercialdetail.Taskname = entity.GetAttributeValue<EntityReference>("trs_taskname").Id;
                _DL_trs_quotationcommercialdetail.trs_rtg = entity.GetAttributeValue<decimal>("trs_rtg");
                rtg += _DL_trs_quotationcommercialdetail.trs_rtg;
                _DL_trs_quotationcommercialdetail.trs_quotation = quotationId;
                _DL_trs_quotationcommercialdetail.Insert(organizationService);
            }
            return rtg;
        }
        #endregion

        #region Events
        #region Forms
        public void Form_OnCreate(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_trs_quotationcommercialheader.EntityName)
                {
                    decimal rtg = FillCommercialDetail(organizationService
                                    , entity.Id
                                    , entity.GetAttributeValue<EntityReference>("trs_quotationid").Id
                                    , entity.GetAttributeValue<EntityReference>("trs_taskheader").Id);

                    _DL_trs_quotationcommercialheader.trs_totalrtg = rtg;
                    _DL_trs_quotationcommercialheader.Update(organizationService, entity.Id);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate : " + ex.Message.ToString());
            }
        }
        #endregion

        #region Lock Record if Quotation Status : Final Approved
        public void Check_QuotationIsFinalApproved(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                #region Variable
                Guid QuotationId = Guid.Empty;
                int QuotationStatus = 0;
                #endregion

                #region Set Variable
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                Entity previous = new Entity("trs_quotation");

                if (pluginExceptionContext.MessageName == "Create" || pluginExceptionContext.MessageName == "Update")
                {
                    entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                }

                if (pluginExceptionContext.MessageName == "Update" || pluginExceptionContext.MessageName == "Delete")
                {
                    previous = (Entity)pluginExceptionContext.PreEntityImages["PreImage"];
                }

                #endregion

                if (entity.LogicalName == _DL_trs_quotationcommercialheader.EntityName)
                {
                    #region Set Variable
                    //QuotationId
                    if (entity.Attributes.Contains("trs_quotationid") && entity.Attributes["trs_quotationid"] != null)
                    {
                        QuotationId = ((EntityReference)entity.Attributes["trs_quotationid"]).Id;
                    }
                    else if (previous.Attributes.Contains("trs_quotationid") && previous.Attributes["trs_quotationid"] != null)
                    {
                        QuotationId = ((EntityReference)previous.Attributes["trs_quotationid"]).Id;
                    }
                    #endregion

                    #region Quotation (Get Quotation Status)
                    QueryExpression quotation = new QueryExpression();
                    quotation.EntityName = "trs_quotation";
                    quotation.ColumnSet.AllColumns = true;

                    ConditionExpression exQuotation = new ConditionExpression();
                    exQuotation.AttributeName = "trs_quotationid";
                    exQuotation.Operator = ConditionOperator.Equal;
                    exQuotation.Values.Add(QuotationId);

                    FilterExpression fQuotation = new FilterExpression();
                    fQuotation.FilterOperator = LogicalOperator.And;
                    fQuotation.Conditions.Add(exQuotation);

                    quotation.Criteria.AddFilter(fQuotation);
                    EntityCollection QuotationResult = organizationService.RetrieveMultiple(quotation);

                    if (QuotationResult.Entities.Count > 0)
                    {
                        foreach (var itemQuotation in QuotationResult.Entities)
                        {
                            if (itemQuotation.Attributes.Contains("statuscode") && itemQuotation.Attributes["statuscode"] != null)
                                QuotationStatus = ((OptionSetValue)itemQuotation.Attributes["statuscode"]).Value;
                        }
                    }
                    #endregion

                    #region Check if Qutation Status is Final Approved
                    if (QuotationStatus == 167630002) 
                    {
                        if (pluginExceptionContext.InputParameters.Contains("Target") && pluginExceptionContext.InputParameters["Target"] is Entity)
                        {
                            throw new Exception("Quotation Status already Final Approved. You cannot change it.");
                        }
                    }
                    #endregion
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        #endregion

        #region Fields
        #endregion
        #endregion
    }
}
