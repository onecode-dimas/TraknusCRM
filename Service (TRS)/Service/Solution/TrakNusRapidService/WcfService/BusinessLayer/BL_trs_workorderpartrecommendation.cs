using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using TrakNusRapidService.DataLayer;

namespace TrakNusRapidServiceWcfService.BusinessLayer
{
    public class BL_trs_workorderpartrecommendation
    {
        #region Constants
        private const string _classname = "BL_trs_workorderpartrecommendation";
        #endregion

        #region Dependencies
        private DL_trs_workorderpartrecommendation _DL_trs_workorderpartrecommendation = new DL_trs_workorderpartrecommendation();
        private DL_trs_masterpart _DL_trs_masterpart = new DL_trs_masterpart();
        #endregion

        #region Privates
        #endregion

        #region Publics
        public void RecordRecommendation(IOrganizationService organizationService
            , Guid mobileGuid, Guid woId, Guid sectionId, Guid? taskListHeaderId, Guid? taskListDetailId
            , Guid? partId, string partNumberCatalog, int quantity
            , DateTime modifiedOn, DateTime syncTime)
        {
            try
            {

                Entity ePart = null;
                if (partId != null)
                    ePart = _DL_trs_masterpart.Select(organizationService, (Guid)partId);

                QueryExpression queryExpression = new QueryExpression(_DL_trs_workorderpartrecommendation.EntityName);
                queryExpression.ColumnSet = new ColumnSet(true);
                FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                filterExpression.AddCondition("trs_workorder", ConditionOperator.Equal, woId);
                filterExpression.AddCondition("trs_mobileguid", ConditionOperator.Equal, mobileGuid.ToString());
                EntityCollection ecolRecommendation = _DL_trs_workorderpartrecommendation.Select(organizationService, queryExpression);
                if (ecolRecommendation.Entities.Count > 0)
                {
                    Entity eRecommendation = ecolRecommendation.Entities[0];
                    if ((DateTime)eRecommendation.Attributes["modifiedon"] < modifiedOn)
                    {
                        _DL_trs_workorderpartrecommendation = new DL_trs_workorderpartrecommendation();
                        if (partId != null)
                        {
                            _DL_trs_workorderpartrecommendation.trs_partdescription = ePart.GetAttributeValue<string>("trs_partdescription");
                            _DL_trs_workorderpartrecommendation.trs_partnumber = (Guid)partId;
                        }
                        _DL_trs_workorderpartrecommendation.trs_section = sectionId;
                        if (taskListDetailId != null)
                        {
                            _DL_trs_workorderpartrecommendation.trs_task = (Guid)taskListHeaderId;
                            _DL_trs_workorderpartrecommendation.trs_tasklistdetailid = (Guid)taskListDetailId;
                        }
                        _DL_trs_workorderpartrecommendation.trs_quantity = quantity;
                        _DL_trs_workorderpartrecommendation.trs_synctime = syncTime;
                        _DL_trs_workorderpartrecommendation.trs_frommobile = true;
                        _DL_trs_workorderpartrecommendation.trs_partnumbercatalog = partNumberCatalog;
                        _DL_trs_workorderpartrecommendation.Update(organizationService, eRecommendation.Id);
                    }
                    else
                    {
                        throw new Exception("CRM more update.");
                    }
                }
                else
                {
                    _DL_trs_workorderpartrecommendation = new DL_trs_workorderpartrecommendation();
                    if (partId != null)
                    {
                        _DL_trs_workorderpartrecommendation.trs_partdescription = ePart.GetAttributeValue<string>("trs_partdescription");
                        _DL_trs_workorderpartrecommendation.trs_partnumber = (Guid)partId;
                    }
                    _DL_trs_workorderpartrecommendation.trs_workorder = woId;
                    _DL_trs_workorderpartrecommendation.trs_mobileguid = mobileGuid.ToString();
                    _DL_trs_workorderpartrecommendation.trs_section = sectionId;
                    if (taskListDetailId != null)
                    {
                        _DL_trs_workorderpartrecommendation.trs_task = (Guid)taskListHeaderId;
                        _DL_trs_workorderpartrecommendation.trs_tasklistdetailid = (Guid)taskListDetailId;
                    }
                    _DL_trs_workorderpartrecommendation.trs_quantity = quantity;
                    _DL_trs_workorderpartrecommendation.trs_synctime = syncTime;
                    _DL_trs_workorderpartrecommendation.trs_frommobile = true;
                    _DL_trs_workorderpartrecommendation.trs_partnumbercatalog = partNumberCatalog;
                    _DL_trs_workorderpartrecommendation.Insert(organizationService);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".RecordRecommendation : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}