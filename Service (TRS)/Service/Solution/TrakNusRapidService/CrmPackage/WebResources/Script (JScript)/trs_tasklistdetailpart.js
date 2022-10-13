
function SetMasterPartDescription() {
    var serverUrl = "http://" + window.location.host + "/" + Xrm.Page.context.getOrgUniqueName();
    var part = Xrm.Page.getAttribute("trs_masterpartid").getValue();
    if (part != null) {
        // Creating the Odata Endpoint
        var oDataPath = serverUrl + "/XRMServices/2011/OrganizationData.svc";
        var retrieveReq = new XMLHttpRequest();
        var Odata = oDataPath + "/trs_masterpartSet?$select=trs_name,trs_PartDescription&$filter=trs_masterpartId eq guid'" + part[0].id + "'";
        retrieveReq.open("GET", Odata, false);
        retrieveReq.setRequestHeader("Accept", "application/json");
        retrieveReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        retrieveReq.onreadystatechange = function () { retrieveReqCallBack(this); };
        retrieveReq.send();
        if (retrieveReq.readyState == 4 /* complete */) {
            var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
            if (retrieved.results != null && retrieved.results.length > 0) {
                if (retrieved.results[0].trs_PartDescription != null)
                    Xrm.Page.getAttribute("trs_partdescription").setValue(retrieved.results[0].trs_PartDescription);
            }
        }
    }
}