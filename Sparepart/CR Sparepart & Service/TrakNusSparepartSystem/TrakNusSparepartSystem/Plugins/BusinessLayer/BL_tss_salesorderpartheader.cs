using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TrakNusSparepartSystem.DataLayer;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Crm.Sdk.Messages;
using TrakNusSparepartSystem.Helper;

namespace TrakNusSparepartSystem.Plugins.BusinessLayer
{
    public class BL_tss_salesorderpartheader
    {
        #region Constants
        private const string _classname = "BL_tss_salesorderpartheader";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_tss_salesorderpartheader _DL_tss_salesorderpartheader = new DL_tss_salesorderpartheader();
        private BL_trs_runningnumber _BL_trs_runningnumber = new BL_trs_runningnumber();
        private DL_tss_approverlist _DL_tss_approverlist = new DL_tss_approverlist();
        private ShareRecords _ShareRecords = new ShareRecords();
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        private DL_email _DL_email = new DL_email();
        #endregion

        #region Forms Event
        public void Form_OnCreate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_tss_salesorderpartheader.EntityName)
                {
                    //checkPONumberPreCreate(organizationService, entity);

                    DateTime createdOn = DateTime.Now.ToLocalTime();
                    //string categoryCode = "03";
                    //Generate New Running Number
                    string newRunningNumber = _BL_trs_runningnumber.GenerateNewRunningNumberSalesOrder(
                        organizationService, pluginExceptionContext, _DL_tss_salesorderpartheader.EntityName, createdOn);
                    if (entity.Attributes.Contains("tss_sonumber"))
                        entity.Attributes["tss_sonumber"] = newRunningNumber;
                    else
                        entity.Attributes.Add("tss_sonumber", newRunningNumber);
                }
                else
                {
                    return;
                }

                
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PreOperation : " + ex.Message.ToString());
            }
        }

        #region Generate Running Number Of QuotationId
        public void Form_OnCreate_GenerateSOId_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext, ITracingService tracer)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_tss_salesorderpartheader.EntityName)
                {
                    DL_tss_runningnumberid number = new DL_tss_runningnumberid();
                    string newRunningIdString = number.newRunningIdString(organizationService, _DL_tss_salesorderpartheader.EntityName, entity.Id, tracer);
                    entity.Attributes["tss_soid"] = newRunningIdString;
                    organizationService.Update(entity);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_GenerateSOId_PostOperation : " + ex.Message.ToString());
            }
        }
        #endregion 

        public void checkPONumber(IOrganizationService organizationService, Entity now)
        {
            if (now.GetAttributeValue<string>("tss_ponumber") != null)
            {
                var context = new OrganizationServiceContext(organizationService);
                var checkponumber = (from c in context.CreateQuery(_DL_tss_salesorderpartheader.EntityName)
                                     where c.GetAttributeValue<string>("tss_ponumber") == now.GetAttributeValue<string>("tss_ponumber")
                                     select c).ToList();

                var ct = 0;
                string po = string.Empty;
                for (int i = 0; i < checkponumber.Count; i++)
                {
                    if (checkponumber[i].GetAttributeValue<Guid>("tss_sopartheaderid") != now.Id && checkponumber[i].GetAttributeValue<OptionSetValue>("tss_statecode").Value != 865920002)
                    {
                        ct++;
                        po = checkponumber[i].GetAttributeValue<string>("tss_sonumber");
                    }
                }

                if (ct > 0)
                {
                    throw new InvalidPluginExecutionException("PO Number already used on " + po + ". Please change PO Number.");
                }
            }
        }

        public void checkPONumberPreCreate(IOrganizationService organizationService, Entity now)
        {
            if (now.GetAttributeValue<string>("tss_ponumber") != null)
            {
                var context = new OrganizationServiceContext(organizationService);
                var checkponumber = (from c in context.CreateQuery(_DL_tss_salesorderpartheader.EntityName)
                                     where c.GetAttributeValue<string>("tss_ponumber") == now.GetAttributeValue<string>("tss_ponumber")
                                     select c).ToList();

                for (int i = 0; i < checkponumber.Count; i++)
                {
                    throw new InvalidPluginExecutionException("PO Number already used on " + checkponumber[i].GetAttributeValue<string>("tss_sonumber") + ". Please change PO Number.");
                }
            }
        }

        public void Form_OnUpdate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExecutionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExecutionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_tss_salesorderpartheader.EntityName)
                {
                    Entity now = _DL_tss_salesorderpartheader.Select(organizationService, entity.Id);

                    //if (now != null)
                    //{
                    //    checkPONumber(organizationService, now);
                    //}
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void Form_OnUpdate_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExecutionContext)
        {
            string trace = string.Empty;
            try
            {
                Entity entity = (Entity)pluginExecutionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_tss_salesorderpartheader.EntityName)
                {
                    Entity now = _DL_tss_salesorderpartheader.Select(organizationService, entity.Id);

                    if (now != null)
                    {
                        //checkPONumber(organizationService, now);

                        //send email
                        if (now.GetAttributeValue<bool>("tss_sendemailsaleseffort") && now.GetAttributeValue<string>("tss_sendemailsaleseffortmessage") == "send")
                        {
                            if(now.GetAttributeValue<EntityReference>("tss_branch") != null)
                            {
                                Guid currentMatrixApprover = Guid.Empty;
                                EntityReference currentApprover = new EntityReference();
                                int escalateHour = 0;
                                string useremail = string.Empty;

                                var context = new OrganizationServiceContext(organizationService);
                                var matrix = (from c in context.CreateQuery("tss_matrixapprovalsaleseffort")
                                              where c.GetAttributeValue<EntityReference>("tss_branch").Id == now.GetAttributeValue<EntityReference>("tss_branch").Id
                                              select c).ToList();

                                //get highest priority approver
                                int min = int.MaxValue;
                                for (int i = 0; i < matrix.Count; i++)
                                {
                                    //insert approver to approver list
                                    Entity entApproverList = new Entity("tss_approverlist");
                                    entApproverList.Attributes["tss_salesorderpartheaderid"] = new EntityReference("tss_sopartheader", entity.Id);
                                    entApproverList.Attributes["tss_approver"] = new EntityReference("systemuser", matrix[i].GetAttributeValue<EntityReference>("tss_approver").Id);
                                    entApproverList.Attributes["tss_type"] = new OptionSetValue(865920001);
                                    organizationService.Create(entApproverList);

                                    //_DL_tss_approverlist.tss_sopart = entity.Id;
                                    //_DL_tss_approverlist.tss_approver = matrix[i].GetAttributeValue<EntityReference>("tss_approver").Id;
                                    //_DL_tss_approverlist.CreateApprover(organizationService);

                                    //share record to approver
                                    _ShareRecords.ShareRecord(organizationService, _DL_tss_salesorderpartheader.Select(organizationService, entity.Id), _DL_systemuser.Select(organizationService, matrix[i].GetAttributeValue<EntityReference>("tss_approver").Id));

                                    //share record  to approver for so lines, so attachment, sales effort



                                    //check lowest approver
                                    if (matrix[i].GetAttributeValue<int>("tss_priorityno") < min)
                                    {
                                        min = matrix[i].GetAttributeValue<int>("tss_priorityno");
                                        currentMatrixApprover = matrix[i].GetAttributeValue<Guid>("tss_matrixapprovalsaleseffortid");
                                        currentApprover = matrix[i].GetAttributeValue<EntityReference>("tss_approver");
                                        escalateHour = matrix[i].GetAttributeValue<int>("tss_escalatehour");
                                    }
                                }

                                Entity user = _DL_systemuser.Select(organizationService, currentApprover.Id);
                                if (user != null)
                                {
                                    useremail = user.GetAttributeValue<string>("internalemailaddress");
                                }

                                //create email
                                //EntityCollection emailFromCollection = new EntityCollection();
                                //EntityCollection emailToCollection = new EntityCollection();
                                //Entity emailTo = new Entity("activityparty");
                                //Entity emailFrom = new Entity("activityparty");
                                //Guid emailCreated = Guid.Empty;

                                //emailTo.Attributes["addressused"] = useremail;
                                //emailFrom.Attributes["partyid"] = new EntityReference("systemuser", pluginExecutionContext.InitiatingUserId);
                                //emailFromCollection.Entities.Add(emailFrom);
                                //emailToCollection.Entities.Add(emailTo);

                                //_DL_email.subject = "Test Submit Sales Effort";
                                //_DL_email.description = "Testing";
                                //_DL_email.from = emailFromCollection;
                                //_DL_email.to = emailToCollection;
                                //_DL_email.Insert(organizationService, out emailCreated);

                                Guid email = Guid.Empty;
                                var emailAgent = new EmailAgent();
                                emailAgent.AddSender(pluginExecutionContext.InitiatingUserId);
                                emailAgent.AddReceiver("systemuser", user.Id);
                                emailAgent.subject = "Test Submit Sales Effort";
                                emailAgent.description = "Testing";
                                emailAgent.priority = EmailAgent.Priority_Normal;
                                emailAgent.trs_autosend = true;//set false dulu, jadi draft.
                                emailAgent.Create(organizationService, out email);
                                emailAgent.Send(organizationService, email);

                                //send email
                                //SendEmailRequest req = new SendEmailRequest();
                                //req.EmailId = emailCreated;
                                //req.TrackingToken = "";
                                //req.IssueSend = true;
                                //SendEmailResponse res = (SendEmailResponse)organizationService.Execute(req);

                                //update sales effort datetime
                                Entity sopartheader = new Entity(_DL_tss_salesorderpartheader.EntityName);
                                sopartheader.Id = entity.Id;
                                sopartheader.Attributes["tss_saleseffortcurrentapprover"] = new EntityReference("tss_matrixapprovalsaleseffort", currentMatrixApprover);
                                sopartheader.Attributes["tss_saleseffortdatetime"] = now.GetAttributeValue<DateTime>("tss_saleseffortdatetime").AddHours(escalateHour);
                                sopartheader.Attributes["tss_sendemailsaleseffortmessage"] = string.Empty;
                                organizationService.Update(sopartheader);
                            }
                        }

                        

                        //Approve all sales effort
                        if (now.GetAttributeValue<bool>("tss_approveallsaleseffort"))
                        {
                            var context = new OrganizationServiceContext(organizationService);
                            var saleseffortlist = (from c in context.CreateQuery("tss_sopartsaleseffort")
                                                   where c.GetAttributeValue<EntityReference>("tss_sopartheaderid").Id == entity.Id
                                                   select c).ToList();

                            for (int i = 0; i < saleseffortlist.Count; i++)
                            {
                                if (saleseffortlist[i].GetAttributeValue<bool>("tss_approvalstatus") == false)
                                {
                                    Entity ent = new Entity("tss_sopartsaleseffort");
                                    ent.Id = saleseffortlist[i].GetAttributeValue<Guid>("tss_sopartsaleseffortid");
                                    ent.Attributes["tss_approvalstatus"] = true;
                                    ent.Attributes["tss_approveby"] = new EntityReference("systemuser", pluginExecutionContext.InitiatingUserId);
                                    organizationService.Update(ent);
                                }
                            }

                            //remove approval list and share read only
                            var approverList = (from c in context.CreateQuery("tss_approverlist")
                                                where c.GetAttributeValue<EntityReference>("tss_salesorderpartheaderid").Id == entity.Id
                                                where c.GetAttributeValue<OptionSetValue>("tss_type").Value == 865920001
                                                select c).ToList();
                            foreach (var x in approverList)
                            {
                                _ShareRecords.UnShareRecord(organizationService, _DL_tss_salesorderpartheader.Select(organizationService, entity.Id), _DL_systemuser.Select(organizationService, x.GetAttributeValue<EntityReference>("tss_approver").Id));
                                _ShareRecords.ShareRecordReadOnly(organizationService, _DL_tss_salesorderpartheader.Select(organizationService, entity.Id), _DL_systemuser.Select(organizationService, x.GetAttributeValue<EntityReference>("tss_approver").Id));
                                organizationService.Delete("tss_approverlist", x.Id);
                            }
                        }

                        if (now.Contains("tss_approvecreditlimitby") && now.Attributes["tss_approvecreditlimitby"] != null)
                        {
                            //remove approval list and share read only
                            var context = new OrganizationServiceContext(organizationService);
                            var approverList = (from c in context.CreateQuery("tss_approverlist")
                                                where c.GetAttributeValue<EntityReference>("tss_salesorderpartheaderid").Id == entity.Id
                                                where c.GetAttributeValue<OptionSetValue>("tss_type").Value == 865920000
                                                select c).ToList();
                            foreach (var x in approverList)
                            {
                                _ShareRecords.UnShareRecord(organizationService, _DL_tss_salesorderpartheader.Select(organizationService, entity.Id), _DL_systemuser.Select(organizationService, x.GetAttributeValue<EntityReference>("tss_approver").Id));
                                _ShareRecords.ShareRecordReadOnly(organizationService, _DL_tss_salesorderpartheader.Select(organizationService, entity.Id), _DL_systemuser.Select(organizationService, x.GetAttributeValue<EntityReference>("tss_approver").Id));
                                organizationService.Delete("tss_approverlist", x.Id);
                            }
                        }

                        if (true)
                        {
                            var context = new OrganizationServiceContext(organizationService);
                            var approverList = (from c in context.CreateQuery("tss_approverlist")
                                                where c.GetAttributeValue<EntityReference>("tss_salesorderpartheaderid").Id == entity.Id
                                                where c.GetAttributeValue<OptionSetValue>("tss_type").Value == 865920001
                                                select c).ToList();
                            foreach (var x in approverList)
                            {
                                 var saleseffortlist = (from c in context.CreateQuery("tss_sopartsaleseffort")
                                                   where c.GetAttributeValue<EntityReference>("tss_sopartheaderid").Id == entity.Id
                                                   select c).ToList();

                                 foreach (var s in saleseffortlist)
                                 {
                                     _ShareRecords.ShareRecordReadOnly(organizationService, s, _DL_systemuser.Select(organizationService, x.GetAttributeValue<EntityReference>("tss_approver").Id));
                                 }

                                var soLines = (from c in context.CreateQuery("tss_sopartlines")
                                               where c.GetAttributeValue<EntityReference>("tss_sopartheaderid").Id == entity.Id
                                               select c).ToList();
                                foreach (var e in soLines)
                                {
                                    _ShareRecords.ShareRecordReadOnly(organizationService, e, _DL_systemuser.Select(organizationService, x.GetAttributeValue<EntityReference>("tss_approver").Id));
                                }

                                var attachments = (from c in context.CreateQuery("tss_attachment")
                                                   where c.GetAttributeValue<EntityReference>("tss_salesorderpartheader").Id == entity.Id
                                                   select c).ToList();
                                foreach (var a in attachments)
                                {
                                    _ShareRecords.ShareRecordReadOnly(organizationService, a, _DL_systemuser.Select(organizationService, x.GetAttributeValue<EntityReference>("tss_approver").Id));
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate_PostOperation : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}
