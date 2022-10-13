using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System.Configuration;


namespace Check_Aging_Overdue_Prospect
{
    class Program
    {
        //- Create Console Application to Send Email Notification
        //- If there is any Prospect with Overdue Rating or Prospect exceed Overdue allowance (based on notification overdue prospect & send notification overdue prospect) then send email notification to PDH and HO user
        //- Add Console Application to Windows Scheduler and set schedule to run daily

        static void Main(string[] args)
        {
            string _connectionString = string.Empty;
            if (IsValidConnectionString(ConfigurationManager.ConnectionStrings["CRMCS"].ConnectionString))
                _connectionString = ConfigurationManager.ConnectionStrings["CRMCS"].ConnectionString;
            else
                _connectionString = "Url=http://10.0.10.43/traktornusantara; Domain=traknus; Username=admin.crm; Password=pass@word2;";
            Console.WriteLine("Console Check Aging Overdue Prospect");
            Console.WriteLine("====================================");
            Console.WriteLine("Getting crmserviceclient..");
            CrmServiceClient conn = new CrmServiceClient(_connectionString);
            try
            {
                Console.WriteLine("Getting crmorganizationservice..");
                IOrganizationService _orgService = (IOrganizationService)conn.OrganizationServiceProxy;
                //create
                /*
                Entity prospectpartheader = new Entity("tss_prospectpartheader");
                prospectpartheader.Attributes["tss_name"] = "TEST CREATE";
                prospectpartheader.Attributes["tss_description"] = "test description";
                _orgService.Create(prospectpartheader);
                */

                //retreive multiple
                
                QueryExpression query = new QueryExpression();
                query.EntityName = "tss_prospectpartheader";
                query.ColumnSet = new ColumnSet(true);
                query.AddOrder("tss_estimationclosedate", OrderType.Ascending);
                Console.WriteLine("Loading ProspectPartHeader Records..");
                DataCollection<Entity> entities = _orgService.RetrieveMultiple(query).Entities;
                foreach (var entity in entities)
                {
                    Console.WriteLine();
                    Console.WriteLine("Prospect header with Id: " + entity.Id.ToString());
                    try
                    {
                        if (entity.Contains("tss_rating"))
                        {
                            Guid ratingID = entity.GetAttributeValue<EntityReference>("tss_rating").Id;
                            Entity rating = _orgService.Retrieve("tss_rating", ratingID, new ColumnSet(true));
                            if (rating.GetAttributeValue<bool>("tss_isoverdue"))
                            {
                                SendEmailNotif(entity.Id, _orgService);
                            }
                            else
                            {
                                if (entity.Contains("tss_estimationclosedate"))
                                {
                                    DateTime today = DateTime.Today.Date;
                                    DateTime estclosedate = entity.GetAttributeValue<DateTime>("tss_estimationclosedate").Date;
                                    QueryExpression queryOverdue = new QueryExpression("tss_sparepartsetup")
                                    {
                                        ColumnSet = new ColumnSet(true),
                                        Criteria =
                                        {
                                            Conditions =
                                        {
                                            new ConditionExpression("tss_notifoverdue", ConditionOperator.NotNull),
                                        }
                                        }
                                    };
                                    Entity setup = _orgService.RetrieveMultiple(queryOverdue).Entities[0];
                                    OptionSetValue period = setup.GetAttributeValue<OptionSetValue>("tss_period");
                                    int notifoverdue = setup.GetAttributeValue<int>("tss_notifoverdue");
                                    string periodTxt = OptionSetExtractor.GetOptionSetText(_orgService, "tss_sparepartsetup", "tss_period", period.Value);
                                    if (periodTxt == "Week")
                                    {
                                        int days = notifoverdue * 7;
                                        if (today > estclosedate.AddDays(days))
                                        {
                                            SendEmailNotif(entity.Id, _orgService);
                                            Console.WriteLine("Notification Email sent!");
                                        }
                                        else
                                            Console.WriteLine("No notification");
                                    }
                                    else
                                    {
                                        if (today > estclosedate.AddMonths(notifoverdue))
                                        {
                                            SendEmailNotif(entity.Id, _orgService);
                                            Console.WriteLine("Notification Email sent!");
                                        }
                                        else
                                            Console.WriteLine("No notification");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("No notification");
                                }
                            }

                        }
                        else if (entity.Contains("tss_estimationclosedate"))
                        {
                            DateTime today = DateTime.Today.Date;
                            DateTime estclosedate = entity.GetAttributeValue<DateTime>("tss_estimationclosedate").Date;
                            QueryExpression queryOverdue = new QueryExpression("tss_sparepartsetup")
                            {
                                ColumnSet = new ColumnSet(true),
                                Criteria =
                                {
                                    Conditions =
                                        {
                                            new ConditionExpression("tss_notifoverdue", ConditionOperator.NotNull),
                                        }
                                }
                            };
                            Entity setup = _orgService.RetrieveMultiple(queryOverdue).Entities[0];
                            OptionSetValue period = setup.GetAttributeValue<OptionSetValue>("tss_period");
                            int notifoverdue = setup.GetAttributeValue<int>("tss_notifoverdue");
                            string periodTxt = OptionSetExtractor.GetOptionSetText(_orgService, "tss_sparepartsetup", "tss_period", period.Value);
                            if (periodTxt == "Week")
                            {
                                int days = notifoverdue * 7;
                                if (today > estclosedate.AddDays(days))
                                {
                                    SendEmailNotif(entity.Id, _orgService);
                                    Console.WriteLine("Notification Email sent!");
                                }
                                else
                                    Console.WriteLine("No notification");
                            }
                            else
                            {
                                if (today > estclosedate.AddMonths(notifoverdue))
                                {
                                    SendEmailNotif(entity.Id, _orgService);
                                    Console.WriteLine("Notification Email sent!");
                                }
                                else
                                    Console.WriteLine("No notification");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Notification Email sent!");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    //Console.WriteLine(entity.Id.ToString() + " " + entity.GetAttributeValue<EntityReference>("tss_currency").Id.ToString() + " " + entity.GetAttributeValue<EntityReference>("tss_currency").Name);
                }            
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
        public static void SendEmailNotif(Guid prospectheaderID, IOrganizationService organizationService)
        {
            try
            {
                Entity prospectheader = organizationService.Retrieve("tss_prospectpartheader", prospectheaderID, new ColumnSet(true));
                //Guid owner = prospectheader.GetAttributeValue<EntityReference>("owninguser").Id;
                EntityCollection admins = GetFromAdminCRM(organizationService);
                Guid crmadmin = admins.Entities[0].Id;
                    //new Guid("1CBA90DC-4DE9-E111-9AA2-544249894792");

                QueryExpression queryReceiver = new QueryExpression("tss_sendnotifoverdueprospect")
                {
                    ColumnSet = new ColumnSet(true),
                    Criteria =
                    {
                        Conditions =
                                    {
                                        new ConditionExpression("tss_user", ConditionOperator.NotNull),
                                    }
                    }
                };

                EntityCollection receivers = organizationService.RetrieveMultiple(queryReceiver);
                Guid[] Receiver = new Guid[receivers.Entities.Count];
                Console.WriteLine("Finished set Email Receipient");
                int x = 0;
                foreach (var receiver in receivers.Entities)
                {
                    Receiver[x] = receiver.GetAttributeValue<EntityReference>("tss_user").Id;
                    x++;
                }
                string topic = prospectheader.GetAttributeValue<string>("tss_name");
                string desc = prospectheader.GetAttributeValue<string>("tss_description");

                //woEntity.GetAttributeValue<EntityReference>("owninguser").Id;
                var emailGuid = CreateEmailNotif(crmadmin, Receiver, crmadmin,
                    organizationService, topic, desc);
                Console.WriteLine("sending email...");

                //var emailAgent = new EmailAgent();
                //emailAgent.Send(organizationService, emailGuid);
            }
            catch (Exception ex)
            {
                //Process here for needed (Tracing or whatever.)
                Console.WriteLine("Failed to Send Email. Technical Details : \r\n" + ex.ToString());
            }
        }

        private static Guid CreateEmailNotif(Guid senderGuid, Guid[] receiverGuids, Guid ccGuid, IOrganizationService organizationService, string Topic, string Description)
        {
            try
            {
                Guid email = Guid.Empty;

                var subject = @"Notification Prospect Overdue";
                string row = string.Empty;
                var bodyTemplate = @"Dear All,<br/>
                                Please check to prospect with details as below, <br/>Topic: " + Topic;
                bodyTemplate += ". <br/>Description: " + Description;
                bodyTemplate += @". <br/><br/>The prospect is already past overdue, please respond immidiately!<br/><br/>
                                Thank you,<br/>
                                Admin CRM";

                var emailAgent = new EmailAgent();

                emailAgent.AddSender(senderGuid);
                foreach (var receive in receiverGuids)
                {
                    emailAgent.AddReceiver("systemuser", receive);
                }
                emailAgent.AddCC("systemuser", ccGuid); //bermasalah owning usernya
                emailAgent.subject = subject;
                emailAgent.description = bodyTemplate;
                emailAgent.priority = EmailAgent.Priority_High;
                Console.WriteLine("generating email...");
                emailAgent.Create(organizationService, out email);

                return email;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private static EntityCollection GetFromAdminCRM(IOrganizationService organizationService)
        {
            DataLayer.DL_systemuser adm = new DataLayer.DL_systemuser();

            QueryExpression queryExpression = new QueryExpression(adm.EntityName);
            queryExpression.ColumnSet.AddColumns("systemuserid", "domainname");
            queryExpression.Criteria.AddCondition("domainname", ConditionOperator.Equal, "TRAKNUS\\admin.crm");

            return adm.Select(organizationService, queryExpression);
        }
    }
}
