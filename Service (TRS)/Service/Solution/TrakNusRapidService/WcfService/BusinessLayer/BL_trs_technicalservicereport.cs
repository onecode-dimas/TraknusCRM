using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using TrakNusRapidService.DataLayer;

namespace TrakNusRapidServiceWcfService.BusinessLayer
{
    public class BL_trs_technicalservicereport
    {
        #region Constants
        private const string _classname = "BL_trs_technicalservicereport";
        #endregion

        #region Dependencies
        private DL_trs_technicalservicereport _DL_trs_technicalservicereport = new DL_trs_technicalservicereport();
        #endregion

        #region Privates
        #endregion

        #region Publics
        public void RecordTSR(IOrganizationService organizationService
            , Guid mobileGuid, Guid woId, Guid populationId
            , string symptom, bool operatingCondition, string conditionDescription
            , Guid? productTypeId, int? application, int? typeofSoil
            , DateTime repairDate, DateTime troubleDate, Guid? partsCausedId
            , string technicalAnalysis, string correctionTaken
            , int? gensetType, int? sector, int oldHM
            , DateTime modifiedOn, DateTime syncTime)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_trs_technicalservicereport.EntityName);
                queryExpression.ColumnSet = new ColumnSet(true);
                FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                filterExpression.AddCondition("trs_workorder", ConditionOperator.Equal, woId);
                filterExpression.AddCondition("trs_mobileguid", ConditionOperator.Equal, mobileGuid.ToString());
                EntityCollection entityCollection = _DL_trs_technicalservicereport.Select(organizationService, queryExpression);
                if (entityCollection.Entities.Count > 0)
                {
                    Entity entity = entityCollection.Entities[0];
                    if ((DateTime)entity.Attributes["modifiedon"] < modifiedOn)
                    {
                        _DL_trs_technicalservicereport = new DL_trs_technicalservicereport();
                        _DL_trs_technicalservicereport.trs_symptom = symptom;
                        _DL_trs_technicalservicereport.trs_operatingcondition = operatingCondition;
                        _DL_trs_technicalservicereport.trs_conditiondescription = conditionDescription;
                        if (productTypeId != null)
                        _DL_trs_technicalservicereport.trs_producttype = (Guid)productTypeId;
                        if (application != null)
                            _DL_trs_technicalservicereport.trs_application = (int)application;
                        if (typeofSoil != null)
                            _DL_trs_technicalservicereport.trs_typeofsoil = (int)typeofSoil;
                        _DL_trs_technicalservicereport.trs_repairdate = repairDate;
                        _DL_trs_technicalservicereport.trs_troubledate = troubleDate;
                        if (partsCausedId != null)
                            _DL_trs_technicalservicereport.trs_partscaused = (Guid)partsCausedId;
                        _DL_trs_technicalservicereport.trs_technicalanalysis = technicalAnalysis;
                        _DL_trs_technicalservicereport.trs_correctiontaken = correctionTaken;
                        if (gensetType != null)
                            _DL_trs_technicalservicereport.trs_gensettype = (int)gensetType;
                        if (sector != null)
                            _DL_trs_technicalservicereport.trs_sector = (int)sector;
                        _DL_trs_technicalservicereport.trs_synctime = syncTime;
                        _DL_trs_technicalservicereport.trs_frommobile = true;
                        _DL_trs_technicalservicereport.trs_workshop = false;
                        _DL_trs_technicalservicereport.Update(organizationService, entity.Id);
                    }
                    else
                    {
                        throw new Exception("CRM more update.");
                    }
                }
                else
                {
                    _DL_trs_technicalservicereport = new DL_trs_technicalservicereport();
                    _DL_trs_technicalservicereport.trs_workorder = woId;
                    _DL_trs_technicalservicereport.trs_equipment = populationId;
                    _DL_trs_technicalservicereport.trs_symptom = symptom;
                    _DL_trs_technicalservicereport.trs_operatingcondition = operatingCondition;
                    _DL_trs_technicalservicereport.trs_conditiondescription = conditionDescription;
                    if (productTypeId != null)
                        _DL_trs_technicalservicereport.trs_producttype = (Guid)productTypeId;
                    if (application != null)
                        _DL_trs_technicalservicereport.trs_application = (int)application;
                    if (typeofSoil != null)
                        _DL_trs_technicalservicereport.trs_typeofsoil = (int)typeofSoil;
                    _DL_trs_technicalservicereport.trs_repairdate = repairDate;
                    _DL_trs_technicalservicereport.trs_troubledate = troubleDate;
                    if (partsCausedId != null)
                        _DL_trs_technicalservicereport.trs_partscaused = (Guid)partsCausedId;
                    _DL_trs_technicalservicereport.trs_technicalanalysis = technicalAnalysis;
                    _DL_trs_technicalservicereport.trs_correctiontaken = correctionTaken;
                    _DL_trs_technicalservicereport.trs_oldhm = oldHM;
                    _DL_trs_technicalservicereport.trs_synctime = syncTime;
                    _DL_trs_technicalservicereport.trs_frommobile = true;
                    _DL_trs_technicalservicereport.trs_workshop = false;
                    _DL_trs_technicalservicereport.trs_mobileguid = mobileGuid.ToString();
                    _DL_trs_technicalservicereport.Insert(organizationService);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".RecordTSR : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}