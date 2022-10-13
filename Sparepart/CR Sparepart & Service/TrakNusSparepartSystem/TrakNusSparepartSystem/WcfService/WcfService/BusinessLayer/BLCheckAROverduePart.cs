using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrakNusSparepartSystemWcfService
{
    public class BLCheckAROverduePart : ICheckAROverdue
    {

        public WS_Response_CheckAROverdue CheckAR(string transNumber, string customerCode, string businessTransType, IOrganizationService serviceProxy)
        {
            WS_Response_CheckAROverdue response = new WS_Response_CheckAROverdue();
            bool flagTransNumber = false, flagIOperatingParty = false;
            int AROverdue = 0,AROverdueService=0, AROverdueUnit=0;
            string accountNumber = string.Empty;
            decimal CurrenAR = 0, OverdueAmountAR = 0, totalBilling = 0, remainingPlafond = 0,plafond = 0;
            EntityReference refAccount = new EntityReference();
            string fetchHeader = String.Format(@"
                            <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='tss_sopartheader'>
                                <attribute name='tss_sopartheaderid' />
                                <attribute name='tss_totalamount' />
                                <attribute name='tss_customer' />
                                <order attribute='tss_customer' descending='false' />
                                <filter type='and'>
                                  <condition attribute='tss_sapsoid' operator='eq' value='{0}' />
                                </filter>
                              </entity>
                            </fetch>", transNumber);
            EntityCollection resultHeader = serviceProxy.RetrieveMultiple(new FetchExpression(fetchHeader));
            foreach (var c in resultHeader.Entities)
            {
                refAccount = c.Attributes.Contains("tss_customer") ? c.GetAttributeValue<EntityReference>("tss_customer") : new EntityReference();
                totalBilling = c.Attributes.Contains("tss_totalamount") ? c.GetAttributeValue<Money>("tss_totalamount").Value : 0;
                flagTransNumber = true;
            }
            string fetch = String.Format(@"
                            <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='account'>
                                <attribute name='accountid' />
                                <attribute name='ibizcs_customerparty' />
                                <attribute name='accountnumber' />
                                <filter type='and'>
                                  <condition attribute='accountid' operator='eq' uiname='{0}' uitype='account' value='{1}' />
                                </filter>
                                <link-entity name='ibizcs_operatingparty' from='ibizcs_operatingpartyid' to='ibizcs_customerparty' visible='false' link-type='outer' alias='op'>
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
                            </fetch>", refAccount.Name, refAccount.Id);
            EntityCollection result = serviceProxy.RetrieveMultiple(new FetchExpression(fetch));

            foreach (var c in result.Entities)
            {
                AROverdue = c.Attributes.Contains("op.ibizcs_overduedaysarparts") ? (int)c.GetAttributeValue<AliasedValue>("op.ibizcs_overduedaysarparts").Value : 0;
                AROverdueService = c.Attributes.Contains("op.ibizcs_overduedaysarservice") ? (int)c.GetAttributeValue<AliasedValue>("op.ibizcs_overduedaysarservice").Value : 0;
                AROverdueUnit = c.Attributes.Contains("op.ibizcs_overduedaysarunit") ? (int)c.GetAttributeValue<AliasedValue>("op.ibizcs_overduedaysarunit").Value : 0;
                CurrenAR = c.Attributes.Contains("op.ibizcs_currentarpart") ? ((Money)c.GetAttributeValue<AliasedValue>("op.ibizcs_currentarpart").Value).Value : 0;
                OverdueAmountAR = c.Attributes.Contains("op.ibizcs_overdueamountarparts") ? ((Money)c.GetAttributeValue<AliasedValue>("op.ibizcs_overdueamountarparts").Value).Value : 0;
                plafond = c.Attributes.Contains("op.ibizcs_plafonarpart") ? ((Money)c.GetAttributeValue<AliasedValue>("op.ibizcs_plafonarpart").Value).Value : 0;
                remainingPlafond = plafond - (CurrenAR + OverdueAmountAR);
                flagIOperatingParty = c.Attributes.Contains("ibizcs_customerparty");
                accountNumber = c.Attributes.Contains("accountnumber") ? c.GetAttributeValue<string>("accountnumber") : string.Empty;
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
            }
            else if (accountNumber != customerCode)
            {
                response.Flag = false;
                response.Message = string.Format("Invalid Customer Code {0}", customerCode);
            }
            else if (AROverdue > 0||AROverdueService>0||AROverdueUnit>0)
            {
                response.Flag = false;
                response.Message = string.Format("AR {0} OD for Customer Code {1}, {2} days",AROverdue>0?"Part":AROverdueService>0?"Service":"Unit", customerCode, AROverdue>0?AROverdue:AROverdueService>0?AROverdueService:AROverdueUnit);
            }
            else
            {
                if (totalBilling < remainingPlafond)
                    response.Flag = true;
                else
                    response.Flag = false;

                response.Message = string.Format("Remaining Plafond Sparepart for Customer Code {0}, Rp. {1}", customerCode, remainingPlafond.ToString("N0"));
            }
            return response;
        }
    }
}