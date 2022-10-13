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

using Check_Escalation_Approval_for_Market_Size_Result.Helper;
using TrakNusSparepartSystem.DataLayer;

namespace Check_Escalation_Approval_for_Market_Size_Result.Business_Layer
{
    public class BL_SalesTargetNational
    {
        /*ToBe Confirm ko santoni*/
        private const int APPKORWIL = 865920001;
        static DL_tss_salestargetnational _DL_tss_salestargetnationl = new DL_tss_salestargetnational();
        static DL_tss_matrixapprovalmarketsize _DL_tss_matrixapprovalmarketsize = new DL_tss_matrixapprovalmarketsize();
        public void checkEscalationApprovalforSalesTargetBranch()
        {
            string _connectionString = string.Empty;
            if (ConnString.IsValidConnectionString(ConfigurationManager.ConnectionStrings["CRMCS"].ConnectionString))
                _connectionString = ConfigurationManager.ConnectionStrings["CRMCS"].ConnectionString;
            else
                _connectionString = "Url=http://10.0.10.43/traktornusantara; Domain=traknus; Username=admin.crm; Password=pass@word2;";

            string CRM_URL = _connectionString.Split(';')[0].Remove(0, 4);

            Console.WriteLine("Console Check Approval Market Size Result Branch");
            Console.WriteLine("====================================");
            Console.WriteLine("Getting crmserviceclient..");
            CrmServiceClient conn = new CrmServiceClient(_connectionString); try
            {
                Console.WriteLine("Getting crmorganizationservice..");
                IOrganizationService _orgService = (IOrganizationService)conn.OrganizationServiceProxy;

                FilterExpression fNational = new FilterExpression(LogicalOperator.And);
                fNational.AddCondition("tss_activeperiodsend", ConditionOperator.LessEqual, DateTime.Now);
                fNational.AddCondition("tss_activeperiodstart", ConditionOperator.GreaterEqual, DateTime.Now);
                fNational.AddCondition("tss_status", ConditionOperator.Equal, APPKORWIL);

                QueryExpression qNational = new QueryExpression(_DL_tss_salestargetnationl.EntityName);
                qNational.Criteria.AddFilter(fNational);
                qNational.ColumnSet = new ColumnSet(true);
                EntityCollection stNational = _DL_tss_salestargetnationl.Select(_orgService, qNational);

                if (stNational.Entities.Count > 0)
                {
                    foreach (var national in stNational.Entities)
                    {
                        FilterExpression fMat = new FilterExpression(LogicalOperator.And);
                        
                        fMat.AddCondition("tss_approvaltype", ConditionOperator.Equal, 865920002);/* approval type Division Head*/
                        fMat.AddCondition("tss_approvalfor", ConditionOperator.Equal, 865920001);/* approval for sales target */

                        QueryExpression qMat = new QueryExpression(_DL_tss_matrixapprovalmarketsize.EntityName);
                        qMat.Criteria.AddFilter(fMat);
                        qMat.ColumnSet = new ColumnSet(true);
                        qMat.Orders.Add(new Microsoft.Xrm.Sdk.Query.OrderExpression("tss_priorityno", Microsoft.Xrm.Sdk.Query.OrderType.Ascending));
                        EntityCollection mats = _DL_tss_matrixapprovalmarketsize.Select(_orgService, qMat);

                        Entity currMats = mats.Entities.Where(o => o.Id == national.GetAttributeValue<EntityReference>("tss_currentapproval").Id).FirstOrDefault();

                        DateTime currApprovalDate = national.GetAttributeValue<DateTime>("tss_currentapprovaldate");
                        int currEscHr = currMats.GetAttributeValue<int>("tss_escalatehour");
                        int currPriority = currMats.GetAttributeValue<int>("tss_priorityno");
                        int diff = (DateTime.Now - currApprovalDate).Hours;
                        Console.WriteLine("Different in hour: " + diff);

                        if (mats.Entities.Count > 0)
                        {
                            if (diff > currEscHr)
                            {
                                int nextPriority = currPriority + 1;
                                Entity nextApp = mats.Entities.Where(x => x.GetAttributeValue<int>("tss_priorityno") == nextPriority).FirstOrDefault();

                                if (nextApp != null)
                                {
                                    EntityCollection admins = GetFromAdminCRM(_orgService);
                                    Guid crmadmin = admins.Entities[0].Id;

                                    string emailSubject = national.GetAttributeValue<string>("tss_name") + " is waiting for KORWIL approval";
                                    string emailContent = "Please review and approve.";
                                    EmailAgent.SendEmailNotif(crmadmin, nextApp.GetAttributeValue<EntityReference>("tss_approver").Id, Guid.Empty, _orgService, emailSubject, emailContent);
                                    Console.WriteLine("Notification sent!");

                                    national["tss_currentapproval"] = nextApp.GetAttributeValue<EntityReference>("tss_approver").Id;
                                    _orgService.Update(national);
                                    Console.WriteLine("Update current approval!");
                                }
                                else
                                {

                                    Console.WriteLine("Next approval not found! Notification will be sent later!");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Notification has reached the last approval!");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
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
