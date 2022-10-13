function isSMExist(){
    var flag = false;
    var userId = Xrm.Page.context.getUserId();
    userId = userId.replace('{', '').replace('}', '');

    var req = new XMLHttpRequest();
    req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v8.0/tss_approvallistmarketsizes?$select=_tss_approver_value&$filter=_tss_approver_value eq " + userId + ' and tss_type eq 865920005', false);
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
    req.onreadystatechange = function() {
        if (this.readyState === 4) {
            req.onreadystatechange = null;
            if (this.status === 200) {
                var result = JSON.parse(this.response);
                if(result.value.length > 0){
                    flag = true;
                }
            } else {
                Xrm.Utility.alertDialog(this.statusText);
            }
        }
    };

    req.send();

    //2018.09.25 | Jika masih ada Sales Target Branch dengan status Waiting Approval Korwil, tidak bisa approve National.
    XrmServiceToolkit.Rest.RetrieveMultiple("tss_salestargetbranchSet", "?$select=tss_salestargetbranchId&$filter=tss_Status/Value eq 865920001", function (results) {
        if (results.length > 0) {
            flag = false;
        }
    }, function (error) {
        Xrm.Utility.alertDialog(error.message);
    }, function () {
        //On Complete - Do Something
    }, false);

    console.log('Flag: ', flag);

    return flag;
}
function btn_ApproveSalesTargetNational() {
    try {
        var workflowId = 'E4453A3D-EC28-41E5-9F3C-5041A6B62BF2';
        var workflowName = 'Approve Sales Target National';
        ExecuteWorkflow(workflowId, workflowName, function () { Xrm.Page.data.refresh(true); });
    }
    catch (e) {
        Xrm.Utility.alertDialog('Fail to Approve Sales Target National : ' + e.message);
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

//Revise --------------------------------------------------------------------------------

function btn_ReviseSalesTargetNational() {
    try {
        var workflowId = 'AEF898BF-1D15-472A-A47E-B51B20526F14';
        var workflowName = 'Revise Sales Target National';
        ExecuteWorkflow(workflowId, workflowName, function () { Xrm.Page.data.refresh(true); });
    }
    catch (e) {
        Xrm.Utility.alertDialog('Fail to Revise Sales Target National : ' + e.message);
    }
}

//---------------------------------------------------------------------------------------

/* Disable Fields */
function disableFields() {
    //if (Xrm.Page.getAttribute("tss_status").getValue() == 865920001)
    //{
    //    Xrm.Page.getControl("tss_totalamountmarketsizeye").setDisabled(true);
    //    Xrm.Page.getControl("tss_totalpctgmarketsizeyearly").setDisabled(true);
    //    Xrm.Page.getControl("tss_totalyearly").setDisabled(true);
    //}

    if ((Xrm.Page.getAttribute("tss_status").getValue() >= 865920001 && Xrm.Page.getAttribute("tss_status").getValue() <= 865920003) && (Xrm.Page.getAttribute("tss_statusreason").getValue() == 865920001 || Xrm.Page.getAttribute("tss_statusreason").getValue() == 865920002)) {
        Xrm.Page.ui.controls.forEach(function (control, i) {
            if (control && control.getDisabled && !control.getDisabled()) {
                control.setDisabled(true);
            }
        });
    }
    Xrm.Page.ui.refreshRibbon();
}
