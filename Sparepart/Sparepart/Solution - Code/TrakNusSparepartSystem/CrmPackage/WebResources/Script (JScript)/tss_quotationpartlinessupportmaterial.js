function CalculateTotalPrice() {
    var totalPrice;
    var price = Xrm.Page.getAttribute("tss_price").getValue();
    var quantity = Xrm.Page.getAttribute("tss_quantity").getValue();
    if (price != null && quantity != null) {
        totalPrice = price * quantity;
        Xrm.Page.getAttribute("tss_totalprice").setValue(totalPrice);
        Xrm.Page.getAttribute('tss_totalprice').setSubmitMode("always");
    }
}

function disableAllFields() {
    Xrm.Page.ui.controls.forEach(function (control, i) {
        if (control && control.getDisabled && !control.getDisabled()) {
            control.setDisabled(true);
        }
    });
}