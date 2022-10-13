using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using TrakNusRapidService.DataLayer;
using TrakNusRapidService.Helper;

namespace TrakNusRapidService.Plugins.BusinessLayer
{
    class BL_trs_workorderpartrecommendation
    {
        #region Constants
        private const string _classname = "BL_trs_workorderpartrecommendation";
        #endregion

        #region Depedencies
        private DL_trs_workorderpartrecommendation _DL_trs_workorderpartrecommendation = new DL_trs_workorderpartrecommendation();
        #endregion

        #region Privates
        private void SendtoMobile(IOrganizationService organizationService, Guid id, bool workshop, bool fromMobile)
        {
            try
            {
                if (!workshop && !fromMobile)
                {
                    FMobile _fmobile = new FMobile(organizationService);
                    _fmobile.SendServiceAppointment_Recommendation(organizationService, id);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".SendtoMobile : " + ex.Message);
            }
        }
        #endregion

        #region Events
        #region Forms
        public void Form_OnCreate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
            try
            {
                if (entity.LogicalName == _DL_trs_workorderpartrecommendation.EntityName)
                {
                    if (!entity.Attributes.Contains("trs_mobileguid"))
                        entity.Attributes.Add("trs_mobileguid", entity.Id.ToString());
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

        public void Form_OnCreate(IOrganizationService organizationService, IPluginExecutionContext pluginExcecutionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExcecutionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_trs_workorderpartrecommendation.EntityName)
                {
                    bool workshop = false;
                    if (entity.Attributes.Contains("trs_workshop") && entity.Attributes["trs_workshop"] != null)
                        workshop = entity.GetAttributeValue<bool>("trs_workshop");

                    bool fromMobile = false;
                    if (entity.Attributes.Contains("trs_frommobile") && entity.Attributes["trs_frommobile"] != null)
                        fromMobile = entity.GetAttributeValue<bool>("trs_frommobile");

                    SendtoMobile(organizationService, entity.Id, workshop, fromMobile);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate : " + ex.Message.ToString());
            }
        }

        public void Form_OnUpdate(IOrganizationService organizationService, IPluginExecutionContext pluginExcecutionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExcecutionContext.InputParameters["Target"];
                Entity pre = (Entity)pluginExcecutionContext.PreEntityImages["Target"];
                if (entity.LogicalName == _DL_trs_workorderpartrecommendation.EntityName)
                {
                    bool workshop = false;
                    if (entity.Attributes.Contains("trs_workshop") && entity.Attributes["trs_workshop"] != null)
                        workshop = entity.GetAttributeValue<bool>("trs_workshop");
                    else if (pre.Attributes.Contains("trs_workshop") && pre.Attributes["trs_workshop"] != null)
                        workshop = pre.GetAttributeValue<bool>("trs_workshop");

                    bool fromMobile = false;
                    if (entity.Attributes.Contains("trs_frommobile") && entity.Attributes["trs_frommobile"] != null)
                        fromMobile = entity.GetAttributeValue<bool>("trs_frommobile");

                    SendtoMobile(organizationService, entity.Id, workshop, fromMobile);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate : " + ex.Message.ToString());
            }
        }
        #endregion
        #endregion
    }
}
