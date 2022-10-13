function partNumber_onchange()
{ 

    XrmServiceToolkit.Rest.Retrieve(Xrm.Page.getAttribute("tss_partnumber").getValue()[0].id , "trs_masterpartSet", "tss_commoditytype", null, function (result) {
        var tss_commoditytype = result.tss_commoditytype;
        if(tss_commoditytype.Value == "865920000")
        {
            Xrm.Page.getControl("tss_hmplanning").setDisabled(false);
            Xrm.Page.getControl("tss_hmplanning").setVisible(true);
            Xrm.Page.getAttribute("tss_hmplanning").setRequiredLevel("required");

            Xrm.Page.getControl("tss_tyrecode").setDisabled(false);
            Xrm.Page.getControl("tss_tyrecode").setVisible(true);
            Xrm.Page.getAttribute("tss_tyrecode").setRequiredLevel("required");

        } else {
            Xrm.Page.getControl("tss_hmplanning").setDisabled(true);
            Xrm.Page.getControl("tss_hmplanning").setVisible(false);
            Xrm.Page.getAttribute("tss_hmplanning").setRequiredLevel("none");
            Xrm.Page.getAttribute("tss_hmplanning").setValue(null);

            Xrm.Page.getControl("tss_tyrecode").setDisabled(true);
            Xrm.Page.getControl("tss_tyrecode").setVisible(false);
            Xrm.Page.getAttribute("tss_tyrecode").setRequiredLevel("none");
            Xrm.Page.getAttribute("tss_tyrecode").setValue(null);
        }
    }, function (error) {
        Xrm.Utility.alertDialog(error.message);
    }, true);
}