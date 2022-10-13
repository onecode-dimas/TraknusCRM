using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace TrakNusRapidService.Workflow
{

    public sealed class Mechanic_RemoteWipe : CodeActivity
    {
        #region Depedencies
        private BusinessLayer.BL_equipment _BL_equipment = new BusinessLayer.BL_equipment();
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

            Guid id = workflowContext.PrimaryEntityId;
            Guid user = workflowContext.UserId;
            IOrganizationService organizationService = organizationServiceFactory.CreateOrganizationService(user);

            _BL_equipment.RemoteWipe(organizationService, tracingService, id);
        }
    }
}
