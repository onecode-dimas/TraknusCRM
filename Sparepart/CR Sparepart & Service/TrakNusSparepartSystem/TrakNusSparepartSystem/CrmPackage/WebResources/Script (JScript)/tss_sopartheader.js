///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// Helper
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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

var StatusReasons = {
    Active: 865920000,
    Submitted: 865920001,
    Closed: 865920002,
    Reserved: 865920003,
    Fulfilled: 865920004
}

var Status = {
    New: 865920000
}
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FORM ON LOAD AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function Form_OnLoad() {
    var formType = Xrm.Page.ui.getFormType();
    var SourceType = Xrm.Page.getAttribute("tss_sourcetype").getValue();

    if (SourceType != null) {
        if (formType == formStatus.Create) {
            SourceTypeChanges(SourceType);
        }
        else if (formType == formStatus.Update) {

        }
    }
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FIELD EVENT AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// PUBLICS AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function SourceTypeChanges(SourceType) {
    if (SourceType == SourceTypes.DirectSales || SourceType == SourceTypes.MarketSize || SourceType == SourceTypes.Service) {
        SetVisibility_Field("tss_quotationlink", true);
        SetVisibility_Field("tss_prospectlink", true);
    }
    else {
        SetVisibility_Field("tss_quotationlink", false);
        SetVisibility_Field("tss_prospectlink", false);
    }

    if (SourceType == SourceTypes.Counter) {
        EnableDisable_Field("tss_top", false);
    }
    else {
        EnableDisable_Field("tss_top", true);
    }
    
    EnableDisable_Field("tss_sourcetype", false);
    EnableDisable_Field("tss_pss", false);
    EnableDisable_Field("tss_distributionchannel", false);
    EnableDisable_Field("tss_salesorganization", false);
    EnableDisable_Field("tss_division", false);
    EnableDisable_Field("tss_currency", false);
    EnableDisable_Field("tss_customer", false);
    EnableDisable_Field("tss_contact", false);
    EnableDisable_Field("tss_soldto", false);
    EnableDisable_Field("tss_soldtocontact", false);
    EnableDisable_Field("tss_billto", false);
    EnableDisable_Field("tss_billtocontact", false);
    EnableDisable_Field("tss_shipto", false);
    EnableDisable_Field("tss_shiptocontact", false);
    EnableDisable_Field("tss_ponumber", false);
    EnableDisable_Field("tss_podate", false);
    EnableDisable_Field("tss_paymentterm", false);
    EnableDisable_Field("tss_requestdeliverydate", false);

    RemoveOptionSet_Field("tss_sourcetype", SourceTypes.DirectSales);
    RemoveOptionSet_Field("tss_sourcetype", SourceTypes.MarketSize);
    RemoveOptionSet_Field("tss_sourcetype", SourceTypes.Service);

    SetValue_Field("tss_statusreason", StatusReasons.Active);
    SetValue_Field("tss_statecode", Status.New);
}

function EnableDisable_Field(fieldname, enabled) {
    Xrm.Page.getControl(fieldname).setDisabled(enabled);
}

function RemoveOptionSet_Field(fieldname, optionset) {
    try {
        Xrm.Page.getControl(fieldname).removeOption(optionset);
    } catch (e) {

    }
}

function SetValue_Field(fieldname, value) {
    try {
        Xrm.Page.getAttribute(fieldname).setValue(value);
    } catch (e) {
        throw new Error("SetValue_Field : " + e.message);
    }
}

function OnChange_SourceType() {
    try {
        var SourceType = Xrm.Page.getAttribute("tss_sourcetype").getValue();

        if (SourceType != null) {
            SourceTypeChanges(SourceType);
        }
    } catch (e) {
        throw new Error("OnChange_SourceType : " + e.message);
    }
}

function SetVisibility_Field(ControlName, Visible) {
    try {
        Xrm.Page.getControl(ControlName).setVisible(Visible);
    }
    catch (e) {
        throw new Error("SetVisibility : " + e.message);
    }
}
