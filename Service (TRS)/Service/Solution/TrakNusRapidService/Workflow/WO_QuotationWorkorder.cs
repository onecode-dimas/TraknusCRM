// <copyright file="WO_QuotationWorkorder.cs" company="Microsoft">
// Copyright (c) 2014 All Rights Reserved
// </copyright>
// <author>Microsoft</author>
// <date>12/3/2014 1:42:08 PM</date>
// <summary>Implements the WO_QuotationWorkorder Workflow Activity.</summary>
namespace TrakNusRapidService.Workflow
{
    using System;
    using System.Activities;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;

    public sealed class WO_QuotationWorkorder : CodeActivity
    {
        #region Depedencies
        private BusinessLayer.BL_serviceappointment _BL_serviceappointment = new BusinessLayer.BL_serviceappointment();
        #endregion

        // Define an activity input argument of type string
        public InArgument<string> Text { get; set; }

        /// <summary>
        /// Executes the workflow activity.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        protected override void Execute(CodeActivityContext context)
        {

            // Obtain the runtime value of the Text input argument
            string text = context.GetValue(this.Text);

            ITracingService tracingService = context.GetExtension<ITracingService>();
            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory organizationServiceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService organizationService = organizationServiceFactory.CreateOrganizationService(workflowContext.UserId);

            Guid id = workflowContext.PrimaryEntityId;

            _BL_serviceappointment.CreateQuotationWorkorder(organizationService, tracingService, id);
        }
    }
}