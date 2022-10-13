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
   public class BL_tss_salesactualbranch
    {
        DL_tss_salesactualbranch _DL_tss_salesactualbranch = new DL_tss_salesactualbranch();
        public void GenerateSalesActualBranch(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context,Guid refSalesTargetBranch)
        {
            var tssType = Enum.GetValues(typeof(TssType));
            
            foreach (var o in tssType)
            {
                Entity entity = new Entity(_DL_tss_salesactualbranch.EntityName);
                entity["tss_type"] = new OptionSetValue((int)o);
                entity["tss_refsalestargetbranch"] =new EntityReference("tss_salestargetbranch", refSalesTargetBranch==Guid.Empty? context.PrimaryEntityId:refSalesTargetBranch);
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
                entity["tss_totalyearlyactualms"] = new Money(0);
                organizationService.Create(entity);
            }
        }
    }
}
