using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Update_Rating_Prospect
{
    class Program
    {
        static void Main(string[] args)
        {
            //CrmServiceClient service = new CrmServiceClient(ConfigurationManager.ConnectionStrings["Traknus"].ConnectionString);

            //XrmServiceContext XrmServiceContext = new XrmServiceContext(service);

            string _connectionString = string.Empty;
            if (IsValidConnectionString(ConfigurationManager.ConnectionStrings["Traknus"].ConnectionString))
                _connectionString = ConfigurationManager.ConnectionStrings["Traknus"].ConnectionString;
            //else
            //  _connectionString = "Url=http://10.0.10.43/traktornusantara; Domain=traknus; Username=admin.crm; Password=pass@word2;";

            CrmServiceClient conn = new CrmServiceClient(_connectionString);
            IOrganizationService _orgService = (IOrganizationService)conn.OrganizationServiceProxy;

            Console.WriteLine("success connect to crm org service");
            UpdateProspectHeader(_orgService);
            //RetrieveRating(_orgService);
            Console.WriteLine("Finish!!!");
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

        public static int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }


        public static void UpdateProspectHeader(IOrganizationService _orgService)
        {
            QueryExpression query = new QueryExpression("tss_prospectpartheader")
            {
                ColumnSet = new ColumnSet(true),
                Criteria =
                {
                    Conditions =
                    {                                           
                        new ConditionExpression("tss_name", ConditionOperator.NotNull),
                    }
                }
            };

            //query.AddOrder("tss_topic", OrderType.Ascending);

            EntityCollection items = _orgService.RetrieveMultiple(query);

            foreach (var item in items.Entities)
            {
                //Console.WriteLine("Guid: {0}, Topic: {1}, Estimated Close Date: {2}, Rating: {3}", item.Id.ToString(), item.GetAttributeValue<string>("tss_topic"), item.GetAttributeValue<DateTime>("tss_estimationclosedate").ToString(), (item.GetAttributeValue<EntityReference>("tss_rating") == null ? "" : item.GetAttributeValue<EntityReference>("tss_rating").Name)); 

                try
                {
                    if (item.GetAttributeValue<DateTime>("tss_estimationclosedate").Date < DateTime.Now.Date)  //jika estimated date lebihkecil hari ini maka overdue
                    {
                        //update menjadi overdue
                        QueryExpression rquery = new QueryExpression("tss_rating")
                        {
                            ColumnSet = new ColumnSet(true),
                            Criteria =
                            {
                                Conditions =
                                {                                           
                                    new ConditionExpression("tss_period", ConditionOperator.Null),  //overdue
                                    new ConditionExpression("tss_isoverdue", ConditionOperator.Equal,true), 
                                }
                            }
                        };
                        Entity rating = _orgService.RetrieveMultiple(rquery).Entities[0]; //get the first value of rating basen on query tss_periodis null (Overdue)
                        item.Attributes["tss_rating"] = new EntityReference(rating.LogicalName, rating.Id);
                        _orgService.Update(item);  //update entity prospect header rating to be oeverdue
                        Console.WriteLine("Guid: {0}, Topic: {1}, update based on Overdue", item.Id.ToString(), item.GetAttributeValue<string>("tss_name"));
                    }
                    else
                    {
                        //if est close date bigger than today date
                        //get monnth or week
                        int TotalMonths = GetMonthDifference(DateTime.Now.Date, item.GetAttributeValue<DateTime>("tss_estimationclosedate").Date);
                        if (TotalMonths > 0)
                        {
                            int PeriodValueMonth = OptionSetExtractor.GetOptionSetValue(_orgService, "tss_rating", "tss_period", "Month");

                            int PeriodValueWeek = OptionSetExtractor.GetOptionSetValue(_orgService, "tss_rating", "tss_period", "Week");

                            QueryExpression rquery = new QueryExpression("tss_rating")
                            {
                                ColumnSet = new ColumnSet(true),
                                Criteria =
                                {
                                    Conditions =
                                {      
                                    //new ConditionExpression("tss_period", ConditionOperator.NotNull),
                                    //new ConditionExpression("tss_period", ConditionOperator.NotEqual, PeriodValueWeek),
                                    new ConditionExpression("tss_period", ConditionOperator.Equal, PeriodValueMonth),
                                    new ConditionExpression("tss_startperiod", ConditionOperator.LessEqual, TotalMonths),
                                    new ConditionExpression("tss_endperiod", ConditionOperator.GreaterEqual, TotalMonths),
                                }
                                }
                            };

                            /*QueryExpression rquery = new QueryExpression("tss_rating");
                            rquery.ColumnSet = new ColumnSet(true);

                            rquery.Criteria = new FilterExpression();
                            rquery.Criteria.AddCondition("tss_period", ConditionOperator.NotNull);
                            rquery.Criteria.AddCondition("tss_period", ConditionOperator.Contains, PeriodValueMonth);

                            FilterExpression childFilter = rquery.Criteria.AddFilter(LogicalOperator.And);
                            childFilter.AddCondition("tss_period", ConditionOperator.Equal, PeriodValueMonth);

                            childFilter.AddCondition("tss_startperiod", ConditionOperator.LessEqual, TotalMonths);
                            childFilter.AddCondition("tss_endperiod", ConditionOperator.GreaterEqual, TotalMonths);*/

                            //EntityCollection ratings = _orgService.RetrieveMultiple(rquery);
                            //Entity ratingperiod = ratings.Entities.Where(o => o["tss_period"] == new OptionSetValue(PeriodValue)).Single();
                            //Console.WriteLine(ratingperiod.Id.ToString());

                            //EntityCollection ratings = _orgService.RetrieveMultiple(query);
                            //foreach (var rating in ratings.Entities)
                            //{
                            //    if (rating.Attributes.Contains("tss_period"))
                            //    {
                            //        if (rating.GetAttributeValue<OptionSetValue>("tss_period").Value != null)
                            //        {
                            //            item.Attributes["tss_rating"] = new EntityReference(rating.LogicalName, rating.Id);
                            //            _orgService.Update(item);
                            //            Console.WriteLine("Guid: {0}, Topic: {1}, update based on Week", item.Id.ToString(), item.GetAttributeValue<string>("tss_name"));
                            //        }
                            //    }

                            //}


                            Entity rating = _orgService.RetrieveMultiple(rquery).Entities[0];
                            item.Attributes["tss_rating"] = new EntityReference(rating.LogicalName, rating.Id);
                            _orgService.Update(item);
                            Console.WriteLine("Guid: {0}, Topic: {1}, update based on Month", item.Id.ToString(), item.GetAttributeValue<string>("tss_name"));
                        }
                        else
                        {
                            //var totalDays = (item.GetAttributeValue<DateTime>("tss_estimationclosedate") - DateTime.Now.Date).TotalDays;

                            int days = (item.GetAttributeValue<DateTime>("tss_estimationclosedate").DayOfYear - DateTime.Now.DayOfYear);

                            int weeks = (days - (days % 7)) / 7;  //convert days to week

                            int PeriodValueWeek = OptionSetExtractor.GetOptionSetValue(_orgService, "tss_rating", "tss_period", "Week");
                            int PeriodValueMonth = OptionSetExtractor.GetOptionSetValue(_orgService, "tss_rating", "tss_period", "Month");

                            QueryExpression rquery = new QueryExpression("tss_rating")
                            {
                                ColumnSet = new ColumnSet(true),
                                Criteria =
                                {
                                    Conditions =
                                    {                     
                                        //new ConditionExpression("tss_period", ConditionOperator.NotNull),
                                        //new ConditionExpression("tss_period", ConditionOperator.NotEqual, PeriodValueMonth),
                                        new ConditionExpression("tss_period", ConditionOperator.Equal, PeriodValueWeek),
                                        new ConditionExpression("tss_startperiod", ConditionOperator.LessEqual, weeks),
                                        new ConditionExpression("tss_endperiod", ConditionOperator.GreaterEqual, weeks),
                                    }
                                }
                            };

                            //EntityCollection ratings = _orgService.RetrieveMultiple(query);
                            //foreach (var rating in ratings.Entities)
                            //{
                            //    if (rating.Attributes.Contains("tss_period"))
                            //    {
                            //        item.Attributes["tss_rating"] = new EntityReference(rating.LogicalName, rating.Id);
                            //        _orgService.Update(item);
                            //        Console.WriteLine("Guid: {0}, Topic: {1}, update based on Week", item.Id.ToString(), item.GetAttributeValue<string>("tss_name"));
                            //    }

                            //}
                            Entity rating = _orgService.RetrieveMultiple(rquery).Entities[0];
                            item.Attributes["tss_rating"] = new EntityReference(rating.LogicalName, rating.Id);
                            _orgService.Update(item);
                            Console.WriteLine("Guid: {0}, Topic: {1}, update based on Week", item.Id.ToString(), item.GetAttributeValue<string>("tss_name"));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

        }

        static void RetrieveRating(IOrganizationService _orgService)
        {
            int PeriodValueMonth = OptionSetExtractor.GetOptionSetValue(_orgService, "tss_rating", "tss_period", "Month");

            int PeriodValueWeek = OptionSetExtractor.GetOptionSetValue(_orgService, "tss_rating", "tss_period", "Week");

            QueryExpression query = new QueryExpression("tss_rating")
            {
                ColumnSet = new ColumnSet(true),
                Criteria =
                {
                    Conditions =
                    {                                           
                        new ConditionExpression("tss_period", ConditionOperator.NotNull),
                        new ConditionExpression("tss_period", ConditionOperator.NotEqual, PeriodValueWeek),
                    }
                }
            };

            query.AddOrder("tss_rating", OrderType.Ascending);

            EntityCollection items = _orgService.RetrieveMultiple(query);

            foreach (var item in items.Entities)
            {
                //string PeriodName = OptionSetExtractor.GetOptionSetText(_orgService, "tss_rating", "tss_period", item.GetAttributeValue<OptionSetValue>("tss_period").Value);

                Console.WriteLine("Guid: " + item.Id.ToString());
                Console.WriteLine("Rating: " + item.GetAttributeValue<string>("tss_rating"));
                //Console.WriteLine("Period: " + c == null ? "" : PeriodName); //option set
                Console.WriteLine("Start Period: " + item.GetAttributeValue<int>("tss_startperiod"));  //whole number
                Console.WriteLine("End Period: " + item.GetAttributeValue<int>("tss_endperiod"));   //whole number
                Console.WriteLine("is Overdue: " + item.GetAttributeValue<bool>("tss_isoverdue"));
                Console.WriteLine();
            }
        }
    }
}
