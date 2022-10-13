using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.Plugins.BusinessLayer
{
    public class BL_tss_matrixtyreconsump
    {
        #region Constant
        private string _classname = "BL_tss_matrixtyreconsump";
        private string _entityname = "tss_matrixtyreconsump";

        public string EntityName
        {
            get { return _entityname; }
        }
        #endregion

        #region Plugins
        public void Form_OnCreate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExcecutionContext, Entity CurrentEntity, ITracingService tracer)
        {
            try
            {
                if (CurrentEntity.Contains("tss_code"))
                {
                    string Code = CurrentEntity.Attributes["tss_code"].ToString();
                    QueryExpression Query = new QueryExpression(pluginExcecutionContext.PrimaryEntityName);
                    Query.ColumnSet = new ColumnSet(true);
                    Query.Criteria.AddCondition("tss_code", ConditionOperator.Equal, Code);

                    EntityCollection Items = organizationService.RetrieveMultiple(Query);
                    if (Items.Entities.Count() > 0)
                    {
                        throw new Exception("Code already exist. Cannot create record with same code.");
                    }
                }
            }
            catch(Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PreOperation: " + ex.Message.ToString());
            }
        }
        public void Form_OnUpdate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExcecutionContext, Entity CurrentEntity, ITracingService tracer)
        {
            try
            {
                if (CurrentEntity.Contains("tss_code"))
                {
                    string Code = CurrentEntity.Attributes["tss_code"].ToString();
                    QueryExpression Query = new QueryExpression(pluginExcecutionContext.PrimaryEntityName);
                    Query.ColumnSet = new ColumnSet(true);
                    Query.Criteria.AddCondition("tss_code", ConditionOperator.Equal, Code);

                    EntityCollection Items = organizationService.RetrieveMultiple(Query);
                    if (Items.Entities.Count() > 0)
                    {
                        throw new Exception("Code already exist. Cannot update record with same code.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate_PreOperation: " + ex.Message.ToString());
            }
        }
        #endregion
    }
}
