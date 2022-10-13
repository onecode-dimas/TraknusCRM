using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhancementCRM.Plugins.Business_Layer
{
    public class BL_cposaleseffort
    {
        #region Properties
        private string _classname = "BL_cposaleseffort";
        private string _entityname_cposaleseffort = "ittn_cposaleseffort";

        private string _entityname_approvallistcposaleseffort = "ittn_approvallistcposaleseffort";
        private string _entityname_incentive = "new_incentive";
        private string _entityname_salesorder = "salesorder";
        private string _entityname_salesorderdetail = "salesorderdetail";
        private string _entityname_systemuser = "systemuser";
        #endregion

        public void PostUpdate_ittn_cposaleseffort_approve(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        {
            bool _approvesaleseffort = _entity.GetAttributeValue<bool>("ittn_approvesaleseffort");

            if (_approvesaleseffort)
            {
                _entity = _organizationservice.Retrieve(_entityname_cposaleseffort, _entity.Id, new ColumnSet(true));

                #region UPDATE CPO SALES EFFORT
                Entity _update = new Entity(_entityname_cposaleseffort);
                _update.Id = _entity.Id;

                _update["ittn_approvesaleseffortdate"] = DateTime.Now;
                _update["ittn_approvesaleseffortby"] = new EntityReference(_entityname_systemuser, _context.UserId);
                _update["ittn_statusreason"] = new OptionSetValue(841150001);
                _update["ittn_needapprovesaleseffort"] = false;

                _organizationservice.Update(_update);
                #endregion ---

                Entity _salesorder = _organizationservice.Retrieve(_entityname_salesorder, _entity.GetAttributeValue<EntityReference>("ittn_cpo").Id, new ColumnSet(true));

                QueryExpression _queryexpression = new QueryExpression(_entityname_incentive);
                _queryexpression.ColumnSet = new ColumnSet(true);
                _queryexpression.Orders.Add(new OrderExpression("new_cpoitemnumber", OrderType.Ascending));
                _queryexpression.Criteria.AddCondition("new_cpo", ConditionOperator.Equal, _salesorder.Id);
                _queryexpression.Criteria.AddCondition("new_cpoitemnumber", ConditionOperator.NotNull);
                _queryexpression.Criteria.AddCondition("ittn_mainincentive", ConditionOperator.Null);
                EntityCollection _incentives = _organizationservice.RetrieveMultiple(_queryexpression);

                _queryexpression = new QueryExpression(_entityname_cposaleseffort);
                _queryexpression.ColumnSet = new ColumnSet(true);
                _queryexpression.Criteria.AddCondition("ittn_cpo", ConditionOperator.Equal, _salesorder.Id);
                _queryexpression.Criteria.AddCondition("ittn_statusreason", ConditionOperator.Equal, 841150001);
                EntityCollection _cposalesefforts = _organizationservice.RetrieveMultiple(_queryexpression);

                decimal _effortcontribution = _entity.GetAttributeValue<decimal>("ittn_effortcontribution");
                decimal _totaleffortcontribution = _cposalesefforts.Entities.AsEnumerable().Sum(x => x.GetAttributeValue<decimal>("ittn_effortcontribution"));
                decimal _effortcontributionformain = 100 - _totaleffortcontribution;

                foreach (var _incentive in _incentives.Entities)
                {
                    Guid _incentiveid = _incentive.Id;
                    decimal _itemnumber = _incentive.GetAttributeValue<decimal>("new_cpoitemnumber");

                    _queryexpression = new QueryExpression(_entityname_salesorderdetail);
                    _queryexpression.ColumnSet = new ColumnSet(true);
                    _queryexpression.Criteria.AddCondition("salesorderid", ConditionOperator.Equal, _salesorder.Id);
                    _queryexpression.Criteria.AddCondition("new_itemnumber", ConditionOperator.Equal, _itemnumber);
                    Entity _salesorderdetail = _organizationservice.RetrieveMultiple(_queryexpression).Entities.FirstOrDefault();

                    if (_salesorderdetail != null)
                    {
                        decimal _priceperunit = _salesorderdetail.GetAttributeValue<Money>("new_priceperunit").Value;
                        decimal _nettformain = _priceperunit * _effortcontributionformain / 100;
                        decimal _nettforchild = _priceperunit * _effortcontribution / 100;

                        #region UPDATE INCENTIVE MAIN
                        Entity _main_update = new Entity(_entityname_incentive);
                        _main_update.Id = _incentiveid;

                        _main_update["new_netincentive"] = _nettformain;
                        _main_update["new_ecsalespercentage"] = _effortcontributionformain;

                        _organizationservice.Update(_main_update);
                        #endregion ---

                        _queryexpression = new QueryExpression(_entityname_incentive);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria.AddCondition("new_cpo", ConditionOperator.Equal, _salesorder.Id);
                        _queryexpression.Criteria.AddCondition("new_cpoitemnumber", ConditionOperator.Equal, _itemnumber);
                        _queryexpression.Criteria.AddCondition("ittn_mainincentive", ConditionOperator.Equal, _incentiveid);
                        Entity _incentive_child = _organizationservice.RetrieveMultiple(_queryexpression).Entities.FirstOrDefault();

                        if (_incentive_child == null)
                        {
                            #region CREATE INCENTIVE CHILD
                            Entity _new = new Entity(_entityname_incentive);
                            _new = _incentive;
                            _new.Id = Guid.Empty;
                            _new.Attributes.Remove("new_incentiveid");

                            _new["new_netincentive"] = _nettforchild;
                            _new["new_ecsalespercentage"] = _effortcontribution;
                            _new["ittn_mainincentive"] = new EntityReference(_entityname_incentive, _incentiveid);

                            _organizationservice.Create(_new);
                            #endregion ---
                        }
                        else
                        {
                            #region UPDATE INCENTIVE CHILD
                            Entity _update_child = new Entity(_entityname_incentive);
                            _update_child.Id = _incentive_child.Id;

                            _update_child["new_netincentive"] = _nettforchild;
                            _update_child["new_ecsalespercentage"] = _effortcontribution;

                            _organizationservice.Update(_update_child);
                            #endregion ---
                        }
                    }
                }
            }
        }

        public void PostUpdate_ittn_cposaleseffort_effortcontribution(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        {
            if (_entity != null)
            {
                _entity = _organizationservice.Retrieve(_entityname_cposaleseffort, _entity.Id, new ColumnSet(true));

                #region UPDATE CPO SALES EFFORT
                Entity _update = new Entity(_entityname_cposaleseffort);
                _update.Id = _entity.Id;

                _update["ittn_approvesaleseffort"] = false;
                _update["ittn_approvesaleseffortdate"] = null;
                _update["ittn_approvesaleseffortby"] = null;
                _update["ittn_statusreason"] = null;
                _update["ittn_needapprovesaleseffort"] = true;
                _update["ittn_reqapprovesaleseffortdate"] = null;
                _update["ittn_saleseffortcurrentapprover"] = null;
                _update["ittn_saleseffortescalationdate"] = null;

                _organizationservice.Update(_update);
                #endregion ---

                #region DELETE APPROVAL LIST
                QueryExpression _queryexpression = new QueryExpression(_entityname_approvallistcposaleseffort);
                _queryexpression.ColumnSet = new ColumnSet(true);
                _queryexpression.Criteria.AddCondition("ittn_cposaleseffort", ConditionOperator.Equal, _entity.Id);
                EntityCollection _approvallistcposalesefforts = _organizationservice.RetrieveMultiple(_queryexpression);

                foreach (var _approvallistcposaleseffort in _approvallistcposalesefforts.Entities)
                {
                    _organizationservice.Delete(_entityname_approvallistcposaleseffort, _approvallistcposaleseffort.Id);
                }
                #endregion ---
            }
        }
    }
}
