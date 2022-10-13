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
    class BL_task
    {
        #region Constants
        private const string _classname = "BL_task";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_task _DL_task = new DL_task();
        #endregion

        #region Privates
        #endregion

        #region Events
        public EntityCollection getAllCommercialHeader(IOrganizationService organizationService, Guid Id)
        {
            try
            {
                QueryExpression qe = new QueryExpression(_DL_task.EntityName);
                qe.ColumnSet = new ColumnSet(true);

                ConditionExpression ce = new ConditionExpression();
                ce.AttributeName = "activityid";
                ce.Operator = ConditionOperator.Equal;
                ce.Values.Add(Id.ToString());

                qe.Criteria.AddCondition(ce);
                
                return _DL_task.Select(organizationService, qe);
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException("Workflow at BL_task returned error" + ex.ToString());
            }
        }

        public EntityCollection getAllRelatedCommercialHeader(IOrganizationService organizationService, Guid Id)
        {
            try
            {
                QueryExpression qe = new QueryExpression(_DL_task.EntityName);
                qe.ColumnSet = new ColumnSet(true);

                ConditionExpression ce = new ConditionExpression();
                ce.AttributeName = "trs_operationid";
                ce.Operator = ConditionOperator.Equal;
                ce.Values.Add(Id.ToString());

                qe.Criteria.AddCondition(ce);

                return _DL_task.Select(organizationService, qe);
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(ex.ToString());
            }
        }

        public EntityCollection getAllRelatedCommercialHeaderByField(IOrganizationService organizationService, Guid Id, string fieldName )
        {
            try
            {
                QueryExpression qe = new QueryExpression(_DL_task.EntityName);
                qe.ColumnSet = new ColumnSet(true);

                ConditionExpression ce = new ConditionExpression();
                ce.AttributeName = fieldName;
                ce.Operator = ConditionOperator.Equal;
                ce.Values.Add(Id.ToString());

                qe.Criteria.AddCondition(ce);

                return _DL_task.Select(organizationService, qe);
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(ex.ToString());
            }
        }
        
        #endregion
    }
}
