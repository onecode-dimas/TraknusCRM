using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.WorkflowActivity.Business_Layer
{
    public abstract class BaseCustomeWofkflow
    {
        protected IOrganizationService CRMOrganizationService;
        protected ITracingService CRMTracingService;
        protected IWorkflowContext CRMContext;
        protected FilterExpression CRMFilterExp;
        protected QueryExpression CRMQExp;
        protected EntityCollection EntityMultyRetrieve;
        protected Entity EntitySingleRetrieve;
        protected  const int SOHEADER_INVOICED = 865920003;
        public BaseCustomeWofkflow(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {
            CRMContext = context;
            CRMOrganizationService = organizationService;
            CRMTracingService = tracingService;
        }
        protected EntityCollection RetrieveUnitGroup(Entity sopartheader, bool invoicestatus)
        {

            FilterExpression fe = new FilterExpression(LogicalOperator.And);
            fe.AddCondition("tss_sopartheaderid", ConditionOperator.In, CRMContext.PrimaryEntityId);
            if (invoicestatus)
                fe.AddCondition("tss_status", ConditionOperator.Equal, SOHEADER_INVOICED);
            LinkEntity linkSoPartLine = new LinkEntity
            {
                LinkFromEntityName = "tss_sopartlines",
                LinkToEntityName = "trs_masterpart",
                LinkFromAttributeName = "tss_partnumber",
                LinkToAttributeName = "trs_masterpartid",
                Columns = new ColumnSet(new string[] { "trs_product" }),
                JoinOperator = JoinOperator.Inner,
                EntityAlias = "sopartline"
            };

            LinkEntity linkPartMaster = new LinkEntity
            {
                LinkFromEntityName = "trs_masterpart",
                LinkToEntityName = "product",
                LinkFromAttributeName = "trs_product",
                LinkToAttributeName = "productid",
                Columns = new ColumnSet(new string[] { "new_unitgroup" }),
                JoinOperator = JoinOperator.Inner,
                EntityAlias = "product"
            };

            CRMQExp = new QueryExpression("tss_sopartlines");
            CRMQExp.LinkEntities.Add(linkSoPartLine);
            CRMQExp.LinkEntities[0].LinkEntities.Add(linkPartMaster);
            CRMQExp.ColumnSet = new ColumnSet(new string[] { "tss_partnumber", "createdon" });
            CRMQExp.Criteria.AddFilter(fe);
            EntityCollection joinsopartline = CRMOrganizationService.RetrieveMultiple(CRMQExp);
            return joinsopartline;
        }

    }
}
