using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace TrakNusSparepartSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            string _connectionString = string.Empty;
            if (IsValidConnectionString(ConfigurationManager.ConnectionStrings["Traknus"].ConnectionString))
                _connectionString = ConfigurationManager.ConnectionStrings["Traknus"].ConnectionString;
            else
                _connectionString = "Url=http://10.0.10.43/traktornusantara; Domain=traknus; Username=admin.crm; Password=pass@word2;";

            try
            {
                Console.WriteLine("Console Update Rating Prospect");
                Console.WriteLine("====================================");
                Console.WriteLine("Getting crmserviceclient..");
                CrmServiceClient conn = new CrmServiceClient(_connectionString);
                Console.WriteLine("Getting crmorganizationservice..");
                IOrganizationService _orgService = (IOrganizationService)conn.OrganizationServiceProxy;
                Console.WriteLine();
                CheckExpiredSetupDealer(_orgService);
                Console.WriteLine("====================================");
                Console.WriteLine("Finish!!!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static Boolean IsValidConnectionString(String connectionString)
        {
            if (connectionString.Replace(" ", "").ToLower().Contains("url=") ||
                connectionString.Replace(" ", "").ToLower().Contains("server=") ||
                connectionString.Replace(" ", "").ToLower().Contains("serviceuri="))
                return true;

            return false;
        }

        private static void CheckExpiredSetupDealer(IOrganizationService _IOrganizationService)
        {
            try
            {
                QueryExpression query = new QueryExpression("tss_dealerheader")
                {
                    ColumnSet = new ColumnSet(true),
                    Criteria =
                    {
                        Conditions =
                    {                                           
                        new ConditionExpression("tss_startdate", ConditionOperator.NotNull),
                        new ConditionExpression("tss_enddate", ConditionOperator.NotNull)
                    }
                    }
                };
                EntityCollection items = _IOrganizationService.RetrieveMultiple(query);
                foreach (var item in items.Entities)
                {
                    LocalTimeFromUtcTimeRequest convert = new LocalTimeFromUtcTimeRequest
                    {
                        UtcTime = item.GetAttributeValue<DateTime>("tss_enddate"),
                        TimeZoneCode = 205 // Timezone of user
                    };
                    LocalTimeFromUtcTimeResponse response = (LocalTimeFromUtcTimeResponse)_IOrganizationService.Execute(convert);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private bool IsSetupDealerExpired(IOrganizationService _IOrganizationService, DateTime EndDate)
        {

        }
    }
}
