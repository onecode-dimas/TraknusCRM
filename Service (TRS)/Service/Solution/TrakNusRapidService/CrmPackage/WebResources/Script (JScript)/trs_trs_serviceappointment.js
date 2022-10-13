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

function trs_cmdHold_OnClick() {
    var isdirty = checkDirty();
    if (isdirty == false) {
        var workflowId = '6CB329F9-4DAA-4E4D-A944-C45522FEB9E4';
        var workflowName = "Hold WO";
        ExecuteWorkflow(workflowId, workflowName);
    }
}

function trs_cmdUnhold_OnClick() {
    var isdirty = checkDirty();
    if (isdirty == false) {
        var workflowId = '8764E271-78BC-410B-A85C-8AACBCFC980B';
        var workflowName = "Unhold WO";
        ExecuteWorkflow(workflowId, workflowName);
    }
}

function trs_cmdDispatch_OnClick() {
    var isdirty = checkDirty();
    if (isdirty == false) {
        var workflowId = '3A9C28EC-106E-44F2-8A28-3A7B8A7C3DD3';
        var workflowName = "Dispatch WO";
        ExecuteWorkflow(workflowId, workflowName);
    }
}

function WO_PartsSummary() {
    var isdirty = checkDirty();
    if (isdirty == false) {
        var workflowId = 'F4ECC0F7-A6E3-4555-B2B4-A0FEAA7636E5';
        var workflowName = "Summarize Parts WO";
        ExecuteWorkflow(workflowId, workflowName);
    }
}

function WO_Release() {
    var isdirty = checkDirty();
    if (isdirty == false) {
        var workflowId = '3BE8F213-1CC1-42E1-9134-B7F603D61817';
        var workflowName = 'Release WO';
        ExecuteWorkflow(workflowId, workflowName);
        //var workflowId = '81BDD1E6-DB1E-4315-BD17-CAC66E0ED52A';
        //var workflowName = 'Get Parts and Tools';
        //ExecuteWorkflow(workflowId, workflowName);
    }
}

function WO_GetPartsTools() {
    var isdirty = checkDirty();
    if (isdirty == false) {
        var workflowId = '81BDD1E6-DB1E-4315-BD17-CAC66E0ED52A';
        var workflowName = 'Get Parts and Tools';
        ExecuteWorkflow(workflowId, workflowName);
    }
}

function WO_PartReturn() {
    var isdirty = checkDirty();
    if (isdirty == false) {
        var workflowId = '314a5e16-407f-47eb-98dc-025f486854e1';
        var workflowName = 'Return Part';
        ExecuteWorkflow(workflowId, workflowName);
    }
}

function WO_RequestTools() {
    var isdirty = checkDirty();
    if (isdirty == false) {
        var workflowId = '06ffd51b-f2fa-4db7-9027-047115dcf17b';
        var workflowName = 'Request Tools';
        ExecuteWorkflow(workflowId, workflowName);
    }
}

function WO_CalculateRTG() {
    var isdirty = checkDirty();
    if (isdirty == false) {
        var workflowId = 'DC9ABAFE-C85C-4F12-AF51-BD39ECB18624';
        var workflowName = 'Calculate RTG';
        ExecuteWorkflow(workflowId, workflowName);
    }
}

function UpdateWOtoSAP() {
    var isdirty = checkDirty();
    if (isdirty == false) {
        var workflowId = 'A73DD083-81AF-453F-B032-0B3B6A2696CD';
        var workflowName = 'WO Update to SAP';
        ExecuteWorkflow(workflowId, workflowName);
    }
}

function trs_CopyQuotation_OnClick() {
    var isdirty = checkDirty();
    if (isdirty == false) {
        var workflowId = '4E961079-4B9C-4A26-B099-E9F65F10D7E6';
        var workflowName = 'WO Copy data Quotation Workorder';
        ExecuteWorkflow(workflowId, workflowName);
    }
}

function WO_TECO() {
    try {
        var isdirty = checkDirty();
        if (isdirty == false) {
            var type = 0;
            var pmacttypelookup = Xrm.Page.getAttribute("trs_pmacttype").getValue();
            if (pmacttypelookup != null) {
                var pmacttype = pmacttypelookup[0].name;
                if (pmacttype == 'INSPECTION' || pmacttype == 'PPM')
                    type = 1;
                else if (pmacttype == 'WARRANTY REPAIR' || pmacttype == 'REPAIR')
                    type = 2;

                var status = IsAllowtoSubmitTeco(type);

                if (status) {
                    var workflowId = 'E8614282-8D1B-4BC6-A7BA-5BAFB4D9A860';
                    var workflowName = 'Submit TECO';
                    ExecuteWorkflow(workflowId, workflowName);

                    if (type == 2) {
                        var workflowId = 'AF239336-2056-4682-B0DE-D5312D5A990C';
                        var workflowName = 'TSR_SendEmailNotification';
                        ExecuteWorkflow(workflowId, workflowName);
                    }
                }
                else {
                    switch(type) {
                        case 1:
                            alert('Please insert PPM Report');
                            break;
                        case 2:
                            alert('Please insert TSR');
                            break;
                        default:
                            alert('Failed to Submit TECO');
                    }
                }
            }
        }
    }
    catch (e) {
        alert("WO_TECO : " + e.message);
    }
}

function IsAllowtoSubmitTeco(type) {   // If Type equals warranty repair, must have tsr
    try {
        var status = false;
        var recordId = Xrm.Page.data.entity.getId().replace('{', '').replace('}', '');

        if (type == 1) {
            var path = "/trs_ppmreportSet?$select=trs_ppmreportId&$filter=trs_WorkOrder/Id eq guid'" + recordId + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved.results != null && retrieved.results.length > 0) {
                    status = true;
                }
            }
        }
        else if (type == 2) {
            var path = "/trs_technicalservicereportSet?$select=trs_TSRNumber&$filter=trs_WorkOrder/Id eq guid'" + recordId + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved.results.length > 0) {
                    status = true;
                }
            }
        }
        else {
            status = true;
        }

        return status;
    }
    catch (e) {
        throw ("IsAllowtoSubmitTeco : " + e.message);
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

function RibbonRefreshDisplay() {
    Xrm.Page.ui.refreshRibbon();
}

function setWorkshop() {
    var work = Xrm.Page.getAttribute("trs_workshop").getValue();
    if (work == 1) {
        //    Xrm.Page.getAttribute("trs_contactperson").setRequiredLevel("recommended");
        //    Xrm.Page.getAttribute("trs_cpphone").setRequiredLevel("recommended");
        //    Xrm.Page.getAttribute("trs_cponsite").setRequiredLevel("recommended");
        //    Xrm.Page.getAttribute("trs_phoneonsite").setRequiredLevel("recommended");
        return false;
    }
    else {
        //    Xrm.Page.getAttribute("trs_contactperson").setRequiredLevel("required");
        //    Xrm.Page.getAttribute("trs_cpphone").setRequiredLevel("required");
        //    Xrm.Page.getAttribute("trs_cponsite").setRequiredLevel("required");
        //    Xrm.Page.getAttribute("trs_phoneonsite").setRequiredLevel("required");
        return true;
    }
    RibbonRefreshDisplay();
}