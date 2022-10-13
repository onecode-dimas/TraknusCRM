using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IServicePRD
    {
        [OperationContract]
        string EncryptText(string value);

        #region CPO ( Sales Order )
        [OperationContract]
        CRM_WS_Response Allocation(string Token, ParamAllocationData[] Param);

        [OperationContract]
        CRM_WS_Response DeliveryOrder(string Token, ParamDeliveryOrderData[] Param);

        [OperationContract]
        CRM_WS_Response ServiceRequisition(string Token, ParamServiceRequisitionData[] Param);

        [OperationContract]
        CRM_WS_Response BAST(string Token, ParamBASTData[] Param);

        [OperationContract]
        CRM_WS_Response Invoice(string Token, ParamInvoiceData[] Param);

        [OperationContract]
        CRM_WS_Response Payment(string Token, ParamPaymentData[] Param);

        [OperationContract]
        CRM_WS_Response Retur(string Token, ParamReturData[] Param);

        [OperationContract]
        CRM_WS_Response SPB(string Token, ParamSPBData[] Param);
        #endregion ---

        #region Master
        [OperationContract]
        CRM_WS_Response Product(string Token, ParamProductData[] Param);

        [OperationContract]
        CRM_WS_Response Population(string Token, ParamPopulationData[] Param);

        [OperationContract]
        CRM_WS_Response SalesBOM(string Token, ParamSalesBOMData[] Param);
        #endregion ---

        #region Transaction
        [OperationContract]
        CRM_WS_Response Incentive(string Token, ParamIncentiveData[] Param);

        [OperationContract]
        CRM_WS_Response IncentivePayment(string Token, ParamIncentivePaymentData[] Param);

        [OperationContract]
        CRM_WS_Response UpdateCPOIDSAP(string Token, ParamUpdateCPOData[] Param);
        #endregion ---
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CRM_WS_Response
    {
        private string _Result = string.Empty;
        private string _errorDescription = string.Empty;
        private DateTime _syncDate = DateTime.MinValue;

        [DataMember]
        public string Result
        {
            get { return _Result; }
            set { _Result = value; }
        }

        [DataMember]
        public string ErrorDescription
        {
            get { return _errorDescription; }
            set { _errorDescription = value; }
        }

        [DataMember]
        public DateTime SyncDate
        {
            get { return _syncDate; }
            set { _syncDate = value; }
        }
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

    #region CPO ( Sales Order )

    [DataContract]
    public class ParamAllocationData
    {
        private string _CPOIdSAP = null;
        private string _ItemNumber = null;
        private DateTime _AllocationDate = DateTime.MinValue;
        private string _SerialNumber = null;

        [DataMember]
        public string CPOIdSAP
        {
            get { return _CPOIdSAP; }
            set { _CPOIdSAP = value; }
        }

        [DataMember]
        public string ItemNumber
        {
            get { return _ItemNumber; }
            set { _ItemNumber = value; }
        }

        [DataMember]
        public DateTime AllocationDate
        {
            get { return _AllocationDate; }
            set { _AllocationDate = value; }
        }

        [DataMember]
        public string SerialNumber
        {
            get { return _SerialNumber; }
            set { _SerialNumber = value; }
        }
    }

    [DataContract]
    public class ParamDeliveryOrderData
    {
        private string _CPOIdSAP = null;
        private string _ItemNumber = null;
        private DateTime _DODate = DateTime.MinValue;
        private string _DONo = null;

        [DataMember]
        public string CPOIdSAP
        {
            get { return _CPOIdSAP; }
            set { _CPOIdSAP = value; }
        }

        [DataMember]
        public string ItemNumber
        {
            get { return _ItemNumber; }
            set { _ItemNumber = value; }
        }

        [DataMember]
        public DateTime DODate
        {
            get { return _DODate; }
            set { _DODate = value; }
        }

        [DataMember]
        public string DONo
        {
            get { return _DONo; }
            set { _DONo = value; }
        }
    }

    [DataContract]
    public class ParamServiceRequisitionData
    {
        private string _CPOIdSAP = null;
        private string _ItemNumber = null;
        private DateTime _SRDate = DateTime.MinValue;
        private string _SRNo = null;

        [DataMember]
        public string CPOIdSAP
        {
            get { return _CPOIdSAP; }
            set { _CPOIdSAP = value; }
        }

        [DataMember]
        public string ItemNumber
        {
            get { return _ItemNumber; }
            set { _ItemNumber = value; }
        }

        [DataMember]
        public DateTime SRDate
        {
            get { return _SRDate; }
            set { _SRDate = value; }
        }

        [DataMember]
        public string SRNo
        {
            get { return _SRNo; }
            set { _SRNo = value; }
        }
    }

    [DataContract]
    public class ParamBASTData
    {
        private string _CPOIdSAP = null;
        private string _ItemNumber = null;
        private DateTime _BASTDate = DateTime.MinValue;
        private string _BASTNo = null;

        [DataMember]
        public string CPOIdSAP
        {
            get { return _CPOIdSAP; }
            set { _CPOIdSAP = value; }
        }

        [DataMember]
        public string ItemNumber
        {
            get { return _ItemNumber; }
            set { _ItemNumber = value; }
        }

        [DataMember]
        public DateTime BASTDate
        {
            get { return _BASTDate; }
            set { _BASTDate = value; }
        }

        [DataMember]
        public string BASTNo
        {
            get { return _BASTNo; }
            set { _BASTNo = value; }
        }
    }

    [DataContract]
    public class ParamInvoiceData
    {
        private string _CPOIdSAP = null;
        private string _ItemNumber = null;
        private DateTime _InvoiceDate = DateTime.MinValue;
        private string _InvoiceNo = null;

        [DataMember]
        public string CPOIdSAP
        {
            get { return _CPOIdSAP; }
            set { _CPOIdSAP = value; }
        }

        [DataMember]
        public string ItemNumber
        {
            get { return _ItemNumber; }
            set { _ItemNumber = value; }
        }

        [DataMember]
        public DateTime InvoiceDate
        {
            get { return _InvoiceDate; }
            set { _InvoiceDate = value; }
        }

        [DataMember]
        public string InvoiceNo
        {
            get { return _InvoiceNo; }
            set { _InvoiceNo = value; }
        }
    }

    [DataContract]
    public class ParamPaymentData
    {
        private string _CPOIdSAP = null;
        private string _ItemNumber = null;
        private DateTime _PaymentDate = DateTime.MinValue;
        private string _InvoiceNo = null;

        [DataMember]
        public string CPOIdSAP
        {
            get { return _CPOIdSAP; }
            set { _CPOIdSAP = value; }
        }

        [DataMember]
        public string ItemNumber
        {
            get { return _ItemNumber; }
            set { _ItemNumber = value; }
        }

        [DataMember]
        public DateTime PaymentDate
        {
            get { return _PaymentDate; }
            set { _PaymentDate = value; }
        }

        [DataMember]
        public string InvoiceNo
        {
            get { return _InvoiceNo; }
            set { _InvoiceNo = value; }
        }
    }

    [DataContract]
    public class ParamReturData
    {
        private string _CPOIdSAP = null;
        private string _ItemNumber = null;
        private string _ReturNo = null;

        [DataMember]
        public string CPOIdSAP
        {
            get { return _CPOIdSAP; }
            set { _CPOIdSAP = value; }
        }

        [DataMember]
        public string ItemNumber
        {
            get { return _ItemNumber; }
            set { _ItemNumber = value; }
        }

        [DataMember]
        public string ReturNo
        {
            get { return _ReturNo; }
            set { _ReturNo = value; }
        }
    }

    [DataContract]
    public class ParamSPBData
    {
        private string _CPOIdSAP = null;
        private string _ItemNumber = null;
        private DateTime _SPBDate = DateTime.MinValue;
        private string _SPBNo = null;

        [DataMember]
        public string CPOIdSAP
        {
            get { return _CPOIdSAP; }
            set { _CPOIdSAP = value; }
        }

        [DataMember]
        public string ItemNumber
        {
            get { return _ItemNumber; }
            set { _ItemNumber = value; }
        }

        [DataMember]
        public DateTime SPBDate
        {
            get { return _SPBDate; }
            set { _SPBDate = value; }
        }

        [DataMember]
        public string SPBNo
        {
            get { return _SPBNo; }
            set { _SPBNo = value; }
        }
    }
    #endregion ---

    #region Master

    [DataContract]
    public class ParamProductData
    {
        private string _ProductID = null;
        private string _ProductType = null;
        private string _ProductName = null;
        private string _Description = null;
        private string _UnitGroup = null;
        private string _Currency = null;
        private string _MaterialGroup = null;
        private string _Division = null;
        private string _ProfitCenter = null;
        private string _SalesOrganization = null;
        private string _ExternalMaterialGroup = null;

        [DataMember]
        public string ProductID
        {
            get { return _ProductID; }
            set { _ProductID = value; }
        }

        [DataMember]
        public string ProductName
        {
            get { return _ProductName; }
            set { _ProductName = value; }
        }

        [DataMember]
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        [DataMember]
        public string UnitGroup
        {
            get { return _UnitGroup; }
            set { _UnitGroup = value; }
        }

        [DataMember]
        public string Currency
        {
            get { return _Currency; }
            set { _Currency = value; }
        }

        [DataMember]
        public string MaterialGroup
        {
            get { return _MaterialGroup; }
            set { _MaterialGroup = value; }
        }

        [DataMember]
        public string ProductType
        {
            get { return _ProductType; }
            set { _ProductType = value; }
        }

        [DataMember]
        public string Division
        {
            get { return _Division; }
            set { _Division = value; }
        }

        [DataMember]
        public string ProfitCenter
        {
            get { return _ProfitCenter; }
            set { _ProfitCenter = value; }
        }

        [DataMember]
        public string SalesOrganization
        {
            get { return _SalesOrganization; }
            set { _SalesOrganization = value; }
        }

        [DataMember]
        public string ExternalMaterialGroup
        {
            get { return _ExternalMaterialGroup; }
            set { _ExternalMaterialGroup = value; }
        }

        [DataMember]
        public List<ParamProductData_Characteristic> Characteristics { get; set; }
    }

    [DataContract]
    public class ParamProductData_Characteristic
    {
        private string _Characteristic = null;
        private string _CharacteristicValue = null;
        private string _CharacteristicValueDescription = null;
        private string _Classification = null;
        private string _ClassificationDescription = null;
        private string _ProductNumber = null;
        private string _PrintRelevant = null;

        [DataMember]
        public string Characteristic
        {
            get { return _Characteristic; }
            set { _Characteristic = value; }
        }

        [DataMember]
        public string CharacteristicValue
        {
            get { return _CharacteristicValue; }
            set { _CharacteristicValue = value; }
        }

        [DataMember]
        public string CharacteristicValueDescription
        {
            get { return _CharacteristicValueDescription; }
            set { _CharacteristicValueDescription = value; }
        }

        [DataMember]
        public string Classification
        {
            get { return _Classification; }
            set { _Classification = value; }
        }

        [DataMember]
        public string ClassificationDescription
        {
            get { return _ClassificationDescription; }
            set { _ClassificationDescription = value; }
        }

        [DataMember]
        public string ProductNumber
        {
            get { return _ProductNumber; }
            set { _ProductNumber = value; }
        }

        [DataMember]
        public string PrintRelevant
        {
            get { return _PrintRelevant; }
            set { _PrintRelevant = value; }
        }
    }

    [DataContract]
    public class ParamPopulationData
    {
        private string _SerialNumber = null;
        private string _EquipmentNumber = null;
        private string _EngineNumber = null;
        private string _Description = null;
        private string _Model = null;
        private string _Plant = null;
        private string _WarrantyStartDate = null;
        private string _WarrantyEndDate = null;
        private string _FunctionalLocation = null;
        private string _FunctionalLocationCode = null;
        private string _PONumber = null;
        private string _WRSDate = null;

        [DataMember]
        public string SerialNumber
        {
            get { return _SerialNumber; }
            set { _SerialNumber = value; }
        }

        [DataMember]
        public string EquipmentNumber
        {
            get { return _EquipmentNumber; }
            set { _EquipmentNumber = value; }
        }

        [DataMember]
        public string EngineNumber
        {
            get { return _EngineNumber; }
            set { _EngineNumber = value; }
        }

        [DataMember]
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        [DataMember]
        public string Model
        {
            get { return _Model; }
            set { _Model = value; }
        }

        [DataMember]
        public string Plant
        {
            get { return _Plant; }
            set { _Plant = value; }
        }

        [DataMember]
        public string WarrantyStartDate
        {
            get { return _WarrantyStartDate; }
            set { _WarrantyStartDate = value; }
        }

        [DataMember]
        public string WarrantyEndDate
        {
            get { return _WarrantyEndDate; }
            set { _WarrantyEndDate = value; }
        }

        [DataMember]
        public string FunctionalLocation
        {
            get { return _FunctionalLocation; }
            set { _FunctionalLocation = value; }
        }

        [DataMember]
        public string FunctionalLocationCode
        {
            get { return _FunctionalLocationCode; }
            set { _FunctionalLocationCode = value; }
        }

        [DataMember]
        public string PONumber
        {
            get { return _PONumber; }
            set { _PONumber = value; }
        }

        [DataMember]
        public string WRSDate
        {
            get { return _WRSDate; }
            set { _WRSDate = value; }
        }

        [DataMember]
        public List<ParamPopulationData_Characteristic> Characteristics { get; set; }
    }

    [DataContract]
    public class ParamPopulationData_Characteristic
    {
        private string _Characteristic = null;
        private string _CharacteristicValue = null;
        private string _CharacteristicValueDescription = null;
        private string _Classification = null;
        private string _ClassificationDescription = null;
        private string _SerialNumber = null;
        private string _PrintRelevant = null;

        [DataMember]
        public string Characteristic
        {
            get { return _Characteristic; }
            set { _Characteristic = value; }
        }

        [DataMember]
        public string CharacteristicValue
        {
            get { return _CharacteristicValue; }
            set { _CharacteristicValue = value; }
        }

        [DataMember]
        public string CharacteristicValueDescription
        {
            get { return _CharacteristicValueDescription; }
            set { _CharacteristicValueDescription = value; }
        }

        [DataMember]
        public string Classification
        {
            get { return _Classification; }
            set { _Classification = value; }
        }

        [DataMember]
        public string ClassificationDescription
        {
            get { return _ClassificationDescription; }
            set { _ClassificationDescription = value; }
        }

        [DataMember]
        public string SerialNumber
        {
            get { return _SerialNumber; }
            set { _SerialNumber = value; }
        }

        [DataMember]
        public string PrintRelevant
        {
            get { return _PrintRelevant; }
            set { _PrintRelevant = value; }
        }
    }

    [DataContract]
    public class ParamSalesBOMData
    {
        private string _ProductID = null;
        private string _AlternativeBOM = null;
        private string _IsDelete = null;

        [DataMember]
        public string ProductID
        {
            get { return _ProductID; }
            set { _ProductID = value; }
        }

        [DataMember]
        public string AlternativeBOM
        {
            get { return _AlternativeBOM; }
            set { _AlternativeBOM = value; }
        }

        [DataMember]
        public string IsDelete
        {
            get { return _IsDelete; }
            set { _IsDelete = value; }
        }

        [DataMember]
        public List<ParamSalesBOMData_Product> Products { get; set; }
    }

    [DataContract]
    public class ParamSalesBOMData_Product
    {
        private string _ProductID = null;
        private string _Mandatory = null;
        private string _Quantity = null;
        private string _IsDelete = null;

        [DataMember]
        public string ProductID
        {
            get { return _ProductID; }
            set { _ProductID = value; }
        }

        [DataMember]
        public string Mandatory
        {
            get { return _Mandatory; }
            set { _Mandatory = value; }
        }

        [DataMember]
        public string Quantity
        {
            get { return _Quantity; }
            set { _Quantity = value; }
        }

        [DataMember]
        public string IsDelete
        {
            get { return _IsDelete; }
            set { _IsDelete = value; }
        }
    }

    #endregion ---

    #region Transaction

    [DataContract]
    public class ParamIncentiveData
    {
        private string _CPOIDSAP = null;
        private string _ItemNumber = null;
        private string _F4 = null;
        private string _F5 = null;

        [DataMember]
        public string CPOIDSAP
        {
            get { return _CPOIDSAP; }
            set { _CPOIDSAP = value; }
        }

        [DataMember]
        public string ItemNumber
        {
            get { return _ItemNumber; }
            set { _ItemNumber = value; }
        }

        [DataMember]
        public string F4
        {
            get { return _F4; }
            set { _F4 = value; }
        }

        [DataMember]
        public string F5
        {
            get { return _F5; }
            set { _F5 = value; }
        }
    }

    [DataContract]
    public class ParamIncentivePaymentData
    {
        private string _CPOIDSAP = null;
        private string _ItemNumber = null;
        private string _PaymentSettlement = null;
        private string _Amount = null;
        private DateTime _PaymentDate = DateTime.MinValue;
        private string _RVNo = null;

        [DataMember]
        public string CPOIDSAP
        {
            get { return _CPOIDSAP; }
            set { _CPOIDSAP = value; }
        }

        [DataMember]
        public string ItemNumber
        {
            get { return _ItemNumber; }
            set { _ItemNumber = value; }
        }

        [DataMember]
        public string PaymentSettlement
        {
            get { return _PaymentSettlement; }
            set { _PaymentSettlement = value; }
        }

        [DataMember]
        public string Amount
        {
            get { return _Amount; }
            set { _Amount = value; }
        }

        [DataMember]
        public DateTime PaymentDate
        {
            get { return _PaymentDate; }
            set { _PaymentDate = value; }
        }

        [DataMember]
        public string RVNo
        {
            get { return _RVNo; }
            set { _RVNo = value; }
        }
    }

    [DataContract]
    public class ParamUpdateCPOData
    {
        private string _CPOIDSAP = null;
        private string _CPOIDSAPUpdateTo = null;

        [DataMember]
        public string CPOIDSAP
        {
            get { return _CPOIDSAP; }
            set { _CPOIDSAP = value; }
        }

        [DataMember]
        public string CPOIDSAPUpdateTo
        {
            get { return _CPOIDSAPUpdateTo; }
            set { _CPOIDSAPUpdateTo = value; }
        }
    }

    #endregion ---
}
