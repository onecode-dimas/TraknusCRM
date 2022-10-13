using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using TrakNusRapidService.DataLayer;
using TrakNusRapidService.Helper;
using Microsoft.Xrm.Sdk.Query;

//temporary
using TrakNusRapidService.Helper.MobileWebService;

namespace TrakNusRapidService.Plugins.BusinessLayer
{
    class BL_incident
    {
        #region Constants
        private const string _classname = "BL_incident";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_incident _DL_incident = new DL_incident();
        private DL_contract _DL_contract = new DL_contract();
        private DL_contractdetail _DL_contractdetail = new DL_contractdetail();
        private DL_trs_populationincontract _DL_trs_populationincontract = new DL_trs_populationincontract();
        private BL_trs_runningnumber _BL_trs_runningnumber = new BL_trs_runningnumber();
        #endregion

        #region Privates
        private void SendtoMobile(IOrganizationService organizationService, Guid id)
        {
            try
            {
                FMobile _fmobile = new FMobile(organizationService);
                _fmobile.SendIncident(organizationService, id);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".SendtoMobile : " + ex.Message);
            }
        }
        #endregion

        #region Publics
        #endregion Publics

        #region Events
        #region Forms

        public void Form_OnCreate_PreValidate(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_incident.EntityName)
                {
                    
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PreValidate : " + ex.Message.ToString());
            }
        }

        public void Form_OnCreate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_incident.EntityName)
                {
                    DateTime createdOn = ((DateTime)entity.Attributes["createdon"]).ToLocalTime();

                    //Generate New Running Number
                    string newRunningNumber = _BL_trs_runningnumber.GenerateNewRunningNumber(organizationService, pluginExceptionContext, _DL_incident.EntityName, createdOn);
                    if (entity.Attributes.Contains("ticketnumber"))
                        entity.Attributes["ticketnumber"] = newRunningNumber;
                    else
                        entity.Attributes.Add("ticketnumber", newRunningNumber);

                    //Get Contract Information
                    Entity eContract = new Entity();
                    if (entity.Attributes.Contains("contractid"))
                    {
                        if (entity.Attributes.Contains("contractdetailid"))
                        {
                            eContract = _DL_contract.Select(organizationService, entity.GetAttributeValue<EntityReference>("contractid").Id);
                        }
                        else
                        {
                            throw new InvalidPluginExecutionException("Please fill Contract Detail !");
                        }
                    }
                    else
                    {
                        QueryExpression queryExpression = new QueryExpression(_DL_contract.EntityName);
                        queryExpression.ColumnSet = new ColumnSet(true);

                        LinkEntity leContractDetail = new LinkEntity();
                        leContractDetail.LinkFromEntityName = _DL_contract.EntityName;
                        leContractDetail.LinkFromAttributeName = "contractid";
                        leContractDetail.LinkToEntityName = _DL_contractdetail.EntityName;
                        leContractDetail.LinkToAttributeName = "contractid";
                        leContractDetail.JoinOperator = JoinOperator.Inner;
                        leContractDetail.EntityAlias = "contractdetail";
                        leContractDetail.Columns = new ColumnSet(true);

                        LinkEntity leContractDetailEquipment = new LinkEntity();
                        leContractDetailEquipment.LinkFromEntityName = _DL_contractdetail.EntityName;
                        leContractDetailEquipment.LinkFromAttributeName = "contractdetailid";
                        leContractDetailEquipment.LinkToEntityName = _DL_trs_populationincontract.EntityName;
                        leContractDetailEquipment.LinkToAttributeName = "trs_contractline";
                        leContractDetailEquipment.JoinOperator = JoinOperator.Inner;
                        leContractDetailEquipment.EntityAlias = "contractequipment";
                        leContractDetailEquipment.Columns = new ColumnSet(true);
                        leContractDetailEquipment.LinkCriteria.AddCondition("trs_equipment", ConditionOperator.Equal, entity.GetAttributeValue<EntityReference>("trs_unit").Id);

                        leContractDetail.LinkEntities.Add(leContractDetailEquipment);
                        queryExpression.LinkEntities.Add(leContractDetail);
                        FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                        filterExpression.AddCondition("statecode", ConditionOperator.Equal, 2);
                        filterExpression.AddCondition("statuscode", ConditionOperator.Equal, 3);
                        filterExpression.AddCondition("activeon", ConditionOperator.GreaterEqual, createdOn);
                        filterExpression.AddCondition("expireson", ConditionOperator.LessEqual, createdOn);

                        EntityCollection ecolContract = _DL_contract.Select(organizationService, queryExpression);
                        if (ecolContract.Entities.Count > 0)
                        {
                            eContract = ecolContract.Entities[0];

                            if (entity.Attributes.Contains("contractid"))
                                entity.Attributes["contractid"] = new EntityReference(_DL_contract.EntityName, eContract.GetAttributeValue<Guid>("contractid"));
                            else
                                entity.Attributes.Add("contractid", new EntityReference(_DL_contract.EntityName, eContract.GetAttributeValue<Guid>("contractid")));
                            if (entity.Attributes.Contains("contractdetailid"))
                                entity.Attributes["contractdetailid"] = new EntityReference(_DL_contractdetail.EntityName, (Guid)eContract.GetAttributeValue<AliasedValue>("contractdetail.contractdetailid").Value);
                            else
                                entity.Attributes.Add("contractdetailid", new EntityReference(_DL_contractdetail.EntityName, (Guid)eContract.GetAttributeValue<AliasedValue>("contractdetail.contractdetailid").Value));
                        }
                    }

                    //Fill OTOS and OTS
                    if (eContract.Attributes.Contains("trs_otos"))
                    {
                        int contractOTOS = (int)eContract.Attributes["trs_otos"];
                        if (entity.Attributes.Contains("responseby"))
                            entity.Attributes["responseby"] = createdOn.AddMinutes(contractOTOS);
                        else
                            entity.Attributes.Add("responseby", createdOn.AddMinutes(contractOTOS));
                    }
                    if (eContract.Attributes.Contains("trs_ots"))
                    {
                        int contractOTS = (int)eContract.Attributes["trs_ots"];
                        if (entity.Attributes.Contains("resolveby"))
                            entity.Attributes["resolveby"] = createdOn.AddDays(contractOTS);
                        else
                            entity.Attributes.Add("resolveby", createdOn.AddDays(contractOTS));
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

        public void Form_OnCreate(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                
                if (entity.LogicalName == _DL_incident.EntityName)
                {
                    SendtoMobile(organizationService, entity.Id);
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
                if (entity.LogicalName == _DL_incident.EntityName)
                {
                    SendtoMobile(organizationService, entity.Id);
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

        #region Fields
        #endregion
        #endregion
    }
}
