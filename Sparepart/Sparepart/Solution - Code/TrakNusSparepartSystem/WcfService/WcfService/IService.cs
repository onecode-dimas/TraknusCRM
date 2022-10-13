using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace TrakNusSparepartSystemWcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        string EncryptText(string value);

        [OperationContract]
        CRM_WS_Response CancelSO(string token, string SO_NO_SAP);

        [OperationContract]
        CRM_WS_Response UpdatePartStock(string token, ParamPartstockUpdate[] stocks, DateTime date, DateTime time);

        [OperationContract]
        CRM_WS_Response CreateDeliveryandSOSubline(string token, ParamCreateSOsubline[] sublines);

        [OperationContract]
        CRM_WS_Response UpdateSOSubline(string token, ParamSOsubline[] sublines);

        [OperationContract]
        CRM_WS_Response UpdateARBalanceCustomer(string token, ParamOutstandingARCustomer[] arbalance);

        [OperationContract]
        CRM_WS_Response UpdateDeliveryOrder(string token, ParamDeliveryOrder[] sublines);

        [OperationContract]
        CRM_WS_Response CreateInvoiceSO(string token, string invoice_No, DateTime invoice_Date, ParamCreateInvoiceSO[] invoice_Details);

        [OperationContract]
        CRM_WS_Response CancelInvoiceSO(string token, string invoice_No, string cancel_invoice_No, DateTime invoice_Date, ParamCancelInvoiceSO[] invoice_Details);

        [OperationContract]
        CRM_WS_Response CreateTO_SO(string token, string TO_NO_SAP, string DN_NO_SAP, DateTime TO_Date, DateTime TO_Time, ParamCreateTO_SO[] TO_Details);

        [OperationContract]
        CRM_WS_Response ConfirmTO_SO(string token, string TO_NO_SAP, string DN_NO_SAP, DateTime Conf_TO_Date, DateTime Conf_TO_Time, string Conf_TO_Status, ParamConfirmTO_SO[] TO_Details);

        [OperationContract]
        CRM_WS_Response PaidSalesOrder(string token, ParamPaidSalesOrder[] SalesOrders);

        [OperationContract]
        CRM_WS_Response CreateProductMaster(string token, ParamCreateProductMaster[] products);

        [OperationContract]
        CRM_WS_Response UpdateProductMasterInterchange(string token, ParamUpdateProductMasterInterchange[] products);
    }

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

    [DataContract]
    public class ParamPartstockUpdate
    {
        private string _sparepart_NO = string.Empty;
        private string _branch = string.Empty;
        private int _stockQty = 0;

        [DataMember]
        public string Sparepart_NO
        {
            get { return _sparepart_NO; }
            set { _sparepart_NO = value; }
        }

        [DataMember]
        public string Branch
        {
            get { return _branch; }
            set { _branch = value; }
        }

        [DataMember]
        public int StockQty
        {
            get { return _stockQty; }
            set { _stockQty = value; }
        }
    }

    [DataContract]
    public class ParamCreateSOsubline
    {
        private string _SO_NO_SAP = null;
        private string _partNumber = null;
        private string _partBranch = null;
        private string _Delivery_NO = null;
        private int _Delivery_qty = int.MinValue;
        private string _Delivery_NO_Reference = null;

        [DataMember]
        public int Delivery_Qty
        {
            get { return _Delivery_qty; }
            set { _Delivery_qty = value; }
        }

        [DataMember]
        public string SO_NO_SAP
        {
            get { return _SO_NO_SAP; }
            set { _SO_NO_SAP = value; }
        }

        [DataMember]
        public string PartNumber
        {
            get { return _partNumber; }
            set { _partNumber = value; }
        }

        [DataMember]
        public string PartBranch
        {
            get { return _partBranch; }
            set { _partBranch = value; }
        }
        
        [DataMember]
        public string Delivery_NO
        {
            get { return _Delivery_NO; }
            set { _Delivery_NO = value; }
        }

        [DataMember]
        public string Delivery_NO_Reference
        {
            get { return _Delivery_NO_Reference; }
            set { _Delivery_NO_Reference = value; }
        }
    }

    [DataContract]
    public class ParamSOsubline
    {
        private string _SO_NO_SAP = null;
        private string _partNumber = null;
        private string _partBranch = null;
        private int _qtyAvail = int.MinValue;
        private int _qtyInden = int.MinValue;
        private string _RSV_NO = null;
        private DateTime _RSV_DATE = DateTime.MinValue;
        private string _PO_NO_PRIN = null;
        private DateTime _PO_DATE_PRIN = DateTime.MinValue;
        private DateTime _ETA_JKT = DateTime.MinValue;
        private DateTime _WRS_DATE = DateTime.MinValue;
        private string _IBTS_NO = null;
        private DateTime _IBTS_DATE = DateTime.MinValue;
        private string _IBTS_WHS_NO = null;
        private string _WHS_BRANCH = null;
        private DateTime _WHS_BRANCH_DATE = DateTime.MinValue;

        [DataMember]
        public string SO_NO_SAP
        {
            get { return _SO_NO_SAP; }
            set { _SO_NO_SAP = value; }
        }

        [DataMember]
        public string PartNumber
        {
            get { return _partNumber; }
            set { _partNumber = value; }
        }

        [DataMember]
        public string PartBranch
        {
            get { return _partBranch; }
            set { _partBranch = value; }
        }

        [DataMember]
        public int QtyAvail
        {
            get { return _qtyAvail; }
            set { _qtyAvail = value; }
        }

        [DataMember]
        public int QtyInden
        {
            get { return _qtyInden; }
            set { _qtyInden = value; }
        }

        [DataMember]
        public string RSV_NO
        {
            get { return _RSV_NO; }
            set { _RSV_NO = value; }
        }

        [DataMember]
        public DateTime RSV_DATE
        {
            get { return _RSV_DATE; }
            set { _RSV_DATE = value; }
        }

        [DataMember]
        public string PO_NO_PRIN
        {
            get { return _PO_NO_PRIN; }
            set { _PO_NO_PRIN = value; }
        }

        [DataMember]
        public DateTime PO_DATE_PRIN
        {
            get { return _PO_DATE_PRIN; }
            set { _PO_DATE_PRIN = value; }
        }

        [DataMember]
        public DateTime ETA_JKT
        {
            get { return _ETA_JKT; }
            set { _ETA_JKT = value; }
        }

        [DataMember]
        public DateTime WRS_DATE
        {
            get { return _WRS_DATE; }
            set { _WRS_DATE = value; }
        }

        [DataMember]
        public string IBTS_NO
        {
            get { return _IBTS_NO; }
            set { _IBTS_NO = value; }
        }

        [DataMember]
        public DateTime IBTS_DATE
        {
            get { return _IBTS_DATE; }
            set { _IBTS_DATE = value; }
        }

        [DataMember]
        public string IBTS_WHS_NO
        {
            get { return _IBTS_WHS_NO; }
            set { _IBTS_WHS_NO = value; }
        }
        
        [DataMember]
        public string WHS_BRANCH
        {
            get { return _WHS_BRANCH; }
            set { _WHS_BRANCH = value; }
        }

        [DataMember]
        public DateTime WHS_BRANCH_DATE
        {
            get { return _WHS_BRANCH_DATE; }
            set { _WHS_BRANCH_DATE = value; }
        }
    }

    [DataContract]
    public class ParamDeliveryOrder
    {
        private string _SO_NO_SAP = string.Empty;
        private string _delivery_NO = string.Empty;
        private string _sparepart_NO = string.Empty;
        private string _partBranch = string.Empty;
        private int _delivery_Qty = 0;
        private DateTime _Exp_Pick_Up = DateTime.MinValue;
        private DateTime _Cust_Receipt_Date = DateTime.MinValue;

        [DataMember]
        public string SO_NO_SAP
        {
            get { return _SO_NO_SAP; }
            set { _SO_NO_SAP = value; }
        }

        [DataMember]
        public string Delivery_NO
        {
            get { return _delivery_NO; }
            set { _delivery_NO = value; }
        }

        [DataMember]
        public string PartNumber
        {
            get { return _sparepart_NO; }
            set { _sparepart_NO = value; }
        }

        [DataMember]
        public string PartBranch
        {
            get { return _partBranch; }
            set { _partBranch = value; }
        }

        [DataMember]
        public int Delivery_Qty
        {
            get { return _delivery_Qty; }
            set { _delivery_Qty = value; }
        }

        [DataMember]
        public DateTime Exp_Pick_Up
        {
            get { return _Exp_Pick_Up; }
            set { _Exp_Pick_Up = value; }
        }

        [DataMember]
        public DateTime Cust_Receipt_Date
        {
            get { return _Cust_Receipt_Date; }
            set { _Cust_Receipt_Date = value; }
        }
    }

    [DataContract]
    public class ParamOutstandingARCustomer
    {
        private string _CUST_ACC_NO = null;

        #region Unit AR Per Product
        private Int64 _MF_Current_Amt = 0;
        //private decimal _MF_Balance_Amt = 0;
        private Int64 _MF_Overdue_Amt = 0;
        private Int64 _MF_Overdue_Days = 0;

        private Int64 _PER_Current_Amt = 0;
        //private decimal _PER_Balance_Amt = 0;
        private Int64 _PER_Overdue_Amt = 0;
        private Int64 _PER_Overdue_Days = 0;

        private Int64 _TYT_Current_Amt = 0;
        //private decimal _TYT_Balance_Amt = 0;
        private Int64 _TYT_Overdue_Amt = 0;
        private Int64 _TYT_Overdue_Days = 0;

        private Int64 _STM_Current_Amt = 0;
        //private decimal _STM_Balance_Amt = 0;
        private Int64 _STM_Overdue_Amt = 0;
        private Int64 _STM_Overdue_Days = 0;

        private Int64 _SAK_Current_Amt = 0;
        //private decimal _SAK_Balance_Amt = 0;
        private Int64 _SAK_Overdue_Amt = 0;
        private Int64 _SAK_Overdue_Days = 0;

        private Int64 _GDN_Current_Amt = 0;
        //private decimal _GDN_Balance_Amt = 0;
        private Int64 _GDN_Overdue_Amt = 0;
        private Int64 _GDN_Overdue_Days = 0;

        private Int64 _FGW_Current_Amt = 0;
        //private decimal _FGW_Balance_Amt = 0;
        private Int64 _FGW_Overdue_Amt = 0;
        private Int64 _FGW_Overdue_Days = 0;

        private Int64 _BTF_Current_Amt = 0;
        //private decimal _BTF_Balance_Amt = 0;
        private Int64 _BTF_Overdue_Amt = 0;
        private Int64 _BTF_Overdue_Days = 0;

        [DataMember]
        public string CUST_ACC_NO
        {
            get { return _CUST_ACC_NO; }
            set { _CUST_ACC_NO = value; }
        }

        #region MF
        [DataMember]
        public Int64 MF_Current_Amt
        {
            get { return _MF_Current_Amt; }
            set { _MF_Current_Amt = value; }
        }
        //[DataMember]
        //public decimal MF_Balance_Amt
        //{
        //    get { return _MF_Balance_Amt; }
        //    set { _MF_Balance_Amt = value; }
        //}
        [DataMember]
        public Int64 MF_Overdue_Amt
        {
            get { return _MF_Overdue_Amt; }
            set { _MF_Overdue_Amt = value; }
        }
        [DataMember]
        public Int64 MF_Overdue_Days
        {
            get { return _MF_Overdue_Days; }
            set { _MF_Overdue_Days = value; }
        }
        #endregion
        #region PER
        [DataMember]
        public Int64 PER_Current_Amt
        {
            get { return _PER_Current_Amt; }
            set { _PER_Current_Amt = value; }
        }
        //[DataMember]
        //public decimal PER_Balance_Amt
        //{
        //    get { return _PER_Balance_Amt; }
        //    set { _PER_Balance_Amt = value; }
        //}
        [DataMember]
        public Int64 PER_Overdue_Amt
        {
            get { return _PER_Overdue_Amt; }
            set { _PER_Overdue_Amt = value; }
        }
        [DataMember]
        public Int64 PER_Overdue_Days
        {
            get { return _PER_Overdue_Days; }
            set { _PER_Overdue_Days = value; }
        }
        #endregion
        #region TYT
        [DataMember]
        public Int64 TYT_Current_Amt
        {
            get { return _TYT_Current_Amt; }
            set { _TYT_Current_Amt = value; }
        }
        //[DataMember]
        //public decimal TYT_Balance_Amt
        //{
        //    get { return _TYT_Balance_Amt; }
        //    set { _TYT_Balance_Amt = value; }
        //}
        [DataMember]
        public Int64 TYT_Overdue_Amt
        {
            get { return _TYT_Overdue_Amt; }
            set { _TYT_Overdue_Amt = value; }
        }
        [DataMember]
        public Int64 TYT_Overdue_Days
        {
            get { return _TYT_Overdue_Days; }
            set { _TYT_Overdue_Days = value; }
        }
        #endregion
        #region STM
        [DataMember]
        public Int64 STM_Current_Amt
        {
            get { return _STM_Current_Amt; }
            set { _STM_Current_Amt = value; }
        }
        //[DataMember]
        //public decimal STM_Balance_Amt
        //{
        //    get { return _STM_Balance_Amt; }
        //    set { _STM_Balance_Amt = value; }
        //}
        [DataMember]
        public Int64 STM_Overdue_Amt
        {
            get { return _STM_Overdue_Amt; }
            set { _STM_Overdue_Amt = value; }
        }
        [DataMember]
        public Int64 STM_Overdue_Days
        {
            get { return _STM_Overdue_Days; }
            set { _STM_Overdue_Days = value; }
        }
        #endregion
        #region SAK
        [DataMember]
        public Int64 SAK_Current_Amt
        {
            get { return _SAK_Current_Amt; }
            set { _SAK_Current_Amt = value; }
        }
        //[DataMember]
        //public decimal SAK_Balance_Amt
        //{
        //    get { return _SAK_Balance_Amt; }
        //    set { _SAK_Balance_Amt = value; }
        //}
        [DataMember]
        public Int64 SAK_Overdue_Amt
        {
            get { return _SAK_Overdue_Amt; }
            set { _SAK_Overdue_Amt = value; }
        }
        [DataMember]
        public Int64 SAK_Overdue_Days
        {
            get { return _SAK_Overdue_Days; }
            set { _SAK_Overdue_Days = value; }
        }
        #endregion
        #region GDN
        [DataMember]
        public Int64 GDN_Current_Amt
        {
            get { return _GDN_Current_Amt; }
            set { _GDN_Current_Amt = value; }
        }
        //[DataMember]
        //public decimal GDN_Balance_Amt
        //{
        //    get { return _GDN_Balance_Amt; }
        //    set { _GDN_Balance_Amt = value; }
        //}
        [DataMember]
        public Int64 GDN_Overdue_Amt
        {
            get { return _GDN_Overdue_Amt; }
            set { _GDN_Overdue_Amt = value; }
        }
        [DataMember]
        public Int64 GDN_Overdue_Days
        {
            get { return _GDN_Overdue_Days; }
            set { _GDN_Overdue_Days = value; }
        }
        #endregion
        #region FGW
        [DataMember]
        public Int64 FGW_Current_Amt
        {
            get { return _FGW_Current_Amt; }
            set { _FGW_Current_Amt = value; }
        }
        //[DataMember]
        //public decimal FGW_Balance_Amt
        //{
        //    get { return _FGW_Balance_Amt; }
        //    set { _FGW_Balance_Amt = value; }
        //}
        [DataMember]
        public Int64 FGW_Overdue_Amt
        {
            get { return _FGW_Overdue_Amt; }
            set { _FGW_Overdue_Amt = value; }
        }
        [DataMember]
        public Int64 FGW_Overdue_Days
        {
            get { return _FGW_Overdue_Days; }
            set { _FGW_Overdue_Days = value; }
        }
        #endregion
        #region BTF
        [DataMember]
        public Int64 BTF_Current_Amt
        {
            get { return _BTF_Current_Amt; }
            set { _BTF_Current_Amt = value; }
        }
        //[DataMember]
        //public decimal BTF_Balance_Amt
        //{
        //    get { return _BTF_Balance_Amt; }
        //    set { _BTF_Balance_Amt = value; }
        //}
        [DataMember]
        public Int64 BTF_Overdue_Amt
        {
            get { return _BTF_Overdue_Amt; }
            set { _BTF_Overdue_Amt = value; }
        }
        [DataMember]
        public Int64 BTF_Overdue_Days
        {
            get { return _BTF_Overdue_Days; }
            set { _BTF_Overdue_Days = value; }
        }
        #endregion
        #region UnitTotalAR
        private Int64 _Unit_Current_Amt = 0;
        //private decimal _Unit_Balance_Amt = 0;
        private Int64 _Unit_Overdue_Amt = 0;
        private Int64 _Unit_Overdue_Days = 0;

        [DataMember]
        public Int64 Unit_Current_Amt
        {
            get { return _Unit_Current_Amt; }
            set { _Unit_Current_Amt = value; }
        }
        //[DataMember]
        //public decimal Unit_Balance_Amt
        //{
        //    get { return _Unit_Balance_Amt; }
        //    set { _Unit_Balance_Amt = value; }
        //}
        [DataMember]
        public Int64 Unit_Overdue_Amt
        {
            get { return _Unit_Overdue_Amt; }
            set { _Unit_Overdue_Amt = value; }
        }
        [DataMember]
        public Int64 Unit_Overdue_Days
        {
            get { return _Unit_Overdue_Days; }
            set { _Unit_Overdue_Days = value; }
        }
        #endregion
        #region ServiceTotalAR
        private Int64 _Service_Current_Amt = 0;
        //private decimal _Service_Balance_Amt = 0;
        private Int64 _Service_Overdue_Amt = 0;
        private Int64 _Service_Overdue_Days = 0;

        [DataMember]
        public Int64 Service_Current_Amt
        {
            get { return _Service_Current_Amt; }
            set { _Service_Current_Amt = value; }
        }
        //[DataMember]
        //public decimal Service_Balance_Amt
        //{
        //    get { return _Service_Balance_Amt; }
        //    set { _Service_Balance_Amt = value; }
        //}
        [DataMember]
        public Int64 Service_Overdue_Amt
        {
            get { return _Service_Overdue_Amt; }
            set { _Service_Overdue_Amt = value; }
        }
        [DataMember]
        public Int64 Service_Overdue_Days
        {
            get { return _Service_Overdue_Days; }
            set { _Service_Overdue_Days = value; }
        }
        #endregion
        #region SparepartTotalAR
        private Int64 _Sparepart_Current_Amt = 0;
        //private decimal _Sparepart_Balance_Amt = 0;
        private Int64 _Sparepart_Overdue_Amt = 0;
        private Int64 _Sparepart_Overdue_Days = 0;

        [DataMember]
        public Int64 Sparepart_Current_Amt
        {
            get { return _Sparepart_Current_Amt; }
            set { _Sparepart_Current_Amt = value; }
        }
        //[DataMember]
        //public decimal Sparepart_Balance_Amt
        //{
        //    get { return _Sparepart_Balance_Amt; }
        //    set { _Sparepart_Balance_Amt = value; }
        //}
        [DataMember]
        public Int64 Sparepart_Overdue_Amt
        {
            get { return _Sparepart_Overdue_Amt; }
            set { _Sparepart_Overdue_Amt = value; }
        }
        [DataMember]
        public Int64 Sparepart_Overdue_Days
        {
            get { return _Sparepart_Overdue_Days; }
            set { _Sparepart_Overdue_Days = value; }
        }
        #endregion
        #region RentalTotalAR
        private Int64 _Rental_Current_Amt = 0;
        //private decimal _Rental_Balance_Amt = 0;
        private Int64 _Rental_Overdue_Amt = 0;
        private Int64 _Rental_Overdue_Days = 0;

        [DataMember]
        public Int64 Rental_Current_Amt
        {
            get { return _Rental_Current_Amt; }
            set { _Rental_Current_Amt = value; }
        }
        //[DataMember]
        //public decimal Rental_Balance_Amt
        //{
        //    get { return _Rental_Balance_Amt; }
        //    set { _Rental_Balance_Amt = value; }
        //}
        [DataMember]
        public Int64 Rental_Overdue_Amt
        {
            get { return _Rental_Overdue_Amt; }
            set { _Rental_Overdue_Amt = value; }
        }
        [DataMember]
        public Int64 Rental_Overdue_Days
        {
            get { return _Rental_Overdue_Days; }
            set { _Rental_Overdue_Days = value; }
        }
        #endregion

        #region unused
        //[DataMember]
        //public UnitTotalAR Unit_TotalAR
        //{
        //    get { return _Unit_TotalAR; }
        //    set { _Unit_TotalAR = value; }
        //}

        //[DataMember]
        //public ServiceTotalAR Service_TotalAR
        //{
        //    get { return _Service_TotalAR; }
        //    set { _Service_TotalAR = value; }
        //}

        //[DataMember]
        //public SparepartTotalAR Sparepart_TotalAR
        //{
        //    get { return _Sparepart_TotalAR; }
        //    set { _Sparepart_TotalAR = value; }
        //}
        #endregion
        #endregion
    }

    //[DataContract]
    //public class UnitTotalAR
    //{
    //    #region UnitTotalAR
    //    private decimal _Unit_Current_Amt = 0;
    //    private decimal _Unit_Balance_Amt = 0;
    //    private decimal _Unit_Overdue_Amt = 0;
    //    private int _Unit_Overdue_Days = 0;

    //    [DataMember]
    //    public decimal Unit_Current_Amt
    //    {
    //        get { return _Unit_Current_Amt; }
    //        set { _Unit_Current_Amt = value; }
    //    }
    //    [DataMember]
    //    public decimal Unit_Balance_Amt
    //    {
    //        get { return _Unit_Balance_Amt; }
    //        set { _Unit_Balance_Amt = value; }
    //    }
    //    [DataMember]
    //    public decimal Unit_Overdue_Amt
    //    {
    //        get { return _Unit_Overdue_Amt; }
    //        set { _Unit_Overdue_Amt = value; }
    //    }
    //    [DataMember]
    //    public int Unit_Overdue_Days
    //    {
    //        get { return _Unit_Overdue_Days; }
    //        set { _Unit_Overdue_Days = value; }
    //    }
    //    #endregion
    //}

    //[DataContract]
    //public class ServiceTotalAR
    //{
    //    #region ServiceTotalAR
    //    private decimal _Service_Current_Amt = 0;
    //    private decimal _Service_Balance_Amt = 0;
    //    private decimal _Service_Overdue_Amt = 0;
    //    private int _Service_Overdue_Days = 0;

    //    [DataMember]
    //    public decimal Service_Current_Amt
    //    {
    //        get { return _Service_Current_Amt; }
    //        set { _Service_Current_Amt = value; }
    //    }
    //    [DataMember]
    //    public decimal Service_Balance_Amt
    //    {
    //        get { return _Service_Balance_Amt; }
    //        set { _Service_Balance_Amt = value; }
    //    }
    //    [DataMember]
    //    public decimal Service_Overdue_Amt
    //    {
    //        get { return _Service_Overdue_Amt; }
    //        set { _Service_Overdue_Amt = value; }
    //    }
    //    [DataMember]
    //    public int Service_Overdue_Days
    //    {
    //        get { return _Service_Overdue_Days; }
    //        set { _Service_Overdue_Days = value; }
    //    }
    //    #endregion
    //}

    //[DataContract]
    //public class SparepartTotalAR
    //{
    //    #region SparepartTotalAR
    //    private decimal _Sparepart_Current_Amt = 0;
    //    private decimal _Sparepart_Balance_Amt = 0;
    //    private decimal _Sparepart_Overdue_Amt = 0;
    //    private int _Sparepart_Overdue_Days = 0;

    //    [DataMember]
    //    public decimal Sparepart_Current_Amt
    //    {
    //        get { return _Sparepart_Current_Amt; }
    //        set { _Sparepart_Current_Amt = value; }
    //    }
    //    [DataMember]
    //    public decimal Sparepart_Balance_Amt
    //    {
    //        get { return _Sparepart_Balance_Amt; }
    //        set { _Sparepart_Balance_Amt = value; }
    //    }
    //    [DataMember]
    //    public decimal Sparepart_Overdue_Amt
    //    {
    //        get { return _Sparepart_Overdue_Amt; }
    //        set { _Sparepart_Overdue_Amt = value; }
    //    }
    //    [DataMember]
    //    public int Sparepart_Overdue_Days
    //    {
    //        get { return _Sparepart_Overdue_Days; }
    //        set { _Sparepart_Overdue_Days = value; }
    //    }
    //    #endregion
    //}

    [DataContract]
    public class ParamCreateInvoiceSO
    {
        private string _SO_NO_SAP = null;
        private string _sparepart_NO = null;
        private string _partBranch = null;
        private int _Qty = 0;
        private int _INV_VAL = 0;

        [DataMember]
        public string SO_NO_SAP
        {
            get { return _SO_NO_SAP; }
            set { _SO_NO_SAP = value; }
        }

        [DataMember]
        public string Sparepart_NO
        {
            get { return _sparepart_NO; }
            set { _sparepart_NO = value; }
        }

        [DataMember]
        public string PartBranch
        {
            get { return _partBranch; }
            set { _partBranch = value; }
        }

        [DataMember]
        public int Quantity
        {
            get { return _Qty; }
            set { _Qty = value; }
        }

        [DataMember]
        public int INV_Value
        {
            get { return _INV_VAL; }
            set { _INV_VAL = value; }
        }
    }

    [DataContract]
    public class ParamCancelInvoiceSO
    {
        private string _SO_NO_SAP = null;
        private string _sparepart_NO = null;
        private string _partBranch = null;
        private int _Qty = 0;
        private int _INV_VAL = 0;

        [DataMember]
        public string SO_NO_SAP
        {
            get { return _SO_NO_SAP; }
            set { _SO_NO_SAP = value; }
        }

        [DataMember]
        public string Sparepart_NO
        {
            get { return _sparepart_NO; }
            set { _sparepart_NO = value; }
        }

        [DataMember]
        public string PartBranch
        {
            get { return _partBranch; }
            set { _partBranch = value; }
        }

        [DataMember]
        public int Quantity
        {
            get { return _Qty; }
            set { _Qty = value; }
        }

        [DataMember]
        public int INV_Value
        {
            get { return _INV_VAL; }
            set { _INV_VAL = value; }
        }
    }

    [DataContract]
    public class ParamCreateTO_SO
    {
        private string _SO_NO_SAP = null;
        private string _sparepart_NO = null;
        private string _partBranch = null;
        private int _Qty = 0;

        [DataMember]
        public string SO_NO_SAP
        {
            get { return _SO_NO_SAP; }
            set { _SO_NO_SAP = value; }
        }

        [DataMember]
        public string Sparepart_NO
        {
            get { return _sparepart_NO; }
            set { _sparepart_NO = value; }
        }

        [DataMember]
        public string PartBranch
        {
            get { return _partBranch; }
            set { _partBranch = value; }
        }

        [DataMember]
        public int Quantity
        {
            get { return _Qty; }
            set { _Qty = value; }
        }
    }

    [DataContract]
    public class ParamConfirmTO_SO
    {
        private string _SO_NO_SAP = null;
        private string _sparepart_NO = null;
        private string _partBranch = null;
        private int _Qty = 0;

        [DataMember]
        public string SO_NO_SAP
        {
            get { return _SO_NO_SAP; }
            set { _SO_NO_SAP = value; }
        }

        [DataMember]
        public string Sparepart_NO
        {
            get { return _sparepart_NO; }
            set { _sparepart_NO = value; }
        }

        [DataMember]
        public string PartBranch
        {
            get { return _partBranch; }
            set { _partBranch = value; }
        }

        [DataMember]
        public int Quantity
        {
            get { return _Qty; }
            set { _Qty = value; }
        }
    }

    [DataContract]
    public class ParamPaidSalesOrder
    {
        private string _SO_NO_SAP = null;
        private string _INV_NO = null;
        private DateTime _paid_DATE = DateTime.MinValue;
        private string _sparepart_NO = null;
        private string _partBranch = null;
        private int _paid_VAL = 0;

        [DataMember]
        public string SO_NO_SAP
        {
            get { return _SO_NO_SAP; }
            set { _SO_NO_SAP = value; }
        }

        [DataMember]
        public string INV_Number
        {
            get { return _INV_NO; }
            set { _INV_NO = value; }
        }

        [DataMember]
        public DateTime Paid_DATE
        {
            get { return _paid_DATE; }
            set { _paid_DATE = value; }
        }

        [DataMember]
        public string Sparepart_NO
        {
            get { return _sparepart_NO; }
            set { _sparepart_NO = value; }
        }

        [DataMember]
        public string PartBranch
        {
            get { return _partBranch; }
            set { _partBranch = value; }
        }

        [DataMember]
        public int Paid_Value
        {
            get { return _paid_VAL; }
            set { _paid_VAL = value; }
        }
    }

    [DataContract]
    public class ParamCreateProductMaster
    {
        private string _sparepart_NO = string.Empty;
        private string _description = string.Empty;
        private string _UoM = string.Empty;
        private string _unitGroup = string.Empty;
        private string _product = string.Empty;

        [DataMember]
        public string Sparepart_NO
        {
            get { return _sparepart_NO; }
            set { _sparepart_NO = value; }
        }

        [DataMember]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        [DataMember]
        public string UoM
        {
            get { return _UoM; }
            set { _UoM = value; }
        }

        [DataMember]
        public string UnitGroup
        {
            get { return _unitGroup; }
            set { _unitGroup = value; }
        }

        [DataMember]
        public string Product
        {
            get { return _product; }
            set { _product = value; }
        }
    }

    [DataContract]
    public class ParamUpdateProductMasterInterchange
    {
        private string _sparepart_NO = string.Empty;
        private string _sparepart_NO_Inter = string.Empty;
        private DateTime _start_Inter = DateTime.MinValue.Date;
        private DateTime _end_Inter = DateTime.MaxValue.Date;

        [DataMember]
        public string Sparepart_NO
        {
            get { return _sparepart_NO; }
            set { _sparepart_NO = value; }
        }

        [DataMember]
        public string Sparepart_NO_Interchange
        {
            get { return _sparepart_NO_Inter; }
            set { _sparepart_NO_Inter = value; }
        }

        [DataMember]
        public DateTime Start_Interchange
        {
            get { return _start_Inter; }
            set { _start_Inter = value; }
        }

        [DataMember]
        public DateTime End_Interchange
        {
            get { return _end_Inter; }
            set { _end_Inter = value; }
        }
    }
}
