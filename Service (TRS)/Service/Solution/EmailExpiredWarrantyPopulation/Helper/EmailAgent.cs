using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;

using SendEmail.DataLayer;

namespace SendEmail.Helper
{
    public class EmailAgent
    {
        #region Constants
        private const string _classname = "EmailAgent";
        #endregion

        #region Dependencies
        private DL_activityparty _DL_activityparty = new DL_activityparty();
        private DL_email _DL_email = new DL_email();
        DL_systemuser _DL_systemuser = new DL_systemuser();
        #endregion

        private EntityCollection _senderList = new EntityCollection();
        private EntityCollection _receiverList = new EntityCollection();

        private string _subject;
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
                _DL_email.Insert(organizationService, out emailId);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Create : " + ex.Message.ToString());
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
