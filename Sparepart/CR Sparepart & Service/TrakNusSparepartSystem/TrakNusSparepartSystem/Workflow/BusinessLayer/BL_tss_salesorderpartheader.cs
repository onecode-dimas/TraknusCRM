using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using TrakNusSparepartSystem.DataLayer;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using TrakNusSparepartSystem.Workflow.Helper;
using TrakNusSparepartSystem.Workflow.WebserviceSAP;
using System.ServiceModel.Configuration;
using System.Configuration;
using System.ServiceModel;
using System.Web;
using System.Activities;
using Microsoft.Xrm.Sdk.Client;

namespace TrakNusSparepartSystem.Workflow.BusinessLayer
{
    public class BL_tss_salesorderpartheader
    {
        #region Constants
        private const int PartInterchange_STATUS_ACTIVE = 865920000;
        private const int SOURCETYPE_SERVICE = 865920002;

        private const int STATECODE_CANCEL = 865920002;
        private const int STATECODE_SOPROCESSEDTOSAP = 865920001;
        private const int STATUSREASON_CLOSED = 865920002;
        private const int STATUSREASON_SUBMITTED = 865920001;

        private const int QUOTATIONSTATUS_CLOSED = 865920004;
        private const int QUOTATIONSTATUSREASON_LOST = 865920007;
        private const int QUOTATIONSTATUSASSIGNQUO_SOSUBMITTEDTOSAP = 865920005;

        private const int PROSPECTSTATUSREASON_LOST = 865920002;

        private const int SUBLINE_TRANSCTTYPE_CREATEDO = 865920000;
        private const string USER_TITLE_COUNTER = "Counter Part";
        #endregion

        #region Dependencies
        private DL_account _DL_account = new DL_account();
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        private DL_businessunit _DL_businessunit = new DL_businessunit();
        private DL_tss_prospectpartheader _DL_tss_prospectpartheader = new DL_tss_prospectpartheader();
        private DL_trs_quotation _DL_trs_quotation = new DL_trs_quotation();
        private DL_contact _DL_contact = new DL_contact();
        private DL_paymentterm _DL_paymentterm = new DL_paymentterm();
        private DL_tss_salesorderpartheader _DL_tss_salesorderpartheader = new DL_tss_salesorderpartheader();
        private DL_tss_quotationpartheader _DL_tss_quotationpartheader = new DL_tss_quotationpartheader();
        private Service serviceEncryption = new Service();
        private MWSLog _mwsLog = new MWSLog();
        private ShareRecords _ShareRecords = new ShareRecords();
        #endregion

        #region Properties
        private string _classname = "BL_tss_salesorderpartheader";

        private string _entityname = "tss_sopartheader";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Sales Order Part Header";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _tss_sonumber = false;
        private string _tss_sonumber_value;
        public string tss_sonumber
        {
            get { return _tss_sonumber ? _tss_sonumber_value : null; }
            set { _tss_sonumber = true; _tss_sonumber_value = value; }
        }

        private bool _tss_sotationid = false;
        private string _tss_sotationid_value;
        public string tss_sotationid
        {
            get { return _tss_sotationid ? _tss_sotationid_value : null; }
            set { _tss_sotationid = true; _tss_sotationid_value = value; }
        }

        private bool _tss_customer = false;
        private EntityReference _tss_customer_value;
        public Guid tss_customer
        {
            get { return _tss_customer ? _tss_customer_value.Id : Guid.Empty; }
            set { _tss_customer = true; _tss_customer_value = new EntityReference(_DL_account.EntityName, value); }
        }

        private bool _tss_branch = false;
        private EntityReference _tss_branch_value;
        public Guid tss_branch
        {
            get { return _tss_branch ? _tss_branch_value.Id : Guid.Empty; }
            set { _tss_branch = true; _tss_branch_value = new EntityReference(_DL_businessunit.EntityName, value); }
        }

        private bool _tss_quotationlink = false;
        private string _tss_quotationlink_value;
        public string tss_quotationlink
        {
            get { return _tss_quotationlink ? _tss_quotationlink_value : string.Empty; }
            set { _tss_quotationlink = true; _tss_quotationlink_value = value; }
        }

        private bool _tss_prospectlink = false;
        private EntityReference _tss_prospectlink_value;
        public Guid tss_prospectlink
        {
            get { return _tss_prospectlink ? _tss_prospectlink_value.Id : Guid.Empty; }
            set { _tss_prospectlink = true; _tss_prospectlink_value = new EntityReference(_DL_tss_prospectpartheader.EntityName, value); }
        }

        private bool _tss_quotserviceno = false;
        private EntityReference _tss_quotserviceno_value;
        public Guid tss_quotserviceno
        {
            get { return _tss_quotserviceno ? _tss_quotserviceno_value.Id : Guid.Empty; }
            set { _tss_quotserviceno = true; _tss_quotserviceno_value = new EntityReference(_DL_trs_quotation.EntityName, value); }
        }

        private bool _tss_estimationclosedate = false;
        private DateTime _tss_estimationclosedate_value;
        public DateTime tss_estimationclosedate
        {
            get { return _tss_estimationclosedate ? _tss_estimationclosedate_value.ToLocalTime() : DateTime.MinValue; }
            set { _tss_estimationclosedate = true; _tss_estimationclosedate_value = value.ToLocalTime(); }
        }

        private bool _tss_requestdeliverydate = false;
        private DateTime _tss_requestdeliverydate_value;
        public DateTime tss_requestdeliverydate
        {
            get { return _tss_requestdeliverydate ? _tss_requestdeliverydate_value.ToLocalTime() : DateTime.MinValue; }
            set { _tss_requestdeliverydate = true; _tss_requestdeliverydate_value = value.ToLocalTime(); }
        }

        private bool _tss_pss = false;
        private EntityReference _tss_pss_value;
        public Guid tss_pss
        {
            get { return _tss_pss ? _tss_pss_value.Id : Guid.Empty; }
            set { _tss_pss = true; _tss_pss_value = new EntityReference(_DL_systemuser.EntityName, value); }
        }

        //OptionSet
        private bool _tss_statuscode = false;
        private int _tss_statuscode_value;
        public int tss_statuscode
        {
            get { return _tss_statuscode ? _tss_statuscode_value : int.MinValue; }
            set { _tss_statuscode = true; _tss_statuscode_value = value; }
        }

        //OptionSet
        private bool _tss_statusreason = false;
        private int _tss_statusreason_value;
        public int tss_statusreason
        {
            get { return _tss_statusreason ? _tss_statusreason_value : int.MinValue; }
            set { _tss_statusreason = true; _tss_statusreason_value = value; }
        }

        private bool _tss_revision = false;
        private int _tss_revision_value;
        public int tss_revision
        {
            get { return _tss_revision ? _tss_revision_value : int.MinValue; }
            set { _tss_revision = true; _tss_revision_value = value; }
        }

        private bool _tss_contact = false;
        private EntityReference _tss_contact_value;
        public Guid tss_contact
        {
            get { return _tss_contact ? _tss_contact_value.Id : Guid.Empty; }
            set { _tss_contact = true; _tss_contact_value = new EntityReference(_DL_contact.EntityName, value); }
        }

        private bool _tss_totalamount = false;
        private Money _tss_totalamount_value;
        public decimal tss_totalamount
        {
            get { return _tss_totalamount ? _tss_totalamount_value.Value : 0; }
            set { _tss_totalamount = true; _tss_totalamount_value = new Money(value); }
        }
        
        //OptionSet
        private bool _tss_sourcetype = false;
        private int _tss_sourcetype_value;
        public int tss_sourcetype
        {
            get { return _tss_sourcetype ? _tss_sourcetype_value : int.MinValue; }
            set { _tss_sourcetype = true; _tss_sourcetype_value = value; }
        }

        //OptionSet
        private bool _tss_top = false;
        private int _tss_top_value;
        public int tss_top
        {
            get { return _tss_top ? _tss_top_value : int.MinValue; }
            set { _tss_top = true; _tss_top_value = value; }
        }

        //Two Option
        private bool _tss_package = false;
        private int _tss_package_value;
        public int tss_package
        {
            get { return _tss_package ? _tss_package_value : int.MinValue; }
            set { _tss_package = true; _tss_package_value = value; }
        }

        private bool _tss_paymentterm = false;
        private EntityReference _tss_paymentterm_value;
        public Guid tss_paymentterm
        {
            get { return _tss_paymentterm ? _tss_paymentterm_value.Id : Guid.Empty; }
            set { _tss_paymentterm = true; _tss_paymentterm_value = new EntityReference(_DL_paymentterm.EntityName, value); }
        }

        #endregion

        public void submitSOtoSAP_OnClick(IOrganizationService organizationService, Guid id, ITracingService trace)
        {
            try
            {
                //Parameter Sales Order Header
                string token, order_type, CRM_SO_NO, Sales_Org, Dis_Ch, Division, Sales_Off, Customer_Code,
                    PO_NO_Cust, Payment_Term, Currency, Delivery_Plant, Package_Type, Package_Name, Package_NO,
                    Package_Price, Sales_Unit_Package, Salesman, Price_List, Price_Group, Sold_To, Bill_To, Ship_To;

                DateTime PO_Date_Cust, Req_Delivery_Date;
                int Package_Qty = 0; //set default to 0
                string UserTitle = string.Empty;

                #region set empty all string
                token = string.Empty;
                order_type = string.Empty;
                CRM_SO_NO = string.Empty;
                Sales_Org = string.Empty;
                Dis_Ch = string.Empty;
                Division = string.Empty;
                Sales_Off = string.Empty;
                Customer_Code = string.Empty;
                PO_NO_Cust = string.Empty;
                Payment_Term = string.Empty;
                Currency = string.Empty;
                Delivery_Plant = string.Empty;
                Package_Type = string.Empty;
                Package_Name = string.Empty;
                Package_NO = string.Empty;
                Package_Price = string.Empty;
                Sales_Unit_Package = string.Empty;
                Salesman = string.Empty;
                Price_List = string.Empty;
                Price_Group = string.Empty;
                Sold_To = string.Empty;
                Bill_To = string.Empty;
                Ship_To = string.Empty;
                #endregion

                ZcrmCreateSo create_SO = new ZcrmCreateSo();
                List<ZstrDetailSo> details = new List<ZstrDetailSo>();
                DateTime today = DateTime.Now.Date;

                //0035192283
                var getSOPartHeader = _DL_tss_salesorderpartheader.Select(organizationService, id);
                if (getSOPartHeader.Attributes.Contains("tss_sonumber"))
                {
                    QueryExpression queryPartsetup = new QueryExpression("tss_sparepartsetup");
                    queryPartsetup.ColumnSet = new ColumnSet(true);
                    queryPartsetup.Criteria.AddCondition("tss_name", ConditionOperator.Equal, "TSS");
                    Entity Partsetup = organizationService.RetrieveMultiple(queryPartsetup).Entities[0];
                    string uniqueKey = Partsetup.GetAttributeValue<string>("tss_sapintegrationuniquekey");
                    string SAPPassword = Partsetup.GetAttributeValue<string>("tss_sapwebservicepassword");

                    CRM_SO_NO = getSOPartHeader.Attributes["tss_sonumber"].ToString();
                    token = serviceEncryption.EncryptText(CRM_SO_NO, uniqueKey); //generate token

                    if (getSOPartHeader.Contains("tss_salesorganization"))
                    {
                        Entity Sls_Org = organizationService.Retrieve("new_salesorganization", getSOPartHeader.GetAttributeValue<EntityReference>("tss_salesorganization").Id, new ColumnSet(new string[] { "new_code" }));
                        Sales_Org = Sls_Org.GetAttributeValue<string>("new_code");
                    }
                    //getSOPartHeader.GetAttributeValue<EntityReference>("tss_salesorganization").Name;
                    if (getSOPartHeader.Contains("tss_distributionchannel"))
                    {
                        Entity dc = organizationService.Retrieve("new_distchannel", getSOPartHeader.GetAttributeValue<EntityReference>("tss_distributionchannel").Id, new ColumnSet(true));
                        Dis_Ch = dc.GetAttributeValue<string>("new_code");
                        //getSOPartHeader.GetAttributeValue<EntityReference>("tss_disctributionchannel").Name;
                    }
                    if (getSOPartHeader.Contains("tss_division"))
                    {
                        Division = getSOPartHeader.GetAttributeValue<EntityReference>("tss_division").Name;
                    }
                    if (getSOPartHeader.Contains("tss_ordertype"))
                    {
                        var orderTypeVal = getSOPartHeader.GetAttributeValue<OptionSetValue>("tss_ordertype").Value;

                        var context=  new OrganizationServiceContext(organizationService);
                        var stringmap = (from c in context.CreateQuery("stringmap")
                                         where c.GetAttributeValue<string>("attributename") == "tss_ordertype"
                                         where c.GetAttributeValue<int>("attributevalue") == orderTypeVal
                                         select c).ToList();

                        if (stringmap.Count > 0)
                        {
                            order_type = stringmap[0].GetAttributeValue<string>("value");
                        }
                    }

                    Entity customer = _DL_account.Select(organizationService, getSOPartHeader.GetAttributeValue<EntityReference>("tss_customer").Id);
                    #region check customer
                    // check apakah Account Number TN, NPWP, and Tax Status kosong/null, jika kosong maka batalin dan throw Please complete Account Number TN, NPWP, and Tax Status data for current Customer"
                    if (customer.Contains("accountnumber") && customer.Contains("new_npwp") && customer.Contains("new_taxstatus"))
                    {
                        //"";//to be confirmed 
                        Customer_Code = customer.GetAttributeValue<string>("accountnumber");
                        for (int x = Customer_Code.Length; x < 10; x++)
                        {
                            Customer_Code = "0" + Customer_Code;
                        }

                        try
                        {
                            QueryExpression querygetContracts = new QueryExpression("contract");
                            querygetContracts.ColumnSet = new ColumnSet(true);
                            querygetContracts.Criteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.And,
                                Conditions =
                                {
                                    new ConditionExpression("statecode", ConditionOperator.Equal, 2),
                                    new ConditionExpression("trs_customer", ConditionOperator.Equal, getSOPartHeader.GetAttributeValue<EntityReference>("tss_customer").Id),
                                    new ConditionExpression("activeon", ConditionOperator.LessEqual, today),
                                    new ConditionExpression("expireson", ConditionOperator.GreaterEqual, today)
                                }
                            };

                            //EntityCollection contracts = organizationService.RetrieveMultiple(querygetContracts);
                            //if (contracts.Entities.Count > 0)
                            //    order_type = "ZPSC";
                            //else
                            //    order_type = "ZPSE";
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidPluginExecutionException("Error when retrieve active contracts for current customer, with detail as follow : " + ex.Message);
                        }
                    }
                    else
                    {
                        throw new InvalidWorkflowException("Please complete Account Number TN, NPWP, and Tax Status data for current Customer");
                    }
                    #endregion

                    trace.Trace("Retrieve PO Number and Date");
                    PO_NO_Cust = getSOPartHeader.Attributes["tss_ponumber"].ToString();
                    PO_Date_Cust = getSOPartHeader.GetAttributeValue<DateTime>("tss_podate").ToLocalTime();

                    #region check forbidden po
                    trace.Trace("check forbidden PO");
                    // check apakah PO ada di forbidden po jk ya maka abort dan throw alert "PO Number contain forbidden text. Submit to SAP cancelled"
                    QueryExpression querycheckForbiddenPO = new QueryExpression("tss_forbiddenponumber");
                    querycheckForbiddenPO.ColumnSet = new ColumnSet(true);
                    querycheckForbiddenPO.Criteria.AddCondition("tss_forbidtext", ConditionOperator.Equal, PO_NO_Cust);
                    EntityCollection ForbiddenPO = organizationService.RetrieveMultiple(querycheckForbiddenPO);
                    if (ForbiddenPO.Entities.Count > 0)
                        throw new InvalidWorkflowException("PO Number contain forbidden text. Submit to SAP cancelled");
                    #endregion

                    if (getSOPartHeader.Contains("tss_paymentterm"))
                    {
                        Entity pt = organizationService.Retrieve("new_paymentterm", getSOPartHeader.GetAttributeValue<EntityReference>("tss_paymentterm").Id, new ColumnSet(true));
                        Payment_Term = pt.GetAttributeValue<string>("new_code"); //dari quotation services ambil code nya.
                    }
                    trace.Trace("Retrieve Currency");
                    Currency = getSOPartHeader.GetAttributeValue<EntityReference>("transactioncurrencyid").Name; //IDR
                    trace.Trace("Retrieve Req_Delivery_Date");
                    Req_Delivery_Date = getSOPartHeader.GetAttributeValue<DateTime>("tss_requestdeliverydate").ToLocalTime(); // dari quotation

                    //Entity BU_PSS = _DL_businessunit.Select(organizationService, PSS.GetAttributeValue<EntityReference>("businessunitid").Id);

                    //Delivery_Plant = ""; //tidak perlu branch.GetAttributeValue<string>("trs_branchcode");
                    //getSOPartHeader.GetAttributeValue<EntityReference>("tss_branch").Name; //branch code dari PSS
                    #region check ispackage
                    bool ispackage = getSOPartHeader.GetAttributeValue<bool>("tss_package");
                    if (ispackage)
                    {
                        Package_Type = "PACK1";
                    }
                    else
                    {
                        Package_Type = "PACK0";
                    }

                    #endregion

                    if (getSOPartHeader.Contains("tss_packagesname"))
                        Package_Name = getSOPartHeader.GetAttributeValue<string>("tss_packagesname");
                    if (getSOPartHeader.Contains("tss_packageno"))
                        Package_NO = getSOPartHeader.GetAttributeValue<string>("tss_packageno");
                    if (getSOPartHeader.Contains("tss_packageqty"))
                        Package_Qty = getSOPartHeader.GetAttributeValue<int>("tss_packageqty");
                    if (getSOPartHeader.Contains("tss_totalamount"))
                        Package_Price = Convert.ToInt64(getSOPartHeader.GetAttributeValue<Money>("tss_totalamount").Value).ToString();
                    if (getSOPartHeader.Contains("tss_packageunit"))
                        Sales_Unit_Package = getSOPartHeader.GetAttributeValue<EntityReference>("tss_packageunit").Name; //"Pc";
                    else
                        Sales_Unit_Package = "PC";

                    if (!getSOPartHeader.Contains("tss_pss"))
                        throw new InvalidWorkflowException("Field PSS is empty!");

                    Entity PSS = _DL_systemuser.Select(organizationService, getSOPartHeader.GetAttributeValue<EntityReference>("tss_pss").Id);

                    if (PSS.Contains("title") && PSS.Attributes["title"] != null)
                        UserTitle = PSS.GetAttributeValue<String>("title");

                    if (PSS.Contains("businessunitid"))
                    {
                        Entity branch = organizationService.Retrieve("businessunit", PSS.GetAttributeValue<EntityReference>("businessunitid").Id, new ColumnSet(new string[] { "trs_branchcode" }));
                        Sales_Off = branch.GetAttributeValue<string>("trs_branchcode");
                    }
                    if (PSS.Contains("new_nrp"))
                        Salesman = PSS.GetAttributeValue<string>("new_nrp");
                    else
                        Salesman = "9999999";//defaultnya

                    //if (customer.Contains("tss_balancearsparepart") && !string.IsNullOrEmpty(Package_Price) && customer.Contains("tss_plafondarsparepart"))
                    //{
                        //decimal amount = decimal.Parse(Package_Price);
                        //int.Parse(Package_Price);
                        //string balance_ar = customer.GetAttributeValue<string>("tss_balancearsparepart");//dari customer
                        //int plafond_ar = customer.GetAttributeValue<int>("tss_plafondarsparepart");
                        //decimal balance = decimal.Parse(balance_ar);
                        //decimal plafond = Convert.ToDecimal(plafond_ar);

                        //throw new Exception("balance ar : " + balance + "\r\n amount : " + amount);

                        #region Unused [15/10/2018]
                        //if (getSOPartHeader.GetAttributeValue<OptionSetValue>("tss_plafondbelowsoamount").Value != 865920002)
                        //{
                            //if (amount > balance)
                            //{
                            //    //buat draft email
                            //    decimal limit = amount - balance;
                            //    QueryExpression queryMatrixapprovalcredit = new QueryExpression("tss_matrixapprovalcreditlimit")
                            //    {
                            //        ColumnSet = new ColumnSet(true),
                            //        Criteria =
                            //        {
                            //            Conditions =
                            //            {
                            //                new ConditionExpression("tss_branch", ConditionOperator.Equal, PSS.GetAttributeValue<EntityReference>("businessunitid").Id),
                            //                new ConditionExpression("tss_end", ConditionOperator.GreaterEqual, Convert.ToInt32(limit)),
                            //                new ConditionExpression("tss_start", ConditionOperator.LessEqual, Convert.ToInt32(limit))
                            //            }
                            //        },
                            //        Orders =
                            //        {
                            //            new OrderExpression("tss_priorityno", OrderType.Ascending)
                            //        }
                            //    };
                            //    EntityCollection MatrixApprovals = organizationService.RetrieveMultiple(queryMatrixapprovalcredit);
                            //    Entity currApprover = MatrixApprovals.Entities.FirstOrDefault();
                            //    if (currApprover == null)
                            //    {
                            //        throw new InvalidWorkflowException("Approval credit limit not found!");
                            //    }
                            //    EntityReference currapproval = currApprover.GetAttributeValue<EntityReference>("tss_approver");
                            //    Guid receiver = currapproval.Id;
                            //    String receivername = currapproval.Name;
                            //    QueryExpression systemUserQuery = new QueryExpression("systemuser")
                            //    {
                            //        ColumnSet = new ColumnSet(true),
                            //        Criteria =
                            //        {
                            //            Conditions =
                            //            {
                            //                new ConditionExpression("fullname", ConditionOperator.Equal, "Admin CRM")
                            //            }
                            //        }
                            //    };

                            //    //Guid cc = _DL_systemuser.Select(organizationService, systemUserQuery).Entities[0].Id;
                            //    Guid sender = _DL_systemuser.Select(organizationService, systemUserQuery).Entities[0].Id;
                            //    trace.Trace("start create email creditlimit");
                            //    Entity email = CreateEmailSubmitSOtoSAP(sender, receiver, sender, getSOPartHeader, organizationService, trace, receivername);
                            //    organizationService.Create(email);

                            //    getSOPartHeader["tss_clcurrentapprover"] = currApprover.ToEntityReference();
                            //    getSOPartHeader["tss_cldatetime"] = DateTime.Now.ToLocalTime();

                            //    //update flag plafon below so limit set YES
                            //    getSOPartHeader["tss_plafondbelowsoamount"] = new OptionSetValue(865920001);

                            //    getSOPartHeader["tss_approvecreditlimit"] = false;


                            //    //update New Customer Sparepart
                            //    if (getSOPartHeader.Attributes.Contains("tss_customer"))
                            //    {
                            //        var context = new OrganizationServiceContext(organizationService);
                            //        var account = (from c in context.CreateQuery("account")
                            //                       where c.GetAttributeValue<Guid>("accountid") == getSOPartHeader.GetAttributeValue<EntityReference>("tss_customer").Id
                            //                       select c).ToList();
                            //        if (account.Count > 0)
                            //        {
                            //            Entity ent = new Entity("account");
                            //            ent.Id = getSOPartHeader.GetAttributeValue<EntityReference>("tss_customer").Id;
                            //            ent.Attributes["tss_newcustomersp"] = false;
                            //            organizationService.Update(ent);
                            //        }
                            //    }

                            //    organizationService.Update(getSOPartHeader);
                            //    trace.Trace("success update ");
                            //    //add records to approval list
                            //    foreach (var MatrixApproval in MatrixApprovals.Entities)
                            //    {
                            //        Entity addApprover = new Entity("tss_approverlist");
                            //        addApprover["tss_approver"] = MatrixApproval.GetAttributeValue<EntityReference>("tss_approver");
                            //        addApprover["tss_salesorderpartheaderid"] = getSOPartHeader.ToEntityReference();
                            //        addApprover["tss_type"] = new OptionSetValue(865920000);
                            //        Guid userId = MatrixApproval.GetAttributeValue<EntityReference>("tss_approver").Id;
                            //        Entity systemUser = _DL_systemuser.Select(organizationService, userId);
                            //        _ShareRecords = new ShareRecords();
                            //        //Share Grant Access - Shared Form to SO Part
                            //        _ShareRecords.ShareRecord(organizationService, getSOPartHeader, systemUser);
                            //        //Create Records
                            //        organizationService.Create(addApprover);
                            //    }
                            //    trace.Trace("Update approval list!");

                            //    #region Flag Plafond Under SO Amount
                            //    //Entity SOHeader = new Entity(_DL_tss_salesorderpartheader.EntityName);
                            //    //if (getSOPartHeader.Contains("tss_plafondbelowsoamount") && getSOPartHeader.Attributes["tss_plafondbelowsoamount"] != null)
                            //    //{
                            //    //    if (getSOPartHeader.GetAttributeValue<bool>("tss_plafondbelowsoamount") == true)
                            //    //        SOHeader["tss_plafondbelowsoamount"] = false;
                            //    //    else
                            //    //        SOHeader["tss_plafondbelowsoamount"] = true;
                            //    //}
                            //    //else
                            //    //    SOHeader["tss_plafondbelowsoamount"] = true;
                            //    //SOHeader.Id = id;
                            //    //organizationService.Update(SOHeader);
                            //    #endregion
                            //}
                            #endregion
                            //else //if balance >= so amount
                            //{
                    Price_List = "P5";//masih default
                    Price_Group = "P2";//masih default

                    //update flag plafon below so limit set FALSE

                    getSOPartHeader["tss_plafondbelowsoamount"] = new OptionSetValue(865920000);
                    organizationService.Update(getSOPartHeader);

                    trace.Trace("Get all to");
                    if (getSOPartHeader.Contains("tss_soldto"))
                    {
                        Entity soldto = _DL_account.Select(organizationService, getSOPartHeader.GetAttributeValue<EntityReference>("tss_soldto").Id);
                        if (soldto.Contains("accountnumber"))
                        {
                            Sold_To = soldto.GetAttributeValue<string>("accountnumber");
                            //int x = Sold_To.Length;
                            for (int x = Sold_To.Length; x < 10; x++)
                            {
                                Sold_To = "0" + Sold_To;
                            }
                        }
                        else
                            throw new InvalidWorkflowException("Customer " + getSOPartHeader.GetAttributeValue<EntityReference>("tss_soldto").Name + " account number is empty!");
                    }

                    if (getSOPartHeader.Contains("tss_billto"))
                    {
                        Entity billto = _DL_account.Select(organizationService, getSOPartHeader.GetAttributeValue<EntityReference>("tss_billto").Id);
                        if (billto.Contains("accountnumber"))
                        {
                            Bill_To = billto.GetAttributeValue<string>("accountnumber");
                            for (int x = Bill_To.Length; x < 10; x++)
                            {
                                Bill_To = "0" + Bill_To;
                            }
                        }
                        else
                            throw new InvalidWorkflowException("Customer " + getSOPartHeader.GetAttributeValue<EntityReference>("tss_billto").Name + " account number is empty!");
                    }

                    if (getSOPartHeader.Contains("tss_shipto"))
                    {
                        Entity shipto = _DL_account.Select(organizationService, getSOPartHeader.GetAttributeValue<EntityReference>("tss_shipto").Id);
                        if (shipto.Contains("accountnumber"))
                        {
                            Ship_To = shipto.GetAttributeValue<string>("accountnumber");
                            for (int x = Ship_To.Length; x < 10; x++)
                            {
                                Ship_To = "0" + Ship_To;
                            }
                        }
                        else
                            throw new InvalidWorkflowException("Customer " + getSOPartHeader.GetAttributeValue<EntityReference>("tss_shipto").Name + " account number is empty!");
                    }

                    #region set Create_so Value
                    trace.Trace("Starting create so value");
                    create_SO.SoCrmNo = CRM_SO_NO;
                    create_SO.CsrfToken = token;
                    create_SO.BillTo = Bill_To;//"0000106681";

                    if (getSOPartHeader.Contains("tss_billtocontact"))
                    {
                        Entity ContactBill = _DL_contact.Select(organizationService, getSOPartHeader.GetAttributeValue<EntityReference>("tss_billtocontact").Id);
                        create_SO.ContactBill = ContactBill.GetAttributeValue<string>("fullname");
                    }
                    else
                        create_SO.ContactBill = "";

                    if (getSOPartHeader.Contains("tss_shiptocontact"))
                    {
                        Entity ContactShip = _DL_contact.Select(organizationService, getSOPartHeader.GetAttributeValue<EntityReference>("tss_shiptocontact").Id);
                        create_SO.ContactShip = ContactShip.GetAttributeValue<string>("fullname");
                    }
                    else
                        create_SO.ContactShip = "";

                    if (getSOPartHeader.Contains("tss_pss"))
                    {
                        Entity ContactSls = _DL_systemuser.Select(organizationService, getSOPartHeader.GetAttributeValue<EntityReference>("tss_pss").Id);
                        create_SO.ContactSls = ContactSls.GetAttributeValue<string>("fullname");
                    }
                    else
                        create_SO.ContactSls = "";

                    if (getSOPartHeader.Contains("tss_soldtocontact"))
                    {
                        Entity ContactSold = _DL_contact.Select(organizationService, getSOPartHeader.GetAttributeValue<EntityReference>("tss_soldtocontact").Id);
                        create_SO.ContactSold = ContactSold.GetAttributeValue<string>("fullname");
                    }
                    else
                        create_SO.ContactSold = "";

                    create_SO.Currency = Currency;//"IDR";
                    create_SO.DistrChan = Dis_Ch;//"01";
                    create_SO.Division = Division;//"B1";
                    create_SO.DocType = order_type;//"ZPSE";
                    create_SO.PackName = Package_Name;//"Package test";
                    create_SO.PackPrice = Package_Price;//"55000020";
                    create_SO.PackSlsunit = Sales_Unit_Package;//"PC";
                    create_SO.PackQty = Package_Qty.ToString();//"1";
                    create_SO.PackType = Package_Type;//"PACK1";
                    create_SO.PartnNumb = Customer_Code;// "0000106681"; //customer number
                    create_SO.Plant = Sales_Off; // Deliver plant == sales office
                    create_SO.Pmnttrms = Payment_Term;// "D30";
                    create_SO.PriceGrp = Price_Group;// "P2"; // part non government
                    create_SO.PriceList = Price_List;// "P5"; //price list 5
                    create_SO.PurchNoC = PO_NO_Cust;// "PLB/094/F/XI/TN/2016";
                    create_SO.Salesman = Salesman;// "1412003";//default 9999999
                    create_SO.SalesOff = Sales_Off;// "A014";s
                    create_SO.SalesOrg = Sales_Org;// "A000";[
                    create_SO.ShipTo = Ship_To;// "0000106681";//account number (TN)
                    create_SO.SoldTo = Sold_To;// "0000106681";//account number (TN)
                    create_SO.PurchDate = PO_Date_Cust.ToString("yyyy-MM-dd");//"2018-01-02";
                    create_SO.ReqDateH = Req_Delivery_Date.ToString("yyyy-MM-dd");// "2018-01-02";

                    trace.Trace("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}\n{9}\n{10}\n{11}\n{12}\n{13}\n{14}\n{15}\n{16}\n{17}\n{18}\n{19}\n{20}\n{21}\n{22}\n{23}\n{24}",
                        create_SO.SoCrmNo, create_SO.CsrfToken, create_SO.BillTo, create_SO.Currency, create_SO.DistrChan, create_SO.Division,
                        create_SO.DocType, create_SO.PackName, create_SO.PackPrice, create_SO.PackSlsunit, create_SO.PackQty, create_SO.PackType,
                        create_SO.PartnNumb, create_SO.Plant, create_SO.Pmnttrms, create_SO.PriceGrp, create_SO.PriceList, create_SO.PurchNoC,
                        create_SO.Salesman, create_SO.SalesOff, create_SO.SalesOrg, create_SO.ShipTo, create_SO.SoldTo, create_SO.PurchDate, create_SO.ReqDateH);
                    #endregion

                    #region set create SO details value
                    QueryExpression querygetSOpartlines = new QueryExpression("tss_sopartlines");
                    querygetSOpartlines.ColumnSet = new ColumnSet(true);
                    querygetSOpartlines.Criteria.AddCondition("tss_sopartheaderid", ConditionOperator.Equal, id);
                    trace.Trace("Retrieving all SOPartLines");

                    EntityCollection SOPartLines = organizationService.RetrieveMultiple(querygetSOpartlines);
                    string parametersline = string.Empty;
                    if (SOPartLines.Entities.Count > 0)
                    {
                        foreach (var SOPartline in SOPartLines.Entities)
                        {
                            //parameter Sales Order Details
                            ZstrDetailSo detail = new ZstrDetailSo();
                            string Material_ID, Sales_Unit, Item_Category, Item_Number;
                            decimal Price_Amount = 0;
                            int Quantity = 0;

                            Material_ID = string.Empty;
                            Sales_Unit = string.Empty;
                            Item_Category = string.Empty;
                            Item_Number = "000010";
                            //default isi ini.
                            Material_ID = SOPartline.GetAttributeValue<EntityReference>("tss_partnumber").Name;

                            if (SOPartline.Contains("tss_isinterchange"))
                            {
                                //jika isinterchange == true maka material number yang diisi itu Part number interchange nya.
                                if (SOPartline.GetAttributeValue<bool>("tss_isinterchange"))
                                {
                                    if (SOPartline.Contains("tss_partnumberinterchange"))
                                    {
                                        EntityReference interchange = SOPartline.GetAttributeValue<EntityReference>("tss_partnumberinterchange");
                                        Entity partInterchange = organizationService.Retrieve("tss_partmasterlinesinterchange", interchange.Id, new ColumnSet(true));
                                        EntityReference partnumberInterchange = partInterchange.GetAttributeValue<EntityReference>("tss_partnumberinterchange");

                                            #region Update [30-08-2018] ada update di SAP, jadi walaupun interchange tetap kirim parent part numbernya
                                            //Material_ID = partnumberInterchange.Name;
                                            Material_ID = SOPartline.GetAttributeValue<EntityReference>("tss_partnumber").Name;
                                            #endregion
                                        }
                                    else
                                    {
                                        throw new Exception("Field Partnumberinterchange in Sales Order Part Line is empty!");
                                    }
                                }
                            }
                            trace.Trace("Get SO part line: " + Material_ID);

                            if (UserTitle == USER_TITLE_COUNTER)
                            {
                                if (SOPartline.Contains("tss_priceafterdiscount"))
                                    Price_Amount = SOPartline.GetAttributeValue<Money>("tss_priceafterdiscount").Value;
                            }
                            else
                            {
                                if (SOPartline.Contains("tss_finalprice"))
                                    Price_Amount = SOPartline.GetAttributeValue<Money>("tss_finalprice").Value;
                            }
                            trace.Trace("Get price amount: " + Price_Amount.ToString());

                            if (SOPartline.Contains("tss_qtyrequest"))
                                Quantity = SOPartline.GetAttributeValue<int>("tss_qtyrequest");
                            trace.Trace("Get qty request: " + Price_Amount.ToString());

                            if (SOPartline.Contains("tss_unit"))
                                Sales_Unit = SOPartline.GetAttributeValue<EntityReference>("tss_unit").Name;
                            else
                                Sales_Unit = "PC";
                            trace.Trace("Get sales unit: " + Sales_Unit);

                            if (SOPartline.Contains("tss_itemnumber"))
                            {
                                Item_Number = SOPartline.GetAttributeValue<int>("tss_itemnumber").ToString();
                                for (int x = Item_Number.Length; x < 6; x++)
                                {
                                    Item_Number = "0" + Item_Number;
                                }
                                trace.Trace("Get sales unit: " + Sales_Unit);
                            }

                            detail.ItmNumber = Item_Number;
                            detail.Material = Material_ID;
                            detail.TargetQty = Quantity; //0
                            detail.SalesUnit = Sales_Unit; //PC
                            detail.CondValue = Price_Amount;
                            //detail.Currency = "IDR";//parameter field tidak terkirim di parameter Price_Amount;

                            if (string.IsNullOrEmpty(Item_Category))
                                detail.ItemCateg = "ZPST"; //ZPST - Akan didiskusikan di internal Traknus untuk menentukan Logicnya
                            trace.Trace("add details parameter");
                            details.Add(detail);

                            trace.Trace("input parametersline");
                            parametersline = parametersline + string.Format("SO Line Parameters: {0} {1} {2} {3} {4} {5};", Item_Number, Material_ID, Quantity, Sales_Unit, Price_Amount, "ZPST");
                        }

                        create_SO.DetailSo = details.ToArray();
                        trace.Trace("Detail SO is not null.");
                    }
                    else
                    {
                        create_SO.DetailSo = null;
                        throw new InvalidWorkflowException("SO part line is empty. Please add SO part line first!");
                    }
                    #endregion

                    trace.Trace("getting clientsection.");
                    //ClientSection clientSection = ConfigurationManager.GetSection("system.serviceModel/client") as ClientSection;
                    //var CHendpoint = clientSection.Endpoints[0];

                    string _webServiceUrl = Partsetup.GetAttributeValue<string>("tss_sapwebservice");
                    EndpointAddress remoteAddress = new EndpointAddress(_webServiceUrl);
                    BasicHttpBinding httpbinding = new System.ServiceModel.BasicHttpBinding();
                    httpbinding.Name = "ZWEB_SERVICE_CRM";
                    httpbinding.MessageEncoding = WSMessageEncoding.Mtom;
                    httpbinding.TextEncoding = Encoding.UTF8;
                    //httpbinding.SendTimeout = new TimeSpan(0, 10, 0);
                    httpbinding.MaxReceivedMessageSize = Int32.MaxValue; //[10 Feb 2020] Added by Santony due to error "The maximum message size quota for incoming messages (65536) has been exceeded. To increase the quota, use the MaxReceivedMessageSize property on the appropriate binding element"
                    httpbinding.MaxBufferSize = Int32.MaxValue; //[10 Feb 2020] Added by Santony due to error "The maximum message size quota for incoming messages (65536) has been exceeded. To increase the quota, use the MaxReceivedMessageSize property on the appropriate binding element"

                    trace.Trace("creating services client.");

                    ZWEB_SERVICES_CRMClient client = new ZWEB_SERVICES_CRMClient(httpbinding, remoteAddress);
                    //(CHendpoint.Name, CHendpoint.Address.AbsoluteUri.Replace("tnsapdtn", "192.168.0.67"));
                    client.ChannelFactory.Credentials.HttpDigest.ClientCredential.UserName = "FU-CRMSALES";
                    client.ChannelFactory.Credentials.HttpDigest.ClientCredential.UserName = SAPPassword;
                    //client.ChannelFactory.Credentials.HttpDigest.ClientCredential.UserName = "1nd()n3$*A";

                    client.ClientCredentials.UserName.UserName = "FU-CRMSALES";
                    client.ClientCredentials.UserName.Password = SAPPassword;
                    //client.ClientCredentials.UserName.Password = "1nd()n3$*A";
                    client.Endpoint.Binding.SendTimeout = new TimeSpan(0, 10, 0);

                    try
                    {
                        //remark off sap lines 784 - 983
                                    
                        trace.Trace("open client webservice.");
                        client.Open();

                        ZcrmCreateSoResponse response = client.ZcrmCreateSo(create_SO);

                        DateTime syncdate = DateTime.Parse(response.SyncDate);
                        DateTime synctime = response.SyncTime;
                        syncdate = syncdate.AddHours(syncdate.Hour);
                        syncdate = syncdate.AddMinutes(syncdate.Minute);
                        syncdate = syncdate.AddSeconds(syncdate.Second);

                        trace.Trace("Response Result: " + response.Result);
                        trace.Trace("Response Description: " + response.Description);
                        trace.Trace("Response Sync Date & Time: " + syncdate.ToString());

                        if (string.Equals(response.Result, "success"))
                        {
                            //update SAP SO number in CRM.
                            #region update SAP SO Number in CRM.
                            string SO_NO_SAP = response.Description;
                            //bool iszero = true;

                            //while (iszero)
                            //{
                            //    if (SO_NO_SAP.First() == '0')
                            //        SO_NO_SAP = SO_NO_SAP.Remove(0, 1);
                            //    else
                            //        iszero = false;
                            //}

                            getSOPartHeader["tss_sapsoid"] = response.Description;
                            getSOPartHeader["tss_statecode"] = new OptionSetValue(STATECODE_SOPROCESSEDTOSAP);
                            getSOPartHeader["tss_statusreason"] = new OptionSetValue(STATUSREASON_SUBMITTED);
                                    
                            organizationService.Update(getSOPartHeader);
                            #endregion

                            #region Update Flag New Customer in Account to No After Success Submit SO to SAP
                            //customer["tss_newcustomersp"] = false;
                            //organizationService.Update(customer); 
                            #endregion

                            ZstrResultDo[] DOs = response.ResultDo;
                            #region create SO sublines
                            foreach (ZstrResultDo DO in DOs)
                            {
                                string PartNumber = DO.Matrl;
                                string PartBranch = DO.Brnch;

                                trace.Trace("Delivery Number : " + DO.Delno);
                                trace.Trace("Delivery Qty : " + DO.Delqt.ToString());
                                trace.Trace("SO Number : " + DO.Sonum);
                                trace.Trace("Part Number : " + PartNumber);
                                trace.Trace("Part Branch : " + PartBranch);

                                //_mwsLog.Write(MethodBase.GetCurrentMethod().Name, "Delivery Number : " + DO.Delno + _mwsLog.ColumnsSeparator
                                //    + "Delivery Qty : " + DO.Delqt.ToString() + _mwsLog.ColumnsSeparator
                                //    + "SO Number : " + DO.Sonum + _mwsLog.ColumnsSeparator
                                //    + "Part Number : " + PartNumber + _mwsLog.ColumnsSeparator
                                //    + "Part Branch : " + PartBranch + _mwsLog.ColumnsSeparator
                                //    + response.Result + " at " + syncdate.ToString(), MWSLog.LogType.Information, MWSLog.Source.Inbound);

                                QueryExpression queryGetPart = new QueryExpression("trs_masterpart");
                                queryGetPart.ColumnSet = new ColumnSet(true);
                                queryGetPart.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("trs_name", ConditionOperator.Equal, PartNumber),
                                    }
                                };
                                Entity part = organizationService.RetrieveMultiple(queryGetPart).Entities[0];

                                QueryExpression queryGetBranch = new QueryExpression("businessunit");
                                queryGetBranch.ColumnSet = new ColumnSet(true);
                                queryGetBranch.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("trs_branchcode", ConditionOperator.Equal, PartBranch),
                                        new ConditionExpression("name", ConditionOperator.Equal, PartBranch)
                                    }
                                };
                                Entity Branch = organizationService.RetrieveMultiple(queryGetBranch).Entities.FirstOrDefault();

                                if (Branch == null)
                                    throw new InvalidWorkflowException("Branch " + PartBranch + " from return SAP webservice not found.");

                                QueryExpression queryGetPartlinesInter = new QueryExpression("tss_partmasterlinesinterchange");
                                queryGetPartlinesInter.ColumnSet = new ColumnSet(true);
                                queryGetPartlinesInter.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("tss_partnumberinterchange", ConditionOperator.Equal, part.Id),
                                        //new ConditionExpression("tss_status", ConditionOperator.Equal, PartInterchange_STATUS_ACTIVE)//status = active //05-02-2018 [Comment by Santony] - still can use part number even part number status not active
                                    }
                                };
                                EntityCollection PartlineInters = organizationService.RetrieveMultiple(queryGetPartlinesInter);
                                List<Entity> SOLines = new List<Entity>();
                                Entity SO_Line = new Entity();

                                if (PartlineInters.Entities.Count > 0)
                                {
                                    SOLines = SOPartLines.Entities.Where(o => o.GetAttributeValue<bool>("tss_isinterchange") == true && o.GetAttributeValue<EntityReference>("tss_partnumberinterchange").Id == PartlineInters.Entities[0].Id).ToList();
                                    SO_Line = SOLines.FirstOrDefault();
                                    if (SO_Line == null)
                                        SO_Line = SOPartLines.Entities.Where(o => o.GetAttributeValue<EntityReference>("tss_partnumber").Id == part.Id).FirstOrDefault();
                                }
                                else
                                {
                                    SOLines = SOPartLines.Entities.Where(o => o.GetAttributeValue<EntityReference>("tss_partnumber").Id == part.Id).ToList();
                                    SO_Line = SOLines.FirstOrDefault();
                                }

                                if (SO_Line != null)
                                {
                                    #region create SO subline with DO
                                    Entity SO_Subline = new Entity("tss_salesorderpartsublines");
                                    SO_Subline["tss_salesorderpartlines"] = SO_Line.ToEntityReference();
                                    SO_Subline["tss_qtyavailablebranch"] = Branch.ToEntityReference();
                                    SO_Subline["tss_name"] = PartNumber;
                                    SO_Subline["tss_deliveryno"] = DO.Delno;
                                    SO_Subline["tss_deliveryqty"] = Convert.ToInt32(DO.Delqt);
                                    SO_Subline["tss_transactiontype"] = new OptionSetValue(SUBLINE_TRANSCTTYPE_CREATEDO);
                                    organizationService.Create(SO_Subline);
                                    #endregion
                                }
                                else
                                {
                                    //throw new Exception("SO line not found!");
                                }
                            }
                            #endregion

                            //update Quotation Service - STATUS ASSIGN QUO to SO SUBMITTED TO SAP jika SourceType == Service
                            int sourcetypeValue = getSOPartHeader.GetAttributeValue<OptionSetValue>("tss_sourcetype").Value;
                            if (sourcetypeValue == SOURCETYPE_SERVICE)
                            {
                                string quotationId = getSOPartHeader.GetAttributeValue<string>("tss_quotationlink");

                                QueryExpression getQuotPartHeaderQuery = new QueryExpression("tss_quotationpartheader")
                                {
                                    ColumnSet = new ColumnSet(true),
                                    Criteria =
                                    {
                                        Conditions =
                                        {
                                            new ConditionExpression("tss_quotationid", ConditionOperator.Equal, quotationId)
                                        }
                                    }
                                };

                                //get quotation part
                                Entity getQuotPartHeader = _DL_tss_quotationpartheader.Select(organizationService, getQuotPartHeaderQuery).Entities[0];
                                EntityReference serviceRef = getQuotPartHeader.GetAttributeValue<EntityReference>("tss_quotationserviceno");

                                //get quotation service and update status assign quo
                                Entity quotationService = _DL_trs_quotation.Select(organizationService, serviceRef.Id);
                                _DL_trs_quotation.tss_statusassignquo = QUOTATIONSTATUSASSIGNQUO_SOSUBMITTEDTOSAP;
                                _DL_trs_quotation.UpdateStatusAssignQuo(organizationService, quotationService.Id);

                                Guid quotationEntityId = getQuotPartHeader.Attributes.Contains("tss_quotationserviceno") ? getQuotPartHeader.GetAttributeValue<EntityReference>("tss_quotationserviceno").Id : Guid.Empty;
                                if (quotationEntityId != Guid.Empty)
                                {
                                    Entity quotationEn = new Entity("trs_quotation");
                                    quotationEn["trs_quotationid"] = quotationEntityId;
                                    quotationEn["tss_statusassignquo"] = new OptionSetValue(865920005); //so submit sap
                                    organizationService.Update(quotationEn);
                                }
                            }
                            string paramsheaderdetail = string.Format("SO Header Parameters: {0}+{1}+{2}+{3}+{4}+{5}+{6}+{7}+{8}+{9}+{10}+{11}+{12}+{13}+{14}+{15}+{16}+{17}+{18}+{19}+{20}+{21}+{22}+{23}+{24}+{25}",
                                CRM_SO_NO, token, order_type, Sales_Org, Dis_Ch, 
                                Division, Sales_Off, Customer_Code, PO_NO_Cust, Payment_Term, 
                                Currency, Delivery_Plant, Package_Type, Package_Name, Package_NO,
                                Package_Price, Sales_Unit_Package, Salesman, Price_List, Price_Group,
                                Sold_To, Bill_To, Ship_To, PO_Date_Cust.ToString("yyyy-MM-dd"), Req_Delivery_Date.ToString("yyyy-MM-dd"), SO_NO_SAP);

                            _mwsLog.Write(MethodBase.GetCurrentMethod().Name, paramsheaderdetail + _mwsLog.ColumnsSeparator + parametersline
                                + response.Result + " at " + syncdate.ToString(), MWSLog.LogType.Information, MWSLog.Source.Outbound);
                        }
                        else
                        {
                            string paramsheaderdetail = string.Format("SO Header Parameters: {0}+{1}+{2}+{3}+{4}+{5}+{6}+{7}+{8}+{9}+{10}+{11}+{12}+{13}+{14}+{15}+{16}+{17}+{18}+{19}+{20}+{21}+{22}+{23}+{24}",
                                CRM_SO_NO, token, order_type, Sales_Org, Dis_Ch,
                                Division, Sales_Off, Customer_Code, PO_NO_Cust, Payment_Term,
                                Currency, Delivery_Plant, Package_Type, Package_Name, Package_NO,
                                Package_Price, Sales_Unit_Package, Salesman, Price_List, Price_Group,
                                Sold_To, Bill_To, Ship_To, PO_Date_Cust.ToString("yyyy-MM-dd"), Req_Delivery_Date.ToString("yyyy-MM-dd"));

                            _mwsLog.Write(MethodBase.GetCurrentMethod().Name, paramsheaderdetail + _mwsLog.ColumnsSeparator + parametersline + _mwsLog.ColumnsSeparator
                                + "Failed to Create Sales Order on SAP. Details : " + response.Description, MWSLog.LogType.Error, MWSLog.Source.Outbound);
                            throw new InvalidWorkflowException(string.Format("Result: {0} Details: {1}", response.Result, response.Description));
                        }
                                    
                    }
                    catch (Exception ex)
                    {
                        string paramsheaderdetail = string.Format("SO Header Parameters: {0}+{1}+{2}+{3}+{4}+{5}+{6}+{7}+{8}+{9}+{10}+{11}+{12}+{13}+{14}+{15}+{16}+{17}+{18}+{19}+{20}+{21}+{22}+{23}+{24}",
                                CRM_SO_NO, token, order_type, Sales_Org, Dis_Ch,
                                Division, Sales_Off, Customer_Code, PO_NO_Cust, Payment_Term,
                                Currency, Delivery_Plant, Package_Type, Package_Name, Package_NO,
                                Package_Price, Sales_Unit_Package, Salesman, Price_List, Price_Group,
                                Sold_To, Bill_To, Ship_To, PO_Date_Cust.ToString("yyyy-MM-dd"), Req_Delivery_Date.ToString("yyyy-MM-dd"));

                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, paramsheaderdetail + _mwsLog.ColumnsSeparator + parametersline + _mwsLog.ColumnsSeparator
                            + "Failed to Create Sales Order on SAP. Details : " + ex.Message, MWSLog.LogType.Error, MWSLog.Source.Outbound);

                        trace.Trace("Failed Time: " + DateTime.Now.ToString());
                        client.Abort();
                        client.Close();
                        throw new Exception("Error detail : " + ex.Message);
                    }
                    finally
                    {
                        client.Close();
                    }
                    #region Unused [15/10/2018]
                            //}
                        //}
                    //}
                    //else
                    //{
                    //    throw new InvalidWorkflowException("Balance AR Sparepart or Plafond AR Sparepart in customer is empty!");
                    //}
                    #endregion
                }
                else
                {
                    throw new InvalidWorkflowException("CRM SO Number is null/empty!");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".submitSOtoSAP_OnClick : " + ex.Message.ToString());
            }
        }

        private void CancelSalesOrder_SAP(IOrganizationService organizationService, ITracingService trace,string SO_NO_SAP)
        {
            ZWEB_SERVICES_CRMClient client = null;

            trace.Trace("creating services client");
            try
            {
                //input value to parameters.
                QueryExpression queryPartsetup = new QueryExpression("tss_sparepartsetup");
                queryPartsetup.ColumnSet = new ColumnSet(true);
                queryPartsetup.Criteria.AddCondition("tss_name", ConditionOperator.Equal, "TSS");

                Entity partsetup = organizationService.RetrieveMultiple(queryPartsetup).Entities[0];
                string uniqueKey = partsetup.GetAttributeValue<string>("tss_sapintegrationuniquekey");
                string SAPPassword = partsetup.GetAttributeValue<string>("tss_sapwebservicepassword");

                ZcrmCancelSo cancelSO = new ZcrmCancelSo(); //harusnya ZcrmCancelSo nunggu update dari wahyu.
                cancelSO.CsrfToken = serviceEncryption.EncryptText(SO_NO_SAP, uniqueKey);
                cancelSO.Salesdocument = SO_NO_SAP;

                string webServiceUrl = partsetup.GetAttributeValue<string>("tss_sapwebservice");
                EndpointAddress remoteAddress = new EndpointAddress(webServiceUrl);
                BasicHttpBinding httpbinding = new BasicHttpBinding
                {
                    Name = "ZWEB_SERVICE_CRM",
                    MessageEncoding = WSMessageEncoding.Mtom,
                    TextEncoding = Encoding.UTF8,
                    MaxReceivedMessageSize = Int32.MaxValue, //[10 Feb 2020] Added by Santony due to error "The maximum message size quota for incoming messages (65536) has been exceeded. To increase the quota, use the MaxReceivedMessageSize property on the appropriate binding element"
                    MaxBufferSize = Int32.MaxValue //[10 Feb 2020] Added by Santony due to error "The maximum message size quota for incoming messages (65536) has been exceeded. To increase the quota, use the MaxReceivedMessageSize property on the appropriate binding element"
                };
                client = new ZWEB_SERVICES_CRMClient(httpbinding, remoteAddress);
                //(CHendpoint.Name, CHendpoint.Address.AbsoluteUri.Replace("tnsapdtn", "192.168.0.67"));
                client.ChannelFactory.Credentials.HttpDigest.ClientCredential.UserName = "FU-CRMSALES";
                client.ChannelFactory.Credentials.HttpDigest.ClientCredential.UserName = SAPPassword;
                //client.ChannelFactory.Credentials.HttpDigest.ClientCredential.UserName = "1nd()n3$*A";

                client.ClientCredentials.UserName.UserName = "FU-CRMSALES";
                client.ClientCredentials.UserName.Password = SAPPassword;
                //client.ClientCredentials.UserName.Password = "1nd()n3$*A";

                trace.Trace("creating services client.");
                client.Open();

                ZcrmCancelSoResponse response = client.ZcrmCancelSo(cancelSO);

                DateTime syncdate = DateTime.Parse(response.SyncDate);
                DateTime synctime = response.SyncTime;
                syncdate = syncdate.AddHours(syncdate.Hour);
                syncdate = syncdate.AddMinutes(syncdate.Minute);
                syncdate = syncdate.AddSeconds(syncdate.Second);

                trace.Trace("Response Result: " + response.Result);
                trace.Trace("Response Description: " + response.Description);
                trace.Trace("Response Sync Date & Time: " + response.SyncDate + " " + response.SyncTime);

                if (string.Equals(response.Result, "success"))
                {
                    string parameters = string.Format("SAP SO Number: {0};Token: {1}", cancelSO.Salesdocument,
                        cancelSO.CsrfToken);
                    _mwsLog.Write(MethodBase.GetCurrentMethod().Name, parameters + _mwsLog.ColumnsSeparator
                                                                                 + response.Result + " at " +
                                                                                 syncdate.ToString(),
                        MWSLog.LogType.Information, MWSLog.Source.Outbound);
                }
                else
                {
                    string parameters = string.Format("SAP SO Number: {0};Token: {1}", cancelSO.Salesdocument,
                        cancelSO.CsrfToken);
                    _mwsLog.Write(MethodBase.GetCurrentMethod().Name, parameters + _mwsLog.ColumnsSeparator +
                                                                      "Failed to Cancel Sales Order on SAP for Sales Order Number : "
                                                                      + SO_NO_SAP + ". Details :  " +
                                                                      response.Description, MWSLog.LogType.Error,
                        MWSLog.Source.Outbound);
                    throw new Exception("Failed to Cancel Sales Order on SAP for Sales Order Number : " + SO_NO_SAP +
                                        ". Details :  " + response.Description);
                }
            }
            catch (Exception ex)
            {
                trace.Trace("Failed Time: " + DateTime.Now.ToString());
                if (client != null)
                {
                    client.Abort();
                    client.Close();
                }

                throw new Exception("Error details : " + ex.Message);
            }
            finally
            {
                client.Close();
            }
        }

        private string EntityDump(Entity entity)
        {
            StringBuilder str = new StringBuilder();

            str.AppendLine("==================================");
            str.AppendLine("Logical Name : " + entity.LogicalName);
            str.AppendLine("Entity Id    : " + entity.Id);
            str.AppendLine("==================================");
            str.AppendLine("Attributes :");
            str.AppendLine("==================================");
            foreach (var entityAttribute in entity.Attributes)
            {

                var value = entityAttribute.Value;

                if (value is EntityReference)
                {
                    var entityReference = value as EntityReference;
                    str.AppendLine("##############");
                    str.AppendLine("Entity Reference");
                    str.AppendLine("Attribute Name : " + entityAttribute.Key);
                    str.AppendLine("Logical Name : " + entityReference.LogicalName);
                    str.AppendLine("Id : " + entityReference.Id);
                }
                else

                if (value is OptionSetValue)
                {
                    var optionSetValue = value as OptionSetValue;
                    str.AppendLine("##############");
                    str.AppendLine(entityAttribute.Key + " : OptionSet " + optionSetValue.Value);
                }
                else

                if (value is Money)
                {
                    str.AppendLine("##############");
                    str.AppendLine(entityAttribute.Key + " : Money " + (value as Money).Value);
                }
                else
                {
                    str.AppendLine("##############");
                    str.AppendLine(String.Format("{0} : {1}", entityAttribute.Key, value));
                }
            }
            str.AppendLine("##############");
            str.AppendLine("==================================");
            return str.ToString();
        }


        public bool isTOConfirmFromSOSubLines(IOrganizationService organizationService, Guid id)
        {
            bool isTOConfirm = false;

            //get all lines from so header
            QueryExpression qeLine = new QueryExpression("tss_sopartlines")
            {
                ColumnSet = new ColumnSet(true),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("tss_sopartheaderid",ConditionOperator.Equal,id)
                    }
                }
            };
            EntityCollection ecLines = organizationService.RetrieveMultiple(qeLine);

            //each lines get all sub lines and search TO Confirm status
            if (ecLines.Entities.Count > 0)
            {
                foreach (Entity eLine in ecLines.Entities)
                {
                    QueryExpression qeSubLine = new QueryExpression("tss_salesorderpartsublines")
                    {
                        ColumnSet = new ColumnSet(true),
                        Criteria = new FilterExpression
                        {
                            Conditions =
                            {
                                new ConditionExpression("tss_salesorderpartlines",ConditionOperator.Equal,eLine.Id)
                            }
                        }
                    };
                    EntityCollection ecSubLines = organizationService.RetrieveMultiple(qeSubLine);
                    if (ecSubLines.Entities.Count > 0)
                    {
                        foreach (Entity enSubLine in ecSubLines.Entities)
                        {
                            if (enSubLine.GetAttributeValue<OptionSetValue>("tss_transactiontype").Value == 865920005)
                            {
                                isTOConfirm = true;
                            }
                        }
                    }

                }
            }

            return isTOConfirm;
        }

        public void UpdateStatus_SalesOrder(IOrganizationService organizationService, Guid id, string entityName, ITracingService trace)
        {
            try
            {
                var sopartheader = organizationService.Retrieve(entityName, id, new ColumnSet(true));

                if (sopartheader.Attributes.Contains("tss_statusreason"))
                {
                    //get SO number SAP
                    string SO_NO_SAP = sopartheader.GetAttributeValue<string>("tss_sapsoid");
                    bool IsNotAllowCancel = sopartheader.GetAttributeValue<bool>("tss_isallowcancel");

                    //search so sub lines with TO Confirm Yes
                    bool isTOConfirm = isTOConfirmFromSOSubLines(organizationService, id);

                    if (!string.IsNullOrWhiteSpace(SO_NO_SAP) && !IsNotAllowCancel && !isTOConfirm)
                        {
                        //Cancel here
                        CancelSalesOrder_SAP(organizationService, trace, SO_NO_SAP); // If fail, auto throw exception.
                    }

                    #region success
                    //Update Status on Sales Order
                    _DL_tss_salesorderpartheader.tss_statuscode = STATECODE_CANCEL;
                    _DL_tss_salesorderpartheader.tss_statusreason = STATUSREASON_CLOSED;
                    _DL_tss_salesorderpartheader.UpdateStatusSO(organizationService, id);

                    //Check Quotation Link if exist then update status on Quotation Part Header
                    Guid quotID = Guid.Empty;
                    EntityReference erQuotation = sopartheader.GetAttributeValue<EntityReference>("tss_quotationlink");
                    

                    if (erQuotation != null)
                    {
                        quotID = erQuotation.Id;
                        //Update Status Reason & Quotation Status after Create Sales Order Successfully
                        _DL_tss_quotationpartheader.tss_quotationstatus = QUOTATIONSTATUS_CLOSED;
                        _DL_tss_quotationpartheader.tss_statusreason = QUOTATIONSTATUSREASON_LOST;
                        _DL_tss_quotationpartheader.UpdateStatusAfterCreateSO(organizationService, quotID);

                        //Check ProspectLink on Quotation Part
                        Entity quotation = _DL_tss_quotationpartheader.Select(organizationService, quotID);
                        EntityReference erProspect = quotation.GetAttributeValue<EntityReference>("tss_prospectlink");
                        if (erProspect != null)
                        {
                            Guid prosId = erProspect.Id;
                            //Update Status Reason & Pipeline Phase on Prospect Part Header after create Sales Order Part Header
                            _DL_tss_prospectpartheader.tss_statusreason = PROSPECTSTATUSREASON_LOST;
                            _DL_tss_prospectpartheader.UpdateStatusReasonAndPipeline(organizationService, prosId);
                        }
                    }

                    #endregion
                    //rollback stats.

                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".UpdateStatus_SalesOrder : " + ex.Message.ToString());
            }
        }

        private Entity CreateEmailSubmitSOtoSAP(Guid senderGuid, Guid receiverGuid, Guid ccGuid, Entity configurationEntity, IOrganizationService organizationService, ITracingService trace, string receiverName)
        {
            try
            {
                trace.Trace("Start create email");
                String strPathAndQuery = HttpContext.Current.Request.Url.PathAndQuery;
                string url = HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");
                url += "TraktorNusantara";
                HelperFunction help = new HelperFunction();
                
                string objecttypecode = string.Empty;
                help.GetObjectTypeCode(organizationService, EntityName, out objecttypecode);

                string SONumber = configurationEntity.GetAttributeValue<string>("tss_sonumber");

                var subject = @"Waiting Approval Credit Limit on Sales Order Part with SO Number " + SONumber;
                string row = string.Empty;
                var bodyTemplate = @"Dear Mr/Ms " + receiverName + @",<br/><br/>
                                Please approve credit limit in Sales Order Part";
                bodyTemplate += @".<br/><br/>CRM URL : <a href='" + url + "/main.aspx?etc=" + objecttypecode + "&id=%7b" + configurationEntity.Id.ToString() + "%7d&pagetype=entityrecord'>Click here</a>";
                bodyTemplate += @".<br/><br/>
                                Thank you,<br/><br/>
                                Admin CRM";
                
                var emailAgent = new Helper.EmailAgent();
                var emailDescription = bodyTemplate;
                var emailFactory = new Helper.EmailFactory();
                
                emailFactory.SetFrom(emailAgent.CreateActivityParty(new List<Tuple<Guid, string>>
                {
                    new Tuple<Guid, string>(senderGuid,"systemuser")
                }));
                emailFactory.SetTo(emailAgent.CreateActivityParty(new List<Tuple<Guid, string>>
                {
                    new Tuple<Guid, string>(receiverGuid,"systemuser")
                }));
                emailFactory.SetCC(emailAgent.CreateActivityParty(new List<Tuple<Guid, string>>
                {
                    new Tuple<Guid, string>(ccGuid,"systemuser")
                }));
                emailFactory.SetSubject(subject);
                emailFactory.SetContent(emailDescription);
                trace.Trace("finish create email");

                return emailFactory.Create();
            }
            catch (Exception ex)
            {
                throw new Exception("Create Email Failed. Technical Details :\r\n" + ex.ToString());
            }
        }
    }
}
