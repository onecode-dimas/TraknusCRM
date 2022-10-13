using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using EnhancementCRM.HelperUnit;
using System.Web;
using System.Globalization;

namespace EnhancementCRM.Plugins.Business_Layer
{
    public class BL_quote
    {
        #region Properties
        private string _classname = "BL_quote";
        private string _entityname = "quote";

        private string _entityname_masterconditiontype = "ittn_masterconditiontype";
        private string _entityname_approvallistquoteminprice = "ittn_approvallistquoteminprice";
        private string _entityname_approvallistquotedic = "ittn_approvallistquotedic";
        private string _entityname_deliveryplant = "new_deliveryplant";
        private string _entityname_masterroute = "ittn_masterroute";
        private string _entityname_mastervendor = "ittn_mastervendor";
        private string _entityname_matrixapprovalquoteminprice = "ittn_matrixapprovalquoteminprice";
        private string _entityname_matrixapprovalquotedic = "ittn_matrixapprovalquotedic";
        private string _entityname_minprice = "new_pricelist";
        private string _entityname_productpricelevel = "productpricelevel";
        private string _entityname_opportunity = "opportunity";
        private string _entityname_opportunityproduct = "opportunityproduct";
        private string _entityname_quote = "quote";
        private string _entityname_quotedetail = "quotedetail";
        private string _entityname_quoteconditiontype = "ittn_quoteconditiontype";
        private string _entityname_systemuser = "systemuser";
        private string _entityname_activityparty = "activityparty";

        private int _deliveryoption_franco = 841150000;
        private int _deliveryoption_loco = 841150001;
        private string _defaultvendor_code = "0000000";
        private const int _participationtypemask_customer = 11;
        private const int _partyobjecttypecode_account = 1;
        #endregion

        #region Constants
        private const int MonthAdd = 1;
        #endregion

        #region Depedencies
        #endregion

        public void Form_OnCreate_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext, Entity CurrentEntity, ITracingService tracer)
        {
            try
            {
                #region Variables
                DateTime CreatedOn, OneMonthAfterCreatedOn = DateTime.MinValue;
                #endregion

                Entity Quote = organizationService.Retrieve(_entityname, CurrentEntity.Id, new ColumnSet(true));

                #region Attributes
                if (Quote.Contains("createdon") && Quote.Attributes["createdon"] != null)
                {
                    CreatedOn = Quote.GetAttributeValue<DateTime>("createdon");
                    OneMonthAfterCreatedOn = CreatedOn.AddMonths(MonthAdd);
                }
                #endregion

                #region Update Fields
                Quote["ittn_next1monthaftercreatedon"] = OneMonthAfterCreatedOn;
                organizationService.Update(Quote);
                #endregion

                // COPY CONDITION TYPE IF REVISION > 0
                int _revisionnumber = Quote.Attributes.Contains("revisionnumber") ? Quote.GetAttributeValue<int>("revisionnumber") : 0;

                if (_revisionnumber > 0)
                {
                    QueryExpression _queryexpression = new QueryExpression(_entityname);
                    _queryexpression.ColumnSet = new ColumnSet(true);
                    _queryexpression.Criteria.AddCondition("quotenumber", ConditionOperator.Equal, Quote.GetAttributeValue<string>("quotenumber"));
                    _queryexpression.Criteria.AddCondition("revisionnumber", ConditionOperator.Equal, (_revisionnumber - 1));
                    Entity _quote = organizationService.RetrieveMultiple(_queryexpression).Entities.FirstOrDefault();

                    if (_quote != null)
                    {
                        _queryexpression = new QueryExpression(_entityname_quoteconditiontype);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria.AddCondition("ittn_quote", ConditionOperator.Equal, _quote.Id);
                        EntityCollection _quoteconditiontypes = organizationService.RetrieveMultiple(_queryexpression);

                        foreach (var item in _quoteconditiontypes.Entities)
                        {
                            #region CREATE NEW CONDITION TYPE WITHIN THE NEW QUOTE
                            Entity _row = new Entity(_entityname_quoteconditiontype);
                            _row = item;
                            _row.Id = Guid.Empty;
                            _row.Attributes.Remove("ittn_quoteconditiontypeid");

                            _row["ittn_quote"] = new EntityReference(_entityname_quote, Quote.Id);

                            organizationService.Create(_row);
                            #endregion ---
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PostOperation: " + ex.Message.ToString());
            }
        }

        public void PostCreate_quote_approveminprice(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        {
            try
            {
                _entity = _organizationservice.Retrieve(_entityname_quote, _entity.Id, new ColumnSet(true));
                CultureInfo culture = new CultureInfo("id-ID");

                if (_entity != null)
                {
                    Guid _quoteid = _entity.Id;
                    Guid _opportunityid = _entity.GetAttributeValue<EntityReference>("opportunityid").Id;
                    Guid _unitgroupid = _entity.GetAttributeValue<EntityReference>("new_unitgroup").Id;
                    Entity _opportunity = _organizationservice.Retrieve(_entityname_opportunity, _opportunityid, new ColumnSet(true));
                    Entity _quote = _organizationservice.Retrieve(_entityname_quote, _quoteid, new ColumnSet(true));
                    int _revisionnumber = _entity.GetAttributeValue<int>("revisionnumber");
                    string CustomerName = string.Empty;

                    Entity _owner = _organizationservice.Retrieve(_entityname_systemuser, _entity.GetAttributeValue<EntityReference>("ownerid").Id, new ColumnSet(true));
                    Guid _businessunitid = _owner.Attributes.Contains("businessunitid") ? _owner.GetAttributeValue<EntityReference>("businessunitid").Id : new Guid();

                    QueryExpression _queryexpression = new QueryExpression(_entityname_opportunityproduct);
                    _queryexpression.ColumnSet = new ColumnSet(true);
                    _queryexpression.Criteria.AddCondition("opportunityid", ConditionOperator.Equal, _opportunityid);
                    EntityCollection _opportunityproducts = _organizationservice.RetrieveMultiple(_queryexpression);

                    decimal _totalbaseamount = 0;
                    decimal _totalminprice = 0;

                    foreach (var _opportunityproduct in _opportunityproducts.Entities)
                    {
                        _queryexpression = new QueryExpression(_entityname_productpricelevel);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria.AddCondition("productid", ConditionOperator.Equal, _opportunityproduct.GetAttributeValue<EntityReference>("productid").Id);
                        _queryexpression.Criteria.AddCondition("transactioncurrencyid", ConditionOperator.Equal, _opportunityproduct.GetAttributeValue<EntityReference>("transactioncurrencyid").Id);

                        EntityCollection _productpricelevels = _organizationservice.RetrieveMultiple(_queryexpression);

                        if (_productpricelevels.Entities.Count() > 0)
                        {
                            Entity _productpricelevel = _productpricelevels.Entities.FirstOrDefault();

                            _totalbaseamount += _productpricelevel.GetAttributeValue<Money>("amount").Value;

                            // GET MIN PRICE
                            _queryexpression = new QueryExpression(_entityname_minprice);
                            _queryexpression.ColumnSet = new ColumnSet(true);
                            _queryexpression.Criteria.AddCondition("new_pricelist", ConditionOperator.Equal, _productpricelevel.GetAttributeValue<EntityReference>("pricelevelid").Id);
                            _queryexpression.Criteria.AddCondition("new_item", ConditionOperator.Equal, _opportunityproduct.GetAttributeValue<EntityReference>("productid").Id);
                            EntityCollection _minprices = _organizationservice.RetrieveMultiple(_queryexpression);

                            if (_minprices.Entities.Count() > 0)
                            {
                                Entity _minprice = _minprices.Entities.FirstOrDefault();

                                _totalminprice += (_minprice.GetAttributeValue<Money>("new_minimumprice").Value) * (_opportunityproduct.GetAttributeValue<decimal>("quantity"));
                            }

                        }
                    }

                    //decimal _totalbaseamount = _opportunityproducts.Entities.AsEnumerable().Where(x => x.GetAttributeValue<Money>("baseamount").Value > 0).Sum(x => x.GetAttributeValue<Money>("baseamount").Value);
                    decimal _totalextendedamount = _opportunityproducts.Entities.AsEnumerable().Where(x => x.GetAttributeValue<Money>("ittn_totalextendedamount").Value > 0).Sum(x => x.GetAttributeValue<Money>("ittn_totalextendedamount").Value);

                    #region Pengambilan data Customer from ActivityParty [13-07-2020]
                    if (_quote.Contains("customerid"))
                    {
                        #region Comments
                        //QueryExpression queryExpression = new QueryExpression(_entityname_activityparty);
                        //queryExpression.ColumnSet = new ColumnSet(true);
                        //FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                        //filterExpression.AddCondition("activityid", ConditionOperator.Equal, _quoteid);
                        //filterExpression.AddCondition("participationtypemask", ConditionOperator.Equal, _participationtypemask_customer);
                        //filterExpression.AddCondition("partyobjecttypecode", ConditionOperator.Equal, _partyobjecttypecode_account);
                        //EntityCollection ecActivityParty = _organizationservice.RetrieveMultiple(queryExpression);

                        //if (ecActivityParty.Entities.Count > 0)
                        //{
                        //    Entity enActivityParty = ecActivityParty.Entities[0];
                        //    if (enActivityParty.Contains("partyid") && enActivityParty.Attributes["partyid"] != null)
                        //    {
                        //        Entity enAccount = _organizationservice.Retrieve("account", ((EntityReference)enActivityParty.Attributes["partyid"]).Id, new ColumnSet(true));
                        //        CustomerName = enAccount.Contains("name") ? enAccount.Attributes["name"].ToString() : string.Empty;
                        //    }
                        //}
                        #endregion
                        Entity enAccount = _organizationservice.Retrieve("account", _quote.GetAttributeValue<EntityReference>("customerid").Id, new ColumnSet(true));
                        CustomerName = enAccount.Contains("name") ? enAccount.Attributes["name"].ToString() : string.Empty;
                    }
                    #endregion

                    // CHECK QUOTE DIC / QUOTE MIN PRICE
                    if (_totalextendedamount < _totalminprice)
                    {
                        // QUOTE DIC
                        _queryexpression = new QueryExpression(_entityname_matrixapprovalquotedic);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Orders.Add(new OrderExpression("ittn_priorityno", OrderType.Ascending));
                        _queryexpression.Criteria.AddCondition("ittn_unitgroup", ConditionOperator.Equal, _unitgroupid);

                        EntityCollection _matrixapprovalquotedics = _organizationservice.RetrieveMultiple(_queryexpression);

                        if (_matrixapprovalquotedics.Entities.Count() > 0)
                        {
                            Entity _firstmatrixapprover = new Entity(_entityname_matrixapprovalquotedic);
                            Guid _currentapproverid = new Guid();
                            int _index = 0;

                            foreach (var _matrixapprovalquotedic in _matrixapprovalquotedics.Entities)
                            {
                                Guid _approverid = _matrixapprovalquotedic.GetAttributeValue<EntityReference>("ittn_approver").Id;
                                Entity _target = _organizationservice.Retrieve(_entityname_systemuser, _approverid, new ColumnSet(true));

                                #region Create
                                Entity _approvallistquotedic = new Entity(_entityname_approvallistquotedic);
                                _approvallistquotedic["ittn_approver"] = new EntityReference(_entityname_systemuser, _approverid);
                                _approvallistquotedic["ittn_name"] = _target.GetAttributeValue<string>("fullname");
                                _approvallistquotedic["ittn_quote"] = new EntityReference(_entityname_quote, _quoteid);
                                _organizationservice.Create(_approvallistquotedic);
                                #endregion ---

                                #region Share Record
                                ShareRecords _sharerecords = new ShareRecords();

                                _sharerecords.ShareRecord(_organizationservice, _entity, _target);
                                #endregion ---

                                if (_index == 0)
                                {
                                    _firstmatrixapprover = _matrixapprovalquotedic;
                                    _currentapproverid = _approverid;
                                }

                                _index += 1;
                            }

                            Entity _currentapprover = _organizationservice.Retrieve(_entityname_systemuser, _currentapproverid, new ColumnSet(true));

                            #region Update
                            Entity _quote_update = new Entity(_entityname_quote);
                            _quote_update.Id = _entity.Id;

                            _quote_update["ittn_diccurrentapprover"] = new EntityReference(_entityname_matrixapprovalquotedic, _firstmatrixapprover.Id);
                            _quote_update["ittn_reqapprovedicdate"] = DateTime.Now;

                            if (_revisionnumber == 0)
                                _quote_update["ittn_statusreason"] = new OptionSetValue(841150006); // WAITING APPROVAL DIC
                            else
                            {
                                _quote_update["ittn_statusreason"] = new OptionSetValue(841150002); // APPROVED
                                _quote_update["new_activatelock"] = new OptionSetValue(3); //APPROVED
                            }

                            if (_quote_update.Attributes.Contains("new_underminprice"))
                                _quote_update["new_underminprice"] = true;
                            else
                                _quote_update.Attributes.Add("new_underminprice", true);

                            _organizationservice.Update(_quote_update);
                            #endregion ---

                            #region Send Email
                            String _stringpathandquery = HttpContext.Current.Request.Url.PathAndQuery;
                            string CRM_URL = HttpContext.Current.Request.Url.AbsoluteUri.Replace(_stringpathandquery, "/");
                            CRM_URL += _context.OrganizationName; // "TraktorNusantara";

                            string _objecttypecode = string.Empty;
                            string _emailsubject = "[NEED APPROVAL] " + _quote.GetAttributeValue<string>("quotenumber");
                            string _emailcontent = "";

                            Globalization.GetObjectTypeCode(_organizationservice, "quote", out _objecttypecode);

                            _emailcontent += "Dear " + _currentapprover.GetAttributeValue<string>("fullname") + ",<br/><br/>";
                            _emailcontent += "Need Your Approval DIC for Quotation below:<br/><br/>";
                            _emailcontent += "<ul>";
                            _emailcontent += "<li>Quotation Number : " + _quote.GetAttributeValue<string>("quotenumber") + "</li>";
                            _emailcontent += "<li>Customer : " + CustomerName + "</li>";
                            _emailcontent += "<li>Topic : " + _opportunity.GetAttributeValue<string>("name") + "</li>";
                            _emailcontent += "<li>Sales Amount : " + (_opportunity.GetAttributeValue<Money>("totalamount")).Value.ToString("C", culture) + "</li>";
                            _emailcontent += "</ul><br/>";
                            _emailcontent += "<a href='" + CRM_URL + "/main.aspx?etc=" + _objecttypecode + "&pagetype=entityrecord&id=%7b" + _entity.Id + "%7d'>Click here to open in CRM.</a><br/><br/>";

                            _emailcontent += "Thanks,<br/><br/>";
                            _emailcontent += "Regards,<br/><br/>";
                            _emailcontent += _quote.GetAttributeValue<EntityReference>("createdby").Name;

                            EmailAgent _emailagent = new EmailAgent();
                            _emailagent.SendNotification(_context.UserId, _currentapprover.Id, Guid.Empty, _organizationservice, _emailsubject, _emailcontent);
                            #endregion ---

                        }
                        else
                        {
                            throw new InvalidPluginExecutionException("CANNOT FIND MATRIX APPROVAL QUOTE DISC !");
                        }
                    }
                    else
                    {
                        decimal _totalamountpercentage = 0;

                        if (_totalbaseamount > 0)
                            _totalamountpercentage = (1 - (_totalextendedamount / _totalbaseamount)) * 100;

                        if (_totalamountpercentage > 0)
                        {
                            // QUOTE MIN PRICE
                            _queryexpression = new QueryExpression(_entityname_matrixapprovalquoteminprice);
                            _queryexpression.ColumnSet = new ColumnSet(true);
                            _queryexpression.Orders.Add(new OrderExpression("ittn_priorityno", OrderType.Ascending));
                            _queryexpression.Criteria.AddCondition("ittn_startpercentage", ConditionOperator.LessEqual, _totalamountpercentage);
                            _queryexpression.Criteria.AddCondition("ittn_endpercentage", ConditionOperator.GreaterEqual, _totalamountpercentage);
                            _queryexpression.Criteria.AddCondition("ittn_unitgroup", ConditionOperator.Equal, _unitgroupid);
                            _queryexpression.Criteria.AddCondition("ittn_needapproval", ConditionOperator.Equal, true);
                            _queryexpression.Criteria.AddCondition("ittn_businessunit", ConditionOperator.Equal, _businessunitid);

                            EntityCollection _matrixapprovalquoteminprices = _organizationservice.RetrieveMultiple(_queryexpression);

                            if (_matrixapprovalquoteminprices.Entities.Count() > 0)
                            {
                                Entity _firstmatrixapprover = new Entity(_entityname_matrixapprovalquoteminprice);
                                Guid _currentapproverid = new Guid();
                                int _index = 0;

                                foreach (var _matrixapprovalquoteminprice in _matrixapprovalquoteminprices.Entities)
                                {
                                    Guid _approverid = _matrixapprovalquoteminprice.GetAttributeValue<EntityReference>("ittn_approver").Id;
                                    Entity _target = _organizationservice.Retrieve(_entityname_systemuser, _approverid, new ColumnSet(true));

                                    #region Create
                                    Entity _approvallistquoteminprice = new Entity(_entityname_approvallistquoteminprice);
                                    _approvallistquoteminprice["ittn_approver"] = new EntityReference(_entityname_systemuser, _approverid);
                                    _approvallistquoteminprice["ittn_name"] = _target.GetAttributeValue<string>("fullname");
                                    _approvallistquoteminprice["ittn_quote"] = new EntityReference(_entityname_quote, _quoteid);
                                    _organizationservice.Create(_approvallistquoteminprice);
                                    #endregion ---

                                    #region Share Record
                                    ShareRecords _sharerecords = new ShareRecords();

                                    _sharerecords.ShareRecord(_organizationservice, _entity, _target);
                                    #endregion ---

                                    if (_index == 0)
                                    {
                                        _firstmatrixapprover = _matrixapprovalquoteminprice;
                                        _currentapproverid = _approverid;
                                    }

                                    _index += 1;
                                }

                                Entity _currentapprover = _organizationservice.Retrieve(_entityname_systemuser, _currentapproverid, new ColumnSet(true));

                                #region Update
                                Entity _quote_update = new Entity(_entityname_quote);
                                _quote_update.Id = _entity.Id;

                                _quote_update["ittn_minpricecurrentapprover"] = new EntityReference(_entityname_matrixapprovalquoteminprice, _firstmatrixapprover.Id);
                                _quote_update["ittn_reqapproveminpricedate"] = DateTime.Now;

                                if (_revisionnumber == 0)
                                    _quote_update["ittn_statusreason"] = new OptionSetValue(841150001); // WAITING APPROVAL MIN PRICE
                                else
                                {
                                    _quote_update["ittn_statusreason"] = new OptionSetValue(841150002); // APPROVED
                                    _quote_update["new_activatelock"] = new OptionSetValue(3); //APPROVED
                                }

                                if (_quote_update.Attributes.Contains("new_underminprice"))
                                    _quote_update["new_underminprice"] = false;
                                else
                                    _quote_update.Attributes.Add("new_underminprice", false);

                                _organizationservice.Update(_quote_update);
                                #endregion ---

                                #region Send Email
                                String _stringpathandquery = HttpContext.Current.Request.Url.PathAndQuery;
                                string CRM_URL = HttpContext.Current.Request.Url.AbsoluteUri.Replace(_stringpathandquery, "/");
                                CRM_URL += _context.OrganizationName; // "TraktorNusantara";

                                string _objecttypecode = string.Empty;
                                string _emailsubject = "[NEED APPROVAL] " + _quote.GetAttributeValue<string>("quotenumber");
                                string _emailcontent = "";
                                
                                Globalization.GetObjectTypeCode(_organizationservice, "quote", out _objecttypecode);

                                _emailcontent += "Dear " + _currentapprover.GetAttributeValue<string>("fullname") + ",<br/><br/>";
                                _emailcontent += "Need Your Approval Min Price for Quotation below:<br/><br/>";
                                _emailcontent += "<ul>";
                                _emailcontent += "<li>Quotation Number : " + _quote.GetAttributeValue<string>("quotenumber") + "</li>";
                                _emailcontent += "<li>Customer : " + CustomerName + "</li>";
                                _emailcontent += "<li>Topic : " + _opportunity.GetAttributeValue<string>("name") + "</li>";
                                _emailcontent += "<li>Sales Amount : " + (_opportunity.GetAttributeValue<Money>("totalamount")).Value.ToString("C", culture) + "</li>";
                                _emailcontent += "</ul><br/>";
                                _emailcontent += "<a href='" + CRM_URL + "/main.aspx?etc=" + _objecttypecode + "&pagetype=entityrecord&id=%7b" + _entity.Id + "%7d'>Click here to open in CRM.</a><br/><br/>";

                                _emailcontent += "Thanks,<br/><br/>";
                                _emailcontent += "Regards,<br/><br/>";
                                _emailcontent += _quote.GetAttributeValue<EntityReference>("createdby").Name;

                                EmailAgent _emailagent = new EmailAgent();
                                _emailagent.SendNotification(_context.UserId, _currentapprover.Id, Guid.Empty, _organizationservice, _emailsubject, _emailcontent);
                                #endregion ---
                            }
                            else
                            {
                                throw new InvalidPluginExecutionException("CANNOT FIND MATRIX APPROVAL QUOTE MIN PRICE !");
                            }
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".PostCreate_quote_approveminprice: " + ex.Message.ToString());
            }
        }

        public void PostUpdate_quote_isshippingcentralized(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        {
            try
            {
                _entity = _organizationservice.Retrieve(_entityname_quote, _entity.Id, new ColumnSet(true));

                if (_entity != null)
                {
                    bool _isshippingcentralized = _entity.Attributes.Contains("ittn_isshippingcentralized") ? _entity.GetAttributeValue<bool>("ittn_isshippingcentralized") : false;

                    if (_isshippingcentralized)
                    {
                        QueryExpression _queryexpression = new QueryExpression(_entityname_quotedetail);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria.AddCondition("quoteid", ConditionOperator.Equal, _entity.Id);
                        EntityCollection _quotedetails = _organizationservice.RetrieveMultiple(_queryexpression);

                        foreach (var _quotedetail in _quotedetails.Entities)
                        {
                            Entity _row = new Entity(_entityname_quotedetail);
                            _row.Id = _quotedetail.Id;

                            _row["new_deliveryoption"] = new OptionSetValue(_entity.GetAttributeValue<OptionSetValue>("ittn_deliveryoption").Value);
                            //_row["ittn_nondefaultbom"] = _entity.GetAttributeValue<bool>("ittn_nondefaultbom");

                            if (_entity.GetAttributeValue<OptionSetValue>("ittn_deliveryoption").Value == _deliveryoption_franco)
                            {
                                Guid _shippingpointid = _entity.GetAttributeValue<EntityReference>("ittn_shippingpoint").Id;
                                Guid _vendorid;

                                _queryexpression = new QueryExpression(_entityname_mastervendor);
                                _queryexpression.ColumnSet = new ColumnSet(true);
                                _queryexpression.Criteria.AddCondition("ittn_code", ConditionOperator.Equal, _defaultvendor_code);
                                EntityCollection _mastervendors = _organizationservice.RetrieveMultiple(_queryexpression);

                                if (_mastervendors.Entities.Count() > 0)
                                {
                                    _vendorid = _mastervendors.Entities.FirstOrDefault().Id;
                                }
                                else
                                    throw new InvalidPluginExecutionException("Doesn't found default vendor for current destination!");

                                //if (!_entity.Contains("ittn_vendor"))
                                //    throw new InvalidPluginExecutionException("Doesn't found shipping price for current destination!");
                                //else
                                //    _vendorid = _entity.GetAttributeValue<EntityReference>("ittn_vendor").Id;

                                string _shippingaddress = _entity.GetAttributeValue<string>("ittn_shippingaddress");

                                _row["ittn_shippingpoint"] = new EntityReference(_entityname_masterroute, _shippingpointid);
                                _row["ittn_vendor"] = new EntityReference(_entityname_mastervendor, _vendorid);
                                _row["ittn_shippingaddress"] = _shippingaddress;

                                _row["ittn_deliveryplanbranch"] = null;
                            }
                            else
                            {
                                Guid _deliverybranchid = _entity.GetAttributeValue<EntityReference>("ittn_deliverybranch").Id;
                                _row["ittn_deliveryplanbranch"] = new EntityReference(_entityname_deliveryplant, _deliverybranchid);

                                _row["ittn_shippingpoint"] = null;
                                _row["ittn_vendor"] = null;
                                _row["ittn_shippingaddress"] = null;
                            }

                            _organizationservice.Update(_row);
                        }
                    }
   
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".PostUpdate_quote_isshippingcentralized: " + ex.Message.ToString());
            }
        }

        public void PostUpdate_quote_paymentterm(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        {
            try
            {
                _entity = _organizationservice.Retrieve(_entityname_quote, _entity.Id, new ColumnSet(true));

                if (_entity != null)
                {
                    if (_entity.Attributes.Contains("new_paymentterm"))
                    {
                        QueryExpression _queryexpression = new QueryExpression(_entityname_quotedetail);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria.AddCondition("quoteid", ConditionOperator.Equal, _entity.Id);
                        EntityCollection _quotedetails = _organizationservice.RetrieveMultiple(_queryexpression);

                        foreach (var _quotedetail in _quotedetails.Entities)
                        {
                            BL_quotedetail _QUOTEDETAIL = new BL_quotedetail();
                            _QUOTEDETAIL.CostUnitSAP(_organizationservice, _context, _quotedetail, _tracer);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".PostUpdate_quote_isshippingcentralized: " + ex.Message.ToString());
            }
        }

    }
}
