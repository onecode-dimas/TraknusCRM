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
    public class BL_ittn_pricelistunitoriginal
    {
        #region Properties
        private string _classname = "BL_ittn_pricelistunitoriginal";
        #endregion

        #region Constants
        #endregion

        #region Depedencies
        #endregion

        #region Publics
        public void PriceListUnitOriginal(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {
            try
            {
                #region Variables
                String ProductName, ExchangeRateName, ExchangeRateYear;
                Guid ExchangeRateId, ExchangeRateLocalCurrencyId;
                int ExchangeRateMonth;
                Decimal ExchangeRateValue, PriceOriginal;
                #endregion

                Entity PriceList = organizationService.Retrieve(context.PrimaryEntityName, context.PrimaryEntityId, new ColumnSet(true));

                #region Attributes
                if (PriceList.Contains("ittn_product") && PriceList.Attributes["ittn_product"] != null)
                    ProductName = PriceList.GetAttributeValue<EntityReference>("ittn_product").Name;
                else
                    throw new InvalidWorkflowException("Product is null/ empty!");

                if (PriceList.Contains("ittn_exchangerate") && PriceList.Attributes["ittn_exchangerate"] != null)
                {
                    ExchangeRateId = PriceList.GetAttributeValue<EntityReference>("ittn_exchangerate").Id;
                    ExchangeRateName = PriceList.GetAttributeValue<EntityReference>("ittn_exchangerate").Name;

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

                    if (ExchangeRate.Contains("transactioncurrencyid") && ExchangeRate.Attributes["transactioncurrencyid"] != null)
                        ExchangeRateLocalCurrencyId = ExchangeRate.GetAttributeValue<EntityReference>("transactioncurrencyid").Id;
                    else
                        throw new InvalidWorkflowException("Exchange Rate Local Currency is null/ empty!");
                }
                else
                    throw new InvalidWorkflowException("Exchange Rate is null/ empty!");

                if (PriceList.Contains("ittn_priceoriginal") && PriceList.Attributes["ittn_priceoriginal"] != null)
                    PriceOriginal = PriceList.GetAttributeValue<Money>("ittn_priceoriginal").Value;
                else
                    throw new InvalidWorkflowException("Price Original is null/ empty!");
                #endregion

                #region Set Price List Attributes
                Entity PriceListUpdate = new Entity(context.PrimaryEntityName);
                PriceListUpdate["ittn_prielistunitname"] = ProductName + " - " + ExchangeRateName;
                PriceListUpdate["ittn_exchangeratevalue"] = Convert.ToDecimal(ExchangeRateValue);
                PriceListUpdate["ittn_exchangeratemonth"] = new OptionSetValue(ExchangeRateMonth);
                PriceListUpdate["ittn_exchangerateyear"] = ExchangeRateYear;
                PriceListUpdate["ittn_localprice"] = Convert.ToDecimal(PriceOriginal) * Convert.ToDecimal(ExchangeRateValue);

                PriceListUpdate.Id = context.PrimaryEntityId;
                organizationService.Update(PriceListUpdate);
                #endregion

                #region Update Price List Original
                QueryExpression qe = new QueryExpression("productpricelevel");
                qe.ColumnSet = new ColumnSet(true);
                FilterExpression fe = qe.Criteria.AddFilter(LogicalOperator.And);
                fe.AddCondition("transactioncurrencyid", ConditionOperator.Equal, ExchangeRateLocalCurrencyId.ToString());
                fe.AddCondition("productnumber", ConditionOperator.Equal, ProductName);
                EntityCollection ec = organizationService.RetrieveMultiple(qe);

                if (ec.Entities.Count > 0)
                {
                    foreach (var en in ec.Entities)
                    {
                        Entity UpdatePriceListOriginal = new Entity("productpricelevel");
                        UpdatePriceListOriginal["amount"] = new Money(Convert.ToDecimal(PriceOriginal) * Convert.ToDecimal(ExchangeRateValue));
                        UpdatePriceListOriginal.Id = en.Id;
                        organizationService.Update(UpdatePriceListOriginal);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".PriceListUnitOriginal : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}
