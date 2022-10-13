using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using TrakNusSparepartSystem.DataLayer;
using TrakNusSparepartSystem.Workflow.Helper;
using TrakNusSparepartSystem.Workflow.WebserviceSAP;
using System.ServiceModel;
using System.Activities;
using System.Web;
using Microsoft.Xrm.Sdk.Client;

namespace TrakNusSparepartSystem.Workflow.BusinessLayer
{
    public class BL_tss_quotationpartheader
    {
        #region Dependencies
        private DL_tss_quotationpartheader _DL_tss_quotationpartheader = new DL_tss_quotationpartheader();
        private DL_tss_quotationpartlines _DL_tss_quotationpartlines = new DL_tss_quotationpartlines();
        private DL_tss_salesorderpartheader _DL_tss_salesorderpartheader = new DL_tss_salesorderpartheader();
        private DL_account _DL_account = new DL_account();
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        private DL_businessunit _DL_businessunit = new DL_businessunit();
        private DL_tss_prospectpartheader _DL_tss_prospectpartheader = new DL_tss_prospectpartheader();
        private DL_trs_quotation _DL_trs_quotation = new DL_trs_quotation();
        private DL_contact _DL_contact = new DL_contact();
        private DL_tss_salesorderpartlines _DL_tss_salesorderpartlines = new DL_tss_salesorderpartlines();
        private Service serviceEncryption = new Service();
        private MWSLog _mwsLog = new MWSLog();
        private DL_tss_salesorderpartlinesservice _DL_tss_salesorderpartlinesservice = new DL_tss_salesorderpartlinesservice();
        private DL_tss_quotationparthistorypackage _DL_tss_quotationparthistorypackage = new DL_tss_quotationparthistorypackage();
        private DL_tss_quotationparthistorypackagelines _DL_tss_quotationparthistorypackagelines = new DL_tss_quotationparthistorypackagelines();
        private ShareRecords _ShareRecords = new ShareRecords();
        private DL_tss_approverlist _DL_tss_approverlist = new DL_tss_approverlist();
        #endregion
        #region Constant
        private const int STATUS_NEW = 865920000;
        private const int PIPELINEPHASE_PROSPECT = 865920000;
        private const int PartInterchange_STATUS_ACTIVE = 865920000;

        private const int STATUSCODE_DRAFT = 865920000;
        private const int STATUSCODE_INPROGRESS = 865920001;
        private const int STATUSCODE_APPROVED = 865920002;
        private const int STATUSCODE_ACTIVE = 865920003;
        private const int STATUSCODE_CLOSED = 865920004;
        private const int STATUSCODE_WON = 865920005;

        private const int STATUSREASON_OPEN = 865920000;
        private const int STATUSREASON_AppTOP = 865920001;
        private const int STATUSREASON_AppDiscount = 865920002;
        private const int STATUSREASON_AppPackage = 865920003;
        private const int STATUSREASON_APPROVED = 865920004;
        private const int STATUSREASON_ACTIVE = 865920005;
        private const int STATUSREASON_FINALAPPROVED = 865920006;
        private const int STATUSREASON_LOST = 865920007;
        private const int STATUSREASON_WON = 865920008;
        private const int STATUSREASON_REVISE = 865920009;

        private const int SOURCETYPE_SERVICE = 865920002;
        #endregion
        #region Properties
        private string _classname = "BL_tss_quotationpartheader";

        private string _entityname = "tss_quotationpartheader";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Quotation Part Header";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _tss_quonumber = false;
        private string _tss_quonumber_value;
        public string tss_quonumber
        {
            get { return _tss_quonumber ? _tss_quonumber_value : null; }
            set { _tss_quonumber = true; _tss_quonumber_value = value; }
        }

        private bool _tss_quotationid = false;
        private string _tss_quotationid_value;
        public string tss_quotationid
        {
            get { return _tss_quotationid ? _tss_quotationid_value : null; }
            set { _tss_quotationid = true; _tss_quotationid_value = value; }
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
        private bool _tss_quotationstatus = false;
        private int _tss_quotationstatus_value;
        public int tss_quotationstatus
        {
            get { return _tss_quotationstatus ? _tss_quotationstatus_value : int.MinValue; }
            set { _tss_quotationstatus = true; _tss_quotationstatus_value = value; }
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

        private bool _tss_totalprice = false;
        private Money _tss_totalprice_value;
        public decimal tss_totalprice
        {
            get { return _tss_totalprice ? _tss_totalprice_value.Value : 0; }
            set { _tss_totalprice = true; _tss_totalprice_value = new Money(value); }
        }

        private bool _tss_totalfinalprice = false;
        private Money _tss_totalfinalprice_value;
        public decimal tss_totalfinalprice
        {
            get { return _tss_totalfinalprice ? _tss_totalfinalprice_value.Value : 0; }
            set { _tss_totalfinalprice = true; _tss_totalfinalprice_value = new Money(value); }
        }

        private bool _tss_totalconfidencelevel = false;
        private int _tss_totalconfidencelevel_value;
        public int tss_totalconfidencelevel
        {
            get { return _tss_totalconfidencelevel ? _tss_totalconfidencelevel_value : int.MinValue; }
            set { _tss_totalconfidencelevel = true; _tss_totalconfidencelevel_value = value; }
        }

        private bool _tss_activedate = false;
        private DateTime _tss_activedate_value;
        public DateTime tss_activedate
        {
            get { return _tss_activedate ? _tss_activedate_value.ToLocalTime() : DateTime.MinValue; }
            set { _tss_activedate = true; _tss_activedate_value = value.ToLocalTime(); }
        }
        #endregion

        public void FinalQuotation_OnClick(IOrganizationService organizationService, Guid id, string entityName, ITracingService trace) //, IPluginExecutionContext pluginExceptionContext
        {
            try
            {
                var getQuotPartHeader = organizationService.Retrieve(entityName, id, new ColumnSet(true));
                if (getQuotPartHeader.Attributes.Contains("tss_statusreason"))
                {
                    if (getQuotPartHeader.GetAttributeValue<OptionSetValue>("tss_statuscode").Value == 865920003)
                    {
                        _DL_tss_quotationpartheader.tss_quotationstatus = STATUSCODE_APPROVED;
                        _DL_tss_quotationpartheader.tss_statusreason = STATUSREASON_FINALAPPROVED;
                        _DL_tss_quotationpartheader.FinalQuotation(organizationService, id);
                    }
                    else
                    {
                        throw new Exception("Can't update final quotation because quotation status isn't active.");
                    }
                }

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".FinalQuotation_OnClick : " + ex.Message.ToString());
            }
        }

        public void RevisedQuotation_OnClick(IOrganizationService organizationService, Guid id, string entityName, ITracingService trace) //, IPluginExecutionContext pluginExceptionContext
        {
            try
            {
                var getQuotPartHeader = organizationService.Retrieve(entityName, id, new ColumnSet(true));
                if (getQuotPartHeader.Attributes.Contains("tss_quotationnumber"))
                {
                    _DL_tss_quotationpartheader.tss_statusreason = 865920009;//update to be Revised => tss_statusreason
                    _DL_tss_quotationpartheader.tss_quotationstatus = 865920004; //updaye to be Closed  => tss_statuscode
                    _DL_tss_quotationpartheader.RevisedQuotation(organizationService, id);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".RevisedQuotation_OnClick : " + ex.Message.ToString());
            }
        }

        public void CreateQuotation_CloneEntity(IOrganizationService organizationService, Guid id, string entityName, ITracingService trace) //, IPluginExecutionContext pluginExceptionContext
        {
            try
            {
                var getQuotPartHeader = organizationService.Retrieve(entityName, id, new ColumnSet(true));
                if (getQuotPartHeader.Attributes.Contains("tss_quotationnumber"))
                {
                    if (getQuotPartHeader.GetAttributeValue<string>("tss_quotationnumber") != string.Empty)
                    {

                        CopyEntityQuotation(organizationService, entityName, id);
                    }
                    else
                    {
                        throw new Exception("Can't update final quotation because quotation status isn't active.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".CreateQuotation_CloneEntity : " + ex.Message.ToString());
            }
        }

        public void CreateSalesOrder_OnClick(IOrganizationService organizationService, Guid id, string entityName, ITracingService trace) //, IPluginExecutionContext pluginExceptionContext
        {
            try
            {
                Guid SOid = Guid.Empty;
                Guid packageUnit = Guid.Empty;
                Guid historyPackage = Guid.Empty;
                var quotpartheader = organizationService.Retrieve(entityName, id, new ColumnSet(true));
                if (quotpartheader.Attributes.Contains("tss_quotationnumber"))
                {
                    #region SO Part Header - General
                    _DL_tss_salesorderpartheader = new DL_tss_salesorderpartheader();
                    if (quotpartheader.Attributes.Contains("tss_requestdeliverydate"))
                    {
                        _DL_tss_salesorderpartheader.tss_customer = quotpartheader.GetAttributeValue<EntityReference>("tss_customer").Id;
                        _DL_tss_salesorderpartheader.tss_contact = quotpartheader.GetAttributeValue<EntityReference>("tss_contact").Id;
                        _DL_tss_salesorderpartheader.tss_pss = quotpartheader.GetAttributeValue<EntityReference>("tss_pss").Id;
                        _DL_tss_salesorderpartheader.tss_requestdeliverydate = quotpartheader.GetAttributeValue<DateTime>("tss_requestdeliverydate");
                        _DL_tss_salesorderpartheader.tss_currency = quotpartheader.GetAttributeValue<EntityReference>("tss_currency").Id;
                        _DL_tss_salesorderpartheader.tss_statuscode = 865920000; //tss_statecode
                        _DL_tss_salesorderpartheader.tss_statusreason = 865920000;
                        _DL_tss_salesorderpartheader.tss_sourcetype = quotpartheader.GetAttributeValue<OptionSetValue>("tss_sourcetype").Value;
                        if (quotpartheader.Attributes.Contains("tss_totalprice")) _DL_tss_salesorderpartheader.tss_totalamount = quotpartheader.GetAttributeValue<Money>("tss_totalprice").Value;
                        _DL_tss_salesorderpartheader.tss_branch = quotpartheader.GetAttributeValue<EntityReference>("tss_branch").Id;
                        _DL_tss_salesorderpartheader.tss_quotationlink = id;
                        if (quotpartheader.Attributes.Contains("tss_prospectlink"))
                        {
                            _DL_tss_salesorderpartheader.tss_prospectlink = quotpartheader.GetAttributeValue<EntityReference>("tss_prospectlink").Id;
                        }
                        _DL_tss_salesorderpartheader.tss_quotationlink = id;
                    }
                    else
                    {
                        throw new InvalidWorkflowException("Please fill the all General Quotation Part Information before create SO Part !");
                    }
                    #endregion
                    #region SO Part Header - Package
                    bool check = quotpartheader.GetAttributeValue<bool>("tss_package");
                    if (check)
                    {
                        if (quotpartheader.Attributes.Contains("tss_packageunit"))
                        {
                            if (quotpartheader.GetAttributeValue<EntityReference>("tss_packageunit").Id != Guid.Empty)
                            {
                                _DL_tss_salesorderpartheader.tss_package = check; //<-- Is Package (Two options)
                                _DL_tss_salesorderpartheader.tss_packageno = quotpartheader.GetAttributeValue<string>("tss_packageno");
                                _DL_tss_salesorderpartheader.tss_packagename = quotpartheader.GetAttributeValue<string>("tss_packagesname");
                                _DL_tss_salesorderpartheader.tss_packagedescription = quotpartheader.GetAttributeValue<string>("tss_packagedescription");
                                _DL_tss_salesorderpartheader.tss_packageqty = quotpartheader.GetAttributeValue<int>("tss_packageqty");
                                _DL_tss_salesorderpartheader.tss_packageunit = quotpartheader.GetAttributeValue<EntityReference>("tss_packageunit").Id;
                                packageUnit = _DL_tss_salesorderpartheader.tss_packageunit;

                                #region Create Quotation Part - History Package
                                //If Package is YES then create record on Quotation Part - History Package & Lines
                                _DL_tss_quotationparthistorypackage = new DL_tss_quotationparthistorypackage();
                                _DL_tss_quotationparthistorypackage.tss_packageno = quotpartheader.GetAttributeValue<string>("tss_packageno");
                                _DL_tss_quotationparthistorypackage.tss_packagesname = quotpartheader.GetAttributeValue<string>("tss_packagesname");
                                historyPackage = _DL_tss_quotationparthistorypackage.CreateQuotationPartHistoryPackage(organizationService);
                                #endregion
                            }
                        }
                        else
                        {
                            throw new InvalidWorkflowException("Please fill the all Package information before create SO Part !");
                        }

                        //
                    }
                    else
                    {
                        _DL_tss_salesorderpartheader.tss_package = check;
                    }
                    #endregion
                    #region SO Part Header - Payment
                    if (quotpartheader.Attributes.Contains("tss_top"))
                    {
                        _DL_tss_salesorderpartheader.tss_top = quotpartheader.GetAttributeValue<OptionSetValue>("tss_top").Value;
                        if (quotpartheader.Attributes.Contains("tss_paymentterm"))
                        {
                            _DL_tss_salesorderpartheader.tss_paymentterm = quotpartheader.GetAttributeValue<EntityReference>("tss_paymentterm").Id;
                        }
                    }
                    else
                    {
                        throw new InvalidWorkflowException("Please fill the all Term of Payment information before create SO Part !");
                    }
                    #endregion
                    SOid = _DL_tss_salesorderpartheader.CreateSalesOrder(organizationService, trace);
                    if (SOid == Guid.Empty)
                    {
                        throw new InvalidWorkflowException("Failed to create Sales Order Part!");
                    }

                    #region Create SO Lines
                    //Create SO part lines
                    QueryExpression qQuopartlines = new QueryExpression("tss_quotationpartlines")
                    {
                        ColumnSet = new ColumnSet(true),
                        Criteria = new FilterExpression()
                        {
                            Conditions =
                            {
                                new ConditionExpression("tss_quotationpartheader",ConditionOperator.Equal,id)
                            }
                        }
                    };

                    var ecQuopartlines = organizationService.RetrieveMultiple(qQuopartlines);
                    if (ecQuopartlines != null)
                    {
                        foreach (var item in ecQuopartlines.Entities)
                        {
                            Guid pricegroupID = Guid.Empty;
                            Guid pricetypeID = Guid.Empty;
                            _DL_tss_salesorderpartlines = new DL_tss_salesorderpartlines();
                            if (item.Attributes.Contains("tss_reqdeliverydate") && item.Attributes.Contains("tss_itemnumber")
                                && item.Attributes.Contains("tss_sourcetype") && item.Attributes.Contains("tss_totalprice") && item.Attributes.Contains("tss_quantity"))
                            {
                                _DL_tss_salesorderpartlines.tss_sopartheader = SOid;
                                if (item.Attributes.Contains("tss_partnumber"))
                                {
                                    _DL_tss_salesorderpartlines.tss_partnumber = item.GetAttributeValue<EntityReference>("tss_partnumber").Id;

                                    Entity partmaster = organizationService.Retrieve("trs_masterpart", item.GetAttributeValue<EntityReference>("tss_partnumber").Id, new ColumnSet(true));

                                    if (partmaster.Attributes.Contains("trs_unitmeasurement"))
                                        _DL_tss_salesorderpartlines.tss_unit = partmaster.GetAttributeValue<EntityReference>("trs_unitmeasurement").Id;
                                }
                                if (item.Attributes.Contains("tss_itemnumber")) { _DL_tss_salesorderpartlines.tss_itemnumber = item.GetAttributeValue<int>("tss_itemnumber"); }
                                if (item.Attributes.Contains("tss_reqdeliverydate")) { _DL_tss_salesorderpartlines.tss_requestdeliverydate = item.GetAttributeValue<DateTime>("tss_reqdeliverydate"); }
                                if (item.Attributes.Contains("tss_sourcetype")) 
                                {
                                    if (quotpartheader.GetAttributeValue<OptionSetValue>("tss_sourcetype").Value == 865920002)
                                    {
                                        _DL_tss_salesorderpartlines.tss_sourcetype = 865920005;
                                    }
                                    else if (quotpartheader.GetAttributeValue<OptionSetValue>("tss_sourcetype").Value == 865920001)
                                    {
                                        _DL_tss_salesorderpartlines.tss_sourcetype = 865920000;
                                    }
                                    else if (quotpartheader.GetAttributeValue<OptionSetValue>("tss_sourcetype").Value == 865920000)
                                    {
                                        _DL_tss_salesorderpartlines.tss_sourcetype = 865920003;
                                    }
                                }

                                QueryExpression qePriceGroup = new QueryExpression("new_pricegroup");
                                qePriceGroup.ColumnSet = new ColumnSet(true);
                                qePriceGroup.Criteria.AddCondition("new_code", ConditionOperator.Equal, "P2");
                                EntityCollection ecPriceGroup = organizationService.RetrieveMultiple(qePriceGroup);
                                if (ecPriceGroup.Entities.Count > 0)
                                {
                                    foreach (var pricegroup in ecPriceGroup.Entities)
                                    {
                                        pricegroupID = pricegroup.Id;
                                    }
                                }
                                //P5 belum di set pada entity Price List Part
                                //QueryExpression qePriceType = new QueryExpression("tss_pricelistpart");
                                //qePriceType.ColumnSet = new ColumnSet(true);
                                //qePriceType.Criteria.AddCondition("tss_code", ConditionOperator.Equal, "P5");
                                //EntityCollection ecPriceType = organizationService.RetrieveMultiple(qePriceType);
                                //if (ecPriceType.Entities.Count > 0)
                                //{
                                //    foreach (var pricetype in ecPriceType.Entities)
                                //    {
                                //        pricetypeID = pricetype.Id;
                                //    }
                                //}
                                _DL_tss_salesorderpartlines.tss_pricegroup = pricegroupID;
                                //_DL_tss_salesorderpartlines.tss_pricetype = pricetypeID;


                                bool interchange = item.GetAttributeValue<bool>("tss_isinterchange");
                                if (interchange)
                                {
                                    _DL_tss_salesorderpartlines.tss_isinterchange = true;
                                    if (item.Attributes.Contains("tss_partnumberinterchange"))
                                    {
                                        _DL_tss_salesorderpartlines.tss_pninterchange = item.GetAttributeValue<EntityReference>("tss_partnumberinterchange").Id;

                                        Entity partmasterinterchange = organizationService.Retrieve("trs_masterpart", item.GetAttributeValue<EntityReference>("tss_partnumberinterchange").Id, new ColumnSet(true));

                                        if (partmasterinterchange.Attributes.Contains("trs_unitmeasurement"))
                                            _DL_tss_salesorderpartlines.tss_unit = partmasterinterchange.GetAttributeValue<EntityReference>("trs_unitmeasurement").Id;
                                    }
                                }
                                else
                                {
                                    _DL_tss_salesorderpartlines.tss_isinterchange = false;
                                }
                                _DL_tss_salesorderpartlines.tss_status = STATUS_NEW;
                                if (item.Attributes.Contains("tss_currency")) { _DL_tss_salesorderpartlines.tss_currency = item.GetAttributeValue<EntityReference>("tss_currency").Id; }
                                if (item.Attributes.Contains("tss_quantity")) { _DL_tss_salesorderpartlines.tss_qtyrequest = item.GetAttributeValue<int>("tss_quantity"); }
                                //if (item.Attributes.Contains("tss_price") && !check) { _DL_tss_salesorderpartlines.tss_finalprice = item.GetAttributeValue<Money>("tss_price").Value; }
                                //if (item.Attributes.Contains("tss_finalprice") && check) { _DL_tss_salesorderpartlines.tss_finalprice = item.GetAttributeValue<Money>("tss_finalprice").Value; }
                                if (item.Attributes.Contains("tss_totalprice") && !check) 
                                { 
                                    _DL_tss_salesorderpartlines.tss_totalprice = item.GetAttributeValue<Money>("tss_totalprice").Value;
                                    _DL_tss_salesorderpartlines.tss_finalprice = item.GetAttributeValue<Money>("tss_totalprice").Value / item.GetAttributeValue<int>("tss_quantity");
                                }
                                if (item.Attributes.Contains("tss_totalfinalprice") && check)
                                {
                                    _DL_tss_salesorderpartlines.tss_totalprice = item.GetAttributeValue<Money>("tss_totalfinalprice").Value;
                                    _DL_tss_salesorderpartlines.tss_finalprice = item.GetAttributeValue<Money>("tss_totalfinalprice").Value / item.GetAttributeValue<int>("tss_quantity");
                                }

                                if (item.Attributes.Contains("tss_unitgroup"))
                                {
                                    _DL_tss_salesorderpartlines.tss_unitgroup = item.GetAttributeValue<EntityReference>("tss_unitgroup").Id;
                                }



                                _DL_tss_salesorderpartlines.CreateSOLinesFromQuotationLines(organizationService, trace);

                                #region Create Quotation Part - History Package Lines
                                //Create Quotation Part - History Package Lines
                                if (historyPackage != Guid.Empty && check)
                                {
                                    _DL_tss_quotationparthistorypackagelines = new DL_tss_quotationparthistorypackagelines();
                                    _DL_tss_quotationparthistorypackagelines.tss_historypackage = historyPackage;
                                    if (item.Attributes.Contains("tss_partnumberinterchange")) { _DL_tss_quotationparthistorypackagelines.tss_partnumberinterchange = item.GetAttributeValue<EntityReference>("tss_partnumberinterchange").Id; }
                                    _DL_tss_quotationparthistorypackagelines.tss_partnumber = item.GetAttributeValue<EntityReference>("tss_partnumber").Id;
                                    _DL_tss_quotationparthistorypackagelines.tss_quantity = item.GetAttributeValue<int>("tss_quantity");
                                    //_DL_tss_quotationparthistorypackagelines.tss_price = item.GetAttributeValue<Money>("tss_price").Value;
                                    //_DL_tss_quotationparthistorypackagelines.tss_totalprice = item.GetAttributeValue<Money>("tss_totalprice").Value;
                                    //_DL_tss_quotationparthistorypackagelines.tss_finalprice = item.GetAttributeValue<Money>("tss_finalprice").Value;
                                    //_DL_tss_quotationparthistorypackagelines.tss_discountby
                                    //_DL_tss_quotationparthistorypackagelines.tss_discpercent
                                    //_DL_tss_quotationparthistorypackagelines.tss_discamount
                                    if (item.Attributes.Contains("tss_price")) { _DL_tss_quotationparthistorypackagelines.tss_price = item.GetAttributeValue<Money>("tss_price").Value; }
                                    if (item.Attributes.Contains("tss_finalprice")) { _DL_tss_quotationparthistorypackagelines.tss_finalprice = item.GetAttributeValue<Money>("tss_finalprice").Value; }
                                    if (item.Attributes.Contains("tss_totalfinalprice")) { _DL_tss_quotationparthistorypackagelines.tss_totalprice = item.GetAttributeValue<Money>("tss_totalfinalprice").Value; }
                                    _DL_tss_quotationparthistorypackagelines.CreateQuotationPartHistoryPackageLines(organizationService);
                                }
                                #endregion
                            }
                            else
                            {
                                throw new InvalidWorkflowException("Please check the entire Quotation Part - Lines information before Created Sales Order.");
                            }

                        }
                    }
                    else
                    {
                        throw new InvalidPluginExecutionException("Cannot create Sales Order Part, please fill the Quotation Part - Lines first.");
                    }
                    #endregion
                    #region Create Service if source type is Service
                    if (SOURCETYPE_SERVICE == quotpartheader.GetAttributeValue<OptionSetValue>("tss_sourcetype").Value)
                    {
                        //update quotation if quotation service no exist
                        Guid quotationId = quotpartheader.Attributes.Contains("tss_quotationserviceno") ? quotpartheader.GetAttributeValue<EntityReference>("tss_quotationserviceno").Id : Guid.Empty ;
                        if (quotationId != Guid.Empty)
                        {
                            Entity quotationEn = new Entity("trs_quotation");
                            quotationEn["trs_quotationid"] = quotationId;
                            quotationEn["tss_statusassignquo"] =  new OptionSetValue(865920004); //so created
                            organizationService.Update(quotationEn);
                        }


                        QueryExpression qQuotService = new QueryExpression("tss_quotationpartlinesservice")
                        {
                            ColumnSet = new ColumnSet(true),
                            Criteria = new FilterExpression()
                            {
                                Conditions =
                            {
                                new ConditionExpression("tss_quotationpartheader",ConditionOperator.Equal,id)
                            }
                            }
                        };
                        EntityCollection ecQuotService = organizationService.RetrieveMultiple(qQuotService);
                        if (ecQuotService.Entities.Count > 0)
                        {
                            foreach (var item in ecQuotService.Entities)
                            {
                                if (item.Attributes.Contains("tss_commercialheader") && item.Attributes.Contains("tss_price")
                                    && item.Attributes.Contains("tss_discountamount") && item.Attributes.Contains("tss_totalprice"))
                                {
                                    _DL_tss_salesorderpartlinesservice = new DL_tss_salesorderpartlinesservice();
                                    _DL_tss_salesorderpartlinesservice.tss_sopartheader = SOid;
                                    _DL_tss_salesorderpartlinesservice.tss_commercialheader = item.GetAttributeValue<string>("tss_commercialheader");
                                    _DL_tss_salesorderpartlinesservice.tss_price = item.GetAttributeValue<Money>("tss_price").Value;
                                    _DL_tss_salesorderpartlinesservice.tss_discamount = item.GetAttributeValue<Money>("tss_discountamount").Value;
                                    _DL_tss_salesorderpartlinesservice.tss_totalprice = item.GetAttributeValue<Money>("tss_totalprice").Value;
                                    _DL_tss_salesorderpartlinesservice.CreateSOLinesService(organizationService);
                                }
                                else
                                {
                                    throw new InvalidPluginExecutionException("Cannot create Sales Order Part, please check Service information record.");
                                }

                
                                
                            }
                        }
                        else
                        {
                            throw new InvalidPluginExecutionException("Cannot create Sales Order Part, please fill the Quotation Part Lines - Service first.");
                        }
                    }
                    #endregion
                    UpdateStatusAfterCreateSO(organizationService, id, SOid, entityName, trace);

                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".CreateSalesOrder_OnClick : " + ex.Message.ToString());
            }
        }

        public void UpdateStatusAfterCreateSO(IOrganizationService organizationService, Guid id, Guid soid, string entityName, ITracingService trace)
        {
            try
            {
                var quotpartheader = organizationService.Retrieve(entityName, id, new ColumnSet(true));
                if (quotpartheader.Attributes.Contains("tss_sourcetype"))
                {
                    if (quotpartheader.Attributes.Contains("tss_prospectlink"))
                    {
                        if (quotpartheader.GetAttributeValue<OptionSetValue>("tss_sourcetype").Value == 865920000 || quotpartheader.GetAttributeValue<OptionSetValue>("tss_sourcetype").Value == 865920001)
                        {
                            //Update Status Reason & Pipeline Phase on Prospect Part Header after create Sales Order Part Header
                            Guid prosId = quotpartheader.GetAttributeValue<EntityReference>("tss_prospectlink").Id;
                            _DL_tss_prospectpartheader.tss_pipelinephase = 865920002;
                            _DL_tss_prospectpartheader.tss_statusreason = 865920001;
                            _DL_tss_prospectpartheader.UpdateStatusReasonAndPipeline(organizationService, prosId);
                        }
                    }
                    //Update Status Reason & Quotation Status after Create Sales Order Successfully
                    _DL_tss_quotationpartheader.tss_quotationstatus = 865920005;
                    _DL_tss_quotationpartheader.tss_statusreason = 865920008;
                    _DL_tss_quotationpartheader.tss_salesorderreference = soid;
        
                    _DL_tss_quotationpartheader.UpdateStatusAfterCreateSO(organizationService, id);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".UpdateStatusAfterCreateSO : " + ex.Message.ToString());
            }
        }

        public void ReviseProspect_OnClick(IOrganizationService organizationService, Guid id, string entityName, ITracingService trace) //, IPluginExecutionContext pluginExceptionContext
        {
            try
            {
                var getQuotPartHeader = organizationService.Retrieve(entityName, id, new ColumnSet(true));
                if (getQuotPartHeader.Attributes.Contains("tss_prospectlink"))
                {
                    Guid prosID = getQuotPartHeader.GetAttributeValue<EntityReference>("tss_prospectlink").Id;
                    var quotationStatusCode = getQuotPartHeader.GetAttributeValue<OptionSetValue>("tss_statuscode").Value;
                    var quotationStatusReason = getQuotPartHeader.GetAttributeValue<OptionSetValue>("tss_statusreason").Value;
                    if ((quotationStatusReason == STATUSREASON_OPEN && quotationStatusCode == STATUSCODE_DRAFT)
                        || (quotationStatusCode == STATUSCODE_APPROVED && quotationStatusReason == STATUSREASON_APPROVED)
                        || (quotationStatusCode == STATUSCODE_ACTIVE && quotationStatusReason == STATUSREASON_ACTIVE))
                    {
                        _DL_tss_quotationpartheader.tss_quotationstatus = 865920004;
                        _DL_tss_quotationpartheader.tss_statusreason = 865920009;
                        _DL_tss_quotationpartheader.ReviseProspect(organizationService, id);


                        QueryExpression queryExpressionQuotationPartProspect = new QueryExpression("tss_quotationpartheader")
                        {
                            ColumnSet = new ColumnSet(true),
                            Criteria =
                            {
                                Filters =
                                {
                                    new FilterExpression(LogicalOperator.And)
                                    {
                                        Conditions =
                                        {
                                            new ConditionExpression("tss_statuscode",ConditionOperator.NotEqual,865920004),//Closed
                                            new ConditionExpression("tss_statuscode",ConditionOperator.NotEqual,865920005),//Won
                                        }
                                    },
                                    new FilterExpression()
                                    {
                                        Conditions =
                                        {
                                            new ConditionExpression("tss_prospectlink",ConditionOperator.Equal,prosID),  
                                        }
                                    }
                                },
                                FilterOperator = LogicalOperator.And
                            }
                        };

                        var quotationPartProspectResult =
                            organizationService.RetrieveMultiple(queryExpressionQuotationPartProspect);
                        //if no active quotation for the prospect, then update.
                        if (quotationPartProspectResult.Entities.Count == 0)
                        {
                            //Update Flag Reviser Prospect on Prospect Part
                            _DL_tss_prospectpartheader = new DL_tss_prospectpartheader();
                            //_DL_tss_prospectpartheader.tss_reviser = true;
                            _DL_tss_prospectpartheader.tss_pipelinephase = PIPELINEPHASE_PROSPECT;
                            _DL_tss_prospectpartheader.UpdateStatusReasonAndPipeline(organizationService, prosID);
                        }
                        
                    }
                    else
                    {
                        throw new Exception("Can't Revise Prospect because quotation status isn't active.");
                    }
                }

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".ReviseProspect_OnClick : " + ex.Message.ToString());
            }
        }

        public void ApproveTop_OnClick(IOrganizationService organizationService, Guid id, string entityName, ITracingService trace, Guid approver)
        {
            try
            {
                Entity systemUser = new Entity();
                Guid userId = Guid.Empty;

                var getQuotPartHeader = organizationService.Retrieve(entityName, id, new ColumnSet(true));
                if (getQuotPartHeader.Attributes.Contains("tss_requestnewtop"))
                {
                    _DL_tss_quotationpartheader.tss_top = getQuotPartHeader.GetAttributeValue<OptionSetValue>("tss_requestnewtop").Value;
                    _DL_tss_quotationpartheader.tss_approvenewtop = true; //yess
                    _DL_tss_quotationpartheader.tss_approvetopby = approver;
                    _DL_tss_quotationpartheader.tss_quotationstatus = 865920002; //approved
                    _DL_tss_quotationpartheader.tss_statusreason = 865920004; //approved
                    _DL_tss_quotationpartheader.tss_requestnewtop = null;
                    _DL_tss_quotationpartheader.ApproveTopQuotation(organizationService, id);

                    #region create email
                    HelperFunction help = new HelperFunction();
                    string CRM_URL = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/");
                    CRM_URL += "TraktorNusantara";
                    string objecttypecode = string.Empty;
                    string fullname = string.Empty;
                    string targetCustomer = string.Empty;
                    string targetUnit = string.Empty;
                    string createdby = string.Empty;
                    help.GetObjectTypeCode(organizationService, "tss_quotationpartheader", out objecttypecode);

                    var context = new OrganizationServiceContext(organizationService);
                    if (getQuotPartHeader.Attributes.Contains("tss_pss"))
                    {
                        var user = (from c in context.CreateQuery("systemuser")
                                    where c.GetAttributeValue<Guid>("systemuserid") == getQuotPartHeader.GetAttributeValue<EntityReference>("tss_pss").Id
                                    select c).ToList();
                        if (user.Count > 0)
                        {
                            fullname = user[0].GetAttributeValue<string>("fullname");
                        }
                    }

                    var quot = (from c in context.CreateQuery("tss_quotationpartheader")
                                where c.GetAttributeValue<Guid>("tss_quotationpartheaderid") == id
                                select c).ToList();
                    if (quot.Count > 0)
                    {
                        if (quot[0].Attributes.Contains("tss_customer"))
                        {
                            targetCustomer = quot[0].GetAttributeValue<EntityReference>("tss_customer").Name;
                        }
                        if (quot[0].Attributes.Contains("tss_unit"))
                        {
                            targetUnit = quot[0].GetAttributeValue<EntityReference>("tss_unit").Name;
                        }
                        if (quot[0].Attributes.Contains("createdby"))
                        {
                            createdby = quot[0].GetAttributeValue<EntityReference>("createdby").Name;
                        }
                    }

                    var subject = @"Approve Change TOP has been approved in Quotation Part with Qoutation Number " + getQuotPartHeader.GetAttributeValue<string>("tss_quotationnumber");

                    var bodyTemplate = @"Dear Mr/Ms " + fullname + @",<br/><br/>
                                Your Quotation below already change TOP approved<br/><br/>";
                    if (targetCustomer != string.Empty) bodyTemplate += "Target customer : " + targetCustomer + "<br/>";
                    if (targetUnit != string.Empty) bodyTemplate += "Target unit : " + targetUnit + "<br/>";
                    bodyTemplate += "Quotation : " + "<a href='" + CRM_URL + "/main.aspx?etc=" + objecttypecode + "&pagetype=entityrecord&id=%7b" + id + "%7d'>Click here</a><br/><br/>";
                    bodyTemplate += "Thanks,<br/><br/>";
                    bodyTemplate += "Regards,<br/><br/>";
                    bodyTemplate += createdby;

                    var emailAgent = new Helper.EmailAgent();
                    var emailDescription = bodyTemplate;
                    var emailFactory = new Helper.EmailFactory();

                    emailFactory.SetFrom(emailAgent.CreateActivityParty(new List<Tuple<Guid, string>>
                    {
                        new Tuple<Guid, string>(approver,"systemuser")
                    }));
                    emailFactory.SetTo(emailAgent.CreateActivityParty(new List<Tuple<Guid, string>>
                    {
                        new Tuple<Guid, string>(getQuotPartHeader.GetAttributeValue<EntityReference>("tss_pss").Id,"systemuser")
                    }));
                    emailFactory.SetSubject(subject);
                    emailFactory.SetContent(emailDescription);
                    Guid EmailId = organizationService.Create(emailFactory.Create());
                    emailFactory.SendEmail(organizationService, EmailId);
                    #endregion

                    //delete unshare and all record in approver list
                    _DL_tss_approverlist = new DL_tss_approverlist();
                    QueryExpression qeApproverList = new QueryExpression(_DL_tss_approverlist.EntityName)
                    {
                        ColumnSet = new ColumnSet(true),
                        Criteria = new FilterExpression()
                        {
                            Conditions =
                                {
                                    new ConditionExpression("tss_quotationpartheaderid",ConditionOperator.Equal,id)
                                }
                        }
                    };
                    EntityCollection ecApproverList = _DL_tss_approverlist.Select(organizationService, qeApproverList);
                    foreach (var lstApprover in ecApproverList.Entities)
                    {
                        userId = lstApprover.GetAttributeValue<EntityReference>("tss_approver").Id;
                        systemUser = _DL_systemuser.Select(organizationService, userId);
                        _ShareRecords = new ShareRecords();
                        //Un-Shared Grant Access - Shared Form
                        _ShareRecords.UnShareRecord(organizationService, getQuotPartHeader, systemUser);
                        _ShareRecords.ShareRecordReadOnly(organizationService, getQuotPartHeader, systemUser);
                        //Delete the records
                        organizationService.Delete(_DL_tss_approverlist.EntityName, lstApprover.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".ApproveTop_OnClick : " + ex.Message.ToString());
            }
        }

        public void ApproveDiscount_OnClick(IOrganizationService organizationService, Guid id,
            string entityName, ITracingService trace, Guid userApproved)
        {
            try
            {
                Entity systemUser = new Entity();
                Guid userId = Guid.Empty;
                var getQuotPartHeader = organizationService.Retrieve(entityName, id, new ColumnSet(true));
                if (getQuotPartHeader.Contains("tss_approvediscount"))
                {
                    _DL_tss_quotationpartheader.tss_approvediscount = true; //yess
                    _DL_tss_quotationpartheader.tss_approvediscountby = userApproved;
                    _DL_tss_quotationpartheader.tss_quotationstatus = 865920002; //approved
                    _DL_tss_quotationpartheader.tss_statusreason = 865920004;   //approved
                    _DL_tss_quotationpartheader.ApproveDiscountQuotation(organizationService, id);

                    #region create email
                    HelperFunction help = new HelperFunction();
                    string CRM_URL = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/");
                    CRM_URL += "TraktorNusantara";
                    string objecttypecode = string.Empty;
                    string fullname = string.Empty;
                    string targetCustomer = string.Empty;
                    string targetUnit = string.Empty;
                    string createdby = string.Empty;
                    help.GetObjectTypeCode(organizationService, "tss_quotationpartheader", out objecttypecode);

                    var context = new OrganizationServiceContext(organizationService);
                    if (getQuotPartHeader.Attributes.Contains("tss_pss"))
                    {
                        var user = (from c in context.CreateQuery("systemuser")
                                    where c.GetAttributeValue<Guid>("systemuserid") == getQuotPartHeader.GetAttributeValue<EntityReference>("tss_pss").Id
                                    select c).ToList();
                        if (user.Count > 0)
                        {
                            fullname = user[0].GetAttributeValue<string>("fullname");
                        }
                    }

                    var quot = (from c in context.CreateQuery("tss_quotationpartheader")
                                where c.GetAttributeValue<Guid>("tss_quotationpartheaderid") == id
                                select c).ToList();
                    if (quot.Count > 0)
                    {
                        if (quot[0].Attributes.Contains("tss_customer"))
                        {
                            targetCustomer = quot[0].GetAttributeValue<EntityReference>("tss_customer").Name;
                        }
                        if (quot[0].Attributes.Contains("tss_unit"))
                        {
                            targetUnit = quot[0].GetAttributeValue<EntityReference>("tss_unit").Name;
                        }
                        if (quot[0].Attributes.Contains("createdby"))
                        {
                            createdby = quot[0].GetAttributeValue<EntityReference>("createdby").Name;
                        }
                    }

                    var subject = @"Approve Discount has been approved in Quotation Part with Qoutation Number " + getQuotPartHeader.GetAttributeValue<string>("tss_quotationnumber");

                    var bodyTemplate = @"Dear Mr/Ms " + fullname + @",<br/><br/>
                                Your Quotation below already discount approved with this price : " + Math.Round(getQuotPartHeader.GetAttributeValue<Money>("tss_totalprice").Value, 2).ToString("N") +".<br/><br/>";
                    if (targetCustomer != string.Empty) bodyTemplate += "Target customer : " + targetCustomer + "<br/>";
                    if (targetUnit != string.Empty) bodyTemplate += "Target unit : " + targetUnit + "<br/>";
                    bodyTemplate += "Quotation : " + "<a href='" + CRM_URL + "/main.aspx?etc=" + objecttypecode + "&pagetype=entityrecord&id=%7b" + id + "%7d'>Click here</a><br/><br/>";
                    bodyTemplate += "Thanks,<br/><br/>";
                    bodyTemplate += "Regards,<br/><br/>";
                    bodyTemplate += createdby;

                    var emailAgent = new Helper.EmailAgent();
                    var emailDescription = bodyTemplate;
                    var emailFactory = new Helper.EmailFactory();

                    emailFactory.SetFrom(emailAgent.CreateActivityParty(new List<Tuple<Guid, string>>
                    {
                        new Tuple<Guid, string>(userApproved,"systemuser")
                    }));
                    emailFactory.SetTo(emailAgent.CreateActivityParty(new List<Tuple<Guid, string>>
                    {
                        new Tuple<Guid, string>(getQuotPartHeader.GetAttributeValue<EntityReference>("tss_pss").Id,"systemuser")
                    }));
                    emailFactory.SetSubject(subject);
                    emailFactory.SetContent(emailDescription);
                    Guid emailId = organizationService.Create(emailFactory.Create());
                    emailFactory.SendEmail(organizationService, emailId);
                    #endregion

                    _DL_tss_approverlist = new DL_tss_approverlist();
                    QueryExpression qeApproverList = new QueryExpression(_DL_tss_approverlist.EntityName)
                    {
                        ColumnSet = new ColumnSet(true),
                        Criteria = new FilterExpression()
                        {
                            Conditions =
                                {
                                    new ConditionExpression("tss_quotationpartheaderid",ConditionOperator.Equal,id)
                                }
                        }
                    };
                    EntityCollection ecApproverList = _DL_tss_approverlist.Select(organizationService, qeApproverList);
                    foreach (var lstApprover in ecApproverList.Entities)
                    {
                        userId = lstApprover.GetAttributeValue<EntityReference>("tss_approver").Id;
                        systemUser = _DL_systemuser.Select(organizationService, userId);
                        _ShareRecords = new ShareRecords();
                        //Un-Shared Grant Access - Shared Form
                        _ShareRecords.UnShareRecord(organizationService, getQuotPartHeader, systemUser);
                        _ShareRecords.ShareRecordReadOnly(organizationService, getQuotPartHeader, systemUser);
                        //Delete the records
                        organizationService.Delete(_DL_tss_approverlist.EntityName, lstApprover.Id);

                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".ApproveDiscount_OnClick : " + ex.Message.ToString());
            }
        }

        public void UpdatePSS(IOrganizationService organizationService, Guid id, string entityName, ITracingService trace)
        {//Ga di pake
            try
            {
                Guid quotID = Guid.Empty;
                trace.Trace("Retrieve record quotation part header");
                var getQuotPartHeader = organizationService.Retrieve(entityName, id, new ColumnSet(true));
                if (getQuotPartHeader.Contains("tss_pss"))
                {
                    trace.Trace("Retrieve field entityreference PSS in quotation part header");
                    EntityReference pss = getQuotPartHeader.GetAttributeValue<EntityReference>("tss_pss");
                    EntityReference serviceRef = getQuotPartHeader.GetAttributeValue<EntityReference>("tss_quotationserviceno");

                    Guid user = getQuotPartHeader.GetAttributeValue<EntityReference>("ownerid").Id;

                    trace.Trace("Retrieve quotation service");
                    var service = organizationService.Retrieve("trs_quotation", serviceRef.Id, new ColumnSet(true));
                    Entity quotationService = new Entity("trs_quotation");
                    quotationService.Id = service.Id;
                    if (pss.Id != Guid.Empty) quotationService["tss_pss"] = pss;
                    organizationService.Update(quotationService);

                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".UpdatePSS : " + ex.Message.ToString());
            }
        }

        public void CopyEntityQuotation(IOrganizationService organizationService, string entityName, Guid id)
        {
            try
            {
                var QuotationPartHeader = organizationService.Retrieve(entityName, id, new ColumnSet(true));

                Entity target = new Entity(QuotationPartHeader.LogicalName);
                if (QuotationPartHeader != null && QuotationPartHeader.Attributes.Count > 0)
                {
                    CloneRecord(QuotationPartHeader, target);  //clone header

                    Guid clonedQuoId = organizationService.Create(target);
                    //ClonedQuotation.Set(executionContext, new EntityReference("tss_quotationpartheader", clonedOppId));

                    #region Clone Quotation Part Lines
                    QueryExpression QueryLines = new QueryExpression("tss_quotationpartlines");
                    QueryLines.ColumnSet = new ColumnSet(true);
                    QueryLines.Criteria.AddCondition("tss_quotationpartheader", ConditionOperator.Equal, QuotationPartHeader.Id);

                    EntityCollection lineItems = organizationService.RetrieveMultiple(QueryLines);
                    if (lineItems.Entities.Count > 0)
                    {
                        foreach (Entity sItem in lineItems.Entities)
                        {
                            Entity tItem = new Entity(sItem.LogicalName);
                            CloneRecord(sItem, tItem);   //clone line
                            tItem.Attributes["tss_quotationpartheader"] = new EntityReference("tss_quotationpartheader", clonedQuoId);

                            Guid CloneLinesGuid = organizationService.Create(tItem);

                            #region Update TotalPrice and TotalFinalPrice in Quotation Part Lines
                            //Guid CloneLinesGuid = organizationService.Create(tItem);  //get guid clone record Lines
                            Entity ChildEntity = organizationService.Retrieve("tss_quotationpartlines", CloneLinesGuid, new ColumnSet(true));
                            ChildEntity.Attributes["tss_finalprice"] = null;
                            ChildEntity.Attributes["tss_totalfinalprice"] = null;
                            ChildEntity.Attributes["tss_approvebypa"] = false;
                            ChildEntity.Attributes["tss_approvefinalpriceby"] = null;
                            organizationService.Update(ChildEntity);
                            #endregion
                        }
                    }
                    #endregion

                    #region Update Quotation Part Header New
                    var revision = QuotationPartHeader.GetAttributeValue<int>("tss_revision");
                    //if (revision == null)
                    //    revision = 0;
                    revision += 1;

                    Entity newEntity = organizationService.Retrieve(entityName, clonedQuoId, new ColumnSet(true));  //using new record  header
                    //Entity entity = new Entity(_entityname);
                    newEntity.Attributes["tss_quotationnumber"] = QuotationPartHeader.GetAttributeValue<string>("tss_quotationnumber"); //update using old record
                    //entity.Attributes["tss_quotationid"] = QuotationPartHeader.GetAttributeValue<string>("tss_quotationid"); //increament +1
                    newEntity.Attributes["tss_statusreason"] = new OptionSetValue(865920000); //open
                    newEntity.Attributes["tss_statuscode"] = new OptionSetValue(865920000); //  draft
                    newEntity.Attributes["tss_revision"] = revision;
                    //entity.Attributes["tss_finalprice"] = 0;
                    //entity.Attributes["tss_totalprice"] = 0;
                    newEntity.Attributes["tss_totalfinalprice"] = null;

                    //package section
                    newEntity.Attributes["tss_package"] = false;  //two option  set to NO
                    newEntity.Attributes["tss_packageno"] = string.Empty;  //text
                    newEntity.Attributes["tss_packagesname"] = string.Empty; //text
                    newEntity.Attributes["tss_packageqty"] = null;  //text
                    newEntity.Attributes["tss_packageunit"] = null;  //lookup
                    newEntity.Attributes["tss_packagedescription"] = string.Empty; //text
                    newEntity.Attributes["tss_totalexpectedpackageamount"] = null; //curruency
                    newEntity.Attributes["tss_approvepackage"] = false;

                    //update quote service no dengan data quotation lama agar link bisa mengaju pada data baru ketika  source type service n service no tidak kosong
                    if (QuotationPartHeader.Contains("tss_quotationserviceno") && QuotationPartHeader.GetAttributeValue<OptionSetValue>("tss_sourcetype").Value == 865920002)
                    {
                        //update quotation no in quotation service with new guid of quotation part header
                        var Quotation = organizationService.Retrieve("trs_quotation", QuotationPartHeader.GetAttributeValue<EntityReference>("tss_quotationserviceno").Id, new ColumnSet(true));
                        Quotation.Id = QuotationPartHeader.GetAttributeValue<EntityReference>("tss_quotationserviceno").Id;
                        Quotation.Attributes["tss_quotationpartno"] = new EntityReference("tss_quotationpartheader", newEntity.Id);
                        organizationService.Update(Quotation);

                        //newEntity.Attributes["tss_quotationserviceno"] = new EntityReference("trs_quotation", QuotationPartHeader.GetAttributeValue<EntityReference>("tss_quotationserviceno").Id);

                        #region INSERT TO LOG AFTER REVISE QUOTATION WITH OLD DATA when SOUCE TYPE is SERVICE
                        Entity LogEntity = new Entity("tss_logrevisequotation");
                        LogEntity.Attributes["tss_quotationpartheader"] = new EntityReference("tss_quotationpartheader", QuotationPartHeader.Id);
                        LogEntity.Attributes["tss_quotationserviceno"] = new EntityReference("trs_quotation", QuotationPartHeader.GetAttributeValue<EntityReference>("tss_quotationserviceno").Id);
                        organizationService.Create(LogEntity);
                        #endregion
                    }

                    organizationService.Update(newEntity);
                    #endregion

                    /*#region INSERT TO LOG AFTER REVISE QUOTATION WITH OLD DATA when SOUCE TYPE is SERVICE
                    if (QuotationPartHeader.GetAttributeValue<OptionSetValue>("tss_sourcetype").Value == 865920002) //if SOURCE TYPE = SERVICE
                    {
                        Entity LogEntity = new Entity("tss_logrevisequotation");
                        LogEntity.Attributes["tss_quotationpartheader"] = QuotationPartHeader.GetAttributeValue<EntityReference>("tss_quotationpartheader");
                        LogEntity.Attributes["tss_quotationserviceno"] = QuotationPartHeader.GetAttributeValue<EntityReference>("tss_quotationserviceno");
                        organizationService.Create(LogEntity);
                    }
                    #endregion */

                    #region Clone Quotation Part Line - Service
                    QueryExpression QueryLineService = new QueryExpression("tss_quotationpartlinesservice");
                    QueryLineService.ColumnSet = new ColumnSet(true);
                    QueryLineService.Criteria.AddCondition("tss_quotationpartheader", ConditionOperator.Equal, QuotationPartHeader.Id);

                    EntityCollection ServiceItems = organizationService.RetrieveMultiple(QueryLineService);
                    if (ServiceItems.Entities.Count > 0)
                    {
                        foreach (Entity sItem in ServiceItems.Entities)
                        {
                            Entity tItem = new Entity(sItem.LogicalName);
                            CloneRecord(sItem, tItem);   //clone line
                            tItem.Attributes["tss_quotationpartheader"] = new EntityReference("tss_quotationpartheader", clonedQuoId);
                            organizationService.Create(tItem);
                        }
                    }
                    #endregion

                    #region Clone Quotation Part Line - Supporting Material
                    QueryExpression SupportingMaterialQuery = new QueryExpression("tss_quotationpartlinessupportingmaterial");
                    SupportingMaterialQuery.ColumnSet = new ColumnSet(true);
                    SupportingMaterialQuery.Criteria.AddCondition("tss_quotationpartheader", ConditionOperator.Equal, QuotationPartHeader.Id);

                    EntityCollection SupportingMaterialItems = organizationService.RetrieveMultiple(SupportingMaterialQuery);
                    if (SupportingMaterialItems.Entities.Count > 0)
                    {
                        foreach (Entity sItem in SupportingMaterialItems.Entities)
                        {
                            Entity tItem = new Entity(sItem.LogicalName);
                            CloneRecord(sItem, tItem);   //clone line
                            tItem.Attributes["tss_quotationpartheader"] = new EntityReference("tss_quotationpartheader", clonedQuoId);
                            organizationService.Create(tItem);
                        }
                    }
                    #endregion

                    #region Clone Quotation Part - Indicator
                    QueryExpression IndicatorQuery = new QueryExpression("tss_quotationpartindicator");
                    IndicatorQuery.ColumnSet = new ColumnSet(true);
                    IndicatorQuery.Criteria.AddCondition("tss_quotationpartheader", ConditionOperator.Equal, QuotationPartHeader.Id);

                    EntityCollection IndicatorItems = organizationService.RetrieveMultiple(IndicatorQuery);
                    if (IndicatorItems.Entities.Count > 0)
                    {
                        foreach (Entity sItem in IndicatorItems.Entities)
                        {
                            Entity tItem = new Entity(sItem.LogicalName);
                            CloneRecord(sItem, tItem);   //clone line
                            tItem.Attributes["tss_quotationpartheader"] = new EntityReference("tss_quotationpartheader", clonedQuoId);
                            Guid indicatorId = organizationService.Create(tItem);

                            if (!sItem.Attributes.Contains("tss_result")) //NULL
                            {
                                //update NULL result to be No
                                tItem.Id = indicatorId;
                                tItem.Attributes["tss_result"] = null;
                                organizationService.Update(tItem);
                            }
                        }
                    }
                    #endregion

                    #region Clone Quotation Part - Reason Discount/Package
                    QueryExpression ReasonQuery = new QueryExpression("tss_quotationpartreasondiscountpackage");
                    ReasonQuery.ColumnSet = new ColumnSet(true);
                    ReasonQuery.Criteria.AddCondition("tss_quotationpartheader", ConditionOperator.Equal, QuotationPartHeader.Id);

                    EntityCollection ReasonItems = organizationService.RetrieveMultiple(ReasonQuery);
                    if (ReasonItems.Entities.Count > 0)
                    {
                        foreach (Entity sItem in ReasonItems.Entities)
                        {
                            Entity tItem = new Entity(sItem.LogicalName);
                            CloneRecord(sItem, tItem);   //clone line
                            tItem.Attributes["tss_quotationpartheader"] = new EntityReference("tss_quotationpartheader", clonedQuoId);
                            Guid ReasonId = organizationService.Create(tItem);

                            if (!sItem.Attributes.Contains("tss_result")) //NULL
                            {
                                //update NULL result to be No
                                tItem.Id = ReasonId;
                                tItem.Attributes["tss_result"] = null;
                                organizationService.Update(tItem);
                            }

                            #region Clone Quotation Part – Reason Competitor
                            QueryExpression ReasonCompetitorQuery = new QueryExpression("tss_quotationpartreasoncompetitor");
                            ReasonCompetitorQuery.ColumnSet = new ColumnSet(true);
                            ReasonCompetitorQuery.Criteria.AddCondition("tss_reasondiscountpackage", ConditionOperator.Equal, sItem.Id);

                            EntityCollection ReasonCompetitorItems = organizationService.RetrieveMultiple(ReasonCompetitorQuery);
                            if (ReasonCompetitorItems.Entities.Count > 0)
                            {
                                foreach (Entity sItem1 in ReasonCompetitorItems.Entities)
                                {
                                    Entity tItem1 = new Entity(sItem1.LogicalName);
                                    CloneRecord(sItem1, tItem1);   //clone line
                                    tItem1.Attributes["tss_reasondiscountpackage"] = new EntityReference("tss_quotationpartreasoncompetitor", ReasonId);
                                    organizationService.Create(tItem);
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".ReviseQuotation : " + ex.Message.ToString());
            }
        }

        private static void CloneRecord(Entity sourcEntity, Entity target)
        {
            string[] attributesToExclude = new string[]
            {
                    "modifiedon",
                    "createdon",
                    "statecode",
                    //"tss_quotationid",
                    "tss_statusreason",
                    "tss_statuscode",
                    "tss_revision",
                    "tss_packageno",
                    "tss_package",
                    "tss_packagesname",
                    "tss_packageqty",
                    "tss_packageunit",
                    "tss_packagedescription",
                    "tss_totalexpectedpackageamount",
                    "tss_approvepackage",
                    "tss_finalprice",
                    //"tss_totalprice",
                    "tss_totalfinalprice",
                    "tss_activedate",
                    //part lines
                     "tss_checkstockstatus",
                      "tss_qtyindent"
            };

            //string[] attributesToExclude = new string[]
            //{
            //        "modifiedon",
            //        "createdon"
            //};

            foreach (string attrName in sourcEntity.Attributes.Keys)
            {
                if (!attributesToExclude.Contains(attrName) && attrName.ToLower() != sourcEntity.LogicalName.ToLower() + "id")
                {
                    target.Attributes.Add(attrName, sourcEntity[attrName]);
                }
            }
        }

        public void ActiveQuotation_OnClick(IOrganizationService organizationService, Guid id, string entityName, ITracingService trace)
        {
            try
            {
                var QuotationPartHeader = organizationService.Retrieve(entityName, id, new ColumnSet(true));
                if (QuotationPartHeader != null && QuotationPartHeader.Attributes.Count > 0)
                {
                    #region Check Indicator not Use
                    /*QueryExpression QueryIndicator = new QueryExpression("tss_quotationpartindicator");
                    QueryIndicator.ColumnSet = new ColumnSet(true);
                    QueryIndicator.Criteria.AddCondition("tss_quotationpartheader", ConditionOperator.Equal, QuotationPartHeader.Id);

                    EntityCollection IndicatorItems = organizationService.RetrieveMultiple(QueryIndicator);
                    if (IndicatorItems.Entities.Count > 0)
                    {
                        foreach (var item in IndicatorItems.Entities)
                        {
                            //var QuotationId = item.GetAttributeValue<EntityReference>("tss_quotationpartheader"); //lookup
                            //var Indicator = item.GetAttributeValue<string>("tss_indicator");
                            //var Result = item.GetAttributeValue<bool>("tss_result");  //two option

                            //if (Indicator != null && Result != null && Value != null)//if indicator valid
                            //if (Result != true || Result != false)
                            //if (!item.Attributes.Contains("tss_result"))
                            //{
                            //    throw new Exception("INDICATOR NOT VALID");
                            //}
                            if (!item.Attributes.Contains("tss_result"))
                            {
                                throw new Exception("INDICATOR NOT VALID!");
                            }
                        }
                        //throw new Exception("Error....Indicator Not Valid");


                        //when data indicator valid update quotation part header
                        Entity entity = new Entity(entityName);
                        entity.Id = id;
                        entity.Attributes["tss_statuscode"] = new OptionSetValue(865920003); //quotation status to be Active
                        entity.Attributes["tss_statusreason"] = new OptionSetValue(865920005); //status reason to be Active
                        organizationService.Update(entity);

                        if (QuotationPartHeader.GetAttributeValue<OptionSetValue>("tss_sourcetype").Value == 865920002) //if Source Type is Service
                        {

                            //throw new Exception("work");

                            #region update data quotation service
                            QueryExpression QuotationServiceQuery = new QueryExpression("trs_quotation");  //component from quotation
                            QuotationServiceQuery.ColumnSet = new ColumnSet(true);
                            QuotationServiceQuery.Criteria.AddCondition("tss_quotationpartno", ConditionOperator.Equal, QuotationPartHeader.Id);

                            EntityCollection QuotationServiceItems = organizationService.RetrieveMultiple(QuotationServiceQuery);
                            if (QuotationServiceItems.Entities.Count > 0)
                            {
                                foreach (var QuotationService in QuotationServiceItems.Entities)
                                {
                                    Entity QuotationServiceEntity = new Entity("trs_quotation");
                                    QuotationServiceEntity.Id = QuotationService.Id;
                                    QuotationServiceEntity.Attributes["tss_statusassignquo"] = new OptionSetValue(865920001); //status assign quo to be quo Active
                                    throw new Exception(QuotationService.GetAttributeValue<OptionSetValue>("statuscode").Value.ToString());

                                    organizationService.Update(QuotationServiceEntity);
                                    trace.Trace("successfull update trs_quotation");

                                    #region Update data Component (Quotation Part Summary) - quotation part lines
                                    QueryExpression QueryPartLines = new QueryExpression("tss_quotationpartlines");  //component from quotation tss_quotationpartlines
                                    QueryPartLines.ColumnSet = new ColumnSet(true);
                                    QueryPartLines.Criteria.AddCondition("tss_quotationpartheader", ConditionOperator.Equal, QuotationPartHeader.Id);

                                    EntityCollection PartLinesItems = organizationService.RetrieveMultiple(QueryPartLines);
                                    if (PartLinesItems.Entities.Count > 0)
                                    {
                                        foreach (var PartLines in PartLinesItems.Entities)
                                        {
                                            //get id
                                            QueryExpression query = new QueryExpression("trs_quotationpartssummary");
                                            QueryPartLines.ColumnSet = new ColumnSet(true);
                                            QueryPartLines.Criteria.AddCondition("trs_quotationnumber", ConditionOperator.Equal, QuotationService.Id);
                                            var summary = organizationService.RetrieveMultiple(query).Entities[0];
                                            if (summary.Id != null && summary.Attributes.Count > 0) //when data found
                                            {
                                                trace.Trace("guid of trs_quotationpartssummary: {0}", summary.Id);
                                                //update 
                                                Entity SummaryEntity = new Entity("trs_quotationpartssummary");
                                                SummaryEntity.Id = summary.Id;
                                                SummaryEntity.Attributes["trs_quotationnumber"] = new EntityReference("trs_quotation", QuotationService.Id);
                                                SummaryEntity.Attributes["trs_partnumber"] = PartLines.GetAttributeValue<EntityReference>("tss_partnumber");
                                                SummaryEntity.Attributes["trs_manualquantity"] = PartLines.GetAttributeValue<int>("tss_quantity");
                                                SummaryEntity.Attributes["trs_price"] = PartLines.GetAttributeValue<Money>("tss_price");
                                                SummaryEntity.Attributes["trs_discountby"] = PartLines.GetAttributeValue<bool>("tss_discountby");
                                                SummaryEntity.Attributes["trs_discountamount"] = PartLines.GetAttributeValue<Money>("tss_discountamount");
                                                SummaryEntity.Attributes["trs_discountpercent"] = PartLines.GetAttributeValue<decimal>("tss_discountpercent"); //erro dsini..  quotation service Decimal part lines Int
                                                SummaryEntity.Attributes["trs_totalprice"] = PartLines.GetAttributeValue<Money>("tss_totalprice");
                                                organizationService.Update(SummaryEntity);
                                            }
                                        }
                                    }
                                    #endregion

                                    #region Update data Service (Quotation Commercial Header) - quotation part lines service
                                    QueryExpression QueryService = new QueryExpression("tss_quotationpartlinesservice");  //component from quotation
                                    QueryService.ColumnSet = new ColumnSet(true);
                                    QueryService.Criteria.AddCondition("tss_quotationpartheader", ConditionOperator.Equal, QuotationPartHeader.Id);

                                    EntityCollection ServiceItems = organizationService.RetrieveMultiple(QueryService);
                                    if (ServiceItems.Entities.Count > 0)
                                    {
                                        foreach (Entity service in ServiceItems.Entities)
                                        {
                                            //get id
                                            QueryExpression query = new QueryExpression("trs_quotationcommercialheader");
                                            QueryPartLines.ColumnSet = new ColumnSet(true);
                                            QueryPartLines.Criteria.AddCondition("trs_quotationid", ConditionOperator.Equal, QuotationService.Id);
                                            var commercial = organizationService.RetrieveMultiple(query).Entities[0];
                                            if (commercial.Id != null && commercial.Attributes.Count > 0) //when data found
                                            {
                                                trace.Trace("guid of trs_quotationpartssummary: {0}", commercial.Id);
                                                //update
                                                Entity CommercialEntity = new Entity("trs_quotationcommercialheader");
                                                CommercialEntity.Id = commercial.Id;
                                                CommercialEntity.Attributes["trs_quotationid"] = new EntityReference("trs_quotation", QuotationService.Id);
                                                CommercialEntity.Attributes["trs_taskheader"] = service.GetAttributeValue<EntityReference>("tss_taskheader");
                                                CommercialEntity.Attributes["trs_pricetype"] = service.GetAttributeValue<OptionSetValue>("tss_pricetype");
                                                CommercialEntity.Attributes["trs_commercialheader"] = service.GetAttributeValue<string>("tss_commercialheader");
                                                CommercialEntity.Attributes["trs_price"] = service.GetAttributeValue<Money>("tss_price");
                                                CommercialEntity.Attributes["trs_discountby"] = service.GetAttributeValue<bool>("tss_discountby");
                                                CommercialEntity.Attributes["trs_discountpercent"] = service.GetAttributeValue<decimal>("tss_discountpercent");
                                                CommercialEntity.Attributes["trs_discountamount"] = service.GetAttributeValue<Money>("tss_discountamount");
                                                CommercialEntity.Attributes["trs_totalprice"] = service.GetAttributeValue<Money>("tss_totalprice");
                                                organizationService.Update(CommercialEntity);
                                            }
                                        }
                                    }
                                    #endregion

                                    #region Update data Supporting Material (Quotation Supporting Material) - quotation part lines supporting material
                                    QueryExpression QueryMaterial = new QueryExpression("tss_quotationpartlinessupportingmaterial");  //component from quotation
                                    QueryMaterial.ColumnSet = new ColumnSet(true);
                                    QueryMaterial.Criteria.AddCondition("tss_quotationpartheader", ConditionOperator.Equal, QuotationPartHeader.Id);

                                    EntityCollection MaterialItems = organizationService.RetrieveMultiple(QueryMaterial);
                                    if (MaterialItems.Entities.Count > 0)
                                    {
                                        foreach (Entity SupportMaterial in MaterialItems.Entities)
                                        {
                                            //get id
                                            QueryExpression query = new QueryExpression("trs_quotationsupportingmaterial");
                                            QueryPartLines.ColumnSet = new ColumnSet(true);
                                            QueryPartLines.Criteria.AddCondition("trs_quotationid", ConditionOperator.Equal, QuotationService.Id);
                                            var material = organizationService.RetrieveMultiple(query).Entities[0];
                                            if (material.Id != null && material.Attributes.Count > 0) //when data found
                                            {
                                                trace.Trace("guid of trs_quotationpartssummary: {0}", material.Id);
                                                //update
                                                Entity MaterialEntity = new Entity("trs_quotationsupportingmaterial");
                                                MaterialEntity.Id = material.Id;
                                                MaterialEntity.Attributes["trs_quotationid"] = new EntityReference("trs_quotation", QuotationService.Id);
                                                MaterialEntity.Attributes[" trs_type"] = SupportMaterial.GetAttributeValue<bool>("tss_type");
                                                MaterialEntity.Attributes["trs_supportingmaterial"] = SupportMaterial.GetAttributeValue<string>("tss_supportingmaterial");
                                                MaterialEntity.Attributes["trs_price"] = SupportMaterial.GetAttributeValue<Money>("tss_price");
                                                MaterialEntity.Attributes["trs_quantity"] = SupportMaterial.GetAttributeValue<string>("tss_quantity");
                                                MaterialEntity.Attributes["trs_totalprice"] = SupportMaterial.GetAttributeValue<Money>("tss_totalprice");
                                                organizationService.Update(MaterialEntity);
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                            #endregion
                        }
                    }*/
                    #endregion


                    #region Without Check Indicator (use this)
                    Entity entity = new Entity(entityName);
                    entity.Id = id;
                    entity.Attributes["tss_statuscode"] = new OptionSetValue(865920003); //quotation status to be Active
                    entity.Attributes["tss_statusreason"] = new OptionSetValue(865920005); //status reason to be Active
                    entity.Attributes["tss_activedate"] = DateTime.Now;
                    organizationService.Update(entity);

                   
                    if (QuotationPartHeader.GetAttributeValue<OptionSetValue>("tss_sourcetype").Value == 865920002) //if Source Type is Service
                    {

                        Guid quotationEntityId = QuotationPartHeader.Attributes.Contains("tss_quotationserviceno") ? QuotationPartHeader.GetAttributeValue<EntityReference>("tss_quotationserviceno").Id : Guid.Empty;
                            if (quotationEntityId != Guid.Empty)
                            {
                                Entity quotationEn = new Entity("trs_quotation");
                                quotationEn["trs_quotationid"] = quotationEntityId;
                                quotationEn["tss_statusassignquo"] = new OptionSetValue(865920001);  //quo active
                                organizationService.Update(quotationEn);
                            }
                       

                        if (QuotationPartHeader.Attributes.Contains("tss_quotationserviceno"))
                        {
                            #region update data quotation service
                            QueryExpression QuotationServiceQuery = new QueryExpression("trs_quotation");  //component from quotation
                            QuotationServiceQuery.ColumnSet = new ColumnSet(true);
                            QuotationServiceQuery.Criteria.AddCondition("tss_quotationpartno", ConditionOperator.Equal, QuotationPartHeader.Id);

                            EntityCollection QuotationServiceItems = organizationService.RetrieveMultiple(QuotationServiceQuery);
                            if (QuotationServiceItems.Entities.Count > 0)
                            {
                                foreach (var QuotationService in QuotationServiceItems.Entities)
                                {
                                    Entity QuotationServiceEntity = new Entity("trs_quotation");
                                    QuotationServiceEntity.Id = QuotationService.Id;
                                    QuotationServiceEntity.Attributes["tss_statusassignquo"] = new OptionSetValue(865920001); //status assign quo to be quo Active
                                    //throw new Exception(QuotationService.GetAttributeValue<OptionSetValue>("statuscode").Value.ToString());
                                    organizationService.Update(QuotationServiceEntity);

                                    #region Update data Component (Quotation Part Summary) - quotation part lines
                                    QueryExpression QueryPartLines = new QueryExpression("tss_quotationpartlines");  //component from quotation tss_quotationpartlines
                                    QueryPartLines.ColumnSet = new ColumnSet(true);
                                    QueryPartLines.Criteria.AddCondition("tss_quotationpartheader", ConditionOperator.Equal, QuotationPartHeader.Id);

                                    EntityCollection PartLinesItems = organizationService.RetrieveMultiple(QueryPartLines);
                                    if (PartLinesItems.Entities.Count > 0)
                                    {
                                        foreach (var PartLines in PartLinesItems.Entities)
                                        {
                                            //get id
                                            QueryExpression query = new QueryExpression("trs_quotationpartssummary");
                                            query.ColumnSet = new ColumnSet(true);
                                            query.Criteria.AddCondition("trs_quotationnumber", ConditionOperator.Equal, QuotationService.Id);
                                            EntityCollection ecSumarry = organizationService.RetrieveMultiple(query);
                                            if (ecSumarry.Entities.Count > 0)
                                            {
                                                Entity summary = ecSumarry.Entities[0];
                                                if (summary.Id != null && summary.Attributes.Count > 0) //when data found
                                                {
                                                    trace.Trace("guid of trs_quotationpartssummary: {0}", summary.Id);
                                                    //update 
                                                    Entity SummaryEntity = new Entity("trs_quotationpartssummary");
                                                    SummaryEntity.Id = summary.Id;
                                                    SummaryEntity.Attributes["trs_quotationnumber"] = new EntityReference("trs_quotation", QuotationService.Id);
                                                    SummaryEntity.Attributes["trs_partnumber"] = PartLines.GetAttributeValue<EntityReference>("tss_partnumber");
                                                    SummaryEntity.Attributes["trs_manualquantity"] = PartLines.GetAttributeValue<int>("tss_quantity");
                                                    SummaryEntity.Attributes["trs_price"] = PartLines.GetAttributeValue<Money>("tss_price");
                                                    SummaryEntity.Attributes["trs_discountby"] = PartLines.GetAttributeValue<bool>("tss_discountby");
                                                    SummaryEntity.Attributes["trs_discountamount"] = PartLines.GetAttributeValue<Money>("tss_discountamount");
                                                    SummaryEntity.Attributes["trs_discountpercent"] = PartLines.GetAttributeValue<decimal>("tss_discountpercent"); //erro dsini..  quotation service Decimal part lines Int
                                                    SummaryEntity.Attributes["trs_totalprice"] = PartLines.GetAttributeValue<Money>("tss_totalprice");
                                                    organizationService.Update(SummaryEntity);
                                                }
                                            }

                                            ////////////////////////////////////

                                            #region IF ISPACKAGE IS YESS THEN INSERT TO HISTORY PACKAGE
                                            /*if (QuotationPartHeader.Attributes.Contains("tss_package"))
                                            {
                                                if (QuotationPartHeader.GetAttributeValue<bool>("tss_package") == true) //if ispackage YESS
                                                {
                                                    //HEADER
                                                    _DL_tss_quotationparthistorypackage.tss_packageno = QuotationPartHeader.GetAttributeValue<string>("tss_packageno");
                                                    _DL_tss_quotationparthistorypackage.tss_packagesname = QuotationPartHeader.GetAttributeValue<string>("tss_packagesname");
                                                    Guid HistoryId = _DL_tss_quotationparthistorypackage.CreateQuotationPartHistoryPackage(organizationService);

                                                    //QUOTATION PART LINES - HISTORY PACKAGE LINES
                                                    Entity HistoryLineEntity = new Entity("tss_quotationparthistorypackagelines");
                                                    HistoryLineEntity.Attributes["tss_historypackage"] = new EntityReference(_DL_tss_quotationparthistorypackage.EntityName, HistoryId);
                                                    HistoryLineEntity.Attributes["tss_partnumber"] = PartLines.GetAttributeValue<EntityReference>("tss_partnumber");
                                                    HistoryLineEntity.Attributes["tss_quantity"] = PartLines.GetAttributeValue<int>("tss_quantity");
                                                    HistoryLineEntity.Attributes["tss_price"] = PartLines.GetAttributeValue<Money>("tss_price");
                                                    HistoryLineEntity.Attributes["tss_discountby"] = PartLines.GetAttributeValue<bool>("tss_discountby");
                                                    HistoryLineEntity.Attributes["tss_discountamount"] = PartLines.GetAttributeValue<Money>("tss_discountamount");
                                                    HistoryLineEntity.Attributes["tss_discountpercent"] = PartLines.GetAttributeValue<decimal>("tss_discountpercent"); //erro dsini..  quotation service Decimal part lines Int
                                                    HistoryLineEntity.Attributes["tss_totalprice"] = PartLines.GetAttributeValue<Money>("tss_totalprice");
                                                    HistoryLineEntity.Attributes["tss_finalprice"] = PartLines.GetAttributeValue<Money>("tss_finalprice");
                                                    organizationService.Create(HistoryLineEntity);
                                                }
                                            }*/
                                            #endregion

                                        }
                                    }
                                    #endregion

                                    #region Update data Service (Quotation Commercial Header) - quotation part lines service
                                    QueryExpression QueryService = new QueryExpression("tss_quotationpartlinesservice");  //component from quotation
                                    QueryService.ColumnSet = new ColumnSet(true);
                                    QueryService.Criteria.AddCondition("tss_quotationpartheader", ConditionOperator.Equal, QuotationPartHeader.Id);

                                    EntityCollection ServiceItems = organizationService.RetrieveMultiple(QueryService);
                                    if (ServiceItems.Entities.Count > 0)
                                    {
                                        foreach (Entity service in ServiceItems.Entities)
                                        {
                                            //get id
                                            QueryExpression query = new QueryExpression("trs_quotationcommercialheader");
                                            query.ColumnSet = new ColumnSet(true);
                                            query.Criteria.AddCondition("trs_quotationid", ConditionOperator.Equal, QuotationService.Id);
                                            EntityCollection ecQuotationCommercialHeader = organizationService.RetrieveMultiple(query);
                                            if (ecQuotationCommercialHeader.Entities.Count > 0)
                                            {

                                                var commercial = ecQuotationCommercialHeader.Entities[0];
                                                if (commercial.Id != null && commercial.Attributes.Count > 0) //when data found
                                                {
                                                    trace.Trace("guid of trs_quotationpartssummary: {0}", commercial.Id);
                                                    //update
                                                    Entity CommercialEntity = new Entity("trs_quotationcommercialheader");
                                                    CommercialEntity.Id = commercial.Id;
                                                    CommercialEntity.Attributes["trs_quotationid"] = new EntityReference("trs_quotation", QuotationService.Id);
                                                    CommercialEntity.Attributes["trs_taskheader"] = service.GetAttributeValue<EntityReference>("tss_taskheader");
                                                    CommercialEntity.Attributes["trs_pricetype"] = service.GetAttributeValue<OptionSetValue>("tss_pricetype");
                                                    CommercialEntity.Attributes["trs_commercialheader"] = service.GetAttributeValue<string>("tss_commercialheader");
                                                    CommercialEntity.Attributes["trs_price"] = service.GetAttributeValue<Money>("tss_price");
                                                    CommercialEntity.Attributes["trs_discountby"] = service.GetAttributeValue<bool>("tss_discountby");
                                                    CommercialEntity.Attributes["trs_discountpercent"] = service.GetAttributeValue<decimal>("tss_discountpercent");
                                                    CommercialEntity.Attributes["trs_discountamount"] = service.GetAttributeValue<Money>("tss_discountamount");
                                                    CommercialEntity.Attributes["trs_totalprice"] = service.GetAttributeValue<Money>("tss_totalprice");
                                                    organizationService.Update(CommercialEntity);
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    #region Update data Supporting Material (Quotation Supporting Material) - quotation part lines supporting material
                                    QueryExpression QueryMaterial = new QueryExpression("tss_quotationpartlinessupportingmaterial");  //component from quotation
                                    QueryMaterial.ColumnSet = new ColumnSet(true);
                                    QueryMaterial.Criteria.AddCondition("tss_quotationpartheader", ConditionOperator.Equal, QuotationPartHeader.Id);

                                    EntityCollection MaterialItems = organizationService.RetrieveMultiple(QueryMaterial);
                                    if (MaterialItems.Entities.Count > 0)
                                    {
                                        foreach (Entity SupportMaterial in MaterialItems.Entities)
                                        {
                                            //get id
                                            QueryExpression query = new QueryExpression("trs_quotationsupportingmaterial");
                                            query.ColumnSet = new ColumnSet(true);
                                            query.Criteria.AddCondition("trs_quotationid", ConditionOperator.Equal, QuotationService.Id);
                                            EntityCollection ecQuotationSupportingMaterial = organizationService.RetrieveMultiple(query);
                                            if (ecQuotationSupportingMaterial.Entities.Count > 0)
                                            {
                                                Entity material = ecQuotationSupportingMaterial.Entities[0];
                                                if (material.Id != null && material.Attributes.Count > 0) //when data found
                                                {
                                                    trace.Trace("guid of trs_quotationpartssummary: {0}", material.Id);
                                                    //update
                                                    Entity MaterialEntity = new Entity("trs_quotationsupportingmaterial");
                                                    MaterialEntity.Id = material.Id;
                                                    MaterialEntity.Attributes["trs_quotationid"] = new EntityReference("trs_quotation", QuotationService.Id);
                                                    MaterialEntity.Attributes["trs_type"] = SupportMaterial.GetAttributeValue<bool>("tss_type");
                                                    MaterialEntity.Attributes["trs_supportingmaterial"] = SupportMaterial.GetAttributeValue<string>("tss_supportingmaterial");
                                                    MaterialEntity.Attributes["trs_price"] = SupportMaterial.GetAttributeValue<Money>("tss_price");
                                                    MaterialEntity.Attributes["trs_quantity"] = SupportMaterial.GetAttributeValue<string>("tss_quantity");
                                                    MaterialEntity.Attributes["trs_totalprice"] = SupportMaterial.GetAttributeValue<Money>("tss_totalprice");
                                                    organizationService.Update(MaterialEntity);
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            throw new InvalidPluginExecutionException("Cannot be Acivated...Quotation Service No Is Empty");
                        }
                    }
                    else
                    {
                        if (QuotationPartHeader.GetAttributeValue<EntityReference>("tss_prospectlink") != null)
                        {
                            Guid prosID = QuotationPartHeader.GetAttributeValue<EntityReference>("tss_prospectlink").Id;
                            //Update all quotation to LOST
                            QueryExpression qeQuot = new QueryExpression("tss_quotationpartheader");  //component from quotation
                            qeQuot.ColumnSet = new ColumnSet(true);
                            qeQuot.Criteria.AddCondition("tss_prospectlink", ConditionOperator.Equal, prosID);
                            EntityCollection ecQuot = organizationService.RetrieveMultiple(qeQuot);
                            if (ecQuot.Entities.Count > 0)
                            {
                                foreach (var quot in ecQuot.Entities)
                                {
                                    if (quot.GetAttributeValue<OptionSetValue>("tss_statusreason").Value != STATUSREASON_ACTIVE
                                        && quot.GetAttributeValue<OptionSetValue>("tss_statuscode").Value != STATUSCODE_ACTIVE)
                                    {
                                        _DL_tss_quotationpartheader = new DL_tss_quotationpartheader();
                                        _DL_tss_quotationpartheader.tss_statusreason = STATUSREASON_LOST;
                                        _DL_tss_quotationpartheader.tss_quotationstatus = STATUSCODE_CLOSED;
                                        _DL_tss_quotationpartheader.UpdateStatusAfterCreateSO(organizationService, quot.Id);
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".ActiveQuotation_OnClick : " + ex.Message.ToString());
            }
        }

        #region DOESN'T USER -- CREATED BY AMIN VICENT
        /*public void ActiveQuotation_OnClick(IOrganizationService organizationService, Guid id, string entityName, ITracingService trace)
        {
            try
            {
                var QuotationPartHeader = organizationService.Retrieve(entityName, id, new ColumnSet(true));
                if (QuotationPartHeader != null && QuotationPartHeader.Attributes.Count > 0)
                {
                    //if discount ,ditermined by under minimum price YESS 
                    if (QuotationPartHeader.GetAttributeValue<bool>("tss_underminimumprice") == true)
                    {
                        var STATUS_REASON_APPROVED = 865920004;
                        if (isDiscountReason(organizationService, QuotationPartHeader.Id) == true
                            && QuotationPartHeader.GetAttributeValue<int>("tss_statusreason") == STATUS_REASON_APPROVED
                            && QuotationPartHeader.GetAttributeValue<bool>("tss_statusreason") == true)
                        {
                            //active quotation
                            ActiveQuotation(organizationService, id, entityName, trace);
                        }
                    }
                    else
                    {
                        //active quotation
                        ActiveQuotation(organizationService, id, entityName, trace);
                    }
                }

            }
            catch (Exception ex)
            {

                throw new InvalidPluginExecutionException(_classname + ".ActiveQuotationCheckDiscount_OnClick : " + ex.Message.ToString());
            }
        }*/ 
        #endregion

        public bool isDiscountReason(IOrganizationService organizationService, Guid quoId)
        {
            var flag = true;

            QueryExpression QueryIndicator = new QueryExpression("tss_quotationpartreasondiscountpackage");
            QueryIndicator.ColumnSet = new ColumnSet(true);
            QueryIndicator.Criteria.AddCondition("tss_quotationpartheader", ConditionOperator.Equal, quoId);

            EntityCollection IndicatorItems = organizationService.RetrieveMultiple(QueryIndicator);
            if (IndicatorItems.Entities.Count > 0)
            {
                foreach (var item in IndicatorItems.Entities)
                {
                    if (!item.Attributes.Contains("tss_result"))
                    {
                        flag = false;
                        throw new Exception("You must be fulfill Discount Reason!");
                    }
                    else
                    {
                        flag = true;
                    }
                }
            }

            return flag;
        }

        public void CheckStock(IOrganizationService organizationService, Guid id, ITracingService trace)
        {
            try
            {
                Entity QuoPart = _DL_tss_quotationpartheader.Select(organizationService, id);
                //EntityReference PSS = QuoPart.GetAttributeValue<EntityReference>("tss_pss");
                //Entity PSS = _DL_systemuser.Select(organizationService, QuoPart.GetAttributeValue<EntityReference>("tss_pss").Id);
                //Entity BranchPSS = _DL_businessunit.Select(organizationService, PSS.GetAttributeValue<EntityReference>("businessunitid").Id);

                //string PSS_branchcode = BranchPSS.GetAttributeValue<string>("trs_branchcode");

                QueryExpression qQuopartlines = new QueryExpression("tss_quotationpartlines")
                {
                    ColumnSet = new ColumnSet(true),
                    Criteria = new FilterExpression()
                    {
                        Conditions =
                        {
                            new ConditionExpression("tss_quotationpartheader",ConditionOperator.Equal,id)
                        }
                    }
                };
                EntityCollection quotationpartlines = _DL_tss_quotationpartlines.Select(organizationService, qQuopartlines);

                #region set webservice SAP
                //input value to parameters.
                QueryExpression queryPartsetup = new QueryExpression("tss_sparepartsetup");
                queryPartsetup.ColumnSet = new ColumnSet(true);
                queryPartsetup.Criteria.AddCondition("tss_name", ConditionOperator.Equal, "TSS");
                Entity Partsetup = organizationService.RetrieveMultiple(queryPartsetup).Entities[0];
                //string uniqueKey;
                string uniqueKey = Partsetup.GetAttributeValue<string>("tss_sapintegrationuniquekey");
                string SAPPassword = Partsetup.GetAttributeValue<string>("tss_sapwebservicepassword");

                string _webServiceUrl = Partsetup.GetAttributeValue<string>("tss_sapwebservice");
                EndpointAddress remoteAddress = new EndpointAddress(_webServiceUrl);

                //WSHttpBinding httpbinding = new WSHttpBinding();
                //httpbinding.Name = "ZWEB_SERVICE_CRM";
                //httpbinding.MessageEncoding = WSMessageEncoding.Mtom;
                //httpbinding.TextEncoding = Encoding.UTF8;
                //trace.Trace(httpbinding.EnvelopeVersion.ToString());

                BasicHttpBinding httpbinding = new BasicHttpBinding();
                httpbinding.Name = "ZWEB_SERVICE_CRM";
                httpbinding.MessageEncoding = WSMessageEncoding.Mtom;
                httpbinding.TextEncoding = Encoding.UTF8;

                trace.Trace(httpbinding.EnvelopeVersion.ToString());

                EntityCollection updateSublines = new EntityCollection();
                EntityCollection createSublines = new EntityCollection();
                EntityCollection updatePartStocks = new EntityCollection();
                EntityCollection createPartStocks = new EntityCollection();

                #endregion

                #region vers. 2
                #region core
                trace.Trace("creating services client.");
                using (ZWEB_SERVICES_CRMClient client = new ZWEB_SERVICES_CRMClient(httpbinding, remoteAddress))
                {
                    ZcrmStatusStock stock = new ZcrmStatusStock();
                    List<ZstrStatusCek> listcekstock = new List<ZstrStatusCek>();
                    List<EntityReference> partnumbers = new List<EntityReference>();
                    string primary = string.Empty;
                    string parametersheader = string.Empty;
                    string parametersdetail = string.Empty;

                    if (quotationpartlines.Entities.Count > 0)
                    {
                        int[] qtyTotal = new int[quotationpartlines.Entities.Count];
                        for (int x = 0; x < quotationpartlines.Entities.Count; x++)
                        {
                            Entity quotationpartline = quotationpartlines.Entities[x];
                            ZstrStatusCek cekstock = new ZstrStatusCek();
                            EntityReference Partnumber = quotationpartline.GetAttributeValue<EntityReference>("tss_partnumber");
                            bool ispartinterchange = false;

                            if (quotationpartline.Contains("tss_isinterchange"))
                                ispartinterchange = quotationpartline.GetAttributeValue<bool>("tss_isinterchange");

                            if (ispartinterchange)
                            {
                                EntityReference interchange = quotationpartline.GetAttributeValue<EntityReference>("tss_partnumberinterchange");
                                Entity partlinesinterchange = organizationService.Retrieve("tss_partmasterlinesinterchange", interchange.Id, new ColumnSet(true));
                                Partnumber = partlinesinterchange.GetAttributeValue<EntityReference>("tss_partnumberinterchange");
                            }
                            partnumbers.Add(Partnumber);
                            parametersdetail = parametersdetail + string.Format("Parameter detail Part Number: {0};", Partnumber.Name);
                            trace.Trace("Current Line: " + Partnumber.Name);

                            if (x == 0)
                            {
                                primary = Partnumber.Name;
                                stock.CsrfToken = serviceEncryption.EncryptText(Partnumber.Name, uniqueKey);
                                parametersheader = string.Format("Parameter header Part Number: {0};Token: {1}", primary, stock.CsrfToken);
                            }
                            cekstock.PartNumber = Partnumber.Name;
                            listcekstock.Add(cekstock);
                        }
                        stock.StatusCek = listcekstock.ToArray();

                        try
                        {
                            client.ChannelFactory.Credentials.HttpDigest.ClientCredential.UserName = "FU-CRMSALES";
                            client.ChannelFactory.Credentials.HttpDigest.ClientCredential.UserName = SAPPassword;
                            //client.ChannelFactory.Credentials.HttpDigest.ClientCredential.UserName = "1nd()n3$*A";

                            client.ClientCredentials.UserName.UserName = "FU-CRMSALES";
                            client.ClientCredentials.UserName.Password = SAPPassword;
                            //client.ClientCredentials.UserName.Password = "1nd()n3$*A";

                            client.Open();
                            trace.Trace("success open ws sap client");

                            try
                            {
                                trace.Trace("Trigger check stock in sap.");

                                ZcrmStatusStockResponse response = client.ZcrmStatusStock(stock);
                                DateTime syncdatetime = DateTime.Parse(response.SyncDate);
                                DateTime synctime = response.SyncTime;
                                syncdatetime = syncdatetime.AddHours(synctime.Hour);
                                syncdatetime = syncdatetime.AddMinutes(synctime.Minute);
                                syncdatetime = syncdatetime.AddSeconds(synctime.Second);

                                trace.Trace("Response Result: " + response.Result);
                                trace.Trace("Response Description: " + response.Description);
                                trace.Trace("Response Sync Date & Time: " + syncdatetime.ToString());

                                if (string.Equals(response.Result, "success"))
                                {
                                    #region success
                                    //Update Status on Sales Order
                                    ZstrStatusStock[] stocks = response.StatusReturn;
                                    if (stocks.Length > 0)
                                    {
                                        foreach (var pstock in stocks)
                                        {
                                            int qty = 0;
                                            //partnumber (name di part master) //branch (new_code di Account) //stock (qty) // uom (PC/EA)
                                            string[] qtys = pstock.Stock.Trim().Split('.');
                                            if (qtys.Length > 1)
                                                qty = int.Parse(qtys[0]);

                                            string branchcode = string.Empty;

                                            //insert branch
                                            if (!string.IsNullOrWhiteSpace(pstock.Branch))
                                            {
                                                branchcode = pstock.Branch;
                                                //strBranches[createSublines.Entities.Count] = branchcode;
                                            }
                                            trace.Trace("Current item branchcode: " + branchcode + " qty: " + qty.ToString());

                                            EntityReference Partnumber = partnumbers.Where(o => o.Name == pstock.PartNumber).First();
                                            QueryExpression queryGetPartlinesInter = new QueryExpression("tss_partmasterlinesinterchange");
                                            queryGetPartlinesInter.ColumnSet = new ColumnSet(true);
                                            queryGetPartlinesInter.Criteria = new FilterExpression
                                            {
                                                FilterOperator = LogicalOperator.And,
                                                Conditions =
                                                {
                                                    new ConditionExpression("tss_partnumberinterchange", ConditionOperator.Equal, Partnumber.Id),
                                                    new ConditionExpression("tss_status", ConditionOperator.Equal, PartInterchange_STATUS_ACTIVE)
                                                }
                                            };
                                            EntityCollection PartlineInters = organizationService.RetrieveMultiple(queryGetPartlinesInter);
                                            Entity quotationpartline = new Entity();
                                            //int occurence = quotationpartlines.Entities.Where(o => o.GetAttributeValue<bool>("tss_isinterchange") == true && o.GetAttributeValue<EntityReference>("tss_partnumberinterchange").Id == PartlineInter.Id).Count();
                                            //if (occurence > 0)
                                            //{
                                            //    if (occurence == 1)
                                            //        trace.Trace("Found 1 item");
                                            //    else
                                            //        trace.Trace("Duplicate found");
                                            //}
                                            //else
                                            //{
                                            //    trace.Trace("Not found");
                                            //}
                                            if (PartlineInters.Entities.Count > 0)
                                            {
                                                Entity PartlineInter = PartlineInters.Entities[0];
                                                quotationpartline = quotationpartlines.Entities.Where(o => o.GetAttributeValue<bool>("tss_isinterchange") == true && o.GetAttributeValue<EntityReference>("tss_partnumberinterchange").Id == PartlineInter.Id).First();
                                            }
                                            else
                                            {
                                                quotationpartline = quotationpartlines.Entities.Where(o => o.GetAttributeValue<EntityReference>("tss_partnumber").Name == pstock.PartNumber).First();
                                            }

                                            QueryExpression qBranch = new QueryExpression("businessunit");
                                            qBranch.ColumnSet = new ColumnSet(true);
                                            qBranch.Criteria.AddCondition("trs_branchcode", ConditionOperator.Equal, branchcode);
                                            qBranch.Criteria.AddCondition("name", ConditionOperator.Equal, branchcode);
                                            Entity branch = _DL_businessunit.Select(organizationService, qBranch).Entities[0];

                                            QueryExpression qQuosublines = new QueryExpression("tss_quotationpartsublines");
                                            qQuosublines.ColumnSet = new ColumnSet(true);
                                            qQuosublines.Criteria.AddCondition("tss_quotationpartlines", ConditionOperator.Equal, quotationpartline.Id);
                                            qQuosublines.Criteria.AddCondition("tss_branchstockavailable", ConditionOperator.Equal, branch.Id);
                                            EntityCollection Quosublines = organizationService.RetrieveMultiple(qQuosublines);

                                            if (Quosublines.Entities.Count > 0)
                                            {
                                                trace.Trace("Same Subline in partline found!");
                                                Entity qpsubline = Quosublines.Entities[0];
                                                qpsubline["tss_qtyavailable"] = qty;
                                                updateSublines.Entities.Add(qpsubline);

                                                for (int x = 0; x < quotationpartlines.Entities.Count; x++)
                                                {
                                                    if (quotationpartlines.Entities[x].Id == quotationpartline.Id)
                                                        qtyTotal[x] += qty;
                                                }
                                            }
                                            else
                                            {
                                                Entity qpsubline = new Entity("tss_quotationpartsublines");
                                                qpsubline["tss_quotationpartlines"] = quotationpartline.ToEntityReference();
                                                qpsubline["tss_partnumber"] = Partnumber;
                                                qpsubline["tss_branchstockavailable"] = branch.ToEntityReference();
                                                qpsubline["tss_qtyavailable"] = qty;

                                                createSublines.Entities.Add(qpsubline);

                                                for (int x = 0; x < quotationpartlines.Entities.Count; x++)
                                                {
                                                    if (quotationpartlines.Entities[x].Id == quotationpartline.Id)
                                                        qtyTotal[x] += qty;
                                                }
                                            }

                                            QueryExpression qPartStocks = new QueryExpression("trs_partstock");
                                            qPartStocks.ColumnSet = new ColumnSet(true);
                                            qPartStocks.Criteria.AddCondition("trs_partnumber", ConditionOperator.Equal, Partnumber.Id);
                                            qPartStocks.Criteria.AddCondition("trs_branch", ConditionOperator.Equal, branch.Id);
                                            EntityCollection partstocks = organizationService.RetrieveMultiple(qPartStocks);
                                            if (partstocks.Entities.Count > 0)
                                            {
                                                Entity partstock = partstocks.Entities[0];
                                                partstock["trs_quantity"] = Convert.ToDecimal(qty);
                                                partstock["tss_checkstockdate"] = syncdatetime;
                                                updatePartStocks.Entities.Add(partstock);
                                            }
                                            else
                                            {
                                                Entity partstock = new Entity("trs_partstock");
                                                partstock["trs_partnumber"] = Partnumber;
                                                partstock["trs_branch"] = branch.ToEntityReference();
                                                partstock["trs_quantity"] = Convert.ToDecimal(qty);
                                                partstock["tss_checkstockdate"] = syncdatetime;
                                                createPartStocks.Entities.Add(partstock);
                                            }
                                        }
                                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, parametersheader + _mwsLog.ColumnsSeparator + parametersdetail
                                                 + response.Result + " at " + syncdatetime.ToString(), MWSLog.LogType.Information, MWSLog.Source.Outbound);
                                    }
                                    else
                                    {
                                        trace.Trace("Stocks Not Found!!!");
                                    }
                                    #endregion
                                }
                                else
                                {
                                    _mwsLog.Write(MethodBase.GetCurrentMethod().Name, parametersheader + _mwsLog.ColumnsSeparator + parametersdetail +
                                        "Failed to Check Availability Stock on SAP for Part Number : " + primary + ". Details :  " + response.Description, MWSLog.LogType.Information, MWSLog.Source.Outbound);
                                    throw new Exception("Failed to Check Availability Stock on SAP for Part Number : " + primary + ". Details :  " + response.Description);
                                }
                            }
                            catch (Exception ex)
                            {
                                trace.Trace("Failed Time: " + DateTime.Now.ToString());
                                throw new Exception(ex.Message);
                            }
                        }
                        catch (Exception ex)
                        {
                            client.Close();
                            throw new Exception(ex.Message);
                        }
                        finally
                        {
                            client.Close();
                        }

                        #region create Quotation Part subline
                        if (quotationpartlines.Entities.Count > 0)
                        {
                            trace.Trace("Start Update Quotation Part Line");
                            for (int x = 0; x < quotationpartlines.Entities.Count; x++)
                            {
                                Entity quotationpartline = quotationpartlines.Entities[x];
                                int qty = quotationpartline.GetAttributeValue<int>("tss_quantity");
                                if (qtyTotal[x] == 0)
                                {
                                    quotationpartline["tss_checkstockstatus"] = "Stock not found";
                                }
                                else
                                {
                                    quotationpartline["tss_checkstockstatus"] = "Success";
                                }

                                if (qtyTotal[x] < qty)
                                {
                                    quotationpartline["tss_qtyindent"] = qty - qtyTotal[x];
                                }
                                else
                                {
                                    quotationpartline["tss_qtyindent"] = 0;
                                }
                                organizationService.Update(quotationpartline);
                            }
                        }
                        if (createSublines.Entities.Count > 0)
                        {
                            trace.Trace("Start Update Quotation Subline Records");
                            for (int x = 0; x < updateSublines.Entities.Count; x++)
                            {
                                Entity updatesubline = updateSublines.Entities[x];
                                organizationService.Update(updatesubline);
                            }
                            trace.Trace("Start Create Quotation Subline Records");
                            //string[] strBranches = listBranch.ToArray();
                            for (int x = 0; x < createSublines.Entities.Count; x++)
                            {
                                Entity createsubline = createSublines.Entities[x];
                                Guid subid = organizationService.Create(createsubline);
                            }
                            trace.Trace("Start Update Partstock Records");
                            if (updatePartStocks.Entities.Count > 0)
                            {
                                foreach (var partStock in updatePartStocks.Entities)
                                {
                                    organizationService.Update(partStock);
                                }
                            }
                            trace.Trace("Start Create Partstock Records");
                            if (createPartStocks.Entities.Count > 0)
                            {
                                foreach (var createStock in createPartStocks.Entities)
                                {
                                    organizationService.Create(createStock);
                                }
                            }
                        }
                        #endregion
                    }
                }
                #endregion

                #endregion
                #region vers. 1
                #region core
                //trace.Trace("creating services client.");
                //using (ZWEB_SERVICE_CRMClient client = new ZWEB_SERVICE_CRMClient(httpbinding, remoteAddress))
                //{
                //    try
                //    {
                //        client.ChannelFactory.Credentials.HttpDigest.ClientCredential.UserName = "ag-rent";
                //        client.ChannelFactory.Credentials.HttpDigest.ClientCredential.UserName = "traknus2";

                //        client.ClientCredentials.UserName.UserName = "ag-rent";
                //        client.ClientCredentials.UserName.Password = "traknus2";
                //        trace.Trace("trying to modify binding timeout");
                //        client.Endpoint.Binding.OpenTimeout = new TimeSpan(1, 0, 0);
                //        client.Endpoint.Binding.CloseTimeout = new TimeSpan(1, 0, 0);
                //        client.Endpoint.Binding.SendTimeout = new TimeSpan(1, 0, 0);

                //        client.Open();
                //        trace.Trace("success open ws sap client");
                //        foreach (Entity quotationpartline in quotationpartlines.Entities)
                //        {                        //ZWEB_SERVICE_CRMClient client = new ZWEB_SERVICE_CRMClient(httpbinding, remoteAddress);
                //            #region initiation
                //            ZcrmStatusStock stock = new ZcrmStatusStock();
                //            List<ZstrStatusCek> listcekstock = new List<ZstrStatusCek>();
                //            ZstrStatusCek cekstock = new ZstrStatusCek();
                //            EntityReference Partnumber = quotationpartline.GetAttributeValue<EntityReference>("tss_partnumber");
                //            trace.Trace("Current Line: " + Partnumber.Name);

                //            //cekstock.Branch = Branchcode;
                //            cekstock.PartNumber = Partnumber.Name;
                //            listcekstock.Add(cekstock);

                //            stock.CsrfToken = serviceEncryption.EncryptText(Partnumber.Name, uniqueKey);
                //            stock.StatusCek = listcekstock.ToArray();
                //            #endregion
                //            try
                //            {

                //                trace.Trace("Trigger check stock in sap.");

                //                ZcrmStatusStockResponse response = client.ZcrmStatusStock(stock);
                //                DateTime syncdatetime = DateTime.Parse(response.SyncDate);
                //                DateTime synctime = response.SyncTime;
                //                syncdatetime.AddHours(synctime.Hour);
                //                syncdatetime.AddMinutes(synctime.Minute);
                //                syncdatetime.AddSeconds(synctime.Second);

                //                trace.Trace("Response Result: " + response.Result);
                //                trace.Trace("Response Description: " + response.Description);
                //                trace.Trace("Response Sync Date & Time: " + syncdatetime.ToString());

                //                if (string.Equals(response.Result, "success"))
                //                {
                //                    #region success
                //                    //Update Status on Sales Order
                //                    ZstrStatusStock[] stocks = response.StatusReturn;
                //                    if (stocks.Length > 0)
                //                    {
                //                        foreach (var pstock in stocks)
                //                        {
                //                            int qty = 0;
                //                            //partnumber (name di part master) //branch (new_code di Account) //stock (qty) // uom (PC/EA)
                //                            string[] qtys = pstock.Stock.Trim().Split('.');
                //                            if (qtys.Length > 1)
                //                                qty = int.Parse(qtys[0]);

                //                            string branchcode = string.Empty;

                //                            //insert branch
                //                            if (!string.IsNullOrWhiteSpace(pstock.Branch))
                //                            {
                //                                branchcode = pstock.Branch;
                //                                listBranch.Add(branchcode);
                //                                //strBranches[createSublines.Entities.Count] = branchcode;
                //                            }

                //                            trace.Trace("Current item branchcode: " + branchcode + " qty: " + qty.ToString());

                //                            //QueryExpression qBranch = new QueryExpression("businessunit");
                //                            //qBranch.ColumnSet = new ColumnSet(true);
                //                            //qBranch.Criteria.AddCondition("trs_branchcode", ConditionOperator.Equal, branchcode);
                //                            //Entity branch = _DL_businessunit.Select(organizationService, qBranch).Entities[0];

                //                            //QueryExpression qQuosublines = new QueryExpression("tss_quotationpartsublines");
                //                            //qQuosublines.ColumnSet = new ColumnSet(true);
                //                            //qQuosublines.Criteria.AddCondition("tss_quotationpartlines", ConditionOperator.Equal, quotationpartline.Id);
                //                            //qQuosublines.Criteria.AddCondition("tss_branchstockavailable", ConditionOperator.Equal, branch.Id);
                //                            //EntityCollection Quosublines = organizationService.RetrieveMultiple(qQuosublines);

                //                            //if (Quosublines.Entities.Count > 0) {
                //                            //    trace.Trace("Same Subline in partline found!");
                //                            //}
                //                            //else
                //                            //{
                //                            Entity qpsubline = new Entity("tss_quotationpartsublines");
                //                            qpsubline["tss_quotationpartlines"] = quotationpartline.ToEntityReference();
                //                            qpsubline["tss_partnumber"] = Partnumber;
                //                            //qpsubline["tss_branchstockavailable"] = branch.ToEntityReference();
                //                            qpsubline["tss_qtyavailable"] = qty;

                //                            createSublines.Entities.Add(qpsubline);
                //                            //}
                //                        }

                //                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, Partnumber.Name + _mwsLog.ColumnsSeparator
                //                                 + response.Result, MWSLog.LogType.Information, MWSLog.Source.Outbound);
                //                    }
                //                    else
                //                    {
                //                        trace.Trace("Stocks Not Found!!!");
                //                    }
                //                    #endregion
                //                }
                //                else
                //                {
                //                    throw new Exception("Failed to Check Availability Stock on SAP for Part Number : " + Partnumber.Name + ". Details :  " + response.Description);
                //                }
                //            }
                //            catch (Exception ex)
                //            {
                //                trace.Trace("Failed Time: " + DateTime.Now.ToString());

                //                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, _classname + ".CheckStock : " + _mwsLog.ColumnsSeparator
                //                             + ex.Message, MWSLog.LogType.Information, MWSLog.Source.Outbound);
                //                throw new Exception(ex.Message);
                //            }
                //        }

                //    }
                //    catch (Exception ex)
                //    {
                //        client.Close();
                //        throw new Exception(ex.Message);
                //    }
                //    finally 
                //    {
                //        client.Close();
                //    }
                //}
                #endregion
                #region create Quotation Part subline
                //if (createSublines.Entities.Count > 0)
                //{
                //    trace.Trace("Start Create Quotation Subline Records");
                //    string[] strBranches = listBranch.ToArray();
                //    for (int x = 0; x < createSublines.Entities.Count; x++)
                //    {
                //        Entity createsubline = createSublines.Entities[x];
                //        string branchcode = strBranches[x];

                //        QueryExpression qBranch = new QueryExpression("businessunit");
                //        qBranch.ColumnSet = new ColumnSet(true);
                //        qBranch.Criteria.AddCondition("trs_branchcode", ConditionOperator.Equal, branchcode);
                //        Entity branch = _DL_businessunit.Select(organizationService, qBranch).Entities[0];

                //        QueryExpression qQuosublines = new QueryExpression("tss_quotationpartsublines");
                //        qQuosublines.ColumnSet = new ColumnSet(true);
                //        qQuosublines.Criteria.AddCondition("tss_quotationpartlines", ConditionOperator.Equal, createsubline.GetAttributeValue<EntityReference>("tss_quotationpartlines").Id);
                //        qQuosublines.Criteria.AddCondition("tss_branchstockavailable", ConditionOperator.Equal, branch.Id);
                //        EntityCollection Quosublines = organizationService.RetrieveMultiple(qQuosublines);

                //        if (Quosublines.Entities.Count > 0)
                //        {
                //            trace.Trace("Same Subline in partline found!");
                //        }
                //        else
                //        {
                //            Entity qpsubline = new Entity("tss_quotationpartsublines");
                //            qpsubline["tss_quotationpartlines"] = createsubline.GetAttributeValue<EntityReference>("tss_quotationpartlines");
                //            qpsubline["tss_partnumber"] = createsubline.GetAttributeValue<EntityReference>("tss_partnumber");
                //            qpsubline["tss_branchstockavailable"] = branch.ToEntityReference();
                //            qpsubline["tss_qtyavailable"] = createsubline.GetAttributeValue<int>("tss_qtyavailable");

                //            organizationService.Create(qpsubline);
                //            //createsubline["tss_branchstockavailable"] = branch.ToEntityReference();
                //        }
                //        //organizationService.Create(createSublines.Entities[x]);
                //    }
                //}
                #endregion
                #endregion
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".CheckStock : " + ex.Message.ToString());
            }
        }

        public void sendEmailtoPackageApproval(IOrganizationService organizationService, Guid id, string entityName, ITracingService trace)
        {
            string tracing = string.Empty;
            try
            {
                var entity = organizationService.Retrieve(entityName, id, new ColumnSet(true));
                Entity sender = GetFromAdminCRM(organizationService).Entities.First();
                try
                {
                    if (entity.Contains("tss_pss") && entity.Contains("tss_quotationnumber"))
                    {
                        string quo_Number = entity.GetAttributeValue<string>("tss_quotationnumber");
                        trace.Trace("Current Quotation Number : " + quo_Number);
                        DL_systemuser _DL_systemuser = new DL_systemuser();
                        Entity pss = _DL_systemuser.Select(organizationService, entity.GetAttributeValue<EntityReference>("tss_pss").Id);
                        EntityReference branch = pss.GetAttributeValue<EntityReference>("businessunitid");

                        trace.Trace(branch.Name);
                        QueryExpression queryGetQP_Lines = new QueryExpression("tss_quotationpartlines");
                        queryGetQP_Lines.ColumnSet = new ColumnSet(true);
                        queryGetQP_Lines.Criteria = new FilterExpression
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions =
                                {
                                    new ConditionExpression("tss_quotationpartheader", ConditionOperator.Equal, entity.Id),
                                    new ConditionExpression("tss_approvebypa", ConditionOperator.NotEqual, true),
                                    new ConditionExpression("tss_unitgroup", ConditionOperator.NotNull)
                                }
                        };
                        EntityCollection QP_Lines = organizationService.RetrieveMultiple(queryGetQP_Lines);

                        if (QP_Lines.Entities.Count > 0)
                        {
                            tracing += "QP_Lines.Entities.Count = " + QP_Lines.Entities.Count + Environment.NewLine;
                            String strPathAndQuery = HttpContext.Current.Request.Url.PathAndQuery;
                            string CRM_URL = HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");
                            CRM_URL += "TraktorNusantara";

                            HelperFunction help = new HelperFunction();
                            string objecttypecode = string.Empty;
                            help.GetObjectTypeCode(organizationService, "tss_quotationpartheader", out objecttypecode);

                            Entity[] arrLines = QP_Lines.Entities.ToArray();
                            EntityReference[] distRefs = arrLines.Select(o => o.GetAttributeValue<EntityReference>("tss_unitgroup")).GroupBy(o => o.Id).Select(o => o.First()).ToArray();
                            foreach (var distRef in distRefs)
                            {
                                tracing += "distRef = " + distRef.Name + Environment.NewLine;
                                Entity[] LinesbyUnitGroup = arrLines.Where(o => o.GetAttributeValue<EntityReference>("tss_unitgroup").Id == distRef.Id).ToArray();
                                Entity[] LinesbyUnitGroupOther = arrLines.Where(o => o.GetAttributeValue<EntityReference>("tss_unitgroup").Id != distRef.Id).ToArray();
                                Entity LinebyUnitGroup = LinesbyUnitGroup.First();
                                DateTime currentdatetime = DateTime.Now.ToLocalTime();

                                EntityReference currentApproval = LinebyUnitGroup.GetAttributeValue<EntityReference>("tss_packagecurrentapprover");

                                QueryExpression queryMatrixapprovalpackage = new QueryExpression("tss_matrixapprovalpackage")
                                {
                                    ColumnSet = new ColumnSet(true),
                                    Criteria =
                                    {
                                        Conditions =
                                        {
                                            new ConditionExpression("tss_branch", ConditionOperator.Equal, branch.Id),
                                            new ConditionExpression("tss_unitgroup", ConditionOperator.Equal, distRef.Id)
                                        }
                                    },
                                    Orders =
                                    {
                                        new OrderExpression("tss_priorityno", OrderType.Ascending)
                                    }
                                };

                                EntityCollection MatrixApprovals = organizationService.RetrieveMultiple(queryMatrixapprovalpackage);
                                Entity currMatrixApproval = MatrixApprovals.Entities.FirstOrDefault();

                                if (currMatrixApproval != null)
                                {
                                    tracing += "currMatrixApproval = " + currMatrixApproval.Id + Environment.NewLine;
                                    EntityReference currApproval = currMatrixApproval.GetAttributeValue<EntityReference>("tss_approver");
                                    EntityReference ccRef = entity.GetAttributeValue<EntityReference>("ownerid");
                                    Entity Approvaldetail = organizationService.Retrieve("systemuser", currApproval.Id, new ColumnSet(true));
                                    string receivername = Approvaldetail.GetAttributeValue<string>("fullname");
                                    string programName = string.Empty;
                                    string targetCustomer = string.Empty;
                                    string targetUnit = string.Empty;
                                    string createdby = string.Empty;
                                    decimal? totalExpectedPackageAmount = null;

                                    var context = new OrganizationServiceContext(organizationService);
                                    var quot = (from c in context.CreateQuery("tss_quotationpartheader")
                                                where c.GetAttributeValue<Guid>("tss_quotationpartheaderid") == entity.Id
                                                select c).ToList();
                                    if (quot.Count > 0)
                                    {
                                        if (quot[0].Attributes.Contains("tss_packagesname"))
                                        {
                                            programName = quot[0].GetAttributeValue<string>("tss_packagesname");
                                        }
                                        if (quot[0].Attributes.Contains("tss_customer"))
                                        {
                                            targetCustomer = quot[0].GetAttributeValue<EntityReference>("tss_customer").Name;
                                        }
                                        if (quot[0].Attributes.Contains("tss_unit"))
                                        {
                                            targetUnit = quot[0].GetAttributeValue<EntityReference>("tss_unit").Name;
                                        }
                                        if (quot[0].Attributes.Contains("createdby"))
                                        {
                                            createdby = quot[0].GetAttributeValue<EntityReference>("createdby").Name;
                                        }
                                        if (quot[0].Attributes.Contains("tss_totalexpectedpackageamount"))
                                        {
                                            totalExpectedPackageAmount = quot[0].GetAttributeValue<Money>("tss_totalexpectedpackageamount").Value;
                                        }
                                    }

                                    var subject = @"Waiting Approval Convert to Package on Quotation Part with Quotation Number " + entity.GetAttributeValue<string>("tss_quotationnumber");

                                    var bodyTemplate = "Dear Mr/Ms " + receivername + ",<br/><br/>";
                                    bodyTemplate += "Need Your Approval Convert to Package for Quotation below:<br/><br/>";
                                    if (!String.IsNullOrEmpty(entity.GetAttributeValue<string>("tss_quotationnumber"))) bodyTemplate += "Quotation Number : " + entity.GetAttributeValue<string>("tss_quotationnumber") + "<br/>";
                                    if (programName != string.Empty) bodyTemplate += "Program Name : " + programName + "<br/>";
                                    if (targetCustomer != string.Empty) bodyTemplate += "Target customer : " + targetCustomer + "<br/>";
                                    if (targetUnit != string.Empty) bodyTemplate += "Target unit : " + targetUnit + "<br/>";
                                    if (totalExpectedPackageAmount != null)
                                        bodyTemplate += "Total Expected Package Amount : " + totalExpectedPackageAmount + "<br />";
                                    bodyTemplate += "Quotation : " + "<a href='" + CRM_URL + "/main.aspx?etc=" + objecttypecode + "&pagetype=entityrecord&id=%7b" + entity.Id + "%7d'>Click here</a><br/><br/>";
                                    bodyTemplate += "Thanks,<br/><br/>";
                                    bodyTemplate += "Regards,<br/><br/>";
                                    bodyTemplate += createdby;

//                                    var bodyTemplate = @"Dear Mr/Ms " + receivername + @",<br/><br/>
//                                                        Please approve convert to package request in Quotation Part.<br/><br/>
//                                                        CRM URL : <a href='" + CRM_URL + "/main.aspx?etc=" + objecttypecode + "&id=%7b" + entity.Id + "%7d&pagetype=entityrecord'>Click here</a>";
//                                    bodyTemplate += @"<br/><br/>
//                                                        Thank you,<br/><br/>
//                                                        Admin CRM";

                                    Entity email = SendEmailNotif(sender.Id, currApproval.Id, ccRef.Id, organizationService, subject, bodyTemplate);
                                    Guid EmailId = organizationService.Create(email);
                                    EmailFactory emailfactory = new EmailFactory();
                                    emailfactory.SendEmail(organizationService, EmailId);

                                    trace.Trace("Notification sent!");

                                    foreach (var updateline in LinesbyUnitGroup)
                                    {
                                        tracing += "updateline = " + updateline.Id + Environment.NewLine;
                                        updateline["tss_packagecurrentapprover"] = currMatrixApproval.ToEntityReference();
                                        updateline["tss_packagedatetime"] = currentdatetime;
                                        organizationService.Update(updateline);
                                    }
                                    trace.Trace("Update current approval!");

                                    //add records to approval list
                                    foreach (var MatrixApproval in MatrixApprovals.Entities)
                                    {
                                        tracing += "Share = " + MatrixApproval.GetAttributeValue<EntityReference>("tss_approver").Name + Environment.NewLine;
                                        Guid userId = MatrixApproval.GetAttributeValue<EntityReference>("tss_approver").Id;
                                        Entity systemUser = _DL_systemuser.Select(organizationService, userId);
                                        _ShareRecords = new ShareRecords();
                                        //Share Grant Access - Shared Form to Quotation Part
                                        _ShareRecords.ShareRecord(organizationService, entity, systemUser);

                                        foreach (var updateline in LinesbyUnitGroup)
                                        {
                                            tracing += "Approver = " + MatrixApproval.GetAttributeValue<EntityReference>("tss_approver").Name + Environment.NewLine;
                                            Entity addApprover = new Entity(_DL_tss_approverlist.EntityName);
                                            addApprover["tss_approver"] = systemUser.ToEntityReference();

                                            //Comment on 19/2/2018 remove approve list on quotation part header
                                            addApprover["tss_quotationpartheaderid"] = entity.ToEntityReference();
                                            
                                            addApprover["tss_quotationpartlinesid"] = updateline.ToEntityReference();
                                            organizationService.Create(addApprover);

                                            _ShareRecords = new ShareRecords();
                                            //Share Grant Access - Shared Form to Quotation Part Line
                                            _ShareRecords.ShareRecord(organizationService, updateline, systemUser);
                                            //Create Records
                                        }

                                        //update on 19/2/2018 share record read only to other quotation part lines
                                        foreach (var updateline in LinesbyUnitGroupOther)
                                        {
                                            _ShareRecords = new ShareRecords();
                                            _ShareRecords.UnShareRecord(organizationService, updateline, systemUser);
                                            _ShareRecords.ShareRecordReadOnly(organizationService, updateline, systemUser);
                                        }
                                    }
                                    trace.Trace("Update approval list!");

                                    //update header
                                    Entity entHeader = new Entity("tss_quotationpartheader");
                                    entHeader.Id = entity.Id;
                                    entHeader.Attributes["tss_package"] = true;
                                    entHeader.Attributes["tss_statusreason"] = new OptionSetValue(865920003);
                                    entHeader.Attributes["tss_statuscode"] = new OptionSetValue(865920001);
                                    entHeader.Attributes["tss_requestconverttopackage"] = false;
                                    organizationService.Update(entHeader);
                                }
                                else
                                {
                                    throw new Exception("Approval not found!");
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("No record quotation part lines found!");
                        }
                    }
                    else
                    {
                        throw new Exception("Field PSS / Quotation Number is null");
                    }

                    //throw new Exception(tracing);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".sendEmailtoPackageApproval : " + ex.Message.ToString());
            }
        }

        public void sendEmailtoTOPApproval(IOrganizationService organizationService, Guid id, string entityName, ITracingService trace)
        {
            try
            {
                var entity = organizationService.Retrieve(entityName, id, new ColumnSet(true));
                Entity sender = GetFromAdminCRM(organizationService).Entities.First();
                try
                {
                    if (entity.Contains("tss_quotationnumber") && entity.Contains("tss_pss"))
                    {
                        string quo_Number = entity.GetAttributeValue<string>("tss_quotationnumber");
                        trace.Trace("Current Quotation Number : " + quo_Number);
                        DL_systemuser _DL_systemuser = new DL_systemuser();
                        Entity pss = _DL_systemuser.Select(organizationService, entity.GetAttributeValue<EntityReference>("tss_pss").Id);
                        EntityReference branch = pss.GetAttributeValue<EntityReference>("businessunitid");

                        trace.Trace(branch.Name);
                        DateTime currentdatetime = DateTime.Now;

                        QueryExpression queryMatrixapprovaltop = new QueryExpression("tss_matrixapprovaltop")
                        {
                            ColumnSet = new ColumnSet(true),
                            Criteria =
                            {
                                Conditions =
                                {
                                    new ConditionExpression("tss_branch", ConditionOperator.Equal, branch.Id),
                                }
                            },
                            Orders =
                            {
                                new OrderExpression("tss_priorityno", OrderType.Ascending)
                            }
                        };

                        EntityCollection MatrixApprovals = organizationService.RetrieveMultiple(queryMatrixapprovaltop);
                        Entity currentMatrixApproval = MatrixApprovals.Entities.FirstOrDefault();

                        if (currentMatrixApproval != null)
                        {
                            String strPathAndQuery = HttpContext.Current.Request.Url.PathAndQuery;
                            string CRM_URL = HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");
                            CRM_URL += "TraktorNusantara";

                            HelperFunction help = new HelperFunction();
                            string objecttypecode = string.Empty;
                            string targetCustomer = string.Empty;
                            string targetUnit = string.Empty;
                            string createdby = string.Empty;

                            help.GetObjectTypeCode(organizationService, "tss_quotationpartheader", out objecttypecode);

                            EntityReference currApproval = currentMatrixApproval.GetAttributeValue<EntityReference>("tss_approver");
                            Entity Approvaldetail = organizationService.Retrieve("systemuser", currApproval.Id, new ColumnSet(true));
                            string receivername = Approvaldetail.GetAttributeValue<string>("fullname");
                            EntityReference ccRef = entity.GetAttributeValue<EntityReference>("ownerid");

                            var context = new OrganizationServiceContext(organizationService);
                            var quot = (from c in context.CreateQuery("tss_quotationpartheader")
                                        where c.GetAttributeValue<Guid>("tss_quotationpartheaderid") == entity.Id
                                        select c).ToList();
                            if (quot.Count > 0)
                            {
                                if (quot[0].Attributes.Contains("tss_customer"))
                                {
                                    targetCustomer = quot[0].GetAttributeValue<EntityReference>("tss_customer").Name;
                                }
                                if (quot[0].Attributes.Contains("tss_unit"))
                                {
                                    targetUnit = quot[0].GetAttributeValue<EntityReference>("tss_unit").Name;
                                }
                                if (quot[0].Attributes.Contains("createdby"))
                                {
                                    createdby = quot[0].GetAttributeValue<EntityReference>("createdby").Name;
                                }
                            }

                            var subject = @"Waiting Approval Change TOP on Quotation Part with Quotation Number " + entity.GetAttributeValue<string>("tss_quotationnumber");

                            var bodyTemplate = "Dear Mr/Ms" + receivername + ",<br/><br/>";
                            bodyTemplate += "Need Your Approval Change TOP for Quotation below:<br/><br/>";
                            if (targetCustomer != string.Empty) bodyTemplate += "Target customer : " + targetCustomer + "<br/>";
                            if (targetUnit != string.Empty) bodyTemplate += "Target unit : " + targetUnit + "<br/>";
                            bodyTemplate += "Quotation : " + "<a href='" + CRM_URL + "/main.aspx?etc=" + objecttypecode + "&pagetype=entityrecord&id=%7b" + entity.Id + "%7d'>Click here</a><br/><br/>";
                            bodyTemplate += "Thanks,<br/><br/>";
                            bodyTemplate += "Regards,<br/><br/>";
                            bodyTemplate += createdby;

//                            var bodyTemplate = @"Dear Mr/Ms " + receivername + @",<br/><br/>
//                                Please approve change TOP request in Quotation Part.<br/><br/>
//                                CRM URL : <a href='" + CRM_URL + "/main.aspx?etc=" + objecttypecode + "&id=%7b" + entity.Id + "%7d&pagetype=entityrecord'>Click here</a>";
//                            bodyTemplate += @"<br/><br/>
//                                Thank you,<br/><br/>
//                                Admin CRM";

                            Entity email = SendEmailNotif(sender.Id, currApproval.Id, ccRef.Id, organizationService, subject, bodyTemplate);
                            organizationService.Create(email);
                            trace.Trace("Notification sent!");
                            entity["tss_statuscode"] = new OptionSetValue(STATUSCODE_INPROGRESS);
                            entity["tss_statusreason"] = new OptionSetValue(STATUSREASON_AppTOP);
                            entity["tss_topcurrentapprover"] = currentMatrixApproval.ToEntityReference();
                            entity["tss_topdatetime"] = currentdatetime;
                            organizationService.Update(entity);
                            trace.Trace("Update current approval!");

                            //add records to approval list
                            foreach (var MatrixApproval in MatrixApprovals.Entities)
                            {
                                EntityReference Approval = MatrixApproval.GetAttributeValue<EntityReference>("tss_approver");
                                Entity Approvalrecord = organizationService.Retrieve("systemuser", Approval.Id, new ColumnSet(true));

                                Entity addApprover = new Entity("tss_approverlist");
                                addApprover["tss_approver"] = Approval;
                                addApprover["tss_quotationpartheaderid"] = entity.ToEntityReference();

                                _ShareRecords = new ShareRecords();
                                //Share Grant Access - Shared Form to Quotation Part
                                _ShareRecords.ShareRecord(organizationService, entity, Approvalrecord);
                                //Create Records
                                organizationService.Create(addApprover);
                            }
                            trace.Trace("Update approval list!");
                        }
                        else
                        {
                            throw new Exception("Approval not found!");
                        }
                    }
                    else
                    {
                        throw new Exception("Field Quotation Number / PSS is null!");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".sendEmailtoTOPApproval : " + ex.Message.ToString());
            }
        }

        #region Public Email
        private Entity GetSystemUserByFullname(IOrganizationService organizationService, string fullName)
        {
            try
            {
                var systemUserQuery = new QueryExpression("systemuser")
                {
                    ColumnSet = new ColumnSet(true),
                    Criteria =
                    {
                        Conditions =
                        {
                            new ConditionExpression("fullname", ConditionOperator.Equal, fullName)
                        }
                    }
                };
                var systemUserCollection = organizationService.RetrieveMultiple(systemUserQuery);
                if (systemUserCollection.Entities.Count > 0)
                {
                    return systemUserCollection.Entities.First();
                }
                else
                {
                    throw new Exception("System user with fullname " + fullName + " is not found.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured when getting system user by full name.\r\nTechnical Details: " + ex.ToString());
            }
        }

        private Entity SendEmailNotif(Guid senderGuid, Guid receiverGuid, Guid ccGuid, IOrganizationService organizationService, string subject, string bodyTemplate)
        {
            try
            {
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
                return emailFactory.Create();
            }
            catch (Exception ex)
            {
                throw new Exception("Send Email Failed. Technical Details :\r\n" + ex.ToString());
            }
        }

        private static EntityCollection GetFromAdminCRM(IOrganizationService organizationService)
        {
            DL_systemuser adm = new DL_systemuser();

            QueryExpression queryExpression = new QueryExpression(adm.EntityName);
            queryExpression.ColumnSet.AddColumns("systemuserid", "domainname");
            queryExpression.Criteria.AddCondition("domainname", ConditionOperator.Equal, "TRAKNUS\\admin.crm");

            return adm.Select(organizationService, queryExpression);
        }
        #endregion
    }
}
