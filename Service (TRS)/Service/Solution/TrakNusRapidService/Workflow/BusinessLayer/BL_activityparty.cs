using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using TrakNusRapidService.DataLayer;


namespace TrakNusRapidService.Workflow.BusinessLayer
{
    class BL_activityparty
    {
        #region Constants
        private const string _classname = "BL_activityparty";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_activityparty _DL_activityparty = new DL_activityparty();
        #endregion

        #region Privates
        #endregion

        #region Events
        public EntityCollection getResourcesActivityParty(IOrganizationService organizationService, Guid Id)
        {
            try
            {
                QueryExpression qe = new QueryExpression(_DL_activityparty.EntityName);
                qe.ColumnSet = new ColumnSet(true);

                ConditionExpression conId = new ConditionExpression();
                conId.AttributeName = "activityid";
                conId.Operator = ConditionOperator.Equal;
                conId.Values.Add(Id.ToString());

                ConditionExpression typeMask = new ConditionExpression();
                typeMask.AttributeName = "participationtypemask";
                typeMask.Operator = ConditionOperator.Equal;
                typeMask.Values.Add("10");
                //Activity Party Types = 10 (Specifies a resource/ServiceAppointment.Resources)
                ////http://msdn.microsoft.com/en-us/library/gg328549.aspx
                
                qe.Criteria.AddCondition(conId);
                qe.Criteria.AddCondition(typeMask);

                return _DL_activityparty.Select(organizationService, qe);
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(ex.ToString());
            }
        }

        public EntityCollection getResourcesActivityPartyGlobal(IOrganizationService organizationService, Guid Id, Int32 typeMaskVal)
        {
            try
            {
                QueryExpression qe = new QueryExpression(_DL_activityparty.EntityName);
                qe.ColumnSet = new ColumnSet(true);

                ConditionExpression conId = new ConditionExpression();
                conId.AttributeName = "activityid";
                conId.Operator = ConditionOperator.Equal;
                conId.Values.Add(Id.ToString());

                ConditionExpression typeMask = new ConditionExpression();
                typeMask.AttributeName = "participationtypemask";
                typeMask.Operator = ConditionOperator.Equal;
                typeMask.Values.Add(typeMaskVal);
                //Activity Party Types = 10 (Specifies a resource/ServiceAppointment.Resources)
                //Activity Party Types = 11 (Specifies a Specifies a customer.)

                ////http://msdn.microsoft.com/en-us/library/gg328549.aspx

                qe.Criteria.AddCondition(conId);
                qe.Criteria.AddCondition(typeMask);

                return _DL_activityparty.Select(organizationService, qe);
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(ex.ToString());
            }
        }
        
        #endregion
    }
}
