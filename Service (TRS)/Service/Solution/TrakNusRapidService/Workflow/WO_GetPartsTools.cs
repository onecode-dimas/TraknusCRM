// <copyright file="WO_GetPartsTools.cs" company="">
// Copyright (c) 2014 All Rights Reserved
// </copyright>
// <author></author>
// <date>11/19/2014 2:05:27 PM</date>
// <summary>Implements the WO_GetPartsTools Workflow Activity.</summary>
namespace TrakNusRapidService.Workflow
{
    using System;
    using System.Activities;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;

    public sealed class WO_GetPartsTools : CodeActivity
    {
        #region Depedencies
        private BusinessLayer.BL_serviceappointment _BL_serviceappointment = new BusinessLayer.BL_serviceappointment();
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

            _BL_serviceappointment.GetPartsTools(organizationService, tracingService, id);
        }
    }
}