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
    class BL_new_population
    {
        #region Constants
        private const string _classname = "BL_new_population";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_new_population _DL_new_population = new DL_new_population();
        #endregion

        #region Privates
        #endregion

        #region Events
        public EntityCollection getRecordNewPopulation(IOrganizationService organizationService, Guid Id)
        {
            try
            {
                QueryExpression qe = new QueryExpression(_DL_new_population.EntityName);
                qe.ColumnSet = new ColumnSet(true);

                ConditionExpression ce = new ConditionExpression();
                ce.AttributeName = "new_populationid";
                ce.Operator = ConditionOperator.Equal;
                ce.Values.Add(Id.ToString());

                qe.Criteria.AddCondition(ce);

                return _DL_new_population.Select(organizationService, qe);
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(ex.ToString());
            }
        }
        #endregion
    }
}
