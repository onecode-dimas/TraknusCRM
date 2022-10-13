﻿// <copyright file="GenerateMasterMarketSizeFromKeyAccount.cs" company="">
// Copyright (c) 2019 All Rights Reserved
// </copyright>
// <author></author>
// <date>08-Feb-19 5:38:35 PM</date>
// <summary>Implements the GenerateMasterMarketSizeFromKeyAccount Workflow Activity.</summary>
namespace TrakNusSparepartSystem.WorkflowActivity
{
    using System;
    using System.Activities;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;
    using WorkflowActivity.BusinessLayer;
    using Business_Layer;

    public class GenerateMasterMarketSizeFromKeyAccount : CodeActivity
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

            tracingService.Trace("Entered GenerateMasterMarketSizeFromKeyAccount.Execute(), Activity Instance Id: {0}, Workflow Instance Id: {1}",
                executionContext.ActivityInstanceId,
                executionContext.WorkflowInstanceId);

            // Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve workflow context.");
            }

            tracingService.Trace("GenerateMasterMarketSizeFromKeyAccount.Execute(), Correlation Id: {0}, Initiating User: {1}",
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

                BL_tss_mastermarketsize_sp _bl_tss_mastermarketsize_sp = new BL_tss_mastermarketsize_sp();
                _bl_tss_mastermarketsize_sp.GenerateMasterMarketSizeFromKeyAccount_UsingSP_OnClick(service, tracingService, context, recordID);
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                tracingService.Trace("Exception: {0}", e.ToString());

                // Handle the exception.
                throw;
            }

            tracingService.Trace("Exiting GenerateMasterMarketSizeFromKeyAccount.Execute(), Correlation Id: {0}", context.CorrelationId);
        }
    }
}