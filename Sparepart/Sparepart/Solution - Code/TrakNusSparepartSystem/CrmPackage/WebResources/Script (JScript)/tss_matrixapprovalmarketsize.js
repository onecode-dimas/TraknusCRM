function onLoad(){
    if(Xrm.Page.ui.getFormType() == 1){
        Xrm.Page.getAttribute("tss_approvaltype").setRequiredLevel("required"); 
        Xrm.Page.getControl("tss_branch").setDisabled(true);
        Xrm.Page.getControl("tss_approver").setDisabled(true);
    }
}

function onChange_ApprovalType(){
    var obj = Xrm.Page.getAttribute("tss_approvaltype");
    if (obj != null) {
        var optionSetValue = obj.getValue();
        if(optionSetValue == 865920000 || optionSetValue == 865920001){
            //PDH
            Xrm.Page.getControl("tss_branch").setDisabled(false);
            Xrm.Page.getControl("tss_approver").setDisabled(true);
            Xrm.Page.getAttribute("tss_branch").setRequiredLevel("required"); 
            Xrm.Page.getAttribute("tss_approver").setRequiredLevel("required"); 
            Xrm.Page.getAttribute("tss_branch").setValue(null);
            Xrm.Page.getAttribute("tss_approver").setValue(null);  
        }
        else if(optionSetValue == 865920002){
            //DH
            Xrm.Page.getControl("tss_branch").setDisabled(true);
            Xrm.Page.getControl("tss_approver").setDisabled(false);
            Xrm.Page.getAttribute("tss_branch").setRequiredLevel("none"); 
            Xrm.Page.getAttribute("tss_branch").setValue(null);
            Xrm.Page.getAttribute("tss_approver").setValue(null);
        }
        
    }
}

function onChange_Branch(){
     var objBranch = Xrm.Page.getAttribute("tss_branch");
     var objApp = Xrm.Page.getAttribute("tss_approvaltype");

     if (objApp != null) {
         preFilterLookupApprover();
         Xrm.Page.getControl("tss_approver").setDisabled(false);
         Xrm.Page.getAttribute("tss_approver").setValue(null);  
     }
     if(objBranch.getValue() == null && Xrm.Page.getAttribute("tss_approvaltype").getValue != 865920002){
         Xrm.Page.getControl("tss_approver").setDisabled(true);
     }
}

function preFilterLookupApprover() {
    Xrm.Page.getControl("tss_approver").addPreSearch(function () {
        addLookupFilterApprover(Xrm.Page.getAttribute("tss_branch"), Xrm.Page.getAttribute("tss_approvaltype").getValue());
    });
}

function addLookupFilterApprover(branch, type) {
    if(type == 865920000){
         var fetchFilters = "<filter type='and'>" + 
            "<condition attribute='businessunitid' uitype='systemuser' operator='eq' value='" + branch.getValue()[0].id + "'/>" +
            "<filter type='or'>" +
            "<condition attribute='title' operator='eq' value='PDH'/>" +
            "<condition attribute='title' operator='eq' value='PDH Part'/></filter></filter>";
            Xrm.Page.getControl("tss_approver").addCustomFilter(fetchFilters);
    }
    else if(type == 865920001){
        var fetchFilters = "<filter type='and'>" + 
            "<condition attribute='businessunitid' uitype='systemuser' operator='eq' value='" + branch.getValue()[0].id + "'/></filter>";
            //"<filter type='or'>" +
            //"<condition attribute='title' operator='eq' value='PDH'/>" +
            //"<condition attribute='title' operator='eq' value='PDH Part'/></filter></filter>";
            Xrm.Page.getControl("tss_approver").addCustomFilter(fetchFilters);
    }
    else if(type == 865920002){
         var fetchFilters = "<filter>" + 
            "<condition attribute='title' uitype='systemuser' operator='eq' value='Manager'/></filter>";
         //var fetchFilters = "<filter type='and'>" + 
            //"<condition attribute='businessunitid' uitype='systemuser' operator='eq' value='" + branch + "'/></filter>";
            //"<filter type='or'>" +
            //"<condition attribute='title' operator='eq' value='PDH'/>" +
            //"<condition attribute='title' operator='eq' value='PDH Part'/></filter></filter>";
            Xrm.Page.getControl("tss_approver").addCustomFilter(fetchFilters);
    }
}