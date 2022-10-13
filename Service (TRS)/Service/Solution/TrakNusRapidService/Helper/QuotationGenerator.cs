using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;

using TrakNusRapidService.DataLayer;

namespace TrakNusRapidService.Helper
{
    public class QuotationGenerator
    {
        #region Constants
        private const string _classname = "QuotationGenerator";
        #endregion

        #region Depedencies
        private DL_trs_masterpart _DL_trs_masterpart = new DL_trs_masterpart();
        private DL_new_pricelistcpo _DL_new_pricelistcpo = new DL_new_pricelistcpo();
        private DL_trs_partpricemaster _DL_trs_partpricemaster = new DL_trs_partpricemaster();
        private DL_trs_quotationpartssummary _DL_trs_quotationpartssummary = new DL_trs_quotationpartssummary();
        private DL_trs_quotationtool _DL_trs_quotationtool = new DL_trs_quotationtool();
        #endregion

        #region Properties
        #endregion

        #region Privates
        #endregion

        #region Publics
        public void SummarizeParts(IOrganizationService organizationService, Guid id)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression();
                Entity ePart = new Entity();
                EntityCollection ecolPartPrice = new EntityCollection();
                Entity ePartPrice = new Entity();
                EntityCollection ecolPartsSummary = new EntityCollection();
                Entity ePartsSummary = new Entity();
                EntityCollection ecolMaxItemNumber = new EntityCollection();
                Entity eMaxItemNumber = new Entity();
                int itemNumber = 0;
                int quantity = 0;
                Guid? currency = null;
                Money price = new Money(0);
                FilterExpression filterExpression = new FilterExpression();

                FetchExpression fetchExpression = new FetchExpression(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' aggregate='true'>
                                                                              <entity name='trs_quotationpartdetail'>
                                                                                <attribute name='trs_quantity' alias='trs_quantity_sum' aggregate='sum' />
                                                                                <attribute name='trs_partnumber' alias='trs_partnumber' groupby='true' />
                                                                                <filter type='and'>
                                                                                  <condition attribute='trs_quotation' operator='eq' uitype='trs_quotation' value='{" + id.ToString() + @"}' />
                                                                                  <condition attribute='statecode' operator='eq' value='0' />
                                                                                  <condition attribute='statuscode' operator='eq' value='1' />
                                                                                </filter>
                                                                              </entity>
                                                                            </fetch>");
                EntityCollection entityCollection = organizationService.RetrieveMultiple(fetchExpression);
                foreach (Entity entity in entityCollection.Entities)
                {
                    ePart = _DL_trs_masterpart.Select(organizationService, ((EntityReference)entity.GetAttributeValue<AliasedValue>("trs_partnumber").Value).Id);
                    quantity = (int)entity.GetAttributeValue<AliasedValue>("trs_quantity_sum").Value;

                    //Get Price
                    currency = null;
                    price = new Money(0);

                    if (currency == null)
                    {
                        queryExpression = new QueryExpression(_DL_trs_partpricemaster.EntityName);
                        queryExpression.ColumnSet = new ColumnSet(true);

                        LinkEntity lePriceListCPO = new LinkEntity();
                        lePriceListCPO.LinkFromEntityName = _DL_trs_partpricemaster.EntityName;
                        lePriceListCPO.LinkFromAttributeName = "trs_pricelist";
                        lePriceListCPO.LinkToEntityName = _DL_new_pricelistcpo.EntityName;
                        lePriceListCPO.LinkToAttributeName = "new_pricelistcpoid";
                        lePriceListCPO.JoinOperator = JoinOperator.Inner;
                        lePriceListCPO.LinkCriteria.AddCondition("new_code", ConditionOperator.Equal, "P1");

                        queryExpression.LinkEntities.Add(lePriceListCPO);
                        queryExpression.Criteria.AddCondition("trs_partmaster", ConditionOperator.Equal, ePart.Id);
                        ecolPartPrice = _DL_trs_partpricemaster.Select(organizationService, queryExpression);
                        if (ecolPartPrice.Entities.Count > 0)
                        {
                            ePartPrice = ecolPartPrice.Entities[0];
                            currency = ePartPrice.GetAttributeValue<EntityReference>("transactioncurrencyid").Id;
                            price = ePartPrice.GetAttributeValue<Money>("trs_price");
                        }
                    }

                    queryExpression = new QueryExpression(_DL_trs_quotationpartssummary.EntityName);
                    queryExpression.ColumnSet = new ColumnSet(true);
                    filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                    filterExpression.AddCondition("trs_quotationnumber", ConditionOperator.Equal, id);
                    filterExpression.AddCondition("trs_partnumber", ConditionOperator.Equal, ePart.Id);
                    ecolPartsSummary = _DL_trs_quotationpartssummary.Select(organizationService, queryExpression);
                    if (ecolPartsSummary.Entities.Count > 0)
                    {
                        ePartsSummary = ecolPartsSummary.Entities[0];
                        _DL_trs_quotationpartssummary = new DL_trs_quotationpartssummary();
                        _DL_trs_quotationpartssummary.trs_tasklistquantity = quantity;
                        if (currency != null)
                        {
                            _DL_trs_quotationpartssummary.transactioncurrencyid = (Guid)currency;
                            _DL_trs_quotationpartssummary.trs_price = price;
                        }
                        _DL_trs_quotationpartssummary.Update(organizationService, ePartsSummary.Id);
                    }
                    else
                    {
//                        fetchExpression = new FetchExpression(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' aggregate='true'>
//                                                                      <entity name='trs_quotationpartssummary'>
//                                                                        <attribute name='trs_itemnumber' alias='trs_itemnumber' aggregate='max' />
//                                                                        <filter type='and'>
//                                                                          <condition attribute='trs_quotationnumber' operator='eq' uitype='trs_quotation' value='{" + id.ToString() + @"}' />
//                                                                        </filter>
//                                                                      </entity>
//                                                                    </fetch>");
//                        ecolMaxItemNumber = organizationService.RetrieveMultiple(fetchExpression);
//                        if (ecolMaxItemNumber.Entities.Count > 0)
//                        {
//                            eMaxItemNumber = ecolMaxItemNumber.Entities[0];
//                            if (eMaxItemNumber.Attributes.Contains("trs_itemnumber"))
//                                itemNumber = (int)eMaxItemNumber.GetAttributeValue<AliasedValue>("trs_itemnumber").Value + 1;
//                            else
//                                itemNumber = 1;
//                        }
//                        else
//                        {
//                            itemNumber = 1;
//                        }

                        _DL_trs_quotationpartssummary = new DL_trs_quotationpartssummary();
                        _DL_trs_quotationpartssummary.trs_quotationnumber = id;
                        //_DL_trs_quotationpartssummary.trs_itemnumber = itemNumber;
                        _DL_trs_quotationpartssummary.trs_partnumber = ePart.Id;
                        //_DL_trs_quotationpartssummary.trs_partname = ePart.GetAttributeValue<string>("trs_partdescription");
                        _DL_trs_quotationpartssummary.trs_tasklistquantity = quantity;
                        if (currency != null)
                        {
                            _DL_trs_quotationpartssummary.transactioncurrencyid = (Guid)currency;
                            _DL_trs_quotationpartssummary.trs_price = price;
                            _DL_trs_quotationpartssummary.trs_totalprice = new Money(quantity * price.Value);
                        }
                        _DL_trs_quotationpartssummary.Insert(organizationService);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SummarizeParts : " + ex.Message);
            }
        }

        public void SummarizeToolGroups(IOrganizationService organizationService, Guid id)
        {
            try
            {
                List<Guid> QuotationToolGroups = new List<Guid>();
                Guid QuotationToolGroup;

                QueryExpression queryExpression = new QueryExpression(_DL_trs_quotationtool.EntityName);
                queryExpression.ColumnSet = new ColumnSet(true);
                queryExpression.Criteria.AddCondition("trs_quotation", ConditionOperator.Equal, id);
                EntityCollection ecolWOTools = _DL_trs_quotationtool.Select(organizationService, queryExpression);
                foreach (Entity eWOTools in ecolWOTools.Entities)
                {
                    QuotationToolGroups.Add(eWOTools.GetAttributeValue<EntityReference>("trs_toolsgroup").Id);
                }

                FetchExpression fetchExpression = new FetchExpression(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' aggregate='true'>
                                                                          <entity name='trs_quotationcommercialdetail'>
                                                                            <filter type='and'>
                                                                              <condition attribute='trs_quotation' operator='eq' uitype='trs_quotation' value='{" + id.ToString() + @"}' />
                                                                            </filter>
                                                                            <link-entity name='trs_trs_quotationcommercialdetail_trs_tools' from='trs_quotationcommercialdetailid' to='trs_quotationcommercialdetailid' alias='trs_commercialdetailtools'>
                                                                              <attribute name='trs_toolsgroupid' alias='trs_toolsgroupid' groupby='true' />
                                                                            </link-entity>
                                                                          </entity>
                                                                        </fetch>");
                EntityCollection entityCollection = organizationService.RetrieveMultiple(fetchExpression);
                foreach (Entity entity in entityCollection.Entities)
                {
                    QuotationToolGroup = (Guid)entity.GetAttributeValue<AliasedValue>("trs_toolsgroupid").Value;
                    if (QuotationToolGroups.Exists(x => x == QuotationToolGroup)) { }
                    else
                    {
                        _DL_trs_quotationtool = new DL_trs_quotationtool();
                        _DL_trs_quotationtool.trs_quotation = id;
                        _DL_trs_quotationtool.trs_toolsgroup = QuotationToolGroup;
                        _DL_trs_quotationtool.Insert(organizationService);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SummarizeToolGroups : " + ex.Message);
            }
        }
        #endregion
    }
}
