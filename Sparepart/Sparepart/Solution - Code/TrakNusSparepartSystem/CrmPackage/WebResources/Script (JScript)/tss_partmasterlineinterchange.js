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

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FORM ON LOAD AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function Form_OnLoad() {
    try {
        var formstatus = Xrm.Page.ui.getFormType();

        if (formstatus < 2) {
            Xrm.Page.getAttribute("tss_partnumberinterchange").setValue(null);
        }
    }
    catch (e) {
        alert("Form_OnLoad : " + e.message);
    }
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FIELD EVENT AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function PartNumberInterchange_OnChange() {
    try {
        Xrm.Page.getAttribute("tss_partnumber").setValue(null);

        var PartNumberInterchangeLookup = Xrm.Page.getAttribute("tss_partnumberinterchange").getValue();
        if (PartNumberInterchangeLookup != null) {
            var path = "/trs_masterpartSet?$select=trs_name&$filter=trs_masterpartId eq guid'" + PartNumberInterchangeLookup[0].id + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved.results != null && retrieved.results.length > 0) {
                    if (retrieved.results[0].trs_name != null) {
                        Xrm.Page.getAttribute("tss_partnumber").setValue(retrieved.results[0].trs_name);
                        Xrm.Page.getAttribute("tss_partnumber").setSubmitMode("always");
                    }
                }
            }
        }
    }
    catch (e) {
        alert("PartMaster_OnChange : " + e.message);
    }
}