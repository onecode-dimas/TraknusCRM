using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrakNusSparepartSystemScheduler.Helper;

namespace TrakNusSparepartSystemScheduler.Business_Layer
{
    public class BL_IncentiveCalculation
    {
        private const int STATUS_PROCESSED = 865920000;
        public void IncetiveCalculation()
        {
            string _connectionString = string.Empty;
            if (ConnString.IsValidConnectionString(ConfigurationManager.ConnectionStrings["CRMCS"].ConnectionString))
                _connectionString = ConfigurationManager.ConnectionStrings["CRMCS"].ConnectionString;
            else
                _connectionString = "Url=http://10.0.10.43/traktornusantara; Domain=traknus; Username=admin.crm; Password=pass@word2;";

            string CRM_URL = _connectionString.Split(';')[0].Remove(0, 4);

            Console.WriteLine("Incentive Calculation");
            Console.WriteLine("====================================");
            Console.WriteLine("Getting crmserviceclient..");
            CrmServiceClient conn = new CrmServiceClient(_connectionString); try
            {
                IOrganizationService _orgService = (IOrganizationService)conn.OrganizationServiceProxy;
                FilterExpression fe = new FilterExpression(LogicalOperator.And);
                fe.AddCondition("tss_status", ConditionOperator.In, 865920004);//paid status
                LinkEntity linkQuotePart = new LinkEntity
                {
                    LinkFromEntityName = "tss_quotationpartheader",
                    LinkToEntityName = "trs_quotation",
                    LinkFromAttributeName = "tss_quotationpartheaderid",
                    LinkToAttributeName = "tss_quotationpartno",
                    JoinOperator = JoinOperator.Inner,
                    Columns = new ColumnSet(new string[] { "trs_quotationid" }),
                    EntityAlias = "quoteservice"
                };

                LinkEntity linkQuoteService = new LinkEntity
                {
                    LinkFromEntityName = "trs_quotation",
                    LinkToEntityName = "trs_quotationcommercialheader",
                    LinkFromAttributeName = "trs_quotationid",
                    LinkToAttributeName = "trs_quotationid",
                    JoinOperator = JoinOperator.LeftOuter,
                    Columns = new ColumnSet(new string[] { "trs_commercialheader" }),
                    EntityAlias = "service"
                };

                LinkEntity linkSOPart = new LinkEntity
                {
                    LinkFromEntityName = "tss_quotationpartheader",
                    LinkToEntityName = "tss_sopartheader",
                    LinkFromAttributeName = "tss_quotationpartheaderid",
                    LinkToAttributeName = "tss_quotationlink",
                    JoinOperator = JoinOperator.Inner,
                    Columns = new ColumnSet(new string[] { "tss_sopartheaderid", "tss_pss", "tss_branch" }),
                    EntityAlias = "sopart"
                };

                LinkEntity linkSOPartLines = new LinkEntity
                {
                    LinkFromEntityName = "tss_sopartheader",
                    LinkToEntityName = "tss_sopartlines",
                    LinkFromAttributeName = "tss_sopartheaderid",
                    LinkToAttributeName = "tss_sopartheaderid",
                    JoinOperator = JoinOperator.Inner,
                    EntityAlias = "sopartline"
                };

                QueryExpression CRMQExp = new QueryExpression("tss_quotationpartheader");
                CRMQExp.ColumnSet = new ColumnSet(new string[] { "createdon" });
                CRMQExp.LinkEntities.Add(linkQuotePart);
                CRMQExp.LinkEntities.Add(linkSOPart);
                CRMQExp.LinkEntities[0].LinkEntities.Add(linkQuoteService);
                CRMQExp.LinkEntities[1].LinkEntities.Add(linkSOPartLines);
                CRMQExp.LinkEntities[1].LinkEntities[0].LinkCriteria.AddFilter(fe);
                EntityCollection joinsoheader = _orgService.RetrieveMultiple(CRMQExp);

                var quoteServiceDistinct = (from i in joinsoheader.Entities
                                            group i by new
                                            {
                                                service = (string)i.GetAttributeValue<AliasedValue>("service.trs_commercialheader").Value,
                                                quoteserviceid = (Guid)i.GetAttributeValue<AliasedValue>("quoteservice.trs_quotationid").Value
                                            } into g
                                            select new
                                            {
                                                service = g.Key.service,
                                                quoteserviceid = g.Key.quoteserviceid
                                            }).ToList();

                var sohederDistinct = (from i in joinsoheader.Entities
                                       group i by new
                                       {
                                           pss = (EntityReference)i.GetAttributeValue<AliasedValue>("sopart.tss_pss").Value,
                                           branch = (EntityReference)i.GetAttributeValue<AliasedValue>("sopart.tss_branch").Value,
                                           sonumber = (Guid)i.GetAttributeValue<AliasedValue>("sopart.tss_sopartheaderid").Value,
                                           quoteserviceid = (Guid)i.GetAttributeValue<AliasedValue>("quoteservice.trs_quotationid").Value
                                       } into g
                                       select new
                                       {
                                           pss = g.Key.pss,
                                           branch = g.Key.branch,
                                           sonumber = g.Key.sonumber,
                                           quoteserviceid = g.Key.quoteserviceid
                                       }
                          ).ToList();



                #region Setup data
                CRMQExp = new QueryExpression("tss_sparepartsetup");
                CRMQExp.ColumnSet = new ColumnSet(true);
                Entity setup = _orgService.RetrieveMultiple(CRMQExp).Entities.FirstOrDefault();

                int incentivePeriod = setup.GetAttributeValue<int>("tss_incentiveperiod");
                DateTime lastEndDateCalculation = setup.GetAttributeValue<DateTime>("tss_lastdateincentiveperiod").ToLocalTime().AddDays(incentivePeriod);
                if (DateTime.Now.Date < lastEndDateCalculation.Date)
                {
                    throw new Exception("The process cannot contenue");
                }
                else
                {
                    foreach (var o in sohederDistinct)
                    {
                        bool isExistService = quoteServiceDistinct.Any(x => x.quoteserviceid == o.quoteserviceid);
                        if (isExistService)
                        {
                            decimal? incentiveNet = 0, cashIn = 0, f1 = 0, f2 = 0, f3 = 0, f4 = 0, f5 = 0, f6 = 0, f7 = 0, p = 0;
                            DateTime startPeriodVisit = DateTime.Now.AddMonths(-1 * incentivePeriod);
                            FilterExpression f = new FilterExpression(LogicalOperator.And);
                            f.AddCondition("tss_type", ConditionOperator.Equal, 865920000);//tss type=SO
                            f.AddCondition("tss_sonumber", ConditionOperator.Equal, o.sonumber);
                            f.AddCondition("tss_pss", ConditionOperator.Equal, o.pss.Id);
                            f.AddCondition("tss_branch", ConditionOperator.Equal, o.branch.Id);
                            CRMQExp = new QueryExpression("tss_soincentive");
                            CRMQExp.ColumnSet = new ColumnSet(true);
                            CRMQExp.Criteria.AddFilter(f);
                            Entity soIncentive = _orgService.RetrieveMultiple(CRMQExp).Entities.FirstOrDefault();
                            f1 = soIncentive?.GetAttributeValue<decimal>("tss_f1");
                            f2 = soIncentive?.GetAttributeValue<decimal>("tss_f2");
                            f3 = GetF3Factor(_orgService, o.pss, o.branch, startPeriodVisit);
                            f4 = 0;
                            f5 = soIncentive.Attributes.Contains("tss_f5") ? soIncentive?.GetAttributeValue<decimal>("tss_f5") : 0;
                            f6 = GetF6Factor(_orgService, startPeriodVisit) / Helper.Common.DiffDays(startPeriodVisit, DateTime.Now.Date);
                            DateTime startPeriod = lastEndDateCalculation.AddDays(1);
                            Entity entityToInsert = new Entity("tss_incentivefinal");
                            entityToInsert["tss_startperiod"] = startPeriod;
                            entityToInsert["tss_endperiod"] = startPeriod.AddMonths(incentivePeriod);
                            entityToInsert["tss_pss"] = o.pss;
                            entityToInsert["tss_branch"] = o.branch;
                            entityToInsert["tss_f1"] = f1;
                            entityToInsert["tss_f2"] = f2;
                            entityToInsert["tss_f3"] = f3;
                            entityToInsert["tss_f4"] = 0;
                            entityToInsert["tss_f5"] = f5;
                            entityToInsert["tss_f6"] = f6;
                            p = GetPFactor(_orgService, o.sonumber, setup);
                            f7 = p * f1 * f3;
                            entityToInsert["tss_f7"] = f7;
                            cashIn = GetTotalPaidF5DocPayment(o.sonumber, _orgService);

                            incentiveNet = (cashIn * f1 * f2 * f3 * f4 * f5 * f6) - f7;
                            entityToInsert["tss_incentivenet"] = incentiveNet;
                            entityToInsert["tss_netcashin"] = cashIn;
                            entityToInsert["tss_status"] = new OptionSetValue(STATUS_PROCESSED);
                            _orgService.Create(entityToInsert);
                        }
                    }
                }
                #endregion


            }
            catch (Exception ex)
            { throw; }
            Console.WriteLine("Successfully Execute");
        }
        private decimal GetPFactor(IOrganizationService _orgService, Guid sopartHeaderId, Entity setup)
        {
            int AROverduePenalty = setup.GetAttributeValue<int>("tss_aroverduepenalty");
            LinkEntity linkSOPartSubLines = new LinkEntity
            {
                LinkFromEntityName = "tss_sopartlines",
                LinkToEntityName = "tss_salesorderpartsublines",
                LinkFromAttributeName = "tss_sopartlinesid",
                LinkToAttributeName = "tss_salesorderpartlines",
                Columns = new ColumnSet(new string[] { "tss_invoicevalue", "tss_invoiceduedate", "tss_invoiceno" }),
                JoinOperator = JoinOperator.Inner,
                EntityAlias = "sopartsubline"
            };

            QueryExpression CRMQExp = new QueryExpression("tss_sopartlines");
            CRMQExp.ColumnSet = new ColumnSet(new string[] { "tss_sopartheaderid" });
            CRMQExp.LinkEntities.Add(linkSOPartSubLines);
            CRMQExp.Criteria.AddCondition("tss_sopartheaderid", ConditionOperator.Equal, sopartHeaderId);
            CRMQExp.LinkEntities[0].LinkCriteria.AddCondition("tss_transactiontype", ConditionOperator.Equal, 865920002);//Create Invoice
            CRMQExp.LinkEntities[0].LinkCriteria.AddCondition("tss_invoiceduedate", ConditionOperator.NotNull);
            EntityCollection invoice = _orgService.RetrieveMultiple(CRMQExp);

            linkSOPartSubLines = new LinkEntity
            {
                LinkFromEntityName = "tss_sopartlines",
                LinkToEntityName = "tss_salesorderpartsublines",
                LinkFromAttributeName = "tss_sopartlinesid",
                LinkToAttributeName = "tss_salesorderpartlines",
                Columns = new ColumnSet(new string[] { "tss_paidvalue", "tss_paiddate", "tss_invoiceno" }),
                JoinOperator = JoinOperator.Inner,
                EntityAlias = "sopartsubline"
            };

            CRMQExp = new QueryExpression("tss_sopartlines");
            CRMQExp.ColumnSet = new ColumnSet(new string[] { "tss_sopartheaderid" });
            CRMQExp.LinkEntities.Add(linkSOPartSubLines);
            CRMQExp.Criteria.AddCondition("tss_sopartheaderid", ConditionOperator.Equal, sopartHeaderId);
            CRMQExp.LinkEntities[0].LinkCriteria.AddCondition("tss_transactiontype", ConditionOperator.Equal, 865920006);//Create Invoice
            CRMQExp.LinkEntities[0].LinkCriteria.AddCondition("tss_paiddate", ConditionOperator.NotNull);
            EntityCollection payment = _orgService.RetrieveMultiple(CRMQExp);

            List<string> invoiceno = payment.Entities.Select(x => (string)x.GetAttributeValue<AliasedValue>("sopartsubline.tss_invoiceno").Value).ToList();
            var invoiceFiltered = invoice.Entities.Where(x => invoiceno.Contains((string)x.GetAttributeValue<AliasedValue>("sopartsubline.tss_invoiceno").Value)).ToList();
            decimal totalPFactor = 0;
            foreach (var o in payment.Entities)
            {
                DateTime paidDate = (DateTime)o.GetAttributeValue<AliasedValue>("sopartsubline.tss_paiddate").Value;
                var val = invoiceFiltered
                    .Where(x => x.GetAttributeValue<EntityReference>("tss_sopartheaderid").Id == o.GetAttributeValue<EntityReference>("tss_sopartheaderid").Id
                     && ((DateTime)x.GetAttributeValue<AliasedValue>("sopartsubline.tss_invoiceduedate").Value).AddDays(AROverduePenalty) < paidDate)
                    .Sum(m => ((Money)m.GetAttributeValue<AliasedValue>("sopartsubline.tss_invoicevalue").Value).Value);
                totalPFactor = totalPFactor + invoiceFiltered
                    .Where(x => x.GetAttributeValue<EntityReference>("tss_sopartheaderid").Id == o.GetAttributeValue<EntityReference>("tss_sopartheaderid").Id
                     && ((DateTime)x.GetAttributeValue<AliasedValue>("sopartsubline.tss_invoiceduedate").Value).AddDays(AROverduePenalty) < paidDate)
                    .Sum(m => ((Money)m.GetAttributeValue<AliasedValue>("sopartsubline.tss_invoicevalue").Value).Value);
            }
            return totalPFactor;
        }
        private decimal GetF3Factor(IOrganizationService _orgService, EntityReference pss, EntityReference branch, DateTime startPeriod)
        {

            QueryExpression CRMQExp = new QueryExpression("tss_salestargetpss")
            {
                ColumnSet = new ColumnSet(true),
                Criteria =
                {
                    FilterOperator=LogicalOperator.And,
                    Conditions=
                    {
                     new ConditionExpression("tss_pss",ConditionOperator.Equal, pss.Id),
                     new ConditionExpression("tss_branch",ConditionOperator.Equal, branch.Id),
                     new ConditionExpression("tss_status",ConditionOperator.Equal, 865920003)
                    }
                }
            };

            Entity salesTarget = _orgService.RetrieveMultiple(CRMQExp).Entities.FirstOrDefault();
            decimal totalAllSales = salesTarget?.GetAttributeValue<Money>("tss_totalallsalesyearly").Value ?? 0;

            LinkEntity linkSOPartLines = new LinkEntity
            {
                LinkFromEntityName = "tss_sopartheader",
                LinkToEntityName = "tss_sopartlines",
                LinkFromAttributeName = "tss_sopartheaderid",
                LinkToAttributeName = "tss_sopartheaderid",
                Columns = new ColumnSet(new string[] { "tss_sopartheaderid" }),
                JoinOperator = JoinOperator.Inner,
                EntityAlias = "sopartline"
            };

            LinkEntity linkSOPartSubLines = new LinkEntity
            {
                LinkFromEntityName = "tss_sopartlines",
                LinkToEntityName = "tss_salesorderpartsublines",
                LinkFromAttributeName = "tss_sopartlinesid",
                LinkToAttributeName = "tss_salesorderpartlines",
                Columns = new ColumnSet(new string[] { "tss_invoicevalue" }),
                JoinOperator = JoinOperator.Inner,
                EntityAlias = "sopartsubline"
            };

            CRMQExp = new QueryExpression("tss_sopartheader");
            CRMQExp.LinkEntities.Add(linkSOPartLines);
            CRMQExp.LinkEntities[0].LinkEntities.Add(linkSOPartSubLines);
            CRMQExp.LinkEntities[0].LinkEntities[0].LinkCriteria.AddCondition("tss_invoicedate", ConditionOperator.GreaterThan, startPeriod);
            CRMQExp.LinkEntities[0].LinkEntities[0].LinkCriteria.AddCondition("tss_invoicedate", ConditionOperator.LessThan, DateTime.Now.Date);
            CRMQExp.LinkEntities[0].LinkEntities[0].LinkCriteria.AddCondition("tss_transactiontype", ConditionOperator.Equal, 865920002);//Create Invoice
            EntityCollection soPartHeader = _orgService.RetrieveMultiple(CRMQExp);

            decimal totalSOPartHeader = soPartHeader.Entities.Sum(x => ((Money)(x.GetAttributeValue<AliasedValue>("sopartsubline.tss_invoicevalue").Value)).Value);
            decimal achievement = totalSOPartHeader == 0 ? 0 : (totalAllSales / totalSOPartHeader) * 100;
            CRMQExp = new QueryExpression("tss_incentivef3achievement")
            {

                ColumnSet = new ColumnSet(true),
                Criteria =
                {
                    FilterOperator=LogicalOperator.And,
                    Conditions=
                    {
                    new ConditionExpression("tss_startachievement",ConditionOperator.GreaterEqual,(int)Math.Round(achievement)),
                    new ConditionExpression("tss_endachievement",ConditionOperator.GreaterEqual,(int)Math.Round(achievement))
                    }
                }
            };
            Entity entityAchievement = _orgService.RetrieveMultiple(CRMQExp).Entities.FirstOrDefault();
            decimal result = 0;

            if (entityAchievement != null)
                result = (bool)entityAchievement?.Attributes.Contains("tss_factor") ? entityAchievement?.GetAttributeValue<decimal>("tss_factor") ?? 0 : 0;


            return result;
        }
        private decimal GetF6Factor(IOrganizationService _orgService, DateTime startPeriodVisit)
        {
            QueryExpression CRMQExp = new QueryExpression("tss_partactivityheader");
            CRMQExp.ColumnSet = new ColumnSet(new string[] { "tss_actualdate" });
            CRMQExp.Criteria.AddCondition("tss_actualdate", ConditionOperator.GreaterEqual, startPeriodVisit);
            CRMQExp.Criteria.AddCondition("tss_actualdate", ConditionOperator.LessEqual, DateTime.Now.Date);
            CRMQExp.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            EntityCollection entityPartVisit = _orgService.RetrieveMultiple(CRMQExp);

            var groupActualVisit = (from i in entityPartVisit.Entities

                                    group i by new
                                    {
                                        visitdate = (EntityReference)i.GetAttributeValue<AliasedValue>("scheduledstart").Value
                                    } into g
                                    select new
                                    {
                                        visitdate = g.Key.visitdate,
                                        visitPerDay = g.Count()
                                    }
                                    ).ToList();

            int totalDays = groupActualVisit.Count;
            int totalVisit = groupActualVisit.Sum(x => x.visitPerDay);
            decimal avgVisitPerDays = totalDays == 0 ? 0 : ((decimal)totalVisit / (decimal)totalDays);
            /*(E)865.920.000
             * (M)865.920.001
             * (NI)865.920.002
             * (IC)865.920.003
             */

            int SalesSystemType = 865920003;
            if (avgVisitPerDays == 1)
                SalesSystemType = 865920002;
            if (avgVisitPerDays == 2)
                SalesSystemType = 865920001;
            if (avgVisitPerDays > 2)
                SalesSystemType = 865920000;


            CRMQExp = new QueryExpression("tss_incentivef6salessystemfactor")
            {
                ColumnSet = new ColumnSet(true),
                Criteria =
                                {
                                    FilterOperator=LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("tss_salessystemtype",ConditionOperator.Equal,SalesSystemType)
                                    }
                                }
            };

            Entity entityF6Factor = _orgService.RetrieveMultiple(CRMQExp).Entities.FirstOrDefault();
            decimal totalFactor = 0;
            if ((bool)entityF6Factor?.Attributes.Contains("tss_factor"))
                totalFactor = entityF6Factor?.GetAttributeValue<decimal>("tss_factor") ?? decimal.Zero;

            return totalFactor;
        }
        private decimal GetTotalPaidF5DocPayment(Guid sopartHeaderId, IOrganizationService crmOrgCervice)
        {

            LinkEntity linkQuotePart = new LinkEntity
            {
                LinkFromEntityName = "tss_soincentive",
                LinkToEntityName = "tss_documentpaymentf5",
                LinkFromAttributeName = "tss_sonumber",
                LinkToAttributeName = "tss_sonumber",
                JoinOperator = JoinOperator.Inner,
                EntityAlias = "docf5"
            };

            LinkEntity linkQuoteService = new LinkEntity
            {
                LinkFromEntityName = "tss_documentpaymentf5",
                LinkToEntityName = "tss_documentpaymentf5lines",
                LinkFromAttributeName = "tss_documentpaymentf5id",
                LinkToAttributeName = "tss_documentpaymentf5id",
                JoinOperator = JoinOperator.Inner,
                Columns = new ColumnSet(new string[] { "tss_f5", "tss_paidamount" }),
                EntityAlias = "docf5detail"
            };
            QueryExpression q = new QueryExpression("tss_soincentive");
            q.ColumnSet = new ColumnSet(new string[] { "tss_pss", "tss_sonumber" });
            q.LinkEntities.Add(linkQuotePart);
            q.LinkEntities[0].LinkEntities.Add(linkQuoteService);
            q.Criteria.AddCondition("tss_sonumber", ConditionOperator.Equal, sopartHeaderId);
            EntityCollection entityMultyRetrieve = crmOrgCervice.RetrieveMultiple(q);

            var docf5Details = (from i in entityMultyRetrieve.Entities
                                group i by new
                                {
                                    pss = i.GetAttributeValue<EntityReference>("tss_pss").Id,
                                    sonumber = i.GetAttributeValue<EntityReference>("tss_sonumber").Id
                                } into g
                                select new
                                {
                                    pss = g.Key.pss,
                                    sonumber = g.Key.sonumber,
                                    paidValue = g.Sum(o => ((Money)o.GetAttributeValue<AliasedValue>("docf5detail.tss_paidamount").Value).Value)
                                }
                      ).ToList().FirstOrDefault();
            decimal result = docf5Details?.paidValue ?? decimal.Zero;
            return result;
        }
    }
}
