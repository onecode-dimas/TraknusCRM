using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhancementCRM.Plugins.Business_Layer
{
    public class BL_ittn_matrixcpoconditiontypebyproduct
    {
        #region Properties
        private string _classname = "BL_ittn_matrixcpoconditiontypebyproduct";
        private string _entityname_matrixcpoconditiontypebyproduct = "ittn_matrixcpoconditiontypebyproduct";

        private string _entityname_systemuser = "systemuser";
        #endregion

        public void PreCreate_ittn_matrixcpoconditiontypebyproduct(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        {
            try
            {
                Guid _unitgroup = _entity.GetAttributeValue<EntityReference>("ittn_unitgroup").Id;
                Guid _conditiontype = _entity.GetAttributeValue<EntityReference>("ittn_conditiontype").Id;
                Guid _model = _entity.GetAttributeValue<EntityReference>("ittn_model").Id;

                QueryExpression _queryexpression = new QueryExpression(_entityname_matrixcpoconditiontypebyproduct);
                _queryexpression.ColumnSet = new ColumnSet(true);
                _queryexpression.Criteria.AddCondition("ittn_unitgroup", ConditionOperator.Equal, _unitgroup);
                _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _conditiontype);
                _queryexpression.Criteria.AddCondition("ittn_model", ConditionOperator.Equal, _model);
                EntityCollection _matrixcpoconditiontypebyproducts = _organizationservice.RetrieveMultiple(_queryexpression);

                if (_matrixcpoconditiontypebyproducts.Entities.Count() > 0)
                    throw new InvalidPluginExecutionException("Matrix Sales Order ( CPO ) Condition Type with this Unit Group, Condition Type, and Model already exist !");
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".PreCreate_ittn_matrixcpoconditiontypebyproduct: " + ex.Message.ToString());
            }
        }

        public void PreUpdate_ittn_matrixcpoconditiontypebyproduct(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        {
            try
            {
                Guid _unitgroup = new Guid();
                Guid _conditiontype = new Guid();
                Guid _model = new Guid();
                Entity _matrixcpoconditiontypebyproduct_existing = _organizationservice.Retrieve(_entityname_matrixcpoconditiontypebyproduct, _entity.Id, new ColumnSet(true));

                if (_entity.Attributes.Contains("ittn_unitgroup"))
                    _unitgroup = _entity.GetAttributeValue<EntityReference>("ittn_unitgroup").Id;
                else
                    _unitgroup = _matrixcpoconditiontypebyproduct_existing.GetAttributeValue<EntityReference>("ittn_unitgroup").Id;

                if (_entity.Attributes.Contains("ittn_conditiontype"))
                    _conditiontype = _entity.GetAttributeValue<EntityReference>("ittn_conditiontype").Id;
                else
                    _conditiontype = _matrixcpoconditiontypebyproduct_existing.GetAttributeValue<EntityReference>("ittn_conditiontype").Id;

                if (_entity.Attributes.Contains("ittn_model"))
                    _model = _entity.GetAttributeValue<EntityReference>("ittn_model").Id;
                else
                    _model = _matrixcpoconditiontypebyproduct_existing.GetAttributeValue<EntityReference>("ittn_model").Id;

                if (_unitgroup != new Guid() && _conditiontype != new Guid() && _model != new Guid())
                {
                    QueryExpression _queryexpression = new QueryExpression(_entityname_matrixcpoconditiontypebyproduct);
                    _queryexpression.ColumnSet = new ColumnSet(true);
                    _queryexpression.Criteria.AddCondition("ittn_matrixcpoconditiontypebyproductid", ConditionOperator.NotEqual, _matrixcpoconditiontypebyproduct_existing.Id);
                    _queryexpression.Criteria.AddCondition("ittn_unitgroup", ConditionOperator.Equal, _unitgroup);
                    _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _conditiontype);
                    _queryexpression.Criteria.AddCondition("ittn_model", ConditionOperator.Equal, _model);
                    EntityCollection _matrixcpoconditiontypebyproducts = _organizationservice.RetrieveMultiple(_queryexpression);

                    if (_matrixcpoconditiontypebyproducts.Entities.Count() > 0)
                        throw new InvalidPluginExecutionException("Matrix Sales Order ( CPO ) Condition Type with this Unit Group, Condition Type, and Model already exist !");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".PreUpdate_ittn_matrixcpoconditiontypebyproduct: " + ex.Message.ToString());
            }
        }
    }
}
