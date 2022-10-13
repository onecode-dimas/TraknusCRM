using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Client.Services;

namespace CalculateRTG.BusinessLayer
{
    public class BL_task
    {
        public EntityCollection CheckingTaskRecord(OrganizationService organizationService, Guid _id)
        {
            DataLayer.DL_task tsk = new DataLayer.DL_task();

            QueryExpression queryExpression = new QueryExpression(tsk.EntityName);
            queryExpression.ColumnSet.AddColumns("activityid", "trs_operationid", "trs_totalrtg");
            queryExpression.Criteria.AddCondition("trs_operationid", ConditionOperator.Equal, _id);

            return tsk.Select(organizationService, queryExpression);
        }
    }
}
