///<reference path="MSXRMTOOLS.Xrm.Page.2016.js"/>
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// Helper
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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

var SourceTypes = {
    DirectSales: 865920000,
    MarketSize: 865920001,
    Service: 865920002,
    Dealer: 865920003,
    Counter: 865920004
};
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FORM ON LOAD AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function OnLoad() {
    setSourceType();
    if (formType == formStatus.Create) {
        CreateSO_CounterAndDealer();
    }
    if (formType > 1) {
        CheckSOTotalAmountOnRefreshGrid("SOPartLines");
        MirrorSoldToWithCustomer();
        MirrorSoldToContactWithContact();
    }
    EnableField();
    LockFieldCheckup();
    //refresh ribbon
    refreshRibbonOnChange();
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FIELD EVENT AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// PUBLICS AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function refreshRibbonOnChange() {
    Xrm.Page.ui.refreshRibbon();
}

function CreateSO_CounterAndDealer() {
    Xrm.Page.getControl("tss_sourcetype").removeOption(SourceTypes.DirectSales);
    Xrm.Page.getControl("tss_sourcetype").removeOption(SourceTypes.MarketSize);
    Xrm.Page.getControl("tss_sourcetype").removeOption(SourceTypes.Service);

    if (Xrm.Page.getAttribute("tss_quotationlink").getValue() == null && Xrm.Page.getAttribute("tss_statecode").getValue() == 865920000 && Xrm.Page.getAttribute("tss_statusreason").getValue() == 865920000) {
        setDisabledField("tss_sourcetype", true);
    }
    else {
        setDisabledField("tss_sourcetype", true);
    }
}

function HideGrid() {
    var source = Xrm.Page.getAttribute("tss_sourcetype").getValue();
    Xrm.Page.ui.tabs.get("tab_11").setVisible(true);
    Xrm.Page.ui.tabs.get("tab_12").setVisible(true);

    if (source == SourceTypes.Service) {
        Xrm.Page.ui.tabs.get("tab_9").setVisible(true);
        Xrm.Page.ui.tabs.get("tab_10").setVisible(true);
    }
    else if (source == SourceTypes.Dealer || source == SourceTypes.Counter) {
        Xrm.Page.ui.tabs.get("tab_11").setVisible(false);
        Xrm.Page.ui.tabs.get("tab_12").setVisible(false);
    }
    else {
        Xrm.Page.ui.tabs.get("tab_9").setVisible(false);
        Xrm.Page.ui.tabs.get("tab_10").setVisible(false);
    }

    if (source == SourceTypes.DirectSales || source == SourceTypes.MarketSize || source == SourceTypes.Service) {
        makeReadOnly();
    }

    if (Xrm.Page.getAttribute("ownerid").getValue()[0].id.replace(/[{}]/g, "").toLowerCase() != Xrm.Page.context.getUserId().replace(/[{}]/g, "").toLowerCase() && Xrm.Page.getAttribute("tss_statussaleseffort").getValue() == null) {
        makeReadOnlyGrid();
    }
}

var intervalId = true;
function makeReadOnly() {
    var sopartlinesGrid = 'SOPartLines';
    try {
        var subgridsLoaded = false;
        if ($("div[id$='" + sopartlinesGrid + "']").length > 0 && !subgridsLoaded) {
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
        alert("makeReadOnly() Error: " + e.message);
    }
}

function makeReadOnlyGrid() {
    var gridName = "SalesEffort";
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

function setDisabledField(field, value) {
    Xrm.Page.getControl(field).setDisabled(value);
}

function EnableField() {

    if (Xrm.Page.getAttribute("tss_statecode").getValue() == 865920000 || Xrm.Page.getAttribute("tss_statusreason").getValue() == 865920000) {
        var source = Xrm.Page.getAttribute("tss_sourcetype").getValue();
        Xrm.Page.ui.tabs.get("tab_6").setVisible(false);
        if (Xrm.Page.getAttribute("tss_package").getValue() == true) {
            Xrm.Page.ui.tabs.get("tab_6").setVisible(true);
        }

        if (source == SourceTypes.Dealer || source == SourceTypes.Counter) {
            setDisabledField("tss_requestdeliverydate", false);
            setDisabledField("tss_distributionchannel", false);
            setDisabledField("tss_customer", false);
            setDisabledField("tss_contact", false);
            setDisabledField("tss_currency", true);
            setDisabledField("tss_paymentterm", false);
            setDisabledField("tss_ponumber", false);
            setDisabledField("tss_podate", false);
            setDisabledField("tss_top", false);
            setDisabledField("tss_ordertype", false);
            Xrm.Page.ui.tabs.get("tab_6").setVisible(false);
        }
        else {
            setDisabledField("tss_customer", true);
            setDisabledField("tss_currency", true);
            setDisabledField("tss_ordertype", false);
        }
    }
    else {
        disableFormFields();
        makeReadOnly();
    }
}

function setVisibleField(field, value) {
    Xrm.Page.getControl(field).setVisible(value);
}

function ShowField() {
    if (Xrm.Page.getAttribute("tss_quotationlink").getValue() != null) {
        setVisibleField("tss_quotationlink", true);
    }
    else {
        setVisibleField("tss_quotationlink", false);
    }

    if (Xrm.Page.getAttribute("tss_prospectlink").getValue() != null) {
        setVisibleField("tss_prospectlink", true);
    }
    else {
        setVisibleField("tss_prospectlink", false);
    }
}

function CancelSO_onClick() {
    //A9A4E06B-C74A-412D-B7E6-E6034358785A
    var approve = confirm("Are you sure to Cancel Sales Order ?");
    if (approve) {
        var guid = Xrm.Page.data.entity.getId();
        var entityName = Xrm.Page.data.entity.getEntityName();
        if (guid == null) {
            alert('Record not found!');
        }
        else {
            //alert(guid);
            var DialogGUID = "A9A4E06B-C74A-412D-B7E6-E6034358785A";
            var serverUrl;
            if (Xrm.Page.context.getClientUrl !== undefined) {
                serverUrl = Xrm.Page.context.getClientUrl();
            } else {
                serverUrl = Xrm.Page.context.getServerUrl();
            }
            var url = serverUrl + "/cs/dialog/rundialog.aspx?DialogId=" + "{" + DialogGUID + "}" + "&EntityName=" + entityName + "&ObjectId=" + guid;
            //alert(url);
            window.showModalDialog(url, null, "dialogHeight:540px;dialogWidth:720px;center:yes;resizable:1;maximize:1;minimize:1;status:no;scroll:no");
            window.location.reload(true);
        }
    }
}

function SubmitToSAP_onClick() {
    //alert('Submit to SAP!');
    var workflowId = '6D50721F-58B8-482F-9359-277657309176';
    var workflowName = 'Submit SO to SAP';
    var currentDateTime = new Date();
    Xrm.Page.getAttribute("tss_sosubmitdate").setValue(currentDateTime);
    Xrm.Page.getAttribute("tss_sosubmitdate").setSubmitMode("always");
    Xrm.Page.getAttribute("tss_sosubmitdate").fireOnChange();
    ExecuteWorkflow(workflowId, workflowName, function () { RefreshForm(); });
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
if (!window.showModalDialog) {
    window.showModalDialog = function (arg1, arg2, arg3) {
        var w;
        var h;
        var resizable = "no";
        var scroll = "no";
        var status = "no";
        // get the modal specs
        var mdattrs = arg3.split(";");
        for (i = 0; i < mdattrs.length; i++) {
            var mdattr = mdattrs[i].split(":");
            var n = mdattr[0];
            var v = mdattr[1];
            if (n) {
                n = n.trim().toLowerCase();
            }
            if (v) {
                v = v.trim().toLowerCase();
            }
            if (n == "dialogheight") {
                h = v.replace("px", "");
            }
            else if (n == "dialogwidth") {
                w = v.replace("px", "");
            }
            else if (n == "resizable") {
                resizable = v;
            }
            else if (n == "scroll") {
                scroll = v;
            }
            else if (n == "status") {
                status = v;
            }
        }
        var left = window.screenX + (window.outerWidth / 2) - (w / 2);
        var top = window.screenY + (window.outerHeight / 2) - (h / 2);
        var targetWin = window.open(arg1, arg1, 'toolbar=no, location=no, directories=no, status=' + status + ', menubar=no, scrollbars=' + scroll + ', resizable=' + resizable + ', copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
        targetWin.focus();
    };
}

function ReloadForm() {
    var entityId = Xrm.Page.data.entity.getId();
    Xrm.Utility.openEntityForm("tss_sopartheader", entityId)
}

function SetCurrentUserID(fieldName) {
    var currentUser = new Array();
    currentUser[0] = new Object();
    currentUser[0].id = Xrm.Page.context.getUserId();
    currentUser[0].name = Xrm.Page.context.getUserName();
    currentUser[0].entityType = 'systemuser'; 

    Xrm.Page.getControl(fieldName).setDisabled(false);
    Xrm.Page.getAttribute(fieldName).setValue(currentUser);
    Xrm.Page.getAttribute(fieldName).setSubmitMode("always");
    Xrm.Page.data.save();
    Xrm.Page.getControl(fieldName).setDisabled(true);
}

function OpenAccount() {
    var totalAmount = Xrm.Page.getAttribute("tss_totalamount").getValue();

    var parameters = {};
    parameters["param_tss_plafondarsparepart"] = totalAmount;

    var customerId = Xrm.Page.getAttribute("tss_customer").getValue()[0].id;

    //Xrm.Utility.openEntityForm("account", customerId, parameters);
    Xrm.Page.data.save();
    Xrm.Utility.openEntityForm("account", customerId);
}

function ApproveCreditLimitTransaction_onClick() {
    Xrm.Page.getAttribute("tss_approvecreditlimit").setValue(true);
    Xrm.Page.getAttribute("tss_approvecreditlimit").setSubmitMode("always");
    SetCurrentUserID("tss_approvecreditlimitby");

    //set plafon below so amount to approve
    Xrm.Page.getAttribute("tss_plafondbelowsoamount").setValue(865920002);
    Xrm.Page.getAttribute("tss_plafondbelowsoamount").setSubmitMode("always");
    Xrm.Page.data.save();

    ReloadForm();
}

function ApproveAddPlafondCreditLimit_onClick() {
    SetCurrentUserID("tss_approvecreditlimitby");

    OpenAccount();
}

function SetPSSLookup() {
    var source = Xrm.Page.getAttribute("tss_sourcetype").getValue();

    if (source == SourceTypes.Counter || source == SourceTypes.Dealer) {
        addPSSFilter();
    }
}

function addPSSFilter() {
    var viewId = "{6fd72744-3676-41d4-8003-ae4cde9ac282}";
    var entityName = "systemuser";
    var viewDisplayName = "PSS";

    var fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>" +
                        "<entity name='systemuser'>" +
                            "<attribute name='fullname' />" +
                            "<attribute name='title' />" +
                            "<attribute name='systemuserid' />" +
                            " <order attribute='fullname' descending='false' />" +
                            "<filter type='and'>" +
                                "<condition attribute='isdisabled' operator='eq' value='0'  />" +
                                "<condition attribute='title' operator='eq' value='PSS'  />" +
                            "</filter>" +
                          "</entity>" +
                    "</fetch>";

    var layoutXml = "<grid name='resultset' " +
                    "object='1' " +
                    "jump='systemuserid' " +
                    "select='1' " +
                    "icon='1' " +
                    "preview='1'>" +
                    "<row name='result' " +
                    "id='systemuserid'>" +
                    "<cell name='fullname' " +
                    "width='200' />" +
                    "<cell name='title' " +
                    "width='200' />" +
                    "</row>" +
                    "</grid>";

    Xrm.Page.getControl("tss_pss").addCustomView(viewId, entityName, viewDisplayName, fetchXml, layoutXml, true);
}

function SetContact() {
    var source = Xrm.Page.getAttribute("tss_sourcetype").getValue();
    if (source == SourceTypes.Dealer || source == SourceTypes.Counter) {
        if (Xrm.Page.getAttribute("tss_customer").getValue() != null) {
            var customerid = Xrm.Page.getAttribute("tss_customer").getValue()[0].id;
            if (customerid != null) {
                XrmServiceToolkit.Rest.Retrieve(customerid, "AccountSet", "PrimaryContactId", null, function (result) {
                    if (result != null) {
                        var primaryContactId = result.PrimaryContactId;
                        if (primaryContactId != null) {
                            Xrm.Page.getAttribute("tss_contact").setValue([{ id: primaryContactId.Id, name: primaryContactId.Name, entityType: primaryContactId.LogicalName }]);
                            Xrm.Page.getAttribute("tss_contact").setSubmitMode("always");
                        }
                    }
                }, function (error) {
                    Xrm.Utility.alertDialog(error.message);
                }, false);
            }
        }
        else {
            Xrm.Page.getAttribute("tss_contact").setValue(null);
            Xrm.Page.getAttribute("tss_contact").setSubmitMode("always");
        }
        Xrm.Page.getAttribute("tss_contact").fireOnChange();
    }
}

function SetBranch() {
    var source = Xrm.Page.getAttribute("tss_sourcetype").getValue();
    if (source == SourceTypes.Counter || source == SourceTypes.Dealer) {
        if (Xrm.Page.getAttribute("tss_pss").getValue() != null) {
            var userid = Xrm.Page.getAttribute("tss_pss").getValue()[0].id;
            if (userid != null) {
                XrmServiceToolkit.Rest.Retrieve(userid, "SystemUserSet", "BusinessUnitId", null, function (result) {
                    var businessUnitId = result.BusinessUnitId;
                    if (businessUnitId != null) {
                        Xrm.Page.getAttribute("tss_branch").setValue([{ id: businessUnitId.Id, name: businessUnitId.Name, entityType: businessUnitId.LogicalName }]);
                        Xrm.Page.getAttribute("tss_branch").setSubmitMode("always");
                    }
                }, function (error) {
                    Xrm.Utility.alertDialog(error.message);
                }, false);
            }
        }
        else {
            Xrm.Page.getAttribute("tss_branch").setValue(null);
            Xrm.Page.getAttribute("tss_branch").setSubmitMode("always");
        }
    }
}

function SubmitSalesEffort_onClink() {
    //update to waiting approval
    Xrm.Page.getAttribute("tss_statussaleseffort").setValue(865920000);
    Xrm.Page.getAttribute("tss_statussaleseffort").setSubmitMode("always");

    if (Xrm.Page.getAttribute("tss_sendemailsaleseffort").getValue() != null) {
        if (!Xrm.Page.getAttribute("tss_sendemailsaleseffort").getValue()) {
            //update send email to trigger plugin
            Xrm.Page.getAttribute("tss_sendemailsaleseffort").setValue(true);
            Xrm.Page.getAttribute("tss_sendemailsaleseffort").setSubmitMode("always");
            Xrm.Page.getAttribute("tss_sendemailsaleseffortmessage").setValue("send");
            Xrm.Page.getAttribute("tss_sendemailsaleseffortmessage").setSubmitMode("always");

            //update datetime to now
            var currentDateTime = new Date();
            Xrm.Page.getAttribute("tss_saleseffortdatetime").setValue(currentDateTime);
            Xrm.Page.getAttribute("tss_saleseffortdatetime").setSubmitMode("always");

            //reload form
            ReloadForm();
        }
    }
}

function SubmitSalesEffort_Display() {
    if (formType > 1) {
        var entityId = Xrm.Page.data.entity.getId();
        var userId = Xrm.Page.context.getUserId();
        var count;

        XrmServiceToolkit.Rest.RetrieveMultiple("tss_sopartsaleseffortSet", "?$select=tss_sopartsaleseffortId&$filter=tss_sopartheaderId/Id eq (guid'" + entityId.replace("{", "").replace("}", "") + "')", function (results) {
            count = results.length;
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, function () {
        }, false);

        if (Xrm.Page.getAttribute("tss_statussaleseffort").getValue() == null && count > 0 && Xrm.Page.getAttribute("ownerid").getValue()[0].id.replace(/[{}]/g, "").toLowerCase() == userId.replace(/[{}]/g, "").toLowerCase()) return true;
        else return false;
    }
    return false;
}

function SetCustomerLookup() {
    var source = Xrm.Page.getAttribute("tss_sourcetype").getValue();

    if (source == SourceTypes.Dealer) {
        addCustomerFilter();
    }
    else if (source == SourceTypes.Counter) {
        addCustomerFilterCounter();
    }
    else {
        addCustomerFilterCounter();
    }
}

function addCustomerFilter() {
    var viewId = "{6fd72744-3676-41d4-8003-ae4cde9ac284}";
    var entityName = "account";
    var viewDisplayName = "Customer";

    var fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>" +
                        "<entity name='account'>" +
                            "<attribute name='name' />" +
                            "<attribute name='accountid' />" +
                            "<attribute name='accountnumber' />" +
                            "<attribute name='new_npwp' />" +
                            " <order attribute='name' descending='false' />" +
                            "<filter type='and'>" +
                                "<condition attribute='statecode' operator='eq' value='0'  />" +
                                "<condition attribute='new_category' operator='eq' value='5'  />" +
                            "</filter>" +
                          "</entity>" +
                    "</fetch>";

    var layoutXml = "<grid name='resultset' " +
                    "object='1' " +
                    "jump='accountid' " +
                    "select='1' " +
                    "icon='1' " +
                    "preview='1'>" +
                    "<row name='result' " +
                    "id='accountid'>" +
                    "<cell name='name' " +
                    "width='200' />" +
                    "<cell name='accountnumber' " +
                    "width='200' />" +
                    "<cell name='new_npwp' " +
                    "width='200' />" +
                    "</row>" +
                    "</grid>";

    Xrm.Page.getControl("tss_customer").addCustomView(viewId, entityName, viewDisplayName, fetchXml, layoutXml, true);
}

function addCustomerFilterCounter() {
    var viewId = "{6fd72744-3676-41d4-8003-ae4cde9ac288}";
    var entityName = "account";
    var viewDisplayName = "Customer";

    var fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>" +
                        "<entity name='account'>" +
                            "<attribute name='name' />" +
                            "<attribute name='accountid' />" +
                            "<attribute name='accountnumber' />" +
                            "<attribute name='new_npwp' />" +
                            " <order attribute='name' descending='false' />" +
                            "<filter type='and'>" +
                                "<condition attribute='statecode' operator='eq' value='0'  />" +
                                //"<condition attribute='new_category' operator='ne' value='5'  />" +
                            "</filter>" +
                          "</entity>" +
                    "</fetch>";

    var layoutXml = "<grid name='resultset' " +
                    "object='1' " +
                    "jump='accountid' " +
                    "select='1' " +
                    "icon='1' " +
                    "preview='1'>" +
                    "<row name='result' " +
                    "id='accountid'>" +
                    "<cell name='name' " +
                    "width='200' />" +
                    "<cell name='accountnumber' " +
                    "width='200' />" +
                    "<cell name='new_npwp' " +
                    "width='200' />" +
                    "</row>" +
                    "</grid>";

    Xrm.Page.getControl("tss_customer").addCustomView(viewId, entityName, viewDisplayName, fetchXml, layoutXml, true);
}

function SetTOP() {
    var source = Xrm.Page.getAttribute("tss_sourcetype").getValue();

    if (source == SourceTypes.Dealer) {
        if (Xrm.Page.getAttribute("tss_customer").getValue() != null) {
            var customer = Xrm.Page.getAttribute("tss_customer").getValue()[0].id;

            //get date with format 2017-12-31T17:00:00.000Z
            var currdate = new Date();
            if (Xrm.Page.getAttribute("createdon").getValue() != null) {
                currdate = Xrm.Page.getAttribute("createdon").getValue();
            }
            else {
                currdate = new Date();
            }
            var formattedDate =
                    currdate.getFullYear() + "-" +
                    ("00" + (currdate.getMonth() + 1)).slice(-2) + "-" +
                    ("00" + currdate.getDate()).slice(-2) + "T" +
                    ("00" + currdate.getHours()).slice(-2) + ":" +
                    ("00" + currdate.getMinutes()).slice(-2) + ":" +
                    ("00" + currdate.getSeconds()).slice(-2) + ".000Z";

            if (customer != null) {
                var top;
                var paymentTerm;
                XrmServiceToolkit.Rest.RetrieveMultiple("tss_dealerheaderSet", "?$select=tss_PaymentTerm,tss_TOPbyBankFinancing&$filter=tss_StartDate le datetime'" + formattedDate + "' and  tss_EndDate ge datetime'" + formattedDate + "' and tss_DealerName/Id eq (guid'" + customer + "')", function (results) {
                    if (results.length > 0) {
                        if (results[0].tss_TOPbyBankFinancing != null) {
                            Xrm.Page.getAttribute("tss_top").setValue(results[0].tss_TOPbyBankFinancing.Value);
                            Xrm.Page.getAttribute("tss_top").setSubmitMode("always");

                            top = results[0].tss_TOPbyBankFinancing.Value;
                            if (results[0].tss_PaymentTerm != null) {
                                paymentTerm = results[0].tss_PaymentTerm;
                            }
                        }
                    }
                }, function (error) {
                    Xrm.Utility.alertDialog(error.message);
                }, function () {
                }, false);

                if (top == 865920001 && paymentTerm != null) {
                    Xrm.Page.getAttribute("tss_paymentterm").setValue([{ id: paymentTerm.Id, name: paymentTerm.Name, entityType: paymentTerm.LogicalName }]);
                    Xrm.Page.getAttribute("tss_paymentterm").setSubmitMode("always");
                }
                else if (top == 865920000) {
                    XrmServiceToolkit.Rest.RetrieveMultiple("trs_paymenttermSet", "?$select=trs_paymenttermId&$filter=trs_name eq 'D1'", function (results) {
                        if (results.length > 0) {
                            Xrm.Page.getAttribute("tss_paymentterm").setValue([{ id: results[i].trs_paymenttermId, name: "D1", entityType: "trs_paymentterm" }]);
                            Xrm.Page.getAttribute("tss_paymentterm").setSubmitMode("always");
                        }
                    }, function (error) {
                        Xrm.Utility.alertDialog(error.message);
                    }, function () {
                    }, false);
                }
            }
        }
        else {
            Xrm.Page.getAttribute("tss_paymentterm").setValue(null);
            Xrm.Page.getAttribute("tss_paymentterm").setSubmitMode("always");
            Xrm.Page.getAttribute("tss_top").setValue(null);
            Xrm.Page.getAttribute("tss_top").setSubmitMode("always");
        }
    }
}

function SetCurrency() {
    XrmServiceToolkit.Rest.RetrieveMultiple("TransactionCurrencySet", "?$select=TransactionCurrencyId&$filter=CurrencyName eq 'IDR'", function (results) {
        if (results.length > 0) {
            var transactionCurrencyId = results[0].TransactionCurrencyId;
            if (transactionCurrencyId != null) {
                Xrm.Page.getAttribute("tss_currency").setValue([{ id: transactionCurrencyId, name: 'IDR', entityType: "transactioncurrency" }]);
                Xrm.Page.getAttribute("tss_currency").setSubmitMode("always");
            }
        }
    }, function (error) {
        Xrm.Utility.alertDialog(error.message);
    }, function () {
        //On Complete - Do Something
    }, false);
}

function OpenImportDialog() {
    if (Xrm.Page.getAttribute("tss_sonumber").getValue() != null) {
        var entityId = Xrm.Page.data.entity.getId();
        var tss_sonumber = Xrm.Page.getAttribute("tss_sonumber").getValue();
        var customParameters = encodeURIComponent("entityid=" + entityId + "&entityname=" + tss_sonumber);
        Xrm.Utility.openWebResource("tss_importsalesorderpartlines.html", customParameters);
    }
}

function GetSelectedRecords(selectedItems) {
    var ct = 0;
    for (i = 0; i < selectedItems.length ; i++) {
        //alert("Id=" + selectedItems[i].Id + "\nName=" + selectedItems.Name + "\nTypeCode=" + selectedItems.TypeCode.toString() + "\nTypeName=" + selectedItems.TypeName);
        ct++;
        var currentUser = Xrm.Page.context.getUserId();
        var entity = {};
        entity.tss_Approvalstatus = true;
        entity.tss_ApproveBy = {
            Id: currentUser,
            LogicalName: "systemuser"
        };

        XrmServiceToolkit.Rest.Update(selectedItems[i].Id, entity, "tss_sopartsaleseffortSet", function () {
            //Success - No Return Data - Do Something
        }, function (error) {
            Xrm.Utility.alertDialog("GetSelectedRecords" + error.message);
        }, true);
    }
    if (ct == selectedItems.length) ReloadForm();
}

function ApproveAllSalesEffort_onClick() {
    Xrm.Page.getAttribute("tss_approveallsaleseffort").setValue(true);
    Xrm.Page.getAttribute("tss_approveallsaleseffort").setSubmitMode("always");
    ReloadForm();
}

function CheckSOTotalAmountOnRefreshGrid(gridElementId) {
    try {
        AddEventOnRefreshGrid(gridElementId, function () {
            GetSOTotalAmountRelatedData(Xrm.Page.data.entity.getId(), function (totalAmount) {
                var compareAttribute = function (attributeName, valueCheck) {
                    var attr = Xrm.Page.getAttribute(attributeName);
                    if (attr !== undefined && attr !== null) {
                        var value = attr.getValue();
                        if (value !== valueCheck) {
                            attr.setValue(valueCheck);
                            attr.setSubmitMode("always");
                            attr.fireOnChange();
                            Xrm.Page.data.setFormDirty(false);
                            Xrm.Page.data.entity.save();
                        }
                    }
                };
                compareAttribute("tss_totalamount", Number(totalAmount));
                //Xrm.Page.data.refresh();
            });
        });
    } catch (e) {
        alert("CheckSOTotalAmountOnRefreshGrid: " + e.message);
    }
}
function AddEventOnRefreshGrid(gridElementId, onRefreshFunction) {
    try {
        var elem = document.getElementById(gridElementId);
        if (elem !== null) {
            var ctrl = document.getElementById(gridElementId).control;
            ctrl.add_onRefresh(function () {
                onRefreshFunction();
            });
        } else {
            setTimeout(function () {
                AddEventOnRefreshGrid(gridElementId, onRefreshFunction);
            }, 1000);
        }
    } catch (e) {
        alert("AddEventOnRefreshGrid: " + e.message);
    }
}
function GetSOTotalAmountRelatedData(soId, successCallback) {
    XrmServiceToolkit.Rest.Retrieve(
        soId,
        "tss_sopartheaderSet",
        "tss_TotalAmount", null,
        function (result) {
            var tss_TotalAmount = result.tss_TotalAmount.Value;
            successCallback(tss_TotalAmount);
        },
        function (error) {
            console.log(error.message);
        },
        true
    );
}

function CheckUserPrivilegeApprove() {
    var flag = true;

    var soId = Xrm.Page.data.entity.getId();
    var UserID = Xrm.Page.context.getUserId();

    XrmServiceToolkit.Rest.RetrieveMultiple(
    "tss_approverlistSet",
    "$select=*&$filter=tss_SalesOrderPartHeaderId/Id eq (guid'" + soId + "') and tss_Approver/Id eq (guid'" + UserID + "') and tss_Type/Value eq 865920000",
    function (results) {
        if (results.length == 0) {
            flag = false;
        }
    },
    function (error) {
        alert('Retrieve Approval List: ' + error.message);
    },
    function onComplete() {
        //alert(" records should have been retrieved.");
    },
    false
    );

    if (flag) {
        return true;
    }
    else {
        return false;
    }

    //return flag;
}

function EnableDisableRibbon_ApproveAllSalesEffort() {
    var approverAllSalesEffort = Xrm.Page.getAttribute("tss_approveallsaleseffort").getValue();
    var sourceType = Xrm.Page.getAttribute("tss_sourcetype").getValue();

    if (formType <= 1) return false;

    if (sourceType == 865920003 || sourceType == 865920004) return false;

    var resultCheck = false;
    var entityId = Xrm.Page.data.entity.getId();
    var userId = Xrm.Page.context.getUserId();
    XrmServiceToolkit.Rest.RetrieveMultiple("tss_approverlistSet", "?$select=tss_approverlistId&$filter=tss_Type/Value eq 865920001 and tss_Approver/Id eq (guid'" + userId + "') and tss_SalesOrderPartHeaderId/Id eq (guid'" + entityId + "')", function (results) {
        if (results.length > 0) resultCheck = true;
    }, function (error) {
        Xrm.Utility.alertDialog(error.message);
    }, function () {
    }, false);

    //if (Xrm.Page.getAttribute("tss_pss").getValue() != null && Xrm.Page.context.getUserId() != null) {
    //    XrmServiceToolkit.Rest.Retrieve(Xrm.Page.getAttribute("tss_pss").getValue()[0].id, "SystemUserSet", "ParentSystemUserId", null, function (result) {
    //        if (result.ParentSystemUserId != null) {
    //            var parentSystemUserId = result.ParentSystemUserId;
    //            if (parentSystemUserId.Id.replace(/[{}]/g, "").toLowerCase() == Xrm.Page.context.getUserId().replace(/[{}]/g, "").toLowerCase()) {
    //                resultCheck = true;
    //            }
    //        }
    //    }, function (error) {
    //        Xrm.Utility.alertDialog(error.message);
    //    }, false);
    //}

    var checkSalesEffort = false;
    XrmServiceToolkit.Rest.RetrieveMultiple("tss_sopartsaleseffortSet", "?$select=tss_sopartsaleseffortId&$filter=tss_sopartheaderId/Id eq (guid'" + entityId + "')", function (results) {
        if (results.length > 0) checkSalesEffort = true;
    }, function (error) {
        Xrm.Utility.alertDialog(error.message);
    }, function () {
    }, false);

    if (resultCheck == true && approverAllSalesEffort == false && checkSalesEffort == true && Xrm.Page.getAttribute("tss_statussaleseffort").getValue() == 865920000) {
        return true;
    }
    else {
        return false;
    }
}

function EnableDisableRibbon_ApproveCreditLimitTransaction() {

    var approverCreditLimit = Xrm.Page.getAttribute("tss_approvecreditlimit").getValue();

    if (formType <= 1) return false;

    if (CheckUserPrivilegeApprove() == true && approverCreditLimit == false) {
        return true;
    }
    else {
        return false;
    }
}

function EnableDisableRibbon_ApprovePlafondCreditLimit() {
    var approverCreditLimit = Xrm.Page.getAttribute("tss_approvecreditlimit").getValue();
    if (formType <= 1) return false;
    if (CheckUserPrivilegeApprove() == true && approverCreditLimit == false) {
        return true;
    }
    else {
        return false;
    }
}

function checkQuotationLink() {
    var quot = Xrm.Page.getAttribute("tss_quotationlink").getValue();
    if (quot != null) {
        setDisabledField("tss_sourcetype", true);
        setDisabledField("tss_pss", true);
        setDisabledField("tss_requestdeliverydate", true);
        setDisabledField("tss_top", true);
    }
}

function checkTotalAmount() {
    if (Xrm.Page.getAttribute("tss_totalamount").getValue() != null) {
        if (Xrm.Page.getAttribute("tss_totalamount").getValue() > 0) {
            return true;
        }
    }
    return false;
}

function setDivision() {
    if (Xrm.Page.getAttribute("tss_division").getValue() == null) {
        XrmServiceToolkit.Rest.RetrieveMultiple("new_divisionSet", "?$select=new_divisionId&$filter=new_Code eq 'B1'", function (results) {
            if (results.length > 0) {
                Xrm.Page.getAttribute("tss_division").setValue([{ id: results[0].new_divisionId, name: "B1", entityType: "new_division" }]);
                Xrm.Page.getAttribute("tss_division").setSubmitMode("always");
            }
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, function () {
            //On Complete - Do Something
        }, false);
    }
}

function setSalesOrganization() {
    if (Xrm.Page.getAttribute("tss_salesorganization").getValue() == null) {
        XrmServiceToolkit.Rest.RetrieveMultiple("new_salesorganizationSet", "?$select=new_salesorganizationId&$filter=new_name eq 'Traktor%20Nusantara'", function (results) {
            if (results.length > 0) {
                Xrm.Page.getAttribute("tss_salesorganization").setValue([{ id: results[0].new_salesorganizationId, name: "Traktor Nusantara", entityType: "new_salesorganization" }]);
                Xrm.Page.getAttribute("tss_salesorganization").setSubmitMode("always");
            }
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, function () {
            //On Complete - Do Something
        }, false);
    }
}

function disableFormFields() {
    Xrm.Page.ui.controls.forEach(function (control, index) {
            control.setDisabled(true);
    });
}

function MirrorSoldToWithCustomer() {
    //tss_customer
    //tss_soldto
    var customer = Xrm.Page.getAttribute("tss_customer").getValue();
    var soldTo = Xrm.Page.getAttribute("tss_soldto").getValue();
    if (customer != null) {
        Xrm.Page.getAttribute("tss_soldto").setValue([
            customer[0]
        ]);
        Xrm.Page.getAttribute("tss_soldto").setSubmitMode("always");

        Xrm.Page.getAttribute("tss_billto").setValue([
           customer[0]
        ]);
        Xrm.Page.getAttribute("tss_billto").setSubmitMode("always");

        Xrm.Page.getAttribute("tss_shipto").setValue([
           customer[0]
        ]);
        Xrm.Page.getAttribute("tss_shipto").setSubmitMode("always");


        //Xrm.Page.getAttribute("tss_contact").fireOnChange();
    } else if (customer == null) {
        Xrm.Page.getAttribute("tss_soldto").setValue(null);
        Xrm.Page.getAttribute("tss_soldto").setSubmitMode("always");

        Xrm.Page.getAttribute("tss_billto").setValue(null);
        Xrm.Page.getAttribute("tss_billto").setSubmitMode("always");

        Xrm.Page.getAttribute("tss_shipto").setValue(null);
        Xrm.Page.getAttribute("tss_shipto").setSubmitMode("always");

        Xrm.Page.getAttribute("tss_soldtocontact").setValue(null);
        Xrm.Page.getAttribute("tss_soldtocontact").setSubmitMode("always");

        Xrm.Page.getAttribute("tss_billtocontact").setValue(null);
        Xrm.Page.getAttribute("tss_billtocontact").setSubmitMode("always");

        Xrm.Page.getAttribute("tss_shiptocontact").setValue(null);
        Xrm.Page.getAttribute("tss_shiptocontact").setSubmitMode("always");
    }

}

function RemoveSoldToContact() {
    var soldTo = Xrm.Page.getAttribute("tss_soldto").getValue();
    if (soldTo == null) {
        Xrm.Page.getAttribute("tss_soldtocontact").setValue(null);
        Xrm.Page.getAttribute("tss_soldtocontact").setSubmitMode("always");
    }
}

function RemoveBillToContact() {
    var billTo = Xrm.Page.getAttribute("tss_billto").getValue();
    if (billTo == null) {
        Xrm.Page.getAttribute("tss_billtocontact").setValue(null);
        Xrm.Page.getAttribute("tss_billtocontact").setSubmitMode("always");
    }
}

function RemoveShipToContact() {
    var shipTo = Xrm.Page.getAttribute("tss_shipto").getValue();
    if (shipTo == null) {
        Xrm.Page.getAttribute("tss_shiptocontact").setValue(null);
        Xrm.Page.getAttribute("tss_shiptocontact").setSubmitMode("always");
    }
}

function MirrorSoldToContactWithContact() {
    //tss_contact
    //tss_soldtocontact
    var contact = Xrm.Page.getAttribute("tss_contact").getValue();
    var soldToContact = Xrm.Page.getAttribute("tss_soldtocontact").getValue();
    if (contact != null ) {
        Xrm.Page.getAttribute("tss_soldtocontact").setValue([contact[0]]);
        Xrm.Page.getAttribute("tss_soldtocontact").setSubmitMode("always");

        Xrm.Page.getAttribute("tss_billtocontact").setValue([contact[0]]);
        Xrm.Page.getAttribute("tss_billtocontact").setSubmitMode("always");

        Xrm.Page.getAttribute("tss_shiptocontact").setValue([contact[0]]);
        Xrm.Page.getAttribute("tss_shiptocontact").setSubmitMode("always");
    } else if (contact == null) {
        Xrm.Page.getAttribute("tss_soldtocontact").setValue(null);
        Xrm.Page.getAttribute("tss_soldtocontact").setSubmitMode("always");
        Xrm.Page.getAttribute("tss_billtocontact").setValue(null);
        Xrm.Page.getAttribute("tss_billtocontact").setSubmitMode("always");
        Xrm.Page.getAttribute("tss_shiptocontact").setValue(null);
        Xrm.Page.getAttribute("tss_shiptocontact").setSubmitMode("always");
    }
}


function LockAllField(exceptionFieldArray) {
    if (exceptionFieldArray === "undefined" || exceptionFieldArray == null)
        exceptionFieldArray = [];
    Xrm.Page.ui.controls.forEach(function (control, index) {
        var isExcepted = false;
        for (var idx = 0; idx < exceptionFieldArray.length; idx++) {
            if (exceptionFieldArray[idx] === control.getName()) isExcepted = true;
        }

        console.log("Setting control " + control.getName() + " to ." + !isExcepted);
        control.setDisabled(!isExcepted);
    });
}

function LockFieldCheckup() {

    /*
     *  [13:58, 2/14/2018] Yenti Tjia (MIO): Status Reason = Active, Status = New , Owner = User, Source Type = Counter / Dealer,
     *  bisa edit field:
     *  PO Number, PO Date, Source Type, PSS, Request Delivery Date, Sales Order Quotation, Distribution Channel, Division, Sales Organization,
     *  Customer, Contact, Currency, Contact Sold To, Bill To & Contact, Ship To & Contact, TOP, Payment Term, Add new part Line
     *  [13:59, 2/14/2018] Yenti Tjia (MIO): Status Reason = Active, Status = New , Owner = User, Source Type = selain 'Counter / Dealer',
     * bisa edit field: PO Number, PO Date, Distribution Channel, Division, Sales Organization, Contact, Contact Sold To, Bill To & Contact, Ship To & Contact, Payment Term
     */
    var statusReasonConst = {
        Active: 865920000,
        Submitted: 865920001,
        Closed: 865920002,
        Reserved: 865920003,
        Fulfilled: 865920004
    }

    var statusReason = Xrm.Page.getAttribute("tss_statusreason").getValue();

    var statusConst = {
        New: 865920000
    }

    var status = Xrm.Page.getAttribute("tss_statecode").getValue();

    var owner = Xrm.Page.getAttribute("ownerid").getValue();
    var currentUser = Xrm.Page.context.getUserId();

    var sourceTypeConst = {
        DirectSales: 865920000,
        MarketSize: 865920001,
        Service: 865920002,
        Dealer: 865920003,
        Counter: 865920004
    }

    var sourceType = Xrm.Page.getAttribute("tss_sourcetype").getValue();

    var counterOrDealerAllowField = [
        "tss_ponumber", "tss_podate", "tss_sourcetype", "tss_pss", "tss_requestdeliverydate", "tss_salesorderquotation",
        "tss_distributionchannel", "tss_division", "tss_salesorganization", "tss_customer", "tss_contact", "tss_currency",
        "tss_soldtocontact", "tss_billtocontact", "tss_shiptocontact", "tss_top", "tss_paymentterm", "tss_billto", "tss_shipto","tss_ordertype"
    ];

    var elseThanCounterOrDealerAllowedField = [
        "tss_ponumber", "tss_podate", "tss_distributionchannel", "tss_division", "tss_salesorganization", "tss_contact",
        "tss_soldtocontact", "tss_billtocontact", "tss_shiptocontact", "tss_paymentterm", "tss_billto", "tss_shipto", "tss_ordertype"
    ];

    console.log("Entering Lock Field Checkup");
    console.log(statusReason);
    console.log(status);
    console.log(owner);
    console.log(sourceType);

    if (formType !== formStatus.Update) {
        console.log("Form Type is not Update, exiting function");
        return;
    }

    if (statusReason === statusReasonConst.Active &&
        status === statusConst.New &&
        (sourceType === sourceTypeConst.Counter || sourceType === sourceTypeConst.Dealer) &&
        owner[0].id === currentUser) {
        console.log("Source Type is Dealer / Counter");
        console.log("Locking all field else than below");
        console.log(counterOrDealerAllowField);
        LockAllField(counterOrDealerAllowField);
    }
    else if (statusReason === statusReasonConst.Active &&
        status === statusConst.New &&
        (sourceType !== sourceTypeConst.Counter && sourceType !== sourceTypeConst.Dealer) &&
        owner[0].id === currentUser) {
        console.log("Source Type is not Dealer / Counter");
        console.log("Locking all field else than below");
        console.log(elseThanCounterOrDealerAllowedField);
        LockAllField(elseThanCounterOrDealerAllowedField);
    } else {
        console.log("Not in check condition, cannot edit.");
        LockAllField();
        makeReadOnly();
    }
}

function setRibbonImportLines() {
    var userId = Xrm.Page.context.getUserId();
    if (Xrm.Page.getAttribute("tss_statusreason").getValue() != null && Xrm.Page.getAttribute("tss_statecode").getValue() != null) {
        if (Xrm.Page.getAttribute("tss_statusreason").getValue() == 865920000 && Xrm.Page.getAttribute("tss_statecode").getValue() == 865920000 && Xrm.Page.getAttribute("ownerid").getValue()[0].id.replace(/[{}]/g, "").toLowerCase() == userId.replace(/[{}]/g, "").toLowerCase() && Xrm.Page.getAttribute("tss_clcurrentapprover").getValue() == null && Xrm.Page.ui.getFormType() > 1) {
            return true;
        }
    }
    return false;
}

function setRibbonCancelSO() {
    var userId = Xrm.Page.context.getUserId();
    if (Xrm.Page.getAttribute("tss_sonumber").getValue() != null) {
        if (Xrm.Page.getAttribute("ownerid").getValue()[0].id.replace(/[{}]/g, "").toLowerCase() == userId.replace(/[{}]/g, "").toLowerCase()) {
            if ((Xrm.Page.getAttribute("tss_statusreason").getValue() == 865920000 || Xrm.Page.getAttribute("tss_statusreason").getValue() == 865920001 || Xrm.Page.getAttribute("tss_statusreason").getValue() == 865920003)) {
                return true;
            }
        }
    }
    return false;
}

function setRibbonSubmitToSAP() {
    var userId = Xrm.Page.context.getUserId();
    if (Xrm.Page.getAttribute("tss_statusreason").getValue() != null && Xrm.Page.getAttribute("tss_statecode").getValue() != null) {
        if (Xrm.Page.getAttribute("tss_statusreason").getValue() == 865920000 && Xrm.Page.getAttribute("tss_statecode").getValue() == 865920000 && Xrm.Page.getAttribute("ownerid").getValue()[0].id.replace(/[{}]/g, "").toLowerCase() == userId.replace(/[{}]/g, "").toLowerCase()) {
            return true;
        }
    }
    return false;
}

function setHidePackageSection() {
    var sourcetype = Xrm.Page.getAttribute("tss_sourcetype").getValue();
    if (sourcetype == 865920003 || sourcetype == 865920004) //dealer or counter, hide section package
    {
        Xrm.Page.ui.tabs.get("tab_6").setVisible(false);
    }
}


function setLookupValuePss() {
    var formType = Xrm.Page.ui.getFormType();

    if (formType == 1) {
        var sourcetype = Xrm.Page.getAttribute("tss_sourcetype").getValue();
        if (sourcetype == SourceTypes.Counter|| sourcetype == SourceTypes.Dealer) //dealer or counter, hide section package
        {

            var lookupOwner = Xrm.Page.data.entity.attributes.get("ownerid");


            if (lookupOwner != null) {
                var entityId = lookupOwner.getValue()[0].id;
                var entityType = lookupOwner.getValue()[0].entityType;
                var name = lookupOwner.getValue()[0].name;


                //set pss lookup
                var lookup = new Array();
                lookup[0] = new Object();
                lookup[0].id = entityId;
                lookup[0].name = name;
                lookup[0].entityType = entityType;
                Xrm.Page.getAttribute("tss_pss").setValue(lookup);

                Xrm.Page.getAttribute("tss_pss").fireOnChange();

            }
        }
    }
}

//empty quotation, hide pss
function hidePssByQuotation() {
    var quotation = Xrm.Page.getAttribute("tss_quotationlink").getValue();
    if (quotation == null) {
        Xrm.Page.getControl("tss_pss").setVisible(false);
    }
}

function setHideServiceAndSupportingMaterialSection() {
    var sourcetype = Xrm.Page.getAttribute("tss_sourcetype").getValue();
    if (sourcetype != SourceTypes.Service) //dealer or counter, hide section package
    {
        Xrm.Page.ui.tabs.get("tab_9").setVisible(false);
        Xrm.Page.ui.tabs.get("tab_10").setVisible(false);
    }
}

function setHideNewButton() {
    var flagCheckNew = false;
    var id = Xrm.Page.context.getUserId();

    XrmServiceToolkit.Rest.Retrieve(id, "SystemUserSet", "Title", null, function (result) {
        var title = result.Title;
        if (title.toLowerCase() == "counter part" || title.toLowerCase() == "dealer part officer") {
            flagCheckNew = true;
        }
    }, function (error) {
        Xrm.Utility.alertDialog(error.message);
    }, false);

    return flagCheckNew;
}

function setSourceType() {
    if (Xrm.Page.getAttribute("tss_sourcetype").getValue() == null) {
        var id = Xrm.Page.context.getUserId();

        XrmServiceToolkit.Rest.Retrieve(id, "SystemUserSet", "Title", null, function (result) {
            var title = result.Title;
            if (title.toLowerCase() == "counter part") {
                Xrm.Page.getAttribute("tss_sourcetype").setValue(865920004);
                Xrm.Page.getAttribute("tss_sourcetype").setSubmitMode("always");
            }
            else if (title.toLowerCase() == "dealer part officer") {
                Xrm.Page.getAttribute("tss_sourcetype").setValue(865920003);
                Xrm.Page.getAttribute("tss_sourcetype").setSubmitMode("always");
            }
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, false);
    }
}

function showSOFromQuotation() {
    //Unused
    //setVisibleField("tss_sofromquotation", true);
    //addCustomerFilterSalesOrderByCustomer();
    //Xrm.Page.getControl("tss_sofromquotation").setDisabled(false);
    //Xrm.Page.getControl("tss_sofromquotation").setFocus();

    setVisibleField("tss_sofromquotationlink", true);
    addCustomerFilterSalesOrderByCustomer();
    Xrm.Page.getControl("tss_sofromquotationlink").setDisabled(false);
    Xrm.Page.getControl("tss_sofromquotationlink").setFocus();
}

function showSOFromQuotationIfExist() {
    //Unused
    //if (Xrm.Page.getAttribute("tss_sofromquotation").getValue() != null) {
    //    setVisibleField("tss_sofromquotation", true);
    //}

    if (Xrm.Page.getAttribute("tss_sofromquotationlink").getValue() != null) {
        setVisibleField("tss_sofromquotationlink", true);
    }
}


function addCustomerFilterSalesOrderByCustomer() {
    
    if (Xrm.Page.getAttribute("tss_customer").getValue() == null) return;

    //20180901 - di comment karena xml diganti menjadi addpresearch
    //var customerId = Xrm.Page.getAttribute("tss_customer").getValue()[0].id;
    //var customerName = Xrm.Page.getAttribute("tss_customer").getValue()[0].name;
    //var viewId = "{6fd72744-3676-41d4-8003-ae4cde9ac282}";
    //var entityName = "tss_sopartheader";
    //var viewDisplayName = "SO";

    //var fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>" +
    //                    "<entity name='tss_sopartheader'>" +
    //                        "<attribute name='tss_sonumber' />" +
    //                        "<attribute name='tss_sopartheaderid' />" +
    //                        " <order attribute='tss_sonumber' descending='false' />" +
    //                        "<filter type='and'>" +
    //                            "<condition attribute='tss_customer' operator='eq' value='"+customerId+"' uitype='account' uiname='"+customerName+"'/>" +
    //                            "<condition attribute='statecode' operator='eq' value='0'  />" +
    //                            "<condition attribute='tss_statusreason' operator='ne' value='865920002'  />" +  //not equal close
    //                        "</filter>" +
    //                      "</entity>" +
    //                "</fetch>";

    //var layoutXml = "<grid name='resultset' " +
    //                "object='1' " +
    //                "jump='tss_sopartheaderid' " +
    //                "select='1' " +
    //                "icon='1' " +
    //                "preview='1'>" +
    //                "<row name='result' " +
    //                "id='tss_sopartheaderid'>" +
    //                "<cell name='tss_sonumber' " +
    //                "width='200' />" +
    //                "</row>" +
    //                "</grid>";

    //Xrm.Page.getControl("tss_sofromquotation").addCustomView(viewId, entityName, viewDisplayName, fetchXml, layoutXml, true);

    //20180901 - xml diganti menjadi addpresearch
    preFilterSOFromQuotation();
}

function preFilterSOFromQuotation() {
    var cust = Xrm.Page.getAttribute("tss_customer");
    var custObj = cust.getValue();

    Xrm.Page.getControl("tss_sofromquotationlink")._control && Xrm.Page.getControl("tss_sofromquotationlink")._control.tryCompleteOnDemandInitialization && Xrm.Page.getControl("tss_sofromquotationlink")._control.tryCompleteOnDemandInitialization();
    Xrm.Page.getControl("tss_sofromquotationlink").addPreSearch(function () {
        addLookupFilterSOFromQuotation();
    });
}

function addLookupFilterSOFromQuotation() {
    var cust = Xrm.Page.getAttribute("tss_customer");
    var custObj = cust.getValue();

    var statreason = Xrm.Page.getAttribute("tss_statusreason");
    
    if (custObj != null) {
        var fetchFilters = "<filter type='and'>" +
            "<condition attribute='tss_customer' uitype='" + custObj[0].entityType + "' operator='eq' value='" + custObj[0].id.replace("{", "").replace("}", "") + "'/>" +
            "<condition attribute='tss_statusreason' uitype='tss_statusreason' operator='ne' value='865920002'/></filter>";
        Xrm.Page.getControl("tss_sofromquotationlink").addCustomFilter(fetchFilters);
    }
}

function updateFieldsSOFromQuotationOnSave() {
    //Unused
    //if (Xrm.Page.getAttribute("tss_sofromquotation").getValue() != null && Xrm.Page.getAttribute("tss_statusreason").getValue() != 865920002
    //    && Xrm.Page.getAttribute("tss_statecode").getValue() != 865920002) {


    //    var today = new Date();

    //    var totalAmount = Xrm.Page.getAttribute("tss_totalamount").getValue();

    //    Xrm.Page.getAttribute("tss_canceldescription").setValue("Use SO from Quotation. SO number : " +
    //       Xrm.Page.getAttribute("tss_sofromquotation").getValue()[0].name.toString() + "");
    //    Xrm.Page.getAttribute("tss_closedate").setValue(today);
    //    Xrm.Page.getAttribute("tss_closeamount").setValue(totalAmount.toString());
    //    Xrm.Page.getAttribute("tss_statusreason").setValue(865920002);
    //    Xrm.Page.getAttribute("tss_statecode").setValue(865920002);

    //    Xrm.Page.getAttribute("tss_canceldescription").setSubmitMode("always");
    //    Xrm.Page.getAttribute("tss_closedate").setSubmitMode("always");
    //    Xrm.Page.getAttribute("tss_closeamount").setSubmitMode("always");
    //    Xrm.Page.getAttribute("tss_statusreason").setSubmitMode("always");
    //    Xrm.Page.getAttribute("tss_statecode").setSubmitMode("always");
    //    var id = Xrm.Page.data.entity.getId();
    //    Xrm.Page.data.save();
    //    Xrm.Page.data.refresh(true);
    //    window.location.reload(true);
    //    Xrm.Utility.openEntityForm("tss_sopartheader", Xrm.Page.data.entity.getId()); 
    //}

    //20180829 - Event diubah untuk menambahkan perubahan terhadap quotation link di SO
    //if (Xrm.Page.getAttribute("tss_sofromquotationlink").getValue() != null && Xrm.Page.getAttribute("tss_statusreason").getValue() != 865920002
    //    && Xrm.Page.getAttribute("tss_statecode").getValue() != 865920002) {
    //    var today = new Date();
    //    var totalAmount = Xrm.Page.getAttribute("tss_totalamount").getValue();

    //    Xrm.Page.getAttribute("tss_canceldescription").setValue("Use Quotation from PSS. Quotation number : " +
    //       Xrm.Page.getAttribute("tss_sofromquotationlink").getValue()[0].name.toString() + "");
    //    Xrm.Page.getAttribute("tss_closedate").setValue(today);
    //    Xrm.Page.getAttribute("tss_closeamount").setValue(totalAmount.toString());
    //    Xrm.Page.getAttribute("tss_statusreason").setValue(865920002);
    //    Xrm.Page.getAttribute("tss_statecode").setValue(865920002);

    //    Xrm.Page.getAttribute("tss_canceldescription").setSubmitMode("always");
    //    Xrm.Page.getAttribute("tss_closedate").setSubmitMode("always");
    //    Xrm.Page.getAttribute("tss_closeamount").setSubmitMode("always");
    //    Xrm.Page.getAttribute("tss_statusreason").setSubmitMode("always");
    //    Xrm.Page.getAttribute("tss_statecode").setSubmitMode("always");
    //    var id = Xrm.Page.data.entity.getId();
    //    Xrm.Page.data.save();
    //    Xrm.Page.data.refresh(true);
    //    window.location.reload(true);
    //    Xrm.Utility.openEntityForm("tss_sopartheader", Xrm.Page.data.entity.getId());
    //}

    if (Xrm.Page.getAttribute("tss_sofromquotationlink").getValue() != null && Xrm.Page.getAttribute("tss_statusreason").getValue() != 865920002
        && Xrm.Page.getAttribute("tss_statecode").getValue() != 865920002) {
        var today = new Date();
        var totalAmount = Xrm.Page.getAttribute("tss_totalamount").getValue();

        Xrm.Page.getAttribute("tss_canceldescription").setValue("Use Quotation from PSS. Quotation number : " +
           Xrm.Page.getAttribute("tss_sofromquotationlink").getValue()[0].name.toString() + "");
        Xrm.Page.getAttribute("tss_closedate").setValue(today);
        //Xrm.Page.getAttribute("tss_closeamount").setValue(totalAmount.toString()); //20180829 - ditambahkan pengecekan null, jika tidak akan menyebabkan SO tidak bisa di save jika tidak ada line.
        if (totalAmount != null) {
            Xrm.Page.getAttribute("tss_closeamount").setValue(totalAmount.toString());
        }
        else {
            Xrm.Page.getAttribute("tss_closeamount").setValue("0");
        }
        
        Xrm.Page.getAttribute("tss_statusreason").setValue(865920002);
        Xrm.Page.getAttribute("tss_statecode").setValue(865920002);

        Xrm.Page.getAttribute("tss_canceldescription").setSubmitMode("always");
        Xrm.Page.getAttribute("tss_closedate").setSubmitMode("always");
        Xrm.Page.getAttribute("tss_closeamount").setSubmitMode("always");
        //Xrm.Page.getAttribute("tss_statusreason").setSubmitMode("always");
        //Xrm.Page.getAttribute("tss_statecode").setSubmitMode("always");

        //var id = Xrm.Page.data.entity.getId();
        //Xrm.Page.data.save();
        //Xrm.Page.data.refresh(true);

        //20180830
        CopyHeaderAndDetailFromSO(Xrm.Page.getAttribute("tss_sofromquotationlink").getValue()[0].id.toString());

        //20180830 - di comment karena dipindahkan ke dalam function CopyHeaderAndDetailFromSO
        //var id = Xrm.Page.data.entity.getId();
        //Xrm.Page.data.save();
        //Xrm.Page.data.refresh(true);
        //window.location.reload(true);
        Xrm.Utility.openEntityForm("tss_sopartheader", Xrm.Page.data.entity.getId());
    }
}

function CopyHeaderAndDetailFromSO(idQuotationLink)
{
    console.log("Quotation Id " + idQuotationLink.replace("{", "").replace("}", ""));

    var idSalesOrder;

    XrmServiceToolkit.Rest.Retrieve(idQuotationLink.replace("{", "").replace("}", ""), "tss_quotationpartheaderSet", "tss_SalesOrderNo, tss_SalesOrderReference", null, function (result) {
        var tss_SalesOrderNo = result.tss_SalesOrderNo;
        var tss_SalesOrderReference = result.tss_SalesOrderReference;

        if (tss_SalesOrderNo.Id != undefined && tss_SalesOrderNo.Id != null) {
            idSalesOrder = tss_SalesOrderNo.Id;
        }
        else
        {
            idSalesOrder = tss_SalesOrderReference.Id;
        }

        console.log("ID Sales Order " + idSalesOrder);

        if (idSalesOrder != undefined && idSalesOrder != null) {
            try {
                var id = Xrm.Page.data.entity.getId();

                ClearSOLines(id);
                CreateDetailFromSO(idSalesOrder);
                UpdateHeaderFromSO(idSalesOrder);
                
            } catch (e) {
                Xrm.Utility.alertDialog(e.message);
            }
        }
        else
        {
            //var id = Xrm.Page.data.entity.getId();
            //Xrm.Page.data.save();
            //Xrm.Page.data.refresh(true);
            //window.location.reload(true);
            //Xrm.Utility.openEntityForm("tss_sopartheader", Xrm.Page.data.entity.getId());
        }
    }, function (error) {
        Xrm.Utility.alertDialog(error.message);
    }, true);
}

function UpdateHeaderFromSO(idSalesOrder)
{
    console.log("Header IdSO " + idSalesOrder.replace("{", "").replace("}", ""));

    XrmServiceToolkit.Rest.Retrieve(idSalesOrder, "tss_sopartheaderSet", "tss_ApproveAllSalesEffort,tss_ApproveCreditLimit,tss_ApproveCreditLimitBy,tss_Billto,tss_BillToContact,tss_Branch,tss_CancelDescription,tss_clcurrentapprover,tss_cldatetime,tss_CloseAmount,tss_CloseDate,tss_Contact,tss_Currency,tss_Customer,tss_DistributionChannel,tss_Division,tss_HaveContract,tss_IsAllowCancel,tss_OrderType,tss_Package,tss_PackageDescription,tss_PackageId,tss_PackageNo,tss_PackageQty,tss_PackagesName,tss_PackageUnit,tss_PaymentTerm,tss_plafondbelowsoamount,tss_PODate,tss_PONumber,tss_ProspectLink,tss_PSS,tss_QuotationLink,tss_RequestDeliveryDate,tss_SalesEffortCurrentApprover,tss_SalesEffortDateTime,tss_SalesOrderQuotation,tss_SalesOrganization,tss_SAPSOId,tss_SendEmailSalesEffort,tss_SendEmailSalesEffortMessage,tss_Shipto,tss_ShipToContact,tss_SOFromQuotation,tss_SOFromQuotationLink,tss_SOId,tss_Soldto,tss_SoldToContact,tss_sonumber,tss_sopartheaderId,tss_SourceType,tss_StateCode,tss_StatusReason,tss_StatusSalesEffort,tss_TOP,tss_TotalAmount,tss_totalamount_Base", null, function (result) {
        var tss_ApproveAllSalesEffort = result.tss_ApproveAllSalesEffort;
        var tss_ApproveCreditLimit = result.tss_ApproveCreditLimit;
        var tss_ApproveCreditLimitBy = result.tss_ApproveCreditLimitBy;
        var tss_Billto = result.tss_Billto;
        var tss_BillToContact = result.tss_BillToContact;
        var tss_Branch = result.tss_Branch;
        var tss_CancelDescription = result.tss_CancelDescription;
        var tss_clcurrentapprover = result.tss_clcurrentapprover;
        var tss_cldatetime = result.tss_cldatetime;
        var tss_CloseAmount = result.tss_CloseAmount;
        var tss_CloseDate = result.tss_CloseDate;
        var tss_Contact = result.tss_Contact;
        var tss_Currency = result.tss_Currency;
        var tss_Customer = result.tss_Customer;
        var tss_DistributionChannel = result.tss_DistributionChannel;
        var tss_Division = result.tss_Division;
        var tss_HaveContract = result.tss_HaveContract;
        var tss_IsAllowCancel = result.tss_IsAllowCancel;
        var tss_OrderType = result.tss_OrderType;
        var tss_Package = result.tss_Package;
        var tss_PackageDescription = result.tss_PackageDescription;
        var tss_PackageId = result.tss_PackageId;
        var tss_PackageNo = result.tss_PackageNo;
        var tss_PackageQty = result.tss_PackageQty;
        var tss_PackagesName = result.tss_PackagesName;
        var tss_PackageUnit = result.tss_PackageUnit;
        var tss_PaymentTerm = result.tss_PaymentTerm;
        var tss_plafondbelowsoamount = result.tss_plafondbelowsoamount;
        var tss_PODate = result.tss_PODate;
        var tss_PONumber = result.tss_PONumber;
        var tss_ProspectLink = result.tss_ProspectLink;
        var tss_PSS = result.tss_PSS;
        var tss_QuotationLink = result.tss_QuotationLink;
        var tss_RequestDeliveryDate = result.tss_RequestDeliveryDate;
        var tss_SalesEffortCurrentApprover = result.tss_SalesEffortCurrentApprover;
        var tss_SalesEffortDateTime = result.tss_SalesEffortDateTime;
        var tss_SalesOrderQuotation = result.tss_SalesOrderQuotation;
        var tss_SalesOrganization = result.tss_SalesOrganization;
        var tss_SAPSOId = result.tss_SAPSOId;
        var tss_SendEmailSalesEffort = result.tss_SendEmailSalesEffort;
        var tss_SendEmailSalesEffortMessage = result.tss_SendEmailSalesEffortMessage;
        var tss_Shipto = result.tss_Shipto;
        var tss_ShipToContact = result.tss_ShipToContact;
        var tss_SOFromQuotation = result.tss_SOFromQuotation;
        var tss_SOFromQuotationLink = result.tss_SOFromQuotationLink;
        var tss_SOId = result.tss_SOId;
        var tss_Soldto = result.tss_Soldto;
        var tss_SoldToContact = result.tss_SoldToContact;
        var tss_sonumber = result.tss_sonumber;
        var tss_sopartheaderId = result.tss_sopartheaderId;
        var tss_SourceType = result.tss_SourceType;
        var tss_StateCode = result.tss_StateCode;
        var tss_StatusReason = result.tss_StatusReason;
        var tss_StatusSalesEffort = result.tss_StatusSalesEffort;
        var tss_TOP = result.tss_TOP;
        var tss_TotalAmount = result.tss_TotalAmount;
        var tss_totalamount_Base = result.tss_totalamount_Base;

        var entity = {};

        //entity.tss_ApproveAllSalesEffort = tss_ApproveAllSalesEffort;
        //entity.tss_ApproveCreditLimit = tss_ApproveCreditLimit;
        //entity.tss_ApproveCreditLimitBy = { Id: tss_ApproveCreditLimitBy.Id, LogicalName: "systemuser" };
        //entity.tss_CancelDescription = tss_CancelDescription;
        //entity.tss_clcurrentapprover = { Id: tss_clcurrentapprover.Id, LogicalName: "tss_matrixapprovalcreditlimit" };
        //entity.tss_cldatetime = tss_cldatetime;
        //entity.tss_QuotationLink = { Id: tss_QuotationLink.Id, LogicalName: "tss_quotationpartheader" };
        //entity.tss_SalesEffortCurrentApprover = { Id: tss_SalesEffortCurrentApprover.Id, LogicalName: "tss_matrixapprovalsaleseffort" };
        //entity.tss_SalesEffortDateTime = tss_SalesEffortDateTime;
        //entity.tss_SendEmailSalesEffort = tss_SendEmailSalesEffort;
        //entity.tss_SendEmailSalesEffortMessage = tss_SendEmailSalesEffortMessage;
        //entity.tss_SOId = tss_SOId;
        //entity.tss_sonumber = tss_sonumber;
        //entity.tss_SourceType = { Value: tss_SourceType.Value };


        entity.tss_Billto = { Id: tss_Billto.Id, LogicalName: tss_Billto.LogicalName };
        entity.tss_BillToContact = { Id: tss_BillToContact.Id, LogicalName: tss_BillToContact.LogicalName };
        entity.tss_Branch = { Id: tss_Branch.Id, LogicalName: tss_Branch.LogicalName };
        entity.tss_CloseAmount = tss_CloseAmount;
        entity.tss_CloseDate = tss_CloseDate;
        entity.tss_Contact = { Id: tss_Contact.Id, LogicalName: tss_Contact.LogicalName };
        entity.tss_Currency = { Id: tss_Currency.Id, LogicalName: tss_Currency.LogicalName };
        entity.tss_Customer = { Id: tss_Customer.Id, LogicalName: tss_Customer.LogicalName };
        entity.tss_DistributionChannel = { Id: tss_DistributionChannel.Id, LogicalName: tss_DistributionChannel.LogicalName };
        entity.tss_Division = { Id: tss_Division.Id, LogicalName: tss_Division.LogicalName };
        entity.tss_HaveContract = tss_HaveContract;
        entity.tss_IsAllowCancel = tss_IsAllowCancel;
        entity.tss_OrderType = { Value: tss_OrderType.Value };
        entity.tss_Package = tss_Package;
        entity.tss_PackageDescription = tss_PackageDescription;
        entity.tss_PackageId = tss_PackageId;
        entity.tss_PackageNo = tss_PackageNo;
        entity.tss_PackageQty = tss_PackageQty;
        entity.tss_PackagesName = tss_PackagesName;
        entity.tss_PackageUnit = { Id: tss_PackageUnit.Id, LogicalName: tss_PackageUnit.LogicalName };
        entity.tss_PaymentTerm = { Id: tss_PaymentTerm.Id, LogicalName: tss_PaymentTerm.LogicalName };
        entity.tss_plafondbelowsoamount = { Value: tss_plafondbelowsoamount.Value };
        entity.tss_PODate = tss_PODate;
        entity.tss_PONumber = tss_PONumber;
        entity.tss_ProspectLink = { Id: tss_ProspectLink.Id, LogicalName: tss_ProspectLink.LogicalName };
        entity.tss_PSS = { Id: tss_PSS.Id, LogicalName: tss_PSS.LogicalName };
        entity.tss_RequestDeliveryDate = tss_RequestDeliveryDate;
        entity.tss_SalesOrderQuotation = { Id: tss_SalesOrderQuotation.Id, LogicalName: tss_SalesOrderQuotation.LogicalName };
        entity.tss_SalesOrganization = { Id: tss_SalesOrganization.Id, LogicalName: tss_SalesOrganization.LogicalName };
        entity.tss_SAPSOId = tss_SAPSOId;
        entity.tss_Shipto = { Id: tss_Shipto.Id, LogicalName: tss_Shipto.LogicalName };
        entity.tss_ShipToContact = { Id: tss_ShipToContact.Id, LogicalName: tss_ShipToContact.LogicalName };
        entity.tss_SOFromQuotation = { Id: tss_SOFromQuotation.Id, LogicalName: tss_SOFromQuotation.LogicalName };
        //entity.tss_SOFromQuotationLink = { Id: tss_SOFromQuotationLink.Id, LogicalName: tss_SOFromQuotationLink.LogicalName };
        entity.tss_Soldto = { Id: tss_Soldto.Id, LogicalName: tss_Soldto.LogicalName };
        entity.tss_SoldToContact = { Id: tss_SoldToContact.Id, LogicalName: tss_SoldToContact.LogicalName };
        //entity.tss_StateCode = { Value: tss_StateCode.Value };
        //entity.tss_StatusReason = { Value: tss_StatusReason.Value };
        entity.tss_StatusSalesEffort = { Value: tss_StatusSalesEffort.Value };
        entity.tss_TOP = { Value: tss_TOP.Value };
        entity.tss_TotalAmount = { Value: tss_TotalAmount.Value };

        XrmServiceToolkit.Rest.Update(Xrm.Page.data.entity.getId(), entity, "tss_sopartheaderSet", function () {
            //Success - No Return Data - Do Something
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, true);
    }, function (error) {
        Xrm.Utility.alertDialog(error.message);
    }, true);
}

function CreateDetailFromSO(idSalesOrder)
{
    console.log("Detail IdSO " + idSalesOrder.replace("{", "").replace("}", ""));

    XrmServiceToolkit.Rest.RetrieveMultiple("tss_sopartlinesSet", "?$select=tss_AmountInvoiceSAP,tss_amountinvoicesap_Base,tss_DiscountAmount,tss_discountamount_Base,tss_DiscountBy,tss_DiscountPercent,tss_Finalprice,tss_finalprice_Base,tss_IsInterchange,tss_ItemCategory,tss_itemnumber,tss_name,tss_PartDescription,tss_Partnumber,tss_PartNumberInterchange,tss_PriceAfterDiscount,tss_priceafterdiscount_Base,tss_PriceGroup,tss_pricetype,tss_QtyPartialNotSupply,tss_QtyRequest,tss_QtySupply,tss_Requestdeliverydate,tss_ReservationQty,tss_Salesman,tss_SOPartHeaderId,tss_sopartlinesId,tss_SourceType,tss_Status,tss_TotalPrice,tss_totalprice_Base,tss_Unit,tss_UnitGroup,ExchangeRate,ImportSequenceNumber,OverriddenCreatedOn,OwnerId,OwningBusinessUnit,OwningTeam,OwningUser,statecode,statuscode,TimeZoneRuleVersionNumber,TransactionCurrencyId,UTCConversionTimeZoneCode,VersionNumber&$filter=tss_SOPartHeaderId/Id eq (guid'" + idSalesOrder.replace("{", "").replace("}", "") + "')", function (results) {
        for (var i = 0; i < results.length; i++) {
            var exchangeRate = results[i].ExchangeRate;
            var importSequenceNumber = results[i].ImportSequenceNumber;
            var overriddenCreatedOn = results[i].OverriddenCreatedOn;
            var ownerId = results[i].OwnerId;
            var owningBusinessUnit = results[i].OwningBusinessUnit;
            var owningTeam = results[i].OwningTeam;
            var owningUser = results[i].OwningUser;
            var statecode = results[i].statecode;
            var statuscode = results[i].statuscode;
            var timeZoneRuleVersionNumber = results[i].TimeZoneRuleVersionNumber;
            var transactionCurrencyId = results[i].TransactionCurrencyId;
            var tss_sopartlinesId = results[i].tss_sopartlinesId;
            var uTCConversionTimeZoneCode = results[i].UTCConversionTimeZoneCode;
            var versionNumber = results[i].VersionNumber;

            var tss_AmountInvoiceSAP = results[i].tss_AmountInvoiceSAP;
            var tss_amountinvoicesap_Base = results[i].tss_amountinvoicesap_Base;
            var tss_DiscountAmount = results[i].tss_DiscountAmount;
            var tss_discountamount_Base = results[i].tss_discountamount_Base;
            var tss_DiscountBy = results[i].tss_DiscountBy;
            var tss_DiscountPercent = results[i].tss_DiscountPercent;
            var tss_Finalprice = results[i].tss_Finalprice;
            var tss_finalprice_Base = results[i].tss_finalprice_Base;
            var tss_IsInterchange = results[i].tss_IsInterchange;
            var tss_ItemCategory = results[i].tss_ItemCategory;
            var tss_itemnumber = results[i].tss_itemnumber;
            var tss_name = results[i].tss_name;
            var tss_PartDescription = results[i].tss_PartDescription;
            var tss_Partnumber = results[i].tss_Partnumber;
            var tss_PartNumberInterchange = results[i].tss_PartNumberInterchange;
            var tss_PriceAfterDiscount = results[i].tss_PriceAfterDiscount;
            var tss_priceafterdiscount_Base = results[i].tss_priceafterdiscount_Base;
            var tss_PriceGroup = results[i].tss_PriceGroup;
            var tss_pricetype = results[i].tss_pricetype;
            var tss_QtyPartialNotSupply = results[i].tss_QtyPartialNotSupply;
            var tss_QtyRequest = results[i].tss_QtyRequest;
            var tss_QtySupply = results[i].tss_QtySupply;
            var tss_Requestdeliverydate = results[i].tss_Requestdeliverydate;
            var tss_ReservationQty = results[i].tss_ReservationQty;
            var tss_Salesman = results[i].tss_Salesman;
            var tss_SOPartHeaderId = results[i].tss_SOPartHeaderId;
            var tss_sopartlinesId = results[i].tss_sopartlinesId;
            var tss_SourceType = results[i].tss_SourceType;
            var tss_Status = results[i].tss_Status;
            var tss_TotalPrice = results[i].tss_TotalPrice;
            var tss_totalprice_Base = results[i].tss_totalprice_Base;
            var tss_Unit = results[i].tss_Unit;
            var tss_UnitGroup = results[i].tss_UnitGroup;

            var entity = {};

            //entity.ExchangeRate = exchangeRate;
            //entity.ImportSequenceNumber = importSequenceNumber;
            //entity.OverriddenCreatedOn = overriddenCreatedOn;
            //entity.statuscode = { Value: statuscode.Value };
            //entity.TimeZoneRuleVersionNumber = timeZoneRuleVersionNumber;
            //entity.TransactionCurrencyId = { Id: transactionCurrencyId.Id, LogicalName: "transactioncurrency" };
            //entity.UTCConversionTimeZoneCode = uTCConversionTimeZoneCode;
            
            console.log("tss_DiscountAmount " + tss_DiscountAmount.Value);
            console.log("tss_DiscountPercent " + tss_DiscountPercent.Value);
            console.log("tss_Finalprice " + tss_Finalprice.Value);
            console.log("tss_PriceAfterDiscount " + tss_PriceAfterDiscount.Value);

            var totalprice = tss_PriceAfterDiscount * tss_QtyRequest;
            
            entity.tss_AmountInvoiceSAP = { Value: tss_AmountInvoiceSAP.Value };
            entity.tss_DiscountAmount = { Value: tss_DiscountAmount.Value };
            entity.tss_DiscountBy = tss_DiscountBy;
            entity.tss_DiscountPercent = tss_DiscountPercent;
            entity.tss_Finalprice = { Value: tss_Finalprice.Value };
            entity.tss_IsInterchange = tss_IsInterchange;
            entity.tss_ItemCategory = { Value: tss_ItemCategory.Value };
            entity.tss_itemnumber = tss_itemnumber;
            entity.tss_name = tss_name;
            entity.tss_PartDescription = tss_PartDescription;
            entity.tss_Partnumber = { Id: tss_Partnumber.Id, LogicalName: "trs_masterpart" };
            entity.tss_PartNumberInterchange = { Id: tss_PartNumberInterchange.Id, LogicalName: tss_PartNumberInterchange.LogicalName };
            entity.tss_PriceAfterDiscount = { Value: tss_PriceAfterDiscount.Value };
            entity.tss_PriceGroup = { Id: tss_PriceGroup.Id, LogicalName: tss_PriceGroup.LogicalName };
            entity.tss_pricetype = { Id: tss_pricetype.Id, LogicalName: tss_pricetype.LogicalName };
            entity.tss_QtyPartialNotSupply = tss_QtyPartialNotSupply;
            entity.tss_QtyRequest = tss_QtyRequest;
            entity.tss_QtySupply = tss_QtySupply;
            entity.tss_Requestdeliverydate = tss_Requestdeliverydate;
            entity.tss_ReservationQty = tss_ReservationQty;
            entity.tss_Salesman = { Id: Xrm.Page.context.getUserId(), LogicalName: tss_Salesman.LogicalName };
            entity.tss_SOPartHeaderId = { Id: Xrm.Page.data.entity.getId(), LogicalName: "tss_sopartheader" };
            //entity.tss_sopartlinesId = "123";
            entity.tss_SourceType = { Value: tss_SourceType.Value };
            entity.tss_Status = { Value: tss_Status.Value };
            entity.tss_TotalPrice = { Value: tss_TotalPrice.Value };
            entity.tss_Unit = { Id: tss_Unit.Id, LogicalName: "trs_unitofmeasurement" };
            entity.tss_UnitGroup = { Id: tss_UnitGroup.Id, LogicalName: "uomschedule" };

            XrmServiceToolkit.Rest.Create(entity, "tss_sopartlinesSet", function (result) {
                var newEntityId = result.tss_sopartlinesId;

                updateTotalPriceLines(newEntityId);
            }, function (error) {
                Xrm.Utility.alertDialog(error.message);
            }, true);
        }
    }, function (error) {
        Xrm.Utility.alertDialog(error.message);
    }, function () {
        //On Complete - Do Something
    }, true);
}

function ClearSOLines(idSalesOrder)
{
    console.log("Clear IdSO " + idSalesOrder.replace("{", "").replace("}", ""));

    XrmServiceToolkit.Rest.RetrieveMultiple("tss_sopartlinesSet", "?$select=tss_sopartlinesId&$filter=tss_SOPartHeaderId/Id eq (guid'" + idSalesOrder.replace("{", "").replace("}", "") + "')", function (results) {
        for (var i = 0; i < results.length; i++) {
            var tss_sopartlinesId = results[i].tss_sopartlinesId;

            XrmServiceToolkit.Rest.Delete(tss_sopartlinesId, "tss_sopartlinesSet", function () {
                //Success - No Return Data - Do Something
            }, function (error) {
                Xrm.Utility.alertDialog(error.message);
            }, true);
        }
    }, function (error) {
        Xrm.Utility.alertDialog(error.message);
    }, function () {
        //On Complete - Do Something
    }, true);
}

function updateTotalPriceLines(sopartlinesId) {
    XrmServiceToolkit.Rest.Retrieve(sopartlinesId, "tss_sopartlinesSet", "tss_TotalPrice,tss_PriceAfterDiscount,tss_QtyRequest", null, function (result) {
        var tss_TotalPrice = result.tss_TotalPrice;
        var tss_PriceAfterDiscount = result.tss_PriceAfterDiscount;
        var tss_QtyRequest = result.tss_QtyRequest;

        if (tss_PriceAfterDiscount.Value != null && tss_PriceAfterDiscount.Value != 0) {
            var totalprice = tss_PriceAfterDiscount.Value * tss_QtyRequest;

            var entity = {};
            entity.tss_TotalPrice = {
                Value: parseFloat(totalprice).toFixed(2)
            };

            XrmServiceToolkit.Rest.Update(sopartlinesId, entity, "tss_sopartlinesSet", function () {
                //Success - No Return Data - Do Something
            }, function (error) {
                Xrm.Utility.alertDialog(error.message);
            }, false);
        }
    }, function (error) {
        Xrm.Utility.alertDialog(error.message);
    }, false);
}

function alertPlafondBelowSOAmount(){
    if (Xrm.Page.getAttribute("tss_plafondbelowsoamount").getValue() == 865920001 && Xrm.Page.getAttribute("ownerid").getValue()[0].id.replace(/[{}]/g, "").toLowerCase() == Xrm.Page.context.getUserId().replace(/[{}]/g, "").toLowerCase()) {
            alert("ALERT PLAFOND BELOW SO AMOUNT")
    }
}


function setPlafindBelowSOAmount_CustomerChange() {
    if (Xrm.Page.getAttribute("tss_plafondbelowsoamount").getValue() == 865920001) {
        Xrm.Page.getAttribute("tss_plafondbelowsoamount").setValue() == 865920000;
    }
}