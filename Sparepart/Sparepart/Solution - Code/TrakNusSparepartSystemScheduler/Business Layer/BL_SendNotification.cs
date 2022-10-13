using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;

using TrakNusSparepartSystemScheduler.Helper;
using TrakNusSparepartSystem.DataLayer;

namespace TrakNusSparepartSystemScheduler.Business_Layer
{
    public class BL_SendNotification
    {
        private const int PSS_STATUS_CLOSED = 865920006;
        private const int PSS_STATUS_REASON_COMPLETED = 865920003;
        private const int BRANCH_STATUS_CLOSED = 865920005;
        private const int BRANCH_STATUS_REASON_COMPLETED = 865920003;
        private const int NAT_STATUS_CLOSED = 865920003;
        private const int NAT_STATUS_REASON_COMPLETED = 865920003;
        private const int ST_PSS_STATUS_CLOSED = 865920006;
        private const int ST_PSS_STATUS_REASON_COMPLETED = 865920002;
        private const int ST_BRANCH_STATUS_CLOSED = 865920005;
        private const int ST_BRANCH_STATUS_REASON_COMPLETED = 865920002;
        private const int ST_NAT_STATUS_CLOSED = 865920002;
        private const int ST_NAT_STATUS_REASON_COMPLETED = 865920002;
        private const int POPPROS_STATUS_CLOSE = 865920002;
        private const int POPPROS_STATUS_REASON_COMPLETED = 865920002;

        private const int KEYACCOUNT_STATUS_CLOSE = 865920005;
        private const int KEYACCOUNT_REASON_EXPIRED = 865920002;

        DL_tss_mastermarketsize _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        DL_tss_marketsizeresultpss _DL_tss_marketsizeresultpss = new DL_tss_marketsizeresultpss();
        DL_tss_marketsizeresultbranch _DL_tss_marketsizeresultbranch = new DL_tss_marketsizeresultbranch();
        DL_tss_marketsizeresultnational _DL_tss_marketsizeresultnational = new DL_tss_marketsizeresultnational();
        DL_tss_salestargetpss _DL_tss_salestargetpss = new DL_tss_salestargetpss();
        DL_tss_salestargetbranch _DL_tss_salestargetbranch = new DL_tss_salestargetbranch();
        DL_tss_salestargetnational _DL_tss_salestargetnational = new DL_tss_salestargetnational();
        DL_tss_marketsizeresultmapping _DL_tss_marketsizeresultmapping = new DL_tss_marketsizeresultmapping();
        DL_tss_potentialprospectpart _DL_tss_potentialprospectpart = new DL_tss_potentialprospectpart();
        DL_tss_sparepartsetup _DL_tss_sparepartsetup = new DL_tss_sparepartsetup();
        DL_tss_matrixapprovalmarketsize _DL_tss_matrixapprovalmarketsize = new DL_tss_matrixapprovalmarketsize();
        DL_user _DL_user = new DL_user();
        DL_tss_keyaccount _DL_tss_keyaccount = new DL_tss_keyaccount();
        private const int APPPDH = 865920001;
        public void SendNotifBeforeStardPeriod(SendEmailNotif emailNotif)
        {
            string _connectionString = string.Empty;
            int notifBeforStarPeriod = 0;
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
                FilterExpression fSetup = new FilterExpression(LogicalOperator.And);
                fSetup.AddCondition("tss_name", ConditionOperator.Equal, "TSS");

                QueryExpression qSetup = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
                qSetup.ColumnSet = new ColumnSet(true);
                qSetup.Criteria.AddFilter(fSetup);
                EntityCollection setups = _DL_tss_sparepartsetup.Select(_orgService, qSetup);
                bool isMsPeriodEnd = false;
                FilterExpression fMarketSize = new FilterExpression(LogicalOperator.And);
                if (setups.Entities.Count > 0)
                {
                    if (emailNotif == SendEmailNotif.BeforeStartMSPeriod)
                    {

                        notifBeforStarPeriod = (setups.Entities[0].GetAttributeValue<int>("tss_notifbeforestartms"));
                        fMarketSize.AddCondition("tss_activeperiodstart", ConditionOperator.Equal, DateTime.Now.AddDays(notifBeforStarPeriod).Date);
                    }
                    else
                    {
                        notifBeforStarPeriod = (setups.Entities[0].GetAttributeValue<int>("tss_notifbeforeendms"));
                        fMarketSize.AddCondition("tss_activeperiodsend", ConditionOperator.Equal, DateTime.Now.AddDays(notifBeforStarPeriod).Date);
                        isMsPeriodEnd = true;
                    }
                }
                #region Loop PSS Data

                string[] titles = ConfigurationManager.AppSettings["TITLEPSS"].ToString().Split('|');
                QueryExpression qMarketSize = new QueryExpression(_DL_tss_mastermarketsize.EntityName);
                qMarketSize.Criteria.AddFilter(fMarketSize);
                qMarketSize.ColumnSet = new ColumnSet(true);
                qMarketSize.LinkEntities.Add(
                    new LinkEntity(
                        _DL_tss_mastermarketsize.EntityName,
                        _DL_user.EntityName,
                        "tss_pss",
                        "systemuserid",
                        JoinOperator.Inner
                ));
                qMarketSize.LinkEntities[0].Columns.AddColumns("businessunitid", "systemuserid");
                qMarketSize.LinkEntities[0].EntityAlias = "sysuser";
                qMarketSize.LinkEntities[0].LinkCriteria.AddCondition(new ConditionExpression("title", ConditionOperator.In, titles));
                EntityCollection marketSize = _DL_tss_mastermarketsize.Select(_orgService, qMarketSize);
                var result = (from r in marketSize.Entities.AsEnumerable()
                              group r by new
                              {
                                  pss = (Guid)r.GetAttributeValue<AliasedValue>("sysuser.systemuserid").Value,
                                  mstartperiod = r.GetAttributeValue<DateTime>("tss_msperiodstart").ToLocalTime(),
                                  msendperiod = r.GetAttributeValue<DateTime>("tss_msperiodend").ToLocalTime()
                              } into g
                              select new
                              {

                                  pss = g.Key.pss,
                                  msperiostart = g.Key.mstartperiod,
                                  msperiodend = g.Key.msendperiod
                              }).ToList();


                foreach (var g in result)
                {

                    if (!isMsPeriodEnd)
                    {
                        string emailSubject = "email notification";
                        string emailContent = EmailAgent.GetContentEmail("", "", CRM_URL, marketSize);
                        EntityCollection admins = GetFromAdminCRM(_orgService);
                        Guid crmadmin = admins.Entities[0].Id;

                        EmailAgent.SendEmailNotif(crmadmin, g.pss, Guid.Empty, _orgService, emailSubject, emailContent);
                        Console.WriteLine("Notification sent!");
                    }
                    else
                    {
                        foreach (Entity entity in marketSize.Entities)
                        {
                            FilterExpression fexp;
                            QueryExpression qexp;

                            #region Closing Market Result
                            fexp = new FilterExpression();
                            fexp.AddCondition("tss_pss", ConditionOperator.Equal, entity.GetAttributeValue<EntityReference>("tss_pss").Id);
                            //fPSS.AddCondition("tss_status", ConditionOperator.Equal, APPPDH);

                            qexp = new QueryExpression(_DL_tss_marketsizeresultpss.EntityName);
                            qexp.Criteria.AddFilter(fexp);
                            qexp.ColumnSet = new ColumnSet(true);
                            EntityCollection msPSS = _DL_tss_marketsizeresultpss.Select(_orgService, qexp);
                            List<string> branchIds = new List<string>();
                            foreach (Entity mspss in msPSS.Entities)
                            {
                                branchIds.Add(mspss.GetAttributeValue<EntityReference>("tss_branch").Id.ToString());
                                mspss["tss_status"] = new OptionSetValue(PSS_STATUS_CLOSED);
                                mspss["tss_statusreason"] = new OptionSetValue(PSS_STATUS_REASON_COMPLETED);
                                _orgService.Update(mspss);
                            }


                            fexp = new FilterExpression(LogicalOperator.And);
                            fexp.AddCondition("tss_branch", ConditionOperator.In, branchIds.Distinct().ToArray());
                            // fexp.AddCondition("tss_status", ConditionOperator.NotEqual, BRANCH_STATUS_CLOSED);
                            //fexp.AddCondition("tss_statusreason", ConditionOperator.NotEqual, BRANCH_STATUS_REASON_COMPLETED);

                            qexp = new QueryExpression(_DL_tss_marketsizeresultbranch.EntityName);
                            qexp.Criteria.AddFilter(fexp);
                            qexp.ColumnSet = new ColumnSet(true);
                            EntityCollection entityBranch = _DL_tss_marketsizeresultbranch.Select(_orgService, qexp);
                            foreach (var branch in entityBranch.Entities)
                            {
                                branch["tss_status"] = new OptionSetValue(BRANCH_STATUS_CLOSED);
                                branch["tss_statusreason"] = new OptionSetValue(BRANCH_STATUS_REASON_COMPLETED);
                                _orgService.Update(branch);
                            }

                            fexp = new FilterExpression(LogicalOperator.And);
                            fexp.AddCondition("tss_marketsizeresultbranch", ConditionOperator.In, entityBranch.Entities.Select(x => x.Id.ToString()).Distinct().ToArray());

                            qexp = new QueryExpression(_DL_tss_marketsizeresultmapping.EntityName);
                            qexp.Criteria.AddFilter(fexp);
                            qexp.ColumnSet = new ColumnSet(true);
                            EntityCollection msmapping = _DL_tss_marketsizeresultmapping.Select(_orgService, qexp);

                            if (msmapping.Entities.Count > 0)
                            {
                                EntityReference nationalRef = msmapping.Entities[0].GetAttributeValue<EntityReference>("tss_marketsizeresultnational");
                                qexp = new QueryExpression(_DL_tss_marketsizeresultnational.EntityName);
                                qexp.ColumnSet = new ColumnSet(true);
                                Entity msNational = new Entity(nationalRef.LogicalName, nationalRef.Id);
                                msNational["tss_status"] = new OptionSetValue(NAT_STATUS_CLOSED);
                                msNational["tss_statusreason"] = new OptionSetValue(NAT_STATUS_REASON_COMPLETED);
                                _orgService.Update(msNational);
                            }

                            #endregion

                            #region Sales Target
                            fexp = new FilterExpression();
                            fexp.AddCondition("tss_pss", ConditionOperator.Equal, entity.GetAttributeValue<EntityReference>("tss_pss").Id);
                            //fPSS.AddCondition("tss_status", ConditionOperator.Equal, APPPDH);

                            qexp = new QueryExpression(_DL_tss_salestargetpss.EntityName);
                            qexp.Criteria.AddFilter(fexp);
                            qexp.ColumnSet = new ColumnSet(true);
                            EntityCollection stPSS = _DL_tss_salestargetpss.Select(_orgService, qexp);
                            branchIds = new List<string>();
                            foreach (Entity stpss in stPSS.Entities)
                            {
                                branchIds.Add(stpss.GetAttributeValue<EntityReference>("tss_branch").Id.ToString());
                                stpss["tss_status"] = new OptionSetValue(ST_PSS_STATUS_CLOSED);
                                stpss["tss_statusreason"] = new OptionSetValue(ST_PSS_STATUS_REASON_COMPLETED);
                                _orgService.Update(stpss);
                            }

                            fexp = new FilterExpression(LogicalOperator.And);
                            fexp.AddCondition("tss_branch", ConditionOperator.In, branchIds.Distinct().ToArray());
                            //fexp.AddCondition("tss_status", ConditionOperator.Equal, ST_BRANCH_STATUS_CLOSED);
                            //fexp.AddCondition("tss_statusreason", ConditionOperator.Equal, ST_BRANCH_STATUS_REASON_COMPLETED);

                            qexp = new QueryExpression(_DL_tss_salestargetbranch.EntityName);
                            qexp.Criteria.AddFilter(fexp);
                            qexp.ColumnSet = new ColumnSet(true);
                            EntityCollection stBranch = _DL_tss_salestargetbranch.Select(_orgService, qexp);
                            foreach (var stbranch in stBranch.Entities)
                            {
                                stbranch["tss_status"] = new OptionSetValue(ST_BRANCH_STATUS_CLOSED);
                                stbranch["tss_statusreason"] = new OptionSetValue(ST_BRANCH_STATUS_REASON_COMPLETED);
                                _orgService.Update(stbranch);
                            }

                            fexp = new FilterExpression(LogicalOperator.And);
                            fexp.AddCondition("tss_salestargetbranch", ConditionOperator.In, stBranch.Entities.Select(x => x.Id.ToString()).Distinct().ToArray());

                            qexp = new QueryExpression(_DL_tss_marketsizeresultmapping.EntityName);
                            qexp.Criteria.AddFilter(fexp);
                            qexp.ColumnSet = new ColumnSet(true);
                            EntityCollection stmapping = _DL_tss_marketsizeresultmapping.Select(_orgService, qexp);

                            if (stmapping.Entities.Count > 0)
                            {
                                qexp = new QueryExpression(_DL_tss_salestargetnational.EntityName);
                                qexp.ColumnSet = new ColumnSet(true);
                                EntityReference stNatRef = stmapping.Entities[0].GetAttributeValue<EntityReference>("tss_salestargetnational");
                                Entity stNational = new Entity(stNatRef.LogicalName, stNatRef.Id);
                                stNational["tss_status"] = new OptionSetValue(ST_NAT_STATUS_CLOSED);
                                stNational["tss_statusreason"] = new OptionSetValue(ST_NAT_STATUS_REASON_COMPLETED);
                                _orgService.Update(stNational);
                            }
                            #endregion

                            #region Potential Prospect Part
                            fexp = new FilterExpression(LogicalOperator.And);
                            fexp.AddCondition("tss_pss", ConditionOperator.Equal, entity.GetAttributeValue<EntityReference>("tss_pss").Id);
                            // fexp.AddCondition("tss_status", ConditionOperator.NotEqual, POPPROS_STATUS_CLOSE);
                            //  fexp.AddCondition("tss_statusreason", ConditionOperator.NotEqual, POPPROS_STATUS_REASON_COMPLETED);

                            qexp = new QueryExpression(_DL_tss_potentialprospectpart.EntityName);
                            qexp.Criteria.AddFilter(fexp);
                            qexp.ColumnSet = new ColumnSet(true);
                            EntityCollection potentialprospect = _DL_tss_potentialprospectpart.Select(_orgService, qexp);
                            foreach (var o in potentialprospect.Entities)
                            {
                                Entity oToUpdate = new Entity(o.LogicalName, o.Id);
                                oToUpdate["tss_status"] = new OptionSetValue(POPPROS_STATUS_CLOSE);
                                oToUpdate["tss_statusreason"] = new OptionSetValue(POPPROS_STATUS_REASON_COMPLETED);
                                _orgService.Update(oToUpdate);
                            }
                            #endregion

                        }
                        #region Update Key Account
                        FilterExpression fexpKeyAccount = new FilterExpression(LogicalOperator.And);
                        fexpKeyAccount.AddCondition("tss_pss", ConditionOperator.Equal, g.pss);
                        // fexp.AddCondition("tss_status", ConditionOperator.NotEqual, POPPROS_STATUS_CLOSE);
                        //  fexp.AddCondition("tss_statusreason", ConditionOperator.NotEqual, POPPROS_STATUS_REASON_COMPLETED);

                        QueryExpression qexpKeyAccount = new QueryExpression(_DL_tss_keyaccount.EntityName);
                        qexpKeyAccount.Criteria.AddFilter(fexpKeyAccount);
                        qexpKeyAccount.ColumnSet = new ColumnSet(true);
                        EntityCollection keyAccount = _DL_tss_keyaccount.Select(_orgService, qexpKeyAccount);
                        foreach (var o in keyAccount.Entities)
                        {
                            Entity keyAccountToUpdate = new Entity(o.LogicalName, o.Id);
                            keyAccountToUpdate["tss_status"] = new OptionSetValue(KEYACCOUNT_STATUS_CLOSE);
                            //keyAccountToUpdate["tss_statusreason"] = new OptionSetValue(KEYACCOUNT_STATUS_REASON_COMPLETED);
                            keyAccountToUpdate["tss_reason"] = new OptionSetValue(KEYACCOUNT_REASON_EXPIRED);
                            _orgService.Update(keyAccountToUpdate);
                        }
                        #endregion
                        break;
                    }
                }

                #endregion
            }
            catch (Exception)
            {

                throw;
            }
            Console.WriteLine("Successfully Execute");

        }
        public void SendNotifEnableButtonRevise()
        {
            string _connectionString = string.Empty;
            int notifBeforStarPeriod = 0;
            int notifBeforEndPeriod = 0;
            int evaluationPeriod = 0;
            int evaluationDuration = 0;
            DateTime evaluationStart;
            DateTime evaluationEnd;
            if (ConnString.IsValidConnectionString(ConfigurationManager.ConnectionStrings["CRMCS"].ConnectionString))
                _connectionString = ConfigurationManager.ConnectionStrings["CRMCS"].ConnectionString;
            else
                _connectionString = "Url=http://10.0.10.43/traktornusantara; Domain=traknus; Username=admin.crm; Password=pass@word2;";

            string CRM_URL = _connectionString.Split(';')[0].Remove(0, 4);

            Console.WriteLine("Send email notif key account");
            Console.WriteLine("====================================");
            Console.WriteLine("Getting crmserviceclient..");
            CrmServiceClient conn = new CrmServiceClient(_connectionString); try
            {
                IOrganizationService _orgService = (IOrganizationService)conn.OrganizationServiceProxy;
                FilterExpression fSetup = new FilterExpression(LogicalOperator.And);
                fSetup.AddCondition("tss_name", ConditionOperator.Equal, "TSS");

                QueryExpression qSetup = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
                qSetup.ColumnSet = new ColumnSet(true);
                qSetup.Criteria.AddFilter(fSetup);
                EntityCollection setups = _DL_tss_sparepartsetup.Select(_orgService, qSetup);
                if (setups.Entities.Count > 0)
                {
                    notifBeforStarPeriod = setups.Entities[0].GetAttributeValue<int>("tss_notifbeforestartms");
                    notifBeforEndPeriod = setups.Entities[0].GetAttributeValue<int>("tss_notifbeforeendms");
                    evaluationDuration = setups.Entities[0].GetAttributeValue<int>("tss_evaluationdurationms");
                    evaluationPeriod = setups.Entities[0].GetAttributeValue<int>("tss_evaluationms");
                }

                //FilterExpression fka = new FilterExpression(LogicalOperator.And);
                //fka.AddCondition("tss_msperiodstart", ConditionOperator.Equal, DateTime.Now.Date);
                //fka.AddCondition("tss_activestartdate", ConditionOperator.GreaterEqual, DateTime.Now.Date);
                //fPSS.AddCondition("tss_status", ConditionOperator.Equal, APPPDH);
                string[] titles = ConfigurationManager.AppSettings["TITLEPSS"].ToString().Split('|');
                QueryExpression qKeyAccount = new QueryExpression(_DL_tss_keyaccount.EntityName);
                //qKeyAccount.Criteria.AddFilter(fka);
                qKeyAccount.ColumnSet = new ColumnSet(true);
                qKeyAccount.LinkEntities.Add(
                    new LinkEntity(
                        _DL_tss_keyaccount.EntityName,
                        _DL_user.EntityName,
                        "tss_pss",
                        "systemuserid",
                        JoinOperator.Inner
                ));
                qKeyAccount.LinkEntities[0].Columns.AddColumns("businessunitid", "systemuserid");
                qKeyAccount.LinkEntities[0].EntityAlias = "sysuser";
                qKeyAccount.LinkEntities[0].LinkCriteria.AddCondition(new ConditionExpression("title", ConditionOperator.In, titles));
                EntityCollection keyAccounts = _DL_tss_keyaccount.Select(_orgService, qKeyAccount);
                foreach (Entity keyaccount in keyAccounts.Entities)
                {
                    evaluationStart = keyaccount.GetAttributeValue<DateTime>("tss_msperiodstart").ToLocalTime().AddMonths(evaluationPeriod);
                    evaluationEnd = evaluationStart.AddDays(evaluationDuration);
                    Entity keyAccountToUpdate = new Entity(keyaccount.LogicalName, keyaccount.Id);
                    if (evaluationStart.Date <= DateTime.Now.Date && evaluationEnd >= DateTime.Now.Date)
                    {
                        keyAccountToUpdate.Attributes["tss_isevaluation"] = true;
                        _orgService.Update(keyAccountToUpdate);
                    }
                    else
                    {
                        keyAccountToUpdate.Attributes["tss_isevaluation"] = false;
                        _orgService.Update(keyAccountToUpdate);
                    }
                    if (evaluationStart.AddDays((-1 * notifBeforStarPeriod)).Date == DateTime.Now.Date)
                    {
                        string emailSubject = keyaccount.GetAttributeValue<string>("tss_name") + " email notification";
                        string emailContent = "Please review and approve.";
                        EntityCollection admins = GetFromAdminCRM(_orgService);
                        Guid crmadmin = admins.Entities[0].Id;
                        EmailAgent.SendEmailNotif(crmadmin, keyaccount.GetAttributeValue<Guid>("sysuser.systemuserid"), Guid.Empty, _orgService, emailSubject, emailContent);
                        Console.WriteLine("Notification sent!");
                    }
                }
            }
            catch (Exception ex)
            { throw; }
            Console.WriteLine("Successfully Execute");
        }
        private static EntityCollection GetFromAdminCRM(IOrganizationService organizationService)
        {
            DL_systemuser adm = new DL_systemuser();

            QueryExpression queryExpression = new QueryExpression(adm.EntityName);
            queryExpression.ColumnSet.AddColumns("systemuserid", "domainname");
            queryExpression.Criteria.AddCondition("domainname", ConditionOperator.Equal, "TRAKNUS\\admin.crm");

            return adm.Select(organizationService, queryExpression);
        }
    }
}
