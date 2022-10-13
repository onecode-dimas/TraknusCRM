using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System.Net.Mail;

namespace PortalCRMTraknus
{
    public partial class RequestAccountNumberNPWP : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string id = Request.QueryString["id"];
            string strConnectionCRM = ConfigurationManager.AppSettings["ConnectionCRM"];

            var connection = Microsoft.Xrm.Client.CrmConnection.Parse(strConnectionCRM);
            Xrm.XrmServiceContext crm = new Xrm.XrmServiceContext(connection);

            var acc = crm.AccountSet.Where(a => a.Id.Equals(new Guid(id))).FirstOrDefault();
            if (acc != null)
            {
                try
                {
                    crm.ClearChanges();
                    var updateAcc = new Xrm.Account
                    {
                        Id = acc.Id,
                        new_sendRequestAccount = true
                    };
                    crm.Attach(updateAcc);
                    crm.UpdateObject(updateAcc);
                    crm.SaveChanges();
                    Response.Write("<script type='text/javascript'>self.close();</script>");
                }
                catch (Exception exc)
                {
                    Response.Write("<script type='text/javascript'>alert('"+ exc.ToString() +"');self.close();</script>");
                }
            }

            /*
            string id = Request.QueryString["id"];
            //throw new Exception(id.ToString());
            //id = "{83FD30FF-3002-E211-92C9-005056924533}";
            try
            {
                Guid acc_id = new Guid(id.Substring(1, id.Length - 2));
                string strConnectionCRM = ConfigurationManager.AppSettings["ConnectionCRM"];

                var connection = Microsoft.Xrm.Client.CrmConnection.Parse(strConnectionCRM);
                Xrm.XrmServiceContext crm = new Xrm.XrmServiceContext(connection);
                var acc = crm.AccountSet.Where(a => a.Id.Equals(acc_id)).FirstOrDefault();
                //var acc = crm.AccountSet.FirstOrDefault();
                if (acc != null)
                {
                    if (acc.AccountNumber == null || acc.new_NPWP == null)
                    {
                        string custome =
                            "<table>" +
                                "<tr>" +
                                    "<td>Account Name</td>" +
                                    "<td> : </td>" +
                                    "<td>" + acc.Name + "</td>" +
                                "</tr>" +
                                "<tr>" +
                                    "<td>E-mail</td>" +
                                    "<td> : </td>" +
                                    "<td>" + acc.EMailAddress1 + "</td>" +
                                "</tr>" +
                                "<tr>" +
                                    "<td>Main Phone</td>" +
                                    "<td> : </td>" +
                                    "<td>" + acc.Telephone1 + "</td>" +
                                "</tr>" +
                                "<tr>" +
                                    "<td>Fax</td>" +
                                    "<td> : </td>" +
                                    "<td>" + acc.Fax + "</td>" +
                                "</tr>" +
                            "</table>";
                        Response.Write("Email Process Send");
                        SendEmail(custome, acc.Name);
                    }
                    else
                    {
                        Response.Write("<script type='text/javascript'>self.close();</script>");
                    }
                }
                else
                {
                    throw new Exception("Account Not Found");
                }
                Response.Write("<script type='text/javascript'>alert('Email has been send'); self.close();</script>");
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            */
        }
        private void SendEmail(string custome,string name)
        {
            string strSmtpClient = ConfigurationManager.AppSettings["NPWP_SmtpClient"];
            string strFrom = ConfigurationManager.AppSettings["NPWP_From"];
            string strTo = ConfigurationManager.AppSettings["NPWP_To"];
            string strSubject = ConfigurationManager.AppSettings["NPWP_Subject"];
            string strBody = ConfigurationManager.AppSettings["NPWP_Body"];

            strBody = strBody.Replace("{", "<");
            strBody = strBody.Replace("}", ">");

            strBody = strBody.Replace("[custome]", custome);
            strSubject = strSubject.Replace("[name]", name);

            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient(strSmtpClient);

            //SmtpServer.Send(strFrom, "fatoni@ignitech.net", strSubject, strBody);

            mail.From = new MailAddress(strFrom);
            mail.To.Add(strTo);
            //mail.To.Add("fatoni@ignitech.net, raymond@ignitech.net");
            mail.Subject = strSubject;
            mail.IsBodyHtml = true;
            mail.Body = strBody;

            //SmtpServer.Credentials = new System.Net.NetworkCredential("admin.crm", "p@ssw0rdcrm");
            SmtpServer.Credentials = new System.Net.NetworkCredential("service.crm", "53rv1c35tr4knu5CRM");
            SmtpServer.UseDefaultCredentials = true;
            //SmtpServer.Credentials = null;
            //SmtpServer.UseDefaultCredentials = false;
            SmtpServer.EnableSsl = true;
            //SmtpServer.EnableSsl = false;
            

            SmtpServer.Send(mail);
        }
    }
}