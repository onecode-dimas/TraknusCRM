using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrakNusSparepartSystem.DataLayer;
using TrakNusSparepartSystem.Helper;
using TrakNusSparepartSystem.WorkflowActivity.Business_Layer;
using System.Configuration;
using System.Data.Services;
using TrakNusSparepartSystem.DataLayer.Interface;
using CrmEarlyBound;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Crm.Sdk.Messages;

namespace TrakNusSparepartSystem.WorkflowActivity.BusinessLayer
{
    public class BL_tss_mastermarketsize
    {
        #region Constant
        private const int STATUS_OPEN = 865920000;
        private const int STATUS_REASON_LINES_OPEN = 865920000;
        private const int STATUS_LINES_DRAFT = 865920000;
        private const int STATUS_CALCULATE = 865920001;

        private const int STATUS_REASON_CALCULATE = 865920003;
        private const int KA_SATATUS_REASON_DRAFT= 865920005;

        private const int STATUS_ERROR = 865920002;
        private const int UNITTYPE_UIO = 865920000;
        private const int UNITTYPE_NONUIO = 865920001;
        private const int UNITTYPE_COMMODITY = 865920002;
        private const int STATUS_COMPLETED_MS = 865920000;
        private const int STATUS_ERROR_MS = 865920001;
        private const int STATUS_CLOSED_MS = 865920002;
        private const int STATUSREASON_COMPLETED_MS = 865920000;
        private const int STATUSREASON_ERROR_MS = 865920001;
        private const int MTD1 = 865920000;
        private const int MTD2 = 865920001;
        private const int MTD3 = 865920002;
        private const int MTD4 = 865920003;
        private const int MTD5 = 865920004;
        private const int BATTERY_TYPE = 865920001;
        private const int MS_TYPE = 865920001;
        private const int PART_COMMODITYTYPE_BATTERY = 865920000;
        private const int PART_COMMODITYTYPE_OIL = 865920001;
        private const int PART_COMMODITYTYPE_TYRE = 865920002;
        private const int PART_COMMODITYTYPE_BY_PART = 865920000;
        private const int PART_COMMODITYTYPE_BY_SPEC = 865920001;
        #endregion
        #region Depedencies
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        private DL_account _DL_account = new DL_account();
        private DL_tss_keyaccount _DL_tss_keyaccount = new DL_tss_keyaccount();
        private DL_tss_kauio _DL_tss_kauio = new DL_tss_kauio();
        private DL_population _DL_population = new DL_population();
        private DL_product _DL_product = new DL_product();
        private DL_tss_kagroupuiocommodity _DL_tss_kagroupuiocommodity = new DL_tss_kagroupuiocommodity();
        private DL_tss_groupuiocommodity _DL_tss_groupuiocommodity = new DL_tss_groupuiocommodity();
        private DL_tss_groupuiocommodityaccount _DL_tss_groupuiocommodityaccount = new DL_tss_groupuiocommodityaccount();
        private DL_tss_sparepartsetup _DL_tss_sparepartsetup = new DL_tss_sparepartsetup();
        private DL_tss_mastermarketsize _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        private DL_tss_mastermarketsizelines _DL_tss_mastermarketsizelines = new DL_tss_mastermarketsizelines();
        private DL_tss_mastermarketsizesublines _DL_tss_mastermarketsizesublines = new DL_tss_mastermarketsizesublines();
        private DL_tss_marketsizepartconsump _DL_tss_marketsizepartconsump = new DL_tss_marketsizepartconsump();
        private DL_trs_masterpart _DL_trs_masterpart = new DL_trs_masterpart();
        private DL_tss_partmasterlinesmodel _DL_tss_partmasterlinesmodel = new DL_tss_partmasterlinesmodel();
        private DL_tss_partmasterlinesbattery _DL_tss_partmasterlinesbattery = new DL_tss_partmasterlinesbattery();
        private DL_tss_partmasterlinesbatterytypeconsump _DL_tss_partmasterlinesbatterytypeconsump = new DL_tss_partmasterlinesbatterytypeconsump();
        private DL_tss_sparepartpricemaster _DL_tss_sparepartpricemaster = new DL_tss_sparepartpricemaster();
        private DL_tss_pricelistpart _DL_tss_pricelistpart = new DL_tss_pricelistpart();
        private DL_tss_matrixtyreconsump _DL_tss_matrixtyreconsump = new DL_tss_matrixtyreconsump();
        private RetrieveHelper _retrievehelper = new RetrieveHelper();

        private decimal? nulldecimal = null;
        private int? nullint = null;
        private DateTime? nulldatetime = null;

        protected IDataLayer _datalayer;
        #endregion

        public EntityCollection RetrieveMultipleKeyAccount(IOrganizationService organizationService, QueryExpression query)
        {
            EntityCollection result = new EntityCollection();
            int fetchCount = 5000;
            int pageNumber = 1;

            query.PageInfo = new PagingInfo();
            query.PageInfo.Count = fetchCount;
            query.PageInfo.PageNumber = pageNumber;
            query.PageInfo.PagingCookie = null;

            while (true)
            {
                EntityCollection collections = organizationService.RetrieveMultiple(query);

                if (collections.Entities.Count > 0)
                {
                    result.Entities.AddRange(collections.Entities);
                }

                if (collections.MoreRecords)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = collections.PagingCookie;
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        public void GenerateMasterMarketSize_OnClick(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {
            try
            {
                GenerateMasterMS(organizationService, tracingService, context, null);
            }
            catch (Exception e)
            {
                throw new InvalidWorkflowException(e.Message.ToString());
            }
        }

        public void GenerateMasterMarketSizeFromKeyAccount_OnClick(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context, string[] recordIds)
        {
            try
            {
                FilterExpression _filterexpression = new FilterExpression(LogicalOperator.Or);
                _filterexpression.AddCondition("tss_status", ConditionOperator.Equal, STATUS_OPEN);
                _filterexpression.AddCondition("tss_status", ConditionOperator.Equal, STATUS_ERROR);

                FilterExpression _FilterExpression = new FilterExpression(LogicalOperator.And);
                _FilterExpression.AddFilter(_filterexpression);
                _FilterExpression.AddCondition("tss_keyaccountid", ConditionOperator.In, recordIds);
                _FilterExpression.AddCondition("tss_pss", ConditionOperator.Equal, context.UserId);

                QueryExpression _queryexpression = new QueryExpression("tss_keyaccount");
                _queryexpression.ColumnSet = new ColumnSet(true);
                _queryexpression.Criteria.AddFilter(_FilterExpression);

                EntityCollection _keyaccountcollection = organizationService.RetrieveMultiple(_queryexpression);

                GenerateMasterMSv2(organizationService, tracingService, context, _keyaccountcollection);
                GenerateMasterMSSublinesv2(organizationService, tracingService, context, _keyaccountcollection);

                //GenerateMasterMS(organizationService, tracingService, context, _keyaccountcollection);
                //GenerateMasterMSSublines(organizationService, tracingService, context, _keyaccountcollection);

                BL_tss_marketsizesummarybypartnumber _BL_tss_marketsizesummarybypartnumber = new BL_tss_marketsizesummarybypartnumber();
                _BL_tss_marketsizesummarybypartnumber.GenerateMarketSizeSummaryByPartNumber(organizationService, context, _keyaccountcollection);

                BL_tss_marketsizesummarybygroupuiocommodity _BL_tss_marketsizesummarybygroupuiocommodity = new BL_tss_marketsizesummarybygroupuiocommodity();
                _BL_tss_marketsizesummarybygroupuiocommodity.GenerateMarketSizeSummaryByGroupUIOCommodity(organizationService, context, _keyaccountcollection);
            }
            catch (Exception e)
            {
                throw new InvalidWorkflowException(e.Message.ToString());
            }
        }

        public void GenerateMasterMSv2(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context, EntityCollection _keyaccountcollection)
        {
            using (CrmServiceContext _context = new CrmServiceContext(organizationService))
            {
                bool _kasuccess = true;
                int _calculatemethod = 0;
                decimal _methodvalue = 0;
                decimal _currenthm;
                decimal _lasthm;
                DateTime _currenthmdate;
                DateTime _lasthmdate;
                DateTime _deliverydate;
                DateTime _warrantyenddate;
                DateTime _calculatemarketsizedate = DateTime.Now.ToLocalTime().Date;

                var _sparepartsetup = (from tbmatrixmarketsizeperiod in _context.tss_matrixmarketsizeperiodSet
                                       where tbmatrixmarketsizeperiod.tss_IsActive == true
                                       select tbmatrixmarketsizeperiod).ToList().FirstOrDefault();

                if (_sparepartsetup != null)
                {
                    #region GENERATE KA UIO
                    var _alldatakauioheader = (from tbkauio in _context.tss_kauioSet
                                               join tbkeyaccount in _context.tss_keyaccountSet on tbkauio.tss_KeyAccountId.Id equals tbkeyaccount.tss_keyaccountId
                                               join tbpopulation in _context.new_populationSet on tbkauio.tss_SerialNumber.Id equals tbpopulation.new_populationId
                                               join tbproduct in _context.ProductSet on tbpopulation.trs_ProductMaster.Id equals tbproduct.ProductId
                                               //join tbunitgroupmarketsize in _context.tss_unitgroupmarketsizeSet on new { product = tbpopulation.trs_ProductMaster.Id, unitgroup = tbproduct.DefaultUoMScheduleId.Id } equals new { product = tbunitgroupmarketsize.tss_Model.Id, unitgroup = tbunitgroupmarketsize.tss_UnitGroup.Id }
                                               join tbunitgroupmarketsize in _context.tss_unitgroupmarketsizeSet on tbpopulation.trs_ProductMaster.Id equals tbunitgroupmarketsize.tss_Model.Id
                                               where (tbkeyaccount.tss_Status.Equals(STATUS_OPEN) || tbkeyaccount.tss_Status.Equals(STATUS_ERROR))
                                                   && tbkeyaccount.tss_PSS.Id == context.UserId
                                                   && tbkeyaccount.tss_Customer != null
                                                   && tbkeyaccount.tss_ActiveEndDate >= DateTime.Now
                                                   && tbkeyaccount.tss_ActiveStartDate <= DateTime.Now
                                                   && tbkeyaccount.tss_CalculatetoMS == true
                                                   && tbkeyaccount.StatusCode.Value == 1
                                                   && tbkauio.tss_CalculatetoMS == true
                                                   && tbkauio.tss_CalculateStatus == false
                                               //&& _keyaccountcollection
                                               //&& 
                                               //&& _keyaccountids.Contains(new Guid(tbkeyaccount.tss_keyaccountId.GetValueOrDefault().ToString()))
                                               select new
                                               {
                                                   tbkeyaccount_LogicalName = tbkeyaccount.LogicalName,
                                                   tbkeyaccount_tss_keyaccountId = tbkeyaccount.tss_keyaccountId,
                                                   tbkeyaccount_tss_MSPeriodStart = tbkeyaccount.tss_MSPeriodStart,
                                                   tbkeyaccount_tss_MSPeriodEnd = tbkeyaccount.tss_MSPeriodEnd,
                                                   tbkeyaccount_tss_ActiveStartDate = tbkeyaccount.tss_ActiveStartDate,
                                                   tbkeyaccount_tss_ActiveEndDate = tbkeyaccount.tss_ActiveEndDate,
                                                   tbkeyaccount_tss_Revision = tbkeyaccount.tss_Revision,
                                                   tbkauio_LogicalName = tbkauio.LogicalName,
                                                   tbkauio_tss_kauioId = tbkauio.tss_kauioId,
                                                   tbkauio_tss_PSS = tbkauio.tss_PSS,
                                                   tbkauio_tss_Customer = tbkauio.tss_Customer,
                                                   tbpopulation_LogicalName = tbpopulation.LogicalName,
                                                   tbpopulation_new_SerialNumber = tbpopulation.new_SerialNumber,
                                                   tbpopulation_tss_MSCurrentHourMeter = tbpopulation.tss_MSCurrentHourMeter,
                                                   tbpopulation_tss_MSLastHourMeter = tbpopulation.tss_MSLastHourMeter,
                                                   tbpopulation_tss_MSCurrentHourMeterDate = tbpopulation.tss_MSCurrentHourMeterDate,
                                                   tbpopulation_tss_MSLastHourMeterDate = tbpopulation.tss_MSLastHourMeterDate,
                                                   tbpopulation_new_DeliveryDate = tbpopulation.new_DeliveryDate,
                                                   tbpopulation_trs_WarrantyStartdate = tbpopulation.trs_WarrantyStartdate,
                                                   tbpopulation_trs_WarrantyEndDate = tbpopulation.trs_WarrantyEndDate,
                                                   tbpopulation_tss_EstWorkingHour = tbpopulation.tss_EstWorkingHour,
                                                   tbpopulation_tss_PopulationStatus = tbpopulation.tss_PopulationStatus,
                                                   tbpopulation_new_populationId = tbpopulation.new_populationId,
                                                   tbpopulation_trs_ProductMaster = tbpopulation.trs_ProductMaster,
                                                   tbproduct_LogicalName = tbproduct.LogicalName,
                                                   tbproduct_ProductId = tbproduct.ProductId,
                                                   tbproduct_tss_UseTyre = tbproduct.tss_UseTyre,
                                                   tbunitgroupmarketsize_LogicalName = tbunitgroupmarketsize.LogicalName,
                                                   tbunitgroupmarketsize_tss_MaintenanceInterval = tbunitgroupmarketsize.tss_MaintenanceInterval
                                               }).ToList();

                    foreach (var _datakauio in _alldatakauioheader)
                    {
                        #region GENERATE MARKET SIZE KA UIO

                        tss_MasterMarketSize _mastermarketsize = new tss_MasterMarketSize();

                        _currenthm = 0;
                        _lasthm = 0;
                        
                        //CHECK METHOD 5
                        if ((_datakauio.tbpopulation_new_DeliveryDate.HasValue)
                            && _datakauio.tbkeyaccount_tss_MSPeriodStart.HasValue
                            && _datakauio.tbkeyaccount_tss_MSPeriodEnd.HasValue)
                        {
                            _calculatemethod = MTD5;
                            _deliverydate = _datakauio.tbpopulation_new_DeliveryDate.GetValueOrDefault().ToLocalTime();

                            if (!_datakauio.tbpopulation_trs_WarrantyEndDate.HasValue)
                                _warrantyenddate = _deliverydate.AddYears(1);
                            else
                                _warrantyenddate = _datakauio.tbpopulation_trs_WarrantyEndDate.GetValueOrDefault().ToLocalTime();

                            DateTime _msperiodstart = _datakauio.tbkeyaccount_tss_MSPeriodStart.GetValueOrDefault().ToLocalTime();
                            DateTime _msperiodend = _datakauio.tbkeyaccount_tss_MSPeriodEnd.GetValueOrDefault().ToLocalTime();

                            CalculateDate _calculate = new CalculateDate();
                            decimal _diffwarrantydate = _calculate.DiffYear(_deliverydate, _warrantyenddate);
                            decimal _diffperioddate = _calculate.DiffYear(_msperiodstart, _msperiodend);
                            decimal _periodpmmethod5 = (decimal)(_diffwarrantydate + _diffperioddate);

                            if (_periodpmmethod5 > 0)
                            {
                                _mastermarketsize.tss_periodpmmethod5 = _periodpmmethod5;
                                _methodvalue = _periodpmmethod5;
                            }
                        }

                        //CHECK METHOD 4
                        if (_datakauio.tbpopulation_new_DeliveryDate.HasValue
                            && _datakauio.tbkeyaccount_tss_MSPeriodStart.HasValue
                            && _datakauio.tbkeyaccount_tss_MSPeriodEnd.HasValue)
                        {
                            _calculatemethod = MTD4;

                            _deliverydate = _datakauio.tbpopulation_new_DeliveryDate.GetValueOrDefault().ToLocalTime();
                            DateTime _msperiodstart = _datakauio.tbkeyaccount_tss_MSPeriodStart.GetValueOrDefault().ToLocalTime();
                            DateTime _msperiodend = _datakauio.tbkeyaccount_tss_MSPeriodEnd.GetValueOrDefault().ToLocalTime();

                            CalculateDate _calculate = new CalculateDate();
                            decimal _diffdeliverydate = _calculate.DiffYear(_deliverydate, _calculatemarketsizedate);
                            decimal _diffperioddate = _calculate.DiffYear(_msperiodstart, _msperiodend);
                            decimal _periodpmmethod4 = (decimal)(_diffdeliverydate + _diffperioddate);

                            if (_periodpmmethod4 > 0)
                            {
                                _mastermarketsize.tss_PeriodPMMethod4 = _periodpmmethod4;
                                _methodvalue = _periodpmmethod4;
                            }
                        }

                        //CHECK METHOD 3
                        if (_datakauio.tbpopulation_tss_EstWorkingHour.HasValue)
                        {
                            _calculatemethod = MTD3;

                            decimal _estworkinghour = _datakauio.tbpopulation_tss_EstWorkingHour.GetValueOrDefault();

                            if (_estworkinghour > 0)
                            {
                                decimal _estworkinghourvalue = _estworkinghour > (decimal)24 ? (decimal)24 : _estworkinghour;

                                _mastermarketsize.tss_AvgHMMethod3 = _estworkinghourvalue;
                                _methodvalue = _estworkinghourvalue;
                            }
                        }

                        //CHECK METHOD 2
                        if (_datakauio.tbpopulation_tss_MSCurrentHourMeter.HasValue
                            && _datakauio.tbpopulation_tss_MSCurrentHourMeterDate.HasValue
                            && (_datakauio.tbpopulation_new_DeliveryDate.HasValue || (_datakauio.tbpopulation_trs_WarrantyStartdate.HasValue && _datakauio.tbpopulation_trs_WarrantyEndDate.HasValue)))
                        {
                            _calculatemethod = MTD2;

                            _currenthm = _datakauio.tbpopulation_tss_MSCurrentHourMeter.GetValueOrDefault();
                            _currenthmdate = _datakauio.tbpopulation_tss_MSCurrentHourMeterDate.GetValueOrDefault().ToLocalTime();

                            if (!_datakauio.tbpopulation_trs_WarrantyEndDate.HasValue && _datakauio.tbpopulation_new_DeliveryDate.HasValue)
                            {
                                _deliverydate = _datakauio.tbpopulation_new_DeliveryDate.GetValueOrDefault().ToLocalTime();
                                _warrantyenddate = _deliverydate.AddYears(1);
                            }
                            else
                                _warrantyenddate = _datakauio.tbpopulation_trs_WarrantyEndDate.GetValueOrDefault().ToLocalTime();

                            if ((decimal)(_currenthmdate - _warrantyenddate).TotalDays > 0)
                            {
                                decimal _avghmmethod2 = _currenthm / ((decimal)(_currenthmdate - _warrantyenddate).TotalDays);

                                if (_avghmmethod2 > 0)
                                {
                                    decimal _avghmmethod2value = _avghmmethod2 > (decimal)24 ? (decimal)24 : _avghmmethod2;

                                    _mastermarketsize.tss_AvgHMMethod2 = _avghmmethod2value;
                                    _methodvalue = _avghmmethod2value;
                                }
                            }
                        }

                        //CHECK METHOD 1
                        if (_datakauio.tbpopulation_tss_MSCurrentHourMeter.HasValue
                            && _datakauio.tbpopulation_tss_MSLastHourMeter.HasValue
                            && _datakauio.tbpopulation_tss_MSCurrentHourMeterDate.HasValue
                            && _datakauio.tbpopulation_tss_MSLastHourMeterDate.HasValue)
                        {
                            _calculatemethod = MTD1;

                            _currenthm = _datakauio.tbpopulation_tss_MSCurrentHourMeter.GetValueOrDefault();
                            _lasthm = _datakauio.tbpopulation_tss_MSLastHourMeter.GetValueOrDefault();
                            _currenthmdate = _datakauio.tbpopulation_tss_MSCurrentHourMeterDate.GetValueOrDefault().ToLocalTime();
                            _lasthmdate = _datakauio.tbpopulation_tss_MSLastHourMeterDate.GetValueOrDefault().ToLocalTime();

                            if ((decimal)(_currenthmdate - _lasthmdate).TotalDays > 0)
                            {
                                decimal _avghmmethod1 = (_currenthm - _lasthm) / ((decimal)(_currenthmdate - _lasthmdate).TotalDays);

                                if (_avghmmethod1 > 0)
                                {
                                    decimal _avghmmethod1value = _avghmmethod1 > (decimal)24 ? (decimal)24 : _avghmmethod1;

                                    _mastermarketsize.tss_AvgHMMethod1 = _avghmmethod1 > (decimal)24 ? (decimal)24 : _avghmmethod1;
                                    _methodvalue = _avghmmethod1value;
                                }
                            }
                        }

                        if (_calculatemethod == MTD1 || _calculatemethod == MTD2 || _calculatemethod == MTD3 || _calculatemethod == MTD4 || _calculatemethod == MTD5)
                        {
                            //GENERATE MARKET SIZE UIO / NON UIO

                            _mastermarketsize.tss_UnitType = new OptionSetValue(_datakauio.tbpopulation_tss_PopulationStatus.GetValueOrDefault() ? UNITTYPE_UIO : UNITTYPE_NONUIO);
                            _mastermarketsize.tss_MSPeriodStart = _datakauio.tbkeyaccount_tss_MSPeriodStart.GetValueOrDefault().ToLocalTime();
                            _mastermarketsize.tss_MSPeriodEnd = _datakauio.tbkeyaccount_tss_MSPeriodEnd.GetValueOrDefault().ToLocalTime();
                            _mastermarketsize.tss_ActivePeriodStart = _datakauio.tbkeyaccount_tss_ActiveStartDate.GetValueOrDefault().ToLocalTime();
                            _mastermarketsize.tss_ActivePeriodSEnd = _datakauio.tbkeyaccount_tss_ActiveEndDate.GetValueOrDefault();
                            _mastermarketsize.tss_msperiod = _datakauio.tbkeyaccount_tss_Revision.GetValueOrDefault();

                            _mastermarketsize.tss_PSS = new EntityReference(_datakauio.tbkauio_LogicalName, _datakauio.tbkauio_tss_PSS.Id);
                            _mastermarketsize.tss_Customer = new EntityReference(_datakauio.tbkauio_LogicalName, _datakauio.tbkauio_tss_Customer.Id);
                            _mastermarketsize.tss_SerialNumber = new EntityReference(_datakauio.tbpopulation_LogicalName, _datakauio.tbpopulation_new_populationId.GetValueOrDefault());
                            _mastermarketsize.tss_Status = new OptionSetValue(STATUS_COMPLETED_MS);
                            _mastermarketsize.tss_StatusReason = new OptionSetValue(STATUSREASON_COMPLETED_MS);
                            _mastermarketsize.tss_UseTyre = _datakauio.tbproduct_tss_UseTyre.HasValue ? _datakauio.tbproduct_tss_UseTyre.GetValueOrDefault() : false;
                            _mastermarketsize.tss_keyaccountid = new EntityReference(_datakauio.tbkeyaccount_LogicalName, _datakauio.tbkeyaccount_tss_keyaccountId.GetValueOrDefault());
                            _mastermarketsize.tss_ismsresultpssgenerated = false;
                            _mastermarketsize.tss_issublinesgenerated = false;

                            Guid _mastermarketsizeid = organizationService.Create(_mastermarketsize);


                            #region --------------------------------------------------------------------------------------------------------------------------------------------------------------
                            #region --------------------------------------------------------------------------------------------------------------------------------------------------------------
                            #region GENERATE MARKET SIZE LINES KA UIO

                            string _errordescription = "";
                            int _rowcount = 0;
                            int _pmperiod = 0;
                            int _interval = _datakauio.tbunitgroupmarketsize_tss_MaintenanceInterval.GetValueOrDefault();
                            int _duedatems = _sparepartsetup.tss_DueDateMS.GetValueOrDefault();
                            int _mscurrenthourmeter = (int)Math.Round(_datakauio.tbpopulation_tss_MSCurrentHourMeter.GetValueOrDefault(), 0);
                            int _hmconsumppm;
                            int _forecastpmdate;
                            DateTime _mscurrenthourmeterdate = _datakauio.tbpopulation_tss_MSCurrentHourMeterDate.HasValue ? _datakauio.tbpopulation_tss_MSCurrentHourMeterDate.GetValueOrDefault().ToLocalTime() : DateTime.MinValue;
                            DateTime _nextpmdate;

                            if (_calculatemethod == MTD1 || _calculatemethod == MTD2 || _calculatemethod == MTD3)
                            {
                                if (_mscurrenthourmeterdate != DateTime.MinValue && _interval > 0 && _methodvalue > 0)
                                {
                                    _nextpmdate = _mscurrenthourmeterdate.AddDays((int)Math.Round((((Math.Round((decimal)_mscurrenthourmeter / _interval, 0) * _interval) + _interval) - _mscurrenthourmeter) / _methodvalue, 0));

                                    while (_mscurrenthourmeterdate <= _sparepartsetup.tss_EndDateMarketSize.GetValueOrDefault().ToLocalTime())
                                    {
                                        tss_mastermarketsizelines _mastermarketsizelines = new tss_mastermarketsizelines();

                                        if (_mscurrenthourmeterdate >= _sparepartsetup.tss_StartDateMarketSize.GetValueOrDefault().ToLocalTime())
                                            _pmperiod += 1;

                                        _mastermarketsizelines.tss_MasterMarketSizeRef = new EntityReference(_mastermarketsize.LogicalName, _mastermarketsizeid);
                                        _mastermarketsizelines.tss_HMPM = (int)(Math.Round((decimal)_mscurrenthourmeter / _interval, 0) * _interval) + _interval;
                                        _mastermarketsizelines.tss_HMConsumpPM = _mastermarketsizelines.tss_HMPM.GetValueOrDefault() - _mscurrenthourmeter;
                                        _mastermarketsizelines.tss_ForecastPMDate = (int)Math.Round((decimal)_mastermarketsizelines.tss_HMConsumpPM.GetValueOrDefault() / _methodvalue, 0);
                                        _mastermarketsizelines.tss_EstimatedPMDate = _mscurrenthourmeterdate;
                                        _mastermarketsizelines.tss_PMPeriod = _pmperiod;
                                        _mastermarketsizelines.tss_MethodCalculationUsed = new OptionSetValue(_calculatemethod);
                                        _mastermarketsizelines.tss_Status = new OptionSetValue(STATUS_LINES_DRAFT);
                                        _mastermarketsizelines.tss_StatusReason = new OptionSetValue(STATUS_REASON_LINES_OPEN);

                                        _mastermarketsizelines.tss_PM = _DL_tss_mastermarketsizelines.GetPMType(organizationService, _datakauio.tbproduct_ProductId.GetValueOrDefault(), _mastermarketsizelines.tss_HMPM.GetValueOrDefault()).ToString();

                                        //DUE DATE = NEXT PM DATE - DUE DATE MARKET SIZE
                                        if (_pmperiod > 0 && _nextpmdate <= _sparepartsetup.tss_EndDateMarketSize.GetValueOrDefault().ToLocalTime())
                                            _mastermarketsizelines.tss_DueDate = _nextpmdate.AddDays(-_duedatems);

                                        _mscurrenthourmeter = _mastermarketsizelines.tss_HMPM.GetValueOrDefault();
                                        _hmconsumppm = _mastermarketsizelines.tss_HMConsumpPM.GetValueOrDefault();
                                        _forecastpmdate = _mastermarketsizelines.tss_ForecastPMDate.GetValueOrDefault();
                                        _mscurrenthourmeterdate = _nextpmdate;
                                        _nextpmdate = _mscurrenthourmeterdate.AddDays(_mastermarketsizelines.tss_ForecastPMDate.GetValueOrDefault());

                                        Guid _mastermarketsizelinesid = organizationService.Create(_mastermarketsizelines);

                                        _rowcount += 1;
                                    }

                                    if (_rowcount == 0)
                                        _errordescription = "Process Generate Lines FAILED. Please Check Data Population !";
                                }
                                else
                                    _errordescription = "Process Generate Lines FAILED. Please Check 'Population MSCurrentHourMeterDate' / 'Unit Group Maintenance Interval' !";
                            }
                            else if (_calculatemethod == MTD4 || _calculatemethod == MTD5)
                            {
                                _deliverydate = _datakauio.tbpopulation_new_DeliveryDate.GetValueOrDefault().ToLocalTime();
                                DateTime _today = DateTime.Now;
                                DateTime _msperiodstart = _mastermarketsize.tss_MSPeriodStart.GetValueOrDefault().ToLocalTime();
                                DateTime _msperiodend = _mastermarketsize.tss_MSPeriodEnd.GetValueOrDefault().ToLocalTime();

                                tss_mastermarketsizelines _mastermarketsizelines = new tss_mastermarketsizelines();

                                _mastermarketsizelines.tss_MasterMarketSizeRef = new EntityReference(_mastermarketsize.LogicalName, _mastermarketsizeid);
                                _mastermarketsizelines.tss_MethodCalculationUsed = new OptionSetValue(_calculatemethod);
                                _mastermarketsizelines.tss_Status = new OptionSetValue(STATUS_LINES_DRAFT);
                                _mastermarketsizelines.tss_StatusReason = new OptionSetValue(STATUS_REASON_LINES_OPEN);

                                //CALCULATE AGING
                                CalculateDate _calculate = new CalculateDate();

                                int _aging = 0;
                                int _agingmsperiod = _calculate.DiffYear(_msperiodstart, _msperiodend);
                                int _agingdeliverydate = _calculate.DiffYear(_deliverydate, _today);

                                _aging = _agingmsperiod + _agingdeliverydate;

                                _mastermarketsizelines.tss_Aging = Convert.ToInt32(_aging);

                                Guid _mastermarketsizelinesid = organizationService.Create(_mastermarketsizelines);
                            }
                            else
                                _errordescription = "Process Generate Lines FAILED. Calculate Method is NOT Found !";


                            #region UPDATE KEY ACCOUNT / KA UIO / MASTER MARKET SIZE
                            if (_errordescription == "")
                            {
                                #region UPDATE STATUS KA UIO (SUCCESS)
                                tss_kauio _kauio = new tss_kauio();
                                _kauio.Id = _datakauio.tbkauio_tss_kauioId.GetValueOrDefault();
                                _kauio.tss_CalculateStatus = true;
                                _kauio.tss_errordescription = "";
                                organizationService.Update(_kauio);
                                #endregion UPDATE STATUS KA UIO (SUCCESS)
                            }
                            else
                            {
                                _kasuccess = false;

                                #region UPDATE STATUS MASTER MARKET SIZE DAN KEY ACCOUNT DAN KA UIO (FAILED)
                                tss_kauio _kauio = new tss_kauio();
                                _kauio.Id = _datakauio.tbkauio_tss_kauioId.GetValueOrDefault();
                                _kauio.tss_CalculateStatus = false;
                                _kauio.tss_errordescription = _errordescription;
                                organizationService.Update(_kauio);

                                tss_MasterMarketSize tss_mastermarketsize = new tss_MasterMarketSize();
                                tss_mastermarketsize.Id = _mastermarketsizeid;
                                tss_mastermarketsize.tss_Status = new OptionSetValue(STATUS_ERROR);
                                tss_mastermarketsize.tss_StatusReason = new OptionSetValue(STATUSREASON_ERROR_MS);
                                tss_mastermarketsize.tss_ErrorMessage = _errordescription;
                                organizationService.Update(tss_mastermarketsize);
                                #endregion UPDATE STATUS MASTER MARKET SIZE DAN KEY ACCOUNT DAN KA UIO (FAILED)
                            }
                            #endregion UPDATE KEY ACCOUNT / KA UIO / MASTER MARKET SIZE

                            #endregion GENERATE MARKET SIZE LINES KA UIO
                            #endregion --------------------------------------------------------------------------------------------------------------------------------------------------------------
                            #endregion --------------------------------------------------------------------------------------------------------------------------------------------------------------
                        }
                        else
                        {
                            //GENERATE MARKET SIZE UIO / NON UIO ( ERROR )

                            _kasuccess = false;

                            List<string> _errorcollection = new List<string>();
                            string _errormessage = "";

                            if (!_datakauio.tbpopulation_tss_MSCurrentHourMeter.HasValue)
                                _errorcollection.Add("Current Hour Meter");

                            if (!_datakauio.tbpopulation_tss_MSCurrentHourMeterDate.HasValue)
                                _errorcollection.Add("Current Hour Meter (Date)");

                            if (!_datakauio.tbpopulation_tss_MSLastHourMeter.HasValue)
                                _errorcollection.Add("Last Hour Meter");

                            if (!_datakauio.tbpopulation_tss_MSLastHourMeterDate.HasValue)
                                _errorcollection.Add("Last Hour Meter (Date)");

                            if (!_datakauio.tbpopulation_new_DeliveryDate.HasValue)
                                _errorcollection.Add("Delivery Date");

                            if (_errorcollection.Count() > 0)
                            {
                                foreach (var item in _errorcollection)
                                {
                                    _errormessage += item + ", ";
                                }

                                _errormessage = _errormessage.Substring(0, _errormessage.Length - 1) + " still not define !";
                            }
                            else
                                _errormessage = "Current Hour Meter, Current Hour Meter (Date), Last Hour Meter , Last Hour Meter (Date), Delivery Date or Overhaul Date still not define";

                            _mastermarketsize.tss_UnitType = new OptionSetValue(_datakauio.tbpopulation_tss_PopulationStatus.GetValueOrDefault() ? UNITTYPE_UIO : UNITTYPE_NONUIO);
                            _mastermarketsize.tss_MSPeriodStart = _datakauio.tbkeyaccount_tss_MSPeriodStart.GetValueOrDefault().ToLocalTime();
                            _mastermarketsize.tss_MSPeriodEnd = _datakauio.tbkeyaccount_tss_MSPeriodEnd.GetValueOrDefault().ToLocalTime();
                            _mastermarketsize.tss_ActivePeriodStart = _datakauio.tbkeyaccount_tss_ActiveStartDate.GetValueOrDefault().ToLocalTime();
                            _mastermarketsize.tss_ActivePeriodSEnd = _datakauio.tbkeyaccount_tss_ActiveEndDate.GetValueOrDefault();
                            _mastermarketsize.tss_msperiod = _datakauio.tbkeyaccount_tss_Revision.GetValueOrDefault();

                            _mastermarketsize.tss_PSS = new EntityReference(_datakauio.tbkauio_LogicalName, _datakauio.tbkauio_tss_PSS.Id);
                            _mastermarketsize.tss_Customer = new EntityReference(_datakauio.tbkauio_LogicalName, _datakauio.tbkauio_tss_Customer.Id);
                            _mastermarketsize.tss_SerialNumber = new EntityReference(_datakauio.tbpopulation_LogicalName, _datakauio.tbpopulation_new_populationId.GetValueOrDefault());
                            _mastermarketsize.tss_Status = new OptionSetValue(STATUS_ERROR_MS);
                            _mastermarketsize.tss_StatusReason = new OptionSetValue(STATUSREASON_ERROR_MS);
                            _mastermarketsize.tss_ErrorMessage = _errormessage;
                            _mastermarketsize.tss_UseTyre = _datakauio.tbproduct_tss_UseTyre.HasValue ? _datakauio.tbproduct_tss_UseTyre.GetValueOrDefault() : false;
                            _mastermarketsize.tss_keyaccountid = new EntityReference(_datakauio.tbkeyaccount_LogicalName, _datakauio.tbkeyaccount_tss_keyaccountId.GetValueOrDefault());
                            _mastermarketsize.tss_ismsresultpssgenerated = false;
                            _mastermarketsize.tss_issublinesgenerated = true;

                            Guid _mastermarketsizeid = organizationService.Create(_mastermarketsize);
                        }
                        #endregion

                        #region UPDATE KEY ACCOUNT
                        if (_kasuccess)
                        {
                            tss_keyaccount new_keyaccount = new tss_keyaccount();
                            new_keyaccount.Id = _datakauio.tbkeyaccount_tss_keyaccountId.GetValueOrDefault();
                            new_keyaccount.tss_Status = new OptionSetValue(STATUS_CALCULATE);
                            new_keyaccount.tss_Reason = new OptionSetValue(STATUS_REASON_CALCULATE);
                            organizationService.Update(new_keyaccount);
                        }
                        else
                        {
                            tss_keyaccount new_keyaccount = new tss_keyaccount();
                            new_keyaccount.Id = _datakauio.tbkeyaccount_tss_keyaccountId.GetValueOrDefault();
                            new_keyaccount.tss_Status = new OptionSetValue(STATUS_ERROR);
                            new_keyaccount.tss_Reason = new OptionSetValue(STATUS_REASON_CALCULATE);
                            organizationService.Update(new_keyaccount);
                        }
                        #endregion UPDATE KEY ACCOUNT

                    }

                    #region --------------------------------------------------------------------------------------------------------------------------------------------------------------
                    #region GENERATE SUBLINES - KA UIO / KA NON UIO (TEMP)
                    //var _alldatakauiotemp = (from tballdatakauioheader in _alldatakauioheader.AsEnumerable()
                    //                         join tbmastermarketsize in _context.tss_MasterMarketSizeSet on new
                    //                         {
                    //                             keyaccount = (Guid)tballdatakauioheader.tbkeyaccount_tss_keyaccountId,
                    //                             serialnumber = (Guid)tballdatakauioheader.tbpopulation_new_populationId,
                    //                             activeperiodstart = tballdatakauioheader.tbkeyaccount_tss_ActiveStartDate,
                    //                             activeperiodend = tballdatakauioheader.tbkeyaccount_tss_ActiveEndDate
                    //                         }
                    //                         equals new
                    //                         {
                    //                             keyaccount = tbmastermarketsize.tss_keyaccountid.Id,
                    //                             serialnumber = tbmastermarketsize.tss_SerialNumber.Id,
                    //                             activeperiodstart = tbmastermarketsize.tss_ActivePeriodStart,
                    //                             activeperiodend = tbmastermarketsize.tss_ActivePeriodSEnd
                    //                         }
                    //                         join tbmastermarketsizelines in _context.tss_mastermarketsizelinesSet on tbmastermarketsize.tss_MasterMarketSizeId equals tbmastermarketsizelines.tss_MasterMarketSizeRef.Id
                    //                         join tbpopulation in _context.new_populationSet on tbmastermarketsize.tss_SerialNumber.Id equals tbpopulation.new_populationId
                    //                         where ((tbmastermarketsizelines.tss_PMPeriod > 0 && tbmastermarketsizelines.tss_DueDate != null)
                    //                            || tbmastermarketsizelines.tss_Aging > 0)
                    //                         select new
                    //                         {
                    //                             tbmastermarketsize_tss_MSPeriodStart = tbmastermarketsize.tss_MSPeriodStart,
                    //                             tbmastermarketsize_tss_MSPeriodEnd = tbmastermarketsize.tss_MSPeriodEnd,
                    //                             tbmastermarketsizelines_LogicalName = tbmastermarketsizelines.LogicalName,
                    //                             tbmastermarketsizelines_tss_mastermarketsizelinesId = tbmastermarketsizelines.tss_mastermarketsizelinesId,
                    //                             tbmastermarketsizelines_tss_MethodCalculationUsed = tbmastermarketsizelines.tss_MethodCalculationUsed.Value,
                    //                             tbmastermarketsizelines_tss_PM = tbmastermarketsizelines.tss_PM,
                    //                             tbpopulation_new_populationId = tbpopulation.new_populationId,
                    //                             tbpopulation_trs_ProductMaster = tbpopulation.trs_ProductMaster,
                    //                             tbpopulation_new_DeliveryDate = tbpopulation.new_DeliveryDate
                    //                         }).ToList();

                    //// GENERATE SUBLINES - KA UIO / KA NON UIO - BY PM PERIOD
                    //var _alldatakauiobypmperiod = (from tballdatakauiotemp in _alldatakauiotemp.AsEnumerable()
                    //                               join tbmarketsizepartconsump in _context.tss_MarketSizePartConsumpSet on tballdatakauiotemp.tbpopulation_trs_ProductMaster.Id equals tbmarketsizepartconsump.tss_Model.Id
                    //                               join tbmasterpart in _context.trs_masterpartSet on tbmarketsizepartconsump.tss_PartNumber.Id equals tbmasterpart.trs_masterpartId
                    //                               join tbpartmasterlinesmodel in _context.tss_partmasterlinesmodelSet on tbmarketsizepartconsump.tss_PartNumber.Id equals tbpartmasterlinesmodel.tss_PartMasterId.Id
                    //                               where tbmarketsizepartconsump.tss_TypePM == tballdatakauiotemp.tbmastermarketsizelines_tss_PM
                    //                               && tbmarketsizepartconsump.tss_Model.Id == tballdatakauiotemp.tbpopulation_new_populationId
                    //                               && tbpartmasterlinesmodel.tss_Model.Id == tballdatakauiotemp.tbpopulation_new_populationId
                    //                               select new
                    //                               {
                    //                                   tballdatakauiotemp_tbmastermarketsizelines_LogicalName = tballdatakauiotemp.tbmastermarketsizelines_LogicalName,
                    //                                   tballdatakauiotemp_tbmastermarketsizelines_tss_mastermarketsizelinesId = tballdatakauiotemp.tbmastermarketsizelines_tss_mastermarketsizelinesId,
                    //                                   tbmasterpart_LogicalName = tbmasterpart.LogicalName,
                    //                                   tbmasterpart_trs_masterpartId = tbmasterpart.trs_masterpartId,
                    //                                   tbmasterpart_trs_PartDescription = tbmasterpart.trs_PartDescription,
                    //                                   tbmarketsizepartconsump_tss_Qty = tbmarketsizepartconsump.tss_Qty
                    //                               }).ToList();

                    //foreach (var _datakauiobypmperiod in _alldatakauiobypmperiod)
                    //{
                    //    decimal _price = 0;
                    //    decimal _minimumprice = 0;

                    //    QueryExpression _queryexpression = new QueryExpression(_DL_tss_sparepartpricemaster.EntityName);
                    //    _queryexpression.LinkEntities.Add(new LinkEntity(_DL_tss_sparepartpricemaster.EntityName, _DL_tss_pricelistpart.EntityName, "tss_pricelistpart", "tss_pricelistpartid", JoinOperator.Inner));
                    //    _queryexpression.Criteria.AddCondition("tss_partmaster", ConditionOperator.Equal, _datakauiobypmperiod.tbmasterpart_trs_masterpartId);
                    //    _queryexpression.LinkEntities[0].LinkCriteria.AddCondition("tss_type", ConditionOperator.Equal, MS_TYPE);
                    //    _queryexpression.LinkEntities[0].EntityAlias = "tss_pricelistpart";
                    //    _queryexpression.ColumnSet = new ColumnSet(false);
                    //    _queryexpression.ColumnSet.AddColumns("tss_price", "tss_minimumprice");

                    //    EntityCollection _sparepartpricemastercollection = organizationService.RetrieveMultiple(_queryexpression);

                    //    if (_sparepartpricemastercollection.Entities.Count() > 0)
                    //    {
                    //        _price = _sparepartpricemastercollection.Entities[0].Contains("tss_price") ? _sparepartpricemastercollection.Entities[0].GetAttributeValue<Money>("tss_price").Value : 0;
                    //        _minimumprice = _sparepartpricemastercollection.Entities[0].Contains("tss_minimumprice") ? _sparepartpricemastercollection.Entities[0].GetAttributeValue<Money>("tss_minimumprice").Value : 0;
                    //    }

                    //    // INSERT
                    //    tss_mastermarketsizesublines _mastermarketsizesublines = new tss_mastermarketsizesublines();

                    //    _mastermarketsizesublines.tss_MasterMSLinesRef = new EntityReference(_datakauiobypmperiod.tballdatakauiotemp_tbmastermarketsizelines_LogicalName, (Guid)_datakauiobypmperiod.tballdatakauiotemp_tbmastermarketsizelines_tss_mastermarketsizelinesId);
                    //    _mastermarketsizesublines.tss_PartNumber = new EntityReference(_datakauiobypmperiod.tbmasterpart_LogicalName, (Guid)_datakauiobypmperiod.tbmasterpart_trs_masterpartId);
                    //    _mastermarketsizesublines.tss_partdescription = _datakauiobypmperiod.tbmasterpart_trs_PartDescription;
                    //    _mastermarketsizesublines.tss_Qty = _datakauiobypmperiod.tbmarketsizepartconsump_tss_Qty;
                    //    _mastermarketsizesublines.tss_originalqty = _datakauiobypmperiod.tbmarketsizepartconsump_tss_Qty;
                    //    _mastermarketsizesublines.tss_Price = new Money(_price);
                    //    _mastermarketsizesublines.tss_MinimumPrice = new Money(_minimumprice);

                    //    organizationService.Create(_mastermarketsizesublines);
                    //}

                    //CalculateDate _calculatedate = new CalculateDate();

                    //// GENERATE SUBLINES - KA UIO / KA NON UIO - BY AGING
                    //var _alldatakauiobyaging = (from tballdatakauiotemp in _alldatakauiotemp.AsEnumerable()
                    //                            join tbmarketsizepartconsumpaging in _context.tss_marketsizepartconsumpagingSet on tballdatakauiotemp.tbpopulation_trs_ProductMaster.Id equals tbmarketsizepartconsumpaging.tss_Model.Id
                    //                            join tbmasterpart in _context.trs_masterpartSet on tbmarketsizepartconsumpaging.tss_PartNumber.Id equals tbmasterpart.trs_masterpartId
                    //                            join tbpartmasterlinesmodel in _context.tss_partmasterlinesmodelSet on tbmarketsizepartconsumpaging.tss_PartNumber.Id equals tbpartmasterlinesmodel.tss_PartMasterId.Id
                    //                            where
                    //                            (
                    //                                (
                    //                                    tbmarketsizepartconsumpaging.tss_RangeAgingFrom <= (_calculatedate.DiffYear(tballdatakauiotemp.tbmastermarketsize_tss_MSPeriodStart.Value.ToLocalTime(), tballdatakauiotemp.tbmastermarketsize_tss_MSPeriodEnd.Value.ToLocalTime()) + _calculatedate.DiffYear(tballdatakauiotemp.tbpopulation_new_DeliveryDate.Value.ToLocalTime(),DateTime.Now.ToLocalTime()))
                    //                                    && tbmarketsizepartconsumpaging.tss_RangeAgingTo >= (_calculatedate.DiffYear(tballdatakauiotemp.tbmastermarketsize_tss_MSPeriodStart.Value.ToLocalTime(), tballdatakauiotemp.tbmastermarketsize_tss_MSPeriodEnd.Value.ToLocalTime()) + _calculatedate.DiffYear(tballdatakauiotemp.tbpopulation_new_DeliveryDate.Value.ToLocalTime(), DateTime.Now.ToLocalTime()))
                    //                                )
                    //                                || tbmarketsizepartconsumpaging.tss_Aging == (_calculatedate.DiffYear(tballdatakauiotemp.tbmastermarketsize_tss_MSPeriodStart.Value.ToLocalTime(), tballdatakauiotemp.tbmastermarketsize_tss_MSPeriodEnd.Value.ToLocalTime()) + _calculatedate.DiffYear(tballdatakauiotemp.tbpopulation_new_DeliveryDate.Value.ToLocalTime(),DateTime.Now.ToLocalTime()))
                    //                            )
                    //                            && tbmarketsizepartconsumpaging.tss_Model.Id == tballdatakauiotemp.tbpopulation_new_populationId
                    //                            && tbpartmasterlinesmodel.tss_Model.Id == tballdatakauiotemp.tbpopulation_new_populationId
                    //                            select new
                    //                            {
                    //                                tballdatakauiotemp_tbmastermarketsizelines_LogicalName = tballdatakauiotemp.tbmastermarketsizelines_LogicalName,
                    //                                tballdatakauiotemp_tbmastermarketsizelines_tss_mastermarketsizelinesId = tballdatakauiotemp.tbmastermarketsizelines_tss_mastermarketsizelinesId,
                    //                                tbmasterpart_LogicalName = tbmasterpart.LogicalName,
                    //                                tbmasterpart_trs_masterpartId = tbmasterpart.trs_masterpartId,
                    //                                tbmasterpart_trs_PartDescription = tbmasterpart.trs_PartDescription,
                    //                                tbmarketsizepartconsump_tss_Qty = tbmarketsizepartconsumpaging.tss_Qty
                    //                            }).ToList();

                    //foreach (var _datakauiobyaging in _alldatakauiobyaging)
                    //{
                    //    decimal _price = 0;
                    //    decimal _minimumprice = 0;

                    //    QueryExpression _queryexpression = new QueryExpression(_DL_tss_sparepartpricemaster.EntityName);
                    //    _queryexpression.LinkEntities.Add(new LinkEntity(_DL_tss_sparepartpricemaster.EntityName, _DL_tss_pricelistpart.EntityName, "tss_pricelistpart", "tss_pricelistpartid", JoinOperator.Inner));
                    //    _queryexpression.Criteria.AddCondition("tss_partmaster", ConditionOperator.Equal, _datakauiobyaging.tbmasterpart_trs_masterpartId);
                    //    _queryexpression.LinkEntities[0].LinkCriteria.AddCondition("tss_type", ConditionOperator.Equal, MS_TYPE);
                    //    _queryexpression.LinkEntities[0].EntityAlias = "tss_pricelistpart";
                    //    _queryexpression.ColumnSet = new ColumnSet(false);
                    //    _queryexpression.ColumnSet.AddColumns("tss_price", "tss_minimumprice");

                    //    EntityCollection _sparepartpricemastercollection = organizationService.RetrieveMultiple(_queryexpression);

                    //    if (_sparepartpricemastercollection.Entities.Count() > 0)
                    //    {
                    //        _price = _sparepartpricemastercollection.Entities[0].Contains("tss_price") ? _sparepartpricemastercollection.Entities[0].GetAttributeValue<Money>("tss_price").Value : 0;
                    //        _minimumprice = _sparepartpricemastercollection.Entities[0].Contains("tss_minimumprice") ? _sparepartpricemastercollection.Entities[0].GetAttributeValue<Money>("tss_minimumprice").Value : 0;
                    //    }

                    //    // INSERT
                    //    tss_mastermarketsizesublines _mastermarketsizesublines = new tss_mastermarketsizesublines();

                    //    _mastermarketsizesublines.tss_MasterMSLinesRef = new EntityReference(_datakauiobyaging.tballdatakauiotemp_tbmastermarketsizelines_LogicalName, (Guid)_datakauiobyaging.tballdatakauiotemp_tbmastermarketsizelines_tss_mastermarketsizelinesId);
                    //    _mastermarketsizesublines.tss_PartNumber = new EntityReference(_datakauiobyaging.tbmasterpart_LogicalName, (Guid)_datakauiobyaging.tbmasterpart_trs_masterpartId);
                    //    _mastermarketsizesublines.tss_partdescription = _datakauiobyaging.tbmasterpart_trs_PartDescription;
                    //    _mastermarketsizesublines.tss_Qty = _datakauiobyaging.tbmarketsizepartconsump_tss_Qty;
                    //    _mastermarketsizesublines.tss_originalqty = _datakauiobyaging.tbmarketsizepartconsump_tss_Qty;
                    //    _mastermarketsizesublines.tss_Price = new Money(_price);
                    //    _mastermarketsizesublines.tss_MinimumPrice = new Money(_minimumprice);

                    //    organizationService.Create(_mastermarketsizesublines);
                    //}
                    #endregion GENERATE SUBLINES - KA UIO / KA NON UIO (TEMP)
                    #endregion --------------------------------------------------------------------------------------------------------------------------------------------------------------

                    #endregion GENERATE KA UIO

                    #region GENERATE MARKET SIZE KA GROUP UIO COMMODITY
                    var _alldatakagroupuioheader = (from tbkeyaccount in _context.tss_keyaccountSet
                                                    join tbkagroupuiocommodity in _context.tss_kagroupuiocommoditySet on tbkeyaccount.tss_keyaccountId equals tbkagroupuiocommodity.tss_KeyAccountId.Id
                                                    join tbgroupuiocommodityaccount in _context.tss_groupuiocommodityaccountSet on tbkagroupuiocommodity.tss_GroupUIOCommodity.Id equals tbgroupuiocommodityaccount.tss_groupuiocommodityaccountId
                                                    join tbgroupuiocommodityheader in _context.tss_groupuiocommodityheaderSet on tbgroupuiocommodityaccount.tss_GroupUIOCommodityHeader.Id equals tbgroupuiocommodityheader.tss_groupuiocommodityheaderId
                                                    where (tbkeyaccount.tss_Status.Equals(STATUS_OPEN) || tbkeyaccount.tss_Status.Equals(STATUS_ERROR))
                                                       && tbkeyaccount.tss_PSS.Id == context.UserId
                                                       && tbkeyaccount.tss_Customer != null
                                                       && tbkeyaccount.tss_ActiveEndDate >= DateTime.Now
                                                       && tbkeyaccount.tss_ActiveStartDate <= DateTime.Now
                                                       && tbkeyaccount.tss_CalculatetoMS == true
                                                       && tbkeyaccount.StatusCode.Value == 1
                                                       && tbkagroupuiocommodity.tss_CalculatetoMS == true
                                                       && tbkagroupuiocommodity.tss_CalculateStatus == false
                                                       && tbgroupuiocommodityaccount.tss_Qty > 0
                                                    //&& _keyaccountids.Contains(new Guid(tbkeyaccount.tss_keyaccountId.GetValueOrDefault().ToString()))
                                                    select new
                                                    {
                                                        tbkeyaccount_LogicalName = tbkeyaccount.LogicalName,
                                                        tbkeyaccount_tss_keyaccountId = tbkeyaccount.tss_keyaccountId,
                                                        tbkeyaccount_tss_MSPeriodStart = tbkeyaccount.tss_MSPeriodStart,
                                                        tbkeyaccount_tss_MSPeriodEnd = tbkeyaccount.tss_MSPeriodEnd,
                                                        tbkeyaccount_tss_ActiveStartDate = tbkeyaccount.tss_ActiveStartDate,
                                                        tbkeyaccount_tss_ActiveEndDate = tbkeyaccount.tss_ActiveEndDate,
                                                        tbkeyaccount_tss_Revision = tbkeyaccount.tss_Revision,
                                                        tbkagroupuiocommodity_LogicalName = tbkagroupuiocommodity.LogicalName,
                                                        tbkagroupuiocommodity_tss_kagroupuiocommodityId = tbkagroupuiocommodity.tss_kagroupuiocommodityId,
                                                        tbkagroupuiocommodity_tss_pss = tbkagroupuiocommodity.tss_pss,
                                                        tbkagroupuiocommodity_tss_customer = tbkagroupuiocommodity.tss_customer,
                                                        tbgroupuiocommodityaccount_LogicalName = tbgroupuiocommodityaccount.LogicalName,
                                                        tbgroupuiocommodityaccount_tss_Qty = tbgroupuiocommodityaccount.tss_Qty,
                                                        tbgroupuiocommodityheader_LogicalName = tbgroupuiocommodityheader.LogicalName,
                                                        tbgroupuiocommodityheader_tss_groupuiocommodityheaderId = tbgroupuiocommodityheader.tss_groupuiocommodityheaderId
                                                    }).ToList();

                    foreach (var _datakagroupuio in _alldatakagroupuioheader)
                    {
                        int _quantity = _datakagroupuio.tbgroupuiocommodityaccount_tss_Qty.HasValue ? _datakagroupuio.tbgroupuiocommodityaccount_tss_Qty.GetValueOrDefault() : 0;

                        if (_quantity > 0)
                        {
                            tss_MasterMarketSize _mastermarketsize = new tss_MasterMarketSize();

                            _mastermarketsize.tss_PSS = new EntityReference(_datakagroupuio.tbkagroupuiocommodity_LogicalName, _datakagroupuio.tbkagroupuiocommodity_tss_pss.Id);
                            _mastermarketsize.tss_Customer = new EntityReference(_datakagroupuio.tbkagroupuiocommodity_LogicalName, _datakagroupuio.tbkagroupuiocommodity_tss_customer.Id);
                            _mastermarketsize.tss_GroupUIOCommodityHeader = new EntityReference(_datakagroupuio.tbgroupuiocommodityheader_LogicalName, _datakagroupuio.tbgroupuiocommodityheader_tss_groupuiocommodityheaderId.GetValueOrDefault());
                            _mastermarketsize.tss_Qty = _quantity;
                            _mastermarketsize.tss_UnitType = new OptionSetValue(UNITTYPE_COMMODITY);
                            _mastermarketsize.tss_Status = new OptionSetValue(STATUS_COMPLETED_MS);
                            _mastermarketsize.tss_StatusReason = new OptionSetValue(STATUSREASON_COMPLETED_MS);
                            _mastermarketsize.tss_MSPeriodStart = _datakagroupuio.tbkeyaccount_tss_MSPeriodStart.GetValueOrDefault().ToLocalTime();
                            _mastermarketsize.tss_MSPeriodEnd = _datakagroupuio.tbkeyaccount_tss_MSPeriodEnd.GetValueOrDefault().ToLocalTime();
                            _mastermarketsize.tss_ActivePeriodStart = _datakagroupuio.tbkeyaccount_tss_ActiveStartDate.GetValueOrDefault().ToLocalTime();
                            _mastermarketsize.tss_ActivePeriodSEnd = _datakagroupuio.tbkeyaccount_tss_ActiveEndDate.GetValueOrDefault();
                            _mastermarketsize.tss_keyaccountid = new EntityReference(_datakagroupuio.tbkeyaccount_LogicalName, _datakagroupuio.tbkeyaccount_tss_keyaccountId.GetValueOrDefault());
                            _mastermarketsize.tss_msperiod = _datakagroupuio.tbkeyaccount_tss_Revision.GetValueOrDefault();
                            _mastermarketsize.tss_ismsresultpssgenerated = false;
                            _mastermarketsize.tss_issublinesgenerated = false;

                            Guid _mastermarketsizeid = organizationService.Create(_mastermarketsize);
                        }
                        else
                        {
                            _kasuccess = false;

                            tss_MasterMarketSize _mastermarketsize = new tss_MasterMarketSize();

                            _mastermarketsize.tss_PSS = new EntityReference(_datakagroupuio.tbkagroupuiocommodity_LogicalName, _datakagroupuio.tbkagroupuiocommodity_tss_pss.Id);
                            _mastermarketsize.tss_Customer = new EntityReference(_datakagroupuio.tbkagroupuiocommodity_LogicalName, _datakagroupuio.tbkagroupuiocommodity_tss_customer.Id);
                            _mastermarketsize.tss_UnitType = new OptionSetValue(UNITTYPE_COMMODITY);
                            _mastermarketsize.tss_Status = new OptionSetValue(STATUS_ERROR_MS);
                            _mastermarketsize.tss_StatusReason = new OptionSetValue(STATUSREASON_ERROR_MS);
                            _mastermarketsize.tss_ErrorMessage = "Quantity NOT Found !";
                            _mastermarketsize.tss_MSPeriodStart = _datakagroupuio.tbkeyaccount_tss_MSPeriodStart.GetValueOrDefault().ToLocalTime();
                            _mastermarketsize.tss_MSPeriodEnd = _datakagroupuio.tbkeyaccount_tss_MSPeriodEnd.GetValueOrDefault().ToLocalTime();
                            _mastermarketsize.tss_ActivePeriodStart = _datakagroupuio.tbkeyaccount_tss_ActiveStartDate.GetValueOrDefault().ToLocalTime();
                            _mastermarketsize.tss_ActivePeriodSEnd = _datakagroupuio.tbkeyaccount_tss_ActiveEndDate.GetValueOrDefault();
                            _mastermarketsize.tss_keyaccountid = new EntityReference(_datakagroupuio.tbkeyaccount_LogicalName, _datakagroupuio.tbkeyaccount_tss_keyaccountId.GetValueOrDefault());
                            _mastermarketsize.tss_msperiod = _datakagroupuio.tbkeyaccount_tss_Revision.GetValueOrDefault();
                            _mastermarketsize.tss_ismsresultpssgenerated = false;
                            _mastermarketsize.tss_issublinesgenerated = true;

                            Guid _mastermarketsizeid = organizationService.Create(_mastermarketsize);
                        }

                        if (_kasuccess)
                        {
                            tss_keyaccount new_keyaccount = new tss_keyaccount();
                            new_keyaccount.Id = _datakagroupuio.tbkeyaccount_tss_keyaccountId.GetValueOrDefault();
                            new_keyaccount.tss_Status = new OptionSetValue(STATUS_CALCULATE);
                            new_keyaccount.tss_Reason = new OptionSetValue(STATUS_REASON_CALCULATE);
                            organizationService.Update(new_keyaccount);
                        }
                        else
                        {
                            tss_keyaccount new_keyaccount = new tss_keyaccount();
                            new_keyaccount.Id = _datakagroupuio.tbkeyaccount_tss_keyaccountId.GetValueOrDefault();
                            new_keyaccount.tss_Status = new OptionSetValue(STATUS_ERROR);
                            new_keyaccount.tss_Reason = new OptionSetValue(STATUS_REASON_CALCULATE);
                            organizationService.Update(new_keyaccount);
                        }
                    }

                    #region --------------------------------------------------------------------------------------------------------------------------------------------------------------
                    #region GENERATE SUBLINES - GROUP UIO COMMODITY
                    //var _alldatakagroupuio = (from tballdatakagroupuioheader in _alldatakagroupuioheader.AsEnumerable()
                    //                          join tbmastermarketsize in _context.tss_MasterMarketSizeSet on new
                    //                          {
                    //                              keyaccount = (Guid)tballdatakagroupuioheader.tbkeyaccount_tss_keyaccountId,
                    //                              groupcommodityheader = (Guid)tballdatakagroupuioheader.tbgroupuiocommodityheader_tss_groupuiocommodityheaderId,
                    //                              activeperiodstart = tballdatakagroupuioheader.tbkeyaccount_tss_ActiveStartDate,
                    //                              activeperiodend = tballdatakagroupuioheader.tbkeyaccount_tss_ActiveEndDate
                    //                          }
                    //                          equals new
                    //                          {
                    //                              keyaccount = (Guid)tbmastermarketsize.tss_keyaccountid.Id,
                    //                              groupcommodityheader = (Guid)tbmastermarketsize.tss_GroupUIOCommodityHeader.Id,
                    //                              activeperiodstart = tbmastermarketsize.tss_ActivePeriodStart,
                    //                              activeperiodend = tbmastermarketsize.tss_ActivePeriodSEnd
                    //                          }
                    //                          join tbgroupuiocommodity in _context.tss_groupuiocommoditySet on tballdatakagroupuioheader.tbgroupuiocommodityheader_tss_groupuiocommodityheaderId equals tbgroupuiocommodity.tss_GroupUIOCommodityHeaderId.Id
                    //                          select new
                    //                          {
                    //                              tbmastermarketsize_LogicalName = tbmastermarketsize.LogicalName,
                    //                              tbmastermarketsize_tss_MasterMarketSizeId = tbmastermarketsize.tss_MasterMarketSizeId.Value,
                    //                              tbkagroupuiocommodity_tss_kagroupuiocommodityId = tballdatakagroupuioheader.tbkagroupuiocommodity_tss_kagroupuiocommodityId,
                    //                              tbgroupuiocommodityaccount_tss_Qty = tballdatakagroupuioheader.tbgroupuiocommodityaccount_tss_Qty,
                    //                              tbgroupuiocommodity_LogicalName = tbgroupuiocommodity.LogicalName,
                    //                              tbgroupuiocommodity_tss_PartCommodityType = tbgroupuiocommodity.tss_PartCommodityType,
                    //                              tbgroupuiocommodity_tss_BatteryBy = tbgroupuiocommodity.tss_BatteryBy,
                    //                              tbgroupuiocommodity_tss_BatteryPartNumber = tbgroupuiocommodity.tss_BatteryPartNumber,
                    //                              tbgroupuiocommodity_tss_BatteryType = tbgroupuiocommodity.tss_BatteryType,
                    //                              tbgroupuiocommodity_tss_BatteryTraction = tbgroupuiocommodity.tss_BatteryTraction,
                    //                              tbgroupuiocommodity_tss_BatteryCranking = tbgroupuiocommodity.tss_BatteryCranking,
                    //                              tbgroupuiocommodity_tss_Battery = tbgroupuiocommodity.tss_Battery,
                    //                              tbgroupuiocommodity_tss_Oilby = tbgroupuiocommodity.tss_Oilby,
                    //                              tbgroupuiocommodity_tss_OilPartNumber = tbgroupuiocommodity.tss_OilPartNumber,
                    //                              tbgroupuiocommodity_tss_JenisOil = tbgroupuiocommodity.tss_JenisOil,
                    //                              tbgroupuiocommodity_tss_OilQtyby = tbgroupuiocommodity.tss_OilQtyby,
                    //                              tbgroupuiocommodity_tss_OilQtyPcs = tbgroupuiocommodity.tss_OilQtyPcs,
                    //                              tbgroupuiocommodity_tss_Oil = tbgroupuiocommodity.tss_Oil,
                    //                              tbgroupuiocommodity_tss_Tyreby = tbgroupuiocommodity.tss_Tyreby,
                    //                              tbgroupuiocommodity_tss_TyrePartNumber = tbgroupuiocommodity.tss_TyrePartNumber,
                    //                              tbgroupuiocommodity_tss_TyreSpec = tbgroupuiocommodity.tss_TyreSpec,
                    //                              tbgroupuiocommodity_tss_Tyre = tbgroupuiocommodity.tss_Tyre
                    //                          }).ToList();

                    //foreach (var _datakagroupuio in _alldatakagroupuio)
                    //{
                    //    #region --------------------------------------------------------------------------------------------------------------------------------------------------------------
                    //    #region --------------------------------------------------------------------------------------------------------------------------------------------------------------
                    //    #region GENERATE MARKET SIZE SUBLINES KA GROUP UIO COMMODITY

                    //    Guid _partnumberid = new Guid();
                    //    int _quantity = _datakagroupuio.tbgroupuiocommodityaccount_tss_Qty.HasValue ? _datakagroupuio.tbgroupuiocommodityaccount_tss_Qty.GetValueOrDefault() : 0;

                    //    tss_mastermarketsizesublines _mastermarketsizesublines = new tss_mastermarketsizesublines();
                    //    _mastermarketsizesublines.tss_MasterMarketSizeId = new EntityReference(_datakagroupuio.tbmastermarketsize_LogicalName, _datakagroupuio.tbmastermarketsize_tss_MasterMarketSizeId);

                    //    //OPTION SET - BATTERY / OIL / TYRE
                    //    if (_datakagroupuio.tbgroupuiocommodity_tss_PartCommodityType.Value == PART_COMMODITYTYPE_BATTERY)
                    //    {
                    //        Guid _partnumberbattery = new Guid();

                    //        if (_datakagroupuio.tbgroupuiocommodity_tss_BatteryBy.Value == PART_COMMODITYTYPE_BY_PART)
                    //        {
                    //            _partnumberbattery = _datakagroupuio.tbgroupuiocommodity_tss_BatteryPartNumber.Id;

                    //            if (_partnumberbattery != null)
                    //            {
                    //                _partnumberid = _partnumberbattery;
                    //                _mastermarketsizesublines.tss_PartNumber = new EntityReference("trs_masterpart", _partnumberbattery);
                    //            }
                    //        }
                    //        else if (_datakagroupuio.tbgroupuiocommodity_tss_BatteryBy.Value == PART_COMMODITYTYPE_BY_SPEC)
                    //        {
                    //            //BATTERY TYPE - TRACTION / CRANKING
                    //            if (_datakagroupuio.tbgroupuiocommodity_tss_BatteryType.Value == 865920000)
                    //            {
                    //                QueryExpression _queryexpression = new QueryExpression("tss_partmasterlinesbattery");
                    //                _queryexpression.ColumnSet = new ColumnSet(true);
                    //                _queryexpression.Criteria = new FilterExpression
                    //                {
                    //                    FilterOperator = LogicalOperator.And,
                    //                    Conditions =
                    //                        {
                    //                            new ConditionExpression("tss_batterymodeltraction", ConditionOperator.Equal, _datakagroupuio.tbgroupuiocommodity_tss_BatteryTraction.Id)
                    //                        }
                    //                };

                    //                EntityCollection _batterytraction = organizationService.RetrieveMultiple(_queryexpression);

                    //                if (_batterytraction.Entities.Count() > 0)
                    //                {
                    //                    _partnumberid = _batterytraction.Entities[0].GetAttributeValue<EntityReference>("tss_partmasterid").Id;
                    //                    _partnumberbattery = _partnumberid;
                    //                    _mastermarketsizesublines.tss_PartNumber = new EntityReference("trs_masterpart", _partnumberbattery);
                    //                }
                    //            }
                    //            else if (_datakagroupuio.tbgroupuiocommodity_tss_BatteryBy.Value == 865920001)
                    //            {
                    //                QueryExpression _queryexpression = new QueryExpression("tss_partmasterlinesbattery");
                    //                _queryexpression.ColumnSet = new ColumnSet(true);
                    //                _queryexpression.Criteria = new FilterExpression
                    //                {
                    //                    FilterOperator = LogicalOperator.And,
                    //                    Conditions =
                    //                        {
                    //                            new ConditionExpression("tss_batterymodelcranking", ConditionOperator.Equal, _datakagroupuio.tbgroupuiocommodity_tss_BatteryCranking.Id)
                    //                        }
                    //                };
                    //                EntityCollection _batterycranking = organizationService.RetrieveMultiple(_queryexpression);

                    //                if (_batterycranking.Entities.Count() > 0)
                    //                {
                    //                    _partnumberid = _batterycranking.Entities[0].GetAttributeValue<EntityReference>("tss_partmasterid").Id;
                    //                    _partnumberbattery = _partnumberid;
                    //                    _mastermarketsizesublines.tss_PartNumber = new EntityReference("trs_masterpart", _partnumberbattery);
                    //                }
                    //            }
                    //        }

                    //        if (_partnumberbattery != new Guid())
                    //        {
                    //            DL_tss_mastermarketsizesublines _partmaster = GetPartMaster(organizationService, _partnumberbattery);

                    //            _mastermarketsizesublines.tss_Price = new Money(_partmaster.tss_price);
                    //            _mastermarketsizesublines.tss_MinimumPrice = new Money(_partmaster.tss_minimumprice);
                    //            _mastermarketsizesublines.tss_partdescription = _partmaster.tss_partdescription;
                    //        }

                    //        _quantity = _datakagroupuio.tbgroupuiocommodity_tss_Battery.HasValue ? _datakagroupuio.tbgroupuiocommodity_tss_Battery.GetValueOrDefault() : 0;
                    //    }
                    //    else if (_datakagroupuio.tbgroupuiocommodity_tss_PartCommodityType.Value == PART_COMMODITYTYPE_OIL)
                    //    {
                    //        Guid _partnumberoil = new Guid();

                    //        if (_datakagroupuio.tbgroupuiocommodity_tss_Oilby.Value == PART_COMMODITYTYPE_BY_PART)
                    //        {
                    //            _partnumberoil = _datakagroupuio.tbgroupuiocommodity_tss_OilPartNumber.Id;

                    //            if (_partnumberoil != null)
                    //            {
                    //                _partnumberid = _partnumberoil;
                    //                _mastermarketsizesublines.tss_PartNumber = new EntityReference("trs_masterpart", _partnumberoil);
                    //            }
                    //        }
                    //        else if (_datakagroupuio.tbgroupuiocommodity_tss_Oilby.Value == PART_COMMODITYTYPE_BY_SPEC)
                    //        {
                    //            QueryExpression _queryexpression = new QueryExpression("tss_partmasterlinesoil");
                    //            _queryexpression.ColumnSet = new ColumnSet(true);
                    //            _queryexpression.Criteria = new FilterExpression
                    //            {
                    //                FilterOperator = LogicalOperator.And,
                    //                Conditions =
                    //                    {
                    //                        new ConditionExpression("tss_oiltype", ConditionOperator.Equal, _datakagroupuio.tbgroupuiocommodity_tss_JenisOil.Id)
                    //                    }
                    //            };

                    //            EntityCollection _partmasterlinesoil = organizationService.RetrieveMultiple(_queryexpression);

                    //            if (_partmasterlinesoil.Entities.Count() > 0)
                    //            {
                    //                _partnumberid = _partmasterlinesoil.Entities[0].GetAttributeValue<EntityReference>("tss_partmasterid").Id;
                    //                _partnumberoil = _partnumberid;
                    //                _mastermarketsizesublines.tss_PartNumber = new EntityReference("trs_masterpart", _partnumberoil);
                    //            }
                    //        }

                    //        if (_partnumberoil != new Guid())
                    //        {
                    //            DL_tss_mastermarketsizesublines _partmaster = GetPartMaster(organizationService, _partnumberoil);

                    //            _mastermarketsizesublines.tss_Price = new Money(_partmaster.tss_price);
                    //            _mastermarketsizesublines.tss_MinimumPrice = new Money(_partmaster.tss_minimumprice);
                    //            _mastermarketsizesublines.tss_partdescription = _partmaster.tss_partdescription;
                    //        }

                    //        if (_datakagroupuio.tbgroupuiocommodity_tss_OilQtyby.Value == 865920000)
                    //            _quantity = _datakagroupuio.tbgroupuiocommodity_tss_OilQtyPcs.HasValue ? _datakagroupuio.tbgroupuiocommodity_tss_OilQtyPcs.Value : 0;
                    //        else if (_datakagroupuio.tbgroupuiocommodity_tss_OilQtyby.Value == 865920001)
                    //            _quantity = _datakagroupuio.tbgroupuiocommodity_tss_Oil.HasValue ? _datakagroupuio.tbgroupuiocommodity_tss_Oil.Value : 0;
                    //    }
                    //    else if (_datakagroupuio.tbgroupuiocommodity_tss_PartCommodityType.Value == PART_COMMODITYTYPE_TYRE)
                    //    {
                    //        Guid _partnumbertyre = new Guid();

                    //        if (_datakagroupuio.tbgroupuiocommodity_tss_Tyreby.Value == PART_COMMODITYTYPE_BY_PART)
                    //        {
                    //            _partnumbertyre = _datakagroupuio.tbgroupuiocommodity_tss_TyrePartNumber.Id;

                    //            if (_partnumbertyre != null)
                    //            {
                    //                _partnumberid = _partnumbertyre;
                    //                _mastermarketsizesublines.tss_PartNumber = new EntityReference("trs_masterpart", _partnumbertyre);
                    //            }
                    //        }
                    //        else if (_datakagroupuio.tbgroupuiocommodity_tss_Tyreby.Value == PART_COMMODITYTYPE_BY_SPEC)
                    //        {
                    //            Entity _tss_partmasterlinestyretype = organizationService.Retrieve("tss_partmasterlinestyretype", _datakagroupuio.tbgroupuiocommodity_tss_TyreSpec.Id, new ColumnSet(true));

                    //            if (_tss_partmasterlinestyretype != null)
                    //            {
                    //                _partnumberid = _tss_partmasterlinestyretype.GetAttributeValue<EntityReference>("tss_partmasterid").Id;
                    //                _partnumbertyre = _partnumberid;
                    //                _mastermarketsizesublines.tss_PartNumber = new EntityReference("trs_masterpart", _partnumbertyre);
                    //            }
                    //        }

                    //        if (_partnumbertyre != new Guid())
                    //        {
                    //            DL_tss_mastermarketsizesublines _partmaster = GetPartMaster(organizationService, _partnumbertyre);

                    //            _mastermarketsizesublines.tss_Price = new Money(_partmaster.tss_price);
                    //            _mastermarketsizesublines.tss_MinimumPrice = new Money(_partmaster.tss_minimumprice);
                    //            _mastermarketsizesublines.tss_partdescription = _partmaster.tss_partdescription;
                    //        }

                    //        _quantity = _datakagroupuio.tbgroupuiocommodity_tss_Tyre.GetValueOrDefault();
                    //    }

                    //    _mastermarketsizesublines.tss_Qty = _datakagroupuio.tbgroupuiocommodityaccount_tss_Qty.Value * _quantity;
                    //    _mastermarketsizesublines.tss_originalqty = _quantity;

                    //    if (_partnumberid != new Guid())
                    //    {
                    //        organizationService.Create(_mastermarketsizesublines);

                    //        tss_kagroupuiocommodity _kagroupuiocommodity = new tss_kagroupuiocommodity();
                    //        _kagroupuiocommodity.Id = _datakagroupuio.tbkagroupuiocommodity_tss_kagroupuiocommodityId.GetValueOrDefault();
                    //        _kagroupuiocommodity.tss_CalculateStatus = true;
                    //        organizationService.Update(_kagroupuiocommodity);
                    //    }
                    //    else
                    //    {
                    //        _kasuccess = false;

                    //        tss_kagroupuiocommodity _kagroupuiocommodity = new tss_kagroupuiocommodity();
                    //        _kagroupuiocommodity.Id = _datakagroupuio.tbkagroupuiocommodity_tss_kagroupuiocommodityId.GetValueOrDefault();
                    //        _kagroupuiocommodity.tss_CalculateStatus = false;
                    //        organizationService.Update(_kagroupuiocommodity);

                    //        tss_MasterMarketSize tss_mastermarketsize = new tss_MasterMarketSize();
                    //        tss_mastermarketsize.Id = _datakagroupuio.tbmastermarketsize_tss_MasterMarketSizeId;
                    //        tss_mastermarketsize.tss_Status = new OptionSetValue(STATUS_ERROR_MS);
                    //        tss_mastermarketsize.tss_StatusReason = new OptionSetValue(STATUSREASON_ERROR_MS);
                    //        tss_mastermarketsize.tss_ErrorMessage = "Part Number NOT Found !";
                    //        organizationService.Update(tss_mastermarketsize);
                    //    }

                    //    #endregion GENERATE MARKET SIZE SUBLINES KA GROUP UIO COMMODITY
                    //    #endregion --------------------------------------------------------------------------------------------------------------------------------------------------------------
                    //    #endregion --------------------------------------------------------------------------------------------------------------------------------------------------------------

                    //}
                    #endregion GENERATE SUBLINES - GROUP UIO COMMODITY
                    #endregion --------------------------------------------------------------------------------------------------------------------------------------------------------------



                    #endregion GENERATE MARKET SIZE KA GROUP UIO COMMODITY
                }
                else
                    throw new InvalidWorkflowException("Spare Part Setup NOT found !");

            }
        }

        //public void GenerateMasterMSv2(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context, EntityCollection _keyaccountcollection)
        //{
        //    using (CrmServiceContext _context = new CrmServiceContext(organizationService))
        //    {
        //        //bool _method1 = false;
        //        //bool _method2 = false;
        //        //bool _method3 = false;
        //        //bool _method4 = false;
        //        //bool _method5 = false;

        //        bool _kasuccess = true;
        //        int _calculatemethod = 0;
        //        decimal _methodvalue = 0;
        //        decimal _currenthm;
        //        decimal _lasthm;
        //        DateTime _currenthmdate;
        //        DateTime _lasthmdate;
        //        DateTime _deliverydate;
        //        DateTime _warrantyenddate;
        //        DateTime _calculatemarketsizedate = DateTime.Now.ToLocalTime().Date;

        //        var _sparepartsetup = (from tbmatrixmarketsizeperiod in _context.tss_matrixmarketsizeperiodSet
        //                               where tbmatrixmarketsizeperiod.tss_IsActive == true
        //                               select tbmatrixmarketsizeperiod).ToList().FirstOrDefault();

        //        if (_sparepartsetup != null)
        //        {
        //            #region GENERATE KA UIO
        //            var _alldatakauio = (from tbkauio in _context.tss_kauioSet
        //                                 join tbkeyaccount in _context.tss_keyaccountSet on tbkauio.tss_KeyAccountId.Id equals tbkeyaccount.tss_keyaccountId
        //                                 join tbpopulation in _context.new_populationSet on tbkauio.tss_SerialNumber.Id equals tbpopulation.new_populationId
        //                                 join tbproduct in _context.ProductSet on tbpopulation.trs_ProductMaster.Id equals tbproduct.ProductId
        //                                 join tbunitgroupmarketsize in _context.tss_unitgroupmarketsizeSet on tbpopulation.trs_ProductMaster equals tbunitgroupmarketsize.tss_Model
        //                                 //join tbunitgroupmarketsize in _context.tss_unitgroupmarketsizeSet on new { product = tbpopulation.trs_ProductMaster, unitgroup = tbproduct.DefaultUoMScheduleId } equals new { product = tbunitgroupmarketsize.tss_Model, unitgroup = tbunitgroupmarketsize.tss_UnitGroup }
        //                                 where (tbkeyaccount.tss_Status.Equals(STATUS_OPEN) || tbkeyaccount.tss_Status.Equals(STATUS_ERROR))
        //                                     && tbkeyaccount.tss_PSS.Id == context.UserId
        //                                     && tbkeyaccount.tss_Customer != null
        //                                     && tbkeyaccount.tss_ActiveEndDate >= DateTime.Now
        //                                     && tbkeyaccount.tss_ActiveStartDate <= DateTime.Now
        //                                     && tbkeyaccount.tss_CalculatetoMS == true
        //                                     && tbkeyaccount.StatusCode.Value == 1
        //                                 //&& _keyaccountcollection
        //                                 //&& 
        //                                 //&& _keyaccountids.Contains(new Guid(tbkeyaccount.tss_keyaccountId.GetValueOrDefault().ToString()))
        //                                 select new
        //                                 {
        //                                     tbkeyaccount_LogicalName = tbkeyaccount.LogicalName,
        //                                     tbkeyaccount_tss_keyaccountId = tbkeyaccount.tss_keyaccountId,
        //                                     tbkeyaccount_tss_MSPeriodStart = tbkeyaccount.tss_MSPeriodStart,
        //                                     tbkeyaccount_tss_MSPeriodEnd = tbkeyaccount.tss_MSPeriodEnd,
        //                                     tbkeyaccount_tss_ActiveStartDate = tbkeyaccount.tss_ActiveStartDate,
        //                                     tbkeyaccount_tss_ActiveEndDate = tbkeyaccount.tss_ActiveEndDate,
        //                                     tbkeyaccount_tss_Revision = tbkeyaccount.tss_Revision,
        //                                     tbkauio_LogicalName = tbkauio.LogicalName,
        //                                     tbkauio_tss_kauioId = tbkauio.tss_kauioId,
        //                                     tbkauio_tss_PSS = tbkauio.tss_PSS,
        //                                     tbkauio_tss_Customer = tbkauio.tss_Customer,
        //                                     tbpopulation_LogicalName = tbpopulation.LogicalName,
        //                                     tbpopulation_new_SerialNumber = tbpopulation.new_SerialNumber,
        //                                     tbpopulation_tss_MSCurrentHourMeter = tbpopulation.tss_MSCurrentHourMeter,
        //                                     tbpopulation_tss_MSLastHourMeter = tbpopulation.tss_MSLastHourMeter,
        //                                     tbpopulation_tss_MSCurrentHourMeterDate = tbpopulation.tss_MSCurrentHourMeterDate,
        //                                     tbpopulation_tss_MSLastHourMeterDate = tbpopulation.tss_MSLastHourMeterDate,
        //                                     tbpopulation_new_DeliveryDate = tbpopulation.new_DeliveryDate,
        //                                     tbpopulation_trs_WarrantyStartdate = tbpopulation.trs_WarrantyStartdate,
        //                                     tbpopulation_trs_WarrantyEndDate = tbpopulation.trs_WarrantyEndDate,
        //                                     tbpopulation_tss_EstWorkingHour = tbpopulation.tss_EstWorkingHour,
        //                                     tbpopulation_tss_PopulationStatus = tbpopulation.tss_PopulationStatus,
        //                                     tbpopulation_new_populationId = tbpopulation.new_populationId,
        //                                     tbpopulation_trs_ProductMaster = tbpopulation.trs_ProductMaster,
        //                                     tbproduct_LogicalName = tbproduct.LogicalName,
        //                                     tbproduct_ProductId = tbproduct.ProductId,
        //                                     tbproduct_tss_UseTyre = tbproduct.tss_UseTyre,
        //                                     tbunitgroupmarketsize_LogicalName = tbunitgroupmarketsize.LogicalName,
        //                                     tbunitgroupmarketsize_tss_MaintenanceInterval = tbunitgroupmarketsize.tss_MaintenanceInterval
        //                                 }).ToList();

        //            foreach (var _datakauio in _alldatakauio)
        //            {
        //                #region GENERATE MARKET SIZE KA UIO

        //                tss_MasterMarketSize _mastermarketsize = new tss_MasterMarketSize();

        //                _currenthm = 0;
        //                _lasthm = 0;
        //                //_currenthmdate = DateTime.MinValue;
        //                //_lasthmdate = DateTime.MinValue;
        //                //_warrantyenddate = DateTime.MinValue;

        //                //CHECK METHOD 5
        //                if ((_datakauio.tbpopulation_new_DeliveryDate.HasValue)
        //                    && _datakauio.tbkeyaccount_tss_MSPeriodStart.HasValue
        //                    && _datakauio.tbkeyaccount_tss_MSPeriodEnd.HasValue)
        //                {
        //                    //_method5 = true;
        //                    _calculatemethod = MTD5;
        //                    _deliverydate = _datakauio.tbpopulation_new_DeliveryDate.GetValueOrDefault().ToLocalTime();

        //                    if (!_datakauio.tbpopulation_trs_WarrantyEndDate.HasValue)
        //                        _warrantyenddate = _deliverydate.AddYears(1);
        //                    else
        //                        _warrantyenddate = _datakauio.tbpopulation_trs_WarrantyEndDate.GetValueOrDefault().ToLocalTime();

        //                    DateTime _msperiodstart = _datakauio.tbkeyaccount_tss_MSPeriodStart.GetValueOrDefault().ToLocalTime();
        //                    DateTime _msperiodend = _datakauio.tbkeyaccount_tss_MSPeriodEnd.GetValueOrDefault().ToLocalTime();

        //                    CalculateDate _calculate = new CalculateDate();
        //                    decimal _diffwarrantydate = _calculate.DiffYear(_deliverydate, _warrantyenddate);
        //                    decimal _diffperioddate = _calculate.DiffYear(_msperiodstart, _msperiodend);
        //                    decimal _periodpmmethod5 = (decimal)(_diffwarrantydate + _diffperioddate);

        //                    if (_periodpmmethod5 > 0)
        //                    {
        //                        _mastermarketsize.tss_periodpmmethod5 = _periodpmmethod5;
        //                        _methodvalue = _periodpmmethod5;
        //                    }
        //                }

        //                //CHECK METHOD 4
        //                if (_datakauio.tbpopulation_new_DeliveryDate.HasValue
        //                    && _datakauio.tbkeyaccount_tss_MSPeriodStart.HasValue
        //                    && _datakauio.tbkeyaccount_tss_MSPeriodEnd.HasValue)
        //                {
        //                    _calculatemethod = MTD4;

        //                    _deliverydate = _datakauio.tbpopulation_new_DeliveryDate.GetValueOrDefault().ToLocalTime();
        //                    DateTime _msperiodstart = _datakauio.tbkeyaccount_tss_MSPeriodStart.GetValueOrDefault().ToLocalTime();
        //                    DateTime _msperiodend = _datakauio.tbkeyaccount_tss_MSPeriodEnd.GetValueOrDefault().ToLocalTime();

        //                    CalculateDate _calculate = new CalculateDate();
        //                    decimal _diffdeliverydate = _calculate.DiffYear(_deliverydate, _calculatemarketsizedate);
        //                    decimal _diffperioddate = _calculate.DiffYear(_msperiodstart, _msperiodend);
        //                    decimal _periodpmmethod4 = (decimal)(_diffdeliverydate + _diffperioddate);

        //                    if (_periodpmmethod4 > 0)
        //                    {
        //                        _mastermarketsize.tss_PeriodPMMethod4 = _periodpmmethod4;
        //                        _methodvalue = _periodpmmethod4;
        //                    }
        //                }

        //                //CHECK METHOD 3
        //                if (_datakauio.tbpopulation_tss_EstWorkingHour.HasValue)
        //                {
        //                    _calculatemethod = MTD3;

        //                    decimal _estworkinghour = _datakauio.tbpopulation_tss_EstWorkingHour.GetValueOrDefault();

        //                    if (_estworkinghour > 0)
        //                    {
        //                        decimal _estworkinghourvalue = _estworkinghour > (decimal)24 ? (decimal)24 : _estworkinghour;

        //                        _mastermarketsize.tss_AvgHMMethod3 = _estworkinghourvalue;
        //                        _methodvalue = _estworkinghourvalue;
        //                    }
        //                }

        //                //CHECK METHOD 2
        //                if (_datakauio.tbpopulation_tss_MSCurrentHourMeter.HasValue
        //                    && _datakauio.tbpopulation_tss_MSCurrentHourMeterDate.HasValue
        //                    && (_datakauio.tbpopulation_new_DeliveryDate.HasValue || (_datakauio.tbpopulation_trs_WarrantyStartdate.HasValue && _datakauio.tbpopulation_trs_WarrantyEndDate.HasValue)))
        //                {
        //                    _calculatemethod = MTD2;

        //                    _currenthm = _datakauio.tbpopulation_tss_MSCurrentHourMeter.GetValueOrDefault();
        //                    _currenthmdate = _datakauio.tbpopulation_tss_MSCurrentHourMeterDate.GetValueOrDefault().ToLocalTime();

        //                    if (!_datakauio.tbpopulation_trs_WarrantyEndDate.HasValue && _datakauio.tbpopulation_new_DeliveryDate.HasValue)
        //                    {
        //                        _deliverydate = _datakauio.tbpopulation_new_DeliveryDate.GetValueOrDefault().ToLocalTime();
        //                        _warrantyenddate = _deliverydate.AddYears(1);
        //                    }
        //                    else
        //                        _warrantyenddate = _datakauio.tbpopulation_trs_WarrantyEndDate.GetValueOrDefault().ToLocalTime();

        //                    if ((decimal)(_currenthmdate - _warrantyenddate).TotalDays > 0)
        //                    {
        //                        decimal _avghmmethod2 = _currenthm / ((decimal)(_currenthmdate - _warrantyenddate).TotalDays);

        //                        if (_avghmmethod2 > 0)
        //                        {
        //                            decimal _avghmmethod2value = _avghmmethod2 > (decimal)24 ? (decimal)24 : _avghmmethod2;

        //                            _mastermarketsize.tss_AvgHMMethod2 = _avghmmethod2value;
        //                            _methodvalue = _avghmmethod2value;
        //                        }
        //                    }
        //                }

        //                //CHECK METHOD 1
        //                if (_datakauio.tbpopulation_tss_MSCurrentHourMeter.HasValue
        //                    && _datakauio.tbpopulation_tss_MSLastHourMeter.HasValue
        //                    && _datakauio.tbpopulation_tss_MSCurrentHourMeterDate.HasValue
        //                    && _datakauio.tbpopulation_tss_MSLastHourMeterDate.HasValue)
        //                {
        //                    _calculatemethod = MTD1;

        //                    _currenthm = _datakauio.tbpopulation_tss_MSCurrentHourMeter.GetValueOrDefault();
        //                    _lasthm = _datakauio.tbpopulation_tss_MSLastHourMeter.GetValueOrDefault();
        //                    _currenthmdate = _datakauio.tbpopulation_tss_MSCurrentHourMeterDate.GetValueOrDefault().ToLocalTime();
        //                    _lasthmdate = _datakauio.tbpopulation_tss_MSLastHourMeterDate.GetValueOrDefault().ToLocalTime();

        //                    if ((decimal)(_currenthmdate - _lasthmdate).TotalDays > 0)
        //                    {
        //                        decimal _avghmmethod1 = (_currenthm - _lasthm) / ((decimal)(_currenthmdate - _lasthmdate).TotalDays);

        //                        if (_avghmmethod1 > 0)
        //                        {
        //                            decimal _avghmmethod1value = _avghmmethod1 > (decimal)24 ? (decimal)24 : _avghmmethod1;

        //                            _mastermarketsize.tss_AvgHMMethod1 = _avghmmethod1 > (decimal)24 ? (decimal)24 : _avghmmethod1;
        //                            _methodvalue = _avghmmethod1value;
        //                        }
        //                    }
        //                }

        //                if (_calculatemethod == MTD1 || _calculatemethod == MTD2 || _calculatemethod == MTD3 || _calculatemethod == MTD4 || _calculatemethod == MTD5)
        //                {
        //                    //GENERATE MARKET SIZE UIO / NON UIO

        //                    _mastermarketsize.tss_UnitType = new OptionSetValue(_datakauio.tbpopulation_tss_PopulationStatus.GetValueOrDefault() ? UNITTYPE_UIO : UNITTYPE_NONUIO);
        //                    _mastermarketsize.tss_MSPeriodStart = _datakauio.tbkeyaccount_tss_MSPeriodStart.GetValueOrDefault().ToLocalTime();
        //                    _mastermarketsize.tss_MSPeriodEnd = _datakauio.tbkeyaccount_tss_MSPeriodEnd.GetValueOrDefault().ToLocalTime();
        //                    _mastermarketsize.tss_ActivePeriodStart = _datakauio.tbkeyaccount_tss_ActiveStartDate.GetValueOrDefault().ToLocalTime();
        //                    _mastermarketsize.tss_ActivePeriodSEnd = _datakauio.tbkeyaccount_tss_ActiveEndDate.GetValueOrDefault();
        //                    _mastermarketsize.tss_msperiod = _datakauio.tbkeyaccount_tss_Revision.GetValueOrDefault();

        //                    _mastermarketsize.tss_PSS = new EntityReference(_datakauio.tbkauio_LogicalName, _datakauio.tbkauio_tss_PSS.Id);
        //                    _mastermarketsize.tss_Customer = new EntityReference(_datakauio.tbkauio_LogicalName, _datakauio.tbkauio_tss_Customer.Id);
        //                    _mastermarketsize.tss_SerialNumber = new EntityReference(_datakauio.tbpopulation_LogicalName, _datakauio.tbpopulation_new_populationId.GetValueOrDefault());
        //                    _mastermarketsize.tss_Status = new OptionSetValue(STATUS_COMPLETED_MS);
        //                    _mastermarketsize.tss_StatusReason = new OptionSetValue(STATUSREASON_COMPLETED_MS);
        //                    _mastermarketsize.tss_UseTyre = _datakauio.tbproduct_tss_UseTyre.HasValue ? _datakauio.tbproduct_tss_UseTyre.GetValueOrDefault() : false;
        //                    _mastermarketsize.tss_keyaccountid = new EntityReference(_datakauio.tbkeyaccount_LogicalName, _datakauio.tbkeyaccount_tss_keyaccountId.GetValueOrDefault());
        //                    _mastermarketsize.tss_ismsresultpssgenerated = false;
        //                    _mastermarketsize.tss_issublinesgenerated = false;

        //                    Guid _mastermarketsizeid = organizationService.Create(_mastermarketsize);


        //                    #region --------------------------------------------------------------------------------------------------------------------------------------------------------------
        //                    #region --------------------------------------------------------------------------------------------------------------------------------------------------------------
        //                    #region GENERATE MARKET SIZE LINES KA UIO

        //                    string _errordescription = "";
        //                    int _rowcount = 0;
        //                    int _pmperiod = 0;
        //                    int _interval = _datakauio.tbunitgroupmarketsize_tss_MaintenanceInterval.GetValueOrDefault();
        //                    int _duedatems = _sparepartsetup.tss_DueDateMS.GetValueOrDefault();
        //                    int _mscurrenthourmeter = (int)Math.Round(_datakauio.tbpopulation_tss_MSCurrentHourMeter.GetValueOrDefault(), 0);
        //                    int _hmconsumppm;
        //                    int _forecastpmdate;
        //                    DateTime _mscurrenthourmeterdate = _datakauio.tbpopulation_tss_MSCurrentHourMeterDate.HasValue ? _datakauio.tbpopulation_tss_MSCurrentHourMeterDate.GetValueOrDefault().ToLocalTime() : DateTime.MinValue;
        //                    DateTime _nextpmdate;

        //                    if (_calculatemethod == MTD1 || _calculatemethod == MTD2 || _calculatemethod == MTD3)
        //                    {
        //                        if (_mscurrenthourmeterdate != DateTime.MinValue && _interval > 0 && _methodvalue > 0)
        //                        {
        //                            _nextpmdate = _mscurrenthourmeterdate.AddDays((int)Math.Round((((Math.Round((decimal)_mscurrenthourmeter / _interval, 0) * _interval) + _interval) - _mscurrenthourmeter) / _methodvalue, 0));

        //                            while (_nextpmdate <= _sparepartsetup.tss_EndDateMarketSize.GetValueOrDefault().ToLocalTime())
        //                            {
        //                                tss_mastermarketsizelines _mastermarketsizelines = new tss_mastermarketsizelines();

        //                                if (_mscurrenthourmeterdate >= _sparepartsetup.tss_StartDateMarketSize.GetValueOrDefault().ToLocalTime())
        //                                    _pmperiod += 1;

        //                                _mastermarketsizelines.tss_MasterMarketSizeRef = new EntityReference(_mastermarketsize.LogicalName, _mastermarketsizeid);
        //                                _mastermarketsizelines.tss_HMPM = (int)(Math.Round((decimal)_mscurrenthourmeter / _interval, 0) * _interval) + _interval;
        //                                _mastermarketsizelines.tss_HMConsumpPM = _mastermarketsizelines.tss_HMPM.GetValueOrDefault() - _mscurrenthourmeter;
        //                                _mastermarketsizelines.tss_ForecastPMDate = (int)Math.Round((decimal)_mastermarketsizelines.tss_HMConsumpPM.GetValueOrDefault() / _methodvalue, 0);
        //                                _mastermarketsizelines.tss_EstimatedPMDate = _mscurrenthourmeterdate;
        //                                _mastermarketsizelines.tss_PMPeriod = _pmperiod;
        //                                _mastermarketsizelines.tss_MethodCalculationUsed = new OptionSetValue(_calculatemethod);
        //                                _mastermarketsizelines.tss_Status = new OptionSetValue(STATUS_LINES_DRAFT);
        //                                _mastermarketsizelines.tss_StatusReason = new OptionSetValue(STATUS_REASON_LINES_OPEN);

        //                                _mastermarketsizelines.tss_PM = _DL_tss_mastermarketsizelines.GetPMType(organizationService, _datakauio.tbproduct_ProductId.GetValueOrDefault(), _mastermarketsizelines.tss_HMPM.GetValueOrDefault()).ToString();

        //                                //DUE DATE = NEXT PM DATE - DUE DATE MARKET SIZE
        //                                if (_pmperiod > 0 && _nextpmdate <= _sparepartsetup.tss_EndDateMarketSize.GetValueOrDefault().ToLocalTime())
        //                                    _mastermarketsizelines.tss_DueDate = _nextpmdate.AddDays(-_duedatems);

        //                                _mscurrenthourmeter = _mastermarketsizelines.tss_HMPM.GetValueOrDefault();
        //                                _hmconsumppm = _mastermarketsizelines.tss_HMConsumpPM.GetValueOrDefault();
        //                                _forecastpmdate = _mastermarketsizelines.tss_ForecastPMDate.GetValueOrDefault();
        //                                _mscurrenthourmeterdate = _nextpmdate;
        //                                _nextpmdate = _mscurrenthourmeterdate.AddDays(_mastermarketsizelines.tss_ForecastPMDate.GetValueOrDefault());

        //                                Guid _mastermarketsizelinesid = organizationService.Create(_mastermarketsizelines);

        //                                _rowcount += 1;
        //                            }

        //                            //while (_mscurrenthourmeterdate <= _sparepartsetup.tss_EndDateMarketSize.GetValueOrDefault().ToLocalTime() && _nextpmdate <= _sparepartsetup.tss_EndDateMarketSize.GetValueOrDefault().ToLocalTime())
        //                            //{
        //                            //    tss_mastermarketsizelines _mastermarketsizelines = new tss_mastermarketsizelines();

        //                            //    if (_mscurrenthourmeterdate >= _sparepartsetup.tss_StartDateMarketSize.GetValueOrDefault().ToLocalTime())
        //                            //        _pmperiod += 1;

        //                            //    _mastermarketsizelines.tss_MasterMarketSizeRef = new EntityReference(_mastermarketsize.LogicalName, _mastermarketsizeid);
        //                            //    _mastermarketsizelines.tss_HMPM = (int)(Math.Round((decimal)_mscurrenthourmeter / _interval, 0) * _interval) + _interval;
        //                            //    _mastermarketsizelines.tss_HMConsumpPM = _mastermarketsizelines.tss_HMPM.GetValueOrDefault() - _mscurrenthourmeter;
        //                            //    _mastermarketsizelines.tss_ForecastPMDate = (int)Math.Round((decimal)_mastermarketsizelines.tss_HMConsumpPM.GetValueOrDefault() / _methodvalue, 0);
        //                            //    _mastermarketsizelines.tss_EstimatedPMDate = _mscurrenthourmeterdate.AddDays(_mastermarketsizelines.tss_ForecastPMDate.GetValueOrDefault());
        //                            //    _mastermarketsizelines.tss_PMPeriod = _pmperiod;
        //                            //    _mastermarketsizelines.tss_MethodCalculationUsed = new OptionSetValue(_calculatemethod);
        //                            //    _mastermarketsizelines.tss_Status = new OptionSetValue(STATUS_LINES_DRAFT);
        //                            //    _mastermarketsizelines.tss_StatusReason = new OptionSetValue(STATUS_REASON_LINES_OPEN);

        //                            //    _mastermarketsizelines.tss_PM = _DL_tss_mastermarketsizelines.GetPMType(organizationService, _datakauio.tbproduct_ProductId.GetValueOrDefault(), _mastermarketsizelines.tss_HMPM.GetValueOrDefault()).ToString();

        //                            //    _mscurrenthourmeter = _mastermarketsizelines.tss_HMPM.GetValueOrDefault();
        //                            //    _hmconsumppm = _mastermarketsizelines.tss_HMConsumpPM.GetValueOrDefault();
        //                            //    _forecastpmdate = _mastermarketsizelines.tss_ForecastPMDate.GetValueOrDefault();
        //                            //    _mscurrenthourmeterdate = _mastermarketsizelines.tss_EstimatedPMDate.GetValueOrDefault().ToLocalTime();
        //                            //    _nextpmdate = _mscurrenthourmeterdate.AddDays((int)Math.Round((((Math.Round((decimal)_mscurrenthourmeter / _interval, 0) * _interval) + _interval) - _mscurrenthourmeter) / _methodvalue, 0));

        //                            //    //DUE DATE = NEXT PM DATE - DUE DATE MARKET SIZE
        //                            //    if (_pmperiod > 0 && _nextpmdate <= _sparepartsetup.tss_EndDateMarketSize.GetValueOrDefault().ToLocalTime())
        //                            //        _mastermarketsizelines.tss_DueDate = _nextpmdate.AddDays(-_duedatems);

        //                            //    Guid _mastermarketsizelinesid = organizationService.Create(_mastermarketsizelines);

        //                            //    #region --------------------------------------------------------------------------------------------------------------------------------------------------------------
        //                            //    #region --------------------------------------------------------------------------------------------------------------------------------------------------------------
        //                            //    #region GENERATE MARKET SIZE SUBLINES KA UIO - MTD1 / MTD2 / MTD3

        //                            //    //var _alldatamarketsizepartconsump = (from tbmarketsizepartconsump in _context.tss_MarketSizePartConsumpSet
        //                            //    //                                     join tbmasterpart in _context.trs_masterpartSet on tbmarketsizepartconsump.tss_PartNumber.Id equals tbmasterpart.trs_masterpartId
        //                            //    //                                     join tbpartmasterlinesmodel in _context.tss_partmasterlinesmodelSet on tbmarketsizepartconsump.tss_PartNumber.Id equals tbpartmasterlinesmodel.tss_PartMasterId.Id
        //                            //    //                                     where tbmarketsizepartconsump.tss_Model.Id == _datakauio.tbpopulation_trs_ProductMaster.Id
        //                            //    //                                        && tbmarketsizepartconsump.tss_TypePM == _mastermarketsizelines.tss_PM
        //                            //    //                                     select new
        //                            //    //                                     {
        //                            //    //                                         tbmarketsizepartconsump_LogicalName = tbmarketsizepartconsump.LogicalName,
        //                            //    //                                         tbmarketsizepartconsump_tss_PartNumber = tbmarketsizepartconsump.tss_PartNumber,
        //                            //    //                                         tbmarketsizepartconsump_tss_Qty = tbmarketsizepartconsump.tss_Qty,
        //                            //    //                                         tbmasterpart_LogicalName = tbmasterpart.LogicalName,
        //                            //    //                                         tbmasterpart_tss_commoditytype = tbmasterpart.tss_commoditytype,
        //                            //    //                                         tbpartmasterlinesmodel_LogicalName = tbpartmasterlinesmodel.LogicalName
        //                            //    //                                     }).ToList();

        //                            //    //foreach (var _datamarketsizepartconsump in _alldatamarketsizepartconsump)
        //                            //    //{
        //                            //    //    int _quantity = _datamarketsizepartconsump.tbmarketsizepartconsump_tss_Qty.HasValue ? _datamarketsizepartconsump.tbmarketsizepartconsump_tss_Qty.GetValueOrDefault() : 0;
        //                            //    //    decimal _price = 0;
        //                            //    //    decimal _minimumprice = 0;

        //                            //    //    #region CHECK IF BATTERY
        //                            //    //    if (_datamarketsizepartconsump.tbmasterpart_tss_commoditytype.Value == BATTERY_TYPE)
        //                            //    //    {
        //                            //    //        QueryExpression _queryexpression = new QueryExpression(_DL_tss_partmasterlinesbattery.EntityName);
        //                            //    //        _queryexpression.LinkEntities.Add(new LinkEntity(_DL_tss_partmasterlinesbattery.EntityName, _DL_tss_partmasterlinesbatterytypeconsump.EntityName, "tss_partmasterlinesbatteryid", "tss_refpartmasterlinesbattery", JoinOperator.Inner));
        //                            //    //        _queryexpression.LinkEntities[0].Columns.AddColumns("tss_nextconsump", "tss_partmasterlinesbatterytypeconsumpid");
        //                            //    //        _queryexpression.LinkEntities[0].EntityAlias = "tss_partmasterlinesbatterytypeconsump";
        //                            //    //        _queryexpression.Criteria.AddCondition("tss_partmasterid", ConditionOperator.Equal, _datamarketsizepartconsump.tbmarketsizepartconsump_tss_PartNumber.Id);
        //                            //    //        _queryexpression.ColumnSet = new ColumnSet(true);

        //                            //    //        EntityCollection _partmasterlinesbattery = _DL_tss_partmasterlinesbattery.Select(organizationService, _queryexpression);

        //                            //    //        if (_partmasterlinesbattery.Entities.Count > 0)
        //                            //    //        {
        //                            //    //            if (_sparepartsetup.tss_StartDateMarketSize.HasValue && _sparepartsetup.tss_EndDateMarketSize.HasValue && _partmasterlinesbattery[0].Contains("tss_partmasterlinesbatterytypeconsump.tss_nextconsump") &&
        //                            //    //               (DateTime)_partmasterlinesbattery[0].GetAttributeValue<AliasedValue>("tss_partmasterlinesbatterytypeconsump.tss_nextconsump").Value >= _sparepartsetup.tss_StartDateMarketSize.GetValueOrDefault().ToLocalTime() &&
        //                            //    //               (DateTime)_partmasterlinesbattery[0].GetAttributeValue<AliasedValue>("tss_partmasterlinesbatterytypeconsump.tss_nextconsump").Value <= _sparepartsetup.tss_EndDateMarketSize.GetValueOrDefault().ToLocalTime())
        //                            //    //            {
        //                            //    //                tss_partmasterlinesbatterytypeconsump _partmasterlinesbatterytypeconsump = new tss_partmasterlinesbatterytypeconsump();

        //                            //    //                _partmasterlinesbatterytypeconsump.Id = (Guid)_partmasterlinesbattery[0].GetAttributeValue<AliasedValue>("tss_partmasterlinesbatterytypeconsump.tss_partmasterlinesbatterytypeconsumpid").Value;
        //                            //    //                _partmasterlinesbatterytypeconsump.tss_LastConsump = _mastermarketsizelines.tss_EstimatedPMDate.GetValueOrDefault();
        //                            //    //                _partmasterlinesbatterytypeconsump.tss_NextConsump = _mastermarketsizelines.tss_EstimatedPMDate.GetValueOrDefault().AddDays(_partmasterlinesbattery[0].GetAttributeValue<int>("tss_cycleconsump"));

        //                            //    //                organizationService.Update(_partmasterlinesbatterytypeconsump);
        //                            //    //            }
        //                            //    //        }
        //                            //    //    }
        //                            //    //    #endregion CHECK IF BATTERY
        //                            //    //}

        //                            //    _rowcount += 1;

        //                            //    #endregion GENERATE MARKET SIZE SUBLINES KA UIO - MTD1 / MTD2 / MTD3
        //                            //    #endregion --------------------------------------------------------------------------------------------------------------------------------------------------------------
        //                            //    #endregion --------------------------------------------------------------------------------------------------------------------------------------------------------------
        //                            //}

        //                            if (_rowcount == 0)
        //                                _errordescription = "Process Generate Lines FAILED. Please Check Data Population !";
        //                        }
        //                        else
        //                            _errordescription = "Process Generate Lines FAILED. Please Check 'Population MSCurrentHourMeterDate' / 'Unit Group Maintenance Interval' !";
        //                    }
        //                    else if (_calculatemethod == MTD4 || _calculatemethod == MTD5)
        //                    {
        //                        _deliverydate = _datakauio.tbpopulation_new_DeliveryDate.GetValueOrDefault().ToLocalTime();
        //                        DateTime _today = DateTime.Now;
        //                        DateTime _msperiodstart = _mastermarketsize.tss_MSPeriodStart.GetValueOrDefault().ToLocalTime();
        //                        DateTime _msperiodend = _mastermarketsize.tss_MSPeriodEnd.GetValueOrDefault().ToLocalTime();

        //                        tss_mastermarketsizelines _mastermarketsizelines = new tss_mastermarketsizelines();

        //                        _mastermarketsizelines.tss_MasterMarketSizeRef = new EntityReference(_mastermarketsize.LogicalName, _mastermarketsizeid);
        //                        _mastermarketsizelines.tss_MethodCalculationUsed = new OptionSetValue(_calculatemethod);
        //                        _mastermarketsizelines.tss_Status = new OptionSetValue(STATUS_LINES_DRAFT);
        //                        _mastermarketsizelines.tss_StatusReason = new OptionSetValue(STATUS_REASON_LINES_OPEN);

        //                        //CALCULATE AGING
        //                        CalculateDate _calculate = new CalculateDate();

        //                        int _aging = 0;
        //                        int _agingmsperiod = _calculate.DiffYear(_msperiodstart, _msperiodend);
        //                        int _agingdeliverydate = _calculate.DiffYear(_deliverydate, _today);

        //                        _aging = _agingmsperiod + _agingdeliverydate;

        //                        _mastermarketsizelines.tss_Aging = Convert.ToInt32(_aging);

        //                        Guid _mastermarketsizelinesid = organizationService.Create(_mastermarketsizelines);
        //                    }
        //                    else
        //                        _errordescription = "Process Generate Lines FAILED. Calculate Method is NOT Found !";


        //                    #region UPDATE KEY ACCOUNT / KA UIO / MASTER MARKET SIZE
        //                    if (_errordescription == "")
        //                    {
        //                        #region UPDATE STATUS KA UIO (SUCCESS)
        //                        tss_kauio _kauio = new tss_kauio();
        //                        _kauio.Id = _datakauio.tbkauio_tss_kauioId.GetValueOrDefault();
        //                        _kauio.tss_CalculateStatus = true;
        //                        _kauio.tss_errordescription = "";
        //                        organizationService.Update(_kauio);
        //                        #endregion UPDATE STATUS KA UIO (SUCCESS)
        //                    }
        //                    else
        //                    {
        //                        _kasuccess = false;

        //                        #region UPDATE STATUS MASTER MARKET SIZE DAN KEY ACCOUNT DAN KA UIO (FAILED)
        //                        tss_kauio _kauio = new tss_kauio();
        //                        _kauio.Id = _datakauio.tbkauio_tss_kauioId.GetValueOrDefault();
        //                        _kauio.tss_CalculateStatus = false;
        //                        _kauio.tss_errordescription = _errordescription;
        //                        organizationService.Update(_kauio);

        //                        tss_MasterMarketSize tss_mastermarketsize = new tss_MasterMarketSize();
        //                        tss_mastermarketsize.Id = _mastermarketsizeid;
        //                        tss_mastermarketsize.tss_Status = new OptionSetValue(STATUS_ERROR);
        //                        tss_mastermarketsize.tss_StatusReason = new OptionSetValue(STATUSREASON_ERROR_MS);
        //                        tss_mastermarketsize.tss_ErrorMessage = _errordescription;
        //                        organizationService.Update(tss_mastermarketsize);
        //                        #endregion UPDATE STATUS MASTER MARKET SIZE DAN KEY ACCOUNT DAN KA UIO (FAILED)
        //                    }
        //                    #endregion UPDATE KEY ACCOUNT / KA UIO / MASTER MARKET SIZE

        //                    #endregion GENERATE MARKET SIZE LINES KA UIO
        //                    #endregion --------------------------------------------------------------------------------------------------------------------------------------------------------------
        //                    #endregion --------------------------------------------------------------------------------------------------------------------------------------------------------------
        //                }
        //                else
        //                {
        //                    //GENERATE MARKET SIZE UIO / NON UIO ( ERROR )

        //                    _kasuccess = false;

        //                    List<string> _errorcollection = new List<string>();
        //                    string _errormessage = "";

        //                    if (!_datakauio.tbpopulation_tss_MSCurrentHourMeter.HasValue)
        //                        _errorcollection.Add("Current Hour Meter");

        //                    if (!_datakauio.tbpopulation_tss_MSCurrentHourMeterDate.HasValue)
        //                        _errorcollection.Add("Current Hour Meter (Date)");

        //                    if (!_datakauio.tbpopulation_tss_MSLastHourMeter.HasValue)
        //                        _errorcollection.Add("Last Hour Meter");

        //                    if (!_datakauio.tbpopulation_tss_MSLastHourMeterDate.HasValue)
        //                        _errorcollection.Add("Last Hour Meter (Date)");

        //                    if (!_datakauio.tbpopulation_new_DeliveryDate.HasValue)
        //                        _errorcollection.Add("Delivery Date");

        //                    if (_errorcollection.Count() > 0)
        //                    {
        //                        foreach (var item in _errorcollection)
        //                        {
        //                            _errormessage += item + ", ";
        //                        }

        //                        _errormessage = _errormessage.Substring(0, _errormessage.Length - 1) + " still not define !";
        //                    }
        //                    else
        //                        _errormessage = "Current Hour Meter, Current Hour Meter (Date), Last Hour Meter , Last Hour Meter (Date), Delivery Date or Overhaul Date still not define";

        //                    _mastermarketsize.tss_UnitType = new OptionSetValue(_datakauio.tbpopulation_tss_PopulationStatus.GetValueOrDefault() ? UNITTYPE_UIO : UNITTYPE_NONUIO);
        //                    _mastermarketsize.tss_MSPeriodStart = _datakauio.tbkeyaccount_tss_MSPeriodStart.GetValueOrDefault().ToLocalTime();
        //                    _mastermarketsize.tss_MSPeriodEnd = _datakauio.tbkeyaccount_tss_MSPeriodEnd.GetValueOrDefault().ToLocalTime();
        //                    _mastermarketsize.tss_ActivePeriodStart = _datakauio.tbkeyaccount_tss_ActiveStartDate.GetValueOrDefault().ToLocalTime();
        //                    _mastermarketsize.tss_ActivePeriodSEnd = _datakauio.tbkeyaccount_tss_ActiveEndDate.GetValueOrDefault();
        //                    _mastermarketsize.tss_msperiod = _datakauio.tbkeyaccount_tss_Revision.GetValueOrDefault();

        //                    _mastermarketsize.tss_PSS = new EntityReference(_datakauio.tbkauio_LogicalName, _datakauio.tbkauio_tss_PSS.Id);
        //                    _mastermarketsize.tss_Customer = new EntityReference(_datakauio.tbkauio_LogicalName, _datakauio.tbkauio_tss_Customer.Id);
        //                    _mastermarketsize.tss_SerialNumber = new EntityReference(_datakauio.tbpopulation_LogicalName, _datakauio.tbpopulation_new_populationId.GetValueOrDefault());
        //                    _mastermarketsize.tss_Status = new OptionSetValue(STATUS_ERROR_MS);
        //                    _mastermarketsize.tss_StatusReason = new OptionSetValue(STATUSREASON_ERROR_MS);
        //                    _mastermarketsize.tss_ErrorMessage = _errormessage;
        //                    _mastermarketsize.tss_UseTyre = _datakauio.tbproduct_tss_UseTyre.HasValue ? _datakauio.tbproduct_tss_UseTyre.GetValueOrDefault() : false;
        //                    _mastermarketsize.tss_keyaccountid = new EntityReference(_datakauio.tbkeyaccount_LogicalName, _datakauio.tbkeyaccount_tss_keyaccountId.GetValueOrDefault());
        //                    _mastermarketsize.tss_ismsresultpssgenerated = false;
        //                    _mastermarketsize.tss_issublinesgenerated = true;

        //                    Guid _mastermarketsizeid = organizationService.Create(_mastermarketsize);
        //                }
        //                #endregion

        //                #region UPDATE KEY ACCOUNT
        //                if (_kasuccess)
        //                {
        //                    tss_keyaccount new_keyaccount = new tss_keyaccount();
        //                    new_keyaccount.Id = _datakauio.tbkeyaccount_tss_keyaccountId.GetValueOrDefault();
        //                    new_keyaccount.tss_Status = new OptionSetValue(STATUS_CALCULATE);
        //                    new_keyaccount.tss_Reason = new OptionSetValue(STATUS_REASON_CALCULATE);
        //                    organizationService.Update(new_keyaccount);
        //                }
        //                else
        //                {
        //                    tss_keyaccount new_keyaccount = new tss_keyaccount();
        //                    new_keyaccount.Id = _datakauio.tbkeyaccount_tss_keyaccountId.GetValueOrDefault();
        //                    new_keyaccount.tss_Status = new OptionSetValue(STATUS_ERROR);
        //                    new_keyaccount.tss_Reason = new OptionSetValue(STATUS_REASON_CALCULATE);
        //                    organizationService.Update(new_keyaccount);
        //                }
        //                #endregion UPDATE KEY ACCOUNT

        //            }
        //            #endregion GENERATE KA UIO

        //            // --------------------------------------------------------------------------------------------------------------------------------------------------------------
        //            // --------------------------------------------------------------------------------------------------------------------------------------------------------------

        //            #region GENERATE MARKET SIZE KA GROUP UIO COMMODITY
        //            var _alldatakagroupuioheader = (from tbkeyaccount in _context.tss_keyaccountSet
        //                                            join tbkagroupuiocommodity in _context.tss_kagroupuiocommoditySet on tbkeyaccount.tss_keyaccountId equals tbkagroupuiocommodity.tss_KeyAccountId.Id
        //                                            join tbgroupuiocommodityaccount in _context.tss_groupuiocommodityaccountSet on tbkagroupuiocommodity.tss_GroupUIOCommodity.Id equals tbgroupuiocommodityaccount.tss_groupuiocommodityaccountId
        //                                            join tbgroupuiocommodityheader in _context.tss_groupuiocommodityheaderSet on tbgroupuiocommodityaccount.tss_GroupUIOCommodityHeader.Id equals tbgroupuiocommodityheader.tss_groupuiocommodityheaderId
        //                                            where (tbkeyaccount.tss_Status.Equals(STATUS_OPEN) || tbkeyaccount.tss_Status.Equals(STATUS_ERROR))
        //                                               && tbkeyaccount.tss_PSS.Id == context.UserId
        //                                               && tbkeyaccount.tss_Customer != null
        //                                               && tbkeyaccount.tss_ActiveEndDate >= DateTime.Now
        //                                               && tbkeyaccount.tss_ActiveStartDate <= DateTime.Now
        //                                               && tbkeyaccount.tss_CalculatetoMS == true
        //                                               && tbkeyaccount.StatusCode.Value == 1
        //                                               && tbgroupuiocommodityaccount.tss_Qty > 0
        //                                            //&& _keyaccountids.Contains(new Guid(tbkeyaccount.tss_keyaccountId.GetValueOrDefault().ToString()))
        //                                            select new
        //                                            {
        //                                                tbkeyaccount_LogicalName = tbkeyaccount.LogicalName,
        //                                                tbkeyaccount_tss_keyaccountId = tbkeyaccount.tss_keyaccountId,
        //                                                tbkeyaccount_tss_MSPeriodStart = tbkeyaccount.tss_MSPeriodStart,
        //                                                tbkeyaccount_tss_MSPeriodEnd = tbkeyaccount.tss_MSPeriodEnd,
        //                                                tbkeyaccount_tss_ActiveStartDate = tbkeyaccount.tss_ActiveStartDate,
        //                                                tbkeyaccount_tss_ActiveEndDate = tbkeyaccount.tss_ActiveEndDate,
        //                                                tbkeyaccount_tss_Revision = tbkeyaccount.tss_Revision,
        //                                                tbkagroupuiocommodity_LogicalName = tbkagroupuiocommodity.LogicalName,
        //                                                tbkagroupuiocommodity_tss_kagroupuiocommodityId = tbkagroupuiocommodity.tss_kagroupuiocommodityId,
        //                                                tbkagroupuiocommodity_tss_pss = tbkagroupuiocommodity.tss_pss,
        //                                                tbkagroupuiocommodity_tss_customer = tbkagroupuiocommodity.tss_customer,
        //                                                tbgroupuiocommodityaccount_LogicalName = tbgroupuiocommodityaccount.LogicalName,
        //                                                tbgroupuiocommodityaccount_tss_Qty = tbgroupuiocommodityaccount.tss_Qty,
        //                                                //tbgroupuiocommodityaccount_tss_GroupUIOCommodityHeader = tbgroupuiocommodityaccount.tss_GroupUIOCommodityHeader,
        //                                                tbgroupuiocommodityheader_LogicalName = tbgroupuiocommodityheader.LogicalName,
        //                                                tbgroupuiocommodityheader_tss_groupuiocommodityheaderId = tbgroupuiocommodityheader.tss_groupuiocommodityheaderId
        //                                            }).ToList();

        //            var _alldatakagroupuio = (from tball in _alldatakagroupuioheader.AsEnumerable()
        //                                      join tbgroupuiocommodity in _context.tss_groupuiocommoditySet on tball.tbgroupuiocommodityheader_tss_groupuiocommodityheaderId equals tbgroupuiocommodity.tss_GroupUIOCommodityHeaderId.Id
        //                                      select new
        //                                      {
        //                                          tbgroupuiocommodity_LogicalName = tbgroupuiocommodity.LogicalName,
        //                                          tbgroupuiocommodity_tss_PartCommodityType = tbgroupuiocommodity.tss_PartCommodityType,
        //                                          tbgroupuiocommodity_tss_BatteryBy = tbgroupuiocommodity.tss_BatteryBy,
        //                                          tbgroupuiocommodity_tss_BatteryPartNumber = tbgroupuiocommodity.tss_BatteryPartNumber,
        //                                          tbgroupuiocommodity_tss_BatteryType = tbgroupuiocommodity.tss_BatteryType,
        //                                          tbgroupuiocommodity_tss_BatteryTraction = tbgroupuiocommodity.tss_BatteryTraction,
        //                                          tbgroupuiocommodity_tss_BatteryCranking = tbgroupuiocommodity.tss_BatteryCranking,
        //                                          tbgroupuiocommodity_tss_Battery = tbgroupuiocommodity.tss_Battery,
        //                                          tbgroupuiocommodity_tss_Oilby = tbgroupuiocommodity.tss_Oilby,
        //                                          tbgroupuiocommodity_tss_OilPartNumber = tbgroupuiocommodity.tss_OilPartNumber,
        //                                          tbgroupuiocommodity_tss_JenisOil = tbgroupuiocommodity.tss_JenisOil,
        //                                          tbgroupuiocommodity_tss_OilQtyby = tbgroupuiocommodity.tss_OilQtyby,
        //                                          tbgroupuiocommodity_tss_OilQtyPcs = tbgroupuiocommodity.tss_OilQtyPcs,
        //                                          tbgroupuiocommodity_tss_Oil = tbgroupuiocommodity.tss_Oil,
        //                                          tbgroupuiocommodity_tss_Tyreby = tbgroupuiocommodity.tss_Tyreby,
        //                                          tbgroupuiocommodity_tss_TyrePartNumber = tbgroupuiocommodity.tss_TyrePartNumber,
        //                                          tbgroupuiocommodity_tss_TyreSpec = tbgroupuiocommodity.tss_TyreSpec,
        //                                          tbgroupuiocommodity_tss_Tyre = tbgroupuiocommodity.tss_Tyre
        //                                      }).ToList();

        //            foreach (var _datakagroupuio in _alldatakagroupuio)
        //            {
        //                int _quantity = _datakagroupuio.tbgroupuiocommodityaccount_tss_Qty.HasValue ? _datakagroupuio.tbgroupuiocommodityaccount_tss_Qty.GetValueOrDefault() : 0;

        //                if (_quantity > 0)
        //                {
        //                    tss_MasterMarketSize _mastermarketsize = new tss_MasterMarketSize();

        //                    _mastermarketsize.tss_PSS = new EntityReference(_datakagroupuio.tbkagroupuiocommodity_LogicalName, _datakagroupuio.tbkagroupuiocommodity_tss_pss.Id);
        //                    _mastermarketsize.tss_Customer = new EntityReference(_datakagroupuio.tbkagroupuiocommodity_LogicalName, _datakagroupuio.tbkagroupuiocommodity_tss_customer.Id);
        //                    _mastermarketsize.tss_GroupUIOCommodityHeader = new EntityReference(_datakagroupuio.tbgroupuiocommodityheader_LogicalName, _datakagroupuio.tbgroupuiocommodityheader_tss_groupuiocommodityheaderId.GetValueOrDefault());
        //                    _mastermarketsize.tss_Qty = _quantity;
        //                    _mastermarketsize.tss_UnitType = new OptionSetValue(UNITTYPE_COMMODITY);
        //                    _mastermarketsize.tss_Status = new OptionSetValue(STATUS_COMPLETED_MS);
        //                    _mastermarketsize.tss_StatusReason = new OptionSetValue(STATUSREASON_COMPLETED_MS);
        //                    _mastermarketsize.tss_MSPeriodStart = _datakagroupuio.tbkeyaccount_tss_MSPeriodStart.GetValueOrDefault().ToLocalTime();
        //                    _mastermarketsize.tss_MSPeriodEnd = _datakagroupuio.tbkeyaccount_tss_MSPeriodEnd.GetValueOrDefault().ToLocalTime();
        //                    _mastermarketsize.tss_ActivePeriodStart = _datakagroupuio.tbkeyaccount_tss_ActiveStartDate.GetValueOrDefault().ToLocalTime();
        //                    _mastermarketsize.tss_ActivePeriodSEnd = _datakagroupuio.tbkeyaccount_tss_ActiveEndDate.GetValueOrDefault();
        //                    _mastermarketsize.tss_keyaccountid = new EntityReference(_datakagroupuio.tbkeyaccount_LogicalName, _datakagroupuio.tbkeyaccount_tss_keyaccountId.GetValueOrDefault());
        //                    _mastermarketsize.tss_msperiod = _datakagroupuio.tbkeyaccount_tss_Revision.GetValueOrDefault();
        //                    _mastermarketsize.tss_ismsresultpssgenerated = false;
        //                    _mastermarketsize.tss_issublinesgenerated = false;

        //                    Guid _mastermarketsizeid = organizationService.Create(_mastermarketsize);


        //                    #region --------------------------------------------------------------------------------------------------------------------------------------------------------------
        //                    #region --------------------------------------------------------------------------------------------------------------------------------------------------------------
        //                    #region GENERATE MARKET SIZE SUBLINES KA GROUP UIO COMMODITY

        //                    Guid _partnumberid = new Guid();
        //                    _quantity = 0;

        //                    tss_mastermarketsizesublines _mastermarketsizesublines = new tss_mastermarketsizesublines();
        //                    _mastermarketsizesublines.tss_MasterMarketSizeId = new EntityReference(_mastermarketsize.LogicalName, _mastermarketsizeid);

        //                    //OPTION SET - BATTERY / OIL / TYRE
        //                    if (_datakagroupuio.tbgroupuiocommodity_tss_PartCommodityType.Value == PART_COMMODITYTYPE_BATTERY)
        //                    {
        //                        Guid _partnumberbattery = new Guid();

        //                        if (_datakagroupuio.tbgroupuiocommodity_tss_BatteryBy.Value == PART_COMMODITYTYPE_BY_PART)
        //                        {
        //                            _partnumberbattery = _datakagroupuio.tbgroupuiocommodity_tss_BatteryPartNumber.Id;

        //                            if (_partnumberbattery != null)
        //                            {
        //                                _partnumberid = _partnumberbattery;
        //                                _mastermarketsizesublines.tss_PartNumber = new EntityReference("trs_masterpart", _partnumberbattery);
        //                            }
        //                        }
        //                        else if (_datakagroupuio.tbgroupuiocommodity_tss_BatteryBy.Value == PART_COMMODITYTYPE_BY_SPEC)
        //                        {
        //                            //BATTERY TYPE - TRACTION / CRANKING
        //                            if (_datakagroupuio.tbgroupuiocommodity_tss_BatteryType.Value == 865920000)
        //                            {
        //                                QueryExpression _queryexpression = new QueryExpression("tss_partmasterlinesbattery");
        //                                _queryexpression.ColumnSet = new ColumnSet(true);
        //                                _queryexpression.Criteria = new FilterExpression
        //                                {
        //                                    FilterOperator = LogicalOperator.And,
        //                                    Conditions =
        //                                    {
        //                                        new ConditionExpression("tss_batterymodeltraction", ConditionOperator.Equal, _datakagroupuio.tbgroupuiocommodity_tss_BatteryTraction.Id)
        //                                    }
        //                                };

        //                                EntityCollection _batterytraction = organizationService.RetrieveMultiple(_queryexpression);

        //                                if (_batterytraction.Entities.Count() > 0)
        //                                {
        //                                    _partnumberid = _batterytraction.Entities[0].GetAttributeValue<EntityReference>("tss_partmasterid").Id;
        //                                    _partnumberbattery = _partnumberid;
        //                                    _mastermarketsizesublines.tss_PartNumber = new EntityReference("trs_masterpart", _partnumberbattery);
        //                                }
        //                            }
        //                            else if (_datakagroupuio.tbgroupuiocommodity_tss_BatteryBy.Value == 865920001)
        //                            {
        //                                QueryExpression _queryexpression = new QueryExpression("tss_partmasterlinesbattery");
        //                                _queryexpression.ColumnSet = new ColumnSet(true);
        //                                _queryexpression.Criteria = new FilterExpression
        //                                {
        //                                    FilterOperator = LogicalOperator.And,
        //                                    Conditions =
        //                                    {
        //                                        new ConditionExpression("tss_batterymodelcranking", ConditionOperator.Equal, _datakagroupuio.tbgroupuiocommodity_tss_BatteryCranking.Id)
        //                                    }
        //                                };
        //                                EntityCollection _batterycranking = organizationService.RetrieveMultiple(_queryexpression);

        //                                if (_batterycranking.Entities.Count() > 0)
        //                                {
        //                                    _partnumberid = _batterycranking.Entities[0].GetAttributeValue<EntityReference>("tss_partmasterid").Id;
        //                                    _partnumberbattery = _partnumberid;
        //                                    _mastermarketsizesublines.tss_PartNumber = new EntityReference("trs_masterpart", _partnumberbattery);
        //                                }
        //                            }
        //                        }

        //                        if (_partnumberbattery != new Guid())
        //                        {
        //                            DL_tss_mastermarketsizesublines _partmaster = GetPartMaster(organizationService, _partnumberbattery);

        //                            _mastermarketsizesublines.tss_Price = new Money(_partmaster.tss_price);
        //                            _mastermarketsizesublines.tss_MinimumPrice = new Money(_partmaster.tss_minimumprice);
        //                            _mastermarketsizesublines.tss_partdescription = _partmaster.tss_partdescription;
        //                        }

        //                        _quantity = _datakagroupuio.tbgroupuiocommodity_tss_Battery.HasValue ? _datakagroupuio.tbgroupuiocommodity_tss_Battery.GetValueOrDefault() : 0;
        //                    }
        //                    else if (_datakagroupuio.tbgroupuiocommodity_tss_PartCommodityType.Value == PART_COMMODITYTYPE_OIL)
        //                    {
        //                        Guid _partnumberoil = new Guid();

        //                        if (_datakagroupuio.tbgroupuiocommodity_tss_Oilby.Value == PART_COMMODITYTYPE_BY_PART)
        //                        {
        //                            _partnumberoil = _datakagroupuio.tbgroupuiocommodity_tss_OilPartNumber.Id;

        //                            if (_partnumberoil != null)
        //                            {
        //                                _partnumberid = _partnumberoil;
        //                                _mastermarketsizesublines.tss_PartNumber = new EntityReference("trs_masterpart", _partnumberoil);
        //                            }
        //                        }
        //                        else if (_datakagroupuio.tbgroupuiocommodity_tss_Oilby.Value == PART_COMMODITYTYPE_BY_SPEC)
        //                        {
        //                            QueryExpression _queryexpression = new QueryExpression("tss_partmasterlinesoil");
        //                            _queryexpression.ColumnSet = new ColumnSet(true);
        //                            _queryexpression.Criteria = new FilterExpression
        //                            {
        //                                FilterOperator = LogicalOperator.And,
        //                                Conditions =
        //                                {
        //                                    new ConditionExpression("tss_oiltype", ConditionOperator.Equal, _datakagroupuio.tbgroupuiocommodity_tss_JenisOil.Id)
        //                                }
        //                            };

        //                            EntityCollection _partmasterlinesoil = organizationService.RetrieveMultiple(_queryexpression);

        //                            if (_partmasterlinesoil.Entities.Count() > 0)
        //                            {
        //                                _partnumberid = _partmasterlinesoil.Entities[0].GetAttributeValue<EntityReference>("tss_partmasterid").Id;
        //                                _partnumberoil = _partnumberid;
        //                                _mastermarketsizesublines.tss_PartNumber = new EntityReference("trs_masterpart", _partnumberoil);
        //                            }
        //                        }

        //                        if (_partnumberoil != new Guid())
        //                        {
        //                            DL_tss_mastermarketsizesublines _partmaster = GetPartMaster(organizationService, _partnumberoil);

        //                            _mastermarketsizesublines.tss_Price = new Money(_partmaster.tss_price);
        //                            _mastermarketsizesublines.tss_MinimumPrice = new Money(_partmaster.tss_minimumprice);
        //                            _mastermarketsizesublines.tss_partdescription = _partmaster.tss_partdescription;
        //                        }

        //                        if (_datakagroupuio.tbgroupuiocommodity_tss_OilQtyby.Value == 865920000)
        //                            _quantity = _datakagroupuio.tbgroupuiocommodity_tss_OilQtyPcs.HasValue ? _datakagroupuio.tbgroupuiocommodity_tss_OilQtyPcs.Value : 0;
        //                        else if (_datakagroupuio.tbgroupuiocommodity_tss_OilQtyby.Value == 865920001)
        //                            _quantity = _datakagroupuio.tbgroupuiocommodity_tss_Oil.HasValue ? _datakagroupuio.tbgroupuiocommodity_tss_Oil.Value : 0;
        //                    }
        //                    else if (_datakagroupuio.tbgroupuiocommodity_tss_PartCommodityType.Value == PART_COMMODITYTYPE_TYRE)
        //                    {
        //                        Guid _partnumbertyre = new Guid();

        //                        if (_datakagroupuio.tbgroupuiocommodity_tss_Tyreby.Value == PART_COMMODITYTYPE_BY_PART)
        //                        {
        //                            _partnumbertyre = _datakagroupuio.tbgroupuiocommodity_tss_TyrePartNumber.Id;

        //                            if (_partnumbertyre != null)
        //                            {
        //                                _partnumberid = _partnumbertyre;
        //                                _mastermarketsizesublines.tss_PartNumber = new EntityReference("trs_masterpart", _partnumbertyre);
        //                            }
        //                        }
        //                        else if (_datakagroupuio.tbgroupuiocommodity_tss_Tyreby.Value == PART_COMMODITYTYPE_BY_SPEC)
        //                        {
        //                            Entity _tss_partmasterlinestyretype = organizationService.Retrieve("tss_partmasterlinestyretype", _datakagroupuio.tbgroupuiocommodity_tss_TyreSpec.Id, new ColumnSet(true));

        //                            if (_tss_partmasterlinestyretype != null)
        //                            {
        //                                _partnumberid = _tss_partmasterlinestyretype.GetAttributeValue<EntityReference>("tss_partmasterid").Id;
        //                                _partnumbertyre = _partnumberid;
        //                                _mastermarketsizesublines.tss_PartNumber = new EntityReference("trs_masterpart", _partnumbertyre);
        //                            }
        //                        }

        //                        if (_partnumbertyre != new Guid())
        //                        {
        //                            DL_tss_mastermarketsizesublines _partmaster = GetPartMaster(organizationService, _partnumbertyre);

        //                            _mastermarketsizesublines.tss_Price = new Money(_partmaster.tss_price);
        //                            _mastermarketsizesublines.tss_MinimumPrice = new Money(_partmaster.tss_minimumprice);
        //                            _mastermarketsizesublines.tss_partdescription = _partmaster.tss_partdescription;
        //                        }

        //                        _quantity = _datakagroupuio.tbgroupuiocommodity_tss_Tyre.GetValueOrDefault();
        //                    }

        //                    _mastermarketsizesublines.tss_Qty = _datakagroupuio.tbgroupuiocommodityaccount_tss_Qty.Value * _quantity;
        //                    _mastermarketsizesublines.tss_originalqty = _quantity;

        //                    if (_partnumberid != new Guid())
        //                    {
        //                        organizationService.Create(_mastermarketsizesublines);

        //                        tss_kagroupuiocommodity _kagroupuiocommodity = new tss_kagroupuiocommodity();
        //                        _kagroupuiocommodity.Id = _datakagroupuio.tbkagroupuiocommodity_tss_kagroupuiocommodityId.GetValueOrDefault();
        //                        _kagroupuiocommodity.tss_CalculateStatus = true;
        //                        organizationService.Update(_kagroupuiocommodity);
        //                    }
        //                    else
        //                    {
        //                        _kasuccess = false;

        //                        tss_kagroupuiocommodity _kagroupuiocommodity = new tss_kagroupuiocommodity();
        //                        _kagroupuiocommodity.Id = _datakagroupuio.tbkagroupuiocommodity_tss_kagroupuiocommodityId.GetValueOrDefault();
        //                        _kagroupuiocommodity.tss_CalculateStatus = false;
        //                        organizationService.Update(_kagroupuiocommodity);

        //                        tss_MasterMarketSize tss_mastermarketsize = new tss_MasterMarketSize();
        //                        tss_mastermarketsize.Id = _mastermarketsizeid;
        //                        tss_mastermarketsize.tss_Status = new OptionSetValue(STATUS_ERROR_MS);
        //                        tss_mastermarketsize.tss_StatusReason = new OptionSetValue(STATUSREASON_ERROR_MS);
        //                        tss_mastermarketsize.tss_ErrorMessage = "Part Number NOT Found !";
        //                        organizationService.Update(tss_mastermarketsize);
        //                    }

        //                    #endregion GENERATE MARKET SIZE SUBLINES KA GROUP UIO COMMODITY
        //                    #endregion --------------------------------------------------------------------------------------------------------------------------------------------------------------
        //                    #endregion --------------------------------------------------------------------------------------------------------------------------------------------------------------
        //                }
        //                else
        //                {
        //                    _kasuccess = false;

        //                    tss_MasterMarketSize _mastermarketsize = new tss_MasterMarketSize();

        //                    _mastermarketsize.tss_PSS = new EntityReference(_datakagroupuio.tbkagroupuiocommodity_LogicalName, _datakagroupuio.tbkagroupuiocommodity_tss_pss.Id);
        //                    _mastermarketsize.tss_Customer = new EntityReference(_datakagroupuio.tbkagroupuiocommodity_LogicalName, _datakagroupuio.tbkagroupuiocommodity_tss_customer.Id);
        //                    //_mastermarketsize.tss_GroupUIOCommodityHeader = new EntityReference(_datakagroupuio.tbgroupuiocommodityaccount_LogicalName, _datakagroupuio.tbgroupuiocommodityaccount_tss_GroupUIOCommodityHeader.Id);
        //                    _mastermarketsize.tss_UnitType = new OptionSetValue(UNITTYPE_COMMODITY);
        //                    _mastermarketsize.tss_Status = new OptionSetValue(STATUS_ERROR_MS);
        //                    _mastermarketsize.tss_StatusReason = new OptionSetValue(STATUSREASON_ERROR_MS);
        //                    _mastermarketsize.tss_ErrorMessage = "Quantity NOT Found !";
        //                    _mastermarketsize.tss_MSPeriodStart = _datakagroupuio.tbkeyaccount_tss_MSPeriodStart.GetValueOrDefault().ToLocalTime();
        //                    _mastermarketsize.tss_MSPeriodEnd = _datakagroupuio.tbkeyaccount_tss_MSPeriodEnd.GetValueOrDefault().ToLocalTime();
        //                    _mastermarketsize.tss_ActivePeriodStart = _datakagroupuio.tbkeyaccount_tss_ActiveStartDate.GetValueOrDefault().ToLocalTime();
        //                    _mastermarketsize.tss_ActivePeriodSEnd = _datakagroupuio.tbkeyaccount_tss_ActiveEndDate.GetValueOrDefault();
        //                    _mastermarketsize.tss_keyaccountid = new EntityReference(_datakagroupuio.tbkeyaccount_LogicalName, _datakagroupuio.tbkeyaccount_tss_keyaccountId.GetValueOrDefault());
        //                    _mastermarketsize.tss_msperiod = _datakagroupuio.tbkeyaccount_tss_Revision.GetValueOrDefault();
        //                    _mastermarketsize.tss_ismsresultpssgenerated = false;
        //                    _mastermarketsize.tss_issublinesgenerated = true;

        //                    Guid _mastermarketsizeid = organizationService.Create(_mastermarketsize);
        //                }

        //                if (_kasuccess)
        //                {
        //                    tss_keyaccount new_keyaccount = new tss_keyaccount();
        //                    new_keyaccount.Id = _datakagroupuio.tbkeyaccount_tss_keyaccountId.GetValueOrDefault();
        //                    new_keyaccount.tss_Status = new OptionSetValue(STATUS_CALCULATE);
        //                    new_keyaccount.tss_Reason = new OptionSetValue(STATUS_REASON_CALCULATE);
        //                    organizationService.Update(new_keyaccount);
        //                }
        //                else
        //                {
        //                    tss_keyaccount new_keyaccount = new tss_keyaccount();
        //                    new_keyaccount.Id = _datakagroupuio.tbkeyaccount_tss_keyaccountId.GetValueOrDefault();
        //                    new_keyaccount.tss_Status = new OptionSetValue(STATUS_ERROR);
        //                    new_keyaccount.tss_Reason = new OptionSetValue(STATUS_REASON_CALCULATE);
        //                    organizationService.Update(new_keyaccount);
        //                }

        //            }

        //            #endregion GENERATE MARKET SIZE KA GROUP UIO COMMODITY
        //        }
        //        else
        //            throw new InvalidWorkflowException("Spare Part Setup NOT found !");

        //    }
        //}

        //public void GenerateMasterMS(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context, EntityCollection _keyaccountcollection)
        //{
        //    try
        //    {

        //FilterExpression _filterstatus = new FilterExpression(LogicalOperator.Or);
        //_filterstatus.AddCondition("tss_status", ConditionOperator.Equal, STATUS_OPEN);
        //_filterstatus.AddCondition("tss_status", ConditionOperator.Equal, STATUS_ERROR);

        //FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
        //_filterexpression.AddFilter(_filterstatus);
        //_filterexpression.AddCondition("tss_pss", ConditionOperator.Equal, context.UserId);
        //_filterexpression.AddCondition("tss_customer", ConditionOperator.NotNull);
        //_filterexpression.AddCondition("tss_activeenddate", ConditionOperator.GreaterEqual, DateTime.Today);
        //_filterexpression.AddCondition("tss_activestartdate", ConditionOperator.LessEqual, DateTime.Today);
        //_filterexpression.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true);

        //LinkEntity _link_kauio = new LinkEntity
        //{
        //    LinkFromEntityName = "tss_keyaccount",
        //    LinkToEntityName = "tss_kauio",
        //    LinkFromAttributeName = "tss_keyaccountid",
        //    LinkToAttributeName = "tss_keyaccountid",
        //    Columns = new ColumnSet(true),
        //    EntityAlias = "kauio",
        //    JoinOperator = JoinOperator.Inner
        //};

        //LinkEntity _link_population = new LinkEntity
        //{
        //    LinkFromEntityName = "tss_kauio",
        //    LinkToEntityName = "new_population",
        //    LinkFromAttributeName = "tss_serialnumber",
        //    LinkToAttributeName = "new_populationid",
        //    Columns = new ColumnSet(true),
        //    EntityAlias = "population",
        //    JoinOperator = JoinOperator.Inner
        //};

        //LinkEntity _link_product = new LinkEntity
        //{
        //    LinkFromEntityName = "new_population",
        //    LinkToEntityName = "product",
        //    LinkFromAttributeName = "trs_productmaster",
        //    LinkToAttributeName = "productid",
        //    Columns = new ColumnSet(true),
        //    EntityAlias = "product",
        //    JoinOperator = JoinOperator.Inner
        //};

        //_link_kauio.LinkEntities.Add(_link_population);
        //_link_population.LinkEntities.Add(_link_product);

        //QueryExpression _queryexpression = new QueryExpression("tss_keyaccount");
        //_queryexpression.LinkEntities.Add(_link_kauio);
        //_queryexpression.Criteria.AddFilter(_filterexpression);

        //EntityCollection _alldatatogenerates = RetrieveMultipleKeyAccount(organizationService, _queryexpression);

        //        FilterExpression _filterexpression = new FilterExpression();
        //        QueryExpression _queryexpression = new QueryExpression();

        //        #region GET KEY ACCOUNT
        //        if (_keyaccountcollection == null)
        //        {
        //            _filterexpression = new FilterExpression(LogicalOperator.Or);
        //            _filterexpression.AddCondition("tss_status", ConditionOperator.Equal, STATUS_OPEN);
        //            _filterexpression.AddCondition("tss_status", ConditionOperator.Equal, STATUS_ERROR);

        //            _filterexpression = new FilterExpression(LogicalOperator.And);
        //            _filterexpression.AddFilter(_filterexpression);
        //            _filterexpression.AddCondition("tss_pss", ConditionOperator.Equal, context.UserId);
        //            _filterexpression.AddCondition("tss_customer", ConditionOperator.NotNull);
        //            _filterexpression.AddCondition("tss_activeenddate", ConditionOperator.GreaterEqual, DateTime.Today);
        //            _filterexpression.AddCondition("tss_activestartdate", ConditionOperator.LessEqual, DateTime.Today);
        //            _filterexpression.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true); //2018.10.12

        //            _queryexpression = new QueryExpression(_DL_tss_keyaccount.EntityName);
        //            _queryexpression.ColumnSet = new ColumnSet(true);
        //            _queryexpression.Orders.Add(new OrderExpression("tss_customername", OrderType.Ascending));
        //            _queryexpression.Criteria.AddFilter(_filterexpression);

        //            _keyaccountcollection = organizationService.RetrieveMultiple(_queryexpression); // _DL_tss_mastermarketsize.Select(organizationService, qKA);
        //        }
        //        #endregion

        //        if (_keyaccountcollection.Entities.Count > 0)
        //        {
        //            foreach (var _keyaccountvar in _keyaccountcollection.Entities)
        //            {
        //                Entity _keyaccount = _keyaccountvar;
        //                bool _keyaccounterrorstatus = true;

        //                #region GET KEY ACCOUNT - KA UIO
        //                _filterexpression = new FilterExpression(LogicalOperator.And);
        //                _filterexpression.AddCondition("tss_keyaccountid", ConditionOperator.Equal, _keyaccount.Id);
        //                _filterexpression.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true);
        //                _filterexpression.AddCondition("tss_calculatestatus", ConditionOperator.Equal, false);
        //                _filterexpression.AddCondition("tss_serialnumber", ConditionOperator.NotNull);

        //                _queryexpression = new QueryExpression(_DL_tss_kauio.EntityName);
        //                _queryexpression.ColumnSet = new ColumnSet(true);
        //                _queryexpression.Orders.Add(new OrderExpression("tss_serialnumbername", OrderType.Ascending));
        //                _queryexpression.Criteria.AddFilter(_filterexpression);

        //                List<Entity> _kauiolist = _retrievehelper.RetrieveMultiple(organizationService, _queryexpression);

        //                if (_kauiolist.Count() > 0)
        //                {
        //                    foreach (var _kauiovar in _kauiolist)
        //                    {
        //                        Entity _kauio = _kauiovar;
        //                        Entity _population = organizationService.Retrieve("new_population", _kauio.GetAttributeValue<EntityReference>("tss_serialnumber").Id, new ColumnSet(true));

        //                        if (_population.Contains("trs_productmaster"))
        //                        {
        //                            Guid _mastermarketSizeid = new Guid();
        //                            bool _method1 = false;
        //                            bool _method2 = false;
        //                            bool _method3 = false;
        //                            bool _method4 = false;
        //                            bool _method5 = false;

        //                            //CHECK METHOD 1
        //                            if (_population.Attributes.Contains("tss_mscurrenthourmeter")
        //                                && _population.Attributes.Contains("tss_mslasthourmeter")
        //                                && _population.Attributes.Contains("tss_mscurrenthourmeterdate")
        //                                && _population.Attributes.Contains("tss_mslasthourmeterdate"))
        //                            {
        //                                _method1 = true;
        //                                Console.Write("Method 1 : " + _method1);
        //                            }

        //                            //CHECK METHOD 2
        //                            if (_population.Attributes.Contains("tss_mscurrenthourmeter")
        //                                && _population.Attributes.Contains("tss_mscurrenthourmeterdate")
        //                                && (_population.Attributes.Contains("new_deliverydate") || _population.Attributes.Contains("trs_warrantyenddate")))
        //                            {
        //                                _method2 = true;
        //                                Console.Write("Method 2 : " + _method2);
        //                            }

        //                            //CHECK METHOD 3
        //                            if (_population.Attributes.Contains("tss_estworkinghour"))
        //                            {
        //                                _method3 = true;
        //                                Console.Write("Method 3 : " + _method3);
        //                            }

        //                            //CHECK METHOD 4
        //                            if (_population.Attributes.Contains("new_deliverydate")
        //                                && _keyaccount.Attributes.Contains("tss_msperiodstart")
        //                                && _keyaccount.Attributes.Contains("tss_msperiodend"))
        //                            {
        //                                _method4 = true;
        //                                Console.Write("Method 4 : " + _method4);
        //                            }

        //                            //CHECK METHOD 5
        //                            if ((_population.Attributes.Contains("new_deliverydate") || _population.Attributes.Contains("trs_warrantyenddate"))
        //                                && _keyaccount.Attributes.Contains("tss_msperiodstart")
        //                                && _keyaccount.Attributes.Contains("tss_msperiodend"))
        //                            {
        //                                _method5 = true;
        //                                Console.Write("Method 5 : " + _method5);
        //                            }

        //                            if (_method1 || _method2 || _method3 || _method4 || _method5)
        //                            {
        //                                decimal _currenthm;
        //                                decimal _lasthm;
        //                                DateTime _currenthmdate;
        //                                DateTime _lasthmdate;
        //                                DateTime _deliverydate;
        //                                DateTime _warrantyenddate = DateTime.MinValue;
        //                                DateTime _calculatemarketsizedate = DateTime.Now.ToLocalTime().Date;

        //                                _queryexpression = new QueryExpression(_DL_product.EntityName);
        //                                _queryexpression.Criteria.AddCondition("productid", ConditionOperator.Equal, _population.GetAttributeValue<EntityReference>("trs_productmaster").Id);
        //                                _queryexpression.ColumnSet.AddColumn("tss_usetyre");

        //                                bool _usetyre = _DL_product.Select(organizationService, _queryexpression).Entities.FirstOrDefault().GetAttributeValue<bool>("tss_usetyre");

        //                                _queryexpression = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
        //                                _queryexpression.ColumnSet = new ColumnSet(true);

        //                                EntityCollection _sparepartsetupcollection = _DL_tss_sparepartsetup.Select(organizationService, _queryexpression);

        //                                if (_sparepartsetupcollection.Entities.Count > 0)
        //                                {
        //                                    Entity _setup = _sparepartsetupcollection.Entities[0];

        //                                    _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        //                                    _DL_tss_mastermarketsize.tss_pss = _kauio.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                                    _DL_tss_mastermarketsize.tss_customer = _kauio.GetAttributeValue<EntityReference>("tss_customer").Id;
        //                                    _DL_tss_mastermarketsize.tss_serialnumber = _population.GetAttributeValue<Guid>("new_populationid");

        //                                    //METHOD 1
        //                                    if (_population.Contains("tss_mscurrenthourmeter")
        //                                       && _population.Contains("tss_mslasthourmeter")
        //                                       && _population.Contains("tss_mscurrenthourmeterdate")
        //                                       && _population.Contains("tss_mslasthourmeterdate"))
        //                                    {
        //                                        _currenthm = _population.GetAttributeValue<decimal>("tss_mscurrenthourmeter");
        //                                        _lasthm = _population.GetAttributeValue<decimal>("tss_mslasthourmeter");
        //                                        _currenthmdate = _population.GetAttributeValue<DateTime>("tss_mscurrenthourmeterdate").ToLocalTime().Date;
        //                                        _lasthmdate = _population.GetAttributeValue<DateTime>("tss_mslasthourmeterdate").ToLocalTime().Date;

        //                                        if ((decimal)(_currenthmdate - _lasthmdate).TotalDays > 0)
        //                                        {
        //                                            decimal _avghmmethod1 = (_currenthm - _lasthm) / ((decimal)(_currenthmdate - _lasthmdate).TotalDays);

        //                                            if (_avghmmethod1 > 0)
        //                                                _DL_tss_mastermarketsize.tss_avghmmethod1 = _avghmmethod1 > (decimal)24 ? (decimal)24 : _avghmmethod1;
        //                                        }
        //                                    }

        //                                    //METHOD 2
        //                                    if (_population.Contains("tss_mscurrenthourmeter")
        //                                       && _population.Contains("tss_mscurrenthourmeterdate")
        //                                       && (_population.Contains("new_deliverydate") || _population.Contains("trs_warrantyenddate")))
        //                                    {
        //                                        _currenthm = _population.GetAttributeValue<decimal>("tss_mscurrenthourmeter");
        //                                        _currenthmdate = _population.GetAttributeValue<DateTime>("tss_mscurrenthourmeterdate").ToLocalTime().Date;
        //                                        _deliverydate = _population.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().Date;

        //                                        if (!_population.Attributes.Contains("trs_warrantyenddate"))
        //                                            _warrantyenddate = _population.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().AddYears(1).Date;
        //                                        else
        //                                            _warrantyenddate = _population.GetAttributeValue<DateTime>("trs_warrantyenddate").ToLocalTime().Date;

        //                                        if ((decimal)(_currenthmdate - _warrantyenddate).TotalDays > 0)
        //                                        {
        //                                            decimal _avghmmethod2 = _currenthm / ((decimal)(_currenthmdate - _warrantyenddate).TotalDays);

        //                                            if (_avghmmethod2 > 0)
        //                                                _DL_tss_mastermarketsize.tss_avghmmethod2 = _avghmmethod2 > (decimal)24 ? (decimal)24 : _avghmmethod2;
        //                                        }
        //                                    }

        //                                    //METHOD 3
        //                                    if (_population.Contains("tss_estworkinghour"))
        //                                    {
        //                                        decimal _estworkinghour = _population.GetAttributeValue<int>("tss_estworkinghour");

        //                                        if (_estworkinghour > 0)
        //                                            _DL_tss_mastermarketsize.tss_avghmmethod3 = _estworkinghour > (decimal)24 ? (decimal)24 : _estworkinghour;
        //                                    }

        //                                    //METHOD 4
        //                                    if (_population.Contains("new_deliverydate")
        //                                        && _keyaccount.Contains("tss_msperiodstart")
        //                                        && _keyaccount.Contains("tss_msperiodend"))
        //                                    {
        //                                        _deliverydate = _population.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().Date;
        //                                        DateTime _msperiodstart = _keyaccount.GetAttributeValue<DateTime>("tss_msperiodstart").ToLocalTime().Date;
        //                                        DateTime _msperiodend = _keyaccount.GetAttributeValue<DateTime>("tss_msperiodend").ToLocalTime().Date;

        //                                        CalculateDate _calculate = new CalculateDate();
        //                                        decimal _diffdeliverydate = _calculate.DiffYear(_deliverydate, _calculatemarketsizedate);
        //                                        decimal _diffperioddate = _calculate.DiffYear(_msperiodstart, _msperiodend);
        //                                        decimal _periodpmmethod4 = (decimal)(_diffdeliverydate + _diffperioddate);

        //                                        if (_periodpmmethod4 > 0)
        //                                            _DL_tss_mastermarketsize.tss_periodpmmethod4 = _periodpmmethod4;
        //                                    }

        //                                    //METHOD 5
        //                                    if (_population.Contains("new_deliverydate")
        //                                        && _keyaccount.Contains("tss_msperiodstart")
        //                                        && _keyaccount.Contains("tss_msperiodend"))
        //                                    {
        //                                        if (!_population.Attributes.Contains("trs_warrantyenddate"))
        //                                            _warrantyenddate = _population.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().AddYears(1).Date;
        //                                        else
        //                                            _warrantyenddate = _population.GetAttributeValue<DateTime>("trs_warrantyenddate").ToLocalTime().Date;

        //                                        _deliverydate = _population.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().Date;
        //                                        DateTime _msperiodstart = _keyaccount.GetAttributeValue<DateTime>("tss_msperiodstart").ToLocalTime().Date;
        //                                        DateTime _msperiodend = _keyaccount.GetAttributeValue<DateTime>("tss_msperiodend").ToLocalTime().Date;

        //                                        CalculateDate _calculate = new CalculateDate();
        //                                        decimal _diffwarrantydate = _calculate.DiffYear(_deliverydate, _warrantyenddate);
        //                                        decimal _diffperioddate = _calculate.DiffYear(_msperiodstart, _msperiodend);

        //                                        decimal _periodpmmethod5 = (decimal)(_diffwarrantydate + _diffperioddate);

        //                                        if (_periodpmmethod5 > 0)
        //                                            _DL_tss_mastermarketsize.tss_periodpmmethod5 = _periodpmmethod5;
        //                                    }

        //                                    if (_population.Contains("tss_populationstatus"))
        //                                        _DL_tss_mastermarketsize.tss_unittype = _population.GetAttributeValue<bool>("tss_populationstatus") ? UNITTYPE_UIO : UNITTYPE_NONUIO;

        //                                    _DL_tss_mastermarketsize.tss_status = STATUS_COMPLETED_MS;
        //                                    _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_COMPLETED_MS;
        //                                    _DL_tss_mastermarketsize.tss_usetyre = _usetyre;
        //                                    _DL_tss_mastermarketsize.tss_msperiodstart = _keyaccount.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                                    _DL_tss_mastermarketsize.tss_msperiodend = _keyaccount.GetAttributeValue<DateTime>("tss_msperiodend");
        //                                    _DL_tss_mastermarketsize.tss_activeperiodstart = _keyaccount.GetAttributeValue<DateTime>("tss_activestartdate");
        //                                    _DL_tss_mastermarketsize.tss_activeperiodsend = _keyaccount.GetAttributeValue<DateTime>("tss_activeenddate");
        //                                    _DL_tss_mastermarketsize.tss_keyaccountid = _keyaccount.Id;
        //                                    _DL_tss_mastermarketsize.tss_msperiod = _keyaccount.GetAttributeValue<int>("tss_revision");
        //                                    _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;

        //                                    _mastermarketSizeid = _DL_tss_mastermarketsize.Insert(organizationService);

        //                                    #region UPDATE STATUS KA UIO (SUCCESS)
        //                                    _DL_tss_kauio = new DL_tss_kauio();
        //                                    _DL_tss_kauio.tss_calculatestatus = true;
        //                                    _DL_tss_kauio.tss_errordescription = "";
        //                                    _DL_tss_kauio.UpdateStatus(organizationService, _kauio.GetAttributeValue<Guid>("tss_kauioid"));
        //                                    #endregion UPDATE STATUS KA UIO (SUCCESS)

        //                                    #region GENERATE MASTER MARKET SIZE LINES
        //                                    GenerateMasterMSLines(organizationService, _DL_tss_mastermarketsize, _population, _setup);
        //                                    #endregion
        //                                }
        //                                else
        //                                {
        //                                    // KAUIO FAILED
        //                                }
        //                            }
        //                            else
        //                            {
        //                                List<string> _errorcollection = new List<string>();
        //                                string _errormessage = "";
        //                                _keyaccounterrorstatus = false;

        //                                _queryexpression = new QueryExpression(_DL_product.EntityName);
        //                                _queryexpression.Criteria.AddCondition("productid", ConditionOperator.Equal, _population.GetAttributeValue<EntityReference>("trs_productmaster").Id);
        //                                _queryexpression.ColumnSet.AddColumn("tss_usetyre");

        //                                bool _usetyre = _DL_product.Select(organizationService, _queryexpression).Entities.FirstOrDefault().GetAttributeValue<bool>("tss_usetyre");

        //                                if (!_population.Contains("tss_mscurrenthourmeter"))
        //                                    _errorcollection.Add("Current Hour Meter");

        //                                if (!_population.Contains("tss_mscurrenthourmeterdate"))
        //                                    _errorcollection.Add("Current Hour Meter (Date)");

        //                                if (!_population.Contains("tss_mslasthourmeter"))
        //                                    _errorcollection.Add("Last Hour Meter");

        //                                if (!_population.Contains("tss_mslasthourmeterdate"))
        //                                    _errorcollection.Add("Last Hour Meter (Date)");

        //                                if (!_population.Contains("new_deliverydate"))
        //                                    _errorcollection.Add("Delivery Date");

        //                                if (_errorcollection.Count() > 0)
        //                                {
        //                                    foreach (var item in _errorcollection)
        //                                    {
        //                                        _errormessage += item + ", ";
        //                                    }

        //                                    _errormessage = _errormessage.Substring(0, _errormessage.Length - 1) + " still not define !";
        //                                }
        //                                else
        //                                    _errormessage = "Current Hour Meter, Current Hour Meter (Date), Last Hour Meter , Last Hour Meter (Date), Delivery Date or Overhaul Date still not define";

        //                                _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();

        //                                if (_population.Contains("tss_populationstatus"))
        //                                    _DL_tss_mastermarketsize.tss_unittype = _population.GetAttributeValue<bool>("tss_populationstatus") ? UNITTYPE_UIO : UNITTYPE_NONUIO;

        //                                _DL_tss_mastermarketsize.tss_pss = _kauio.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                                _DL_tss_mastermarketsize.tss_customer = _kauio.GetAttributeValue<EntityReference>("tss_customer").Id; // pop.GetAttributeValue<EntityReference>("new_customercode").Id;
        //                                _DL_tss_mastermarketsize.tss_serialnumber = _population.GetAttributeValue<Guid>("new_populationid");
        //                                _DL_tss_mastermarketsize.tss_status = STATUS_ERROR_MS;
        //                                _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_ERROR_MS;
        //                                _DL_tss_mastermarketsize.tss_errormessage = _errormessage;
        //                                _DL_tss_mastermarketsize.tss_usetyre = _usetyre;
        //                                _DL_tss_mastermarketsize.tss_msperiodstart = _keyaccount.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                                _DL_tss_mastermarketsize.tss_msperiodend = _keyaccount.GetAttributeValue<DateTime>("tss_msperiodend");
        //                                _DL_tss_mastermarketsize.tss_activeperiodstart = _keyaccount.GetAttributeValue<DateTime>("tss_activestartdate");
        //                                _DL_tss_mastermarketsize.tss_activeperiodsend = _keyaccount.GetAttributeValue<DateTime>("tss_activeenddate");
        //                                _DL_tss_mastermarketsize.tss_keyaccountid = _keyaccount.Id;
        //                                _DL_tss_mastermarketsize.tss_msperiod = _keyaccount.GetAttributeValue<int>("tss_revision");
        //                                _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;

        //                                _DL_tss_mastermarketsize.Insert(organizationService);

        //                                #region UPDATE STATUS KA UIO (FAILED)
        //                                _DL_tss_kauio = new DL_tss_kauio();
        //                                _DL_tss_kauio.tss_calculatestatus = false;
        //                                _DL_tss_kauio.tss_errordescription = _errormessage;
        //                                _DL_tss_kauio.UpdateStatus(organizationService, _kauio.GetAttributeValue<Guid>("tss_kauioid"));
        //                                #endregion UPDATE STATUS KA UIO (FAILED)
        //                            }
        //                        }
        //                        else
        //                        {
        //                            _keyaccounterrorstatus = false;
        //                            _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();

        //                            if (_population.Contains("tss_populationstatus"))
        //                                _DL_tss_mastermarketsize.tss_unittype = _population.GetAttributeValue<bool>("tss_populationstatus") ? UNITTYPE_UIO : UNITTYPE_NONUIO;

        //                            _DL_tss_mastermarketsize.tss_pss = _kauio.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                            _DL_tss_mastermarketsize.tss_customer = _kauio.GetAttributeValue<EntityReference>("tss_customer").Id; // pop.GetAttributeValue<EntityReference>("new_customercode").Id;
        //                            _DL_tss_mastermarketsize.tss_serialnumber = _population.GetAttributeValue<Guid>("new_populationid");
        //                            _DL_tss_mastermarketsize.tss_status = STATUS_ERROR_MS;
        //                            _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_ERROR_MS;
        //                            _DL_tss_mastermarketsize.tss_errormessage = "Product Master NOT found !";
        //                            _DL_tss_mastermarketsize.tss_msperiodstart = _keyaccount.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                            _DL_tss_mastermarketsize.tss_msperiodend = _keyaccount.GetAttributeValue<DateTime>("tss_msperiodend");
        //                            _DL_tss_mastermarketsize.tss_activeperiodstart = _keyaccount.GetAttributeValue<DateTime>("tss_activestartdate");
        //                            _DL_tss_mastermarketsize.tss_activeperiodsend = _keyaccount.GetAttributeValue<DateTime>("tss_activeenddate");
        //                            _DL_tss_mastermarketsize.tss_keyaccountid = _keyaccount.Id;
        //                            _DL_tss_mastermarketsize.tss_msperiod = _keyaccount.GetAttributeValue<int>("tss_revision");
        //                            _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;

        //                            _DL_tss_mastermarketsize.Insert(organizationService);

        //                            #region UPDATE STATUS KA UIO (FAILED)
        //                            _DL_tss_kauio = new DL_tss_kauio();
        //                            _DL_tss_kauio.tss_calculatestatus = false;
        //                            _DL_tss_kauio.tss_errordescription = "Product Master NOT found !";
        //                            _DL_tss_kauio.UpdateStatus(organizationService, _kauio.GetAttributeValue<Guid>("tss_kauioid"));
        //                            #endregion UPDATE STATUS KA UIO (FAILED)
        //                        }
        //                    }
        //                }
        //                #endregion GET KEY ACCOUNT - KA UIO

        //                // --- --- --- //

        //                #region GET KEY ACCOUNT - KA GROUP UIO COMMODITY
        //                _filterexpression = new FilterExpression(LogicalOperator.And);
        //                _filterexpression.AddCondition("tss_keyaccountid", ConditionOperator.Equal, _keyaccount.Id);
        //                _filterexpression.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true);
        //                _filterexpression.AddCondition("tss_calculatestatus", ConditionOperator.Equal, false);
        //                _filterexpression.AddCondition("tss_groupuiocommodity", ConditionOperator.NotNull);

        //                _queryexpression = new QueryExpression(_DL_tss_kagroupuiocommodity.EntityName);
        //                _queryexpression.ColumnSet = new ColumnSet(true);
        //                _queryexpression.Criteria.AddFilter(_filterexpression);

        //                List<Entity> _kagroupuiocommoditylist = _retrievehelper.RetrieveMultiple(organizationService, _queryexpression);

        //                if (_kagroupuiocommoditylist.Count() > 0)
        //                {
        //                    foreach (var _kagroupuiocommodityvar in _kagroupuiocommoditylist)
        //                    {
        //                        Entity _kagroupuiocommodity = _kagroupuiocommodityvar;
        //                        Entity _groupuiocommodityaccount = organizationService.Retrieve("tss_groupuiocommodityaccount", _kagroupuiocommodity.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id, new ColumnSet(true));

        //                        if (_groupuiocommodityaccount.Attributes.Contains("tss_groupuiocommodityheader"))
        //                        {
        //                            if (_groupuiocommodityaccount.Attributes.Contains("tss_qty"))
        //                            {
        //                                if (_groupuiocommodityaccount.GetAttributeValue<int>("tss_qty") > 0)
        //                                {
        //                                    Guid _mastermarketsizeid = new Guid();

        //                                    _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        //                                    _DL_tss_mastermarketsize.tss_pss = _kagroupuiocommodity.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                                    _DL_tss_mastermarketsize.tss_customer = _kagroupuiocommodity.GetAttributeValue<EntityReference>("tss_customer").Id;
        //                                    //_DL_tss_mastermarketsize.tss_groupuiocommodity = groupAccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityname").Id;
        //                                    _DL_tss_mastermarketsize.tss_groupuiocommodityheader = _groupuiocommodityaccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id;
        //                                    _DL_tss_mastermarketsize.tss_qty = _groupuiocommodityaccount.GetAttributeValue<int>("tss_qty");
        //                                    _DL_tss_mastermarketsize.tss_unittype = UNITTYPE_COMMODITY;
        //                                    _DL_tss_mastermarketsize.tss_status = STATUS_COMPLETED_MS;
        //                                    _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_COMPLETED_MS;
        //                                    _DL_tss_mastermarketsize.tss_msperiodstart = _keyaccount.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                                    _DL_tss_mastermarketsize.tss_msperiodend = _keyaccount.GetAttributeValue<DateTime>("tss_msperiodend");
        //                                    _DL_tss_mastermarketsize.tss_activeperiodstart = _keyaccount.GetAttributeValue<DateTime>("tss_activestartdate");
        //                                    _DL_tss_mastermarketsize.tss_activeperiodsend = _keyaccount.GetAttributeValue<DateTime>("tss_activeenddate");
        //                                    _DL_tss_mastermarketsize.tss_keyaccountid = _keyaccount.Id;
        //                                    _DL_tss_mastermarketsize.tss_msperiod = _keyaccount.GetAttributeValue<int>("tss_revision");
        //                                    _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;

        //                                    _mastermarketsizeid = _DL_tss_mastermarketsize.Insert(organizationService);

        //                                    #region UPDATE STATUS KEY ACCOUNT DAN KA GROUP UIO COMMODITY (SUCCESS)
        //                                    _DL_tss_kagroupuiocommodity = new DL_tss_kagroupuiocommodity();
        //                                    _DL_tss_kagroupuiocommodity.tss_calculatestatus = true;
        //                                    _DL_tss_kagroupuiocommodity.UpdateStatus(organizationService, _kagroupuiocommodity.GetAttributeValue<Guid>("tss_kagroupuiocommodityid"));
        //                                    #endregion UPDATE STATUS KEY ACCOUNT DAN KA GROUP UIO COMMODITY (SUCCESS)

        //                                    #region GENERATE MASTER MARKET SIZE SUBLINES - GROUP UIO COMMODITY
        //                                    Entity _tss_groupuiocommodityheader = organizationService.Retrieve("tss_groupuiocommodityheader", _groupuiocommodityaccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id, new ColumnSet(true));

        //                                    _filterexpression = new FilterExpression(LogicalOperator.And);
        //                                    _filterexpression.AddCondition("tss_groupuiocommodityheaderid", ConditionOperator.Equal, _groupuiocommodityaccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id);

        //                                    _queryexpression = new QueryExpression(_DL_tss_groupuiocommodity.EntityName);
        //                                    _queryexpression.ColumnSet = new ColumnSet(true);
        //                                    _queryexpression.Criteria.AddFilter(_filterexpression);

        //                                    EntityCollection _groupuiocommoditycollection = _DL_tss_groupuiocommodity.Select(organizationService, _queryexpression);

        //                                    foreach (var _groupuiocommodity in _groupuiocommoditycollection.Entities)
        //                                    {
        //                                        GenerateSublinesCommodity(organizationService, _DL_tss_mastermarketsize, _groupuiocommodity, _groupuiocommodityaccount, _kagroupuiocommodity);
        //                                    }
        //                                    #endregion GENERATE MASTER MARKET SIZE SUBLINES - GROUP UIO COMMODITY
        //                                }
        //                                else
        //                                {
        //                                    _keyaccounterrorstatus = false;

        //                                    _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        //                                    _DL_tss_mastermarketsize.tss_pss = _kagroupuiocommodity.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                                    _DL_tss_mastermarketsize.tss_customer = _kagroupuiocommodity.GetAttributeValue<EntityReference>("tss_customer").Id;
        //                                    //_DL_tss_mastermarketsize.tss_groupuiocommodity = groupAccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityname").Id;
        //                                    _DL_tss_mastermarketsize.tss_groupuiocommodityheader = _groupuiocommodityaccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id;
        //                                    _DL_tss_mastermarketsize.tss_unittype = UNITTYPE_COMMODITY;
        //                                    _DL_tss_mastermarketsize.tss_status = STATUS_ERROR_MS;
        //                                    _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_ERROR_MS;
        //                                    _DL_tss_mastermarketsize.tss_errormessage = "Quantity NOT Define !";
        //                                    _DL_tss_mastermarketsize.tss_msperiodstart = _keyaccount.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                                    _DL_tss_mastermarketsize.tss_msperiodend = _keyaccount.GetAttributeValue<DateTime>("tss_msperiodend");
        //                                    _DL_tss_mastermarketsize.tss_activeperiodstart = _keyaccount.GetAttributeValue<DateTime>("tss_activestartdate");
        //                                    _DL_tss_mastermarketsize.tss_activeperiodsend = _keyaccount.GetAttributeValue<DateTime>("tss_activeenddate");
        //                                    _DL_tss_mastermarketsize.tss_keyaccountid = _keyaccount.Id;
        //                                    _DL_tss_mastermarketsize.tss_msperiod = _keyaccount.GetAttributeValue<int>("tss_revision");
        //                                    _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;

        //                                    _DL_tss_mastermarketsize.Insert(organizationService);

        //                                    #region UPDATE STATUS KEY ACCOUNT DAN KA GROUP UIO COMMODITY (FAILED)
        //                                    _DL_tss_kagroupuiocommodity = new DL_tss_kagroupuiocommodity();
        //                                    _DL_tss_kagroupuiocommodity.tss_calculatestatus = false;
        //                                    _DL_tss_kagroupuiocommodity.UpdateStatus(organizationService, _kagroupuiocommodity.GetAttributeValue<Guid>("tss_kagroupuiocommodityid"));
        //                                    #endregion UPDATE STATUS KEY ACCOUNT DAN KA GROUP UIO COMMODITY (FAILED)
        //                                }
        //                            }
        //                            else
        //                            {
        //                                // KA GROUP UIO COMMODITY FAILED - QUANTITY NOT DEFINE !
        //                                _keyaccounterrorstatus = false;

        //                                _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        //                                _DL_tss_mastermarketsize.tss_pss = _kagroupuiocommodity.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                                _DL_tss_mastermarketsize.tss_customer = _kagroupuiocommodity.GetAttributeValue<EntityReference>("tss_customer").Id;
        //                                //_DL_tss_mastermarketsize.tss_groupuiocommodity = groupAccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityname").Id;
        //                                _DL_tss_mastermarketsize.tss_groupuiocommodityheader = _groupuiocommodityaccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id;
        //                                _DL_tss_mastermarketsize.tss_unittype = UNITTYPE_COMMODITY;
        //                                _DL_tss_mastermarketsize.tss_status = STATUS_ERROR_MS;
        //                                _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_ERROR_MS;
        //                                _DL_tss_mastermarketsize.tss_errormessage = "Quantity NOT Define !";
        //                                _DL_tss_mastermarketsize.tss_msperiodstart = _keyaccount.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                                _DL_tss_mastermarketsize.tss_msperiodend = _keyaccount.GetAttributeValue<DateTime>("tss_msperiodend");
        //                                _DL_tss_mastermarketsize.tss_activeperiodstart = _keyaccount.GetAttributeValue<DateTime>("tss_activestartdate");
        //                                _DL_tss_mastermarketsize.tss_activeperiodsend = _keyaccount.GetAttributeValue<DateTime>("tss_activeenddate");
        //                                _DL_tss_mastermarketsize.tss_keyaccountid = _keyaccount.Id;
        //                                _DL_tss_mastermarketsize.tss_msperiod = _keyaccount.GetAttributeValue<int>("tss_revision");
        //                                _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;

        //                                _DL_tss_mastermarketsize.Insert(organizationService);

        //                                #region UPDATE STATUS KEY ACCOUNT DAN KA GROUP UIO COMMODITY (FAILED)
        //                                _DL_tss_kagroupuiocommodity = new DL_tss_kagroupuiocommodity();
        //                                _DL_tss_kagroupuiocommodity.tss_calculatestatus = false;
        //                                _DL_tss_kagroupuiocommodity.UpdateStatus(organizationService, _kagroupuiocommodity.GetAttributeValue<Guid>("tss_kagroupuiocommodityid"));
        //                                #endregion UPDATE STATUS KEY ACCOUNT DAN KA GROUP UIO COMMODITY (FAILED)
        //                            }
        //                        }
        //                        else
        //                        {
        //                            // KA GROUP UIO COMMODITY FAILED
        //                        }
        //                    }
        //                }
        //                #endregion GET KEY ACCOUNT - KA GROUP UIO COMMODITY

        //                #region UPDATE KEY ACCOUNT
        //                if (_keyaccounterrorstatus)
        //                {
        //                    _DL_tss_keyaccount = new DL_tss_keyaccount();
        //                    _DL_tss_keyaccount.tss_status = STATUS_CALCULATE;
        //                    _DL_tss_keyaccount.tss_reason = STATUS_REASON_CALCULATE;
        //                    _DL_tss_keyaccount.UpdateStatus(organizationService, _keyaccount.Id);
        //                }
        //                else
        //                {
        //                    _DL_tss_keyaccount = new DL_tss_keyaccount();
        //                    _DL_tss_keyaccount.tss_status = STATUS_ERROR;
        //                    _DL_tss_keyaccount.UpdateStatus(organizationService, _keyaccount.Id);
        //                }
        //                #endregion UPDATE KEY ACCOUNT
        //            }

        //            //BL_tss_marketsizesummarybypartnumber _BL_tss_marketsizesummarybypartnumber = new BL_tss_marketsizesummarybypartnumber();
        //            //_BL_tss_marketsizesummarybypartnumber.GenerateMarketSizeSummaryByPartNumber(organizationService, context);

        //            //BL_tss_marketsizesummarybygroupuiocommodity _BL_tss_marketsizesummarybygroupuiocommodity = new BL_tss_marketsizesummarybygroupuiocommodity();
        //            //_BL_tss_marketsizesummarybygroupuiocommodity.GenerateMarketSizeSummaryByGroupUIOCommodity(organizationService, context, _keyaccountcollection);
        //        }
        //        else
        //        {
        //            throw new InvalidWorkflowException("Key Account NOT Found !");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new InvalidWorkflowException(e.Message.ToString());
        //    }
        //}

        public void GenerateMasterMS(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context, EntityCollection _keyaccountcollection)
        {
            QueryExpression _queryexpression = new QueryExpression("tss_matrixmarketsizeperiod");
            _queryexpression.ColumnSet = new ColumnSet(true);

            EntityCollection _sparepartsetupcollection = organizationService.RetrieveMultiple(_queryexpression);

            #region GET KEY ACCOUNT
            if (_keyaccountcollection == null)
            {
                FilterExpression _filterexpression = new FilterExpression(LogicalOperator.Or);
                _filterexpression.AddCondition("tss_status", ConditionOperator.Equal, STATUS_OPEN);
                _filterexpression.AddCondition("tss_status", ConditionOperator.Equal, STATUS_ERROR);

                _filterexpression = new FilterExpression(LogicalOperator.And);
                _filterexpression.AddFilter(_filterexpression);
                _filterexpression.AddCondition("tss_pss", ConditionOperator.Equal, context.UserId);
                _filterexpression.AddCondition("tss_customer", ConditionOperator.NotNull);
                _filterexpression.AddCondition("tss_activeenddate", ConditionOperator.GreaterEqual, DateTime.Today);
                _filterexpression.AddCondition("tss_activestartdate", ConditionOperator.LessEqual, DateTime.Today);
                _filterexpression.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true); //2018.10.12

                _queryexpression = new QueryExpression(_DL_tss_keyaccount.EntityName);
                _queryexpression.ColumnSet = new ColumnSet(true);
                _queryexpression.Orders.Add(new OrderExpression("tss_customername", OrderType.Ascending));
                _queryexpression.Criteria.AddFilter(_filterexpression);

                _keyaccountcollection = organizationService.RetrieveMultiple(_queryexpression);
            }
            #endregion

            if (_keyaccountcollection.Entities.Count > 0)
            {
                #region GENERATE MARKET SIZE KA UIO
                #region GET KEY ACCOUNT - KA UIO
                //Get KA UIO
                object[] _keyaccountids = _keyaccountcollection.Entities.Select(x => (object)x.Id).ToArray();

                FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
                _filterexpression.AddCondition("tss_keyaccountid", ConditionOperator.In, _keyaccountids);
                _filterexpression.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true);
                _filterexpression.AddCondition("tss_serialnumber", ConditionOperator.NotNull);
                _filterexpression.AddCondition("tss_calculatestatus", ConditionOperator.NotEqual, true);

                _queryexpression = new QueryExpression(_DL_tss_kauio.EntityName);
                _queryexpression.ColumnSet = new ColumnSet(true);
                //_queryexpression.Orders.Add(new OrderExpression("tss_serialnumbername", OrderType.Ascending));
                _queryexpression.Criteria.AddFilter(_filterexpression);

                List<Entity> _kauiolist = _retrievehelper.RetrieveMultiple(organizationService, _queryexpression); // organizationService.RetrieveMultiple(qKAUIO); // _DL_tss_kauio.Select(organizationService, qKAUIO);
                #endregion

                #region GENERATE PROCESS
                //Check Population
                if (_kauiolist.Count > 0)
                {
                    object[] _serialnumbers = _kauiolist.Select(x => (object)x.GetAttributeValue<EntityReference>("tss_serialnumber").Id).ToArray();

                    _filterexpression = new FilterExpression(LogicalOperator.And);
                    _filterexpression.AddCondition("new_populationid", ConditionOperator.In, _serialnumbers);
                    _filterexpression.AddCondition("trs_productmaster", ConditionOperator.NotNull);

                    _queryexpression = new QueryExpression(_DL_population.EntityName);
                    _queryexpression.ColumnSet = new ColumnSet(true);
                    _queryexpression.Orders.Add(new OrderExpression("new_serialnumber", OrderType.Ascending));
                    _queryexpression.Criteria.AddFilter(_filterexpression);

                    List<Entity> _populationlist = _retrievehelper.RetrieveMultiple(organizationService, _queryexpression); // organizationService.RetrieveMultiple(qPop); // _DL_population.Select(organizationService, qPop);

                    if (_populationlist.Count() > 0)
                    {
                        bool _successstatus = true;
                        List<Guid> _marketsizeids = new List<Guid>();

                        foreach (var _population in _populationlist)
                        {
                            Guid _masterMarketSize = new Guid();
                            Entity _currentuio = _kauiolist.Where(x => x.GetAttributeValue<EntityReference>("tss_serialnumber").Id == _population.GetAttributeValue<Guid>("new_populationid")).FirstOrDefault();
                            Entity _currentkeyaccount = _keyaccountcollection.Entities.Where(x => x.Id == _currentuio.GetAttributeValue<EntityReference>("tss_keyaccountid").Id).FirstOrDefault();

                            bool _method1 = false;
                            bool _method2 = false;
                            bool _method3 = false;
                            bool _method4 = false;
                            bool _method5 = false;
                            bool _usetyre = false;

                            _queryexpression = new QueryExpression(_DL_product.EntityName);
                            _queryexpression.Criteria.AddCondition("productid", ConditionOperator.Equal, _population.GetAttributeValue<EntityReference>("trs_productmaster").Id);
                            _queryexpression.ColumnSet.AddColumn("tss_usetyre");

                            Entity _product = organizationService.Retrieve("product", _population.GetAttributeValue<EntityReference>("trs_productmaster").Id, new ColumnSet(true));
                            if (_product != null)
                                _usetyre = _product.GetAttributeValue<bool>("tss_usetyre");

                            //CHECK METHOD 1
                            if (_population.Attributes.Contains("tss_mscurrenthourmeter")
                                && _population.Attributes.Contains("tss_mslasthourmeter")
                                && _population.Attributes.Contains("tss_mscurrenthourmeterdate")
                                && _population.Attributes.Contains("tss_mslasthourmeterdate"))
                            {
                                _method1 = true;
                                Console.Write("Method 1 : " + _method1);
                            }

                            //CHECK METHOD 2
                            if (_population.Attributes.Contains("tss_mscurrenthourmeter")
                                && _population.Attributes.Contains("tss_mscurrenthourmeterdate")
                                && (_population.Attributes.Contains("new_deliverydate") || _population.Attributes.Contains("trs_warrantyenddate")))
                            {
                                _method2 = true;
                                Console.Write("Method 2 : " + _method2);
                            }

                            //CHECK METHOD 3
                            if (_population.Attributes.Contains("tss_estworkinghour"))
                            {
                                _method3 = true;
                                Console.Write("Method 3 : " + _method3);
                            }

                            //CHECK METHOD 4
                            if (_population.Attributes.Contains("new_deliverydate")
                                && _currentkeyaccount.Attributes.Contains("tss_msperiodstart")
                                && _currentkeyaccount.Attributes.Contains("tss_msperiodend"))
                            {
                                _method4 = true;
                                Console.Write("Method 4 : " + _method4);
                            }

                            //CHECK METHOD 5
                            if ((_population.Attributes.Contains("new_deliverydate") || _population.Attributes.Contains("trs_warrantyenddate"))
                                && _currentkeyaccount.Attributes.Contains("tss_msperiodstart")
                                && _currentkeyaccount.Attributes.Contains("tss_msperiodend"))
                            {
                                _method5 = true;
                                Console.Write("Method 5 : " + _method5);
                            }

                            if (_method1 || _method2 || _method3 || _method4 || _method5)
                            {
                                //GENERATE MARKET SIZE UIO / NON UIO
                                decimal _currenthm;
                                decimal _lasthm;
                                DateTime _currenthmdate;
                                DateTime _lasthmdate;
                                DateTime _deliverydate;
                                DateTime _warrantyenddate = DateTime.MinValue;
                                DateTime _calculatemarketsizedate = DateTime.Now.ToLocalTime().Date;

                                //_queryexpression = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
                                //_queryexpression.ColumnSet = new ColumnSet(true);

                                //EntityCollection _sparepartsetupcollection = _DL_tss_sparepartsetup.Select(organizationService, _queryexpression);

                                if (_sparepartsetupcollection.Entities.Count > 0)
                                {
                                    Entity _setup = _sparepartsetupcollection.Entities[0];

                                    _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
                                    _DL_tss_mastermarketsize.tss_pss = _currentuio.GetAttributeValue<EntityReference>("tss_pss").Id;
                                    _DL_tss_mastermarketsize.tss_customer = _currentuio.GetAttributeValue<EntityReference>("tss_customer").Id; // pop.GetAttributeValue<EntityReference>("new_customercode").Id;
                                    _DL_tss_mastermarketsize.tss_serialnumber = _population.GetAttributeValue<Guid>("new_populationid");

                                    //METHOD 1
                                    if (_population.Contains("tss_mscurrenthourmeter")
                                       && _population.Contains("tss_mslasthourmeter")
                                       && _population.Contains("tss_mscurrenthourmeterdate")
                                       && _population.Contains("tss_mslasthourmeterdate"))
                                    {
                                        _currenthm = _population.GetAttributeValue<decimal>("tss_mscurrenthourmeter");
                                        _lasthm = _population.GetAttributeValue<decimal>("tss_mslasthourmeter");
                                        _currenthmdate = _population.GetAttributeValue<DateTime>("tss_mscurrenthourmeterdate").ToLocalTime().Date;
                                        _lasthmdate = _population.GetAttributeValue<DateTime>("tss_mslasthourmeterdate").ToLocalTime().Date;

                                        if ((decimal)(_currenthmdate - _lasthmdate).TotalDays > 0)
                                        {
                                            decimal _avghmmethod1 = (_currenthm - _lasthm) / ((decimal)(_currenthmdate - _lasthmdate).TotalDays);

                                            if (_avghmmethod1 > 0)
                                                _DL_tss_mastermarketsize.tss_avghmmethod1 = _avghmmethod1 > (decimal)24 ? (decimal)24 : _avghmmethod1;
                                        }
                                    }

                                    //METHOD 2
                                    if (_population.Contains("tss_mscurrenthourmeter")
                                       && _population.Contains("tss_mscurrenthourmeterdate")
                                       && (_population.Contains("new_deliverydate") || _population.Contains("trs_warrantyenddate")))
                                    {
                                        _currenthm = _population.GetAttributeValue<decimal>("tss_mscurrenthourmeter");
                                        _currenthmdate = _population.GetAttributeValue<DateTime>("tss_mscurrenthourmeterdate").ToLocalTime().Date;
                                        _deliverydate = _population.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().Date;

                                        if (!_population.Attributes.Contains("trs_warrantyenddate"))
                                            _warrantyenddate = _population.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().AddYears(1).Date;
                                        else
                                            _warrantyenddate = _population.GetAttributeValue<DateTime>("trs_warrantyenddate").ToLocalTime().Date;

                                        if ((decimal)(_currenthmdate - _warrantyenddate).TotalDays > 0)
                                        {
                                            decimal _avghmmethod2 = _currenthm / ((decimal)(_currenthmdate - _warrantyenddate).TotalDays);

                                            if (_avghmmethod2 > 0)
                                                _DL_tss_mastermarketsize.tss_avghmmethod2 = _avghmmethod2 > (decimal)24 ? (decimal)24 : _avghmmethod2;
                                        }
                                    }

                                    //METHOD 3
                                    if (_population.Contains("tss_estworkinghour"))
                                    {
                                        decimal _estworkinghour = _population.GetAttributeValue<int>("tss_estworkinghour");

                                        if (_estworkinghour > 0)
                                            _DL_tss_mastermarketsize.tss_avghmmethod3 = _estworkinghour > (decimal)24 ? (decimal)24 : _estworkinghour;
                                    }

                                    //METHOD 4
                                    if (_population.Contains("new_deliverydate")
                                        && _currentkeyaccount.Contains("tss_msperiodstart")
                                        && _currentkeyaccount.Contains("tss_msperiodend"))
                                    {
                                        _deliverydate = _population.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().Date;
                                        DateTime _msperiodstart = _currentkeyaccount.GetAttributeValue<DateTime>("tss_msperiodstart").ToLocalTime().Date;
                                        DateTime _msperiodend = _currentkeyaccount.GetAttributeValue<DateTime>("tss_msperiodend").ToLocalTime().Date;

                                        CalculateDate _calculate = new CalculateDate();
                                        decimal _diffdeliverydate = _calculate.DiffYear(_deliverydate, _calculatemarketsizedate);
                                        decimal _diffperioddate = _calculate.DiffYear(_msperiodstart, _msperiodend);
                                        decimal _periodpmmethod4 = (decimal)(_diffdeliverydate + _diffperioddate);

                                        if (_periodpmmethod4 > 0)
                                            _DL_tss_mastermarketsize.tss_periodpmmethod4 = _periodpmmethod4;
                                    }

                                    //METHOD 5
                                    if (_population.Contains("new_deliverydate")
                                        && _currentkeyaccount.Contains("tss_msperiodstart")
                                        && _currentkeyaccount.Contains("tss_msperiodend"))
                                    {
                                        if (!_population.Attributes.Contains("trs_warrantyenddate"))
                                            _warrantyenddate = _population.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().AddYears(1).Date;
                                        else
                                            _warrantyenddate = _population.GetAttributeValue<DateTime>("trs_warrantyenddate").ToLocalTime().Date;

                                        _deliverydate = _population.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().Date;
                                        DateTime _msperiodstart = _currentkeyaccount.GetAttributeValue<DateTime>("tss_msperiodstart").ToLocalTime().Date;
                                        DateTime _msperiodend = _currentkeyaccount.GetAttributeValue<DateTime>("tss_msperiodend").ToLocalTime().Date;

                                        CalculateDate _calculate = new CalculateDate();
                                        decimal _diffwarrantydate = _calculate.DiffYear(_deliverydate, _warrantyenddate);
                                        decimal _diffperioddate = _calculate.DiffYear(_msperiodstart, _msperiodend);

                                        decimal _periodpmmethod5 = (decimal)(_diffwarrantydate + _diffperioddate);

                                        if (_periodpmmethod5 > 0)
                                            _DL_tss_mastermarketsize.tss_periodpmmethod5 = _periodpmmethod5;
                                    }

                                    if (_population.Contains("tss_populationstatus"))
                                        _DL_tss_mastermarketsize.tss_unittype = _population.GetAttributeValue<bool>("tss_populationstatus") ? UNITTYPE_UIO : UNITTYPE_NONUIO;

                                    _DL_tss_mastermarketsize.tss_status = STATUS_COMPLETED_MS;
                                    _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_COMPLETED_MS;
                                    _DL_tss_mastermarketsize.tss_usetyre = _usetyre;
                                    _DL_tss_mastermarketsize.tss_msperiodstart = _currentkeyaccount.GetAttributeValue<DateTime>("tss_msperiodstart");
                                    _DL_tss_mastermarketsize.tss_msperiodend = _currentkeyaccount.GetAttributeValue<DateTime>("tss_msperiodend");
                                    _DL_tss_mastermarketsize.tss_activeperiodstart = _currentkeyaccount.GetAttributeValue<DateTime>("tss_activestartdate");
                                    _DL_tss_mastermarketsize.tss_activeperiodsend = _currentkeyaccount.GetAttributeValue<DateTime>("tss_activeenddate");
                                    _DL_tss_mastermarketsize.tss_keyaccountid = _currentkeyaccount.Id;
                                    _DL_tss_mastermarketsize.tss_msperiod = _currentkeyaccount.GetAttributeValue<int>("tss_revision");
                                    _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;
                                    _DL_tss_mastermarketsize.tss_issublinesgenerated = false;

                                    _masterMarketSize = _DL_tss_mastermarketsize.Insert(organizationService);

                                    #region GENERATE MASTER MARKET SIZE LINES
                                    string _errordescription = "";
                                    _errordescription = GenerateMasterMSLines(organizationService, _DL_tss_mastermarketsize, _population, _setup);
                                    #endregion

                                    if (_errordescription == "")
                                    {
                                        #region UPDATE STATUS KEY ACCOUNT DAN KA UIO (SUCCESS)
                                        _DL_tss_keyaccount = new DL_tss_keyaccount();
                                        _DL_tss_keyaccount.tss_status = STATUS_CALCULATE;
                                        _DL_tss_keyaccount.tss_reason = STATUS_REASON_CALCULATE;
                                        _DL_tss_keyaccount.UpdateStatus(organizationService, _currentuio.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

                                        _DL_tss_kauio = new DL_tss_kauio();
                                        _DL_tss_kauio.tss_calculatestatus = true;
                                        _DL_tss_kauio.tss_errordescription = "";
                                        _DL_tss_kauio.UpdateStatus(organizationService, _currentuio.GetAttributeValue<Guid>("tss_kauioid"));
                                        #endregion UPDATE STATUS KEY ACCOUNT DAN KA UIO (SUCCESS)
                                    }
                                    else
                                    {
                                        #region UPDATE STATUS MASTER MARKET SIZE DAN KEY ACCOUNT DAN KA UIO (FAILED)
                                        _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
                                        _DL_tss_mastermarketsize.tss_status = STATUS_ERROR;
                                        _DL_tss_mastermarketsize.tss_errormessage = _errordescription;
                                        _DL_tss_mastermarketsize.Update(organizationService, _masterMarketSize);

                                        _DL_tss_keyaccount = new DL_tss_keyaccount();
                                        _DL_tss_keyaccount.tss_status = STATUS_ERROR;
                                        _DL_tss_keyaccount.UpdateStatus(organizationService, _currentuio.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

                                        _DL_tss_kauio = new DL_tss_kauio();
                                        _DL_tss_kauio.tss_calculatestatus = false;
                                        _DL_tss_kauio.tss_errordescription = _errordescription;
                                        _DL_tss_kauio.UpdateStatus(organizationService, _currentuio.GetAttributeValue<Guid>("tss_kauioid"));
                                        #endregion UPDATE STATUS MASTER MARKET SIZE DAN KEY ACCOUNT DAN KA UIO (FAILED)
                                    }
                                }
                            }
                            else
                            {
                                List<string> _errorcollection = new List<string>();
                                string _errormessage = "";

                                //Generate MS UIO/Non UIO
                                _queryexpression = new QueryExpression(_DL_product.EntityName);
                                _queryexpression.Criteria.AddCondition("productid", ConditionOperator.Equal, _population.GetAttributeValue<EntityReference>("trs_productmaster").Id);
                                _queryexpression.ColumnSet.AddColumn("tss_usetyre");

                                //bool _usetyre = _DL_product.Select(organizationService, _queryexpression).Entities.FirstOrDefault().GetAttributeValue<bool>("tss_usetyre");

                                if (!_population.Contains("tss_mscurrenthourmeter"))
                                    _errorcollection.Add("Current Hour Meter");

                                if (!_population.Contains("tss_mscurrenthourmeterdate"))
                                    _errorcollection.Add("Current Hour Meter (Date)");

                                if (!_population.Contains("tss_mslasthourmeter"))
                                    _errorcollection.Add("Last Hour Meter");

                                if (!_population.Contains("tss_mslasthourmeterdate"))
                                    _errorcollection.Add("Last Hour Meter (Date)");

                                if (!_population.Contains("new_deliverydate"))
                                    _errorcollection.Add("Delivery Date");

                                if (_errorcollection.Count() > 0)
                                {
                                    foreach (var item in _errorcollection)
                                    {
                                        _errormessage += item + ", ";
                                    }

                                    _errormessage = _errormessage.Substring(0, _errormessage.Length - 1) + " still not define !";
                                }
                                else
                                    _errormessage = "Current Hour Meter, Current Hour Meter (Date), Last Hour Meter , Last Hour Meter (Date), Delivery Date or Overhaul Date still not define";

                                _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();

                                if (_population.Contains("tss_populationstatus"))
                                    _DL_tss_mastermarketsize.tss_unittype = _population.GetAttributeValue<bool>("tss_populationstatus") ? UNITTYPE_UIO : UNITTYPE_NONUIO;

                                _DL_tss_mastermarketsize.tss_pss = _currentuio.GetAttributeValue<EntityReference>("tss_pss").Id;
                                _DL_tss_mastermarketsize.tss_customer = _currentuio.GetAttributeValue<EntityReference>("tss_customer").Id; // pop.GetAttributeValue<EntityReference>("new_customercode").Id;
                                _DL_tss_mastermarketsize.tss_serialnumber = _population.GetAttributeValue<Guid>("new_populationid");
                                _DL_tss_mastermarketsize.tss_status = STATUS_ERROR_MS;
                                _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_ERROR_MS;
                                _DL_tss_mastermarketsize.tss_errormessage = _errormessage;
                                _DL_tss_mastermarketsize.tss_usetyre = _usetyre;
                                _DL_tss_mastermarketsize.tss_msperiodstart = _currentkeyaccount.GetAttributeValue<DateTime>("tss_msperiodstart");
                                _DL_tss_mastermarketsize.tss_msperiodend = _currentkeyaccount.GetAttributeValue<DateTime>("tss_msperiodend");
                                _DL_tss_mastermarketsize.tss_activeperiodstart = _currentkeyaccount.GetAttributeValue<DateTime>("tss_activestartdate");
                                _DL_tss_mastermarketsize.tss_activeperiodsend = _currentkeyaccount.GetAttributeValue<DateTime>("tss_activeenddate");
                                _DL_tss_mastermarketsize.tss_keyaccountid = _currentkeyaccount.Id;
                                _DL_tss_mastermarketsize.tss_msperiod = _currentkeyaccount.GetAttributeValue<int>("tss_revision");
                                _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;
                                _DL_tss_mastermarketsize.tss_issublinesgenerated = true;

                                _DL_tss_mastermarketsize.Insert(organizationService);

                                #region UPDATE STATUS KEY ACCOUNT DAN KA UIO (FAILED)
                                _DL_tss_keyaccount = new DL_tss_keyaccount();
                                _DL_tss_keyaccount.tss_status = STATUS_ERROR;
                                _DL_tss_keyaccount.UpdateStatus(organizationService, _currentuio.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

                                _DL_tss_kauio = new DL_tss_kauio();
                                _DL_tss_kauio.tss_calculatestatus = false;
                                _DL_tss_kauio.tss_errordescription = _errormessage;
                                _DL_tss_kauio.UpdateStatus(organizationService, _currentuio.GetAttributeValue<Guid>("tss_kauioid"));
                                #endregion UPDATE STATUS KEY ACCOUNT DAN KA UIO (FAILED)
                            }
                        }
                    }
                }
                #endregion GENERATE PROCESS
                #endregion GENERATE MARKET SIZE KA UIO

                // --- --- //

                #region GENERATE MARKET SIZE KA GROUP UIO COMMODITY
                #region GET KEY ACCOUNT - KA GROUP UIO COMMODITY
                _filterexpression = new FilterExpression(LogicalOperator.And);
                _filterexpression.AddCondition("tss_keyaccountid", ConditionOperator.In, _keyaccountids);
                _filterexpression.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true);
                _filterexpression.AddCondition("tss_groupuiocommodity", ConditionOperator.NotNull);

                _queryexpression = new QueryExpression(_DL_tss_kagroupuiocommodity.EntityName);
                _queryexpression.ColumnSet = new ColumnSet(true);
                _queryexpression.Criteria.AddFilter(_filterexpression);

                List<Entity> _kagroupuiocommoditylist = _retrievehelper.RetrieveMultiple(organizationService, _queryexpression); // _DL_tss_kagroupuiocommodity.Select(organizationService, qkaUIOCommodity);
                #endregion GET KEY ACCOUNT - KA GROUP UIO COMMODITY

                #region GENERATE PROCESS
                if (_kagroupuiocommoditylist.Count > 0)
                {
                    #region Get Group Commodity Account
                    object[] _kagroupuiocommodityids = _kagroupuiocommoditylist.Select(x => (object)x.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id).ToArray();

                    _filterexpression = new FilterExpression(LogicalOperator.And);
                    _filterexpression.AddCondition("tss_groupuiocommodityaccountid", ConditionOperator.In, _kagroupuiocommodityids);
                    _filterexpression.AddCondition("tss_groupuiocommodityheader", ConditionOperator.NotNull);

                    _queryexpression = new QueryExpression(_DL_tss_groupuiocommodityaccount.EntityName);
                    _queryexpression.ColumnSet = new ColumnSet(true);
                    _queryexpression.Criteria.AddFilter(_filterexpression);

                    List<Entity> _groupuiocommodityaccountlist = _retrievehelper.RetrieveMultiple(organizationService, _queryexpression);
                    #endregion

                    if (_groupuiocommodityaccountlist.Count > 0)
                    {
                        foreach (var _groupuiocommodityaccount in _groupuiocommodityaccountlist)
                        {
                            Guid _mastermarketsizeid = new Guid();
                            Entity _currentkagroupuio = _kagroupuiocommoditylist.Where(x => x.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id == _groupuiocommodityaccount.GetAttributeValue<Guid>("tss_groupuiocommodityaccountid")).FirstOrDefault();
                            Entity _currentkeyaccount = _keyaccountcollection.Entities.Where(x => x.Id == _currentkagroupuio.GetAttributeValue<EntityReference>("tss_keyaccountid").Id).FirstOrDefault();

                            if (_groupuiocommodityaccount.Contains("tss_qty") && _groupuiocommodityaccount.GetAttributeValue<int>("tss_qty") > 0)
                            {
                                _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
                                _DL_tss_mastermarketsize.tss_pss = _currentkagroupuio.GetAttributeValue<EntityReference>("tss_pss").Id;
                                _DL_tss_mastermarketsize.tss_customer = _currentkagroupuio.GetAttributeValue<EntityReference>("tss_customer").Id;
                                //_DL_tss_mastermarketsize.tss_groupuiocommodity = groupAccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityname").Id;
                                _DL_tss_mastermarketsize.tss_groupuiocommodityheader = _groupuiocommodityaccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id;
                                _DL_tss_mastermarketsize.tss_qty = _groupuiocommodityaccount.GetAttributeValue<int>("tss_qty");
                                _DL_tss_mastermarketsize.tss_unittype = UNITTYPE_COMMODITY;
                                _DL_tss_mastermarketsize.tss_status = STATUS_COMPLETED_MS;
                                _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_COMPLETED_MS;
                                _DL_tss_mastermarketsize.tss_msperiodstart = _currentkeyaccount.GetAttributeValue<DateTime>("tss_msperiodstart");
                                _DL_tss_mastermarketsize.tss_msperiodend = _currentkeyaccount.GetAttributeValue<DateTime>("tss_msperiodend");
                                _DL_tss_mastermarketsize.tss_activeperiodstart = _currentkeyaccount.GetAttributeValue<DateTime>("tss_activestartdate");
                                _DL_tss_mastermarketsize.tss_activeperiodsend = _currentkeyaccount.GetAttributeValue<DateTime>("tss_activeenddate");
                                _DL_tss_mastermarketsize.tss_keyaccountid = _currentkeyaccount.Id;
                                _DL_tss_mastermarketsize.tss_msperiod = _currentkeyaccount.GetAttributeValue<int>("tss_revision");
                                _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;
                                _DL_tss_mastermarketsize.tss_issublinesgenerated = false;

                                _mastermarketsizeid = _DL_tss_mastermarketsize.Insert(organizationService);

                                #region UPDATE STATUS KA GROUP UIO COMMODITY (SUCCESS)
                                _DL_tss_keyaccount = new DL_tss_keyaccount();
                                _DL_tss_keyaccount.tss_status = STATUS_CALCULATE;
                                _DL_tss_keyaccount.UpdateStatus(organizationService, _currentkagroupuio.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

                                _DL_tss_kagroupuiocommodity = new DL_tss_kagroupuiocommodity();
                                _DL_tss_kagroupuiocommodity.tss_calculatestatus = true;
                                _DL_tss_kagroupuiocommodity.UpdateStatus(organizationService, _currentkagroupuio.GetAttributeValue<Guid>("tss_kagroupuiocommodityid"));
                                #endregion UPDATE STATUS KA GROUP UIO COMMODITY (SUCCESS)

                                #region GENERATE MASTER MARKET SIZE SUBLINES - GROUP UIO COMMODITY
                                Entity _tss_groupuiocommodityaccount = organizationService.Retrieve("tss_groupuiocommodityaccount", _currentkagroupuio.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id, new ColumnSet(true));

                                if (_tss_groupuiocommodityaccount != null)
                                {
                                    Entity _tss_groupuiocommodityheader = organizationService.Retrieve("tss_groupuiocommodityheader", _tss_groupuiocommodityaccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id, new ColumnSet(true));

                                    _filterexpression = new FilterExpression(LogicalOperator.And);
                                    _filterexpression.AddCondition("tss_groupuiocommodityheaderid", ConditionOperator.In, _tss_groupuiocommodityheader.Id);

                                    _queryexpression = new QueryExpression(_DL_tss_groupuiocommodity.EntityName);
                                    _queryexpression.ColumnSet = new ColumnSet(true);
                                    _queryexpression.Criteria.AddFilter(_filterexpression);

                                    EntityCollection _groupuiocommoditycollection = _DL_tss_groupuiocommodity.Select(organizationService, _queryexpression);

                                    if (_groupuiocommoditycollection.Entities.Count > 0)
                                    {
                                        foreach (var _groupuiocommodity in _groupuiocommoditycollection.Entities)
                                        {
                                            if (_groupuiocommodity != null)
                                                GenerateSublinesCommodity(organizationService, _DL_tss_mastermarketsize, _groupuiocommodity, _groupuiocommodityaccount, _currentkagroupuio);
                                        }
                                    }
                                }
                                #endregion GENERATE MASTER MARKET SIZE SUBLINES - GROUP UIO COMMODITY
                            }
                            else
                            {
                                _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
                                _DL_tss_mastermarketsize.tss_pss = _currentkagroupuio.GetAttributeValue<EntityReference>("tss_pss").Id;
                                _DL_tss_mastermarketsize.tss_customer = _currentkagroupuio.GetAttributeValue<EntityReference>("tss_customer").Id;
                                //_DL_tss_mastermarketsize.tss_groupuiocommodity = groupAccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityname").Id;
                                _DL_tss_mastermarketsize.tss_groupuiocommodityheader = _groupuiocommodityaccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id;
                                _DL_tss_mastermarketsize.tss_unittype = UNITTYPE_COMMODITY;
                                _DL_tss_mastermarketsize.tss_status = STATUS_ERROR_MS;
                                _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_ERROR_MS;
                                _DL_tss_mastermarketsize.tss_errormessage = "Qty still not define";
                                _DL_tss_mastermarketsize.tss_msperiodstart = _currentkeyaccount.GetAttributeValue<DateTime>("tss_msperiodstart");
                                _DL_tss_mastermarketsize.tss_msperiodend = _currentkeyaccount.GetAttributeValue<DateTime>("tss_msperiodend");
                                _DL_tss_mastermarketsize.tss_activeperiodstart = _currentkeyaccount.GetAttributeValue<DateTime>("tss_activestartdate");
                                _DL_tss_mastermarketsize.tss_activeperiodsend = _currentkeyaccount.GetAttributeValue<DateTime>("tss_activeenddate");
                                _DL_tss_mastermarketsize.tss_keyaccountid = _currentkeyaccount.Id;
                                _DL_tss_mastermarketsize.tss_msperiod = _currentkeyaccount.GetAttributeValue<int>("tss_revision");
                                _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;
                                _DL_tss_mastermarketsize.tss_issublinesgenerated = true;

                                _DL_tss_mastermarketsize.Insert(organizationService);

                                #region UPDATE STATUS KA GROUP UIO COMMODITY (FAILED)
                                _DL_tss_keyaccount = new DL_tss_keyaccount();
                                _DL_tss_keyaccount.tss_status = STATUS_ERROR;
                                _DL_tss_keyaccount.UpdateStatus(organizationService, _currentkagroupuio.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

                                _DL_tss_kagroupuiocommodity = new DL_tss_kagroupuiocommodity();
                                _DL_tss_kagroupuiocommodity.tss_calculatestatus = false;
                                _DL_tss_kagroupuiocommodity.UpdateStatus(organizationService, _currentkagroupuio.GetAttributeValue<Guid>("tss_kagroupuiocommodityid"));
                                #endregion UPDATE STATUS KA GROUP UIO COMMODITY (FAILED)
                            }
                        }
                    }
                }
                #endregion GENERATE PROCESS
                #endregion GENERATE MARKET SIZE KA GROUP UIO COMMODITY

                //Guid[] customerID = listKeyAccount.Entities.GroupBy(x => x.GetAttributeValue<EntityReference>("tss_customer").Id).Select(x => x.First().GetAttributeValue<EntityReference>("tss_customer").Id).ToArray();

                //BL_tss_marketsizeresultpss _BL_tss_marketsizeresultpss = new BL_tss_marketsizeresultpss();
                //_BL_tss_marketsizeresultpss.GenerateMarketSizeResultPSS_New(organizationService, context, customerID);

                //BL_tss_marketsizesummarybypartnumber _BL_tss_marketsizesummarybypartnumber = new BL_tss_marketsizesummarybypartnumber();
                //_BL_tss_marketsizesummarybypartnumber.GenerateMarketSizeSummaryByPartNumber(organizationService, context);

                //BL_tss_marketsizesummarybygroupuiocommodity _BL_tss_marketsizesummarybygroupuiocommodity = new BL_tss_marketsizesummarybygroupuiocommodity();
                //_BL_tss_marketsizesummarybygroupuiocommodity.GenerateMarketSizeSummaryByGroupUIOCommodity(organizationService, context, _keyaccountcollection);
            }
            else
            {
                throw new InvalidWorkflowException("Key Account Not Found !");
            }
        }

        public string GenerateMasterMSLines(IOrganizationService organizationService, DL_tss_mastermarketsize _mastermarketsize, Entity _population, Entity _setup)
        {
            //List<DL_tss_mastermarketsizelines> _mastermarketsizelineslist = new List<DL_tss_mastermarketsizelines>();

            decimal _populationcurrenthm;
            decimal _methodvalue = 0;
            int _calculatemethod = 0;
            int _interval = 0;
            int _duedatems = 0;
            int _hmpm;
            int _hmconsumppm;
            int _forecastpmdate;
            int _pmperiod = 0;
            string _errordescription = "";
            DateTime _populationcurrentdate;
            DateTime _pmdate;
            DateTime _nextpmdate;

            QueryExpression _queryexpression = new QueryExpression("tss_unitgroupmarketsize");
            _queryexpression.Criteria.AddCondition("tss_model", ConditionOperator.Equal, _population.GetAttributeValue<EntityReference>("trs_productmaster").Id);
            _queryexpression.ColumnSet.AddColumn("tss_maintenanceinterval");
            EntityCollection _unitgroupmarketsizecollection = organizationService.RetrieveMultiple(_queryexpression);

            _populationcurrenthm = Math.Round(_population.GetAttributeValue<decimal>("tss_mscurrenthourmeter"), 0);
            _populationcurrentdate = _population.Attributes.Contains("tss_mscurrenthourmeterdate") ? _population.GetAttributeValue<DateTime>("tss_mscurrenthourmeterdate").ToLocalTime().Date : DateTime.MinValue;

            if (_unitgroupmarketsizecollection.Entities.Count > 0)
            {
                Entity _unitgroupmarketsize = _unitgroupmarketsizecollection.Entities[0];

                _interval = _unitgroupmarketsize.GetAttributeValue<int>("tss_maintenanceinterval");
                _duedatems = _setup.GetAttributeValue<int>("tss_duedatems");

                if (_mastermarketsize._tss_avghmmethod1)
                {
                    _methodvalue = _mastermarketsize.tss_avghmmethod1;
                    _calculatemethod = MTD1;
                }
                else if (_mastermarketsize._tss_avghmmethod2)
                {
                    _methodvalue = _mastermarketsize.tss_avghmmethod2;
                    _calculatemethod = MTD2;
                }
                else if (_mastermarketsize._tss_avghmmethod3)
                {
                    _methodvalue = _mastermarketsize.tss_avghmmethod3;
                    _calculatemethod = MTD3;
                }
                else if (_mastermarketsize._tss_periodpmmethod4)
                {
                    _methodvalue = _mastermarketsize.tss_periodpmmethod4;
                    _calculatemethod = MTD4;
                }
                else if (_mastermarketsize._tss_periodpmmethod5)
                {
                    _methodvalue = _mastermarketsize.tss_periodpmmethod5;
                    _calculatemethod = MTD5;
                }

                if (_calculatemethod == MTD1 || _calculatemethod == MTD2 || _calculatemethod == MTD3)
                {
                    _hmpm = (int)Math.Round(_populationcurrenthm, 0);
                    _pmdate = _populationcurrentdate;

                    if (_populationcurrentdate != DateTime.MinValue && _interval > 0 && _methodvalue > 0)
                    {
                        _nextpmdate = _pmdate.AddDays((int)Math.Round((((Math.Round((decimal)_hmpm / _interval, 0) * _interval) + _interval) - _hmpm) / _methodvalue, 0));

                        while (_pmdate <= _setup.GetAttributeValue<DateTime>("tss_enddatemarketsize").ToLocalTime().Date && _nextpmdate <= _setup.GetAttributeValue<DateTime>("tss_enddatemarketsize"))
                        {
                            _DL_tss_mastermarketsizelines = new DL_tss_mastermarketsizelines();

                            //if (_DL_tss_mastermarketsizelines.tss_estimatedpmdate.ToLocalTime().Date >= _setup.GetAttributeValue<DateTime>("tss_startdatems").ToLocalTime().Date) // 2019.03.27
                            if (_pmdate >= _setup.GetAttributeValue<DateTime>("tss_startdatemarketsize").ToLocalTime().Date)
                                _pmperiod += 1;

                            _DL_tss_mastermarketsizelines.tss_mastermarketsizeref = _mastermarketsize.tss_mastermarketsizeid;
                            _DL_tss_mastermarketsizelines.tss_hmpm = (int)(Math.Round((decimal)_hmpm / _interval, 0) * _interval) + _interval;
                            _DL_tss_mastermarketsizelines.tss_hmconsumppm = _DL_tss_mastermarketsizelines.tss_hmpm - _hmpm;
                            _DL_tss_mastermarketsizelines.tss_forecastpmdate = (int)Math.Round(_DL_tss_mastermarketsizelines.tss_hmconsumppm / _methodvalue, 0);
                            _DL_tss_mastermarketsizelines.tss_estimatedpmdate = _pmdate.AddDays(_DL_tss_mastermarketsizelines.tss_forecastpmdate);
                            _DL_tss_mastermarketsizelines.tss_pmperiod = _pmperiod;
                            _DL_tss_mastermarketsizelines.tss_methodcalculationused = _calculatemethod;
                            //_DL_tss_mastermarketsizelines.tss_aging = (int)Math.Floor((delivDt - endMSDt).TotalDays / 365); TBD
                            //_DL_tss_mastermarketsizelines.tss_annualaging TBD
                            _DL_tss_mastermarketsizelines.tss_status = STATUS_LINES_DRAFT;
                            _DL_tss_mastermarketsizelines.tss_statusreason = STATUS_REASON_LINES_OPEN;

                            _DL_tss_mastermarketsizelines.tss_pm = _DL_tss_mastermarketsizelines.GetPMType(organizationService, _population.GetAttributeValue<EntityReference>("trs_productmaster").Id, _DL_tss_mastermarketsizelines.tss_hmpm).ToString();
                            //Entity _product = organizationService.Retrieve("product", _population.GetAttributeValue<EntityReference>("trs_productmaster").Id, new ColumnSet(true));
                            //int _pm = _matrixperiodmaintenancetype.Where(x => x.GetAttributeValue<EntityReference>("tss_unitgroup").Id == _product.GetAttributeValue<EntityReference>("defaultuomscheduleid").Id && 
                            //            x.GetAttributeValue<int>("tss_nextplanhm") == _DL_tss_mastermarketsizelines.GetNextPlanHM(_DL_tss_mastermarketsizelines.tss_hmpm)).Select(x => x.GetAttributeValue<int>("tss_typepm")).FirstOrDefault();
                            //_DL_tss_mastermarketsizelines.tss_pm = _pm.ToString();

                            _hmpm = _DL_tss_mastermarketsizelines.tss_hmpm;
                            _hmconsumppm = _DL_tss_mastermarketsizelines.tss_hmconsumppm;
                            _forecastpmdate = _DL_tss_mastermarketsizelines.tss_forecastpmdate;
                            _pmdate = _DL_tss_mastermarketsizelines.tss_estimatedpmdate;
                            _nextpmdate = _pmdate.AddDays((int)Math.Round((((Math.Round((decimal)_hmpm / _interval, 0) * _interval) + _interval) - _hmpm) / _methodvalue, 0));

                            //DUE DATE = NEXT PM DATE - DUE DATE MARKET SIZE
                            if (_pmperiod > 0 && _nextpmdate <= _setup.GetAttributeValue<DateTime>("tss_enddatemarketsize").ToLocalTime().Date)
                                _DL_tss_mastermarketsizelines.tss_duedate = _nextpmdate.AddDays(-_duedatems);

                            _DL_tss_mastermarketsizelines.Insert(organizationService);
                            //_mastermarketsizelineslist.Add(_DL_tss_mastermarketsizelines);
                        }

                        // 2019.03.22 - GENERATE SUBLINES
                        //foreach (var lines in listLines)
                        //{
                        //    #region Generate Sub Lines
                        //    GenerateMasterMSSubLines(organizationService, population, setup, lines, ms.tss_usetyre, listLines[0].tss_hmpm, listLines[listLines.Count - 1].tss_hmpm, ms);
                        //    #endregion
                        //}
                    }
                    else
                        _errordescription = "Process Generate Lines FAILED. Please Check 'Population Current Date' / 'Maintenance Interval'";
                }
                else if (_calculatemethod == MTD4 || _calculatemethod == MTD5)
                {
                    DateTime _deliverydate = _population.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime();
                    DateTime _today = DateTime.Now;
                    DateTime _msperiodstart = _mastermarketsize.tss_msperiodstart.ToLocalTime();
                    DateTime _msperiodend = _mastermarketsize.tss_msperiodend.ToLocalTime();

                    DL_tss_mastermarketsizelines _mastermarketsizelines = new DL_tss_mastermarketsizelines();
                    _mastermarketsizelines.tss_mastermarketsizeref = new EntityReference("tss_mastermarketsize", _mastermarketsize.tss_mastermarketsizeid).Id;
                    _mastermarketsizelines.tss_methodcalculationused = _calculatemethod;
                    _mastermarketsizelines.tss_status = STATUS_LINES_DRAFT;
                    _mastermarketsizelines.tss_statusreason = STATUS_REASON_LINES_OPEN;

                    //CALCULATE AGING
                    CalculateDate _calculate = new CalculateDate();

                    int _aging = 0;
                    int _agingmsperiod = _calculate.DiffYear(_msperiodstart, _msperiodend);
                    int _agingdeliverydate = _calculate.DiffYear(_deliverydate, _today);

                    _aging = _agingmsperiod + _agingdeliverydate;

                    _mastermarketsizelines.tss_aging = Convert.ToInt32(_aging);
                    _mastermarketsizelines.Insert(organizationService);

                    // 2019.03.22 - GENERATE SUBLINES
                    //GenerateMasterMSSubLines(organizationService, population, setup, newLine, ms.tss_usetyre, 0, 0, ms);
                }
                else
                    _errordescription = "Process Generate Lines FAILED. Calculate Method is NOT Found !";
            }
            else
                _errordescription = "Process Generate Lines FAILED. Unit Group is NOT Found !";

            return _errordescription;
        }

        public void GenerateMasterMSSublinesv2(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context, EntityCollection _keyaccountcollection)
        {
            try
            {
                QueryExpression _queryexpression = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
                _queryexpression.ColumnSet = new ColumnSet(true);
                EntityCollection setups = _DL_tss_sparepartsetup.Select(organizationService, _queryexpression);

                if (setups.Entities.Count() > 0)
                {
                    foreach (var _keyaccountitem in _keyaccountcollection.Entities)
                    {
                        Entity _keyaccount = _keyaccountitem;

                        FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
                        _filterexpression.AddCondition("tss_unittype", ConditionOperator.NotNull); // 2019.03.21
                        _filterexpression.AddCondition("tss_unittype", ConditionOperator.NotEqual, UNITTYPE_COMMODITY);
                        _filterexpression.AddCondition("tss_pss", ConditionOperator.Equal, context.UserId);
                        _filterexpression.AddCondition("tss_status", ConditionOperator.Equal, STATUS_COMPLETED_MS);
                        _filterexpression.AddCondition("tss_keyaccountid", ConditionOperator.Equal, _keyaccount.Id);
                        _filterexpression.AddCondition("tss_issublinesgenerated", ConditionOperator.Equal, false);
                        _filterexpression.AddCondition("tss_activeperiodstart", ConditionOperator.LessEqual, _keyaccount.GetAttributeValue<DateTime>("tss_activestartdate").ToLocalTime());
                        _filterexpression.AddCondition("tss_activeperiodsend", ConditionOperator.GreaterEqual, _keyaccount.GetAttributeValue<DateTime>("tss_activeenddate").ToLocalTime());

                        _queryexpression = new QueryExpression(_DL_tss_mastermarketsize.EntityName);
                        _queryexpression.Criteria.AddFilter(_filterexpression);
                        _queryexpression.ColumnSet.AddColumns("tss_mastermarketsizeid");

                        List<Entity> _mastermarketsizecollection = _retrievehelper.RetrieveMultiple(organizationService, _queryexpression);

                        if (_mastermarketsizecollection.Count > 0)
                        {
                            object[] _mastermarketsizeids = _mastermarketsizecollection.Select(x => (object)x.Id).ToArray();

                            FilterExpression _filterlines1 = new FilterExpression(LogicalOperator.And);
                            _filterlines1.AddCondition("tss_pmperiod", ConditionOperator.GreaterThan, 0);
                            _filterlines1.AddCondition("tss_duedate", ConditionOperator.NotNull);

                            FilterExpression _filterlines2 = new FilterExpression(LogicalOperator.Or);
                            _filterlines2.AddFilter(_filterlines1);
                            _filterlines2.AddCondition("tss_aging", ConditionOperator.GreaterThan, 0);

                            _filterexpression = new FilterExpression(LogicalOperator.And);
                            _filterexpression.AddCondition("tss_mastermarketsizeref", ConditionOperator.In, _mastermarketsizeids);
                            _filterexpression.AddFilter(_filterlines2);

                            _queryexpression = new QueryExpression(_DL_tss_mastermarketsizelines.EntityName);
                            _queryexpression.Criteria.AddFilter(_filterexpression);
                            _queryexpression.ColumnSet = new ColumnSet(true);

                            List<Entity> _mastermarketsizelinescollection = _retrievehelper.RetrieveMultiple(organizationService, _queryexpression);

                            foreach (var item in _mastermarketsizelinescollection)
                            {
                                Entity _line = item;
                                Entity _header = organizationService.Retrieve("tss_mastermarketsize", item.GetAttributeValue<EntityReference>("tss_mastermarketsizeref").Id, new ColumnSet(true));
                                Entity _population = organizationService.Retrieve("new_population", _header.GetAttributeValue<EntityReference>("tss_serialnumber").Id, new ColumnSet(true));

                                int _methodcalculationused = _line.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value;
                                int _firsthmpm = _mastermarketsizelinescollection[0].Attributes.Contains("tss_hmpm") ? _mastermarketsizelinescollection[0].GetAttributeValue<int>("tss_hmpm") : 0;
                                int _lasthmpm = _mastermarketsizelinescollection[_mastermarketsizelinescollection.Count() - 1].Attributes.Contains("tss_hmpm") ? _mastermarketsizelinescollection[_mastermarketsizelinescollection.Count() - 1].GetAttributeValue<int>("tss_hmpm") : 0;

                                Entity setup = setups.Entities[0];

                                if (_methodcalculationused == MTD1 || _methodcalculationused == MTD2 || _methodcalculationused == MTD3)
                                    GenerateSublinesUIOv2(organizationService, _population, setup, _line, _header.GetAttributeValue<bool>("tss_usetyre"), _firsthmpm, _lasthmpm, _header);
                                else if (_methodcalculationused == MTD4 || _methodcalculationused == MTD5)
                                    GenerateSublinesUIOv2(organizationService, _population, setup, _line, _header.GetAttributeValue<bool>("tss_usetyre"), 0, 0, _header);
                            }

                            foreach (var _mastermarketsize in _mastermarketsizecollection)
                            {
                                Entity _entitytoupdate = new Entity(_mastermarketsize.LogicalName, _mastermarketsize.Id);

                                if (_entitytoupdate.Attributes.Contains("tss_issublinesgenerated"))
                                    _entitytoupdate["tss_issublinesgenerated"] = true;
                                else
                                    _entitytoupdate.Attributes.Add("tss_issublinesgenerated", true);

                                organizationService.Update(_entitytoupdate);
                            }
                        }
                    }
                }
                else
                    throw new InvalidWorkflowException("Spare Part Setup NOT found !");
            }
            catch (Exception e)
            {
                throw new InvalidWorkflowException(e.Message.ToString());
            }
        }

        public void GenerateSublinesUIOv2(IOrganizationService organizationService, Entity _population, Entity _setup, Entity _mastermarketsizelines, bool _usertyre, int _firsthmpm, int _lasthmpm, Entity _mastermarketsize)
        {
            try
            {
                FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
                QueryExpression _queryexpression = new QueryExpression();

                if (_mastermarketsizelines.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD1 || _mastermarketsizelines.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD2 || _mastermarketsizelines.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD3)
                {
                    _filterexpression.AddCondition("tss_model", ConditionOperator.Equal, _population.GetAttributeValue<EntityReference>("trs_productmaster").Id);
                    _filterexpression.AddCondition("tss_typepm", ConditionOperator.Equal, _mastermarketsizelines.GetAttributeValue<string>("tss_pm"));

                    _queryexpression = new QueryExpression(_DL_tss_marketsizepartconsump.EntityName);
                    _queryexpression.LinkEntities.Add(new LinkEntity(_DL_tss_marketsizepartconsump.EntityName, _DL_trs_masterpart.EntityName, "tss_partnumber", "trs_masterpartid", JoinOperator.Inner));
                    _queryexpression.LinkEntities.Add(new LinkEntity(_DL_tss_marketsizepartconsump.EntityName, _DL_tss_partmasterlinesmodel.EntityName, "tss_partnumber", "tss_partmasterid", JoinOperator.Inner));
                }
                else if (_mastermarketsizelines.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD4 || _mastermarketsizelines.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD5)
                {
                    DateTime _deliverydate = _population.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime();
                    DateTime _today = DateTime.Now;
                    DateTime _msperiodstart = _mastermarketsize.GetAttributeValue<DateTime>("tss_msperiodstart").ToLocalTime();
                    DateTime _msperiodend = _mastermarketsize.GetAttributeValue<DateTime>("tss_msperiodend").ToLocalTime();

                    CalculateDate _calculate = new CalculateDate();
                    int _agingmsperiod = _calculate.DiffYear(_msperiodstart, _msperiodend);
                    int _agingdeliverydate = _calculate.DiffYear(_deliverydate, _today);
                    int _aging = 0;

                    _aging = _agingmsperiod + _agingdeliverydate;

                    _filterexpression.AddCondition("tss_model", ConditionOperator.Equal, _population.GetAttributeValue<EntityReference>("trs_productmaster").Id);

                    FilterExpression _frange = new FilterExpression(LogicalOperator.And);
                    _frange.AddCondition("tss_rangeagingfrom", ConditionOperator.LessEqual, _aging);
                    _frange.AddCondition("tss_rangeagingto", ConditionOperator.GreaterThan, _aging);

                    FilterExpression _faging = new FilterExpression(LogicalOperator.Or);
                    _faging.AddFilter(_frange);
                    _faging.AddCondition("tss_aging", ConditionOperator.Equal, _aging);

                    _filterexpression.AddFilter(_faging);

                    _queryexpression = new QueryExpression("tss_marketsizepartconsumpaging");
                    _queryexpression.LinkEntities.Add(new LinkEntity("tss_marketsizepartconsumpaging", _DL_trs_masterpart.EntityName, "tss_partnumber", "trs_masterpartid", JoinOperator.Inner));
                    _queryexpression.LinkEntities.Add(new LinkEntity("tss_marketsizepartconsumpaging", _DL_tss_partmasterlinesmodel.EntityName, "tss_partnumber", "tss_partmasterid", JoinOperator.Inner));
                }

                _queryexpression.LinkEntities[0].Columns.AddColumns("tss_commoditytype");
                _queryexpression.LinkEntities[0].EntityAlias = "trs_masterpart";
                _queryexpression.LinkEntities[1].Columns.AddColumn("tss_model");
                _queryexpression.LinkEntities[1].LinkCriteria.AddCondition("tss_model", ConditionOperator.Equal, _population.GetAttributeValue<EntityReference>("trs_productmaster").Id);
                _queryexpression.LinkEntities[1].EntityAlias = "tss_partmasterlinesmodel";
                _queryexpression.Criteria.AddFilter(_filterexpression);
                _queryexpression.Orders.Add(new OrderExpression("tss_modelname", OrderType.Ascending));
                _queryexpression.ColumnSet = new ColumnSet(true);

                EntityCollection _marketsizepartcollection = organizationService.RetrieveMultiple(_queryexpression);

                if (_marketsizepartcollection.Entities.Count > 0)
                {
                    foreach (var _marketsizepart in _marketsizepartcollection.Entities)
                    {
                        int _quantity = _marketsizepart.Attributes.Contains("tss_qty") ? _marketsizepart.GetAttributeValue<int>("tss_qty") : 0;
                        decimal _price = 0m;
                        decimal _minimumprice = 0m;

                        if (_mastermarketsizelines.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD1 || _mastermarketsizelines.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD2 || _mastermarketsizelines.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD3)
                        {
                            #region CHECK IF BATTERY
                            if (_marketsizepart.Contains("trs_masterpart.tss_commoditytype") && ((OptionSetValue)_marketsizepart.GetAttributeValue<AliasedValue>("trs_masterpart.tss_commoditytype").Value).Value == BATTERY_TYPE)
                            {
                                _queryexpression = new QueryExpression(_DL_tss_partmasterlinesbattery.EntityName);
                                _queryexpression.LinkEntities.Add(new LinkEntity(_DL_tss_partmasterlinesbattery.EntityName, _DL_tss_partmasterlinesbatterytypeconsump.EntityName, "tss_partmasterlinesbatteryid", "tss_refpartmasterlinesbattery", JoinOperator.Inner));
                                _queryexpression.LinkEntities[0].Columns.AddColumns("tss_nextconsump", "tss_partmasterlinesbatterytypeconsumpid");
                                _queryexpression.LinkEntities[0].EntityAlias = "tss_partmasterlinesbatterytypeconsump";
                                _queryexpression.Criteria.AddCondition("tss_partmasterid", ConditionOperator.Equal, _marketsizepart.GetAttributeValue<EntityReference>("tss_partnumber").Id);
                                _queryexpression.ColumnSet = new ColumnSet(true);
                                EntityCollection _partmasterlinesbattery = _DL_tss_partmasterlinesbattery.Select(organizationService, _queryexpression);

                                if (_partmasterlinesbattery.Entities.Count > 0)
                                {
                                    if (_setup.Contains("tss_startdatems") && _setup.Contains("tss_enddatems") && _partmasterlinesbattery[0].Contains("tss_partmasterlinesbatterytypeconsump.tss_nextconsump") &&
                                       (DateTime)_partmasterlinesbattery[0].GetAttributeValue<AliasedValue>("tss_partmasterlinesbatterytypeconsump.tss_nextconsump").Value >= _setup.GetAttributeValue<DateTime>("tss_startdatems") &&
                                       (DateTime)_partmasterlinesbattery[0].GetAttributeValue<AliasedValue>("tss_partmasterlinesbatterytypeconsump.tss_nextconsump").Value <= _setup.GetAttributeValue<DateTime>("tss_enddatems"))
                                    {
                                        _DL_tss_partmasterlinesbatterytypeconsump = new DL_tss_partmasterlinesbatterytypeconsump();
                                        _DL_tss_partmasterlinesbatterytypeconsump.tss_lastconsump = _mastermarketsizelines.GetAttributeValue<DateTime>("tss_estimatedpmdate");
                                        _DL_tss_partmasterlinesbatterytypeconsump.tss_nextconsump = _DL_tss_partmasterlinesbatterytypeconsump.tss_lastconsump.AddDays(_partmasterlinesbattery[0].GetAttributeValue<int>("tss_cycleconsump"));
                                        _DL_tss_partmasterlinesbatterytypeconsump.Update(organizationService, (Guid)_partmasterlinesbattery[0].GetAttributeValue<AliasedValue>("tss_partmasterlinesbatterytypeconsump.tss_partmasterlinesbatterytypeconsumpid").Value);
                                    }
                                    else
                                        continue;
                                }
                                else
                                    continue;
                            }
                            #endregion CHECK IF BATTERY

                            #region CHECK IF USE TYRE
                            if (_marketsizepart.Contains("tss_tyrecode") && _usertyre)
                            {
                                _filterexpression = new FilterExpression(LogicalOperator.Or);
                                _filterexpression.AddCondition("tss_tyrefrontcode", ConditionOperator.Equal, _marketsizepart.GetAttributeValue<EntityReference>("tss_tyrecode").Id);
                                _filterexpression.AddCondition("tss_tyrerearcode", ConditionOperator.Equal, _marketsizepart.GetAttributeValue<EntityReference>("tss_tyrecode").Id);

                                _queryexpression = new QueryExpression("trs_functionallocation");
                                _queryexpression.Criteria.AddCondition("trs_functionallocationid", ConditionOperator.Equal, _population.GetAttributeValue<EntityReference>("trs_functionallocation").Id);
                                _queryexpression.Criteria.AddFilter(_filterexpression);
                                _queryexpression.ColumnSet.AddColumns("tss_tyrefrontcode", "tss_tyrerearcode");
                                EntityCollection _functionallocationcollection = organizationService.RetrieveMultiple(_queryexpression);

                                if (_functionallocationcollection.Entities.Count > 0)
                                {
                                    Guid _code = _functionallocationcollection.Entities[0].Contains("tss_tyrefrontcode") && _functionallocationcollection.Entities[0].GetAttributeValue<EntityReference>("tss_tyrefrontcode").Id == _marketsizepart.GetAttributeValue<EntityReference>("tss_tyrecode").Id ?
                                                _functionallocationcollection.Entities[0].GetAttributeValue<EntityReference>("tss_tyrefrontcode").Id :
                                                _functionallocationcollection.Entities[0].Contains("tss_tyrerearcode") && _functionallocationcollection.Entities[0].GetAttributeValue<EntityReference>("tss_tyrerearcode").Id == _marketsizepart.GetAttributeValue<EntityReference>("tss_tyrecode").Id ?
                                                _functionallocationcollection.Entities[0].GetAttributeValue<EntityReference>("tss_tyrerearcode").Id : Guid.Empty;

                                    if (_code != Guid.Empty)
                                    {
                                        _queryexpression = new QueryExpression(_DL_tss_matrixtyreconsump.EntityName);
                                        _queryexpression.Criteria.AddCondition("tss_matrixtyreconsumpid", ConditionOperator.Equal, _code);
                                        _queryexpression.ColumnSet.AddColumn("tss_hmplanning");
                                        EntityCollection _matrixtyreconsumpcollection = _DL_tss_matrixtyreconsump.Select(organizationService, _queryexpression);

                                        if (_matrixtyreconsumpcollection.Entities.Count > 0)
                                            _quantity = (int)Math.Round(((decimal)_lasthmpm - (decimal)_firsthmpm) / (decimal)_matrixtyreconsumpcollection.Entities[0].GetAttributeValue<int>("tss_hmplanning"), 0);
                                    }
                                    else
                                        continue;
                                }
                            }
                            #endregion CHECK IF USE TYRE
                        }

                        _queryexpression = new QueryExpression(_DL_tss_sparepartpricemaster.EntityName);
                        _queryexpression.LinkEntities.Add(new LinkEntity(_DL_tss_sparepartpricemaster.EntityName, _DL_tss_pricelistpart.EntityName, "tss_pricelistpart", "tss_pricelistpartid", JoinOperator.Inner));
                        _queryexpression.Criteria.AddCondition("tss_partmaster", ConditionOperator.Equal, _marketsizepart.GetAttributeValue<EntityReference>("tss_partnumber").Id);
                        _queryexpression.LinkEntities[0].LinkCriteria.AddCondition("tss_type", ConditionOperator.Equal, MS_TYPE);
                        _queryexpression.LinkEntities[0].EntityAlias = "tss_pricelistpart";
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        EntityCollection _sparepartpricemaster = _DL_tss_sparepartpricemaster.Select(organizationService, _queryexpression);

                        if (_sparepartpricemaster.Entities.Count > 0)
                        {
                            _price = _sparepartpricemaster.Entities[0].Contains("tss_price") ? _sparepartpricemaster.Entities[0].GetAttributeValue<Money>("tss_price").Value : 0m;
                            _minimumprice = _sparepartpricemaster.Entities[0].Contains("tss_minimumprice") ? _sparepartpricemaster.Entities[0].GetAttributeValue<Money>("tss_minimumprice").Value : 0m;
                        }


                        _DL_tss_mastermarketsizesublines = new DL_tss_mastermarketsizesublines();
                        _DL_tss_mastermarketsizesublines.tss_mastermslinesref = _mastermarketsizelines.Id;
                        _DL_tss_mastermarketsizesublines.tss_partnumber = _marketsizepart.GetAttributeValue<EntityReference>("tss_partnumber").Id;
                        _DL_tss_mastermarketsizesublines.tss_partdescription = _marketsizepart.GetAttributeValue<string>("trs_partdescription");
                        _DL_tss_mastermarketsizesublines.tss_qty = _quantity;
                        _DL_tss_mastermarketsizesublines.tss_originalqty = _quantity;
                        _DL_tss_mastermarketsizesublines.tss_price = _price;
                        _DL_tss_mastermarketsizesublines.tss_minimumprice = _minimumprice;

                        _DL_tss_mastermarketsizesublines.Insert(organizationService);

                        //tss_mastermarketsizesublines _mastermarketsizesublines = new tss_mastermarketsizesublines();

                        //_mastermarketsizesublines.tss_MasterMSLinesRef = new EntityReference(_mastermarketsizelines.LogicalName, _mastermarketsizelines.Id);
                        //_mastermarketsizesublines.tss_PartNumber = new EntityReference("trs_masterpart", _marketsizepart.GetAttributeValue<EntityReference>("tss_partnumber").Id);
                        //_mastermarketsizesublines.tss_partdescription = _marketsizepart.GetAttributeValue<AliasedValue>("trs_masterpart.trs_partdescription").Value.ToString();
                        //_mastermarketsizesublines.tss_Qty = _quantity;
                        //_mastermarketsizesublines.tss_originalqty = _quantity;
                        //_mastermarketsizesublines.tss_Price = new Money(_price);
                        //_mastermarketsizesublines.tss_MinimumPrice = new Money(_minimumprice);

                        //organizationService.Create(_mastermarketsizesublines);
                    }
                }
            }
            catch (Exception e)
            {
                throw new InvalidWorkflowException(e.Message);
            }
        }

        //public void GenerateMasterMSLines(IOrganizationService organizationService, DL_tss_mastermarketsize _mastermarketsize, Entity _population, Entity _setup)
        //{
        //    //List<DL_tss_mastermarketsizelines> _mastermarketsizelineslist = new List<DL_tss_mastermarketsizelines>();

        //    decimal _populationcurrenthm;
        //    decimal _methodvalue = 0;
        //    int _calculatemethod = 0;
        //    int _interval = 0;
        //    int _duedatems = 0;
        //    int _hmpm;
        //    int _hmconsumppm;
        //    int _forecastpmdate;
        //    int _pmperiod = 0;
        //    DateTime _populationcurrentdate;
        //    DateTime _pmdate;
        //    DateTime _nextpmdate;

        //    QueryExpression _queryexpression = new QueryExpression("tss_unitgroupmarketsize");
        //    _queryexpression.Criteria.AddCondition("tss_model", ConditionOperator.Equal, _population.GetAttributeValue<EntityReference>("trs_productmaster").Id);
        //    _queryexpression.ColumnSet.AddColumn("tss_maintenanceinterval");
        //    EntityCollection _unitgroupmarketsizecollection = organizationService.RetrieveMultiple(_queryexpression);

        //    _populationcurrenthm = Math.Round(_population.GetAttributeValue<decimal>("tss_mscurrenthourmeter"), 0);
        //    _populationcurrentdate = _population.Attributes.Contains("tss_mscurrenthourmeterdate") ? _population.GetAttributeValue<DateTime>("tss_mscurrenthourmeterdate").ToLocalTime().Date : DateTime.MinValue;

        //    if (_unitgroupmarketsizecollection.Entities.Count > 0)
        //    {
        //        Entity _unitgroupmarketsize = _unitgroupmarketsizecollection.Entities[0];

        //        _interval = _unitgroupmarketsize.GetAttributeValue<int>("tss_maintenanceinterval");
        //        _duedatems = _setup.GetAttributeValue<int>("tss_duedatems");

        //        if (_mastermarketsize._tss_avghmmethod1)
        //        {
        //            _methodvalue = _mastermarketsize.tss_avghmmethod1;
        //            _calculatemethod = MTD1;
        //        }
        //        else if (_mastermarketsize._tss_avghmmethod2)
        //        {
        //            _methodvalue = _mastermarketsize.tss_avghmmethod2;
        //            _calculatemethod = MTD2;
        //        }
        //        else if (_mastermarketsize._tss_avghmmethod3)
        //        {
        //            _methodvalue = _mastermarketsize.tss_avghmmethod3;
        //            _calculatemethod = MTD3;
        //        }
        //        else if (_mastermarketsize._tss_periodpmmethod4)
        //        {
        //            _methodvalue = _mastermarketsize.tss_periodpmmethod4;
        //            _calculatemethod = MTD4;
        //        }
        //        else if (_mastermarketsize._tss_periodpmmethod5)
        //        {
        //            _methodvalue = _mastermarketsize.tss_periodpmmethod5;
        //            _calculatemethod = MTD5;
        //        }

        //        if (_calculatemethod == MTD1 || _calculatemethod == MTD2 || _calculatemethod == MTD3)
        //        {
        //            _hmpm = (int)Math.Round(_populationcurrenthm, 0);
        //            _pmdate = _populationcurrentdate;

        //            if (_populationcurrentdate != DateTime.MinValue && _interval > 0 && _methodvalue > 0)
        //            {
        //                _nextpmdate = _pmdate.AddDays((int)Math.Round((((Math.Round((decimal)_hmpm / _interval, 0) * _interval) + _interval) - _hmpm) / _methodvalue, 0));

        //                while (_pmdate <= _setup.GetAttributeValue<DateTime>("tss_enddatems").ToLocalTime().Date && _nextpmdate <= _setup.GetAttributeValue<DateTime>("tss_enddatems"))
        //                {
        //                    _DL_tss_mastermarketsizelines = new DL_tss_mastermarketsizelines();

        //                    if (_DL_tss_mastermarketsizelines.tss_estimatedpmdate.ToLocalTime().Date >= _setup.GetAttributeValue<DateTime>("tss_startdatems").ToLocalTime().Date)
        //                        _pmperiod += 1;

        //                    _DL_tss_mastermarketsizelines.tss_mastermarketsizeref = _mastermarketsize.tss_mastermarketsizeid;
        //                    _DL_tss_mastermarketsizelines.tss_hmpm = (int)(Math.Round((decimal)_hmpm / _interval, 0) * _interval) + _interval;
        //                    _DL_tss_mastermarketsizelines.tss_hmconsumppm = _DL_tss_mastermarketsizelines.tss_hmpm - _hmpm;
        //                    _DL_tss_mastermarketsizelines.tss_forecastpmdate = (int)Math.Round(_DL_tss_mastermarketsizelines.tss_hmconsumppm / _methodvalue, 0);
        //                    _DL_tss_mastermarketsizelines.tss_estimatedpmdate = _pmdate.AddDays(_DL_tss_mastermarketsizelines.tss_forecastpmdate);
        //                    _DL_tss_mastermarketsizelines.tss_pm = _DL_tss_mastermarketsizelines.GetPMType(organizationService, _population.GetAttributeValue<EntityReference>("trs_productmaster").Id, _DL_tss_mastermarketsizelines.tss_hmpm).ToString();
        //                    _DL_tss_mastermarketsizelines.tss_pmperiod = _pmperiod;
        //                    _DL_tss_mastermarketsizelines.tss_methodcalculationused = _calculatemethod;
        //                    //_DL_tss_mastermarketsizelines.tss_aging = (int)Math.Floor((delivDt - endMSDt).TotalDays / 365); TBD
        //                    //_DL_tss_mastermarketsizelines.tss_annualaging TBD
        //                    _DL_tss_mastermarketsizelines.tss_status = STATUS_LINES_DRAFT;
        //                    _DL_tss_mastermarketsizelines.tss_statusreason = STATUS_REASON_LINES_OPEN;

        //                    _hmpm = _DL_tss_mastermarketsizelines.tss_hmpm;
        //                    _hmconsumppm = _DL_tss_mastermarketsizelines.tss_hmconsumppm;
        //                    _forecastpmdate = _DL_tss_mastermarketsizelines.tss_forecastpmdate;
        //                    _pmdate = _DL_tss_mastermarketsizelines.tss_estimatedpmdate;
        //                    _nextpmdate = _pmdate.AddDays((int)Math.Round((((Math.Round((decimal)_hmpm / _interval, 0) * _interval) + _interval) - _hmpm) / _methodvalue, 0));

        //                    //DUE DATE = NEXT PM DATE - DUE DATE MARKET SIZE
        //                    if (_pmperiod > 0 && _nextpmdate <= _setup.GetAttributeValue<DateTime>("tss_enddatems").ToLocalTime().Date)
        //                        _DL_tss_mastermarketsizelines.tss_duedate = _nextpmdate.AddDays(-_duedatems);

        //                    _DL_tss_mastermarketsizelines.Insert(organizationService);
        //                    //_mastermarketsizelineslist.Add(_DL_tss_mastermarketsizelines);
        //                }

        //                // 2019.03.22 - GENERATE SUBLINES
        //                //foreach (var lines in listLines)
        //                //{
        //                //    #region Generate Sub Lines
        //                //    GenerateMasterMSSubLines(organizationService, population, setup, lines, ms.tss_usetyre, listLines[0].tss_hmpm, listLines[listLines.Count - 1].tss_hmpm, ms);
        //                //    #endregion
        //                //}
        //            }
        //        }
        //        else if (_calculatemethod == MTD4 || _calculatemethod == MTD5)
        //        {
        //            DateTime _deliverydate = _population.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime();
        //            DateTime _today = DateTime.Now;
        //            DateTime _msperiodstart = _mastermarketsize.tss_msperiodstart.ToLocalTime();
        //            DateTime _msperiodend = _mastermarketsize.tss_msperiodend.ToLocalTime();

        //            DL_tss_mastermarketsizelines _mastermarketsizelines = new DL_tss_mastermarketsizelines();
        //            _mastermarketsizelines.tss_mastermarketsizeref = new EntityReference("tss_mastermarketsize", _mastermarketsize.tss_mastermarketsizeid).Id;
        //            _mastermarketsizelines.tss_methodcalculationused = _calculatemethod;
        //            _mastermarketsizelines.tss_status = STATUS_LINES_DRAFT;
        //            _mastermarketsizelines.tss_statusreason = STATUS_REASON_LINES_OPEN;

        //            //CALCULATE AGING
        //            CalculateDate _calculate = new CalculateDate();

        //            int _aging = 0;
        //            int _agingmsperiod = _calculate.DiffYear(_msperiodstart, _msperiodend);
        //            int _agingdeliverydate = _calculate.DiffYear(_deliverydate, _today);

        //            _aging = _agingmsperiod + _agingdeliverydate;

        //            _mastermarketsizelines.tss_aging = Convert.ToInt32(_aging);
        //            _mastermarketsizelines.Insert(organizationService);

        //            // 2019.03.22 - GENERATE SUBLINES
        //            //GenerateMasterMSSubLines(organizationService, population, setup, newLine, ms.tss_usetyre, 0, 0, ms);
        //        }

        //    }
        //}

        //public void GenerateMasterMSSublines(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context, EntityCollection _keyaccountcollection)
        //{
        //    try
        //    {
        //        //FilterExpression fStatus = new FilterExpression(LogicalOperator.Or);
        //        ////fStatus.AddCondition("tss_status", ConditionOperator.Equal, STATUS_OPEN);
        //        ////fStatus.AddCondition("tss_status", ConditionOperator.Equal, STATUS_ERROR);
        //        //fStatus.AddCondition("tss_status", ConditionOperator.Equal, STATUS_CALCULATE);

        //        //FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
        //        //_filterexpression.AddFilter(fStatus);
        //        //_filterexpression.AddCondition("tss_pss", ConditionOperator.Equal, context.UserId);
        //        //_filterexpression.AddCondition("tss_customer", ConditionOperator.NotNull);
        //        //_filterexpression.AddCondition("tss_activeenddate", ConditionOperator.GreaterEqual, DateTime.Today);
        //        //_filterexpression.AddCondition("tss_activestartdate", ConditionOperator.LessEqual, DateTime.Today);
        //        //_filterexpression.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true); //2018.10.12

        //        //QueryExpression _queryexpression = new QueryExpression(_DL_tss_keyaccount.EntityName);
        //        //_queryexpression.Criteria.AddFilter(_filterexpression);
        //        //_queryexpression.ColumnSet = new ColumnSet(true);
        //        //_keyaccountcollection = _retrievehelper.RetrieveMultiple(organizationService, _queryexpression);

        //        foreach (var _keyaccountitem in _keyaccountcollection.Entities)
        //        {
        //            Entity _keyaccount = _keyaccountitem;

        //            FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
        //            _filterexpression.AddCondition("tss_unittype", ConditionOperator.NotNull); // 2019.03.21
        //            _filterexpression.AddCondition("tss_unittype", ConditionOperator.NotEqual, UNITTYPE_COMMODITY);
        //            _filterexpression.AddCondition("tss_pss", ConditionOperator.Equal, context.UserId);
        //            _filterexpression.AddCondition("tss_status", ConditionOperator.Equal, STATUS_COMPLETED_MS);
        //            _filterexpression.AddCondition("tss_keyaccountid", ConditionOperator.Equal, _keyaccount.Id);
        //            _filterexpression.AddCondition("tss_issublinesgenerated", ConditionOperator.Equal, false);

        //            QueryExpression _queryexpression = new QueryExpression(_DL_tss_mastermarketsize.EntityName);
        //            _queryexpression.Criteria.AddFilter(_filterexpression);
        //            _queryexpression.ColumnSet = new ColumnSet(true);
        //            List<Entity> _mastermarketsizecollection = _retrievehelper.RetrieveMultiple(organizationService, _queryexpression);

        //            if (_mastermarketsizecollection.Count > 0)
        //            {
        //                object[] _mastermarketsizeids = _mastermarketsizecollection.Select(x => (object)x.Id).ToArray();

        //                FilterExpression _filterlines1 = new FilterExpression(LogicalOperator.And);
        //                _filterlines1.AddCondition("tss_pmperiod", ConditionOperator.GreaterThan, 0);
        //                _filterlines1.AddCondition("tss_duedate", ConditionOperator.NotNull);

        //                FilterExpression _filterlines2 = new FilterExpression(LogicalOperator.Or);
        //                _filterlines2.AddFilter(_filterlines1);
        //                _filterlines2.AddCondition("tss_aging", ConditionOperator.GreaterThan, 0);

        //                _filterexpression = new FilterExpression(LogicalOperator.And);
        //                _filterexpression.AddCondition("tss_mastermarketsizeref", ConditionOperator.In, _mastermarketsizeids);
        //                _filterexpression.AddFilter(_filterlines2);

        //                _queryexpression = new QueryExpression(_DL_tss_mastermarketsizelines.EntityName);
        //                _queryexpression.Criteria.AddFilter(_filterexpression);
        //                _queryexpression.ColumnSet = new ColumnSet(true);
        //                List<Entity> _mastermarketsizelinescollection = _retrievehelper.RetrieveMultiple(organizationService, _queryexpression);

        //                foreach (var item in _mastermarketsizelinescollection)
        //                {
        //                    Entity _line = item;
        //                    Entity _header = organizationService.Retrieve("tss_mastermarketsize", item.GetAttributeValue<EntityReference>("tss_mastermarketsizeref").Id, new ColumnSet(true));
        //                    Entity _population = organizationService.Retrieve("new_population", _header.GetAttributeValue<EntityReference>("tss_serialnumber").Id, new ColumnSet(true));
        //                    //Entity _population = organizationService.Retrieve("new_population", _header.tss_serialnumber, new ColumnSet(true));

        //                    int _methodcalculationused = _line.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value;
        //                    int _firsthmpm = _mastermarketsizelinescollection[0].Attributes.Contains("tss_hmpm") ? _mastermarketsizelinescollection[0].GetAttributeValue<int>("tss_hmpm") : 0;
        //                    int _lasthmpm = _mastermarketsizelinescollection[_mastermarketsizelinescollection.Count() - 1].Attributes.Contains("tss_hmpm") ? _mastermarketsizelinescollection[_mastermarketsizelinescollection.Count() - 1].GetAttributeValue<int>("tss_hmpm") : 0;

        //                    _queryexpression = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
        //                    _queryexpression.ColumnSet = new ColumnSet(true);
        //                    EntityCollection setups = _DL_tss_sparepartsetup.Select(organizationService, _queryexpression);

        //                    if (setups.Entities.Count > 0)
        //                    {
        //                        Entity setup = setups.Entities[0];

        //                        if (_methodcalculationused == MTD1 || _methodcalculationused == MTD2 || _methodcalculationused == MTD3)
        //                            GenerateSublinesUIO(organizationService, _population, setup, _line, _header.GetAttributeValue<bool>("tss_usetyre"), _firsthmpm, _lasthmpm, _header);
        //                        else if (_methodcalculationused == MTD4 || _methodcalculationused == MTD5)
        //                            GenerateSublinesUIO(organizationService, _population, setup, _line, _header.GetAttributeValue<bool>("tss_usetyre"), 0, 0, _header);
        //                    }
        //                }

        //                foreach (var _mastermarketsize in _mastermarketsizecollection)
        //                {
        //                    Entity _entitytoupdate = new Entity(_mastermarketsize.LogicalName, _mastermarketsize.Id);

        //                    if (_entitytoupdate.Attributes.Contains("tss_issublinesgenerated"))
        //                        _entitytoupdate["tss_issublinesgenerated"] = true;
        //                    else
        //                        _entitytoupdate.Attributes.Add("tss_issublinesgenerated", true);


        //                    organizationService.Update(_entitytoupdate);
        //                }

        //                //try
        //                //{
        //                //    ExecuteMultipleRequest requestWithResults = new ExecuteMultipleRequest()
        //                //    {
        //                //        // Assign settings that define execution behavior: continue on error, return responses. 
        //                //        Settings = new ExecuteMultipleSettings()
        //                //        {
        //                //            ContinueOnError = false,
        //                //            ReturnResponses = true
        //                //        },
        //                //        // Create an empty organization request collection.
        //                //        Requests = new OrganizationRequestCollection()
        //                //    };

        //                //    // Add a CreateRequest for each entity to the request collection.
        //                //    foreach (var entity in _mastermarketsizecollection)
        //                //    {
        //                //        Entity _entitytoupdate = new Entity(entity.LogicalName, entity.Id);
        //                //        _entitytoupdate["tss_issublinesgenerated"] = true;

        //                //        UpdateRequest updateRequest = new UpdateRequest { Target = _entitytoupdate };
        //                //        requestWithResults.Requests.Add(updateRequest);
        //                //    }

        //                //    // Execute all the requests in the request collection using a single web method call.
        //                //    ExecuteMultipleResponse responseWithResults =
        //                //        (ExecuteMultipleResponse)organizationService.Execute(requestWithResults);

        //                //    if (responseWithResults.IsFaulted)
        //                //    {
        //                //        throw new InvalidWorkflowException("Update Master Market Size FAILED !");
        //                //    }
        //                //}
        //                //catch (Exception e)
        //                //{
        //                //    throw new InvalidWorkflowException(e.Message.ToString());
        //                //}
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new InvalidWorkflowException(e.Message.ToString());
        //    }
        //}

        //public void GenerateSublinesUIO(IOrganizationService organizationService, Entity _population, Entity _setup, Entity _mastermarketsizelines, bool _usertyre, int _firsthmpm, int _lasthmpm, Entity _mastermarketsize)
        //{
        //    try
        //    {
        //        FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
        //        QueryExpression _queryexpression = new QueryExpression();

        //        if (_mastermarketsizelines.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD1 || _mastermarketsizelines.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD2 || _mastermarketsizelines.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD3)
        //        {
        //            _filterexpression.AddCondition("tss_model", ConditionOperator.Equal, _population.GetAttributeValue<EntityReference>("trs_productmaster").Id);
        //            _filterexpression.AddCondition("tss_typepm", ConditionOperator.Equal, _mastermarketsizelines.GetAttributeValue<string>("tss_pm"));

        //            _queryexpression = new QueryExpression(_DL_tss_marketsizepartconsump.EntityName);
        //            _queryexpression.LinkEntities.Add(new LinkEntity(_DL_tss_marketsizepartconsump.EntityName, _DL_trs_masterpart.EntityName, "tss_partnumber", "trs_masterpartid", JoinOperator.Inner));
        //            _queryexpression.LinkEntities.Add(new LinkEntity(_DL_tss_marketsizepartconsump.EntityName, _DL_tss_partmasterlinesmodel.EntityName, "tss_partnumber", "tss_partmasterid", JoinOperator.Inner));
        //        }
        //        else if (_mastermarketsizelines.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD4 || _mastermarketsizelines.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD5)
        //        {
        //            DateTime _deliverydate = _population.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime();
        //            DateTime _today = DateTime.Now;
        //            DateTime _msperiodstart = _mastermarketsize.GetAttributeValue<DateTime>("tss_msperiodstart").ToLocalTime();
        //            DateTime _msperiodend = _mastermarketsize.GetAttributeValue<DateTime>("tss_msperiodend").ToLocalTime();

        //            CalculateDate _calculate = new CalculateDate();
        //            int _agingmsperiod = _calculate.DiffYear(_msperiodstart, _msperiodend);
        //            int _agingdeliverydate = _calculate.DiffYear(_deliverydate, _today);
        //            int _aging = 0;

        //            _aging = _agingmsperiod + _agingdeliverydate;

        //            _filterexpression.AddCondition("tss_model", ConditionOperator.Equal, _population.GetAttributeValue<EntityReference>("trs_productmaster").Id);

        //            FilterExpression _frange = new FilterExpression(LogicalOperator.And);
        //            _frange.AddCondition("tss_rangeagingfrom", ConditionOperator.LessEqual, _aging);
        //            _frange.AddCondition("tss_rangeagingto", ConditionOperator.GreaterThan, _aging);

        //            FilterExpression _faging = new FilterExpression(LogicalOperator.Or);
        //            _faging.AddFilter(_frange);
        //            _faging.AddCondition("tss_aging", ConditionOperator.Equal, _aging);

        //            _filterexpression.AddFilter(_faging);

        //            _queryexpression = new QueryExpression("tss_marketsizepartconsumpaging");
        //            _queryexpression.LinkEntities.Add(new LinkEntity("tss_marketsizepartconsumpaging", _DL_trs_masterpart.EntityName, "tss_partnumber", "trs_masterpartid", JoinOperator.Inner));
        //            _queryexpression.LinkEntities.Add(new LinkEntity("tss_marketsizepartconsumpaging", _DL_tss_partmasterlinesmodel.EntityName, "tss_partnumber", "tss_partmasterid", JoinOperator.Inner));
        //        }

        //        _queryexpression.LinkEntities[0].Columns.AddColumns("tss_commoditytype");
        //        _queryexpression.LinkEntities[0].EntityAlias = "trs_masterpart";
        //        _queryexpression.LinkEntities[1].Columns.AddColumn("tss_model");
        //        _queryexpression.LinkEntities[1].LinkCriteria.AddCondition("tss_model", ConditionOperator.Equal, _population.GetAttributeValue<EntityReference>("trs_productmaster").Id);
        //        _queryexpression.LinkEntities[1].EntityAlias = "tss_partmasterlinesmodel";
        //        _queryexpression.Criteria.AddFilter(_filterexpression);
        //        _queryexpression.Orders.Add(new OrderExpression("tss_modelname", Microsoft.Xrm.Sdk.Query.OrderType.Ascending));
        //        _queryexpression.ColumnSet = new ColumnSet(true);

        //        EntityCollection _marketsizepartcollection = organizationService.RetrieveMultiple(_queryexpression);

        //        if (_marketsizepartcollection.Entities.Count > 0)
        //        {
        //            foreach (var _marketsizepart in _marketsizepartcollection.Entities)
        //            {
        //                int _quantity = _marketsizepart.Attributes.Contains("tss_qty") ? _marketsizepart.GetAttributeValue<int>("tss_qty") : 0;
        //                decimal _price = 0m;
        //                decimal _minimumprice = 0m;

        //                if (_mastermarketsizelines.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD1 || _mastermarketsizelines.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD2 || _mastermarketsizelines.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD3)
        //                {
        //                    #region CHECK IF BATTERY
        //                    if (_marketsizepart.Contains("trs_masterpart.tss_commoditytype") && ((OptionSetValue)_marketsizepart.GetAttributeValue<AliasedValue>("trs_masterpart.tss_commoditytype").Value).Value == BATTERY_TYPE)
        //                    {
        //                        _queryexpression = new QueryExpression(_DL_tss_partmasterlinesbattery.EntityName);
        //                        _queryexpression.LinkEntities.Add(new LinkEntity(_DL_tss_partmasterlinesbattery.EntityName, _DL_tss_partmasterlinesbatterytypeconsump.EntityName, "tss_partmasterlinesbatteryid", "tss_refpartmasterlinesbattery", JoinOperator.Inner));
        //                        _queryexpression.LinkEntities[0].Columns.AddColumns("tss_nextconsump", "tss_partmasterlinesbatterytypeconsumpid");
        //                        _queryexpression.LinkEntities[0].EntityAlias = "tss_partmasterlinesbatterytypeconsump";
        //                        _queryexpression.Criteria.AddCondition("tss_partmasterid", ConditionOperator.Equal, _marketsizepart.GetAttributeValue<EntityReference>("tss_partnumber").Id);
        //                        _queryexpression.ColumnSet = new ColumnSet(true);
        //                        EntityCollection _partmasterlinesbattery = _DL_tss_partmasterlinesbattery.Select(organizationService, _queryexpression);

        //                        if (_partmasterlinesbattery.Entities.Count > 0)
        //                        {
        //                            if (_setup.Contains("tss_startdatems") && _setup.Contains("tss_enddatems") && _partmasterlinesbattery[0].Contains("tss_partmasterlinesbatterytypeconsump.tss_nextconsump") &&
        //                               (DateTime)_partmasterlinesbattery[0].GetAttributeValue<AliasedValue>("tss_partmasterlinesbatterytypeconsump.tss_nextconsump").Value >= _setup.GetAttributeValue<DateTime>("tss_startdatems") &&
        //                               (DateTime)_partmasterlinesbattery[0].GetAttributeValue<AliasedValue>("tss_partmasterlinesbatterytypeconsump.tss_nextconsump").Value <= _setup.GetAttributeValue<DateTime>("tss_enddatems"))
        //                            {
        //                                _DL_tss_partmasterlinesbatterytypeconsump = new DL_tss_partmasterlinesbatterytypeconsump();
        //                                _DL_tss_partmasterlinesbatterytypeconsump.tss_lastconsump = _mastermarketsizelines.GetAttributeValue<DateTime>("tss_estimatedpmdate");
        //                                _DL_tss_partmasterlinesbatterytypeconsump.tss_nextconsump = _DL_tss_partmasterlinesbatterytypeconsump.tss_lastconsump.AddDays(_partmasterlinesbattery[0].GetAttributeValue<int>("tss_cycleconsump"));
        //                                _DL_tss_partmasterlinesbatterytypeconsump.Update(organizationService, (Guid)_partmasterlinesbattery[0].GetAttributeValue<AliasedValue>("tss_partmasterlinesbatterytypeconsump.tss_partmasterlinesbatterytypeconsumpid").Value);
        //                            }
        //                            else
        //                                continue;
        //                        }
        //                        else
        //                            continue;
        //                    }
        //                    #endregion CHECK IF BATTERY

        //                    #region CHECK IF USE TYRE
        //                    if (_marketsizepart.Contains("tss_tyrecode") && _usertyre)
        //                    {
        //                        _filterexpression = new FilterExpression(LogicalOperator.Or);
        //                        _filterexpression.AddCondition("tss_tyrefrontcode", ConditionOperator.Equal, _marketsizepart.GetAttributeValue<EntityReference>("tss_tyrecode").Id);
        //                        _filterexpression.AddCondition("tss_tyrerearcode", ConditionOperator.Equal, _marketsizepart.GetAttributeValue<EntityReference>("tss_tyrecode").Id);

        //                        _queryexpression = new QueryExpression("trs_functionallocation");
        //                        _queryexpression.Criteria.AddCondition("trs_functionallocationid", ConditionOperator.Equal, _population.GetAttributeValue<EntityReference>("trs_functionallocation").Id);
        //                        _queryexpression.Criteria.AddFilter(_filterexpression);
        //                        _queryexpression.ColumnSet.AddColumns("tss_tyrefrontcode", "tss_tyrerearcode");
        //                        EntityCollection _functionallocationcollection = organizationService.RetrieveMultiple(_queryexpression);

        //                        if (_functionallocationcollection.Entities.Count > 0)
        //                        {
        //                            Guid _code = _functionallocationcollection.Entities[0].Contains("tss_tyrefrontcode") && _functionallocationcollection.Entities[0].GetAttributeValue<EntityReference>("tss_tyrefrontcode").Id == _marketsizepart.GetAttributeValue<EntityReference>("tss_tyrecode").Id ?
        //                                        _functionallocationcollection.Entities[0].GetAttributeValue<EntityReference>("tss_tyrefrontcode").Id :
        //                                        _functionallocationcollection.Entities[0].Contains("tss_tyrerearcode") && _functionallocationcollection.Entities[0].GetAttributeValue<EntityReference>("tss_tyrerearcode").Id == _marketsizepart.GetAttributeValue<EntityReference>("tss_tyrecode").Id ?
        //                                        _functionallocationcollection.Entities[0].GetAttributeValue<EntityReference>("tss_tyrerearcode").Id : Guid.Empty;

        //                            if (_code != Guid.Empty)
        //                            {
        //                                _queryexpression = new QueryExpression(_DL_tss_matrixtyreconsump.EntityName);
        //                                _queryexpression.Criteria.AddCondition("tss_matrixtyreconsumpid", ConditionOperator.Equal, _code);
        //                                _queryexpression.ColumnSet.AddColumn("tss_hmplanning");
        //                                EntityCollection _matrixtyreconsumpcollection = _DL_tss_matrixtyreconsump.Select(organizationService, _queryexpression);

        //                                if (_matrixtyreconsumpcollection.Entities.Count > 0)
        //                                    _quantity = (int)Math.Round(((decimal)_lasthmpm - (decimal)_firsthmpm) / (decimal)_matrixtyreconsumpcollection.Entities[0].GetAttributeValue<int>("tss_hmplanning"), 0);
        //                            }
        //                            else
        //                                continue;
        //                        }
        //                    }
        //                    #endregion CHECK IF USE TYRE
        //                }

        //                _queryexpression = new QueryExpression(_DL_tss_sparepartpricemaster.EntityName);
        //                _queryexpression.LinkEntities.Add(new LinkEntity(_DL_tss_sparepartpricemaster.EntityName, _DL_tss_pricelistpart.EntityName, "tss_pricelistpart", "tss_pricelistpartid", JoinOperator.Inner));
        //                _queryexpression.Criteria.AddCondition("tss_partmaster", ConditionOperator.Equal, _marketsizepart.GetAttributeValue<EntityReference>("tss_partnumber").Id);
        //                _queryexpression.LinkEntities[0].LinkCriteria.AddCondition("tss_type", ConditionOperator.Equal, MS_TYPE);
        //                _queryexpression.LinkEntities[0].EntityAlias = "tss_pricelistpart";
        //                _queryexpression.ColumnSet = new ColumnSet(true);
        //                EntityCollection _sparepartpricemaster = _DL_tss_sparepartpricemaster.Select(organizationService, _queryexpression);

        //                if (_sparepartpricemaster.Entities.Count > 0)
        //                {
        //                    _price = _sparepartpricemaster.Entities[0].Contains("tss_price") ? _sparepartpricemaster.Entities[0].GetAttributeValue<Money>("tss_price").Value : 0m;
        //                    _minimumprice = _sparepartpricemaster.Entities[0].Contains("tss_minimumprice") ? _sparepartpricemaster.Entities[0].GetAttributeValue<Money>("tss_minimumprice").Value : 0m;
        //                }

        //                _DL_tss_mastermarketsizesublines = new DL_tss_mastermarketsizesublines();
        //                _DL_tss_mastermarketsizesublines.tss_mastermslinesref = _mastermarketsizelines.Id;
        //                _DL_tss_mastermarketsizesublines.tss_partnumber = _marketsizepart.GetAttributeValue<EntityReference>("tss_partnumber").Id;
        //                _DL_tss_mastermarketsizesublines.tss_partdescription = _marketsizepart.GetAttributeValue<string>("trs_partdescription");
        //                _DL_tss_mastermarketsizesublines.tss_qty = _quantity;
        //                _DL_tss_mastermarketsizesublines.tss_originalqty = _quantity;
        //                _DL_tss_mastermarketsizesublines.tss_price = _price;
        //                _DL_tss_mastermarketsizesublines.tss_minimumprice = _minimumprice;

        //                _DL_tss_mastermarketsizesublines.Insert(organizationService);
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new InvalidWorkflowException(e.Message);
        //    }
        //}

        public void GenerateSublinesCommodity(IOrganizationService organizationService, DL_tss_mastermarketsize _mastermarketsize, Entity _currentgroupuiocommodity, Entity _currentgroupuiocommodityaccount, Entity _currentkagroupuiocommodity)
        {
            Guid _partnumberid = new Guid();
            int _quantity = 0;

            _DL_tss_mastermarketsizesublines = new DL_tss_mastermarketsizesublines();
            _DL_tss_mastermarketsizesublines.tss_mastermarketsizeid = new EntityReference("tss_mastermarketsize", _mastermarketsize.tss_mastermarketsizeid).Id;

            //OPTION SET - BATTERY / OIL / TYRE
            if (_currentgroupuiocommodity.GetAttributeValue<OptionSetValue>("tss_partcommoditytype").Value == PART_COMMODITYTYPE_BATTERY)
            {
                Guid _partnumberbattery = new Guid();

                if (_currentgroupuiocommodity.GetAttributeValue<OptionSetValue>("tss_batteryby").Value == PART_COMMODITYTYPE_BY_PART)
                {
                    _partnumberbattery = _currentgroupuiocommodity.GetAttributeValue<EntityReference>("tss_batterypartnumber").Id;

                    if (_partnumberbattery != null)
                    {
                        _partnumberid = _partnumberbattery;
                        _DL_tss_mastermarketsizesublines.tss_partnumber = _partnumberbattery;
                    }
                }
                else if (_currentgroupuiocommodity.GetAttributeValue<OptionSetValue>("tss_batteryby").Value == PART_COMMODITYTYPE_BY_SPEC)
                {
                    //BATTERY TYPE - TRACTION / CRANKING
                    if (_currentgroupuiocommodity.GetAttributeValue<OptionSetValue>("tss_batterytype").Value == 865920000)
                    {
                        QueryExpression _queryexpression = new QueryExpression("tss_partmasterlinesbattery");
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria = new FilterExpression
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions =
                            {
                                new ConditionExpression("tss_batterymodeltraction", ConditionOperator.Equal, _currentgroupuiocommodity.GetAttributeValue<EntityReference>("tss_batterytraction").Id)
                            }
                        };

                        EntityCollection _batterytraction = organizationService.RetrieveMultiple(_queryexpression);

                        if (_batterytraction.Entities.Count() > 0)
                        {
                            _partnumberid = _batterytraction.Entities[0].GetAttributeValue<EntityReference>("tss_partmasterid").Id;
                            _partnumberbattery = _partnumberid;
                            _DL_tss_mastermarketsizesublines.tss_partnumber = _partnumberbattery;
                        }
                    }
                    else if (_currentgroupuiocommodity.GetAttributeValue<OptionSetValue>("tss_batterytype").Value == 865920001)
                    {
                        QueryExpression _queryexpression = new QueryExpression("tss_partmasterlinesbattery");
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria = new FilterExpression
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions =
                            {
                                new ConditionExpression("tss_batterymodelcranking", ConditionOperator.Equal, _currentgroupuiocommodity.GetAttributeValue<EntityReference>("tss_batterycranking").Id)
                            }
                        };
                        EntityCollection _batterycranking = organizationService.RetrieveMultiple(_queryexpression);

                        if (_batterycranking.Entities.Count() > 0)
                        {
                            _partnumberid = _batterycranking.Entities[0].GetAttributeValue<EntityReference>("tss_partmasterid").Id;
                            _partnumberbattery = _partnumberid;
                            _DL_tss_mastermarketsizesublines.tss_partnumber = _partnumberbattery;
                        }
                    }
                }

                if (_partnumberbattery != new Guid())
                {
                    DL_tss_mastermarketsizesublines _partmaster = GetPartMaster(organizationService, _partnumberbattery);
                    _DL_tss_mastermarketsizesublines.tss_price = _partmaster.tss_price;
                    _DL_tss_mastermarketsizesublines.tss_minimumprice = _partmaster.tss_minimumprice;
                    _DL_tss_mastermarketsizesublines.tss_partdescription = _partmaster.tss_partdescription;
                }

                _quantity = _currentgroupuiocommodity.GetAttributeValue<int>("tss_battery");
            }
            else if (_currentgroupuiocommodity.GetAttributeValue<OptionSetValue>("tss_partcommoditytype").Value == PART_COMMODITYTYPE_OIL)
            {
                Guid _partnumberoil = new Guid();

                if (_currentgroupuiocommodity.GetAttributeValue<OptionSetValue>("tss_oilby").Value == PART_COMMODITYTYPE_BY_PART)
                {
                    _partnumberoil = _currentgroupuiocommodity.GetAttributeValue<EntityReference>("tss_oilpartnumber").Id;

                    if (_partnumberoil != null)
                    {
                        _partnumberid = _partnumberoil;
                        _DL_tss_mastermarketsizesublines.tss_partnumber = _partnumberoil;
                    }
                }
                else if (_currentgroupuiocommodity.GetAttributeValue<OptionSetValue>("tss_oilby").Value == PART_COMMODITYTYPE_BY_SPEC)
                {
                    QueryExpression _queryexpression = new QueryExpression("tss_partmasterlinesoil");
                    _queryexpression.ColumnSet = new ColumnSet(true);
                    _queryexpression.Criteria = new FilterExpression
                    {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                        {
                            new ConditionExpression("tss_oiltype", ConditionOperator.Equal, _currentgroupuiocommodity.GetAttributeValue<EntityReference>("tss_jenisoil").Id)
                        }
                    };

                    EntityCollection _partmasterlinesoil = organizationService.RetrieveMultiple(_queryexpression);

                    if (_partmasterlinesoil.Entities.Count() > 0)
                    {
                        _partnumberid = _partmasterlinesoil.Entities[0].GetAttributeValue<EntityReference>("tss_partmasterid").Id;
                        _partnumberoil = _partnumberid;
                        _DL_tss_mastermarketsizesublines.tss_partnumber = _partnumberoil;
                    }
                }

                if (_partnumberoil != new Guid())
                {
                    DL_tss_mastermarketsizesublines _partmaster = GetPartMaster(organizationService, _partnumberoil);

                    _DL_tss_mastermarketsizesublines.tss_price = _partmaster.tss_price;
                    _DL_tss_mastermarketsizesublines.tss_minimumprice = _partmaster.tss_minimumprice;
                    _DL_tss_mastermarketsizesublines.tss_partdescription = _partmaster.tss_partdescription;
                }

                if (_currentgroupuiocommodity.GetAttributeValue<OptionSetValue>("tss_oilqtyby").Value == 865920000)
                    _quantity = _currentgroupuiocommodity.GetAttributeValue<int>("tss_oilqtypcs");
                else if (_currentgroupuiocommodity.GetAttributeValue<OptionSetValue>("tss_oilqtyby").Value == 865920001)
                    _quantity = _currentgroupuiocommodity.GetAttributeValue<int>("tss_oil");
            }
            else if (_currentgroupuiocommodity.GetAttributeValue<OptionSetValue>("tss_partcommoditytype").Value == PART_COMMODITYTYPE_TYRE)
            {
                Guid _partnumbertyre = new Guid();

                if (_currentgroupuiocommodity.GetAttributeValue<OptionSetValue>("tss_tyreby").Value == PART_COMMODITYTYPE_BY_PART)
                {
                    _partnumbertyre = _currentgroupuiocommodity.GetAttributeValue<EntityReference>("tss_tyrepartnumber").Id;

                    if (_partnumbertyre != null)
                    {
                        _partnumberid = _partnumbertyre;
                        _DL_tss_mastermarketsizesublines.tss_partnumber = _partnumbertyre;
                    }
                }
                else if (_currentgroupuiocommodity.GetAttributeValue<OptionSetValue>("tss_tyreby").Value == PART_COMMODITYTYPE_BY_SPEC)
                {
                    Entity _tss_partmasterlinestyretype = organizationService.Retrieve("tss_partmasterlinestyretype", _currentgroupuiocommodity.GetAttributeValue<EntityReference>("tss_tyrespec").Id, new ColumnSet(true));

                    if (_tss_partmasterlinestyretype != null)
                    {
                        _partnumberid = _tss_partmasterlinestyretype.GetAttributeValue<EntityReference>("tss_partmasterid").Id;
                        _partnumbertyre = _partnumberid;
                        _DL_tss_mastermarketsizesublines.tss_partnumber = _partnumbertyre;
                    }
                }

                if (_partnumbertyre != new Guid())
                {
                    DL_tss_mastermarketsizesublines _partmaster = GetPartMaster(organizationService, _partnumbertyre);

                    _DL_tss_mastermarketsizesublines.tss_price = _partmaster.tss_price;
                    _DL_tss_mastermarketsizesublines.tss_minimumprice = _partmaster.tss_minimumprice;
                    _DL_tss_mastermarketsizesublines.tss_partdescription = _partmaster.tss_partdescription;
                }

                _quantity = _currentgroupuiocommodity.GetAttributeValue<int>("tss_tyre");
            }

            _DL_tss_mastermarketsizesublines.tss_qty = _currentgroupuiocommodityaccount.GetAttributeValue<int>("tss_qty") * _quantity;
            _DL_tss_mastermarketsizesublines.tss_originalqty = _quantity;

            if (_partnumberid != new Guid())
                _DL_tss_mastermarketsizesublines.Insert(organizationService);
            else
            {
                _DL_tss_kagroupuiocommodity = new DL_tss_kagroupuiocommodity();
                _DL_tss_kagroupuiocommodity.tss_calculatestatus = false;
                _DL_tss_kagroupuiocommodity.UpdateStatus(organizationService, _currentkagroupuiocommodity.Id); //.GetAttributeValue<Guid>("tss_kagroupuiocommodityid"));

                _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
                _DL_tss_mastermarketsize.tss_status = STATUS_ERROR_MS;
                _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_ERROR_MS;
                _DL_tss_mastermarketsize.tss_errormessage = "Part Number NOT Found !";
                _DL_tss_mastermarketsize.Update(organizationService, _mastermarketsize.tss_mastermarketsizeid);
            }
        }

        public DL_tss_mastermarketsizesublines GetPartMaster(IOrganizationService organizationService, Guid _partnumberid)
        {
            decimal _price = 0m;
            decimal _minimumprice = 0m;
            string _partdescription = "";

            QueryExpression _queryexpression = new QueryExpression(_DL_tss_sparepartpricemaster.EntityName);
            _queryexpression.LinkEntities.Add(new LinkEntity(_DL_tss_sparepartpricemaster.EntityName, _DL_tss_pricelistpart.EntityName, "tss_pricelistpart", "tss_pricelistpartid", JoinOperator.Inner));
            _queryexpression.Criteria.AddCondition("tss_partmaster", ConditionOperator.Equal, _partnumberid);
            _queryexpression.LinkEntities[0].LinkCriteria.AddCondition("tss_type", ConditionOperator.Equal, MS_TYPE);
            _queryexpression.LinkEntities[0].EntityAlias = "tss_pricelistpart";
            _queryexpression.ColumnSet = new ColumnSet(true);
            EntityCollection _sparepartpricemastercollection = _DL_tss_sparepartpricemaster.Select(organizationService, _queryexpression);

            DL_tss_mastermarketsizesublines _mastermarketsizesublines = new DL_tss_mastermarketsizesublines();

            if (_sparepartpricemastercollection.Entities.Count > 0)
            {
                _price = _sparepartpricemastercollection.Entities[0].Contains("tss_price") ? _sparepartpricemastercollection.Entities[0].GetAttributeValue<Money>("tss_price").Value : 0m;
                _minimumprice = _sparepartpricemastercollection.Entities[0].Contains("tss_minimumprice") ? _sparepartpricemastercollection.Entities[0].GetAttributeValue<Money>("tss_minimumprice").Value : 0m;
                _partdescription = _sparepartpricemastercollection.Entities[0].GetAttributeValue<String>("tss_partmastername");

                _mastermarketsizesublines.tss_price = _price;
                _mastermarketsizesublines.tss_minimumprice = _minimumprice;
                _mastermarketsizesublines.tss_partdescription = _partdescription;
            }
            else
            {
                _mastermarketsizesublines.tss_price = 0;
                _mastermarketsizesublines.tss_minimumprice = 0;
                _mastermarketsizesublines.tss_partdescription = _partdescription;
            }

            return _mastermarketsizesublines;
        }

        public void Revise(IOrganizationService organizationService, EntityCollection pssToRevise)
        {
            var groupPssCustomer = (from g in pssToRevise.Entities.AsEnumerable()
                                    select new
                                    {
                                        pssid = g.GetAttributeValue<EntityReference>("tss_pss").Id,
                                        customerId = g.GetAttributeValue<EntityReference>("tss_customer").Id,
                                        entityName = g.LogicalName
                                    }).ToList().Distinct();

            foreach (var o in groupPssCustomer)
            {
                QueryExpression q = new QueryExpression(_DL_tss_mastermarketsize.EntityName);
                q.ColumnSet = new ColumnSet(true);

                FilterExpression f = new FilterExpression(LogicalOperator.And);
                f.AddCondition("tss_pss", ConditionOperator.Equal, o.pssid);
                f.AddCondition("tss_customer", ConditionOperator.Equal, o.customerId);
                f.AddCondition("tss_status", ConditionOperator.Equal, 865920000);
                q.Criteria.AddFilter(f);

                EntityCollection enToUpdate = organizationService.RetrieveMultiple(q);

                foreach (var entoupdate in enToUpdate.Entities)
                {
                    entoupdate.Attributes["tss_status"] = new OptionSetValue(865920002);
                    organizationService.Update(entoupdate);
                }
            }
        }


        #region COMMENT

        /// <summary>
        /// GENERATE MASTER MARKET SIZE WITHOUT CALCULATION AND LINES & SUBLINES
        /// </summary>
        /// <param name="organizationService"></param>
        /// <param name="ms"></param>
        /// <param name="population"></param>
        /// <param name="setup"></param>
        //public void GenerateMasterMS(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context, EntityCollection listKeyAccount)
        //{
        //    #region Get Key Account
        //    if (listKeyAccount == null)
        //    {
        //        FilterExpression fStatus = new FilterExpression(LogicalOperator.Or);
        //        fStatus.AddCondition("tss_status", ConditionOperator.Equal, STATUS_OPEN);
        //        fStatus.AddCondition("tss_status", ConditionOperator.Equal, STATUS_ERROR);

        //        FilterExpression fKA = new FilterExpression(LogicalOperator.And);
        //        fKA.AddFilter(fStatus);
        //        fKA.AddCondition("tss_pss", ConditionOperator.Equal, context.UserId);
        //        fKA.AddCondition("tss_customer", ConditionOperator.NotNull);
        //        fKA.AddCondition("tss_activeenddate", ConditionOperator.GreaterEqual, DateTime.Today);
        //        fKA.AddCondition("tss_activestartdate", ConditionOperator.LessEqual, DateTime.Today);
        //        fKA.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true); //2018.10.12

        //        QueryExpression qKA = new QueryExpression(_DL_tss_keyaccount.EntityName);
        //        qKA.Criteria.AddFilter(fKA);
        //        qKA.ColumnSet = new ColumnSet(true);
        //        listKeyAccount = organizationService.RetrieveMultiple(qKA); // _DL_tss_mastermarketsize.Select(organizationService, qKA);
        //    }
        //    #endregion

        //    if (listKeyAccount.Entities.Count > 0)
        //    {
        //        #region Generate MS KAUIO
        //        #region Get KA UIO
        //        //Get KA UIO
        //        object[] kaIds = listKeyAccount.Entities.Select(x => (object)x.Id).ToArray();
        //        FilterExpression fKAUIO = new FilterExpression(LogicalOperator.And);
        //        fKAUIO.AddCondition("tss_keyaccountid", ConditionOperator.In, kaIds);
        //        fKAUIO.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true);
        //        fKAUIO.AddCondition("tss_serialnumber", ConditionOperator.NotNull);

        //        QueryExpression qKAUIO = new QueryExpression(_DL_tss_kauio.EntityName);
        //        qKAUIO.ColumnSet = new ColumnSet(true);
        //        qKAUIO.Criteria.AddFilter(fKAUIO);
        //        //EntityCollection listKAUIO = organizationService.RetrieveMultiple(qKAUIO); // _DL_tss_kauio.Select(organizationService, qKAUIO);
        //        List<Entity> listKAUIO = RetrieveMultipleSDK(organizationService, qKAUIO);
        //        #endregion

        //        #region Generatex
        //        //Check Population
        //        if (listKAUIO.Count > 0)
        //        {
        //            //throw new InvalidWorkflowException(listKAUIO.Count().ToString());

        //            object[] serialNums = listKAUIO.Select(x => (object)x.GetAttributeValue<EntityReference>("tss_serialnumber").Id).ToArray();

        //            FilterExpression fPop = new FilterExpression(LogicalOperator.And);
        //            fPop.AddCondition("new_populationid", ConditionOperator.In, serialNums);
        //            fPop.AddCondition("trs_productmaster", ConditionOperator.NotNull);

        //            QueryExpression qPop = new QueryExpression(_DL_population.EntityName);
        //            qPop.ColumnSet = new ColumnSet(true);
        //            qPop.Criteria.AddFilter(fPop);
        //            List<Entity> listPop = RetrieveMultipleSDK(organizationService, qPop); // organizationService.RetrieveMultiple(qPop); // _DL_population.Select(organizationService, qPop);

        //            if (listPop.Count > 0)
        //            {
        //                //List<Guid> msIds = new List<Guid>();

        //                foreach (var pop in listPop)
        //                {
        //                    Guid _masterMarketSize = new Guid();
        //                    Entity currentUIO = listKAUIO.Where(x => x.GetAttributeValue<EntityReference>("tss_serialnumber").Id == pop.GetAttributeValue<Guid>("new_populationid")).FirstOrDefault();
        //                    Entity currentKA = listKeyAccount.Entities.Where(x => x.Id == currentUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id).FirstOrDefault();

        //                    _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        //                    _DL_tss_mastermarketsize.tss_pss = currentUIO.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                    _DL_tss_mastermarketsize.tss_customer = currentUIO.GetAttributeValue<EntityReference>("tss_customer").Id; // pop.GetAttributeValue<EntityReference>("new_customercode").Id;
        //                    _DL_tss_mastermarketsize.tss_serialnumber = pop.GetAttributeValue<Guid>("new_populationid");

        //                    if (pop.Contains("tss_populationstatus"))
        //                    {
        //                        _DL_tss_mastermarketsize.tss_unittype = pop.GetAttributeValue<bool>("tss_populationstatus") ? UNITTYPE_UIO : UNITTYPE_NONUIO;
        //                    }
        //                    _DL_tss_mastermarketsize.tss_status = STATUS_COMPLETED_MS;
        //                    _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_COMPLETED_MS;
        //                    //_DL_tss_mastermarketsize.tss_usetyre = useTyre;
        //                    _DL_tss_mastermarketsize.tss_msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                    _DL_tss_mastermarketsize.tss_msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend");
        //                    _DL_tss_mastermarketsize.tss_activeperiodstart = currentKA.GetAttributeValue<DateTime>("tss_activestartdate");
        //                    _DL_tss_mastermarketsize.tss_activeperiodsend = currentKA.GetAttributeValue<DateTime>("tss_activeenddate");
        //                    _DL_tss_mastermarketsize.tss_keyaccountid = currentKA.Id;
        //                    _DL_tss_mastermarketsize.tss_msperiod = currentKA.GetAttributeValue<int>("tss_revision");
        //                    _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;

        //                    _masterMarketSize = _DL_tss_mastermarketsize.Insert(organizationService);

        //                }
        //            }
        //        }
        //        #endregion

        //        #endregion

        //        #region Generate KA Group UIO Commodity
        //        #region Get KA Group UIO Commodity
        //        //Get KA UIO
        //        FilterExpression fkaUIOCommodity = new FilterExpression(LogicalOperator.And);
        //        fkaUIOCommodity.AddCondition("tss_keyaccountid", ConditionOperator.In, kaIds);
        //        fkaUIOCommodity.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true);
        //        fkaUIOCommodity.AddCondition("tss_groupuiocommodity", ConditionOperator.NotNull);

        //        QueryExpression qkaUIOCommodity = new QueryExpression(_DL_tss_kagroupuiocommodity.EntityName);
        //        qkaUIOCommodity.ColumnSet = new ColumnSet(true);
        //        qkaUIOCommodity.Criteria.AddFilter(fkaUIOCommodity);
        //        EntityCollection listKAGroupUIOCommodity = _DL_tss_kagroupuiocommodity.Select(organizationService, qkaUIOCommodity);
        //        #endregion

        //        #region Generate
        //        if (listKAGroupUIOCommodity.Entities.Count > 0)
        //        {
        //            #region Get Group Commodity Account
        //            object[] kaGroupUIOCommIds = listKAGroupUIOCommodity.Entities.Select(x => (object)x.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id).ToArray();

        //            FilterExpression fGroupAccount = new FilterExpression(LogicalOperator.And);
        //            fGroupAccount.AddCondition("tss_groupuiocommodityaccountid", ConditionOperator.In, kaGroupUIOCommIds);
        //            fGroupAccount.AddCondition("tss_groupuiocommodityheader", ConditionOperator.NotNull);

        //            QueryExpression qGroupAccount = new QueryExpression(_DL_tss_groupuiocommodityaccount.EntityName);
        //            qGroupAccount.ColumnSet = new ColumnSet(true);
        //            qGroupAccount.Criteria.AddFilter(fGroupAccount);
        //            EntityCollection listGroupAccount = _DL_tss_groupuiocommodityaccount.Select(organizationService, qGroupAccount);
        //            #endregion

        //            if (listGroupAccount.Entities.Count > 0)
        //            {
        //                foreach (var groupAccount in listGroupAccount.Entities)
        //                {
        //                    Guid _masterMarketSize = new Guid();
        //                    Entity currentKAGroupUIO = listKAGroupUIOCommodity.Entities.Where(x => x.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id == groupAccount.GetAttributeValue<Guid>("tss_groupuiocommodityaccountid")).FirstOrDefault();
        //                    Entity currentKA = listKeyAccount.Entities.Where(x => x.Id == currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id).FirstOrDefault();

        //                    if (groupAccount.Contains("tss_qty") && groupAccount.GetAttributeValue<int>("tss_qty") > 0)
        //                    {
        //                        _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        //                        _DL_tss_mastermarketsize.tss_pss = currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                        _DL_tss_mastermarketsize.tss_customer = currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_customer").Id;
        //                        //_DL_tss_mastermarketsize.tss_groupuiocommodity = groupAccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityname").Id;
        //                        _DL_tss_mastermarketsize.tss_groupuiocommodityheader = groupAccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id;
        //                        _DL_tss_mastermarketsize.tss_qty = groupAccount.GetAttributeValue<int>("tss_qty");
        //                        _DL_tss_mastermarketsize.tss_unittype = UNITTYPE_COMMODITY;
        //                        _DL_tss_mastermarketsize.tss_status = STATUS_COMPLETED_MS;
        //                        _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_COMPLETED_MS;
        //                        _DL_tss_mastermarketsize.tss_msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                        _DL_tss_mastermarketsize.tss_msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend");
        //                        _DL_tss_mastermarketsize.tss_activeperiodstart = currentKA.GetAttributeValue<DateTime>("tss_activestartdate");
        //                        _DL_tss_mastermarketsize.tss_activeperiodsend = currentKA.GetAttributeValue<DateTime>("tss_activeenddate");
        //                        _DL_tss_mastermarketsize.tss_keyaccountid = currentKA.Id;
        //                        _DL_tss_mastermarketsize.tss_msperiod = currentKA.GetAttributeValue<int>("tss_revision");
        //                        _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;

        //                        _masterMarketSize = _DL_tss_mastermarketsize.Insert(organizationService);


        //                    }
        //                    else
        //                    {
        //                        _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        //                        _DL_tss_mastermarketsize.tss_pss = currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                        _DL_tss_mastermarketsize.tss_customer = currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_customer").Id;
        //                        //_DL_tss_mastermarketsize.tss_groupuiocommodity = groupAccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityname").Id;
        //                        _DL_tss_mastermarketsize.tss_groupuiocommodityheader = groupAccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id;
        //                        _DL_tss_mastermarketsize.tss_unittype = UNITTYPE_COMMODITY;
        //                        _DL_tss_mastermarketsize.tss_status = STATUS_ERROR_MS;
        //                        _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_ERROR_MS;
        //                        _DL_tss_mastermarketsize.tss_errormessage = "Qty still not define";
        //                        _DL_tss_mastermarketsize.tss_msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                        _DL_tss_mastermarketsize.tss_msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend");
        //                        _DL_tss_mastermarketsize.tss_activeperiodstart = currentKA.GetAttributeValue<DateTime>("tss_activestartdate");
        //                        _DL_tss_mastermarketsize.tss_activeperiodsend = currentKA.GetAttributeValue<DateTime>("tss_activeenddate");
        //                        _DL_tss_mastermarketsize.tss_keyaccountid = currentKA.Id;
        //                        _DL_tss_mastermarketsize.tss_msperiod = currentKA.GetAttributeValue<int>("tss_revision");
        //                        _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;

        //                        _DL_tss_mastermarketsize.Insert(organizationService);

        //                    }
        //                }
        //            }
        //        }
        //        #endregion
        //        #endregion

        //        //Guid[] customerID = listKeyAccount.Entities.GroupBy(x => x.GetAttributeValue<EntityReference>("tss_customer").Id).Select(x => x.First().GetAttributeValue<EntityReference>("tss_customer").Id).ToArray();

        //        //BL_tss_marketsizeresultpss _BL_tss_marketsizeresultpss = new BL_tss_marketsizeresultpss();
        //        //_BL_tss_marketsizeresultpss.GenerateMarketSizeResultPSS_New(organizationService, context, customerID);

        //        //BL_tss_marketsizesummarybypartnumber _BL_tss_marketsizesummarybypartnumber = new BL_tss_marketsizesummarybypartnumber();
        //        //_BL_tss_marketsizesummarybypartnumber.GenerateMarketSizeSummaryByPartNumber(organizationService, context);

        //        //BL_tss_marketsizesummarybygroupuiocommodity _BL_tss_marketsizesummarybygroupuiocommodity = new BL_tss_marketsizesummarybygroupuiocommodity();
        //        //_BL_tss_marketsizesummarybygroupuiocommodity.GenerateMarketSizeSummaryByGroupUIOCommodity(organizationService, context, listKeyAccount);
        //    }
        //    else
        //    {
        //        throw new InvalidWorkflowException("KAUIO or KA Group UIO Commodity not exist.");
        //    }
        //}






        //public void GenerateMasterMS(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context, EntityCollection listKeyAccount)
        //{
        //    #region Get Key Account
        //    if (listKeyAccount == null)
        //    {
        //        FilterExpression fStatus = new FilterExpression(LogicalOperator.Or);
        //        fStatus.AddCondition("tss_status", ConditionOperator.Equal, STATUS_OPEN);
        //        fStatus.AddCondition("tss_status", ConditionOperator.Equal, STATUS_ERROR);

        //        FilterExpression fKA = new FilterExpression(LogicalOperator.And);
        //        fKA.AddFilter(fStatus);
        //        fKA.AddCondition("tss_pss", ConditionOperator.Equal, context.UserId);
        //        fKA.AddCondition("tss_customer", ConditionOperator.NotNull);
        //        fKA.AddCondition("tss_activeenddate", ConditionOperator.GreaterEqual, DateTime.Today);
        //        fKA.AddCondition("tss_activestartdate", ConditionOperator.LessEqual, DateTime.Today);
        //        fKA.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true); //2018.10.12

        //        QueryExpression qKA = new QueryExpression(_DL_tss_keyaccount.EntityName);
        //        qKA.Criteria.AddFilter(fKA);
        //        qKA.ColumnSet = new ColumnSet(true);
        //        listKeyAccount = organizationService.RetrieveMultiple(qKA); // _DL_tss_mastermarketsize.Select(organizationService, qKA);
        //    }
        //    #endregion

        //    if (listKeyAccount.Entities.Count > 0)
        //    {
        //        #region Generate MS KAUIO
        //        #region Get KA UIO
        //        //Get KA UIO
        //        object[] kaIds = listKeyAccount.Entities.Select(x => (object)x.Id).ToArray();
        //        FilterExpression fKAUIO = new FilterExpression(LogicalOperator.And);
        //        fKAUIO.AddCondition("tss_keyaccountid", ConditionOperator.In, kaIds);
        //        fKAUIO.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true);
        //        fKAUIO.AddCondition("tss_serialnumber", ConditionOperator.NotNull);

        //        QueryExpression qKAUIO = new QueryExpression(_DL_tss_kauio.EntityName);
        //        qKAUIO.ColumnSet = new ColumnSet(true);
        //        qKAUIO.Criteria.AddFilter(fKAUIO);
        //        EntityCollection listKAUIO = organizationService.RetrieveMultiple(qKAUIO); // _DL_tss_kauio.Select(organizationService, qKAUIO);
        //        #endregion

        //        #region Generatex
        //        //Check Population
        //        if (listKAUIO.Entities.Count > 0)
        //        {
        //            object[] serialNums = listKAUIO.Entities.Select(x => (object)x.GetAttributeValue<EntityReference>("tss_serialnumber").Id).ToArray();

        //            FilterExpression fPop = new FilterExpression(LogicalOperator.And);
        //            fPop.AddCondition("new_populationid", ConditionOperator.In, serialNums);
        //            fPop.AddCondition("trs_productmaster", ConditionOperator.NotNull);

        //            QueryExpression qPop = new QueryExpression(_DL_population.EntityName);
        //            qPop.ColumnSet = new ColumnSet(true);
        //            qPop.Criteria.AddFilter(fPop);
        //            EntityCollection listPop = organizationService.RetrieveMultiple(qPop); // _DL_population.Select(organizationService, qPop);

        //            if (listPop.Entities.Count > 0)
        //            {
        //                List<Guid> msIds = new List<Guid>();

        //                foreach (var pop in listPop.Entities)
        //                {
        //                    Guid _masterMarketSize = new Guid();
        //                    Entity currentUIO = listKAUIO.Entities.Where(x => x.GetAttributeValue<EntityReference>("tss_serialnumber").Id == pop.GetAttributeValue<Guid>("new_populationid")).FirstOrDefault();
        //                    Entity currentKA = listKeyAccount.Entities.Where(x => x.Id == currentUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id).FirstOrDefault();

        //                    bool Method1 = false;
        //                    bool Method2 = false;
        //                    bool Method3 = false;
        //                    bool Method4 = false;
        //                    bool Method5 = false;

        //                    //CHECK METHOD 1
        //                    if (pop.Attributes.Contains("tss_mscurrenthourmeter")
        //                        && pop.Attributes.Contains("tss_mslasthourmeter")
        //                        && pop.Attributes.Contains("tss_mscurrenthourmeterdate")
        //                        && pop.Attributes.Contains("tss_mslasthourmeterdate"))
        //                    {
        //                        Method1 = true;
        //                        Console.Write("Method 1 : " + Method1);
        //                    }

        //                    //CHECK METHOD 2
        //                    if (pop.Attributes.Contains("tss_mscurrenthourmeter")
        //                        && pop.Attributes.Contains("tss_mscurrenthourmeterdate")
        //                        && (pop.Attributes.Contains("new_deliverydate") || pop.Attributes.Contains("trs_warrantyenddate")))
        //                    {
        //                        Method2 = true;
        //                        Console.Write("Method 2 : " + Method2);
        //                    }

        //                    //CHECK METHOD 3
        //                    if (pop.Attributes.Contains("tss_estworkinghour"))
        //                    {
        //                        Method3 = true;
        //                        Console.Write("Method 3 : " + Method3);
        //                    }

        //                    //CHECK METHOD 4
        //                    if (pop.Attributes.Contains("new_deliverydate")
        //                        && currentKA.Attributes.Contains("tss_msperiodstart")
        //                        && currentKA.Attributes.Contains("tss_msperiodend"))
        //                    {
        //                        Method4 = true;
        //                        Console.Write("Method 4 : " + Method4);
        //                    }

        //                    //CHECK METHOD 5
        //                    if ((pop.Attributes.Contains("new_deliverydate") || pop.Attributes.Contains("trs_warrantyenddate"))
        //                        && currentKA.Attributes.Contains("tss_msperiodstart")
        //                        && currentKA.Attributes.Contains("tss_msperiodend"))
        //                    {
        //                        Method5 = true;
        //                        Console.Write("Method 5 : " + Method5);
        //                    }

        //                    if (Method1 || Method2 || Method3 || Method4 || Method5)
        //                    {
        //                        //Generate MS UIO/Non UIO
        //                        decimal currentHM;
        //                        decimal lastHM;
        //                        DateTime currentHMDate;
        //                        DateTime lastHMDate;
        //                        DateTime deliveryDate;
        //                        DateTime warrantyEndDate = DateTime.MinValue;
        //                        DateTime calculateMarketSizeDate = DateTime.Now.ToLocalTime().Date;

        //                        QueryExpression qPro = new QueryExpression(_DL_product.EntityName);
        //                        qPro.Criteria.AddCondition("productid", ConditionOperator.Equal, pop.GetAttributeValue<EntityReference>("trs_productmaster").Id);
        //                        qPro.ColumnSet.AddColumn("tss_usetyre");
        //                        bool useTyre = _DL_product.Select(organizationService, qPro).Entities.FirstOrDefault().GetAttributeValue<bool>("tss_usetyre");

        //                        QueryExpression qSetup = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
        //                        qSetup.ColumnSet = new ColumnSet(true);
        //                        EntityCollection setups = _DL_tss_sparepartsetup.Select(organizationService, qSetup);

        //                        if (setups.Entities.Count > 0)
        //                        {
        //                            Entity setup = setups.Entities[0];
        //                            _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        //                            _DL_tss_mastermarketsize.tss_pss = currentUIO.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                            _DL_tss_mastermarketsize.tss_customer = currentUIO.GetAttributeValue<EntityReference>("tss_customer").Id; // pop.GetAttributeValue<EntityReference>("new_customercode").Id;
        //                            _DL_tss_mastermarketsize.tss_serialnumber = pop.GetAttributeValue<Guid>("new_populationid");

        //                            //METHOD 1
        //                            if (pop.Contains("tss_mscurrenthourmeter")
        //                               && pop.Contains("tss_mslasthourmeter")
        //                               && pop.Contains("tss_mscurrenthourmeterdate")
        //                               && pop.Contains("tss_mslasthourmeterdate"))
        //                            {
        //                                currentHM = pop.GetAttributeValue<decimal>("tss_mscurrenthourmeter");
        //                                lastHM = pop.GetAttributeValue<decimal>("tss_mslasthourmeter");
        //                                currentHMDate = pop.GetAttributeValue<DateTime>("tss_mscurrenthourmeterdate").ToLocalTime().Date;
        //                                lastHMDate = pop.GetAttributeValue<DateTime>("tss_mslasthourmeterdate").ToLocalTime().Date;

        //                                //if ((decimal)(currentHMDate - lastHMDate).TotalDays == 0)
        //                                //    _DL_tss_mastermarketsize.tss_avghmmethod1 = 0;
        //                                //else
        //                                //    _DL_tss_mastermarketsize.tss_avghmmethod1 = avghmmethod1 > (decimal)24 ? (decimal)24 : 0;

        //                                if ((decimal)(currentHMDate - lastHMDate).TotalDays > 0)
        //                                {
        //                                    decimal avghmmethod1 = (currentHM - lastHM) / ((decimal)(currentHMDate - lastHMDate).TotalDays);

        //                                    if (avghmmethod1 > 0)
        //                                    {
        //                                        _DL_tss_mastermarketsize.tss_avghmmethod1 = avghmmethod1 > (decimal)24 ? (decimal)24 : avghmmethod1;
        //                                    }
        //                                }
        //                            }

        //                            //METHOD 2
        //                            if (pop.Contains("tss_mscurrenthourmeter")
        //                               && pop.Contains("tss_mscurrenthourmeterdate")
        //                               && (pop.Contains("new_deliverydate") || pop.Contains("trs_warrantyenddate")))
        //                            {
        //                                currentHM = pop.GetAttributeValue<decimal>("tss_mscurrenthourmeter");
        //                                currentHMDate = pop.GetAttributeValue<DateTime>("tss_mscurrenthourmeterdate").ToLocalTime().Date;
        //                                deliveryDate = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().Date;

        //                                if (!pop.Attributes.Contains("trs_warrantyenddate"))
        //                                {
        //                                    warrantyEndDate = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().AddYears(1).Date;
        //                                }
        //                                else
        //                                {
        //                                    warrantyEndDate = pop.GetAttributeValue<DateTime>("trs_warrantyenddate").ToLocalTime().Date;
        //                                }

        //                                //if ((decimal)(currentHMDate - deliveryDate).TotalDays == 0)
        //                                //    _DL_tss_mastermarketsize.tss_avghmmethod2 = 0;
        //                                //else
        //                                //    _DL_tss_mastermarketsize.tss_avghmmethod2 = avghmmethod2 > (decimal)24 ? (decimal)24 : 0;

        //                                if ((decimal)(currentHMDate - warrantyEndDate).TotalDays > 0)
        //                                {
        //                                    decimal avghmmethod2 = currentHM / ((decimal)(currentHMDate - warrantyEndDate).TotalDays);

        //                                    if (avghmmethod2 > 0)
        //                                    {
        //                                        _DL_tss_mastermarketsize.tss_avghmmethod2 = avghmmethod2 > (decimal)24 ? (decimal)24 : avghmmethod2;
        //                                    }
        //                                }
        //                            }

        //                            //METHOD 3
        //                            if (pop.Contains("tss_estworkinghour"))
        //                            {
        //                                decimal estimateWH = pop.GetAttributeValue<int>("tss_estworkinghour");

        //                                if (estimateWH > 0)
        //                                {
        //                                    _DL_tss_mastermarketsize.tss_avghmmethod3 = estimateWH > (decimal)24 ? (decimal)24 : estimateWH;
        //                                }
        //                            }

        //                            //METHOD 4
        //                            if (pop.Contains("new_deliverydate")
        //                                && currentKA.Contains("tss_msperiodstart")
        //                                && currentKA.Contains("tss_msperiodend"))
        //                            {
        //                                CalculateDate oCalculate = new CalculateDate();

        //                                deliveryDate = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().Date;
        //                                DateTime msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart").ToLocalTime().Date;
        //                                DateTime msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend").ToLocalTime().Date;

        //                                decimal oDeliveryDate = oCalculate.DiffYear(deliveryDate, calculateMarketSizeDate);
        //                                decimal oPeriodDate = oCalculate.DiffYear(msperiodstart, msperiodend);

        //                                decimal periodpmmethod4 = (decimal)(oDeliveryDate + oPeriodDate);

        //                                if (periodpmmethod4 > 0)
        //                                {
        //                                    _DL_tss_mastermarketsize.tss_periodpmmethod4 = periodpmmethod4;
        //                                }
        //                            }

        //                            //METHOD 5
        //                            if (pop.Contains("new_deliverydate")
        //                                && currentKA.Contains("tss_msperiodstart")
        //                                && currentKA.Contains("tss_msperiodend"))
        //                            {
        //                                if (!pop.Attributes.Contains("trs_warrantyenddate"))
        //                                {
        //                                    warrantyEndDate = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().AddYears(1).Date;
        //                                }
        //                                else
        //                                {
        //                                    warrantyEndDate = pop.GetAttributeValue<DateTime>("trs_warrantyenddate").ToLocalTime().Date;
        //                                }

        //                                CalculateDate oCalculate = new CalculateDate();

        //                                deliveryDate = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().Date;
        //                                DateTime msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart").ToLocalTime().Date;
        //                                DateTime msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend").ToLocalTime().Date;

        //                                decimal oWarrantyDate = oCalculate.DiffYear(deliveryDate, warrantyEndDate);
        //                                decimal oPeriodDate = oCalculate.DiffYear(msperiodstart, msperiodend);

        //                                decimal periodpmmethod5 = (decimal)(oWarrantyDate + oPeriodDate);

        //                                if (periodpmmethod5 > 0)
        //                                {
        //                                    _DL_tss_mastermarketsize.tss_periodpmmethod5 = periodpmmethod5;
        //                                }
        //                            }

        //                            if (pop.Contains("tss_populationstatus"))
        //                            {
        //                                _DL_tss_mastermarketsize.tss_unittype = pop.GetAttributeValue<bool>("tss_populationstatus") ? UNITTYPE_UIO : UNITTYPE_NONUIO;
        //                            }
        //                            _DL_tss_mastermarketsize.tss_status = STATUS_COMPLETED_MS;
        //                            _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_COMPLETED_MS;
        //                            _DL_tss_mastermarketsize.tss_usetyre = useTyre;
        //                            _DL_tss_mastermarketsize.tss_msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                            _DL_tss_mastermarketsize.tss_msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend");
        //                            _DL_tss_mastermarketsize.tss_activeperiodstart = currentKA.GetAttributeValue<DateTime>("tss_activestartdate");
        //                            _DL_tss_mastermarketsize.tss_activeperiodsend = currentKA.GetAttributeValue<DateTime>("tss_activeenddate");
        //                            _DL_tss_mastermarketsize.tss_keyaccountid = currentKA.Id;
        //                            _DL_tss_mastermarketsize.tss_msperiod = currentKA.GetAttributeValue<int>("tss_revision");
        //                            _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;

        //                            _masterMarketSize = _DL_tss_mastermarketsize.Insert(organizationService);

        //                            #region Update Status
        //                            _DL_tss_keyaccount = new DL_tss_keyaccount();
        //                            _DL_tss_keyaccount.tss_status = STATUS_CALCULATE;
        //                            _DL_tss_keyaccount.tss_reason = STATUS_REASON_CALCULATE;
        //                            _DL_tss_keyaccount.UpdateStatus(organizationService, currentUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

        //                            _DL_tss_kauio = new DL_tss_kauio();
        //                            _DL_tss_kauio.tss_calculatestatus = true;
        //                            _DL_tss_kauio.tss_errordescription = "";
        //                            _DL_tss_kauio.UpdateStatus(organizationService, currentUIO.GetAttributeValue<Guid>("tss_kauioid"));
        //                            #endregion

        //                            #region Generate Lines
        //                            GenerateMasterMSLines(organizationService, _DL_tss_mastermarketsize, pop, setup);
        //                            #endregion
        //                        }
        //                    }
        //                    else
        //                    {
        //                        List<string> _errorcollection = new List<string>();
        //                        string _errormessage = "";

        //                        //Generate MS UIO/Non UIO
        //                        QueryExpression qPro = new QueryExpression(_DL_product.EntityName);
        //                        qPro.Criteria.AddCondition("productid", ConditionOperator.Equal, pop.GetAttributeValue<EntityReference>("trs_productmaster").Id);
        //                        qPro.ColumnSet.AddColumn("tss_usetyre");
        //                        bool useTyre = _DL_product.Select(organizationService, qPro).Entities.FirstOrDefault().GetAttributeValue<bool>("tss_usetyre");

        //                        if (!pop.Contains("tss_mscurrenthourmeter"))
        //                            _errorcollection.Add("Current Hour Meter");

        //                        if (!pop.Contains("tss_mscurrenthourmeterdate"))
        //                            _errorcollection.Add("Current Hour Meter (Date)");

        //                        if (!pop.Contains("tss_mslasthourmeter"))
        //                            _errorcollection.Add("Last Hour Meter");

        //                        if (!pop.Contains("tss_mslasthourmeterdate"))
        //                            _errorcollection.Add("Last Hour Meter (Date)");

        //                        if (!pop.Contains("new_deliverydate"))
        //                            _errorcollection.Add("Delivery Date");

        //                        if (_errorcollection.Count() > 0)
        //                        {
        //                            foreach (var item in _errorcollection)
        //                            {
        //                                _errormessage += item + ", ";
        //                            }

        //                            _errormessage = _errormessage.Substring(0, _errormessage.Length - 1) + " still not define !";
        //                        }
        //                        else
        //                            _errormessage = "Current Hour Meter, Current Hour Meter (Date), Last Hour Meter , Last Hour Meter (Date), Delivery Date or Overhaul Date still not define";

        //                        _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();

        //                        _DL_tss_mastermarketsize.tss_pss = currentUIO.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                        _DL_tss_mastermarketsize.tss_customer = currentUIO.GetAttributeValue<EntityReference>("tss_customer").Id; // pop.GetAttributeValue<EntityReference>("new_customercode").Id;
        //                        _DL_tss_mastermarketsize.tss_serialnumber = pop.GetAttributeValue<Guid>("new_populationid");
        //                        if (pop.Contains("tss_populationstatus"))
        //                        {
        //                            _DL_tss_mastermarketsize.tss_unittype = pop.GetAttributeValue<bool>("tss_populationstatus") ? UNITTYPE_UIO : UNITTYPE_NONUIO;
        //                        }
        //                        _DL_tss_mastermarketsize.tss_status = STATUS_ERROR_MS;
        //                        _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_ERROR_MS;
        //                        _DL_tss_mastermarketsize.tss_errormessage = _errormessage;
        //                        _DL_tss_mastermarketsize.tss_usetyre = useTyre;
        //                        _DL_tss_mastermarketsize.tss_msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                        _DL_tss_mastermarketsize.tss_msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend");
        //                        _DL_tss_mastermarketsize.tss_activeperiodstart = currentKA.GetAttributeValue<DateTime>("tss_activestartdate");
        //                        _DL_tss_mastermarketsize.tss_activeperiodsend = currentKA.GetAttributeValue<DateTime>("tss_activeenddate");
        //                        _DL_tss_mastermarketsize.tss_keyaccountid = currentKA.Id;
        //                        _DL_tss_mastermarketsize.tss_msperiod = currentKA.GetAttributeValue<int>("tss_revision");
        //                        _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;

        //                        _DL_tss_mastermarketsize.Insert(organizationService);

        //                        #region Update Status
        //                        _DL_tss_keyaccount = new DL_tss_keyaccount();
        //                        _DL_tss_keyaccount.tss_status = STATUS_ERROR;
        //                        _DL_tss_keyaccount.UpdateStatus(organizationService, currentUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

        //                        _DL_tss_kauio = new DL_tss_kauio();
        //                        _DL_tss_kauio.tss_calculatestatus = false;
        //                        _DL_tss_kauio.tss_errordescription = _errormessage;
        //                        _DL_tss_kauio.UpdateStatus(organizationService, currentUIO.GetAttributeValue<Guid>("tss_kauioid"));
        //                        #endregion
        //                    }
        //                }
        //            }
        //        }
        //        #endregion

        //        #endregion

        //        #region Generate KA Group UIO Commodity
        //        #region Get KA Group UIO Commodity
        //        //Get KA UIO
        //        FilterExpression fkaUIOCommodity = new FilterExpression(LogicalOperator.And);
        //        fkaUIOCommodity.AddCondition("tss_keyaccountid", ConditionOperator.In, kaIds);
        //        fkaUIOCommodity.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true);
        //        fkaUIOCommodity.AddCondition("tss_groupuiocommodity", ConditionOperator.NotNull);

        //        QueryExpression qkaUIOCommodity = new QueryExpression(_DL_tss_kagroupuiocommodity.EntityName);
        //        qkaUIOCommodity.ColumnSet = new ColumnSet(true);
        //        qkaUIOCommodity.Criteria.AddFilter(fkaUIOCommodity);
        //        EntityCollection listKAGroupUIOCommodity = _DL_tss_kagroupuiocommodity.Select(organizationService, qkaUIOCommodity);
        //        #endregion

        //        #region Generate
        //        if (listKAGroupUIOCommodity.Entities.Count > 0)
        //        {
        //            #region Get Group Commodity Account
        //            object[] kaGroupUIOCommIds = listKAGroupUIOCommodity.Entities.Select(x => (object)x.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id).ToArray();

        //            FilterExpression fGroupAccount = new FilterExpression(LogicalOperator.And);
        //            fGroupAccount.AddCondition("tss_groupuiocommodityaccountid", ConditionOperator.In, kaGroupUIOCommIds);
        //            fGroupAccount.AddCondition("tss_groupuiocommodityheader", ConditionOperator.NotNull);

        //            QueryExpression qGroupAccount = new QueryExpression(_DL_tss_groupuiocommodityaccount.EntityName);
        //            qGroupAccount.ColumnSet = new ColumnSet(true);
        //            qGroupAccount.Criteria.AddFilter(fGroupAccount);
        //            EntityCollection listGroupAccount = _DL_tss_groupuiocommodityaccount.Select(organizationService, qGroupAccount);
        //            #endregion

        //            if (listGroupAccount.Entities.Count > 0)
        //            {
        //                foreach (var groupAccount in listGroupAccount.Entities)
        //                {
        //                    Guid _masterMarketSize = new Guid();
        //                    Entity currentKAGroupUIO = listKAGroupUIOCommodity.Entities.Where(x => x.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id == groupAccount.GetAttributeValue<Guid>("tss_groupuiocommodityaccountid")).FirstOrDefault();
        //                    Entity currentKA = listKeyAccount.Entities.Where(x => x.Id == currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id).FirstOrDefault();

        //                    if (groupAccount.Contains("tss_qty") && groupAccount.GetAttributeValue<int>("tss_qty") > 0)
        //                    {
        //                        _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        //                        _DL_tss_mastermarketsize.tss_pss = currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                        _DL_tss_mastermarketsize.tss_customer = currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_customer").Id;
        //                        //_DL_tss_mastermarketsize.tss_groupuiocommodity = groupAccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityname").Id;
        //                        _DL_tss_mastermarketsize.tss_groupuiocommodityheader = groupAccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id;
        //                        _DL_tss_mastermarketsize.tss_qty = groupAccount.GetAttributeValue<int>("tss_qty");
        //                        _DL_tss_mastermarketsize.tss_unittype = UNITTYPE_COMMODITY;
        //                        _DL_tss_mastermarketsize.tss_status = STATUS_COMPLETED_MS;
        //                        _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_COMPLETED_MS;
        //                        _DL_tss_mastermarketsize.tss_msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                        _DL_tss_mastermarketsize.tss_msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend");
        //                        _DL_tss_mastermarketsize.tss_activeperiodstart = currentKA.GetAttributeValue<DateTime>("tss_activestartdate");
        //                        _DL_tss_mastermarketsize.tss_activeperiodsend = currentKA.GetAttributeValue<DateTime>("tss_activeenddate");
        //                        _DL_tss_mastermarketsize.tss_keyaccountid = currentKA.Id;
        //                        _DL_tss_mastermarketsize.tss_msperiod = currentKA.GetAttributeValue<int>("tss_revision");
        //                        _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;

        //                        _masterMarketSize = _DL_tss_mastermarketsize.Insert(organizationService);

        //                        #region Update Status
        //                        _DL_tss_keyaccount = new DL_tss_keyaccount();
        //                        _DL_tss_keyaccount.tss_status = STATUS_CALCULATE;
        //                        _DL_tss_keyaccount.UpdateStatus(organizationService, currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

        //                        _DL_tss_kagroupuiocommodity = new DL_tss_kagroupuiocommodity();
        //                        _DL_tss_kagroupuiocommodity.tss_calculatestatus = true;
        //                        _DL_tss_kagroupuiocommodity.UpdateStatus(organizationService, currentKAGroupUIO.GetAttributeValue<Guid>("tss_kagroupuiocommodityid"));
        //                        #endregion

        //                        #region Generate Sub Lines
        //                        Entity _tss_groupuiocommodityaccount = organizationService.Retrieve("tss_groupuiocommodityaccount", currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id, new ColumnSet(true));

        //                        if (_tss_groupuiocommodityaccount != null)
        //                        {
        //                            Entity _tss_groupuiocommodityheader = organizationService.Retrieve("tss_groupuiocommodityheader", _tss_groupuiocommodityaccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id, new ColumnSet(true));

        //                            FilterExpression fGroupUIOCommodity = new FilterExpression(LogicalOperator.And);
        //                            fGroupUIOCommodity.AddCondition("tss_groupuiocommodityheaderid", ConditionOperator.In, _tss_groupuiocommodityheader.Id);

        //                            QueryExpression qGroupUIOCommodity = new QueryExpression(_DL_tss_groupuiocommodity.EntityName);
        //                            qGroupUIOCommodity.ColumnSet = new ColumnSet(true);
        //                            qGroupUIOCommodity.Criteria.AddFilter(fGroupUIOCommodity);
        //                            EntityCollection listGroupUIOCommodity = _DL_tss_groupuiocommodity.Select(organizationService, qGroupUIOCommodity);

        //                            if (listGroupUIOCommodity.Entities.Count > 0)
        //                            {
        //                                foreach (var group in listGroupUIOCommodity.Entities)
        //                                {
        //                                    Entity _tss_groupuiocommodity = organizationService.Retrieve("tss_groupuiocommodity", group.Id, new ColumnSet(true));

        //                                    if (_tss_groupuiocommodity != null)
        //                                    {
        //                                        GenerateMasterMSSubLinesCommodity(organizationService, _DL_tss_mastermarketsize, _tss_groupuiocommodity, groupAccount, currentKAGroupUIO);
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        #endregion
        //                    }
        //                    else
        //                    {
        //                        _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        //                        _DL_tss_mastermarketsize.tss_pss = currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                        _DL_tss_mastermarketsize.tss_customer = currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_customer").Id;
        //                        //_DL_tss_mastermarketsize.tss_groupuiocommodity = groupAccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityname").Id;
        //                        _DL_tss_mastermarketsize.tss_groupuiocommodityheader = groupAccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id;
        //                        _DL_tss_mastermarketsize.tss_unittype = UNITTYPE_COMMODITY;
        //                        _DL_tss_mastermarketsize.tss_status = STATUS_ERROR_MS;
        //                        _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_ERROR_MS;
        //                        _DL_tss_mastermarketsize.tss_errormessage = "Qty still not define";
        //                        _DL_tss_mastermarketsize.tss_msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                        _DL_tss_mastermarketsize.tss_msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend");
        //                        _DL_tss_mastermarketsize.tss_activeperiodstart = currentKA.GetAttributeValue<DateTime>("tss_activestartdate");
        //                        _DL_tss_mastermarketsize.tss_activeperiodsend = currentKA.GetAttributeValue<DateTime>("tss_activeenddate");
        //                        _DL_tss_mastermarketsize.tss_keyaccountid = currentKA.Id;
        //                        _DL_tss_mastermarketsize.tss_msperiod = currentKA.GetAttributeValue<int>("tss_revision");
        //                        _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;

        //                        _DL_tss_mastermarketsize.Insert(organizationService);

        //                        #region Update Status
        //                        _DL_tss_keyaccount = new DL_tss_keyaccount();
        //                        _DL_tss_keyaccount.tss_status = STATUS_ERROR;
        //                        _DL_tss_keyaccount.UpdateStatus(organizationService, currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

        //                        _DL_tss_kagroupuiocommodity = new DL_tss_kagroupuiocommodity();
        //                        _DL_tss_kagroupuiocommodity.tss_calculatestatus = false;
        //                        _DL_tss_kagroupuiocommodity.UpdateStatus(organizationService, currentKAGroupUIO.GetAttributeValue<Guid>("tss_kagroupuiocommodityid"));
        //                        #endregion
        //                    }
        //                }
        //            }
        //        }
        //        #endregion
        //        #endregion

        //        //Guid[] customerID = listKeyAccount.Entities.GroupBy(x => x.GetAttributeValue<EntityReference>("tss_customer").Id).Select(x => x.First().GetAttributeValue<EntityReference>("tss_customer").Id).ToArray();

        //        //BL_tss_marketsizeresultpss _BL_tss_marketsizeresultpss = new BL_tss_marketsizeresultpss();
        //        //_BL_tss_marketsizeresultpss.GenerateMarketSizeResultPSS_New(organizationService, context, customerID);

        //        BL_tss_marketsizesummarybypartnumber _BL_tss_marketsizesummarybypartnumber = new BL_tss_marketsizesummarybypartnumber();
        //        _BL_tss_marketsizesummarybypartnumber.GenerateMarketSizeSummaryByPartNumber(organizationService, context);

        //        BL_tss_marketsizesummarybygroupuiocommodity _BL_tss_marketsizesummarybygroupuiocommodity = new BL_tss_marketsizesummarybygroupuiocommodity();
        //        _BL_tss_marketsizesummarybygroupuiocommodity.GenerateMarketSizeSummaryByGroupUIOCommodity(organizationService, context, listKeyAccount);
        //    }
        //    else
        //    {
        //        throw new InvalidWorkflowException("KAUIO or KA Group UIO Commodity not exist.");
        //    }
        //}




















        //public void GenerateMasterMS(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context, EntityCollection listKeyAccount)
        //{
        //    try
        //    {
        //        ExecuteMultipleRequest requestWithResults = new ExecuteMultipleRequest()
        //        {
        //            // Assign settings that define execution behavior: continue on error, return responses. 
        //            Settings = new ExecuteMultipleSettings()
        //            {
        //                ContinueOnError = false,
        //                ReturnResponses = true
        //            },
        //            // Create an empty organization request collection.
        //            Requests = new OrganizationRequestCollection()
        //        };

        //        EntityCollection input = GetCollectionOfEntitiesToCreate(organizationService, tracingService, context, listKeyAccount);

        //        // Add a CreateRequest for each entity to the request collection.
        //        foreach (var entity in input.Entities)
        //        {
        //            CreateRequest createRequest = new CreateRequest { Target = entity };
        //            requestWithResults.Requests.Add(createRequest);
        //        }

        //        // Execute all the requests in the request collection using a single web method call.
        //        ExecuteMultipleResponse responseWithResults =
        //            (ExecuteMultipleResponse)organizationService.Execute(requestWithResults);

        //        if (responseWithResults.IsFaulted)
        //        {
        //            throw new InvalidWorkflowException("Generate Master Market Size FAILED !");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new InvalidWorkflowException(e.Message.ToString());
        //    }
        //}




        //public void GenerateMasterMS(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context, EntityCollection listKeyAccount)
        //{
        //    try
        //    {
        //        const int maxItemsPerBatch = 1000;
        //        const int maxBatches = 100;

        //        // Create several (local, in memory) entities in a collection. 
        //        EntityCollection input = GetCollectionOfEntitiesToCreate(organizationService, tracingService, context, listKeyAccount);

        //        Stopwatch sw = new Stopwatch();
        //        sw.Start();

        //        List<System.Threading.Tasks.Task> tasks = new List<System.Threading.Tasks.Task>();

        //        for (int j = 0; j < maxBatches; j++)
        //        {
        //            var t1 = System.Threading.Tasks.Task.Factory.StartNew(() =>
        //            {
        //                ExecuteMultipleRequest requestWithResults = new ExecuteMultipleRequest()
        //                {
        //                    // Assign settings that define execution behavior: continue on error, return responses. 
        //                    Settings = new ExecuteMultipleSettings()
        //                    {
        //                        ContinueOnError = false,
        //                        ReturnResponses = true
        //                    },
        //                    // Create an empty organization request collection.
        //                    Requests = new OrganizationRequestCollection()
        //                };



        //                for (int i = 0; i < maxItemsPerBatch; i++)
        //                {
        //                    // Add a CreateRequest for each entity to the request collection.
        //                    foreach (var entity in input.Entities)
        //                    {
        //                        CreateRequest createRequest = new CreateRequest { Target = entity };
        //                        requestWithResults.Requests.Add(createRequest);
        //                    }
        //                }

        //                // Execute all the requests in the request collection using a single web method call.
        //                ExecuteMultipleResponse responseWithResults =
        //                    (ExecuteMultipleResponse)organizationService.Execute(requestWithResults);

        //                if (responseWithResults.IsFaulted)
        //                {
        //                    throw new InvalidWorkflowException("Generate Master Market Size FAILED !");
        //                }
        //            });

        //            tasks.Add(t1);
        //        }

        //        System.Threading.Tasks.Task.WhenAll(tasks).Wait();

        //        //foreach (var responseItem in responseWithResults.Responses)
        //        //{
        //        //    // A valid response.
        //        //    if (responseItem.Response != null)
        //        //        tracingService.Trace(responseItem.Response.ToString());
        //        //    //throw new InvalidWorkflowException(responseItem.Response.ToString());

        //        //    // An error has occurred.
        //        //    else if (responseItem.Fault != null)
        //        //        //tracingService.Trace(responseItem.Fault.ToString());
        //        //        throw new InvalidWorkflowException(responseItem.Fault.ToString());
        //        //}
        //    }
        //    catch (Exception e)
        //    {
        //        throw new InvalidWorkflowException(e.Message.ToString());
        //    }
        //}




        //public EntityCollection GetCollectionOfEntitiesToCreate(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context, EntityCollection listKeyAccount)
        //{
        //    EntityCollection _result = new EntityCollection();

        //    #region Get Key Account
        //    if (listKeyAccount == null)
        //    {
        //        FilterExpression fStatus = new FilterExpression(LogicalOperator.Or);
        //        fStatus.AddCondition("tss_status", ConditionOperator.Equal, STATUS_OPEN);
        //        fStatus.AddCondition("tss_status", ConditionOperator.Equal, STATUS_ERROR);

        //        FilterExpression fKA = new FilterExpression(LogicalOperator.And);
        //        fKA.AddFilter(fStatus);
        //        fKA.AddCondition("tss_pss", ConditionOperator.Equal, context.UserId);
        //        fKA.AddCondition("tss_customer", ConditionOperator.NotNull);
        //        fKA.AddCondition("tss_activeenddate", ConditionOperator.GreaterEqual, DateTime.Today);
        //        fKA.AddCondition("tss_activestartdate", ConditionOperator.LessEqual, DateTime.Today);
        //        fKA.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true); //2018.10.12

        //        QueryExpression qKA = new QueryExpression(_DL_tss_keyaccount.EntityName);
        //        qKA.Criteria.AddFilter(fKA);
        //        qKA.ColumnSet = new ColumnSet(true);
        //        //EntityCollection listKeyAccount = _DL_tss_mastermarketsize.Select(organizationService, qKA);
        //        listKeyAccount = organizationService.RetrieveMultiple(qKA); // _DL_tss_mastermarketsize.Select(organizationService, qKA);
        //    }
        //    #endregion

        //    if (listKeyAccount.Entities.Count > 0)
        //    {
        //        #region Generate MS KAUIO
        //        #region Get KA UIO
        //        //Get KA UIO
        //        object[] kaIds = listKeyAccount.Entities.Select(x => (object)x.Id).ToArray();
        //        FilterExpression fKAUIO = new FilterExpression(LogicalOperator.And);
        //        fKAUIO.AddCondition("tss_keyaccountid", ConditionOperator.In, kaIds);
        //        fKAUIO.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true);
        //        fKAUIO.AddCondition("tss_serialnumber", ConditionOperator.NotNull);

        //        QueryExpression qKAUIO = new QueryExpression(_DL_tss_kauio.EntityName);
        //        qKAUIO.ColumnSet = new ColumnSet(true);
        //        qKAUIO.Criteria.AddFilter(fKAUIO);
        //        EntityCollection listKAUIO = organizationService.RetrieveMultiple(qKAUIO); // _DL_tss_kauio.Select(organizationService, qKAUIO);
        //        #endregion

        //        #region Generatex
        //        //Check Population
        //        if (listKAUIO.Entities.Count > 0)
        //        {
        //            object[] serialNums = listKAUIO.Entities.Select(x => (object)x.GetAttributeValue<EntityReference>("tss_serialnumber").Id).ToArray();

        //            FilterExpression fPop = new FilterExpression(LogicalOperator.And);
        //            fPop.AddCondition("new_populationid", ConditionOperator.In, serialNums);
        //            fPop.AddCondition("trs_productmaster", ConditionOperator.NotNull);

        //            QueryExpression qPop = new QueryExpression(_DL_population.EntityName);
        //            qPop.ColumnSet = new ColumnSet(true);
        //            qPop.Criteria.AddFilter(fPop);
        //            EntityCollection listPop = organizationService.RetrieveMultiple(qPop); // _DL_population.Select(organizationService, qPop);

        //            if (listPop.Entities.Count > 0)
        //            {
        //                List<Guid> msIds = new List<Guid>();

        //                foreach (var pop in listPop.Entities)
        //                {
        //                    Guid _masterMarketSize = new Guid();
        //                    Entity currentUIO = listKAUIO.Entities.Where(x => x.GetAttributeValue<EntityReference>("tss_serialnumber").Id == pop.GetAttributeValue<Guid>("new_populationid")).FirstOrDefault();
        //                    Entity currentKA = listKeyAccount.Entities.Where(x => x.Id == currentUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id).FirstOrDefault();

        //                    bool Method1 = false;
        //                    bool Method2 = false;
        //                    bool Method3 = false;
        //                    bool Method4 = false;
        //                    bool Method5 = false;

        //                    //CHECK METHOD 1
        //                    if (pop.Attributes.Contains("tss_mscurrenthourmeter")
        //                        && pop.Attributes.Contains("tss_mslasthourmeter")
        //                        && pop.Attributes.Contains("tss_mscurrenthourmeterdate")
        //                        && pop.Attributes.Contains("tss_mslasthourmeterdate"))
        //                    {
        //                        Method1 = true;
        //                        Console.Write("Method 1 : " + Method1);
        //                    }

        //                    //CHECK METHOD 2
        //                    if (pop.Attributes.Contains("tss_mscurrenthourmeter")
        //                        && pop.Attributes.Contains("tss_mscurrenthourmeterdate")
        //                        && (pop.Attributes.Contains("new_deliverydate") || pop.Attributes.Contains("trs_warrantyenddate")))
        //                    {
        //                        Method2 = true;
        //                        Console.Write("Method 2 : " + Method2);
        //                    }

        //                    //CHECK METHOD 3
        //                    if (pop.Attributes.Contains("tss_estworkinghour"))
        //                    {
        //                        Method3 = true;
        //                        Console.Write("Method 3 : " + Method3);
        //                    }

        //                    //CHECK METHOD 4
        //                    if (pop.Attributes.Contains("new_deliverydate")
        //                        && currentKA.Attributes.Contains("tss_msperiodstart")
        //                        && currentKA.Attributes.Contains("tss_msperiodend"))
        //                    {
        //                        Method4 = true;
        //                        Console.Write("Method 4 : " + Method4);
        //                    }

        //                    //CHECK METHOD 5
        //                    if ((pop.Attributes.Contains("new_deliverydate") || pop.Attributes.Contains("trs_warrantyenddate"))
        //                        && currentKA.Attributes.Contains("tss_msperiodstart")
        //                        && currentKA.Attributes.Contains("tss_msperiodend"))
        //                    {
        //                        Method5 = true;
        //                        Console.Write("Method 5 : " + Method5);
        //                    }

        //                    if (Method1 || Method2 || Method3 || Method4 || Method5)
        //                    {
        //                        //Generate MS UIO/Non UIO
        //                        decimal currentHM;
        //                        decimal lastHM;
        //                        DateTime currentHMDate;
        //                        DateTime lastHMDate;
        //                        DateTime deliveryDate;
        //                        DateTime warrantyEndDate = DateTime.MinValue;
        //                        DateTime calculateMarketSizeDate = DateTime.Now.ToLocalTime().Date;

        //                        QueryExpression qPro = new QueryExpression(_DL_product.EntityName);
        //                        qPro.Criteria.AddCondition("productid", ConditionOperator.Equal, pop.GetAttributeValue<EntityReference>("trs_productmaster").Id);
        //                        qPro.ColumnSet.AddColumn("tss_usetyre");
        //                        bool useTyre = organizationService.RetrieveMultiple(qPop).Entities.FirstOrDefault().GetAttributeValue<bool>("tss_usetyre"); // _DL_product.Select(organizationService, qPro).Entities.FirstOrDefault().GetAttributeValue<bool>("tss_usetyre");

        //                        QueryExpression qSetup = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
        //                        qSetup.ColumnSet = new ColumnSet(true);
        //                        EntityCollection setups = organizationService.RetrieveMultiple(qSetup); // _DL_tss_sparepartsetup.Select(organizationService, qSetup);

        //                        if (setups.Entities.Count > 0)
        //                        {
        //                            Entity setup = setups.Entities[0];
        //                            _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        //                            _DL_tss_mastermarketsize.tss_pss = currentUIO.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                            _DL_tss_mastermarketsize.tss_customer = currentUIO.GetAttributeValue<EntityReference>("tss_customer").Id; // pop.GetAttributeValue<EntityReference>("new_customercode").Id;
        //                            _DL_tss_mastermarketsize.tss_serialnumber = pop.GetAttributeValue<Guid>("new_populationid");

        //                            //METHOD 1
        //                            if (pop.Contains("tss_mscurrenthourmeter")
        //                               && pop.Contains("tss_mslasthourmeter")
        //                               && pop.Contains("tss_mscurrenthourmeterdate")
        //                               && pop.Contains("tss_mslasthourmeterdate"))
        //                            {
        //                                currentHM = pop.GetAttributeValue<decimal>("tss_mscurrenthourmeter");
        //                                lastHM = pop.GetAttributeValue<decimal>("tss_mslasthourmeter");
        //                                currentHMDate = pop.GetAttributeValue<DateTime>("tss_mscurrenthourmeterdate").ToLocalTime().Date;
        //                                lastHMDate = pop.GetAttributeValue<DateTime>("tss_mslasthourmeterdate").ToLocalTime().Date;

        //                                //if ((decimal)(currentHMDate - lastHMDate).TotalDays == 0)
        //                                //    _DL_tss_mastermarketsize.tss_avghmmethod1 = 0;
        //                                //else
        //                                //    _DL_tss_mastermarketsize.tss_avghmmethod1 = avghmmethod1 > (decimal)24 ? (decimal)24 : 0;

        //                                if ((decimal)(currentHMDate - lastHMDate).TotalDays > 0)
        //                                {
        //                                    decimal avghmmethod1 = (currentHM - lastHM) / ((decimal)(currentHMDate - lastHMDate).TotalDays);

        //                                    if (avghmmethod1 > 0)
        //                                    {
        //                                        _DL_tss_mastermarketsize.tss_avghmmethod1 = avghmmethod1 > (decimal)24 ? (decimal)24 : avghmmethod1;
        //                                    }
        //                                }
        //                            }

        //                            //METHOD 2
        //                            if (pop.Contains("tss_mscurrenthourmeter")
        //                               && pop.Contains("tss_mscurrenthourmeterdate")
        //                               && (pop.Contains("new_deliverydate") || pop.Contains("trs_warrantyenddate")))
        //                            {
        //                                currentHM = pop.GetAttributeValue<decimal>("tss_mscurrenthourmeter");
        //                                currentHMDate = pop.GetAttributeValue<DateTime>("tss_mscurrenthourmeterdate").ToLocalTime().Date;
        //                                deliveryDate = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().Date;

        //                                if (!pop.Attributes.Contains("trs_warrantyenddate"))
        //                                {
        //                                    warrantyEndDate = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().AddYears(1).Date;
        //                                }
        //                                else
        //                                {
        //                                    warrantyEndDate = pop.GetAttributeValue<DateTime>("trs_warrantyenddate").ToLocalTime().Date;
        //                                }

        //                                //if ((decimal)(currentHMDate - deliveryDate).TotalDays == 0)
        //                                //    _DL_tss_mastermarketsize.tss_avghmmethod2 = 0;
        //                                //else
        //                                //    _DL_tss_mastermarketsize.tss_avghmmethod2 = avghmmethod2 > (decimal)24 ? (decimal)24 : 0;

        //                                if ((decimal)(currentHMDate - warrantyEndDate).TotalDays > 0)
        //                                {
        //                                    decimal avghmmethod2 = currentHM / ((decimal)(currentHMDate - warrantyEndDate).TotalDays);

        //                                    if (avghmmethod2 > 0)
        //                                    {
        //                                        _DL_tss_mastermarketsize.tss_avghmmethod2 = avghmmethod2 > (decimal)24 ? (decimal)24 : avghmmethod2;
        //                                    }
        //                                }
        //                            }

        //                            //METHOD 3
        //                            if (pop.Contains("tss_estworkinghour"))
        //                            {
        //                                decimal estimateWH = pop.GetAttributeValue<int>("tss_estworkinghour");

        //                                if (estimateWH > 0)
        //                                {
        //                                    _DL_tss_mastermarketsize.tss_avghmmethod3 = estimateWH > (decimal)24 ? (decimal)24 : estimateWH;
        //                                }
        //                            }

        //                            //METHOD 4
        //                            if (pop.Contains("new_deliverydate")
        //                                && currentKA.Contains("tss_msperiodstart")
        //                                && currentKA.Contains("tss_msperiodend"))
        //                            {
        //                                CalculateDate oCalculate = new CalculateDate();

        //                                deliveryDate = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().Date;
        //                                DateTime msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart").ToLocalTime().Date;
        //                                DateTime msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend").ToLocalTime().Date;

        //                                decimal oDeliveryDate = oCalculate.DiffYear(deliveryDate, calculateMarketSizeDate);
        //                                decimal oPeriodDate = oCalculate.DiffYear(msperiodstart, msperiodend);

        //                                decimal periodpmmethod4 = (decimal)(oDeliveryDate + oPeriodDate);

        //                                if (periodpmmethod4 > 0)
        //                                {
        //                                    _DL_tss_mastermarketsize.tss_periodpmmethod4 = periodpmmethod4;
        //                                }
        //                            }

        //                            //METHOD 5
        //                            if (pop.Contains("new_deliverydate")
        //                                && currentKA.Contains("tss_msperiodstart")
        //                                && currentKA.Contains("tss_msperiodend"))
        //                            {
        //                                if (!pop.Attributes.Contains("trs_warrantyenddate"))
        //                                {
        //                                    warrantyEndDate = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().AddYears(1).Date;
        //                                }
        //                                else
        //                                {
        //                                    warrantyEndDate = pop.GetAttributeValue<DateTime>("trs_warrantyenddate").ToLocalTime().Date;
        //                                }

        //                                CalculateDate oCalculate = new CalculateDate();

        //                                deliveryDate = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().Date;
        //                                DateTime msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart").ToLocalTime().Date;
        //                                DateTime msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend").ToLocalTime().Date;

        //                                decimal oWarrantyDate = oCalculate.DiffYear(deliveryDate, warrantyEndDate);
        //                                decimal oPeriodDate = oCalculate.DiffYear(msperiodstart, msperiodend);

        //                                decimal periodpmmethod5 = (decimal)(oWarrantyDate + oPeriodDate);

        //                                if (periodpmmethod5 > 0)
        //                                {
        //                                    _DL_tss_mastermarketsize.tss_periodpmmethod5 = periodpmmethod5;
        //                                }
        //                            }

        //                            if (pop.Contains("tss_populationstatus"))
        //                            {
        //                                _DL_tss_mastermarketsize.tss_unittype = pop.GetAttributeValue<bool>("tss_populationstatus") ? UNITTYPE_UIO : UNITTYPE_NONUIO;
        //                            }
        //                            _DL_tss_mastermarketsize.tss_status = STATUS_COMPLETED_MS;
        //                            _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_COMPLETED_MS;
        //                            _DL_tss_mastermarketsize.tss_usetyre = useTyre;
        //                            _DL_tss_mastermarketsize.tss_msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                            _DL_tss_mastermarketsize.tss_msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend");
        //                            _DL_tss_mastermarketsize.tss_activeperiodstart = currentKA.GetAttributeValue<DateTime>("tss_activestartdate");
        //                            _DL_tss_mastermarketsize.tss_activeperiodsend = currentKA.GetAttributeValue<DateTime>("tss_activeenddate");
        //                            _DL_tss_mastermarketsize.tss_keyaccountid = currentKA.Id;
        //                            _DL_tss_mastermarketsize.tss_msperiod = currentKA.GetAttributeValue<int>("tss_revision");
        //                            _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;

        //                            _result.Entities.Add(_DL_tss_mastermarketsize.GetEntity(organizationService));

        //                            //_masterMarketSize = _DL_tss_mastermarketsize.Insert(organizationService);

        //                            //#region Update Status
        //                            //_DL_tss_keyaccount = new DL_tss_keyaccount();
        //                            //_DL_tss_keyaccount.tss_status = STATUS_CALCULATE;
        //                            //_DL_tss_keyaccount.tss_reason = STATUS_REASON_CALCULATE;
        //                            //_DL_tss_keyaccount.UpdateStatus(organizationService, currentUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

        //                            //_DL_tss_kauio = new DL_tss_kauio();
        //                            //_DL_tss_kauio.tss_calculatestatus = true;
        //                            //_DL_tss_kauio.tss_errordescription = "";
        //                            //_DL_tss_kauio.UpdateStatus(organizationService, currentUIO.GetAttributeValue<Guid>("tss_kauioid"));
        //                            //#endregion

        //                            //#region Generate Lines
        //                            //GenerateMasterMSLines(organizationService, _DL_tss_mastermarketsize, pop, setup);
        //                            //#endregion
        //                        }
        //                    }
        //                    else
        //                    {
        //                        List<string> _errorcollection = new List<string>();
        //                        string _errormessage = "";

        //                        //Generate MS UIO/Non UIO
        //                        QueryExpression qPro = new QueryExpression(_DL_product.EntityName);
        //                        qPro.Criteria.AddCondition("productid", ConditionOperator.Equal, pop.GetAttributeValue<EntityReference>("trs_productmaster").Id);
        //                        qPro.ColumnSet.AddColumn("tss_usetyre");
        //                        bool useTyre = organizationService.RetrieveMultiple(qPop).Entities.FirstOrDefault().GetAttributeValue<bool>("tss_usetyre"); // _DL_product.Select(organizationService, qPro).Entities.FirstOrDefault().GetAttributeValue<bool>("tss_usetyre");

        //                        if (!pop.Contains("tss_mscurrenthourmeter"))
        //                            _errorcollection.Add("Current Hour Meter");

        //                        if (!pop.Contains("tss_mscurrenthourmeterdate"))
        //                            _errorcollection.Add("Current Hour Meter (Date)");

        //                        if (!pop.Contains("tss_mslasthourmeter"))
        //                            _errorcollection.Add("Last Hour Meter");

        //                        if (!pop.Contains("tss_mslasthourmeterdate"))
        //                            _errorcollection.Add("Last Hour Meter (Date)");

        //                        if (!pop.Contains("new_deliverydate"))
        //                            _errorcollection.Add("Delivery Date");

        //                        if (_errorcollection.Count() > 0)
        //                        {
        //                            foreach (var item in _errorcollection)
        //                            {
        //                                _errormessage += item + ", ";
        //                            }

        //                            _errormessage = _errormessage.Substring(0, _errormessage.Length - 1) + " still not define !";
        //                        }
        //                        else
        //                            _errormessage = "Current Hour Meter, Current Hour Meter (Date), Last Hour Meter , Last Hour Meter (Date), Delivery Date or Overhaul Date still not define";

        //                        _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();

        //                        _DL_tss_mastermarketsize.tss_pss = currentUIO.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                        _DL_tss_mastermarketsize.tss_customer = currentUIO.GetAttributeValue<EntityReference>("tss_customer").Id; // pop.GetAttributeValue<EntityReference>("new_customercode").Id;
        //                        _DL_tss_mastermarketsize.tss_serialnumber = pop.GetAttributeValue<Guid>("new_populationid");
        //                        if (pop.Contains("tss_populationstatus"))
        //                        {
        //                            _DL_tss_mastermarketsize.tss_unittype = pop.GetAttributeValue<bool>("tss_populationstatus") ? UNITTYPE_UIO : UNITTYPE_NONUIO;
        //                        }
        //                        _DL_tss_mastermarketsize.tss_status = STATUS_ERROR_MS;
        //                        _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_ERROR_MS;
        //                        _DL_tss_mastermarketsize.tss_errormessage = _errormessage;
        //                        _DL_tss_mastermarketsize.tss_usetyre = useTyre;
        //                        _DL_tss_mastermarketsize.tss_msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                        _DL_tss_mastermarketsize.tss_msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend");
        //                        _DL_tss_mastermarketsize.tss_activeperiodstart = currentKA.GetAttributeValue<DateTime>("tss_activestartdate");
        //                        _DL_tss_mastermarketsize.tss_activeperiodsend = currentKA.GetAttributeValue<DateTime>("tss_activeenddate");
        //                        _DL_tss_mastermarketsize.tss_keyaccountid = currentKA.Id;
        //                        _DL_tss_mastermarketsize.tss_msperiod = currentKA.GetAttributeValue<int>("tss_revision");
        //                        _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;

        //                        _result.Entities.Add(_DL_tss_mastermarketsize.GetEntity(organizationService));

        //                        //_DL_tss_mastermarketsize.Insert(organizationService);

        //                        //#region Update Status
        //                        //_DL_tss_keyaccount = new DL_tss_keyaccount();
        //                        //_DL_tss_keyaccount.tss_status = STATUS_ERROR;
        //                        //_DL_tss_keyaccount.UpdateStatus(organizationService, currentUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

        //                        //_DL_tss_kauio = new DL_tss_kauio();
        //                        //_DL_tss_kauio.tss_calculatestatus = false;
        //                        //_DL_tss_kauio.tss_errordescription = _errormessage;
        //                        //_DL_tss_kauio.UpdateStatus(organizationService, currentUIO.GetAttributeValue<Guid>("tss_kauioid"));
        //                        //#endregion
        //                    }
        //                }
        //            }
        //        }
        //        #endregion

        //        #endregion

        //        //#region Generate KA Group UIO Commodity
        //        //#region Get KA Group UIO Commodity
        //        ////Get KA UIO
        //        //FilterExpression fkaUIOCommodity = new FilterExpression(LogicalOperator.And);
        //        //fkaUIOCommodity.AddCondition("tss_keyaccountid", ConditionOperator.In, kaIds);
        //        //fkaUIOCommodity.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true);
        //        //fkaUIOCommodity.AddCondition("tss_groupuiocommodity", ConditionOperator.NotNull);

        //        //QueryExpression qkaUIOCommodity = new QueryExpression(_DL_tss_kagroupuiocommodity.EntityName);
        //        //qkaUIOCommodity.ColumnSet = new ColumnSet(true);
        //        //qkaUIOCommodity.Criteria.AddFilter(fkaUIOCommodity);
        //        //EntityCollection listKAGroupUIOCommodity = _DL_tss_kagroupuiocommodity.Select(organizationService, qkaUIOCommodity);
        //        //#endregion

        //        //#region Generate
        //        //if (listKAGroupUIOCommodity.Entities.Count > 0)
        //        //{
        //        //    #region Get Group Commodity Account
        //        //    object[] kaGroupUIOCommIds = listKAGroupUIOCommodity.Entities.Select(x => (object)x.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id).ToArray();

        //        //    FilterExpression fGroupAccount = new FilterExpression(LogicalOperator.And);
        //        //    fGroupAccount.AddCondition("tss_groupuiocommodityaccountid", ConditionOperator.In, kaGroupUIOCommIds);
        //        //    fGroupAccount.AddCondition("tss_groupuiocommodityheader", ConditionOperator.NotNull);

        //        //    QueryExpression qGroupAccount = new QueryExpression(_DL_tss_groupuiocommodityaccount.EntityName);
        //        //    qGroupAccount.ColumnSet = new ColumnSet(true);
        //        //    qGroupAccount.Criteria.AddFilter(fGroupAccount);
        //        //    EntityCollection listGroupAccount = _DL_tss_groupuiocommodityaccount.Select(organizationService, qGroupAccount);
        //        //    #endregion

        //        //    if (listGroupAccount.Entities.Count > 0)
        //        //    {
        //        //        foreach (var groupAccount in listGroupAccount.Entities)
        //        //        {
        //        //            Guid _masterMarketSize = new Guid();
        //        //            Entity currentKAGroupUIO = listKAGroupUIOCommodity.Entities.Where(x => x.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id == groupAccount.GetAttributeValue<Guid>("tss_groupuiocommodityaccountid")).FirstOrDefault();
        //        //            Entity currentKA = listKeyAccount.Entities.Where(x => x.Id == currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id).FirstOrDefault();

        //        //            if (groupAccount.Contains("tss_qty") && groupAccount.GetAttributeValue<int>("tss_qty") > 0)
        //        //            {
        //        //                _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        //        //                _DL_tss_mastermarketsize.tss_pss = currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_pss").Id;
        //        //                _DL_tss_mastermarketsize.tss_customer = currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_customer").Id;
        //        //                //_DL_tss_mastermarketsize.tss_groupuiocommodity = groupAccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityname").Id;
        //        //                _DL_tss_mastermarketsize.tss_groupuiocommodityheader = groupAccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id;
        //        //                _DL_tss_mastermarketsize.tss_qty = groupAccount.GetAttributeValue<int>("tss_qty");
        //        //                _DL_tss_mastermarketsize.tss_unittype = UNITTYPE_COMMODITY;
        //        //                _DL_tss_mastermarketsize.tss_status = STATUS_COMPLETED_MS;
        //        //                _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_COMPLETED_MS;
        //        //                _DL_tss_mastermarketsize.tss_msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart");
        //        //                _DL_tss_mastermarketsize.tss_msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend");
        //        //                _DL_tss_mastermarketsize.tss_activeperiodstart = currentKA.GetAttributeValue<DateTime>("tss_activestartdate");
        //        //                _DL_tss_mastermarketsize.tss_activeperiodsend = currentKA.GetAttributeValue<DateTime>("tss_activeenddate");
        //        //                _DL_tss_mastermarketsize.tss_keyaccountid = currentKA.Id;
        //        //                _DL_tss_mastermarketsize.tss_msperiod = currentKA.GetAttributeValue<int>("tss_revision");
        //        //                _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;

        //        //                _masterMarketSize = _DL_tss_mastermarketsize.Insert(organizationService);

        //        //                #region Update Status
        //        //                _DL_tss_keyaccount = new DL_tss_keyaccount();
        //        //                _DL_tss_keyaccount.tss_status = STATUS_CALCULATE;
        //        //                _DL_tss_keyaccount.UpdateStatus(organizationService, currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

        //        //                _DL_tss_kagroupuiocommodity = new DL_tss_kagroupuiocommodity();
        //        //                _DL_tss_kagroupuiocommodity.tss_calculatestatus = true;
        //        //                _DL_tss_kagroupuiocommodity.UpdateStatus(organizationService, currentKAGroupUIO.GetAttributeValue<Guid>("tss_kagroupuiocommodityid"));
        //        //                #endregion

        //        //                #region Generate Sub Lines
        //        //                Entity _tss_groupuiocommodityaccount = organizationService.Retrieve("tss_groupuiocommodityaccount", currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id, new ColumnSet(true));

        //        //                if (_tss_groupuiocommodityaccount != null)
        //        //                {
        //        //                    Entity _tss_groupuiocommodityheader = organizationService.Retrieve("tss_groupuiocommodityheader", _tss_groupuiocommodityaccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id, new ColumnSet(true));

        //        //                    FilterExpression fGroupUIOCommodity = new FilterExpression(LogicalOperator.And);
        //        //                    fGroupUIOCommodity.AddCondition("tss_groupuiocommodityheaderid", ConditionOperator.In, _tss_groupuiocommodityheader.Id);

        //        //                    QueryExpression qGroupUIOCommodity = new QueryExpression(_DL_tss_groupuiocommodity.EntityName);
        //        //                    qGroupUIOCommodity.ColumnSet = new ColumnSet(true);
        //        //                    qGroupUIOCommodity.Criteria.AddFilter(fGroupUIOCommodity);
        //        //                    EntityCollection listGroupUIOCommodity = _DL_tss_groupuiocommodity.Select(organizationService, qGroupUIOCommodity);

        //        //                    if (listGroupUIOCommodity.Entities.Count > 0)
        //        //                    {
        //        //                        foreach (var group in listGroupUIOCommodity.Entities)
        //        //                        {
        //        //                            Entity _tss_groupuiocommodity = organizationService.Retrieve("tss_groupuiocommodity", group.Id, new ColumnSet(true));

        //        //                            if (_tss_groupuiocommodity != null)
        //        //                            {
        //        //                                GenerateMasterMSSubLinesCommodity(organizationService, _DL_tss_mastermarketsize, _tss_groupuiocommodity, groupAccount, currentKAGroupUIO);
        //        //                            }
        //        //                        }
        //        //                    }
        //        //                }
        //        //                #endregion
        //        //            }
        //        //            else
        //        //            {
        //        //                _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        //        //                _DL_tss_mastermarketsize.tss_pss = currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_pss").Id;
        //        //                _DL_tss_mastermarketsize.tss_customer = currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_customer").Id;
        //        //                //_DL_tss_mastermarketsize.tss_groupuiocommodity = groupAccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityname").Id;
        //        //                _DL_tss_mastermarketsize.tss_groupuiocommodityheader = groupAccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id;
        //        //                _DL_tss_mastermarketsize.tss_unittype = UNITTYPE_COMMODITY;
        //        //                _DL_tss_mastermarketsize.tss_status = STATUS_ERROR_MS;
        //        //                _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_ERROR_MS;
        //        //                _DL_tss_mastermarketsize.tss_errormessage = "Qty still not define";
        //        //                _DL_tss_mastermarketsize.tss_msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart");
        //        //                _DL_tss_mastermarketsize.tss_msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend");
        //        //                _DL_tss_mastermarketsize.tss_activeperiodstart = currentKA.GetAttributeValue<DateTime>("tss_activestartdate");
        //        //                _DL_tss_mastermarketsize.tss_activeperiodsend = currentKA.GetAttributeValue<DateTime>("tss_activeenddate");
        //        //                _DL_tss_mastermarketsize.tss_keyaccountid = currentKA.Id;
        //        //                _DL_tss_mastermarketsize.tss_msperiod = currentKA.GetAttributeValue<int>("tss_revision");
        //        //                _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;

        //        //                _DL_tss_mastermarketsize.Insert(organizationService);

        //        //                #region Update Status
        //        //                _DL_tss_keyaccount = new DL_tss_keyaccount();
        //        //                _DL_tss_keyaccount.tss_status = STATUS_ERROR;
        //        //                _DL_tss_keyaccount.UpdateStatus(organizationService, currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

        //        //                _DL_tss_kagroupuiocommodity = new DL_tss_kagroupuiocommodity();
        //        //                _DL_tss_kagroupuiocommodity.tss_calculatestatus = false;
        //        //                _DL_tss_kagroupuiocommodity.UpdateStatus(organizationService, currentKAGroupUIO.GetAttributeValue<Guid>("tss_kagroupuiocommodityid"));
        //        //                #endregion
        //        //            }
        //        //        }
        //        //    }
        //        //}
        //        //#endregion
        //        //#endregion

        //        //Guid[] customerID = listKeyAccount.Entities.GroupBy(x => x.GetAttributeValue<EntityReference>("tss_customer").Id).Select(x => x.First().GetAttributeValue<EntityReference>("tss_customer").Id).ToArray();

        //        //BL_tss_marketsizeresultpss _BL_tss_marketsizeresultpss = new BL_tss_marketsizeresultpss();
        //        //_BL_tss_marketsizeresultpss.GenerateMarketSizeResultPSS_New(organizationService, context, customerID);

        //        //BL_tss_marketsizesummarybypartnumber _BL_tss_marketsizesummarybypartnumber = new BL_tss_marketsizesummarybypartnumber();
        //        //_BL_tss_marketsizesummarybypartnumber.GenerateMarketSizeSummaryByPartNumber(organizationService, context);

        //        //BL_tss_marketsizesummarybygroupuiocommodity _BL_tss_marketsizesummarybygroupuiocommodity = new BL_tss_marketsizesummarybygroupuiocommodity();
        //        //_BL_tss_marketsizesummarybygroupuiocommodity.GenerateMarketSizeSummaryByGroupUIOCommodity(organizationService, context, listKeyAccount);
        //    }
        //    else
        //    {
        //        //throw new InvalidWorkflowException("KAUIO or KA Group UIO Commodity not exist.");
        //    }

        //    return _result;
        //}







        //public void GenerateMasterMS(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context, EntityCollection listKeyAccount)
        //{
        //    #region Get Key Account
        //    if (listKeyAccount == null)
        //    {
        //        FilterExpression fStatus = new FilterExpression(LogicalOperator.Or);
        //        fStatus.AddCondition("tss_status", ConditionOperator.Equal, STATUS_OPEN);
        //        fStatus.AddCondition("tss_status", ConditionOperator.Equal, STATUS_ERROR);

        //        FilterExpression fKA = new FilterExpression(LogicalOperator.And);
        //        fKA.AddFilter(fStatus);
        //        fKA.AddCondition("tss_pss", ConditionOperator.Equal, context.UserId);
        //        fKA.AddCondition("tss_customer", ConditionOperator.NotNull);
        //        fKA.AddCondition("tss_activeenddate", ConditionOperator.GreaterEqual, DateTime.Today);
        //        fKA.AddCondition("tss_activestartdate", ConditionOperator.LessEqual, DateTime.Today);
        //        fKA.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true); //2018.10.12

        //        QueryExpression qKA = new QueryExpression(_DL_tss_keyaccount.EntityName);
        //        qKA.Criteria.AddFilter(fKA);
        //        qKA.ColumnSet = new ColumnSet(true);
        //        //EntityCollection listKeyAccount = _DL_tss_mastermarketsize.Select(organizationService, qKA);
        //        listKeyAccount = _DL_tss_mastermarketsize.Select(organizationService, qKA);
        //    }
        //    #endregion

        //    const int maxItemsPerBatch = 1;
        //    const int maxBatches = 1;

        //    Stopwatch sw = new Stopwatch();
        //    sw.Start();

        //    List<System.Threading.Tasks.Task> tasks = new List<System.Threading.Tasks.Task>();

        //    for (int j = 0; j < maxBatches; j++)
        //    {
        //        var t1 = System.Threading.Tasks.Task.Factory.StartNew(() =>
        //        {
        //            tracingService.Trace("Exception: HELLO !");

        //            //XrmServiceContext
        //            using (CrmOrganizationServiceContext asd = new CrmOrganizationServiceContext(new OrganizationService(new CrmConnection(new ConnectionStringSettings("TRSCRM", "Url = http://10.0.10.43/TraktorNusantara; Domain = traknus; Username = admin.crm; Password = p@55w0rdTNc12m18"))
        //            {
        //                Timeout = TimeSpan.FromMinutes(5),
        //                ServiceConfigurationInstanceMode = ServiceConfigurationInstanceMode.PerRequest
        //            })))
        //            {
        //                tracingService.Trace("Exception: MASUK !");

        //                ExecuteMultipleRequest request = new ExecuteMultipleRequest
        //                {
        //                    Settings = new ExecuteMultipleSettings
        //                    {
        //                        ContinueOnError = false,
        //                        ReturnResponses = false
        //                    },
        //                    Requests = new OrganizationRequestCollection()
        //                };

        //                for (int i = 0; i < maxItemsPerBatch; i++)
        //                {
        //                    if (listKeyAccount.Entities.Count > 0)
        //                    {
        //                        #region Generate MS KAUIO
        //                        #region Get KA UIO
        //                        //Get KA UIO
        //                        object[] kaIds = listKeyAccount.Entities.Select(x => (object)x.Id).ToArray();
        //                        FilterExpression fKAUIO = new FilterExpression(LogicalOperator.And);
        //                        fKAUIO.AddCondition("tss_keyaccountid", ConditionOperator.In, kaIds);
        //                        fKAUIO.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true);
        //                        fKAUIO.AddCondition("tss_serialnumber", ConditionOperator.NotNull);

        //                        QueryExpression qKAUIO = new QueryExpression(_DL_tss_kauio.EntityName);
        //                        qKAUIO.ColumnSet = new ColumnSet(true);
        //                        qKAUIO.Criteria.AddFilter(fKAUIO);
        //                        EntityCollection listKAUIO = _DL_tss_kauio.Select(organizationService, qKAUIO);
        //                        #endregion

        //                        #region Generatex
        //                        //Check Population
        //                        if (listKAUIO.Entities.Count > 0)
        //                        {
        //                            object[] serialNums = listKAUIO.Entities.Select(x => (object)x.GetAttributeValue<EntityReference>("tss_serialnumber").Id).ToArray();

        //                            FilterExpression fPop = new FilterExpression(LogicalOperator.And);
        //                            fPop.AddCondition("new_populationid", ConditionOperator.In, serialNums);
        //                            fPop.AddCondition("trs_productmaster", ConditionOperator.NotNull);

        //                            QueryExpression qPop = new QueryExpression(_DL_population.EntityName);
        //                            qPop.ColumnSet = new ColumnSet(true);
        //                            qPop.Criteria.AddFilter(fPop);
        //                            EntityCollection listPop = _DL_population.Select(organizationService, qPop);

        //                            if (listPop.Entities.Count > 0)
        //                            {
        //                                List<Guid> msIds = new List<Guid>();

        //                                foreach (var pop in listPop.Entities)
        //                                {
        //                                    Guid _masterMarketSize = new Guid();
        //                                    Entity currentUIO = listKAUIO.Entities.Where(x => x.GetAttributeValue<EntityReference>("tss_serialnumber").Id == pop.GetAttributeValue<Guid>("new_populationid")).FirstOrDefault();
        //                                    Entity currentKA = listKeyAccount.Entities.Where(x => x.Id == currentUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id).FirstOrDefault();

        //                                    bool Method1 = false;
        //                                    bool Method2 = false;
        //                                    bool Method3 = false;
        //                                    bool Method4 = false;
        //                                    bool Method5 = false;

        //                                    //CHECK METHOD 1
        //                                    if (pop.Attributes.Contains("tss_mscurrenthourmeter")
        //                                        && pop.Attributes.Contains("tss_mslasthourmeter")
        //                                        && pop.Attributes.Contains("tss_mscurrenthourmeterdate")
        //                                        && pop.Attributes.Contains("tss_mslasthourmeterdate"))
        //                                    {
        //                                        Method1 = true;
        //                                        Console.Write("Method 1 : " + Method1);
        //                                    }

        //                                    //CHECK METHOD 2
        //                                    if (pop.Attributes.Contains("tss_mscurrenthourmeter")
        //                                        && pop.Attributes.Contains("tss_mscurrenthourmeterdate")
        //                                        && (pop.Attributes.Contains("new_deliverydate") || pop.Attributes.Contains("trs_warrantyenddate")))
        //                                    {
        //                                        Method2 = true;
        //                                        Console.Write("Method 2 : " + Method2);
        //                                    }

        //                                    //CHECK METHOD 3
        //                                    if (pop.Attributes.Contains("tss_estworkinghour"))
        //                                    {
        //                                        Method3 = true;
        //                                        Console.Write("Method 3 : " + Method3);
        //                                    }

        //                                    //CHECK METHOD 4
        //                                    if (pop.Attributes.Contains("new_deliverydate")
        //                                        && currentKA.Attributes.Contains("tss_msperiodstart")
        //                                        && currentKA.Attributes.Contains("tss_msperiodend"))
        //                                    {
        //                                        Method4 = true;
        //                                        Console.Write("Method 4 : " + Method4);
        //                                    }

        //                                    //CHECK METHOD 5
        //                                    if ((pop.Attributes.Contains("new_deliverydate") || pop.Attributes.Contains("trs_warrantyenddate"))
        //                                        && currentKA.Attributes.Contains("tss_msperiodstart")
        //                                        && currentKA.Attributes.Contains("tss_msperiodend"))
        //                                    {
        //                                        Method5 = true;
        //                                        Console.Write("Method 5 : " + Method5);
        //                                    }

        //                                    if (Method1 || Method2 || Method3 || Method4 || Method5)
        //                                    {
        //                                        //Generate MS UIO/Non UIO
        //                                        decimal currentHM;
        //                                        decimal lastHM;
        //                                        DateTime currentHMDate;
        //                                        DateTime lastHMDate;
        //                                        DateTime deliveryDate;
        //                                        DateTime warrantyEndDate = DateTime.MinValue;
        //                                        DateTime calculateMarketSizeDate = DateTime.Now.ToLocalTime().Date;

        //                                        QueryExpression qPro = new QueryExpression(_DL_product.EntityName);
        //                                        qPro.Criteria.AddCondition("productid", ConditionOperator.Equal, pop.GetAttributeValue<EntityReference>("trs_productmaster").Id);
        //                                        qPro.ColumnSet.AddColumn("tss_usetyre");
        //                                        bool useTyre = _DL_product.Select(organizationService, qPro).Entities.FirstOrDefault().GetAttributeValue<bool>("tss_usetyre");

        //                                        QueryExpression qSetup = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
        //                                        qSetup.ColumnSet = new ColumnSet(true);
        //                                        EntityCollection setups = _DL_tss_sparepartsetup.Select(organizationService, qSetup);

        //                                        if (setups.Entities.Count > 0)
        //                                        {
        //                                            Entity setup = setups.Entities[0];
        //                                            _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        //                                            _DL_tss_mastermarketsize.tss_pss = currentUIO.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                                            _DL_tss_mastermarketsize.tss_customer = currentUIO.GetAttributeValue<EntityReference>("tss_customer").Id; // pop.GetAttributeValue<EntityReference>("new_customercode").Id;
        //                                            _DL_tss_mastermarketsize.tss_serialnumber = pop.GetAttributeValue<Guid>("new_populationid");

        //                                            //METHOD 1
        //                                            if (pop.Contains("tss_mscurrenthourmeter")
        //                                               && pop.Contains("tss_mslasthourmeter")
        //                                               && pop.Contains("tss_mscurrenthourmeterdate")
        //                                               && pop.Contains("tss_mslasthourmeterdate"))
        //                                            {
        //                                                currentHM = pop.GetAttributeValue<decimal>("tss_mscurrenthourmeter");
        //                                                lastHM = pop.GetAttributeValue<decimal>("tss_mslasthourmeter");
        //                                                currentHMDate = pop.GetAttributeValue<DateTime>("tss_mscurrenthourmeterdate").ToLocalTime().Date;
        //                                                lastHMDate = pop.GetAttributeValue<DateTime>("tss_mslasthourmeterdate").ToLocalTime().Date;

        //                                                //if ((decimal)(currentHMDate - lastHMDate).TotalDays == 0)
        //                                                //    _DL_tss_mastermarketsize.tss_avghmmethod1 = 0;
        //                                                //else
        //                                                //    _DL_tss_mastermarketsize.tss_avghmmethod1 = avghmmethod1 > (decimal)24 ? (decimal)24 : 0;

        //                                                if ((decimal)(currentHMDate - lastHMDate).TotalDays > 0)
        //                                                {
        //                                                    decimal avghmmethod1 = (currentHM - lastHM) / ((decimal)(currentHMDate - lastHMDate).TotalDays);

        //                                                    if (avghmmethod1 > 0)
        //                                                    {
        //                                                        _DL_tss_mastermarketsize.tss_avghmmethod1 = avghmmethod1 > (decimal)24 ? (decimal)24 : avghmmethod1;
        //                                                    }
        //                                                }
        //                                            }

        //                                            //METHOD 2
        //                                            if (pop.Contains("tss_mscurrenthourmeter")
        //                                               && pop.Contains("tss_mscurrenthourmeterdate")
        //                                               && (pop.Contains("new_deliverydate") || pop.Contains("trs_warrantyenddate")))
        //                                            {
        //                                                currentHM = pop.GetAttributeValue<decimal>("tss_mscurrenthourmeter");
        //                                                currentHMDate = pop.GetAttributeValue<DateTime>("tss_mscurrenthourmeterdate").ToLocalTime().Date;
        //                                                deliveryDate = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().Date;

        //                                                if (!pop.Attributes.Contains("trs_warrantyenddate"))
        //                                                {
        //                                                    warrantyEndDate = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().AddYears(1).Date;
        //                                                }
        //                                                else
        //                                                {
        //                                                    warrantyEndDate = pop.GetAttributeValue<DateTime>("trs_warrantyenddate").ToLocalTime().Date;
        //                                                }

        //                                                //if ((decimal)(currentHMDate - deliveryDate).TotalDays == 0)
        //                                                //    _DL_tss_mastermarketsize.tss_avghmmethod2 = 0;
        //                                                //else
        //                                                //    _DL_tss_mastermarketsize.tss_avghmmethod2 = avghmmethod2 > (decimal)24 ? (decimal)24 : 0;

        //                                                if ((decimal)(currentHMDate - warrantyEndDate).TotalDays > 0)
        //                                                {
        //                                                    decimal avghmmethod2 = currentHM / ((decimal)(currentHMDate - warrantyEndDate).TotalDays);

        //                                                    if (avghmmethod2 > 0)
        //                                                    {
        //                                                        _DL_tss_mastermarketsize.tss_avghmmethod2 = avghmmethod2 > (decimal)24 ? (decimal)24 : avghmmethod2;
        //                                                    }
        //                                                }
        //                                            }

        //                                            //METHOD 3
        //                                            if (pop.Contains("tss_estworkinghour"))
        //                                            {
        //                                                decimal estimateWH = pop.GetAttributeValue<int>("tss_estworkinghour");

        //                                                if (estimateWH > 0)
        //                                                {
        //                                                    _DL_tss_mastermarketsize.tss_avghmmethod3 = estimateWH > (decimal)24 ? (decimal)24 : estimateWH;
        //                                                }
        //                                            }

        //                                            //METHOD 4
        //                                            if (pop.Contains("new_deliverydate")
        //                                                && currentKA.Contains("tss_msperiodstart")
        //                                                && currentKA.Contains("tss_msperiodend"))
        //                                            {
        //                                                CalculateDate oCalculate = new CalculateDate();

        //                                                deliveryDate = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().Date;
        //                                                DateTime msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart").ToLocalTime().Date;
        //                                                DateTime msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend").ToLocalTime().Date;

        //                                                decimal oDeliveryDate = oCalculate.DiffYear(deliveryDate, calculateMarketSizeDate);
        //                                                decimal oPeriodDate = oCalculate.DiffYear(msperiodstart, msperiodend);

        //                                                decimal periodpmmethod4 = (decimal)(oDeliveryDate + oPeriodDate);

        //                                                if (periodpmmethod4 > 0)
        //                                                {
        //                                                    _DL_tss_mastermarketsize.tss_periodpmmethod4 = periodpmmethod4;
        //                                                }
        //                                            }

        //                                            //METHOD 5
        //                                            if (pop.Contains("new_deliverydate")
        //                                                && currentKA.Contains("tss_msperiodstart")
        //                                                && currentKA.Contains("tss_msperiodend"))
        //                                            {
        //                                                if (!pop.Attributes.Contains("trs_warrantyenddate"))
        //                                                {
        //                                                    warrantyEndDate = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().AddYears(1).Date;
        //                                                }
        //                                                else
        //                                                {
        //                                                    warrantyEndDate = pop.GetAttributeValue<DateTime>("trs_warrantyenddate").ToLocalTime().Date;
        //                                                }

        //                                                CalculateDate oCalculate = new CalculateDate();

        //                                                deliveryDate = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().Date;
        //                                                DateTime msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart").ToLocalTime().Date;
        //                                                DateTime msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend").ToLocalTime().Date;

        //                                                decimal oWarrantyDate = oCalculate.DiffYear(deliveryDate, warrantyEndDate);
        //                                                decimal oPeriodDate = oCalculate.DiffYear(msperiodstart, msperiodend);

        //                                                decimal periodpmmethod5 = (decimal)(oWarrantyDate + oPeriodDate);

        //                                                if (periodpmmethod5 > 0)
        //                                                {
        //                                                    _DL_tss_mastermarketsize.tss_periodpmmethod5 = periodpmmethod5;
        //                                                }
        //                                            }

        //                                            if (pop.Contains("tss_populationstatus"))
        //                                            {
        //                                                _DL_tss_mastermarketsize.tss_unittype = pop.GetAttributeValue<bool>("tss_populationstatus") ? UNITTYPE_UIO : UNITTYPE_NONUIO;
        //                                            }
        //                                            _DL_tss_mastermarketsize.tss_status = STATUS_COMPLETED_MS;
        //                                            _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_COMPLETED_MS;
        //                                            _DL_tss_mastermarketsize.tss_usetyre = useTyre;
        //                                            _DL_tss_mastermarketsize.tss_msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                                            _DL_tss_mastermarketsize.tss_msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend");
        //                                            _DL_tss_mastermarketsize.tss_activeperiodstart = currentKA.GetAttributeValue<DateTime>("tss_activestartdate");
        //                                            _DL_tss_mastermarketsize.tss_activeperiodsend = currentKA.GetAttributeValue<DateTime>("tss_activeenddate");
        //                                            _DL_tss_mastermarketsize.tss_keyaccountid = currentKA.Id;
        //                                            _DL_tss_mastermarketsize.tss_msperiod = currentKA.GetAttributeValue<int>("tss_revision");
        //                                            _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;

        //                                            Entity _mastermarketsize = _DL_tss_mastermarketsize.GetEntity(organizationService);

        //                                            tracingService.Trace("Exception: {0} !", _mastermarketsize.Id.ToString());

        //                                            request.Requests.Add(new CreateRequest { Target = _mastermarketsize });

        //                                            //_masterMarketSize = _DL_tss_mastermarketsize.Insert(organizationService);

        //                                            //#region Update Status
        //                                            //_DL_tss_keyaccount = new DL_tss_keyaccount();
        //                                            //_DL_tss_keyaccount.tss_status = STATUS_CALCULATE;
        //                                            //_DL_tss_keyaccount.tss_reason = STATUS_REASON_CALCULATE;
        //                                            //_DL_tss_keyaccount.UpdateStatus(organizationService, currentUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

        //                                            //_DL_tss_kauio = new DL_tss_kauio();
        //                                            //_DL_tss_kauio.tss_calculatestatus = true;
        //                                            //_DL_tss_kauio.tss_errordescription = "";
        //                                            //_DL_tss_kauio.UpdateStatus(organizationService, currentUIO.GetAttributeValue<Guid>("tss_kauioid"));
        //                                            //#endregion

        //                                            //#region Generate Lines
        //                                            //GenerateMasterMSLines(organizationService, _DL_tss_mastermarketsize, pop, setup);
        //                                            //#endregion
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        List<string> _errorcollection = new List<string>();
        //                                        string _errormessage = "";

        //                                        //Generate MS UIO/Non UIO
        //                                        QueryExpression qPro = new QueryExpression(_DL_product.EntityName);
        //                                        qPro.Criteria.AddCondition("productid", ConditionOperator.Equal, pop.GetAttributeValue<EntityReference>("trs_productmaster").Id);
        //                                        qPro.ColumnSet.AddColumn("tss_usetyre");
        //                                        bool useTyre = _DL_product.Select(organizationService, qPro).Entities.FirstOrDefault().GetAttributeValue<bool>("tss_usetyre");

        //                                        if (!pop.Contains("tss_mscurrenthourmeter"))
        //                                            _errorcollection.Add("Current Hour Meter");

        //                                        if (!pop.Contains("tss_mscurrenthourmeterdate"))
        //                                            _errorcollection.Add("Current Hour Meter (Date)");

        //                                        if (!pop.Contains("tss_mslasthourmeter"))
        //                                            _errorcollection.Add("Last Hour Meter");

        //                                        if (!pop.Contains("tss_mslasthourmeterdate"))
        //                                            _errorcollection.Add("Last Hour Meter (Date)");

        //                                        if (!pop.Contains("new_deliverydate"))
        //                                            _errorcollection.Add("Delivery Date");

        //                                        if (_errorcollection.Count() > 0)
        //                                        {
        //                                            foreach (var item in _errorcollection)
        //                                            {
        //                                                _errormessage += item + ", ";
        //                                            }

        //                                            _errormessage = _errormessage.Substring(0, _errormessage.Length - 1) + " still not define !";
        //                                        }
        //                                        else
        //                                            _errormessage = "Current Hour Meter, Current Hour Meter (Date), Last Hour Meter , Last Hour Meter (Date), Delivery Date or Overhaul Date still not define";

        //                                        _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();

        //                                        _DL_tss_mastermarketsize.tss_pss = currentUIO.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                                        _DL_tss_mastermarketsize.tss_customer = currentUIO.GetAttributeValue<EntityReference>("tss_customer").Id; // pop.GetAttributeValue<EntityReference>("new_customercode").Id;
        //                                        _DL_tss_mastermarketsize.tss_serialnumber = pop.GetAttributeValue<Guid>("new_populationid");
        //                                        if (pop.Contains("tss_populationstatus"))
        //                                        {
        //                                            _DL_tss_mastermarketsize.tss_unittype = pop.GetAttributeValue<bool>("tss_populationstatus") ? UNITTYPE_UIO : UNITTYPE_NONUIO;
        //                                        }
        //                                        _DL_tss_mastermarketsize.tss_status = STATUS_ERROR_MS;
        //                                        _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_ERROR_MS;
        //                                        _DL_tss_mastermarketsize.tss_errormessage = _errormessage;
        //                                        _DL_tss_mastermarketsize.tss_usetyre = useTyre;
        //                                        _DL_tss_mastermarketsize.tss_msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                                        _DL_tss_mastermarketsize.tss_msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend");
        //                                        _DL_tss_mastermarketsize.tss_activeperiodstart = currentKA.GetAttributeValue<DateTime>("tss_activestartdate");
        //                                        _DL_tss_mastermarketsize.tss_activeperiodsend = currentKA.GetAttributeValue<DateTime>("tss_activeenddate");
        //                                        _DL_tss_mastermarketsize.tss_keyaccountid = currentKA.Id;
        //                                        _DL_tss_mastermarketsize.tss_msperiod = currentKA.GetAttributeValue<int>("tss_revision");
        //                                        _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;

        //                                        Entity _mastermarketsize = _DL_tss_mastermarketsize.GetEntity(organizationService);

        //                                        tracingService.Trace("Exception: {0} !", _mastermarketsize.Id.ToString());

        //                                        request.Requests.Add(new CreateRequest { Target = _mastermarketsize });

        //                                        //_DL_tss_mastermarketsize.Insert(organizationService);

        //                                        //#region Update Status
        //                                        //_DL_tss_keyaccount = new DL_tss_keyaccount();
        //                                        //_DL_tss_keyaccount.tss_status = STATUS_ERROR;
        //                                        //_DL_tss_keyaccount.UpdateStatus(organizationService, currentUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

        //                                        //_DL_tss_kauio = new DL_tss_kauio();
        //                                        //_DL_tss_kauio.tss_calculatestatus = false;
        //                                        //_DL_tss_kauio.tss_errordescription = _errormessage;
        //                                        //_DL_tss_kauio.UpdateStatus(organizationService, currentUIO.GetAttributeValue<Guid>("tss_kauioid"));
        //                                        //#endregion
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        #endregion

        //                        #endregion

        //                        //#region Generate KA Group UIO Commodity
        //                        //#region Get KA Group UIO Commodity
        //                        ////Get KA UIO
        //                        //FilterExpression fkaUIOCommodity = new FilterExpression(LogicalOperator.And);
        //                        //fkaUIOCommodity.AddCondition("tss_keyaccountid", ConditionOperator.In, kaIds);
        //                        //fkaUIOCommodity.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true);
        //                        //fkaUIOCommodity.AddCondition("tss_groupuiocommodity", ConditionOperator.NotNull);

        //                        //QueryExpression qkaUIOCommodity = new QueryExpression(_DL_tss_kagroupuiocommodity.EntityName);
        //                        //qkaUIOCommodity.ColumnSet = new ColumnSet(true);
        //                        //qkaUIOCommodity.Criteria.AddFilter(fkaUIOCommodity);
        //                        //EntityCollection listKAGroupUIOCommodity = _DL_tss_kagroupuiocommodity.Select(organizationService, qkaUIOCommodity);
        //                        //#endregion

        //                        //#region Generate
        //                        //if (listKAGroupUIOCommodity.Entities.Count > 0)
        //                        //{
        //                        //    #region Get Group Commodity Account
        //                        //    object[] kaGroupUIOCommIds = listKAGroupUIOCommodity.Entities.Select(x => (object)x.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id).ToArray();

        //                        //    FilterExpression fGroupAccount = new FilterExpression(LogicalOperator.And);
        //                        //    fGroupAccount.AddCondition("tss_groupuiocommodityaccountid", ConditionOperator.In, kaGroupUIOCommIds);
        //                        //    fGroupAccount.AddCondition("tss_groupuiocommodityheader", ConditionOperator.NotNull);

        //                        //    QueryExpression qGroupAccount = new QueryExpression(_DL_tss_groupuiocommodityaccount.EntityName);
        //                        //    qGroupAccount.ColumnSet = new ColumnSet(true);
        //                        //    qGroupAccount.Criteria.AddFilter(fGroupAccount);
        //                        //    EntityCollection listGroupAccount = _DL_tss_groupuiocommodityaccount.Select(organizationService, qGroupAccount);
        //                        //    #endregion

        //                        //    if (listGroupAccount.Entities.Count > 0)
        //                        //    {
        //                        //        foreach (var groupAccount in listGroupAccount.Entities)
        //                        //        {
        //                        //            Guid _masterMarketSize = new Guid();
        //                        //            Entity currentKAGroupUIO = listKAGroupUIOCommodity.Entities.Where(x => x.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id == groupAccount.GetAttributeValue<Guid>("tss_groupuiocommodityaccountid")).FirstOrDefault();
        //                        //            Entity currentKA = listKeyAccount.Entities.Where(x => x.Id == currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id).FirstOrDefault();

        //                        //            if (groupAccount.Contains("tss_qty") && groupAccount.GetAttributeValue<int>("tss_qty") > 0)
        //                        //            {
        //                        //                _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        //                        //                _DL_tss_mastermarketsize.tss_pss = currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                        //                _DL_tss_mastermarketsize.tss_customer = currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_customer").Id;
        //                        //                //_DL_tss_mastermarketsize.tss_groupuiocommodity = groupAccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityname").Id;
        //                        //                _DL_tss_mastermarketsize.tss_groupuiocommodityheader = groupAccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id;
        //                        //                _DL_tss_mastermarketsize.tss_qty = groupAccount.GetAttributeValue<int>("tss_qty");
        //                        //                _DL_tss_mastermarketsize.tss_unittype = UNITTYPE_COMMODITY;
        //                        //                _DL_tss_mastermarketsize.tss_status = STATUS_COMPLETED_MS;
        //                        //                _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_COMPLETED_MS;
        //                        //                _DL_tss_mastermarketsize.tss_msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                        //                _DL_tss_mastermarketsize.tss_msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend");
        //                        //                _DL_tss_mastermarketsize.tss_activeperiodstart = currentKA.GetAttributeValue<DateTime>("tss_activestartdate");
        //                        //                _DL_tss_mastermarketsize.tss_activeperiodsend = currentKA.GetAttributeValue<DateTime>("tss_activeenddate");
        //                        //                _DL_tss_mastermarketsize.tss_keyaccountid = currentKA.Id;
        //                        //                _DL_tss_mastermarketsize.tss_msperiod = currentKA.GetAttributeValue<int>("tss_revision");
        //                        //                _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;

        //                        //                _masterMarketSize = _DL_tss_mastermarketsize.Insert(organizationService);

        //                        //                #region Update Status
        //                        //                _DL_tss_keyaccount = new DL_tss_keyaccount();
        //                        //                _DL_tss_keyaccount.tss_status = STATUS_CALCULATE;
        //                        //                _DL_tss_keyaccount.UpdateStatus(organizationService, currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

        //                        //                _DL_tss_kagroupuiocommodity = new DL_tss_kagroupuiocommodity();
        //                        //                _DL_tss_kagroupuiocommodity.tss_calculatestatus = true;
        //                        //                _DL_tss_kagroupuiocommodity.UpdateStatus(organizationService, currentKAGroupUIO.GetAttributeValue<Guid>("tss_kagroupuiocommodityid"));
        //                        //                #endregion

        //                        //                #region Generate Sub Lines
        //                        //                Entity _tss_groupuiocommodityaccount = organizationService.Retrieve("tss_groupuiocommodityaccount", currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id, new ColumnSet(true));

        //                        //                if (_tss_groupuiocommodityaccount != null)
        //                        //                {
        //                        //                    Entity _tss_groupuiocommodityheader = organizationService.Retrieve("tss_groupuiocommodityheader", _tss_groupuiocommodityaccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id, new ColumnSet(true));

        //                        //                    FilterExpression fGroupUIOCommodity = new FilterExpression(LogicalOperator.And);
        //                        //                    fGroupUIOCommodity.AddCondition("tss_groupuiocommodityheaderid", ConditionOperator.In, _tss_groupuiocommodityheader.Id);

        //                        //                    QueryExpression qGroupUIOCommodity = new QueryExpression(_DL_tss_groupuiocommodity.EntityName);
        //                        //                    qGroupUIOCommodity.ColumnSet = new ColumnSet(true);
        //                        //                    qGroupUIOCommodity.Criteria.AddFilter(fGroupUIOCommodity);
        //                        //                    EntityCollection listGroupUIOCommodity = _DL_tss_groupuiocommodity.Select(organizationService, qGroupUIOCommodity);

        //                        //                    if (listGroupUIOCommodity.Entities.Count > 0)
        //                        //                    {
        //                        //                        foreach (var group in listGroupUIOCommodity.Entities)
        //                        //                        {
        //                        //                            Entity _tss_groupuiocommodity = organizationService.Retrieve("tss_groupuiocommodity", group.Id, new ColumnSet(true));

        //                        //                            if (_tss_groupuiocommodity != null)
        //                        //                            {
        //                        //                                GenerateMasterMSSubLinesCommodity(organizationService, _DL_tss_mastermarketsize, _tss_groupuiocommodity, groupAccount, currentKAGroupUIO);
        //                        //                            }
        //                        //                        }
        //                        //                    }
        //                        //                }
        //                        //                #endregion
        //                        //            }
        //                        //            else
        //                        //            {
        //                        //                _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        //                        //                _DL_tss_mastermarketsize.tss_pss = currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                        //                _DL_tss_mastermarketsize.tss_customer = currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_customer").Id;
        //                        //                //_DL_tss_mastermarketsize.tss_groupuiocommodity = groupAccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityname").Id;
        //                        //                _DL_tss_mastermarketsize.tss_groupuiocommodityheader = groupAccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id;
        //                        //                _DL_tss_mastermarketsize.tss_unittype = UNITTYPE_COMMODITY;
        //                        //                _DL_tss_mastermarketsize.tss_status = STATUS_ERROR_MS;
        //                        //                _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_ERROR_MS;
        //                        //                _DL_tss_mastermarketsize.tss_errormessage = "Qty still not define";
        //                        //                _DL_tss_mastermarketsize.tss_msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                        //                _DL_tss_mastermarketsize.tss_msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend");
        //                        //                _DL_tss_mastermarketsize.tss_activeperiodstart = currentKA.GetAttributeValue<DateTime>("tss_activestartdate");
        //                        //                _DL_tss_mastermarketsize.tss_activeperiodsend = currentKA.GetAttributeValue<DateTime>("tss_activeenddate");
        //                        //                _DL_tss_mastermarketsize.tss_keyaccountid = currentKA.Id;
        //                        //                _DL_tss_mastermarketsize.tss_msperiod = currentKA.GetAttributeValue<int>("tss_revision");
        //                        //                _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;

        //                        //                _DL_tss_mastermarketsize.Insert(organizationService);

        //                        //                #region Update Status
        //                        //                _DL_tss_keyaccount = new DL_tss_keyaccount();
        //                        //                _DL_tss_keyaccount.tss_status = STATUS_ERROR;
        //                        //                _DL_tss_keyaccount.UpdateStatus(organizationService, currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

        //                        //                _DL_tss_kagroupuiocommodity = new DL_tss_kagroupuiocommodity();
        //                        //                _DL_tss_kagroupuiocommodity.tss_calculatestatus = false;
        //                        //                _DL_tss_kagroupuiocommodity.UpdateStatus(organizationService, currentKAGroupUIO.GetAttributeValue<Guid>("tss_kagroupuiocommodityid"));
        //                        //                #endregion
        //                        //            }
        //                        //        }
        //                        //    }
        //                        //}
        //                        //#endregion
        //                        //#endregion

        //                        //Guid[] customerID = listKeyAccount.Entities.GroupBy(x => x.GetAttributeValue<EntityReference>("tss_customer").Id).Select(x => x.First().GetAttributeValue<EntityReference>("tss_customer").Id).ToArray();

        //                        //BL_tss_marketsizeresultpss _BL_tss_marketsizeresultpss = new BL_tss_marketsizeresultpss();
        //                        //_BL_tss_marketsizeresultpss.GenerateMarketSizeResultPSS_New(organizationService, context, customerID);

        //                        //BL_tss_marketsizesummarybypartnumber _BL_tss_marketsizesummarybypartnumber = new BL_tss_marketsizesummarybypartnumber();
        //                        //_BL_tss_marketsizesummarybypartnumber.GenerateMarketSizeSummaryByPartNumber(organizationService, context);

        //                        //BL_tss_marketsizesummarybygroupuiocommodity _BL_tss_marketsizesummarybygroupuiocommodity = new BL_tss_marketsizesummarybygroupuiocommodity();
        //                        //_BL_tss_marketsizesummarybygroupuiocommodity.GenerateMarketSizeSummaryByGroupUIOCommodity(organizationService, context, listKeyAccount);
        //                    }
        //                    else
        //                    {
        //                        throw new InvalidWorkflowException("KAUIO or KA Group UIO Commodity not exist.");
        //                    }
        //                }

        //                //OrganizationResponse response = asd.Execute(request);

        //                //throw new InvalidPluginExecutionException(response.Results.Keys.ToString()) ;
        //                //if (response.IsFaulted)
        //                //{

        //                //};
        //            }
        //        });

        //        tasks.Add(t1);
        //    }

        //    System.Threading.Tasks.Task.WhenAll(tasks).Wait();
        //}












        //public void GenerateMasterMS(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context, EntityCollection listKeyAccount)
        //{
        //    #region Get Key Account
        //    if (listKeyAccount == null)
        //    {
        //        FilterExpression fStatus = new FilterExpression(LogicalOperator.Or);
        //        fStatus.AddCondition("tss_status", ConditionOperator.Equal, STATUS_OPEN);
        //        fStatus.AddCondition("tss_status", ConditionOperator.Equal, STATUS_ERROR);

        //        FilterExpression fKA = new FilterExpression(LogicalOperator.And);
        //        fKA.AddFilter(fStatus);
        //        fKA.AddCondition("tss_pss", ConditionOperator.Equal, context.UserId);
        //        fKA.AddCondition("tss_customer", ConditionOperator.NotNull);
        //        fKA.AddCondition("tss_activeenddate", ConditionOperator.GreaterEqual, DateTime.Today);
        //        fKA.AddCondition("tss_activestartdate", ConditionOperator.LessEqual, DateTime.Today);
        //        fKA.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true); //2018.10.12

        //        QueryExpression qKA = new QueryExpression(_DL_tss_keyaccount.EntityName);
        //        qKA.Criteria.AddFilter(fKA);
        //        qKA.ColumnSet = new ColumnSet(true);
        //        //EntityCollection listKeyAccount = _DL_tss_mastermarketsize.Select(organizationService, qKA);
        //        listKeyAccount = _DL_tss_mastermarketsize.Select(organizationService, qKA);
        //    }
        //    #endregion

        //    if (listKeyAccount.Entities.Count > 0)
        //    {
        //        #region Generate MS KAUIO
        //        #region Get KA UIO
        //        //Get KA UIO
        //        object[] kaIds = listKeyAccount.Entities.Select(x => (object)x.Id).ToArray();
        //        FilterExpression fKAUIO = new FilterExpression(LogicalOperator.And);
        //        fKAUIO.AddCondition("tss_keyaccountid", ConditionOperator.In, kaIds);
        //        fKAUIO.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true);
        //        fKAUIO.AddCondition("tss_serialnumber", ConditionOperator.NotNull);

        //        QueryExpression qKAUIO = new QueryExpression(_DL_tss_kauio.EntityName);
        //        qKAUIO.ColumnSet = new ColumnSet(true);
        //        qKAUIO.Criteria.AddFilter(fKAUIO);
        //        EntityCollection listKAUIO = _DL_tss_kauio.Select(organizationService, qKAUIO);
        //        #endregion

        //        #region Generatex
        //        //Check Population
        //        if (listKAUIO.Entities.Count > 0)
        //        {
        //            object[] serialNums = listKAUIO.Entities.Select(x => (object)x.GetAttributeValue<EntityReference>("tss_serialnumber").Id).ToArray();

        //            FilterExpression fPop = new FilterExpression(LogicalOperator.And);
        //            fPop.AddCondition("new_populationid", ConditionOperator.In, serialNums);
        //            fPop.AddCondition("trs_productmaster", ConditionOperator.NotNull);

        //            QueryExpression qPop = new QueryExpression(_DL_population.EntityName);
        //            qPop.ColumnSet = new ColumnSet(true);
        //            qPop.Criteria.AddFilter(fPop);
        //            EntityCollection listPop = _DL_population.Select(organizationService, qPop);

        //            if (listPop.Entities.Count > 0)
        //            {
        //                List<Guid> msIds = new List<Guid>();

        //                foreach (var pop in listPop.Entities)
        //                {
        //                    Guid _masterMarketSize = new Guid();
        //                    Entity currentUIO = listKAUIO.Entities.Where(x => x.GetAttributeValue<EntityReference>("tss_serialnumber").Id == pop.GetAttributeValue<Guid>("new_populationid")).FirstOrDefault();
        //                    Entity currentKA = listKeyAccount.Entities.Where(x => x.Id == currentUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id).FirstOrDefault();

        //                    bool Method1 = false;
        //                    bool Method2 = false;
        //                    bool Method3 = false;
        //                    bool Method4 = false;
        //                    bool Method5 = false;

        //                    //CHECK METHOD 1
        //                    if (pop.Attributes.Contains("tss_mscurrenthourmeter")
        //                        && pop.Attributes.Contains("tss_mslasthourmeter")
        //                        && pop.Attributes.Contains("tss_mscurrenthourmeterdate")
        //                        && pop.Attributes.Contains("tss_mslasthourmeterdate"))
        //                    {
        //                        Method1 = true;
        //                        Console.Write("Method 1 : " + Method1);
        //                    }

        //                    //CHECK METHOD 2
        //                    if (pop.Attributes.Contains("tss_mscurrenthourmeter")
        //                        && pop.Attributes.Contains("tss_mscurrenthourmeterdate")
        //                        && (pop.Attributes.Contains("new_deliverydate") || pop.Attributes.Contains("trs_warrantyenddate")))
        //                    {
        //                        Method2 = true;
        //                        Console.Write("Method 2 : " + Method2);
        //                    }

        //                    //CHECK METHOD 3
        //                    if (pop.Attributes.Contains("tss_estworkinghour"))
        //                    {
        //                        Method3 = true;
        //                        Console.Write("Method 3 : " + Method3);
        //                    }

        //                    //CHECK METHOD 4
        //                    if (pop.Attributes.Contains("new_deliverydate")
        //                        && currentKA.Attributes.Contains("tss_msperiodstart")
        //                        && currentKA.Attributes.Contains("tss_msperiodend"))
        //                    {
        //                        Method4 = true;
        //                        Console.Write("Method 4 : " + Method4);
        //                    }

        //                    //CHECK METHOD 5
        //                    if ((pop.Attributes.Contains("new_deliverydate") || pop.Attributes.Contains("trs_warrantyenddate"))
        //                        && currentKA.Attributes.Contains("tss_msperiodstart")
        //                        && currentKA.Attributes.Contains("tss_msperiodend"))
        //                    {
        //                        Method5 = true;
        //                        Console.Write("Method 5 : " + Method5);
        //                    }

        //                    if (Method1 || Method2 || Method3 || Method4 || Method5)
        //                    {
        //                        //Generate MS UIO/Non UIO
        //                        decimal currentHM;
        //                        decimal lastHM;
        //                        DateTime currentHMDate;
        //                        DateTime lastHMDate;
        //                        DateTime deliveryDate;
        //                        DateTime warrantyEndDate = DateTime.MinValue;
        //                        DateTime calculateMarketSizeDate = DateTime.Now.ToLocalTime().Date;

        //                        QueryExpression qPro = new QueryExpression(_DL_product.EntityName);
        //                        qPro.Criteria.AddCondition("productid", ConditionOperator.Equal, pop.GetAttributeValue<EntityReference>("trs_productmaster").Id);
        //                        qPro.ColumnSet.AddColumn("tss_usetyre");
        //                        bool useTyre = _DL_product.Select(organizationService, qPro).Entities.FirstOrDefault().GetAttributeValue<bool>("tss_usetyre");

        //                        QueryExpression qSetup = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
        //                        qSetup.ColumnSet = new ColumnSet(true);
        //                        EntityCollection setups = _DL_tss_sparepartsetup.Select(organizationService, qSetup);

        //                        if (setups.Entities.Count > 0)
        //                        {
        //                            Entity setup = setups.Entities[0];
        //                            _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        //                            _DL_tss_mastermarketsize.tss_pss = currentUIO.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                            _DL_tss_mastermarketsize.tss_customer = currentUIO.GetAttributeValue<EntityReference>("tss_customer").Id; // pop.GetAttributeValue<EntityReference>("new_customercode").Id;
        //                            _DL_tss_mastermarketsize.tss_serialnumber = pop.GetAttributeValue<Guid>("new_populationid");

        //                            //METHOD 1
        //                            if (pop.Contains("tss_mscurrenthourmeter")
        //                               && pop.Contains("tss_mslasthourmeter")
        //                               && pop.Contains("tss_mscurrenthourmeterdate")
        //                               && pop.Contains("tss_mslasthourmeterdate"))
        //                            {
        //                                currentHM = pop.GetAttributeValue<decimal>("tss_mscurrenthourmeter");
        //                                lastHM = pop.GetAttributeValue<decimal>("tss_mslasthourmeter");
        //                                currentHMDate = pop.GetAttributeValue<DateTime>("tss_mscurrenthourmeterdate").ToLocalTime().Date;
        //                                lastHMDate = pop.GetAttributeValue<DateTime>("tss_mslasthourmeterdate").ToLocalTime().Date;

        //                                //if ((decimal)(currentHMDate - lastHMDate).TotalDays == 0)
        //                                //    _DL_tss_mastermarketsize.tss_avghmmethod1 = 0;
        //                                //else
        //                                //    _DL_tss_mastermarketsize.tss_avghmmethod1 = avghmmethod1 > (decimal)24 ? (decimal)24 : 0;

        //                                if ((decimal)(currentHMDate - lastHMDate).TotalDays > 0)
        //                                {
        //                                    decimal avghmmethod1 = (currentHM - lastHM) / ((decimal)(currentHMDate - lastHMDate).TotalDays);

        //                                    if (avghmmethod1 > 0)
        //                                    {
        //                                        _DL_tss_mastermarketsize.tss_avghmmethod1 = avghmmethod1 > (decimal)24 ? (decimal)24 : avghmmethod1;
        //                                    }
        //                                }
        //                            }

        //                            //METHOD 2
        //                            if (pop.Contains("tss_mscurrenthourmeter")
        //                               && pop.Contains("tss_mscurrenthourmeterdate")
        //                               && (pop.Contains("new_deliverydate") || pop.Contains("trs_warrantyenddate")))
        //                            {
        //                                currentHM = pop.GetAttributeValue<decimal>("tss_mscurrenthourmeter");
        //                                currentHMDate = pop.GetAttributeValue<DateTime>("tss_mscurrenthourmeterdate").ToLocalTime().Date;
        //                                deliveryDate = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().Date;

        //                                if (!pop.Attributes.Contains("trs_warrantyenddate"))
        //                                {
        //                                    warrantyEndDate = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().AddYears(1).Date;
        //                                }
        //                                else
        //                                {
        //                                    warrantyEndDate = pop.GetAttributeValue<DateTime>("trs_warrantyenddate").ToLocalTime().Date;
        //                                }

        //                                //if ((decimal)(currentHMDate - deliveryDate).TotalDays == 0)
        //                                //    _DL_tss_mastermarketsize.tss_avghmmethod2 = 0;
        //                                //else
        //                                //    _DL_tss_mastermarketsize.tss_avghmmethod2 = avghmmethod2 > (decimal)24 ? (decimal)24 : 0;

        //                                if ((decimal)(currentHMDate - warrantyEndDate).TotalDays > 0)
        //                                {
        //                                    decimal avghmmethod2 = currentHM / ((decimal)(currentHMDate - warrantyEndDate).TotalDays);

        //                                    if (avghmmethod2 > 0)
        //                                    {
        //                                        _DL_tss_mastermarketsize.tss_avghmmethod2 = avghmmethod2 > (decimal)24 ? (decimal)24 : avghmmethod2;
        //                                    }
        //                                }
        //                            }

        //                            //METHOD 3
        //                            if (pop.Contains("tss_estworkinghour"))
        //                            {
        //                                decimal estimateWH = pop.GetAttributeValue<int>("tss_estworkinghour");

        //                                if (estimateWH > 0)
        //                                {
        //                                    _DL_tss_mastermarketsize.tss_avghmmethod3 = estimateWH > (decimal)24 ? (decimal)24 : estimateWH;
        //                                }
        //                            }

        //                            //METHOD 4
        //                            if (pop.Contains("new_deliverydate")
        //                                && currentKA.Contains("tss_msperiodstart")
        //                                && currentKA.Contains("tss_msperiodend"))
        //                            {
        //                                CalculateDate oCalculate = new CalculateDate();

        //                                deliveryDate = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().Date;
        //                                DateTime msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart").ToLocalTime().Date;
        //                                DateTime msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend").ToLocalTime().Date;

        //                                decimal oDeliveryDate = oCalculate.DiffYear(deliveryDate, calculateMarketSizeDate);
        //                                decimal oPeriodDate = oCalculate.DiffYear(msperiodstart, msperiodend);

        //                                decimal periodpmmethod4 = (decimal)(oDeliveryDate + oPeriodDate);

        //                                if (periodpmmethod4 > 0)
        //                                {
        //                                    _DL_tss_mastermarketsize.tss_periodpmmethod4 = periodpmmethod4;
        //                                }
        //                            }

        //                            //METHOD 5
        //                            if (pop.Contains("new_deliverydate")
        //                                && currentKA.Contains("tss_msperiodstart")
        //                                && currentKA.Contains("tss_msperiodend"))
        //                            {
        //                                if (!pop.Attributes.Contains("trs_warrantyenddate"))
        //                                {
        //                                    warrantyEndDate = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().AddYears(1).Date;
        //                                }
        //                                else
        //                                {
        //                                    warrantyEndDate = pop.GetAttributeValue<DateTime>("trs_warrantyenddate").ToLocalTime().Date;
        //                                }

        //                                CalculateDate oCalculate = new CalculateDate();

        //                                deliveryDate = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().Date;
        //                                DateTime msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart").ToLocalTime().Date;
        //                                DateTime msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend").ToLocalTime().Date;

        //                                decimal oWarrantyDate = oCalculate.DiffYear(deliveryDate, warrantyEndDate);
        //                                decimal oPeriodDate = oCalculate.DiffYear(msperiodstart, msperiodend);

        //                                decimal periodpmmethod5 = (decimal)(oWarrantyDate + oPeriodDate);

        //                                if (periodpmmethod5 > 0)
        //                                {
        //                                    _DL_tss_mastermarketsize.tss_periodpmmethod5 = periodpmmethod5;
        //                                }
        //                            }

        //                            if (pop.Contains("tss_populationstatus"))
        //                            {
        //                                _DL_tss_mastermarketsize.tss_unittype = pop.GetAttributeValue<bool>("tss_populationstatus") ? UNITTYPE_UIO : UNITTYPE_NONUIO;
        //                            }
        //                            _DL_tss_mastermarketsize.tss_status = STATUS_COMPLETED_MS;
        //                            _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_COMPLETED_MS;
        //                            _DL_tss_mastermarketsize.tss_usetyre = useTyre;
        //                            _DL_tss_mastermarketsize.tss_msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                            _DL_tss_mastermarketsize.tss_msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend");
        //                            _DL_tss_mastermarketsize.tss_activeperiodstart = currentKA.GetAttributeValue<DateTime>("tss_activestartdate");
        //                            _DL_tss_mastermarketsize.tss_activeperiodsend = currentKA.GetAttributeValue<DateTime>("tss_activeenddate");
        //                            _DL_tss_mastermarketsize.tss_keyaccountid = currentKA.Id;
        //                            _DL_tss_mastermarketsize.tss_msperiod = currentKA.GetAttributeValue<int>("tss_revision");
        //                            _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;

        //                            _masterMarketSize = _DL_tss_mastermarketsize.Insert(organizationService);

        //                            #region Update Status
        //                            _DL_tss_keyaccount = new DL_tss_keyaccount();
        //                            _DL_tss_keyaccount.tss_status = STATUS_CALCULATE;
        //                            _DL_tss_keyaccount.tss_reason = STATUS_REASON_CALCULATE;
        //                            _DL_tss_keyaccount.UpdateStatus(organizationService, currentUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

        //                            _DL_tss_kauio = new DL_tss_kauio();
        //                            _DL_tss_kauio.tss_calculatestatus = true;
        //                            _DL_tss_kauio.tss_errordescription = "";
        //                            _DL_tss_kauio.UpdateStatus(organizationService, currentUIO.GetAttributeValue<Guid>("tss_kauioid"));
        //                            #endregion

        //                            #region Generate Lines
        //                            GenerateMasterMSLines(organizationService, _DL_tss_mastermarketsize, pop, setup);
        //                            #endregion
        //                        }
        //                    }
        //                    else
        //                    {
        //                        List<string> _errorcollection = new List<string>();
        //                        string _errormessage = "";

        //                        //Generate MS UIO/Non UIO
        //                        QueryExpression qPro = new QueryExpression(_DL_product.EntityName);
        //                        qPro.Criteria.AddCondition("productid", ConditionOperator.Equal, pop.GetAttributeValue<EntityReference>("trs_productmaster").Id);
        //                        qPro.ColumnSet.AddColumn("tss_usetyre");
        //                        bool useTyre = _DL_product.Select(organizationService, qPro).Entities.FirstOrDefault().GetAttributeValue<bool>("tss_usetyre");

        //                        if (!pop.Contains("tss_mscurrenthourmeter"))
        //                            _errorcollection.Add("Current Hour Meter");

        //                        if (!pop.Contains("tss_mscurrenthourmeterdate"))
        //                            _errorcollection.Add("Current Hour Meter (Date)");

        //                        if (!pop.Contains("tss_mslasthourmeter"))
        //                            _errorcollection.Add("Last Hour Meter");

        //                        if (!pop.Contains("tss_mslasthourmeterdate"))
        //                            _errorcollection.Add("Last Hour Meter (Date)");

        //                        if (!pop.Contains("new_deliverydate"))
        //                            _errorcollection.Add("Delivery Date");

        //                        if (_errorcollection.Count() > 0)
        //                        {
        //                            foreach (var item in _errorcollection)
        //                            {
        //                                _errormessage += item + ", ";
        //                            }

        //                            _errormessage = _errormessage.Substring(0, _errormessage.Length - 1) + " still not define !";
        //                        }
        //                        else
        //                            _errormessage = "Current Hour Meter, Current Hour Meter (Date), Last Hour Meter , Last Hour Meter (Date), Delivery Date or Overhaul Date still not define";

        //                        _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();

        //                        _DL_tss_mastermarketsize.tss_pss = currentUIO.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                        _DL_tss_mastermarketsize.tss_customer = currentUIO.GetAttributeValue<EntityReference>("tss_customer").Id; // pop.GetAttributeValue<EntityReference>("new_customercode").Id;
        //                        _DL_tss_mastermarketsize.tss_serialnumber = pop.GetAttributeValue<Guid>("new_populationid");
        //                        if (pop.Contains("tss_populationstatus"))
        //                        {
        //                            _DL_tss_mastermarketsize.tss_unittype = pop.GetAttributeValue<bool>("tss_populationstatus") ? UNITTYPE_UIO : UNITTYPE_NONUIO;
        //                        }
        //                        _DL_tss_mastermarketsize.tss_status = STATUS_ERROR_MS;
        //                        _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_ERROR_MS;
        //                        _DL_tss_mastermarketsize.tss_errormessage = _errormessage;
        //                        _DL_tss_mastermarketsize.tss_usetyre = useTyre;
        //                        _DL_tss_mastermarketsize.tss_msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                        _DL_tss_mastermarketsize.tss_msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend");
        //                        _DL_tss_mastermarketsize.tss_activeperiodstart = currentKA.GetAttributeValue<DateTime>("tss_activestartdate");
        //                        _DL_tss_mastermarketsize.tss_activeperiodsend = currentKA.GetAttributeValue<DateTime>("tss_activeenddate");
        //                        _DL_tss_mastermarketsize.tss_keyaccountid = currentKA.Id;
        //                        _DL_tss_mastermarketsize.tss_msperiod = currentKA.GetAttributeValue<int>("tss_revision");
        //                        _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;

        //                        _DL_tss_mastermarketsize.Insert(organizationService);

        //                        #region Update Status
        //                        _DL_tss_keyaccount = new DL_tss_keyaccount();
        //                        _DL_tss_keyaccount.tss_status = STATUS_ERROR;
        //                        _DL_tss_keyaccount.UpdateStatus(organizationService, currentUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

        //                        _DL_tss_kauio = new DL_tss_kauio();
        //                        _DL_tss_kauio.tss_calculatestatus = false;
        //                        _DL_tss_kauio.tss_errordescription = _errormessage;
        //                        _DL_tss_kauio.UpdateStatus(organizationService, currentUIO.GetAttributeValue<Guid>("tss_kauioid"));
        //                        #endregion
        //                    }
        //                }
        //            }
        //        }
        //        #endregion

        //        #endregion

        //        #region Generate KA Group UIO Commodity
        //        #region Get KA Group UIO Commodity
        //        //Get KA UIO
        //        FilterExpression fkaUIOCommodity = new FilterExpression(LogicalOperator.And);
        //        fkaUIOCommodity.AddCondition("tss_keyaccountid", ConditionOperator.In, kaIds);
        //        fkaUIOCommodity.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true);
        //        fkaUIOCommodity.AddCondition("tss_groupuiocommodity", ConditionOperator.NotNull);

        //        QueryExpression qkaUIOCommodity = new QueryExpression(_DL_tss_kagroupuiocommodity.EntityName);
        //        qkaUIOCommodity.ColumnSet = new ColumnSet(true);
        //        qkaUIOCommodity.Criteria.AddFilter(fkaUIOCommodity);
        //        EntityCollection listKAGroupUIOCommodity = _DL_tss_kagroupuiocommodity.Select(organizationService, qkaUIOCommodity);
        //        #endregion

        //        #region Generate
        //        if (listKAGroupUIOCommodity.Entities.Count > 0)
        //        {
        //            #region Get Group Commodity Account
        //            object[] kaGroupUIOCommIds = listKAGroupUIOCommodity.Entities.Select(x => (object)x.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id).ToArray();

        //            FilterExpression fGroupAccount = new FilterExpression(LogicalOperator.And);
        //            fGroupAccount.AddCondition("tss_groupuiocommodityaccountid", ConditionOperator.In, kaGroupUIOCommIds);
        //            fGroupAccount.AddCondition("tss_groupuiocommodityheader", ConditionOperator.NotNull);

        //            QueryExpression qGroupAccount = new QueryExpression(_DL_tss_groupuiocommodityaccount.EntityName);
        //            qGroupAccount.ColumnSet = new ColumnSet(true);
        //            qGroupAccount.Criteria.AddFilter(fGroupAccount);
        //            EntityCollection listGroupAccount = _DL_tss_groupuiocommodityaccount.Select(organizationService, qGroupAccount);
        //            #endregion

        //            if (listGroupAccount.Entities.Count > 0)
        //            {
        //                foreach (var groupAccount in listGroupAccount.Entities)
        //                {
        //                    Guid _masterMarketSize = new Guid();
        //                    Entity currentKAGroupUIO = listKAGroupUIOCommodity.Entities.Where(x => x.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id == groupAccount.GetAttributeValue<Guid>("tss_groupuiocommodityaccountid")).FirstOrDefault();
        //                    Entity currentKA = listKeyAccount.Entities.Where(x => x.Id == currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id).FirstOrDefault();

        //                    if (groupAccount.Contains("tss_qty") && groupAccount.GetAttributeValue<int>("tss_qty") > 0)
        //                    {
        //                        _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        //                        _DL_tss_mastermarketsize.tss_pss = currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                        _DL_tss_mastermarketsize.tss_customer = currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_customer").Id;
        //                        //_DL_tss_mastermarketsize.tss_groupuiocommodity = groupAccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityname").Id;
        //                        _DL_tss_mastermarketsize.tss_groupuiocommodityheader = groupAccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id;
        //                        _DL_tss_mastermarketsize.tss_qty = groupAccount.GetAttributeValue<int>("tss_qty");
        //                        _DL_tss_mastermarketsize.tss_unittype = UNITTYPE_COMMODITY;
        //                        _DL_tss_mastermarketsize.tss_status = STATUS_COMPLETED_MS;
        //                        _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_COMPLETED_MS;
        //                        _DL_tss_mastermarketsize.tss_msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                        _DL_tss_mastermarketsize.tss_msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend");
        //                        _DL_tss_mastermarketsize.tss_activeperiodstart = currentKA.GetAttributeValue<DateTime>("tss_activestartdate");
        //                        _DL_tss_mastermarketsize.tss_activeperiodsend = currentKA.GetAttributeValue<DateTime>("tss_activeenddate");
        //                        _DL_tss_mastermarketsize.tss_keyaccountid = currentKA.Id;
        //                        _DL_tss_mastermarketsize.tss_msperiod = currentKA.GetAttributeValue<int>("tss_revision");
        //                        _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;

        //                        _masterMarketSize = _DL_tss_mastermarketsize.Insert(organizationService);

        //                        #region Update Status
        //                        _DL_tss_keyaccount = new DL_tss_keyaccount();
        //                        _DL_tss_keyaccount.tss_status = STATUS_CALCULATE;
        //                        _DL_tss_keyaccount.UpdateStatus(organizationService, currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

        //                        _DL_tss_kagroupuiocommodity = new DL_tss_kagroupuiocommodity();
        //                        _DL_tss_kagroupuiocommodity.tss_calculatestatus = true;
        //                        _DL_tss_kagroupuiocommodity.UpdateStatus(organizationService, currentKAGroupUIO.GetAttributeValue<Guid>("tss_kagroupuiocommodityid"));
        //                        #endregion

        //                        #region Generate Sub Lines
        //                        Entity _tss_groupuiocommodityaccount = organizationService.Retrieve("tss_groupuiocommodityaccount", currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id, new ColumnSet(true));

        //                        if (_tss_groupuiocommodityaccount != null)
        //                        {
        //                            Entity _tss_groupuiocommodityheader = organizationService.Retrieve("tss_groupuiocommodityheader", _tss_groupuiocommodityaccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id, new ColumnSet(true));

        //                            FilterExpression fGroupUIOCommodity = new FilterExpression(LogicalOperator.And);
        //                            fGroupUIOCommodity.AddCondition("tss_groupuiocommodityheaderid", ConditionOperator.In, _tss_groupuiocommodityheader.Id);

        //                            QueryExpression qGroupUIOCommodity = new QueryExpression(_DL_tss_groupuiocommodity.EntityName);
        //                            qGroupUIOCommodity.ColumnSet = new ColumnSet(true);
        //                            qGroupUIOCommodity.Criteria.AddFilter(fGroupUIOCommodity);
        //                            EntityCollection listGroupUIOCommodity = _DL_tss_groupuiocommodity.Select(organizationService, qGroupUIOCommodity);

        //                            if (listGroupUIOCommodity.Entities.Count > 0)
        //                            {
        //                                foreach (var group in listGroupUIOCommodity.Entities)
        //                                {
        //                                    Entity _tss_groupuiocommodity = organizationService.Retrieve("tss_groupuiocommodity", group.Id, new ColumnSet(true));

        //                                    if (_tss_groupuiocommodity != null)
        //                                    {
        //                                        GenerateMasterMSSubLinesCommodity(organizationService, _DL_tss_mastermarketsize, _tss_groupuiocommodity, groupAccount, currentKAGroupUIO);
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        #endregion
        //                    }
        //                    else
        //                    {
        //                        _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        //                        _DL_tss_mastermarketsize.tss_pss = currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                        _DL_tss_mastermarketsize.tss_customer = currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_customer").Id;
        //                        //_DL_tss_mastermarketsize.tss_groupuiocommodity = groupAccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityname").Id;
        //                        _DL_tss_mastermarketsize.tss_groupuiocommodityheader = groupAccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id;
        //                        _DL_tss_mastermarketsize.tss_unittype = UNITTYPE_COMMODITY;
        //                        _DL_tss_mastermarketsize.tss_status = STATUS_ERROR_MS;
        //                        _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_ERROR_MS;
        //                        _DL_tss_mastermarketsize.tss_errormessage = "Qty still not define";
        //                        _DL_tss_mastermarketsize.tss_msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                        _DL_tss_mastermarketsize.tss_msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend");
        //                        _DL_tss_mastermarketsize.tss_activeperiodstart = currentKA.GetAttributeValue<DateTime>("tss_activestartdate");
        //                        _DL_tss_mastermarketsize.tss_activeperiodsend = currentKA.GetAttributeValue<DateTime>("tss_activeenddate");
        //                        _DL_tss_mastermarketsize.tss_keyaccountid = currentKA.Id;
        //                        _DL_tss_mastermarketsize.tss_msperiod = currentKA.GetAttributeValue<int>("tss_revision");
        //                        _DL_tss_mastermarketsize.tss_ismsresultpssgenerated = false;

        //                        _DL_tss_mastermarketsize.Insert(organizationService);

        //                        #region Update Status
        //                        _DL_tss_keyaccount = new DL_tss_keyaccount();
        //                        _DL_tss_keyaccount.tss_status = STATUS_ERROR;
        //                        _DL_tss_keyaccount.UpdateStatus(organizationService, currentKAGroupUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

        //                        _DL_tss_kagroupuiocommodity = new DL_tss_kagroupuiocommodity();
        //                        _DL_tss_kagroupuiocommodity.tss_calculatestatus = false;
        //                        _DL_tss_kagroupuiocommodity.UpdateStatus(organizationService, currentKAGroupUIO.GetAttributeValue<Guid>("tss_kagroupuiocommodityid"));
        //                        #endregion
        //                    }
        //                }
        //            }
        //        }
        //        #endregion
        //        #endregion

        //        //Guid[] customerID = listKeyAccount.Entities.GroupBy(x => x.GetAttributeValue<EntityReference>("tss_customer").Id).Select(x => x.First().GetAttributeValue<EntityReference>("tss_customer").Id).ToArray();

        //        //BL_tss_marketsizeresultpss _BL_tss_marketsizeresultpss = new BL_tss_marketsizeresultpss();
        //        //_BL_tss_marketsizeresultpss.GenerateMarketSizeResultPSS_New(organizationService, context, customerID);

        //        BL_tss_marketsizesummarybypartnumber _BL_tss_marketsizesummarybypartnumber = new BL_tss_marketsizesummarybypartnumber();
        //        _BL_tss_marketsizesummarybypartnumber.GenerateMarketSizeSummaryByPartNumber(organizationService, context);

        //        BL_tss_marketsizesummarybygroupuiocommodity _BL_tss_marketsizesummarybygroupuiocommodity = new BL_tss_marketsizesummarybygroupuiocommodity();
        //        _BL_tss_marketsizesummarybygroupuiocommodity.GenerateMarketSizeSummaryByGroupUIOCommodity(organizationService, context, listKeyAccount);
        //    }
        //    else
        //    {
        //        throw new InvalidWorkflowException("KAUIO or KA Group UIO Commodity not exist.");
        //    }
        //}





        //public void GenerateMasterMS(IOrganizationService organizationService, IWorkflowContext context)
        //{
        //    #region Get Key Account
        //    FilterExpression fStatus = new FilterExpression(LogicalOperator.Or);
        //    fStatus.AddCondition("tss_status", ConditionOperator.Equal, STATUS_OPEN);
        //    fStatus.AddCondition("tss_status", ConditionOperator.Equal, STATUS_ERROR);

        //    FilterExpression fKA = new FilterExpression(LogicalOperator.And);
        //    fKA.AddFilter(fStatus);
        //    fKA.AddCondition("tss_pss", ConditionOperator.Equal, context.UserId);
        //    fKA.AddCondition("tss_customer", ConditionOperator.NotNull);
        //    fKA.AddCondition("tss_activeenddate", ConditionOperator.GreaterEqual, DateTime.Today);
        //    fKA.AddCondition("tss_activestartdate", ConditionOperator.LessEqual, DateTime.Today);
        //    fKA.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true); //2018.10.12

        //    QueryExpression qKA = new QueryExpression(_DL_tss_keyaccount.EntityName);
        //    qKA.Criteria.AddFilter(fKA);
        //    qKA.ColumnSet = new ColumnSet(true);
        //    EntityCollection listKeyAccount = _DL_tss_mastermarketsize.Select(organizationService, qKA);
        //    #endregion

        //    if (listKeyAccount.Entities.Count > 0)
        //    {
        //        #region Generate MS KAUIO
        //        #region Get KA UIO
        //        //Get KA UIO
        //        object[] kaIds = listKeyAccount.Entities.Select(x => (object)x.Id).ToArray();
        //        FilterExpression fKAUIO = new FilterExpression(LogicalOperator.And);
        //        fKAUIO.AddCondition("tss_keyaccountid", ConditionOperator.In, kaIds);
        //        fKAUIO.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true);
        //        fKAUIO.AddCondition("tss_serialnumber", ConditionOperator.NotNull);

        //        QueryExpression qKAUIO = new QueryExpression(_DL_tss_kauio.EntityName);
        //        qKAUIO.ColumnSet = new ColumnSet(true);
        //        qKAUIO.Criteria.AddFilter(fKAUIO);
        //        EntityCollection listKAUIO = _DL_tss_kauio.Select(organizationService, qKAUIO);
        //        #endregion

        //        #region Generatex
        //        //Check Population
        //        if (listKAUIO.Entities.Count > 0)
        //        {
        //            object[] serialNums = listKAUIO.Entities.Select(x => (object)x.GetAttributeValue<EntityReference>("tss_serialnumber").Id).ToArray();

        //            FilterExpression fPop = new FilterExpression(LogicalOperator.And);
        //            fPop.AddCondition("new_populationid", ConditionOperator.In, serialNums);
        //            fPop.AddCondition("trs_productmaster", ConditionOperator.NotNull);

        //            QueryExpression qPop = new QueryExpression(_DL_population.EntityName);
        //            qPop.ColumnSet = new ColumnSet(true);
        //            qPop.Criteria.AddFilter(fPop);
        //            EntityCollection listPop = _DL_population.Select(organizationService, qPop);

        //            if (listPop.Entities.Count > 0)
        //            {
        //                List<Guid> msIds = new List<Guid>();

        //                foreach (var pop in listPop.Entities)
        //                {
        //                    Guid _masterMarketSize = new Guid();
        //                    Entity currentUIO = listKAUIO.Entities.Where(x => x.GetAttributeValue<EntityReference>("tss_serialnumber").Id == pop.GetAttributeValue<Guid>("new_populationid")).FirstOrDefault();
        //                    Entity currentKA = listKeyAccount.Entities.Where(x => x.Id == currentUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id).FirstOrDefault();

        //                    bool Method1 = false;
        //                    bool Method2 = false;
        //                    bool Method3 = false;
        //                    bool Method4 = false;
        //                    bool Method5 = false;

        //                    //CHECK METHOD 1
        //                    if (pop.Attributes.Contains("tss_mscurrenthourmeter")
        //                        && pop.Attributes.Contains("tss_mslasthourmeter")
        //                        && pop.Attributes.Contains("tss_mscurrenthourmeterdate")
        //                        && pop.Attributes.Contains("tss_mslasthourmeterdate"))
        //                    {
        //                        Method1 = true;
        //                        Console.Write("Method 1 : " + Method1);
        //                    }

        //                    //CHECK METHOD 2
        //                    if (pop.Attributes.Contains("tss_mscurrenthourmeter")
        //                        && pop.Attributes.Contains("tss_mscurrenthourmeterdate")
        //                        && (pop.Attributes.Contains("new_deliverydate") || pop.Attributes.Contains("trs_warrantyenddate")))
        //                    {
        //                        Method2 = true;
        //                        Console.Write("Method 2 : " + Method2);
        //                    }

        //                    //CHECK METHOD 3
        //                    if (pop.Attributes.Contains("tss_estworkinghour"))
        //                    {
        //                        Method3 = true;
        //                        Console.Write("Method 3 : " + Method3);
        //                    }

        //                    //CHECK METHOD 4
        //                    if (pop.Attributes.Contains("new_deliverydate")
        //                        && currentKA.Attributes.Contains("tss_msperiodstart")
        //                        && currentKA.Attributes.Contains("tss_msperiodend"))
        //                    {
        //                        Method4 = true;
        //                        Console.Write("Method 4 : " + Method4);
        //                    }

        //                    //CHECK METHOD 5
        //                    if ((pop.Attributes.Contains("new_deliverydate") || pop.Attributes.Contains("trs_warrantyenddate"))
        //                        && currentKA.Attributes.Contains("tss_msperiodstart")
        //                        && currentKA.Attributes.Contains("tss_msperiodend"))
        //                    {
        //                        Method5 = true;
        //                        Console.Write("Method 5 : " + Method5);
        //                    }

        //                    if (Method1 || Method2 || Method3 || Method4 || Method5)
        //                    {
        //                        //Generate MS UIO/Non UIO
        //                        decimal currentHM;
        //                        decimal lastHM;
        //                        DateTime currentHMDate;
        //                        DateTime lastHMDate;
        //                        DateTime deliveryDate;
        //                        DateTime warrantyEndDate = DateTime.MinValue;
        //                        DateTime calculateMarketSizeDate = DateTime.Now.ToLocalTime().Date;

        //                        QueryExpression qPro = new QueryExpression(_DL_product.EntityName);
        //                        qPro.Criteria.AddCondition("productid", ConditionOperator.Equal, pop.GetAttributeValue<EntityReference>("trs_productmaster").Id);
        //                        qPro.ColumnSet.AddColumn("tss_usetyre");
        //                        bool useTyre = _DL_product.Select(organizationService, qPro).Entities.FirstOrDefault().GetAttributeValue<bool>("tss_usetyre");

        //                        QueryExpression qSetup = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
        //                        qSetup.ColumnSet = new ColumnSet(true);
        //                        EntityCollection setups = _DL_tss_sparepartsetup.Select(organizationService, qSetup);

        //                        if (setups.Entities.Count > 0)
        //                        {
        //                            Entity setup = setups.Entities[0];
        //                            _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        //                            _DL_tss_mastermarketsize.tss_pss = currentUIO.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                            _DL_tss_mastermarketsize.tss_customer = currentUIO.GetAttributeValue<EntityReference>("tss_customer").Id; // pop.GetAttributeValue<EntityReference>("new_customercode").Id;
        //                            _DL_tss_mastermarketsize.tss_serialnumber = pop.GetAttributeValue<Guid>("new_populationid");

        //                            //METHOD 1
        //                            if (pop.Contains("tss_mscurrenthourmeter")
        //                               && pop.Contains("tss_mslasthourmeter")
        //                               && pop.Contains("tss_mscurrenthourmeterdate")
        //                               && pop.Contains("tss_mslasthourmeterdate"))
        //                            {
        //                                currentHM = pop.GetAttributeValue<decimal>("tss_mscurrenthourmeter");
        //                                lastHM = pop.GetAttributeValue<decimal>("tss_mslasthourmeter");
        //                                currentHMDate = pop.GetAttributeValue<DateTime>("tss_mscurrenthourmeterdate").ToLocalTime().Date;
        //                                lastHMDate = pop.GetAttributeValue<DateTime>("tss_mslasthourmeterdate").ToLocalTime().Date;

        //                                //if ((decimal)(currentHMDate - lastHMDate).TotalDays == 0)
        //                                //    _DL_tss_mastermarketsize.tss_avghmmethod1 = 0;
        //                                //else
        //                                //    _DL_tss_mastermarketsize.tss_avghmmethod1 = avghmmethod1 > (decimal)24 ? (decimal)24 : 0;

        //                                if ((decimal)(currentHMDate - lastHMDate).TotalDays > 0)
        //                                {
        //                                    decimal avghmmethod1 = (currentHM - lastHM) / ((decimal)(currentHMDate - lastHMDate).TotalDays);

        //                                    if (avghmmethod1 > 0)
        //                                    {
        //                                        _DL_tss_mastermarketsize.tss_avghmmethod1 = avghmmethod1 > (decimal)24 ? (decimal)24 : avghmmethod1;
        //                                    }
        //                                }
        //                            }

        //                            //METHOD 2
        //                            if (pop.Contains("tss_mscurrenthourmeter")
        //                               && pop.Contains("tss_mscurrenthourmeterdate")
        //                               && (pop.Contains("new_deliverydate") || pop.Contains("trs_warrantyenddate")))
        //                            {
        //                                currentHM = pop.GetAttributeValue<decimal>("tss_mscurrenthourmeter");
        //                                currentHMDate = pop.GetAttributeValue<DateTime>("tss_mscurrenthourmeterdate").ToLocalTime().Date;
        //                                deliveryDate = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().Date;

        //                                if (!pop.Attributes.Contains("trs_warrantyenddate"))
        //                                {
        //                                    warrantyEndDate = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().AddYears(1).Date;
        //                                }
        //                                else
        //                                {
        //                                    warrantyEndDate = pop.GetAttributeValue<DateTime>("trs_warrantyenddate").ToLocalTime().Date;
        //                                }

        //                                //if ((decimal)(currentHMDate - deliveryDate).TotalDays == 0)
        //                                //    _DL_tss_mastermarketsize.tss_avghmmethod2 = 0;
        //                                //else
        //                                //    _DL_tss_mastermarketsize.tss_avghmmethod2 = avghmmethod2 > (decimal)24 ? (decimal)24 : 0;

        //                                if ((decimal)(currentHMDate - warrantyEndDate).TotalDays > 0)
        //                                {
        //                                    decimal avghmmethod2 = currentHM / ((decimal)(currentHMDate - warrantyEndDate).TotalDays);

        //                                    if (avghmmethod2 > 0)
        //                                    {
        //                                        _DL_tss_mastermarketsize.tss_avghmmethod2 = avghmmethod2 > (decimal)24 ? (decimal)24 : avghmmethod2;
        //                                    }
        //                                }
        //                            }

        //                            //METHOD 3
        //                            if (pop.Contains("tss_estworkinghour"))
        //                            {
        //                                decimal estimateWH = pop.GetAttributeValue<int>("tss_estworkinghour");

        //                                if (estimateWH > 0)
        //                                {
        //                                    _DL_tss_mastermarketsize.tss_avghmmethod3 = estimateWH > (decimal)24 ? (decimal)24 : estimateWH;
        //                                }
        //                            }

        //                            //METHOD 4
        //                            if (pop.Contains("new_deliverydate")
        //                                && currentKA.Contains("tss_msperiodstart")
        //                                && currentKA.Contains("tss_msperiodend"))
        //                            {
        //                                CalculateDate oCalculate = new CalculateDate();

        //                                deliveryDate = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().Date;
        //                                DateTime msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart").ToLocalTime().Date;
        //                                DateTime msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend").ToLocalTime().Date;

        //                                decimal oDeliveryDate = oCalculate.DiffYear(deliveryDate, calculateMarketSizeDate);
        //                                decimal oPeriodDate = oCalculate.DiffYear(msperiodstart, msperiodend);

        //                                decimal periodpmmethod4 = (decimal)(oDeliveryDate + oPeriodDate);

        //                                if (periodpmmethod4 > 0)
        //                                {
        //                                    _DL_tss_mastermarketsize.tss_periodpmmethod4 = periodpmmethod4;
        //                                }
        //                            }

        //                            //METHOD 5
        //                            if (pop.Contains("new_deliverydate")
        //                                && currentKA.Contains("tss_msperiodstart")
        //                                && currentKA.Contains("tss_msperiodend"))
        //                            {
        //                                if (!pop.Attributes.Contains("trs_warrantyenddate"))
        //                                {
        //                                    warrantyEndDate = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().AddYears(1).Date;
        //                                }
        //                                else
        //                                {
        //                                    warrantyEndDate = pop.GetAttributeValue<DateTime>("trs_warrantyenddate").ToLocalTime().Date;
        //                                }

        //                                CalculateDate oCalculate = new CalculateDate();

        //                                deliveryDate = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().Date;
        //                                DateTime msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart").ToLocalTime().Date;
        //                                DateTime msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend").ToLocalTime().Date;

        //                                decimal oWarrantyDate = oCalculate.DiffYear(deliveryDate, warrantyEndDate);
        //                                decimal oPeriodDate = oCalculate.DiffYear(msperiodstart, msperiodend);

        //                                decimal periodpmmethod5 = (decimal)(oWarrantyDate + oPeriodDate);

        //                                if (periodpmmethod5 > 0)
        //                                {
        //                                    _DL_tss_mastermarketsize.tss_periodpmmethod5 = periodpmmethod5;
        //                                }
        //                            }

        //                            if (pop.Contains("tss_populationstatus"))
        //                            {
        //                                _DL_tss_mastermarketsize.tss_unittype = pop.GetAttributeValue<bool>("tss_populationstatus") ? UNITTYPE_UIO : UNITTYPE_NONUIO;
        //                            }
        //                            _DL_tss_mastermarketsize.tss_status = STATUS_COMPLETED_MS;
        //                            _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_COMPLETED_MS;
        //                            _DL_tss_mastermarketsize.tss_usetyre = useTyre;
        //                            _DL_tss_mastermarketsize.tss_msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                            _DL_tss_mastermarketsize.tss_msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend");
        //                            _DL_tss_mastermarketsize.tss_activeperiodstart = currentKA.GetAttributeValue<DateTime>("tss_activestartdate");
        //                            _DL_tss_mastermarketsize.tss_activeperiodsend = currentKA.GetAttributeValue<DateTime>("tss_activeenddate");
        //                            _masterMarketSize = _DL_tss_mastermarketsize.Insert(organizationService);

        //                            #region Update Status
        //                            _DL_tss_keyaccount = new DL_tss_keyaccount();
        //                            _DL_tss_keyaccount.tss_status = STATUS_CALCULATE;
        //                            _DL_tss_keyaccount.tss_reason = STATUS_REASON_CALCULATE;
        //                            _DL_tss_keyaccount.UpdateStatus(organizationService, currentUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

        //                            _DL_tss_kauio = new DL_tss_kauio();
        //                            _DL_tss_kauio.tss_calculatestatus = true;
        //                            _DL_tss_kauio.UpdateStatus(organizationService, currentUIO.GetAttributeValue<Guid>("tss_kauioid"));
        //                            #endregion

        //                            #region Generate Lines
        //                            GenerateMasterMSLines(organizationService, _DL_tss_mastermarketsize, pop, setup);
        //                            #endregion
        //                        }
        //                    }
        //                    else
        //                    {
        //                        //Generate MS UIO/Non UIO
        //                        QueryExpression qPro = new QueryExpression(_DL_product.EntityName);
        //                        qPro.Criteria.AddCondition("productid", ConditionOperator.Equal, pop.GetAttributeValue<EntityReference>("trs_productmaster").Id);
        //                        qPro.ColumnSet.AddColumn("tss_usetyre");
        //                        bool useTyre = _DL_product.Select(organizationService, qPro).Entities.FirstOrDefault().GetAttributeValue<bool>("tss_usetyre");

        //                        _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();

        //                        _DL_tss_mastermarketsize.tss_pss = currentUIO.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                        _DL_tss_mastermarketsize.tss_customer = currentUIO.GetAttributeValue<EntityReference>("tss_customer").Id; // pop.GetAttributeValue<EntityReference>("new_customercode").Id;
        //                        _DL_tss_mastermarketsize.tss_serialnumber = pop.GetAttributeValue<Guid>("new_populationid");
        //                        if (pop.Contains("tss_populationstatus"))
        //                        {
        //                            _DL_tss_mastermarketsize.tss_unittype = pop.GetAttributeValue<bool>("tss_populationstatus") ? UNITTYPE_UIO : UNITTYPE_NONUIO;
        //                        }
        //                        _DL_tss_mastermarketsize.tss_status = STATUS_ERROR_MS;
        //                        _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_ERROR_MS;
        //                        _DL_tss_mastermarketsize.tss_errormessage = "Current Hour Meter, Current Hour Meter (Date), Last Hour Meter , Last Hour Meter (Date), Delivery Date or Overhaul Date still not define";
        //                        _DL_tss_mastermarketsize.tss_usetyre = useTyre;
        //                        _DL_tss_mastermarketsize.tss_msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                        _DL_tss_mastermarketsize.tss_msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend");
        //                        _DL_tss_mastermarketsize.tss_activeperiodstart = currentKA.GetAttributeValue<DateTime>("tss_activestartdate");
        //                        _DL_tss_mastermarketsize.tss_activeperiodsend = currentKA.GetAttributeValue<DateTime>("tss_activeenddate");

        //                        _DL_tss_mastermarketsize.Insert(organizationService);

        //                        #region Update Status
        //                        _DL_tss_keyaccount = new DL_tss_keyaccount();
        //                        _DL_tss_keyaccount.tss_status = STATUS_ERROR;
        //                        _DL_tss_keyaccount.UpdateStatus(organizationService, currentUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

        //                        _DL_tss_kauio = new DL_tss_kauio();
        //                        _DL_tss_kauio.tss_calculatestatus = false;
        //                        _DL_tss_kauio.UpdateStatus(organizationService, currentUIO.GetAttributeValue<Guid>("tss_kauioid"));
        //                        #endregion
        //                    }
        //                }
        //            }
        //        }
        //        #endregion

        //        #endregion

        //        #region Generate KA Group UIO Commodity
        //        #region Get KA Group UIO Commodity
        //        //Get KA UIO
        //        FilterExpression fkaUIOCommodity = new FilterExpression(LogicalOperator.And);
        //        fkaUIOCommodity.AddCondition("tss_keyaccountid", ConditionOperator.In, kaIds);
        //        fkaUIOCommodity.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true);
        //        fkaUIOCommodity.AddCondition("tss_groupuiocommodity", ConditionOperator.NotNull);

        //        QueryExpression qkaUIOCommodity = new QueryExpression(_DL_tss_kagroupuiocommodity.EntityName);
        //        qkaUIOCommodity.ColumnSet = new ColumnSet(true);
        //        qkaUIOCommodity.Criteria.AddFilter(fkaUIOCommodity);
        //        EntityCollection listKAUIOGroupCommodity = _DL_tss_kauio.Select(organizationService, qkaUIOCommodity);
        //        #endregion

        //        #region Generate
        //        if (listKAUIOGroupCommodity.Entities.Count > 0)
        //        {
        //            #region Get Group Commodity Account
        //            object[] groups = listKAUIOGroupCommodity.Entities.Select(x => (object)x.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id).ToArray();

        //            FilterExpression fGroup = new FilterExpression(LogicalOperator.And);
        //            fGroup.AddCondition("tss_groupuiocommodityaccountid", ConditionOperator.In, groups);
        //            fGroup.AddCondition("tss_groupuiocommodityname", ConditionOperator.NotNull);

        //            QueryExpression qGroup = new QueryExpression(_DL_tss_groupuiocommodityaccount.EntityName);
        //            qGroup.ColumnSet = new ColumnSet(true);
        //            qGroup.Criteria.AddFilter(fGroup);
        //            EntityCollection listGroup = _DL_tss_groupuiocommodityaccount.Select(organizationService, qGroup);
        //            #endregion

        //            if (listGroup.Entities.Count > 0)
        //            {
        //                foreach (var group in listGroup.Entities)
        //                {
        //                    Guid _masterMarketSize = new Guid();
        //                    Entity currentUIOGroup = listKAUIOGroupCommodity.Entities.Where(x => x.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id == group.GetAttributeValue<Guid>("tss_groupuiocommodityaccountid")).FirstOrDefault();
        //                    Entity currentKA = listKeyAccount.Entities.Where(x => x.Id == currentUIOGroup.GetAttributeValue<EntityReference>("tss_keyaccountid").Id).FirstOrDefault();

        //                    if (group.Contains("tss_qty") && group.GetAttributeValue<int>("tss_qty") > 0)
        //                    {
        //                        _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        //                        _DL_tss_mastermarketsize.tss_pss = currentUIOGroup.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                        _DL_tss_mastermarketsize.tss_customer = currentUIOGroup.GetAttributeValue<EntityReference>("tss_customer").Id;
        //                        _DL_tss_mastermarketsize.tss_groupuiocommodity = group.GetAttributeValue<EntityReference>("tss_groupuiocommodityname").Id;
        //                        _DL_tss_mastermarketsize.tss_qty = group.GetAttributeValue<int>("tss_qty");
        //                        _DL_tss_mastermarketsize.tss_unittype = UNITTYPE_COMMODITY;
        //                        _DL_tss_mastermarketsize.tss_status = STATUS_COMPLETED_MS;
        //                        _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_COMPLETED_MS;
        //                        _DL_tss_mastermarketsize.tss_msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                        _DL_tss_mastermarketsize.tss_msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend");
        //                        _DL_tss_mastermarketsize.tss_activeperiodstart = currentKA.GetAttributeValue<DateTime>("tss_activestartdate");
        //                        _DL_tss_mastermarketsize.tss_activeperiodsend = currentKA.GetAttributeValue<DateTime>("tss_activeenddate");
        //                        _masterMarketSize = _DL_tss_mastermarketsize.Insert(organizationService);

        //                        #region Update Status
        //                        _DL_tss_keyaccount = new DL_tss_keyaccount();
        //                        _DL_tss_keyaccount.tss_status = STATUS_CALCULATE;
        //                        _DL_tss_keyaccount.UpdateStatus(organizationService, currentUIOGroup.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

        //                        _DL_tss_kagroupuiocommodity = new DL_tss_kagroupuiocommodity();
        //                        _DL_tss_kagroupuiocommodity.tss_calculatestatus = true;
        //                        _DL_tss_kagroupuiocommodity.UpdateStatus(organizationService, currentUIOGroup.GetAttributeValue<Guid>("tss_kagroupuiocommodityid"));
        //                        #endregion

        //                        #region Generate Sub Lines
        //                        Entity _tss_groupuiocommodityaccount = organizationService.Retrieve("tss_groupuiocommodityaccount", currentUIOGroup.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id, new ColumnSet(true));

        //                        if (_tss_groupuiocommodityaccount != null)
        //                        {
        //                            //_DL_tss_mastermarketsizesublines.tss_partnumber = _tss_groupuiocommodityaccount.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id;
        //                            Entity _tss_groupuiocommodity = organizationService.Retrieve("tss_groupuiocommodity", _tss_groupuiocommodityaccount.GetAttributeValue<EntityReference>("tss_groupuiocommodityname").Id, new ColumnSet(true));

        //                            if (_tss_groupuiocommodity != null)
        //                            {
        //                                GenerateMasterMSSubLinesCommodity(organizationService, _DL_tss_mastermarketsize, _tss_groupuiocommodity, currentUIOGroup);
        //                            }
        //                        }
        //                        #endregion
        //                    }
        //                    else
        //                    {
        //                        _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        //                        _DL_tss_mastermarketsize.tss_pss = currentUIOGroup.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                        _DL_tss_mastermarketsize.tss_customer = currentUIOGroup.GetAttributeValue<EntityReference>("tss_customer").Id;
        //                        _DL_tss_mastermarketsize.tss_groupuiocommodity = group.GetAttributeValue<EntityReference>("tss_groupuiocommodityname").Id;
        //                        _DL_tss_mastermarketsize.tss_unittype = UNITTYPE_COMMODITY;
        //                        _DL_tss_mastermarketsize.tss_status = STATUS_ERROR_MS;
        //                        _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_ERROR_MS;
        //                        _DL_tss_mastermarketsize.tss_errormessage = "Qty still not define";
        //                        _DL_tss_mastermarketsize.tss_msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                        _DL_tss_mastermarketsize.tss_msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend");
        //                        _DL_tss_mastermarketsize.tss_activeperiodstart = currentKA.GetAttributeValue<DateTime>("tss_activestartdate");
        //                        _DL_tss_mastermarketsize.tss_activeperiodsend = currentKA.GetAttributeValue<DateTime>("tss_activeenddate");
        //                        _DL_tss_mastermarketsize.Insert(organizationService);

        //                        #region Update Status
        //                        _DL_tss_keyaccount = new DL_tss_keyaccount();
        //                        _DL_tss_keyaccount.tss_status = STATUS_ERROR;
        //                        _DL_tss_keyaccount.UpdateStatus(organizationService, currentUIOGroup.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

        //                        _DL_tss_kagroupuiocommodity = new DL_tss_kagroupuiocommodity();
        //                        _DL_tss_kagroupuiocommodity.tss_calculatestatus = false;
        //                        _DL_tss_kagroupuiocommodity.UpdateStatus(organizationService, currentUIOGroup.GetAttributeValue<Guid>("tss_kagroupuiocommodityid"));
        //                        #endregion
        //                    }
        //                }
        //            }
        //        }
        //        #endregion
        //        #endregion

        //        Guid[] customerID = listKeyAccount.Entities.GroupBy(x => x.GetAttributeValue<EntityReference>("tss_customer").Id).Select(x => x.First().GetAttributeValue<EntityReference>("tss_customer").Id).ToArray();

        //        BL_tss_marketsizeresultpss _BL_tss_marketsizeresultpss = new BL_tss_marketsizeresultpss();
        //        _BL_tss_marketsizeresultpss.GenerateMarketSizeResultPSS_New(organizationService, context, customerID);
        //    }
        //    else
        //    {
        //        throw new InvalidWorkflowException("KAUIO or KA Group UIO Commodity not exist.");
        //    }
        //}

        //public void GenerateMasterMS(IOrganizationService organizationService, IWorkflowContext context)
        //{
        //    #region Get Key Account
        //    FilterExpression fStatus = new FilterExpression(LogicalOperator.Or);
        //    fStatus.AddCondition("tss_status", ConditionOperator.Equal, STATUS_OPEN);
        //    fStatus.AddCondition("tss_status", ConditionOperator.Equal, STATUS_ERROR);

        //    FilterExpression fKA = new FilterExpression(LogicalOperator.And);
        //    fKA.AddFilter(fStatus);
        //    fKA.AddCondition("tss_pss", ConditionOperator.Equal, context.UserId);
        //    fKA.AddCondition("tss_customer", ConditionOperator.NotNull);
        //    fKA.AddCondition("tss_activeenddate", ConditionOperator.GreaterEqual, DateTime.Today);
        //    fKA.AddCondition("tss_activestartdate", ConditionOperator.LessEqual, DateTime.Today);
        //    fKA.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true); //2018.10.12

        //    QueryExpression qKA = new QueryExpression(_DL_tss_keyaccount.EntityName);
        //    qKA.Criteria.AddFilter(fKA);
        //    qKA.ColumnSet = new ColumnSet(true);
        //    EntityCollection listKeyAccount = _DL_tss_mastermarketsize.Select(organizationService, qKA);
        //    #endregion

        //    if (listKeyAccount.Entities.Count > 0)
        //    {
        //        #region Generate MS KAUIO
        //        #region Get KA UIO
        //        //Get KA UIO
        //        object[] kaIds = listKeyAccount.Entities.Select(x => (object)x.Id).ToArray();
        //        FilterExpression fKAUIO = new FilterExpression(LogicalOperator.And);
        //        fKAUIO.AddCondition("tss_keyaccountid", ConditionOperator.In, kaIds);
        //        fKAUIO.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true);
        //        fKAUIO.AddCondition("tss_serialnumber", ConditionOperator.NotNull);

        //        QueryExpression qKAUIO = new QueryExpression(_DL_tss_kauio.EntityName);
        //        qKAUIO.ColumnSet = new ColumnSet(true);
        //        qKAUIO.Criteria.AddFilter(fKAUIO);
        //        EntityCollection listKAUIO = _DL_tss_kauio.Select(organizationService, qKAUIO);
        //        #endregion

        //        #region Generatex
        //        //Check Population
        //        if (listKAUIO.Entities.Count > 0)
        //        {
        //            object[] serialNums = listKAUIO.Entities.Select(x => (object)x.GetAttributeValue<EntityReference>("tss_serialnumber").Id).ToArray();

        //            FilterExpression fPop = new FilterExpression(LogicalOperator.And);
        //            fPop.AddCondition("new_populationid", ConditionOperator.In, serialNums);
        //            fPop.AddCondition("trs_productmaster", ConditionOperator.NotNull);

        //            QueryExpression qPop = new QueryExpression(_DL_population.EntityName);
        //            qPop.ColumnSet = new ColumnSet(true);
        //            qPop.Criteria.AddFilter(fPop);
        //            EntityCollection listPop = _DL_population.Select(organizationService, qPop);

        //            if (listPop.Entities.Count > 0)
        //            {
        //                List<Guid> msIds = new List<Guid>();

        //                foreach (var pop in listPop.Entities)
        //                {
        //                    Guid _masterMarketSize = new Guid();
        //                    Entity currentUIO = listKAUIO.Entities.Where(x => x.GetAttributeValue<EntityReference>("tss_serialnumber").Id == pop.GetAttributeValue<Guid>("new_populationid")).FirstOrDefault();
        //                    Entity currentKA = listKeyAccount.Entities.Where(x => x.Id == currentUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id).FirstOrDefault();

        //                    bool Method1 = false;
        //                    bool Method2 = false;
        //                    bool Method3 = false;
        //                    bool Method4 = false;

        //                    //CHECK METHOD 1
        //                    if (pop.Attributes.Contains("new_latesthourmeter")
        //                        && pop.Attributes.Contains("trs_hourmeter")
        //                        && pop.Attributes.Contains("trs_datelatesthourmeter")
        //                        && pop.Attributes.Contains("trs_datehourmeter"))
        //                    {
        //                        Method1 = true;
        //                        Console.Write("Method 1 : " + Method1);
        //                    }

        //                    //CHECK METHOD 2
        //                    if (pop.Attributes.Contains("new_latesthourmeter")
        //                        && pop.Attributes.Contains("trs_datelatesthourmeter")
        //                        && (pop.Attributes.Contains("new_deliverydate") || pop.Attributes.Contains("trs_warrantyenddate")))
        //                    {
        //                        Method2 = true;
        //                        Console.Write("Method 2 : " + Method2);
        //                    }

        //                    //CHECK METHOD 3
        //                    if (pop.Attributes.Contains("tss_estworkinghour")
        //                        && pop.Attributes.Contains("tss_startdatems")
        //                        && (pop.Attributes.Contains("new_deliverydate") || pop.Attributes.Contains("trs_warrantyenddate")))
        //                    {
        //                        Method3 = true;
        //                        Console.Write("Method 3 : " + Method3);
        //                    }

        //                    //CHECK METHOD 4
        //                    if (pop.Attributes.Contains("new_deliverydate"))
        //                    {
        //                        Method4 = true;
        //                        Console.Write("Method 4 : " + Method4);
        //                    }

        //                    //if (pop.Contains("new_deliverydate")
        //                    //    || pop.Contains("tss_overhauldate"))
        //                    if (Method1 || Method2 || Method3 || Method4)
        //                    {
        //                        //Generate MS UIO/Non UIO
        //                        decimal currHM;
        //                        decimal lastHM;
        //                        DateTime currDt;
        //                        DateTime lastDt;
        //                        DateTime delivDt;
        //                        DateTime warrDt = DateTime.MinValue;

        //                        QueryExpression qPro = new QueryExpression(_DL_product.EntityName);
        //                        qPro.Criteria.AddCondition("productid", ConditionOperator.Equal, pop.GetAttributeValue<EntityReference>("trs_productmaster").Id);
        //                        qPro.ColumnSet.AddColumn("tss_usetyre");
        //                        bool useTyre = _DL_product.Select(organizationService, qPro).Entities.FirstOrDefault().GetAttributeValue<bool>("tss_usetyre");

        //                        QueryExpression qSetup = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
        //                        qSetup.ColumnSet = new ColumnSet(true);
        //                        EntityCollection setups = _DL_tss_sparepartsetup.Select(organizationService, qSetup);

        //                        if (setups.Entities.Count > 0)
        //                        {
        //                            Entity setup = setups.Entities[0];
        //                            _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        //                            _DL_tss_mastermarketsize.tss_pss = currentUIO.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                            _DL_tss_mastermarketsize.tss_customer = currentUIO.GetAttributeValue<EntityReference>("tss_customer").Id; // pop.GetAttributeValue<EntityReference>("new_customercode").Id;
        //                            _DL_tss_mastermarketsize.tss_serialnumber = pop.GetAttributeValue<Guid>("new_populationid");

        //                            //METHOD 1
        //                            if (pop.Contains("new_latesthourmeter")
        //                               && pop.Contains("trs_hourmeter")
        //                               && pop.Contains("trs_datelatesthourmeter")
        //                               && pop.Contains("trs_datehourmeter"))
        //                            {
        //                                currHM = pop.GetAttributeValue<decimal>("new_latesthourmeter");
        //                                lastHM = pop.GetAttributeValue<decimal>("trs_hourmeter");
        //                                currDt = pop.GetAttributeValue<DateTime>("trs_datelatesthourmeter").ToLocalTime().Date;
        //                                lastDt = pop.GetAttributeValue<DateTime>("trs_datehourmeter").ToLocalTime().Date;

        //                                if ((decimal)(currDt - lastDt).TotalDays == 0)
        //                                    _DL_tss_mastermarketsize.tss_avghmmethod1 = 0;
        //                                else
        //                                    _DL_tss_mastermarketsize.tss_avghmmethod1 = (currHM - lastHM) / ((decimal)(currDt - lastDt).TotalDays);
        //                            }

        //                            //METHOD 2
        //                            if (pop.Contains("new_latesthourmeter")
        //                               && pop.Contains("trs_datelatesthourmeter")
        //                               && pop.Contains("new_deliverydate"))
        //                            {
        //                                currHM = pop.GetAttributeValue<decimal>("new_latesthourmeter");
        //                                currDt = pop.GetAttributeValue<DateTime>("trs_datelatesthourmeter").ToLocalTime().Date;

        //                                delivDt = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().Date;

        //                                if (!pop.Attributes.Contains("trs_warrantyenddate"))
        //                                {
        //                                    warrDt = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().AddYears(1).Date;
        //                                }
        //                                else
        //                                {
        //                                    warrDt = pop.GetAttributeValue<DateTime>("trs_warrantyenddate").ToLocalTime().Date;
        //                                }

        //                                if ((decimal)(currDt - delivDt).TotalDays == 0)
        //                                    _DL_tss_mastermarketsize.tss_avghmmethod2 = 0;
        //                                else
        //                                    _DL_tss_mastermarketsize.tss_avghmmethod2 = currHM / ((decimal)(currDt - warrDt).TotalDays);
        //                            }

        //                            //METHOD 3
        //                            if (pop.Contains("tss_estworkinghour")
        //                               && setup.Contains("tss_startdatems")
        //                               && pop.Contains("new_deliverydate"))
        //                            {
        //                                decimal estimateWH = pop.GetAttributeValue<int>("tss_estworkinghour");
        //                                DateTime startMSDate = setup.GetAttributeValue<DateTime>("tss_startdatems").ToLocalTime().Date;

        //                                if (!pop.Attributes.Contains("trs_warrantyenddate"))
        //                                {
        //                                    warrDt = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().AddYears(1).Date;
        //                                }
        //                                else
        //                                {
        //                                    warrDt = pop.GetAttributeValue<DateTime>("trs_warrantyenddate").ToLocalTime().Date;
        //                                }

        //                                _DL_tss_mastermarketsize.tss_avghmmethod3 = estimateWH / ((decimal)(startMSDate - warrDt).TotalDays);
        //                            }

        //                            //METHOD 4
        //                            if (pop.Contains("new_deliverydate"))
        //                            {
        //                                DateTime zeroTime = new DateTime(1, 1, 1);

        //                                currDt = DateTime.Now.ToLocalTime().Date;
        //                                delivDt = pop.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime().Date;
        //                                DateTime msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart").ToLocalTime().Date;
        //                                DateTime msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend").ToLocalTime().Date;

        //                                CalculateDate oCalculateDate = new CalculateDate();

        //                                int oDelivery = oCalculateDate.DiffYear(delivDt, currDt);
        //                                int oPeriod = oCalculateDate.DiffYear(msperiodstart, msperiodend);

        //                                if ((decimal)(currDt - delivDt).TotalDays == 0)
        //                                    _DL_tss_mastermarketsize.tss_periodpmmethod4 = 0;
        //                                else
        //                                    _DL_tss_mastermarketsize.tss_periodpmmethod4 = (decimal)(oDelivery + oPeriod);
        //                            }

        //                            if (pop.Contains("tss_populationstatus"))
        //                            {
        //                                _DL_tss_mastermarketsize.tss_unittype = pop.GetAttributeValue<bool>("tss_populationstatus") ? UNITTYPE_UIO : UNITTYPE_NONUIO;
        //                            }
        //                            _DL_tss_mastermarketsize.tss_status = STATUS_COMPLETED_MS;
        //                            _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_COMPLETED_MS;
        //                            _DL_tss_mastermarketsize.tss_usetyre = useTyre;
        //                            _DL_tss_mastermarketsize.tss_msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                            _DL_tss_mastermarketsize.tss_msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend");
        //                            _DL_tss_mastermarketsize.tss_activeperiodstart = currentKA.GetAttributeValue<DateTime>("tss_activestartdate");
        //                            _DL_tss_mastermarketsize.tss_activeperiodsend = currentKA.GetAttributeValue<DateTime>("tss_activeenddate");
        //                            _masterMarketSize = _DL_tss_mastermarketsize.Insert(organizationService);

        //                            #region Update Status
        //                            _DL_tss_keyaccount = new DL_tss_keyaccount();
        //                            _DL_tss_keyaccount.tss_status = STATUS_CALCULATE;
        //                            _DL_tss_keyaccount.UpdateStatus(organizationService, currentUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

        //                            _DL_tss_kauio = new DL_tss_kauio();
        //                            _DL_tss_kauio.tss_calculatestatus = true;
        //                            _DL_tss_kauio.UpdateStatus(organizationService, currentUIO.GetAttributeValue<Guid>("tss_kauioid"));
        //                            #endregion

        //                            #region Generate Lines
        //                            GenerateMasterMSLines(organizationService, _DL_tss_mastermarketsize, pop, setup);
        //                            #endregion
        //                        }
        //                    }
        //                    else
        //                    {
        //                        //Generate MS UIO/Non UIO
        //                        QueryExpression qPro = new QueryExpression(_DL_product.EntityName);
        //                        qPro.Criteria.AddCondition("productid", ConditionOperator.Equal, pop.GetAttributeValue<EntityReference>("trs_productmaster").Id);
        //                        qPro.ColumnSet.AddColumn("tss_usetyre");
        //                        bool useTyre = _DL_product.Select(organizationService, qPro).Entities.FirstOrDefault().GetAttributeValue<bool>("tss_usetyre");

        //                        _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();

        //                        //if (pop.Contains("new_latesthourmeter")
        //                        //       && pop.Contains("trs_hourmeter")
        //                        //       && pop.Contains("trs_datelatesthourmeter")
        //                        //       && pop.Contains("trs_datehourmeter"))
        //                        //{
        //                        //    decimal currHM;
        //                        //    decimal lastHM;
        //                        //    DateTime currDt;
        //                        //    DateTime lastDt;

        //                        //    currHM = pop.GetAttributeValue<decimal>("new_latesthourmeter");
        //                        //    lastHM = pop.GetAttributeValue<decimal>("trs_hourmeter");
        //                        //    currDt = pop.GetAttributeValue<DateTime>("trs_datelatesthourmeter").Date;
        //                        //    lastDt = pop.GetAttributeValue<DateTime>("trs_datehourmeter").Date;

        //                        //    if ((decimal)(currDt - lastDt).TotalDays == 0)
        //                        //        _DL_tss_mastermarketsize.tss_avghmmethod1 = 0;
        //                        //    else
        //                        //        _DL_tss_mastermarketsize.tss_avghmmethod1 = (currHM - lastHM) / ((decimal)(currDt - lastDt).TotalDays);
        //                        //}

        //                        _DL_tss_mastermarketsize.tss_pss = currentUIO.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                        _DL_tss_mastermarketsize.tss_customer = currentUIO.GetAttributeValue<EntityReference>("tss_customer").Id; // pop.GetAttributeValue<EntityReference>("new_customercode").Id;
        //                        _DL_tss_mastermarketsize.tss_serialnumber = pop.GetAttributeValue<Guid>("new_populationid");
        //                        if (pop.Contains("tss_populationstatus"))
        //                        {
        //                            _DL_tss_mastermarketsize.tss_unittype = pop.GetAttributeValue<bool>("tss_populationstatus") ? UNITTYPE_UIO : UNITTYPE_NONUIO;
        //                        }
        //                        _DL_tss_mastermarketsize.tss_status = STATUS_ERROR_MS;
        //                        _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_ERROR_MS;
        //                        _DL_tss_mastermarketsize.tss_errormessage = "Current Hour Meter, Current Hour Meter (Date), Last Hour Meter , Last Hour Meter (Date), Delivery Date or Overhaul Date still not define";
        //                        _DL_tss_mastermarketsize.tss_usetyre = useTyre;
        //                        _DL_tss_mastermarketsize.tss_msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                        _DL_tss_mastermarketsize.tss_msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend");
        //                        _DL_tss_mastermarketsize.tss_activeperiodstart = currentKA.GetAttributeValue<DateTime>("tss_activestartdate");
        //                        _DL_tss_mastermarketsize.tss_activeperiodsend = currentKA.GetAttributeValue<DateTime>("tss_activeenddate");

        //                        _DL_tss_mastermarketsize.Insert(organizationService);

        //                        #region Update Status
        //                        _DL_tss_keyaccount = new DL_tss_keyaccount();
        //                        _DL_tss_keyaccount.tss_status = STATUS_ERROR;
        //                        _DL_tss_keyaccount.UpdateStatus(organizationService, currentUIO.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

        //                        _DL_tss_kauio = new DL_tss_kauio();
        //                        _DL_tss_kauio.tss_calculatestatus = false;
        //                        _DL_tss_kauio.UpdateStatus(organizationService, currentUIO.GetAttributeValue<Guid>("tss_kauioid"));
        //                        #endregion
        //                    }
        //                }
        //            }
        //        }
        //        #endregion

        //        #endregion

        //        #region Generate KA Group UIO Commodity
        //        #region Get KA Group UIO Commodity
        //        //Get KA UIO
        //        FilterExpression fkaUIOCommodity = new FilterExpression(LogicalOperator.And);
        //        fkaUIOCommodity.AddCondition("tss_keyaccountid", ConditionOperator.In, kaIds);
        //        fkaUIOCommodity.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true);
        //        fkaUIOCommodity.AddCondition("tss_groupuiocommodity", ConditionOperator.NotNull);

        //        QueryExpression qkaUIOCommodity = new QueryExpression(_DL_tss_kagroupuiocommodity.EntityName);
        //        qkaUIOCommodity.ColumnSet = new ColumnSet(true);
        //        qkaUIOCommodity.Criteria.AddFilter(fkaUIOCommodity);
        //        EntityCollection listKAUIOGroupCommodity = _DL_tss_kauio.Select(organizationService, qkaUIOCommodity);
        //        #endregion

        //        #region Generate
        //        if (listKAUIOGroupCommodity.Entities.Count > 0)
        //        {
        //            #region Get Group Commodity Account
        //            object[] groups = listKAUIOGroupCommodity.Entities.Select(x => (object)x.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id).ToArray();

        //            FilterExpression fGroup = new FilterExpression(LogicalOperator.And);
        //            fGroup.AddCondition("tss_groupuiocommodityaccountid", ConditionOperator.In, groups);
        //            fGroup.AddCondition("tss_groupuiocommodityname", ConditionOperator.NotNull);

        //            QueryExpression qGroup = new QueryExpression(_DL_tss_groupuiocommodityaccount.EntityName);
        //            qGroup.ColumnSet = new ColumnSet(true);
        //            qGroup.Criteria.AddFilter(fGroup);
        //            EntityCollection listGroup = _DL_tss_groupuiocommodityaccount.Select(organizationService, qGroup);
        //            #endregion

        //            if (listGroup.Entities.Count > 0)
        //            {
        //                foreach (var group in listGroup.Entities)
        //                {
        //                    Guid _masterMarketSize = new Guid();
        //                    Entity currentUIOGroup = listKAUIOGroupCommodity.Entities.Where(x => x.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id == group.GetAttributeValue<Guid>("tss_groupuiocommodityaccountid")).FirstOrDefault();
        //                    Entity currentKA = listKeyAccount.Entities.Where(x => x.Id == currentUIOGroup.GetAttributeValue<EntityReference>("tss_keyaccountid").Id).FirstOrDefault();

        //                    if (group.Contains("tss_qty") && group.GetAttributeValue<int>("tss_qty") > 0)
        //                    {
        //                        _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        //                        _DL_tss_mastermarketsize.tss_pss = currentUIOGroup.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                        _DL_tss_mastermarketsize.tss_customer = currentUIOGroup.GetAttributeValue<EntityReference>("tss_customer").Id;
        //                        _DL_tss_mastermarketsize.tss_groupuiocommodity = group.GetAttributeValue<EntityReference>("tss_groupuiocommodityname").Id;
        //                        _DL_tss_mastermarketsize.tss_qty = group.GetAttributeValue<int>("tss_qty");
        //                        _DL_tss_mastermarketsize.tss_unittype = UNITTYPE_COMMODITY;
        //                        _DL_tss_mastermarketsize.tss_status = STATUS_COMPLETED_MS;
        //                        _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_COMPLETED_MS;
        //                        _DL_tss_mastermarketsize.tss_msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                        _DL_tss_mastermarketsize.tss_msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend");
        //                        _DL_tss_mastermarketsize.tss_activeperiodstart = currentKA.GetAttributeValue<DateTime>("tss_activestartdate");
        //                        _DL_tss_mastermarketsize.tss_activeperiodsend = currentKA.GetAttributeValue<DateTime>("tss_activeenddate");
        //                        _masterMarketSize = _DL_tss_mastermarketsize.Insert(organizationService);

        //                        #region Update Status
        //                        _DL_tss_keyaccount = new DL_tss_keyaccount();
        //                        _DL_tss_keyaccount.tss_status = STATUS_CALCULATE;
        //                        _DL_tss_keyaccount.UpdateStatus(organizationService, currentUIOGroup.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

        //                        _DL_tss_kagroupuiocommodity = new DL_tss_kagroupuiocommodity();
        //                        _DL_tss_kagroupuiocommodity.tss_calculatestatus = true;
        //                        _DL_tss_kagroupuiocommodity.UpdateStatus(organizationService, currentUIOGroup.GetAttributeValue<Guid>("tss_kagroupuiocommodityid"));
        //                        #endregion
        //                    }
        //                    else
        //                    {
        //                        _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        //                        _DL_tss_mastermarketsize.tss_pss = currentUIOGroup.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                        _DL_tss_mastermarketsize.tss_customer = currentUIOGroup.GetAttributeValue<EntityReference>("tss_customer").Id;
        //                        _DL_tss_mastermarketsize.tss_groupuiocommodity = group.GetAttributeValue<EntityReference>("tss_groupuiocommodityname").Id;
        //                        _DL_tss_mastermarketsize.tss_unittype = UNITTYPE_COMMODITY;
        //                        _DL_tss_mastermarketsize.tss_status = STATUS_ERROR_MS;
        //                        _DL_tss_mastermarketsize.tss_statusreason = STATUSREASON_ERROR_MS;
        //                        _DL_tss_mastermarketsize.tss_errormessage = "Qty still not define";
        //                        _DL_tss_mastermarketsize.tss_msperiodstart = currentKA.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                        _DL_tss_mastermarketsize.tss_msperiodend = currentKA.GetAttributeValue<DateTime>("tss_msperiodend");
        //                        _DL_tss_mastermarketsize.tss_activeperiodstart = currentKA.GetAttributeValue<DateTime>("tss_activestartdate");
        //                        _DL_tss_mastermarketsize.tss_activeperiodsend = currentKA.GetAttributeValue<DateTime>("tss_activeenddate");
        //                        _DL_tss_mastermarketsize.Insert(organizationService);

        //                        #region Update Status
        //                        _DL_tss_keyaccount = new DL_tss_keyaccount();
        //                        _DL_tss_keyaccount.tss_status = STATUS_ERROR;
        //                        _DL_tss_keyaccount.UpdateStatus(organizationService, currentUIOGroup.GetAttributeValue<EntityReference>("tss_keyaccountid").Id);

        //                        _DL_tss_kagroupuiocommodity = new DL_tss_kagroupuiocommodity();
        //                        _DL_tss_kagroupuiocommodity.tss_calculatestatus = false;
        //                        _DL_tss_kagroupuiocommodity.UpdateStatus(organizationService, currentUIOGroup.GetAttributeValue<Guid>("tss_kagroupuiocommodityid"));
        //                        #endregion
        //                    }
        //                }
        //            }
        //        }
        //        #endregion
        //        #endregion

        //        Guid[] customerID = listKeyAccount.Entities.GroupBy(x => x.GetAttributeValue<EntityReference>("tss_customer").Id).Select(x => x.First().GetAttributeValue<EntityReference>("tss_customer").Id).ToArray();

        //        BL_tss_marketsizeresultpss _BL_tss_marketsizeresultpss = new BL_tss_marketsizeresultpss();
        //        _BL_tss_marketsizeresultpss.GenerateMarketSizeResultPSS_New(organizationService, context, customerID);
        //    }
        //    else
        //    {
        //        throw new InvalidWorkflowException("KAUIO or KA Group UIO Commodity not exist.");
        //    }
        //}

        //public void GenerateMasterMSSubLines(IOrganizationService organizationService, Entity population, Entity setup, DL_tss_mastermarketsizelines msLines, bool useTyre, int firstHMPM, int lastHMPM, DL_tss_mastermarketsize ms)
        //{
        //    FilterExpression fPart = new FilterExpression(LogicalOperator.And);

        //    QueryExpression qPart = new QueryExpression();

        //    if (msLines.tss_methodcalculationused == MTD1 || msLines.tss_methodcalculationused == MTD2 || msLines.tss_methodcalculationused == MTD3)
        //    {
        //        fPart.AddCondition("tss_model", ConditionOperator.Equal, population.GetAttributeValue<EntityReference>("trs_productmaster").Id);
        //        fPart.AddCondition("tss_typepm", ConditionOperator.Equal, msLines.tss_pm);

        //        qPart = new QueryExpression(_DL_tss_marketsizepartconsump.EntityName);
        //        qPart.LinkEntities.Add(new LinkEntity(_DL_tss_marketsizepartconsump.EntityName, _DL_trs_masterpart.EntityName, "tss_partnumber", "trs_masterpartid", JoinOperator.Inner));
        //        qPart.LinkEntities.Add(new LinkEntity(_DL_tss_marketsizepartconsump.EntityName, _DL_tss_partmasterlinesmodel.EntityName, "tss_partnumber", "tss_partmasterid", JoinOperator.Inner));
        //    }
        //    else if(msLines.tss_methodcalculationused == MTD4 || msLines.tss_methodcalculationused == MTD5)
        //    {
        //        DateTime deliveryDate = population.GetAttributeValue<DateTime>("new_deliverydate").ToLocalTime();
        //        DateTime currentGenerate = DateTime.Now;
        //        DateTime msStarPeriod = ms.tss_msperiodstart.ToLocalTime();
        //        DateTime msEndPeriod = ms.tss_msperiodend.ToLocalTime();

        //        CalculateDate calDate = new CalculateDate();
        //        int aging = 0;
        //        int agingMsPeriod = calDate.DiffYear(msStarPeriod, msEndPeriod);
        //        int agingDeliveryDate = calDate.DiffYear(deliveryDate, currentGenerate);
        //        aging = agingMsPeriod + agingDeliveryDate;

        //        fPart.AddCondition("tss_model", ConditionOperator.Equal, population.GetAttributeValue<EntityReference>("trs_productmaster").Id);

        //        FilterExpression fRange = new FilterExpression(LogicalOperator.And);
        //        fRange.AddCondition("tss_rangeagingfrom", ConditionOperator.LessEqual, aging);
        //        fRange.AddCondition("tss_rangeagingto", ConditionOperator.GreaterThan, aging);

        //        FilterExpression fAging = new FilterExpression(LogicalOperator.Or);
        //        fAging.AddFilter(fRange);
        //        fAging.AddCondition("tss_aging", ConditionOperator.Equal, aging);

        //        fPart.AddFilter(fAging);

        //        qPart = new QueryExpression("tss_marketsizepartconsumpaging");
        //        qPart.LinkEntities.Add(new LinkEntity("tss_marketsizepartconsumpaging", _DL_trs_masterpart.EntityName, "tss_partnumber", "trs_masterpartid", JoinOperator.Inner));
        //        qPart.LinkEntities.Add(new LinkEntity("tss_marketsizepartconsumpaging", _DL_tss_partmasterlinesmodel.EntityName, "tss_partnumber", "tss_partmasterid", JoinOperator.Inner));
        //    }

        //    qPart.LinkEntities[0].Columns.AddColumns("tss_commoditytype");
        //    qPart.LinkEntities[0].EntityAlias = "trs_masterpart";
        //    qPart.LinkEntities[1].Columns.AddColumn("tss_model");
        //    qPart.LinkEntities[1].LinkCriteria.AddCondition("tss_model", ConditionOperator.Equal, population.GetAttributeValue<EntityReference>("trs_productmaster").Id);
        //    qPart.LinkEntities[1].EntityAlias = "tss_partmasterlinesmodel";
        //    qPart.Criteria.AddFilter(fPart);
        //    qPart.ColumnSet = new ColumnSet(true);

        //    EntityCollection parts = organizationService.RetrieveMultiple(qPart);

        //    if (parts.Entities.Count > 0)
        //    {
        //        foreach (var part in parts.Entities)
        //        {
        //            int qty = part.GetAttributeValue<int>("tss_qty");
        //            decimal pr = 0m;
        //            decimal minpr = 0m;

        //            if (msLines.tss_methodcalculationused == MTD1 || msLines.tss_methodcalculationused == MTD2 || msLines.tss_methodcalculationused == MTD3)
        //            {
        //                if (part.Contains("trs_masterpart.tss_commoditytype") &&
        //                ((OptionSetValue)part.GetAttributeValue<AliasedValue>("trs_masterpart.tss_commoditytype").Value).Value == BATTERY_TYPE)
        //                {
        //                    QueryExpression qBatt = new QueryExpression(_DL_tss_partmasterlinesbattery.EntityName);
        //                    qBatt.LinkEntities.Add(new LinkEntity(_DL_tss_partmasterlinesbattery.EntityName, _DL_tss_partmasterlinesbatterytypeconsump.EntityName, "tss_partmasterlinesbatteryid", "tss_refpartmasterlinesbattery", JoinOperator.Inner));
        //                    qBatt.LinkEntities[0].Columns.AddColumns("tss_nextconsump", "tss_partmasterlinesbatterytypeconsumpid");
        //                    qBatt.LinkEntities[0].EntityAlias = "tss_partmasterlinesbatterytypeconsump";
        //                    qBatt.Criteria.AddCondition("tss_partmasterid", ConditionOperator.Equal, part.GetAttributeValue<EntityReference>("tss_partnumber").Id);
        //                    qBatt.ColumnSet = new ColumnSet(true);
        //                    EntityCollection batt = _DL_tss_partmasterlinesbattery.Select(organizationService, qBatt);

        //                    if (batt.Entities.Count > 0)
        //                    {
        //                        if (setup.Contains("tss_startdatems") && setup.Contains("tss_enddatems") && batt[0].Contains("tss_partmasterlinesbatterytypeconsump.tss_nextconsump") &&
        //                           (DateTime)batt[0].GetAttributeValue<AliasedValue>("tss_partmasterlinesbatterytypeconsump.tss_nextconsump").Value >= setup.GetAttributeValue<DateTime>("tss_startdatems") &&
        //                           (DateTime)batt[0].GetAttributeValue<AliasedValue>("tss_partmasterlinesbatterytypeconsump.tss_nextconsump").Value <= setup.GetAttributeValue<DateTime>("tss_enddatems"))
        //                        {
        //                            _DL_tss_partmasterlinesbatterytypeconsump = new DL_tss_partmasterlinesbatterytypeconsump();
        //                            _DL_tss_partmasterlinesbatterytypeconsump.tss_lastconsump = msLines.tss_estimatedpmdate;
        //                            _DL_tss_partmasterlinesbatterytypeconsump.tss_nextconsump = _DL_tss_partmasterlinesbatterytypeconsump.tss_lastconsump.AddDays(batt[0].GetAttributeValue<int>("tss_cycleconsump"));
        //                            _DL_tss_partmasterlinesbatterytypeconsump.Update(organizationService, (Guid)batt[0].GetAttributeValue<AliasedValue>("tss_partmasterlinesbatterytypeconsump.tss_partmasterlinesbatterytypeconsumpid").Value);
        //                        }
        //                        else
        //                        {
        //                            continue;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        continue;
        //                    }
        //                }

        //                if (part.Contains("tss_tyrecode") && useTyre)
        //                {
        //                    FilterExpression fFunc = new FilterExpression(LogicalOperator.Or);
        //                    fFunc.AddCondition("tss_tyrefrontcode", ConditionOperator.Equal, part.GetAttributeValue<EntityReference>("tss_tyrecode").Id);
        //                    fFunc.AddCondition("tss_tyrerearcode", ConditionOperator.Equal, part.GetAttributeValue<EntityReference>("tss_tyrecode").Id);

        //                    QueryExpression qFunc = new QueryExpression("trs_functionallocation");
        //                    qFunc.Criteria.AddCondition("trs_functionallocationid", ConditionOperator.Equal, population.GetAttributeValue<EntityReference>("trs_functionallocation").Id);
        //                    qFunc.Criteria.AddFilter(fFunc);
        //                    qFunc.ColumnSet.AddColumns("tss_tyrefrontcode", "tss_tyrerearcode");
        //                    EntityCollection funcs = organizationService.RetrieveMultiple(qFunc);

        //                    if (funcs.Entities.Count > 0)
        //                    {
        //                        Guid code = funcs.Entities[0].Contains("tss_tyrefrontcode") && funcs.Entities[0].GetAttributeValue<EntityReference>("tss_tyrefrontcode").Id == part.GetAttributeValue<EntityReference>("tss_tyrecode").Id ?
        //                                    funcs.Entities[0].GetAttributeValue<EntityReference>("tss_tyrefrontcode").Id :
        //                                    funcs.Entities[0].Contains("tss_tyrerearcode") && funcs.Entities[0].GetAttributeValue<EntityReference>("tss_tyrerearcode").Id == part.GetAttributeValue<EntityReference>("tss_tyrecode").Id ?
        //                                    funcs.Entities[0].GetAttributeValue<EntityReference>("tss_tyrerearcode").Id : Guid.Empty;
        //                        if (code != Guid.Empty)
        //                        {
        //                            QueryExpression qTyre = new QueryExpression(_DL_tss_matrixtyreconsump.EntityName);
        //                            qTyre.Criteria.AddCondition("tss_matrixtyreconsumpid", ConditionOperator.Equal, code);
        //                            qTyre.ColumnSet.AddColumn("tss_hmplanning");
        //                            EntityCollection tyres = _DL_tss_matrixtyreconsump.Select(organizationService, qTyre);
        //                            if (tyres.Entities.Count > 0)
        //                            {
        //                                qty = (int)Math.Round(((decimal)lastHMPM - (decimal)firstHMPM) / (decimal)tyres.Entities[0].GetAttributeValue<int>("tss_hmplanning"), 0);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            continue;
        //                        }
        //                    }
        //                }
        //            }

        //            QueryExpression qPrice = new QueryExpression(_DL_tss_sparepartpricemaster.EntityName);
        //            qPrice.LinkEntities.Add(new LinkEntity(_DL_tss_sparepartpricemaster.EntityName, _DL_tss_pricelistpart.EntityName, "tss_pricelistpart", "tss_pricelistpartid", JoinOperator.Inner));
        //            qPrice.Criteria.AddCondition("tss_partmaster", ConditionOperator.Equal, part.GetAttributeValue<EntityReference>("tss_partnumber").Id);
        //            qPrice.LinkEntities[0].LinkCriteria.AddCondition("tss_type", ConditionOperator.Equal, MS_TYPE);
        //            qPrice.LinkEntities[0].EntityAlias = "tss_pricelistpart";
        //            qPrice.ColumnSet = new ColumnSet(true);
        //            EntityCollection price = _DL_tss_sparepartpricemaster.Select(organizationService, qPrice);

        //            if (price.Entities.Count > 0)
        //            {
        //                pr = price.Entities[0].Contains("tss_price") ? price.Entities[0].GetAttributeValue<Money>("tss_price").Value : 0m;
        //                minpr = price.Entities[0].Contains("tss_minimumprice") ? price.Entities[0].GetAttributeValue<Money>("tss_minimumprice").Value : 0m;
        //            }

        //            _DL_tss_mastermarketsizesublines = new DL_tss_mastermarketsizesublines();
        //            _DL_tss_mastermarketsizesublines.tss_mastermslinesref = msLines.tss_mastermarketsizelinesid;
        //            _DL_tss_mastermarketsizesublines.tss_partnumber = part.GetAttributeValue<EntityReference>("tss_partnumber").Id;
        //            _DL_tss_mastermarketsizesublines.tss_qty = qty;
        //            _DL_tss_mastermarketsizesublines.tss_originalqty = qty;
        //            _DL_tss_mastermarketsizesublines.tss_price = pr;
        //            _DL_tss_mastermarketsizesublines.tss_minimumprice = minpr;

        //            Entity oPartNumber = organizationService.Retrieve("trs_masterpart", part.GetAttributeValue<EntityReference>("tss_partnumber").Id, new ColumnSet(true));
        //            _DL_tss_mastermarketsizesublines.tss_partdescription = oPartNumber.GetAttributeValue<String>("trs_partdescription");

        //            _DL_tss_mastermarketsizesublines.Insert(organizationService);
        //        }
        //    }

        //}

        #endregion

    }
}
