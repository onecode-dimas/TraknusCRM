var formType = Xrm.Page.ui.getFormType();
var formStatus = {
    Undefined: 0,
    Create: 1,
    Update: 2,
    ReadOnly: 3,
    Disabled: 4,
    QuickCreate: 5,
    BulkEdit: 6,
    ReadOptimized: 11
};

//Function
function UnitGroupMarketSize_OnLoad()
{
    
}

function UnitGroup_OnChange()
{
    var oUnitGroup = Xrm.Page.getAttribute('tss_unitgroup').getValue();

    if (oUnitGroup != null) {
        Xrm.Page.getControl("tss_model").setDisabled(false);
        Xrm.Page.getAttribute("tss_model").setRequiredLevel("required");

        UnitGroup_PreFilter();
    }
    else
    {
        Xrm.Page.getControl("tss_model").setDisabled(true);
        Xrm.Page.getAttribute("tss_model").setRequiredLevel("none");
        Xrm.Page.getAttribute("tss_model").setValue(null);
    }
}

function UnitGroup_PreFilter()
{
    Xrm.Page.getControl("tss_model")._control && Xrm.Page.getControl("tss_model")._control.tryCompleteOnDemandInitialization && Xrm.Page.getControl("tss_model")._control.tryCompleteOnDemandInitialization();
    Xrm.Page.getControl("tss_model").addPreSearch(function () {
        var oUnitGroup = Xrm.Page.getAttribute("tss_unitgroup");
        var oUnitGroupObj = oUnitGroup.getValue();

        if (oUnitGroupObj != null) {
            var fetchFilters = "<filter type='and'>" +
                "<condition attribute='defaultuomscheduleid' uitype='" + oUnitGroupObj[0].entityType + "' operator='eq' value='" + oUnitGroupObj[0].id + "'/>" +
                "<condition attribute='producttypecode' operator='eq' value='1'/></filter>";
            Xrm.Page.getControl("tss_model").addCustomFilter(fetchFilters);
        }
    });
}