using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using TrakNusRapidService.DataLayer;
using TrakNusRapidService.Helper;

namespace TrakNusRapidServiceWcfService.BusinessLayer
{
    public class BL_trs_ppmreport
    {
        #region Constants
        private const string _classname = "BL_trs_ppmreport";
        #endregion

        #region Dependencies
        private DL_trs_ppmreport _DL_trs_ppmreport = new DL_trs_ppmreport();
        #endregion

        #region Privates
        #endregion

        #region Publics
        public void RecordPPMInsReport(IOrganizationService organizationService
            , Guid mobileGuid, Guid woId, int reportType
            , Guid productTypeId, Guid populationId, Guid typeofWorkId
            , int comments, int machineCondition, int typeofSoil, int application
            , string analysis, DateTime repairDate, DateTime troubleDate
            , DateTime modifiedOn, DateTime syncTime)
        {
            try
            {
                if (reportType == Configuration.ReportType_Inspection || reportType == Configuration.ReportType_PPM)
                {
                    QueryExpression queryExpression = new QueryExpression(_DL_trs_ppmreport.EntityName);
                    queryExpression.ColumnSet = new ColumnSet(true);
                    FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                    filterExpression.AddCondition("trs_workorder", ConditionOperator.Equal, woId);
                    filterExpression.AddCondition("trs_mobileguid", ConditionOperator.Equal, mobileGuid.ToString());
                    EntityCollection entityCollection = _DL_trs_ppmreport.Select(organizationService, queryExpression);
                    if (entityCollection.Entities.Count > 0)
                    {
                        Entity entity = entityCollection.Entities[0];
                        if ((DateTime)entity.Attributes["modifiedon"] < modifiedOn)
                        {
                            _DL_trs_ppmreport = new DL_trs_ppmreport();
                            _DL_trs_ppmreport.trs_producttype = productTypeId;
                            _DL_trs_ppmreport.trs_typeofwork = typeofWorkId;
                            _DL_trs_ppmreport.trs_comments = comments;
                            _DL_trs_ppmreport.trs_machinecondition = machineCondition;
                            _DL_trs_ppmreport.trs_typeofsoil = typeofSoil;
                            _DL_trs_ppmreport.trs_application = application;
                            _DL_trs_ppmreport.trs_analysis = analysis;
                            _DL_trs_ppmreport.trs_repairdate = repairDate;
                            _DL_trs_ppmreport.trs_troubledate = troubleDate;
                            _DL_trs_ppmreport.trs_synctime = syncTime;
                            _DL_trs_ppmreport.trs_frommobile = true;
                            _DL_trs_ppmreport.trs_workshop = false;
                            _DL_trs_ppmreport.Update(organizationService, entity.Id);
                        }
                    }
                    else
                    {
                        _DL_trs_ppmreport = new DL_trs_ppmreport();
                        _DL_trs_ppmreport.trs_workorder = woId;
                        _DL_trs_ppmreport.trs_producttype = productTypeId;
                        _DL_trs_ppmreport.trs_new_population = populationId;
                        _DL_trs_ppmreport.trs_typeofwork = typeofWorkId;
                        _DL_trs_ppmreport.trs_comments = comments;
                        _DL_trs_ppmreport.trs_machinecondition = machineCondition;
                        _DL_trs_ppmreport.trs_typeofsoil = typeofSoil;
                        _DL_trs_ppmreport.trs_application = application;
                        _DL_trs_ppmreport.trs_analysis = analysis;
                        _DL_trs_ppmreport.trs_repairdate = repairDate;
                        _DL_trs_ppmreport.trs_troubledate = troubleDate;
                        _DL_trs_ppmreport.trs_synctime = syncTime;
                        _DL_trs_ppmreport.trs_frommobile = true;
                        _DL_trs_ppmreport.trs_workshop = false;
                        _DL_trs_ppmreport.trs_type = reportType;
                        _DL_trs_ppmreport.trs_mobileguid = mobileGuid.ToString();
                        _DL_trs_ppmreport.Insert(organizationService);
                    }
                }
                else
                {
                    throw new Exception("Can not found Report Type !");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".RecordPPMInsReport : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}