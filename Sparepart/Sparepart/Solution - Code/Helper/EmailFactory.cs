using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;

namespace TrakNusSparepartSystem.Helper
{
    public class EmailFactory
    {
        private Entity[] From;
        private Entity[] Cc;
        private Entity[] Bcc;
        private String Subject;
        private String Content;
        private Entity[] To;

        /// <summary>
        /// Set The "From" Part Email 
        /// </summary>
        /// <param name="activityParties">Entity of Activity Parties <see cref="ParserAgent"/> at Console Application</param>
        /// <returns>This Email Factory for Fluent Chaining</returns>
        public EmailFactory SetFrom(Entity[] activityParties)
        {
            From = activityParties;
            return this;
        }

        /// <summary>
        /// Set The "CC" Part Email 
        /// </summary>
        /// <param name="activityParties">Entity of Activity Parties <see cref="ParserAgent"/> at Console Application</param>
        /// <returns>This Email Factory for Fluent Chaining</returns>
        public EmailFactory SetCC(Entity[] activityParties)
        {
            Cc = activityParties;
            return this;
        }

        /// <summary>
        /// Set The "To" Part Email 
        /// </summary>
        /// <param name="activityParties">Entity of Activity Parties <see cref="ParserAgent"/> at Console Application</param>
        /// <returns>This Email Factory for Fluent Chaining</returns>
        public EmailFactory SetTo(Entity[] activityParties)
        {
            To = activityParties;
            return this;
        }

        /// <summary>
        /// Set The "BCC" Part Email 
        /// </summary>
        /// <param name="activityParties">Entity of Activity Parties <see cref="ParserAgent"/> at Console Application</param>
        /// <returns>This Email Factory for Fluent Chaining</returns>
        public EmailFactory SetBcc(Entity[] activityParties)
        {
            Bcc = activityParties;
            return this;
        }

        /// <summary>
        /// Set the Subject for this Email <seealso cref="ParserAgent"/> at Console Application for paramterization (SetParameterValue)
        /// </summary>
        /// <param name="subject">String of subject</param>
        /// <returns>This Email Factory for Fluent Chaining</returns>
        public EmailFactory SetSubject(String subject)
        {
            Subject = subject;
            return this;
        }

        /// <summary>
        /// Set the content for this Email <seealso cref="ParserAgent"/> at Console Application for paramterization (SetParameterValue)
        /// </summary>
        /// <param name="content"></param>
        /// <returns>This Email Factory for Fluent Chaining</returns>
        public EmailFactory SetContent(String content)
        {
            Content = content;
            return this;
        }

        /// <summary>
        /// Create the Email Entity
        /// </summary>
        /// <returns></returns>
        public Entity Create()
        {
            Entity email = new Entity("email");
            if (To != null)
                email["to"] = To;
            if (From != null)
                email["from"] = From;
            if (Bcc != null)
                email["bcc"] = Bcc;
            if (Cc != null)
                email["cc"] = Cc;
            if (Subject != null)
                email["subject"] = Subject;
            if (Content != null)
                email["description"] = Content;
            return email;
        }

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
    }
}