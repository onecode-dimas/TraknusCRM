var formstatus = 1;
var formLabel = '';
var formInformation = 'Information';
var formIDR = 'Sparepart Price Master IDR';
var formJPY = 'Sparepart Price Master JPY';
var currencyIDR = 'IDR';
var currencyJPY = 'JPY';

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

function DisableField(ControlName) {
    try {
        Xrm.Page.getControl(ControlName).setDisabled(true);
    }
    catch (e) {
        throw new Error("DisableField : " + e.message);
    }
}

function EnableField(ControlName) {
    try {
        Xrm.Page.getControl(ControlName).setDisabled(false);
    }
    catch (e) {
        throw new Error("EnableField : " + e.message);
    }
}

function addLookupFilter(currencyType) {
    fetchXml = "<filter type='and'><condition attribute='currencyname' operator='eq' value='" + currencyType + "' /></filter>";
    Xrm.Page.getControl("transactioncurrencyid").addCustomFilter(fetchXml);
}

function preFilterLookup(currencyType) {
    Xrm.Page.getControl("transactioncurrencyid")._control && Xrm.Page.getControl("transactioncurrencyid")._control.tryCompleteOnDemandInitialization && Xrm.Page.getControl("transactioncurrencyid")._control.tryCompleteOnDemandInitialization();
    Xrm.Page.getControl("transactioncurrencyid").removePreSearch();

    if (currencyType != null) {
        Xrm.Page.getControl("transactioncurrencyid").addPreSearch(function () {
            addLookupFilter(currencyType);
        });
    }
}

function SetLookupNull(lookupAttribute) {
    var lookupObject = Xrm.Page.getAttribute(lookupAttribute);

    if (lookupObject != null) {
        Xrm.Page.getAttribute(lookupAttribute).setValue(null);
    }
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FORM ON LOAD AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function SparepartPriceMaster_Form_OnLoad() {
    try {
        formstatus = Xrm.Page.ui.getFormType();
        formLabel = Xrm.Page.ui.formSelector.getCurrentItem().getLabel();

        if (formstatus < 2) {
            //SetLookupNull('transactioncurrencyid');

            if (formLabel == formInformation) {
                preFilterLookup(null);
            }
            else {
                if (formLabel == formIDR) {
                    preFilterLookup(currencyIDR);
                }

                if (formLabel == formJPY) {
                    preFilterLookup(currencyJPY);
                }
            }

            PartMaster_OnChange();
        }
    }
    catch (e) {
        alert("Form_OnLoad : " + e.message);
    }
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FIELD EVENT AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function PartMaster_OnChange() {
    try {
        Xrm.Page.getAttribute("tss_partname").setValue(null);

        var PartMasterLookup = Xrm.Page.getAttribute("tss_partmaster").getValue();
        if (PartMasterLookup != null) {
            var path = "/trs_masterpartSet?$select=trs_PartDescription&$filter=trs_masterpartId eq guid'" + PartMasterLookup[0].id + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved.results != null && retrieved.results.length > 0) {
                    if (retrieved.results[0].trs_PartDescription != null) {
                        Xrm.Page.getAttribute("tss_partname").setValue(retrieved.results[0].trs_PartDescription);
                    }
                }
            }
        }
    }
    catch (e) {
        alert("PartMaster_OnChange : " + e.message);
    }
}