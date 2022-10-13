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
using Microsoft.Xrm.Tooling.Connector;
using TrakNusSparepartSystem.WcfService;
using System.Security.Cryptography;
using System.Configuration;
using TrakNusSparepartSystem.WcfService.Helper;

namespace TrakNusSparepartSystemWcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service : IService
    {
        #region Constants
        private string uniqueKey = ConfigurationManager.AppSettings["UniqueKey"].ToString();
        private string Phrase = ConfigurationManager.AppSettings["Phrase"].ToString();
        private const int SOURCETYPE_SERVICE = 865920002;
        private const int PartInterchange_STATUS_ACTIVE = 865920000;
        private const int PartInterchange_STATUS_DEACTIVE = 865920001;

        private const int SO_STATECODE_CANCEL = 865920002;
        private const int SO_STATECODE_DELIVERED = 865920003;
        private const int SO_STATECODE_INVOICED = 865920004;
        private const int SO_STATECODE_PAID = 865920005;
        private const int SO_STATUSREASON_CLOSED = 865920002;

        private const int SOLINE_STATUS_CANCEL = 865920001;
        private const int SOLINE_STATUS_DELIVERED = 865920002;
        private const int SOLINE_STATUS_INVOICED = 865920003;
        private const int SOLINE_STATUS_PAID = 865920004;

        private const int SUBLINE_TRANSCTTYPE_CREATEDO = 865920000;
        private const int SUBLINE_TRANSCTTYPE_UPDATEDR = 865920001;
        private const int SUBLINE_TRANSCTTYPE_CREATEINV = 865920002;
        private const int SUBLINE_TRANSCTTYPE_UPDATEINV = 865920003;
        private const int SUBLINE_TRANSCTTYPE_CREATETO = 865920004;
        private const int SUBLINE_TRANSCTTYPE_CONFIRMTO = 865920005;
        private const int SUBLINE_TRANSCTTYPE_CREATEPYMNT = 865920006;
        private const int SUBLINE_TRANSCTTYPE_UPDATEZPART = 865920007;

        private const int QUOTE_STATUSCODE_CLOSED = 865920004;
        private const int QUOTE_STATUSREASON_LOST = 865920007;

        private const int PROSPECT_STATUSREASON_LOST = 865920002;

        private const double HOURS = 0;
        #endregion

        #region Dependencies
        private MWSLog _mwsLog = new MWSLog();
        #endregion

        #region Privates
        private string Encrypt(string text, string uniqueKey)
        {
            try
            {
                UTF8Encoding e = new UTF8Encoding();
                byte[] keyByte = e.GetBytes(uniqueKey);
                byte[] passByte = e.GetBytes(text);
                HMACSHA256 hash = new HMACSHA256(keyByte);
                string hashedPwd = byteArrayToString(hash.ComputeHash(passByte));
                return hashedPwd;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodBase.GetCurrentMethod().Name + " : " + ex.Message.ToString());
            }
        }

        private static string byteArrayToString(byte[] hash)
        {
            String result = "";
            for (int i = 0; i < hash.Length; i++)
            {
                result += hash[i].ToString("X2");
            }
            return (result);
        }

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
                string EncryptedText = Encrypt(text, uniqueKey);

                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, EncryptedText, MWSLog.LogType.Information, MWSLog.Source.Inbound);
                syncronizedResult.Success = true;

                return string.Format("Encrypted Text : {0}", EncryptedText);
            }
            catch (Exception ex)
            {
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, text + _mwsLog.ColumnsSeparator
                     + "Error : " + ex.Message.ToString(), MWSLog.LogType.Error, MWSLog.Source.Inbound);
                syncronizedResult.Success = false;
                syncronizedResult.ErrorMessage = ex.Message.ToString();
            }
            return syncronizedResult.ErrorMessage;
        }
        #endregion

        #region Delivery Order
        /* Description : Used for Create Manual DO From SAP */
        public CRM_WS_Response CreateDeliveryandSOSubline(string token, ParamCreateSOsubline[] sublines)
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

                    string Delivery_NO = sublines[0].Delivery_NO;
                    string generateToken = Encrypt(Delivery_NO, uniqueKey);
                    if (generateToken == token)
                    {
                        EntityCollection createSOsubLines = new EntityCollection();

                        #region looping sublines params
                        for (int x = 0; x < sublines.Length; x++)
                        {
                            ParamCreateSOsubline subline = sublines[x];
                            if (!string.IsNullOrEmpty(subline.Delivery_NO) && !string.IsNullOrWhiteSpace(subline.SO_NO_SAP) && !string.IsNullOrWhiteSpace(subline.PartNumber) && !string.IsNullOrWhiteSpace(subline.PartBranch))
                            {
                                string SO_NO_SAP = subline.SO_NO_SAP;
                                string PartNumber = subline.PartNumber;
                                string PartBranch = subline.PartBranch;
                                int DeliveryQty = subline.Delivery_Qty;

                                QueryExpression queryGetPart = new QueryExpression("trs_masterpart");
                                queryGetPart.ColumnSet = new ColumnSet(true);
                                queryGetPart.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("trs_name", ConditionOperator.Equal, PartNumber),
                                    }
                                };
                                Entity part = organizationService.RetrieveMultiple(queryGetPart).Entities.FirstOrDefault();

                                QueryExpression queryGetBranch = new QueryExpression("businessunit");
                                queryGetBranch.ColumnSet = new ColumnSet(true);
                                queryGetBranch.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("trs_branchcode", ConditionOperator.Equal, PartBranch),
                                        new ConditionExpression("name", ConditionOperator.Equal, PartBranch)
                                    }
                                };
                                Entity Branch = organizationService.RetrieveMultiple(queryGetBranch).Entities.FirstOrDefault();

                                QueryExpression queryGetSO = new QueryExpression("tss_sopartheader");
                                queryGetSO.ColumnSet = new ColumnSet(true);
                                queryGetSO.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("tss_sapsoid", ConditionOperator.Equal, SO_NO_SAP)
                                    }
                                };
                                Entity SO_Header = organizationService.RetrieveMultiple(queryGetSO).Entities.FirstOrDefault();

                                if (part == null)
                                    throw new Exception("Partnumber not found in part master!");
                                if (Branch == null)
                                    throw new Exception("Branch not found in master branch!");
                                if (SO_Header == null)
                                    throw new Exception("SO Part with current SAP SO Number not found in master branch!");

                                QueryExpression queryGetSOLine = new QueryExpression("tss_sopartlines");
                                queryGetSOLine.ColumnSet = new ColumnSet(true);
                                queryGetSOLine.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.Or,
                                    Conditions =
                                    {
                                        new ConditionExpression("tss_sopartheaderid", ConditionOperator.Equal, SO_Header.Id)
                                        //new ConditionExpression("tss_partnumber", ConditionOperator.Equal, part.Id)
                                    }
                                };

                                EntityCollection SO_Lines = organizationService.RetrieveMultiple(queryGetSOLine);

                                QueryExpression queryGetPartlinesInter = new QueryExpression("tss_partmasterlinesinterchange");
                                queryGetPartlinesInter.ColumnSet = new ColumnSet(true);
                                queryGetPartlinesInter.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("tss_partnumberinterchange", ConditionOperator.Equal, part.Id),
                                        //new ConditionExpression("tss_status", ConditionOperator.Equal, PartInterchange_STATUS_ACTIVE)//status = active
                                    }
                                };
                                EntityCollection PartlineInters = organizationService.RetrieveMultiple(queryGetPartlinesInter);
                                List<Entity> SOLines = new List<Entity>();
                                Entity SO_Line = new Entity();

                                if (PartlineInters.Entities.Count > 0)
                                {
                                    SOLines = SO_Lines.Entities.Where(o => o.GetAttributeValue<bool>("tss_isinterchange") == true && o.GetAttributeValue<EntityReference>("tss_partnumberinterchange").Id == PartlineInters.Entities[0].Id).ToList();
                                    SO_Line = SOLines.FirstOrDefault();
                                    if (SO_Line == null)
                                        SO_Line = SO_Lines.Entities.Where(o => o.GetAttributeValue<EntityReference>("tss_partnumber").Id == part.Id).FirstOrDefault();
                                }
                                else
                                {
                                    SOLines = SO_Lines.Entities.Where(o => o.GetAttributeValue<EntityReference>("tss_partnumber").Id == part.Id).ToList();
                                    SO_Line = SOLines.FirstOrDefault();
                                }

                                if (SO_Line != null)
                                {
                                    #region create SO subline
                                    Entity SO_Subline = new Entity("tss_salesorderpartsublines");
                                    //Must inputted
                                    SO_Subline["tss_salesorderpartlines"] = SO_Line.ToEntityReference();
                                    SO_Subline["tss_qtyavailablebranch"] = Branch.ToEntityReference();
                                    SO_Subline["tss_name"] = PartNumber;
                                    SO_Subline["tss_deliveryno"] = subline.Delivery_NO;
                                    SO_Subline["tss_transactiontype"] = new OptionSetValue(SUBLINE_TRANSCTTYPE_CREATEDO);
                                    //Int
                                    SO_Subline["tss_deliveryqty"] = DeliveryQty;
                                    //SO_Subline["tss_qtyavailable"] = QtyAvail;
                                    //SO_Subline["tss_qtyindent"] = QtyInden;

                                    //DateTime
                                    //if (subline.Expedition_Pick_Up != DateTime.MinValue) { SO_Subline["tss_expeditionpickupdate"] = subline.Expedition_Pick_Up; }
                                    //if (subline.ETA_JKT != DateTime.MinValue) { SO_Subline["tss_etajakarta"] = subline.ETA_JKT; }
                                    //if (subline.IBTS_DATE != DateTime.MinValue) { SO_Subline["tss_ibtsdate"] = subline.IBTS_DATE; }
                                    //if (subline.PO_DATE_PRIN != DateTime.MinValue) { SO_Subline["tss_podateprincipal"] = subline.PO_DATE_PRIN; }
                                    //if (subline.RSV_DATE != DateTime.MinValue) { SO_Subline["tss_reservationdate"] = subline.RSV_DATE; }
                                    //if (subline.WHS_BRANCH_DATE != DateTime.MinValue) { SO_Subline["tss_branchdate"] = subline.WHS_BRANCH_DATE; }
                                    //if (subline.WRS_DATE != DateTime.MinValue) { SO_Subline["tss_wrsdate"] = subline.WRS_DATE; }
                                    //String
                                    //if (subline.IBTS_NO != null) { SO_Subline["tss_ibtsno"] = subline.IBTS_NO; }
                                    //if (subline.IBTS_WHS_NO != null) { SO_Subline["tss_ibtswhsno"] = subline.IBTS_WHS_NO; }
                                    //if (subline.PO_NO_PRIN != null) { SO_Subline["tss_ponumberprincipal"] = subline.PO_NO_PRIN; }
                                    //if (subline.RSV_NO != null) { SO_Subline["tss_reservationno"] = subline.RSV_NO; }
                                    //if (subline.WHS_BRANCH != null) { SO_Subline["tss_wrsbranch"] = subline.WHS_BRANCH; }

                                    createSOsubLines.Entities.Add(SO_Subline);
                                    #endregion
                                }
                                else
                                {
                                    throw new Exception("SO line not found!");
                                }
                            }
                            else
                            {
                                throw new Exception("Paremeter Delivery Number / SO Number SAP / Part Number / Part Branch is Empty!");
                            }
                        }
                        #endregion
                        #region create SOsubLines to CRM
                        //create so part subline on crm.
                        if (createSOsubLines.Entities.Count > 0)
                        {
                            foreach (var sub in createSOsubLines.Entities)
                            {
                                organizationService.Create(sub);
                            }
                        }

                        response.Result = "Success";
                        response.ErrorDescription = "Success to Create Delivery Order on Sales Order Sub Lines on CRM.";
                        response.SyncDate = DateTime.Now;
                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, Delivery_NO + _mwsLog.ColumnsSeparator
                             + response.Result, MWSLog.LogType.Information, MWSLog.Source.Inbound);
                        #endregion
                    }
                    else
                    {
                        throw new Exception("Access denied invalid token!");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = "Failed";
                response.ErrorDescription = "Failed to Create Delivery Order on Sales Order Sub Lines on CRM. Details : " + ex.Message;
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator
                                 + "Error : " + ex.Message.ToString(), MWSLog.LogType.Error, MWSLog.Source.Inbound);
            }
            return response;
        }

        /* Description : Used for Update DO From SAP (From ZPART Report) */
        public CRM_WS_Response UpdateDeliveryOrder(string token, ParamDeliveryOrder[] sublines)
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

                    string Primary_DO = sublines[0].Delivery_NO;
                    string generateToken = Encrypt(Primary_DO, uniqueKey);
                    if (generateToken == token)
                    {
                        EntityCollection updateSOsubLines = new EntityCollection();
                        EntityCollection updateSOLines = new EntityCollection();
                        EntityCollection updateSOParts = new EntityCollection();

                        #region looping sublines params
                        for (int x = 0; x < sublines.Length; x++)
                        {
                            ParamDeliveryOrder subline = sublines[x];
                            if (!string.IsNullOrWhiteSpace(subline.SO_NO_SAP) && !string.IsNullOrWhiteSpace(subline.PartNumber))
                            {
                                string SO_NO_SAP = subline.SO_NO_SAP;
                                string PartNumber = subline.PartNumber;
                                string PartBranch = subline.PartBranch;
                                string DeliveryNumber = subline.Delivery_NO;
                                int DeliveryQty = subline.Delivery_Qty;

                                QueryExpression queryGetPart = new QueryExpression("trs_masterpart");
                                queryGetPart.ColumnSet = new ColumnSet(true);
                                queryGetPart.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("trs_name", ConditionOperator.Equal, PartNumber),
                                    }
                                };
                                Entity part = organizationService.RetrieveMultiple(queryGetPart).Entities.FirstOrDefault();

                                QueryExpression queryGetBranch = new QueryExpression("businessunit");
                                queryGetBranch.ColumnSet = new ColumnSet(true);
                                queryGetBranch.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("trs_branchcode", ConditionOperator.Equal, PartBranch),
                                        new ConditionExpression("name", ConditionOperator.Equal, PartBranch)
                                    }
                                };
                                Entity Branch = organizationService.RetrieveMultiple(queryGetBranch).Entities.FirstOrDefault();

                                QueryExpression queryGetSO = new QueryExpression("tss_sopartheader");
                                queryGetSO.ColumnSet = new ColumnSet(true);
                                queryGetSO.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("tss_sapsoid", ConditionOperator.Equal, SO_NO_SAP)
                                    }
                                };
                                Entity SO_Header = organizationService.RetrieveMultiple(queryGetSO).Entities.FirstOrDefault();

                                if (part == null)
                                    throw new Exception("Partnumber not found in part master!");
                                if (Branch == null)
                                    throw new Exception("Branch not found in master branch!");
                                if (SO_Header == null)
                                    throw new Exception("SO Part with current SAP SO Number not found in master branch!");

                                QueryExpression queryGetSOLine = new QueryExpression("tss_sopartlines");
                                queryGetSOLine.ColumnSet = new ColumnSet(true);
                                queryGetSOLine.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.Or,
                                    Conditions =
                                    {
                                        new ConditionExpression("tss_sopartheaderid", ConditionOperator.Equal, SO_Header.Id)
                                        //new ConditionExpression("tss_partnumber", ConditionOperator.Equal, part.Id)
                                    }
                                };

                                EntityCollection SO_Lines = organizationService.RetrieveMultiple(queryGetSOLine);

                                QueryExpression queryGetPartlinesInter = new QueryExpression("tss_partmasterlinesinterchange");
                                queryGetPartlinesInter.ColumnSet = new ColumnSet(true);
                                queryGetPartlinesInter.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("tss_partnumberinterchange", ConditionOperator.Equal, part.Id),
                                        //new ConditionExpression("tss_status", ConditionOperator.Equal, PartInterchange_STATUS_ACTIVE)//status = active
                                    }
                                };
                                EntityCollection PartlineInters = organizationService.RetrieveMultiple(queryGetPartlinesInter);
                                List<Entity> SOLines = new List<Entity>();
                                Entity SO_Line = new Entity();

                                if (PartlineInters.Entities.Count > 0)
                                {
                                    SOLines = SO_Lines.Entities.Where(o => o.GetAttributeValue<bool>("tss_isinterchange") == true && o.GetAttributeValue<EntityReference>("tss_partnumberinterchange").Id == PartlineInters.Entities[0].Id).ToList();
                                    SO_Line = SOLines.FirstOrDefault();
                                    if (SO_Line == null)
                                        SO_Line = SO_Lines.Entities.Where(o => o.GetAttributeValue<EntityReference>("tss_partnumber").Id == part.Id).FirstOrDefault();
                                }
                                else
                                {
                                    SOLines = SO_Lines.Entities.Where(o => o.GetAttributeValue<EntityReference>("tss_partnumber").Id == part.Id).ToList();
                                    SO_Line = SOLines.FirstOrDefault();
                                }

                                if (SO_Line != null)
                                {
                                    QueryExpression queryGetSOSubLine = new QueryExpression("tss_salesorderpartsublines");
                                    queryGetSOSubLine.ColumnSet = new ColumnSet(true);
                                    queryGetSOSubLine.Criteria = new FilterExpression
                                    {
                                        FilterOperator = LogicalOperator.And,
                                        Conditions =
                                        {
                                            new ConditionExpression("tss_name", ConditionOperator.Equal, PartNumber),
                                            new ConditionExpression("tss_salesorderpartlines", ConditionOperator.Equal, SO_Line.Id)
                                        }
                                    };

                                    EntityCollection SO_Sublines = organizationService.RetrieveMultiple(queryGetSOSubLine);

                                    //if get exception it means so subline not exist, then create so sublines.
                                    try
                                    {
                                        //new ConditionExpression("tss_transactiontype", ConditionOperator.Equal, SUBLINE_TRANSCTTYPE_CREATEDO)

                                        #region update SO subline
                                        Entity SO_Subline = SO_Sublines.Entities.Where(o => o.GetAttributeValue<string>("tss_deliveryno") == DeliveryNumber && o.GetAttributeValue<EntityReference>("tss_qtyavailablebranch").Id == Branch.Id && o.GetAttributeValue<OptionSetValue>("tss_transactiontype").Value == SUBLINE_TRANSCTTYPE_CREATEDO).First();
                                        List<Entity> items = SO_Sublines.Entities.ToList();
                                        int totaldeliveryQty = DeliveryQty;
                                        //Int
                                        SO_Subline["tss_deliveryqty"] = DeliveryQty;
                                        SO_Subline["tss_transactiontype"] = new OptionSetValue(SUBLINE_TRANSCTTYPE_UPDATEDR);
                                        //DateTime
                                        if (subline.Exp_Pick_Up != DateTime.MinValue) { SO_Subline["tss_expeditionpickupdate"] = ConvertDateTime(subline.Exp_Pick_Up); }
                                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, subline.Exp_Pick_Up.ToString(), MWSLog.LogType.Information, MWSLog.Source.Inbound);
                                        if (subline.Cust_Receipt_Date != DateTime.MinValue && !SO_Subline.Contains("tss_customerreceiptdate")) { SO_Subline["tss_customerreceiptdate"] = ConvertDateTime(subline.Cust_Receipt_Date); }
                                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, subline.Cust_Receipt_Date.ToString(), MWSLog.LogType.Information, MWSLog.Source.Inbound);

                                        updateSOsubLines.Entities.Add(SO_Subline);
                                        int qtyReq = SO_Line.GetAttributeValue<int>("tss_qtyrequest");

                                        foreach (var item in items)
                                        {
                                            if (item.Contains("tss_deliveryqty"))
                                                totaldeliveryQty += item.GetAttributeValue<int>("tss_deliveryqty");
                                        }

                                        //Jika Delivery Qty di SO Sub Lines = Qty Request di SO Lines, status jadi delivered
                                        if (totaldeliveryQty == qtyReq)
                                        {
                                            SO_Line["tss_status"] = new OptionSetValue(SOLINE_STATUS_DELIVERED);

                                            if (updateSOLines.Entities.Count > 0)
                                            {
                                                bool isexist = updateSOLines.Entities.Any(a => a.Id == SO_Line.Id);
                                                if (!isexist)
                                                    updateSOLines.Entities.Add(SO_Line);
                                            }
                                            else
                                            {
                                                updateSOLines.Entities.Add(SO_Line);
                                            }

                                            if (updateSOParts.Entities.Count > 0)
                                            {
                                                bool isexist = updateSOParts.Entities.Any(a => a.Id == SO_Header.Id);
                                                if (!isexist)
                                                    updateSOParts.Entities.Add(SO_Header);
                                            }
                                            else
                                            {
                                                updateSOParts.Entities.Add(SO_Header);
                                            }
                                        }
                                        #endregion
                                    }
                                    catch
                                    {
                                        throw new Exception("SO subline not found!");
                                    }
                                }
                                else
                                {
                                    throw new Exception("SO line not found!");
                                }
                            }
                            else
                            {
                                throw new Exception("Paremeter SO Number SAP / Part Number is Empty!");
                            }
                        }
                        #endregion
                        #region update Delivery Order in SO subline to CRM
                        //check if there is any update so sub.
                        if (updateSOsubLines.Entities.Count > 0)
                        {
                            foreach (var sub in updateSOsubLines.Entities)
                            {
                                organizationService.Update(sub);
                            }
                        }
                        if (updateSOLines.Entities.Count > 0)
                        {
                            foreach (var updateSOLine in updateSOLines.Entities)
                            {
                                organizationService.Update(updateSOLine);
                            }
                        }
                        if (updateSOParts.Entities.Count > 0)
                        {
                            foreach (var updateSOPart in updateSOParts.Entities)
                            {
                                QueryExpression queryGetSOLine = new QueryExpression("tss_sopartlines");
                                queryGetSOLine.ColumnSet = new ColumnSet(true);
                                queryGetSOLine.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("tss_sopartheaderid", ConditionOperator.Equal, updateSOPart.Id)
                                    }
                                };
                                EntityCollection SO_Lines = organizationService.RetrieveMultiple(queryGetSOLine);
                                int Linesrecord = SO_Lines.Entities.Count;
                                var Linesparametered = from c in SO_Lines.Entities
                                                       where c.GetAttributeValue<OptionSetValue>("tss_status").Value == SOLINE_STATUS_DELIVERED
                                                       || c.GetAttributeValue<OptionSetValue>("tss_status").Value == SOLINE_STATUS_INVOICED
                                                       || c.GetAttributeValue<OptionSetValue>("tss_status").Value == SOLINE_STATUS_PAID
                                                       select c;
                                if (Linesrecord == Linesparametered.Count() && Linesparametered.Any(x => x.GetAttributeValue<OptionSetValue>("tss_status").Value == SOLINE_STATUS_DELIVERED))
                                {
                                    updateSOPart["tss_statecode"] = new OptionSetValue(SO_STATECODE_DELIVERED);
                                    organizationService.Update(updateSOPart);
                                }
                            }
                        }

                        response.Result = "Success";
                        response.ErrorDescription = "Success to Create Delivery Order on Sales Order Sub Lines on CRM.";
                        response.SyncDate = DateTime.Now;
                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, Primary_DO + _mwsLog.ColumnsSeparator
                             + response.Result, MWSLog.LogType.Information, MWSLog.Source.Inbound);
                        #endregion
                    }
                    else
                    {
                        throw new Exception("Access denied invalid token!");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = "Failed";
                response.ErrorDescription = "Failed to Create Delivery Order on Sales Order Sub Lines on CRM. Details : " + ex.Message;
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator
                                 + "Error : " + ex.Message.ToString(), MWSLog.LogType.Error, MWSLog.Source.Inbound);
            }
            return response;
        }
        #endregion

        #region Sales Order
        public CRM_WS_Response UpdateSOSubline(string token, ParamSOsubline[] sublines)
        {   //parameter so subline ditambahkan branch code.
            CRM_WS_Response response = new CRM_WS_Response();
            try
            {
                string _connectionString = string.Empty;

                if (IsValidConnectionString(ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString))
                    _connectionString = ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString;

                using (CrmServiceClient conn = new CrmServiceClient(_connectionString))
                {
                    IOrganizationService organizationService = (IOrganizationService)conn.OrganizationServiceProxy;

                    string Primary_SO_NO_SAP = sublines[0].SO_NO_SAP;
                    string generateToken = Encrypt(Primary_SO_NO_SAP, uniqueKey);
                    if (generateToken == token)
                    {
                        EntityCollection updateSOsubLines = new EntityCollection();
                        EntityCollection createSOsubLines = new EntityCollection();

                        #region looping sublines params
                        for (int x = 0; x < sublines.Length; x++)
                        {
                            ParamSOsubline subline = sublines[x];
                            if (!string.IsNullOrWhiteSpace(subline.SO_NO_SAP) && !string.IsNullOrWhiteSpace(subline.PartNumber) && !string.IsNullOrWhiteSpace(subline.PartBranch))
                            {
                                #region Contoh validasi
                                //if (subline.ETA_JKT != DateTime.MinValue) { details = details + " " + subline.ETA_JKT.ToShortDateString(); }
                                //if (subline.IBTS_DATE != DateTime.MinValue) { details = details + " " + subline.IBTS_DATE.ToShortDateString(); }
                                //if (subline.PO_DATE_PRIN != DateTime.MinValue) { details = details + " " + subline.PO_DATE_PRIN.ToShortDateString(); }
                                //if (subline.RSV_DATE != DateTime.MinValue) { details = details + " " + subline.RSV_DATE.ToShortDateString(); }
                                //if (subline.WHS_BRANCH_DATE != DateTime.MinValue) { details = details + " " + subline.WHS_BRANCH_DATE.ToShortDateString(); }
                                //if (subline.WRS_DATE != DateTime.MinValue) { details = details + " " + subline.WRS_DATE.ToShortDateString(); }

                                //if (subline.QtyAvail != int.MinValue) { details = details + " " + subline.QtyAvail.ToString(); }
                                //if (subline.QtyInden != int.MinValue) { details = details + " " + subline.QtyInden.ToString(); }

                                //if (subline.IBTS_NO != null) { details = details + " " + subline.IBTS_NO; }
                                //if (subline.IBTS_WHS_NO != null) { details = details + " " + subline.IBTS_WHS_NO; }
                                //if (subline.PO_NO_PRIN != null) { details = details + " " + subline.PO_NO_PRIN; }
                                //if (subline.RSV_NO != null) { details = details + " " + subline.RSV_NO; }
                                //if (subline.WHS_BRANCH != null) { details = details + " " + subline.WHS_BRANCH; }
                                #endregion

                                string SO_NO_SAP = subline.SO_NO_SAP;
                                string PartNumber = subline.PartNumber;
                                string PartBranch = subline.PartBranch;
                                int QtyAvail = subline.QtyAvail;
                                int QtyInden = subline.QtyInden;

                                QueryExpression queryGetPart = new QueryExpression("trs_masterpart");
                                queryGetPart.ColumnSet = new ColumnSet(true);
                                queryGetPart.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("trs_name", ConditionOperator.Equal, PartNumber),
                                    }
                                };
                                Entity part = organizationService.RetrieveMultiple(queryGetPart).Entities.FirstOrDefault();

                                QueryExpression queryGetBranch = new QueryExpression("businessunit");
                                queryGetBranch.ColumnSet = new ColumnSet(true);
                                queryGetBranch.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("trs_branchcode", ConditionOperator.Equal, PartBranch),
                                        new ConditionExpression("name", ConditionOperator.Equal, PartBranch)
                                    }
                                };
                                Entity Branch = organizationService.RetrieveMultiple(queryGetBranch).Entities.FirstOrDefault();

                                QueryExpression queryGetSO = new QueryExpression("tss_sopartheader");
                                queryGetSO.ColumnSet = new ColumnSet(true);
                                queryGetSO.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("tss_sapsoid", ConditionOperator.Equal, SO_NO_SAP)
                                    }
                                };
                                Entity SO_Header = organizationService.RetrieveMultiple(queryGetSO).Entities.FirstOrDefault();

                                if (part == null)
                                    throw new Exception("Partnumber not found in part master!");
                                if (Branch == null)
                                    throw new Exception("Branch not found in master branch!");
                                if (SO_Header == null)
                                    throw new Exception("SO Part with current SAP SO Number not found in master branch!");

                                QueryExpression queryGetSOLine = new QueryExpression("tss_sopartlines");
                                queryGetSOLine.ColumnSet = new ColumnSet(true);
                                queryGetSOLine.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.Or,
                                    Conditions =
                                    {
                                        new ConditionExpression("tss_sopartheaderid", ConditionOperator.Equal, SO_Header.Id)
                                        //new ConditionExpression("tss_partnumber", ConditionOperator.Equal, part.Id)
                                    }
                                };

                                EntityCollection SO_Lines = organizationService.RetrieveMultiple(queryGetSOLine);

                                QueryExpression queryGetPartlinesInter = new QueryExpression("tss_partmasterlinesinterchange");
                                queryGetPartlinesInter.ColumnSet = new ColumnSet(true);
                                queryGetPartlinesInter.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("tss_partnumberinterchange", ConditionOperator.Equal, part.Id),
                                        //new ConditionExpression("tss_status", ConditionOperator.Equal, PartInterchange_STATUS_ACTIVE)//status = active
                                    }
                                };
                                EntityCollection PartlineInters = organizationService.RetrieveMultiple(queryGetPartlinesInter);
                                List<Entity> SOLines = new List<Entity>();
                                Entity SO_Line = new Entity();

                                if (PartlineInters.Entities.Count > 0)
                                {
                                    SOLines = SO_Lines.Entities.Where(o => o.GetAttributeValue<bool>("tss_isinterchange") == true && o.GetAttributeValue<EntityReference>("tss_partnumberinterchange").Id == PartlineInters.Entities[0].Id).ToList();
                                    SO_Line = SOLines.FirstOrDefault();
                                    if (SO_Line == null)
                                        SO_Line = SO_Lines.Entities.Where(o => o.GetAttributeValue<EntityReference>("tss_partnumber").Id == part.Id).FirstOrDefault();
                                }
                                else
                                {
                                    SOLines = SO_Lines.Entities.Where(o => o.GetAttributeValue<EntityReference>("tss_partnumber").Id == part.Id).ToList();
                                    SO_Line = SOLines.FirstOrDefault();
                                }

                                if (SO_Line != null)
                                {
                                    QueryExpression queryGetSOSubLine = new QueryExpression("tss_salesorderpartsublines");
                                    queryGetSOSubLine.ColumnSet = new ColumnSet(true);
                                    queryGetSOSubLine.Criteria = new FilterExpression
                                    {
                                        FilterOperator = LogicalOperator.And,
                                        Conditions =
                                            {
                                                new ConditionExpression("tss_name", ConditionOperator.Equal, PartNumber),
                                                new ConditionExpression("tss_salesorderpartlines", ConditionOperator.Equal, SO_Line.Id),
                                                new ConditionExpression("tss_transactiontype", ConditionOperator.Equal, SUBLINE_TRANSCTTYPE_UPDATEZPART),
                                                //harusnya tambah parameter branch codenya condition + qtyavailablebranch sesuai dengan branch partnya.
                                                new ConditionExpression("tss_qtyavailablebranch", ConditionOperator.Equal, Branch.Id)
                                            }
                                    };

                                    EntityCollection SO_Sublines = organizationService.RetrieveMultiple(queryGetSOSubLine);

                                    //if get exception it means so subline not exist, then create so sublines.
                                    if (SO_Sublines.Entities.Count > 0)
                                    {
                                        #region update SO subline
                                        Entity SO_Subline = SO_Sublines.Entities.First();

                                        //DateTime
                                        if (subline.ETA_JKT != DateTime.MinValue) { SO_Subline["tss_etajakarta"] = ConvertDateTime(subline.ETA_JKT); }
                                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, subline.ETA_JKT.ToString(), MWSLog.LogType.Information, MWSLog.Source.Inbound);
                                        if (subline.IBTS_DATE != DateTime.MinValue) { SO_Subline["tss_ibtsdate"] = ConvertDateTime(subline.IBTS_DATE); }
                                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, subline.IBTS_DATE.ToString(), MWSLog.LogType.Information, MWSLog.Source.Inbound);
                                        if (subline.PO_DATE_PRIN != DateTime.MinValue) { SO_Subline["tss_podateprincipal"] = ConvertDateTime(subline.PO_DATE_PRIN); }
                                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, subline.PO_DATE_PRIN.ToString(), MWSLog.LogType.Information, MWSLog.Source.Inbound);
                                        if (subline.RSV_DATE != DateTime.MinValue) { SO_Subline["tss_reservationdate"] = ConvertDateTime(subline.RSV_DATE); }
                                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, subline.RSV_DATE.ToString(), MWSLog.LogType.Information, MWSLog.Source.Inbound);
                                        if (subline.WHS_BRANCH_DATE != DateTime.MinValue) { SO_Subline["tss_branchdate"] = ConvertDateTime(subline.WHS_BRANCH_DATE); }
                                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, subline.WHS_BRANCH_DATE.ToString(), MWSLog.LogType.Information, MWSLog.Source.Inbound);
                                        if (subline.WRS_DATE != DateTime.MinValue) { SO_Subline["tss_wrsdate"] = ConvertDateTime(subline.WRS_DATE); }
                                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, subline.WRS_DATE.ToString(), MWSLog.LogType.Information, MWSLog.Source.Inbound);
                                        //String
                                        if (subline.IBTS_NO != null) { SO_Subline["tss_ibtsno"] = subline.IBTS_NO; }
                                        if (subline.IBTS_WHS_NO != null) { SO_Subline["tss_ibtswhsno"] = subline.IBTS_WHS_NO; }
                                        if (subline.PO_NO_PRIN != null) { SO_Subline["tss_ponumberprincipal"] = subline.PO_NO_PRIN; }
                                        if (subline.RSV_NO != null) { SO_Subline["tss_reservationno"] = subline.RSV_NO; }
                                        if (subline.WHS_BRANCH != null) { SO_Subline["tss_wrsbranch"] = subline.WHS_BRANCH; }
                                        //Int
                                        SO_Subline["tss_qtyavailable"] = QtyAvail;
                                        SO_Subline["tss_qtyindent"] = QtyInden;

                                        updateSOsubLines.Entities.Add(SO_Subline);
                                        #endregion
                                    }
                                    else
                                    {
                                        #region create SO subline
                                        Entity SO_Subline = new Entity("tss_salesorderpartsublines");
                                        //Must inputted
                                        SO_Subline["tss_salesorderpartlines"] = SO_Line.ToEntityReference();
                                        SO_Subline["tss_qtyavailablebranch"] = Branch.ToEntityReference();
                                        SO_Subline["tss_name"] = PartNumber;
                                        //Int
                                        SO_Subline["tss_qtyavailable"] = QtyAvail;
                                        SO_Subline["tss_qtyindent"] = QtyInden;
                                        SO_Subline["tss_transactiontype"] = new OptionSetValue(SUBLINE_TRANSCTTYPE_UPDATEZPART);
                                        //DateTime
                                        if (subline.ETA_JKT != DateTime.MinValue) { SO_Subline["tss_etajakarta"] = ConvertDateTime(subline.ETA_JKT); }
                                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, subline.ETA_JKT.ToString(), MWSLog.LogType.Information, MWSLog.Source.Inbound);
                                        if (subline.IBTS_DATE != DateTime.MinValue) { SO_Subline["tss_ibtsdate"] = ConvertDateTime(subline.IBTS_DATE); }
                                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, subline.IBTS_DATE.ToString(), MWSLog.LogType.Information, MWSLog.Source.Inbound);
                                        if (subline.PO_DATE_PRIN != DateTime.MinValue) { SO_Subline["tss_podateprincipal"] = ConvertDateTime(subline.PO_DATE_PRIN); }
                                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, subline.PO_DATE_PRIN.ToString(), MWSLog.LogType.Information, MWSLog.Source.Inbound);
                                        if (subline.RSV_DATE != DateTime.MinValue) { SO_Subline["tss_reservationdate"] = ConvertDateTime(subline.RSV_DATE); }
                                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, subline.RSV_DATE.ToString(), MWSLog.LogType.Information, MWSLog.Source.Inbound);
                                        if (subline.WHS_BRANCH_DATE != DateTime.MinValue) { SO_Subline["tss_branchdate"] = ConvertDateTime(subline.WHS_BRANCH_DATE); }
                                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, subline.WHS_BRANCH_DATE.ToString(), MWSLog.LogType.Information, MWSLog.Source.Inbound);
                                        if (subline.WRS_DATE != DateTime.MinValue) { SO_Subline["tss_wrsdate"] = ConvertDateTime(subline.WRS_DATE); }
                                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, subline.WRS_DATE.ToString(), MWSLog.LogType.Information, MWSLog.Source.Inbound);
                                        //String
                                        if (subline.IBTS_NO != null) { SO_Subline["tss_ibtsno"] = subline.IBTS_NO; }
                                        if (subline.IBTS_WHS_NO != null) { SO_Subline["tss_ibtswhsno"] = subline.IBTS_WHS_NO; }
                                        if (subline.PO_NO_PRIN != null) { SO_Subline["tss_ponumberprincipal"] = subline.PO_NO_PRIN; }
                                        if (subline.RSV_NO != null) { SO_Subline["tss_reservationno"] = subline.RSV_NO; }
                                        if (subline.WHS_BRANCH != null) { SO_Subline["tss_wrsbranch"] = subline.WHS_BRANCH; }
                                        createSOsubLines.Entities.Add(SO_Subline);
                                        #endregion
                                    }
                                }
                                else
                                {
                                    throw new Exception("SO line not found!");
                                }
                            }
                            else
                            {
                                throw new Exception("Paremeter SO Number SAP / Part Number / Part Branch is Empty!");
                            }
                        }
                        #endregion
                        #region update/create SOsubLines to CRM
                        //check if there is any update so sub.
                        if (updateSOsubLines.Entities.Count > 0)
                        {
                            foreach (var sub in updateSOsubLines.Entities)
                            {
                                organizationService.Update(sub);
                            }
                        }
                        //check if there is any create so sub.
                        if (createSOsubLines.Entities.Count > 0)
                        {
                            foreach (var sub in createSOsubLines.Entities)
                            {
                                organizationService.Create(sub);
                            }
                        }

                        response.Result = "Success";
                        response.ErrorDescription = "Success to Create/Update Sales Order Sub Lines on CRM.";
                        response.SyncDate = DateTime.Now;
                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, Primary_SO_NO_SAP + _mwsLog.ColumnsSeparator
                             + response.Result, MWSLog.LogType.Information, MWSLog.Source.Inbound);
                        #endregion
                    }
                    else
                    {
                        throw new Exception("Access denied invalid token!");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = "Failed";
                response.ErrorDescription = "Failed to Create/Update Sales Order Sub Lines on CRM. Details : " + ex.Message;
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator
                                 + "Error : " + ex.Message.ToString(), MWSLog.LogType.Error, MWSLog.Source.Inbound);
            }
            return response;
        }

        public CRM_WS_Response CancelSO(string token, string SO_NO_SAP)
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

                    string generateToken = Encrypt(SO_NO_SAP, uniqueKey);
                    if (generateToken == token)
                    {
                        if (!string.IsNullOrWhiteSpace(SO_NO_SAP))
                        {
                            #region Cancel SO
                            QueryExpression queryGetSOPart = new QueryExpression("tss_sopartheader");
                            queryGetSOPart.ColumnSet = new ColumnSet(true);
                            queryGetSOPart.Criteria.AddCondition("tss_sapsoid", ConditionOperator.Equal, SO_NO_SAP);
                            Entity SOPart = organizationService.RetrieveMultiple(queryGetSOPart).Entities[0];

                            if (SOPart == null)
                                throw new Exception("SO Part with current SAP SO Number not found in master branch!");

                            #region Update SO Header
                            SOPart["tss_statecode"] = new OptionSetValue(SO_STATECODE_CANCEL);
                            SOPart["tss_statusreason"] = new OptionSetValue(SO_STATUSREASON_CLOSED);
                            organizationService.Update(SOPart);
                            #endregion

                            #region Update SO Lines
                            QueryExpression queryGetSOPartlines = new QueryExpression("tss_sopartlines");
                            queryGetSOPartlines.ColumnSet = new ColumnSet(true);
                            queryGetSOPartlines.Criteria.AddCondition("tss_sopartheaderid", ConditionOperator.Equal, SOPart.Id);
                            EntityCollection SOPartlines = organizationService.RetrieveMultiple(queryGetSOPartlines);

                            foreach (var SOPartline in SOPartlines.Entities)
                            {
                                SOPartline["tss_status"] = new OptionSetValue(SOLINE_STATUS_CANCEL);
                                organizationService.Update(SOPartline);
                            }
                            #endregion

                            #region Update Quotation to 
                            if (SOPart.Attributes.Contains("tss_quotationlink") && SOPart.Attributes["tss_quotationlink"] != null)
                            {
                                Entity Quotation = new Entity("tss_quotationpartheader");
                                Quotation["tss_statuscode"] = new OptionSetValue(QUOTE_STATUSCODE_CLOSED);
                                Quotation["tss_statusreason"] = new OptionSetValue(QUOTE_STATUSREASON_LOST);
                                Quotation["tss_quotationpartheaderid"] = SOPart.GetAttributeValue<EntityReference>("tss_quotationlink").Id;
                                organizationService.Update(Quotation);
                            }
                            #endregion

                            #region Update Prospect
                            if (SOPart.Attributes.Contains("tss_prospectlink") && SOPart.Attributes["tss_prospectlink"] != null)
                            {
                                Entity Prospect = new Entity("tss_prospectpartheader");
                                Prospect["tss_statusreason"] = new OptionSetValue(PROSPECT_STATUSREASON_LOST);
                                Prospect["tss_prospectpartheaderid"] = SOPart.GetAttributeValue<EntityReference>("tss_prospectlink").Id;
                                organizationService.Update(Prospect);
                            }
                            #endregion

                            response.Result = "Success";
                            response.ErrorDescription = "Success to Cancel SO on CRM.";
                            response.SyncDate = DateTime.Now;
                            _mwsLog.Write(MethodBase.GetCurrentMethod().Name, SO_NO_SAP + _mwsLog.ColumnsSeparator
                                     + response.Result, MWSLog.LogType.Information, MWSLog.Source.Inbound);
                            #endregion
                        }
                        else
                        {
                            throw new Exception("Paremeter SO Number SAP is Empty!");
                        }
                    }
                    else
                    {
                        throw new Exception("Access denied invalid token!");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = "Failed";
                response.ErrorDescription = "Failed to Cancel SO on CRM for Sales Order Number : " + SO_NO_SAP + ". Details : " + ex.Message;
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator
                             + "Error : " + response.ErrorDescription, MWSLog.LogType.Error, MWSLog.Source.Inbound);
            }

            return response;
        }

        public CRM_WS_Response PaidSalesOrder(string token, ParamPaidSalesOrder[] SalesOrders)
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

                    string Primary_INV_Number = SalesOrders[0].INV_Number;
                    string generateToken = Encrypt(Primary_INV_Number, uniqueKey);
                    if (generateToken == token)
                    {
                        EntityCollection createSOsubLines = new EntityCollection();
                        EntityCollection updateSOlines = new EntityCollection();
                        EntityCollection updateSOheaders = new EntityCollection();

                        #region looping confirm TO Details params
                        for (int x = 0; x < SalesOrders.Length; x++)
                        {
                            ParamPaidSalesOrder SalesOrder = SalesOrders[x];

                            _mwsLog.Write(MethodBase.GetCurrentMethod().Name, SalesOrder.SO_NO_SAP + _mwsLog.ColumnsSeparator
                                 + SalesOrder.Sparepart_NO + _mwsLog.ColumnsSeparator
                                 + SalesOrder.PartBranch + _mwsLog.ColumnsSeparator
                                 + SalesOrder.Paid_DATE + _mwsLog.ColumnsSeparator
                                 + SalesOrder.INV_Number + _mwsLog.ColumnsSeparator
                                 + SalesOrder.Paid_Value, MWSLog.LogType.Information, MWSLog.Source.Inbound);

                            string SO_NO_SAP = SalesOrder.SO_NO_SAP;
                            string PartNumber = SalesOrder.Sparepart_NO;
                            string PartBranch = SalesOrder.PartBranch;

                            QueryExpression queryGetPart = new QueryExpression("trs_masterpart");
                            queryGetPart.ColumnSet = new ColumnSet(true);
                            queryGetPart.Criteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.And,
                                Conditions =
                                    {
                                        new ConditionExpression("trs_name", ConditionOperator.Equal, PartNumber),
                                    }
                            };
                            Entity part = organizationService.RetrieveMultiple(queryGetPart).Entities.FirstOrDefault();

                            QueryExpression queryGetBranch = new QueryExpression("businessunit");
                            queryGetBranch.ColumnSet = new ColumnSet(true);
                            queryGetBranch.Criteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.And,
                                Conditions =
                                    {
                                        new ConditionExpression("trs_branchcode", ConditionOperator.Equal, SalesOrder.PartBranch),
                                        new ConditionExpression("name", ConditionOperator.Equal, SalesOrder.PartBranch)
                                    }
                            };
                            Entity Branch = organizationService.RetrieveMultiple(queryGetBranch).Entities.FirstOrDefault();

                            QueryExpression queryGetSO = new QueryExpression("tss_sopartheader");
                            queryGetSO.ColumnSet = new ColumnSet(true);
                            queryGetSO.Criteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.And,
                                Conditions =
                                    {
                                        new ConditionExpression("tss_sapsoid", ConditionOperator.Equal, SO_NO_SAP)
                                    }
                            };
                            Entity SO_Header = organizationService.RetrieveMultiple(queryGetSO).Entities.FirstOrDefault();

                            if (part == null)
                                throw new Exception("Partnumber not found in part master!");
                            if (Branch == null)
                                throw new Exception("Branch not found in master branch!");
                            if (SO_Header == null)
                                throw new Exception("SO Part with current SAP SO Number not found in master branch!");

                            QueryExpression queryGetSOLine = new QueryExpression("tss_sopartlines");
                            queryGetSOLine.ColumnSet = new ColumnSet(true);
                            queryGetSOLine.Criteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.Or,
                                Conditions =
                                    {
                                        new ConditionExpression("tss_sopartheaderid", ConditionOperator.Equal, SO_Header.Id)
                                        //new ConditionExpression("tss_partnumber", ConditionOperator.Equal, part.Id)
                                    }
                            };

                            EntityCollection SO_Lines = organizationService.RetrieveMultiple(queryGetSOLine);

                            QueryExpression queryGetPartlinesInter = new QueryExpression("tss_partmasterlinesinterchange");
                            queryGetPartlinesInter.ColumnSet = new ColumnSet(true);
                            queryGetPartlinesInter.Criteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.And,
                                Conditions =
                                {
                                    new ConditionExpression("tss_partnumberinterchange", ConditionOperator.Equal, part.Id),
                                    //new ConditionExpression("tss_status", ConditionOperator.Equal, PartInterchange_STATUS_ACTIVE)//status = active
                                }
                            };
                            EntityCollection PartlineInters = organizationService.RetrieveMultiple(queryGetPartlinesInter);
                            List<Entity> SOLines = new List<Entity>();
                            Entity SO_Line = new Entity();

                            if (PartlineInters.Entities.Count > 0)
                            {
                                SOLines = SO_Lines.Entities.Where(o => o.GetAttributeValue<bool>("tss_isinterchange") == true && o.GetAttributeValue<EntityReference>("tss_partnumberinterchange").Id == PartlineInters.Entities[0].Id).ToList();
                                SO_Line = SOLines.FirstOrDefault();
                                if (SO_Line == null)
                                    SO_Line = SO_Lines.Entities.Where(o => o.GetAttributeValue<EntityReference>("tss_partnumber").Id == part.Id).FirstOrDefault();
                            }
                            else
                            {
                                SOLines = SO_Lines.Entities.Where(o => o.GetAttributeValue<EntityReference>("tss_partnumber").Id == part.Id).ToList();
                                SO_Line = SOLines.FirstOrDefault();
                            }

                            if (SO_Line != null)
                            {
                                QueryExpression queryGetSOSubLine = new QueryExpression("tss_salesorderpartsublines");
                                queryGetSOSubLine.ColumnSet = new ColumnSet(true);
                                queryGetSOSubLine.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("tss_name", ConditionOperator.Equal, PartNumber),
                                        new ConditionExpression("tss_salesorderpartlines", ConditionOperator.Equal, SO_Line.Id),
                                        //new ConditionExpression("tss_transactiontype", ConditionOperator.Equal, SUBLINE_TRANSCTTYPE_CREATEPYMNT) //UNUSED [6/2/2018] BY SANTONY
                                    }
                                };
                                EntityCollection SO_Sublines = organizationService.RetrieveMultiple(queryGetSOSubLine);

                                #region update SO subline
                                //create new subline for transaction type payment
                                Entity SO_Subline = new Entity("tss_salesorderpartsublines");
                                SO_Subline["tss_name"] = PartNumber;
                                SO_Subline["tss_salesorderpartlines"] = SO_Line.ToEntityReference();
                                SO_Subline["tss_qtyavailablebranch"] = Branch.ToEntityReference();
                                //DateTime
                                if (SalesOrder.Paid_DATE != DateTime.MinValue) { SO_Subline["tss_paiddate"] = ConvertDateTime(SalesOrder.Paid_DATE); }
                                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, SalesOrder.Paid_DATE.ToString(), MWSLog.LogType.Information, MWSLog.Source.Inbound);
                                //String
                                if (SalesOrder.INV_Number != null) { SO_Subline["tss_invoiceno"] = SalesOrder.INV_Number; }
                                //Int
                                decimal paid_val = Convert.ToDecimal(SalesOrder.Paid_Value);
                                SO_Subline["tss_paidvalue"] = new Money(paid_val);
                                SO_Subline["tss_transactiontype"] = new OptionSetValue(SUBLINE_TRANSCTTYPE_CREATEPYMNT);
                                createSOsubLines.Entities.Add(SO_Subline);

                                decimal totalPaid = paid_val;
                                foreach (var subline in SO_Sublines.Entities)
                                {
                                    if (subline.Contains("tss_paidvalue"))
                                        totalPaid += subline.GetAttributeValue<Money>("tss_paidvalue").Value;
                                }

                                if (SO_Line.Contains("tss_amountinvoicesap") && SO_Line.Attributes["tss_amountinvoicesap"] != null)
                                {
                                    var inv_val = SO_Line.GetAttributeValue<Money>("tss_amountinvoicesap").Value;
                                    if (inv_val == totalPaid)
                                    {
                                        SO_Line["tss_status"] = new OptionSetValue(SOLINE_STATUS_PAID);
                                        //updateSOlines.Entities.Add(SO_Line);
                                        if (updateSOlines.Entities.Count > 0)
                                        {
                                            bool isexist = updateSOlines.Entities.Any(a => a.Id == SO_Line.Id);
                                            if (!isexist)
                                                updateSOlines.Entities.Add(SO_Line);
                                        }
                                        else
                                        {
                                            updateSOlines.Entities.Add(SO_Line);
                                        }

                                        if (updateSOheaders.Entities.Count > 0)
                                        {
                                            bool isexist = updateSOheaders.Entities.Any(a => a.Id == SO_Header.Id);
                                            if (!isexist)
                                                updateSOheaders.Entities.Add(SO_Header);
                                        }
                                        else
                                        {
                                            updateSOheaders.Entities.Add(SO_Header);
                                        }
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                throw new Exception("SO line not found!");
                            }
                        }
                        #endregion
                        #region update so sublines on crm
                        if (createSOsubLines.Entities.Count > 0)
                        {
                            foreach (var createSOsubLine in createSOsubLines.Entities)
                            {
                                organizationService.Create(createSOsubLine);
                            }
                        }
                        if (updateSOlines.Entities.Count > 0)
                        {
                            foreach (var updateSOLine in updateSOlines.Entities)
                            {
                                organizationService.Update(updateSOLine);
                            }
                        }
                        if (updateSOheaders.Entities.Count > 0)
                        {
                            foreach (var updateSOPart in updateSOheaders.Entities)
                            {
                                QueryExpression queryGetSOLine = new QueryExpression("tss_sopartlines");
                                queryGetSOLine.ColumnSet = new ColumnSet(true);
                                queryGetSOLine.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("tss_sopartheaderid", ConditionOperator.Equal, updateSOPart.Id)
                                    }
                                };
                                EntityCollection SO_Lines = organizationService.RetrieveMultiple(queryGetSOLine);
                                int Linesrecord = SO_Lines.Entities.Count;
                                var Linesparametered = from c in SO_Lines.Entities
                                                       where c.GetAttributeValue<OptionSetValue>("tss_status").Value == SOLINE_STATUS_PAID
                                                       select c;
                                if (Linesrecord == Linesparametered.Count())
                                {
                                    updateSOPart["tss_statecode"] = new OptionSetValue(SO_STATECODE_PAID);
                                    organizationService.Update(updateSOPart);
                                }
                            }
                        }

                        response.Result = "Success";
                        response.ErrorDescription = "Success to update Payment Invoice on Sales Order Sub Lines on CRM.";
                        response.SyncDate = DateTime.Now;
                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, Primary_INV_Number + _mwsLog.ColumnsSeparator
                                 + response.Result, MWSLog.LogType.Information, MWSLog.Source.Inbound);
                        #endregion
                    }
                    else
                    {
                        throw new Exception("Access denied invalid token!");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = "Failed";
                response.ErrorDescription = "Failed to update Payment Invoice on Sales Order Sub Lines on CRM. Details : " + ex.Message;
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator
                                 + "Error : " + ex.Message.ToString(), MWSLog.LogType.Error, MWSLog.Source.Inbound);
            }
            return response;
        }
        #endregion

        #region SO Invoice
        public CRM_WS_Response CreateInvoiceSO(string token, string invoice_No, DateTime invoice_Date, ParamCreateInvoiceSO[] invoice_Details)
        {
            CRM_WS_Response response = new CRM_WS_Response();
            try
            {
                string _connectionString = string.Empty;
                decimal AmountInvoiceSAP = 0;

                if (IsValidConnectionString(ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString))
                    _connectionString = ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString;

                using (CrmServiceClient conn = new CrmServiceClient(_connectionString))
                {
                    IOrganizationService organizationService = (IOrganizationService)conn.OrganizationServiceProxy;

                    string generateToken = Encrypt(invoice_No, uniqueKey);
                    if (generateToken == token)
                    {
                        EntityCollection createInvoices = new EntityCollection();
                        EntityCollection updateSOLines = new EntityCollection();
                        EntityCollection updateSOParts = new EntityCollection();

                        #region looping sublines params
                        for (int x = 0; x < invoice_Details.Length; x++)
                        {
                            ParamCreateInvoiceSO paramdetail = invoice_Details[x];

                            QueryExpression queryGetPart = new QueryExpression("trs_masterpart");
                            queryGetPart.ColumnSet = new ColumnSet(true);
                            queryGetPart.Criteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.And,
                                Conditions =
                                    {
                                        new ConditionExpression("trs_name", ConditionOperator.Equal, paramdetail.Sparepart_NO)
                                    }
                            };
                            Entity part = organizationService.RetrieveMultiple(queryGetPart).Entities.FirstOrDefault();

                            QueryExpression queryGetBranch = new QueryExpression("businessunit");
                            queryGetBranch.ColumnSet = new ColumnSet(true);
                            queryGetBranch.Criteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.And,
                                Conditions =
                                    {
                                        new ConditionExpression("trs_branchcode", ConditionOperator.Equal, paramdetail.PartBranch),
                                        new ConditionExpression("name", ConditionOperator.Equal, paramdetail.PartBranch)
                                    }
                            };
                            Entity Branch = organizationService.RetrieveMultiple(queryGetBranch).Entities.FirstOrDefault();

                            QueryExpression queryGetSO = new QueryExpression("tss_sopartheader");
                            queryGetSO.ColumnSet = new ColumnSet(true);
                            queryGetSO.Criteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.And,
                                Conditions =
                                    {
                                        new ConditionExpression("tss_sapsoid", ConditionOperator.Equal, paramdetail.SO_NO_SAP)
                                    }
                            };
                            Entity SO_Header = organizationService.RetrieveMultiple(queryGetSO).Entities.FirstOrDefault();

                            if (part == null)
                                throw new Exception("Partnumber not found in part master!");
                            if (Branch == null)
                                throw new Exception("Branch not found in master branch!");
                            if (SO_Header == null)
                                throw new Exception("SO Part with current SAP SO Number not found in master branch!");

                            QueryExpression queryGetSOLine = new QueryExpression("tss_sopartlines");
                            queryGetSOLine.ColumnSet = new ColumnSet(true);
                            queryGetSOLine.Criteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.Or,
                                Conditions =
                                    {
                                        new ConditionExpression("tss_sopartheaderid", ConditionOperator.Equal, SO_Header.Id)
                                        //new ConditionExpression("tss_partnumber", ConditionOperator.Equal, part.Id)
                                    }
                            };

                            EntityCollection SO_Lines = organizationService.RetrieveMultiple(queryGetSOLine);

                            QueryExpression queryGetPartlinesInter = new QueryExpression("tss_partmasterlinesinterchange");
                            queryGetPartlinesInter.ColumnSet = new ColumnSet(true);
                            queryGetPartlinesInter.Criteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.And,
                                Conditions =
                                {
                                    new ConditionExpression("tss_partnumberinterchange", ConditionOperator.Equal, part.Id),
                                    //new ConditionExpression("tss_status", ConditionOperator.Equal, PartInterchange_STATUS_ACTIVE)//status = active
                                }
                            };
                            EntityCollection PartlineInters = organizationService.RetrieveMultiple(queryGetPartlinesInter);
                            List<Entity> SOLines = new List<Entity>();
                            Entity SO_Line = new Entity();

                            if (PartlineInters.Entities.Count > 0)
                            {
                                SOLines = SO_Lines.Entities.Where(o => o.GetAttributeValue<bool>("tss_isinterchange") == true && o.GetAttributeValue<EntityReference>("tss_partnumberinterchange").Id == PartlineInters.Entities[0].Id).ToList();
                                SO_Line = SOLines.FirstOrDefault();
                                if (SO_Line == null)
                                    SO_Line = SO_Lines.Entities.Where(o => o.GetAttributeValue<EntityReference>("tss_partnumber").Id == part.Id).FirstOrDefault();
                            }
                            else
                            {
                                SOLines = SO_Lines.Entities.Where(o => o.GetAttributeValue<EntityReference>("tss_partnumber").Id == part.Id).ToList();
                                SO_Line = SOLines.FirstOrDefault();
                            }

                            if (SO_Line != null)
                            {
                                QueryExpression queryGetSOSubLine = new QueryExpression("tss_salesorderpartsublines");
                                queryGetSOSubLine.ColumnSet = new ColumnSet(true);
                                queryGetSOSubLine.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("tss_name", ConditionOperator.Equal, paramdetail.Sparepart_NO),
                                        new ConditionExpression("tss_salesorderpartlines", ConditionOperator.Equal, SO_Line.Id)
                                    }
                                };

                                EntityCollection SO_Sublines = organizationService.RetrieveMultiple(queryGetSOSubLine);

                                EntityReference Payterm = SO_Header.GetAttributeValue<EntityReference>("tss_paymentterm");
                                Entity Paymentterm = organizationService.Retrieve("new_paymentterm", Payterm.Id, new ColumnSet(true));
                                int days = 0;
                                if (Paymentterm.Contains("tss_days"))
                                    days = Paymentterm.GetAttributeValue<int>("tss_days");
                                int invoiceQty = paramdetail.Quantity;

                                List<Entity> items = SO_Sublines.Entities.Where(o => o.GetAttributeValue<OptionSetValue>("tss_transactiontype").Value == SUBLINE_TRANSCTTYPE_CREATEINV).ToList();

                                #region Update SO Sub Lines
                                Entity SO_Subline = new Entity("tss_salesorderpartsublines");
                                SO_Subline["tss_name"] = paramdetail.Sparepart_NO;
                                SO_Subline["tss_salesorderpartlines"] = SO_Line.ToEntityReference();
                                SO_Subline["tss_qtyavailablebranch"] = Branch.ToEntityReference();
                                SO_Subline["tss_invoiceno"] = invoice_No;
                                SO_Subline["tss_transactiontype"] = new OptionSetValue(SUBLINE_TRANSCTTYPE_CREATEINV);
                                //detail["Tss_CancelInvoiceNo"]
                                SO_Subline["tss_invoicedate"] = invoice_Date;
                                SO_Subline["tss_invoiceduedate"] = invoice_Date.AddDays(days); //invoice date + payment terms days.
                                SO_Subline["tss_invoicevalue"] = new Money(Convert.ToDecimal(paramdetail.INV_Value));
                                SO_Subline["tss_invoiceqty"] = paramdetail.Quantity;
                                createInvoices.Entities.Add(SO_Subline);
                                #endregion

                                #region Update SO Lines
                                if (SO_Line.Attributes.Contains("tss_amountinvoicesap") && SO_Line.Attributes["tss_amountinvoicesap"] != null)
                                    AmountInvoiceSAP = SO_Line.GetAttributeValue<Money>("tss_amountinvoicesap").Value;

                                SO_Line["tss_amountinvoicesap"] = new Money(AmountInvoiceSAP + Convert.ToDecimal(paramdetail.INV_Value));
                                updateSOLines.Entities.Add(SO_Line);
                                #endregion

                                int qtyReq = SO_Line.GetAttributeValue<int>("tss_qtyrequest");
                                foreach (var item in items)
                                {
                                    if (item.Contains("tss_invoiceqty"))
                                        invoiceQty += item.GetAttributeValue<int>("tss_invoiceqty");
                                }

                                //Jika Invoice Qty di SO Sub Lines = Qty Request di SO Lines, status jadi invoiced
                                if (invoiceQty == qtyReq)
                                {
                                    SO_Line["tss_status"] = new OptionSetValue(SOLINE_STATUS_INVOICED);

                                    //updateSOLines.Entities.Add(SO_Line);
                                    if (updateSOLines.Entities.Count > 0)
                                    {
                                        bool isexist = updateSOLines.Entities.Any(a => a.Id == SO_Line.Id);
                                        if (!isexist)
                                            updateSOLines.Entities.Add(SO_Line);
                                    }
                                    else
                                    {
                                        updateSOLines.Entities.Add(SO_Line);
                                    }

                                    if (updateSOParts.Entities.Count > 0)
                                    {
                                        bool isexist = updateSOParts.Entities.Any(a => a.Id == SO_Header.Id);
                                        if (!isexist)
                                            updateSOParts.Entities.Add(SO_Header);
                                    }
                                    else
                                    {
                                        updateSOParts.Entities.Add(SO_Header);
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception("SO line not found!");
                            }
                        }
                        #endregion
                        #region create invoice to CRM
                        if (createInvoices.Entities.Count > 0)
                        {
                            foreach (var createInvoice in createInvoices.Entities)
                            {
                                organizationService.Create(createInvoice);
                            }
                        }
                        if (updateSOLines.Entities.Count > 0)
                        {
                            foreach (var updateSOLine in updateSOLines.Entities)
                            {
                                organizationService.Update(updateSOLine);
                            }
                        }
                        if (updateSOParts.Entities.Count > 0)
                        {
                            foreach (var updateSOPart in updateSOParts.Entities)
                            {
                                QueryExpression queryGetSOLine = new QueryExpression("tss_sopartlines");
                                queryGetSOLine.ColumnSet = new ColumnSet(true);
                                queryGetSOLine.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("tss_sopartheaderid", ConditionOperator.Equal, updateSOPart.Id)
                                    }
                                };
                                EntityCollection SO_Lines = organizationService.RetrieveMultiple(queryGetSOLine);
                                int Linesrecord = SO_Lines.Entities.Count;
                                var Linesparametered = from c in SO_Lines.Entities
                                                       where c.GetAttributeValue<OptionSetValue>("tss_status").Value == SOLINE_STATUS_INVOICED
                                                       || c.GetAttributeValue<OptionSetValue>("tss_status").Value == SOLINE_STATUS_PAID
                                                       select c;
                                if (Linesrecord == Linesparametered.Count())
                                {
                                    updateSOPart["tss_statecode"] = new OptionSetValue(SO_STATECODE_INVOICED);
                                    organizationService.Update(updateSOPart);
                                }
                            }
                        }

                        response.Result = "Success";
                        response.ErrorDescription = "Success to Create SO Invoice on Sales Order Sub Lines on CRM.";
                        response.SyncDate = DateTime.Now;
                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, invoice_No + _mwsLog.ColumnsSeparator
                             + response.Result, MWSLog.LogType.Information, MWSLog.Source.Inbound);
                        #endregion
                    }
                    else
                    {
                        throw new Exception("Access denied invalid token!");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = "Failed";
                response.ErrorDescription = "Failed to Create SO Invoice on Sales Order Sub Lines on CRM. Details : " + ex.Message;
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator
                                 + "Error : " + ex.Message.ToString(), MWSLog.LogType.Error, MWSLog.Source.Inbound);
            }
            return response;
        }

        public CRM_WS_Response CancelInvoiceSO(string token, string invoice_No, string cancel_invoice_No, DateTime invoice_Date, ParamCancelInvoiceSO[] invoice_Details)
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

                    string generateToken = Encrypt(cancel_invoice_No, uniqueKey);
                    if (generateToken == token)
                    {
                        EntityCollection cancelInvoices = new EntityCollection();
                        EntityCollection updateSOLines = new EntityCollection();
                        EntityCollection updateSOParts = new EntityCollection();

                        #region looping sublines params
                        for (int x = 0; x < invoice_Details.Length; x++)
                        {
                            ParamCancelInvoiceSO paramdetail = invoice_Details[x];

                            QueryExpression queryGetPart = new QueryExpression("trs_masterpart");
                            queryGetPart.ColumnSet = new ColumnSet(true);
                            queryGetPart.Criteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.And,
                                Conditions =
                                    {
                                        new ConditionExpression("trs_name", ConditionOperator.Equal, paramdetail.Sparepart_NO)
                                    }
                            };
                            Entity part = organizationService.RetrieveMultiple(queryGetPart).Entities.FirstOrDefault();

                            QueryExpression queryGetBranch = new QueryExpression("businessunit");
                            queryGetBranch.ColumnSet = new ColumnSet(true);
                            queryGetBranch.Criteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.And,
                                Conditions =
                                    {
                                        new ConditionExpression("trs_branchcode", ConditionOperator.Equal, paramdetail.PartBranch),
                                        new ConditionExpression("name", ConditionOperator.Equal, paramdetail.PartBranch)
                                    }
                            };
                            Entity Branch = organizationService.RetrieveMultiple(queryGetBranch).Entities.FirstOrDefault();

                            QueryExpression queryGetSO = new QueryExpression("tss_sopartheader");
                            queryGetSO.ColumnSet = new ColumnSet(true);
                            queryGetSO.Criteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.And,
                                Conditions =
                                {
                                    new ConditionExpression("tss_sapsoid", ConditionOperator.Equal, paramdetail.SO_NO_SAP)
                                }
                            };
                            Entity SO_Header = organizationService.RetrieveMultiple(queryGetSO).Entities.FirstOrDefault();

                            if (part == null)
                                throw new Exception("Partnumber not found in part master!");
                            if (Branch == null)
                                throw new Exception("Branch not found in master branch!");
                            if (SO_Header == null)
                                throw new Exception("SO Part with current SAP SO Number not found in master branch!");

                            QueryExpression queryGetSOLine = new QueryExpression("tss_sopartlines");
                            queryGetSOLine.ColumnSet = new ColumnSet(true);
                            queryGetSOLine.Criteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.Or,
                                Conditions =
                                {
                                    new ConditionExpression("tss_sopartheaderid", ConditionOperator.Equal, SO_Header.Id)
                                    //new ConditionExpression("tss_partnumber", ConditionOperator.Equal, part.Id)
                                }
                            };

                            EntityCollection SO_Lines = organizationService.RetrieveMultiple(queryGetSOLine);

                            QueryExpression queryGetPartlinesInter = new QueryExpression("tss_partmasterlinesinterchange");
                            queryGetPartlinesInter.ColumnSet = new ColumnSet(true);
                            queryGetPartlinesInter.Criteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.And,
                                Conditions =
                                {
                                    new ConditionExpression("tss_partnumberinterchange", ConditionOperator.Equal, part.Id),
                                    //new ConditionExpression("tss_status", ConditionOperator.Equal, PartInterchange_STATUS_ACTIVE)//status = active
                                }
                            };
                            EntityCollection PartlineInters = organizationService.RetrieveMultiple(queryGetPartlinesInter);
                            List<Entity> SOLines = new List<Entity>();
                            Entity SO_Line = new Entity();

                            if (PartlineInters.Entities.Count > 0)
                            {
                                SOLines = SO_Lines.Entities.Where(o => o.GetAttributeValue<bool>("tss_isinterchange") == true && o.GetAttributeValue<EntityReference>("tss_partnumberinterchange").Id == PartlineInters.Entities[0].Id).ToList();
                                SO_Line = SOLines.FirstOrDefault();
                                if (SO_Line == null)
                                    SO_Line = SO_Lines.Entities.Where(o => o.GetAttributeValue<EntityReference>("tss_partnumber").Id == part.Id).FirstOrDefault();
                            }
                            else
                            {
                                SOLines = SO_Lines.Entities.Where(o => o.GetAttributeValue<EntityReference>("tss_partnumber").Id == part.Id).ToList();
                                SO_Line = SOLines.FirstOrDefault();
                            }

                            if (SO_Line != null)
                            {
                                QueryExpression queryGetSOSubLine = new QueryExpression("tss_salesorderpartsublines");
                                queryGetSOSubLine.ColumnSet = new ColumnSet(true);
                                queryGetSOSubLine.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("tss_name", ConditionOperator.Equal, paramdetail.Sparepart_NO),
                                        new ConditionExpression("tss_salesorderpartlines", ConditionOperator.Equal, SO_Line.Id)
                                        //new ConditionExpression("tss_invoiceno", ConditionOperator.Equal, invoice_No)
                                    }
                                };
                                EntityCollection SO_Sublines = organizationService.RetrieveMultiple(queryGetSOSubLine);
                                try
                                {
                                    List<Entity> exceptThis = SO_Sublines.Entities.Where(o => o.GetAttributeValue<OptionSetValue>("tss_transactiontype").Value == SUBLINE_TRANSCTTYPE_CREATEINV).ToList();
                                    Entity SO_Subline = SO_Sublines.Entities.Where(o => o.GetAttributeValue<string>("tss_invoiceno") == invoice_No && o.GetAttributeValue<EntityReference>("tss_qtyavailablebranch").Id == Branch.Id).First();
                                    int invoiceQty = paramdetail.Quantity;
                                    //SO_Subline["tss_invoiceno"] = invoice_No;
                                    SO_Subline["tss_cancelinvoiceno"] = cancel_invoice_No;
                                    SO_Subline["tss_invoiceqty"] = paramdetail.Quantity;
                                    SO_Subline["tss_transactiontype"] = new OptionSetValue(SUBLINE_TRANSCTTYPE_UPDATEINV);
                                    //SO_Subline["tss_invoicedate"] = invoice_Date;
                                    //SO_Subline["tss_invoicevalue"] = new Money(Convert.ToDecimal(paramdetail.INV_Value));
                                    cancelInvoices.Entities.Add(SO_Subline);

                                    int qtyReq = SO_Line.GetAttributeValue<int>("tss_qtyrequest");
                                    foreach (var exp in exceptThis)
                                    {
                                        if (exp.Contains("tss_invoiceqty"))
                                            invoiceQty += exp.GetAttributeValue<int>("tss_invoiceqty");
                                    }

                                    //Jika total Invoice Qty di SO Sub Lines = Qty Request di SO Lines, status jadi delivered
                                    if (invoiceQty == qtyReq)
                                    {
                                        SO_Line["tss_status"] = new OptionSetValue(SOLINE_STATUS_DELIVERED);

                                        if (updateSOLines.Entities.Count > 0)
                                        {
                                            bool isexist = updateSOLines.Entities.Any(a => a.Id == SO_Line.Id);
                                            if (!isexist)
                                                updateSOLines.Entities.Add(SO_Line);
                                        }
                                        else
                                        {
                                            updateSOLines.Entities.Add(SO_Line);
                                        }

                                        if (updateSOParts.Entities.Count > 0)
                                        {
                                            bool isexist = updateSOParts.Entities.Any(a => a.Id == SO_Header.Id);
                                            if (!isexist)
                                                updateSOParts.Entities.Add(SO_Header);
                                        }
                                        else
                                        {
                                            updateSOParts.Entities.Add(SO_Header);
                                        }
                                    }
                                }
                                catch
                                {
                                    throw new Exception("SO subline not found!");
                                }
                            }
                            else
                            {
                                throw new Exception("SO line not found!");
                            }
                        }
                        #endregion
                        #region cancel invoice to CRM
                        if (cancelInvoices.Entities.Count > 0)
                        {
                            foreach (var cancelInvoice in cancelInvoices.Entities)
                            {
                                organizationService.Update(cancelInvoice);
                            }
                        }
                        if (updateSOLines.Entities.Count > 0)
                        {
                            foreach (var updateSOLine in updateSOLines.Entities)
                            {
                                organizationService.Update(updateSOLine);
                            }
                        }
                        if (updateSOParts.Entities.Count > 0)
                        {
                            foreach (var updateSOPart in updateSOParts.Entities)
                            {
                                QueryExpression queryGetSOLine = new QueryExpression("tss_sopartlines");
                                queryGetSOLine.ColumnSet = new ColumnSet(true);
                                queryGetSOLine.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("tss_sopartheaderid", ConditionOperator.Equal, updateSOPart.Id)
                                    }
                                };
                                EntityCollection SO_Lines = organizationService.RetrieveMultiple(queryGetSOLine);
                                int Linesrecord = SO_Lines.Entities.Count;
                                var Linesparametered = from c in SO_Lines.Entities
                                                       where c.GetAttributeValue<OptionSetValue>("tss_status").Value == SOLINE_STATUS_DELIVERED
                                                       || c.GetAttributeValue<OptionSetValue>("tss_status").Value == SOLINE_STATUS_INVOICED
                                                       || c.GetAttributeValue<OptionSetValue>("tss_status").Value == SOLINE_STATUS_PAID
                                                       select c;
                                if (Linesrecord == Linesparametered.Count() && Linesparametered.Any(x => x.GetAttributeValue<OptionSetValue>("tss_status").Value == SOLINE_STATUS_DELIVERED))
                                {
                                    updateSOPart["tss_statecode"] = new OptionSetValue(SO_STATECODE_DELIVERED);
                                    organizationService.Update(updateSOPart);
                                }
                            }
                        }
                        response.Result = "Success";
                        response.ErrorDescription = "Success to Cancel SO Invoice on Sales Order Sub Lines on CRM.";
                        response.SyncDate = DateTime.Now;
                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, invoice_No + _mwsLog.ColumnsSeparator
                             + response.Result, MWSLog.LogType.Information, MWSLog.Source.Inbound);
                        #endregion
                    }
                    else
                    {
                        throw new Exception("Access denied invalid token!");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = "Failed";
                response.ErrorDescription = "Failed to Cancel SO Invoice on Sales Order Sub Lines on CRM. Details : " + ex.Message;
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator
                                 + "Error : " + ex.Message.ToString(), MWSLog.LogType.Error, MWSLog.Source.Inbound);
            }
            return response;
        }
        #endregion

        #region Transfer Order
        public CRM_WS_Response CreateTO_SO(string token, string TO_NO_SAP, string DN_NO_SAP, DateTime TO_Date, DateTime TO_Time, ParamCreateTO_SO[] TO_Details)
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

                    string generateToken = Encrypt(TO_NO_SAP, uniqueKey);
                    if (generateToken == token)
                    {
                        EntityCollection createTOs = new EntityCollection();

                        #region looping createTO Details params
                        for (int x = 0; x < TO_Details.Length; x++)
                        {
                            ParamCreateTO_SO TO_Detail = TO_Details[x];

                            string SO_NO_SAP = TO_Detail.SO_NO_SAP;
                            string PartNumber = TO_Detail.Sparepart_NO;

                            QueryExpression queryGetPart = new QueryExpression("trs_masterpart");
                            queryGetPart.ColumnSet = new ColumnSet(true);
                            queryGetPart.Criteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.And,
                                Conditions =
                                    {
                                        new ConditionExpression("trs_name", ConditionOperator.Equal, PartNumber),
                                    }
                            };
                            Entity part = organizationService.RetrieveMultiple(queryGetPart).Entities.FirstOrDefault();

                            QueryExpression queryGetBranch = new QueryExpression("businessunit");
                            queryGetBranch.ColumnSet = new ColumnSet(true);
                            queryGetBranch.Criteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.And,
                                Conditions =
                                {
                                    new ConditionExpression("trs_branchcode", ConditionOperator.Equal, TO_Detail.PartBranch),
                                    new ConditionExpression("name", ConditionOperator.Equal, TO_Detail.PartBranch)
                                }
                            };
                            Entity Branch = organizationService.RetrieveMultiple(queryGetBranch).Entities.FirstOrDefault();

                            QueryExpression queryGetSO = new QueryExpression("tss_sopartheader");
                            queryGetSO.ColumnSet = new ColumnSet(true);
                            queryGetSO.Criteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.And,
                                Conditions =
                                {
                                    new ConditionExpression("tss_sapsoid", ConditionOperator.Equal, SO_NO_SAP)
                                }
                            };
                            Entity SO_Header = organizationService.RetrieveMultiple(queryGetSO).Entities.FirstOrDefault();

                            if (part == null)
                                throw new Exception("Partnumber not found in part master!");
                            if (Branch == null)
                                throw new Exception("Branch not found in master branch!");
                            if (SO_Header == null)
                                throw new Exception("SO Part with current SAP SO Number not found in master branch!");

                            QueryExpression queryGetSOLine = new QueryExpression("tss_sopartlines");
                            queryGetSOLine.ColumnSet = new ColumnSet(true);
                            queryGetSOLine.Criteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.Or,
                                Conditions =
                                {
                                    new ConditionExpression("tss_sopartheaderid", ConditionOperator.Equal, SO_Header.Id)
                                    //new ConditionExpression("tss_partnumber", ConditionOperator.Equal, part.Id)
                                }
                            };

                            EntityCollection SO_Lines = organizationService.RetrieveMultiple(queryGetSOLine);

                            QueryExpression queryGetPartlinesInter = new QueryExpression("tss_partmasterlinesinterchange");
                            queryGetPartlinesInter.ColumnSet = new ColumnSet(true);
                            queryGetPartlinesInter.Criteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.And,
                                Conditions =
                                {
                                    new ConditionExpression("tss_partnumberinterchange", ConditionOperator.Equal, part.Id),
                                    //new ConditionExpression("tss_status", ConditionOperator.Equal, PartInterchange_STATUS_ACTIVE)//status = active
                                }
                            };
                            EntityCollection PartlineInters = organizationService.RetrieveMultiple(queryGetPartlinesInter);
                            List<Entity> SOLines = new List<Entity>();
                            Entity SO_Line = new Entity();

                            if (PartlineInters.Entities.Count > 0)
                            {
                                SOLines = SO_Lines.Entities.Where(o => o.GetAttributeValue<bool>("tss_isinterchange") == true && o.GetAttributeValue<EntityReference>("tss_partnumberinterchange").Id == PartlineInters.Entities[0].Id).ToList();
                                SO_Line = SOLines.FirstOrDefault();
                                if (SO_Line == null)
                                    SO_Line = SO_Lines.Entities.Where(o => o.GetAttributeValue<EntityReference>("tss_partnumber").Id == part.Id).FirstOrDefault();
                            }
                            else
                            {
                                SOLines = SO_Lines.Entities.Where(o => o.GetAttributeValue<EntityReference>("tss_partnumber").Id == part.Id).ToList();
                                SO_Line = SOLines.FirstOrDefault();
                            }

                            if (SO_Line != null)
                            {
                                #region update SO subline
                                Entity SO_Subline = new Entity("tss_salesorderpartsublines");
                                SO_Subline["tss_name"] = PartNumber;
                                SO_Subline["tss_salesorderpartlines"] = SO_Line.ToEntityReference();
                                SO_Subline["tss_qtyavailablebranch"] = Branch.ToEntityReference();

                                //DateTime
                                if (TO_Date != DateTime.MinValue) { SO_Subline["tss_todate"] = ConvertDateTime(TO_Date); }
                                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, TO_Date.ToString(), MWSLog.LogType.Information, MWSLog.Source.Inbound);
                                if (TO_Time != DateTime.MinValue) { SO_Subline["tss_totime"] = ConvertDateTime(TO_Time); }
                                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, TO_Time.ToString(), MWSLog.LogType.Information, MWSLog.Source.Inbound);
                                //String
                                if (DN_NO_SAP != null) { SO_Subline["tss_deliveryno"] = DN_NO_SAP; }
                                if (TO_NO_SAP != null) { SO_Subline["tss_saptono"] = TO_NO_SAP; }
                                //Int
                                SO_Subline["tss_toqty"] = TO_Detail.Quantity;
                                SO_Subline["tss_transactiontype"] = new OptionSetValue(SUBLINE_TRANSCTTYPE_CREATETO);
                                createTOs.Entities.Add(SO_Subline);
                                #endregion
                            }
                            else
                            {
                                throw new Exception("SO line not found!");
                            }
                        }
                        #endregion
                        #region create SO subline createTO
                        if (createTOs.Entities.Count > 0)
                        {
                            foreach (var createTO in createTOs.Entities)
                            {
                                organizationService.Create(createTO);
                            }
                        }

                        response.Result = "Success";
                        response.ErrorDescription = "Success to Create Transfer Order on Sales Order Sub Lines on CRM.";
                        response.SyncDate = DateTime.Now;
                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, TO_NO_SAP + _mwsLog.ColumnsSeparator
                             + response.Result, MWSLog.LogType.Information, MWSLog.Source.Inbound);
                        #endregion
                    }
                    else
                    {
                        throw new Exception("Access denied invalid token!");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = "Failed";
                response.ErrorDescription = "Failed to Create Transfer Order on Sales Order Sub Lines on CRM. Details : " + ex.Message;
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator
                                 + "Error : " + ex.Message.ToString(), MWSLog.LogType.Error, MWSLog.Source.Inbound);
            }
            return response;
        }

        public CRM_WS_Response ConfirmTO_SO(string token, string TO_NO_SAP, string DN_NO_SAP, DateTime Conf_TO_Date, DateTime Conf_TO_Time, string Conf_TO_Status, ParamConfirmTO_SO[] TO_Details)
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

                    string generateToken = Encrypt(TO_NO_SAP, uniqueKey);
                    if (generateToken == token)
                    {
                        EntityCollection confirmTOs = new EntityCollection();
                        EntityCollection updateSOs = new EntityCollection();

                        #region looping createTO Details params
                        for (int x = 0; x < TO_Details.Length; x++)
                        {
                            ParamConfirmTO_SO TO_Detail = TO_Details[x];

                            string SO_NO_SAP = TO_Detail.SO_NO_SAP;
                            string PartNumber = TO_Detail.Sparepart_NO;
                            string PartBranch = TO_Detail.PartBranch;

                            QueryExpression queryGetPart = new QueryExpression("trs_masterpart");
                            queryGetPart.ColumnSet = new ColumnSet(true);
                            queryGetPart.Criteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.And,
                                Conditions =
                                    {
                                        new ConditionExpression("trs_name", ConditionOperator.Equal, PartNumber),
                                    }
                            };
                            Entity part = organizationService.RetrieveMultiple(queryGetPart).Entities.FirstOrDefault();

                            QueryExpression queryGetBranch = new QueryExpression("businessunit");
                            queryGetBranch.ColumnSet = new ColumnSet(true);
                            queryGetBranch.Criteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.And,
                                Conditions =
                                    {
                                        new ConditionExpression("trs_branchcode", ConditionOperator.Equal, TO_Detail.PartBranch),
                                        new ConditionExpression("name", ConditionOperator.Equal, TO_Detail.PartBranch)
                                    }
                            };
                            Entity Branch = organizationService.RetrieveMultiple(queryGetBranch).Entities.FirstOrDefault();

                            QueryExpression queryGetSO = new QueryExpression("tss_sopartheader");
                            queryGetSO.ColumnSet = new ColumnSet(true);
                            queryGetSO.Criteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.And,
                                Conditions =
                                    {
                                        new ConditionExpression("tss_sapsoid", ConditionOperator.Equal, SO_NO_SAP)
                                    }
                            };
                            Entity SO_Header = organizationService.RetrieveMultiple(queryGetSO).Entities.FirstOrDefault();

                            if (part == null)
                                throw new Exception("Partnumber not found in part master!");
                            if (Branch == null)
                                throw new Exception("Branch not found in master branch!");
                            if (SO_Header == null)
                                throw new Exception("SO Part with current SAP SO Number not found in master branch!");

                            QueryExpression queryGetSOLine = new QueryExpression("tss_sopartlines");
                            queryGetSOLine.ColumnSet = new ColumnSet(true);
                            queryGetSOLine.Criteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.Or,
                                Conditions =
                                    {
                                        new ConditionExpression("tss_sopartheaderid", ConditionOperator.Equal, SO_Header.Id)
                                        //new ConditionExpression("tss_partnumber", ConditionOperator.Equal, part.Id)
                                    }
                            };

                            EntityCollection SO_Lines = organizationService.RetrieveMultiple(queryGetSOLine);

                            QueryExpression queryGetPartlinesInter = new QueryExpression("tss_partmasterlinesinterchange");
                            queryGetPartlinesInter.ColumnSet = new ColumnSet(true);
                            queryGetPartlinesInter.Criteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.And,
                                Conditions =
                                {
                                    new ConditionExpression("tss_partnumberinterchange", ConditionOperator.Equal, part.Id),
                                    //new ConditionExpression("tss_status", ConditionOperator.Equal, PartInterchange_STATUS_ACTIVE)//status = active
                                }
                            };
                            EntityCollection PartlineInters = organizationService.RetrieveMultiple(queryGetPartlinesInter);
                            List<Entity> SOLines = new List<Entity>();
                            Entity SO_Line = new Entity();

                            if (PartlineInters.Entities.Count > 0)
                            {
                                SOLines = SO_Lines.Entities.Where(o => o.GetAttributeValue<bool>("tss_isinterchange") == true && o.GetAttributeValue<EntityReference>("tss_partnumberinterchange").Id == PartlineInters.Entities[0].Id).ToList();
                                SO_Line = SOLines.FirstOrDefault();
                                if (SO_Line == null)
                                    SO_Line = SO_Lines.Entities.Where(o => o.GetAttributeValue<EntityReference>("tss_partnumber").Id == part.Id).FirstOrDefault();
                            }
                            else
                            {
                                SOLines = SO_Lines.Entities.Where(o => o.GetAttributeValue<EntityReference>("tss_partnumber").Id == part.Id).ToList();
                                SO_Line = SOLines.FirstOrDefault();
                            }

                            if (SO_Line != null)
                            {
                                QueryExpression queryGetSOSubLine = new QueryExpression("tss_salesorderpartsublines");
                                queryGetSOSubLine.ColumnSet = new ColumnSet(true);
                                queryGetSOSubLine.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("tss_name", ConditionOperator.Equal, PartNumber),
                                        new ConditionExpression("tss_saptono", ConditionOperator.Equal, TO_NO_SAP),
                                        new ConditionExpression("tss_salesorderpartlines", ConditionOperator.Equal, SO_Line.Id),
                                        new ConditionExpression("tss_qtyavailablebranch", ConditionOperator.Equal, Branch.Id)
                                    }
                                };
                                EntityCollection SO_Sublines = organizationService.RetrieveMultiple(queryGetSOSubLine);

                                if (SO_Sublines.Entities.Count > 0)
                                {
                                    #region update SO subline

                                    Entity SO_Subline = SO_Sublines.Entities.First();
                                    //DateTime
                                    if (Conf_TO_Date != DateTime.MinValue) { SO_Subline["tss_toconfirmdate"] = ConvertDateTime(Conf_TO_Date); }
                                    _mwsLog.Write(MethodBase.GetCurrentMethod().Name, Conf_TO_Date.ToString(), MWSLog.LogType.Information, MWSLog.Source.Inbound);
                                    if (Conf_TO_Time != DateTime.MinValue) { SO_Subline["tss_toconfirmtime"] = ConvertDateTime(Conf_TO_Time); }
                                    _mwsLog.Write(MethodBase.GetCurrentMethod().Name, Conf_TO_Time.ToString(), MWSLog.LogType.Information, MWSLog.Source.Inbound);
                                    //String
                                    if (Conf_TO_Status != null) { SO_Subline["tss_toconfirmstatus"] = Conf_TO_Status; }
                                    SO_Subline["tss_transactiontype"] = new OptionSetValue(SUBLINE_TRANSCTTYPE_CONFIRMTO);
                                    confirmTOs.Entities.Add(SO_Subline);

                                    if (updateSOs.Entities.Count > 0)
                                    {
                                        if (!updateSOs.Entities.Any(d => d.Id == SO_Header.Id))
                                        {
                                            if (SO_Header.Contains("tss_isallowcancel"))
                                            {
                                                if (!SO_Header.GetAttributeValue<bool>("tss_isallowcancel"))
                                                {
                                                    SO_Header["tss_isallowcancel"] = true; // terbalik di naming saja tetapi valuenya benar, maksudnya tss is not allow 
                                                    updateSOs.Entities.Add(SO_Header);
                                                }
                                            }
                                            else
                                            {
                                                SO_Header["tss_isallowcancel"] = true; // terbalik di naming saja tetapi valuenya benar, maksudnya tss is not allow 
                                                updateSOs.Entities.Add(SO_Header);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (SO_Header.Contains("tss_isallowcancel"))
                                        {
                                            if (!SO_Header.GetAttributeValue<bool>("tss_isallowcancel")) 
                                            {
                                                SO_Header["tss_isallowcancel"] = true; // terbalik di naming saja tetapi valuenya benar, maksudnya tss is not allow 
                                                updateSOs.Entities.Add(SO_Header);
                                            }
                                        }
                                        else
                                        {
                                            SO_Header["tss_isallowcancel"] = true;
                                            updateSOs.Entities.Add(SO_Header);
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    throw new Exception("SO subline not found!");
                                }
                            }
                            else
                            {
                                throw new Exception("SO line not found!");
                            }
                        }
                        #endregion
                        #region update SO subline in field createTO
                        if (confirmTOs.Entities.Count > 0)
                        {
                            foreach (var confirmTO in confirmTOs.Entities)
                            {
                                organizationService.Update(confirmTO);
                            }
                        }
                        if (updateSOs.Entities.Count > 0)
                        {
                            foreach (var updateSO in updateSOs.Entities)
                            {
                                organizationService.Update(updateSO);
                            }
                        }

                        response.Result = "Success";
                        response.ErrorDescription = "Success to Confirm Transfer Order on Sales Order Sub Lines on CRM.";
                        response.SyncDate = DateTime.Now;
                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, TO_NO_SAP + _mwsLog.ColumnsSeparator
                             + response.Result, MWSLog.LogType.Information, MWSLog.Source.Inbound);
                        #endregion
                    }
                    else
                    {
                        throw new Exception("Access denied invalid token!");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = "Failed";
                response.ErrorDescription = "Failed to Confirm Transfer Order on Sales Order Sub Lines on CRM. Details : " + ex.Message;
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator
                                 + "Error : " + ex.Message.ToString(), MWSLog.LogType.Error, MWSLog.Source.Inbound);
            }
            return response;
        }
        #endregion

        #region Product & Part Stock
        public CRM_WS_Response UpdatePartStock(string token, ParamPartstockUpdate[] stocks, DateTime date, DateTime time)
        {
            CRM_WS_Response response = new CRM_WS_Response();
            try
            {
                DateTime CheckStockDate = date;
                CheckStockDate = CheckStockDate.AddHours(time.Hour);
                CheckStockDate = CheckStockDate.AddMinutes(time.Minute);
                CheckStockDate = CheckStockDate.AddSeconds(time.Second);

                string _connectionString = string.Empty;

                if (IsValidConnectionString(ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString))
                    _connectionString = ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString;

                using (CrmServiceClient conn = new CrmServiceClient(_connectionString))
                {
                    IOrganizationService organizationService = (IOrganizationService)conn.OrganizationServiceProxy;

                    string Primary_SP_NO = stocks[0].Sparepart_NO;
                    string generateToken = Encrypt(Primary_SP_NO, uniqueKey);
                    if (generateToken == token)
                    {
                        EntityCollection updatePartStocks = new EntityCollection();
                        EntityCollection createPartStocks = new EntityCollection();

                        #region looping partstock params
                        for (int x = 0; x < stocks.Length; x++)
                        {
                            ParamPartstockUpdate stock = stocks[x];
                            if (!string.IsNullOrWhiteSpace(stock.Sparepart_NO) && !string.IsNullOrWhiteSpace(stock.Branch))
                            {
                                QueryExpression queryGetPart = new QueryExpression("trs_masterpart");
                                queryGetPart.ColumnSet = new ColumnSet(true);
                                queryGetPart.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("trs_name", ConditionOperator.Equal, stock.Sparepart_NO)
                                    }
                                };
                                Entity part = organizationService.RetrieveMultiple(queryGetPart).Entities.FirstOrDefault();

                                QueryExpression queryGetBranch = new QueryExpression("businessunit");
                                queryGetBranch.ColumnSet = new ColumnSet(true);
                                queryGetBranch.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("trs_branchcode", ConditionOperator.Equal, stock.Branch),
                                        new ConditionExpression("name", ConditionOperator.Equal, stock.Branch)
                                    }
                                };
                                Entity branch = organizationService.RetrieveMultiple(queryGetBranch).Entities.FirstOrDefault();

                                if (part == null)
                                    throw new Exception("Partnumber not found in part master!");
                                if (branch == null)
                                    throw new Exception("Branch not found in master branch!");

                                QueryExpression queryGetPartstock = new QueryExpression("trs_partstock");
                                queryGetPartstock.ColumnSet = new ColumnSet(true);
                                queryGetPartstock.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("trs_partnumber", ConditionOperator.Equal, part.Id),
                                        new ConditionExpression("trs_branch", ConditionOperator.Equal, branch.Id)
                                    }
                                };
                                EntityCollection Partstocks = organizationService.RetrieveMultiple(queryGetPartstock);
                                if (Partstocks.Entities.Count > 0)
                                {
                                    Entity Partstock = Partstocks.Entities[0];
                                    Partstock["trs_quantity"] = (decimal)stock.StockQty;
                                    if (CheckStockDate != DateTime.MinValue) Partstock["tss_checkstockdate"] = ConvertDateTime(CheckStockDate);
                                    _mwsLog.Write(MethodBase.GetCurrentMethod().Name, CheckStockDate.ToString(), MWSLog.LogType.Information, MWSLog.Source.Inbound);
                                    updatePartStocks.Entities.Add(Partstock);
                                }
                                else
                                {
                                    Entity Partstock = new Entity("trs_partstock");
                                    Partstock["trs_partnumber"] = part.ToEntityReference();
                                    Partstock["trs_branch"] = branch.ToEntityReference();
                                    Partstock["trs_quantity"] = (decimal)stock.StockQty;
                                    if (CheckStockDate != DateTime.MinValue) Partstock["tss_checkstockdate"] = ConvertDateTime(CheckStockDate);
                                    _mwsLog.Write(MethodBase.GetCurrentMethod().Name, CheckStockDate.ToString(), MWSLog.LogType.Information, MWSLog.Source.Inbound);
                                    createPartStocks.Entities.Add(Partstock);
                                }
                            }
                            else
                            {
                                throw new Exception("Parameter Sparepart Number / Branch is Empty!");
                            }
                        }
                        #endregion
                        #region update partstock
                        if (updatePartStocks.Entities.Count > 0)
                        {
                            foreach (var partStock in updatePartStocks.Entities)
                            {
                                organizationService.Update(partStock);
                            }
                        }
                        if (createPartStocks.Entities.Count > 0)
                        {
                            foreach (var createStock in createPartStocks.Entities)
                            {
                                organizationService.Create(createStock);
                            }
                        }
                        response.Result = "Success";
                        response.ErrorDescription = "Success to Create / Update Part Stock on CRM.";
                        response.SyncDate = DateTime.Now;
                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, Primary_SP_NO + _mwsLog.ColumnsSeparator
                                 + response.Result, MWSLog.LogType.Information, MWSLog.Source.Inbound);
                        #endregion
                    }
                    else
                    {
                        throw new Exception("Access denied invalid token!");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = "Failed";
                response.ErrorDescription = "Failed to Create / Update Part Stock on CRM. Details : " + ex.Message;
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator
                             + "Error : " + response.ErrorDescription, MWSLog.LogType.Error, MWSLog.Source.Inbound);
            }
            return response;
        }

        public CRM_WS_Response CreateProductMaster(string token, ParamCreateProductMaster[] products)
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

                    string Primary_SP_NO = products[0].Sparepart_NO;
                    string generateToken = Encrypt(Primary_SP_NO, uniqueKey);
                    if (generateToken == token)
                    {
                        EntityCollection createParts = new EntityCollection();

                        #region looping productmasters params
                        for (int x = 0; x < products.Length; x++)
                        {
                            ParamCreateProductMaster product = products[x];
                            if (!string.IsNullOrWhiteSpace(product.Sparepart_NO) && !string.IsNullOrWhiteSpace(product.UnitGroup) && !string.IsNullOrWhiteSpace(product.Product) && !string.IsNullOrWhiteSpace(product.UoM))
                            {
                                QueryExpression queryGetUoM = new QueryExpression("trs_unitofmeasurement");
                                queryGetUoM.ColumnSet = new ColumnSet(true);
                                queryGetUoM.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("trs_uofmname", ConditionOperator.Equal, product.UoM)
                                    }
                                };
                                Entity UoM = organizationService.RetrieveMultiple(queryGetUoM).Entities.FirstOrDefault();

                                if (UoM == null)
                                    throw new Exception("Unit measurement name not found in master unit of measurement!");

                                QueryExpression queryGetUnitGroup = new QueryExpression("new_materialtype");
                                queryGetUnitGroup.ColumnSet = new ColumnSet(true);
                                queryGetUnitGroup.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("new_materialtypecode", ConditionOperator.Equal, product.UnitGroup)
                                    }
                                };
                                Entity UnitGroup = organizationService.RetrieveMultiple(queryGetUnitGroup).Entities.FirstOrDefault();

                                if (UnitGroup == null)
                                    throw new Exception("Unit group not found in master material type!");

                                QueryExpression queryGetMatGroup = new QueryExpression("new_materialgroup");
                                queryGetMatGroup.ColumnSet = new ColumnSet(true);
                                queryGetMatGroup.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("new_mgcode", ConditionOperator.Equal, product.Product)
                                    }
                                };
                                Entity MatGroup = organizationService.RetrieveMultiple(queryGetMatGroup).Entities.FirstOrDefault();

                                if (MatGroup == null)
                                    throw new Exception("Material group not found in master material group!");

                                QueryExpression queryGetPart = new QueryExpression("trs_masterpart");
                                queryGetPart.ColumnSet = new ColumnSet(true);
                                queryGetPart.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("trs_name", ConditionOperator.Equal, product.Sparepart_NO)
                                    }
                                };
                                Entity existingpart = organizationService.RetrieveMultiple(queryGetPart).Entities.FirstOrDefault();
                                if (existingpart != null)
                                    throw new Exception("Current partnumber is already exist in part master!");

                                Entity part = new Entity("trs_masterpart");
                                part["trs_name"] = product.Sparepart_NO;
                                if (product.Description != null) part["trs_partdescription"] = product.Description;
                                part["trs_unitmeasurement"] = new EntityReference("trs_unitofmeasurement", UoM.Id); //lookup
                                part["trs_materialtype"] = new EntityReference("new_materialtype", UnitGroup.Id); //lookup
                                part["trs_materialgroup"] = new EntityReference("new_materialgroup", MatGroup.Id); //lookup
                                createParts.Entities.Add(part);
                            }
                            else
                            {
                                throw new Exception("Access denied invalid token!");
                            }
                        }
                        #endregion
                        #region update productmasters
                        if (createParts.Entities.Count > 0)
                        {
                            foreach (var createPart in createParts.Entities)
                            {
                                organizationService.Create(createPart);
                            }
                        }

                        response.Result = "Success";
                        response.ErrorDescription = "Success to Create Product Master on CRM.";
                        response.SyncDate = DateTime.Now;
                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, Primary_SP_NO + _mwsLog.ColumnsSeparator
                                 + response.Result, MWSLog.LogType.Information, MWSLog.Source.Inbound);
                        #endregion
                    }
                    else
                    {
                        throw new Exception("Parameter Sparepart Number / Branch is Empty!");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = "Failed";
                response.ErrorDescription = "Failed to Create Product Master on CRM. Details : " + ex.Message;
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator
                             + "Error : " + response.ErrorDescription, MWSLog.LogType.Error, MWSLog.Source.Inbound);
            }
            return response;
        }

        public CRM_WS_Response UpdateProductMasterInterchange(string token, ParamUpdateProductMasterInterchange[] products)
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

                    string Primary_SP_NO = products[0].Sparepart_NO;
                    string generateToken = Encrypt(Primary_SP_NO, uniqueKey);
                    if (generateToken == token)
                    {
                        EntityCollection createPartsInter = new EntityCollection();
                        EntityCollection updatePartsInter = new EntityCollection();
                        EntityCollection updateParts = new EntityCollection();

                        #region looping productmasters params
                        for (int x = 0; x < products.Length; x++)
                        {
                            ParamUpdateProductMasterInterchange product = products[x];
                            if (!string.IsNullOrWhiteSpace(product.Sparepart_NO) && !string.IsNullOrWhiteSpace(product.Sparepart_NO_Interchange))
                            {
                                QueryExpression queryGetPart = new QueryExpression("trs_masterpart");
                                queryGetPart.ColumnSet = new ColumnSet(true);
                                queryGetPart.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("trs_name", ConditionOperator.Equal, product.Sparepart_NO)
                                    }
                                };
                                Entity part = organizationService.RetrieveMultiple(queryGetPart).Entities.FirstOrDefault();

                                if (part == null)
                                    throw new Exception("Partnumber not found in part master!");

                                QueryExpression queryGetPartInter = new QueryExpression("trs_masterpart");
                                queryGetPartInter.ColumnSet = new ColumnSet(true);
                                queryGetPartInter.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                        {
                                            new ConditionExpression("trs_name", ConditionOperator.Equal, product.Sparepart_NO_Interchange)
                                        }
                                };
                                Entity partInter = organizationService.RetrieveMultiple(queryGetPartInter).Entities.FirstOrDefault();

                                if (partInter == null)
                                    throw new Exception("Partnumber interchange not found in part master!");

                                QueryExpression queryGetPartlinesInter = new QueryExpression("tss_partmasterlinesinterchange");
                                queryGetPartlinesInter.ColumnSet = new ColumnSet(true);
                                queryGetPartlinesInter.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("tss_partnumberinterchange", ConditionOperator.Equal, partInter.Id),
                                        new ConditionExpression("tss_status", ConditionOperator.Equal, PartInterchange_STATUS_ACTIVE)
                                    }
                                };
                                EntityCollection partlineinters = organizationService.RetrieveMultiple(queryGetPartlinesInter);
                                //if found part interchange with current part interchange number then update them and change status to deactive
                                if (partlineinters.Entities.Count > 0)
                                {
                                    //add validation to Update sparepart number to inactive and add end date.
                                    foreach (var currentpartlineinter in partlineinters.Entities)
                                    {
                                        if (product.Start_Interchange.Date != DateTime.MaxValue.Date) currentpartlineinter["tss_enddate"] = ConvertDateTime(product.Start_Interchange);
                                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, product.Start_Interchange.ToString(), MWSLog.LogType.Information, MWSLog.Source.Inbound);
                                        currentpartlineinter["tss_status"] = new OptionSetValue(PartInterchange_STATUS_DEACTIVE);
                                        updatePartsInter.Entities.Add(currentpartlineinter);
                                    }
                                    //throw new Exception("Sparepart Number Interchange already used in another Sparepart!");
                                }
                                //else
                                //{
                                //}
                                if (partInter.Contains("tss_ispartinterchange"))
                                {
                                    if (!partInter.GetAttributeValue<bool>("tss_ispartinterchange"))
                                    {
                                        partInter["tss_ispartinterchange"] = true;
                                        updateParts.Entities.Add(partInter);
                                    }
                                }
                                else
                                {
                                    partInter["tss_ispartinterchange"] = true;
                                    updateParts.Entities.Add(partInter);
                                }

                                //create new record based on parameters
                                Entity partlineinter = new Entity("tss_partmasterlinesinterchange");
                                partlineinter["tss_partnumber"] = product.Sparepart_NO_Interchange;
                                partlineinter["tss_partnumberinterchange"] = new EntityReference("trs_masterpart", partInter.Id); //lookup ke Part Master interchange
                                partlineinter["tss_partmasterid"] = new EntityReference("trs_masterpart", part.Id); //lookup ke Part Master - relationship
                                if (product.Start_Interchange.Date != DateTime.MinValue.Date) partlineinter["tss_startdate"] = ConvertDateTime(product.Start_Interchange);
                                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, product.Start_Interchange.ToString(), MWSLog.LogType.Information, MWSLog.Source.Inbound);
                                if (product.End_Interchange.Date != DateTime.MaxValue.Date) partlineinter["tss_enddate"] = ConvertDateTime(product.End_Interchange);
                                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, product.End_Interchange.ToString(), MWSLog.LogType.Information, MWSLog.Source.Inbound);
                                partlineinter["tss_status"] = new OptionSetValue(PartInterchange_STATUS_ACTIVE);
                                createPartsInter.Entities.Add(partlineinter);
                            }
                            else
                            {
                                throw new Exception("Parameter Sparepart Number / Interchange is Empty!");
                            }
                        }
                        #endregion
                        #region update productmasters

                        if (updateParts.Entities.Count > 0)
                        {
                            foreach (var updatePart in updateParts.Entities)
                            {
                                organizationService.Update(updatePart);
                            }
                        }
                        if (updatePartsInter.Entities.Count > 0)
                        {
                            foreach (var updatePartInter in updatePartsInter.Entities)
                            {
                                organizationService.Update(updatePartInter);
                            }
                        }
                        if (createPartsInter.Entities.Count > 0)
                        {
                            foreach (var createPartInter in createPartsInter.Entities)
                            {
                                organizationService.Create(createPartInter);
                            }
                        }

                        response.Result = "Success";
                        response.ErrorDescription = "Success to Update Product Master Intechange on CRM.";
                        response.SyncDate = DateTime.Now;
                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, Primary_SP_NO + _mwsLog.ColumnsSeparator
                                 + response.Result, MWSLog.LogType.Information, MWSLog.Source.Inbound);
                        #endregion
                    }
                    else
                    {
                        throw new Exception("Access denied invalid token!");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = "Failed";
                response.ErrorDescription = "Failed to Update Product Master Intechange on CRM. Details : " + ex.Message;
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator
                             + "Error : " + response.ErrorDescription, MWSLog.LogType.Error, MWSLog.Source.Inbound);
            }
            return response;
        }
        #endregion

        #region AR Balance
        public CRM_WS_Response UpdateARBalanceCustomer(string token, ParamOutstandingARCustomer[] arbalances)
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

                    string CUST_ACC_NO = arbalances[0].CUST_ACC_NO;
                    string generateToken = Encrypt(CUST_ACC_NO, uniqueKey);
                    if (generateToken == token)
                    {
                        #region looping productmasters params
                        for (int x = 0; x < arbalances.Length; x++)
                        {
                            ParamOutstandingARCustomer arbalance = arbalances[x];
                            if (!string.IsNullOrWhiteSpace(arbalance.CUST_ACC_NO))
                            {
                                #region UpdateARBalance
                                QueryExpression queryGetCustomer = new QueryExpression("account");
                                queryGetCustomer.ColumnSet = new ColumnSet(true);
                                queryGetCustomer.Criteria.AddCondition("accountnumber", ConditionOperator.Equal, CUST_ACC_NO);
                                Entity Customer = organizationService.RetrieveMultiple(queryGetCustomer).Entities.FirstOrDefault();

                                if (Customer == null)
                                    throw new Exception("Customer with current account number not found!");
                                else
                                {
                                    var Customers = new Entity("account");
                                    //SAP ONLY SEND CURRENT AMOUNT, OVERDUE AMOUNT, AND OVERDUE DAYS TO CRM
                                    //CRM WILL CALCULATE BALANCE = PLAFON (FROM CRM) - CURRENT (FROM SAP)
                                    #region MF
                                    Customers["new_currentarmf"] = arbalance.MF_Current_Amt.ToString("#,##0");
                                    Customers["new_overduearmf"] = arbalance.MF_Overdue_Amt.ToString("#,##0");
                                    Customers["tss_overduedaysarmf"] = arbalance.MF_Overdue_Days.ToString("#,##0");
                                    Customers["new_balancearmf"] = CalculateBalanceAR(Customer.GetAttributeValue<String>("tss_plafondarmf"), arbalance.MF_Current_Amt.ToString());
                                    #endregion

                                    #region PER
                                    Customers["new_currentarper"] = arbalance.PER_Current_Amt.ToString("#,##0");
                                    Customers["new_overduearper"] = arbalance.PER_Overdue_Amt.ToString("#,##0");
                                    Customers["tss_overduedaysarper"] = arbalance.PER_Overdue_Days.ToString("#,##0");
                                    Customers["new_balancearper"] = CalculateBalanceAR(Customer.GetAttributeValue<String>("tss_plafondarper"), arbalance.PER_Current_Amt.ToString());
                                    #endregion

                                    #region TYT
                                    Customers["new_currentartyt"] = arbalance.TYT_Current_Amt.ToString("#,##0");
                                    Customers["new_overdueartyt"] = arbalance.TYT_Overdue_Amt.ToString("#,##0");
                                    Customers["tss_overduedaysartyt"] = arbalance.TYT_Overdue_Days.ToString("#,##0");
                                    Customers["new_balanceartyt"] = CalculateBalanceAR(Customer.GetAttributeValue<String>("tss_plafondartyt"), arbalance.TYT_Current_Amt.ToString());
                                    #endregion

                                    #region SMV
                                    Customers["new_currentarstm"] = arbalance.STM_Current_Amt.ToString("#,##0");
                                    Customers["new_overduearstm"] = arbalance.STM_Overdue_Amt.ToString("#,##0");
                                    Customers["tss_overduedaysarstm"] = arbalance.STM_Overdue_Days.ToString("#,##0");
                                    Customers["new_balancearstm"] = CalculateBalanceAR(Customer.GetAttributeValue<String>("tss_plafondarsmv"), arbalance.STM_Current_Amt.ToString());
                                    #endregion

                                    #region SAK
                                    Customers["new_currentarsak"] = arbalance.SAK_Current_Amt.ToString("#,##0");
                                    Customers["new_overduearsak"] = arbalance.SAK_Overdue_Amt.ToString("#,##0");
                                    Customers["tss_overduedaysarsak"] = arbalance.SAK_Overdue_Days.ToString("#,##0");
                                    Customers["new_balancearsak"] = CalculateBalanceAR(Customer.GetAttributeValue<String>("tss_plafondarsak"), arbalance.SAK_Current_Amt.ToString());
                                    #endregion

                                    #region GDN
                                    Customers["new_currentargdn"] = arbalance.GDN_Current_Amt.ToString("#,##0");
                                    Customers["new_overdueargdn"] = arbalance.GDN_Overdue_Amt.ToString("#,##0");
                                    Customers["tss_overduedaysargdn"] = arbalance.GDN_Overdue_Days.ToString("#,##0");
                                    Customers["new_balanceargdn"] = CalculateBalanceAR(Customer.GetAttributeValue<String>("tss_plafondargdn"), arbalance.GDN_Current_Amt.ToString());
                                    #endregion

                                    #region FGW
                                    Customers["new_currentarfgw"] = arbalance.FGW_Current_Amt.ToString("#,##0");
                                    Customers["new_overduearfgw"] = arbalance.FGW_Overdue_Amt.ToString("#,##0");
                                    Customers["tss_overduedaysarfgw"] = arbalance.FGW_Overdue_Days.ToString("#,##0");
                                    Customers["new_balancearfgw"] = CalculateBalanceAR(Customer.GetAttributeValue<String>("tss_plafondarfgw"), arbalance.FGW_Current_Amt.ToString());
                                    #endregion

                                    #region BTF
                                    Customers["new_currentarbtf"] = arbalance.BTF_Current_Amt.ToString("#,##0");
                                    Customers["new_overduearbtf"] = arbalance.BTF_Overdue_Amt.ToString("#,##0");
                                    Customers["tss_overduedaysarbtf"] = arbalance.BTF_Overdue_Days.ToString("#,##0");
                                    Customers["new_balancearbtf"] = CalculateBalanceAR(Customer.GetAttributeValue<String>("tss_plafondarbtf"), arbalance.BTF_Current_Amt.ToString());
                                    #endregion

                                    #region SPAREPART
                                    Customers["tss_currentarsparepart"] = arbalance.Sparepart_Current_Amt.ToString("#,##0");
                                    Customers["tss_overduearsp"] = arbalance.Sparepart_Overdue_Amt.ToString("#,##0");
                                    Customers["tss_overduedaysarsp"] = arbalance.Sparepart_Overdue_Days.ToString("#,##0");
                                    Customers["tss_balancearsparepart"] = CalculateBalanceAR((Customer.GetAttributeValue<int>("tss_plafondarsparepart")).ToString(), arbalance.Sparepart_Current_Amt.ToString());
                                    #endregion

                                    #region SERVICE
                                    Customers["tss_currentsv"] = arbalance.Service_Current_Amt.ToString("#,##0");
                                    Customers["tss_overduearsv"] = arbalance.Service_Overdue_Amt.ToString("#,##0");
                                    Customers["tss_overduedaysarsv"] = arbalance.Service_Overdue_Days.ToString("#,##0");
                                    Customers["tss_balancearsv"] = CalculateBalanceAR(Customer.GetAttributeValue<String>("tss_plafondarservice"), arbalance.Service_Current_Amt.ToString());
                                    #endregion

                                    #region RENTAL
                                    Customers["tss_currentrental"] = arbalance.Rental_Current_Amt.ToString("#,##0");
                                    Customers["tss_overduearrental"] = arbalance.Rental_Overdue_Amt.ToString("#,##0");
                                    Customers["tss_overduedaysarrental"] = arbalance.Rental_Overdue_Days.ToString("#,##0");
                                    Customers["tss_balancearrental"] = CalculateBalanceAR(Customer.GetAttributeValue<String>("tss_plafondarrental"), arbalance.Rental_Current_Amt.ToString());
                                    #endregion

                                    #region TOTAL AR UNIT
                                    Customers["tss_totalcurrentarunit"] = arbalance.Unit_Current_Amt.ToString("#,##0");
                                    Customers["tss_totaloverduearunit"] = arbalance.Unit_Overdue_Amt.ToString("#,##0");
                                    Customers["tss_totaloverduedaysarunit"] = arbalance.Unit_Overdue_Days.ToString("#,##0");
                                    #endregion

                                    #region Calculate Balance AR = Plafond AR - Current AR
                                    #endregion

                                    Customers.Id = Customer.Id;
                                    organizationService.Update(Customers);

                                    response.Result = "Success";
                                    response.ErrorDescription = "Success to Update Outstanding AR Data on CRM.";
                                    response.SyncDate = DateTime.Now;
                                    _mwsLog.Write(MethodBase.GetCurrentMethod().Name, CUST_ACC_NO + _mwsLog.ColumnsSeparator
                                             + response.Result, MWSLog.LogType.Information, MWSLog.Source.Inbound);
                                }
                                #endregion
                            }
                            else
                            {
                                throw new Exception("Paremeter Customer Account Number is Empty!");
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        throw new Exception("Access denied invalid token!");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = "Failed";
                response.ErrorDescription = "Failed to Update Outstanding AR Data on CRM for Customer : " + arbalances[0].CUST_ACC_NO + ". Details : " + ex.Message;
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, response.Result + _mwsLog.ColumnsSeparator
                             + "Error : " + response.ErrorDescription, MWSLog.LogType.Error, MWSLog.Source.Inbound);
            }

            return response;
        }
        #endregion
        #endregion

        #region Helper
        public DateTime ConvertDateTime(DateTime DateTimeSAP)
        {
            DateTime DatetimeConvert = DateTime.MinValue;
            try
            {
                DatetimeConvert = DateTimeSAP.AddHours(HOURS * -1);
            }
            catch (Exception ex)
            {
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, "Failed" + _mwsLog.ColumnsSeparator
                    + "Error : " + ex.Message + _mwsLog.ColumnsSeparator
                    + " for DateTime : " + DateTimeSAP.ToString(), MWSLog.LogType.Error, MWSLog.Source.Inbound);
            }
            return DatetimeConvert;
        }

        public string CalculateBalanceAR(string PlafondAR, string CurrentAR)
        {
            string BalanceAR = null ;
            try
            {
                if (PlafondAR != null && CurrentAR != null)
                {
                    decimal Plafond_AR = decimal.Parse(PlafondAR);
                    decimal Current_AR = decimal.Parse(CurrentAR);

                    BalanceAR = (Plafond_AR - Current_AR).ToString("#,##0");
                }
            }
            catch (Exception ex)
            {
                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, "Failed" + _mwsLog.ColumnsSeparator
                   + "Error : " + ex.Message + _mwsLog.ColumnsSeparator, MWSLog.LogType.Error, MWSLog.Source.Inbound);
            }

            return BalanceAR;
        }
        #endregion
    }
}
