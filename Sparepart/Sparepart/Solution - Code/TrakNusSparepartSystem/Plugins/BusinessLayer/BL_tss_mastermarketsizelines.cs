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
    public class BL_tss_mastermarketsizelines
    {
        #region Constants
        private const string _classname = "BL_tss_mastermarketsizelines";
        private const string _entityname = "tss_mastermarketsizelines";
        #endregion

        public void Form_OnCreate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                string oName = "";

                if (entity.LogicalName == _entityname)
                {
                    EntityReference oMasterMarketSizeRefQ = (EntityReference)entity.Attributes["tss_mastermarketsizeref"];

                    var oMasterMarketSizeName = (organizationService.Retrieve(oMasterMarketSizeRefQ.LogicalName, oMasterMarketSizeRefQ.Id, new ColumnSet(true)))["tss_name"].ToString();

                    String oHMPM = "";

                    if (entity.Attributes.Contains("tss_hmpm"))
                    {
                        oHMPM = ((int)entity.Attributes["tss_hmpm"]).ToString();
                        oName = oMasterMarketSizeName + " - " + oHMPM;
                    }
                    else
                    {
                        oName = oMasterMarketSizeName;
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
    }
}
