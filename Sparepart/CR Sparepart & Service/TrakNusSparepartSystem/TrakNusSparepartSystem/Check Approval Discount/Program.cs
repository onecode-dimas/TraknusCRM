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

namespace Check_Approval_Discount
{
    class Program
    {
        #region constant
        private static int StatusReason_WaitingApprovalDiscount = 865920002;
        #endregion
        static void Main(string[] args)
        {
            string _connectionString = string.Empty;
            if (IsValidConnectionString(ConfigurationManager.ConnectionStrings["CRMCS"].ConnectionString))
                _connectionString = ConfigurationManager.ConnectionStrings["CRMCS"].ConnectionString;
            else
                _connectionString = "Url=http://10.0.10.43/traktornusantara; Domain=traknus; Username=admin.crm; Password=pass@word2;";

            string CRM_URL = _connectionString.Split(';')[0].Remove(0, 4);
            
            Console.WriteLine("Console Check Approval Discount");
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
                        new ConditionExpression("tss_statusreason", ConditionOperator.Equal, StatusReason_WaitingApprovalDiscount)
                    }
                };
                EntityCollection QP_Headers = _orgService.RetrieveMultiple(queryGetQP_Header);

                Console.WriteLine("Loading Quotation Part Header Records..");
                foreach (var entity in QP_Headers.Entities)
                {
                    ////Console.WriteLine("Quotation header with Id: " + entity.Id.ToString());
                    //if (entity.Contains("tss_quotationnumber"))
                    //{
                    //    string quo_Number = entity.GetAttributeValue<string>("tss_quotationnumber");
                    //    Console.WriteLine("Current Quotation Number : " + quo_Number);
                    //    //QUO.1801-03.A00000163-DTR
                    //}
                    try
                    {
                        if (entity.Contains("tss_quotationnumber") && entity.Contains("tss_pss") && entity.Contains("tss_discountdatetime") && entity.Contains("tss_discountcurrentapprover"))
                        {
                            string quo_Number = entity.GetAttributeValue<string>("tss_quotationnumber");
                            Console.WriteLine("Current Quotation Number : " + quo_Number);
                            DL_systemuser _DL_systemuser = new DL_systemuser();
                            Entity pss = _DL_systemuser.Select(_orgService, entity.GetAttributeValue<EntityReference>("tss_pss").Id);
                            EntityReference branch = pss.GetAttributeValue<EntityReference>("businessunitid");

                            Console.WriteLine(branch.Name);
                            QueryExpression queryGetQP_Lines = new QueryExpression("tss_quotationpartlines")
                            {
                                ColumnSet = new ColumnSet(true),
                                Criteria =
                                {
                                    Conditions =
                                    {
                                        new ConditionExpression("tss_quotationpartheader", ConditionOperator.Equal, entity.Id),
                                        new ConditionExpression("tss_discountpercent", ConditionOperator.GreaterThan, Convert.ToDecimal(0))
                                    }
                                },
                                Orders =
                                {
                                    new OrderExpression("tss_discountpercent", OrderType.Descending)
                                }
                            };
                            EntityCollection QP_Lines = _orgService.RetrieveMultiple(queryGetQP_Lines);

                            if (QP_Lines.Entities.Count > 0)
                            {
                                Entity QP_Line = QP_Lines.Entities.First();
                                DateTime currentdatetime = DateTime.Now;
                                DateTime discountdatetime = entity.GetAttributeValue<DateTime>("tss_discountdatetime").ToLocalTime();
                                EntityReference currentApproval = entity.GetAttributeValue<EntityReference>("tss_discountcurrentapprover");

                                decimal percentageindecimal = QP_Line.GetAttributeValue<decimal>("tss_discountpercent");
                                int percentage = Convert.ToInt32(Math.Round(percentageindecimal, 0, MidpointRounding.AwayFromZero));

                                Console.WriteLine("Decimal before round: " + percentageindecimal.ToString());
                                Console.WriteLine("Decimal after round: " + percentage.ToString());

                                QueryExpression queryMatrixapprovaldiscount = new QueryExpression("tss_matrixapprovaldiscount")
                                {
                                    ColumnSet = new ColumnSet(true),
                                    Criteria =
                                    {
                                        Conditions =
                                        {
                                            new ConditionExpression("tss_branch", ConditionOperator.Equal, branch.Id),
                                            new ConditionExpression("tss_endpercentage", ConditionOperator.GreaterEqual, percentage),
                                            new ConditionExpression("tss_startpercentage", ConditionOperator.LessThan, percentage)
                                        }
                                    },
                                    Orders =
                                    {
                                        new OrderExpression("tss_endpercentage", OrderType.Descending)
                                    }
                                };

                                EntityCollection MatrixApprovals = _orgService.RetrieveMultiple(queryMatrixapprovaldiscount);
                                Entity lastMatrixApproval = MatrixApprovals.Entities.First();
                                List<Entity> currMatrixApprovals = MatrixApprovals.Entities.Where(o => o.Id == currentApproval.Id).ToList();

                                int maxEndPercentage = lastMatrixApproval.GetAttributeValue<int>("tss_endpercentage");
                                int currEscalateHour = currMatrixApprovals.First().GetAttributeValue<int>("tss_escalatehour");
                                int currEndPercentage = currMatrixApprovals.First().GetAttributeValue<int>("tss_endpercentage");
                                int diff = (currentdatetime - discountdatetime).Hours;
                                Console.WriteLine("Different in hour: " + diff);

                                if (currEndPercentage != maxEndPercentage)
                                {
                                    var nextApprovers = MatrixApprovals.Entities.Where(o => o.GetAttributeValue<int>("tss_escalatehour") <= diff && o.GetAttributeValue<int>("tss_startpercentage") == currEndPercentage);
                                    if (nextApprovers != null)
                                    {
                                        List<Guid> nextapproversGUID = new List<Guid>();
                                        foreach(var nextApprover in nextApprovers)
                                        {
                                            EntityReference nextApproval = nextApprover.GetAttributeValue<EntityReference>("tss_approver");
                                            nextapproversGUID.Add(nextApproval.Id);
                                        }
                                        SendEmailNotif(entity.Id, nextapproversGUID.ToArray(), _orgService, CRM_URL);
                                        Console.WriteLine("Notification sent!");

                                        entity["tss_discountcurrentapprover"] = nextApprovers.First().ToEntityReference();
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
                                Console.WriteLine("No record quotation part lines found!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Field quotation number / branch / discount date time / discount current approver in quotation part header is null!");
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

        public static void SendEmailNotif(Guid quotationID, Guid[] receivers, IOrganizationService organizationService, string url)
        {
            try
            {
                Entity quotationheader = organizationService.Retrieve("tss_quotationpartheader", quotationID, new ColumnSet(true));

                EntityCollection admins = GetFromAdminCRM(organizationService);
                Guid crmadmin = admins.Entities[0].Id;
                //new Guid("1CBA90DC-4DE9-E111-9AA2-544249894792");
                EntityReference ccRef = quotationheader.GetAttributeValue<EntityReference>("ownerid");
                string quotationNumber = quotationheader.GetAttributeValue<string>("tss_quotationnumber");

                var emailGuid = CreateEmailNotif(crmadmin, receivers, ccRef.Id,
                    organizationService, quotationNumber, quotationheader.Id.ToString(), url);

                Console.WriteLine("sending email...");
            }
            catch (Exception ex)
            {
                //Process here for needed (Tracing or whatever.)
                Console.WriteLine("Failed to Send Email. Technical Details : \r\n" + ex.ToString());
            }
        }

        private static Guid CreateEmailNotif(Guid senderGuid, Guid[] receiverGuids, Guid ccGuid, IOrganizationService organizationService, string QuotationNumber, string recordID, string url)
        {
            try
            {
                Guid email = Guid.Empty;
                HelperFunction help = new HelperFunction();

                string objecttypecode = string.Empty;
                string fullNames = string.Empty;
                string targetCustomer = string.Empty;
                string targetUnit = string.Empty;
                string createdby = string.Empty;

                help.GetObjectTypeCode(organizationService, "tss_quotationpartheader", out objecttypecode);

                var context = new OrganizationServiceContext(organizationService);
                var quot = (from c in context.CreateQuery("tss_quotationpartheader")
                            where c.GetAttributeValue<Guid>("tss_quotationpartheaderid") == new Guid(recordID)
                            select c).ToList();
                //get receiver fullname
                for (int i = 0; i < receiverGuids.Count(); i++)
                {
                    var user = (from c in context.CreateQuery("systemuser")
                                where c.GetAttributeValue<Guid>("systemuserid") == receiverGuids[i]
                                select c).ToList();
                    if (user.Count > 0)
                    {
                        if (user[0].Attributes.Contains("fullname"))
                        {
                            if (i > 0) fullNames += " / ";
                            fullNames += user[0].GetAttributeValue<string>("fullname");
                        }
                    }
                }
                if (quot.Count > 0)
                {
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
                }

                var subject = @"Waiting Approval Discount on Quotation Part with Quotation Number " + QuotationNumber;
                string row = string.Empty;

                var bodyTemplate = "Dear " + fullNames + ",<br/><br/>";
                bodyTemplate += "Need Your Approval Discount for Quotation below:<br/><br/>";
                if (targetCustomer != string.Empty) bodyTemplate += "Target customer : " + targetCustomer + "<br/>";
                if (targetUnit != string.Empty) bodyTemplate += "Target unit : " + targetUnit + "<br/>";
                bodyTemplate += "Quotation : " + "<a href='" + url + "/main.aspx?etc=" + objecttypecode + "&pagetype=entityrecord&id=%7b" + recordID + "%7d'>Click here</a><br/><br/>";
                bodyTemplate += "Thanks,<br/><br/>";
                bodyTemplate += "Regards,<br/><br/>";
                bodyTemplate += createdby;

//                var bodyTemplate = @"Dear All,<br/><br/>
//                                Please approve discount request in Quotation Part";
//                bodyTemplate += @".<br/><br/>CRM URL : <a href='" + url + "/main.aspx?etc=" + objecttypecode + "&id=%7b" + recordID + "%7d&pagetype=entityrecord'>Click here</a>";
//                bodyTemplate += @".<br/><br/>
//                                Thank you,<br/><br/>
//                                Admin CRM";

                var emailAgent = new EmailAgent();

                emailAgent.AddSender(senderGuid);
                foreach (Guid receiverGuid in receiverGuids)
                {
                    emailAgent.AddReceiver("systemuser", receiverGuid);
                }
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
