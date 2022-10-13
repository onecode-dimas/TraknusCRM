using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using TrakNusRapidService.DataLayer;
using TrakNusRapidService.Helper;

namespace TrakNusRapidService.Workflow.BusinessLayer
{
    class BL_equipment
    {
        #region Constants
        private const string _classname = "BL_equipment";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_equipment _DL_new_population = new DL_equipment();
        #endregion

        #region Privates
        #endregion

        #region Events
        public void ResetPassword(IOrganizationService organizationService, ITracingService tracingService, Guid admin, Guid id)
        {
            try
            {
                FMobile _fMobile = new FMobile(organizationService);
                _fMobile.RequestNewPassword(organizationService, admin, id);
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(ex.Message);
            }
        }

        public void RemoteWipe(IOrganizationService organizationService, ITracingService tracingService, Guid id)
        {
            try
            {
                FMobile _fMobile = new FMobile(organizationService);
                _fMobile.RemoteWipe(organizationService, id);
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(ex.Message);
            }
        }
        #endregion
    }
}
