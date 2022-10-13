function CheckDiscountBy() {

    var formType = Xrm.Page.ui.getFormType();
    if (formType == 1) {
        //on create set 
        var currentValue = Xrm.Page.getAttribute("tss_discountby").getValue();
        Xrm.Page.getAttribute("tss_discountby").setValue(1);
        Xrm.Page.getAttribute("tss_discountby").setValue(0);
        Xrm.Page.getAttribute("tss_discountby").setValue(1);
        Xrm.Page.getAttribute("tss_discountby").setSubmitMode("always");
    }
  
}