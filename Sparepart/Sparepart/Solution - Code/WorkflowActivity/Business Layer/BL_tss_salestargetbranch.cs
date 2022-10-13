using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrakNusSparepartSystem.DataLayer;
using TrakNusSparepartSystem.Helper;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;

namespace TrakNusSparepartSystem.WorkflowActivity.Business_Layer
{
    public class BL_tss_salestargetbranch
    {
        #region Constant
        private const int UNITTYPE_UIO = 865920000;
        private const int UNITTYPE_NONUIO = 865920001;
        private const int UNITTYPE_COMMODITY = 865920002;
        private const int STATUS_OPEN = 865920000;
        private const int STATUS_DRAFT = 865920000;
        private const int STATUS_COMPLETED_MS = 865920000;
        private const int REVISION = 865920001;
        private const int PSS_STATUS_ACTIVE = 865920005;
        private const int PSS_STATUS_REASON_ACTIVE = 865920001;
        private const int BRANCH_STATUS_ACTIVE = 865920004;
        private const int BRANCH_STATUS_REASON_COMPLETED = 865920003;
        private const int NATIONAL_STATUS_ACTIVE = 865920002;
        private const int NATIONAL_STATUS_REASON_COMPLETED = 865920003;
        private const int KEY_ACCOUNT_STATUS_ACTIVE = 865920003;
        private const int KEY_ACCOUNT_STATUS_REASON_ACTIVE = 865920001;
        private const int KEY_ACCOUNT_STATUS_REVISE = 865920004;
        private const int KEY_ACCOUNT_VERSION_REVISE = 865920001;
        private const int TARGET_SALES_NAT_STATUS_DRAFT = 865920000;
        private const int TARGET_SALES_NAT_STATUS_REASON_OPEN = 865920000;
        private const int TARGET_SALES_PSS_STATUS_DRAFT = 865920000;
        private const int TARGET_SALES_PSS_STATUS_REASON_OPEN = 865920000;

        private const int TARGET_SALES_BRANCH_STATUS_DRAFT = 865920000;
        private const int TARGET_SALES_BRANCH_STATUS_REASON_OPEN = 865920000;
        #endregion

        #region Dependencies
        DL_user _DL_user = new DL_user();
        DL_account _DL_account = new DL_account();
        DL_tss_marketsizeresultpss _DL_tss_marketsizeresultpss = new DL_tss_marketsizeresultpss();
        DL_tss_salestargetbranch _DL_tss_salestargetbranch = new DL_tss_salestargetbranch();
        DL_tss_salestargetnational _DL_tss_salestargetnational = new DL_tss_salestargetnational();
        DL_tss_keyaccount _DL_tss_keyaccount = new DL_tss_keyaccount();
        DL_tss_mastermarketsize _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        DL_tss_mastermarketsizelines _DL_tss_mastermarketsizelines = new DL_tss_mastermarketsizelines();
        DL_tss_mastermarketsizesublines _DL_tss_mastermarketsizesublines = new DL_tss_mastermarketsizesublines();
        DL_tss_sparepartsetup _DL_tss_sparepartsetup = new DL_tss_sparepartsetup();
        DL_tss_matrixapprovalmarketsize _DL_tss_matrixapprovalmarketsize = new DL_tss_matrixapprovalmarketsize();
        DL_tss_marketsizeresultmapping _DL_tss_marketsizeresultmapping = new DL_tss_marketsizeresultmapping();
        DL_tss_salestargetbranch _DL_tss_marketsizeresultbranch = new DL_tss_salestargetbranch();
        BL_tss_salestargetnation _BL_tss_salestargetnation = new BL_tss_salestargetnation();
        DL_tss_salestargetpss _DL_tss_salestargetpss = new DL_tss_salestargetpss();
        RetrieveHelper _retrievehelper = new RetrieveHelper();
        #endregion

        public Guid GenerateSalesTargetBranch(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {

            DateTime startDtMS = DateTime.MinValue;
            DateTime endDtMS = DateTime.MinValue;
            decimal totalJanuary = 0m;
            decimal totalFebruary = 0m;
            decimal totalMarch = 0m;
            decimal totalApril = 0m;
            decimal totalMay = 0m;
            decimal totalJune = 0m;
            decimal totalJuly = 0m;
            decimal totalAugust = 0m;
            decimal totalSept = 0m;
            decimal totalOct = 0m;
            decimal totalNov = 0m;
            decimal totalDec = 0m;
            decimal totalYearly = 0m;


            Entity stPss = _DL_tss_salestargetpss.Select(organizationService, context.PrimaryEntityId);
            Guid salesTargetBrachID = Guid.Empty;
            if (stPss.Contains("tss_branch"))
            {

                #region get Part Setup
                //Get SparePart Config 
                FilterExpression fSetup = new FilterExpression(LogicalOperator.And);
                fSetup.AddCondition("tss_name", ConditionOperator.Equal, "TSS");

                QueryExpression qSetup = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
                qSetup.ColumnSet = new ColumnSet(true);
                qSetup.Criteria.AddFilter(fSetup);
                EntityCollection setups = _DL_tss_sparepartsetup.Select(organizationService, qSetup);

                FilterExpression fmatrixmsperiod = new FilterExpression(LogicalOperator.And);
                fmatrixmsperiod.AddCondition("tss_isactive", ConditionOperator.Equal, true);

                QueryExpression qmatrixmsperiod = new QueryExpression("tss_matrixmarketsizeperiod");
                qmatrixmsperiod.ColumnSet = new ColumnSet(true);
                qmatrixmsperiod.Criteria.AddFilter(fmatrixmsperiod);
                EntityCollection matrixmsperiod = organizationService.RetrieveMultiple(qmatrixmsperiod);
                #endregion



                #region Retrieve MS PSS
                FilterExpression fmspss = new FilterExpression(LogicalOperator.And);
                fmspss.AddCondition("tss_marketsizeresultpssid", ConditionOperator.Equal, stPss.GetAttributeValue<EntityReference>("tss_marketsizeid").Id);
                fmspss.AddCondition("tss_branch", ConditionOperator.Equal, stPss.GetAttributeValue<EntityReference>("tss_branch").Id);
                fmspss.AddCondition("tss_pss", ConditionOperator.Equal, stPss.GetAttributeValue<EntityReference>("tss_pss").Id);
                
                QueryExpression qmspss = new QueryExpression(_DL_tss_marketsizeresultpss.EntityName);
                qmspss.ColumnSet = new ColumnSet(true);
                qmspss.Criteria.AddFilter(fmspss);
                EntityCollection mspss = _DL_tss_marketsizeresultpss.Select(organizationService, qmspss);

                if (mspss.Entities.Count > 0)
                {
                    #region Check Revise on KA
                    FilterExpression fmapp = new FilterExpression(LogicalOperator.And);
                    fmapp.AddCondition("tss_marketsizeresultpss", ConditionOperator.Equal, mspss.Entities[0].Id);
                    QueryExpression qmapp = new QueryExpression(_DL_tss_marketsizeresultmapping.EntityName);
                    qmapp.ColumnSet = new ColumnSet(true);
                    qmapp.Criteria.AddFilter(fmapp);
                    EntityCollection mapps = _DL_tss_marketsizeresultmapping.Select(organizationService, qmapp);
                    foreach (var mapp in mapps.Entities)
                    {
                        EntityReference keyAccountRef = mapp.GetAttributeValue<EntityReference>("tss_keyaccount");
                        Entity keyAccount = _DL_tss_keyaccount.Select(organizationService, keyAccountRef.Id);
                        _DL_tss_keyaccount.CheckReviseStatus(keyAccount, matrixmsperiod[0], ref startDtMS, ref endDtMS);
                    }
                    #endregion
                    
                    #region calculate total Amount
                    
                    totalJanuary = stPss.GetAttributeValue<Money>("tss_january").Value;
                    totalFebruary = stPss.GetAttributeValue<Money>("tss_february").Value;
                    totalMarch = stPss.GetAttributeValue<Money>("tss_march").Value;
                    totalApril = stPss.GetAttributeValue<Money>("tss_april").Value;
                    totalMay = stPss.GetAttributeValue<Money>("tss_may").Value;
                    totalJune = stPss.GetAttributeValue<Money>("tss_june").Value;
                    totalJuly = stPss.GetAttributeValue<Money>("tss_july").Value;
                    totalAugust = stPss.GetAttributeValue<Money>("tss_august").Value;
                    totalSept = stPss.GetAttributeValue<Money>("tss_september").Value;
                    totalOct = stPss.GetAttributeValue<Money>("tss_october").Value;
                    totalNov = stPss.GetAttributeValue<Money>("tss_november").Value;
                    totalDec = stPss.GetAttributeValue<Money>("tss_december").Value;
                    totalYearly = stPss.GetAttributeValue<Money>("tss_totalyearly").Value;


                    #endregion

                    FilterExpression _filter1 = new FilterExpression(LogicalOperator.Or);
                    _filter1.AddCondition("tss_status", ConditionOperator.Equal, 865920000);
                    _filter1.AddCondition("tss_status", ConditionOperator.Equal, 865920001);

                    FilterExpression fSTB = new FilterExpression(LogicalOperator.And);
                    //2018.09.17 - start = lessequal & end = greaterequal
                    //fSTB.AddCondition("tss_activeperiodsend", ConditionOperator.LessEqual, DateTime.Now);
                    //fSTB.AddCondition("tss_activeperiodstart", ConditionOperator.GreaterEqual, DateTime.Now);
                    fSTB.AddCondition("tss_activeenddate", ConditionOperator.GreaterEqual, DateTime.Now);
                    fSTB.AddCondition("tss_activestartdate", ConditionOperator.LessEqual, DateTime.Now);
                    fSTB.AddCondition("tss_branch", ConditionOperator.Equal, stPss.GetAttributeValue<EntityReference>("tss_branch").Id);
                    //fSTB.AddCondition("tss_status", ConditionOperator.Equal, 865920001);
                    fSTB.AddFilter(_filter1);

                    QueryExpression qSTB = new QueryExpression(_DL_tss_salestargetbranch.EntityName);
                    qSTB.Criteria.AddFilter(fSTB);
                    qSTB.ColumnSet = new ColumnSet(true);
                    EntityCollection enc_SalesTargetBranch = _DL_tss_salestargetbranch.Select(organizationService, qSTB);

                    if (enc_SalesTargetBranch.Entities.Count > 0)
                    {
                        salesTargetBrachID = enc_SalesTargetBranch.Entities[0].Id;
                        enc_SalesTargetBranch.Entities[0]["tss_activeenddate"] = stPss.GetAttributeValue<DateTime>("tss_activeenddate");
                        enc_SalesTargetBranch.Entities[0]["tss_branch"] = new EntityReference("businessunit", stPss.GetAttributeValue<EntityReference>("tss_branch").Id);
                        enc_SalesTargetBranch.Entities[0]["tss_activestartdate"] = stPss.GetAttributeValue<DateTime>("tss_activestartdate");
                        enc_SalesTargetBranch.Entities[0]["tss_msperiodend"] = endDtMS;
                        enc_SalesTargetBranch.Entities[0]["tss_msperiodstart"] = startDtMS;
                        //enc_SalesTargetBranch.Entities[0]["tss_status"] = new OptionSetValue(865920000);
                        //enc_SalesTargetBranch.Entities[0]["tss_statusreason"] = new OptionSetValue(865920000);
                        
                        enc_SalesTargetBranch.Entities[0]["tss_january"] = new Money(enc_SalesTargetBranch.Entities[0].GetAttributeValue<Money>("tss_january").Value + totalJanuary);
                        enc_SalesTargetBranch.Entities[0]["tss_february"] = new Money(enc_SalesTargetBranch.Entities[0].GetAttributeValue<Money>("tss_february").Value + totalFebruary);
                        enc_SalesTargetBranch.Entities[0]["tss_march"] = new Money(enc_SalesTargetBranch.Entities[0].GetAttributeValue<Money>("tss_march").Value + totalMarch);
                        enc_SalesTargetBranch.Entities[0]["tss_april"] = new Money(enc_SalesTargetBranch.Entities[0].GetAttributeValue<Money>("tss_april").Value + totalApril);
                        enc_SalesTargetBranch.Entities[0]["tss_may"] = new Money(enc_SalesTargetBranch.Entities[0].GetAttributeValue<Money>("tss_may").Value + totalMay);
                        enc_SalesTargetBranch.Entities[0]["tss_june"] = new Money(enc_SalesTargetBranch.Entities[0].GetAttributeValue<Money>("tss_june").Value + totalJune);
                        enc_SalesTargetBranch.Entities[0]["tss_july"] = new Money(enc_SalesTargetBranch.Entities[0].GetAttributeValue<Money>("tss_july").Value + totalJuly);
                        enc_SalesTargetBranch.Entities[0]["tss_august"] = new Money(enc_SalesTargetBranch.Entities[0].GetAttributeValue<Money>("tss_august").Value + totalAugust);
                        enc_SalesTargetBranch.Entities[0]["tss_september"] = new Money(enc_SalesTargetBranch.Entities[0].GetAttributeValue<Money>("tss_september").Value + totalSept);
                        enc_SalesTargetBranch.Entities[0]["tss_october"] = new Money(enc_SalesTargetBranch.Entities[0].GetAttributeValue<Money>("tss_october").Value + totalOct);
                        enc_SalesTargetBranch.Entities[0]["tss_november"] = new Money(enc_SalesTargetBranch.Entities[0].GetAttributeValue<Money>("tss_november").Value + totalNov);
                        enc_SalesTargetBranch.Entities[0]["tss_december"] = new Money(enc_SalesTargetBranch.Entities[0].GetAttributeValue<Money>("tss_december").Value + totalDec);
                        enc_SalesTargetBranch.Entities[0]["tss_totalyearly"] = new Money(enc_SalesTargetBranch.Entities[0].GetAttributeValue<Money>("tss_totalyearly").Value + totalYearly);

                        organizationService.Update(enc_SalesTargetBranch.Entities[0]);
                    }
                    else
                    {
                        Entity en_SalesTargetBranch = new Entity(_DL_tss_salestargetbranch.EntityName);
                        en_SalesTargetBranch["tss_branch"] = new EntityReference("businessunit", stPss.GetAttributeValue<EntityReference>("tss_branch").Id);
                        en_SalesTargetBranch["tss_activeenddate"] = stPss.GetAttributeValue<DateTime>("tss_activeenddate");
                        en_SalesTargetBranch["tss_activestartdate"] = stPss.GetAttributeValue<DateTime>("tss_activestartdate");
                        en_SalesTargetBranch["tss_msperiodend"] = startDtMS;
                        en_SalesTargetBranch["tss_msperiodstart"] = startDtMS;
                        en_SalesTargetBranch["tss_status"] = new OptionSetValue(865920000);
                        en_SalesTargetBranch["tss_statusreason"] = new OptionSetValue(865920000);
                        en_SalesTargetBranch["tss_january"] = new Money(totalJanuary);
                        en_SalesTargetBranch["tss_february"] = new Money(totalFebruary);
                        en_SalesTargetBranch["tss_march"] = new Money(totalMarch);
                        en_SalesTargetBranch["tss_april"] = new Money(totalApril);
                        en_SalesTargetBranch["tss_may"] = new Money(totalMay);
                        en_SalesTargetBranch["tss_june"] = new Money(totalJune);
                        en_SalesTargetBranch["tss_july"] = new Money(totalJuly);
                        en_SalesTargetBranch["tss_august"] = new Money(totalAugust);
                        en_SalesTargetBranch["tss_september"] = new Money(totalSept);
                        en_SalesTargetBranch["tss_october"] = new Money(totalOct);
                        en_SalesTargetBranch["tss_november"] = new Money(totalNov);
                        en_SalesTargetBranch["tss_december"] = new Money(totalDec);
                        en_SalesTargetBranch["tss_totalyearly"] = new Money(totalYearly);

                        salesTargetBrachID = organizationService.Create(en_SalesTargetBranch);
                    }
                }
                #endregion
            }
            return salesTargetBrachID;
        }

        public void ApproveSalesTargetBranch_OnClick(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {

            Guid recordID;
            FilterExpression fMs = new FilterExpression(LogicalOperator.And);
            fMs.AddCondition("tss_unittype", ConditionOperator.NotNull);
            fMs.AddCondition("tss_pss", ConditionOperator.Equal, context.UserId);
            fMs.AddCondition("tss_status", ConditionOperator.Equal, STATUS_COMPLETED_MS);

            QueryExpression qMS = new QueryExpression(_DL_tss_mastermarketsize.EntityName);
            qMS.Criteria.AddFilter(fMs);
            qMS.ColumnSet = new ColumnSet(true);
            List<Entity> ms = _retrievehelper.RetrieveMultiple(organizationService, qMS); // _DL_tss_mastermarketsize.Select(organizationService, qMS);
            Guid[] custs = ms.GroupBy(x => x.GetAttributeValue<EntityReference>("tss_customer").Id).Select(x => x.First().GetAttributeValue<EntityReference>("tss_customer").Id).ToArray();

            Entity EN_tss_salestargetbranch = _DL_tss_salestargetbranch.Select(organizationService, context.PrimaryEntityId);

            #region Update status sales target PSS to "Waiting Approval SM"

            FilterExpression fMapp = new FilterExpression(LogicalOperator.And);
            fMapp.AddCondition("tss_salestargetbranch", ConditionOperator.Equal, context.PrimaryEntityId);

            QueryExpression qMapp = new QueryExpression("tss_marketsizeresultmapping");
            qMapp.Criteria.AddFilter(fMapp);
            qMapp.ColumnSet = new ColumnSet(true);
            EntityCollection ENC_tss_marketsizeresultmapping = _DL_tss_marketsizeresultmapping.Select(organizationService, qMapp);

            recordID = _BL_tss_salestargetnation.GenerateSalesTargetNational(organizationService, tracingService, context);
            foreach (Entity en_mapping in ENC_tss_marketsizeresultmapping.Entities)
            {
                en_mapping["tss_salestargetnational"] = new EntityReference("tss_salestargetnational", recordID);
                organizationService.Update(en_mapping);

                Entity EN_tss_salestargetpss = _DL_tss_salestargetpss.Select(organizationService, en_mapping.GetAttributeValue<EntityReference>("tss_salestargetpss").Id);
                EN_tss_salestargetpss["tss_status"] = new OptionSetValue(865920005);//Waiting Approval SM
                organizationService.Update(EN_tss_salestargetpss);

            }
            #endregion


            #region update status sales target national
            Entity entityToUpdate = _DL_tss_salestargetnational.Select(organizationService, recordID);
            entityToUpdate["tss_status"] = new OptionSetValue(865920003);//Waiting Approval SM
            entityToUpdate["tss_statusreason"] = new OptionSetValue(865920001);//Waiting Approval 
            organizationService.Update(entityToUpdate);
            #endregion

            #region insert approver , notif approver and share records


            if (EN_tss_salestargetbranch.Contains("tss_branch"))
            {
                #region Update sales target branch
                EN_tss_salestargetbranch["tss_status"] = new OptionSetValue(865920002);//Waiting Approval SM
                EN_tss_salestargetbranch["tss_approvedate"] = DateTime.Now.ToLocalTime();
                organizationService.Update(EN_tss_salestargetbranch);
                #endregion

                FilterExpression fappMat = new FilterExpression(LogicalOperator.And);
                fappMat.AddCondition("tss_branch", ConditionOperator.Equal, EN_tss_salestargetbranch.GetAttributeValue<EntityReference>("tss_branch").Id);
                fappMat.AddCondition("tss_approvaltype", ConditionOperator.Equal, 865920001);//korwil
                fappMat.AddCondition("tss_approvalfor", ConditionOperator.Equal, 865920001);//sales target

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
                        ENT_approvalListMarketSize["tss_salestargetnational"] = new EntityReference("tss_salestargetnational", recordID);
                        ENT_approvalListMarketSize["tss_type"] = new OptionSetValue(865920005);//Sales Target National

                        organizationService.Create(ENT_approvalListMarketSize);
                        #endregion

                        #region share records
                        ShareRecords objShare = new ShareRecords();
                        Entity en_targetUser = new Entity("systemuser", appMatrix.GetAttributeValue<EntityReference>("tss_approver").Id);

                        objShare.ShareRecord(organizationService, entityToUpdate, en_targetUser);
                        #endregion
                    }

                    TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent objEmail = new TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent();
                    string emailSubject = entityToUpdate.GetAttributeValue<string>("tss_name") + " is waiting for approval";
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

        public void ReviseSalesTargetBranch_OnClick(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {
            DateTime startDtMS = DateTime.MinValue;
            DateTime endDtMS = DateTime.MinValue;

            Entity stBranchToUpdate = _DL_tss_salestargetbranch.Select(organizationService, context.PrimaryEntityId);
            stBranchToUpdate["tss_status"] = new OptionSetValue(TARGET_SALES_BRANCH_STATUS_DRAFT);
            stBranchToUpdate["tss_statusreason"] = new OptionSetValue(TARGET_SALES_BRANCH_STATUS_REASON_OPEN);
            stBranchToUpdate["tss_january"] = new Money(0);
            stBranchToUpdate["tss_february"] = new Money(0);
            stBranchToUpdate["tss_march"] = new Money(0);
            stBranchToUpdate["tss_april"] = new Money(0);
            stBranchToUpdate["tss_may"] = new Money(0);
            stBranchToUpdate["tss_june"] = new Money(0);
            stBranchToUpdate["tss_july"] = new Money(0);
            stBranchToUpdate["tss_august"] = new Money(0);
            stBranchToUpdate["tss_september"] = new Money(0);
            stBranchToUpdate["tss_october"] = new Money(0);
            stBranchToUpdate["tss_november"] = new Money(0);
            stBranchToUpdate["tss_december"] = new Money(0);
            stBranchToUpdate["tss_totalyearly"] = new Money(0);
            stBranchToUpdate["tss_totalallsalesyearly"] = new Money(0);
            stBranchToUpdate["tss_totalamountmarketsizeyearly"] = new Money(0);
            organizationService.Update(stBranchToUpdate);

            #region Update status sales target PSS to "Draft"

            FilterExpression fMapp = new FilterExpression(LogicalOperator.And);
            fMapp.AddCondition("tss_salestargetbranch", ConditionOperator.Equal, context.PrimaryEntityId);

            QueryExpression qMapp = new QueryExpression("tss_marketsizeresultmapping");
            qMapp.Criteria.AddFilter(fMapp);
            qMapp.ColumnSet = new ColumnSet(true);
            EntityCollection ENC_tss_marketsizeresultmapping = _DL_tss_marketsizeresultmapping.Select(organizationService, qMapp);

            foreach (Entity en_mapping in ENC_tss_marketsizeresultmapping.Entities)
            {
                Entity EN_tss_salestargetpss = _DL_tss_salestargetpss.Select(organizationService, en_mapping.GetAttributeValue<EntityReference>("tss_salestargetpss").Id);
                EN_tss_salestargetpss["tss_status"] = new OptionSetValue(TARGET_SALES_PSS_STATUS_DRAFT);//Waiting Approval SM
                organizationService.Update(EN_tss_salestargetpss);

            }
            #endregion

            #region Send Notif
            FilterExpression fappMat = new FilterExpression(LogicalOperator.And);
            fappMat.AddCondition("tss_branch", ConditionOperator.Equal, stBranchToUpdate.GetAttributeValue<EntityReference>("tss_branch").Id);
            fappMat.AddCondition("tss_approvaltype", ConditionOperator.Equal, 865920000);//PDH
            fappMat.AddCondition("tss_approvalfor", ConditionOperator.Equal, 865920001);//sales target

            QueryExpression qappMat = new QueryExpression("tss_matrixapprovalmarketsize");
            qappMat.Criteria.AddFilter(fappMat);
            qappMat.Orders.Add(new Microsoft.Xrm.Sdk.Query.OrderExpression("tss_priorityno", Microsoft.Xrm.Sdk.Query.OrderType.Ascending));
            qappMat.ColumnSet = new ColumnSet(true);

            EntityCollection en_approvalMatrixMarketSize = _DL_tss_matrixapprovalmarketsize.Select(organizationService, qappMat);

            if (en_approvalMatrixMarketSize.Entities.Count > 0)
            {
                TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent objEmail = new TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent();
                string emailSubject = stBranchToUpdate.GetAttributeValue<string>("tss_name") + " is waiting for approval";
                string emailContent = "Please review and approve.";
                objEmail.SendEmailNotif(context.UserId, en_approvalMatrixMarketSize.Entities[0].GetAttributeValue<EntityReference>("tss_approver").Id, Guid.Empty, organizationService, emailSubject, emailContent);

            }
            else
            {
                throw new InvalidWorkflowException("No Approval Matrix found. Please contact your IT administrator.");
            }
            #endregion
        }
    }
}
