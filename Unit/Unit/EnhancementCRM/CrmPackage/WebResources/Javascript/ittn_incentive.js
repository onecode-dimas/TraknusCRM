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
    //ShowGridPrintPreview();
    onLoad_LoadSSRSReport();
}

function ApproveIncentive() {
    var actionName = 'new_ITTNActionIncentiveApproveIncentive';
    var confirmMessage = 'Do you want to Approve Incentive?';
    var successMessage = 'Successfully approve Incentive.';

    ExecuteAction(actionName, confirmMessage, successMessage, function () { Xrm.Page.data.refresh(true); });
}

function CustomRule_ApproveIncentive() {
    var _flag = false;

    var _statusreason = GetAttributeValue(Xrm.Page.getAttribute("ittn_statusreason"));

    if (_statusreason == 841150000 && User_IsApprover()) {
        _flag = true;
    }

    return _flag;
}

function User_IsApprover() {
    var _flag = false;

    var recordid = Xrm.Page.data.entity.getId();
    var userid = Xrm.Page.context.getUserId();
    var path = "/ittn_approvallistincentiveSet?$select=ittn_approvallistincentiveId&$filter=ittn_Incentive/Id eq (guid'" + recordid + "') and ittn_Approver/Id eq (guid'" + userid + "')";
    var retrieveReq = RetrieveOData(path);
    if (retrieveReq.readyState == 4 /* complete */) {
        var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
        if (retrieved != null && retrieved.results.length > 0) {
            _flag = true;
        }
    }

    return _flag;
}

function ShowGridPrintPreview() {
    if (Xrm.Page.ui.tabs.get("tab_printpreview") != null) {
        try {
            var recordID = Xrm.Page.data.entity.getId();
            var path = "/new_incentiveSet?$select=new_incentiveId&$filter=new_incentiveId eq (guid'" + recordID + "') and ittn_StatusReason/Value eq 841150001";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved != null && retrieved.results.length > 0) {
                    Xrm.Page.ui.tabs.get("tab_printpreview").setVisible(true);
                    onLoad_LoadSSRSReport();
                }
                //else {
                //    Xrm.Page.ui.tabs.get("tab_printpreview").setVisible(false);
                //}
            }
        }
        catch (e) {
            throw new Error("SetActType : " + e.Message);
        }
    }
}

function onLoad_LoadSSRSReport() {
    debugger;
    var dataID = Xrm.Page.data.entity.getId();

    if (dataID != "") {
        //First get the page type (Create, Update etc.)
        var pageType = Xrm.Page.ui.getFormType();
        //Then, only proceed if the Form Type DOES NOT equal create, can be changed depending on requirements. Full list of form types can be found here:
        //https://msdn.microsoft.com/en-us/library/gg327828.aspx#BKMK_getFormType
        if (pageType > 1) {
            //Get the value that you want to parameterise
            var guid = Xrm.Page.data.entity.getId();
            //remove braces in guid
            guid = guid.replace(/[{}]/g, "");
            //alert(guid);
            //Now, get the the name of the IFRAME we want to update
            var iFrame = Xrm.Page.ui.controls.get("IFRAME_printpreview");
            //Then, specify the Report Server URL and Report Name.
            var reportURL;

            if (Xrm.Page.context.getClientUrl !== undefined) {
                reportURL = Xrm.Page.context.getClientUrl();
            } else {
                reportURL = Xrm.Page.context.getServerUrl();
            }
                        
            reportURL = reportURL + "/crmreports/viewer/viewer.aspx?" + "action=run&helpID=TRAKNUSReport_IncentiveCombination.rdl&id=%7bB6AD0EBE-D2F3-E911-946E-005056937630%7d";
            reportURL = reportURL + "&p:IncentiveId=";

            var paramaterizedReportURL = reportURL + guid;

            //Finally, if there is no value in the Account Name field, hide the IFRAME; otherwise, update the URL of the IFRAME accordingly.
            if (guid == null) {
                iFrame.setVisible(false);
            }
            else {
                iFrame.setSrc(paramaterizedReportURL);
            }
        }
        else {
            iFrame.setVisible(false);
        }
    }
}
