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
using TrakNusSparepartSystem.Helper;

namespace Check_Escalation_Approval_for_Market_Size_Result.Business_Layer
{

    public class BL_SendNotification
    {
        DL_tss_mastermarketsize _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        DL_tss_sparepartsetup _DL_tss_sparepartsetup = new DL_tss_sparepartsetup();
        RetrieveHelper _retrievehelper = new RetrieveHelper();

        private const int APPPDH = 865920001;
        public void SendNotifBeforStardPeriod()
        {
            string _connectionString = string.Empty;
            int notifBeforStarPeriod = 0;
            DateTime msPeriodStart;
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
                IOrganizationService _orgService = (IOrganizationService)conn.OrganizationServiceProxy;
                FilterExpression fSetup = new FilterExpression(LogicalOperator.And);
                fSetup.AddCondition("tss_name", ConditionOperator.Equal, "TSS");

                QueryExpression qSetup = new QueryExpression(_DL_tss_sparepartsetup.EntityName);
                qSetup.ColumnSet = new ColumnSet(true);
                qSetup.Criteria.AddFilter(fSetup);
                EntityCollection setups = _DL_tss_sparepartsetup.Select(_orgService, qSetup);

                if (setups.Entities.Count > 0)
                {
                    //TBA
                    notifBeforStarPeriod = setups.Entities[0].GetAttributeValue<int>("tss_startdatems");
                }
                #region Loop PSS Data
                FilterExpression fMarketSize = new FilterExpression(LogicalOperator.And);
                fMarketSize.AddCondition("tss_activeperiodsend", ConditionOperator.LessEqual, DateTime.Now);
                fMarketSize.AddCondition("tss_activeperiodstart", ConditionOperator.GreaterEqual, DateTime.Now);
                //fPSS.AddCondition("tss_status", ConditionOperator.Equal, APPPDH);

                QueryExpression qMarketSize = new QueryExpression(_DL_tss_mastermarketsize.EntityName);
                qMarketSize.Criteria.AddFilter(fMarketSize);
                qMarketSize.ColumnSet = new ColumnSet(true);
                List<Entity> marketSize = _retrievehelper.RetrieveMultiple(_orgService, qMarketSize); // _DL_tss_mastermarketsize.Select(_orgService, qMarketSize);
                foreach (Entity entity in marketSize)
                {
                }
                
                #endregion
            }
            catch (Exception)
            {

                throw;
            }
           
        }
    }
}
