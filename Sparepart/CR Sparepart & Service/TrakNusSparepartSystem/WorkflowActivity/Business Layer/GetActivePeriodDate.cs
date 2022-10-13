using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using TrakNusSparepartSystem.WorkflowActivity.Interface;
using Microsoft.Xrm.Sdk.Query;
using TrakNusSparepartSystem.DataLayer;

namespace TrakNusSparepartSystem.WorkflowActivity.Business_Layer
{
    public class GetActivePeriodDate
    {
        private IOrganizationService _organizationservice;
        DateTime? _startdatemarketsize;
        DateTime? _enddatemarketsize;
        int? _evaluationmarketsize;

        public DateTime? StartDateMarketSize
        {
            get
            {
                return _startdatemarketsize;
            }
        }

        public DateTime? EndDateMarketSize
        {
            get
            {
                return _enddatemarketsize;
            }
        }

        public int? EvaluationMarketSize
        {
            get
            {
                return _evaluationmarketsize;
            }
        }

        public GetActivePeriodDate(IOrganizationService _organizationserviceref)
        {
            _organizationservice = _organizationserviceref;
            _startdatemarketsize = null;
            _enddatemarketsize = null;
            _evaluationmarketsize = null;
        }

        public void Process()
        {
            FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
            _filterexpression.AddCondition("tss_isactive", ConditionOperator.Equal, true);

            QueryExpression _queryexpression = new QueryExpression("tss_matrixmarketsizeperiod");
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddFilter(_filterexpression);

            EntityCollection _matrixmarketsizeperiodcollection = _organizationservice.RetrieveMultiple(_queryexpression);

            if (_matrixmarketsizeperiodcollection.Entities.Count() > 0)
            {
                Entity _matrixmarketsizeperiod = _matrixmarketsizeperiodcollection.Entities.FirstOrDefault();

                _startdatemarketsize = _matrixmarketsizeperiod.GetAttributeValue<DateTime>("tss_startdatemarketsize");
                _enddatemarketsize = _matrixmarketsizeperiod.GetAttributeValue<DateTime>("tss_enddatemarketsize");
                _evaluationmarketsize = _matrixmarketsizeperiod.GetAttributeValue<int>("tss_evaluationmarketsize");
            }
            else
            {
                throw new InvalidPluginExecutionException("Active Period not found ! Please make sure 1 period is activate in Matrix Market Size Period !");
            }
        }
    }
}
