using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrakNusSparepartSystem.DataLayer;


using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using TrakNusSparepartSystem.Helper;

namespace TrakNusSparepartSystem.WorkflowActivity.Business_Layer
{
    public class BL_tss_totalpartconsumpmarketsize
    {
        #region Constant
        private const int STATUS_COMPLETED_MS = 865920000;
        private const int REVISION = 865920001;

        private const int MTD1 = 865920000;
        private const int MTD2 = 865920001;
        private const int MTD3 = 865920002;
        private const int MTD4 = 865920003;
        private const int MTD5 = 865920004;
        #endregion

        #region dependencies
        DL_tss_keyaccount _DL_tss_keyaccount = new DL_tss_keyaccount();
        DL_tss_marketsizeresultpss _DL_tss_marketsizeresultpss = new DL_tss_marketsizeresultpss();
        DL_tss_mastermarketsize _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        DL_tss_mastermarketsizelines _DL_tss_mastermarketsizelines = new DL_tss_mastermarketsizelines();
        DL_tss_mastermarketsizesublines _DL_tss_mastermarketsizesublines = new DL_tss_mastermarketsizesublines();
        DL_tss_sparepartsetup _DL_tss_sparepartsetup = new DL_tss_sparepartsetup();
        DL_tss_totalpartconsumpmarketsize _DL_tss_totalpartconsumpmarketsize = new DL_tss_totalpartconsumpmarketsize();
        RetrieveHelper _retrievehelper = new RetrieveHelper();

        #endregion

        //public void GenerateTotalPartConsumpMarketSize(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        //{
        //    DateTime startDtMS = DateTime.MinValue;
        //    DateTime endDtMS = DateTime.MinValue;


        //    #region Get all Market Size PSS in current periode 

        //    FilterExpression fpss = new FilterExpression(LogicalOperator.And);
        //    //2018.09.17 - start = lessequal & end = greaterequal
        //    //fpss.AddCondition("tss_activeperiodsend", ConditionOperator.LessEqual, DateTime.Now);
        //    //fpss.AddCondition("tss_activeperiodstart", ConditionOperator.GreaterEqual, DateTime.Now);
        //    fpss.AddCondition("tss_activeperiodsend", ConditionOperator.GreaterEqual, DateTime.Now);
        //    fpss.AddCondition("tss_activeperiodstart", ConditionOperator.LessEqual, DateTime.Now);

        //    QueryExpression qpss = new QueryExpression("tss_marketsizeresultpss");
        //    qpss.Criteria.AddFilter(fpss);
        //    qpss.ColumnSet = new ColumnSet(true);
        //    EntityCollection ENC_tss_marketsizeresultpss = _DL_tss_marketsizeresultpss.Select(organizationService, qpss);

        //    #endregion

        //    #region get Part Setup
        //    //Get SparePart Config 

        //    FilterExpression fSetup = new FilterExpression(LogicalOperator.And);
        //    fSetup.AddCondition("tss_name", ConditionOperator.Equal, "TSS");

        //    QueryExpression qSetup = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
        //    qSetup.ColumnSet = new ColumnSet(true);
        //    qSetup.Criteria.AddFilter(fSetup);
        //    EntityCollection setups = _DL_tss_sparepartsetup.Select(organizationService, qSetup);
        //    #endregion

        //    foreach (Entity en_MarketSizeResultPSS in ENC_tss_marketsizeresultpss.Entities)
        //    {
        //        #region getCustomer
        //        FilterExpression fMs = new FilterExpression(LogicalOperator.And);
        //        fMs.AddCondition("tss_unittype", ConditionOperator.NotNull);
        //        fMs.AddCondition("tss_pss", ConditionOperator.Equal, en_MarketSizeResultPSS.GetAttributeValue<EntityReference>("tss_pss").Id);
        //        fMs.AddCondition("tss_status", ConditionOperator.Equal, STATUS_COMPLETED_MS);

        //        QueryExpression qMS = new QueryExpression(_DL_tss_mastermarketsize.EntityName);
        //        qMS.Criteria.AddFilter(fMs);
        //        qMS.ColumnSet = new ColumnSet(true);
        //        EntityCollection ms = _DL_tss_mastermarketsize.Select(organizationService, qMS);
        //        Guid[] custs = ms.Entities.GroupBy(x => x.GetAttributeValue<EntityReference>("tss_customer").Id).Select(x => x.First().GetAttributeValue<EntityReference>("tss_customer").Id).ToArray();
        //        #endregion

        //        #region Check if Revised
        //        //Check if revised on KA
        //        for (var i = 0; i < custs.Count(); i++)
        //        {

        //            FilterExpression fKA = new FilterExpression(LogicalOperator.And);
        //            fKA.AddCondition("tss_pss", ConditionOperator.Equal, en_MarketSizeResultPSS.GetAttributeValue<EntityReference>("tss_pss").Id);
        //            fKA.AddCondition("tss_customer", ConditionOperator.Equal, custs[i]);

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
        //        }
        //        #endregion



        //        //get Master Market Size
        //        Entity en_MasterMarketSize = new Entity("tss_mastermarketsize", en_MarketSizeResultPSS.GetAttributeValue<EntityReference>("tss_marketsizeid").Id);

        //        #region get Master Market Size Line
        //        FilterExpression fMMSZL = new FilterExpression(LogicalOperator.And);
        //        fMMSZL.AddCondition("tss_mastermarketsizeref", ConditionOperator.Equal, en_MarketSizeResultPSS.GetAttributeValue<EntityReference>("tss_marketsizeid").Id);

        //        QueryExpression qMMZL = new QueryExpression(_DL_tss_mastermarketsize.EntityName);
        //        qSetup.ColumnSet = new ColumnSet(true);
        //        qSetup.Criteria.AddFilter(fMMSZL);
        //        EntityCollection enc_MasterMarketSizeLine = _DL_tss_mastermarketsizelines.Select(organizationService, qMMZL);

        //        #endregion

        //        #region get Master Market Size Sub Line 

        //        foreach (Entity en_MasterMarketSizeLine in enc_MasterMarketSizeLine.Entities)
        //        {
        //            FilterExpression fMMSZSL = new FilterExpression(LogicalOperator.And);
        //            fMMSZSL.AddCondition("tss_mastermslinesref", ConditionOperator.Equal, en_MasterMarketSizeLine.GetAttributeValue<EntityReference>("tss_mastermarketsizelinesid").Id);

        //            QueryExpression qMMZSL = new QueryExpression(_DL_tss_mastermarketsize.EntityName);
        //            qMMZSL.ColumnSet = new ColumnSet(true);
        //            qMMZSL.Criteria.AddFilter(fMMSZSL);
        //            EntityCollection enc_MasterMarketSizeSubLine = _DL_tss_mastermarketsizesublines.Select(organizationService, qMMZSL);

        //            #region calculate total Amount

        //            foreach (Entity en_MasterMarketSizeSubLine in enc_MasterMarketSizeSubLine.Entities)
        //            {
        //                Entity en_TotalPartConsumpMarketSize = new Entity(_DL_tss_totalpartconsumpmarketsize.EntityName);

        //                en_TotalPartConsumpMarketSize["tss_marketsizeid"] = en_MarketSizeResultPSS.GetAttributeValue<EntityReference>("tss_marketsizeid").Id;
        //                en_TotalPartConsumpMarketSize["tss_customer"] = en_MasterMarketSize.GetAttributeValue<EntityReference>("tss_customer").Id;
        //                en_TotalPartConsumpMarketSize["tss_pss"] = en_MarketSizeResultPSS.GetAttributeValue<EntityReference>("tss_pss").Id;
        //                en_TotalPartConsumpMarketSize["tss_msperiodend"] = en_MarketSizeResultPSS.GetAttributeValue<DateTime>("tss_msperiodend");
        //                en_TotalPartConsumpMarketSize["tss_msperiodstart"] = en_MarketSizeResultPSS.GetAttributeValue<DateTime>("tss_msperiodstart");
        //                if (startDtMS != DateTime.MinValue)
        //                    en_TotalPartConsumpMarketSize["tss_activeperiodstart"] = startDtMS;
        //                if (endDtMS != DateTime.MinValue)
        //                    en_TotalPartConsumpMarketSize["tss_activeperiodsend"] = endDtMS;
        //                en_TotalPartConsumpMarketSize["tss_partnumber"] = en_MasterMarketSizeSubLine.GetAttributeValue<EntityReference>("tss_partnumber").Id;
        //               en_TotalPartConsumpMarketSize["tss_qty"] = en_MasterMarketSizeSubLine.GetAttributeValue<int>("tss_qty");

        //                organizationService.Create(en_TotalPartConsumpMarketSize);
        //            }

        //            #endregion
        //        }

        //        #endregion


        //    }

        //}

        public void GenerateTotalPartConsumpMarketSize(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {
            int quantity;
            DateTime startDtMS = DateTime.MinValue;
            DateTime endDtMS = DateTime.MinValue;

            FilterExpression fMSRPSS = new FilterExpression(LogicalOperator.And);
            fMSRPSS.AddCondition("tss_activeperiodstart", ConditionOperator.LessEqual, DateTime.Now);
            fMSRPSS.AddCondition("tss_activeperiodsend", ConditionOperator.GreaterEqual, DateTime.Now);
            fMSRPSS.AddCondition("tss_pss", ConditionOperator.Equal, context.UserId);
            //fMSRPSS.AddCondition("tss_unittype", ConditionOperator.NotNull);
            fMSRPSS.AddCondition("tss_status", ConditionOperator.Equal, 865920005);

            QueryExpression qMSRPSS = new QueryExpression(_DL_tss_marketsizeresultpss.EntityName);
            qMSRPSS.Criteria.AddFilter(fMSRPSS);
            qMSRPSS.ColumnSet = new ColumnSet(true);

            EntityCollection marketSizeResulPSSQ = _DL_tss_marketsizeresultpss.Select(organizationService, qMSRPSS);
            
            foreach (Entity marketSizeResulPSS in marketSizeResulPSSQ.Entities)
            {
                #region GET PART SETUP
                FilterExpression fSetup = new FilterExpression(LogicalOperator.And);
                fSetup.AddCondition("tss_name", ConditionOperator.Equal, "TSS");

                QueryExpression qSetup = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
                qSetup.ColumnSet = new ColumnSet(true);
                qSetup.Criteria.AddFilter(fSetup);
                EntityCollection setups = _DL_tss_sparepartsetup.Select(organizationService, qSetup);
                #endregion

                #region CHECK IF REVISED
                FilterExpression fKA = new FilterExpression(LogicalOperator.And);
                fKA.AddCondition("tss_pss", ConditionOperator.Equal, marketSizeResulPSS.GetAttributeValue<EntityReference>("tss_pss").Id);
                fKA.AddCondition("tss_customer", ConditionOperator.Equal, marketSizeResulPSS.GetAttributeValue<EntityReference>("tss_customer").Id);

                QueryExpression qKA = new QueryExpression(_DL_tss_keyaccount.EntityName);
                qKA.Criteria.AddFilter(fKA);
                qKA.ColumnSet.AddColumn("tss_version");
                qKA.AddOrder("tss_revision", OrderType.Descending);
                EntityCollection ka = _DL_tss_keyaccount.Select(organizationService, qKA);

                Entity _keyaccount = null;

                if (ka.Entities.Count > 0 && setups.Entities.Count > 0)
                {
                    _keyaccount = ka[0];

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
                #endregion

                FilterExpression fExpresion = new FilterExpression(LogicalOperator.And);
                fExpresion.AddCondition("tss_activeperiodstart", ConditionOperator.LessEqual, DateTime.Now);
                fExpresion.AddCondition("tss_activeperiodsend", ConditionOperator.GreaterEqual, DateTime.Now);
                fExpresion.AddCondition("tss_pss", ConditionOperator.Equal, marketSizeResulPSS.GetAttributeValue<EntityReference>("tss_pss").Id);
                fExpresion.AddCondition("tss_customer", ConditionOperator.Equal, marketSizeResulPSS.GetAttributeValue<EntityReference>("tss_customer").Id);
                fExpresion.AddCondition("tss_status", ConditionOperator.Equal, STATUS_COMPLETED_MS);

                QueryExpression qExpresion = new QueryExpression(_DL_tss_mastermarketsize.EntityName);
                qExpresion.Criteria.AddFilter(fExpresion);
                qExpresion.ColumnSet = new ColumnSet(true);

                List<Entity> mMarketSize = _retrievehelper.RetrieveMultiple(organizationService, qExpresion); // _DL_tss_mastermarketsize.Select(organizationService, qExpresion);

                object[] marketSizeId = mMarketSize.Select(x => (object)x.Id).ToArray();

                FilterExpression a1 = new FilterExpression(LogicalOperator.Or);
                a1.AddCondition("tss_methodcalculationused", ConditionOperator.Equal, MTD1);
                a1.AddCondition("tss_methodcalculationused", ConditionOperator.Equal, MTD2);
                a1.AddCondition("tss_methodcalculationused", ConditionOperator.Equal, MTD3);

                FilterExpression a2 = new FilterExpression(LogicalOperator.And);
                a2.AddFilter(a1);
                a2.AddCondition("tss_duedate", ConditionOperator.NotNull);

                FilterExpression a3 = new FilterExpression(LogicalOperator.Or);
                a3.AddFilter(a2);
                a3.AddCondition("tss_methodcalculationused", ConditionOperator.Equal, MTD4);
                a3.AddCondition("tss_methodcalculationused", ConditionOperator.Equal, MTD5);

                //foreach (Entity entityMS in mMarketSize.Entities)
                //{
                qExpresion = new QueryExpression(_DL_tss_mastermarketsizelines.EntityName);
                LinkEntity linkToSubline = new LinkEntity
                {
                    LinkFromEntityName = _DL_tss_mastermarketsizelines.EntityName,
                    LinkToEntityName = _DL_tss_mastermarketsizesublines.EntityName,
                    LinkFromAttributeName = "tss_mastermarketsizelinesid",
                    LinkToAttributeName = "tss_mastermslinesref",
                    Columns = new ColumnSet(true),
                    EntityAlias = "mastermarketsizesubline",
                    JoinOperator = JoinOperator.Inner

                };
                qExpresion.LinkEntities.Add(linkToSubline);
                qExpresion.Criteria.AddCondition("tss_mastermarketsizeref", ConditionOperator.In, marketSizeId);
                qExpresion.Criteria.AddFilter(a3);
                //qExpresion.Criteria.AddCondition("tss_duedate", ConditionOperator.NotNull);
                qExpresion.ColumnSet = new ColumnSet(true);

                List<Entity> msLines = _retrievehelper.RetrieveMultiple(organizationService, qExpresion); // _DL_tss_mastermarketsizelines.Select(organizationService, qExpresion);

                // PART CONSUMP KA-UIO & KA Non UIO
                if (msLines.Count > 0)
                {
                    var groupByQty = (from r in msLines.AsEnumerable()
                                        group r by new
                                        {
                                            groupByPartNumber = (EntityReference)r.GetAttributeValue<AliasedValue>("mastermarketsizesubline.tss_partnumber").Value
                                        } into g
                                        select new
                                        {
                                            partNumber = g.Key.groupByPartNumber,
                                            sumAmount = g.Sum(x => (Convert.ToDecimal(x.GetAttributeValue<AliasedValue>("mastermarketsizesubline.tss_qty").Value)))
                                        }).ToList();

                    foreach (var o in groupByQty)
                    {
                        FilterExpression fExist = new FilterExpression(LogicalOperator.And);
                        fExist.AddCondition("tss_pss", ConditionOperator.Equal, marketSizeResulPSS.GetAttributeValue<EntityReference>("tss_pss").Id);
                        fExist.AddCondition("tss_customer", ConditionOperator.Equal, marketSizeResulPSS.GetAttributeValue<EntityReference>("tss_customer").Id);
                        fExist.AddCondition("tss_partnumber", ConditionOperator.Equal, o.partNumber.Id);
                        fExist.AddCondition("tss_marketsizeid", ConditionOperator.Equal, marketSizeResulPSS.Id);

                        QueryExpression qExist = new QueryExpression(_DL_tss_totalpartconsumpmarketsize.EntityName);
                        qExist.Criteria.AddFilter(fExist);
                        qExist.ColumnSet = new ColumnSet(true);

                        EntityCollection totalPartConsumpQ = _DL_tss_totalpartconsumpmarketsize.Select(organizationService, qExist);

                        if (totalPartConsumpQ.Entities.Count() == 0)
                        {
                            Entity totalPartConsump = new Entity(_DL_tss_totalpartconsumpmarketsize.EntityName);

                            totalPartConsump["tss_marketsizeid"] = new EntityReference("tss_marketsizeresultpss", marketSizeResulPSS.Id);
                            totalPartConsump["tss_pss"] = new EntityReference("systemuser", marketSizeResulPSS.GetAttributeValue<EntityReference>("tss_pss").Id);
                            totalPartConsump["tss_customer"] = new EntityReference("account", marketSizeResulPSS.GetAttributeValue<EntityReference>("tss_customer").Id);
                            totalPartConsump["tss_partnumber"] = new EntityReference("trs_masterpart", o.partNumber.Id);
                            totalPartConsump["tss_qty"] = o.sumAmount;
                            totalPartConsump["tss_remainingqty"] = o.sumAmount;
                            totalPartConsump["tss_remainingqtyactual"] = o.sumAmount;

                            totalPartConsump["tss_activeenddate"] = marketSizeResulPSS.GetAttributeValue<DateTime>("tss_activeperiodsend");
                            totalPartConsump["tss_activestartdate"] = marketSizeResulPSS.GetAttributeValue<DateTime>("tss_activeperiodstart");
                            if (startDtMS != DateTime.MinValue)
                                totalPartConsump["tss_msstartdate"] = startDtMS;
                            if (endDtMS != DateTime.MinValue)
                                totalPartConsump["tss_msenddate"] = endDtMS;

                            organizationService.Create(totalPartConsump);
                        }
                        else
                        {
                            Entity entityupdate = new Entity(_DL_tss_totalpartconsumpmarketsize.EntityName);
                            entityupdate.Id = totalPartConsumpQ.Entities[0].Id;

                            entityupdate["tss_marketsizeid"] = new EntityReference("tss_marketsizeresultpss", marketSizeResulPSS.Id);
                            entityupdate["tss_pss"] = new EntityReference("systemuser", marketSizeResulPSS.GetAttributeValue<EntityReference>("tss_pss").Id);
                            entityupdate["tss_customer"] = new EntityReference("account", marketSizeResulPSS.GetAttributeValue<EntityReference>("tss_customer").Id);
                            entityupdate["tss_partnumber"] = new EntityReference("trs_masterpart", o.partNumber.Id);
                            entityupdate["tss_qty"] = o.sumAmount;
                            entityupdate["tss_remainingqty"] = o.sumAmount;
                            entityupdate["tss_remainingqtyactual"] = o.sumAmount;

                            organizationService.Update(entityupdate);
                        }
                    }
                }
                //else
                //{
                //    qExpresion = new QueryExpression(_DL_tss_mastermarketsizesublines.EntityName);
                //    qExpresion.Criteria.AddCondition("tss_mastermarketsizeid", ConditionOperator.In, marketSizeId);
                //    qExpresion.ColumnSet = new ColumnSet(true);

                //    EntityCollection msSubLines = _DL_tss_mastermarketsizesublines.Select
                //        (organizationService, qExpresion);

                //    if(msSubLines.Entities.Count > 0)
                //    {
                //        var groupByQty = (from r in msSubLines.Entities.AsEnumerable()
                //                          group r by new
                //                          {
                //                              groupByPartNumber = r.GetAttributeValue<EntityReference>("tss_partnumber").Id
                //                          } into g
                //                          select new
                //                          {
                //                              partNumber = g.Key.groupByPartNumber,
                //                              sumAmount = g.Sum(x => (Convert.ToDecimal(x.GetAttributeValue<int>("tss_qty"))))
                //                          }).ToList();

                //        foreach (var o in groupByQty)
                //        {
                //            FilterExpression fExist = new FilterExpression(LogicalOperator.And);
                //            fExist.AddCondition("tss_pss", ConditionOperator.Equal, marketSizeResulPSS.GetAttributeValue<EntityReference>("tss_pss").Id);
                //            fExist.AddCondition("tss_customer", ConditionOperator.Equal, marketSizeResulPSS.GetAttributeValue<EntityReference>("tss_customer").Id);
                //            fExist.AddCondition("tss_partnumber", ConditionOperator.Equal, o.partNumber);

                //            QueryExpression qExist = new QueryExpression(_DL_tss_totalpartconsumpmarketsize.EntityName);
                //            qExist.Criteria.AddFilter(fExist);
                //            qExist.ColumnSet = new ColumnSet(true);

                //            EntityCollection totalPartConsumpQ = _DL_tss_totalpartconsumpmarketsize.Select(organizationService, qExist);

                //            if (totalPartConsumpQ.Entities.Count() == 0)
                //            {
                //                Entity totalPartConsump = new Entity(_DL_tss_totalpartconsumpmarketsize.EntityName);

                //                totalPartConsump["tss_marketsizeid"] = new EntityReference("tss_marketsizeresultpss", marketSizeResulPSS.Id);
                //                totalPartConsump["tss_pss"] = new EntityReference("systemuser", marketSizeResulPSS.GetAttributeValue<EntityReference>("tss_pss").Id);
                //                totalPartConsump["tss_customer"] = new EntityReference("account", marketSizeResulPSS.GetAttributeValue<EntityReference>("tss_customer").Id);
                //                totalPartConsump["tss_partnumber"] = new EntityReference("trs_masterpart", o.partNumber);
                //                totalPartConsump["tss_qty"] = o.sumAmount;
                //                totalPartConsump["tss_remainingqty"] = o.sumAmount;
                //                totalPartConsump["tss_remainingqtyactual"] = o.sumAmount;

                //                totalPartConsump["tss_activeenddate"] = marketSizeResulPSS.GetAttributeValue<DateTime>("tss_activeperiodsend");
                //                totalPartConsump["tss_activestartdate"] = marketSizeResulPSS.GetAttributeValue<DateTime>("tss_activeperiodstart");
                //                if (startDtMS != DateTime.MinValue)
                //                    totalPartConsump["tss_msstartdate"] = startDtMS;
                //                if (endDtMS != DateTime.MinValue)
                //                    totalPartConsump["tss_msenddate"] = endDtMS;

                //                organizationService.Create(totalPartConsump);
                //            }
                //            else
                //            {
                //                Entity entityupdate = new Entity(_DL_tss_totalpartconsumpmarketsize.EntityName);
                //                entityupdate.Id = totalPartConsumpQ.Entities[0].Id;

                //                entityupdate["tss_marketsizeid"] = new EntityReference("tss_marketsizeresultpss", marketSizeResulPSS.Id);
                //                entityupdate["tss_pss"] = new EntityReference("systemuser", marketSizeResulPSS.GetAttributeValue<EntityReference>("tss_pss").Id);
                //                entityupdate["tss_customer"] = new EntityReference("account", marketSizeResulPSS.GetAttributeValue<EntityReference>("tss_customer").Id);
                //                entityupdate["tss_partnumber"] = new EntityReference("trs_masterpart", o.partNumber);
                //                entityupdate["tss_qty"] = o.sumAmount;
                //                entityupdate["tss_remainingqty"] = o.sumAmount;
                //                entityupdate["tss_remainingqtyactual"] = o.sumAmount;


                //                organizationService.Update(entityupdate);
                //            }
                //        }
                //    }
                //}
                //}

                // GROUP UIO COMMODITY
                qExpresion = new QueryExpression(_DL_tss_mastermarketsizesublines.EntityName);
                qExpresion.Criteria.AddCondition("tss_mastermarketsizeid", ConditionOperator.In, marketSizeId);
                qExpresion.ColumnSet = new ColumnSet(true);

                List<Entity> msSubLines = _retrievehelper.RetrieveMultiple(organizationService, qExpresion); // _DL_tss_mastermarketsizesublines.Select(organizationService, qExpresion);

                if (msSubLines.Count > 0)
                {
                    var groupByQty = (from r in msSubLines.AsEnumerable()
                                      group r by new
                                      {
                                          groupByPartNumber = r.GetAttributeValue<EntityReference>("tss_partnumber").Id
                                      } into g
                                      select new
                                      {
                                          partNumber = g.Key.groupByPartNumber,
                                          sumAmount = g.Sum(x => (Convert.ToDecimal(x.GetAttributeValue<int>("tss_qty"))))
                                      }).ToList();

                    foreach (var o in groupByQty)
                    {
                        FilterExpression fExist = new FilterExpression(LogicalOperator.And);
                        fExist.AddCondition("tss_pss", ConditionOperator.Equal, marketSizeResulPSS.GetAttributeValue<EntityReference>("tss_pss").Id);
                        fExist.AddCondition("tss_customer", ConditionOperator.Equal, marketSizeResulPSS.GetAttributeValue<EntityReference>("tss_customer").Id);
                        fExist.AddCondition("tss_partnumber", ConditionOperator.Equal, o.partNumber);

                        QueryExpression qExist = new QueryExpression(_DL_tss_totalpartconsumpmarketsize.EntityName);
                        qExist.Criteria.AddFilter(fExist);
                        qExist.ColumnSet = new ColumnSet(true);

                        EntityCollection totalPartConsumpQ = _DL_tss_totalpartconsumpmarketsize.Select(organizationService, qExist);

                        if (totalPartConsumpQ.Entities.Count() == 0)
                        {
                            Entity totalPartConsump = new Entity(_DL_tss_totalpartconsumpmarketsize.EntityName);

                            totalPartConsump["tss_marketsizeid"] = new EntityReference("tss_marketsizeresultpss", marketSizeResulPSS.Id);
                            totalPartConsump["tss_pss"] = new EntityReference("systemuser", marketSizeResulPSS.GetAttributeValue<EntityReference>("tss_pss").Id);
                            totalPartConsump["tss_customer"] = new EntityReference("account", marketSizeResulPSS.GetAttributeValue<EntityReference>("tss_customer").Id);
                            totalPartConsump["tss_partnumber"] = new EntityReference("trs_masterpart", o.partNumber);
                            totalPartConsump["tss_qty"] = o.sumAmount;
                            totalPartConsump["tss_remainingqty"] = o.sumAmount;
                            totalPartConsump["tss_remainingqtyactual"] = o.sumAmount;

                            totalPartConsump["tss_activeenddate"] = marketSizeResulPSS.GetAttributeValue<DateTime>("tss_activeperiodsend");
                            totalPartConsump["tss_activestartdate"] = marketSizeResulPSS.GetAttributeValue<DateTime>("tss_activeperiodstart");
                            if (startDtMS != DateTime.MinValue)
                                totalPartConsump["tss_msstartdate"] = startDtMS;
                            if (endDtMS != DateTime.MinValue)
                                totalPartConsump["tss_msenddate"] = endDtMS;

                            organizationService.Create(totalPartConsump);
                        }
                        else
                        {
                            Entity entityupdate = new Entity(_DL_tss_totalpartconsumpmarketsize.EntityName);
                            entityupdate.Id = totalPartConsumpQ.Entities[0].Id;

                            entityupdate["tss_marketsizeid"] = new EntityReference("tss_marketsizeresultpss", marketSizeResulPSS.Id);
                            entityupdate["tss_pss"] = new EntityReference("systemuser", marketSizeResulPSS.GetAttributeValue<EntityReference>("tss_pss").Id);
                            entityupdate["tss_customer"] = new EntityReference("account", marketSizeResulPSS.GetAttributeValue<EntityReference>("tss_customer").Id);
                            entityupdate["tss_partnumber"] = new EntityReference("trs_masterpart", o.partNumber);
                            entityupdate["tss_qty"] = o.sumAmount;
                            entityupdate["tss_remainingqty"] = o.sumAmount;
                            entityupdate["tss_remainingqtyactual"] = o.sumAmount;


                            organizationService.Update(entityupdate);
                        }
                    }
                }







            }
        }

        //public void GenerateTotalPartConsumpMarketSize(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        //{
        //    DateTime startDtMS = DateTime.MinValue;
        //    DateTime endDtMS = DateTime.MinValue;

        //    #region GET MARKET SIZE RESULT IN ACTIVE PERIOD
        //    FilterExpression fMS = new FilterExpression(LogicalOperator.And);
        //    fMS.AddCondition("tss_unittype", ConditionOperator.NotNull);
        //    fMS.AddCondition("tss_status", ConditionOperator.Equal, STATUS_COMPLETED_MS);
        //    //fMarketSizePSS.AddCondition("tss_pss", ConditionOperator.Equal, marketSizePSS.GetAttributeValue<EntityReference>("tss_pss").Id);

        //    QueryExpression qMS = new QueryExpression(_DL_tss_mastermarketsize.EntityName);
        //    qMS.Criteria.AddFilter(fMS);
        //    qMS.ColumnSet = new ColumnSet(true);
        //    EntityCollection marketSize = _DL_tss_mastermarketsize.Select(organizationService, qMS);
        //    #endregion

        //    foreach (var iMSR in marketSize.Entities)
        //    {
        //        #region GET MARKET SIZE RESULT LINE
        //        FilterExpression fMSRL = new FilterExpression(LogicalOperator.And);
        //        fMSRL.AddCondition("tss_mastermarketsizeref", ConditionOperator.Equal, iMSR.Id);

        //        QueryExpression qMSRL = new QueryExpression(_DL_tss_mastermarketsizelines.EntityName);
        //        qMSRL.ColumnSet = new ColumnSet(true);
        //        qMSRL.Criteria.AddFilter(fMSRL);
        //        EntityCollection marketSizeLine = _DL_tss_mastermarketsizelines.Select(organizationService, qMSRL);
        //        #endregion

        //        foreach (var iMSRLine in marketSizeLine.Entities)
        //        {
        //            #region GET MARKET SIZE RESULT SUBLINE
        //            FilterExpression fMSRSL = new FilterExpression(LogicalOperator.And);
        //            fMSRSL.AddCondition("tss_mastermslinesref", ConditionOperator.Equal, iMSRLine.Id);

        //            QueryExpression qMSRSL = new QueryExpression(_DL_tss_mastermarketsizesublines.EntityName);
        //            qMSRSL.ColumnSet = new ColumnSet(true);
        //            qMSRSL.Criteria.AddFilter(fMSRSL);
        //            EntityCollection marketSizeSubLine = _DL_tss_mastermarketsizesublines.Select(organizationService, qMSRSL);
        //            #endregion

        //            #region CALCULATE TOTAL AMOUNT
        //            foreach (Entity iMSRSubLine in marketSizeSubLine.Entities)
        //            {
        //                Entity en_TotalPartConsumpMarketSize = new Entity(_DL_tss_totalpartconsumpmarketsize.EntityName);

        //                //en_TotalPartConsumpMarketSize["tss_marketsizeid"] = new EntityReference(en_MarketSizeResultPSS.LogicalName, en_MarketSizeResultPSS.Id);
        //                en_TotalPartConsumpMarketSize["tss_customer"] = new EntityReference(iMSR.LogicalName, iMSR.GetAttributeValue<EntityReference>("tss_customer").Id);
        //                en_TotalPartConsumpMarketSize["tss_pss"] = new EntityReference(iMSR.LogicalName, iMSR.GetAttributeValue<EntityReference>("tss_pss").Id);
        //                en_TotalPartConsumpMarketSize["tss_partnumber"] = new EntityReference(iMSRSubLine.LogicalName, iMSRSubLine.GetAttributeValue<EntityReference>("tss_partnumber").Id);
        //                en_TotalPartConsumpMarketSize["tss_qty"] = iMSRSubLine.GetAttributeValue<int>("tss_qty");

        //                en_TotalPartConsumpMarketSize["tss_activeenddate"] = iMSR.GetAttributeValue<DateTime>("tss_activeperiodsend");
        //                en_TotalPartConsumpMarketSize["tss_activestartdate"] = iMSR.GetAttributeValue<DateTime>("tss_activeperiodstart");
        //                if (startDtMS != DateTime.MinValue)
        //                    en_TotalPartConsumpMarketSize["tss_msstartdate"] = startDtMS;
        //                if (endDtMS != DateTime.MinValue)
        //                    en_TotalPartConsumpMarketSize["tss_msenddate"] = endDtMS;

        //                organizationService.Create(en_TotalPartConsumpMarketSize);
        //            }
        //            #endregion
        //        }
        //    }
        //}
    }
}
