using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;
using System.Activities;
using System.Security.Cryptography;
using System.Reflection;
using System.ServiceModel;
using EnhancementCRM.HelperUnit;
using EnhancementCRM.HelperUnit.WebServiceUnitSAP;
using System.Text.RegularExpressions;
using EnhancementCRM.HelperUnit.ZPST_CRM_CREATE_SO_UNIT;

namespace EnhancementCRM.WorkflowActivity.Business_Layer
{
    public class BL_CPO
    {
        #region Properties
        private string _classname = "BL_CPO";
        private string _entityname = "salesorder";
        #endregion

        #region Constants
        private const string ConfigurationName = "TRS";
        private const string ConfigurationEntityName = "trs_workflowconfiguration";
        private const string ConfigurationEntityPrimaryFieldName = "trs_generalconfig";
        private const string ConfigurationWebServiceEntityName = "ittn_webservicesconfiguration";
        private const int WebService_SubmitCPO = 841150000;
        private const string SAPWebService_Username = "MIS-ABAP1";
        private const int STATUSCOP_CPOPROCESSEDTOSAP = 2;
        private const int PRODUCT_SALESINVENTORY = 1;
        private const string Separator_Description = ", ";
        #endregion

        #region Depedencies
        Generator _Generator = new Generator();
        MWSLog _mwsLog = new MWSLog();
        #endregion

        #region Publics
        public void SubmitSalesOrdertoSAP(IOrganizationService organizationService, Guid Primaryid, ITracingService trace)
        {
            try
            {
                #region Declare Parameters
                #region Header Parameters
                String Token, OrderNumber, OrderType, DocumentDate, DistChannel, SalesOrg, Divisi, SalesOffice, SoldToParty, PONumber, DocCurrency
                    , PaymentTerm, DeliveryPlant, Salesman, Pabean, CustomerGroup, PriceList, PriceGroup, SalesGroup, BusinessSector
                    , PKP, Reservation, HandlingImport, DocumentRequired, DeliveryTerms, AddressMap, PICTitle, UnitToBeOperated
                    , OtherRemarks, SalesManagerRemarks, POLeasingNumber, DPRemarks, BalanceRemarks, BillingDateTermin1, BillingDateTermin2, PODate, DeliveryTime
                    , DPAmount, Termin1Amount, Termin2Amount, ParameterLog, Retention, TopRetention, Route, Project, ContactName, JobPosition, Address, ContactDetailText
                    , DPPercent, Payment2Percent, Payment3Percent, Payment4Percent
                    , DPWrite, Payment2Write, Payment3Write, Payment4Write;
                #endregion

                #region Lines Parameters
                String ItemNumber, ParentNumber, Material, UnitSpecification, LeasingNumber, ProductType, Qty, Price, FOC;
                #endregion
                #endregion

                #region Set Value Parameters
                OrderType = null;
                DocumentDate = null;
                DistChannel = null;
                SalesOrg = null;
                Divisi = null;
                SalesOffice = null;
                SoldToParty = null;
                DPAmount = null;
                BillingDateTermin1 = "00000000";
                Termin1Amount = "0";
                BillingDateTermin2 = "00000000";
                Termin2Amount = "0";
                PONumber = null;
                PODate = null;
                DocCurrency = null;
                PaymentTerm = null;
                DeliveryPlant = null;
                Salesman = null;
                Pabean = null;
                CustomerGroup = null;
                PriceList = null;
                PriceGroup = null;
                SalesGroup = null;
                BusinessSector = null;
                PKP = null;
                Reservation = null;
                HandlingImport = null;
                DocumentRequired = null;
                DeliveryTerms = null;
                AddressMap = null;
                PICTitle = null;
                UnitToBeOperated = null;
                DeliveryTime = null;
                OtherRemarks = null;
                SalesManagerRemarks = null;
                POLeasingNumber = null;
                DPRemarks = null;
                BalanceRemarks = null;
                ItemNumber = null;
                ParentNumber = "0";
                Material = null;
                Qty = null;
                Price = null;
                UnitSpecification = null;
                LeasingNumber = null;
                ProductType = null;
                ParameterLog = null;
                Retention = "0";
                TopRetention = null;
                Route = null;
                Project = null;
                FOC = null;
                ContactName = null;
                JobPosition = null;
                Address = null;
                ContactDetailText = null;
                DPPercent = null;
                Payment2Percent = null;
                Payment3Percent = null;
                Payment4Percent = null;
                DPWrite = null;
                Payment2Write = null;
                Payment3Write = null;
                Payment4Write = null;
                #endregion

                #region Declare Web Service SAP - [19 July 2019] Unused, due to use new WS from Vendor PST
                //ZcrmCreateSoUnit CreateSOSAP_Header = new ZcrmCreateSoUnit();
                //List<ZtsoUnitTxt> details = new List<ZtsoUnitTxt>();
                //ZtsoUnitTxt detail = new ZtsoUnitTxt();
                //String WebServiceURL, UniqueKey, SAPUsername, SAPPassword;
                #endregion

                #region Declare Web Service SAP (Vendor : PST, Update : 19 July 2019)
                ZPST_CRM_CREATE_SO_UNIT CreateSOSAP_Header = new ZPST_CRM_CREATE_SO_UNIT();
                List<ZPST_UNIT_TXT> details = new List<ZPST_UNIT_TXT>();
                ZPST_UNIT_TXT detail = new ZPST_UNIT_TXT();
                String WebServiceURL, UniqueKey = null, SAPUsername, SAPPassword;
                #endregion

                #region Process
                var CPOHeader = organizationService.Retrieve(_entityname, Primaryid, new ColumnSet(true));

                if (CPOHeader.Attributes.Contains("ordernumber"))
                {
                    OrderNumber = CPOHeader.GetAttributeValue<String>("ordernumber");

                    #region Get CPO Lines Data
                    trace.Trace("Start Getting CPO Lines Data");

                    QueryExpression qelines = new QueryExpression("salesorderdetail");
                    qelines.ColumnSet = new ColumnSet(true);
                    qelines.Criteria.AddCondition("salesorderid", ConditionOperator.Equal, CPOHeader.GetAttributeValue<Guid>("salesorderid"));
                    EntityCollection ecLines = organizationService.RetrieveMultiple(qelines);

                    if (ecLines.Entities.Count > 0)
                    {
                        #region Get SAP WebService Configuration
                        trace.Trace("Start Getting SAP WebService Configuration");

                        QueryExpression queryConfiguration = new QueryExpression(ConfigurationEntityName);
                        queryConfiguration.ColumnSet = new ColumnSet(true);
                        queryConfiguration.Criteria.AddCondition(ConfigurationEntityPrimaryFieldName, ConditionOperator.Equal, ConfigurationName);
                        EntityCollection ECSAPConfiguration = organizationService.RetrieveMultiple(queryConfiguration);

                        if (ECSAPConfiguration.Entities.Count > 0)
                        {
                            Entity SAPConfiguration = ECSAPConfiguration.Entities[0];

                            QueryExpression WSCPO = new QueryExpression(ConfigurationWebServiceEntityName);
                            WSCPO.ColumnSet = new ColumnSet(true);
                            WSCPO.Criteria.AddCondition("ittn_workflowconfiguration", ConditionOperator.Equal, SAPConfiguration.Id);
                            WSCPO.Criteria.AddCondition("ittn_webservicefor", ConditionOperator.Equal, WebService_SubmitCPO);
                            EntityCollection ECWS = organizationService.RetrieveMultiple(WSCPO);

                            if (ECWS.Entities.Count > 0)
                            {
                                Entity WSCPOS = ECWS.Entities[0];

                                WebServiceURL = WSCPOS.GetAttributeValue<string>("ittn_sapwebservice");
                                UniqueKey = WSCPOS.GetAttributeValue<string>("ittn_sapintegrationuniquekey");
                                SAPUsername = WSCPOS.GetAttributeValue<string>("ittn_sapwebserviceusername");
                                SAPPassword = WSCPOS.GetAttributeValue<string>("ittn_sapwebservicepassword");
                            }
                            else
                                throw new InvalidPluginExecutionException("Web Service for Submit CPO is null/empty!");
                        }
                        else
                            throw new InvalidPluginExecutionException("Cannot fount Workflow Configuration with name " + ConfigurationName + " !");
                        #endregion

                        #region Generate Token
                        trace.Trace("Start Generate Token");

                        Token = _Generator.Encrypt(OrderNumber, UniqueKey);

                        #region  Default Data for Testing
                        //Token = "43DC6D8865363B6E8EA204BC3DCA4FD86B32C93E57A22F38B6FF633463151BDC";
                        #endregion

                        //CreateSOSAP_Header.CsrfToken = Token; //[19 July 2019] Unused, due to use new WS from Vendor PST
                        CreateSOSAP_Header.CSRF_TOKEN = Token;
                        #endregion

                        #region Get CPO Header Data
                        trace.Trace("Start Getting CPO Header Data");

                        if (CPOHeader.Contains("new_ordertype"))
                        {
                            Entity Ord = organizationService.Retrieve("new_ordertype", CPOHeader.GetAttributeValue<EntityReference>("new_ordertype").Id, new ColumnSet(true));
                            OrderType = Ord.GetAttributeValue<string>("new_code");
                        }
                        if (CPOHeader.Contains("new_documentdate"))
                            DocumentDate = CPOHeader.GetAttributeValue<DateTime>("new_documentdate").ToLocalTime().Date.ToString("yyyy-MM-dd"); //Format : YYYYMMDD -> 20180131
                        else
                            DocumentDate = DateTime.Now.ToLocalTime().Date.ToString("yyyy-MM-dd"); //Format : YYYYMMDD -> 20180131
                        if (CPOHeader.Contains("new_distchannel"))
                        {
                            Entity dc = organizationService.Retrieve("new_distchannel", CPOHeader.GetAttributeValue<EntityReference>("new_distchannel").Id, new ColumnSet(true));
                            DistChannel = dc.GetAttributeValue<string>("new_code");
                        }
                        if (CPOHeader.Contains("new_salesorganization"))
                        {
                            Entity Sls_Org = organizationService.Retrieve("new_salesorganization", CPOHeader.GetAttributeValue<EntityReference>("new_salesorganization").Id, new ColumnSet(true));
                            SalesOrg = Sls_Org.GetAttributeValue<string>("new_code");
                        }
                        if (CPOHeader.Contains("new_salesoffice"))
                        {
                            Entity SOff = organizationService.Retrieve("new_deliveryplant", CPOHeader.GetAttributeValue<EntityReference>("new_salesoffice").Id, new ColumnSet(true));
                            SalesOffice = SOff.GetAttributeValue<string>("new_name");
                        }
                        if (CPOHeader.Contains("customerid"))
                        {
                            Entity Acc = organizationService.Retrieve("account", CPOHeader.GetAttributeValue<EntityReference>("customerid").Id, new ColumnSet(true));

                            if (SalesOrg == "A000")
                            {
                                if (Acc.Contains("accountnumber"))
                                    SoldToParty = Acc.GetAttributeValue<string>("accountnumber");
                            }
                            else if (SalesOrg == "S000")
                            {
                                if (Acc.Contains("new_accountnumbershn"))
                                    SoldToParty = Acc.GetAttributeValue<string>("new_accountnumbershn");
                            }
                            
                            if (Acc.Contains("new_taxstatus"))
                            {
                                Entity TaxStatus = organizationService.Retrieve("new_tax", Acc.GetAttributeValue<EntityReference>("new_taxstatus").Id, new ColumnSet(true));
                                PKP = TaxStatus.GetAttributeValue<String>("new_code");
                            }

                            if (Acc.Contains("new_businesssector"))
                            {
                                Entity Business = organizationService.Retrieve("new_businesssector", Acc.GetAttributeValue<EntityReference>("new_businesssector").Id, new ColumnSet(true));
                                BusinessSector = Business.GetAttributeValue<String>("new_code");
                            }
                        }
                        //if (CPOHeader.Contains("new_detailamount"))
                        //    DPAmount = CPOHeader.GetAttributeValue<Money>("new_detailamount").Value.ToString();
                        if (CPOHeader.Contains("new_ponumber"))
                            PONumber = CPOHeader.GetAttributeValue<String>("new_ponumber");
                        if (CPOHeader.Contains("new_podate"))
                            PODate = CPOHeader.GetAttributeValue<DateTime>("new_podate").ToLocalTime().Date.ToString("yyyy-MM-dd"); //Format : YYYYMMDD -> 20180131
                        if (CPOHeader.Contains("transactioncurrencyid"))
                            DocCurrency = CPOHeader.GetAttributeValue<EntityReference>("transactioncurrencyid").Name;
                        if (CPOHeader.Contains("new_paymentterm"))
                        {
                            Entity pterm = organizationService.Retrieve("new_paymentterm", CPOHeader.GetAttributeValue<EntityReference>("new_paymentterm").Id, new ColumnSet(true));

                            if (pterm.Contains("new_code"))
                                PaymentTerm = pterm.GetAttributeValue<string>("new_code");
                        }
                        if (CPOHeader.Contains("new_deliveryplant"))
                        {
                            Entity dplant = organizationService.Retrieve("new_deliveryplant", CPOHeader.GetAttributeValue<EntityReference>("new_deliveryplant").Id, new ColumnSet(true));
                            DeliveryPlant = dplant.GetAttributeValue<string>("new_name");
                        }
                        if (CPOHeader.Contains("ownerid"))
                        {
                            Entity owner = organizationService.Retrieve("systemuser", CPOHeader.GetAttributeValue<EntityReference>("ownerid").Id, new ColumnSet(true));

                            if (owner.Contains("new_nrp"))
                                Salesman = owner.GetAttributeValue<string>("new_nrp");
                            else
                                throw new InvalidPluginExecutionException("Owner NRP is null/ empty!");
                        }
                        if (CPOHeader.Contains("new_pabeanlk"))
                        {
                            Entity pabean = organizationService.Retrieve("new_pabean", CPOHeader.GetAttributeValue<EntityReference>("new_pabeanlk").Id, new ColumnSet(true));
                            Pabean = pabean.GetAttributeValue<string>("new_code");
                        }
                        if (CPOHeader.Contains("new_pricelistcpo"))
                        {
                            Entity pricelistcpo = organizationService.Retrieve("new_pricelistcpo", CPOHeader.GetAttributeValue<EntityReference>("new_pricelistcpo").Id, new ColumnSet(true));
                            PriceList = pricelistcpo.GetAttributeValue<string>("new_code");
                        }
                        if (CPOHeader.Contains("new_pricegroup"))
                        {
                            Entity pricegroup = organizationService.Retrieve("new_pricegroup", CPOHeader.GetAttributeValue<EntityReference>("new_pricegroup").Id, new ColumnSet(true));
                            PriceGroup = pricegroup.GetAttributeValue<string>("new_code");
                        }
                        if (CPOHeader.Contains("new_reservation"))
                        {
                            Entity Reserve = organizationService.Retrieve("new_reservation", CPOHeader.GetAttributeValue<EntityReference>("new_reservation").Id, new ColumnSet(true));
                            Reservation = Reserve.GetAttributeValue<string>("new_code");
                        }
                        if (CPOHeader.Contains("new_handlingimport"))
                        {
                            Entity Handling = organizationService.Retrieve("new_handlingimport", CPOHeader.GetAttributeValue<EntityReference>("new_handlingimport").Id, new ColumnSet(true));
                            HandlingImport = Handling.GetAttributeValue<string>("new_code");
                        }
                        if (CPOHeader.Contains("new_documentrequired"))
                        {
                            Entity Document = organizationService.Retrieve("new_documentrequired", CPOHeader.GetAttributeValue<EntityReference>("new_documentrequired").Id, new ColumnSet(true));
                            DocumentRequired = Document.GetAttributeValue<string>("new_code");
                        }
                        if (CPOHeader.Contains("new_deliveryterms"))
                            DeliveryTerms = CPOHeader.GetAttributeValue<String>("new_deliveryterms");
                        if (CPOHeader.Contains("new_adressmap"))
                            AddressMap = CPOHeader.GetAttributeValue<String>("new_adressmap");
                        if (CPOHeader.Contains("new_pictitle"))
                            PICTitle = CPOHeader.GetAttributeValue<String>("new_pictitle");
                        if (CPOHeader.Contains("new_unittobeoperatedin"))
                            UnitToBeOperated = CPOHeader.GetAttributeValue<String>("new_unittobeoperatedin");
                        if (CPOHeader.Contains("new_deliverytime"))
                            DeliveryTime = CPOHeader.GetAttributeValue<DateTime>("new_deliverytime").ToLocalTime().Date.ToString("yyyy-MM-dd"); //Format : YYYYMMDD -> 20180131
                        if (CPOHeader.Contains("new_otherremarks"))
                            OtherRemarks = CPOHeader.GetAttributeValue<String>("new_otherremarks");
                        if (CPOHeader.Contains("new_salesmanagerremarks"))
                            SalesManagerRemarks = CPOHeader.GetAttributeValue<String>("new_salesmanagerremarks");
                        if (CPOHeader.Contains("new_poleasingnumber"))
                            POLeasingNumber = CPOHeader.GetAttributeValue<String>("new_poleasingnumber");
                        if (CPOHeader.Contains("new_dpremarks"))
                            DPRemarks = CPOHeader.GetAttributeValue<String>("new_dpremarks");
                        if (CPOHeader.Contains("new_balanceremarks"))
                            BalanceRemarks = CPOHeader.GetAttributeValue<String>("new_balanceremarks");
                        if (CPOHeader.Contains("new_leasing"))
                        {
                            Entity Leasing = organizationService.Retrieve("new_leasing", CPOHeader.GetAttributeValue<EntityReference>("new_leasing").Id, new ColumnSet(true));
                            LeasingNumber = Leasing.GetAttributeValue<string>("new_code");
                        }
                        if (CPOHeader.Contains("ittn_projectretention") && CPOHeader.GetAttributeValue<bool>("ittn_projectretention") == true)
                        {
                            Retention = Decimal.Round(CPOHeader.GetAttributeValue<Decimal>("ittn_retention"), 0, MidpointRounding.AwayFromZero).ToString();

                            Entity topreten = organizationService.Retrieve("new_paymentterm", CPOHeader.GetAttributeValue<EntityReference>("ittn_paymenttermretention").Id, new ColumnSet(true));

                            if (topreten.Contains("new_code"))
                                TopRetention = topreten.GetAttributeValue<string>("new_code");
                        }
                        if (CPOHeader.Contains("ittn_project") && CPOHeader.GetAttributeValue<bool>("ittn_project") == true)
                            Project = "X";
                        if (CPOHeader.Contains("ittn_bastto"))
                        {
                            Entity Contact = organizationService.Retrieve("contact", CPOHeader.GetAttributeValue<EntityReference>("ittn_bastto").Id, new ColumnSet(true));

                            if (Contact.Contains("fullname"))
                                ContactName = Contact.GetAttributeValue<String>("fullname");
                            //else
                            //    throw new InvalidPluginExecutionException("BAST Contact Full Name is null/ empty!");

                            if (Contact.Contains("jobtitle"))
                                JobPosition = Contact.GetAttributeValue<String>("jobtitle");
                            //else
                            //    throw new InvalidPluginExecutionException("BAST Contact Job Position is null/ empty!");

                            if (Contact.Contains("address1_line1"))
                                Address = Contact.GetAttributeValue<String>("address1_line1");
                            //else
                            //    throw new InvalidPluginExecutionException("BAST Contact Address is null/ empty!");

                            ContactDetailText = ContactName + ";" + JobPosition + ";" + Address;
                        }
                        //else
                        //    throw new InvalidPluginExecutionException("BAST Contact is null/ empty!");

                        if (CPOHeader.Contains("quoteid"))
                        {
                            Entity Quote = organizationService.Retrieve("quote", CPOHeader.GetAttributeValue<EntityReference>("quoteid").Id, new ColumnSet(true));

                            if (Quote.GetAttributeValue<bool>("new_selectpayment") == false) //Percentage
                            {
                                if (Quote.Contains("new_downpayment") && Quote.Attributes["new_downpayment"] != null)
                                    DPPercent = Quote.GetAttributeValue<decimal>("new_downpayment").ToString();
                                if (Quote.Contains("new_payment2") && Quote.Attributes["new_payment2"] != null)
                                    Payment2Percent = Quote.GetAttributeValue<decimal>("new_payment2").ToString();
                                if (Quote.Contains("new_payment3") && Quote.Attributes["new_payment3"] != null)
                                    Payment3Percent = Quote.GetAttributeValue<decimal>("new_payment3").ToString();
                                if (Quote.Contains("ittn_payment4") && Quote.Attributes["ittn_payment4"] != null)
                                    Payment4Percent = Quote.GetAttributeValue<decimal>("ittn_payment4").ToString();
                            }
                            else if (Quote.GetAttributeValue<bool>("new_selectpayment") == true) //Write-in
                            {
                                if (Quote.Contains("new_downpaymentwritein") && Quote.Attributes["new_downpaymentwritein"] != null)
                                    DPWrite = Quote.GetAttributeValue<Money>("new_downpaymentwritein").Value.ToString();
                                if (Quote.Contains("new_paymentiiwritein") && Quote.Attributes["new_paymentiiwritein"] != null)
                                    Payment2Write = Quote.GetAttributeValue<Money>("new_paymentiiwritein").Value.ToString();
                                if (Quote.Contains("new_paymentiiiwritein") && Quote.Attributes["new_paymentiiiwritein"] != null)
                                    Payment3Write = Quote.GetAttributeValue<Money>("new_paymentiiiwritein").Value.ToString();
                                if (Quote.Contains("ittn_paymentivwritein") && Quote.Attributes["ittn_paymentivwritein"] != null)
                                    Payment4Write = Quote.GetAttributeValue<Money>("ittn_paymentivwritein").Value.ToString();
                            }
                        }
                        else
                            throw new InvalidPluginExecutionException("Quote is null/ empty!");
                        #endregion

                        ParameterLog += string.Format("Header Parameters : Token : {0}, OrderNumber (Bstkd) : {1}, OrderType (Auart) : {2}, DocumentDate (Audat) : {3}, DistChannel (Vtweg): {4}, " +
                            "SalesOrg (Vkorg) : {5}, Divisi (Spart) : {6}, SalesOffice (Vkbur) : {7}, SoldToParty (Kunnr) : {8}, BillingDateDP (Fkdat09) : {9}, DPAmount (Fakwr09) : {10}, " +
                            "BillingDateTermin1 (Fkdat10) : {11}, Termin1Amount (Fakwr10) : {12}, BillingDateTermin2 (Fkdat20) : {13}, Termin2Amount (Fakwr20) : {14}, PONumber (BstkdCust): {15}, " +
                            "PODate (Bstdk) : {16}, DocCurrency (Waerk) : {17}, PaymentTerm (Zterm) : {18}, DeliveryPlant (Werks) : {19}, Salesman (Pernr) : {20}, Pabean (Ktgrd) : {21}, " +
                            "CustomerGroup (Kdgrp) : {22}, PriceList (Pltyp) : {23}, PriceGroup (Konda) : {24}, BusinessSector (Kvgr1): {25}, PKP (Kvgr2) : {26}, Reservation (Kvgr3) : {27}, " +
                            "HandlingImport (Kvgr4) : {28}, DocumentRequired (Kvgr5) : {29}, DeliveryTerms (Txhzfu01) : {30}, AddressMap (Txhzfu02) : {31}, PICTitle (Txhzfu03) : {32}, " +
                            "UnitToBeOperated (Txhzfu04) : {33}, DeliveryTime (Txhzfu05) : {34}, OtherRemarks (Txhzfu06) : {35}, SalesManagerRemarks (Txhzfu07) : {36}, POLeasingNumber (Txhzfu08) : {37}, " +
                            "DPRemarks (Txhzfu09) : {38}, BalanceRemarks (Txhzfu10) : {39}, LeasingNumber (leasing) : {40}" +
                            "DPPercent (DP_PERCENT) : {41}, Payment2Percent (TERMIN1_PERCENT) : {42}, Payment3Percent (TERMIN2_PERCENT) : {43}, Payment4Percent (TERMIN3_PERCENT) : {44}" +
                            "DPWrite (DP_VALUE) : {45}, Payment2Write (TERMIN1_VALUE) : {46}, Payment3Write (TERMIN2_VALUE) : {47}, Payment4Write (TERMIN3_VALUE) : {48}" + _mwsLog.ColumnsSeparator
                            , Token, OrderNumber, OrderType, DocumentDate, DistChannel, SalesOrg, Divisi, SalesOffice, SoldToParty, DocumentDate, DPAmount, BillingDateTermin1, Termin1Amount, BillingDateTermin2
                            , Termin2Amount, PONumber, PODate, DocCurrency, PaymentTerm, DeliveryPlant, Salesman, Pabean, CustomerGroup, PriceList, PriceGroup, BusinessSector, PKP, Reservation, HandlingImport
                            , DocumentRequired, DeliveryTerms, AddressMap, PICTitle, UnitToBeOperated, DeliveryTime, OtherRemarks, SalesManagerRemarks, POLeasingNumber, DPRemarks, BalanceRemarks, LeasingNumber
                            , DPPercent, Payment2Percent, Payment3Percent, Payment4Percent, DPWrite, Payment2Write, Payment3Write, Payment4Write);

                        foreach (var enLines in ecLines.Entities)
                        {
                            //detail = new ZtsoUnitTxt(); //[19 July 2019] Unused, due to use new WS from Vendor PST
                            detail = new ZPST_UNIT_TXT();

                            #region Set CPO Header Data to WebService
                            trace.Trace("Start Set WebService CPO Header Data");

                            #region  Default Data for Testing
                            //OrderNumber = "ORD-12345-A7A9T9";
                            //OrderType = "ZVDI";
                            //DocumentDate = "2015-01-01";
                            //DistChannel = "1";
                            //SalesOrg = "A000";
                            //Divisi = "N1";
                            //SalesOffice = "A005";
                            //SoldToParty = "501445";
                            //DPAmount = "480000000";
                            //BillingDateTermin1 = "2015-01-01";
                            //Termin1Amount = "0";
                            //BillingDateTermin2 = "2015-01-01";
                            //Termin2Amount = "0";
                            //PONumber = "SPJB/00001/KBT/VIII/2018-MOS";
                            //PODate = "2015-01-01";
                            //DocCurrency = "IDR";
                            //PaymentTerm = "1";
                            //DeliveryPlant = "A005";
                            //Salesman = "2110003";
                            //Pabean = "A1";
                            //CustomerGroup = "2";
                            //PriceList = "P1";
                            //PriceGroup = "A1";
                            //BusinessSector = "7";
                            //PKP = "Z1";
                            //Reservation = "Z1";
                            //HandlingImport = "Z2";
                            //DocumentRequired = "Z2";
                            //DeliveryTerms = "Franco Wahau-Kutim";
                            //AddressMap = "Wahau/Kutai Timur";
                            //PICTitle = "Bp Kusnadi/Owner";
                            //UnitToBeOperated = "Kaltim";
                            //DeliveryTime = "August 2018";
                            //OtherRemarks = "Private Coy";
                            //SalesManagerRemarks = "Mohon buat SR DO unit diatas";
                            //POLeasingNumber = "PO";
                            //DPRemarks = "480000000";
                            //BalanceRemarks = "Payment By Leasing";
                            //LeasingNumber = "1";
                            #endregion

                            #region [19 July 2019] Unused, due to use new WS from Vendor PST
                            //detail.Bstkd = OrderNumber;
                            //detail.Auart = OrderType;
                            //detail.Audat = DocumentDate;
                            //detail.Vtweg = DistChannel;
                            //detail.Vkorg = SalesOrg;
                            //detail.Vkbur = SalesOffice;
                            //detail.Kunnr = SoldToParty;
                            //detail.Fkdat09 = DocumentDate;
                            //detail.Fakwr09 = DPAmount;
                            //detail.Fkdat10 = BillingDateTermin1;
                            //detail.Fakwr10 = Termin1Amount;
                            //detail.Fkdat20 = BillingDateTermin2;
                            //detail.Fakwr20 = Termin2Amount;
                            //detail.BstkdCust = PONumber;
                            //detail.Bstdk = PODate;
                            //detail.Waerk = DocCurrency;
                            //detail.Zterm = PaymentTerm;
                            //detail.Werks = DeliveryPlant;
                            //detail.Pernr = Salesman;
                            //detail.Ktgrd = Pabean;
                            //detail.Kdgrp = CustomerGroup;
                            //detail.Pltyp = PriceList;
                            //detail.Konda = PriceGroup;
                            //detail.Kvgr1 = BusinessSector;
                            //detail.Kvgr2 = PKP;
                            //detail.Kvgr3 = Reservation;
                            //detail.Kvgr4 = HandlingImport;
                            //detail.Kvgr5 = DocumentRequired;
                            //detail.Txhzfu01 = DeliveryTerms;
                            //detail.Txhzfu02 = AddressMap;
                            //detail.Txhzfu03 = PICTitle;
                            //detail.Txhzfu04 = UnitToBeOperated;
                            //detail.Txhzfu05 = DeliveryTime;
                            //detail.Txhzfu06 = OtherRemarks;
                            //detail.Txhzfu07 = SalesManagerRemarks;
                            //detail.Txhzfu08 = POLeasingNumber;
                            //detail.Txhzfu09 = DPRemarks;
                            //detail.Txhzfu10 = BalanceRemarks;
                            //detail.Leasing = LeasingNumber;
                            #endregion

                            detail.BSTKD = OrderNumber;
                            detail.AUART = OrderType;
                            detail.AUDAT = DocumentDate;
                            detail.VTWEG = DistChannel;
                            detail.VKORG = SalesOrg;
                            detail.VKBUR = SalesOffice;
                            detail.EBELN = PONumber;
                            detail.KUNNR = SoldToParty;
                            detail.SOTP = SoldToParty;
                            detail.SHTP = SoldToParty;
                            detail.BITP = SoldToParty;
                            detail.PAYER = SoldToParty;
                            detail.FKDAT09 = DocumentDate;
                            //detail.FAKWR09 = DPAmount;
                            detail.FKDAT10 = BillingDateTermin1;
                            detail.FAKWR10 = Termin1Amount;
                            detail.FKDAT20 = BillingDateTermin2;
                            detail.FAKWR20 = Termin2Amount;
                            detail.BSTKD_CUST = PONumber;
                            detail.DATE = DocumentDate;
                            detail.BSTDK = PODate;
                            detail.RDATE = DeliveryTime;
                            detail.WAERK = DocCurrency;
                            detail.ZTERM = PaymentTerm;
                            detail.WERKS = DeliveryPlant;
                            detail.PERNR = Salesman;
                            detail.KTGRD = Pabean;
                            detail.KDGRP = CustomerGroup;
                            detail.PLTYP = PriceList;
                            detail.KONDA = PriceGroup;
                            detail.KVGR1 = BusinessSector;
                            detail.KVGR2 = PKP;
                            detail.KVGR3 = Reservation;
                            detail.KVGR4 = HandlingImport;
                            detail.KVGR5 = DocumentRequired;
                            detail.TXHZFU01 = DeliveryTerms;
                            detail.TXHZFU02 = AddressMap;
                            detail.TXHZFU03 = PICTitle;
                            detail.TXHZFU04 = UnitToBeOperated;
                            detail.TXHZFU05 = DeliveryTime;
                            detail.TXHZFU06 = OtherRemarks;
                            detail.TXHZFU07 = SalesManagerRemarks;
                            detail.TXHZFU08 = POLeasingNumber;
                            detail.TXHZFU09 = DPRemarks;
                            detail.TXHZFU10 = BalanceRemarks;
                            detail.LEASING = LeasingNumber;
                            detail.RETEN = Retention;
                            detail.TOPRETEN = TopRetention;
                            detail.FLAG_PROJECT = Project;
                            detail.TXHZFU11 = ContactDetailText;
                            detail.DP_PERCENT = DPPercent;
                            detail.TERMIN1_PERCENT = Payment2Percent;
                            detail.TERMIN2_PERCENT = Payment3Percent;
                            detail.TERMIN3_PERCENT = Payment4Percent;
                            detail.DP_VALUE = DPWrite;
                            detail.TERMIN1_VALUE = Payment2Write;
                            detail.TERMIN2_VALUE = Payment3Write;
                            detail.TERMIN3_VALUE = Payment4Write;
                            #endregion

                            #region Get CPO Lines Data
                            trace.Trace("Start Getting CPO Lines Data");

                            if (enLines.Contains("productid"))
                            {
                                Entity Product = organizationService.Retrieve("product", enLines.GetAttributeValue<EntityReference>("productid").Id, new ColumnSet(true));

                                //if (!enLines.Contains("new_parentnumber") && Product.Contains("producttypecode") && Product.GetAttributeValue<OptionSetValue>("producttypecode").Value == PRODUCT_SALESINVENTORY)
                                if (enLines.Contains("new_itemnumber"))
                                {
                                    if (Product.Contains("productnumber"))
                                        Material = Product.GetAttributeValue<String>("productnumber");
                                    if (Product.Contains("producttypecode"))
                                        ProductType = Product.GetAttributeValue<OptionSetValue>("producttypecode").Value.ToString();

                                    if (Product.Contains("new_salesgroup"))
                                    {
                                        Entity Sales = organizationService.Retrieve("new_salesgroup", Product.GetAttributeValue<EntityReference>("new_salesgroup").Id, new ColumnSet(true));
                                        SalesGroup = Sales.GetAttributeValue<String>("new_salesgroupcode");
                                    }

                                    if (enLines.Contains("new_itemnumber"))
                                        ItemNumber = Decimal.Round(enLines.GetAttributeValue<Decimal>("new_itemnumber"), 0, MidpointRounding.AwayFromZero).ToString();
                                    if (enLines.Contains("new_parentnumber"))
                                        ParentNumber = Decimal.Round(enLines.GetAttributeValue<Decimal>("new_parentnumber"), 0, MidpointRounding.AwayFromZero).ToString();
                                    else
                                        ParentNumber = "0";
                                    if (enLines.Contains("quantity"))
                                        Qty = Decimal.Round(enLines.GetAttributeValue<Decimal>("quantity"), 0, MidpointRounding.AwayFromZero).ToString();
                                    if (enLines.Contains("new_priceperunit"))
                                        Price = new decimal(0).ToString();
                                    if (enLines.Contains("description"))
                                        UnitSpecification = EliminateSymbols(enLines.GetAttributeValue<String>("description"));
                                        //UnitSpecification = GetProductAttachmentDetail(organizationService, CPOHeader.GetAttributeValue<Guid>("salesorderid"), ItemNumber, enLines.GetAttributeValue<String>("description"));
                                    if (Product.Contains("new_division"))
                                        Divisi = Product.GetAttributeValue<EntityReference>("new_division").Name;

                                    if (enLines.Contains("ittn_foc") && enLines.GetAttributeValue<bool>("ittn_foc") == true)
                                        FOC = "X";
                                    else
                                        FOC = null;

                                    if (enLines.Contains("ittn_route"))
                                    {
                                        Entity MasterRoute = organizationService.Retrieve("ittn_masterroute", enLines.GetAttributeValue<EntityReference>("ittn_route").Id, new ColumnSet(true));
                                        Route = MasterRoute.GetAttributeValue<String>("ittn_route");
                                    }
                                    else if (enLines.Contains("ittn_deliveryplanbranch"))
                                    {
                                        Entity DeliveryPlanBranch = organizationService.Retrieve("new_deliveryplant", enLines.GetAttributeValue<EntityReference>("ittn_deliveryplanbranch").Id, new ColumnSet(true));
                                        Route = DeliveryPlanBranch.GetAttributeValue<String>("new_description");
                                    }
                                    else
                                        throw new InvalidPluginExecutionException("CPO Product Route on line item number : " + ItemNumber + " is null/ empty!");

                                    //if (enLines.Contains("new_extendedamount") && enLines.Contains("new_tax"))
                                    //    DPAmount = (enLines.GetAttributeValue<Money>("new_extendedamount").Value - enLines.GetAttributeValue<Money>("new_tax").Value).ToString();
                                    //else
                                    //    DPAmount = null;

                                    if (enLines.Contains("ittn_totalextendedamount"))
                                        DPAmount = (enLines.GetAttributeValue<Money>("ittn_totalextendedamount").Value).ToString();
                                    else
                                        DPAmount = null;

                                    #region Set CPO Lines Data to WebService
                                    trace.Trace("Start Set WebService CPO Lines Data");

                                    #region  Default Data for Testing
                                    //ItemNumber = "1000";
                                    //Material = "U50-5S";
                                    //Qty = "1";
                                    //Price = "1";
                                    //UnitSpecification = "0";
                                    //ProductType = "KBT";
                                    //SalesGroup = "KBT";
                                    #endregion

                                    #region [19 July 2019] Unused, due to use new WS from Vendor PST
                                    //detail.Posnr = ItemNumber;
                                    //detail.Matnr = Material;
                                    //detail.Kwmeng = Qty;
                                    //detail.Kbetr = Price;
                                    //detail.Txizfu01 = UnitSpecification;
                                    //detail.Flag = ProductType;
                                    //detail.Vkgrp = SalesGroup;
                                    //detail.Spart = Divisi;
                                    #endregion

                                    detail.POSNR = ItemNumber;
                                    detail.HOSNR = ParentNumber;
                                    detail.MATNR = Material;
                                    detail.QTY = Qty;
                                    detail.KBETR = Price;
                                    detail.TXIZFU01 = UnitSpecification;
                                    detail.FLAG = ProductType;
                                    detail.VKGRP = SalesGroup;
                                    detail.SPART = Divisi;
                                    detail.FLAG_FOC = FOC;
                                    detail.ROUTE = Route;
                                    detail.FAKWR09 = DPAmount;

                                    ParameterLog += string.Format("Lines Parameters : ItemNumber (Posnr) : {0}, Material (Matnr) : {1}, Qty (Kwmeng) : {2}, Price (Kbetr) : {3}, UnitSpecification (Txizfu01) : {4}, " +
                                        "ProductType (Flag) : {5}, SalesGroup (Vkgrp) : {6}, FOC : {7}" + _mwsLog.ColumnsSeparator
                                        , ItemNumber, Material, Qty, Price, UnitSpecification, ProductType, SalesGroup, FOC);

                                    #region List of Condition Type
                                    QueryExpression cpocondtype = new QueryExpression("ittn_cpoconditiontype");
                                    cpocondtype.ColumnSet = new ColumnSet(true);
                                    cpocondtype.Criteria.AddCondition("ittn_cpo", ConditionOperator.Equal, CPOHeader.GetAttributeValue<Guid>("salesorderid"));
                                    cpocondtype.Criteria.AddCondition("ittn_amount", ConditionOperator.GreaterThan, new decimal(0));
                                    EntityCollection eccpocondtype = organizationService.RetrieveMultiple(cpocondtype);
                                    if (eccpocondtype.Entities.Count > 0)
                                    {
                                        ParameterLog += string.Format("Condition Type Parameters : ");

                                        foreach (var encpocondtype in eccpocondtype.Entities)
                                        {
                                            if (encpocondtype.Contains("ittn_conditiontype") && encpocondtype.Contains("ittn_itemnumber"))
                                            {
                                                Entity ConditionType = organizationService.Retrieve("ittn_masterconditiontype", encpocondtype.GetAttributeValue<EntityReference>("ittn_conditiontype").Id, new ColumnSet(true));

                                                if (ConditionType.Contains("ittn_code"))
                                                {
                                                    string ConditionTypeCode = ConditionType.GetAttributeValue<String>("ittn_code");
                                                    string ItemNumberConditionType = Decimal.Round(encpocondtype.GetAttributeValue<Decimal>("ittn_itemnumber"), 0, MidpointRounding.AwayFromZero).ToString();

                                                    if (ItemNumber == ItemNumberConditionType)
                                                    {
                                                        switch (ConditionTypeCode)
                                                        {
                                                            case "ZAS1":
                                                                detail.ZAS1 = encpocondtype.GetAttributeValue<Money>("ittn_amount").Value.ToString();
                                                                break;
                                                            case "ZAS2":
                                                                detail.ZAS2 = encpocondtype.GetAttributeValue<Money>("ittn_amount").Value.ToString();
                                                                break;
                                                            case "ZAS3":
                                                                detail.ZAS3 = encpocondtype.GetAttributeValue<Money>("ittn_amount").Value.ToString();
                                                                break;
                                                            case "ZC00":
                                                                detail.ZC00 = encpocondtype.GetAttributeValue<Money>("ittn_amount").Value.ToString();
                                                                break;
                                                            case "ZCB0":
                                                                detail.ZCB0 = encpocondtype.GetAttributeValue<Money>("ittn_amount").Value.ToString();
                                                                break;
                                                            case "ZDL0":
                                                                detail.ZDL0 = encpocondtype.GetAttributeValue<Money>("ittn_amount").Value.ToString();
                                                                break;
                                                            case "ZFAT":
                                                                detail.ZFAT = encpocondtype.GetAttributeValue<Money>("ittn_amount").Value.ToString();
                                                                break;
                                                            case "ZFG0":
                                                                detail.ZFG0 = encpocondtype.GetAttributeValue<Money>("ittn_amount").Value.ToString();
                                                                break;
                                                            case "ZFR0":
                                                                detail.ZFR0 = encpocondtype.GetAttributeValue<Money>("ittn_amount").Value.ToString();
                                                                break;
                                                            case "ZFR1":
                                                                detail.ZFR1 = encpocondtype.GetAttributeValue<Money>("ittn_amount").Value.ToString();
                                                                break;
                                                            case "ZIN0":
                                                                detail.ZIN0 = encpocondtype.GetAttributeValue<Money>("ittn_amount").Value.ToString();
                                                                break;
                                                            case "ZPDI":
                                                                detail.ZPDI = encpocondtype.GetAttributeValue<Money>("ittn_amount").Value.ToString();
                                                                break;
                                                            case "ZTRC":
                                                                detail.ZTRC = encpocondtype.GetAttributeValue<Money>("ittn_amount").Value.ToString();
                                                                break;
                                                            case "ZV04":
                                                                detail.ZV04 = encpocondtype.GetAttributeValue<Money>("ittn_amount").Value.ToString();
                                                                break;
                                                            case "ZV11":
                                                                detail.ZV11 = encpocondtype.GetAttributeValue<Money>("ittn_amount").Value.ToString();
                                                                break;
                                                            case "ZVCR":
                                                                detail.ZVCR = encpocondtype.GetAttributeValue<Money>("ittn_amount").Value.ToString();
                                                                break;
                                                            case "ZW00":
                                                                detail.ZW00 = encpocondtype.GetAttributeValue<Money>("ittn_amount").Value.ToString();
                                                                break;
                                                            case "ZINS":
                                                                detail.ZINS = encpocondtype.GetAttributeValue<Money>("ittn_amount").Value.ToString();
                                                                break;
                                                            case "ZPTS":
                                                                detail.ZPTS = encpocondtype.GetAttributeValue<Money>("ittn_amount").Value.ToString();
                                                                break;
                                                            default:
                                                                break;
                                                        }
                                                        ParameterLog += string.Format("Condition Type : {0}, Amount : {1}" + _mwsLog.ColumnsSeparator
                                                            , ConditionTypeCode, encpocondtype.GetAttributeValue<Money>("ittn_amount").Value.ToString());
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                    #endregion

                                    #region Add Details Parameter
                                    details.Add(detail);
                                    #endregion

                                    #region Add Details Parameter
                                    //CreateSOSAP_Header.TDetail = details.ToArray(); //[19 July 2019] Unused, due to use new WS from Vendor PST
                                    CreateSOSAP_Header.T_DETAIL = details.ToArray();
                                    #endregion
                                }
                            }   
                            #endregion
                        }

                        if (CreateSOSAP_Header.T_DETAIL != null) /*(CreateSOSAP_Header.TDetail != null) //[19 July 2019] Unused, due to use new WS from Vendor PST*/
                        {
                            #region Open Connection to SAP WebService
                            trace.Trace("Getting WebService Client");

                            EndpointAddress remoteAddress = new EndpointAddress(WebServiceURL);
                            BasicHttpBinding httpbinding = new BasicHttpBinding();
                            httpbinding.Name = "ZWEB_SERVICE_CRM";
                            httpbinding.MessageEncoding = WSMessageEncoding.Mtom;
                            httpbinding.TextEncoding = Encoding.UTF8;
                            httpbinding.SendTimeout = new TimeSpan(0, 10, 0);

                            trace.Trace("Creating Services Client");

                            //ZWEB_SERVICES_UNITClient client = new ZWEB_SERVICES_UNITClient(httpbinding, remoteAddress); //[19 July 2019] Unused, due to use new WS from Vendor PST
                            ZPST_CRM_CREATE_SO_UNIT_V8Client client = new ZPST_CRM_CREATE_SO_UNIT_V8Client(httpbinding, remoteAddress);
                            client.ClientCredentials.UserName.UserName = SAPUsername;
                            client.ClientCredentials.UserName.Password = SAPPassword;

                            try
                            {
                                trace.Trace("Open Client WebService");

                                client.Open();

                                //ZcrmCreateSoUnitResponse response = client.ZcrmCreateSoUnit(CreateSOSAP_Header); //[19 July 2019] Unused, due to use new WS from Vendor PST
                                ZPST_CRM_CREATE_SO_UNITResponse response = client.ZPST_CRM_CREATE_SO_UNIT(CreateSOSAP_Header);

                                if (response != null)
                                {
                                    //DateTime SyncDate = DateTime.Parse(response.SyncDate); //[19 July 2019] Unused, due to use new WS from Vendor PST
                                    //DateTime SyncTime = response.SyncTime; //[19 July 2019] Unused, due to use new WS from Vendor PST
                                    DateTime SyncDate = DateTime.Parse(response.SYNC_DATE);
                                    DateTime SyncTime = response.SYNC_TIME;
                                    SyncDate = SyncDate.AddHours(SyncDate.Hour);
                                    SyncDate = SyncDate.AddMinutes(SyncDate.Minute);
                                    SyncDate = SyncDate.AddSeconds(SyncDate.Second);

                                    if (response.T_DTL_OUT[0].RESULT != null)/*(response.TDtlOut[0].Result != null) //[19 July 2019] Unused, due to use new WS from Vendor PST*/
                                    {
                                        //String Result = response.TDtlOut[0].Result.ToString(); //[19 July 2019] Unused, due to use new WS from Vendor PST
                                        String Result = response.T_DTL_OUT[0].RESULT.ToString();

                                        if (string.Equals(Result, "success"))
                                        {
                                            if (!CPOHeader.Contains("new_cpoidsap"))
                                            {
                                                #region Update CPO ID SAP & Status on CRM - CPO Header
                                                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, "Success to Create Sales Order on SAP. CRM Order Number : "
                                                    //+ OrderNumber + ", CPO ID SAP : " + response.TDtlOut[0].Vbeln //[19 July 2019] Unused, due to use new WS from Vendor PST
                                                    + OrderNumber + ", CPO ID SAP : " + response.T_DTL_OUT[0].VBELN
                                                    + _mwsLog.ColumnsSeparator + ParameterLog, MWSLog.LogType.Information, MWSLog.Source.Outbound);

                                                //string CPOIDSAP = response.TDtlOut[0].Vbeln; //[19 July 2019] Unused, due to use new WS from Vendor PST
                                                string CPOIDSAP = response.T_DTL_OUT[0].VBELN;

                                                Entity UpdateCPO = new Entity(_entityname);
                                                UpdateCPO["new_cpoidsap"] = CPOIDSAP;
                                                UpdateCPO["new_statuscpo"] = new OptionSetValue(STATUSCOP_CPOPROCESSEDTOSAP);
                                                UpdateCPO["new_isconnector"] = "1"; //Validasi CRM Unit yang lama, harus diisi selain "0"
                                                UpdateCPO.Id = Primaryid;

                                                organizationService.Update(UpdateCPO);
                                                #endregion

                                                #region Update Status on CRM - CPO Lines
                                                foreach (var enLines in ecLines.Entities)
                                                {
                                                    Entity UpdateCPOLines = new Entity("salesorderdetail");
                                                    UpdateCPOLines["new_statuscpo"] = new OptionSetValue(STATUSCOP_CPOPROCESSEDTOSAP);
                                                    UpdateCPOLines.Id = enLines.Id;

                                                    organizationService.Update(UpdateCPOLines);
                                                }
                                                #endregion
                                            }
                                        }
                                        else
                                        {
                                            //throw new InvalidPluginExecutionException(string.Format("Result : {0}, Description : {1}", response.TDtlOut[0].Result, response.TDtlOut[0].Description)); //[19 July 2019] Unused, due to use new WS from Vendor PST
                                            throw new InvalidPluginExecutionException(string.Format("Result : {0}, Description : {1}", response.T_DTL_OUT[0].RESULT, response.T_DTL_OUT[0].DESCRIPTION));
                                        }
                                    }
                                }
                                else
                                {
                                    trace.Trace("WebService Response is null/ empty!");
                                    throw new InvalidPluginExecutionException("WebService Response is null/ empty!");
                                }
                            }
                            catch (Exception ex)
                            {
                                _mwsLog.Write(MethodBase.GetCurrentMethod().Name, "Failed to Create Sales Order on SAP. CRM Order Number : "
                                    + OrderNumber + ", Details : " + ex.Message + _mwsLog.ColumnsSeparator + ParameterLog, MWSLog.LogType.Error, MWSLog.Source.Outbound);

                                trace.Trace("Failed Time: " + DateTime.Now.ToString());

                                client.Abort();
                                client.Close();

                                throw new InvalidPluginExecutionException("Error Detail : " + ex.Message.ToString());
                            }
                            finally
                            {
                                client.Close();
                            }
                            #endregion
                        }
                        else
                        {
                            throw new InvalidPluginExecutionException("CPO Product with Product Type Sales Inventory is null/ empty!");
                        }
                    }
                    else
                    {
                        //CreateSOSAP_Header.TDetail = null; //[19 July 2019] Unused, due to use new WS from Vendor PST
                        CreateSOSAP_Header.T_DETAIL = null;
                        throw new InvalidPluginExecutionException("CPO Product is null/ empty!");
                    }
                    #endregion                   
                }
                else
                {
                    throw new InvalidPluginExecutionException("Order ID is null/ empty!");
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".SubmitSalesOrdertoSAP : " + ex.Message.ToString());
            }
        }

        public string GetProductAttachmentDetail(IOrganizationService organizationService, Guid CPOId, string ParentNumber, string UnitDescription)
        {
            string UnitSpecification = UnitDescription;

            QueryExpression qelines = new QueryExpression("salesorderdetail");
            qelines.ColumnSet = new ColumnSet(true);
            qelines.Criteria.AddCondition("salesorderid", ConditionOperator.Equal, CPOId);
            qelines.Criteria.AddCondition("new_parentnumber", ConditionOperator.Equal, ParentNumber);
            EntityCollection ecLines = organizationService.RetrieveMultiple(qelines);
            if (ecLines.Entities.Count > 0)
            {
                foreach (var enLines in ecLines.Entities)
                {
                    if (enLines.Contains("description"))
                    {
                        UnitSpecification += Separator_Description + enLines.GetAttributeValue<String>("description");
                    }
                }
            }

            return UnitSpecification;
        }

        public string EliminateSymbols(string UnitDescription)
        {
            string UnitSpecification = UnitDescription;

            UnitSpecification = UnitSpecification.Replace(" & ", " AND ");
            UnitSpecification = UnitSpecification.Replace("&", " AND ");
            UnitSpecification = UnitSpecification.Split(new[] { '#', '$', '^', '<', '>', '"' }, StringSplitOptions.RemoveEmptyEntries).Aggregate((s, s1) => s + s1);
            UnitSpecification = UnitSpecification.Trim('\'');

            return UnitSpecification;
        }
        #endregion
    }
}
