//Get Form Type
var query = localStorage.getItem('populationstatus');
var formType = Xrm.Page.ui.getFormType();
var formStatus = {
    Undefined: 0,
    Create: 1,
    Update: 2,
    ReadOnly: 3,
    Disabled: 4,
    QuickCreate: 5,
    BulkEdit: 6,
    ReadOptimized: 11
};

function SetDefaultValue_TwoOptionField(fieldname) {
    var attr = Xrm.Page.getAttribute(fieldname);
    if (attr != null) {
        if (attr.getValue() == false) {
            attr.setValue(0);
            attr.setValue(1);
            attr.setSubmitMode("always");
        }
    }
}

function GetAttributeValue(attribute) {
    var result = null;

    if (attribute != null) {
        if (attribute.getValue() != null) {
            result = attribute.getValue();
        }
    }

    return result;
}

function IsConfirm() {
    var result = false;
    var keyAccountID = Xrm.Page.getAttribute("tss_keyaccountid").getValue()[0].id;
    XrmServiceToolkit.Rest.Retrieve(keyAccountID, "tss_keyaccountSet", "tss_Reason,tss_Status", null, function (result) {
        var tss_Reason = result.tss_Reason;
        var tss_Status = result.tss_Status;
        if (tss_Status == 865920005)
            result = false;
    }, function (error) {
        Xrm.Utility.alertDialog(error.message);
    }, false);
}

//function OnSave(context) {
//    debugger;
//    var saveEvent = context.getEventArgs();
//    //var status = 0;
//    //var keyAccountID = Xrm.Page.getAttribute("tss_keyaccountid").getValue()[0].id;
//    //XrmServiceToolkit.Rest.Retrieve(keyAccountID, "tss_keyaccountSet", "tss_Reason,tss_Status", null, function (result) {
//    //    status = result.tss_Status.Value;
//    //}, function (error) {
//    //    Xrm.Utility.alertDialog(error.message);
//    //}, false);

//    //if (status != 865920000) {
//    //    Xrm.Utility.alertDialog("Data has been processing, canot modify this data");
//    //    saveEvent.preventDefault();
//    //}

//    var _valid = IsValid();
    
//    if (_valid == false) {
//        Xrm.Utility.alertDialog("Serial number is used !");
//        saveEvent.preventDefault();
//    }
//}

function OnSave() {
    debugger;
    var _valid = IsValid();

    if (_valid == false) {
        saveEvent.preventDefault();
    }
}

function IsValid() {
    debugger;
    var _result = true;
    var _serialnumber = GetAttributeValue(Xrm.Page.getAttribute("tss_serialnumber"));
    var _keyaccountId = GetAttributeValue(Xrm.Page.getAttribute("tss_keyaccountid"));

    if (_serialnumber != null) {
        XrmServiceToolkit.Rest.RetrieveMultiple("tss_kauioSet", "?$select=tss_kauioId,tss_KeyAccountId&$filter=tss_SerialNumber/Id eq (guid'" + _serialnumber[0].id + "')", function (results) {
            for (var i = 0; i < results.length; i++) {
                //var tss_kauioId = results[i].tss_kauioId;
                var tss_KeyAccountId = results[i].tss_KeyAccountId;

                XrmServiceToolkit.Rest.Retrieve(tss_KeyAccountId.Id, "tss_keyaccountSet", "tss_KAMSId,tss_keyaccountId,tss_PSS,tss_Status", null, function (result) {
                    var tss_KAMSId = result.tss_KAMSId;
                    var tss_keyaccountId = result.tss_keyaccountId;
                    var tss_PSS = result.tss_PSS;
                    var tss_Status = result.tss_Status.Value;

                    //Open      865920000
                    //Calculate 865920001
                    //Active    865920003

                    if (tss_KeyAccountId.Id != tss_keyaccountId && (tss_Status == 865920000 || tss_Status == 865920001 || tss_Status == 865920003)) {
                        _result = false;

                        Xrm.Utility.alertDialog("Serial number is used in Key Account '" + tss_KAMSId + "' !");
                    }
                }, function (error) {
                    Xrm.Utility.alertDialog(error.message);
                }, false);

                if (_result == false) {
                    break;
                }
            }
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, function () {
            //On Complete - Do Something
        }, false);
    }

    return _result;
}

//Event OnLoad
function Form_OnLoad() {
    debugger;

    if (formType < 2) {
        SetDefaultValue_TwoOptionField("tss_calculatetoms");

        Xrm.Page.getAttribute("tss_calculatestatus").setValue(null);
        Xrm.Page.getAttribute("tss_calculatestatus").setSubmitMode("always");

        Xrm.Page.getAttribute("tss_reason").setRequiredLevel("none");
        Xrm.Page.getControl("tss_reason").setVisible(false);
    }

    var keyAccountID = Xrm.Page.getAttribute("tss_keyaccountid").getValue()[0].id;

    Xrm.Page.getControl("tss_estworkinghour").setDisabled(true);

    XrmServiceToolkit.Rest.Retrieve(keyAccountID, "tss_keyaccountSet", "tss_Customer,tss_FunLock,tss_PSS,tss_Status,tss_Revision", null, function (result) {
        if (result.tss_Customer.Id != null) {
            Xrm.Page.getAttribute("tss_customer").setValue([{ id: result.tss_Customer.Id, name: result.tss_Customer.Name, entityType: result.tss_Customer.LogicalName }]);
            //Xrm.Page.getAttribute("tss_customer").setSubmitMode("always");
            if (result.tss_Status.Value != 865920000) {
                Xrm.Page.getControl("tss_serialnumber").setDisabled(true);
                Xrm.Page.getControl("tss_currenthourmeter").setDisabled(true);
                Xrm.Page.getControl("tss_currenthourmeterdate").setDisabled(true);
                Xrm.Page.getControl("tss_lasthourmeter").setDisabled(true);
                Xrm.Page.getControl("tss_lasthourmeterdate").setDisabled(true);
                //Xrm.Page.getControl("tss_estworkinghour").setDisabled(true);
                Xrm.Page.getControl("tss_calculatetoms").setDisabled(true);
                Xrm.Page.getControl("tss_reason").setDisabled(true);
            }
            else
            {
                if (result.tss_Revision > 0) {
                    Xrm.Page.getControl("tss_calculatetoms").setDisabled(false);
                }
            }
        }

        if (result.tss_Customer.Id != null) {
            Xrm.Page.getAttribute("tss_pss").setValue([{ id: result.tss_PSS.Id, name: result.tss_PSS.Name, entityType: result.tss_PSS.LogicalName }]);
            //Xrm.Page.getAttribute("tss_pss").setSubmitMode("always");

            XrmServiceToolkit.Rest.Retrieve(result.tss_Customer.Id, "AccountSet", "new_BusinessSector", null, function (result) {
                if (result.new_BusinessSector.Id != null) {
                    XrmServiceToolkit.Rest.Retrieve(result.new_BusinessSector.Id, "new_businesssectorSet", "tss_estworkinghour", null, function (result) {
                        Xrm.Page.getAttribute("tss_estworkinghour").setValue(result.tss_EstWorkingHour);
                        Xrm.Page.getAttribute("tss_estworkinghour").setSubmitMode("always");
                    }, function (error) {
                        Xrm.Utility.alertDialog(error.message);
                    }, false);
                }
            }, function (error) {
                Xrm.Utility.alertDialog(error.message);
            }, false);
        }

        //preFilterLookupSerialNumber(result.tss_FunLock);
        preFilterLookupSerialNumber();
        //UIO_Prefilter(Customer, FunLock);
    }, function (error) {
        Xrm.Utility.alertDialog(error.message);
    }, true);
}

//function preFilterLookupSerialNumber(funLock) {
//    Xrm.Page.getControl("tss_serialnumber")._control && Xrm.Page.getControl("tss_serialnumber")._control.tryCompleteOnDemandInitialization && Xrm.Page.getControl("tss_serialnumber")._control.tryCompleteOnDemandInitialization();
//    Xrm.Page.getControl("tss_serialnumber").addPreSearch(function () {
//        addLookupFilterSerialNumber(funLock);
//    });
//}

//function addLookupFilterSerialNumber(funLock) {
//    var cust = Xrm.Page.getAttribute("tss_customer");
//    var custObj = cust.getValue();

//    if (custObj != null) {
//        var fetchFilters = "<filter type='and'>" +
//            "<condition attribute='new_customercode' uitype='" + custObj[0].entityType + "' operator='eq' value='" + custObj[0].id + "'/>" +
//            "<condition attribute='trs_functionallocation' uitype='" + funLock.LogicalName + "' operator='eq' value='" + funLock.Id + "'/></filter>";
//        Xrm.Page.getControl("tss_serialnumber").addCustomFilter(fetchFilters);
//    }
//}

function preFilterLookupSerialNumber() {
    //Xrm.Page.getControl("tss_serialnumber").setDisabled(false);
    //Xrm.Page.getAttribute("tss_serialnumber").setValue(null);
    preFilterLookupSN();
}

function preFilterLookupSN() {
    Xrm.Page.getControl("tss_serialnumber").addPreSearch(function () {
        addLookupFilterSN();
    });
}

function addLookupFilterSN() {
    debugger;
    var cust = Xrm.Page.getAttribute("tss_customer");
    var populationstatus = "1";

    if (query == 'Active KA UIO - UIO') {
        populationstatus = "1";
    }
    else if (query == 'Active KA UIO - Non UIO') {
        populationstatus = "0";
    }

    var fetchFilters = "<filter type='and'>" +
        "<condition attribute='new_customercode' uitype='" + cust.getValue()[0].entityType + "' operator='eq' value='" + cust.getValue()[0].id + "'/>" +
        "<condition attribute='tss_populationstatus' operator='eq' value='" + populationstatus + "'/>" +

        "<filter type='or'>" +

            "<filter type='and'>" +
            "<condition attribute='tss_mscurrenthourmeter' operator='not-null' />" +
            "<condition attribute='tss_mscurrenthourmeterdate' operator='not-null' />" +
            "<condition attribute='tss_mslasthourmeter' operator='not-null' />" +
            "<condition attribute='tss_mslasthourmeterdate' operator='not-null' />" +
            "</filter>" +

            "<filter type='and'>" +
            "<condition attribute='tss_mscurrenthourmeter' operator='not-null' />" +
            "<condition attribute='tss_mscurrenthourmeterdate' operator='not-null' />" +
            "<condition attribute='new_deliverydate' operator='not-null' />" +
            "</filter>" +

            "<filter type='and'>" +
            "<condition attribute='tss_mscurrenthourmeter' operator='null' />" +
            "<condition attribute='tss_mscurrenthourmeterdate' operator='null' />" +
            "<condition attribute='tss_mslasthourmeter' operator='null' />" +
            "<condition attribute='tss_mslasthourmeterdate' operator='null' />" +
            "<condition attribute='new_deliverydate' operator='not-null' />" +
            "</filter>" +

        "</filter>" +

        "</filter>";

    Xrm.Page.getControl("tss_serialnumber").addCustomFilter(fetchFilters);
}

function GetPopulationStatus(selectedCtrl) {
    query = "";
    localStorage.removeItem('populationstatus');
    localStorage.clear();

    localStorage.setItem('populationstatus', selectedCtrl.get_viewTitle());
}