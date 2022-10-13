// <copyright file="Account_ApproveToFinanceDeptHead.cs" company="">
// Copyright (c) 2022 All Rights Reserved
// </copyright>
// <author></author>
// <date>10/6/2022 9:57:42 PM</date>
// <summary>Implements the Account_ApproveToFinanceDeptHead Workflow Activity.</summary>
namespace AgitEnhancement2022.Workflow
{
    using System;
    using System.Activities;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;
    using BusinessLayer;

    public sealed class Account_ApproveToFinanceDeptHead : CodeActivity
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

            tracingService.Trace("Entered Account_ApproveToFinanceDeptHead.Execute(), Activity Instance Id: {0}, Workflow Instance Id: {1}",
                executionContext.ActivityInstanceId,
                executionContext.WorkflowInstanceId);

            // Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve workflow context.");
            }

            tracingService.Trace("Account_ApproveToFinanceDeptHead.Execute(), Correlation Id: {0}, Initiating User: {1}",
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
                BL_Account _account = new BL_Account();
                //_account.RequestApprovalForFAT(service, context, _recordids);
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                tracingService.Trace("Exception: {0}", e.ToString());

                // Handle the exception.
                throw;
            }

            tracingService.Trace("Exiting Account_ApproveToFinanceDeptHead.Execute(), Correlation Id: {0}", context.CorrelationId);
        }
    }
}