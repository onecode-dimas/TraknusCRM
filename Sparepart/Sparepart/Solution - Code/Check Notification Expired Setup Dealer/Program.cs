using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrakNusSparepartSystem.DataLayer;


namespace Check_Notification_Expired_Setup_Dealer
{
    class Program
    {
        public enum Period
        {
            Week = 865920000,
            Month = 865920001
        }

        static void Main(string[] args)
        {
            string _connectionString = string.Empty;
            if (IsValidConnectionString(ConfigurationManager.ConnectionStrings["CRMCS"].ConnectionString))
                _connectionString = ConfigurationManager.ConnectionStrings["CRMCS"].ConnectionString;
            else
                _connectionString = "Url=http://10.0.10.43/traktornusantara; Domain=traknus; Username=admin.crm; Password=pass@word2;";
            Console.WriteLine("Console Notification Expired Setup Dealer");
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
                query.EntityName = "tss_dealerheader";
                query.ColumnSet = new ColumnSet(true);
                query.AddOrder("tss_enddate", OrderType.Ascending);
                Console.WriteLine("Loading Setup Delaer Records..");
                DataCollection<Entity> entities = _orgService.RetrieveMultiple(query).Entities;
                
                //cek notification period
                QueryExpression qe = new QueryExpression
                {
                    EntityName = "tss_notificationperiod",
                    ColumnSet = new ColumnSet(true),
                    Criteria =
                    {
                        Filters =
                        {
                            new FilterExpression(){
                                Conditions = {
                                    new ConditionExpression("tss_periodfor",ConditionOperator.Equal,865920000)  //865,920,000 -> dealer
                                }
                            }
                        },
                    }
                };

                DataCollection<Entity> notifPeriodEntities = _orgService.RetrieveMultiple(qe).Entities;


                foreach (var entity in entities)
                {
                    string dealerName = "";
                    if (entity.Contains("tss_dealername"))
                    {
                        dealerName = entity.GetAttributeValue<EntityReference>("tss_dealername").Name;
                    }
                    Console.WriteLine();
                    Console.WriteLine("Setup Delaer " + dealerName + " with Id: " 
                        + entity.Id.ToString());
                    try
                    {
                        //only record containt start date and end date  
                        if (entity.Contains("tss_startdate") && entity.Contains("tss_enddate"))
                        {
                            DateTime today = DateTime.Today.Date.ToUniversalTime();
                            DateTime addFromToday = new DateTime();
                            
                          
                            DateTime estEndDate = entity.GetAttributeValue<DateTime>("tss_enddate");

                            foreach (var entityPeriod in notifPeriodEntities)
                            {
                                String periodName = "";
                                int period = entityPeriod.GetAttributeValue<OptionSetValue>("tss_period").Value;
                                int numberPeriod = entityPeriod.GetAttributeValue<int>("tss_notificationevery");

                                if (period == Period.Month.GetHashCode())
                                {
                                    addFromToday = today.AddMonths(numberPeriod);
                                    periodName = "bulan";
                                }
                                else if (period == Period.Week.GetHashCode())
                                {
                                    addFromToday = today.AddDays(7*(numberPeriod));
                                    periodName = "minggu";
                                }

                                if(addFromToday == estEndDate){
                                    //send email
                                    SendEmailNotif(entity.Id, _orgService);
                                    Console.WriteLine("Notification Email sent! Dealer Name : " + dealerName +" \nNotification Period : "+numberPeriod+" "+periodName);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message, ex.InnerException);
                    }
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

        public static void SendEmailNotif(Guid dealerHeaderID, IOrganizationService organizationService)
        {
            try
            {
                Entity dealerHeader = organizationService.Retrieve("tss_dealerheader", dealerHeaderID, new ColumnSet(true));
                //Guid owner = prospectheader.GetAttributeValue<EntityReference>("owninguser").Id;
                EntityCollection admins = GetFromAdminCRM(organizationService);
                Guid crmadmin = admins.Entities[0].Id;
            

                Guid tssManagerId = Guid.NewGuid();
                Guid pssId = Guid.NewGuid();

                /*
                // query for CC
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
                */


                Guid[] ReceiverCC = new Guid[0];
                Guid receiverId =  dealerHeader.GetAttributeValue<EntityReference>("ownerid").Id;
                Console.WriteLine("Finished set Email Receipient");
              
              
                string topic = "TOPIC TEMPLATE";
                string desc = "DESC TEMPLATE";

               
                var emailGuid = CreateEmailNotif(crmadmin, receiverId, ReceiverCC, crmadmin,
                    organizationService, topic, desc);
                Console.WriteLine("sending email...");
             
            }
            catch (Exception ex)
            {
                //Process here for needed (Tracing or whatever.)
                Console.WriteLine("Failed to Send Email. Technical Details : \r\n" + ex.ToString());
            }
        }

        private static Guid CreateEmailNotif(Guid senderGuid, Guid receiverGuid, Guid[] receiverCCGuids, Guid ccGuid, IOrganizationService organizationService, string Topic, string Description)
        {
            try
            {
                Guid email = Guid.Empty;

                var subject = @"Notification Expired Setup Dealer";
                string row = string.Empty;
                var bodyTemplate = @"Dear All,<br/>
                                Please check to dealer with details as below, <br/>Topic: " + Topic;
                bodyTemplate += ". <br/>Description: " + Description;
                bodyTemplate += @". <br/><br/>The dealer is already past overdue, please respond immidiately!<br/><br/>
                                Thank you,<br/>
                                Admin CRM";

                var emailAgent = new EmailAgent();

                emailAgent.AddSender(senderGuid);
                emailAgent.AddReceiver("systemuser", receiverGuid);
                foreach (var receiverCC in receiverCCGuids)
                {
                    emailAgent.AddCC("systemuser", receiverCC); 
                }

                emailAgent.subject = subject;
                emailAgent.description = bodyTemplate;
                emailAgent.priority = EmailAgent.Priority_High;
                emailAgent.trs_autosend = true;
                Console.WriteLine("generating email...");
                emailAgent.Create(organizationService, out email);
                emailAgent.Send(organizationService, email);

                return email;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
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
