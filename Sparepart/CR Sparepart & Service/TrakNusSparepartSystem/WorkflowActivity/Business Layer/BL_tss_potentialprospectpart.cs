using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using TrakNusSparepartSystem.DataLayer;
using TrakNusSparepartSystem.Helper;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using CrmEarlyBound;

namespace TrakNusSparepartSystem.WorkflowActivity.Business_Layer
{
    public class BL_tss_potentialprospectpart
    {
        private const int ST_PSS_STATUS_ACTIVE = 865920003;
        private const int REVISION = 865920001;
        private const int STATUS_COMPLETED_MS = 865920000;
        DL_tss_marketsizeresultpss _DL_tss_marketsizeresultpss = new DL_tss_marketsizeresultpss(); DL_tss_potentialprospectpart _DL_tss_potentialprospectpart = new DL_tss_potentialprospectpart();
        DL_tss_salestargetnational _DL_tss_salestargetnational = new DL_tss_salestargetnational();
        DL_tss_salestargetpss _DL_tss_salestargetpss = new DL_tss_salestargetpss();
        DL_tss_marketsizeresultmapping _DL_tss_marketsizeresultmapping = new DL_tss_marketsizeresultmapping();
        DL_tss_mastermarketsize _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        DL_tss_sparepartsetup _DL_tss_sparepartsetup = new DL_tss_sparepartsetup();
        DL_tss_keyaccount _DL_tss_keyaccount = new DL_tss_keyaccount();
        DL_tss_mastermarketsizelines _DL_tss_mastermarketsizelines = new DL_tss_mastermarketsizelines();
        DL_tss_mastermarketsizesublines _DL_tss_mastermarketsizesublines = new DL_tss_mastermarketsizesublines();
        DL_tss_potentialprospectpartlines _DL_tss_potentialprospectpartlines = new DL_tss_potentialprospectpartlines();
        DL_tss_potentialprospectpartsublines _DL_tss_potentialprospectpartsublines = new DL_tss_potentialprospectpartsublines();
        RetrieveHelper _retrievehelper = new RetrieveHelper();

        public void GeneratePotentialProspectPart(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {
            using (CrmServiceContext _context = new CrmServiceContext(organizationService))
            {
                DateTime startDtMS = DateTime.MinValue;
                DateTime endDtMS = DateTime.MinValue;
                decimal tototal = 0m;
                Guid customerId = Guid.Empty;
                Guid customerGroup = Guid.Empty;
                Guid groupCommodity = Guid.Empty;
                Guid serialNumber = Guid.Empty;
                Guid marketSizeId = Guid.Empty;
                DateTime msPeriodStart = DateTime.MinValue;
                DateTime msPeriodEnd = DateTime.MinValue;

                #region get Part Setup
                //Get SparePart Config 
                FilterExpression fSetup = new FilterExpression(LogicalOperator.And);
                fSetup.AddCondition("tss_name", ConditionOperator.Equal, "TSS");

                QueryExpression qSetup = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
                qSetup.ColumnSet = new ColumnSet(true);
                qSetup.Criteria.AddFilter(fSetup);
                EntityCollection setups = _DL_tss_sparepartsetup.Select(organizationService, qSetup);
                #endregion
                msPeriodStart = setups[0].GetAttributeValue<DateTime>("tss_startdatems");
                msPeriodEnd = setups[0].GetAttributeValue<DateTime>("tss_enddatems");

                FilterExpression filter = new FilterExpression(LogicalOperator.And);
                filter.AddCondition("tss_salestargetnational", ConditionOperator.Equal, context.PrimaryEntityId);
                QueryExpression query = new QueryExpression(_DL_tss_marketsizeresultmapping.EntityName);
                query.Criteria.AddFilter(filter);
                query.ColumnSet = new ColumnSet(true);

                LinkEntity lintToSTPss = new LinkEntity
                {
                    LinkFromEntityName = _DL_tss_marketsizeresultmapping.EntityName,
                    LinkToEntityName = _DL_tss_salestargetpss.EntityName,
                    LinkFromAttributeName = "tss_salestargetpss",
                    LinkToAttributeName = "tss_salestargetpssid",
                    Columns = new ColumnSet(new string[] { "tss_status" }),
                    EntityAlias = "stpss",
                    JoinOperator = JoinOperator.Inner

                };

                query.LinkEntities.Add(lintToSTPss);
                query.LinkEntities[0].LinkCriteria.AddCondition("tss_status", ConditionOperator.Equal, ST_PSS_STATUS_ACTIVE);

                EntityCollection marketsizeresultmappingcollection = _DL_tss_marketsizeresultmapping.Select(organizationService, query);

                var marketsizeresultmapping = (from r in marketsizeresultmappingcollection.Entities.AsEnumerable()
                                               group r by new
                                               {
                                                   groupbysalestargetpss = ((EntityReference)r.Attributes["tss_salestargetpss"]).Id.ToString()
                                               } into g
                                               select new
                                               {
                                                   salestargetpss = g.Key.groupbysalestargetpss
                                               }).ToList();

                int x1 = 0;
                #region Retrieve Market Size Result Mapping
                foreach (var mapsalestarget in marketsizeresultmapping)
                {

                    #region Retrieve MS Market Size  Result
                    //Entity entitySTPSS = _DL_tss_salestargetpss.Select(organizationService, mapsalestarget.GetAttributeValue<EntityReference>("tss_salestargetpss").Id);
                    Entity entitySTPSS = _DL_tss_salestargetpss.Select(organizationService, new Guid( mapsalestarget.salestargetpss));

                    Entity mspss = _DL_tss_marketsizeresultpss.Select(organizationService, entitySTPSS.GetAttributeValue<EntityReference>("tss_marketsizeid").Id);

                    FilterExpression fmasterms = new FilterExpression(LogicalOperator.And);
                    fmasterms.AddCondition("tss_pss", ConditionOperator.Equal, mspss.GetAttributeValue<EntityReference>("tss_pss").Id);
                    fmasterms.AddCondition("tss_customer", ConditionOperator.Equal, mspss.GetAttributeValue<EntityReference>("tss_customer").Id);
                    fmasterms.AddCondition("tss_status", ConditionOperator.Equal, STATUS_COMPLETED_MS);
                    fmasterms.AddCondition("tss_activeperiodstart", ConditionOperator.LessEqual, DateTime.Now);
                    fmasterms.AddCondition("tss_activeperiodsend", ConditionOperator.GreaterEqual, DateTime.Now);

                    QueryExpression qmasterms = new QueryExpression(_DL_tss_mastermarketsize.EntityName);
                    qmasterms.ColumnSet = new ColumnSet(true);
                    qmasterms.Criteria.AddFilter(fmasterms);
                    List<Entity> masterms = _retrievehelper.RetrieveMultiple(organizationService, qmasterms); // _DL_tss_mastermarketsize.Select(organizationService, qmasterms);

                    #region Revise on KA
                    FilterExpression fmsmapping = new FilterExpression(LogicalOperator.And);
                    fmsmapping.AddCondition("tss_marketsizeresultpss", ConditionOperator.Equal, mspss.Id);
                    QueryExpression qmsmapping = new QueryExpression(_DL_tss_marketsizeresultmapping.EntityName);
                    qmsmapping.Criteria.AddFilter(fmsmapping);
                    qmsmapping.ColumnSet = new ColumnSet(true);
                    EntityCollection msmapping = _DL_tss_marketsizeresultmapping.Select(organizationService, qmsmapping);
                    foreach (var mapping in msmapping.Entities)
                    {
                        EntityReference keyAccountRef = mapping.GetAttributeValue<EntityReference>("tss_keyaccount");
                        Entity keyAccount = _DL_tss_keyaccount.Select(organizationService, keyAccountRef.Id);
                        _DL_tss_keyaccount.CheckReviseStatus(keyAccount, setups[0], ref startDtMS, ref endDtMS);
                    }
                    #endregion

                    #region loop Master Market Size
                    int x2 = 0;
                    foreach (var masterMarkeSize in masterms)
                    {
                        tototal = 0m;
                        Entity potentialppToInsert = new Entity(_DL_tss_potentialprospectpart.EntityName);
                        potentialppToInsert["tss_activeperiodstart"] = startDtMS;
                        potentialppToInsert["tss_activeperiodend"] = endDtMS;
                        potentialppToInsert["tss_pss"] = new EntityReference("systemuser", mspss.GetAttributeValue<EntityReference>("tss_pss").Id);
                        potentialppToInsert["tss_customer"] = new EntityReference("account", mspss.GetAttributeValue<EntityReference>("tss_customer").Id);
                        bool skipPotentialProspectPartLine = false;
                        if (masterMarkeSize.Attributes.Contains("tss_customergroup"))
                            potentialppToInsert["tss_customergroup"] = new EntityReference("account", masterMarkeSize.GetAttributeValue<EntityReference>("tss_customergroup").Id);

                        if (masterMarkeSize.Attributes.Contains("tss_groupuiocommodity"))
                        {
                            if (masterMarkeSize.Attributes["tss_groupuiocommodity"] != null)
                                potentialppToInsert["tss_groupuiocommodity"] = new EntityReference("tss_groupuiocommodity", masterMarkeSize.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id);

                            skipPotentialProspectPartLine = true;
                        }

                        if (masterMarkeSize.Attributes.Contains("tss_groupuiocommodityheader"))
                        {
                            if (masterMarkeSize.Attributes["tss_groupuiocommodityheader"] != null)
                                potentialppToInsert["tss_groupuiocommodityheader"] = new EntityReference("tss_groupuiocommodityheader", masterMarkeSize.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id);

                            skipPotentialProspectPartLine = true;
                        }

                        if (masterMarkeSize.Attributes.Contains("tss_serialnumber"))
                        {
                            if (masterMarkeSize.Attributes["tss_serialnumber"] != null)
                            {
                                Guid _serialnumber = masterMarkeSize.GetAttributeValue<EntityReference>("tss_serialnumber").Id;
                                Entity _population = organizationService.Retrieve("new_population", _serialnumber, new ColumnSet(true));

                                potentialppToInsert["tss_serialnumber"] = new EntityReference("new_population", _serialnumber);
                                potentialppToInsert["tss_serialnumberpopulation"] = _population.GetAttributeValue<string>("new_serialnumber");
                            }
                        }

                        potentialppToInsert["tss_marketsizeid"] = new EntityReference("tss_marketsizeresultpss", mspss.Id);
                        potentialppToInsert["tss_msperiodstart"] = msPeriodStart;
                        potentialppToInsert["tss_msperiodend"] = msPeriodEnd;
                        potentialppToInsert["tss_totalamount"] = new Money(tototal);
                        Guid recordHeaderId = organizationService.Create(potentialppToInsert);
                        int unitType = masterMarkeSize.GetAttributeValue<OptionSetValue>("tss_unittype").Value;


                        #region get Master Market Size Line
                        bool isMehtod1 = false,
                            isMehtod2 = false,
                            isMehtod3 = false,
                            isMehtod4 = false,
                            isMehtod5 = false;

                        if (masterMarkeSize.Attributes.Contains("tss_avghmmethod1") && masterMarkeSize.Attributes["tss_avghmmethod1"] != null)
                            isMehtod1 = true;
                        else if (masterMarkeSize.Attributes.Contains("tss_avghmmethod2") && masterMarkeSize.Attributes["tss_avghmmethod2"] != null)
                            isMehtod2 = true;
                        else if (masterMarkeSize.Attributes.Contains("tss_avghmmethod3") && masterMarkeSize.Attributes["tss_avghmmethod3"] != null)
                            isMehtod3 = true;
                        else if (masterMarkeSize.Attributes.Contains("tss_periodpmmethod4") && masterMarkeSize.Attributes["tss_periodpmmethod4"] != null)
                            isMehtod4 = true;
                        else if (masterMarkeSize.Attributes.Contains("tss_periodpmmethod5") && masterMarkeSize.Attributes["tss_periodpmmethod5"] != null)
                            isMehtod5 = true;

                        FilterExpression fmastermsline = new FilterExpression(LogicalOperator.And);
                        QueryExpression qmastermsline = new QueryExpression(_DL_tss_mastermarketsizelines.EntityName);
                        if (isMehtod1 || isMehtod2 || isMehtod3)
                        {
                            fmastermsline.AddCondition("tss_mastermarketsizeref", ConditionOperator.Equal, masterMarkeSize.Id);
                            fmastermsline.AddCondition("tss_duedate", ConditionOperator.NotNull);
                            fmastermsline.AddCondition("tss_pmperiod", ConditionOperator.GreaterThan, 0);

                            qmastermsline.ColumnSet = new ColumnSet(true);
                            qmastermsline.Criteria.AddFilter(fmastermsline);
                        }
                        else if (isMehtod4 || isMehtod5)
                        {
                            fmastermsline.AddCondition("tss_mastermarketsizeref", ConditionOperator.Equal, masterMarkeSize.Id);
                            qmastermsline.ColumnSet = new ColumnSet(true);
                            qmastermsline.Criteria.AddFilter(fmastermsline);
                        }

                        List<Entity> mastermarketsizelines = _retrievehelper.RetrieveMultiple(organizationService, qmastermsline); // _DL_tss_mastermarketsizelines.Select(organizationService, qmastermsline);

                        #endregion

                        #region get Master Market Size Sub Line 
                        int x3 = 0;

                        if (skipPotentialProspectPartLine)
                        {
                            this.BuildPotentialProspectPartSubLine(ref tototal, masterMarkeSize, organizationService, recordHeaderId, new Guid(), skipPotentialProspectPartLine);
                        }
                        else
                        {
                            foreach (Entity mmsline in mastermarketsizelines)
                            {
                                try
                                {
                                    Entity potentialppline = new Entity(_DL_tss_potentialprospectpartlines.EntityName);
                                    potentialppline["tss_pmperiod"] = mmsline.GetAttributeValue<int>("tss_pmperiod").ToString();
                                    //potentialppline["tss_annualaging"] = mmsline.GetAttributeValue<int>("tss_annualaging").ToString();
                                    potentialppline["tss_annualaging"] = mmsline.GetAttributeValue<int>("tss_aging").ToString();

                                    if (DateTime.MinValue != mmsline.GetAttributeValue<DateTime>("tss_duedate"))
                                        potentialppline["tss_estclosedate"] = mmsline.GetAttributeValue<DateTime>("tss_duedate");

                                    if (mmsline.Attributes.Contains("tss_methodcalculationused"))
                                    {
                                        int _methodcalculationused = mmsline.GetAttributeValue<OptionSetValue>("tss_methodcalculationused").Value;

                                        if (_methodcalculationused == 865920000 || _methodcalculationused == 865920001 || _methodcalculationused == 865920002)
                                            potentialppline["tss_marketsizetype"] = new OptionSetValue(865920000);
                                        else
                                            potentialppline["tss_marketsizetype"] = new OptionSetValue(865920001);
                                    }

                                    potentialppline["tss_potentialprospectpart"] = new EntityReference(_DL_tss_potentialprospectpart.EntityName, recordHeaderId);
                                    Guid recordDetailId = organizationService.Create(potentialppline);
                                    this.BuildPotentialProspectPartSubLine(ref tototal, mmsline, organizationService, recordHeaderId, recordDetailId, skipPotentialProspectPartLine);
                                    x3++;
                                }
                                catch (Exception ex)
                                {
                                    Console.Write(x3);
                                    throw;
                                }

                            }
                        }
                        x2++;
                        Entity potentialppToUpdate = _DL_tss_potentialprospectpart.Select(organizationService, recordHeaderId);
                        potentialppToUpdate["tss_totalamount"] = new Money(tototal);
                        organizationService.Update(potentialppToUpdate);
                        #endregion
                    }
                    x1++;
                    #endregion

                    #endregion
                }
                #endregion
            }
        }

        public void BuildPotentialProspectPartSubLine(ref decimal tototal, Entity refEntity, IOrganizationService organizationService, Guid refRecordHeaderId, Guid refRecordLineId, bool skipToPartLines)
        {

            FilterExpression fmmssubline = new FilterExpression(LogicalOperator.And);
            //Method 1,2,3 Filter PM Period>0 and duedate <> null

            if (skipToPartLines)
                fmmssubline.AddCondition("tss_mastermarketsizeid", ConditionOperator.Equal, refEntity.Id);
            else
                fmmssubline.AddCondition("tss_mastermslinesref", ConditionOperator.Equal, refEntity.Id);

            QueryExpression qmmssubline = new QueryExpression(_DL_tss_mastermarketsizesublines.EntityName);
            qmmssubline.ColumnSet = new ColumnSet(true);
            qmmssubline.Criteria.AddFilter(fmmssubline);
            List<Entity> mastermarketsizesublines = _retrievehelper.RetrieveMultiple(organizationService, qmmssubline); // _DL_tss_mastermarketsizesublines.Select(organizationService, qmmssubline);

            #region insert potential prospect part linse
            int x4 = 0;
            foreach (Entity mmssubline in mastermarketsizesublines)
            {
                Entity potentialpplsublineToInsert = new Entity(_DL_tss_potentialprospectpartsublines.EntityName);
                if (mmssubline.Attributes.Contains("tss_partnumber"))
                    potentialpplsublineToInsert["tss_partnumber"] = mmssubline.GetAttributeValue<EntityReference>("tss_partnumber");

                if (skipToPartLines)
                    potentialpplsublineToInsert["tss_potentialprospectpartref"] = new EntityReference(_DL_tss_potentialprospectpart.EntityName, refRecordHeaderId);
                else
                {
                    potentialpplsublineToInsert["tss_potentialprospectpartref"] = new EntityReference(_DL_tss_potentialprospectpart.EntityName, refRecordHeaderId);
                    potentialpplsublineToInsert["tss_potentialprospectpartlinesref"] = new EntityReference(_DL_tss_potentialprospectpartlines.EntityName, refRecordLineId);
                }

                potentialpplsublineToInsert["tss_qty"] = mmssubline.Attributes.Contains("tss_qty") ? mmssubline.GetAttributeValue<int>("tss_qty") : 0;
                potentialpplsublineToInsert["tss_price"] = mmssubline.Attributes.Contains("tss_price") ? mmssubline.GetAttributeValue<Money>("tss_price") : new Money(0);
                potentialpplsublineToInsert["tss_minimumprice"] = mmssubline.Attributes.Contains("tss_minimumprice") ? mmssubline.GetAttributeValue<Money>("tss_minimumprice") : new Money(0);
                potentialpplsublineToInsert["tss_remainingqty"] = mmssubline.Attributes.Contains("tss_qty") ? mmssubline.GetAttributeValue<int>("tss_qty") : 0;
                potentialpplsublineToInsert["tss_remainingqtyactual"] = mmssubline.Attributes.Contains("tss_qty") ? mmssubline.GetAttributeValue<int>("tss_qty") : 0;

                if (mmssubline.Attributes.Contains("tss_price") && mmssubline.Attributes.Contains("tss_qty"))
                    tototal = tototal + (mmssubline.GetAttributeValue<Money>("tss_price").Value * mmssubline.GetAttributeValue<int>("tss_qty"));
                organizationService.Create(potentialpplsublineToInsert);
                x4++;
            }
            #endregion
        }

    }
}