var formstatus = 1;
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

function SetLookupValue(fieldname, value) {
    try {
        var lookup = new Array();
        lookup[0] = new Object();
        lookup[0].id = value.Id;
        lookup[0].name = value.Name;
        lookup[0].entityType = value.LogicalName;

        Xrm.Page.getAttribute(fieldname).setValue(lookup);
        Xrm.Page.getAttribute(fieldname).setSubmitMode("always");
        Xrm.Page.getAttribute(fieldname).fireOnChange();
    }
    catch (e) {
        throw new Error("SetLookupValue : " + e.message);
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

        var taskListQuantity = Xrm.Page.getAttribute("trs_tasklistquantity").getValue();
        if (taskListQuantity == null)
            taskListQuantity = 0;
        var manualQuantity = Xrm.Page.getAttribute("trs_manualquantity").getValue(); taskListQuantity
        if (manualQuantity == null)
            manualQuantity = 0;
        quantity = manualQuantity + taskListQuantity;

        if (price != null && price > 0 && discountBy != null) {
            price = price * quantity;
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
        var lookupPart = Xrm.Page.getAttribute("trs_partnumber").getValue();
        if (lookupPart == null)
            throw new Error("Please fill Part Number first !");
        var lookupPriceList = Xrm.Page.getAttribute("trs_pricetype").getValue();
        if (lookupPriceList == null)
            throw new Error("Please fill Price Type first !");

        var price = GetPartPrice(lookupPart[0].id, lookupPriceList[0].id);

        Xrm.Page.getAttribute("trs_price").setValue(parseFloat(eval(price)));
        Xrm.Page.getAttribute("trs_price").setSubmitMode("always");
        Xrm.Page.getAttribute("trs_price").fireOnChange();
    }
    catch (e) {
        throw new Error("SetPrice : " + e.message);
    }
}

function SetDefaultPriceType() {
    try {
        var priceType = GetDefaultPriceType();
        if (priceType != null) {
            SetLookupValue("trs_pricetype", priceType);
        }
    }
    catch (e) {
        throw new Error("SetDefaultPriceType : " + e.message);
    }
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FORM ON LOAD AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function Form_OnLoad() {
    try {
        formstatus = Xrm.Page.ui.getFormType();

        if (formstatus < 2) {
            SetDefaultPriceType();
        }
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

function ManualQuantity_OnChange() {
    try {
        CalculateTotals();
    }
    catch (e) {
        alert("ManualQuantity_OnChange : " + e.message);
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

function PartNumber_OnChange() {
    try {
        SetPrice();
    }
    catch (e) {
        alert("PartNumber_OnChange : " + e.message);
    }
}

function Currency_Onchange() {
    try {
    }
    catch (e) {
        alert("Currency_Onchange : " + e.message);
    }
}
