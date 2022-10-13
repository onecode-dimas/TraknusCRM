
function btn_ConvertToProspectPart() {
    try {
        var workflowId = '6b25a71e-af85-4c7d-b42c-83c075c36753';
        var workflowName = 'Convert to Prospect Part';
        ExecuteWorkflow(workflowId, workflowName, function () { OpenNewProspectPart(); });
    }
    catch (e) {
        Xrm.Utility.alertDialog('Convert to Prospect Part : ' + e.message);
    }
}

function OpenNewProspectPart() {
    try {


        var windowOptions = {
            openInNewWindow: true
        };

        var potentialProspectPartLinesId = Xrm.Page.data.entity.getId().replace('{', '').replace('}', '');
        var customerId = null;
        var pssId = null;
        var potentialProspectPartId = null;
        var prospectpartheaderId;

        XrmServiceToolkit.Rest.Retrieve(potentialProspectPartLinesId, "tss_PotentialProspectPartLinesSet", "tss_PotentialProspectPart", null, function (result) {
            potentialProspectPartId = result.tss_PotentialProspectPart.Id;
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, false);

        XrmServiceToolkit.Rest.Retrieve(potentialProspectPartId, "tss_PotentialProspectPartSet", "tss_Customer,tss_PSS", null, function (result) {
            customerId = result.tss_Customer.Id;
            pssId = result.tss_PSS.Id;
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, false);

        XrmServiceToolkit.Rest.RetrieveMultiple("tss_prospectpartheaderSet",
                                                "$select=tss_prospectpartheaderId&$top=1&$orderby=CreatedOn desc&$filter=tss_PSS/Id eq (guid'" + pssId + "') and tss_Customer/Id eq (guid'" + customerId + "')",
                                                function (result) {
                                                    prospectpartheaderId = result[0].tss_prospectpartheaderId;
                                                }, function (error) {
                                                    alert(error.message);
                                                }, function onComplete() {
                                                    //alert("DONE.");
                                                }, false
                                               );
        if (prospectpartheaderId)
            Xrm.Utility.openEntityForm("tss_prospectpartheader", prospectpartheaderId, null, windowOptions);

    } catch (e) {
        alert('Failed try to open Prospect Part Header because ' + e.message);
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
            alert('Successfully executed ' + workflowName + '.');
            if (successCallback !== undefined && typeof successCallback === "function") {
                successCallback();
            }
        }
        else {
            var faultstring = req.responseXML.getElementsByTagName("faultstring")[0].textContent;
            alert('Fail to execute ' + workflowName + '.\r\n Response Code : (' + req.status + ')' + "\r\nPlease contact your IT support.\r\nTechnical Details :" + faultstring);
            if (failedCallback !== undefined && typeof failedCallback === "function") {
                failedCallback(new Error(faultstring));
            }
        }
    }
}