function ExecuteWorkflow(workflowId, workflowName) {
    var _return = window.confirm('Are you want to ' + workflowName + ' ?');
    if (_return) {
        //var url = Xrm.Page.context.getServerUrl();
        var url = "http://" + window.location.host + "/" + Xrm.Page.context.getOrgUniqueName();
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
        req.onreadystatechange = function () { AssignResponse(req, workflowName); };
        req.send(request);
    }
}

function AssignResponse(req, workflowName) {
    if (req.readyState == 4) {
        if (req.status == 200) {
            alert('Successfully executed ' + workflowName + '.');
        }
        else {
            alert('Fail executed ' + workflowName + '.');
        }
    }
}
//4F182761-A896-4447-9771-1CE60A94996D

function checkDirty() {
    var mod = Xrm.Page.data.entity.getIsDirty();
    if (mod) {
        alert("You have to save the form first!");
        return true;
    }
    else {
        return false;
    }
}

function ribbon_SubmitSAP_OnClick() {
    //var workflowId = "4F182761-A896-4447-9771-1CE60A94996D";
    //var workflowName = "Send Email Product Specialist";
    //ExecuteWorkflow(workflowId, workflowName);
    if (checkDirty == false) {
        alert('OI OKE NI');
    }
}
