using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrakNusSparepartSystem.DataLayer;
using TrakNusSparepartSystem.Helper;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using TrakNusSparepartSystem.WorkflowActivity.Interface;
using TrakNusSparepartSystem.WorkflowActivity.Business_Layer;

namespace TrakNusSparepartSystem.WorkflowActivity.BusinessLayer
{
    public class BL_tss_marketsizeresultbranch
    {
        #region Constant
        private const int STATUS_OPEN = 865920000;
        private const int WAITINGAPPSM = 865920001;
        private const int STATUS_DRAFT = 865920000;
        private const int WAITINGKORWIL = 865920001;
        private const int STATUS_REMOVED = 865920002;
        private const int STATUS_DISABLED = 865920007;
        private const int REVISION = 865920001;
        private const int STATUS_COMPLETED_MS = 865920000;
        #endregion
        #region Dependencies
        DL_user _DL_user = new DL_user();
        DL_tss_mastermarketsize _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        DL_tss_marketsizeresultpss _DL_tss_marketsizeresultpss = new DL_tss_marketsizeresultpss();
        DL_tss_marketsizeresultbranch _DL_tss_marketsizeresultbranch = new DL_tss_marketsizeresultbranch();
        DL_tss_marketsizeresultnational _DL_tss_marketsizeresultnational = new DL_tss_marketsizeresultnational();
        DL_tss_marketsizeresultmapping _DL_tss_marketsizeresultmapping = new DL_tss_marketsizeresultmapping();
        DL_tss_matrixapprovalmarketsize _DL_tss_matrixapprovalmarketsize = new DL_tss_matrixapprovalmarketsize();
        DL_tss_sparepartsetup _DL_tss_sparepartsetup = new DL_tss_sparepartsetup();
        DL_tss_keyaccount _DL_tss_keyaccount = new DL_tss_keyaccount();
        BL_KeyAccount _BL_KeyAccount = new BL_KeyAccount();
        BL_tss_mastermarketsize _BL_tss_mastermarketsize = new BL_tss_mastermarketsize();
        EntityCollection pssToRevise = new EntityCollection();
        RetrieveHelper _retrievehelper = new RetrieveHelper();
        #endregion


        public Guid GenerateMarketSizeResultBranch(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {
            int totaluio = 0;
            int totalnonuio = 0;
            int totalgroupuiocommodity = 0;
            int totalalluio = 0;

            decimal totalmscommodity = 0m;
            decimal totalmsstandardpart = 0m;
            decimal totalamountms = 0m;

            DateTime startPeriodMS = new DateTime(1900, 1, 1);
            DateTime endPeriodMS = new DateTime(1900, 1, 1);
            DateTime startDtMS = new DateTime(1900, 1, 1);
            DateTime endDtMS = new DateTime(1900, 1, 1);

            Entity msPss = _DL_tss_marketsizeresultpss.Select(organizationService, context.PrimaryEntityId);
            if (msPss.Contains("tss_branch"))
            {
                FilterExpression fMSBr = new FilterExpression(LogicalOperator.And);
                //2018.09.17 - start = lessequal & end = greaterequal
                //fMSBr.AddCondition("tss_activeperiodsend", ConditionOperator.LessEqual, DateTime.Now);
                //fMSBr.AddCondition("tss_activeperiodstart", ConditionOperator.GreaterEqual, DateTime.Now);
                fMSBr.AddCondition("tss_activeperiodsend", ConditionOperator.GreaterEqual, DateTime.Now);
                fMSBr.AddCondition("tss_activeperiodstart", ConditionOperator.LessEqual, DateTime.Now);
                fMSBr.AddCondition("tss_branch", ConditionOperator.Equal, msPss.GetAttributeValue<EntityReference>("tss_branch").Id);
                fMSBr.AddCondition("tss_status", ConditionOperator.Equal, WAITINGKORWIL);

                QueryExpression qMSBr = new QueryExpression(_DL_tss_marketsizeresultbranch.EntityName);
                qMSBr.Criteria.AddFilter(fMSBr);
                qMSBr.ColumnSet = new ColumnSet(true);
                EntityCollection msBr = _DL_tss_marketsizeresultbranch.Select(organizationService, qMSBr);

                if (msBr.Entities.Count > 0)
                {
                    Entity updateBr = msBr.Entities[0];
                    totaluio = updateBr.GetAttributeValue<int>("tss_totaluio") + msPss.GetAttributeValue<int>("tss_totaluio");
                    totalnonuio = updateBr.GetAttributeValue<int>("tss_totalnonuio") + msPss.GetAttributeValue<int>("tss_totalnonuio");
                    totalgroupuiocommodity = updateBr.GetAttributeValue<int>("tss_totalgroupuiocommodity") + msPss.GetAttributeValue<int>("tss_totalgroupuiocommodity");
                    totalalluio = totaluio + totalnonuio + totalgroupuiocommodity;
                    totalmscommodity = updateBr.GetAttributeValue<Money>("tss_totalmscommodity").Value + msPss.GetAttributeValue<Money>("tss_totalmscommodity").Value;
                    totalmsstandardpart = updateBr.GetAttributeValue<Money>("tss_totalmsstandardpart").Value + msPss.GetAttributeValue<Money>("tss_totalmsstandardpart").Value;
                    totalamountms = updateBr.GetAttributeValue<Money>("tss_totalamountms").Value + msPss.GetAttributeValue<Money>("tss_totalamountms").Value;

                    _DL_tss_marketsizeresultbranch = new DL_tss_marketsizeresultbranch();
                    _DL_tss_marketsizeresultbranch.tss_totaluio = totaluio;
                    _DL_tss_marketsizeresultbranch.tss_totalnonuio = totalnonuio;
                    _DL_tss_marketsizeresultbranch.tss_totalgroupuiocommodity = totalgroupuiocommodity;
                    _DL_tss_marketsizeresultbranch.tss_totalalluio = totalalluio;
                    _DL_tss_marketsizeresultbranch.tss_totalmscommodity = totalmscommodity;
                    _DL_tss_marketsizeresultbranch.tss_totalmsstandardpart = totalmsstandardpart;
                    _DL_tss_marketsizeresultbranch.tss_totalamountms = totalamountms;
                    _DL_tss_marketsizeresultbranch.Update(organizationService, updateBr.Id);
                    return updateBr.Id;
                }
                else
                {
                    totaluio = msPss.GetAttributeValue<int>("tss_totaluio");
                    totalnonuio = msPss.GetAttributeValue<int>("tss_totalnonuio");
                    totalgroupuiocommodity = msPss.GetAttributeValue<int>("tss_totalgroupuiocommodity");
                    totalalluio = totaluio + totalnonuio + totalgroupuiocommodity;
                    totalmscommodity = msPss.GetAttributeValue<Money>("tss_totalmscommodity").Value;
                    totalmsstandardpart = msPss.GetAttributeValue<Money>("tss_totalmsstandardpart").Value;
                    totalamountms = msPss.GetAttributeValue<Money>("tss_totalamountms") == null ? 0m : msPss.GetAttributeValue<Money>("tss_totalamountms").Value;
                    startPeriodMS = msPss.GetAttributeValue<DateTime>("tss_msperiodstart") == DateTime.MinValue ? new DateTime(1900, 1, 1) : msPss.GetAttributeValue<DateTime>("tss_msperiodstart");
                    endPeriodMS = msPss.GetAttributeValue<DateTime>("tss_msperiodend") == DateTime.MinValue ? new DateTime(1900, 1, 1) : msPss.GetAttributeValue<DateTime>("tss_msperiodend");
                    startDtMS = msPss.GetAttributeValue<DateTime>("tss_activeperiodstart") == DateTime.MinValue ? new DateTime(1900, 1, 1) : msPss.GetAttributeValue<DateTime>("tss_activeperiodstart");
                    endDtMS = msPss.GetAttributeValue<DateTime>("tss_activeperiodsend") == DateTime.MinValue ? new DateTime(1900, 1, 1) : msPss.GetAttributeValue<DateTime>("tss_activeperiodsend");

                    _DL_tss_marketsizeresultbranch = new DL_tss_marketsizeresultbranch();
                    _DL_tss_marketsizeresultbranch.tss_branch = msPss.GetAttributeValue<EntityReference>("tss_branch").Id;
                    _DL_tss_marketsizeresultbranch.tss_totaluio = totaluio;
                    _DL_tss_marketsizeresultbranch.tss_totalnonuio = totalnonuio;
                    _DL_tss_marketsizeresultbranch.tss_totalgroupuiocommodity = totalgroupuiocommodity;
                    _DL_tss_marketsizeresultbranch.tss_totalalluio = totalalluio;
                    _DL_tss_marketsizeresultbranch.tss_totalmscommodity = totalmscommodity;
                    _DL_tss_marketsizeresultbranch.tss_totalmsstandardpart = totalmsstandardpart;
                    _DL_tss_marketsizeresultbranch.tss_totalamountms = totalamountms;
                    _DL_tss_marketsizeresultbranch.tss_msperiodstart = startPeriodMS;
                    _DL_tss_marketsizeresultbranch.tss_msperiodend = endPeriodMS;
                    _DL_tss_marketsizeresultbranch.tss_activeperiodstart = startDtMS;
                    _DL_tss_marketsizeresultbranch.tss_activeperiodsend = endDtMS;
                    _DL_tss_marketsizeresultbranch.tss_status = WAITINGKORWIL;
                    _DL_tss_marketsizeresultbranch.tss_statusreason = STATUS_OPEN;
                    return _DL_tss_marketsizeresultbranch.Insert(organizationService);
                }
            }
            return Guid.Empty;
        }

        public void ApproveMarketSizeResultBranch_OnClick(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {

            DateTime startPeriodMS = new DateTime();
            DateTime endPeriodMS = new DateTime();
            DateTime startDtMS = DateTime.MinValue;
            DateTime endDtMS = DateTime.MinValue;
            Entity EN_tss_marketsizeresultnational;
            Guid recordID;

            #region Retrieve data market size mapping
            FilterExpression fMapp = new FilterExpression(LogicalOperator.And);
            fMapp.AddCondition("tss_marketsizeresultbranch", ConditionOperator.Equal, context.PrimaryEntityId);

            QueryExpression qMapp = new QueryExpression("tss_marketsizeresultmapping");
            qMapp.Criteria.AddFilter(fMapp);
            qMapp.ColumnSet = new ColumnSet(true);
            EntityCollection ENC_tss_marketsizeresultmapping = _DL_tss_marketsizeresultmapping.Select(organizationService, qMapp);
            #endregion

            QueryExpression qUser = new QueryExpression(_DL_user.EntityName);
            qUser.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, context.UserId);
            qUser.ColumnSet.AddColumn("businessunitid");
            Entity user = _DL_user.Select(organizationService, qUser).Entities[0];

            FilterExpression fMs = new FilterExpression(LogicalOperator.And);
            fMs.AddCondition("tss_unittype", ConditionOperator.NotNull);
            fMs.AddCondition("tss_pss", ConditionOperator.Equal, context.UserId);
            fMs.AddCondition("tss_status", ConditionOperator.Equal, STATUS_COMPLETED_MS);

            QueryExpression qMS = new QueryExpression(_DL_tss_mastermarketsize.EntityName);
            qMS.Criteria.AddFilter(fMs);
            qMS.ColumnSet = new ColumnSet(true);
            List<Entity> ms = _retrievehelper.RetrieveMultiple(organizationService, qMS); // _DL_tss_mastermarketsize.Select(organizationService, qMS);
            Guid[] custs = ms.GroupBy(x => x.GetAttributeValue<EntityReference>("tss_customer").Id).Select(x => x.First().GetAttributeValue<EntityReference>("tss_customer").Id).ToArray();

            Entity EN_tss_marketsizeresultbranch = _DL_tss_marketsizeresultbranch.Select(organizationService, context.PrimaryEntityId);

            FilterExpression fMSN = new FilterExpression(LogicalOperator.And);
            //2018.09.17 - start = lessequal & end = greaterequal
            //fMSN.AddCondition("tss_activeperiodsend", ConditionOperator.LessEqual, DateTime.Now);
            //fMSN.AddCondition("tss_activeperiodstart", ConditionOperator.GreaterEqual, DateTime.Now);
            fMSN.AddCondition("tss_activeperiodsend", ConditionOperator.GreaterEqual, DateTime.Now);
            fMSN.AddCondition("tss_activeperiodstart", ConditionOperator.LessEqual, DateTime.Now);
            fMSN.AddCondition("tss_status", ConditionOperator.Equal, WAITINGAPPSM);

            QueryExpression qMSN = new QueryExpression("tss_marketsizeresultnational");
            qMSN.Criteria.AddFilter(fMSN);
            qMSN.ColumnSet = new ColumnSet(true);
            EntityCollection ENC_tss_marketsizeresultnational = _DL_tss_marketsizeresultnational.Select(organizationService, qMSN);

            #region Generate record on Market Size Result National based
            if (ENC_tss_marketsizeresultnational.Entities.Count > 0)
            {
                // Update existing market size result national
                ENC_tss_marketsizeresultnational.Entities[0]["tss_name"] = "National";
                ENC_tss_marketsizeresultnational.Entities[0]["tss_totaluio"] = ENC_tss_marketsizeresultnational.Entities[0].GetAttributeValue<int>("tss_totaluio") + EN_tss_marketsizeresultbranch.GetAttributeValue<int>("tss_totaluio");
                ENC_tss_marketsizeresultnational.Entities[0]["tss_totalnonuio"] = ENC_tss_marketsizeresultnational.Entities[0].GetAttributeValue<int>("tss_totalnonuio") + EN_tss_marketsizeresultbranch.GetAttributeValue<int>("tss_totalnonuio");
                ENC_tss_marketsizeresultnational.Entities[0]["tss_totalgroupuiocommodity"] = ENC_tss_marketsizeresultnational.Entities[0].GetAttributeValue<int>("tss_totalgroupuiocommodity") + EN_tss_marketsizeresultbranch.GetAttributeValue<int>("tss_totalgroupuiocommodity");
                ENC_tss_marketsizeresultnational.Entities[0]["tss_totalalluio"] =
                                                                                ENC_tss_marketsizeresultnational.Entities[0].GetAttributeValue<int>("tss_totalalluio") +
                                                                                EN_tss_marketsizeresultbranch.GetAttributeValue<int>("tss_totaluio") +
                                                                                EN_tss_marketsizeresultbranch.GetAttributeValue<int>("tss_totalnonuio") +
                                                                                EN_tss_marketsizeresultbranch.GetAttributeValue<int>("tss_totalgroupuiocommodity");

                decimal totalmscommodity = ENC_tss_marketsizeresultnational.Entities[0].GetAttributeValue<Money>("tss_totalmscommodity").Value + EN_tss_marketsizeresultbranch.GetAttributeValue<Money>("tss_totalmscommodity").Value;
                decimal totalmsstandardpart = ENC_tss_marketsizeresultnational.Entities[0].GetAttributeValue<Money>("tss_totalmsstandardpart").Value + EN_tss_marketsizeresultbranch.GetAttributeValue<Money>("tss_totalmsstandardpart").Value;

                ENC_tss_marketsizeresultnational.Entities[0]["tss_totalmscommodity"] = new Money(ENC_tss_marketsizeresultnational.Entities[0].GetAttributeValue<Money>("tss_totalmscommodity").Value + EN_tss_marketsizeresultbranch.GetAttributeValue<Money>("tss_totalmscommodity").Value);
                ENC_tss_marketsizeresultnational.Entities[0]["tss_totalmsstandardpart"] = new Money(ENC_tss_marketsizeresultnational.Entities[0].GetAttributeValue<Money>("tss_totalmsstandardpart").Value + EN_tss_marketsizeresultbranch.GetAttributeValue<Money>("tss_totalmsstandardpart").Value);
                ENC_tss_marketsizeresultnational.Entities[0]["tss_totalamountms"] = new Money(totalmscommodity + totalmsstandardpart);
                ENC_tss_marketsizeresultnational.Entities[0]["tss_status"] = new OptionSetValue(WAITINGAPPSM);
                ENC_tss_marketsizeresultnational.Entities[0]["tss_statusreason"] = new OptionSetValue(STATUS_OPEN);
                EN_tss_marketsizeresultnational = ENC_tss_marketsizeresultnational.Entities[0];
                recordID = EN_tss_marketsizeresultnational.GetAttributeValue<Guid>("tss_marketsizeresultnationalid");
                organizationService.Update(ENC_tss_marketsizeresultnational.Entities[0]);
            }
            else
            {
                // Insert market size result national for current period

                EN_tss_marketsizeresultnational = new Entity("tss_marketsizeresultnational");
                EN_tss_marketsizeresultnational["tss_name"] = "National";
                EN_tss_marketsizeresultnational["tss_totaluio"] = EN_tss_marketsizeresultbranch.GetAttributeValue<int>("tss_totaluio");
                EN_tss_marketsizeresultnational["tss_totalnonuio"] = EN_tss_marketsizeresultbranch.GetAttributeValue<int>("tss_totalnonuio");
                EN_tss_marketsizeresultnational["tss_totalgroupuiocommodity"] = EN_tss_marketsizeresultbranch.GetAttributeValue<int>("tss_totalgroupuiocommodity");
                EN_tss_marketsizeresultnational["tss_totalalluio"] = EN_tss_marketsizeresultbranch.GetAttributeValue<int>("tss_totalnonuio") + EN_tss_marketsizeresultbranch.GetAttributeValue<int>("tss_totalgroupuiocommodity") + EN_tss_marketsizeresultbranch.GetAttributeValue<int>("tss_totaluio");

                EN_tss_marketsizeresultnational["tss_totalmscommodity"] = EN_tss_marketsizeresultbranch.GetAttributeValue<Money>("tss_totalmscommodity");
                EN_tss_marketsizeresultnational["tss_totalmsstandardpart"] = EN_tss_marketsizeresultbranch.GetAttributeValue<Money>("tss_totalmsstandardpart");
                EN_tss_marketsizeresultnational["tss_totalamountms"] = new Money(EN_tss_marketsizeresultbranch.GetAttributeValue<Money>("tss_totalmscommodity").Value + EN_tss_marketsizeresultbranch.GetAttributeValue<Money>("tss_totalmsstandardpart").Value);

                //Get SparePart Config 

                //FilterExpression fSetup = new FilterExpression(LogicalOperator.And);
                //fSetup.AddCondition("tss_name", ConditionOperator.Equal, "TSS");

                //QueryExpression qSetup = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
                //qSetup.ColumnSet = new ColumnSet(true);
                //qSetup.Criteria.AddFilter(fSetup);
                //EntityCollection setups = _DL_tss_sparepartsetup.Select(organizationService, qSetup);

                //if (setups.Entities.Count > 0)
                //{
                //    //TBA
                //    startPeriodMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems");
                //    endPeriodMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_enddatems");
                //}

                GetActivePeriodDate _getactiveperioddate = new GetActivePeriodDate(organizationService);
                _getactiveperioddate.Process();

                startPeriodMS = (DateTime)_getactiveperioddate.StartDateMarketSize;
                endPeriodMS = (DateTime)_getactiveperioddate.EndDateMarketSize;
                int _evaluationmarketsize = (int)_getactiveperioddate.EvaluationMarketSize;

                #region Revise on KA
                foreach (var mapping in ENC_tss_marketsizeresultmapping.Entities)
                {
                    EntityReference keyAccountRef = mapping.GetAttributeValue<EntityReference>("tss_keyaccount");
                    Entity keyAccount = _DL_tss_keyaccount.Select(organizationService, keyAccountRef.Id);
                    _DL_tss_keyaccount.StatusRevise(keyAccount, ref startPeriodMS, ref endPeriodMS, _evaluationmarketsize);
                }

                startDtMS = startPeriodMS;
                endDtMS = endPeriodMS;
                #endregion

                EN_tss_marketsizeresultnational["tss_msperiodstart"] = startPeriodMS;
                EN_tss_marketsizeresultnational["tss_msperiodend"] = endPeriodMS;

                if (startDtMS != DateTime.MinValue)
                    EN_tss_marketsizeresultnational["tss_activeperiodstart"] = startDtMS;
                if (endDtMS != DateTime.MinValue)
                    EN_tss_marketsizeresultnational["tss_activeperiodsend"] = endDtMS;
                EN_tss_marketsizeresultnational["tss_statusreason"] = new OptionSetValue(STATUS_OPEN);
                EN_tss_marketsizeresultnational["tss_status"] = new OptionSetValue(WAITINGAPPSM);

                recordID = organizationService.Create(EN_tss_marketsizeresultnational);
                EN_tss_marketsizeresultnational.Id = recordID;
            }

            #endregion

            #region Update status Market Size Result PSS to "Waiting Approval SM"

            foreach (Entity en_mapping in ENC_tss_marketsizeresultmapping.Entities)
            {
                Entity EN_tss_marketsizeresultpss = _DL_tss_marketsizeresultpss.Select(organizationService, en_mapping.GetAttributeValue<EntityReference>("tss_marketsizeresultpss").Id);
                EN_tss_marketsizeresultpss["tss_status"] = new OptionSetValue(865920003);
                organizationService.Update(EN_tss_marketsizeresultpss);
            }

            //Update ID Market size result national 
            foreach (Entity en in ENC_tss_marketsizeresultmapping.Entities)
            {
                en["tss_marketsizeresultnational"] = new EntityReference("tss_marketsizeresultnational", (recordID));
                organizationService.Update(en);
            }


            #endregion

            #region insert approver , notif approver and share records

            FilterExpression fbranch = new FilterExpression(LogicalOperator.And);
            fbranch.AddCondition("tss_activeperiodsend", ConditionOperator.Equal, EN_tss_marketsizeresultbranch.GetAttributeValue<DateTime>("tss_activeperiodsend"));
            fbranch.AddCondition("tss_activeperiodstart", ConditionOperator.Equal, EN_tss_marketsizeresultbranch.GetAttributeValue<DateTime>("tss_activeperiodstart"));
            fbranch.AddCondition("tss_marketsizeresultbranchid", ConditionOperator.Equal, context.PrimaryEntityId);
            fbranch.AddCondition("tss_status", ConditionOperator.In, 865920000, 865920001);

            QueryExpression qbranch = new QueryExpression(_DL_tss_marketsizeresultbranch.EntityName);
            qbranch.Criteria.AddFilter(fbranch);
            qbranch.ColumnSet = new ColumnSet(true);
            EntityCollection ENC_tss_marketsizeresultbranch = _DL_tss_marketsizeresultbranch.Select(organizationService, qbranch);

            if (ENC_tss_marketsizeresultbranch.Entities.Count > 0)
            {
                ENC_tss_marketsizeresultbranch[0]["tss_approvedate"] = DateTime.Now.ToLocalTime();
                ENC_tss_marketsizeresultbranch[0]["tss_status"] = new OptionSetValue(865920002);/*Waiting Approval SM*/
                ENC_tss_marketsizeresultbranch[0]["tss_statusreason"] = new OptionSetValue(865920001);/*Waiting Approval*/
                organizationService.Update(ENC_tss_marketsizeresultbranch[0]);

                FilterExpression fappMat = new FilterExpression(LogicalOperator.And);
                //20180918 | branch tidak dipake
                //fappMat.AddCondition("tss_branch", ConditionOperator.Equal, EN_tss_marketsizeresultbranch.GetAttributeValue<EntityReference>("tss_branch").Id);
                fappMat.AddCondition("tss_approvaltype", ConditionOperator.Equal, 865920002);
                fappMat.AddCondition("tss_approvalfor", ConditionOperator.Equal, 865920000);

                QueryExpression qappMat = new QueryExpression("tss_matrixapprovalmarketsize");
                qappMat.Criteria.AddFilter(fappMat);
                qappMat.Orders.Add(new Microsoft.Xrm.Sdk.Query.OrderExpression("tss_priorityno", Microsoft.Xrm.Sdk.Query.OrderType.Ascending));
                qappMat.ColumnSet = new ColumnSet(true);

                EntityCollection en_approvalMatrixMarketSize = _DL_tss_matrixapprovalmarketsize.Select(organizationService, qappMat);

                if (en_approvalMatrixMarketSize.Entities.Count > 0)
                {
                    foreach (Entity appMatrix in en_approvalMatrixMarketSize.Entities)
                    {

                        #region insert to list Approval Matrix
                        Entity ENT_approvalListMarketSize = new Entity("tss_approvallistmarketsize");
                        ENT_approvalListMarketSize["tss_approver"] = new EntityReference("systemuser", appMatrix.GetAttributeValue<EntityReference>("tss_approver").Id);
                        ENT_approvalListMarketSize["tss_marketsizeresultnational"] = new EntityReference("tss_marketsizeresultnational", recordID);
                        ENT_approvalListMarketSize["tss_type"] = new OptionSetValue(865920002);
                        organizationService.Create(ENT_approvalListMarketSize);
                        #endregion
                        #region share records
                        ShareRecords objShare = new ShareRecords();
                        Entity en_targetUser = new Entity("systemuser", appMatrix.GetAttributeValue<EntityReference>("tss_approver").Id);

                        objShare.ShareRecord(organizationService, EN_tss_marketsizeresultnational, en_targetUser);
                        #endregion
                    }

                    TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent objEmail = new TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent();
                    string emailSubject = EN_tss_marketsizeresultnational.GetAttributeValue<string>("tss_name") + " is waiting for approval";
                    string emailContent = "Please review and approve.";
                    objEmail.SendEmailNotif(context.UserId, en_approvalMatrixMarketSize.Entities[0].GetAttributeValue<EntityReference>("tss_approver").Id, Guid.Empty, organizationService, emailSubject, emailContent);

                }
                else
                {
                    throw new InvalidWorkflowException("No Approval Matrix found. Please contact your IT administrator.");
                }
            }

            #endregion
        }

        public void ReviseMarketSizeResultBranch(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {
            #region 2018.09.19 | Get User
            //QueryExpression qUser = new QueryExpression(_DL_user.EntityName);
            //qUser.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, context.UserId);
            //qUser.ColumnSet.AddColumn("title");

            //Entity EN_User = _DL_user.Select(organizationService, qUser).Entities[0];

            //var oTitle = EN_User.GetAttributeValue<EntityReference>("title").Name;
            #endregion

            #region 2018.09.19 | Process Revise
            //if (oTitle == "Korwil" || oTitle =="SM")
            //{
            Entity EN_marketsizeresultbranch = _DL_tss_marketsizeresultbranch.Select(organizationService, context.PrimaryEntityId);
            EN_marketsizeresultbranch["tss_status"] = new OptionSetValue(865920006); //DISABLED
            EN_marketsizeresultbranch["tss_statusreason"] = new OptionSetValue(865920004); //REMOVED

            QueryExpression qMarketSizeResultMapping = new QueryExpression(_DL_tss_marketsizeresultmapping.EntityName);
            qMarketSizeResultMapping.Criteria.AddCondition("tss_marketsizeresultbranch", ConditionOperator.Equal, context.PrimaryEntityId);
            qMarketSizeResultMapping.ColumnSet.AddColumn("tss_marketsizeresultpss");
            qMarketSizeResultMapping.ColumnSet = new ColumnSet(true);

            EntityCollection EN_marketsizeresultmapping = _DL_tss_marketsizeresultmapping.Select(organizationService, qMarketSizeResultMapping);


            if (EN_marketsizeresultmapping.Entities.Count > 0)
            {
                foreach (Entity en_mapping in EN_marketsizeresultmapping.Entities)
                {
                    Entity EN_tss_marketsizeresultpss = _DL_tss_marketsizeresultpss.Select(organizationService, en_mapping.GetAttributeValue<EntityReference>("tss_marketsizeresultpss").Id);
                    pssToRevise.Entities.Add(EN_tss_marketsizeresultpss);

                    EN_tss_marketsizeresultpss["tss_status"] = new OptionSetValue(STATUS_DISABLED);
                    EN_tss_marketsizeresultpss["tss_statusreason"] = new OptionSetValue(STATUS_REMOVED);

                    organizationService.Update(EN_tss_marketsizeresultpss);

                    //Get User
                    QueryExpression qUser = new QueryExpression(_DL_user.EntityName);
                    qUser.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, EN_tss_marketsizeresultpss.GetAttributeValue<EntityReference>("tss_pss").Id);
                    qUser.ColumnSet.AddColumn("title");

                    Entity EN_User = _DL_user.Select(organizationService, qUser).Entities[0];

                    EN_User["tss_marketsizeconfirmed"] = false;
                    organizationService.Update(EN_User);
                }


                //FilterExpression f = new FilterExpression(LogicalOperator.And);
                //QueryExpression q = new QueryExpression(_DL_tss_marketsizeresultmapping.EntityName);
                //Guid[] keyAccountId = EN_marketsizeresultmapping.Entities.Select(x => x.Id).Distinct().ToArray();
                //object[] keyAccountId = EN_marketsizeresultmapping.Entities.Select(x => (object)x.GetAttributeValue<EntityReference>("tss_keyaccount").Id).ToArray();

                //f = new FilterExpression(LogicalOperator.And);
                //f.AddCondition("tss_keyaccountid", ConditionOperator.In, keyAccountId);

                //q = new QueryExpression(_DL_tss_keyaccount.EntityName);
                //q.ColumnSet = new ColumnSet(true);
                //q.Criteria.AddFilter(f);

                object[] keyAccountId = EN_marketsizeresultmapping.Entities.Select(x => (object)x.GetAttributeValue<EntityReference>("tss_keyaccount").Id).ToArray();

                FilterExpression f = new FilterExpression(LogicalOperator.And);
                f.AddCondition("tss_keyaccountid", ConditionOperator.In, keyAccountId);

                QueryExpression q = new QueryExpression(_DL_tss_keyaccount.EntityName);
                q.ColumnSet = new ColumnSet(true);
                q.Criteria.AddFilter(f);

                EntityCollection entityToRevise = _DL_tss_keyaccount.Select(organizationService, q);

                _BL_tss_mastermarketsize.Revise(organizationService, pssToRevise);
                _BL_KeyAccount.ReviseKeyAccount(organizationService, tracingService, context, entityToRevise);
            }

            //EN_User["tss_marketsizeconfirmed"] = false;
            //organizationService.Update(EN_User);
            organizationService.Update(EN_marketsizeresultbranch);




            #region 2018.09.17 | Generate Email to PSS and all previous approver base on Market Size Matrix Approver to inform them that the market size has been revised.
            QueryExpression qappMat = new QueryExpression(_DL_tss_matrixapprovalmarketsize.EntityName);
            qappMat.Orders.Add(new Microsoft.Xrm.Sdk.Query.OrderExpression("tss_priorityno", Microsoft.Xrm.Sdk.Query.OrderType.Ascending));
            qappMat.ColumnSet = new ColumnSet(true);

            EntityCollection entityApprovalMatrixMarketSize = _DL_tss_matrixapprovalmarketsize.Select(organizationService, qappMat);

            if (entityApprovalMatrixMarketSize.Entities.Count > 0)
            {
                foreach (var entity in entityApprovalMatrixMarketSize.Entities)
                {
                    TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent objEmail = new TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent();
                    string emailSubject = EN_marketsizeresultbranch.GetAttributeValue<string>("tss_name") + " has been revised";
                    string emailContent = "Market Size Result Branch has been revised";
                    objEmail.SendEmailNotif(context.UserId, entity.GetAttributeValue<EntityReference>("tss_approver").Id, Guid.Empty, organizationService, emailSubject, emailContent);
                }
            }
            else
            {
                throw new InvalidWorkflowException("Approval Matrix not found. Please contact your IT administrator.");
            }
            #endregion
            //}
            #endregion
        }

    }
}
