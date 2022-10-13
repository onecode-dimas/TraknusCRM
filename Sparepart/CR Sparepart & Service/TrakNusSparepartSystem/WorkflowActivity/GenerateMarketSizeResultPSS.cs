// <copyright file="GenerateMarketSizeResultPSS.cs" company="">
// Copyright (c) 2019 All Rights Reserved
// </copyright>
// <author></author>
// <date>2/8/2019 3:09:52 PM</date>
// <summary>Implements the GenerateMarketSizeResultPSS Workflow Activity.</summary>
namespace TrakNusSparepartSystem.WorkflowActivity
{
    using System;
    using System.Activities;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;
    using WorkflowActivity.BusinessLayer;
    using Business_Layer;

    using TrakNusSparepartSystem.DataLayer;
    using Microsoft.Xrm.Sdk.Query;
    using System.Linq;
    using TrakNusSparepartSystem.Helper;
    using System.Collections.Generic;

    public sealed class GenerateMarketSizeResultPSS : CodeActivity
    {
        /// <summary>
        /// Executes the workflow activity.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        protected override void Execute(CodeActivityContext executionContext)
        {
            // Create the tracing service
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();
            RetrieveHelper _retrievehelper = new RetrieveHelper();

            if (tracingService == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve tracing service.");
            }

            tracingService.Trace("Entered GenerateMarketSizeResultPSS.Execute(), Activity Instance Id: {0}, Workflow Instance Id: {1}",
                executionContext.ActivityInstanceId,
                executionContext.WorkflowInstanceId);

            // Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve workflow context.");
            }

            tracingService.Trace("GenerateMarketSizeResultPSS.Execute(), Correlation Id: {0}, Initiating User: {1}",
                context.CorrelationId,
                context.InitiatingUserId);

            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                // TODO: Implement your custom Workflow business logic.

                BL_tss_marketsizeresultpss _BL_tss_marketsizeresultpss = new BL_tss_marketsizeresultpss();
                _BL_tss_marketsizeresultpss.GenerateMarketSizeResultPSS_OnClick(service, context);
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                tracingService.Trace("Exception: {0}", e.ToString());

                // Handle the exception.
                throw;
            }

            tracingService.Trace("Exiting GenerateMarketSizeResultPSS.Execute(), Correlation Id: {0}", context.CorrelationId);
        }
    }
}