using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrakNusSparepartSystem.DataLayer;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using TrakNusSparepartSystem.Helper;

namespace TrakNusSparepartSystem.WorkflowActivity.Business_Layer
{
    public class BL_tss_marketsizesummarybypartnumber
    {
        private RetrieveHelper _retrievehelper = new RetrieveHelper();

        public void GenerateMarketSizeSummaryByPartNumber(IOrganizationService organizationService, IWorkflowContext context, EntityCollection listKeyAccount)
        {
            object[] _keyaccountids = listKeyAccount.Entities.Select(x => (object)x.Id).ToArray();

            List<Entity> entityMulipleToRetrieve;
            FilterExpression fExpresion;
            QueryExpression qExpresion;

            fExpresion = new FilterExpression(LogicalOperator.And);
            fExpresion.AddCondition("tss_status", ConditionOperator.Equal, 865920000);
            fExpresion.AddCondition("tss_keyaccountid", ConditionOperator.In, _keyaccountids);
            //fExpresion.AddCondition("tss_status", ConditionOperator.Equal, 865920000);
            qExpresion = new QueryExpression("tss_mastermarketsize");


            LinkEntity linkToLine = new LinkEntity
            {
                LinkFromEntityName = "tss_mastermarketsize",
                LinkToEntityName = "tss_mastermarketsizelines",
                LinkFromAttributeName = "tss_mastermarketsizeid",
                LinkToAttributeName = "tss_mastermarketsizeref",
                Columns = new ColumnSet(true),
                EntityAlias = "line",
                JoinOperator = JoinOperator.Inner

            };

            LinkEntity linkToSubline = new LinkEntity
            {
                LinkFromEntityName = "tss_mastermarketsizelines",
                LinkToEntityName = "tss_mastermarketsizesublines",
                LinkFromAttributeName = "tss_mastermarketsizelinesid",
                LinkToAttributeName = "tss_mastermslinesref",
                Columns = new ColumnSet(true),
                EntityAlias = "subline",
                JoinOperator = JoinOperator.Inner
            };

            linkToLine.LinkEntities.Add(linkToSubline);
            qExpresion.LinkEntities.Add(linkToLine);
            qExpresion.ColumnSet = new ColumnSet(true);
            qExpresion.Criteria.AddFilter(fExpresion);
            entityMulipleToRetrieve = _retrievehelper.RetrieveMultiple(organizationService, qExpresion); // organizationService.RetrieveMultiple(qExpresion);

            var groupPartNumber = (from i in entityMulipleToRetrieve.AsEnumerable()

                                   group i by new
                                   {
                                       partNumber = (EntityReference)i.GetAttributeValue<AliasedValue>("subline.tss_partnumber").Value,
                                       price = ((Money)i.GetAttributeValue<AliasedValue>("subline.tss_price").Value).Value,
                                       //partDescription = (string)i.GetAttributeValue<AliasedValue>("subline.tss_partdescription").Value,
                                       //customer = (EntityReference)i.GetAttributeValue<AliasedValue>("line.tss_customer").Value,
                                       //groupUIOCommodity = (EntityReference)i.GetAttributeValue<AliasedValue>("line.tss_groupuiocommodity").Value,
                                   } into g
                                   select new
                                   {
                                       Qty = g.Sum(o => (int)o.GetAttributeValue<AliasedValue>("subline.tss_qty").Value),
                                       PartNumber = g.Key.partNumber,
                                       Price = g.Key.price,
                                       //PartDescription = g.Key.partDescription,
                                       //Customer = g.Key.customer,
                                       //GroupUIOCommodity = g.Key.groupUIOCommodity
                                   }).ToList();

            foreach (var o in groupPartNumber)
            {
                fExpresion = new FilterExpression(LogicalOperator.And);
                fExpresion.AddCondition("tss_partnumber", ConditionOperator.Equal, o.PartNumber.Id);
                qExpresion = new QueryExpression("tss_marketsizesummarybypartnumber");
                qExpresion.ColumnSet = new ColumnSet(true);
                qExpresion.Criteria.AddFilter(fExpresion);
                EntityCollection entityToUpdate = organizationService.RetrieveMultiple(qExpresion);

                if (entityToUpdate.Entities.Count > 0)
                {
                    Entity enToUpdate = entityToUpdate.Entities[0];
                    //enToUpdate["tss_partdescription"] = o.PartDescription;
                    //enToUpdate["tss_partnumber"] = o.PartNumber;
                    //enToUpdate["tss_customer"] = o.Customer;
                    //enToUpdate["tss_groupuiocommodity"] = o.GroupUIOCommodity;
                    enToUpdate["tss_qty"] = o.Qty;
                    enToUpdate["tss_totalamount"] = new Money(o.Price * o.Qty);
                    organizationService.Update(enToUpdate);

                }
                else
                {
                    Entity entityToInsert = new Entity("tss_marketsizesummarybypartnumber");
                    //entityToInsert["tss_partdescription"] = o.PartDescription;
                    entityToInsert["tss_partnumber"] = o.PartNumber;
                    //entityToInsert["tss_customer"] = o.Customer;
                    //entityToInsert["tss_groupuiocommodity"] = o.GroupUIOCommodity;
                    entityToInsert["tss_qty"] = o.Qty;
                    entityToInsert["tss_totalamount"] = new Money(o.Price * o.Qty);
                    organizationService.Create(entityToInsert);
                }

            }
        }

    }
}
