function RetrieveTaskList(HeaderRetrieveReq) {
    try {
        if (HeaderRetrieveReq.readyState == 4 /* complete */) {
            var FieldRetrieved = this.parent.JSON.parse(HeaderRetrieveReq.responseText).d;

            if (FieldRetrieved != null && FieldRetrieved.results.length > 0) {
                if (FieldRetrieved.results[0].trs_TaskName != null) {
                    var arrTask = new Array();
                    arrTask[0] = new Object();
                    arrTask[0].id = FieldRetrieved.results[0].trs_TaskName.Id;
                    arrTask[0].name = FieldRetrieved.results[0].trs_TaskName.Name;
                    arrTask[0].entityType = "trs_tasklist";
                    Xrm.Page.getAttribute("trs_taskname").setValue(arrTask);
                }
                if (FieldRetrieved.results[0].trs_MechanicGrade != null) {
                    var arrMec = new Array();
                    arrMec[0] = new Object();
                    arrMec[0].id = FieldRetrieved.results[0].trs_MechanicGrade.Id;
                    arrMec[0].name = FieldRetrieved.results[0].trs_MechanicGrade.Name;
                    arrMec[0].entityType = "trs_mechanicgrade";
                    Xrm.Page.getAttribute("trs_mechanicgrade").setValue(arrMec);
                }
                if (FieldRetrieved.results[0].trs_RTG != null) {
                    Xrm.Page.getAttribute("trs_rtg").setValue(parseFloat(FieldRetrieved.results[0].trs_RTG));
                }
                Xrm.Page.getAttribute("trs_taskname").setSubmitMode("always");
                Xrm.Page.getAttribute("trs_mechanicgrade").setSubmitMode("always");
                Xrm.Page.getAttribute("trs_rtg").setSubmitMode("always");
            }
        }
    }
    catch (e) {
        alert(e.message);
    }
}

function GetTaskListData() {
    try {
        var serverUrl = "http://" + window.location.host + "/" + Xrm.Page.context.getOrgUniqueName();
        var oDataPath = serverUrl + "/XRMServices/2011/OrganizationData.svc";
        var HeaderRetrieveReq = new XMLHttpRequest();
        var task = Xrm.Page.getAttribute("trs_commercialtask").getValue();
        var Odata = oDataPath + "/trs_commercialtaskSet?$select=trs_MechanicGrade,trs_RTG,trs_TaskName&$filter=trs_commercialtaskId eq guid'" + task[0].id + "'";

        HeaderRetrieveReq.open("GET", Odata, false);
        HeaderRetrieveReq.setRequestHeader("Accept", "application/json");
        HeaderRetrieveReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        HeaderRetrieveReq.onreadystatechange = function () { RetrieveTaskList(this); };
        HeaderRetrieveReq.send();
    }
    catch (e) {
        alert(e.Message);
    }
}