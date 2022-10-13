using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;
using System.Activities;

namespace EnhancementCRM.WorkflowActivity.Business_Layer
{
    public class BL_serviceappointment
    {
        #region Constants
        private string _classname = "BL_serviceappointment";
        private string _entityname = "serviceappointment";
        #endregion

        public void Release(IOrganizationService organizationService, ITracingService tracingService, Guid id)
        {
            try
            {
                UpdateStatusCodeReleased(organizationService, id);
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(_classname + ".Release: " + ex.Message.ToString());
            }
        }

        public void UpdateStatusCodeReleased(IOrganizationService organizationService, Guid id)
        {
            try
            {
                EntityReference entityReference = new EntityReference(_entityname, id);
                OrganizationRequest organizationRequest = new OrganizationRequest();
                organizationRequest.RequestName = "SetState";
                organizationRequest["EntityMoniker"] = entityReference;
                organizationRequest["State"] = new OptionSetValue(0);
                organizationRequest["Status"] = new OptionSetValue(2);
                organizationService.Execute(organizationRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".UpdateStatusCodeReleased : " + ex.Message);
            }
        }
    }   
}
