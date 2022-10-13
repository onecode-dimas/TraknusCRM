///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Helper

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function DisableField(ControlName) {

    try {

        Xrm.Page.getControl(ControlName).setDisabled(true);

    }

    catch (e) {

        throw new Error("DisableField : " + e.message);

    }

}



function EnableField(ControlName) {

    try {

        Xrm.Page.getControl(ControlName).setDisabled(false);

    }

    catch (e) {

        throw new Error("EnableField : " + e.message);

    }

}



function RequiredControl(ControlName) {

    try {

        Xrm.Page.getAttribute(ControlName).setRequiredLevel("required");

    }

    catch (e) {

        throw new Error("RequiredControl : " + e.message);

    }

}



function UnrequiredControl(ControlName) {

    try {

        Xrm.Page.getAttribute(ControlName).setRequiredLevel("none");

    }

    catch (e) {

        throw new Error("UnrequiredControl : " + e.message);

    }

}



function SetFieldValue(fieldname, value) {

    try {

        Xrm.Page.getAttribute(fieldname).setValue(value);

        Xrm.Page.getAttribute(fieldname).setSubmitMode("always");

        Xrm.Page.getAttribute(fieldname).fireOnChange();

    }

    catch (e) {

        throw new Error("SetFieldValue : " + e.message);

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

function Clear_DiscountAmount() {

    try {

        SetFieldValue("trs_discountamount", 0);

        DisableField("trs_discountamount");

        UnrequiredControl("trs_discountamount");

        RequiredControl("trs_discountpercent");

        EnableField("trs_discountpercent");

    }

    catch (e) {

        alert("Clear_DiscountAmount : " + e.message);

    }

}



function Clear_DiscountPercent() {

    try {

        SetFieldValue("trs_discountpercent", 0);

        DisableField("trs_discountpercent");

        UnrequiredControl("trs_discountpercent");

        RequiredControl("trs_discountamount");

        EnableField("trs_discountamount");

    }

    catch (e) {

        alert("Clear_DiscountPercent : " + e.message);

    }

}



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

        var TaskHeaderLookup = Xrm.Page.getAttribute("trs_tasklistheader").getValue();

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

        DiscountBy_OnChange();

        var _check = Check_IsPMActTypeExist();

        if (_check == true) {
            HidePricingField();
        }

    }

    catch (e) {

        alert("Form_OnLoad : " + e.message);

    }

}

function Check_IsPMActTypeExist() {

    try {

        var _flag = false;
        var _operationid = GetAttributeValue(Xrm.Page.getAttribute("trs_operationid"));

        if (_operationid != null) {
            var path = "/ServiceAppointmentSet?$select=ActivityId,trs_PMActType&$filter=ActivityId eq (guid'" + _operationid[0].id + "') and  trs_PMActType ne null";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved != null && retrieved.results.length > 0) {
                    
                    path = "/trs_tasklistgroupSet?$select=trs_tasklistgroupId&$filter=trs_tasklistgroupId eq (guid'" + retrieved.results[0].trs_PMActType.Id + "') and  ittn_OrderType ne null";
                    retrieveReq = RetrieveOData(path);
                    if (retrieveReq.readyState == 4 /* complete */) {
                        retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                        if (retrieved != null && retrieved.results.length > 0) {

                            _flag = true;

                        }
                    }

                }
            }
        }

        return _flag;

    } catch (e) {

        alert("Check_IsPMActTypeExist : " + e.message);

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

        //Updated by Santony [1/12/2016] : Replace Business Rules

        //CalculateTotals();



        //Discount By : 0 => Percent, 1 => Amount

        var discountBy = Xrm.Page.getAttribute("trs_discountby").getValue();

        var DiscountAmount = Xrm.Page.getAttribute("trs_discountamount").getValue();

        var DiscountPercent = Xrm.Page.getAttribute("trs_discountpercent").getValue();



        if (discountBy == 0) {

            if (DiscountPercent == null)

                SetFieldValue("trs_discountpercent", 0);

            Clear_DiscountAmount();

        }

        else if (discountBy == 1) {

            if (DiscountAmount == null)

                SetFieldValue("trs_discountamount", 0);

            Clear_DiscountPercent();

        }

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

function HidePricingField() {

    Xrm.Page.getAttribute("transactioncurrencyid").setRequiredLevel("none");
    Xrm.Page.getControl("transactioncurrencyid").setVisible(false);

    Xrm.Page.getAttribute("trs_pricetype").setRequiredLevel("none");
    Xrm.Page.getControl("trs_pricetype").setVisible(false);

    Xrm.Page.getAttribute("trs_price").setRequiredLevel("none");
    Xrm.Page.getControl("trs_price").setVisible(false);

    Xrm.Page.getAttribute("trs_discountby").setRequiredLevel("none");
    Xrm.Page.getControl("trs_discountby").setVisible(false);

    Xrm.Page.getAttribute("trs_discountamount").setRequiredLevel("none");
    Xrm.Page.getControl("trs_discountamount").setVisible(false);

    Xrm.Page.getAttribute("trs_discountpercent").setRequiredLevel("none");
    Xrm.Page.getControl("trs_discountpercent").setVisible(false);

    Xrm.Page.getAttribute("trs_totalprice").setRequiredLevel("none");
    Xrm.Page.getControl("trs_totalprice").setVisible(false);

}

function TaskHeader_OnChange() {

    debugger;

    try {

        var TaskHeaderLookup = Xrm.Page.getAttribute("trs_tasklistheader").getValue();

        if (TaskHeaderLookup != null) {

            var TaskHeaderid = TaskHeaderLookup[0].id;

            var path = "/trs_tasklistheaderSet?$select=TransactionCurrencyId,trs_TaskListGroup&$filter=trs_tasklistheaderId eq guid'" + TaskHeaderid + "'";

            var retrieveReq = RetrieveOData(path);

            if (retrieveReq.readyState == 4 /* complete */) {

                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;

                if (retrieved.results != null && retrieved.results.length > 0) {

                    var _check = Check_IsPMActTypeExist();

                    if (_check == true) {

                        var TaskListGroup = retrieved.results[0].trs_TaskListGroup;

                        if (TaskListGroup != null) {

                            path = "/trs_tasklistgroupSet?$select=trs_tasklistgroupname&$filter=trs_tasklistgroupId eq guid'" + TaskListGroup.Id + "'";

                            retrieveReq = RetrieveOData(path);

                            if (retrieveReq.readyState == 4) {

                                retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;

                                if (retrieved.results != null && retrieved.results.length > 0) {

                                    var TaskListGroupName = retrieved.results[0].trs_tasklistgroupname;

                                    if (TaskListGroupName != null) {

                                        Xrm.Page.getAttribute("subject").setValue(TaskListGroupName);

                                        Xrm.Page.getAttribute("subject").setSubmitMode("always");

                                    }

                                }

                            }

                        }

                        // HIDE

                        HidePricingField();

                    }
                    else {

                        SetPrice();



                        var currency = retrieved.results[0].TransactionCurrencyId;

                        if (currency != null) {

                            SetLookupValue("transactioncurrencyid", currency);

                        }

                        else {

                            Xrm.Page.getAttribute("transactioncurrencyid").setValue(null);

                            Xrm.Page.getAttribute("transactioncurrencyid").setSubmitMode("always");

                            Xrm.Page.getAttribute("transactioncurrencyid").fireOnChange();

                        }



                        var TaskListGroup = retrieved.results[0].trs_TaskListGroup;

                        if (TaskListGroup != null) {

                            path = "/trs_tasklistgroupSet?$select=trs_tasklistgroupname&$filter=trs_tasklistgroupId eq guid'" + TaskListGroup.Id + "'";

                            retrieveReq = RetrieveOData(path);

                            if (retrieveReq.readyState == 4) {

                                retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;

                                if (retrieved.results != null && retrieved.results.length > 0) {

                                    var TaskListGroupName = retrieved.results[0].trs_tasklistgroupname;

                                    if (TaskListGroupName != null) {

                                        Xrm.Page.getAttribute("subject").setValue(TaskListGroupName);

                                        Xrm.Page.getAttribute("subject").setSubmitMode("always");

                                    }

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
