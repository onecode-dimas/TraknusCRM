function showToolsTransfer() {
    var parameters = {};
    // Get current recordId and set to parameter as lookup
    parameters["trs_tools"] = Xrm.Page.data.entity.getId();
    parameters["trs_toolsname"] = Xrm.Page.getAttribute("trs_toolsname").getValue();
    // Open form
    Xrm.Utility.openEntityForm("trs_toolstransfer", null, parameters);
}

function showToolsUsage() {
    var parameters = {};
    // Get current recordId and set to parameter as lookup
    parameters["trs_toolsmaster"] = Xrm.Page.data.entity.getId();
    parameters["trs_toolsmastername"] = Xrm.Page.getAttribute("trs_toolsname").getValue();
    // Set tools name equals to tools master name
    parameters["trs_toolsname"] = parameters["trs_toolsmastername"];
    // Open form
    Xrm.Utility.openEntityForm("trs_toolsusage", null, parameters);
}