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
    public class BL_tss_kagroupuiocommodity
    {
        #region Constants
        private const string _classname = "BL_tss_kagroupuiocommodity";
        private const string _entityname = "tss_kagroupuiocommodity";
        #endregion

        #region Depedencies
        private BL_trs_runningnumber _BL_trs_runningnumber = new BL_trs_runningnumber();
        private BL_tss_population _BL_tss_population = new BL_tss_population();
        #endregion

        #region CREATE
        public void Form_OnCreate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];

                if (entity.LogicalName == _entityname)
                {
                    Entity keyAccount = organizationService.Retrieve("tss_keyaccount", entity.GetAttributeValue<EntityReference>("tss_keyaccountid").Id, new ColumnSet(true));

                    if (keyAccount.GetAttributeValue<OptionSetValue>("tss_status").Value != 865920000) //OPEN
                    {
                        throw new InvalidPluginExecutionException("KA Group UIO Commodity can only be created when the status Key Account is OPEN.");
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
        #endregion

        #region UPDATE
        public void Form_OnUpdate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));

                if (entity.LogicalName == _entityname)
                {
                    Entity keyAccount = organizationService.Retrieve("tss_keyaccount", entity.GetAttributeValue<EntityReference>("tss_keyaccountid").Id, new ColumnSet(true));

                    if (keyAccount.GetAttributeValue<OptionSetValue>("tss_status").Value != 865920000) //OPEN
                    {
                        throw new InvalidPluginExecutionException("KA Group UIO Commodity can only be updated when the status Key Account is OPEN.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate_PreOperation : " + ex.Message.ToString());
            }
        }
        #endregion

        #region DELETE
        public void Form_OnDelete_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                try
                {
                    Entity entity = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));

                    if (entity.LogicalName == _entityname)
                    {
                        Entity keyAccount = organizationService.Retrieve("tss_keyaccount", entity.GetAttributeValue<EntityReference>("tss_keyaccountid").Id, new ColumnSet(true));

                        if (keyAccount.GetAttributeValue<OptionSetValue>("tss_status").Value != 865920000) //OPEN
                        {
                            throw new InvalidPluginExecutionException("KA Group UIO Commodity can only be deleted when the status Key Account is OPEN.");
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
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnDelete_PreOperation : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}