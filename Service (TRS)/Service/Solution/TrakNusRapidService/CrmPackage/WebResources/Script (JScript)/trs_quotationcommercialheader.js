///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Helper
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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
        retrieveReq.onreadystatechange = function () { retrieveReqCallBack(this); };
        retrieveReq.send();
        return retrieveReq;
    }
    catch (e) {
        throw new Error("RetrieveOData : Failed to retrieve OData !");
    }
}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Publics
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function CalculateTotals() {
    try {
        var price = Xrm.Page.getAttribute("trs_price").getValue();
        var discountBy = Xrm.Page.getAttribute("trs_discountby").getValue();
        var discountAmount = Xrm.Page.getAttribute("trs_discountamount").getValue();
        var discountPercent = Xrm.Page.getAttribute("trs_discountpercent").getValue();
        var totalPrice = 0;

        if (price != null && price > 0 && discountBy != null) {
            if (discountBy == true) {
                Xrm.Page.getAttribute("trs_discountpercent").setValue(discountAmount * 100 / price);
                Xrm.Page.getAttribute("trs_discountpercent").setSubmitMode("always");
            }
            else {
                if (discountPercent == 0) {
                    discountAmount = 0;
                }
                else {
                    discountAmount = discountPercent * price / 100;
                }
                Xrm.Page.getAttribute("trs_discountamount").setValue(discountAmount);
                Xrm.Page.getAttribute("trs_discountamount").setSubmitMode("always");
                totalPrice = price;
            }
            totalPrice = price - discountAmount;
        }
        Xrm.Page.getAttribute("trs_totalprice").setValue(totalPrice);
        Xrm.Page.getAttribute("trs_totalprice").setSubmitMode("always");
    }
    catch (e) {
        throw new Error("CalculateTotals : " + e.message);
    }
}

function SetPrice() {
    try {
        var TaskHeaderLookup = Xrm.Page.getAttribute("trs_taskheader").getValue();
        if (TaskHeaderLookup == null)
            throw new Error("Please fill Task Header first !");
        var PriceType = Xrm.Page.getAttribute("trs_pricetype").getValue();
        if (PriceType == null)
            throw new Error("Please fill Price Type first !");

        var price = GetTaskListPrice(PriceType, TaskHeaderLookup[0].id);
        Xrm.Page.getAttribute("trs_price").setValue(parseFloat(eval(price)));
        Xrm.Page.getAttribute("trs_price").setSubmitMode("always");
        Xrm.Page.getAttribute("trs_price").fireOnChange();
    }
    catch (e) {
        throw new Error("SetPrice : " + e.message);
    }
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FORM ON LOAD AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function Form_OnLoad() {
    try {
    }
    catch (e) {
        alert("Form_OnLoad : " + e.message);
    }
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FORM ON SAVE AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function Form_OnSave() {
    try {
    }
    catch (e) {
        alert("Form_OnSave : " + e.message);
    }
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FIELD EVENT AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function Price_OnChange() {
    try {
        CalculateTotals();
    }
    catch (e) {
        alert("Price_OnChange : " + e.message);
    }
}

function PriceType_OnChange() {
    try {
        SetPrice();
    }
    catch (e) {
        alert("PriceType_OnChange : " + e.message);
    }
}

function DiscountBy_OnChange() {
    try {
        CalculateTotals();
    }
    catch (e) {
        alert("DiscountBy_OnChange : " + e.message);
    }
}

function DiscountAmount_OnChange() {
    try {
        CalculateTotals();
    }
    catch (e) {
        alert("DiscountAmount_OnChange : " + e.message);
    }
}

function DiscountPercent_OnChange() {
    try {
        CalculateTotals();
    }
    catch (e) {
        alert("DiscountPercent_OnChange : " + e.message);
    }
}

function TaskHeader_OnChange() {
    try {
        var TaskHeaderLookup = Xrm.Page.getAttribute("trs_taskheader").getValue();
        if (TaskHeaderLookup != null) {
            var TaskHeaderid = TaskHeaderLookup[0].id;
            var path = "/trs_tasklistheaderSet?$select=trs_TaskListGroup&$filter=trs_tasklistheaderId eq guid'" + TaskHeaderid + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved.results != null && retrieved.results.length > 0) {
                    SetPrice();

                    var TaskListGroup = retrieved.results[0].trs_TaskListGroup;
                    if (TaskListGroup != null) {
                        path = "/trs_tasklistgroupSet?$select=trs_tasklistgroupname&$filter=trs_tasklistgroupId eq guid'" + TaskListGroup.Id + "'";
                        retrieveReq = RetrieveOData(path);
                        if (retrieveReq.readyState == 4) {
                            retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                            if (retrieved.results != null && retrieved.results.length > 0) {
                                var TaskListGroupName = retrieved.results[0].trs_tasklistgroupname;
                                if (TaskListGroupName != null) {
                                    Xrm.Page.getAttribute("trs_commercialheader").setValue(TaskListGroupName);
                                    Xrm.Page.getAttribute("trs_commercialheader").setSubmitMode("always");
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    catch (e) {
        alert("TaskHeader_OnChange : " + e.message);
    }
}