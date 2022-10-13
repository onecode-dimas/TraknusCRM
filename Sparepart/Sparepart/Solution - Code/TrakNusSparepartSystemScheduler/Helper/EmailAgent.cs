using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;

namespace TrakNusSparepartSystemScheduler.Helper
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

        public static string GetContentEmail(string manager, string requester, string urlCrm, EntityCollection entity)
        {

            StringBuilder sbContentEmail = new StringBuilder();
            if (string.IsNullOrEmpty(manager))
                sbContentEmail.Append(string.Format("Dear PSS/PDH/SM/GM"));
            else
                sbContentEmail.Append(string.Format("Dear {0}", manager));
            sbContentEmail.Append("</br>");
            sbContentEmail.Append("</br>");
            sbContentEmail.Append("</br>");
            sbContentEmail.Append("Need to review.");
            sbContentEmail.Append("</br>");
            sbContentEmail.Append("Please click link to open it below:");
            sbContentEmail.Append("</br>");
            foreach (Entity o in entity.Entities)
            {
                string url = string.Format("{0}/main.aspx?etn={1}&pagetype=entityrecord&id={2}", urlCrm, o.LogicalName, o.Id);
                sbContentEmail.Append(string.Format("<a href='{0}'>{1}</a></br>", url, o.GetAttributeValue<string>("tss_name")));
            }

            sbContentEmail.Append("</br>");
            sbContentEmail.Append("</br>");
            sbContentEmail.Append("Thanks and Regards,");
            sbContentEmail.Append("</br>");
            sbContentEmail.Append("</br>");
            if (string.IsNullOrEmpty(requester))
                sbContentEmail.Append("System");
            else
                sbContentEmail.Append(requester);

            return sbContentEmail.ToString();
        }
    }
}
