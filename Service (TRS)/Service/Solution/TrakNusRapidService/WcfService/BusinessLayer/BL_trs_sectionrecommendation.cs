using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using TrakNusRapidService.DataLayer;

namespace TrakNusRapidServiceWcfService.BusinessLayer
{
    public class BL_trs_sectionrecommendation
    {
        #region Constants
        private const string _classname = "BL_trs_sectionrecommendation";
        #endregion

        #region Dependencies
        private DL_trs_sectionrecommendation _DL_trs_sectionrecommendation = new DL_trs_sectionrecommendation();
        private DL_trs_ppmreport _DL_trs_ppmreport = new DL_trs_ppmreport();
        #endregion

        #region Privates
        #endregion

        #region Publics
        public void RecordPPMInsRecommendation(IOrganizationService organizationService, Guid id
            , Guid mobileGuid, Guid woId, Guid sectionId, Guid recommendationId
            , DateTime modifiedOn, DateTime syncTime)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_trs_sectionrecommendation.EntityName);
                queryExpression.ColumnSet = new ColumnSet(true);
                FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                filterExpression.AddCondition("trs_wonumber", ConditionOperator.Equal, woId);
                filterExpression.AddCondition("trs_mobileguid", ConditionOperator.Equal, mobileGuid.ToString());
                
                EntityCollection entityCollection = _DL_trs_sectionrecommendation.Select(organizationService, queryExpression);
                if (entityCollection.Entities.Count > 0)
                {
                    Entity entity = entityCollection.Entities[0];
                    if ((DateTime)entity.Attributes["modifiedon"] < modifiedOn)
                    {
                        _DL_trs_sectionrecommendation = new DL_trs_sectionrecommendation();
                        //_DL_trs_sectionrecommendation.trs_name = name;
                        _DL_trs_sectionrecommendation.trs_section = sectionId;
                        _DL_trs_sectionrecommendation.trs_recommendation = recommendationId;
                        _DL_trs_sectionrecommendation.trs_synctime = syncTime;
                        _DL_trs_sectionrecommendation.trs_frommobile = true;
                        _DL_trs_sectionrecommendation.trs_workshop = false;
                        _DL_trs_sectionrecommendation.Update(organizationService, entity.Id);
                    }
                }
                else
                {
                    queryExpression = new QueryExpression(_DL_trs_ppmreport.EntityName);
                    queryExpression.Criteria.AddCondition("trs_workorder", ConditionOperator.Equal, woId);
                    EntityCollection ePPMInsReport = _DL_trs_ppmreport.Select(organizationService, queryExpression);
                    if (ePPMInsReport.Entities.Count > 0)
                    {
                        _DL_trs_sectionrecommendation = new DL_trs_sectionrecommendation();
                        _DL_trs_sectionrecommendation.trs_reportnumber = ePPMInsReport.Entities[0].Id;
                        _DL_trs_sectionrecommendation.trs_wonumber = woId;
                        //_DL_trs_sectionrecommendation.trs_name = name;
                        _DL_trs_sectionrecommendation.trs_section = sectionId;
                        _DL_trs_sectionrecommendation.trs_recommendation = recommendationId;
                        _DL_trs_sectionrecommendation.trs_synctime = syncTime;
                        _DL_trs_sectionrecommendation.trs_frommobile = true;
                        _DL_trs_sectionrecommendation.trs_workshop = false;
                        _DL_trs_sectionrecommendation.trs_mobileguid = mobileGuid.ToString();
                        _DL_trs_sectionrecommendation.Insert(organizationService);
                    }
                    else
                    {
                        throw new Exception("Can not found PPM Inspection Report for WO with Id = '" + woId.ToString() + "'");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".RecordPPMInsRecommendation : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}