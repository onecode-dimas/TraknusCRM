using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SendEmail.Helper;
using SendEmail.DataLayer;
using SendEmail.Helper;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Client.Services;

namespace SendEmail.BusinessLayer
{
    class BL_new_population
    {
        #region Dependencies
        private Helper.LogCreator log = new Helper.LogCreator();
        private string _subject = string.Empty;
        private EmailAgent _EmailAgent = new EmailAgent();
        private EntityCollection _senderList = new EntityCollection();

        DL_account _DL_account = new DL_account();
        DL_contact _DL_contact = new DL_contact();
        DL_email email = new DL_email();
        DL_systemuser _DL_systemuser = new DL_systemuser();
        #endregion

        public void Execute(OrganizationService organizationService, Guid userId)
        {
            #region Variable
            string emailMessage = string.Empty;
            string count = string.Empty;
            string prodItem = string.Empty;
            string prodName = string.Empty;

            Guid FromId = new Guid();
            Guid accId = Guid.Empty;
            Guid contactID = Guid.Empty;
            #endregion

            Helper.EntityExtractor entityExtractor = new Helper.EntityExtractor();

            log.UserID = userId;
            log.Write("starting reminder process ...");

            EntityCollection userFrom = GetFromAdminCRM(organizationService);
            if (userFrom.Entities.Count > 0)
            {
                FromId = ((Guid)userFrom.Entities[0].Attributes["systemuserid"]);
            }

            EntityCollection expiredWarranty = GetNextThreeMonthExpired(organizationService);
            count = expiredWarranty.Entities.Count.ToString();

            foreach (Entity ePop in expiredWarranty.Entities)
            {
                try
                {
                    if (ePop.Attributes.Contains("new_productitem") && ePop.Attributes["new_productitem"] != null)
                    {
                        prodItem = ePop.Attributes["new_productitem"].ToString();
                    }

                    if (ePop.Attributes.Contains("new_productname") && ePop.Attributes["new_productname"] != null)
                    {
                        prodName = ePop.Attributes["new_productname"].ToString();
                    }

                    if (ePop.Attributes.Contains("new_customercode") && ePop.Attributes["new_customercode"] != null)
                    {
                        accId = ((EntityReference)ePop.Attributes["new_customercode"]).Id;
                    }

                    Entity acc = organizationService.Retrieve("account", accId, new ColumnSet(new string[] { "primarycontactid" }));

                    if (acc.Attributes.Contains("primarycontactid") && acc.Attributes["primarycontactid"] != null)
                    {
                        contactID = ((EntityReference)acc.Attributes["primarycontactid"]).Id;
                    }

                    Entity contact = organizationService.Retrieve("contact", contactID, new ColumnSet(new string[] { "emailaddress1", "fullname" }));                    
                    if (acc.Attributes.Contains("primarycontactid"))
                    {
                        emailMessage = "Next 3 Month for Product item : " + prodItem + " , Product name = " + prodName;
                        SendEmailForContact(organizationService, userId, contactID, emailMessage);
                        log.Write("email sent for reminder '" + ePop.Id.ToString() + "'");
                    }
                    else
                    {
                        emailMessage = "Dont have email address for contact name =  " + contact.Attributes["fullname"].ToString();
                        SendEmailForAdmin(organizationService, FromId, emailMessage);
                        log.Write("email sent for reminder '" + ePop.Id.ToString() + "'");
                    }
                    
                }
                catch (Exception ex)
                {
                    log.Write(ex.Message);
                }
            }

            log.Write("reminder process finished ...");
        }

        private EntityCollection GetNextThreeMonthExpired(OrganizationService organizationService)
        {
            DataLayer.DL_new_population pop = new DataLayer.DL_new_population();

            QueryExpression queryExpression = new QueryExpression(pop.EntityName);
            queryExpression.ColumnSet.AddColumns("trs_warrantyenddate", "trs_functionallocation", "new_customercode", "new_productitem", "new_productname");

            FilterExpression filterExpression = new FilterExpression(LogicalOperator.And);
            filterExpression.AddCondition("trs_warrantyenddate", ConditionOperator.NextXMonths, 1);
            
            queryExpression.Criteria.AddFilter(filterExpression);

            return pop.Select(organizationService, queryExpression);
            
        }

        private EntityCollection GetFromAdminCRM(OrganizationService organizationService)
        {
            DataLayer.DL_systemuser adm = new DataLayer.DL_systemuser();

            QueryExpression queryExpression = new QueryExpression(adm.EntityName);
            queryExpression.ColumnSet.AddColumns("systemuserid", "domainname");
            queryExpression.Criteria.AddCondition("domainname", ConditionOperator.Equal,"TRAKNUS\\admin.crm");

            return adm.Select(organizationService, queryExpression);
        }

        private void SendEmailForAdmin(OrganizationService organizationService, Guid fromUserId, string emailMessage)
        {
            BusinessLayer.BL_Email ble = new BusinessLayer.BL_Email();
            ble.AddSender(fromUserId);
            ble.description = emailMessage;
            ble.subject = _subject;

            DataLayer.DL_systemuser systemuser = new DataLayer.DL_systemuser();
            QueryExpression queryExpression = new QueryExpression(systemuser.EntityName);
            queryExpression.ColumnSet.AddColumns("systemuserid");

            FilterExpression filterExpression = new FilterExpression(LogicalOperator.And);
            filterExpression.AddCondition("name", ConditionOperator.Equal, "AdminTraknus");
            filterExpression.AddCondition("businessUnitidname", ConditionOperator.Equal, "Traknus");
            queryExpression.Criteria.AddFilter(filterExpression);

            EntityCollection entityCollection = systemuser.Select(organizationService, queryExpression);
            foreach (Entity entity in entityCollection.Entities)
            {
                ble.AddReceiver(systemuser.EntityName, ((Guid)entity.Attributes["systemuserid"]));
            }

            Guid emailId;
            ble.Create(organizationService, out emailId);
            log.Write("sending e-mail ...");
            ble.Send(organizationService, emailId);
        }

        private void SendEmailForContact(OrganizationService organizationService, Guid fromUserId, Guid toUserId, string emailMessage)
        {
            Guid emailId = Guid.Empty;

            EmailAgent _emailagent = new EmailAgent();
            _emailagent.AddSender(fromUserId);
            _emailagent.AddReceiver(_DL_contact.EntityName, toUserId);
            _emailagent.subject = emailMessage;
            _emailagent.description = emailMessage;
            _emailagent.Create(organizationService, out emailId);
        }
    }
}
