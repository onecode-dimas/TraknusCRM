using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

using Microsoft.Xrm.Sdk;

using TrakNusRapidService.DataLayer;
using TrakNusRapidService.Helper;

namespace TrakNusRapidService.Workflow.BusinessLayer
{
    class BL_email
    {
        #region Constants
        private const string _classname = "BL_email";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_email _DL_email = new DL_email();

        private EmailAgent emailAgent = new EmailAgent();
        #endregion

        #region Privates
        #endregion

        #region Events
        public void AutomaticSend(IOrganizationService organizationService, ITracingService tracingService, Guid id)
        {
            try
            {
                Entity entity = _DL_email.Select(organizationService, id);
                if (entity.Attributes.Contains("trs_autosendcategory") && entity.GetAttributeValue<decimal>("trs_autosendcategory") > 0)
                {
                    emailAgent.Send(organizationService, id);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(ex.Message);
            }
        }
        #endregion
    }
}
