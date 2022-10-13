using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrakNusSparepartSystem.DataLayer;
using TrakNusSparepartSystem.WorkflowActivity.Helper;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System.Globalization;
using TrakNusSparepartSystem.WorkflowActivity.Interface;
using TrakNusSparepartSystem.WorkflowActivity.Business_Layer;
using TrakNusSparepartSystem.Helper;
using Microsoft.Xrm.Sdk.Client;
using System.Data.SqlClient;
using System.Data;

namespace TrakNusSparepartSystem.WorkflowActivity.BusinessLayer
{
    public class BL_tss_marketsizeresultnational
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
        private const int GROUP_UIO_COMMODITY = 865920002;
        private const int MS_STATUS_COMPLETE = 865920000;
        #endregion

        #region Dependencies
        DL_user _DL_user = new DL_user();
        DL_account _DL_account = new DL_account();
        DL_tss_approvallistmarketsize _DL_tss_approvallistmarketsize = new DL_tss_approvallistmarketsize();
        DL_tss_marketsizeresultpss _DL_tss_marketsizeresultpss = new DL_tss_marketsizeresultpss();
        DL_tss_marketsizeresultbranch _DL_tss_marketsizeresultbranch = new DL_tss_marketsizeresultbranch();
        DL_tss_marketsizeresultnational _DL_tss_marketsizeresultnational = new DL_tss_marketsizeresultnational();
        DL_tss_keyaccount _DL_tss_keyaccount = new DL_tss_keyaccount();
        DL_tss_mastermarketsize _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        DL_tss_mastermarketsizelines _DL_tss_mastermarketsizelines = new DL_tss_mastermarketsizelines();
        DL_tss_mastermarketsizesublines _DL_tss_mastermarketsizesublines = new DL_tss_mastermarketsizesublines();
        DL_tss_sparepartsetup _DL_tss_sparepartsetup = new DL_tss_sparepartsetup();
        DL_tss_marketsizeresultmapping _DL_tss_marketsizeresultmapping = new DL_tss_marketsizeresultmapping();
        DL_tss_matrixapprovalmarketsize _DL_tss_matrixapprovalmarketsize = new DL_tss_matrixapprovalmarketsize();
        DL_tss_salestargetnational _DL_tss_salestargetnational = new DL_tss_salestargetnational();
        DL_tss_salestargetbranch _DL_tss_salestargetbranch = new DL_tss_salestargetbranch();
        DL_tss_salestargetpss _DL_tss_salestargetpss = new DL_tss_salestargetpss();
        BL_KeyAccount _BL_KeyAccount = new BL_KeyAccount();
        BL_tss_mastermarketsize _BL_tss_mastermarketsize = new BL_tss_mastermarketsize();
        EntityCollection pssToRevise = new EntityCollection();
        RetrieveHelper _retrievehelper = new RetrieveHelper();
        #endregion

        #region Variable Global
        private decimal[] targetSalesMarketSize;
        decimal[] targetSalesMarketSizeToInsert;
        private DateTime
            msPeriodStart = DateTime.MinValue,
            msPeriodEnd = DateTime.MinValue,
            activeStartDate = DateTime.MinValue,
            activeEndDate = DateTime.MinValue;
        private decimal targetMSTotalYearly = 0m;
        private ICalculate Calculate;
        #endregion

        public void ApproveMarketSizeResultNational_UsingSP_OnClick(IOrganizationService _organizationservice, ITracingService _tracingservice, IWorkflowContext _workflowcontext, string _recordids)
        {
            using (OrganizationServiceContext _context = new OrganizationServiceContext(_organizationservice))
            {
                //throw new InvalidWorkflowException(_recordids);
                List<SqlParameter> _sqlparameters = new List<SqlParameter>()
                {
                    //new SqlParameter("@tss_marketsizeresultnational", SqlDbType.NVarChar) { Value = _workflowcontext.PrimaryEntityId.ToString().Replace("{", "").Replace("}", "") },
                    new SqlParameter("@tss_marketsizeresultnational", SqlDbType.NVarChar) { Value = _recordids },
                    new SqlParameter("@systemuserid", SqlDbType.NVarChar) { Value = _workflowcontext.UserId.ToString().Replace("{", "").Replace("}", "") },
                };

                DataTable _datatable = new GetStoredProcedure().Connect("sp_ms_ApproveMS_MarketSizeResultNational", _sqlparameters, false);

                #region Generate Email to PSS and All previous approver base on Market Size Matrix Approver to inform them that the market size has been apprved
                Entity _marketsizeresultnational = _DL_tss_marketsizeresultnational.Select(_organizationservice, new Guid(_recordids));
                QueryExpression _queryexpression = new QueryExpression(_DL_tss_matrixapprovalmarketsize.EntityName);
                _queryexpression.Orders.Add(new OrderExpression("tss_priorityno", OrderType.Ascending));
                _queryexpression.ColumnSet = new ColumnSet(true);

                EntityCollection _matrixapprovalmarketsize = _DL_tss_matrixapprovalmarketsize.Select(_organizationservice, _queryexpression);

                if (_matrixapprovalmarketsize.Entities.Count > 0)
                {
                    foreach (var entity in _matrixapprovalmarketsize.Entities)
                    {
                        Helper.EmailAgent objEmail = new Helper.EmailAgent();
                        string emailSubject = _marketsizeresultnational.GetAttributeValue<string>("tss_name") + " has been approved";
                        string emailContent = "Market Size has been approved";

                        objEmail.SendEmailNotif(_workflowcontext.UserId, entity.GetAttributeValue<EntityReference>("tss_approver").Id, Guid.Empty, _organizationservice, emailSubject, emailContent);
                    }
                }
                else
                {
                    throw new InvalidWorkflowException("No Approval Matrix found. Please contact your IT administrator.");
                }
                #endregion
            }
        }

        public void GenerateMarketSizeResultNational(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {
            int totaluio = 0;
            int totalnonuio = 0;
            int totalgroupuiocommodity = 0;
            int totalalluio = 0;

            decimal totalmscommodity = 0m;
            decimal totalmsstandardpart = 0m;
            decimal totalamountms = 0m;

            DateTime startPeriodMS = new DateTime();
            DateTime endPeriodMS = new DateTime();
            DateTime startDtMS = new DateTime();
            DateTime endDtMS = new DateTime();

            QueryExpression qMS = new QueryExpression(_DL_tss_mastermarketsize.EntityName);
            qMS.ColumnSet = new ColumnSet(true);
            List<Entity> ms = _retrievehelper.RetrieveMultiple(organizationService, qMS); // _DL_tss_mastermarketsize.Select(organizationService, qMS);

            object[] msIds = ms.Select(x => (object)x.Id).ToArray();

            if (ms.Count > 0)
            {
                QueryExpression qMSLines = new QueryExpression(_DL_tss_mastermarketsizelines.EntityName);
                qMSLines.LinkEntities.Add(new LinkEntity(_DL_tss_mastermarketsizelines.EntityName, _DL_tss_mastermarketsizesublines.EntityName, "tss_mastermarketsizelinesid", "tss_mastermslinesref", JoinOperator.Inner));
                qMSLines.LinkEntities[0].Columns = new ColumnSet(true);
                qMSLines.LinkEntities[0].EntityAlias = "tss_mastermarketsizesublines";
                qMSLines.Criteria.AddCondition("tss_mastermarketsizeref", ConditionOperator.Equal, msIds);
                qMSLines.ColumnSet = new ColumnSet(true);
                List<Entity> msLines = _retrievehelper.RetrieveMultiple(organizationService, qMS); // _DL_tss_mastermarketsizelines.Select(organizationService, qMS);

                totaluio = ms.Where(x => x.GetAttributeValue<int>("tss_unittype") == UNITTYPE_UIO).Count();
                totalnonuio = ms.Where(x => x.GetAttributeValue<int>("tss_unittype") == UNITTYPE_NONUIO).Count();
                totalgroupuiocommodity = ms.Where(x => x.GetAttributeValue<int>("tss_unittype") == UNITTYPE_COMMODITY).Count();
                totalalluio = totaluio + totalnonuio + totalgroupuiocommodity;

                if (msLines.Count > 0)
                {
                    Guid[] idsUIO = ms.Where(x => x.GetAttributeValue<int>("tss_unittype") == UNITTYPE_UIO || x.GetAttributeValue<int>("tss_unittype") == UNITTYPE_NONUIO).Select(x => x.Id).ToArray();
                    Guid[] idsComm = ms.Where(x => x.GetAttributeValue<int>("tss_unittype") == UNITTYPE_COMMODITY).Select(x => x.Id).ToArray();

                    totalmscommodity = msLines
                                            .Where(x => idsComm.Contains(x.GetAttributeValue<EntityReference>("tss_mastermarketsizeref").Id))
                                            .Sum(x => x.GetAttributeValue<Money>("tss_qty").Value * x.GetAttributeValue<Money>("tss_price").Value);
                    totalmsstandardpart = msLines
                                            .Where(x => idsUIO.Contains(x.GetAttributeValue<EntityReference>("tss_mastermarketsizeref").Id))
                                            .Sum(x => x.GetAttributeValue<Money>("tss_qty").Value * x.GetAttributeValue<Money>("tss_price").Value);
                    totalamountms = totalmscommodity + totalmsstandardpart;
                }

                QueryExpression qSetup = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
                qSetup.ColumnSet = new ColumnSet(true);
                EntityCollection setups = _DL_tss_sparepartsetup.Select(organizationService, qSetup);

                if (setups.Entities.Count > 0)
                {
                    //TBA
                    startPeriodMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_msperiodstart");
                    endPeriodMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_msperiodend");
                }

                QueryExpression qKA = new QueryExpression(_DL_tss_keyaccount.EntityName);
                qKA.ColumnSet.AddColumn("tss_version");
                EntityCollection ka = _DL_tss_keyaccount.Select(organizationService, qKA);

                if (ka.Entities.Count > 0 && setups.Entities.Count > 0)
                {
                    if (ka.Entities.Where(x => x.GetAttributeValue<int>("tss_version") == REVISION).Count() == 0)
                    {
                        startDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_activeperiodstart");
                        endDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_activeperiodsend");
                    }
                    else
                    {
                        startDtMS = new DateTime(setups.Entities[0].GetAttributeValue<DateTime>("tss_activeperiodstart").Year, setups.Entities[0].GetAttributeValue<DateTime>("tss_activeperiodstart").Month, 1).AddMonths(setups.Entities[0].GetAttributeValue<int>("tss_evaluationms"));
                        endDtMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_activeperiodsend");
                    }
                }

                _DL_tss_marketsizeresultnational = new DL_tss_marketsizeresultnational();
                _DL_tss_marketsizeresultnational.tss_totaluio = totaluio;
                _DL_tss_marketsizeresultnational.tss_totalnonuio = totalnonuio;
                _DL_tss_marketsizeresultnational.tss_totalgroupuiocommodity = totalgroupuiocommodity;
                _DL_tss_marketsizeresultnational.tss_totalalluio = totalalluio;
                _DL_tss_marketsizeresultnational.tss_totalmscommodity = totalmscommodity;
                _DL_tss_marketsizeresultnational.tss_totalmsstandardpart = totalmsstandardpart;
                _DL_tss_marketsizeresultnational.tss_totalamountms = totalamountms;
                _DL_tss_marketsizeresultnational.tss_msperiodstart = startPeriodMS;
                _DL_tss_marketsizeresultnational.tss_msperiodend = endPeriodMS;
                _DL_tss_marketsizeresultnational.tss_activeperiodstart = startDtMS;
                _DL_tss_marketsizeresultnational.tss_activeperiodend = endDtMS;
                _DL_tss_marketsizeresultnational.tss_status = STATUS_DRAFT;
                _DL_tss_marketsizeresultnational.tss_statusreason = STATUS_OPEN;
                _DL_tss_marketsizeresultnational.Insert(organizationService);
            }
        }

        public void GenerateSalesTargetPSS(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context, Entity entityMsPss, Entity entityKA)
        {
            try
            {
                List<SalesTarget> salesTargetMonth = new List<SalesTarget>();
                string marketsizetId = string.Empty;
                FilterExpression fExpresion = new FilterExpression(LogicalOperator.And);
                fExpresion.AddCondition("tss_activeperiodstart", ConditionOperator.LessEqual, DateTime.Now);
                fExpresion.AddCondition("tss_activeperiodsend", ConditionOperator.GreaterEqual, DateTime.Now);
                fExpresion.AddCondition("tss_pss", ConditionOperator.Equal, entityMsPss.GetAttributeValue<EntityReference>("tss_pss").Id);
                fExpresion.AddCondition("tss_status", ConditionOperator.Equal, MS_STATUS_COMPLETE);
                fExpresion.AddCondition("tss_customer", ConditionOperator.Equal, entityMsPss.GetAttributeValue<EntityReference>("tss_customer").Id);
                QueryExpression qExpresion = new QueryExpression(_DL_tss_mastermarketsize.EntityName);
                qExpresion.Criteria.AddFilter(fExpresion);
                qExpresion.ColumnSet = new ColumnSet(true);
                List<Entity> mMarketZise = _retrievehelper.RetrieveMultiple(organizationService, qExpresion); // _DL_tss_mastermarketsize.Select(organizationService, qExpresion);


                targetSalesMarketSizeToInsert = new decimal[12];
                List<decimal[]> targetSales = new List<decimal[]>();
                foreach (Entity entityMS in mMarketZise)
                {
                    #region Set value Sales Target

                    marketsizetId = entityMS.Id.ToString();


                    if (entityMS.Attributes.Contains("tss_avghmmethod1") && entityMS.Attributes["tss_avghmmethod1"] != null)
                        Calculate = new Calculate01(organizationService, entityMS);
                    else if (entityMS.Attributes.Contains("tss_avghmmethod2") && entityMS.Attributes["tss_avghmmethod2"] != null)
                        Calculate = new Calculate02(organizationService, entityMS);
                    else if (entityMS.Attributes.Contains("tss_avghmmethod3") && entityMS.Attributes["tss_avghmmethod3"] != null)
                        Calculate = new Calculate03(organizationService, entityMS);
                    else if (entityMS.Attributes.Contains("tss_periodpmmethod4") && entityMS.Attributes["tss_periodpmmethod4"] != null)
                        Calculate = new Calculate04(organizationService, entityMS);
                    else if (entityMS.Attributes.Contains("tss_periodpmmethod5") && entityMS.Attributes["tss_periodpmmethod5"] != null)
                        Calculate = new Calculate05(organizationService, entityMS);
                    else if (entityMS.Attributes.Contains("tss_unittype") && entityMS.GetAttributeValue<OptionSetValue>("tss_unittype").Value == GROUP_UIO_COMMODITY)
                        Calculate = new CalculateCommodity(organizationService, entityMS);

                    Calculate.Calculate();
                    targetSales.Add(Calculate.TargetSales);


                    #endregion

                    #region Set Value Active Start Period
                    if (entityKA.GetAttributeValue<OptionSetValue>("tss_version").Value == KEY_ACCOUNT_VERSION_REVISE/*status revise*/)
                    {
                        qExpresion = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
                        qExpresion.Criteria.AddCondition("tss_name", ConditionOperator.Equal, "TSS");
                        qExpresion.ColumnSet = new ColumnSet(true);
                        EntityCollection setups = _DL_tss_sparepartsetup.Select(organizationService, qExpresion);

                        if (setups.Entities.Count > 0)
                        {
                            activeStartDate = new DateTime();
                            activeStartDate = entityMsPss.GetAttributeValue<DateTime>("tss_msperiodstart");
                            activeStartDate = new DateTime(activeStartDate.Year, activeStartDate.Month, 1).AddMonths(setups.Entities[0].GetAttributeValue<int>("tss_evaluationms"));
                            activeEndDate = new DateTime();
                            activeEndDate = entityMsPss.GetAttributeValue<DateTime>("tss_msperiodend");
                            activeEndDate = new DateTime(activeEndDate.Year, activeEndDate.Month, 1).AddMonths(setups.Entities[0].GetAttributeValue<int>("tss_evaluationms"));
                        }
                        else
                            activeStartDate = entityMsPss.GetAttributeValue<DateTime>("tss_msperiodstart");
                        activeEndDate = entityMsPss.GetAttributeValue<DateTime>("tss_msperiodend");


                    }
                    else
                    {
                        activeStartDate = entityMsPss.GetAttributeValue<DateTime>("tss_msperiodstart");
                        activeEndDate = entityMsPss.GetAttributeValue<DateTime>("tss_msperiodend");
                    }
                    #endregion

                    #region Set Value Market Size Period
                    msPeriodStart = new DateTime();
                    msPeriodStart = entityMsPss.GetAttributeValue<DateTime>("tss_msperiodstart");
                    msPeriodEnd = new DateTime();
                    msPeriodEnd = entityMsPss.GetAttributeValue<DateTime>("tss_msperiodend");
                    #endregion

                    #region Set Value Total Yearly  
                    //targetMSTotalYearly = 0;
                    //foreach (var o in targetSalesMarketSize)
                    //{
                    //    targetMSTotalYearly += o;
                    //}
                    #endregion
                }

                foreach (var o in targetSales)
                {
                    for (int i = 0; i < o.Length; i++)
                    {
                        targetSalesMarketSizeToInsert[i] += o[i];
                    }
                }

                targetMSTotalYearly = 0;
                foreach (var o in targetSalesMarketSizeToInsert)
                {
                    targetMSTotalYearly += o;
                }

                FilterExpression fstpss = new FilterExpression(LogicalOperator.And);
                fstpss.AddCondition("tss_marketsizeid", ConditionOperator.Equal, entityMsPss.Id);
                //fstpss.AddCondition("tss_pss", ConditionOperator.Equal, entityMsPss.GetAttributeValue<EntityReference>("tss_pss").Id);

                QueryExpression qstpss = new QueryExpression(_DL_tss_salestargetpss.EntityName);
                qstpss.ColumnSet = new ColumnSet(true);
                qstpss.Criteria.AddFilter(fstpss);

                EntityCollection stpssIsExist = _DL_tss_salestargetpss.Select(organizationService, qstpss);
                if (stpssIsExist.Entities.Count == 0)
                {
                    Entity entityToInsert = new Entity("tss_salestargetpss");
                    entityToInsert["tss_msperiodstart"] = msPeriodStart;
                    entityToInsert["tss_msenddate"] = msPeriodEnd;
                    entityToInsert["tss_activeenddate"] = activeEndDate;
                    entityToInsert["tss_activestartdate"] = activeStartDate;
                    entityToInsert["tss_marketsizeid"] = new EntityReference("tss_marketsizeresultpss", entityMsPss.Id);
                    entityToInsert["tss_pss"] = new EntityReference("systemuser", entityMsPss.GetAttributeValue<EntityReference>("tss_pss").Id);
                    entityToInsert["tss_branch"] = new EntityReference("businessunit", entityMsPss.GetAttributeValue<EntityReference>("tss_branch").Id);
                    entityToInsert["tss_january"] = new Money(targetSalesMarketSizeToInsert[0]);
                    entityToInsert["tss_february"] = new Money(targetSalesMarketSizeToInsert[1]);
                    entityToInsert["tss_march"] = new Money(targetSalesMarketSizeToInsert[2]);
                    entityToInsert["tss_april"] = new Money(targetSalesMarketSizeToInsert[3]);
                    entityToInsert["tss_may"] = new Money(targetSalesMarketSizeToInsert[4]);
                    entityToInsert["tss_june"] = new Money(targetSalesMarketSizeToInsert[5]);
                    entityToInsert["tss_july"] = new Money(targetSalesMarketSizeToInsert[6]);
                    entityToInsert["tss_august"] = new Money(targetSalesMarketSizeToInsert[7]);
                    entityToInsert["tss_september"] = new Money(targetSalesMarketSizeToInsert[8]);
                    entityToInsert["tss_october"] = new Money(targetSalesMarketSizeToInsert[9]);
                    entityToInsert["tss_november"] = new Money(targetSalesMarketSizeToInsert[10]);
                    entityToInsert["tss_december"] = new Money(targetSalesMarketSizeToInsert[11]);
                    entityToInsert["tss_totalyearly"] = new Money(targetMSTotalYearly);
                    entityToInsert["tss_totalallsalesyearly"] = new Money(0);
                    entityToInsert["tss_totalpctgmarketsizeyearly"] = Convert.ToDecimal(0);
                    entityToInsert["tss_totalamountmarketsizeyearly"] = new Money(0);
                    entityToInsert["tss_status"] = new OptionSetValue(TARGET_SALES_PSS_STATUS_DRAFT);
                    entityToInsert["tss_statusreason"] = new OptionSetValue(TARGET_SALES_PSS_STATUS_REASON_OPEN);
                    organizationService.Create(entityToInsert);
                }
                else
                {
                    Entity _entitytoupdate = stpssIsExist.Entities[0];
                    
                    _entitytoupdate["tss_january"] = new Money(_entitytoupdate.GetAttributeValue<Money>("tss_january").Value + targetSalesMarketSizeToInsert[0]);
                    _entitytoupdate["tss_february"] = new Money(_entitytoupdate.GetAttributeValue<Money>("tss_february").Value + targetSalesMarketSizeToInsert[1]);
                    _entitytoupdate["tss_march"] = new Money(_entitytoupdate.GetAttributeValue<Money>("tss_march").Value + targetSalesMarketSizeToInsert[2]);
                    _entitytoupdate["tss_april"] = new Money(_entitytoupdate.GetAttributeValue<Money>("tss_april").Value + targetSalesMarketSizeToInsert[3]);
                    _entitytoupdate["tss_may"] = new Money(_entitytoupdate.GetAttributeValue<Money>("tss_may").Value + targetSalesMarketSizeToInsert[4]);
                    _entitytoupdate["tss_june"] = new Money(_entitytoupdate.GetAttributeValue<Money>("tss_june").Value + targetSalesMarketSizeToInsert[5]);
                    _entitytoupdate["tss_july"] = new Money(_entitytoupdate.GetAttributeValue<Money>("tss_july").Value + targetSalesMarketSizeToInsert[6]);
                    _entitytoupdate["tss_august"] = new Money(_entitytoupdate.GetAttributeValue<Money>("tss_august").Value + targetSalesMarketSizeToInsert[7]);
                    _entitytoupdate["tss_september"] = new Money(_entitytoupdate.GetAttributeValue<Money>("tss_september").Value + targetSalesMarketSizeToInsert[8]);
                    _entitytoupdate["tss_october"] = new Money(_entitytoupdate.GetAttributeValue<Money>("tss_october").Value + targetSalesMarketSizeToInsert[9]);
                    _entitytoupdate["tss_november"] = new Money(_entitytoupdate.GetAttributeValue<Money>("tss_november").Value + targetSalesMarketSizeToInsert[10]);
                    _entitytoupdate["tss_december"] = new Money(_entitytoupdate.GetAttributeValue<Money>("tss_december").Value + targetSalesMarketSizeToInsert[11]);
                    _entitytoupdate["tss_totalyearly"] = new Money(_entitytoupdate.GetAttributeValue<Money>("tss_totalyearly").Value + targetMSTotalYearly);

                    organizationService.Update(_entitytoupdate);

                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public void ApproveMarketSizeResultNational_OnClick(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {
            try
            {
                DateTime startPeriodMS = DateTime.Now.Date;
                DateTime endPeriodMS = DateTime.Now.Date;
                DateTime startDtMS = DateTime.Now;
                DateTime endDtMS = DateTime.Now;
                FilterExpression filter;
                QueryExpression query;
                Guid marketSizeResultNationalId = context.PrimaryEntityId;
                #region Update status market size result National to  Active (865920002)(tss_status)  & Completed (865920003)(tss_statusreason)
                query = new QueryExpression(_DL_tss_marketsizeresultnational.EntityName);
                query.ColumnSet = new ColumnSet(true);
                Entity entityMarketSizeNational = _DL_tss_marketsizeresultnational.Select(organizationService, marketSizeResultNationalId);
                entityMarketSizeNational["tss_status"] = new OptionSetValue(NATIONAL_STATUS_ACTIVE);
                entityMarketSizeNational["tss_statusreason"] = new OptionSetValue(NATIONAL_STATUS_REASON_COMPLETED);
                entityMarketSizeNational["tss_approvedate"] = DateTime.Now.ToLocalTime();
                organizationService.Update(entityMarketSizeNational);
                #endregion

                #region Retrieve market size result mapping
                filter = new FilterExpression(LogicalOperator.And);
                filter.AddCondition("tss_marketsizeresultnational", ConditionOperator.Equal, marketSizeResultNationalId);
                
                query = new QueryExpression(_DL_tss_marketsizeresultmapping.EntityName);
                query.Criteria.AddFilter(filter);
                query.ColumnSet = new ColumnSet(true);
                
                LinkEntity lintToMsPss = new LinkEntity
                {
                    LinkFromEntityName = _DL_tss_marketsizeresultmapping.EntityName,
                    LinkToEntityName = _DL_tss_marketsizeresultpss.EntityName,
                    LinkFromAttributeName = "tss_marketsizeresultpss",
                    LinkToAttributeName = "tss_marketsizeresultpssid",
                    Columns = new ColumnSet(new string[] { "tss_status"}),
                    EntityAlias = "msresultpss",
                    JoinOperator = JoinOperator.Inner

                };
                query.LinkEntities.Add(lintToMsPss);
                query.LinkEntities[0].LinkCriteria.AddCondition("tss_status", ConditionOperator.NotEqual, 865920007);/*Status = Disabled*/
                EntityCollection ENC_tss_marketsizeresultmapping = _DL_tss_marketsizeresultmapping.Select(organizationService, query);
                List<Entity> entityMarketSizeResultPSS = new List<Entity>();

                foreach (Entity en_mapping in ENC_tss_marketsizeresultmapping.Entities)
                {
                    /*Retrieve and update for each entity*/
                    #region Update status market size result PSS to  Active(865920005)(tss_status) & Active (865920001)(tss_statusreason)
                    Entity entity_pss = _DL_tss_marketsizeresultpss.Select(organizationService, en_mapping.GetAttributeValue<EntityReference>(_DL_tss_marketsizeresultpss.EntityName).Id);
                    entity_pss["tss_status"] = new OptionSetValue(PSS_STATUS_ACTIVE);
                    entity_pss["tss_statusreason"] = new OptionSetValue(PSS_STATUS_REASON_ACTIVE);

                    organizationService.Update(entity_pss);
                    #endregion

                    #region Update status Key Account to  Active (865920003) & Active (865920001)
                    Entity entity_keyAccount = _DL_tss_keyaccount.Select(organizationService, en_mapping.GetAttributeValue<EntityReference>(_DL_tss_keyaccount.EntityName).Id);
                    entity_keyAccount["tss_status"] = new OptionSetValue(KEY_ACCOUNT_STATUS_ACTIVE);
                    entity_keyAccount["tss_reason"] = new OptionSetValue(KEY_ACCOUNT_STATUS_REASON_ACTIVE);
                    organizationService.Update(entity_keyAccount);
                    #endregion

                    #region Update status market size result Branch to  Active (865920004)(tss_status)  & Completed (865920003)(tss_statusreason)
                    Entity entity_branch = _DL_tss_marketsizeresultbranch.Select(organizationService, en_mapping.GetAttributeValue<EntityReference>(_DL_tss_marketsizeresultbranch.EntityName).Id);
                    entity_branch["tss_status"] = new OptionSetValue(BRANCH_STATUS_ACTIVE);
                    entity_branch["tss_statusreason"] = new OptionSetValue(BRANCH_STATUS_REASON_COMPLETED);
                    organizationService.Update(entity_branch);
                    #endregion
                    GenerateSalesTargetPSS(organizationService, tracingService, context, entity_pss, entity_keyAccount);
                }
                #endregion

                #region Generate Email to PSS and All previous approver base on Market Size Matrix Approver to inform them that the market size has been apprved
                QueryExpression qappMat = new QueryExpression(_DL_tss_matrixapprovalmarketsize.EntityName);
                qappMat.Orders.Add(new Microsoft.Xrm.Sdk.Query.OrderExpression("tss_priorityno", Microsoft.Xrm.Sdk.Query.OrderType.Ascending));
                qappMat.ColumnSet = new ColumnSet(true);

                EntityCollection entityApprovalMatrixMarketSize = _DL_tss_matrixapprovalmarketsize.Select(organizationService, qappMat);
                if (entityApprovalMatrixMarketSize.Entities.Count > 0)
                {
                    foreach (var entity in entityApprovalMatrixMarketSize.Entities)
                    {
                        TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent objEmail = new TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent();
                        string emailSubject = entityMarketSizeNational.GetAttributeValue<string>("tss_name") + " has been approved";
                        string emailContent = "Market Size has been approved";
                        objEmail.SendEmailNotif(context.UserId, entity.GetAttributeValue<EntityReference>("tss_approver").Id, Guid.Empty, organizationService, emailSubject, emailContent);
                    }
                }
                else
                {
                    throw new InvalidWorkflowException("No Approval Matrix found. Please contact your IT administrator.");
                }
                #endregion
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private void SetValueToEntity(IOrganizationService organizationService, Entity entityPSS, Entity entityKA, Entity entityMS, ref string marketsizetId)
        {


        }

        public void ReviseMarketSizeResultNational_OnClick(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {

            #region get and update status current Market Size Result National
            Entity En_CurrMarketSizeResultNational = _DL_tss_marketsizeresultnational.Select(organizationService, context.PrimaryEntityId);
            En_CurrMarketSizeResultNational["tss_status"] = new OptionSetValue(865920004);
            En_CurrMarketSizeResultNational["tss_statusreason"] = new OptionSetValue(865920004);

            organizationService.Update(En_CurrMarketSizeResultNational);
            #endregion

            #region Get Mapping Market Size Result
            FilterExpression fMMSR = new FilterExpression(LogicalOperator.And);
            fMMSR.AddCondition("tss_marketsizeresultnational", ConditionOperator.Equal, context.PrimaryEntityId);

            QueryExpression qMMSR = new QueryExpression(_DL_tss_marketsizeresultmapping.EntityName);
            qMMSR.Criteria.AddFilter(fMMSR);
            qMMSR.ColumnSet = new ColumnSet(true);

            EntityCollection ENC_MarketSizeResultMapping = _DL_tss_marketsizeresultmapping.Select(organizationService, qMMSR);

            //GET BRANCH & PSS COLLECTION
            var listMarketSizeResultBranchID = ENC_MarketSizeResultMapping.Entities.Select(x => (EntityReference)x["tss_marketsizeresultbranch"]).Distinct().ToArray();

            FilterExpression fMMB = new FilterExpression(LogicalOperator.Or);

            foreach (var marketSizeResultBranchID in listMarketSizeResultBranchID)
            {
                fMMB.AddCondition("tss_marketsizeresultbranch", ConditionOperator.Equal, marketSizeResultBranchID.Id);
            }

            QueryExpression qMMB = new QueryExpression(_DL_tss_marketsizeresultmapping.EntityName);
            qMMB.Criteria.AddFilter(fMMB);
            qMMB.ColumnSet = new ColumnSet(true);

            EntityCollection ENC_MarketSizeResultMappingBranch = _DL_tss_marketsizeresultmapping.Select(organizationService, qMMB);

            var listMarketSizeResultPSSID = ENC_MarketSizeResultMappingBranch.Entities.Select(x => (EntityReference)x["tss_marketsizeresultpss"]).Distinct().ToArray();

            //UPDATE BRANCH
            foreach (var item in listMarketSizeResultBranchID)
            {
                Entity marketSizeBranch = _DL_tss_marketsizeresultbranch.Select(organizationService, item.Id);

                marketSizeBranch["tss_status"] = new OptionSetValue(865920006); //DISABLED
                marketSizeBranch["tss_statusreason"] = new OptionSetValue(865920004); //REMOVED

                organizationService.Update(marketSizeBranch);
            }

            //UPDATE PSS
            foreach (var item in listMarketSizeResultPSSID)
            {
                Entity marketSizePSS = _DL_tss_marketsizeresultpss.Select(organizationService, item.Id);
                pssToRevise.Entities.Add(marketSizePSS);
                marketSizePSS["tss_status"] = new OptionSetValue(865920007);
                marketSizePSS["tss_statusreason"] = new OptionSetValue(865920002);

                organizationService.Update(marketSizePSS);

                Entity EN_SystemUser = _DL_user.Select(organizationService, marketSizePSS.GetAttributeValue<EntityReference>("tss_pss").Id);

                EN_SystemUser["tss_marketsizeconfirmed"] = false;

                organizationService.Update(EN_SystemUser);
            }
            FilterExpression f = new FilterExpression(LogicalOperator.And);
            QueryExpression q = new QueryExpression(_DL_tss_marketsizeresultmapping.EntityName);
            //Guid[] keyAccountId = ENC_MarketSizeResultMapping.Entities.Select(x => x.Id).Distinct().ToArray();
            object[] keyAccountId = ENC_MarketSizeResultMapping.Entities.Select(x => (object)x.GetAttributeValue<EntityReference>("tss_keyaccount").Id).ToArray();

            f = new FilterExpression(LogicalOperator.And);
            f.AddCondition("tss_keyaccountid", ConditionOperator.In, keyAccountId);

            q = new QueryExpression(_DL_tss_keyaccount.EntityName);
            q.ColumnSet = new ColumnSet(true);
            q.Criteria.AddFilter(f);

            EntityCollection entityToRevise = _DL_tss_keyaccount.Select(organizationService, q);

            _BL_tss_mastermarketsize.Revise(organizationService, pssToRevise);
            _BL_KeyAccount.ReviseKeyAccount(organizationService, tracingService, context, entityToRevise);

            #endregion

            #region Notif Email to All Previous Approver
            FilterExpression fApp = new FilterExpression(LogicalOperator.And);
            fApp.AddCondition("tss_marketsizeresultnational", ConditionOperator.Equal, context.PrimaryEntityId);

            QueryExpression qApp = new QueryExpression(_DL_tss_approvallistmarketsize.EntityName);
            qApp.Criteria.AddFilter(fApp);
            qApp.ColumnSet = new ColumnSet(true);

            EntityCollection ENC_ApprovalList = _DL_tss_approvallistmarketsize.Select(organizationService, qApp);

            #region get user admin

            QueryExpression queryExpression = new QueryExpression(_DL_user.EntityName);
            queryExpression.ColumnSet.AddColumns("systemuserid", "domainname");
            queryExpression.Criteria.AddCondition("domainname", ConditionOperator.Equal, "TRAKNUS\\admin.crm");

            EntityCollection admins = _DL_user.Select(organizationService, queryExpression);
            Guid crmadmin = admins.Entities[0].Id;

            #endregion

            foreach (Entity approval in ENC_ApprovalList.Entities)
            {
                TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent objEmail = new TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent();
                string emailSubject = En_CurrMarketSizeResultNational.GetAttributeValue<string>("tss_name") + " is revised";
                string emailContent = "Please recheck and review.";
                objEmail.SendEmailNotif(context.UserId, approval.GetAttributeValue<EntityReference>("tss_approver").Id, Guid.Empty, organizationService, emailSubject, emailContent);
            }

            #endregion
        }

        //public void ReviseMarketSizeResultNational_OnClick(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)

        //{

        //    #region get and update status current Market Size Result National
        //    Entity En_CurrMarketSizeResultNational = _DL_tss_marketsizeresultnational.Select(organizationService, context.UserId);
        //    En_CurrMarketSizeResultNational["tss_status"] = new OptionSetValue(865920002);
        //    En_CurrMarketSizeResultNational["tss_statusreason"] = new OptionSetValue(865920002);
        //    organizationService.Update(En_CurrMarketSizeResultNational);
        //    #endregion

        //    #region get Mapping Market Size Result 
        //    FilterExpression fMMSR = new FilterExpression(LogicalOperator.And);
        //    fMMSR.AddCondition("tss_marketsizeresultnational", ConditionOperator.Equal, context.PrimaryEntityId);

        //    QueryExpression qMMSR = new QueryExpression(_DL_tss_marketsizeresultmapping.EntityName);
        //    qMMSR.Criteria.AddFilter(fMMSR);
        //    qMMSR.ColumnSet = new ColumnSet(true);

        //    EntityCollection ENC_MarketSizeResultMapping = _DL_tss_marketsizeresultmapping.Select(organizationService, qMMSR);

        //    #endregion

        //    #region get and update status current Market Size Result Branch
        //    var listMarketSizeResultBranchID = ENC_MarketSizeResultMapping.Entities.Select(x => (object)x["tss_marketsizeresultbranch"]).Distinct().ToArray();

        //    FilterExpression fMMB = new FilterExpression(LogicalOperator.Or);

        //    foreach (string marketSizeResultBranchID in listMarketSizeResultBranchID)
        //    {
        //        fMMB.AddCondition("tss_marketsizeresultbranch", ConditionOperator.Equal, new Guid(marketSizeResultBranchID));

        //    }
        //    QueryExpression qMMB = new QueryExpression(_DL_tss_marketsizeresultmapping.EntityName);
        //    qMMB.Criteria.AddFilter(fMMB);
        //    qMMB.ColumnSet = new ColumnSet(true);

        //    EntityCollection ENC_MarketSizeResultBranch = _DL_tss_marketsizeresultbranch.Select(organizationService, qMMB);

        //    foreach (Entity En_MarketSizeResultBranch in ENC_MarketSizeResultBranch.Entities)
        //    {
        //        En_MarketSizeResultBranch["tss_status"] = new OptionSetValue(865920004);
        //        En_MarketSizeResultBranch["tss_statusreason"] = new OptionSetValue(865920002);
        //        organizationService.Update(En_MarketSizeResultBranch);
        //    }

        //    #endregion

        //    #region get and update current Market Size Result PSS
        //    foreach (Entity maps in ENC_MarketSizeResultMapping.Entities)
        //    {
        //        Entity EN_MarketSizeResultPSS = _DL_tss_marketsizeresultpss.Select(organizationService, maps.GetAttributeValue<EntityReference>("tss_marketsizeresultpss").Id);
        //        EN_MarketSizeResultPSS["tss_status"] = new OptionSetValue(865920007);
        //        EN_MarketSizeResultPSS["tss_statusreason"] = new OptionSetValue(865920002);
        //        organizationService.Update(EN_MarketSizeResultPSS);

        //        Entity EN_SystemUser = _DL_user.Select(organizationService, EN_MarketSizeResultPSS.GetAttributeValue<EntityReference>("tss_pss").Id);
        //        EN_SystemUser["tss_marketsizeconfirmed"] = false;
        //        organizationService.Update(EN_SystemUser);
        //    }

        //    #endregion

        //    #region Notif Email to All Previous Approver
        //    FilterExpression fApp = new FilterExpression(LogicalOperator.And);
        //    fApp.AddCondition("tss_marketsizeresultnational", ConditionOperator.Equal, context.PrimaryEntityId);

        //    QueryExpression qApp = new QueryExpression(_DL_tss_approvallistmarketsize.EntityName);
        //    qApp.Criteria.AddFilter(fApp);
        //    qApp.ColumnSet = new ColumnSet(true);

        //    EntityCollection ENC_ApprovalList = _DL_tss_approvallistmarketsize.Select(organizationService, qApp);

        //    #region get user admin

        //    QueryExpression queryExpression = new QueryExpression(_DL_user.EntityName);
        //    queryExpression.ColumnSet.AddColumns("systemuserid", "domainname");
        //    queryExpression.Criteria.AddCondition("domainname", ConditionOperator.Equal, "TRAKNUS\\admin.crm");

        //    EntityCollection admins = _DL_user.Select(organizationService, queryExpression);
        //    Guid crmadmin = admins.Entities[0].Id;

        //    #endregion

        //    foreach (Entity approval in ENC_ApprovalList.Entities)
        //    {
        //        TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent objEmail = new TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent();
        //        string emailSubject = En_CurrMarketSizeResultNational.GetAttributeValue<string>("tss_name") + " is revised";
        //        string emailContent = "Please recheck and review.";
        //        objEmail.SendEmailNotif(context.UserId, approval.GetAttributeValue<EntityReference>("tss_approver").Id, Guid.Empty, organizationService, emailSubject, emailContent);
        //    }

        //    #endregion

        //}

        private void BuildSalesTarget()
        {

        }
    }

    public class SalesTarget
    {
        public int Month { get; set; }
        public decimal Value { get; set; }
    }
}
