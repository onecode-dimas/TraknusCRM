using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using TrakNusRapidService.DataLayer;


namespace TrakNusRapidService.Workflow.BusinessLayer
{
    public class BL_trs_taskassignmenthistory
    {
        #region Constants
        private const string _classname = "BL_trs_taskassignmenthistory";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_trs_taskassignmenthistory _DL_trs_taskassignmenthistory = new DL_trs_taskassignmenthistory();
        #endregion

        #region Events
        public EntityCollection getAllTaskAssignmentHistory(IOrganizationService organizationService, Guid Id)
        {
            try
            {
                QueryExpression qe = new QueryExpression(_DL_trs_taskassignmenthistory.EntityName);
                qe.ColumnSet = new ColumnSet(true);

                ConditionExpression ce = new ConditionExpression();
                ce.AttributeName = "serviceid";
                ce.Operator = ConditionOperator.Equal;
                ce.Values.Add(Id.ToString());

                qe.Criteria.AddCondition(ce);

                return _DL_trs_taskassignmenthistory.Select(organizationService, qe);
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(ex.ToString());
            }
        }
        #endregion
    }
}
