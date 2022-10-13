using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using CrmEarlyBound;

namespace Generate_Market_Size_by_Key_Account
{
    class Program
    {
        private const int STATUS_OPEN = 865920000;
        private const int STATUS_ERROR = 865920002;
        

        static void Main(string[] args)
        {
            string _connectionString = string.Empty;
            if (IsValidConnectionString(ConfigurationManager.ConnectionStrings["Traknus"].ConnectionString))
                _connectionString = ConfigurationManager.ConnectionStrings["Traknus"].ConnectionString;
            else
                _connectionString = "Url=http://10.0.10.43/traktornusantara; Domain=traknus; Username=dev.crm; Password=p4ssw0rdc12m18;";

            try
            {
                Console.WriteLine("Console Generate Market Size by Key Account");
                Console.WriteLine("====================================");
                Console.WriteLine("Getting crmserviceclient..");
                CrmServiceClient conn = new CrmServiceClient(_connectionString);
                Console.WriteLine("Getting crmorganizationservice..");
                IOrganizationService _orgService = (IOrganizationService)conn.OrganizationServiceProxy;
                Console.WriteLine();
                //GenerateMarketSizebyKeyAccount(_orgService);
                CountbyLinq(_orgService);
                Console.WriteLine("====================================");
                Console.WriteLine("Finish!!!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }

        private static Boolean IsValidConnectionString(String connectionString)
        {
            if (connectionString.Replace(" ", "").ToLower().Contains("url=") ||
                connectionString.Replace(" ", "").ToLower().Contains("server=") ||
                connectionString.Replace(" ", "").ToLower().Contains("serviceuri="))
                return true;

            return false;
        }

        public static void CountbyLinq(IOrganizationService _orgService)
        {
            DateTime? nulldatetime = null;
            Guid keyaccountid = new Guid("FA28C281-7A4F-E911-9460-005056937630");
            Guid UserId = new Guid("1CBA90DC-4DE9-E111-9AA2-544249894792");

            List<Guid> asd = new List<Guid>();
            asd.Add(keyaccountid);

            FilterExpression _filterexpression = new FilterExpression(LogicalOperator.Or);
            _filterexpression.AddCondition("tss_status", ConditionOperator.Equal, STATUS_OPEN);
            _filterexpression.AddCondition("tss_status", ConditionOperator.Equal, STATUS_ERROR);

            FilterExpression _FilterExpression = new FilterExpression(LogicalOperator.And);
            _FilterExpression.AddFilter(_filterexpression);
            _FilterExpression.AddCondition("tss_keyaccountid", ConditionOperator.In, keyaccountid);
            _FilterExpression.AddCondition("tss_pss", ConditionOperator.Equal, UserId);

            QueryExpression _queryexpression = new QueryExpression("tss_keyaccount");
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddFilter(_FilterExpression);

            EntityCollection _keyaccountcollection = _orgService.RetrieveMultiple(_queryexpression);

            using (CrmServiceContext _context = new CrmServiceContext(_orgService))
            {
                var _alldatakauio = (from tbkauio in _context.tss_kauioSet
                                     join tbkeyaccount in _context.tss_keyaccountSet on tbkauio.tss_KeyAccountId.Id equals tbkeyaccount.tss_keyaccountId
                                     join tbpopulation in _context.new_populationSet on tbkauio.tss_SerialNumber.Id equals tbpopulation.new_populationId
                                     join tbproduct in _context.ProductSet on tbpopulation.trs_ProductMaster.Id equals tbproduct.ProductId
                                     join tbunitgroupmarketsize in _context.tss_unitgroupmarketsizeSet on tbpopulation.trs_ProductMaster equals tbunitgroupmarketsize.tss_Model
                                     //join tbunitgroupmarketsize in _context.tss_unitgroupmarketsizeSet on new { product = tbpopulation.trs_ProductMaster, unitgroup = tbproduct.DefaultUoMScheduleId } equals new { product = tbunitgroupmarketsize.tss_Model, unitgroup = tbunitgroupmarketsize.tss_UnitGroup }
                                     where (tbkeyaccount.tss_Status.Equals(STATUS_OPEN) || tbkeyaccount.tss_Status.Equals(STATUS_ERROR))
                                         && tbkeyaccount.tss_PSS.Id == UserId
                                         && tbkeyaccount.tss_Customer != null
                                         && tbkeyaccount.tss_ActiveEndDate >= DateTime.Now
                                         && tbkeyaccount.tss_ActiveStartDate <= DateTime.Now
                                         && tbkeyaccount.tss_CalculatetoMS == true
                                         && asd.Contains(tbkauio.tss_KeyAccountId.Id)
                                     //&& 
                                     //&& _keyaccountids.Contains(new Guid(tbkeyaccount.tss_keyaccountId.GetValueOrDefault().ToString()))
                                     select new
                                     {
                                         tbkeyaccount_LogicalName = tbkeyaccount.LogicalName,
                                         tbkeyaccount_tss_keyaccountId = tbkeyaccount.tss_keyaccountId,
                                         tbkeyaccount_tss_MSPeriodStart = tbkeyaccount.tss_MSPeriodStart,
                                         tbkeyaccount_tss_MSPeriodEnd = tbkeyaccount.tss_MSPeriodEnd,
                                         tbkeyaccount_tss_ActiveStartDate = tbkeyaccount.tss_ActiveStartDate,
                                         tbkeyaccount_tss_ActiveEndDate = tbkeyaccount.tss_ActiveEndDate,
                                         tbkeyaccount_tss_Revision = tbkeyaccount.tss_Revision,
                                         tbkauio_LogicalName = tbkauio.LogicalName,
                                         tbkauio_tss_PSS = tbkauio.tss_PSS,
                                         tbkauio_tss_Customer = tbkauio.tss_Customer,
                                         tbpopulation_LogicalName = tbpopulation.LogicalName,
                                         tbpopulation_new_SerialNumber = tbpopulation.new_SerialNumber,
                                         tbpopulation_tss_MSCurrentHourMeter = tbpopulation.tss_MSCurrentHourMeter,
                                         tbpopulation_tss_MSLastHourMeter = tbpopulation.tss_MSLastHourMeter,
                                         tbpopulation_tss_MSCurrentHourMeterDate = tbpopulation.tss_MSCurrentHourMeterDate,
                                         tbpopulation_tss_MSLastHourMeterDate = tbpopulation.tss_MSLastHourMeterDate,
                                         tbpopulation_new_DeliveryDate = tbpopulation.new_DeliveryDate,
                                         tbpopulation_trs_WarrantyStartdate = tbpopulation.trs_WarrantyStartdate,
                                         tbpopulation_trs_WarrantyEndDate = tbpopulation.trs_WarrantyEndDate,
                                         tbpopulation_tss_EstWorkingHour = tbpopulation.tss_EstWorkingHour,
                                         tbpopulation_tss_PopulationStatus = tbpopulation.tss_PopulationStatus,
                                         tbpopulation_new_populationId = tbpopulation.new_populationId,
                                         tbproduct_LogicalName = tbproduct.LogicalName,
                                         tbproduct_ProductId = tbproduct.ProductId,
                                         tbproduct_tss_UseTyre = tbproduct.tss_UseTyre,
                                         tbunitgroupmarketsize_LogicalName = tbunitgroupmarketsize.LogicalName,
                                         tbunitgroupmarketsize_tss_MaintenanceInterval = tbunitgroupmarketsize.tss_MaintenanceInterval
                                     }).ToList();

                Console.WriteLine("Count : " + _alldatakauio.Count());
            }
        }

        public static void GenerateMarketSizebyKeyAccount(IOrganizationService _orgService)
        {
            try
            {
            double count = 0;
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Data Source=10.0.10.43;User Id=sa; Password=pass@word2; Initial Catalog=CRMTrakNus_MSCRM";

                conn.Open();

                SqlCommand cmd = new SqlCommand("MS_GenerateMarketSize_KeyAccount", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@KeyAccountId", "FA28C281-7A4F-E911-9460-005056937630"));

                //throw new Exception("StartDate : " + StartDate + ", " + "EndDate : " + EndDate + ", " + "SalesID : " + salesId);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        tss_MasterMarketSize _tss_MasterMarketSize = new tss_MasterMarketSize();
                        _tss_MasterMarketSize.tss_keyaccountid = new EntityReference("tss_keyaccount", new Guid(rdr["KeyAccount"].ToString()));
                        _tss_MasterMarketSize.tss_SerialNumber = new EntityReference("new_population", new Guid(rdr["SerialNumber"].ToString()));
                        _tss_MasterMarketSize.tss_Customer = new EntityReference("account", new Guid(rdr["Customer"].ToString()));
                        _tss_MasterMarketSize.tss_PSS = new EntityReference("systemuser", new Guid(rdr["PSS"].ToString()));
                        //_tss_MasterMarketSize.tss_MSPeriodStart = Convert.ToDateTime(rdr["MSPeriodStart"]);
                        //_tss_MasterMarketSize.tss_MSPeriodEnd = Convert.ToDateTime(rdr["MSPeriodEnd"]);
                        //_tss_MasterMarketSize.tss_ActivePeriodStart = Convert.ToDateTime(rdr["MSActiveStart"]);
                        //_tss_MasterMarketSize.tss_ActivePeriodSEnd = Convert.ToDateTime(rdr["MSActveEnd"]);
                        _tss_MasterMarketSize.tss_UseTyre = Convert.ToBoolean(rdr["UseTyre"]);
                        _tss_MasterMarketSize.tss_ismsresultpssgenerated = Convert.ToBoolean(rdr["IsMSResultPSSGenerated"]);
                        _tss_MasterMarketSize.tss_issublinesgenerated = Convert.ToBoolean(rdr["IsSubLinesGenerated"]);
                        //if (!rdr.IsDBNull(13))
                        //    _tss_MasterMarketSize.tss_AvgHMMethod1 = Convert.ToDecimal(rdr["Method1"]);
                        
                        //_tss_MasterMarketSize.tss_AvgHMMethod2 = Convert.ToDecimal(rdr["Method2"]);
                        //_tss_MasterMarketSize.tss_AvgHMMethod3 = Convert.ToDecimal(rdr["Method3"]);
                        //_tss_MasterMarketSize.tss_PeriodPMMethod4 = Convert.ToDecimal(rdr["Method4"]);
                        //_tss_MasterMarketSize.tss_periodpmmethod5 = Convert.ToDecimal(rdr["Method5"]);

                        _orgService.Create(_tss_MasterMarketSize);
                        count += 1;
                    }
                }
            }

            Console.WriteLine(count.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }   
}
