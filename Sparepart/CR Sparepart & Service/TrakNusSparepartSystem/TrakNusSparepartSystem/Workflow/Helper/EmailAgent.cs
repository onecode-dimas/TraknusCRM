using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusSparepartSystem.Workflow.Helper
{
    public class EmailAgent
    {
        /// <summary>
        /// Create activity party for email
        /// </summary>
        /// <param name="input">List of Tuple that consist of Entity Guid and Entity Logical Name</param>
        /// <returns>Array of Activity Party Entity</returns>
        public Entity[] CreateActivityParty(List<Tuple<Guid, String>> input)
        {
            return input.Select(n =>
            {
                Entity activityPartyEntity = new Entity("activityparty");
                activityPartyEntity["partyid"] = new EntityReference(n.Item2, n.Item1);
                return activityPartyEntity;
            }).ToArray();
        }

        /// <summary>
        /// Send the email
        /// </summary>
        /// <param name="organizationService">CRM Organization Service</param>
        /// <param name="emailGuid">Email Entity that is going to be sent</param>
        public void SendEmail(IOrganizationService organizationService, Guid emailGuid)
        {
            SendEmailRequest request = new SendEmailRequest()
            {
                EmailId = emailGuid,
                IssueSend = true,
                TrackingToken = ""
            };
            organizationService.Execute(request);
        }

        private EntityCollection ExecuteFetchXml(IOrganizationService organizationService, string fetchXml)
        {
            try
            {
                EntityCollection result = organizationService.RetrieveMultiple(new FetchExpression(fetchXml));
                return result;
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        private String GenerateRowHtml(Entity entity, IOrganizationService organizationService, string template)
        {
            ParserAgent parserAgent = new ParserAgent();
            return parserAgent.ParseTemplate(template, entity, organizationService);
        }

        public string GenerateEmail(EntityCollection entityCollection, IOrganizationService organizationService, Entity configurationEntity, string bodyTemplate, string rowTemplate)
        {
            ParserAgent agent = new ParserAgent();

            bodyTemplate = agent.ParseTemplate(bodyTemplate, configurationEntity, organizationService);

            StringBuilder str = new StringBuilder();
            for (int index = 0; index < entityCollection.Entities.Count; index++)
            {
                var entity = entityCollection.Entities[index];
                entity["datarow"] = index + 1;
                str.AppendLine(GenerateRowHtml(entity, organizationService, rowTemplate));
            }

            return bodyTemplate.Replace("@datarow", str.ToString());
        }


    }


}
