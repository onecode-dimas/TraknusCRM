//﻿//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////// Refresh Grid Quot. Part Lines - Discount / Package
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function CheckReasonOnRefreshGrid(gridElementId) {
    AddEventOnRefreshGrid(gridElementId, function () {

        if (Xrm.Page.data.entity.getId() != null) //20180828 - ditambahkan pengecekan terhadap Xrm.Page.data.entity.getId() apakah null atau tidak.
        {
            GetReasonRelatedData(Xrm.Page.data.entity.getId(), function (result) {
                var compareAttribute = function (attributeName, valueCheck) {
                    var attr = Xrm.Page.getAttribute(attributeName);
                    if (attr !== undefined && attr !== null) {
                        var value = attr.getValue();
                        if (value !== valueCheck) {
                            attr.setValue(valueCheck);
                            attr.setSubmitMode("always");
                            attr.fireOnChange();
                            Xrm.Page.data.setFormDirty(false);
                        }
                    }
                };
                compareAttribute("tss_result", result);
            });
        }

    });
}

function GetReasonRelatedData(quotationId, successCallback) {
    var value;
    XrmServiceToolkit.Rest.RetrieveMultiple(
        "tss_quotationpartreasondiscountpackageSet",
        "$select=*&$filter=tss_QuotationPartHeader/Id eq (guid'" + quotationId + "')",
        function (result) {
            if (result.length > 0) {
                for (var i = 0; i < result.length; i++) {
                    value = result[i].tss_Result;
                    successCallback(result);
                }
            }
        }, function (error) {
            alert('Retrieve Reason Discount / Package: ' + error.message);
        }, function onComplete() {
            //alert("DONE.");
        }, false
    );
    //    XrmServiceToolkit.Rest.Retrieve(
    //	quotationId,
    //		"tss_quotationpartreasondiscountpackageSet",
    //	null, null,
    //	function (result) {
    //        var result = results.tss_Result;
    //        successCallback(result);
    //	},function (error) {
    //	    console.log(error.message);
    //	},true);
}
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// ON LOAD Additional Refresh Grid
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
var STATUSCODE_ACTIVE = 865920003;
var STATUSCODE_CLOSED = 865920004;
var STATUSCODE_WON = 865920005;

var STATUSREASON_ACTIVE = 865920005;
var STATUSREASON_FINALAPPROVE = 865920006;
var STATUSREASON_LOST = 865920007;
var STATUSREASON_WON = 865920008;
var STATUSREASON_REVISED = 865920009;

function forceSubmitDirtyFields() {
    //var message = "The following fields are dirty: \n";
    Xrm.Page.data.entity.attributes.forEach(function (attribute, index) {
        if (attribute.getIsDirty() == true) {
            attribute.setSubmitMode('always');
            //message += "\u2219 " + attribute.getName() + "\n";
        }
    });
    //alert(message);

}
function retrieveQuotationPartLines() {
    var quotid = Xrm.Page.data.entity.getId();
    var id;
    var elem;
    XrmServiceToolkit.Rest.RetrieveMultiple(
        "tss_quotationpartlinesSet",
        "$select=*&$filter=tss_quotationpartheader/Id eq (guid'" + quotid + "')",
    function (result) {
        if (result.length > 0) {
            for (var i = 0; i < result.length; i++) {
                id = "gridBodyTable_gridDelBtn_" + i;
                elem = document.getElementById(id);

                if (elem != null) //20180828 - ditambahkan pengecekan terhadap elem, apakah null atau tidak.
                {
                    if (elem != null && elem.parentNode !== "undefined" && elem.parentNode != null)
                        elem.parentNode.removeChild(elem);
                }
            }
        }
    }, function (error) {
        alert(error.message);
    }, function onComplete() {
        //alert(" records should have been retrieved.");
    }, false);
}

function CheckQuotationTotalAmountOnRefreshGrid(gridElementId) {
    try {
        AddEventOnRefreshGrid(gridElementId, function () {
            GetQuotationTotalAmountRelatedData(Xrm.Page.data.entity.getId(), function (totalPrice, underMinPrice, totalfinal) {
                var compareAttribute = function (attributeName, valueCheck) {
                    var attr = Xrm.Page.getAttribute(attributeName);
                    if (attr !== undefined && attr !== null) {
                        var value = attr.getValue();
                        if (value !== valueCheck) {
                            attr.setValue(valueCheck);
                            attr.setSubmitMode("always");
                            attr.fireOnChange();
                            Xrm.Page.data.setFormDirty(false);
                        }
                    }
                };
                compareAttribute("tss_totalprice", Number(totalPrice));
                compareAttribute("tss_underminimumprice", underMinPrice);
                //compareAttribute("tss_totalfinalprice", Number(totalfinal));
                //compareAttribute("tss_top", top);
            });
        });
    }
    catch (e) {
        alert("CheckQuotationTotalAmountOnRefreshGrid: " + e.message);
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
        }
        else {
            setTimeout(function () {
                AddEventOnRefreshGrid(gridElementId, onRefreshFunction);
            }, 1000);
        }
    }
    catch (e) {
        alert("AddEventOnRefreshGrid: " + e.message);
    }
}

function GetQuotationTotalAmountRelatedData(quotationId, successCallback) {
    XrmServiceToolkit.Rest.Retrieve(
	quotationId,
		"tss_quotationpartheaderSet",
	null, null,

	function (result) {
	    var tss_TotalPrice = result.tss_totalprice.Value;
	    var tss_UnderMinimumPrice = result.tss_underminimumprice;
	    var tss_totalfinalprice = result.tss_totalfinalprice;
	    //var tss_top = result.tss_TOP.Value;
	    successCallback(tss_TotalPrice, tss_UnderMinimumPrice);
	},

	function (error) {
	    console.log(error.message);
	},
	true);
}
function CheckServiceSourceType() {
    var sourcetype_service = 865920002;
    var quotationpartGrid = 'QuotationPart';
    var tab_serviceGrid = 'tab_9';
    var tab_materialGrid = 'tab_10';
    var section_quotationGrid = 'tab_3_section_1';
    var sourcetype = Xrm.Page.getAttribute("tss_sourcetype").getValue();
    //Disable Quotation Subgrid
    //$('#' + quotationpartGrid + '_addImageButton').css('display', 'none');
    //$('#' + quotationpartGrid + '_openAssociatedGridViewImageButton').css('display', 'none');
    if (sourcetype == sourcetype_service) {
        Xrm.Page.ui.tabs.get(tab_serviceGrid).setVisible(true);
        Xrm.Page.ui.tabs.get(tab_materialGrid).setVisible(true);
    } else {
        Xrm.Page.ui.tabs.get(tab_serviceGrid).setVisible(false);
        Xrm.Page.ui.tabs.get(tab_materialGrid).setVisible(false);
    }

}

function doesControlHaveAttribute(control) {
    var controlType = control.getControlType();
    return controlType != "iframe" && controlType != "webresource" && controlType != "subgrid";
}
function disableFormFields(onOff) {
    Xrm.Page.ui.controls.forEach(function (control, index) {
        if (doesControlHaveAttribute(control)) {
            control.setDisabled(onOff);
        }
    });
}
var sourcetype_quoteservice = 865920002;
function setupForm() {
    if (Xrm.Page.getAttribute("tss_statusreason").getValue() == STATUSREASON_ACTIVE ||
            Xrm.Page.getAttribute("tss_statusreason").getValue() == STATUSREASON_FINALAPPROVE ||
            Xrm.Page.getAttribute("tss_statusreason").getValue() == STATUSREASON_LOST ||
            Xrm.Page.getAttribute("tss_statusreason").getValue() == STATUSREASON_WON ||
            Xrm.Page.getAttribute("tss_statusreason").getValue() == STATUSREASON_REVISED) {
        disableFormFields(true);
        //makeReadOnly();
    }
    if (Xrm.Page.getAttribute("tss_sourcetype").getValue() != sourcetype_quoteservice) {
        //makeReadOnly();
    } else {
        CheckQuotationTotalAmountOnRefreshGrid('QuotationPartLines');
        enableReqDeliveryDate();
    }
}
var intervalId = true;
function makeReadOnly() {
    var prospectlinesGrid = 'ProspectPartLines';
    var quotationpartGrid = 'QuotationPartLines';
    try {
        var subgridsLoaded = false;
        if ($("div[id$='" + quotationpartGrid + "']").length > 0 && !subgridsLoaded) {
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

function removeButtonsFromSubGrid(subgridControl) {
    if (intervalId) {
        $('#' + subgridControl.getName() + '_addImageButton').css('display', 'none');
        $('#' + subgridControl.getName() + '_openAssociatedGridViewImageButton').css('display', 'none');
        retrieveQuotationPartLines();
        clearInterval(intervalId);
    }
}
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// RIBBON BUTTON
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function ribbon_Active() {
    //alert('ribbon Active');
    var workflowId = '7165FE9B-9CF6-E711-940D-0050569355F1';
    var workflowName = 'Active Qutotation Part Header';

    //var requiredFields = ["tss_requestdeliverydate"];
    //var isAllRequiredFieldFilled = true;
    //for (var idx = 0; idx < requiredFields.length; idx++) {
    //    if (Xrm.Page.getAttribute(requiredFields[idx]).getValue() == null) isAllRequiredFieldFilled = false;
    //}
    //if (!isAllRequiredFieldFilled) {
    //    alert("Please fill all required fields.");
    //} else {
    ExecuteWorkflow(workflowId, workflowName, function () {
        RefreshForm();
        //disableRibbonActive();
    });
    //}

}


function setfocustoPSS() {
    Xrm.Page.getAttribute("tss_pss").setRequiredLevel("required");
    //RefreshForm();
    Xrm.Page.ui.controls.get("tss_pss").setFocus();
}

function updatePSS() {
    try {
        //var workflowId = 'A02E0BEB-855B-4DCA-8D97-4AAA11A51A01';
        var workflowId = 'A93E5676-4AA7-4AE0-8778-37BE67213829';
        var workflowName = 'Assign to PSS';
        ExecuteWorkflow(workflowId, workflowName, function () {
            RefreshForm();
            RefreshForm();
        });
    }
    catch (e) {
        alert('Exception on trigger workflow Assign to PSS' + e.message);
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
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// RIBBON BUTTON
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function set_newcustomer_field() {
    var isnew = Xrm.Page.getAttribute("tss_isnewcustomer");
    if (isnew.getValue()) {
        isnew.setValue(true);
        isnew.setSubmitMode('always');
    }
}
function onLoad() {
    try {
        //refresh ribbon
        refreshRibbonOnChange();

        var formType = Xrm.Page.ui.getFormType();
        var StatusReason = Xrm.Page.getAttribute("tss_statusreason").getValue();
        var StatusCode = Xrm.Page.getAttribute("tss_statuscode").getValue();
        var tab_printpreviewquotationGrid = 'tab_printpreviewquotation';

        CheckReasonOnRefreshGrid('QuotationPartReason');
    
        setOnLoad();
        setupForm();
    
        //disableButtonFinalQuotation();
        //disableButtonChangeTOP();
        CheckServiceSourceType();

        if ((StatusReason == 865920005 && StatusCode == 865920003) || (StatusReason == 865920006 && StatusCode == 865920002) || (StatusReason == 865920008 && StatusCode == 865920005)) { //Status Reason Active & Status Active Or Final Approved & Approved Or Won & Won
            Xrm.Page.ui.tabs.get(tab_printpreviewquotationGrid).setVisible(false);
            //Xrm.Page.ui.tabs.get(tab_printpreviewquotationGrid).setVisible(true);
            //onLoad_LoadSSRSReport();
        }
        else {
            Xrm.Page.ui.tabs.get(tab_printpreviewquotationGrid).setVisible(false);
        }
    
        //forceSubmitDirtyFields();
        //enableReqDeliveryDate();
        //preFilterLookupPSS();
        //hideGridReasonDiscount();
        //hideSectionPackage();
        //ConvertToPackage_Action();
        //hideUnit();
        //hideCloseDateDescription();
        //HideApproverList();
    } catch (e) {
        console.log(e.message);
    }
}

function HideApproverList() {
    var tab_approverlist = 'tab_11';
    Xrm.Page.ui.tabs.get(tab_approverlist).setVisible(false);
}

function hideCloseDateDescription() {
    var CLOSED = 865920004;
    var quoStatus = Xrm.Page.getAttribute("tss_statuscode").getValue();
    if (quoStatus == CLOSED) {
        Xrm.Page.ui.controls.get("tss_closedate").setVisible(true);
        Xrm.Page.ui.controls.get("tss_closedescription").setVisible(true);
    }
    else {
        Xrm.Page.ui.controls.get("tss_closedate").setVisible(false);
        Xrm.Page.ui.controls.get("tss_closedescription").setVisible(false);
    }

    //Xrm.Page.getControl("tss_closedate").setVisible(false);
    //Xrm.Page.getControl("tss_closedescription").setVisible(false);

    //if (Xrm.Page.getAttribute("tss_closedate").getValue() != null) {
    //    Xrm.Page.getControl("tss_closedate").setVisible(true);
    //}
    //if (Xrm.Page.getAttribute("tss_closedescription").getValue() != null) {
    //    Xrm.Page.getControl("tss_closedescription").setVisible(true);
    //}
}

function hideUnit() {
    Xrm.Page.getControl("tss_unit").setVisible(false);

    if (Xrm.Page.getAttribute("tss_unit").getValue() != null) {
        Xrm.Page.getControl("tss_unit").setVisible(true);
    }
}

function hideGridReasonDiscount() {
    var UMP = Xrm.Page.getAttribute("tss_underminimumprice").getValue();
    var requestConvertToPackage = Xrm.Page.getAttribute("tss_requestconverttopackage").getValue();
    var isPackage = Xrm.Page.getAttribute("tss_package").getValue();
    var tab_reasondiscount = 'tab_6';
    var tab_package = 'tab_8';

    var visibility = (UMP === true) || (requestConvertToPackage === true) || (isPackage === true);
    Xrm.Page.ui.tabs.get(tab_reasondiscount).setVisible(visibility);
}

function hideSectionPackage() {
    var isPackage = Xrm.Page.getAttribute("tss_package").getValue();
    var tab_package = 'tab_8';
    if (isPackage) {
        Xrm.Page.ui.tabs.get(tab_package).setVisible(true);
    } else {
        Xrm.Page.ui.tabs.get(tab_package).setVisible(false);
    }

}




function refreshRibbonOnChange() {
    Xrm.Page.ui.refreshRibbon();
}

function preFilterLookupPSS() {
    Xrm.Page.getControl("tss_pss")._control && Xrm.Page.getControl("tss_pss")._control.tryCompleteOnDemandInitialization && Xrm.Page.getControl("tss_pss")._control.tryCompleteOnDemandInitialization();
    Xrm.Page.getControl("tss_pss").addPreSearch(function () {
        addLookupFilterPSS();
    });
}

function addLookupFilterPSS() {
    var cust = Xrm.Page.getAttribute("tss_branch");
    var custObj = cust.getValue();
    if (custObj != null) { 	//<condition attribute='title' operator='eq' value='PSS'/>
        //"<filter type='and'><condition attribute='businessunitid' uitype='" + custObj[0].entityType + "' operator='eq' value='" + custObj[0].id + "'/><condition attribute='title' operator='eq' value='PSS'/></filter>";
        var fetchFilter = "<filter><condition attribute='title' operator='eq' value='PSS'/></filter>";
        Xrm.Page.getControl("tss_pss").addCustomFilter(fetchFilter);
    }
}

function enableReqDeliveryDate() {
    var DRAFT = 865920000;
    var OPEN = 865920000;
    var APPROVE_CODE = 865920002;
    var APPROVE_REASON = 865920004;
    var formType = Xrm.Page.ui.getFormType();
    var pss = Xrm.Page.getAttribute("tss_pss").getValue();
    var quotaionStatus = Xrm.Page.getAttribute("tss_statuscode").getValue();
    var quotaionReason = Xrm.Page.getAttribute("tss_statusreason").getValue();
    var SOURCE_SERVICE = 865920002;
    var sourceType = Xrm.Page.getAttribute("tss_sourcetype").getValue();

    if (sourceType == SOURCE_SERVICE) {
        Xrm.Page.ui.controls.get("tss_requestdeliverydate").setDisabled(false);
        Xrm.Page.getAttribute("tss_requestdeliverydate").setRequiredLevel('required');
    }
    else {
        if (formType > 1 && pss != null && ((quotaionStatus == DRAFT && quotaionReason == OPEN) || (quotaionStatus == APPROVE_CODE && quotaionReason == APPROVE_REASON))
            && Xrm.Page.getAttribute("ownerid").getValue()[0].id.replace(/[{}]/g, "").toLowerCase() == Xrm.Page.context.getUserId().replace(/[{}]/g, "").toLowerCase()
            ) {
            /*if (pss[0].id.replace(/[{}]/g, "").toLowerCase() == Xrm.Page.context.getUserId().replace(/[{}]/g, "").toLowerCase()) {
    
                Xrm.Page.ui.controls.get("tss_requestdeliverydate").setDisabled(false);
                Xrm.Page.getAttribute("tss_requestdeliverydate").setRequiredLevel('required');
            }
            else {
                Xrm.Page.getAttribute("tss_requestdeliverydate").setRequiredLevel('none');
                Xrm.Page.ui.controls.get("tss_requestdeliverydate").setDisabled(true);
            }*/
            //Xrm.Page.ui.controls.get("tss_requestdeliverydate").setFocus();

            Xrm.Page.ui.controls.get("tss_requestdeliverydate").setDisabled(false);
            Xrm.Page.getAttribute("tss_requestdeliverydate").setRequiredLevel('required');
        }
        else {
            Xrm.Page.getAttribute("tss_requestdeliverydate").setRequiredLevel('none');
            Xrm.Page.ui.controls.get("tss_requestdeliverydate").setDisabled(true);
        }
    }

}


function setOnLoad() {
    var formType = Xrm.Page.ui.getFormType();
    var prospect = Xrm.Page.getAttribute("tss_prospectlink").getValue();
    var quotservice = Xrm.Page.getAttribute("tss_quotationserviceno").getValue();
    if (formType < 2 && (prospect == null || quotservice == null)) {
        alert('Quotation Part can created on Prospect Part or Quotation Service');
        Xrm.Page.ui.close();
        //Set Default Value for Two Option Field
        //SetDefaultValue_TwoOptionField("tss_havecontract");
        //SetDefaultValue_TwoOptionField("tss_underminimumprice");
        //SetDefaultValue_TwoOptionField("tss_reviseprospect");
        //Xrm.Page.getAttribute("tss_totalamount").setValue(0);
    }
}

function clickChangeTOP() {
    var approve = confirm("Are you sure to change TOP ?");
    if (approve) {
        var guid = Xrm.Page.data.entity.getId();
        if (guid == null) {
            alert('Record not found!');
        }
        else {
            //alert(guid);
            var DialogGUID = "FB5E716D-5CE6-4BBC-88A0-1960338E8F1F";
            var dynamicServerUrl;
            if (Xrm.Page.context.getClientUrl !== undefined) {
                dynamicServerUrl = Xrm.Page.context.getClientUrl();
            } else {
                dynamicServerUrl = Xrm.Page.context.getServerUrl();
            }
            var serverUrl = dynamicServerUrl + "/cs/dialog/rundialog.aspx?DialogId=" + "{" + DialogGUID + "}" + "&EntityName=tss_quotationpartheader&ObjectId=" + guid;
            //alert( serverUrl);
            window.showModalDialog(serverUrl, null, "dialogHeight:540px;dialogWidth:720px;center:yes;resizable:1;maximize:1;minimize:1;status:no;scroll:no");
            window.location.reload(true);
        }
    }
}

function ConvertToPackage_OnSaveCheckup() {
    if (Xrm.Page.getAttribute("tss_requestconverttopackage").getValue() !== true) return; // if already initiated, continue the process.
    if (Xrm.Page.getAttribute("tss_package").getValue() === true) return; // no need convert, already package.
    //recheck first, there is case when you fill up the package then its already filled, weird.


    var arrayAttributeRequired = [
        "tss_packageno", "tss_packagesname", "tss_packagedescription",
        "tss_packageunit", "tss_packageqty", "tss_totalexpectedpackageamount"
    ];

    var isAllFilled = true;
    for (var index = 0; index < arrayAttributeRequired.length; index++) {
        if (Xrm.Page.getAttribute(arrayAttributeRequired[index]).getValue() == null) isAllFilled = false;
    }
    if (!isAllFilled) return;
    //if any data is not filled, then quit. Dont change anything.

    var workflowId = '459A6778-3C8C-4668-AD6D-859A9EB2F0D6';
    var workflowName = 'Send Email to Package Approval';
    ExecuteWorkflow(workflowId, workflowName, function () { Xrm.Utility.openEntityForm(Xrm.Page.data.entity.getEntityName(), Xrm.Page.data.entity.getId()); });

    //Change the status here before save. :)
    //Xrm.Page.getAttribute("tss_package").setValue(true);
    //Xrm.Page.getAttribute("tss_package").setSubmitMode("always");

    //var statusReasonWaitingApprovalPackage = 865920003;
    //Xrm.Page.getAttribute("tss_statusreason").setValue(statusReasonWaitingApprovalPackage);
    //Xrm.Page.getAttribute("tss_statusreason").setSubmitMode("always");

    //var quotationStatusInProgress = 865920001;
    //Xrm.Page.getAttribute("tss_statuscode").setValue(quotationStatusInProgress);
    //Xrm.Page.getAttribute("tss_statuscode").setSubmitMode("always");

    //Xrm.Page.getAttribute("tss_requestconverttopackage").setValue(false); //disable this, because the process is completed.
    //the rest is executed (send email,etc) on workflow. Check for process workflow real-time.
}

function ConvertToPackage_Action() {
    //Lock Package Field first.
    var arrayAttributeRequired = [
        "tss_packageno", "tss_packagesname", "tss_packagedescription",
        "tss_packageunit", "tss_packageqty", "tss_totalexpectedpackageamount"
    ];

    for (var index = 0; index < arrayAttributeRequired.length; index++) {
        try {
            Xrm.Page.getControl(arrayAttributeRequired[index]).setDisabled(true);
        } catch (e) {
            console.log("Error in convert to package, locking attribute - " +
                arrayAttributeRequired[index]);
            console.log(e);
        }
    }

    if (Xrm.Page.getAttribute("tss_requestconverttopackage").getValue() !== true) return; // if already initiated, continue the process.
    if (Xrm.Page.getAttribute("tss_package").getValue() === true) return; // no need convert, already package.

    var scrollToId = function (documentId) {
        var element = document.getElementById(documentId);
        element.scrollIntoView();
    }

    var elementIdPackage = "tab1";
    var elementIdDiscountReason = "tab6";
    var checkDiscountReason = function () {
        var returnResult = true;
        var entityid = Xrm.Page.data.entity.getId();

        if (entityid != null) {
            XrmServiceToolkit.Rest.RetrieveMultiple("tss_quotationpartreasondiscountpackageSet", "?$select=tss_quotationpartreasondiscountpackageId,tss_Result&$filter=tss_QuotationPartHeader/Id eq (guid'" + entityid + "')", function (results) {
                for (var i = 0; i < results.length; i++) {
                    var tss_Result = results[i].tss_Result;
                    if (tss_Result == null) {
                        returnResult = false;
                        break;
                    }
                }
            }, function (error) {
                Xrm.Utility.alertDialog(error.message);
            }, function () {
                //On Complete - Do Something
            }, false);
        }

        return returnResult;
    }

    var showPackageSection = function () {
        var tabPackage = "tab_8";
        Xrm.Page.ui.tabs.get(tabPackage).setVisible(true);
        scrollToId(elementIdPackage);

        var arrayAttributeRequired = [
            "tss_packageno", "tss_packagesname", "tss_packagedescription",
            "tss_packageunit", "tss_packageqty", "tss_totalexpectedpackageamount"
        ];

        for (var index = 0; index < arrayAttributeRequired.length; index++) {
            try {
                Xrm.Page.getAttribute(arrayAttributeRequired[index]).setRequiredLevel("required");
                Xrm.Page.getControl(arrayAttributeRequired[index]).setDisabled(false);
                Xrm.Page.getControl(arrayAttributeRequired[index]).setVisible(true);
            } catch (e) {
                console.log("Error in convert to package, setting required level attribute - " +
                    arrayAttributeRequired[index]);
                console.log(e);
            }
        }

        //set default quantity
        if (Xrm.Page.getAttribute("tss_packageqty").getValue() == null)
            Xrm.Page.getAttribute("tss_packageqty").setValue(1);
    }

    var flagDiscountReason = checkDiscountReason();

    //Xrm.Page.ui.setFormNotification("Request convert to package detected.", "INFO", "CONVERTTOPACKAGE");
    if (!flagDiscountReason) {
        //some of the discount reason is empty, need to focus on that
        Xrm.Page.ui.setFormNotification("Please complete the discount reason first to open package section.", "INFO", "DISCOUNTREASON");
        var tabReasondiscount = 'tab_6';
        Xrm.Page.ui.tabs.get(tabReasondiscount).setVisible(true);
        scrollToId(elementIdDiscountReason);

        //If there is any changes, check for changes in discount reason.
        var checkerHandler = function () {
            if (Xrm.Page.getAttribute("tss_requestconverttopackage").getValue() !== true) return;
            console.log("Checker Handler is called");
            var result = checkDiscountReason();
            if (result) {
                Xrm.Page.getControl("QuotationPartReason").removeOnLoad(this);
                showPackageSection();
                //Xrm.Page.ui.clearFormNotification("CONVERTTOPACKAGE");
                Xrm.Page.ui.clearFormNotification("DISCOUNTREASON");
            }
        }

        if (Xrm.Page.getControl("QuotationPartReason") != null) {
            Xrm.Page.getControl("QuotationPartReason").addOnLoad(checkerHandler);
        }
    } else {
        showPackageSection();
        //Xrm.Page.ui.setFormNotification("Please fill the required data for convert to package.", "INFO", "CONVERTPACKAGE");
        //later handle in plugin for checking and changing the status. or just in js :p (onsave)
    }
}

function ribbon_ConvertToPackage() {
    //alert('Ribbon TOP');
    var approve = confirm("Do you want to Convert To Package ?");
    if (approve) {

        Xrm.Page.getAttribute("tss_requestconverttopackage").setValue(true);
        Xrm.Page.getAttribute("tss_requestconverttopackage").setSubmitMode("always");

        ConvertToPackage_Action();

        //var guid = Xrm.Page.data.entity.getId();
        //if (guid == null) {
        //    alert('Record not found!');
        //}
        //else {
        //    //alert(guid);
        //    var DialogGUID = "17A79A86-8C31-41B4-AFF9-B183145FE8ED";
        //var dynamicServerUrl;
        //if (Xrm.Page.context.getClientUrl !== undefined) {
        //    dynamicServerUrl = Xrm.Page.context.getClientUrl();
        //} else {
        //    dynamicServerUrl = Xrm.Page.context.getServerUrl();
        //}
        //    var serverUrl = dynamicServerUrl+"/cs/dialog/rundialog.aspx?DialogId=" + "{" + DialogGUID + "}" + "&EntityName=tss_quotationpartheader&ObjectId=" + guid;
        //    //alert( serverUrl);
        //    window.showModalDialog(serverUrl, null, "dialogHeight:540px;dialogWidth:720px;center:yes;resizable:1;maximize:1;minimize:1;status:no;scroll:no");
        //    window.location.reload(true);
        //}
    }
}

function DisableEnable_RibbonActive() {

    var STATUS_CODE_APPROVED = 865920002;
    var STATUS_CODE_DRAFT = 865920000;
    var STATUS_REASON_APPROVED = 865920004;
    var STATUS_REASON_OPEN = 865920000;

    var statusCode = Xrm.Page.getAttribute("tss_statuscode").getValue();
    var statusReason = Xrm.Page.getAttribute("tss_statusreason").getValue();
    var underMinimumPrice = Xrm.Page.getAttribute("tss_underminimumprice").getValue();
    var approvedDiscount = Xrm.Page.getAttribute("tss_approvediscount").getValue();

    if ((statusCode == STATUS_CODE_DRAFT && statusReason == STATUS_REASON_OPEN) || (statusCode == STATUS_CODE_APPROVED && statusReason == STATUS_REASON_APPROVED)) {
        if (underMinimumPrice == false || (underMinimumPrice == true && approvedDiscount == true)) {
            return true;
        }
    }
    return false;
}

function disableButtonFinalQuotation() {
    /*var STATUS_CODE_ACTIVE = 865920003;
    var STATUS_REASON_ACTIVE = 865920005;

    var STATUS_CODE_APPROVED = 865920002;
    var STATUS_REASON_APPROVED = 865920004;

    var statusCode = Xrm.Page.getAttribute("tss_statuscode").getValue();
    var statusReason = Xrm.Page.getAttribute("tss_statusreason").getValue();
    var underMinimumPrice = Xrm.Page.getAttribute("tss_underminimumprice").getValue();
    var approvedDiscount = Xrm.Page.getAttribute("tss_approvediscount").getValue();

    if(underMinimumPrice == true){
        if(approvedDiscount == true && statusCode == STATUS_CODE_APPROVED && statusReason == STATUS_REASON_APPROVED){
            return true;
        }
        else{
            return false;
        }
    }
    else{
        if(statusCode == STATUS_CODE_ACTIVE && statusReason == STATUS_REASON_ACTIVE){
            return true;
        }
        else{
            return false;
        }
    }*/
}

function disableButtonCreateSo() {
    var STATUS_CODE_ACTIVE = 865920003;
    var STATUS_REASON_ACTIVE = 865920005;

    var STATUS_CODE_APPROVED = 865920002;
    var STATUS_REASON_FINAL_APPROVED = 865920006;

    var statusCode = Xrm.Page.getAttribute("tss_statuscode").getValue();
    var statusReason = Xrm.Page.getAttribute("tss_statusreason").getValue();
    var underMinimumPrice = Xrm.Page.getAttribute("tss_underminimumprice").getValue();
    var approvedDiscount = Xrm.Page.getAttribute("tss_approvediscount").getValue();

    if (underMinimumPrice == true) {
        if (approvedDiscount == true && statusCode == STATUS_CODE_APPROVED && statusReason == STATUS_REASON_FINAL_APPROVED) {
            return true;
        }
        else {
            return false;
        }
    }
    else {
        if (statusCode == STATUS_CODE_ACTIVE && statusReason == STATUS_REASON_ACTIVE) {
            return true;
        }
        else {
            return false;
        }
    }
}

function disableButtonChangeTOP() {
    var isnewCustomer = Xrm.Page.data.entity.attributes.get("tss_isnewcustomer").getValue();
    if (!isnewCustomer) {
        return false;
    }
    else {
        return true
    }
}

function ribbon_FinalQuotation() {
    var workflowId = '8880DFAD-BFE3-E711-940C-0050569355F1';
    var workflowName = 'Final Quotation';
    ExecuteWorkflow(workflowId, workflowName, function () {
        RefreshForm();
    });
}

function OpenNewSOPartHeader() {
    try {
        var soID;
        var quoID = Xrm.Page.data.entity.getId();
        var windowOptions = {
            openInNewWindow: true
        };
        XrmServiceToolkit.Rest.RetrieveMultiple(
			"tss_sopartheaderSet",
			"$select=tss_sopartheaderId&$top=1&$orderby=CreatedOn desc&$filter=tss_QuotationLink/Id eq (guid'" + quoID + "')",
		function (result) {
		    if (result.length > 0) {
		        soID = result[0].tss_sopartheaderId;
		    }
		}, function (error) {
		    alert(error.message);
		}, function onComplete() {
		    //alert("DONE.");
		}, false);
        if (soID != null && soID != 'undefined') Xrm.Utility.openEntityForm("tss_sopartheader", soID, null, windowOptions);
    }
    catch (e) {
        alert('Failed try to open Sales Order Part because ' + e.message);
    }
}

function ribbon_CreateSO() {
    var workflowId = '2a9683d8-1cee-4b62-ac3d-526ace8c0d31'; //'F802BD20-C6F7-4DF3-8DDE-FA6434DCAA9D';
    var workflowName = 'Create Sales Order';
    var quotid = Xrm.Page.data.entity.getId();
    var id;
    var elem;
    XrmServiceToolkit.Rest.RetrieveMultiple(
        "tss_quotationpartlinesSet",
        "$select=*&$filter=tss_quotationpartheader/Id eq (guid'" + quotid + "')",
    function (result) {
        if (result.length > 0) {
            ExecuteWorkflow(workflowId, workflowName, function () {
                OpenNewSOPartHeader();
                RefreshForm();
            });
        } else {
            alert('Cannot create Sales Order Part, please fill Sales Order Part Lines first.');
        }
    }, function (error) {
        alert(error.message);
    }, function onComplete() {
        //alert(" records should have been retrieved.");
    }, false);
    //new 2a9683d8-1cee-4b62-ac3d-526ace8c0d31

}

function ribbon_ReviseQuotation() {
    //alert('Ribbon ribbon_ReviseQuotation');
    var quotID = Xrm.Page.data.entity.getId();
    //alert(quotID);
    //var QuoNumber = Xrm.Page.getAttribute("tss_quotationnumber").getValue();
    if (quotID != null) {
        //alert('Ribbon ribbon_ReviseQuotation');
        var workflowId = 'B05556D7-D4E3-E711-940C-0050569355F1';
        var workflowName = 'ReviseQuotation';
        ExecuteWorkflowWithoutAlertSuccess(workflowId, workflowName, function () {
            OpenQuotationPartNew();
            //RefreshForm();
        });
    }
    else {
        alert('Record Cannot be Revised, Record not found!');
    }
}

function checkReviseProspect() {
    //Popup after Revise Prospect
    var quotID = Xrm.Page.data.entity.getId();
    var prosID = Xrm.Page.getAttribute("tss_prospectlink").getValue()[0].id;
    var statusreason;
    var statusCode;
    XrmServiceToolkit.Rest.Retrieve(
	quotID,
    "tss_quotationpartheaderSet",
	null, null,
    function (result) {
        statusreason = result.tss_statusreason.Value;
        //alert(statusreason);
        statusCode = result.tss_statuscode.Value;
        //alert(statusCode);
        if (statusreason == 865920009 && statusCode == 865920004) {
            var windowOptions = {
                openInNewWindow: true
            };
            if (!prosID != null && !prosID != undefined) {
                Xrm.Utility.openEntityForm("tss_prospectpartheader", prosID, null, false);
            }
        }
    }, function (error) {
        Xrm.Utility.alertDialog("ribbon_ReviseProspect Retrieve: " + error.message);
    }, false);
}

function ribbon_ReviseProspect() {
    var workflowId = 'ed0530d8-d854-4238-abad-4504b8cc2fa5';
    var workflowName = 'Revise Prospect';
    ExecuteWorkflow(workflowId, workflowName, function () {
        checkReviseProspect();
        //RefreshForm();
    });
}

function ribbon_CloseAsLost() {
    //A6AFED10-BDC3-4C13-B0DF-4D898BB44C13
    var close = confirm("Are you sure to Close As Lost ?");
    if (close) {
        var guid = Xrm.Page.data.entity.getId();
        var entityName = Xrm.Page.data.entity.getEntityName();
        if (guid == null) {
            alert('Record not found!');
        }
        else {
            var DialogGUID = "A6AFED10-BDC3-4C13-B0DF-4D898BB44C13"; //<-- Close As Lost
            var dynamicServerUrl;
            if (Xrm.Page.context.getClientUrl !== undefined) {
                dynamicServerUrl = Xrm.Page.context.getClientUrl();
            } else {
                dynamicServerUrl = Xrm.Page.context.getServerUrl();
            }
            var serverUrl = dynamicServerUrl + "/cs/dialog/rundialog.aspx?DialogId=" + "{" + DialogGUID + "}" + "&EntityName=" + entityName + "&ObjectId=" + guid;
            //alert( serverUrl);
            window.showModalDialog(serverUrl, null, "dialogHeight:540px;dialogWidth:720px;center:yes;resizable:1;maximize:1;minimize:1;status:no;scroll:no");
            window.location.reload(true);
        }
    }
}

function ribbon_ApproveTOP() {
    //if (CheckIsFormManager() == true)//if form Manager
    //{
    /*if(GetSystemUserManager()==true) //if system User Title is contain Manager
    {
        var workflowId = '1B9480C5-FEE6-E711-940C-0050569355F1';
        var workflowName = 'Approve Top';
        ExecuteWorkflow(workflowId, workflowName, function () {
            RefreshForm();
        });
    }
    else
    {
       alert('Only in User with Title Manager can Approve TOP...!!!'); 
    }*/

    var workflowId = '1B9480C5-FEE6-E711-940C-0050569355F1';
    var workflowName = 'Approve Top';
    ExecuteWorkflow(workflowId, workflowName, function () {
        RefreshForm();
    });

    //}
    //else {
    //    alert('Only in Form Manager,...this button can be used...!!!');
    //}


}

function ribbon_AssignToPSS() {
    //var pss = Xrm.Page.getAttribute("tss_pss").getValue();
    Xrm.Page.ui.controls.get("tss_pss").setDisabled(false);
    Xrm.Page.getAttribute("tss_pss").setRequiredLevel("required");
    Xrm.Page.ui.controls.get("tss_pss").setFocus();
}

function disableRibbonAssignToPSS() {
    var QuotationStatus = Xrm.Page.getAttribute("tss_statuscode").getValue();
    var SourceType = Xrm.Page.getAttribute("tss_sourcetype").getValue();
    //when quotation status draft,in progress, approved, and source type : service
    if (SourceType == 865920002 && QuotationStatus == 865920000 || QuotationStatus == 865920001 || QuotationStatus == 865920002) {
        var flag = false;
        var userid = Xrm.Page.context.getUserId();
        var entityid = Xrm.Page.data.entity.getId();
        if (userid != null && entityid != null) {
            XrmServiceToolkit.Rest.RetrieveMultiple("tss_approverlistSet", "?$select=tss_approverlistId&$filter=tss_Approver/Id eq (guid'" + userid + "') and tss_QuotationPartHeaderId/Id eq (guid'" + entityid + "')", function (results) {
                if (results.length > 0) {
                    flag = true;
                }
                else {
                    flag = false;
                }
            }, function (error) {
                Xrm.Utility.alertDialog(error.message);
            }, function () {
                //On Complete - Do Something
            }, false);
        }
        return flag;
    }
    else {
        return false;
    }
}

function ribbon_ApproveDiscount() {
    var workflowId = '39934C66-13E7-E711-940C-0050569355F1';
    var workflowName = 'Approve Discount Quotation';
    ExecuteWorkflow(workflowId, workflowName, function () {
        RefreshForm();
    });
}

function AlertAndClose() {
    var quoNumber = Xrm.Page.data.entity.attributes.get("tss_quotationnumber").getValue();
    var proLink = Xrm.Page.data.entity.attributes.get("tss_prospectlink").getValue();
    if (quoNumber == null || proLink == null) {
        alert('Cannot create Quotation Part Header manually!');
        Xrm.Page.ui.close();
    }
}

function ExecuteWorkflow(workflowId, workflowName, successCallback, failedCallback, silentMode) {
    if (silentMode === "undefined") silentMode = false;
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
            AssignResponse(req, workflowName, successCallback, failedCallback, silentMode);
        };
        req.send(request);
    }
}

function AssignResponse(req, workflowName, successCallback, failedCallback, silentMode) {
    if (req.readyState == 4) {
        if (req.status == 200) {
            if (!silentMode) alert('Successfully executed ' + workflowName + '.');
            if (successCallback !== undefined && typeof successCallback === "function") {
                successCallback();
            }
        }
        else {
            var faultstring = req.responseXML.getElementsByTagName("faultstring")[0].textContent;
            if (!silentMode) alert('Fail to execute ' + workflowName + '.\r\n Response Code : (' + req.status + ')' + "\r\nPlease contact your IT support.\r\nTechnical Details :" + faultstring);
            if (failedCallback !== undefined && typeof failedCallback === "function") {
                failedCallback(new Error(faultstring));
            }
        }
    }
}

function ExecuteWorkflowWithoutConfirmation(workflowId, workflowName, successCallback, failedCallback) {
    //var _return = window.confirm('Do you want to ' + workflowName + ' ?');
    //if (_return) {
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
    //}
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

function AssignResponseWithoutAlertSuccess(req, workflowName, successCallback, failedCallback) {
    if (req.readyState == 4) {
        if (req.status == 200) {
            //alert('Successfully executed ' + workflowName + '.');
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


function RefreshForm() {
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

function OpenQuotationPartNew() {
    try {
        var NewQuoId;
        var QuotationNumber = Xrm.Page.getAttribute("tss_quotationnumber").getValue();
        var windowOptions = {
            openInNewWindow: true
        };
        XrmServiceToolkit.Rest.RetrieveMultiple(
			"tss_quotationpartheaderSet",
			"$select=tss_quotationpartheaderId&$top=1&$orderby=CreatedOn desc&$filter=tss_quotationnumber eq ('" + QuotationNumber + "')",

		function (result) {
		    NewQuoId = result[0].tss_quotationpartheaderId;
		}, function (error) {
		    alert(error.message);
		}, function onComplete() {
		    //alert("DONE.");
		}, false);
        if (NewQuoId != null && NewQuoId != 'undefined') Xrm.Utility.openEntityForm("tss_quotationpartheader", NewQuoId, null, null);
    }
    catch (e) {
        alert('Failed try to open Quotation Part because ' + e.message);
    }
}

//function checkCustomerisnew() {
//    var customer = Xrm.Page.data.entity.attributes.get("tss_customer").getValue()[0];
//    var isnewcustomer = Xrm.Page.data.entity.attributes.get("tss_isnewcustomer").getValue();
//    var newcustomersp = Acc_isnewcustsp(customer.id);
//    if (newcustomersp == 1) {
//        if (isnewcustomer == null) {
//            //isnewcustomer.setValue(1);
//            Xrm.Page.data.entity.attributes.get("tss_isnewcustomer").setValue(true);
//        }
//        else if (isnewcustomer == 0) {
//            //isnewcustomer.setValue(1);
//            Xrm.Page.data.entity.attributes.get("tss_isnewcustomer").setValue(true);
//        }
//    }
//    else if (newcustomersp == 0) {
//        if (isnewcustomer == null) {
//            //isnewcustomer.setValue(0);
//            Xrm.Page.data.entity.attributes.get("tss_isnewcustomer").setValue(false);
//        }
//        else if (isnewcustomer == 1) {
//            //isnewcustomer.setValue(0);
//            Xrm.Page.data.entity.attributes.get("tss_isnewcustomer").setValue(false);
//        }
//    }
//    else {
//        //alert("value is null");
//    }
//}

function Acc_isnewcustsp(guid) {
    var bool;
    var re;
    XrmServiceToolkit.Rest.Retrieve(
	guid,
		"AccountSet",
	null, null,

	function (result) {
	    re = result;
	    //alert("success");
	    if (result.tss_newcustomersp == null) {
	        bool = 2;
	    }
	    else {
	        if (result.tss_newcustomersp.Value) {
	            bool = 1;
	        }
	        else {
	            bool = 0;
	        }
	    }
	},

	function (error) {
	    bool = 2;
	    //alert("failed");
	}, false);
    return bool;
}

function onLoad_LoadSSRSReport() {
    //First get the page type (Create, Update etc.)
    var pageType = Xrm.Page.ui.getFormType();
    //Then, only proceed if the Form Type DOES NOT equal create, can be changed depending on requirements. Full list of form types can be found here:
    //https://msdn.microsoft.com/en-us/library/gg327828.aspx#BKMK_getFormType
    if (pageType > 1) {
        //Get the value that you want to parameterise
        var guid = Xrm.Page.data.entity.getId();
        //remove braces in guid
        guid = guid.replace(/[{}]/g, "");
        //alert(guid);
        //Now, get the the name of the IFRAME we want to update
        var iFrame = Xrm.Page.ui.controls.get("IFRAME_ReportQuotationPart");
        //Then, specify the Report Server URL and Report Name.
        var reportURL;

        if (Xrm.Page.context.getClientUrl !== undefined) {
            reportURL = Xrm.Page.context.getClientUrl();
        } else {
            reportURL = Xrm.Page.context.getServerUrl();
        }

        reportURL = reportURL + "/crmreports/viewer/viewer.aspx?" + "action=run&helpID=TRAKNUS%20Report%20-%20Quotation%20Part.rdl&id=%7bE8548098-F9E5-E711-940C-0050569355F1%7d";
        reportURL = reportURL + "&p:QUOTATIONID=";
        //alert(reportURL);
        //Now combine the report url and parameter together into a full URL string
        var paramaterizedReportURL = reportURL + guid;
        //alert(paramaterizedReportURL);
        //Finally, if there is no value in the Account Name field, hide the IFRAME; otherwise, update the URL of the IFRAME accordingly.
        if (guid == null) {
            iFrame.setVisible(false);
        }
        else {
            iFrame.setSrc(paramaterizedReportURL);
        }
    }
    else {
        iFrame.setVisible(false);
    }
}

//approve top can use only in Form: Manager
function HiddenRibbonApproveTop() {
    var flag = true;

    var newRequestTop = Xrm.Page.getAttribute("tss_requestnewtop").getValue();
    var statusReason = Xrm.Page.getAttribute("tss_statusreason").getValue();

    var formLabel;
    var currForm = Xrm.Page.ui.formSelector.getCurrentItem();
    formLabel = currForm.getLabel();
    //var btnApproveTop = top.document.getElementById("tss_quotationpartheader|NoRelationship|Form|tss.tss_quotationpartheader.CmdApproveTOP");
    if (formLabel == "Manager") {
        flag = true;
        //return true;

        /*if(newRequestTop==865920000) //CBD
        {
           return false; 
        }*/
    }
    else {
        //return false;
        flag = false;
    }
    return flag;
}

function HiddenRibbonApproveTopNew() {
    var formLabel;
    var currForm = Xrm.Page.ui.formSelector.getCurrentItem();
    formLabel = currForm.getLabel();

    //if (formLabel == "Manager")

    var statusReason = Xrm.Page.getAttribute("tss_statusreason").getValue();
    if (statusReason == 865920001) ////waiting approval top
    {
        return true;
    }
    else {
        return false;
    }
}

/*function getUserTitle()
{
	flag = true;
	var UserID = Xrm.Page.context.getUserId();
	XrmServiceToolkit.Rest.Retrieve(
	UserID,
		"SystemUserSet",
	null, null,

	function (result)
	{
		if (result.Title == 'Manager')
		{
			flag = true;
		}
		else
		{
			flag = false;
		}
	}, function (error)
	{
		alert("Failed retrieve user Manager on setupForm");
	}, false);
	return flag;
}*/

function ribbon_CheckStock() {
    //alert('Ribbon ribbon_CheckStock');
    var workflowId = 'B8A4530D-151C-4FA7-A515-34811EF9649F';
    var workflowName = 'Check Stock';
    ExecuteWorkflow(workflowId, workflowName, function () {
        RefreshForm();
    });
}

function CheckIsFormManager() {
    var flag = true;
    var currForm = Xrm.Page.ui.formSelector.getCurrentItem();
    if (currForm != null) {
        formLabel = currForm.getLabel();
        if (formLabel == "Manager") {
            flag = true;
        }
        else {
            flag = false;
        }
    }
    return flag;
}

function GetSystemUserManager() {
    var flag = true;
    var UserID = Xrm.Page.context.getUserId();
    XrmServiceToolkit.Rest.Retrieve(
    UserID,
        "SystemUserSet",
    null, null,

    function (result) {
        //alert('FUllname: ' +result.FullName+ ' Title: '+result.Title);
        if (result.Title == 'Manager') {
            //alert('Your Manager'); 
            flag = true;
        }
        else {
            //alert('FUllname: ' +result.FullName+ ' Title: '+result.Title);
            flag = false;
        }
    }, function (error) {
        alert("Failed retrieve System User");
    }, false);

    return flag;

    /*var lookupObject = Xrm.Page.getAttribute("ownerid");
    if (lookupObject != null) {
        var lookUpObjectValue = lookupObject.getValue();
        if ((lookUpObjectValue != null)) {
            var lookupid = lookUpObjectValue[0].id;
        } 
    } 

    //var selectQuery = "/SystemUserSet?&$filter=SystemUserId eq guid'" + lookupid + "'&$select=JobTitle, FullName,Title";
    var selectQuery = "/SystemUserSet?&$filter=SystemUserId eq guid'" + lookupid + "' and substringof(Title,'Manager')&$select=JobTitle, FullName,Title";
    var oDataResult = null;
    oDataResult = MakeRequest(selectQuery);

    //alert("Name = " + oDataResult[0].FullName + "\n" + "Job Title = " + oDataResult[0].JobTitle+ "\n" + "Title = " + oDataResult[0].Title);
    if(oDataResult[0].Title != null)
    {
        //alert('Data Found');
        return true;
    }
    else
    {
        //alert('data not found..!!!');
        return false;
    }*/
}

//STRT//ODATA HELPER/////////////////////////////////////////
function MakeRequest(query) {
    var serverUrl = Xrm.Page.context.getClientUrl();
    var oDataEndpointUrl = serverUrl + "/XRMServices/2011/OrganizationData.svc/";
    oDataEndpointUrl += query;
    var service = GetRequestObject();
    if (service != null) {
        service.open("GET", oDataEndpointUrl, false);
        service.setRequestHeader("X-Requested-With", "XMLHttpRequest");
        service.setRequestHeader("Accept", "application/json, text/javascript, */*");
        service.send(null);
        var retrieved = JSON.parse(service.responseText).d;
        var results = new Array();
        for (var i = 0; i < retrieved.results.length; i++) {
            results.push(retrieved.results[i]);
        }
        return results;
    }
    return null;
}

function GetRequestObject() {
    if (window.XMLHttpRequest) {
        return new window.XMLHttpRequest;
    } else {
        try {
            return new ActiveXObject("MSXML2.XMLHTTP.3.0");
        } catch (ex) {
            return null;
        }
    }
}

//END//ODATA HELPER/////////////////////////////////////////

var SOURCETYPE_SERVICE = 865920002;
var STATUSCODE_DRAFT = 865920000;
var STATUSCODE_INPROGRESS = 865920001;
var SOURCETYPE_APPROVED = 865920002;
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// ON SAVE - SECTION
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function updateStatus(econtext) {
    var eventArgs = econtext.getEventArgs();
    var isPackage = Xrm.Page.getAttribute("tss_package").getValue();
    var requestNewTop = Xrm.Page.getAttribute("tss_requestnewtop").getValue();
    if ((eventArgs.getSaveMode() == 1 || eventArgs.getSaveMode() == 2) && requestNewTop && CheckTotalPaymentAmount() && CheckTotalAging()) {
        var id = Xrm.Page.data.entity.getId();
        var waitingTOP = 865920001;
        var entity = {};
        entity.tss_statusreason = { Value: waitingTOP };
        entity.tss_statuscode = { Value: STATUSCODE_INPROGRESS };
        XrmServiceToolkit.Rest.Update(
            id, entity, "tss_quotationpartheaderSet",
            function () {
                alert("Successfully update to waiting approval TOP.");
            },
            function (error) {
                alert("Failed to update TOP: " + error.message);
            }, false
        );
    }

}
function preventSave(econtext) {
    var id = Xrm.Page.data.entity.getId();
    var reason = false;
    var eventArgs = econtext.getEventArgs();
    var underminprice = Xrm.Page.getAttribute("tss_underminimumprice");
    var sourceType = Xrm.Page.getAttribute("tss_sourcetype").getValue();
    //XrmServiceToolkit.Rest.RetrieveMultiple(
    //	"tss_quotationpartreasondiscountpackageSet",
    //	"$select=*&$filter=tss_QuotationPartHeader/Id eq (guid'" + id + "')",
    //function (result) {
    //    if (result.length > 0) {
    //        for (var i = 0; i < result.length; i++) {
    //            if (result[i].tss_Result == null && result[i].tss_sourcetype != SOURCETYPE_SERVICE && underminprice.getValue()) {
    //                reason = true;
    //                break;
    //            }
    //        }
    //    }
    //}, function (error) {
    //    alert('Retrieve Quotation-Reason: ' + error.message);
    //}, function onComplete() {
    //    //alert(" records should have been retrieved.");
    //}, false);
    var pss = Xrm.Page.getAttribute("tss_pss").getValue();
    var sourcetype = Xrm.Page.getAttribute("tss_sourcetype").getValue();
    var statuscode = Xrm.Page.getAttribute("tss_statuscode").getValue();
    //disable all save (auto save,save, save and close)
    if (statuscode == STATUSCODE_DRAFT || statuscode == STATUSCODE_INPROGRESS || statuscode == SOURCETYPE_APPROVED) {
        //if (reason == true) {
        //    if (eventArgs.getSaveMode() == 1 || eventArgs.getSaveMode() == 2 || eventArgs.getSaveMode() == 59 || eventArgs.getSaveMode() == 80) {
        //        eventArgs.preventDefault();
        //        alert("Cannot save the record, You must be fulfill Discount Reason.");
        //    }
        //}
    } else {
        if (pss == null) {
            if (eventArgs.getSaveMode() == 1 || eventArgs.getSaveMode() == 2 || eventArgs.getSaveMode() == 59 || eventArgs.getSaveMode() == 80) {
                eventArgs.preventDefault();
                alert("Cannot save the record, PSS still not setup for this Quotation Part");
            }
        }
    }

    var topOptionSet = {
        CBD: 865920000,
        CAD: 865920001
    }

    if (Xrm.Page.getAttribute("tss_top").getValue() === topOptionSet.CAD
        || Xrm.Page.getAttribute("tss_requestnewtop").getValue() === topOptionSet.CAD) { // CAD
        //CHECK TOTAL PAYMENT PERCENT
        if (CheckTotalPaymentPercent() == false) {
            if (eventArgs.getSaveMode() == 1 || eventArgs.getSaveMode() == 2 || eventArgs.getSaveMode() == 59 || eventArgs.getSaveMode() == 80) {
                eventArgs.preventDefault();
                alert("Cannot save the record, Total Payment Percent must be Equal 100");
            }
        }

        //CHECK TOTAL PAYMENT AMOUNT
        if (CheckTotalPaymentAmount() == false) {
            if (eventArgs.getSaveMode() == 1 || eventArgs.getSaveMode() == 2 || eventArgs.getSaveMode() == 59 || eventArgs.getSaveMode() == 80) {
                eventArgs.preventDefault();
                //alert("Cannot save the record, Total Payment Amount must be Equal TOTAL PRICE or TOTAL FINAL PRICE");
                alert("Cannot save the record, Total Payment Amount must be Equal TOTAL PRICE");
            }
        }

        //CHECK TOTAL AGING
        if (CheckTotalAging() == false) {
            if (eventArgs.getSaveMode() == 1 || eventArgs.getSaveMode() == 2 || eventArgs.getSaveMode() == 59 || eventArgs.getSaveMode() == 80) {
                eventArgs.preventDefault();
                alert("Cannot save the record, Total aging must be equal than total days on Payment Term");
            }
        }
    }

    if (CheckTotalPaymentPercent() == true && CheckTotalPaymentAmount() == true && CheckTotalAging() == true) {
        if (Xrm.Page.getAttribute("tss_requestnewtop").getValue() != null) {
            setStatus();
        }
    }

    if (Xrm.Page.getAttribute("tss_requestconverttopackage").getValue() === true) {
        var saveEventMode = {
            Save: 1,
            SaveAndClose: 2,
            SaveAndNew: 59,
            AutoSave: 70
        }

        var currSaveMode = eventArgs.getSaveMode();

        if (currSaveMode === saveEventMode.Save ||
            currSaveMode === saveEventMode.SaveAndClose ||
            currSaveMode === saveEventMode.SaveAndNew) {
            ConvertToPackage_OnSaveCheckup();
        } else {
            eventArgs.preventDefault(); // Dont save other than that. (No autosave)
        }
    }

    if (checkDiscountReason() == false) {
        Xrm.Page.ui.setFormNotification("Please insert result in discount reason.", "INFO", "ONSAVE");
    }
}


function CheckTotalPaymentPercent() {
    var flag = false;

    var PERCENTAGE = false;
    var AMOUNT = true;
    var totalPercent;
    var selectPayment = Xrm.Page.getAttribute("tss_paymenttype").getValue();
    var downPaymentPercent = Xrm.Page.getAttribute("tss_dppercent").getValue();
    var payment2Percent = Xrm.Page.getAttribute("tss_payment2percent").getValue();
    var payment3Percent = Xrm.Page.getAttribute("tss_payment3percent").getValue();

    //if (selectPayment == PERCENTAGE) {
    if (downPaymentPercent != null || payment2Percent != null || payment3Percent != null) {
        totalPercent = (downPaymentPercent + payment2Percent + payment3Percent);
        if (totalPercent != 100) {
            //alert('Total Payment Percent: '+ totalPercent +' must be Equal 100');
            flag = false;
        }
        else {
            flag = true;
        }
    }
    //}

    return flag;
}

function onChange_PaymentPercent() {
    var amount1;
    var totalPrice = Xrm.Page.getAttribute("tss_totalprice").getValue();
    var downPaymentPercent = Xrm.Page.getAttribute("tss_dppercent").getValue();
    var downPaymentAmount = Xrm.Page.getAttribute("tss_dpamount");

    if (downPaymentPercent != null) {
        amount1 = Math.round(totalPrice * (downPaymentPercent / 100));
        downPaymentAmount.setValue(amount1);
        downPaymentAmount.setSubmitMode("always");

        /////ALERT -- ADDING BY AMIN///////////////////////////
        //var payment2Percent = Xrm.Page.getAttribute("tss_payment2percent").getValue();
        //var payment3Percent = Xrm.Page.getAttribute("tss_payment3percent").getValue();

        //if (downPaymentPercent != null || payment2Percent != null || payment3Percent != null) {
        //    totalPercent = (downPaymentPercent + payment2Percent + payment3Percent);
        //    if (totalPercent != 100) {
        //        alert('Total Payment Percent: ' + totalPercent + ' must be Equal 100');
        //    }
        //}
        //////////////////////////////////////////////////////////////
    }
}

function onChange_Payment2Percent() {
    var amount2;
    var totalPrice = Xrm.Page.getAttribute("tss_totalprice").getValue();
    var payment2Percent = Xrm.Page.getAttribute("tss_payment2percent").getValue();
    var payment2Amount = Xrm.Page.getAttribute("tss_payment2amount");

    if (payment2Percent != null) {
        amount2 = Math.round(totalPrice * (payment2Percent / 100));
        payment2Amount.setValue(amount2);
        payment2Amount.setSubmitMode("always");

        /////ALERT -- ADDING BY AMIN///////////////////////////
        //var downPaymentPercent = Xrm.Page.getAttribute("tss_dppercent").getValue();
        ////var payment2Percent = Xrm.Page.getAttribute("tss_payment2percent").getValue();
        //var payment3Percent = Xrm.Page.getAttribute("tss_payment3percent").getValue();

        //if (downPaymentPercent != null || payment2Percent != null || payment3Percent != null) {
        //    totalPercent = (downPaymentPercent + payment2Percent + payment3Percent);
        //    if (totalPercent != 100) {
        //        alert('Total Payment Percent: ' + totalPercent + ' must be Equal 100');
        //    }
        //}
        //////////////////////////////////////////////////////////////
    }
}

function onChange_Payment3Percent() {
    var amount3;
    var totalPrice = Xrm.Page.getAttribute("tss_totalprice").getValue();
    var payment3Percent = Xrm.Page.getAttribute("tss_payment3percent").getValue();
    var payment3Amount = Xrm.Page.getAttribute("tss_payment3amount");
    if (payment3Percent != null) {
        amount3 = Math.round(totalPrice * (payment3Percent / 100));
        payment3Amount.setValue(amount3);
        payment3Amount.setSubmitMode("always");

        /////ALERT -- ADDING BY AMIN///////////////////////////
        //var downPaymentPercent = Xrm.Page.getAttribute("tss_dppercent").getValue();
        //var payment2Percent = Xrm.Page.getAttribute("tss_payment2percent").getValue();
        ////var payment3Percent = Xrm.Page.getAttribute("tss_payment3percent").getValue();

        //if (downPaymentPercent != null || payment2Percent != null || payment3Percent != null) {
        //    totalPercent = (downPaymentPercent + payment2Percent + payment3Percent);
        //    if (totalPercent != 100) {
        //        alert('Total Payment Percent: ' + totalPercent + ' must be Equal 100');
        //    }
        //}
        //////////////////////////////////////////////////////////////
    }
}

function CheckTotalPaymentAmount() {
    var flag = false;

    var NO = false;
    var YES = true;
    var AMOUNT = true;
    var totalAmount;
    var isPackage = Xrm.Page.getAttribute("tss_package").getValue();
    var selectPayment = Xrm.Page.getAttribute("tss_paymenttype").getValue();
    var downPaymentAmount = Xrm.Page.getAttribute("tss_dpamount").getValue();
    var payment2Amount = Xrm.Page.getAttribute("tss_payment2amount").getValue();
    var payment3Amount = Xrm.Page.getAttribute("tss_payment3amount").getValue();
    var totalPrice = Xrm.Page.getAttribute("tss_totalprice").getValue();
    //var totalFinalPrice = Xrm.Page.getAttribute("tss_totalfinalprice").getValue();

    //if(isPackage == NO)
    //{
    //if (selectPayment == AMOUNT) 
    //{
    if (downPaymentAmount != null || payment2Amount != null || payment3Amount != null) {
        totalAmount = downPaymentAmount + payment2Amount + payment3Amount;

        if (totalAmount != totalPrice) {
            //alert('Total Payment Amount: '+ totalAmount +' must be Equal TOTAL PRICE: '+totalPrice);
            flag = false;
        }
        else {
            flag = true;
        }
    }
    //}
    //}
    /*else
    {
        if(selectPayment == AMOUNT)
        {
            if(downPaymentAmount != null || payment2Amount != null || payment3Amount != null)
            {
                totalAmount = downPaymentAmount + payment2Amount + payment3Amount;
                if(totalAmount != totalFinalPrice)
                {
                    //alert('Total Payment Amount: '+ totalAmount +' must be Equal TOTAL FINAL PRICE: '+totalFinalPrice);
                    flag = false;
                } 
                else
                {
                   flag = true; 
                }
            }
        } 
    }*/
    return flag;
}

function onChange_PaymentAmount() {
    var percent1;
    var downPaymentAmount = Xrm.Page.getAttribute("tss_dpamount").getValue();
    var totalPrice = Xrm.Page.getAttribute("tss_totalprice").getValue();

    var downPaymentPercent = Xrm.Page.getAttribute("tss_dppercent");

    if (downPaymentAmount != null) {
        percent1 = (downPaymentAmount / totalPrice) * 100;
        downPaymentPercent.setValue(percent1);
        downPaymentPercent.setSubmitMode("always");
    }

}

function onChange_Payment2Amount() {
    var percent2;
    var payment2Amount = Xrm.Page.getAttribute("tss_payment2amount").getValue();
    var totalPrice = Xrm.Page.getAttribute("tss_totalprice").getValue();
    var payment2Percent = Xrm.Page.getAttribute("tss_payment2percent");

    if (payment2Amount != null) {
        percent2 = (payment2Amount / totalPrice) * 100;
        payment2Percent.setValue(percent2);
        payment2Percent.setSubmitMode("always");
    }

}

function onChange_Payment3Amount() {
    var percent3;
    var payment3Amount = Xrm.Page.getAttribute("tss_payment3amount").getValue();
    var totalPrice = Xrm.Page.getAttribute("tss_totalprice").getValue();
    var payment3Percent = Xrm.Page.getAttribute("tss_payment3percent");

    if (payment3Amount != null) {
        percent3 = (payment3Amount / totalPrice) * 100;
        payment3Percent.setValue(percent3);
        payment3Percent.setSubmitMode("always");
    }

}


function CheckTotalAging() {
    try {
        var flag = false;

        var re;
        var paymentTerm = Xrm.Page.getAttribute("tss_paymentterm").getValue();
        var aging2 = Xrm.Page.getAttribute("tss_aging2").getValue();
        var aging3 = Xrm.Page.getAttribute("tss_aging3").getValue();
        var totalAging;
        var totalDays;

        if (paymentTerm != null) {
            var lookUpObjectValue = paymentTerm;
            if ((lookUpObjectValue != null)) {
                var lookupid = lookUpObjectValue[0].id;
                var lookupname = lookUpObjectValue[0].name;
            }

            XrmServiceToolkit.Rest.Retrieve(
            lookupid,
                "new_paymenttermSet",
            null, null,
            function (result) {
                //SourceType = result[0].trs_Product;
                //alert('success');
                re = result;
            },
            function (error) {
                alert(error.message);
            },
            false);

            totalDays = re.tss_Days;
            if (totalDays != null) {
                if (aging2 != null || aging3 != null) {
                    totalAging = aging2 + aging3;
                    if (totalAging != totalDays) {
                        //alert('Total Aging: '+ totalAging +' must be Equal TOTAL DAYS in Payment Term: '+totalDays); 
                        flag = false;
                    }
                    else {
                        flag = true;
                    }
                }
            }
            else {
                flag = false;
            }
        }

        return flag;
    }
    catch (ex) {
        alert('onChange_Aging: ' + ex.message);
    }
}

function onChange_Aging() {
    try {
        var re;
        var paymentTerm = Xrm.Page.getAttribute("tss_paymentterm").getValue();
        var aging2 = Xrm.Page.getAttribute("tss_aging2").getValue();
        var aging3 = Xrm.Page.getAttribute("tss_aging3").getValue();
        var totalAging;
        var totalDays;

        if (paymentTerm != null) {
            var lookUpObjectValue = paymentTerm;
            if ((lookUpObjectValue != null)) {
                var lookupid = lookUpObjectValue[0].id;
                var lookupname = lookUpObjectValue[0].name;
            }

            XrmServiceToolkit.Rest.Retrieve(
            lookupid,
                "new_paymenttermSet",
            null, null,
            function (result) {
                //SourceType = result[0].trs_Product;
                //alert('success');
                re = result;
            },
            function (error) {
                alert(error.message);
            },
            false);

            totalDays = re.tss_Days;
            if (totalDays != null) {
                if (aging2 != null || aging3 != null) {
                    totalAging = aging2 + aging3;
                    if (totalAging != totalDays) {
                        alert('Total Aging: ' + totalAging + ' must be Equal TOTAL DAYS in Payment Term: ' + totalDays);
                    }
                }
            }
            else {
                alert('Error,..Payment Term: ' + lookupname + ' does not have Total Days');
            }
        }
        else {
            alert('Error,..Please Select Payment Term first..!!!');
        }
    }
    catch (ex) {
        alert('onChange_Aging: ' + ex.message);
    }
}

function CheckUserPrivilegeApprove() {
    //    try {
    var flag = false;

    var quotationId = Xrm.Page.data.entity.getId();
    var UserID = Xrm.Page.context.getUserId();
    XrmServiceToolkit.Rest.RetrieveMultiple(
    "tss_approverlistSet",
    "$select=*&$filter=tss_QuotationPartHeaderId/Id eq (guid'" + quotationId + "') and tss_Approver/Id eq (guid'" + UserID + "')",
    function (results) {
        if (results.length == 0) {
            flag = false;
        }
        else {
            flag = true;
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

    if (flag == true) {
        return true;
    }
    else {
        return false;
    }
    //return flag;
    //    }
    //    catch (ex) {
    //        alert('Check User Privilege Approve ' + ex.message)
    //    }

}


function EnableDisableRibbon_ApproveDiscount() {
    var QUOTATION_STATUS_IN_PROGRESS = 865920001; //tss_statuscode
    var STATUS_REASON_WAITING_APPROVAL_DISCOUNT = 865920002; //tss_statusreason
    var UNDER_MINIMUM_PRICE = true;

    var quotationStatus = Xrm.Page.getAttribute("tss_statuscode").getValue();
    var statusReason = Xrm.Page.getAttribute("tss_statusreason").getValue();
    var underMinimumPrice = Xrm.Page.getAttribute("tss_underminimumprice").getValue();

    if (CheckUserPrivilegeApprove() == true && quotationStatus == QUOTATION_STATUS_IN_PROGRESS && statusReason == STATUS_REASON_WAITING_APPROVAL_DISCOUNT) {
        return true;
    }
    else {
        return false;
    }
}

function EnableDisableRibbonCheck_ApproveTop() {
    var QUOTATION_STATUS_IN_PROGRESS = 865920001;
    var STATUS_REASON_WAITING_APPROVAL_TOP = 865920001;

    var quotationStatus = Xrm.Page.getAttribute("tss_statuscode").getValue();
    var statusReason = Xrm.Page.getAttribute("tss_statusreason").getValue();
    var requestNewTop = Xrm.Page.getAttribute("tss_requestnewtop").getValue();
    if (CheckUserPrivilegeApprove() == true && quotationStatus == QUOTATION_STATUS_IN_PROGRESS && statusReason == STATUS_REASON_WAITING_APPROVAL_TOP) {
        return true;
    }
    else {
        return false;
    }
}

function setSalesOrder() {
    Xrm.Page.getControl("tss_salesorderno").setDisabled(false);
    Xrm.Page.getControl("tss_salesorderno").setVisible(true);
    Xrm.Page.getAttribute("tss_salesorderno").setRequiredLevel('required');
    preFilterLookupSalesOrder();
}

function addSOFilter() {
    var viewId = "{6fd72744-3676-41d4-8003-ae4cde9ac282}";
    var entityName = "tss_sopartheader";
    var viewDisplayName = "SO";

    var fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>" +
                        "<entity name='tss_sopartheader'>" +
                            "<attribute name='tss_sonumber' />" +
                            "<attribute name='tss_sopartheaderid' />" +
                            " <order attribute='tss_sonumber' descending='false' />" +
                            "<filter type='and'>" +
                                "<condition attribute='tss_statusreason' operator='ne' value='865920002'  />" +
                                "<condition attribute='statecode' operator='eq' value='0'  />" +
                                "<condition attribute='tss_sourcetype' operator='eq' value='865920004'  />" +
                            "</filter>" +
                          "</entity>" +
                    "</fetch>";

    var layoutXml = "<grid name='resultset' " +
                    "object='1' " +
                    "jump='tss_sopartheaderid' " +
                    "select='1' " +
                    "icon='1' " +
                    "preview='1'>" +
                    "<row name='result' " +
                    "id='tss_sopartheaderid'>" +
                    "<cell name='tss_sonumber' " +
                    "width='200' />" +
                    "</row>" +
                    "</grid>";

    Xrm.Page.getControl("tss_salesorderno").addCustomView(viewId, entityName, viewDisplayName, fetchXml, layoutXml, true);
}


function setStatus() {
    if (Xrm.Page.getAttribute("tss_paymenttype").getValue() != null) {
        var paymenttype = Xrm.Page.getAttribute("tss_paymenttype").getValue();

        //percentage
        //if ((paymenttype == 0
        //    && Xrm.Page.getAttribute("tss_dppercent").getValue() != null
        //    && Xrm.Page.getAttribute("tss_payment2percent").getValue() != null
        //    && Xrm.Page.getAttribute("tss_payment3percent").getValue() != null
        //    && Xrm.Page.getAttribute("tss_aging2").getValue() != null
        //    && Xrm.Page.getAttribute("tss_aging3").getValue() != null) ||
        //    (paymenttype == 1
        //    && Xrm.Page.getAttribute("tss_dpamount").getValue() != null
        //    && Xrm.Page.getAttribute("tss_payment2amount").getValue() != null
        //    && Xrm.Page.getAttribute("tss_payment3amount").getValue() != null
        //    && Xrm.Page.getAttribute("tss_aging2").getValue() != null
        //    && Xrm.Page.getAttribute("tss_aging3").getValue() != null)) {

        Xrm.Page.getAttribute("tss_statuscode").setValue(865920001);
        Xrm.Page.getAttribute("tss_statuscode").setSubmitMode("always");
        Xrm.Page.getAttribute("tss_statuscode").fireOnChange();
        Xrm.Page.getAttribute("tss_statusreason").setValue(865920001);
        Xrm.Page.getAttribute("tss_statusreason").setSubmitMode("always");
        Xrm.Page.getAttribute("tss_statusreason").fireOnChange();

        Xrm.Page.getAttribute("tss_dppercent").setSubmitMode("always");
        Xrm.Page.getAttribute("tss_dppercent").fireOnChange();
        Xrm.Page.getAttribute("tss_payment2percent").setSubmitMode("always");
        Xrm.Page.getAttribute("tss_payment2percent").fireOnChange();
        Xrm.Page.getAttribute("tss_payment3percent").setSubmitMode("always");
        Xrm.Page.getAttribute("tss_payment3percent").fireOnChange();
        Xrm.Page.getAttribute("tss_dpamount").setSubmitMode("always");
        Xrm.Page.getAttribute("tss_dpamount").fireOnChange();
        Xrm.Page.getAttribute("tss_payment2amount").setSubmitMode("always");
        Xrm.Page.getAttribute("tss_payment2amount").fireOnChange();
        Xrm.Page.getAttribute("tss_payment3amount").setSubmitMode("always");
        Xrm.Page.getAttribute("tss_payment3amount").fireOnChange();
        Xrm.Page.getAttribute("tss_aging2").setSubmitMode("always");
        Xrm.Page.getAttribute("tss_aging2").fireOnChange();
        Xrm.Page.getAttribute("tss_aging3").setSubmitMode("always");
        Xrm.Page.getAttribute("tss_aging3").fireOnChange();

        //var workflowId = '81b4bffb-385a-48d2-b312-c57605f7a21b';
        //var workflowName = 'Send Email to TOP Approval';
        //ExecuteWorkflowWithoutConfirmation(workflowId, workflowName, function () {
        //    RefreshForm();
        //},true);
        //}
    }
}


function checkCurrentUserPSS() {
    if (Xrm.Page.getAttribute("tss_pss").getValue() != null) {
        var pss = Xrm.Page.getAttribute("tss_pss").getValue();
        if (pss[0].id.replace(/[{}]/g, "").toLowerCase() == Xrm.Page.context.getUserId().replace(/[{}]/g, "").toLowerCase()) {
            return true;
        }
        return false;
    }
    return false;
}

function checkPSSOnChange() {
    if (Xrm.Page.getAttribute("tss_pss").getValue() != null) {
        Xrm.Page.getControl("tss_pss").setDisabled(true);
    }
}

function convertToPackage() {
    var statuscode = Xrm.Page.getAttribute("tss_statuscode").getValue();
    var statusreason = Xrm.Page.getAttribute("tss_statusreason").getValue();

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //// 20180823
    //if ((Xrm.Page.getAttribute("tss_package").getValue() == false && Xrm.Page.getAttribute("tss_underminimumprice").getValue() == false && ((statuscode == 865920000 && statusreason == 865920000) || (statuscode == 865920002 && statusreason == 865920004)))
    //    || (Xrm.Page.getAttribute("tss_approvediscount").getValue() == true && Xrm.Page.getAttribute("tss_underminimumprice").getValue() == true && statuscode == 865920000 && statusreason == 865920000)) {
    //    return true;
    //
    //return false;
    //}
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    if ((Xrm.Page.getAttribute("tss_package").getValue() == false && ((statuscode == 865920000 && statusreason == 865920000) || (statuscode == 865920002 && statusreason == 865920004)))
        || (Xrm.Page.getAttribute("tss_approvediscount").getValue() == true && Xrm.Page.getAttribute("tss_underminimumprice").getValue() == true && statuscode == 865920002 && statusreason == 865920004)) {
        return true;
    }

    return false;

    //var flagConvert = true;

    //return flagConvert && (Xrm.Page.getAttribute("tss_requestconverttopackage").getValue() !== true);
    //var entityid = Xrm.Page.data.entity.getId();
    //XrmServiceToolkit.Rest.RetrieveMultiple("tss_quotationpartreasondiscountpackageSet", "?$select=tss_quotationpartreasondiscountpackageId,tss_Result&$filter=tss_QuotationPartHeader/Id eq (guid'" + entityid + "')", function (results) {
    //    for (var i = 0; i < results.length; i++) {
    //        var tss_Result = results[i].tss_Result;
    //        if (tss_Result == null) {
    //            flagConvert = false;
    //            break;
    //        }
    //        }
    //}, function (error) {
    //    Xrm.Utility.alertDialog(error.message);
    //}, function () {
    //    //On Complete - Do Something
    //}, false);

    //return flagConvert;
}

function checkApproveNewTOP() {
    if (Xrm.Page.getAttribute("tss_approvetopby").getValue() != null) {
        var arr = ["tss_paymentterm",
            "tss_paymenttype",
            "tss_dppercent",
            "tss_payment2percent",
            "tss_payment3percent",
            "tss_dpamount",
            "tss_payment2amount",
            "tss_payment3amount",
            "tss_approvenewtop",
            "tss_aging2",
            "tss_aging3"];
        for (var i = 0; i < arr.length; i++) {
            try {
                Xrm.Page.getControl(arr[i]).setDisabled(true);
            } catch (e) {
                console.log(e);
            }
        }
    }
}

function setPaymentTerm() {
    Xrm.Page.getControl("tss_paymentterm").setVisible(false);
    if (Xrm.Page.getAttribute("tss_top").getValue() == 865920001 && Xrm.Page.getAttribute("tss_isnewcustomer").getValue() == false) {
        Xrm.Page.getControl("tss_paymentterm").setVisible(true);
    }
}

function checkDiscountReason() {
    var flagCheck = true;

    var underMinimumPrice = Xrm.Page.getAttribute("tss_underminimumprice").getValue();
    var approvedDiscount = Xrm.Page.getAttribute("tss_approvediscount").getValue();

    if (Xrm.Page.getAttribute("tss_statuscode").getValue() == 865920000 && Xrm.Page.getAttribute("tss_statusreason").getValue() == 865920000 && underMinimumPrice == true && approvedDiscount == false) {
        var entityid = Xrm.Page.data.entity.getId();
        XrmServiceToolkit.Rest.RetrieveMultiple("tss_quotationpartreasondiscountpackageSet", "?$select=tss_quotationpartreasondiscountpackageId,tss_Result&$filter=tss_QuotationPartHeader/Id eq (guid'" + entityid + "')", function (results) {
            for (var i = 0; i < results.length; i++) {
                var tss_Result = results[i].tss_Result;
                if (tss_Result == null) {
                    flagCheck = false;
                    break;
                }
            }
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, function () {
        }, false);
    }

    return flagCheck;
}

function setTermOfPaymentTab() {
    sectiondisable("tab_2_section_1", true);

    var top = Xrm.Page.getAttribute("tss_top").getValue();
    var quotStatus = Xrm.Page.getAttribute("tss_statuscode").getValue();
    var reasonStatus = Xrm.Page.getAttribute("tss_statusreason").getValue();
    var reqNewTop = Xrm.Page.getAttribute("tss_requestnewtop").getValue();
    var under = Xrm.Page.getAttribute("tss_underminimumprice").getValue();
    var paymentTerm = Xrm.Page.getAttribute("tss_paymentterm").getValue();
    var pss = Xrm.Page.getAttribute("tss_pss").getValue();

    if ((top == 865920000 && quotStatus == 865920000 && reasonStatus == 865920000 && reqNewTop == 865920001 && under == false && paymentTerm != null && pss[0].id.replace(/[{}]/g, "").toLowerCase() == Xrm.Page.context.getUserId().replace(/[{}]/g, "").toLowerCase())
        || (top == 865920001 && quotStatus == 865920000 && reasonStatus == 865920000 && reqNewTop == null && under == false && paymentTerm == null && pss[0].id.replace(/[{}]/g, "").toLowerCase() == Xrm.Page.context.getUserId().replace(/[{}]/g, "").toLowerCase())
        || (top == 865920000 && quotStatus == 865920002 && reasonStatus == 865920004 && reqNewTop == 865920001 && under == true && paymentTerm != null && pss[0].id.replace(/[{}]/g, "").toLowerCase() == Xrm.Page.context.getUserId().replace(/[{}]/g, "").toLowerCase())
        || (top == 865920001 && quotStatus == 865920000 && reasonStatus == 865920000 && reqNewTop == null && under == true && paymentTerm == null && pss[0].id.replace(/[{}]/g, "").toLowerCase() == Xrm.Page.context.getUserId().replace(/[{}]/g, "").toLowerCase())) {
        sectiondisable("tab_2_section_1", false);
    }
}

function sectiondisable(sectionname, disablestatus) {
    var ctrlName = Xrm.Page.ui.controls.get();
    for (var i in ctrlName) {
        var ctrl = ctrlName[i];
        var ctrlSection = ctrl.getParent();
        if (ctrlSection != null) {
            if (ctrlSection.getName() == sectionname) {
                ctrl.setDisabled(disablestatus);
            }
        }
    }
}



function RDDOnChange() {
    if (Xrm.Page.getAttribute("tss_requestdeliverydate").getValue() != null) {
        Xrm.Page.getAttribute("tss_requestdeliverydate").setSubmitMode("always");
        Xrm.Utility.openEntityForm(Xrm.Page.data.entity.getEntityName(), Xrm.Page.data.entity.getId());
    }
}


function setDisableField() {
    if (Xrm.Page.getAttribute("tss_salesorderno").getValue() != null) {
        Xrm.Page.getControl("tss_salesorderno").setDisabled(true);
    }
}

function preFilterLookupSalesOrder() {
  

        if (Xrm.Page.getAttribute("tss_customer").getValue() == null) return;

        var customerId = Xrm.Page.getAttribute("tss_customer").getValue()[0].id;
        var customerName = Xrm.Page.getAttribute("tss_customer").getValue()[0].name;
        var viewId = "{6fd72744-3676-41d4-8003-ae4cde9ac283}";
        var entityName = "tss_sopartheader";
        var viewDisplayName = "SO";

        var fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>" +
                            "<entity name='tss_sopartheader'>" +
                                "<attribute name='tss_sonumber' />" +
                                "<attribute name='tss_sopartheaderid' />" +
                                " <order attribute='tss_sonumber' descending='false' />" +
                                "<filter type='and'>" +
                                    "<condition attribute='tss_customer' operator='eq' value='"+customerId+"' uitype='account' uiname='"+customerName+"'/>" +
                                    "<condition attribute='statecode' operator='eq' value='0'  />" +
                                    "<condition attribute='tss_quotationlink' operator='null' />"
                                "</filter>" +
                              "</entity>" +
                        "</fetch>";

        var layoutXml = "<grid name='resultset' " +
                        "object='1' " +
                        "jump='tss_sopartheaderid' " +
                        "select='1' " +
                        "icon='1' " +
                        "preview='1'>" +
                        "<row name='result' " +
                        "id='tss_sopartheaderid'>" +
                        "<cell name='tss_sonumber' " +
                        "width='200' />" +
                        "</row>" +
                        "</grid>";

        Xrm.Page.getControl("tss_salesorderno").addCustomView(viewId, entityName, viewDisplayName, fetchXml, layoutXml, true);
}