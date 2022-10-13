using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using TrakNusRapidService.DataLayer;
using TrakNusRapidService.Helper;

namespace TrakNusRapidService.Plugins.BusinessLayer
{
    class BL_serviceappointment
    {
        #region Constants
        private const string _classname = "BL_serviceappoinment";
        private const int _depth = 1;
        private const int _participationtypemask_customer = 11;
        private const int _partyobjecttypecode_account = 1;
        private const string trPartHeader = "H";
        private const string trPartDetail = "D";

        private enum TrType
        {
            Create = 1,
            Change = 2,
            Release = 3,
            AssignMechanic = 4,
            Confirmation = 5,
            PartConsume = 6,
            TECO = 9,
            CancelWO = 0
        }
        #endregion

        #region Depedencies
        private DL_serviceappointment _DL_serviceappointment = new DL_serviceappointment();
        private DL_activityparty _DL_activityparty = new DL_activityparty();
        private DL_trs_tasklistgroup _DL_trs_tasklistgroup = new DL_trs_tasklistgroup();
        private BL_trs_mtar _BL_trs_mtar = new BL_trs_mtar();
        private BL_trs_runningnumber _BL_trs_runningnumber = new BL_trs_runningnumber();
        private DL_new_population _DL_new_population = new DL_new_population();
        private DL_account _DL_account = new DL_account();
        private DL_trs_runningnumber _DL_trs_runningnumber = new DL_trs_runningnumber();
        private DL_trs_runningnumberlastnumber _DL_trs_runningnumberlastnumber = new DL_trs_runningnumberlastnumber();
        private DL_businessunit _DL_businessunit = new DL_businessunit();
        private DL_equipment _DL_equipment = new DL_equipment();
        private DL_task _DL_task = new DL_task();
        private DL_transactioncurrency _DL_transactioncurrency = new DL_transactioncurrency();
        private DL_trs_acttype _DL_trs_acttype = new DL_trs_acttype();
        private DL_trs_functionallocation _DL_trs_functionallocation = new DL_trs_functionallocation();
        private DL_trs_masterpart _DL_trs_masterpart = new DL_trs_masterpart();
        private DL_trs_paymentterm _DL_trs_paymentterm = new DL_trs_paymentterm();
        private DL_trs_profitcenter _DL_trs_profitcenter = new DL_trs_profitcenter();
        private DL_trs_responsiblecostcenter _DL_trs_responsiblecostcenter = new DL_trs_responsiblecostcenter();
        private DL_trs_workcenter _DL_trs_workcenter = new DL_trs_workcenter();
        private DL_trs_workorderpartssummary _DL_trs_workorderpartssummary = new DL_trs_workorderpartssummary();
        private DL_trs_workordersupportingmaterial _DL_trs_workordersupportingmaterial = new DL_trs_workordersupportingmaterial();
        private DL_trs_tsrpartsdamageddetail _DL_trs_tsrpartsdamageddetail = new DL_trs_tsrpartsdamageddetail();
        private DL_trs_tsrpartdetails _DL_trs_tsrpartdetails = new DL_trs_tsrpartdetails();
        private EmailAgent _EmailAgent = new EmailAgent();
        private DL_incident _DL_incident = new DL_incident();
        private DL_trs_workflowconfiguration _DL_trs_workflowconfiguration = new DL_trs_workflowconfiguration();
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        private FSAP _fSAP = new FSAP();
        #endregion

        #region Function
        private EntityCollection _CustomersList = new EntityCollection();

        public void AddCustomer(Guid CustId)
        {
            try
            {
                _DL_activityparty = new DL_activityparty();
                _DL_activityparty.partyid = new EntityReference(_DL_account.EntityName, CustId);
                _CustomersList.Entities.Add(_DL_activityparty.GetEntity());
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".AddSender : " + ex.Message.ToString());
            }
        }
        #endregion

        #region Privates
        private void SendtoMobile(IOrganizationService organizationService, Guid id, int statuscode, bool fromMobile, bool workshop)
        {
            try
            {
                if (!fromMobile && !workshop)
                {
                    FMobile _fMobile = new FMobile(organizationService);
                    switch (statuscode)
                    {
                        case Configuration.WOStatus_Dispatched:
                            _fMobile.SendServiceAppointment(organizationService, id);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendtoMobile : " + ex.Message);
            }
        }

        private void CreateNewLastNumber(IOrganizationService organizationService
            , IPluginExecutionContext pluginExecutionContext, int year, Guid runningNumberId)
        {
            try
            {
                _DL_trs_runningnumberlastnumber = new DL_trs_runningnumberlastnumber();
                _DL_trs_runningnumberlastnumber.trs_year = year.ToString();
                _DL_trs_runningnumberlastnumber.trs_runningnumberid = runningNumberId;
                _DL_trs_runningnumberlastnumber.trs_lastnumber = 0;
                _DL_trs_runningnumberlastnumber.Insert(organizationService);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".CreateNewLastNumber : " + ex.Message.ToString());
            }
        }

        private void GetLastNumber(IOrganizationService organizationService
            , IPluginExecutionContext pluginExecutionContext, int year, Guid runningNumberId
            , out Guid id, out decimal lastNumber, out Guid lockingBy)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_trs_runningnumberlastnumber.EntityName);
                queryExpression.ColumnSet.AddColumns("trs_lastnumber", "trs_lockingby");

                FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                filterExpression.AddCondition("trs_year", ConditionOperator.Equal, year.ToString());
                filterExpression.AddCondition("trs_runningnumberid", ConditionOperator.Equal, runningNumberId);

                EntityCollection entityCollection = _DL_trs_runningnumberlastnumber.Select(organizationService, queryExpression);
                if (entityCollection.Entities.Count > 0)
                {
                    Entity entity = entityCollection.Entities[0];
                    id = entity.Id;
                    lastNumber = Convert.ToDecimal(entity.Attributes["trs_lastnumber"].ToString());
                    if (entity.Attributes.Contains("trs_lockingby"))
                        lockingBy = ((EntityReference)entity.Attributes["trs_lockingby"]).Id;
                    else
                        lockingBy = new Guid();
                }
                else
                {
                    CreateNewLastNumber(organizationService, pluginExecutionContext, year, runningNumberId);
                    GetLastNumber(organizationService, pluginExecutionContext, year, runningNumberId, out id, out lastNumber, out lockingBy);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".GetLastNumber : " + ex.Message.ToString());
            }
        }

        private void LockLastNumber(Guid id, Guid user)
        {
            try
            {
                string query = "UPDATE " + _DL_trs_runningnumberlastnumber.EntityName + "Base" + Convert.ToChar(13);
                query += "SET trs_lockingby = '" + user.ToString() + "'" + Convert.ToChar(13);
                query += "WHERE trs_runningnumberlastnumberId = '" + id.ToString() + "'" + Convert.ToChar(13);
                query += " AND trs_lockingby IS NULL OR trs_lockingby = '" + Guid.Empty.ToString() + "'";

                ODBCConnector odbcConnector = new ODBCConnector();
                odbcConnector.ConnectionName = Configuration.ConnectionNameODBC;
                odbcConnector.ConnectionType = ODBCConnectorType.SqlServer2008;
                odbcConnector.Update(query);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".LockLastNumber : " + ex.Message.ToString());
            }
        }

        private void UnlockLastNumber(Guid id, Guid user)
        {
            try
            {
                string query = "UPDATE " + _DL_trs_runningnumberlastnumber.EntityName + "Base" + Convert.ToChar(13);
                query += "SET trs_lockingby = '" + Guid.Empty.ToString() + "'" + Convert.ToChar(13);
                query += "WHERE trs_runningnumberlastnumberId = '" + id.ToString() + "'" + Convert.ToChar(13);
                query += " AND trs_lockingby = '" + user.ToString() + "'";

                ODBCConnector odbcConnector = new ODBCConnector();
                odbcConnector.ConnectionName = Configuration.ConnectionNameODBC;
                odbcConnector.ConnectionType = ODBCConnectorType.SqlServer2008;
                odbcConnector.Update(query);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".LockLastNumber : " + ex.Message.ToString());
            }
        }

        private void UpdateLastNumber(IOrganizationService organizationService, Guid id, decimal lastNumber)
        {
            try
            {
                _DL_trs_runningnumberlastnumber = new DL_trs_runningnumberlastnumber();
                _DL_trs_runningnumberlastnumber.trs_lastnumber = lastNumber;
                _DL_trs_runningnumberlastnumber.Update(organizationService, id);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".UpdateLastNumber : " + ex.Message.ToString());
            }
        }

        private void ReleasetoSAP(IOrganizationService organizationService, Guid id)
        {
            if (_fSAP.SynchronizetoSAP(organizationService))
            {
                #region create
                string header = string.Empty;
                string operationdetail = string.Empty;
                string operationdetailprice = string.Empty;
                string component = string.Empty;
                string componentprice = string.Empty;
                string release = string.Empty;
                string assignMechanic = string.Empty;

                string currency = string.Empty;
                string termOfPayment = string.Empty;
                string actType = string.Empty;
                string pricePerUnit = string.Empty;
                string discountPercent = string.Empty;
                string materialNo = string.Empty;
                #region header
                Entity enServiceAppointment = _DL_serviceappointment.Select(organizationService, id);
                string trType = ((int)TrType.Create).ToString();
                string trSta = string.Empty;
                string woNumber = string.Empty;
                string plant = string.Empty;
                string businessArea = string.Empty;
                string shorttext = string.Empty;
                string purchaseorder = string.Empty;
                string accindic = string.Empty;
                string workcenter = string.Empty;
                string pmacctype = string.Empty;
                string bscstart = string.Empty;
                string bscend = string.Empty;
                string equipment = string.Empty;
                string functionallocation = string.Empty;
                string customer = string.Empty;
                string responsiblecostcenter = string.Empty;
                string profitcenter = string.Empty;

                //if (enServiceAppointment.Contains("subject"))
                //woNumber = enServiceAppointment.Attributes["subject"].ToString();

                if (enServiceAppointment.Contains("trs_crmwonumber"))
                    woNumber = enServiceAppointment.Attributes["trs_crmwonumber"].ToString();

                if (enServiceAppointment.Contains("trs_plant"))
                {
                    //Entity enBusinessUnit = _DL_businessunit.Select(organizationService, ((EntityReference)enServiceAppointment.Attributes["trs_plant"]).Id);
                    //plant = enBusinessUnit.Attributes["name"].ToString();
                    // EHN -- field trs_plant tidak terbaca 16/12/2014
                    plant = ((EntityReference)enServiceAppointment.Attributes["trs_plant"]).Name;
                }
                if (enServiceAppointment.Contains("trs_branch"))
                    businessArea = ((EntityReference)enServiceAppointment.Attributes["trs_branch"]).Name;
                if (enServiceAppointment.Contains("description"))
                    shorttext = enServiceAppointment.Attributes["description"].ToString();
                if (enServiceAppointment.Contains("trs_ponumber"))
                    purchaseorder = enServiceAppointment.Attributes["trs_ponumber"].ToString();
                if (enServiceAppointment.Contains("trs_accind"))
                {
                    accindic = ((OptionSetValue)enServiceAppointment.Attributes["trs_accind"]).Value.ToString("00");
                }

                if (enServiceAppointment.Contains("trs_workcenter"))
                {
                    EntityReference erWorkcenter = (EntityReference)enServiceAppointment.Attributes["trs_workcenter"];
                    Entity enWorkCenter = _DL_trs_workcenter.Select(organizationService, erWorkcenter.Id);
                    workcenter = enWorkCenter.Contains("trs_workcenter") ? enWorkCenter.Attributes["trs_workcenter"].ToString() : string.Empty;
                }
                if (enServiceAppointment.Contains("trs_pmacttype"))
                {
                    EntityReference erTaskListGroup = (EntityReference)enServiceAppointment.Attributes["trs_pmacttype"];
                    Entity enTaskListGroup = _DL_trs_tasklistgroup.Select(organizationService, erTaskListGroup.Id);
                    pmacctype = enTaskListGroup.Contains("trs_pmacttype") ? enTaskListGroup.Attributes["trs_pmacttype"].ToString() : string.Empty;
                }
                if (enServiceAppointment.Contains("scheduledstart"))
                    bscstart = ((DateTime)enServiceAppointment.Attributes["scheduledstart"]).ToLocalTime().ToString("yyyyMMdd");
                if (enServiceAppointment.Contains("scheduledend"))
                    bscend = ((DateTime)enServiceAppointment.Attributes["scheduledend"]).ToLocalTime().ToString("yyyyMMdd");
                if (enServiceAppointment.Contains("trs_equipment"))
                {
                    EntityReference erEquipment = (EntityReference)enServiceAppointment.Attributes["trs_equipment"];
                    Entity enEquipment = _DL_new_population.Select(organizationService, erEquipment.Id);
                    equipment = enEquipment.Contains("new_serialnumber") ? enEquipment.Attributes["new_serialnumber"].ToString() : string.Empty;
                }
                if (enServiceAppointment.Contains("trs_functionallocation"))
                {
                    EntityReference erFunctionallocation = (EntityReference)enServiceAppointment.Attributes["trs_functionallocation"];
                    Entity enFunctionallocation = _DL_trs_functionallocation.Select(organizationService, erFunctionallocation.Id);
                    functionallocation = enFunctionallocation.Contains("trs_functionalcode") ? enFunctionallocation.Attributes["trs_functionalcode"].ToString() : string.Empty;
                }

                // customer from party list, must have only one customer
                if (enServiceAppointment.Contains("customers"))
                {
                    QueryExpression queryExpression = new QueryExpression(_DL_activityparty.EntityName);
                    queryExpression.ColumnSet = new ColumnSet(true);
                    FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                    filterExpression.AddCondition("activityid", ConditionOperator.Equal, id);
                    filterExpression.AddCondition("participationtypemask", ConditionOperator.Equal, _participationtypemask_customer);
                    filterExpression.AddCondition("partyobjecttypecode", ConditionOperator.Equal, _partyobjecttypecode_account);
                    EntityCollection ecActivityParty = _DL_activityparty.Select(organizationService, queryExpression);

                    if (ecActivityParty.Entities.Count > 0)
                    {
                        Entity enActivityParty = ecActivityParty.Entities[0];
                        if (enActivityParty.Contains("partyid") && enActivityParty.Attributes["partyid"] != null)
                        {
                            Entity enAccount = _DL_account.Select(organizationService, ((EntityReference)enActivityParty.Attributes["partyid"]).Id);
                            customer = enAccount.Contains("accountnumber") ? enAccount.Attributes["accountnumber"].ToString() : string.Empty;
                        }
                    }
                }

                if (enServiceAppointment.Contains("trs_responsiblecctr"))
                {
                    EntityReference erResponsiblecostcenter = (EntityReference)enServiceAppointment.Attributes["trs_responsiblecctr"];
                    Entity enResponsiblecostcenter = _DL_trs_responsiblecostcenter.Select(organizationService, erResponsiblecostcenter.Id);
                    responsiblecostcenter = enResponsiblecostcenter.Contains("trs_costcenter") ? enResponsiblecostcenter.Attributes["trs_costcenter"].ToString() : string.Empty;
                }
                if (enServiceAppointment.Contains("trs_profitcenter"))
                {
                    EntityReference erProfitcenter = (EntityReference)enServiceAppointment.Attributes["trs_profitcenter"];
                    Entity enProfitcenter = _DL_trs_profitcenter.Select(organizationService, erProfitcenter.Id);
                    profitcenter = enProfitcenter.Contains("trs_profitcenter") ? enProfitcenter.Attributes["trs_profitcenter"].ToString() : string.Empty;
                }

                if (enServiceAppointment.Contains("trs_acttype"))
                {
                    Entity enActType = _DL_trs_acttype.Select(organizationService, ((EntityReference)enServiceAppointment["trs_acttype"]).Id);
                    actType = enActType["trs_name"].ToString();
                }

                if (enServiceAppointment.Contains("transactioncurrencyid"))
                {

                    QueryExpression qeCurrency = new QueryExpression("transactioncurrency");
                    qeCurrency.ColumnSet = new ColumnSet(true);
                    FilterExpression feCurrency = qeCurrency.Criteria.AddFilter(LogicalOperator.And);
                    feCurrency.AddCondition("transactioncurrencyid", ConditionOperator.Equal, ((EntityReference)enServiceAppointment["transactioncurrencyid"]).Id);
                    EntityCollection ecCurrency = _DL_transactioncurrency.Select(organizationService, qeCurrency);
                    if (ecCurrency.Entities.Count > 0)
                    {
                        currency = ecCurrency.Entities[0]["currencyname"].ToString();
                    }
                }
                if (enServiceAppointment.Contains("trs_paymentterm"))
                {
                    Entity enPaymentTerm = _DL_trs_paymentterm.Select(organizationService, ((EntityReference)enServiceAppointment["trs_paymentterm"]).Id);
                    termOfPayment = enPaymentTerm.Contains("trs_name") ? enPaymentTerm["trs_name"].ToString() : string.Empty;
                }

                header = ((int)TrType.Create).ToString() + "|" + trSta + "|" + trPartHeader + "|" + woNumber + "|" + plant + "|" + businessArea + "|" + shorttext + "|" + purchaseorder
                        + "|" + accindic + "|" + workcenter + "|" + plant + "|" + pmacctype + "|" + bscstart + "|" + bscend
                        + "|" + equipment + "|" + functionallocation + "|" + customer + "|" + responsiblecostcenter + "|" + profitcenter;

                #endregion
                #region operation/commercial header
                int itemCountOperation = 10;
                string opDetType = "O";
                string opItemNo = string.Empty;
                string opShortText = string.Empty;
                string stdTextKey = string.Empty;
                string work = string.Empty;
                string unit = "H";
                string duration = string.Empty;
                #region commercial header
                QueryExpression qeCommercialHeader = new QueryExpression(_DL_task.EntityName);
                qeCommercialHeader.ColumnSet = new ColumnSet(true);
                FilterExpression feCommercialHeader = qeCommercialHeader.Criteria.AddFilter(LogicalOperator.And);
                feCommercialHeader.AddCondition("trs_operationid", ConditionOperator.Equal, id);
                EntityCollection ecCommercialHeader = _DL_task.Select(organizationService, qeCommercialHeader);

                foreach (Entity enCommercialHeader in ecCommercialHeader.Entities)
                {
                    pricePerUnit = string.Empty;
                    discountPercent = string.Empty;
                    materialNo = string.Empty;
                    opItemNo = itemCountOperation.ToString();
                    if (operationdetailprice != string.Empty)
                        operationdetailprice += Environment.NewLine;
                    if (enCommercialHeader.Contains("subject"))
                        opShortText = enCommercialHeader["subject"].ToString();
                    //if (enCommercialHeader.Contains("trs_totalrtg"))
                    //    work = ((decimal)enCommercialHeader["trs_totalrtg"]).ToString("0.##");
                    //operation quantity hardcode 1
                    work = "1";
                    if (enCommercialHeader.Contains("trs_totalprice"))
                        pricePerUnit = ((Money)enCommercialHeader["trs_totalprice"]).Value.ToString("0.##");
                    //if (enCommercialHeader.Contains("trs_discountpercent"))
                    //    discountPercent = ((decimal)enCommercialHeader["trs_discountpercent"]).ToString("0.##");

                    operationdetail += Environment.NewLine + ((int)TrType.Create).ToString() + "|" + trSta + "|" + trPartDetail + "|" + woNumber
                        + "|" + opDetType + "|" + opItemNo + "|" + opShortText + "|" + stdTextKey + "|" + work + "|" + unit
                        + "|" + duration + "|" + unit + "|" + actType;

                    operationdetailprice += woNumber + "|" + opDetType + "|" + opItemNo + "|" + materialNo + "|" + opShortText
                        + "|" + work + "|" + unit + "|" + pricePerUnit + "|" + discountPercent + "|" + currency + "|" + termOfPayment;

                    // update item number
                    _DL_task = new DL_task();
                    _DL_task.trs_itemnumber = itemCountOperation;
                    _DL_task.Update(organizationService, enCommercialHeader.Id);
                    itemCountOperation += 10;
                }
                #endregion
                #region supporting material
                QueryExpression qeWOSupportingMaterialOtherService = new QueryExpression(_DL_trs_workordersupportingmaterial.EntityName);
                qeWOSupportingMaterialOtherService.ColumnSet = new ColumnSet(true);
                FilterExpression feWOSupportingMaterialOtherService = qeWOSupportingMaterialOtherService.Criteria.AddFilter(LogicalOperator.And);
                feWOSupportingMaterialOtherService.AddCondition("trs_workorderid", ConditionOperator.Equal, id);
                feWOSupportingMaterialOtherService.AddCondition("trs_supportingmaterialtype", ConditionOperator.Equal, false);
                EntityCollection ecWOSupportingMaterialOtherService = _DL_trs_workordersupportingmaterial.Select(organizationService, qeWOSupportingMaterialOtherService);

                foreach (Entity enWOSupportingMaterial in ecWOSupportingMaterialOtherService.Entities)
                {
                    pricePerUnit = string.Empty;
                    discountPercent = string.Empty;
                    materialNo = string.Empty;
                    opItemNo = itemCountOperation.ToString();
                    if (enWOSupportingMaterial.Contains("trs_supportingmaterialname"))
                        opShortText = enWOSupportingMaterial["trs_supportingmaterialname"].ToString();
                    if (enWOSupportingMaterial.Contains("trs_standardtext"))
                        stdTextKey = OptionSetExtractor.GetOptionSetText(organizationService, enWOSupportingMaterial.LogicalName,
                            "trs_standardtext", ((OptionSetValue)enWOSupportingMaterial["trs_standardtext"]).Value);
                    //if (enWOSupportingMaterial.Contains("trs_quantity"))
                    //    work = enWOSupportingMaterial["trs_quantity"].ToString();
                    //operation quantity hardcode 1
                    work = "1";
                    if (enWOSupportingMaterial.Contains("trs_totalprice"))
                        pricePerUnit = ((Money)enWOSupportingMaterial["trs_totalprice"]).Value.ToString("0.##");

                    operationdetail += Environment.NewLine + ((int)TrType.Create).ToString() + "|" + trSta + "|" + trPartDetail + "|" + woNumber
                        + "|" + opDetType + "|" + opItemNo + "|" + opShortText + "|" + stdTextKey + "|" + work + "|" + unit
                        + "|" + duration + "|" + unit + "|" + actType;

                    operationdetailprice += Environment.NewLine + woNumber + "|" + opDetType + "|" + opItemNo + "|" + materialNo + "|" + opShortText
                        + "|" + work + "|" + unit + "|" + pricePerUnit + "|" + discountPercent + "|" + currency + "|" + termOfPayment;

                    // update item number
                    _DL_trs_workordersupportingmaterial = new DL_trs_workordersupportingmaterial();
                    _DL_trs_workordersupportingmaterial.trs_itemnumber = itemCountOperation;
                    _DL_trs_workordersupportingmaterial.Update(organizationService, enWOSupportingMaterial.Id);
                    itemCountOperation += 10;
                }
                #endregion
                #endregion
                #region component detail
                int itemCountComponent = 10;
                int compTaskListQuantity = 0;
                int compManualQuantity = 0;
                int compTotalQuantity = 0;
                string compDetType = "C";
                string compItemNo = string.Empty;
                string compMaterialDesc = string.Empty;
                string compReqQty = string.Empty;
                string compUM = string.Empty;
                string compPrice = string.Empty;
                string compSloc = string.Empty;
                string compOpertnNo = "10";
                #region part summary
                unit = "PC";
                QueryExpression qeWOPartSummary = new QueryExpression(_DL_trs_workorderpartssummary.EntityName);
                qeWOPartSummary.ColumnSet = new ColumnSet(true);
                FilterExpression feWOPartSummary = qeWOPartSummary.Criteria.AddFilter(LogicalOperator.And);
                feWOPartSummary.AddCondition("trs_workorder", ConditionOperator.Equal, id);
                EntityCollection ecWOPartSummary = _DL_trs_workorderpartssummary.Select(organizationService, qeWOPartSummary);

                foreach (Entity enWOPartSummary in ecWOPartSummary.Entities)
                {
                    pricePerUnit = string.Empty;
                    discountPercent = string.Empty;
                    materialNo = string.Empty;
                    compItemNo = itemCountComponent.ToString();
                    if (enWOPartSummary.Contains("trs_partnumber"))
                    {
                        EntityReference erMasterPart = (EntityReference)enWOPartSummary["trs_partnumber"];
                        Entity enMasterPart = _DL_trs_masterpart.Select(organizationService, erMasterPart.Id);
                        materialNo = enMasterPart.Contains("trs_name") ? enMasterPart.Attributes["trs_name"].ToString() : string.Empty;
                    }
                    if (enWOPartSummary.Contains("trs_tasklistquantity"))
                        compTaskListQuantity = Convert.ToInt32(enWOPartSummary["trs_tasklistquantity"]);

                    if (enWOPartSummary.Contains("trs_manualquantity"))
                        compManualQuantity = Convert.ToInt32(enWOPartSummary["trs_manualquantity"]);

                    compTotalQuantity = compTaskListQuantity + compManualQuantity;
                    compReqQty = compTotalQuantity.ToString();

                    if (enWOPartSummary.Contains("trs_price"))
                        pricePerUnit = ((Money)enWOPartSummary["trs_price"]).Value.ToString("0.##");
                    if (enWOPartSummary.Contains("trs_discountpercent"))
                        discountPercent = ((decimal)enWOPartSummary["trs_discountpercent"]).ToString("0.##");

                    component += Environment.NewLine + ((int)TrType.Create).ToString() + "|" + trSta + "|" + trPartDetail + "|" + woNumber +
                        "|" + compDetType + "|" + compItemNo + "|" + materialNo + "|" + compMaterialDesc + "|" + compReqQty + "|" + compUM +
                        "|" + compPrice + "|" + plant + "|" + compSloc + "|" + compOpertnNo;

                    componentprice += Environment.NewLine + woNumber + "|" + compDetType + "|" + compItemNo + "|" + materialNo + "|" + compMaterialDesc
                        + "|" + compReqQty + "|" + unit + "|" + pricePerUnit + "|" + discountPercent + "|" + currency + "|" + termOfPayment;

                    // update item number
                    _DL_trs_workorderpartssummary = new DL_trs_workorderpartssummary();
                    _DL_trs_workorderpartssummary.trs_itemnumber = itemCountComponent;
                    _DL_trs_workorderpartssummary.Update(organizationService, enWOPartSummary.Id);
                    itemCountComponent += 10;
                }
                #endregion
                #region supporting material
                unit = "EA";
                QueryExpression qeWOSupportingMaterialExternalMaterial = new QueryExpression(_DL_trs_workordersupportingmaterial.EntityName);
                qeWOSupportingMaterialExternalMaterial.ColumnSet = new ColumnSet(true);
                FilterExpression feWOSupportingMaterialExternalMaterial = qeWOSupportingMaterialExternalMaterial.Criteria.AddFilter(LogicalOperator.And);
                feWOSupportingMaterialExternalMaterial.AddCondition("trs_workorderid", ConditionOperator.Equal, id);
                feWOSupportingMaterialExternalMaterial.AddCondition("trs_supportingmaterialtype", ConditionOperator.Equal, true);
                EntityCollection ecWOSupportingMaterialExternalMaterial = _DL_trs_workordersupportingmaterial.Select(organizationService, qeWOSupportingMaterialExternalMaterial);

                foreach (Entity enWOSupportingMaterial in ecWOSupportingMaterialExternalMaterial.Entities)
                {
                    pricePerUnit = string.Empty;
                    discountPercent = string.Empty;
                    materialNo = string.Empty;
                    compItemNo = itemCountComponent.ToString();
                    if (enWOSupportingMaterial.Contains("trs_supportingmaterialname"))
                        compMaterialDesc = enWOSupportingMaterial["trs_supportingmaterialname"].ToString();
                    if (enWOSupportingMaterial.Contains("trs_quantity"))
                        compReqQty = enWOSupportingMaterial["trs_quantity"].ToString();
                    if (enWOSupportingMaterial.Contains("trs_price"))
                        compPrice = ((Money)enWOSupportingMaterial["trs_price"]).Value.ToString("0.##");

                    if (enWOSupportingMaterial.Contains("trs_quantity"))
                        work = enWOSupportingMaterial["trs_quantity"].ToString();
                    if (enWOSupportingMaterial.Contains("trs_totalprice"))
                        pricePerUnit = ((Money)enWOSupportingMaterial["trs_totalprice"]).Value.ToString("0.##");

                    component += Environment.NewLine + ((int)TrType.Create).ToString() + "|" + trSta + "|" + trPartDetail + "|" + woNumber
                        + "|" + compDetType + "|" + compItemNo + "|" + materialNo + "|" + compMaterialDesc + "|" + compReqQty + "|" + unit
                        + "|" + compPrice + "|" + plant + "|" + compSloc + "|" + compOpertnNo;

                    componentprice += Environment.NewLine + woNumber + "|" + compDetType + "|" + compItemNo + "|" + materialNo + "|" + compMaterialDesc
                        + "|" + compReqQty + "|" + unit + "|" + pricePerUnit + "|" + discountPercent + "|" + currency + "|" + termOfPayment;

                    // update item number
                    _DL_trs_workordersupportingmaterial = new DL_trs_workordersupportingmaterial();
                    _DL_trs_workordersupportingmaterial.trs_itemnumber = itemCountComponent;
                    _DL_trs_workordersupportingmaterial.Update(organizationService, enWOSupportingMaterial.Id);
                    itemCountComponent += 10;
                }

                #endregion
                #endregion
                #endregion
                #region release
                release = Environment.NewLine + ((int)TrType.Release).ToString() + "|" + trSta + "|" + trPartHeader + "|" + woNumber;
                #endregion
                #region assignmechanic
                int itemNo = 10;
                string personNo = string.Empty;
                work = "1";

                foreach (Entity enCommercialHeader in ecCommercialHeader.Entities)
                {
                    //if (enCommercialHeader.Contains("trs_itemnumber"))
                    //    itemNo = ((int)enCommercialHeader["trs_itemnumber"]).ToString();
                    //if (enCommercialHeader.Contains("trs_totalrtg"))
                    //    work = ((decimal)enCommercialHeader["trs_totalrtg"]).ToString("##.##");
                    //operation quantity hardcode 1

                    if (enServiceAppointment.Contains("trs_mechanicleader"))
                    {
                        Entity enMechanic = _DL_equipment.Select(organizationService, ((EntityReference)enServiceAppointment["trs_mechanicleader"]).Id);
                        personNo = enMechanic.Contains("trs_nrp") ? enMechanic["trs_nrp"].ToString() : string.Empty;
                    }

                    assignMechanic += Environment.NewLine + ((int)TrType.AssignMechanic).ToString() + "|" + trSta + "|" +
                        trPartDetail + "|" + woNumber + "|" + opDetType + "|" + itemNo.ToString() + "|" + personNo + "|" + work;

                    itemNo += 10;
                }

                foreach (Entity enWOSupportingMaterial in ecWOSupportingMaterialOtherService.Entities)
                {
                    assignMechanic += Environment.NewLine + ((int)TrType.AssignMechanic).ToString() + "|" + trSta + "|" +
                        trPartDetail + "|" + woNumber + "|" + opDetType + "|" + itemNo.ToString() + "|" + personNo + "|" + work;

                    itemNo += 10;
                }
                #endregion
                #region write
                string path = @"\\" + _fSAP.GetSAPSharingPath(organizationService) + @"\02_TRSTOSAP\";
                //string path = @"D:\Shared Folder\";
                if (System.IO.Directory.Exists(path))
                {
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string content = header + operationdetail + component + release + assignMechanic;
                    string filename = "WR_" + timestamp + "_" + woNumber + ".txt";

                    string filenamePrice = "WP_" + timestamp + "_" + woNumber + ".txt";
                    string contentPrice = operationdetailprice + componentprice;

                    System.IO.File.WriteAllText(path + filename, content);
                    System.IO.File.WriteAllText(path + filenamePrice, contentPrice);

                    _DL_serviceappointment.trs_lasterror = string.Empty;
                    _DL_serviceappointment.trs_lastfilename = filename;
                    _DL_serviceappointment.Update(organizationService, id);
                }
                else
                {
                    throw new Exception("Directory not found: " + path);
                }
                #endregion
            }
        }

        private void UpdatePopulasi_HourMeter(IOrganizationService organizationService, Guid populationId, decimal hourMeter, DateTime hourDate)
        {
            if (hourMeter > 0)
            {
                //Update by : [Santony] on [8/4/2015]
                //Remark : di DataLayer nya belum ditambahkan update untuk field "trs_hourmeteronvisit" dan "trs_datevisit"
                _DL_new_population = new DL_new_population();
                _DL_new_population.trs_hourmeteronvisit = hourMeter;
                _DL_new_population.trs_datevisit = hourDate;
                _DL_new_population.Update(organizationService, populationId);
            }
        }
        #endregion

        #region Events
        #region Forms
        public void Form_OnCreate(IOrganizationService organizationService, IPluginExecutionContext pluginExecutionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExecutionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_serviceappointment.EntityName)
                {
                    /* Add by Thomas - 19 March 2015 (Moved from OnUpdate) */
                    string new_serialnumber = string.Empty;
                    if (entity.Attributes.Contains("new_serialnumber") && entity.Attributes["new_serialnumber"] != null)
                    {
                        new_serialnumber = entity.GetAttributeValue<string>("new_serialnumber");
                    }
                    else
                    {
                        throw new InvalidPluginExecutionException("Can not found Serial Number");
                    }

                    Guid trs_pmacttype = Guid.Empty;
                    if (entity.Attributes.Contains("trs_pmacttype") && entity.Attributes["trs_pmacttype"] != null)
                    {
                        trs_pmacttype = entity.GetAttributeValue<EntityReference>("trs_pmacttype").Id;
                    }
                    else
                    {
                        throw new InvalidPluginExecutionException("Can not found PMActType");
                    }

                    Entity eTasklistGroup = _DL_trs_tasklistgroup.Select(organizationService, trs_pmacttype);
                    string trs_tasklistgroupname = eTasklistGroup.GetAttributeValue<string>("trs_tasklistgroupname");

                    string subject = new_serialnumber + "-" + trs_tasklistgroupname;
                    subject = subject.Substring(0, subject.Length <= 30 ? subject.Length : 30);
                    /* End add by Thomas - 19 March 2015 (Moved from OnUpdate) */

                    string newRunningNumber = _BL_trs_runningnumber.GenerateNewRunningNumber(organizationService, pluginExecutionContext, _DL_serviceappointment.EntityName, (DateTime)entity.Attributes["createdon"]);
                    _DL_serviceappointment = new DL_serviceappointment();
                    _DL_serviceappointment.trs_crmwonumber = newRunningNumber;
                    _DL_serviceappointment.subject = subject;
                    _DL_serviceappointment.Update(organizationService, entity.Id);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate : " + ex.Message.ToString());
            }
        }

        public void Form_OnUpdate_PreValidate(IOrganizationService organizationService, IPluginExecutionContext pluginExecutionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExecutionContext.InputParameters["Target"];
                Entity pre = (Entity)pluginExecutionContext.PreEntityImages["Target"];
                if (entity.LogicalName == _DL_serviceappointment.EntityName)
                {
                    /* Move by Thomas to OnCreate - 19 March 2015
                    string new_serialnumber = string.Empty;
                    if (entity.Attributes.Contains("new_serialnumber") && entity.Attributes["new_serialnumber"] != null)
                    {
                        new_serialnumber = entity.GetAttributeValue<string>("new_serialnumber");
                    }
                    else if (pre.Attributes.Contains("new_serialnumber") && pre.Attributes["new_serialnumber"] != null)
                    {
                        new_serialnumber = pre.GetAttributeValue<string>("new_serialnumber");
                    }
                    else
                    {
                        throw new InvalidPluginExecutionException("Can not found Serial Number");
                    }

                    Guid trs_pmacttype = Guid.Empty;
                    if (entity.Attributes.Contains("trs_pmacttype") && entity.Attributes["trs_pmacttype"] != null)
                    {
                        trs_pmacttype = entity.GetAttributeValue<EntityReference>("trs_pmacttype").Id;
                    }
                    else if (pre.Attributes.Contains("trs_pmacttype") && pre.Attributes["trs_pmacttype"] != null)
                    {
                        trs_pmacttype = pre.GetAttributeValue<EntityReference>("trs_pmacttype").Id;
                    }
                    else
                    {
                        throw new InvalidPluginExecutionException("Can not found PMActType");
                    }

                    Entity eTasklistGroup = _DL_trs_tasklistgroup.Select(organizationService, trs_pmacttype);
                    string trs_tasklistgroupname = eTasklistGroup.GetAttributeValue<string>("trs_tasklistgroupname");

                    string subject = new_serialnumber + "-" + trs_tasklistgroupname;
                    if (entity.Attributes.Contains("subject"))
                        entity.Attributes["subject"] = subject.Substring(0, subject.Length <= 30 ? subject.Length : 30);
                    else
                        entity.Attributes.Add("subject", subject.Substring(0, subject.Length <= 30 ? subject.Length : 30));
                     * */
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate_PreValidate : " + ex.Message.ToString());
            }
        }
        #endregion

        #region Fields
        public void Status_OnChange(IOrganizationService organizationService, IPluginExecutionContext pluginExecutionContext)
        {
            try
            {
                Entity ePre = (Entity)pluginExecutionContext.PreEntityImages["Target"];
                Entity entity = (Entity)pluginExecutionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_serviceappointment.EntityName && entity.Attributes.Contains("statuscode"))
                {
                    //Filing Number
                    if (entity.GetAttributeValue<OptionSetValue>("statuscode").Value == Configuration.WOStatus_Released
                            && entity.GetAttributeValue<String>("trs_fillingnumber") == null)
                    {
                        DateTime? createdOn = null;
                        if (entity.Attributes.Contains("createdon"))
                            createdOn = entity.GetAttributeValue<DateTime>("createdon");
                        else if (ePre.Attributes.Contains("createdon"))
                            createdOn = ePre.GetAttributeValue<DateTime>("createdon");
                        else
                            throw new InvalidPluginExecutionException("Can not found Created On for this WO.");

                        Guid? branchId = null;
                        if (entity.Attributes.Contains("trs_branch"))
                            branchId = entity.GetAttributeValue<EntityReference>("trs_branch").Id;
                        else if (ePre.Attributes.Contains("trs_branch"))
                            branchId = ePre.GetAttributeValue<EntityReference>("trs_branch").Id;
                        else
                            throw new InvalidPluginExecutionException("Can not found Branch Id for this WO.");

                        double printingSequence = 0;
                        if (entity.Attributes.Contains("trs_printingsequence"))
                            printingSequence = entity.GetAttributeValue<double>("trs_printingsequence");
                        else if (ePre.Attributes.Contains("trs_printingsequence"))
                            printingSequence = ePre.GetAttributeValue<double>("trs_printingsequence");

                        string newRunningNumber = GenerateNewRunningNumber(organizationService
                                                                            , pluginExecutionContext
                                                                            , (DateTime)createdOn
                                                                            , (Guid)branchId
                                                                            , printingSequence);

                        _DL_serviceappointment = new DL_serviceappointment();
                        _DL_serviceappointment.trs_fillingnumber = newRunningNumber;
                        _DL_serviceappointment.Update(organizationService, entity.Id);
                    }

                    //Create Email
                    if (entity.Attributes.Contains("trs_lasterror") && entity.Attributes["trs_lasterror"] != null)
                    {
                        Guid sender = pluginExecutionContext.InitiatingUserId;
                        string lastError = entity.GetAttributeValue<string>("trs_lasterror");

                        Guid emailId = Guid.Empty;
                        Guid receiver = Guid.Empty;

                        QueryExpression queryExpression = new QueryExpression(_DL_trs_workflowconfiguration.EntityName);
                        queryExpression.ColumnSet = new ColumnSet(true);
                        queryExpression.Criteria.AddCondition("trs_generalconfig", ConditionOperator.Equal, Configuration.ConfigurationName);
                        EntityCollection entityCollection = _DL_trs_workflowconfiguration.Select(organizationService, queryExpression);
                        if (entityCollection.Entities.Count > 0)
                        {
                            Entity eConfiguration = entityCollection.Entities[0];
                            receiver = eConfiguration.GetAttributeValue<EntityReference>("trs_gadgetadministrator").Id;
                        }
                        else
                            throw new Exception("Please setup Gadget Administrator first !");

                        EmailAgent emailAgent = new EmailAgent();
                        emailAgent.AddSender(sender);
                        emailAgent.AddReceiver(_DL_systemuser.EntityName, receiver);
                        emailAgent.subject = "Information error Work order from SAP : " + lastError;
                        emailAgent.description = lastError;
                        emailAgent.Create(organizationService, out emailId);
                    }
               

                    if (entity.GetAttributeValue<OptionSetValue>("statuscode").Value == Configuration.WOStatus_Released)
                    {
                        ReleasetoSAP(organizationService, entity.Id);
                        //CheckIfPartExistOnSAP(organizationService, pluginExecutionContext);
                    }

                    if (entity.GetAttributeValue<OptionSetValue>("statuscode").Value == Configuration.WOStatus_Completed)
                    {
                        Guid pmacttype = Guid.Empty;
                        Guid population = Guid.Empty;
                        Guid CustomerId = Guid.Empty;
                        Guid ContactId = Guid.Empty;
                        Guid RegardingObjectId = Guid.Empty;

                        string Topic = string.Empty;
                        string CRMWoNumber = string.Empty;
                        
                        //pmacttype
                        if (entity.Attributes.Contains("trs_pmacttype"))
                            pmacttype = entity.GetAttributeValue<EntityReference>("trs_pmacttype").Id;
                        else if (ePre.Attributes.Contains("trs_pmacttype"))
                            pmacttype = ePre.GetAttributeValue<EntityReference>("trs_pmacttype").Id;
                        else
                            throw new InvalidPluginExecutionException("Can not found PMActType !");

                        //Population
                        if (entity.Attributes.Contains("trs_equipment"))
                            population = entity.GetAttributeValue<EntityReference>("trs_equipment").Id;
                        else if (ePre.Attributes.Contains("trs_equipment"))
                            population = ePre.GetAttributeValue<EntityReference>("trs_equipment").Id;
                        else
                            throw new InvalidPluginExecutionException("Can not found Population !");

                        //Customer
                        QueryExpression qeCust = new QueryExpression();
                        qeCust.EntityName = "new_population";
                        qeCust.ColumnSet.AllColumns = true;

                        ConditionExpression ceCust = new ConditionExpression();
                        ceCust.AttributeName = "new_populationid";
                        ceCust.Operator = ConditionOperator.Equal;
                        ceCust.Values.Add(population);

                        FilterExpression feCust = new FilterExpression();
                        feCust.FilterOperator = LogicalOperator.And;
                        feCust.Conditions.Add(ceCust);

                        qeCust.Criteria.AddFilter(feCust);
                        EntityCollection ecCust = organizationService.RetrieveMultiple(qeCust);

                        if (ecCust.Entities.Count > 0)
                        {
                            foreach (var itemCust in ecCust.Entities)
                            {
                                if (itemCust.Attributes.Contains("new_customercode") && itemCust.Attributes["new_customercode"] != null)
                                    CustomerId = ((EntityReference)itemCust.Attributes["new_customercode"]).Id;
                                else
                                    throw new InvalidPluginExecutionException("Can not found Customer !");
                            }
                        }

                        //Contact
                        if (entity.Attributes.Contains("trs_contactperson"))
                            ContactId = entity.GetAttributeValue<EntityReference>("trs_contactperson").Id;
                        else if (ePre.Attributes.Contains("trs_contactperson"))
                            ContactId = ePre.GetAttributeValue<EntityReference>("trs_contactperson").Id;
                        else
                            throw new InvalidPluginExecutionException("Can not found Contact Person !");

                        //RegardingObjectId
                        if (entity.Attributes.Contains("regardingobjectid"))
                            RegardingObjectId = entity.GetAttributeValue<EntityReference>("regardingobjectid").Id;
                        else if (ePre.Attributes.Contains("regardingobjectid"))
                            RegardingObjectId = ePre.GetAttributeValue<EntityReference>("regardingobjectid").Id;
                        else
                            throw new InvalidPluginExecutionException("Can not found Regarding Case !");

                        //CRM WO Number
                        if (entity.Attributes.Contains("trs_crmwonumber"))
                            CRMWoNumber = entity.GetAttributeValue<string>("trs_crmwonumber");
                        else if (ePre.Attributes.Contains("trs_crmwonumber"))
                            CRMWoNumber = ePre.GetAttributeValue<string>("trs_crmwonumber");
                        else
                            throw new InvalidPluginExecutionException("Can not found CRM WO Number !");

                        Entity eTasklistGroup = _DL_trs_tasklistgroup.Select(organizationService, pmacttype);
                        switch (Convert.ToInt32(eTasklistGroup.GetAttributeValue<string>("trs_pmacttype")))
                        {
                            case Configuration.PMActType_Delivery:
                                DateTime? docDate = null;
                                if (entity.Attributes.Contains("trs_documentdate"))
                                    docDate = entity.GetAttributeValue<DateTime>("trs_documentdate").ToLocalTime();
                                else if (ePre.Attributes.Contains("trs_documentdate"))
                                    docDate = ePre.GetAttributeValue<DateTime>("trs_documentdate").ToLocalTime();

                                int? statusinOperation = null;
                                if (entity.Attributes.Contains("trs_statusinoperation"))
                                    statusinOperation = entity.GetAttributeValue<OptionSetValue>("trs_statusinoperation").Value;
                                else if (ePre.Attributes.Contains("trs_statusinoperation"))
                                    statusinOperation = ePre.GetAttributeValue<OptionSetValue>("trs_statusinoperation").Value;

                                if (docDate == null)
                                {
                                    throw new InvalidPluginExecutionException("Please fill BAST Sign Date first !");
                                }
                                else if (statusinOperation == null)
                                {
                                    throw new InvalidPluginExecutionException("Please fill Status in Operation first !");
                                }
                                else
                                {
                                    _DL_new_population = new DL_new_population();
                                    //_DL_new_population.trs_unitstatus = true;
                                    _DL_new_population.trs_bastsigndate = (DateTime)docDate;
                                    _DL_new_population.new_statusinoperation = (int)statusinOperation;
                                    _DL_new_population.Update(organizationService, population);
                                }
                                break;
                            case Configuration.PMActType_Assy_Disassy:
                                _DL_new_population = new DL_new_population();
                                if (ePre.GetAttributeValue<bool>("trs_assemblingtype"))
                                    _DL_new_population.trs_lock = false;
                                else
                                    _DL_new_population.trs_lock = true;
                                _DL_new_population.Update(organizationService, population);
                                break;
                            case Configuration.PMActType_PDI:
                                Guid PMActType = Guid.Empty;

                                Topic = "Pembuatan SR Delivery atas WO PDI No. " + CRMWoNumber.ToString();

                                #region Look for PMActType
                                //Look for PMActType
                                QueryExpression qeTask = new QueryExpression();
                                qeTask.EntityName = "trs_tasklistgroup";
                                qeTask.ColumnSet.AllColumns = true;

                                ConditionExpression ceTask = new ConditionExpression();
                                ceTask.AttributeName = "trs_tasklistgroupname";
                                ceTask.Operator = ConditionOperator.Equal;
                                ceTask.Values.Add("Delivery");

                                FilterExpression feTask = new FilterExpression();
                                feTask.FilterOperator = LogicalOperator.And;
                                feTask.Conditions.Add(ceTask);

                                qeTask.Criteria.AddFilter(feTask);
                                EntityCollection ecTask = organizationService.RetrieveMultiple(qeTask);

                                if (ecTask.Entities.Count > 0)
                                {
                                    foreach (var itemTask in ecTask.Entities)
                                    {
                                        if (itemTask.Attributes.Contains("trs_tasklistgroupid") && itemTask.Attributes["trs_tasklistgroupid"] != null)
                                            PMActType = ((Guid)itemTask.Attributes["trs_tasklistgroupid"]);
                                    }
                                }
                                #endregion

                                _DL_incident = new DL_incident();
                                _DL_incident.trs_unit = population;
                                _DL_incident.customerid = CustomerId;
                                _DL_incident.primarycontactid = ContactId;
                                _DL_incident.title = Topic;
                                _DL_incident.description = Topic;
                                _DL_incident.parentcaseid = RegardingObjectId;
                                _DL_incident.trs_pmacttype = PMActType;
                                _DL_incident.Insert(organizationService);
                                break;
                        }

                        UpdatePopulasi_HourMeter(organizationService
                            , ePre.GetAttributeValue<EntityReference>("trs_equipment").Id
                            , ePre.GetAttributeValue<decimal>("trs_lasthourmeter")
                            , DateTime.Now);
                    }

                    //if ((ePre.Attributes.Contains("trs_frommobile") && ePre.GetAttributeValue<bool>("trs_frommobile"))
                    //    || (ePre.Attributes.Contains("trs_workshop") && ePre.GetAttributeValue<bool>("trs_workshop"))
                    //    || entity.GetAttributeValue<OptionSetValue>("statuscode").Value == Configuration.WOStatus_New)
                    //    return;
                    //else
                    bool workshop = false;
                    if (entity.Attributes.Contains("trs_workshop") && entity.Attributes["trs_workshop"] != null)
                        workshop = entity.GetAttributeValue<bool>("trs_workshop");
                    else if (ePre.Attributes.Contains("trs_workshop") && ePre.Attributes["trs_workshop"] != null)
                        workshop = ePre.GetAttributeValue<bool>("trs_workshop");

                    bool fromMobile = false;
                    if (entity.Attributes.Contains("trs_frommobile") && entity.Attributes["trs_frommobile"] != null)
                        fromMobile = entity.GetAttributeValue<bool>("trs_frommobile");
                    else if (ePre.Attributes.Contains("trs_frommobile") && ePre.Attributes["trs_frommobile"] != null)
                        fromMobile = ePre.GetAttributeValue<bool>("trs_frommobile");

                    SendtoMobile(organizationService, entity.Id, entity.GetAttributeValue<OptionSetValue>("statuscode").Value, fromMobile, workshop);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Status_OnChange : " + ex.Message.ToString());
            }
        }

        public void CheckIfPartExistOnSAP(IOrganizationService organizationService, IPluginExecutionContext pluginExecutionContext)
        {
            try
            {
                #region Variable
                Guid WoId = Guid.Empty;
                bool DoesNotExistOnSAP = false;
                string ListPartNumber = string.Empty;
                #endregion

                Entity entity = (Entity)pluginExecutionContext.InputParameters["Target"];
                WoId = entity.Id;

                QueryExpression qeParts = new QueryExpression();
                qeParts.EntityName = "trs_workorderpartssummary";
                qeParts.ColumnSet.AllColumns = true;

                ConditionExpression ceParts = new ConditionExpression();
                ceParts.AttributeName = "trs_workorder";
                ceParts.Operator = ConditionOperator.Equal;
                ceParts.Values.Add(WoId);

                FilterExpression feParts = new FilterExpression();
                feParts.FilterOperator = LogicalOperator.And;
                feParts.Conditions.Add(ceParts);

                qeParts.Criteria.AddFilter(feParts);
                EntityCollection ecParts = organizationService.RetrieveMultiple(qeParts);

                if (ecParts.Entities.Count > 0)
                {
                    foreach (var iParts in ecParts.Entities)
                    {
                        QueryExpression qeMParts = new QueryExpression();
                        qeMParts.EntityName = "trs_masterpart";
                        qeMParts.ColumnSet.AllColumns = true;

                        ConditionExpression ceMParts = new ConditionExpression();
                        ceMParts.AttributeName = "trs_masterpartId";
                        ceMParts.Operator = ConditionOperator.Equal;
                        ceMParts.Values.Add((Guid)iParts.Attributes["trs_partnumber"]);

                        FilterExpression feMParts = new FilterExpression();
                        feMParts.FilterOperator = LogicalOperator.And;
                        feMParts.Conditions.Add(ceMParts);

                        qeMParts.Criteria.AddFilter(feMParts);
                        EntityCollection ecMParts = organizationService.RetrieveMultiple(qeMParts);

                        if (ecMParts.Entities.Count > 0)
                        {
                            foreach (var iMParts in ecMParts.Entities)
                            {
                                if (((OptionSetValue)iMParts.Attributes["trs_isinsap"]).Value == 0)
                                {
                                    DoesNotExistOnSAP = true;
                                    ListPartNumber += iMParts.Attributes["trs_name"].ToString() + Environment.NewLine;
                                }
                            }
                        }
                    }
                }

                if (DoesNotExistOnSAP == false)
                    ReleasetoSAP(organizationService, entity.Id);
                else
                    throw new Exception("Please create these Part Master on SAP : " + Environment.NewLine + ListPartNumber);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate : " + ex.Message.ToString());
            }
        }
        #endregion
        #endregion

        #region Public
        public void CreateWO(IOrganizationService organizationService, IPluginExecutionContext pluginExcecutionContext)
        {
            try
            {
                #region Variable
                Guid PopulationId = Guid.Empty;
                Guid CustomerId = Guid.Empty;
                Guid ProfitCenterId = Guid.Empty;
                Guid WorkCenterId = Guid.Empty;
                Guid ResponsibleCctrId = Guid.Empty;
                Guid ActType = Guid.Empty;
                Guid Service = Guid.Empty;
                Guid PMActType = Guid.Empty;
                Guid UserId = Guid.Empty;
                Guid BusinessUnitId = Guid.Empty;
                Guid PlantId = Guid.Empty;
                Guid CaseId = Guid.Empty;
                Guid ContactId = Guid.Empty;
                Guid TaskHeaderId = Guid.Empty;
                Guid ProductId = Guid.Empty;
                Guid BranchId = Guid.Empty;
                Guid TeamId = Guid.Empty;
                Guid TaskId = Guid.Empty;
                Guid FuncLocId = Guid.Empty;
                Guid TaskListGroupId = Guid.Empty;

                string CustPhone = string.Empty;
                string CustNPWP = string.Empty;
                string CustAddress = string.Empty;
                string SerialNumber = string.Empty;
                string Product = string.Empty;
                string Model = string.Empty;
                string EngineNumber = string.Empty;
                string ChasisNumber = string.Empty;
                string Subject = "WO";
                string Plant = string.Empty;
                string Description = string.Empty;
                string EquipmentNumber = string.Empty;
                string AccountNumber = string.Empty;
                string ASSCount = string.Empty;
                string PMActTypeCode = string.Empty;
                string ContactPhone = string.Empty;
                string TaskDescription = string.Empty;
                string FuncLocAddress = string.Empty;

                decimal HourMeter = 0;
                decimal LastHourMeter = 0;

                OptionSetValue AccIndic = new OptionSetValue(3); //AccIndic : Waranty

                DateTime DeliveryDate = DateTime.Now.ToLocalTime();
                DateTime ScheduledStart = DateTime.Now.ToLocalTime();
                DateTime ScheduledEnd = DateTime.Now.ToLocalTime();

                TimeSpan DateInterval = new TimeSpan(0, 1, 0, 0);
                #endregion

                #region Set Variable
                Entity entity = (Entity)pluginExcecutionContext.InputParameters["Target"];
                Entity previous = new Entity("new_population");

                if (pluginExcecutionContext.MessageName == "Create" || pluginExcecutionContext.MessageName == "Update")
                {
                    entity = (Entity)pluginExcecutionContext.InputParameters["Target"];
                }

                if (pluginExcecutionContext.MessageName == "Update" || pluginExcecutionContext.MessageName == "Delete")
                {
                    previous = (Entity)pluginExcecutionContext.PreEntityImages["PreImage"];
                }
                #endregion

                if (entity.LogicalName == _DL_new_population.EntityName)
                {
                    #region Set Variable
                    //PopulationId
                    if (entity.Attributes.Contains("new_populationid") && entity.Attributes["new_populationid"] != null)
                    {
                        PopulationId = ((Guid)entity.Attributes["new_populationid"]);
                    }
                    else if (previous.Attributes.Contains("new_populationid") && previous.Attributes["new_populationid"] != null)
                    {
                        PopulationId = ((Guid)previous.Attributes["new_populationid"]);
                    }

                    //CustomerId
                    if (entity.Attributes.Contains("new_customercode") && entity.Attributes["new_customercode"] != null)
                    {
                        CustomerId = ((EntityReference)entity.Attributes["new_customercode"]).Id;
                    }
                    else if (previous.Attributes.Contains("new_customercode") && previous.Attributes["new_customercode"] != null)
                    {
                        CustomerId = ((EntityReference)previous.Attributes["new_customercode"]).Id;
                    }
                    #endregion

                    #region Entity Collection Customer
                    AddCustomer(CustomerId);
                    #endregion

                    #region Customer Detail
                    //Customer Detail
                    QueryExpression qe1 = new QueryExpression();
                    qe1.EntityName = "account";
                    qe1.ColumnSet.AllColumns = true;

                    ConditionExpression con1 = new ConditionExpression();
                    con1.AttributeName = "accountid";
                    con1.Operator = ConditionOperator.Equal;
                    con1.Values.Add(CustomerId);

                    FilterExpression Mainfilter1 = new FilterExpression();
                    Mainfilter1.FilterOperator = LogicalOperator.And;
                    Mainfilter1.Conditions.Add(con1);

                    qe1.Criteria.AddFilter(Mainfilter1);
                    EntityCollection results1 = organizationService.RetrieveMultiple(qe1);

                    if (results1.Entities.Count > 0)
                    {
                        foreach (var item1 in results1.Entities)
                        {
                            if (item1.Attributes.Contains("telephone1") && item1.Attributes["telephone1"] != null)
                                CustPhone = item1.Attributes["telephone1"].ToString();
                            if (item1.Attributes.Contains("new_npwp") && item1.Attributes["new_npwp"] != null)
                                CustNPWP = item1.Attributes["new_npwp"].ToString();
                            if (item1.Attributes.Contains("address1_name") && item1.Attributes["address1_name"] != null)
                                CustAddress = item1.Attributes["address1_name"].ToString();
                            if (item1.Attributes.Contains("accountnumber") && item1.Attributes["accountnumber"] != null)
                                AccountNumber = item1.Attributes["accountnumber"].ToString();
                        }
                    }
                    #endregion

                    #region Contact Detail
                    QueryExpression contact = new QueryExpression();
                    contact.EntityName = "contact";
                    contact.ColumnSet.AllColumns = true;

                    ConditionExpression excontact = new ConditionExpression();
                    excontact.AttributeName = "contactid";
                    excontact.Operator = ConditionOperator.Equal;
                    excontact.Values.Add(ContactId);

                    FilterExpression fcontact = new FilterExpression();
                    fcontact.FilterOperator = LogicalOperator.And;
                    fcontact.Conditions.Add(excontact);

                    contact.Criteria.AddFilter(fcontact);
                    EntityCollection contactresult = organizationService.RetrieveMultiple(contact);

                    if (contactresult.Entities.Count > 0)
                    {
                        foreach (var itemcontact in contactresult.Entities)
                        {
                            if (itemcontact.Attributes.Contains("telephone1") && itemcontact.Attributes["telephone1"] != null)
                                ContactPhone = itemcontact.Attributes["telephone1"].ToString();
                        }
                    }
                    #endregion

                    #region Population Detail
                    //Population Detail
                    QueryExpression qe2 = new QueryExpression();
                    qe2.EntityName = "new_population";
                    qe2.ColumnSet.AllColumns = true;

                    ConditionExpression con2 = new ConditionExpression();
                    con2.AttributeName = "new_populationid";
                    con2.Operator = ConditionOperator.Equal;
                    con2.Values.Add(PopulationId);

                    FilterExpression Mainfilter2 = new FilterExpression();
                    Mainfilter2.FilterOperator = LogicalOperator.And;
                    Mainfilter2.Conditions.Add(con2);

                    qe2.Criteria.AddFilter(Mainfilter2);
                    EntityCollection results2 = organizationService.RetrieveMultiple(qe2);

                    if (results2.Entities.Count > 0)
                    {
                        foreach (var item2 in results2.Entities)
                        {
                            if (item2.Attributes.Contains("new_serialnumber") && item2.Attributes["new_serialnumber"] != null)
                                SerialNumber = item2.Attributes["new_serialnumber"].ToString();
                            if (item2.Attributes.Contains("new_productname") && item2.Attributes["new_productname"] != null)
                                Product = item2.Attributes["new_productname"].ToString();
                            if (item2.Attributes.Contains("new_model") && item2.Attributes["new_model"] != null)
                                Model = item2.Attributes["new_model"].ToString();
                            if (item2.Attributes.Contains("new_deliverydate") && item2.Attributes["new_deliverydate"] != null)
                                DeliveryDate = Convert.ToDateTime(item2.Attributes["new_deliverydate"]);
                            if (item2.Attributes.Contains("new_enginenumber") && item2.Attributes["new_enginenumber"] != null)
                                EngineNumber = item2.Attributes["new_enginenumber"].ToString();
                            if (item2.Attributes.Contains("trs_chasisnumber") && item2.Attributes["trs_chasisnumber"] != null)
                                ChasisNumber = item2.Attributes["trs_chasisnumber"].ToString();
                            if (item2.Attributes.Contains("trs_hourmeter") && item2.Attributes["trs_hourmeter"] != null)
                                HourMeter = Convert.ToDecimal(item2.Attributes["trs_hourmeter"]);
                            if (item2.Attributes.Contains("new_latesthourmeter") && item2.Attributes["new_latesthourmeter"] != null)
                                LastHourMeter = Convert.ToDecimal(item2.Attributes["new_latesthourmeter"]);
                            if (item2.Attributes.Contains("new_deliverydate") && item2.Attributes["new_deliverydate"] != null)
                                DeliveryDate = Convert.ToDateTime(item2.Attributes["new_deliverydate"]).ToLocalTime();
                            if (item2.Attributes.Contains("trs_equipmentnumber") && item2.Attributes["trs_equipmentnumber"] != null)
                                EquipmentNumber = item2.Attributes["trs_equipmentnumber"].ToString();
                            if (item2.Attributes.Contains("trs_productmaster") && item2.Attributes["trs_productmaster"] != null)
                                ProductId = ((EntityReference)item2.Attributes["trs_productmaster"]).Id;

                            #region Branch & Plant
                            if (item2.Attributes.Contains("trs_functionallocation") && item2.Attributes["trs_functionallocation"] != null)
                            {
                                FuncLocId = ((EntityReference)item2.Attributes["trs_functionallocation"]).Id;

                                QueryExpression qeFuncLoc = new QueryExpression();
                                qeFuncLoc.EntityName = "trs_functionallocation";
                                qeFuncLoc.ColumnSet.AllColumns = true;

                                ConditionExpression ceFuncLoc = new ConditionExpression();
                                ceFuncLoc.AttributeName = "trs_functionallocationid";
                                ceFuncLoc.Operator = ConditionOperator.Equal;
                                ceFuncLoc.Values.Add(FuncLocId);

                                FilterExpression feFunctLoc = new FilterExpression();
                                feFunctLoc.FilterOperator = LogicalOperator.And;
                                feFunctLoc.Conditions.Add(ceFuncLoc);

                                qeFuncLoc.Criteria.AddFilter(feFunctLoc);
                                EntityCollection ecFuncLoc = organizationService.RetrieveMultiple(qeFuncLoc);

                                if (ecFuncLoc.Entities.Count > 0)
                                {
                                    foreach (var itemFuncLoc in ecFuncLoc.Entities)
                                    {
                                        if (itemFuncLoc.Attributes.Contains("trs_branch") && itemFuncLoc.Attributes["trs_branch"] != null)
                                            BranchId = ((EntityReference)itemFuncLoc.Attributes["trs_branch"]).Id;
                                        if (itemFuncLoc.Attributes.Contains("trs_plant") && itemFuncLoc.Attributes["trs_plant"] != null)
                                            PlantId = ((EntityReference)itemFuncLoc.Attributes["trs_plant"]).Id;
                                        if (itemFuncLoc.Attributes.Contains("trs_functionaladdress") && itemFuncLoc.Attributes["trs_functionaladdress"] != null)
                                            FuncLocAddress = itemFuncLoc.Attributes["trs_functionaladdress"].ToString();
                                    }
                                }
                            }
                            #endregion

                            #region Team
                            QueryExpression qeTeam = new QueryExpression();
                            qeTeam.EntityName = "team";
                            qeTeam.ColumnSet.AllColumns = true;

                            ConditionExpression ceTeam = new ConditionExpression();
                            ceTeam.AttributeName = "businessunitid";
                            ceTeam.Operator = ConditionOperator.Equal;
                            ceTeam.Values.Add(BranchId);

                            FilterExpression feTeam = new FilterExpression();
                            feTeam.FilterOperator = LogicalOperator.And;
                            feTeam.Conditions.Add(ceTeam);

                            qeTeam.Criteria.AddFilter(feTeam);
                            EntityCollection ecTeam = organizationService.RetrieveMultiple(qeTeam);

                            if (ecTeam.Entities.Count > 0)
                            {
                                foreach (var itemTeam in ecTeam.Entities)
                                {
                                    if (itemTeam.Attributes.Contains("teamid") && itemTeam.Attributes["teamid"] != null)
                                        TeamId = ((Guid)itemTeam.Attributes["teamid"]);
                                }
                            }
                            #endregion

                            #region Scheduled Start & Scheduled End
                            if (item2.Attributes.Contains("trs_ass1scheduledate") && item2.Attributes["trs_ass1scheduledate"] != null)
                            {
                                if (item2.Attributes.Contains("trs_ass2scheduledate") && item2.Attributes["trs_ass2scheduledate"] != null)
                                {
                                    if (item2.Attributes.Contains("trs_ass3scheduledate") && item2.Attributes["trs_ass3scheduledate"] != null)
                                        ScheduledStart = Convert.ToDateTime(item2.Attributes["trs_ass3scheduledate"]).ToLocalTime();
                                    else
                                        ScheduledStart = Convert.ToDateTime(item2.Attributes["trs_ass2scheduledate"]).ToLocalTime();
                                }
                                else
                                    ScheduledStart = Convert.ToDateTime(item2.Attributes["trs_ass1scheduledate"]).ToLocalTime();
                            }
                            ScheduledEnd = ScheduledStart + DateInterval;
                            #endregion

                            #region PMActType Value
                            if (item2.Attributes.Contains("trs_ass1") && item2.Attributes["trs_ass1"] != null)
                            {
                                if (item2.Attributes.Contains("trs_ass2") && item2.Attributes["trs_ass2"] != null)
                                {
                                    if (item2.Attributes.Contains("trs_ass3") && item2.Attributes["trs_ass3"] != null)
                                    {
                                        ASSCount = "ASS III"; // ASS 3
                                        PMActTypeCode = "008"; //PMActType 008 for ASS III
                                    }
                                    else
                                    {
                                        ASSCount = "ASS II"; //ASS 2
                                        PMActTypeCode = "007"; //PMActType 007 for ASS II
                                    }
                                }
                                else
                                {
                                    ASSCount = "ASS I"; //ASS 1
                                    PMActTypeCode = "006"; //PMActType 006 for ASS I
                                }
                            }

                            QueryExpression pmacct = new QueryExpression();
                            pmacct.EntityName = "trs_tasklistgroup";
                            pmacct.ColumnSet.AllColumns = true;

                            ConditionExpression Condpmacct = new ConditionExpression();
                            Condpmacct.AttributeName = "trs_pmacttype";
                            Condpmacct.Operator = ConditionOperator.Equal;
                            Condpmacct.Values.Add(PMActTypeCode);

                            FilterExpression Filterpmacct = new FilterExpression();
                            Filterpmacct.FilterOperator = LogicalOperator.And;
                            Filterpmacct.Conditions.Add(Condpmacct);

                            pmacct.Criteria.AddFilter(Filterpmacct);
                            EntityCollection epmacct = organizationService.RetrieveMultiple(pmacct);

                            if (epmacct.Entities.Count > 0)
                            {
                                foreach (var PmacctResult in epmacct.Entities)
                                {
                                    if (PmacctResult.Attributes.Contains("trs_tasklistgroupid") && PmacctResult.Attributes["trs_tasklistgroupid"] != null)
                                        PMActType = ((Guid)PmacctResult.Attributes["trs_tasklistgroupid"]);
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion

                    #region Case Detail
                    QueryExpression cases = new QueryExpression();
                    cases.EntityName = "incident";
                    cases.ColumnSet.AllColumns = true;

                    ConditionExpression excases = new ConditionExpression();
                    excases.AttributeName = "trs_unit";
                    excases.Operator = ConditionOperator.Equal;
                    excases.Values.Add(PopulationId);

                    ConditionExpression topic = new ConditionExpression();
                    topic.AttributeName = "title";
                    topic.Operator = ConditionOperator.Like;
                    topic.Values.Add("%" + ASSCount);

                    FilterExpression fcases = new FilterExpression();
                    fcases.FilterOperator = LogicalOperator.And;
                    fcases.Conditions.Add(excases);
                    fcases.Conditions.Add(topic);

                    cases.Criteria.AddFilter(fcases);
                    EntityCollection casesresult = organizationService.RetrieveMultiple(cases);

                    if (casesresult.Entities.Count > 0)
                    {
                        foreach (var itemcases in casesresult.Entities)
                        {
                            if (itemcases.Attributes.Contains("incidentid") && itemcases.Attributes["incidentid"] != null)
                                CaseId = ((Guid)itemcases.Attributes["incidentid"]);
                            if (itemcases.Attributes.Contains("primarycontactid") && itemcases.Attributes["primarycontactid"] != null)
                                ContactId = ((EntityReference)itemcases.Attributes["primarycontactid"]).Id;
                        }
                    }

                    EntityReference RegardingObjectId = new EntityReference("incident", CaseId);
                    #endregion

                    #region Acttype - Look for 'WARNTY'
                    QueryExpression qeActtype = new QueryExpression();
                    qeActtype.EntityName = "trs_acttype";
                    qeActtype.ColumnSet.AllColumns = true;

                    ConditionExpression ceActtype = new ConditionExpression();
                    ceActtype.AttributeName = "trs_name";
                    ceActtype.Operator = ConditionOperator.Like;
                    ceActtype.Values.Add("WARNTY");

                    FilterExpression feActtype = new FilterExpression();
                    feActtype.FilterOperator = LogicalOperator.And;
                    feActtype.Conditions.Add(ceActtype);

                    qeActtype.Criteria.AddFilter(feActtype);
                    EntityCollection ecActtype = organizationService.RetrieveMultiple(qeActtype);

                    if (ecActtype.Entities.Count > 0)
                    {
                        foreach (var Acttype in ecActtype.Entities)
                        {
                            if (Acttype.Attributes.Contains("trs_acttypeid") && Acttype.Attributes["trs_acttypeid"] != null)
                                ActType = ((Guid)Acttype.Attributes["trs_acttypeid"]);
                        }
                    }
                    #endregion

                    #region Service - Look for 'Service Activity'
                    QueryExpression qeService = new QueryExpression();
                    qeService.EntityName = "service";
                    qeService.ColumnSet.AllColumns = true;

                    ConditionExpression ceService = new ConditionExpression();
                    ceService.AttributeName = "name";
                    ceService.Operator = ConditionOperator.Like;
                    ceService.Values.Add("Service Activity");

                    FilterExpression feService = new FilterExpression();
                    feService.FilterOperator = LogicalOperator.And;
                    feService.Conditions.Add(ceService);

                    qeService.Criteria.AddFilter(feService);
                    EntityCollection ecService = organizationService.RetrieveMultiple(qeService);

                    if (ecService.Entities.Count > 0)
                    {
                        foreach (var Services in ecService.Entities)
                        {
                            if (Services.Attributes.Contains("serviceid") && Services.Attributes["serviceid"] != null)
                                Service = ((Guid)Services.Attributes["serviceid"]);
                        }
                    }
                    #endregion

                    #region Profit Center
                    QueryExpression pc = new QueryExpression();
                    pc.EntityName = "trs_profitcenter";
                    pc.ColumnSet.AllColumns = true;

                    ConditionExpression cpc = new ConditionExpression();
                    cpc.AttributeName = "trs_businessarea";
                    cpc.Operator = ConditionOperator.Equal;
                    cpc.Values.Add(BranchId);

                    ConditionExpression cpcname = new ConditionExpression();
                    cpcname.AttributeName = "trs_name";
                    cpcname.Operator = ConditionOperator.Like;
                    cpcname.Values.Add("Service");

                    FilterExpression pcf = new FilterExpression();
                    pcf.FilterOperator = LogicalOperator.And;
                    pcf.Conditions.Add(cpc);
                    pcf.Conditions.Add(cpcname);

                    pc.Criteria.AddFilter(pcf);
                    EntityCollection pcc = organizationService.RetrieveMultiple(pc);

                    if (pcc.Entities.Count > 0)
                    {
                        foreach (var ProfitCenter in pcc.Entities)
                        {
                            if (ProfitCenter.Attributes.Contains("trs_profitcenterid") && ProfitCenter.Attributes["trs_profitcenterid"] != null)
                                ProfitCenterId = ((Guid)ProfitCenter.Attributes["trs_profitcenterid"]);
                        }
                    }
                    #endregion

                    #region Work Center
                    QueryExpression wc = new QueryExpression();
                    wc.EntityName = "trs_workcenter";
                    wc.ColumnSet.AllColumns = true;

                    ConditionExpression wcc = new ConditionExpression();
                    wcc.AttributeName = "trs_plant";
                    wcc.Operator = ConditionOperator.Equal;
                    wcc.Values.Add(BranchId);

                    FilterExpression wcf = new FilterExpression();
                    wcf.FilterOperator = LogicalOperator.And;
                    wcf.Conditions.Add(wcc);

                    wc.Criteria.AddFilter(wcf);
                    EntityCollection wce = organizationService.RetrieveMultiple(wc);

                    if (wce.Entities.Count > 0)
                    {
                        foreach (var WorkCenter in wce.Entities)
                        {
                            if (WorkCenter.Attributes.Contains("trs_workcenterid") && WorkCenter.Attributes["trs_workcenterid"] != null)
                                WorkCenterId = ((Guid)WorkCenter.Attributes["trs_workcenterid"]);
                        }
                    }
                    #endregion

                    #region ResponsibleCctr
                    QueryExpression rc = new QueryExpression();
                    rc.EntityName = "trs_responsiblecostcenter";
                    rc.ColumnSet.AllColumns = true;

                    ConditionExpression rcc = new ConditionExpression();
                    rcc.AttributeName = "trs_profitcenter";
                    rcc.Operator = ConditionOperator.Equal;
                    rcc.Values.Add(ProfitCenterId);

                    FilterExpression rcf = new FilterExpression();
                    rcf.FilterOperator = LogicalOperator.And;
                    rcf.Conditions.Add(rcc);

                    rc.Criteria.AddFilter(rcf);
                    EntityCollection rce = organizationService.RetrieveMultiple(rc);

                    if (rce.Entities.Count > 0)
                    {
                        foreach (var ResponsibleCctr in rce.Entities)
                        {
                            if (ResponsibleCctr.Attributes.Contains("trs_responsiblecostcenterid") && ResponsibleCctr.Attributes["trs_responsiblecostcenterid"] != null)
                                ResponsibleCctrId = ((Guid)ResponsibleCctr.Attributes["trs_responsiblecostcenterid"]);
                        }
                    }
                    #endregion

                    #region WO Description
                    Description = "WO-" + EquipmentNumber + "-" + AccountNumber + "-" + ASSCount;
                    #endregion

                    #region Task List Header
                    QueryExpression th = new QueryExpression();
                    th.EntityName = "trs_tasklistheader";
                    th.ColumnSet.AllColumns = true;

                    ConditionExpression thc = new ConditionExpression();
                    thc.AttributeName = "trs_product";
                    thc.Operator = ConditionOperator.Equal;
                    thc.Values.Add(ProductId);

                    ConditionExpression thcc = new ConditionExpression();
                    thcc.AttributeName = "trs_tasklistgroup";
                    thcc.Operator = ConditionOperator.Equal;
                    thcc.Values.Add(PMActType);

                    FilterExpression thf = new FilterExpression();
                    thf.FilterOperator = LogicalOperator.And;
                    thf.Conditions.Add(thc);
                    thf.Conditions.Add(thcc);

                    th.Criteria.AddFilter(thf);
                    EntityCollection the = organizationService.RetrieveMultiple(th);

                    if (the.Entities.Count > 0)
                    {
                        foreach (var TaskHeaderResult in the.Entities)
                        {
                            if (TaskHeaderResult.Attributes.Contains("trs_tasklistheaderid") && TaskHeaderResult.Attributes["trs_tasklistheaderid"] != null)
                                TaskHeaderId = ((Guid)TaskHeaderResult.Attributes["trs_tasklistheaderid"]);
                            if (TaskHeaderResult.Attributes.Contains("trs_tasklistgroup") && TaskHeaderResult.Attributes["trs_tasklistgroup"] != null)
                                TaskListGroupId = ((Guid)TaskHeaderResult.Attributes["trs_tasklistgroup"]);
                        }
                    }
                    #endregion

                    #region Task List Group
                    QueryExpression qeTaskListGroup = new QueryExpression();
                    qeTaskListGroup.EntityName = "trs_tasklistgroup";
                    qeTaskListGroup.ColumnSet.AllColumns = true;

                    ConditionExpression ceTaskListGroup = new ConditionExpression();
                    ceTaskListGroup.AttributeName = "trs_tasklistgroupId";
                    ceTaskListGroup.Operator = ConditionOperator.Equal;
                    ceTaskListGroup.Values.Add(TaskListGroupId);

                    FilterExpression feTaskListGroup = new FilterExpression();
                    feTaskListGroup.FilterOperator = LogicalOperator.And;
                    feTaskListGroup.Conditions.Add(ceTaskListGroup);

                    th.Criteria.AddFilter(feTaskListGroup);
                    EntityCollection ecTaskListGroup = organizationService.RetrieveMultiple(qeTaskListGroup);

                    if (ecTaskListGroup.Entities.Count > 0)
                    {
                        foreach (var TaskListGroupResult in ecTaskListGroup.Entities)
                        {
                            if (TaskListGroupResult.Attributes.Contains("trs_tasklistgroupname") && TaskListGroupResult.Attributes["trs_tasklistgroupname"] != null)
                                TaskDescription = TaskListGroupResult.Attributes["trs_tasklistgroupname"].ToString();
                        }
                    }
                    #endregion

                    #region Create WO
                    //Create WO
                    _DL_serviceappointment = new DL_serviceappointment();
                    _DL_serviceappointment.customers = _CustomersList;
                    _DL_serviceappointment.trs_phone = CustPhone;
                    _DL_serviceappointment.trs_npwp = CustNPWP;
                    _DL_serviceappointment.trs_address = CustAddress;
                    _DL_serviceappointment.trs_equipment = PopulationId;
                    _DL_serviceappointment.new_serialnumber = SerialNumber;
                    _DL_serviceappointment.trs_product = Product;
                    _DL_serviceappointment.trs_productmodel = Model;
                    _DL_serviceappointment.new_deliverydate = DeliveryDate;
                    _DL_serviceappointment.scheduledstart = ScheduledStart;
                    _DL_serviceappointment.scheduledend = ScheduledEnd;
                    _DL_serviceappointment.trs_enginenumber = EngineNumber;
                    _DL_serviceappointment.trs_chasisnumber = ChasisNumber;
                    _DL_serviceappointment.trs_hourmeter = HourMeter;
                    _DL_serviceappointment.trs_lasthourmeter = LastHourMeter;
                    _DL_serviceappointment.subject = Subject;
                    _DL_serviceappointment.trs_accind = AccIndic;
                    _DL_serviceappointment.trs_acttype = ActType;
                    _DL_serviceappointment.trs_pmacttype = PMActType;
                    _DL_serviceappointment.trs_branch = BranchId;
                    _DL_serviceappointment.trs_plant = PlantId;
                    _DL_serviceappointment.trs_profitcenter = ProfitCenterId;
                    _DL_serviceappointment.serviceid = Service;
                    _DL_serviceappointment.trs_workcenter = WorkCenterId;
                    _DL_serviceappointment.trs_responsiblecctr = ResponsibleCctrId;
                    _DL_serviceappointment.description = Description;
                    _DL_serviceappointment.regardingobjectid = (EntityReference)RegardingObjectId;
                    _DL_serviceappointment.trs_contactperson = ContactId;
                    _DL_serviceappointment.trs_cpphone = ContactPhone;
                    _DL_serviceappointment.trs_functionallocation = FuncLocId;
                    _DL_serviceappointment.trs_address = FuncLocAddress;

                    Guid WoId = _DL_serviceappointment.Insert(organizationService);
                    #endregion

                    #region Create Commercial Header
                    if (TaskHeaderId != Guid.Empty)
                    {
                        _DL_task = new DL_task();
                        _DL_task.trs_operationid = WoId;
                        _DL_task.regardingobjectid = RegardingObjectId.Id;
                        _DL_task.trs_tasklistheader = TaskHeaderId;
                        _DL_task.trs_price = 0;
                        _DL_task.trs_totalprice = 0;
                        _DL_task.trs_discountamount = 0;
                        _DL_task.trs_discountpercent = 0;
                        _DL_task.subject = TaskDescription;

                        TaskId = _DL_task.Insert(organizationService);
                        //_DL_task.AssociatetoWO(organizationService, TaskHeaderId, WoId);
                    }
                    #endregion

                    #region Assign WO - COMMENT 21/3/2015 (SANTONY) --> UAT
                    //try
                    //{
                    //    // Create the Request Object and Set the Request Object's Properties

                    //    AssignRequest assign = new AssignRequest
                    //    {
                    //        Assignee = new EntityReference("team", TeamId),
                    //        Target = new EntityReference(_DL_serviceappointment.EntityName, WoId)
                    //    };

                    //    // Execute the Request
                    //    organizationService.Execute(assign);
                    //}
                    //catch (Exception ex)
                    //{
                    //    throw new Exception("An error occured while assign Team to a Service Appointment record." + ex.Message);
                    //}
                    #endregion

                    #region Assign Commercial Header - COMMENT 21/3/2015 (SANTONY) --> UAT
                    //if (TaskId != Guid.Empty)
                    //{
                    //    try
                    //    {
                    //        // Create the Request Object and Set the Request Object's Properties
                    //        AssignRequest assign = new AssignRequest
                    //        {
                    //            Assignee = new EntityReference("team", TeamId),
                    //            Target = new EntityReference(_DL_task.EntityName, TaskId)
                    //        };

                    //        // Execute the Request
                    //        organizationService.Execute(assign);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        throw new Exception("An error occured while assign Team to a Commercial Header record." + ex.Message);
                    //    }
                    //}
                    #endregion
                }
                else
                { }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Create_WO : " + ex.Message.ToString());
            }
        }

        public string GenerateNewRunningNumber(IOrganizationService organizationService
            , IPluginExecutionContext pluginExecutionContext
            , DateTime createdOn
            , Guid branchId                 //trs_branch
            , double printingSequence = 0   //trs_printingsequence
        )
        {
            try
            {
                int year = createdOn.Year;
                string branchCode = string.Empty;
                string entityName = _DL_serviceappointment.EntityName + "F";

                Entity eBranch = _DL_businessunit.Select(organizationService, branchId);
                if (eBranch.Attributes.Contains("name"))
                    branchCode = eBranch.GetAttributeValue<string>("name");

                QueryExpression queryExpression = new QueryExpression(_DL_trs_runningnumber.EntityName);
                queryExpression.ColumnSet.AddColumns("trs_prefix");

                FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                filterExpression.AddCondition("trs_entityname", ConditionOperator.Equal, entityName);

                EntityCollection entityCollection = _DL_trs_runningnumber.Select(organizationService, queryExpression);
                if (entityCollection.Entities.Count > 0)
                {
                    Entity entity = entityCollection.Entities[0];

                    Guid lastNumberId = new Guid();
                    decimal lastNumber = 0;
                    Guid lockingBy = new Guid();

                    GetLastNumber(organizationService, pluginExecutionContext, year, entity.Id, out lastNumberId, out lastNumber, out lockingBy);

                    lastNumber++;
                    string newRunningNumber = branchCode;
                    newRunningNumber += "-" + year.ToString().Substring(2, 2);
                    newRunningNumber += "-" + lastNumber.ToString("00000000");
                    newRunningNumber += "-" + printingSequence.ToString("000");

                    UpdateLastNumber(organizationService, lastNumberId, lastNumber);

                    return newRunningNumber.ToUpper();
                }
                else
                {
                    throw new InvalidPluginExecutionException("Please setup running number first for Entity Name '" + entityName + "'.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".GenerateNewRunningNumber : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}
