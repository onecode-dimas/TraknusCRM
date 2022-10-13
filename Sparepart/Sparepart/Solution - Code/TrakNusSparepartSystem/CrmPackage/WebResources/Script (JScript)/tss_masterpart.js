///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Helper
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
var tab_InterchangeGrid = 'tab_4';
var tab_PriceList = 'Tab_Price_List';
var tab_ModelCompatibility = 'tab_model_compatibility';
var tab_Image = 'tab_5';
var tab_Interchange = 'tab_4';
var tab_TaskList = 'tab_6';
var tab_TyreType = 'tab_7';
var tab_TyrePosition = 'tab_8';
var tab_BatterayType = 'tab_9';
var tab_Oil = 'tab_10';
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FORM ON LOAD AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function Form_OnLoad() {
    OnChange_IsPartInterchange();
    partType_onChange();
    OnChange_IsPartInterchange();
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FIELD EVENT AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function OnChange_IsPartInterchange() {
    var IsInterchange = Xrm.Page.data.entity.attributes.get("tss_ispartinterchange").getValue();

    if (IsInterchange != null && IsInterchange == 1) {
        Xrm.Page.ui.tabs.get(tab_InterchangeGrid).setVisible(false);
    }
    else {
        Xrm.Page.ui.tabs.get(tab_InterchangeGrid).setVisible(true);
    }
}

function partType_onChange() {
    var MAIN_PART = 865920000;

    var partType = Xrm.Page.getAttribute("tss_parttype").getValue();
    if (partType != null) {
        if (partType == MAIN_PART) {
            Xrm.Page.getControl("tss_commoditytype").setVisible(false);
            Xrm.Page.getAttribute("tss_commoditytype").setRequiredLevel("none");
            Xrm.Page.getAttribute("tss_commoditytype").setValue(null);

            //visible tab
            Xrm.Page.ui.tabs.get(tab_TaskList).setVisible(true);

            Xrm.Page.ui.tabs.get(tab_TyreType).setVisible(false);
            Xrm.Page.ui.tabs.get(tab_TyrePosition).setVisible(false);
            Xrm.Page.ui.tabs.get(tab_BatterayType).setVisible(false);
            Xrm.Page.ui.tabs.get(tab_Oil).setVisible(false);
        }
        else {
            Xrm.Page.getControl("tss_commoditytype").setVisible(true);
            Xrm.Page.getAttribute("tss_commoditytype").setRequiredLevel("required");

            //visible tab
            Xrm.Page.ui.tabs.get(tab_TaskList).setVisible(false);
        }
    }
}

function preventSave(econtext) {
    var COMMODITY = 865920001;
    var partType = Xrm.Page.getAttribute("tss_parttype").getValue();
    var brand = Xrm.Page.getAttribute("tss_brand").getValue();
    var eventArgs = econtext.getEventArgs();
    if (partType == COMMODITY && brand == null) {
        if (eventArgs.getSaveMode() == 70 || eventArgs.getSaveMode() == 1 || eventArgs.getSaveMode() == 2 || eventArgs.getSaveMode() == 59 || eventArgs.getSaveMode() == 80) {
            eventArgs.preventDefault();
            alert("Cannot save the record, Brand still Empty");
        }
    }
}

function onChange_CommodityType() {
    var TYRE = 865920000;
    var BATTERAY = 865920001;
    var OIL = 865920002;
    var OTHERS = 865920003;

    var commodityType = Xrm.Page.getAttribute("tss_commoditytype").getValue();

    if (commodityType != null) {
        if (commodityType == TYRE) {
            //show
            Xrm.Page.ui.tabs.get(tab_TyreType).setVisible(true);
            Xrm.Page.ui.tabs.get(tab_TyrePosition).setVisible(true);

            //hide
            Xrm.Page.ui.tabs.get(tab_BatterayType).setVisible(false);
            Xrm.Page.ui.tabs.get(tab_Oil).setVisible(false);
        }
        else if (commodityType == BATTERAY) {
            //show
            Xrm.Page.ui.tabs.get(tab_BatterayType).setVisible(true);

            //hide
            Xrm.Page.ui.tabs.get(tab_TyreType).setVisible(false);
            Xrm.Page.ui.tabs.get(tab_TyrePosition).setVisible(false);
            Xrm.Page.ui.tabs.get(tab_Oil).setVisible(false);
        }
        else if (commodityType == OIL) {
            //show
            Xrm.Page.ui.tabs.get(tab_Oil).setVisible(true);

            //hide
            Xrm.Page.ui.tabs.get(tab_TyreType).setVisible(false);
            Xrm.Page.ui.tabs.get(tab_TyrePosition).setVisible(false);
            Xrm.Page.ui.tabs.get(tab_BatterayType).setVisible(false);
        }
        else {
            Xrm.Page.ui.tabs.get(tab_TyreType).setVisible(false);
            Xrm.Page.ui.tabs.get(tab_TyrePosition).setVisible(false);
            Xrm.Page.ui.tabs.get(tab_BatterayType).setVisible(false);
            Xrm.Page.ui.tabs.get(tab_Oil).setVisible(false);
        }
    }
}