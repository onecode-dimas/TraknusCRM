// <copyright file="ConvertToProspectPartFromPotentialProspectPartSublLine.cs" company="">
// Copyright (c) 2018 All Rights Reserved
// </copyright>
// <author></author>
// <date>9/19/2018 2:33:45 PM</date>
// <summary>Implements the ConvertToProspectPartFromPotentialProspectPartSublLine Workflow Activity.</summary>
namespace TrakNusSparepartSystem.WorkflowActivity
{
    using System;
    using System.Activities;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;

    using TrakNusSparepartSystem.WorkflowActivity.BusinessLayer;

    public sealed class ConvertToProspectPartFromPotentialProspectPartSubLine : CodeActivity
    {
        [Input("RecordID")]
        public InArgument<string> RecordID { get; set; }

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

            tracingService.Trace("Entered ConvertToProspectPartFromPotentialProspectPartSublLine.Execute(), Activity Instance Id: {0}, Workflow Instance Id: {1}",
                executionContext.ActivityInstanceId,
                executionContext.WorkflowInstanceId);

            // Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve workflow context.");
            }

            tracingService.Trace("ConvertToProspectPartFromPotentialProspectPartSublLine.Execute(), Correlation Id: {0}, Initiating User: {1}",
                context.CorrelationId,
                context.InitiatingUserId);

            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                string recordID = RecordID.Get<string>(executionContext);
               BL_tss_prospectpart _BL_tss_prospectpart = new BL_tss_prospectpart();
                _BL_tss_prospectpart.GenerateProspectPartFromPotentialProspectPartSubLine_OnClick(service, tracingService, context, recordID);
                // TODO: Implement your custom Workflow business logic.
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                tracingService.Trace("Exception: {0}", e.ToString());

                // Handle the exception.
                throw;
            }

            tracingService.Trace("Exiting ConvertToProspectPartFromPotentialProspectPartSublLine.Execute(), Correlation Id: {0}", context.CorrelationId);
        }
    }
}