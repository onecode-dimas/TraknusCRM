function AccountOnLoadGetParam() {
    var plafond = 0;

    if (Xrm.Page.context.getQueryStringParameters().param_tss_plafondarsparepart != null && Xrm.Page.context.getQueryStringParameters().param_tss_plafondarsparepart != 'undefined') {
        plafond = Xrm.Page.context.getQueryStringParameters().param_tss_plafondarsparepart;
        Xrm.Page.getAttribute("tss_plafondarsparepart").setValue(parseInt(plafond));
        Xrm.Page.getAttribute("tss_plafondarsparepart").setSubmitMode("always");
    }
}