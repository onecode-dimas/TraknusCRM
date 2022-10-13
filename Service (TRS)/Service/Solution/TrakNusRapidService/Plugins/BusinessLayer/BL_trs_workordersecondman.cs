using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using TrakNusRapidService.DataLayer;

namespace TrakNusRapidService.Plugins.BusinessLayer
{
    public class BL_trs_workordersecondman
    {
        #region Constants
        private const string _classname = "BL_trs_mtar";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_trs_workordersecondman _DL_trs_workordersecondman = new DL_trs_workordersecondman();
        #endregion

        #region Privates
        private void RemoveSecondman()
        {
        }
        #endregion

        #region Publics
        public void RemoveSecondman(IOrganizationService organizationService, Guid activityId, Guid equipmentId)
        {
            QueryExpression queryExpression = new QueryExpression(_DL_trs_workordersecondman.EntityName);
            FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
            filterExpression.AddCondition("trs_activityid", ConditionOperator.Equal, activityId);
            filterExpression.AddCondition("trs_equipmentid", ConditionOperator.Equal, equipmentId);

            EntityCollection entityCollection = _DL_trs_workordersecondman.Select(organizationService, queryExpression);
            foreach (Entity entity in entityCollection.Entities)
            {
                _DL_trs_workordersecondman.Deactivate(organizationService, entity.Id);
            }
        }

        #region Forms
        #endregion

        #region Fields
        #endregion
        #endregion
    }
}
