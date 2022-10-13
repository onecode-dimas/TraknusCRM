using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using TrakNusSparepartSystem.WorkflowActivity.Interface;
using TrakNusSparepartSystem.DataLayer;
using Microsoft.Xrm.Sdk.Query;
using TrakNusSparepartSystem.Helper;

namespace TrakNusSparepartSystem.WorkflowActivity.Business_Layer
{
    public class Calculate04 : ICalculate
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

        public Calculate04(IOrganizationService orgService, Entity refEntity)
        {
            _DL_tss_mastermarketsizelines = new DL_tss_mastermarketsizelines();
            _DL_tss_mastermarketsizesublines = new DL_tss_mastermarketsizesublines();
            nOrganizationService = orgService;
            nTargetSales = new decimal[12];
            nRefEntity = refEntity;
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
               
                var groupHeader = (from r in msLines.AsEnumerable()
                                   group r by new { groupByHeader = (((EntityReference)r.Attributes["tss_mastermarketsizeref"]).Id) } into g
                                   select new
                                   {
                                       sumAmount = g.Sum(x =>
                                                           Convert.ToDecimal(((Money)x.GetAttributeValue<AliasedValue>("mastermarketsizesubline.tss_price").Value).Value)
                                                           * Convert.ToDecimal(((int)x.GetAttributeValue<AliasedValue>("mastermarketsizesubline.tss_qty").Value))),
                                   
                                   }).ToList();
                if (groupHeader.Count > 0)
                {

                    decimal val = groupHeader[0].sumAmount;
                    decimal totalProrate = 0m;
                    totalProrate = val / (decimal)12;
                    for (int i = 1; i <= 12; i++)
                    {
                        nTargetSales[i - 1] = totalProrate;
                    }
                }

            }

        }

    }
}
