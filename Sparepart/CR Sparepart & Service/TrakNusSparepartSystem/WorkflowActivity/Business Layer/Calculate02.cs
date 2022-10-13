using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using TrakNusSparepartSystem.WorkflowActivity.Interface;
using Microsoft.Xrm.Sdk.Query;
using TrakNusSparepartSystem.DataLayer;
using TrakNusSparepartSystem.Helper;

namespace TrakNusSparepartSystem.WorkflowActivity.Business_Layer
{
   public class Calculate02 : ICalculate
    {
        private DL_tss_mastermarketsizelines _DL_tss_mastermarketsizelines;
        private DL_tss_mastermarketsizesublines _DL_tss_mastermarketsizesublines;
        private QueryExpression nQExpresion;
        private IOrganizationService nOrganizationService;
        private Entity nRefEntity;
        RetrieveHelper _retrievehelper = new RetrieveHelper();

        private decimal[] nTargetSales;

        public decimal[] TargetSales
        {
            get
            {
                return nTargetSales;
            }
        }

        public Calculate02(IOrganizationService orgService, Entity refEntity)
        {
            _DL_tss_mastermarketsizelines = new DL_tss_mastermarketsizelines();
            _DL_tss_mastermarketsizesublines = new DL_tss_mastermarketsizesublines();
            nOrganizationService = orgService;
            nRefEntity = refEntity;
            nTargetSales = new decimal[12];
        }
        public void Calculate()
        {
            nQExpresion = new QueryExpression(_DL_tss_mastermarketsizelines.EntityName);
            LinkEntity lintToSubline = new LinkEntity
            {
                LinkFromEntityName = _DL_tss_mastermarketsizelines.EntityName,
                LinkToEntityName = _DL_tss_mastermarketsizesublines.EntityName,
                LinkFromAttributeName = "tss_mastermarketsizelinesid",
                LinkToAttributeName = "tss_mastermslinesref",
                Columns = new ColumnSet(true),
                EntityAlias = "mastermarketsizesubline",
                JoinOperator = JoinOperator.Inner

            };
            nQExpresion.LinkEntities.Add(lintToSubline);
            nQExpresion.Criteria.AddCondition("tss_mastermarketsizeref", ConditionOperator.In, nRefEntity.Id);
            nQExpresion.ColumnSet = new ColumnSet(true);
            List<Entity> msLines = _retrievehelper.RetrieveMultiple(nOrganizationService, nQExpresion); // _DL_tss_mastermarketsizelines.Select(nOrganizationService, nQExpresion);

            if (msLines.Count > 0)
            {
                var groupByMonht = (from r in msLines.AsEnumerable().Where(o => o.GetAttributeValue<DateTime>("tss_duedate").ToLocalTime().Date != DateTime.MinValue.Date)
                                    group r by new { groupByDueDate = (r.GetAttributeValue<DateTime>("tss_duedate").ToLocalTime().Date).Month } into g
                                    select new
                                    {
                                        sumAmount = g.Sum(x =>
                                                           ((Money)x.GetAttributeValue<AliasedValue>("mastermarketsizesubline.tss_price").Value).Value
                                                           * Convert.ToDecimal(x.GetAttributeValue<AliasedValue>("mastermarketsizesubline.tss_qty").Value)),
                                        dueDate = g.Key.groupByDueDate
                                    }).ToList();
                foreach (var o in groupByMonht)
                {
                    if (Convert.ToInt32(o.dueDate) != 0)
                        nTargetSales[Convert.ToInt32(o.dueDate) - 1] = o.sumAmount;
                }

            }

        }

       
    }
}
