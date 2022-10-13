using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using TrakNusRapidService.DataLayer;

namespace TrakNusRapidServiceWcfService.BusinessLayer
{
    public class BL_trs_technicalservicereportdocumentation
    {
        #region Constants
        private const string _classname = "BL_trs_technicalservicereportdocumentation";
        #endregion

        #region Dependencies
        private DL_trs_technicalservicereportdocumentation _DL_trs_technicalservicereportdocumentation = new DL_trs_technicalservicereportdocumentation();
        private DL_trs_technicalservicereport _DL_trs_technicalservicereport = new DL_trs_technicalservicereport();
        #endregion

        #region Privates
        #endregion

        #region Publics
        public void RecordTSRDocumentation(IOrganizationService organizationService
            , Guid mobileGuid, Guid woId, Uri url, string description
            , DateTime modifiedOn, DateTime syncTime)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_trs_technicalservicereportdocumentation.EntityName);
                queryExpression.ColumnSet = new ColumnSet(true);
                FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                filterExpression.AddCondition("trs_workorder", ConditionOperator.Equal, woId);
                filterExpression.AddCondition("trs_mobileguid", ConditionOperator.Equal, mobileGuid.ToString());
                EntityCollection ecolTSRDocumentation = _DL_trs_technicalservicereportdocumentation.Select(organizationService, queryExpression);
                if (ecolTSRDocumentation.Entities.Count > 0)
                {
                    Entity eTSRDocumentation = ecolTSRDocumentation.Entities[0];
                    if ((DateTime)eTSRDocumentation.Attributes["modifiedon"] < modifiedOn)
                    {
                        _DL_trs_technicalservicereportdocumentation = new DL_trs_technicalservicereportdocumentation();
                        _DL_trs_technicalservicereportdocumentation.trs_url = url;
                        _DL_trs_technicalservicereportdocumentation.trs_description = description;
                        _DL_trs_technicalservicereportdocumentation.trs_synctime = syncTime;
                        _DL_trs_technicalservicereportdocumentation.trs_frommobile = true;
                        _DL_trs_technicalservicereportdocumentation.trs_workshop = false;
                        _DL_trs_technicalservicereportdocumentation.Update(organizationService, eTSRDocumentation.Id);
                    }
                    else
                    {
                        throw new Exception("CRM more update.");
                    }
                }
                else
                {
                    queryExpression = new QueryExpression(_DL_trs_technicalservicereport.EntityName);
                    queryExpression.Criteria.AddCondition("trs_workorder", ConditionOperator.Equal, woId);
                    EntityCollection ecolTSR = _DL_trs_technicalservicereport.Select(organizationService, queryExpression);
                    if (ecolTSR.Entities.Count > 0)
                    {
                        _DL_trs_technicalservicereportdocumentation = new DL_trs_technicalservicereportdocumentation();
                        _DL_trs_technicalservicereportdocumentation.trs_technicalservicereportid = ecolTSR.Entities[0].Id;
                        _DL_trs_technicalservicereportdocumentation.trs_workorderid = woId;
                        _DL_trs_technicalservicereportdocumentation.trs_mobileguid = mobileGuid.ToString();
                        _DL_trs_technicalservicereportdocumentation.trs_url = url;
                        _DL_trs_technicalservicereportdocumentation.trs_description = description;
                        _DL_trs_technicalservicereportdocumentation.trs_synctime = syncTime;
                        _DL_trs_technicalservicereportdocumentation.trs_frommobile = true;
                        _DL_trs_technicalservicereportdocumentation.trs_workshop = false;
                        _DL_trs_technicalservicereportdocumentation.Insert(organizationService);
                    }
                    else
                    {
                        throw new Exception("Can not found Technical Service Report for WO with Id : '" + woId.ToString() + "'");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".RecordTSRDocumentation : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}