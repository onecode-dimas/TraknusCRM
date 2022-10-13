using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.Plugins.BusinessLayer
{
    public class BL_tss_rating
    {
        private string _classname = "BL_tss_rating";
        private string _entityname = "tss_rating";

        public void Form_OnCreate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext, Entity CurrentEntity, ITracingService tracer)
        {
            try
            {
                QueryExpression Query = new QueryExpression(pluginExceptionContext.PrimaryEntityName);  
                Query.ColumnSet = new ColumnSet(true);
                Query.Criteria.AddCondition("tss_isoverdue", ConditionOperator.Equal, true);

                EntityCollection Items = organizationService.RetrieveMultiple(Query);
                if (Items.Entities.Count > 0)
                {
                    if (CurrentEntity.Contains("tss_isoverdue"))
                    {
                        if ((bool)CurrentEntity.Attributes["tss_isoverdue"] == true)
                            throw new Exception("IsOverdue with Yes Value is Exist....cannot create same data...!");
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
                QueryExpression Query = new QueryExpression(pluginExceptionContext.PrimaryEntityName);
                Query.ColumnSet = new ColumnSet(true);
                Query.Criteria.AddCondition("tss_isoverdue", ConditionOperator.Equal, true);

                EntityCollection Items = organizationService.RetrieveMultiple(Query);
                if (Items.Entities.Count > 0)
                {
                    foreach (var item in Items.Entities )
                    {
                        if (CurrentEntity.Contains("tss_isoverdue") && item.Id != CurrentEntity.Id)
                        {
                            if ((bool)CurrentEntity.Attributes["tss_isoverdue"] == true)
                                throw new Exception("IsOverdue with Yes Value is Exist....cannot create same data...!");
                        } 
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate_PreOperation: " + ex.Message.ToString());
            }
        }

        public void Form_OnCreate_PreOperation_Period(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext, Entity CurrentEntity, ITracingService tracer)
        {
            try
            {
                var WEEK = 865920000;
                var MONTH = 865920001;
                var OPTION_VALUE = 0;

                OptionSetValue PeriodeOption = (OptionSetValue)CurrentEntity.Attributes["tss_period"];

                if (PeriodeOption.Value == WEEK)
                    OPTION_VALUE = MONTH;
                else
                    OPTION_VALUE = WEEK;

                QueryExpression Query = new QueryExpression(pluginExceptionContext.PrimaryEntityName);
                Query.ColumnSet = new ColumnSet(true);
                Query.Criteria.AddCondition("tss_period", ConditionOperator.Equal, OPTION_VALUE);

                EntityCollection Items = organizationService.RetrieveMultiple(Query);
                if (Items.Entities.Count > 0)
                {
                    foreach (var item in Items.Entities)
                    {
                        if (CurrentEntity.Contains("tss_period"))
                        {
                            tracer.Trace("read if condition");
                            if (PeriodeOption.Value != item.GetAttributeValue<OptionSetValue>("tss_period").Value)
                                throw new Exception("Cannot save data, You must select Period with the same of Existing Data Value");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PreOperation_Period: " + ex.Message.ToString());
            }
        }

        public void Form_OnUpdate_PreOperation_Period(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext, Entity CurrentEntity, ITracingService tracer)
        {
            try
            {
                var WEEK = 865920000;
                var MONTH = 865920001;
                var OPTION_VALUE = 0;

                if (CurrentEntity.Contains("tss_period") && CurrentEntity.Attributes["tss_period"] !=null)
                {
                    OptionSetValue PeriodeOption = (OptionSetValue)CurrentEntity.Attributes["tss_period"];

                    if (PeriodeOption.Value == WEEK)
                        OPTION_VALUE = MONTH;
                    else
                        OPTION_VALUE = WEEK;

                    QueryExpression Query = new QueryExpression(pluginExceptionContext.PrimaryEntityName);
                    Query.ColumnSet = new ColumnSet(true);
                    Query.Criteria.AddCondition("tss_period", ConditionOperator.Equal, OPTION_VALUE);

                    EntityCollection Items = organizationService.RetrieveMultiple(Query);
                    if (Items.Entities.Count > 0)
                    {
                        foreach (var item in Items.Entities)
                        {
                            if (CurrentEntity.Contains("tss_period") && CurrentEntity.Id != item.Id)
                            {
                                tracer.Trace("read if condition");
                                if (PeriodeOption.Value != item.GetAttributeValue<OptionSetValue>("tss_period").Value)
                                    throw new Exception("Cannot save data, You must select Period with the same of Existing Data Value");
                            }
                        }
                    }
                    else
                    {
                        tracer.Trace("No data");
                    }

                    tracer.Trace("Finish Check data");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate_PreOperation_Period: " + ex.Message.ToString());
            }
        }
    }
}
