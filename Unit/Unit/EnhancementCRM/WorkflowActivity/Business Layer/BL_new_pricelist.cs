using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;
using System.Activities;

namespace EnhancementCRM.WorkflowActivity.Business_Layer
{
    public class BL_new_pricelist
    {
        #region Properties
        private string _classname = "BL_new_pricelist";
        #endregion

        #region Constants
        private string _prefixdata = "Min Price";
        #endregion

        #region Depedencies
        #endregion

        #region Publics
        public void MinimumPrice(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {
            try
            {
                #region Variables
                String ProductName, ExchangeRateName, ExchangeRateYear;
                Guid ExchangeRateId;
                int ExchangeRateMonth;
                Decimal ExchangeRateValue, PriceOriginal;
                #endregion

                Entity MinPrice = organizationService.Retrieve(context.PrimaryEntityName, context.PrimaryEntityId, new ColumnSet(true));

                #region Attributes
                if (MinPrice.Contains("new_item") && MinPrice.Attributes["new_item"] != null)
                    ProductName = MinPrice.GetAttributeValue<EntityReference>("new_item").Name;
                else
                    throw new InvalidWorkflowException("Item is null/ empty!");

                if (MinPrice.Contains("ittn_exchangerate") && MinPrice.Attributes["ittn_exchangerate"] != null)
                {
                    ExchangeRateId = MinPrice.GetAttributeValue<EntityReference>("ittn_exchangerate").Id;
                    ExchangeRateName = MinPrice.GetAttributeValue<EntityReference>("ittn_exchangerate").Name;

                    Entity ExchangeRate = organizationService.Retrieve("ittn_exchangerateidr", ExchangeRateId, new ColumnSet(true));

                    if (ExchangeRate.Contains("ittn_month") && ExchangeRate.Attributes["ittn_month"] != null)
                        ExchangeRateMonth = ExchangeRate.GetAttributeValue<OptionSetValue>("ittn_month").Value;
                    else
                        throw new InvalidWorkflowException("Exchange Rate Month is null/ empty!");

                    if (ExchangeRate.Contains("ittn_year") && ExchangeRate.Attributes["ittn_year"] != null)
                        ExchangeRateYear = ExchangeRate.GetAttributeValue<String>("ittn_year");
                    else
                        throw new InvalidWorkflowException("Exchange Rate Year is null/ empty!");

                    if (ExchangeRate.Contains("ittn_ratevalue") && ExchangeRate.Attributes["ittn_ratevalue"] != null)
                        ExchangeRateValue = ExchangeRate.GetAttributeValue<Money>("ittn_ratevalue").Value;
                    else
                        throw new InvalidWorkflowException("Exchange Rate Value is null/ empty!");
                }
                else
                    throw new InvalidWorkflowException("Exchange Rate is null/ empty!");

                if (MinPrice.Contains("ittn_originalminprice") && MinPrice.Attributes["ittn_originalminprice"] != null)
                    PriceOriginal = MinPrice.GetAttributeValue<decimal>("ittn_originalminprice");
                else
                    throw new InvalidWorkflowException("Original Minimum Price is null/ empty!");
                #endregion

                #region Set Minimum Price Attributes
                Entity MinPriceUpdate = new Entity(context.PrimaryEntityName);
                MinPriceUpdate["new_name"] = _prefixdata + " - " + ProductName + " - " + ExchangeRateName;
                MinPriceUpdate["ittn_exchangeratevalue"] = Convert.ToDecimal(ExchangeRateValue);
                MinPriceUpdate["ittn_exchangeratemonth"] = new OptionSetValue(ExchangeRateMonth);
                MinPriceUpdate["ittn_exchangerateyear"] = ExchangeRateYear;
                MinPriceUpdate["new_minimumprice"] = new Money(PriceOriginal * Convert.ToDecimal(ExchangeRateValue));

                MinPriceUpdate.Id = context.PrimaryEntityId;
                organizationService.Update(MinPriceUpdate);
                #endregion
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".MinimumPrice : " + ex.Message.ToString());
            }

        }
        #endregion
    }
}
