///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// Helper
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//Get Form Type
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

function SetDefaultValue_TwoOptionField(fieldname) {
    var attr = Xrm.Page.getAttribute(fieldname);
    if (attr != null) {
        if (attr.getValue() == false) {
            attr.setValue(0);
            attr.setValue(1);
            attr.setSubmitMode("always");
        }
    }
}

function ActiveStartDate_OnChange() {
    var msStartDate = Xrm.Page.getAttribute("tss_msperiodstart").getValue();

    if (formType == formStatus.Create) {
        Xrm.Page.getAttribute("tss_activestartdate").setValue(msStartDate);
    }
    else if (formType == formStatus.Update) {

    }
}

function ActiveEndDate_OnChange() {
    var msEndDate = Xrm.Page.getAttribute("tss_msperiodend").getValue();

    if (formType == formStatus.Create) {
        Xrm.Page.getAttribute("tss_activeenddate").setValue(msEndDate);
    }
    else if (formType == formStatus.Update) {

    }
}

function GetAttributeValue(attribute) {
    var result = null;

    if (attribute != null) {
        if (attribute.getValue() != null) {
            result = attribute.getValue();
        }
    }

    return result;
}

function onLoad() {
    if (formType == formStatus.Create) {

        var userId = Xrm.Page.context.getUserId();
        var userName = Xrm.Page.context.getUserName();
        var lookupVal = new Array();
        lookupVal[0] = new Object();
        lookupVal[0].id = userId;
        lookupVal[0].name = userName;
        lookupVal[0].entityType = "systemuser";

        Xrm.Page.getAttribute("tss_pss").setValue(lookupVal);
        //Xrm.Page.getControl("tss_funlock").setDisabled(true);

        Xrm.Page.getAttribute("tss_revision").setValue(0);
        SetDefaultValue_TwoOptionField("tss_calculatetoms");

        loadMSDate();
    }

    LockData();
}

function IsPSS() {
    var flag = false;
    var userId = Xrm.Page.context.getUserId().replace("{", "").replace("}", "");

    //XrmServiceToolkit.Rest.Retrieve(userId, "SystemUserSet", "SystemUserId,Title", null, function (result) {
    //    var systemUserId = result.SystemUserId;
    //    var title = result.Title;

    //    if (title == "PSS") {
    //        flag = true;
    //    }
    //}, function (error) {
    //    Xrm.Utility.alertDialog(error.message);
    //}, false);

    var req = new XMLHttpRequest();
    req.open("GET", Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/SystemUserSet(guid'1CBA90DC-4DE9-E111-9AA2-544249894792')?$select=SystemUserId,Title", false);
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.onreadystatechange = function () {
        if (this.readyState === 4) {
            this.onreadystatechange = null;
            if (this.status === 200) {
                var result = JSON.parse(this.responseText).d;
                var systemUserId = result.SystemUserId;
                var title = result.Title;

                if (title == "PSS") {
                    flag = true;
                }
            } else {
                Xrm.Utility.alertDialog(this.statusText);
            }
        }
    };
    req.send();

    return flag;
}

function LockData() {
    debugger;

    var oStatus = Xrm.Page.getAttribute("tss_status").getValue();
    var oReason = Xrm.Page.getAttribute("tss_reason").getValue();
    //var oRelated = IsRelatedToMarketSize();
    var _revision = GetAttributeValue(Xrm.Page.getAttribute("tss_revision"));

    //if status ACTIVE
    //if ((oStatus == 865920001 || oStatus == 865920002 || oStatus == 865920003) && (oReason == 865920001 || oReason == 865920002) && oRelated) {
    if ((oStatus == 865920001 || oStatus == 865920002 || oStatus == 865920003) && (oReason == 865920001 || oReason == 865920002)) {
        Xrm.Page.getControl("tss_customer").setDisabled(true);
        //Xrm.Page.getControl("tss_funlock").setDisabled(true);
        //Xrm.Page.getControl("tss_calculatetoms").setDisabled(true);
    } else if (oStatus != 865920000) {
        console.log("status: ", oStatus);
        Xrm.Page.getControl("tss_customer").setDisabled(true);
        //Xrm.Page.getControl("tss_funlock").setDisabled(true);
        //Xrm.Page.getControl("tss_calculatetoms").setDisabled(true);
    }
    else {
        var oID = Xrm.Page.data.entity.getId();
        var _kauioqty = 0, _kagroupuiocommodity = 0;

        var req = new XMLHttpRequest();
        req.open("GET", Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/tss_kauioSet?$select=tss_kauioId&$filter=tss_KeyAccountId/Id eq (guid'" + oID + "')", false);
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                this.onreadystatechange = null;
                if (this.status === 200) {
                    var returned = JSON.parse(this.responseText).d;
                    var results = returned.results;

                    _kauioqty = results.length;
                } else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send();

        var req = new XMLHttpRequest();
        req.open("GET", Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/tss_kagroupuiocommoditySet?$select=tss_kagroupuiocommodityId&$filter=tss_KeyAccountId/Id eq (guid'" + oID + "')", false);
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                this.onreadystatechange = null;
                if (this.status === 200) {
                    var returned = JSON.parse(this.responseText).d;
                    var results = returned.results;

                    _kagroupuiocommodity = results.length;
                } else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send();

        if (_kauioqty > 0 || _kagroupuiocommodity > 0) {
            Xrm.Page.getControl("tss_customer").setDisabled(true);
        }
        else
            Xrm.Page.getControl("tss_customer").setDisabled(false);

        //Xrm.Page.getControl("tss_customer").setDisabled(false);
        //Xrm.Page.getControl("tss_funlock").setDisabled(false);
        //Xrm.Page.getControl("tss_calculatetoms").setDisabled(false);
    }

    if (_revision != null) {
        if (_revision == 0) 
            Xrm.Page.getControl("tss_calculatetoms").setDisabled(true);
        else
            Xrm.Page.getControl("tss_calculatetoms").setDisabled(false);
    }
}

function IsRelatedToMarketSize() {
    debugger;

    var oResult = false;
    var oID = Xrm.Page.data.entity.getId();
    var _kauioqty = 0, _kagroupuiocommodity = 0;

    XrmServiceToolkit.Rest.RetrieveMultiple("tss_kauioSet", "?$select=tss_kauioId&$filter=tss_KeyAccountId/Id eq (guid'" + oID + "')", function (results) {
        _kauioqty = results.length;
    }, function (error) {
        Xrm.Utility.alertDialog(error.message);
    }, function () {
        //On Complete - Do Something
    }, true);

    XrmServiceToolkit.Rest.RetrieveMultiple("tss_kagroupuiocommoditySet", "?$select=tss_kagroupuiocommodityId&$filter=tss_KeyAccountId/Id eq (guid'" + oID + "')", function (results) {
        _kagroupuiocommodity = results.length;
    }, function (error) {
        Xrm.Utility.alertDialog(error.message);
    }, function () {
        //On Complete - Do Something
    }, true);

    if (_kauioqty > 0 || _kagroupuiocommodity > 0) {
        oResult = true;
    }

    //if (oID != null && oID != "") {
    //    XrmServiceToolkit.Rest.RetrieveMultiple("tss_marketsizeresultmappingSet", "?$select=tss_MarketSizeResultPSS&$filter=tss_KeyAccount/Id eq (guid'" + oID + "')", function (results) {
    //        for (var i = 0; i < results.length; i++) {
    //            var tss_MarketSizeResultPSS = results[i].tss_MarketSizeResultPSS;
    //            var oStatus;
    //            var oReason;

    //            if (tss_MarketSizeResultPSS.Id != null) {
    //                XrmServiceToolkit.Rest.Retrieve(tss_MarketSizeResultPSS.Id, "tss_marketsizeresultpssSet", "tss_marketsizeresultpssId,tss_Status,tss_StatusReason", null, function (result) {
    //                    oStatus = result.tss_Status;
    //                    oReason = result.tss_StatusReason;
    //                }, function (error) {
    //                    Xrm.Utility.alertDialog(error.message);
    //                }, false);

    //                if ((oStatus == 865920001 || oStatus == 865920002 || oStatus == 865920003 || oStatus == 865920004 || oStatus == 865920005 || oStatus == 865920006 || oStatus == 865920007) && (oReason == 865920001) || oReason == 865920002 || oReason == 865920003) {
    //                    oResult = true;
    //                }
    //            }
    //        }
    //    }, function (error) {
    //        Xrm.Utility.alertDialog(error.message);
    //    }, function () {
    //        //On Complete - Do Something
    //    }, false);
    //}

    return oResult;
}

function loadMSDate() {
    //var req = new XMLHttpRequest();
    //req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v8.0/tss_sparepartsetups()?$select=tss_enddatems,tss_startdatems", false);
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
    //                if (result.value[0]["tss_startdatems"] != null) {
    //                    Xrm.Page.getAttribute("tss_msperiodstart").setValue(new Date(result.value[0]["tss_startdatems"]));
    //                    Xrm.Page.getAttribute("tss_activestartdate").setValue(new Date(result.value[0]["tss_startdatems"]));
    //                }
    //                if (result.value[0]["tss_enddatems"] != null) {
    //                    Xrm.Page.getAttribute("tss_msperiodend").setValue(new Date(result.value[0]["tss_enddatems"]));
    //                    Xrm.Page.getAttribute("tss_activeenddate").setValue(new Date(result.value[0]["tss_enddatems"]));
    //                }
    //            }
    //        } else {
    //            Xrm.Utility.alertDialog(this.statusText);
    //        }
    //    }
    //};
    //req.send();
    debugger;

    XrmServiceToolkit.Rest.RetrieveMultiple("tss_matrixmarketsizeperiodSet", "?$select=tss_DueDateMS,tss_EndDateMarketSize,tss_EvaluationDuration,tss_EvaluationMarketSize,tss_IsActive,tss_matrixmarketsizeperiodId,tss_name,tss_StartDateMarketSize&$filter=tss_IsActive eq true", function (results) {
        if (results.length > 0) {
            var tss_DueDateMS = results[0].tss_DueDateMS;
            var tss_EndDateMarketSize = results[0].tss_EndDateMarketSize;
            var tss_EvaluationDuration = results[0].tss_EvaluationDuration;
            var tss_EvaluationMarketSize = results[0].tss_EvaluationMarketSize;
            var tss_IsActive = results[0].tss_IsActive;
            var tss_matrixmarketsizeperiodId = results[0].tss_matrixmarketsizeperiodId;
            var tss_name = results[0].tss_name;
            var tss_StartDateMarketSize = results[0].tss_StartDateMarketSize;

            Xrm.Page.getAttribute("tss_msperiodstart").setValue(new Date(tss_StartDateMarketSize));
            Xrm.Page.getAttribute("tss_activestartdate").setValue(new Date(tss_StartDateMarketSize));

            Xrm.Page.getAttribute("tss_msperiodend").setValue(new Date(tss_EndDateMarketSize));
            Xrm.Page.getAttribute("tss_activeenddate").setValue(new Date(tss_EndDateMarketSize));

            Xrm.Page.getAttribute("tss_msperiodstart").setSubmitMode("always");
            Xrm.Page.getAttribute("tss_activestartdate").setSubmitMode("always");
            Xrm.Page.getAttribute("tss_msperiodend").setSubmitMode("always");
            Xrm.Page.getAttribute("tss_activeenddate").setSubmitMode("always");
        }
        else {
            Xrm.Page.getAttribute("tss_msperiodstart").setValue(null);
            Xrm.Page.getAttribute("tss_activestartdate").setValue(null);

            Xrm.Page.getAttribute("tss_msperiodend").setValue(null);
            Xrm.Page.getAttribute("tss_activeenddate").setValue(null);

            Xrm.Page.getAttribute("tss_msperiodstart").setSubmitMode("always");
            Xrm.Page.getAttribute("tss_activestartdate").setSubmitMode("always");
            Xrm.Page.getAttribute("tss_msperiodend").setSubmitMode("always");
            Xrm.Page.getAttribute("tss_activeenddate").setSubmitMode("always");
        }
    }, function (error) {
        Xrm.Utility.alertDialog(error.message);
    }, function () {
        //On Complete - Do Something
    }, false);
}
function onChange_Customer() {
    Xrm.Page.getControl("tss_funlock").setDisabled(false);
    Xrm.Page.getAttribute("tss_funlock").setValue(null);
    preFilterLookupFunLoc();
}

function preFilterLookupFunLoc() {
    Xrm.Page.getControl("tss_funlock").addPreSearch(function () {
        addLookupFilterFunLoc(Xrm.Page.getAttribute("tss_customer").getValue()[0].id);
    });
}

function addLookupFilterFunLoc(cust) {
    var fetchFilters = "<filter>" +
    "<condition attribute='trs_customer' uitype='account' operator='eq' value='" + cust + "'/></filter>";
    Xrm.Page.getControl("tss_funlock").addCustomFilter(fetchFilters);
}


//revise Key Account
function btn_ReviseKeyAccount() {
    try {
        var KeyAccountID = Xrm.Page.data.entity.getId();

        if (KeyAccountID != null) {
            var workflowId = 'e0ff8fe1-400e-451b-a12b-9bc8a4ca4fb7';
            var workflowName = 'Revise Key Account';
            ExecuteWorkflowWithoutAlertSuccess(workflowId, workflowName, function () { OpenKeyAccountNew(); });
        }
        else {
            alert('Record Cannot be Revised, Record not found!');
        }
    } catch (e) {

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

function ExecuteWorkflowWithoutAlertSuccess(workflowId, workflowName, failedCallback) {
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
            AssignResponseWithoutAlertSuccess(req, workflowName, failedCallback);
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

function AssignResponseWithoutAlertSuccess(req, workflowName, successCallback, failedCallback) {
    if (req.readyState == 4) {
        if (req.status == 200) {
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

function IsConfirm() {
    var flag = true;

    if (Xrm.Page.ui.getFormType() > 1)
    {
        var keyAccountID = Xrm.Page.data.entity.getId();
        XrmServiceToolkit.Rest.Retrieve(keyAccountID, "tss_keyaccountSet", "tss_Reason,tss_Status", null, function (result) {
            var tss_Status = result.tss_Status.Value;
            flag = tss_Status == 865920000;
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, false);
    }

    console.log("Flag: ", flag);
    return flag;
}

function OpenKeyAccountNew() {
    try {
        var NewKAId;
        var KeyAccountID = Xrm.Page.getAttribute("tss_kamsid").getValue();
        var windowOptions = {
            openInNewWindow: true
        };

        XrmServiceToolkit.Rest.RetrieveMultiple("tss_keyaccountSet", "?$select=tss_keyaccountId&$filter=tss_KAMSId eq '" + KeyAccountID + "'&$orderby=CreatedOn desc", function (results) {
            NewKAId = results[0].tss_keyaccountId;
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, function () {
            //On Complete - Do Something
        }, false);

        if (NewKAId != null && NewKAId != 'undefined') Xrm.Utility.openEntityForm("tss_keyaccount", NewKAId, null, null);
    }
    catch (e) {
        alert('Failed try to open Key Account because ' + e.message);
    }
}

function GenerateMasterMarketSize(grid, selectedItemCount, selectedIds) {
    debugger;

    if (selectedItemCount > 0) {
        //alert(selectedItemIds);
        var actionId = '9A610288-3511-430E-A61C-DF4F1CC96A43';
        var actionName = 'new_AGITKeyAccountGenerateMasterMarketSizeFromKeyAccount';

        ExecuteAction_GenerateMasterMarketSize(actionName, selectedIds, function () { Xrm.Page.data.refresh(true); });
    }
    else {
        alert("Please choose Key Account to generate Master Market Size !");
    }
}

function ExecuteAction_GenerateMasterMarketSize(actionName, selectedIds, successCallback, failedCallback) {
    var _return = window.confirm('Do you want to Generate Master Market Size From Key Account ?');
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
        req.open("POST", oDataPath + actionName, false);
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState == 4) {
                req.onreadystatechange = null;
                if (this.status == 200 || this.status == 204) {
                    Xrm.Utility.alertDialog('Successfully executed Generate Master Market Size From Key Account.');
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
                    Xrm.Utility.alertDialog('Fail to Generate Master Market Size From Key Account .\r\nResponse Code : (' + req.status + ')' + "\r\nPlease contact your IT support.\r\nTechnical Details : " + error);
                }
            }
        };

        req.send(JSON.stringify(parameters));
    }
}

function Show_Revise() {
    var _result = false;
    var _ispss = IsPSS();
    var _status = GetAttributeValue(Xrm.Page.getAttribute("tss_status"));
    var _isevaluation = GetAttributeValue(Xrm.Page.getAttribute("tss_isevaluation"));

    //if (_status != null) {
    //    if (_ispss && ((_status == 865920000 || _status == 865920004) || _isevaluation == true)) {
    //        _result = true;
    //    }
    //}

    if (_isevaluation != null) {
        if (_isevaluation) {
            _result = true;
        }
    }

    return _result;
}

//function ExecuteWorkflow_GenerateMasterMarketSize(workflowId, workflowName, selectedItemIds, successCallback, failedCallback) {
//    var _return = window.confirm('Do you want to ' + workflowName + ' ?');
//    if (_return) {
//        //var url = Xrm.Page.context.getServerUrl();
//        var url = document.location.protocol + "//" + document.location.host + "/" + Xrm.Page.context.getOrgUniqueName();
//        //var entityId = Xrm.Page.data.entity.getId();
//        var OrgServicePath = "/XRMServices/2011/Organization.svc/web";
//        url = url + OrgServicePath;
//        var request;
//        request = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
//			"<s:Body>" +
//			"<Execute xmlns=\"http://schemas.microsoft.com/xrm/2011/Contracts/Services\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">" +
//			"<request i:type=\"b:ExecuteWorkflowRequest\" xmlns:a=\"http://schemas.microsoft.com/xrm/2011/Contracts\" xmlns:b=\"http://schemas.microsoft.com/crm/2011/Contracts\">" +
//			"<a:Parameters xmlns:c=\"http://schemas.datacontract.org/2004/07/System.Collections.Generic\">" +

//            //"<a:KeyValuePairOfstringanyType>" +
//			//"<c:key>EntityId</c:key>" +
//			//"<c:value i:type=\"d:guid\" xmlns:d=\"http://schemas.microsoft.com/2003/10/Serialization/\">" + entityId + "</c:value>" +
//			//"</a:KeyValuePairOfstringanyType>" +

//            "<a:KeyValuePairOfstringanyType>" +
//			"<c:key>WorkflowId</c:key>" +
//			"<c:value i:type=\"d:guid\" xmlns:d=\"http://schemas.microsoft.com/2003/10/Serialization/\">" + workflowId + "</c:value>" +
//			"</a:KeyValuePairOfstringanyType>" +

//            "<a:KeyValuePairOfstringanyType>" +
//			"<c:key>SelectedItemIds</c:key>" +
//            "<c:value i:type=\"d:string\" xmlns:d=\"http://www.w3.org/2001/XMLSchema\">" + selectedItemIds + "</c:value>" +
//			"</a:KeyValuePairOfstringanyType>" +

//			"</a:Parameters>" +
//			"<a:RequestId i:nil=\"true\" />" +
//			"<a:RequestName>ExecuteWorkflow</a:RequestName>" +
//			"</request>" +
//			"</Execute>" +
//			"</s:Body>" +
//			"</s:Envelope>";
//        var req = new XMLHttpRequest();
//        req.open("POST", url, true)
//        // Responses will return XML. It isn't possible to return JSON.
//        req.setRequestHeader("Accept", "application/xml, text/xml, */*");
//        req.setRequestHeader("Content-Type", "text/xml; charset=utf-8");
//        req.setRequestHeader("SOAPAction", "http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Execute");
//        req.onreadystatechange = function () {
//            AssignResponse(req, workflowName, successCallback, failedCallback);
//        };
//        req.send(request);
//    }
//}