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

using Microsoft.Crm.Sdk.Messages;

namespace TrakNusSparepartSystem.WorkflowActivity.BusinessLayer
{
    public class BL_tss_prospectpart
    {
        #region Constant
        private const int STATUS_OPEN = 865920000;
        private const int STATUS_ERROR = 865920002;
        private const int UNITTYPE_UIO = 865920000;
        private const int UNITTYPE_NONUIO = 865920001;
        private const int UNITTYPE_COMMODITY = 865920002;
        private const int STATUS_COMPLETED_MS = 865920000;
        private const int STATUS_ERROR_MS = 865920001;
        private const int STATUS_CLOSED_MS = 865920002;
        private const int STATUSREASON_COMPLETED_MS = 865920000;
        private const int STATUSREASON_ERROR_MS = 865920001;
        #endregion

        #region Depedencies

        private DL_tss_potentialprospectpart _DL_tss_potentialprospectpart = new DL_tss_potentialprospectpart();
        private DL_tss_potentialprospectpartlines _DL_tss_potentialprospectpartlines = new DL_tss_potentialprospectpartlines();
        private DL_tss_potentialprospectpartsublines _DL_tss_potentialprospectpartsublines = new DL_tss_potentialprospectpartsublines();
        private DL_tss_prospectpartheader _DL_tss_prospectpartheader = new DL_tss_prospectpartheader();
        private DL_tss_prospectpartlines _DL_tss_prospectpartlines = new DL_tss_prospectpartlines();

        private DL_tss_mastermarketsize _DL_tss_marketmarketsize = new DL_tss_mastermarketsize();
        private DL_tss_mastermarketsizelines _DL_tss_marketmarketsizelines = new DL_tss_mastermarketsizelines();
        private DL_tss_mastermarketsizesublines _DL_tss_marketmarketsizesublines = new DL_tss_mastermarketsizesublines();
        private DL_tss_totalpartconsumpmarketsize _DL_tss_totalpartconsumpmarketsize = new DL_tss_totalpartconsumpmarketsize();
        private DL_tss_pricelistpart _DL_tss_pricelistpart = new DL_tss_pricelistpart();
        private DL_partMaster _DL_partMaster = new DL_partMaster();
        private DL_account _DL_account = new DL_account();
        private DL_businessunit _DL_businessunit = new DL_businessunit();
        private DL_tss_sparepartpricemaster _DL_tss_sparepartpricemaster = new DL_tss_sparepartpricemaster();
        private DL_population _DL_population = new DL_population();

        #endregion

        public List<object> GetSparePartPriceMaster(IOrganizationService organizationService, Guid _partnumberid, bool _isdirectsales)
        {
            List<object> oResult = new List<object>();
            //decimal oResult = 0;

            #region GET PRICE LIST ID
            FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);

            if (_isdirectsales)
                _filterexpression.AddCondition("tss_type", ConditionOperator.Equal, 865920000);
            else
                _filterexpression.AddCondition("tss_type", ConditionOperator.Equal, 865920001);

            QueryExpression _queryexpression = new QueryExpression("tss_pricelistpart");
            _queryexpression.Criteria.AddFilter(_filterexpression);
            _queryexpression.ColumnSet = new ColumnSet(true);

            EntityCollection _pricelistpartcollection = _DL_tss_pricelistpart.Select(organizationService, _queryexpression);
            #endregion

            if (_pricelistpartcollection.Entities.Count() > 0)
            {
                Guid _pricelistpartid = _pricelistpartcollection.Entities[0].Id;

                #region GET SPARE PART PRICE MASTER
                FilterExpression fSparePartPriceMaster = new FilterExpression(LogicalOperator.And);
                fSparePartPriceMaster.AddCondition("tss_partmaster", ConditionOperator.Equal, _partnumberid);
                fSparePartPriceMaster.AddCondition("tss_pricelistpart", ConditionOperator.Equal, _pricelistpartid);

                QueryExpression qSparePartPriceMaster = new QueryExpression("tss_sparepartpricemaster");
                qSparePartPriceMaster.Criteria.AddFilter(fSparePartPriceMaster);
                qSparePartPriceMaster.ColumnSet = new ColumnSet(true);

                EntityCollection _sparepartpricemastercollection = _DL_tss_sparepartpricemaster.Select(organizationService, qSparePartPriceMaster);
                #endregion

                if (_sparepartpricemastercollection.Entities.Count() > 0)
                {
                    //PRICE & MINIMUM PRICE
                    oResult.Add((bool)true);
                    oResult.Add((decimal)_sparepartpricemastercollection.Entities[0].GetAttributeValue<Money>("tss_price").Value);
                    oResult.Add((decimal)_sparepartpricemastercollection.Entities[0].GetAttributeValue<Money>("tss_minimumprice").Value);
                    oResult.Add((Guid)_sparepartpricemastercollection.Entities[0].GetAttributeValue<EntityReference>("transactioncurrencyid").Id);
                }
                else
                {
                    oResult.Add((bool)false);
                    oResult.Add((decimal)0);
                    oResult.Add((decimal)0);
                    oResult.Add(new Guid("7459618D-4EE9-E111-9AA2-544249894792"));
                }
            }
            else
            {
                oResult.Add((bool)false);
                oResult.Add((decimal)0);
                oResult.Add((decimal)0);
                oResult.Add(new Guid("7459618D-4EE9-E111-9AA2-544249894792"));
            }

            return oResult;
        }

        public void SetProspectPart(IOrganizationService organizationService, Entity _potentialprospectpart, EntityCollection _potentialprospectpartlinescollection, Guid _prospectpartheaderid, bool _isdirectsales)
        {
            #region GENERATE PROSPECT PART LINE
            EntityCollection _potentialprospectpartsublinescollection = new EntityCollection();

            if (_potentialprospectpartlinescollection.Entities.Count == 0)
            {
                FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
                _filterexpression.AddCondition("tss_potentialprospectpartref", ConditionOperator.Equal, _potentialprospectpart.Id);

                QueryExpression _queryexpression = new QueryExpression("tss_potentialprospectpartsublines");
                _queryexpression.Criteria.AddFilter(_filterexpression);
                _queryexpression.ColumnSet = new ColumnSet(true);

                _potentialprospectpartsublinescollection = organizationService.RetrieveMultiple(_queryexpression);
            }
            else
            {
                string[] _potentialprospectpartlinesids = _potentialprospectpartlinescollection.Entities.Select(x => x.GetAttributeValue<Guid>("tss_potentialprospectpartlinesid").ToString()).ToArray();

                FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
                _filterexpression.AddCondition("tss_potentialprospectpartlinesref", ConditionOperator.In, _potentialprospectpartlinesids);

                QueryExpression _queryexpression = new QueryExpression("tss_potentialprospectpartsublines");
                _queryexpression.Criteria.AddFilter(_filterexpression);
                _queryexpression.ColumnSet = new ColumnSet(true);

                _potentialprospectpartsublinescollection = _DL_tss_potentialprospectpartsublines.Select(organizationService, _queryexpression);
            }

            GenerateProspectSubLines(organizationService, _potentialprospectpart, _potentialprospectpartsublinescollection, _prospectpartheaderid, _isdirectsales);
            #endregion
        }

        public void GenerateProspectSubLines(IOrganizationService organizationService, Entity _potentialprospectpart, EntityCollection _potentialprospectpartsublinescollection, Guid _prospectpartheaderid, bool _isdirectsales)
        {
            FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
            QueryExpression _queryexpression = new QueryExpression();

            int _linenumber = 0;
            var _potentialprospectpartsublinesgroup = (from r in _potentialprospectpartsublinescollection.Entities.AsEnumerable()
                                                       group r by new
                                                       {
                                                           partnumberId = (r.GetAttributeValue<EntityReference>("tss_partnumber").Id)
                                                       } into g
                                                       select new
                                                       {
                                                           qty = g.Sum(x => x.GetAttributeValue<int>("tss_qty")),
                                                           remainingqty = g.Sum(x => x.GetAttributeValue<int>("tss_remainingqty")),
                                                           parnumberId = g.Key.partnumberId
                                                       }).ToList();

            foreach (var _subline in _potentialprospectpartsublinesgroup)
            {
                _linenumber += 10;

                Entity _prospectpartlines = new Entity("tss_prospectpartlines");

                _prospectpartlines["tss_itemnumber"] = _linenumber;
                _prospectpartlines["tss_prospectpartheader"] = new EntityReference("tss_prospectpartheader", _prospectpartheaderid);
                _prospectpartlines["tss_partnumber"] = new EntityReference("trs_masterpart", _subline.parnumberId);

                Entity _masterpart = organizationService.Retrieve("trs_masterpart", _subline.parnumberId, new ColumnSet(true));
                _prospectpartlines["tss_partdescription"] = _masterpart.GetAttributeValue<String>("trs_partdescription");

                #region GET PRICE
                List<object> _pricemaster = GetSparePartPriceMaster(organizationService, _subline.parnumberId, _isdirectsales);
                bool _pricemasterexist = (bool)_pricemaster[0];
                decimal _price = (decimal)_pricemaster[1];
                decimal _minimumprice = (decimal)_pricemaster[2];
                Guid _transactioncurrencyid = (Guid)_pricemaster[3];

                if (_pricemasterexist)
                {
                    _prospectpartlines["tss_price"] = new Money(_price);
                    _prospectpartlines["tss_minimumprice"] = new Money(_minimumprice);
                    _prospectpartlines["transactioncurrencyid"] = new EntityReference("transactioncurrency", _transactioncurrencyid);
                }
                else
                {
                    _prospectpartlines["tss_price"] = new Money(0);
                    _prospectpartlines["tss_minimumprice"] = new Money(0);
                    _prospectpartlines["transactioncurrencyid"] = new EntityReference("transactioncurrency", new Guid("7459618D-4EE9-E111-9AA2-544249894792"));
                }

                _prospectpartlines["tss_discountamount"] = new Money(0);
                _prospectpartlines["tss_discountpercent"] = Convert.ToDecimal(0);
                _prospectpartlines["tss_finalprice"] = new Money(0);
                #endregion


                #region FIND UNIT GROUP
                if (_potentialprospectpart.Attributes.Contains("tss_serialnumber"))
                {
                    Entity _population = organizationService.Retrieve("new_population", _potentialprospectpart.GetAttributeValue<EntityReference>("tss_serialnumber").Id, new ColumnSet(true));

                    if (_population != null)
                    {
                        _filterexpression = new FilterExpression(LogicalOperator.And);
                        _filterexpression.AddCondition("name", ConditionOperator.Equal, _population.GetAttributeValue<string>("new_productitem"));

                        _queryexpression = new QueryExpression("uomschedule");
                        _queryexpression.Criteria.AddFilter(_filterexpression);
                        _queryexpression.ColumnSet = new ColumnSet(true);

                        EntityCollection _uomschedule = organizationService.RetrieveMultiple(_queryexpression);
                        _prospectpartlines["tss_unitgroup"] = new EntityReference("uomschedule", _uomschedule.Entities[0].Id);
                    }
                }
                #endregion


                #region GET PART TYPE
                _filterexpression = new FilterExpression(LogicalOperator.And);
                _filterexpression.AddCondition("trs_masterpartid", ConditionOperator.Equal, _subline.parnumberId);

                _queryexpression = new QueryExpression("trs_masterpart");
                _queryexpression.Criteria.AddFilter(_filterexpression);
                _queryexpression.ColumnSet = new ColumnSet(true);

                EntityCollection _masterpartcollection = _DL_partMaster.Select(organizationService, _queryexpression);

                if (_masterpartcollection.Entities.Count() > 0)
                {
                    if (_masterpartcollection.Entities[0].Attributes.Contains("tss_parttype"))
                        _prospectpartlines["tss_parttype"] = new OptionSetValue(_masterpartcollection.Entities[0].GetAttributeValue<OptionSetValue>("tss_parttype").Value);
                }
                #endregion


                #region LOOK UP TO MARKET SIZE
                _filterexpression = new FilterExpression(LogicalOperator.And);

                _filterexpression.AddCondition("tss_pss", ConditionOperator.Equal, _potentialprospectpart.GetAttributeValue<EntityReference>("tss_pss").Id);
                _filterexpression.AddCondition("tss_customer", ConditionOperator.Equal, _potentialprospectpart.GetAttributeValue<EntityReference>("tss_customer").Id);

                if (_potentialprospectpart.Attributes.Contains("tss_serialnumber"))
                    _filterexpression.AddCondition("tss_serialnumber", ConditionOperator.Equal, _potentialprospectpart.GetAttributeValue<EntityReference>("tss_serialnumber").Id);
                else
                    _filterexpression.AddCondition("tss_groupuiocommodityheader", ConditionOperator.Equal, _potentialprospectpart.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id);

                _queryexpression = new QueryExpression("tss_mastermarketsize");
                _queryexpression.Criteria.AddFilter(_filterexpression);
                _queryexpression.ColumnSet = new ColumnSet(true);

                EntityCollection _marketmarketsizecollection = _DL_tss_marketmarketsize.Select(organizationService, _queryexpression);
                #endregion


                #region LOOK UP TO MARKET SIZE SUBLINES
                _filterexpression = new FilterExpression(LogicalOperator.And);
                _filterexpression.AddCondition("tss_mastermarketsizeid", ConditionOperator.Equal, _marketmarketsizecollection.Entities[0].Id);

                _queryexpression = new QueryExpression("tss_mastermarketsizesublines");
                _queryexpression.Criteria.AddFilter(_filterexpression);
                _queryexpression.ColumnSet = new ColumnSet(true);

                EntityCollection _marketmarketsizesublinescollection = _DL_tss_marketmarketsizesublines.Select(organizationService, _queryexpression);
                #endregion


                #region LOOK UP TO PRICE LIST PART
                _filterexpression = new FilterExpression(LogicalOperator.And);

                if (_isdirectsales)
                    _filterexpression.AddCondition("tss_type", ConditionOperator.Equal, 865920000);
                else
                    _filterexpression.AddCondition("tss_type", ConditionOperator.Equal, 865920001);

                _queryexpression = new QueryExpression("tss_pricelistpart");
                _queryexpression.Criteria.AddFilter(_filterexpression);
                _queryexpression.ColumnSet = new ColumnSet(true);

                EntityCollection _pricelistpartcollection = _DL_tss_pricelistpart.Select(organizationService, _queryexpression);
                #endregion

                if (_pricelistpartcollection.Entities.Count() > 0)
                    _prospectpartlines["tss_pricetype"] = new EntityReference("tss_pricelistpart", _pricelistpartcollection.Entities[0].Id);
                else
                    throw new Exception("Price List Part not found.");

                _prospectpartlines["tss_refmarketsize"] = new EntityReference(_DL_tss_potentialprospectpart.EntityName, _potentialprospectpart.Id);

                int remainingqty = 0;

                if (_subline.remainingqty == 0)
                {
                    if (_isdirectsales)
                    {
                        _prospectpartlines["tss_sourcetype"] = new OptionSetValue(865920001);
                        _prospectpartlines["tss_quantity"] = Convert.ToDecimal(_subline.qty);
                        _prospectpartlines["tss_quantityconvert"] = Convert.ToDecimal(_subline.qty);
                        _prospectpartlines["tss_qtymarketsize"] = Convert.ToDecimal(_subline.qty);
                        _prospectpartlines["tss_priceamount"] = new Money(_price * Convert.ToDecimal(_subline.qty));

                        organizationService.Create(_prospectpartlines);
                    }
                }
                else if (_subline.remainingqty >= _subline.qty)
                {
                    remainingqty = _subline.remainingqty - _subline.qty;

                    _prospectpartlines["tss_sourcetype"] = new OptionSetValue(865920001);
                    _prospectpartlines["tss_quantity"] = Convert.ToDecimal(_subline.qty);
                    _prospectpartlines["tss_quantityconvert"] = Convert.ToDecimal(_subline.qty);
                    _prospectpartlines["tss_qtymarketsize"] = Convert.ToDecimal(_subline.qty);
                    _prospectpartlines["tss_priceamount"] = new Money(_price * Convert.ToDecimal(_subline.qty));

                    organizationService.Create(_prospectpartlines);
                }
                else if (_subline.remainingqty < _subline.qty)
                {
                    remainingqty = 0;

                    _prospectpartlines["tss_sourcetype"] = new OptionSetValue(865920001);
                    _prospectpartlines["tss_quantity"] = Convert.ToDecimal(_subline.remainingqty);
                    _prospectpartlines["tss_quantityconvert"] = Convert.ToDecimal(_subline.remainingqty);
                    _prospectpartlines["tss_qtymarketsize"] = Convert.ToDecimal(_subline.remainingqty);
                    _prospectpartlines["tss_priceamount"] = new Money(_price * Convert.ToDecimal(_subline.remainingqty));

                    organizationService.Create(_prospectpartlines);
                }

                //if (!_isdirectsales)
                //{
                //    #region LOOK UP TO TOTAL PART CONSUMP MARKET SIZE
                //    _filterexpression = new FilterExpression(LogicalOperator.And);
                //    _filterexpression.AddCondition("tss_marketsizeid", ConditionOperator.Equal, _potentialprospectpart.GetAttributeValue<EntityReference>("tss_marketsizeid").Id);
                //    _filterexpression.AddCondition("tss_pss", ConditionOperator.Equal, _potentialprospectpart.GetAttributeValue<EntityReference>("tss_pss").Id);
                //    _filterexpression.AddCondition("tss_partnumber", ConditionOperator.Equal, _subline.parnumberId);

                //    _queryexpression = new QueryExpression("tss_totalpartconsumpmarketsize");
                //    _queryexpression.Criteria.AddFilter(_filterexpression);
                //    _queryexpression.ColumnSet = new ColumnSet(true);

                //    EntityCollection _totalpartconsumpmarketsizecollection = _DL_tss_totalpartconsumpmarketsize.Select(organizationService, _queryexpression);

                //    if (_totalpartconsumpmarketsizecollection.Entities.Count() > 0)
                //    {
                //        Entity _totalpartconsumpmarketsize = _totalpartconsumpmarketsizecollection.Entities[0];
                //        int _qtyactualtotalpartconsumpmarketsize = _totalpartconsumpmarketsize.GetAttributeValue<int>("tss_remainingqty");

                //        _totalpartconsumpmarketsize["tss_remainingqty"] = _qtyactualtotalpartconsumpmarketsize - _subline.remainingqty;

                //        organizationService.Update(_totalpartconsumpmarketsize);
                //    }
                //    #endregion
                //}
            }

            //if (!_isdirectsales)
            //{
            //    // UPDATE
            //    foreach (var _potentialprospectpartsublinesitem in _potentialprospectpartsublinescollection.Entities)
            //    {
            //        _potentialprospectpartsublinesitem["tss_remainingqty"] = 0;

            //        organizationService.Update(_potentialprospectpartsublinesitem);
            //    }
            //}
        }

        public void GenerateMasterProspectPartHeader_OnClick(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context, string recordID)
        {
            try
            {
                if (String.IsNullOrEmpty(recordID))
                    throw new InvalidWorkflowException("Unable to find Potential Prospect Part. Please make sure to save and then retry the action.");
                else
                {
                    Entity _prospectpartheader = new Entity("tss_prospectpartheader");

                    #region GET CURRENT POTENTIAL PROSPECT PART
                    Entity _potentialprospectpart = organizationService.Retrieve("tss_potentialprospectpart", context.PrimaryEntityId, new ColumnSet(true));
                    Entity _customer = organizationService.Retrieve("account", _potentialprospectpart.GetAttributeValue<EntityReference>("tss_customer").Id, new ColumnSet(true));
                    #endregion

                    #region GET CURRENT POTENTIAL PROSPECT PART LINES
                    FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
                    _filterexpression.AddCondition("tss_potentialprospectpart", ConditionOperator.Equal, _potentialprospectpart.Id);

                    QueryExpression _queryexpression = new QueryExpression("tss_potentialprospectpartlines");
                    _queryexpression.Criteria.AddFilter(_filterexpression);
                    _queryexpression.ColumnSet = new ColumnSet(true);

                    EntityCollection _potentialprospectpartlinescollection = organizationService.RetrieveMultiple(_queryexpression);
                    #endregion

                    #region SET TOTAL AMOUNT
                    int _totalqty = 0;
                    int _remainingqty = 0;
                    decimal _totalprice = 0;
                    decimal _totalamount = 0;
                    string _title = "";
                    bool _isdirectsales = false;

                    if (_potentialprospectpartlinescollection.Entities.Count == 0)
                    {
                        _filterexpression = new FilterExpression(LogicalOperator.And);
                        _filterexpression.AddCondition("tss_potentialprospectpartref", ConditionOperator.Equal, _potentialprospectpart.Id);

                        _queryexpression = new QueryExpression("tss_potentialprospectpartsublines");
                        _queryexpression.Criteria.AddFilter(_filterexpression);
                        _queryexpression.ColumnSet = new ColumnSet(true);

                        EntityCollection _potentialprospectpartsublinescollection = organizationService.RetrieveMultiple(_queryexpression);

                        foreach (Entity _potentialprospectpartsublinesitem in _potentialprospectpartsublinescollection.Entities)
                        {
                            _totalqty += Convert.ToInt32(_potentialprospectpartsublinesitem["tss_qty"]);
                            _remainingqty += Convert.ToInt32(_potentialprospectpartsublinesitem["tss_remainingqty"]);
                            _totalprice += Convert.ToDecimal(_potentialprospectpartsublinesitem.GetAttributeValue<Money>("tss_price").Value);
                        }

                        if (_potentialprospectpart.Attributes.Contains("tss_groupuiocommodityheader"))
                        {
                            Entity _groupuiocommodityheader = organizationService.Retrieve("tss_groupuiocommodityheader", _potentialprospectpart.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id, new ColumnSet(true));

                            if (_groupuiocommodityheader != null)
                                _title = _groupuiocommodityheader.GetAttributeValue<string>("tss_groupuiocommodityname");
                            else
                                _title = "[No Name]";
                        }
                        else
                            _title = "[No Name]";
                    }
                    else
                    {
                        foreach (Entity _line in _potentialprospectpartlinescollection.Entities)
                        {

                            _filterexpression = new FilterExpression(LogicalOperator.And);
                            _filterexpression.AddCondition("tss_potentialprospectpartlinesref", ConditionOperator.Equal, _line["tss_potentialprospectpartlinesid"].ToString());

                            _queryexpression = new QueryExpression("tss_potentialprospectpartsublines");
                            _queryexpression.Criteria.AddFilter(_filterexpression);
                            _queryexpression.ColumnSet = new ColumnSet(true);

                            EntityCollection _potentialprospectpartsublinescollection = organizationService.RetrieveMultiple(_queryexpression);

                            foreach (Entity _potentialprospectpartsublinesitem in _potentialprospectpartsublinescollection.Entities)
                            {
                                _totalqty += Convert.ToInt32(_potentialprospectpartsublinesitem["tss_qty"]);
                                _remainingqty += Convert.ToInt32(_potentialprospectpartsublinesitem["tss_remainingqty"]);
                                _totalprice += Convert.ToDecimal(_potentialprospectpartsublinesitem.GetAttributeValue<Money>("tss_price").Value);
                            }
                        }

                        _title = _potentialprospectpart.GetAttributeValue<string>("tss_serialnumberpopulation");
                    }

                    if (_remainingqty == 0)
                    {
                        //throw new InvalidPluginExecutionException("Cannot Generate Prospect Part. No remaining quantity.");
                        _prospectpartheader["tss_sourcetype"] = new OptionSetValue(865920000);
                        _isdirectsales = true;
                    }
                    else
                    {
                        _prospectpartheader["tss_sourcetype"] = new OptionSetValue(865920001);
                    }

                    _totalamount = _totalqty * _totalprice;
                    #endregion

                    #region GET LATEST EST.CLOSE DATE
                    DateTime _newdatetime = new DateTime(1900, 1, 1);

                    foreach (Entity _potentialprospectpartlinesitem in _potentialprospectpartlinescollection.Entities)
                    {
                        DateTime _estclosedate = _potentialprospectpartlinesitem.GetAttributeValue<DateTime>("tss_estclosedate");

                        if (_estclosedate.Date != DateTime.MinValue.Date)
                        {
                            if (_estclosedate > _newdatetime)
                                _newdatetime = _estclosedate;
                        }
                    }
                    #endregion

                    #region GET BRANCH CODE
                    string _branchcode = string.Empty;
                    Entity _businessunit = _DL_businessunit.Select(organizationService, context.BusinessUnitId);

                    if (_businessunit.Attributes.Contains("trs_branchcode"))
                        _branchcode = _businessunit.Attributes["trs_branchcode"].ToString();
                    #endregion

                    #region GENERATE PROSPECT PART HEADER
                    _prospectpartheader["tss_estimationclosedate"] = _newdatetime;
                    _prospectpartheader["tss_pipelinephase"] = new OptionSetValue(865920000);
                    _prospectpartheader["tss_statusreason"] = new OptionSetValue(865920000);
                    _prospectpartheader["tss_customer"] = _potentialprospectpart.GetAttributeValue<EntityReference>("tss_customer");
                    _prospectpartheader["tss_refmarketsize"] = new EntityReference(_DL_tss_potentialprospectpart.EntityName, _potentialprospectpart.Id);
                    _prospectpartheader["tss_currency"] = new EntityReference("transactioncurrency", new Guid("7459618D-4EE9-E111-9AA2-544249894792"));
                    _prospectpartheader["ownerid"] = new EntityReference("systemuser", context.UserId);
                    _prospectpartheader["tss_pss"] = _potentialprospectpart.GetAttributeValue<EntityReference>("tss_pss");
                    _prospectpartheader["tss_branch"] = new EntityReference(_DL_businessunit.EntityName, context.BusinessUnitId);
                    _prospectpartheader["tss_totalamount"] = _totalamount;
                    _prospectpartheader["tss_topic"] = "Prospect From MS - " +
                        _title +
                        " - " +
                        DateTime.Now.ToLocalTime().Year.ToString() +
                        " by Unit";

                    if (_customer.Attributes.Contains("primarycontactid"))
                        _prospectpartheader["tss_contact"] = _customer.GetAttributeValue<EntityReference>("primarycontactid");

                    Guid _prospectpartheaderid = organizationService.Create(_prospectpartheader);
                    #endregion

                    SetProspectPart(organizationService, _potentialprospectpart, _potentialprospectpartlinescollection, _prospectpartheaderid, _isdirectsales);
                }
            }
            catch (Exception e)
            {
                throw new InvalidWorkflowException(e.Message);
            }
        }

        public void GenerateProspectPartFromPotentialProspectPartLine_OnClick(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {
            Entity _prospectpartheader = new Entity("tss_prospectpartheader");
            Entity _potentialprospectpartlines = _DL_tss_potentialprospectpartlines.Select(organizationService, context.PrimaryEntityId);
            Entity _potentialprospectpart = _DL_tss_potentialprospectpart.Select(organizationService, _potentialprospectpartlines.GetAttributeValue<EntityReference>("tss_potentialprospectpart").Id);
            Entity _customer = organizationService.Retrieve("account", _potentialprospectpart.GetAttributeValue<EntityReference>("tss_customer").Id, new ColumnSet(true));

            #region SET TOTAL AMOUNT
            int _totalqty = 0;
            int _remainingqty = 0;
            decimal _totalprice = 0;
            decimal _totalamount = 0;
            bool _isdirectsales = false;

            FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
            _filterexpression.AddCondition("tss_potentialprospectpartlinesref", ConditionOperator.Equal, _potentialprospectpartlines["tss_potentialprospectpartlinesid"].ToString());

            QueryExpression _queryexpression = new QueryExpression("tss_potentialprospectpartsublines");
            _queryexpression.Criteria.AddFilter(_filterexpression);
            _queryexpression.ColumnSet = new ColumnSet(true);

            EntityCollection _potentialprospectpartsublinescollection = organizationService.RetrieveMultiple(_queryexpression);

            foreach (Entity _potentialprospectpartsublinesitem in _potentialprospectpartsublinescollection.Entities)
            {
                _totalqty += Convert.ToInt32(_potentialprospectpartsublinesitem["tss_qty"]);
                _remainingqty += Convert.ToInt32(_potentialprospectpartsublinesitem["tss_remainingqty"]);
                _totalprice += Convert.ToDecimal(_potentialprospectpartsublinesitem.GetAttributeValue<Money>("tss_price").Value);
            }

            if (_remainingqty == 0)
            {
                //throw new InvalidPluginExecutionException("Cannot Generate Prospect Part. No remaining quantity.");
                _prospectpartheader["tss_sourcetype"] = new OptionSetValue(865920000);
                _isdirectsales = true;
            }
            else
            {
                _prospectpartheader["tss_sourcetype"] = new OptionSetValue(865920001);
            }

            _totalamount = _totalqty * _totalprice;
            #endregion

            #region GET LATEST EST.CLOSE DATE
            DateTime _newdatetime = new DateTime(1900, 1, 1);

            DateTime _estclosedate = _potentialprospectpartlines.GetAttributeValue<DateTime>("tss_estclosedate");

            if (_estclosedate.Date != DateTime.MinValue.Date)
            {
                if (_estclosedate > _newdatetime)
                    _newdatetime = _estclosedate;
            }
            #endregion

            #region GET BRANCH CODE
            string _branchcode = string.Empty;
            Entity _businessunit = _DL_businessunit.Select(organizationService, context.BusinessUnitId);

            if (_businessunit.Attributes.Contains("trs_branchcode"))
                _branchcode = _businessunit.Attributes["trs_branchcode"].ToString();
            #endregion

            #region GENERATE PROSPECT PART HEADER
            _prospectpartheader["tss_estimationclosedate"] = _newdatetime;
            _prospectpartheader["tss_pipelinephase"] = new OptionSetValue(865920000);
            _prospectpartheader["tss_statusreason"] = new OptionSetValue(865920000);
            _prospectpartheader["tss_customer"] = _potentialprospectpart.GetAttributeValue<EntityReference>("tss_customer");
            _prospectpartheader["tss_refmarketsize"] = new EntityReference(_DL_tss_potentialprospectpart.EntityName, _potentialprospectpart.Id);
            _prospectpartheader["tss_currency"] = new EntityReference("transactioncurrency", new Guid("7459618D-4EE9-E111-9AA2-544249894792"));
            _prospectpartheader["ownerid"] = new EntityReference("systemuser", context.UserId);
            _prospectpartheader["tss_pss"] = _potentialprospectpart.GetAttributeValue<EntityReference>("tss_pss");
            _prospectpartheader["tss_branch"] = new EntityReference(_DL_businessunit.EntityName, context.BusinessUnitId);
            _prospectpartheader["tss_totalamount"] = _totalamount;
            _prospectpartheader["tss_topic"] = "Prospect From MS - " +
                _potentialprospectpart.GetAttributeValue<string>("tss_serialnumberpopulation") +
                " - " +
                DateTime.Now.ToLocalTime().Year.ToString() +
                " by Unit";

            if (_customer.Attributes.Contains("primarycontactid"))
                _prospectpartheader["tss_contact"] = _customer.GetAttributeValue<EntityReference>("primarycontactid");

            Guid _prospectpartheaderid = organizationService.Create(_prospectpartheader);
            #endregion

            EntityCollection _potentialprospectpartlinescollection = new EntityCollection();

            _potentialprospectpartlinescollection.Entities.Add(_potentialprospectpartlines);

            SetProspectPart(organizationService, _potentialprospectpart, _potentialprospectpartlinescollection, _prospectpartheaderid, _isdirectsales);
        }

        public void GenerateProspectPartFromPotentialProspectPartSubLine_OnClick(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context, string recordID)
        {
            Entity _prospectpartheader = new Entity("tss_prospectpartheader");
            string[] _recordidcollection = recordID.Split(',');
            string _title = "";
            DateTime _newdatetime = new DateTime(1900, 1, 1);

            #region GET ALL SELECTED POTENTIAL PROSPECT PART SUBLINES
            FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
            _filterexpression.AddCondition("tss_potentialprospectpartsublinesid", ConditionOperator.In, _recordidcollection);

            QueryExpression _queryexpression = new QueryExpression("tss_potentialprospectpartsublines");
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddFilter(_filterexpression);

            EntityCollection _potentialprospectpartsublinescollection = _DL_tss_potentialprospectpartsublines.Select(organizationService, _queryexpression);
            #endregion

            #region GET SELECTED POTENTIAL PROSPECT PART
            Entity _potentialprospectpart = new Entity();
            EntityCollection _potentialprospectpartlinescollection = new EntityCollection();

            if (_potentialprospectpartsublinescollection.Entities.Count() > 0)
            {
                if (!_potentialprospectpartsublinescollection.Entities[0].Attributes.Contains("tss_potentialprospectpartlinesref"))
                {
                    _potentialprospectpart = _DL_tss_potentialprospectpart.Select(organizationService, _potentialprospectpartsublinescollection.Entities[0].GetAttributeValue<EntityReference>("tss_potentialprospectpartref").Id);

                    Entity _groupuiocommodityheader = organizationService.Retrieve("tss_groupuiocommodityheader", _potentialprospectpart.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id, new ColumnSet(true));

                    if (_groupuiocommodityheader != null)
                        _title = _groupuiocommodityheader.GetAttributeValue<string>("tss_groupuiocommodityname");
                    else
                        _title = "[No Name]";
                }
                else
                {
                    #region GET ALL SELECTED POTENTIAL PROSPECT PART LINE
                    _filterexpression = new FilterExpression(LogicalOperator.Or);

                    foreach (Entity _subline in _potentialprospectpartsublinescollection.Entities)
                    {
                        _filterexpression.AddCondition("tss_potentialprospectpartlinesid", ConditionOperator.Equal, _subline.GetAttributeValue<EntityReference>("tss_potentialprospectpartlinesref").Id);
                    }

                    _queryexpression = new QueryExpression(_DL_tss_potentialprospectpartlines.EntityName);
                    _queryexpression.Criteria.AddFilter(_filterexpression);
                    _queryexpression.ColumnSet = new ColumnSet(true);

                    _potentialprospectpartlinescollection = _DL_tss_potentialprospectpartlines.Select(organizationService, _queryexpression);
                    _potentialprospectpart = _DL_tss_potentialprospectpart.Select(organizationService, _potentialprospectpartlinescollection.Entities[0].GetAttributeValue<EntityReference>("tss_potentialprospectpart").Id);

                    _title = _potentialprospectpart.GetAttributeValue<string>("tss_serialnumberpopulation");
                    #endregion

                    #region GET LATEST EST.CLOSE DATE
                    foreach (Entity _potentialprospectpartlinesitem in _potentialprospectpartlinescollection.Entities)
                    {
                        DateTime _estclosedate = _potentialprospectpartlinesitem.GetAttributeValue<DateTime>("tss_estclosedate");

                        if (_estclosedate.Date != DateTime.MinValue.Date)
                        {
                            if (_estclosedate > _newdatetime)
                                _newdatetime = _estclosedate;
                        }
                    }
                    #endregion
                }
            }
            else
            {
                throw new InvalidPluginExecutionException("Convert failed. Cannot find any lines.");
            }

            Entity _customer = organizationService.Retrieve("account", _potentialprospectpart.GetAttributeValue<EntityReference>("tss_customer").Id, new ColumnSet(true));
            #endregion

            #region SET TOTAL AMOUNT
            int _totalqty = 0;
            int _remainingqty = 0;
            decimal _totalprice = 0;
            decimal _totalamount = 0;
            bool _isdirectsales = false;

            foreach (Entity _potentialprospectpartsublinesitem in _potentialprospectpartsublinescollection.Entities)
            {
                _totalqty += Convert.ToInt32(_potentialprospectpartsublinesitem["tss_qty"]);
                _remainingqty += Convert.ToInt32(_potentialprospectpartsublinesitem["tss_remainingqty"]);
                _totalprice += Convert.ToDecimal(_potentialprospectpartsublinesitem.GetAttributeValue<Money>("tss_price").Value);
            }

            if (_remainingqty == 0)
            {
                //throw new InvalidPluginExecutionException("Cannot Generate Prospect Part. No remaining quantity.");
                _prospectpartheader["tss_sourcetype"] = new OptionSetValue(865920000);
                _isdirectsales = true;
            }
            else
            {
                _prospectpartheader["tss_sourcetype"] = new OptionSetValue(865920001);
            }

            _totalamount = _totalqty * _totalprice;
            #endregion

            #region GET BRANCH CODE
            string _branchcode = string.Empty;
            Entity _businessunit = _DL_businessunit.Select(organizationService, context.BusinessUnitId);

            if (_businessunit.Attributes.Contains("trs_branchcode"))
                _branchcode = _businessunit.Attributes["trs_branchcode"].ToString();
            #endregion

            #region GENERATE PROSPECT PART HEADER
            _prospectpartheader["tss_estimationclosedate"] = _newdatetime;
            _prospectpartheader["tss_pipelinephase"] = new OptionSetValue(865920000);
            _prospectpartheader["tss_statusreason"] = new OptionSetValue(865920000);
            _prospectpartheader["tss_customer"] = _potentialprospectpart.GetAttributeValue<EntityReference>("tss_customer");
            _prospectpartheader["tss_refmarketsize"] = new EntityReference(_DL_tss_potentialprospectpart.EntityName, _potentialprospectpart.Id);
            _prospectpartheader["tss_currency"] = new EntityReference("transactioncurrency", new Guid("7459618D-4EE9-E111-9AA2-544249894792"));
            _prospectpartheader["ownerid"] = new EntityReference("systemuser", context.UserId);
            _prospectpartheader["tss_pss"] = _potentialprospectpart.GetAttributeValue<EntityReference>("tss_pss");
            _prospectpartheader["tss_branch"] = new EntityReference(_DL_businessunit.EntityName, context.BusinessUnitId);
            _prospectpartheader["tss_totalamount"] = _totalamount;
            _prospectpartheader["tss_topic"] = "Prospect From MS - " +
                _title +
                " - " +
                DateTime.Now.ToLocalTime().Year.ToString() +
                " by Unit";

            if (_customer.Attributes.Contains("primarycontactid"))
                _prospectpartheader["tss_contact"] = _customer.GetAttributeValue<EntityReference>("primarycontactid");

            Guid _prospectpartheaderid = organizationService.Create(_prospectpartheader);
            #endregion

            #region GENERATE PROSPECT PART LINE
            GenerateProspectSubLines(organizationService, _potentialprospectpart, _potentialprospectpartsublinescollection, _prospectpartheaderid, _isdirectsales);
            #endregion
        }























        //public void GenerateProspectPartFromPotentialProspectPartLine_OnClick(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        //{
        //    Entity _prospectpartheader = new Entity("tss_prospectpartheader");
        //    Entity _potentialprospectpartlines = _DL_tss_potentialprospectpartlines.Select(organizationService, context.PrimaryEntityId);
        //    Entity _potentialprospectpart = _DL_tss_potentialprospectpart.Select(organizationService, _potentialprospectpartlines.GetAttributeValue<EntityReference>("tss_potentialprospectpart").Id);
        //    Entity _customer = organizationService.Retrieve("account", _potentialprospectpart.GetAttributeValue<EntityReference>("tss_customer").Id, new ColumnSet(true));

        //    #region SET TOTAL AMOUNT
        //    int _totalqty = 0;
        //    int _remainingqtyactual = 0;
        //    decimal _totalprice = 0;
        //    decimal _totalamount = 0;
        //    bool _isdirectsales = false;

        //    FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
        //    _filterexpression.AddCondition("tss_potentialprospectpartlinesref", ConditionOperator.Equal, _potentialprospectpartlines["tss_potentialprospectpartlinesid"].ToString());

        //    QueryExpression _queryexpression = new QueryExpression("tss_potentialprospectpartsublines");
        //    _queryexpression.Criteria.AddFilter(_filterexpression);
        //    _queryexpression.ColumnSet = new ColumnSet(true);

        //    EntityCollection _potentialprospectpartsublinescollection = organizationService.RetrieveMultiple(_queryexpression);

        //    foreach (Entity _potentialprospectpartsublinesitem in _potentialprospectpartsublinescollection.Entities)
        //    {
        //        _totalqty += Convert.ToInt32(_potentialprospectpartsublinesitem["tss_qty"]);
        //        _remainingqtyactual += Convert.ToInt32(_potentialprospectpartsublinesitem["tss_remainingqtyactual"]);
        //        _totalprice += Convert.ToDecimal(_potentialprospectpartsublinesitem.GetAttributeValue<Money>("tss_price").Value);
        //    }

        //    if (_remainingqtyactual == 0)
        //    {
        //        //throw new InvalidPluginExecutionException("Cannot Generate Prospect Part. No remaining quantity.");
        //        _prospectpartheader["tss_sourcetype"] = new OptionSetValue(865920000);
        //        _isdirectsales = true;
        //    }
        //    else
        //    {
        //        _prospectpartheader["tss_sourcetype"] = new OptionSetValue(865920001);
        //    }

        //    _totalamount = _totalqty * _totalprice;
        //    #endregion

        //    #region GET LATEST EST.CLOSE DATE
        //    DateTime _newdatetime = new DateTime(1900, 1, 1);

        //    DateTime _estclosedate = _potentialprospectpartlines.GetAttributeValue<DateTime>("tss_estclosedate");

        //    if (_estclosedate.Date != DateTime.MinValue.Date)
        //    {
        //        if (_estclosedate > _newdatetime)
        //            _newdatetime = _estclosedate;
        //    }
        //    #endregion

        //    #region GET BRANCH CODE
        //    string _branchcode = string.Empty;
        //    Entity _businessunit = _DL_businessunit.Select(organizationService, context.BusinessUnitId);

        //    if (_businessunit.Attributes.Contains("trs_branchcode"))
        //        _branchcode = _businessunit.Attributes["trs_branchcode"].ToString();
        //    #endregion

        //    #region GENERATE PROSPECT PART HEADER
        //    _prospectpartheader["tss_estimationclosedate"] = _newdatetime;
        //    _prospectpartheader["tss_pipelinephase"] = new OptionSetValue(865920000);
        //    _prospectpartheader["tss_statusreason"] = new OptionSetValue(865920000);
        //    _prospectpartheader["tss_customer"] = _potentialprospectpart.GetAttributeValue<EntityReference>("tss_customer");
        //    _prospectpartheader["tss_refmarketsize"] = new EntityReference(_DL_tss_potentialprospectpart.EntityName, _potentialprospectpart.Id);
        //    _prospectpartheader["tss_currency"] = new EntityReference("transactioncurrency", new Guid("7459618D-4EE9-E111-9AA2-544249894792"));
        //    _prospectpartheader["ownerid"] = new EntityReference("systemuser", context.UserId);
        //    _prospectpartheader["tss_pss"] = _potentialprospectpart.GetAttributeValue<EntityReference>("tss_pss");
        //    _prospectpartheader["tss_branch"] = new EntityReference(_DL_businessunit.EntityName, context.BusinessUnitId);
        //    _prospectpartheader["tss_totalamount"] = _totalamount;
        //    _prospectpartheader["tss_topic"] = "Prospect From MS - " +
        //        _potentialprospectpart.GetAttributeValue<string>("tss_serialnumberpopulation") +
        //        " - " +
        //        DateTime.Now.ToLocalTime().Year.ToString() +
        //        " by Unit";

        //    if (_customer.Attributes.Contains("primarycontactid"))
        //        _prospectpartheader["tss_contact"] = _customer.GetAttributeValue<EntityReference>("primarycontactid");

        //    Guid _prospectpartheaderid = organizationService.Create(_prospectpartheader);
        //    #endregion

        //    EntityCollection _potentialprospectpartlinescollection = new EntityCollection();

        //    _potentialprospectpartlinescollection.Entities.Add(_potentialprospectpartlines);

        //    SetProspectPart(organizationService, _potentialprospectpart, _potentialprospectpartlinescollection, _prospectpartheaderid, _isdirectsales);
        //}


        //public void GenerateProspectPartFromPotentialProspectPartSubLine_OnClick(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context, string recordID)
        //{
        //    Entity _prospectpartheader = new Entity("tss_prospectpartheader");
        //    string[] _recordidcollection = recordID.Split(',');
        //    string _title = "";
        //    DateTime _newdatetime = new DateTime(1900, 1, 1);

        //    #region GET ALL SELECTED POTENTIAL PROSPECT PART SUBLINES
        //    FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
        //    _filterexpression.AddCondition("tss_potentialprospectpartsublinesid", ConditionOperator.In, _recordidcollection);

        //    QueryExpression _queryexpression = new QueryExpression("tss_potentialprospectpartsublines");
        //    _queryexpression.ColumnSet = new ColumnSet(true);
        //    _queryexpression.Criteria.AddFilter(_filterexpression);

        //    EntityCollection _potentialprospectpartsublinescollection = _DL_tss_potentialprospectpartsublines.Select(organizationService, _queryexpression);
        //    #endregion

        //    #region GET SELECTED POTENTIAL PROSPECT PART
        //    Entity _potentialprospectpart = new Entity();
        //    EntityCollection _potentialprospectpartlinescollection = new EntityCollection();

        //    if (_potentialprospectpartsublinescollection.Entities.Count() > 0)
        //    {
        //        if (!_potentialprospectpartsublinescollection.Entities[0].Attributes.Contains("tss_potentialprospectpartlinesref"))
        //        {
        //            _potentialprospectpart = _DL_tss_potentialprospectpart.Select(organizationService, _potentialprospectpartsublinescollection.Entities[0].GetAttributeValue<EntityReference>("tss_potentialprospectpartref").Id);

        //            Entity _groupuiocommodityheader = organizationService.Retrieve("tss_groupuiocommodityheader", _potentialprospectpart.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id, new ColumnSet(true));

        //            if (_groupuiocommodityheader != null)
        //                _title = _groupuiocommodityheader.GetAttributeValue<string>("tss_groupuiocommodityname");
        //            else
        //                _title = "[No Name]";
        //        }
        //        else
        //        {
        //            #region GET ALL SELECTED POTENTIAL PROSPECT PART LINE
        //            _filterexpression = new FilterExpression(LogicalOperator.Or);

        //            foreach (Entity _subline in _potentialprospectpartsublinescollection.Entities)
        //            {
        //                _filterexpression.AddCondition("tss_potentialprospectpartlinesid", ConditionOperator.Equal, _subline.GetAttributeValue<EntityReference>("tss_potentialprospectpartlinesref").Id);
        //            }

        //            _queryexpression = new QueryExpression(_DL_tss_potentialprospectpartlines.EntityName);
        //            _queryexpression.Criteria.AddFilter(_filterexpression);
        //            _queryexpression.ColumnSet = new ColumnSet(true);

        //            _potentialprospectpartlinescollection = _DL_tss_potentialprospectpartlines.Select(organizationService, _queryexpression);
        //            _potentialprospectpart = _DL_tss_potentialprospectpart.Select(organizationService, _potentialprospectpartlinescollection.Entities[0].GetAttributeValue<EntityReference>("tss_potentialprospectpart").Id);

        //            _title = _potentialprospectpart.GetAttributeValue<string>("tss_serialnumberpopulation");
        //            #endregion

        //            #region GET LATEST EST.CLOSE DATE
        //            foreach (Entity _potentialprospectpartlinesitem in _potentialprospectpartlinescollection.Entities)
        //            {
        //                DateTime _estclosedate = _potentialprospectpartlinesitem.GetAttributeValue<DateTime>("tss_estclosedate");

        //                if (_estclosedate.Date != DateTime.MinValue.Date)
        //                {
        //                    if (_estclosedate > _newdatetime)
        //                        _newdatetime = _estclosedate;
        //                }
        //            }
        //            #endregion
        //        }
        //    }
        //    else
        //    {
        //        throw new InvalidPluginExecutionException("Convert failed. Cannot find any lines.");
        //    }

        //    Entity _customer = organizationService.Retrieve("account", _potentialprospectpart.GetAttributeValue<EntityReference>("tss_customer").Id, new ColumnSet(true));
        //    #endregion

        //    #region SET TOTAL AMOUNT
        //    int _totalqty = 0;
        //    int _remainingqtyactual = 0;
        //    decimal _totalprice = 0;
        //    decimal _totalamount = 0;
        //    bool _isdirectsales = false;

        //    foreach (Entity _potentialprospectpartsublinesitem in _potentialprospectpartsublinescollection.Entities)
        //    {
        //        _totalqty += Convert.ToInt32(_potentialprospectpartsublinesitem["tss_qty"]);
        //        _remainingqtyactual += Convert.ToInt32(_potentialprospectpartsublinesitem["tss_remainingqtyactual"]);
        //        _totalprice += Convert.ToDecimal(_potentialprospectpartsublinesitem.GetAttributeValue<Money>("tss_price").Value);
        //    }

        //    if (_remainingqtyactual == 0)
        //    {
        //        //throw new InvalidPluginExecutionException("Cannot Generate Prospect Part. No remaining quantity.");
        //        _prospectpartheader["tss_sourcetype"] = new OptionSetValue(865920000);
        //        _isdirectsales = true;
        //    }
        //    else
        //    {
        //        _prospectpartheader["tss_sourcetype"] = new OptionSetValue(865920001);
        //    }

        //    _totalamount = _totalqty * _totalprice;
        //    #endregion

        //    #region GET BRANCH CODE
        //    string _branchcode = string.Empty;
        //    Entity _businessunit = _DL_businessunit.Select(organizationService, context.BusinessUnitId);

        //    if (_businessunit.Attributes.Contains("trs_branchcode"))
        //        _branchcode = _businessunit.Attributes["trs_branchcode"].ToString();
        //    #endregion

        //    #region GENERATE PROSPECT PART HEADER
        //    _prospectpartheader["tss_estimationclosedate"] = _newdatetime;
        //    _prospectpartheader["tss_pipelinephase"] = new OptionSetValue(865920000);
        //    _prospectpartheader["tss_statusreason"] = new OptionSetValue(865920000);
        //    _prospectpartheader["tss_customer"] = _potentialprospectpart.GetAttributeValue<EntityReference>("tss_customer");
        //    _prospectpartheader["tss_refmarketsize"] = new EntityReference(_DL_tss_potentialprospectpart.EntityName, _potentialprospectpart.Id);
        //    _prospectpartheader["tss_currency"] = new EntityReference("transactioncurrency", new Guid("7459618D-4EE9-E111-9AA2-544249894792"));
        //    _prospectpartheader["ownerid"] = new EntityReference("systemuser", context.UserId);
        //    _prospectpartheader["tss_pss"] = _potentialprospectpart.GetAttributeValue<EntityReference>("tss_pss");
        //    _prospectpartheader["tss_branch"] = new EntityReference(_DL_businessunit.EntityName, context.BusinessUnitId);
        //    _prospectpartheader["tss_totalamount"] = _totalamount;
        //    _prospectpartheader["tss_topic"] = "Prospect From MS - " +
        //        _title +
        //        " - " +
        //        DateTime.Now.ToLocalTime().Year.ToString() +
        //        " by Unit";

        //    if (_customer.Attributes.Contains("primarycontactid"))
        //        _prospectpartheader["tss_contact"] = _customer.GetAttributeValue<EntityReference>("primarycontactid");

        //    Guid _prospectpartheaderid = organizationService.Create(_prospectpartheader);
        //    #endregion

        //    #region GENERATE PROSPECT PART LINE
        //    GenerateProspectSubLines(organizationService, _potentialprospectpart, _potentialprospectpartsublinescollection, _prospectpartheaderid, _isdirectsales);
        //    #endregion
        //}







































        //public void GenerateMasterProspectPartHeader_OnClick(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context, string recordID)
        //{
        //    string errModul = string.Empty;

        //    try
        //    {
        //        if (String.IsNullOrEmpty(recordID))
        //        {
        //            throw new InvalidWorkflowException("Unable to find potential prospect part, please make sure to save and then retry the action .");
        //        }
        //        else
        //        {
        //            //Entity _ENT_potentialprospectpart = new Entity("DL_tss_potentialprospectpart");
        //            #region get current potential prospect part
        //            FilterExpression fPP = new FilterExpression(LogicalOperator.And);
        //            fPP.AddCondition("tss_potentialprospectpartid", ConditionOperator.Equal, new Guid(recordID));

        //            QueryExpression qPP = new QueryExpression("tss_potentialprospectpart");
        //            qPP.Criteria.AddFilter(fPP);
        //            qPP.ColumnSet = new ColumnSet(true);

        //            EntityCollection listPotentialProspectPart = _DL_tss_potentialprospectpart.Select(organizationService, qPP);
        //            #endregion

        //            #region get current potential prospect part lines
        //            FilterExpression fPPL = new FilterExpression(LogicalOperator.And);
        //            fPPL.AddCondition("tss_potentialprospectpart", ConditionOperator.Equal, listPotentialProspectPart.Entities[0]["tss_potentialprospectpartid"].ToString());

        //            QueryExpression qPPL = new QueryExpression("tss_potentialprospectpartlines");
        //            qPPL.Criteria.AddFilter(fPPL);
        //            qPPL.ColumnSet = new ColumnSet(true);

        //            EntityCollection listPotentialProspectPartLines = _DL_tss_potentialprospectpartlines.Select(organizationService, qPPL);

        //            #endregion

        //            #region get total Amount

        //            int totalQty = 0;
        //            decimal totalPrice = 0;
        //            decimal totalAmount = 0;

        //            if (listPotentialProspectPartLines.Entities.Count == 0)
        //            {
        //                FilterExpression fPPSL = new FilterExpression(LogicalOperator.And);
        //                fPPSL.AddCondition("tss_potentialprospectpartref", ConditionOperator.Equal, listPotentialProspectPart.Entities[0]["tss_potentialprospectpartid"].ToString());

        //                QueryExpression qPPSL = new QueryExpression("tss_potentialprospectpartsublines");
        //                qPPSL.Criteria.AddFilter(fPPSL);
        //                qPPSL.ColumnSet = new ColumnSet(true);

        //                EntityCollection listPotentialProspectPartSubLines = _DL_tss_potentialprospectpartsublines.Select(organizationService, qPPSL);

        //                foreach (Entity enSubLines in listPotentialProspectPartSubLines.Entities)
        //                {
        //                    totalQty += Convert.ToInt32(enSubLines["tss_qty"]);
        //                    totalPrice += Convert.ToDecimal(enSubLines.GetAttributeValue<Money>("tss_price").Value);
        //                }
        //            }
        //            else
        //            {
        //                foreach (Entity eLines in listPotentialProspectPartLines.Entities)
        //                {

        //                    FilterExpression fPPSL = new FilterExpression(LogicalOperator.And);
        //                    fPPSL.AddCondition("tss_potentialprospectpartlinesref", ConditionOperator.Equal, eLines["tss_potentialprospectpartlinesid"].ToString());

        //                    QueryExpression qPPSL = new QueryExpression("tss_potentialprospectpartsublines");
        //                    qPPSL.Criteria.AddFilter(fPPSL);
        //                    qPPSL.ColumnSet = new ColumnSet(true);

        //                    EntityCollection listPotentialProspectPartSubLines = _DL_tss_potentialprospectpartsublines.Select(organizationService, qPPSL);

        //                    foreach (Entity enSubLines in listPotentialProspectPartSubLines.Entities)
        //                    {
        //                        totalQty += Convert.ToInt32(enSubLines["tss_qty"]);
        //                        totalPrice += Convert.ToDecimal(enSubLines.GetAttributeValue<Money>("tss_price").Value);
        //                    }
        //                }
        //            }

        //            totalAmount = totalQty * totalPrice;
        //            #endregion

        //            #region get latest est Close Date

        //            DateTime estCloseDate = new DateTime(1900, 1, 1);

        //            foreach (Entity en in listPotentialProspectPartLines.Entities)
        //            {
        //                if (!String.IsNullOrEmpty(en.GetAttributeValue<DateTime>("tss_estclosedate").ToString()))
        //                {
        //                    if (en.GetAttributeValue<DateTime>("tss_estclosedate") > estCloseDate)
        //                    {
        //                        estCloseDate = en.GetAttributeValue<DateTime>("tss_estclosedate");
        //                    }
        //                }
        //            }

        //            #endregion
        //            #region get CompanyName

        //            FilterExpression fC = new FilterExpression(LogicalOperator.And);
        //            fC.AddCondition("accountid", ConditionOperator.Equal, listPotentialProspectPart.Entities[0].GetAttributeValue<EntityReference>("tss_customer").Id);

        //            QueryExpression qC = new QueryExpression("account");
        //            qC.Criteria.AddFilter(fC);
        //            qC.ColumnSet = new ColumnSet(true);

        //            EntityCollection listCustomer = _DL_account.Select(organizationService, qC);

        //            #endregion


        //            #region getBranchCode
        //            string branchCode = string.Empty;
        //            Entity eBusinessUnit = _DL_businessunit.Select(organizationService, context.BusinessUnitId);
        //            //throw new Exception("Test : " + eBusinessUnit.Attributes["trs_branchcode"].ToString());
        //            if (eBusinessUnit.Attributes.Contains("trs_branchcode"))
        //            {
        //                branchCode = eBusinessUnit.Attributes["trs_branchcode"].ToString();
        //            }
        //            #endregion

        //            #region generate prospectpartHeader

        //            Entity ENT_prospectpartheader = new Entity("tss_prospectpartheader");
        //            ENT_prospectpartheader["tss_sourcetype"] = new OptionSetValue(865920001);
        //            ENT_prospectpartheader["tss_estimationclosedate"] = estCloseDate;
        //            ENT_prospectpartheader["tss_pipelinephase"] = new OptionSetValue(865920000);
        //            ENT_prospectpartheader["tss_statusreason"] = new OptionSetValue(865920000);
        //            ENT_prospectpartheader["tss_customer"] = listPotentialProspectPart.Entities[0].GetAttributeValue<EntityReference>("tss_customer");
        //            if (listCustomer.Entities[0].GetAttributeValue<EntityReference>("primarycontactid") != null)
        //            {
        //                ENT_prospectpartheader["tss_contact"] = listCustomer.Entities[0].GetAttributeValue<EntityReference>("primarycontactid");
        //            }
        //            ENT_prospectpartheader["tss_refmarketsize"] = listPotentialProspectPart.Entities[0].GetAttributeValue<Guid>("tss_potentialprospectpartid");
        //            ENT_prospectpartheader["tss_currency"] = new EntityReference("transactioncurrency", new Guid("7459618D-4EE9-E111-9AA2-544249894792"));
        //            ENT_prospectpartheader["ownerid"] = new EntityReference("systemuser", context.UserId);
        //            ENT_prospectpartheader["tss_pss"] = listPotentialProspectPart.Entities[0].GetAttributeValue<EntityReference>("tss_pss");
        //            ENT_prospectpartheader["tss_branch"] = new EntityReference(_DL_businessunit.EntityName, context.BusinessUnitId);
        //            ENT_prospectpartheader["tss_totalamount"] = totalAmount;
        //            ENT_prospectpartheader["tss_topic"] = "Prospect From MS - " +
        //                listPotentialProspectPart.Entities[0].GetAttributeValue<string>("tss_serialnumberpopulation") +
        //                " - " +
        //                DateTime.Now.ToLocalTime().Year.ToString() +
        //                " by Unit";

        //            //AssignRequest assignRequest = new AssignRequest
        //            //{
        //            //    Assignee = new EntityReference("systemuser", listPotentialProspectPart.Entities[0].GetAttributeValue<EntityReference>("tss_pss").Id),
        //            //    Target = new EntityReference("tss_prospectpartheader", ENT_prospectpartheader.Id)
        //            //};
        //            //organizationService.Execute(assignRequest);
        //            errModul = "Generate Prospect Header failed";
        //            Guid prospectPartHeaderID = organizationService.Create(ENT_prospectpartheader);
        //            #endregion

        //            #region generate prospect part line 
        //            int lineNumber = 0;

        //            if (listPotentialProspectPartLines.Entities.Count == 0)
        //            {
        //                FilterExpression fPPSL = new FilterExpression(LogicalOperator.And);
        //                fPPSL.AddCondition("tss_potentialprospectpartref", ConditionOperator.Equal, listPotentialProspectPart.Entities[0]["tss_potentialprospectpartid"].ToString());

        //                QueryExpression qPPSL = new QueryExpression("tss_potentialprospectpartsublines");
        //                qPPSL.Criteria.AddFilter(fPPSL);
        //                qPPSL.ColumnSet = new ColumnSet(true);

        //                EntityCollection listPotentialProspectPartSubLines = _DL_tss_potentialprospectpartsublines.Select(organizationService, qPPSL);


        //                var groupPotentialProspectPartSubLines = (from r in listPotentialProspectPartSubLines.Entities.AsEnumerable()
        //                                                          group r by new
        //                                                          {
        //                                                              partnumberId = (r.GetAttributeValue<EntityReference>("tss_partnumber").Id)
        //                                                          } into g
        //                                                          select new
        //                                                          {
        //                                                              qty = g.Sum(x => x.GetAttributeValue<int>("tss_qty")),
        //                                                              parnumberId = g.Key.partnumberId
        //                                                          }).ToList();

        //                foreach (var ppsubline in groupPotentialProspectPartSubLines)
        //                {
        //                    lineNumber += 10;
        //                    Entity ENT_prospectpartline = new Entity("tss_prospectpartlines");
        //                    ENT_prospectpartline["tss_itemnumber"] = lineNumber;
        //                    ENT_prospectpartline["tss_prospectpartheader"] = new EntityReference("tss_prospectpartheader", prospectPartHeaderID);
        //                    ENT_prospectpartline["tss_partnumber"] = new EntityReference("trs_masterpart", ppsubline.parnumberId); // enSubLines.GetAttributeValue<EntityReference>("tss_partnumber");

        //                    Entity oPN = organizationService.Retrieve("trs_masterpart", ppsubline.parnumberId, new ColumnSet(true));
        //                    ENT_prospectpartline["tss_partdescription"] = oPN.GetAttributeValue<String>("trs_partdescription");

        //                    #region GET PRICE
        //                    List<object> oPrice = GetSparePartPriceMaster(organizationService, ppsubline.parnumberId);

        //                    ENT_prospectpartline["tss_price"] = new Money((decimal)oPrice[0]);
        //                    ENT_prospectpartline["tss_minimumprice"] = new Money((decimal)oPrice[1]);
        //                    ENT_prospectpartline["transactioncurrencyid"] = new EntityReference("transactioncurrency", (Guid)oPrice[2]);
        //                    //ENT_prospectpartline["tss_qtymarketsize"] = 0;
        //                    ENT_prospectpartline["tss_discountamount"] = new Money(0);
        //                    ENT_prospectpartline["tss_discountpercent"] = Convert.ToDecimal(0);
        //                    ENT_prospectpartline["tss_finalprice"] = new Money(0);
        //                    #endregion

        //                    #region FIND UNIT GROUP
        //                    if (listPotentialProspectPart.Entities[0].GetAttributeValue<EntityReference>("tss_serialnumber") != null)
        //                    {

        //                        FilterExpression fUnitGroup = new FilterExpression(LogicalOperator.And);
        //                        fUnitGroup.AddCondition("new_populationid", ConditionOperator.Equal, listPotentialProspectPart.Entities[0].GetAttributeValue<EntityReference>("tss_serialnumber").Id);

        //                        QueryExpression qUnitGroup = new QueryExpression("new_population");
        //                        qUnitGroup.Criteria.AddFilter(fUnitGroup);
        //                        qUnitGroup.ColumnSet = new ColumnSet(true);

        //                        EntityCollection oUnitGroup = _DL_population.Select(organizationService, qUnitGroup);

        //                        if (oUnitGroup.Entities.Count() > 0)
        //                        {
        //                            fUnitGroup = new FilterExpression(LogicalOperator.And);
        //                            fUnitGroup.AddCondition("name", ConditionOperator.Equal, oUnitGroup.Entities[0].GetAttributeValue<string>("new_productitem"));

        //                            qUnitGroup = new QueryExpression("uomschedule");
        //                            qUnitGroup.Criteria.AddFilter(fUnitGroup);
        //                            qUnitGroup.ColumnSet = new ColumnSet(true);

        //                            EntityCollection ounit = organizationService.RetrieveMultiple(qUnitGroup);
        //                            ENT_prospectpartline["tss_unitgroup"] = new EntityReference("uomschedule", ounit.Entities[0].Id);
        //                        }
        //                    }
        //                    #endregion

        //                    #region get PartType
        //                    FilterExpression fPT = new FilterExpression(LogicalOperator.And);
        //                    fPT.AddCondition("trs_masterpartid", ConditionOperator.Equal, ppsubline.parnumberId);

        //                    QueryExpression qPT = new QueryExpression("trs_masterpart");
        //                    qPT.Criteria.AddFilter(fPT);
        //                    qPT.ColumnSet = new ColumnSet(true);

        //                    EntityCollection listPartMaster = _DL_partMaster.Select(organizationService, qPT);

        //                    errModul = "Get Part Master failed";
        //                    ENT_prospectpartline["tss_parttype"] = new OptionSetValue(listPartMaster.Entities[0].GetAttributeValue<OptionSetValue>("tss_parttype").Value); // listPartMaster.Entities[0].GetAttributeValue<OptionSetValue>("tss_parttype").Value;
        //                    #endregion

        //                    #region look up to market size 
        //                    FilterExpression fMS = new FilterExpression(LogicalOperator.And);
        //                    fMS.AddCondition("tss_pss", ConditionOperator.Equal, listPotentialProspectPart.Entities[0].GetAttributeValue<EntityReference>("tss_pss").Id);
        //                    fMS.AddCondition("tss_customer", ConditionOperator.Equal, listPotentialProspectPart.Entities[0].GetAttributeValue<EntityReference>("tss_customer").Id);
        //                    //2018.09.17 - start = lessequal & end = greaterequal
        //                    //fMS.AddCondition("tss_msperiodend", ConditionOperator.LessEqual, DateTime.Now);
        //                    //fMS.AddCondition("tss_msperiodstart", ConditionOperator.GreaterEqual, DateTime.Now);

        //                    //fMS.AddCondition("tss_msperiodend", ConditionOperator.GreaterEqual, DateTime.Now);
        //                    //fMS.AddCondition("tss_msperiodstart", ConditionOperator.LessEqual, DateTime.Now);

        //                    //Group UIO Commodity
        //                    if (listPotentialProspectPart.Entities[0].GetAttributeValue<EntityReference>("tss_serialnumber") != null)
        //                    {
        //                        fMS.AddCondition("tss_serialnumber", ConditionOperator.Equal, listPotentialProspectPart.Entities[0].GetAttributeValue<EntityReference>("tss_serialnumber").Id);
        //                    }
        //                    else
        //                    {
        //                        fMS.AddCondition("tss_groupuiocommodity", ConditionOperator.Equal, listPotentialProspectPart.Entities[0].GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id);
        //                    }


        //                    QueryExpression qMS = new QueryExpression("tss_mastermarketsize");
        //                    qMS.Criteria.AddFilter(fMS);
        //                    qMS.ColumnSet = new ColumnSet(true);

        //                    errModul = "Get Master market size failed";
        //                    //EntityCollection listMarketSize = _DL_tss_marketmarketsize.Select(organizationService, qMS);
        //                    EntityCollection listMarketSize = _DL_tss_marketmarketsize.Select(organizationService, qMS);
        //                    #endregion


        //                    //Todo process
        //                    #region look up to market size Sublines

        //                    FilterExpression fMSSL = new FilterExpression(LogicalOperator.And);
        //                    fMSSL.AddCondition("tss_mastermarketsizeid", ConditionOperator.Equal, listMarketSize.Entities[0].Id);// listMarketSizeLines.Entities[0].GetAttributeValue<EntityReference>("tss_mastermarketsizelinesid").Id );

        //                    QueryExpression qMSSL = new QueryExpression("tss_mastermarketsizesublines");
        //                    qMSSL.Criteria.AddFilter(fMSSL);
        //                    qMSSL.ColumnSet = new ColumnSet(true);

        //                    errModul = "Get Master market size sub line failed";
        //                    EntityCollection listMarketSizeSubLines = _DL_tss_marketmarketsizesublines.Select(organizationService, qMSSL);

        //                    #endregion

        //                    #region look up to Total part consump market size

        //                    FilterExpression fTCMS = new FilterExpression(LogicalOperator.And);
        //                    //fTCMS.AddCondition("tss_marketsizeid", ConditionOperator.Equal, listMarketSize.Entities[0].Id);
        //                    fTCMS.AddCondition("tss_marketsizeid", ConditionOperator.Equal, listPotentialProspectPart.Entities[0].GetAttributeValue<EntityReference>("tss_marketsizeid").Id);

        //                    QueryExpression qTCMS = new QueryExpression("tss_totalpartconsumpmarketsize");
        //                    qTCMS.Criteria.AddFilter(fTCMS);
        //                    qTCMS.ColumnSet = new ColumnSet(true);

        //                    errModul = "Get total part consump market size failed";
        //                    EntityCollection listTotalPartConsumpMarketSize = _DL_tss_totalpartconsumpmarketsize.Select(organizationService, qTCMS);


        //                    #region FIND PRICE TYPE
        //                    FilterExpression fPriceType = new FilterExpression(LogicalOperator.And);
        //                    fPriceType.AddCondition("tss_type", ConditionOperator.Equal, 865920001);

        //                    QueryExpression qPriceType = new QueryExpression("tss_pricelistpart");
        //                    qPriceType.Criteria.AddFilter(fPriceType);
        //                    qPriceType.ColumnSet = new ColumnSet(true);

        //                    EntityCollection oPriceType = _DL_tss_pricelistpart.Select(organizationService, qPriceType);

        //                    #endregion

        //                    #endregion

        //                    errModul = "Insert Prospect Part Line failed";

        //                    if (listTotalPartConsumpMarketSize.Entities.Count > 0)
        //                    {
        //                        #region if Qty potential prospect <= remaining Qty di total Consump
        //                        if (Convert.ToDecimal(ppsubline.qty) <= listTotalPartConsumpMarketSize.Entities[0].GetAttributeValue<int>("tss_remainingqty"))
        //                        {
        //                            //insert prospect partline
        //                            ENT_prospectpartline["tss_sourcetype"] = new OptionSetValue(865920001);
        //                            ENT_prospectpartline["tss_pricetype"] = new EntityReference("tss_pricelistpart", oPriceType.Entities[0].Id);
        //                            ENT_prospectpartline["tss_refmarketsize"] = new EntityReference(_DL_tss_potentialprospectpart.EntityName, listPotentialProspectPart.Entities[0].GetAttributeValue<Guid>("tss_potentialprospectpartid"));
        //                            ENT_prospectpartline["tss_quantity"] = Convert.ToDecimal(ppsubline.qty);
        //                            ENT_prospectpartline["tss_qtymarketsize"] = Convert.ToDecimal(ppsubline.qty);
        //                            ENT_prospectpartline["tss_priceamount"] = new Money((decimal)oPrice[0] * Convert.ToDecimal(ppsubline.qty));
        //                            //organizationService.Create(ENT_prospectpartline);

        //                            //update total partConsump market size 

        //                            decimal remainingQty = 0;
        //                            remainingQty = Convert.ToDecimal(listTotalPartConsumpMarketSize.Entities[0]["tss_remainingqty"]) - Convert.ToDecimal(ppsubline.qty);



        //                            if (remainingQty == 0)
        //                            {
        //                                //Update status potential prospect part
        //                                listPotentialProspectPart.Entities[0]["tss_status"] = new OptionSetValue(865920002);
        //                                listPotentialProspectPart.Entities[0]["tss_statusreason"] = new OptionSetValue(865920002);
        //                                organizationService.Update(listPotentialProspectPart.Entities[0]);

        //                                //Update Remaining Qty di Market Size Sub Lines
        //                                if (listMarketSizeSubLines.Entities[0].Attributes.Contains("tss_remainingqty"))
        //                                {
        //                                    listMarketSizeSubLines.Entities[0]["tss_remainingqty"] = remainingQty;
        //                                    organizationService.Update(listMarketSizeSubLines.Entities[0]);

        //                                }
        //                            }

        //                            //Update Remaining Qty di totalpartConsumpMarketSize
        //                            listTotalPartConsumpMarketSize.Entities[0]["tss_remainingqty"] = remainingQty;
        //                            organizationService.Update(listTotalPartConsumpMarketSize.Entities[0]);
        //                        }

        //                        #endregion

        //                        #region else if Qty potential prospect > Remaining Qty di total Consump part && Remaining Qty != 0
        //                        else if (Convert.ToDecimal(ppsubline.qty) > listTotalPartConsumpMarketSize.Entities[0].GetAttributeValue<int>("tss_remainingqty") && listTotalPartConsumpMarketSize.Entities[0].GetAttributeValue<int>("tss_remainingqty") > 0)

        //                        {
        //                            //insert prospect partline
        //                            ENT_prospectpartline["tss_sourcetype"] = new OptionSetValue(865920001);
        //                            ENT_prospectpartline["tss_pricetype"] = new EntityReference("tss_pricelistpart", oPriceType.Entities[0].Id);
        //                            ENT_prospectpartline["tss_refmarketsize"] = new EntityReference(_DL_tss_potentialprospectpart.EntityName, listPotentialProspectPart.Entities[0].GetAttributeValue<Guid>("tss_potentialprospectpartid"));
        //                            ENT_prospectpartline["tss_quantity"] = Convert.ToDecimal(ppsubline.qty);
        //                            decimal remainingQty = 0;
        //                            remainingQty = Convert.ToDecimal(ppsubline.qty) - Convert.ToDecimal(listTotalPartConsumpMarketSize.Entities[0]["tss_remainingqty"]);

        //                            ENT_prospectpartline["tss_qtymarketsize"] = remainingQty;
        //                            ENT_prospectpartline["tss_priceamount"] = new Money((decimal)oPrice[0] * Convert.ToDecimal(ppsubline.qty));
        //                            //organizationService.Create(ENT_prospectpartline);

        //                            //update total partConsump market size 

        //                            decimal remainingQtyTotalPartConsump = 0;
        //                            remainingQtyTotalPartConsump = Convert.ToDecimal(listTotalPartConsumpMarketSize.Entities[0]["tss_remainingqty"]) - remainingQty;

        //                            listTotalPartConsumpMarketSize.Entities[0]["tss_remainingqty"] = remainingQtyTotalPartConsump;
        //                            organizationService.Update(listTotalPartConsumpMarketSize.Entities[0]);

        //                            if (remainingQtyTotalPartConsump == 0)
        //                            {
        //                                //Update status potential prospect part
        //                                listPotentialProspectPart.Entities[0]["tss_status"] = new OptionSetValue(865920002);
        //                                listPotentialProspectPart.Entities[0]["tss_statusreason"] = new OptionSetValue(865920002);
        //                                organizationService.Update(listPotentialProspectPart.Entities[0]);

        //                                //Update Remaining Qty di Market Size Sub Lines
        //                                if (listMarketSizeSubLines.Entities[0].Attributes.Contains("tss_remainingqty"))
        //                                {
        //                                    listMarketSizeSubLines.Entities[0]["tss_remainingqty"] = remainingQtyTotalPartConsump;
        //                                    organizationService.Update(listMarketSizeSubLines.Entities[0]);
        //                                }

        //                            }

        //                        }

        //                        #endregion

        //                        #region else if Qty potential prospect > Remaining Qty di total Consump part && Remaining Qty = 0
        //                        else if (Convert.ToDecimal(ppsubline.qty) > listTotalPartConsumpMarketSize.Entities[0].GetAttributeValue<int>("tss_remainingqty") && listTotalPartConsumpMarketSize.Entities[0].GetAttributeValue<int>("tss_remainingqty") == 0)

        //                        {
        //                            //insert prospect partline
        //                            ENT_prospectpartline["tss_sourcetype"] = new OptionSetValue(865920001);
        //                            ENT_prospectpartline["tss_pricetype"] = new EntityReference("tss_pricelistpart", oPriceType.Entities[0].Id);
        //                            ENT_prospectpartline["tss_quantity"] = Convert.ToDecimal(ppsubline.qty);
        //                            ENT_prospectpartline["tss_qtymarketsize"] = 0;
        //                            ENT_prospectpartline["tss_priceamount"] = new Money((decimal)oPrice[0] * Convert.ToDecimal(ppsubline.qty));
        //                            //organizationService.Create(ENT_prospectpartline);
        //                        }

        //                        #endregion

        //                        organizationService.Create(ENT_prospectpartline);
        //                    }
        //                    //end todo
        //                }

        //            }
        //            else
        //            {

        //                // foreach (Entity eLines in listPotentialProspectPartLines.Entities)
        //                // {
        //                string[] pplineId = listPotentialProspectPartLines.Entities.Select(x => x.GetAttributeValue<Guid>("tss_potentialprospectpartlinesid").ToString()).ToArray();
        //                FilterExpression fPPSL = new FilterExpression(LogicalOperator.And);
        //                fPPSL.AddCondition("tss_potentialprospectpartlinesref", ConditionOperator.In, pplineId);

        //                QueryExpression qPPSL = new QueryExpression("tss_potentialprospectpartsublines");
        //                qPPSL.Criteria.AddFilter(fPPSL);
        //                qPPSL.ColumnSet = new ColumnSet(true);

        //                EntityCollection listPotentialProspectPartSubLines = _DL_tss_potentialprospectpartsublines.Select(organizationService, qPPSL);

        //                var groupPotentialProspectPartSubLines = (from r in listPotentialProspectPartSubLines.Entities.AsEnumerable()
        //                                                          group r by new
        //                                                          {
        //                                                              partnumberId = (r.GetAttributeValue<EntityReference>("tss_partnumber").Id)
        //                                                          } into g
        //                                                          select new
        //                                                          {
        //                                                              qty = g.Sum(x => x.GetAttributeValue<int>("tss_qty")),
        //                                                              parnumberId = g.Key.partnumberId
        //                                                          }).ToList();


        //                foreach (var ppsubline in groupPotentialProspectPartSubLines)
        //                {
        //                    lineNumber += 10;
        //                    Entity ENT_prospectpartline = new Entity("tss_prospectpartlines");
        //                    ENT_prospectpartline["tss_itemnumber"] = lineNumber;
        //                    ENT_prospectpartline["tss_prospectpartheader"] = new EntityReference("tss_prospectpartheader", prospectPartHeaderID);
        //                    ENT_prospectpartline["tss_partnumber"] = new EntityReference("trs_masterpart", ppsubline.parnumberId); // enSubLines.GetAttributeValue<EntityReference>("tss_partnumber");

        //                    Entity oPN = organizationService.Retrieve("trs_masterpart", ppsubline.parnumberId, new ColumnSet(true));
        //                    ENT_prospectpartline["tss_partdescription"] = oPN.GetAttributeValue<String>("trs_partdescription");

        //                    #region GET PRICE
        //                    List<object> oPrice = GetSparePartPriceMaster(organizationService, ppsubline.parnumberId);

        //                    ENT_prospectpartline["tss_price"] = new Money((decimal)oPrice[0]);
        //                    ENT_prospectpartline["tss_minimumprice"] = new Money((decimal)oPrice[1]);
        //                    ENT_prospectpartline["transactioncurrencyid"] = new EntityReference("transactioncurrency", (Guid)oPrice[2]);
        //                    ENT_prospectpartline["tss_qtymarketsize"] = 0;
        //                    //ENT_prospectpartline["tss_priceamount"] = new Money(0);
        //                    ENT_prospectpartline["tss_discountamount"] = new Money(0);

        //                    ENT_prospectpartline["tss_discountpercent"] = Convert.ToDecimal(0);
        //                    ENT_prospectpartline["tss_finalprice"] = new Money(0);
        //                    #endregion

        //                    #region FIND UNIT GROUP
        //                    FilterExpression fUnitGroup = new FilterExpression(LogicalOperator.And);
        //                    fUnitGroup.AddCondition("new_populationid", ConditionOperator.Equal, listPotentialProspectPart.Entities[0].GetAttributeValue<EntityReference>("tss_serialnumber").Id);

        //                    QueryExpression qUnitGroup = new QueryExpression("new_population");
        //                    qUnitGroup.Criteria.AddFilter(fUnitGroup);
        //                    qUnitGroup.ColumnSet = new ColumnSet(true);

        //                    EntityCollection oUnitGroup = _DL_population.Select(organizationService, qUnitGroup);

        //                    if (oUnitGroup.Entities.Count() > 0)
        //                    {
        //                        fUnitGroup = new FilterExpression(LogicalOperator.And);
        //                        fUnitGroup.AddCondition("name", ConditionOperator.Equal, oUnitGroup.Entities[0].GetAttributeValue<string>("new_productitem"));

        //                        qUnitGroup = new QueryExpression("uomschedule");
        //                        qUnitGroup.Criteria.AddFilter(fUnitGroup);
        //                        qUnitGroup.ColumnSet = new ColumnSet(true);

        //                        EntityCollection ounit = organizationService.RetrieveMultiple(qUnitGroup);
        //                        ENT_prospectpartline["tss_unitgroup"] = new EntityReference("uomschedule", ounit.Entities[0].Id);
        //                    }
        //                    #endregion

        //                    #region get PartType
        //                    FilterExpression fPT = new FilterExpression(LogicalOperator.And);
        //                    fPT.AddCondition("trs_masterpartid", ConditionOperator.Equal, ppsubline.parnumberId);

        //                    QueryExpression qPT = new QueryExpression("trs_masterpart");
        //                    qPT.Criteria.AddFilter(fPT);
        //                    qPT.ColumnSet = new ColumnSet(true);

        //                    EntityCollection listPartMaster = _DL_partMaster.Select(organizationService, qPT);

        //                    errModul = "Get Part Master failed";
        //                    ENT_prospectpartline["tss_parttype"] = new OptionSetValue(listPartMaster.Entities[0].GetAttributeValue<OptionSetValue>("tss_parttype").Value); // listPartMaster.Entities[0].GetAttributeValue<OptionSetValue>("tss_parttype").Value;
        //                    #endregion

        //                    #region look up to market size 
        //                    FilterExpression fMS = new FilterExpression(LogicalOperator.And);
        //                    fMS.AddCondition("tss_pss", ConditionOperator.Equal, listPotentialProspectPart.Entities[0].GetAttributeValue<EntityReference>("tss_pss").Id);
        //                    fMS.AddCondition("tss_customer", ConditionOperator.Equal, listPotentialProspectPart.Entities[0].GetAttributeValue<EntityReference>("tss_customer").Id);
        //                    //2018.09.17 - start = lessequal & end = greaterequal
        //                    //fMS.AddCondition("tss_msperiodend", ConditionOperator.LessEqual, DateTime.Now);
        //                    //fMS.AddCondition("tss_msperiodstart", ConditionOperator.GreaterEqual, DateTime.Now);

        //                    //fMS.AddCondition("tss_msperiodend", ConditionOperator.GreaterEqual, DateTime.Now);
        //                    //fMS.AddCondition("tss_msperiodstart", ConditionOperator.LessEqual, DateTime.Now);

        //                    fMS.AddCondition("tss_serialnumber", ConditionOperator.Equal, listPotentialProspectPart.Entities[0].GetAttributeValue<EntityReference>("tss_serialnumber").Id);

        //                    QueryExpression qMS = new QueryExpression("tss_mastermarketsize");
        //                    qMS.Criteria.AddFilter(fMS);
        //                    qMS.ColumnSet = new ColumnSet(true);

        //                    errModul = "Get Master market size failed";
        //                    //EntityCollection listMarketSize = _DL_tss_marketmarketsize.Select(organizationService, qMS);
        //                    EntityCollection listMarketSize = _DL_tss_marketmarketsize.Select(organizationService, qMS);
        //                    #endregion

        //                    #region look up to market size lines

        //                    FilterExpression fMSL = new FilterExpression(LogicalOperator.And);
        //                    fMSL.AddCondition("tss_mastermarketsizeref", ConditionOperator.Equal, listMarketSize.Entities[0].Id);

        //                    QueryExpression qMSL = new QueryExpression("tss_mastermarketsizelines");
        //                    qMSL.Criteria.AddFilter(fMSL);
        //                    qMSL.ColumnSet = new ColumnSet(true);
        //                    errModul = "Get Master market size line failed";

        //                    EntityCollection listMarketSizeLines = _DL_tss_marketmarketsizelines.Select(organizationService, qMSL);

        //                    #endregion

        //                    if (listMarketSizeLines.Entities.Count > 0)
        //                    {
        //                        #region look up to market size Sublines

        //                        FilterExpression fMSSL = new FilterExpression(LogicalOperator.And);
        //                        fMSSL.AddCondition("tss_mastermslinesref", ConditionOperator.Equal, listMarketSizeLines.Entities[0].Id);// listMarketSizeLines.Entities[0].GetAttributeValue<EntityReference>("tss_mastermarketsizelinesid").Id );

        //                        QueryExpression qMSSL = new QueryExpression("tss_mastermarketsizesublines");
        //                        qMSL.Criteria.AddFilter(fMSSL);
        //                        qMSL.ColumnSet = new ColumnSet(true);

        //                        errModul = "Get Master market size sub line failed";
        //                        EntityCollection listMarketSizeSubLines = _DL_tss_marketmarketsizesublines.Select(organizationService, qMSSL);

        //                        #endregion

        //                        #region look up to Total part consump market size

        //                        FilterExpression fTCMS = new FilterExpression(LogicalOperator.And);
        //                        //fTCMS.AddCondition("tss_marketsizeid", ConditionOperator.Equal, listMarketSize.Entities[0].Id);
        //                        fTCMS.AddCondition("tss_marketsizeid", ConditionOperator.Equal, listPotentialProspectPart.Entities[0].GetAttributeValue<EntityReference>("tss_marketsizeid").Id);

        //                        QueryExpression qTCMS = new QueryExpression("tss_totalpartconsumpmarketsize");
        //                        qTCMS.Criteria.AddFilter(fTCMS);
        //                        qTCMS.ColumnSet = new ColumnSet(true);

        //                        errModul = "Get total part consump market size failed";
        //                        EntityCollection listTotalPartConsumpMarketSize = _DL_tss_totalpartconsumpmarketsize.Select(organizationService, qTCMS);


        //                        #region FIND PRICE TYPE
        //                        FilterExpression fPriceType = new FilterExpression(LogicalOperator.And);
        //                        fPriceType.AddCondition("tss_type", ConditionOperator.Equal, 865920001);

        //                        QueryExpression qPriceType = new QueryExpression("tss_pricelistpart");
        //                        qPriceType.Criteria.AddFilter(fPriceType);
        //                        qPriceType.ColumnSet = new ColumnSet(true);

        //                        EntityCollection oPriceType = _DL_tss_pricelistpart.Select(organizationService, qPriceType);

        //                        #endregion

        //                        #endregion

        //                        errModul = "Insert Prospect Part Line failed";

        //                        if (listTotalPartConsumpMarketSize.Entities.Count > 0)
        //                        {
        //                            #region if Qty potential prospect <= remaining Qty di total Consump
        //                            if (Convert.ToDecimal(ppsubline.qty) <= listTotalPartConsumpMarketSize.Entities[0].GetAttributeValue<int>("tss_remainingqty"))
        //                            {
        //                                //insert prospect partline
        //                                ENT_prospectpartline["tss_sourcetype"] = new OptionSetValue(865920001);
        //                                ENT_prospectpartline["tss_pricetype"] = new EntityReference("tss_pricelistpart", oPriceType.Entities[0].Id);
        //                                ENT_prospectpartline["tss_refmarketsize"] = new EntityReference(_DL_tss_potentialprospectpart.EntityName, listPotentialProspectPart.Entities[0].GetAttributeValue<Guid>("tss_potentialprospectpartid"));
        //                                ENT_prospectpartline["tss_quantity"] = Convert.ToDecimal(ppsubline.qty);
        //                                ENT_prospectpartline["tss_qtymarketsize"] = Convert.ToDecimal(ppsubline.qty);
        //                                ENT_prospectpartline["tss_priceamount"] = new Money((decimal)oPrice[0] * Convert.ToDecimal(ppsubline.qty));
        //                                //organizationService.Create(ENT_prospectpartline);

        //                                //update total partConsump market size 

        //                                decimal remainingQty = 0;
        //                                remainingQty = Convert.ToDecimal(listTotalPartConsumpMarketSize.Entities[0]["tss_remainingqty"]) - Convert.ToDecimal(ppsubline.qty);



        //                                if (remainingQty == 0)
        //                                {
        //                                    //Update status potential prospect part
        //                                    listPotentialProspectPart.Entities[0]["tss_status"] = new OptionSetValue(865920002);
        //                                    listPotentialProspectPart.Entities[0]["tss_statusreason"] = new OptionSetValue(865920002);
        //                                    organizationService.Update(listPotentialProspectPart.Entities[0]);

        //                                    //Update Remaining Qty di Market Size Sub Lines
        //                                    if (listMarketSizeSubLines.Entities[0].Attributes.Contains("tss_remainingqty"))
        //                                    {
        //                                        listMarketSizeSubLines.Entities[0]["tss_remainingqty"] = remainingQty;
        //                                        organizationService.Update(listMarketSizeSubLines.Entities[0]);
        //                                    }
        //                                }

        //                                //Update Remaining Qty di totalpartConsumpMarketSize
        //                                listTotalPartConsumpMarketSize.Entities[0]["tss_remainingqty"] = remainingQty;
        //                                organizationService.Update(listTotalPartConsumpMarketSize.Entities[0]);
        //                            }

        //                            #endregion

        //                            #region else if Qty potential prospect > Remaining Qty di total Consump part && Remaining Qty != 0
        //                            else if (Convert.ToDecimal(ppsubline.qty) > listTotalPartConsumpMarketSize.Entities[0].GetAttributeValue<int>("tss_remainingqty") && listTotalPartConsumpMarketSize.Entities[0].GetAttributeValue<int>("tss_remainingqty") > 0)

        //                            {
        //                                //insert prospect partline
        //                                ENT_prospectpartline["tss_sourcetype"] = new OptionSetValue(865920001);
        //                                ENT_prospectpartline["tss_pricetype"] = new EntityReference("tss_pricelistpart", oPriceType.Entities[0].Id);
        //                                ENT_prospectpartline["tss_refmarketsize"] = new EntityReference(_DL_tss_potentialprospectpart.EntityName, listPotentialProspectPart.Entities[0].GetAttributeValue<Guid>("tss_potentialprospectpartid"));
        //                                ENT_prospectpartline["tss_quantity"] = Convert.ToDecimal(ppsubline.qty);
        //                                decimal remainingQty = 0;
        //                                remainingQty = Convert.ToDecimal(ppsubline.qty) - Convert.ToDecimal(listTotalPartConsumpMarketSize.Entities[0]["tss_remainingqty"]);

        //                                ENT_prospectpartline["tss_qtymarketsize"] = remainingQty;
        //                                ENT_prospectpartline["tss_priceamount"] = new Money((decimal)oPrice[0] * Convert.ToDecimal(ppsubline.qty));
        //                                // organizationService.Create(ENT_prospectpartline);

        //                                //update total partConsump market size 

        //                                decimal remainingQtyTotalPartConsump = 0;
        //                                remainingQtyTotalPartConsump = Convert.ToDecimal(listTotalPartConsumpMarketSize.Entities[0]["tss_remainingqty"]) - remainingQty;

        //                                listTotalPartConsumpMarketSize.Entities[0]["tss_remainingqty"] = remainingQtyTotalPartConsump;
        //                                organizationService.Update(listTotalPartConsumpMarketSize.Entities[0]);

        //                                if (remainingQtyTotalPartConsump == 0)
        //                                {
        //                                    //Update status potential prospect part
        //                                    listPotentialProspectPart.Entities[0]["tss_status"] = new OptionSetValue(865920002);
        //                                    listPotentialProspectPart.Entities[0]["tss_statusreason"] = new OptionSetValue(865920002);
        //                                    organizationService.Update(listPotentialProspectPart.Entities[0]);

        //                                    //Update Remaining Qty di Market Size Sub Lines
        //                                    if (listMarketSizeSubLines.Entities[0].Attributes.Contains("tss_remainingqty"))
        //                                    {
        //                                        listMarketSizeSubLines.Entities[0]["tss_remainingqty"] = remainingQtyTotalPartConsump;
        //                                        organizationService.Update(listMarketSizeSubLines.Entities[0]);
        //                                    }
        //                                }

        //                            }

        //                            #endregion

        //                            #region else if Qty potential prospect > Remaining Qty di total Consump part && Remaining Qty = 0
        //                            else if (Convert.ToDecimal(ppsubline.qty) > listTotalPartConsumpMarketSize.Entities[0].GetAttributeValue<int>("tss_remainingqty") && listTotalPartConsumpMarketSize.Entities[0].GetAttributeValue<int>("tss_remainingqty") == 0)

        //                            {
        //                                //insert prospect partline
        //                                ENT_prospectpartline["tss_sourcetype"] = new OptionSetValue(865920001);
        //                                ENT_prospectpartline["tss_pricetype"] = new EntityReference("tss_pricelistpart", oPriceType.Entities[0].Id);
        //                                ENT_prospectpartline["tss_quantity"] = Convert.ToDecimal(ppsubline.qty);
        //                                ENT_prospectpartline["tss_qtymarketsize"] = 0;
        //                                ENT_prospectpartline["tss_priceamount"] = new Money((decimal)oPrice[0] * Convert.ToDecimal(ppsubline.qty));
        //                                //organizationService.Create(ENT_prospectpartline);
        //                            }

        //                            #endregion
        //                            organizationService.Create(ENT_prospectpartline);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        //Todo process
        //                        #region look up to market size Sublines

        //                        FilterExpression fMSSL = new FilterExpression(LogicalOperator.And);
        //                        fMSSL.AddCondition("tss_mastermarketsizeid", ConditionOperator.Equal, listMarketSizeLines.Entities[0].Id);// listMarketSizeLines.Entities[0].GetAttributeValue<EntityReference>("tss_mastermarketsizelinesid").Id );

        //                        QueryExpression qMSSL = new QueryExpression("tss_mastermarketsizesublines");
        //                        qMSL.Criteria.AddFilter(fMSSL);
        //                        qMSL.ColumnSet = new ColumnSet(true);

        //                        errModul = "Get Master market size sub line failed";
        //                        EntityCollection listMarketSizeSubLines = _DL_tss_marketmarketsizesublines.Select(organizationService, qMSSL);

        //                        #endregion

        //                        #region look up to Total part consump market size

        //                        FilterExpression fTCMS = new FilterExpression(LogicalOperator.And);
        //                        //fTCMS.AddCondition("tss_marketsizeid", ConditionOperator.Equal, listMarketSize.Entities[0].Id);
        //                        fTCMS.AddCondition("tss_marketsizeid", ConditionOperator.Equal, listPotentialProspectPart.Entities[0].GetAttributeValue<EntityReference>("tss_marketsizeid").Id);

        //                        QueryExpression qTCMS = new QueryExpression("tss_totalpartconsumpmarketsize");
        //                        qTCMS.Criteria.AddFilter(fTCMS);
        //                        qTCMS.ColumnSet = new ColumnSet(true);

        //                        errModul = "Get total part consump market size failed";
        //                        EntityCollection listTotalPartConsumpMarketSize = _DL_tss_totalpartconsumpmarketsize.Select(organizationService, qTCMS);


        //                        #region FIND PRICE TYPE
        //                        FilterExpression fPriceType = new FilterExpression(LogicalOperator.And);
        //                        fPriceType.AddCondition("tss_type", ConditionOperator.Equal, 865920001);

        //                        QueryExpression qPriceType = new QueryExpression("tss_pricelistpart");
        //                        qPriceType.Criteria.AddFilter(fPriceType);
        //                        qPriceType.ColumnSet = new ColumnSet(true);

        //                        EntityCollection oPriceType = _DL_tss_pricelistpart.Select(organizationService, qPriceType);

        //                        #endregion

        //                        #endregion

        //                        errModul = "Insert Prospect Part Line failed";

        //                        if (listTotalPartConsumpMarketSize.Entities.Count > 0)
        //                        {
        //                            #region if Qty potential prospect <= remaining Qty di total Consump
        //                            if (Convert.ToDecimal(ppsubline.qty) <= listTotalPartConsumpMarketSize.Entities[0].GetAttributeValue<int>("tss_remainingqty"))
        //                            {
        //                                //insert prospect partline
        //                                ENT_prospectpartline["tss_sourcetype"] = new OptionSetValue(865920001);
        //                                ENT_prospectpartline["tss_pricetype"] = new EntityReference("tss_pricelistpart", oPriceType.Entities[0].Id);
        //                                ENT_prospectpartline["tss_refmarketsize"] = new EntityReference(_DL_tss_potentialprospectpart.EntityName, listPotentialProspectPart.Entities[0].GetAttributeValue<Guid>("tss_potentialprospectpartid"));
        //                                ENT_prospectpartline["tss_quantity"] = Convert.ToDecimal(ppsubline.qty);
        //                                ENT_prospectpartline["tss_qtymarketsize"] = Convert.ToDecimal(ppsubline.qty);
        //                                ENT_prospectpartline["tss_priceamount"] = new Money((decimal)oPrice[0] * Convert.ToDecimal(ppsubline.qty));
        //                                //organizationService.Create(ENT_prospectpartline);

        //                                //update total partConsump market size 

        //                                decimal remainingQty = 0;
        //                                remainingQty = Convert.ToDecimal(listTotalPartConsumpMarketSize.Entities[0]["tss_remainingqty"]) - Convert.ToDecimal(ppsubline.qty);



        //                                if (remainingQty == 0)
        //                                {
        //                                    //Update status potential prospect part
        //                                    listPotentialProspectPart.Entities[0]["tss_status"] = new OptionSetValue(865920002);
        //                                    listPotentialProspectPart.Entities[0]["tss_statusreason"] = new OptionSetValue(865920002);
        //                                    organizationService.Update(listPotentialProspectPart.Entities[0]);

        //                                    //Update Remaining Qty di Market Size Sub Lines
        //                                    if (listMarketSizeSubLines.Entities[0].Attributes.Contains("tss_remainingqty"))
        //                                    {
        //                                        listMarketSizeSubLines.Entities[0]["tss_remainingqty"] = remainingQty;
        //                                        organizationService.Update(listMarketSizeSubLines.Entities[0]);
        //                                    }
        //                                }

        //                                //Update Remaining Qty di totalpartConsumpMarketSize
        //                                listTotalPartConsumpMarketSize.Entities[0]["tss_remainingqty"] = remainingQty;
        //                                organizationService.Update(listTotalPartConsumpMarketSize.Entities[0]);
        //                            }

        //                            #endregion

        //                            #region else if Qty potential prospect > Remaining Qty di total Consump part && Remaining Qty != 0
        //                            else if (Convert.ToDecimal(ppsubline.qty) > listTotalPartConsumpMarketSize.Entities[0].GetAttributeValue<int>("tss_remainingqty") && listTotalPartConsumpMarketSize.Entities[0].GetAttributeValue<int>("tss_remainingqty") > 0)

        //                            {
        //                                //insert prospect partline
        //                                ENT_prospectpartline["tss_sourcetype"] = new OptionSetValue(865920001);
        //                                ENT_prospectpartline["tss_pricetype"] = new EntityReference("tss_pricelistpart", oPriceType.Entities[0].Id);
        //                                ENT_prospectpartline["tss_refmarketsize"] = new EntityReference(_DL_tss_potentialprospectpart.EntityName, listPotentialProspectPart.Entities[0].GetAttributeValue<Guid>("tss_potentialprospectpartid"));
        //                                ENT_prospectpartline["tss_quantity"] = Convert.ToDecimal(ppsubline.qty);
        //                                decimal remainingQty = 0;
        //                                remainingQty = Convert.ToDecimal(ppsubline.qty) - Convert.ToDecimal(listTotalPartConsumpMarketSize.Entities[0]["tss_remainingqty"]);

        //                                ENT_prospectpartline["tss_qtymarketsize"] = remainingQty;
        //                                ENT_prospectpartline["tss_priceamount"] = new Money((decimal)oPrice[0] * Convert.ToDecimal(ppsubline.qty));
        //                                // organizationService.Create(ENT_prospectpartline);

        //                                //update total partConsump market size 

        //                                decimal remainingQtyTotalPartConsump = 0;
        //                                remainingQtyTotalPartConsump = Convert.ToDecimal(listTotalPartConsumpMarketSize.Entities[0]["tss_remainingqty"]) - remainingQty;

        //                                listTotalPartConsumpMarketSize.Entities[0]["tss_remainingqty"] = remainingQtyTotalPartConsump;
        //                                organizationService.Update(listTotalPartConsumpMarketSize.Entities[0]);

        //                                if (remainingQtyTotalPartConsump == 0)
        //                                {
        //                                    //Update status potential prospect part
        //                                    listPotentialProspectPart.Entities[0]["tss_status"] = new OptionSetValue(865920002);
        //                                    listPotentialProspectPart.Entities[0]["tss_statusreason"] = new OptionSetValue(865920002);
        //                                    organizationService.Update(listPotentialProspectPart.Entities[0]);

        //                                    //Update Remaining Qty di Market Size Sub Lines
        //                                    if (listMarketSizeSubLines.Entities[0].Attributes.Contains("tss_remainingqty"))
        //                                    {
        //                                        listMarketSizeSubLines.Entities[0]["tss_remainingqty"] = remainingQtyTotalPartConsump;
        //                                        organizationService.Update(listMarketSizeSubLines.Entities[0]);
        //                                    }
        //                                }

        //                            }

        //                            #endregion

        //                            #region else if Qty potential prospect > Remaining Qty di total Consump part && Remaining Qty = 0
        //                            else if (Convert.ToDecimal(ppsubline.qty) > listTotalPartConsumpMarketSize.Entities[0].GetAttributeValue<int>("tss_remainingqty") && listTotalPartConsumpMarketSize.Entities[0].GetAttributeValue<int>("tss_remainingqty") == 0)

        //                            {
        //                                //insert prospect partline
        //                                ENT_prospectpartline["tss_sourcetype"] = new OptionSetValue(865920001);
        //                                ENT_prospectpartline["tss_pricetype"] = new EntityReference("tss_pricelistpart", oPriceType.Entities[0].Id);
        //                                ENT_prospectpartline["tss_quantity"] = Convert.ToDecimal(ppsubline.qty);
        //                                ENT_prospectpartline["tss_qtymarketsize"] = 0;
        //                                ENT_prospectpartline["tss_priceamount"] = new Money((decimal)oPrice[0] * Convert.ToDecimal(ppsubline.qty));
        //                                //organizationService.Create(ENT_prospectpartline);
        //                            }

        //                            #endregion

        //                            organizationService.Create(ENT_prospectpartline);
        //                        }
        //                        //end todo
        //                    }
        //                }
        //                //} hide end foreach
        //            }

        //            #endregion
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new InvalidWorkflowException(errModul + ". " + e.Message.ToString());
        //    }
        //}







        //public void GenerateProspectPartFromPotentialProspectPartLine_OnClick(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        //{
        //    Entity en_CurrPotentialProspectPartLine = _DL_tss_potentialprospectpartlines.Select(organizationService, context.PrimaryEntityId);
        //    string errModul = string.Empty;

        //    #region get Potential Prospect Part Header
        //    Entity en_CurrPotentialProspectPart = _DL_tss_potentialprospectpart.Select(organizationService, en_CurrPotentialProspectPartLine.GetAttributeValue<EntityReference>("tss_potentialprospectpart").Id);

        //    #endregion

        //    #region get total Amount

        //    int totalQty = 0;
        //    decimal totalPrice = 0;
        //    decimal totalAmount = 0;

        //    FilterExpression fPPSL = new FilterExpression(LogicalOperator.And);
        //    fPPSL.AddCondition("tss_potentialprospectpartlinesref", ConditionOperator.Equal, en_CurrPotentialProspectPartLine["tss_potentialprospectpartlinesid"].ToString());

        //    QueryExpression qPPSL = new QueryExpression("tss_potentialprospectpartsublines");
        //    qPPSL.Criteria.AddFilter(fPPSL);
        //    qPPSL.ColumnSet = new ColumnSet(true);

        //    EntityCollection listPotentialProspectPartSubLines = _DL_tss_potentialprospectpartsublines.Select(organizationService, qPPSL);

        //    foreach (Entity enSubLines in listPotentialProspectPartSubLines.Entities)
        //    {
        //        totalQty += Convert.ToInt32(enSubLines["tss_qty"]);
        //        totalPrice += Convert.ToDecimal(enSubLines.GetAttributeValue<Money>("tss_price").Value);
        //    }


        //    totalAmount = totalQty * totalPrice;
        //    #endregion

        //    #region get latest est Close Date

        //    DateTime estCloseDate = new DateTime(1900, 1, 1);

        //    if (!String.IsNullOrEmpty(en_CurrPotentialProspectPartLine.GetAttributeValue<DateTime>("tss_estclosedate").ToString()))
        //    {
        //        if (en_CurrPotentialProspectPartLine.GetAttributeValue<DateTime>("tss_estclosedate") > estCloseDate)
        //        {
        //            estCloseDate = en_CurrPotentialProspectPartLine.GetAttributeValue<DateTime>("tss_estclosedate");
        //        }
        //    }


        //    #endregion

        //    #region get CompanyName

        //    FilterExpression fC = new FilterExpression(LogicalOperator.And);
        //    fC.AddCondition("accountid", ConditionOperator.Equal, en_CurrPotentialProspectPart.GetAttributeValue<EntityReference>("tss_customer").Id);

        //    QueryExpression qC = new QueryExpression("account");
        //    qC.Criteria.AddFilter(fC);
        //    qC.ColumnSet = new ColumnSet(true);

        //    EntityCollection listCustomer = _DL_account.Select(organizationService, qC);

        //    #endregion

        //    #region get BranchCode
        //    string branchCode = string.Empty;
        //    Entity eBusinessUnit = _DL_businessunit.Select(organizationService, context.BusinessUnitId);
        //    //throw new Exception("Test : " + eBusinessUnit.Attributes["trs_branchcode"].ToString());
        //    if (eBusinessUnit.Attributes.Contains("trs_branchcode"))
        //    {
        //        branchCode = eBusinessUnit.Attributes["trs_branchcode"].ToString();
        //    }
        //    #endregion

        //    #region generate prospectpartHeader

        //    Entity ENT_prospectpartheader = new Entity("tss_prospectpartheader");
        //    ENT_prospectpartheader["tss_sourcetype"] = new OptionSetValue(865920001);
        //    ENT_prospectpartheader["tss_estimationclosedate"] = estCloseDate;
        //    ENT_prospectpartheader["tss_pipelinephase"] = new OptionSetValue(865920000);
        //    ENT_prospectpartheader["tss_statusreason"] = new OptionSetValue(865920000);
        //    ENT_prospectpartheader["tss_customer"] = new EntityReference("account", en_CurrPotentialProspectPart.GetAttributeValue<EntityReference>("tss_customer").Id);
        //    if (listCustomer.Entities[0].GetAttributeValue<EntityReference>("primarycontactid") != null)
        //    {
        //        ENT_prospectpartheader["tss_contact"] = listCustomer.Entities[0].GetAttributeValue<EntityReference>("primarycontactid");
        //    }

        //    ENT_prospectpartheader["tss_refmarketsize"] =
        //        new EntityReference(_DL_tss_potentialprospectpart.EntityName, en_CurrPotentialProspectPart.GetAttributeValue<Guid>("tss_potentialprospectpartid"));
        //    ENT_prospectpartheader["tss_currency"] = new EntityReference("transactioncurrency", new Guid("7459618D-4EE9-E111-9AA2-544249894792"));
        //    ENT_prospectpartheader["ownerid"] = new EntityReference("systemuser", context.UserId);
        //    ENT_prospectpartheader["tss_pss"] = new EntityReference("systemuser", en_CurrPotentialProspectPart.GetAttributeValue<EntityReference>("tss_pss").Id);
        //    ENT_prospectpartheader["tss_branch"] = new EntityReference(_DL_businessunit.EntityName, context.BusinessUnitId);
        //    ENT_prospectpartheader["tss_totalamount"] = totalAmount;
        //    ENT_prospectpartheader["tss_topic"] = "Prospect From MS - " +
        //                en_CurrPotentialProspectPart.GetAttributeValue<string>("tss_serialnumberpopulation") +
        //                " - " +
        //                DateTime.Now.ToLocalTime().Year.ToString() +
        //                " by Part Number";


        //    //AssignRequest assignRequest = new AssignRequest
        //    //{
        //    //    Assignee = new EntityReference("systemuser", listPotentialProspectPart.Entities[0].GetAttributeValue<EntityReference>("tss_pss").Id),
        //    //    Target = new EntityReference("tss_prospectpartheader", ENT_prospectpartheader.Id)
        //    //};
        //    //organizationService.Execute(assignRequest);
        //    errModul = "Generate Prospect Header failed";
        //    Guid prospectPartHeaderID = organizationService.Create(ENT_prospectpartheader);
        //    #endregion

        //    #region generate prospectpartLine
        //    int lineNumber = 0;

        //    FilterExpression fPPSLA = new FilterExpression(LogicalOperator.And);
        //    fPPSLA.AddCondition("tss_potentialprospectpartlinesref", ConditionOperator.Equal, en_CurrPotentialProspectPartLine["tss_potentialprospectpartlinesid"].ToString());

        //    QueryExpression qPPSLA = new QueryExpression("tss_potentialprospectpartsublines");
        //    qPPSLA.Criteria.AddFilter(fPPSLA);
        //    qPPSLA.ColumnSet = new ColumnSet(true);

        //    EntityCollection listPotentialProspectPartSubLinesA = _DL_tss_potentialprospectpartsublines.Select(organizationService, qPPSLA);

        //    foreach (Entity enSubLines in listPotentialProspectPartSubLinesA.Entities)
        //    {
        //        lineNumber += 10;
        //        Entity ENT_prospectpartline = new Entity("tss_prospectpartlines");
        //        ENT_prospectpartline["tss_itemnumber"] = lineNumber;
        //        ENT_prospectpartline["tss_prospectpartheader"] = new EntityReference("tss_prospectpartheader", prospectPartHeaderID);
        //        ENT_prospectpartline["tss_partnumber"] = new EntityReference("trs_masterpart", enSubLines.GetAttributeValue<EntityReference>("tss_partnumber").Id);

        //        Entity oPN = organizationService.Retrieve("trs_masterpart", enSubLines.GetAttributeValue<EntityReference>("tss_partnumber").Id, new ColumnSet(true));
        //        ENT_prospectpartline["tss_partdescription"] = oPN.GetAttributeValue<String>("trs_partdescription");

        //        #region GET PRICE
        //        List<object> oPrice = GetSparePartPriceMaster(organizationService, enSubLines.GetAttributeValue<EntityReference>("tss_partnumber").Id);

        //        ENT_prospectpartline["tss_price"] = new Money((decimal)oPrice[0]);
        //        ENT_prospectpartline["tss_minimumprice"] = new Money((decimal)oPrice[1]);
        //        ENT_prospectpartline["transactioncurrencyid"] = new EntityReference("transactioncurrency", (Guid)oPrice[2]);
        //        ENT_prospectpartline["tss_qtymarketsize"] = 0;
        //        //ENT_prospectpartline["tss_priceamount"] = new Money(0);
        //        ENT_prospectpartline["tss_discountamount"] = new Money(0);

        //        ENT_prospectpartline["tss_discountpercent"] = Convert.ToDecimal(0);
        //        ENT_prospectpartline["tss_finalprice"] = new Money(0);
        //        #endregion

        //        #region FIND UNIT GROUP
        //        FilterExpression fUnitGroup = new FilterExpression(LogicalOperator.And);
        //        fUnitGroup.AddCondition("new_populationid", ConditionOperator.Equal, en_CurrPotentialProspectPart.GetAttributeValue<EntityReference>("tss_serialnumber").Id);

        //        QueryExpression qUnitGroup = new QueryExpression("new_population");
        //        qUnitGroup.Criteria.AddFilter(fUnitGroup);
        //        qUnitGroup.ColumnSet = new ColumnSet(true);

        //        EntityCollection oUnitGroup = _DL_population.Select(organizationService, qUnitGroup);

        //        if (oUnitGroup.Entities.Count() > 0)
        //        {
        //            fUnitGroup = new FilterExpression(LogicalOperator.And);
        //            fUnitGroup.AddCondition("name", ConditionOperator.Equal, oUnitGroup.Entities[0].GetAttributeValue<string>("new_productitem"));

        //            qUnitGroup = new QueryExpression("uomschedule");
        //            qUnitGroup.Criteria.AddFilter(fUnitGroup);
        //            qUnitGroup.ColumnSet = new ColumnSet(true);

        //            EntityCollection ounit = organizationService.RetrieveMultiple(qUnitGroup);
        //            ENT_prospectpartline["tss_unitgroup"] = new EntityReference("uomschedule", ounit.Entities[0].Id);

        //        }
        //        #endregion

        //        #region get PartType
        //        FilterExpression fPT = new FilterExpression(LogicalOperator.And);
        //        fPT.AddCondition("trs_masterpartid", ConditionOperator.Equal, enSubLines.GetAttributeValue<EntityReference>("tss_partnumber").Id);

        //        QueryExpression qPT = new QueryExpression("trs_masterpart");
        //        qPT.Criteria.AddFilter(fPT);
        //        qPT.ColumnSet = new ColumnSet(true);

        //        EntityCollection listPartMaster = _DL_partMaster.Select(organizationService, qPT);

        //        errModul = "Get Part Master failed";
        //        ENT_prospectpartline["tss_parttype"] = new OptionSetValue(listPartMaster.Entities[0].GetAttributeValue<OptionSetValue>("tss_parttype").Value);
        //        #endregion

        //        #region look up to market size 
        //        FilterExpression fMS = new FilterExpression(LogicalOperator.And);
        //        fMS.AddCondition("tss_pss", ConditionOperator.Equal, en_CurrPotentialProspectPart.GetAttributeValue<EntityReference>("tss_pss").Id);
        //        fMS.AddCondition("tss_customer", ConditionOperator.Equal, en_CurrPotentialProspectPart.GetAttributeValue<EntityReference>("tss_customer").Id);
        //        fMS.AddCondition("tss_msperiodend", ConditionOperator.GreaterEqual, DateTime.Now);
        //        fMS.AddCondition("tss_msperiodstart", ConditionOperator.LessEqual, DateTime.Now);

        //        QueryExpression qMS = new QueryExpression("tss_mastermarketsize");
        //        qMS.Criteria.AddFilter(fMS);
        //        qMS.ColumnSet = new ColumnSet(true);

        //        errModul = "Get Master market size failed";
        //        EntityCollection listMarketSize = _DL_tss_marketmarketsize.Select(organizationService, qMS);
        //        #endregion

        //        #region look up to market size lines

        //        FilterExpression fMSL = new FilterExpression(LogicalOperator.And);
        //        fMSL.AddCondition("tss_mastermarketsizeref", ConditionOperator.Equal, listMarketSize.Entities[0].GetAttributeValue<Guid>("tss_mastermarketsizeid"));

        //        QueryExpression qMSL = new QueryExpression("tss_mastermarketsizelines");
        //        qMSL.Criteria.AddFilter(fMSL);
        //        qMSL.ColumnSet = new ColumnSet(true);
        //        errModul = "Get Master market size line failed";

        //        EntityCollection listMarketSizeLines = _DL_tss_marketmarketsizelines.Select(organizationService, qMSL);

        //        #endregion

        //        #region look up to market size Sublines

        //        FilterExpression fMSSL = new FilterExpression(LogicalOperator.And);
        //        fMSSL.AddCondition("tss_mastermslinesref", ConditionOperator.Equal, listMarketSizeLines.Entities[0].GetAttributeValue<Guid>("tss_mastermarketsizelinesid"));

        //        QueryExpression qMSSL = new QueryExpression("tss_mastermarketsizesublines");
        //        qMSL.Criteria.AddFilter(fMSSL);
        //        qMSL.ColumnSet = new ColumnSet(true);

        //        errModul = "Get Master market size sub line failed";
        //        EntityCollection listMarketSizeSubLines = _DL_tss_marketmarketsizesublines.Select(organizationService, qMSSL);

        //        if (listMarketSizeSubLines.Entities.Count == 0)
        //        {
        //            fMSSL = new FilterExpression(LogicalOperator.And);
        //            fMSSL.AddCondition("tss_mastermarketsizeid", ConditionOperator.Equal, listMarketSize.Entities[0].Id);// listMarketSizeLines.Entities[0].GetAttributeValue<EntityReference>("tss_mastermarketsizelinesid").Id );

        //            qMSSL = new QueryExpression("tss_mastermarketsizesublines");
        //            qMSSL.Criteria.AddFilter(fMSSL);
        //            qMSSL.ColumnSet = new ColumnSet(true);

        //            errModul = "Get Master market size sub line failed";
        //            listMarketSizeSubLines = _DL_tss_marketmarketsizesublines.Select(organizationService, qMSSL);
        //        }

        //        #endregion

        //        #region look up to Total part consump market size

        //        FilterExpression fTCMS = new FilterExpression(LogicalOperator.And);
        //        //fTCMS.AddCondition("tss_marketsizeid", ConditionOperator.Equal, listMarketSize.Entities[0].GetAttributeValue<Guid>("tss_mastermarketsizeid"));
        //        fTCMS.AddCondition("tss_marketsizeid", ConditionOperator.Equal, en_CurrPotentialProspectPart.GetAttributeValue<EntityReference>("tss_marketsizeid").Id);

        //        QueryExpression qTCMS = new QueryExpression("tss_totalpartconsumpmarketsize");
        //        qTCMS.Criteria.AddFilter(fTCMS);
        //        qTCMS.ColumnSet = new ColumnSet(true);

        //        errModul = "Get total part consump market size failed";
        //        EntityCollection listTotalPartConsumpMarketSize = _DL_tss_totalpartconsumpmarketsize.Select(organizationService, qTCMS);

        //        #endregion

        //        errModul = "Insert Prospect Part Line failed";

        //        #region FIND PRICE TYPE
        //        FilterExpression fPriceType = new FilterExpression(LogicalOperator.And);
        //        fPriceType.AddCondition("tss_type", ConditionOperator.Equal, 865920001);

        //        QueryExpression qPriceType = new QueryExpression("tss_pricelistpart");
        //        qPriceType.Criteria.AddFilter(fPriceType);
        //        qPriceType.ColumnSet = new ColumnSet(true);

        //        EntityCollection oPriceType = _DL_tss_pricelistpart.Select(organizationService, qPriceType);
        //        #endregion



        //        #region if Qty potential prospect <= remaining Qty di total Consump
        //        if (Convert.ToDecimal(enSubLines["tss_qty"]) <= listTotalPartConsumpMarketSize.Entities[0].GetAttributeValue<int>("tss_remainingqty"))
        //        {
        //            //insert prospect partline
        //            ENT_prospectpartline["tss_sourcetype"] = new OptionSetValue(865920001);
        //            ENT_prospectpartline["tss_pricetype"] = new EntityReference("tss_pricelistpart", oPriceType.Entities[0].Id);
        //            ENT_prospectpartline["tss_refmarketsize"] = new EntityReference(_DL_tss_potentialprospectpart.EntityName, en_CurrPotentialProspectPart.GetAttributeValue<Guid>("tss_potentialprospectpartid"));
        //            ENT_prospectpartline["tss_quantity"] = Convert.ToDecimal(enSubLines["tss_qty"]);
        //            ENT_prospectpartline["tss_qtymarketsize"] = Convert.ToDecimal(enSubLines["tss_qty"]);
        //            ENT_prospectpartline["tss_priceamount"] = new Money((decimal)oPrice[0] * Convert.ToDecimal(enSubLines["tss_qty"]));

        //            //organizationService.Create(ENT_prospectpartline);

        //            //update total partConsump market size 

        //            decimal remainingQty = 0;
        //            remainingQty = Convert.ToDecimal(listTotalPartConsumpMarketSize.Entities[0]["tss_remainingqty"]) - Convert.ToDecimal(enSubLines["tss_qty"]);



        //            if (remainingQty == 0)
        //            {
        //                //Update status potential prospect part
        //                en_CurrPotentialProspectPart["tss_status"] = new OptionSetValue(865920002);
        //                en_CurrPotentialProspectPart["tss_statusreason"] = new OptionSetValue(865920002);
        //                organizationService.Update(en_CurrPotentialProspectPart);

        //                //Update Remaining Qty di Market Size Sub Lines
        //                if (listMarketSizeSubLines.Entities[0].Attributes.Contains("tss_remainingqty"))
        //                {
        //                    listMarketSizeSubLines.Entities[0]["tss_remainingqty"] = remainingQty;
        //                    organizationService.Update(listMarketSizeSubLines.Entities[0]);
        //                }
        //            }

        //            //Update Remaining Qty di totalpartConsumpMarketSize
        //            listTotalPartConsumpMarketSize.Entities[0]["tss_remainingqty"] = remainingQty;
        //            organizationService.Update(listTotalPartConsumpMarketSize.Entities[0]);
        //        }

        //        #endregion
        //        #region else if Qty potential prospect > Remaining Qty di total Consump part && Remaining Qty != 0
        //        else if (Convert.ToDecimal(enSubLines["tss_qty"]) > listTotalPartConsumpMarketSize.Entities[0].GetAttributeValue<int>("tss_remainingqty") && listTotalPartConsumpMarketSize.Entities[0].GetAttributeValue<int>("tss_remainingqty") > 0)

        //        {
        //            //insert prospect partline
        //            ENT_prospectpartline["tss_sourcetype"] = new OptionSetValue(865920001);
        //            ENT_prospectpartline["tss_pricetype"] = new EntityReference("tss_pricelistpart", oPriceType.Entities[0].Id);
        //            ENT_prospectpartline["tss_refmarketsize"] = new EntityReference(_DL_tss_potentialprospectpart.EntityName, en_CurrPotentialProspectPart.GetAttributeValue<Guid>("tss_potentialprospectpartid"));
        //            ENT_prospectpartline["tss_quantity"] = Convert.ToDecimal(enSubLines["tss_qty"]);
        //            decimal remainingQty = 0;
        //            remainingQty = Convert.ToDecimal(enSubLines["tss_qty"]) - Convert.ToDecimal(listTotalPartConsumpMarketSize.Entities[0]["tss_remainingqty"]);

        //            ENT_prospectpartline["tss_qtymarketsize"] = remainingQty;
        //            ENT_prospectpartline["tss_priceamount"] = new Money((decimal)oPrice[0] * Convert.ToDecimal(enSubLines["tss_qty"]));

        //            //organizationService.Create(ENT_prospectpartline);

        //            //update total partConsump market size 

        //            decimal remainingQtyTotalPartConsump = 0;
        //            remainingQtyTotalPartConsump = Convert.ToDecimal(listTotalPartConsumpMarketSize.Entities[0]["tss_remainingqty"]) - remainingQty;

        //            listTotalPartConsumpMarketSize.Entities[0]["tss_remainingqty"] = remainingQtyTotalPartConsump;
        //            organizationService.Update(listTotalPartConsumpMarketSize.Entities[0]);

        //            if (remainingQtyTotalPartConsump == 0)
        //            {
        //                //Update status potential prospect part
        //                en_CurrPotentialProspectPart["tss_status"] = new OptionSetValue(865920002);
        //                en_CurrPotentialProspectPart["tss_statusreason"] = new OptionSetValue(865920002);
        //                organizationService.Update(en_CurrPotentialProspectPart);

        //                //Update Remaining Qty di Market Size Sub Lines
        //                if (listMarketSizeSubLines.Entities[0].Attributes.Contains("tss_remainingqty"))
        //                {
        //                    listMarketSizeSubLines.Entities[0]["tss_remainingqty"] = remainingQtyTotalPartConsump;
        //                    organizationService.Update(listMarketSizeSubLines.Entities[0]);
        //                }
        //            }

        //        }

        //        #endregion
        //        #region else if Qty potential prospect > Remaining Qty di total Consump part && Remaining Qty = 0
        //        else if (Convert.ToDecimal(enSubLines["tss_qty"]) > listTotalPartConsumpMarketSize.Entities[0].GetAttributeValue<int>("tss_remainingqty") && listTotalPartConsumpMarketSize.Entities[0].GetAttributeValue<int>("tss_remainingqty") == 0)

        //        {
        //            //insert prospect partline
        //            ENT_prospectpartline["tss_sourcetype"] = new OptionSetValue(865920001);
        //            ENT_prospectpartline["tss_pricetype"] = new EntityReference("tss_pricelistpart", oPriceType.Entities[0].Id);
        //            ENT_prospectpartline["tss_quantity"] = Convert.ToDecimal(enSubLines["tss_qty"]);
        //            ENT_prospectpartline["tss_qtymarketsize"] = 0;
        //            ENT_prospectpartline["tss_priceamount"] = new Money((decimal)oPrice[0] * Convert.ToDecimal(enSubLines["tss_qty"]));

        //            // organizationService.Create(ENT_prospectpartline);
        //        }

        //        #endregion                

        //        organizationService.Create(ENT_prospectpartline);
        //    }

        //    #endregion
        //}

        //public void GenerateProspectPartFromPotentialProspectPartSubLine_OnClick(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context, string recordID)
        //{
        //    string[] arrRecordID = recordID.Split(',');
        //    string errModul = string.Empty;

        //    #region get all selected potential prospect part subline
        //    FilterExpression fPPSSL = new FilterExpression(LogicalOperator.Or);

        //    foreach (string record in arrRecordID)
        //    {
        //        fPPSSL.AddCondition("tss_potentialprospectpartsublinesid", ConditionOperator.Equal, record);
        //    }

        //    QueryExpression qPPSL = new QueryExpression("tss_potentialprospectpartsublines");
        //    qPPSL.Criteria.AddFilter(fPPSSL);
        //    qPPSL.ColumnSet = new ColumnSet(true);

        //    EntityCollection listPotentialProspectPartSubLines = _DL_tss_potentialprospectpartsublines.Select(organizationService, qPPSL);

        //    #endregion

        //    #region get all selected Potential Prospect Part line 

        //    FilterExpression fPPL = new FilterExpression(LogicalOperator.Or);

        //    foreach (Entity ent in listPotentialProspectPartSubLines.Entities)
        //    {
        //        fPPL.AddCondition("tss_potentialprospectpartlinesid", ConditionOperator.Equal, ent.GetAttributeValue<EntityReference>("tss_potentialprospectpartlinesref").Id);
        //    }

        //    QueryExpression qPPL = new QueryExpression(_DL_tss_potentialprospectpartlines.EntityName);
        //    qPPL.Criteria.AddFilter(fPPL);
        //    qPPL.ColumnSet = new ColumnSet(true);

        //    EntityCollection listPotentialProspectPartLines = _DL_tss_potentialprospectpartlines.Select(organizationService, qPPL);

        //    #endregion

        //    #region get selected Potential Prospect Part
        //    Entity en_CurrPotentialProspectPart = _DL_tss_potentialprospectpart.Select(organizationService, listPotentialProspectPartLines.Entities[0].GetAttributeValue<EntityReference>("tss_potentialprospectpart").Id);
        //    #endregion

        //    #region get total Amount

        //    int totalQty = 0;
        //    decimal totalPrice = 0;
        //    decimal totalAmount = 0;

        //    foreach (Entity enSubLines in listPotentialProspectPartSubLines.Entities)
        //    {
        //        totalQty += Convert.ToInt32(enSubLines["tss_qty"]);
        //        totalPrice += Convert.ToDecimal(enSubLines.GetAttributeValue<Money>("tss_price").Value);
        //    }
        //    totalAmount = totalQty * totalPrice;

        //    #endregion

        //    #region get latest est Close Date

        //    DateTime estCloseDate = new DateTime(1900, 1, 1);

        //    foreach (Entity en in listPotentialProspectPartLines.Entities)
        //    {
        //        if (!String.IsNullOrEmpty(en.GetAttributeValue<DateTime>("tss_estclosedate").ToString()))
        //        {
        //            if (en.GetAttributeValue<DateTime>("tss_estclosedate") > estCloseDate)
        //            {
        //                estCloseDate = en.GetAttributeValue<DateTime>("tss_estclosedate");
        //            }
        //        }
        //    }
        //    #endregion

        //    #region get CompanyName

        //    FilterExpression fC = new FilterExpression(LogicalOperator.And);
        //    fC.AddCondition("accountid", ConditionOperator.Equal, en_CurrPotentialProspectPart.GetAttributeValue<EntityReference>("tss_customer").Id);

        //    QueryExpression qC = new QueryExpression("account");
        //    qC.Criteria.AddFilter(fC);
        //    qC.ColumnSet = new ColumnSet(true);

        //    EntityCollection listCustomer = _DL_account.Select(organizationService, qC);

        //    #endregion

        //    #region get BranchCode
        //    string branchCode = string.Empty;
        //    Entity eBusinessUnit = _DL_businessunit.Select(organizationService, context.BusinessUnitId);
        //    //throw new Exception("Test : " + eBusinessUnit.Attributes["trs_branchcode"].ToString());
        //    if (eBusinessUnit.Attributes.Contains("trs_branchcode"))
        //    {
        //        branchCode = eBusinessUnit.Attributes["trs_branchcode"].ToString();
        //    }
        //    #endregion

        //    #region generate prospectpartHeader

        //    Entity ENT_prospectpartheader = new Entity("tss_prospectpartheader");
        //    ENT_prospectpartheader["tss_sourcetype"] = new OptionSetValue(865920001);
        //    ENT_prospectpartheader["tss_estimationclosedate"] = estCloseDate;
        //    ENT_prospectpartheader["tss_pipelinephase"] = new OptionSetValue(865920000);
        //    ENT_prospectpartheader["tss_statusreason"] = new OptionSetValue(865920000);
        //    ENT_prospectpartheader["tss_customer"] = new EntityReference("account", en_CurrPotentialProspectPart.GetAttributeValue<EntityReference>("tss_customer").Id);
        //    if (listCustomer.Entities[0].GetAttributeValue<EntityReference>("primarycontactid") != null)
        //    {
        //        ENT_prospectpartheader["tss_contact"] = listCustomer.Entities[0].GetAttributeValue<EntityReference>("primarycontactid");
        //    }
        //    ENT_prospectpartheader["tss_refmarketsize"] = new EntityReference(_DL_tss_potentialprospectpart.EntityName, en_CurrPotentialProspectPart.GetAttributeValue<Guid>("tss_potentialprospectpartid"));
        //    ENT_prospectpartheader["tss_currency"] = new EntityReference("transactioncurrency", new Guid("7459618D-4EE9-E111-9AA2-544249894792"));
        //    ENT_prospectpartheader["ownerid"] = new EntityReference("systemuser", context.UserId);
        //    ENT_prospectpartheader["tss_pss"] = new EntityReference("account", en_CurrPotentialProspectPart.GetAttributeValue<EntityReference>("tss_pss").Id);
        //    ENT_prospectpartheader["tss_branch"] = new EntityReference(_DL_businessunit.EntityName, context.BusinessUnitId);
        //    ENT_prospectpartheader["tss_totalamount"] = new Money(totalAmount);
        //    ENT_prospectpartheader["tss_topic"] = "Prospect From MS - " +
        //                en_CurrPotentialProspectPart.GetAttributeValue<string>("tss_serialnumberpopulation") +
        //                " - " +
        //                DateTime.Now.ToLocalTime().Year.ToString() +
        //                " by Part Number";


        //    //AssignRequest assignRequest = new AssignRequest
        //    //{
        //    //    Assignee = new EntityReference("systemuser", listPotentialProspectPart.Entities[0].GetAttributeValue<EntityReference>("tss_pss").Id),
        //    //    Target = new EntityReference("tss_prospectpartheader", ENT_prospectpartheader.Id)
        //    //};
        //    //organizationService.Execute(assignRequest);
        //    errModul = "Generate Prospect Header failed";
        //    Guid prospectPartHeaderID = organizationService.Create(ENT_prospectpartheader);
        //    #endregion

        //    #region generate prospect part line 
        //    int lineNumber = 0;

        //    foreach (Entity enSubLines in listPotentialProspectPartSubLines.Entities)
        //    {
        //        lineNumber += 10;
        //        Entity ENT_prospectpartline = new Entity("tss_prospectpartlines");
        //        ENT_prospectpartline["tss_itemnumber"] = lineNumber;
        //        ENT_prospectpartline["tss_prospectpartheader"] = new EntityReference("tss_prospectpartheader", prospectPartHeaderID);
        //        ENT_prospectpartline["tss_partnumber"] = new EntityReference("trs_masterpart", enSubLines.GetAttributeValue<EntityReference>("tss_partnumber").Id);

        //        Entity oPN = organizationService.Retrieve("trs_masterpart", enSubLines.GetAttributeValue<EntityReference>("tss_partnumber").Id, new ColumnSet(true));
        //        ENT_prospectpartline["tss_partdescription"] = oPN.GetAttributeValue<String>("trs_partdescription");

        //        #region GET PRICE
        //        List<object> oPrice = GetSparePartPriceMaster(organizationService, enSubLines.GetAttributeValue<EntityReference>("tss_partnumber").Id);

        //        ENT_prospectpartline["tss_price"] = new Money((decimal)oPrice[0]);
        //        ENT_prospectpartline["tss_minimumprice"] = new Money((decimal)oPrice[1]);
        //        ENT_prospectpartline["transactioncurrencyid"] = new EntityReference("transactioncurrency", (Guid)oPrice[2]);
        //        ENT_prospectpartline["tss_qtymarketsize"] = 0;
        //        //ENT_prospectpartline["tss_priceamount"] = new Money(0);
        //        ENT_prospectpartline["tss_discountamount"] = new Money(0);

        //        ENT_prospectpartline["tss_discountpercent"] = Convert.ToDecimal(0);
        //        ENT_prospectpartline["tss_finalprice"] = new Money(0);
        //        #endregion

        //        #region FIND UNIT GROUP
        //        FilterExpression fUnitGroup = new FilterExpression(LogicalOperator.And);
        //        fUnitGroup.AddCondition("new_populationid", ConditionOperator.Equal, en_CurrPotentialProspectPart.GetAttributeValue<EntityReference>("tss_serialnumber").Id);

        //        QueryExpression qUnitGroup = new QueryExpression("new_population");
        //        qUnitGroup.Criteria.AddFilter(fUnitGroup);
        //        qUnitGroup.ColumnSet = new ColumnSet(true);

        //        EntityCollection oUnitGroup = _DL_population.Select(organizationService, qUnitGroup);

        //        if (oUnitGroup.Entities.Count() > 0)
        //        {
        //            fUnitGroup = new FilterExpression(LogicalOperator.And);
        //            fUnitGroup.AddCondition("name", ConditionOperator.Equal, oUnitGroup.Entities[0].GetAttributeValue<string>("new_productitem"));

        //            qUnitGroup = new QueryExpression("uomschedule");
        //            qUnitGroup.Criteria.AddFilter(fUnitGroup);
        //            qUnitGroup.ColumnSet = new ColumnSet(true);

        //            EntityCollection ounit = organizationService.RetrieveMultiple(qUnitGroup);
        //            ENT_prospectpartline["tss_unitgroup"] = new EntityReference("uomschedule", ounit.Entities[0].Id);
        //        }
        //        #endregion

        //        #region get PartType
        //        FilterExpression fPT = new FilterExpression(LogicalOperator.And);
        //        fPT.AddCondition("trs_masterpartid", ConditionOperator.Equal, enSubLines.GetAttributeValue<EntityReference>("tss_partnumber").Id);

        //        QueryExpression qPT = new QueryExpression("trs_masterpart");
        //        qPT.Criteria.AddFilter(fPT);
        //        qPT.ColumnSet = new ColumnSet(true);

        //        EntityCollection listPartMaster = _DL_partMaster.Select(organizationService, qPT);

        //        errModul = "Get Part Master failed";
        //        ENT_prospectpartline["tss_parttype"] = new OptionSetValue(listPartMaster.Entities[0].GetAttributeValue<OptionSetValue>("tss_parttype").Value);
        //        #endregion

        //        #region look up to market size 
        //        FilterExpression fMS = new FilterExpression(LogicalOperator.And);
        //        fMS.AddCondition("tss_pss", ConditionOperator.Equal, en_CurrPotentialProspectPart.GetAttributeValue<EntityReference>("tss_pss").Id);
        //        fMS.AddCondition("tss_customer", ConditionOperator.Equal, en_CurrPotentialProspectPart.GetAttributeValue<EntityReference>("tss_customer").Id);
        //        //2018.09.17 - start = lessequal & end = greaterequal
        //        //fMS.AddCondition("tss_msperiodend", ConditionOperator.LessEqual, DateTime.Now);
        //        //fMS.AddCondition("tss_msperiodstart", ConditionOperator.GreaterEqual, DateTime.Now);
        //        fMS.AddCondition("tss_msperiodend", ConditionOperator.GreaterEqual, DateTime.Now);
        //        fMS.AddCondition("tss_msperiodstart", ConditionOperator.LessEqual, DateTime.Now);

        //        QueryExpression qMS = new QueryExpression("tss_mastermarketsize");
        //        qMS.Criteria.AddFilter(fMS);
        //        qMS.ColumnSet = new ColumnSet(true);

        //        errModul = "Get Master market size failed";
        //        EntityCollection listMarketSize = _DL_tss_marketmarketsize.Select(organizationService, qMS);
        //        #endregion

        //        #region look up to market size lines

        //        FilterExpression fMSL = new FilterExpression(LogicalOperator.And);
        //        fMSL.AddCondition("tss_mastermarketsizeref", ConditionOperator.Equal, listMarketSize.Entities[0].GetAttributeValue<Guid>("tss_mastermarketsizeid"));

        //        QueryExpression qMSL = new QueryExpression("tss_mastermarketsizelines");
        //        qMSL.Criteria.AddFilter(fMSL);
        //        qMSL.ColumnSet = new ColumnSet(true);
        //        errModul = "Get Master market size line failed";

        //        EntityCollection listMarketSizeLines = _DL_tss_marketmarketsizelines.Select(organizationService, qMSL);

        //        #endregion

        //        #region look up to market size Sublines

        //        FilterExpression fMSSL = new FilterExpression(LogicalOperator.And);
        //        fMSSL.AddCondition("tss_mastermslinesref", ConditionOperator.Equal, listMarketSizeLines.Entities[0].GetAttributeValue<Guid>("tss_mastermarketsizelinesid"));

        //        QueryExpression qMSSL = new QueryExpression("tss_mastermarketsizesublines");
        //        qMSL.Criteria.AddFilter(fMSSL);
        //        qMSL.ColumnSet = new ColumnSet(true);

        //        errModul = "Get Master market size sub line failed";
        //        EntityCollection listMarketSizeSubLines = _DL_tss_marketmarketsizesublines.Select(organizationService, qMSSL);

        //        if (listMarketSizeSubLines.Entities.Count == 0)
        //        {
        //            fMSSL = new FilterExpression(LogicalOperator.And);
        //            fMSSL.AddCondition("tss_mastermarketsizeid", ConditionOperator.Equal, listMarketSize.Entities[0].Id);// listMarketSizeLines.Entities[0].GetAttributeValue<EntityReference>("tss_mastermarketsizelinesid").Id );

        //            qMSSL = new QueryExpression("tss_mastermarketsizesublines");
        //            qMSSL.Criteria.AddFilter(fMSSL);
        //            qMSSL.ColumnSet = new ColumnSet(true);

        //            errModul = "Get Master market size sub line failed";
        //            listMarketSizeSubLines = _DL_tss_marketmarketsizesublines.Select(organizationService, qMSSL);
        //        }

        //        #endregion

        //        #region look up to Total part consump market size

        //        FilterExpression fTCMS = new FilterExpression(LogicalOperator.And);
        //        //fTCMS.AddCondition("tss_marketsizeid", ConditionOperator.Equal, listMarketSize.Entities[0].GetAttributeValue<Guid>("tss_mastermarketsizeid"));
        //        fTCMS.AddCondition("tss_marketsizeid", ConditionOperator.Equal, en_CurrPotentialProspectPart.GetAttributeValue<EntityReference>("tss_marketsizeid").Id);

        //        QueryExpression qTCMS = new QueryExpression("tss_totalpartconsumpmarketsize");
        //        qTCMS.Criteria.AddFilter(fTCMS);
        //        qTCMS.ColumnSet = new ColumnSet(true);

        //        errModul = "Get total part consump market size failed";
        //        EntityCollection listTotalPartConsumpMarketSize = _DL_tss_totalpartconsumpmarketsize.Select(organizationService, qTCMS);

        //        #endregion

        //        #region FIND PRICE TYPE
        //        FilterExpression fPriceType = new FilterExpression(LogicalOperator.And);
        //        fPriceType.AddCondition("tss_type", ConditionOperator.Equal, 865920001);

        //        QueryExpression qPriceType = new QueryExpression("tss_pricelistpart");
        //        qPriceType.Criteria.AddFilter(fPriceType);
        //        qPriceType.ColumnSet = new ColumnSet(true);

        //        EntityCollection oPriceType = _DL_tss_pricelistpart.Select(organizationService, qPriceType);
        //        #endregion

        //        errModul = "Insert Prospect Part Line failed";

        //        #region if Qty potential prospect <= remaining Qty di total Consump
        //        if (Convert.ToDecimal(enSubLines["tss_qty"]) <= listTotalPartConsumpMarketSize.Entities[0].GetAttributeValue<int>("tss_remainingqty"))
        //        {
        //            //insert prospect partline
        //            ENT_prospectpartline["tss_sourcetype"] = new OptionSetValue(865920001);
        //            ENT_prospectpartline["tss_pricetype"] = new EntityReference("tss_pricelistpart", oPriceType.Entities[0].Id);
        //            ENT_prospectpartline["tss_refmarketsize"] = new EntityReference(_DL_tss_potentialprospectpart.EntityName, en_CurrPotentialProspectPart.GetAttributeValue<Guid>("tss_potentialprospectpartid"));
        //            ENT_prospectpartline["tss_quantity"] = Convert.ToDecimal(enSubLines["tss_qty"]);
        //            ENT_prospectpartline["tss_qtymarketsize"] = Convert.ToDecimal(enSubLines["tss_qty"]);
        //            ENT_prospectpartline["tss_priceamount"] = new Money((decimal)oPrice[0] * Convert.ToDecimal(enSubLines["tss_qty"]));
        //            //organizationService.Create(ENT_prospectpartline);

        //            //update total partConsump market size 

        //            decimal remainingQty = 0;
        //            remainingQty = Convert.ToDecimal(listTotalPartConsumpMarketSize.Entities[0]["tss_remainingqty"]) - Convert.ToDecimal(enSubLines["tss_qty"]);



        //            if (remainingQty == 0)
        //            {
        //                //Update status potential prospect part
        //                en_CurrPotentialProspectPart["tss_status"] = new OptionSetValue(865920002);
        //                en_CurrPotentialProspectPart["tss_statusreason"] = new OptionSetValue(865920002);
        //                organizationService.Update(en_CurrPotentialProspectPart);

        //                //Update Remaining Qty di Market Size Sub Lines
        //                if (listMarketSizeSubLines.Entities[0].Attributes.Contains("tss_remainingqty"))
        //                {
        //                    listMarketSizeSubLines.Entities[0]["tss_remainingqty"] = remainingQty;
        //                    organizationService.Update(listMarketSizeSubLines.Entities[0]);
        //                }

        //            }

        //            //Update Remaining Qty di totalpartConsumpMarketSize
        //            listTotalPartConsumpMarketSize.Entities[0]["tss_remainingqty"] = remainingQty;
        //            organizationService.Update(listTotalPartConsumpMarketSize.Entities[0]);
        //        }

        //        #endregion

        //        #region else if Qty potential prospect > Remaining Qty di total Consump part && Remaining Qty != 0
        //        else if (Convert.ToDecimal(enSubLines["tss_qty"]) > listTotalPartConsumpMarketSize.Entities[0].GetAttributeValue<int>("tss_remainingqty") && listTotalPartConsumpMarketSize.Entities[0].GetAttributeValue<int>("tss_remainingqty") > 0)

        //        {
        //            //insert prospect partline
        //            ENT_prospectpartline["tss_sourcetype"] = new OptionSetValue(865920001);
        //            ENT_prospectpartline["tss_pricetype"] = new EntityReference("tss_pricelistpart", oPriceType.Entities[0].Id);
        //            ENT_prospectpartline["tss_refmarketsize"] = new EntityReference(_DL_tss_potentialprospectpart.EntityName, en_CurrPotentialProspectPart.GetAttributeValue<Guid>("tss_potentialprospectpartid"));
        //            ENT_prospectpartline["tss_quantity"] = Convert.ToDecimal(enSubLines["tss_qty"]);
        //            decimal remainingQty = 0;
        //            remainingQty = Convert.ToDecimal(enSubLines["tss_qty"]) - Convert.ToDecimal(listTotalPartConsumpMarketSize.Entities[0]["tss_remainingqty"]);

        //            ENT_prospectpartline["tss_qtymarketsize"] = remainingQty;
        //            ENT_prospectpartline["tss_priceamount"] = new Money((decimal)oPrice[0] * Convert.ToDecimal(enSubLines["tss_qty"]));
        //            //organizationService.Create(ENT_prospectpartline);

        //            //update total partConsump market size 

        //            decimal remainingQtyTotalPartConsump = 0;
        //            remainingQtyTotalPartConsump = Convert.ToDecimal(listTotalPartConsumpMarketSize.Entities[0]["tss_remainingqty"]) - remainingQty;

        //            listTotalPartConsumpMarketSize.Entities[0]["tss_remainingqty"] = remainingQtyTotalPartConsump;
        //            organizationService.Update(listTotalPartConsumpMarketSize.Entities[0]);

        //            if (remainingQtyTotalPartConsump == 0)
        //            {
        //                //Update status potential prospect part
        //                en_CurrPotentialProspectPart["tss_status"] = new OptionSetValue(865920002);
        //                en_CurrPotentialProspectPart["tss_statusreason"] = new OptionSetValue(865920002);
        //                organizationService.Update(en_CurrPotentialProspectPart);

        //                //Update Remaining Qty di Market Size Sub Lines
        //                if (listMarketSizeSubLines.Entities[0].Attributes.Contains("tss_remainingqty"))
        //                {
        //                    listMarketSizeSubLines.Entities[0]["tss_remainingqty"] = remainingQtyTotalPartConsump;
        //                    organizationService.Update(listMarketSizeSubLines.Entities[0]);
        //                }
        //            }

        //        }

        //        #endregion

        //        #region else if Qty potential prospect > Remaining Qty di total Consump part && Remaining Qty = 0
        //        else if (Convert.ToDecimal(enSubLines["tss_qty"]) > listTotalPartConsumpMarketSize.Entities[0].GetAttributeValue<int>("tss_remainingqty") && listTotalPartConsumpMarketSize.Entities[0].GetAttributeValue<int>("tss_remainingqty") == 0)

        //        {
        //            //insert prospect partline
        //            ENT_prospectpartline["tss_sourcetype"] = new OptionSetValue(865920001);
        //            ENT_prospectpartline["tss_pricetype"] = new EntityReference("tss_pricelistpart", oPriceType.Entities[0].Id);
        //            ENT_prospectpartline["tss_quantity"] = Convert.ToDecimal(enSubLines["tss_qty"]);
        //            ENT_prospectpartline["tss_qtymarketsize"] = 0;
        //            ENT_prospectpartline["tss_priceamount"] = new Money((decimal)oPrice[0] * Convert.ToDecimal(enSubLines["tss_qty"]));
        //            //organizationService.Create(ENT_prospectpartline);
        //        }

        //        #endregion

        //        organizationService.Create(ENT_prospectpartline);
        //    }


        //    #endregion
        //}
    }
}
