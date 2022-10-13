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
    public class BL_documentpayment : BaseCustomeWofkflow
    {
        public BL_documentpayment(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context) :
        base(organizationService, tracingService, context)
        {
        }

        public void InsertDocumentPayment()
        {

            FilterExpression fe = new FilterExpression(LogicalOperator.And);
            fe.AddCondition("tss_sopartlinesid", ConditionOperator.Equal, CRMContext.PrimaryEntityId);
            LinkEntity linkSoPartLine = new LinkEntity
            {
                LinkFromEntityName = "tss_sopartheader",
                LinkToEntityName = "tss_sopartlines",
                LinkFromAttributeName = "tss_sopartheaderid",
                LinkToAttributeName = "tss_sopartheaderid",
                JoinOperator = JoinOperator.Inner,
                EntityAlias = "sopartlines"
            };

            LinkEntity linkSoPartSubLine = new LinkEntity
            {
                LinkFromEntityName = "tss_sopartlines",
                LinkToEntityName = "tss_salesorderpartsublines",
                LinkFromAttributeName = "tss_sopartlinesid",
                LinkToAttributeName = "tss_salesorderpartlines",
                Columns = new ColumnSet(new string[] { "tss_invoiceno", "tss_invoicedate", "tss_invoiceduedate", "tss_invoicevalue", "tss_paiddate", "tss_paidvalue" }),
                JoinOperator = JoinOperator.Inner,
                EntityAlias = "sopartsublines"
            };

            CRMQExp = new QueryExpression("tss_sopartheader");
            CRMQExp.ColumnSet = new ColumnSet(new string[] { "createdon" });
            CRMQExp.LinkEntities.Add(linkSoPartLine);
            CRMQExp.LinkEntities[0].LinkEntities.Add(linkSoPartSubLine);
            CRMQExp.LinkEntities[0].LinkCriteria.AddFilter(fe);
            CRMQExp.LinkEntities[0].LinkEntities[0].LinkCriteria.AddCondition("tss_transactiontype", ConditionOperator.Equal, 865920006);//Create Payment 
            EntityCollection joinsoheader = CRMOrganizationService.RetrieveMultiple(CRMQExp);
            Entity sopartheader = joinsoheader.Entities.FirstOrDefault();

            CRMQExp = new QueryExpression("tss_sparepartsetup");
            CRMQExp.ColumnSet = new ColumnSet(true);

            Entity setup = CRMOrganizationService.RetrieveMultiple(CRMQExp).Entities.FirstOrDefault();

            DateTime invoiceDate = (sopartheader.Attributes.Contains("sopartsublines.tss_invoicedate")) ? (DateTime)sopartheader.GetAttributeValue<AliasedValue>("sopartsublines.tss_invoiceduedate").Value : DateTime.MinValue;
            DateTime invoiceDueDate = (sopartheader.Attributes.Contains("sopartsublines.tss_invoicedate")) ? (DateTime)sopartheader.GetAttributeValue<AliasedValue>("sopartsublines.tss_invoicedate").Value : DateTime.MinValue;
            int? paymentTolleranceAllowance = setup.GetAttributeValue<int>("tss_paymenttoleranceallowance");

            CRMFilterExp = new FilterExpression(LogicalOperator.And);
            CRMFilterExp.AddCondition("tss_sonumber", ConditionOperator.Equal, sopartheader.Id);

            LinkEntity linkDocF5 = new LinkEntity()
            {
                LinkFromEntityName = "tss_documentpaymentf5",
                LinkToEntityName = "tss_documentpaymentf5lines",
                LinkFromAttributeName = "tss_documentpaymentf5id",
                LinkToAttributeName = "tss_documentpaymentf5id",
                JoinOperator = JoinOperator.Inner,
                EntityAlias = "docf5lines"
            };

            CRMQExp = new QueryExpression("tss_documentpaymentf5");
            CRMQExp.LinkEntities.Add(linkDocF5);
            CRMQExp.LinkEntities[0].LinkCriteria.AddCondition("tss_invoiceno", ConditionOperator.Equal, sopartheader.GetAttributeValue<AliasedValue>("sopartsublines.tss_invoiceno").Value.ToString());
            CRMQExp.Criteria.AddFilter(CRMFilterExp);
            EntityMultyRetrieve = CRMOrganizationService.RetrieveMultiple(CRMQExp);

            CRMQExp = new QueryExpression("tss_soincentive");
            CRMQExp.ColumnSet = new ColumnSet(new string[] { "tss_f5" });
            CRMQExp.Criteria.AddCondition("tss_sonumber", ConditionOperator.Equal, sopartheader.Id);
            EntitySingleRetrieve = CRMOrganizationService.RetrieveMultiple(CRMQExp).Entities.FirstOrDefault();
            Entity soincentiveToUpdate = new Entity(EntitySingleRetrieve.LogicalName, EntitySingleRetrieve.Id);


            if (EntityMultyRetrieve.Entities.Count() == 0)
            {
                Entity entityToInsert = new Entity("tss_documentpaymentf5");
                entityToInsert["tss_sonumber"] = new EntityReference(sopartheader.LogicalName, sopartheader.Id);
                entityToInsert["tss_sodate"] = sopartheader.GetAttributeValue<DateTime>("createdon");
                Guid docHeaderId = CRMOrganizationService.Create(entityToInsert);
                DateTime? paidDate = null, invoiceDateVal = null;

                Entity entityToInsertLines = new Entity("tss_documentpaymentf5lines");
                entityToInsertLines["tss_documentpaymentf5id"] = new EntityReference(entityToInsert.LogicalName, docHeaderId);
                entityToInsertLines["tss_invoiceno"] = (sopartheader.Attributes.Contains("sopartsublines.tss_invoiceno")) ? sopartheader.GetAttributeValue<AliasedValue>("sopartsublines.tss_invoiceno").Value.ToString() : string.Empty;
                entityToInsertLines["tss_invoicedate"] = invoiceDate == DateTime.MinValue ? invoiceDateVal : invoiceDate;
                entityToInsertLines["tss_invoiceamount"] = (sopartheader.Attributes.Contains("sopartsublines.tss_invoicevalue")) ? (Money)sopartheader.GetAttributeValue<AliasedValue>("sopartsublines.tss_invoicevalue").Value : new Money(0);
                entityToInsertLines["tss_paiddate"] = (sopartheader.Attributes.Contains("sopartsublines.tss_paiddate")) ? (DateTime)sopartheader.GetAttributeValue<AliasedValue>("sopartsublines.tss_paiddate").Value : paidDate;
                entityToInsertLines["tss_paidamount"] = (sopartheader.Attributes.Contains("sopartsublines.tss_paidvalue")) ? ((Money)sopartheader.GetAttributeValue<AliasedValue>("sopartsublines.tss_paidvalue").Value) : new Money(0);
                entityToInsertLines["tss_paymenttoleranceallowance"] = Helper.Common.DiffDays(invoiceDueDate, invoiceDate) + paymentTolleranceAllowance ?? 0;
                int overduePayment = 0;
                if (paidDate == null || paidDate == DateTime.MinValue)
                    overduePayment = 0;
                else
                    overduePayment = Helper.Common.DiffDays(paidDate, invoiceDate) - entityToInsertLines.GetAttributeValue<int>("tss_paymenttoleranceallowance");
                entityToInsertLines["tss_overduepayment"] = overduePayment;

                CRMFilterExp = new FilterExpression(LogicalOperator.And);
                CRMFilterExp.AddCondition("tss_startday", ConditionOperator.LessEqual, overduePayment);
                CRMFilterExp.AddCondition("tss_endday", ConditionOperator.GreaterEqual, overduePayment);

                CRMQExp = new QueryExpression("tss_incentivef5collectionfactor");
                CRMQExp.ColumnSet = new ColumnSet(new string[] { "tss_factor" });
                CRMQExp.Criteria.AddFilter(CRMFilterExp);
                Entity incetivef5factor = CRMOrganizationService.RetrieveMultiple(CRMQExp).Entities.FirstOrDefault();
                decimal f5 = 0;
                if (incetivef5factor == null)
                    f5 = 0;
                else
                {
                    if (incetivef5factor.Attributes.Contains("tss_factor"))
                        f5 = incetivef5factor.GetAttributeValue<decimal>("tss_factor");
                }
                entityToInsertLines["tss_f5"] = f5;

                if (EntitySingleRetrieve.Attributes.Contains("tss_f5"))
                    soincentiveToUpdate.Attributes["tss_f5"] = EntitySingleRetrieve.GetAttributeValue<decimal>("tss_f5")+f5;
                else
                    soincentiveToUpdate.Attributes["tss_f5"] = f5;
                CRMOrganizationService.Create(entityToInsertLines);
                CRMOrganizationService.Update(soincentiveToUpdate);
            }
            else
            {

                Entity entityFromRetrieve = EntityMultyRetrieve.Entities.FirstOrDefault();
                Entity entityToUpdate = new Entity(entityFromRetrieve.LogicalName, entityFromRetrieve.Id);
                entityToUpdate["tss_paidamount"] = (sopartheader.Attributes.Contains("sopartsublines.tss_paidvalue")) ? ((Money)sopartheader.GetAttributeValue<AliasedValue>("sopartsublines.tss_paidvalue").Value) : new Money(0);
                CRMOrganizationService.Update(entityToUpdate);
            }
        }

    }
}
