using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

using Microsoft.Xrm.Sdk;
using TrakNusRapidService.DataLayer;
using Microsoft.Xrm.Sdk.Query;

using TrakNusRapidService.Helper;

namespace TrakNusRapidService.Plugins.BusinessLayer
{
    class BL_trs_technicalservicereport
    {
        #region Constants
        private const string _classname = "BL_trs_technicalservicereport";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_trs_technicalservicereport _DL_trs_technicalservicereport = new DL_trs_technicalservicereport();
        private DL_serviceappointment _DL_serviceappointment = new DL_serviceappointment();
        private DL_trs_tasklistgroup _DL_trs_tasklistgroup = new DL_trs_tasklistgroup();
        private DL_trs_tasklisttype _DL_trs_tasklisttype = new DL_trs_tasklisttype();
        private BL_trs_runningnumber _BL_trs_runningnumber = new BL_trs_runningnumber();
        #endregion

        #region Privates
        private void SendtoMobile(IOrganizationService organizationService, Guid id, bool workshop, bool fromMobile)
        {
            try
            {
                if (!workshop && !fromMobile)
                {
                    FMobile _fmobile = new FMobile(organizationService);
                    _fmobile.SendTSR(organizationService, id);
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
                if (entity.LogicalName == _DL_trs_technicalservicereport.EntityName)
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

        public void Form_OnCreate(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_trs_technicalservicereport.EntityName)
                {
                    string newRunningNumber = _BL_trs_runningnumber.GenerateNewRunningNumber(organizationService, pluginExceptionContext, _DL_trs_technicalservicereport.EntityName, (DateTime)entity.Attributes["createdon"]);
                    _DL_trs_technicalservicereport = new DL_trs_technicalservicereport();
                    _DL_trs_technicalservicereport.trs_tsrnumber = newRunningNumber;
                    _DL_trs_technicalservicereport.Update(organizationService, entity.Id);

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

        public void Form_OnUpdate(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity pre = (Entity)pluginExceptionContext.PreEntityImages["Target"];
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_trs_technicalservicereport.EntityName)
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
