using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrakNusRapidService.Helper
{
    public class Configuration
    {
        #region Constants
        public const string ConnectionNameCRM = "TRSCRM";
        public const string ConnectionNameODBC = "TRS";
        public const string ConfigurationName = "TRS";

        public const decimal GPSMaxValue = 999.999999m;
        public const int GPSMaxDigit = 6;
        public const decimal GPSTolerance = 0.01m;
        public const int GPSToleranceDigit = 2;
        public const decimal TNGPSLongitude = 0;
        public const decimal TNGPSLatitude = 0;

        public const int IdleMinutes = 10;
        public const string IdleMinutesinWords = "10 minutes";

        public const string SDHRoleName = "SDH";

        //public const string MobileWebServiceAddress_Development = "http://203.81.250.35/crmservices/mobileservice.asmx";
        //public const string MobileWebServiceAddress_Production = "http://tntrsmbl.cloudapp.net/trsservices/mobileservice.asmx";

        #region Work Order
        public const int PMActType_Delivery = 4;
        public const int PMActType_Assy_Disassy = 3;
        public const int PMActType_PDI = 2;

        public const int WOStatus_New = 1;
        public const int WOStatus_Released = 2;
        public const int WOStatus_Hold = 3;
        public const int WOStatus_Dispatched = 4;
        public const int WOStatus_InProgress = 6;
        public const int WOStatus_Unhold = 6;
        public const int WOStatus_Arrived = 7;
        public const int WOStatus_Invoiced = 8;
        public const int WOStatus_Canceled = 9;
        public const int WOStatus_TECO = 167630002;
        public const int WOStatus_TECObyMechanic = 167630003;
        public const int WOStatus_Completed = 8;

        public const int WOState_Released = 0;
        public const int WOState_Hold = 3;
        public const int WOState_Dispatched = 3;
        public const int WOState_InProgress = 3;
        public const int WOState_Unhold = 3;
        public const int WOState_Arrived = 3;
        public const int WOState_Invoiced = 1;
        public const int WOState_Canceled = 2;
        public const int WOState_TECO = 1;
        public const int WOState_TECObyMechanic = 1;
        public const int WOState_Completed = 1;

        public const int WOTask_Completed = 167630000;
        #endregion

        #region Quotation
        public const int Quo_Draft = 1;
        public const int Quo_Approve = 167630000;
        public const int Quo_Waiting_Approval = 167630001;
        public const int Quo_Final_Approve = 167630002;
        public const int Quo_Revised = 167630003;
        #endregion 

        #region Mechanic Role
        public const int MechanicRole_Member = 167630000;
        public const int MechanicRole_Leader = 167630001;
        public const int MechanicRole_Secondman = 167630002;
        #endregion

        #region MTAR
        public const int MTAR_Dispatch = 167629999;
        public const int MTAR_PrepareatOffice = 167630000;
        public const int MTAR_ReadytoGo = 167630001;
        public const int MTAR_Arrived = 167630002;
        public const int MTAR_Standby = 167630003;
        public const int MTAR_ReadytoRepair = 167630004;
        public const int MTAR_Hold = 167630005;
        public const int MTAR_DoneforToday = 167630006;
        public const int MTAR_Resume = 167630007;
        public const int MTAR_RepairDone = 167630008;
        public const int MTAR_FinishWO = 167630009;
        public const int MTAR_ArrivedatOffice = 167630010;
        public const int MTAR_SubmitTeco = 167630011;
        #endregion

        #region Equipment
        public const int EQStatus_NotMaintained = 167630000;
        public const int EQStatus_Operation = 167630001;
        public const int EQStatus_OperationNeedMaintenance = 167630002;
        public const int EQStatus_BreakDown = 167630003;
        public const int EQStatus_Scrapped = 167630004;
        #endregion

        #region PPM/Inspection
        public const int ReportType_Inspection = 167630000;
        public const int ReportType_PPM = 167630001;
        #endregion
        #endregion
    }
}
