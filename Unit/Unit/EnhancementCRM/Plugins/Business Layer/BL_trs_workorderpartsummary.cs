using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhancementCRM.Plugins.Business_Layer
{
    public class BL_trs_workorderpartsummary
    {
        #region Constants
        private const string _classname = "BL_trs_workorderpartsummary";
        private const int STATECODE_RELEASED = 0;
        private const int STATUSCODE_RELEASED = 2;
        private const string ENTITY_WO = "serviceappointment";
        #endregion

        #region Events
        public void Form_OnCreate_PostOperation_AddPart(IOrganizationService organizationService, IPluginExecutionContext pluginExecutionContext)
        {
            try
            {
                Entity entity = organizationService.Retrieve(pluginExecutionContext.PrimaryEntityName, pluginExecutionContext.PrimaryEntityId, new ColumnSet(true));

                if (entity.LogicalName == pluginExecutionContext.PrimaryEntityName)
                {
                    if (entity.Attributes.Contains("trs_workorder") && entity.Attributes["trs_workorder"] != null)
                    {
                        Entity WO = organizationService.Retrieve(ENTITY_WO, entity.GetAttributeValue<EntityReference>("trs_workorder").Id, new ColumnSet(true));

                        if (WO.Attributes.Contains("new_sapwonumber") && WO.Attributes["new_sapwonumber"] != null
                            && WO.GetAttributeValue<OptionSetValue>("statuscode").Value >= STATUSCODE_RELEASED)
                        {
                            if ((!WO.Attributes.Contains("ittn_needapprovepartchanges") && WO.Attributes["ittn_needapprovepartchanges"] == null)
                                || (WO.Attributes.Contains("ittn_needapprovepartchanges") && WO.Attributes["ittn_needapprovepartchanges"] != null && WO.GetAttributeValue<Boolean>("ittn_needapprovepartchanges") == false))
                            {
                                WO["ittn_needapprovepartchanges"] = true;
                                organizationService.Update(WO);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PostOperation_AddPart: " + ex.Message.ToString());
            }
        }

        public void Form_OnDelete_PostOperation_PartDelete(IOrganizationService organizationService, IPluginExecutionContext pluginExecutionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExecutionContext.PreEntityImages["PreImage"];
                if (entity.LogicalName == pluginExecutionContext.PrimaryEntityName)
                {
                    if (entity.Attributes.Contains("trs_workorder") && entity.Attributes["trs_workorder"] != null)
                    {
                        Entity WO = organizationService.Retrieve(ENTITY_WO, entity.GetAttributeValue<EntityReference>("trs_workorder").Id, new ColumnSet(true));

                        if (WO.Attributes.Contains("new_sapwonumber") && WO.Attributes["new_sapwonumber"] != null
                            && WO.GetAttributeValue<OptionSetValue>("statuscode").Value >= STATUSCODE_RELEASED)
                        {
                            if ((!WO.Attributes.Contains("ittn_needapprovepartchanges") && WO.Attributes["ittn_needapprovepartchanges"] == null)
                                || (WO.Attributes.Contains("ittn_needapprovepartchanges") && WO.Attributes["ittn_needapprovepartchanges"] != null && WO.GetAttributeValue<Boolean>("ittn_needapprovepartchanges") == false))
                            {
                                WO["ittn_needapprovepartchanges"] = true;
                                organizationService.Update(WO);
                            }
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
                throw new InvalidPluginExecutionException(_classname + ".Form_OnDelete_PostOperation_PartDelete : " + ex.Message.ToString());
            }
        }

        public void Form_OnUpdate_ManualQuantity_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExecutionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExecutionContext.InputParameters["Target"];
                Entity pre = (Entity)pluginExecutionContext.PreEntityImages["PreImage"];

                if (entity.LogicalName == pluginExecutionContext.PrimaryEntityName)
                {
                    if (pre.Attributes.Contains("trs_workorder") && pre.Attributes["trs_workorder"] != null)
                    {
                        Entity WO = organizationService.Retrieve(ENTITY_WO, pre.GetAttributeValue<EntityReference>("trs_workorder").Id, new ColumnSet(true));

                        if (WO.Attributes.Contains("new_sapwonumber") && WO.Attributes["new_sapwonumber"] != null
                           && WO.GetAttributeValue<OptionSetValue>("statuscode").Value >= STATUSCODE_RELEASED)
                        {
                            if (entity.Attributes.Contains("trs_manualquantity") && entity.Attributes["trs_manualquantity"] != null
                                && entity.GetAttributeValue<Int32>("trs_manualquantity") != pre.GetAttributeValue<Int32>("trs_manualquantity"))
                            {
                                if ((!WO.Attributes.Contains("ittn_needapprovepartchanges") && WO.Attributes["ittn_needapprovepartchanges"] == null)
                                || (WO.Attributes.Contains("ittn_needapprovepartchanges") && WO.Attributes["ittn_needapprovepartchanges"] != null && WO.GetAttributeValue<Boolean>("ittn_needapprovepartchanges") == false))
                                {
                                    WO["ittn_needapprovepartchanges"] = true;
                                    organizationService.Update(WO);
                                }
                            }
                            else
                                throw new InvalidPluginExecutionException("here");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate_ManualQuantity_PostOperation : " + ex.Message.ToString());
            }
        }

        public void Form_OnUpdate_ManualQuantity_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExecutionContext, Entity _entity, ITracingService _tracer)
        {
            try
            {
                Entity pre = organizationService.Retrieve(_entity.LogicalName, _entity.Id, new ColumnSet(true));

                if (pre.Contains("trs_confirmsupply") && pre.GetAttributeValue<bool>("trs_confirmsupply") == true
                    && (!pre.Contains("trs_confirmreturn") || (pre.Contains("trs_confirmreturn") && pre.GetAttributeValue<bool>("trs_confirmreturn") == false)))
                {
                    throw new InvalidPluginExecutionException("You need to Confirm Return for this part number before change the quantity!");
                }
                else
                {
                    Entity WO = organizationService.Retrieve(ENTITY_WO, pre.GetAttributeValue<EntityReference>("trs_workorder").Id, new ColumnSet(true));
                    WO["ittn_needapprovepartchanges"] = true;
                    organizationService.Update(WO);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate_ManualQuantity_PreOperation: " + ex.Message.ToString());
            }
        }
        #endregion
    }
}
