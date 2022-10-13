// <copyright file="SendEmailTechnicalServiceReport.cs" company="Microsoft">
// Copyright (c) 2014 All Rights Reserved
// </copyright>
// <author>Microsoft</author>
// <date>10/28/2014 2:44:59 PM</date>
// <summary>Implements the SendEmailTechnicalServiceReport Workflow Activity.</summary>
namespace TrakNusRapidService.Workflow
{
    using System;
    using System.Activities;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Sdk.Workflow;
    using TrakNusRapidService.Helper;
    using TrakNusRapidService.DataLayer;

    public sealed class SendEmailTechnicalServiceReport : CodeActivity
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

            tracingService.Trace("Entered SendEmailTechnicalServiceReport.Execute(), Activity Instance Id: {0}, Workflow Instance Id: {1}",
                executionContext.ActivityInstanceId,
                executionContext.WorkflowInstanceId);

            // Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve workflow context.");
            }

            tracingService.Trace("SendEmailTechnicalServiceReport.Execute(), Correlation Id: {0}, Initiating User: {1}",
                context.CorrelationId,
                context.InitiatingUserId);

            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                // TODO: Implement your custom Workflow business logic.
                Guid woID = context.PrimaryEntityId;
                //throw new Exception(guid.ToString());

                Entity eWorkorder = service.Retrieve("serviceappointment", woID, new ColumnSet(new string[] { "new_serialnumber", "subject" }));
                string woNo = string.Empty, equipNo = string.Empty;
                if (eWorkorder.Contains("subject"))
                {
                    woNo = eWorkorder.Attributes["subject"].ToString();
                }
                if (eWorkorder.Contains("new_serialnumber"))
                {
                    equipNo = eWorkorder.Attributes["new_serialnumber"].ToString();
                }

                string sNo = "S001";

                DL_trs_technicalservicereport _DL_trs_technicalservicereport = new DL_trs_technicalservicereport();
                QueryExpression queryExpression = new QueryExpression(_DL_trs_technicalservicereport.EntityName);
                queryExpression.ColumnSet = new ColumnSet(true);

                FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                filterExpression.AddCondition("trs_workorder", ConditionOperator.Equal, woID);

                EntityCollection ecolTSR = _DL_trs_technicalservicereport.Select(service, queryExpression);
                if (ecolTSR.Entities.Count > 0)
                {
                    Entity eTsr = ecolTSR.Entities[0];
                    Guid prodTypeId = ((EntityReference)eTsr.Attributes["trs_producttype"]).Id;
                    Guid populationId = ((EntityReference)eTsr.Attributes["trs_equipment"]).Id;
                    DateTime troubleDate = eTsr.GetAttributeValue<DateTime>("trs_troubledate").ToLocalTime();
                    //throw new Exception(prodTypeId.ToString() + " : " + populationId.ToString());

                    #region Validation for Warranty date
                    BusinessLayer.BL_new_population pop = new BusinessLayer.BL_new_population();
                    EntityCollection ecolPopulation = pop.getRecordNewPopulation(service, populationId);
                    if (ecolPopulation.Entities.Count > 0)
                    {
                        Entity ePopulation = ecolPopulation.Entities[0];
                        DateTime? start = null;
                        if (ePopulation.Attributes.Contains("trs_warrantystartdate"))
                            start = ePopulation.GetAttributeValue<DateTime>("trs_warrantystartdate").ToLocalTime();
                        DateTime? end = null;
                        if (ePopulation.Attributes.Contains("trs_warrantyenddate"))
                            end = ePopulation.GetAttributeValue<DateTime>("trs_warrantyenddate").ToLocalTime();
                        if (start == null || end == null)
                            throw new InvalidPluginExecutionException("E-mail only will be created for Population in warranty period. Please check this population's warranty period !");

                        if (troubleDate >= start && troubleDate <= end)
                        {
                            //throw new Exception("start : " + start + " -  END = " + end);

                            #region Create Email
                            BusinessLayer.BL_trs_producttype pt = new BusinessLayer.BL_trs_producttype();
                            EntityCollection prodType = pt.getAllProductType(service, prodTypeId);
                            if (prodType.Entities.Count > 0)
                            {
                                Guid psUser = Guid.Empty;
                                if (prodType.Entities[0].Attributes.Contains("trs_productspecialistuserid"))
                                    psUser = ((EntityReference)prodType.Entities[0].Attributes["trs_productspecialistuserid"]).Id;
                                else
                                    throw new InvalidWorkflowException("Please setup Product Specialist for this Product Type first !");

                                //throw new Exception(psUser.ToString() + " = receiver,  sender = " + context.InitiatingUserId.ToString());
                                //EmailAgent email = new EmailAgent();
                                //email.description = "Technical Service Report ";
                                //email.subject = "Send Email Product Specialist User";
                                //email.AddSender(context.InitiatingUserId);
                                //email.AddReceiver("systemuser", psUser);
                                //Guid emailId;
                                //email.Create(service, out emailId);
                                //////email.Send(service, emailId);

                                #region My Custom Email
                                string descriptionBody1 = " Already submit new Technical Service Report (TSR) as listed below : \n Number TSR: (ie: TSR140824000103) ";
                                string descriptionBody2 = " If you have any question regarding to this notification, please contact Administrator TRS.";

                                Entity InputEmail = new Entity("email");
                                InputEmail["subject"] = "New TSR";
                                InputEmail["description"] = "This is notification email to inform you : \n For: \n WO Number : " + woNo + "  \n Equipment Number : " + equipNo + "  \n S/N : " + sNo + descriptionBody1 + descriptionBody2;

                                Entity fromParty = new Entity("activityparty");
                                fromParty["partyid"] = new EntityReference("systemuser", context.InitiatingUserId);

                                Entity toParty = new Entity("activityparty");
                                toParty["partyid"] = new EntityReference("systemuser", psUser);

                                InputEmail["from"] = new Entity[] { fromParty };
                                InputEmail["to"] = new Entity[] { toParty };

                                //Create an EMail Record
                                Guid sendEmailId = service.Create(InputEmail);
                                #endregion
                            }
                            #endregion
                        }
                        else
                        {
                            //expired warranty
                        }

                    }
                    #endregion
                }
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                tracingService.Trace("Exception: {0}", e.ToString());

                // Handle the exception.
                throw;
            }

            tracingService.Trace("Exiting SendEmailTechnicalServiceReport.Execute(), Correlation Id: {0}", context.CorrelationId);
        }

 
 
    }
}