using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Web;
using System.Web.Services.Protocols;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Client.Messages;
using Microsoft.Xrm.Client.Configuration;
using Microsoft.Xrm.Client.Services;
using Xrm;
using System.Net.Mail;
using System.Net.Mime;
using System.Text.RegularExpressions;
//using QuotationEmailRDL.reportexec;

namespace PortalCRMTraknus
{
    public partial class QuoteEmailPDF : System.Web.UI.Page
    {
        string emailTo = "";
        string customer_name = "";
        Microsoft.Xrm.Client.CrmConnection connection = null;
        string id = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            string strServer = "http://tncrmdev/TraktorNusantara";
            string strDomain = "traknus";
            string strUsername = "service.crm";
            string strPass = "53rv1c35tr4knu5CRM";
            string strConnectionCRM = "Server=" + strServer + "; Domain=" + strDomain + "; Username=" + strUsername + "; Password=" + strPass + ";";
            connection = Microsoft.Xrm.Client.CrmConnection.Parse(strConnectionCRM);

            id = Request.QueryString["id"];
            id = "900E17EF-853F-E211-90D4-005056924533";

            using (var crm = new XrmServiceContext(connection))
            {
                var etyQuote = crm.QuoteSet.Where(q => q.Id == new Guid(id)).FirstOrDefault();
                if (etyQuote != null)
                {
                    var etyAccount = crm.AccountSet.Where(a => a.Id == etyQuote.CustomerId.Id).FirstOrDefault();
                    if (etyAccount != null)
                    {
                        customer_name = etyAccount.Name;
                        emailTo = etyAccount.EMailAddress1;
                    }
                }
            }
            if (this.CustomerTextBox.Text.Trim() == "")
            {
                this.CustomerTextBox.Text = customer_name;
            }
            if (this.EmailToTextBox.Text.Trim() == "")
            {
                this.EmailToTextBox.Text = emailTo;
            }
        }

        protected void saveFilePDFTo(Byte[] res)
        {
            try
            {
                string strCreateFileTemp = ConfigurationManager.AppSettings["Quote_Create_File_Temp"];

                FileStream fs = File.Create(@"" + strCreateFileTemp, res.Length);
                fs.Write(res, 0, res.Length);
                fs.Close();
            }
            catch (Exception exc)
            {
                Response.Write("Error: " + exc.ToString());
            }
        }
        protected void sendEmailQuotation(string emailTo, string Name)
        {
            string strFileTemp = ConfigurationManager.AppSettings["Quote_File_Temp"];
            string strSMTP = ConfigurationManager.AppSettings["Quote_SmtpClient"];
            string strFrom = ConfigurationManager.AppSettings["Quote_From"];
            string strTo = ConfigurationManager.AppSettings["Quote_To"];
            string strSubject = ConfigurationManager.AppSettings["Quote_Subject"];
            string strBody = ConfigurationManager.AppSettings["Quote_Body"];

            strBody = strBody.Replace("{", "<");
            strBody = strBody.Replace("}", ">");
            strBody = strBody.Replace("[Customer]", Name);

            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            System.Net.Mail.SmtpClient SmtpServer = new System.Net.Mail.SmtpClient();
            Attachment attach = new Attachment(strFileTemp);

            try
            {
                SmtpServer.UseDefaultCredentials = true;
                SmtpServer.EnableSsl = true;
                //SmtpServer.Timeout = 0;
                SmtpServer.Host = strSMTP;
                //SmtpServer.Host = "smtp.gmail.com";
                SmtpServer.EnableSsl = true;

                mail.Subject = strSubject;
                mail.Body = strBody;
                mail.Attachments.Add(attach);
                mail.IsBodyHtml = true;

                mail.From = new System.Net.Mail.MailAddress(strFrom, "Traknus");
                //mail.To.Add(strTo);
                mail.To.Add(emailTo);
                mail.DeliveryNotificationOptions = System.Net.Mail.DeliveryNotificationOptions.OnFailure;
                SmtpServer.Send(mail);

                success_message.Text = "Email Sent";
            }
            catch (Exception excm)
            {
                success_message.Text = "Failed " + excm.ToString();
            }
        }

        protected void SendButton_Click(object sender, EventArgs e)
        {
            bool cekEmail = false;
            customer_name = this.CustomerTextBox.Text.Trim();
            emailTo = this.EmailToTextBox.Text.Trim();

            if (this.EmailToTextBox.Text.Contains(";"))
            {
                string[] emailtos = this.EmailToTextBox.Text.Trim().Split(';');
                foreach (string email in emailtos)
                {
                    if (CekEmail(email))
                    {
                        cekEmail = true;
                    }
                    else
                    {
                        cekEmail = false;
                        break;
                    }
                }
            }
            else
            {
                if (CekEmail(emailTo))
                {
                    cekEmail = true;
                }
                else
                {
                    cekEmail = false;
                }
            }
            if (cekEmail ==  true)
            {
                sendingEmailTo();
            }
            else
            {
                throw new Exception("Invalid Format Email");
            }
        }
        public static bool CekEmail(string Email)
        {
            Regex regy = new Regex(@"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" + 
                  @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
            if (regy.IsMatch(Email))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void sendingEmailTo()
        {
            #region create SSRS

            string strURL_SSRS = ConfigurationManager.AppSettings["Quote_URL_SSRS"];
            string strUser_SSRS = ConfigurationManager.AppSettings["Quote_User_SSRS"];
            string strPass_SSRS = ConfigurationManager.AppSettings["Quote_Password_SSRS"];
            string strDomain_SSRS = ConfigurationManager.AppSettings["Quote_Domain_SSRS"];
            string strReportPath_SSRS = ConfigurationManager.AppSettings["Quote_Report_Path"];
            string strReportLogoPath_SSRS = ConfigurationManager.AppSettings["Quote_Report_Logo_Path"];

            string encodeddata = "";
            string devInfo = @"<DeviceInfo><Toolbar>False</Toolbar></DeviceInfo>";

            ReportExecutionService rs = new ReportExecutionService();

            rs.Url = strURL_SSRS;

            rs.Credentials = new System.Net.NetworkCredential(strUser_SSRS, strPass_SSRS, strDomain_SSRS);

            Byte[] result;
            string encoding;
            string extension;
            string mimetype;


            Warning[] warnings = null;
            string[] streamids;



            string orglcid = Request.QueryString["orglcid"];
            string orgname = Request.QueryString["orgname"];
            string type = Request.QueryString["type"];
            string typename = Request.QueryString["typename"];
            string userlcid = Request.QueryString["userlcid"];

            ParameterValue[] parameters = new ParameterValue[6];
            parameters[0] = new ParameterValue();
            parameters[0].Name = "id";
            parameters[0].Value = id;
            //parameters[0].Value = "1561EE3F-B203-E211-92C9-005056924533";

            parameters[1] = new ParameterValue();
            parameters[1].Name = "orglcid";
            parameters[1].Value = orglcid;
            //parameters[1].Value = "1033";

            parameters[2] = new ParameterValue();
            parameters[2].Name = "orgname";
            parameters[2].Value = orgname;
            //parameters[2].Value = "Traktor Nusantara";

            parameters[3] = new ParameterValue();
            parameters[3].Name = "type";
            parameters[3].Value = type;
            //parameters[3].Value = "1084";

            parameters[4] = new ParameterValue();
            parameters[4].Name = "typename";
            parameters[4].Value = typename;
            //parameters[4].Value = "quote";

            parameters[5] = new ParameterValue();
            parameters[5].Name = "userlcid";
            parameters[5].Value = userlcid;
            //parameters[5].Value = "1033";

            ExecutionInfo execInfo = new ExecutionInfo();
            ExecutionHeader eh = new ExecutionHeader();
            rs.ExecutionHeaderValue = eh;
            //execInfo = rs.LoadReport("/TraktorNusantara_MSCRM/Quotation", null);
            //execInfo = rs.LoadReport(strReportPath_SSRS, null);
            using (var crm = new XrmServiceContext(connection))
            {
                var etyQuote = crm.QuoteSet.Where(q => q.Id == new Guid(id)).FirstOrDefault();
                if (etyQuote != null)
                {
                    if (etyQuote.new_QuoteLogo == true)
                    {
                        execInfo = rs.LoadReport(strReportLogoPath_SSRS, null);
                    }
                    else
                    {
                        execInfo = rs.LoadReport(strReportPath_SSRS, null);
                    }
                }
            }

            //rs.SetExecutionParameters(parameters, "en-us");

            //result = rs.Render("PDF", devInfo, out extension, out mimetype, out encoding, out warnings, out streamids);
            //encodeddata = System.Convert.ToBase64String(result);

            ////Save File Pdf ke Folder
            //saveFilePDFTo(result);

            //Send Email

            #endregion
            sendEmailQuotation(emailTo, customer_name);
        }
        public bool validationMinimumValue()
        {
            return true;
        }
    }
}