using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using TrakNusRapidService.DataLayer;

namespace TrakNusRapidServiceWcfService.BusinessLayer
{
    public class BL_trs_tsrpartsdamageddetail
    {
        #region Constants
        private const string _classname = "BL_trs_tsrpartsdamageddetail";
        #endregion

        #region Dependencies
        private DL_trs_tsrpartsdamageddetail _DL_trs_tsrpartsdamageddetail = new DL_trs_tsrpartsdamageddetail();
        private DL_trs_technicalservicereport _DL_trs_technicalservicereport = new DL_trs_technicalservicereport();
        #endregion

        #region Privates
        #endregion

        #region Publics
        public void RecordTSR_PartDamaged(IOrganizationService organizationService
            , Guid mobileGuid, Guid woId, Guid partId, string partNumberCatalog, int quantity
            , DateTime modifiedOn, DateTime syncTime)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_trs_tsrpartsdamageddetail.EntityName);
                queryExpression.ColumnSet = new ColumnSet(true);
                FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                filterExpression.AddCondition("trs_wonumber", ConditionOperator.Equal, woId);
                filterExpression.AddCondition("trs_mobileguid", ConditionOperator.Equal, mobileGuid.ToString());
                EntityCollection ecolPartsDamagedDetail = _DL_trs_tsrpartsdamageddetail.Select(organizationService, queryExpression);
                if (ecolPartsDamagedDetail.Entities.Count > 0)
                {
                    Entity ePartsDamagedDetail = ecolPartsDamagedDetail.Entities[0];
                    if ((DateTime)ePartsDamagedDetail.Attributes["modifiedon"] < modifiedOn)
                    {
                        _DL_trs_tsrpartsdamageddetail = new DL_trs_tsrpartsdamageddetail();
                        _DL_trs_tsrpartsdamageddetail.trs_partnumber = partId;
                        _DL_trs_tsrpartsdamageddetail.trs_quantity = quantity;
                        _DL_trs_tsrpartsdamageddetail.trs_synctime = syncTime;
                        _DL_trs_tsrpartsdamageddetail.trs_frommobile = true;
                        _DL_trs_tsrpartsdamageddetail.trs_partnumbercatalog = partNumberCatalog;
                        _DL_trs_tsrpartsdamageddetail.Update(organizationService, ePartsDamagedDetail.Id);
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
                        _DL_trs_tsrpartsdamageddetail = new DL_trs_tsrpartsdamageddetail();
                        _DL_trs_tsrpartsdamageddetail.trs_tsrnumber = ecolTSR.Entities[0].Id;
                        _DL_trs_tsrpartsdamageddetail.trs_wonumber = woId;
                        _DL_trs_tsrpartsdamageddetail.trs_mobileguid = mobileGuid.ToString();
                        _DL_trs_tsrpartsdamageddetail.trs_partnumber = partId;
                        _DL_trs_tsrpartsdamageddetail.trs_quantity = quantity;
                        _DL_trs_tsrpartsdamageddetail.trs_synctime = syncTime;
                        _DL_trs_tsrpartsdamageddetail.trs_frommobile = true;
                        _DL_trs_tsrpartsdamageddetail.trs_partnumbercatalog = partNumberCatalog;
                        _DL_trs_tsrpartsdamageddetail.Insert(organizationService);
                    }
                    else
                    {
                        throw new Exception("Can not found Technical Service Report for WO with Id : '" + woId.ToString() + "'");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".RecordTSR_PartDamaged : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}