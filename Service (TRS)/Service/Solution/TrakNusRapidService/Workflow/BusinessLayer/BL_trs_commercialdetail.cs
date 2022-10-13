using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using TrakNusRapidService.DataLayer;

namespace TrakNusRapidService.Workflow.BusinessLayer
{
    public class BL_trs_commercialdetail
    {
        #region Constants
        private const string _classname = "BL_trs_commercialdetail";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_trs_commercialdetail _DL_trs_commercialdetail = new DL_trs_commercialdetail();
        #endregion

        #region Events
        public EntityCollection getAllRelatedCommercialDetailId(IOrganizationService service, Guid trs_commercialheaderid)
        {
            EntityCollection relatedEntities = null;
            try
            {
                QueryExpression qe = new QueryExpression();
                qe.EntityName = "trs_commercialdetail";
                qe.ColumnSet = new ColumnSet(true);

                Relationship relationship = new Relationship();

                qe.Criteria = new FilterExpression();
                qe.Criteria.AddCondition(new ConditionExpression("statecode", ConditionOperator.Equal, "Active"));
                qe.Criteria.AddCondition(new ConditionExpression("trs_commercialheaderid", ConditionOperator.Equal, trs_commercialheaderid.ToString()));

                relatedEntities = _DL_trs_commercialdetail.Select(service, qe);               
            }
            catch (Exception ex)
            {
                throw new Exception("BL_trs_commercialdetail.getAllRelatedCommercialDetailId" + ex.Message.ToString());
            }

            return relatedEntities;
        }

        public EntityCollection getAllRelatedCommercialDetailStatusIncomplete(IOrganizationService service, Guid id, int inComplete)
        {
            EntityCollection relatedEntities = null;
            try
            {
                QueryExpression qe = new QueryExpression();
                qe.EntityName = "trs_commercialdetail";
                qe.ColumnSet = new ColumnSet(true);

                Relationship relationship = new Relationship();

                qe.Criteria = new FilterExpression();
                qe.Criteria.AddCondition(new ConditionExpression("statuscode", ConditionOperator.Equal, inComplete));
                qe.Criteria.AddCondition(new ConditionExpression("trs_commercialheaderid", ConditionOperator.Equal, id.ToString()));

                relatedEntities = _DL_trs_commercialdetail.Select(service, qe);
            }
            catch (Exception ex)
            {
                throw new Exception("BL_trs_commercialdetail.getAllRelatedCommercialDetailStatusIncomplete" + ex.Message.ToString());
            }

            return relatedEntities;
        }
        #endregion
    }
}
