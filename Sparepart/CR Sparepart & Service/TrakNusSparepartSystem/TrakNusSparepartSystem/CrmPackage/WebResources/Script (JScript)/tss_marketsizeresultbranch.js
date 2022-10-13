//Execute Workfow

function btn_ApproveMarketSizeResultBranch() {
    try {
        var workflowId = '9CBBEE8F-0EC3-4B05-9C64-F9D7CD0FE7BE';
        var workflowName = 'Approve Market Size Result Branch';
        ExecuteWorkflow(workflowId, workflowName, function () { Xrm.Page.data.refresh(true); });
    }
    catch (e) {
        Xrm.Utility.alertDialog('Fail to Approve Market Size Result Branch : ' + e.message);
    }
}

function btn_ReviseMarketSizeResultBranch() {
    try {
        var workflowId = '123EA1F4-BDF4-4533-ACF7-DC5B89D46CC6';
        var workflowName = 'Revise Market Size Result Branch';
        ExecuteWorkflow(workflowId, workflowName, function () { Xrm.Page.data.refresh(true); });
    }
    catch (e) {
        Xrm.Utility.alertDialog('Fail to Revise Market Size Result Branch : ' + e.message);
    }
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

//Validation
function isKorwilExist() {
    //var flag = false;
    //var userId = Xrm.Page.context.getUserId();
    //userId = userId.replace('{', '').replace('}', '');
    //var branchID = Xrm.Page.data.entity.getId();
    //branchID = branchID.replace('{', '').replace('}', '');
    //debugger;
    //XrmServiceToolkit.Rest.RetrieveMultiple("tss_approvallistmarketsizeSet", "?$filter=tss_Approver/Id eq (guid'" + userId + "') and tss_MarketSizeResultBranch/Id eq (guid'" + branchID + "') and tss_Type/Value eq 865920000", function (results) {
    //    if (results.length > 0 && Xrm.Page.getAttribute("tss_status").getValue() == 865920001) {
    //        flag = true;
    //    }
    //}, function (error) {
    //    Xrm.Utility.alertDialog(error.message);
    //}, function () {
    //    //On Complete - Do Something
    //}, true);
    //return flag;

    debugger;

    var flag = false;
    var marketsizeresultbranchid = Xrm.Page.data.entity.getId();
    var userId = Xrm.Page.context.getUserId();
    var branch;
    userId = userId.replace('{', '').replace('}', '');

    //XrmServiceToolkit.Rest.Retrieve(userId, "SystemUserSet", "BusinessUnitId", null, function (result) {
    //    branch = result.BusinessUnitId.Id;
    //}, function (error) {
    //    Xrm.Utility.alertDialog(error.message);
    //}, false);

    XrmServiceToolkit.Rest.Retrieve(marketsizeresultbranchid, "tss_MarketSizeResultBranchSet", "tss_Branch", null, function (result) {
        branch = result.tss_Branch.Id;
    }, function (error) {
        Xrm.Utility.alertDialog(error.message);
    }, false);
    
    //var req = new XMLHttpRequest();
    //req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v8.0/tss_approvallistmarketsizes?$select=_tss_approver_value,tss_type&$filter=_tss_approver_value eq " + userId + ' and tss_type eq 865920001', false);
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

    console.log('User ID : ', userId);
    console.log('Branch : ', branch);

    XrmServiceToolkit.Rest.RetrieveMultiple("tss_approvallistmarketsizeSet", "?$select=tss_approvallistmarketsizeId&$filter=tss_Approver/Id eq (guid'" + userId + "') and tss_Type/Value eq 865920001", function (results) {
        if (results.length > 0) {
            flag = true;
        }

        console.log('Flag 1 : ', flag);
    }, function (error) {
        console.log('Error : ', error.message);
    }, function () { }, false);

    //2018.09.25 | Jika masih ada Market Size Result dengan status Waiting Approval PDH, tidak bisa approve Korwil.
    if (branch != null && branch != undefined) {
        XrmServiceToolkit.Rest.RetrieveMultiple("tss_marketsizeresultpssSet", "?$select=tss_MarketSizeResultId&$filter=tss_Status/Value eq 865920001 and tss_Branch/Id eq (guid'" + branch + "')", function (results) {
            if (results.length > 0) {
                flag = false;
            }

            console.log('Flag 2 : ', flag);
        }, function (error) {
            console.log('Error : ', error.message);
        }, function () { }, false);
    }
    //if (branch != null && branch != undefined) {
    //    XrmServiceToolkit.Rest.RetrieveMultiple("tss_marketsizeresultpssSet", "?$select=tss_MarketSizeResultId&$filter=tss_Status/Value eq 865920001 and tss_Branch/Id eq (guid'" + branch + "')", function (results) {
    //        if (results.length > 0) {
    //            flag = false;
    //        }

    //        console.log('Flag 2 : ', flag);
    //    }, function (error) {
    //        console.log('Error : ', error.message);
    //    }, function () { }, false);
    //}
    
    console.log('Flag: ',flag);
    console.log('Status: ',Xrm.Page.getAttribute("tss_status").getValue());
    return flag;
}

/* Disable Fields */
function disableFields() {
    //if (Xrm.Page.getAttribute("tss_status").getValue() == 865920001)
    //{
    //    Xrm.Page.getControl("tss_totalamountmarketsizeye").setDisabled(true);
    //    Xrm.Page.getControl("tss_totalpctgmarketsizeyearly").setDisabled(true);
    //    Xrm.Page.getControl("tss_totalyearly").setDisabled(true);
    //}

    if ((Xrm.Page.getAttribute("tss_status").getValue() >= 865920001 && Xrm.Page.getAttribute("tss_status").getValue() <= 865920005) && (Xrm.Page.getAttribute("tss_statusreason").getValue() == 865920001 || Xrm.Page.getAttribute("tss_statusreason").getValue() == 865920003)) {
        Xrm.Page.ui.controls.forEach(function (control, i) {
            if (control && control.getDisabled && !control.getDisabled()) {
                control.setDisabled(true);
            }
        });
    }
}
