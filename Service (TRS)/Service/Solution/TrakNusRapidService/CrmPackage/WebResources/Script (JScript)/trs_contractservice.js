function required(field, val) {
    Xrm.Page.getControl(field).setRequiredLevel(val);
}

function visible(field, val) {
    Xrm.Page.getControl(field).setVisible(val);
}

function setMaintenance() {
    var type = Xrm.Page.getAttribute("trs_maintenanceperiod").getValue();
    if (type == false) {
        visible("trs_periode", true);
        required("trs_periode", "required");
        required("trs_realhourmeter", "none");
        required("trs_toleransibawah", "none");
        required("trs_toleransiatas", "none");
        visible("trs_realhourmeter", false);
        visible("trs_toleransibawah", false);
        visible("trs_toleransiatas", false);
    }
    else {
        required("trs_periode", "none");
        visible("trs_periode", false);
        visible("trs_realhourmeter", true);
        required("trs_realhourmeter", "required");
        visible("trs_toleransibawah", true);
        required("trs_toleransibawah", "required");
        visible("trs_toleransiatas", true);
        required("trs_toleransiatas", "required");
    }
}


function SetCommercialNameOnTaskHeaderChange() {
    var serverUrl = "http://" + window.location.host + "/" + Xrm.Page.context.getOrgUniqueName();
    //Xrm.Page.getAttribute("subject").setValue(null);
    var TaskHeaderLookup = Xrm.Page.getAttribute("trs_header").getValue();
    if (TaskHeaderLookup != null) {
        var TaskHeaderid = TaskHeaderLookup[0].id;

        // Creating the Odata Endpoint
        var oDataPath = serverUrl + "/XRMServices/2011/OrganizationData.svc";
        var retrieveReq = new XMLHttpRequest();
        var Odata = oDataPath + "/trs_tasklistheaderSet?$select=trs_TaskListGroup&$filter=trs_tasklistheaderId eq guid'" + TaskHeaderid + "'";
        retrieveReq.open("GET", Odata, false);
        retrieveReq.setRequestHeader("Accept", "application/json");
        retrieveReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        retrieveReq.onreadystatechange = function () { retrieveReqCallBack(this); };
        retrieveReq.send();
        if (retrieveReq.readyState == 4 /* complete */) {
            var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
            if (retrieved.results != null && retrieved.results.length > 0) {
                if (retrieved.results[0].trs_TaskListGroup != null) {
                    var tasklistgroupid = retrieved.results[0].trs_TaskListGroup.Id;
                    var odata1 = oDataPath + "/trs_tasklistgroupSet?$select=trs_tasklistgroupname&$filter=trs_tasklistgroupId eq guid'" + tasklistgroupid + "'";
                    var retrieveReq1 = new XMLHttpRequest();
                    retrieveReq1.open("GET", odata1, false);
                    retrieveReq1.setRequestHeader("Accept", "application/json");
                    retrieveReq1.setRequestHeader("Content-Type", "application/json; charset=utf-8");
                    retrieveReq1.onreadystatechange = function () { retrieveReqCallBack(this); };
                    retrieveReq1.send();
                    if (retrieveReq1.readyState == 4) {
                        var retrievedData = this.parent.JSON.parse(retrieveReq1.responseText).d;
                        if (retrievedData.results[0].trs_tasklistgroupname != null) {
                            Xrm.Page.getAttribute("trs_commercialname").setValue(retrievedData.results[0].trs_tasklistgroupname);
                            Xrm.Page.getAttribute("trs_commercialname").setSubmitMode("always");
                        }
                    }
                }
            }
        }
    }
}