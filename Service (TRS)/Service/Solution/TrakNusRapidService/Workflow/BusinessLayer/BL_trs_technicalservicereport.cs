using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using TrakNusRapidService.DataLayer;

namespace TrakNusRapidService.Workflow.BusinessLayer
{
    public class BL_trs_technicalservicereport
    {
        #region Constants
        private const string _classname = "BL_trs_technicalservicereport";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_trs_technicalservicereport _DL_trs_technicalservicereport = new DL_trs_technicalservicereport();
        #endregion

        #region Events
        public EntityCollection getAllTechnicalServiceReport(IOrganizationService service, Guid id)
        {
            EntityCollection relatedEntities = null;
            try
            {

                QueryExpression qe = new QueryExpression(_DL_trs_technicalservicereport.EntityName);
                qe.ColumnSet = new ColumnSet(new string[] { "trs_tsrnumber", "trs_equipment", "trs_producttype"});

                ConditionExpression cetl = new ConditionExpression();
                cetl.AttributeName = "trs_technicalservicereportid";
                cetl.Operator = ConditionOperator.Equal;
                cetl.Values.Add(id.ToString());

                qe.Criteria.AddCondition(cetl);

                return _DL_trs_technicalservicereport.Select(service, qe);
            }
            catch { }

            return relatedEntities;
        }
        #endregion
    }
}
