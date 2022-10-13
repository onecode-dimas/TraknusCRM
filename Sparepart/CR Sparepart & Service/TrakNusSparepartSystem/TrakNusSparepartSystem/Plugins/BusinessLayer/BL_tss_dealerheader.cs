using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using TrakNusSparepartSystem.DataLayer;

namespace TrakNusSparepartSystem.Plugins.BusinessLayer
{
    public  class BL_tss_dealerheader
    {
        #region Constant
        private string _classname = "BL_tss_dealerheader";
        private string _entityname = "tss_dealerheader";

        public string EntityName
        {
            get { return _entityname; }
        }
        #endregion

        #region Plugins
        public void Form_OnCreate_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext, Entity postImg)
        {
            try
            {
                
                Entity entity = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));
                if (entity.LogicalName == pluginExceptionContext.PrimaryEntityName)
                {
                    if (postImg.Attributes.Contains("tss_dealername") && postImg.Attributes.Contains("tss_dealername"))
                    {
                        string dealerName = postImg.GetAttributeValue<EntityReference>("tss_dealername").Name;
                        Entity _entity = new Entity(_entityname);
                        _entity.Id = pluginExceptionContext.PrimaryEntityId;
                        _entity.Attributes["tss_name"] = dealerName;
                        organizationService.Update(_entity);
                    }
                    else
                    {
                        throw new InvalidPluginExecutionException("Cannot save the record, please fill the indicator information first.");
                    }
                }
                else
                {
                    throw new Exception("Wrong entity " + entity.LogicalName + " on Dealer Header !");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PostOperation: " + ex.Message.ToString());
            }
        }

        public void Form_OnUpdate_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext, Entity postImg)
        {
            try
            {

                Entity entity = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));
                if (entity.LogicalName == pluginExceptionContext.PrimaryEntityName)
                {
                    if (postImg.Attributes.Contains("tss_dealername") && postImg.Attributes.Contains("tss_dealername"))
                    {
                        string dealerName = postImg.GetAttributeValue<EntityReference>("tss_dealername").Name;
                        Entity _entity = new Entity(_entityname);
                        _entity.Id = pluginExceptionContext.PrimaryEntityId;
                        _entity.Attributes["tss_name"] = dealerName;
                        organizationService.Update(_entity);
                    }
                    else
                    {
                        throw new InvalidPluginExecutionException("Cannot save the record, please fill the indicator information first.");
                    }
                }
                else
                {
                    throw new Exception("Wrong entity " + entity.LogicalName + " on Dealer Header !");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate_PostOperation: " + ex.Message.ToString());
            }
        }
        #endregion
    }
}
