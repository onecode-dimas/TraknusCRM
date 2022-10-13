function SetCommercialHeader() {
    var serverUrl = "http://" + window.location.host + "/" + Xrm.Page.context.getOrgUniqueName();
    Xrm.Page.getAttribute("subject").setValue(null);
    var lookupObject = Xrm.Page.getAttribute("trs_tasklistheader");
    var TaskListGroupId;
    var PriceType = Xrm.Page.getAttribute("trs_pricetype").getValue();

    Xrm.Page.getAttribute("trs_price").setValue(null);
    Xrm.Page.getAttribute("transactioncurrencyid").setValue(null);

    if (lookupObject != null) {
        var lookUpObjectValue = lookupObject.getValue();

        if ((lookUpObjectValue != null)) {
            var lookupid = lookUpObjectValue[0].id;

            // Creating the Odata Endpoint - Task List Header
            var oDataPath = serverUrl + "/XRMServices/2011/OrganizationData.svc";
            var retrieveReq = new XMLHttpRequest();
            var Odata = oDataPath + "/trs_tasklistheaderSet?$select=trs_TaskListGroup,trs_Price,TransactionCurrencyId&$filter=trs_tasklistheaderId eq guid'" + lookupid + "'";
            retrieveReq.open("GET", Odata, false);
            retrieveReq.setRequestHeader("Accept", "application/json");
            retrieveReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            retrieveReq.onreadystatechange = function () { retrieveReqCallBack(this); };
            retrieveReq.send();
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved.results != null && retrieved.results.length > 0) {
                    if (retrieved.results[0].trs_TaskListGroup != null)
                        TaskListGroupId = retrieved.results[0].trs_TaskListGroup.Id;
                    if (PriceType == 167630001) {
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

                        if (retrieved.results[0].TransactionCurrencyId != null) {
                            var arr = new Array();
                            arr[0] = new Object();
                            arr[0].id = retrieved.results[0].TransactionCurrencyId.Id;
                            arr[0].name = retrieved.results[0].TransactionCurrencyId.Name;
                            arr[0].entityType = retrieved.results[0].TransactionCurrencyId.LogicalName;
                            Xrm.Page.getAttribute("transactioncurrencyid").setValue(arr);
                            Xrm.Page.getAttribute("transactioncurrencyid").setSubmitMode("always");
                            Xrm.Page.getAttribute("transactioncurrencyid").fireOnChange();
                        }
                        else {
                            Xrm.Page.getAttribute("transactioncurrencyid").setValue(null);
                            Xrm.Page.getAttribute("transactioncurrencyid").setSubmitMode("always");
                            Xrm.Page.getAttribute("transactioncurrencyid").fireOnChange();
                        }
                    }
                }
            }

            // Creating the Odata Endpoint - Task List Group
            var retrieveReq2 = new XMLHttpRequest();
            var Odata2 = oDataPath + "/trs_tasklistgroupSet?$select=trs_tasklistgroupname&$filter=trs_tasklistgroupId eq guid'" + TaskListGroupId + "'";
            retrieveReq2.open("GET", Odata2, false);
            retrieveReq2.setRequestHeader("Accept", "application/json");
            retrieveReq2.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            retrieveReq2.onreadystatechange = function () { retrieveReqCallBack(this); };
            retrieveReq2.send();
            if (retrieveReq2.readyState == 4 /* complete */) {
                var retrieved2 = this.parent.JSON.parse(retrieveReq2.responseText).d;
                if (retrieved2.results != null && retrieved2.results.length > 0) {
                    if (retrieved2.results[0].trs_tasklistgroupname != null) {
                        Xrm.Page.getAttribute("subject").setValue(retrieved2.results[0].trs_tasklistgroupname);
                        Xrm.Page.getAttribute("subject").setSubmitMode("always");
                        Xrm.Page.getAttribute("subject").fireOnChange();
                    }
                }
            }
        }
    }
}

function CalculateTotal() {
    var price = Xrm.Page.getAttribute("trs_price").getValue();
    var discountBy = Xrm.Page.getAttribute("trs_discountby").getValue();
    var discountAmount = Xrm.Page.getAttribute("trs_discountamount").getValue();
    var totalPrice = 0;

    if (price != null && discountBy != null) {
        if (discountBy == true) {
            var discountAmount = Xrm.Page.getAttribute("trs_discountamount").getValue();
            Xrm.Page.getAttribute("trs_discountpercent").setValue(discountAmount * 100 / price);
            Xrm.Page.getAttribute("trs_discountpercent").setSubmitMode("always");
            totalPrice = price - discountAmount;
        }
        else {
            var discountPercent = Xrm.Page.getAttribute("trs_discountpercent").getValue();
            if (discountPercent == 0) {
                Xrm.Page.getAttribute("trs_discountamount").setValue(0);
                Xrm.Page.getAttribute("trs_discountamount").setSubmitMode("always");
                totalPrice = price;
            }
            else {
                Xrm.Page.getAttribute("trs_discountamount").setValue(discountPercent / 100 * price);
                Xrm.Page.getAttribute("trs_discountamount").setSubmitMode("always");
                totalPrice = (100 - discountPercent) / 100 * price;
            }
        }
    }
    Xrm.Page.getAttribute("trs_totalprice").setValue(totalPrice);
    Xrm.Page.getAttribute("trs_totalprice").setSubmitMode("always");
}

function SetDiscount() {
    var disc = Xrm.Page.getAttribute("trs_discountby").getValue();
    if (disc == 0) {
        Xrm.Page.getControl("trs_discountpercent").setRequiredLevel("required");
        Xrm.Page.getControl("trs_discountamount").setRequiredLevel("none");
        Xrm.Page.getControl("trs_discountpercent").setDisabled(false);
        Xrm.Page.getControl("trs_discountamount").setDisabled(true);
    }
    else {
        Xrm.Page.getControl("trs_discountpercent").setRequiredLevel("none");
        Xrm.Page.getControl("trs_discountamount").setRequiredLevel("required");
        Xrm.Page.getControl("trs_discountpercent").setDisabled(true);
        Xrm.Page.getControl("trs_discountamount").setDisabled(false);
    }
}

function setDiscountAmount() {
    if (Xrm.Page.getAttribute("trs_discountpercent").getValue() != null) {
        var price = Xrm.Page.getAttribute("trs_price").getValue();
        var disc = Xrm.Page.getAttribute("trs_discountpercent").getValue();
        var amount = price * disc / 100;
        Xrm.Page.getAttribute("trs_discountamount").setValue(amount);
        Xrm.Page.getAttribute("trs_totalprice").setValue(price - amount);
        Xrm.Page.getAttribute("trs_totalprice").setSubmitMode("always");
    }
}

function setDiscountPercent() {
    if (Xrm.Page.getAttribute("trs_discountamount").getValue() != null) {
        var price = Xrm.Page.getAttribute("trs_price").getValue();
        var disc = Xrm.Page.getAttribute("trs_discountamount").getValue();
        var amount = parseFloat(disc * 100 / price);
        Xrm.Page.getAttribute("trs_discountpercent").setValue(amount);
        Xrm.Page.getAttribute("trs_totalprice").setValue(price - disc);
        Xrm.Page.getAttribute("trs_totalprice").setSubmitMode("always");
    }
}