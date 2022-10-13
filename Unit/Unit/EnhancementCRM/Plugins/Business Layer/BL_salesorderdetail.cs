using EnhancementCRM.HelperUnit;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhancementCRM.Plugins.Business_Layer
{
    public class BL_salesorderdetail
    {
        #region Properties
        private string _classname = "BL_salesorderdetail";

        private string _entityname_deliveryplant = "new_deliveryplant";
        private string _entityname_masterconditiontype = "ittn_masterconditiontype";
        private string _entityname_masterroute = "ittn_masterroute";
        private string _entityname_quote = "quote";
        private string _entityname_quotedetail = "quotedetail";
        private string _entityname_quoteconditiontype = "ittn_quoteconditiontype";
        private string _entityname_salesorder = "salesorder";
        private string _entityname_salesorderdetail = "salesorderdetail";
        private string _entityname_salesorderconditiontype = "ittn_cpoconditiontype";
        private string _entityname_matrixsalesorderconditiontypebyproduct = "ittn_matrixcpoconditiontypebyproduct";
        private string _entityname_systemuser = "systemuser";
        private string _entityname_transactioncurrency = "transactioncurrency";
        private string _entityname_transactioncurrency_code = "IDR";

        private const int STATUS_NOTASSIGN = 841150000;
        private const int STATUS_ACTIVE = 841150001;
        private const int STATUS_INACTIVE = 841150002;
        private const int STATUS_INPROGRESS = 841150003;
        #endregion

        public void PostCreate_salesorderdetail(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        {
            try
            {
                Guid _salesorderid = _entity.Attributes.Contains("salesorderid") ? _entity.GetAttributeValue<EntityReference>("salesorderid").Id : new Guid();

                if (_salesorderid != new Guid())
                {
                    Entity _salesorder = _organizationservice.Retrieve(_entityname_salesorder, _salesorderid, new ColumnSet(true));
                    //Entity _salesorderDetail = _organizationservice.Retrieve(_entityname_salesorderdetail, _entity.Id, new ColumnSet(true));
                    Guid _quoteid = _salesorder.Attributes.Contains("quoteid") ? _salesorder.GetAttributeValue<EntityReference>("quoteid").Id : new Guid();

                    if (_quoteid != new Guid())
                    {
                        decimal? _null_decimal = null;
                        decimal? _itemnumber = _entity.Attributes.Contains("new_itemnumber") ? _entity.GetAttributeValue<decimal>("new_itemnumber") : _null_decimal;
                        decimal? _itemnumberhistory = _entity.Attributes.Contains("new_itemnumberhistory") ? _entity.GetAttributeValue<decimal>("new_itemnumberhistory") : _null_decimal;

                        if (_itemnumber != _null_decimal && _itemnumberhistory != _null_decimal)
                        {
                            Guid _productid = _entity.GetAttributeValue<EntityReference>("productid").Id;
                            Guid _unitgroupid = new Guid();
                            Entity _quote = _organizationservice.Retrieve(_entityname_quote, _quoteid, new ColumnSet(true));
                            _unitgroupid = _quote.GetAttributeValue<EntityReference>("new_unitgroup").Id;

                            QueryExpression _queryexpression = new QueryExpression(_entityname_quoteconditiontype);
                            _queryexpression.ColumnSet = new ColumnSet(true);
                            _queryexpression.Criteria.AddCondition("ittn_quote", ConditionOperator.Equal, _quoteid);
                            _queryexpression.Criteria.AddCondition("ittn_itemnumber", ConditionOperator.Equal, _itemnumberhistory);
                            EntityCollection _quoteconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                            foreach (var _quoteconditiontype in _quoteconditiontypes.Entities)
                            {
                                Entity _masterconditiontype = _organizationservice.Retrieve(_entityname_masterconditiontype, _quoteconditiontype.GetAttributeValue<EntityReference>("ittn_conditiontype").Id, new ColumnSet(true));
                                Entity _salesorderconditiontype = new Entity(_entityname_salesorderconditiontype);

                                // CHECK 4 COMPONENT
                                bool _isconditiontypemandatory = false;
                                string _masterconditiontype_code = _masterconditiontype.GetAttributeValue<string>("ittn_code");
                                if (_masterconditiontype_code == "ZCB0" ||
                                    _masterconditiontype_code == "ZVCR" ||
                                    _masterconditiontype_code == "ZFAT" ||
                                    _masterconditiontype_code == "ZO99")
                                {
                                    _isconditiontypemandatory = true;
                                }

                                if ((_isconditiontypemandatory && _quoteconditiontype.GetAttributeValue<OptionSetValue>("ittn_statusreason").Value == STATUS_ACTIVE) || (!_isconditiontypemandatory))
                                {
                                    _salesorderconditiontype["ittn_amount"] = new Money(_quoteconditiontype.GetAttributeValue<Money>("ittn_amount").Value);
                                    _salesorderconditiontype["ittn_conditiontype"] = new EntityReference(_entityname_masterconditiontype, _masterconditiontype.Id);
                                    _salesorderconditiontype["ittn_name"] = _masterconditiontype.GetAttributeValue<string>("ittn_description");
                                    _salesorderconditiontype["ittn_cpo"] = new EntityReference(_entityname_salesorder, _salesorderid);
                                    _salesorderconditiontype["ittn_itemnumber"] = _itemnumber;
                                    _salesorderconditiontype["transactioncurrencyid"] = new EntityReference(_entityname_transactioncurrency, Globalization.GetTransactionCurrencyID(_organizationservice,_entityname_transactioncurrency_code));

                                    _organizationservice.Create(_salesorderconditiontype);
                                }
                            }

                            _queryexpression = new QueryExpression(_entityname_quotedetail);
                            _queryexpression.ColumnSet = new ColumnSet(true);
                            _queryexpression.Criteria.AddCondition("quoteid", ConditionOperator.Equal, _quoteid);
                            _queryexpression.Criteria.AddCondition("new_itemnumber", ConditionOperator.Equal, _itemnumberhistory);
                            Entity _quotedetail = _organizationservice.RetrieveMultiple(_queryexpression).Entities.FirstOrDefault();

                            if (_quotedetail != null)
                            {
                                Entity _row = new Entity(_entityname_salesorderdetail);
                                _row.Id = _entity.Id;

                                if (_quotedetail.Attributes.Contains("ittn_foc")) { _row.Attributes.Add("ittn_foc", _quotedetail.GetAttributeValue<bool>("ittn_foc")); }

                                _organizationservice.Update(_row);

                                //if (!_salesorder.Attributes.Contains("new_unittobeoperatedin"))
                                //{
                                //    Entity _row_salesorder = new Entity(_entityname_salesorder);
                                //    _row_salesorder.Id = _salesorder.Id;

                                //    if (_quotedetail.Attributes.Contains("ittn_shippingpoint"))
                                //    {
                                //        Entity _shippingpoint = _organizationservice.Retrieve(_entityname_masterroute, _quote.GetAttributeValue<EntityReference>("ittn_shippingpoint").Id, new ColumnSet(true));
                                //        _row_salesorder["new_unittobeoperatedin"] = _shippingpoint.GetAttributeValue<string>("ittn_route");
                                //        _row_salesorder["new_adressmap"] = _quote.GetAttributeValue<string>("ittn_shippingaddress");
                                //    }

                                //    if (_quotedetail.Attributes.Contains("ittn_deliveryplanbranch"))
                                //    {
                                //        Entity _deliveryplant = _organizationservice.Retrieve(_entityname_deliveryplant, _quote.GetAttributeValue<EntityReference>("ittn_deliveryplanbranch").Id, new ColumnSet(true));

                                //        _row_salesorder["new_unittobeoperatedin"] = _deliveryplant.GetAttributeValue<string>("new_description");
                                //    }

                                //    _organizationservice.Update(_row_salesorder);

                                //}

                                #region Copy Total Extended Amount from Quote Detail
                                Entity _CPODetailUpdate = new Entity(_entityname_salesorderdetail);
                                _CPODetailUpdate.Id = _entity.Id;

                                _CPODetailUpdate["ittn_totalextendedamount"] = new Money(_quotedetail.GetAttributeValue<Money>("ittn_totalextendedamount").Value);
                                _organizationservice.Update(_CPODetailUpdate);
                                #endregion
                            }

                            //_queryexpression = new QueryExpression(_entityname_matrixsalesorderconditiontypebyproduct);
                            //_queryexpression.ColumnSet = new ColumnSet(true);
                            //_queryexpression.Criteria.AddCondition("ittn_unitgroup", ConditionOperator.Equal, _unitgroupid);
                            //_queryexpression.Criteria.AddCondition("ittn_model", ConditionOperator.Equal, _productid);

                            //EntityCollection _matrixsalesorderconditiontypebyproducts = _organizationservice.RetrieveMultiple(_queryexpression);

                            //foreach (var _matrixsalesorderconditiontypebyproduct in _matrixsalesorderconditiontypebyproducts.Entities)
                            //{
                            //    Entity _masterconditiontype = _organizationservice.Retrieve(_entityname_masterconditiontype, _matrixsalesorderconditiontypebyproduct.GetAttributeValue<EntityReference>("ittn_conditiontype").Id, new ColumnSet(true));
                            //    Entity _salesorderconditiontype = new Entity(_entityname_salesorderconditiontype);

                            //    _salesorderconditiontype["ittn_amount"] = new Money(_matrixsalesorderconditiontypebyproduct.GetAttributeValue<Money>("ittn_amount").Value);
                            //    _salesorderconditiontype["ittn_conditiontype"] = new EntityReference(_entityname_masterconditiontype, _masterconditiontype.Id);
                            //    _salesorderconditiontype["ittn_name"] = _masterconditiontype.GetAttributeValue<string>("ittn_description");
                            //    _salesorderconditiontype["ittn_cpo"] = new EntityReference(_entityname_salesorder, _salesorderid);
                            //    _salesorderconditiontype["ittn_itemnumber"] = _itemnumber;
                            //    _salesorderconditiontype["transactioncurrencyid"] = new EntityReference(_entityname_transactioncurrency, new Guid(_entityname_transactioncurrencyid));

                            //    _organizationservice.Create(_salesorderconditiontype);
                            //}
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".PostCreate_salesorderDetail: " + ex.Message.ToString());
            }
        }

    }
}
