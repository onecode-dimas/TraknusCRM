using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhancementCRM.Plugins.Business_Layer
{
    public class BL_salesorder
    {
        #region Properties
        private string _classname = "BL_salesorder";

        private string _entityname_deliveryplant = "new_deliveryplant";
        private string _entityname_masterconditiontype = "ittn_masterconditiontype";
        private string _entityname_masterroute = "ittn_masterroute";
        private string _entityname_paymentterm = "new_paymentterm";
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
        #endregion

        public void PreCreate_salesorder_checkshippingpoint(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        {
            try
            {
                //Guid _quoteid = _entity.Attributes.Contains("quoteid") ? _entity.GetAttributeValue<EntityReference>("quoteid").Id : new Guid();

                //if (_quoteid != new Guid())
                //{
                //    QueryExpression _queryexpression = new QueryExpression(_entityname_quotedetail);
                //    _queryexpression.ColumnSet = new ColumnSet(true);
                //    _queryexpression.Criteria.AddCondition("quoteid", ConditionOperator.Equal, _quoteid);
                //    _queryexpression.Criteria.AddCondition("ittn_shippingpoint", ConditionOperator.Null);
                //    EntityCollection _quotedetails = _organizationservice.RetrieveMultiple(_queryexpression);

                //    if (_quotedetails.Entities.Count() > 0)
                //    {
                //        int _count = _quotedetails.Entities.Count();

                //        throw new InvalidPluginExecutionException("There are " + _count.ToString() + " Quotation Details do not have shipping point !");
                //    }
                //}

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".PreCreate_salesorder_checkshippingpoint: " + ex.Message.ToString());
            }
        }

        public void PostCreate_salesorder(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        {
            try
            {
                Guid _quoteid = _entity.Attributes.Contains("quoteid") ? _entity.GetAttributeValue<EntityReference>("quoteid").Id : new Guid();

                if (_quoteid != new Guid())
                {
                    Entity _quote = _organizationservice.Retrieve(_entityname_quote, _quoteid, new ColumnSet(true));

                    if (_quote != null)
                    {
                        Entity _row = new Entity(_entityname_salesorder);
                        _row.Id = _entity.Id;

                        if (_quote.Attributes.Contains("ittn_project"))
                        {
                            bool _project = _quote.GetAttributeValue<bool>("ittn_project");
                            _row["ittn_project"] = _project;
                        }

                        if (_quote.Attributes.Contains("ittn_retention"))
                        {
                            bool _retention = _quote.GetAttributeValue<bool>("ittn_retention");
                            _row["ittn_projectretention"] = _retention;
                        }

                        //if (_quote.Attributes.Contains("ittn_isshippingcentralized"))
                        //{
                        //    bool _isshippingcentralized = _quote.GetAttributeValue<bool>("ittn_isshippingcentralized");
                        //    _row["ittn_isshippingcentralized"] = _isshippingcentralized;
                        //}

                        if (_quote.Attributes.Contains("ittn_retentionpercentage")) { _row["ittn_retention"] = _quote.GetAttributeValue<decimal>("ittn_retentionpercentage"); }
                        if (_quote.Attributes.Contains("ittn_paymenttermretention")) { _row["ittn_paymenttermretention"] = new EntityReference(_entityname_paymentterm, _quote.GetAttributeValue<EntityReference>("ittn_paymenttermretention").Id); }

                        //if (_quote.Attributes.Contains("ittn_deliveryoption")) { _row["ittn_deliveryoption"] = new OptionSetValue(_quote.GetAttributeValue<OptionSetValue>("ittn_deliveryoption").Value); }

                        if (_quote.Attributes.Contains("ittn_shippingpoint"))
                        {
                            Entity _shippingpoint = _organizationservice.Retrieve(_entityname_masterroute, _quote.GetAttributeValue<EntityReference>("ittn_shippingpoint").Id, new ColumnSet(true));
                            //_row["ittn_shippingpoint"] = new EntityReference(_entityname_masterroute, _shippingpoint.Id);
                            _row["new_unittobeoperatedin"] = _shippingpoint.GetAttributeValue<string>("ittn_route");
                            _row["new_adressmap"] = _quote.GetAttributeValue<string>("ittn_shippingaddress");
                        }

                        if (_quote.Attributes.Contains("ittn_deliverybranch"))
                        {
                            Entity _deliveryplant = _organizationservice.Retrieve(_entityname_deliveryplant, _quote.GetAttributeValue<EntityReference>("ittn_deliverybranch").Id, new ColumnSet(true));

                            _row["ittn_deliverybranch"] = new EntityReference(_entityname_deliveryplant, _deliveryplant.Id);
                            _row["new_unittobeoperatedin"] = _deliveryplant.GetAttributeValue<string>("new_description");
                        }

                        _organizationservice.Update(_row);
                    }

                }

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".PostCreate_salesorder: " + ex.Message.ToString());
            }
        }

        public void PostUpdate_salesorder(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        {
            try
            {
                _entity = _organizationservice.Retrieve(_entityname_salesorder, _entity.Id, new ColumnSet(true));

                Guid _quoteid = _entity.Attributes.Contains("quoteid") ? _entity.GetAttributeValue<EntityReference>("quoteid").Id : new Guid();

                if (_quoteid != new Guid())
                {
                    QueryExpression _queryexpression = new QueryExpression(_entityname_quotedetail);
                    _queryexpression.ColumnSet = new ColumnSet(true);
                    _queryexpression.Criteria.AddCondition("quoteid", ConditionOperator.Equal, _quoteid);
                    EntityCollection _quotedetails = _organizationservice.RetrieveMultiple(_queryexpression);

                    foreach (var _quotedetail in _quotedetails.Entities)
                    {
                        int _deliveryoption = _quotedetail.GetAttributeValue<OptionSetValue>("new_deliveryoption").Value;
                        Guid _shippingpointid = _quotedetail.Attributes.Contains("ittn_shippingpoint") ? _quotedetail.GetAttributeValue<EntityReference>("ittn_shippingpoint").Id : new Guid();
                        Guid _deliveryplanbranchid = _quotedetail.Attributes.Contains("ittn_deliveryplanbranch") ? _quotedetail.GetAttributeValue<EntityReference>("ittn_deliveryplanbranch").Id : new Guid();

                        if ((_shippingpointid != new Guid() && _shippingpointid != null) || (_deliveryplanbranchid != new Guid() && _deliveryplanbranchid != null))
                        {
                            decimal _itemnumber = _quotedetail.GetAttributeValue<decimal>("new_itemnumber");

                            _queryexpression = new QueryExpression(_entityname_salesorderdetail);
                            _queryexpression.ColumnSet = new ColumnSet(true);
                            _queryexpression.Criteria.AddCondition("salesorderid", ConditionOperator.Equal, _entity.Id);
                            _queryexpression.Criteria.AddCondition("new_itemnumberhistory", ConditionOperator.Equal, _itemnumber);
                            EntityCollection _salesorderdetails = _organizationservice.RetrieveMultiple(_queryexpression);

                            foreach (var _salesorderdetail in _salesorderdetails.Entities)
                            {
                                Entity _row = new Entity(_entityname_salesorderdetail);
                                _row.Id = _salesorderdetail.Id;

                                //_row["ittn_deliveryoption"] = new OptionSetValue(_deliveryoption);

                                if (_shippingpointid != new Guid())
                                {
                                    Entity _shippingpoint = _organizationservice.Retrieve(_entityname_masterroute, _shippingpointid, new ColumnSet(true));
                                    _row.Attributes.Add("ittn_route", new EntityReference(_entityname_masterroute, _shippingpointid));
                                    //_row.Attributes.Add("ittn_shippingaddress", _shippingpoint.GetAttributeValue<string>("ittn_route"));
                                }

                                if (_deliveryplanbranchid != new Guid()) _row.Attributes.Add("ittn_deliveryplanbranch", new EntityReference(_entityname_deliveryplant, _deliveryplanbranchid));

                                //_row["ittn_route"] = new EntityReference(_entityname_masterroute, _shippingpointid);

                                _organizationservice.Update(_row);
                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".PostUpdate_salesorder_ShippingPoint: " + ex.Message.ToString());
            }
        }

        //public void PostCreate_salesorder(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        //{
        //    try
        //    {
        //        Guid _quoteid = _entity.Attributes.Contains("quoteid") ? _entity.GetAttributeValue<EntityReference>("quoteid").Id : new Guid();

        //        if (_quoteid != new Guid())
        //        {
        //            Guid _unitgroupid = new Guid();
        //            Entity _quote = _organizationservice.Retrieve(_entityname_quote, _quoteid, new ColumnSet(true));
        //            _unitgroupid = _quote.GetAttributeValue<EntityReference>("new_unitgroup").Id;

        //            QueryExpression _queryexpression = new QueryExpression(_entityname_quoteconditiontype);
        //            _queryexpression.ColumnSet = new ColumnSet(true);
        //            _queryexpression.Criteria.AddCondition("ittn_quote", ConditionOperator.Equal, _quoteid);
        //            EntityCollection _quoteconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

        //            foreach (var _quoteconditiontype in _quoteconditiontypes.Entities)
        //            {
        //                Entity _masterconditiontype = _organizationservice.Retrieve(_entityname_masterconditiontype, _quoteconditiontype.GetAttributeValue<EntityReference>("ittn_conditiontype").Id, new ColumnSet(true));
        //                Entity _salesorderconditiontype = new Entity(_entityname_salesorderconditiontype);

        //                if (_salesorderconditiontype.GetAttributeValue<bool>("ittn_approveconditiontype") == true)
        //                {
        //                    _salesorderconditiontype["ittn_amount"] = new Money(_quoteconditiontype.GetAttributeValue<Money>("ittn_amount").Value);
        //                    _salesorderconditiontype["ittn_conditiontype"] = new EntityReference(_entityname_masterconditiontype, _masterconditiontype.Id);
        //                    _salesorderconditiontype["ittn_name"] = _masterconditiontype.GetAttributeValue<string>("ittn_description");
        //                    _salesorderconditiontype["ittn_cpo"] = new EntityReference(_entityname_salesorder, _entity.Id);
        //                    _salesorderconditiontype["transactioncurrencyid"] = new EntityReference(_entityname_transactioncurrency, new Guid(_entityname_transactioncurrencyid));

        //                    _organizationservice.Create(_salesorderconditiontype);
        //                }
        //            }

        //            _queryexpression = new QueryExpression(_entityname_matrixsalesorderconditiontypebyproduct);
        //            _queryexpression.ColumnSet = new ColumnSet(true);
        //            _queryexpression.Criteria.AddCondition("ittn_unitgroup", ConditionOperator.Equal, _unitgroupid);
        //            EntityCollection _matrixsalesorderconditiontypebyproducts = _organizationservice.RetrieveMultiple(_queryexpression);

        //            foreach (var _matrixsalesorderconditiontypebyproduct in _matrixsalesorderconditiontypebyproducts.Entities)
        //            {
        //                Entity _masterconditiontype = _organizationservice.Retrieve(_entityname_masterconditiontype, _matrixsalesorderconditiontypebyproduct.GetAttributeValue<EntityReference>("ittn_conditiontype").Id, new ColumnSet(true));
        //                Entity _salesorderconditiontype = new Entity(_entityname_salesorderconditiontype);

        //                _salesorderconditiontype["ittn_amount"] = new Money(_matrixsalesorderconditiontypebyproduct.GetAttributeValue<Money>("ittn_amount").Value);
        //                _salesorderconditiontype["ittn_conditiontype"] = new EntityReference(_entityname_masterconditiontype, _masterconditiontype.Id);
        //                _salesorderconditiontype["ittn_name"] = _masterconditiontype.GetAttributeValue<string>("ittn_description");
        //                _salesorderconditiontype["ittn_cpo"] = new EntityReference(_entityname_salesorder, _entity.Id);
        //                _salesorderconditiontype["transactioncurrencyid"] = new EntityReference(_entityname_transactioncurrency, new Guid(_entityname_transactioncurrencyid));

        //                _organizationservice.Create(_salesorderconditiontype);
        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new InvalidPluginExecutionException(_classname + ".PostCreate_salesorder: " + ex.Message.ToString());
        //    }
        //}

        //public void PostCreate_salesorder(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        //{
        //    try
        //    {
        //        Guid _quoteid = _entity.Attributes.Contains("quoteid") ? _entity.GetAttributeValue<EntityReference>("quoteid").Id : new Guid();

        //        if (_quoteid != new Guid())
        //        {
        //            Guid _unitgroupid = new Guid();
        //            Entity _quote = _organizationservice.Retrieve(_entityname_quote, _quoteid, new ColumnSet(true));
        //            _unitgroupid = _quote.GetAttributeValue<EntityReference>("new_unitgroup").Id;

        //            QueryExpression _queryexpression = new QueryExpression(_entityname_quotedetail);
        //            _queryexpression.ColumnSet = new ColumnSet(true);
        //            _queryexpression.Criteria.AddCondition("quoteid", ConditionOperator.Equal, _quoteid);
        //            EntityCollection _quotedetails = _organizationservice.RetrieveMultiple(_queryexpression);

        //            foreach (var _quotedetail in _quotedetails.Entities)
        //            {
        //                decimal _itemnumber = _quotedetail.GetAttributeValue<decimal>("new_itemnumber");

        //                _queryexpression = new QueryExpression(_entityname_salesorderdetail);
        //                _queryexpression.ColumnSet = new ColumnSet(true);
        //                _queryexpression.Criteria.AddCondition("salesorderid", ConditionOperator.Equal, _entity.Id);
        //                _queryexpression.Criteria.AddCondition("new_itemnumberhistory", ConditionOperator.Equal, _itemnumber);
        //                EntityCollection _salesorderdetails = _organizationservice.RetrieveMultiple(_queryexpression);

        //                foreach (var _salesorderdetail in _salesorderdetails.Entities)
        //                {
        //                    Guid _productid = _salesorderdetail.GetAttributeValue<EntityReference>("productid").Id;
        //                    decimal _itemnumber_salesorderdetail = _salesorderdetail.GetAttributeValue<decimal>("new_itemnumber");

        //                    _queryexpression = new QueryExpression(_entityname_quoteconditiontype);
        //                    _queryexpression.ColumnSet = new ColumnSet(true);
        //                    _queryexpression.Criteria.AddCondition("ittn_quote", ConditionOperator.Equal, _quoteid);
        //                    EntityCollection _quoteconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

        //                    foreach (var _quoteconditiontype in _quoteconditiontypes.Entities)
        //                    {
        //                        Entity _masterconditiontype = _organizationservice.Retrieve(_entityname_masterconditiontype, _quoteconditiontype.GetAttributeValue<EntityReference>("ittn_conditiontype").Id, new ColumnSet(true));
        //                        Entity _salesorderconditiontype = new Entity(_entityname_salesorderconditiontype);

        //                        if (_salesorderconditiontype.GetAttributeValue<bool>("ittn_approveconditiontype") == true)
        //                        {
        //                            _salesorderconditiontype["ittn_amount"] = new Money(_quoteconditiontype.GetAttributeValue<Money>("ittn_amount").Value);
        //                            _salesorderconditiontype["ittn_conditiontype"] = new EntityReference(_entityname_masterconditiontype, _masterconditiontype.Id);
        //                            _salesorderconditiontype["ittn_name"] = _masterconditiontype.GetAttributeValue<string>("ittn_description");
        //                            _salesorderconditiontype["ittn_cpo"] = new EntityReference(_entityname_salesorder, _entity.Id);
        //                            _salesorderconditiontype["ittn_itemnumber"] = _itemnumber_salesorderdetail;
        //                            _salesorderconditiontype["transactioncurrencyid"] = new EntityReference(_entityname_transactioncurrency, new Guid(_entityname_transactioncurrencyid));

        //                            _organizationservice.Create(_salesorderconditiontype);
        //                        }
        //                    }

        //                    _queryexpression = new QueryExpression(_entityname_matrixsalesorderconditiontypebyproduct);
        //                    _queryexpression.ColumnSet = new ColumnSet(true);
        //                    _queryexpression.Criteria.AddCondition("ittn_unitgroup", ConditionOperator.Equal, _unitgroupid);
        //                    _queryexpression.Criteria.AddCondition("ittn_model", ConditionOperator.Equal, _productid);
        //                    EntityCollection _matrixsalesorderconditiontypebyproducts = _organizationservice.RetrieveMultiple(_queryexpression);

        //                    foreach (var _matrixsalesorderconditiontypebyproduct in _matrixsalesorderconditiontypebyproducts.Entities)
        //                    {
        //                        Entity _masterconditiontype = _organizationservice.Retrieve(_entityname_masterconditiontype, _matrixsalesorderconditiontypebyproduct.GetAttributeValue<EntityReference>("ittn_conditiontype").Id, new ColumnSet(true));
        //                        Entity _salesorderconditiontype = new Entity(_entityname_salesorderconditiontype);

        //                        _salesorderconditiontype["ittn_amount"] = new Money(_matrixsalesorderconditiontypebyproduct.GetAttributeValue<Money>("ittn_amount").Value);
        //                        _salesorderconditiontype["ittn_conditiontype"] = new EntityReference(_entityname_masterconditiontype, _masterconditiontype.Id);
        //                        _salesorderconditiontype["ittn_name"] = _masterconditiontype.GetAttributeValue<string>("ittn_description");
        //                        _salesorderconditiontype["ittn_cpo"] = new EntityReference(_entityname_salesorder, _entity.Id);
        //                        _salesorderconditiontype["ittn_itemnumber"] = _itemnumber_salesorderdetail;
        //                        _salesorderconditiontype["transactioncurrencyid"] = new EntityReference(_entityname_transactioncurrency, new Guid(_entityname_transactioncurrencyid));

        //                        _organizationservice.Create(_salesorderconditiontype);
        //                    }

        //                }

        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new InvalidPluginExecutionException(_classname + ".PostCreate_salesorder: " + ex.Message.ToString());
        //    }
        //}

    }
}
