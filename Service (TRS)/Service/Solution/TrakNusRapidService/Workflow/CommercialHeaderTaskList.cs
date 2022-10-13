// <copyright file="CommercialHeaderTaskList.cs" company="Microsoft">
// Copyright (c) 2014 All Rights Reserved
// </copyright>
// <author>Microsoft</author>
// <date>10/23/2014 3:38:55 PM</date>
// <summary>Implements the CommercialHeaderTaskList Workflow Activity.</summary>
namespace TrakNusRapidService.Workflow
{
    using System;
    using System.Activities;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;

    public sealed class CommercialHeaderTaskList : CodeActivity
    {
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

            tracingService.Trace("Entered CommercialHeaderTaskList.Execute(), Activity Instance Id: {0}, Workflow Instance Id: {1}",
                executionContext.ActivityInstanceId,
                executionContext.WorkflowInstanceId);

            // Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve workflow context.");
            }

            tracingService.Trace("CommercialHeaderTaskList.Execute(), Correlation Id: {0}, Initiating User: {1}",
                context.CorrelationId,
                context.InitiatingUserId);

            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                // TODO: Implement your custom Workflow business logic.
                // Get Entity Id
                Guid guid = context.PrimaryEntityId;

                // Get all related Commercial Header Ids 
                BusinessLayer.BL_task blt = new BusinessLayer.BL_task();
                EntityCollection commercialHeaders = blt.getAllCommercialHeader(service, guid);


                /*
                    if trs_productsection :
                     a) input --> true
                     b) blank --> false                  
                */

                if (commercialHeaders.Entities.Count > 0)
                {

                    Guid productSection = new Guid();

                    if (commercialHeaders.Entities[0].Attributes.Contains("trs_productsection") == true)
                    {
                        productSection = ((EntityReference)commercialHeaders.Entities[0].Attributes["trs_productsection"]).Id;
                        string name = string.Empty;
                        Guid idTasklist = new Guid();

                        #region Old Code
                        //QueryExpression qetl = new QueryExpression("trs_tasklist");
                        //qetl.ColumnSet = new ColumnSet(new string[] { "trs_productsection", "trs_name", "trs_tasklistid" });

                        //ConditionExpression cetl = new ConditionExpression();
                        //cetl.AttributeName = "trs_productsection";
                        //cetl.Operator = ConditionOperator.Equal;
                        //cetl.Values.Add(productSection.ToString());
                        //qetl.Criteria.AddCondition(cetl);
                        //EntityCollection entityCollection = service.RetrieveMultiple(qetl);
                        #endregion

                        BusinessLayer.BL_tasklist list = new BusinessLayer.BL_tasklist();
                        EntityCollection entityCollection = list.getAllTaskListByProductSection(service, productSection);
                        if (entityCollection.Entities.Count > 0)
                        {
                            name = entityCollection.Entities[0].Attributes["trs_name"].ToString();
                            idTasklist = (Guid)entityCollection.Entities[0].Attributes["trs_tasklistid"];
                        }

                        if (idTasklist != null)
                        {
                            #region Old Code
                            //Entity detail = new Entity("trs_commercialdetail");
                            //detail.Attributes["trs_commercialdetail"] = new EntityReference("trs_commercialdetail", guid);
                            //detail.Attributes["trs_taskname"] = new EntityReference("trs_tasklist", idTasklist);                            
                            //service.Create(detail);
                            #endregion

                            DataLayer.DL_trs_commercialdetail cdt = new DataLayer.DL_trs_commercialdetail();
                            //add 1 record tasklist
                            cdt.CommercialHeaderId = guid;
                            cdt.Taskname = idTasklist;
                            cdt.Insert(service);
                        }
                        else
                        {
                            throw new Exception("Failed to get Tasklist.");
                        }
                    }
                    else
                    {

                        //QueryExpression qetl = new QueryExpression("trs_tasklist");
                        //qetl.ColumnSet = new ColumnSet(new string[] { "trs_productsection", "trs_name", "trs_tasklistid" });                                               
                        //EntityCollection entityCollection = service.RetrieveMultiple(qetl);

                        BusinessLayer.BL_tasklist list = new BusinessLayer.BL_tasklist();
                        EntityCollection entityCollection = list.getAllTaskList(service);
                        foreach (Entity alls in entityCollection.Entities)
                        {

                            #region Old Code
                            //Entity detail = new Entity("trs_commercialdetail");
                            //detail.Attributes["trs_commercialdetail"] = new EntityReference("trs_commercialdetail", guid);
                            //Guid tempId = (Guid)alls.Attributes["trs_tasklistid"];
                            //detail.Attributes["trs_taskname"] = new EntityReference("trs_tasklist", tempId);
                            //service.Create(detail);
                            #endregion

                            DataLayer.DL_trs_commercialdetail cdt = new DataLayer.DL_trs_commercialdetail();
                            //add 1 record tasklist
                            cdt.CommercialHeaderId = guid;
                            Guid tempId = (Guid)alls.Attributes["trs_tasklistid"];
                            cdt.Taskname = tempId;
                            cdt.Insert(service);
                        }

                    }
                }

            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                tracingService.Trace("Exception: {0}", e.ToString());

                // Handle the exception.
                throw;
            }

            tracingService.Trace("Exiting CommercialHeaderTaskList.Execute(), Correlation Id: {0}", context.CorrelationId);
        }
    }
}