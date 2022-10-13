function setSupportingMaterial() {
    var type = Xrm.Page.getAttribute("trs_type").getValue();
    if (type == false) {
        Xrm.Page.getControl("trs_supportingmaterialoption").setDisabled(false);
        Xrm.Page.getControl("trs_supportingmaterial").setRequiredLevel("none");
    }
    else {
        Xrm.Page.getControl("trs_supportingmaterialoption").setDisabled(true);
        Xrm.Page.getAttribute("trs_supportingmaterialoption").setValue("");
        Xrm.Page.getControl("trs_supportingmaterial").setRequiredLevel("required");
    }
}

function setDetail() {
    var detail = "";
    var sm = Xrm.Page.getAttribute("trs_supportingmaterialoption").getValue();
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
        Xrm.Page.getAttribute("trs_supportingmaterial").setValue(detail);
    }
}

function CalculateTotalPrice() {
    var price = Xrm.Page.getAttribute("trs_price").getValue();
    var quantity = Xrm.Page.getAttribute("trs_quantity").getValue();

    var totalPrice = 0;
    if (price != null && quantity != null) {
        price = price * quantity;
        Xrm.Page.getAttribute("trs_totalprice").setValue(price);
        Xrm.Page.getAttribute("trs_totalprice").setSubmitMode("always");
    }
}

