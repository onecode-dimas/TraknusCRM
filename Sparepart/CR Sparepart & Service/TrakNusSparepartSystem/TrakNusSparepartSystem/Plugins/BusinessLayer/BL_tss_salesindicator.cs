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
    public class BL_tss_salesindicator
    {
        #region Dependencies
        private DL_tss_salesindicator _DL_tss_salesindicator = new DL_tss_salesindicator();
        #endregion
        #region Constant
        private const int STATUS_NEW = 865920000;
        private const int PIPELINEPHASE_PROSPECT = 865920000;
        private const int PartInterchange_STATUS_ACTIVE = 865920000;

        private string _classname = "BL_tss_salesindicator";
        private string _entityname = "tss_indicator";
        public string EntityName
        {
            get { return _entityname; }
        }
        private string _displayname = "Sales Indicator";
        public string DisplayName
        {
            get { return _displayname; }
        }
        #endregion

        #region Plugins
        //Check condition if value sales indicator for Quot. Part is > 100 (Max value 100)
        public void Form_OnCreate_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext, Entity postImg)
        {
            try
            {
                int totalvalue = 0;
                Entity entity = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));
                if (entity.LogicalName == pluginExceptionContext.PrimaryEntityName)
                {
                    if (postImg.Attributes.Contains("tss_value") && postImg.Attributes.Contains("tss_indicator"))
                    {
                        //totalvalue = postImg.GetAttributeValue<int>("tss_value");
                        QueryExpression qSalesIndicator = new QueryExpression(_entityname)
                        {
                            ColumnSet = new ColumnSet(true),
                            Criteria = new FilterExpression()
                            {
                                Conditions =
                                {
                                    new ConditionExpression("tss_value",ConditionOperator.NotNull)
                                }
                            }
                        };
                        var ecSalesIndicator = organizationService.RetrieveMultiple(qSalesIndicator);
                        foreach (var item in ecSalesIndicator.Entities)
                        {
                            _DL_tss_salesindicator.tss_indicatorvalue = item.GetAttributeValue<int>("tss_value");
                            totalvalue += _DL_tss_salesindicator.tss_indicatorvalue;
                        }
                        if (totalvalue > 100 || totalvalue < 0)
                        {
                            throw new InvalidPluginExecutionException("Cannot save the record, Total Indicator must be equal or less than 100 (Total Value: " + totalvalue + ")");                            
                        }
                    }
                    else
                    {
                        throw new InvalidPluginExecutionException("Cannot save the record, please fill the indicator information first.");
                    }
                }
                else
                {
                    throw new Exception("Wrong entity " + entity.LogicalName + " on Sales Indicator !");
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
                int totalvalue = 0;
                Entity entity = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));
                if (entity.LogicalName == pluginExceptionContext.PrimaryEntityName)
                {
                    if (postImg.Attributes.Contains("tss_value") && postImg.Attributes.Contains("tss_indicator"))
                    {
                        //totalvalue += postImg.GetAttributeValue<int>("tss_value");
                        QueryExpression qSalesIndicator = new QueryExpression(_entityname)
                        {
                            ColumnSet = new ColumnSet(true),
                            Criteria = new FilterExpression()
                            {
                                Conditions =
                                {
                                    new ConditionExpression("tss_value",ConditionOperator.NotNull)
                                }
                            }
                        };
                        var ecSalesIndicator = organizationService.RetrieveMultiple(qSalesIndicator);
                        foreach (var item in ecSalesIndicator.Entities)
                        {
                            _DL_tss_salesindicator.tss_indicatorvalue = item.GetAttributeValue<int>("tss_value");
                            totalvalue += _DL_tss_salesindicator.tss_indicatorvalue;
                        }
                        if (totalvalue > 100 || totalvalue < 0)
                        {
                            throw new InvalidPluginExecutionException("Cannot save the record, Total Indicator must be equal or less than 100");
                        }
                    }
                    else
                    {
                        throw new InvalidPluginExecutionException("Cannot save the record, please fill the indicator information first.");
                    }
                }
                else
                {
                    throw new Exception("Wrong entity " + entity.LogicalName + " on Sales Indicator !");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PostOperation: " + ex.Message.ToString());
            }
        }
        #endregion
    }
}
