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
        //retrieveReq.onreadystatechange = function () { retrieveReqCallBack(this); };
        retrieveReq.send();
        return retrieveReq;
    }
    catch (e) {
        throw new Error("RetrieveOData : Failed to retrieve OData !");
    }
}

function GetAttributeValue(_attribute) {
    var _result = null;

    if (_attribute != null) {
        if (_attribute.getValue() != null) {
            _result = _attribute.getValue();
        }
    }

    return _result;
}

function Form_ForceClose() {
    var attributes = Xrm.Page.data.entity.attributes.get();
    for (var i in attributes)
    { attributes[i].setSubmitMode("never"); }

    if (parent.opener != undefined) { window.parent.close(); } else Xrm.Page.ui.close();
}

function OnLoad() {
    Check_SalesOrderDetail();
}

function Check_SalesOrderDetail() {
    debugger;

    var recordID = Xrm.Page.data.entity.getId();
    var _serialnumber = GetAttributeValue(Xrm.Page.getAttribute("new_serialnumber"));

    var path = "/SalesOrderDetailSet?$select=SalesOrderId&$filter=new_SerialNumber eq '" + _serialnumber + "'";
    var retrieveReq = RetrieveOData(path);
    if (retrieveReq.readyState == 4 /* complete */) {
        var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
        if (retrieved != null && retrieved.results.length > 0) {
            var _salesorderid = retrieved.results[0].SalesOrderId;

            Check_CPOConditionType(_salesorderid);
        }
    }
}

function Check_CPOConditionType(_salesorderid) {
    debugger;

    var recordID = Xrm.Page.data.entity.getId();
    var path = "/ittn_cpoconditiontypeSet?$select=ittn_cpoconditiontypeId&$filter=ittn_CPO/Id eq (guid'" + _salesorderid.Id + "') and ittn_Amount/Value eq 0";
    var retrieveReq = RetrieveOData(path);
    if (retrieveReq.readyState == 4 /* complete */) {
        var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
        if (retrieved != null && retrieved.results.length > 0) {
            alert("Condition Type for CPO (Sales Order) with amount 0 is found ! Add New Activity 'Working Order' is failed !");

            Form_ForceClose();
        }
    }
}

