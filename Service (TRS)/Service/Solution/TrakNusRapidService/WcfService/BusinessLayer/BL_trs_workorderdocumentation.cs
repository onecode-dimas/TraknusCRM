using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using TrakNusRapidService.DataLayer;

namespace TrakNusRapidServiceWcfService.BusinessLayer
{
    public class BL_trs_workorderdocumentation
    {
        #region Constants
        private const string _classname = "BL_trs_workorderdocumentation";
        #endregion

        #region Dependencies
        private DL_trs_workorderdocumentation _DL_trs_workorderdocumentation = new DL_trs_workorderdocumentation();
        #endregion

        #region Privates
        #endregion

        #region Publics
        public void RecordWODocumentation(IOrganizationService organizationService
            , Guid mobileGuid, Guid woId, Uri url, string description
            , DateTime modifiedOn, DateTime syncTime)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_trs_workorderdocumentation.EntityName);
                queryExpression.ColumnSet = new ColumnSet(true);
                FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                filterExpression.AddCondition("trs_workorderid", ConditionOperator.Equal, woId);
                filterExpression.AddCondition("trs_mobileguid", ConditionOperator.Equal, mobileGuid.ToString());
                EntityCollection ecolWODocumentation = _DL_trs_workorderdocumentation.Select(organizationService, queryExpression);
                if (ecolWODocumentation.Entities.Count > 0)
                {
                    Entity eWODocumentation = ecolWODocumentation.Entities[0];
                    if ((DateTime)eWODocumentation.Attributes["modifiedon"] < modifiedOn)
                    {
                        _DL_trs_workorderdocumentation = new DL_trs_workorderdocumentation();
                        _DL_trs_workorderdocumentation.trs_url = url;
                        _DL_trs_workorderdocumentation.trs_description = description;
                        _DL_trs_workorderdocumentation.trs_synctime = syncTime;
                        _DL_trs_workorderdocumentation.trs_frommobile = true;
                        _DL_trs_workorderdocumentation.trs_workshop = false;
                        _DL_trs_workorderdocumentation.Update(organizationService, eWODocumentation.Id);
                    }
                    else
                    {
                        throw new Exception("CRM more update.");
                    }
                }
                else
                {
                    _DL_trs_workorderdocumentation = new DL_trs_workorderdocumentation();
                    _DL_trs_workorderdocumentation.trs_workorderid = woId;
                    _DL_trs_workorderdocumentation.trs_mobileguid = mobileGuid.ToString();
                    _DL_trs_workorderdocumentation.trs_url = url;
                    _DL_trs_workorderdocumentation.trs_description = description;
                    _DL_trs_workorderdocumentation.trs_synctime = syncTime;
                    _DL_trs_workorderdocumentation.trs_frommobile = true;
                    _DL_trs_workorderdocumentation.trs_workshop = false;
                    _DL_trs_workorderdocumentation.Insert(organizationService);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".RecordWODocumentation : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}