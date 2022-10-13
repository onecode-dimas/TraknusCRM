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
    class BL_trs_ppmreport
    {
        #region Constants
        private const string _classname = "BL_trs_ppmreport";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_trs_ppmreport _DL_trs_ppmreport = new DL_trs_ppmreport();
        private BL_trs_runningnumber _BL_trs_runningnumber = new BL_trs_runningnumber();
        #endregion

        #region Privates
        private void SendtoMobile(IOrganizationService organizationService, Guid id, bool fromMobile, bool workshop)
        {
            if (!fromMobile && !workshop)
            {
                FMobile _fmobile = new FMobile(organizationService);
                _fmobile.SendPPMInspectionReport(organizationService, id);
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
                if (entity.LogicalName == _DL_trs_ppmreport.EntityName)
                {
                    if (!entity.Attributes.Contains("trs_mobileguid"))
                        entity.Attributes.Add("trs_mobileguid", entity.Id.ToString());

                    string newRunningNumber = _BL_trs_runningnumber.GenerateNewRunningNumber(organizationService, pluginExceptionContext, _DL_trs_ppmreport.EntityName, (DateTime)entity.Attributes["createdon"]);

                    /*167630000 = Inspection ; 167630001 = PPM*/
                    if (entity.GetAttributeValue<OptionSetValue>("trs_type").Value == 167630000)
                    {
                        newRunningNumber = newRunningNumber.Replace("PPM", "INP");
                    }

                    if (entity.Attributes.Contains("trs_reportnumber"))
                        entity.Attributes["trs_reportnumber"] = newRunningNumber;
                    else
                        entity.Attributes.Add("trs_reportnumber", newRunningNumber);
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
                if (entity.LogicalName == _DL_trs_ppmreport.EntityName)
                {
                    bool fromMobile = false;
                    if (entity.Attributes.Contains("trs_frommobile"))
                        fromMobile = entity.GetAttributeValue<bool>("trs_frommobile");

                    bool workshop = false;
                    if (entity.Attributes.Contains("trs_workshop"))
                        workshop = entity.GetAttributeValue<bool>("trs_workshop");

                    //Get PMActType Id From Wo
                    Guid workorderId = ((EntityReference)entity.Attributes["trs_workorder"]).Id;
                    Entity wo = organizationService.Retrieve("serviceappointment", workorderId, new ColumnSet(new string[] { "trs_pmacttype" }));
                    if (wo.Attributes.Contains("trs_pmacttype"))
                    {
                        Guid PMActTypeId = ((EntityReference)wo.Attributes["trs_pmacttype"]).Id;

                        Entity taskListGroup = organizationService.Retrieve("trs_tasklistgroup", PMActTypeId, new ColumnSet(new string[] { "trs_tasklisttypeid" }));
                        Guid tasklistTypeId = ((EntityReference)taskListGroup.Attributes["trs_tasklisttypeid"]).Id;

                        Entity taskListType = organizationService.Retrieve("trs_tasklisttype", tasklistTypeId, new ColumnSet(new string[] { "trs_tasklisttypename" }));
                        string ActType = taskListType.Attributes["trs_tasklisttypename"].ToString();

                        if (ActType.Trim() == "INSPECTION" || ActType.Trim() == "PPM")
                        {
                            SendtoMobile(organizationService, entity.Id, fromMobile, workshop);
                        }
                    }
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
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                Entity pre = (Entity)pluginExceptionContext.PreEntityImages["Target"];
                if (entity.LogicalName == _DL_trs_ppmreport.EntityName)
                {
                    bool fromMobile = false;
                    if (entity.Attributes.Contains("trs_frommobile"))
                        fromMobile = entity.GetAttributeValue<bool>("trs_frommobile");
                    else if (pre.Attributes.Contains("trs_frommobile"))
                        fromMobile = pre.GetAttributeValue<bool>("trs_frommobile");

                    bool workshop = false;
                    if (entity.Attributes.Contains("trs_workshop"))
                        workshop = entity.GetAttributeValue<bool>("trs_workshop");
                    if (pre.Attributes.Contains("trs_workshop"))
                        workshop = pre.GetAttributeValue<bool>("trs_workshop");

                    //Get PMActType Id From Wo
                    Guid workorderId = ((EntityReference)entity.Attributes["trs_workorder"]).Id;
                    Entity wo = organizationService.Retrieve("serviceappointment", workorderId, new ColumnSet(new string[] { "trs_pmacttype" }));
                    if (wo.Attributes.Contains("trs_pmacttype"))
                    {
                        Guid PMActTypeId = ((EntityReference)wo.Attributes["trs_pmacttype"]).Id;

                        Entity taskListGroup = organizationService.Retrieve("trs_tasklistgroup", PMActTypeId, new ColumnSet(new string[] { "trs_tasklisttypeid" }));
                        Guid tasklistTypeId = ((EntityReference)taskListGroup.Attributes["trs_tasklisttypeid"]).Id;

                        Entity taskListType = organizationService.Retrieve("trs_tasklisttype", tasklistTypeId, new ColumnSet(new string[] { "trs_tasklisttypename" }));
                        string ActType = taskListType.Attributes["trs_tasklisttypename"].ToString();

                        if (ActType.Trim() == "INSPECTION" || ActType.Trim() == "PPM")
                        {
                            SendtoMobile(organizationService, entity.Id, fromMobile, workshop);
                        }
                    }
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
        #endregion

        #region Fields
        #endregion
        #endregion
    }
}
