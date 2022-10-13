///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// Helper
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
var source_directsales = 865920000;
var source_marketsize = 865920001;
var source_contract = 865920002;
var source_campaign = 865920003;
var source_quoteservice = 865920005;
var price_directsales = 865920000;
var price_marketsize = 865920001;
var price_contract = 865920002;
var price_campaign = 865920003;
var status_interchange = 865920000;
var statusreason_waitingapprovalpackage = 865920003;
var tss_isinterchange = 'tss_isinterchange';
var tss_editeddiscountbypss = 'tss_editeddiscountbypss';
var tss_approvebypa = 'tss_approvebypa';
var tss_discountby = 'tss_discountby';
var tss_underminimumprice = 'tss_underminimumprice';


function SetDefaultValue_TwoOptionField(fieldname) {
    var attr = Xrm.Page.getAttribute(fieldname);
    if (attr != null) {
        attr.setValue(1);
        attr.setValue(0);
        attr.setSubmitMode("always");
    }
}
function Set_OnLoad() {
    Xrm.Page.getAttribute("tss_discountamount").setValue(0);
    Xrm.Page.getAttribute("tss_discountpercent").setValue(0);
    Xrm.Page.getAttribute("tss_quantity").setValue(0);
    Xrm.Page.getAttribute("tss_finalprice").setValue(0);
    Xrm.Page.getAttribute("tss_totalfinalprice").setValue(0);
    Xrm.Page.getAttribute("tss_totalprice").setValue(0);
    Xrm.Page.getAttribute("tss_priceafterdiscount").setValue(0);
    SetDefaultValue_TwoOptionField(tss_isinterchange);
    SetDefaultValue_TwoOptionField(tss_editeddiscountbypss);
    SetDefaultValue_TwoOptionField(tss_approvebypa);
    SetDefaultValue_TwoOptionField(tss_discountby);
    SetDefaultValue_TwoOptionField(tss_underminimumprice);
    //Xrm.Page.getAttribute('tss_sourcetype').setValue(parseInt(source_quoteservice));
}

function checkIsPackage() {

    var isPss = false;
    var userLoginId = Xrm.Page.context.getUserId().replace("{", "").replace("}", "");



    XrmServiceToolkit.Rest.RetrieveMultiple("SystemUserSet", "?$select=SystemUserId&$filter=Title eq 'PSS' and SystemUserId eq (guid'" + userLoginId + "')", function (results) {
        if (results.length > 0) {
            isPss = true;
        }
    }, function (error) {

    }, function () {
        //On Complete - Do Something
    }, false);



    var flagPackage = false;
    var quotation = Xrm.Page.data.entity.attributes.get("tss_quotationpartheader").getValue();
    if (quotation != null) {
        XrmServiceToolkit.Rest.Retrieve(
        quotation[0].id,
        "tss_quotationpartheaderSet",
        null, null,
        function (result) {
            var relatedPackageAttribute = ["tss_finalprice", "tss_totalfinalprice", "tss_discountby", "tss_priceafterdiscount", "tss_discountamount", "tss_discountpercent",
               "tss_approvebypa", "tss_approvefinalpriceby", "tss_totalexpectedpackageamount", "tss_totalprice"];
            if (result.tss_package == true && isPss == false) {
                flagPackage = true; // di show
            }

            for (var index = 0; index < relatedPackageAttribute.length; index++) {
                Xrm.Page.getControl(relatedPackageAttribute[index]).setVisible(flagPackage);
            }

            if (flagPackage) {
                if (result.tss_totalexpectedpackageamount != null && result.tss_totalexpectedpackageamount.Value != null)
                    Xrm.Page.getAttribute("tss_totalexpectedpackageamount").setValue(parseFloat(result.tss_totalexpectedpackageamount.Value));
            }
        }, function (error) {
            alert("failed retrieve package is: " + error.message);
        }, false);
    } else {
        alert('Quotation Part Lines must be created on Prospect Part !');
        Xrm.Page.ui.close();
    }
    return flagPackage;
}

function checkReqDeliveryDate() {
    //tss_statuscode
    var QUOTATIONSTATUS_DRAFT = 865920000;
    var QUOTATIONSTATUS_APPROVED = 865920002;

    //tss_statusreason
    var STATUSREASON_OPEN = 865920000;
    var STATUSREASON_APPROVED = 865920004;

    var re;
    var quotation = Xrm.Page.data.entity.attributes.get("tss_quotationpartheader").getValue();
    //var reqdeliv = Xrm.Page.data.entity.attributes.get("tss_reqdeliverydate").getValue(); TODO: This is disabled because we require to edit reqdeliv? before, if this null, then no check happens.
    //if (reqdeliv != null && quotation != null) {
    if (quotation != null) {
        XrmServiceToolkit.Rest.Retrieve(
        quotation[0].id,
        "tss_quotationpartheaderSet",
        null, null,
        function (result) {
            re = result;
            /*if((result.tss_statuscode.Value == QUOTATIONSTATUS_DRAFT && result.tss_statusreason.Value == STATUSREASON_OPEN) || (result.tss_statuscode.Value == QUOTATIONSTATUS_APPROVED && result.tss_statusreason.Value == STATUSREASON_APPROVED)){
                Xrm.Page.ui.controls.get("tss_reqdeliverydate").setDisabled(false);
                Xrm.Page.getAttribute("tss_reqdeliverydate").setValue(result.tss_requestdeliverydate);
                Xrm.Page.getAttribute('tss_reqdeliverydate').setSubmitMode("always"); 
                //alert('work');
            }
            else{
                Xrm.Page.getAttribute("tss_reqdeliverydate").setValue(result.tss_requestdeliverydate);
                Xrm.Page.getAttribute('tss_reqdeliverydate').setSubmitMode("always");
                //alert('doesnot work');
            }*/
        }, function (error) {
            alert("failed retrieve request delivery date: " + error.message);
        }, false);

        if ((re.tss_statuscode.Value == QUOTATIONSTATUS_DRAFT && re.tss_statusreason.Value == STATUSREASON_OPEN) || (re.tss_statuscode.Value == QUOTATIONSTATUS_APPROVED && re.tss_statusreason.Value == STATUSREASON_APPROVED)) {
            Xrm.Page.ui.controls.get("tss_reqdeliverydate").setDisabled(false);
            Xrm.Page.getAttribute("tss_reqdeliverydate").setValue(re.tss_requestdeliverydate);
            Xrm.Page.getAttribute('tss_reqdeliverydate').setSubmitMode("always");
        }
        else {
            //disableFormFields(true); //disable all field 
            Xrm.Page.getAttribute("tss_reqdeliverydate").setValue(re.tss_requestdeliverydate);
            Xrm.Page.getAttribute('tss_reqdeliverydate').setSubmitMode("always");
        }
    }
}


//function checkStockQuotationService(){
//    var quotation = Xrm.Page.data.entity.attributes.get("tss_quotationpartheader").getValue();
//    var reqdeliv = Xrm.Page.data.entity.attributes.get("tss_quantity").getValue();
//    if(reqdeliv != null && quotation != null){
//        XrmServiceToolkit.Rest.Retrieve(
//        quotation[0].id,
//        "tss_quotationpartheaderSet",
//        null, null,
//        function (result){
//            Xrm.Page.getAttribute("tss_reqdeliverydate").setValue(result.tss_requestdeliverydate);
//            Xrm.Page.getAttribute('tss_reqdeliverydate').setSubmitMode("always");
//        },function (error){
//            alert("failed retrieve request delivery date: " + error.message);
//        },false);
//    } 
//}

function doesControlHaveAttribute(control) {
    var controlType = control.getControlType();
    return controlType != "iframe" && controlType != "webresource" && controlType != "subgrid";
}
function disableFormFields(onOff) {
    Xrm.Page.ui.controls.forEach(function (control, index) {
        if (doesControlHaveAttribute(control)) {
            control.setDisabled(onOff);
        }
    });
}

function forceSubmitDirtyFields() {
    //var message = "The following fields are dirty: \n";
    Xrm.Page.data.entity.attributes.forEach(function (attribute, index) {
        if (attribute.getIsDirty() == true) {
            attribute.setSubmitMode('always');
            //message += "\u2219 " + attribute.getName() + "\n";
        }
    });
    //alert(message);

}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// On Load
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function FormonLoad() {
    var formType = Xrm.Page.ui.getFormType();
    //Disable Form
    //setupForm();

    checkIsPackage(); //<-- tinggal tambah filter user by PA
    //checkReqDeliveryDate();

    if (formType < 2) {
        setItemNumber();
        // Set Two Options value default on Create Form
        Set_OnLoad();
        if (Xrm.Page.data.entity.attributes.get("tss_quotationpartheader").getValue() != null) {
            SetSourceTypeLines();// set source type as same as header when create
        }
    } else {
        if (Xrm.Page.data.entity.attributes.get("tss_sourcetype").getValue() != source_quoteservice) {
            disableFormFields(true);
        }

        checkReqDeliveryDate();
    }

    preFilterLookupInterchange();

    //Force Submit always
    forceSubmitDirtyFields();

    //refresh ribbon
    refreshRibbonOnChange();
    setFinalPrice();
}

function refreshRibbonOnChange() {
    Xrm.Page.ui.refreshRibbon();
}

function preFilterLookupInterchange() {
    Xrm.Page.getControl("tss_partnumberinterchange")._control && Xrm.Page.getControl("tss_partnumberinterchange")._control.tryCompleteOnDemandInitialization && Xrm.Page.getControl("tss_partnumberinterchange")._control.tryCompleteOnDemandInitialization();
    Xrm.Page.getControl("tss_partnumberinterchange").addPreSearch(function () {
        addLookupFilterInterchange();
    });
}

function addLookupFilterInterchange() {
    var pn = Xrm.Page.getAttribute("tss_partnumber");
    var pnObj = pn.getValue();

    if (pnObj != null) {
        var fetchFilters = "<filter type='and'><condition attribute='tss_partmasterid' uitype='" + pnObj[0].entityType + "' operator='eq' value='" + pnObj[0].id + "'/></filter><filter type='and'><condition attribute='tss_status' uitype='" + pnObj[0].entityType + "' operator='eq' value='" + status_interchange + "'/></filter>";
        Xrm.Page.getControl("tss_partnumberinterchange").addCustomFilter(fetchFilters);
    }
}
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FIELD EVENT AREA
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function setItemNumber() {
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
    var Name;
    if (formType == formStatus.Create) {

        var quotation = Xrm.Page.data.entity.attributes.get("tss_quotationpartheader").getValue();
        if (quotation != null) {
            XrmServiceToolkit.Rest.Retrieve(
			quotation[0].id,
				"tss_quotationpartheaderSet",
			null, null,
			function (result) {
			    Xrm.Page.getAttribute("tss_sourcetype").setValue(parseInt(result.tss_sourcetype.Value));
			},
			function (error) {
			    alert("failed retrieve source type: " + error.message);
			},
			false);
            var itemNumber = Xrm.Page.data.entity.attributes.get("tss_itemnumber").getValue();
            var defaultItemNumber = 10;
            var setIN;
            if (itemNumber == null) {
                XrmServiceToolkit.Rest.RetrieveMultiple(
					"tss_quotationpartlinesSet",
					"$select=*&$filter=tss_quotationpartheader/Id eq (guid'" + quotation[0].id + "')",
				function (result) {
				    if (result.length > 0) {
				        setIN = result[result.length - 1].tss_ItemNumber;
				        setIN = setIN + defaultItemNumber;
				        Xrm.Page.getAttribute("tss_itemnumber").setValue(setIN);
				        Name = quotation[0].name + "-" + setIN;
				    }
				    else {
				        Xrm.Page.getAttribute("tss_itemnumber").setValue(defaultItemNumber);
				        Name = quotation[0].name + "-" + defaultItemNumber;
				    }
				}, function error(error) {
				    alert("Failed to retrieve Quotation Part Lines record.");
				}, function onComplete() {
				    //return;
				}, false);

                Xrm.Page.getAttribute("tss_name").setValue(Name);
            }
        }
    }
}

function partnumber_onChange() {
    try {
        var pricelistID;
        //Get the partnumber lookup off of the record
        var partnumber = Xrm.Page.getAttribute('tss_partnumber').getValue();
        //if partnumber exist, attempt to pull back the entire partnumber record
        if (partnumber != null && partnumber[0].entityType == "trs_masterpart") {
            var partnumberId = partnumber[0].id;
            var serverUrl;
            if (Xrm.Page.context.getClientUrl !== undefined) {
                serverUrl = Xrm.Page.context.getClientUrl();
            } else {
                serverUrl = Xrm.Page.context.getServerUrl();
            }

            //Check Part Number on Sparepart Price with Source Type is
            var temp;
            if (sourcetype == source_directsales) {
                pListPart = 'P1';
            } else if (sourcetype == source_marketsize) {
                pListPart = 'P2';
            } else if (sourcetype == source_contract) {
                pListPart = 'P3';
            } else {
                pListPart = 'P4';
            }

            XrmServiceToolkit.Rest.RetrieveMultiple(
                "tss_pricelistpartSet",
                "$select=*&$filter=tss_Code eq '" + pListPart + "'",
                function (result) {
                    if (result.length > 0) {
                        pricelistID = result[0].tss_pricelistpartId;
                    }
                }, function (error) {
                    alert('Retrieve Price List Part: ' + error.message);
                }, function onComplete() {
                    //alert("DONE.");
                }, false
            );
            XrmServiceToolkit.Rest.RetrieveMultiple(
                "tss_sparepartpricemasterSet",
                "$select=*&$filter=tss_PartMaster/Id eq (guid'" + partnumberId + "') and tss_PriceListPart/Id eq (guid'" + pricelistID + "')",
                function (result) {
                    if (result.length > 0) {
                        //Validasi jika dari MS / DS / CMP / Contract

                        //Checking Sparepart with SourceType to get Sparepart ID / Price / Price List
                        //alert('ID: ' + result[0].tss_sparepartpricemasterId);
                        Xrm.Page.getAttribute("tss_price").setValue(parseInt(result[0].tss_Price.Value));
                        Xrm.Page.getAttribute("tss_minimumprice").setValue(parseInt(result[0].tss_MinimumPrice.Value));
                        //alert(result[0].tss_PriceListPart.Id);

                        var priceList = new Array();
                        priceList[0] = new Object();
                        priceList[0].id = result[0].tss_PriceListPart.Id;
                        priceList[0].entityType = 'tss_PriceListPart'; //<-- Entity Type Price List Part
                        priceList[0].name = result[0].tss_PriceListPart.Name;
                        Xrm.Page.getAttribute("tss_pricetype").setValue(priceList);
                        //Xrm.Page.getAttribute("tss_sourcetype").setValue(parseInt(source_directsales));
                    }
                    else {
                        alert('Empty retrieve Sparepart Price.');
                        Xrm.Page.getAttribute("tss_price").setValue(parseInt(0));
                        Xrm.Page.getAttribute("tss_minimumprice").setValue(parseInt(0));
                        Xrm.Page.getAttribute("tss_partdescription").setValue();
                        Xrm.Page.getAttribute("tss_unitgroup").setValue();
                        Xrm.Page.getAttribute("tss_pricetype").setValue();
                        //Xrm.Page.getAttribute("tss_sourcetype").setValue();
                        Xrm.Page.getAttribute('tss_totalprice').setValue(parseInt(0));
                        Xrm.Page.getAttribute('tss_totalfinalprice').setValue(parseInt(0));
                        Xrm.Page.getAttribute('tss_priceafterdiscount').setValue(parseInt(0));
                        Xrm.Page.getAttribute('tss_discountpercent').setValue();
                        Xrm.Page.getAttribute('tss_discountamount').setValue();
                    }
                }, function (error) {
                    alert('Retrieve Sparepart Price: ' + error.message);
                }, function onComplete() {
                    //alert("DONE.");
                }, false
            );
            var ODataPath = serverUrl + "/XRMServices/2011/OrganizationData.svc";
            var partnumberRequest = new XMLHttpRequest();
            partnumberRequest.open("GET", ODataPath + "/trs_masterpartSet(guid'" + partnumberId + "')", false);
            partnumberRequest.setRequestHeader("Accept", "application/json");
            partnumberRequest.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            partnumberRequest.send();
            //If request was successful, parse the email address
            if (partnumberRequest.status == 200) {
                //Retrieve Value from tss_prospectpartlines
                var re = JSON.parse(partnumberRequest.responseText).d;
                //GET part type & unit group from Spare Part Price Master
                if (re.tss_unitgroup != null) {
                    Xrm.Page.getAttribute("tss_partdescription").setValue(re.trs_PartDescription);
                    //Xrm.Page.getAttribute("tss_parttype").setValue(re.tss_parttype.Value);
                    var setUnit = new Array();
                    setUnit[0] = new Object();
                    setUnit[0].id = re.tss_unitgroup.Id;
                    setUnit[0].entityType = 'uomschedule'; //<-- Entity Type Unit Group
                    setUnit[0].name = re.tss_unitgroup.Name;
                    Xrm.Page.getAttribute("tss_unitgroup").setValue(setUnit);
                } else { alert('Unit Group is empty.'); }
            } else {
                alert('Warning: Request Part Number On Change Failed !');
            }
            Xrm.Page.getAttribute('tss_discountpercent').setValue(parseInt(0));
            Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));
            discPercent_onChange();
        } else {
            //alert('Part Number is empty.');
            Xrm.Page.getAttribute('tss_partnumber').setValue("");
            Xrm.Page.getAttribute("tss_price").setValue(parseInt(0));
            Xrm.Page.getAttribute("tss_minimumprice").setValue(parseInt(0));
            Xrm.Page.getAttribute("tss_partdescription").setValue();
            Xrm.Page.getAttribute("tss_unitgroup").setValue();
            Xrm.Page.getAttribute("tss_pricetype").setValue();
            //Xrm.Page.getAttribute("tss_sourcetype").setValue();
            Xrm.Page.getAttribute('tss_totalprice').setValue(parseInt(0));
            Xrm.Page.getAttribute('tss_totalfinalprice').setValue(parseInt(0));
            Xrm.Page.getAttribute('tss_priceafterdiscount').setValue(parseInt(0));
            Xrm.Page.getAttribute('tss_discountpercent').setValue();
            Xrm.Page.getAttribute('tss_discountamount').setValue();
        }

        Xrm.Page.getAttribute('tss_partnumber').setSubmitMode("always");
    } catch (e) {
        alert('Failed to set on change Part Number : ' + e.message)
    }
    //tss_prospectpartlines

}

function interchange_onChange() {
    //Check Part Master is part interchange
    var directsales = 'Direct Sales Price List';
    var isinterchange = Xrm.Page.getAttribute('tss_isinterchange').getValue();
    if (isinterchange) {
        var interchange;
        var partInter = Xrm.Page.getAttribute('tss_partnumberinterchange').getValue();
        if (partInter != null) {
            XrmServiceToolkit.Rest.Retrieve(
                partInter[0].id,
                "tss_partmasterlinesinterchangeSet",
                null, null,
                function (result) {
                    interchange = result.tss_PartNumberInterchange.Id;
                },
                function (error) {
                    alert("Failed retrieve Part Master - Interchange Lines.");
                },
                false
            );
            //var priceType = Xrm.Page.getAttribute('tss_pricetype').getValue()[0].id;
            //Check on Sparepart Price & Master Part
            XrmServiceToolkit.Rest.RetrieveMultiple(
                "tss_sparepartpricemasterSet",
                "$select=*&$filter=tss_PartMaster/Id eq (guid'" + interchange + "')",
                //and tss_PriceListPart/Id eq (guid'" + priceType + "')", <-- Quotation Lines tidak ada priceType
                function (result) {
                    if (result.length > 0) {
                        //Checking Sparepart with SourceType to get Sparepart ID / Price / Price List
                        //alert('ID: ' + result[0].tss_sparepartpricemasterId);
                        Xrm.Page.getAttribute("tss_price").setValue(parseInt(result[0].tss_Price.Value));
                        Xrm.Page.getAttribute("tss_minimumprice").setValue(parseInt(result[0].tss_MinimumPrice.Value));
                        //alert(result[0].tss_PriceListPart.Id);

                        //Always Direct Sales
                        //var priceList = new Array();
                        //priceList[0] = new Object();
                        //priceList[0].id = result[0].tss_PriceListPart.Id;
                        //priceList[0].entityType = 'tss_PriceListPart'; //<-- Entity Type Price List Part
                        //priceList[0].name = result[0].tss_PriceListPart.Name;
                        //Xrm.Page.getAttribute("tss_pricetype").setValue(priceList);
                        //Xrm.Page.getAttribute("tss_sourcetype").setValue(parseInt(source_directsales));

                    }
                    else {
                        alert("Cannot save the record, price still not setup for this part number");
                        Xrm.Page.getAttribute("tss_price").setValue(parseInt(0));
                        Xrm.Page.getAttribute("tss_minimumprice").setValue(parseInt(0));
                        Xrm.Page.getAttribute("tss_partdescription").setValue();
                        Xrm.Page.getAttribute("tss_unitgroup").setValue();
                        Xrm.Page.getAttribute("tss_pricetype").setValue();
                        Xrm.Page.getAttribute('tss_totalprice').setValue(parseInt(0));
                        Xrm.Page.getAttribute('tss_totalfinalprice').setValue(parseInt(0));
                        Xrm.Page.getAttribute('tss_priceafterdiscount').setValue(parseInt(0));
                        Xrm.Page.getAttribute('tss_discountpercent').setValue();
                        Xrm.Page.getAttribute('tss_discountamount').setValue();
                    }

                }, function (error) {
                    alert('Retrieve Sparepart Price: ' + error.message);
                }, function onComplete() {
                    //alert("DONE.");
                }, false
            );

            //Check on Master Part
            var serverUrl;
            if (Xrm.Page.context.getClientUrl !== undefined) {
                serverUrl = Xrm.Page.context.getClientUrl();
            } else {
                serverUrl = Xrm.Page.context.getServerUrl();
            }
            var ODataPath = serverUrl + "/XRMServices/2011/OrganizationData.svc";
            var partnumberRequest = new XMLHttpRequest();
            partnumberRequest.open("GET", ODataPath + "/trs_masterpartSet(guid'" + interchange + "')", false);
            partnumberRequest.setRequestHeader("Accept", "application/json");
            partnumberRequest.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            partnumberRequest.send();

            //If request was successful, parse the email address
            if (partnumberRequest.status == 200) {
                //Retrieve Value from tss_prospectpartlines
                var re = JSON.parse(partnumberRequest.responseText).d;

                //GET part type & unit group from Spare Part Price Master
                if (re.tss_unitgroup != null && re.trs_PartDescription != null) {
                    Xrm.Page.getAttribute("tss_partdescription").setValue(re.trs_PartDescription);
                    var setUnit = new Array();
                    setUnit[0] = new Object();
                    setUnit[0].id = re.tss_unitgroup.Id;
                    setUnit[0].entityType = 'uomschedule'; //<-- Entity Type Unit Group
                    setUnit[0].name = re.tss_unitgroup.Name;
                    Xrm.Page.getAttribute("tss_unitgroup").setValue(setUnit);

                    Xrm.Page.getAttribute("tss_partdescription").setSubmitMode("always");
                    Xrm.Page.getAttribute("tss_unitgroup").setSubmitMode("always");
                } else { alert('Unit Group is empty.'); }
            } else {
                alert('Warning: Request Part Number Interchange Failed !');
                Xrm.Page.getAttribute("tss_price").setValue(parseInt(0));
                Xrm.Page.getAttribute("tss_minimumprice").setValue(parseInt(0));
                Xrm.Page.getAttribute("tss_partdescription").setValue();
                Xrm.Page.getAttribute("tss_parttype").setValue();
                Xrm.Page.getAttribute("tss_unitgroup").setValue();
                Xrm.Page.getAttribute('tss_priceamount').setValue(parseInt(0));
                Xrm.Page.getAttribute('tss_finalprice').setValue(parseInt(0));
                Xrm.Page.getAttribute('tss_discountpercent').setValue(parseInt(0));
                Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));
            }
            Xrm.Page.getAttribute('tss_discountpercent').setValue(parseInt(0));
            Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));
            discPercent_onChange();
        } else {
            Xrm.Page.getAttribute("tss_price").setValue(parseInt(0));
            Xrm.Page.getAttribute("tss_minimumprice").setValue(parseInt(0));
            Xrm.Page.getAttribute("tss_partdescription").setValue();
            Xrm.Page.getAttribute("tss_unitgroup").setValue();
            Xrm.Page.getAttribute("tss_pricetype").setValue();
            partnumber_onChange();
        }
    }
}

function isinterchange_onChange() {
    var isinterchange = Xrm.Page.getAttribute('tss_isinterchange');
    if (!isinterchange.getValue()) {
        partnumber_onChange();
        Xrm.Page.getAttribute('tss_partnumberinterchange').setValue();
    } else {
        interchange_onChange();
    }
}

function discPercent_onChange(quantity) {
    var minprice = Xrm.Page.getAttribute('tss_minimumprice').getValue();
    var discount = Xrm.Page.getAttribute('tss_discountpercent').getValue();
    var price = Xrm.Page.getAttribute('tss_price').getValue();
    var qty = quantity;
    var formulaPercent;
    var formulaAmount;
    var priceamount;
    var priceafterdiscount;
    var totalprice;
    var finalprice;
    var totalfinalprice;
    //var qty = Xrm.Page.getAttribute('tss_quantity').getValue();
    if (qty == null) {
        qty = Xrm.Page.getAttribute('tss_quantity').getValue();
        formulaAmount = Math.round(price * (discount / 100));
        //var formulaAmount = (price*(discount/100));
        //alert('Discount Amount:Rp ' + formulaAmount);
        formulaPercent = (formulaAmount / price) * 100;
        if (qty > 0) {
            priceamount = ((price - formulaAmount) * qty);
            priceafterdiscount = (price - formulaAmount);
            //Xrm.Page.getAttribute('tss_priceafterdiscount').setValue(parseInt(priceafterdiscount));
            if (formulaAmount != 0) {
                Xrm.Page.getAttribute('tss_priceafterdiscount').setValue(parseInt(priceafterdiscount));
            } else { Xrm.Page.getAttribute('tss_priceafterdiscount').setValue(parseInt(0)); }
            if (priceafterdiscount < minprice) {
                Xrm.Page.getAttribute('tss_underminimumprice').setValue(true);
            } else {
                Xrm.Page.getAttribute('tss_underminimumprice').setValue(false);
            }
            if (discount == null || formulaAmount == null) {
                Xrm.Page.getAttribute('tss_discountpercent').setValue(0);
                Xrm.Page.getAttribute('tss_discountamount').setValue(0);
            } else {
                Xrm.Page.getAttribute('tss_discountamount').setValue(formulaAmount);
            }
            //finalprice = (priceamount / qty);
            //Xrm.Page.getAttribute('tss_finalprice').setValue(parseInt(finalprice));
            if (!checkIsPackage()) {
                Xrm.Page.getAttribute('tss_totalfinalprice').setValue(parseInt(priceamount));
            }
            else {
                if (Xrm.Page.getAttribute('tss_finalprice').getValue() != null) {
                    Xrm.Page.getAttribute('tss_totalfinalprice').setValue(Xrm.Page.getAttribute('tss_finalprice').getValue() * qty);
                }
                else {
                    Xrm.Page.getAttribute('tss_totalfinalprice').setValue(0);
                }
            }
            Xrm.Page.getAttribute('tss_totalprice').setValue(parseInt(priceamount));
        } else {
            Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));
            Xrm.Page.getAttribute('tss_discountpercent').setValue(0);
            //Xrm.Page.getAttribute('tss_finalprice').setValue(parseInt(0));
            Xrm.Page.getAttribute('tss_totalfinalprice').setValue(parseInt(0));
            Xrm.Page.getAttribute('tss_totalprice').setValue(parseInt(0));
            //alert('Please fill the quantity on Quotation Part Lines !');
        }
    }
    else {
        if (qty > 0) {
            if (discount == 0) {
                priceamount = (price * qty);
                Xrm.Page.getAttribute('tss_priceafterdiscount').setValue(parseInt(0));
                Xrm.Page.getAttribute('tss_totalprice').setValue(parseInt(priceamount));
                //Xrm.Page.getAttribute('tss_totalfinalprice').setValue(parseInt(priceamount));
                if (!checkIsPackage()) {
                    Xrm.Page.getAttribute('tss_totalfinalprice').setValue(parseInt(priceamount));
                }
                else {
                    if (Xrm.Page.getAttribute('tss_finalprice').getValue() != null) {
                        Xrm.Page.getAttribute('tss_totalfinalprice').setValue(Xrm.Page.getAttribute('tss_finalprice').getValue() * qty);
                    }
                    else {
                        Xrm.Page.getAttribute('tss_totalfinalprice').setValue(0);
                    }
                }
            }
        } else {
            Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));
            Xrm.Page.getAttribute('tss_discountpercent').setValue(0);
            Xrm.Page.getAttribute('tss_priceafterdiscount').setValue(parseInt(priceafterdiscount));
            Xrm.Page.getAttribute('tss_priceamount').setValue(parseInt(0));
            Xrm.Page.getAttribute('tss_totalfinalprice').setValue(parseInt(priceamount));
            //alert('Please fill the quantity on Quotation Part Lines !');
        }
    }

    Xrm.Page.getAttribute('tss_finalprice').setSubmitMode("always");
    Xrm.Page.getAttribute('tss_underminimumprice').setSubmitMode("always");
    Xrm.Page.getAttribute('tss_discountamount').setSubmitMode("always");
    Xrm.Page.getAttribute('tss_discountpercent').setSubmitMode("always");
    Xrm.Page.getAttribute('tss_totalfinalprice').setSubmitMode("always");
    Xrm.Page.getAttribute('tss_totalprice').setSubmitMode("always");
}

function discAmount_onChange(quantity) {
    var qty;
    var minprice = Xrm.Page.getAttribute('tss_minimumprice').getValue();
    var discount = Xrm.Page.getAttribute('tss_discountamount').getValue();
    var price = Xrm.Page.getAttribute('tss_price').getValue();
    var priceafterdiscount;
    var totalprice;
    var finalprice;
    var totalfinalprice;
    var priceamount;
    var formulaPercent = (discount / price) * 100;
    if (quantity == null) {
        qty = Xrm.Page.getAttribute('tss_quantity').getValue();
        //Change Formula Amount: Price*(Discount Percent/100)
        var formulaAmount = Math.round(price * (formulaPercent / 100));
        if (qty > 0) {
            var priceamount = ((price - formulaAmount) * qty);
            var priceafterdiscount = (price - formulaAmount);
            //Xrm.Page.getAttribute('tss_priceafterdiscount').setValue(parseInt(priceafterdiscount));
            if (formulaAmount != 0) {
                Xrm.Page.getAttribute('tss_priceafterdiscount').setValue(parseInt(priceafterdiscount));
            } else { Xrm.Page.getAttribute('tss_priceafterdiscount').setValue(parseInt(0)); }
            if (priceafterdiscount < minprice) {
                Xrm.Page.getAttribute('tss_underminimumprice').setValue(true);
            } else {
                Xrm.Page.getAttribute('tss_underminimumprice').setValue(false);
            }
            if (discount == null || formulaPercent == null) {
                Xrm.Page.getAttribute('tss_discountamount').setValue(0);
                Xrm.Page.getAttribute('tss_discountpercent').setValue(0);
            } else {
                Xrm.Page.getAttribute('tss_discountpercent').setValue(formulaPercent);
            }
            finalprice = (priceamount / qty);
            //Xrm.Page.getAttribute('tss_finalprice').setValue(parseInt(finalprice));
            if (!checkIsPackage()) {
                Xrm.Page.getAttribute('tss_totalfinalprice').setValue(parseInt(priceamount));
            }
            else {
                if (Xrm.Page.getAttribute('tss_finalprice').getValue() != null) {
                    Xrm.Page.getAttribute('tss_totalfinalprice').setValue(Xrm.Page.getAttribute('tss_finalprice').getValue() * qty);
                }
                else {
                    Xrm.Page.getAttribute('tss_totalfinalprice').setValue(0);
                }
            }
            //Xrm.Page.getAttribute('tss_totalfinalprice').setValue(parseInt(finalprice * qty));
            Xrm.Page.getAttribute('tss_totalprice').setValue(parseInt(priceamount));
        } else {
            Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));
            Xrm.Page.getAttribute('tss_discountpercent').setValue(parseInt(0));
            //Xrm.Page.getAttribute('tss_finalprice').setValue(parseInt(0));
            Xrm.Page.getAttribute('tss_totalfinalprice').setValue(parseInt(0));
            Xrm.Page.getAttribute('tss_totalprice').setValue(parseInt(0));
            //alert('Please fill the quantity on Quatation Part Lines !');
        }
    }
    else {
        qty = quantity;
        if (qty > 0) {
            if (discount == 0) {
                priceamount = (price * qty);
                finalprice = priceamount / qty;
                Xrm.Page.getAttribute('tss_priceafterdiscount').setValue(parseInt(0));
                //Xrm.Page.getAttribute('tss_totalfinalprice').setValue(parseInt(finalprice * qty));
                Xrm.Page.getAttribute('tss_totalprice').setValue(parseInt(priceamount));
                if (!checkIsPackage()) {
                    Xrm.Page.getAttribute('tss_totalfinalprice').setValue(parseInt(priceamount));
                }
                else {
                    if (Xrm.Page.getAttribute('tss_finalprice').getValue() != null) {
                        Xrm.Page.getAttribute('tss_totalfinalprice').setValue(Xrm.Page.getAttribute('tss_finalprice').getValue() * qty);
                    }
                    else {
                        Xrm.Page.getAttribute('tss_totalfinalprice').setValue(0);
                    }
                }
            }
        } else {
            Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));
            Xrm.Page.getAttribute('tss_discountpercent').setValue(0);
            Xrm.Page.getAttribute('tss_totalprice').setValue(parseInt(0));
            //Xrm.Page.getAttribute('tss_finalprice').setValue(parseInt(0));
            Xrm.Page.getAttribute('tss_totalfinalprice').setValue(parseInt(0));
            //alert('Please fill the quantity on Quotation Part Lines !');
        }
    }

    //Xrm.Page.getAttribute('tss_finalprice').setSubmitMode("always");
    Xrm.Page.getAttribute('tss_underminimumprice').setSubmitMode("always");
    Xrm.Page.getAttribute('tss_discountamount').setSubmitMode("always");
    Xrm.Page.getAttribute('tss_discountpercent').setSubmitMode("always");
    Xrm.Page.getAttribute('tss_totalfinalprice').setSubmitMode("always");
    Xrm.Page.getAttribute('tss_totalprice').setSubmitMode("always");
}

function qty_onChange() {
    var qty = Xrm.Page.getAttribute('tss_quantity').getValue();
    var discP = Xrm.Page.getAttribute('tss_discountpercent').getValue();
    var discA = Xrm.Page.getAttribute('tss_discountamount').getValue();
    discAmount_onChange(qty);
    discPercent_onChange(qty);
    Xrm.Page.getAttribute('tss_totalprice').setSubmitMode("always");
}

function setFinalPrice() {
    var flag = false;
    var flagsecond = false;
    var quotation = Xrm.Page.getAttribute("tss_quotationpartheader").getValue();
    if (quotation != null) {
        XrmServiceToolkit.Rest.Retrieve(quotation[0].id, "tss_quotationpartheaderSet", null, null, function (result) {
            if (result.tss_package) {
                flag = true;
            }
        }, function (error) {
            alert("failed retrieve package is: " + error.message);
        }, false);

        if (flag == true) {
            var userid = Xrm.Page.context.getUserId();
            var entityid = Xrm.Page.data.entity.getId();
            if (userid != null && entityid != null) {
                XrmServiceToolkit.Rest.RetrieveMultiple("tss_approverlistSet", "?$select=tss_approverlistId&$filter=tss_Approver/Id eq (guid'" + userid + "') and tss_QuotationPartLinesId/Id eq (guid'" + entityid + "')", function (results) {
                    if (results.length > 0) {
                        flagsecond = true;
                    }
                }, function (error) {
                    Xrm.Utility.alertDialog(error.message);
                }, function () {
                    //On Complete - Do Something
                }, false);
            }

            if (flagsecond == true && (Xrm.Page.getAttribute("tss_approvebypa").getValue() !== true)) {
                //if (flagsecond == true) {
                Xrm.Page.ui.controls.get("tss_finalprice").setDisabled(false);
                if (Xrm.Page.getAttribute("tss_finalprice").getValue() == null) {
                    Xrm.Page.getAttribute("tss_finalprice").setValue(0);
                    Xrm.Page.getAttribute("tss_finalprice").setSubmitMode("always");
                }
            }
        }
    }
}

function finalPriceOnChange() {
    var qty = Xrm.Page.getAttribute('tss_quantity').getValue();
    if (Xrm.Page.getAttribute('tss_finalprice').getValue() != null) {
        Xrm.Page.getAttribute('tss_totalfinalprice').setValue(Xrm.Page.getAttribute('tss_finalprice').getValue() * qty);
        Xrm.Page.getAttribute("tss_totalfinalprice").setSubmitMode("always");
        Xrm.Utility.openEntityForm(Xrm.Page.data.entity.getEntityName(), Xrm.Page.data.entity.getId());
    }
    else {
        Xrm.Page.getAttribute('tss_totalfinalprice').setValue(0);
        Xrm.Page.getAttribute("tss_totalfinalprice").setSubmitMode("always");
    }
}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// PUBLICS AREA
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function approvePackage() {
    try {
        //check final price
        Xrm.Page.getAttribute("tss_finalprice").setSubmitMode("always");
        var finalPrice = Xrm.Page.getAttribute("tss_finalprice").getValue();
        if (finalPrice != null) {
            if (finalPrice <= 0) {
                alert("Please input final price!");
            }
            else {
                var workflowId = 'D791B57B-92B1-412F-8FE1-4997E5289B34';
                var workflowName = 'Approve Package';
                ExecuteWorkflow(workflowId, workflowName, function () {
                    RefreshForm();
                });
            }
        }
        else {
            alert("Please input final price!");
        }
    }
    catch (e) {
        alert('Exception on trigger workflow Approve Package: ' + e.message);
    }
}

function ExecuteWorkflow(workflowId, workflowName, successCallback, failedCallback) {
    var _return = window.confirm('Do you want to ' + workflowName + ' ?');
    if (_return) {
        //var url = Xrm.Page.context.getServerUrl();
        var url = document.location.protocol + "//" + document.location.host + "/" + Xrm.Page.context.getOrgUniqueName();
        var entityId = Xrm.Page.data.entity.getId();
        var OrgServicePath = "/XRMServices/2011/Organization.svc/web";
        url = url + OrgServicePath;
        var request;
        request = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
			"<s:Body>" +
			"<Execute xmlns=\"http://schemas.microsoft.com/xrm/2011/Contracts/Services\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">" +
			"<request i:type=\"b:ExecuteWorkflowRequest\" xmlns:a=\"http://schemas.microsoft.com/xrm/2011/Contracts\" xmlns:b=\"http://schemas.microsoft.com/crm/2011/Contracts\">" +
			"<a:Parameters xmlns:c=\"http://schemas.datacontract.org/2004/07/System.Collections.Generic\">" +
			"<a:KeyValuePairOfstringanyType>" +
			"<c:key>EntityId</c:key>" +
			"<c:value i:type=\"d:guid\" xmlns:d=\"http://schemas.microsoft.com/2003/10/Serialization/\">" + entityId + "</c:value>" +
			"</a:KeyValuePairOfstringanyType>" +
			"<a:KeyValuePairOfstringanyType>" +
			"<c:key>WorkflowId</c:key>" +
			"<c:value i:type=\"d:guid\" xmlns:d=\"http://schemas.microsoft.com/2003/10/Serialization/\">" + workflowId + "</c:value>" +
			"</a:KeyValuePairOfstringanyType>" +
			"</a:Parameters>" +
			"<a:RequestId i:nil=\"true\" />" +
			"<a:RequestName>ExecuteWorkflow</a:RequestName>" +
			"</request>" +
			"</Execute>" +
			"</s:Body>" +
			"</s:Envelope>";
        var req = new XMLHttpRequest();
        req.open("POST", url, true)
        // Responses will return XML. It isn't possible to return JSON.
        req.setRequestHeader("Accept", "application/xml, text/xml, */*");
        req.setRequestHeader("Content-Type", "text/xml; charset=utf-8");
        req.setRequestHeader("SOAPAction", "http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Execute");
        req.onreadystatechange = function () {
            AssignResponse(req, workflowName, successCallback, failedCallback);
        };
        req.send(request);
    }
}

function AssignResponse(req, workflowName, successCallback, failedCallback) {
    if (req.readyState == 4) {
        if (req.status == 200) {
            alert('Successfully executed ' + workflowName + '.');
            if (successCallback !== undefined && typeof successCallback === "function") {
                successCallback();
            }
        }
        else {
            var faultstring = req.responseXML.getElementsByTagName("faultstring")[0].textContent;
            alert('Fail to execute ' + workflowName + '.\r\n Response Code : (' + req.status + ')' + "\r\nPlease contact your IT support.\r\nTechnical Details :" + faultstring);
            if (failedCallback !== undefined && typeof failedCallback === "function") {
                failedCallback(new Error(faultstring));
            }
        }
    }
}

function RefreshForm() {
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
    var check = [];
    check.push(function () {
        return formType !== formStatus.Create;
    });
    var boolArrayCheck = function (predicateArray) {
        //Assume predicateArray is right
        var boolCheck = true;
        for (var index = 0; index < predicateArray.length; index++) {
            boolCheck = boolCheck && predicateArray[index]();
        }
        return boolCheck;
    }
    if (boolArrayCheck(check)) {
        Xrm.Utility.openEntityForm(Xrm.Page.data.entity.getEntityName(), Xrm.Page.data.entity.getId());
    }
}

function SetSourceTypeLines() {
    var reqDeliveryDate;
    var optionSetValue;
    var re;
    var lookupObject = Xrm.Page.getAttribute("tss_quotationpartheader");
    if (lookupObject != null) {
        var lookUpObjectValue = lookupObject.getValue();
        if ((lookUpObjectValue != null)) {
            var lookupid = lookUpObjectValue[0].id;
            var lookupname = lookUpObjectValue[0].name;
        }
        XrmServiceToolkit.Rest.Retrieve(
        lookupid,
            "tss_quotationpartheaderSet",
            null, null,

            function (result) {
                //SourceType = result[0].trs_Product;
                //alert('success');
                re = result;
            },
            function (error) {
                alert(error.message);
            },
            false);

        //alert(SourceType.tss_sourcetype.Value);

        reqDeliveryDate = re.tss_requestdeliverydate;
        Xrm.Page.data.entity.attributes.get("tss_reqdeliverydate").setValue(reqDeliveryDate);

        optionSetValue = re.tss_sourcetype.Value;
        if (optionSetValue == 865920002) //if get service from quotation header
        {
            optionSetValue = source_quoteservice; //convert to be quotation from service
            Xrm.Page.data.entity.attributes.get("tss_sourcetype").setValue(optionSetValue);
        }
        else if (optionSetValue == null) {
            alert('There is no source type found in this Quotation Part, please check the source type.');
            Xrm.Page.ui.close();
        }
    }
}

function displayRuleApprovePackage() {
    //var checkApprove = true;

    var re;
    var quotationstatus_in_progress = 865920001;
    var quotation = Xrm.Page.data.entity.attributes.get("tss_quotationpartheader").getValue();
    var approvebyPA = Xrm.Page.data.entity.attributes.get("tss_approvebypa").getValue();

    XrmServiceToolkit.Rest.Retrieve(
    quotation[0].id,
    "tss_quotationpartheaderSet",
    null, null,
    function (result) {
        re = result;
        /*if(result.length > 0){
            //if(result.tss_statusreason.Value != statusreason_waitingapprovalpackage && !result.tss_approvediscount && !approvebyPA){
            if(result.tss_statusreason == statusreason_waitingapprovalpackage && result.tss_package == true && approvebyPA == false){
                //checkApprove = false;
                alert('true');
                return true;
            }
            else
            {
                alert('false');
                return false;
            }
        }*/

        //if(re.tss_statusreason.Value == statusreason_waitingapprovalpackage && re.tss_package == true && approvebyPA == false)
        /*if((result.tss_statusreason.Value == statusreason_waitingapprovalpackage || result.tss_statuscode.Value == quotationstatus_in_progress) && re.tss_package == true && approvebyPA == false)
        {
            return false;
        }
        else
        {
            return false;
        }*/

    }, function (error) {
        alert("Failed to retrieve quotation part: " + error.message);
    }, false);


    /*if(!checkApprove){
        return false;
    }else{
        return true;
    }*/

    if (CheckUserPrivilegeApprove() == true && (re.tss_statusreason.Value == statusreason_waitingapprovalpackage || re.tss_statuscode.Value == quotationstatus_in_progress) && re.tss_package == true && approvebyPA == false) {
        return true;
    }
    else {
        return false;
    }

    //return checkApprove;
}

function CheckUserPrivilegeApprove() {

    try {
        var flag = true;

        var lookupObject = Xrm.Page.getAttribute("tss_quotationpartheader");
        if (lookupObject != null) {
            var lookUpObjectValue = lookupObject.getValue();
            if ((lookUpObjectValue != null)) {
                var lookupid = lookUpObjectValue[0].id;
            }
        }

        var partID = Xrm.Page.data.entity.getId();
        var UserID = Xrm.Page.context.getUserId();

        //XrmServiceToolkit.Rest.RetrieveMultiple(
        //"tss_approverlistSet",
        //"$select=*&$filter=tss_QuotationPartHeaderId/Id eq (guid'" + lookupid + "') and tss_Approver/Id eq (guid'" + UserID + "') and tss_QuotationPartLinesId/Id eq (guid'" + partID + "')",
        //function (results) {
        //    if (results.length == 0) {
        //        flag = false;
        //    }
        //},
        //function (error) {
        //    alert('Retrieve Approval List: ' + error.message);
        //},
        //function onComplete() {
        //    //alert(" records should have been retrieved.");
        //},
        //false
        //);

        XrmServiceToolkit.Rest.RetrieveMultiple(
        "tss_approverlistSet",
        "$select=*&$filter=tss_Approver/Id eq (guid'" + UserID + "') and tss_QuotationPartLinesId/Id eq (guid'" + partID + "')",
        function (results) {
            if (results.length == 0) {
                flag = false;
            }
        },
        function (error) {
            alert('Retrieve Approval List: ' + error.message);
        },
        function onComplete() {
            //alert(" records should have been retrieved.");
        },
        false
        );

        if (flag) {
            return true;
        }
        else {
            return false;
        }
        //return flag;
    }
    catch (ex) {
        alert('Check User Privilege Approve ' + ex.message)
    }

}


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// ON SAVE AREA
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function preventSave(econtext) {
    var formType = Xrm.Page.ui.getFormType();
    var eventArgs = econtext.getEventArgs();
    var PriceNull = Xrm.Page.getAttribute("tss_price").getValue();
    var part = Xrm.Page.getAttribute("tss_partnumber").getValue();
    var isinterchange = Xrm.Page.getAttribute("tss_isinterchange").getValue();
    var quotID = Xrm.Page.getAttribute("tss_quotationpartheader").getValue()[0].id;
    var id = Xrm.Page.data.entity.getId();
    var checkPart = false;
    if (formType < 2) {
        if (isinterchange) {
            var partinterchange = Xrm.Page.getAttribute("tss_partnumberinterchange").getValue();
            XrmServiceToolkit.Rest.RetrieveMultiple(
                "tss_quotationpartlinesSet",
                "$select=*&$filter=tss_quotationpartheader/Id eq (guid'" + quotID + "') and tss_PartNumberInterchange/Id eq (guid'" + partinterchange[0].id + "')",
            function (result) {
                if (result.length > 0) {
                    checkPart = true;
                }
            }, function (error) {
                alert('preventSave(): ' + error.message);
            }, function onComplete() {
                //alert(" records should have been retrieved.");
            }, false);
        } else {
            XrmServiceToolkit.Rest.RetrieveMultiple(
                "tss_quotationpartlinesSet",
                "$select=*&$filter=tss_quotationpartheader/Id eq (guid'" + quotID + "') and tss_Partnumber/Id eq (guid'" + part[0].id + "')",
            function (result) {
                if (result.length > 0) {
                    checkPart = true;
                }
            }, function (error) {
                alert('preventSave(): ' + error.message);
            }, function onComplete() {
                //alert(" records should have been retrieved.");
            }, false);
        }
    }


    if (PriceNull == null || PriceNull == 0) {
        //disable all save (auto save,save, save and close)
        if (eventArgs.getSaveMode() == 70 || eventArgs.getSaveMode() == 1 || eventArgs.getSaveMode() == 2 || eventArgs.getSaveMode() == 59 || eventArgs.getSaveMode() == 80) {
            eventArgs.preventDefault();
            alert("Cannot save the record, price still not setup for this part number");
        }
    }
    if (checkPart == true) {
        //disable all save (auto save,save, save and close)
        if (eventArgs.getSaveMode() == 70 || eventArgs.getSaveMode() == 1 || eventArgs.getSaveMode() == 2 || eventArgs.getSaveMode() == 59 || eventArgs.getSaveMode() == 80) {
            eventArgs.preventDefault();
            alert("Cannot save the record, part / part interchange already exist on this Quotation Part.");
        }
    }
}