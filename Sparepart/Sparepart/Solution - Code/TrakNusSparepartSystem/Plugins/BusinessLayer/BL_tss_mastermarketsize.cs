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
    public class BL_tss_mastermarketsize
    {
        #region Constants
        private const string _classname = "BL_tss_mastermarketsize";
        private const string _entityname = "tss_mastermarketsize";
        #endregion

        public void Form_OnCreate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                string oName = "";

                if (entity.LogicalName == _entityname)
                {
                    EntityReference oCustomerQ = (EntityReference)entity.Attributes["tss_customer"];

                    var oCustomer = (organizationService.Retrieve(oCustomerQ.LogicalName, oCustomerQ.Id, new ColumnSet(true)))["name"].ToString();
                    String oUnitType = "";

                    if (entity.Attributes.Contains("tss_unittype"))
                    {
                        oUnitType = " - " + entity.FormattedValues["tss_unittype"];
                    }

                    if (entity.Attributes.Contains("tss_groupuiocommodity"))
                    {
                        EntityReference oGroupUIOCommodityQ = (EntityReference)entity.Attributes["tss_groupuiocommodity"];
                        var oGroupUIOCommodity = organizationService.Retrieve(oGroupUIOCommodityQ.LogicalName, oGroupUIOCommodityQ.Id, new ColumnSet(true));

                        oName = "MS - " + oCustomer + oUnitType + " - " + oGroupUIOCommodity["tss_groupuiocommodityname"].ToString();
                    }
                    else if (entity.Attributes.Contains("tss_serialnumber"))
                    {
                        EntityReference oSerialNumberQ = (EntityReference)entity.Attributes["tss_serialnumber"];
                        var oSerialNumber = organizationService.Retrieve(oSerialNumberQ.LogicalName, oSerialNumberQ.Id, new ColumnSet(true));

                        oName = "MS - " + oCustomer + oUnitType + " - " + oSerialNumber["new_productname"].ToString();
                    }
                    else
                    {
                        oName = "MS - " + oCustomer + " - " + oUnitType;
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

                //if (entity.LogicalName == _entityname)
                //{
                //    var oType = entity.GetAttributeValue<OptionSetValue>("tss_approvaltype").Value;
                //    String oApprovalFor = entity.FormattedValues["tss_approvalfor"];
                //    String oApprovalType = entity.FormattedValues["tss_approvaltype"];

                //    string oPriorityNo = entity.Attributes["tss_priorityno"].ToString();
                //    string oName = "";

                //    if (oType == 865920002)
                //        oName = oApprovalFor + " - " + oApprovalType + " - " + oPriorityNo;
                //    else
                //    {
                //        DL_population _DL_population = new DL_population();
                //        EntityReference oBranchQ = (EntityReference)entity.Attributes["tss_branch"];
                //        var oBranch = organizationService.Retrieve(oBranchQ.LogicalName, oBranchQ.Id, new ColumnSet(true));

                //        oName = oApprovalFor + " - " + oApprovalType + " - " + oBranch["name"].ToString() + " - " + oPriorityNo;
                //    }

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
