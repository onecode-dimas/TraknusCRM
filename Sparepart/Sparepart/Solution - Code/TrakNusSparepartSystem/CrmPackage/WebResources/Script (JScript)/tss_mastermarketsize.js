function GetAttributeValue(attribute) {
    var result = null;

    if (attribute != null) {
        if (attribute.getValue() != null) {
            result = attribute.getValue();
        }
    }

    return result;
}

function isMSConfirmed() {
    var flag = true;
    var userId = Xrm.Page.context.getUserId();
    userId = userId.replace('{', '').replace('}', '');
    var req = new XMLHttpRequest();
    req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v8.0/systemusers?$select=tss_marketsizeconfirmed&$filter=systemuserid eq " + userId, false);
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
    req.onreadystatechange = function () {
        if (this.readyState === 4) {
            req.onreadystatechange = null;
            if (this.status === 200) {
                var result = JSON.parse(this.response);
                if (result.value.length > 0) {
                    flag = result.value[0]["tss_marketsizeconfirmed"];
                    if (flag == null) {
                        flag = false;
                    }
                }
            } else {
                Xrm.Utility.alertDialog(this.statusText);
            }
        }
    };
    req.send();
    return flag;
}

function btn_GenerateMasterMarketSize() {
    try {
        var dt = new Date().toISOString();
        var actionName = 'tss_AGITMarketSizeActionGenerateMasterMarketSize';
        var userId = Xrm.Page.context.getUserId();
        XrmServiceToolkit.Rest.RetrieveMultiple(
            "tss_keyaccountSet",
            "$select=*&$filter=tss_PSS/Id eq (guid'" + userId + "') and tss_ActiveEndDate ge datetime'" + dt + "' and tss_ActiveStartDate le datetime'" + dt + "'",
            function (result) {
                if (result.length > 0) {
                    ExecuteAction(actionName, 'Do you want to Generate Master Market Size?');
                }
                else {
                    Xrm.Utility.alertDialog("PSS doesn't exist on Key Account data");
                }
            },
            function (error) {
                Xrm.Utility.alertDialog(error.message);
            },

            function onComplete() {
            }, false
        );
    } catch (e) {
        Xrm.Utility.alertDialog('Fail to Generate Master Market Size : ' + e.message);
    }
}

function btn_GenerateMarketSizeResultPSS() {
    try {
        //alert("UNDER CONSTRUCTION !!");
        //var dt = new Date().toISOString();
        //var actionName = 'tss_AGITMarketSizeActionGenerateMasterMarketSize';
        //var userId = Xrm.Page.context.getUserId();
        //XrmServiceToolkit.Rest.RetrieveMultiple(
        //    "tss_keyaccountSet",
        //    "$select=*&$filter=tss_PSS/Id eq (guid'" + userId + "') and tss_ActiveEndDate ge datetime'" + dt + "' and tss_ActiveStartDate le datetime'" + dt + "'",
        //    function (result) {
        //        if (result.length > 0) {
        //            ExecuteAction(actionName);
        //        }
        //        else {
        //            Xrm.Utility.alertDialog("PSS doesn't exist on Key Account data");
        //        }
        //    },
        //    function (error) {
        //        Xrm.Utility.alertDialog(error.message);
        //    },

        //    function onComplete() {
        //    }, false
        //);AGITMarketSizeActionGenerateMarketSizeResultPSS

        var dt = new Date().toISOString();
        var actionName = 'new_AGITMarketSizeActionGenerateMarketSizeResultPSS';
        var userId = Xrm.Page.context.getUserId();
        XrmServiceToolkit.Rest.RetrieveMultiple(
            "tss_keyaccountSet",
            "$select=*&$filter=tss_PSS/Id eq (guid'" + userId + "') and tss_ActiveEndDate ge datetime'" + dt + "' and tss_ActiveStartDate le datetime'" + dt + "'",
            function (result) {
                if (result.length > 0) {
                    ExecuteAction(actionName, 'Do you want to Generate Market Size Result PSS?');
                }
                else {
                    Xrm.Utility.alertDialog("PSS doesn't exist on Key Account data");
                }
            },
            function (error) {
                Xrm.Utility.alertDialog(error.message);
            },

            function onComplete() {
            }, false
        );
    } catch (e) {
        Xrm.Utility.alertDialog('Fail to Generate Market Size Result PSS : ' + e.message);
    }
}

function btn_GenerateMasterMarketSizeSublines() {
    try {
        var dt = new Date().toISOString();
        var actionName = 'new_AGITMarketSizeActionGenerateMasterMarketSizeSublines';
        var userId = Xrm.Page.context.getUserId();
        XrmServiceToolkit.Rest.RetrieveMultiple(
            "tss_keyaccountSet",
            "$select=*&$filter=tss_PSS/Id eq (guid'" + userId + "') and tss_ActiveEndDate ge datetime'" + dt + "' and tss_ActiveStartDate le datetime'" + dt + "'",
            function (result) {
                if (result.length > 0) {
                    ExecuteAction(actionName, 'Do you want to Generate Master Market Size Sublines ?');
                }
                else {
                    Xrm.Utility.alertDialog("PSS doesn't exist on Key Account data");
                }
            },
            function (error) {
                Xrm.Utility.alertDialog(error.message);
            },

            function onComplete() {
            }, false
        );
    } catch (e) {
        Xrm.Utility.alertDialog('Fail to Generate Master Market Size Sublines : ' + e.message);
    }
}

function ShowHidetabs() {
    var unitType = Xrm.Page.getAttribute("tss_unittype").getValue();

    //UIO =865920000
    //NON UIO=865920001
    //Group UIO Commodity=865920002
    debugger;
    if (unitType == 865920000 || unitType == 865920001) {
        Xrm.Page.ui.tabs.get("tab_3").setVisible(true);
        Xrm.Page.ui.tabs.get("tab_4").setVisible(false);
    } else if (unitType == 865920002) {
        Xrm.Page.ui.tabs.get("tab_4").setVisible(true);
        Xrm.Page.ui.tabs.get("tab_3").setVisible(false);
    }
}

function FormOnload() {
    ShowHidetabs();

    visible_field();
}

function visible_field()
{
    var _errormessage = GetAttributeValue(Xrm.Page.getAttribute("tss_errormessage"));
    var _totalhmconsump = GetAttributeValue(Xrm.Page.getAttribute("tss_totalhmconsump"));
    var _unittype = GetAttributeValue(Xrm.Page.getAttribute("tss_unittype"));

    if (_errormessage != null)
        Xrm.Page.getControl("tss_errormessage").setVisible(true);
    else
        Xrm.Page.getControl("tss_errormessage").setVisible(false);

    if (_totalhmconsump != null)
        Xrm.Page.getControl("tss_totalhmconsump").setVisible(true);
    else
        Xrm.Page.getControl("tss_totalhmconsump").setVisible(false);

    if (_unittype != null) {
        if (_unittype == 865920000 || _unittype == 865920001) {
            // KA UIO
            Xrm.Page.getControl("tss_groupuiocommodityheader").setVisible(false);
            Xrm.Page.getControl("tss_qty").setVisible(false);

            Xrm.Page.getControl("tss_serialnumber").setVisible(true);
            Xrm.Page.getControl("tss_avghmmethod1").setVisible(true);
            Xrm.Page.getControl("tss_avghmmethod2").setVisible(true);
            Xrm.Page.getControl("tss_avghmmethod3").setVisible(true);
            Xrm.Page.getControl("tss_periodpmmethod4").setVisible(true);
            Xrm.Page.getControl("tss_periodpmmethod5").setVisible(true);
        }
        else if (_unittype == 865920002) {
            // KA GROUP UIO COMMODITY
            Xrm.Page.getControl("tss_groupuiocommodityheader").setVisible(true);
            Xrm.Page.getControl("tss_qty").setVisible(true);

            Xrm.Page.getControl("tss_serialnumber").setVisible(false);
            Xrm.Page.getControl("tss_avghmmethod1").setVisible(false);
            Xrm.Page.getControl("tss_avghmmethod2").setVisible(false);
            Xrm.Page.getControl("tss_avghmmethod3").setVisible(false);
            Xrm.Page.getControl("tss_periodpmmethod4").setVisible(false);
            Xrm.Page.getControl("tss_periodpmmethod5").setVisible(false);
        }
    }
    else {
        Xrm.Page.getControl("tss_groupuiocommodityheader").setVisible(false);
        Xrm.Page.getControl("tss_qty").setVisible(false);

        Xrm.Page.getControl("tss_serialnumber").setVisible(true);
        Xrm.Page.getControl("tss_avghmmethod1").setVisible(true);
        Xrm.Page.getControl("tss_avghmmethod2").setVisible(true);
        Xrm.Page.getControl("tss_avghmmethod3").setVisible(true);
        Xrm.Page.getControl("tss_periodpmmethod4").setVisible(true);
        Xrm.Page.getControl("tss_periodpmmethod5").setVisible(true);
    }
}

function ExecuteAction(actionName, question) {
    var _return = window.confirm(question);
    if (_return) {
        var result = null

        // Creating the Odata Endpoint
        var oDataPath = Xrm.Page.context.getClientUrl() + "/api/data/v8.0/";
        var req = new XMLHttpRequest();
        req.open("POST", oDataPath + actionName, false);
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState == 4) {
                req.onreadystatechange = null;
                if (this.status == 200 || this.status == 204) {
                    Xrm.Utility.alertDialog('Successfully executed.');
                    location.reload();
                }
                else {
                    var error = "";
                    if (this.response != null) {
                        error = JSON.parse(this.response).error.message;
                    }
                    Xrm.Utility.alertDialog('Fail to Generate.\r\nResponse Code : (' + req.status + ')' + "\r\nPlease contact your IT support.\r\nTechnical Details : " + error);
                }
            }
        };
        req.send();
    }
}

function Show_GenerateMasterMarketSize() {
    var _result = false;

    return _result;
}

function Show_GenerateMarketSizeResultPSS() {
    var _ismsconfirmed = isMSConfirmed();

    return !_ismsconfirmed;
}

function Show_GenerateMasterMarketSizeSublines() {
    var _result = false;

    return _result;
}