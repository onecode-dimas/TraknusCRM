// <copyright file="GenerateMarketSizeSummaryByPartNumberFromKeyAccount.cs" company="">
// Copyright (c) 2019 All Rights Reserved
// </copyright>
// <author></author>
// <date>17-May-19 10:25:18 AM</date>
// <summary>Implements the GenerateMarketSizeSummaryByPartNumberFromKeyAccount Workflow Activity.</summary>
namespace TrakNusSparepartSystem.WorkflowActivity
{
    using System;
    using System.Activities;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;
    using Business_Layer;

    public sealed class GenerateMarketSizeSummaryByPartNumberFromKeyAccount : CodeActivity
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

            tracingService.Trace("Entered GenerateMarketSizeSummaryByPartNumberFromKeyAccount.Execute(), Activity Instance Id: {0}, Workflow Instance Id: {1}",
                executionContext.ActivityInstanceId,
                executionContext.WorkflowInstanceId);

            // Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve workflow context.");
            }

            tracingService.Trace("GenerateMarketSizeSummaryByPartNumberFromKeyAccount.Execute(), Correlation Id: {0}, Initiating User: {1}",
                context.CorrelationId,
                context.InitiatingUserId);

            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                // TODO: Implement your custom Workflow business logic.

                string recordID = RecordID.Get<string>(executionContext);

                if (!string.IsNullOrEmpty(recordID))
                    recordID = recordID.Replace('{', ' ').Replace('}', ' ');

                BL_tss_mastermarketsize_sp _bl_tss_mastermarketsize_sp = new BL_tss_mastermarketsize_sp();
                _bl_tss_mastermarketsize_sp.GenerateMarketSizeSummaryByPartNumberFromKeyAccount_UsingSP_OnClick(service, tracingService, context, "");
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                tracingService.Trace("Exception: {0}", e.ToString());

                // Handle the exception.
                throw;
            }

            tracingService.Trace("Exiting GenerateMarketSizeSummaryByPartNumberFromKeyAccount.Execute(), Correlation Id: {0}", context.CorrelationId);
        }
    }
}