/****************************************************************************************************
 * Created By   : Thomas Williem Effendi
 * Created Date : 8 October 2014
 * Description  : Class for creating and sending email in CRM
 * Compatibility: CRM 2011, CRM 2013
 *****************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;

using TrakNusRapidService.DataLayer;

namespace TrakNusRapidService.Helper
{
    public class EmailAgent
    {
        #region Constants
        private const string _classname = "EmailAgent";

        public const int Priority_High = 2;
        public const int Priority_Normal = 1;
        public const int Priority_Low = 0;
        #endregion

        #region Dependencies
        private DL_activityparty _DL_activityparty = new DL_activityparty();
        private DL_email _DL_email = new DL_email();
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        #endregion

        private int _priority = Priority_Normal;
        public int priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        private string _subject = null;
        public string subject
        {
            get { return _subject; }
            set { _subject = value; }
        }

        private string _description = null;
        public string description
        {
            get { return _description; }
            set { _description = value; }
        }

        private EntityCollection _senderList = new EntityCollection();
        public void AddSender(Guid senderId)
        {
            try
            {
                _DL_activityparty = new DL_activityparty();
                _DL_activityparty.partyid = new EntityReference(_DL_systemuser.EntityName, senderId);
                _senderList.Entities.Add(_DL_activityparty.GetEntity());
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".AddSender : " + ex.Message.ToString());
            }
        }

        private EntityCollection _receiverList = new EntityCollection();
        public void AddReceiver(string entityLogicalName, Guid receiverId)
        {
            try
            {
                _DL_activityparty = new DL_activityparty();
                _DL_activityparty.partyid = new EntityReference(entityLogicalName, receiverId);
                _receiverList.Entities.Add(_DL_activityparty.GetEntity());
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".AddReceiver : " + ex.Message.ToString());
            }
        }

        public void Create(IOrganizationService organizationService, out Guid emailId)
        {
            try
            {
                _DL_email = new DL_email();
                _DL_email.from = _senderList;
                _DL_email.to = _receiverList;
                _DL_email.subject = _subject;
                _DL_email.description = _description;
                _DL_email.prioritycode = _priority;
                _DL_email.Insert(organizationService, out emailId);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Create : " + ex.Message.ToString());
            }
        }

        public void Update(IOrganizationService organizationService, Guid emailId)
        {
            try
            {
                _DL_email = new DL_email();
                if (_senderList.Entities.Count > 0)
                    _DL_email.from = _senderList;
                if (_receiverList.Entities.Count > 0)
                    _DL_email.to = _receiverList;
                if (_subject != null)
                    _DL_email.subject = _subject;
                if (_description != null)
                    _DL_email.description = _description;
                _DL_email.Update(organizationService, emailId);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }

        public void Send(IOrganizationService organizationService, Guid emailId)
        {
            try
            {
                SendEmailRequest sendEmailRequest = new SendEmailRequest
                {
                    EmailId = emailId,
                    TrackingToken = "",
                    IssueSend = true
                };

                organizationService.Execute(sendEmailRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Send : " + ex.Message.ToString());
            }
        }
    }
}
