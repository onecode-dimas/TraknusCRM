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
    class BL_trs_workordersupportingmaterial
    {
        #region Constants
        private const string _classname = "BL_trs_workordersupportingmaterial";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_trs_workordersupportingmaterial _DL_trs_workordersupportingmaterial = new DL_trs_workordersupportingmaterial();
        #endregion

        #region Privates
        #endregion

        #region Events
        public EntityCollection getAllWoSupportingMaterial(IOrganizationService organizationService, Guid Id)
        {
            try
            {
                QueryExpression qe = new QueryExpression(_DL_trs_workordersupportingmaterial.EntityName);
                qe.ColumnSet = new ColumnSet(true);

                ConditionExpression ce = new ConditionExpression();
                ce.AttributeName = "serviceid";
                ce.Operator = ConditionOperator.Equal;
                ce.Values.Add(Id.ToString());

                qe.Criteria.AddCondition(ce);

                return _DL_trs_workordersupportingmaterial.Select(organizationService, qe);
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(ex.ToString());
            }
        }
        #endregion
    }
}
