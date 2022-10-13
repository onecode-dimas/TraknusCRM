//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// RIBBON AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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

function reviseQuotationClick() {
    var IsDirty = checkDirty();

    if (IsDirty == false) {
        var workflowId = '94e20ade-8f45-4f79-88f2-42e21ac24a70';
        var workflowName = 'WO Revise Quotation';
        ExecuteWorkflow(workflowId, workflowName);
    }
}

function trs_btnApprove_onClick() {
    var IsDirty = checkDirty();

    if (IsDirty == false) {
        var workflowId = '63cb61b6-df24-4fad-b0aa-858c3b481ffc';
        var workflowName = 'WO Approve Quotation';
        ExecuteWorkflow(workflowId, workflowName);
    }
}

function trs_btnFinalApprove_onClick() {
    var IsDirty = checkDirty();

    if (IsDirty == false) {
        var workflowId = '80097c6b-436a-479e-9c62-ad007fe2ca45';
        var workflowName = 'WO Final Approve Quotation';
        ExecuteWorkflow(workflowId, workflowName);
    }
}

function trs_SummarizeParts_onClick() {
    var IsDirty = checkDirty();

    if (IsDirty == false) {
        var workflowId = '15669DD1-24CB-4FD7-ACAD-008EE4051959';
        var workflowName = 'Summarize Parts';
        ExecuteWorkflow(workflowId, workflowName);
    }
}

function ExecuteWorkflow(workflowId, workflowName) {
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
            alert('Fail executed ' + workflowName + '. (' + req.status + ')');
        }
    }
}

function disableButtonQuotation() {
    var statusQuotation = Xrm.Page.getAttribute("statuscode").getValue();
    if (statusQuotation == 167630001) {
        //Revise status
        //A value of true will result in the Enabling of the button, false will Disable it.
        return true
    }
}

function CalculateTotal() {
    var workflowId = 'f44e733d-94b8-47fd-9e6f-7a666f632e35';
    var workflowName = 'Calculate Total';
    ExecuteWorkflow(workflowId, workflowName);
    window.location.reload(true);
}