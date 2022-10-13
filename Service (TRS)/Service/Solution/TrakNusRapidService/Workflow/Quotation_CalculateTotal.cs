// <copyright file="Quotation_CalculateTotal.cs" company="">
// Copyright (c) 2014 All Rights Reserved
// </copyright>
// <author></author>
// <date>11/25/2014 1:23:22 PM</date>
// <summary>Implements the Quotation_CalculateTotal Workflow Activity.</summary>
namespace TrakNusRapidService.Workflow
{
    using System;
    using System.Activities;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;

    public sealed class Quotation_CalculateTotal : CodeActivity
    {
        #region Depedencies
        private BusinessLayer.BL_trs_quotation _BL_trs_quotation = new BusinessLayer.BL_trs_quotation();
        #endregion

        // Define an activity input argument of type string
        public InArgument<string> Text { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.

        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            string text = context.GetValue(this.Text);

            ITracingService tracingService = context.GetExtension<ITracingService>();
            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory organizationServiceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService organizationService = organizationServiceFactory.CreateOrganizationService(workflowContext.UserId);
            
            Guid id = workflowContext.PrimaryEntityId;

            _BL_trs_quotation.CalculateTotal(organizationService, tracingService, id);
        }
    }
}