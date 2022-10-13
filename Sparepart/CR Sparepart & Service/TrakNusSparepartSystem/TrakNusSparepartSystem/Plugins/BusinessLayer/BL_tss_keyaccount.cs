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
    public class BL_tss_keyaccount
    {
        #region Constants
        private const string _classname = "BL_tss_keyaccount";
        private const string _entityname = "tss_keyaccount";
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
                    QueryExpression qExpression = new QueryExpression(_entityname)
                    {
                        ColumnSet = new ColumnSet(true),
                        Criteria =
                        {
                            Filters =
                            {
                                new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("tss_customer", ConditionOperator.Equal, ((EntityReference)entity.Attributes["tss_customer"]).Id),
                                        new ConditionExpression("tss_pss", ConditionOperator.Equal, ((EntityReference)entity.Attributes["tss_pss"]).Id),
                                        new ConditionExpression("tss_status", ConditionOperator.NotIn,new int[] { 865920003, 865920004 }),
                                    }
                                },
                                //new FilterExpression
                                //{
                                //    FilterOperator = LogicalOperator.Or,
                                //    Conditions =
                                //    {
                                //        new ConditionExpression("tss_status", ConditionOperator.Equal, ((EntityReference)entity.Attributes["tss_customer"]).Id),
                                //    }
                                //}
                            }
                        }
                    };

                    EntityCollection ENExpression = organizationService.RetrieveMultiple(qExpression);

                    if (ENExpression.Entities.Count == 0)
                    {
                        DateTime createdOn = DateTime.Now.ToLocalTime();
                        //string categoryCode = "03";
                        //Generate New Running Number
                        string newRunningNumber = _BL_trs_runningnumber.GenerateNewRunningNumberModulMarketSize(
                            organizationService, pluginExceptionContext, _entityname, createdOn);
                        if (entity.Attributes.Contains("tss_kamsid"))
                            entity.Attributes["tss_kamsid"] = newRunningNumber;
                        else
                            entity.Attributes.Add("tss_kamsid", newRunningNumber);
                    }
                    else
                    {
                        throw new InvalidPluginExecutionException("Key Account with this Customer and PSS is already exist.");
                    }
                }
                else
                {
                    return;
                }
            }
            catch(Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PreOperation : " + ex.Message.ToString());
            }
        }

        public void Form_OnUpdate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                Entity entityToUpdate = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));

                if (entity.LogicalName == _entityname)
                {
                    //QueryExpression qExpression = new QueryExpression(_entityname)
                    //{
                    //    ColumnSet = new ColumnSet(true),
                    //    Criteria =
                    //    {
                    //        Filters =
                    //        {
                    //            new FilterExpression
                    //            {
                    //                FilterOperator = LogicalOperator.And,
                    //                Conditions =
                    //                {
                    //                    new ConditionExpression("tss_customer", ConditionOperator.Equal, ((EntityReference)entity.Attributes["tss_customer"]).Id),
                    //                    new ConditionExpression("tss_pss", ConditionOperator.Equal, ((EntityReference)entityToUpdate.Attributes["tss_pss"]).Id),
                    //                    new ConditionExpression("tss_status", ConditionOperator.NotIn,new int[] { 865920003, 865920004 }),
                    //                }
                    //            },
                    //            //new FilterExpression
                    //            //{
                    //            //    FilterOperator = LogicalOperator.Or,
                    //            //    Conditions =
                    //            //    {
                    //            //        new ConditionExpression("tss_status", ConditionOperator.Equal, ((EntityReference)entity.Attributes["tss_customer"]).Id),
                    //            //    }
                    //            //}
                    //        }
                    //    }
                    //};

                    //EntityCollection ENExpression = organizationService.RetrieveMultiple(qExpression);

                    //if (ENExpression.Entities.Count > 0)
                    //{
                    //    throw new InvalidPluginExecutionException("Key Account with this Customer and PSS is already exist.");
                    //}

                    int _active = entity.GetAttributeValue<OptionSetValue>("tss_status").Value;
                    int _status = entityToUpdate.GetAttributeValue<OptionSetValue>("tss_status").Value;

                    if (_status != 865920000)
                    {
                        if (_active != 865920001 && _active != 865920003 && _active != 865920004)
                            throw new InvalidPluginExecutionException("Key Account can only be updated or deleted when the status is OPEN or REVISED !");
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

        public void Form_OnDelete_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));

                if (entity.LogicalName == _entityname)
                {
                    //Entity keyAccount = organizationService.Retrieve("tss_keyaccount", pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));

                    //if (keyAccount.GetAttributeValue<OptionSetValue>("tss_status").Value != 865920000) //OPEN
                    //{
                    //    throw new InvalidPluginExecutionException("Key Account can only be deleted when the status is OPEN.");
                    //}

                    int _status = entity.GetAttributeValue<OptionSetValue>("tss_status").Value;

                    if (_status != 865920000 && _status != 865920004)
                    {
                        throw new InvalidPluginExecutionException("Key Account can only be updated or deleted when the status is OPEN !");
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnDelete_PreOperation : " + ex.Message.ToString());
            }
        }

        #endregion
    }
}
