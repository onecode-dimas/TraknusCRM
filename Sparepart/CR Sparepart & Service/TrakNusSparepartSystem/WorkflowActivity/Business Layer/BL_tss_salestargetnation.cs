using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrakNusSparepartSystem.DataLayer;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;
using System.Data.SqlClient;
using System.Data;

namespace TrakNusSparepartSystem.WorkflowActivity.Business_Layer
{
    public class BL_tss_salestargetnation
    {
        #region Constant
        private const int UNITTYPE_UIO = 865920000;
        private const int UNITTYPE_NONUIO = 865920001;
        private const int UNITTYPE_COMMODITY = 865920002;
        private const int STATUS_OPEN = 865920000;
        private const int STATUS_DRAFT = 865920000;
        private const int STATUS_COMPLETED_MS = 865920000;
        private const int REVISION = 865920001;
        private const int ST_PSS_STATUS_ACTIVE = 865920003;
        private const int ST_PSS_STATUS_REASON_ACTIVE = 865920001;
        private const int ST_BRANCH_STATUS_ACTIVE = 865920004;
        private const int ST_BRANCH_STATUS_REASON_COMPLETED = 865920002;
        private const int ST_NATIONAL_STATUS_ACTIVE = 865920001;
        private const int ST_NATIONAL_STATUS_REASON_COMPLETED = 865920002;

        //private const int KEY_ACCOUNT_STATUS_ACTIVE = 865920003;
        //private const int KEY_ACCOUNT_STATUS_REASON_ACTIVE = 865920001;
        //private const int KEY_ACCOUNT_STATUS_REVISE = 865920004;
        //private const int KEY_ACCOUNT_VERSION_REVISE = 865920001;
        private const int ST_NATIONAL_STATUS_DRAFT = 865920000;
        private const int ST_NATIONAL_STATUS_REASON_OPEN = 865920000;
        //private const int TARGET_SALES_PSS_STATUS_DRAFT = 865920000;
        //private const int TARGET_SALES_PSS_STATUS_REASON_OPEN = 865920000;
        #endregion

        #region Dependencies
        DL_user _DL_user = new DL_user();
        DL_account _DL_account = new DL_account();
        DL_tss_marketsizeresultpss _DL_tss_marketsizeresultpss = new DL_tss_marketsizeresultpss();
        DL_tss_marketsizeresultbranch _DL_tss_marketsizeresultbranch = new DL_tss_marketsizeresultbranch();
        DL_tss_keyaccount _DL_tss_keyaccount = new DL_tss_keyaccount();
        DL_tss_mastermarketsize _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        DL_tss_mastermarketsizelines _DL_tss_mastermarketsizelines = new DL_tss_mastermarketsizelines();
        DL_tss_mastermarketsizesublines _DL_tss_mastermarketsizesublines = new DL_tss_mastermarketsizesublines();
        DL_tss_sparepartsetup _DL_tss_sparepartsetup = new DL_tss_sparepartsetup();
        DL_tss_salestargetnational _DL_tss_salestargetnational = new DL_tss_salestargetnational();
        DL_tss_salestargetbranch _DL_tss_salestargetbranch = new DL_tss_salestargetbranch();
        DL_tss_marketsizeresultmapping _DL_tss_marketsizeresultmapping = new DL_tss_marketsizeresultmapping();
        DL_tss_salestargetpss _DL_tss_salestargetpss = new DL_tss_salestargetpss();
        DL_tss_matrixapprovalmarketsize _DL_tss_matrixapprovalmarketsize = new DL_tss_matrixapprovalmarketsize();

        BL_tss_totalpartconsumpmarketsize _BL_tss_totalpartconsumpmarketsize = new BL_tss_totalpartconsumpmarketsize();
        BL_tss_salesactualbranch _BL_tss_salesactualbranch = new BL_tss_salesactualbranch();
        BL_tss_salesactualpss _BL_tss_salesactualpss = new BL_tss_salesactualpss();
        BL_tss_salesactualnational _BL_tss_salesactualnational = new BL_tss_salesactualnational();
        BL_tss_potentialprospectpart _BL_tss_potentialprospectpart = new BL_tss_potentialprospectpart();
        #endregion


        public Guid GenerateSalesTargetNational(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {

            DateTime startDtMS = DateTime.MinValue;
            DateTime endDtMS = DateTime.MinValue;
            DateTime endDtActive = DateTime.MinValue;
            DateTime startDtActive = DateTime.MinValue;
            Guid salesTargetNationalPrimayKey;
            DL_tss_salestargetnational _DL_tss_salestargetnational = new DL_tss_salestargetnational();

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

            Entity stBranch = _DL_tss_salestargetbranch.Select(organizationService, context.PrimaryEntityId);

          
            endDtActive = stBranch.GetAttributeValue<DateTime>("tss_activeenddate");
            startDtActive = stBranch.GetAttributeValue<DateTime>("tss_activestartdate");

            FilterExpression _filter1 = new FilterExpression(LogicalOperator.Or);
            _filter1.AddCondition("tss_status", ConditionOperator.Equal, 865920000);
            _filter1.AddCondition("tss_status", ConditionOperator.Equal, 865920003);

            FilterExpression fMSN = new FilterExpression(LogicalOperator.And);
            fMSN.AddCondition("tss_activeenddate", ConditionOperator.GreaterEqual, DateTime.Now);
            fMSN.AddCondition("tss_activestartdate", ConditionOperator.LessEqual, DateTime.Now);
            //fMSN.AddCondition("tss_status", ConditionOperator.Equal, 865920003);
            fMSN.AddFilter(_filter1);
            QueryExpression qMSN = new QueryExpression(_DL_tss_salestargetnational.EntityName);
            qMSN.Criteria.AddFilter(fMSN);
            qMSN.ColumnSet = new ColumnSet(true);

            EntityCollection stnational = _DL_tss_salestargetnational.Select(organizationService, qMSN);

            #region Check Revise on KA
            FilterExpression fmappms = new FilterExpression(LogicalOperator.And);
            fmappms.AddCondition("tss_salestargetbranch", ConditionOperator.Equal, stBranch.Id);
            QueryExpression qmappms = new QueryExpression(_DL_tss_marketsizeresultmapping.EntityName);
            qmappms.ColumnSet = new ColumnSet(true);
            qmappms.Criteria.AddFilter(fmappms);
            EntityCollection mappsms = _DL_tss_marketsizeresultmapping.Select(organizationService, qmappms);

            if (mappsms.Entities.Count == 0)
                throw new Exception("Market size branch not found at mapping entity");

            Entity stpss = _DL_tss_salestargetpss.Select(organizationService, mappsms.Entities[0].GetAttributeValue<EntityReference>("tss_salestargetpss").Id);
            Entity msPss = _DL_tss_marketsizeresultpss.Select(organizationService, stpss.GetAttributeValue<EntityReference>("tss_marketsizeid").Id);

            FilterExpression fmapp = new FilterExpression(LogicalOperator.And);
            fmapp.AddCondition("tss_marketsizeresultpss", ConditionOperator.Equal, msPss.Id);
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


            if (stnational.Entities.Count > 0)
            {
                Entity entityToUpdate = stnational.Entities[0];
                salesTargetNationalPrimayKey = entityToUpdate.Id;
                entityToUpdate["tss_msperiodend"] = endDtMS;
                entityToUpdate["tss_msperiodstart"] = startDtMS;
                entityToUpdate["tss_activeenddate"] = endDtActive;
                entityToUpdate["tss_activestartdate"] = startDtActive;
                entityToUpdate["tss_january"] = new Money(entityToUpdate.GetAttributeValue<Money>("tss_january").Value+ stBranch.GetAttributeValue<Money>("tss_january").Value);
                entityToUpdate["tss_february"] = new Money(entityToUpdate.GetAttributeValue<Money>("tss_february").Value+ stBranch.GetAttributeValue<Money>("tss_february").Value);
                entityToUpdate["tss_march"] = new Money(entityToUpdate.GetAttributeValue<Money>("tss_march").Value+ stBranch.GetAttributeValue<Money>("tss_march").Value);
                entityToUpdate["tss_april"] = new Money(entityToUpdate.GetAttributeValue<Money>("tss_april").Value+ stBranch.GetAttributeValue<Money>("tss_april").Value);
                entityToUpdate["tss_may"] = new Money(entityToUpdate.GetAttributeValue<Money>("tss_may").Value+ stBranch.GetAttributeValue<Money>("tss_may").Value);
                entityToUpdate["tss_june"] = new Money(entityToUpdate.GetAttributeValue<Money>("tss_june").Value+ stBranch.GetAttributeValue<Money>("tss_june").Value);
                entityToUpdate["tss_july"] = new Money(entityToUpdate.GetAttributeValue<Money>("tss_july").Value+ stBranch.GetAttributeValue<Money>("tss_july").Value);
                entityToUpdate["tss_august"] = new Money(entityToUpdate.GetAttributeValue<Money>("tss_august").Value+ stBranch.GetAttributeValue<Money>("tss_august").Value);
                entityToUpdate["tss_september"] = new Money(entityToUpdate.GetAttributeValue<Money>("tss_september").Value+ stBranch.GetAttributeValue<Money>("tss_september").Value);
                entityToUpdate["tss_october"] = new Money(entityToUpdate.GetAttributeValue<Money>("tss_october").Value+ stBranch.GetAttributeValue<Money>("tss_october").Value);
                entityToUpdate["tss_november"] = new Money(entityToUpdate.GetAttributeValue<Money>("tss_november").Value+ stBranch.GetAttributeValue<Money>("tss_november").Value);
                entityToUpdate["tss_december"] = new Money(entityToUpdate.GetAttributeValue<Money>("tss_december").Value+ stBranch.GetAttributeValue<Money>("tss_december").Value);
                entityToUpdate["tss_totalyearly"] = new Money(entityToUpdate.GetAttributeValue<Money>("tss_totalyearly").Value+ stBranch.GetAttributeValue<Money>("tss_totalyearly").Value);
                entityToUpdate["tss_totalallsalesyearly"] = new Money(entityToUpdate.GetAttributeValue<Money>("tss_totalallsalesyearly").Value+ stBranch.GetAttributeValue<Money>("tss_totalallsalesyearly").Value);
                entityToUpdate["tss_totalamountmarketsizeyearly"] = new Money(entityToUpdate.GetAttributeValue<Money>("tss_totalamountmarketsizeyearly").Value+ stBranch.GetAttributeValue<Money>("tss_totalamountmarketsizeyearly").Value);
                organizationService.Update(entityToUpdate);
            }
            else
            {
                Entity entityToInsert = new Entity(_DL_tss_salestargetnational.EntityName);
                entityToInsert["tss_status"] = new OptionSetValue(865920003);//Waiting Approval SM;
                entityToInsert["tss_statusreason"] = new OptionSetValue(865920001);
                entityToInsert["tss_msperiodend"] = endDtMS;
                entityToInsert["tss_msperiodstart"] = startDtMS;
                entityToInsert["tss_activeenddate"] = endDtActive;
                entityToInsert["tss_activestartdate"] = startDtActive;
                entityToInsert["tss_january"] = new Money(stBranch.GetAttributeValue<Money>("tss_january").Value);
                entityToInsert["tss_february"] = new Money(stBranch.GetAttributeValue<Money>("tss_february").Value);
                entityToInsert["tss_march"] = new Money(stBranch.GetAttributeValue<Money>("tss_march").Value);
                entityToInsert["tss_april"] = new Money(stBranch.GetAttributeValue<Money>("tss_april").Value);
                entityToInsert["tss_may"] = new Money(stBranch.GetAttributeValue<Money>("tss_may").Value);
                entityToInsert["tss_june"] = new Money(stBranch.GetAttributeValue<Money>("tss_june").Value);
                entityToInsert["tss_july"] = new Money(stBranch.GetAttributeValue<Money>("tss_july").Value);
                entityToInsert["tss_august"] = new Money(stBranch.GetAttributeValue<Money>("tss_august").Value);
                entityToInsert["tss_september"] = new Money(stBranch.GetAttributeValue<Money>("tss_september").Value);
                entityToInsert["tss_october"] = new Money(stBranch.GetAttributeValue<Money>("tss_october").Value);
                entityToInsert["tss_november"] = new Money(stBranch.GetAttributeValue<Money>("tss_november").Value);
                entityToInsert["tss_december"] = new Money(stBranch.GetAttributeValue<Money>("tss_december").Value);
                entityToInsert["tss_totalyearly"] = new Money(stBranch.GetAttributeValue<Money>("tss_totalyearly").Value);
                entityToInsert["tss_totalallsalesyearly"] = new Money(stBranch.GetAttributeValue<Money>("tss_totalallsalesyearly").Value);
                entityToInsert["tss_totalamountmarketsizeyearly"] = new Money(stBranch.GetAttributeValue<Money>("tss_totalamountmarketsizeyearly").Value);
                salesTargetNationalPrimayKey = organizationService.Create(entityToInsert);
            }
            
            return salesTargetNationalPrimayKey;
        }

        public void ApproveSalesTargetNational_OnClick(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {
            DateTime startPeriodMS = DateTime.Now.Date;
            DateTime endPeriodMS = DateTime.Now.Date;
            DateTime startDtMS = DateTime.Now;
            DateTime endDtMS = DateTime.Now;
            FilterExpression filter;
            QueryExpression query;
            Guid salesTargetNationalId = context.PrimaryEntityId;
            #region Update status market size result National to  Active   & Completed
            query = new QueryExpression(_DL_tss_salestargetnational.EntityName);
            query.ColumnSet = new ColumnSet(true);

            Entity entitySalesTargetNational = _DL_tss_salestargetnational.Select(organizationService, salesTargetNationalId);
            entitySalesTargetNational["tss_status"] = new OptionSetValue(ST_NATIONAL_STATUS_ACTIVE);
            entitySalesTargetNational["tss_statusreason"] = new OptionSetValue(ST_NATIONAL_STATUS_REASON_COMPLETED);
            entitySalesTargetNational["tss_approvedate"] = DateTime.Now.ToLocalTime();
            organizationService.Update(entitySalesTargetNational);
            _BL_tss_salesactualnational.GenerateSalesActualNational(organizationService, tracingService, context, entitySalesTargetNational.Id);
            #endregion

            #region Retrieve market size result mapping
            filter = new FilterExpression(LogicalOperator.And);
            filter.AddCondition("tss_salestargetnational", ConditionOperator.Equal, salesTargetNationalId);

            query = new QueryExpression(_DL_tss_marketsizeresultmapping.EntityName);
            query.Criteria.AddFilter(filter);
            query.ColumnSet = new ColumnSet(true);
            EntityCollection ENC_tss_marketsizeresultmapping = _DL_tss_marketsizeresultmapping.Select(organizationService, query);

            foreach (Entity en_mapping in ENC_tss_marketsizeresultmapping.Entities)
            {
                /*Retrieve and update for each entity*/
                #region Update status market size result PSS to  Active & Active
                Entity entity_ts_pss = _DL_tss_salestargetpss.Select(organizationService, en_mapping.GetAttributeValue<EntityReference>(_DL_tss_salestargetpss.EntityName).Id);
                entity_ts_pss["tss_status"] = new OptionSetValue(ST_PSS_STATUS_ACTIVE);
                entity_ts_pss["tss_statusreason"] = new OptionSetValue(ST_PSS_STATUS_REASON_ACTIVE);
                organizationService.Update(entity_ts_pss);
                _BL_tss_salesactualpss.GenerateSalesActualPSS(organizationService, tracingService, context, entity_ts_pss.Id);
                #endregion


                //#region Update status Key Account to  Active (865920003) & Active (865920001)
                //Entity entity_keyAccount = _DL_tss_keyaccount.Select(organizationService, entity_pss.GetAttributeValue<EntityReference>(_DL_tss_keyaccount.EntityName).Id);
                //entity_keyAccount["tss_status"] = new OptionSetValue(KEY_ACCOUNT_STATUS_ACTIVE);
                //entity_keyAccount["tss_reason"] = new OptionSetValue(KEY_ACCOUNT_STATUS_REASON_ACTIVE);
                //organizationService.Update(entity_keyAccount);
                //#endregion

                #region Update status market size result Branch to  Active & Completed
                Entity entity_ts_branch = _DL_tss_salestargetbranch.Select(organizationService, en_mapping.GetAttributeValue<EntityReference>(_DL_tss_salestargetbranch.EntityName).Id);
                entity_ts_branch["tss_status"] = new OptionSetValue(ST_BRANCH_STATUS_ACTIVE);
                entity_ts_branch["tss_statusreason"] = new OptionSetValue(ST_BRANCH_STATUS_REASON_COMPLETED);
                organizationService.Update(entity_ts_branch);
                _BL_tss_salesactualbranch.GenerateSalesActualBranch(organizationService, tracingService, context, entity_ts_branch.Id);
                #endregion

            }
            #endregion

            //2019.09.25
            //_BL_tss_potentialprospectpart.GeneratePotentialProspectPart(organizationService, tracingService, context);

            List<SqlParameter> _sqlparameters = new List<SqlParameter>()
                {
                    //new SqlParameter("@tss_marketsizeresultnational", SqlDbType.NVarChar) { Value = _workflowcontext.PrimaryEntityId.ToString().Replace("{", "").Replace("}", "") },
                    new SqlParameter("@tss_salestargetnational", SqlDbType.NVarChar) { Value = salesTargetNationalId.ToString().Replace("{", "").Replace("}", "") },
                    new SqlParameter("@systemuserid", SqlDbType.NVarChar) { Value = context.UserId.ToString().Replace("{", "").Replace("}", "") },
                };

            DataTable _datatable = new GetStoredProcedure().Connect("sp_ms_ApproveMS_SalesTargetNational", _sqlparameters, false);






            // _BL_tss_totalpartconsumpmarketsize.GenerateTotalPartConsumpMarketSize(organizationService, tracingService, context);

            #region Generate Email to PSS and All previous approver base on Market Size Matrix Approver to inform them that the market size has been apprved
            //QueryExpression qappMat = new QueryExpression(_DL_tss_matrixapprovalmarketsize.EntityName);
            //qappMat.Orders.Add(new Microsoft.Xrm.Sdk.Query.OrderExpression("tss_priorityno", Microsoft.Xrm.Sdk.Query.OrderType.Ascending));
            //qappMat.ColumnSet = new ColumnSet(true);

            //EntityCollection entityApprovalMatrixMarketSize = _DL_tss_matrixapprovalmarketsize.Select(organizationService, qappMat);
            //if (entityApprovalMatrixMarketSize.Entities.Count > 0)
            //{
            //    foreach (var entity in entityApprovalMatrixMarketSize.Entities)
            //    {
            //        TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent objEmail = new TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent();
            //        string emailSubject = entityMarketSizeNational.GetAttributeValue<string>("tss_name") + " has been approved";
            //        string emailContent = "Market Size has been approved";
            //        objEmail.SendEmailNotif(context.UserId, entity.GetAttributeValue<EntityReference>("tss_approver").Id, Guid.Empty, organizationService, emailSubject, emailContent);
            //    }
            //}
            //else
            //{
            //    throw new InvalidWorkflowException("No Approval Matrix found. Please contact your IT administrator.");
            //}
            #endregion
        }

        public void ReviseSalesTargetNational_OnClick(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {
            //#region 2018.09.17 | Update Sales Target National
            //QueryExpression query;
            //FilterExpression filter;
            //Guid salesTargetNationalId = context.PrimaryEntityId;

            //Entity entitySalesTargetNational = _DL_tss_salestargetnational.Select(organizationService, salesTargetNationalId);
            //entitySalesTargetNational["tss_status"] = new OptionSetValue(STATUS_DRAFT);
            //entitySalesTargetNational["tss_statusreason"] = new OptionSetValue(STATUS_OPEN);
            //organizationService.Update(entitySalesTargetNational);
            //#endregion

            //#region 2018.09.17 | Update Status Sales Target National Result PSS & Branch to Status = Draft, StatusReason = Open
            //filter = new FilterExpression(LogicalOperator.And);
            //filter.AddCondition("tss_salestargetnational", ConditionOperator.Equal, salesTargetNationalId);

            //query = new QueryExpression(_DL_tss_marketsizeresultmapping.EntityName);
            //query.Criteria.AddFilter(filter);
            //query.ColumnSet = new ColumnSet(true);
            //EntityCollection ENC_tss_marketsizeresultmapping = _DL_tss_marketsizeresultmapping.Select(organizationService, query);

            //foreach (Entity en_mapping in ENC_tss_marketsizeresultmapping.Entities)
            //{
            //    #region 2018.09.17 | Update Sales Target National Result PSS
            //    Entity entity_ts_pss = _DL_tss_salestargetpss.Select(organizationService, en_mapping.GetAttributeValue<EntityReference>(_DL_tss_salestargetpss.EntityName).Id);
            //    entity_ts_pss["tss_status"] = new OptionSetValue(STATUS_DRAFT);
            //    entity_ts_pss["tss_statusreason"] = new OptionSetValue(STATUS_OPEN);
            //    organizationService.Update(entity_ts_pss);
            //    #endregion

            //    #region 2018.09.17 | Update Sales Target National Result Branch
            //    Entity entity_ts_branch = _DL_tss_salestargetbranch.Select(organizationService, en_mapping.GetAttributeValue<EntityReference>(_DL_tss_salestargetbranch.EntityName).Id);
            //    entity_ts_branch["tss_status"] = new OptionSetValue(ST_BRANCH_STATUS_ACTIVE);
            //    entity_ts_branch["tss_statusreason"] = new OptionSetValue(ST_BRANCH_STATUS_REASON_COMPLETED);
            //    organizationService.Update(entity_ts_branch);
            //    #endregion
            //}
            //#endregion

            //#region 2018.09.17 | Generate Email to PSS and all previous approver base on Market Size Matrix Approver to inform them that the market size has been revised.
            //QueryExpression qappMat = new QueryExpression(_DL_tss_matrixapprovalmarketsize.EntityName);
            //qappMat.Orders.Add(new Microsoft.Xrm.Sdk.Query.OrderExpression("tss_priorityno", Microsoft.Xrm.Sdk.Query.OrderType.Ascending));
            //qappMat.ColumnSet = new ColumnSet(true);

            //EntityCollection entityApprovalMatrixMarketSize = _DL_tss_matrixapprovalmarketsize.Select(organizationService, qappMat);
            //if (entityApprovalMatrixMarketSize.Entities.Count > 0)
            //{
            //    foreach (var entity in entityApprovalMatrixMarketSize.Entities)
            //    {
            //        TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent objEmail = new TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent();
            //        string emailSubject = entitySalesTargetNational.GetAttributeValue<string>("tss_name") + " has been revised";
            //        string emailContent = "Sales Target National has been revised";
            //        objEmail.SendEmailNotif(context.UserId, entity.GetAttributeValue<EntityReference>("tss_approver").Id, Guid.Empty, organizationService, emailSubject, emailContent);
            //    }
            //}
            //else
            //{
            //    throw new InvalidWorkflowException("No Approval Matrix found. Please contact your IT administrator.");
            //}
            //#endregion

            #region get and update status current Sales Target National
            Entity En_CurrSalesTargetNational = _DL_tss_salestargetnational.Select(organizationService, context.PrimaryEntityId);
            En_CurrSalesTargetNational["tss_status"] = new OptionSetValue(STATUS_DRAFT);
            En_CurrSalesTargetNational["tss_statusreason"] = new OptionSetValue(STATUS_OPEN);
            En_CurrSalesTargetNational["tss_january"] = new Money(0);
            En_CurrSalesTargetNational["tss_february"] = new Money(0);
            En_CurrSalesTargetNational["tss_march"] = new Money(0);
            En_CurrSalesTargetNational["tss_april"] = new Money(0);
            En_CurrSalesTargetNational["tss_may"] = new Money(0);
            En_CurrSalesTargetNational["tss_june"] = new Money(0);
            En_CurrSalesTargetNational["tss_july"] = new Money(0);
            En_CurrSalesTargetNational["tss_august"] = new Money(0);
            En_CurrSalesTargetNational["tss_september"] = new Money(0);
            En_CurrSalesTargetNational["tss_october"] = new Money(0);
            En_CurrSalesTargetNational["tss_november"] = new Money(0);
            En_CurrSalesTargetNational["tss_december"] = new Money(0);
            En_CurrSalesTargetNational["tss_totalyearly"] = new Money(0);
            En_CurrSalesTargetNational["tss_totalallsalesyearly"] = new Money(0);
            En_CurrSalesTargetNational["tss_totalamountmarketsizeyearly"] = new Money(0);

            organizationService.Update(En_CurrSalesTargetNational);
            #endregion

            #region Get Sales Target National
            FilterExpression fMMSR = new FilterExpression(LogicalOperator.And);
            fMMSR.AddCondition("tss_salestargetnational", ConditionOperator.Equal, context.PrimaryEntityId);

            QueryExpression qMMSR = new QueryExpression(_DL_tss_marketsizeresultmapping.EntityName);
            qMMSR.Criteria.AddFilter(fMMSR);
            qMMSR.ColumnSet = new ColumnSet(true);

            EntityCollection ENC_MarketSizeResultMapping = _DL_tss_marketsizeresultmapping.Select(organizationService, qMMSR);

            //GET BRANCH & PSS COLLECTION
            var listSalesTargetBranchID = ENC_MarketSizeResultMapping.Entities.Select(x => (EntityReference)x["tss_salestargetbranch"]).Distinct().ToArray();

            FilterExpression fMMB = new FilterExpression(LogicalOperator.Or);

            foreach (var salesTargetBranchID in listSalesTargetBranchID)
            {
                fMMB.AddCondition("tss_salestargetbranch", ConditionOperator.Equal, salesTargetBranchID.Id);
            }

            QueryExpression qMMB = new QueryExpression(_DL_tss_marketsizeresultmapping.EntityName);
            qMMB.Criteria.AddFilter(fMMB);
            qMMB.ColumnSet = new ColumnSet(true);

            EntityCollection ENC_SalesTargetMappingBranch = _DL_tss_marketsizeresultmapping.Select(organizationService, qMMB);

            var listSalesTargetPSSID = ENC_SalesTargetMappingBranch.Entities.Select(x => (EntityReference)x["tss_salestargetpss"]).Distinct().ToArray();

            //UPDATE BRANCH
            foreach (var item in listSalesTargetBranchID)
            {
                Entity salesTargetBranch = _DL_tss_salestargetbranch.Select(organizationService, item.Id);

                salesTargetBranch["tss_status"] = new OptionSetValue(STATUS_DRAFT);
                salesTargetBranch["tss_statusreason"] = new OptionSetValue(STATUS_OPEN);
                salesTargetBranch["tss_january"] = new Money(0);
                salesTargetBranch["tss_february"] = new Money(0);
                salesTargetBranch["tss_march"] = new Money(0);
                salesTargetBranch["tss_april"] = new Money(0);
                salesTargetBranch["tss_may"] = new Money(0);
                salesTargetBranch["tss_june"] = new Money(0);
                salesTargetBranch["tss_july"] = new Money(0);
                salesTargetBranch["tss_august"] = new Money(0);
                salesTargetBranch["tss_september"] = new Money(0);
                salesTargetBranch["tss_october"] = new Money(0);
                salesTargetBranch["tss_november"] = new Money(0);
                salesTargetBranch["tss_december"] = new Money(0);
                salesTargetBranch["tss_totalyearly"] = new Money(0);
                salesTargetBranch["tss_totalallsalesyearly"] = new Money(0);
                salesTargetBranch["tss_totalamountmarketsizeyearly"] = new Money(0);

                organizationService.Update(salesTargetBranch);
            }

            //UPDATE PSS
            foreach (var item in listSalesTargetPSSID)
            {
                Entity salesTargetPSS = _DL_tss_salestargetpss.Select(organizationService, item.Id);

                salesTargetPSS["tss_status"] = new OptionSetValue(STATUS_DRAFT);
                salesTargetPSS["tss_statusreason"] = new OptionSetValue(STATUS_OPEN);

                organizationService.Update(salesTargetPSS);
            }

            #endregion

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
                    string emailSubject = En_CurrSalesTargetNational.GetAttributeValue<string>("tss_name") + " has been revised";
                    string emailContent = "Sales Target National has been revised";
                    objEmail.SendEmailNotif(context.UserId, entity.GetAttributeValue<EntityReference>("tss_approver").Id, Guid.Empty, organizationService, emailSubject, emailContent);
                }
            }
            else
            {
                throw new InvalidWorkflowException("No Approval Matrix found. Please contact your IT administrator.");
            }
            #endregion
        }

        //public void ReviseSalesTargetNational_OnClick(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        //{
        //    #region 2018.09.17 | Update Sales Target National
        //    QueryExpression query;
        //    FilterExpression filter;
        //    Guid salesTargetNationalId = context.PrimaryEntityId;

        //    Entity entitySalesTargetNational = _DL_tss_salestargetnational.Select(organizationService, salesTargetNationalId);
        //    entitySalesTargetNational["tss_status"] = new OptionSetValue(STATUS_DRAFT);
        //    entitySalesTargetNational["tss_statusreason"] = new OptionSetValue(STATUS_OPEN);
        //    organizationService.Update(entitySalesTargetNational);
        //    #endregion

        //    #region 2018.09.17 | Update Status Sales Target National Result PSS & Branch to Status = Draft, StatusReason = Open
        //    filter = new FilterExpression(LogicalOperator.And);
        //    filter.AddCondition("tss_salestargetnational", ConditionOperator.Equal, salesTargetNationalId);

        //    query = new QueryExpression(_DL_tss_marketsizeresultmapping.EntityName);
        //    query.Criteria.AddFilter(filter);
        //    query.ColumnSet = new ColumnSet(true);
        //    EntityCollection ENC_tss_marketsizeresultmapping = _DL_tss_marketsizeresultmapping.Select(organizationService, query);

        //    foreach (Entity en_mapping in ENC_tss_marketsizeresultmapping.Entities)
        //    {
        //        #region 2018.09.17 | Update Sales Target National Result PSS
        //        Entity entity_ts_pss = _DL_tss_salestargetpss.Select(organizationService, en_mapping.GetAttributeValue<EntityReference>(_DL_tss_salestargetpss.EntityName).Id);
        //        entity_ts_pss["tss_status"] = new OptionSetValue(STATUS_DRAFT);
        //        entity_ts_pss["tss_statusreason"] = new OptionSetValue(STATUS_OPEN);
        //        organizationService.Update(entity_ts_pss);
        //        #endregion

        //        #region 2018.09.17 | Update Sales Target National Result Branch
        //        Entity entity_ts_branch = _DL_tss_salestargetbranch.Select(organizationService, en_mapping.GetAttributeValue<EntityReference>(_DL_tss_salestargetbranch.EntityName).Id);
        //        entity_ts_branch["tss_status"] = new OptionSetValue(ST_BRANCH_STATUS_ACTIVE);
        //        entity_ts_branch["tss_statusreason"] = new OptionSetValue(ST_BRANCH_STATUS_REASON_COMPLETED);
        //        organizationService.Update(entity_ts_branch);
        //        #endregion
        //    }
        //    #endregion

        //    #region 2018.09.17 | Generate Email to PSS and all previous approver base on Market Size Matrix Approver to inform them that the market size has been revised.
        //    QueryExpression qappMat = new QueryExpression(_DL_tss_matrixapprovalmarketsize.EntityName);
        //    qappMat.Orders.Add(new Microsoft.Xrm.Sdk.Query.OrderExpression("tss_priorityno", Microsoft.Xrm.Sdk.Query.OrderType.Ascending));
        //    qappMat.ColumnSet = new ColumnSet(true);

        //    EntityCollection entityApprovalMatrixMarketSize = _DL_tss_matrixapprovalmarketsize.Select(organizationService, qappMat);
        //    if (entityApprovalMatrixMarketSize.Entities.Count > 0)
        //    {
        //        foreach (var entity in entityApprovalMatrixMarketSize.Entities)
        //        {
        //            TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent objEmail = new TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent();
        //            string emailSubject = entitySalesTargetNational.GetAttributeValue<string>("tss_name") + " has been revised";
        //            string emailContent = "Sales Target National has been revised";
        //            objEmail.SendEmailNotif(context.UserId, entity.GetAttributeValue<EntityReference>("tss_approver").Id, Guid.Empty, organizationService, emailSubject, emailContent);
        //        }
        //    }
        //    else
        //    {
        //        throw new InvalidWorkflowException("No Approval Matrix found. Please contact your IT administrator.");
        //    }
        //    #endregion
        //}
    }
}
