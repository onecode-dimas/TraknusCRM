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
    class BL_trs_mtar
    {
        #region Constants
        private const string _classname = "BL_trs_mtar";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_trs_mtar _DL_trs_mtar = new DL_trs_mtar();
        private DL_serviceappointment _DL_serviceappointment = new DL_serviceappointment();
        private BL_trs_workordersecondman _BL_trs_workordersecondman = new BL_trs_workordersecondman();
        private WOGenerator _woGenerator = new WOGenerator();
        #endregion

        #region Privates
        private void ChangeWOStatus_Dispatched(IOrganizationService organizationService, Guid woId, bool fromMobile)
        {
            try
            {
                _DL_serviceappointment = new DL_serviceappointment();
                _DL_serviceappointment.trs_isdispatched = true;
                _DL_serviceappointment.trs_frommobile = fromMobile;
                _DL_serviceappointment.Update(organizationService, woId);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".ChangeWOStatus_Hold : " + ex.Message);
            }
        }

        private void ChangeWOStatus_Hold(IOrganizationService organizationService, Guid woId, bool fromMobile)
        {
            try
            {
                //_DL_serviceappointment = new DL_serviceappointment();
                //_DL_serviceappointment.trs_frommobile = fromMobile;
                //_DL_serviceappointment.Update(organizationService, woId);

                _DL_serviceappointment.Hold(organizationService, woId);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".ChangeWOStatus_Hold : " + ex.Message);
            }
        }

        private void ChangeWOStatus_Unhold(IOrganizationService organizationService, Guid woId, bool fromMobile)
        {
            try
            {
                //_DL_serviceappointment = new DL_serviceappointment();
                //_DL_serviceappointment.trs_frommobile = fromMobile;
                //_DL_serviceappointment.Update(organizationService, woId);

                _DL_serviceappointment.Unhold(organizationService, woId);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".ChangeWOStatus_Unhold : " + ex.Message);
            }
        }

        private void ChangeWOStatus_Arrived(IOrganizationService organizationService, Guid woId, bool fromMobile)
        {
            try
            {
                _DL_serviceappointment = new DL_serviceappointment();
                _DL_serviceappointment.trs_isarrived = true;
                _DL_serviceappointment.trs_frommobile = fromMobile;
                _DL_serviceappointment.Update(organizationService, woId);

                _DL_serviceappointment.Arrived(organizationService, woId);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".ChangeWOStatus_Arrived : " + ex.Message);
            }
        }

        private void ChangeWOStatus_InProgress(IOrganizationService organizationService, Guid woId, bool fromMobile)
        {
            try
            {
                _DL_serviceappointment = new DL_serviceappointment();
                _DL_serviceappointment.trs_isarrived = true;
                _DL_serviceappointment.trs_frommobile = fromMobile;
                _DL_serviceappointment.Update(organizationService, woId);

                _DL_serviceappointment.InProgress(organizationService, woId);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".ChangeWOStatus_InProgress : " + ex.Message);
            }
        }

        private void ChangeWOStatus_SubmitTECObyMechanic(IOrganizationService organizationService, Guid woId, bool fromMobile)
        {
            try
            {
                //_DL_serviceappointment = new DL_serviceappointment();
                //_DL_serviceappointment.trs_frommobile = fromMobile;
                //_DL_serviceappointment.Update(organizationService, woId);

                _DL_serviceappointment.SubmitTECObyMechanic(organizationService, woId);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".ChangeWOStatus_SubmitTECObyMechanic : " + ex.Message);
            }
        }

        private void SendtoMobile(IOrganizationService organizationService, Guid id, bool fromMobile, bool workshop)
        {
            if (!fromMobile && !workshop)
            {
                FMobile _fmobile = new FMobile(organizationService);
                _fmobile.SendMTAR(organizationService, id);
            }
        }

        private void UpdateWOActualStart(IOrganizationService organizationService, Guid woId, int mtarStatus, DateTime? actualTime)
        {
            try
            {
                if (actualTime == null)
                {
                    throw new InvalidPluginExecutionException("Please fill the time !");
                }
                else
                {
                    if (mtarStatus == Configuration.MTAR_ReadytoRepair)
                    {
                        _DL_serviceappointment = new DL_serviceappointment();
                        _DL_serviceappointment.actualstart = (DateTime)actualTime;
                        _DL_serviceappointment.Update(organizationService, woId);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".UpdateWOActualStart : " + ex.Message);
            }
        }
        
        private DateTime GetLastMTARDate(IOrganizationService organizationService, Guid woId, Guid mechanicLeaderId)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_trs_mtar.EntityName);
                queryExpression.ColumnSet = new ColumnSet(true);
                FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                filterExpression.AddCondition("trs_workorder", ConditionOperator.Equal, woId);
                filterExpression.AddCondition("trs_mechanic", ConditionOperator.Equal, mechanicLeaderId);
                queryExpression.AddOrder("trs_automatictime", OrderType.Descending);

                EntityCollection eCollection = _DL_trs_mtar.Select(organizationService, queryExpression);
                if (eCollection.Entities.Count > 0)
                    return eCollection.Entities[0].GetAttributeValue<DateTime>("trs_automatictime").ToLocalTime();
                else
                    return DateTime.MinValue;
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".GetLastMTARDate : " + ex.Message);
            }
        }
        #endregion

        #region Publics
        #region Events
        #region Forms
        public void Form_OnCreate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
            try
            {
                if (entity.LogicalName == _DL_trs_mtar.EntityName)
                {
                    if (!entity.Attributes.Contains("trs_mechanic"))
                        throw new Exception("Can not found Mechanic Id.");

                    bool updateWOStatus = false;
                    Guid woId = entity.GetAttributeValue<EntityReference>("trs_workorder").Id;
                    Guid mechanicLeaderId = _woGenerator.GetMechanicLeader(organizationService, woId);
                    if (entity.GetAttributeValue<EntityReference>("trs_mechanic").Id == mechanicLeaderId)
                    {
                        //if (automaticTime > GetLastMTARDate(organizationService, woId, mechanicLeaderId))
                        updateWOStatus = true;
                        //else
                        //    _DL_trs_mtar.trs_updatewostatus = false;
                    }
                    else
                        updateWOStatus = false;
                    if (entity.Attributes.Contains("trs_updatewostatus"))
                        entity.Attributes["trs_updatewostatus"] = updateWOStatus;
                    else
                        entity.Attributes.Add("trs_updatewostatus", updateWOStatus);

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
                Entity ePost = (Entity)pluginExceptionContext.PostEntityImages["PostImage"];
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_trs_mtar.EntityName)
                {
                    if (entity.Attributes.Contains("trs_workorder") && entity.Attributes["trs_workorder"] != null)
                    {
                        Guid woId = entity.GetAttributeValue<EntityReference>("trs_workorder").Id;
                        
                        if (entity.Attributes.Contains("trs_mtarstatus") && entity.Attributes["trs_mtarstatus"] != null)
                        {
                            bool workshop = false;
                            if (entity.Attributes.Contains("trs_workshop") && entity.Attributes["trs_workshop"] != null)
                                workshop = entity.GetAttributeValue<bool>("trs_workshop");

                            bool fromMobile = false;
                            if (entity.Attributes.Contains("trs_frommobile") && entity.Attributes["trs_frommobile"] != null)
                                fromMobile = entity.GetAttributeValue<bool>("trs_frommobile");

                            bool updateWOStatus = false;
                            if (entity.Attributes.Contains("trs_updatewostatus") && entity.Attributes["trs_updatewostatus"] != null)
                                updateWOStatus = entity.GetAttributeValue<bool>("trs_updatewostatus");

                            if (((OptionSetValue)entity.Attributes["trs_mtarstatus"]).Value == Configuration.MTAR_Arrived)
                                _BL_trs_workordersecondman.RemoveSecondman(organizationService, woId, entity.GetAttributeValue<EntityReference>("trs_mechanic").Id);
                            
                            if (updateWOStatus)
                            {
                                DateTime? actualTime = null;
                                if (entity.Attributes.Contains("trs_manualtime") && entity.Attributes["trs_manualtime"] != null)
                                    actualTime = (DateTime)entity.Attributes["trs_manualtime"];
                                else if (entity.Attributes.Contains("trs_automatictime") && entity.Attributes["trs_automatictime"] != null)
                                    actualTime = (DateTime)entity.Attributes["trs_automatictime"];
                                UpdateWOActualStart(organizationService, woId, entity.GetAttributeValue<OptionSetValue>("trs_mtarstatus").Value, actualTime);

                                switch (((OptionSetValue)entity.Attributes["trs_mtarstatus"]).Value)
                                {
                                    case Configuration.MTAR_Dispatch:
                                        ChangeWOStatus_Dispatched(organizationService, woId, fromMobile);
                                        break;
                                    case Configuration.MTAR_Arrived:
                                        ChangeWOStatus_Arrived(organizationService, woId, fromMobile);
                                        break;
                                    case Configuration.MTAR_ReadytoRepair:
                                        ChangeWOStatus_InProgress(organizationService, woId, fromMobile);
                                        break;
                                    case Configuration.MTAR_Hold:
                                        ChangeWOStatus_Hold(organizationService, woId, fromMobile);
                                        break;
                                    case Configuration.MTAR_Resume:
                                        ChangeWOStatus_Unhold(organizationService, woId, fromMobile);
                                        break;
                                    case Configuration.MTAR_SubmitTeco:
                                        ChangeWOStatus_SubmitTECObyMechanic(organizationService, woId, fromMobile);
                                        break;
                                }
                            }

                            SendtoMobile(organizationService, entity.Id, fromMobile, workshop);
                        }
                        else
                        {
                            throw new InvalidPluginExecutionException("MTAR Status can not empty.");
                        }
                    }
                    else
                    {
                        throw new InvalidPluginExecutionException("WO Number can not empty.");
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
        public void Form_OnUpdate_trs_mtarstatus(IOrganizationService organizationService, IPluginExecutionContext pluginExcecutionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExcecutionContext.InputParameters["Target"];
                Entity pre = (Entity)pluginExcecutionContext.PreEntityImages["PreImage"];

                if (pluginExcecutionContext.MessageName == "Update")
                {
                    int mtarstatus = 0;
                    Guid workorderid = Guid.Empty;

                    if (entity.Attributes.Contains("trs_workorder") && entity.Attributes["trs_workorder"] != null)
                        workorderid = ((EntityReference)entity.Attributes["trs_workorder"]).Id;
                    else if (pre.Attributes.Contains("trs_workorder") && pre.Attributes["trs_workorder"] != null)
                        workorderid = ((EntityReference)pre.Attributes["trs_workorder"]).Id;
                    if(entity.Attributes.Contains("trs_mtarstatus") && entity.Attributes["trs_mtarstatus"] != null)
                        mtarstatus = ((OptionSetValue)entity.Attributes["trs_mtarstatus"]).Value;
                    else if (pre.Attributes.Contains("trs_mtarstatus") && pre.Attributes["trs_mtarstatus"] != null)
                        mtarstatus = ((OptionSetValue)pre.Attributes["trs_mtarstatus"]).Value;

                    DateTime? actualTime = null;
                    if (entity.Attributes.Contains("trs_manualtime") && entity.Attributes["trs_manualtime"] != null)
                        actualTime = entity.GetAttributeValue<DateTime>("trs_manualtime").ToLocalTime();
                    else if (pre.Attributes.Contains("trs_manualtime") && pre.Attributes["trs_manualtime"] != null)
                        actualTime = pre.GetAttributeValue<DateTime>("trs_manualtime").ToLocalTime();
                    else if (entity.Attributes.Contains("trs_automatictime") && entity.Attributes["trs_automatictime"] != null)
                        actualTime = entity.GetAttributeValue<DateTime>("trs_automatictime").ToLocalTime();
                    else if (pre.Attributes.Contains("trs_automatictime") && pre.Attributes["trs_automatictime"] != null)
                        actualTime = pre.GetAttributeValue<DateTime>("trs_automatictime").ToLocalTime();
                    UpdateWOActualStart(organizationService, workorderid, mtarstatus, actualTime);

                    bool workshop = false;
                    if (entity.Attributes.Contains("trs_workshop") && entity.Attributes["trs_workshop"] != null)
                        workshop = entity.GetAttributeValue<bool>("trs_workshop");
                    else if (pre.Attributes.Contains("trs_workshop") && pre.Attributes["trs_workshop"] != null)
                        workshop = pre.GetAttributeValue<bool>("trs_workshop");

                    bool fromMobile = false;
                    if (entity.Attributes.Contains("trs_frommobile") && entity.Attributes["trs_frommobile"] != null)
                        fromMobile = entity.GetAttributeValue<bool>("trs_frommobile");

                    SendtoMobile(organizationService, entity.Id, fromMobile, workshop);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate : " + ex.Message.ToString());
            }
        }
        #endregion
        #endregion
        #endregion
    }
}
