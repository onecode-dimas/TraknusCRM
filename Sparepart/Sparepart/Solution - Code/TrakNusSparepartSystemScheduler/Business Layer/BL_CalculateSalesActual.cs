using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;


using TrakNusSparepartSystem.DataLayer;
using TrakNusSparepartSystem.Helper;
using TrakNusSparepartSystemScheduler.Helper;

namespace TrakNusSparepartSystemScheduler.Business_Layer
{
    public class BL_CalculateSalesActual
    {
        //source=tss_sourcetype 865.920.001 (Market Size)
        //staus=tss_statecode 865.920.004 (Invoiced)
        private const int MARKET_SIZE = 865920001;
        private const int INVOICED = 865920003;

        DL_tss_salestargetpss _DL_tss_salestargetpss = new DL_tss_salestargetpss();
        DL_tss_salestargetbranch _DL_tss_salestargetbranch = new DL_tss_salestargetbranch();
        DL_tss_salestargetnational _DL_tss_salestargetnational = new DL_tss_salestargetnational();
        DL_tss_salesorderpartheader _DL_tss_salesorderpartheader = new DL_tss_salesorderpartheader();
        DL_tss_salesorderpartlines _DL_tss_salesorderpartlines = new DL_tss_salesorderpartlines();
        DL_tss_prospectpartheader _DL_tss_prospectpartheader = new DL_tss_prospectpartheader();
        DL_tss_potentialprospectpart _DL_tss_potentialprospectpart = new DL_tss_potentialprospectpart();
        DL_tss_marketsizeresultpss _DL_tss_marketsizeresultpss = new DL_tss_marketsizeresultpss();
        DL_tss_salesactualpss _DL_tss_salesactualpss = new DL_tss_salesactualpss();
        DL_tss_salesactualbranch _DL_tss_salesactualbranch = new DL_tss_salesactualbranch();
        DL_tss_salesactualnational _DL_tss_salesactualnational = new DL_tss_salesactualnational();
        DL_tss_marketsizeresultmapping _DL_tss_marketsizeresultmapping = new DL_tss_marketsizeresultmapping();

        string[] month = { "tss_january", "tss_february", "tss_march", "tss_april", "tss_may", "tss_june", "tss_july", "tss_august", "tss_september", "tss_october", "tss_november", "tss_december" };
        public void CalcualteSalesTarget()
        {
            DateTime activeStartDate = DateTime.MinValue;
            DateTime activeEndate = DateTime.MinValue;

            string _connectionString = string.Empty;
            if (ConnString.IsValidConnectionString(ConfigurationManager.ConnectionStrings["CRMCS"].ConnectionString))
                _connectionString = ConfigurationManager.ConnectionStrings["CRMCS"].ConnectionString;
            else
                _connectionString = "Url=http://10.0.10.43/traktornusantara; Domain=traknus; Username=admin.crm; Password=pass@word2;";

            string CRM_URL = _connectionString.Split(';')[0].Remove(0, 4);

            Console.WriteLine("Send email notif master market size");
            Console.WriteLine("====================================");
            Console.WriteLine("Getting crmserviceclient..");
            CrmServiceClient conn = new CrmServiceClient(_connectionString); try
            {
                IOrganizationService _orgService = (IOrganizationService)conn.OrganizationServiceProxy;

                var tssType = Enum.GetValues(typeof(TssType));
                foreach (var tsstype in tssType)
                {
                    Console.WriteLine(string.Format("Calculate Sales Actual:{0} ", tsstype.ToString()));
                    decimal[] targetMarketSize = new decimal[12];
                    decimal totalYearly = 0m;
                    decimal[] monthval = new decimal[12];


                    TssType enumTss = (TssType)Enum.Parse(typeof(TssType), tsstype.ToString(), true);
                    #region Retrieve Sales Actual PSS
                    FilterExpression fsapss = new FilterExpression(LogicalOperator.And);
                    fsapss.AddCondition("tss_type", ConditionOperator.Equal, (int)enumTss);
                    QueryExpression qsapss = new QueryExpression(_DL_tss_salesactualpss.EntityName);
                    qsapss.ColumnSet = new ColumnSet(true);
                    qsapss.Criteria.AddFilter(fsapss);
                    EntityCollection salesActualToUpdates = _DL_tss_salesactualpss.Select(_orgService, qsapss);
                    string[] leadId = null;
                    Guid marketSizeResultPss = Guid.Empty;
                    int invDate = 0;
                    foreach (var salesactual in salesActualToUpdates.Entities)
                    {
                        decimal[] result = new decimal[12];
                        /*
                         * Sales Order Part [tss_prospectlink]->
                         *[tss_prospectpartheaderid] Prospect Part [tss_refmarketsize]->
                         *[Leadid] Potential Prospect[leadid]->
                         *[tss_potentialprospectid]Potential Prospect Part [tss_marketsizeid]->
                         *[tss_marketsizeresultpssid]Market Size Result PSS

                         *Sales Actual PSS[tss_refsalestargetpss]->
                         *[tss_salestargetpssid] Sales Target PSS [tss_marketsizeid]->
                         *[tss_marketsizeresultpssid]Market Size Result PSS
                         */
                        /*
                         * tss_totalamount pada Sales Order Part
                         */


                        FilterExpression fstpss = new FilterExpression(LogicalOperator.And);
                        fstpss.AddCondition("tss_salestargetpssid", ConditionOperator.Equal, salesactual.GetAttributeValue<EntityReference>("tss_refsalestargetpss").Id);
                        QueryExpression qstpss = new QueryExpression(_DL_tss_salestargetpss.EntityName);
                        qstpss.Criteria.AddFilter(fstpss);
                        qstpss.ColumnSet = new ColumnSet(true);
                        EntityCollection stpssObject = _DL_tss_salestargetpss.Select(_orgService, qstpss);
                        Guid salesTargetPssId = Guid.Empty;
                        if (stpssObject.Entities.Count > 0)
                        {
                            activeStartDate = stpssObject.Entities[0].GetAttributeValue<DateTime>("tss_activestartdate");
                            activeStartDate = stpssObject.Entities[0].GetAttributeValue<DateTime>("tss_activeenddate");
                            marketSizeResultPss = stpssObject.Entities[0].GetAttributeValue<EntityReference>("tss_marketsizeid").Id;
                            salesTargetPssId = stpssObject.Entities[0].GetAttributeValue<Guid>("tss_salestargetpssid");
                        }


                        //FilterExpression fppp = new FilterExpression(LogicalOperator.And);
                        //fppp.AddCondition("tss_marketsizeid", ConditionOperator.Equal, marketSizeResultPss);
                        //QueryExpression qppp = new QueryExpression(_DL_tss_potentialprospectpart.EntityName);
                        //qppp.Criteria.AddFilter(fppp);
                        //qppp.ColumnSet = new ColumnSet(true);                      
                        //EntityCollection potentialProspect = _DL_tss_potentialprospectpart.Select(_orgService, qppp);

                        QueryExpression nQExpresion = new QueryExpression(_DL_tss_potentialprospectpart.EntityName);
                        LinkEntity lintToSubline = new LinkEntity
                        {
                            LinkFromEntityName = _DL_tss_potentialprospectpart.EntityName,
                            LinkToEntityName = _DL_tss_prospectpartheader.EntityName,
                            LinkFromAttributeName = "tss_potentialprospectpartid",
                            LinkToAttributeName = "tss_refmarketsize",
                            Columns = new ColumnSet(true),
                            EntityAlias = "prospectheader",
                            JoinOperator = JoinOperator.Inner

                        };
                        nQExpresion.LinkEntities.Add(lintToSubline);
                        nQExpresion.Criteria.AddCondition("tss_marketsizeid", ConditionOperator.Equal, marketSizeResultPss);
                        nQExpresion.ColumnSet = new ColumnSet(true);
                        EntityCollection potentialProspect = _DL_tss_potentialprospectpart.Select(_orgService, nQExpresion);



                        if (potentialProspect.Entities.Count > 0)
                        {
                            leadId = new string[potentialProspect.Entities.Count];
                            leadId = potentialProspect.Entities.Select(x => x.GetAttributeValue<AliasedValue>("prospectheader.tss_prospectpartheaderid").Value.ToString()).ToArray();

                        }
                        else
                            break;

                        switch (enumTss)
                        {
                            case TssType.SalesMarketSize:
                                result = this.CalculateByMarketSize(activeStartDate, activeStartDate, _orgService, leadId, enumTss);
                                break;
                            case TssType.SalesNonMarketSize:
                                result = this.CalculateByMarketSize(activeStartDate, activeStartDate, _orgService, leadId, enumTss);
                                break;
                            case TssType.SalesAll:
                                result = this.CalculateByMarketSize(activeStartDate, activeStartDate, _orgService, leadId, enumTss);
                                break;
                            case TssType.KAContributionAmount:
                                result = this.CalculateByKAContribution(activeStartDate, activeStartDate, _orgService, salesTargetPssId, false);
                                break;
                            case TssType.KAContributionPercentage:
                                result = this.CalculateByKAContribution(activeStartDate, activeStartDate, _orgService, salesTargetPssId, true);
                                break;
                            case TssType.MarketShareAmount:
                                result = this.CalculateByMarketShare(activeStartDate, activeStartDate, _orgService, salesTargetPssId, false);
                                break;
                            case TssType.MarketSharePercentage:
                                result = this.CalculateByMarketShare(activeStartDate, activeStartDate, _orgService, salesTargetPssId, true);
                                break;
                            default:
                                break;
                        }
                        if (result != null)
                        {

                            totalYearly = 0m;
                            for (int m = 0; m < month.Length; m++)
                            {
                                salesactual[month[m]] = new Money(result[m]);

                                totalYearly = totalYearly + salesactual.GetAttributeValue<Money>(month[m]).Value;
                            }
                            salesactual["tss_totalyearly"] = new Money(totalYearly);
                        }

                        _orgService.Update(salesactual);
                    }
                    #endregion

                    #region Retrieve and Update Sales Actual Branch


                    FilterExpression filter;
                    QueryExpression query;
                    FilterExpression fsabranch = new FilterExpression();
                    fsabranch.AddCondition("tss_type", ConditionOperator.Equal, (int)enumTss);
                    QueryExpression qsabranch = new QueryExpression(_DL_tss_salesactualbranch.EntityName);
                    qsabranch.ColumnSet = new ColumnSet(true);
                    qsabranch.Criteria.AddFilter(fsabranch);
                    EntityCollection saBrancToUpdate = _DL_tss_salesactualbranch.Select(_orgService, qsabranch);

                    totalYearly = 0m;
                    foreach (Entity sabranchtoupdate in saBrancToUpdate.Entities)
                    {
                        FilterExpression fmapping = new FilterExpression(LogicalOperator.And);
                        fmapping.AddCondition("tss_salestargetbranch", ConditionOperator.Equal, sabranchtoupdate.GetAttributeValue<EntityReference>("tss_refsalestargetbranch").Id);
                        QueryExpression qmapping = new QueryExpression(_DL_tss_marketsizeresultmapping.EntityName);
                        qmapping.Criteria.AddFilter(fmapping);
                        qmapping.ColumnSet = new ColumnSet(true);
                        EntityCollection resultmappings = _DL_tss_marketsizeresultmapping.Select(_orgService, qmapping);
                        monthval = new decimal[12];
                        var resulmapping = resultmappings.Entities.Select(x => x.GetAttributeValue<EntityReference>("tss_salestargetpss")).Distinct().ToList();
                        foreach (var salestargetpss in resulmapping)
                        {

                            filter = new FilterExpression(LogicalOperator.And);
                            filter.AddCondition("tss_refsalestargetpss", ConditionOperator.Equal, salestargetpss.Id);
                            filter.AddCondition("tss_type", ConditionOperator.Equal, (int)enumTss);
                            query = new QueryExpression(_DL_tss_salesactualpss.EntityName);
                            query.ColumnSet = new ColumnSet(true);
                            query.Criteria.AddFilter(filter);
                            EntityCollection entitySalesActualPss = _DL_tss_salesactualpss.Select(_orgService, query);


                            Entity entitySalesTargetPss = _DL_tss_salestargetpss.Select(_orgService, salestargetpss.Id);
                            if (entitySalesTargetPss != null)
                            {
                                for (int i = 0; i < month.Length; i++)
                                {
                                    targetMarketSize[i] = entitySalesTargetPss.GetAttributeValue<Money>(month[i]).Value;
                                }

                            }
                            foreach (Entity o in entitySalesActualPss.Entities)
                            {
                                for (int i = 0; i < month.Length; i++)
                                {
                                    monthval[i] += o.GetAttributeValue<Money>(month[i]).Value;
                                }
                            }
                        }

                        for (int a = 0; a < month.Length; a++)
                        {
                            totalYearly += monthval[a];
                            switch (enumTss)
                            {
                                case TssType.SalesMarketSize:
                                    sabranchtoupdate[month[a]] = new Money(monthval[a]);
                                    break;
                                case TssType.SalesNonMarketSize:
                                    sabranchtoupdate[month[a]] = new Money(monthval[a]);
                                    break;
                                case TssType.SalesAll:
                                    sabranchtoupdate[month[a]] = new Money(monthval[a]);
                                    break;
                                case TssType.KAContributionAmount:
                                    sabranchtoupdate[month[a]] = targetMarketSize[a] > 0 ? new Money(monthval[a] / targetMarketSize[a]) : new Money(0);
                                    break;
                                case TssType.KAContributionPercentage:
                                    sabranchtoupdate[month[a]] = targetMarketSize[a] > 0 ? new Money((monthval[a] / targetMarketSize[a]) * 100) : new Money(0);
                                    break;
                                case TssType.MarketShareAmount:
                                    sabranchtoupdate[month[a]] = targetMarketSize[a] > 0 ? new Money(monthval[a] / targetMarketSize[a]) : new Money(0);
                                    break;
                                case TssType.MarketSharePercentage:
                                    sabranchtoupdate[month[a]] = targetMarketSize[a] > 0 ? new Money((monthval[a] / targetMarketSize[a]) * 100) : new Money(0);
                                    break;
                                default:
                                    break;
                            }
                            sabranchtoupdate[month[a]] = new Money(monthval[a]);
                        }
                        sabranchtoupdate["tss_totalyearlyactualms"] = new Money(totalYearly);

                        _orgService.Update(sabranchtoupdate);
                    }
                    #endregion

                    #region Retrieve and Update Sales Actual National
                    QueryExpression qnat = new QueryExpression(_DL_tss_salesactualnational.EntityName);
                    FilterExpression fsanat = new FilterExpression();
                    fsanat.AddCondition("tss_type", ConditionOperator.Equal, (int)enumTss);
                    qnat.ColumnSet = new ColumnSet(true);
                    qnat.Criteria.AddFilter(fsanat);
                    EntityCollection saNatToUpdate = _DL_tss_salesactualnational.Select(_orgService, qnat);

                    totalYearly = 0m;
                    foreach (Entity sanattoupdate in saNatToUpdate.Entities)
                    {
                        FilterExpression fmapping = new FilterExpression(LogicalOperator.And);
                        fmapping.AddCondition("tss_salestargetnational", ConditionOperator.Equal, sanattoupdate.GetAttributeValue<EntityReference>("tss_refsalestargetnational").Id);
                        QueryExpression qmapping = new QueryExpression(_DL_tss_marketsizeresultmapping.EntityName);
                        qmapping.Criteria.AddFilter(fmapping);
                        qmapping.ColumnSet = new ColumnSet(true);
                        EntityCollection resultmappings = _DL_tss_marketsizeresultmapping.Select(_orgService, qmapping);
                        var resultmappingpss = resultmappings.Entities.Select(x => x.GetAttributeValue<EntityReference>("tss_salestargetpss")).Distinct().ToList();
                        monthval = new decimal[12];
                        foreach (var resultmapping in resultmappingpss)
                        {

                            filter = new FilterExpression(LogicalOperator.And);
                            filter.AddCondition("tss_refsalestargetpss", ConditionOperator.Equal, resultmapping.Id);
                            filter.AddCondition("tss_type", ConditionOperator.Equal, (int)enumTss);
                            query = new QueryExpression(_DL_tss_salesactualpss.EntityName);
                            query.ColumnSet = new ColumnSet(true);
                            query.Criteria.AddFilter(filter);
                            EntityCollection entitySalesActualPss = _DL_tss_salesactualpss.Select(_orgService, query);


                            Entity entitySalesTargetPss = _DL_tss_salestargetpss.Select(_orgService, resultmapping.Id);
                            if (entitySalesTargetPss != null)
                            {
                                for (int i = 0; i < month.Length; i++)
                                {
                                    targetMarketSize[i] = entitySalesTargetPss.GetAttributeValue<Money>(month[i]).Value;
                                }

                            }

                            foreach (Entity o in entitySalesActualPss.Entities)
                            {
                                for (int i = 0; i < month.Length; i++)
                                {
                                    monthval[i] += o.GetAttributeValue<Money>(month[i]).Value;
                                }
                            }
                        }

                        for (int a = 0; a < month.Length; a++)
                        {

                            totalYearly += monthval[a];
                            switch (enumTss)
                            {
                                case TssType.SalesMarketSize:
                                    sanattoupdate[month[a]] = new Money(monthval[a]);
                                    break;
                                case TssType.SalesNonMarketSize:
                                    sanattoupdate[month[a]] = new Money(monthval[a]);
                                    break;
                                case TssType.SalesAll:
                                    sanattoupdate[month[a]] = new Money(monthval[a]);
                                    break;
                                case TssType.KAContributionAmount:
                                    sanattoupdate[month[a]] = targetMarketSize[a] > 0 ? new Money(monthval[a] / targetMarketSize[a]) : new Money(0);
                                    break;
                                case TssType.KAContributionPercentage:
                                    sanattoupdate[month[a]] = targetMarketSize[a] > 0 ? new Money((monthval[a] / targetMarketSize[a]) * 100) : new Money(0);
                                    break;
                                case TssType.MarketShareAmount:
                                    sanattoupdate[month[a]] = targetMarketSize[a] > 0 ? new Money(monthval[a] / targetMarketSize[a]) : new Money(0);
                                    break;
                                case TssType.MarketSharePercentage:
                                    sanattoupdate[month[a]] = targetMarketSize[a] > 0 ? new Money((monthval[a] / targetMarketSize[a]) * 100) : new Money(0);
                                    break;
                                default:
                                    break;
                            }

                        }
                        sanattoupdate["tss_totalyearly"] = new Money(totalYearly);

                        _orgService.Update(sanattoupdate);
                    }
                    #endregion
                }


            }
            catch (Exception ex)
            {

                throw;
            }
            Console.WriteLine("Successfully Execute");
        }
        private decimal[] CalculateByMarketSize(DateTime activePeriodStart, DateTime activePeriodEnd, IOrganizationService orgService, string[] leadId, TssType tsstype)
        {

            //source=tss_sourcetype 865.920.001 (Market Size)
            //staus=tss_statecode 865.920.004 (Invoiced)
            decimal[] resultValue = new decimal[12];
            #region Retreive Sales Order Part Header base on leadId
            FilterExpression fsoheader = new FilterExpression();
            FilterExpression filterInvoiceDate = new FilterExpression(LogicalOperator.And);
            QueryExpression qsoheader;

            switch (tsstype)
            {
                case TssType.SalesMarketSize:
                    fsoheader.AddCondition("tss_prospectlink", ConditionOperator.In, leadId);
                    fsoheader.AddCondition("tss_sourcetype", ConditionOperator.Equal, MARKET_SIZE);
                    fsoheader.AddCondition("tss_statecode", ConditionOperator.Equal, INVOICED);
                    break;
                case TssType.SalesNonMarketSize:
                    fsoheader.AddCondition("tss_sourcetype", ConditionOperator.NotEqual, MARKET_SIZE);
                    fsoheader.AddCondition("tss_statecode", ConditionOperator.Equal, INVOICED);
                    break;
                case TssType.SalesAll:
                    fsoheader.AddCondition("tss_statecode", ConditionOperator.Equal, INVOICED);
                    filterInvoiceDate.AddCondition("tss_invoicedate", ConditionOperator.GreaterEqual, activePeriodStart.Date);
                    filterInvoiceDate.AddCondition("tss_invoicedate", ConditionOperator.LessEqual, activePeriodEnd.Date);
                    break;
                case TssType.KAContributionAmount:
                    break;
                case TssType.KAContributionPercentage:
                    break;
                case TssType.MarketShareAmount:
                    break;
                case TssType.MarketSharePercentage:
                    break;
                default:
                    break;
            }


            qsoheader = new QueryExpression(_DL_tss_salesorderpartheader.EntityName);
            qsoheader.Criteria.AddFilter(fsoheader);
            EntityCollection sopartheader = _DL_tss_prospectpartheader.Select(orgService, qsoheader);
            string[] salesOrderPartId = null;// Guid.Empty;
            decimal value = 0m;
            if (sopartheader.Entities.Count > 0)
            {
                salesOrderPartId = new string[sopartheader.Entities.Count];
                salesOrderPartId = sopartheader.Entities.Select(x => x.GetAttributeValue<Guid>("tss_sopartheaderid").ToString()).ToArray();
                //value = sopartheader.Entities.Select(x=>x..GetAttributeValue<decimal>("tss_totalamount");
                FilterExpression fe = new FilterExpression(LogicalOperator.And);
                fe.AddCondition("tss_sopartheaderid", ConditionOperator.In, salesOrderPartId);
                fe.AddCondition("tss_status", ConditionOperator.Equal, INVOICED);
                LinkEntity linkSoPartSubLine = new LinkEntity
                {
                    LinkFromEntityName = _DL_tss_salesorderpartlines.EntityName,
                    LinkToEntityName = "tss_salesorderpartsublines",
                    LinkFromAttributeName = "tss_sopartlinesid",
                    LinkToAttributeName = "tss_salesorderpartlines",
                    Columns = new ColumnSet(new string[] { "tss_invoicedate" }),
                    JoinOperator = JoinOperator.Inner,
                    EntityAlias = "sopartsubline"
                };
                QueryExpression q = new QueryExpression(_DL_tss_salesorderpartlines.EntityName);
                q.ColumnSet = new ColumnSet(new string[] { "tss_totalprice" });
                q.Criteria.AddFilter(fe);
                EntityCollection sopartline = _DL_tss_salesorderpartlines.Select(orgService, q);
                int invoicedate = 0;

                if (sopartline.Entities.Count > 0)
                {
                    var result = (from i in sopartline.Entities
                                  group i by new { invoiceDate = Convert.ToDateTime(i.GetAttributeValue<AliasedValue>("sopartsubline.tss_invoicedate").Value).ToLocalTime().Month } into g
                                  select new
                                  {
                                      sumAmount = g.Sum(o => o.GetAttributeValue<Money>("tss_totalprice").Value),
                                      invoicedate = g.Key.invoiceDate
                                  }
                                  ).ToList();

                    invoicedate = result[0].invoicedate;
                    value = result[0].sumAmount;
                    foreach (var o in result)
                    {
                        resultValue[o.invoicedate - 1] = o.sumAmount;
                    }
                }
            }

            return resultValue;
            #endregion

        }

        private decimal[] CalculateByKAContribution(DateTime activePeriodStart, DateTime activePeriodEnd, IOrganizationService orgService, Guid refsalesTargetPss, bool isPercentage)
        {

            //ambil pss yg tts_type=market actual
            FilterExpression fPss = new FilterExpression(LogicalOperator.And);
            QueryExpression qPss = new QueryExpression(_DL_tss_salesactualpss.EntityName);
            fPss.AddCondition("tss_type", ConditionOperator.In, new string[] { ((int)TssType.SalesMarketSize).ToString(), ((int)TssType.SalesAll).ToString() });
            fPss.AddCondition("tss_refsalestargetpss", ConditionOperator.Equal, refsalesTargetPss);

            qPss.Criteria.AddFilter(fPss);
            qPss.ColumnSet = new ColumnSet(true);
            decimal[] resultValue = new decimal[12];
            decimal valMS = 0m;
            decimal valAllSales = 0m;
            EntityCollection colPss = _DL_tss_salesactualpss.Select(orgService, qPss);
            if (colPss.Entities.Count > 1)
            {
                var objMS = colPss.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_type").Value == (int)TssType.SalesMarketSize).FirstOrDefault();
                var objAllSales = colPss.Entities.Where(x => x.GetAttributeValue<OptionSetValue>("tss_type").Value == (int)TssType.SalesAll).FirstOrDefault();

                for (int i = 0; i < month.Length; i++)
                {
                    decimal result = 0;
                    valMS = objMS.GetAttributeValue<Money>(month[i]).Value;
                    valAllSales = objAllSales.GetAttributeValue<Money>(month[i]).Value;
                    if (valAllSales == 0)
                        result = 0;
                    else
                        result = (valMS / valAllSales);
                    resultValue[i] = isPercentage ? result * 100 : result;
                }

            }
            // decimal valuKaContribution = (valMS / valAllSales);
            // Tuple<decimal, DateTime> resultValue = new Tuple<decimal, DateTime>(isPercentage ? valuKaContribution * 100 : valuKaContribution, invoiceDate);
            return resultValue;
            //ambil pss yg tss_type =all sales
        }

        private decimal[] CalculateByMarketShare(DateTime activePeriodStart, DateTime activePeriodEnd, IOrganizationService orgService, Guid refsalesTargetPss, bool isPercentage)
        {

            //ambil pss yg tts_type=market actual
            FilterExpression fPss = new FilterExpression(LogicalOperator.And);
            QueryExpression qPss = new QueryExpression(_DL_tss_salesactualpss.EntityName);
            fPss.AddCondition("tss_type", ConditionOperator.In, new string[] { ((int)TssType.MarketShareAmount).ToString() });
            fPss.AddCondition("tss_refsalestargetpss", ConditionOperator.Equal, refsalesTargetPss);
            decimal[] resultValue = new decimal[12];
            qPss.Criteria.AddFilter(fPss);
            qPss.ColumnSet = new ColumnSet(true);
            decimal[] valSA = new decimal[12];
            EntityCollection colPss = _DL_tss_salesactualpss.Select(orgService, qPss);
            if (colPss.Entities.Count > 0)
            {
                if (colPss.Entities[0].GetAttributeValue<OptionSetValue>("tss_type").Value == (int)TssType.SalesMarketSize)
                {
                    for (int i = 0; i < month.Length; i++)
                    {
                        valSA[i] = colPss.Entities.FirstOrDefault().GetAttributeValue<Money>(month[i]).Value;
                    }
                }
            }
            FilterExpression fStPss = new FilterExpression(LogicalOperator.And);
            QueryExpression qStPss = new QueryExpression(_DL_tss_salestargetpss.EntityName);
            fStPss.AddCondition("tss_salestargetpssid", ConditionOperator.Equal, refsalesTargetPss);


            qStPss.Criteria.AddFilter(fStPss);
            qStPss.ColumnSet = new ColumnSet(true);
            EntityCollection colStPss = _DL_tss_salesactualpss.Select(orgService, qStPss);
            decimal valST = 0m;
            if (colStPss.Entities.Count > 0)
            {
                decimal result = 0;
                for (int i = 0; i < month.Length; i++)
                {
                    valST = colStPss.Entities.FirstOrDefault().GetAttributeValue<Money>(month[i]).Value;
                    if (valST == 0)
                        result = 0;
                    else
                        result = valSA[i] / valST;

                    resultValue[i] = isPercentage ? result * 100 : result;
                }
            }
            //Tuple<decimal, DateTime> resultValue = new Tuple<decimal, DateTime>(isPercentage ? result * 100 : result, invoiceDate);
            return resultValue;
            //ambil pss yg tss_type =all sales
        }
    }
}
