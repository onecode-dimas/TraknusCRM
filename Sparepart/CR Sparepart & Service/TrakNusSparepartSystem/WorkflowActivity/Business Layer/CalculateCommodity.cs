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
    public class CalculateCommodity : ICalculate
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

        public CalculateCommodity(IOrganizationService orgService, Entity refEntity)
        {
            _DL_tss_mastermarketsizelines = new DL_tss_mastermarketsizelines();
            _DL_tss_mastermarketsizesublines = new DL_tss_mastermarketsizesublines();
            nOrganizationService = orgService;
            nTargetSales = new decimal[12];
            nRefEntity = refEntity;
        }
        public void Calculate()
        {
          nQExpresion = new QueryExpression(_DL_tss_mastermarketsizesublines.EntityName);
            nQExpresion.Criteria.AddCondition("tss_mastermarketsizeid", ConditionOperator.In, nRefEntity.Id);
            nQExpresion.ColumnSet = new ColumnSet(true);
            List<Entity> msLines = _retrievehelper.RetrieveMultiple(nOrganizationService, nQExpresion); // _DL_tss_mastermarketsizesublines.Select(nOrganizationService, nQExpresion);

            if (msLines.Count > 0)
            {
                var groupHeader = (from r in msLines.AsEnumerable()
                                   group r by new { groupByHeader = (r.GetAttributeValue<EntityReference>("tss_mastermarketsizeid").Id) } into g
                                   select new
                                   {
                                       sumAmount = g.Sum(x =>
                                                          (x.GetAttributeValue<Money>("tss_price").Value) * (Convert.ToDecimal(x.GetAttributeValue<int>("tss_qty")))
                                                          ),
                                       headerID = g.Key.groupByHeader
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
