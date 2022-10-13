using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using EnhancementCRM.HelperUnit;
using static EnhancementCRM.HelperUnit.Configuration;

namespace EnhancementCRM.Plugins.Business_Layer
{
    public class BL_ittn_exchangerateidr
    {
        #region Properties
        private string _classname = "BL_ittn_exchangerateidr";
        private string _entityname = "ittn_exchangerateidr";
        #endregion

        #region Constants
        private const int MonthMinus = 1;
        #endregion

        #region Depedencies
        #endregion

        public void Form_OnCreate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext, Entity CurrentEntity, ITracingService tracer)
        {
            try
            {
                #region Variables
                String ExchangeRateYear, ExchangeRateForeignCurrencyName = null, ExchangeRateMonthName;
                Guid ExchangeRateForeignCurrencyId;
                int ExchangeRateMonth;
                #endregion

                Entity ExchangeRate = (Entity)pluginExceptionContext.InputParameters["Target"];

                #region Attributes
                if (ExchangeRate.Contains("ittn_month") && ExchangeRate.Attributes["ittn_month"] != null)
                {
                    ExchangeRateMonth = ExchangeRate.GetAttributeValue<OptionSetValue>("ittn_month").Value;
                    ExchangeRateMonthEnum enums = (ExchangeRateMonthEnum)ExchangeRate.GetAttributeValue<OptionSetValue>("ittn_month").Value;
                    ExchangeRateMonthName = enums.ToString();
                }
                else
                    throw new InvalidPluginExecutionException("Exchange Rate Month is null/ empty!");

                if (ExchangeRate.Contains("ittn_year") && ExchangeRate.Attributes["ittn_year"] != null)
                    ExchangeRateYear = ExchangeRate.GetAttributeValue<String>("ittn_year");
                else
                    throw new InvalidPluginExecutionException("Exchange Rate Year is null/ empty!");

                if (ExchangeRate.Contains("ittn_forreigncurrency") && ExchangeRate.Attributes["ittn_forreigncurrency"] != null)
                {
                    ExchangeRateForeignCurrencyId = ExchangeRate.GetAttributeValue<EntityReference>("ittn_forreigncurrency").Id;

                    Entity Currency = organizationService.Retrieve("transactioncurrency", ExchangeRateForeignCurrencyId, new ColumnSet(true));

                    if (Currency.Contains("isocurrencycode") && Currency.Attributes["isocurrencycode"] != null)
                        ExchangeRateForeignCurrencyName = Currency.GetAttributeValue<String>("isocurrencycode");
                }
                else
                    throw new InvalidPluginExecutionException("Exchange Rate Foreign Currency is null/ empty!");
                #endregion

                #region Find Duplicate Exchange Rate with same Foreign Currency, Month, and Year
                QueryExpression qeER = new QueryExpression(_entityname);
                qeER.ColumnSet = new ColumnSet(true);
                FilterExpression feER = qeER.Criteria.AddFilter(LogicalOperator.And);
                feER.AddCondition("ittn_forreigncurrency", ConditionOperator.Equal, ExchangeRateForeignCurrencyId.ToString());
                feER.AddCondition("ittn_month", ConditionOperator.Equal, ExchangeRateMonth.ToString());
                feER.AddCondition("ittn_year", ConditionOperator.Equal, ExchangeRateYear.ToString());
                EntityCollection ecER = organizationService.RetrieveMultiple(qeER);

                if (ecER.Entities.Count > 0)
                {
                    #region Set Exchange Rate IDR Attributes
                    ExchangeRate.Attributes.Add("ittn_currencyname", ExchangeRateForeignCurrencyName + " - " + ExchangeRateMonthName + " - " + ExchangeRateYear);
                    #endregion
                }
                #endregion
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
                #region Variables
                String ExchangeRateYear, ExchangeRateForeignCurrencyName = null, ExchangeRateMonthName;
                Guid ExchangeRateForeignCurrencyId;
                int ExchangeRateMonth;
                #endregion

                Entity ExchangeRate = organizationService.Retrieve(_entityname, CurrentEntity.Id, new ColumnSet(true));
                Entity ExchangeRateInput = (Entity)pluginExceptionContext.InputParameters["Target"];

                #region Attributes
                if (ExchangeRateInput.Contains("ittn_month") && ExchangeRateInput.Attributes["ittn_month"] != null)
                {
                    ExchangeRateMonth = ExchangeRateInput.GetAttributeValue<OptionSetValue>("ittn_month").Value;
                    ExchangeRateMonthEnum enums = (ExchangeRateMonthEnum)ExchangeRateInput.GetAttributeValue<OptionSetValue>("ittn_month").Value;
                    ExchangeRateMonthName = enums.ToString();
                }
                else
                {
                    ExchangeRateMonth = ExchangeRate.GetAttributeValue<OptionSetValue>("ittn_month").Value;
                    ExchangeRateMonthEnum enums = (ExchangeRateMonthEnum)ExchangeRate.GetAttributeValue<OptionSetValue>("ittn_month").Value;
                    ExchangeRateMonthName = enums.ToString();
                }

                if (ExchangeRateInput.Contains("ittn_year") && ExchangeRateInput.Attributes["ittn_year"] != null)
                    ExchangeRateYear = ExchangeRateInput.GetAttributeValue<String>("ittn_year");
                else
                    ExchangeRateYear = ExchangeRate.GetAttributeValue<String>("ittn_year");

                if (ExchangeRateInput.Contains("ittn_forreigncurrency") && ExchangeRateInput.Attributes["ittn_forreigncurrency"] != null)
                {
                    ExchangeRateForeignCurrencyId = ExchangeRateInput.GetAttributeValue<EntityReference>("ittn_forreigncurrency").Id;

                    Entity Currency = organizationService.Retrieve("transactioncurrency", ExchangeRateForeignCurrencyId, new ColumnSet(true));

                    if (Currency.Contains("isocurrencycode") && Currency.Attributes["isocurrencycode"] != null)
                        ExchangeRateForeignCurrencyName = Currency.GetAttributeValue<String>("isocurrencycode");
                }
                else
                {
                    ExchangeRateForeignCurrencyId = ExchangeRate.GetAttributeValue<EntityReference>("ittn_forreigncurrency").Id;

                    Entity Currency = organizationService.Retrieve("transactioncurrency", ExchangeRateForeignCurrencyId, new ColumnSet(true));

                    if (Currency.Contains("isocurrencycode") && Currency.Attributes["isocurrencycode"] != null)
                        ExchangeRateForeignCurrencyName = Currency.GetAttributeValue<String>("isocurrencycode");
                }
                #endregion

                #region Find Duplicate Exchange Rate with same Foreign Currency, Month, and Year
                QueryExpression qeER = new QueryExpression(_entityname);
                qeER.ColumnSet = new ColumnSet(true);
                FilterExpression feER = qeER.Criteria.AddFilter(LogicalOperator.And);
                feER.AddCondition("ittn_forreigncurrency", ConditionOperator.Equal, ExchangeRateForeignCurrencyId.ToString());
                feER.AddCondition("ittn_month", ConditionOperator.Equal, ExchangeRateMonth.ToString());
                feER.AddCondition("ittn_year", ConditionOperator.Equal, ExchangeRateYear.ToString());
                EntityCollection ecER = organizationService.RetrieveMultiple(qeER);

                if (ecER.Entities.Count <= 0)
                {
                    #region Set Exchange Rate IDR Attributes
                    ExchangeRateInput.Attributes.Add("ittn_currencyname", ExchangeRateForeignCurrencyName + " - " + ExchangeRateMonthName + " - " + ExchangeRateYear);
                    #endregion
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate_PreOperation: " + ex.Message.ToString());
            }
        }

        public void Form_OnCreate_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext, Entity CurrentEntity, ITracingService tracer)
        {
            try
            {
                #region Variables
                String ExchangeRateYear, ExchangeRateForeignCurrencyName = null, ExchangeRateLocalCurrencyName = null, ExchangeRateMonthName;
                Guid ExchangeRateForeignCurrencyId, ExchangeRateLocalCurrencyId;
                int ExchangeRateMonth, PreviousMonthExchangeRate, PreviousYearExchangeRate;
                DateTime ExchangeRateDate, PreviousMonthDate;
                #endregion

                Entity ExchangeRate = organizationService.Retrieve(_entityname, CurrentEntity.Id, new ColumnSet(true));
                Entity ExchangeRateInput = (Entity)pluginExceptionContext.InputParameters["Target"];

                #region Attributes
                if (ExchangeRateInput.Contains("ittn_month") && ExchangeRateInput.Attributes["ittn_month"] != null)
                {
                    ExchangeRateMonth = ExchangeRateInput.GetAttributeValue<OptionSetValue>("ittn_month").Value;
                    ExchangeRateMonthEnum enums = (ExchangeRateMonthEnum)ExchangeRateInput.GetAttributeValue<OptionSetValue>("ittn_month").Value;
                    ExchangeRateMonthName = enums.ToString();
                }
                else
                {
                    ExchangeRateMonth = ExchangeRate.GetAttributeValue<OptionSetValue>("ittn_month").Value;
                    ExchangeRateMonthEnum enums = (ExchangeRateMonthEnum)ExchangeRate.GetAttributeValue<OptionSetValue>("ittn_month").Value;
                    ExchangeRateMonthName = enums.ToString();
                }

                if (ExchangeRateInput.Contains("ittn_year") && ExchangeRateInput.Attributes["ittn_year"] != null)
                    ExchangeRateYear = ExchangeRateInput.GetAttributeValue<String>("ittn_year");
                else
                    ExchangeRateYear = ExchangeRate.GetAttributeValue<String>("ittn_year");

                ExchangeRateDate = Convert.ToDateTime(ExchangeRateYear + "-" + ExchangeRateMonth + "-01");
                PreviousMonthDate = ExchangeRateDate.AddMonths(-MonthMinus);
                PreviousMonthExchangeRate = PreviousMonthDate.Month;
                PreviousYearExchangeRate = PreviousMonthDate.Year;

                if (ExchangeRateInput.Contains("ittn_forreigncurrency") && ExchangeRateInput.Attributes["ittn_forreigncurrency"] != null)
                {
                    ExchangeRateForeignCurrencyId = ExchangeRateInput.GetAttributeValue<EntityReference>("ittn_forreigncurrency").Id;

                    Entity Currency = organizationService.Retrieve("transactioncurrency", ExchangeRateForeignCurrencyId, new ColumnSet(true));

                    if (Currency.Contains("isocurrencycode") && Currency.Attributes["isocurrencycode"] != null)
                        ExchangeRateForeignCurrencyName = Currency.GetAttributeValue<String>("isocurrencycode");
                }
                else
                {
                    ExchangeRateForeignCurrencyId = ExchangeRate.GetAttributeValue<EntityReference>("ittn_forreigncurrency").Id;

                    Entity Currency = organizationService.Retrieve("transactioncurrency", ExchangeRateForeignCurrencyId, new ColumnSet(true));

                    if (Currency.Contains("isocurrencycode") && Currency.Attributes["isocurrencycode"] != null)
                        ExchangeRateForeignCurrencyName = Currency.GetAttributeValue<String>("isocurrencycode");
                }

                if (ExchangeRateInput.Contains("transactioncurrencyid") && ExchangeRateInput.Attributes["transactioncurrencyid"] != null)
                {
                    ExchangeRateLocalCurrencyId = ExchangeRateInput.GetAttributeValue<EntityReference>("transactioncurrencyid").Id;

                    Entity Currency = organizationService.Retrieve("transactioncurrency", ExchangeRateLocalCurrencyId, new ColumnSet(true));

                    if (Currency.Contains("isocurrencycode") && Currency.Attributes["isocurrencycode"] != null)
                        ExchangeRateLocalCurrencyName = Currency.GetAttributeValue<String>("isocurrencycode");
                }
                else
                {
                    ExchangeRateLocalCurrencyId = ExchangeRate.GetAttributeValue<EntityReference>("transactioncurrencyid").Id;

                    Entity Currency = organizationService.Retrieve("transactioncurrency", ExchangeRateLocalCurrencyId, new ColumnSet(true));

                    if (Currency.Contains("isocurrencycode") && Currency.Attributes["isocurrencycode"] != null)
                        ExchangeRateLocalCurrencyName = Currency.GetAttributeValue<String>("isocurrencycode");
                }
                #endregion

                #region Find Previous Exchange Rate
                QueryExpression qeER = new QueryExpression(_entityname);
                qeER.ColumnSet = new ColumnSet(true);
                FilterExpression feER = qeER.Criteria.AddFilter(LogicalOperator.And);
                feER.AddCondition("ittn_month", ConditionOperator.Equal, PreviousMonthExchangeRate.ToString());
                feER.AddCondition("ittn_year", ConditionOperator.Equal, PreviousYearExchangeRate.ToString());
                feER.AddCondition("ittn_forreigncurrency", ConditionOperator.Equal, ExchangeRateForeignCurrencyId.ToString());
                feER.AddCondition("transactioncurrencyid", ConditionOperator.Equal, ExchangeRateLocalCurrencyId.ToString());
                EntityCollection ecER = organizationService.RetrieveMultiple(qeER);

                if (ecER.Entities.Count > 0)
                {
                    foreach (var enER in ecER.Entities)
                    {
                        #region Update Related Price List Unit Original
                        QueryExpression qeprice = new QueryExpression("ittn_pricelistunitoriginal");
                        qeprice.ColumnSet = new ColumnSet(true);
                        FilterExpression feprice = qeprice.Criteria.AddFilter(LogicalOperator.And);
                        feprice.AddCondition("ittn_exchangerate", ConditionOperator.Equal, enER.Id.ToString());
                        EntityCollection ecprice = organizationService.RetrieveMultiple(qeprice);

                        if (ecprice.Entities.Count > 0)
                        {
                            foreach (var enprice in ecprice.Entities)
                            {
                                Entity PriceListUpdate = new Entity("ittn_pricelistunitoriginal");
                                PriceListUpdate["ittn_exchangerate"] = new EntityReference(_entityname, ExchangeRate.Id);
                                PriceListUpdate.Id = enprice.Id;
                                organizationService.Update(PriceListUpdate);
                            }
                        }
                        #endregion

                        #region Update Related Minimum Price
                        QueryExpression qeminprice = new QueryExpression("new_pricelist");
                        qeminprice.ColumnSet = new ColumnSet(true);
                        FilterExpression feminprice = qeminprice.Criteria.AddFilter(LogicalOperator.And);
                        feminprice.AddCondition("ittn_exchangerate", ConditionOperator.Equal, enER.Id.ToString());
                        EntityCollection ecminprice = organizationService.RetrieveMultiple(qeminprice);

                        if (ecminprice.Entities.Count > 0)
                        {
                            foreach (var enminprice in ecminprice.Entities)
                            {
                                Entity MinPriceUpdate = new Entity("new_pricelist");
                                MinPriceUpdate["ittn_exchangerate"] = new EntityReference(_entityname, ExchangeRate.Id);
                                MinPriceUpdate.Id = enminprice.Id;
                                organizationService.Update(MinPriceUpdate);
                            }
                        }
                        #endregion
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PostOperation: " + ex.Message.ToString());
            }
        }
    }
}
