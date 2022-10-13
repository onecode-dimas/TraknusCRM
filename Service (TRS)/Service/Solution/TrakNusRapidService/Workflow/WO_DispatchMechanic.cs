// <copyright file="WO_DispatchMechanic.cs" company="Microsoft">
// Copyright (c) 2014 All Rights Reserved
// </copyright>
// <author>Microsoft</author>
// <date>10/24/2014 3:32:26 PM</date>
// <summary>Implements the WO_DispatchMechanic Workflow Activity.</summary>
namespace TrakNusRapidService.Workflow
{
    using System;
    using System.Activities;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Sdk.Workflow;
    using TrakNusRapidService.Workflow;

    public sealed class WO_DispatchMechanic : CodeActivity
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

            tracingService.Trace("Entered WO_DispatchMechanic.Execute(), Activity Instance Id: {0}, Workflow Instance Id: {1}",
                executionContext.ActivityInstanceId,
                executionContext.WorkflowInstanceId);

            // Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve workflow context.");
            }

            tracingService.Trace("WO_DispatchMechanic.Execute(), Correlation Id: {0}, Initiating User: {1}",
                context.CorrelationId,
                context.InitiatingUserId);

            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                // TODO: Implement your custom Workflow business logic.
                Guid woId = context.PrimaryEntityId;

                #region Send Informasi Mechanic dengan task yang masih belum selesai
                BusinessLayer.BL_serviceappointment wo1 = new BusinessLayer.BL_serviceappointment();
                EntityCollection eServiceApp1 = wo1.getAllServiceAppointment(service, woId);
                if (eServiceApp1.Entities.Count > 0)
                {
                    string secondManId = null, subleaderId = null;
                    string leaderId = ((EntityReference)eServiceApp1.Entities[0].Attributes["trs_mechanicleader"]).Id.ToString();
                    if (eServiceApp1.Entities[0].Attributes.Contains("trs_mechanicsecondman"))
                    {
                        secondManId = eServiceApp1.Entities[0].Attributes["trs_mechanicsecondman"].ToString();
                    }

                    if (eServiceApp1.Entities[0].Attributes.Contains("trs_mechanicsubleader"))
                    {
                        subleaderId = eServiceApp1.Entities[0].Attributes["trs_mechanicsubleader"].ToString();
                    }
                    Int32 mechanicRole = new Int32();

                    BusinessLayer.BL_activityparty ap1 = new BusinessLayer.BL_activityparty();
                    EntityCollection eAp1 = ap1.getResourcesActivityParty(service, woId);
                    foreach (Entity partyList1 in eAp1.Entities)
                    {
                        Guid mechanicId = ((EntityReference)partyList1.Attributes["partyid"]).Id;
                        string mId = mechanicId.ToString();
                        //Compare string with string
                        //Ordinal =  is case-sensitive, which means that the two strings must match character for character
                        //example "and" does not equal "And" or "AND"

                        if (leaderId.Equals(mId, StringComparison.Ordinal))
                        {
                            mechanicRole = 167630000;
                            //Leader
                        }
                        else if (secondManId.Equals(mId, StringComparison.Ordinal))
                        {
                            mechanicRole = 167630003;
                            //Second Man
                        }
                        else if (subleaderId.Equals(mId, StringComparison.Ordinal))
                        {
                            mechanicRole = 167630001;
                            //Sub Leader
                        }
                        else
                        {
                            mechanicRole = 167630002;
                            //"Member"
                        }

                        

                        #region Mechanic / Entity Equipment
                        Entity equipment = service.Retrieve("equipment", mechanicId, new ColumnSet(new string[] { "trs_nrp" }));

                        string nrp = "-";

                        if (equipment.Attributes.Contains("trs_nrp"))
                        {
                            nrp = equipment.Attributes["trs_nrp"].ToString();
                        }
                        #endregion

                        #region Commercial HD
                        BusinessLayer.BL_task taskHd = new BusinessLayer.BL_task();
                        EntityCollection ecolWoHd = taskHd.getAllRelatedCommercialHeaderByField(service, woId, "trs_operationid");
                        if (ecolWoHd.Entities.Count > 0)
                        {
                            Guid activityIdTask = ecolWoHd.Entities[0].GetAttributeValue<Guid>("activityid");

                            #region Commercial DT
                            BusinessLayer.BL_trs_commercialdetail dt = new BusinessLayer.BL_trs_commercialdetail();
                            EntityCollection ecolCdt = dt.getAllRelatedCommercialDetailStatusIncomplete(service, activityIdTask, 1);
                            foreach (Entity e in ecolCdt.Entities)
                            {
                                Guid comDetailId = e.GetAttributeValue<Guid>("trs_commercialdetailid");
                                string taskCode = null, cdtName = null, ctaskName = null, stateCode = null;
                                if (e.Attributes.Contains("trs_commercialdetailname"))
                                {
                                    taskCode = e.Attributes["trs_taskcode"].ToString();
                                }

                                if (e.Attributes.Contains("trs_commercialdetailname"))
                                {
                                    cdtName = e.Attributes["trs_commercialdetailname"].ToString();
                                }

                                if (e.Attributes.Contains("trs_commercialtaskname"))
                                {
                                    ctaskName = e.Attributes["trs_commercialtaskname"].ToString();
                                }

                                if (e.Attributes.Contains("statecode"))
                                {
                                    stateCode = e.Attributes["statecode"].ToString();
                                }
                                BusinessLayer.BL_trs_commercialdetailmechanic cdm = new BusinessLayer.BL_trs_commercialdetailmechanic();
                                cdm.Form_OnCreate(comDetailId, nrp, mechanicRole, service); 
                            }


                            #endregion
                        }
                        #endregion
                    }
                }
                #endregion


            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                tracingService.Trace("Exception: {0}", e.ToString());

                // Handle the exception.
                throw;
            }

            tracingService.Trace("Exiting WO_DispatchMechanic.Execute(), Correlation Id: {0}", context.CorrelationId);
        }
    }
}