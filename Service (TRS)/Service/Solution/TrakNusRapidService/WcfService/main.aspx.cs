using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Reflection;

using TrakNusRapidService.Helper;

namespace TrakNusRapidServiceWcfService
{
    public partial class main : System.Web.UI.Page
    {
        LogReader logReader = new LogReader();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                /*ddlFunctionName.DataSource = GetListofFunction();
                ddlFunctionName.DataTextField = "functionName";
                ddlFunctionName.DataValueField = "functionName";
                ddlFunctionName.DataBind();*/

                ddlFileName.DataSource = logReader.GetListFile();
                ddlFileName.DataTextField = "name";
                ddlFileName.DataValueField = "fileName";
                ddlFileName.DataBind();
            }
        }

        protected DataTable GetListofFunction()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("functionName");

            foreach (MethodInfo methodInfo in typeof(Mobile).GetMethods())
            {
                dt.Rows.Add(methodInfo.Name);
            }
            return dt;
        }

        protected void ddlFileName_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void btnShow_Click(object sender, EventArgs e)
        {
            lblContent.Text = logReader.Read(ddlFileName.SelectedValue, "");
        }
    }
}