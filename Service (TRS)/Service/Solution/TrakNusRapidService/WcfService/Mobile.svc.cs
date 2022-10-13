using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Reflection;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Client.Services;

using TrakNusRapidService.Helper;

namespace TrakNusRapidServiceWcfService
{
    using BusinessLayer;

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Mobile" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Mobile.svc or Mobile.svc.cs at the Solution Explorer and start debugging.
    public class Mobile : IMobile
    {
        #region Depedencies
        private BL_serviceappointment _BL_serviceappointment = new BL_serviceappointment();
        private BL_new_population _BL_new_population = new BL_new_population();
        private BL_trs_mtar _BL_trs_mtar = new BL_trs_mtar();
        private BL_trs_workorderpartssummary _BL_trs_workorderpartssummary = new BL_trs_workorderpartssummary();
        private BL_trs_workorderpartrecommendation _BL_trs_workorderpartrecommendation = new BL_trs_workorderpartrecommendation();
        private BL_trs_commercialdetail _BL_trs_commercialdetail = new BL_trs_commercialdetail();
        private BL_trs_commercialdetailmechanic _BL_trs_commercialdetailmechanic = new BL_trs_commercialdetailmechanic();
        private BL_trs_ppmreport _BL_trs_ppmreport = new BL_trs_ppmreport();
        private BL_trs_sectionrecommendation _BL_trs_sectionrecommendation = new BL_trs_sectionrecommendation();
        private BL_trs_technicalservicereport _BL_trs_technicalservicereport = new BL_trs_technicalservicereport();
        private BL_trs_tsrpartsdamageddetail _BL_trs_tsrpartsdamageddetail = new BL_trs_tsrpartsdamageddetail();
        private BL_trs_tsrpartdetails _BL_trs_tsrpartdetails = new BL_trs_tsrpartdetails();
        private BL_equipment _BL_equipment = new BL_equipment();
        private BL_trs_voc _BL_trs_voc = new BL_trs_voc();
        private BL_trs_workorderdocumentation _BL_trs_workorderdocumentation = new BL_trs_workorderdocumentation();
        private BL_trs_technicalservicereportdocumentation _BL_trs_technicalservicereportdocumentation = new BL_trs_technicalservicereportdocumentation();
        private CRMConnector crmConnector = null;
        private Helper.MWSLog _mwsLog = new Helper.MWSLog();
        #endregion

        #region Privates
        #endregion

        #region Publics
        #region Population
        public SyncronizeResult UpdatePopulationLocation(DataPopulation dataPopulation)
        {
            SyncronizeResult syncronizedResult = new SyncronizeResult();
            try
            {
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name + _mwsLog.ColumnsSeparator
                    + dataPopulation.WOId.ToString() + _mwsLog.ColumnsSeparator
                    + dataPopulation.PopulationId.ToString() + _mwsLog.ColumnsSeparator
                    + dataPopulation.CustomerId.ToString() + _mwsLog.ColumnsSeparator
                    + dataPopulation.Longitude.ToString() + _mwsLog.ColumnsSeparator
                    + dataPopulation.Latitude.ToString());

                crmConnector = new CRMConnector(_mwsLog.Verbose, _mwsLog);
                crmConnector.ConnectionName = Configuration.ConnectionNameCRM;
                OrganizationService organizationService = crmConnector.Connect();
                Guid userId = crmConnector.GetLoggedUserId(organizationService);
                syncronizedResult.SyncTime = DateTime.Now;

                _BL_new_population.UpdatePopulationLocation((IOrganizationService)organizationService
                    , dataPopulation.WOId
                    , dataPopulation.PopulationId
                    , dataPopulation.CustomerId
                    , dataPopulation.Longitude
                    , dataPopulation.Latitude
                    , dataPopulation.Area
                    , syncronizedResult.SyncTime);
                syncronizedResult.Success = true;
            }
            catch (Exception ex)
            {
                syncronizedResult.Success = false;
                syncronizedResult.ErrorMessage = ex.Message.ToString();
            }
            return syncronizedResult;
        }
        #endregion

        #region Equipment
        public SyncronizeResult UpdateMechanicLocation(DataMechanic_Location dataMechanic)
        {
            SyncronizeResult syncronizedResult = new SyncronizeResult();
            try
            {
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name + _mwsLog.ColumnsSeparator
                    + dataMechanic.MechanicId.ToString() + _mwsLog.ColumnsSeparator
                    + dataMechanic.Longitude.ToString() + _mwsLog.ColumnsSeparator
                    + dataMechanic.Latitude.ToString());

                crmConnector = new CRMConnector(_mwsLog.Verbose, _mwsLog);
                crmConnector.ConnectionName = Configuration.ConnectionNameCRM;
                OrganizationService organizationService = crmConnector.Connect();
                Guid userId = crmConnector.GetLoggedUserId(organizationService);
                syncronizedResult.SyncTime = DateTime.Now;

                _BL_equipment.UpdateMechanicLocation((IOrganizationService)organizationService
                    , dataMechanic.MechanicId
                    , dataMechanic.Longitude
                    , dataMechanic.Latitude
                    , syncronizedResult.SyncTime);
                syncronizedResult.Success = true;
            }
            catch (Exception ex)
            {
                syncronizedResult.Success = false;
                syncronizedResult.ErrorMessage = ex.Message.ToString();
            }
            return syncronizedResult;
        }

        public SyncronizeResult SendMechanicPassword(DataMechanic dataMechanic)
        {
            SyncronizeResult syncronizedResult = new SyncronizeResult();
            try
            {
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name + _mwsLog.ColumnsSeparator + dataMechanic.MechanicId.ToString());

                crmConnector = new CRMConnector(_mwsLog.Verbose, _mwsLog);
                crmConnector.ConnectionName = Configuration.ConnectionNameCRM;
                OrganizationService organizationService = crmConnector.Connect();
                Guid userId = crmConnector.GetLoggedUserId(organizationService);
                Guid newId = Guid.Empty;
                syncronizedResult.SyncTime = DateTime.Now;

                _BL_equipment.SendPasswordMechanic(organizationService
                    , userId
                    , dataMechanic.MechanicId
                    , dataMechanic.Password);
                syncronizedResult.Success = true;
            }
            catch (Exception ex)
            {
                syncronizedResult.Success = false;
                syncronizedResult.ErrorMessage = ex.Message.ToString();
            }
            return syncronizedResult;
        }
        #endregion

        #region Work Order
        public SyncronizeResult UnconfirmedWO(DataWO_Unconfirmed dataWO)
        {
            SyncronizeResult syncronizedResult = new SyncronizeResult();
            try
            {
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name + _mwsLog.ColumnsSeparator
                    + dataWO.WOId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWO.WONumber);

                crmConnector = new CRMConnector(_mwsLog.Verbose, _mwsLog);
                crmConnector.ConnectionName = Configuration.ConnectionNameCRM;
                OrganizationService organizationService = crmConnector.Connect();
                Guid userId = crmConnector.GetLoggedUserId(organizationService);
                syncronizedResult.SyncTime = DateTime.Now;

                _BL_serviceappointment.RejectDispathedServiceAppointment((IOrganizationService)organizationService
                    , userId
                    , dataWO.WOId
                    , dataWO.WONumber
                    , syncronizedResult.SyncTime);
                syncronizedResult.Success = true;
            }
            catch (Exception ex)
            {
                syncronizedResult.Success = false;
                syncronizedResult.ErrorMessage = ex.Message.ToString();
            }
            return syncronizedResult;
        }

        public SyncronizeResult CancelConfirmedWO(DataWO_CancelConfirmedbyMechanic dataWO)
        {
            SyncronizeResult syncronizedResult = new SyncronizeResult();
            try
            {
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name + _mwsLog.ColumnsSeparator
                    + dataWO.WOId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWO.WONumber + _mwsLog.ColumnsSeparator
                    + dataWO.CancelReason);

                crmConnector = new CRMConnector(_mwsLog.Verbose, _mwsLog);
                crmConnector.ConnectionName = Configuration.ConnectionNameCRM;
                OrganizationService organizationService = crmConnector.Connect();
                Guid userId = crmConnector.GetLoggedUserId(organizationService);
                syncronizedResult.SyncTime = DateTime.Now;

                _BL_serviceappointment.CancelDispathedServiceAppointment((IOrganizationService)organizationService
                    , userId
                    , dataWO.WOId
                    , dataWO.WONumber
                    , dataWO.CancelReason
                    , syncronizedResult.SyncTime);
                syncronizedResult.Success = true;
            }
            catch (Exception ex)
            {
                syncronizedResult.Success = false;
                syncronizedResult.ErrorMessage = ex.Message.ToString();
            }
            return syncronizedResult;
        }

        public SyncronizeResult RequestParts(DataWO_RequestParts dataWO)
        {
            SyncronizeResult syncronizedResult = new SyncronizeResult();
            try
            {
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name + _mwsLog.ColumnsSeparator
                    + dataWO.WOId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWO.WONumber);

                crmConnector = new CRMConnector(_mwsLog.Verbose, _mwsLog);
                crmConnector.ConnectionName = Configuration.ConnectionNameCRM;
                OrganizationService organizationService = crmConnector.Connect();
                Guid userId = crmConnector.GetLoggedUserId(organizationService);
                Guid newId = Guid.Empty;
                syncronizedResult.SyncTime = DateTime.Now;

                int i=0;
                string description = "This is notification email to inform you :<br/><br/>";
                description += "For<br/>";
                description += "WO Number : " + dataWO.WONumber + "<br/>";
                description += "PMActType : " + dataWO.PMActType + "<br/>";
                description += "Description : " + dataWO.WODescription + "<br/>";
                description += "Mechanic Leader : " + dataWO.MechanicLeaderName + "<br/>";
                description += "NRP : " + dataWO.MechanicLeaderNRP + "<br/><br/>";
                description += "Have Parts request as listed below:<br/><br/>";
                description += "<table border='0'>";
                description += "<tr><td>No.</td><td>P/N</td><td>Part Description</td><td>Qty</td></tr>";
                foreach (DataPart dataPart in dataWO.ListParts)
                {
                    i++;
                    description += "<tr><td>" + i.ToString() + "</td><td>" + dataPart.PartNumber + "</td><td>" + dataPart.PartDescription +  "</td><td>" + dataPart.Quantity + "</td></tr>";
                }
                description += "</table><br/><br/>";
                description += "Considering this as Part Order Form. If you have any question regarding to this notification, please re-confirm your Mechanic Leader otherwise you can contact Administrator TRS.<br/><br/>";
                description += "Thank you<br/><br/>";
                description += dataWO.RequestTime.ToString("dd-MM-yyyy   HH:mm:ss") + "<br/>";
                description += "Administrator TRS";
                _BL_serviceappointment.RequestParts(organizationService
                    , userId
                    , dataWO.WOId
                    , description);
                syncronizedResult.Success = true;
            }
            catch (Exception ex)
            {
                syncronizedResult.Success = false;
                syncronizedResult.ErrorMessage = ex.Message.ToString();
            }
            return syncronizedResult;
        }

        public SyncronizeResult UpdateWOStatus_Hold(DataWO_Hold dataWO)
        {
            SyncronizeResult syncronizedResult = new SyncronizeResult();
            try
            {
                if (dataWO.ManualTime != null)
                {
                    _mwsLog.Write(MethodBase.GetCurrentMethod().Name + _mwsLog.ColumnsSeparator
                        + dataWO.WOId.ToString() + _mwsLog.ColumnsSeparator
                        + dataWO.Longitude.ToString() + _mwsLog.ColumnsSeparator
                        + dataWO.Latitude.ToString() + _mwsLog.ColumnsSeparator
                        + dataWO.MechanicId.ToString() + _mwsLog.ColumnsSeparator
                        + dataWO.MTARId.ToString() + _mwsLog.ColumnsSeparator
                        + dataWO.Reason + _mwsLog.ColumnsSeparator
                        + dataWO.SystemTime.ToString("yyyy/MM/dd HH:mm:ss") + _mwsLog.ColumnsSeparator
                        + ((DateTime)dataWO.ManualTime).ToString("yyyy/MM/dd HH:mm:ss") + _mwsLog.ColumnsSeparator
                        + dataWO.ModifiedTime.ToString("yyyy/MM/dd HH:mm:ss"));
                }
                else
                {
                    _mwsLog.Write(MethodBase.GetCurrentMethod().Name + _mwsLog.ColumnsSeparator
                        + dataWO.WOId.ToString() + _mwsLog.ColumnsSeparator
                        + dataWO.Longitude.ToString() + _mwsLog.ColumnsSeparator
                        + dataWO.Latitude.ToString() + _mwsLog.ColumnsSeparator
                        + dataWO.MechanicId.ToString() + _mwsLog.ColumnsSeparator
                        + dataWO.MTARId.ToString() + _mwsLog.ColumnsSeparator
                        + dataWO.Reason + _mwsLog.ColumnsSeparator
                        + dataWO.SystemTime.ToString("yyyy/MM/dd HH:mm:ss") + _mwsLog.ColumnsSeparator
                        + "null" + _mwsLog.ColumnsSeparator
                        + dataWO.ModifiedTime.ToString("yyyy/MM/dd HH:mm:ss"));
                }

                crmConnector = new CRMConnector(_mwsLog.Verbose, _mwsLog);
                crmConnector.ConnectionName = Configuration.ConnectionNameCRM;
                OrganizationService organizationService = crmConnector.Connect();
                Guid userId = crmConnector.GetLoggedUserId(organizationService);
                syncronizedResult.SyncTime = DateTime.Now;

                _BL_serviceappointment.UpdateStatus_Hold((IOrganizationService)organizationService
                    , dataWO.WOId
                    , dataWO.MechanicId
                    , dataWO.MTARId
                    , dataWO.Reason
                    , dataWO.Longitude
                    , dataWO.Latitude
                    , dataWO.SystemTime
                    , dataWO.ManualTime
                    , dataWO.ModifiedTime
                    , syncronizedResult.SyncTime);
                syncronizedResult.Success = true;
            }
            catch (Exception ex)
            {
                syncronizedResult.Success = false;
                syncronizedResult.ErrorMessage = ex.Message.ToString();
            }
            return syncronizedResult;
        }

        public SyncronizeResult UpdateWOStatus_Unhold(DataWO_Unhold dataWO)
        {
            SyncronizeResult syncronizedResult = new SyncronizeResult();
            try
            {
                if (dataWO.ManualTime != null)
                {
                    _mwsLog.Write(MethodBase.GetCurrentMethod().Name + _mwsLog.ColumnsSeparator
                        + dataWO.WOId.ToString() + _mwsLog.ColumnsSeparator
                        + dataWO.Longitude.ToString() + _mwsLog.ColumnsSeparator
                        + dataWO.Latitude.ToString() + _mwsLog.ColumnsSeparator
                        + dataWO.MechanicId.ToString() + _mwsLog.ColumnsSeparator
                        + dataWO.MTARId.ToString() + _mwsLog.ColumnsSeparator
                        + dataWO.Reason + _mwsLog.ColumnsSeparator
                        + dataWO.SystemTime.ToString("yyyy/MM/dd HH:mm:ss") + _mwsLog.ColumnsSeparator
                        + ((DateTime)dataWO.ManualTime).ToString("yyyy/MM/dd HH:mm:ss") + _mwsLog.ColumnsSeparator
                        + dataWO.ModifiedTime.ToString("yyyy/MM/dd HH:mm:ss"));
                }
                else
                {
                    _mwsLog.Write(MethodBase.GetCurrentMethod().Name + _mwsLog.ColumnsSeparator
                        + dataWO.WOId.ToString() + _mwsLog.ColumnsSeparator
                        + dataWO.Longitude.ToString() + _mwsLog.ColumnsSeparator
                        + dataWO.Latitude.ToString() + _mwsLog.ColumnsSeparator
                        + dataWO.MechanicId.ToString() + _mwsLog.ColumnsSeparator
                        + dataWO.MTARId.ToString() + _mwsLog.ColumnsSeparator
                        + dataWO.Reason + _mwsLog.ColumnsSeparator
                        + dataWO.SystemTime.ToString("yyyy/MM/dd HH:mm:ss") + _mwsLog.ColumnsSeparator
                        + "null" + _mwsLog.ColumnsSeparator
                        + dataWO.ModifiedTime.ToString("yyyy/MM/dd HH:mm:ss"));
                }

                crmConnector = new CRMConnector(_mwsLog.Verbose, _mwsLog);
                crmConnector.ConnectionName = Configuration.ConnectionNameCRM;
                OrganizationService organizationService = crmConnector.Connect();
                Guid userId = crmConnector.GetLoggedUserId(organizationService);
                syncronizedResult.SyncTime = DateTime.Now;

                _BL_serviceappointment.UpdateStatus_Unhold((IOrganizationService)organizationService
                    , dataWO.WOId
                    , dataWO.MechanicId
                    , dataWO.MTARId
                    , dataWO.Reason
                    , dataWO.Longitude
                    , dataWO.Latitude
                    , dataWO.SystemTime
                    , dataWO.ManualTime
                    , dataWO.ModifiedTime
                    , syncronizedResult.SyncTime);
                syncronizedResult.Success = true;
            }
            catch (Exception ex)
            {
                syncronizedResult.Success = false;
                syncronizedResult.ErrorMessage = ex.Message.ToString();
            }
            return syncronizedResult;
        }

        public SyncronizeResult UpdateWOStatus_SubmitTECO(DataWO_SubmitTECO dataWO)
        {
            if (dataWO.ManualTime != null)
            {
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name + _mwsLog.ColumnsSeparator
                    + dataWO.WOId.ToString() + _mwsLog.ColumnsSeparator
                        + dataWO.InspectorComments + _mwsLog.ColumnsSeparator
                        + dataWO.InspectorSuggestion + _mwsLog.ColumnsSeparator
                        + dataWO.CustomerComments + _mwsLog.ColumnsSeparator
                        + dataWO.CustomerSatisfaction + _mwsLog.ColumnsSeparator
                        + dataWO.DocumentDate + _mwsLog.ColumnsSeparator
                        + dataWO.HourMeter + _mwsLog.ColumnsSeparator
                        + "Status : " + dataWO.Status + _mwsLog.ColumnsSeparator
                        + dataWO.Longitude + _mwsLog.ColumnsSeparator
                        + dataWO.Latitude + _mwsLog.ColumnsSeparator
                        + dataWO.MechanicId.ToString() + _mwsLog.ColumnsSeparator
                        + dataWO.MTARId.ToString() + _mwsLog.ColumnsSeparator
                        + dataWO.Reason + _mwsLog.ColumnsSeparator
                        + dataWO.SystemTime.ToString("yyyy/MM/dd HH:mm:ss") + _mwsLog.ColumnsSeparator
                        + dataWO.ManualTime.Value.ToString("yyyy/MM/dd HH:mm:ss") + _mwsLog.ColumnsSeparator
                        + dataWO.ModifiedTime.ToString("yyyy/MM/dd HH:mm:ss"));
            }
            else
            {
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name + _mwsLog.ColumnsSeparator
                    + dataWO.WOId.ToString() + _mwsLog.ColumnsSeparator
                        + dataWO.InspectorComments + _mwsLog.ColumnsSeparator
                        + dataWO.InspectorSuggestion + _mwsLog.ColumnsSeparator
                        + dataWO.CustomerComments + _mwsLog.ColumnsSeparator
                        + dataWO.CustomerSatisfaction + _mwsLog.ColumnsSeparator
                        + dataWO.DocumentDate + _mwsLog.ColumnsSeparator
                        + dataWO.HourMeter + _mwsLog.ColumnsSeparator
                        + "Status : " + dataWO.Status + _mwsLog.ColumnsSeparator
                        + dataWO.Longitude + _mwsLog.ColumnsSeparator
                        + dataWO.Latitude + _mwsLog.ColumnsSeparator
                        + dataWO.MechanicId.ToString() + _mwsLog.ColumnsSeparator
                        + dataWO.MTARId.ToString() + _mwsLog.ColumnsSeparator
                        + dataWO.Reason + _mwsLog.ColumnsSeparator
                        + dataWO.SystemTime.ToString("yyyy/MM/dd HH:mm:ss") + _mwsLog.ColumnsSeparator
                        + "null" + _mwsLog.ColumnsSeparator
                        + dataWO.ModifiedTime.ToString("yyyy/MM/dd HH:mm:ss"));
            }

            SyncronizeResult syncronizedResult = new SyncronizeResult();
            try
            {
                crmConnector = new CRMConnector(_mwsLog.Verbose, _mwsLog);
                crmConnector.ConnectionName = Configuration.ConnectionNameCRM;
                OrganizationService organizationService = crmConnector.Connect();
                Guid userId = crmConnector.GetLoggedUserId(organizationService);
                syncronizedResult.SyncTime = DateTime.Now;

                _BL_serviceappointment.UpdateStatus_SubmitTECO((IOrganizationService)organizationService
                    , dataWO.WOId
                    , dataWO.MechanicId
                    , dataWO.MTARId
                    , dataWO.InspectorComments
                    , dataWO.InspectorSuggestion
                    , dataWO.CustomerComments
                    , dataWO.CustomerSatisfaction
                    , dataWO.DocumentDate
                    , dataWO.HourMeter
                    , dataWO.Status
                    , dataWO.Reason
                    , dataWO.Longitude
                    , dataWO.Latitude
                    , dataWO.SystemTime
                    , dataWO.ManualTime
                    , dataWO.ModifiedTime
                    , syncronizedResult.SyncTime);
                syncronizedResult.Success = true;
            }
            catch (Exception ex)
            {
                syncronizedResult.Success = false;
                syncronizedResult.ErrorMessage = ex.Message.ToString();
            }
            return syncronizedResult;
        }

        public SyncronizeResult UpdateWO_Documentation(DataWO_Documentation dataWO)
        {
            SyncronizeResult syncronizedResult = new SyncronizeResult();
            try
            {
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name + _mwsLog.ColumnsSeparator
                    + dataWO.WODocumentationId + _mwsLog.ColumnsSeparator
                    + dataWO.WOId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWO.Url + _mwsLog.ColumnsSeparator
                    + dataWO.Description + _mwsLog.ColumnsSeparator
                    + "start");

                crmConnector = new CRMConnector(_mwsLog.Verbose, _mwsLog);
                crmConnector.ConnectionName = Configuration.ConnectionNameCRM;
                OrganizationService organizationService = crmConnector.Connect();
                Guid userId = crmConnector.GetLoggedUserId(organizationService);
                Guid newId = Guid.Empty;
                syncronizedResult.SyncTime = DateTime.Now;

                _BL_trs_workorderdocumentation.RecordWODocumentation((IOrganizationService)organizationService
                    , dataWO.WODocumentationId
                    , dataWO.WOId
                    , dataWO.Url
                    , dataWO.Description
                    , dataWO.ModifiedTime
                    , syncronizedResult.SyncTime);

                _mwsLog.Write(MethodBase.GetCurrentMethod().Name + _mwsLog.ColumnsSeparator
                    + dataWO.WODocumentationId + _mwsLog.ColumnsSeparator
                    + dataWO.WOId.ToString() + _mwsLog.ColumnsSeparator
                    + "success");

                syncronizedResult.Success = true;
            }
            catch (Exception ex)
            {
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name + _mwsLog.ColumnsSeparator
                    + dataWO.WODocumentationId + _mwsLog.ColumnsSeparator
                    + dataWO.WOId.ToString() + _mwsLog.ColumnsSeparator
                    + "failed:" + ex.Message);

                syncronizedResult.Success = false;
                syncronizedResult.ErrorMessage = ex.Message.ToString();
            }
            return syncronizedResult;
        }

        public SyncronizeResult UpdateWO_TaskFinish(DataWO_TaskFinish dataWOTask)
        {
            _mwsLog.Write(MethodBase.GetCurrentMethod().Name + _mwsLog.ColumnsSeparator
                + dataWOTask.WOTaskId.ToString() + _mwsLog.ColumnsSeparator
                + dataWOTask.SystemTime.ToString("yyyy/MM/dd HH:mm:ss") + _mwsLog.ColumnsSeparator
                + dataWOTask.ManualTime.ToString("yyyy/MM/dd HH:mm:ss") + _mwsLog.ColumnsSeparator
                + dataWOTask.ModifiedTime.ToString("yyyy/MM/dd HH:mm:ss"));

            SyncronizeResult syncronizedResult = new SyncronizeResult();
            try
            {
                crmConnector = new CRMConnector(_mwsLog.Verbose, _mwsLog);
                crmConnector.ConnectionName = Configuration.ConnectionNameCRM;
                OrganizationService organizationService = crmConnector.Connect();
                Guid userId = crmConnector.GetLoggedUserId(organizationService);
                syncronizedResult.SyncTime = DateTime.Now;

                _BL_trs_commercialdetail.UpdateFinishTime((IOrganizationService)organizationService
                    , dataWOTask.WOTaskId
                    , dataWOTask.SystemTime
                    , dataWOTask.ManualTime
                    , dataWOTask.ModifiedTime
                    , syncronizedResult.SyncTime);
                syncronizedResult.Success = true;
            }
            catch (Exception ex)
            {
                syncronizedResult.Success = false;
                syncronizedResult.ErrorMessage = ex.Message.ToString();
            }
            return syncronizedResult;
        }

        public SyncronizeResult UpdateWO_TaskMechanicRole(DataWO_TaskMechanicRole dataWOTask)
        {
            SyncronizeResult syncronizedResult = new SyncronizeResult();
            try
            {
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name + _mwsLog.ColumnsSeparator
                    + dataWOTask.WOTaskMechanicId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOTask.MechanicRole.ToString());

                crmConnector = new CRMConnector(_mwsLog.Verbose, _mwsLog);
                crmConnector.ConnectionName = Configuration.ConnectionNameCRM;
                OrganizationService organizationService = crmConnector.Connect();
                Guid userId = crmConnector.GetLoggedUserId(organizationService);
                syncronizedResult.SyncTime = DateTime.Now;

                _BL_trs_commercialdetailmechanic.UpdateMechanicRole((IOrganizationService)organizationService
                    , dataWOTask.WOTaskMechanicId
                    , dataWOTask.MechanicRole
                    , dataWOTask.ModifiedTime
                    , syncronizedResult.SyncTime);
                syncronizedResult.Success = true;
            }
            catch (Exception ex)
            {
                syncronizedResult.Success = false;
                syncronizedResult.ErrorMessage = ex.Message.ToString();
            }
            return syncronizedResult;
        }

        public SyncronizeResult UpdateWO_PartReturned(DataWO_PartReturned dataWOPart)
        {
            SyncronizeResult syncronizedResult = new SyncronizeResult();
            try
            {
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name + _mwsLog.ColumnsSeparator
                    + dataWOPart.PartSummaryId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOPart.ReturnedQty.ToString());

                crmConnector = new CRMConnector(_mwsLog.Verbose, _mwsLog);
                crmConnector.ConnectionName = Configuration.ConnectionNameCRM;
                OrganizationService organizationService = crmConnector.Connect();
                Guid userId = crmConnector.GetLoggedUserId(organizationService);
                syncronizedResult.SyncTime = DateTime.Now;

                _BL_trs_workorderpartssummary.UpdateReturnedQuantity((IOrganizationService)organizationService
                    , dataWOPart.PartSummaryId
                    , dataWOPart.ReturnedQty
                    , dataWOPart.ModifiedTime
                    , syncronizedResult.SyncTime);
                syncronizedResult.Success = true;
            }
            catch (Exception ex)
            {
                syncronizedResult.Success = false;
                syncronizedResult.ErrorMessage = ex.Message.ToString();
            }
            return syncronizedResult;
        }

        public SyncronizeResult UpdateWO_Recommendation(DataWO_Recommendation dataWORecommendation)
        {
            SyncronizeResult syncronizedResult = new SyncronizeResult();
            try
            {
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name + _mwsLog.ColumnsSeparator
                    + dataWORecommendation.PartRecommendationId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWORecommendation.WorkOrderId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWORecommendation.SectionId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWORecommendation.TaskListHeaderId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWORecommendation.TaskListDetailId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWORecommendation.PartId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWORecommendation.PartNumberCatalog + _mwsLog.ColumnsSeparator
                    + dataWORecommendation.Quantity);

                if (dataWORecommendation.TaskListDetailId != null && dataWORecommendation.TaskListHeaderId == null)
                    throw new Exception("Please send Task List Header Id too !");

                crmConnector = new CRMConnector(_mwsLog.Verbose, _mwsLog);
                crmConnector.ConnectionName = Configuration.ConnectionNameCRM;
                OrganizationService organizationService = crmConnector.Connect();
                Guid userId = crmConnector.GetLoggedUserId(organizationService);
                Guid newId = Guid.Empty;
                syncronizedResult.SyncTime = DateTime.Now;

                _BL_trs_workorderpartrecommendation.RecordRecommendation((IOrganizationService)organizationService
                    , dataWORecommendation.PartRecommendationId
                    , dataWORecommendation.WorkOrderId
                    , dataWORecommendation.SectionId
                    , dataWORecommendation.TaskListHeaderId
                    , dataWORecommendation.TaskListDetailId
                    , dataWORecommendation.PartId
                    , dataWORecommendation.PartNumberCatalog
                    , dataWORecommendation.Quantity
                    , dataWORecommendation.ModifiedTime
                    , syncronizedResult.SyncTime);
                syncronizedResult.Success = true;
            }
            catch (Exception ex)
            {
                syncronizedResult.Success = false;
                syncronizedResult.ErrorMessage = ex.Message.ToString();
            }
            return syncronizedResult;
        }

        public SyncronizeResult UpdateWO_MTAR(DataWO_MTAR dataWOMTAR)
        {
            SyncronizeResult syncronizedResult = new SyncronizeResult();
            try
            {
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name + _mwsLog.ColumnsSeparator
                    + dataWOMTAR.WOId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOMTAR.MechanicId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOMTAR.MTARId.ToString()
                    + dataWOMTAR.Status.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOMTAR.Reason + _mwsLog.ColumnsSeparator
                    + dataWOMTAR.Longitude.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOMTAR.Latitude.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOMTAR.SystemTime.ToString("yyyy/MM/dd HH:mm:ss") + _mwsLog.ColumnsSeparator
                    + (dataWOMTAR.ManualTime == null ? "null" : ((DateTime)dataWOMTAR.ManualTime).ToString("yyyy/MM/dd HH:mm:ss")));

                crmConnector = new CRMConnector(_mwsLog.Verbose, _mwsLog);
                crmConnector.ConnectionName = Configuration.ConnectionNameCRM;
                OrganizationService organizationService = crmConnector.Connect();
                Guid userId = crmConnector.GetLoggedUserId(organizationService);
                syncronizedResult.SyncTime = DateTime.Now;

                _BL_trs_mtar.RecordMTAR((IOrganizationService)organizationService
                    , dataWOMTAR.WOId
                    , dataWOMTAR.MechanicId
                    , dataWOMTAR.MTARId
                    , dataWOMTAR.Status
                    , dataWOMTAR.Reason
                    , dataWOMTAR.Longitude
                    , dataWOMTAR.Latitude
                    , dataWOMTAR.SystemTime
                    , dataWOMTAR.ManualTime
                    , dataWOMTAR.ModifiedTime
                    , syncronizedResult.SyncTime
                    , false);
                syncronizedResult.Success = true;
            }
            catch (Exception ex)
            {
                syncronizedResult.Success = false;
                syncronizedResult.ErrorMessage = ex.Message.ToString();
            }
            return syncronizedResult;
        }

        #region PPM/Inspection Report
        public SyncronizeResult UpdateWO_PPM_Inspection_Report(DataWO_PPM_Inspection_Report dataWOPPM)
        {
            SyncronizeResult syncronizedResult = new SyncronizeResult();
            try
            {
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name + _mwsLog.ColumnsSeparator
                    + dataWOPPM.PPM_Inspection_ReportId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOPPM.WOId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOPPM.ReportType.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOPPM.ProductTypeId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOPPM.PopulationId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOPPM.TypeofWorkId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOPPM.Comments + _mwsLog.ColumnsSeparator
                    + dataWOPPM.MachineCondition.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOPPM.TypeofSoil.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOPPM.Application.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOPPM.Analysis + _mwsLog.ColumnsSeparator
                    + dataWOPPM.RepairDate.ToString("yyyy/MM/dd HH:mm:ss") + _mwsLog.ColumnsSeparator
                    + dataWOPPM.TroubleDate.ToString("yyyy/MM/dd HH:mm:ss"));

                crmConnector = new CRMConnector(_mwsLog.Verbose, _mwsLog);
                crmConnector.ConnectionName = Configuration.ConnectionNameCRM;
                OrganizationService organizationService = crmConnector.Connect();
                Guid userId = crmConnector.GetLoggedUserId(organizationService);
                Guid newId = Guid.Empty;
                syncronizedResult.SyncTime = DateTime.Now;

                _BL_trs_ppmreport.RecordPPMInsReport(organizationService
                    , dataWOPPM.PPM_Inspection_ReportId
                    , dataWOPPM.WOId
                    , dataWOPPM.ReportType
                    , dataWOPPM.ProductTypeId
                    , dataWOPPM.PopulationId
                    , dataWOPPM.TypeofWorkId
                    , dataWOPPM.Comments
                    , dataWOPPM.MachineCondition
                    , dataWOPPM.TypeofSoil
                    , dataWOPPM.Application
                    , dataWOPPM.Analysis
                    , dataWOPPM.RepairDate
                    , dataWOPPM.TroubleDate
                    , dataWOPPM.ModifiedTime
                    , syncronizedResult.SyncTime);
                syncronizedResult.Success = true;
            }
            catch (Exception ex)
            {
                syncronizedResult.Success = false;
                syncronizedResult.ErrorMessage = ex.Message.ToString();
            }
            return syncronizedResult;
        }

        public SyncronizeResult UpdateWO_PPM_Inspection_Recommendation(DataWO_PPM_Inspection_Recommendation dataWOPPM)
        {
            SyncronizeResult syncronizedResult = new SyncronizeResult();
            try
            {
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name + _mwsLog.ColumnsSeparator
                    + dataWOPPM.PPM_Inspection_RecommendationId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOPPM.PPM_Inspection_ReportId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOPPM.WOId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOPPM.SectionId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOPPM.RecommendationId.ToString());

                crmConnector = new CRMConnector(_mwsLog.Verbose, _mwsLog);
                crmConnector.ConnectionName = Configuration.ConnectionNameCRM;
                OrganizationService organizationService = crmConnector.Connect();
                Guid userId = crmConnector.GetLoggedUserId(organizationService);
                Guid newId = Guid.Empty;
                syncronizedResult.SyncTime = DateTime.Now;

                _BL_trs_sectionrecommendation.RecordPPMInsRecommendation(organizationService
                    , dataWOPPM.PPM_Inspection_RecommendationId
                    , dataWOPPM.PPM_Inspection_ReportId
                    , dataWOPPM.WOId
                    , dataWOPPM.SectionId
                    , dataWOPPM.RecommendationId
                    , dataWOPPM.ModifiedTime
                    , syncronizedResult.SyncTime);

                syncronizedResult.Success = true;
            }
            catch (Exception ex)
            {
                syncronizedResult.Success = false;
                syncronizedResult.ErrorMessage = ex.Message.ToString();
            }
            return syncronizedResult;
        }
        #endregion

        #region Technical Service Report
        public SyncronizeResult UpdateWO_TSR(DataWO_TSR dataWOTSR)
        {
            SyncronizeResult syncronizedResult = new SyncronizeResult();
            try
            {
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name + _mwsLog.ColumnsSeparator
                    + dataWOTSR.TSRId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOTSR.WOId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOTSR.PopulationId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOTSR.Symptom + _mwsLog.ColumnsSeparator
                    + dataWOTSR.OperatingCondition.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOTSR.ConditionDescription + _mwsLog.ColumnsSeparator
                    + dataWOTSR.ProductType.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOTSR.ProductApplication.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOTSR.TypeofSoil.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOTSR.RepairDate.ToString("yyyy/MM/dd HH:mm:ss") + _mwsLog.ColumnsSeparator
                    + dataWOTSR.TroubleDate.ToString("yyyy/MM/dd HH:mm:ss") + _mwsLog.ColumnsSeparator
                    + dataWOTSR.PartsCaused.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOTSR.TechnicalAnalysis + _mwsLog.ColumnsSeparator
                    + dataWOTSR.CorrectionTaken + _mwsLog.ColumnsSeparator
                    + dataWOTSR.OldHM.ToString());

                crmConnector = new CRMConnector(_mwsLog.Verbose, _mwsLog);
                crmConnector.ConnectionName = Configuration.ConnectionNameCRM;
                OrganizationService organizationService = crmConnector.Connect();
                Guid userId = crmConnector.GetLoggedUserId(organizationService);
                syncronizedResult.SyncTime = DateTime.Now;

                _BL_trs_technicalservicereport.RecordTSR(organizationService
                    , dataWOTSR.TSRId
                    , dataWOTSR.WOId
                    , dataWOTSR.PopulationId
                    , dataWOTSR.Symptom
                    , dataWOTSR.OperatingCondition
                    , dataWOTSR.ConditionDescription
                    , dataWOTSR.ProductType
                    , dataWOTSR.ProductApplication
                    , dataWOTSR.TypeofSoil
                    , dataWOTSR.RepairDate
                    , dataWOTSR.TroubleDate
                    , dataWOTSR.PartsCaused
                    , dataWOTSR.TechnicalAnalysis
                    , dataWOTSR.CorrectionTaken
                    , dataWOTSR.GensetType
                    , dataWOTSR.Sector
                    , dataWOTSR.OldHM
                    , dataWOTSR.ModifiedTime
                    , syncronizedResult.SyncTime);

                syncronizedResult.Success = true;
            }
            catch (Exception ex)
            {
                syncronizedResult.Success = false;
                syncronizedResult.ErrorMessage = ex.Message.ToString();
            }
            return syncronizedResult;
        }

        public SyncronizeResult UpdateWO_TSRDocumentation(DataWO_TSRDocumentation dataWOTSR)
        {
            SyncronizeResult syncronizedResult = new SyncronizeResult();
            try
            {
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name + _mwsLog.ColumnsSeparator
                    + dataWOTSR.TSRDocumentationId.ToString());

                crmConnector = new CRMConnector(_mwsLog.Verbose, _mwsLog);
                crmConnector.ConnectionName = Configuration.ConnectionNameCRM;
                OrganizationService organizationService = crmConnector.Connect();
                Guid userId = crmConnector.GetLoggedUserId(organizationService);
                syncronizedResult.SyncTime = DateTime.Now;

                _BL_trs_technicalservicereportdocumentation.RecordTSRDocumentation((IOrganizationService)organizationService
                    , dataWOTSR.TSRDocumentationId
                    , dataWOTSR.WOId
                    , dataWOTSR.Url
                    , dataWOTSR.Description
                    , dataWOTSR.ModifiedTime
                    , syncronizedResult.SyncTime);
                syncronizedResult.Success = true;
            }
            catch (Exception ex)
            {
                syncronizedResult.Success = false;
                syncronizedResult.ErrorMessage = ex.Message.ToString();
            }
            return syncronizedResult;
        }

        public SyncronizeResult UpdateWO_TSRPartInstalled(DataWO_TSRPartInstalled dataWOTSR)
        {
            SyncronizeResult syncronizedResult = new SyncronizeResult();
            try
            {
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name + _mwsLog.ColumnsSeparator
                    + dataWOTSR.TSRPartInstalledDetailId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOTSR.WOId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOTSR.PartId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOTSR.PartNumberCatalog + _mwsLog.ColumnsSeparator
                    + dataWOTSR.Quantity.ToString());

                crmConnector = new CRMConnector(_mwsLog.Verbose, _mwsLog);
                crmConnector.ConnectionName = Configuration.ConnectionNameCRM;
                OrganizationService organizationService = crmConnector.Connect();
                Guid userId = crmConnector.GetLoggedUserId(organizationService);
                Guid newId = Guid.Empty;
                syncronizedResult.SyncTime = DateTime.Now;

                _BL_trs_tsrpartdetails.RecordTSR_PartInstalled(organizationService
                    , dataWOTSR.TSRPartInstalledDetailId
                    , dataWOTSR.WOId
                    , dataWOTSR.PartId
                    , dataWOTSR.PartNumberCatalog
                    , dataWOTSR.Quantity
                    , dataWOTSR.ModifiedTime
                    , syncronizedResult.SyncTime);
                syncronizedResult.Success = true;
            }
            catch (Exception ex)
            {
                syncronizedResult.Success = false;
                syncronizedResult.ErrorMessage = ex.Message.ToString();
            }
            return syncronizedResult;
        }

        public SyncronizeResult UpdateWO_TSRPartDamaged(DataWO_TSRPartDamaged dataWOTSR)
        {
            SyncronizeResult syncronizedResult = new SyncronizeResult();
            try
            {
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name + _mwsLog.ColumnsSeparator
                    + dataWOTSR.TSRPartDamagedDetailId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOTSR.WOId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOTSR.PartId.ToString() + _mwsLog.ColumnsSeparator
                    + dataWOTSR.PartNumberCatalog + _mwsLog.ColumnsSeparator
                    + dataWOTSR.Quantity.ToString());

                crmConnector = new CRMConnector(_mwsLog.Verbose, _mwsLog);
                crmConnector.ConnectionName = Configuration.ConnectionNameCRM;
                OrganizationService organizationService = crmConnector.Connect();
                Guid userId = crmConnector.GetLoggedUserId(organizationService);
                Guid newId = Guid.Empty;
                syncronizedResult.SyncTime = DateTime.Now;

                _BL_trs_tsrpartsdamageddetail.RecordTSR_PartDamaged(organizationService
                    , dataWOTSR.TSRPartDamagedDetailId
                    , dataWOTSR.WOId
                    , dataWOTSR.PartId
                    , dataWOTSR.PartNumberCatalog
                    , dataWOTSR.Quantity
                    , dataWOTSR.ModifiedTime
                    , syncronizedResult.SyncTime);
                syncronizedResult.Success = true;
            }
            catch (Exception ex)
            {
                syncronizedResult.Success = false;
                syncronizedResult.ErrorMessage = ex.Message.ToString();
            }
            return syncronizedResult;
        }
        #endregion
        #endregion

        #region VOC
        public SyncronizeResult UpdateVOC(DataVOC dataVOC)
        {
            SyncronizeResult syncronizedResult = new SyncronizeResult();
            try
            {
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name + _mwsLog.ColumnsSeparator
                    + dataVOC.Contact + _mwsLog.ColumnsSeparator
                    + dataVOC.Topic + _mwsLog.ColumnsSeparator
                    + dataVOC.Customer + _mwsLog.ColumnsSeparator
                    + dataVOC.VOCType);

                crmConnector = new CRMConnector(_mwsLog.Verbose, _mwsLog);
                crmConnector.ConnectionName = Configuration.ConnectionNameCRM;
                OrganizationService organizationService = crmConnector.Connect();
                Guid userId = crmConnector.GetLoggedUserId(organizationService);
                Guid newId = Guid.Empty;
                syncronizedResult.SyncTime = DateTime.Now;

                _BL_trs_voc.RecordVOC(organizationService
                    , dataVOC.VOCId
                    , dataVOC.Contact
                    , dataVOC.Topic
                    , dataVOC.Customer
                    , dataVOC.VOCType
                    , dataVOC.Comment
                    , dataVOC.ModifiedTime
                    , syncronizedResult.SyncTime
                    , out newId);
                syncronizedResult.NewId = newId;
                syncronizedResult.Success = true;
            }
            catch (Exception ex)
            {
                syncronizedResult.Success = false;
                syncronizedResult.ErrorMessage = ex.Message.ToString();
            }
            return syncronizedResult;
        }
        #endregion
        #endregion
    }
}
