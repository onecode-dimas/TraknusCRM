function setToolsMaster() {
    var serverUrl = "http://" + window.location.host + "/" + Xrm.Page.context.getOrgUniqueName();
    var ToolsMasterLookup = Xrm.Page.getAttribute("trs_toolsmaster").getValue();

    Xrm.Page.getAttribute("trs_toolsname").setValue(null);

    if (ToolsMasterLookup != null) {
        var ToolsMasterLookupID = ToolsMasterLookup[0].id;

        // Creating the Odata Endpoint
        var oDataPath = serverUrl + "/XRMServices/2011/OrganizationData.svc";
        var retrieveReq = new XMLHttpRequest();
        var Odata = oDataPath + "/trs_toolsmasterSet?$select=trs_toolsname&$filter=trs_toolsmasterId eq guid'" + ToolsMasterLookupID + "'";
        retrieveReq.open("GET", Odata, false);
        retrieveReq.setRequestHeader("Accept", "application/json");
        retrieveReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        retrieveReq.onreadystatechange = function () { retrieveReqCallBack(this); };
        retrieveReq.send();
        if (retrieveReq.readyState == 4) {
            var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
            if (retrieved.results != null && retrieved.results.length > 0) {
                if (retrieved.results[0].trs_toolsname != null) {
                    Xrm.Page.getAttribute("trs_toolsname").setValue(retrieved.results[0].trs_toolsname);
                }
            }
        }
    }
}