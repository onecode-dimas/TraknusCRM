// <copyright file="Incentive_ApproveIncentive.cs" company="">
// Copyright (c) 2019 All Rights Reserved
// </copyright>
// <author></author>
// <date>18-Jul-19 2:34:10 PM</date>
// <summary>Implements the Incentive_ApproveIncentive Workflow Activity.</summary>
namespace EnhancementCRM.WorkflowActivity
{
    using System;
    using System.Activities;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;
    using Business_Layer;

    public sealed class Incentive_ApproveIncentive : CodeActivity
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

            tracingService.Trace("Entered Incentive_ApproveIncentive.Execute(), Activity Instance Id: {0}, Workflow Instance Id: {1}",
                executionContext.ActivityInstanceId,
                executionContext.WorkflowInstanceId);

            // Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve workflow context.");
            }

            tracingService.Trace("Incentive_ApproveIncentive.Execute(), Correlation Id: {0}, Initiating User: {1}",
                context.CorrelationId,
                context.InitiatingUserId);

            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                string _recordid = RecordID.Get<string>(executionContext);
                string[] _recordids;

                if (!string.IsNullOrEmpty(_recordid))
                    _recordid = _recordid.Replace('{', ' ').Replace('}', ' ');

                _recordids = _recordid.Split(',');

                // TODO: Implement your custom Workflow business logic.
                BL_ittn_incentive _incentive = new BL_ittn_incentive();
                _incentive.ApproveIncentive(service, context, _recordids);
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                tracingService.Trace("Exception: {0}", e.ToString());

                // Handle the exception.
                throw;
            }

            tracingService.Trace("Exiting Incentive_ApproveIncentive.Execute(), Correlation Id: {0}", context.CorrelationId);
        }
    }
}