var formstatus = 1;

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
function GetPartPrice(partId, pricelistId) {
    try {
        var price = 0;
        var path = "/trs_partpricemasterSet?$select=trs_Price&$filter=trs_PartMaster/Id eq guid'" + partId;
        path += "' and trs_PriceList/Id eq guid'" + pricelistId + "'";
        var retrieveReq = RetrieveOData(path);
        if (retrieveReq.readyState == 4 /* complete */) {
            var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
            if (retrieved.results != null && retrieved.results.length > 0) {
                if (retrieved.results[0].trs_Price != null) {
                    var price = retrieved.results[0].trs_Price.Value;
                }
            }
        }
        return price;
    }
    catch (e) {
        throw new Error("GetPartPrice : " + e.message);
    }
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FORM ON LOAD AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function PartPriceMaster_Form_OnLoad() {
    try {
        formstatus = Xrm.Page.ui.getFormType();

        if (formstatus < 2) {
            PartMaster_OnChange();
        }
    }
    catch (e) {
        alert("Form_OnLoad : " + e.message);
    }
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FORM ON SAVE AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function PartPriceMaster_Form_OnSave() {
    try {
    }
    catch (e) {
        alert("Form_OnSave : " + e.message);
    }
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FIELD EVENT AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function PartMaster_OnChange() {
    try {
        Xrm.Page.getAttribute("trs_partname").setValue(null);

        var PartMasterLookup = Xrm.Page.getAttribute("trs_partmaster").getValue();
        if (PartMasterLookup != null) {
            var path = "/trs_masterpartSet?$select=trs_PartDescription&$filter=trs_masterpartId eq guid'" + PartMasterLookup[0].id + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved.results != null && retrieved.results.length > 0) {
                    if (retrieved.results[0].trs_PartDescription != null) {
                        Xrm.Page.getAttribute("trs_partname").setValue(retrieved.results[0].trs_PartDescription);
                    }
                }
            }
        }
    }
    catch (e) {
        alert("PartMaster_OnChange : " + e.message);
    }
}

function PriceList_OnChange() {
    try {
        Xrm.Page.getAttribute("transactioncurrencyid").setValue(null);

        var PriceListLookup = Xrm.Page.getAttribute("trs_pricelist").getValue();
        if (PriceListLookup != null) {
            SetLookupValue("transactioncurrencyid", GetDefaultPriceListCPOCurrency(PriceListLookup[0].id));
        }
    }
    catch (e) {
        alert("PriceList_OnChange : " + e.message);
    }
}