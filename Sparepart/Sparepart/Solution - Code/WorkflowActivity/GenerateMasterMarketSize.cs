﻿// <copyright file="GenerateMasterMarketSize.cs" company="">
// Copyright (c) 2018 All Rights Reserved
// </copyright>
// <author></author>
// <date>8/20/2018 10:03:56 AM</date>
// <summary>Implements the GenerateMasterMarketSize Workflow Activity.</summary>
namespace TrakNusSparepartSystem.WorkflowActivity
{
    using System;
    using System.Activities;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;
    using WorkflowActivity.BusinessLayer;
    using Business_Layer;

    public sealed class GenerateMasterMarketSize : CodeActivity
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

            tracingService.Trace("Entered GenerateMasterMarketSize.Execute(), Activity Instance Id: {0}, Workflow Instance Id: {1}",
                executionContext.ActivityInstanceId,
                executionContext.WorkflowInstanceId);

            // Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve workflow context.");
            }

            tracingService.Trace("GenerateMasterMarketSize.Execute(), Correlation Id: {0}, Initiating User: {1}",
                context.CorrelationId,
                context.InitiatingUserId);

            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            
            try
            {
                // TODO: Implement your custom Workflow business logic.
                BL_tss_mastermarketsize _BL_tss_mastermarketsize = new BL_tss_mastermarketsize();
                _BL_tss_mastermarketsize.GenerateMasterMarketSize_OnClick(service, tracingService, context);

                //BL_tss_marketsizeresultpss _BL_tss_marketsizeresultpss = new BL_tss_marketsizeresultpss();
                //_BL_tss_marketsizeresultpss.GenerateMarketSizeResultPSS(service, tracingService, context);
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                tracingService.Trace("Exception: {0}", e.ToString());

                // Handle the exception.
                throw;
            }

            tracingService.Trace("Exiting GenerateMasterMarketSize.Execute(), Correlation Id: {0}", context.CorrelationId);
        }
    }
}