using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using TrakNusRapidService.DataLayer;

namespace TrakNusRapidService.Workflow.BusinessLayer
{
    class BL_trs_quotationpartdetail
    {
        #region Constants
        private const string _classname = "BL_trs_quotationpartdetail";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_trs_quotationcommercialdetail _DL_trs_quotationcommercialdetail = new DL_trs_quotationcommercialdetail();
        private DL_trs_quotationpartdetail _DL_trs_quotationpartdetail = new DL_trs_quotationpartdetail();
        #endregion

        #region Events
        //// Get all part details related only to quotation
        //public EntityCollection getAllPartsNotRelatedtoCommercialDetail(IOrganizationService organizationService, Guid quotationId)
        //{
        //    EntityCollection relatedEntities = null;
        //    try
        //    {
        //        QueryExpression qe = new QueryExpression();
        //        qe.EntityName = _DL_trs_quotationpartdetail.EntityName;
        //        qe.ColumnSet = new ColumnSet(true);

        //        Relationship relationship = new Relationship();

        //        qe.Criteria = new FilterExpression();
        //        qe.Criteria.AddCondition(new ConditionExpression("statecode", ConditionOperator.Equal, "Active"));
        //        qe.Criteria.AddCondition(new ConditionExpression("trs_quotationid", ConditionOperator.Equal, quotationId));
        //        qe.Criteria.AddCondition(new ConditionExpression("trs_commercialdetailid", ConditionOperator.Null));

        //        relatedEntities = _DL_trs_quotationpartdetail.Select(organizationService, qe);                
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("BL_trs_quotationpartdetail.getAllRelatedPartDetails" + ex.Message.ToString());
        //    }

        //    return relatedEntities;
        //}

        // Get all part details related only to quotation and quotation commercial details
        public EntityCollection getAllPartsRelatedtoCommercialDetail(IOrganizationService organizationService, Guid quotationId)
        {
            EntityCollection relatedEntities = null;
            try
            {
                QueryExpression qe = new QueryExpression();
                qe.EntityName = _DL_trs_quotationpartdetail.EntityName;
                qe.ColumnSet = new ColumnSet(true);

                LinkEntity leQuotationCommercialDetail = new LinkEntity();
                leQuotationCommercialDetail.LinkFromEntityName = _DL_trs_quotationpartdetail.EntityName;
                leQuotationCommercialDetail.LinkFromAttributeName = "trs_commercialdetailid";
                leQuotationCommercialDetail.LinkToEntityName =_DL_trs_quotationcommercialdetail.EntityName;
                leQuotationCommercialDetail.LinkToAttributeName = "trs_quotationcommercialdetailid";
                leQuotationCommercialDetail.JoinOperator = JoinOperator.Inner;
                leQuotationCommercialDetail.EntityAlias = "trs_quotationcommercialdetail";
                leQuotationCommercialDetail.LinkCriteria.AddCondition("trs_quotation", ConditionOperator.Equal, quotationId);

                qe.Criteria = new FilterExpression();
                qe.LinkEntities.Add(leQuotationCommercialDetail);
                qe.Criteria.AddCondition(new ConditionExpression("statecode", ConditionOperator.Equal, "Active"));

                relatedEntities = _DL_trs_quotationpartdetail.Select(organizationService, qe);
            }
            catch (Exception ex)
            {
                throw new Exception("BL_trs_quotationpartdetail.getAllRelatedQuotationDetailsPartDetailss" + ex.Message.ToString());
            }

            return relatedEntities;
        }
        #endregion
    }
}
