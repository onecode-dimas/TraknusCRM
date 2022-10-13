using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrakNusSparepartSystem.DataLayer;
using TrakNusSparepartSystem.WorkflowActivity.Helper;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
namespace TrakNusSparepartSystem.WorkflowActivity.Business_Layer
{
   public class BL_tss_salesactualnational
    {
        DL_tss_salesactualnational _DL_tss_salesactualnational = new DL_tss_salesactualnational();

        public void GenerateSalesActualNational(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context, Guid refSalesTargetNational)
        {
            QueryExpression _queryexpression = new QueryExpression(_DL_tss_salesactualnational.EntityName);
            _queryexpression.Criteria.AddCondition("tss_refsalestargetnational", ConditionOperator.Equal, refSalesTargetNational);
            
            EntityCollection _salesactualnationalcollection = organizationService.RetrieveMultiple(_queryexpression);

            if (_salesactualnationalcollection.Entities.Count() == 0)
            {
                var tssType = Enum.GetValues(typeof(TssType));
                foreach (var o in tssType)
                {
                    Entity entity = new Entity(_DL_tss_salesactualnational.EntityName);
                    entity["tss_type"] = new OptionSetValue((int)o);
                    entity["tss_refsalestargetnational"] = new EntityReference("tss_salestargetnational", refSalesTargetNational == Guid.Empty ? context.PrimaryEntityId : refSalesTargetNational);
                    entity["tss_january"] = new Money(0);
                    entity["tss_february"] = new Money(0);
                    entity["tss_march"] = new Money(0);
                    entity["tss_april"] = new Money(0);
                    entity["tss_may"] = new Money(0);
                    entity["tss_june"] = new Money(0);
                    entity["tss_july"] = new Money(0);
                    entity["tss_august"] = new Money(0);
                    entity["tss_september"] = new Money(0);
                    entity["tss_october"] = new Money(0);
                    entity["tss_november"] = new Money(0);
                    entity["tss_december"] = new Money(0);
                    entity["tss_totalyearly"] = new Money(0);
                    organizationService.Create(entity);
                }
            }
        }
    }
}
