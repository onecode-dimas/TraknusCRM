using EnhancementCRM.HelperUnit.Data_Layer;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhancementCRM.HelperUnit
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
        public EntityCollection SenderList
        {
            get { return _senderList; }
        }

        private bool _trs_autosend = false;
        public bool trs_autosend
        {
            get { return _trs_autosend; }
            set { _trs_autosend = value; }
        }

        private decimal _trs_autosendcategory = 0;
        public decimal trs_autosendcategory
        {
            get { return _trs_autosendcategory; }
            set { _trs_autosendcategory = value; }
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

        private EntityCollection _receiverList = new EntityCollection();
        public EntityCollection ReceiverList
        {
            get { return _receiverList; }
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

        private EntityCollection _ccList = new EntityCollection();
        public EntityCollection CCList
        {
            get { return _ccList; }
        }
        public void AddCC(string entityLogicalName, Guid ccId)
        {
            try
            {
                _DL_activityparty = new DL_activityparty();
                _DL_activityparty.partyid = new EntityReference(entityLogicalName, ccId);
                _ccList.Entities.Add(_DL_activityparty.GetEntity());
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".AddCC : " + ex.Message.ToString());
            }
        }

        public void Create(IOrganizationService organizationService, out Guid emailId)
        {
            try
            {
                emailId = Guid.Empty;
                _DL_email = new DL_email();
                _DL_email.from = _senderList;
                _DL_email.to = _receiverList;
                if (_ccList.Entities.Count > 0)
                    _DL_email.cc = _ccList;
                _DL_email.subject = _subject;
                _DL_email.description = _description;
                _DL_email.prioritycode = _priority;
                _DL_email.trs_autosend = _trs_autosend;

                _DL_email.trs_autosendcategory = _trs_autosendcategory;
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
                if (_ccList.Entities.Count > 0)
                    _DL_email.cc = _ccList;
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

        public void SendNotification(Guid senderGuid, Guid receiverGuid, Guid ccGuid, IOrganizationService organizationService, string subject, string bodyTemplate)
        {
            try
            {
                var emailAgent = new EmailAgent();

                Guid email = Guid.Empty;
                emailAgent.AddSender(senderGuid);
                emailAgent.AddReceiver("systemuser", receiverGuid);
                //emailAgent.AddCC("systemuser", ccGuid); //bermasalah owning usernya
                emailAgent.subject = subject;
                emailAgent.description = bodyTemplate;
                emailAgent.priority = EmailAgent.Priority_High;
                emailAgent.trs_autosend = true;//set false dulu, jadi draft.
                emailAgent.Create(organizationService, out email);

                emailAgent.Send(organizationService, email);
            }
            catch (Exception ex)
            {
                throw new Exception("Send Email Failed. Technical Details :\r\n" + ex.ToString());
            }
        }
    }
}
