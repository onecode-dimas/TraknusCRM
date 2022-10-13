using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using TrakNusRapidService.DataLayer;

namespace TrakNusRapidService.Workflow.BusinessLayer
{
    class BL_trs_masterpart
    {
        #region Constants
        private const string _classname = "BL_trs_masterpart";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_trs_masterpart _DL_trs_masterpart = new DL_trs_masterpart();
        #endregion

        #region Events
        public EntityCollection getPartMasterbyTaskId(IOrganizationService organizationService, Guid Id)
        {
            EntityCollection partMaster = null;
            try
            {
                QueryExpression qe = new QueryExpression(_DL_trs_masterpart.EntityName);
                qe.ColumnSet.AddColumns("trs_name","trs_PartDescription");

                ConditionExpression ce = new ConditionExpression();
                ce.AttributeName = _DL_trs_masterpart.trs_Task;
                ce.Operator = ConditionOperator.Equal;
                ce.Values.Add(Id);

                qe.Criteria.AddCondition(ce);

                partMaster = _DL_trs_masterpart.Select(organizationService, qe);
            }
            catch (Exception ex)
            {
                throw new Exception("Error BL_trs_masterpart.Select : " + ex.Message, ex.InnerException);
            }
            return partMaster;
        }
        #endregion
    }
}
