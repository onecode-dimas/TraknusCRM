function OnLoad() {
    //refresh ribbon
    refreshRibbonOnChange();
}

function refreshRibbonOnChange() {
    Xrm.Page.ui.refreshRibbon();

}

function onChangeRefresh() {
    Xrm.Page.data.save();
}

//function EnableDisableRibbon_ApproveSalesEffort(grid){
function EnableDisableRibbon_ApproveSalesEffort() {

    //var approverAllSalesEffort = Xrm.Page.getAttribute("tss_approveallsaleseffort").getValue(); 

    if (CheckUserPrivilegeApprove() == true) {
        return true;
    }
    else {
        return false;
    }

    //refresh subgrid
    //grid.refresh();
}

function CheckUserPrivilegeApprove() {

    var flag = true;

    var lookupObject = Xrm.Page.getAttribute("tss_sopartheaderid");
    if (lookupObject != null) {
        var lookUpObjectValue = lookupObject.getValue();
        if ((lookUpObjectValue != null)) {
            var lookupid = lookUpObjectValue[0].id;
        }
    }

    //var soId = Xrm.Page.data.entity.getId();
    var UserID = Xrm.Page.context.getUserId();

    XrmServiceToolkit.Rest.RetrieveMultiple(
    "tss_approverlistSet",
    "$select=*&$filter=tss_SalesOrderPartHeaderId/Id eq (guid'" + lookupid + "') and tss_Approver/Id eq (guid'" + UserID + "')",
    function (results) {
        if (results.length == 0) {
            flag = false;
        }
    },
    function (error) {
        alert('Retrieve Approval List: ' + error.message);
    },
    function onComplete() {
        //alert(" records should have been retrieved.");
    },
    false
    );

    if (flag) {
        return true;
    }
    else {
        return false;
    }
    //return flag;
}

function setApprovalStatus() {
    if (Xrm.Page.ui.getFormType() == 1) {
        Xrm.Page.getAttribute("tss_approvalstatus").setValue(1);
        Xrm.Page.getAttribute("tss_approvalstatus").setSubmitMode("always");
        Xrm.Page.getAttribute("tss_approvalstatus").setValue(0);
        Xrm.Page.getAttribute("tss_approvalstatus").setSubmitMode("always");
    }
}

function lockField() {
    
    if (Xrm.Page.getAttribute("tss_approvalstatus").getValue() == 1 && Xrm.Page.getAttribute("tss_approveby").getValue() != null) {
        Xrm.Page.getControl("tss_pss").setDisabled(true);
        Xrm.Page.getControl("tss_percentageeffort").setDisabled(true);
    }
}