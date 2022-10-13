function getLocalPrice() {
    var getProductID = Xrm.Page.getAttribute("new_product").getValue();

    if (getProductID != null) {
        var ProductID = getProductID[0].id;
        var ittn_LocalPrice;
        var ittn_ExchangeRateMonth;
        var ittn_ExchangeRateValue;

        XrmServiceToolkit.Rest.RetrieveMultiple("ittn_pricelistunitoriginalSet", "?$select=ittn_ExchangeRateMonth,ittn_ExchangeRateValue,ittn_LocalPrice&$filter=ittn_Product/Id eq (guid'" + ProductID + "')", function (results) {
            if (results.length > 0) {
                for (var i = 0; i < results.length; i++) {
                    ittn_ExchangeRateMonth = results[i].ittn_ExchangeRateMonth;
                    ittn_ExchangeRateValue = results[i].ittn_ExchangeRateValue;
                    ittn_LocalPrice = results[i].ittn_LocalPrice;

                    SetFieldValue("ittn_exhcangeratemonth", parseInt(eval(ittn_ExchangeRateMonth.Value)));
                    SetFieldValue("ittn_exchangeratevalue", parseFloat(eval(ittn_ExchangeRateValue)));
                    SetFieldValue("ittn_localprice", parseFloat(eval(ittn_LocalPrice)));
                }
            }
            else {
                SetFieldValue("ittn_exhcangeratemonth", null);
                SetFieldValue("ittn_exchangeratevalue", null);
                SetFieldValue("ittn_localprice", null);
            }
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, function () {
            //On Complete - Do Something
        }, false);
    }
    else {
        SetFieldValue("ittn_exhcangeratemonth", null);
        SetFieldValue("ittn_exchangeratevalue", null);
        SetFieldValue("ittn_localprice", null);
    }
}

function SetFieldValue(FieldName, Value) {
    Xrm.Page.getAttribute(FieldName).setValue(Value);
    Xrm.Page.getAttribute(FieldName).setSubmitMode("always");
    Xrm.Page.getAttribute(FieldName).fireOnChange()
}

function Set_One_Month_After_CreatedOn() {
    var FormType = Xrm.Page.ui.getFormType();
    var CurrentDateTime = new Date();
    var MonthAdd = 1;

    if (FormType < 2) { //Create
        var OneMonthAfterCreatedOn = CurrentDateTime.setMonth(CurrentDateTime.getMonth() + MonthAdd);

        SetFieldValue("ittn_next1monthaftercreatedon", OneMonthAfterCreatedOn);
    }
}