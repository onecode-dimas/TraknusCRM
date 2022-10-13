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

namespace Update_Rating_Prospect
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
                UpdateProspectHeader(_orgService);
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

        public static int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }

        public static void UpdateProspectHeader(IOrganizationService _orgService)
        {
            int OpenStatus = OptionSetExtractor.GetOptionSetValue(_orgService, "tss_prospectpartheader", "tss_statusreason", "Open");

            QueryExpression query = new QueryExpression("tss_prospectpartheader")
            {
                ColumnSet = new ColumnSet(true),
                Criteria =
                {
                    Conditions =
                    {                                           
                        new ConditionExpression("tss_topic", ConditionOperator.NotNull),
                        new ConditionExpression("tss_statusreason", ConditionOperator.Equal,OpenStatus), //only status Open will be update
                    }
                }
            };
            query.AddOrder("tss_topic", OrderType.Ascending);

            EntityCollection items = _orgService.RetrieveMultiple(query);

            foreach (var item in items.Entities)
            {
                //DateTime EstimationCloseDate = DateTime.MinValue;

                //Estimation Close Date
                //if (item.Attributes.Contains("tss_estimationclosedate"))
                //    EstimationCloseDate = item.GetAttributeValue<DateTime>("tss_estimationclosedate");
                //else
                //    throw new Exception("Can not found Estimation Close Date !");

                try
                {
                    LocalTimeFromUtcTimeRequest convert = new LocalTimeFromUtcTimeRequest
                    {
                        UtcTime = item.GetAttributeValue<DateTime>("tss_estimationclosedate"),
                        TimeZoneCode = 205 // Timezone of user
                    };
                    LocalTimeFromUtcTimeResponse response = (LocalTimeFromUtcTimeResponse)_orgService.Execute(convert);
                    int Compare = DateTime.Compare(response.LocalTime.Date, DateTime.Now.Date);

                    if (Compare < 0)
                    {
                        #region Overdue
                        QueryExpression OverdueQuery = new QueryExpression("tss_rating")
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

                        /*EntityCollection ratings = _orgService.RetrieveMultiple(query);
                        if (ratings.Entities.Count > 0)
                        {
                            foreach (var rating in ratings.Entities)
                            {
                                item.Attributes["tss_rating"] = new EntityReference(rating.LogicalName, rating.Id);
                                _orgService.Update(item);
                                Console.WriteLine("Guid: {0}, Topic: {1}, update based on Overdue Succesfull", item.Id.ToString(), item.GetAttributeValue<string>("tss_topic"));
                            }
                        }*/

                        EntityCollection RatingOverdue = _orgService.RetrieveMultiple(OverdueQuery);
                        if (RatingOverdue.Entities.Count > 0) //when Rating using Period Week Found in Setup
                        {
                            Entity rating = _orgService.RetrieveMultiple(OverdueQuery).Entities[0]; //get the first value of rating basen on query tss_periodis null (Overdue)
                            item.Attributes["tss_rating"] = new EntityReference(rating.LogicalName, rating.Id);
                            _orgService.Update(item);  //update entity prospect header rating to be oeverdue
                            Console.WriteLine("Guid: {0}, Topic: {1}, Notes: Update based on Overdue Succesfull", item.Id.ToString(), item.GetAttributeValue<string>("tss_topic"));
                        }
                        #endregion
                    }
                    else
                    {
                        //if est close date bigger than today date
                        //get monnth or week
                        //DateTime compareTo = DateTime.Parse("8/13/2010 8:33:21 AM");
                        //DateTime now = DateTime.Parse("2/9/2012 10:10:11 AM");
                        //var dateSpan = DateTimeSpan.CompareDates(compareTo, now);

                        var dateSpan = CalculateDate.DateTimeSpan.CompareDates(response.LocalTime.Date, DateTime.Now.Date);
                        int TotalMonths = dateSpan.Months;
                        int TotalDays = dateSpan.Days;
                        //int TotalMonths = GetMonthDifference(DateTime.Now.Date, item.GetAttributeValue<DateTime>("tss_estimationclosedate").Date);
                        if (TotalMonths > 0)
                        {
                            if (TotalMonths > 0 && TotalDays > 0)
                            {
                                TotalMonths += 1;  //inverse +1 in TotalMonth

                                //update to be warm
                                #region Month
                                int PeriodValueMonth = OptionSetExtractor.GetOptionSetValue(_orgService, "tss_rating", "tss_period", "Month");

                                int PeriodValueWeek = OptionSetExtractor.GetOptionSetValue(_orgService, "tss_rating", "tss_period", "Week");

                                QueryExpression MonthQuery = new QueryExpression("tss_rating")
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
                                        //new ConditionExpression("tss_endperiod", ConditionOperator.GreaterThan, TotalMonths),
                                    }
                                    }
                                };

                                /*EntityCollection ratings = _orgService.RetrieveMultiple(query);
                                if (ratings.Entities.Count > 0)
                                {
                                    foreach (var rating in ratings.Entities)
                                    {
                                        item.Attributes["tss_rating"] = new EntityReference(rating.LogicalName, rating.Id);
                                        _orgService.Update(item);
                                        Console.WriteLine("Guid: {0}, Topic: {1}, update based on Month Succesfull", item.Id.ToString(), item.GetAttributeValue<string>("tss_topic"));
                                    }
                                }*/

                                EntityCollection RatingMonths = _orgService.RetrieveMultiple(MonthQuery);
                                if (RatingMonths.Entities.Count > 0) //when Rating using Period Week Found in Setup
                                {
                                    Entity rating = _orgService.RetrieveMultiple(MonthQuery).Entities[0];
                                    if (rating.Id != Guid.Empty)
                                    {
                                        item.Attributes["tss_rating"] = new EntityReference(rating.LogicalName, rating.Id);
                                        _orgService.Update(item);
                                        Console.WriteLine("Guid: {0}, Topic: {1}, Notes: Update based on Month Succesfull", item.Id.ToString(), item.GetAttributeValue<string>("tss_topic"));
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                #region Month
                                int PeriodValueMonth = OptionSetExtractor.GetOptionSetValue(_orgService, "tss_rating", "tss_period", "Month");

                                int PeriodValueWeek = OptionSetExtractor.GetOptionSetValue(_orgService, "tss_rating", "tss_period", "Week");

                                QueryExpression MonthQuery = new QueryExpression("tss_rating")
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
                                        //new ConditionExpression("tss_endperiod", ConditionOperator.GreaterThan, TotalMonths),
                                    }
                                    }
                                };

                                EntityCollection RatingMonths = _orgService.RetrieveMultiple(MonthQuery);
                                if (RatingMonths.Entities.Count > 0) //when Rating using Period Week Found in Setup
                                {
                                    Entity rating = _orgService.RetrieveMultiple(MonthQuery).Entities[0];
                                    if (rating.Id != Guid.Empty)
                                    {
                                        item.Attributes["tss_rating"] = new EntityReference(rating.LogicalName, rating.Id);
                                        _orgService.Update(item);
                                        Console.WriteLine("Guid: {0}, Topic: {1}, Notes: Update based on Month Succesfull", item.Id.ToString(), item.GetAttributeValue<string>("tss_topic"));
                                    }
                                }
                                #endregion
                            }

                            //#region Month
                            //int PeriodValueMonth = OptionSetExtractor.GetOptionSetValue(_orgService, "tss_rating", "tss_period", "Month");

                            //int PeriodValueWeek = OptionSetExtractor.GetOptionSetValue(_orgService, "tss_rating", "tss_period", "Week");

                            //QueryExpression MonthQuery = new QueryExpression("tss_rating")
                            //{
                            //    ColumnSet = new ColumnSet(true),
                            //    Criteria =
                            //    {
                            //        Conditions =
                            //        {      
                            //            //new ConditionExpression("tss_period", ConditionOperator.NotNull),
                            //            //new ConditionExpression("tss_period", ConditionOperator.NotEqual, PeriodValueWeek),
                            //            new ConditionExpression("tss_period", ConditionOperator.Equal, PeriodValueMonth),
                            //            new ConditionExpression("tss_startperiod", ConditionOperator.LessEqual, TotalMonths),
                            //            new ConditionExpression("tss_endperiod", ConditionOperator.GreaterEqual, TotalMonths),
                            //            //new ConditionExpression("tss_endperiod", ConditionOperator.GreaterThan, TotalMonths),
                            //        }
                            //    }
                            //};

                            ///*EntityCollection ratings = _orgService.RetrieveMultiple(query);
                            //if (ratings.Entities.Count > 0)
                            //{
                            //    foreach (var rating in ratings.Entities)
                            //    {
                            //        item.Attributes["tss_rating"] = new EntityReference(rating.LogicalName, rating.Id);
                            //        _orgService.Update(item);
                            //        Console.WriteLine("Guid: {0}, Topic: {1}, update based on Month Succesfull", item.Id.ToString(), item.GetAttributeValue<string>("tss_topic"));
                            //    }
                            //}*/

                            //EntityCollection RatingMonths = _orgService.RetrieveMultiple(MonthQuery);
                            //if (RatingMonths.Entities.Count > 0) //when Rating using Period Week Found in Setup
                            //{
                            //    Entity rating = _orgService.RetrieveMultiple(MonthQuery).Entities[0];
                            //    if (rating.Id != Guid.Empty)
                            //    {
                            //        item.Attributes["tss_rating"] = new EntityReference(rating.LogicalName, rating.Id);
                            //        _orgService.Update(item);
                            //        Console.WriteLine("Guid: {0}, Topic: {1}, Notes: Update based on Month Succesfull", item.Id.ToString(), item.GetAttributeValue<string>("tss_topic"));
                            //    }
                            //}
                            //#endregion
                        }
                        else
                        {
                            #region Week
                            //int PeriodValueMonth = OptionSetExtractor.GetOptionSetValue(_orgService, "tss_rating", "tss_period", "Month");
                            int PeriodValueWeek = OptionSetExtractor.GetOptionSetValue(_orgService, "tss_rating", "tss_period", "Week");

                            int days = (response.LocalTime.DayOfYear - DateTime.Now.DayOfYear);
                            int weeks = (days - (days % 7)) / 7;  //convert days to week

                            QueryExpression WeekQuery = new QueryExpression("tss_rating")
                            {
                                ColumnSet = new ColumnSet(true),
                                Criteria =
                                {
                                    Conditions =
                                    {                     
                                        //new ConditionExpression("tss_rating", ConditionOperator.NotNull),
                                        //new ConditionExpression("tss_period", ConditionOperator.NotEqual, PeriodValueMonth),
                                        new ConditionExpression("tss_period", ConditionOperator.Equal, PeriodValueWeek),
                                        new ConditionExpression("tss_startperiod", ConditionOperator.LessEqual, weeks),
                                        new ConditionExpression("tss_endperiod", ConditionOperator.GreaterEqual, weeks),
                                    }
                                }
                            };

                            EntityCollection RatingWeeks = _orgService.RetrieveMultiple(WeekQuery);
                            if (RatingWeeks.Entities.Count > 0) //when Rating using Period Week Found in Setup
                            {
                                Entity rating = _orgService.RetrieveMultiple(WeekQuery).Entities[0];
                                if (rating.Id != Guid.Empty)
                                {
                                    item.Attributes["tss_rating"] = new EntityReference(rating.LogicalName, rating.Id);
                                    //_orgService.Update(item);
                                    Console.WriteLine("Guid: {0}, Topic: {1}, Notes: Update based on Week Succesfull", item.Id.ToString(), item.GetAttributeValue<string>("tss_topic"));
                                }
                            }
                            else
                            {
                                //get Rating Hot
                                #region Update less than a month to be Rating Hot
                                QueryExpression HotQuery = new QueryExpression("tss_rating")
                                {
                                    ColumnSet = new ColumnSet(true),
                                    Criteria =
                                    {
                                        Conditions =
                                        {                     
                                            new ConditionExpression("tss_rating", ConditionOperator.Equal, "Hot"),
                                        }
                                    }
                                };
                                Entity HotRating = _orgService.RetrieveMultiple(HotQuery).Entities[0];
                                if (HotRating.Id != Guid.Empty)
                                {
                                    item.Attributes["tss_rating"] = new EntityReference("tss_rating", HotRating.Id);  //updateto Hot
                                    _orgService.Update(item);
                                    Console.WriteLine("Guid: {0}, Topic: {1}, Notes: Update based on Hot Succesfull", item.Id.ToString(), item.GetAttributeValue<string>("tss_topic"));
                                }
                                #endregion
                            } 
                            #endregion                               
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            var Total = items.Entities.Count;
            Console.WriteLine("\nTotal Data in Open Status:{0}", Total.ToString());//count total data
        }

    }
}
