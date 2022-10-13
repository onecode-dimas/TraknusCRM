function SetRequiredDiscountField() {
    var SelectedDiscount = Xrm.Page.getAttribute("trs_discountby").getValue();

    if (SelectedDiscount == 0) //Discount by Percent
    {
        Xrm.Page.getAttribute("trs_discountpercent").setRequiredLevel("required");
        Xrm.Page.getAttribute("trs_discountamount").setRequiredLevel("none");

        Xrm.Page.ui.controls.get("trs_discountpercent").setDisabled(false);
        Xrm.Page.ui.controls.get("trs_discountamount").setDisabled(true);

        Xrm.Page.getAttribute("trs_discountpercent").setValue(null);
        Xrm.Page.getAttribute("trs_discountamount").setValue(null);
        Xrm.Page.getAttribute("trs_totalprice").setValue(null);

        Xrm.Page.getControl("trs_discountpercent").setFocus()
    }
    else if (SelectedDiscount == 1) //Discount by Amount
    {
        Xrm.Page.getAttribute("trs_discountamount").setRequiredLevel("required");
        Xrm.Page.getAttribute("trs_discountpercent").setRequiredLevel("none");

        Xrm.Page.ui.controls.get("trs_discountamount").setDisabled(false);
        Xrm.Page.ui.controls.get("trs_discountpercent").setDisabled(true);

        Xrm.Page.getAttribute("trs_discountpercent").setValue(null);
        Xrm.Page.getAttribute("trs_discountamount").setValue(null);
        Xrm.Page.getAttribute("trs_totalprice").setValue(null);

        Xrm.Page.getControl("trs_discountamount").setFocus()
    }
}

function SetAmountDiscount() {
    var Price = Xrm.Page.getAttribute("trs_partprice").getValue();
    var Qty = Xrm.Page.getAttribute("trs_quantity").getValue();
    var DiscountAmount = Xrm.Page.getAttribute("trs_discountamount").getValue();
    var DiscountPercent = Xrm.Page.getAttribute("trs_discountpercent").getValue();
    var Result;
    var TotalPrice;
    var TotalUnit;

    if (Price == null || Price <= 0)
        alert("Please fill Part Price first.");
    else if (Qty == null || Qty <= 0)
        alert("Please fill Quantity first.");
    else {
        TotalUnit = Price * Qty;

        if (DiscountAmount != null || DiscountAmount > 0) {
            Result = (DiscountAmount / TotalUnit) * 100;
            TotalPrice = TotalUnit - DiscountAmount;

            Xrm.Page.getAttribute("trs_discountpercent").setValue(Result);
        }
        if (DiscountPercent != null || DiscountPercent > 0) {
            if (DiscountPercent > 0)
                Result = (TotalUnit * (DiscountPercent / 100));
            if (DiscountPercent <= 0)
                Result = 0;
            TotalPrice = TotalUnit - Result;
            Xrm.Page.getAttribute("trs_discountamount").setValue(Result);
        }
        Xrm.Page.getAttribute("trs_totalprice").setValue(TotalPrice);
    }
}

function SetMasterPartDescription() {
    var serverUrl = "http://" + window.location.host + "/" + Xrm.Page.context.getOrgUniqueName();
    var part = Xrm.Page.getAttribute("trs_partnumber").getValue();
    if (part != null) {
        // Creating the Odata Endpoint
        var oDataPath = serverUrl + "/XRMServices/2011/OrganizationData.svc";
        var retrieveReq = new XMLHttpRequest();
        var Odata = oDataPath + "/trs_masterpartSet?$select=trs_name,trs_PartDescription&$filter=trs_masterpartId eq guid'" + part[0].id + "'";
        retrieveReq.open("GET", Odata, false);
        retrieveReq.setRequestHeader("Accept", "application/json");
        retrieveReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        retrieveReq.onreadystatechange = function () { retrieveReqCallBack(this); };
        retrieveReq.send();
        if (retrieveReq.readyState == 4 /* complete */) {
            var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
            if (retrieved.results != null && retrieved.results.length > 0) {
                if (retrieved.results[0].trs_PartDescription != null)
                    Xrm.Page.getAttribute("trs_partdescription").setValue(retrieved.results[0].trs_PartDescription);
            }
        }
    }
}

function GetPartPrice() {
    var serverUrl = "http://" + window.location.host + "/" + Xrm.Page.context.getOrgUniqueName();
    Xrm.Page.getAttribute("trs_price").setValue(null);
    var lookupObjectPartNumber = Xrm.Page.getAttribute("trs_partnumber");
    var lookupObjectPriceType = Xrm.Page.getAttribute("trs_pricetype");
    var PartNumberId;
    var PriceTypeId;

    if (lookupObjectPartNumber != null && lookupObjectPriceType != null) {
        var lookupObjectPartNumberValue = lookupObjectPartNumber.getValue();
        var lookupObjectPriceTypeValue = lookupObjectPriceType.getValue();

        if (lookupObjectPartNumberValue != null && lookupObjectPriceTypeValue != null) {
            var lookupObjectPartNumberId = lookupObjectPartNumberValue[0].id;
            var lookupObjectPriceTypeId = lookupObjectPriceTypeValue[0].id;

            // Creating the Odata Endpoint - Part Price Master
            var oDataPath = serverUrl + "/XRMServices/2011/OrganizationData.svc";
            var retrieveReq = new XMLHttpRequest();
            var Odata = oDataPath + "/trs_partpricemasterSet?$select=trs_Price&$filter=trs_PartMaster/Id eq guid'" + lookupObjectPartNumberId + "' and trs_PriceList/Id eq guid'" + lookupObjectPriceTypeId + "'";
            retrieveReq.open("GET", Odata, false);
            retrieveReq.setRequestHeader("Accept", "application/json");
            retrieveReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            retrieveReq.onreadystatechange = function () { retrieveReqCallBack(this); };
            retrieveReq.send();
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved.results != null && retrieved.results.length > 0) {
                    if (retrieved.results[0].trs_Price != null && retrieved.results[0].trs_Price.Value > 0) {
                        var value = retrieved.results[0].trs_Price.Value;
                        Xrm.Page.getAttribute("trs_price").setValue(parseFloat(eval(value)));
                        Xrm.Page.getAttribute("trs_price").setSubmitMode("always");
                        Xrm.Page.getAttribute("trs_price").fireOnChange();
                    }
                    else {
                        Xrm.Page.getAttribute("trs_price").setValue(null);
                        Xrm.Page.getAttribute("trs_price").setSubmitMode("always");
                        Xrm.Page.getAttribute("trs_price").fireOnChange();
                    }
                }
            }
        }
    }
}