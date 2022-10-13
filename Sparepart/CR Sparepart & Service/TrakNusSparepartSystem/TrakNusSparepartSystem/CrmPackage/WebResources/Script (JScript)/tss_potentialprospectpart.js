function GetAttributeValue(attribute) {
    var result = null;

    if (attribute != null) {
        if (attribute.getValue() != null) {
            result = attribute.getValue();
        }
    }

    return result;
}

function disableConfirmButton() {

    var pssId = Xrm.Page.getAttribute("tss_pss").getValue()[0].id.replace('{', '').replace('}', '');
    var userId = Xrm.Page.context.getUserId().replace('{', '').replace('}', '');

    console.log('PSS ID : ', pssId);
    console.log('User Login: ', userId);
    //console.log('Status : ', pssStatus);
    if (userId == pssId) {
        return true;
    }
    else {
        return false;
    }
}

function btn_GenerateProspectPart() {
    try {
        var actionName = 'Convert Prospect Part Header';
        var workflowid = '3AB90824-6E3F-41B2-8D1E-B48F1A15C325';
        ExecuteAction(workflowid, actionName, function () { OpenNewProspectPart(); });
    } catch (e) {
        Xrm.Utility.alertDialog('Fail to Generate Prospect Part Header : ' + e.message);
    }
}

function ExecuteAction(workflowId, workflowName, successCallback, failedCallback) {
    var _return = window.confirm('Do you want to Generate Prospect Part Header?');
    if (_return) {
        var result = null;
        

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
        //req.send(JSON.stringify(parameters));
    }
}
function AssignResponse(req, workflowName, successCallback, failedCallback) {
    if (req.readyState == 4) {
        if (req.status == 200) {
            if (workflowName != 'Create Quotation') {
                alert('Successfully executed ' + workflowName + '.');
            }
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
function OpenNewProspectPart() {
    try {
        var prospectpartheaderId;
        var customerId = Xrm.Page.getAttribute("tss_customer").getValue()[0].id.replace('{', '').replace('}', '');
        var pssId = Xrm.Page.getAttribute("tss_pss").getValue()[0].id.replace('{', '').replace('}', '');

        var windowOptions = {
            openInNewWindow: true
        };
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

function ShowHideTabs() {
    debugger;
    console.log("testing");
    var groupuiocommodity = GetAttributeValue(Xrm.Page.getAttribute("tss_groupuiocommodityheader"));
    //var groupuiocommodity = Xrm.Page.getAttribute("tss_groupuiocommodity").getValue();

    //tab_5 == AGING
    //tab_7 == PM PERIOD
    
    if (groupuiocommodity != null) {
        if (groupuiocommodity) {
            Xrm.Page.getControl("tss_serialnumber").setVisible(false);
            Xrm.Page.getControl("tss_serialnumberpopulation").setVisible(false);
            Xrm.Page.getControl("tss_groupuiocommodityheader").setVisible(true);

            Xrm.Page.ui.tabs.get("tab_5").setVisible(false);
            Xrm.Page.ui.tabs.get("tab_7").setVisible(false);
            Xrm.Page.ui.tabs.get("tab_8").setVisible(true);
        } else {
            Xrm.Page.getControl("tss_serialnumber").setVisible(true);
            Xrm.Page.getControl("tss_serialnumberpopulation").setVisible(true);
            Xrm.Page.getControl("tss_groupuiocommodityheader").setVisible(false);

            Xrm.Page.ui.tabs.get("tab_5").setVisible(true);
            Xrm.Page.ui.tabs.get("tab_7").setVisible(true);
            Xrm.Page.ui.tabs.get("tab_8").setVisible(false);
        }
    }
    else {
        Xrm.Page.getControl("tss_serialnumber").setVisible(true);
        Xrm.Page.getControl("tss_serialnumberpopulation").setVisible(true);
        Xrm.Page.getControl("tss_groupuiocommodityheader").setVisible(false);

        Xrm.Page.ui.tabs.get("tab_5").setVisible(true);
        Xrm.Page.ui.tabs.get("tab_7").setVisible(true);
        Xrm.Page.ui.tabs.get("tab_8").setVisible(false);
    }
}

function FormOnload() {
    ShowHideTabs();
    disableConfirmButton();
    Set_SerialNumberPopulation();

    makeReadOnlyGrid("ProspectPartLinesGrid");
    makeReadOnlyGrid("ProspectPartLinesByAgingGrid");
}

function makeReadOnlyGrid(gridName) {
    try {
        var subgridsLoaded = false;
        if ($("div[id$='" + gridName + "']").length > 0 && !subgridsLoaded) {
            intervalId = setInterval(function () {
                var subgridsArr = Xrm.Page.getControl(function (control, index) {
                    return control.getControlType() == 'subgrid';
                });
                subgridsArr.forEach(function (control, index) {
                    removeButtonsFromSubGrid(control);
                });
            }, 500);
        }
    }
    catch (e) {
        alert("makeReadOnlyGrid() Error: " + e.message);
    }
}

function removeButtonsFromSubGrid(subgridControl) {
    if (intervalId) {
        $('#' + subgridControl.getName() + '_addImageButton').css('display', 'none');
        $('#' + subgridControl.getName() + '_openAssociatedGridViewImageButton').css('display', 'none');
        //retrieveQuotationPartLines();
        clearInterval(intervalId);
    }
}

function Set_SerialNumberPopulation() {
    debugger;
    var _serialnumber = GetAttributeValue(Xrm.Page.getAttribute("tss_serialnumber"));

    if (_serialnumber != null) {
        XrmServiceToolkit.Rest.Retrieve(_serialnumber[0].id, "new_populationSet", "new_SerialNumber", null, function (result) {
            var new_SerialNumber = result.new_SerialNumber;

            Xrm.Page.getAttribute("tss_serialnumberpopulation").setValue(new_SerialNumber);
            Xrm.Page.getAttribute("tss_serialnumberpopulation").setSubmitMode("always");
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, false);
    }
    else {
        Xrm.Page.getAttribute("tss_serialnumberpopulation").setValue(null);
        Xrm.Page.getAttribute("tss_serialnumberpopulation").setSubmitMode("always");
    }
}