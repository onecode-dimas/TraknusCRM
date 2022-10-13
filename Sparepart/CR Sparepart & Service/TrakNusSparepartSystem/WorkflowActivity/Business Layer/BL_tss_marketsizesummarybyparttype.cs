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
using Microsoft.Xrm.Sdk.Client;
using System.Data.SqlClient;
using System.Data;
using TrakNusSparepartSystem.WorkflowActivity.Business_Layer;

namespace TrakNusSparepartSystem.WorkflowActivity.BusinessLayer
{
    public class BL_tss_marketsizesummarybyparttype
    {
        #region Constant
        private const int ACTIVE = 865920005;
        private const int REVISION = 865920001;
        private const int STATUS_COMPLETED_MS = 865920000;

        private const int MAINPART = 865920000;
        private const int COMMODITY = 865920001;
        private const int TYRE = 865920000;
        private const int BATTERY = 865920001;
        private const int OIL = 865920002;
        private const int OTHERS = 865920003;
        private const int TYRE_AGRO = 100000000;
        private const int TYRE_INDUSTRIAL = 100000001;

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
        DL_trs_masterpart _DL_trs_masterpart = new DL_trs_masterpart();
        DL_tss_marketsizesummarybyparttype _DL_tss_marketsizesummarybyparttype = new DL_tss_marketsizesummarybyparttype();
        DL_tss_sparepartsetup _DL_tss_sparepartsetup = new DL_tss_sparepartsetup();
        DL_tss_keyaccount _DL_tss_keyaccount = new DL_tss_keyaccount();
        RetrieveHelper _retrievehelper = new RetrieveHelper();
        #endregion

        //public void GenerateMarketSizeSummaryByPartType(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        //{
        //    decimal mainparts = 0m;
        //    decimal tyre = 0m;
        //    decimal battery = 0m;
        //    decimal batterycranking = 0m;
        //    decimal batterytraction = 0m;
        //    decimal oil = 0m;
        //    decimal others = 0m;
        //    decimal typeagro = 0m;
        //    decimal typeindustry = 0m;
        //    decimal totalamountms = 0m;
        //    decimal totalcommodity = 0m;

        //    DateTime startPeriodMS = new DateTime();
        //    DateTime endPeriodMS = new DateTime();
        //    DateTime startDtMS = new DateTime();
        //    DateTime endDtMS = new DateTime();

        //    FilterExpression fMSPss = new FilterExpression(LogicalOperator.And);
        //    fMSPss.AddCondition("tss_activeperiodsend", ConditionOperator.GreaterEqual, DateTime.Now);
        //    fMSPss.AddCondition("tss_activeperiodstart", ConditionOperator.LessEqual, DateTime.Now);
        //    fMSPss.AddCondition("tss_status", ConditionOperator.Equal, ACTIVE);

        //    QueryExpression qMSPss = new QueryExpression(_DL_tss_marketsizeresultpss.EntityName);
        //    qMSPss.Criteria.AddFilter(fMSPss);
        //    qMSPss.ColumnSet = new ColumnSet(true);
        //    EntityCollection msPss = _DL_tss_marketsizeresultpss.Select(organizationService, qMSPss);

        //    if(msPss.Entities.Count > 0)
        //    {
        //        foreach(var pss in msPss.Entities)
        //        {
        //            FilterExpression fMS = new FilterExpression(LogicalOperator.And);
        //            fMS.AddCondition("tss_activeperiodsend", ConditionOperator.GreaterEqual, DateTime.Now);
        //            fMS.AddCondition("tss_activeperiodstart", ConditionOperator.LessEqual, DateTime.Now);
        //            fMS.AddCondition("tss_pss", ConditionOperator.Equal, pss.GetAttributeValue<EntityReference>("tss_pss").Id);
        //            fMS.AddCondition("tss_customer", ConditionOperator.Equal, pss.GetAttributeValue<EntityReference>("tss_customer").Id);

        //            QueryExpression qMS = new QueryExpression(_DL_tss_mastermarketsize.EntityName);
        //            qMS.Criteria.AddFilter(fMS);
        //            qMS.ColumnSet = new ColumnSet(true);
        //            EntityCollection ms = _DL_tss_mastermarketsize.Select(organizationService, qMS);

        //            if (ms.Entities.Count > 0)
        //            {
        //                object[] msIds = ms.Entities.Select(x => (object)x.Id).ToArray();

        //                QueryExpression qMSLines = new QueryExpression(_DL_tss_mastermarketsizelines.EntityName);
        //                qMSLines.Criteria.AddCondition("tss_mastermarketsizeref", ConditionOperator.In, msIds);
        //                qMSLines.ColumnSet = new ColumnSet(true);
        //                EntityCollection msLines = _DL_tss_mastermarketsizelines.Select(organizationService, qMSLines);

        //                if (msLines.Entities.Count > 0)
        //                {
        //                    object[] msLinesIds = msLines.Entities.Select(x => (object)x.Id).ToArray();

        //                    QueryExpression qMSSubLines = new QueryExpression(_DL_tss_mastermarketsizesublines.EntityName);
        //                    qMSSubLines.Criteria.AddCondition("tss_mastermslinesref", ConditionOperator.In, msLinesIds);
        //                    qMSSubLines.ColumnSet = new ColumnSet(true);
        //                    EntityCollection msSubLines = _DL_tss_mastermarketsizesublines.Select(organizationService, qMSSubLines);
        //                    if (msSubLines.Entities.Count > 0)
        //                    {
        //                        object[] partIds = msSubLines.Entities.Select(x => (object)x.GetAttributeValue<EntityReference>("tss_partnumber").Id).ToArray();

        //                        QueryExpression qPart = new QueryExpression(_DL_trs_masterpart.EntityName);
        //                        qPart.Criteria.AddCondition("trs_masterpartid", ConditionOperator.In, partIds);
        //                        qPart.Criteria.AddCondition("tss_parttype", ConditionOperator.NotNull);
        //                        qPart.ColumnSet = new ColumnSet(true);
        //                        EntityCollection parts = _DL_trs_masterpart.Select(organizationService, qPart);
        //                        if (parts.Entities.Count > 0)
        //                        {

        //                            if (parts.Entities.Where(x => x.Attributes.Contains("tss_parttype")).Count() > 0)
        //                            {
        //                                object[] mainPartIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == MAINPART).Select(x => (object)x.Id).ToArray();
        //                                object[] tyreIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == COMMODITY && x.GetAttributeValue<OptionSetValue>("tss_commoditytype").Value == TYRE).Select(x => (object)x.Id).ToArray();
        //                                object[] batteryIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == COMMODITY && x.GetAttributeValue<OptionSetValue>("tss_commoditytype").Value == BATTERY).Select(x => (object)x.Id).ToArray();
        //                                object[] oilIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == COMMODITY && x.GetAttributeValue<OptionSetValue>("tss_commoditytype").Value == OIL).Select(x => (object)x.Id).ToArray();
        //                                //object[] othersIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == COMMODITY && x.GetAttributeValue<OptionSetValue>("tss_commoditytype").Value == OTHERS).Select(x => (object)x.Id).ToArray();
        //                                //object[] tyreAgroIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == COMMODITY && x.GetAttributeValue<OptionSetValue>("tss_commoditytype").Value == TYRE_AGRO).Select(x => (object)x.Id).ToArray();
        //                                //object[] tyreIndustrialIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == COMMODITY && x.GetAttributeValue<OptionSetValue>("tss_commoditytype").Value == TYRE_INDUSTRIAL).Select(x => (object)x.Id).ToArray();
        //                                //object[] commIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == COMMODITY).Select(x => (object)x.Id).ToArray();

        //                                mainparts = mainPartIds.Count() > 0 ? msSubLines.Entities.Where(x => mainPartIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
        //                                tyre = tyreIds.Count() > 0 ? msSubLines.Entities.Where(x => tyreIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
        //                                battery = batteryIds.Count() > 0 ? msSubLines.Entities.Where(x => batteryIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
        //                                oil = oilIds.Count() > 0 ? msSubLines.Entities.Where(x => oilIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
        //                                //others = othersIds.Count() > 0 ? msSubLines.Entities.Where(x => othersIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
        //                                //typeagro = tyreAgroIds.Count() > 0 ? msSubLines.Entities.Where(x => tyreAgroIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
        //                                //typeindustry = tyreIndustrialIds.Count() > 0 ? msSubLines.Entities.Where(x => tyreIndustrialIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;

        //                                //typeagro
        //                                //typeindustry
        //                                //batterytraction
        //                                //batterycranking
        //                                //totalcommodity = commIds.Count() > 0 ? msSubLines.Entities.Where(x => commIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;

        //                                QueryExpression qSetup = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
        //                                qSetup.ColumnSet = new ColumnSet(true);
        //                                EntityCollection setups = _DL_tss_sparepartsetup.Select(organizationService, qSetup);

        //                                if (setups.Entities.Count > 0)
        //                                {
        //                                    //TBA
        //                                    startPeriodMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems");
        //                                    endPeriodMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_enddatems");
        //                                }
        //                                FilterExpression fKA = new FilterExpression(LogicalOperator.And);
        //                                fKA.AddCondition("tss_pss", ConditionOperator.Equal, pss.GetAttributeValue<EntityReference>("tss_pss").Id);
        //                                fKA.AddCondition("tss_customer", ConditionOperator.Equal, pss.GetAttributeValue<EntityReference>("tss_customer").Id);
        //                                fKA.AddCondition("tss_activeenddate", ConditionOperator.GreaterEqual, DateTime.Now);
        //                                fKA.AddCondition("tss_activestartdate", ConditionOperator.LessEqual, DateTime.Now);

        //                                QueryExpression qKA = new QueryExpression(_DL_tss_keyaccount.EntityName);
        //                                qKA.Criteria.AddFilter(fKA);
        //                                qKA.ColumnSet.AddColumn("tss_version");
        //                                EntityCollection ka = _DL_tss_keyaccount.Select(organizationService, qKA);

        //                                if (ka.Entities.Count > 0 && setups.Entities.Count > 0)
        //                                {
        //                                    if (ka.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_version").Value == REVISION).Count() == 0)
        //                                    {
        //                                        startDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems");
        //                                        endDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_enddatems");
        //                                    }
        //                                    else
        //                                    {
        //                                        startDtMS = new DateTime(setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems").Year, setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems").Month, 1).AddMonths(setups.Entities[0].GetAttributeValue<int>("tss_evaluationms"));
        //                                        endDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_enddatems");
        //                                    }
        //                                }

        //                                _DL_tss_marketsizesummarybyparttype = new DL_tss_marketsizesummarybyparttype();

        //                                _DL_tss_marketsizesummarybyparttype.tss_marketsizeid = pss.Id;
        //                                _DL_tss_marketsizesummarybyparttype.tss_pss = pss.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                                _DL_tss_marketsizesummarybyparttype.tss_msperiodstart = startPeriodMS;
        //                                _DL_tss_marketsizesummarybyparttype.tss_msperiodend = endPeriodMS;
        //                                _DL_tss_marketsizesummarybyparttype.tss_activeperiodstart = startDtMS;
        //                                _DL_tss_marketsizesummarybyparttype.tss_activeperiodsend = endDtMS;

        //                                _DL_tss_marketsizesummarybyparttype.tss_mainparts = mainparts;
        //                                _DL_tss_marketsizesummarybyparttype.tss_typeagro = tyre; //typeagro
        //                                _DL_tss_marketsizesummarybyparttype.tss_typeindustry = typeindustry;
        //                                _DL_tss_marketsizesummarybyparttype.tss_batterycranking = battery; //batterycranking
        //                                _DL_tss_marketsizesummarybyparttype.tss_batterytraction = batterytraction;
        //                                _DL_tss_marketsizesummarybyparttype.tss_oil = oil;
        //                                _DL_tss_marketsizesummarybyparttype.tss_totalamountms = mainparts + tyre + battery + oil; // + others + typeagro + typeindustry;

        //                                _DL_tss_marketsizesummarybyparttype.Insert(organizationService);
        //                            }

        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    QueryExpression qMSSubLines = new QueryExpression(_DL_tss_mastermarketsizesublines.EntityName);
        //                    qMSSubLines.Criteria.AddCondition("tss_mastermarketsizeid", ConditionOperator.In, msIds);
        //                    qMSSubLines.ColumnSet = new ColumnSet(true);
        //                    EntityCollection msSubLines = _DL_tss_mastermarketsizelines.Select(organizationService, qMSSubLines);

        //                    if (msSubLines.Entities.Count > 0)
        //                    {
        //                        object[] partIds = msSubLines.Entities.Select(x => (object)x.GetAttributeValue<EntityReference>("tss_partnumber").Id).ToArray();

        //                        QueryExpression qPart = new QueryExpression(_DL_trs_masterpart.EntityName);
        //                        qPart.Criteria.AddCondition("trs_masterpartid", ConditionOperator.In, partIds);
        //                        qPart.Criteria.AddCondition("tss_parttype", ConditionOperator.NotNull);
        //                        qPart.ColumnSet = new ColumnSet(true);
        //                        EntityCollection parts = _DL_trs_masterpart.Select(organizationService, qPart);
        //                        if (parts.Entities.Count > 0)
        //                        {

        //                            if (parts.Entities.Where(x => x.Attributes.Contains("tss_parttype")).Count() > 0)
        //                            {

        //                                object[] mainPartIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == MAINPART).Select(x => (object)x.Id).ToArray();
        //                                object[] tyreIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == COMMODITY && x.GetAttributeValue<OptionSetValue>("tss_commoditytype").Value == TYRE).Select(x => (object)x.Id).ToArray();
        //                                object[] batteryIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == COMMODITY && x.GetAttributeValue<OptionSetValue>("tss_commoditytype").Value == BATTERY).Select(x => (object)x.Id).ToArray();
        //                                object[] oilIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == COMMODITY && x.GetAttributeValue<OptionSetValue>("tss_commoditytype").Value == OIL).Select(x => (object)x.Id).ToArray();
        //                                //object[] othersIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == COMMODITY && x.GetAttributeValue<OptionSetValue>("tss_commoditytype").Value == OTHERS).Select(x => (object)x.Id).ToArray();
        //                                //object[] tyreAgroIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == COMMODITY && x.GetAttributeValue<OptionSetValue>("tss_commoditytype").Value == TYRE_AGRO).Select(x => (object)x.Id).ToArray();
        //                                //object[] tyreIndustrialIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == COMMODITY && x.GetAttributeValue<OptionSetValue>("tss_commoditytype").Value == TYRE_INDUSTRIAL).Select(x => (object)x.Id).ToArray();
        //                                //object[] commIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == COMMODITY).Select(x => (object)x.Id).ToArray();

        //                                mainparts = mainPartIds.Count() > 0 ? msSubLines.Entities.Where(x => mainPartIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
        //                                tyre = tyreIds.Count() > 0 ? msSubLines.Entities.Where(x => tyreIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
        //                                battery = batteryIds.Count() > 0 ? msSubLines.Entities.Where(x => batteryIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
        //                                oil = oilIds.Count() > 0 ? msSubLines.Entities.Where(x => oilIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
        //                                //others = othersIds.Count() > 0 ? msSubLines.Entities.Where(x => othersIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
        //                                //typeagro = tyreAgroIds.Count() > 0 ? msSubLines.Entities.Where(x => tyreAgroIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
        //                                //typeindustry = tyreIndustrialIds.Count() > 0 ? msSubLines.Entities.Where(x => tyreIndustrialIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;

        //                                //typeagro
        //                                //typeindustry
        //                                //batterytraction
        //                                //batterycranking
        //                                //totalcommodity = commIds.Count() > 0 ? msSubLines.Entities.Where(x => commIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;

        //                                QueryExpression qSetup = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
        //                                qSetup.ColumnSet = new ColumnSet(true);
        //                                EntityCollection setups = _DL_tss_sparepartsetup.Select(organizationService, qSetup);

        //                                if (setups.Entities.Count > 0)
        //                                {
        //                                    //TBA
        //                                    startPeriodMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems");
        //                                    endPeriodMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_enddatems");
        //                                }
        //                                FilterExpression fKA = new FilterExpression(LogicalOperator.And);
        //                                fKA.AddCondition("tss_pss", ConditionOperator.Equal, pss.GetAttributeValue<EntityReference>("tss_pss").Id);
        //                                fKA.AddCondition("tss_customer", ConditionOperator.Equal, pss.GetAttributeValue<EntityReference>("tss_customer").Id);
        //                                fKA.AddCondition("tss_activeenddate", ConditionOperator.GreaterEqual, DateTime.Now);
        //                                fKA.AddCondition("tss_activestartdate", ConditionOperator.LessEqual, DateTime.Now);

        //                                QueryExpression qKA = new QueryExpression(_DL_tss_keyaccount.EntityName);
        //                                qKA.Criteria.AddFilter(fKA);
        //                                qKA.ColumnSet.AddColumn("tss_version");
        //                                EntityCollection ka = _DL_tss_keyaccount.Select(organizationService, qKA);

        //                                if (ka.Entities.Count > 0 && setups.Entities.Count > 0)
        //                                {
        //                                    if (ka.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_version").Value == REVISION).Count() == 0)
        //                                    {
        //                                        startDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems");
        //                                        endDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_enddatems");
        //                                    }
        //                                    else
        //                                    {
        //                                        startDtMS = new DateTime(setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems").Year, setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems").Month, 1).AddMonths(setups.Entities[0].GetAttributeValue<int>("tss_evaluationms"));
        //                                        endDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_enddatems");
        //                                    }
        //                                }

        //                                _DL_tss_marketsizesummarybyparttype = new DL_tss_marketsizesummarybyparttype();

        //                                _DL_tss_marketsizesummarybyparttype.tss_marketsizeid = pss.Id;
        //                                _DL_tss_marketsizesummarybyparttype.tss_pss = pss.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                                _DL_tss_marketsizesummarybyparttype.tss_msperiodstart = startPeriodMS;
        //                                _DL_tss_marketsizesummarybyparttype.tss_msperiodend = endPeriodMS;
        //                                _DL_tss_marketsizesummarybyparttype.tss_activeperiodstart = startDtMS;
        //                                _DL_tss_marketsizesummarybyparttype.tss_activeperiodsend = endDtMS;

        //                                _DL_tss_marketsizesummarybyparttype.tss_mainparts = mainparts;
        //                                _DL_tss_marketsizesummarybyparttype.tss_typeagro = tyre; //typeagro
        //                                _DL_tss_marketsizesummarybyparttype.tss_typeindustry = typeindustry;
        //                                _DL_tss_marketsizesummarybyparttype.tss_batterycranking = battery; //batterycranking
        //                                _DL_tss_marketsizesummarybyparttype.tss_batterytraction = batterytraction;
        //                                _DL_tss_marketsizesummarybyparttype.tss_oil = oil;
        //                                _DL_tss_marketsizesummarybyparttype.tss_totalamountms = mainparts + tyre + battery + oil; // + others + typeagro + typeindustry;

        //                                _DL_tss_marketsizesummarybyparttype.Insert(organizationService);
        //                            }

        //                        }
        //                    }
        //                }
        //            }


        //        }
        //    }
        //}

        public void GenerateMarketSizeSummaryByPartType_UsingSP_OnClick(IOrganizationService _organizationservice, ITracingService _tracingservice, IWorkflowContext _workflowcontext)
        {
            using (OrganizationServiceContext _context = new OrganizationServiceContext(_organizationservice))
            {
                List<SqlParameter> _sqlparameters = new List<SqlParameter>()
                {
                    new SqlParameter("@systemuserid", SqlDbType.NVarChar) { Value = _workflowcontext.UserId.ToString().Replace("{", "").Replace("}", "") },
                };

                DataTable _datatable = new GetStoredProcedure().Connect("sp_ms_GenerateMS_SummaryByPartType_ALL", _sqlparameters, false);
            }
        }

        public void GenerateMarketSizeSummaryByPartType(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {
            decimal mainparts = 0m;
            decimal tyre = 0m;
            decimal battery = 0m;
            decimal batterycranking = 0m;
            decimal batterytraction = 0m;
            decimal oil = 0m;
            decimal others = 0m;
            decimal typeagro = 0m;
            decimal typeindustry = 0m;
            decimal totalamountms = 0m;
            decimal totalcommodity = 0m;

            DateTime startPeriodMS = new DateTime();
            DateTime endPeriodMS = new DateTime();
            DateTime startDtMS = new DateTime();
            DateTime endDtMS = new DateTime();

            FilterExpression fMSPSS = new FilterExpression(LogicalOperator.And);
            fMSPSS.AddCondition("tss_activeperiodsend", ConditionOperator.GreaterEqual, DateTime.Now);
            fMSPSS.AddCondition("tss_activeperiodstart", ConditionOperator.LessEqual, DateTime.Now);
            fMSPSS.AddCondition("tss_status", ConditionOperator.Equal, ACTIVE);

            QueryExpression qMSPSS = new QueryExpression(_DL_tss_marketsizeresultpss.EntityName);
            qMSPSS.Criteria.AddFilter(fMSPSS);
            qMSPSS.ColumnSet = new ColumnSet(true);
            EntityCollection msPSS = _DL_tss_marketsizeresultpss.Select(organizationService, qMSPSS);

            if (msPSS.Entities.Count > 0)
            {
                foreach (var pss in msPSS.Entities)
                {
                    //MAINPARTS
                    FilterExpression aMS = new FilterExpression(LogicalOperator.Or);
                    aMS.AddCondition("tss_unittype", ConditionOperator.Equal, 865920000);
                    aMS.AddCondition("tss_unittype", ConditionOperator.Equal, 865920001);

                    FilterExpression fMS = new FilterExpression(LogicalOperator.And);
                    fMS.AddFilter(aMS);
                    fMS.AddCondition("tss_activeperiodsend", ConditionOperator.GreaterEqual, DateTime.Now);
                    fMS.AddCondition("tss_activeperiodstart", ConditionOperator.LessEqual, DateTime.Now);
                    fMS.AddCondition("tss_pss", ConditionOperator.Equal, pss.GetAttributeValue<EntityReference>("tss_pss").Id);
                    fMS.AddCondition("tss_customer", ConditionOperator.Equal, pss.GetAttributeValue<EntityReference>("tss_customer").Id);
                    fMS.AddCondition("tss_status", ConditionOperator.Equal, STATUS_COMPLETED_MS);

                    QueryExpression qMS = new QueryExpression(_DL_tss_mastermarketsize.EntityName);
                    qMS.Criteria.AddFilter(fMS);
                    qMS.ColumnSet = new ColumnSet(true);
                    List<Entity> ms = _retrievehelper.RetrieveMultiple(organizationService, qMS); // _DL_tss_mastermarketsize.Select(organizationService, qMS);

                    if (ms.Count > 0)
                    {
                        object[] msIds = ms.Select(x => (object)x.Id).ToArray();

                        QueryExpression qMSLines = new QueryExpression(_DL_tss_mastermarketsizelines.EntityName);
                        qMSLines.Criteria.AddCondition("tss_mastermarketsizeref", ConditionOperator.In, msIds);
                        //qMSLines.Criteria.AddCondition("tss_duedate", ConditionOperator.NotNull);
                        qMSLines.ColumnSet = new ColumnSet(true);

                        List<Entity> msLines = _retrievehelper.RetrieveMultiple(organizationService, qMSLines); // _DL_tss_mastermarketsizelines.Select(organizationService, qMSLines);

                        if (msLines.Count > 0)
                        {
                            object[] msLinesIds = msLines
                                .Where(x =>
                                (
                                    (x.GetAttributeValue<DateTime>("tss_duedate") != DateTime.MinValue && (x.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD1 || x.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD2 || x.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD3)) ||
                                    (x.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD4 || x.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value == MTD5))
                                )
                                .Select(x => (object)x.Id).ToArray();

                            QueryExpression qMSSubLines = new QueryExpression(_DL_tss_mastermarketsizesublines.EntityName);
                            qMSSubLines.Criteria.AddCondition("tss_mastermslinesref", ConditionOperator.In, msLinesIds);
                            qMSSubLines.ColumnSet = new ColumnSet(true);

                            List<Entity> msSubLines = _retrievehelper.RetrieveMultiple(organizationService, qMSSubLines); // _DL_tss_mastermarketsizesublines.Select(organizationService, qMSSubLines);

                            if (msSubLines.Count > 0)
                            {
                                object[] partIds = msSubLines.Select(x => (object)x.GetAttributeValue<EntityReference>("tss_partnumber").Id).ToArray();

                                QueryExpression qPart = new QueryExpression(_DL_trs_masterpart.EntityName);
                                qPart.Criteria.AddCondition("trs_masterpartid", ConditionOperator.In, partIds);
                                qPart.Criteria.AddCondition("tss_parttype", ConditionOperator.NotNull);
                                qPart.ColumnSet = new ColumnSet(true);
                                EntityCollection parts = _DL_trs_masterpart.Select(organizationService, qPart);

                                if (parts.Entities.Count > 0)
                                {
                                    if (parts.Entities.Where(x => x.Attributes.Contains("tss_parttype")).Count() > 0)
                                    {
                                        object[] mainPartIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == MAINPART).Select(x => (object)x.Id).ToArray();
                                        object[] tyreIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == COMMODITY && x.GetAttributeValue<OptionSetValue>("tss_commoditytype").Value == TYRE).Select(x => (object)x.Id).ToArray();
                                        object[] batteryIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == COMMODITY && x.GetAttributeValue<OptionSetValue>("tss_commoditytype").Value == BATTERY).Select(x => (object)x.Id).ToArray();
                                        object[] oilIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == COMMODITY && x.GetAttributeValue<OptionSetValue>("tss_commoditytype").Value == OIL).Select(x => (object)x.Id).ToArray();
                                        //object[] othersIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == COMMODITY && x.GetAttributeValue<OptionSetValue>("tss_commoditytype").Value == OTHERS).Select(x => (object)x.Id).ToArray();
                                        //object[] tyreAgroIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == COMMODITY && x.GetAttributeValue<OptionSetValue>("tss_commoditytype").Value == TYRE_AGRO).Select(x => (object)x.Id).ToArray();
                                        //object[] tyreIndustrialIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == COMMODITY && x.GetAttributeValue<OptionSetValue>("tss_commoditytype").Value == TYRE_INDUSTRIAL).Select(x => (object)x.Id).ToArray();
                                        //object[] commIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == COMMODITY).Select(x => (object)x.Id).ToArray();

                                        List<Guid> batteryTractionIds = new List<Guid>();
                                        List<Guid> batteryCrankingIds = new List<Guid>();
                                        List<Guid> batteryOnlyIds = new List<Guid>();

                                        foreach (Guid batteryId in batteryIds)
                                        {
                                            QueryExpression qTraction = new QueryExpression("tss_partmasterlinesbattery");
                                            qTraction.Criteria.AddCondition("tss_partmasterid", ConditionOperator.Equal, (Guid)batteryId);
                                            qTraction.Criteria.AddCondition("tss_batterytype", ConditionOperator.Equal, 865920000);
                                            qTraction.ColumnSet = new ColumnSet(true);
                                            EntityCollection oTraction = organizationService.RetrieveMultiple(qTraction);

                                            if (oTraction.Entities.Count > 0)
                                            {
                                                batteryTractionIds.Add(batteryId);
                                            }
                                            else
                                            {
                                                QueryExpression qCranking = new QueryExpression("tss_partmasterlinesbattery");
                                                qCranking.Criteria.AddCondition("tss_partmasterid", ConditionOperator.Equal, (Guid)batteryId);
                                                qTraction.Criteria.AddCondition("tss_batterytype", ConditionOperator.Equal, 865920001);
                                                qCranking.ColumnSet = new ColumnSet(true);
                                                EntityCollection oCranking = organizationService.RetrieveMultiple(qCranking);

                                                if (oCranking.Entities.Count > 0)
                                                {
                                                    batteryCrankingIds.Add(batteryId);
                                                }
                                                else
                                                {
                                                    batteryOnlyIds.Add(batteryId);
                                                }
                                            }
                                        }

                                        mainparts = mainPartIds.Count() > 0 ? msSubLines.Where(x => mainPartIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
                                        tyre = tyreIds.Count() > 0 ? msSubLines.Where(x => tyreIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
                                        //battery = batteryIds.Count() > 0 ? msSubLines.Where(x => batteryIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
                                        oil = oilIds.Count() > 0 ? msSubLines.Where(x => oilIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
                                        //others = othersIds.Count() > 0 ? msSubLines.Where(x => othersIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
                                        //typeagro = tyreAgroIds.Count() > 0 ? msSubLines.Where(x => tyreAgroIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
                                        //typeindustry = tyreIndustrialIds.Count() > 0 ? msSubLines.Where(x => tyreIndustrialIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
                                        //totalcommodity = commIds.Count() > 0 ? msSubLines.Where(x => commIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
                                        batterytraction = batteryTractionIds.Count() > 0 ? msSubLines.Where(x => batteryTractionIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
                                        batterycranking = batteryCrankingIds.Count() > 0 ? msSubLines.Where(x => batteryCrankingIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
                                        battery = batteryOnlyIds.Count() > 0 ? msSubLines.Where(x => batteryOnlyIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;

                                    }
                                }
                            }
                        }
                    }

                    //COMMODITY
                    fMS = new FilterExpression(LogicalOperator.And);
                    fMS.AddCondition("tss_activeperiodsend", ConditionOperator.GreaterEqual, DateTime.Now);
                    fMS.AddCondition("tss_activeperiodstart", ConditionOperator.LessEqual, DateTime.Now);
                    fMS.AddCondition("tss_pss", ConditionOperator.Equal, pss.GetAttributeValue<EntityReference>("tss_pss").Id);
                    fMS.AddCondition("tss_customer", ConditionOperator.Equal, pss.GetAttributeValue<EntityReference>("tss_customer").Id);
                    fMS.AddCondition("tss_unittype", ConditionOperator.Equal, 865920002);
                    fMS.AddCondition("tss_status", ConditionOperator.Equal, STATUS_COMPLETED_MS);

                    qMS = new QueryExpression(_DL_tss_mastermarketsize.EntityName);
                    qMS.Criteria.AddFilter(fMS);
                    qMS.ColumnSet = new ColumnSet(true);

                    ms = new List<Entity>();
                    ms = _retrievehelper.RetrieveMultiple(organizationService, qMS); // _DL_tss_mastermarketsize.Select(organizationService, qMS);

                    if (ms.Count > 0)
                    {
                        object[] msIds = ms.Select(x => (object)x.Id).ToArray();

                        QueryExpression qMSSubLinesCommodity = new QueryExpression(_DL_tss_mastermarketsizesublines.EntityName);
                        qMSSubLinesCommodity.Criteria.AddCondition("tss_mastermarketsizeid", ConditionOperator.In, msIds);
                        qMSSubLinesCommodity.ColumnSet = new ColumnSet(true);

                        List<Entity> msSubLinesCommodity = _retrievehelper.RetrieveMultiple(organizationService, qMSSubLinesCommodity); // _DL_tss_mastermarketsizesublines.Select(organizationService, qMSSubLinesCommodity);

                        if (msSubLinesCommodity.Count > 0)
                        {
                            object[] partIds = msSubLinesCommodity.Select(x => (object)x.GetAttributeValue<EntityReference>("tss_partnumber").Id).ToArray();

                            QueryExpression qPart = new QueryExpression(_DL_trs_masterpart.EntityName);
                            qPart.Criteria.AddCondition("trs_masterpartid", ConditionOperator.In, partIds);
                            qPart.Criteria.AddCondition("tss_parttype", ConditionOperator.NotNull);
                            qPart.ColumnSet = new ColumnSet(true);
                            EntityCollection parts = _DL_trs_masterpart.Select(organizationService, qPart);

                            if (parts.Entities.Count > 0)
                            {
                                if (parts.Entities.Where(x => x.Attributes.Contains("tss_parttype")).Count() > 0)
                                {
                                    object[] mainPartIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == MAINPART).Select(x => (object)x.Id).ToArray();
                                    object[] tyreIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == COMMODITY && x.GetAttributeValue<OptionSetValue>("tss_commoditytype").Value == TYRE).Select(x => (object)x.Id).ToArray();
                                    object[] batteryIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == COMMODITY && x.GetAttributeValue<OptionSetValue>("tss_commoditytype").Value == BATTERY).Select(x => (object)x.Id).ToArray();
                                    object[] oilIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == COMMODITY && x.GetAttributeValue<OptionSetValue>("tss_commoditytype").Value == OIL).Select(x => (object)x.Id).ToArray();
                                    //object[] othersIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == COMMODITY && x.GetAttributeValue<OptionSetValue>("tss_commoditytype").Value == OTHERS).Select(x => (object)x.Id).ToArray();
                                    //object[] tyreAgroIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == COMMODITY && x.GetAttributeValue<OptionSetValue>("tss_commoditytype").Value == TYRE_AGRO).Select(x => (object)x.Id).ToArray();
                                    //object[] tyreIndustrialIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == COMMODITY && x.GetAttributeValue<OptionSetValue>("tss_commoditytype").Value == TYRE_INDUSTRIAL).Select(x => (object)x.Id).ToArray();
                                    //object[] commIds = parts.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_parttype").Value == COMMODITY).Select(x => (object)x.Id).ToArray();

                                    List<Guid> batteryTractionIds = new List<Guid>();
                                    List<Guid> batteryCrankingIds = new List<Guid>();
                                    List<Guid> batteryOnlyIds = new List<Guid>();

                                    foreach (Guid batteryId in batteryIds)
                                    {
                                        QueryExpression qTraction = new QueryExpression("tss_partmasterlinesbattery");
                                        qTraction.Criteria.AddCondition("tss_partmasterid", ConditionOperator.Equal, (Guid)batteryId);
                                        qTraction.Criteria.AddCondition("tss_batterytype", ConditionOperator.Equal, 865920000);
                                        qTraction.ColumnSet = new ColumnSet(true);
                                        EntityCollection oTraction = organizationService.RetrieveMultiple(qTraction);

                                        if (oTraction.Entities.Count > 0)
                                        {
                                            batteryTractionIds.Add(batteryId);
                                        }
                                        else
                                        {
                                            QueryExpression qCranking = new QueryExpression("tss_partmasterlinesbattery");
                                            qCranking.Criteria.AddCondition("tss_partmasterid", ConditionOperator.Equal, (Guid)batteryId);
                                            qTraction.Criteria.AddCondition("tss_batterytype", ConditionOperator.Equal, 865920001);
                                            qCranking.ColumnSet = new ColumnSet(true);
                                            EntityCollection oCranking = organizationService.RetrieveMultiple(qCranking);

                                            if (oCranking.Entities.Count > 0)
                                            {
                                                batteryCrankingIds.Add(batteryId);
                                            }
                                            else
                                            {
                                                batteryOnlyIds.Add(batteryId);
                                            }
                                        }
                                    }

                                    mainparts += mainPartIds.Count() > 0 ? msSubLinesCommodity.Where(x => mainPartIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
                                    tyre += tyreIds.Count() > 0 ? msSubLinesCommodity.Where(x => tyreIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
                                    //battery += batteryIds.Count() > 0 ? msSubLinesCommodity.Where(x => batteryIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
                                    oil += oilIds.Count() > 0 ? msSubLinesCommodity.Where(x => oilIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
                                    //others += othersIds.Count() > 0 ? msSubLinesCommodity.Where(x => othersIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
                                    //typeagro += tyreAgroIds.Count() > 0 ? msSubLinesCommodity.Where(x => tyreAgroIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
                                    //typeindustry += tyreIndustrialIds.Count() > 0 ? msSubLinesCommodity.Where(x => tyreIndustrialIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
                                    //totalcommodity += commIds.Count() > 0 ? msSubLines.Where(x => commIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
                                    batterytraction = batteryTractionIds.Count() > 0 ? msSubLinesCommodity.Where(x => batteryTractionIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
                                    batterycranking = batteryCrankingIds.Count() > 0 ? msSubLinesCommodity.Where(x => batteryCrankingIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;
                                    battery = batteryOnlyIds.Count() > 0 ? msSubLinesCommodity.Where(x => batteryOnlyIds.Contains(x.GetAttributeValue<EntityReference>("tss_partnumber").Id)).Sum(x => x.GetAttributeValue<int>("tss_qty") * x.GetAttributeValue<Money>("tss_price").Value) : 0m;

                                }

                            }
                        }
                    }

                    //CALCULATE TOTAL
                    QueryExpression qSetup = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
                    qSetup.ColumnSet = new ColumnSet(true);
                    EntityCollection setups = _DL_tss_sparepartsetup.Select(organizationService, qSetup);

                    if (setups.Entities.Count > 0)
                    {
                        //TBA
                        startPeriodMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems");
                        endPeriodMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_enddatems");
                    }
                    FilterExpression fKA = new FilterExpression(LogicalOperator.And);
                    fKA.AddCondition("tss_pss", ConditionOperator.Equal, pss.GetAttributeValue<EntityReference>("tss_pss").Id);
                    fKA.AddCondition("tss_customer", ConditionOperator.Equal, pss.GetAttributeValue<EntityReference>("tss_customer").Id);
                    fKA.AddCondition("tss_activeenddate", ConditionOperator.GreaterEqual, DateTime.Now);
                    fKA.AddCondition("tss_activestartdate", ConditionOperator.LessEqual, DateTime.Now);

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

                    FilterExpression fExist = new FilterExpression(LogicalOperator.And);
                    fExist.AddCondition("tss_pss", ConditionOperator.Equal, pss.GetAttributeValue<EntityReference>("tss_pss").Id);
                    fExist.AddCondition("tss_marketsizeid", ConditionOperator.Equal, pss.Id);

                    QueryExpression qExist = new QueryExpression(_DL_tss_marketsizesummarybyparttype.EntityName);
                    qExist.Criteria.AddFilter(fExist);
                    qExist.ColumnSet = new ColumnSet(true);

                    EntityCollection totalSummaryByPartTypeQ = _DL_tss_marketsizesummarybyparttype.Select(organizationService, qExist);

                    if (totalSummaryByPartTypeQ.Entities.Count() == 0)
                    {
                        _DL_tss_marketsizesummarybyparttype = new DL_tss_marketsizesummarybyparttype();

                        _DL_tss_marketsizesummarybyparttype.tss_marketsizeid = pss.Id;
                        _DL_tss_marketsizesummarybyparttype.tss_pss = pss.GetAttributeValue<EntityReference>("tss_pss").Id;
                        _DL_tss_marketsizesummarybyparttype.tss_msperiodstart = startPeriodMS;
                        _DL_tss_marketsizesummarybyparttype.tss_msperiodend = endPeriodMS;
                        _DL_tss_marketsizesummarybyparttype.tss_activeperiodstart = startDtMS;
                        _DL_tss_marketsizesummarybyparttype.tss_activeperiodsend = endDtMS;

                        _DL_tss_marketsizesummarybyparttype.tss_mainparts = mainparts;
                        _DL_tss_marketsizesummarybyparttype.tss_typeagro = tyre; //typeagro
                        _DL_tss_marketsizesummarybyparttype.tss_typeindustry = typeindustry;
                        _DL_tss_marketsizesummarybyparttype.tss_batterycranking = batterycranking + battery;
                        _DL_tss_marketsizesummarybyparttype.tss_batterytraction = batterytraction;
                        _DL_tss_marketsizesummarybyparttype.tss_oil = oil;
                        _DL_tss_marketsizesummarybyparttype.tss_totalamountms = mainparts + tyre + batterycranking + batterytraction + battery + oil; // + others + typeagro + typeindustry;

                        _DL_tss_marketsizesummarybyparttype.Insert(organizationService);
                    }
                    else
                    {
                        _DL_tss_marketsizesummarybyparttype = new DL_tss_marketsizesummarybyparttype();

                        _DL_tss_marketsizesummarybyparttype.tss_msperiodstart = startPeriodMS;
                        _DL_tss_marketsizesummarybyparttype.tss_msperiodend = endPeriodMS;
                        _DL_tss_marketsizesummarybyparttype.tss_activeperiodstart = startDtMS;
                        _DL_tss_marketsizesummarybyparttype.tss_activeperiodsend = endDtMS;

                        _DL_tss_marketsizesummarybyparttype.tss_mainparts = mainparts;
                        _DL_tss_marketsizesummarybyparttype.tss_typeagro = tyre; //typeagro
                        _DL_tss_marketsizesummarybyparttype.tss_typeindustry = typeindustry;
                        _DL_tss_marketsizesummarybyparttype.tss_batterycranking = batterycranking + battery;
                        _DL_tss_marketsizesummarybyparttype.tss_batterytraction = batterytraction;
                        _DL_tss_marketsizesummarybyparttype.tss_oil = oil;
                        _DL_tss_marketsizesummarybyparttype.tss_totalamountms = mainparts + tyre + batterycranking + batterytraction + battery + oil; // + others + typeagro + typeindustry;

                        _DL_tss_marketsizesummarybyparttype.Update(organizationService, totalSummaryByPartTypeQ.Entities[0].Id);
                    }


                }
            }
        }


    }
}
