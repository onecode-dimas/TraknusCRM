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
    public class BL_ittn_exchangerateidr
    {
        #region Properties
        private string _classname = "BL_ittn_exchangerateidr";
        #endregion

        #region Constants
        public enum ExchangeRateMonth
        {
            January = 1,
            February = 2,
            March = 3,
            April = 4,
            May = 5,
            June = 6,
            July = 7,
            August = 8,
            September = 9,
            October = 10,
            November = 11,
            December = 12
        }
        #endregion

        #region Depedencies
        #endregion

        #region Publics
        public void ExchangeRateIDR(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {
            try
            {
                #region Variables
                String ExchangeRateYear, ExchangeRateForeignCurrencyName, ExchangeRateMonthName;
                Guid ExchangeRateForeignCurrencyId;
                int ExchangeRateMonth;
                #endregion

                Entity ExchangeRate = organizationService.Retrieve(context.PrimaryEntityName, context.PrimaryEntityId, new ColumnSet(true));

                #region Attributes
                if (ExchangeRate.Contains("ittn_month") && ExchangeRate.Attributes["ittn_month"] != null)
                {
                    ExchangeRateMonth = ExchangeRate.GetAttributeValue<OptionSetValue>("ittn_month").Value;
                    ExchangeRateMonthName = ExchangeRate.FormattedValues["ittn_month"].ToString();
                }
                else
                    throw new InvalidWorkflowException("Exchange Rate Month is null/ empty!");

                if (ExchangeRate.Contains("ittn_year") && ExchangeRate.Attributes["ittn_year"] != null)
                    ExchangeRateYear = ExchangeRate.GetAttributeValue<String>("ittn_year");
                else
                    throw new InvalidWorkflowException("Exchange Rate Year is null/ empty!");

                if (ExchangeRate.Contains("ittn_forreigncurrency") && ExchangeRate.Attributes["ittn_forreigncurrency"] != null)
                {
                    ExchangeRateForeignCurrencyId = ExchangeRate.GetAttributeValue<EntityReference>("ittn_forreigncurrency").Id;
                    ExchangeRateForeignCurrencyName = ExchangeRate.GetAttributeValue<EntityReference>("ittn_forreigncurrency").Name;
                }
                else
                    throw new InvalidWorkflowException("Exchange Rate Foreign Currency is null/ empty!");
                #endregion

                #region Check for Same Foreign Currency, Month, and Year
                QueryExpression qe = new QueryExpression(context.PrimaryEntityName);
                qe.ColumnSet = new ColumnSet(true);
                FilterExpression fe = qe.Criteria.AddFilter(LogicalOperator.And);
                fe.AddCondition("ittn_forreigncurrencyid", ConditionOperator.Equal, ExchangeRateForeignCurrencyId.ToString());
                fe.AddCondition("ittn_month", ConditionOperator.Equal, ExchangeRateMonth);
                fe.AddCondition("ittn_year", ConditionOperator.Equal, ExchangeRateYear);
                EntityCollection ec = organizationService.RetrieveMultiple(qe);

                if (ec.Entities.Count > 0)
                    throw new InvalidWorkflowException("Found duplicate Exchange Rate for Foreign Currency : " + ExchangeRateForeignCurrencyName + ", Month : " + ExchangeRateMonthName + ", Year : " + ExchangeRateYear + " !");
                else
                {
                    #region Set Exchange Rate IDR Attributes
                    throw new InvalidWorkflowException("yes");
                    #endregion
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".ExchangeRateIDR : " + ex.Message.ToString());
            }

        }
        #endregion
    }
}
