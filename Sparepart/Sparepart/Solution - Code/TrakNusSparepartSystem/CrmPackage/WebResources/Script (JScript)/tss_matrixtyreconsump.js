function getattributevalue(_attribute) {
    var _result = null;

    if (_attribute != null) {
        if (_attribute.getValue() != null) {
            _result = _attribute.getValue();
        }
    }

    return _result;
}

function onload_form() {
    onchange_sector();
}

function onchange_sector() {
    var _sector = getattributevalue(Xrm.Page.getAttribute("tss_sector"));

    if (_sector != null) {
        if (_sector == 865920000) {
            // INDUSTRIAL
            Xrm.Page.getAttribute("tss_issue").setRequiredLevel("required");
            Xrm.Page.getControl("tss_issue").setVisible(true);
            Xrm.Page.getControl("tss_issue").setDisabled(false);

            Xrm.Page.getAttribute("tss_season").setRequiredLevel("none");
            Xrm.Page.getControl("tss_season").setVisible(false);
            Xrm.Page.getControl("tss_season").setDisabled(true);
            Xrm.Page.getAttribute("tss_season").setValue(null);
            Xrm.Page.getAttribute("tss_season").setSubmitMode("always");
        }
        else if (_sector == 865920001) {
            // AGRO
            Xrm.Page.getAttribute("tss_issue").setRequiredLevel("none");
            Xrm.Page.getControl("tss_issue").setVisible(false);
            Xrm.Page.getControl("tss_issue").setDisabled(true);
            Xrm.Page.getAttribute("tss_issue").setValue(null);
            Xrm.Page.getAttribute("tss_issue").setSubmitMode("always");

            Xrm.Page.getAttribute("tss_season").setRequiredLevel("required");
            Xrm.Page.getControl("tss_season").setVisible(true);
            Xrm.Page.getControl("tss_season").setDisabled(false);
        }
    }
    else {
        Xrm.Page.getAttribute("tss_issue").setRequiredLevel("none");
        Xrm.Page.getControl("tss_issue").setVisible(false);
        Xrm.Page.getControl("tss_issue").setDisabled(true);
        Xrm.Page.getAttribute("tss_issue").setValue(null);
        Xrm.Page.getAttribute("tss_issue").setSubmitMode("always");

        Xrm.Page.getAttribute("tss_season").setRequiredLevel("none");
        Xrm.Page.getControl("tss_season").setVisible(false);
        Xrm.Page.getControl("tss_season").setDisabled(true);
        Xrm.Page.getAttribute("tss_season").setValue(null);
        Xrm.Page.getAttribute("tss_season").setSubmitMode("always");
    }
}