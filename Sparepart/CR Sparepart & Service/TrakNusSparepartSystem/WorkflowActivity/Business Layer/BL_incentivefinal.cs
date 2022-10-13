using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusSparepartSystem.WorkflowActivity.Business_Layer
{
    public class BL_incentivefinal : BaseCustomeWofkflow
    {
        public BL_incentivefinal(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context) : 
        base(organizationService, tracingService, context)
        {
        }

        public void IncentiveCalculation()
        {
           
        }
    }
}
