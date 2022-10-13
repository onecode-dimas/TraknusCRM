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
using TrakNusSparepartSystem.Helper;

namespace CheckApprovalSalesEffort
{
    class Program
    {
        static void Main(string[] args)
        {
            string _connectionString = string.Empty;
            if (IsValidConnectionString(ConfigurationManager.ConnectionStrings["CRMCS"].ConnectionString))
            {
                _connectionString = ConfigurationManager.ConnectionStrings["CRMCS"].ConnectionString;
            }
            else
            {
                _connectionString = "Url=http://10.0.10.43/traktornusantara; Domain=traknus; Username=admin.crm; Password=pass@word2;";
            }

            string CRM_URL = _connectionString.Split(';')[0].Remove(0, 4);

            Console.WriteLine("Console Check Approval Sales Effort");
            Console.WriteLine("====================================");
            Console.WriteLine("Getting crmserviceclient..");
            CrmServiceClient conn = new CrmServiceClient(_connectionString);

            try
            {
                Console.WriteLine("Getting crmorganizationservice..");
                IOrganizationService _orgService = (IOrganizationService)conn.OrganizationServiceProxy;

                var context = new OrganizationServiceContext(_orgService);
                var sopartheader = (from c in context.CreateQuery("tss_sopartheader")
                                    where c.GetAttributeValue<bool>("tss_sendemailsaleseffort") == true
                                    where c.GetAttributeValue<OptionSetValue>("tss_statussaleseffort").Value == 865920000
                                    select c).ToList();
                
                Console.WriteLine("Loading SO Part Header Records..");
                for (int i = 0; i < sopartheader.Count(); i++)
                {
                    Console.WriteLine("Current SO Number : " + sopartheader[i].GetAttributeValue<string>("tss_sonumber"));
                    if (sopartheader[i].GetAttributeValue<DateTime>("tss_saleseffortdatetime").ToLocalTime() < DateTime.Now)
                    {
                        //get current approval matrix
                        var currentMatrix = (from c in context.CreateQuery("tss_matrixapprovalsaleseffort")
                                             where c.GetAttributeValue<Guid>("tss_matrixapprovalsaleseffortid") == sopartheader[i].GetAttributeValue<EntityReference>("tss_saleseffortcurrentapprover").Id
                                             select c).ToList();

                        ////if reach last approval continue
                        //if (currentMatrix[0].GetAttributeValue<int>("tss_priorityno") == 1)
                        //{
                        //    Console.WriteLine("Has reached the last approval!");
                        //    continue;
                        //}

                        //get next approval matrix
                        var nextMatrix = (from c in context.CreateQuery("tss_matrixapprovalsaleseffort")
                                          where c.GetAttributeValue<EntityReference>("tss_branch").Id == currentMatrix[0].GetAttributeValue<EntityReference>("tss_branch").Id
                                          where c.GetAttributeValue<int>("tss_priorityno") > currentMatrix[0].GetAttributeValue<int>("tss_priorityno")
                                          select c).ToList();

                        //get highest priority approver
                        if (nextMatrix.Count > 0)
                        {
                            Guid currentMatrixApprover = Guid.Empty;
                            EntityReference currentApprover = new EntityReference();
                            int escalateHour = 0;
                            string useremail = string.Empty;
                            int min = int.MaxValue;
                            for (int j = 0; j < nextMatrix.Count; j++)
                            {
                                if (nextMatrix[j].GetAttributeValue<int>("tss_priorityno") < min)
                                {
                                    min = nextMatrix[j].GetAttributeValue<int>("tss_priorityno");
                                    currentMatrixApprover = nextMatrix[j].GetAttributeValue<Guid>("tss_matrixapprovalsaleseffortid");
                                    currentApprover = nextMatrix[j].GetAttributeValue<EntityReference>("tss_approver");
                                    escalateHour = nextMatrix[j].GetAttributeValue<int>("tss_escalatehour");
                                }
                            }

                            var user = (from c in context.CreateQuery("systemuser")
                                        where c.GetAttributeValue<Guid>("systemuserid") == currentApprover.Id
                                        select c).ToList();

                            if (user.Count > 0)
                            {
                                useremail = user[0].GetAttributeValue<string>("internalemailaddress");
                            }

                            //send email
                            Entity email = new Entity("email");
                            Entity emailTo = new Entity("activityparty");
                            Entity emailFrom = new Entity("activityparty");

                            emailTo.Attributes["addressused"] = useremail;
                            emailFrom.Attributes["partyid"] = new EntityReference("systemuser", sopartheader[i].GetAttributeValue<EntityReference>("ownerid").Id);
                            email.Attributes["to"] = new Entity[] { emailTo };
                            email.Attributes["from"] = new Entity[] { emailFrom };

                            email.Attributes["subject"] = "Test Submit Sales Effort";
                            email.Attributes["description"] = "";
                            email.Attributes["trs_autosend"] = true;

                            Guid emailCreated = _orgService.Create(email);
                            EmailAgent emailagent = new EmailAgent();
                            emailagent.Send(_orgService, emailCreated);
                            Console.WriteLine("Email sent!");

                            Entity updatesopartheader = new Entity("tss_sopartheader");
                            updatesopartheader.Id = sopartheader[i].GetAttributeValue<Guid>("tss_sopartheaderid");
                            updatesopartheader.Attributes["tss_saleseffortcurrentapprover"] = new EntityReference("tss_matrixapprovalsaleseffort", currentMatrixApprover);
                            updatesopartheader.Attributes["tss_saleseffortdatetime"] = sopartheader[i].GetAttributeValue<DateTime>("tss_saleseffortdatetime").AddHours(escalateHour);
                            _orgService.Update(updatesopartheader);
                            Console.WriteLine("Update current approval!");
                        }
                        else
                        {
                            Console.WriteLine("Next approval not found! Email will be sent later!");
                        }
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
    }
}
