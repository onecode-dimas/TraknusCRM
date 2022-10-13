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
    public class BL_trs_quotationcommercialheader
    {
        #region Constants
        private const string _classname = "BL_trs_quotationcommercialheader";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_trs_quotationcommercialheader _DL_trs_quotationcommercialheader = new DL_trs_quotationcommercialheader();
        #endregion

        #region Privates
        #endregion

        #region Events
        public EntityCollection getAllCommercialHeader(IOrganizationService organizationService, Guid Id)
        {
            try
            {
                QueryExpression qe = new QueryExpression(_DL_trs_quotationcommercialheader.EntityName);
                qe.ColumnSet = new ColumnSet(true);

                ConditionExpression ce = new ConditionExpression();
                ce.AttributeName = "activityid";
                ce.Operator = ConditionOperator.Equal;
                ce.Values.Add(Id.ToString());

                qe.Criteria.AddCondition(ce);

                return _DL_trs_quotationcommercialheader.Select(organizationService, qe);
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException("Workflow at BL_trs_quotationcommercialheader returned error" + ex.ToString());
            }
        }

        public EntityCollection getAllRelatedCommercialHeader(IOrganizationService organizationService, Guid Id)
        {
            try
            {
                QueryExpression qe = new QueryExpression(_DL_trs_quotationcommercialheader.EntityName);
                qe.ColumnSet = new ColumnSet(true);

                ConditionExpression ce = new ConditionExpression();
                ce.AttributeName = "regardingobjectid";
                ce.Operator = ConditionOperator.Equal;
                ce.Values.Add(Id.ToString());

                qe.Criteria.AddCondition(ce);

                return _DL_trs_quotationcommercialheader.Select(organizationService, qe);
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException("Workflow at BL_trs_quotationcommercialheader returned error" + ex.ToString());
            }
        }
        #endregion
    }
}
