function GetAttributeValue(attribute) {
    var result = null;

    if (attribute != null) {
        if (attribute.getValue() != null) {
            result = attribute.getValue();
        }
    }

    return result;
}

function showSubGridButtons(gridname, visible) {
    try {
        var subgridsLoaded = false;

        if ($("div[id$='" + gridname + "']").length > 0 && !subgridsLoaded) {
            intervalId = setInterval(function () {
                var subgridsArr = Xrm.Page.getControl(function (control, index) {
                    return control.getControlType() == 'subgrid';
                });
                subgridsArr.forEach(function (control, index) {
                    removeButtonsFromSubGrid(control, visible);
                });
            }, 500);
        }
    }
    catch (e) {
        alert("makeReadOnly() Error: " + e.message);
    }
}

function removeButtonsFromSubGrid(subgridControl, visible) {
    if (intervalId) {
        if (visible) {
            $('#' + subgridControl.getName() + '_addImageButton').css('display', 'block');
            $('#' + subgridControl.getName() + '_openAssociatedGridViewImageButton').css('display', 'block');
        }
        else {
            $('#' + subgridControl.getName() + '_addImageButton').css('display', 'none');
            $('#' + subgridControl.getName() + '_openAssociatedGridViewImageButton').css('display', 'none');
        }

        clearInterval(intervalId);
    }
}

function onLoad() {
    debugger;
    showSubGridButtons("GridSalesActualMarketSize", false);
    showSubGridButtons("GridSalesActualNonMarketSize", false);
    showSubGridButtons("GridSalesActualAll", false);
    showSubGridButtons("GridKAContributionAmount", false);
    showSubGridButtons("GridKAContributionPercentage", false);
    showSubGridButtons("GridMarketShareAmount", false);
    showSubGridButtons("GridMarketSharePercentage", false);
}

function btn_ConfirmSalesTargetPss() {
    try {

        if (Xrm.Page.getAttribute("tss_totalpctgmarketsizeyearly").getValue() <= 0 || Xrm.Page.getAttribute("tss_totalamountmarketsizeyearly").getValue() <= 0 || Xrm.Page.getAttribute("tss_totalallsalesyearly").getValue() <= 0) {

            Xrm.Page.getControl("tss_totalpctgmarketsizeyearly").setDisabled(false);
            Xrm.Page.getControl("tss_totalamountmarketsizeyearly").setDisabled(false);
            Xrm.Page.getControl("tss_totalallsalesyearly").setDisabled(false);
            Xrm.Utility.alertDialog('Failed to Confirm Sales Target PSS : Please fill all Target Achievement Fields more than 0');
        } else {
            var workflowId = '7E466BC7-86F7-4013-80DF-FCA3F28BE075';

            var workflowName = 'Confirm Sales Target PSS';
            ExecuteWorkflow(workflowId, workflowName, function () { Xrm.Page.data.refresh(true); });
        }
    }
    catch (e) {
        Xrm.Utility.alertDialog('Confirm Sales Target PSS : ' + e.message);
    }
}
function btn_ApproveSalesTargetPss() {
    try {
        var workflowId = '2D7950A7-C9F3-4201-A18C-047B5A057684';
        var workflowName = 'Approve Sales Target PSS';
        ExecuteWorkflow(workflowId, workflowName, function () { Xrm.Page.data.refresh(true); });
    }
    catch (e) {
        Xrm.Utility.alertDialog('Approve Sales Target PSS : ' + e.message);
    }
}

function btn_ReviseSalesTargetPss() {
    try {
        var workflowId = 'ff537663-14d7-4698-9ef6-8d8976b521c2';
        var workflowName = 'Revise Sales Target PSS';
        ExecuteWorkflow(workflowId, workflowName, function () { Xrm.Page.data.refresh(true); });
        if (Xrm.Page.getAttribute("tss_status").getValue() == 865920001) {
            Xrm.Page.getControl("tss_totalpctgmarketsizeyearly").setDisabled(false);
            Xrm.Page.getControl("tss_totalamountmarketsizeyearly").setDisabled(false);
            Xrm.Page.getControl("tss_totalallsalesyearly").setDisabled(false);
        }
    }
    catch (e) {
        Xrm.Utility.alertDialog('Revise Sales Target PSS : ' + e.message);
    }
}

function disableConfirmButton() {

    var pssId = Xrm.Page.getAttribute("tss_pss").getValue()[0].id.replace('{', '').replace('}', '');
    var userId = Xrm.Page.context.getUserId().replace('{', '').replace('}', '');
    var pssStatus = Xrm.Page.getAttribute("tss_status").getValue();
    console.log('PSS ID : ', pssId);
    console.log('User Login: ', userId);
    console.log('Status : ', pssStatus);
    if (pssStatus == 865920000 /*Status Draft*/ && userId == pssId) {
        return true;
    }
    else {
        return false;
    }
    Xrm.Page.ui.refreshRibbon();
}

function isPDHExist() {
    var flag = false;
    var userId = Xrm.Page.context.getUserId();
    userId = userId.replace('{', '').replace('}', '');

    //var req = new XMLHttpRequest();
    //req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v8.0/tss_approvallistmarketsizes?$select=_tss_approver_value&$filter=_tss_approver_value eq " + userId + ' and tss_type eq 865920003', false);
    //req.setRequestHeader("OData-MaxVersion", "4.0");
    //req.setRequestHeader("OData-Version", "4.0");
    //req.setRequestHeader("Accept", "application/json");
    //req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    //req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
    //req.onreadystatechange = function () {
    //    if (this.readyState === 4) {
    //        req.onreadystatechange = null;
    //        if (this.status === 200) {
    //            var result = JSON.parse(this.response);
    //            if (result.value.length > 0) {
    //                flag = true;
    //            }
    //        } else {
    //            Xrm.Utility.alertDialog(this.statusText);
    //        }
    //    }
    //};
    //req.send();

    XrmServiceToolkit.Rest.RetrieveMultiple("tss_approvallistmarketsizeSet", "?$select=tss_approvallistmarketsizeId&$filter=tss_Approver/Id eq (guid'" + userId + "') and tss_Type/Value eq 865920003", function (results) {
        if (results.length > 0) {
            flag = true;
        }
    }, function (error) {
        console.log('Error : ', error.message);
    }, function () { }, false);

    console.log("Flag: ", flag);
    return flag;
    Xrm.Page.ui.refreshRibbon();
}
function ExecuteWorkflow(workflowId, workflowName, successCallback, failedCallback) {
    var _return = window.confirm('Do you want to ' + workflowName + ' ?');
    if (_return) {
        //var url = Xrm.Page.context.getServerUrl();
        var url = document.location.protocol + "//" + document.location.host + "/" + Xrm.Page.context.getOrgUniqueName();
        var entityId = Xrm.Page.data.entity.getId();
        var OrgServicePath = "/XRMServices/2011/Organization.svc/web";
        url = url + OrgServicePath;
        var request;
        request = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
			"<s:Body>" +
			"<Execute xmlns=\"http://schemas.microsoft.com/xrm/2011/Contracts/Services\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">" +
			"<request i:type=\"b:ExecuteWorkflowRequest\" xmlns:a=\"http://schemas.microsoft.com/xrm/2011/Contracts\" xmlns:b=\"http://schemas.microsoft.com/crm/2011/Contracts\">" +
			"<a:Parameters xmlns:c=\"http://schemas.datacontract.org/2004/07/System.Collections.Generic\">" +
			"<a:KeyValuePairOfstringanyType>" +
			"<c:key>EntityId</c:key>" +
			"<c:value i:type=\"d:guid\" xmlns:d=\"http://schemas.microsoft.com/2003/10/Serialization/\">" + entityId + "</c:value>" +
			"</a:KeyValuePairOfstringanyType>" +
			"<a:KeyValuePairOfstringanyType>" +
			"<c:key>WorkflowId</c:key>" +
			"<c:value i:type=\"d:guid\" xmlns:d=\"http://schemas.microsoft.com/2003/10/Serialization/\">" + workflowId + "</c:value>" +
			"</a:KeyValuePairOfstringanyType>" +
			"</a:Parameters>" +
			"<a:RequestId i:nil=\"true\" />" +
			"<a:RequestName>ExecuteWorkflow</a:RequestName>" +
			"</request>" +
			"</Execute>" +
			"</s:Body>" +
			"</s:Envelope>";
        var req = new XMLHttpRequest();
        req.open("POST", url, true)
        // Responses will return XML. It isn't possible to return JSON.
        req.setRequestHeader("Accept", "application/xml, text/xml, */*");
        req.setRequestHeader("Content-Type", "text/xml; charset=utf-8");
        req.setRequestHeader("SOAPAction", "http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Execute");
        req.onreadystatechange = function () {
            AssignResponse(req, workflowName, successCallback, failedCallback);
        };
        req.send(request);
    }
}

function AssignResponse(req, workflowName, successCallback, failedCallback) {
    if (req.readyState == 4) {
        if (req.status == 200) {
            Xrm.Utility.alertDialog('Successfully ' + workflowName + '.');
            if (successCallback !== undefined && typeof successCallback === "function") {
                successCallback();
            }
            location.reload();
        }
        else {
            var faultstring = req.responseXML.getElementsByTagName("faultstring")[0].textContent;
            Xrm.Utility.alertDialog('Fail to ' + workflowName + '.\r\n Response Code : (' + req.status + ')' + "\r\nPlease contact your IT support.\r\nTechnical Details :" + faultstring);
            if (failedCallback !== undefined && typeof failedCallback === "function") {
                failedCallback(new Error(faultstring));
            }
        }
    }
}

function TriggerToSave() {
    debugger;
    Xrm.Page.data.save();
    var totalamountmarketsizeyearly = Xrm.Page.getAttribute("tss_totalamountmarketsizeyearly").getValue();
    var totalallsalesyearly = Xrm.Page.getAttribute("tss_totalallsalesyearly").getValue();
    var totalpctgmarketsizeyearly = Xrm.Page.getAttribute("tss_totalpctgmarketsizeyearly").getValue();
    if (totalamountmarketsizeyearly > 0 && totalallsalesyearly > 0 && totalpctgmarketsizeyearly > 0) {
        console.log('autosave: ', 'Ok');

        var workflowId = '7E466BC7-86F7-4013-80DF-FCA3F28BE075';
        var workflowName = 'Confirm Sales Target PSS';
        ExecuteWorkflow(workflowId, workflowName, function () { Xrm.Page.data.refresh(true); });

    }
    Xrm.Page.ui.refreshRibbon();
}

/* Disable Fields */
function disableFields() {
    //if (Xrm.Page.getAttribute("tss_status").getValue() == 865920001)
    //{
    //    Xrm.Page.getControl("tss_totalamountmarketsizeye").setDisabled(true);
    //    Xrm.Page.getControl("tss_totalpctgmarketsizeyearly").setDisabled(true);
    //    Xrm.Page.getControl("tss_totalyearly").setDisabled(true);
    //}

    if ((Xrm.Page.getAttribute("tss_status").getValue() >= 865920001 && Xrm.Page.getAttribute("tss_status").getValue() <= 865920006) && (Xrm.Page.getAttribute("tss_statusreason").getValue() == 865920001 || Xrm.Page.getAttribute("tss_statusreason").getValue() == 865920006)) {
        Xrm.Page.ui.controls.forEach(function (control, i) {
            if (control && control.getDisabled && !control.getDisabled()) {
                control.setDisabled(true);
            }
        });
    }
    Xrm.Page.ui.refreshRibbon();
}

function process_CalculateTargetAchievementAllSalesYearly() {
    var _totalallsalesyearly = GetAttributeValue(Xrm.Page.getAttribute("tss_totalallsalesyearly"));
    var _totalyearly = GetAttributeValue(Xrm.Page.getAttribute("tss_totalyearly"));

    if (_totalallsalesyearly != null && _totalyearly != null) {
        if (_totalallsalesyearly < _totalyearly) {
            alert("Target Achievement All Sales Yearly cannot more than Total Yearly !");

            Xrm.Page.getAttribute("tss_totalallsalesyearly").setValue(null);
            Xrm.Page.getAttribute("tss_totalamountmarketsizeyearly").setValue(null);
            Xrm.Page.getAttribute("tss_totalpctgmarketsizeyearly").setValue(null);

            Xrm.Page.getAttribute("tss_totalallsalesyearly").setSubmitMode("always");
            Xrm.Page.getAttribute("tss_totalamountmarketsizeyearly").setSubmitMode("always");
            Xrm.Page.getAttribute("tss_totalpctgmarketsizeyearly").setSubmitMode("always");
        }
        else
        {
            process_CalculateTargetAchievementMSYearlyPercentage();
        }
    }
    else {
        Xrm.Page.getAttribute("tss_totalamountmarketsizeyearly").setValue(null);
        Xrm.Page.getAttribute("tss_totalpctgmarketsizeyearly").setValue(null);

        Xrm.Page.getAttribute("tss_totalamountmarketsizeyearly").setSubmitMode("always");
        Xrm.Page.getAttribute("tss_totalpctgmarketsizeyearly").setSubmitMode("always");
    }
}

function process_CalculateTargetAchievementMSYearlyAmount() {
    var _totalallsalesyearly = GetAttributeValue(Xrm.Page.getAttribute("tss_totalallsalesyearly"));
    var _totalyearly = GetAttributeValue(Xrm.Page.getAttribute("tss_totalyearly"));
    var _totalamountmarketsizeyearly = GetAttributeValue(Xrm.Page.getAttribute("tss_totalamountmarketsizeyearly"));

    if (_totalallsalesyearly != null && _totalyearly != null && _totalamountmarketsizeyearly != null) {
        if (_totalallsalesyearly > 0 && _totalyearly > 0 && _totalamountmarketsizeyearly > 0) {
            var _calculatetotalyearly = _totalallsalesyearly - _totalyearly;
            var _totalpctgmarketsizeyearly = _calculatetotalyearly / _totalamountmarketsizeyearly * 100;

            Xrm.Page.getAttribute("tss_totalpctgmarketsizeyearly").setValue(_totalpctgmarketsizeyearly);
            Xrm.Page.getAttribute("tss_totalpctgmarketsizeyearly").setSubmitMode("always");
        }
        else {
            Xrm.Page.getAttribute("tss_totalamountmarketsizeyearly").setValue(null);
            Xrm.Page.getAttribute("tss_totalpctgmarketsizeyearly").setValue(null);

            Xrm.Page.getAttribute("tss_totalamountmarketsizeyearly").setSubmitMode("always");
            Xrm.Page.getAttribute("tss_totalpctgmarketsizeyearly").setSubmitMode("always");
        }
    }
    else {
        Xrm.Page.getAttribute("tss_totalpctgmarketsizeyearly").setValue(null);
        Xrm.Page.getAttribute("tss_totalpctgmarketsizeyearly").setSubmitMode("always");
    }
}

function process_CalculateTargetAchievementMSYearlyPercentage() {
    var _totalallsalesyearly = GetAttributeValue(Xrm.Page.getAttribute("tss_totalallsalesyearly"));
    var _totalyearly = GetAttributeValue(Xrm.Page.getAttribute("tss_totalyearly"));
    var _totalpctgmarketsizeyearly = GetAttributeValue(Xrm.Page.getAttribute("tss_totalpctgmarketsizeyearly"));

    if (_totalallsalesyearly != null && _totalyearly != null && _totalpctgmarketsizeyearly != null) {
        if (_totalallsalesyearly > 0 && _totalyearly > 0 && _totalpctgmarketsizeyearly > 0) {
            var _calculatetotalyearly = _totalallsalesyearly - _totalyearly;
            var _totalamountmarketsizeyearly = 100 * _calculatetotalyearly / _totalpctgmarketsizeyearly;

            Xrm.Page.getAttribute("tss_totalamountmarketsizeyearly").setValue(_totalamountmarketsizeyearly);
            Xrm.Page.getAttribute("tss_totalamountmarketsizeyearly").setSubmitMode("always");
        }
        else
        {
            Xrm.Page.getAttribute("tss_totalamountmarketsizeyearly").setValue(null);
            Xrm.Page.getAttribute("tss_totalpctgmarketsizeyearly").setValue(null);

            Xrm.Page.getAttribute("tss_totalamountmarketsizeyearly").setSubmitMode("always");
            Xrm.Page.getAttribute("tss_totalpctgmarketsizeyearly").setSubmitMode("always");
        }
    }
    else {
        Xrm.Page.getAttribute("tss_totalamountmarketsizeyearly").setValue(null);
        Xrm.Page.getAttribute("tss_totalamountmarketsizeyearly").setSubmitMode("always");
    }
}