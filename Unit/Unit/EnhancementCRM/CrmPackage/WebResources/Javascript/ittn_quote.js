function ExecuteAction(actionName, confirmMessage, successMessage, successCallback, failedCallback) {
    //var _return = window.confirm(confirmMessage);

    //if (_return) {
    //    var result = null;
    //    var recordID = Xrm.Page.data.entity.getId();

    //    var parameters = {
    //        "RecordID": recordID
    //    };

    //    // Creating the Odata Endpoint
    //    var oDataPath = Xrm.Page.context.getClientUrl() + "/api/data/v8.0/";
    //    var req = new XMLHttpRequest();
    //    req.open("POST", oDataPath + actionName, true);
    //    req.setRequestHeader("Accept", "application/json");
    //    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    //    req.onreadystatechange = function () {
    //        if (this.readyState == 4) {
    //            req.onreadystatechange = null;
    //            if (this.status == 200 || this.status == 204) {
    //                //Xrm.Utility.alertDialog(successMessage);
    //                //location.reload();
    //                //AssignResponse(req, 'Successfully executed Generate Prospect Part Header.', successCallback, failedCallback);
    //                if (successCallback !== undefined && typeof successCallback === "function") {
    //                    successCallback();
    //                }
    //            }
    //            else {
    //                var error = "";
    //                if (this.response != null) {
    //                    error = JSON.parse(this.response).error.message;
    //                }
    //                Xrm.Utility.alertDialog('Fail to Execute.\r\nResponse Code : (' + req.status + ')' + "\r\nPlease contact your IT support.\r\nTechnical Details : " + error);
    //            }
    //        }
    //    };

    //    req.send(JSON.stringify(parameters));
    //}

    var result = null;
    var recordID = Xrm.Page.data.entity.getId();

    var parameters = {
        "RecordID": recordID
    };

    // Creating the Odata Endpoint
    var oDataPath = Xrm.Page.context.getClientUrl() + "/api/data/v8.0/";
    var req = new XMLHttpRequest();
    req.open("POST", oDataPath + actionName, true);
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.onreadystatechange = function () {
        if (this.readyState == 4) {
            req.onreadystatechange = null;
            if (this.status == 200 || this.status == 204) {
                //Xrm.Utility.alertDialog(successMessage);
                //location.reload();
                //AssignResponse(req, 'Successfully executed Generate Prospect Part Header.', successCallback, failedCallback);
                if (successCallback !== undefined && typeof successCallback === "function") {
                    successCallback();
                }
            }
            else {
                var error = "";
                if (this.response != null) {
                    error = JSON.parse(this.response).error.message;
                }
                Xrm.Utility.alertDialog('Fail to Execute.\r\nResponse Code : (' + req.status + ')' + "\r\nPlease contact your IT support.\r\nTechnical Details : " + error);
            }
        }
    };

    req.send(JSON.stringify(parameters));
}

function RetrieveOData(path) {
    try {
        var serverUrl = "http://" + window.location.host + "/" + Xrm.Page.context.getOrgUniqueName();

        // Creating the Odata Endpoint
        var oDataPath = serverUrl + "/XRMServices/2011/OrganizationData.svc";
        var retrieveReq = new XMLHttpRequest();
        var OdataPopulation = oDataPath + path;
        retrieveReq.open("GET", OdataPopulation, false);
        retrieveReq.setRequestHeader("Accept", "application/json");
        retrieveReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        //retrieveReq.onreadystatechange = function () { retrieveReqCallBack(this); };
        retrieveReq.send();
        return retrieveReq;
    }
    catch (e) {
        throw new Error("RetrieveOData : Failed to retrieve OData !");
    }
}

function GetAttributeValue(_attribute) {
    var _result = null;

    if (_attribute != null) {
        if (_attribute.getValue() != null) {
            _result = _attribute.getValue();
        }
    }

    return _result;
}

function Form_ForceClose() {
    var attributes = Xrm.Page.data.entity.attributes.get();
    for (var i in attributes)
    { attributes[i].setSubmitMode("never"); }

    if (parent.opener != undefined) { window.parent.close(); } else Xrm.Page.ui.close();
}

function OnLoad() {
    debugger;
    var formName = Xrm.Page.ui.formSelector.getCurrentItem().getLabel();
    var _statecode = GetAttributeValue(Xrm.Page.getAttribute("statecode"));
    //ShowGridConditionType();
    if (formName == 'Information')
    {
        CheckConditionType();
    }
    else {
        if (_statecode == 0) {
            SetTotalCostUnit();
        }
    }
    
    if (_statecode == 0) {
        //SetUnderMinPrice();
        UPDATE_DeliveryTerms();
    }

    Xrm.Page.ui.refreshRibbon();
}

function OnSave() {
    //if (_isaddpersonaldiscount) {
    //    _isaddpersonaldiscount = false;

    //    var actionName = 'new_ITTNActionQuoteAddPersonalDiscount';
    //    var confirmMessage = 'Do you want to Add Personal Discount?';
    //    var successMessage = 'Successfully executed Add Personal Discount.';

    //    ExecuteAction(actionName, confirmMessage, successMessage, function () { Xrm.Page.data.refresh(true); });
    //}

    //if (_isaddfat) {
    //    _isaddfat = false;

    //    var actionName = 'new_ITTNActionQuoteAddFAT';
    //    var confirmMessage = 'Do you want to Add FAT?';
    //    var successMessage = 'Successfully executed Add FAT.';

    //    ExecuteAction(actionName, confirmMessage, successMessage, function () { Xrm.Page.data.refresh(true); });
    //}

    //if (_isaddvoucher) {
    //    _isaddvoucher = false;

    //    var actionName = 'new_ITTNActionQuoteAddVoucher';
    //    var confirmMessage = 'Do you want to Add Voucher?';
    //    var successMessage = 'Successfully executed Add Voucher.';

    //    ExecuteAction(actionName, confirmMessage, successMessage, function () { Xrm.Page.data.refresh(true); });
    //}

    //if (_isaddothers) {
    //    _isaddothers = false;

    //    var actionName = 'new_ITTNActionQuoteAddOthers';
    //    var confirmMessage = 'Do you want to Add Others?';
    //    var successMessage = 'Successfully executed Add Others.';

    //    ExecuteAction(actionName, confirmMessage, successMessage, function () { Xrm.Page.data.refresh(true); });
    //}

}

function ShowGridConditionType() {
    if (Xrm.Page.ui.tabs.get("tab_12") != null) {
        try {
            var recordID = Xrm.Page.data.entity.getId();
            var path = "/ittn_quoteconditiontypeSet?$select=ittn_quoteconditiontypeId&$filter=ittn_Quote/Id eq (guid'" + recordID + "')";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved != null && retrieved.results.length > 0) {
                    Xrm.Page.ui.tabs.get("tab_12").setVisible(true);
                }
                else {
                    Xrm.Page.ui.tabs.get("tab_12").setVisible(false);
                }
            }
        }
        catch (e) {
            throw new Error("SetActType : " + e.Message);
        }
    }
}

function CheckConditionType() {
    
    // -------------------- -------------------- -------------------- -------------------- --------------------
    // PERSONAL DISCOUNT
    var _ispersonaldiscountcentralized = GetAttributeValue(Xrm.Page.getAttribute("ittn_ispersonaldiscountcentralized"));

    if (_ispersonaldiscountcentralized) {
        Xrm.Page.ui.tabs.get("tab_14").sections.get("tab_14_section_2").setVisible(true);
        Xrm.Page.getAttribute("ittn_personaldiscountamount").setRequiredLevel("required");
    }
    else if (!_ispersonaldiscountcentralized) {
        Xrm.Page.ui.tabs.get("tab_14").sections.get("tab_14_section_2").setVisible(false);
        Xrm.Page.getAttribute("ittn_personaldiscountamount").setRequiredLevel("none");

        Xrm.Page.getAttribute("ittn_personaldiscountamount").setValue(null);
        Xrm.Page.getAttribute("ittn_personaldiscountamount").setSubmitMode("always");
    }
    // -------------------- -------------------- -------------------- -------------------- --------------------

    // -------------------- -------------------- -------------------- -------------------- --------------------
    // FAT
    var _isfatcentralized = GetAttributeValue(Xrm.Page.getAttribute("ittn_isfatcentralized"));

    if (_isfatcentralized) {
        Xrm.Page.ui.tabs.get("tab_14").sections.get("tab_14_section_1").setVisible(true);
        Xrm.Page.getAttribute("ittn_fatamount").setRequiredLevel("required");
    }
    else if (!_isfatcentralized) {
        Xrm.Page.ui.tabs.get("tab_14").sections.get("tab_14_section_1").setVisible(false);
        Xrm.Page.getAttribute("ittn_fatamount").setRequiredLevel("none");

        Xrm.Page.getAttribute("ittn_fatamount").setValue(null);
        Xrm.Page.getAttribute("ittn_fatamount").setSubmitMode("always");
    }
    // -------------------- -------------------- -------------------- -------------------- --------------------

    // -------------------- -------------------- -------------------- -------------------- --------------------
    // VOUCHER
    var _isvouchercentralized = GetAttributeValue(Xrm.Page.getAttribute("ittn_isvouchercentralized"));

    if (_isvouchercentralized) {
        Xrm.Page.ui.tabs.get("tab_14").sections.get("tab_14_section_3").setVisible(true);
        Xrm.Page.getAttribute("ittn_voucheramount").setRequiredLevel("required");
    }
    else if (!_isvouchercentralized) {
        Xrm.Page.ui.tabs.get("tab_14").sections.get("tab_14_section_3").setVisible(false);
        Xrm.Page.getAttribute("ittn_voucheramount").setRequiredLevel("none");

        Xrm.Page.getAttribute("ittn_voucheramount").setValue(null);
        Xrm.Page.getAttribute("ittn_voucheramount").setSubmitMode("always");
    }
    // -------------------- -------------------- -------------------- -------------------- --------------------

    // -------------------- -------------------- -------------------- -------------------- --------------------
    // OTHERS
    var _isotherscentralized = GetAttributeValue(Xrm.Page.getAttribute("ittn_isotherscentralized"));

    if (_isotherscentralized) {
        Xrm.Page.ui.tabs.get("tab_14").sections.get("tab_14_section_4").setVisible(true);
        Xrm.Page.getAttribute("ittn_othersamount").setRequiredLevel("required");
    }
    else if (!_isotherscentralized) {
        Xrm.Page.ui.tabs.get("tab_14").sections.get("tab_14_section_4").setVisible(false);
        Xrm.Page.getAttribute("ittn_othersamount").setRequiredLevel("none");

        Xrm.Page.getAttribute("ittn_othersamount").setValue(null);
        Xrm.Page.getAttribute("ittn_othersamount").setSubmitMode("always");
    }
    // -------------------- -------------------- -------------------- -------------------- --------------------
    
}

function SetUnderMinPrice() {
    var _underminprice_field = "new_underminprice";
    var _reqapproveminpricedate = GetAttributeValue(Xrm.Page.getAttribute("ittn_reqapproveminpricedate"));
    var _activatelock = GetAttributeValue(Xrm.Page.getAttribute("new_activatelock"));

    if (_reqapproveminpricedate != null) {
        Xrm.Page.getAttribute(_underminprice_field).setValue(true);
        Xrm.Page.getAttribute(_underminprice_field).setSubmitMode("always");
        Xrm.Page.getAttribute(_underminprice_field).fireOnChange();
        Xrm.Page.data.entity.save();
    }
}

function SetTotalCostUnit() {
    var recordID = Xrm.Page.data.entity.getId();
    var path = "/QuoteDetailSet?$select=ittn_MovingPrice&$filter=QuoteId/Id eq (guid'" + recordID + "') and ( ittn_MovingPrice ne null and ittn_MovingPrice/Value gt 0)";
    var retrieveReq = RetrieveOData(path);
    if (retrieveReq.readyState == 4 /* complete */) {
        var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
        if (retrieved != null && retrieved.results.length > 0) {
            var _totalamount = GetAttributeValue(Xrm.Page.getAttribute("totalamount"));
            var _totalcostunit = parseFloat(0);
            var _gp = 0;

            var i;
            for (i = 0; i < retrieved.results.length; i++) {
                _totalcostunit += parseFloat(retrieved.results[i].ittn_MovingPrice.Value);
            }

            _gp = (1 - (_totalcostunit / _totalamount)) * 100;

            Xrm.Page.getAttribute("ittn_totalcostunit").setValue(_totalcostunit);
            Xrm.Page.getAttribute("ittn_totalcostunit").setSubmitMode("always");

            Xrm.Page.getAttribute("ittn_gp").setValue(_gp);
            Xrm.Page.getAttribute("ittn_gp").setSubmitMode("always");
        }
        else {
            Xrm.Page.getAttribute("ittn_totalcostunit").setValue(0);
            Xrm.Page.getAttribute("ittn_totalcostunit").setSubmitMode("always");

            Xrm.Page.getAttribute("ittn_gp").setValue(0);
            Xrm.Page.getAttribute("ittn_gp").setSubmitMode("always");
        }
    }
    else {
        Xrm.Page.getAttribute("ittn_totalcostunit").setValue(0);
        Xrm.Page.getAttribute("ittn_totalcostunit").setSubmitMode("always");

        Xrm.Page.getAttribute("ittn_gp").setValue(0);
        Xrm.Page.getAttribute("ittn_gp").setSubmitMode("always");
    }

    Xrm.Page.data.entity.save();
}

function CheckStock() {
    var actionName = 'new_ITTNActionQuoteCheckStock';
    var confirmMessage = 'Do you want to Check Stock?';
    var successMessage = 'Successfully executed Check Stock.';

    ExecuteAction(actionName, confirmMessage, successMessage, function () { Xrm.Page.data.refresh(true); });
}

function Count_QuoteConditionTypeNeedToApprove() {
    var _result = 0;

    var recordID = Xrm.Page.data.entity.getId();
    //var path = "/ittn_quoteconditiontypeSet?$select=ittn_quoteconditiontypeId&$filter=ittn_Quote/Id eq (guid'" + recordID + "') and ittn_NeedApproveConditionType eq true and ittn_statusreason/Value eq 841150003";
    var path = "/ittn_quoteconditiontypeSet?$select=ittn_quoteconditiontypeId&$filter=ittn_Quote/Id eq (guid'" + recordID + "') and ittn_statusreason/Value eq 841150003";
    var retrieveReq = RetrieveOData(path);
    if (retrieveReq.readyState == 4 /* complete */) {
        var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
        if (retrieved != null) {
            _result = retrieved.results.length;
        }
    }

    return _result;
}

function User_IsApprover() {
    var _flag = false;

    var recordid = Xrm.Page.data.entity.getId();
    var userid = Xrm.Page.context.getUserId();
    var path = "/ittn_approvallistquoteminpriceSet?$filter=ittn_Quote/Id eq (guid'" + recordid + "') and ittn_Approver/Id eq (guid'" + userid + "')";
    var retrieveReq = RetrieveOData(path);
    if (retrieveReq.readyState == 4 /* complete */) {
        var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
        if (retrieved != null && retrieved.results.length > 0) {
            _flag = true;
        }
    }

    return _flag;
}

function User_IsApproverDIC() {
    var _flag = false;

    var recordid = Xrm.Page.data.entity.getId();
    var userid = Xrm.Page.context.getUserId();
    var path = "/ittn_approvallistquotedicSet?$filter=ittn_Quote/Id eq (guid'" + recordid + "') and ittn_Approver/Id eq (guid'" + userid + "')";
    var retrieveReq = RetrieveOData(path);
    if (retrieveReq.readyState == 4 /* complete */) {
        var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
        if (retrieved != null && retrieved.results.length > 0) {
            _flag = true;
        }
    }

    return _flag;
}

function User_IsApprover_PersonalDiscount() {
    //var _flag = false;
    //var userid = Xrm.Page.context.getUserId();

    //var path = "/ittn_matrixapprovalconditiontypeSet?$select=ittn_Approver,ittn_matrixapprovalconditiontypeId&$filter=ittn_matrixapprovalconditiontypeId eq (guid'123123')";
    //var retrieveReq = RetrieveOData(path);
    //if (retrieveReq.readyState == 4 /* complete */) {
    //    var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
    //    if (retrieved != null && retrieved.results.length > 0) {
    //        _flag = true;
    //    }
    //}

    //return _flag;
}

function User_IsApprover_FAT() {

}

function User_IsApprover_Voucher() {

}

function User_IsApprover_Others() {

}

// -------------------- -------------------- -------------------- -------------------- --------------------

function OnChange_IsPersonalDiscountCentralized(_event) {
    var _ispersonaldiscountcentralized = GetAttributeValue(Xrm.Page.getAttribute("ittn_ispersonaldiscountcentralized"));

    if (_ispersonaldiscountcentralized) {
        Xrm.Page.getAttribute("ittn_personaldiscountamount").setRequiredLevel("required");
        Xrm.Page.getControl("ittn_personaldiscountamount").setVisible(true);
    }
    else
    {
        Xrm.Page.getAttribute("ittn_personaldiscountamount").setRequiredLevel("none");
        Xrm.Page.getControl("ittn_personaldiscountamount").setVisible(false);
    }
}

function OnChange_IsFATCentralized(_event) {
    var _isfatcentralized = GetAttributeValue(Xrm.Page.getAttribute("ittn_isfatcentralized"));

    if (_isfatcentralized) {
        Xrm.Page.getAttribute("ittn_fatamount").setRequiredLevel("required");
        Xrm.Page.getControl("ittn_fatamount").setVisible(true);
    }
    else {
        Xrm.Page.getAttribute("ittn_fatamount").setRequiredLevel("none");
        Xrm.Page.getControl("ittn_fatamount").setVisible(false);
    }
}

function OnChange_IsVoucherCentralized(_event) {
    var _isvouchercentralized = GetAttributeValue(Xrm.Page.getAttribute("ittn_isvouchercentralized"));

    if (_isvouchercentralized) {
        Xrm.Page.getAttribute("ittn_voucheramount").setRequiredLevel("required");
        Xrm.Page.getControl("ittn_voucheramount").setVisible(true);
    }
    else {
        Xrm.Page.getAttribute("ittn_voucheramount").setRequiredLevel("none");
        Xrm.Page.getControl("ittn_voucheramount").setVisible(false);
    }
}

function OnChange_IsOthersCentralized(_event) {
    var _isotherscentralized = GetAttributeValue(Xrm.Page.getAttribute("ittn_isotherscentralized"));

    if (_isotherscentralized) {
        Xrm.Page.getAttribute("ittn_othersamount").setRequiredLevel("required");
        Xrm.Page.getControl("ittn_othersamount").setVisible(true);
    }
    else {
        Xrm.Page.getAttribute("ittn_othersamount").setRequiredLevel("none");
        Xrm.Page.getControl("ittn_othersamount").setVisible(false);
    }
}

// -------------------- -------------------- -------------------- -------------------- --------------------

function AcceptQuoteOrCreateOrder() {
    var _shippingpoint_notexist = CheckShippingPoint_NotExist();
    //var _shippingpoint_equalzero = CheckShippingPoint_EqualZero();

    //if (_shippingpoint_notexist > 0) {
    //    alert("There are " + _shippingpoint_notexist.toString() + " Quotation Details do not have Shipping Point !");
    //    return;
    //}
    //else if (_shippingpoint_equalzero > 0) {
    //    alert("There are " + _shippingpoint_equalzero.toString() + " Quotation Details have Shipping Point Amount equal 0 !");
    //    return;
    //}
    //else
    if (_shippingpoint_notexist == 0) {
        var $v_0 = Xrm.Page.data.entity.attributes.get('statecode');

        if (!$v_0) {
            return;
        }

        var $v_1 = $v_0.getValue();

        if ($v_1 === 1) {
            AcceptQuote();
        }
        if ($v_1 === 2) {
            CreateOrder();
        }
    }
}

function AcceptQuote() {
    if (!Xrm.Page.data.getIsValid()) {
        return;
    }

    if (Xrm.Page.context.client.getClient() === 'Outlook') {
        if (!ConfirmCreateOrder()) {
            return;
        }
    }

    var $v_0 = Xrm.Page.data.entity.attributes.get('opportunityid').getValue();
    var $v_1 = ($v_0) ? $v_0[0].id : '';

    if (Xrm.Page.context.client.getClient() !== 'Mobile' || !Mscrm.InternalUtilities.DialogUtility.isMDDEnabled()) {
        var $v_2 = {};
        $v_2['quoteNumber'] = Xrm.Page.data.entity.attributes.get('quotenumber').getValue();
        $v_2['revisionNumber'] = Xrm.Page.data.entity.attributes.get('revisionnumber').getValue();
        $v_2['opportunityID'] = $v_1;
        var $v_3 = new Xrm.DialogOptions();
        $v_3.height = 500;
        $v_3.width = 475;
        var $v_4 = Mscrm.GlobalImported.CrmUri.create('/sfa/quotes/dlg_accept.aspx?QuoteId=' + CrmEncodeDecode.CrmUrlEncode(Xrm.Page.data.entity.getId()) + '&opportunityid=' + CrmEncodeDecode.CrmUrlEncode($v_1));
        Xrm.Internal.openDialog($v_4.toString(), $v_3, $v_2, null, performActionAfterAcceptQuote);
    }
    else {
        if (Mscrm.InternalUtilities.JSTypes.isNullOrEmptyString($v_1)) {
            $3(Xrm.Page.data.entity.getId(), $v_1, false);
        }
        else {
            var $v_5 = 2;
            Xrm.Internal.messages.canCloseOpportunity(new Microsoft.Crm.Client.Core.Framework.Guid($v_1), new Microsoft.Crm.Client.Core.Framework.Guid(Xrm.Page.data.entity.getId()), $v_5).then(function ($p1_0) {
                $3(Xrm.Page.data.entity.getId(), $v_1, ($p1_0).canClose);
            }, Mscrm.InternalUtilities.ClientApiUtility.actionFailedCallback);
        }
    }
}

function CreateOrder() {
    if (!Xrm.Page.data.getIsValid()) {
        return;
    }

    if (Xrm.Page.context.client.getClient() === 'Outlook') {
        if (!ConfirmCreateOrder()) {
            return;
        }
    }

    var $v_0 = Microsoft.Crm.Client.Core.Storage.Common.AllColumns.get_instance();

    Xrm.Internal.messages.createOrder(new Microsoft.Crm.Client.Core.Framework.Guid(Xrm.Page.data.entity.getId()), $v_0).then(function ($p1_0) {
        var $v_1 = ($p1_0).entity;
        var $v_2 = $v_1.get_identifier().Id.toString();
        if (!Mscrm.InternalUtilities.JSTypes.isNullOrEmptyString($v_2)) {
            Xrm.Utility.openEntityForm('salesorder', $v_2);
        }
    }, Mscrm.InternalUtilities.ClientApiUtility.actionFailedCallback);
}

function ConfirmCreateOrder() {
    try {
        if (Xrm.Page.context.client.getClientState() === 'Offline') {
            return confirm(Xrm.Internal.getResourceString('LOCID_OFFLINE_CRE_ORD_FROM_QUO'));
        }
        else {
            return true;
        }
    }
    catch ($v_0) {
        throw $v_0;
    }
}

function CheckShippingPoint_NotExist() {
    var _result = 0;
    var _recordid = Xrm.Page.data.entity.getId();

    var path = "/QuoteDetailSet?$select=QuoteDetailId&$filter=QuoteId/Id eq (guid'" + _recordid + "') and ( ittn_ShippingPoint eq null and ittn_DeliveryPlanBranch eq null and new_ParentNumber eq null)";
    var retrieveReq = RetrieveOData(path);
    if (retrieveReq.readyState == 4 /* complete */) {
        var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
        if (retrieved != null && retrieved.results.length > 0) {
            _result = retrieved.results.length;
        }
    }

    return _result;
}

function CheckShippingPoint_EqualZero() {
    var _result = 0;
    var _recordid = Xrm.Page.data.entity.getId();

    var path = "/ittn_quoteconditiontypeSet?$select=ittn_quoteconditiontypeId&$filter=ittn_Quote/Id eq (guid'" + _recordid + "') and ittn_TotalAmount/Value eq 0";
    var retrieveReq = RetrieveOData(path);
    if (retrieveReq.readyState == 4 /* complete */) {
        var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
        if (retrieved != null && retrieved.results.length > 0) {
            _result = retrieved.results.length;
        }
    }

    return _result;
}

function IsShippingPointValid(_context) {
    var _result = false;
    var _finalquotation = GetAttributeValue(Xrm.Page.getAttribute("new_finalquotation"));
    var _count_notexist = CheckShippingPoint_NotExist();
    //var _count_shippingpointequalzero = CheckShippingPoint_EqualZero();
    var _eventargs = _context.getEventArgs();

    if (_finalquotation == true) {
        if (_count_notexist > 0) {
            //Xrm.Utility.alertDialog("There are " + _count_notexist.toString() + " Quotation Details do not have Shipping Point / Delivery Plan Branch !");
            Xrm.Utility.alertDialog("There is not shipping information in this Quote !");

            if (_eventargs != null) {
                _eventargs.preventDefault();
            }

            return;
        }
        //else if (_count_shippingpointequalzero > 0) {
            //Xrm.Utility.alertDialog("There are " + _count_shippingpointequalzero.toString() + " Quotation Details have Shipping Point / Delivery Plan Branch Amount equal 0 !");

            //if (_eventargs != null) {
            //    _eventargs.preventDefault();
            //}

            //return;
        //}
        //else if (_count_notexist == 0 && _count_shippingpointequalzero == 0) {
            //_result = true;
        //}
        
        if (_count_notexist == 0) {
            _result = true;
        }
    }

    return _result;
}

// -------------------- -------------------- -------------------- -------------------- --------------------

function CustomRule_CreateOrder() {

}

function CustomRule_CreateOrder_Original() {
    var _result = false;

    return _result;
}

function CustomRule_CheckStock() {
    var _result = false;
    var _statuscode = GetAttributeValue(Xrm.Page.getAttribute("statuscode"));
    var _statecode = GetAttributeValue(Xrm.Page.getAttribute("statecode"));

    if (_statuscode != 4 && _statecode == 0) {
        _result = true;
    }

    return _result;
}

function CustomRule_ApproveQuote() {
    var _flag = false;

    // 841150000 // Draft
    // 841150001 // Waiting Approval Min Price
    // 841150002 // Approved
    // 841150006 // Waiting Approval DIC

    var _statusreason = GetAttributeValue(Xrm.Page.getAttribute("ittn_statusreason"));

    if (_statusreason == 841150001 && Count_QuoteConditionTypeNeedToApprove() == 0 && User_IsApprover()) {
        _flag = true;
    }

    return _flag;
}

function CustomRule_ApproveQuoteDIC() {
    var _flag = false;

    // 841150000 // Draft
    // 841150001 // Waiting Approval Min Price
    // 841150002 // Approved
    // 841150006 // Waiting Approval DIC

    var _statusreason = GetAttributeValue(Xrm.Page.getAttribute("ittn_statusreason"));

    if (_statusreason == 841150006 && Count_QuoteConditionTypeNeedToApprove() == 0 && User_IsApproverDIC()) {
        _flag = true;
    }

    return _flag;
}

function CustomRule_ActivateQuote() {
    var _flag = false;

    try {
        var _statusreason = GetAttributeValue(Xrm.Page.getAttribute("ittn_statusreason"));
        //if (_statusreason != 841150001 && _statusreason != 841150006 && Count_QuoteConditionTypeNeedToApprove() == 0 && CheckShippingPoint_NotExist() == 0 && CheckShippingPoint_EqualZero() == 0) {
        //    _flag = true;
        //}
        if (_statusreason != 841150001 && _statusreason != 841150006 && Count_QuoteConditionTypeNeedToApprove() == 0) {
            if (CheckShippingPoint_NotExist() == 0) {
                _flag = true;
            }
        }
    }
    catch (e) {
        throw new Error("SetActType : " + e.Message);
    }

    return _flag;
}

function ApproveQuote() {
    var _confirmMessage = 'Do you want to Approve Quote (Under Min Price)?';
    var _return = window.confirm(_confirmMessage);

    if (_return) {
        var _recordid = Xrm.Page.data.entity.getId();
        var _userid = null;
        var _minpricecurrentapprover = GetAttributeValue(Xrm.Page.getAttribute("ittn_minpricecurrentapprover"));

        var path = "/ittn_matrixapprovalquoteminpriceSet?$select=ittn_Approver&$filter=ittn_matrixapprovalquoteminpriceId eq (guid'" + _minpricecurrentapprover[0].id + "')";
        var retrieveReq = RetrieveOData(path);
        if (retrieveReq.readyState == 4 /* complete */) {
            var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
            if (retrieved != null && retrieved.results.length > 0) {
                _userid = retrieved.results[0].ittn_Approver.Id;
            }
        }

        var _today = new Date();
        _today.setDate(_today.getDate());

        var entity = {};
        entity.ittn_ApproveMinPriceBy = {
            Id: _userid,
            LogicalName: "systemuser"
        };
        entity.ittn_ApproveMinPriceDate = _today;
        entity.ittn_StatusReason = {
            Value: 841150002
        };
        entity.new_ActivateLock = {
            Value: 3
        };

        XrmServiceToolkit.Rest.Update(_recordid, entity, "QuoteSet", function () {
            //Success - No Return Data - Do Something
            alert("Successfully approved.");

            Xrm.Utility.openEntityForm(Xrm.Page.data.entity.getEntityName(), Xrm.Page.data.entity.getId());
            //Xrm.Page.data.refresh(true);
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, false);
    }
}

function ApproveQuoteDIC() {
    var _confirmMessage = 'Do you want to Approve Quote (DIC)?';
    var _return = window.confirm(_confirmMessage);

    if (_return) {
        var _recordid = Xrm.Page.data.entity.getId();
        var _userid = null;
        var _diccurrentapprover = GetAttributeValue(Xrm.Page.getAttribute("ittn_diccurrentapprover"));

        var path = "/ittn_matrixapprovalquotedicSet?$select=ittn_Approver&$filter=ittn_matrixapprovalquotedicId eq (guid'" + _diccurrentapprover[0].id + "')";
        var retrieveReq = RetrieveOData(path);
        if (retrieveReq.readyState == 4 /* complete */) {
            var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
            if (retrieved != null && retrieved.results.length > 0) {
                _userid = retrieved.results[0].ittn_Approver.Id;
            }
        }

        var _today = new Date();
        _today.setDate(_today.getDate());

        var entity = {};
        entity.ittn_ApproveDICBy = {
            Id: _userid,
            LogicalName: "systemuser"
        };
        entity.ittn_ApproveDICDate = _today;
        entity.ittn_StatusReason = {
            Value: 841150002
        };
        entity.new_ActivateLock = {
            Value: 3
        };

        XrmServiceToolkit.Rest.Update(_recordid, entity, "QuoteSet", function () {
            //Success - No Return Data - Do Something
            alert("Successfully approved.");

            Xrm.Utility.openEntityForm(Xrm.Page.data.entity.getEntityName(), Xrm.Page.data.entity.getId());
            //Xrm.Page.data.refresh(true);
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, false);
    }
}

function CustomRule_ChangeColor() {
    //var _result = CustomRule_ConditionTypeList();
    var _result = false;

    return _result;
}

function ChangeColor() {
    var actionName = 'new_ITTNActionQuoteChangeColor';
    var confirmMessage = 'Do you want to Change Color?';
    var successMessage = 'Successfully executed Change Color.';
    var _return = window.confirm(confirmMessage);

    if (_return) {
        ExecuteAction(actionName, confirmMessage, successMessage, function () { Xrm.Page.data.refresh(true); });
    }
}

// CUSTOM RULE : CONDITION LIST ( HEADER ) -------------------- -------------------- -------------------- -------------------- --------------------

function CustomRule_ConditionTypeList() {
    var _result = false;
    var _shippingpoint = CheckShippingPoint_NotExist();
    var _statuscode = GetAttributeValue(Xrm.Page.getAttribute("statuscode"));
    var _statecode = GetAttributeValue(Xrm.Page.getAttribute("statecode"));
    var _statusreason = GetAttributeValue(Xrm.Page.getAttribute("ittn_statusreason"));
    var _finalquotation = GetAttributeValue(Xrm.Page.getAttribute("new_finalquotation"));

    //if (_shippingpoint == 0 && _statuscode != 4 && _statecode == 0 && (_statusreason == 841150000 || _statusreason == 841150002) && _finalquotation == 1) {
        //_result = true;
    //}

    if (_shippingpoint == 0 && _statuscode != 4 && _statecode == 0 && (_statusreason == 841150000 || _statusreason == 841150002) && _finalquotation == 1) {
    _result = true;
    }

    return _result;
}

// CUSTOM RULE : PERSONAL DISCOUNT -------------------- -------------------- -------------------- -------------------- --------------------

function CustomRule_AddPersonalDiscount() {
    var _conditiontypecode = "ZCB0";
    var _result = !CheckConditionType_NeedApprove(_conditiontypecode) && !CheckConditionType_NeedApprovalRequest(_conditiontypecode) && !CheckConditionType_AlreadyActive(_conditiontypecode);
    
    return _result;
}

function CustomRule_RequestApprovalForPersonalDiscount() {
    var _conditiontypecode = "ZCB0";
    var _result = !CheckConditionType_NeedApprove(_conditiontypecode) && CheckConditionType_NeedApprovalRequest(_conditiontypecode);

    return _result;
}

function CustomRule_ApprovalPersonalDiscount() {
    var _conditiontypecode = "ZCB0";
    var _result = CheckConditionType_NeedApprove(_conditiontypecode);

    return _result;
}

// CUSTOM RULE : FAT -------------------- -------------------- -------------------- -------------------- --------------------

function CustomRule_AddFAT() {
    var _conditiontypecode = "ZFAT";
    var _result = !CheckConditionType_NeedApprove(_conditiontypecode) && !CheckConditionType_NeedApprovalRequest(_conditiontypecode) && !CheckConditionType_AlreadyActive(_conditiontypecode);

    return _result;
}

function CustomRule_RequestApprovalForFAT() {
    var _conditiontypecode = "ZFAT";
    var _result = !CheckConditionType_NeedApprove(_conditiontypecode) && CheckConditionType_NeedApprovalRequest(_conditiontypecode);

    return _result;
}

function CustomRule_ApprovalFAT() {
    var _conditiontypecode = "ZFAT";
    var _result = CheckConditionType_NeedApprove(_conditiontypecode);

    return _result;
}

// CUSTOM RULE : VOUCHER -------------------- -------------------- -------------------- -------------------- --------------------

function CustomRule_AddVoucher() {
    var _conditiontypecode = "ZVCR";
    var _result = !CheckConditionType_NeedApprove(_conditiontypecode) && !CheckConditionType_NeedApprovalRequest(_conditiontypecode) && !CheckConditionType_AlreadyActive(_conditiontypecode);

    return _result;
}

function CustomRule_RequestApprovalForVoucher() {
    var _conditiontypecode = "ZVCR";
    var _result = !CheckConditionType_NeedApprove(_conditiontypecode) && CheckConditionType_NeedApprovalRequest(_conditiontypecode);

    return _result;
}

function CustomRule_ApprovalVoucher() {
    var _conditiontypecode = "ZVCR";
    var _result = CheckConditionType_NeedApprove(_conditiontypecode);

    return _result;
}

// CUSTOM RULE : OTHERS -------------------- -------------------- -------------------- -------------------- --------------------

function CustomRule_AddOthers() {
    var _conditiontypecode = "ZO99";
    var _result = !CheckConditionType_NeedApprove(_conditiontypecode) && !CheckConditionType_NeedApprovalRequest(_conditiontypecode) && !CheckConditionType_AlreadyActive(_conditiontypecode);

    return _result;
}

function CustomRule_RequestApprovalForOthers() {
    var _conditiontypecode = "ZO99";
    var _result = !CheckConditionType_NeedApprove(_conditiontypecode) && CheckConditionType_NeedApprovalRequest(_conditiontypecode);

    return _result;
}

function CustomRule_ApprovalOthers() {
    var _conditiontypecode = "ZO99";
    var _result = CheckConditionType_NeedApprove(_conditiontypecode);

    return _result;
}

// -------------------- -------------------- -------------------- -------------------- --------------------

var _isaddpersonaldiscount = false;
var _isaddfat = false;
var _isaddvoucher = false;
var _isaddothers = false;

function ConditionTypeList() {
    // IF CONDITION TYPE BUTTON (HEADER) CLICKED
    // WHAT TO DO ?
}

function AddPersonalDiscount() {
    var formName = Xrm.Page.ui.formSelector.getCurrentItem().getLabel();

    if (formName == 'Information') {
        _isaddpersonaldiscount = true;

        //Xrm.Page.ui.tabs.get("tab_14").sections.get("tab_14_section_2").setVisible(true);

        //Xrm.Page.getAttribute("ittn_ispersonaldiscountcentralized").setValue(false);
        //Xrm.Page.getAttribute("ittn_ispersonaldiscountcentralized").setSubmitMode("always");
        //Xrm.Page.getControl("ittn_ispersonaldiscountcentralized").setDisabled(false);

        //Xrm.Page.getControl("ittn_personaldiscountamount").setVisible(false);
        ////Xrm.Page.getAttribute("ittn_personaldiscountamount").setRequiredLevel("required");
        ////Xrm.Page.getControl("ittn_personaldiscountamount").setDisabled(false);

        if (_isaddpersonaldiscount) {
            _isaddpersonaldiscount = false;

            var actionName = 'new_ITTNActionQuoteAddPersonalDiscount';
            var confirmMessage = 'Do you want to Add Personal Discount?';
            var successMessage = 'Successfully executed Add Personal Discount.';

            ExecuteAction(actionName, confirmMessage, successMessage, function () { Xrm.Page.data.refresh(true); });
        }
    }

}

function RequestApprovalForPersonalDiscount() {
    var actionName = 'new_ITTNActionQuoteRequestApprovalForPersonalDiscount';
    var confirmMessage = 'Do you want to request approval for Conditional Type - Personal Discount ?';
    var successMessage = 'Successfully executed request approval for Conditional Type - Personal Discount.';

    var confirm = window.confirm(confirmMessage);

    if (confirm) ExecuteAction(actionName, confirmMessage, successMessage, function () { Xrm.Page.data.refresh(true); });
}

function ApprovePersonalDiscount() {
    var actionName = 'new_ITTNActionQuoteApprovePersonalDiscount';
    var confirmMessage = 'Do you want to approve Conditional Type - Personal Discount ?';
    var successMessage = 'Successfully executed approve Conditional Type - Personal Discount.';

    var confirm = window.confirm(confirmMessage);

    if (confirm) ExecuteAction(actionName, confirmMessage, successMessage, function () { Xrm.Page.data.refresh(true); });
}

function RejectPersonalDiscount() {
    var actionName = 'new_ITTNActionQuoteRejectPersonalDiscount';
    var confirmMessage = 'Do you want to reject Conditional Type - Personal Discount ?';
    var successMessage = 'Successfully executed reject Conditional Type - Personal Discount.';

    var confirm = window.confirm(confirmMessage);

    if (confirm) ExecuteAction(actionName, confirmMessage, successMessage, function () { Xrm.Page.data.refresh(true); });
}

function CheckPersonalDiscount_CurrentApproval() {

}

function AddFAT() {
    var formName = Xrm.Page.ui.formSelector.getCurrentItem().getLabel();

    if (formName == 'Information') {
        _isaddfat = true;

        //Xrm.Page.ui.tabs.get("tab_14").sections.get("tab_14_section_1").setVisible(true);

        //Xrm.Page.getAttribute("ittn_isfatcentralized").setValue(false);
        //Xrm.Page.getAttribute("ittn_isfatcentralized").setSubmitMode("always");
        //Xrm.Page.getControl("ittn_isfatcentralized").setDisabled(false);

        //Xrm.Page.getControl("ittn_fatamount").setVisible(false);
        ////Xrm.Page.getAttribute("ittn_fatamount").setRequiredLevel("required");
        ////Xrm.Page.getControl("ittn_fatamount").setDisabled(false);

        if (_isaddfat) {
            _isaddfat = false;

            var actionName = 'new_ITTNActionQuoteAddFAT';
            var confirmMessage = 'Do you want to Add FAT?';
            var successMessage = 'Successfully executed Add FAT.';

            ExecuteAction(actionName, confirmMessage, successMessage, function () { Xrm.Page.data.refresh(true); });
        }
    }
}

function RequestApprovalForFAT() {
    var actionName = 'new_ITTNActionQuoteRequestApprovalForFAT';
    var confirmMessage = 'Do you want to request approval for Conditional Type - FAT ?';
    var successMessage = 'Successfully executed request approval for Conditional Type - FAT.';
    var confirm = window.confirm(confirmMessage);

    if (confirm) ExecuteAction(actionName, confirmMessage, successMessage, function () { Xrm.Page.data.refresh(true); });
}

function ApproveFAT() {
    var actionName = 'new_ITTNActionQuoteApproveFAT';
    var confirmMessage = 'Do you want to approve Conditional Type - FAT ?';
    var successMessage = 'Successfully executed approve Conditional Type - FAT.';

    var confirm = window.confirm(confirmMessage);

    if (confirm) ExecuteAction(actionName, confirmMessage, successMessage, function () { Xrm.Page.data.refresh(true); });
}

function RejectFAT() {
    var actionName = 'new_ITTNActionQuoteRejectFAT';
    var confirmMessage = 'Do you want to reject Conditional Type - FAT ?';
    var successMessage = 'Successfully executed reject Conditional Type - FAT.';

    var confirm = window.confirm(confirmMessage);

    if (confirm) ExecuteAction(actionName, confirmMessage, successMessage, function () { Xrm.Page.data.refresh(true); });
}

function AddVoucher() {
    var formName = Xrm.Page.ui.formSelector.getCurrentItem().getLabel();

    if (formName == 'Information') {
        _isaddvoucher = true;

        //Xrm.Page.ui.tabs.get("tab_14").sections.get("tab_14_section_3").setVisible(true);

        //Xrm.Page.getAttribute("ittn_isvouchercentralized").setValue(false);
        //Xrm.Page.getAttribute("ittn_isvouchercentralized").setSubmitMode("always");
        //Xrm.Page.getControl("ittn_isvouchercentralized").setDisabled(false);

        //Xrm.Page.getControl("ittn_voucheramount").setVisible(false);
        ////Xrm.Page.getAttribute("ittn_voucheramount").setRequiredLevel("required");
        ////Xrm.Page.getControl("ittn_voucheramount").setDisabled(false);

        if (_isaddvoucher) {
            _isaddvoucher = false;

            var actionName = 'new_ITTNActionQuoteAddVoucher';
            var confirmMessage = 'Do you want to Add Voucher?';
            var successMessage = 'Successfully executed Add Voucher.';

            ExecuteAction(actionName, confirmMessage, successMessage, function () { Xrm.Page.data.refresh(true); });
        }
    }
}

function RequestApprovalForVoucher() {
    var actionName = 'new_ITTNActionQuoteRequestApprovalForVoucher';
    var confirmMessage = 'Do you want to request approval for Conditional Type - Voucher ?';
    var successMessage = 'Successfully executed request approval for Conditional Type - Voucher.';

    var confirm = window.confirm(confirmMessage);

    if (confirm) ExecuteAction(actionName, confirmMessage, successMessage, function () { Xrm.Page.data.refresh(true); });
}

function ApproveVoucher() {
    var actionName = 'new_ITTNActionQuoteApproveVoucher';
    var confirmMessage = 'Do you want to approve Conditional Type - Voucher ?';
    var successMessage = 'Successfully executed approve Conditional Type - Voucher.';

    var confirm = window.confirm(confirmMessage);

    if (confirm) ExecuteAction(actionName, confirmMessage, successMessage, function () { Xrm.Page.data.refresh(true); });
}

function RejectVoucher() {
    var actionName = 'new_ITTNActionQuoteRejectVoucher';
    var confirmMessage = 'Do you want to reject Conditional Type - Voucher ?';
    var successMessage = 'Successfully executed reject Conditional Type - Voucher.';

    var confirm = window.confirm(confirmMessage);

    if (confirm) ExecuteAction(actionName, confirmMessage, successMessage, function () { Xrm.Page.data.refresh(true); });
}

function AddOthers() {
    var formName = Xrm.Page.ui.formSelector.getCurrentItem().getLabel();

    if (formName == 'Information') {
        _isaddothers = true;

        //Xrm.Page.ui.tabs.get("tab_14").sections.get("tab_14_section_4").setVisible(true);

        //Xrm.Page.getAttribute("ittn_isotherscentralized").setValue(false);
        //Xrm.Page.getAttribute("ittn_isotherscentralized").setSubmitMode("always");
        //Xrm.Page.getControl("ittn_isotherscentralized").setDisabled(false);

        //Xrm.Page.getControl("ittn_othersamount").setVisible(false);
        ////Xrm.Page.getAttribute("ittn_othersamount").setRequiredLevel("required");
        ////Xrm.Page.getControl("ittn_othersamount").setDisabled(false);

        if (_isaddothers) {
            _isaddothers = false;

            var actionName = 'new_ITTNActionQuoteAddOthers';
            var confirmMessage = 'Do you want to Add Others?';
            var successMessage = 'Successfully executed Add Others.';

            ExecuteAction(actionName, confirmMessage, successMessage, function () { Xrm.Page.data.refresh(true); });
        }
    }
    
}

function RequestApprovalForOthers() {
    var actionName = 'new_ITTNActionQuoteRequestApprovalForOthers';
    var confirmMessage = 'Do you want to request approval for Conditional Type - Others ?';
    var successMessage = 'Successfully executed request approval for Conditional Type - Others.';

    var confirm = window.confirm(confirmMessage);

    if (confirm) ExecuteAction(actionName, confirmMessage, successMessage, function () { Xrm.Page.data.refresh(true); });
}

function ApproveOthers() {
    var actionName = 'new_ITTNActionQuoteApproveOthers';
    var confirmMessage = 'Do you want to approve Conditional Type - Others ?';
    var successMessage = 'Successfully executed approve Conditional Type - Others.';

    var confirm = window.confirm(confirmMessage);

    if (confirm) ExecuteAction(actionName, confirmMessage, successMessage, function () { Xrm.Page.data.refresh(true); });
}

function RejectOthers() {
    var actionName = 'new_ITTNActionQuoteRejectOthers';
    var confirmMessage = 'Do you want to reject Conditional Type - Others ?';
    var successMessage = 'Successfully executed reject Conditional Type - Others.';

    var confirm = window.confirm(confirmMessage);

    if (confirm) ExecuteAction(actionName, confirmMessage, successMessage, function () { Xrm.Page.data.refresh(true); });
}

function CheckConditionType_NeedApprove(_conditiontypecode) {
    debugger;
    var _flag = false;
    var _recordid = Xrm.Page.data.entity.getId();

    var path = "/ittn_masterconditiontypeSet?$select=ittn_masterconditiontypeId&$filter=ittn_code eq '" + _conditiontypecode + "'";
    var retrieveReq = RetrieveOData(path);
    if (retrieveReq.readyState == 4 /* complete */) {
        var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
        if (retrieved != null && retrieved.results.length > 0) {
            var _masterconditiontypeid = retrieved.results[0].ittn_masterconditiontypeId;

            //path = "/ittn_quoteconditiontypeSet?$select=ittn_quoteconditiontypeId&$filter=ittn_Quote/Id eq (guid'" + _recordid + "') and ittn_ConditionType/Id eq (guid'" + _masterconditiontypeid + "') and ittn_NeedApproveConditionType eq true and ittn_statusreason/Value eq 841150003";
            path = "/ittn_quoteconditiontypeSet?$select=ittn_quoteconditiontypeId&$filter=ittn_Quote/Id eq (guid'" + _recordid + "') and ittn_ConditionType/Id eq (guid'" + _masterconditiontypeid + "') and ittn_statusreason/Value eq 841150003";
            retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved != null && retrieved.results.length > 0) {
                    _flag = true;
                }
            }

        }
    }

    return _flag;
}

function CheckConditionType_NeedApprovalRequest(_conditiontypecode) {
    var _flag = false;
    var _recordid = Xrm.Page.data.entity.getId();

    var path = "/ittn_masterconditiontypeSet?$select=ittn_masterconditiontypeId&$filter=ittn_code eq '" + _conditiontypecode + "'";
    var retrieveReq = RetrieveOData(path);
    if (retrieveReq.readyState == 4 /* complete */) {
        var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
        if (retrieved != null && retrieved.results.length > 0) {
            var _masterconditiontypeid = retrieved.results[0].ittn_masterconditiontypeId;

            path = "/ittn_quoteconditiontypeSet?$select=ittn_quoteconditiontypeId&$filter=ittn_Quote/Id eq (guid'" + _recordid + "') and ittn_ConditionType/Id eq (guid'" + _masterconditiontypeid + "') and ittn_statusreason/Value eq 841150000";
            retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved != null && retrieved.results.length > 0) {
                    _flag = true;
                }
            }

        }
    }

    return _flag;
}

function CheckConditionType_AlreadyActive(_conditiontypecode) {
    var _flag = false;
    var _recordid = Xrm.Page.data.entity.getId();

    var path = "/ittn_masterconditiontypeSet?$select=ittn_masterconditiontypeId&$filter=ittn_code eq '" + _conditiontypecode + "'";
    var retrieveReq = RetrieveOData(path);
    if (retrieveReq.readyState == 4 /* complete */) {
        var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
        if (retrieved != null && retrieved.results.length > 0) {
            var _masterconditiontypeid = retrieved.results[0].ittn_masterconditiontypeId;

            path = "/ittn_quoteconditiontypeSet?$select=ittn_quoteconditiontypeId&$filter=ittn_Quote/Id eq (guid'" + _recordid + "') and ittn_ConditionType/Id eq (guid'" + _masterconditiontypeid + "') and ittn_statusreason/Value eq 841150001";
            retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved != null && retrieved.results.length > 0) {
                    _flag = true;
                }
            }

        }
    }

    return _flag;
}

function OnChange_ShippingPoint() {
    var _shippingpoint = GetAttributeValue(Xrm.Page.getAttribute("ittn_shippingpoint"));

    if (_shippingpoint != null) {
        var path = "/ittn_mastershippingpricelistSet?$select=ittn_mastershippingpricelistId,ittn_ServiceAgent&$filter=ittn_ShipmentRoute/Id eq (guid'" + _shippingpoint[0].id + "') and ittn_DefaultServiceAgent eq true";
        var retrieveReq = RetrieveOData(path);
        if (retrieveReq.readyState == 4 /* complete */) {
            var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
            if (retrieved != null && retrieved.results.length > 0) {
                var _serviceagentid = retrieved.results[0].ittn_ServiceAgent.Id;

                path = "/ittn_mastervendorSet?$select=ittn_Code,ittn_mastervendorId,ittn_name&$filter=ittn_mastervendorId eq (guid'" + _serviceagentid + "')";
                retrieveReq = RetrieveOData(path);
                if (retrieveReq.readyState == 4 /* complete */) {
                    retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                    if (retrieved != null && retrieved.results.length > 0) {

                        Xrm.Page.getAttribute("ittn_vendor").setValue([{ id: retrieved.results[0].ittn_mastervendorId, name: retrieved.results[0].ittn_name, entityType: "ittn_mastervendor" }]);
                        Xrm.Page.getAttribute("ittn_vendor").setSubmitMode("always");

                    }
                }

            }
        }
    }

    UPDATE_DeliveryTerms();

}

function UPDATE_DeliveryTerms() {
    debugger;

    var _RESULT = null;
    var _statecode = GetAttributeValue(Xrm.Page.getAttribute("statecode"));
    var _isshippingcentralized = GetAttributeValue(Xrm.Page.getAttribute("ittn_isshippingcentralized"));

    if (_statecode == 0) {
        if (_isshippingcentralized) {
            var _deliveryoption = GetAttributeValue(Xrm.Page.getAttribute("ittn_deliveryoption"));

            if (_deliveryoption != null) {
                var _FRANCO = 841150000;
                var _LOCO = 841150001;

                if (_deliveryoption == _FRANCO) {
                    _RESULT = "Franco";

                    var _shippingpoint = GetAttributeValue(Xrm.Page.getAttribute("ittn_shippingpoint"));

                    if (_shippingpoint != null) {
                        var path = "/ittn_masterrouteSet?$select=ittn_masterrouteId,ittn_Route,ittn_routedescription&$filter=ittn_masterrouteId eq (guid'" + _shippingpoint[0].id + "')";
                        var retrieveReq = RetrieveOData(path);
                        if (retrieveReq.readyState == 4 /* complete */) {
                            var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                            if (retrieved != null && retrieved.results.length > 0) {
                                _RESULT += " " + retrieved.results[0].ittn_Route;
                            }
                        }
                    }
                }
                else if (_deliveryoption == _LOCO) {
                    _RESULT = "Loco";

                    var _deliverybranch = GetAttributeValue(Xrm.Page.getAttribute("ittn_deliverybranch"));

                    if (_deliverybranch != null) {
                        var path = "/new_deliveryplantSet?$select=new_deliveryplantId,new_Description,new_name&$filter=new_deliveryplantId eq (guid'" + _deliverybranch[0].id + "')";
                        var retrieveReq = RetrieveOData(path);
                        if (retrieveReq.readyState == 4 /* complete */) {
                            var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                            if (retrieved != null && retrieved.results.length > 0) {
                                _RESULT += " " + retrieved.results[0].new_Description;
                            }
                        }
                    }
                }
                else {
                    _RESULT = null;
                }

                Xrm.Page.getAttribute("new_deliveryterms").setValue(_RESULT);
                Xrm.Page.getAttribute("new_deliveryterms").setSubmitMode("always");
            }
            else {
                Xrm.Page.getAttribute("new_deliveryterms").setValue(_RESULT);
                Xrm.Page.getAttribute("new_deliveryterms").setSubmitMode("always");
            }
        }
        else {
            _RESULT = "Loco..../Franco Site....";

            Xrm.Page.getAttribute("new_deliveryterms").setValue(_RESULT);
            Xrm.Page.getAttribute("new_deliveryterms").setSubmitMode("always");
        }
    }

}