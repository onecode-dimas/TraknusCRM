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
    Check_IsShippingCentralized();
    DynamicallyFilterSubgrid();
}

function Check_IsShippingCentralized() {
    //debugger;

    var _recordid = GetAttributeValue(Xrm.Page.getAttribute("quoteid"));
    var _isshippingcentralized = false;

    var path = "/QuoteSet?$select=ittn_IsShippingCentralized&$filter=QuoteId eq (guid'" + _recordid[0].id + "')";
    var retrieveReq = RetrieveOData(path);
    if (retrieveReq.readyState == 4 /* complete */) {
        var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
        if (retrieved != null && retrieved.results.length > 0) {
            _isshippingcentralized = retrieved.results[0].ittn_IsShippingCentralized;

            if (_isshippingcentralized) {
                var _deliveryoption_franco = 841150000;
                var _deliveryoption_loco = 841150001;
                var _deliveryoption = GetAttributeValue(Xrm.Page.getAttribute("new_deliveryoption"));

                Xrm.Page.getControl("new_deliveryoption").setDisabled(_isshippingcentralized);

                if (_deliveryoption == _deliveryoption_franco) {
                    Xrm.Page.getControl("ittn_shippingpoint").setDisabled(_isshippingcentralized);
                    Xrm.Page.getControl("ittn_vendor").setDisabled(_isshippingcentralized);
                    Xrm.Page.getControl("ittn_shippingaddress").setDisabled(_isshippingcentralized);
                }
                else {
                    Xrm.Page.getControl("ittn_deliveryplanbranch").setDisabled(_isshippingcentralized);
                }

            }

        }

    }

}

function DynamicallyFilterSubgrid() {
    var objSubGrid = document.getElementById("QuoteConditionType");

    //if (objSubGrid == null || objSubGrid.readyState != "complete") {
    //    setTimeout(DynamicallyFilterSubgrid, 2000);

    //    return;
    //}

    if (objSubGrid == null) {
        setTimeout(DynamicallyFilterSubgrid, 2000);

        return;
    }
    else {
        //debugger;

        var _quoteid = Xrm.Page.getAttribute('quoteid').getValue();
        var _itemnumber = Xrm.Page.getAttribute('new_itemnumber').getValue();
        var _conditionvalue1;
        var _conditionvalue2;
        var _conditionvalue3;

        if (_quoteid != null) {
            _conditionvalue1 = "<condition value='" + _quoteid[0].id + "' attribute='ittn_quote' operator='eq' uitype='quote' />";
        }
        else {
            _conditionvalue1 = "<condition value='00000000-0000-0000-0000-000000000000' attribute='ittn_quote' operator='eq' uitype='quote' />";
        }

        if (_itemnumber != null) {
            _conditionvalue2 = "<condition value='" + _itemnumber + "' attribute='ittn_itemnumber' operator='eq' />";
        }
        else {
            _conditionvalue2 = "<condition value='999' attribute='ittn_itemnumber' operator='eq' />";
        }

        var GUID_PERSONALDISCOUNT = null;
        var GUID_FAT = null;
        var GUID_VOUCHER = null;
        var GUID_OTHERS = null;
        var CODE_PERSONALDISCOUNT = "ZCB0";
        var CODE_FAT = "ZFAT";
        var CODE_VOUCHER = "ZVCR";
        var CODE_OTHERS = "ZO99";
        var path = "/ittn_masterconditiontypeSet?$select=ittn_code,ittn_Description,ittn_masterconditiontypeId&$filter=(ittn_code eq '" + CODE_PERSONALDISCOUNT + "' or ittn_code eq '" + CODE_FAT + "' or ittn_code eq '" + CODE_VOUCHER + "' or ittn_code eq '" + CODE_OTHERS + "')";
        var retrieveReq = RetrieveOData(path);
        if (retrieveReq.readyState == 4 /* complete */) {
            var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
            if (retrieved != null && retrieved.results.length > 0) {
                for (i = 0; i < retrieved.results.length; i++) {
                    if (retrieved.results[i].ittn_code == CODE_PERSONALDISCOUNT) {
                        GUID_PERSONALDISCOUNT = retrieved.results[i].ittn_masterconditiontypeId;
                    }
                    else if (retrieved.results[i].ittn_code == CODE_FAT) {
                        GUID_FAT = retrieved.results[i].ittn_masterconditiontypeId;
                    }
                    else if (retrieved.results[i].ittn_code == CODE_VOUCHER) {
                        GUID_VOUCHER = retrieved.results[i].ittn_masterconditiontypeId;
                    }
                    else if (retrieved.results[i].ittn_code == CODE_OTHERS) {
                        GUID_OTHERS = retrieved.results[i].ittn_masterconditiontypeId;
                    }
                }
            }
        }

        _conditionvalue3 = "<filter type='or'>";
            
        if (GUID_PERSONALDISCOUNT != null) _conditionvalue3 += "<condition value='" + GUID_PERSONALDISCOUNT + "' attribute='ittn_conditiontype' operator='eq' />";
        if (GUID_FAT != null) _conditionvalue3 += "<condition value='" + GUID_FAT + "' attribute='ittn_conditiontype' operator='eq' />";
        if (GUID_VOUCHER != null) _conditionvalue3 += "<condition value='" + GUID_VOUCHER + "' attribute='ittn_conditiontype' operator='eq' />";
        if (GUID_OTHERS != null) _conditionvalue3 += "<condition value='" + GUID_OTHERS + "' attribute='ittn_conditiontype' operator='eq' />";

        _conditionvalue3 += "</filter>";

        var FetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
            "<entity name='ittn_quoteconditiontype'>" +
            "<attribute name='ittn_quoteconditiontypeid' />" +
            "<attribute name='ittn_name' />" +
            "<attribute name='createdon' />" +
            "<order descending='false' attribute='ittn_name' />" +
            "<filter type='and'>" +
            _conditionvalue1 +
            _conditionvalue2 +
            _conditionvalue3 +
            "</filter>" +
            "</entity>" +
            "</fetch>";

        objSubGrid.control.SetParameter("fetchXml", FetchXml);

        objSubGrid.control.Refresh();

    }
}

function CustomRule_ChangeColor() {
    var _result = true;

    return _result;
}

function ChangeColor(grid, selectedItemCount, selectedIds) {
    var actionName = 'new_ITTNActionQuoteChangeColor';
    var confirmMessage = 'Do you want to Change Color?';
    var successMessage = 'Successfully executed Change Color.';

    if (selectedItemCount == 1) {
        ExecuteAction(actionName, selectedIds, confirmMessage, successMessage, function () { grid.Refresh(); });
    }
}

function ExecuteAction(actionName, selectedIds, confirmMessage, successMessage, successCallback, failedCallback) {
    var _return = window.confirm(confirmMessage);

    if (_return) {
        var result = null;
        var recordID = "";

        for (var i = 0; i < selectedIds.length; i++) {
            if (recordID == "")
                recordID = selectedIds[i];
            else
                recordID = recordID + "," + selectedIds[i];
        }

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