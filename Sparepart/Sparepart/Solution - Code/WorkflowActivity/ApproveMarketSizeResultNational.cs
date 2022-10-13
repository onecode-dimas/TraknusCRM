// <copyright file="ApproveMarketSizeRisultNasional.cs" company="">
// Copyright (c) 2018 All Rights Reserved
// </copyright>
// <author></author>
// <date>9/3/2018 2:43:05 PM</date>
// <summary>Implements the ApproveMarketSizeRisultNasional Workflow Activity.</summary>
namespace TrakNusSparepartSystem.WorkflowActivity
{
    using System;
    using System.Activities;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;
    using WorkflowActivity.BusinessLayer;

    public sealed class ApproveMarketSizeResultNational : CodeActivity
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

            tracingService.Trace("Entered ApproveMarketSizeResultNasional.Execute(), Activity Instance Id: {0}, Workflow Instance Id: {1}",
                executionContext.ActivityInstanceId,
                executionContext.WorkflowInstanceId);

            // Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve workflow context.");
            }

            tracingService.Trace("ApproveMarketSizeResultNasional.Execute(), Correlation Id: {0}, Initiating User: {1}",
                context.CorrelationId,
                context.InitiatingUserId);

            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                // TODO: Implement your custom Workflow business logic.
                string recordID = RecordID.Get<string>(executionContext);

                if (!string.IsNullOrEmpty(recordID))
                    recordID = recordID.Replace("{", "").Replace("}", "");

                BL_tss_marketsizeresultnational _BL_tss_marketsizeresultnational = new BL_tss_marketsizeresultnational();
                _BL_tss_marketsizeresultnational.ApproveMarketSizeResultNational_UsingSP_OnClick(service, tracingService, context, recordID);

                //BL_tss_marketsizeresultnational _BL_tss_marketsizeresultnational = new BL_tss_marketsizeresultnational();
                //_BL_tss_marketsizeresultnational.ApproveMarketSizeResultNational_OnClick(service, tracingService, context);

                //BL_tss_marketsizesummarybyunit _BL_tss_marketsizesummarybyunit = new BL_tss_marketsizesummarybyunit();
                //_BL_tss_marketsizesummarybyunit.generateMarketSizeSummarybyUnit(service, context);

                //BL_tss_marketsizesummarybyparttype _BL_tss_marketsizesummarybyparttype = new BL_tss_marketsizesummarybyparttype();
                //_BL_tss_marketsizesummarybyparttype.GenerateMarketSizeSummaryByPartType(service, tracingService, context);
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                tracingService.Trace("Exception: {0}", e.ToString());

                // Handle the exception.
                throw;
            }

            tracingService.Trace("Exiting ApproveMarketSizeResultNasional.Execute(), Correlation Id: {0}", context.CorrelationId);
        }
    }
}