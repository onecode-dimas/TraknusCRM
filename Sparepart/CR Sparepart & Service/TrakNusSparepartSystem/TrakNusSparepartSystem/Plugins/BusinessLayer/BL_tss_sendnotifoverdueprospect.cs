using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.Plugins.BusinessLayer
{
    public class BL_tss_sendnotifoverdueprospect
    {
        private string _classname = "BL_tss_sendnotifoverdueprospect";

        public void Form_OnCreate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext, Entity CurrentEntity,ITracingService tracer)
        {
            try
            {
                EntityReference UserEntity = (EntityReference)CurrentEntity.Attributes["tss_user"];

                QueryExpression Query = new QueryExpression(pluginExceptionContext.PrimaryEntityName);
                Query.ColumnSet = new ColumnSet(true);
                Query.Criteria.AddCondition("tss_user", ConditionOperator.Equal, UserEntity.Id);

                EntityCollection Items = organizationService.RetrieveMultiple(Query);
                if (Items.Entities.Count > 0)
                {
                    foreach (var item in Items.Entities)
                    {
                        if (CurrentEntity.Attributes.Contains("tss_user"))
                        {
                            tracer.Trace("read if condition");
                            if (UserEntity.Id == item.GetAttributeValue<EntityReference>("tss_user").Id)
                                throw new Exception("Same User is Exist....cannot create same data...!");
                        } 
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PreOperation: " + ex.Message.ToString());
            }
        }

        public void Form_OnUpdate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext, Entity CurrentEntity, ITracingService tracer)
        {
            try
            {
                EntityReference UserEntity = (EntityReference)CurrentEntity.Attributes["tss_user"];

                QueryExpression Query = new QueryExpression(pluginExceptionContext.PrimaryEntityName);
                Query.ColumnSet = new ColumnSet(true);
                Query.Criteria.AddCondition("tss_user", ConditionOperator.Equal, UserEntity.Id);

                EntityCollection Items = organizationService.RetrieveMultiple(Query);
                if (Items.Entities.Count > 0)
                {
                    foreach (var item in Items.Entities)
                    {
                        if (CurrentEntity.Attributes.Contains("tss_user") && item.Id != CurrentEntity.Id)
                        {
                            tracer.Trace("read if condition");
                            if (UserEntity.Id == item.GetAttributeValue<EntityReference>("tss_user").Id)
                                throw new Exception("Same User is Exist....cannot create same data...!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PreOperation: " + ex.Message.ToString());
            }
        }
    }
}
