using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrakNusSparepartSystem.DataLayer;
using TrakNusSparepartSystem.Helper;

namespace Check_Approval_Credit_Limit
{
    class Program
    {
        static void Main(string[] args)
        {
            string _connectionString = string.Empty;
            if (IsValidConnectionString(ConfigurationManager.ConnectionStrings["CRMCS"].ConnectionString))
                _connectionString = ConfigurationManager.ConnectionStrings["CRMCS"].ConnectionString;
            else
                _connectionString = "Url=http://10.0.10.43/traktornusantara; Domain=traknus; Username=admin.crm; Password=pass@word2;";

            string CRM_URL = _connectionString.Split(';')[0].Remove(0, 4);

            Console.WriteLine("Console Check Approval Credit Limit");
            Console.WriteLine("====================================");
            Console.WriteLine("Getting crmserviceclient..");
            CrmServiceClient conn = new CrmServiceClient(_connectionString);
            try
            {
                Console.WriteLine("Getting crmorganizationservice..");
                IOrganizationService _orgService = (IOrganizationService)conn.OrganizationServiceProxy;

                QueryExpression querySO_Header = new QueryExpression("tss_sopartheader");
                querySO_Header.ColumnSet = new ColumnSet(true);
                querySO_Header.Criteria = new FilterExpression
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                    {
                        new ConditionExpression("tss_approvecreditlimit", ConditionOperator.Equal, false),
                        new ConditionExpression("tss_clcurrentapprover", ConditionOperator.NotNull)
                    }
                };
                EntityCollection SO_Headers = _orgService.RetrieveMultiple(querySO_Header);

                Console.WriteLine("Loading SO Part Records..");
                foreach (var entity in SO_Headers.Entities)
                {
                    try
                    {
                        if (entity.Contains("tss_sonumber") && entity.Contains("tss_clcurrentapprover") && entity.Contains("tss_cldatetime") && entity.Contains("tss_pss"))
                        {
                            string SO_Number = entity.GetAttributeValue<string>("tss_sonumber");
                            Console.WriteLine("Current SO Number : " + SO_Number);
                            DL_systemuser _DL_systemuser = new DL_systemuser();
                            DL_account _DL_account = new DL_account();
                            Entity pss = _DL_systemuser.Select(_orgService, entity.GetAttributeValue<EntityReference>("tss_pss").Id);
                            EntityReference branch = pss.GetAttributeValue<EntityReference>("businessunitid");

                            Entity customer = _DL_account.Select(_orgService, entity.GetAttributeValue<EntityReference>("tss_customer").Id);
                            decimal amount = entity.GetAttributeValue<Money>("tss_totalamount").Value;
                            string balance_ar = customer.GetAttributeValue<string>("tss_balancearsparepart");
                            decimal balance = decimal.Parse(balance_ar);
                            decimal limit = amount - balance;
                            
                            DateTime currentdatetime = DateTime.Now;
                            DateTime cldatetime = entity.GetAttributeValue<DateTime>("tss_cldatetime").ToLocalTime();
                            EntityReference currentApproval = entity.GetAttributeValue<EntityReference>("tss_clcurrentapprover");

                            QueryExpression queryMatrixapprovalCL = new QueryExpression("tss_matrixapprovalcreditlimit")
                            {
                                ColumnSet = new ColumnSet(true),
                                Criteria =
                                {
                                    Conditions =
                                    {
                                        new ConditionExpression("tss_branch", ConditionOperator.Equal, branch.Id),
                                        new ConditionExpression("tss_end", ConditionOperator.GreaterEqual, Convert.ToInt32(limit)),
                                        new ConditionExpression("tss_start", ConditionOperator.LessEqual, Convert.ToInt32(limit))
                                    }
                                },
                                Orders =
                                {
                                   new OrderExpression("tss_priorityno", OrderType.Ascending)
                                }
                            };

                            EntityCollection MatrixApprovals = _orgService.RetrieveMultiple(queryMatrixapprovalCL);
                            Entity currMatrixApproval = MatrixApprovals.Entities.Where(o => o.Id == currentApproval.Id).First();

                            int currEscalateHour = currMatrixApproval.GetAttributeValue<int>("tss_escalatehour");
                            int currPriorty = currMatrixApproval.GetAttributeValue<int>("tss_priorityno");
                            int diff = (currentdatetime - cldatetime).Hours;
                            Console.WriteLine("Different in hour: " + diff);
                            if (currPriorty != MatrixApprovals.Entities.Count)
                            {
                                int nextPriority = currPriorty + 1;

                                Entity nextApprover = MatrixApprovals.Entities.Where(o => o.GetAttributeValue<int>("tss_escalatehour") <= diff && o.GetAttributeValue<int>("tss_priorityno") == nextPriority).SingleOrDefault();
                                if (nextApprover != null)
                                {
                                    //Entity nextApprover = MatrixApprovals.Entities.Where(o => o.GetAttributeValue<int>("tss_priorityno") == currPriorty).Single();
                                    EntityReference nextApproval = nextApprover.GetAttributeValue<EntityReference>("tss_approver");
                                    SendEmailNotif(entity.Id, nextApproval.Id, _orgService, CRM_URL);
                                    Console.WriteLine("Notification sent!");

                                    entity["tss_clcurrentapprover"] = nextApprover.ToEntityReference();
                                    _orgService.Update(entity);
                                    Console.WriteLine("Update current approval!");
                                }
                                else
                                {
                                    Console.WriteLine("Next approval not found! Notification will be sent later!");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Notification has reached the last approval!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Field SO number / CL current approver / CL date / PSS branch is null");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
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

        public static void SendEmailNotif(Guid quotationID, Guid receiver, IOrganizationService organizationService, string url)
        {
            try
            {
                Entity SOheader = organizationService.Retrieve("tss_sopartheader", quotationID, new ColumnSet(true));

                EntityCollection admins = GetFromAdminCRM(organizationService);
                Guid crmadmin = admins.Entities[0].Id;
                //new Guid("1CBA90DC-4DE9-E111-9AA2-544249894792");
                EntityReference ccRef = SOheader.GetAttributeValue<EntityReference>("ownerid");
                string SONumber = SOheader.GetAttributeValue<string>("tss_sonumber");

                var emailGuid = CreateEmailNotif(crmadmin, receiver, ccRef.Id,
                    organizationService, SONumber, SOheader.Id.ToString(), url);

                Console.WriteLine("sending email...");
            }
            catch (Exception ex)
            {
                //Process here for needed (Tracing or whatever.)
                Console.WriteLine("Failed to Send Email. Technical Details : \r\n" + ex.ToString());
            }
        }

        private static Guid CreateEmailNotif(Guid senderGuid, Guid receiverGuid, Guid ccGuid, IOrganizationService organizationService, string SONumber, string recordID, string url)
        {
            try
            {
                Guid email = Guid.Empty;
                DL_systemuser _DL_user = new DL_systemuser();
                HelperFunction help = new HelperFunction();

                Entity receiverRecord = _DL_user.Select(organizationService, receiverGuid);
                string Fullname = receiverRecord.GetAttributeValue<string>("fullname");
                string objecttypecode = string.Empty;
                string targetCustomer = string.Empty;
                string createdby = string.Empty;

                help.GetObjectTypeCode(organizationService, "tss_sopartheader", out objecttypecode);

                var context = new OrganizationServiceContext(organizationService);
                var so = (from c in context.CreateQuery("tss_quotationpartheader")
                            where c.GetAttributeValue<Guid>("tss_quotationpartheaderid") == new Guid(recordID)
                            select c).ToList();
                if (so.Count > 0)
                {
                    if (so[0].Attributes.Contains("tss_customer"))
                    {
                        targetCustomer = so[0].GetAttributeValue<EntityReference>("tss_customer").Name;
                    }
                    if (so[0].Attributes.Contains("createdby"))
                    {
                        createdby = so[0].GetAttributeValue<EntityReference>("createdby").Name;
                    }
                }


                var subject = @"Waiting Approval Credit Limit on SO Part with SO Number " + SONumber;
                string row = string.Empty;

                var bodyTemplate = "Dear Mr/Ms " + Fullname + ",<br/><br/>";
                bodyTemplate += "Need Your Approval Credit Limit for Sales Order below:<br/><br/>";
                if (targetCustomer != string.Empty) bodyTemplate += "Target customer : " + targetCustomer + "<br/>";
                bodyTemplate += "Sales Order : " + "<a href='" + url + "/main.aspx?etc=" + objecttypecode + "&pagetype=entityrecord&id=%7b" + recordID + "%7d'>Click here</a><br/><br/>";
                bodyTemplate += "Thanks,<br/><br/>";
                bodyTemplate += "Regards,<br/><br/>";
                bodyTemplate += createdby;
                
                
//                var bodyTemplate = @"Dear Mr/Ms " + Fullname + @",<br/><br/>
//                                Please approve credit limit request in SO Part";
//                bodyTemplate += @".<br/><br/>CRM URL : <a href='" + url + "/main.aspx?etc=" + objecttypecode + "&id=%7b" + recordID + "%7d&pagetype=entityrecord'>Click here</a>";
//                bodyTemplate += @".<br/><br/>
//                                Thank you,<br/><br/>
//                                Admin CRM";

                var emailAgent = new EmailAgent();

                emailAgent.AddSender(senderGuid);
                emailAgent.AddReceiver("systemuser", receiverGuid);
                emailAgent.AddCC("systemuser", ccGuid); //bermasalah owning usernya
                emailAgent.subject = subject;
                emailAgent.description = bodyTemplate;
                emailAgent.priority = EmailAgent.Priority_High;
                emailAgent.trs_autosend = true;//set false dulu, jadi draft.
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
