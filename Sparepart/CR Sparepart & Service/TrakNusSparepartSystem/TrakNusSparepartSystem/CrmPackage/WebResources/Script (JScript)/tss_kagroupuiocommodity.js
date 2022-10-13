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

function IsConfirm() {
    debugger;
    var result = true;
    var keyAccountID = Xrm.Page.getAttribute("tss_keyaccountid").getValue()[0].id;
    XrmServiceToolkit.Rest.Retrieve(keyAccountID, "tss_keyaccountSet", "tss_Reason,tss_Status", null, function (result) {
        var tss_Reason = result.tss_Reason;
        var tss_Status = result.tss_Status;
        if (tss_Reason == 865920005)
            result = false;
    }, function (error) {
        Xrm.Utility.alertDialog(error.message);
    }, false);
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

function OnSave(context) {
    var saveEvent = context.getEventArgs();
    var status = 0;
    var keyAccountID = Xrm.Page.getAttribute("tss_keyaccountid").getValue()[0].id;
    XrmServiceToolkit.Rest.Retrieve(keyAccountID, "tss_keyaccountSet", "tss_Reason,tss_Status", null, function (result) {
        status = result.tss_Status.Value;
    }, function (error) {
        Xrm.Utility.alertDialog(error.message);
    }, false);

    if (status != 865920000) {
        Xrm.Utility.alertDialog("Data has been processing, canot modify this data");
        saveEvent.preventDefault();
    }
}
//Event OnLoad
function Form_OnLoad() {
    if (formType < 2) {
        SetDefaultValue_TwoOptionField("tss_calculatetoms");

        Xrm.Page.getAttribute("tss_calculatestatus").setValue(null);
        Xrm.Page.getAttribute("tss_calculatestatus").setSubmitMode("always");

        Xrm.Page.getAttribute("tss_reason").setRequiredLevel("none");
        Xrm.Page.getControl("tss_reason").setVisible(false);
    }

    var keyAccountID = Xrm.Page.getAttribute("tss_keyaccountid").getValue()[0].id;

    XrmServiceToolkit.Rest.Retrieve(keyAccountID, "tss_keyaccountSet", "tss_Customer,tss_PSS,tss_Status,tss_Revision", null, function (result) {
        if (result.tss_Customer.Id != null) {
            Xrm.Page.getAttribute("tss_customer").setValue([{ id: result.tss_Customer.Id, name: result.tss_Customer.Name, entityType: result.tss_Customer.LogicalName }]);
            //Xrm.Page.getAttribute("tss_Customer").setSubmitMode("always");
            if (result.tss_Status.Value != 865920000) {
                Xrm.Page.getControl("tss_groupuiocommodity").setDisabled(true);
                Xrm.Page.getControl("tss_calculatetoms").setDisabled(true);
                Xrm.Page.getControl("tss_reason").setDisabled(true);
            }
            else {
                if (result.tss_Revision > 0) {
                    Xrm.Page.getControl("tss_calculatetoms").setDisabled(false);
                }
            }
        }

        if (result.tss_PSS.Id != null) {
            Xrm.Page.getAttribute("tss_pss").setValue([{ id: result.tss_PSS.Id, name: result.tss_PSS.Name, entityType: result.tss_PSS.LogicalName }]);
            //Xrm.Page.getAttribute("tss_pss").setSubmitMode("always");
        }
        GroupUIOCommodity_Prefilter();
    }, function (error) {
        Xrm.Utility.alertDialog(error.message);
    }, true);

    onchange_calculatetoms();
}

//PreFilter Group UIO Commodity
function GroupUIOCommodity_Prefilter() {

    var Customer = Xrm.Page.getAttribute("tss_customer").getValue();

    Xrm.Page.getControl("tss_groupuiocommodity")._control && Xrm.Page.getControl("tss_groupuiocommodity")._control.tryCompleteOnDemandInitialization && Xrm.Page.getControl("tss_groupuiocommodity")._control.tryCompleteOnDemandInitialization();
    Xrm.Page.getControl("tss_groupuiocommodity").addPreSearch(function () {
        addLookupFilterGroupUIOCommodity(Customer);
    });

}

function addLookupFilterGroupUIOCommodity(Customer) {
  

    if (Customer != null) {
        var fetchFilters = "<filter type='and'>" +
            "<condition attribute='tss_customer' uitype='" + Customer[0].entityType + "' operator='eq' value='" + Customer[0].id + "'/>" + 
            "</filter>";
        Xrm.Page.getControl("tss_groupuiocommodity").addCustomFilter(fetchFilters);
    }
}

//PreFilter Tyre Spec
function GroupUIOCommodityTyreSpec_Prefilter() {
    var TyreType = Xrm.Page.getAttribute("tss_tyretype").getValue();

    Xrm.Page.getControl("tss_tyrespec")._control && Xrm.Page.getControl("tss_tyrespec")._control.tryCompleteOnDemandInitialization && Xrm.Page.getControl("tss_tyrespec")._control.tryCompleteOnDemandInitialization();
    Xrm.Page.getControl("tss_tyrespec").addPreSearch(function () {
        addLookupFilterGroupUIOCommodityTyreSpec(TyreType);
    });

}

function addLookupFilterGroupUIOCommodityTyreSpec(TyreType) {
    if (TyreType != null) {
        var fetchFilters = "<filter type='and'>" +
            "<condition attribute='tss_tyretype' uitype='" + "tss_partmasterlinestyretype" + "' operator='eq' value='" + TyreType + "'/>" +
            "</filter>";
        Xrm.Page.getControl("tss_tyrespec").addCustomFilter(fetchFilters);
    }
}

function onchange_calculatetoms() {
    var _calculatetoms = GetAttributeValue(Xrm.Page.getAttribute("tss_calculatetoms"));

    if (_calculatetoms != null) {
        if (_calculatetoms) {
            Xrm.Page.getAttribute("tss_reason").setRequiredLevel("none");
            Xrm.Page.getControl("tss_reason").setVisible(false);
            Xrm.Page.getControl("tss_reason").setDisabled(true);
            Xrm.Page.getAttribute("tss_reason").setValue(null);
            Xrm.Page.getAttribute("tss_reason").setSubmitMode("always");
        }
        else {
            Xrm.Page.getAttribute("tss_reason").setRequiredLevel("required");
            Xrm.Page.getControl("tss_reason").setVisible(true);
            Xrm.Page.getControl("tss_reason").setDisabled(false);
        }
    }
    else {
        Xrm.Page.getAttribute("tss_reason").setRequiredLevel("none");
        Xrm.Page.getControl("tss_reason").setVisible(false);
        Xrm.Page.getControl("tss_reason").setDisabled(true);
        Xrm.Page.getAttribute("tss_reason").setValue(null);
        Xrm.Page.getAttribute("tss_reason").setSubmitMode("always");
    }
}