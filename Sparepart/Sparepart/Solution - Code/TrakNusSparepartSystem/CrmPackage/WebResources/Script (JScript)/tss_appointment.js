function setDisabledField(field, value) {
    Xrm.Page.getControl(field).setDisabled(value);
}

function DisableField() {
    if (Xrm.Page.ui.getFormType() > 1 && Xrm.Page.getAttribute("statecode").getValue() != 0) {
        setDisabledField("tss_customer", true);
        setDisabledField("requiredattendees", true);
        setDisabledField("optionalattendees", true);
        setDisabledField("subject", true);
        setDisabledField("location", true);
        setDisabledField("regardingobjectid", true);
        setDisabledField("scheduledstart", true);
        setDisabledField("scheduledend", true);
        setDisabledField("isalldayevent", true);
        setDisabledField("scheduleddurationminutes", true);
        setDisabledField("description", true);
    }
}

function DisableForm() {
    if (Xrm.Page.ui.getFormType() > 1 && Xrm.Page.getAttribute("statecode").getValue() != 0) {
        Xrm.Page.ui.controls.forEach(function (control, index) {
            control.setDisabled(true);
        });
    }
}