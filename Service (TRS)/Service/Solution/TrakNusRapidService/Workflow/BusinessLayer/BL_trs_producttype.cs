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
    class BL_trs_producttype
    {
        #region Constants
        private const string _classname = "BL_trs_producttype";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_trs_producttype _DL_trs_producttype = new DL_trs_producttype();
        #endregion

        #region Privates
        #endregion

        #region Events
        public EntityCollection getAllProductType(IOrganizationService organizationService, Guid Id)
        {
            try
            {
                QueryExpression qe = new QueryExpression(_DL_trs_producttype.EntityName);
                qe.ColumnSet = new ColumnSet(true);

                ConditionExpression ce = new ConditionExpression();
                ce.AttributeName = "trs_producttypeid";
                ce.Operator = ConditionOperator.Equal;
                ce.Values.Add(Id.ToString());

                qe.Criteria.AddCondition(ce);

                return _DL_trs_producttype.Select(organizationService, qe);
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException("Workflow at BL_trs_producttype returned error" + ex.ToString());
            }
        }


        #endregion
    }
}
