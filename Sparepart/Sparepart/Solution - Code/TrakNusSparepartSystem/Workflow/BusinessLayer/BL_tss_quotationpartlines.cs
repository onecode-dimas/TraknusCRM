using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TrakNusSparepartSystem.DataLayer;
using TrakNusSparepartSystem.Workflow.Helper;

namespace TrakNusSparepartSystem.Workflow.BusinessLayer
{
    class BL_tss_quotationpartlines
    {
        #region Constants
        private const string _classname = "BL_tss_quotationpartlines";
        private const int QUOTATIONPARTSTATUSCODE_APPROVED = 865920002;
        private const int QUOTATIONPARTSTATUSREASON_APPROVED = 865920004;
        #endregion

        #region Depedencies
        private ShareRecords _ShareRecords = new ShareRecords();
        private DL_tss_approverlist _DL_tss_approverlist = new DL_tss_approverlist();
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        #endregion

        #region Events
        public void ApprovePackage_OnClick(IOrganizationService organizationService, ITracingService tracingService, Guid id, Guid user)
        {
            try
            {
                Entity Quopartline = organizationService.Retrieve("tss_quotationpartlines", id, new ColumnSet(true));
                bool isnotapproved = false;
                bool allapproved = true;

                //int linesnotapproved = 0;
                tracingService.Trace("Checking approve by pa field");
                if (Quopartline.Attributes.Contains("tss_approvebypa"))
                {
                    isnotapproved = Quopartline.GetAttributeValue<bool>("tss_approvebypa");
                }

                //Validasi Final Approve
                if (!isnotapproved)
                {
                    String strPathAndQuery = HttpContext.Current.Request.Url.PathAndQuery;
                    string CRM_URL = HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");
                    CRM_URL += "TraktorNusantara";

                    Quopartline["tss_approvebypa"] = true;
                    Quopartline["tss_approvefinalpriceby"] = new EntityReference("systemuser", user);
                    organizationService.Update(Quopartline);
                    tracingService.Trace("Finish update quotation part lines");

                    EntityReference refparent = Quopartline.GetAttributeValue<EntityReference>("tss_quotationpartheader");
                    EntityReference refUG = Quopartline.GetAttributeValue<EntityReference>("tss_unitgroup");
                    Entity header = organizationService.Retrieve("tss_quotationpartheader", refparent.Id, new ColumnSet(true));

                    QueryExpression querygetLines = new QueryExpression("tss_quotationpartlines");
                    querygetLines.ColumnSet = new ColumnSet(true);
                    querygetLines.Criteria.AddCondition("tss_quotationpartheader", ConditionOperator.Equal, refparent.Id);

                    EntityCollection lines = organizationService.RetrieveMultiple(querygetLines);
                    tracingService.Trace("Checking all quotation part lines");
                    List<Entity> linesExceptThis = lines.Entities.Where(o => o.Id != Quopartline.Id).ToList();
                    foreach (Entity line in linesExceptThis)
                    {
                        bool approvedbypa = false;
                        if (line.Contains("tss_approvebypa"))
                        {
                            approvedbypa = line.GetAttributeValue<bool>("tss_approvebypa");
                        }
                        if (!approvedbypa)
                        {
                            allapproved = false;
                        }
                    }

                    if (allapproved)
                    {
                        tracingService.Trace("Updating Header approve package");
                        header["tss_approvepackage"] = true;
                        header["tss_statuscode"] = new OptionSetValue(QUOTATIONPARTSTATUSCODE_APPROVED);
                        header["tss_statusreason"] = new OptionSetValue(QUOTATIONPARTSTATUSREASON_APPROVED);
                        organizationService.Update(header);
                        tracingService.Trace("Finish update Header approve package");

                        EntityReference PSS = header.GetAttributeValue<EntityReference>("tss_pss");
                        Guid receiver = PSS.Id;
                        tracingService.Trace("GUID PSS: " + receiver.ToString());
                        Entity PSSdetail = organizationService.Retrieve("systemuser", PSS.Id, new ColumnSet(true));
                        string receivername = PSSdetail.GetAttributeValue<string>("fullname");
                        tracingService.Trace("PSS Name: " + receivername);

                        Entity admin = GetSystemUserByFullname(organizationService, "Admin CRM");
                        Guid sender = admin.Id;
                        tracingService.Trace("GUID CRM admin: " + sender);

                        var email = CreateEmail(sender, receiver, user, header, lines, organizationService, tracingService, receivername, CRM_URL);
                        var emailGuid = organizationService.Create(email);
                        EmailFactory emailfactory = new EmailFactory();
                        emailfactory.SendEmail(organizationService, emailGuid);
                        //var emailAgent = new Helper.EmailAgent();
                        //emailAgent.SendEmail(organizationService, emailGuid);
                        tracingService.Trace("Finish Send Email");

                        //check approver list on lines
                        var context = new OrganizationServiceContext(organizationService);
                        var quotlines = (from c in context.CreateQuery("tss_quotationpartlines")
                                         where c.GetAttributeValue<EntityReference>("tss_quotationpartheader").Id == refparent.Id
                                         select c).ToList();

                        _DL_tss_approverlist = new DL_tss_approverlist();

                        foreach (var quotline in quotlines)
                        {
                            QueryExpression qeApproverList = new QueryExpression(_DL_tss_approverlist.EntityName)
                            {
                                ColumnSet = new ColumnSet(true),
                                Criteria = new FilterExpression()
                                {
                                    Conditions =
                                    {
                                        new ConditionExpression("tss_quotationpartlinesid",ConditionOperator.Equal,quotline.Id)
                                    }
                                }
                            };
                            EntityCollection ecApproverList = _DL_tss_approverlist.Select(organizationService, qeApproverList);

                            _ShareRecords = new ShareRecords();
                            foreach (var approverlist in ecApproverList.Entities)
                            {
                                //remove approve list
                                organizationService.Delete(_DL_tss_approverlist.EntityName, approverlist.Id);
                            }

                            _ShareRecords.UnShareAllRecords(organizationService, quotline, Quopartline.GetAttributeValue<EntityReference>("ownerid").Id);



                            //check approver list on header
                            QueryExpression qeApproverListHeader = new QueryExpression(_DL_tss_approverlist.EntityName)
                            {
                                ColumnSet = new ColumnSet(true),
                                Criteria = new FilterExpression()
                                {
                                    Conditions =
                                    {
                                        new ConditionExpression("tss_quotationpartheaderid",ConditionOperator.Equal,header.Id)
                                    }
                                }
                            };
                            EntityCollection ecApproverListHeader = _DL_tss_approverlist.Select(organizationService, qeApproverList);
                            foreach (var approverlist in ecApproverListHeader.Entities)
                            {
                                //remove approve list
                                organizationService.Delete(_DL_tss_approverlist.EntityName, approverlist.Id);
                            }

                        }
                        _ShareRecords.UnShareAllRecords(organizationService, header, Quopartline.GetAttributeValue<EntityReference>("ownerid").Id);
                    }
                    else
                    {
                        tracingService.Trace("Lines not all approved.");
                    }
                }
                else
                {
                    throw new Exception("Approve Package Can't update approve by pa field");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(_classname + ".ApprovePackage_OnClick : " + ex.Message.ToString());
            }
        }

        private Entity GetSystemUserByFullname(IOrganizationService organizationService, string fullName)
        {
            try
            {
                var systemUserQuery = new QueryExpression("systemuser")
                {
                    ColumnSet = new ColumnSet(true),
                    Criteria =
                    {
                        Conditions =
                        {
                            new ConditionExpression("fullname", ConditionOperator.Equal, fullName)
                        }
                    }
                };
                var systemUserCollection = organizationService.RetrieveMultiple(systemUserQuery);
                if (systemUserCollection.Entities.Count > 0)
                {
                    return systemUserCollection.Entities.First();
                }
                else
                {
                    throw new Exception("System user with fullname " + fullName + " is not found.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured when getting system user by full name.\r\nTechnical Details: " + ex.ToString());
            }
        }

        private Entity CreateEmail(Guid senderGuid, Guid receiverGuid, Guid ccGuid, Entity configurationEntity, EntityCollection dataEntityCollection, IOrganizationService organizationService, ITracingService trace, string receiverName, string url)
        {
            try
            {
                trace.Trace("Start create email");
                HelperFunction help = new HelperFunction();
                string objecttypecode = string.Empty;
                string programName = string.Empty;
                string targetCustomer = string.Empty;
                string targetUnit = string.Empty;
                string createdby = string.Empty;
                decimal? finalPrice = null;

                help.GetObjectTypeCode(organizationService, "tss_quotationpartheader", out objecttypecode);

                var context = new OrganizationServiceContext(organizationService);
                var quot = (from c in context.CreateQuery("tss_quotationpartheader")
                            where c.GetAttributeValue<Guid>("tss_quotationpartheaderid") == configurationEntity.Id
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
                    if (quot[0].Attributes.Contains("tss_totalfinalprice"))
                    {
                        finalPrice = quot[0].GetAttributeValue<Money>("tss_totalfinalprice").Value;
                    }
                }

                var subject = @"Approve Package has been approved in Quotation Part with Qoutation Number " + configurationEntity.GetAttributeValue<string>("tss_quotationnumber");

                var bodyTemplate = @"Dear Mr/Ms " + receiverName + @",<br/><br/>
                                Your Quotation below already package approved<br/><br/>";
                if (!String.IsNullOrEmpty(configurationEntity.GetAttributeValue<string>("tss_quotationnumber"))) bodyTemplate += "Quotation Number : " + configurationEntity.GetAttributeValue<string>("tss_quotationnumber") + "<br/>";
                if (programName != string.Empty) bodyTemplate += "Program Name : " + programName + "<br/>";
                if (targetCustomer != string.Empty) bodyTemplate += "Target customer : " + targetCustomer + "<br/>";
                if (targetUnit != string.Empty) bodyTemplate += "Target unit : " + targetUnit + "<br/>";
                if (finalPrice != null)
                    bodyTemplate += "Total Expected Package Amount : " + finalPrice + "<br />";
                bodyTemplate += "Quotation : " + "<a href='" + url + "/main.aspx?etc=" + objecttypecode + "&pagetype=entityrecord&id=%7b" + configurationEntity.Id + "%7d'>Click here</a><br/><br/>";
                bodyTemplate += "Thanks,<br/><br/>";
                bodyTemplate += "Regards,<br/><br/>";
                bodyTemplate += createdby;

//                var bodyTemplate = @"Dear Mr/Ms " + receiverName + @",<br/><br/>
//                                The Final price has been approved at IDR " + configurationEntity.GetAttributeValue<Money>("tss_totalprice").Value.ToString() + @".<br/><br/>
//                                CRM URL : <a href='" + url + "/main.aspx?etc=" + objecttypecode + "&id=%7b" + configurationEntity.Id + "%7d&pagetype=entityrecord'>Click here</a>";
//                bodyTemplate += @"<br/><br/>
//                                Thank you,<br/>
//                                Admin CRM";

                trace.Trace("finish create body email");

                var emailAgent = new Helper.EmailAgent();
                var emailDescription = bodyTemplate;
                var emailFactory = new Helper.EmailFactory();

                emailFactory.SetFrom(emailAgent.CreateActivityParty(new List<Tuple<Guid, string>>
                {
                    new Tuple<Guid, string>(senderGuid,"systemuser")
                }));
                emailFactory.SetTo(emailAgent.CreateActivityParty(new List<Tuple<Guid, string>>
                {
                    new Tuple<Guid, string>(receiverGuid,"systemuser")
                }));
                emailFactory.SetCC(emailAgent.CreateActivityParty(new List<Tuple<Guid, string>>
                {
                    new Tuple<Guid, string>(ccGuid,"systemuser")
                }));
                emailFactory.SetSubject(subject);
                emailFactory.SetContent(emailDescription);
                trace.Trace("finish create email");

                return emailFactory.Create();
            }
            catch (Exception ex)
            {
                throw new Exception("Create Email Failed. Technical Details :\r\n" + ex.ToString());
            }
        }
        #endregion
    }
}
