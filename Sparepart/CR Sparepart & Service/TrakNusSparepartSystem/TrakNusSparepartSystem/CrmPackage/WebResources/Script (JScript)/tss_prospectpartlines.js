///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// Helper
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//- Event : On Change Field
///<reference path="MSXRMTOOLS.Xrm.Page.2016.js"/>
var source_directsales = 865920000;
var source_marketsize = 865920001;
var source_contract = 865920002;
var source_campaign = 865920003;

var price_directsales = 865920000;
var price_marketsize = 865920001;
var price_contract = 865920002;
var price_campaign = 865920003;

var status_interchange = 865920000;

function SetDefaultValue_TwoOptionField(fieldname) {
    var attr = Xrm.Page.getAttribute(fieldname);
    if (attr != null) {
        attr.setValue(1);
        attr.setValue(0);
    }

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

function GetAttributeValue(attribute) {
    var result = null;

    if (attribute != null) {
        if (attribute.getValue() != null) {
            result = attribute.getValue();
        }
    }

    return result;
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FORM ON LOAD AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//- Event : On load Field
function onLoad() {
    var formType = Xrm.Page.ui.getFormType();
    //Disable Form
    setupForm();
    //alert('Prospect Part Lines');
    //setSourceType();
    //updatePriceType();
    setItemNumber();

    if (formType < 2) {
        Set_OnLoad();
    }
    preFilterLookupInterchange();

    //Force Submit always
    forceSubmitDirtyFields();
    visible_field();
}

function visible_field() {
    var _sourcetype = GetAttributeValue(Xrm.Page.getAttribute("tss_sourcetype"));

    if (_sourcetype != null) {
        if (_sourcetype == 865920000) {
            // DIRECT SALES
            Xrm.Page.getAttribute("tss_sourcetype").setRequiredLevel("none");
            Xrm.Page.getControl("tss_sourcetype").setVisible(false);
        }
        else if (_sourcetype == 865920001) {
            // MARKET SIZE
            Xrm.Page.getAttribute("tss_sourcetype").setRequiredLevel("none");
            Xrm.Page.getControl("tss_sourcetype").setVisible(true);
        }
    }
    else {
        Xrm.Page.getAttribute("tss_sourcetype").setRequiredLevel("required");
        Xrm.Page.getControl("tss_sourcetype").setVisible(true);
    }
}


//Disable Form
var pipelinephase_inquiry = 865920000;
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
function setupForm() {
    var pipeline;
    var cekPipeline = Xrm.Page.data.entity.attributes.get("tss_prospectpartheader").getValue();
    if (cekPipeline != null) {
        cekPipeline = cekPipeline[0].id;
        XrmServiceToolkit.Rest.Retrieve(
            cekPipeline,
            "tss_prospectpartheaderSet",
            null, null,
            function (result) {
                pipeline = result.tss_Pipelinephase.Value;
            },
            function (error) {
                alert("failed retrieve on function setupForm: " + error.message);
            },
            false
        );
        if (Xrm.Page.ui.getFormType() == 2 && pipeline != pipelinephase_inquiry) {
            disableFormFields(true);
        }
    }
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


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// PUBLICS AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function Set_OnLoad() {
    Xrm.Page.getAttribute("tss_discountamount").setValue(0);
    Xrm.Page.getAttribute("tss_discountpercent").setValue(0);
    Xrm.Page.getAttribute("tss_quantity").setValue(0);
    Xrm.Page.getAttribute("tss_priceamount").setValue(0);
    Xrm.Page.getAttribute("tss_finalprice").setValue(0);
    SetDefaultValue_TwoOptionField("tss_discountby");
}

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
    if (formType == formStatus.Create) {
        var prospect = Xrm.Page.data.entity.attributes.get("tss_prospectpartheader").getValue();
        if (prospect != null) {
            Xrm.Page.getAttribute("tss_isinterchange").setValue(1);
            Xrm.Page.getAttribute("tss_isinterchange").setValue(0);
            Xrm.Page.getAttribute("tss_discountoutcontract").setValue(1);
            Xrm.Page.getAttribute("tss_discountoutcontract").setValue(0);
            Xrm.Page.getAttribute("tss_discountby").setValue(0);
            Xrm.Page.getAttribute("tss_underminimumprice").setValue(1);
            Xrm.Page.getAttribute("tss_underminimumprice").setValue(0);
            var prospect = Xrm.Page.data.entity.attributes.get("tss_prospectpartheader").getValue();
            XrmServiceToolkit.Rest.Retrieve(
                prospect[0].id,
                "tss_prospectpartheaderSet",
                null, null,
                function (result) {
                    Xrm.Page.getAttribute("tss_sourcetype").setValue(parseInt(result.tss_Sourcetype.Value));
                },
                function (error) {
                    alert("failed retrieve source type: " + error.message);
                },
                false
            );
            //alert(prospect[0].id);
            var itemNumber = Xrm.Page.data.entity.attributes.get("tss_itemnumber").getValue();
            //alert(itemNumber);
            var defaultItemNumber = 10;
            var setIN;
            if (itemNumber == null) {
                XrmServiceToolkit.Rest.RetrieveMultiple(
                    "tss_prospectpartlinesSet",
                    "$select=*&$filter=tss_ProspectPartHeader/Id eq (guid'" + prospect[0].id + "')",
                    function (result) {
                        if (result.length > 0) {
                            //                            for(var i = 0; i < result.length; i++) {
                            //                                setIN = result[i].tss_ItemNumber;
                            //                            }
                            setIN = result[result.length - 1].tss_ItemNumber;
                            setIN = setIN + defaultItemNumber;
                            Xrm.Page.getAttribute("tss_itemnumber").setValue(setIN);
                        } else {
                            Xrm.Page.getAttribute("tss_itemnumber").setValue(defaultItemNumber);
                        }
                    }, function error(error) {
                        alert("Failed to retrieve Prospect Part Lines record.");
                    }, function onComplete() {
                        //return;
                    }, false
                );
            }
        } else {
            alert('Prospect Part Lines must be created on Prospect Part !');
            Xrm.Page.ui.close();
        }
    }

}

function setUnderMinPrice() {
    var amdisc = Xrm.Page.getAttribute("tss_discountamount").getValue();
    var price = Xrm.Page.getAttribute("tss_price").getValue();
    var redprice = price - amdisc;
    var minprice = Xrm.Page.getAttribute("tss_minimumprice").getValue();
    var entity = {};
    var prosID = Xrm.Page.getAttribute("tss_prospectpartheader").getValue()[0].id;
    alert(prosID);
    if (redprice < minprice) {
        Xrm.Page.getAttribute("tss_underminimumprice").setValue(1);
        entity.tss_underminimumprice = { Value: 1 };
        XrmServiceToolkit.Rest.Update(
            prosID,
            entity,
            "tss_prospectpartheaderSet",
            function (result) {
                alert("Successfully updated Minimum Price on Prospect Part");
            },
            function (error) {
                alert("Failed to Update Prospect Part Header: " + error.message);
            },
            false
        );
    }
    else {
        Xrm.Page.getAttribute("tss_underminimumprice").setValue(0);
        entity.tss_underminimumprice = { Value: 0 };
        XrmServiceToolkit.Rest.Update(
            prosID,
            entity,
            "tss_prospectpartheaderSet",
            function (result) {
                alert("Successfully updated Minimum Price on Prospect Part");
            },
            function (error) {
                alert("Failed to Update Prospect Part Header: " + error.message);
            },
            false
        );
    }
}

function updatePriceType() {
    var prospectpartHeaderGUID = Xrm.Page.data.entity.attributes.get("tss_prospectpartheader").getValue()[0];
    Retrieve(prospectpartHeaderGUID.id);
    //var partNumberGUID = Xrm.Page.data.entity.attributes.get("tss_partnumber").getValue()[0].id;
}

function UpdateSourceType()
{
    var sourceType = Xrm.Page.getAttribute("tss_sourcetype").getValue();

    if (sourceType != null) {
        if (sourceType == 865920000) { //DirectSales
            partnumber_onChange();

            Xrm.Page.ui.controls.get("tss_sourcetype").setVisible(false)
            Xrm.Page.getAttribute("tss_sourcetype").setValue(865920000); //change to direct sales
            Xrm.Page.getAttribute("tss_sourcetype").setSubmitMode("always");

            Xrm.Page.getAttribute("tss_refmarketsize").setValue(null);
            Xrm.Page.getAttribute("tss_refmarketsizebypmperiod").setValue(null);

            Xrm.Page.getAttribute("tss_qtymarketsize").setValue(null);
            Xrm.Page.getAttribute("tss_qtymarketsize").setSubmitMode("always");
        }
        //else if (sourceType == 865920001) { //MarketSize
            
        //}
    }
}

function Retrieve(guid) {
    var contract = 865920002;
    XrmServiceToolkit.Rest.Retrieve(
        guid,
        "tss_prospectpartheaderSet",
        null, null,
        function (result) {
            var source = result.tss_Sourcetype.Value;
            Xrm.Page.getAttribute("tss_sourcetype").setValue(source);
            if (source == contract) {
                Xrm.Page.getAttribute("tss_discountoutcontract").setValue(false);
            } else {
                Xrm.Page.getAttribute("tss_discountoutcontract").setValue(true);
            }
        },
        function (error) {
            alert("Failed Retrieve Source Type!");
        }, false
    );
}

function CheckPriceDirectSales() {
    var DirectSales = Xrm.Page.getAttribute("tss_sourcetype").getValue();

    Xrm.Page.data.entity.attributes.get("tss_price").setRequiredLevel("required");

    alert("Cannot save the record, price still not setup for this part number");
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FIELD EVENT AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function interchange_onChange() {
    //Check Part Master is part interchange
    var directsales = 'Direct Sales Price List';
    var isinterchange = Xrm.Page.getAttribute('tss_isinterchange').getValue();
    if (isinterchange) {

        // set ke tampungan
        discountAmountTemp = Xrm.Page.getAttribute("tss_discountamount").getValue();
        discountPercentTemp = Xrm.Page.getAttribute("tss_discountpercent").getValue();

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
            var priceType = Xrm.Page.getAttribute('tss_pricetype').getValue()[0].id;
            //Check on Sparepart Price & Master Part
            XrmServiceToolkit.Rest.RetrieveMultiple(
                "tss_sparepartpricemasterSet",
                "$select=*&$filter=tss_PartMaster/Id eq (guid'" + interchange + "') and tss_PriceListPart/Id eq (guid'" + priceType + "')",
                function (result) {
                    if (result.length > 0) {
                        if (result[0].tss_Price.Value != null) {
                            Xrm.Page.getAttribute("tss_price").setValue(parseInt(result[0].tss_Price.Value));
                        } else {
                            alert('This Part does not contain price.');
                            Xrm.Page.getAttribute("tss_price").setValue(parseInt(0));
                        }
                        if (result[0].tss_MinimumPrice.Value != null) {
                            Xrm.Page.getAttribute("tss_minimumprice").setValue(parseInt(result[0].tss_MinimumPrice.Value));
                            Xrm.Page.getAttribute("tss_minimumprice").setSubmitMode("always");
                        } else {
                            alert('This Part does not contain minimum price.');
                            Xrm.Page.getAttribute("tss_minimumprice").setValue(parseInt(0));
                            Xrm.Page.getAttribute("tss_minimumprice").setSubmitMode("always");
                        }
                        //Always Direct Sales
                        if (result[0].tss_PriceListPart.Id != null && result[0].tss_PriceListPart.Id != undefined) {
                            var priceList = new Array();
                            priceList[0] = new Object();
                            priceList[0].id = result[0].tss_PriceListPart.Id;
                            priceList[0].entityType = 'tss_PriceListPart'; //<-- Entity Type Price List Part
                            priceList[0].name = result[0].tss_PriceListPart.Name;
                            Xrm.Page.getAttribute("tss_pricetype").setValue(priceList);
                            //Xrm.Page.getAttribute("tss_sourcetype").setValue(parseInt(source_directsales));
                        } else {
                            alert('This Part does not contain Price List Part.');
                            Xrm.Page.getAttribute("tss_pricetype").setValue(null);
                        }
                    }
                    else {
                        alert("Cannot save the record, price still not setup for this part number");
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
                if (re.tss_parttype != null && re.tss_unitgroup != null) {
                    Xrm.Page.getAttribute("tss_parttype").setValue(re.tss_parttype.Value);
                    Xrm.Page.getAttribute("tss_partdescription").setValue();
                    Xrm.Page.getAttribute("tss_partdescription").setValue(re.trs_PartDescription);
                    var setUnit = new Array();
                    setUnit[0] = new Object();
                    setUnit[0].id = re.tss_unitgroup.Id;
                    setUnit[0].entityType = 'uomschedule'; //<-- Entity Type Unit Group
                    setUnit[0].name = re.tss_unitgroup.Name;
                    Xrm.Page.getAttribute("tss_unitgroup").setValue(setUnit);

                    Xrm.Page.getAttribute("tss_partdescription").setSubmitMode("always");
                    Xrm.Page.getAttribute("tss_unitgroup").setSubmitMode("always");
                    Xrm.Page.getAttribute("tss_parttype").setSubmitMode("always");
                } else { alert('Part Type / Unit Group is empty.'); }
            } else {
                alert('Warning: Request Part Number Interchange Failed !');
            }

            Xrm.Page.getAttribute("tss_discountamount").setValue(parseInt(0));
            Xrm.Page.getAttribute("tss_discountpercent").setValue(parseInt(0));
            discPercent_onChange();
            Xrm.Page.getAttribute('tss_price').setSubmitMode("always");
        } else {
            Xrm.Page.getAttribute("tss_price").setValue(parseInt(0));
            Xrm.Page.getAttribute("tss_minimumprice").setValue(parseInt(0));
            Xrm.Page.getAttribute("tss_minimumprice").setSubmitMode("always");
            Xrm.Page.getAttribute("tss_partdescription").setValue();
            Xrm.Page.getAttribute("tss_unitgroup").setValue();
            Xrm.Page.getAttribute("tss_parttype").setValue();
            Xrm.Page.getAttribute("tss_pricetype").setValue();
            partnumber_onChange();
        }

    }
}


//tampungan 
var discountAmountTemp = 0;
var discountPercentTemp = 0;

function isinterchange_onChange() {
    var isinterchange = Xrm.Page.getAttribute('tss_isinterchange');
    if (!isinterchange.getValue()) { // NO
        partnumber_onChange();
        Xrm.Page.getAttribute('tss_partnumberinterchange').setValue();
    } else {                        // YES
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

    //var qty = Xrm.Page.getAttribute('tss_quantity').getValue();
    if (discount < 100) {
        if (qty == null) {
            qty = Xrm.Page.getAttribute('tss_quantity').getValue();
            formulaAmount = Math.round(price * (discount / 100));
            //var formulaAmount = (price*(discount/100));
            //alert('Discount Amount:Rp ' + formulaAmount);
            formulaPercent = (formulaAmount / price) * 100;
            if (qty > 0) {
                priceamount = ((price - formulaAmount) * qty);
                priceafterdiscount = (price - formulaAmount);
                if (formulaAmount != 0) {
                    Xrm.Page.getAttribute('tss_finalprice').setValue(parseInt(priceafterdiscount));
                } else { Xrm.Page.getAttribute('tss_finalprice').setValue(parseInt(0)); }
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
                Xrm.Page.getAttribute('tss_priceamount').setValue(parseInt(priceamount));
            } else {
                Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));
                Xrm.Page.getAttribute('tss_discountpercent').setValue(0);
                Xrm.Page.getAttribute('tss_priceamount').setValue(parseInt(0));
                //alert('Please fill the quantity on Prospect Part Lines !');
            }
        } else {
            if (qty > 0) {
                var price = Xrm.Page.getAttribute('tss_price').getValue();
                if (discount == 0) {
                    //formulaAmount = 0;
                    //formulaPercent = 0;
                    //priceafterdiscount = (price - formulaAmount);
                    priceamount = (price * qty);
                    //Xrm.Page.getAttribute('tss_finalprice').setValue(parseInt(priceafterdiscount));
                    Xrm.Page.getAttribute('tss_priceamount').setValue(parseInt(priceamount));
                    Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));
                    Xrm.Page.getAttribute('tss_discountpercent').setValue(parseInt(0));
                }
                    //            var finalprice = Xrm.Page.getAttribute('tss_finalprice').getValue();
                    //            var priceamount = (finalprice * qty);
                    //            Xrm.Page.getAttribute('tss_priceamount').setValue(parseInt(priceamount));

                    //////TAMBAHAN AMIN//////////////////
                else {
                    qty = Xrm.Page.getAttribute('tss_quantity').getValue();
                    formulaAmount = Math.round(price * (discount / 100));
                    //var formulaAmount = (price*(discount/100));
                    //alert('Discount Amount:Rp ' + formulaAmount);
                    formulaPercent = (formulaAmount / price) * 100;
                    if (qty > 0) {
                        priceamount = ((price - formulaAmount) * qty);
                        priceafterdiscount = (price - formulaAmount);
                        if (formulaAmount != 0) {
                            Xrm.Page.getAttribute('tss_finalprice').setValue(parseInt(priceafterdiscount));
                        } else { Xrm.Page.getAttribute('tss_finalprice').setValue(parseInt(0)); }
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
                        Xrm.Page.getAttribute('tss_priceamount').setValue(parseInt(priceamount));
                    } else {
                        Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));
                        Xrm.Page.getAttribute('tss_discountpercent').setValue(0);
                        Xrm.Page.getAttribute('tss_priceamount').setValue(parseInt(0));
                        //alert('Please fill the quantity on Prospect Part Lines !');
                    }
                }
                /////////////////////////////////////

            } else {
                Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));
                Xrm.Page.getAttribute('tss_discountpercent').setValue(0);
                Xrm.Page.getAttribute('tss_priceamount').setValue(parseInt(0));
                Xrm.Page.getAttribute('tss_finalprice').setValue(parseInt(0));
                //alert('Please fill the quantity on Prospect Part Lines !');
            }
        }


    } else {
        alert('Discount percent cannot be greater equal than 100 %.');
        Xrm.Page.getAttribute('tss_discountpercent').setValue(0);
    }
    Xrm.Page.getAttribute('tss_finalprice').setSubmitMode("always");
    Xrm.Page.getAttribute('tss_underminimumprice').setSubmitMode("always");
    Xrm.Page.getAttribute('tss_discountpercent').setSubmitMode("always");
    Xrm.Page.getAttribute('tss_discountamount').setSubmitMode("always");
    Xrm.Page.getAttribute('tss_priceamount').setSubmitMode("always");
}

function discAmount_onChange(quantity) {
    var qty;
    var minprice = Xrm.Page.getAttribute('tss_minimumprice').getValue();
    var discount = Xrm.Page.getAttribute('tss_discountamount').getValue();
    var price = Xrm.Page.getAttribute('tss_price').getValue();
    var formulaPercent = (discount / price) * 100;
    var priceamount;
    if (discount < price) {
        if (quantity == null) {
            qty = Xrm.Page.getAttribute('tss_quantity').getValue();
            //Change Formula Amount: Price*(Discount Percent/100)
            var formulaAmount = Math.round(price * (formulaPercent / 100));
            if (qty > 0) {
                var priceamount = ((price - formulaAmount) * qty);
                var priceafterdiscount = (price - formulaAmount);
                if (formulaAmount != 0) {
                    Xrm.Page.getAttribute('tss_finalprice').setValue(parseInt(priceafterdiscount));
                } else { Xrm.Page.getAttribute('tss_finalprice').setValue(parseInt(0)); }

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
                Xrm.Page.getAttribute('tss_priceamount').setValue(parseInt(priceamount));
            } else {
                Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));
                Xrm.Page.getAttribute('tss_discountpercent').setValue(parseInt(0));
                Xrm.Page.getAttribute('tss_priceamount').setValue(parseInt(0));
                //alert('Please fill the quantity on Prospect Part Lines !');
            }
        } else {
            qty = quantity;
            if (qty > 0) {
                //var price = Xrm.Page.getAttribute('tss_price').getValue();
                if (discount == 0) {
                    //formulaAmount = 0;
                    //formulaPercent = 0;
                    //priceafterdiscount = (price - formulaAmount);
                    priceamount = (price * qty);
                    //Xrm.Page.getAttribute('tss_finalprice').setValue(parseInt(priceafterdiscount));
                    Xrm.Page.getAttribute('tss_priceamount').setValue(parseInt(priceamount));
                    Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));
                    Xrm.Page.getAttribute('tss_discountpercent').setValue(parseInt(0));
                }
                    //var finalprice = Xrm.Page.getAttribute('tss_finalprice').getValue();
                    //var priceamount = (finalprice * qty);
                    //Xrm.Page.getAttribute('tss_priceamount').setValue(parseInt(priceamount));

                    //////TAMBAHAN AMIN//////////////////
                else {
                    qty = Xrm.Page.getAttribute('tss_quantity').getValue();
                    //Change Formula Amount: Price*(Discount Percent/100)
                    var formulaAmount = Math.round(price * (formulaPercent / 100));
                    if (qty > 0) {
                        var priceamount = ((price - formulaAmount) * qty);
                        var priceafterdiscount = (price - formulaAmount);
                        if (formulaAmount != 0) {
                            Xrm.Page.getAttribute('tss_finalprice').setValue(parseInt(priceafterdiscount));
                        } else { Xrm.Page.getAttribute('tss_finalprice').setValue(parseInt(0)); }

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
                        Xrm.Page.getAttribute('tss_priceamount').setValue(parseInt(priceamount));
                    } else {
                        Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));
                        Xrm.Page.getAttribute('tss_discountpercent').setValue(parseInt(0));
                        Xrm.Page.getAttribute('tss_priceamount').setValue(parseInt(0));
                        //alert('Please fill the quantity on Prospect Part Lines !');
                    }
                }
                //////////////////////////////
            } else {
                Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));
                Xrm.Page.getAttribute('tss_discountpercent').setValue(0);
                Xrm.Page.getAttribute('tss_priceamount').setValue(parseInt(0));
                Xrm.Page.getAttribute('tss_finalprice').setValue(parseInt(0));
                Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));
                //alert('Please fill the quantity on Prospect Part Lines !');
            }
        }


    } else {
        alert('Discount amount cannot be greater equal than price part.');
        Xrm.Page.getAttribute('tss_discountamount').setValue(0);
    }
    Xrm.Page.getAttribute('tss_finalprice').setSubmitMode("always");
    Xrm.Page.getAttribute('tss_underminimumprice').setSubmitMode("always");
    Xrm.Page.getAttribute('tss_discountamount').setSubmitMode("always");
    Xrm.Page.getAttribute('tss_discountpercent').setSubmitMode("always");
    Xrm.Page.getAttribute('tss_priceamount').setSubmitMode("always");
}

function qty_onChange() {
    var qty = Xrm.Page.getAttribute('tss_quantity').getValue();
    //var discP = Xrm.Page.getAttribute('tss_discountpercent').getValue();
    //var discA = Xrm.Page.getAttribute('tss_discountamount').getValue();

    /*if(qty != null){
        discAmount_onChange(qty);
        //discPercent_onChange(qty);
        Xrm.Page.getAttribute('tss_priceamount').setSubmitMode("always");
    }*/


    discAmount_onChange(qty);
    discPercent_onChange(qty);
    Xrm.Page.getAttribute('tss_priceamount').setSubmitMode("always");
}

function partnumber_onChange()
{
    var isExist = IsExistInMarketSize();

    if (isExist != null)
        GetSparePartPriceMarketSize(isExist);

    GetSparePartPriceMaster();
}

function IsExistInMarketSize()
{
    debugger;
    var oResult;
    var oPartNumber = Xrm.Page.getAttribute('tss_partnumber').getValue();

    if (oPartNumber != null)
    {
        oPartNumber = oPartNumber[0].id;

        var oPartHeaderId = Xrm.Page.data.entity.attributes.get("tss_prospectpartheader").getValue()[0].id;
        var oCustomer;
        var oPSS;
        var oEstimationDate;

        XrmServiceToolkit.Rest.Retrieve(oPartHeaderId, "tss_prospectpartheaderSet", "tss_Estimationclosedate,tss_Customer,tss_PSS,tss_prospectpartheaderId", null, function (result) {
            oCustomer = result.tss_Customer.Id;
            oPSS = result.tss_PSS.Id;
            var estimationdate = new Date(result.tss_Estimationclosedate);

            oEstimationDate =
                estimationdate.getFullYear() + "-" +
                ("00" + (estimationdate.getMonth() + 1)).slice(-2) + "-" +
                ("00" + estimationdate.getDate()).slice(-2) + "T" +
                ("00" + estimationdate.getHours()).slice(-2) + ":" +
                ("00" + estimationdate.getMinutes()).slice(-2) + ":" +
                ("00" + estimationdate.getSeconds()).slice(-2) + ".000Z";

        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, false);

        XrmServiceToolkit.Rest.RetrieveMultiple("tss_totalpartconsumpmarketsizeSet", "?$select=tss_name,tss_totalpartconsumpmarketsizeId,tss_RemainingQty&$filter=tss_PartNumber/Id eq (guid'" + oPartNumber + "') and tss_Customer/Id eq (guid'" + oCustomer + "') and tss_PSS/Id eq (guid'" + oPSS + "') and  tss_ActiveStartDate le datetime'" + oEstimationDate + "' and  tss_ActiveEndDate ge datetime'" + oEstimationDate + "'", function (results) {
            //for (var i = 0; i < results.length; i++) {
            //    var tss_name = results[i].tss_name;
            //    var tss_totalpartconsumpmarketsizeId = results[i].tss_totalpartconsumpmarketsizeId;
            //}

            if (results.length > 0)
                oResult = results[0];
            else
                oResult = null;
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, function () {
            //On Complete - Do Something
        }, false);
    }
    else
    {
        oResult = null;
    }

    return oResult;
}

function GetSparePartPriceMarketSize(totalpartconsump)
{
    var oLinesId = Xrm.Page.data.entity.getId();
    var oQty = Xrm.Page.getAttribute('tss_quantity').getValue();
    var oQtyRemaining = totalpartconsump.tss_RemainingQty;

    if (oQty > oQtyRemaining && oQtyRemaining == 0) {
        Xrm.Page.getAttribute("tss_refmarketsize").setValue(null);
        Xrm.Page.getAttribute("tss_refmarketsizebypmperiod").setValue(null);

        Xrm.Page.getAttribute("tss_qtymarketsize").setValue(null);
        Xrm.Page.getAttribute("tss_qtymarketsize").setSubmitMode("always");

        Xrm.Page.ui.controls.get("tss_sourcetype").setVisible(false)
        Xrm.Page.getAttribute("tss_sourcetype").setValue(null); //change to market size
        Xrm.Page.getAttribute("tss_sourcetype").setSubmitMode("always");
    }
    else if (oQty > oQtyRemaining && oQtyRemaining != 0) {
        Xrm.Page.ui.controls.get("tss_sourcetype").setVisible(true)
        Xrm.Page.getAttribute("tss_sourcetype").setValue(865920001); //change to market size
        Xrm.Page.getAttribute("tss_sourcetype").setSubmitMode("always");

        //XrmServiceToolkit.Rest.RetrieveMultiple("tss_pricelistpartSet", "?$select=tss_pricelistpartId&$filter=tss_Code eq 'P2'", function (results) {
        //    if (results.length > 0) {
        //        Xrm.Page.getAttribute("tss_pricetype").setValue([{ id: results[0].tss_pricelistpartId.Id, name: results[0].tss_pricelistpartId.Name, entityType: results[0].tss_pricelistpartId.LogicalName }]);
        //        Xrm.Page.getAttribute("tss_pricetype").setSubmitMode("always");
        //    }
        //}, function (error) {
        //    Xrm.Utility.alertDialog(error.message);
        //}, function () {
        //    //On Complete - Do Something
        //}, false);

        XrmServiceToolkit.Rest.RetrieveMultiple("tss_pricelistpartSet", "?$select=tss_pricelistpartId&$filter=tss_Type/Value eq 865920001", function (results) {
            if (results.length > 0) {
                Xrm.Page.getAttribute("tss_pricetype").setValue([{ id: results[0].tss_pricelistpartId.Id, name: results[0].tss_pricelistpartId.Name, entityType: results[0].tss_pricelistpartId.LogicalName }]);
                Xrm.Page.getAttribute("tss_pricetype").setSubmitMode("always");
            }
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, function () {
            //On Complete - Do Something
        }, false);

        var qtymarketsize = oQty - oQtyRemaining;

        Xrm.Page.getAttribute("tss_qtymarketsize").setValue(qtymarketsize);
        Xrm.Page.getAttribute("tss_qtymarketsize").setSubmitMode("always");

        var oQtyRemaining = oQtyRemaining - oQty;
        var entity = {};
        entity.tss_RemainingQty = oQtyRemaining;

        XrmServiceToolkit.Rest.Update(totalpartconsump.tss_totalpartconsumpmarketsizeId, entity, "tss_totalpartconsumpmarketsizeSet", function () {
            //Success - No Return Data - Do Something
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, false);
    }
    else if (oQty <= oQtyRemaining)
    {
        Xrm.Page.ui.controls.get("tss_sourcetype").setVisible(true)
        Xrm.Page.getAttribute("tss_sourcetype").setValue(865920001); //change to market size
        Xrm.Page.getAttribute("tss_sourcetype").setSubmitMode("always");

        //XrmServiceToolkit.Rest.RetrieveMultiple("tss_pricelistpartSet", "?$select=tss_pricelistpartId&$filter=tss_Code eq 'P2'", function (results) {
        //    if (results.length > 0) {
        //        Xrm.Page.getAttribute("tss_pricetype").setValue([{ id: results[0].tss_pricelistpartId.Id, name: results[0].tss_pricelistpartId.Name, entityType: results[0].tss_pricelistpartId.LogicalName }]);
        //        Xrm.Page.getAttribute("tss_pricetype").setSubmitMode("always");
        //    }
        //}, function (error) {
        //    Xrm.Utility.alertDialog(error.message);
        //}, function () {
        //    //On Complete - Do Something
        //}, false);

        XrmServiceToolkit.Rest.RetrieveMultiple("tss_pricelistpartSet", "?$select=tss_pricelistpartId&$filter=tss_Type/Value eq 865920001", function (results) {
            if (results.length > 0) {
                Xrm.Page.getAttribute("tss_pricetype").setValue([{ id: results[0].tss_pricelistpartId.Id, name: results[0].tss_pricelistpartId.Name, entityType: results[0].tss_pricelistpartId.LogicalName }]);
                Xrm.Page.getAttribute("tss_pricetype").setSubmitMode("always");
            }
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, function () {
            //On Complete - Do Something
        }, false);
        
        Xrm.Page.getAttribute("tss_qtymarketsize").setValue(oQty);
        Xrm.Page.getAttribute("tss_qtymarketsize").setSubmitMode("always");

        var oQtyRemaining = oQtyRemaining - oQty;
        var entity = {};
        entity.tss_RemainingQty = oQtyRemaining;

        XrmServiceToolkit.Rest.Update(totalpartconsump.tss_totalpartconsumpmarketsizeId, entity, "tss_totalpartconsumpmarketsizeSet", function () {
            //Success - No Return Data - Do Something
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, false);
    }
}

function GetSparePartPriceMaster()
{
    try {
        //Get the partnumber lookup off of the record
        var pListPart;
        var pricelistID;
        var partnumber = Xrm.Page.getAttribute('tss_partnumber').getValue();
        var sourcetype = Xrm.Page.getAttribute('tss_sourcetype').getValue();
        //if partnumber exist, attempt to pull back the entire partnumber record
        if (partnumber != null && partnumber[0].entityType == "trs_masterpart")
        {
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
                        var price = result[0].tss_Price.Value;
                        var minprice = result[0].tss_MinimumPrice.Value;
                        if (price != null) {
                            Xrm.Page.getAttribute("tss_price").setValue(parseInt(result[0].tss_Price.Value));
                        } else {
                            alert('This Part does not contain price.');
                            Xrm.Page.getAttribute("tss_price").setValue(parseInt(0));
                        }

                        if (minprice != null) {
                            Xrm.Page.getAttribute("tss_minimumprice").setValue(parseInt(result[0].tss_MinimumPrice.Value));
                            Xrm.Page.getAttribute("tss_minimumprice").setSubmitMode("always");
                        } else {
                            alert('This Part does not contain minimum price.');
                            Xrm.Page.getAttribute("tss_minimumprice").setValue(parseInt(0));
                            Xrm.Page.getAttribute("tss_minimumprice").setSubmitMode("always");
                        }

                        //alert(result[0].tss_PriceListPart.Id);
                        var discount = Xrm.Page.getAttribute("tss_discountamount").getValue();
                        var qty = Xrm.Page.getAttribute("tss_quantity").getValue();

                        price = ((price - discount) * qty);
                        Xrm.Page.getAttribute("tss_priceamount").setValue(parseInt(price));


                        //Always Direct Sales
                        if (result[0].tss_PriceListPart.Id != null && result[0].tss_PriceListPart.Id != undefined) {
                            var priceList = new Array();
                            priceList[0] = new Object();
                            priceList[0].id = result[0].tss_PriceListPart.Id;
                            priceList[0].entityType = 'tss_PriceListPart'; //<-- Entity Type Price List Part
                            priceList[0].name = result[0].tss_PriceListPart.Name;
                            Xrm.Page.getAttribute("tss_pricetype").setValue(priceList);
                            //Xrm.Page.getAttribute("tss_sourcetype").setValue(parseInt(source_directsales));
                        } else {
                            alert('This Part does not contain Price List Part.');
                            Xrm.Page.getAttribute("tss_pricetype").setValue(null);
                        }

                    }
                    else {
                        alert('Empty retrieve Sparepart Price.');
                        Xrm.Page.getAttribute("tss_price").setValue(parseInt(0));
                        Xrm.Page.getAttribute("tss_minimumprice").setValue(parseInt(0));
                        Xrm.Page.getAttribute("tss_minimumprice").setSubmitMode("always");
                        Xrm.Page.getAttribute("tss_partdescription").setValue();
                        Xrm.Page.getAttribute("tss_parttype").setValue();
                        Xrm.Page.getAttribute("tss_unitgroup").setValue();
                        Xrm.Page.getAttribute("tss_pricelist").setValue();
                        Xrm.Page.getAttribute("tss_pricetype").setValue();
                        Xrm.Page.getAttribute('tss_priceamount').setValue(parseInt(0));
                        Xrm.Page.getAttribute('tss_finalprice').setValue(parseInt(0));
                        Xrm.Page.getAttribute('tss_discountpercent').setValue(parseInt(0));
                        Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));

                        Xrm.Page.ui.controls.get("tss_sourcetype").setVisible(false)
                        Xrm.Page.getAttribute("tss_sourcetype").setValue(null); //change to market size
                        Xrm.Page.getAttribute("tss_sourcetype").setSubmitMode("always");
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
                if (re.tss_parttype != null && re.tss_unitgroup != null) {
                    Xrm.Page.getAttribute("tss_parttype").setValue(re.tss_parttype.Value);
                    Xrm.Page.getAttribute("tss_partdescription").setValue(re.trs_PartDescription);
                    var setUnit = new Array();
                    setUnit[0] = new Object();
                    setUnit[0].id = re.tss_unitgroup.Id;
                    setUnit[0].entityType = 'uomschedule'; //<-- Entity Type Unit Group
                    setUnit[0].name = re.tss_unitgroup.Name;
                    Xrm.Page.getAttribute("tss_unitgroup").setValue(setUnit);
                } else { alert('Part Type / Unit Group is empty.'); }
            } else {
                alert('Warning: Request Part Number On Change Failed !');
            }


            if (Xrm.Page.getAttribute('tss_isinterchange').getValue() == true ) Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));
            if (Xrm.Page.getAttribute('tss_isinterchange').getValue() == true) Xrm.Page.getAttribute('tss_discountpercent').setValue(parseInt(0));

            if (Xrm.Page.getAttribute('tss_isinterchange').getValue() == false) Xrm.Page.getAttribute('tss_discountamount').setValue(discountAmountTemp);
            if (Xrm.Page.getAttribute('tss_isinterchange').getValue() == false) Xrm.Page.getAttribute('tss_discountpercent').setValue(discountPercentTemp);


            discPercent_onChange();
            Xrm.Page.getAttribute('tss_price').setSubmitMode("always");
        } else {
            console.log('Part Number is empty.');
            Xrm.Page.getAttribute('tss_partnumber').setValue("");
            Xrm.Page.getAttribute("tss_price").setValue(parseInt(0));
            Xrm.Page.getAttribute("tss_minimumprice").setValue(parseInt(0));
            Xrm.Page.getAttribute("tss_minimumprice").setSubmitMode("always");
            Xrm.Page.getAttribute("tss_partdescription").setValue();
            Xrm.Page.getAttribute("tss_parttype").setValue();
            Xrm.Page.getAttribute("tss_unitgroup").setValue();
            Xrm.Page.getAttribute("tss_pricelist").setValue();
            Xrm.Page.getAttribute("tss_pricetype").setValue();
            //Xrm.Page.getAttribute("tss_sourcetype").setValue();
            Xrm.Page.getAttribute('tss_priceamount').setValue(parseInt(0));
            Xrm.Page.getAttribute('tss_finalprice').setValue(parseInt(0));
            Xrm.Page.getAttribute('tss_discountpercent').setValue(parseInt(0));
            Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));

            Xrm.Page.ui.controls.get("tss_sourcetype").setVisible(false)
            Xrm.Page.getAttribute("tss_sourcetype").setValue(null); //change to market size
            Xrm.Page.getAttribute("tss_sourcetype").setSubmitMode("always");
        }

        Xrm.Page.getAttribute('tss_partnumber').setSubmitMode("always");
    } catch (e) {
        alert('Failed to set on change Part Number : ' + e.message)
    }
}

//function partnumber_onChange() {
//    try {
//        //Get the partnumber lookup off of the record
//        var pListPart;
//        var pricelistID;
//        var partnumber = Xrm.Page.getAttribute('tss_partnumber').getValue();
//        var sourcetype = Xrm.Page.getAttribute('tss_sourcetype').getValue();
//        //if partnumber exist, attempt to pull back the entire partnumber record
//        if (partnumber != null && partnumber[0].entityType == "trs_masterpart")
//        {
//            var partnumberId = partnumber[0].id;
//            var serverUrl;

//            if (Xrm.Page.context.getClientUrl !== undefined) {
//                serverUrl = Xrm.Page.context.getClientUrl();
//            } else {
//                serverUrl = Xrm.Page.context.getServerUrl();
//            }

//            //Check Part Number on Sparepart Price with Source Type is
//            var temp;
//            if (sourcetype == source_directsales) {
//                pListPart = 'P1';
//            } else if (sourcetype == source_marketsize) {
//                pListPart = 'P2';
//            } else if (sourcetype == source_contract) {
//                pListPart = 'P3';
//            } else {
//                pListPart = 'P4';
//            }

//            XrmServiceToolkit.Rest.RetrieveMultiple(
//                "tss_pricelistpartSet",
//                "$select=*&$filter=tss_Code eq '" + pListPart + "'",
//                function (result) {
//                    if (result.length > 0) {
//                        pricelistID = result[0].tss_pricelistpartId;
//                    }
//                }, function (error) {
//                    alert('Retrieve Price List Part: ' + error.message);
//                }, function onComplete() {
//                    //alert("DONE.");
//                }, false
//            );
//            XrmServiceToolkit.Rest.RetrieveMultiple(
//                "tss_sparepartpricemasterSet",
//                "$select=*&$filter=tss_PartMaster/Id eq (guid'" + partnumberId + "') and tss_PriceListPart/Id eq (guid'" + pricelistID + "')",
//                function (result) {
//                    if (result.length > 0) {
//                        //Validasi jika dari MS / DS / CMP / Contract

//                        //Checking Sparepart with SourceType to get Sparepart ID / Price / Price List
//                        //alert('ID: ' + result[0].tss_sparepartpricemasterId);
//                        var price = result[0].tss_Price.Value;
//                        var minprice = result[0].tss_MinimumPrice.Value;
//                        if (price != null) {
//                            Xrm.Page.getAttribute("tss_price").setValue(parseInt(result[0].tss_Price.Value));
//                        } else {
//                            alert('This Part does not contain price.');
//                            Xrm.Page.getAttribute("tss_price").setValue(parseInt(0));
//                        }

//                        if (minprice != null) {
//                            Xrm.Page.getAttribute("tss_minimumprice").setValue(parseInt(result[0].tss_MinimumPrice.Value));
//                            Xrm.Page.getAttribute("tss_minimumprice").setSubmitMode("always");
//                        } else {
//                            alert('This Part does not contain minimum price.');
//                            Xrm.Page.getAttribute("tss_minimumprice").setValue(parseInt(0));
//                            Xrm.Page.getAttribute("tss_minimumprice").setSubmitMode("always");
//                        }

//                        //alert(result[0].tss_PriceListPart.Id);
//                        var discount = Xrm.Page.getAttribute("tss_discountamount").getValue();
//                        var qty = Xrm.Page.getAttribute("tss_quantity").getValue();

//                        price = ((price - discount) * qty);
//                        Xrm.Page.getAttribute("tss_priceamount").setValue(parseInt(price));


//                        //Always Direct Sales
//                        if (result[0].tss_PriceListPart.Id != null && result[0].tss_PriceListPart.Id != undefined) {
//                            var priceList = new Array();
//                            priceList[0] = new Object();
//                            priceList[0].id = result[0].tss_PriceListPart.Id;
//                            priceList[0].entityType = 'tss_PriceListPart'; //<-- Entity Type Price List Part
//                            priceList[0].name = result[0].tss_PriceListPart.Name;
//                            Xrm.Page.getAttribute("tss_pricetype").setValue(priceList);
//                            //Xrm.Page.getAttribute("tss_sourcetype").setValue(parseInt(source_directsales));
//                        } else {
//                            alert('This Part does not contain Price List Part.');
//                            Xrm.Page.getAttribute("tss_pricetype").setValue(null);
//                        }

//                    }
//                    else {
//                        alert('Empty retrieve Sparepart Price.');
//                        Xrm.Page.getAttribute("tss_price").setValue(parseInt(0));
//                        Xrm.Page.getAttribute("tss_minimumprice").setValue(parseInt(0));
//                        Xrm.Page.getAttribute("tss_minimumprice").setSubmitMode("always");
//                        Xrm.Page.getAttribute("tss_partdescription").setValue();
//                        Xrm.Page.getAttribute("tss_parttype").setValue();
//                        Xrm.Page.getAttribute("tss_unitgroup").setValue();
//                        Xrm.Page.getAttribute("tss_pricelist").setValue();
//                        Xrm.Page.getAttribute("tss_pricetype").setValue();
//                        Xrm.Page.getAttribute('tss_priceamount').setValue(parseInt(0));
//                        Xrm.Page.getAttribute('tss_finalprice').setValue(parseInt(0));
//                        Xrm.Page.getAttribute('tss_discountpercent').setValue(parseInt(0));
//                        Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));
//                    }

//                }, function (error) {
//                    alert('Retrieve Sparepart Price: ' + error.message);
//                }, function onComplete() {
//                    //alert("DONE.");
//                }, false
//            );
//            var ODataPath = serverUrl + "/XRMServices/2011/OrganizationData.svc";
//            var partnumberRequest = new XMLHttpRequest();
//            partnumberRequest.open("GET", ODataPath + "/trs_masterpartSet(guid'" + partnumberId + "')", false);
//            partnumberRequest.setRequestHeader("Accept", "application/json");
//            partnumberRequest.setRequestHeader("Content-Type", "application/json; charset=utf-8");
//            partnumberRequest.send();

//            //If request was successful, parse the email address
//            if (partnumberRequest.status == 200) {
//                //Retrieve Value from tss_prospectpartlines
//                var re = JSON.parse(partnumberRequest.responseText).d;

//                //GET part type & unit group from Spare Part Price Master
//                if (re.tss_parttype != null && re.tss_unitgroup != null) {
//                    Xrm.Page.getAttribute("tss_parttype").setValue(re.tss_parttype.Value);
//                    Xrm.Page.getAttribute("tss_partdescription").setValue(re.trs_PartDescription);
//                    var setUnit = new Array();
//                    setUnit[0] = new Object();
//                    setUnit[0].id = re.tss_unitgroup.Id;
//                    setUnit[0].entityType = 'uomschedule'; //<-- Entity Type Unit Group
//                    setUnit[0].name = re.tss_unitgroup.Name;
//                    Xrm.Page.getAttribute("tss_unitgroup").setValue(setUnit);
//                } else { alert('Part Type / Unit Group is empty.'); }
//            } else {
//                alert('Warning: Request Part Number On Change Failed !');
//            }

          
//            if (Xrm.Page.getAttribute('tss_isinterchange').getValue() == true ) Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));
//            if (Xrm.Page.getAttribute('tss_isinterchange').getValue() == true) Xrm.Page.getAttribute('tss_discountpercent').setValue(parseInt(0));

//            if (Xrm.Page.getAttribute('tss_isinterchange').getValue() == false) Xrm.Page.getAttribute('tss_discountamount').setValue(discountAmountTemp);
//            if (Xrm.Page.getAttribute('tss_isinterchange').getValue() == false) Xrm.Page.getAttribute('tss_discountpercent').setValue(discountPercentTemp);

           
//            discPercent_onChange();
//            Xrm.Page.getAttribute('tss_price').setSubmitMode("always");
//        } else {
//            console.log('Part Number is empty.');
//            Xrm.Page.getAttribute('tss_partnumber').setValue("");
//            Xrm.Page.getAttribute("tss_price").setValue(parseInt(0));
//            Xrm.Page.getAttribute("tss_minimumprice").setValue(parseInt(0));
//            Xrm.Page.getAttribute("tss_minimumprice").setSubmitMode("always");
//            Xrm.Page.getAttribute("tss_partdescription").setValue();
//            Xrm.Page.getAttribute("tss_parttype").setValue();
//            Xrm.Page.getAttribute("tss_unitgroup").setValue();
//            Xrm.Page.getAttribute("tss_pricelist").setValue();
//            Xrm.Page.getAttribute("tss_pricetype").setValue();
//            //Xrm.Page.getAttribute("tss_sourcetype").setValue();
//            Xrm.Page.getAttribute('tss_priceamount').setValue(parseInt(0));
//            Xrm.Page.getAttribute('tss_finalprice').setValue(parseInt(0));
//            Xrm.Page.getAttribute('tss_discountpercent').setValue(parseInt(0));
//            Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));
//        }

//        Xrm.Page.getAttribute('tss_partnumber').setSubmitMode("always");
//    } catch (e) {
//        alert('Failed to set on change Part Number : ' + e.message)
//    }
//    //tss_prospectpartlines

//}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FORM ON SAVE AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//event : on save form
/*Prevent save when price is null or 0 onSave Form event
referensi : http://bit.ly/2DscHvu
*/
function preventSave(econtext) {
    var formType = Xrm.Page.ui.getFormType();
    var eventArgs = econtext.getEventArgs();
    var PriceNull = Xrm.Page.getAttribute("tss_price").getValue();
    var part = Xrm.Page.getAttribute("tss_partnumber").getValue();
    var isinterchange = Xrm.Page.getAttribute("tss_isinterchange").getValue();
    var prosID = Xrm.Page.getAttribute("tss_prospectpartheader").getValue()[0].id;
    var id = Xrm.Page.data.entity.getId();
    var checkPart = false;
    if (formType < 2) {
        if (isinterchange) {
            var partinterchange = Xrm.Page.getAttribute("tss_partnumberinterchange").getValue();
            XrmServiceToolkit.Rest.RetrieveMultiple(
                "tss_prospectpartlinesSet",
                "$select=*&$filter=tss_ProspectPartHeader/Id eq (guid'" + prosID + "') and tss_PartNumberInterchange/Id eq (guid'" + partinterchange[0].id + "')",
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
                "tss_prospectpartlinesSet",
                "$select=*&$filter=tss_ProspectPartHeader/Id eq (guid'" + prosID + "') and tss_partnumber/Id eq (guid'" + part[0].id + "')",
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
            alert("Cannot save the record, part / part interchange already exist on this Prospect Part.");
        }
    }

    if (Xrm.Page.getAttribute("tss_unitgroup").getValue() != null) {
        if (Xrm.Page.getAttribute("tss_unitgroup").getValue()[0].id == null) {
            alert("Cannot save the record because Unit Group is empty.")
            eventArgs.preventDefault();
        }
    } else if (Xrm.Page.getAttribute("tss_unitgroup").getValue() == null) {
        alert("Cannot save the record because Unit Group is empty.")
        eventArgs.preventDefault();
    }
}

