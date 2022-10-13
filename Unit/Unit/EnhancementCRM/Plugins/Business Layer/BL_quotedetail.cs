using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using EnhancementCRM.HelperUnit;
using System.ServiceModel;
using EnhancementCRM.HelperUnit.SAP_MOVING_PRICE;
using EnhancementCRM.HelperUnit.ZPST_CRM_BOM_COMPONENT_PRICE;
using System.Reflection;

namespace EnhancementCRM.Plugins.Business_Layer
{
    public class BL_quotedetail
    {
        #region Properties
        private string _classname = "BL_quotedetail";
        private string _entityname = "quotedetail";

        private string _entityname_businessunit = "businessunit";
        private string _entityname_deliveryplant = "new_deliveryplant";
        private string _entityname_deliveryunitloco = "ittn_deliveryunitloco";
        private string _entityname_masterconditiontype = "ittn_masterconditiontype";
        private string _entityname_masterroute = "ittn_masterroute";
        private string _entityname_mastershippingpricelist = "ittn_mastershippingpricelist";
        private string _entityname_mastertransportandaccomodation = "ittn_mastertransportandaccomodation";
        private string _entityname_matrixsalesorderconditiontypebyproduct = "ittn_matrixcpoconditiontypebyproduct";
        private string _entityname_opportunityproduct = "opportunityproduct";
        private string _entityname_product = "product";
        private string _entityname_quoteconditiontype = "ittn_quoteconditiontype";
        private string _entityname_quote = "quote";
        private string _entityname_quotedetail = "quotedetail";
        private string _entityname_systemuser = "systemuser";
        private string _entityname_transactioncurrency = "transactioncurrency";
        private string _entityname_transactioncurrency_code = "IDR";
        private string _entityname_workflowconfiguration = "trs_workflowconfiguration";
        private string _entityname_workflowconfiguration_name = "TRS";
        private string _entityname_workflowconfiguration_primaryfieldname = "trs_generalconfig";
        private const string ConfigurationWebServiceEntityName = "ittn_webservicesconfiguration";
        private const int WebService_QuoteMovingPrice = 841150003;
        private const int WebService_QuoteBOMComponentPrice = 841150004;

        private int _deliveryoption_franco = 841150000;
        private int _deliveryoption_loco = 841150001;
        #endregion

        #region Constants
        private const int MonthAdd = 1;

        private const int STATUS_ACTIVE = 841150001;
        #endregion

        #region Depedencies
        Generator _generator = new Generator();
        MWSLog _mwslog = new MWSLog();
        #endregion ---

        public void Form_OnCreate_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext, Entity CurrentEntity, ITracingService tracer)
        {
            try
            {
                #region Variables
                DateTime CreatedOn, OneMonthAfterCreatedOn = DateTime.MinValue;
                #endregion

                Entity QuoteDetail = organizationService.Retrieve(_entityname, CurrentEntity.Id, new ColumnSet(true));
                Entity Quote = organizationService.Retrieve(_entityname_quote, QuoteDetail.GetAttributeValue<EntityReference>("quoteid").Id, new ColumnSet(true));

                #region Attributes
                if (QuoteDetail.Contains("createdon") && QuoteDetail.Attributes["createdon"] != null)
                {
                    CreatedOn = QuoteDetail.GetAttributeValue<DateTime>("createdon");
                    OneMonthAfterCreatedOn = CreatedOn.AddMonths(MonthAdd);
                }
                #endregion

                #region Update Fields
                QuoteDetail["ittn_next1monthaftercreatedon"] = OneMonthAfterCreatedOn;
                //organizationService.Update(QuoteDetail);
                #endregion

                #region UPDATE FIELDS ittn_nondefaultbom
                QueryExpression _queryexpression = new QueryExpression(_entityname_opportunityproduct);
                _queryexpression.ColumnSet = new ColumnSet(true);
                _queryexpression.Criteria.AddCondition("opportunityid", ConditionOperator.Equal, Quote.GetAttributeValue<EntityReference>("opportunityid").Id);
                _queryexpression.Criteria.AddCondition("new_itemnumber", ConditionOperator.Equal, QuoteDetail.GetAttributeValue<decimal>("new_itemnumber"));
                Entity _opportunityproduct = organizationService.RetrieveMultiple(_queryexpression).Entities.FirstOrDefault();

                if (_opportunityproduct != null)
                {
                    if (_opportunityproduct.Attributes.Contains("ittn_alternativebomnumber"))
                        QuoteDetail["ittn_alternativebomnumber"] = _opportunityproduct.GetAttributeValue<int>("ittn_alternativebomnumber");

                    if (_opportunityproduct.Attributes.Contains("ittn_nondefaultbom"))
                    {
                        if (_opportunityproduct.GetAttributeValue<bool>("ittn_nondefaultbom"))
                        {
                            Entity _row = new Entity(_entityname_quotedetail);
                            _row.Id = QuoteDetail.Id;

                            _row["ittn_nondefaultbom"] = true;
                            organizationService.Update(_row);
                        }
                    }

                    #region Copy Total Extended Amount from Prospect Detail
                    Entity _QuoteDetailUpdate = new Entity(_entityname_quotedetail);
                    _QuoteDetailUpdate.Id = QuoteDetail.Id;

                    _QuoteDetailUpdate["ittn_totalextendedamount"] = new Money(_opportunityproduct.GetAttributeValue<Money>("ittn_totalextendedamount").Value);
                    organizationService.Update(_QuoteDetailUpdate);
                    #endregion
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PostOperation: " + ex.Message.ToString());
            }
        }

        public void CreateConditionTypeMandatory(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        {
            try
            {
                _entity = _organizationservice.Retrieve(_entityname_quotedetail, _entity.Id, new ColumnSet(true));
                Guid _quoteid = _entity.Attributes.Contains("quoteid") ? _entity.GetAttributeValue<EntityReference>("quoteid").Id : new Guid();

                if (_quoteid != new Guid())
                {
                    decimal? _null_decimal = null;
                    decimal? _itemnumber = _entity.Attributes.Contains("new_itemnumber") ? _entity.GetAttributeValue<decimal>("new_itemnumber") : _null_decimal;

                    if (_itemnumber != _null_decimal)
                    {
                        Entity _owner = _organizationservice.Retrieve(_entityname_systemuser, _entity.GetAttributeValue<EntityReference>("ownerid").Id, new ColumnSet(true));
                        Entity _product = _organizationservice.Retrieve(_entityname_product, _entity.GetAttributeValue<EntityReference>("productid").Id, new ColumnSet(true));
                        Entity _quote = _organizationservice.Retrieve(_entityname_quote, _quoteid, new ColumnSet(true));
                        Guid _businessunitid = _owner.Attributes.Contains("businessunitid") ? _owner.GetAttributeValue<EntityReference>("businessunitid").Id : new Guid();
                        Guid _unitgroupid = _quote.GetAttributeValue<EntityReference>("new_unitgroup").Id;

                        if (_businessunitid == new Guid())
                            throw new InvalidPluginExecutionException("User DOES NOT have Business Unit !");

                        decimal _amountjcs = 0;

                        QueryExpression _queryexpression = new QueryExpression(_entityname_quoteconditiontype);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria.AddCondition("ittn_ismandatory", ConditionOperator.Equal, true);
                        _queryexpression.Criteria.AddCondition("ittn_quote", ConditionOperator.Equal, _quoteid);
                        _queryexpression.Criteria.AddCondition("ittn_itemnumber", ConditionOperator.Equal, _itemnumber);
                        EntityCollection _quoteconditiontypes_fordelete = _organizationservice.RetrieveMultiple(_queryexpression);

                        foreach (var _quoteconditiontype_fordelete in _quoteconditiontypes_fordelete.Entities)
                        {
                            _organizationservice.Delete(_entityname_quoteconditiontype, _quoteconditiontype_fordelete.Id);
                        }

                        if (_entity.Attributes.Contains("ittn_shippingpoint"))
                        {
                            Entity _shippingpoint = _organizationservice.Retrieve(_entityname_masterroute, _entity.GetAttributeValue<EntityReference>("ittn_shippingpoint").Id, new ColumnSet(true));

                            _queryexpression = new QueryExpression(_entityname_mastertransportandaccomodation);
                            _queryexpression.ColumnSet = new ColumnSet(true);
                            _queryexpression.Criteria.AddCondition("ittn_shipmentroute", ConditionOperator.Equal, _shippingpoint.Id);
                            _queryexpression.Criteria.AddCondition("transactioncurrencyid", ConditionOperator.Equal, _entity.GetAttributeValue<EntityReference>("transactioncurrencyid").Id);
                            //_queryexpression.Criteria.AddCondition("ittn_transportplanningpoint", ConditionOperator.Equal, _businessunitid);

                            EntityCollection _mastertransportandaccomodations = _organizationservice.RetrieveMultiple(_queryexpression);

                            if (_mastertransportandaccomodations.Entities.Count() > 0)
                            {
                                Entity _mastertransportandaccomodation = _mastertransportandaccomodations.Entities[0];

                                _amountjcs += _mastertransportandaccomodation.Attributes.Contains("ittn_amountjcs") ? _mastertransportandaccomodation.GetAttributeValue<Money>("ittn_amountjcs").Value : 0;
                            }

                        }

                        _queryexpression = new QueryExpression(_entityname_matrixsalesorderconditiontypebyproduct);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria.AddCondition("ittn_unitgroup", ConditionOperator.Equal, _unitgroupid);
                        _queryexpression.Criteria.AddCondition("ittn_model", ConditionOperator.Equal, _product.Id);

                        EntityCollection _matrixsalesorderconditiontypebyproducts = _organizationservice.RetrieveMultiple(_queryexpression);

                        foreach (var _matrixsalesorderconditiontypebyproduct in _matrixsalesorderconditiontypebyproducts.Entities)
                        {
                            Entity _masterconditiontype = _organizationservice.Retrieve(_entityname_masterconditiontype, _matrixsalesorderconditiontypebyproduct.GetAttributeValue<EntityReference>("ittn_conditiontype").Id, new ColumnSet(true));
                            Entity _quoteconditiontype = new Entity(_entityname_quoteconditiontype);
                            decimal _amount = _matrixsalesorderconditiontypebyproduct.GetAttributeValue<Money>("ittn_amount").Value + _amountjcs;
                            decimal _quantity = _entity.GetAttributeValue<decimal>("new_quantity");
                            decimal _totalamount = _amount * _quantity;

                            _quoteconditiontype["ittn_ismandatory"] = true;
                            _quoteconditiontype["ittn_amount"] = new Money(_amount);
                            _quoteconditiontype["ittn_totalamount"] = new Money(_totalamount);
                            _quoteconditiontype["ittn_conditiontype"] = new EntityReference(_entityname_masterconditiontype, _masterconditiontype.Id);
                            _quoteconditiontype["ittn_name"] = _masterconditiontype.GetAttributeValue<string>("ittn_description");
                            _quoteconditiontype["ittn_quote"] = new EntityReference(_entityname_quote, _quoteid);
                            _quoteconditiontype["ittn_itemnumber"] = _itemnumber;
                            _quoteconditiontype["ittn_statusreason"] = new OptionSetValue(STATUS_ACTIVE);
                            _quoteconditiontype["transactioncurrencyid"] = new EntityReference(_entityname_transactioncurrency, Globalization.GetTransactionCurrencyID(_organizationservice, _entityname_transactioncurrency_code));

                            _organizationservice.Create(_quoteconditiontype);
                        }
                    }

                }

            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(_classname + ".PostCreate_quotedetail_createconditiontype: " + e.Message.ToString());
            }
        }

        public void CreateConditionType(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _trace)
        {
            _entity = _organizationservice.Retrieve(_entityname_quotedetail, _entity.Id, new ColumnSet(true));
            Guid _quoteid = _entity.Attributes.Contains("quoteid") ? _entity.GetAttributeValue<EntityReference>("quoteid").Id : new Guid();
            Guid _businessunitidparent = Guid.Empty;
            String BranchCodeOwner = null;

            if (_quoteid != new Guid())
            {
                decimal? _null_decimal = null;
                decimal? _itemnumber = _entity.Attributes.Contains("new_itemnumber") ? _entity.GetAttributeValue<decimal>("new_itemnumber") : _null_decimal;

                if (_itemnumber != _null_decimal)
                {
                    Entity _owner = _organizationservice.Retrieve(_entityname_systemuser, _entity.GetAttributeValue<EntityReference>("ownerid").Id, new ColumnSet(true));
                    Entity _product = _organizationservice.Retrieve(_entityname_product, _entity.GetAttributeValue<EntityReference>("productid").Id, new ColumnSet(true));
                    Entity _quote = _organizationservice.Retrieve(_entityname_quote, _quoteid, new ColumnSet(true));
                    Guid _transactioncurrencyid = _entity.Attributes.Contains("transactioncurrencyid") ? _entity.GetAttributeValue<EntityReference>("transactioncurrencyid").Id : Globalization.GetTransactionCurrencyID(_organizationservice, _entityname_transactioncurrency_code);
                    Guid _businessunitid = _owner.Attributes.Contains("businessunitid") ? _owner.GetAttributeValue<EntityReference>("businessunitid").Id : new Guid();
                    Guid _unitgroupid = _quote.GetAttributeValue<EntityReference>("new_unitgroup").Id;

                    //if (_businessunitid == new Guid())
                    //    throw new InvalidPluginExecutionException("User DOES NOT have Business Unit !");
                    //else
                    //{
                    //    Entity BusinessUnit = _organizationservice.Retrieve("businessunit", _businessunitid, new ColumnSet(true));

                    //    if (BusinessUnit.Contains("trs_branchcode") && BusinessUnit.Attributes["trs_branchcode"] != null)
                    //    {
                    //        BranchCodeOwner = BusinessUnit.GetAttributeValue<String>("trs_branchcode");

                    //        QueryExpression _queryexpressionbu = new QueryExpression(_entityname_businessunit);
                    //        _queryexpressionbu.ColumnSet = new ColumnSet(true);
                    //        _queryexpressionbu.Criteria.AddCondition("trs_branchcode", ConditionOperator.Equal, BranchCodeOwner);
                    //        _queryexpressionbu.Criteria.AddCondition("name", ConditionOperator.Equal, BranchCodeOwner);
                    //        //_queryexpressionbu.Criteria.AddCondition("parentbusinessunitid", ConditionOperator.Null);
                    //        EntityCollection _masterbusinessunit = _organizationservice.RetrieveMultiple(_queryexpressionbu);

                    //        if (_masterbusinessunit.Entities.Count() == 0)
                    //            throw new InvalidPluginExecutionException("There is no name '" + BranchCodeOwner + "' in Business Unit!");
                    //        else
                    //            _businessunitidparent = _masterbusinessunit.Entities.FirstOrDefault().Id;
                    //    }
                    //}

                    decimal _amount = 0;
                    Guid _masterconditiontypeid = new Guid();

                    QueryExpression _queryexpression = new QueryExpression(_entityname_masterconditiontype);
                    _queryexpression.ColumnSet = new ColumnSet(true);
                    _queryexpression.Criteria.AddCondition("ittn_code", ConditionOperator.Equal, "ZFR0");
                    EntityCollection _masterconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                    if (_masterconditiontypes.Entities.Count() == 0)
                        throw new InvalidPluginExecutionException("There is no Code 'ZFR0' in Master Condition Type!");
                    else
                        _masterconditiontypeid = _masterconditiontypes.Entities.FirstOrDefault().Id;

                    _queryexpression = new QueryExpression(_entityname_quoteconditiontype);
                    _queryexpression.ColumnSet = new ColumnSet(true);
                    _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontypeid);
                    _queryexpression.Criteria.AddCondition("ittn_quote", ConditionOperator.Equal, _quoteid);
                    _queryexpression.Criteria.AddCondition("ittn_itemnumber", ConditionOperator.Equal, _itemnumber);
                    EntityCollection _quoteconditiontypes_fordelete = _organizationservice.RetrieveMultiple(_queryexpression);

                    foreach (var _quoteconditiontype_fordelete in _quoteconditiontypes_fordelete.Entities)
                    {
                        _organizationservice.Delete(_entityname_quoteconditiontype, _quoteconditiontype_fordelete.Id);
                    }

                    if (_entity.GetAttributeValue<OptionSetValue>("new_deliveryoption").Value == _deliveryoption_franco)
                    {
                        Entity _shippingpoint = _organizationservice.Retrieve(_entityname_masterroute, _entity.GetAttributeValue<EntityReference>("ittn_shippingpoint").Id, new ColumnSet(true));

                        _queryexpression = new QueryExpression(_entityname_mastershippingpricelist);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria.AddCondition("ittn_model", ConditionOperator.Equal, _product.Id);
                        _queryexpression.Criteria.AddCondition("ittn_shipmentroute", ConditionOperator.Equal, _shippingpoint.Id);
                        _queryexpression.Criteria.AddCondition("transactioncurrencyid", ConditionOperator.Equal, _transactioncurrencyid);
                        //_queryexpression.Criteria.AddCondition("ittn_transportplanningpoint", ConditionOperator.Equal, _businessunitidparent);

                        EntityCollection _mastershippingpricelists = _organizationservice.RetrieveMultiple(_queryexpression);

                        if (_mastershippingpricelists.Entities.Count() > 0)
                        {
                            Entity _mastershippingpricelist = _mastershippingpricelists.Entities[0];

                            _amount += _mastershippingpricelist.GetAttributeValue<Money>("ittn_amount").Value;
                        }
                    }
                    else
                    {
                        Entity _deliveryplant = _organizationservice.Retrieve(_entityname_deliveryplant, _entity.GetAttributeValue<EntityReference>("ittn_deliveryplanbranch").Id, new ColumnSet(true));

                        _queryexpression = new QueryExpression(_entityname_deliveryunitloco);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria.AddCondition("ittn_model", ConditionOperator.Equal, _product.Id);
                        _queryexpression.Criteria.AddCondition("ittn_branchdestination", ConditionOperator.Equal, _deliveryplant.Id);
                        _queryexpression.Criteria.AddCondition("transactioncurrencyid", ConditionOperator.Equal, _transactioncurrencyid);

                        EntityCollection _deliveryunitlocos = _organizationservice.RetrieveMultiple(_queryexpression);

                        if (_deliveryunitlocos.Entities.Count() > 0)
                        {
                            Entity _deliveryunitloco = _deliveryunitlocos.Entities[0];

                            _amount += _deliveryunitloco.GetAttributeValue<Money>("ittn_shipmentamount").Value;
                        }
                    }

                    decimal _quantity = _entity.GetAttributeValue<decimal>("new_quantity");
                    decimal _totalamount = _amount * _quantity;

                    Entity _quoteconditiontype = new Entity(_entityname_quoteconditiontype);

                    _quoteconditiontype["ittn_ismandatory"] = false;
                    _quoteconditiontype["ittn_amount"] = new Money(_amount);
                    _quoteconditiontype["ittn_totalamount"] = new Money(_totalamount);
                    _quoteconditiontype["ittn_conditiontype"] = new EntityReference(_entityname_masterconditiontype, _masterconditiontypeid);
                    _quoteconditiontype["ittn_name"] = _masterconditiontypes.Entities.FirstOrDefault().GetAttributeValue<string>("ittn_description");
                    _quoteconditiontype["ittn_quote"] = new EntityReference(_entityname_quote, _quoteid);
                    _quoteconditiontype["ittn_itemnumber"] = _itemnumber;
                    _quoteconditiontype["ittn_statusreason"] = new OptionSetValue(STATUS_ACTIVE);
                    _quoteconditiontype["transactioncurrencyid"] = new EntityReference(_entityname_transactioncurrency, Globalization.GetTransactionCurrencyID(_organizationservice, _entityname_transactioncurrency_code));

                    _organizationservice.Create(_quoteconditiontype);

                    // -------------------- -------------------- -------------------- -------------------- --------------------
                    // TOTAL AMOUNT ADDED WITH JCS AMOUNT

                    //_queryexpression = new QueryExpression(_entityname_quoteconditiontype);
                    //_queryexpression.ColumnSet = new ColumnSet(true);
                    //_queryexpression.Criteria.AddCondition("ittn_quote", ConditionOperator.Equal, _quoteid);
                    //_queryexpression.Criteria.AddCondition("ittn_itemnumber", ConditionOperator.Equal, _itemnumber);
                    //_queryexpression.Criteria.AddCondition("ittn_ismandatory", ConditionOperator.Equal, true);
                    //EntityCollection _quoteconditiontypes_forupdates = _organizationservice.RetrieveMultiple(_queryexpression);

                    //foreach (var _quoteconditiontypes_forupdate in _quoteconditiontypes_forupdates.Entities)
                    //{

                    //}

                    //_queryexpression = new QueryExpression(_entityname_matrixsalesorderconditiontypebyproduct);
                    //_queryexpression.ColumnSet = new ColumnSet(true);
                    //_queryexpression.Criteria.AddCondition("ittn_unitgroup", ConditionOperator.Equal, _unitgroupid);
                    //_queryexpression.Criteria.AddCondition("ittn_model", ConditionOperator.Equal, _product.Id);

                    //EntityCollection _matrixsalesorderconditiontypebyproducts = _organizationservice.RetrieveMultiple(_queryexpression);

                    //foreach (var _matrixsalesorderconditiontypebyproduct in _matrixsalesorderconditiontypebyproducts.Entities)
                    //{
                    //    Entity _masterconditiontype = _organizationservice.Retrieve(_entityname_masterconditiontype, _matrixsalesorderconditiontypebyproduct.GetAttributeValue<EntityReference>("ittn_conditiontype").Id, new ColumnSet(true));
                    //    //Entity _quoteconditiontype = new Entity(_entityname_quoteconditiontype);

                    //    //_quoteconditiontype["ittn_ismandatory"] = true;
                    //    //_quoteconditiontype["ittn_amount"] = new Money(_matrixsalesorderconditiontypebyproduct.GetAttributeValue<Money>("ittn_amount").Value);
                    //    //_quoteconditiontype["ittn_conditiontype"] = new EntityReference(_entityname_masterconditiontype, _masterconditiontype.Id);
                    //    //_quoteconditiontype["ittn_name"] = _masterconditiontype.GetAttributeValue<string>("ittn_description");
                    //    //_quoteconditiontype["ittn_quote"] = new EntityReference(_entityname_quote, _quoteid);
                    //    //_quoteconditiontype["ittn_itemnumber"] = _itemnumber;
                    //    //_quoteconditiontype["transactioncurrencyid"] = new EntityReference(_entityname_transactioncurrency, Globalization.GetTransactionCurrencyID(_organizationservice, _entityname_transactioncurrency_code));

                    //    //_organizationservice.Create(_quoteconditiontype);


                    //    if (_entity.GetAttributeValue<bool>("ittn_ismandatory"))
                    //    {
                    //        _queryexpression = new QueryExpression(_entityname_mastertransportandaccomodation);
                    //        _queryexpression.ColumnSet = new ColumnSet(true);
                    //        _queryexpression.Criteria.AddCondition("ittn_shipmentroute", ConditionOperator.Equal, _shippingpoint.Id);
                    //        _queryexpression.Criteria.AddCondition("transactioncurrencyid", ConditionOperator.Equal, _transactioncurrencyid);
                    //        _queryexpression.Criteria.AddCondition("ittn_transportplanningpoint", ConditionOperator.Equal, _businessunitid);

                    //        EntityCollection _mastertransportandaccomodations = _organizationservice.RetrieveMultiple(_queryexpression);

                    //        if (_mastertransportandaccomodations.Entities.Count() > 0)
                    //        {
                    //            Entity _mastertransportandaccomodation = _mastertransportandaccomodations.Entities[0];

                    //            _totalamount += _mastertransportandaccomodation.Attributes.Contains("ittn_amountjcs") ? _mastertransportandaccomodation.GetAttributeValue<Money>("ittn_amountjcs").Value : 0;
                    //        }
                    //    }
                    //}
                    // -------------------- -------------------- -------------------- -------------------- --------------------
                }

            }
        }

        public void CreateConditionTypeInternalTest(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _trace, Entity _preimage)
        {
            _entity = _organizationservice.Retrieve(_entityname_quotedetail, _entity.Id, new ColumnSet(true));
            Guid _quoteid = _entity.Attributes.Contains("quoteid") ? _entity.GetAttributeValue<EntityReference>("quoteid").Id : new Guid();

            if (_quoteid != new Guid())
            {
                decimal? _null_decimal = null;
                decimal? _itemnumber = _entity.Attributes.Contains("new_itemnumber") ? _entity.GetAttributeValue<decimal>("new_itemnumber") : _null_decimal;

                if (_itemnumber != _null_decimal)
                {
                    Guid _masterconditiontypeid = new Guid();

                    QueryExpression _queryexpression = new QueryExpression(_entityname_masterconditiontype);
                    _queryexpression.ColumnSet = new ColumnSet(true);
                    _queryexpression.Criteria.AddCondition("ittn_code", ConditionOperator.Equal, "ZPTS");
                    EntityCollection _masterconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                    if (_masterconditiontypes.Entities.Count() == 0)
                        throw new InvalidPluginExecutionException("There is no Code 'ZPTS' in Master Condition Type!");
                    else
                        _masterconditiontypeid = _masterconditiontypes.Entities.FirstOrDefault().Id;

                    bool _needinternaltest = _entity.GetAttributeValue<bool>("ittn_needinternaltest");
                    decimal _amount = _entity.Attributes.Contains("ittn_internaltestamount") ? _entity.GetAttributeValue<Money>("ittn_internaltestamount").Value : 0;
                    decimal _quantity = _entity.GetAttributeValue<decimal>("new_quantity");
                    decimal _totalamount = _amount * _quantity;

                    _queryexpression = new QueryExpression(_entityname_quoteconditiontype);
                    _queryexpression.ColumnSet = new ColumnSet(true);
                    _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontypeid);
                    _queryexpression.Criteria.AddCondition("ittn_quote", ConditionOperator.Equal, _quoteid);
                    _queryexpression.Criteria.AddCondition("ittn_itemnumber", ConditionOperator.Equal, _itemnumber);
                    _queryexpression.Criteria.AddCondition("ittn_ismandatory", ConditionOperator.Equal, true);

                    Entity _quoteconditiontypes_mandatory = _organizationservice.RetrieveMultiple(_queryexpression).Entities.FirstOrDefault();
                    
                    if (_needinternaltest)
                    {
                        if (_quoteconditiontypes_mandatory == null)
                        {
                            Entity _quoteconditiontype = new Entity(_entityname_quoteconditiontype);

                            _quoteconditiontype["ittn_amount"] = new Money(_amount);
                            _quoteconditiontype["ittn_totalamount"] = new Money(_totalamount);
                            _quoteconditiontype["ittn_conditiontype"] = new EntityReference(_entityname_masterconditiontype, _masterconditiontypeid);
                            _quoteconditiontype["ittn_name"] = _masterconditiontypes.Entities.FirstOrDefault().GetAttributeValue<string>("ittn_description");
                            _quoteconditiontype["ittn_quote"] = new EntityReference(_entityname_quote, _quoteid);
                            _quoteconditiontype["ittn_itemnumber"] = _itemnumber;
                            _quoteconditiontype["ittn_statusreason"] = new OptionSetValue(STATUS_ACTIVE);
                            _quoteconditiontype["transactioncurrencyid"] = new EntityReference(_entityname_transactioncurrency, Globalization.GetTransactionCurrencyID(_organizationservice, _entityname_transactioncurrency_code));

                            _organizationservice.Create(_quoteconditiontype);
                        }
                        else
                        {
                            decimal _amount_mandatory = _quoteconditiontypes_mandatory.GetAttributeValue<Money>("ittn_amount").Value;
                            decimal _totalamount_mandatory = _amount_mandatory * _quantity;

                            decimal _t_amount = _amount + _amount_mandatory;
                            decimal _t_totalamount = _totalamount + _totalamount_mandatory;

                            Entity _quoteconditiontype = new Entity(_entityname_quoteconditiontype);
                            _quoteconditiontype.Id = _quoteconditiontypes_mandatory.Id;

                            _quoteconditiontype["ittn_amount"] = new Money(_t_amount);
                            _quoteconditiontype["ittn_totalamount"] = new Money(_t_totalamount);

                            _organizationservice.Update(_quoteconditiontype);
                        }
                    }
                    else
                    {
                        if (_quoteconditiontypes_mandatory == null)
                        {
                            _queryexpression = new QueryExpression(_entityname_quoteconditiontype);
                            _queryexpression.ColumnSet = new ColumnSet(true);
                            _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontypeid);
                            _queryexpression.Criteria.AddCondition("ittn_quote", ConditionOperator.Equal, _quoteid);
                            _queryexpression.Criteria.AddCondition("ittn_itemnumber", ConditionOperator.Equal, _itemnumber);
                            EntityCollection _quoteconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                            foreach (var _quoteconditiontype in _quoteconditiontypes.Entities)
                            {
                                _organizationservice.Delete(_entityname_quoteconditiontype, _quoteconditiontype.Id);
                            }
                        }
                        else
                        {
                            _amount = _preimage.GetAttributeValue<Money>("ittn_internaltestamount").Value;
                            _totalamount = _amount * _quantity;

                            decimal _amount_mandatory = _quoteconditiontypes_mandatory.GetAttributeValue<Money>("ittn_amount").Value;
                            decimal _totalamount_mandatory = _amount_mandatory * _quantity;

                            decimal _t_amount = _amount_mandatory - _amount;
                            decimal _t_totalamount = _totalamount_mandatory - _totalamount;

                            Entity _quoteconditiontype = new Entity(_entityname_quoteconditiontype);
                            _quoteconditiontype.Id = _quoteconditiontypes_mandatory.Id;

                            _quoteconditiontype["ittn_amount"] = new Money(_t_amount);
                            _quoteconditiontype["ittn_totalamount"] = new Money(_t_totalamount);

                            _organizationservice.Update(_quoteconditiontype);
                        }
                        
                    }
                    
                }

            }

        }

        public decimal CostUnitSAP_ZPST_FM_005(IOrganizationService _organizationservice, ITracingService _trace, Entity _quote, Entity _product)
        {
            MWSLog _mwslog = new MWSLog();

            //string _token = string.Empty;
            //string _usercredentials = string.Empty;
            decimal _costunit = new decimal(0);
            string _sapwebservice = string.Empty;
            string _sapintegrationuniquekey = string.Empty;
            string _sapwebserviceusername = string.Empty;
            string _sapwebservicepassword = string.Empty;
            string _plant = null;
            string _materialname = _product.GetAttributeValue<string>("productnumber");

            _trace.Trace("Start Getting SAP WebService Configuration");

            QueryExpression _queryexpression = new QueryExpression(_entityname_workflowconfiguration);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition(_entityname_workflowconfiguration_primaryfieldname, ConditionOperator.Equal, _entityname_workflowconfiguration_name);
            EntityCollection ECSAPConfiguration = _organizationservice.RetrieveMultiple(_queryexpression);

            if (ECSAPConfiguration.Entities.Count > 0)
            {
                Entity _workflowconfiguration = ECSAPConfiguration.Entities[0];

                int _defaultcheckstockplant = _workflowconfiguration.GetAttributeValue<OptionSetValue>("ittn_defaultcheckstockplant").Value;

                if (_defaultcheckstockplant == 0)
                {
                    _plant = "A001";
                }
                else
                {
                    Entity _systemuser = _organizationservice.Retrieve(_entityname_systemuser, _quote.GetAttributeValue<EntityReference>("ownerid").Id, new ColumnSet(true));
                    Entity _businessunit = _organizationservice.Retrieve(_entityname_businessunit, _systemuser.GetAttributeValue<EntityReference>("businessunitid").Id, new ColumnSet(true));

                    _plant = _businessunit.GetAttributeValue<string>("trs_branchcode");
                }

                QueryExpression WSCPO = new QueryExpression(ConfigurationWebServiceEntityName);
                WSCPO.ColumnSet = new ColumnSet(true);
                WSCPO.Criteria.AddCondition("ittn_workflowconfiguration", ConditionOperator.Equal, _workflowconfiguration.Id);
                WSCPO.Criteria.AddCondition("ittn_webservicefor", ConditionOperator.Equal, WebService_QuoteMovingPrice);
                EntityCollection ECWS = _organizationservice.RetrieveMultiple(WSCPO);

                if (ECWS.Entities.Count > 0)
                {
                    Entity WSCPOS = ECWS.Entities[0];

                    _sapwebservice = WSCPOS.GetAttributeValue<string>("ittn_sapwebservice");
                    _sapintegrationuniquekey = WSCPOS.GetAttributeValue<string>("ittn_sapintegrationuniquekey");
                    _sapwebserviceusername = WSCPOS.GetAttributeValue<string>("ittn_sapwebserviceusername");
                    _sapwebservicepassword = WSCPOS.GetAttributeValue<string>("ittn_sapwebservicepassword");
                }
                else
                    throw new InvalidPluginExecutionException("Web Service for Check Quote Moving Price is null/empty!");

            }
            else
                throw new InvalidPluginExecutionException("Cannot fount Workflow Configuration with name " + _entityname_workflowconfiguration_name + " !");

            _trace.Trace("Getting WebService Client");

            BasicHttpBinding _httpbinding = new BasicHttpBinding();
            _httpbinding.Name = "ZWEB_SERVICE_CRM";
            _httpbinding.MessageEncoding = WSMessageEncoding.Mtom;
            _httpbinding.TextEncoding = Encoding.UTF8;
            _httpbinding.SendTimeout = new TimeSpan(0, 10, 0);
            EndpointAddress _remoteaddress = new EndpointAddress(_sapwebservice);

            _trace.Trace("Creating Services Client");

            ZPST_FM_005_V2Client _client = new ZPST_FM_005_V2Client(_httpbinding, _remoteaddress);
            _client.ClientCredentials.UserName.UserName = _sapwebserviceusername;
            _client.ClientCredentials.UserName.Password = _sapwebservicepassword;

            ZPST_FM_005Request _saprequest = new ZPST_FM_005Request();
            ZPST_FM_005Response _sapresponse = new ZPST_FM_005Response();
            ZPST_FM_005 _quotedetail_movingprice = new ZPST_FM_005();

            _quotedetail_movingprice.CSRF_TOKEN = _generator.Encrypt(_materialname, _sapintegrationuniquekey);
            _quotedetail_movingprice.MATERIAL = _materialname;
            _quotedetail_movingprice.PLANT = _plant;

            try
            {
                _trace.Trace("Open Client WebService");

                _client = new ZPST_FM_005_V2Client(_httpbinding, _remoteaddress);
                _client.Open();
                _sapresponse = _client.ZPST_FM_005(_quotedetail_movingprice);
            }
            catch (Exception _exception)
            {
                throw new InvalidPluginExecutionException(_exception.Message);
                _mwslog.Write(MethodBase.GetCurrentMethod().Name, "Failed to Check Quote Moving Price on SAP. Details : " + _exception.Message, MWSLog.LogType.Error, MWSLog.Source.Outbound);
            }
            finally
            {
                _client.Close();
            }

            if (_sapresponse != null)
            {
                _costunit = _sapresponse.MOVING_PRICE;
            }

            return _costunit;
        }

        public decimal BOM_Component_Price_ZPST_FM_006(IOrganizationService _organizationservice, ITracingService _trace, Entity _quote, Entity _product)
        {
            MWSLog _mwslog = new MWSLog();

            //string _token = string.Empty;
            //string _usercredentials = string.Empty;
            decimal _costunit = new decimal(0);
            string _sapwebservice = string.Empty;
            string _sapintegrationuniquekey = string.Empty;
            string _sapwebserviceusername = string.Empty;
            string _sapwebservicepassword = string.Empty;
            string _plant = null;
            string _materialname = _product.GetAttributeValue<string>("productnumber");

            _trace.Trace("Start Getting SAP WebService Configuration");

            QueryExpression _queryexpression = new QueryExpression(_entityname_workflowconfiguration);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition(_entityname_workflowconfiguration_primaryfieldname, ConditionOperator.Equal, _entityname_workflowconfiguration_name);
            EntityCollection ECSAPConfiguration = _organizationservice.RetrieveMultiple(_queryexpression);

            if (ECSAPConfiguration.Entities.Count > 0)
            {
                Entity _workflowconfiguration = ECSAPConfiguration.Entities[0];

                int _defaultcheckstockplant = _workflowconfiguration.GetAttributeValue<OptionSetValue>("ittn_defaultcheckstockplant").Value;

                if (_defaultcheckstockplant == 0)
                {
                    _plant = "A001";
                }
                else
                {
                    Entity _systemuser = _organizationservice.Retrieve(_entityname_systemuser, _quote.GetAttributeValue<EntityReference>("ownerid").Id, new ColumnSet(true));
                    Entity _businessunit = _organizationservice.Retrieve(_entityname_businessunit, _systemuser.GetAttributeValue<EntityReference>("businessunitid").Id, new ColumnSet(true));

                    _plant = _businessunit.GetAttributeValue<string>("trs_branchcode");
                }

                QueryExpression WSCPO = new QueryExpression(ConfigurationWebServiceEntityName);
                WSCPO.ColumnSet = new ColumnSet(true);
                WSCPO.Criteria.AddCondition("ittn_workflowconfiguration", ConditionOperator.Equal, _workflowconfiguration.Id);
                WSCPO.Criteria.AddCondition("ittn_webservicefor", ConditionOperator.Equal, WebService_QuoteBOMComponentPrice);
                EntityCollection ECWS = _organizationservice.RetrieveMultiple(WSCPO);

                if (ECWS.Entities.Count > 0)
                {
                    Entity WSCPOS = ECWS.Entities[0];

                    _sapwebservice = WSCPOS.GetAttributeValue<string>("ittn_sapwebservice");
                    _sapintegrationuniquekey = WSCPOS.GetAttributeValue<string>("ittn_sapintegrationuniquekey");
                    _sapwebserviceusername = WSCPOS.GetAttributeValue<string>("ittn_sapwebserviceusername");
                    _sapwebservicepassword = WSCPOS.GetAttributeValue<string>("ittn_sapwebservicepassword");
                }
                else
                    throw new InvalidPluginExecutionException("Web Service for Check BOM Component Price is null/empty!");

            }
            else
                throw new InvalidPluginExecutionException("Cannot fount Workflow Configuration with name " + _entityname_workflowconfiguration_name + " !");

            _trace.Trace("Getting WebService Client");

            BasicHttpBinding _httpbinding = new BasicHttpBinding();
            _httpbinding.Name = "ZWEB_SERVICE_CRM";
            _httpbinding.MessageEncoding = WSMessageEncoding.Mtom;
            _httpbinding.TextEncoding = Encoding.UTF8;
            _httpbinding.SendTimeout = new TimeSpan(0, 10, 0);
            EndpointAddress _remoteaddress = new EndpointAddress(_sapwebservice);

            _trace.Trace("Creating Services Client");

            ZPST_FM_006_v2Client _client = new ZPST_FM_006_v2Client(_httpbinding, _remoteaddress);
            _client.ClientCredentials.UserName.UserName = _sapwebserviceusername;
            _client.ClientCredentials.UserName.Password = _sapwebservicepassword;

            ZPST_FM_006Request _saprequest = new ZPST_FM_006Request();
            ZPST_FM_006Response _sapresponse = new ZPST_FM_006Response();
            ZPST_FM_006 _quotedetail_bom_component_price = new ZPST_FM_006();

            _quotedetail_bom_component_price.CSRF_TOKEN = _generator.Encrypt(_materialname, _sapintegrationuniquekey);
            _quotedetail_bom_component_price.MATNR = _materialname;

            try
            {
                _trace.Trace("Open Client WebService");

                _client = new ZPST_FM_006_v2Client(_httpbinding, _remoteaddress);
                _client.Open();
                _sapresponse = _client.ZPST_FM_006(_quotedetail_bom_component_price);
            }
            catch (Exception _exception)
            {
                throw new InvalidPluginExecutionException(_exception.Message);
                _mwslog.Write(MethodBase.GetCurrentMethod().Name, "Failed to Check BOM Component Price on SAP. Details : " + _exception.Message, MWSLog.LogType.Error, MWSLog.Source.Outbound);
            }
            finally
            {
                _client.Close();
            }

            if (_sapresponse != null)
            {
                _costunit = _sapresponse.AMOUNT;
            }

            return _costunit;
        }

        public void CostUnitSAP(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _trace)
        {
            _entity = _organizationservice.Retrieve(_entityname_quotedetail, _entity.Id, new ColumnSet(true));

            if (!_entity.Attributes.Contains("ittn_costunitsap"))
            {
                Guid _quoteid = _entity.Attributes.Contains("quoteid") ? _entity.GetAttributeValue<EntityReference>("quoteid").Id : new Guid();

                if (_quoteid != new Guid())
                {
                    if (_entity.Attributes.Contains("productid"))
                    {
                        if (!_entity.Attributes.Contains("new_parentnumber"))
                        {
                            decimal _movingprice = new decimal(0);
                            decimal _attachment_total = new decimal(0);
                            decimal _movingprice_total = new decimal(0);
                            decimal _bomcomponenct_price = new decimal(0);

                            // MAIN UNIT
                            Entity _quote = _organizationservice.Retrieve(_entityname_quote, _quoteid, new ColumnSet(true));
                            Entity _product = _organizationservice.Retrieve(_entityname_product, _entity.GetAttributeValue<EntityReference>("productid").Id, new ColumnSet(true));
                            decimal _itemnumber = _entity.GetAttributeValue<decimal>("new_itemnumber");

                            _movingprice = CostUnitSAP_ZPST_FM_005(_organizationservice, _trace, _quote, _product);

                            // ATTACHMENT(S)
                            QueryExpression _queryexpression = new QueryExpression(_entityname_quotedetail);
                            _queryexpression.ColumnSet = new ColumnSet(true);
                            _queryexpression.Criteria.AddCondition("quoteid", ConditionOperator.Equal, _quoteid);
                            _queryexpression.Criteria.AddCondition("new_parentnumber", ConditionOperator.Equal, _itemnumber);
                            _queryexpression.Criteria.AddCondition("ittn_nondefaultbom", ConditionOperator.Equal, true);

                            EntityCollection _quotedetails = _organizationservice.RetrieveMultiple(_queryexpression);

                            foreach (var _quotedetail in _quotedetails.Entities)
                            {
                                _product = _organizationservice.Retrieve(_entityname_product, _quotedetail.GetAttributeValue<EntityReference>("productid").Id, new ColumnSet(true));
                                _bomcomponenct_price = BOM_Component_Price_ZPST_FM_006(_organizationservice, _trace, _quote, _product);

                                Entity _rowdetail = new Entity(_entityname_quotedetail);
                                _rowdetail.Id = _quotedetail.Id;

                                _rowdetail["ittn_movingprice"] = new Money(_bomcomponenct_price);
                                _rowdetail["ittn_costunitsap"] = new Money(_bomcomponenct_price);

                                _organizationservice.Update(_rowdetail);

                                _attachment_total += _bomcomponenct_price;
                            }

                            _movingprice_total = _movingprice + _attachment_total;

                            Entity _row = new Entity(_entityname_quotedetail);
                            _row.Id = _entity.Id;

                            _row["ittn_movingprice"] = new Money(_movingprice_total);
                            _row["ittn_costunitsap"] = new Money(_movingprice_total);

                            _organizationservice.Update(_row);

                            //if (_sapresponse != null)
                            //{
                            //    decimal _movingprice = _sapresponse.MOVING_PRICE;

                            //    Entity _row = new Entity(_entityname_quotedetail);
                            //    _row.Id = _entity.Id;

                            //    _row["ittn_movingprice"] = new Money(_movingprice);
                            //    _row["ittn_costunitsap"] = new Money(_movingprice);

                            //    _organizationservice.Update(_row);
                            //}
                        }
                        else
                        {
                            Entity _row = new Entity(_entityname_quotedetail);
                            _row.Id = _entity.Id;

                            _row["ittn_movingprice"] = null;
                            _row["ittn_costunitsap"] = null;

                            _organizationservice.Update(_row);
                        }

                    }

                }

            }

        }

        public void CostUnit(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _trace)
        {
            _entity = _organizationservice.Retrieve(_entityname_quotedetail, _entity.Id, new ColumnSet(true));
            Guid _quoteid = _entity.Attributes.Contains("quoteid") ? _entity.GetAttributeValue<EntityReference>("quoteid").Id : new Guid();

            if (_quoteid != new Guid())
            {
                if (!_entity.Attributes.Contains("new_parentnumber"))
                {
                    Entity _product = _organizationservice.Retrieve(_entityname_product, _entity.GetAttributeValue<EntityReference>("productid").Id, new ColumnSet(true));
                    Entity _owner = _organizationservice.Retrieve(_entityname_systemuser, _entity.GetAttributeValue<EntityReference>("ownerid").Id, new ColumnSet(true));
                    Guid _businessunitid = _owner.Attributes.Contains("businessunitid") ? _owner.GetAttributeValue<EntityReference>("businessunitid").Id : new Guid();
                    Guid _transactioncurrencyid = _entity.Attributes.Contains("transactioncurrencyid") ? _entity.GetAttributeValue<EntityReference>("transactioncurrencyid").Id : Globalization.GetTransactionCurrencyID(_organizationservice, _entityname_transactioncurrency_code);

                    decimal? _nulldecimal = null;
                    decimal _itemnumber = _entity.GetAttributeValue<decimal>("new_itemnumber");
                    decimal? _parentnumber = _entity.Attributes.Contains("new_parentnumber") ? _entity.GetAttributeValue<decimal>("new_parentnumber") : _nulldecimal;
                    decimal _N1_totalconditiontype = 0;
                    decimal _N2_shipment_totalamount = 0;
                    decimal _N2_shipment_totaljcs = 0;
                    decimal _N3_movingprice = _entity.Attributes.Contains("ittn_costunitsap") ? _entity.GetAttributeValue<Money>("ittn_costunitsap").Value : new decimal(0);
                    decimal _N3_totalmovingprice = 0;

                    // -------------------- -------------------- -------------------- -------------------- --------------------
                    // N1
                    QueryExpression _queryexpression = new QueryExpression(_entityname_quoteconditiontype);
                    _queryexpression.ColumnSet = new ColumnSet(true);
                    _queryexpression.Criteria.AddCondition("ittn_quote", ConditionOperator.Equal, _quoteid);
                    _queryexpression.Criteria.AddCondition("ittn_itemnumber", ConditionOperator.Equal, _itemnumber);

                    EntityCollection _quoteconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                    if (_quoteconditiontypes.Entities.Count() > 0)
                    {
                        _N1_totalconditiontype = _quoteconditiontypes.Entities.AsEnumerable().Where(x => x.GetAttributeValue<Money>("ittn_amount").Value > 0).Sum(x => x.GetAttributeValue<Money>("ittn_amount").Value);
                    }
                    // -------------------- -------------------- -------------------- -------------------- --------------------

                    // -------------------- -------------------- -------------------- -------------------- --------------------
                    // N2
                    //if (_entity.GetAttributeValue<OptionSetValue>("new_deliveryoption").Value == _deliveryoption_franco)
                    //{
                    //    Entity _shippingpoint = _organizationservice.Retrieve(_entityname_masterroute, _entity.GetAttributeValue<EntityReference>("ittn_shippingpoint").Id, new ColumnSet(true));

                    //    _queryexpression = new QueryExpression(_entityname_mastershippingpricelist);
                    //    _queryexpression.ColumnSet = new ColumnSet(true);
                    //    _queryexpression.Criteria.AddCondition("ittn_model", ConditionOperator.Equal, _product.Id);
                    //    _queryexpression.Criteria.AddCondition("ittn_shipmentroute", ConditionOperator.Equal, _shippingpoint.Id);
                    //    _queryexpression.Criteria.AddCondition("transactioncurrencyid", ConditionOperator.Equal, _transactioncurrencyid);
                    //    _queryexpression.Criteria.AddCondition("ittn_transportplanningpoint", ConditionOperator.Equal, _businessunitid);

                    //    EntityCollection _mastershippingpricelists = _organizationservice.RetrieveMultiple(_queryexpression);

                    //    if (_mastershippingpricelists.Entities.Count() > 0)
                    //    {
                    //        Entity _mastershippingpricelist = _mastershippingpricelists.Entities[0];

                    //        _N2_shipment_totalamount = _mastershippingpricelist.GetAttributeValue<Money>("ittn_amount").Value;
                    //        //_N2_shipment_totaljcs = _mastershippingpricelist.GetAttributeValue<Money>("ittn_amountjcs").Value;
                    //    }

                    //    _queryexpression = new QueryExpression(_entityname_mastertransportandaccomodation);
                    //    _queryexpression.ColumnSet = new ColumnSet(true);
                    //    _queryexpression.Criteria.AddCondition("ittn_shipmentroute", ConditionOperator.Equal, _shippingpoint.Id);
                    //    _queryexpression.Criteria.AddCondition("transactioncurrencyid", ConditionOperator.Equal, Globalization.GetTransactionCurrencyID(_organizationservice, _entityname_transactioncurrency_code));
                    //    _queryexpression.Criteria.AddCondition("ittn_transportplanningpoint", ConditionOperator.Equal, _businessunitid);

                    //    EntityCollection _mastertransportandaccomodations = _organizationservice.RetrieveMultiple(_queryexpression);

                    //    if (_mastertransportandaccomodations.Entities.Count() > 0)
                    //    {
                    //        Entity _mastertransportandaccomodation = _mastertransportandaccomodations.Entities[0];

                    //        _N2_shipment_totaljcs = _mastertransportandaccomodation.GetAttributeValue<Money>("ittn_amountjcs").Value;
                    //    }
                    //}
                    //else
                    //{
                    //    Entity _deliveryplant = _organizationservice.Retrieve(_entityname_deliveryplant, _entity.GetAttributeValue<EntityReference>("ittn_deliveryplanbranch").Id, new ColumnSet(true));

                    //    _queryexpression = new QueryExpression(_entityname_deliveryunitloco);
                    //    _queryexpression.ColumnSet = new ColumnSet(true);
                    //    _queryexpression.Criteria.AddCondition("ittn_model", ConditionOperator.Equal, _product.Id);
                    //    _queryexpression.Criteria.AddCondition("ittn_branchdestination", ConditionOperator.Equal, _deliveryplant.Id);
                    //    _queryexpression.Criteria.AddCondition("transactioncurrencyid", ConditionOperator.Equal, _transactioncurrencyid);

                    //    EntityCollection _deliveryunitlocos = _organizationservice.RetrieveMultiple(_queryexpression);

                    //    if (_deliveryunitlocos.Entities.Count() > 0)
                    //    {
                    //        Entity _deliveryunitloco = _deliveryunitlocos.Entities[0];

                    //        _N2_shipment_totalamount = _deliveryunitloco.GetAttributeValue<Money>("ittn_shipmentamount").Value;
                    //    }
                    //}

                    // -------------------- -------------------- -------------------- -------------------- --------------------

                    // -------------------- -------------------- -------------------- -------------------- --------------------
                    // TOTAL MOVING PRICE
                    //_N3_totalmovingprice = _N1_totalconditiontype + (_N2_shipment_totaljcs - _N2_shipment_totalamount) + _N3_movingprice;

                    _N3_totalmovingprice = _N1_totalconditiontype + _N3_movingprice;
                    // -------------------- -------------------- -------------------- -------------------- --------------------

                    // -------------------- -------------------- -------------------- -------------------- --------------------
                    // TOTAL GP
                    decimal _gp = 0;
                    decimal _totalextendedamount = 0;

                    FilterExpression _filterexpression = new FilterExpression(LogicalOperator.Or);
                    _filterexpression.AddCondition("new_itemnumber", ConditionOperator.Equal, _itemnumber);
                    //_filterexpression.AddCondition("new_parentnumber", ConditionOperator.Equal, _itemnumber);

                    _queryexpression = new QueryExpression(_entityname_quotedetail);
                    _queryexpression.ColumnSet = new ColumnSet(true);
                    _queryexpression.Criteria.AddCondition("quoteid", ConditionOperator.Equal, _quoteid);
                    _queryexpression.Criteria.AddCondition("ittn_totalextendedamount", ConditionOperator.GreaterThan, new decimal(0));
                    _queryexpression.Criteria.AddFilter(_filterexpression);

                    EntityCollection _quotedetail_forcalculate = _organizationservice.RetrieveMultiple(_queryexpression);

                    if (_quotedetail_forcalculate.Entities.Count() > 0)
                    {
                        _totalextendedamount = _quotedetail_forcalculate.Entities.AsEnumerable().Sum(x => x.GetAttributeValue<Money>("ittn_totalextendedamount").Value);
                    }

                    if (_N3_totalmovingprice > 0 && _totalextendedamount > 0)
                    {
                        _gp = 100 - ((_N3_totalmovingprice / _totalextendedamount) * 100);
                    }

                    // -------------------- -------------------- -------------------- -------------------- --------------------

                    Entity _row = new Entity(_entityname_quotedetail);
                    _row.Id = _entity.Id;

                    _row["ittn_movingprice"] = new Money(_N3_totalmovingprice);
                    _row["ittn_gp"] = _gp;

                    _organizationservice.Update(_row);
                }
                else
                {
                    Entity _row = new Entity(_entityname_quotedetail);
                    _row.Id = _entity.Id;

                    _row["ittn_movingprice"] = null;
                    _row["ittn_gp"] = null;

                    _organizationservice.Update(_row);
                }

            }

        }

        public void UpdateTotalCostUnit(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _trace)
        {
            _entity = _organizationservice.Retrieve(_entityname_quotedetail, _entity.Id, new ColumnSet(true));
            Guid _quoteid = _entity.Attributes.Contains("quoteid") ? _entity.GetAttributeValue<EntityReference>("quoteid").Id : new Guid();

            if (_quoteid != new Guid())
            {
                QueryExpression _queryexpression = new QueryExpression(_entityname_quotedetail);
                _queryexpression.ColumnSet = new ColumnSet(true);
                _queryexpression.Criteria.AddCondition("quoteid", ConditionOperator.Equal, _quoteid);
                _queryexpression.Criteria.AddCondition("ittn_movingprice", ConditionOperator.NotNull);
                _queryexpression.Criteria.AddCondition("ittn_movingprice", ConditionOperator.GreaterThan, 0);

                EntityCollection _quotedetails = _organizationservice.RetrieveMultiple(_queryexpression);

                if (_quotedetails.Entities.Count() > 0)
                {
                    decimal _totalcostunit = _quotedetails.Entities.AsEnumerable().Sum(x => x.GetAttributeValue<Money>("ittn_movingprice").Value);

                    Entity _update = new Entity(_entityname_quote);
                    _update.Id = _quoteid;

                    _update["ittn_totalcostunit"] = new Money(_totalcostunit);

                    _organizationservice.Update(_update);
                }

            }
                
        }

    }
}
