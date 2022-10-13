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

function GetCommodityType(values) {
    console.log("Value Parameter: ", values);
    debugger;
}

function OnLoad() {
    debugger;
    if (formType < 2 && Xrm.Page.getAttribute("createdon").getValue() == null) {
        Hide_BATTERY();
        Hide_OIL();
        Hide_TYRE();

        SetDefault_Battery();
    }
    else
    {
        var oPartCommodityType = Xrm.Page.getAttribute("tss_partcommoditytype").getValue();

        // BATTERY
        if (oPartCommodityType == 865920000) {
            Hide_OIL();
            Hide_TYRE();

            Xrm.Page.getAttribute("tss_batteryby").setRequiredLevel("required");
            Xrm.Page.getAttribute("tss_battery").setRequiredLevel("required");

            var oBatteryBy = Xrm.Page.getAttribute("tss_batteryby").getValue();

            if (oBatteryBy == 865920000) {
                Xrm.Page.getAttribute("tss_batterypartnumber").setRequiredLevel("required");

                Xrm.Page.getControl("tss_batterytype").setVisible(false);
                Xrm.Page.getControl("tss_batterytraction").setVisible(false);
                Xrm.Page.getControl("tss_batterycranking").setVisible(false);
            }
            else if (oBatteryBy == 865920001) {
                Xrm.Page.getAttribute("tss_batterytype").setRequiredLevel("required");

                Xrm.Page.getControl("tss_batterypartnumber").setVisible(false);

                var oBatteryType = Xrm.Page.getAttribute("tss_batterytype").getValue();

                if (oBatteryType == 865920000) {
                    Xrm.Page.getAttribute("tss_batterytraction").setRequiredLevel("required");

                    Xrm.Page.getControl("tss_batterycranking").setVisible(false);
                }
                else if (oBatteryType == 865920001) {
                    Xrm.Page.getAttribute("tss_batterycranking").setRequiredLevel("required");

                    Xrm.Page.getControl("tss_batterytraction").setVisible(false);
                }
            }
        }
        // OIL
        else if (oPartCommodityType == 865920001) {
            Hide_BATTERY();
            Hide_TYRE();

            Xrm.Page.getAttribute("tss_oilby").setRequiredLevel("required");
            Xrm.Page.getAttribute("tss_oilqtyby").setRequiredLevel("required");

            var oOilBy = Xrm.Page.getAttribute("tss_oilby").getValue();

            if (oOilBy == 865920000) {
                Xrm.Page.getAttribute("tss_oilpartnumber").setRequiredLevel("required");

                Xrm.Page.getControl("tss_jenisoil").setVisible(false);
            }
            else if (oOilBy == 865920001) {
                Xrm.Page.getAttribute("tss_jenisoil").setRequiredLevel("required");

                Xrm.Page.getControl("tss_oilpartnumber").setVisible(false);
            }

            var oOilQtyBy = Xrm.Page.getAttribute("tss_oilqtyby").getValue();

            if (oOilQtyBy == 865920000) {
                Xrm.Page.getAttribute("tss_oilqtypcs").setRequiredLevel("required");

                Xrm.Page.getControl("tss_oil").setVisible(false);
            }
            else if (oOilQtyBy == 865920001) {
                Xrm.Page.getAttribute("tss_oil").setRequiredLevel("required");

                Xrm.Page.getControl("tss_oilqtypcs").setVisible(false);
            }
        }
        // TYRE
        else if (oPartCommodityType == 865920002) {
            Hide_BATTERY();
            Hide_OIL();

            Xrm.Page.getAttribute("tss_tyreby").setRequiredLevel("required");
            Xrm.Page.getAttribute("tss_tyre").setRequiredLevel("required");

            var oTyreBy = Xrm.Page.getAttribute("tss_tyreby").getValue();

            if (oTyreBy == 865920000) {
                Xrm.Page.getAttribute("tss_tyrepartnumber").setRequiredLevel("required");

                Xrm.Page.getControl("tss_tyretype").setVisible(false);
                Xrm.Page.getControl("tss_tyreposition").setVisible(false);
                Xrm.Page.getControl("tss_tyrespec").setVisible(false);
            }
            else if (oTyreBy == 865920001) {
                Xrm.Page.getAttribute("tss_tyretype").setRequiredLevel("required");
                Xrm.Page.getAttribute("tss_tyreposition").setRequiredLevel("required");
                Xrm.Page.getAttribute("tss_tyrespec").setRequiredLevel("required");

                Xrm.Page.getControl("tss_tyrepartnumber").setVisible(false);
            }
        }
    }
}

function SetDefault_Battery() {
    Xrm.Page.getControl("tss_batteryby").setVisible(true);
    Xrm.Page.getControl("tss_batterypartnumber").setVisible(true);
    Xrm.Page.getControl("tss_battery").setVisible(true);

    Xrm.Page.getAttribute("tss_batteryby").setRequiredLevel("required");
    Xrm.Page.getAttribute("tss_batterypartnumber").setRequiredLevel("required");
    Xrm.Page.getAttribute("tss_battery").setRequiredLevel("required");

    Xrm.Page.getAttribute("tss_batteryby").setValue(865920000);

    // --

    Xrm.Page.getControl("tss_batterytype").setVisible(false);
    Xrm.Page.getControl("tss_batterytraction").setVisible(false);
    Xrm.Page.getControl("tss_batterycranking").setVisible(false);

    Xrm.Page.getAttribute("tss_batterytype").setRequiredLevel("none");
    Xrm.Page.getAttribute("tss_batterytraction").setRequiredLevel("none");
    Xrm.Page.getAttribute("tss_batterycranking").setRequiredLevel("none");

    Xrm.Page.getAttribute("tss_batterytype").setSubmitMode("always");
    Xrm.Page.getAttribute("tss_batterytraction").setSubmitMode("always");
    Xrm.Page.getAttribute("tss_batterycranking").setSubmitMode("always");

    Xrm.Page.getAttribute("tss_batterytype").setValue(null);
    Xrm.Page.getAttribute("tss_batterytraction").setValue(null);
    Xrm.Page.getAttribute("tss_batterycranking").setValue(null);
}

function SetDefault_Oil() {
    Xrm.Page.getControl("tss_oilby").setVisible(true);
    Xrm.Page.getControl("tss_oilqtyby").setVisible(true);
    Xrm.Page.getControl("tss_oilqtypcs").setVisible(true);

    Xrm.Page.getAttribute("tss_oilby").setRequiredLevel("required");
    Xrm.Page.getAttribute("tss_oilqtyby").setRequiredLevel("required");
    Xrm.Page.getAttribute("tss_oilqtypcs").setRequiredLevel("required");

    Xrm.Page.getAttribute("tss_oilby").setValue(865920000);
    Xrm.Page.getAttribute("tss_oilqtyby").setValue(865920000);

    // --

    Xrm.Page.getControl("tss_oilpartnumber").setVisible(true);
    Xrm.Page.getAttribute("tss_oilpartnumber").setRequiredLevel("required");

    // --

    Xrm.Page.getControl("tss_jenisoil").setVisible(false);
    Xrm.Page.getAttribute("tss_jenisoil").setRequiredLevel("none");
    Xrm.Page.getAttribute("tss_jenisoil").setValue(null);

    // --

    Xrm.Page.getControl("tss_oil").setVisible(false);
    Xrm.Page.getAttribute("tss_oil").setRequiredLevel("none");
    Xrm.Page.getAttribute("tss_oil").setValue(null);
}

function SetDefault_Tyre() {
    Xrm.Page.getControl("tss_tyreby").setVisible(true);
    Xrm.Page.getControl("tss_tyre").setVisible(true);

    Xrm.Page.getAttribute("tss_tyreby").setRequiredLevel("required");
    Xrm.Page.getAttribute("tss_tyre").setRequiredLevel("required");
    
    Xrm.Page.getAttribute("tss_tyreby").setValue(865920000);

    // --

    Xrm.Page.getControl("tss_tyrepartnumber").setVisible(true);
    Xrm.Page.getAttribute("tss_tyrepartnumber").setRequiredLevel("required");

    // --

    Xrm.Page.getControl("tss_tyretype").setVisible(false);
    Xrm.Page.getControl("tss_tyreposition").setVisible(false);
    Xrm.Page.getControl("tss_tyrespec").setVisible(false);

    Xrm.Page.getAttribute("tss_tyretype").setRequiredLevel("none");
    Xrm.Page.getAttribute("tss_tyreposition").setRequiredLevel("none");
    Xrm.Page.getAttribute("tss_tyrespec").setRequiredLevel("none");

    Xrm.Page.getAttribute("tss_tyretype").setValue(null);
    Xrm.Page.getAttribute("tss_tyreposition").setValue(null);
    Xrm.Page.getAttribute("tss_tyrespec").setValue(null);
}

function PartCommodityType_onChange() {
    debugger;
    var oPartCommodityType = Xrm.Page.getAttribute("tss_partcommoditytype").getValue();

    // BATTERY
    if (oPartCommodityType == 865920000) {
        PartCommodityType_BATTERY();
    }
    // OIL
    else if (oPartCommodityType == 865920001) {
        PartCommodityType_OIL();
    }
    // TYRE
    else if (oPartCommodityType == 865920002) {
        PartCommodityType_TYRE();
    }
}

function PartCommodityType_BATTERY() {
    Hide_OIL();
    Hide_TYRE();

    SetDefault_Battery();
}

function PartCommodityType_OIL() {
    Hide_BATTERY();
    Hide_TYRE();

    SetDefault_Oil();
}

function PartCommodityType_TYRE() {
    Hide_BATTERY();
    Hide_OIL();

    SetDefault_Tyre();
}

function Hide_BATTERY() {
    Xrm.Page.getAttribute("tss_batteryby").setRequiredLevel("none");
    Xrm.Page.getAttribute("tss_batterypartnumber").setRequiredLevel("none");
    Xrm.Page.getAttribute("tss_batterytype").setRequiredLevel("none");
    Xrm.Page.getAttribute("tss_batterytraction").setRequiredLevel("none");
    Xrm.Page.getAttribute("tss_batterycranking").setRequiredLevel("none");
    Xrm.Page.getAttribute("tss_battery").setRequiredLevel("none");

    Xrm.Page.getAttribute("tss_batteryby").setValue(null);
    Xrm.Page.getAttribute("tss_batterypartnumber").setValue(null);
    Xrm.Page.getAttribute("tss_batterytype").setValue(null);
    Xrm.Page.getAttribute("tss_batterytraction").setValue(null);
    Xrm.Page.getAttribute("tss_batterycranking").setValue(null);
    Xrm.Page.getAttribute("tss_battery").setValue(null);

    Xrm.Page.getAttribute("tss_batteryby").setSubmitMode("always");
    Xrm.Page.getAttribute("tss_batterypartnumber").setSubmitMode("always");
    Xrm.Page.getAttribute("tss_batterytype").setSubmitMode("always");
    Xrm.Page.getAttribute("tss_batterytraction").setSubmitMode("always");
    Xrm.Page.getAttribute("tss_batterycranking").setSubmitMode("always");
    Xrm.Page.getAttribute("tss_battery").setSubmitMode("always");

    Xrm.Page.getControl("tss_batteryby").setVisible(false);
    Xrm.Page.getControl("tss_batterypartnumber").setVisible(false);
    Xrm.Page.getControl("tss_batterytype").setVisible(false);
    Xrm.Page.getControl("tss_batterytraction").setVisible(false);
    Xrm.Page.getControl("tss_batterycranking").setVisible(false);
    Xrm.Page.getControl("tss_battery").setVisible(false);
}

function Hide_OIL() {
    Xrm.Page.getAttribute("tss_oilby").setRequiredLevel("none");
    Xrm.Page.getAttribute("tss_oilpartnumber").setRequiredLevel("none");
    Xrm.Page.getAttribute("tss_jenisoil").setRequiredLevel("none");
    Xrm.Page.getAttribute("tss_oilqtyby").setRequiredLevel("none");
    Xrm.Page.getAttribute("tss_oilqtypcs").setRequiredLevel("none");
    Xrm.Page.getAttribute("tss_oil").setRequiredLevel("none");

    Xrm.Page.getAttribute("tss_oilby").setValue(null);
    Xrm.Page.getAttribute("tss_oilpartnumber").setValue(null);
    Xrm.Page.getAttribute("tss_jenisoil").setValue(null);
    Xrm.Page.getAttribute("tss_oilqtyby").setValue(null);
    Xrm.Page.getAttribute("tss_oilqtypcs").setValue(null);
    Xrm.Page.getAttribute("tss_oil").setValue(null);

    Xrm.Page.getAttribute("tss_oilby").setSubmitMode("always");
    Xrm.Page.getAttribute("tss_oilpartnumber").setSubmitMode("always");
    Xrm.Page.getAttribute("tss_jenisoil").setSubmitMode("always");
    Xrm.Page.getAttribute("tss_oilqtyby").setSubmitMode("always");
    Xrm.Page.getAttribute("tss_oilqtypcs").setSubmitMode("always");
    Xrm.Page.getAttribute("tss_oil").setSubmitMode("always");

    Xrm.Page.getControl("tss_oilby").setVisible(false);
    Xrm.Page.getControl("tss_oilpartnumber").setVisible(false);
    Xrm.Page.getControl("tss_jenisoil").setVisible(false);
    Xrm.Page.getControl("tss_oilqtyby").setVisible(false);
    Xrm.Page.getControl("tss_oilqtypcs").setVisible(false);
    Xrm.Page.getControl("tss_oil").setVisible(false);
}

function Hide_TYRE() {
    Xrm.Page.getAttribute("tss_tyreby").setRequiredLevel("none");
    Xrm.Page.getAttribute("tss_tyrepartnumber").setRequiredLevel("none");
    Xrm.Page.getAttribute("tss_tyretype").setRequiredLevel("none");
    Xrm.Page.getAttribute("tss_tyreposition").setRequiredLevel("none");
    Xrm.Page.getAttribute("tss_tyrespec").setRequiredLevel("none");
    Xrm.Page.getAttribute("tss_tyre").setRequiredLevel("none");

    Xrm.Page.getAttribute("tss_tyreby").setValue(null);
    Xrm.Page.getAttribute("tss_tyrepartnumber").setValue(null);
    Xrm.Page.getAttribute("tss_tyretype").setValue(null);
    Xrm.Page.getAttribute("tss_tyreposition").setValue(null);
    Xrm.Page.getAttribute("tss_tyrespec").setValue(null);
    Xrm.Page.getAttribute("tss_tyre").setValue(null);

    Xrm.Page.getAttribute("tss_tyreby").setSubmitMode("always");
    Xrm.Page.getAttribute("tss_tyrepartnumber").setSubmitMode("always");
    Xrm.Page.getAttribute("tss_tyretype").setSubmitMode("always");
    Xrm.Page.getAttribute("tss_tyreposition").setSubmitMode("always");
    Xrm.Page.getAttribute("tss_tyrespec").setSubmitMode("always");
    Xrm.Page.getAttribute("tss_tyre").setSubmitMode("always");

    Xrm.Page.getControl("tss_tyreby").setVisible(false);
    Xrm.Page.getControl("tss_tyrepartnumber").setVisible(false);
    Xrm.Page.getControl("tss_tyretype").setVisible(false);
    Xrm.Page.getControl("tss_tyreposition").setVisible(false);
    Xrm.Page.getControl("tss_tyrespec").setVisible(false);
    Xrm.Page.getControl("tss_tyre").setVisible(false);
}

function BatteryBy_OnChange() {
    debugger;
    var oPartCommodityType = Xrm.Page.getAttribute("tss_partcommoditytype").getValue();

    // BATTERY
    if (oPartCommodityType == 865920000) {
        var oBatteryBy = Xrm.Page.getAttribute("tss_batteryby").getValue();

        if (oBatteryBy == 865920000) {
            SetDefault_Battery();
        }
        else if (oBatteryBy == 865920001) {
            Xrm.Page.getAttribute("tss_batterypartnumber").setRequiredLevel("none");
            Xrm.Page.getAttribute("tss_batterypartnumber").setValue(null);
            Xrm.Page.getControl("tss_batterypartnumber").setVisible(false);

            Xrm.Page.getControl("tss_batterytype").setVisible(true);
            Xrm.Page.getAttribute("tss_batterytype").setRequiredLevel("required");
            Xrm.Page.getAttribute("tss_batterytype").setValue(865920000);

            Xrm.Page.getControl("tss_batterytraction").setVisible(true);
            Xrm.Page.getAttribute("tss_batterytraction").setRequiredLevel("required");

            // --

            Xrm.Page.getControl("tss_batterycranking").setVisible(false);
            Xrm.Page.getAttribute("tss_batterycranking").setRequiredLevel("none");
            Xrm.Page.getAttribute("tss_batterycranking").setValue(null);
        }
    }
}

function BatteryType_OnChange() {
    debugger;
    var oPartCommodityType = Xrm.Page.getAttribute("tss_partcommoditytype").getValue();

    // BATTERY
    if (oPartCommodityType == 865920000) {
        var oBatteryBy = Xrm.Page.getAttribute("tss_batteryby").getValue();

        if (oBatteryBy == 865920001) {
            var oBatteryType = Xrm.Page.getAttribute("tss_batterytype").getValue();

            if (oBatteryType == 865920000) {
                Xrm.Page.getControl("tss_batterytraction").setVisible(true);
                Xrm.Page.getAttribute("tss_batterytraction").setRequiredLevel("required");

                // --

                Xrm.Page.getControl("tss_batterycranking").setVisible(false);
                Xrm.Page.getAttribute("tss_batterycranking").setRequiredLevel("none");
                Xrm.Page.getAttribute("tss_batterycranking").setValue(null);
            }
            else if (oBatteryType == 865920001) {
                Xrm.Page.getControl("tss_batterytraction").setVisible(false);
                Xrm.Page.getAttribute("tss_batterytraction").setRequiredLevel("none");
                Xrm.Page.getAttribute("tss_batterytraction").setValue(null);

                // --

                Xrm.Page.getControl("tss_batterycranking").setVisible(true);
                Xrm.Page.getAttribute("tss_batterycranking").setRequiredLevel("required");
            }
        }
    }

    //var oBatteryType = Xrm.Page.getAttribute("tss_batterytype").getValue();

    //if (oBatteryType == 865920000) {
    //    Xrm.Page.getControl("tss_batterytraction").setVisible(true);
    //    Xrm.Page.getAttribute("tss_batterytraction").setRequiredLevel("required");

    //    //--

    //    Xrm.Page.getAttribute("tss_batterycranking").setRequiredLevel("none");
    //    Xrm.Page.getAttribute("tss_batterycranking").setValue(null);
    //    Xrm.Page.getControl("tss_batterycranking").setVisible(false);
    //}
    //else if (oBatteryType == 865920001) {
    //    Xrm.Page.getAttribute("tss_batterytraction").setRequiredLevel("none");
    //    Xrm.Page.getAttribute("tss_batterytraction").setValue(null);
    //    Xrm.Page.getControl("tss_batterytraction").setVisible(false);

    //    //--

    //    Xrm.Page.getControl("tss_batterycranking").setVisible(true);
    //    Xrm.Page.getAttribute("tss_batterycranking").setRequiredLevel("required");
    //}
}

function OilBy_OnChange() {
    debugger;
    var oPartCommodityType = Xrm.Page.getAttribute("tss_partcommoditytype").getValue();

    // OIL
    if (oPartCommodityType == 865920001) {
        var oOilBy = Xrm.Page.getAttribute("tss_oilby").getValue();

        if (oOilBy == 865920000) {
            Xrm.Page.getControl("tss_oilpartnumber").setVisible(true);
            Xrm.Page.getAttribute("tss_oilpartnumber").setRequiredLevel("required");

            // --

            Xrm.Page.getControl("tss_jenisoil").setVisible(false);
            Xrm.Page.getAttribute("tss_jenisoil").setRequiredLevel("none");
            Xrm.Page.getAttribute("tss_jenisoil").setValue(null);
        }
        else if (oOilBy == 865920001) {
            Xrm.Page.getControl("tss_oilpartnumber").setVisible(false);
            Xrm.Page.getAttribute("tss_oilpartnumber").setRequiredLevel("none");
            Xrm.Page.getAttribute("tss_oilpartnumber").setValue(null);

            // --

            Xrm.Page.getControl("tss_jenisoil").setVisible(true);
            Xrm.Page.getAttribute("tss_jenisoil").setRequiredLevel("required");
        }
    }
}

function OilQtyBy_OnChange() {
    debugger;
    var oPartCommodityType = Xrm.Page.getAttribute("tss_partcommoditytype").getValue();

    // OIL
    if (oPartCommodityType == 865920001) {
        var oOilQtyBy = Xrm.Page.getAttribute("tss_oilqtyby").getValue();

        if (oOilQtyBy == 865920000) {
            Xrm.Page.getControl("tss_oil").setVisible(false);
            Xrm.Page.getAttribute("tss_oil").setRequiredLevel("none");
            Xrm.Page.getAttribute("tss_oil").setValue(null);

            // --

            Xrm.Page.getControl("tss_oilqtypcs").setVisible(true);
            Xrm.Page.getAttribute("tss_oilqtypcs").setRequiredLevel("required");
        }
        else if (oOilQtyBy == 865920001) {
            Xrm.Page.getControl("tss_oil").setVisible(true);
            Xrm.Page.getAttribute("tss_oil").setRequiredLevel("required");

            // --

            Xrm.Page.getControl("tss_oilqtypcs").setVisible(false);
            Xrm.Page.getAttribute("tss_oilqtypcs").setRequiredLevel("none");
            Xrm.Page.getAttribute("tss_oilqtypcs").setValue(null);
        }
    }
}

function TyreBy_OnChange() {
    debugger;
    var oPartCommodityType = Xrm.Page.getAttribute("tss_partcommoditytype").getValue();

    // TYRE
    if (oPartCommodityType == 865920002) {
        var oTyreBy = Xrm.Page.getAttribute("tss_tyreby").getValue();

        if (oTyreBy == 865920000) {
            SetDefault_Tyre();
        }
        else if (oTyreBy == 865920001) {
            Xrm.Page.getControl("tss_tyrepartnumber").setVisible(false);
            Xrm.Page.getAttribute("tss_tyrepartnumber").setRequiredLevel("none");
            Xrm.Page.getAttribute("tss_tyrepartnumber").setValue(null);

            // --

            Xrm.Page.getControl("tss_tyretype").setVisible(true);
            Xrm.Page.getControl("tss_tyreposition").setVisible(true);
            Xrm.Page.getControl("tss_tyrespec").setVisible(true);

            Xrm.Page.getAttribute("tss_tyretype").setRequiredLevel("required");
            Xrm.Page.getAttribute("tss_tyreposition").setRequiredLevel("required");
            Xrm.Page.getAttribute("tss_tyrespec").setRequiredLevel("required");
        }
    }
}