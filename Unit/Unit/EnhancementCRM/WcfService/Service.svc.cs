using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Reflection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Security.Cryptography;
using System.Configuration;
using WcfService;
using Microsoft.Xrm.Tooling.Connector;
using EnhancementCRM.HelperUnit;
using System.Net;

namespace WcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service : IServicePRD
    {
        #region Constants
        private string UniqueKey = ConfigurationManager.AppSettings["UniqueKey"].ToString();

        private const string _EntityName_Account = "account";
        private const string _EntityName_CPO = "salesorder";
        private const string _EntityName_CPOLines = "salesorderdetail";
        private const string _EntityName_Currency = "transactioncurrency";
        private const string _EntityName_Division = "new_division";
        private const string _EntityName_FunctionalLocation = "trs_functionallocation";
        private const string _EntityName_Incentive = "new_incentive";
        private const string _EntityName_IncentivePayment = "ittn_incentivepayment";
        private const string _EntityName_IncentiveF4Parameter = "new_f4incentiveparameter";
        private const string _EntityName_IncentiveF5Parameter = "new_f5incentiveparameter";
        private const string _EntityName_approvallistincentive = "ittn_approvallistincentive";
        private const string _EntityName_matrixapprovalincentive = "ittn_matrixapprovalincentive";
        private const string _EntityName_MaterialGroup = "new_materialgroup";
        private const string _EntityName_MasterCharacteristic = "ittn_mastercharacteristic";
        private const string _EntityName_MasterClassification = "ittn_masterclassification";
        private const string _EntityName_Population = "new_population";
        private const string _EntityName_PopulationCharacteristic = "ittn_populationcharacteristic";
        private const string _EntityName_Product = "product";
        private const string _EntityName_ProductCharacteristic = "ittn_productcharacteristic";
        private const string _entityname_salesbom = "ittn_salesbom";
        private const string _entityname_salesbomproducts = "ittn_salesbomproducts";
        private const string _EntityName_UnitGroup = "uomschedule";
        private const string _EntityName_Uom = "uom";
        private const string _EntityName_SalesOrganization = "new_salesorganization";
        private const string _EntityName_systemuser = "systemuser";

        private const int CPOLines_Status_New = 1;
        private const int CPOLines_Status_ProcessedToSAP = 2;
        private const int CPOLines_Status_UnitAllocated = 3;
        private const int CPOLines_Status_DOIssued = 4;
        private const int CPOLines_Status_SRCreated = 5;
        private const int CPOLines_Status_BAST = 6;
        private const int CPOLines_Status_Invoice = 7;
        private const int CPOLines_Status_Paid = 8;
        private const int CPOLines_Status_Cancel = 9;
        private const int CPOLines_Status_Error = 10;
        private const int CPOLines_Status_Retur = 11;
        private const int CPOLines_Status_SPB = 12;
        #endregion

        #region Dependencies
        private Generator _Generator = new Generator();
        private MWSLogWebService _mwsLog = new MWSLogWebService();
        #endregion

        #region Privates
        private static Boolean IsValidConnectionString(String connectionString)
        {
            if (connectionString.Replace(" ", "").ToLower().Contains("url=") ||
                connectionString.Replace(" ", "").ToLower().Contains("server=") ||
                connectionString.Replace(" ", "").ToLower().Contains("serviceuri="))
                return true;

            return false;
        }
        #endregion

        #region Publics
        #region Create Token
        public string EncryptText(string text)
        {
            SyncronizeResult syncronizedResult = new SyncronizeResult();
            try
            {
                string EncryptedText = _Generator.Encrypt(text, UniqueKey);

                //_mwsLog.Write(MethodBase.GetCurrentMethod().Name, "Encrypted Text : " + EncryptedText, MWSLogWebService.LogType.Information, MWSLogWebService.Source.Inbound);
                syncronizedResult.Success = true;

                return (EncryptedText);
            }
            catch (Exception ex)
            {
                //_mwsLog.Write(MethodBase.GetCurrentMethod().Name, text + _mwsLog.ColumnsSeparator
                //     + "Error : " + ex.Message.ToString(), MWSLogWebService.LogType.Error, MWSLogWebService.Source.Inbound);
                syncronizedResult.Success = false;
                syncronizedResult.ErrorMessage = ex.Message.ToString();
            }
            return syncronizedResult.ErrorMessage;
        }
        #endregion

        #region Allocation
        public CRM_WS_Response Allocation(string Token, ParamAllocationData[] Param)
        {
            CRM_WS_Response response = new CRM_WS_Response();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {
                string _connectionString = string.Empty;

                if (IsValidConnectionString(ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString))
                    _connectionString = ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString;

                using (CrmServiceClient conn = new CrmServiceClient(_connectionString))
                {
                    IOrganizationService organizationService = (IOrganizationService)conn.OrganizationServiceProxy;

                    String SerialNumberForToken = Param[0].SerialNumber;
                    String GenerateToken = _Generator.Encrypt(SerialNumberForToken, UniqueKey);
                    if (string.Equals(GenerateToken, Token))
                    {
                        #region Process
                        #region Looping Param
                        EntityCollection ecParam = new EntityCollection();

                        for (int i = 0; i < Param.Length; i++)
                        {
                            ParamAllocationData lines = Param[i];
                            if (!string.IsNullOrEmpty(lines.CPOIdSAP) && !string.IsNullOrEmpty(lines.ItemNumber) && !string.IsNullOrEmpty(lines.AllocationDate.ToString()) && !string.IsNullOrEmpty(lines.SerialNumber))
                            {
                                string CPOIDSAP = lines.CPOIdSAP;
                                string ItemNumber = lines.ItemNumber;
                                DateTime AllocationDate = lines.AllocationDate;
                                string SerialNumber = lines.SerialNumber;

                                #region Find CPO Data
                                QueryExpression qeCPOheader = new QueryExpression(_EntityName_CPO);
                                qeCPOheader.ColumnSet = new ColumnSet(true);
                                qeCPOheader.Criteria.AddCondition("new_cpoidsap", ConditionOperator.Equal, CPOIDSAP);
                                Entity ecCPO = organizationService.RetrieveMultiple(qeCPOheader).Entities.FirstOrDefault();

                                if (ecCPO != null)
                                {
                                    
                                    QueryExpression qeCPOLines = new QueryExpression(_EntityName_CPOLines);
                                    qeCPOLines.ColumnSet = new ColumnSet(true);
                                    qeCPOLines.Criteria.AddCondition("salesorderid", ConditionOperator.Equal, ecCPO.Id);
                                    qeCPOLines.Criteria.AddCondition("new_itemnumber", ConditionOperator.Equal, ItemNumber);
                                    EntityCollection ecCPOLines = organizationService.RetrieveMultiple(qeCPOLines);

                                    if (ecCPOLines != null)
                                    {
                                        Entity CPOLines = ecCPOLines.Entities.FirstOrDefault();

                                        #region Update Allocation Data
                                        Entity UpdateCPOLines = new Entity(_EntityName_CPOLines);
                                        UpdateCPOLines["new_tanggalallocation"] = AllocationDate;
                                        UpdateCPOLines["new_serialnumber"] = SerialNumber;
                                        UpdateCPOLines["new_statuscpo"] = new OptionSetValue(CPOLines_Status_UnitAllocated);
                                        UpdateCPOLines.Id = CPOLines.Id;
                                        organizationService.Update(UpdateCPOLines);
                                        #endregion

                                        response.Result = "Success";
                                        response.ErrorDescription = "Success to Create Allocation Data on CRM CPO. Details : CPO ID SAP : " + CPOIDSAP + ", Item Number : " + ItemNumber + ", Serial Number : " + SerialNumber;
                                        response.SyncDate = DateTime.Now;
                                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator + response.ErrorDescription, MWSLogWebService.LogType.Information, MWSLogWebService.Source.Inbound);
                                    }
                                    else
                                    {
                                        throw new Exception("Cannot found CPO CRM with CPO ID SAP : " + CPOIDSAP + " and Item Number : " + ItemNumber + " !");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Cannot found CPO CRM with CPO ID SAP : " + CPOIDSAP + " !");
                                }
                                #endregion
                            }
                            else
                            {
                                throw new Exception("Parameter CPO ID SAP / Item Number / Allocation Date / Serial Number is Empty!");
                            }
                        }
                        #endregion
                        #endregion
                    }
                    else
                    {
                        throw new Exception("Access Denied, Invalid Token!");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = "Failed";
                response.ErrorDescription = "Failed to Create Allocation on CPO Lines on CRM. Details : " + ex.Message;
                response.SyncDate = DateTime.Now;
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator
                    + "Error : " + ex.Message.ToString(), MWSLogWebService.LogType.Error, MWSLogWebService.Source.Inbound);
            }
            return response;
        }
        #endregion

        #region Delivery Order
        public CRM_WS_Response DeliveryOrder(string Token, ParamDeliveryOrderData[] Param)
        {
            CRM_WS_Response response = new CRM_WS_Response();

            try
            {
                string _connectionString = string.Empty;

                if (IsValidConnectionString(ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString))
                    _connectionString = ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString;

                using (CrmServiceClient conn = new CrmServiceClient(_connectionString))
                {
                    IOrganizationService organizationService = (IOrganizationService)conn.OrganizationServiceProxy;

                    String DONoForToken = Param[0].DONo;
                    String GenerateToken = _Generator.Encrypt(DONoForToken, UniqueKey);
                    if (string.Equals(GenerateToken, Token))
                    {
                        #region Process
                        #region Looping Param
                        EntityCollection ecParam = new EntityCollection();

                        for (int i = 0; i < Param.Length; i++)
                        {
                            ParamDeliveryOrderData lines = Param[i];
                            if (!string.IsNullOrEmpty(lines.CPOIdSAP) && !string.IsNullOrEmpty(lines.ItemNumber) && !string.IsNullOrEmpty(lines.DODate.ToString()) && !string.IsNullOrEmpty(lines.DONo))
                            {
                                string CPOIDSAP = lines.CPOIdSAP;
                                string ItemNumber = lines.ItemNumber;
                                DateTime DODate = lines.DODate;
                                string DONo = lines.DONo;

                                #region Find CPO Data
                                QueryExpression qeCPOheader = new QueryExpression(_EntityName_CPO);
                                qeCPOheader.ColumnSet = new ColumnSet(true);
                                qeCPOheader.Criteria.AddCondition("new_cpoidsap", ConditionOperator.Equal, CPOIDSAP);
                                Entity ecCPO = organizationService.RetrieveMultiple(qeCPOheader).Entities.FirstOrDefault();

                                if (ecCPO != null)
                                {
                                    
                                    QueryExpression qeCPOLines = new QueryExpression(_EntityName_CPOLines);
                                    qeCPOLines.ColumnSet = new ColumnSet(true);
                                    qeCPOLines.Criteria.AddCondition("salesorderid", ConditionOperator.Equal, ecCPO.Id);
                                    qeCPOLines.Criteria.AddCondition("new_itemnumber", ConditionOperator.Equal, ItemNumber);
                                    EntityCollection ecCPOLines = organizationService.RetrieveMultiple(qeCPOLines);

                                    if (ecCPOLines != null)
                                    {
                                        Entity CPOLines = ecCPOLines.Entities.FirstOrDefault();

                                        #region Update Delivery Order Data
                                        Entity UpdateCPOLines = new Entity(_EntityName_CPOLines);
                                        UpdateCPOLines["new_tanggaldo"] = DODate;
                                        UpdateCPOLines["new_nomordo"] = DONo;
                                        UpdateCPOLines["new_statuscpo"] = new OptionSetValue(CPOLines_Status_DOIssued);
                                        UpdateCPOLines.Id = CPOLines.Id;
                                        organizationService.Update(UpdateCPOLines);
                                        #endregion

                                        response.Result = "Success";
                                        response.ErrorDescription = "Success to Create Delivery Order Data on CRM CPO. Details : CPO ID SAP : " + CPOIDSAP + ", Item Number : " + ItemNumber + ", Delivery Order No : " + DONo;
                                        response.SyncDate = DateTime.Now;
                                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator + response.ErrorDescription, MWSLogWebService.LogType.Information, MWSLogWebService.Source.Inbound);
                                    }
                                    else
                                    {
                                        throw new Exception("Cannot found CPO CRM with CPO ID SAP : " + CPOIDSAP + " and Item Number : " + ItemNumber + " !");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Cannot found CPO CRM with CPO ID SAP : " + CPOIDSAP + " !");
                                }
                                #endregion
                            }
                            else
                            {
                                throw new Exception("Parameter CPO ID SAP / Item Number / Delivery Order Date / Delivery Order No is Empty!");
                            }
                        }
                        #endregion
                        #endregion
                    }
                    else
                    {
                        throw new Exception("Access Denied, Invalid Token!");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = "Failed";
                response.ErrorDescription = "Failed to Create Delivery Order on CPO Lines on CRM. Details : " + ex.Message;
                response.SyncDate = DateTime.Now;
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator
                    + "Error : " + ex.Message.ToString(), MWSLogWebService.LogType.Error, MWSLogWebService.Source.Inbound);
            }
            return response;
        }
        #endregion

        #region Service Requisition
        public CRM_WS_Response ServiceRequisition(string Token, ParamServiceRequisitionData[] Param)
        {
            CRM_WS_Response response = new CRM_WS_Response();

            try
            {
                string _connectionString = string.Empty;

                if (IsValidConnectionString(ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString))
                    _connectionString = ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString;

                using (CrmServiceClient conn = new CrmServiceClient(_connectionString))
                {
                    IOrganizationService organizationService = (IOrganizationService)conn.OrganizationServiceProxy;

                    String SRNoForToken = Param[0].SRNo;
                    String GenerateToken = _Generator.Encrypt(SRNoForToken, UniqueKey);
                    if (string.Equals(GenerateToken, Token))
                    {
                        #region Process
                        #region Looping Param
                        EntityCollection ecParam = new EntityCollection();

                        for (int i = 0; i < Param.Length; i++)
                        {
                            ParamServiceRequisitionData lines = Param[i];
                            if (!string.IsNullOrEmpty(lines.CPOIdSAP) && !string.IsNullOrEmpty(lines.ItemNumber) && !string.IsNullOrEmpty(lines.SRDate.ToString()) && !string.IsNullOrEmpty(lines.SRNo))
                            {
                                string CPOIDSAP = lines.CPOIdSAP;
                                string ItemNumber = lines.ItemNumber;
                                DateTime SRDate = lines.SRDate;
                                string SRNo = lines.SRNo;

                                #region Find CPO Data
                                QueryExpression qeCPOheader = new QueryExpression(_EntityName_CPO);
                                qeCPOheader.ColumnSet = new ColumnSet(true);
                                qeCPOheader.Criteria.AddCondition("new_cpoidsap", ConditionOperator.Equal, CPOIDSAP);
                                Entity ecCPO = organizationService.RetrieveMultiple(qeCPOheader).Entities.FirstOrDefault();

                                if (ecCPO != null)
                                {
                                    
                                    QueryExpression qeCPOLines = new QueryExpression(_EntityName_CPOLines);
                                    qeCPOLines.ColumnSet = new ColumnSet(true);
                                    qeCPOLines.Criteria.AddCondition("salesorderid", ConditionOperator.Equal, ecCPO.Id);
                                    qeCPOLines.Criteria.AddCondition("new_itemnumber", ConditionOperator.Equal, ItemNumber);
                                    EntityCollection ecCPOLines = organizationService.RetrieveMultiple(qeCPOLines);

                                    if (ecCPOLines != null)
                                    {
                                        Entity CPOLines = ecCPOLines.Entities.FirstOrDefault();

                                        #region Update SR Data
                                        Entity UpdateCPOLines = new Entity(_EntityName_CPOLines);
                                        UpdateCPOLines["new_tanggalsr"] = SRDate;
                                        UpdateCPOLines["new_nomorsr"] = SRNo;
                                        UpdateCPOLines["new_statuscpo"] = new OptionSetValue(CPOLines_Status_SRCreated);
                                        UpdateCPOLines.Id = CPOLines.Id;
                                        organizationService.Update(UpdateCPOLines);
                                        #endregion

                                        response.Result = "Success";
                                        response.ErrorDescription = "Success to Create Service Requisition Data on CRM CPO. Details : CPO ID SAP : " + CPOIDSAP + ", Item Number : " + ItemNumber + ", Service Requisition No : " + SRNo;
                                        response.SyncDate = DateTime.Now;
                                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator + response.ErrorDescription, MWSLogWebService.LogType.Information, MWSLogWebService.Source.Inbound);
                                    }
                                    else
                                    {
                                        throw new Exception("Cannot found CPO CRM with CPO ID SAP : " + CPOIDSAP + " and Item Number : " + ItemNumber + " !");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Cannot found CPO CRM with CPO ID SAP : " + CPOIDSAP + " !");
                                }
                                #endregion
                            }
                            else
                            {
                                throw new Exception("Parameter CPO ID SAP / Item Number / Service Requisition Date / Service Requisition No is Empty!");
                            }
                        }
                        #endregion
                        #endregion
                    }
                    else
                    {
                        throw new Exception("Access Denied, Invalid Token!");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = "Failed";
                response.ErrorDescription = "Failed to Create Service Requisition on CPO Lines on CRM. Details : " + ex.Message;
                response.SyncDate = DateTime.Now;
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator
                    + "Error : " + ex.Message.ToString(), MWSLogWebService.LogType.Error, MWSLogWebService.Source.Inbound);
            }
            return response;
        }
        #endregion

        #region BAST
        public CRM_WS_Response BAST(string Token, ParamBASTData[] Param)
        {
            CRM_WS_Response response = new CRM_WS_Response();

            try
            {
                string _connectionString = string.Empty;

                if (IsValidConnectionString(ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString))
                    _connectionString = ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString;

                using (CrmServiceClient conn = new CrmServiceClient(_connectionString))
                {
                    IOrganizationService organizationService = (IOrganizationService)conn.OrganizationServiceProxy;

                    String BASTNoForToken = Param[0].BASTNo;
                    String GenerateToken = _Generator.Encrypt(BASTNoForToken, UniqueKey);
                    if (string.Equals(GenerateToken, Token))
                    {
                        #region Process
                        #region Looping Param
                        EntityCollection ecParam = new EntityCollection();

                        for (int i = 0; i < Param.Length; i++)
                        {
                            ParamBASTData lines = Param[i];
                            if (!string.IsNullOrEmpty(lines.CPOIdSAP) && !string.IsNullOrEmpty(lines.ItemNumber) && !string.IsNullOrEmpty(lines.BASTDate.ToString()) && !string.IsNullOrEmpty(lines.BASTNo))
                            {
                                string CPOIDSAP = lines.CPOIdSAP;
                                string ItemNumber = lines.ItemNumber;
                                DateTime BASTDate = lines.BASTDate;
                                string BASTNo = lines.BASTNo;

                                #region Find CPO Data
                                QueryExpression qeCPOheader = new QueryExpression(_EntityName_CPO);
                                qeCPOheader.ColumnSet = new ColumnSet(true);
                                qeCPOheader.Criteria.AddCondition("new_cpoidsap", ConditionOperator.Equal, CPOIDSAP);
                                Entity ecCPO = organizationService.RetrieveMultiple(qeCPOheader).Entities.FirstOrDefault();

                                if (ecCPO != null)
                                {
                                    
                                    QueryExpression qeCPOLines = new QueryExpression(_EntityName_CPOLines);
                                    qeCPOLines.ColumnSet = new ColumnSet(true);
                                    qeCPOLines.Criteria.AddCondition("salesorderid", ConditionOperator.Equal, ecCPO.Id);
                                    qeCPOLines.Criteria.AddCondition("new_itemnumber", ConditionOperator.Equal, ItemNumber);
                                    EntityCollection ecCPOLines = organizationService.RetrieveMultiple(qeCPOLines);

                                    if (ecCPOLines != null)
                                    {
                                        Entity CPOLines = ecCPOLines.Entities.FirstOrDefault();

                                        #region Update BAST Data
                                        Entity UpdateCPOLines = new Entity(_EntityName_CPOLines);
                                        UpdateCPOLines["new_tanggalbast"] = BASTDate;
                                        UpdateCPOLines["new_nomorbast"] = BASTNo;
                                        UpdateCPOLines["new_statuscpo"] = new OptionSetValue(CPOLines_Status_BAST);
                                        UpdateCPOLines.Id = CPOLines.Id;
                                        organizationService.Update(UpdateCPOLines);
                                        #endregion

                                        response.Result = "Success";
                                        response.ErrorDescription = "Success to Create BAST Data on CRM CPO. Details : CPO ID SAP : " + CPOIDSAP + ", Item Number : " + ItemNumber + ", BAST No : " + BASTNo;
                                        response.SyncDate = DateTime.Now;
                                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator + response.ErrorDescription, MWSLogWebService.LogType.Information, MWSLogWebService.Source.Inbound);
                                    }
                                    else
                                    {
                                        throw new Exception("Cannot found CPO CRM with CPO ID SAP : " + CPOIDSAP + " and Item Number : " + ItemNumber + " !");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Cannot found CPO CRM with CPO ID SAP : " + CPOIDSAP + " !");
                                }
                                #endregion
                            }
                            else
                            {
                                throw new Exception("Parameter CPO ID SAP / Item Number / BAST Date / BAST No is Empty!");
                            }
                        }
                        #endregion
                        #endregion
                    }
                    else
                    {
                        throw new Exception("Access Denied, Invalid Token!");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = "Failed";
                response.ErrorDescription = "Failed to Create BAST on CPO Lines on CRM. Details : " + ex.Message;
                response.SyncDate = DateTime.Now;
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator
                    + "Error : " + ex.Message.ToString(), MWSLogWebService.LogType.Error, MWSLogWebService.Source.Inbound);
            }
            return response;
        }
        #endregion

        #region Invoice
        public CRM_WS_Response Invoice(string Token, ParamInvoiceData[] Param)
        {
            CRM_WS_Response response = new CRM_WS_Response();

            try
            {
                string _connectionString = string.Empty;

                if (IsValidConnectionString(ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString))
                    _connectionString = ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString;

                using (CrmServiceClient conn = new CrmServiceClient(_connectionString))
                {
                    IOrganizationService organizationService = (IOrganizationService)conn.OrganizationServiceProxy;

                    String InvoiceNoForToken = Param[0].InvoiceNo;
                    String GenerateToken = _Generator.Encrypt(InvoiceNoForToken, UniqueKey);
                    if (string.Equals(GenerateToken, Token))
                    {
                        #region Process
                        #region Looping Param
                        EntityCollection ecParam = new EntityCollection();

                        for (int i = 0; i < Param.Length; i++)
                        {
                            ParamInvoiceData lines = Param[i];
                            if (!string.IsNullOrEmpty(lines.CPOIdSAP) && !string.IsNullOrEmpty(lines.ItemNumber) && !string.IsNullOrEmpty(lines.InvoiceDate.ToString()) && !string.IsNullOrEmpty(lines.InvoiceNo))
                            {
                                string CPOIDSAP = lines.CPOIdSAP;
                                string ItemNumber = lines.ItemNumber;
                                DateTime InvoiceDate = lines.InvoiceDate;
                                string InvoiceNo = lines.InvoiceNo;

                                #region Find CPO Data
                                QueryExpression qeCPOheader = new QueryExpression(_EntityName_CPO);
                                qeCPOheader.ColumnSet = new ColumnSet(true);
                                qeCPOheader.Criteria.AddCondition("new_cpoidsap", ConditionOperator.Equal, CPOIDSAP);
                                Entity ecCPO = organizationService.RetrieveMultiple(qeCPOheader).Entities.FirstOrDefault();

                                if (ecCPO != null)
                                {

                                    QueryExpression qeCPOLines = new QueryExpression(_EntityName_CPOLines);
                                    qeCPOLines.ColumnSet = new ColumnSet(true);
                                    qeCPOLines.Criteria.AddCondition("salesorderid", ConditionOperator.Equal, ecCPO.Id);
                                    qeCPOLines.Criteria.AddCondition("new_itemnumber", ConditionOperator.Equal, ItemNumber);
                                    EntityCollection ecCPOLines = organizationService.RetrieveMultiple(qeCPOLines);

                                    if (ecCPOLines != null)
                                    {
                                        Entity CPOLines = ecCPOLines.Entities.FirstOrDefault();

                                        #region Update BAST Data
                                        Entity UpdateCPOLines = new Entity(_EntityName_CPOLines);
                                        UpdateCPOLines["new_tanggalinvoice"] = InvoiceDate;
                                        UpdateCPOLines["new_nomorinvoice"] = InvoiceNo;
                                        UpdateCPOLines["new_statuscpo"] = new OptionSetValue(CPOLines_Status_Invoice);
                                        UpdateCPOLines.Id = CPOLines.Id;
                                        organizationService.Update(UpdateCPOLines);
                                        #endregion

                                        response.Result = "Success";
                                        response.ErrorDescription = "Success to Create Invoice Data on CRM CPO. Details : CPO ID SAP : " + CPOIDSAP + ", Item Number : " + ItemNumber + ", Invoice No : " + InvoiceNo;
                                        response.SyncDate = DateTime.Now;
                                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator + response.ErrorDescription, MWSLogWebService.LogType.Information, MWSLogWebService.Source.Inbound);
                                    }
                                    else
                                    {
                                        throw new Exception("Cannot found CPO CRM with CPO ID SAP : " + CPOIDSAP + " and Item Number : " + ItemNumber + " !");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Cannot found CPO CRM with CPO ID SAP : " + CPOIDSAP + " !");
                                }
                                #endregion
                            }
                            else
                            {
                                throw new Exception("Parameter CPO ID SAP / Item Number / Invoice Date / Invoice No is Empty!");
                            }
                        }
                        #endregion
                        #endregion
                    }
                    else
                    {
                        throw new Exception("Access Denied, Invalid Token!");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = "Failed";
                response.ErrorDescription = "Failed to Create Invoice on CPO Lines on CRM. Details : " + ex.Message;
                response.SyncDate = DateTime.Now;
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator
                    + "Error : " + ex.Message.ToString(), MWSLogWebService.LogType.Error, MWSLogWebService.Source.Inbound);
            }
            return response;
        }
        #endregion

        #region Paid
        public CRM_WS_Response Payment(string Token, ParamPaymentData[] Param)
        {
            CRM_WS_Response response = new CRM_WS_Response();

            try
            {
                string _connectionString = string.Empty;

                if (IsValidConnectionString(ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString))
                    _connectionString = ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString;

                using (CrmServiceClient conn = new CrmServiceClient(_connectionString))
                {
                    IOrganizationService organizationService = (IOrganizationService)conn.OrganizationServiceProxy;

                    String InvoiceNoForToken = Param[0].InvoiceNo;
                    String GenerateToken = _Generator.Encrypt(InvoiceNoForToken, UniqueKey);
                    if (string.Equals(GenerateToken, Token))
                    {
                        #region Process
                        #region Looping Param
                        EntityCollection ecParam = new EntityCollection();

                        for (int i = 0; i < Param.Length; i++)
                        {
                            ParamPaymentData lines = Param[i];
                            if (!string.IsNullOrEmpty(lines.CPOIdSAP) && !string.IsNullOrEmpty(lines.ItemNumber) && !string.IsNullOrEmpty(lines.PaymentDate.ToString()) && !string.IsNullOrEmpty(lines.InvoiceNo))
                            {
                                string CPOIDSAP = lines.CPOIdSAP;
                                string ItemNumber = lines.ItemNumber;
                                DateTime PaymentDate = lines.PaymentDate;
                                string InvoiceNo = lines.InvoiceNo;

                                #region Find CPO Data
                                QueryExpression qeCPOheader = new QueryExpression(_EntityName_CPO);
                                qeCPOheader.ColumnSet = new ColumnSet(true);
                                qeCPOheader.Criteria.AddCondition("new_cpoidsap", ConditionOperator.Equal, CPOIDSAP);
                                Entity ecCPO = organizationService.RetrieveMultiple(qeCPOheader).Entities.FirstOrDefault();

                                if (ecCPO != null)
                                {

                                    QueryExpression qeCPOLines = new QueryExpression(_EntityName_CPOLines);
                                    qeCPOLines.ColumnSet = new ColumnSet(true);
                                    qeCPOLines.Criteria.AddCondition("salesorderid", ConditionOperator.Equal, ecCPO.Id);
                                    qeCPOLines.Criteria.AddCondition("new_itemnumber", ConditionOperator.Equal, ItemNumber);
                                    qeCPOLines.Criteria.AddCondition("new_nomorinvoice", ConditionOperator.Equal, InvoiceNo);
                                    EntityCollection ecCPOLines = organizationService.RetrieveMultiple(qeCPOLines);

                                    if (ecCPOLines != null)
                                    {
                                        Entity CPOLines = ecCPOLines.Entities.FirstOrDefault();

                                        #region Update BAST Data
                                        Entity UpdateCPOLines = new Entity(_EntityName_CPOLines);
                                        UpdateCPOLines["new_paymentdate"] = PaymentDate;
                                        UpdateCPOLines["new_statuscpo"] = new OptionSetValue(CPOLines_Status_Paid);
                                        UpdateCPOLines.Id = CPOLines.Id;
                                        organizationService.Update(UpdateCPOLines);
                                        #endregion

                                        response.Result = "Success";
                                        response.ErrorDescription = "Success to Create Invoice Payment Data on CRM CPO. Details : CPO ID SAP : " + CPOIDSAP + ", Item Number : " + ItemNumber + ", Invoice No : " + InvoiceNo;
                                        response.SyncDate = DateTime.Now;
                                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator + response.ErrorDescription, MWSLogWebService.LogType.Information, MWSLogWebService.Source.Inbound);
                                    }
                                    else
                                    {
                                        throw new Exception("Cannot found CPO CRM with CPO ID SAP : " + CPOIDSAP + " and Item Number : " + ItemNumber + " !");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Cannot found CPO CRM with CPO ID SAP : " + CPOIDSAP + " !");
                                }
                                #endregion
                            }
                            else
                            {
                                throw new Exception("Parameter CPO ID SAP / Item Number / Payment Date / Invoice No is Empty!");
                            }
                        }
                        #endregion
                        #endregion
                    }
                    else
                    {
                        throw new Exception("Access Denied, Invalid Token!");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = "Failed";
                response.ErrorDescription = "Failed to Create Invoice Payment on CPO Lines on CRM. Details : " + ex.Message;
                response.SyncDate = DateTime.Now;
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator
                    + "Error : " + ex.Message.ToString(), MWSLogWebService.LogType.Error, MWSLogWebService.Source.Inbound);
            }
            return response;
        }
        #endregion

        #region SPB
        public CRM_WS_Response SPB(string Token, ParamSPBData[] Param)
        {
            CRM_WS_Response response = new CRM_WS_Response();

            try
            {
                string _connectionString = string.Empty;

                if (IsValidConnectionString(ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString))
                    _connectionString = ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString;

                using (CrmServiceClient conn = new CrmServiceClient(_connectionString))
                {
                    IOrganizationService organizationService = (IOrganizationService)conn.OrganizationServiceProxy;

                    String SPBNoForToken = Param[0].SPBNo;
                    String GenerateToken = _Generator.Encrypt(SPBNoForToken, UniqueKey);
                    if (string.Equals(GenerateToken, Token))
                    {
                        #region Process
                        #region Looping Param
                        EntityCollection ecParam = new EntityCollection();

                        for (int i = 0; i < Param.Length; i++)
                        {
                            ParamSPBData lines = Param[i];
                            if (!string.IsNullOrEmpty(lines.CPOIdSAP) && !string.IsNullOrEmpty(lines.ItemNumber) && !string.IsNullOrEmpty(lines.SPBDate.ToString()) && !string.IsNullOrEmpty(lines.SPBNo))
                            {
                                string CPOIDSAP = lines.CPOIdSAP;
                                string ItemNumber = lines.ItemNumber;
                                DateTime SPBDate = lines.SPBDate;
                                string SPBNo = lines.SPBNo;

                                #region Find CPO Data
                                QueryExpression qeCPOheader = new QueryExpression(_EntityName_CPO);
                                qeCPOheader.ColumnSet = new ColumnSet(true);
                                qeCPOheader.Criteria.AddCondition("new_cpoidsap", ConditionOperator.Equal, CPOIDSAP);
                                Entity ecCPO = organizationService.RetrieveMultiple(qeCPOheader).Entities.FirstOrDefault();

                                if (ecCPO != null)
                                {

                                    QueryExpression qeCPOLines = new QueryExpression(_EntityName_CPOLines);
                                    qeCPOLines.ColumnSet = new ColumnSet(true);
                                    qeCPOLines.Criteria.AddCondition("salesorderid", ConditionOperator.Equal, ecCPO.Id);
                                    qeCPOLines.Criteria.AddCondition("new_itemnumber", ConditionOperator.Equal, ItemNumber);
                                    EntityCollection ecCPOLines = organizationService.RetrieveMultiple(qeCPOLines);

                                    if (ecCPOLines != null)
                                    {
                                        Entity CPOLines = ecCPOLines.Entities.FirstOrDefault();

                                        #region Update SPB Data
                                        Entity UpdateCPOLines = new Entity(_EntityName_CPOLines);
                                        UpdateCPOLines["ittn_nomorspb"] = SPBNo;
                                        UpdateCPOLines["ittn_tanggalspb"] = SPBDate;
                                        UpdateCPOLines["new_statuscpo"] = new OptionSetValue(CPOLines_Status_SPB);
                                        UpdateCPOLines.Id = CPOLines.Id;
                                        organizationService.Update(UpdateCPOLines);
                                        #endregion

                                        response.Result = "Success";
                                        response.ErrorDescription = "Success to Create SPB Data on CRM CPO. Details : CPO ID SAP : " + CPOIDSAP + ", Item Number : " + ItemNumber + ", SPB No : " + SPBNo + ", SPB Date : " + SPBDate;
                                        response.SyncDate = DateTime.Now;
                                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator + response.ErrorDescription, MWSLogWebService.LogType.Information, MWSLogWebService.Source.Inbound);
                                    }
                                    else
                                    {
                                        throw new Exception("Cannot found CPO CRM with CPO ID SAP : " + CPOIDSAP + " and Item Number : " + ItemNumber + " !");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Cannot found CPO CRM with CPO ID SAP : " + CPOIDSAP + " !");
                                }
                                #endregion
                            }
                            else
                            {
                                throw new Exception("Parameter CPO ID SAP / Item Number / SPB Date / SPB No is Empty!");
                            }
                        }
                        #endregion
                        #endregion
                    }
                    else
                    {
                        throw new Exception("Access Denied, Invalid Token!");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = "Failed";
                response.ErrorDescription = "Failed to Create SPB on CPO Lines on CRM. Details : " + ex.Message;
                response.SyncDate = DateTime.Now;
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator
                    + "Error : " + ex.Message.ToString(), MWSLogWebService.LogType.Error, MWSLogWebService.Source.Inbound);
            }
            return response;
        }
        #endregion

        #region Retur
        public CRM_WS_Response Retur(string Token, ParamReturData[] Param)
        {
            CRM_WS_Response response = new CRM_WS_Response();

            try
            {
                string _connectionString = string.Empty;

                if (IsValidConnectionString(ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString))
                    _connectionString = ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString;

                using (CrmServiceClient conn = new CrmServiceClient(_connectionString))
                {
                    IOrganizationService organizationService = (IOrganizationService)conn.OrganizationServiceProxy;

                    String ReturNoForToken = Param[0].ReturNo;
                    String GenerateToken = _Generator.Encrypt(ReturNoForToken, UniqueKey);
                    if (string.Equals(GenerateToken, Token))
                    {
                        #region Process
                        #region Looping Param
                        EntityCollection ecParam = new EntityCollection();

                        for (int i = 0; i < Param.Length; i++)
                        {
                            ParamReturData lines = Param[i];
                            if (!string.IsNullOrEmpty(lines.CPOIdSAP) && !string.IsNullOrEmpty(lines.ItemNumber) && !string.IsNullOrEmpty(lines.ReturNo))
                            {
                                string CPOIDSAP = lines.CPOIdSAP;
                                string ItemNumber = lines.ItemNumber;
                                string ReturNo = lines.ReturNo;

                                #region Find CPO Data
                                QueryExpression qeCPOheader = new QueryExpression(_EntityName_CPO);
                                qeCPOheader.ColumnSet = new ColumnSet(true);
                                qeCPOheader.Criteria.AddCondition("new_cpoidsap", ConditionOperator.Equal, CPOIDSAP);
                                Entity ecCPO = organizationService.RetrieveMultiple(qeCPOheader).Entities.FirstOrDefault();

                                if (ecCPO != null)
                                {

                                    QueryExpression qeCPOLines = new QueryExpression(_EntityName_CPOLines);
                                    qeCPOLines.ColumnSet = new ColumnSet(true);
                                    qeCPOLines.Criteria.AddCondition("salesorderid", ConditionOperator.Equal, ecCPO.Id);
                                    qeCPOLines.Criteria.AddCondition("new_itemnumber", ConditionOperator.Equal, ItemNumber);
                                    EntityCollection ecCPOLines = organizationService.RetrieveMultiple(qeCPOLines);

                                    if (ecCPOLines != null)
                                    {
                                        Entity CPOLines = ecCPOLines.Entities.FirstOrDefault();

                                        #region Update BAST Data
                                        Entity UpdateCPOLines = new Entity(_EntityName_CPOLines);
                                        UpdateCPOLines["ittn_nomorsoretur"] = ReturNo;
                                        UpdateCPOLines["new_statuscpo"] = new OptionSetValue(CPOLines_Status_Retur);
                                        UpdateCPOLines.Id = CPOLines.Id;
                                        organizationService.Update(UpdateCPOLines);
                                        #endregion

                                        response.Result = "Success";
                                        response.ErrorDescription = "Success to Create Retur Data on CRM CPO. Details : CPO ID SAP : " + CPOIDSAP + ", Item Number : " + ItemNumber + ", Retur No : " + ReturNo;
                                        response.SyncDate = DateTime.Now;
                                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator + response.ErrorDescription, MWSLogWebService.LogType.Information, MWSLogWebService.Source.Inbound);
                                    }
                                    else
                                    {
                                        throw new Exception("Cannot found CPO CRM with CPO ID SAP : " + CPOIDSAP + " and Item Number : " + ItemNumber + " !");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Cannot found CPO CRM with CPO ID SAP : " + CPOIDSAP + " !");
                                }
                                #endregion
                            }
                            else
                            {
                                throw new Exception("Parameter CPO ID SAP / Item Number / Retur No is Empty!");
                            }
                        }
                        #endregion
                        #endregion
                    }
                    else
                    {
                        throw new Exception("Access Denied, Invalid Token!");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = "Failed";
                response.ErrorDescription = "Failed to Create Retur on CPO Lines on CRM. Details : " + ex.Message;
                response.SyncDate = DateTime.Now;
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator
                    + "Error : " + ex.Message.ToString(), MWSLogWebService.LogType.Error, MWSLogWebService.Source.Inbound);
            }
            return response;
        }
        #endregion

        #region Product
        public CRM_WS_Response Product(string Token, ParamProductData[] Param)
        {
            CRM_WS_Response response = new CRM_WS_Response();

            try
            {
                string _connectionString = string.Empty;

                if (IsValidConnectionString(ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString))
                    _connectionString = ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString;

                using (CrmServiceClient conn = new CrmServiceClient(_connectionString))
                {
                    IOrganizationService organizationService = (IOrganizationService)conn.OrganizationServiceProxy;

                    String ProductIDForToken = Param[0].ProductID;
                    String GenerateToken = _Generator.Encrypt(ProductIDForToken, UniqueKey);

                    if (string.Equals(GenerateToken, Token))
                    {
                        for (int i = 0; i < Param.Length; i++)
                        {
                            //if (Param[i].Characteristics.Count() == 0)
                            //    throw new Exception("Parameter <List> Product Characteristic is Empty!");

                            QueryExpression _queryexpression = new QueryExpression();
                            ParamProductData product = Param[i];

                            string ProductID = null;
                            string ProductType = null;
                            string ProductName = null;
                            string Description = null;
                            string UnitGroup = null;
                            string Currency = null;
                            string MaterialGroup = null;
                            string Division = null;
                            string ProfitCenter = null;
                            string SalesOrganization = null;
                            string ExternalMaterialGroup = null;

                            int? _producttype = null;
                            Guid _uomscheduleid = Guid.Empty;
                            Guid _uomid = Guid.Empty;
                            Guid _transactioncurrencyid = Guid.Empty;
                            Guid _materialgroupid = Guid.Empty;
                            Guid _divisionid = Guid.Empty;
                            Guid _salesorganizationid = Guid.Empty;
                            Guid _externalmaterialgroupid = Guid.Empty;
                            Guid _productid = Guid.Empty;
                            string _description = "";
                            bool _isnew = true;

                            Entity _product = new Entity(_EntityName_Product);

                            if (!string.IsNullOrEmpty(product.ProductID))
                            {
                                ProductID = product.ProductID;
                                _product["productnumber"] = ProductID;
                            }
                            else
                                throw new Exception("Parameter Product ID is Empty!");

                            // FIND PRODUCT DATA
                            #region FIND PRODUCT DATA
                            _queryexpression = new QueryExpression(_EntityName_Product);
                            _queryexpression.ColumnSet = new ColumnSet(true);
                            _queryexpression.Criteria.AddCondition("productnumber", ConditionOperator.Equal, ProductID);
                            EntityCollection _products = organizationService.RetrieveMultiple(_queryexpression);

                            _isnew = (_products.Entities.Count() == 0) ? true : false;
                            #endregion ---

                            // FIELDS ( CHECKING )
                            #region FIELDS ( CHECKING )
                            if (!string.IsNullOrEmpty(product.ProductType))
                            {
                                ProductType = product.ProductType;

                                if (ProductType == "Sales Inventory")
                                    _producttype = 1;
                                else if (ProductType == "Product Attachment")
                                    _producttype = 5;
                                else if (ProductType == "Service")
                                    _producttype = 7;

                                if (_producttype != null)
                                    _product["producttypecode"] = new OptionSetValue((int)_producttype);
                                else
                                    throw new Exception("Product Type '" + ProductType + "' is wrong! Use value 'Sales Inventory', 'Product Attachment', or 'Service' !");
                            }
                            else if (_isnew)
                                throw new Exception("Parameter Product Type is Empty!");

                            if (!string.IsNullOrEmpty(product.ProductName))
                            {
                                ProductName = product.ProductName;
                                _product["name"] = ProductName;
                            }
                            //else if (_isnew)
                            //    throw new Exception("Parameter Product Name is Empty!");

                            if (!string.IsNullOrEmpty(product.Description))
                            {
                                Description = product.Description;
                                _product["description"] = Description;
                            }
                            else if (_isnew)
                                throw new Exception("Parameter Description is Empty!");

                            if (!string.IsNullOrEmpty(product.UnitGroup))
                            {
                                UnitGroup = product.UnitGroup;

                                _queryexpression = new QueryExpression(_EntityName_UnitGroup);
                                _queryexpression.Criteria.AddCondition("baseuomname", ConditionOperator.Equal, UnitGroup);
                                EntityCollection _uomschedules = organizationService.RetrieveMultiple(_queryexpression);

                                if (_uomschedules.Entities.Count() > 0)
                                    _uomscheduleid = _uomschedules.Entities.FirstOrDefault().Id;
                                else
                                    throw new Exception("Unit Group '" + UnitGroup + "' is NOT found!");

                                _queryexpression = new QueryExpression(_EntityName_Uom);
                                _queryexpression.Criteria.AddCondition("name", ConditionOperator.Equal, UnitGroup);
                                EntityCollection _uoms = organizationService.RetrieveMultiple(_queryexpression);

                                if (_uoms.Entities.Count() > 0)
                                    _uomid = _uoms.Entities.FirstOrDefault().Id;
                                else
                                    throw new Exception("Unit of Measurements for Unit Group '" + UnitGroup + "' is NOT found!");

                                _product["defaultuomid"] = new EntityReference(_EntityName_Uom, _uomid);
                                _product["defaultuomscheduleid"] = new EntityReference(_EntityName_UnitGroup, _uomscheduleid);
                            }
                            else if (_isnew)
                                throw new Exception("Parameter Unit Group is Empty!");

                            if (!string.IsNullOrEmpty(product.Currency))
                            {
                                Currency = product.Currency;

                                _queryexpression = new QueryExpression(_EntityName_Currency);
                                _queryexpression.Criteria.AddCondition("currencyname", ConditionOperator.Equal, Currency);
                                EntityCollection _transactioncurrencys = organizationService.RetrieveMultiple(_queryexpression);

                                if (_transactioncurrencys.Entities.Count() > 0)
                                    _transactioncurrencyid = _transactioncurrencys.Entities.FirstOrDefault().Id;
                                else
                                    throw new Exception("Currency '" + Currency + "' is NOT found!");

                                _product["transactioncurrencyid"] = new EntityReference(_EntityName_Currency, _transactioncurrencyid);
                            }
                            else if (_isnew)
                                throw new Exception("Parameter Currency is Empty!");

                            if (!string.IsNullOrEmpty(product.MaterialGroup))
                            {
                                MaterialGroup = product.MaterialGroup;

                                _queryexpression = new QueryExpression(_EntityName_MaterialGroup);
                                _queryexpression.Criteria.AddCondition("new_mgcode", ConditionOperator.Equal, MaterialGroup);
                                EntityCollection _materialgroups = organizationService.RetrieveMultiple(_queryexpression);

                                if (_materialgroups.Entities.Count() == 0)
                                {
                                    Entity _materialgroup = new Entity(_EntityName_MaterialGroup);
                                    _materialgroup["new_mgcode"] = MaterialGroup;
                                    _materialgroupid = organizationService.Create(_materialgroup);
                                }
                                else
                                    _materialgroupid = _materialgroups.Entities.FirstOrDefault().Id;

                                _product["new_materialgroup"] = new EntityReference(_EntityName_MaterialGroup, _materialgroupid);
                            }
                            else if (_isnew)
                                throw new Exception("Parameter Material Group is Empty!");

                            if (!string.IsNullOrEmpty(product.Division))
                            {
                                Division = product.Division;

                                _queryexpression = new QueryExpression(_EntityName_Division);
                                _queryexpression.Criteria.AddCondition("new_code", ConditionOperator.Equal, Division);
                                EntityCollection _divisions = organizationService.RetrieveMultiple(_queryexpression);

                                if (_divisions.Entities.Count() > 0)
                                    _divisionid = _divisions.Entities.FirstOrDefault().Id;
                                else
                                    throw new Exception("Division '" + Division + "' is NOT found!");

                                _product["new_division"] = new EntityReference(_EntityName_Division, _divisionid);
                            }
                            else if (_isnew)
                                throw new Exception("Parameter Division is Empty!");

                            if (!string.IsNullOrEmpty(product.ProfitCenter))
                            {
                                ProfitCenter = product.ProfitCenter;
                                _product["new_profitcenter"] = ProfitCenter;
                            }
                            else if (_isnew)
                                throw new Exception("Parameter Profit Center is Empty!");

                            if (!string.IsNullOrEmpty(product.SalesOrganization))
                            {
                                SalesOrganization = product.SalesOrganization;

                                _queryexpression = new QueryExpression(_EntityName_SalesOrganization);
                                _queryexpression.Criteria.AddCondition("new_code", ConditionOperator.Equal, SalesOrganization);
                                EntityCollection _salesorganizations = organizationService.RetrieveMultiple(_queryexpression);

                                if (_salesorganizations.Entities.Count() > 0)
                                    _salesorganizationid = _salesorganizations.Entities.FirstOrDefault().Id;
                                else
                                    throw new Exception("Sales Organization '" + SalesOrganization + "' is NOT found!");

                                _product["new_salesorganization"] = new EntityReference(_EntityName_SalesOrganization, _salesorganizationid);
                            }
                            else if (_isnew)
                                throw new Exception("Parameter Sales Organization is Empty!");

                            if (!string.IsNullOrEmpty(product.ExternalMaterialGroup))
                            {
                                ExternalMaterialGroup = product.ExternalMaterialGroup;

                                _queryexpression = new QueryExpression(_EntityName_MaterialGroup);
                                _queryexpression.Criteria.AddCondition("new_mgcode", ConditionOperator.Equal, ExternalMaterialGroup);
                                EntityCollection _externalmaterialgroups = organizationService.RetrieveMultiple(_queryexpression);

                                if (_externalmaterialgroups.Entities.Count() == 0)
                                {
                                    Entity _externalmaterialgroup = new Entity(_EntityName_MaterialGroup);
                                    _externalmaterialgroup["new_mgcode"] = ExternalMaterialGroup;
                                    _externalmaterialgroupid = organizationService.Create(_externalmaterialgroup);
                                }
                                else
                                    _externalmaterialgroupid = _externalmaterialgroups.Entities.FirstOrDefault().Id;

                                _product["new_externalmaterialgroup"] = new EntityReference(_EntityName_MaterialGroup, _externalmaterialgroupid);
                            }
                            else if (_isnew)
                                throw new Exception("Parameter External Material Group is Empty!");
                            #endregion ---

                            // INSERT / UPDATE ( PRODUCT )
                            #region INSERT / UPDATE ( PRODUCT )
                            if (_isnew)
                            {
                                // INSERT PRODUCT
                                _productid = organizationService.Create(_product);
                            }
                            else
                            {
                                // UPDATE PRODUCT
                                _productid = _products.Entities.FirstOrDefault().Id;
                                _product.Id = _productid;

                                organizationService.Update(_product);
                            }
                            #endregion ---

                            // PRODUCT CHARACTERISTIC
                            #region PRODUCT CHARACTERISTIC
                            foreach (var item in Param[i].Characteristics)
                            {
                                string ProductNumber_Item = null;
                                string Characteristic = null;
                                string CharacteristicValue = null;
                                string CharacteristicValueDescription = null;
                                string Classification = null;
                                string ClassificationDescription = null;
                                bool PrintRelevant = false;

                                Guid _productid_item = Guid.Empty;
                                Guid _characteristicid = Guid.Empty;
                                Guid _classificationid = Guid.Empty;
                                Guid _productcharacteristicid = Guid.Empty;

                                if (!string.IsNullOrEmpty(item.ProductNumber))
                                {
                                    ProductNumber_Item = item.ProductNumber;

                                    _queryexpression = new QueryExpression(_EntityName_Product);
                                    _queryexpression.ColumnSet = new ColumnSet(true);
                                    _queryexpression.Criteria.AddCondition("productnumber", ConditionOperator.Equal, ProductNumber_Item);
                                    EntityCollection _products_items = organizationService.RetrieveMultiple(_queryexpression);

                                    if (_products_items.Entities.Count() > 0)
                                    {
                                        if (_productid == _products_items.Entities.FirstOrDefault().Id)
                                        {
                                            _productid_item = _products_items.Entities.FirstOrDefault().Id;

                                            Entity _productcharacteristic = new Entity(_EntityName_ProductCharacteristic);
                                            _productcharacteristic["ittn_product"] = new EntityReference(_EntityName_Product, _productid_item);

                                            if (!string.IsNullOrEmpty(item.Characteristic))
                                            {
                                                Characteristic = item.Characteristic;

                                                _queryexpression = new QueryExpression(_EntityName_MasterCharacteristic);
                                                _queryexpression.Criteria.AddCondition("ittn_code", ConditionOperator.Equal, Characteristic);
                                                EntityCollection _characteristics = organizationService.RetrieveMultiple(_queryexpression);

                                                if (_characteristics.Entities.Count() == 0)
                                                {
                                                    Entity _characteristic = new Entity(_EntityName_MasterCharacteristic);
                                                    _characteristic["ittn_code"] = Characteristic;
                                                    _characteristic["ittn_description"] = (CharacteristicValue != null ? CharacteristicValue : "");
                                                    _characteristicid = organizationService.Create(_characteristic);
                                                }
                                                else
                                                    _characteristicid = _characteristics.Entities.FirstOrDefault().Id;

                                                _productcharacteristic["ittn_name"] = Characteristic;
                                                _productcharacteristic["ittn_characteristic"] = new EntityReference(_EntityName_MasterCharacteristic, _characteristicid);
                                            }
                                            else
                                                throw new Exception("CHARACTERISTIC: Parameter Characteristic is Empty!");

                                            if (!string.IsNullOrEmpty(item.Classification))
                                            {
                                                Classification = item.Classification;

                                                _queryexpression = new QueryExpression(_EntityName_MasterClassification);
                                                _queryexpression.Criteria.AddCondition("ittn_code", ConditionOperator.Equal, Classification);
                                                EntityCollection _classifications = organizationService.RetrieveMultiple(_queryexpression);

                                                if (_classifications.Entities.Count() == 0)
                                                {
                                                    Entity _classification = new Entity(_EntityName_MasterClassification);
                                                    _classification["ittn_code"] = Classification;
                                                    _classification["ittn_description"] = ClassificationDescription;
                                                    _classificationid = organizationService.Create(_classification);
                                                }
                                                else
                                                    _classificationid = _classifications.Entities.FirstOrDefault().Id;

                                                _productcharacteristic["ittn_classification"] = new EntityReference(_EntityName_MasterClassification, _classificationid);
                                            }
                                            else
                                                throw new Exception("CHARACTERISTIC: Parameter Classification is Empty!");

                                            _queryexpression = new QueryExpression(_EntityName_ProductCharacteristic);
                                            _queryexpression.ColumnSet = new ColumnSet(true);
                                            _queryexpression.Criteria.AddCondition("ittn_product", ConditionOperator.Equal, _productid_item);
                                            _queryexpression.Criteria.AddCondition("ittn_characteristic", ConditionOperator.Equal, _characteristicid);
                                            _queryexpression.Criteria.AddCondition("ittn_classification", ConditionOperator.Equal, _classificationid);
                                            EntityCollection _productcharacteristics = organizationService.RetrieveMultiple(_queryexpression);

                                            bool _isnew_item = (_productcharacteristics.Entities.Count() == 0) ? true : false;

                                            if (!string.IsNullOrEmpty(item.CharacteristicValue))
                                            {
                                                CharacteristicValue = item.CharacteristicValue;
                                                _productcharacteristic["ittn_characteristicvalue"] = CharacteristicValue;
                                            }
                                            //else if(_isnew_item)
                                            //    throw new Exception("CHARACTERISTIC: Parameter Characteristic Value is Empty!");

                                            if (!string.IsNullOrEmpty(item.CharacteristicValueDescription))
                                            {
                                                CharacteristicValueDescription = item.CharacteristicValueDescription;
                                                _productcharacteristic["ittn_characteristicvaluedescription"] = CharacteristicValueDescription;
                                            }
                                            //else if(_isnew_item)
                                            //    throw new Exception("CHARACTERISTIC: Parameter Characteristic Value Description is Empty!");

                                            if (!string.IsNullOrEmpty(item.ClassificationDescription))
                                            {
                                                ClassificationDescription = item.ClassificationDescription;
                                            }
                                            else if (_isnew_item)
                                                throw new Exception("CHARACTERISTIC: Parameter Classification Description is Empty!");

                                            if (!string.IsNullOrEmpty(item.PrintRelevant))
                                            {
                                                try
                                                {
                                                    PrintRelevant = bool.Parse(item.PrintRelevant);
                                                    _productcharacteristic["ittn_use"] = PrintRelevant;
                                                }
                                                catch (Exception)
                                                {
                                                    throw new Exception("CHARACTERISTIC: Print Relevant is wrong! Use value 'TRUE' or 'FALSE' !");
                                                }
                                            }
                                            else
                                                throw new Exception("CHARACTERISTIC: Parameter Print Relevant is Empty!");

                                            // INSERT / UPDATE ( PRODUCT CHARACTERISTIC )
                                            #region INSERT / UPDATE ( PRODUCT )
                                            if (_isnew_item)
                                            {
                                                // INSERT PRODUCT CHARACTERISTIC
                                                organizationService.Create(_productcharacteristic);
                                            }
                                            else
                                            {
                                                // UPDATE PRODUCT CHARACTERISTIC
                                                _productcharacteristicid = _productcharacteristics.Entities.FirstOrDefault().Id;
                                                _productcharacteristic.Id = _productcharacteristicid;

                                                organizationService.Update(_productcharacteristic);
                                            }
                                            #endregion ---
                                        }
                                        else
                                            throw new Exception("CHARACTERISTIC: Product Number '" + product.ProductID + "' is different with Product Number in characteristic!");
                                    }
                                    else
                                        throw new Exception("CHARACTERISTIC: Product Number '" + ProductNumber_Item + "' is NOT found!");
                                }
                                //else
                                //    throw new Exception("CHARACTERISTIC: Parameter Product Number is Empty!");
                            }

                            // UPDATE POPULATION DESCRIPTION ( PRODUCT DETAILS )
                            _queryexpression = new QueryExpression(_EntityName_ProductCharacteristic);
                            _queryexpression.ColumnSet = new ColumnSet(true);
                            _queryexpression.Criteria.AddCondition("ittn_product", ConditionOperator.Equal, _productid);
                            _queryexpression.Criteria.AddCondition("ittn_use", ConditionOperator.Equal, true);
                            _queryexpression.Criteria.AddCondition("ittn_characteristicvaluedescription", ConditionOperator.NotEqual, "");
                            EntityCollection _descriptionupdate = organizationService.RetrieveMultiple(_queryexpression);

                            foreach (var item in _descriptionupdate.Entities)
                            {
                                _description += item.GetAttributeValue<string>("ittn_characteristicvaluedescription") + ", ";
                            }

                            if (_description.Length > 0)
                            {
                                Entity _product_update = new Entity(_EntityName_Product);
                                _product_update.Id = _productid;
                                _product_update["description"] = _description.Substring(0, _description.Length - 2);

                                organizationService.Update(_product_update);
                            }
                            #endregion ---

                            // INSERT LOG
                            #region INSERT LOG
                            response.Result = "Success";
                            response.ErrorDescription = "Success to " + ((_isnew) ? "Create" : "Update") + " Product Data on CRM. Details : Product ID : " + product.ProductID;
                            response.SyncDate = DateTime.Now;
                            _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator + response.ErrorDescription, MWSLogWebService.LogType.Information, MWSLogWebService.Source.Inbound);
                            #endregion ---
                        }
                    }
                    else
                    {
                        throw new Exception("Access Denied, Invalid Token!");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = "Failed";
                response.ErrorDescription = "Failed to Create Product on CRM. Details : " + ex.Message;
                response.SyncDate = DateTime.Now;
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator
                    + "Error : " + ex.Message.ToString(), MWSLogWebService.LogType.Error, MWSLogWebService.Source.Inbound);
            }
            return response;
        }
        #endregion

        #region Population
        public CRM_WS_Response Population(string Token, ParamPopulationData[] Param)
        {
            CRM_WS_Response response = new CRM_WS_Response();

            try
            {
                string _connectionString = string.Empty;

                if (IsValidConnectionString(ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString))
                    _connectionString = ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString;

                using (CrmServiceClient conn = new CrmServiceClient(_connectionString))
                {
                    IOrganizationService organizationService = (IOrganizationService)conn.OrganizationServiceProxy;

                    String SerialNumberIDForToken = Param[0].SerialNumber;
                    String GenerateToken = _Generator.Encrypt(SerialNumberIDForToken, UniqueKey);

                    if (string.Equals(GenerateToken, Token))
                    {
                        for (int i = 0; i < Param.Length; i++)
                        {
                            //if (Param[i].Characteristics.Count() == 0)
                            //    throw new Exception("Parameter <List> Population Characteristic is Empty!");

                            QueryExpression _queryexpression = new QueryExpression();
                            ParamPopulationData population = Param[i];

                            string SerialNumber = null;
                            string EquipmentNumber = null;
                            string EngineNumber = null;
                            string Description = null;
                            string Model = null;
                            string Plant = null;
                            DateTime? WarrantyStartDate = null;
                            DateTime? WarrantyEndDate = null;
                            string FunctionalLocation = null;
                            string FunctionalLocationCode = null;
                            string PONumber = null;
                            DateTime? WRSDate = null;

                            Guid _functionallocationid = Guid.Empty;
                            Guid _customerid = new Guid("FF8F47F2-77A1-E211-8445-005056926D22"); // ( TRAKTOR NUSANTARA, PT ) //
                            Guid _productid = Guid.Empty;
                            Guid _populationid = Guid.Empty;
                            string _description = "";
                            bool _isnew = true;

                            Entity _population = new Entity(_EntityName_Population);
                            _population["new_customercode"] = new EntityReference(_EntityName_Account, _customerid);

                            if (!string.IsNullOrEmpty(population.SerialNumber))
                            {
                                SerialNumber = population.SerialNumber;
                                _population["new_serialnumber"] = SerialNumber;
                            }
                            else
                                throw new Exception("Parameter Serial Number is Empty!");

                            if (!string.IsNullOrEmpty(population.EquipmentNumber))
                            {
                                EquipmentNumber = population.EquipmentNumber;
                                _population["trs_equipmentnumber"] = EquipmentNumber;
                            }
                            else
                                throw new Exception("Parameter Equipment Number is Empty!");

                            // FIND POPULATION DATA
                            #region FIND POPULATION DATA
                            _queryexpression = new QueryExpression(_EntityName_Population);
                            _queryexpression.ColumnSet = new ColumnSet(true);
                            _queryexpression.Criteria.AddCondition("new_serialnumber", ConditionOperator.Equal, SerialNumber);
                            EntityCollection _populations = organizationService.RetrieveMultiple(_queryexpression);

                            _isnew = (_populations.Entities.Count() == 0) ? true : false;
                            #endregion ---

                            // FIELDS ( CHECKING )
                            #region FIELDS ( CHECKING )
                            if (!string.IsNullOrEmpty(population.EngineNumber))
                            {
                                EngineNumber = population.EngineNumber;
                                _population["new_enginenumber"] = EngineNumber;
                            }
                            //else if (_isnew)
                            //    throw new Exception("Parameter Engine Number is Empty!");


                            if (!string.IsNullOrEmpty(population.Description))
                            {
                                Description = population.Description;
                                _population["new_description"] = Description;
                            }
                            //else if (_isnew)
                            //    throw new Exception("Parameter Description is Empty!");

                            if (!string.IsNullOrEmpty(population.Model))
                            {
                                Model = population.Model;

                                _queryexpression = new QueryExpression(_EntityName_Product);
                                _queryexpression.ColumnSet = new ColumnSet(true);
                                _queryexpression.Criteria.AddCondition("productnumber", ConditionOperator.Equal, Model);
                                EntityCollection _products = organizationService.RetrieveMultiple(_queryexpression);

                                if (_products.Entities.Count() > 0)
                                {
                                    _productid = _products.Entities.FirstOrDefault().Id;

                                    _population["new_model"] = Model;
                                    _population["trs_productmaster"] = new EntityReference(_EntityName_Product, _productid);
                                }
                                else
                                    throw new Exception("Model ( Product ) '" + Model + "' is NOT found!");
                            }
                            else if (_isnew)
                                throw new Exception("Parameter Model is Empty!");

                            if (!string.IsNullOrEmpty(population.Plant))
                            {
                                Plant = population.Plant;
                                _population["new_plant"] = Plant;
                            }
                            else if (_isnew)
                                throw new Exception("Parameter Plant is Empty!");

                            if (!string.IsNullOrEmpty(population.WarrantyStartDate))
                            {
                                try
                                {
                                    WarrantyStartDate = DateTime.Parse(population.WarrantyStartDate);
                                    _population["trs_warrantystartdate"] = WarrantyStartDate;
                                }
                                catch (Exception)
                                {
                                    throw new Exception("Format DateTime for Warranty Start Date is wrong! Use format YYYY-MM-DD !");
                                }
                            }
                            //else if (_isnew)
                            //    throw new Exception("Parameter Warranty Start Date is Empty!");

                            if (!string.IsNullOrEmpty(population.WarrantyEndDate))
                            {
                                try
                                {
                                    WarrantyEndDate = DateTime.Parse(population.WarrantyEndDate);
                                    _population["trs_warrantyenddate"] = WarrantyEndDate;
                                }
                                catch (Exception)
                                {
                                    throw new Exception("Format DateTime for Warranty End Date is wrong! Use format YYYY-MM-DD !");
                                }
                            }
                            //else if (_isnew)
                            //    throw new Exception("Parameter Warranty End Date is Empty!");

                            if (!string.IsNullOrEmpty(population.FunctionalLocation))
                            {
                                FunctionalLocation = population.FunctionalLocation;

                                _queryexpression = new QueryExpression(_EntityName_FunctionalLocation);
                                _queryexpression.ColumnSet = new ColumnSet(true);
                                _queryexpression.Criteria.AddCondition("trs_name", ConditionOperator.Equal, FunctionalLocation);
                                EntityCollection _functionallocations = organizationService.RetrieveMultiple(_queryexpression);

                                if (_functionallocations.Entities.Count() > 0)
                                {
                                    _functionallocationid = _functionallocations.Entities.FirstOrDefault().Id;
                                    _population["trs_functionallocation"] = new EntityReference(_EntityName_FunctionalLocation, _functionallocationid);
                                }
                                //else
                                //    throw new Exception("Functional Location '" + FunctionalLocation + "' is NOT found!");
                            }
                            //else if (_isnew)
                            //    throw new Exception("Parameter Functional Location is Empty!");

                            if (!string.IsNullOrEmpty(population.FunctionalLocationCode))
                            {
                                FunctionalLocationCode = population.FunctionalLocationCode;
                                _population["new_funcloccode"] = FunctionalLocationCode;
                            }
                            //else if (_isnew)
                            //    throw new Exception("Parameter Functional Location Code is Empty!");

                            if (!string.IsNullOrEmpty(population.PONumber))
                            {
                                PONumber = population.PONumber;
                                _population["trs_ponumber"] = PONumber;
                            }
                            //else if (_isnew)
                            //    throw new Exception("Parameter PO Number is Empty!");

                            if (!string.IsNullOrEmpty(population.WRSDate))
                            {
                                try
                                {
                                    WRSDate = DateTime.Parse(population.WRSDate);
                                    _population["trs_wrsdate"] = WRSDate;
                                }
                                catch (Exception)
                                {
                                    throw new Exception("Format DateTime for WRS Date is wrong! Use format YYYY-MM-DD !");
                                }
                            }
                            else if (_isnew)
                                throw new Exception("Parameter WRS Date is Empty!");
                            #endregion ---

                            // INSERT / UPDATE ( POPULATION )
                            #region INSERT / UPDATE ( POPULATION )
                            if (_isnew)
                            {
                                // INSERT POPULATION
                                _populationid = organizationService.Create(_population);
                            }
                            else
                            {
                                // UPDATE POPULATION
                                _populationid = _populations.Entities.FirstOrDefault().Id;
                                _population.Id = _populationid;

                                organizationService.Update(_population);
                            }
                            #endregion ---

                            // POPULATION CHARACTERISTIC
                            #region POPULATION CHARACTERISTIC
                            foreach (var item in Param[i].Characteristics)
                            {
                                string SerialNumber_Item = null;
                                string Characteristic = null;
                                string CharacteristicValue = null;
                                string CharacteristicValueDescription = null;
                                string Classification = null;
                                string ClassificationDescription = null;
                                bool PrintRelevant = false;

                                Guid _populationid_item = Guid.Empty;
                                Guid _characteristicid = Guid.Empty;
                                Guid _classificationid = Guid.Empty;
                                Guid _populationcharacteristicid = Guid.Empty;

                                if (!string.IsNullOrEmpty(item.SerialNumber))
                                {
                                    SerialNumber_Item = item.SerialNumber;

                                    _queryexpression = new QueryExpression(_EntityName_Population);
                                    _queryexpression.ColumnSet = new ColumnSet(true);
                                    _queryexpression.Criteria.AddCondition("new_serialnumber", ConditionOperator.Equal, SerialNumber_Item);
                                    EntityCollection _populations_items = organizationService.RetrieveMultiple(_queryexpression);

                                    if (_populations_items.Entities.Count() > 0)
                                    {
                                        if (_population.Id == _populations_items.Entities.FirstOrDefault().Id)
                                        {
                                            _populationid_item = _populations_items.Entities.FirstOrDefault().Id;

                                            Entity _populationcharacteristic = new Entity(_EntityName_PopulationCharacteristic);
                                            _populationcharacteristic["ittn_serialnumber"] = new EntityReference(_EntityName_Population, _populationid_item);

                                            if (!string.IsNullOrEmpty(item.Characteristic))
                                            {
                                                Characteristic = item.Characteristic;

                                                _queryexpression = new QueryExpression(_EntityName_MasterCharacteristic);
                                                _queryexpression.Criteria.AddCondition("ittn_code", ConditionOperator.Equal, Characteristic);
                                                EntityCollection _characteristics = organizationService.RetrieveMultiple(_queryexpression);

                                                if (_characteristics.Entities.Count() == 0)
                                                {
                                                    Entity _characteristic = new Entity(_EntityName_MasterCharacteristic);
                                                    _characteristic["ittn_code"] = Characteristic;
                                                    _characteristic["ittn_description"] = (CharacteristicValue != null ? CharacteristicValue : "");
                                                    _characteristicid = organizationService.Create(_characteristic);
                                                }
                                                else
                                                    _characteristicid = _characteristics.Entities.FirstOrDefault().Id;

                                                _populationcharacteristic["ittn_name"] = Characteristic;
                                                _populationcharacteristic["ittn_characteristic"] = new EntityReference(_EntityName_MasterCharacteristic, _characteristicid);
                                            }
                                            else
                                                throw new Exception("CHARACTERISTIC: Parameter Characteristic is Empty!");

                                            if (!string.IsNullOrEmpty(item.Classification))
                                            {
                                                Classification = item.Classification;

                                                _queryexpression = new QueryExpression(_EntityName_MasterClassification);
                                                _queryexpression.Criteria.AddCondition("ittn_code", ConditionOperator.Equal, Classification);
                                                EntityCollection _classifications = organizationService.RetrieveMultiple(_queryexpression);

                                                if (_classifications.Entities.Count() == 0)
                                                {
                                                    Entity _classification = new Entity(_EntityName_MasterClassification);
                                                    _classification["ittn_code"] = Classification;
                                                    _classification["ittn_description"] = ClassificationDescription;
                                                    _classificationid = organizationService.Create(_classification);
                                                }
                                                else
                                                    _classificationid = _classifications.Entities.FirstOrDefault().Id;

                                                _populationcharacteristic["ittn_classification"] = new EntityReference(_EntityName_MasterClassification, _classificationid);
                                            }
                                            else
                                                throw new Exception("CHARACTERISTIC: Parameter Classification is Empty!");

                                            _queryexpression = new QueryExpression(_EntityName_PopulationCharacteristic);
                                            _queryexpression.ColumnSet = new ColumnSet(true);
                                            _queryexpression.Criteria.AddCondition("ittn_serialnumber", ConditionOperator.Equal, _populationid_item);
                                            _queryexpression.Criteria.AddCondition("ittn_characteristic", ConditionOperator.Equal, _characteristicid);
                                            _queryexpression.Criteria.AddCondition("ittn_classification", ConditionOperator.Equal, _classificationid);
                                            EntityCollection _populationcharacteristics = organizationService.RetrieveMultiple(_queryexpression);

                                            bool _isnew_item = (_populationcharacteristics.Entities.Count() == 0) ? true : false;

                                            if (!string.IsNullOrEmpty(item.CharacteristicValue))
                                            {
                                                CharacteristicValue = item.CharacteristicValue;
                                                _populationcharacteristic["ittn_characteristicvalue"] = CharacteristicValue;
                                            }
                                            //else if(_isnew_item)
                                            //    throw new Exception("CHARACTERISTIC: Parameter Characteristic Value is Empty!");

                                            if (!string.IsNullOrEmpty(item.CharacteristicValueDescription))
                                            {
                                                CharacteristicValueDescription = item.CharacteristicValueDescription;
                                                _populationcharacteristic["ittn_characteristicvaluedescription"] = CharacteristicValueDescription;
                                            }
                                            //else if(_isnew_item)
                                            //    throw new Exception("CHARACTERISTIC: Parameter Characteristic Value Description is Empty!");

                                            if (!string.IsNullOrEmpty(item.ClassificationDescription))
                                            {
                                                ClassificationDescription = item.ClassificationDescription;
                                            }
                                            else if (_isnew_item)
                                                throw new Exception("CHARACTERISTIC: Parameter Classification Description is Empty!");

                                            if (!string.IsNullOrEmpty(item.PrintRelevant))
                                            {
                                                try
                                                {
                                                    PrintRelevant = bool.Parse(item.PrintRelevant);
                                                    _populationcharacteristic["ittn_use"] = PrintRelevant;
                                                }
                                                catch (Exception)
                                                {
                                                    throw new Exception("CHARACTERISTIC: Print Relevant is wrong! Use value 'TRUE' or 'FALSE' !");
                                                }
                                            }
                                            else
                                                throw new Exception("CHARACTERISTIC: Parameter Print Relevant is Empty!");

                                            // INSERT / UPDATE ( POPULATION CHARACTERISTIC )
                                            #region INSERT / UPDATE ( POPULATION CHARACTERISTIC )
                                            if (_isnew_item)
                                            {
                                                // INSERT POPULATION CHARACTERISTIC
                                                organizationService.Create(_populationcharacteristic);
                                            }
                                            else
                                            {
                                                // UPDATE POPULATION CHARACTERISTIC
                                                _populationcharacteristicid = _populationcharacteristics.Entities.FirstOrDefault().Id;
                                                _populationcharacteristic.Id = _populationcharacteristicid;

                                                organizationService.Update(_populationcharacteristic);
                                            }
                                            #endregion ---
                                        }
                                        else
                                            throw new Exception("CHARACTERISTIC: Serial Number '" + SerialNumber + "' is different with Serial Number in characteristic!");
                                    }
                                    else
                                        throw new Exception("CHARACTERISTIC: Serial Number '" + SerialNumber_Item + "' is NOT found!");
                                }
                                //else
                                //    throw new Exception("CHARACTERISTIC: Parameter Serial Number is Empty!");
                            }

                            // UPDATE POPULATION DESCRIPTION ( PRODUCT DETAILS )
                            _queryexpression = new QueryExpression(_EntityName_PopulationCharacteristic);
                            _queryexpression.ColumnSet = new ColumnSet(true);
                            _queryexpression.Criteria.AddCondition("ittn_serialnumber", ConditionOperator.Equal, _populationid);
                            _queryexpression.Criteria.AddCondition("ittn_use", ConditionOperator.Equal, true);
                            _queryexpression.Criteria.AddCondition("ittn_characteristicvaluedescription", ConditionOperator.NotEqual, "");
                            EntityCollection _descriptionupdate = organizationService.RetrieveMultiple(_queryexpression);

                            foreach (var item in _descriptionupdate.Entities)
                            {
                                _description += item.GetAttributeValue<string>("ittn_characteristicvaluedescription") + ", ";
                            }

                            if (_description.Length > 0)
                            {
                                Entity _population_update = new Entity(_EntityName_Population);
                                _population_update.Id = _populationid;
                                _population_update["new_description"] = _description.Substring(0, _description.Length - 2);

                                organizationService.Update(_population_update);
                            }
                            #endregion ---

                            // INSERT LOG
                            #region INSERT LOG
                            response.Result = "Success";
                            response.ErrorDescription = "Success to " + ((_isnew) ? "Create" : "Update") + " Population Data on CRM. Details : Serial Number : " + population.SerialNumber;
                            response.SyncDate = DateTime.Now;
                            _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator + response.ErrorDescription, MWSLogWebService.LogType.Information, MWSLogWebService.Source.Inbound);
                            #endregion ---
                        }
                    }
                    else
                    {
                        throw new Exception("Access Denied, Invalid Token!");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = "Failed";
                response.ErrorDescription = "Failed to Create Population on CRM. Details : " + ex.Message;
                response.SyncDate = DateTime.Now;
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator
                    + "Error : " + ex.Message.ToString(), MWSLogWebService.LogType.Error, MWSLogWebService.Source.Inbound);
            }
            return response;
        }
        #endregion

        #region Sales BOM
        public CRM_WS_Response SalesBOM(string Token, ParamSalesBOMData[] Param)
        {
            CRM_WS_Response response = new CRM_WS_Response();

            try
            {
                string _connectionString = string.Empty;

                if (IsValidConnectionString(ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString))
                    _connectionString = ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString;

                using (CrmServiceClient conn = new CrmServiceClient(_connectionString))
                {
                    IOrganizationService organizationService = (IOrganizationService)conn.OrganizationServiceProxy;

                    String ProductIDForToken = Param[0].ProductID;
                    String GenerateToken = _Generator.Encrypt(ProductIDForToken, UniqueKey);

                    if (string.Equals(GenerateToken, Token))
                    {
                        for (int i = 0; i < Param.Length; i++)
                        {
                            //if (Param[i].Products.Count() == 0)
                            //    throw new Exception("Parameter <List> Sales BOM Products is Empty!");

                            QueryExpression _queryexpression = new QueryExpression();
                            ParamSalesBOMData salesbom = Param[i];

                            string ProductID = null;
                            int? AlternativeBOM = null;
                            bool? IsDelete = null;

                            Guid _productid = new Guid();
                            Guid _salesbomid = new Guid();

                            bool _isnew = true;

                            Entity _salesbom = new Entity(_entityname_salesbom);
                            
                            if (!string.IsNullOrEmpty(salesbom.ProductID))
                            {
                                ProductID = salesbom.ProductID;

                                _queryexpression = new QueryExpression(_EntityName_Product);
                                _queryexpression.ColumnSet = new ColumnSet(true);
                                _queryexpression.Criteria.AddCondition("productnumber", ConditionOperator.Equal, ProductID);
                                EntityCollection _products = organizationService.RetrieveMultiple(_queryexpression);

                                if (_products.Entities.Count() > 0)
                                {
                                    _productid = _products.Entities.FirstOrDefault().Id;

                                    //_salesbom["ittn_name"] = ProductID;
                                    _salesbom["ittn_product"] = new EntityReference(_EntityName_Product, _productid);
                                }
                                else
                                    throw new Exception("Model ( Product ) '" + ProductID + "' is NOT found!");
                            }
                            else
                                throw new Exception("Parameter Serial Number is Empty!");

                            if (!string.IsNullOrEmpty(salesbom.AlternativeBOM))
                            {
                                try
                                {
                                    AlternativeBOM = int.Parse(salesbom.AlternativeBOM);
                                    _salesbom["ittn_alternativebom"] = AlternativeBOM;
                                    _salesbom["ittn_name"] = ProductID + " - " + AlternativeBOM;
                                }
                                catch (Exception)
                                {
                                    throw new Exception("Alternative BOM is wrong! Use INTEGER value !");
                                }
                            }
                            else if (_isnew)
                                throw new Exception("Parameter Alternative BOM is Empty!");

                            if (!string.IsNullOrEmpty(salesbom.IsDelete))
                            {
                                try
                                {
                                    IsDelete = bool.Parse(salesbom.IsDelete);
                                }
                                catch (Exception)
                                {
                                    throw new Exception("IsDelete is wrong! Use value 'TRUE' or 'FALSE' !");
                                }
                            }
                            else if (_isnew)
                                throw new Exception("Parameter IsDelete is Empty!  Use value 'TRUE' or 'FALSE' !");

                            // FIND SALES BOM DATA
                            #region FIND SALES BOM DATA
                            _queryexpression = new QueryExpression(_entityname_salesbom);
                            _queryexpression.ColumnSet = new ColumnSet(true);
                            _queryexpression.Criteria.AddCondition("ittn_product", ConditionOperator.Equal, _productid);
                            _queryexpression.Criteria.AddCondition("ittn_alternativebom", ConditionOperator.Equal, AlternativeBOM);
                            EntityCollection _salesboms = organizationService.RetrieveMultiple(_queryexpression);

                            _isnew = (_salesboms.Entities.Count() == 0) ? true : false;
                            #endregion ---

                            if ((bool)IsDelete)
                            {
                                // DELETE SALES BOM
                                if (_salesboms.Entities.Count() == 0)
                                {
                                    throw new Exception("Sales BOM Model ( Product ) '" + ProductID + "' with Alternative BOM '" + AlternativeBOM.ToString() + "' is NOT found! Delete failed !");
                                }
                                else
                                {
                                    _queryexpression = new QueryExpression(_entityname_salesbomproducts);
                                    _queryexpression.ColumnSet = new ColumnSet(true);
                                    _queryexpression.Criteria.AddCondition("ittn_salesbom", ConditionOperator.Equal, _salesboms.Entities.FirstOrDefault().Id);
                                    EntityCollection _delete_salesbomproducts = organizationService.RetrieveMultiple(_queryexpression);

                                    foreach (var _delete_salesbomproduct in _delete_salesbomproducts.Entities)
                                    {
                                        organizationService.Delete(_entityname_salesbomproducts, _delete_salesbomproduct.Id);
                                    }

                                    organizationService.Delete(_entityname_salesbom, _salesboms.Entities.FirstOrDefault().Id);

                                    // INSERT LOG
                                    #region INSERT LOG
                                    response.Result = "Success";
                                    response.ErrorDescription = "Success to Delete Sales BOM Data on CRM. Details : Sales BOM Product : " + salesbom.ProductID;
                                    response.SyncDate = DateTime.Now;
                                    _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator + response.ErrorDescription, MWSLogWebService.LogType.Information, MWSLogWebService.Source.Inbound);
                                    #endregion ---
                                }
                            }
                            else
                            {
                                // INSERT / UPDATE ( SALES BOM )
                                #region INSERT / UPDATE ( SALES BOM )
                                if (_isnew)
                                {
                                    // INSERT SALES BOM
                                    _salesbomid = organizationService.Create(_salesbom);
                                }
                                else
                                {
                                    // UPDATE SALES BOM
                                    _salesbomid = _salesboms.Entities.FirstOrDefault().Id;
                                    _salesbom.Id = _salesbomid;

                                    organizationService.Update(_salesbom);
                                }
                                #endregion ---

                                // SALES BOM PRODUCT
                                #region SALES BOM PRODUCT
                                foreach (var item in Param[i].Products)
                                {
                                    string ProductID_Item = null;
                                    bool? Mandatory = null;
                                    int? Quantity = null;
                                    bool? IsDelete_Item = null;

                                    Guid _productid_item = new Guid();
                                    Guid _salesbomproductid = new Guid();

                                    if (!string.IsNullOrEmpty(item.ProductID))
                                    {
                                        ProductID_Item = item.ProductID;

                                        _queryexpression = new QueryExpression(_EntityName_Product);
                                        _queryexpression.ColumnSet = new ColumnSet(true);
                                        _queryexpression.Criteria.AddCondition("productnumber", ConditionOperator.Equal, ProductID_Item);
                                        EntityCollection _products_items = organizationService.RetrieveMultiple(_queryexpression);

                                        if (_products_items.Entities.Count() > 0)
                                        {
                                            if (_productid != _products_items.Entities.FirstOrDefault().Id)
                                            {
                                                _productid_item = _products_items.Entities.FirstOrDefault().Id;

                                                _queryexpression = new QueryExpression(_entityname_salesbomproducts);
                                                _queryexpression.ColumnSet = new ColumnSet(true);
                                                _queryexpression.Criteria.AddCondition("ittn_salesbom", ConditionOperator.Equal, _salesbomid);
                                                _queryexpression.Criteria.AddCondition("ittn_product", ConditionOperator.Equal, _productid_item);
                                                EntityCollection _salesbomproductss = organizationService.RetrieveMultiple(_queryexpression);

                                                bool _isnew_item = (_salesbomproductss.Entities.Count() == 0) ? true : false;

                                                if (!string.IsNullOrEmpty(item.IsDelete))
                                                {
                                                    try
                                                    {
                                                        IsDelete_Item = bool.Parse(item.IsDelete);
                                                    }
                                                    catch (Exception)
                                                    {
                                                        throw new Exception("PRODUCT: IsDelete is wrong! Use value 'TRUE' or 'FALSE' !");
                                                    }
                                                }
                                                else if (_isnew_item)
                                                    throw new Exception("PRODUCT: Parameter IsDelete is Empty!  Use value 'TRUE' or 'FALSE' !");

                                                if ((bool)IsDelete_Item)
                                                {
                                                    // DELETE SALES BOM
                                                    if (_salesbomproductss.Entities.Count() == 0)
                                                    {
                                                        throw new Exception("PRODUCT: Sales BOM Model ( Product ) '" + ProductID_Item + "' is NOT found! Delete failed !");
                                                    }
                                                    else
                                                    {
                                                        organizationService.Delete(_entityname_salesbomproducts, _salesbomproductss.Entities.FirstOrDefault().Id);

                                                        // INSERT LOG
                                                        #region INSERT LOG
                                                        response.Result = "Success";
                                                        response.ErrorDescription = "Success to Delete Sales BOM Product Data on CRM. Details : Sales BOM Product : " + salesbom.ProductID;
                                                        response.SyncDate = DateTime.Now;
                                                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator + response.ErrorDescription, MWSLogWebService.LogType.Information, MWSLogWebService.Source.Inbound);
                                                        #endregion ---
                                                    }
                                                }
                                                else
                                                {
                                                    Entity _salesbomproducts = new Entity(_entityname_salesbomproducts);
                                                    _salesbomproducts["ittn_salesbom"] = new EntityReference(_entityname_salesbom, _salesbomid);
                                                    _salesbomproducts["ittn_name"] = ProductID_Item;
                                                    _salesbomproducts["ittn_product"] = new EntityReference(_EntityName_Product, _productid_item);

                                                    if (!string.IsNullOrEmpty(item.Mandatory))
                                                    {
                                                        if (item.Mandatory == "2")
                                                            Mandatory = false;
                                                        else if (item.Mandatory == "1")
                                                            Mandatory = true;
                                                        else
                                                            throw new Exception("PRODUCT: Mandatory is wrong! Use value '1' for TRUE or '2' for FALSE !");

                                                        _salesbomproducts["ittn_mandatory"] = Mandatory;
                                                    }
                                                    else if (_isnew_item)
                                                        throw new Exception("PRODUCT: Mandatory is Empty!");

                                                    if (!string.IsNullOrEmpty(item.Quantity))
                                                    {
                                                        try
                                                        {
                                                            Quantity = int.Parse(item.Quantity);
                                                            _salesbomproducts["ittn_quantity"] = Quantity;
                                                        }
                                                        catch (Exception)
                                                        {
                                                            throw new Exception("PRODUCT: Quantity is wrong! Use INTEGER value !");
                                                        }
                                                    }
                                                    else if (_isnew_item)
                                                        throw new Exception("PRODUCT: Parameter Quantity Value is Empty!");

                                                    // INSERT / UPDATE ( SALES BOM PRODUCT )
                                                    #region INSERT / UPDATE ( SALES BOM PRODUCT )
                                                    if (_isnew_item)
                                                    {
                                                        // INSERT SALES BOM PRODUCT
                                                        organizationService.Create(_salesbomproducts);
                                                    }
                                                    else
                                                    {
                                                        // UPDATE SALES BOM PRODUCT
                                                        _salesbomproductid = _salesbomproductss.Entities.FirstOrDefault().Id;
                                                        _salesbomproducts.Id = _salesbomproductid;

                                                        organizationService.Update(_salesbomproducts);
                                                    }
                                                    #endregion ---
                                                }

                                            }
                                            else
                                                throw new Exception("PRODUCT: Model ( Product ) '" + ProductID_Item + "' must be different with Model ( Product ) in product !");
                                        }
                                        else
                                            throw new Exception("PRODUCT: Model ( Product ) '" + ProductID_Item + "' is NOT found!");
                                    }
                                    //else
                                    //    throw new Exception("CHARACTERISTIC: Parameter Serial Number is Empty!");
                                }
                                #endregion ---

                                // INSERT LOG
                                #region INSERT LOG
                                response.Result = "Success";
                                response.ErrorDescription = "Success to " + ((_isnew) ? "Create" : "Update") + " Sales BOM Data on CRM. Details : Sales BOM Product : " + salesbom.ProductID;
                                response.SyncDate = DateTime.Now;
                                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator + response.ErrorDescription, MWSLogWebService.LogType.Information, MWSLogWebService.Source.Inbound);
                                #endregion ---
                            }

                        }
                    }
                    else
                    {
                        throw new Exception("Access Denied, Invalid Token!");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = "Failed";
                response.ErrorDescription = "Failed to Create SalesBOM on CRM. Details : " + ex.Message;
                response.SyncDate = DateTime.Now;
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator
                    + "Error : " + ex.Message.ToString(), MWSLogWebService.LogType.Error, MWSLogWebService.Source.Inbound);
            }
            return response;
        }
        #endregion

        #region Incentive
        public CRM_WS_Response Incentive(string Token, ParamIncentiveData[] Param)
        {
            CRM_WS_Response response = new CRM_WS_Response();

            try
            {
                string _connectionString = string.Empty;

                if (IsValidConnectionString(ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString))
                    _connectionString = ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString;

                using (CrmServiceClient conn = new CrmServiceClient(_connectionString))
                {
                    IOrganizationService organizationService = (IOrganizationService)conn.OrganizationServiceProxy;

                    String CPOIDSAPForToken = Param[0].CPOIDSAP;
                    String GenerateToken = _Generator.Encrypt(CPOIDSAPForToken, UniqueKey);

                    if (string.Equals(GenerateToken, Token))
                    {
                        #region Process
                        #region Looping Param
                        EntityCollection ecParam = new EntityCollection();

                        for (int i = 0; i < Param.Length; i++)
                        {
                            QueryExpression _queryexpression = new QueryExpression();
                            ParamIncentiveData _salesorder = Param[i];

                            string CPOIDSAP = null;
                            string ItemNumber = null;
                            decimal? F4 = null;
                            decimal? F5 = null;

                            Guid _salesorderid = Guid.Empty;
                            Guid _incentivef4parameterid = Guid.Empty;
                            Guid _incentivef5parameterid = Guid.Empty;
                            decimal? _f4 = null;
                            decimal? _f5 = null;
                            string _f4name = null;
                            string _f5name = null;

                            if (!string.IsNullOrEmpty(_salesorder.CPOIDSAP))
                            {
                                CPOIDSAP = _salesorder.CPOIDSAP;
                            }
                            else
                                throw new Exception("Parameter CPO ID SAP is Empty!");

                            if (!string.IsNullOrEmpty(_salesorder.ItemNumber))
                            {
                                ItemNumber = _salesorder.ItemNumber;
                            }
                            else
                                throw new Exception("Parameter Item Number is Empty!");

                            if (!string.IsNullOrEmpty(_salesorder.F4))
                            {
                                try
                                {
                                    F4 = decimal.Parse( _salesorder.F4);
                                }
                                catch (Exception)
                                {
                                    throw new Exception("F4 is wrong! Use INTEGER/DECIMAL value !");
                                }

                                _queryexpression = new QueryExpression(_EntityName_IncentiveF4Parameter);
                                _queryexpression.ColumnSet = new ColumnSet(true);
                                _queryexpression.Criteria.AddCondition("new_start", ConditionOperator.LessEqual, F4);
                                _queryexpression.Criteria.AddCondition("new_end", ConditionOperator.GreaterEqual, F4);
                                EntityCollection _incentivef4parameters = organizationService.RetrieveMultiple(_queryexpression);

                                if (_incentivef4parameters.Entities.Count() > 0)
                                {
                                    _incentivef4parameterid = _incentivef4parameters.Entities.FirstOrDefault().Id;
                                    _f4 = _incentivef4parameters.Entities.FirstOrDefault().GetAttributeValue<decimal>("new_koefisien");
                                    _f4name = _incentivef4parameters.Entities.FirstOrDefault().GetAttributeValue<string>("new_name");
                                }
                                else
                                {
                                    throw new Exception("Coeffiecient F4 NOT Found in range !");
                                }
                                
                            }
                            else
                                throw new Exception("Parameter F4 is Empty!");

                            if (!string.IsNullOrEmpty(_salesorder.F5))
                            {
                                try
                                {
                                    F5 = decimal.Parse(_salesorder.F5);
                                }
                                catch (Exception)
                                {
                                    throw new Exception("F5 is wrong! Use INTEGER/DECIMAL value !");
                                }

                                _queryexpression = new QueryExpression(_EntityName_IncentiveF5Parameter);
                                _queryexpression.ColumnSet = new ColumnSet(true);
                                _queryexpression.Criteria.AddCondition("new_start", ConditionOperator.LessEqual, F5);
                                _queryexpression.Criteria.AddCondition("new_end", ConditionOperator.GreaterEqual, F5);
                                EntityCollection _incentivef5parameters = organizationService.RetrieveMultiple(_queryexpression);

                                if (_incentivef5parameters.Entities.Count() > 0)
                                {
                                    _incentivef5parameterid = _incentivef5parameters.Entities.FirstOrDefault().Id;
                                    _f5 = _incentivef5parameters.Entities.FirstOrDefault().GetAttributeValue<decimal>("new_koefisien");
                                    _f5name = _incentivef5parameters.Entities.FirstOrDefault().GetAttributeValue<string>("new_name");
                                }
                                else
                                {
                                    throw new Exception("Coeffiecient F5 NOT Found in range !");
                                }
                                
                            }
                            else
                                throw new Exception("Parameter F5 is Empty!");

                            #region Find CPO Data
                            _queryexpression = new QueryExpression(_EntityName_CPO);
                            _queryexpression.ColumnSet = new ColumnSet(true);
                            _queryexpression.Criteria.AddCondition("new_cpoidsap", ConditionOperator.Equal, CPOIDSAP);
                            Entity _cpo = organizationService.RetrieveMultiple(_queryexpression).Entities.FirstOrDefault();

                            if (_cpo != null)
                            {
                                _queryexpression = new QueryExpression(_EntityName_Incentive);
                                _queryexpression.ColumnSet = new ColumnSet(true);
                                _queryexpression.Criteria.AddCondition("new_cpo", ConditionOperator.Equal, _cpo.Id);
                                _queryexpression.Criteria.AddCondition("new_cpoitemnumber", ConditionOperator.Equal, ItemNumber);
                                Entity _incentive = organizationService.RetrieveMultiple(_queryexpression).Entities.FirstOrDefault();

                                if (_incentive != null)
                                {
                                    Entity _firstmatrixapprover = new Entity(_EntityName_matrixapprovalincentive);
                                    Guid _currentapproverid = new Guid();
                                    int _index = 0;

                                    #region Delete and Create Approval List Incentive
                                    Guid _unitgroupid = _cpo.GetAttributeValue<EntityReference>("new_unitgroup").Id;

                                    _queryexpression = new QueryExpression(_EntityName_approvallistincentive);
                                    _queryexpression.ColumnSet = new ColumnSet(true);
                                    _queryexpression.Criteria.AddCondition("ittn_incentive", ConditionOperator.Equal, _incentive.Id);
                                    EntityCollection _approvallistincentives = organizationService.RetrieveMultiple(_queryexpression);

                                    foreach (var _approvallistincentive in _approvallistincentives.Entities)
                                    {
                                        organizationService.Delete(_EntityName_approvallistincentive, _approvallistincentive.Id);
                                    }

                                    _queryexpression = new QueryExpression(_EntityName_matrixapprovalincentive);
                                    _queryexpression.ColumnSet = new ColumnSet(true);
                                    _queryexpression.Orders.Add(new OrderExpression("ittn_priorityno", OrderType.Ascending));
                                    _queryexpression.Criteria.AddCondition("ittn_unitgroup", ConditionOperator.Equal, _unitgroupid);
                                    EntityCollection _matrixapprovalincentives = organizationService.RetrieveMultiple(_queryexpression);

                                    if (_matrixapprovalincentives.Entities.Count() > 0)
                                    {
                                        foreach (var _matrixapprovalincentive in _matrixapprovalincentives.Entities)
                                        {
                                            Entity _systemuser = organizationService.Retrieve(_EntityName_systemuser, _matrixapprovalincentive.GetAttributeValue<EntityReference>("ittn_approver").Id, new ColumnSet(true));
                                            Entity _approvallistincentive = new Entity(_EntityName_approvallistincentive);

                                            _approvallistincentive["ittn_approver"] = new EntityReference(_EntityName_systemuser, _systemuser.Id);
                                            _approvallistincentive["ittn_incentive"] = new EntityReference(_EntityName_Incentive, _incentive.Id);
                                            _approvallistincentive["ittn_name"] = _systemuser.GetAttributeValue<string>("fullname");

                                            organizationService.Create(_approvallistincentive);

                                            #region Share Record
                                            ShareRecords _sharerecords = new ShareRecords();

                                            _sharerecords.ShareRecord(organizationService, _incentive, _systemuser);
                                            #endregion ---

                                            if (_index == 0)
                                            {
                                                _firstmatrixapprover = _matrixapprovalincentive;
                                                _currentapproverid = _systemuser.Id;
                                            }

                                            _index += 1;
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Approval list in Matrix Approval Incentives NOT Found !");
                                    }
                                    #endregion

                                    #region Update Allocation Data
                                    Entity _incentive_update = new Entity(_EntityName_Incentive);
                                    _incentive_update.Id = _incentive.Id;

                                    _incentive_update["ittn_statusreason"] = new OptionSetValue(841150000); // WAITING APPROVAL INCENTIVE
                                    _incentive_update["new_termofpaymentf4"] = _f4name;
                                    _incentive_update["new_koefisienf4"] = _f4;
                                    _incentive_update["new_paymentconditionoverduef5"] = _f5name;
                                    _incentive_update["new_koefisienf5"] = _f5;
                                    _incentive_update["ittn_currentincentiveapprover"] = new EntityReference(_EntityName_matrixapprovalincentive, _firstmatrixapprover.Id);
                                    _incentive_update["ittn_reqapprovalincentivedate"] = DateTime.Now;
                                    
                                    organizationService.Update(_incentive_update);
                                    #endregion

                                    //#region Send Email
                                    //string _emailsubject = _cpo.GetAttributeValue<string>("name") + " is waiting for approval !";
                                    //string _emailcontent = "Please review and approve.";

                                    //EmailAgent _emailagent = new EmailAgent();
                                    //_emailagent.SendNotification(_firstmatrixapprover.Id, _firstmatrixapprover.Id, Guid.Empty, organizationService, _emailsubject, _emailcontent);
                                    //#endregion ---

                                    response.Result = "Success";
                                    response.ErrorDescription = "Success to Update Incentive Data on CRM CPO. Details : CPO ID SAP : " + CPOIDSAP + ", Item Number : " + ItemNumber;
                                    response.SyncDate = DateTime.Now;
                                    _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator + response.ErrorDescription, MWSLogWebService.LogType.Information, MWSLogWebService.Source.Inbound);
                                }
                                else
                                {
                                    throw new Exception("Cannot found Incentive CPO CRM with CPO ID SAP : " + CPOIDSAP + " and Item Number : " + ItemNumber + " !");
                                }
                            }
                            else
                            {
                                throw new Exception("Cannot found CPO CRM with CPO ID SAP : " + CPOIDSAP + " !");
                            }
                            #endregion
                        }
                        #endregion ---
                        #endregion ---
                    }
                    else
                    {
                        throw new Exception("Access Denied, Invalid Token!");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = "Failed";
                response.ErrorDescription = "Failed to Create Incentive on CRM. Details : " + ex.Message;
                response.SyncDate = DateTime.Now;
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator
                    + "Error : " + ex.Message.ToString(), MWSLogWebService.LogType.Error, MWSLogWebService.Source.Inbound);
            }
            return response;
        }

        public CRM_WS_Response IncentivePayment(string Token, ParamIncentivePaymentData[] Param)
        {
            CRM_WS_Response response = new CRM_WS_Response();

            try
            {
                string _connectionString = string.Empty;

                if (IsValidConnectionString(ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString))
                    _connectionString = ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString;

                using (CrmServiceClient conn = new CrmServiceClient(_connectionString))
                {
                    IOrganizationService organizationService = (IOrganizationService)conn.OrganizationServiceProxy;

                    String CPOIDSAPForToken = Param[0].CPOIDSAP;
                    String GenerateToken = _Generator.Encrypt(CPOIDSAPForToken, UniqueKey);

                    if (string.Equals(GenerateToken, Token))
                    {
                        #region Process
                        #region Looping Param
                        EntityCollection ecParam = new EntityCollection();

                        for (int i = 0; i < Param.Length; i++)
                        {
                            QueryExpression _queryexpression = new QueryExpression();
                            ParamIncentivePaymentData _incentivepayment = Param[i];

                            string CPOIDSAP = null;
                            string ItemNumber = null;
                            string PaymentSettlement = null;
                            decimal Amount = 0;
                            DateTime PaymentDate = DateTime.MinValue;
                            string RVNo = null;

                            if (!string.IsNullOrEmpty(_incentivepayment.CPOIDSAP))
                            {
                                CPOIDSAP = _incentivepayment.CPOIDSAP;
                            }
                            else
                                throw new Exception("Parameter CPO ID SAP is Empty!");

                            if (!string.IsNullOrEmpty(_incentivepayment.ItemNumber))
                            {
                                ItemNumber = _incentivepayment.ItemNumber;
                            }
                            else
                                throw new Exception("Parameter Item Number is Empty!");

                            if (!string.IsNullOrEmpty(_incentivepayment.PaymentSettlement))
                            {
                                PaymentSettlement = _incentivepayment.PaymentSettlement;
                            }
                            else
                                throw new Exception("Parameter Payment Settlement is Empty!");

                            if (!string.IsNullOrEmpty(_incentivepayment.Amount))
                            {
                                Amount = decimal.Parse(_incentivepayment.Amount);
                            }
                            else
                                throw new Exception("Parameter Amount is Empty!");

                            if (!string.IsNullOrEmpty(_incentivepayment.PaymentDate.ToString()))
                            {
                                PaymentDate = _incentivepayment.PaymentDate;
                            }
                            else
                                throw new Exception("Parameter Payment Date is Empty!");

                            if (!string.IsNullOrEmpty(_incentivepayment.RVNo))
                            {
                                RVNo = _incentivepayment.RVNo;
                            }
                            else
                                throw new Exception("Parameter RV No is Empty!");

                            #region Find CPO Data
                            _queryexpression = new QueryExpression(_EntityName_CPO);
                            _queryexpression.ColumnSet = new ColumnSet(true);
                            _queryexpression.Criteria.AddCondition("new_cpoidsap", ConditionOperator.Equal, CPOIDSAP);
                            Entity _cpo = organizationService.RetrieveMultiple(_queryexpression).Entities.FirstOrDefault();

                            if (_cpo != null)
                            {
                                _queryexpression = new QueryExpression(_EntityName_Incentive);
                                _queryexpression.ColumnSet = new ColumnSet(true);
                                _queryexpression.Criteria.AddCondition("new_cpo", ConditionOperator.Equal, _cpo.Id);
                                _queryexpression.Criteria.AddCondition("new_cpoitemnumber", ConditionOperator.Equal, ItemNumber);
                                Entity _incentive = organizationService.RetrieveMultiple(_queryexpression).Entities.FirstOrDefault();

                                if (_incentive != null)
                                {
                                    _queryexpression = new QueryExpression(_EntityName_IncentivePayment);
                                    _queryexpression.ColumnSet = new ColumnSet(true);
                                    _queryexpression.Criteria.AddCondition("ittn_incentive", ConditionOperator.Equal, _incentive.Id);
                                    _queryexpression.Criteria.AddCondition("ittn_paymentsettlement", ConditionOperator.Equal, PaymentSettlement);
                                    Entity _incentivepayments = organizationService.RetrieveMultiple(_queryexpression).Entities.FirstOrDefault();

                                    if (_incentivepayments == null)
                                    {
                                        Entity IncentivePayment = new Entity(_EntityName_IncentivePayment);
                                        IncentivePayment["ittn_incentive"] = new EntityReference(_EntityName_Incentive, _incentive.Id);
                                        IncentivePayment["ittn_paymentsettlement"] = PaymentSettlement;
                                        IncentivePayment["ittn_amount"] = new Money(Amount);
                                        IncentivePayment["ittn_paymentdate"] = PaymentDate;
                                        IncentivePayment["ittn_rvno"] = RVNo;

                                        organizationService.Create(IncentivePayment);

                                        response.Result = "Success";
                                        response.ErrorDescription = "Success to Create Incentive Payment Data on CRM CPO Incentive. Details : CPO ID SAP : " + CPOIDSAP + ", Item Number : " + ItemNumber;
                                        response.SyncDate = DateTime.Now;
                                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator + response.ErrorDescription, MWSLogWebService.LogType.Information, MWSLogWebService.Source.Inbound);
                                    }
                                    else
                                    {
                                        throw new Exception("There is already Payment Settlement with Name : " + PaymentSettlement + " for this incentive !");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Cannot found Incentive CPO CRM with CPO ID SAP : " + CPOIDSAP + " and Item Number : " + ItemNumber + " !");
                                }
                            }
                            else
                            {
                                throw new Exception("Cannot found CPO CRM with CPO ID SAP : " + CPOIDSAP + " !");
                            }
                            #endregion
                        }
                        #endregion ---
                        #endregion ---
                    }
                    else
                    {
                        throw new Exception("Access Denied, Invalid Token!");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = "Failed";
                response.ErrorDescription = "Failed to Create Incentive Payment on CRM. Details : " + ex.Message;
                response.SyncDate = DateTime.Now;
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator
                    + "Error : " + ex.Message.ToString(), MWSLogWebService.LogType.Error, MWSLogWebService.Source.Inbound);
            }
            return response;
        }
        #endregion

        #region Update CPO Number
        public CRM_WS_Response UpdateCPOIDSAP(string Token, ParamUpdateCPOData[] Param)
        {
            CRM_WS_Response response = new CRM_WS_Response();

            try
            {
                string _connectionString = string.Empty;

                if (IsValidConnectionString(ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString))
                    _connectionString = ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString;

                using (CrmServiceClient conn = new CrmServiceClient(_connectionString))
                {
                    IOrganizationService organizationService = (IOrganizationService)conn.OrganizationServiceProxy;

                    String CPOIDSAP = Param[0].CPOIDSAP;
                    String GenerateToken = _Generator.Encrypt(CPOIDSAP, UniqueKey);
                    if (string.Equals(GenerateToken, Token))
                    {
                        #region Process
                        QueryExpression qeCPOheader = new QueryExpression(_EntityName_CPO);
                        qeCPOheader.ColumnSet = new ColumnSet(true);
                        qeCPOheader.Criteria.AddCondition("new_cpoidsap", ConditionOperator.Equal, CPOIDSAP);
                        Entity ecCPO = organizationService.RetrieveMultiple(qeCPOheader).Entities.FirstOrDefault();

                        if (ecCPO != null)
                        {
                            #region Update CPO ID SAP Data 
                            Entity CPOUpdate = new Entity(_EntityName_CPO);
                            CPOUpdate["new_cpoidsap"] = Param[0].CPOIDSAPUpdateTo;
                            CPOUpdate.Id = ecCPO.Id;
                            organizationService.Update(CPOUpdate);
                            #endregion

                            response.Result = "Success";
                            response.ErrorDescription = "Success to Update CPO ID SAP Data on CRM CPO. Details : CPO ID SAP : " + CPOIDSAP;
                            response.SyncDate = DateTime.Now;
                            _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator + response.ErrorDescription, MWSLogWebService.LogType.Information, MWSLogWebService.Source.Inbound);
                        }
                        else
                        {
                            throw new Exception("Cannot found CPO CRM with CPO ID SAP : " + CPOIDSAP + " !");
                        }
                        #endregion
                    }
                    else
                    {
                        throw new Exception("Access Denied, Invalid Token!");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = "Failed";
                response.ErrorDescription = "Failed to Update CPO ID SAP on CRM. Details : " + ex.Message;
                response.SyncDate = DateTime.Now;
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator
                    + "Error : " + ex.Message.ToString(), MWSLogWebService.LogType.Error, MWSLogWebService.Source.Inbound);
            }
            return response;
        }
        #endregion

        #endregion
    }
}
