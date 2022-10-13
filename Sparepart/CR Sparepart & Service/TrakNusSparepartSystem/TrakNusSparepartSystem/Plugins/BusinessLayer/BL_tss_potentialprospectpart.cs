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
    public class BL_tss_potentialprospectpart
    {
        #region Constants
        private const string _classname = "BL_tss_potentialprospectpart";
        private const string _entityname = "tss_potentialprospectpart";
        #endregion

        #region Depedencies
        private BL_trs_runningnumber _BL_trs_runningnumber = new BL_trs_runningnumber();
        #endregion


        #region Form Event

        public void Form_OnCreate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                if (entity.LogicalName == _entityname)
                {

                    DateTime createdOn = DateTime.Now.ToLocalTime();
                    //string categoryCode = "03";
                    //Generate New Running Number
                    string newRunningNumber = _BL_trs_runningnumber.GenerateNewRunningNumberModulMarketSize(
                        organizationService, pluginExceptionContext, _entityname, createdOn);
                    if (entity.Attributes.Contains("tss_potentialprospectid"))
                        entity.Attributes["tss_potentialprospectid"] = newRunningNumber;
                    else
                        entity.Attributes.Add("tss_potentialprospectid", newRunningNumber);
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

        #endregion
    }
}
