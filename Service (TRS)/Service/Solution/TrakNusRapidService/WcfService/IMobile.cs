using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using TrakNusRapidService.Helper;

namespace TrakNusRapidServiceWcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IMobile" in both code and config file together.
    [ServiceContract]
    public interface IMobile
    {
        [OperationContract]
        SyncronizeResult UpdatePopulationLocation(DataPopulation dataPopulation);

        [OperationContract]
        SyncronizeResult UpdateMechanicLocation(DataMechanic_Location dataMechanic);

        [OperationContract]
        SyncronizeResult SendMechanicPassword(DataMechanic dataMechanic);

        [OperationContract]
        SyncronizeResult UnconfirmedWO(DataWO_Unconfirmed dataWO);

        [OperationContract]
        SyncronizeResult CancelConfirmedWO(DataWO_CancelConfirmedbyMechanic dataWO);

        [OperationContract]
        SyncronizeResult RequestParts(DataWO_RequestParts dataWO);

        [OperationContract]
        SyncronizeResult UpdateWOStatus_Hold(DataWO_Hold dataWO);

        [OperationContract]
        SyncronizeResult UpdateWOStatus_Unhold(DataWO_Unhold dataWO);

        [OperationContract]
        SyncronizeResult UpdateWOStatus_SubmitTECO(DataWO_SubmitTECO dataWO);

        [OperationContract]
        SyncronizeResult UpdateWO_Documentation(DataWO_Documentation dataWO);

        [OperationContract]
        SyncronizeResult UpdateWO_TaskFinish(DataWO_TaskFinish dataWOTask);

        [OperationContract]
        SyncronizeResult UpdateWO_TaskMechanicRole(DataWO_TaskMechanicRole dataWOTask);

        [OperationContract]
        SyncronizeResult UpdateWO_PartReturned(DataWO_PartReturned dataWOPart);

        [OperationContract]
        SyncronizeResult UpdateWO_Recommendation(DataWO_Recommendation dataWORecommendation);

        [OperationContract]
        SyncronizeResult UpdateWO_MTAR(DataWO_MTAR dataWOMTAR);

        [OperationContract]
        SyncronizeResult UpdateWO_PPM_Inspection_Report(DataWO_PPM_Inspection_Report dataWOPPM);

        [OperationContract]
        SyncronizeResult UpdateWO_PPM_Inspection_Recommendation(DataWO_PPM_Inspection_Recommendation dataWOPPM);

        [OperationContract]
        SyncronizeResult UpdateWO_TSR(DataWO_TSR dataWOTSR);

        [OperationContract]
        SyncronizeResult UpdateWO_TSRDocumentation(DataWO_TSRDocumentation dataWO);

        [OperationContract]
        SyncronizeResult UpdateWO_TSRPartInstalled(DataWO_TSRPartInstalled dataWOTSR);

        [OperationContract]
        SyncronizeResult UpdateWO_TSRPartDamaged(DataWO_TSRPartDamaged dataWOTSR);

        [OperationContract]
        SyncronizeResult UpdateVOC(DataVOC dataVOC);
    }

    [DataContract]
    public class SyncronizeResult
    {
        private bool _success = false;
        private string _errorMessage = string.Empty;
        private DateTime _syncTime = DateTime.MinValue;
        private Guid _newId = Guid.Empty;

        [DataMember]
        public bool Success
        {
            get { return _success; }
            set { _success = value; }
        }

        [DataMember]
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        [DataMember]
        public DateTime SyncTime
        {
            get { return _syncTime; }
            set { _syncTime = value; }
        }

        [DataMember]
        public Guid NewId
        {
            get { return _newId; }
            set { _newId = value; }
        }
    }

    [DataContract]
    public class DataPopulation
    {
        Guid _woId = Guid.Empty;
        Guid _populationId = Guid.Empty;
        Guid _customerId = Guid.Empty;
        decimal _longitude = Configuration.GPSMaxValue;
        decimal _latitude = Configuration.GPSMaxValue;
        string _area = string.Empty;

        [DataMember]
        public Guid WOId
        {
            get { return _woId; }
            set { _woId = value; }
        }

        [DataMember]
        public Guid PopulationId
        {
            get { return _populationId; }
            set { _populationId = value; }
        }

        [DataMember]
        public Guid CustomerId
        {
            get { return _customerId; }
            set { _customerId = value; }
        }

        [DataMember]
        public decimal Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }

        [DataMember]
        public decimal Latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
        }

        [DataMember]
        public string Area
        {
            get { return _area; }
            set { _area = value; }
        }
    }

    [DataContract]
    public class DataMechanic_Location
    {
        Guid _equipmentId = Guid.Empty;
        decimal _longitude = Configuration.GPSMaxValue;
        decimal _latitude = Configuration.GPSMaxValue;

        [DataMember]
        public Guid MechanicId
        {
            get { return _equipmentId; }
            set { _equipmentId = value; }
        }

        [DataMember]
        public decimal Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }

        [DataMember]
        public decimal Latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
        }
    }

    [DataContract]
    public class DataMechanic
    {
        Guid _equipmentId = Guid.Empty;
        string _password = string.Empty;

        [DataMember]
        public Guid MechanicId
        {
            get { return _equipmentId; }
            set { _equipmentId = value; }
        }

        [DataMember]
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
    }

    [DataContract]
    public class DataPart
    {
        Guid _partId = Guid.Empty;
        string _partNumber = string.Empty;
        string _partDescription = string.Empty;
        int _quantity = 0;

        [DataMember]
        public Guid PartId
        {
            get { return _partId; }
            set { _partId = value; }
        }

        [DataMember]
        public string PartNumber
        {
            get { return _partNumber; }
            set { _partNumber = value; }
        }

        [DataMember]
        public string PartDescription
        {
            get { return _partDescription; }
            set { _partDescription = value; }
        }

        [DataMember]
        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }
    }

    [DataContract]
    public class DataWO
    {
        Guid _trs_workorder = Guid.Empty;
        string _trs_workordernumber = string.Empty;
        DateTime _trs_documentdate = DateTime.MinValue;
        Uri _trs_documentlink = null;
        string _trs_cancelreason = string.Empty;
        DateTime _modifiedon = DateTime.MinValue;

        [DataMember]
        public Guid WOId
        {
            get { return _trs_workorder; }
            set { _trs_workorder = value; }
        }

        public string WONumber
        {
            get { return _trs_workordernumber; }
            set { _trs_workordernumber = value; }
        }

        [DataMember]
        public DateTime DocumentDate
        {
            get { return _trs_documentdate; }
            set { _trs_documentdate = value; }
        }

        [DataMember]
        public Uri DocumentUri
        {
            get { return _trs_documentlink; }
            set { _trs_documentlink = value; }
        }

        [DataMember]
        public string CancelReason
        {
            get { return _trs_cancelreason; }
            set { _trs_cancelreason = value; }
        }

        [DataMember]
        public DateTime ModifiedTime
        {
            get { return _modifiedon; }
            set { _modifiedon = value; }
        }
    }

    [DataContract]
    public class DataWO_Unconfirmed
    {
        Guid _trs_workorder = Guid.Empty;
        string _trs_workordernumber = string.Empty;
        DateTime _modifiedon = DateTime.MinValue;

        [DataMember]
        public Guid WOId
        {
            get { return _trs_workorder; }
            set { _trs_workorder = value; }
        }

        public string WONumber
        {
            get { return _trs_workordernumber; }
            set { _trs_workordernumber = value; }
        }

        [DataMember]
        public DateTime ModifiedTime
        {
            get { return _modifiedon; }
            set { _modifiedon = value; }
        }
    }

    [DataContract]
    public class DataWO_CancelConfirmedbyMechanic
    {
        Guid _trs_workorder = Guid.Empty;
        string _trs_workordernumber = string.Empty;
        string _trs_cancelreason = string.Empty;
        DateTime _modifiedon = DateTime.MinValue;

        [DataMember]
        public Guid WOId
        {
            get { return _trs_workorder; }
            set { _trs_workorder = value; }
        }

        public string WONumber
        {
            get { return _trs_workordernumber; }
            set { _trs_workordernumber = value; }
        }

        [DataMember]
        public string CancelReason
        {
            get { return _trs_cancelreason; }
            set { _trs_cancelreason = value; }
        }

        [DataMember]
        public DateTime ModifiedTime
        {
            get { return _modifiedon; }
            set { _modifiedon = value; }
        }
    }

    [DataContract]
    public class DataWO_Hold
    {
        Guid _trs_workorder = Guid.Empty;
        decimal _longitude = Configuration.GPSMaxValue;
        decimal _latitude = Configuration.GPSMaxValue;
        Guid _mechanicId = Guid.Empty;
        Guid _mtarId = Guid.Empty;
        string _reason = string.Empty;
        DateTime _systemtime = DateTime.MinValue;
        DateTime? _manualtime = null;
        DateTime _modifiedon = DateTime.MinValue;

        [DataMember]
        public Guid WOId
        {
            get { return _trs_workorder; }
            set { _trs_workorder = value; }
        }

        [DataMember]
        public decimal Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }

        [DataMember]
        public decimal Latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
        }

        [DataMember]
        public Guid MechanicId
        {
            get { return _mechanicId; }
            set { _mechanicId = value; }
        }

        [DataMember]
        public Guid MTARId
        {
            get { return _mtarId; }
            set { _mtarId = value; }
        }

        [DataMember]
        public string Reason
        {
            get { return _reason; }
            set { _reason = value; }
        }

        [DataMember]
        public DateTime SystemTime
        {
            get { return _systemtime; }
            set { _systemtime = value; }
        }

        [DataMember]
        public DateTime? ManualTime
        {
            get { return _manualtime; }
            set { _manualtime = value; }
        }

        [DataMember]
        public DateTime ModifiedTime
        {
            get { return _modifiedon; }
            set { _modifiedon = value; }
        }
    }

    [DataContract]
    public class DataWO_Unhold
    {
        Guid _trs_workorder = Guid.Empty;
        decimal _longitude = Configuration.GPSMaxValue;
        decimal _latitude = Configuration.GPSMaxValue;
        Guid _mechanicId = Guid.Empty;
        Guid _mtarId = Guid.Empty;
        string _reason = string.Empty;
        DateTime _systemtime = DateTime.MinValue;
        DateTime? _manualtime = null;
        DateTime _modifiedon = DateTime.MinValue;

        [DataMember]
        public Guid WOId
        {
            get { return _trs_workorder; }
            set { _trs_workorder = value; }
        }

        [DataMember]
        public decimal Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }

        [DataMember]
        public decimal Latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
        }

        [DataMember]
        public Guid MechanicId
        {
            get { return _mechanicId; }
            set { _mechanicId = value; }
        }

        [DataMember]
        public Guid MTARId
        {
            get { return _mtarId; }
            set { _mtarId = value; }
        }

        [DataMember]
        public string Reason
        {
            get { return _reason; }
            set { _reason = value; }
        }

        [DataMember]
        public DateTime SystemTime
        {
            get { return _systemtime; }
            set { _systemtime = value; }
        }

        [DataMember]
        public DateTime? ManualTime
        {
            get { return _manualtime; }
            set { _manualtime = value; }
        }

        [DataMember]
        public DateTime ModifiedTime
        {
            get { return _modifiedon; }
            set { _modifiedon = value; }
        }
    }

    [DataContract]
    public class DataWO_SubmitTECO
    {
        Guid _trs_workorder = Guid.Empty;
        string _trs_inspectorcomments = string.Empty;
        Guid? _trs_inspectorsuggestion = null;
        string _trs_customercomments = string.Empty;
        string _trs_customersatisfaction = string.Empty;
        DateTime _trs_documentdate = DateTime.MinValue;
        //Uri _trs_documentlink = null;
        DateTime _modifiedon = DateTime.MinValue;
        decimal? _trs_lasthourmeter = null;
        int? _trs_statusinoperation = null;
        decimal _longitude = Configuration.GPSMaxValue;
        decimal _latitude = Configuration.GPSMaxValue;
        Guid _mechanicId = Guid.Empty;
        Guid _mtarId = Guid.Empty;
        string _reason = string.Empty;
        DateTime _systemtime = DateTime.MinValue;
        DateTime? _manualtime = null;

        [DataMember]
        public Guid WOId
        {
            get { return _trs_workorder; }
            set { _trs_workorder = value; }
        }

        [DataMember]
        public string InspectorComments
        {
            get { return _trs_inspectorcomments; }
            set { _trs_inspectorcomments = value; }
        }

        [DataMember]
        public Guid? InspectorSuggestion
        {
            get { return _trs_inspectorsuggestion; }
            set { _trs_inspectorsuggestion = value; }
        }

        [DataMember]
        public string CustomerComments
        {
            get { return _trs_customercomments; }
            set { _trs_customercomments = value; }
        }

        [DataMember]
        public string CustomerSatisfaction
        {
            get { return _trs_customersatisfaction; }
            set { _trs_customersatisfaction = value; }
        }

        [DataMember]
        public DateTime DocumentDate
        {
            get { return _trs_documentdate; }
            set { _trs_documentdate = value; }
        }

        //[DataMember]
        //public Uri DocumentUri
        //{
        //    get { return _trs_documentlink; }
        //    set { _trs_documentlink = value; }
        //}

        [DataMember]
        public DateTime ModifiedTime
        {
            get { return _modifiedon; }
            set { _modifiedon = value; }
        }

        [DataMember]
        public decimal? HourMeter
        {
            get { return _trs_lasthourmeter; }
            set { _trs_lasthourmeter = value; }
        }

        [DataMember]
        public int? Status
        {
            get { return _trs_statusinoperation; }
            set { _trs_statusinoperation = value; }
        }

        [DataMember]
        public decimal Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }

        [DataMember]
        public decimal Latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
        }

        [DataMember]
        public Guid MechanicId
        {
            get { return _mechanicId; }
            set { _mechanicId = value; }
        }

        [DataMember]
        public Guid MTARId
        {
            get { return _mtarId; }
            set { _mtarId = value; }
        }

        [DataMember]
        public string Reason
        {
            get { return _reason; }
            set { _reason = value; }
        }

        [DataMember]
        public DateTime SystemTime
        {
            get { return _systemtime; }
            set { _systemtime = value; }
        }

        [DataMember]
        public DateTime? ManualTime
        {
            get { return _manualtime; }
            set { _manualtime = value; }
        }
    }

    [DataContract]
    public class DataWO_Documentation
    {
        Guid _trs_workorderdocumentationid = Guid.Empty;
        Guid _trs_workorder = Guid.Empty;
        Uri _trs_url = null;
        string _trs_description = string.Empty;
        DateTime _modifiedon = DateTime.MinValue;

        [DataMember]
        public Guid WODocumentationId
        {
            get { return _trs_workorderdocumentationid; }
            set { _trs_workorderdocumentationid = value; }
        }

        [DataMember]
        public Guid WOId
        {
            get { return _trs_workorder; }
            set { _trs_workorder = value; }
        }

        [DataMember]
        public Uri Url
        {
            get { return _trs_url; }
            set { _trs_url = value; }
        }

        [DataMember]
        public string Description
        {
            get { return _trs_description; }
            set { _trs_description = value; }
        }

        [DataMember]
        public DateTime ModifiedTime
        {
            get { return _modifiedon; }
            set { _modifiedon = value; }
        }
    }

    [DataContract]
    public class DataWO_TaskFinish
    {
        Guid _woTaskId = Guid.Empty;
        DateTime _systemtime = DateTime.MinValue;
        DateTime _manualtime = DateTime.MinValue;
        DateTime _modifiedon = DateTime.MinValue;

        [DataMember]
        public Guid WOTaskId
        {
            get { return _woTaskId; }
            set { _woTaskId = value; }
        }

        [DataMember]
        public DateTime SystemTime
        {
            get { return _systemtime; }
            set { _systemtime = value; }
        }

        [DataMember]
        public DateTime ManualTime
        {
            get { return _manualtime; }
            set { _manualtime = value; }
        }

        [DataMember]
        public DateTime ModifiedTime
        {
            get { return _modifiedon; }
            set { _modifiedon = value; }
        }
    }

    [DataContract]
    public class DataWO_TaskMechanicRole
    {
        Guid _woTaskMechanicId = Guid.Empty;
        int _mechanicRole = int.MinValue;
        DateTime _modifiedon = DateTime.MinValue;

        [DataMember]
        public Guid WOTaskMechanicId
        {
            get { return _woTaskMechanicId; }
            set { _woTaskMechanicId = value; }
        }

        [DataMember]
        public int MechanicRole
        {
            get { return _mechanicRole; }
            set { _mechanicRole = value; }
        }

        [DataMember]
        public DateTime ModifiedTime
        {
            get { return _modifiedon; }
            set { _modifiedon = value; }
        }
    }

    [DataContract]
    public class DataWO_PartReturned
    {
        Guid _partSummaryId = Guid.Empty;
        int _returnedQty = 0;
        DateTime _modifiedtime = DateTime.MinValue;

        [DataMember]
        public Guid PartSummaryId
        {
            get { return _partSummaryId; }
            set { _partSummaryId = value; }
        }

        [DataMember]
        public int ReturnedQty
        {
            get { return _returnedQty; }
            set { _returnedQty = value; }
        }

        [DataMember]
        public DateTime ModifiedTime
        {
            get { return _modifiedtime; }
            set { _modifiedtime = value; }
        }
    }

    [DataContract]
    public class DataWO_Recommendation
    {
        Guid _trs_workorderpartrecommendationid = Guid.Empty;
        Guid _trs_workorder = Guid.Empty;
        Guid _trs_section = Guid.Empty;
        Guid? _trs_tasklistheaderid = null;
        Guid? _trs_tasklistdetailid = null;
        Guid? _trs_partnumber = null;
        int _trs_quantity = 0;
        string _trs_partnumbercatalog = string.Empty;
        DateTime _modifiedtime = DateTime.MinValue;

        [DataMember]
        public Guid PartRecommendationId
        {
            get { return _trs_workorderpartrecommendationid; }
            set { _trs_workorderpartrecommendationid = value; }
        }

        [DataMember]
        public Guid WorkOrderId
        {
            get { return _trs_workorder; }
            set { _trs_workorder = value; }
        }

        [DataMember]
        public Guid SectionId
        {
            get { return _trs_section; }
            set { _trs_section = value; }
        }

        [DataMember]
        public Guid? TaskListHeaderId
        {
            get { return _trs_tasklistheaderid; }
            set { _trs_tasklistheaderid = value; }
        }

        [DataMember]
        public Guid? TaskListDetailId
        {
            get { return _trs_tasklistdetailid; }
            set { _trs_tasklistdetailid = value; }
        }

        [DataMember]
        public Guid? PartId
        {
            get { return _trs_partnumber; }
            set { _trs_partnumber = value; }
        }

        [DataMember]
        public int Quantity
        {
            get { return _trs_quantity; }
            set { _trs_quantity = value; }
        }

        [DataMember]
        public DateTime ModifiedTime
        {
            get { return _modifiedtime; }
            set { _modifiedtime = value; }
        }

        [DataMember]
        public string PartNumberCatalog
        {
            get { return _trs_partnumbercatalog; }
            set { _trs_partnumbercatalog = value; }
        }
}

    [DataContract]
    public class DataWO_MTAR
    {
        Guid _woId = Guid.Empty;
        Guid _mechanicId = Guid.Empty;
        Guid _mtarId = Guid.Empty;
        int _status = 0;
        string _reason = string.Empty;
        decimal _longitude = Configuration.GPSMaxValue;
        decimal _latitude = Configuration.GPSMaxValue;
        DateTime _systemtime = DateTime.MinValue;
        DateTime? _manualtime = null;
        DateTime _modifiedtime = DateTime.MinValue;

        [DataMember]
        public Guid WOId
        {
            get { return _woId; }
            set { _woId = value; }
        }

        [DataMember]
        public Guid MechanicId
        {
            get { return _mechanicId; }
            set { _mechanicId = value; }
        }

        [DataMember]
        public Guid MTARId
        {
            get { return _mtarId; }
            set { _mtarId = value; }
        }

        [DataMember]
        public int Status
        {
            get { return _status; }
            set { _status = value; }
        }

        [DataMember]
        public string Reason
        {
            get { return _reason; }
            set { _reason = value; }
        }

        [DataMember]
        public decimal Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }

        [DataMember]
        public decimal Latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
        }

        [DataMember]
        public DateTime SystemTime
        {
            get { return _systemtime; }
            set { _systemtime = value; }
        }

        [DataMember]
        public DateTime? ManualTime
        {
            get { return _manualtime; }
            set { _manualtime = value; }
        }

        [DataMember]
        public DateTime ModifiedTime
        {
            get { return _modifiedtime; }
            set { _modifiedtime = value; }
        }
    }

    [DataContract]
    public class DataWO_PPM_Inspection_Report
    {
        Guid _trs_ppmreportid = Guid.Empty;
        Guid _trs_workorder = Guid.Empty;
        Guid _trs_producttype = Guid.Empty;
        Guid _trs_equipment = Guid.Empty;
        Guid _trs_typeofwork = Guid.Empty;
        int _trs_comments = int.MinValue;
        int _trs_machinecondition = int.MinValue;
        int _trs_typeofsoil = int.MinValue;
        int _trs_application = int.MinValue;
        string _trs_analysis = string.Empty;
        DateTime _trs_repairdate = DateTime.MinValue;
        DateTime _trs_troubledate = DateTime.MinValue;
        DateTime _modifiedon = DateTime.MinValue;
        int _trs_type = int.MinValue;

        [DataMember]
        public Guid PPM_Inspection_ReportId
        {
            get { return _trs_ppmreportid; }
            set { _trs_ppmreportid = value; }
        }

        [DataMember]
        public Guid WOId
        {
            get { return _trs_workorder; }
            set { _trs_workorder = value; }
        }

        [DataMember]
        public Guid ProductTypeId
        {
            get { return _trs_producttype; }
            set { _trs_producttype = value; }
        }

        [DataMember]
        public Guid PopulationId
        {
            get { return _trs_equipment; }
            set { _trs_equipment = value; }
        }

        [DataMember]
        public Guid TypeofWorkId
        {
            get { return _trs_typeofwork; }
            set { _trs_typeofwork = value; }
        }

        [DataMember]
        public int Comments
        {
            get { return _trs_comments; }
            set { _trs_comments = value; }
        }

        [DataMember]
        public int MachineCondition
        {
            get { return _trs_machinecondition; }
            set { _trs_machinecondition = value; }
        }

        [DataMember]
        public int TypeofSoil
        {
            get { return _trs_typeofsoil; }
            set { _trs_typeofsoil = value; }
        }

        [DataMember]
        public int Application
        {
            get { return _trs_application; }
            set { _trs_application = value; }
        }

        [DataMember]
        public string Analysis
        {
            get { return _trs_analysis; }
            set { _trs_analysis = value; }
        }

        [DataMember]
        public DateTime RepairDate
        {
            get { return _trs_repairdate; }
            set { _trs_repairdate = value; }
        }

        [DataMember]
        public DateTime TroubleDate
        {
            get { return _trs_troubledate; }
            set { _trs_troubledate = value; }
        }

        [DataMember]
        public DateTime ModifiedTime
        {
            get { return _modifiedon; }
            set { _modifiedon = value; }
        }

        [DataMember]
        public int ReportType
        {
            get { return _trs_type; }
            set { _trs_type = value; }
        }
    }

    [DataContract]
    public class DataWO_PPM_Inspection_Recommendation
    {
        Guid _trs_ppmrecommendationod = Guid.Empty;
        Guid _trs_ppmreportid = Guid.Empty;
        Guid _trs_workorder = Guid.Empty;
        Guid _trs_section = Guid.Empty;
        Guid _trs_recommendation = Guid.Empty;
        DateTime _modifiedon = DateTime.MinValue;

        [DataMember]
        public Guid PPM_Inspection_RecommendationId
        {
            get { return _trs_ppmreportid; }
            set { _trs_ppmreportid = value; }
        }

        [DataMember]
        public Guid PPM_Inspection_ReportId
        {
            get { return _trs_ppmreportid; }
            set { _trs_ppmreportid = value; }
        }

        [DataMember]
        public Guid WOId
        {
            get { return _trs_workorder; }
            set { _trs_workorder = value; }
        }

        [DataMember]
        public Guid SectionId
        {
            get { return _trs_section; }
            set { _trs_section = value; }
        }

        [DataMember]
        public Guid RecommendationId
        {
            get { return _trs_recommendation; }
            set { _trs_recommendation = value; }
        }

        [DataMember]
        public DateTime ModifiedTime
        {
            get { return _modifiedon; }
            set { _modifiedon = value; }
        }
    }

    [DataContract]
    public class DataWO_TSR
    {
        Guid _trs_technicalservicereportid = Guid.Empty;
        Guid _trs_workorder = Guid.Empty;
        Guid _trs_populationid = Guid.Empty;
        int? _trs_application = null;
        string _trs_conditiondescription = string.Empty;
        string _trs_correctiontaken = string.Empty;
        bool _trs_operatingcondition = false;
        Guid? _trs_partscaused = null;
        Guid? _trs_producttypeid = null;
        DateTime _trs_repairdate = DateTime.MinValue;
        string _trs_symptom = string.Empty;
        string _trs_technicalanalysis = string.Empty;
        DateTime _trs_troubledate = DateTime.MinValue;
        int? _trs_typeofsoil = null;
        DateTime _modifiedon = DateTime.MinValue;
        int? _trs_gensettype = null;
        int? _trs_sector = null;
        int _trs_oldhm = 0;
        
        [DataMember]
        public Guid TSRId
        {
            get { return _trs_technicalservicereportid; }
            set { _trs_technicalservicereportid = value; }
        }

        [DataMember]
        public Guid WOId
        {
            get { return _trs_workorder; }
            set { _trs_workorder = value; }
        }

        [DataMember]
        public int? ProductApplication
        {
            get { return _trs_application; }
            set { _trs_application = value; }
        }

        [DataMember]
        public string ConditionDescription
        {
            get { return _trs_conditiondescription; }
            set { _trs_conditiondescription = value; }
        }

        [DataMember]
        public string CorrectionTaken
        {
            get { return _trs_correctiontaken; }
            set { _trs_correctiontaken = value; }
        }

        [DataMember]
        public Guid PopulationId
        {
            get { return _trs_populationid; }
            set { _trs_populationid = value; }
        }

        [DataMember]
        public bool OperatingCondition
        {
            get { return _trs_operatingcondition; }
            set { _trs_operatingcondition = value; }
        }

        [DataMember]
        public Guid? PartsCaused
        {
            get { return _trs_partscaused; }
            set { _trs_partscaused = value; }
        }

        [DataMember]
        public Guid? ProductType
        {
            get { return _trs_producttypeid; }
            set { _trs_producttypeid = value; }
        }

        [DataMember]
        public DateTime RepairDate
        {
            get { return _trs_repairdate; }
            set { _trs_repairdate = value; }
        }

        [DataMember]
        public string Symptom
        {
            get { return _trs_symptom; }
            set { _trs_symptom = value; }
        }

        [DataMember]
        public string TechnicalAnalysis
        {
            get { return _trs_technicalanalysis; }
            set { _trs_technicalanalysis = value; }
        }

        [DataMember]
        public DateTime TroubleDate
        {
            get { return _trs_troubledate; }
            set { _trs_troubledate = value; }
        }

        [DataMember]
        public int? TypeofSoil
        {
            get { return _trs_typeofsoil; }
            set { _trs_typeofsoil = value; }
        }

        [DataMember]
        public DateTime ModifiedTime
        {
            get { return _modifiedon; }
            set { _modifiedon = value; }
        }

        [DataMember]
        public int? GensetType
        {
            get { return _trs_gensettype; }
            set { _trs_gensettype = value; }
        }

        [DataMember]
        public int? Sector
        {
            get { return _trs_gensettype; }
            set { _trs_gensettype = value; }
        }

        [DataMember]
        public int OldHM
        {
            get { return _trs_oldhm; }
            set { _trs_oldhm = value; }
        }
    }

    [DataContract]
    public class DataWO_TSRDocumentation
    {
        Guid _trs_technicalservicereportdocumentation = Guid.Empty;
        Guid _trs_workorder = Guid.Empty;
        Uri _trs_url = null;
        string _trs_description = string.Empty;
        DateTime _modifiedon = DateTime.MinValue;

        [DataMember]
        public Guid TSRDocumentationId
        {
            get { return _trs_technicalservicereportdocumentation; }
            set { _trs_technicalservicereportdocumentation = value; }
        }

        [DataMember]
        public Guid WOId
        {
            get { return _trs_workorder; }
            set { _trs_workorder = value; }
        }

        [DataMember]
        public Uri Url
        {
            get { return _trs_url; }
            set { _trs_url = value; }
        }

        [DataMember]
        public string Description
        {
            get { return _trs_description; }
            set { _trs_description = value; }
        }

        [DataMember]
        public DateTime ModifiedTime
        {
            get { return _modifiedon; }
            set { _modifiedon = value; }
        }
    }

    [DataContract]
    public class DataWO_TSRPartInstalled
    {
        Guid _trs_tsrpartinstalleddetailid = Guid.Empty;
        Guid _trs_workorder = Guid.Empty;
        Guid _trs_partnumber = Guid.Empty;
        int _trs_quantity = 0;
        string _trs_partnumbercatalog = string.Empty;
        DateTime _modifiedon = DateTime.MinValue;

        [DataMember]
        public Guid TSRPartInstalledDetailId
        {
            get { return _trs_tsrpartinstalleddetailid; }
            set { _trs_tsrpartinstalleddetailid = value; }
        }

        [DataMember]
        public Guid WOId
        {
            get { return _trs_workorder; }
            set { _trs_workorder = value; }
        }

        [DataMember]
        public Guid PartId
        {
            get { return _trs_partnumber; }
            set { _trs_partnumber = value; }
        }

        [DataMember]
        public int Quantity
        {
            get { return _trs_quantity; }
            set { _trs_quantity = value; }
        }

        [DataMember]
        public DateTime ModifiedTime
        {
            get { return _modifiedon; }
            set { _modifiedon = value; }
        }

        [DataMember]
        public string PartNumberCatalog
        {
            get { return _trs_partnumbercatalog; }
            set { _trs_partnumbercatalog = value; }
        }
    }

    [DataContract]
    public class DataWO_TSRPartDamaged
    {
        Guid _trs_tsrpartdamageddetailid = Guid.Empty;
        Guid _trs_workorder = Guid.Empty;
        Guid _trs_partnumber = Guid.Empty;
        int _trs_quantity = 0;
        string _trs_partnumbercatalog = string.Empty;
        DateTime _modifiedon = DateTime.MinValue;

        [DataMember]
        public Guid TSRPartDamagedDetailId
        {
            get { return _trs_tsrpartdamageddetailid; }
            set { _trs_tsrpartdamageddetailid = value; }
        }

        [DataMember]
        public Guid WOId
        {
            get { return _trs_workorder; }
            set { _trs_workorder = value; }
        }

        [DataMember]
        public Guid PartId
        {
            get { return _trs_partnumber; }
            set { _trs_partnumber = value; }
        }

        [DataMember]
        public int Quantity
        {
            get { return _trs_quantity; }
            set { _trs_quantity = value; }
        }

        [DataMember]
        public DateTime ModifiedTime
        {
            get { return _modifiedon; }
            set { _modifiedon = value; }
        }

        [DataMember]
        public string PartNumberCatalog
        {
            get { return _trs_partnumbercatalog; }
            set { _trs_partnumbercatalog = value; }
        }
    }

    [DataContract]
    public class DataWO_RequestParts
    {
        Guid _trs_workorder = Guid.Empty;
        string _trs_workordernumber = string.Empty;
        string _pmacttype = string.Empty;
        string _wodescription = string.Empty;
        string _mechanicleadernrp = string.Empty;
        string _mechanicleadername = string.Empty;
        DateTime _requesttime = DateTime.MinValue;
        List<DataPart> _listparts = new List<DataPart>();

        [DataMember]
        public Guid WOId
        {
            get { return _trs_workorder; }
            set { _trs_workorder = value; }
        }

        [DataMember]
        public string WONumber
        {
            get { return _trs_workordernumber; }
            set { _trs_workordernumber = value; }
        }

        [DataMember]
        public string PMActType
        {
            get { return _pmacttype; }
            set { _pmacttype = value; }
        }

        [DataMember]
        public string WODescription
        {
            get { return _wodescription; }
            set { _wodescription = value; }
        }

        [DataMember]
        public string MechanicLeaderNRP
        {
            get { return _mechanicleadernrp; }
            set { _mechanicleadernrp = value; }
        }

        [DataMember]
        public string MechanicLeaderName
        {
            get { return _mechanicleadername; }
            set { _mechanicleadername = value; }
        }

        [DataMember]
        public DateTime RequestTime
        {
            get { return _requesttime; }
            set { _requesttime = value; }
        }

        [DataMember]
        public List<DataPart> ListParts
        {
            get { return _listparts; }
            set { _listparts = value; }
        }
    }

    [DataContract]
    public class DataVOC
    {
        Guid _trs_vocid = Guid.Empty;
        string _trs_contact = string.Empty;
        string _trs_topic = string.Empty;
        string _trs_customer = string.Empty;
        string _trs_voctype = string.Empty;
        string _trs_comment = string.Empty;
        DateTime _modifiedon = DateTime.MinValue;

        [DataMember]
        public Guid VOCId
        {
            get { return _trs_vocid; }
            set { _trs_vocid = value; }
        }

        [DataMember]
        public string Contact
        {
            get { return _trs_contact; }
            set { _trs_contact = value; }
        }

        [DataMember]
        public string Topic
        {
            get { return _trs_topic; }
            set { _trs_topic = value; }
        }

        [DataMember]
        public string Customer
        {
            get { return _trs_customer; }
            set { _trs_customer = value; }
        }

        [DataMember]
        public string VOCType
        {
            get { return _trs_voctype; }
            set { _trs_voctype = value; }
        }

        [DataMember]
        public string Comment
        {
            get { return _trs_comment; }
            set { _trs_comment = value; }
        }

        [DataMember]
        public DateTime ModifiedTime
        {
            get { return _modifiedon; }
            set { _modifiedon = value; }
        }
    }
}
