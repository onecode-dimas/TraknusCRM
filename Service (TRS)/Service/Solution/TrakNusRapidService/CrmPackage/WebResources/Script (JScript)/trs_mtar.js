function SetAutomaticTimeManualTimeRequiredField() {
    try {
        var automatictime = Xrm.Page.getAttribute("trs_automatictime").getValue();
        if (automatictime == null) {
            Xrm.Page.getControl('trs_manualtime').setRequiredLevel("required");
        }
        else {
            Xrm.Page.getControl('trs_manualtime').setRequiredLevel("none");
        }
    }
    catch (e) {
        alert(e.Message);
    }
}

function preFilterLookupMechanic() {
    Xrm.Page.getControl("trs_mechanic").addPreSearch(addLookupFilter);
}
function addLookupFilter() {
    var mechanic = window.top.opener.Xrm.Page.getAttribute("resources").getValue();

    if (mechanic != null) {
        var cond = "";
        for (var I = 0; I < mechanic.length; I++) {
            cond += "<condition attribute='equipmentid' operator='eq' value='" + mechanic[I].id + "' />";
        }
        fetchXml = "<filter type='and'>" + "<filter type='or'>" + cond + "</filter>" + "</filter>";
        Xrm.Page.getControl("trs_mechanic").addCustomFilter(fetchXml);
    }
}

