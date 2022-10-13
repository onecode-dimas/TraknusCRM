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

namespace Check_Approval_Package
{
    class Program
    {
        #region constant
        private static int StatusReason_WaitingApprovalPackage = 865920003;
        #endregion
        static void Main(string[] args)
        {
            string _connectionString = string.Empty;
            if (IsValidConnectionString(ConfigurationManager.ConnectionStrings["CRMCS"].ConnectionString))
                _connectionString = ConfigurationManager.ConnectionStrings["CRMCS"].ConnectionString;
            else
                _connectionString = "Url=http://10.0.10.43/traktornusantara; Domain=traknus; Username=admin.crm; Password=pass@word2;";

            string CRM_URL = _connectionString.Split(';')[0].Remove(0, 4);
            
            Console.WriteLine("Console Check Approval Package");
            Console.WriteLine("====================================");
            Console.WriteLine("Getting crmserviceclient..");
            CrmServiceClient conn = new CrmServiceClient(_connectionString);
            try
            {
                Console.WriteLine("Getting crmorganizationservice..");
                IOrganizationService _orgService = (IOrganizationService)conn.OrganizationServiceProxy;

                QueryExpression queryGetQP_Header = new QueryExpression("tss_quotationpartheader");
                queryGetQP_Header.ColumnSet = new ColumnSet(true);
                queryGetQP_Header.Criteria = new FilterExpression
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                    {
                        new ConditionExpression("tss_statusreason", ConditionOperator.Equal, StatusReason_WaitingApprovalPackage)
                    }
                };
                EntityCollection QP_Headers = _orgService.RetrieveMultiple(queryGetQP_Header);

                Console.WriteLine("Loading Quotation Part Header Records..");
                foreach (var entity in QP_Headers.Entities)
                {
                    try
                    {
                        if (entity.Contains("tss_pss") && entity.Contains("tss_quotationnumber"))
                        {
                            string quo_Number = entity.GetAttributeValue<string>("tss_quotationnumber");
                            Console.WriteLine("Current Quotation Number : " + quo_Number);
                            DL_systemuser _DL_systemuser = new DL_systemuser();
                            Entity pss = _DL_systemuser.Select(_orgService, entity.GetAttributeValue<EntityReference>("tss_pss").Id);
                            EntityReference branch = pss.GetAttributeValue<EntityReference>("businessunitid");

                            Console.WriteLine(branch.Name);
                            QueryExpression queryGetQP_Lines = new QueryExpression("tss_quotationpartlines");
                            queryGetQP_Lines.ColumnSet = new ColumnSet(true);
                            queryGetQP_Lines.Criteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.And,
                                Conditions =
                                {
                                    new ConditionExpression("tss_quotationpartheader", ConditionOperator.Equal, entity.Id),
                                    new ConditionExpression("tss_approvebypa", ConditionOperator.NotEqual, true),
                                    new ConditionExpression("tss_unitgroup", ConditionOperator.NotNull)
                                }
                            };
                            EntityCollection QP_Lines = _orgService.RetrieveMultiple(queryGetQP_Lines);

                            if (QP_Lines.Entities.Count > 0)
                            {
                                Entity[] arrLines = QP_Lines.Entities.ToArray();
                                EntityReference[] distRefs = arrLines.Select(o => o.GetAttributeValue<EntityReference>("tss_unitgroup")).GroupBy(o => o.Id).Select(o => o.First()).ToArray();
                                foreach (var distRef in distRefs)
                                {
                                    Entity[] LinesbyUnitGroup = arrLines.Where(o => o.GetAttributeValue<EntityReference>("tss_unitgroup").Id == distRef.Id).ToArray();
                                    Entity LinebyUnitGroup = LinesbyUnitGroup.First();
                                    DateTime currentdatetime = DateTime.Now;
                                    DateTime packagedatetime = LinebyUnitGroup.GetAttributeValue<DateTime>("tss_packagedatetime").ToLocalTime();
                                    EntityReference currentApproval = LinebyUnitGroup.GetAttributeValue<EntityReference>("tss_packagecurrentapprover");

                                    QueryExpression queryMatrixapprovalpackage = new QueryExpression("tss_matrixapprovalpackage")
                                    {
                                        ColumnSet = new ColumnSet(true),
                                        Criteria =
                                        {
                                            Conditions =
                                            {
                                                new ConditionExpression("tss_branch", ConditionOperator.Equal, branch.Id),
                                                new ConditionExpression("tss_unitgroup", ConditionOperator.Equal, distRef.Id)
                                            }
                                        },
                                        Orders =
                                        {
                                           new OrderExpression("tss_priorityno", OrderType.Ascending)
                                        }
                                    };

                                    EntityCollection MatrixApprovals = _orgService.RetrieveMultiple(queryMatrixapprovalpackage);
                                    Entity currMatrixApproval = MatrixApprovals.Entities.Where(o => o.Id == currentApproval.Id).First();

                                    int currEscalateHour = currMatrixApproval.GetAttributeValue<int>("tss_escalatehour");
                                    int currPriorty = currMatrixApproval.GetAttributeValue<int>("tss_priorityno");
                                    int diff = (currentdatetime - packagedatetime).Hours;
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

                                            foreach (var updateline in LinesbyUnitGroup)
                                            {
                                                updateline["tss_packagecurrentapprover"] = nextApprover.ToEntityReference();
                                                _orgService.Update(updateline);
                                            }
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
                            }
                            else
                            {
                                Console.WriteLine("No record quotation part lines found!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Field branch / quoation number in quotation part header is null");
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
                Entity quotationheader = organizationService.Retrieve("tss_quotationpartheader", quotationID, new ColumnSet(true));

                EntityCollection admins = GetFromAdminCRM(organizationService);
                Guid crmadmin = admins.Entities[0].Id;
                //new Guid("1CBA90DC-4DE9-E111-9AA2-544249894792");
                EntityReference ccRef = quotationheader.GetAttributeValue<EntityReference>("ownerid");
                string quotationNumber = quotationheader.GetAttributeValue<string>("tss_quotationnumber");

                var emailGuid = CreateEmailNotif(crmadmin, receiver, ccRef.Id,
                    organizationService, quotationNumber, quotationheader.Id.ToString(), url);

                Console.WriteLine("sending email...");
            }
            catch (Exception ex)
            {
                //Process here for needed (Tracing or whatever.)
                Console.WriteLine("Failed to Send Email. Technical Details : \r\n" + ex.ToString());
            }
        }

        private static Guid CreateEmailNotif(Guid senderGuid, Guid receiverGuid, Guid ccGuid, IOrganizationService organizationService, string QuotationNumber, string recordID, string url)
        {
            try
            {
                Guid email = Guid.Empty;
                DL_systemuser _DL_user = new DL_systemuser();
                HelperFunction help = new HelperFunction();

                Entity receiverRecord = _DL_user.Select(organizationService, receiverGuid);
                string Fullname = receiverRecord.GetAttributeValue<string>("fullname");
                string objecttypecode = string.Empty;
                string programName = string.Empty;
                string targetCustomer = string.Empty;
                string targetUnit = string.Empty;
                string createdby = string.Empty;
                decimal? totalExpectedPackageAmount = null;
                

                help.GetObjectTypeCode(organizationService, "tss_quotationpartheader", out objecttypecode);

                var context = new OrganizationServiceContext(organizationService);
                var quot = (from c in context.CreateQuery("tss_quotationpartheader")
                            where c.GetAttributeValue<Guid>("tss_quotationpartheaderid") == new Guid(recordID)
                            select c).ToList();
                if (quot.Count > 0)
                {
                    if (quot[0].Attributes.Contains("tss_packagesname"))
                    {
                        programName = quot[0].GetAttributeValue<string>("tss_packagesname");
                    }
                    if (quot[0].Attributes.Contains("tss_customer"))
                    {
                        targetCustomer = quot[0].GetAttributeValue<EntityReference>("tss_customer").Name;
                    }
                    if (quot[0].Attributes.Contains("tss_unit"))
                    {
                        targetUnit = quot[0].GetAttributeValue<EntityReference>("tss_unit").Name;
                    }
                    if (quot[0].Attributes.Contains("createdby"))
                    {
                        createdby = quot[0].GetAttributeValue<EntityReference>("createdby").Name;
                    }
                    if (quot[0].Attributes.Contains("tss_totalexpectedpackageamount"))
                    {
                        totalExpectedPackageAmount = quot[0].GetAttributeValue<Money>("tss_totalexpectedpackageamount").Value;
                    }
                }

                var subject = @"Waiting Approval Convert to Package on Quotation Part with Quotation Number " + QuotationNumber;
                string row = string.Empty;

                var bodyTemplate = "Dear Mr/Ms " + Fullname + ",<br/><br/>";
                bodyTemplate += "Need Your Approval Convert to Package for Quotation below:<br/><br/>";
                if (!String.IsNullOrEmpty(QuotationNumber)) bodyTemplate += "Quotation Number : " + QuotationNumber + "<br/>";
                if (programName != string.Empty) bodyTemplate += "Program Name : " + programName + "<br/>";
                if (targetCustomer != string.Empty) bodyTemplate += "Target customer : " + targetCustomer + "<br/>";
                if (targetUnit != string.Empty) bodyTemplate += "Target unit : " + targetUnit + "<br/>";
                if (totalExpectedPackageAmount != null)
                    bodyTemplate += "Total Expected Package Amount : " + totalExpectedPackageAmount + "<br />";

                bodyTemplate += "Quotation : " + "<a href='" + url + "/main.aspx?etc=" + objecttypecode + "&pagetype=entityrecord&id=%7b" + recordID + "%7d'>Click here</a><br/><br/>";
                bodyTemplate += "Thanks,<br/><br/>";
                bodyTemplate += "Regards,<br/><br/>";
                bodyTemplate += createdby;


//                var bodyTemplate = @"Dear Mr/Ms " + Fullname + @",<br/><br/>
//                                Please approve convert to package request in Quotation Part";
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

        /*
        public static void SendEmailNotif(Guid quotationID, Guid receiver, IOrganizationService organizationService, Entity[] details)
        {
            try
            {
                Entity quotationheader = organizationService.Retrieve("tss_quotationpartheader", quotationID, new ColumnSet(true));

                EntityCollection admins = GetFromAdminCRM(organizationService);
                Guid crmadmin = admins.Entities[0].Id;
                //new Guid("1CBA90DC-4DE9-E111-9AA2-544249894792");

                string quotationId = quotationheader.GetAttributeValue<string>("tss_quotationnumber");

                var emailGuid = CreateEmailNotif(crmadmin, receiver, crmadmin,
                    organizationService, quotationId, details);

                Console.WriteLine("sending email...");
            }
            catch (Exception ex)
            {
                //Process here for needed (Tracing or whatever.)
                Console.WriteLine("Failed to Send Email. Technical Details : \r\n" + ex.ToString());
            }
        }

        private static Guid CreateEmailNotif(Guid senderGuid, Guid receiverGuid, Guid ccGuid, IOrganizationService organizationService, string quotationId, Entity[] details)
        {
            try
            {
                Guid email = Guid.Empty;
                DL_systemuser _DL_user = new DL_systemuser();

                Entity receiverRecord = _DL_user.Select(organizationService, receiverGuid);
                string Fullname = receiverRecord.GetAttributeValue<string>("fullname");

                var subject = @"Waiting Approval Convert to Package";
                string row = string.Empty;
                var bodyTemplate = @"Dear Mr/Ms " + Fullname + @",<br/>
                                Please approve the convert to package request in Quotation Part Lines at Quotation Part with Quotation Id: " + quotationId;
                bodyTemplate += @".<br/>Please click the following link: ";
                foreach(var detail in details)
                {
                    string recordID = detail.Id.ToString();
                    string partnum = detail.GetAttributeValue<EntityReference>("tss_partnumber").Name;
                    row += "<br/><a href='http://10.0.10.43/TraktorNusantara/main.aspx?etc=10182&id=%7b" + recordID + "%7d&pagetype=entityrecord'>" + partnum + "<a>";
                }
                bodyTemplate += row;
                bodyTemplate += @"<br/><br/><br/>
                                Thank you,<br/>
                                Admin CRM";

                var emailAgent = new EmailAgent();
                
                emailAgent.AddSender(senderGuid);
                emailAgent.AddReceiver("systemuser", receiverGuid);
                emailAgent.AddCC("systemuser", ccGuid); //bermasalah owning usernya
                emailAgent.subject = subject;
                emailAgent.description = bodyTemplate;
                emailAgent.priority = EmailAgent.Priority_High;
                emailAgent.trs_autosend = false;//set false dulu, jadi draft.
                Console.WriteLine("generating email...");
                emailAgent.Create(organizationService, out email);
                
                return email;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        */

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
