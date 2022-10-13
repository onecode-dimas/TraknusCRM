// <copyright file="ContractPreventive_OnCreate.cs" company="">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author></author>
// <date>1/29/2015 9:17:58 AM</date>
// <summary>Implements the ContractPreventive_OnCreate Workflow Activity.</summary>
namespace TrakNusRapidService.Workflow
{
    using System;
    using System.Activities;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;

    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;

    public sealed class ContractPreventive_OnCreate : CodeActivity
    {
        #region Depedencies
        private BusinessLayer.BL_trs_contractpreventive _BL_trs_contractpreventive = new BusinessLayer.BL_trs_contractpreventive();
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

            _BL_trs_contractpreventive.GenerateActivity(organizationService, tracingService, id);
        }
    }
}