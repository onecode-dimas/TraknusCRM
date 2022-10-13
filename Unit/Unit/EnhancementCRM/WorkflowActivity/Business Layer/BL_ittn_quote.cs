using EnhancementCRM.HelperUnit;
using EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EnhancementCRM.WorkflowActivity.Business_Layer
{
    public class BL_ittn_quote
    {
        #region Depedencies
        Generator _generator = new Generator();
        MWSLog _mwslog = new MWSLog();
        #endregion ---

        private string _entityname_businessunit = "businessunit";
        private string _entityname_masterconditiontype = "ittn_masterconditiontype";
        private string _entityname_matrixapprovalconditiontype = "ittn_matrixapprovalconditiontype";
        private string _entityname_product = "product";
        private string _entityname_quote = "quote";
        private string _entityname_quotedetail = "quotedetail";
        private string _entityname_quoteconditiontype = "ittn_quoteconditiontype";
        private string _entityname_transactioncurrency = "transactioncurrency";
        private string _entityname_transactioncurrency_code = "IDR";
        private string _entityname_systemuser = "systemuser";
        private string _entityname_materialgroup = "new_materialgroup";
        private string _entityname_workflowconfiguration = "trs_workflowconfiguration";
        private string _entityname_workflowconfiguration_name = "TRS";
        private string _entityname_workflowconfiguration_primaryfieldname = "trs_generalconfig";
        private const string ConfigurationWebServiceEntityName = "ittn_webservicesconfiguration";
        private const int WebService_CheckStock = 841150001;

        // CODE
        private const string CODE_PERSONALDISCOUNT = "ZCB0";
        private const string CODE_FAT = "ZFAT";
        private const string CODE_VOUCHER = "ZVCR";
        private const string CODE_OTHERS = "ZO99";
        private const string PRODUCT_PRODUCTNUMBER_PAINTING = "PAINTING";
        private const string PRODUCT_MATERIALGROUP_PAINTING = "S-PAINT";

        //private const int APPROVEBY_SALESMANAGER = 841150000;
        private const int APPROVEBY_GENERALMANAGER = 841150001;
        private const int APPROVEBY_DIRECTOR = 841150002;

        private const int STATUS_NOTASSIGN = 841150000;
        private const int STATUS_ACTIVE = 841150001;
        private const int STATUS_INACTIVE = 841150002;
        private const int STATUS_INPROGRESS = 841150003;
        private const int PRODUCT_PRODUCTTYPE_SERVICE = 7;

        public void CheckStock(IOrganizationService _organizationservice, IWorkflowContext _context, string[] _recordids, ITracingService _trace)
        {
            MWSLog _mwslog = new MWSLog();
            string _token = string.Empty;
            string _usercredentials = string.Empty;
            Guid _quoteid = new Guid(_recordids[0]);
            string _sapwebservice = string.Empty;
            string _sapintegrationuniquekey = string.Empty;
            string _sapwebserviceusername = string.Empty;
            string _sapwebservicepassword = string.Empty;
            string _plant = null;

            Entity _quote = _organizationservice.Retrieve(_entityname_quote, _quoteid, new ColumnSet(true));
            //string _quotenumber = _quote.GetAttributeValue<string>("quotenumber");

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
                WSCPO.Criteria.AddCondition("ittn_webservicefor", ConditionOperator.Equal, WebService_CheckStock);
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
                    throw new InvalidPluginExecutionException("Web Service for Update Work Order Warranty is null/empty!");
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

            ZPST_FM_002_v7Client _client = new ZPST_FM_002_v7Client(_httpbinding, _remoteaddress);
            _client.ClientCredentials.UserName.UserName = _sapwebserviceusername;
            _client.ClientCredentials.UserName.Password = _sapwebservicepassword;

            _queryexpression = new QueryExpression(_entityname_quotedetail);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition("quoteid", ConditionOperator.Equal, _quoteid);
            EntityCollection _quotedetails = _organizationservice.RetrieveMultiple(_queryexpression);

            ZPST_FM_002Request _saprequest = new ZPST_FM_002Request();
            ZPST_FM_002Response _sapresponse = new ZPST_FM_002Response();
            ZPST_FM_002 _quotedetail_checkstock = new ZPST_FM_002();

            _trace.Trace("Start Getting Quote Data");
            string _parameterlog = null;

            foreach (var _quotedetail in _quotedetails.Entities)
            {
                _quotedetail_checkstock = new ZPST_FM_002();

                Entity _product = _organizationservice.Retrieve(_entityname_product, _quotedetail.GetAttributeValue<EntityReference>("productid").Id, new ColumnSet(true));

                string _materialname = _product.GetAttributeValue<string>("name");
                string _uom = "PC";

                //string _materialname = "2654407";
                //string _uom = "PC";

                _quotedetail_checkstock.CSRF_TOKEN = _generator.Encrypt(_materialname, _sapintegrationuniquekey);
                _quotedetail_checkstock.MATERIAL = _materialname;
                _quotedetail_checkstock.PLANT = _plant;
                _quotedetail_checkstock.UOM = _uom;

                try
                {
                    _trace.Trace("Open Client WebService");

                    _client = new ZPST_FM_002_v7Client(_httpbinding, _remoteaddress);
                    _client.Open();
                    _sapresponse = _client.ZPST_FM_002(_quotedetail_checkstock);
                }
                catch (Exception _exception)
                {
                    throw new InvalidWorkflowException(_exception.Message);
                    _mwslog.Write(MethodBase.GetCurrentMethod().Name, "Failed to Check Stock on SAP. Details : " + _exception.Message, MWSLog.LogType.Error, MWSLog.Source.Outbound);
                }
                finally
                {
                    _client.Close();
                }

                if (_sapresponse != null)
                {
                    string _errormessage = string.Empty;

                    Entity _detail = new Entity(_entityname_quotedetail);
                    _detail.Id = _quotedetail.Id;

                    _detail["ittn_stockquantity"] = _sapresponse.Z_VALUES[0].WKBST;

                    _organizationservice.Update(_detail);
                }
            }
        }

        public void SendMail(IOrganizationService _organizationservice, IWorkflowContext _context, Entity _masterconditiontype, Entity _quote, Entity _sender, Entity _receiver, Guid _cc)
        {
            String _stringpathandquery = HttpContext.Current.Request.Url.PathAndQuery;
            string CRM_URL = HttpContext.Current.Request.Url.AbsoluteUri.Replace(_stringpathandquery, "/");
            CRM_URL += _context.OrganizationName; // "TraktorNusantara";

            string _objecttypecode = string.Empty;
            string _emailsubject = "REQUEST [" + _masterconditiontype.GetAttributeValue<string>("ittn_description") + "] - [" + _quote.GetAttributeValue<string>("name") + "] - [" + _sender.GetAttributeValue<string>("fullname") + "]";
            string _emailcontent = "";

            //throw new InvalidWorkflowException("IDR " + string.Format("{0:#,0.00;- #,0.00;'-'}", _quote.GetAttributeValue<Money>("totalamount").Value));

            Globalization.GetObjectTypeCode(_organizationservice, "quote", out _objecttypecode);

            _emailcontent += "Dear " + _receiver.GetAttributeValue<string>("fullname") + ",<br/><br/>";
            _emailcontent += "There is request for " + _masterconditiontype.GetAttributeValue<string>("ittn_description") + " with details below :<br/><br/>";
            _emailcontent += "<ul>";
            _emailcontent += "<li>Quotation Number : " + _quote.GetAttributeValue<string>("quotenumber") + "</li>";
            _emailcontent += "<li>Customer : " + _quote.GetAttributeValue<EntityReference>("customerid").Name + "</li>";
            _emailcontent += "<li>BC : " + _quote.GetAttributeValue<EntityReference>("createdby").Name + "</li>";
            _emailcontent += "<li>Total Sales Amount : IDR " + string.Format("{0:#,0.00;- #,0.00;'-'}", _quote.GetAttributeValue<Money>("totalamount").Value) + "</li>";
            _emailcontent += "</ul><br/>";
            _emailcontent += "You can Approve / Reject the request from <a href='" + CRM_URL + "/main.aspx?etc=" + _objecttypecode + "&pagetype=entityrecord&id=%7b" + _quote.Id + "%7d'>here</a>.<br/><br/>";

            _emailcontent += "Thanks.<br/><br/>";
            _emailcontent += "Regards,<br/>";
            _emailcontent += _quote.GetAttributeValue<EntityReference>("createdby").Name;

            EmailAgent _emailagent = new EmailAgent();
            _emailagent.SendNotification(_sender.Id, _receiver.Id, _cc, _organizationservice, _emailsubject, _emailcontent);
        }

        // ---------------------------------------- ---------------------------------------- ---------------------------------------- ----------------------------------------
        #region PERSONAL DISCOUNT

        public void AddPersonalDiscount(IOrganizationService _organizationservice, IWorkflowContext _context, string[] _recordids)
        {
            Guid _quoteid = new Guid(_recordids[0]);
            Guid _masterconditiontypeid = new Guid();

            Entity _quote = _organizationservice.Retrieve(_entityname_quote, _quoteid, new ColumnSet(true));

            QueryExpression _queryexpression = new QueryExpression(_entityname_masterconditiontype);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition("ittn_code", ConditionOperator.Equal, CODE_PERSONALDISCOUNT);
            EntityCollection _masterconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

            if (_masterconditiontypes.Entities.Count() == 0)
                throw new InvalidWorkflowException("There is no Code '" + CODE_PERSONALDISCOUNT + "' in Master Condition Type!");
            else
                _masterconditiontypeid = _masterconditiontypes.Entities.FirstOrDefault().Id;

            _queryexpression = new QueryExpression(_entityname_quotedetail);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition("quoteid", ConditionOperator.Equal, _quoteid);
            _queryexpression.Criteria.AddCondition("new_parentnumber", ConditionOperator.Null);
            EntityCollection _quotedetails = _organizationservice.RetrieveMultiple(_queryexpression);

            foreach (var _quotedetail in _quotedetails.Entities)
            {
                decimal _amount = 0;
                decimal _totalamount = 0;
                decimal _itemnumber = _quotedetail.GetAttributeValue<decimal>("new_itemnumber");
                decimal _quantity = _quotedetail.GetAttributeValue<decimal>("new_quantity");

                if (_quote.Attributes.Contains("ittn_personaldiscountamount"))
                {
                    _amount = _quote.GetAttributeValue<Money>("ittn_personaldiscountamount").Value;
                    _totalamount = _amount;
                }

                if (_quantity > 0)
                    _amount = _amount / _quantity;

                //_queryexpression = new QueryExpression(_entityname_quoteconditiontype);
                //_queryexpression.ColumnSet = new ColumnSet(false);
                //_queryexpression.Criteria.AddCondition("ittn_quote", ConditionOperator.Equal, _quoteid);
                //_queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontypeid);

                //EntityCollection _quoteconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                //if (_quoteconditiontypes.Entities.Count() > 0)
                //    throw new InvalidWorkflowException("Code '" + CODE_PERSONALDISCOUNT + "' ( Personal Discount ) already exist in this Quotation Condition Type!");
                //else
                //{
                Entity _quoteconditiontype = new Entity(_entityname_quoteconditiontype);

                _quoteconditiontype["ittn_amount"] = new Money(_amount);
                _quoteconditiontype["ittn_totalamount"] = new Money(_totalamount);
                _quoteconditiontype["ittn_conditiontype"] = new EntityReference(_entityname_masterconditiontype, _masterconditiontypeid);
                _quoteconditiontype["ittn_name"] = _masterconditiontypes.Entities.FirstOrDefault().GetAttributeValue<string>("ittn_description");
                _quoteconditiontype["ittn_quote"] = new EntityReference(_entityname_quote, _quoteid);
                _quoteconditiontype["ittn_itemnumber"] = _itemnumber;
                _quoteconditiontype["ittn_needapproveconditiontype"] = _masterconditiontypes.Entities.FirstOrDefault().GetAttributeValue<bool>("ittn_needapproval");
                _quoteconditiontype["ittn_statusreason"] = new OptionSetValue(STATUS_NOTASSIGN);
                _quoteconditiontype["transactioncurrencyid"] = new EntityReference(_entityname_transactioncurrency, Globalization.GetTransactionCurrencyID(_organizationservice, _entityname_transactioncurrency_code));

                _organizationservice.Create(_quoteconditiontype);
                //}
            }

        }

        public void RequestApprovalForPersonalDiscount(IOrganizationService _organizationservice, IWorkflowContext _context, string[] _recordids)
        {
            Guid _quoteid = new Guid(_recordids[0]);
            Entity _masterconditiontype = new Entity(_entityname_masterconditiontype);
            Entity _matrixapprovalconditiontype = new Entity(_entityname_matrixapprovalconditiontype);
            Entity _quote = _organizationservice.Retrieve(_entityname_quote, _quoteid, new ColumnSet(true));
            Entity _owner = _organizationservice.Retrieve(_entityname_systemuser, _quote.GetAttributeValue<EntityReference>("createdby").Id, new ColumnSet(true));
            Guid _unitgroupid = _quote.GetAttributeValue<EntityReference>("new_unitgroup").Id;

            QueryExpression _queryexpression = new QueryExpression(_entityname_masterconditiontype);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition("ittn_code", ConditionOperator.Equal, CODE_PERSONALDISCOUNT);
            EntityCollection _masterconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

            if (_masterconditiontypes.Entities.Count() == 0)
                throw new InvalidWorkflowException("There is no Code '" + CODE_PERSONALDISCOUNT + "' in Master Condition Type!");
            else
                _masterconditiontype = _masterconditiontypes.Entities.FirstOrDefault();

            // GET MATRIX APPROVAL
            _queryexpression = new QueryExpression(_entityname_matrixapprovalconditiontype);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Orders.Add(new OrderExpression("ittn_priorityno", OrderType.Ascending));
            _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontype.Id);
            _queryexpression.Criteria.AddCondition("ittn_unitgroup", ConditionOperator.Equal, _unitgroupid);
            _queryexpression.Criteria.AddCondition("ittn_approveby", ConditionOperator.Equal, APPROVEBY_GENERALMANAGER);
            EntityCollection _matrixapprovalconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

            if (_matrixapprovalconditiontypes.Entities.Count() == 0)
                throw new InvalidWorkflowException("There is no Approver for code '" + CODE_PERSONALDISCOUNT + "' with level 'GENERAL MANAGER' in Master Condition Type!");
            else
                _matrixapprovalconditiontype = _matrixapprovalconditiontypes.Entities.FirstOrDefault();

            // SHARE RECORDS
            Entity _currentapprover = _organizationservice.Retrieve(_entityname_systemuser, _matrixapprovalconditiontype.GetAttributeValue<EntityReference>("ittn_approver").Id, new ColumnSet(true));
            ShareRecords _sharerecords = new ShareRecords();
            _sharerecords.ShareRecord(_organizationservice, _quote, _currentapprover);

            // UPDATE QUOTE
            Entity _quote_update = new Entity(_entityname_quote);
            _quote_update.Id = _quoteid;

            _quote_update["ittn_needapprovepersonaldiscount"] = true;
            _quote_update["ittn_reqapprovepersonaldiscountdate"] = DateTime.Now.ToLocalTime();
            _quote_update["ittn_currentapproverpersonaldiscount"] = new EntityReference(_entityname_matrixapprovalconditiontype, _matrixapprovalconditiontype.Id);

            _organizationservice.Update(_quote_update);

            // UPDATE QUOTE PRODUCT
            _queryexpression = new QueryExpression(_entityname_quoteconditiontype);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition("ittn_quote", ConditionOperator.Equal, _quoteid);
            _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontype.Id);
            _queryexpression.Criteria.AddCondition("ittn_statusreason", ConditionOperator.Equal, STATUS_NOTASSIGN);
            EntityCollection _quoteconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

            foreach (var _quoteconditiontype in _quoteconditiontypes.Entities)
            {
                if (_quoteconditiontype.GetAttributeValue<Money>("ittn_amount").Value > 0)
                {
                    Entity _quoteconditiontype_forupdate = new Entity(_entityname_quoteconditiontype);
                    _quoteconditiontype_forupdate.Id = _quoteconditiontype.Id;

                    _quoteconditiontype_forupdate["ittn_statusreason"] = new OptionSetValue(STATUS_INPROGRESS);

                    _organizationservice.Update(_quoteconditiontype_forupdate);
                }
                else
                {
                    throw new InvalidWorkflowException("Amount PERSONAL DISCOUNT must be greater than 0 !");
                }
            }

            // SEND EMAIL
            SendMail(_organizationservice, _context, _masterconditiontype, _quote, _owner, _currentapprover, Guid.Empty);

        }

        public void ApprovePersonalDiscount(IOrganizationService _organizationservice, IWorkflowContext _context, string[] _recordids)
        {
            Guid _quoteid = new Guid(_recordids[0]);
            Entity _masterconditiontype = new Entity(_entityname_masterconditiontype);
            Entity _matrixapprovalconditiontype = new Entity(_entityname_matrixapprovalconditiontype);
            Entity _quote = _organizationservice.Retrieve(_entityname_quote, _quoteid, new ColumnSet(true));
            Entity _owner = _organizationservice.Retrieve(_entityname_systemuser, _quote.GetAttributeValue<EntityReference>("createdby").Id, new ColumnSet(true));
            Guid _unitgroupid = _quote.GetAttributeValue<EntityReference>("new_unitgroup").Id;

            QueryExpression _queryexpression = new QueryExpression(_entityname_masterconditiontype);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition("ittn_code", ConditionOperator.Equal, CODE_PERSONALDISCOUNT);
            EntityCollection _masterconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

            if (_masterconditiontypes.Entities.Count() == 0)
                throw new InvalidWorkflowException("There is no Code '" + CODE_PERSONALDISCOUNT + "' in Master Condition Type!");
            else
                _masterconditiontype = _masterconditiontypes.Entities.FirstOrDefault();

            Guid _currentapproverpersonaldiscountid = _quote.Attributes.Contains("ittn_currentapproverpersonaldiscount") ? _quote.GetAttributeValue<EntityReference>("ittn_currentapproverpersonaldiscount").Id : new Guid();

            if (_currentapproverpersonaldiscountid != new Guid())
            {
                Entity _currentapprover = _organizationservice.Retrieve(_entityname_matrixapprovalconditiontype, _currentapproverpersonaldiscountid, new ColumnSet(true));

                if (_currentapprover.GetAttributeValue<EntityReference>("ittn_approver").Id == _context.UserId)
                {
                    Entity _currentapproverpersonaldiscount = _organizationservice.Retrieve(_entityname_matrixapprovalconditiontype, _currentapproverpersonaldiscountid, new ColumnSet(true));

                    //if (_currentapproverpersonaldiscount.GetAttributeValue<OptionSetValue>("ittn_approveby").Value == APPROVEBY_SALESMANAGER)
                    //{
                    //    // GET MATRIX APPROVAL
                    //    _queryexpression = new QueryExpression(_entityname_matrixapprovalconditiontype);
                    //    _queryexpression.ColumnSet = new ColumnSet(true);
                    //    _queryexpression.Orders.Add(new OrderExpression("ittn_priorityno", OrderType.Ascending));
                    //    _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontype.Id);
                    //    _queryexpression.Criteria.AddCondition("ittn_unitgroup", ConditionOperator.Equal, _unitgroupid);
                    //    _queryexpression.Criteria.AddCondition("ittn_approveby", ConditionOperator.Equal, APPROVEBY_GENERALMANAGER);

                    //    EntityCollection _matrixapprovalconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                    //    if (_matrixapprovalconditiontypes.Entities.Count() == 0)
                    //        throw new InvalidWorkflowException("There is no Approver for code '" + CODE_PERSONALDISCOUNT + "' with level 'General Manager' in Master Condition Type!");
                    //    else
                    //        _matrixapprovalconditiontype = _matrixapprovalconditiontypes.Entities.FirstOrDefault();

                    //    // UPDATE QUOTE
                    //    Entity _quote_update = new Entity(_entityname_quote);
                    //    _quote_update.Id = _quoteid;

                    //    _quote_update["ittn_needapprovepersonaldiscount"] = true;
                    //    _quote_update["ittn_reqapprovepersonaldiscountdate"] = DateTime.Now.ToLocalTime();
                    //    _quote_update["ittn_currentapproverpersonaldiscount"] = new EntityReference(_entityname_matrixapprovalconditiontype, _matrixapprovalconditiontype.Id);
                    //    _quote_update["ittn_approvepersonaldiscountdate"] = null;
                    //    _quote_update["ittn_approvepersonaldiscountby"] = null;

                    //    _organizationservice.Update(_quote_update);
                    //}
                    //else
                    if (_currentapproverpersonaldiscount.GetAttributeValue<OptionSetValue>("ittn_approveby").Value == APPROVEBY_GENERALMANAGER)
                    {
                        // GET MATRIX APPROVAL
                        _queryexpression = new QueryExpression(_entityname_matrixapprovalconditiontype);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Orders.Add(new OrderExpression("ittn_priorityno", OrderType.Ascending));
                        _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontype.Id);
                        _queryexpression.Criteria.AddCondition("ittn_unitgroup", ConditionOperator.Equal, _unitgroupid);
                        _queryexpression.Criteria.AddCondition("ittn_approveby", ConditionOperator.Equal, APPROVEBY_DIRECTOR);

                        EntityCollection _matrixapprovalconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                        if (_matrixapprovalconditiontypes.Entities.Count() == 0)
                            throw new InvalidWorkflowException("There is no Approver for code '" + CODE_PERSONALDISCOUNT + "' with level 'DIRECTOR' in Master Condition Type!");
                        else
                            _matrixapprovalconditiontype = _matrixapprovalconditiontypes.Entities.FirstOrDefault();

                        // SHARE RECORDS
                        Entity _nextapprover = _organizationservice.Retrieve(_entityname_systemuser, _matrixapprovalconditiontype.GetAttributeValue<EntityReference>("ittn_approver").Id, new ColumnSet(true));
                        ShareRecords _sharerecords = new ShareRecords();
                        _sharerecords.ShareRecord(_organizationservice, _quote, _nextapprover);

                        // UPDATE QUOTE
                        Entity _quote_update = new Entity(_entityname_quote);
                        _quote_update.Id = _quoteid;

                        _quote_update["ittn_needapprovepersonaldiscount"] = true;
                        _quote_update["ittn_reqapprovepersonaldiscountdate"] = DateTime.Now.ToLocalTime();
                        _quote_update["ittn_currentapproverpersonaldiscount"] = new EntityReference(_entityname_matrixapprovalconditiontype, _matrixapprovalconditiontype.Id);
                        _quote_update["ittn_approvepersonaldiscountdate"] = null;
                        _quote_update["ittn_approvepersonaldiscountby"] = null;

                        _organizationservice.Update(_quote_update);

                        // SEND EMAIL
                        SendMail(_organizationservice, _context, _masterconditiontype, _quote, _owner, _nextapprover, Guid.Empty);
                    }
                    else if (_currentapproverpersonaldiscount.GetAttributeValue<OptionSetValue>("ittn_approveby").Value == APPROVEBY_DIRECTOR)
                    {
                        // UPDATE QUOTE
                        Entity _quote_update = new Entity(_entityname_quote);
                        _quote_update.Id = _quoteid;

                        _quote_update["ittn_needapprovepersonaldiscount"] = false;
                        _quote_update["ittn_currentapproverpersonaldiscount"] = null;
                        _quote_update["ittn_approvepersonaldiscountdate"] = DateTime.Now.ToLocalTime();
                        _quote_update["ittn_approvepersonaldiscountby"] = new EntityReference(_entityname_systemuser, _context.UserId);

                        _organizationservice.Update(_quote_update);

                        // UPDATE QUOTE PRODUCT
                        _queryexpression = new QueryExpression(_entityname_quoteconditiontype);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria.AddCondition("ittn_quote", ConditionOperator.Equal, _quoteid);
                        _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontype.Id);
                        _queryexpression.Criteria.AddCondition("ittn_statusreason", ConditionOperator.Equal, STATUS_INPROGRESS);
                        EntityCollection _quoteconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                        foreach (var _quoteconditiontype in _quoteconditiontypes.Entities)
                        {
                            Entity _quoteconditiontype_forupdate = new Entity(_entityname_quoteconditiontype);
                            _quoteconditiontype_forupdate.Id = _quoteconditiontype.Id;

                            _quoteconditiontype_forupdate["ittn_statusreason"] = new OptionSetValue(STATUS_ACTIVE);

                            _organizationservice.Update(_quoteconditiontype_forupdate);
                        }

                    }

                }
                else
                {
                    throw new InvalidWorkflowException("You DO NOT have access to approve this Quote - Personal Discount !");
                }

            }

        }

        public void RejectPersonalDiscount(IOrganizationService _organizationservice, IWorkflowContext _context, string[] _recordids)
        {
            Guid _quoteid = new Guid(_recordids[0]);
            Entity _masterconditiontype = new Entity(_entityname_masterconditiontype);
            Entity _matrixapprovalconditiontype = new Entity(_entityname_matrixapprovalconditiontype);
            Entity _quote = _organizationservice.Retrieve(_entityname_quote, _quoteid, new ColumnSet(true));
            Guid _unitgroupid = _quote.GetAttributeValue<EntityReference>("new_unitgroup").Id;

            QueryExpression _queryexpression = new QueryExpression(_entityname_masterconditiontype);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition("ittn_code", ConditionOperator.Equal, CODE_PERSONALDISCOUNT);
            EntityCollection _masterconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

            if (_masterconditiontypes.Entities.Count() == 0)
                throw new InvalidWorkflowException("There is no Code '" + CODE_PERSONALDISCOUNT + "' in Master Condition Type!");
            else
                _masterconditiontype = _masterconditiontypes.Entities.FirstOrDefault();

            Guid _currentapproverpersonaldiscountid = _quote.Attributes.Contains("ittn_currentapproverpersonaldiscount") ? _quote.GetAttributeValue<EntityReference>("ittn_currentapproverpersonaldiscount").Id : new Guid();

            if (_currentapproverpersonaldiscountid != new Guid())
            {
                Entity _currentapprover = _organizationservice.Retrieve(_entityname_matrixapprovalconditiontype, _currentapproverpersonaldiscountid, new ColumnSet(true));

                if (_currentapprover.GetAttributeValue<EntityReference>("ittn_approver").Id == _context.UserId)
                {
                    Entity _currentapproverpersonaldiscount = _organizationservice.Retrieve(_entityname_matrixapprovalconditiontype, _currentapproverpersonaldiscountid, new ColumnSet(true));

                    // UPDATE QUOTE
                    Entity _quote_update = new Entity(_entityname_quote);
                    _quote_update.Id = _quoteid;

                    _quote_update["ittn_needapprovepersonaldiscount"] = false;
                    _quote_update["ittn_reqapprovepersonaldiscountdate"] = null;
                    _quote_update["ittn_currentapproverpersonaldiscount"] = null;
                    _quote_update["ittn_approvepersonaldiscountdate"] = DateTime.Now.ToLocalTime();
                    _quote_update["ittn_approvepersonaldiscountby"] = new EntityReference(_entityname_systemuser, _context.UserId);

                    _organizationservice.Update(_quote_update);

                    // UPDATE QUOTE PRODUCT
                    _queryexpression = new QueryExpression(_entityname_quoteconditiontype);
                    _queryexpression.ColumnSet = new ColumnSet(true);
                    _queryexpression.Criteria.AddCondition("ittn_quote", ConditionOperator.Equal, _quoteid);
                    _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontype.Id);
                    _queryexpression.Criteria.AddCondition("ittn_statusreason", ConditionOperator.Equal, STATUS_INPROGRESS);
                    EntityCollection _quoteconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                    foreach (var _quoteconditiontype in _quoteconditiontypes.Entities)
                    {
                        Entity _quoteconditiontype_forupdate = new Entity(_entityname_quoteconditiontype);
                        _quoteconditiontype_forupdate.Id = _quoteconditiontype.Id;

                        _quoteconditiontype_forupdate["ittn_statusreason"] = new OptionSetValue(STATUS_INACTIVE);

                        _organizationservice.Update(_quoteconditiontype_forupdate);
                    }

                }
                else
                {
                    throw new InvalidWorkflowException("You DO NOT have access to approve this Quote - Personal Discount !");
                }

            }

        }

        #endregion
        // ---------------------------------------- ---------------------------------------- ---------------------------------------- ----------------------------------------



        // ---------------------------------------- ---------------------------------------- ---------------------------------------- ----------------------------------------
        #region FAT

        public void AddFAT(IOrganizationService _organizationservice, IWorkflowContext _context, string[] _recordids)
        {
            Guid _quoteid = new Guid(_recordids[0]);
            Guid _masterconditiontypeid = new Guid();

            Entity _quote = _organizationservice.Retrieve(_entityname_quote, _quoteid, new ColumnSet(true));

            QueryExpression _queryexpression = new QueryExpression(_entityname_masterconditiontype);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition("ittn_code", ConditionOperator.Equal, "ZFAT");
            EntityCollection _masterconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

            if (_masterconditiontypes.Entities.Count() == 0)
                throw new InvalidWorkflowException("There is no Code 'ZFAT' in Master Condition Type!");
            else
                _masterconditiontypeid = _masterconditiontypes.Entities.FirstOrDefault().Id;

            _queryexpression = new QueryExpression(_entityname_quotedetail);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition("quoteid", ConditionOperator.Equal, _quoteid);
            _queryexpression.Criteria.AddCondition("new_parentnumber", ConditionOperator.Null);
            EntityCollection _quotedetails = _organizationservice.RetrieveMultiple(_queryexpression);

            foreach (var _quotedetail in _quotedetails.Entities)
            {
                decimal _amount = 0;
                decimal _totalamount = 0;
                decimal _itemnumber = _quotedetail.GetAttributeValue<decimal>("new_itemnumber");
                decimal _quantity = _quotedetail.GetAttributeValue<decimal>("new_quantity");

                if (_quote.Attributes.Contains("ittn_fatamount"))
                {
                    _amount = _quote.GetAttributeValue<Money>("ittn_fatamount").Value;
                    _totalamount = _amount;
                }

                if (_quantity > 0)
                    _amount = _amount / _quantity;

                //_queryexpression = new QueryExpression(_entityname_quoteconditiontype);
                //_queryexpression.ColumnSet = new ColumnSet(false);
                //_queryexpression.Criteria.AddCondition("ittn_quote", ConditionOperator.Equal, _quoteid);
                //_queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontypeid);

                //EntityCollection _quoteconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                //if (_quoteconditiontypes.Entities.Count() > 0)
                //    throw new InvalidWorkflowException("Code 'ZFAT' ( FAT Cost ) already exist in this Quotation Condition Type!");
                //else
                //{
                Entity _quoteconditiontype = new Entity(_entityname_quoteconditiontype);

                _quoteconditiontype["ittn_amount"] = new Money(_amount);
                _quoteconditiontype["ittn_totalamount"] = new Money(_totalamount);
                _quoteconditiontype["ittn_conditiontype"] = new EntityReference(_entityname_masterconditiontype, _masterconditiontypeid);
                _quoteconditiontype["ittn_name"] = _masterconditiontypes.Entities.FirstOrDefault().GetAttributeValue<string>("ittn_description");
                _quoteconditiontype["ittn_quote"] = new EntityReference(_entityname_quote, _quoteid);
                _quoteconditiontype["ittn_itemnumber"] = _itemnumber;
                _quoteconditiontype["ittn_needapproveconditiontype"] = _masterconditiontypes.Entities.FirstOrDefault().GetAttributeValue<bool>("ittn_needapproval");
                _quoteconditiontype["transactioncurrencyid"] = new EntityReference(_entityname_transactioncurrency, Globalization.GetTransactionCurrencyID(_organizationservice, _entityname_transactioncurrency_code));

                _organizationservice.Create(_quoteconditiontype);
                //}
            }

        }

        public void RequestApprovalForFAT(IOrganizationService _organizationservice, IWorkflowContext _context, string[] _recordids)
        {
            Guid _quoteid = new Guid(_recordids[0]);
            Entity _masterconditiontype = new Entity(_entityname_masterconditiontype);
            Entity _matrixapprovalconditiontype = new Entity(_entityname_matrixapprovalconditiontype);
            Entity _quote = _organizationservice.Retrieve(_entityname_quote, _quoteid, new ColumnSet(true));
            Entity _owner = _organizationservice.Retrieve(_entityname_systemuser, _quote.GetAttributeValue<EntityReference>("createdby").Id, new ColumnSet(true));
            Guid _unitgroupid = _quote.GetAttributeValue<EntityReference>("new_unitgroup").Id;

            QueryExpression _queryexpression = new QueryExpression(_entityname_masterconditiontype);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition("ittn_code", ConditionOperator.Equal, CODE_FAT);
            EntityCollection _masterconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

            if (_masterconditiontypes.Entities.Count() == 0)
                throw new InvalidWorkflowException("There is no Code '" + CODE_FAT + "' in Master Condition Type!");
            else
                _masterconditiontype = _masterconditiontypes.Entities.FirstOrDefault();

            // GET MATRIX APPROVAL
            _queryexpression = new QueryExpression(_entityname_matrixapprovalconditiontype);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Orders.Add(new OrderExpression("ittn_priorityno", OrderType.Ascending));
            _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontype.Id);
            _queryexpression.Criteria.AddCondition("ittn_unitgroup", ConditionOperator.Equal, _unitgroupid);
            _queryexpression.Criteria.AddCondition("ittn_approveby", ConditionOperator.Equal, APPROVEBY_GENERALMANAGER);
            EntityCollection _matrixapprovalconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

            if (_matrixapprovalconditiontypes.Entities.Count() == 0)
                throw new InvalidWorkflowException("There is no Approver for code '" + CODE_FAT + "' with level 'GENERAL MANAGER' in Master Condition Type!");
            else
                _matrixapprovalconditiontype = _matrixapprovalconditiontypes.Entities.FirstOrDefault();

            // SHARE RECORDS
            Entity _currentapprover = _organizationservice.Retrieve(_entityname_systemuser, _matrixapprovalconditiontype.GetAttributeValue<EntityReference>("ittn_approver").Id, new ColumnSet(true));
            ShareRecords _sharerecords = new ShareRecords();
            _sharerecords.ShareRecord(_organizationservice, _quote, _currentapprover);

            // UPDATE QUOTE
            Entity _quote_update = new Entity(_entityname_quote);
            _quote_update.Id = _quoteid;

            _quote_update["ittn_needapprovefat"] = true;
            _quote_update["ittn_reqapprovefatdate"] = DateTime.Now.ToLocalTime();
            _quote_update["ittn_currentapproverfat"] = new EntityReference(_entityname_matrixapprovalconditiontype, _matrixapprovalconditiontype.Id);

            _organizationservice.Update(_quote_update);

            // UPDATE QUOTE PRODUCT
            _queryexpression = new QueryExpression(_entityname_quoteconditiontype);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition("ittn_quote", ConditionOperator.Equal, _quoteid);
            _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontype.Id);
            _queryexpression.Criteria.AddCondition("ittn_statusreason", ConditionOperator.Equal, STATUS_NOTASSIGN);
            EntityCollection _quoteconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

            foreach (var _quoteconditiontype in _quoteconditiontypes.Entities)
            {
                if (_quoteconditiontype.GetAttributeValue<Money>("ittn_amount").Value > 0)
                {
                    Entity _quoteconditiontype_forupdate = new Entity(_entityname_quoteconditiontype);
                    _quoteconditiontype_forupdate.Id = _quoteconditiontype.Id;

                    _quoteconditiontype_forupdate["ittn_statusreason"] = new OptionSetValue(STATUS_INPROGRESS);

                    _organizationservice.Update(_quoteconditiontype_forupdate);
                }
                else
                {
                    throw new InvalidWorkflowException("Amount FAT must be greater than 0 !");
                }
            }

            // SEND EMAIL
            SendMail(_organizationservice, _context, _masterconditiontype, _quote, _owner, _currentapprover, Guid.Empty);

        }

        public void ApproveFAT(IOrganizationService _organizationservice, IWorkflowContext _context, string[] _recordids)
        {
            Guid _quoteid = new Guid(_recordids[0]);
            Entity _masterconditiontype = new Entity(_entityname_masterconditiontype);
            Entity _matrixapprovalconditiontype = new Entity(_entityname_matrixapprovalconditiontype);
            Entity _quote = _organizationservice.Retrieve(_entityname_quote, _quoteid, new ColumnSet(true));
            Entity _owner = _organizationservice.Retrieve(_entityname_systemuser, _quote.GetAttributeValue<EntityReference>("createdby").Id, new ColumnSet(true));
            Guid _unitgroupid = _quote.GetAttributeValue<EntityReference>("new_unitgroup").Id;

            QueryExpression _queryexpression = new QueryExpression(_entityname_masterconditiontype);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition("ittn_code", ConditionOperator.Equal, CODE_FAT);
            EntityCollection _masterconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

            if (_masterconditiontypes.Entities.Count() == 0)
                throw new InvalidWorkflowException("There is no Code '" + CODE_FAT + "' in Master Condition Type!");
            else
                _masterconditiontype = _masterconditiontypes.Entities.FirstOrDefault();

            Guid _currentapproverfatid = _quote.Attributes.Contains("ittn_currentapproverfat") ? _quote.GetAttributeValue<EntityReference>("ittn_currentapproverfat").Id : new Guid();

            if (_currentapproverfatid != new Guid())
            {
                Entity _currentapprover = _organizationservice.Retrieve(_entityname_matrixapprovalconditiontype, _currentapproverfatid, new ColumnSet(true));

                if (_currentapprover.GetAttributeValue<EntityReference>("ittn_approver").Id == _context.UserId)
                {
                    Entity _currentapproverfat = _organizationservice.Retrieve(_entityname_matrixapprovalconditiontype, _currentapproverfatid, new ColumnSet(true));

                    //if (_currentapproverfat.GetAttributeValue<OptionSetValue>("ittn_approveby").Value == APPROVEBY_SALESMANAGER)
                    //{
                    //    // GET MATRIX APPROVAL
                    //    _queryexpression = new QueryExpression(_entityname_matrixapprovalconditiontype);
                    //    _queryexpression.ColumnSet = new ColumnSet(true);
                    //    _queryexpression.Orders.Add(new OrderExpression("ittn_priorityno", OrderType.Ascending));
                    //    _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontype.Id);
                    //    _queryexpression.Criteria.AddCondition("ittn_unitgroup", ConditionOperator.Equal, _unitgroupid);
                    //    _queryexpression.Criteria.AddCondition("ittn_approveby", ConditionOperator.Equal, APPROVEBY_GENERALMANAGER);

                    //    EntityCollection _matrixapprovalconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                    //    if (_matrixapprovalconditiontypes.Entities.Count() == 0)
                    //        throw new InvalidWorkflowException("There is no Approver for code '" + CODE_FAT + "' with level 'General Manager' in Master Condition Type!");
                    //    else
                    //        _matrixapprovalconditiontype = _matrixapprovalconditiontypes.Entities.FirstOrDefault();

                    //    // UPDATE QUOTE
                    //    Entity _quote_update = new Entity(_entityname_quote);
                    //    _quote_update.Id = _quoteid;

                    //    _quote_update["ittn_needapprovefat"] = true;
                    //    _quote_update["ittn_reqapprovefatdate"] = DateTime.Now.ToLocalTime();
                    //    _quote_update["ittn_currentapproverfat"] = new EntityReference(_entityname_matrixapprovalconditiontype, _matrixapprovalconditiontype.Id);
                    //    _quote_update["ittn_approvefatdate"] = null;
                    //    _quote_update["ittn_approvefatby"] = null;

                    //    _organizationservice.Update(_quote_update);
                    //}
                    //else
                    if (_currentapproverfat.GetAttributeValue<OptionSetValue>("ittn_approveby").Value == APPROVEBY_GENERALMANAGER)
                    {
                        // GET MATRIX APPROVAL
                        _queryexpression = new QueryExpression(_entityname_matrixapprovalconditiontype);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Orders.Add(new OrderExpression("ittn_priorityno", OrderType.Ascending));
                        _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontype.Id);
                        _queryexpression.Criteria.AddCondition("ittn_unitgroup", ConditionOperator.Equal, _unitgroupid);
                        _queryexpression.Criteria.AddCondition("ittn_approveby", ConditionOperator.Equal, APPROVEBY_DIRECTOR);

                        EntityCollection _matrixapprovalconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                        if (_matrixapprovalconditiontypes.Entities.Count() == 0)
                            throw new InvalidWorkflowException("There is no Approver for code '" + CODE_FAT + "' with level 'DIRECTOR' in Master Condition Type!");
                        else
                            _matrixapprovalconditiontype = _matrixapprovalconditiontypes.Entities.FirstOrDefault();

                        // SHARE RECORDS
                        Entity _nextapprover = _organizationservice.Retrieve(_entityname_systemuser, _matrixapprovalconditiontype.GetAttributeValue<EntityReference>("ittn_approver").Id, new ColumnSet(true));
                        ShareRecords _sharerecords = new ShareRecords();
                        _sharerecords.ShareRecord(_organizationservice, _quote, _nextapprover);

                        // UPDATE QUOTE
                        Entity _quote_update = new Entity(_entityname_quote);
                        _quote_update.Id = _quoteid;

                        _quote_update["ittn_needapprovefat"] = true;
                        _quote_update["ittn_reqapprovefatdate"] = DateTime.Now.ToLocalTime();
                        _quote_update["ittn_currentapproverfat"] = new EntityReference(_entityname_matrixapprovalconditiontype, _matrixapprovalconditiontype.Id);
                        _quote_update["ittn_approvefatdate"] = null;
                        _quote_update["ittn_approvefatby"] = null;

                        _organizationservice.Update(_quote_update);

                        // SEND EMAIL
                        SendMail(_organizationservice, _context, _masterconditiontype, _quote, _owner, _nextapprover, Guid.Empty);
                    }
                    else if (_currentapproverfat.GetAttributeValue<OptionSetValue>("ittn_approveby").Value == APPROVEBY_DIRECTOR)
                    {
                        // UPDATE QUOTE
                        Entity _quote_update = new Entity(_entityname_quote);
                        _quote_update.Id = _quoteid;

                        _quote_update["ittn_needapprovefat"] = false;
                        _quote_update["ittn_currentapproverfat"] = null;
                        _quote_update["ittn_approvefatdate"] = DateTime.Now.ToLocalTime();
                        _quote_update["ittn_approvefatby"] = new EntityReference(_entityname_systemuser, _context.UserId);

                        _organizationservice.Update(_quote_update);

                        // UPDATE QUOTE PRODUCT
                        _queryexpression = new QueryExpression(_entityname_quoteconditiontype);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria.AddCondition("ittn_quote", ConditionOperator.Equal, _quoteid);
                        _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontype.Id);
                        _queryexpression.Criteria.AddCondition("ittn_statusreason", ConditionOperator.Equal, STATUS_INPROGRESS);
                        EntityCollection _quoteconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                        foreach (var _quoteconditiontype in _quoteconditiontypes.Entities)
                        {
                            Entity _quoteconditiontype_forupdate = new Entity(_entityname_quoteconditiontype);
                            _quoteconditiontype_forupdate.Id = _quoteconditiontype.Id;

                            _quoteconditiontype_forupdate["ittn_statusreason"] = new OptionSetValue(STATUS_ACTIVE);

                            _organizationservice.Update(_quoteconditiontype_forupdate);
                        }

                    }
                }
                else
                {
                    throw new InvalidWorkflowException("You DO NOT have access to approve this Quote - FAT !");
                }

            }

        }

        public void RejectFAT(IOrganizationService _organizationservice, IWorkflowContext _context, string[] _recordids)
        {
            Guid _quoteid = new Guid(_recordids[0]);
            Entity _masterconditiontype = new Entity(_entityname_masterconditiontype);
            Entity _matrixapprovalconditiontype = new Entity(_entityname_matrixapprovalconditiontype);
            Entity _quote = _organizationservice.Retrieve(_entityname_quote, _quoteid, new ColumnSet(true));
            Guid _unitgroupid = _quote.GetAttributeValue<EntityReference>("new_unitgroup").Id;

            QueryExpression _queryexpression = new QueryExpression(_entityname_masterconditiontype);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition("ittn_code", ConditionOperator.Equal, CODE_FAT);
            EntityCollection _masterconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

            if (_masterconditiontypes.Entities.Count() == 0)
                throw new InvalidWorkflowException("There is no Code '" + CODE_FAT + "' in Master Condition Type!");
            else
                _masterconditiontype = _masterconditiontypes.Entities.FirstOrDefault();

            Guid _currentapproverfatid = _quote.Attributes.Contains("ittn_currentapproverfat") ? _quote.GetAttributeValue<EntityReference>("ittn_currentapproverfat").Id : new Guid();

            if (_currentapproverfatid != new Guid())
            {
                Entity _currentapprover = _organizationservice.Retrieve(_entityname_matrixapprovalconditiontype, _currentapproverfatid, new ColumnSet(true));

                if (_currentapprover.GetAttributeValue<EntityReference>("ittn_approver").Id == _context.UserId)
                {
                    Entity _currentapproverfat = _organizationservice.Retrieve(_entityname_matrixapprovalconditiontype, _currentapproverfatid, new ColumnSet(true));

                    // UPDATE QUOTE
                    Entity _quote_update = new Entity(_entityname_quote);
                    _quote_update.Id = _quoteid;

                    _quote_update["ittn_needapprovefat"] = false;
                    _quote_update["ittn_reqapprovefatdate"] = null;
                    _quote_update["ittn_currentapproverfat"] = null;
                    _quote_update["ittn_approvefatdate"] = DateTime.Now.ToLocalTime();
                    _quote_update["ittn_approvefatby"] = new EntityReference(_entityname_systemuser, _context.UserId);

                    _organizationservice.Update(_quote_update);

                    // UPDATE QUOTE PRODUCT
                    _queryexpression = new QueryExpression(_entityname_quoteconditiontype);
                    _queryexpression.ColumnSet = new ColumnSet(true);
                    _queryexpression.Criteria.AddCondition("ittn_quote", ConditionOperator.Equal, _quoteid);
                    _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontype.Id);
                    _queryexpression.Criteria.AddCondition("ittn_statusreason", ConditionOperator.Equal, STATUS_INPROGRESS);
                    EntityCollection _quoteconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                    foreach (var _quoteconditiontype in _quoteconditiontypes.Entities)
                    {
                        Entity _quoteconditiontype_forupdate = new Entity(_entityname_quoteconditiontype);
                        _quoteconditiontype_forupdate.Id = _quoteconditiontype.Id;

                        _quoteconditiontype_forupdate["ittn_statusreason"] = new OptionSetValue(STATUS_INACTIVE);

                        _organizationservice.Update(_quoteconditiontype_forupdate);
                    }
                }
                else
                {
                    throw new InvalidWorkflowException("You DO NOT have access to approve this Quote - FAT !");
                }

            }

        }

        #endregion
        // ---------------------------------------- ---------------------------------------- ---------------------------------------- ----------------------------------------



        // ---------------------------------------- ---------------------------------------- ---------------------------------------- ----------------------------------------
        #region VOUCHER

        public void AddVoucher(IOrganizationService _organizationservice, IWorkflowContext _context, string[] _recordids)
        {
            Guid _quoteid = new Guid(_recordids[0]);
            Guid _masterconditiontypeid = new Guid();

            Entity _quote = _organizationservice.Retrieve(_entityname_quote, _quoteid, new ColumnSet(true));

            QueryExpression _queryexpression = new QueryExpression(_entityname_masterconditiontype);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition("ittn_code", ConditionOperator.Equal, "ZVCR");
            EntityCollection _masterconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

            if (_masterconditiontypes.Entities.Count() == 0)
                throw new InvalidWorkflowException("There is no Code 'ZVCR' in Master Condition Type!");
            else
                _masterconditiontypeid = _masterconditiontypes.Entities.FirstOrDefault().Id;

            _queryexpression = new QueryExpression(_entityname_quotedetail);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition("quoteid", ConditionOperator.Equal, _quoteid);
            _queryexpression.Criteria.AddCondition("new_parentnumber", ConditionOperator.Null);
            EntityCollection _quotedetails = _organizationservice.RetrieveMultiple(_queryexpression);

            foreach (var _quotedetail in _quotedetails.Entities)
            {
                decimal _amount = 0;
                decimal _totalamount = 0;
                decimal _itemnumber = _quotedetail.GetAttributeValue<decimal>("new_itemnumber");
                decimal _quantity = _quotedetail.GetAttributeValue<decimal>("new_quantity");

                if (_quote.Attributes.Contains("ittn_voucheramount"))
                {
                    _amount = _quote.GetAttributeValue<Money>("ittn_voucheramount").Value;
                    _totalamount = _amount;
                }

                if (_quantity > 0)
                    _amount = _amount / _quantity;

                //_queryexpression = new QueryExpression(_entityname_quoteconditiontype);
                //_queryexpression.ColumnSet = new ColumnSet(false);
                //_queryexpression.Criteria.AddCondition("ittn_quote", ConditionOperator.Equal, _quoteid);
                //_queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontypeid);

                //EntityCollection _quoteconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                //if (_quoteconditiontypes.Entities.Count() > 0)
                //    throw new InvalidWorkflowException("Code 'ZVCR' ( Voucher ) already exist in this Quotation Condition Type!");
                //else
                //{
                Entity _quoteconditiontype = new Entity(_entityname_quoteconditiontype);

                _quoteconditiontype["ittn_amount"] = new Money(_amount);
                _quoteconditiontype["ittn_totalamount"] = new Money(_totalamount);
                _quoteconditiontype["ittn_conditiontype"] = new EntityReference(_entityname_masterconditiontype, _masterconditiontypeid);
                _quoteconditiontype["ittn_name"] = _masterconditiontypes.Entities.FirstOrDefault().GetAttributeValue<string>("ittn_description");
                _quoteconditiontype["ittn_quote"] = new EntityReference(_entityname_quote, _quoteid);
                _quoteconditiontype["ittn_itemnumber"] = _itemnumber;
                _quoteconditiontype["ittn_needapproveconditiontype"] = _masterconditiontypes.Entities.FirstOrDefault().GetAttributeValue<bool>("ittn_needapproval");
                _quoteconditiontype["transactioncurrencyid"] = new EntityReference(_entityname_transactioncurrency, Globalization.GetTransactionCurrencyID(_organizationservice, _entityname_transactioncurrency_code));

                _organizationservice.Create(_quoteconditiontype);
                //}
            }

        }

        public void RequestApprovalForVoucher(IOrganizationService _organizationservice, IWorkflowContext _context, string[] _recordids)
        {
            Guid _quoteid = new Guid(_recordids[0]);
            Entity _masterconditiontype = new Entity(_entityname_masterconditiontype);
            Entity _matrixapprovalconditiontype = new Entity(_entityname_matrixapprovalconditiontype);
            Entity _quote = _organizationservice.Retrieve(_entityname_quote, _quoteid, new ColumnSet(true));
            Entity _owner = _organizationservice.Retrieve(_entityname_systemuser, _quote.GetAttributeValue<EntityReference>("createdby").Id, new ColumnSet(true));
            Guid _unitgroupid = _quote.GetAttributeValue<EntityReference>("new_unitgroup").Id;

            QueryExpression _queryexpression = new QueryExpression(_entityname_masterconditiontype);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition("ittn_code", ConditionOperator.Equal, CODE_VOUCHER);
            EntityCollection _masterconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

            if (_masterconditiontypes.Entities.Count() == 0)
                throw new InvalidWorkflowException("There is no Code '" + CODE_VOUCHER + "' in Master Condition Type!");
            else
                _masterconditiontype = _masterconditiontypes.Entities.FirstOrDefault();

            // GET MATRIX APPROVAL
            _queryexpression = new QueryExpression(_entityname_matrixapprovalconditiontype);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Orders.Add(new OrderExpression("ittn_priorityno", OrderType.Ascending));
            _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontype.Id);
            _queryexpression.Criteria.AddCondition("ittn_unitgroup", ConditionOperator.Equal, _unitgroupid);
            _queryexpression.Criteria.AddCondition("ittn_approveby", ConditionOperator.Equal, APPROVEBY_GENERALMANAGER);
            EntityCollection _matrixapprovalconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

            if (_matrixapprovalconditiontypes.Entities.Count() == 0)
                throw new InvalidWorkflowException("There is no Approver for code '" + CODE_VOUCHER + "' with level 'GENERAL MANAGER' in Master Condition Type!");
            else
                _matrixapprovalconditiontype = _matrixapprovalconditiontypes.Entities.FirstOrDefault();

            // SHARE RECORDS
            Entity _currentapprover = _organizationservice.Retrieve(_entityname_systemuser, _matrixapprovalconditiontype.GetAttributeValue<EntityReference>("ittn_approver").Id, new ColumnSet(true));
            ShareRecords _sharerecords = new ShareRecords();
            _sharerecords.ShareRecord(_organizationservice, _quote, _currentapprover);

            // UPDATE QUOTE
            Entity _quote_update = new Entity(_entityname_quote);
            _quote_update.Id = _quoteid;

            _quote_update["ittn_needapprovevoucher"] = true;
            _quote_update["ittn_reqapprovevoucherdate"] = DateTime.Now.ToLocalTime();
            _quote_update["ittn_currentapprovervoucher"] = new EntityReference(_entityname_matrixapprovalconditiontype, _matrixapprovalconditiontype.Id);

            _organizationservice.Update(_quote_update);

            // UPDATE QUOTE PRODUCT
            _queryexpression = new QueryExpression(_entityname_quoteconditiontype);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition("ittn_quote", ConditionOperator.Equal, _quoteid);
            _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontype.Id);
            _queryexpression.Criteria.AddCondition("ittn_statusreason", ConditionOperator.Equal, STATUS_NOTASSIGN);
            EntityCollection _quoteconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

            foreach (var _quoteconditiontype in _quoteconditiontypes.Entities)
            {
                if (_quoteconditiontype.GetAttributeValue<Money>("ittn_amount").Value > 0)
                {
                    Entity _quoteconditiontype_forupdate = new Entity(_entityname_quoteconditiontype);
                    _quoteconditiontype_forupdate.Id = _quoteconditiontype.Id;

                    _quoteconditiontype_forupdate["ittn_statusreason"] = new OptionSetValue(STATUS_INPROGRESS);

                    _organizationservice.Update(_quoteconditiontype_forupdate);
                }
                else
                {
                    throw new InvalidWorkflowException("Amount VOUCHER must be greater than 0 !");
                }
            }

            // SEND EMAIL
            SendMail(_organizationservice, _context, _masterconditiontype, _quote, _owner, _currentapprover, Guid.Empty);

        }

        public void ApproveVoucher(IOrganizationService _organizationservice, IWorkflowContext _context, string[] _recordids)
        {
            Guid _quoteid = new Guid(_recordids[0]);
            Entity _masterconditiontype = new Entity(_entityname_masterconditiontype);
            Entity _matrixapprovalconditiontype = new Entity(_entityname_matrixapprovalconditiontype);
            Entity _quote = _organizationservice.Retrieve(_entityname_quote, _quoteid, new ColumnSet(true));
            Entity _owner = _organizationservice.Retrieve(_entityname_systemuser, _quote.GetAttributeValue<EntityReference>("createdby").Id, new ColumnSet(true));
            Guid _unitgroupid = _quote.GetAttributeValue<EntityReference>("new_unitgroup").Id;

            QueryExpression _queryexpression = new QueryExpression(_entityname_masterconditiontype);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition("ittn_code", ConditionOperator.Equal, CODE_VOUCHER);
            EntityCollection _masterconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

            if (_masterconditiontypes.Entities.Count() == 0)
                throw new InvalidWorkflowException("There is no Code '" + CODE_VOUCHER + "' in Master Condition Type!");
            else
                _masterconditiontype = _masterconditiontypes.Entities.FirstOrDefault();

            Guid _currentapprovervoucherid = _quote.Attributes.Contains("ittn_currentapprovervoucher") ? _quote.GetAttributeValue<EntityReference>("ittn_currentapprovervoucher").Id : new Guid();

            if (_currentapprovervoucherid != new Guid())
            {
                Entity _currentapprover = _organizationservice.Retrieve(_entityname_matrixapprovalconditiontype, _currentapprovervoucherid, new ColumnSet(true));

                if (_currentapprover.GetAttributeValue<EntityReference>("ittn_approver").Id == _context.UserId)
                {
                    Entity _currentapprovervoucher = _organizationservice.Retrieve(_entityname_matrixapprovalconditiontype, _currentapprovervoucherid, new ColumnSet(true));

                    //if (_currentapprovervoucher.GetAttributeValue<OptionSetValue>("ittn_approveby").Value == APPROVEBY_SALESMANAGER)
                    //{
                    //    // GET MATRIX APPROVAL
                    //    _queryexpression = new QueryExpression(_entityname_matrixapprovalconditiontype);
                    //    _queryexpression.ColumnSet = new ColumnSet(true);
                    //    _queryexpression.Orders.Add(new OrderExpression("ittn_priorityno", OrderType.Ascending));
                    //    _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontype.Id);
                    //    _queryexpression.Criteria.AddCondition("ittn_unitgroup", ConditionOperator.Equal, _unitgroupid);
                    //    _queryexpression.Criteria.AddCondition("ittn_approveby", ConditionOperator.Equal, APPROVEBY_GENERALMANAGER);

                    //    EntityCollection _matrixapprovalconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                    //    if (_matrixapprovalconditiontypes.Entities.Count() == 0)
                    //        throw new InvalidWorkflowException("There is no Approver for code '" + CODE_VOUCHER + "' with level 'General Manager' in Master Condition Type!");
                    //    else
                    //        _matrixapprovalconditiontype = _matrixapprovalconditiontypes.Entities.FirstOrDefault();

                    //    // UPDATE QUOTE
                    //    Entity _quote_update = new Entity(_entityname_quote);
                    //    _quote_update.Id = _quoteid;

                    //    _quote_update["ittn_needapprovevoucher"] = true;
                    //    _quote_update["ittn_reqapprovevoucherdate"] = DateTime.Now.ToLocalTime();
                    //    _quote_update["ittn_currentapprovervoucher"] = new EntityReference(_entityname_matrixapprovalconditiontype, _matrixapprovalconditiontype.Id);
                    //    _quote_update["ittn_approvevoucherdate"] = null;
                    //    _quote_update["ittn_approvevoucherby"] = null;

                    //    _organizationservice.Update(_quote_update);
                    //}
                    //else
                    if (_currentapprovervoucher.GetAttributeValue<OptionSetValue>("ittn_approveby").Value == APPROVEBY_GENERALMANAGER)
                    {
                        // GET MATRIX APPROVAL
                        _queryexpression = new QueryExpression(_entityname_matrixapprovalconditiontype);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Orders.Add(new OrderExpression("ittn_priorityno", OrderType.Ascending));
                        _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontype.Id);
                        _queryexpression.Criteria.AddCondition("ittn_unitgroup", ConditionOperator.Equal, _unitgroupid);
                        _queryexpression.Criteria.AddCondition("ittn_approveby", ConditionOperator.Equal, APPROVEBY_DIRECTOR);

                        EntityCollection _matrixapprovalconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                        if (_matrixapprovalconditiontypes.Entities.Count() == 0)
                            throw new InvalidWorkflowException("There is no Approver for code '" + CODE_VOUCHER + "' with level 'DIRECTOR' in Master Condition Type!");
                        else
                            _matrixapprovalconditiontype = _matrixapprovalconditiontypes.Entities.FirstOrDefault();

                        // SHARE RECORDS
                        Entity _nextapprover = _organizationservice.Retrieve(_entityname_systemuser, _matrixapprovalconditiontype.GetAttributeValue<EntityReference>("ittn_approver").Id, new ColumnSet(true));
                        ShareRecords _sharerecords = new ShareRecords();
                        _sharerecords.ShareRecord(_organizationservice, _quote, _nextapprover);

                        // UPDATE QUOTE
                        Entity _quote_update = new Entity(_entityname_quote);
                        _quote_update.Id = _quoteid;

                        _quote_update["ittn_needapprovevoucher"] = true;
                        _quote_update["ittn_reqapprovevoucherdate"] = DateTime.Now.ToLocalTime();
                        _quote_update["ittn_currentapprovervoucher"] = new EntityReference(_entityname_matrixapprovalconditiontype, _matrixapprovalconditiontype.Id);
                        _quote_update["ittn_approvevoucherdate"] = null;
                        _quote_update["ittn_approvevoucherby"] = null;

                        _organizationservice.Update(_quote_update);

                        // SEND EMAIL
                        SendMail(_organizationservice, _context, _masterconditiontype, _quote, _owner, _nextapprover, Guid.Empty);
                    }
                    else if (_currentapprovervoucher.GetAttributeValue<OptionSetValue>("ittn_approveby").Value == APPROVEBY_DIRECTOR)
                    {
                        // UPDATE QUOTE
                        Entity _quote_update = new Entity(_entityname_quote);
                        _quote_update.Id = _quoteid;

                        _quote_update["ittn_needapprovevoucher"] = false;
                        _quote_update["ittn_currentapprovervoucher"] = null;
                        _quote_update["ittn_approvevoucherdate"] = DateTime.Now.ToLocalTime();
                        _quote_update["ittn_approvevoucherby"] = new EntityReference(_entityname_systemuser, _context.UserId);

                        _organizationservice.Update(_quote_update);

                        // UPDATE QUOTE PRODUCT
                        _queryexpression = new QueryExpression(_entityname_quoteconditiontype);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria.AddCondition("ittn_quote", ConditionOperator.Equal, _quoteid);
                        _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontype.Id);
                        _queryexpression.Criteria.AddCondition("ittn_statusreason", ConditionOperator.Equal, STATUS_INPROGRESS);
                        EntityCollection _quoteconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                        foreach (var _quoteconditiontype in _quoteconditiontypes.Entities)
                        {
                            Entity _quoteconditiontype_forupdate = new Entity(_entityname_quoteconditiontype);
                            _quoteconditiontype_forupdate.Id = _quoteconditiontype.Id;

                            _quoteconditiontype_forupdate["ittn_statusreason"] = new OptionSetValue(STATUS_ACTIVE);

                            _organizationservice.Update(_quoteconditiontype_forupdate);
                        }

                    }
                }
                else
                {
                    throw new InvalidWorkflowException("You DO NOT have access to approve this Quote - Voucher !");
                }

            }

        }

        public void RejectVoucher(IOrganizationService _organizationservice, IWorkflowContext _context, string[] _recordids)
        {
            Guid _quoteid = new Guid(_recordids[0]);
            Entity _masterconditiontype = new Entity(_entityname_masterconditiontype);
            Entity _matrixapprovalconditiontype = new Entity(_entityname_matrixapprovalconditiontype);
            Entity _quote = _organizationservice.Retrieve(_entityname_quote, _quoteid, new ColumnSet(true));
            Guid _unitgroupid = _quote.GetAttributeValue<EntityReference>("new_unitgroup").Id;

            QueryExpression _queryexpression = new QueryExpression(_entityname_masterconditiontype);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition("ittn_code", ConditionOperator.Equal, CODE_VOUCHER);
            EntityCollection _masterconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

            if (_masterconditiontypes.Entities.Count() == 0)
                throw new InvalidWorkflowException("There is no Code '" + CODE_VOUCHER + "' in Master Condition Type!");
            else
                _masterconditiontype = _masterconditiontypes.Entities.FirstOrDefault();

            Guid _currentapprovervoucherid = _quote.Attributes.Contains("ittn_currentapprovervoucher") ? _quote.GetAttributeValue<EntityReference>("ittn_currentapprovervoucher").Id : new Guid();

            if (_currentapprovervoucherid != new Guid())
            {
                Entity _currentapprover = _organizationservice.Retrieve(_entityname_matrixapprovalconditiontype, _currentapprovervoucherid, new ColumnSet(true));

                if (_currentapprover.GetAttributeValue<EntityReference>("ittn_approver").Id == _context.UserId)
                {
                    Entity _currentapprovervoucher = _organizationservice.Retrieve(_entityname_matrixapprovalconditiontype, _currentapprovervoucherid, new ColumnSet(true));

                    // UPDATE QUOTE
                    Entity _quote_update = new Entity(_entityname_quote);
                    _quote_update.Id = _quoteid;

                    _quote_update["ittn_needapprovevoucher"] = false;
                    _quote_update["ittn_reqapprovevoucherdate"] = null;
                    _quote_update["ittn_currentapprovervoucher"] = null;
                    _quote_update["ittn_approvevoucherdate"] = DateTime.Now.ToLocalTime();
                    _quote_update["ittn_approvevoucherby"] = new EntityReference(_entityname_systemuser, _context.UserId);

                    _organizationservice.Update(_quote_update);

                    // UPDATE QUOTE PRODUCT
                    _queryexpression = new QueryExpression(_entityname_quoteconditiontype);
                    _queryexpression.ColumnSet = new ColumnSet(true);
                    _queryexpression.Criteria.AddCondition("ittn_quote", ConditionOperator.Equal, _quoteid);
                    _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontype.Id);
                    _queryexpression.Criteria.AddCondition("ittn_statusreason", ConditionOperator.Equal, STATUS_INPROGRESS);
                    EntityCollection _quoteconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                    foreach (var _quoteconditiontype in _quoteconditiontypes.Entities)
                    {
                        Entity _quoteconditiontype_forupdate = new Entity(_entityname_quoteconditiontype);
                        _quoteconditiontype_forupdate.Id = _quoteconditiontype.Id;

                        _quoteconditiontype_forupdate["ittn_statusreason"] = new OptionSetValue(STATUS_INACTIVE);

                        _organizationservice.Update(_quoteconditiontype_forupdate);
                    }
                }
                else
                {
                    throw new InvalidWorkflowException("You DO NOT have access to approve this Quote - Voucher !");
                }

            }

        }

        #endregion
        // ---------------------------------------- ---------------------------------------- ---------------------------------------- ----------------------------------------



        // ---------------------------------------- ---------------------------------------- ---------------------------------------- ----------------------------------------
        #region OTHERS

        public void AddOthers(IOrganizationService _organizationservice, IWorkflowContext _context, string[] _recordids)
        {
            Guid _quoteid = new Guid(_recordids[0]);
            Guid _masterconditiontypeid = new Guid();

            Entity _quote = _organizationservice.Retrieve(_entityname_quote, _quoteid, new ColumnSet(true));

            QueryExpression _queryexpression = new QueryExpression(_entityname_masterconditiontype);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition("ittn_code", ConditionOperator.Equal, "ZO99");
            EntityCollection _masterconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

            if (_masterconditiontypes.Entities.Count() == 0)
                throw new InvalidWorkflowException("There is no Code 'ZO99' in Master Condition Type!");
            else
                _masterconditiontypeid = _masterconditiontypes.Entities.FirstOrDefault().Id;

            _queryexpression = new QueryExpression(_entityname_quotedetail);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition("quoteid", ConditionOperator.Equal, _quoteid);
            _queryexpression.Criteria.AddCondition("new_parentnumber", ConditionOperator.Null);
            EntityCollection _quotedetails = _organizationservice.RetrieveMultiple(_queryexpression);

            foreach (var _quotedetail in _quotedetails.Entities)
            {
                decimal _amount = 0;
                decimal _totalamount = 0;
                decimal _itemnumber = _quotedetail.GetAttributeValue<decimal>("new_itemnumber");
                decimal _quantity = _quotedetail.GetAttributeValue<decimal>("new_quantity");

                if (_quote.Attributes.Contains("ittn_othersamount"))
                {
                    _amount = _quote.GetAttributeValue<Money>("ittn_othersamount").Value;
                    _totalamount = _amount;
                }

                if (_quantity > 0)
                    _amount = _amount / _quantity;

                //_queryexpression = new QueryExpression(_entityname_quoteconditiontype);
                //_queryexpression.ColumnSet = new ColumnSet(false);
                //_queryexpression.Criteria.AddCondition("ittn_quote", ConditionOperator.Equal, _quoteid);
                //_queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontypeid);

                //EntityCollection _quoteconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                //if (_quoteconditiontypes.Entities.Count() > 0)
                //    throw new InvalidWorkflowException("Code 'ZO99' ( Others ) already exist in this Quotation Condition Type!");
                //else
                //{
                Entity _quoteconditiontype = new Entity(_entityname_quoteconditiontype);

                _quoteconditiontype["ittn_amount"] = new Money(_amount);
                _quoteconditiontype["ittn_totalamount"] = new Money(_totalamount);
                _quoteconditiontype["ittn_conditiontype"] = new EntityReference(_entityname_masterconditiontype, _masterconditiontypeid);
                _quoteconditiontype["ittn_name"] = _masterconditiontypes.Entities.FirstOrDefault().GetAttributeValue<string>("ittn_description");
                _quoteconditiontype["ittn_quote"] = new EntityReference(_entityname_quote, _quoteid);
                _quoteconditiontype["ittn_itemnumber"] = _itemnumber;
                _quoteconditiontype["ittn_needapproveconditiontype"] = _masterconditiontypes.Entities.FirstOrDefault().GetAttributeValue<bool>("ittn_needapproval");
                _quoteconditiontype["transactioncurrencyid"] = new EntityReference(_entityname_transactioncurrency, Globalization.GetTransactionCurrencyID(_organizationservice, _entityname_transactioncurrency_code));

                _organizationservice.Create(_quoteconditiontype);
                //}
            }

        }

        public void RequestApprovalForOthers(IOrganizationService _organizationservice, IWorkflowContext _context, string[] _recordids)
        {
            Guid _quoteid = new Guid(_recordids[0]);
            Entity _masterconditiontype = new Entity(_entityname_masterconditiontype);
            Entity _matrixapprovalconditiontype = new Entity(_entityname_matrixapprovalconditiontype);
            Entity _quote = _organizationservice.Retrieve(_entityname_quote, _quoteid, new ColumnSet(true));
            Entity _owner = _organizationservice.Retrieve(_entityname_systemuser, _quote.GetAttributeValue<EntityReference>("createdby").Id, new ColumnSet(true));
            Guid _unitgroupid = _quote.GetAttributeValue<EntityReference>("new_unitgroup").Id;

            QueryExpression _queryexpression = new QueryExpression(_entityname_masterconditiontype);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition("ittn_code", ConditionOperator.Equal, CODE_OTHERS);
            EntityCollection _masterconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

            if (_masterconditiontypes.Entities.Count() == 0)
                throw new InvalidWorkflowException("There is no Code '" + CODE_OTHERS + "' in Master Condition Type!");
            else
                _masterconditiontype = _masterconditiontypes.Entities.FirstOrDefault();

            // GET MATRIX APPROVAL
            _queryexpression = new QueryExpression(_entityname_matrixapprovalconditiontype);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Orders.Add(new OrderExpression("ittn_priorityno", OrderType.Ascending));
            _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontype.Id);
            _queryexpression.Criteria.AddCondition("ittn_unitgroup", ConditionOperator.Equal, _unitgroupid);
            _queryexpression.Criteria.AddCondition("ittn_approveby", ConditionOperator.Equal, APPROVEBY_GENERALMANAGER);
            EntityCollection _matrixapprovalconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

            if (_matrixapprovalconditiontypes.Entities.Count() == 0)
                throw new InvalidWorkflowException("There is no Approver for code '" + CODE_OTHERS + "' with level 'GENERAL MANAGER' in Master Condition Type!");
            else
                _matrixapprovalconditiontype = _matrixapprovalconditiontypes.Entities.FirstOrDefault();

            // SHARE RECORDS
            Entity _currentapprover = _organizationservice.Retrieve(_entityname_systemuser, _matrixapprovalconditiontype.GetAttributeValue<EntityReference>("ittn_approver").Id, new ColumnSet(true));
            ShareRecords _sharerecords = new ShareRecords();
            _sharerecords.ShareRecord(_organizationservice, _quote, _currentapprover);

            // UPDATE QUOTE
            Entity _quote_update = new Entity(_entityname_quote);
            _quote_update.Id = _quoteid;

            _quote_update["ittn_needapproveothers"] = true;
            _quote_update["ittn_reqapproveothersdate"] = DateTime.Now.ToLocalTime();
            _quote_update["ittn_currentapproverothers"] = new EntityReference(_entityname_matrixapprovalconditiontype, _matrixapprovalconditiontype.Id);

            _organizationservice.Update(_quote_update);

            // UPDATE QUOTE PRODUCT
            _queryexpression = new QueryExpression(_entityname_quoteconditiontype);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition("ittn_quote", ConditionOperator.Equal, _quoteid);
            _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontype.Id);
            _queryexpression.Criteria.AddCondition("ittn_statusreason", ConditionOperator.Equal, STATUS_NOTASSIGN);
            EntityCollection _quoteconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

            foreach (var _quoteconditiontype in _quoteconditiontypes.Entities)
            {
                if (_quoteconditiontype.GetAttributeValue<Money>("ittn_amount").Value > 0)
                {
                    Entity _quoteconditiontype_forupdate = new Entity(_entityname_quoteconditiontype);
                    _quoteconditiontype_forupdate.Id = _quoteconditiontype.Id;

                    _quoteconditiontype_forupdate["ittn_statusreason"] = new OptionSetValue(STATUS_INPROGRESS);

                    _organizationservice.Update(_quoteconditiontype_forupdate);
                }
                else
                {
                    throw new InvalidWorkflowException("Amount OTHERS must be greater than 0 !");
                }
            }

            // SEND EMAIL
            SendMail(_organizationservice, _context, _masterconditiontype, _quote, _owner, _currentapprover, Guid.Empty);

        }

        public void ApproveOthers(IOrganizationService _organizationservice, IWorkflowContext _context, string[] _recordids)
        {
            Guid _quoteid = new Guid(_recordids[0]);
            Entity _masterconditiontype = new Entity(_entityname_masterconditiontype);
            Entity _matrixapprovalconditiontype = new Entity(_entityname_matrixapprovalconditiontype);
            Entity _quote = _organizationservice.Retrieve(_entityname_quote, _quoteid, new ColumnSet(true));
            Entity _owner = _organizationservice.Retrieve(_entityname_systemuser, _quote.GetAttributeValue<EntityReference>("createdby").Id, new ColumnSet(true));
            Guid _unitgroupid = _quote.GetAttributeValue<EntityReference>("new_unitgroup").Id;

            QueryExpression _queryexpression = new QueryExpression(_entityname_masterconditiontype);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition("ittn_code", ConditionOperator.Equal, CODE_OTHERS);
            EntityCollection _masterconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

            if (_masterconditiontypes.Entities.Count() == 0)
                throw new InvalidWorkflowException("There is no Code '" + CODE_OTHERS + "' in Master Condition Type!");
            else
                _masterconditiontype = _masterconditiontypes.Entities.FirstOrDefault();

            Guid _currentapproverothersid = _quote.Attributes.Contains("ittn_currentapproverothers") ? _quote.GetAttributeValue<EntityReference>("ittn_currentapproverothers").Id : new Guid();

            if (_currentapproverothersid != new Guid())
            {
                Entity _currentapprover = _organizationservice.Retrieve(_entityname_matrixapprovalconditiontype, _currentapproverothersid, new ColumnSet(true));

                if (_currentapprover.GetAttributeValue<EntityReference>("ittn_approver").Id == _context.UserId)
                {
                    Entity _currentapproverothers = _organizationservice.Retrieve(_entityname_matrixapprovalconditiontype, _currentapproverothersid, new ColumnSet(true));

                    //if (_currentapproverothers.GetAttributeValue<OptionSetValue>("ittn_approveby").Value == APPROVEBY_SALESMANAGER)
                    //{
                    //    // GET MATRIX APPROVAL
                    //    _queryexpression = new QueryExpression(_entityname_matrixapprovalconditiontype);
                    //    _queryexpression.ColumnSet = new ColumnSet(true);
                    //    _queryexpression.Orders.Add(new OrderExpression("ittn_priorityno", OrderType.Ascending));
                    //    _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontype.Id);
                    //    _queryexpression.Criteria.AddCondition("ittn_unitgroup", ConditionOperator.Equal, _unitgroupid);
                    //    _queryexpression.Criteria.AddCondition("ittn_approveby", ConditionOperator.Equal, APPROVEBY_GENERALMANAGER);

                    //    EntityCollection _matrixapprovalconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                    //    if (_matrixapprovalconditiontypes.Entities.Count() == 0)
                    //        throw new InvalidWorkflowException("There is no Approver for code '" + CODE_OTHERS + "' with level 'General Manager' in Master Condition Type!");
                    //    else
                    //        _matrixapprovalconditiontype = _matrixapprovalconditiontypes.Entities.FirstOrDefault();

                    //    // UPDATE QUOTE
                    //    Entity _quote_update = new Entity(_entityname_quote);
                    //    _quote_update.Id = _quoteid;

                    //    _quote_update["ittn_needapproveothers"] = true;
                    //    _quote_update["ittn_reqapproveothersdate"] = DateTime.Now.ToLocalTime();
                    //    _quote_update["ittn_currentapproverothers"] = new EntityReference(_entityname_matrixapprovalconditiontype, _matrixapprovalconditiontype.Id);
                    //    _quote_update["ittn_approveothersdate"] = null;
                    //    _quote_update["ittn_approveothersby"] = null;

                    //    _organizationservice.Update(_quote_update);
                    //}
                    //else
                    if (_currentapproverothers.GetAttributeValue<OptionSetValue>("ittn_approveby").Value == APPROVEBY_GENERALMANAGER)
                    {
                        // GET MATRIX APPROVAL
                        _queryexpression = new QueryExpression(_entityname_matrixapprovalconditiontype);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Orders.Add(new OrderExpression("ittn_priorityno", OrderType.Ascending));
                        _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontype.Id);
                        _queryexpression.Criteria.AddCondition("ittn_unitgroup", ConditionOperator.Equal, _unitgroupid);
                        _queryexpression.Criteria.AddCondition("ittn_approveby", ConditionOperator.Equal, APPROVEBY_DIRECTOR);

                        EntityCollection _matrixapprovalconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                        if (_matrixapprovalconditiontypes.Entities.Count() == 0)
                            throw new InvalidWorkflowException("There is no Approver for code '" + CODE_OTHERS + "' with level 'DIRECTOR' in Master Condition Type!");
                        else
                            _matrixapprovalconditiontype = _matrixapprovalconditiontypes.Entities.FirstOrDefault();

                        // SHARE RECORDS
                        Entity _nextapprover = _organizationservice.Retrieve(_entityname_systemuser, _matrixapprovalconditiontype.GetAttributeValue<EntityReference>("ittn_approver").Id, new ColumnSet(true));
                        ShareRecords _sharerecords = new ShareRecords();
                        _sharerecords.ShareRecord(_organizationservice, _quote, _nextapprover);

                        // UPDATE QUOTE
                        Entity _quote_update = new Entity(_entityname_quote);
                        _quote_update.Id = _quoteid;

                        _quote_update["ittn_needapproveothers"] = true;
                        _quote_update["ittn_reqapproveothersdate"] = DateTime.Now.ToLocalTime();
                        _quote_update["ittn_currentapproverothers"] = new EntityReference(_entityname_matrixapprovalconditiontype, _matrixapprovalconditiontype.Id);
                        _quote_update["ittn_approveothersdate"] = null;
                        _quote_update["ittn_approveothersby"] = null;

                        _organizationservice.Update(_quote_update);

                        // SEND EMAIL
                        SendMail(_organizationservice, _context, _masterconditiontype, _quote, _owner, _nextapprover, Guid.Empty);
                    }
                    else if (_currentapproverothers.GetAttributeValue<OptionSetValue>("ittn_approveby").Value == APPROVEBY_DIRECTOR)
                    {
                        // UPDATE QUOTE
                        Entity _quote_update = new Entity(_entityname_quote);
                        _quote_update.Id = _quoteid;

                        _quote_update["ittn_needapproveothers"] = false;
                        _quote_update["ittn_currentapproverothers"] = null;
                        _quote_update["ittn_approveothersdate"] = DateTime.Now.ToLocalTime();
                        _quote_update["ittn_approveothersby"] = new EntityReference(_entityname_systemuser, _context.UserId);

                        _organizationservice.Update(_quote_update);

                        // UPDATE QUOTE PRODUCT
                        _queryexpression = new QueryExpression(_entityname_quoteconditiontype);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria.AddCondition("ittn_quote", ConditionOperator.Equal, _quoteid);
                        _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontype.Id);
                        _queryexpression.Criteria.AddCondition("ittn_statusreason", ConditionOperator.Equal, STATUS_INPROGRESS);
                        EntityCollection _quoteconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                        foreach (var _quoteconditiontype in _quoteconditiontypes.Entities)
                        {
                            Entity _quoteconditiontype_forupdate = new Entity(_entityname_quoteconditiontype);
                            _quoteconditiontype_forupdate.Id = _quoteconditiontype.Id;

                            _quoteconditiontype_forupdate["ittn_statusreason"] = new OptionSetValue(STATUS_ACTIVE);

                            _organizationservice.Update(_quoteconditiontype_forupdate);
                        }

                    }
                }
                else
                {
                    throw new InvalidWorkflowException("You DO NOT have access to approve this Quote - Others !");
                }

            }

        }

        public void RejectOthers(IOrganizationService _organizationservice, IWorkflowContext _context, string[] _recordids)
        {
            Guid _quoteid = new Guid(_recordids[0]);
            Entity _masterconditiontype = new Entity(_entityname_masterconditiontype);
            Entity _matrixapprovalconditiontype = new Entity(_entityname_matrixapprovalconditiontype);
            Entity _quote = _organizationservice.Retrieve(_entityname_quote, _quoteid, new ColumnSet(true));
            Guid _unitgroupid = _quote.GetAttributeValue<EntityReference>("new_unitgroup").Id;

            QueryExpression _queryexpression = new QueryExpression(_entityname_masterconditiontype);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition("ittn_code", ConditionOperator.Equal, CODE_OTHERS);
            EntityCollection _masterconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

            if (_masterconditiontypes.Entities.Count() == 0)
                throw new InvalidWorkflowException("There is no Code '" + CODE_OTHERS + "' in Master Condition Type!");
            else
                _masterconditiontype = _masterconditiontypes.Entities.FirstOrDefault();

            Guid _currentapproverothersid = _quote.Attributes.Contains("ittn_currentapproverothers") ? _quote.GetAttributeValue<EntityReference>("ittn_currentapproverothers").Id : new Guid();

            if (_currentapproverothersid != new Guid())
            {
                Entity _currentapprover = _organizationservice.Retrieve(_entityname_matrixapprovalconditiontype, _currentapproverothersid, new ColumnSet(true));

                if (_currentapprover.GetAttributeValue<EntityReference>("ittn_approver").Id == _context.UserId)
                {
                    Entity _currentapproverothers = _organizationservice.Retrieve(_entityname_matrixapprovalconditiontype, _currentapproverothersid, new ColumnSet(true));

                    // UPDATE QUOTE
                    Entity _quote_update = new Entity(_entityname_quote);
                    _quote_update.Id = _quoteid;

                    _quote_update["ittn_needapproveothers"] = false;
                    _quote_update["ittn_reqapproveothersdate"] = null;
                    _quote_update["ittn_currentapproverothers"] = null;
                    _quote_update["ittn_approveothersdate"] = DateTime.Now.ToLocalTime();
                    _quote_update["ittn_approveothersby"] = new EntityReference(_entityname_systemuser, _context.UserId);

                    _organizationservice.Update(_quote_update);

                    // UPDATE QUOTE PRODUCT
                    _queryexpression = new QueryExpression(_entityname_quoteconditiontype);
                    _queryexpression.ColumnSet = new ColumnSet(true);
                    _queryexpression.Criteria.AddCondition("ittn_quote", ConditionOperator.Equal, _quoteid);
                    _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _masterconditiontype.Id);
                    _queryexpression.Criteria.AddCondition("ittn_statusreason", ConditionOperator.Equal, STATUS_INPROGRESS);
                    EntityCollection _quoteconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                    foreach (var _quoteconditiontype in _quoteconditiontypes.Entities)
                    {
                        Entity _quoteconditiontype_forupdate = new Entity(_entityname_quoteconditiontype);
                        _quoteconditiontype_forupdate.Id = _quoteconditiontype.Id;

                        _quoteconditiontype_forupdate["ittn_statusreason"] = new OptionSetValue(STATUS_INACTIVE);

                        _organizationservice.Update(_quoteconditiontype_forupdate);
                    }
                }
                else
                {
                    throw new InvalidWorkflowException("You DO NOT have access to approve this Quote - Others !");
                }

            }

        }

        #endregion
        // ---------------------------------------- ---------------------------------------- ---------------------------------------- ----------------------------------------



        // ---------------------------------------- ---------------------------------------- ---------------------------------------- ----------------------------------------

        public void ChangeColor(IOrganizationService _organizationservice, IWorkflowContext _context, string[] _recordids)
        {
            Guid _quotedetailid = new Guid(_recordids[0]);
            Entity _quotedetail = _organizationservice.Retrieve(_entityname_quotedetail, _quotedetailid, new ColumnSet(true));
            EntityReference _unitgroup = _quotedetail.GetAttributeValue<EntityReference>("new_unitgroup");
            Entity Quote = _organizationservice.Retrieve(_entityname_quote, ((EntityReference)_quotedetail.Attributes["quoteid"]).Id, new ColumnSet(true));
            int QuoteStatusReason = Quote.Contains("ittn_statusreason") ? ((OptionSetValue)Quote.Attributes["ittn_statusreason"]).Value: 0;

            if (QuoteStatusReason == 841150000 || QuoteStatusReason == 841150002) //Quote Status Reason : Draft or Approved
            {
                if (!_quotedetail.Contains("new_parentnumber"))
                {
                    QueryExpression _qematerialgroup = new QueryExpression(_entityname_materialgroup);
                    _qematerialgroup.ColumnSet = new ColumnSet(true);
                    _qematerialgroup.Criteria.AddCondition("new_mgcode", ConditionOperator.Equal, PRODUCT_MATERIALGROUP_PAINTING);
                    Entity _materialgroup = _organizationservice.RetrieveMultiple(_qematerialgroup).Entities.FirstOrDefault();

                    if (_materialgroup != null)
                    {
                        QueryExpression _queryexpression = new QueryExpression(_entityname_product);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria.AddCondition("productnumber", ConditionOperator.Equal, PRODUCT_PRODUCTNUMBER_PAINTING);
                        _queryexpression.Criteria.AddCondition("producttypecode", ConditionOperator.Equal, PRODUCT_PRODUCTTYPE_SERVICE);
                        _queryexpression.Criteria.AddCondition("new_materialgroup", ConditionOperator.Equal, _materialgroup.Id);
                        _queryexpression.Criteria.AddCondition("defaultuomscheduleid", ConditionOperator.Equal, _unitgroup.Id);
                        Entity _product = _organizationservice.RetrieveMultiple(_queryexpression).Entities.FirstOrDefault();

                        if (_product != null)
                        {
                            Entity _quotedetailupdate = new Entity(_entityname_quotedetail);
                            _quotedetailupdate["quoteid"] = (EntityReference)_quotedetail.Attributes["quoteid"];
                            _quotedetailupdate["productid"] = new EntityReference(_entityname_product, (Guid)_product.Attributes["productid"]);
                            _quotedetailupdate["uomid"] = (EntityReference)_quotedetail.Attributes["uomid"];
                            _quotedetailupdate["new_itemnumber"] = GetLastItemNumber(_organizationservice, _context, _quotedetail);
                            _quotedetailupdate["new_parentnumber"] = (Decimal)_quotedetail.Attributes["new_itemnumber"];
                            _quotedetailupdate["new_unitgroup"] = (EntityReference)_quotedetail.Attributes["new_unitgroup"];
                            _quotedetailupdate["new_deliveryoption"] = (OptionSetValue)_quotedetail.Attributes["new_deliveryoption"];
                            _quotedetailupdate["ittn_shippingpoint"] = _quotedetail.Contains("ittn_shippingpoint") ? (EntityReference)_quotedetail.Attributes["ittn_shippingpoint"] : null;
                            _quotedetailupdate["ittn_deliveryplanbranch"] = _quotedetail.Contains("ittn_deliveryplanbranch") ? (EntityReference)_quotedetail.Attributes["ittn_deliveryplanbranch"] : null;
                            _quotedetailupdate["ittn_shippingaddress"] = _quotedetail.Contains("ittn_shippingaddress") ? (String)_quotedetail.Attributes["ittn_shippingaddress"] : null;
                            _quotedetailupdate["new_quantity"] = (Decimal)_quotedetail.Attributes["new_quantity"];
                            _quotedetailupdate["quantity"] = (Decimal)_quotedetail.Attributes["quantity"];
                            _quotedetailupdate["priceperunit"] = new Money(0);
                            _quotedetailupdate["new_percentagediscount"] = new decimal(0);
                            _quotedetailupdate["manualdiscountamount"] = new Money(0);
                            _quotedetailupdate["ittn_totalextendedamount"] = new Money(0);
                            _quotedetailupdate["description"] = _product.Contains("description") ? (String)_product.Attributes["description"] : null;

                            _organizationservice.Create(_quotedetailupdate);
                        }
                        else
                            throw new InvalidWorkflowException("There is no ProductID '" + PRODUCT_PRODUCTNUMBER_PAINTING + "' for Unit Group '" + _unitgroup.Name + "' in Product!");
                    }
                    else
                        throw new InvalidWorkflowException("There is no MG Code '" + PRODUCT_MATERIALGROUP_PAINTING + "' in Material Group!");
                }
                else
                {
                    throw new InvalidWorkflowException("You only can change color for main unit!");
                }
            }
            else
            {
                throw new InvalidWorkflowException("You only can change color when Quote Status Reason is Draft or Approved!");
            }
        }

        public decimal GetLastItemNumber(IOrganizationService _organizationservice, IWorkflowContext _context, Entity _quotedetail)
        {
            decimal LastItemNumber = 0;

            QueryExpression _queryexpression = new QueryExpression(_entityname_quotedetail);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition("quoteid", ConditionOperator.Equal, ((EntityReference)_quotedetail.Attributes["quoteid"]).Id);
            _queryexpression.Criteria.AddCondition("new_parentnumber", ConditionOperator.Equal, (Decimal)_quotedetail.Attributes["new_itemnumber"]);
            _queryexpression.AddOrder("new_itemnumber", OrderType.Descending);
            _queryexpression.PageInfo.Count = 1;
            _queryexpression.PageInfo.PageNumber = 1;
            Entity _quotedetailmax = _organizationservice.RetrieveMultiple(_queryexpression).Entities.FirstOrDefault();

            if (_quotedetailmax != null)
            {
                LastItemNumber = _quotedetailmax.GetAttributeValue<decimal>("new_itemnumber") + new decimal(10);
            }
            return LastItemNumber;
        }

        // ---------------------------------------- ---------------------------------------- ---------------------------------------- ----------------------------------------
    }
}
