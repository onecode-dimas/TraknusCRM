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
    public class BL_tss_matrixapprovalmarketsize
    {
        #region Constants
        private const string _classname = "BL_tss_matrixapprovalmarketsize";
        private const string _entityname = "tss_matrixapprovalmarketsize";
        #endregion

        public void Form_OnCreate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];

                if (entity.LogicalName == _entityname)
                {
                    var oType = entity.GetAttributeValue<OptionSetValue>("tss_approvaltype").Value;
                    String oApprovalFor = entity.FormattedValues["tss_approvalfor"];
                    String oApprovalType = entity.FormattedValues["tss_approvaltype"];

                    string oPriorityNo = entity.Attributes["tss_priorityno"].ToString();
                    string oName = "";

                    if (oType == 865920002)
                        oName = oApprovalFor + " - " + oApprovalType + " - " + oPriorityNo;
                    else
                    {
                        DL_population _DL_population = new DL_population();
                        EntityReference oBranchQ = (EntityReference)entity.Attributes["tss_branch"];
                        var oBranch = organizationService.Retrieve(oBranchQ.LogicalName, oBranchQ.Id, new ColumnSet(true));

                        oName = oApprovalFor + " - " + oApprovalType + " - " + oBranch["name"].ToString() + " - " + oPriorityNo;
                    }

                    if (entity.Attributes.Contains("tss_name"))
                    {
                        entity.Attributes["tss_name"] = oName;
                    }
                    else
                    {
                        entity.Attributes.Add("tss_name", oName);
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PreOperation : " + ex.Message.ToString());
            }
        }

        public void Form_OnUpdate_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));

                if (entity.LogicalName == _entityname)
                {
                    var oType = entity.GetAttributeValue<OptionSetValue>("tss_approvaltype").Value;
                    String oApprovalFor = entity.FormattedValues["tss_approvalfor"];
                    String oApprovalType = entity.FormattedValues["tss_approvaltype"];
                    
                    string oPriorityNo = entity.Attributes["tss_priorityno"].ToString();
                    string oName = "";

                    if (oType == 865920002)
                        oName = oApprovalFor + " - " + oApprovalType + " - " + oPriorityNo;
                    else
                    {
                        DL_population _DL_population = new DL_population();
                        EntityReference oBranchQ = (EntityReference)entity.Attributes["tss_branch"];
                        var oBranch = organizationService.Retrieve(oBranchQ.LogicalName, oBranchQ.Id, new ColumnSet(true));

                        oName = oApprovalFor + " - " + oApprovalType + " - " + oBranch["name"].ToString() + " - " + oPriorityNo;
                    }
                        
                    //if (entity.Attributes.Contains("tss_name"))
                    //{
                    //    entity.Attributes["tss_name"] = oName;
                    //}
                    //else
                    //{
                    //    entity.Attributes.Add("tss_name", oName);
                    //}

                    Entity entityupdate = new Entity(pluginExceptionContext.PrimaryEntityName);
                    entityupdate.Id = pluginExceptionContext.PrimaryEntityId;
                    entityupdate.Attributes["tss_name"] = oName;
                    organizationService.Update(entityupdate);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate_PostOperation: " + ex.Message.ToString());
            }
        }
    }
}
