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
    public class BL_trs_mtar
    {
        #region Constants
        private const string _classname = "BL_trs_mtar";
        #endregion

        #region Dependencies
        DL_trs_mtar _DL_trs_mtar = new DL_trs_mtar();
        DL_serviceappointment _DL_serviceappointment = new DL_serviceappointment();
        FMath _fMath = new FMath();
        #endregion

        #region Privates
        #endregion

        #region Publics
        public void RecordMTAR(IOrganizationService organizationService, Guid woId, Guid mechanidId, Guid mobileGuid
            , int status, string remarks, decimal longitude, decimal latitude
            , DateTime automaticTime, DateTime? manualTime
            , DateTime modifiedOn, DateTime syncTime, bool background)
        {
            try
            {
                if (woId == Guid.Empty)
                    throw new Exception("WO Id can not empty !");

                if (!background && (status == Configuration.MTAR_Hold || status == Configuration.MTAR_Resume || status == Configuration.MTAR_SubmitTeco))
                {
                    throw new Exception("Please use the specialized function for this status !");
                }
                else
                {
                    QueryExpression queryExpression = new QueryExpression(_DL_trs_mtar.EntityName);
                    queryExpression.ColumnSet = new ColumnSet(true);
                    FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                    filterExpression.AddCondition("trs_workorder", ConditionOperator.Equal, woId);
                    filterExpression.AddCondition("trs_mobileguid", ConditionOperator.Equal, mobileGuid.ToString());
                    EntityCollection ecolMTAR = _DL_trs_mtar.Select(organizationService, queryExpression);
                    if (ecolMTAR.Entities.Count > 0)
                    {
                        Entity eMTAR = ecolMTAR.Entities[0];
                        if ((DateTime)eMTAR.Attributes["modifiedon"] < modifiedOn)
                        {
                            _DL_trs_mtar = new DL_trs_mtar();
                            _DL_trs_mtar.trs_mtarstatus = status;
                            _DL_trs_mtar.trs_statusremarks = remarks;
                            _DL_trs_mtar.trs_longitude = _fMath.Truncate(longitude, Configuration.GPSMaxDigit);
                            _DL_trs_mtar.trs_latitude = _fMath.Truncate(latitude, Configuration.GPSMaxDigit);
                            _DL_trs_mtar.trs_automatictime = automaticTime;
                            if (manualTime != null)
                                _DL_trs_mtar.trs_manualtime = (DateTime)manualTime;
                            _DL_trs_mtar.trs_synctime = syncTime;
                            _DL_trs_mtar.trs_frommobile = true;
                            _DL_trs_mtar.trs_workshop = false;
                            _DL_trs_mtar.trs_updatewostatus = false;
                            _DL_trs_mtar.Update(organizationService, eMTAR.Id);
                        }
                        else
                        {
                            throw new Exception("CRM more update.");
                        }
                    }
                    else
                    {
                        FMobile _fMobile = new FMobile(organizationService);

                        _DL_trs_mtar = new DL_trs_mtar();
                        _DL_trs_mtar.trs_name = _fMobile.ConvertMTARtoWords(status);
                        _DL_trs_mtar.trs_mobileguid = mobileGuid.ToString();
                        _DL_trs_mtar.trs_workorder = woId;
                        _DL_trs_mtar.trs_mechanic = mechanidId;
                        _DL_trs_mtar.trs_mtarstatus = _fMobile.ConvertMTAR_MobiletoCRM(status);
                        _DL_trs_mtar.trs_statusremarks = remarks;
                        _DL_trs_mtar.trs_longitude = _fMath.Truncate(longitude, Configuration.GPSMaxDigit);
                        _DL_trs_mtar.trs_latitude = _fMath.Truncate(latitude, Configuration.GPSMaxDigit);
                        if (automaticTime != DateTime.MinValue)
                            _DL_trs_mtar.trs_automatictime = automaticTime;
                        if (manualTime != null)
                            _DL_trs_mtar.trs_manualtime = (DateTime)manualTime;
                        _DL_trs_mtar.trs_synctime = syncTime;
                        _DL_trs_mtar.trs_frommobile = true;
                        _DL_trs_mtar.trs_workshop = false;
                        _DL_trs_mtar.Insert(organizationService);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".RecordMTAR : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}