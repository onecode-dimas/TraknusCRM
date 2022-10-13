using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using TrakNusRapidService.DataLayer;
using TrakNusRapidService.Helper;

namespace TrakNusRapidService.Workflow.BusinessLayer
{
    class BL_trs_quotationpartssummary
    {
        #region Constants
        private const string _classname = "BL_trs_quotationpartssummary";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_trs_quotationpartssummary _DL_trs_quotationpartssummary = new DL_trs_quotationpartssummary();
        private QuotationGenerator _quotationGenerator = new QuotationGenerator();
        #endregion

        #region Events
        public void insertIntoQuotationPartSummary(IOrganizationService organizationService, Guid quotationId, Guid partId, int manualQuantity, int tasklistQuantity)
        {
            try
            {
                EntityReference trs_quotationnumber = new EntityReference { 
                    Id = quotationId,
                    LogicalName = "trs_quotation"
                };
                EntityReference trs_partnumber = new EntityReference
                {
                    Id = partId,
                    LogicalName = "trs_masterpart"
                };
                _DL_trs_quotationpartssummary.trs_quotationnumber = trs_quotationnumber.Id;
                _DL_trs_quotationpartssummary.trs_partnumber = trs_partnumber.Id;
                _DL_trs_quotationpartssummary.trs_manualquantity = manualQuantity;
                _DL_trs_quotationpartssummary.trs_tasklistquantity = tasklistQuantity;
                _DL_trs_quotationpartssummary.Insert(organizationService);
            }
            catch (Exception ex)
            {
                throw new Exception("BL_trs_quotationpartssummary.insertIntoQuotationPartSummary" + ex.Message.ToString());
            }
        }
        #endregion

        internal EntityCollection getQuotationPartSummarybyPartNumber(IOrganizationService service, Guid partMasterId, Guid guid)
        {
            try
            {
                QueryExpression qe = new QueryExpression();
                qe.EntityName = _DL_trs_quotationpartssummary.EntityName;
                qe.ColumnSet = new ColumnSet(true);

                qe.Criteria = new FilterExpression();
                qe.Criteria.AddCondition(new ConditionExpression("statecode", ConditionOperator.Equal, "Active"));
                qe.Criteria.AddCondition(new ConditionExpression("trs_quotationnumber", ConditionOperator.Equal, guid));
                qe.Criteria.AddCondition(new ConditionExpression("trs_partnumber", ConditionOperator.Equal, partMasterId));

                return _DL_trs_quotationpartssummary.Select(service, qe);
            }
            catch (Exception ex)
            {
                throw new Exception("getQuotationPartSummarybyPartNumber." + ex.ToString());
            }
        }

        internal void updateIntoQuotationPartSummary(IOrganizationService service, Guid summaryId, int partsManualQty, int summaryQty)
        {
            try
            {
                _DL_trs_quotationpartssummary.trs_manualquantity = partsManualQty;
                _DL_trs_quotationpartssummary.trs_tasklistquantity = summaryQty;
                _DL_trs_quotationpartssummary.Update(service, summaryId);
            }
            catch (Exception ex)
            {
                throw new Exception("BL_trs_quotationpartssummary.updateIntoQuotationPartSummary" + ex.Message.ToString());
            }
        }

        #region Publics
        /* Add by Thomas - 17 March 2015 */
        public void SummarizeParts(IOrganizationService organizationService, ITracingService tracingService, Guid id)
        {
            try
            {
                _quotationGenerator.SummarizeParts(organizationService, id);
                _quotationGenerator.SummarizeToolGroups(organizationService, id);
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(_classname + ".SummarizeParts : " + ex.Message);
            }
        }
        /* End add by Thomas - 17 March 2015 */
        #endregion
    }
}
