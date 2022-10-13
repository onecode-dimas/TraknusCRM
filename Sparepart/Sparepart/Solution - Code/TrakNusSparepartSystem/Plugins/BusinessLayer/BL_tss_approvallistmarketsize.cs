using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TrakNusSparepartSystem.DataLayer;
using TrakNusSparepartSystem.Helper;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Web;
using System.Collections;

namespace TrakNusSparepartSystem.Plugins.BusinessLayer
{
    public class BL_tss_approvallistmarketsize
    {
        #region Constants
        private const string _classname = "BL_tss_approvallistmarketsize";
        private const string _entityname = "tss_approvallistmarketsize";
        #endregion

        public void Form_OnCreate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                //Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                //if (entity.LogicalName == _entityname)
                //{
                //    //var currentEntity = organizationService.Retrieve(entity.LogicalName, entity.Id, new ColumnSet(true));

                //    OptionSetValue oApprovalFor = (OptionSetValue)entity.Attributes["tss_approvalfor"];
                //    OptionSetValue oApprovalType = (OptionSetValue)entity.Attributes["tss_approvaltype"];
                //    EntityReference oBranch = (EntityReference)entity.Attributes["tss_branch"];
                //    string oPriorityNo = entity.Attributes["tss_priorityno"].ToString();
                //    string oName = oApprovalFor.Value + " - " + oApprovalType.Value + " - " + oBranch.Name + " - " + oPriorityNo.ToString();

                //    if (entity.Attributes.Contains("tss_name"))
                //    {
                //        entity.Attributes["tss_name"] = oName;
                //    }
                //    else
                //    {
                //        entity.Attributes.Add("tss_name", oName);
                //    }
                //}
                //else
                //{
                //    return;
                //}
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PreOperation : " + ex.Message.ToString());
            }
        }
    }
}
