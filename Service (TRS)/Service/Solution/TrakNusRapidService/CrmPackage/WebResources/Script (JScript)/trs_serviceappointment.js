var newform = false;

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Helper
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function RetrieveOData(path) {
    try {
        var serverUrl = "http://" + window.location.host + "/" + Xrm.Page.context.getOrgUniqueName();

        // Creating the Odata Endpoint
        var oDataPath = serverUrl + "/XRMServices/2011/OrganizationData.svc";
        var retrieveReq = new XMLHttpRequest();
        var OdataPopulation = oDataPath + path;
        retrieveReq.open("GET", OdataPopulation, false);
        retrieveReq.setRequestHeader("Accept", "application/json");
        retrieveReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        retrieveReq.onreadystatechange = function () { retrieveReqCallBack(this); };
        retrieveReq.send();
        return retrieveReq;
    }
    catch (e) {
        throw new Error("RetrieveOData : Failed to retrieve OData !");
    }
}

function SetLookupValue(fieldname, value) {
    try {
        var lookup = new Array();
        lookup[0] = new Object();
        lookup[0].id = value.Id;
        lookup[0].name = value.Name;
        lookup[0].entityType = value.LogicalName;

        Xrm.Page.getAttribute(fieldname).setValue(lookup);
        Xrm.Page.getAttribute(fieldname).setSubmitMode("always");
        Xrm.Page.getAttribute(fieldname).fireOnChange();
    }
    catch (e) {
        throw new Error("SetLookupValue : " + e.message);
    }
}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Publics
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function SetActType(type) {
    try {
        var path = "/trs_acttypeSet?$select=trs_acttypeId&$filter=trs_name eq '" + type + "'";
        var retrieveReq = RetrieveOData(path);
        if (retrieveReq.readyState == 4 /* complete */) {
            var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
            if (retrieved != null && retrieved.results.length > 0) {
                if (retrieved.results[0].trs_acttypeId != null) {
                    var arr = new Array();
                    arr[0] = new Object();
                    arr[0].id = retrieved.results[0].trs_acttypeId;
                    arr[0].name = type;
                    arr[0].entityType = "trs_acttype";
                    Xrm.Page.getAttribute("trs_acttype").setValue(arr);
                    Xrm.Page.getAttribute("trs_acttype").setSubmitMode("always");
                    Xrm.Page.getAttribute("trs_acttype").fireOnChange();
                }
            }
        }
    }
    catch (e) {
        throw new Error("SetActType : " + e.Message);
    }
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FORM ON LOAD AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function Form_OnLoad() {
    try
    {
        if (Xrm.Page.ui.getFormType() < 2)
            newform = true;
        
        if (newform) {
            GetDefaultService();
            RetrieveSRData();
        }
        else {
            Xrm.Page.ui.controls.get("trs_accind").setDisabled(true);
            Xrm.Page.ui.controls.get("serviceid").setDisabled(true);
            Xrm.Page.ui.controls.get("trs_acttype").setDisabled(true);
            Xrm.Page.ui.controls.get("trs_pmacttype").setDisabled(true);
            Xrm.Page.ui.controls.get("trs_branch").setDisabled(true);
            Xrm.Page.ui.controls.get("trs_workcenter").setDisabled(true);
            Xrm.Page.ui.controls.get("trs_plant").setDisabled(true);
            Xrm.Page.ui.controls.get("trs_profitcenter").setDisabled(true);
            Xrm.Page.ui.controls.get("trs_responsiblecctr").setDisabled(true);
            Xrm.Page.ui.controls.get("trs_workshop").setDisabled(true);
            Xrm.Page.ui.controls.get("resources").setDisabled(false);
            Xrm.Page.ui.controls.get("trs_cponsite").setDisabled(true);
            Xrm.Page.ui.controls.get("trs_equipment").setDisabled(true);

            CheckWOIsDispatched();
            FilterLookup();
            PMACTTYPE_OnChange();
        }
    }
    catch (e) {
        alert("Form_OnLoad : " + e.message);
    }
}

function RetrieveSRData() {
    try {
        var SRLookup = Xrm.Page.getAttribute("regardingobjectid").getValue();
        if (SRLookup != null) {
            var path = "/IncidentSet?$select=PrimaryContactId,CustomerId,trs_Unit,Description,trs_PMActType&$filter=IncidentId eq guid'" + SRLookup[0].id + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved.results != null && retrieved.results.length > 0) {
                    var customerid = retrieved.results[0].CustomerId;
                    if (customerid.Id != null)
                        SetLookupValue("customers", customerid);
                    var primarycontact = retrieved.results[0].PrimaryContactId;
                    if (primarycontact.Id != null)
                        SetLookupValue("trs_contactperson", primarycontact);
                    var unit = retrieved.results[0].trs_Unit;
                    if (unit.Id != null)
                        SetLookupValue("trs_equipment", unit);
                    var pmacttype = retrieved.results[0].trs_PMActType;
                    if (pmacttype.Id != null)
                        SetLookupValue("trs_pmacttype", pmacttype);
                    Xrm.Page.getAttribute("trs_customernote").setValue(retrieved.results[0].Description);
                }
            }
        }
        else {
            throw new Error("undefined");
        }
    }
    catch (e) {
        if (e.Message == "undefined")
            alert ("RetrieveSRData : Please open this window from SR window");
        else
            alert ("RetrieveSRData : " + e.Message);
    }
}

function CheckWOIsDispatched() {
    try {
        var WOIsDispatched = Xrm.Page.getAttribute("trs_isdispatched").getValue();

        if (WOIsDispatched != null) {
            if (WOIsDispatched == true) {
                Xrm.Page.ui.controls.get("trs_mechanicleader").setDisabled(true);
            }
            else {
                Xrm.Page.ui.controls.get("trs_mechanicleader").setDisabled(false);
            }
        }
    }
    catch (e) {
        alert ("CheckWOIsDispatched : " + e.message);
    }
}

function GetDefaultService() {
    try {
        var path = "/ServiceSet?$filter=Name eq 'Service Activity'";
        var retrieveReq = RetrieveOData(path);
        if (retrieveReq.readyState == 4 /* complete */) {
            var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
            if (retrieved.results != null && retrieved.results.length > 0) {
                if (retrieved.results[0].ServiceId != null) {
                    var arr = new Array();
                    arr[0] = new Object();
                    arr[0].id = retrieved.results[0].ServiceId;
                    arr[0].name = retrieved.results[0].Name;
                    arr[0].entityType = "service";
                    Xrm.Page.getAttribute("serviceid").setValue(arr);
                    Xrm.Page.getAttribute("serviceid").setSubmitMode("always");
                    Xrm.Page.getAttribute("serviceid").fireOnChange();
                }
            }
        }
    }
    catch (e) {
        alert ("GetDefaultService : " + e.message);
    }
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FORM ON SAVE AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function Form_OnSave() {
    try {
        OnSave_AlwaysSubmit();
        OnSave_ClearActualStartDate();
    }
    catch (e) {
        alert("Form_OnSave : " + e.message);
    }
}

function OnSave_ClearActualStartDate() {
    //On Create , Check actualstart and make it's value null
    try {
        if (newform)
            Xrm.Page.getAttribute("actualstart").setValue("");
    }
    catch (e) {
        alert ("OnSave_ClearActualStartDate : " + e.message);
    }
}

function OnSave_AlwaysSubmit() {
    try {
        //Customer detail
        Xrm.Page.getAttribute("trs_phone").setSubmitMode("always");
        Xrm.Page.getAttribute("trs_npwp").setSubmitMode("always");
        Xrm.Page.getAttribute("trs_address").setSubmitMode("always");
        //Contact person detail
        Xrm.Page.getAttribute("trs_cpphone").setSubmitMode("always");
        //Contact person onsite detail
        Xrm.Page.getAttribute("trs_phoneonsite").setSubmitMode("always");
        //Equipment detail
        Xrm.Page.getAttribute("new_serialnumber").setSubmitMode("always");
        Xrm.Page.getAttribute("trs_product").setSubmitMode("always");
        Xrm.Page.getAttribute("trs_productmodel").setSubmitMode("always");
        Xrm.Page.getAttribute("new_deliverydate").setSubmitMode("always");
        Xrm.Page.getAttribute("trs_enginenumber").setSubmitMode("always");
        Xrm.Page.getAttribute("trs_chasisnumber").setSubmitMode("always");
        Xrm.Page.getAttribute("trs_hourmeter").setSubmitMode("always");
        //Functional location detail
        Xrm.Page.getAttribute("location").setSubmitMode("always");
    }
    catch (e) {
        alert ("OnSave_AlwaysSubmit : " + e.message);
    }
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FIELD EVENT AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function Customers_OnChange() {
    try {
        Xrm.Page.getAttribute("trs_phone").setValue(null);
        Xrm.Page.getAttribute("trs_npwp").setValue(null);
        Xrm.Page.getAttribute("trs_address").setValue(null);

        var customerLookup = Xrm.Page.getAttribute("customers").getValue();
        if (customerLookup != null) {
            var customerid = customerLookup[0].id;
            var path = "/AccountSet?$select=Address1_Name,new_NPWP,Telephone1,PrimaryContactId&$filter=AccountId eq guid'" + customerid + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                FilterContactLookup();
                FilterContactonSiteLookup();

                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved.results != null && retrieved.results.length > 0) {
                    if (retrieved.results[0].Telephone1 != null)
                        Xrm.Page.getAttribute("trs_phone").setValue(retrieved.results[0].Telephone1);
                    if (retrieved.results[0].new_NPWP != null)
                        Xrm.Page.getAttribute("trs_npwp").setValue(retrieved.results[0].new_NPWP);
                    if (retrieved.results[0].Address1_Name)
                        Xrm.Page.getAttribute("trs_address").setValue(retrieved.results[0].Address1_Name);
                    var primaryContactId = retrieved.results[0].PrimaryContactId;
                    if (primaryContactId.Id != null)
                        SetLookupValue("trs_contactperson", primaryContactId);
                }
            }
        }
    }
    catch (e) {
        alert("Customer_OnChange : " + e.message);
    }
}

function ContactPerson_OnChange() {
    try {
        Xrm.Page.getAttribute("trs_cpphone").setValue(null);

        var contactLookup = Xrm.Page.getAttribute("trs_contactperson").getValue();
        if (contactLookup != null) {
            var contactid = contactLookup[0].id;
            var path = "/ContactSet?$select=Telephone1&$filter=ContactId eq guid'" + contactid + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved.results != null && retrieved.results.length > 0) {
                    if (retrieved.results[0].Telephone1 != null)
                        Xrm.Page.getAttribute("trs_cpphone").setValue(retrieved.results[0].Telephone1);
                }
            }
        }
    }
    catch (e) {
        alert("ContactPerson_OnChange : " + e.message);
    }
}

function SetContactPersonOnSite_OnChange() {
    try {
        Xrm.Page.getAttribute("trs_phoneonsite").setValue(null);

        var contactLookup = Xrm.Page.getAttribute("trs_cponsite").getValue();
        if (contactLookup != null) {
            var contactid = contactLookup[0].id;
            var path = "/ContactSet?$select=Telephone1&$filter=ContactId eq guid'" + contactid + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved.results != null && retrieved.results.length > 0) {
                    if (retrieved.results[0].Telephone1 != null)
                        Xrm.Page.getAttribute("trs_phoneonsite").setValue(retrieved.results[0].Telephone1);
                }
            }
        }
    }
    catch (e) {
        alert("SetContactPersonOnSite_OnChange : " + e.message);
    }
}

function Equipment_OnChange() {
    try {
        Xrm.Page.getAttribute("new_serialnumber").setValue(null);
        Xrm.Page.getAttribute("trs_product").setValue(null);
        Xrm.Page.getAttribute("trs_productmodel").setValue(null);
        Xrm.Page.getAttribute("new_deliverydate").setValue(null);
        Xrm.Page.getAttribute("trs_enginenumber").setValue(null);
        Xrm.Page.getAttribute("trs_chasisnumber").setValue(null);
        Xrm.Page.getAttribute("trs_hourmeter").setValue(null);
        Xrm.Page.getAttribute("trs_oldhm").setValue(null);

        var equipmentLookup = Xrm.Page.getAttribute("trs_equipment").getValue();
        if (equipmentLookup != null) {
            var path = "/new_populationSet?$select=new_DeliveryDate,new_enginenumber,new_Model,new_ProductName,new_SerialNumber,trs_ChasisNumber,trs_FunctionalLocation,trs_HourMeter,trs_OldHM,new_StatusInOperation&$filter=new_populationId eq guid'" + equipmentLookup[0].id + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved != null && retrieved.results.length > 0) {
                    if (retrieved.results[0].new_SerialNumber != null)
                        Xrm.Page.getAttribute("new_serialnumber").setValue(retrieved.results[0].new_SerialNumber);
                    if (retrieved.results[0].new_ProductName != null)
                        Xrm.Page.getAttribute("trs_product").setValue(retrieved.results[0].new_ProductName);
                    if (retrieved.results[0].new_Model != null)
                        Xrm.Page.getAttribute("trs_productmodel").setValue(retrieved.results[0].new_Model);
                    if (retrieved.results[0].new_DeliveryDate != null)
                        Xrm.Page.getAttribute("new_deliverydate").setValue(new Date(parseInt(retrieved.results[0].new_DeliveryDate.replace("/Date(", "").replace(")/", ""), 10)));
                    if (retrieved.results[0].new_enginenumber != null)
                        Xrm.Page.getAttribute("trs_enginenumber").setValue(retrieved.results[0].new_enginenumber);
                    if (retrieved.results[0].trs_ChasisNumber != null)
                        Xrm.Page.getAttribute("trs_chasisnumber").setValue(retrieved.results[0].trs_ChasisNumber);
                    if (retrieved.results[0].trs_HourMeter != null)
                        Xrm.Page.getAttribute("trs_hourmeter").setValue(Number(retrieved.results[0].trs_HourMeter));
                    if (retrieved.results[0].trs_OldHM != null)
                        Xrm.Page.getAttribute("trs_oldhm").setValue(Number(retrieved.results[0].trs_OldHM));
                    if (retrieved.results[0].new_StatusInOperation != null)
                        Xrm.Page.getAttribute("trs_statusinoperation").setValue(eval(retrieved.results[0].new_StatusInOperation.Value));

                    var functionalLocation = retrieved.results[0].trs_FunctionalLocation;
                    if (functionalLocation.Id != null)
                        SetLookupValue("trs_functionallocation", functionalLocation);
                }
            }
        }
    }
    catch (e) {
        alert("Equipment_OnChange : " + e.message);
    }
}

function Mechanics_OnChange() {
    try {
        FilterMechanicLeaderandSubLeader();
    }
    catch (e) {
        alert("Mechanics_OnChange : " + e.message);
    }
}

function PMACTTYPE_OnChange() {
    try {
        Xrm.Page.getAttribute("trs_assemblingtype").setRequiredLevel("none");
        Xrm.Page.getControl("trs_assemblingtype").setVisible(false);

        var pma = Xrm.Page.getAttribute("trs_pmacttype").getValue();
        if (pma != null) {
            var path = "/trs_tasklistgroupSet?$select=trs_pmacttype&$filter=trs_tasklistgroupId eq guid'" + pma[0].id + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved != null && retrieved.results.length > 0) {
                    if (retrieved.results[0].trs_pmacttype == '003') {
                        Xrm.Page.getControl("trs_assemblingtype").setVisible(true);
                        Xrm.Page.getAttribute("trs_assemblingtype").setRequiredLevel("required");
                    }
                }
            }
        }
    }
    catch (e) {
        alert("PMACTTYPE_OnChange : " + e.message);
    }
}

function AccIndic_OnChange() {
    try {
        var acc = Xrm.Page.getAttribute("trs_accind").getValue();
        if (acc != null && acc != "undefinied") {
            if (acc == 1)
                SetActType("GENRP1");
            else if (acc == 2)
                SetActType("SERV");
            else if (acc == 3)
                SetActType("WARNTY");
            else if (acc == 4)
                SetActType("SERV");
        }
    }
    catch (e) {
        alert("AccIndic_OnChange : " + e.message);
    }
}

function FunctionalLocation_OnChange() {
    try {
        Xrm.Page.getAttribute("location").setValue(null);
        Xrm.Page.getAttribute("trs_branch").setValue(null);
        Xrm.Page.getAttribute("trs_plant").setValue(null);

        var functionallocationLookup = Xrm.Page.getAttribute("trs_functionallocation").getValue();
        if (functionallocationLookup != null) {
            var functionallocationid = functionallocationLookup[0].id;
            var path = "/trs_functionallocationSet?$select=trs_Branch,trs_FunctionalAddress,trs_Plant&$filter=trs_functionallocationId eq guid'" + functionallocationid + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved.results != null && retrieved.results.length > 0) {
                    var branch = retrieved.results[0].trs_Branch;
                    if (branch != null)
                        SetLookupValue("trs_branch", branch);
                    var plant = retrieved.results[0].trs_Plant;
                    if (plant != null)
                        SetLookupValue("trs_plant", plant);
                    if (retrieved.results[0].trs_FunctionalAddress != null)
                        Xrm.Page.getAttribute("location").setValue(retrieved.results[0].trs_FunctionalAddress);
                }
            }
        }
    }
    catch (e) {
        alert("FunctionalLocation_OnChange : " + e.message);
    }
}

function Plant_OnChange() {
    try {
        FilterResposibleCostCenter();
    }
    catch (e) {
        alert("Plant_OnChange : " + e.message);
    }
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FILTERING LOOKUP AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function FilterLookup() {
    try {
        FilterContactLookup();
        FilterContactonSiteLookup();
        FilterMechanicLeaderandSubLeader();
        FilterResposibleCostCenter();
    }
    catch (e) {
        alert("FilterLookup : " + e.message);
    }
}

function FilterContactLookup() {
    try
    {
        Xrm.Page.getControl("trs_contactperson").removePreSearch();
        var cust = Xrm.Page.getAttribute("customers").getValue();
        if (cust != null) {
            var cond = "";
            for (var i = 0; i < cust.length; i++) {
                cond += "<condition attribute='parentcustomerid' operator='eq' value='" + cust[i].id + "' />";
            }
            var fetchXml = "<filter type='and'><filter type='or'>" + cond + "</filter></filter>";

            Xrm.Page.getControl("trs_contactperson").addPreSearch(function () {
                Xrm.Page.getControl("trs_contactperson").addCustomFilter(fetchXml);
            });
        }
    }
    catch (e) {
        throw new Error("FilterContactLookup : " + e.message);
    }
}

function FilterContactonSiteLookup() {
    try {
        Xrm.Page.getControl("trs_cponsite").removePreSearch();
        var cust = Xrm.Page.getAttribute("customers").getValue();
        if (cust != null) {
            var cond = "";
            for (var i = 0; i < cust.length; i++) {
                cond += "<condition attribute='parentcustomerid' operator='eq' value='" + cust[i].id + "' />";
            }
            var fetchXml = "<filter type='and'><filter type='and'><condition attribute='trs_customerreference' operator='eq' value='2' /><filter type='or'>" + cond + "</filter></filter></filter>";

            Xrm.Page.getControl("trs_cponsite").addPreSearch(function () {
                Xrm.Page.getControl("trs_cponsite").addCustomFilter(fetchXml);
            });
        }
    }
    catch (e) {
        throw new Error("FilterContactonSiteLookup : " + e.message);
    }
}

function FilterMechanicLeaderandSubLeader() {
    try {
        Xrm.Page.getControl("trs_mechanicleader").removePreSearch();
        Xrm.Page.getControl("trs_mechanicsubleader").removePreSearch();
        var mechanic = Xrm.Page.getAttribute("resources").getValue();
        if (mechanic != null) {
            var cond = "";
            for (var i = 0; i < mechanic.length; i++) {

                cond += "<condition attribute='equipmentid' operator='eq' value='" + mechanic[i].id + "' />";
            }
            fetchXml = "<filter type='and'>" + "<filter type='or'>" + cond + "</filter>" + "</filter>";

            Xrm.Page.getControl("trs_mechanicleader").addPreSearch(function () {
                Xrm.Page.getControl("trs_mechanicleader").addCustomFilter(fetchXml);
            });
            Xrm.Page.getControl("trs_mechanicsubleader").addPreSearch(function () {
                Xrm.Page.getControl("trs_mechanicsubleader").addCustomFilter(fetchXml);
            });
        }
    }
    catch (e) {
        throw new Error("FilterMechanicLeaderandSubLeader : " + e.message);
    }
}

function FilterResposibleCostCenter() {
    try {
        var plant = Xrm.Page.getAttribute("trs_plant").getValue();
        if (plant != null) {
            var path = "/SiteSet?$select=Name&$filter=SiteId eq guid'" + plant[0].id + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved != null && retrieved.results.length > 0) {
                    str = retrieved.results[0].Name;
                    Xrm.Page.getControl("trs_responsiblecctr").addPreSearch(function () {
                        fetchXml = "<filter type='and'><condition attribute='trs_costcenter' operator='like' value='%" + str + "%' /></filter>";
                        Xrm.Page.getControl("trs_responsiblecctr").addCustomFilter(fetchXml);
                    });
                }
            }
        }
    }
    catch (e) {
        throw new Error("FilterResposibleCostCenter : " + e.message);
    }
}