function setMaintenance() {
    var type = Xrm.Page.getAttribute("trs_maintenanceperiod").getValue();
    if (type == true) {
        Xrm.Page.getControl("trs_maintenanceday").setRequiredLevel("required");
        Xrm.Page.getControl("trs_maintenanceday").setVisible(true);
    }
    else {
        Xrm.Page.getControl("trs_maintenanceday").setRequiredLevel("none");
        Xrm.Page.getControl("trs_maintenanceday").setVisible(false);
    }
}