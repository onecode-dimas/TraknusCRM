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
    User_IsApprover();
    OnChange_Amount();
}

function User_IsApprover() {
    var _flag = false;

    var recordid = Xrm.Page.data.entity.getId();
    var userid = Xrm.Page.context.getUserId();
    var path = "/ittn_approvallistquoteconditiontypeSet?$filter=ittn_QuoteConditionType/Id eq (guid'" + recordid + "') and ittn_Approver/Id eq (guid'" + userid + "')";
    var retrieveReq = RetrieveOData(path);
    if (retrieveReq.readyState == 4 /* complete */) {
        var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
        if (retrieved != null && retrieved.results.length > 0) {
            _flag = true;

            Xrm.Page.getControl("ittn_approveconditiontype").setVisible(true);
        }
    }
    
    return _flag;
}

function OnChange_Amount() {
    debugger;
    var _quote = GetAttributeValue(Xrm.Page.getAttribute("ittn_quote"));
    var _itemnumber = GetAttributeValue(Xrm.Page.getAttribute("ittn_itemnumber"));
    var _amount = GetAttributeValue(Xrm.Page.getAttribute("ittn_amount"));

    if (_amount == null) {
        _amount = 0;
    }

    if (_quote != null && _itemnumber != null) {
        var path = "/QuoteDetailSet?$select=new_Quantity,QuoteDetailId&$filter=QuoteId/Id eq (guid'" + _quote[0].id + "') and new_ItemNumber eq " + _itemnumber + "";
        var retrieveReq = RetrieveOData(path);
        if (retrieveReq.readyState == 4 /* complete */) {
            var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
            if (retrieved != null && retrieved.results.length > 0) {
                var _quantity = retrieved.results[0].new_Quantity;
                var _totalamount = _amount * _quantity;

                Xrm.Page.getAttribute("ittn_totalamount").setValue(_totalamount);
                Xrm.Page.getAttribute("ittn_totalamount").setSubmitMode("always");
            }
        }
    }

}