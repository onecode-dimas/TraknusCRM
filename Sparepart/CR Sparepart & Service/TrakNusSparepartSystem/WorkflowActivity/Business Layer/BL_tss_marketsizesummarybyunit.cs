using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrakNusSparepartSystem.DataLayer;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using TrakNusSparepartSystem.Helper;
using CrmEarlyBound;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Client;
using System.Data.SqlClient;
using System.Data;
using TrakNusSparepartSystem.WorkflowActivity.Business_Layer;

namespace TrakNusSparepartSystem.WorkflowActivity.BusinessLayer
{
    public class BL_tss_marketsizesummarybyunit
    {
        #region Constant
        private const int STATUS_COMPLETED_MS = 865920000;
        private const int ACTIVE = 865920005;
        private const int REVISION = 865920001;

        private const int UNITTYPE_UIO = 865920000;
        private const int UNITTYPE_NONUIO = 865920001;
        private const int UNITTYPE_COMMODITY = 865920002;

        private const int MTD1 = 865920000;
        private const int MTD2 = 865920001;
        private const int MTD3 = 865920002;
        private const int MTD4 = 865920003;
        private const int MTD5 = 865920004;
        #endregion

        #region Dependencies
        DL_tss_marketsizeresultpss _DL_tss_marketsizeresultpss = new DL_tss_marketsizeresultpss();
        DL_tss_mastermarketsize _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        DL_tss_mastermarketsizelines _DL_tss_mastermarketsizelines = new DL_tss_mastermarketsizelines();
        DL_tss_mastermarketsizesublines _DL_tss_mastermarketsizesublines = new DL_tss_mastermarketsizesublines();
        DL_tss_keyaccount _DL_tss_keyaccount = new DL_tss_keyaccount();
        DL_tss_sparepartsetup _DL_tss_sparepartsetup = new DL_tss_sparepartsetup();
        DL_partMaster _DL_partMaster = new DL_partMaster();
        DL_tss_groupuiocommodityaccount _DL_tss_groupuiocommodityaccount = new DL_tss_groupuiocommodityaccount();
        DL_trs_masterpart _DL_trs_masterpart = new DL_trs_masterpart();
        DL_tss_marketsizesummarybyunit _DL_tss_marketsizesummarybyunit = new DL_tss_marketsizesummarybyunit();
        RetrieveHelper _retrievehelper = new RetrieveHelper();
        #endregion

        public void GenerateMarketSizeSummaryByUnit_UsingSP_OnClick(IOrganizationService _organizationservice, ITracingService _tracingservice, IWorkflowContext _workflowcontext)
        {
            using (OrganizationServiceContext _context = new OrganizationServiceContext(_organizationservice))
            {
                List<SqlParameter> _sqlparameters = new List<SqlParameter>()
                {
                    new SqlParameter("@systemuserid", SqlDbType.NVarChar) { Value = _workflowcontext.UserId.ToString().Replace("{", "").Replace("}", "") },
                };

                DataTable _datatable = new GetStoredProcedure().Connect("sp_ms_GenerateMS_SummaryByUnit_ALL", _sqlparameters, false);
            }
        }

        public void generateMarketSizeSummarybyUnit(IOrganizationService organizationService, IWorkflowContext context)
        {
            try
            {
                using (CrmServiceContext _context = new CrmServiceContext(organizationService))
                {
                    //KA UIO
                    var _alldatakauiotemp = (from tbmarketsizeresultpss in _context.tss_marketsizeresultpssSet
                                             join tbmarketsizeresultpssdetail in _context.tss_marketsizeresultpssdetailSet on tbmarketsizeresultpss.tss_marketsizeresultpssId equals tbmarketsizeresultpssdetail.tss_marketsizeresultpssid.Id
                                             join tbkauio in _context.tss_kauioSet on tbmarketsizeresultpssdetail.tss_kauioid.Id equals tbkauio.tss_kauioId
                                             join tbkeyaccount in _context.tss_keyaccountSet on tbkauio.tss_KeyAccountId.Id equals tbkeyaccount.tss_keyaccountId
                                             join tbmastermarketsize in _context.tss_MasterMarketSizeSet on tbkeyaccount.tss_keyaccountId equals tbmastermarketsize.tss_keyaccountid.Id
                                             join tbmastermarketsizelines in _context.tss_mastermarketsizelinesSet on tbmastermarketsize.tss_MasterMarketSizeId equals tbmastermarketsizelines.tss_MasterMarketSizeRef.Id
                                             join tbmastermarketsizesublines in _context.tss_mastermarketsizesublinesSet on tbmastermarketsizelines.tss_mastermarketsizelinesId equals tbmastermarketsizesublines.tss_MasterMSLinesRef.Id
                                             where tbmastermarketsize.tss_PSS.Id == context.UserId
                                                && tbmastermarketsize.tss_Status.Value == STATUS_COMPLETED_MS
                                                && tbmarketsizeresultpss.tss_Status.Value == 865920005
                                             select new
                                             {
                                                 marketsizeresultpssid = tbmarketsizeresultpss.tss_marketsizeresultpssId,
                                                 mastermarketsizeid = tbmastermarketsize.tss_MasterMarketSizeId,
                                                 pss = tbmastermarketsize.tss_PSS.Id,
                                                 serialnumber = tbmastermarketsize.tss_SerialNumber.Id,
                                                 msperiodstart = tbmastermarketsize.tss_MSPeriodStart,
                                                 msperiodend = tbmastermarketsize.tss_MSPeriodEnd,
                                                 price = tbmastermarketsizesublines.tss_Price
                                             }).ToList();

                    var _alldatakauio = (from _alldatakauiotempdata in _alldatakauiotemp.AsEnumerable()
                                         group _alldatakauiotempdata by new
                                         {
                                             _alldatakauiotempdata.marketsizeresultpssid,
                                             _alldatakauiotempdata.mastermarketsizeid,
                                             _alldatakauiotempdata.pss,
                                             _alldatakauiotempdata.serialnumber,
                                             _alldatakauiotempdata.msperiodstart,
                                             _alldatakauiotempdata.msperiodend
                                         } into grouping
                                         select new
                                         {
                                             marketsizeresultpssid = grouping.Key.marketsizeresultpssid,
                                             mastermarketsizeid = grouping.Key.mastermarketsizeid,
                                             pss = grouping.Key.pss,
                                             serialnumber = grouping.Key.serialnumber,
                                             msperiodstart = grouping.Key.msperiodstart,
                                             msperiodend = grouping.Key.msperiodend,
                                             price = grouping.Sum(x => x.price.Value)
                                         }).ToList();

                    foreach (var _datakauio in _alldatakauio)
                    {
                        FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
                        _filterexpression.AddCondition("tss_marketsizeid", ConditionOperator.Equal, _datakauio.mastermarketsizeid);
                        _filterexpression.AddCondition("tss_serialnumber", ConditionOperator.Equal, _datakauio.serialnumber);
                        _filterexpression.AddCondition("tss_msperiodstart", ConditionOperator.Equal, _datakauio.msperiodstart);
                        _filterexpression.AddCondition("tss_msperiodend", ConditionOperator.Equal, _datakauio.msperiodend);

                        QueryExpression _queryexpression = new QueryExpression("tss_marketsizesummarybyunit");
                        _queryexpression.Criteria.AddFilter(_filterexpression);
                        _queryexpression.ColumnSet = new ColumnSet(true);

                        EntityCollection _marketsizesummarybyunitcollection = organizationService.RetrieveMultiple(_queryexpression);

                        if (_marketsizesummarybyunitcollection.Entities.Count() == 0)
                        {
                            // CREATE
                            tss_marketsizesummarybyunit _marketsizesummarybyunit = new tss_marketsizesummarybyunit();

                            _marketsizesummarybyunit.tss_MarketSizeId = new EntityReference("tss_marketsizeresultpss", (Guid)_datakauio.marketsizeresultpssid);
                            _marketsizesummarybyunit.tss_SerialNumber = new EntityReference("new_population", _datakauio.serialnumber);
                            _marketsizesummarybyunit.tss_PSS = new EntityReference("systemuser", _datakauio.pss);
                            _marketsizesummarybyunit.tss_MSPeriodStart = _datakauio.msperiodstart;
                            _marketsizesummarybyunit.tss_MSPeriodEnd = _datakauio.msperiodend;
                            _marketsizesummarybyunit.tss_ActivePeriodStart = _datakauio.msperiodstart;
                            _marketsizesummarybyunit.tss_ActivePeriodSEnd = _datakauio.msperiodend;
                            _marketsizesummarybyunit.tss_TotalAmountMSMainParts = new Money(_datakauio.price);
                            _marketsizesummarybyunit.tss_TotalAmountMSCommodity = new Money(0);
                            _marketsizesummarybyunit.tss_totalamountms = new Money(_datakauio.price);

                            organizationService.Create(_marketsizesummarybyunit);
                        }
                        else
                        {
                            // UPDATE
                            tss_marketsizesummarybyunit _marketsizesummarybyunit = new tss_marketsizesummarybyunit();

                            _marketsizesummarybyunit.Id = _marketsizesummarybyunitcollection.Entities[0].Id;
                            _marketsizesummarybyunit.tss_TotalAmountMSMainParts = new Money(_datakauio.price);
                            _marketsizesummarybyunit.tss_TotalAmountMSCommodity = new Money(0);
                            _marketsizesummarybyunit.tss_totalamountms = new Money(_datakauio.price);

                            organizationService.Update(_marketsizesummarybyunit);
                        }
                    }

                    // KA GROUP UIO COMMODITY
                    var _alldatakagroupuiotemp = (from tbmarketsizeresultpss in _context.tss_marketsizeresultpssSet
                                                  join tbmarketsizeresultpssdetail in _context.tss_marketsizeresultpssdetailSet on tbmarketsizeresultpss.tss_marketsizeresultpssId equals tbmarketsizeresultpssdetail.tss_marketsizeresultpssid.Id
                                                  join tbkagroupuiocommodity in _context.tss_kagroupuiocommoditySet on tbmarketsizeresultpssdetail.tss_groupuiocommodity.Id equals tbkagroupuiocommodity.tss_GroupUIOCommodity.Id
                                                  join tbkeyaccount in _context.tss_keyaccountSet on tbkagroupuiocommodity.tss_KeyAccountId.Id equals tbkeyaccount.tss_keyaccountId
                                                  join tbmastermarketsize in _context.tss_MasterMarketSizeSet on tbkeyaccount.tss_keyaccountId equals tbmastermarketsize.tss_keyaccountid.Id
                                                  join tbmastermarketsizesublines in _context.tss_mastermarketsizesublinesSet on tbmastermarketsize.tss_MasterMarketSizeId equals tbmastermarketsizesublines.tss_MasterMarketSizeId.Id
                                                  where tbmastermarketsize.tss_PSS.Id == context.UserId
                                                     && tbmastermarketsize.tss_Status.Value == STATUS_COMPLETED_MS
                                                     && tbmarketsizeresultpss.tss_Status.Value == 865920005
                                                  select new
                                                  {
                                                      marketsizeresultpssid = tbmarketsizeresultpss.tss_marketsizeresultpssId,
                                                      mastermarketsizeid = tbmastermarketsize.tss_MasterMarketSizeId,
                                                      pss = tbmastermarketsize.tss_PSS.Id,
                                                      groupuiocommodity = tbmastermarketsize.tss_GroupUIOCommodity.Id,
                                                      msperiodstart = tbmastermarketsize.tss_MSPeriodStart,
                                                      msperiodend = tbmastermarketsize.tss_MSPeriodEnd,
                                                      price = tbmastermarketsizesublines.tss_Price
                                                  }).ToList();

                    var _alldatakagroupuio = (from _alldatakagroupuiotempdata in _alldatakagroupuiotemp.AsEnumerable()
                                              group _alldatakagroupuiotempdata by new
                                              {
                                                  _alldatakagroupuiotempdata.marketsizeresultpssid,
                                                  _alldatakagroupuiotempdata.mastermarketsizeid,
                                                  _alldatakagroupuiotempdata.pss,
                                                  _alldatakagroupuiotempdata.groupuiocommodity,
                                                  _alldatakagroupuiotempdata.msperiodstart,
                                                  _alldatakagroupuiotempdata.msperiodend
                                              } into grouping
                                              select new
                                              {
                                                  marketsizeresultpssid = grouping.Key.marketsizeresultpssid,
                                                  mastermarketsizeid = grouping.Key.mastermarketsizeid,
                                                  pss = grouping.Key.pss,
                                                  groupuiocommodity = grouping.Key.groupuiocommodity,
                                                  msperiodstart = grouping.Key.msperiodstart,
                                                  msperiodend = grouping.Key.msperiodend,
                                                  price = grouping.Sum(x => x.price.Value)
                                              }).ToList();

                    foreach (var _datakagroupuio in _alldatakagroupuio)
                    {
                        FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
                        _filterexpression.AddCondition("tss_marketsizeid", ConditionOperator.Equal, _datakagroupuio.mastermarketsizeid);
                        _filterexpression.AddCondition("tss_groupuiocommodityid", ConditionOperator.Equal, _datakagroupuio.groupuiocommodity);
                        _filterexpression.AddCondition("tss_msperiodstart", ConditionOperator.Equal, _datakagroupuio.msperiodstart);
                        _filterexpression.AddCondition("tss_msperiodend", ConditionOperator.Equal, _datakagroupuio.msperiodend);

                        QueryExpression _queryexpression = new QueryExpression("tss_marketsizesummarybyunit");
                        _queryexpression.Criteria.AddFilter(_filterexpression);
                        _queryexpression.ColumnSet = new ColumnSet(true);

                        EntityCollection _marketsizesummarybyunitcollection = organizationService.RetrieveMultiple(_queryexpression);

                        if (_marketsizesummarybyunitcollection.Entities.Count() == 0)
                        {
                            // CREATE
                            tss_marketsizesummarybyunit _marketsizesummarybyunit = new tss_marketsizesummarybyunit();

                            _marketsizesummarybyunit.tss_MarketSizeId = new EntityReference("tss_marketsizeresultpss", (Guid)_datakagroupuio.marketsizeresultpssid);
                            _marketsizesummarybyunit.tss_GroupUIOCommodityId = new EntityReference("tss_groupuiocommodityaccount", _datakagroupuio.groupuiocommodity);
                            _marketsizesummarybyunit.tss_PSS = new EntityReference("systemuser", _datakagroupuio.pss);
                            _marketsizesummarybyunit.tss_MSPeriodStart = _datakagroupuio.msperiodstart;
                            _marketsizesummarybyunit.tss_MSPeriodEnd = _datakagroupuio.msperiodend;
                            _marketsizesummarybyunit.tss_ActivePeriodStart = _datakagroupuio.msperiodstart;
                            _marketsizesummarybyunit.tss_ActivePeriodSEnd = _datakagroupuio.msperiodend;
                            _marketsizesummarybyunit.tss_TotalAmountMSMainParts = new Money(0);
                            _marketsizesummarybyunit.tss_TotalAmountMSCommodity = new Money(_datakagroupuio.price);
                            _marketsizesummarybyunit.tss_totalamountms = new Money(_datakagroupuio.price);

                            organizationService.Create(_marketsizesummarybyunit);
                        }
                        else
                        {
                            // UPDATE
                            tss_marketsizesummarybyunit _marketsizesummarybyunit = new tss_marketsizesummarybyunit();

                            _marketsizesummarybyunit.Id = _marketsizesummarybyunitcollection.Entities[0].Id;
                            _marketsizesummarybyunit.tss_TotalAmountMSMainParts = new Money(_datakagroupuio.price);
                            _marketsizesummarybyunit.tss_TotalAmountMSCommodity = new Money(0);
                            _marketsizesummarybyunit.tss_totalamountms = new Money(_datakagroupuio.price);

                            organizationService.Update(_marketsizesummarybyunit);
                        }
                    }

                }
                
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        public void generateMarketSizeSummarybyUnit_Default(IOrganizationService organizationService, IWorkflowContext context)
        {
            DateTime startDtMS = DateTime.MinValue;
            DateTime endDtMS = DateTime.MinValue;

            #region Get all Market Size PSS in current periode 

            FilterExpression fpss = new FilterExpression(LogicalOperator.And);
            fpss.AddCondition("tss_activeperiodsend", ConditionOperator.GreaterEqual, DateTime.Now);
            fpss.AddCondition("tss_activeperiodstart", ConditionOperator.LessEqual, DateTime.Now);

            QueryExpression qpss = new QueryExpression("tss_marketsizeresultpss");
            qpss.Criteria.AddFilter(fpss);
            qpss.ColumnSet = new ColumnSet(true);
            EntityCollection ENC_tss_marketsizeresultpss = _DL_tss_marketsizeresultpss.Select(organizationService, qpss);

            #endregion

            #region get Part Setup
            //Get SparePart Config 

            FilterExpression fSetup = new FilterExpression(LogicalOperator.And);
            fSetup.AddCondition("tss_name", ConditionOperator.Equal, "TSS");

            QueryExpression qSetup = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
            qSetup.ColumnSet = new ColumnSet(true);
            qSetup.Criteria.AddFilter(fSetup);
            EntityCollection setups = _DL_tss_sparepartsetup.Select(organizationService, qSetup);
            #endregion

            #region Generate Market Size Summary by Unit

            foreach (Entity en_MarketSizeResultPSS in ENC_tss_marketsizeresultpss.Entities)
            {
                decimal totalmscommodity = 0m;
                decimal totalmsmainpart = 0m;
                decimal totalamountms = 0m;

                #region Get Customer
                FilterExpression fMs = new FilterExpression(LogicalOperator.And);
                fMs.AddCondition("tss_unittype", ConditionOperator.NotNull);
                fMs.AddCondition("tss_pss", ConditionOperator.Equal, en_MarketSizeResultPSS.GetAttributeValue<EntityReference>("tss_pss").Id);
                fMs.AddCondition("tss_status", ConditionOperator.Equal, STATUS_COMPLETED_MS);

                QueryExpression qMS = new QueryExpression(_DL_tss_mastermarketsize.EntityName);
                qMS.Criteria.AddFilter(fMs);
                qMS.ColumnSet = new ColumnSet(true);
                EntityCollection ms = _DL_tss_mastermarketsize.Select(organizationService, qMS);
                Guid[] custs = ms.Entities.GroupBy(x => x.GetAttributeValue<EntityReference>("tss_customer").Id).Select(x => x.First().GetAttributeValue<EntityReference>("tss_customer").Id).ToArray();
                #endregion

                #region Check if Revised
                //Check if revised on KA
                for (var i = 0; i < custs.Count(); i++)
                {

                    FilterExpression fKA = new FilterExpression(LogicalOperator.And);
                    fKA.AddCondition("tss_pss", ConditionOperator.Equal, en_MarketSizeResultPSS.GetAttributeValue<EntityReference>("tss_pss").Id);
                    fKA.AddCondition("tss_customer", ConditionOperator.Equal, custs[i]);

                    QueryExpression qKA = new QueryExpression(_DL_tss_keyaccount.EntityName);
                    qKA.Criteria.AddFilter(fKA);
                    qKA.ColumnSet.AddColumn("tss_version");
                    EntityCollection ka = _DL_tss_keyaccount.Select(organizationService, qKA);

                    if (ka.Entities.Count > 0 && setups.Entities.Count > 0)
                    {
                        if (ka.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_version").Value == REVISION).Count() == 0)
                        {
                            startDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems");
                            endDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_enddatems");
                        }
                        else
                        {
                            startDtMS = new DateTime(setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems").Year, setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems").Month, 1).AddMonths(setups.Entities[0].GetAttributeValue<int>("tss_evaluationms"));
                            endDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_enddatems");
                        }
                    }
                }
                #endregion

                //GET MASTER MARKET SIZE
                FilterExpression fMMS = new FilterExpression(LogicalOperator.And);
                fMMS.AddCondition("tss_pss", ConditionOperator.Equal, en_MarketSizeResultPSS.GetAttributeValue<EntityReference>("tss_pss").Id);
                fMMS.AddCondition("tss_customer", ConditionOperator.Equal, en_MarketSizeResultPSS.GetAttributeValue<EntityReference>("tss_customer").Id);
                fMMS.AddCondition("tss_status", ConditionOperator.Equal, STATUS_COMPLETED_MS);
                fMMS.AddCondition("tss_activeperiodstart", ConditionOperator.LessEqual, DateTime.Now);
                fMMS.AddCondition("tss_activeperiodsend", ConditionOperator.GreaterEqual, DateTime.Now);

                QueryExpression qMMS = new QueryExpression(_DL_tss_mastermarketsize.EntityName);
                qMMS.ColumnSet = new ColumnSet(true);
                qMMS.Criteria.AddFilter(fMMS);

                EntityCollection enc_MasterMarketSize = _DL_tss_mastermarketsize.Select(organizationService, qMMS);

                foreach (Entity en_MasterMarketSize in enc_MasterMarketSize.Entities)
                {
                    if (en_MasterMarketSize.GetAttributeValue<OptionSetValue>("tss_unittype").Value == UNITTYPE_UIO || en_MasterMarketSize.GetAttributeValue<OptionSetValue>("tss_unittype").Value == UNITTYPE_NONUIO)
                    {
                        Entity en_MarketSizeSummarybyUnit = new Entity("tss_marketsizesummarybyunit");
                        en_MarketSizeSummarybyUnit["tss_marketsizeid"] = new EntityReference("tss_marketsizeresultpss", en_MarketSizeResultPSS.Id);
                        en_MarketSizeSummarybyUnit["tss_pss"] = en_MarketSizeResultPSS.GetAttributeValue<EntityReference>("tss_pss");
                        en_MarketSizeSummarybyUnit["tss_msperiodend"] = en_MarketSizeResultPSS.GetAttributeValue<DateTime>("tss_msperiodend");
                        en_MarketSizeSummarybyUnit["tss_msperiodstart"] = en_MarketSizeResultPSS.GetAttributeValue<DateTime>("tss_msperiodstart");

                        if (startDtMS != DateTime.MinValue)
                            en_MarketSizeSummarybyUnit["tss_activeperiodstart"] = startDtMS;
                        if (endDtMS != DateTime.MinValue)
                            en_MarketSizeSummarybyUnit["tss_activeperiodsend"] = endDtMS;

                        #region get Master Market Size Line
                        FilterExpression fMMSZL = new FilterExpression(LogicalOperator.And);
                        fMMSZL.AddCondition("tss_mastermarketsizeref", ConditionOperator.Equal, en_MasterMarketSize.Id);

                        QueryExpression qMMZL = new QueryExpression(_DL_tss_mastermarketsizelines.EntityName);
                        qMMZL.ColumnSet = new ColumnSet(true);
                        qMMZL.Criteria.AddFilter(fMMSZL);
                        EntityCollection enc_MasterMarketSizeLine = _DL_tss_mastermarketsizelines.Select(organizationService, qMMZL);
                        #endregion

                        #region get Master Market Size Sub Line
                        foreach (Entity en_MasterMarketSizeLine in enc_MasterMarketSizeLine.Entities)
                        {
                            FilterExpression fMMSZSL = new FilterExpression(LogicalOperator.And);
                            fMMSZSL.AddCondition("tss_mastermslinesref", ConditionOperator.Equal, en_MasterMarketSizeLine.GetAttributeValue<Guid>("tss_mastermarketsizelinesid"));

                            QueryExpression qMMZSL = new QueryExpression(_DL_tss_mastermarketsizesublines.EntityName);
                            qMMZSL.ColumnSet = new ColumnSet(true);
                            qMMZSL.Criteria.AddFilter(fMMSZSL);
                            EntityCollection enc_MasterMarketSizeSubLine = _DL_tss_mastermarketsizesublines.Select(organizationService, qMMZSL);

                            #region calculate total Amount

                            foreach (Entity en_MasterMarketSizeSubLine in enc_MasterMarketSizeSubLine.Entities)
                            {
                                //Entity en_PartMaster = new Entity(_DL_partMaster.EntityName, en_MasterMarketSizeSubLine.GetAttributeValue<EntityReference>("tss_partnumber").Id);
                                Entity en_PartMaster = organizationService.Retrieve(_DL_partMaster.EntityName, en_MasterMarketSizeSubLine.GetAttributeValue<EntityReference>("tss_partnumber").Id, new ColumnSet(true));

                                if (en_PartMaster.Attributes.Contains("tss_parttype"))
                                {
                                    if (en_PartMaster.GetAttributeValue<OptionSetValue>("tss_parttype").Value == 865920000)
                                    {
                                        //if Part Type = Main Part 
                                        totalmsmainpart = totalmsmainpart + (en_MasterMarketSizeSubLine.GetAttributeValue<Money>("tss_price").Value * en_MasterMarketSizeSubLine.GetAttributeValue<int>("tss_qty"));
                                    }
                                    else if (en_PartMaster.GetAttributeValue<OptionSetValue>("tss_parttype").Value == 865920001)
                                    {
                                        //if Part Type = Commodity 
                                        totalmscommodity = totalmscommodity + (en_MasterMarketSizeSubLine.GetAttributeValue<Money>("tss_price").Value * en_MasterMarketSizeSubLine.GetAttributeValue<int>("tss_qty"));
                                    }
                                }
                            }

                            #endregion
                        }
                        #endregion

                        totalamountms = totalmscommodity + totalmsmainpart;
                        en_MarketSizeSummarybyUnit["tss_serialnumber"] = en_MasterMarketSize.GetAttributeValue<EntityReference>("tss_serialnumber");
                        en_MarketSizeSummarybyUnit["tss_totalamountmscommodity"] = new Money(totalmscommodity);
                        en_MarketSizeSummarybyUnit["tss_totalamountmsmainparts"] = new Money(totalmsmainpart);
                        en_MarketSizeSummarybyUnit["tss_totalamountms"] = new Money(totalamountms);

                        organizationService.Create(en_MarketSizeSummarybyUnit);
                    }
                    else if (en_MasterMarketSize.GetAttributeValue<OptionSetValue>("tss_unittype").Value == UNITTYPE_COMMODITY)
                    {
                        Entity en_MarketSizeSummarybyUnit = new Entity("tss_marketsizesummarybyunit");
                        en_MarketSizeSummarybyUnit["tss_marketsizeid"] = new EntityReference("tss_marketsizeresultpss", en_MarketSizeResultPSS.Id);
                        en_MarketSizeSummarybyUnit["tss_pss"] = en_MarketSizeResultPSS.GetAttributeValue<EntityReference>("tss_pss");
                        en_MarketSizeSummarybyUnit["tss_msperiodend"] = en_MarketSizeResultPSS.GetAttributeValue<DateTime>("tss_msperiodend");
                        en_MarketSizeSummarybyUnit["tss_msperiodstart"] = en_MarketSizeResultPSS.GetAttributeValue<DateTime>("tss_msperiodstart");

                        if (startDtMS != DateTime.MinValue)
                            en_MarketSizeSummarybyUnit["tss_activeperiodstart"] = startDtMS;
                        if (endDtMS != DateTime.MinValue)
                            en_MarketSizeSummarybyUnit["tss_activeperiodsend"] = endDtMS;

                        #region GET MASTER MARKET SIZE SUBLINE
                        FilterExpression fMMSZL = new FilterExpression(LogicalOperator.And);
                        fMMSZL.AddCondition("tss_mastermarketsizeid", ConditionOperator.Equal, en_MasterMarketSize.Id);

                        QueryExpression qMMZL = new QueryExpression(_DL_tss_mastermarketsizesublines.EntityName);
                        qMMZL.ColumnSet = new ColumnSet(true);
                        qMMZL.Criteria.AddFilter(fMMSZL);
                        EntityCollection enc_MasterMarketSizeSubLine = _DL_tss_mastermarketsizesublines.Select(organizationService, qMMZL);
                        #endregion

                        foreach (Entity en_MasterMarketSizeSubLine in enc_MasterMarketSizeSubLine.Entities)
                        {
                            if (en_MasterMarketSizeSubLine.Attributes.Contains("tss_partnumber"))
                            {
                                Entity en_PartMaster = organizationService.Retrieve(_DL_partMaster.EntityName, en_MasterMarketSizeSubLine.GetAttributeValue<EntityReference>("tss_partnumber").Id, new ColumnSet(true));

                                if (en_PartMaster.Attributes.Contains("tss_parttype"))
                                {
                                    if (en_PartMaster.GetAttributeValue<OptionSetValue>("tss_parttype").Value == 865920000)
                                    {
                                        //if Part Type = Main Part 
                                        totalmsmainpart = totalmsmainpart + (en_MasterMarketSizeSubLine.GetAttributeValue<Money>("tss_price").Value * en_MasterMarketSizeSubLine.GetAttributeValue<int>("tss_qty"));
                                    }
                                    else if (en_PartMaster.GetAttributeValue<OptionSetValue>("tss_parttype").Value == 865920001)
                                    {
                                        //if Part Type = Commodity 
                                        totalmscommodity = totalmscommodity + (en_MasterMarketSizeSubLine.GetAttributeValue<Money>("tss_price").Value * en_MasterMarketSizeSubLine.GetAttributeValue<int>("tss_qty"));
                                    }
                                }

                            }
                            else
                            {
                                throw new Exception("ID Master Market Size: " + en_MasterMarketSize.Id.ToString() + " ID Master Market Size Sublines: " + en_MasterMarketSizeSubLine.Id.ToString());
                            }
                            //Entity en_PartMaster = new Entity(_DL_partMaster.EntityName, en_MasterMarketSizeSubLine.GetAttributeValue<EntityReference>("tss_partnumber").Id);

                        }

                        totalamountms = totalmscommodity + totalmsmainpart;

                        FilterExpression oFilter = new FilterExpression(LogicalOperator.And);
                        oFilter.AddCondition("tss_groupuiocommodityname", ConditionOperator.Equal, en_MasterMarketSize.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id);

                        QueryExpression oQuery = new QueryExpression(_DL_tss_groupuiocommodityaccount.EntityName);
                        oQuery.Criteria.AddFilter(oFilter);
                        oQuery.ColumnSet = new ColumnSet(true);
                        EntityCollection oResult = _DL_tss_groupuiocommodityaccount.Select(organizationService, oQuery);

                        en_MarketSizeSummarybyUnit["tss_groupuiocommodityid"] = new EntityReference("tss_groupuiocommodityaccount", oResult.Entities[0].Id);
                        en_MarketSizeSummarybyUnit["tss_totalamountmscommodity"] = new Money(totalmscommodity);
                        en_MarketSizeSummarybyUnit["tss_totalamountmsmainparts"] = new Money(totalmsmainpart);
                        en_MarketSizeSummarybyUnit["tss_totalamountms"] = new Money(totalamountms);

                        organizationService.Create(en_MarketSizeSummarybyUnit);
                    }


                }

            }

            #endregion

        }





        //public void generateMarketSizeSummarybyUnit(IOrganizationService organizationService, IWorkflowContext context)
        //{
        //    try
        //    {
        //        decimal totalmsmainpart = 0m;
        //        decimal totalmscommodity = 0m;
        //        decimal totalamountms = 0m;

        //        DateTime startPeriodMS = new DateTime();
        //        DateTime endPeriodMS = new DateTime();
        //        DateTime startDtMS = new DateTime();
        //        DateTime endDtMS = new DateTime();

        //        FilterExpression fSetup = new FilterExpression(LogicalOperator.And);
        //        fSetup.AddCondition("tss_name", ConditionOperator.Equal, "TSS");

        //        QueryExpression qSetup = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
        //        qSetup.ColumnSet = new ColumnSet(true);
        //        qSetup.Criteria.AddFilter(fSetup);
        //        EntityCollection setups = _DL_tss_sparepartsetup.Select(organizationService, qSetup);

        //        FilterExpression fMSPSS = new FilterExpression(LogicalOperator.And);
        //        fMSPSS.AddCondition("tss_activeperiodsend", ConditionOperator.GreaterEqual, DateTime.Now);
        //        fMSPSS.AddCondition("tss_activeperiodstart", ConditionOperator.LessEqual, DateTime.Now);
        //        fMSPSS.AddCondition("tss_status", ConditionOperator.Equal, ACTIVE);

        //        QueryExpression qMSPSS = new QueryExpression(_DL_tss_marketsizeresultpss.EntityName);
        //        qMSPSS.Criteria.AddFilter(fMSPSS);
        //        qMSPSS.ColumnSet = new ColumnSet(true);
        //        EntityCollection msPSS = _DL_tss_marketsizeresultpss.Select(organizationService, qMSPSS);

        //        if (msPSS.Entities.Count > 0)
        //        {
        //            foreach (var pss in msPSS.Entities)
        //            {
        //                #region GET CUSTOMER
        //                FilterExpression fMs = new FilterExpression(LogicalOperator.And);
        //                fMs.AddCondition("tss_unittype", ConditionOperator.NotNull);
        //                fMs.AddCondition("tss_pss", ConditionOperator.Equal, pss.GetAttributeValue<EntityReference>("tss_pss").Id);
        //                fMs.AddCondition("tss_status", ConditionOperator.Equal, STATUS_COMPLETED_MS);

        //                QueryExpression qMS = new QueryExpression(_DL_tss_mastermarketsize.EntityName);
        //                qMS.Criteria.AddFilter(fMs);
        //                qMS.ColumnSet = new ColumnSet(true);
        //                EntityCollection ms = _DL_tss_mastermarketsize.Select(organizationService, qMS);
        //                Guid[] custs = ms.Entities.GroupBy(x => x.GetAttributeValue<EntityReference>("tss_customer").Id).Select(x => x.First().GetAttributeValue<EntityReference>("tss_customer").Id).ToArray();
        //                #endregion

        //                #region CHECK IF REVISED ON KA
        //                for (var i = 0; i < custs.Count(); i++)
        //                {
        //                    FilterExpression fKA = new FilterExpression(LogicalOperator.And);
        //                    fKA.AddCondition("tss_pss", ConditionOperator.Equal, pss.GetAttributeValue<EntityReference>("tss_pss").Id);
        //                    fKA.AddCondition("tss_customer", ConditionOperator.Equal, custs[i]);

        //                    QueryExpression qKA = new QueryExpression(_DL_tss_keyaccount.EntityName);
        //                    qKA.Criteria.AddFilter(fKA);
        //                    qKA.ColumnSet.AddColumn("tss_version");
        //                    EntityCollection ka = _DL_tss_keyaccount.Select(organizationService, qKA);

        //                    if (ka.Entities.Count > 0 && setups.Entities.Count > 0)
        //                    {
        //                        if (ka.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_version").Value == REVISION).Count() == 0)
        //                        {
        //                            startDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems");
        //                            endDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_enddatems");
        //                        }
        //                        else
        //                        {
        //                            startDtMS = new DateTime(setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems").Year, setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems").Month, 1).AddMonths(setups.Entities[0].GetAttributeValue<int>("tss_evaluationms"));
        //                            endDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_enddatems");
        //                        }
        //                    }
        //                }
        //                #endregion

        //                //MAINPARTS
        //                FilterExpression a1 = new FilterExpression(LogicalOperator.Or);
        //                a1.AddCondition("tss_methodcalculationused", ConditionOperator.Equal, MTD1);
        //                a1.AddCondition("tss_methodcalculationused", ConditionOperator.Equal, MTD2);
        //                a1.AddCondition("tss_methodcalculationused", ConditionOperator.Equal, MTD3);

        //                FilterExpression a2 = new FilterExpression(LogicalOperator.And);
        //                a2.AddFilter(a1);
        //                a2.AddCondition("tss_duedate", ConditionOperator.NotNull);

        //                FilterExpression a3 = new FilterExpression(LogicalOperator.Or);
        //                a3.AddFilter(a2);
        //                a3.AddCondition("tss_methodcalculationused", ConditionOperator.Equal, MTD4);
        //                a3.AddCondition("tss_methodcalculationused", ConditionOperator.Equal, MTD5);

        //                QueryExpression nQExpresion = new QueryExpression(_DL_tss_mastermarketsize.EntityName);

        //                LinkEntity linkToLine = new LinkEntity
        //                {
        //                    LinkFromEntityName = _DL_tss_mastermarketsize.EntityName,
        //                    LinkToEntityName = _DL_tss_mastermarketsizelines.EntityName,
        //                    LinkFromAttributeName = "tss_mastermarketsizeid",
        //                    LinkToAttributeName = "tss_mastermarketsizeref",
        //                    Columns = new ColumnSet(true),
        //                    EntityAlias = "mastermarketsizeline",
        //                    JoinOperator = JoinOperator.Inner

        //                };

        //                LinkEntity linkToSubline = new LinkEntity
        //                {
        //                    LinkFromEntityName = _DL_tss_mastermarketsizelines.EntityName,
        //                    LinkToEntityName = _DL_tss_mastermarketsizesublines.EntityName,
        //                    LinkFromAttributeName = "tss_mastermarketsizelinesid",
        //                    LinkToAttributeName = "tss_mastermslinesref",
        //                    Columns = new ColumnSet(true),
        //                    EntityAlias = "mastermarketsizesubline",
        //                    JoinOperator = JoinOperator.Inner

        //                };

        //                linkToLine.LinkEntities.Add(linkToSubline);
        //                nQExpresion.LinkEntities.Add(linkToLine);
        //                //nQExpresion.LinkEntities[0].LinkCriteria.AddCondition("tss_duedate", ConditionOperator.NotNull);
        //                nQExpresion.LinkEntities[0].LinkCriteria.AddFilter(a3);
        //                nQExpresion.Criteria.AddCondition("tss_serialnumber", ConditionOperator.NotNull);
        //                nQExpresion.Criteria.AddCondition("tss_status", ConditionOperator.Equal, STATUS_COMPLETED_MS);
        //                nQExpresion.Criteria.AddCondition("tss_pss", ConditionOperator.Equal, pss.GetAttributeValue<EntityReference>("tss_pss").Id);
        //                nQExpresion.ColumnSet = new ColumnSet(true);
        //                EntityCollection msSubLines = _DL_tss_mastermarketsize.Select(organizationService, nQExpresion);

        //                if (msSubLines.Entities.Count() > 0)
        //                {
        //                    var groupBySN = (from r in msSubLines.Entities.AsEnumerable()
        //                                     group r by new
        //                                     {
        //                                         groupBySerialNumber = r.GetAttributeValue<EntityReference>("tss_serialnumber").Id
        //                                     } into g
        //                                     select new
        //                                     {
        //                                         serialNumber = g.Key.groupBySerialNumber,
        //                                         sumAmount = g.Sum(x =>
        //                                                   ((Money)x.GetAttributeValue<AliasedValue>("mastermarketsizesubline.tss_price").Value).Value
        //                                                   * Convert.ToDecimal(x.GetAttributeValue<AliasedValue>("mastermarketsizesubline.tss_qty").Value))
        //                                     }).ToList();

        //                    foreach (var o in groupBySN)
        //                    {
        //                        FilterExpression fExist = new FilterExpression(LogicalOperator.And);
        //                        fExist.AddCondition("tss_pss", ConditionOperator.Equal, pss.GetAttributeValue<EntityReference>("tss_pss").Id);
        //                        fExist.AddCondition("tss_marketsizeid", ConditionOperator.Equal, pss.Id);
        //                        fExist.AddCondition("tss_serialnumber", ConditionOperator.Equal, o.serialNumber);

        //                        QueryExpression qExist = new QueryExpression(_DL_tss_marketsizesummarybyunit.EntityName);
        //                        qExist.Criteria.AddFilter(fExist);
        //                        qExist.ColumnSet = new ColumnSet(true);

        //                        EntityCollection totalSummaryByUnitQ = organizationService.RetrieveMultiple(qExist);

        //                        if (totalSummaryByUnitQ.Entities.Count() == 0)
        //                        {
        //                            Entity en_MarketSizeSummarybyUnit = new Entity("tss_marketsizesummarybyunit");

        //                            en_MarketSizeSummarybyUnit["tss_marketsizeid"] = new EntityReference("tss_marketsizeresultpss", pss.Id);
        //                            en_MarketSizeSummarybyUnit["tss_serialnumber"] = new EntityReference("new_population", o.serialNumber);
        //                            en_MarketSizeSummarybyUnit["tss_pss"] = pss.GetAttributeValue<EntityReference>("tss_pss");
        //                            en_MarketSizeSummarybyUnit["tss_msperiodend"] = pss.GetAttributeValue<DateTime>("tss_msperiodend");
        //                            en_MarketSizeSummarybyUnit["tss_msperiodstart"] = pss.GetAttributeValue<DateTime>("tss_msperiodstart");

        //                            if (startDtMS != DateTime.MinValue)
        //                                en_MarketSizeSummarybyUnit["tss_activeperiodstart"] = startDtMS;
        //                            if (endDtMS != DateTime.MinValue)
        //                                en_MarketSizeSummarybyUnit["tss_activeperiodsend"] = endDtMS;

        //                            en_MarketSizeSummarybyUnit["tss_totalamountmsmainparts"] = new Money(o.sumAmount);
        //                            en_MarketSizeSummarybyUnit["tss_totalamountmscommodity"] = new Money(0);
        //                            en_MarketSizeSummarybyUnit["tss_totalamountms"] = new Money(o.sumAmount);

        //                            organizationService.Create(en_MarketSizeSummarybyUnit);
        //                        }
        //                        else
        //                        {
        //                            Entity entityupdate = new Entity("tss_marketsizesummarybyunit");
        //                            entityupdate.Id = totalSummaryByUnitQ.Entities[0].Id;

        //                            entityupdate["tss_msperiodend"] = pss.GetAttributeValue<DateTime>("tss_msperiodend");
        //                            entityupdate["tss_msperiodstart"] = pss.GetAttributeValue<DateTime>("tss_msperiodstart");

        //                            if (startDtMS != DateTime.MinValue)
        //                                entityupdate["tss_activeperiodstart"] = startDtMS;
        //                            if (endDtMS != DateTime.MinValue)
        //                                entityupdate["tss_activeperiodsend"] = endDtMS;

        //                            entityupdate["tss_totalamountmsmainparts"] = new Money(o.sumAmount);
        //                            entityupdate["tss_totalamountmscommodity"] = new Money(0);
        //                            entityupdate["tss_totalamountms"] = new Money(o.sumAmount);

        //                            organizationService.Update(entityupdate);
        //                        }
        //                    }
        //                }

        //                //COMMODITY
        //                nQExpresion = new QueryExpression(_DL_tss_mastermarketsize.EntityName);
        //                linkToSubline = new LinkEntity
        //                {
        //                    LinkFromEntityName = _DL_tss_mastermarketsize.EntityName,
        //                    LinkToEntityName = _DL_tss_mastermarketsizesublines.EntityName,
        //                    LinkFromAttributeName = "tss_mastermarketsizeid",
        //                    LinkToAttributeName = "tss_mastermarketsizeid",
        //                    Columns = new ColumnSet(true),
        //                    EntityAlias = "mastermarketsizesubline",
        //                    JoinOperator = JoinOperator.Inner

        //                };
        //                nQExpresion.LinkEntities.Add(linkToSubline);
        //                nQExpresion.Criteria.AddCondition("tss_groupuiocommodity", ConditionOperator.NotNull);
        //                nQExpresion.Criteria.AddCondition("tss_status", ConditionOperator.Equal, STATUS_COMPLETED_MS);
        //                nQExpresion.Criteria.AddCondition("tss_pss", ConditionOperator.Equal, pss.GetAttributeValue<EntityReference>("tss_pss").Id);
        //                nQExpresion.ColumnSet = new ColumnSet(true);
        //                msSubLines = new EntityCollection();
        //                msSubLines = _DL_tss_mastermarketsize.Select(organizationService, nQExpresion);

        //                if (msSubLines.Entities.Count() > 0)
        //                {
        //                    var groupByComm = (from r in msSubLines.Entities.AsEnumerable()
        //                                       group r by new
        //                                       {
        //                                           groupByCommodity = r.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id
        //                                       } into g
        //                                       select new
        //                                       {
        //                                           commodities = g.Key.groupByCommodity,
        //                                           sumAmount = g.Sum(x =>
        //                                                   ((Money)x.GetAttributeValue<AliasedValue>("mastermarketsizesubline.tss_price").Value).Value
        //                                                   * Convert.ToDecimal(x.GetAttributeValue<AliasedValue>("mastermarketsizesubline.tss_qty").Value))
        //                                       }).ToList();

        //                    foreach (var o in groupByComm)
        //                    {
        //                        FilterExpression oFilter = new FilterExpression(LogicalOperator.And);
        //                        oFilter.AddCondition("tss_groupuiocommodityname", ConditionOperator.Equal, o.commodities);

        //                        QueryExpression oQuery = new QueryExpression(_DL_tss_groupuiocommodityaccount.EntityName);
        //                        oQuery.Criteria.AddFilter(oFilter);
        //                        oQuery.ColumnSet = new ColumnSet(true);
        //                        EntityCollection oResult = _DL_tss_groupuiocommodityaccount.Select(organizationService, oQuery);

        //                        FilterExpression fExist = new FilterExpression(LogicalOperator.And);
        //                        fExist.AddCondition("tss_pss", ConditionOperator.Equal, pss.GetAttributeValue<EntityReference>("tss_pss").Id);
        //                        fExist.AddCondition("tss_marketsizeid", ConditionOperator.Equal, pss.Id);
        //                        fExist.AddCondition("tss_groupuiocommodityid", ConditionOperator.Equal, oResult.Entities[0].Id);

        //                        QueryExpression qExist = new QueryExpression(_DL_tss_marketsizesummarybyunit.EntityName);
        //                        qExist.Criteria.AddFilter(fExist);
        //                        qExist.ColumnSet = new ColumnSet(true);

        //                        EntityCollection totalSummaryByUnitQ = organizationService.RetrieveMultiple(qExist);

        //                        if (totalSummaryByUnitQ.Entities.Count() == 0)
        //                        {
        //                            Entity en_MarketSizeSummarybyUnit = new Entity("tss_marketsizesummarybyunit");

        //                            en_MarketSizeSummarybyUnit["tss_marketsizeid"] = new EntityReference("tss_marketsizeresultpss", pss.Id);
        //                            en_MarketSizeSummarybyUnit["tss_pss"] = pss.GetAttributeValue<EntityReference>("tss_pss");
        //                            en_MarketSizeSummarybyUnit["tss_msperiodend"] = pss.GetAttributeValue<DateTime>("tss_msperiodend");
        //                            en_MarketSizeSummarybyUnit["tss_msperiodstart"] = pss.GetAttributeValue<DateTime>("tss_msperiodstart");

        //                            if (startDtMS != DateTime.MinValue)
        //                                en_MarketSizeSummarybyUnit["tss_activeperiodstart"] = startDtMS;
        //                            if (endDtMS != DateTime.MinValue)
        //                                en_MarketSizeSummarybyUnit["tss_activeperiodsend"] = endDtMS;

        //                            en_MarketSizeSummarybyUnit["tss_groupuiocommodityid"] = new EntityReference("tss_groupuiocommodityaccount", oResult.Entities[0].Id);
        //                            en_MarketSizeSummarybyUnit["tss_totalamountmsmainparts"] = new Money(0);
        //                            en_MarketSizeSummarybyUnit["tss_totalamountmscommodity"] = new Money(o.sumAmount);
        //                            en_MarketSizeSummarybyUnit["tss_totalamountms"] = new Money(o.sumAmount);

        //                            organizationService.Create(en_MarketSizeSummarybyUnit);
        //                        }
        //                        else
        //                        {
        //                            Entity entityupdate = new Entity("tss_marketsizesummarybyunit");
        //                            entityupdate.Id = totalSummaryByUnitQ.Entities[0].Id;

        //                            entityupdate["tss_msperiodend"] = pss.GetAttributeValue<DateTime>("tss_msperiodend");
        //                            entityupdate["tss_msperiodstart"] = pss.GetAttributeValue<DateTime>("tss_msperiodstart");

        //                            if (startDtMS != DateTime.MinValue)
        //                                entityupdate["tss_activeperiodstart"] = startDtMS;
        //                            if (endDtMS != DateTime.MinValue)
        //                                entityupdate["tss_activeperiodsend"] = endDtMS;

        //                            entityupdate["tss_totalamountmsmainparts"] = new Money(0);
        //                            entityupdate["tss_totalamountmscommodity"] = new Money(o.sumAmount);
        //                            entityupdate["tss_totalamountms"] = new Money(o.sumAmount);

        //                            organizationService.Update(entityupdate);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {

        //        throw new Exception (e.Message);
        //    }
        //}

        //public void generateMarketSizeSummarybyUnit(IOrganizationService organizationService, IWorkflowContext context)
        //{
        //    try
        //    {
        //        decimal totalmsmainpart = 0m;
        //        decimal totalmscommodity = 0m;
        //        decimal totalamountms = 0m;

        //        DateTime startPeriodMS = new DateTime();
        //        DateTime endPeriodMS = new DateTime();
        //        DateTime startDtMS = new DateTime();
        //        DateTime endDtMS = new DateTime();

        //        FilterExpression fSetup = new FilterExpression(LogicalOperator.And);
        //        fSetup.AddCondition("tss_name", ConditionOperator.Equal, "TSS");

        //        QueryExpression qSetup = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
        //        qSetup.ColumnSet = new ColumnSet(true);
        //        qSetup.Criteria.AddFilter(fSetup);
        //        EntityCollection setups = _DL_tss_sparepartsetup.Select(organizationService, qSetup);

        //        FilterExpression fMSPSS = new FilterExpression(LogicalOperator.And);
        //        fMSPSS.AddCondition("tss_activeperiodsend", ConditionOperator.GreaterEqual, DateTime.Now);
        //        fMSPSS.AddCondition("tss_activeperiodstart", ConditionOperator.LessEqual, DateTime.Now);
        //        fMSPSS.AddCondition("tss_status", ConditionOperator.Equal, ACTIVE);

        //        QueryExpression qMSPSS = new QueryExpression(_DL_tss_marketsizeresultpss.EntityName);
        //        qMSPSS.Criteria.AddFilter(fMSPSS);
        //        qMSPSS.ColumnSet = new ColumnSet(true);
        //        EntityCollection msPSS = _DL_tss_marketsizeresultpss.Select(organizationService, qMSPSS);

        //        if (msPSS.Entities.Count > 0)
        //        {
        //            foreach (var pss in msPSS.Entities)
        //            {
        //                #region GET CUSTOMER
        //                FilterExpression fMs = new FilterExpression(LogicalOperator.And);
        //                fMs.AddCondition("tss_unittype", ConditionOperator.NotNull);
        //                fMs.AddCondition("tss_pss", ConditionOperator.Equal, pss.GetAttributeValue<EntityReference>("tss_pss").Id);
        //                fMs.AddCondition("tss_status", ConditionOperator.Equal, STATUS_COMPLETED_MS);

        //                QueryExpression qMS = new QueryExpression(_DL_tss_mastermarketsize.EntityName);
        //                qMS.Criteria.AddFilter(fMs);
        //                qMS.ColumnSet = new ColumnSet(true);
        //                List<Entity> ms = _retrievehelper.RetrieveMultiple(organizationService, qMS); // _DL_tss_mastermarketsize.Select(organizationService, qMS);
        //                Guid[] custs = ms.GroupBy(x => x.GetAttributeValue<EntityReference>("tss_customer").Id).Select(x => x.First().GetAttributeValue<EntityReference>("tss_customer").Id).ToArray();
        //                #endregion

        //                #region CHECK IF REVISED ON KA
        //                for (var i = 0; i < custs.Count(); i++)
        //                {
        //                    FilterExpression fKA = new FilterExpression(LogicalOperator.And);
        //                    fKA.AddCondition("tss_pss", ConditionOperator.Equal, pss.GetAttributeValue<EntityReference>("tss_pss").Id);
        //                    fKA.AddCondition("tss_customer", ConditionOperator.Equal, custs[i]);

        //                    QueryExpression qKA = new QueryExpression(_DL_tss_keyaccount.EntityName);
        //                    qKA.Criteria.AddFilter(fKA);
        //                    qKA.ColumnSet.AddColumn("tss_version");
        //                    EntityCollection ka = _DL_tss_keyaccount.Select(organizationService, qKA);

        //                    if (ka.Entities.Count > 0 && setups.Entities.Count > 0)
        //                    {
        //                        if (ka.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_version").Value == REVISION).Count() == 0)
        //                        {
        //                            startDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems");
        //                            endDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_enddatems");
        //                        }
        //                        else
        //                        {
        //                            startDtMS = new DateTime(setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems").Year, setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems").Month, 1).AddMonths(setups.Entities[0].GetAttributeValue<int>("tss_evaluationms"));
        //                            endDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_enddatems");
        //                        }
        //                    }
        //                }
        //                #endregion

        //                //MAINPARTS
        //                FilterExpression a1 = new FilterExpression(LogicalOperator.Or);
        //                a1.AddCondition("tss_methodcalculationused", ConditionOperator.Equal, MTD1);
        //                a1.AddCondition("tss_methodcalculationused", ConditionOperator.Equal, MTD2);
        //                a1.AddCondition("tss_methodcalculationused", ConditionOperator.Equal, MTD3);

        //                FilterExpression a2 = new FilterExpression(LogicalOperator.And);
        //                a2.AddFilter(a1);
        //                a2.AddCondition("tss_duedate", ConditionOperator.NotNull);

        //                FilterExpression a3 = new FilterExpression(LogicalOperator.Or);
        //                a3.AddFilter(a2);
        //                a3.AddCondition("tss_methodcalculationused", ConditionOperator.Equal, MTD4);
        //                a3.AddCondition("tss_methodcalculationused", ConditionOperator.Equal, MTD5);

        //                QueryExpression nQExpresion = new QueryExpression(_DL_tss_mastermarketsize.EntityName);

        //                LinkEntity linkToLine = new LinkEntity
        //                {
        //                    LinkFromEntityName = _DL_tss_mastermarketsize.EntityName,
        //                    LinkToEntityName = _DL_tss_mastermarketsizelines.EntityName,
        //                    LinkFromAttributeName = "tss_mastermarketsizeid",
        //                    LinkToAttributeName = "tss_mastermarketsizeref",
        //                    Columns = new ColumnSet(true),
        //                    EntityAlias = "mastermarketsizeline",
        //                    JoinOperator = JoinOperator.Inner

        //                };

        //                LinkEntity linkToSubline = new LinkEntity
        //                {
        //                    LinkFromEntityName = _DL_tss_mastermarketsizelines.EntityName,
        //                    LinkToEntityName = _DL_tss_mastermarketsizesublines.EntityName,
        //                    LinkFromAttributeName = "tss_mastermarketsizelinesid",
        //                    LinkToAttributeName = "tss_mastermslinesref",
        //                    Columns = new ColumnSet(true),
        //                    EntityAlias = "mastermarketsizesubline",
        //                    JoinOperator = JoinOperator.Inner

        //                };

        //                linkToLine.LinkEntities.Add(linkToSubline);
        //                nQExpresion.LinkEntities.Add(linkToLine);
        //                //nQExpresion.LinkEntities[0].LinkCriteria.AddCondition("tss_duedate", ConditionOperator.NotNull);
        //                nQExpresion.LinkEntities[0].LinkCriteria.AddFilter(a3);
        //                nQExpresion.Criteria.AddCondition("tss_serialnumber", ConditionOperator.NotNull);
        //                nQExpresion.Criteria.AddCondition("tss_status", ConditionOperator.Equal, STATUS_COMPLETED_MS);
        //                nQExpresion.Criteria.AddCondition("tss_pss", ConditionOperator.Equal, pss.GetAttributeValue<EntityReference>("tss_pss").Id);
        //                nQExpresion.ColumnSet = new ColumnSet(true);
        //                List<Entity> msSubLines = _retrievehelper.RetrieveMultiple(organizationService, nQExpresion); // _DL_tss_mastermarketsize.Select(organizationService, nQExpresion);

        //                if (msSubLines.Count() > 0)
        //                {
        //                    var groupBySN = (from r in msSubLines.AsEnumerable()
        //                                     group r by new
        //                                     {
        //                                         groupBySerialNumber = r.GetAttributeValue<EntityReference>("tss_serialnumber").Id
        //                                     } into g
        //                                     select new
        //                                     {
        //                                         serialNumber = g.Key.groupBySerialNumber,
        //                                         sumAmount = g.Sum(x =>
        //                                                   ((Money)x.GetAttributeValue<AliasedValue>("mastermarketsizesubline.tss_price").Value).Value
        //                                                   * Convert.ToDecimal(x.GetAttributeValue<AliasedValue>("mastermarketsizesubline.tss_qty").Value))
        //                                     }).ToList();

        //                    foreach (var o in groupBySN)
        //                    {
        //                        FilterExpression fExist = new FilterExpression(LogicalOperator.And);
        //                        fExist.AddCondition("tss_pss", ConditionOperator.Equal, pss.GetAttributeValue<EntityReference>("tss_pss").Id);
        //                        fExist.AddCondition("tss_marketsizeid", ConditionOperator.Equal, pss.Id);
        //                        fExist.AddCondition("tss_serialnumber", ConditionOperator.Equal, o.serialNumber);

        //                        QueryExpression qExist = new QueryExpression(_DL_tss_marketsizesummarybyunit.EntityName);
        //                        qExist.Criteria.AddFilter(fExist);
        //                        qExist.ColumnSet = new ColumnSet(true);

        //                        EntityCollection totalSummaryByUnitQ = organizationService.RetrieveMultiple(qExist);

        //                        if (totalSummaryByUnitQ.Entities.Count() == 0)
        //                        {
        //                            Entity en_MarketSizeSummarybyUnit = new Entity("tss_marketsizesummarybyunit");

        //                            en_MarketSizeSummarybyUnit["tss_marketsizeid"] = new EntityReference("tss_marketsizeresultpss", pss.Id);
        //                            en_MarketSizeSummarybyUnit["tss_serialnumber"] = new EntityReference("new_population", o.serialNumber);
        //                            en_MarketSizeSummarybyUnit["tss_pss"] = pss.GetAttributeValue<EntityReference>("tss_pss");
        //                            en_MarketSizeSummarybyUnit["tss_msperiodend"] = pss.GetAttributeValue<DateTime>("tss_msperiodend");
        //                            en_MarketSizeSummarybyUnit["tss_msperiodstart"] = pss.GetAttributeValue<DateTime>("tss_msperiodstart");

        //                            if (startDtMS != DateTime.MinValue)
        //                                en_MarketSizeSummarybyUnit["tss_activeperiodstart"] = startDtMS;
        //                            if (endDtMS != DateTime.MinValue)
        //                                en_MarketSizeSummarybyUnit["tss_activeperiodsend"] = endDtMS;

        //                            en_MarketSizeSummarybyUnit["tss_totalamountmsmainparts"] = new Money(o.sumAmount);
        //                            en_MarketSizeSummarybyUnit["tss_totalamountmscommodity"] = new Money(0);
        //                            en_MarketSizeSummarybyUnit["tss_totalamountms"] = new Money(o.sumAmount);

        //                            organizationService.Create(en_MarketSizeSummarybyUnit);
        //                        }
        //                        else
        //                        {
        //                            Entity entityupdate = new Entity("tss_marketsizesummarybyunit");
        //                            entityupdate.Id = totalSummaryByUnitQ.Entities[0].Id;

        //                            entityupdate["tss_msperiodend"] = pss.GetAttributeValue<DateTime>("tss_msperiodend");
        //                            entityupdate["tss_msperiodstart"] = pss.GetAttributeValue<DateTime>("tss_msperiodstart");

        //                            if (startDtMS != DateTime.MinValue)
        //                                entityupdate["tss_activeperiodstart"] = startDtMS;
        //                            if (endDtMS != DateTime.MinValue)
        //                                entityupdate["tss_activeperiodsend"] = endDtMS;

        //                            entityupdate["tss_totalamountmsmainparts"] = new Money(o.sumAmount);
        //                            entityupdate["tss_totalamountmscommodity"] = new Money(0);
        //                            entityupdate["tss_totalamountms"] = new Money(o.sumAmount);

        //                            organizationService.Update(entityupdate);
        //                        }
        //                    }
        //                }

        //                //COMMODITY
        //                nQExpresion = new QueryExpression(_DL_tss_mastermarketsize.EntityName);
        //                linkToSubline = new LinkEntity
        //                {
        //                    LinkFromEntityName = _DL_tss_mastermarketsize.EntityName,
        //                    LinkToEntityName = _DL_tss_mastermarketsizesublines.EntityName,
        //                    LinkFromAttributeName = "tss_mastermarketsizeid",
        //                    LinkToAttributeName = "tss_mastermarketsizeid",
        //                    Columns = new ColumnSet(true),
        //                    EntityAlias = "mastermarketsizesubline",
        //                    JoinOperator = JoinOperator.Inner

        //                };
        //                nQExpresion.LinkEntities.Add(linkToSubline);
        //                nQExpresion.Criteria.AddCondition("tss_groupuiocommodityheader", ConditionOperator.NotNull);
        //                nQExpresion.Criteria.AddCondition("tss_status", ConditionOperator.Equal, STATUS_COMPLETED_MS);
        //                nQExpresion.Criteria.AddCondition("tss_pss", ConditionOperator.Equal, pss.GetAttributeValue<EntityReference>("tss_pss").Id);
        //                nQExpresion.ColumnSet = new ColumnSet(true);
        //                msSubLines = new List<Entity>();
        //                msSubLines = _retrievehelper.RetrieveMultiple(organizationService, nQExpresion); // _DL_tss_mastermarketsize.Select(organizationService, nQExpresion);

        //                if (msSubLines.Count() > 0)
        //                {
        //                    var groupByCommHeader = (from r in msSubLines.AsEnumerable()
        //                                           group r by new
        //                                           {
        //                                               groupByCommodityHeader = r.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id
        //                                           } into g
        //                                           select new
        //                                           {
        //                                               commHeader = g.Key.groupByCommodityHeader,
        //                                               sumAmount = g.Sum(x =>
        //                                                           ((Money)x.GetAttributeValue<AliasedValue>("mastermarketsizesubline.tss_price").Value).Value
        //                                                           * Convert.ToDecimal(x.GetAttributeValue<AliasedValue>("mastermarketsizesubline.tss_qty").Value))
        //                                           }).ToList();

        //                    foreach (var o in groupByCommHeader)
        //                    {
        //                        FilterExpression oFilter = new FilterExpression(LogicalOperator.And);
        //                        oFilter.AddCondition("tss_groupuiocommodityheader", ConditionOperator.Equal, o.commHeader);

        //                        QueryExpression oQuery = new QueryExpression(_DL_tss_groupuiocommodityaccount.EntityName);
        //                        oQuery.Criteria.AddFilter(oFilter);
        //                        oQuery.ColumnSet = new ColumnSet(true);
        //                        EntityCollection oResult = _DL_tss_groupuiocommodityaccount.Select(organizationService, oQuery);

        //                        FilterExpression fExist = new FilterExpression(LogicalOperator.And);
        //                        fExist.AddCondition("tss_pss", ConditionOperator.Equal, pss.GetAttributeValue<EntityReference>("tss_pss").Id);
        //                        fExist.AddCondition("tss_marketsizeid", ConditionOperator.Equal, pss.Id);
        //                        fExist.AddCondition("tss_groupuiocommodityid", ConditionOperator.Equal, oResult.Entities[0].Id);

        //                        QueryExpression qExist = new QueryExpression(_DL_tss_marketsizesummarybyunit.EntityName);
        //                        qExist.Criteria.AddFilter(fExist);
        //                        qExist.ColumnSet = new ColumnSet(true);

        //                        EntityCollection totalSummaryByUnitQ = organizationService.RetrieveMultiple(qExist);

        //                        if (totalSummaryByUnitQ.Entities.Count() == 0)
        //                        {
        //                            Entity en_MarketSizeSummarybyUnit = new Entity("tss_marketsizesummarybyunit");

        //                            en_MarketSizeSummarybyUnit["tss_marketsizeid"] = new EntityReference("tss_marketsizeresultpss", pss.Id);
        //                            en_MarketSizeSummarybyUnit["tss_pss"] = pss.GetAttributeValue<EntityReference>("tss_pss");
        //                            en_MarketSizeSummarybyUnit["tss_msperiodend"] = pss.GetAttributeValue<DateTime>("tss_msperiodend");
        //                            en_MarketSizeSummarybyUnit["tss_msperiodstart"] = pss.GetAttributeValue<DateTime>("tss_msperiodstart");

        //                            if (startDtMS != DateTime.MinValue)
        //                                en_MarketSizeSummarybyUnit["tss_activeperiodstart"] = startDtMS;
        //                            if (endDtMS != DateTime.MinValue)
        //                                en_MarketSizeSummarybyUnit["tss_activeperiodsend"] = endDtMS;

        //                            en_MarketSizeSummarybyUnit["tss_groupuiocommodityid"] = new EntityReference("tss_groupuiocommodityaccount", oResult.Entities[0].Id);
        //                            en_MarketSizeSummarybyUnit["tss_totalamountmsmainparts"] = new Money(0);
        //                            en_MarketSizeSummarybyUnit["tss_totalamountmscommodity"] = new Money(o.sumAmount);
        //                            en_MarketSizeSummarybyUnit["tss_totalamountms"] = new Money(o.sumAmount);

        //                            organizationService.Create(en_MarketSizeSummarybyUnit);
        //                        }
        //                        else
        //                        {
        //                            Entity entityupdate = new Entity("tss_marketsizesummarybyunit");
        //                            entityupdate.Id = totalSummaryByUnitQ.Entities[0].Id;

        //                            entityupdate["tss_msperiodend"] = pss.GetAttributeValue<DateTime>("tss_msperiodend");
        //                            entityupdate["tss_msperiodstart"] = pss.GetAttributeValue<DateTime>("tss_msperiodstart");

        //                            if (startDtMS != DateTime.MinValue)
        //                                entityupdate["tss_activeperiodstart"] = startDtMS;
        //                            if (endDtMS != DateTime.MinValue)
        //                                entityupdate["tss_activeperiodsend"] = endDtMS;

        //                            entityupdate["tss_totalamountmsmainparts"] = new Money(0);
        //                            entityupdate["tss_totalamountmscommodity"] = new Money(o.sumAmount);
        //                            entityupdate["tss_totalamountms"] = new Money(o.sumAmount);

        //                            organizationService.Update(entityupdate);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {

        //        throw new Exception(e.Message);
        //    }
        //}
    }
}
