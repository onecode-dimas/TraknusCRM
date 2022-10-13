using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;

namespace Check_Escalation_Approval_for_Market_Size_Result.Helper
{
    public static class EmailAgent
    {
        public static void SendEmailNotif(Guid senderGuid, Guid receiverGuid, Guid ccGuid, IOrganizationService organizationService, string subject, string bodyTemplate)
        {
            try
            {
                var emailAgent = new TrakNusSparepartSystem.Helper.EmailAgent();

                Guid email = Guid.Empty;
                emailAgent.AddSender(senderGuid);
                emailAgent.AddReceiver("systemuser", receiverGuid);
                //emailAgent.AddCC("systemuser", ccGuid); //bermasalah owning usernya
                emailAgent.subject = subject;
                emailAgent.description = bodyTemplate;
                emailAgent.priority = TrakNusSparepartSystem.Helper.EmailAgent.Priority_High;
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
