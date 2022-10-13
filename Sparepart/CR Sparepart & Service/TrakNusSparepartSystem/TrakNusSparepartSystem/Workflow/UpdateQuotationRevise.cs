// <copyright file="UpdateQuotationRevise.cs" company="">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author></author>
// <date>12/18/2017 1:34:43 PM</date>
// <summary>Implements the UpdateQuotationRevise Workflow Activity.</summary>
namespace TrakNusSparepartSystem.Workflow
{
    using System;
    using System.Activities;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Metadata;
    using TrakNusSparepartSystem.Workflow.BusinessLayer;

    public sealed class UpdateQuotationRevise : CodeActivity
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

            tracingService.Trace("Entered UpdateQuotationRevise.Execute(), Activity Instance Id: {0}, Workflow Instance Id: {1}",
                executionContext.ActivityInstanceId,
                executionContext.WorkflowInstanceId);

            // Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve workflow context.");
            }

            tracingService.Trace("UpdateQuotationRevise.Execute(), Correlation Id: {0}, Initiating User: {1}",
                context.CorrelationId,
                context.InitiatingUserId);

            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                // TODO: Implement your custom Workflow business logic.
                if (context.PrimaryEntityName == "tss_quotationpartheader")
                {
                    //update quotation to Revised
                    BL_tss_quotationpartheader _BL_tss_quotationpartheader = new BL_tss_quotationpartheader();
                    _BL_tss_quotationpartheader.RevisedQuotation_OnClick(service, context.PrimaryEntityId, context.PrimaryEntityName, tracingService);

                    //copy entity
                    _BL_tss_quotationpartheader.CreateQuotation_CloneEntity(service, context.PrimaryEntityId, context.PrimaryEntityName, tracingService);
                }
                else
                {
                    throw new Exception("This workflow primary entity must be Quotation Part Header. Current Primary Entity : " + context.PrimaryEntityName);
                }
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                tracingService.Trace("Exception: {0}", e.ToString());

                // Handle the exception.
                throw;
            }

            tracingService.Trace("Exiting UpdateQuotationRevise.Execute(), Correlation Id: {0}", context.CorrelationId);
        }

        
    }
}