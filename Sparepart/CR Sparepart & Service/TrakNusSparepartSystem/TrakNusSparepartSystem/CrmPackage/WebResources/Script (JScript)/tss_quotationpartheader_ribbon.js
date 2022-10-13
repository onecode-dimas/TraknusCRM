///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// Helper
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
var SourceTypes = {
    DirectSales: 865920000,
    MarketSize: 865920001,
    Service: 3
};
var QuotationStatus = {
    Active: 865920003
};
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// RIBBON AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function EnableDisableRibbon_AssigntoPSS() {
    try {
        var Return = false;
        var SourceType = Xrm.Page.getAttribute("tss_sourcetype").getValue();
        var PSS = Xrm.Page.getAttribute("tss_pss").getValue();
        var QuoteStatus = Xrm.Page.getAttribute("tss_statuscode").getValue();
        if (SourceType != null && PSS == null && QuotationStatus != null) {
            if (SourceType == SourceTypes.Service && QuoteStatus < QuotationStatus.Active) {
                Return = true;
            }
            else {
                Return = false;
            }
        }
        else {
            Return = false;
        }

        var userid = Xrm.Page.context.getUserId();
        var entityid = Xrm.Page.data.entity.getId();
        if (userid != null && entityid != null) {
            XrmServiceToolkit.Rest.RetrieveMultiple("tss_approverlistSet", "?$select=tss_approverlistId&$filter=tss_Approver/Id eq (guid'" + userid + "') and tss_QuotationPartHeaderId/Id eq (guid'" + entityid + "')", function (results) {
                if (results.length > 0) {
                    Return = true;
                }
                else {
                    Return = false;
                }
            }, function (error) {
                Xrm.Utility.alertDialog(error.message);
            }, function () {
                //On Complete - Do Something
            }, false);
        }

        return Return;
    }
    catch (e) {
        alert("EnableDisableRibbon_AssigntoPSS : " + e.message);
    }
}


// Function to load external script
function loadExternalScript(url) {
    var x = new ActiveXObject("Msxml2.XMLHTTP");
    x.open("GET", url, false);
    x.send(null);
    window.execScript(x.responseText);
}

function EnableDisableRibbon_ApproveTop() {
    //    try
    //    {
    var WAITING_APPROVAL_TOP = 865920001;
    var statusReason = Xrm.Page.getAttribute("tss_statusreason").getValue();
    var requestNewTop = Xrm.Page.getAttribute("tss_requestnewtop").getValue();
    var approveNewTop = Xrm.Page.getAttribute("tss_approvenewtop").getValue();
    if (requestNewTop != null && approveNewTop == false && statusReason == WAITING_APPROVAL_TOP) {
        return true;
    }
    else {
        return false;
    }

    /*var formLabel;
    var currForm = Xrm.Page.ui.formSelector.getCurrentItem();
    //var currForm = Xrm.Page.ui.formSelector.items[0].navigate;
    if (currForm != null)
    {
        //var currForm = Xrm.Page.ui.formSelector.items.get();
        formLabel = currForm.getLabel();
        if (formLabel == "Manager")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    else
    {
        alert("Current form is unavailable");
    }*/

    //loadExternalScript("/TraktorNusantara/WebResources/tss_quoationpartheader_form_information"); //form information
    //loadExternalScript("/TraktorNusantara/WebResources/tss_quoationpartheader_form_manager"); //form information

    /*set_formname();
    if (formName != null)
    {
        if (formName == 'Manager')
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    else
    {
        alert('Form Name Empty');
    }*/

    /*var formLabel;
    var selector = Xrm.Page.ui.formSelector;
    if (selector != null)
    {
        var currForm = selector.getCurrentItem();
        if (currForm != null)
        {
            formLabel = currForm.getLabel();
            if (formLabel == "Manager")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }*/
    //    }
    //    catch (e)
    //    {
    //        alert("EnableDisableRibbon_ApproveTop : " + e.message);
    //    }
}

function checkRDD() {
    if (Xrm.Page.getAttribute("tss_requestdeliverydate").getValue() == null) return false;
    return true;
}