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
    public class BL_MarketSizeResultPSS
    {
        #region Dependencies
       
        DL_tss_sparepartsetup _DL_tss_sparepartsetup = new DL_tss_sparepartsetup();
        #endregion

        public void checkEscalationApprovalforMarketSizeResultPSS()
        {
            try
            {

                #region setup ConnString
                string _connectionString = string.Empty;
                if (ConnString.IsValidConnectionString(ConfigurationManager.ConnectionStrings["CRMCS"].ConnectionString))
                    _connectionString = ConfigurationManager.ConnectionStrings["CRMCS"].ConnectionString;
                else
                    _connectionString = "Url=http://10.0.10.43/traktornusantara; Domain=traknus; Username=admin.crm; Password=pass@word2;";

                string CRM_URL = _connectionString.Split(';')[0].Remove(0, 4);
                CrmServiceClient conn = new CrmServiceClient(_connectionString);
                #endregion

                IOrganizationService _orgService = (IOrganizationService)conn.OrganizationServiceProxy;

                #region getSparePart Config

                //DateTime startPeriodMS = new DateTime();
                //DateTime endPeriodMS = new DateTime();
                //FilterExpression fSetup = new FilterExpression(LogicalOperator.And);
                //fSetup.AddCondition("tss_name", ConditionOperator.Equal, "TSS");

                //QueryExpression qSetup = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
                //qSetup.ColumnSet = new ColumnSet(true);
                //qSetup.Criteria.AddFilter(fSetup);
                //EntityCollection setups = _orgService.RetrieveMultiple(qSetup);

                //if (setups.Entities.Count > 0)
                //{
                //    //TBA
                //    startPeriodMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_startdatems");
                //    endPeriodMS = setups.Entities[0].GetAttributeValue<DateTime>("tss_enddatems");
                //}

                #endregion

                #region get Market Size Result PSS

                FilterExpression fMSP = new FilterExpression(LogicalOperator.And);
                fMSP.AddCondition("tss_msperiodend", ConditionOperator.LessEqual, DateTime.Now);
                fMSP.AddCondition("tss_msperiodstart", ConditionOperator.GreaterEqual, DateTime.Now);
                fMSP.AddCondition("tss_status", ConditionOperator.Equal, 865920000);

                QueryExpression qMSP = new QueryExpression("tss_marketsizeresultpss");
                qMSP.Criteria.AddFilter(fMSP);
                qMSP.ColumnSet = new ColumnSet(true);

                EntityCollection ENC_MarketSizeResultPss = _orgService.RetrieveMultiple(qMSP);

                #endregion

                #region check if Approved
                foreach (Entity _entity in ENC_MarketSizeResultPss.Entities)
                {
                    Guid currApproverID = _entity.GetAttributeValue<EntityReference>("tss_currentapproval").Id;
                    DateTime currApprovalDate = _entity.GetAttributeValue<DateTime>("tss_currentapprovaldate");

                    #region getApproval Matrix
                    FilterExpression fappMat = new FilterExpression(LogicalOperator.And);
                    fappMat.AddCondition("tss_branch", ConditionOperator.Equal, _entity.GetAttributeValue<EntityReference>("tss_branch").Id);
                    fappMat.AddCondition("tss_approvaltype", ConditionOperator.Equal, 865920000);

                    QueryExpression qappMat = new QueryExpression("tss_matrixapprovalmarketsize");
                    qappMat.Criteria.AddFilter(fappMat);
                    qappMat.Orders.Add(new Microsoft.Xrm.Sdk.Query.OrderExpression("tss_priorityno", Microsoft.Xrm.Sdk.Query.OrderType.Ascending));
                    qappMat.ColumnSet = new ColumnSet(true);

                    EntityCollection ENC_approvalMatrixMarketSize = _orgService.RetrieveMultiple(qappMat);
                    #endregion

                    Entity currAppMatrix = ENC_approvalMatrixMarketSize.Entities.Where(x => x.GetAttributeValue<EntityReference>("tss_approver").Id == currApproverID).FirstOrDefault();

                    int hoursDifference = Convert.ToInt32((DateTime.Now - currApprovalDate).TotalHours);
                    int escalationHours = currAppMatrix.GetAttributeValue<int>("tss_escalationhour");
                    int currPriorityNo = currAppMatrix.GetAttributeValue<int>("tss_priorityno");

                    if (hoursDifference > escalationHours)
                    {
                        Entity nextAppMatrix = ENC_approvalMatrixMarketSize.Entities
                                               .Where(x => x.GetAttributeValue<int>("tss_priorityno") == currPriorityNo + 1).FirstOrDefault();

                        _entity["tss_currentapproval"] = nextAppMatrix.GetAttributeValue<EntityReference>("tss_approver").Id;
                        _orgService.Update(_entity);

                        //send Email 
                        EntityCollection admins = GetFromAdminCRM(_orgService);
                        Guid crmadmin = admins.Entities[0].Id;

                        string emailSubject = _entity.GetAttributeValue<string>("tss_name") + " is waiting for approval";
                        string emailContent = "Please review and approve.";
                        EmailAgent.SendEmailNotif(crmadmin, nextAppMatrix.GetAttributeValue<EntityReference>("tss_approver").Id, Guid.Empty, _orgService, emailSubject, emailContent);

                    }

                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected error while checking Market Size Result PSS Escalation. Error Details : " + ex.Message.ToString());
            }
        }

        private  EntityCollection GetFromAdminCRM(IOrganizationService organizationService)
        {
            DL_systemuser adm = new DL_systemuser();

            QueryExpression queryExpression = new QueryExpression(adm.EntityName);
            queryExpression.ColumnSet.AddColumns("systemuserid", "domainname");
            queryExpression.Criteria.AddCondition("domainname", ConditionOperator.Equal, "TRAKNUS\\admin.crm");

            return adm.Select(organizationService, queryExpression);
        }

    }
}
