// <copyright file="QuotationPartsSummary.cs" company="">
// Copyright (c) 2014 All Rights Reserved
// </copyright>
// <author>Erry Handani</author>
// <date>10/30/2014 10:19:42 AM</date>
// <summary>Implements the QuotationPartsSummary Workflow Activity.</summary>
namespace TrakNusRapidService.Workflow
{
    using System;
    using System.Activities;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;
    using Microsoft.Xrm.Sdk.Query;
    using TrakNusRapidService.DataLayer;
    using BusinessLayer;

    public sealed class QuotationPartsSummary : CodeActivity
    {
        #region Constanta
        private string _classname = "QuotationPartsSummary";
        #endregion

        #region Dependencies
        DL_trs_quotationpartdetail _DL_trs_quotationpartdetail = new DL_trs_quotationpartdetail();
        DL_trs_masterpart _DL_trs_masterpart = new DL_trs_masterpart();
        DL_trs_tasklistdetailpart _DL_trs_tasklistdetailpart = new DL_trs_tasklistdetailpart();
        DL_trs_commercialtask _DL_trs_commercialtask = new DL_trs_commercialtask();
        DL_trs_quotationcommercialheader _DL_trs_quotationcommercialheader = new DL_trs_quotationcommercialheader();
        DL_trs_quotation _DL_trs_quotation = new DL_trs_quotation();
        DL_trs_toolsgroup _DL_trs_toolsgroup = new DL_trs_toolsgroup();
        DL_trs_toolsmaster _DL_trs_toolsmaster = new DL_trs_toolsmaster();
        DL_trs_quotationtool _DL_trs_quotationtool = new DL_trs_quotationtool();
        DL_trs_quotationcommercialdetail _DL_trs_quotationcommercialdetail = new DL_trs_quotationcommercialdetail();

        BL_trs_quotationpartdetail _BL_trs_quotationpartdetail = new BL_trs_quotationpartdetail();

        private BL_trs_quotationpartssummary _BL_trs_quotationpartssummary = new BusinessLayer.BL_trs_quotationpartssummary();
        #endregion

        #region Private
        private Guid GetQuotationPartDetailId(IOrganizationService organizationService, Guid partnumber, Guid quotationId, Guid commercialdetailId)
        {
            EntityCollection oEntityCollection = new EntityCollection();
            Guid quotationpartdetailId = Guid.Empty;

            try
            {
                QueryExpression qeQuotationPartDetail = new QueryExpression();
                qeQuotationPartDetail.EntityName = _DL_trs_quotationpartdetail.EntityName;
                qeQuotationPartDetail.ColumnSet = new ColumnSet(true);

                qeQuotationPartDetail.Criteria = new FilterExpression();
                qeQuotationPartDetail.Criteria.AddCondition(new ConditionExpression("trs_partnumber", ConditionOperator.Equal, partnumber));
                qeQuotationPartDetail.Criteria.AddCondition(new ConditionExpression("trs_quotation", ConditionOperator.Equal, quotationId));
                qeQuotationPartDetail.Criteria.AddCondition(new ConditionExpression("trs_commercialdetailid", ConditionOperator.Equal, commercialdetailId));

                oEntityCollection = _DL_trs_quotationpartdetail.Select(organizationService, qeQuotationPartDetail);
                foreach (Entity oEntity in oEntityCollection.Entities) {
                    quotationpartdetailId = (Guid)oEntity.Attributes["trs_quotationpartdetailid"];
                }
                return quotationpartdetailId;
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".GetQuotationPartDetailId : " + ex.Message.ToString());
            }
        }

        private Guid GetQuotationToolsId(IOrganizationService organizationService, Guid quotationId, Guid toolsmasterId)
        {
            EntityCollection oEntityCollection = new EntityCollection();
            Guid quotationtoolId = Guid.Empty;

            try
            {
                QueryExpression qeQuotationTool = new QueryExpression();
                qeQuotationTool.EntityName = _DL_trs_quotationtool.EntityName;
                qeQuotationTool.ColumnSet = new ColumnSet(true);

                qeQuotationTool.Criteria = new FilterExpression();
                qeQuotationTool.Criteria.AddCondition(new ConditionExpression("trs_quotation", ConditionOperator.Equal, quotationId));
                qeQuotationTool.Criteria.AddCondition(new ConditionExpression("trs_toolsmaster", ConditionOperator.Equal, toolsmasterId));

                oEntityCollection = _DL_trs_quotationpartdetail.Select(organizationService, qeQuotationTool);
                foreach (Entity oEntity in oEntityCollection.Entities)
                {
                    quotationtoolId = (Guid)oEntity.Attributes["trs_quotationtoolid"];
                }
                return quotationtoolId;
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".GetQuotationToolsId : " + ex.Message.ToString());
            }
        }

        private Guid GetQuotationCommercialDetailId(IOrganizationService organizationService, Guid commercialheaderId, Guid quotationId, Guid tasklistdetailId)
        {
            EntityCollection oEntityCollection = new EntityCollection();
            Guid quotationcommercialdetailId = Guid.Empty;

            try
            {
                QueryExpression qeQuotationCommercialDetail = new QueryExpression();
                qeQuotationCommercialDetail.EntityName = _DL_trs_quotationcommercialdetail.EntityName;
                qeQuotationCommercialDetail.ColumnSet = new ColumnSet(true);

                qeQuotationCommercialDetail.Criteria = new FilterExpression();
                qeQuotationCommercialDetail.Criteria.AddCondition(new ConditionExpression("trs_commercialheader", ConditionOperator.Equal, commercialheaderId));
                qeQuotationCommercialDetail.Criteria.AddCondition(new ConditionExpression("trs_quotation", ConditionOperator.Equal, quotationId));
                qeQuotationCommercialDetail.Criteria.AddCondition(new ConditionExpression("trs_tasklistdetail", ConditionOperator.Equal, tasklistdetailId));

                oEntityCollection = _DL_trs_quotationcommercialdetail.Select(organizationService, qeQuotationCommercialDetail);
                foreach (Entity oEntity in oEntityCollection.Entities)
                {
                    quotationcommercialdetailId = oEntity.GetAttributeValue<Guid>("trs_quotationcommercialdetailid");
                }
                return quotationcommercialdetailId;
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".GetQuotationCommercialDetailId : " + ex.Message.ToString());
            }
        }

        private void QuotationTools(IOrganizationService organizationService, Guid quotationId,Guid toolsmasterId) {
            try
            {
                EntityCollection ecToolsGroup = new EntityCollection();
                Guid toolsgroupId = Guid.Empty;

                QueryExpression qeToolsGroup = new QueryExpression();
                qeToolsGroup.EntityName = _DL_trs_toolsgroup.EntityName;
                qeToolsGroup.ColumnSet = new ColumnSet(true);

                LinkEntity leToolsMaster = new LinkEntity();
                leToolsMaster.LinkFromEntityName = _DL_trs_toolsgroup.EntityName;
                leToolsMaster.LinkFromAttributeName = "trs_toolsgroupid";
                leToolsMaster.LinkToEntityName = _DL_trs_toolsmaster.EntityName;
                leToolsMaster.LinkToAttributeName = "trs_toolsgroupid";
                leToolsMaster.EntityAlias = "trs_toolsmaster";
                leToolsMaster.Columns.AddColumn("trs_toolsname");
                leToolsMaster.LinkCriteria.AddCondition("trs_toolsmasterid", ConditionOperator.Equal, toolsmasterId);

                qeToolsGroup.Criteria = new FilterExpression();
                qeToolsGroup.LinkEntities.Add(leToolsMaster);
                qeToolsGroup.Criteria.AddCondition(new ConditionExpression("statecode", ConditionOperator.Equal, "Active"));

                ecToolsGroup = _DL_trs_masterpart.Select(organizationService, qeToolsGroup);

                foreach (Entity eToolsGroup in ecToolsGroup.Entities)
                {
                    if (eToolsGroup.Attributes.Contains("trs_toolsgroupid"))
                    {
                        toolsgroupId = eToolsGroup.GetAttributeValue<Guid>("trs_toolsgroupid");
                        _DL_trs_quotationtool.trs_toolsgroup = toolsgroupId;
                    }

                    _DL_trs_quotationtool.trs_toolsname =(string)eToolsGroup.GetAttributeValue<AliasedValue>("trs_toolsmaster.trs_toolsname").Value;

                    var quotationtoolId = GetQuotationToolsId(organizationService, quotationId, toolsmasterId);

                    _DL_trs_quotationtool.trs_quotation = quotationId;                    
                    _DL_trs_quotationtool.trs_toolsmaster = toolsmasterId;
                    
                    if (quotationId != Guid.Empty && toolsgroupId != Guid.Empty && toolsmasterId != Guid.Empty)
                    {
                        if (quotationtoolId == Guid.Empty)
                        {
                            _DL_trs_quotationtool.Insert(organizationService);
                        }
                        else {
                            _DL_trs_quotationtool.Update(organizationService, quotationtoolId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".QuotationTools : " + ex.Message.ToString());
            }
        }

        private void SummaryQuotationComponents(IOrganizationService organizationService, Guid quotationId)
        {
            EntityCollection ecMasterPart = new EntityCollection();

            try
            {
                QueryExpression qeMasterPart = new QueryExpression();
                qeMasterPart.EntityName = _DL_trs_masterpart.EntityName;
                qeMasterPart.ColumnSet = new ColumnSet(true);
                
                LinkEntity leTaskListDetailPart = new LinkEntity();
                leTaskListDetailPart.LinkFromEntityName = _DL_trs_masterpart.EntityName;
                leTaskListDetailPart.LinkFromAttributeName = "trs_masterpartid";
                leTaskListDetailPart.LinkToEntityName = _DL_trs_tasklistdetailpart.EntityName;
                leTaskListDetailPart.LinkToAttributeName = "trs_masterpartid";
                leTaskListDetailPart.JoinOperator = JoinOperator.Inner;
                leTaskListDetailPart.EntityAlias = "trs_tasklistdetailpart";
                leTaskListDetailPart.Columns.AddColumn("trs_tasklistdetailid");
                
                LinkEntity leCommercialTask = new LinkEntity();
                leCommercialTask.LinkFromEntityName = _DL_trs_tasklistdetailpart.EntityName;
                leCommercialTask.LinkFromAttributeName = "trs_tasklistdetailid";
                leCommercialTask.LinkToEntityName = _DL_trs_commercialtask.EntityName;
                leCommercialTask.LinkToAttributeName = "trs_commercialtaskid";
                leCommercialTask.JoinOperator = JoinOperator.Inner;
                leCommercialTask.EntityAlias = "trs_commercialtask";

                LinkEntity leQuotationCommercialHeader = new LinkEntity();
                leQuotationCommercialHeader.LinkFromEntityName = _DL_trs_commercialtask.EntityName;
                leQuotationCommercialHeader.LinkFromAttributeName = "trs_tasklistheaderid";
                leQuotationCommercialHeader.LinkToEntityName = _DL_trs_quotationcommercialheader.EntityName;
                leQuotationCommercialHeader.LinkToAttributeName = "trs_taskheader";
                leQuotationCommercialHeader.JoinOperator = JoinOperator.Inner;
                leQuotationCommercialHeader.EntityAlias = "trs_quotationcommercialheader";
                leQuotationCommercialHeader.Columns.AddColumn("trs_quotationcommercialheaderid");
                leQuotationCommercialHeader.LinkCriteria.AddCondition("trs_quotationid", ConditionOperator.Equal, quotationId);

                leTaskListDetailPart.LinkEntities.Add(leCommercialTask);
                leCommercialTask.LinkEntities.Add(leQuotationCommercialHeader);

                qeMasterPart.Criteria = new FilterExpression();
                qeMasterPart.LinkEntities.Add(leTaskListDetailPart);
                qeMasterPart.Criteria.AddCondition(new ConditionExpression("statecode", ConditionOperator.Equal, "Active"));

                ecMasterPart = _DL_trs_masterpart.Select(organizationService, qeMasterPart);

                foreach (Entity eMasterPart in ecMasterPart.Entities)
                {
                    Guid masterpartid = Guid.Empty;
                    Guid toolsmasterId = Guid.Empty;
                    Guid commercialdetailId = Guid.Empty;
                    Guid quotationpartdetailId = Guid.Empty;
                    Guid quotationcommercialheaderId = Guid.Empty;
                    Guid tasklistdetailId = Guid.Empty;

                    if (eMasterPart.Attributes.Contains("trs_masterpartid"))
                    {
                        masterpartid = eMasterPart.GetAttributeValue<Guid>("trs_masterpartid");
                        _DL_trs_quotationpartdetail.trs_partnumber = masterpartid;
                    }

                    if (eMasterPart.Attributes.Contains("trs_tools"))
                    {
                        toolsmasterId = eMasterPart.GetAttributeValue<EntityReference>("trs_tools").Id;
                    }

                    var otasklistdetail = eMasterPart.GetAttributeValue<AliasedValue>("trs_tasklistdetailpart.trs_tasklistdetailid").Value;
                    tasklistdetailId = ((EntityReference)otasklistdetail).Id;

                    quotationcommercialheaderId = (Guid)eMasterPart.GetAttributeValue<AliasedValue>("trs_quotationcommercialheader.trs_quotationcommercialheaderid").Value;
                    commercialdetailId = GetQuotationCommercialDetailId(organizationService, quotationcommercialheaderId, quotationId, tasklistdetailId);

                    _DL_trs_quotationpartdetail.trs_commercialdetailid = commercialdetailId;

                    if (eMasterPart.Attributes.Contains("trs_partdescription"))
                    {
                        _DL_trs_quotationpartdetail.trs_partdescription = eMasterPart.GetAttributeValue<string>("trs_partdescription");
                    }

                    quotationpartdetailId = GetQuotationPartDetailId(organizationService, masterpartid, quotationId, commercialdetailId);

                    _DL_trs_quotationpartdetail.trs_quotation = quotationId;
                    _DL_trs_quotationpartdetail.trs_quantity =eMasterPart.GetAttributeValue<int>("trs_quantity");

                    if (quotationpartdetailId == Guid.Empty)
                    {
                        _DL_trs_quotationpartdetail.Insert(organizationService);
                    }
                    else {
                        _DL_trs_quotationpartdetail.Update(organizationService, quotationpartdetailId);
                    }

                    //Collect all Tools Group for Quotation Tools
                    QuotationTools(organizationService, quotationId, toolsmasterId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SummaryQuotationComponents : " + ex.Message.ToString());
            }
        }
        #endregion
        /// <summary>
        /// Executes the workflow activity.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        protected override void Execute(CodeActivityContext executionContext)
        {
            // Create the tracing service
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            if (tracingService == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve tracing service.");
            }

            tracingService.Trace("Entered QuotationPartsSummary.Execute(), Activity Instance Id: {0}, Workflow Instance Id: {1}",
                executionContext.ActivityInstanceId,
                executionContext.WorkflowInstanceId);

            // Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve workflow context.");
            }

            tracingService.Trace("QuotationPartsSummary.Execute(), Correlation Id: {0}, Initiating User: {1}",
                context.CorrelationId,
                context.InitiatingUserId);

            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                // TODO: Implement your custom Workflow business logic.
                
                // Get Quotation Id
                Guid guid = context.PrimaryEntityId;

                /* Change by Thomas - 17 March 2015
                BusinessLayer.BL_trs_quotationpartdetail blqpd = new BusinessLayer.BL_trs_quotationpartdetail();
                BusinessLayer.BL_trs_quotationpartssummary blqps = new BusinessLayer.BL_trs_quotationpartssummary();
                
                //Summarize Components and Tools for Quotation
                SummaryQuotationComponents(service, guid);

                EntityCollection partsRelatedtoQuotation = blqpd.getAllPartsRelatedtoCommercialDetail(service, guid);
                if (partsRelatedtoQuotation.Entities.Count > 0)
                {
                    // Loop through Detail Parts
                    foreach (Entity partsDetails in partsRelatedtoQuotation.Entities)
                    {
                        int partsManualQty = 0;
                        int partsTasklistQty = 0;

                        Guid partMasterId = Guid.Empty;
                        int partsQty = 0;

                        if (partsDetails.Attributes.Contains("trs_partnumber"))
                        {
                            partMasterId = ((EntityReference)partsDetails.Attributes["trs_partnumber"]).Id;
                        }

                        if (partsDetails.Attributes.Contains("trs_quantity"))
                        {
                            partsQty = (int)partsDetails.Attributes["trs_quantity"];
                        }

                        // Find existed part summary with the same part number and get the summary Id
                        EntityCollection summary = blqps.getQuotationPartSummarybyPartNumber(service, partMasterId, guid);

                        // If parts there are parts with the same Id and Work Order
                        if (summary.Entities.Count == 1)
                        {
                            Guid summaryId = Guid.Empty;
                            int summaryQty = 0;
                            Entity summaryEntity = summary.Entities[0];

                            if (summaryEntity.Attributes.Contains("trs_quotationpartssummaryid"))
                            {
                                summaryId = (Guid)summaryEntity.Attributes["trs_quotationpartssummaryid"];
                            }

                            if (summaryEntity.Attributes.Contains("trs_tasklistquantity"))
                            {
                                summaryQty = (int)summaryEntity.Attributes["trs_tasklistquantity"];
                            }

                            summaryQty = summaryQty + partsQty;
                            // Update Work Order Parts Summaries Parts
                            blqps.updateIntoQuotationPartSummary(service, summaryId, partsManualQty, summaryQty);
                            //throw new Exception("count: " + summary.Entities.Count.ToString());
                        }
                        else
                        {
                            partsTasklistQty = partsQty;
                            // Create Work Order Parts Summaries Parts
                            blqps.insertIntoQuotationPartSummary(service, guid, partMasterId, partsManualQty, partsTasklistQty);
                        }

                        //Saving Quotation - Components 

                    }
                }
                else
                {
                    throw new Exception("No parts details are related to Quotation");
                }
                 * */
                _BL_trs_quotationpartssummary.SummarizeParts(service, tracingService, guid);
                /* End change by Thomas - 17 March 2015 */
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                tracingService.Trace("Exception: {0}", e.ToString());

                // Handle the exception.
                throw;
            }

            tracingService.Trace("Exiting QuotationPartsSummary.Execute(), Correlation Id: {0}", context.CorrelationId);
        }
    }
}