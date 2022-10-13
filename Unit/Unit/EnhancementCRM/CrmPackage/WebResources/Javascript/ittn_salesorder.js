function ExecuteAction(actionName, confirmMessage, successMessage, successCallback, failedCallback) {
    debugger;
    var _return = window.confirm(confirmMessage);

    if (_return) {
        var result = null;
        var recordID = Xrm.Page.data.entity.getId();

        var parameters = {
            "RecordID": recordID
        };

        // Creating the Odata Endpoint
        var oDataPath = Xrm.Page.context.getClientUrl() + "/api/data/v8.0/";
        var req = new XMLHttpRequest();
        req.open("POST", oDataPath + actionName, true);
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState == 4) {
                req.onreadystatechange = null;
                if (this.status == 200 || this.status == 204) {
                    Xrm.Utility.alertDialog(successMessage);
                    //location.reload();
                    //AssignResponse(req, 'Successfully executed Generate Prospect Part Header.', successCallback, failedCallback);
                    if (successCallback !== undefined && typeof successCallback === "function") {
                        successCallback();
                    }
                }
                else {
                    var error = "";
                    if (this.response != null) {
                        error = JSON.parse(this.response).error.message;
                    }
                    Xrm.Utility.alertDialog('Fail to Execute.\r\nResponse Code : (' + req.status + ')' + "\r\nPlease contact your IT support.\r\nTechnical Details : " + error);
                }
            }
        };

        req.send(JSON.stringify(parameters));
    }
}

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

function OnLoad() {
    
}

function RequestApproveSalesEffort() {
    var actionName = 'new_ITTNActionCPORequestApproveSalesEffort';
    var confirmMessage = 'Do you want to Request Approve Sales Effort ?';
    var successMessage = 'Successfully executed Request Approve Sales Effort.';

    ExecuteAction(actionName, confirmMessage, successMessage, function () { Xrm.Page.data.refresh(true); });
}

function CustomRule_RequestApproveSalesEffort() {
    var _flag = false;

    try {
        var recordID = Xrm.Page.data.entity.getId();
        var path = "/ittn_cposaleseffortSet?$select=ittn_cposaleseffortId&$filter=ittn_CPO/Id eq (guid'" + recordID + "') and  ittn_ApproveSalesEffortDate eq null and  ittn_ApproveSalesEffortBy eq null";
        var retrieveReq = RetrieveOData(path);
        if (retrieveReq.readyState == 4 /* complete */) {
            var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
            if (retrieved != null && retrieved.results.length > 0) {
                _flag = true;
            }
        }
    }
    catch (e) {
        throw new Error("SetActType : " + e.Message);
    }

    return _flag;
}