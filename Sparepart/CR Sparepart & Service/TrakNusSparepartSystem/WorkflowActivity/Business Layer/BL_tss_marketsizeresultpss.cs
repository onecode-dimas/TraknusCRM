using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Messages;
using System.Activities;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TrakNusSparepartSystem.Helper;
using System.ServiceModel;
using Microsoft.Xrm;
using System.Threading.Tasks;
using CrmEarlyBound;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Win32;
using TrakNusSparepartSystem.DataLayer;
using System.Linq;
using TrakNusSparepartSystem.WorkflowActivity.Business_Layer;

namespace TrakNusSparepartSystem.WorkflowActivity.BusinessLayer
{
    public class BL_tss_marketsizeresultpss
    {
        #region Constant
        private const int UNITTYPE_UIO = 865920000;
        private const int UNITTYPE_NONUIO = 865920001;
        private const int UNITTYPE_COMMODITY = 865920002;
        private const int STATUS_OPEN = 865920000;
        private const int STATUS_DRAFT = 865920000;
        private const int STATUS_REMOVED = 865920002;
        private const int STATUS_DISABLED = 865920007;
        private const int REVISION = 865920001;
        private const int STATUS_COMPLETED_MS = 865920000;

        private const int MTD1 = 865920000;
        private const int MTD2 = 865920001;
        private const int MTD3 = 865920002;
        private const int MTD4 = 865920003;
        private const int MTD5 = 865920004;
        #endregion
        #region Dependencies
        DL_systemuser _DL_user = new DL_systemuser();
        DL_tss_marketsizeresultpss _DL_tss_marketsizeresultpss = new DL_tss_marketsizeresultpss();
        DL_tss_marketsizeresultmapping _DL_tss_marketsizeresultmapping = new DL_tss_marketsizeresultmapping();
        DL_account _DL_account = new DL_account();
        DL_tss_mastermarketsize _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        DL_tss_mastermarketsizelines _DL_tss_mastermarketsizelines = new DL_tss_mastermarketsizelines();
        DL_tss_mastermarketsizesublines _DL_tss_mastermarketsizesublines = new DL_tss_mastermarketsizesublines();
        DL_tss_sparepartsetup _DL_tss_sparepartsetup = new DL_tss_sparepartsetup();
        DL_tss_keyaccount _DL_tss_keyaccount = new DL_tss_keyaccount();
        DL_tss_matrixapprovalmarketsize _DL_tss_matrixapprovalmarketsize = new DL_tss_matrixapprovalmarketsize();
        BL_tss_mastermarketsize _BL_tss_mastermarketsize = new BL_tss_mastermarketsize();
        BL_KeyAccount _BL_KeyAccount = new BL_KeyAccount();
        EntityCollection pssToRevise = new EntityCollection();
        RetrieveHelper _retrievehelper = new RetrieveHelper();
        #endregion

        private string _dbsource = "DBSource";

        public void GenerateMarketSizeResultPSS_OnClick(IOrganizationService _organizationservice, IWorkflowContext _workflowcontext)
        {
            FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
            _filterexpression.AddCondition("tss_pss", ConditionOperator.Equal, _workflowcontext.UserId);
            _filterexpression.AddCondition("tss_status", ConditionOperator.Equal, 865920001); // STATUS CALCULATE

            QueryExpression _queryexpression = new QueryExpression("tss_keyaccount");
            _queryexpression.ColumnSet = new ColumnSet(false);
            _queryexpression.ColumnSet.AddColumn("tss_keyaccountid");
            _queryexpression.Criteria.AddFilter(_filterexpression);

            EntityCollection _keyaccounts = _organizationservice.RetrieveMultiple(_queryexpression);

            foreach (var _keyaccount in _keyaccounts.Entities)
            {
                using (OrganizationServiceContext _context = new OrganizationServiceContext(_organizationservice))
                {
                    List<SqlParameter> _sqlparameters = new List<SqlParameter>()
                    {
                        new SqlParameter("@tss_keyaccountid", SqlDbType.NVarChar) { Value = _keyaccount.Id.ToString().Replace("{", "").Replace("}", "") },
                        new SqlParameter("@systemuserid", SqlDbType.NVarChar) { Value = _workflowcontext.UserId.ToString().Replace("{", "").Replace("}", "") },
                    };

                    DataTable _datatable = new GetStoredProcedure().Connect("sp_ms_GenerateMS_ResultPSS", _sqlparameters, false);
                }
            }
        }

        public void GenerateMarketSizeResultPSS_OnClick(IOrganizationService organizationService, IWorkflowContext context, List<Entity> _keyaccountcollection)
        {
            Guid _DL_tss_marketsizeresultpss_id;
            Guid[] _customerlist = _keyaccountcollection.GroupBy(x => x.GetAttributeValue<EntityReference>("tss_customer").Id).Select(x => x.First().GetAttributeValue<EntityReference>("tss_customer").Id).ToArray();

            int _totaluio = 0;
            int _totalnonuio = 0;
            int _totalgroupuiocommodity = 0;
            int _totalalluio = 0;

            decimal _totalmscommodity = 0m;
            decimal _totalmsstandardpartMT123 = 0m;
            decimal _totalmsstandardpartMT45 = 0m;
            decimal _totalamountms = 0m;

            DateTime _startdatems = new DateTime();
            DateTime _enddatems = new DateTime();

            for (int i = 0; i < _customerlist.Count(); i++)
            {
                Entity _customer = _DL_account.Select(organizationService, _customerlist[i]);

                FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
                _filterexpression.AddCondition("tss_unittype", ConditionOperator.NotNull);
                _filterexpression.AddCondition("tss_pss", ConditionOperator.Equal, context.UserId);
                _filterexpression.AddCondition("tss_status", ConditionOperator.Equal, STATUS_COMPLETED_MS);
                _filterexpression.AddCondition("tss_customer", ConditionOperator.Equal, _customer.Id);
                _filterexpression.AddCondition("tss_ismsresultpssgenerated", ConditionOperator.Equal, false);

                QueryExpression _queryexpression = new QueryExpression(_DL_tss_mastermarketsize.EntityName);
                _queryexpression.Criteria.AddFilter(_filterexpression);
                _queryexpression.ColumnSet = new ColumnSet(true);
                List<Entity> _mastermarketsizecollection = _retrievehelper.RetrieveMultiple(organizationService, _queryexpression);

                if (_mastermarketsizecollection.Count > 0)
                {
                    object[] _mastermarketsizeids = _mastermarketsizecollection.Select(x => (object)x.Id).ToArray();
                    object[] _keyaccountids = _mastermarketsizecollection.Select(x => (object)x.GetAttributeValue<EntityReference>("tss_keyaccountid").Id).ToArray();

                    List<Entity> _currentmastermarketsize = _mastermarketsizecollection.Where(x => x.GetAttributeValue<EntityReference>("tss_customer").Id == _customerlist[i]).ToList();

                    _totaluio = _currentmastermarketsize.Where(x => x.GetAttributeValue<OptionSetValue>("tss_unittype").Value == UNITTYPE_UIO && x.GetAttributeValue<OptionSetValue>("tss_status").Value == STATUS_COMPLETED_MS).Count();
                    _totalnonuio = _currentmastermarketsize.Where(x => x.GetAttributeValue<OptionSetValue>("tss_unittype").Value == UNITTYPE_NONUIO && x.GetAttributeValue<OptionSetValue>("tss_status").Value == STATUS_COMPLETED_MS).Count();
                    _totalgroupuiocommodity = _currentmastermarketsize.Where(x => x.GetAttributeValue<OptionSetValue>("tss_unittype").Value == UNITTYPE_COMMODITY && x.GetAttributeValue<OptionSetValue>("tss_status").Value == STATUS_COMPLETED_MS).Count();
                    _totalalluio = _totaluio + _totalnonuio + _totalgroupuiocommodity;

                    _queryexpression = new QueryExpression(_DL_tss_mastermarketsizelines.EntityName);
                    _queryexpression.LinkEntities.Add(new LinkEntity(_DL_tss_mastermarketsizelines.EntityName, _DL_tss_mastermarketsizesublines.EntityName, "tss_mastermarketsizelinesid", "tss_mastermslinesref", JoinOperator.Inner));
                    _queryexpression.LinkEntities[0].Columns = new ColumnSet(true);
                    _queryexpression.LinkEntities[0].EntityAlias = "tss_mastermarketsizesublines";
                    _queryexpression.Criteria.AddCondition("tss_mastermarketsizeref", ConditionOperator.In, _mastermarketsizeids);
                    _queryexpression.ColumnSet = new ColumnSet(true);
                    List<Entity> _mastermarketsizelinescollection = _retrievehelper.RetrieveMultiple(organizationService, _queryexpression);

                    if (_mastermarketsizelinescollection.Count > 0)
                    {
                        Guid[] idsUIO = _currentmastermarketsize.Where(x => (x.GetAttributeValue<OptionSetValue>("tss_unittype").Value == UNITTYPE_UIO || x.GetAttributeValue<OptionSetValue>("tss_unittype").Value == UNITTYPE_NONUIO) && x.GetAttributeValue<OptionSetValue>("tss_status").Value == STATUS_COMPLETED_MS).Select(x => x.Id).ToArray();
                        Guid[] idsComm = _currentmastermarketsize.Where(x => x.GetAttributeValue<OptionSetValue>("tss_unittype").Value == UNITTYPE_COMMODITY).Select(x => x.Id).ToArray();

                        _totalmsstandardpartMT123 = idsUIO.Count() > 0 ? _mastermarketsizelinescollection
                                              .Where(x => idsUIO.Contains(x.GetAttributeValue<EntityReference>("tss_mastermarketsizeref").Id) && x.GetAttributeValue<DateTime>("tss_duedate") != DateTime.MinValue && (x.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD1 || x.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD2 || x.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD3))
                                              .Sum(x => (int)x.GetAttributeValue<AliasedValue>("tss_mastermarketsizesublines.tss_qty").Value * ((Money)x.GetAttributeValue<AliasedValue>("tss_mastermarketsizesublines.tss_price").Value).Value) : 0m;

                        _totalmsstandardpartMT45 = idsUIO.Count() > 0 ? _mastermarketsizelinescollection
                                                  .Where(x => idsUIO.Contains(x.GetAttributeValue<EntityReference>("tss_mastermarketsizeref").Id) && (x.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD4 || x.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD5))
                                                  .Sum(x => (int)x.GetAttributeValue<AliasedValue>("tss_mastermarketsizesublines.tss_qty").Value * ((Money)x.GetAttributeValue<AliasedValue>("tss_mastermarketsizesublines.tss_price").Value).Value) : 0m;

                        foreach (Guid item in idsComm)
                        {
                            _filterexpression = new FilterExpression(LogicalOperator.And);
                            _filterexpression.AddCondition("tss_mastermarketsizeid", ConditionOperator.Equal, item);

                            _queryexpression = new QueryExpression("tss_mastermarketsizesublines");
                            _queryexpression.Criteria.AddFilter(_filterexpression);
                            _queryexpression.ColumnSet = new ColumnSet(true);
                            List<Entity> _mastermarketsizesublinescollection = _retrievehelper.RetrieveMultiple(organizationService, _queryexpression); // organizationService.RetrieveMultiple(_queryexpression);

                            foreach (var _mastermarketsizesublinesitem in _mastermarketsizesublinescollection)
                                _totalmscommodity += (decimal)(_mastermarketsizesublinesitem.GetAttributeValue<int>("tss_qty") * _mastermarketsizesublinesitem.GetAttributeValue<Money>("tss_price").Value);
                        }

                        _totalamountms = _totalmscommodity + _totalmsstandardpartMT123 + _totalmsstandardpartMT45;
                        _totalmsstandardpartMT123 += _totalmsstandardpartMT45;
                    }
                    else
                    {
                        Guid[] idsComm = _currentmastermarketsize.Where(x => x.GetAttributeValue<OptionSetValue>("tss_unittype").Value == UNITTYPE_COMMODITY).Select(x => x.Id).ToArray();

                        foreach (Guid item in idsComm)
                        {
                            _filterexpression = new FilterExpression(LogicalOperator.And);
                            _filterexpression.AddCondition("tss_mastermarketsizeid", ConditionOperator.Equal, item);

                            _queryexpression = new QueryExpression("tss_mastermarketsizesublines");
                            _queryexpression.Criteria.AddFilter(_filterexpression);
                            _queryexpression.ColumnSet = new ColumnSet(true);

                            List<Entity> _mastermarketsizesublines = _retrievehelper.RetrieveMultiple(organizationService, _queryexpression); // organizationService.RetrieveMultiple(_queryexpression);

                            foreach (var _mastermarketsizesublinesvar in _mastermarketsizesublines)
                                _totalmscommodity += (decimal)(_mastermarketsizesublinesvar.GetAttributeValue<int>("tss_qty") * _mastermarketsizesublinesvar.GetAttributeValue<Money>("tss_price").Value);
                        }

                        _totalamountms = _totalmscommodity;
                    }

                    GetActivePeriodDate _getactiveperioddate = new GetActivePeriodDate(organizationService);
                    _getactiveperioddate.Process();

                    _startdatems = (DateTime)_getactiveperioddate.StartDateMarketSize; // _keyaccount.GetAttributeValue<DateTime>("tss_msperiodstart");
                    _enddatems = (DateTime)_getactiveperioddate.EndDateMarketSize; // _keyaccount.GetAttributeValue<DateTime>("tss_msperiodend");

                    _DL_tss_marketsizeresultpss = new DL_tss_marketsizeresultpss();

                    if (_customer.Contains("parentaccountid"))
                        _DL_tss_marketsizeresultpss.tss_customergroup = _customer.GetAttributeValue<EntityReference>("parentaccountid").Id;

                    _DL_tss_marketsizeresultpss.tss_pss = context.UserId;
                    _DL_tss_marketsizeresultpss.tss_branch = context.BusinessUnitId; // _systemuser.GetAttributeValue<EntityReference>("businessunitid").Id;
                    _DL_tss_marketsizeresultpss.tss_customer = _customerlist[i];
                    _DL_tss_marketsizeresultpss.tss_totaluio = _totaluio;
                    _DL_tss_marketsizeresultpss.tss_totalnonuio = _totalnonuio;
                    _DL_tss_marketsizeresultpss.tss_totalgroupuiocommodity = _totalgroupuiocommodity;
                    _DL_tss_marketsizeresultpss.tss_totalalluio = _totalalluio;
                    _DL_tss_marketsizeresultpss.tss_totalmscommodity = _totalmscommodity;
                    _DL_tss_marketsizeresultpss.tss_totalmsstandardpart = _totalmsstandardpartMT123;
                    _DL_tss_marketsizeresultpss.tss_totalamountms = _totalamountms;
                    _DL_tss_marketsizeresultpss.tss_msperiodstart = _startdatems; // startPeriodMS;
                    _DL_tss_marketsizeresultpss.tss_msperiodend = _enddatems; // endPeriodMS;
                    _DL_tss_marketsizeresultpss.tss_activeperiodstart = _startdatems;
                    _DL_tss_marketsizeresultpss.tss_activeperiodsend = _enddatems;
                    _DL_tss_marketsizeresultpss.tss_status = STATUS_DRAFT;
                    _DL_tss_marketsizeresultpss.tss_statusreason = STATUS_OPEN;
                    _DL_tss_marketsizeresultpss_id = _DL_tss_marketsizeresultpss.Insert(organizationService);

                    _filterexpression = new FilterExpression(LogicalOperator.And);
                    _filterexpression.AddCondition("tss_keyaccountid", ConditionOperator.In, _keyaccountids);
                    _filterexpression.AddCondition("tss_customer", ConditionOperator.Equal, _customer.Id);

                    _queryexpression = new QueryExpression("tss_keyaccount");
                    _queryexpression.ColumnSet = new ColumnSet(true);
                    _queryexpression.Criteria.AddFilter(_filterexpression);

                    _keyaccountcollection = _retrievehelper.RetrieveMultiple(organizationService, _queryexpression);

                    foreach (var _keyaccountvar in _keyaccountcollection)
                    {
                        Entity _keyaccount = _keyaccountvar;

                        // CREATE MAPPING
                        DL_tss_marketsizeresultmapping _DL_tss_marketsizeresultmapping = new DL_tss_marketsizeresultmapping();
                        _DL_tss_marketsizeresultmapping.tss_keyaccount = _keyaccount.Id;
                        _DL_tss_marketsizeresultmapping.tss_marketsizeresultpss = _DL_tss_marketsizeresultpss_id; // context.PrimaryEntityId;
                        _DL_tss_marketsizeresultmapping.Insert(organizationService);

                        // CREATE MARKET SIZE RESULT PSS - DETAIL - KA UIO & NON UIO
                        _filterexpression = new FilterExpression(LogicalOperator.And);
                        _filterexpression.AddCondition("tss_keyaccountid", ConditionOperator.Equal, _keyaccount.Id);

                        _queryexpression = new QueryExpression("tss_kauio");
                        _queryexpression.Criteria.AddFilter(_filterexpression);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        EntityCollection _kauiocollection = organizationService.RetrieveMultiple(_queryexpression);

                        foreach (var _kauioitem in _kauiocollection.Entities)
                        {
                            Entity _marketsizeresultpssdetail = new Entity("tss_marketsizeresultpssdetail");
                            Entity _population = organizationService.Retrieve("new_population", _kauioitem.GetAttributeValue<EntityReference>("tss_serialnumber").Id, new ColumnSet(true));
                            bool _populationstatus = _population.GetAttributeValue<bool>("tss_populationstatus");

                            if (_populationstatus)
                                _marketsizeresultpssdetail["tss_uiotype"] = new OptionSetValue(865920000); // KA UIO
                            else
                                _marketsizeresultpssdetail["tss_uiotype"] = new OptionSetValue(865920001); // KA Non UIO

                            _marketsizeresultpssdetail["tss_kauioid"] = new EntityReference("tss_kauio", _kauioitem.Id);
                            _marketsizeresultpssdetail["tss_serialnumber"] = new EntityReference("new_population", _population.Id);
                            _marketsizeresultpssdetail["tss_marketsizeresultpssid"] = new EntityReference("tss_marketsizeresultpss", _DL_tss_marketsizeresultpss_id);

                            organizationService.Create(_marketsizeresultpssdetail);
                        }

                        // CREATE MARKET SIZE RESULT PSS - DETAIL - KA GROUP UIO COMMODITY
                        _filterexpression = new FilterExpression(LogicalOperator.And);
                        _filterexpression.AddCondition("tss_keyaccountid", ConditionOperator.Equal, _keyaccount.Id);

                        _queryexpression = new QueryExpression("tss_kagroupuiocommodity");
                        _queryexpression.Criteria.AddFilter(_filterexpression);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        EntityCollection _kagroupuiocommoditycollection = organizationService.RetrieveMultiple(_queryexpression);

                        foreach (var _kagroupuiocommodityitem in _kagroupuiocommoditycollection.Entities)
                        {
                            Entity _marketsizeresultpssdetail = new Entity("tss_marketsizeresultpssdetail");

                            _marketsizeresultpssdetail["tss_uiotype"] = new OptionSetValue(865920002);
                            _marketsizeresultpssdetail["tss_marketsizeresultpssid"] = new EntityReference("tss_marketsizeresultpss", _DL_tss_marketsizeresultpss_id);
                            _marketsizeresultpssdetail["tss_groupuiocommodity"] = _kagroupuiocommodityitem.GetAttributeValue<EntityReference>("tss_groupuiocommodity");
                            _marketsizeresultpssdetail["tss_kagroupuiocommodityid"] = new EntityReference("tss_kagroupuiocommodity", _kagroupuiocommodityitem.Id);

                            organizationService.Create(_marketsizeresultpssdetail);
                        }
                    }

                    // UPDATE MASTER MARKET SIZE field 'tss_ismsresultpssgenerated' menjadi 'TRUE' SUPAYA TIDAK BISA GENERATE LEBIH DARI 1x.
                    foreach (var _mastermarketsizeitem in _mastermarketsizecollection)
                    {
                        Entity _mastermarketsize = _mastermarketsizeitem;

                        _mastermarketsize.Attributes["tss_ismsresultpssgenerated"] = true;

                        organizationService.Update(_mastermarketsize);
                    }

                }

            }
        }

        public void ConfirmMarketSizeResultPSS_OnClick(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context, string recordID)
        {
            try
            {
                if (String.IsNullOrEmpty(context.PrimaryEntityId.ToString()))
                {
                    throw new InvalidWorkflowException("Unable to find market size result pss, please make sure to save and then retry the action .");
                }
                else
                {
                    //get Entity market size result pss 
                    Guid marketsizeresultspssPK = new Guid(recordID);
                    Entity EN_marketsizeresultpss = _DL_tss_marketsizeresultpss.Select(organizationService, marketsizeresultspssPK);
                    EN_marketsizeresultpss["tss_status"] = new OptionSetValue(865920001);//"Waiting Approval PDH"
                    EN_marketsizeresultpss["tss_currentapprovaldate"] = DateTime.Now.ToLocalTime();
                    EN_marketsizeresultpss["tss_confirmdate"] = DateTime.Now.ToLocalTime();

                    //get entity User
                    Entity EN_User = _DL_user.Select(organizationService, EN_marketsizeresultpss.GetAttributeValue<EntityReference>("ownerid").Id);
                    EN_User["tss_marketsizeconfirmed"] = true;// "Yes"

                    organizationService.Update(EN_User);
                    organizationService.Update(EN_marketsizeresultpss);
                    //_DL_tss_marketsizeresultpss.Update(organizationService, marketsizeresultspssPK);

                    FilterExpression fappMat = new FilterExpression(LogicalOperator.And);
                    fappMat.AddCondition("tss_branch", ConditionOperator.Equal, EN_marketsizeresultpss.GetAttributeValue<EntityReference>("tss_branch").Id);
                    fappMat.AddCondition("tss_approvaltype", ConditionOperator.Equal, 865920000);
                    fappMat.AddCondition("tss_approvalfor", ConditionOperator.Equal, 865920000);

                    QueryExpression qappMat = new QueryExpression("tss_matrixapprovalmarketsize");
                    qappMat.Criteria.AddFilter(fappMat);
                    qappMat.Orders.Add(new Microsoft.Xrm.Sdk.Query.OrderExpression("tss_priorityno", Microsoft.Xrm.Sdk.Query.OrderType.Ascending));
                    qappMat.ColumnSet = new ColumnSet(true);

                    EntityCollection en_approvalMatrixMarketSize = _DL_tss_matrixapprovalmarketsize.Select(organizationService, qappMat);

                    if (en_approvalMatrixMarketSize.Entities.Count > 0)
                    {
                        EN_marketsizeresultpss["tss_currentapproval"] = new EntityReference(_DL_tss_matrixapprovalmarketsize.EntityName, en_approvalMatrixMarketSize.Entities[0].GetAttributeValue<Guid>("tss_matrixapprovalmarketsizeid"));
                        organizationService.Update(EN_marketsizeresultpss);

                        foreach (Entity appMatrix in en_approvalMatrixMarketSize.Entities)
                        {

                            #region insert to list Approval Matrix
                            Entity ENT_approvalListMarketSize = new Entity("tss_approvallistmarketsize");
                            ENT_approvalListMarketSize["tss_approver"] = appMatrix.GetAttributeValue<EntityReference>("tss_approver");
                            ENT_approvalListMarketSize["tss_marketsizeresultpss"] = new EntityReference("tss_marketsizeresultpss", Guid.Parse(recordID));
                            ENT_approvalListMarketSize["tss_type"] = new OptionSetValue(865920000);

                            organizationService.Create(ENT_approvalListMarketSize);
                            #endregion

                            #region share records
                            ShareRecords objShare = new ShareRecords();
                            Entity en_targetUser = new Entity("systemuser", appMatrix.GetAttributeValue<EntityReference>("tss_approver").Id);

                            objShare.ShareRecord(organizationService, EN_marketsizeresultpss, en_targetUser);
                            #endregion
                        }

                        TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent objEmail = new TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent();
                        string emailSubject = EN_marketsizeresultpss.GetAttributeValue<string>("tss_name") + " is waiting for PDH approval";
                        string emailContent = "Please review and approve.";
                        objEmail.SendEmailNotif(context.UserId, en_approvalMatrixMarketSize.Entities[0].GetAttributeValue<EntityReference>("tss_approver").Id, Guid.Empty, organizationService, emailSubject, emailContent);

                    }
                    else
                    {
                        throw new InvalidWorkflowException("No Approval Matrix found. Please contact your IT administrator.");
                    }
                }

            }
            catch (Exception e)
            {
                throw new InvalidWorkflowException(e.Message.ToString());
            }

        }
        public void ApproveMarketSizeResultPSS(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {
            _DL_tss_marketsizeresultpss = new DL_tss_marketsizeresultpss();
            _DL_tss_marketsizeresultpss.tss_status = 865920002;
            _DL_tss_marketsizeresultpss.tss_approvedate = DateTime.Now.ToLocalTime();
            _DL_tss_marketsizeresultpss.Update(organizationService, context.PrimaryEntityId);

            BL_tss_marketsizeresultbranch _BL_tss_marketsizeresultbranch = new BL_tss_marketsizeresultbranch();
            Guid idMSBranch = _BL_tss_marketsizeresultbranch.GenerateMarketSizeResultBranch(organizationService, tracingService, context);

            QueryExpression qMsPss = new QueryExpression(_DL_tss_marketsizeresultpss.EntityName);
            qMsPss.ColumnSet.AddColumn("tss_branch");
            qMsPss.Criteria.AddCondition("tss_marketsizeresultpssid", ConditionOperator.Equal, context.PrimaryEntityId);
            Entity msPss = _DL_tss_marketsizeresultpss.Select(organizationService, qMsPss).Entities[0];

            FilterExpression fMat = new FilterExpression(LogicalOperator.And);
            fMat.AddCondition("tss_branch", ConditionOperator.Equal, msPss.GetAttributeValue<EntityReference>("tss_branch").Id);
            fMat.AddCondition("tss_approvaltype", ConditionOperator.Equal, 865920001);
            fMat.AddCondition("tss_approvalfor", ConditionOperator.Equal, 865920000);

            QueryExpression qMat = new QueryExpression(_DL_tss_matrixapprovalmarketsize.EntityName);
            qMat.Criteria.AddFilter(fMat);
            qMat.ColumnSet = new ColumnSet(true);
            qMat.Orders.Add(new Microsoft.Xrm.Sdk.Query.OrderExpression("tss_priorityno", Microsoft.Xrm.Sdk.Query.OrderType.Ascending));
            EntityCollection mats = _DL_tss_matrixapprovalmarketsize.Select(organizationService, qMat);

            //Compare Approved MS PSS / Total PSS in branch
            QueryExpression qAppPss = new QueryExpression(_DL_tss_marketsizeresultpss.EntityName);
            qAppPss.Criteria.AddCondition("tss_branch", ConditionOperator.Equal, msPss.GetAttributeValue<EntityReference>("tss_branch").Id);
            qAppPss.ColumnSet = new ColumnSet(true);
            EntityCollection appPss = _DL_tss_marketsizeresultpss.Select(organizationService, qAppPss);

            int app = appPss.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_status").Value == 865920002).Count();

            if (mats.Entities.Count > 0 && idMSBranch != Guid.Empty)
            {
                QueryExpression qMapping = new QueryExpression(_DL_tss_marketsizeresultmapping.EntityName);
                qMapping.ColumnSet.AddColumn("tss_keyaccount");
                qMapping.Criteria.AddCondition("tss_marketsizeresultpss", ConditionOperator.Equal, context.PrimaryEntityId);
                EntityCollection oMapping = _DL_tss_marketsizeresultmapping.Select(organizationService, qMapping);
                if (oMapping.Entities.Count > 0)
                {
                    //DL_tss_marketsizeresultmapping _DL_tss_marketsizeresultmapping = new DL_tss_marketsizeresultmapping();
                    _DL_tss_marketsizeresultmapping.tss_keyaccount = oMapping.Entities[0].GetAttributeValue<EntityReference>("tss_keyaccount").Id;
                    _DL_tss_marketsizeresultmapping.tss_marketsizeresultpss = context.PrimaryEntityId;
                    _DL_tss_marketsizeresultmapping.tss_marketsizeresultbranch = idMSBranch;
                    _DL_tss_marketsizeresultmapping.Update(organizationService, oMapping.Entities[0].Id);
                }
                else
                    throw new Exception("Market Size Result PSS not found");


                foreach (var mat in mats.Entities)
                {
                    //if (appPss.Entities.Count() != app)
                    //{
                    //Insert Approval & Share Record if all result pss approved
                    #region Insert Approval List
                    DL_tss_approvallistmarketsize _DL_tss_approvallistmarketsize = new DL_tss_approvallistmarketsize();
                    _DL_tss_approvallistmarketsize.tss_approver = mat.GetAttributeValue<EntityReference>("tss_approver").Id;
                    _DL_tss_approvallistmarketsize.tss_marketsizeresultpss = context.PrimaryEntityId;
                    _DL_tss_approvallistmarketsize.tss_marketsizeresultbranch = idMSBranch;
                    _DL_tss_approvallistmarketsize.tss_type = 865920001;
                    _DL_tss_approvallistmarketsize.Insert(organizationService);

                    #endregion
                    #region Share Record
                    ShareRecords objShare = new ShareRecords();
                    Entity target = new Entity("systemuser", mat.GetAttributeValue<EntityReference>("tss_approver").Id);
                    objShare.ShareRecord(organizationService, msPss, target);
                    #endregion
                    //}
                }

                //if (appPss.Entities.Count() != app)
                //{
                TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent objEmail = new TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent();
                string emailSubject = msPss.GetAttributeValue<string>("tss_name") + " is waiting for KORWIL approval";
                string emailContent = "Please review and approve.";
                objEmail.SendEmailNotif(context.UserId, mats.Entities[0].GetAttributeValue<EntityReference>("tss_approver").Id, Guid.Empty, organizationService, emailSubject, emailContent);

                DL_tss_marketsizeresultbranch _DL_tss_marketsizeresultbranch = new DL_tss_marketsizeresultbranch();
                Entity branchToUpdate = _DL_tss_marketsizeresultbranch.Select(organizationService, idMSBranch);
                branchToUpdate["tss_status"] = new OptionSetValue(865920001);/*Waiting Approval Korwil*/
                branchToUpdate["tss_name"] = msPss.GetAttributeValue<EntityReference>("tss_branch").Name + " " + msPss.GetAttributeValue<DateTime>("tss_activeperiodstart").ToString("ddMMyyyy") + "-" + msPss.GetAttributeValue<DateTime>("tss_activeperiodend").ToString("ddMMyyyy");
                branchToUpdate["tss_currentapproval"] = new EntityReference(mats.EntityName, mats.Entities[0].Id);
                branchToUpdate["tss_currentapprovaldate"] = DateTime.Now;
                organizationService.Update(branchToUpdate);
                //}

            }
        }

        public void ReviseMarketSizeResultPSS(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {
            #region 2018.09.17 | Update Market Size Result PSS & User
            Entity EN_marketsizeresultpss = _DL_tss_marketsizeresultpss.Select(organizationService, context.PrimaryEntityId);
            EN_marketsizeresultpss["tss_status"] = new OptionSetValue(STATUS_DISABLED);
            EN_marketsizeresultpss["tss_statusreason"] = new OptionSetValue(STATUS_REMOVED);
            pssToRevise.Entities.Add(EN_marketsizeresultpss);
            Entity EN_User = _DL_user.Select(organizationService, EN_marketsizeresultpss.GetAttributeValue<EntityReference>("ownerid").Id);
            EN_User["tss_marketsizeconfirmed"] = false;

            organizationService.Update(EN_User);
            organizationService.Update(EN_marketsizeresultpss);

            _BL_tss_mastermarketsize.Revise(organizationService, pssToRevise);

            FilterExpression f = new FilterExpression(LogicalOperator.And);
            f.AddCondition("tss_marketsizeresultpss", ConditionOperator.Equal, EN_marketsizeresultpss.Id);

            QueryExpression q = new QueryExpression(_DL_tss_marketsizeresultmapping.EntityName);
            q.ColumnSet = new ColumnSet(true);
            q.Criteria.AddFilter(f);

            EntityCollection entityMapping = _DL_tss_marketsizeresultmapping.Select(organizationService, q);
            //Guid[] keyAccountId = entityMapping.Entities.Select(x => x.Id).Distinct().ToArray();
            object[] keyAccountId = entityMapping.Entities.Select(x => (object)x.GetAttributeValue<EntityReference>("tss_keyaccount").Id).ToArray();

            f = new FilterExpression(LogicalOperator.And);
            f.AddCondition("tss_keyaccountid", ConditionOperator.In, keyAccountId);

            q = new QueryExpression(_DL_tss_keyaccount.EntityName);
            q.ColumnSet = new ColumnSet(true);
            q.Criteria.AddFilter(f);

            EntityCollection entityToRevise = _DL_tss_keyaccount.Select(organizationService, q);

            _BL_KeyAccount.ReviseKeyAccount(organizationService, tracingService, context, entityToRevise);
            #endregion

            #region 2018.09.17 | Send Email
            QueryExpression qMsPss = new QueryExpression(_DL_tss_marketsizeresultpss.EntityName);
            qMsPss.ColumnSet.AddColumn("tss_pss");
            qMsPss.Criteria.AddCondition("tss_marketsizeresultpssid", ConditionOperator.Equal, context.PrimaryEntityId);
            Entity msPss = _DL_tss_marketsizeresultpss.Select(organizationService, qMsPss).Entities[0];

            if (msPss != null)
            {
                TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent objEmail = new TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent();
                string emailSubject = msPss.GetAttributeValue<EntityReference>("tss_pss").Name + " please revise.";
                string emailContent = "Please edit.";
                objEmail.SendEmailNotif(context.UserId, msPss.GetAttributeValue<EntityReference>("tss_pss").Id, Guid.Empty, organizationService, emailSubject, emailContent);
            }
            #endregion
        }


        #region COMMENT
        /// <summary>
        /// 
        /// </summary>
        /// <param name="organizationService"></param>
        /// <param name="context"></param>
        /// <param name="_keyaccountcollection"></param>
        /// 

        //public void GenerateMarketSizeResultPSS(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        //{
        //    int totaluio = 0;
        //    int totalnonuio = 0;
        //    int totalgroupuiocommodity = 0;
        //    int totalalluio = 0;

        //    decimal totalmscommodity = 0m;
        //    decimal totalmsstandardpart = 0m;
        //    decimal totalamountms = 0m;

        //    DateTime startPeriodMS = new DateTime();
        //    DateTime endPeriodMS = new DateTime();
        //    DateTime startDtMS = new DateTime();
        //    DateTime endDtMS = new DateTime();

        //    QueryExpression qUser = new QueryExpression(_DL_user.EntityName);
        //    qUser.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, context.UserId);
        //    qUser.ColumnSet.AddColumn("businessunitid");
        //    Entity user = _DL_user.Select(organizationService, qUser).Entities[0];

        //    FilterExpression fMs = new FilterExpression(LogicalOperator.And);
        //    //fMs.AddCondition("tss_unittype", ConditionOperator.NotNull);
        //    fMs.AddCondition("tss_pss", ConditionOperator.Equal, context.UserId);
        //    fMs.AddCondition("tss_status", ConditionOperator.Equal, STATUS_COMPLETED_MS);
        //    //2018.09.17 - start = lessequal & end = greaterequal
        //    //fMs.AddCondition("tss_activeperiodsend", ConditionOperator.LessEqual, DateTime.Now);
        //    //fMs.AddCondition("tss_activeperiodstart", ConditionOperator.GreaterEqual, DateTime.Now);
        //    fMs.AddCondition("tss_activeperiodsend", ConditionOperator.GreaterEqual, DateTime.Now);
        //    fMs.AddCondition("tss_activeperiodstart", ConditionOperator.LessEqual, DateTime.Now);

        //    QueryExpression qMS = new QueryExpression(_DL_tss_mastermarketsize.EntityName);
        //    qMS.Criteria.AddFilter(fMs);
        //    qMS.ColumnSet = new ColumnSet(true);
        //    EntityCollection ms = _DL_tss_mastermarketsize.Select(organizationService, qMS);

        //    if (ms.Entities.Count > 0)
        //    {
        //        object[] msIds = ms.Entities.Select(x => (object)x.Id).ToArray();

        //        QueryExpression qMSLines = new QueryExpression(_DL_tss_mastermarketsizelines.EntityName);
        //        qMSLines.LinkEntities.Add(new LinkEntity(_DL_tss_mastermarketsizelines.EntityName, _DL_tss_mastermarketsizesublines.EntityName, "tss_mastermarketsizelinesid", "tss_mastermslinesref", JoinOperator.Inner));
        //        qMSLines.LinkEntities[0].Columns = new ColumnSet(true);
        //        qMSLines.LinkEntities[0].EntityAlias = "tss_mastermarketsizesublines";
        //        qMSLines.Criteria.AddCondition("tss_mastermarketsizeref", ConditionOperator.In, msIds);
        //        qMSLines.ColumnSet = new ColumnSet(true);
        //        EntityCollection msLines = _DL_tss_mastermarketsizelines.Select(organizationService, qMSLines);

        //        Guid[] custs = ms.Entities.GroupBy(x => x.GetAttributeValue<EntityReference>("tss_customer").Id).Select(x => x.First().GetAttributeValue<EntityReference>("tss_customer").Id).ToArray();

        //        for (int i = 0; i < custs.Count(); i++)
        //        {
        //            QueryExpression qCust = new QueryExpression(_DL_account.EntityName);
        //            qCust.Criteria.AddCondition("accountid", ConditionOperator.Equal, custs[i]);
        //            qCust.ColumnSet.AddColumn("parentaccountid");
        //            Entity cust = _DL_account.Select(organizationService, qCust).Entities[0];

        //            List<Entity> current = ms.Entities.Where(x => x.GetAttributeValue<EntityReference>("tss_customer").Id == custs[i]).ToList();
        //            totaluio = current.Where(x => x.GetAttributeValue<OptionSetValue>("tss_unittype").Value == UNITTYPE_UIO).Count();
        //            totalnonuio = current.Where(x => x.GetAttributeValue<OptionSetValue>("tss_unittype").Value == UNITTYPE_NONUIO).Count();
        //            totalgroupuiocommodity = current.Where(x => x.GetAttributeValue<OptionSetValue>("tss_unittype").Value == UNITTYPE_COMMODITY).Count();
        //            totalalluio = totaluio + totalnonuio + totalgroupuiocommodity;

        //            if (msLines.Entities.Count > 0)
        //            {
        //                Guid[] idsUIO = current.Where(x => x.GetAttributeValue<OptionSetValue>("tss_unittype").Value == UNITTYPE_UIO || x.GetAttributeValue<OptionSetValue>("tss_unittype").Value == UNITTYPE_NONUIO).Select(x => x.Id).ToArray();
        //                Guid[] idsComm = current.Where(x => x.GetAttributeValue<OptionSetValue>("tss_unittype").Value == UNITTYPE_COMMODITY).Select(x => x.Id).ToArray();

        //                totalmscommodity = idsComm.Count() > 0 ? msLines.Entities
        //                                   .Where(x => idsComm.Contains(x.GetAttributeValue<EntityReference>("tss_mastermarketsizeref").Id))
        //                                   .Sum(x => (int)x.GetAttributeValue<AliasedValue>("tss_mastermarketsizesublines.tss_qty").Value * ((Money)x.GetAttributeValue<AliasedValue>("tss_mastermarketsizesublines.tss_price").Value).Value) : 0m;
        //                totalmsstandardpart = idsUIO.Count() > 0 ? msLines.Entities
        //                                      .Where(x => idsUIO.Contains(x.GetAttributeValue<EntityReference>("tss_mastermarketsizeref").Id))
        //                                      .Sum(x => (int)x.GetAttributeValue<AliasedValue>("tss_mastermarketsizesublines.tss_qty").Value * ((Money)x.GetAttributeValue<AliasedValue>("tss_mastermarketsizesublines.tss_price").Value).Value) : 0m;

        //                totalamountms = totalmscommodity + totalmsstandardpart;
        //            }

        //            QueryExpression qSetup = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
        //            qSetup.ColumnSet = new ColumnSet(true);
        //            EntityCollection setups = _DL_tss_sparepartsetup.Select(organizationService, qSetup);

        //            if (setups.Entities.Count > 0)
        //            {
        //                //TBA
        //                startPeriodMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems");
        //                endPeriodMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_enddatems");
        //            }
        //            FilterExpression fKA = new FilterExpression(LogicalOperator.And);
        //            fKA.AddCondition("tss_pss", ConditionOperator.Equal, context.UserId);
        //            fKA.AddCondition("tss_customer", ConditionOperator.Equal, custs[i]);
        //            //2018.09.17 - start = lessequal & end = greaterequal
        //            //fKA.AddCondition("tss_activeenddate", ConditionOperator.LessEqual, DateTime.Now);
        //            //fKA.AddCondition("tss_activestartdate", ConditionOperator.GreaterEqual, DateTime.Now);
        //            fKA.AddCondition("tss_activeenddate", ConditionOperator.GreaterEqual, DateTime.Now);
        //            fKA.AddCondition("tss_activestartdate", ConditionOperator.LessEqual, DateTime.Now);

        //            QueryExpression qKA = new QueryExpression(_DL_tss_keyaccount.EntityName);
        //            qKA.Criteria.AddFilter(fKA);
        //            qKA.ColumnSet.AddColumn("tss_version");
        //            EntityCollection ka = _DL_tss_keyaccount.Select(organizationService, qKA);

        //            if (ka.Entities.Count > 0 && setups.Entities.Count > 0)
        //            {
        //                if (ka.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_version").Value == REVISION).Count() == 0)
        //                {
        //                    startDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems");
        //                    endDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_enddatems");
        //                }
        //                else
        //                {
        //                    startDtMS = new DateTime(setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems").Year, setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems").Month, 1).AddMonths(setups.Entities[0].GetAttributeValue<int>("tss_evaluationms"));
        //                    endDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_enddatems");
        //                }
        //            }

        //            Guid _DL_tss_marketsizeresultpss_id;

        //            _DL_tss_marketsizeresultpss = new DL_tss_marketsizeresultpss();
        //            _DL_tss_marketsizeresultpss.tss_pss = context.UserId;
        //            _DL_tss_marketsizeresultpss.tss_branch = user.GetAttributeValue<EntityReference>("businessunitid").Id;
        //            _DL_tss_marketsizeresultpss.tss_customer = custs[i];
        //            if (cust.Contains("parentaccountid"))
        //            {
        //                _DL_tss_marketsizeresultpss.tss_customergroup = cust.GetAttributeValue<EntityReference>("parentaccountid").Id;
        //            }
        //            _DL_tss_marketsizeresultpss.tss_totaluio = totaluio;
        //            _DL_tss_marketsizeresultpss.tss_totalnonuio = totalnonuio;
        //            _DL_tss_marketsizeresultpss.tss_totalgroupuiocommodity = totalgroupuiocommodity;
        //            _DL_tss_marketsizeresultpss.tss_totalalluio = totalalluio;
        //            _DL_tss_marketsizeresultpss.tss_totalmscommodity = totalmscommodity;
        //            _DL_tss_marketsizeresultpss.tss_totalmsstandardpart = totalmsstandardpart;
        //            _DL_tss_marketsizeresultpss.tss_totalamountms = totalamountms;
        //            _DL_tss_marketsizeresultpss.tss_msperiodstart = startPeriodMS;
        //            _DL_tss_marketsizeresultpss.tss_msperiodend = endPeriodMS;
        //            _DL_tss_marketsizeresultpss.tss_activeperiodstart = startDtMS;
        //            _DL_tss_marketsizeresultpss.tss_activeperiodsend = endDtMS;
        //            _DL_tss_marketsizeresultpss.tss_status = STATUS_DRAFT;
        //            _DL_tss_marketsizeresultpss.tss_statusreason = STATUS_OPEN;
        //            //_DL_tss_marketsizeresultpss_id = _DL_tss_marketsizeresultpss.Insert(organizationService);

        //            DL_tss_marketsizeresultmapping _DL_tss_marketsizeresultmapping = new DL_tss_marketsizeresultmapping();
        //            _DL_tss_marketsizeresultmapping.tss_keyaccount = ka.Entities[0].Id;
        //            //_DL_tss_marketsizeresultmapping.tss_marketsizeresultpss = _DL_tss_marketsizeresultpss_id; // context.PrimaryEntityId;
        //            //_DL_tss_marketsizeresultmapping.Insert(organizationService);
        //        }
        //    }
        //}


        //public void GenerateMarketSizeResultPSS_New(IOrganizationService organizationService, IWorkflowContext context, EntityCollection _keyaccountcollection)
        //{
        //    object[] _keyaccountid = _keyaccountcollection.Entities.Select(x => (object)x.Id).ToArray();
        //    Guid[] _customer = _keyaccountcollection.Entities.GroupBy(x => x.GetAttributeValue<EntityReference>("tss_customer").Id).Select(x => x.First().GetAttributeValue<EntityReference>("tss_customer").Id).ToArray();

        //    int totaluio = 0;
        //    int totalnonuio = 0;
        //    int totalgroupuiocommodity = 0;
        //    int totalalluio = 0;

        //    decimal totalmscommodity = 0m;
        //    decimal totalmsstandardpart = 0m;
        //    decimal totalmsstandardpartMT45 = 0m;
        //    decimal totalamountms = 0m;

        //    DateTime startPeriodMS = new DateTime();
        //    DateTime endPeriodMS = new DateTime();
        //    DateTime startDtMS = new DateTime();
        //    DateTime endDtMS = new DateTime();

        //    QueryExpression qUser = new QueryExpression(_DL_user.EntityName);
        //    qUser.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, context.UserId);
        //    qUser.ColumnSet.AddColumn("businessunitid");
        //    Entity user = _DL_user.Select(organizationService, qUser).Entities[0];

        //    //FilterExpression fMs = new FilterExpression(LogicalOperator.And);
        //    //fMs.AddCondition("tss_unittype", ConditionOperator.NotNull);
        //    //fMs.AddCondition("tss_pss", ConditionOperator.Equal, context.UserId);
        //    //fMs.AddCondition("tss_status", ConditionOperator.Equal, STATUS_COMPLETED_MS);
        //    //fMs.AddCondition("tss_activeperiodsend", ConditionOperator.GreaterEqual, DateTime.Now);
        //    //fMs.AddCondition("tss_activeperiodstart", ConditionOperator.LessEqual, DateTime.Now);

        //    //QueryExpression qMS = new QueryExpression(_DL_tss_mastermarketsize.EntityName);
        //    //qMS.Criteria.AddFilter(fMs);
        //    //qMS.ColumnSet = new ColumnSet(true);
        //    //EntityCollection ms = _DL_tss_mastermarketsize.Select(organizationService, qMS);

        //    FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
        //    _filterexpression.AddCondition("tss_unittype", ConditionOperator.NotNull);
        //    _filterexpression.AddCondition("tss_pss", ConditionOperator.Equal, context.UserId);
        //    _filterexpression.AddCondition("tss_status", ConditionOperator.Equal, STATUS_COMPLETED_MS);
        //    _filterexpression.AddCondition("tss_keyaccountid", ConditionOperator.In, _keyaccountid);
        //    _filterexpression.AddCondition("tss_ismsresultpssgenerated", ConditionOperator.Equal, false);

        //    QueryExpression _queryexpression = new QueryExpression(_DL_tss_mastermarketsize.EntityName);
        //    _queryexpression.Criteria.AddFilter(_filterexpression);
        //    _queryexpression.ColumnSet = new ColumnSet(true);
        //    EntityCollection ms = _DL_tss_mastermarketsize.Select(organizationService, _queryexpression);

        //    if (ms.Entities.Count > 0)
        //    {
        //        object[] msIds = ms.Entities.Select(x => (object)x.Id).ToArray();

        //        QueryExpression qMSLines = new QueryExpression(_DL_tss_mastermarketsizelines.EntityName);
        //        qMSLines.LinkEntities.Add(new LinkEntity(_DL_tss_mastermarketsizelines.EntityName, _DL_tss_mastermarketsizesublines.EntityName, "tss_mastermarketsizelinesid", "tss_mastermslinesref", JoinOperator.Inner));
        //        qMSLines.LinkEntities[0].Columns = new ColumnSet(true);
        //        qMSLines.LinkEntities[0].EntityAlias = "tss_mastermarketsizesublines";
        //        qMSLines.Criteria.AddCondition("tss_mastermarketsizeref", ConditionOperator.In, msIds);
        //        //qMSLines.Criteria.AddCondition("tss_duedate", ConditionOperator.NotNull);
        //        qMSLines.ColumnSet = new ColumnSet(true);
        //        EntityCollection msLines = _DL_tss_mastermarketsizelines.Select(organizationService, qMSLines);

        //        //Guid[] custs = _customer;

        //        for (int i = 0; i < _customer.Count(); i++)
        //        {
        //            QueryExpression qCust = new QueryExpression(_DL_account.EntityName);
        //            qCust.Criteria.AddCondition("accountid", ConditionOperator.Equal, _customer[i]);
        //            qCust.ColumnSet.AddColumn("parentaccountid");
        //            Entity cust = _DL_account.Select(organizationService, qCust).Entities[0];

        //            List<Entity> current = ms.Entities.Where(x => x.GetAttributeValue<EntityReference>("tss_customer").Id == _customer[i]).ToList();
        //            totaluio = current.Where(x => x.GetAttributeValue<OptionSetValue>("tss_unittype").Value == UNITTYPE_UIO && x.GetAttributeValue<OptionSetValue>("tss_status").Value == STATUS_COMPLETED_MS).Count();
        //            totalnonuio = current.Where(x => x.GetAttributeValue<OptionSetValue>("tss_unittype").Value == UNITTYPE_NONUIO && x.GetAttributeValue<OptionSetValue>("tss_status").Value == STATUS_COMPLETED_MS).Count();
        //            totalgroupuiocommodity = current.Where(x => x.GetAttributeValue<OptionSetValue>("tss_unittype").Value == UNITTYPE_COMMODITY && x.GetAttributeValue<OptionSetValue>("tss_status").Value == STATUS_COMPLETED_MS).Count();
        //            totalalluio = totaluio + totalnonuio + totalgroupuiocommodity;

        //            if (msLines.Entities.Count > 0)
        //            {
        //                Guid[] idsUIO = current.Where(x => (x.GetAttributeValue<OptionSetValue>("tss_unittype").Value == UNITTYPE_UIO || x.GetAttributeValue<OptionSetValue>("tss_unittype").Value == UNITTYPE_NONUIO) && x.GetAttributeValue<OptionSetValue>("tss_status").Value == STATUS_COMPLETED_MS).Select(x => x.Id).ToArray();
        //                Guid[] idsComm = current.Where(x => x.GetAttributeValue<OptionSetValue>("tss_unittype").Value == UNITTYPE_COMMODITY).Select(x => x.Id).ToArray();

        //                totalmsstandardpart = idsUIO.Count() > 0 ? msLines.Entities
        //                                      .Where(x => idsUIO.Contains(x.GetAttributeValue<EntityReference>("tss_mastermarketsizeref").Id) && x.GetAttributeValue<DateTime>("tss_duedate") != DateTime.MinValue && (x.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD1 || x.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD2 || x.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD3))
        //                                      .Sum(x => (int)x.GetAttributeValue<AliasedValue>("tss_mastermarketsizesublines.tss_qty").Value * ((Money)x.GetAttributeValue<AliasedValue>("tss_mastermarketsizesublines.tss_price").Value).Value) : 0m;

        //                totalmsstandardpartMT45 = idsUIO.Count() > 0 ? msLines.Entities
        //                                          .Where(x => idsUIO.Contains(x.GetAttributeValue<EntityReference>("tss_mastermarketsizeref").Id) && (x.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD4 || x.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD5))
        //                                          .Sum(x => (int)x.GetAttributeValue<AliasedValue>("tss_mastermarketsizesublines.tss_qty").Value * ((Money)x.GetAttributeValue<AliasedValue>("tss_mastermarketsizesublines.tss_price").Value).Value) : 0m;

        //                foreach (Guid item in idsComm)
        //                {
        //                    FilterExpression fappMat = new FilterExpression(LogicalOperator.And);
        //                    fappMat.AddCondition("tss_mastermarketsizeid", ConditionOperator.Equal, item);

        //                    QueryExpression qappMat = new QueryExpression("tss_mastermarketsizesublines");
        //                    qappMat.Criteria.AddFilter(fappMat);
        //                    qappMat.ColumnSet = new ColumnSet(true);

        //                    EntityCollection oResult = organizationService.RetrieveMultiple(qappMat);

        //                    foreach (var oRow in oResult.Entities)
        //                    {
        //                        totalmscommodity += (decimal)(oRow.GetAttributeValue<int>("tss_qty") * oRow.GetAttributeValue<Money>("tss_price").Value);
        //                    }
        //                }

        //                totalamountms = totalmscommodity + totalmsstandardpart + totalmsstandardpartMT45;
        //                totalmsstandardpart += totalmsstandardpartMT45;
        //            }
        //            else
        //            {
        //                Guid[] idsComm = current.Where(x => x.GetAttributeValue<OptionSetValue>("tss_unittype").Value == UNITTYPE_COMMODITY).Select(x => x.Id).ToArray();

        //                foreach (Guid item in idsComm)
        //                {
        //                    FilterExpression fappMat = new FilterExpression(LogicalOperator.And);
        //                    fappMat.AddCondition("tss_mastermarketsizeid", ConditionOperator.Equal, item);

        //                    QueryExpression qappMat = new QueryExpression("tss_mastermarketsizesublines");
        //                    qappMat.Criteria.AddFilter(fappMat);
        //                    qappMat.ColumnSet = new ColumnSet(true);

        //                    EntityCollection oResult = organizationService.RetrieveMultiple(qappMat);

        //                    foreach (var oRow in oResult.Entities)
        //                    {
        //                        totalmscommodity += (decimal)(oRow.GetAttributeValue<int>("tss_qty") * oRow.GetAttributeValue<Money>("tss_price").Value);
        //                    }
        //                }

        //                totalamountms = totalmscommodity;
        //            }

        //            QueryExpression qSetup = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
        //            qSetup.ColumnSet = new ColumnSet(true);
        //            EntityCollection setups = _DL_tss_sparepartsetup.Select(organizationService, qSetup);

        //            if (setups.Entities.Count > 0)
        //            {
        //                //TBA
        //                startPeriodMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems");
        //                endPeriodMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_enddatems");
        //            }
        //            //FilterExpression fKA = new FilterExpression(LogicalOperator.And);
        //            //fKA.AddCondition("tss_pss", ConditionOperator.Equal, context.UserId);
        //            //fKA.AddCondition("tss_customer", ConditionOperator.Equal, _customer[i]);
        //            //fKA.AddCondition("tss_activeenddate", ConditionOperator.GreaterEqual, DateTime.Now);
        //            //fKA.AddCondition("tss_activestartdate", ConditionOperator.LessEqual, DateTime.Now);
        //            //fKA.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true);
        //            //fKA.AddCondition("tss_status", ConditionOperator.Equal, 865920001);

        //            //QueryExpression qKA = new QueryExpression(_DL_tss_keyaccount.EntityName);
        //            //qKA.Criteria.AddFilter(fKA);
        //            //qKA.ColumnSet.AddColumn("tss_version");
        //            //qKA.AddOrder("createdon", OrderType.Descending);
        //            //EntityCollection ka = _DL_tss_keyaccount.Select(organizationService, qKA);

        //            _filterexpression = new FilterExpression(LogicalOperator.And);
        //            _filterexpression.AddCondition("tss_keyaccountid", ConditionOperator.In, _keyaccountid);

        //            _queryexpression = new QueryExpression("tss_keyaccount");
        //            _queryexpression.Criteria.AddFilter(_filterexpression);
        //            _queryexpression.ColumnSet = new ColumnSet(true);
        //            _queryexpression.AddOrder("createdon", OrderType.Descending);
        //            EntityCollection ka = organizationService.RetrieveMultiple(_queryexpression);

        //            if (ka.Entities.Count > 0 && setups.Entities.Count > 0)
        //            {
        //                if (ka.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_version").Value == REVISION).Count() == 0)
        //                {
        //                    startDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems");
        //                    endDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_enddatems");
        //                }
        //                else
        //                {
        //                    startDtMS = new DateTime(setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems").Year, setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems").Month, 1).AddMonths(setups.Entities[0].GetAttributeValue<int>("tss_evaluationms"));
        //                    endDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_enddatems");
        //                }
        //            }

        //            Guid _DL_tss_marketsizeresultpss_id;

        //            _DL_tss_marketsizeresultpss = new DL_tss_marketsizeresultpss();
        //            _DL_tss_marketsizeresultpss.tss_pss = context.UserId;
        //            _DL_tss_marketsizeresultpss.tss_branch = user.GetAttributeValue<EntityReference>("businessunitid").Id;
        //            _DL_tss_marketsizeresultpss.tss_customer = _customer[i];
        //            if (cust.Contains("parentaccountid"))
        //            {
        //                _DL_tss_marketsizeresultpss.tss_customergroup = cust.GetAttributeValue<EntityReference>("parentaccountid").Id;
        //            }
        //            _DL_tss_marketsizeresultpss.tss_totaluio = totaluio;
        //            _DL_tss_marketsizeresultpss.tss_totalnonuio = totalnonuio;
        //            _DL_tss_marketsizeresultpss.tss_totalgroupuiocommodity = totalgroupuiocommodity;
        //            _DL_tss_marketsizeresultpss.tss_totalalluio = totalalluio;
        //            _DL_tss_marketsizeresultpss.tss_totalmscommodity = totalmscommodity;
        //            _DL_tss_marketsizeresultpss.tss_totalmsstandardpart = totalmsstandardpart;
        //            _DL_tss_marketsizeresultpss.tss_totalamountms = totalamountms;
        //            _DL_tss_marketsizeresultpss.tss_msperiodstart = startDtMS; // startPeriodMS;
        //            _DL_tss_marketsizeresultpss.tss_msperiodend = endDtMS; // endPeriodMS;
        //            _DL_tss_marketsizeresultpss.tss_activeperiodstart = startDtMS;
        //            _DL_tss_marketsizeresultpss.tss_activeperiodsend = endDtMS;
        //            _DL_tss_marketsizeresultpss.tss_status = STATUS_DRAFT;
        //            _DL_tss_marketsizeresultpss.tss_statusreason = STATUS_OPEN;
        //            _DL_tss_marketsizeresultpss_id = _DL_tss_marketsizeresultpss.Insert(organizationService);

        //            foreach (var kaitem in ka.Entities)
        //            {
        //                Entity _ka = kaitem;

        //                DL_tss_marketsizeresultmapping _DL_tss_marketsizeresultmapping = new DL_tss_marketsizeresultmapping();
        //                _DL_tss_marketsizeresultmapping.tss_keyaccount = _ka.Id;
        //                _DL_tss_marketsizeresultmapping.tss_marketsizeresultpss = _DL_tss_marketsizeresultpss_id; // context.PrimaryEntityId;

        //                _DL_tss_marketsizeresultmapping.Insert(organizationService);
        //            }

        //            // CREATE MARKET SIZE RESULT PSS - DETAIL - KA UIO & NON UIO
        //            _filterexpression = new FilterExpression(LogicalOperator.And);
        //            _filterexpression.AddCondition("tss_keyaccountid", ConditionOperator.Equal, ka.Entities[0].Id);

        //            _queryexpression = new QueryExpression("tss_kauio");
        //            _queryexpression.Criteria.AddFilter(_filterexpression);
        //            _queryexpression.ColumnSet = new ColumnSet(true);
        //            EntityCollection _kauiocollection = organizationService.RetrieveMultiple(_queryexpression);

        //            foreach (var _kauioitem in _kauiocollection.Entities)
        //            {
        //                Entity _marketsizeresultpssdetail = new Entity("tss_marketsizeresultpssdetail");

        //                Entity _population = organizationService.Retrieve("new_population", _kauioitem.GetAttributeValue<EntityReference>("tss_serialnumber").Id, new ColumnSet(true));
        //                bool _populationstatus = _population.GetAttributeValue<bool>("tss_populationstatus");

        //                if (_populationstatus)
        //                    _marketsizeresultpssdetail["tss_uiotype"] = new OptionSetValue(865920000); // KA UIO
        //                else
        //                    _marketsizeresultpssdetail["tss_uiotype"] = new OptionSetValue(865920001); // KA Non UIO

        //                _marketsizeresultpssdetail["tss_kauioid"] = new EntityReference("tss_kauio", _kauioitem.Id);

        //                _marketsizeresultpssdetail["tss_serialnumber"] = new EntityReference("new_population", _population.Id);

        //                _marketsizeresultpssdetail["tss_marketsizeresultpssid"] = new EntityReference("tss_marketsizeresultpss", _DL_tss_marketsizeresultpss_id);
        //                //_marketsizeresultpssdetail["tss_currenthourmeter"] = _kauioitem.Attributes.Contains("tss_currenthourmeter") ? _kauioitem.GetAttributeValue<decimal>("tss_currenthourmeter") : nulldecimal;
        //                //_marketsizeresultpssdetail["tss_currenthourmeterdate"] = _kauioitem.GetAttributeValue<DateTime>("tss_currenthourmeterdate") == DateTime.MinValue ? nulldatetime : _kauioitem.GetAttributeValue<DateTime>("tss_currenthourmeterdate").ToLocalTime().Date;
        //                //_marketsizeresultpssdetail["tss_lasthourmeter"] = _kauioitem.Attributes.Contains("tss_lasthourmeter") ? _kauioitem.GetAttributeValue<decimal>("tss_lasthourmeter") : nulldecimal;
        //                //_marketsizeresultpssdetail["tss_lasthourmeterdate"] = _kauioitem.GetAttributeValue<DateTime>("tss_lasthourmeterdate") == DateTime.MinValue ? nulldatetime : _kauioitem.GetAttributeValue<DateTime>("tss_lasthourmeterdate").ToLocalTime().Date;
        //                //_marketsizeresultpssdetail["tss_deliverydate"] = _kauioitem.GetAttributeValue<DateTime>("tss_deliverydate") == DateTime.MinValue ? nulldatetime : _kauioitem.GetAttributeValue<DateTime>("tss_deliverydate").ToLocalTime().Date;
        //                //_marketsizeresultpssdetail["tss_calculatetoms"] = _kauioitem.GetAttributeValue<bool>("tss_calculatetoms");
        //                //_marketsizeresultpssdetail["tss_calculatestatus"] = _kauioitem.GetAttributeValue<bool>("tss_calculatestatus");

        //                organizationService.Create(_marketsizeresultpssdetail);
        //            }

        //            // CREATE MARKET SIZE RESULT PSS - DETAIL - KA GROUP UIO COMMODITY
        //            _filterexpression = new FilterExpression(LogicalOperator.And);
        //            _filterexpression.AddCondition("tss_keyaccountid", ConditionOperator.In, _keyaccountid);

        //            _queryexpression = new QueryExpression("tss_kagroupuiocommodity");
        //            _queryexpression.Criteria.AddFilter(_filterexpression);
        //            _queryexpression.ColumnSet = new ColumnSet(true);
        //            EntityCollection _kagroupuiocommoditycollection = organizationService.RetrieveMultiple(_queryexpression);

        //            foreach (var _kagroupuiocommodityitem in _kagroupuiocommoditycollection.Entities)
        //            {
        //                Entity _marketsizeresultpssdetail = new Entity("tss_marketsizeresultpssdetail");

        //                _marketsizeresultpssdetail["tss_uiotype"] = new OptionSetValue(865920002);
        //                _marketsizeresultpssdetail["tss_marketsizeresultpssid"] = new EntityReference("tss_marketsizeresultpss", _DL_tss_marketsizeresultpss_id);
        //                _marketsizeresultpssdetail["tss_groupuiocommodity"] = _kagroupuiocommodityitem.GetAttributeValue<EntityReference>("tss_groupuiocommodity");
        //                //_marketsizeresultpssdetail["tss_calculatetoms"] = _kagroupuiocommodityitem.GetAttributeValue<bool>("tss_calculatetoms");
        //                //_marketsizeresultpssdetail["tss_calculatestatus"] = _kagroupuiocommodityitem.GetAttributeValue<bool>("tss_calculatestatus");

        //                _marketsizeresultpssdetail["tss_kagroupuiocommodityid"] = new EntityReference("tss_kagroupuiocommodity", _kagroupuiocommodityitem.Id);

        //                organizationService.Create(_marketsizeresultpssdetail);
        //            }
        //        }

        //        // UPDATE MASTER MARKET SIZE field 'tss_ismsresultpssgenerated' menjadi 'TRUE' SUPAYA TIDAK BISA GENERATE LEBIH DARI 1x.
        //        //foreach (var _mastermarketsizecollection in ms.Entities)
        //        //{
        //        //    Entity _mastermarketsize = _mastermarketsizecollection;

        //        //    _mastermarketsize.Attributes["tss_ismsresultpssgenerated"] = true;

        //        //    organizationService.Update(_mastermarketsize);
        //        //}
        //    }
        //}



        //public Guid GenerateMSPSSKAUIO(IOrganizationService organizationService, IWorkflowContext context, Guid customerID, Guid keyAccountID, Guid masterMarketSize)
        //{
        //    int totaluio = 0;
        //    int totalnonuio = 0;
        //    int totalgroupuiocommodity = 0;
        //    int totalalluio = 0;

        //    decimal totalmscommodity = 0m;
        //    decimal totalmsstandardpart = 0m;
        //    decimal totalamountms = 0m;

        //    DateTime startPeriodMS = new DateTime();
        //    DateTime endPeriodMS = new DateTime();
        //    DateTime startDtMS = new DateTime();
        //    DateTime endDtMS = new DateTime();

        //    QueryExpression qUser = new QueryExpression(_DL_user.EntityName);
        //    qUser.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, context.UserId);
        //    qUser.ColumnSet.AddColumn("businessunitid");
        //    Entity user = _DL_user.Select(organizationService, qUser).Entities[0];

        //    QueryExpression qCust = new QueryExpression(_DL_account.EntityName);
        //    qCust.Criteria.AddCondition("accountid", ConditionOperator.Equal, customerID);
        //    qCust.ColumnSet.AddColumn("parentaccountid");
        //    Entity cust = _DL_account.Select(organizationService, qCust).Entities[0];

        //    QueryExpression qSetup = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
        //    qSetup.ColumnSet = new ColumnSet(true);
        //    EntityCollection setups = _DL_tss_sparepartsetup.Select(organizationService, qSetup);

        //    if (setups.Entities.Count > 0)
        //    {
        //        startPeriodMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems");
        //        endPeriodMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_enddatems");
        //    }

        //    FilterExpression fKA = new FilterExpression(LogicalOperator.And);
        //    fKA.AddCondition("tss_pss", ConditionOperator.Equal, context.UserId);
        //    fKA.AddCondition("tss_customer", ConditionOperator.Equal, customerID);
        //    fKA.AddCondition("tss_activeenddate", ConditionOperator.GreaterEqual, DateTime.Now);
        //    fKA.AddCondition("tss_activestartdate", ConditionOperator.LessEqual, DateTime.Now);

        //    QueryExpression qKA = new QueryExpression(_DL_tss_keyaccount.EntityName);
        //    qKA.Criteria.AddFilter(fKA);
        //    qKA.ColumnSet.AddColumn("tss_version");
        //    EntityCollection ka = _DL_tss_keyaccount.Select(organizationService, qKA);

        //    if (ka.Entities.Count > 0 && setups.Entities.Count > 0)
        //    {
        //        if (ka.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_version").Value == REVISION).Count() == 0)
        //        {
        //            startDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems");
        //            endDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_enddatems");
        //        }
        //        else
        //        {
        //            startDtMS = new DateTime(setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems").Year, setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems").Month, 1).AddMonths(setups.Entities[0].GetAttributeValue<int>("tss_evaluationms"));
        //            endDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_enddatems");
        //        }
        //    }

        //    Guid _DL_tss_marketsizeresultpss_id;

        //    _DL_tss_marketsizeresultpss = new DL_tss_marketsizeresultpss();
        //    _DL_tss_marketsizeresultpss.tss_pss = context.UserId;
        //    _DL_tss_marketsizeresultpss.tss_branch = user.GetAttributeValue<EntityReference>("businessunitid").Id;
        //    _DL_tss_marketsizeresultpss.tss_customer = customerID;
        //    if (cust.Contains("parentaccountid"))
        //    {
        //        _DL_tss_marketsizeresultpss.tss_customergroup = cust.GetAttributeValue<EntityReference>("parentaccountid").Id;
        //    }
        //    _DL_tss_marketsizeresultpss.tss_totaluio = totaluio;
        //    _DL_tss_marketsizeresultpss.tss_totalnonuio = totalnonuio;
        //    _DL_tss_marketsizeresultpss.tss_totalgroupuiocommodity = totalgroupuiocommodity;
        //    _DL_tss_marketsizeresultpss.tss_totalalluio = totalalluio;
        //    _DL_tss_marketsizeresultpss.tss_totalmscommodity = totalmscommodity;
        //    _DL_tss_marketsizeresultpss.tss_totalmsstandardpart = totalmsstandardpart;
        //    _DL_tss_marketsizeresultpss.tss_totalamountms = totalamountms;
        //    _DL_tss_marketsizeresultpss.tss_msperiodstart = startPeriodMS;
        //    _DL_tss_marketsizeresultpss.tss_msperiodend = endPeriodMS;
        //    _DL_tss_marketsizeresultpss.tss_activeperiodstart = startDtMS;
        //    _DL_tss_marketsizeresultpss.tss_activeperiodsend = endDtMS;
        //    _DL_tss_marketsizeresultpss.tss_status = STATUS_DRAFT;
        //    _DL_tss_marketsizeresultpss.tss_statusreason = STATUS_OPEN;
        //    _DL_tss_marketsizeresultpss_id = _DL_tss_marketsizeresultpss.Insert(organizationService);

        //    DL_tss_marketsizeresultmapping _DL_tss_marketsizeresultmapping = new DL_tss_marketsizeresultmapping();
        //    _DL_tss_marketsizeresultmapping.tss_keyaccount = keyAccountID;
        //    _DL_tss_marketsizeresultmapping.tss_marketsizeresultpss = _DL_tss_marketsizeresultpss_id; // context.PrimaryEntityId;
        //    _DL_tss_marketsizeresultmapping.Insert(organizationService);

        //    return _DL_tss_marketsizeresultpss_id;
        //}


        //public Guid GenerateMSPSSGroupUIO(IOrganizationService organizationService, IWorkflowContext context, Guid customerID, Guid keyAccountID, Guid masterMarketSize)
        //{
        //    int totaluio = 0;
        //    int totalnonuio = 0;
        //    int totalgroupuiocommodity = 0;
        //    int totalalluio = 0;

        //    decimal totalmscommodity = 0m;
        //    decimal totalmsstandardpart = 0m;
        //    decimal totalamountms = 0m;

        //    DateTime startPeriodMS = new DateTime();
        //    DateTime endPeriodMS = new DateTime();
        //    DateTime startDtMS = new DateTime();
        //    DateTime endDtMS = new DateTime();

        //    QueryExpression qUser = new QueryExpression(_DL_user.EntityName);
        //    qUser.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, context.UserId);
        //    qUser.ColumnSet.AddColumn("businessunitid");
        //    Entity user = _DL_user.Select(organizationService, qUser).Entities[0];

        //    QueryExpression qCust = new QueryExpression(_DL_account.EntityName);
        //    qCust.Criteria.AddCondition("accountid", ConditionOperator.Equal, customerID);
        //    qCust.ColumnSet.AddColumn("parentaccountid");
        //    Entity cust = _DL_account.Select(organizationService, qCust).Entities[0];

        //    QueryExpression qSetup = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
        //    qSetup.ColumnSet = new ColumnSet(true);
        //    EntityCollection setups = _DL_tss_sparepartsetup.Select(organizationService, qSetup);

        //    if (setups.Entities.Count > 0)
        //    {
        //        startPeriodMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems");
        //        endPeriodMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_enddatems");
        //    }

        //    FilterExpression fKA = new FilterExpression(LogicalOperator.And);
        //    fKA.AddCondition("tss_pss", ConditionOperator.Equal, context.UserId);
        //    fKA.AddCondition("tss_customer", ConditionOperator.Equal, customerID);
        //    fKA.AddCondition("tss_activeenddate", ConditionOperator.GreaterEqual, DateTime.Now);
        //    fKA.AddCondition("tss_activestartdate", ConditionOperator.LessEqual, DateTime.Now);

        //    QueryExpression qKA = new QueryExpression(_DL_tss_keyaccount.EntityName);
        //    qKA.Criteria.AddFilter(fKA);
        //    qKA.ColumnSet.AddColumn("tss_version");
        //    EntityCollection ka = _DL_tss_keyaccount.Select(organizationService, qKA);

        //    if (ka.Entities.Count > 0 && setups.Entities.Count > 0)
        //    {
        //        if (ka.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_version").Value == REVISION).Count() == 0)
        //        {
        //            startDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems");
        //            endDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_enddatems");
        //        }
        //        else
        //        {
        //            startDtMS = new DateTime(setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems").Year, setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems").Month, 1).AddMonths(setups.Entities[0].GetAttributeValue<int>("tss_evaluationms"));
        //            endDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_enddatems");
        //        }
        //    }

        //    Guid _DL_tss_marketsizeresultpss_id;

        //    _DL_tss_marketsizeresultpss = new DL_tss_marketsizeresultpss();
        //    _DL_tss_marketsizeresultpss.tss_pss = context.UserId;
        //    _DL_tss_marketsizeresultpss.tss_branch = user.GetAttributeValue<EntityReference>("businessunitid").Id;
        //    _DL_tss_marketsizeresultpss.tss_customer = customerID;
        //    if (cust.Contains("parentaccountid"))
        //    {
        //        _DL_tss_marketsizeresultpss.tss_customergroup = cust.GetAttributeValue<EntityReference>("parentaccountid").Id;
        //    }
        //    _DL_tss_marketsizeresultpss.tss_totaluio = totaluio;
        //    _DL_tss_marketsizeresultpss.tss_totalnonuio = totalnonuio;
        //    _DL_tss_marketsizeresultpss.tss_totalgroupuiocommodity = totalgroupuiocommodity;
        //    _DL_tss_marketsizeresultpss.tss_totalalluio = totalalluio;
        //    _DL_tss_marketsizeresultpss.tss_totalmscommodity = totalmscommodity;
        //    _DL_tss_marketsizeresultpss.tss_totalmsstandardpart = totalmsstandardpart;
        //    _DL_tss_marketsizeresultpss.tss_totalamountms = totalamountms;
        //    _DL_tss_marketsizeresultpss.tss_msperiodstart = startPeriodMS;
        //    _DL_tss_marketsizeresultpss.tss_msperiodend = endPeriodMS;
        //    _DL_tss_marketsizeresultpss.tss_activeperiodstart = startDtMS;
        //    _DL_tss_marketsizeresultpss.tss_activeperiodsend = endDtMS;
        //    _DL_tss_marketsizeresultpss.tss_status = STATUS_DRAFT;
        //    _DL_tss_marketsizeresultpss.tss_statusreason = STATUS_OPEN;
        //    _DL_tss_marketsizeresultpss_id = _DL_tss_marketsizeresultpss.Insert(organizationService);

        //    DL_tss_marketsizeresultmapping _DL_tss_marketsizeresultmapping = new DL_tss_marketsizeresultmapping();
        //    _DL_tss_marketsizeresultmapping.tss_keyaccount = keyAccountID;
        //    _DL_tss_marketsizeresultmapping.tss_marketsizeresultpss = _DL_tss_marketsizeresultpss_id; // context.PrimaryEntityId;
        //    _DL_tss_marketsizeresultmapping.Insert(organizationService);

        //    return _DL_tss_marketsizeresultpss_id;
        //}


        //public void CalculateUIO(IOrganizationService organizationService, IWorkflowContext context, List<Guid> msIds, Boolean IsKAUIO, Guid oPSS)
        //{
        //    int totaluio = 0;
        //    int totalnonuio = 0;
        //    int totalgroupuiocommodity = 0;
        //    int totalalluio = 0;

        //    decimal totalmscommodity = 0m;
        //    decimal totalmsstandardpart = 0m;
        //    decimal totalamountms = 0m;

        //    List<Guid> kauioID = new List<Guid>();
        //    List<Guid> commodityID = new List<Guid>();

        //    foreach (var item in msIds)
        //    {
        //        Entity marketSize = _DL_tss_mastermarketsize.Select(organizationService, item);

        //        if (marketSize.Attributes.Contains("tss_unittype"))
        //        {
        //            if (IsKAUIO)
        //            {
        //                kauioID.Add(item);

        //                if (marketSize.GetAttributeValue<OptionSetValue>("tss_unittype").Value == UNITTYPE_UIO)
        //                {
        //                    totaluio = totaluio + 1;
        //                }
        //                else if (marketSize.GetAttributeValue<OptionSetValue>("tss_unittype").Value == UNITTYPE_NONUIO)
        //                {
        //                    totalnonuio = totalnonuio + 1;
        //                }
        //            }
        //            else
        //            {
        //                commodityID.Add(item);

        //                totalgroupuiocommodity = totalgroupuiocommodity + 1;
        //            }
        //        }
        //    }

        //    totalalluio = totaluio + totalnonuio + totalgroupuiocommodity;

        //    object[] msIdsss = new object[msIds.Count()];

        //    for (int i = 0; i < msIds.Count(); i++)
        //    {
        //        if (msIds[i] != null)
        //        {
        //            msIdsss[i] = msIds[i];
        //        }
        //    }

        //    if (msIdsss.Count() > 0)
        //    {
        //        QueryExpression qMSLines = new QueryExpression(_DL_tss_mastermarketsizelines.EntityName);
        //        qMSLines.LinkEntities.Add(new LinkEntity(_DL_tss_mastermarketsizelines.EntityName, _DL_tss_mastermarketsizesublines.EntityName, "tss_mastermarketsizelinesid", "tss_mastermslinesref", JoinOperator.Inner));
        //        qMSLines.LinkEntities[0].Columns = new ColumnSet(true);
        //        qMSLines.LinkEntities[0].EntityAlias = "tss_mastermarketsizesublines";
        //        qMSLines.Criteria.AddCondition("tss_mastermarketsizeref", ConditionOperator.In, msIdsss);
        //        qMSLines.ColumnSet = new ColumnSet(true);
        //        EntityCollection msLines = _DL_tss_mastermarketsizelines.Select(organizationService, qMSLines);

        //        if (msLines.Entities.Count > 0)
        //        {
        //            if (kauioID.Count() > 0)
        //            {
        //                totalmscommodity = kauioID.Count() > 0 ? msLines.Entities
        //                               .Where(x => kauioID.Contains(x.GetAttributeValue<EntityReference>("tss_mastermarketsizeref").Id))
        //                               .Sum(x => (int)x.GetAttributeValue<AliasedValue>("tss_mastermarketsizesublines.tss_qty").Value * ((Money)x.GetAttributeValue<AliasedValue>("tss_mastermarketsizesublines.tss_price").Value).Value) : 0m;
        //            }

        //            if (commodityID.Count() > 0)
        //            {
        //                totalmsstandardpart = commodityID.Count() > 0 ? msLines.Entities
        //                                  .Where(x => commodityID.Contains(x.GetAttributeValue<EntityReference>("tss_mastermarketsizeref").Id))
        //                                  .Sum(x => (int)x.GetAttributeValue<AliasedValue>("tss_mastermarketsizesublines.tss_qty").Value * ((Money)x.GetAttributeValue<AliasedValue>("tss_mastermarketsizesublines.tss_price").Value).Value) : 0m;
        //            }

        //            totalamountms = totalmscommodity + totalmsstandardpart;
        //        }
        //    }

        //    //Update
        //    Entity marketSizePSS = _DL_tss_marketsizeresultpss.Select(organizationService, oPSS);

        //    marketSizePSS["tss_totaluio"] = totaluio;
        //    marketSizePSS["tss_totalnonuio"] = totalnonuio;
        //    marketSizePSS["tss_totalgroupuiocommodity"] = totalgroupuiocommodity;
        //    marketSizePSS["tss_totalalluio"] = totalalluio;
        //    marketSizePSS["tss_totalmscommodity"] = new Money(totalmscommodity);
        //    marketSizePSS["tss_totalmsstandardpart"] = new Money(totalmsstandardpart);
        //    marketSizePSS["tss_totalamountms"] = new Money(totalamountms);

        //    organizationService.Update(marketSizePSS);
        //}


        #endregion
    }
}
