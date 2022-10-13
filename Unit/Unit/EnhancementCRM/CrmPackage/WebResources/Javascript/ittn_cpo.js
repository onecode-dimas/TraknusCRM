var formType = Xrm.Page.ui.getFormType();
var formStatus = {
    Undefined: 0,
    Create: 1,
    Update: 2,
    ReadOnly: 3,
    Disabled: 4,
    QuickCreate: 5,
    BulkEdit: 6,
    ReadOptimized: 11
};

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

function Visible_Ribbon_Submit_TextFile() {
    var flag = false;
    var CPOSAP = Xrm.Page.getAttribute("new_cpoidsap").getValue();

    if (CPOSAP == null) {
        XrmServiceToolkit.Rest.RetrieveMultiple("trs_workflowconfigurationSet", "?$select=ittn_SubmitSOMethod&$filter=trs_GeneralConfig eq 'TRS'", function (results) {
            if (results.length > 0) {
                for (var i = 0; i < results.length; i++) {
                    var ittn_SubmitSOMethod = results[i].ittn_SubmitSOMethod;

                    if (ittn_SubmitSOMethod.Value == 841150000) {
                        flag = true;
                    }
                }
            }
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, function () {
            //On Complete - Do Something
        }, false);
    }
    
    return flag;
}

function Visible_Ribbon_Submit_WebService() {
    var flag = false;
    var CPOSAP = Xrm.Page.getAttribute("new_cpoidsap").getValue();

    if (CPOSAP == null) {
        XrmServiceToolkit.Rest.RetrieveMultiple("trs_workflowconfigurationSet", "?$select=ittn_SubmitSOMethod&$filter=trs_GeneralConfig eq 'TRS'", function (results) {
            if (results.length > 0) {
                for (var i = 0; i < results.length; i++) {
                    var ittn_SubmitSOMethod = results[i].ittn_SubmitSOMethod;

                    if (ittn_SubmitSOMethod.Value == 841150001) {
                        flag = true;
                    }
                }
            }
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, function () {
            //On Complete - Do Something
        }, false);
    }

    return flag;
}

function SubmitSO_WebService() {
    var workflowId = 'E739B3C5-48E2-4971-B11A-74BD25D1F13D';
    var workflowName = 'Submit CPO to SAP via Web Service';
    ExecuteWorkflow(workflowId, workflowName, function () { RefreshForm(); });
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
        req.onreadystatechange = function () { AssignResponse(req, workflowName, successCallback, failedCallback); };
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

function refreshRibbonOnChange() {
    Xrm.Page.ui.refreshRibbon();
}

function OnLoad() {
    refreshRibbonOnChange();
    // Show_PrintPreviewCPO();
}

function RefreshForm() {
    var check = [];
    check.push(function () {
        return formType !== formStatus.Create;
    });

    var boolArrayCheck = function (predicateArray) {
        //Assume predicateArray is right
        var boolCheck = true;
        for (var index = 0; index < predicateArray.length; index++) {
            boolCheck = boolCheck && predicateArray[index]();
        }
        return boolCheck;
    }

    if (boolArrayCheck(check)) {
        Xrm.Utility.openEntityForm(Xrm.Page.data.entity.getEntityName(), Xrm.Page.data.entity.getId());
    }
}

function Show_PrintPreviewCPO() {
    debugger;
    if (Xrm.Page.ui.tabs.get("tab_print_cpo") != null) {
        try {
            var recordID = Xrm.Page.data.entity.getId();
            var path = "/new_incentiveSet?$select=new_incentiveId&$filter=new_CPO/Id eq (guid'" + recordID + "') and ittn_StatusReason/Value eq 841150000";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved != null && retrieved.results.length > 0) {
                    Xrm.Page.ui.tabs.get("tab_print_cpo").setVisible(false);
                }
                else {
                    Xrm.Page.ui.tabs.get("tab_print_cpo").setVisible(true);
                }
            }
        }
        catch (e) {
            throw new Error("SetActType : " + e.Message);
        }
    }
}