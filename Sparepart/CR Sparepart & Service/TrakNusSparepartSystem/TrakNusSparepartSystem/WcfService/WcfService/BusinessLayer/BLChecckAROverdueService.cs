using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusSparepartSystemWcfService
{
    public class BLChecckAROverdueService : ICheckAROverdue
    {
        public WS_Response_CheckAROverdue CheckAR(string transNumber, string customerCode, string businessTransType, IOrganizationService serviceProxy)
        {
            WS_Response_CheckAROverdue response = new WS_Response_CheckAROverdue();
            bool flagTransNumber = false;
            string accountNumber = string.Empty;
            int AROverdue = 0;
            decimal CurrenAR = 0, OverdueAmountAR = 0, totalBilling = 0, remainingPlafond = 0, plafond = 0;
            string fetch = String.Format(@"
                            <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='serviceappointment'>
                                <attribute name='activityid' />
                                <attribute name='trs_worevenue' />
                                <attribute name='ibizcs_operatingparty'/>
                                <filter type='and'>
                                  <condition attribute='new_sapwonumber' operator='eq' value='{0}' />
                                </filter>
                                <link-entity name='account' from='accountid' to='trs_customer' visible='false' link-type='outer' alias='ac'>
                                    <attribute name='accountnumber' />
                                </link-entity>
                                <link-entity name='ibizcs_operatingparty' from='ibizcs_operatingpartyid' to='ibizcs_operatingparty' visible='false' link-type='outer' alias='op'>
                                  <attribute name='ibizcs_remainingbalancearunit' />
                                  <attribute name='ibizcs_remainingbalancearservice' />
                                  <attribute name='ibizcs_remainingbalancearpart' />
                                  <attribute name='ibizcs_overduedaysarunit' />
                                  <attribute name='ibizcs_overduedaysarservice' />
                                  <attribute name='ibizcs_overduedaysarparts' />
                                  <attribute name='ibizcs_overdueamountarunit' />
                                  <attribute name='ibizcs_overdueamountarservice' />
                                  <attribute name='ibizcs_overdueamountarparts' />
                                  <attribute name='ibizcs_currentarunit' />
                                  <attribute name='ibizcs_currentarservice' />
                                  <attribute name='ibizcs_currentarpart' />
                                  <attribute name='ibizcs_plafondarservice' />
                                  <attribute name='ibizcs_plafonarunit' />
                                  <attribute name='ibizcs_plafonarpart' />
                                </link-entity>
                              </entity>
                            </fetch>", transNumber);
            EntityCollection result = serviceProxy.RetrieveMultiple(new FetchExpression(fetch));
            bool flagIOperatingParty = false;
            foreach (var c in result.Entities)
            {
                AROverdue = c.Attributes.Contains("op.ibizcs_overduedaysarservice") ? (int)c.GetAttributeValue<AliasedValue>("op.ibizcs_overduedaysarservice").Value : 0;
                CurrenAR = c.Attributes.Contains("op.ibizcs_currentarservice") ? ((Money)c.GetAttributeValue<AliasedValue>("op.ibizcs_currentarservice").Value).Value : 0;
                OverdueAmountAR = c.Attributes.Contains("op.ibizcs_overdueamountarservice") ? ((Money)c.GetAttributeValue<AliasedValue>("op.ibizcs_overdueamountarservice").Value).Value : 0;
                plafond = c.Attributes.Contains("op.ibizcs_plafondarservice") ? ((Money)c.GetAttributeValue<AliasedValue>("op.ibizcs_plafondarservice").Value).Value : 0;
                remainingPlafond =plafond-( CurrenAR + OverdueAmountAR);
                totalBilling = c.Attributes.Contains("trs_worevenue") ? c.GetAttributeValue<Money>("trs_worevenue").Value : 0;
                flagIOperatingParty = c.Attributes.Contains("ibizcs_operatingparty");
                flagTransNumber = true;
                accountNumber = c.Attributes.Contains("ac.accountnumber") ?(string) c.GetAttributeValue<AliasedValue>("ac.accountnumber").Value : string.Empty;
            }

            if (!flagTransNumber)
            {
                response.Flag = false;
                response.Message = string.Format("Trans Number Not Found");
            }
            else if (!flagIOperatingParty)
            {
                response.Flag = false;
                response.Message = string.Format("Operating party not found");
            }else if (accountNumber != customerCode)
            {
                response.Flag = false;
                response.Message = string.Format("Invalid Customer Code {0}",customerCode);
            }
            else
            {
                if (AROverdue > 0)
                {
                    response.Flag = false;
                    response.Message = string.Format("AR Service OD for Customer Code {0}, {1} days", customerCode, AROverdue);
                }
                else
                {
                    if (totalBilling < remainingPlafond)
                        response.Flag = true;
                    else
                        response.Flag = false;

                    response.Message = string.Format("Remaining Plafond Service for Customer Code {0}, Rp. {1}", customerCode, remainingPlafond.ToString("N0"));
                }
            }

            return response;
        }
    }
}