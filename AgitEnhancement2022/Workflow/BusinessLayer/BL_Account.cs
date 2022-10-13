using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;

namespace AgitEnhancement2022.Workflow.BusinessLayer
{
    public class BL_Account
    {
        private string _classname = "BL_Account";
        private string _entityname_salesorder = "account";
        private string _entityname_systemuser = "systemuser";

        public void Approve_ToFinanceDeptHead(IOrganizationService _organizationservice, IWorkflowContext _context, string[] _recordids)
        {
            Guid _quoteid = new Guid(_recordids[0]);

            // SEND EMAIL
            //SendMail(_organizationservice, _context, _masterconditiontype, _quote, _owner, _currentapprover, Guid.Empty);

        }

        public void SendMail(IOrganizationService _organizationservice, IWorkflowContext _context, Entity _masterconditiontype, Entity _quote, Entity _sender, Entity _receiver, Guid _cc)
        {
            /*
            String _stringpathandquery = HttpContext.Current.Request.Url.PathAndQuery;
            string CRM_URL = HttpContext.Current.Request.Url.AbsoluteUri.Replace(_stringpathandquery, "/");
            CRM_URL += _context.OrganizationName; // "TraktorNusantara";

            string _objecttypecode = string.Empty;
            string _emailsubject = "REQUEST [" + _masterconditiontype.GetAttributeValue<string>("ittn_description") + "] - [" + _quote.GetAttributeValue<string>("name") + "] - [" + _sender.GetAttributeValue<string>("fullname") + "]";
            string _emailcontent = "";

            //throw new InvalidWorkflowException("IDR " + string.Format("{0:#,0.00;- #,0.00;'-'}", _quote.GetAttributeValue<Money>("totalamount").Value));

            Globalization.GetObjectTypeCode(_organizationservice, "quote", out _objecttypecode);

            _emailcontent += "Dear " + _receiver.GetAttributeValue<string>("fullname") + ",<br/><br/>";
            _emailcontent += "There is request for " + _masterconditiontype.GetAttributeValue<string>("ittn_description") + " with details below :<br/><br/>";
            _emailcontent += "<ul>";
            _emailcontent += "<li>Quotation Number : " + _quote.GetAttributeValue<string>("quotenumber") + "</li>";
            _emailcontent += "<li>Customer : " + _quote.GetAttributeValue<EntityReference>("customerid").Name + "</li>";
            _emailcontent += "<li>BC : " + _quote.GetAttributeValue<EntityReference>("createdby").Name + "</li>";
            _emailcontent += "<li>Total Sales Amount : IDR " + string.Format("{0:#,0.00;- #,0.00;'-'}", _quote.GetAttributeValue<Money>("totalamount").Value) + "</li>";
            _emailcontent += "</ul><br/>";
            _emailcontent += "You can Approve / Reject the request from <a href='" + CRM_URL + "/main.aspx?etc=" + _objecttypecode + "&pagetype=entityrecord&id=%7b" + _quote.Id + "%7d'>here</a>.<br/><br/>";

            _emailcontent += "Thanks.<br/><br/>";
            _emailcontent += "Regards,<br/>";
            _emailcontent += _quote.GetAttributeValue<EntityReference>("createdby").Name;

            EmailAgent _emailagent = new EmailAgent();
            _emailagent.SendNotification(_sender.Id, _receiver.Id, _cc, _organizationservice, _emailsubject, _emailcontent);
            */
        }
    }
}
