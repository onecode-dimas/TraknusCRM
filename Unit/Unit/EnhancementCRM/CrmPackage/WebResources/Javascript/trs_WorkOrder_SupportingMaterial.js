function ValidateSupportingMaterialType() {
    var MaterialType = Xrm.Page.getAttribute("trs_supportingmaterialtype").getValue();

    if (MaterialType == false) {
        Xrm.Page.getAttribute("trs_standardtext").setRequiredLevel("required");
        Xrm.Page.getControl("trs_standardtext").setVisible(true);
        Xrm.Page.getAttribute("trs_supportingmaterialname").setRequiredLevel("none");
        Xrm.Page.getControl("trs_supportingmaterialname").setDisabled(true);
    }
    else {

        Xrm.Page.getAttribute("trs_supportingmaterialname").setRequiredLevel("required");
        Xrm.Page.getControl("trs_supportingmaterialname").setDisabled(false);
        Xrm.Page.getAttribute("trs_standardtext").setRequiredLevel("none");
        Xrm.Page.getControl("trs_standardtext").setVisible(false);

        Xrm.Page.getAttribute("trs_standardtext").setValue(null);
    }
}

function ValidateJenisSupportingMaterial() {
    var MaterialType = Xrm.Page.getAttribute("trs_supportingmaterialtype").getValue();
    //var JensiSupportingMaterial = Xrm.Page.getAttribute("trs_standardtext").getText();
    var detail = "";
    var sm = Xrm.Page.getAttribute("trs_standardtext").getValue();

    if (MaterialType == false) {
        if (sm != null) {
            if (sm == 167630000) {
                detail = "Transportation";
            }
            else if (sm == 167630001) {
                detail = "Accomodation";
            }
            else if (sm == 167630002) {
                detail = "Paket";
            }
            else if (sm == 167630003) {
                detail = "Pemakaian Alat Bantu";
            }
            else if (sm == 167630004) {
                detail = "Transportation & Accomodation";
            }
        }
        Xrm.Page.getAttribute("trs_supportingmaterialname").setValue(detail);
    }
}

function CalculateTotalPrice() {
    var Price = Xrm.Page.getAttribute("trs_price").getValue();
    var Qty = Xrm.Page.getAttribute("trs_quantity").getValue();
    var TotPrice = Price * Qty;

    Xrm.Page.getAttribute("trs_totalprice").setValue(TotPrice);

}

function SetPropertyField() {
    Xrm.Page.getControl("trs_totalprice").setDisabled(true);
}