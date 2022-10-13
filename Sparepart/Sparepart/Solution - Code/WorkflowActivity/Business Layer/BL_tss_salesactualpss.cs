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
using TrakNusSparepartSystem.WorkflowActivity.BusinessLayer;

namespace TrakNusSparepartSystem.WorkflowActivity.Business_Layer
{
    public class BL_tss_salesactualpss
    {
        DL_tss_salesactualpss _DL_tss_salesactualpss = new DL_tss_salesactualpss();
        
        public void GenerateSalesActualPSS(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context,Guid refSalesTargetPss)
        {
            var tssType = Enum.GetValues(typeof(TssType));
            foreach (var o in tssType)
            {
               
                Entity entity = new Entity(_DL_tss_salesactualpss.EntityName);
                entity["tss_type"] = new OptionSetValue((int)o);
                entity["tss_refsalestargetpss"] =new EntityReference("tss_salestargetpss", refSalesTargetPss==Guid.Empty? context.PrimaryEntityId:refSalesTargetPss);
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

        public void ApproveSalesTargetPSS_OnClick(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {

        }
    }
}
