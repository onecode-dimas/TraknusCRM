///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// Helper
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
var formType = Xrm.Page.ui.getFormType();
var userID = Xrm.Page.context.getUserId();
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

var isAuth = false;

function SetDefaultValue_TwoOptionField(fieldname) {
    var attr = Xrm.Page.getAttribute(fieldname);
    if (attr != null) {
        if (attr.getValue() == false) {
            attr.setValue(1);
            attr.setValue(0);
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

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FORM ON LOAD AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function Form_OnLoad() {
    var SOPartHeader = Xrm.Page.data.entity.attributes.get("tss_sopartheaderid").getValue();
    if (formType < 2) {
        SetDefaultValue_TwoOptionField("tss_discountby");
    }

    XrmServiceToolkit.Rest.Retrieve(
        userID,
        "SystemUserSet",
        null, null,
        function (result) {
            //if (result.Title == 'Counter Part' || result.Title == 'Dealer Part Officer') isAuth = true;
            if (result.Title == 'Counter Part') //20180829 - yang bisa input discount hanya counter
            {
                isAuth = true;
            }
            else
            {
                isAuth = false;

                Xrm.Page.getControl("tss_discountamount").setVisible(false);
                Xrm.Page.getControl("tss_discountpercent").setVisible(false);
                Xrm.Page.getControl("tss_discountby").setVisible(false);
                Xrm.Page.getControl("tss_priceafterdiscount").setVisible(false);

                Xrm.Page.getAttribute("tss_discountpercent").setRequiredLevel("none");
            }
        }, function (error) {
            alert("Failed retrieve user on setupForm");
            isAuth = false;
        }, false
    );

    EnableField();
    SetSourceType();

    if (formType == formStatus.Create) {
        if (SOPartHeader != null) {
            SetItemNumber(SOPartHeader);
        }
        else {
            alert('SO Part Lines must be created on SO Part !');
            Xrm.Page.ui.close();
        }
        
        //20180821
        //DiscountBy_onChange();
    }
    else if (formType == formStatus.Update) {
        //20180821
        if (Xrm.Page.getAttribute("tss_discountby").getValue() == 0) {
            Xrm.Page.getControl("tss_discountamount").setDisabled(true);
            Xrm.Page.getControl("tss_discountpercent").setDisabled(false);
        }
        else if (Xrm.Page.getAttribute("tss_discountby").getValue() == 1) {
            Xrm.Page.getControl("tss_discountpercent").setDisabled(true);
            Xrm.Page.getControl("tss_discountamount").setDisabled(false);
        }
    }

    if (isAuth == false) {
        Xrm.Page.getControl("tss_discountamount").setVisible(false);
        Xrm.Page.getControl("tss_discountpercent").setVisible(false);
        Xrm.Page.getControl("tss_discountby").setVisible(false);
        Xrm.Page.getControl("tss_priceafterdiscount").setVisible(false);
        
    }

    ShowTotalPartConsump();
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// PUBLICS AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function SetItemNumber(SOPartHeader) {
    var itemNumber = Xrm.Page.data.entity.attributes.get("tss_itemnumber").getValue();
    var defaultItemNumber = 10;
    var Name;
    var setIN;
    if (itemNumber == null) {
        XrmServiceToolkit.Rest.RetrieveMultiple(
            "tss_sopartlinesSet",
            "$select=*&$filter=tss_SOPartHeaderId/Id eq (guid'" + SOPartHeader[0].id + "')",
            function (result) {
                if (result.length > 0) {
                    var max = 10;
                    for (i = 0; i < result.length; i++) {
                        if (result[i].tss_itemnumber > max) max = result[i].tss_itemnumber;
                    }
                    setIN = max + defaultItemNumber;
                    Xrm.Page.getAttribute("tss_itemnumber").setValue(setIN);
                    Name = SOPartHeader[0].name + "-" + setIN;
                } else {
                    Xrm.Page.getAttribute("tss_itemnumber").setValue(defaultItemNumber);
                    Name = SOPartHeader[0].name + "-" + defaultItemNumber;
                }
            }, function error(error) {
                alert("Failed to retrieve SO Part Lines record.");
            }, function onComplete() {
                //return;
            }, false
        );

        Xrm.Page.getAttribute("tss_name").setValue(Name);
    }
}

function EnableField() {
    SetPartNUmberInterchange();
}

function setDisabledField(field, value) {
    Xrm.Page.getControl(field).setDisabled(value);
}

function SetSourceType() {
    setDisabledField("tss_partnumber", true);
    setDisabledField("tss_isinterchange", true);
    setDisabledField("tss_partnumberinterchange", true);
    setDisabledField("tss_qtyrequest", true);
    if (Xrm.Page.getAttribute("tss_sopartheaderid").getValue() != null) {
        var headerid = Xrm.Page.getAttribute("tss_sopartheaderid").getValue()[0].id;
        XrmServiceToolkit.Rest.Retrieve(headerid, "tss_sopartheaderSet", "tss_SourceType,tss_StateCode", null, function (result) {
            if (result.tss_SourceType != null) {
                var tss_SourceType = result.tss_SourceType;
                if (tss_SourceType.Value == 865920003) {
                    Xrm.Page.getAttribute("tss_sourcetype").setValue(865920004);
                    Xrm.Page.getAttribute("tss_sourcetype").setSubmitMode("always");
                }
                if (tss_SourceType.Value == 865920004) {
                    Xrm.Page.getAttribute("tss_sourcetype").setValue(865920006);
                    Xrm.Page.getAttribute("tss_sourcetype").setSubmitMode("always");
                }
            }
            if (result.tss_StateCode != null) {
                var tss_StateCode = result.tss_StateCode;
                if (tss_StateCode.Value == 865920000 && (tss_SourceType.Value == 865920003 || tss_SourceType.Value == 865920004)) {
                    setDisabledField("tss_partnumber", false);
                    setDisabledField("tss_isinterchange", false);
                    setDisabledField("tss_partnumberinterchange", false);
                    setDisabledField("tss_qtyrequest", false);
                }
            }
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, false);
    }
}

function SetPartNUmberInterchange() {
    Xrm.Page.getControl("tss_partnumberinterchange").setVisible(false);
    if (Xrm.Page.getAttribute("tss_isinterchange").getValue() != null) {
        if (Xrm.Page.getAttribute("tss_isinterchange").getValue()) {
            Xrm.Page.getControl("tss_partnumberinterchange").setVisible(true);
        } else {
            Xrm.Page.getAttribute("tss_partnumberinterchange").setValue(null);
            Xrm.Page.getAttribute("tss_partnumberinterchange").setSubmitMode("always");
            GetPartInfo();
        }
    }
}

function SetSalesMan() {
    if (Xrm.Page.getAttribute("tss_sopartheaderid").getValue() != null) {
        var headerid = Xrm.Page.getAttribute("tss_sopartheaderid").getValue()[0].id;
        XrmServiceToolkit.Rest.Retrieve(headerid, "tss_sopartheaderSet", "tss_PSS,tss_RequestDeliveryDate", null, function (result) {
            var tss_PSS = result.tss_PSS;
            var tss_RequestDeliveryDate = result.tss_RequestDeliveryDate;
            if (tss_PSS != null) {
                Xrm.Page.getAttribute("tss_salesman").setValue([{ id: tss_PSS.Id, name: tss_PSS.Name, entityType: tss_PSS.LogicalName }]);
                Xrm.Page.getAttribute("tss_salesman").setSubmitMode("always");
                Xrm.Page.getAttribute("tss_salesman").fireOnChange();
            }
            if (tss_RequestDeliveryDate != null) {
                Xrm.Page.getAttribute("tss_requestdeliverydate").setValue(tss_RequestDeliveryDate);
                Xrm.Page.getAttribute("tss_requestdeliverydate").setSubmitMode("always");
                Xrm.Page.getAttribute("tss_requestdeliverydate").fireOnChange();
            }
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, false);
    }
}

function CekFinalPrice() {
    if (Xrm.Page.getAttribute("tss_finalprice").getValue() != null) {
        var finalPice = Xrm.Page.getAttribute("tss_finalprice").getValue();
        if (finalPice == 0) {
            alert("Empty retrieve sparepart price");
        }
    } else {
        alert("Empty retrieve sparepart price");
    }
}

function CekFinalPrice_OnSave(context) {
    var saveEvent = context.getEventArgs();

    if (Xrm.Page.getAttribute("tss_finalprice").getValue() != null || Xrm.Page.getAttribute("tss_totalprice").getValue() != null) {
        var finalPice = Xrm.Page.getAttribute("tss_finalprice").getValue();
        var totalPice = Xrm.Page.getAttribute("tss_totalprice").getValue();
        var checkPart = checkBeforeInsert(Xrm.Page.getAttribute("tss_partnumber").getValue()[0].id);
        if (finalPice == 0 || totalPice == 0 || (checkPart == false && formType < 2)) {
            if (finalPice == 0) { alert("Final Price is empty. Can't save record."); }
            if (totalPice == 0) { alert("Total Price is empty. Can't save record."); }
            if (totalPice == 0 && finalPice == 0) { alert("Total Price and Final Price is empty. Can't save record."); }
            if (checkPart == false && formType < 2) {
                alert("Cannot save the record, part / part interchange already exist on this SO Part.");
            }

            saveEvent.preventDefault();
        }
    } else {
        alert("Empty retrieve sparepart price");
        saveEvent.preventDefault();
    }
}

function checkBeforeInsert(partNumber) {
    var oresult = false;
    var sopartheaderid = Xrm.Page.getAttribute("tss_sopartheaderid").getValue()[0].id;

    XrmServiceToolkit.Rest.RetrieveMultiple("tss_sopartlinesSet", "?$select=tss_sopartlinesId&$filter=tss_Partnumber/Id eq (guid'" + partNumber + "') and tss_SOPartHeaderId/Id eq (guid'" + sopartheaderid + "')", function (results) {
        if (results.length > 0) {
            oresult = false;
        }
        else {
            oresult = true;
        }
    }, function (error) {
        Xrm.Utility.alertDialog(error.message);
    }, function () {
        //On Complete - Do Something
    }, false);

    return oresult;
}

function GetPartInfo() {
    var partNumber;
    var quotation = false;
    var customer;
    var unitgroup;

    //get date with format 2017-12-31T17:00:00.000Z
    var currdate = new Date();
    var formattedDate =
            currdate.getFullYear() + "-" +
            ("00" + (currdate.getMonth() + 1)).slice(-2) + "-" +
            ("00" + currdate.getDate()).slice(-2) + "T" +
            ("00" + currdate.getHours()).slice(-2) + ":" +
            ("00" + currdate.getMinutes()).slice(-2) + ":" +
            ("00" + currdate.getSeconds()).slice(-2) + ".000Z";

    if (Xrm.Page.getAttribute("tss_sopartheaderid").getValue() != null) {
        var header = Xrm.Page.getAttribute("tss_sopartheaderid").getValue()[0].id;
        XrmServiceToolkit.Rest.Retrieve(header, "tss_sopartheaderSet", "tss_QuotationLink,tss_Customer", null, function (result) {
            if (result.tss_QuotationLink.Id != null) {
                quotation = true;
            }
            if (result.tss_Customer != null) {
                customer = result.tss_Customer.Id;
            }
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, false);
    }

    if (Xrm.Page.getAttribute("tss_partnumber").getValue() != null) {
        var partNumber = Xrm.Page.getAttribute("tss_partnumber").getValue()[0].id;
    }

    if (partNumber != null) {
        XrmServiceToolkit.Rest.Retrieve(partNumber, "trs_masterpartSet", "trs_PartDescription,trs_unitmeasurement,tss_unitgroup", null, function (result) {
            var trs_PartDescription = result.trs_PartDescription;
            var trs_unitmeasurement = result.trs_unitmeasurement;
            var tss_unitgroup = result.tss_unitgroup;
            if (trs_PartDescription != null) {
                Xrm.Page.getAttribute("tss_partdescription").setValue(trs_PartDescription);
                Xrm.Page.getAttribute("tss_partdescription").setSubmitMode("always");
            }
            if (trs_unitmeasurement != null) {
                Xrm.Page.getAttribute("tss_unit").setValue([{ id: trs_unitmeasurement.Id, name: trs_unitmeasurement.Name, entityType: trs_unitmeasurement.LogicalName }]);
                Xrm.Page.getAttribute("tss_unit").setSubmitMode("always");
            }
            if (tss_unitgroup != null) {
                unitgroup = tss_unitgroup.Id;
                Xrm.Page.getAttribute("tss_unitgroup").setValue([{ id: tss_unitgroup.Id, name: tss_unitgroup.Name, entityType: tss_unitgroup.LogicalName }]);
                Xrm.Page.getAttribute("tss_unitgroup").setSubmitMode("always");
            }
            Xrm.Page.getAttribute("tss_itemcategory").setValue(865920000);
            Xrm.Page.getAttribute("tss_itemcategory").setSubmitMode("always");
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, false);

        //check price list
        var pricelistpartid;
        XrmServiceToolkit.Rest.RetrieveMultiple("tss_pricelistpartSet", "?$select=tss_pricelistpartId&$filter=tss_name eq 'Direct%20Sales%20Price%20List'", function (results) {
            if (results.length > 0) {
                pricelistpartid = results[0].tss_pricelistpartId;
            }
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, function () {
        }, false);

        //check dealerheader
        var dealerheader;
        if (customer != null && Xrm.Page.getAttribute("tss_sourcetype").getValue() == 865920004) {
            XrmServiceToolkit.Rest.RetrieveMultiple("tss_dealerheaderSet", "?$select=tss_dealerheaderId&$filter=tss_DealerName/Id eq (guid'" + customer + "') and tss_PriceList/Id eq (guid'" + pricelistpartid + "') and tss_StartDate le datetime'" + formattedDate + "' and tss_EndDate ge datetime'" + formattedDate + "'", function (results) {
                if(results.length > 0){
                    dealerheader = results[0].tss_dealerheaderId;
                }
            }, function (error) {
                Xrm.Utility.alertDialog(error.message);
            }, function () {
            }, false);
        }

        //check dealerline
        var discby;
        var discAmount;
        var discPercent;
        if (dealerheader != null && unitgroup != null) {
            XrmServiceToolkit.Rest.RetrieveMultiple("tss_dealerlinesSet", "?$select=tss_DiscountAmount,tss_Discountby,tss_DiscountPercent&$filter=tss_DealerHeaderId/Id eq (guid'" + dealerheader + "') and tss_MaterialGroup/Id eq (guid'" + unitgroup + "')", function (results) {
                if (results.length > 0) {
                    if (results[0].tss_Discountby != null) {
                        discby = results[0].tss_Discountby;
                    }
                    if (results[0].tss_DiscountAmount != null) {
                        discAmount = results[0].tss_DiscountAmount;
                    }
                    if (results[0].tss_DiscountPercent != null) {
                        discPercent = results[0].tss_DiscountPercent;
                    }
                }
            }, function (error) {
                Xrm.Utility.alertDialog(error.message);
            }, function () {
            }, false);
        }
        
        if (pricelistpartid != null) {
            XrmServiceToolkit.Rest.RetrieveMultiple("tss_sparepartpricemasterSet", "?$select=tss_Price&$filter=tss_PartMaster/Id eq (guid'" + partNumber + "') and tss_PriceListPart/Id eq (guid'" + pricelistpartid + "')", function (results) {
                if (results.length > 0) {
                    var tss_Price = results[0].tss_Price;
                    if (tss_Price != null && quotation == false) {
                        var finalPrice = 0;

                        if (discby == true && discAmount != null) {
                            finalPrice = tss_Price.Value - discAmount.Value;
                        }
                        else if (discby == false && discPercent != null) {
                            finalPrice = (100 - discPercent) / 100 * tss_Price.Value;
                        }
                        else {
                            finalPrice = tss_Price.Value;
                        }

                        if (finalPrice < 0) finalPrice = 0;
                        Xrm.Page.getAttribute("tss_finalprice").setValue(parseFloat(eval(finalPrice)));
                        Xrm.Page.getAttribute("tss_finalprice").setSubmitMode("always");
                        Xrm.Page.getAttribute("tss_finalprice").fireOnChange();
                    }
                }
                else {
                    if (quotation == false) {
                        Xrm.Page.getAttribute("tss_finalprice").setValue(parseFloat(eval(0)));
                        Xrm.Page.getAttribute("tss_finalprice").setSubmitMode("always");
                        Xrm.Page.getAttribute("tss_finalprice").fireOnChange();
                    }
                }
            }, function (error) {
                Xrm.Utility.alertDialog(error.message);
            }, function () {
            }, false);
            //SetTotalPrice();
        }
        else {
            alert("Price list past not found!");
        }
    }
}

function GetPartInterchangeInfo() {
    var partNumber;
    var customer;
    var quotation = false;
    var unitgroup;

    //get date with format 2017-12-31T17:00:00.000Z
    var currdate = new Date();
    var formattedDate =
            currdate.getFullYear() + "-" +
            ("00" + (currdate.getMonth() + 1)).slice(-2) + "-" +
            ("00" + currdate.getDate()).slice(-2) + "T" +
            ("00" + currdate.getHours()).slice(-2) + ":" +
            ("00" + currdate.getMinutes()).slice(-2) + ":" +
            ("00" + currdate.getSeconds()).slice(-2) + ".000Z";

    if (Xrm.Page.getAttribute("tss_sopartheaderid").getValue() != null) {
        var header = Xrm.Page.getAttribute("tss_sopartheaderid").getValue()[0].id;
        XrmServiceToolkit.Rest.Retrieve(header, "tss_sopartheaderSet", "tss_QuotationLink,tss_Customer", null, function (result) {
            if (result.tss_QuotationLink.Id != null) {
                quotation = true;
            }
            if (result.tss_Customer != null) {
                customer = result.tss_Customer.Id;
            }
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, false);
    }

    if (Xrm.Page.getAttribute("tss_partnumberinterchange").getValue() != null) {
        var partNumberInterchange = Xrm.Page.getAttribute("tss_partnumberinterchange").getValue()[0].id;
        XrmServiceToolkit.Rest.Retrieve(partNumberInterchange, "tss_partmasterlinesinterchangeSet", "tss_PartNumberInterchange", null, function (result) {
            var tss_PartNumberInterchange = result.tss_PartNumberInterchange;
            if (tss_PartNumberInterchange != null) {
                partNumber = tss_PartNumberInterchange.Id;
            }
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, false);
    }

    if (partNumber != null) {
        XrmServiceToolkit.Rest.Retrieve(partNumber, "trs_masterpartSet", "trs_PartDescription,trs_unitmeasurement,tss_unitgroup", null, function (result) {
            var trs_PartDescription = result.trs_PartDescription;
            var trs_unitmeasurement = result.trs_unitmeasurement;
            var tss_unitgroup = result.tss_unitgroup;
            if (trs_PartDescription != null) {
                Xrm.Page.getAttribute("tss_partdescription").setValue(trs_PartDescription);
                Xrm.Page.getAttribute("tss_partdescription").setSubmitMode("always");
            }
            if (trs_unitmeasurement != null) {
                Xrm.Page.getAttribute("tss_unit").setValue([{ id: trs_unitmeasurement.Id, name: trs_unitmeasurement.Name, entityType: trs_unitmeasurement.LogicalName }]);
                Xrm.Page.getAttribute("tss_unit").setSubmitMode("always");
            }
            if (tss_unitgroup != null) {
                unitgroup = tss_unitgroup.Id;
                Xrm.Page.getAttribute("tss_unitgroup").setValue([{ id: tss_unitgroup.Id, name: tss_unitgroup.Name, entityType: tss_unitgroup.LogicalName }]);
                Xrm.Page.getAttribute("tss_unitgroup").setSubmitMode("always");
            }
            Xrm.Page.getAttribute("tss_itemcategory").setValue(865920000);
            Xrm.Page.getAttribute("tss_itemcategory").setSubmitMode("always");
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, false);

        //check price list
        var pricelistpartid;
        XrmServiceToolkit.Rest.RetrieveMultiple("tss_pricelistpartSet", "?$select=tss_pricelistpartId&$filter=tss_name eq 'Direct%20Sales%20Price%20List'", function (results) {
            if (results.length > 0) {
                pricelistpartid = results[0].tss_pricelistpartId;
            }
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, function () {
        }, false);

        //check dealerheader
        var dealerheader;
        if (customer != null && Xrm.Page.getAttribute("tss_sourcetype").getValue() == 865920004) {
            XrmServiceToolkit.Rest.RetrieveMultiple("tss_dealerheaderSet", "?$select=tss_dealerheaderId&$filter=tss_DealerName/Id eq (guid'" + customer + "') and tss_PriceList/Id eq (guid'" + pricelistpartid + "') and tss_StartDate le datetime'" + formattedDate + "' and tss_EndDate ge datetime'" + formattedDate + "'", function (results) {
                if (results.length > 0) {
                    dealerheader = results[0].tss_dealerheaderId;
                }
            }, function (error) {
                Xrm.Utility.alertDialog(error.message);
            }, function () {
            }, false);
        }

        //check dealerline
        var discby;
        var discAmount;
        var discPercent;
        if (dealerheader != null && unitgroup != null) {
            XrmServiceToolkit.Rest.RetrieveMultiple("tss_dealerlinesSet", "?$select=tss_DiscountAmount,tss_Discountby,tss_DiscountPercent&$filter=tss_DealerHeaderId/Id eq (guid'" + dealerheader + "') and tss_MaterialGroup/Id eq (guid'" + unitgroup + "')", function (results) {
                if (results.length > 0) {
                    if (results[0].tss_Discountby != null) {
                        discby = results[0].tss_Discountby;
                    }
                    if (results[0].tss_DiscountAmount != null) {
                        discAmount = results[0].tss_DiscountAmount;
                    }
                    if (results[0].tss_DiscountPercent != null) {
                        discPercent = results[0].tss_DiscountPercent;
                    }
                }
            }, function (error) {
                Xrm.Utility.alertDialog(error.message);
            }, function () {
            }, false);
        }

        if (pricelistpartid != null) {
            XrmServiceToolkit.Rest.RetrieveMultiple("tss_sparepartpricemasterSet", "?$select=tss_Price&$filter=tss_PartMaster/Id eq (guid'" + partNumber + "') and tss_PriceListPart/Id eq (guid'" + pricelistpartid + "')", function (results) {
                if (results.length > 0) {
                    var tss_Price = results[0].tss_Price;
                    if (tss_Price != null && quotation == false) {
                        var finalPrice = 0;

                        if (discby == true && discAmount != null) {
                            finalPrice = tss_Price.Value - discAmount.Value;
                        }
                        else if (discby == false && discPercent != null) {
                            finalPrice = (100 - discPercent) / 100 * tss_Price.Value;
                        }
                        else {
                            finalPrice = tss_Price.Value;
                        }

                        if (finalPrice < 0) finalPrice = 0;

                        Xrm.Page.getAttribute("tss_finalprice").setValue(parseFloat(eval(finalPrice)));
                        Xrm.Page.getAttribute("tss_finalprice").setSubmitMode("always");
                    }
                }
                else {
                    if (quotation == false) {
                        Xrm.Page.getAttribute("tss_finalprice").setValue(parseFloat(eval(0)));
                        Xrm.Page.getAttribute("tss_finalprice").setSubmitMode("always");
                    }
                }
            }, function (error) {
                Xrm.Utility.alertDialog(error.message);
            }, function () {
            }, false);
            SetTotalPrice();
        }
        else {
            alert("Price list past not found!");
        }
    }
}

function SetTotalPrice() {
    if (Xrm.Page.getAttribute("tss_qtyrequest").getValue() == null || Xrm.Page.getAttribute("tss_finalprice").getValue() == null) {
        Xrm.Page.getAttribute("tss_totalprice").setValue(parseFloat(eval(0)));
        Xrm.Page.getAttribute("tss_totalprice").setSubmitMode("always");
    }
    if (Xrm.Page.getAttribute("tss_qtyrequest").getValue() != null && Xrm.Page.getAttribute("tss_finalprice").getValue() != null) {
        var qty = Xrm.Page.getAttribute("tss_qtyrequest").getValue();
        var priceafterdiscount = Xrm.Page.getAttribute("tss_priceafterdiscount").getValue();
        var finalprice = Xrm.Page.getAttribute("tss_finalprice").getValue();

        if (isAuth) //20180820
        {
            if (priceafterdiscount != null && priceafterdiscount > 0) {
                Xrm.Page.getAttribute("tss_totalprice").setValue(parseFloat(eval(qty * priceafterdiscount)));
                Xrm.Page.getAttribute("tss_totalprice").setSubmitMode("always");
            }
            else
            {
                Xrm.Page.getAttribute("tss_totalprice").setValue(parseFloat(eval(qty * finalprice)));
                Xrm.Page.getAttribute("tss_totalprice").setSubmitMode("always");
            }
        }
        else
        {
            Xrm.Page.getAttribute("tss_totalprice").setValue(parseFloat(eval(qty * finalprice)));
            Xrm.Page.getAttribute("tss_totalprice").setSubmitMode("always");
        }
    }
}

function updateTotalPriceLines(sopartlinesId) {
    XrmServiceToolkit.Rest.Retrieve(sopartlinesId, "tss_sopartlinesSet", "tss_TotalPrice,tss_PriceAfterDiscount,tss_QtyRequest", null, function (result) {
        var tss_TotalPrice = result.tss_TotalPrice;
        var tss_PriceAfterDiscount = result.tss_PriceAfterDiscount;
        var tss_QtyRequest = result.tss_QtyRequest;

        if (tss_PriceAfterDiscount.Value != null && tss_PriceAfterDiscount.Value != 0) {
            var totalprice = tss_PriceAfterDiscount.Value * tss_QtyRequest;

            var entity = {};
            entity.tss_TotalPrice = {
                Value: parseFloat(totalprice).toFixed(2)
            };

            XrmServiceToolkit.Rest.Update(sopartlinesId, entity, "tss_sopartlinesSet", function () {
                //Success - No Return Data - Do Something
            }, function (error) {
                Xrm.Utility.alertDialog(error.message);
            }, false);
        }
    }, function (error) {
        Xrm.Utility.alertDialog(error.message);
    }, false);
}

function SetQty() {
    if(Xrm.Page.getAttribute("tss_qtysupply").getValue() == null){
        Xrm.Page.getAttribute("tss_qtysupply").setValue(0);
        Xrm.Page.getAttribute("tss_qtysupply").setSubmitMode("always");
        Xrm.Page.getAttribute("tss_qtysupply").fireOnChange();
    }
    if(Xrm.Page.getAttribute("tss_qtypartialnotsupply").getValue() == null){
        Xrm.Page.getAttribute("tss_qtypartialnotsupply").setValue(0);
        Xrm.Page.getAttribute("tss_qtypartialnotsupply").setSubmitMode("always");
        Xrm.Page.getAttribute("tss_qtypartialnotsupply").fireOnChange();
    }
    if(Xrm.Page.getAttribute("tss_qtyrequest").getValue() == null){
        Xrm.Page.getAttribute("tss_qtyrequest").setValue(0);
        Xrm.Page.getAttribute("tss_qtyrequest").setSubmitMode("always");
        Xrm.Page.getAttribute("tss_qtyrequest").fireOnChange();
    }
    if(Xrm.Page.getAttribute("tss_reservationqty").getValue() == null){
        Xrm.Page.getAttribute("tss_reservationqty").setValue(0);
        Xrm.Page.getAttribute("tss_reservationqty").setSubmitMode("always");
        Xrm.Page.getAttribute("tss_reservationqty").fireOnChange();
    }
}

function setPartInterchangeNull() {
    Xrm.Page.getAttribute("tss_isinterchange").setValue(0);
    Xrm.Page.getAttribute("tss_isinterchange").setSubmitMode("always");
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FIELD EVENT AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function DiscountBy_onChange() {
    if (isAuth) {
        if (Xrm.Page.getAttribute("tss_discountby").getValue() == 0) {
            if (formType == formStatus.Create && (Xrm.Page.getAttribute("tss_discountamount") == null || Xrm.Page.getAttribute("tss_discountpercent") == null)) {
                Xrm.Page.getAttribute("tss_discountamount").setValue(0);
                Xrm.Page.getAttribute("tss_discountpercent").setValue(0);

                Xrm.Page.getAttribute("tss_discountamount").setSubmitMode("always");
                Xrm.Page.getAttribute("tss_discountpercent").setSubmitMode("always");
            }

            discAmount_onChange();

            Xrm.Page.getControl("tss_discountamount").setDisabled(true);
            Xrm.Page.getControl("tss_discountpercent").setDisabled(false);
        }
        else if (Xrm.Page.getAttribute("tss_discountby").getValue() == 1) {
            if (formType == formStatus.Create && (Xrm.Page.getAttribute("tss_discountamount") == null || Xrm.Page.getAttribute("tss_discountpercent") == null)) {
                Xrm.Page.getAttribute("tss_discountpercent").setValue(0);
                Xrm.Page.getAttribute("tss_discountamount").setValue(0);

                Xrm.Page.getAttribute("tss_discountpercent").setSubmitMode("always");
                Xrm.Page.getAttribute("tss_discountamount").setSubmitMode("always");
            }

            discPercent_onChange();

            Xrm.Page.getControl("tss_discountpercent").setDisabled(true);
            Xrm.Page.getControl("tss_discountamount").setDisabled(false);
        }
    }
}

function qty_onChange() {
    //var qty = Xrm.Page.getAttribute("tss_qtyrequest").getValue();
    //var price = Xrm.Page.getAttribute("tss_finalprice").getValue();
    //var discP = Xrm.Page.getAttribute('tss_discountpercent').getValue();
    //var discA = Xrm.Page.getAttribute('tss_discountamount').getValue();

    /*if(qty != null){
        discAmount_onChange(qty);
        //discPercent_onChange(qty);
        Xrm.Page.getAttribute('tss_priceamount').setSubmitMode("always");
    }*/

    //if (Xrm.Page.getAttribute("tss_discountby").getValue() == false)
    //{
    //    Xrm.Page.getAttribute("tss_totalprice").setValue(parseInt(price * qty));
    //    Xrm.Page.getAttribute("tss_totalprice").setSubmitMode("always");
    //}
    //else if (Xrm.Page.getAttribute("tss_discountby").getText().toUpperCase() == "PERCENTAGE")
    //{
    //    discPercent_onChange(qty);
    //} else if (Xrm.Page.getAttribute("tss_discountby").getText().toUpperCase() == "AMOUNT") {
    //    discAmount_onChange(qty);
    //}

    //20180821
    //Xrm.Page.getAttribute("tss_discountby").setValue(0);

    //if (Xrm.Page.getAttribute("tss_discountby").getValue() == 0)
    //{
    //    discPercent_onChange(qty);
    //}
    //else if (Xrm.Page.getAttribute("tss_discountby").getValue() == 1)
    //{
    //    discAmount_onChange(qty);
    //}

    var qty = Xrm.Page.getAttribute('tss_qtyrequest').getValue();

    discAmount_onChange(qty);
    discPercent_onChange(qty);
    Xrm.Page.getAttribute('tss_totalprice').setSubmitMode("always");
}

function discAmount_onChange(quantity) {
    var qty;
    
    var discount = Xrm.Page.getAttribute('tss_discountamount').getValue();
    var price = Xrm.Page.getAttribute('tss_finalprice').getValue();
    var formulaPercent = (discount / price) * 100;
    var priceamount;

    if (price != null) {

        if (discount < price) {
            if (quantity == null) {
                qty = Xrm.Page.getAttribute('tss_qtyrequest').getValue();
                //Change Formula Amount: Price*(Discount Percent/100)
                var formulaAmount = Math.round(price * (formulaPercent / 100));
                if (qty > 0) {
                    var priceamount = ((price - formulaAmount));
                    var priceafterdiscount = (price - formulaAmount);
                    if (formulaAmount != 0) {
                        Xrm.Page.getAttribute('tss_priceafterdiscount').setValue(parseInt(priceafterdiscount));
                    } else { Xrm.Page.getAttribute('tss_priceafterdiscount').setValue(parseInt(price)); }

                    if (discount == null || formulaPercent == null) {
                        Xrm.Page.getAttribute('tss_discountamount').setValue(0);
                        Xrm.Page.getAttribute('tss_discountpercent').setValue(0);
                    } else {
                        Xrm.Page.getAttribute('tss_discountpercent').setValue(formulaPercent);
                    }
                    Xrm.Page.getAttribute('tss_totalprice').setValue(parseInt(priceafterdiscount * qty));
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
                        Xrm.Page.getAttribute('tss_totalprice').setValue(parseInt(priceamount));
                        Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));
                        Xrm.Page.getAttribute('tss_discountpercent').setValue(parseInt(0));
                    }
                        //var finalprice = Xrm.Page.getAttribute('tss_finalprice').getValue();
                        //var priceamount = (finalprice * qty);
                        //Xrm.Page.getAttribute('tss_priceamount').setValue(parseInt(priceamount));

                        //////TAMBAHAN AMIN//////////////////
                    else {
                        qty = Xrm.Page.getAttribute('tss_qtyrequest').getValue();
                        //Change Formula Amount: Price*(Discount Percent/100)
                        var formulaAmount = Math.round(price * (formulaPercent / 100));
                        if (qty > 0) {
                            var priceamount = ((price - formulaAmount) * qty);
                            var priceafterdiscount = (price - formulaAmount);
                            if (formulaAmount != 0) {
                                Xrm.Page.getAttribute('tss_priceafterdiscount').setValue(parseInt(priceafterdiscount));
                            } else { Xrm.Page.getAttribute('tss_priceafterdiscount').setValue(parseInt(price)); }

                            if (discount == null || formulaPercent == null) {
                                Xrm.Page.getAttribute('tss_discountamount').setValue(0);
                                Xrm.Page.getAttribute('tss_discountpercent').setValue(0);
                            } else {
                                Xrm.Page.getAttribute('tss_discountpercent').setValue(formulaPercent);
                            }
                            Xrm.Page.getAttribute('tss_totalprice').setValue(parseInt(priceafterdiscount * qty));
                        } else {
                            Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));
                            Xrm.Page.getAttribute('tss_discountpercent').setValue(parseInt(0));
                            Xrm.Page.getAttribute('tss_totalprice').setValue(parseInt(0));
                            //alert('Please fill the quantity on Prospect Part Lines !');
                        }
                    }
                    //////////////////////////////
                } else {
                    Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));
                    Xrm.Page.getAttribute('tss_discountpercent').setValue(0);
                    Xrm.Page.getAttribute('tss_totalprice').setValue(parseInt(0));
                    Xrm.Page.getAttribute('tss_finalprice').setValue(parseInt(0));
                    //Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));
                    //alert('Please fill the quantity on Prospect Part Lines !');
                }
            }


        } else {
            alert('Discount amount cannot be greater equal than price part.');
            if (quantity == null) {
                qty = Xrm.Page.getAttribute("tss_qtyrequest").getValue();
            }
            Xrm.Page.getAttribute("tss_discountamount").setValue(0);
            Xrm.Page.getAttribute("tss_discountpercent").setValue(0);
            Xrm.Page.getAttribute("tss_totalprice").setValue(parseInt(price * qty));
            Xrm.Page.getAttribute("tss_priceafterdiscount").setValue(parseInt(price));
        }
    }

    Xrm.Page.getAttribute("tss_finalprice").setSubmitMode("always"); 
    Xrm.Page.getAttribute("tss_discountamount").setSubmitMode("always");
    Xrm.Page.getAttribute("tss_discountpercent").setSubmitMode("always");
    Xrm.Page.getAttribute("tss_totalprice").setSubmitMode("always");
    Xrm.Page.getAttribute("tss_priceafterdiscount").setSubmitMode("always");
    Xrm.Page.getAttribute("tss_priceamount").setSubmitMode("always");
}

function discPercent_onChange(quantity) {
    
    var discount = Xrm.Page.getAttribute('tss_discountpercent').getValue();
    var price = Xrm.Page.getAttribute('tss_finalprice').getValue();
    var qty = quantity;
    var formulaPercent;
    var formulaAmount;
    var priceamount;
    var priceafterdiscount;

    //var qty = Xrm.Page.getAttribute('tss_quantity').getValue();
    if (discount < 100) {
        if (qty == null) {
            qty = Xrm.Page.getAttribute('tss_qtyrequest').getValue();
            formulaAmount = Math.round(price * (discount / 100));
            //var formulaAmount = (price*(discount/100));
            //alert('Discount Amount:Rp ' + formulaAmount);
            formulaPercent = (formulaAmount / price) * 100;
            if (qty > 0) {
                priceamount = ((price - formulaAmount));
                priceafterdiscount = (price - formulaAmount);
                if (formulaAmount != 0) {
                    Xrm.Page.getAttribute('tss_priceafterdiscount').setValue(parseInt(priceafterdiscount));
                } else { Xrm.Page.getAttribute('tss_priceafterdiscount').setValue(parseInt(price)); }
              
                if (discount == null || formulaAmount == null) {
                    Xrm.Page.getAttribute('tss_discountpercent').setValue(0);
                    Xrm.Page.getAttribute('tss_discountamount').setValue(0);
                } else {
                    Xrm.Page.getAttribute('tss_discountamount').setValue(formulaAmount);
                }
                Xrm.Page.getAttribute('tss_totalprice').setValue(parseInt(priceafterdiscount * qty));
            } else {
                Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));
                Xrm.Page.getAttribute('tss_discountpercent').setValue(0);
                Xrm.Page.getAttribute('tss_totalprice').setValue(parseInt(0));
                //alert('Please fill the quantity on Prospect Part Lines !');
            }
        } else {
            if (qty > 0) {
                var price = Xrm.Page.getAttribute('tss_finalprice').getValue();

                if (discount == 0) {
                    //formulaAmount = 0;
                    //formulaPercent = 0;
                    //priceafterdiscount = (price - formulaAmount);
                    priceamount = (price * qty);
                    //Xrm.Page.getAttribute('tss_finalprice').setValue(parseInt(priceafterdiscount));
                    Xrm.Page.getAttribute('tss_totalprice').setValue(parseInt(priceamount));
                    Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));
                    Xrm.Page.getAttribute('tss_discountpercent').setValue(parseInt(0));
                }
                    //            var finalprice = Xrm.Page.getAttribute('tss_finalprice').getValue();
                    //            var priceamount = (finalprice * qty);
                    //            Xrm.Page.getAttribute('tss_priceamount').setValue(parseInt(priceamount));

                else {
                    qty = Xrm.Page.getAttribute('tss_qtyrequest').getValue();
                    formulaAmount = Math.round(price * (discount / 100));
                    //var formulaAmount = (price*(discount/100));
                    //alert('Discount Amount:Rp ' + formulaAmount);
                    formulaPercent = (formulaAmount / price) * 100;
                    if (qty > 0) {
                        priceamount = ((price - formulaAmount));
                        priceafterdiscount = (price - formulaAmount);
                        if (formulaAmount != 0) {
                            Xrm.Page.getAttribute('tss_priceafterdiscount').setValue(parseInt(priceafterdiscount));
                        } else { Xrm.Page.getAttribute('tss_priceafterdiscount').setValue(parseInt(price)); }
                       
                        if (discount == null || formulaAmount == null) {
                            Xrm.Page.getAttribute('tss_discountpercent').setValue(0);
                            Xrm.Page.getAttribute('tss_discountamount').setValue(0);
                        } else {
                            Xrm.Page.getAttribute('tss_discountamount').setValue(formulaAmount);
                        }
                        Xrm.Page.getAttribute('tss_totalprice').setValue(parseInt(priceafterdiscount * qty));
                    } else {
                        Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));
                        Xrm.Page.getAttribute('tss_discountpercent').setValue(0);
                        Xrm.Page.getAttribute('tss_totalprice').setValue(parseInt(0));
                        //alert('Please fill the quantity on Prospect Part Lines !');
                    }
                }
                /////////////////////////////////////

            } else {
                Xrm.Page.getAttribute('tss_discountamount').setValue(parseInt(0));
                Xrm.Page.getAttribute('tss_discountpercent').setValue(0);
                Xrm.Page.getAttribute('tss_totalprice').setValue(parseInt(0));
                Xrm.Page.getAttribute('tss_finalprice').setValue(parseInt(0));
                //alert('Please fill the quantity on Prospect Part Lines !');
            }
        }


    } else {
        alert('Discount percent cannot be greater equal than 100 %.');
        if (quantity == null) {
            qty = Xrm.Page.getAttribute('tss_qtyrequest').getValue();
        }
        Xrm.Page.getAttribute('tss_discountamount').setValue(0);
        Xrm.Page.getAttribute('tss_discountpercent').setValue(0);
        Xrm.Page.getAttribute('tss_totalprice').setValue(parseInt(price * qty));
        Xrm.Page.getAttribute('tss_priceafterdiscount').setValue(parseInt(price));
    }

    Xrm.Page.getAttribute("tss_finalprice").setSubmitMode("always");
    Xrm.Page.getAttribute("tss_discountamount").setSubmitMode("always");
    Xrm.Page.getAttribute("tss_discountpercent").setSubmitMode("always");
    Xrm.Page.getAttribute("tss_totalprice").setSubmitMode("always");
    Xrm.Page.getAttribute("tss_priceafterdiscount").setSubmitMode("always");
}

function ShowTotalPartConsump() {
    var _totalpartconsumpmarketsize = GetAttributeValue(Xrm.Page.getAttribute("tss_totalpartconsumpmarketsize"));
    var _sourcetype = GetAttributeValue(Xrm.Page.getAttribute("tss_sourcetype"));

    if (_totalpartconsumpmarketsize != null && _sourcetype != null) {
        if (_sourcetype == 865920000) {
            Xrm.Page.getControl("tss_totalpartconsumpmarketsize").setVisible(true);
        }
    }
}

//20180823
//function discPercent_onChange(quantity)
//{
//    var qty = quantity;
//    var discountpercent = Xrm.Page.getAttribute('tss_discountpercent').getValue();
//    var finalprice = Xrm.Page.getAttribute('tss_finalprice').getValue();
//    var discountamount;
//    var priceafterdiscount;

//    if (discountpercent < 100)
//    {
//        if (qty == null) {
//            qty = Xrm.Page.getAttribute('tss_qtyrequest').getValue();
//        }

//        discountamount = Math.round(finalprice * (discountpercent / 100));
//        priceafterdiscount = (finalprice - discountamount);

//        if (priceafterdiscount != 0)
//        {
//            Xrm.Page.getAttribute('tss_priceafterdiscount').setValue(parseInt(priceafterdiscount));
//        }
//        else
//        {
//            Xrm.Page.getAttribute('tss_priceafterdiscount').setValue(parseInt(0));
//        }

//        if (discountpercent == null || discountamount == null)
//        {
//            Xrm.Page.getAttribute('tss_discountpercent').setValue(0);
//            Xrm.Page.getAttribute('tss_discountamount').setValue(0);
//        }
//        else
//        {
//            Xrm.Page.getAttribute('tss_discountpercent').setValue(discountpercent);
//            Xrm.Page.getAttribute('tss_discountamount').setValue(discountamount);
//        }

//        Xrm.Page.getAttribute('tss_totalprice').setValue(parseInt(priceafterdiscount * qty));
//        Xrm.Page.getAttribute("tss_totalprice").setSubmitMode("always");
//        Xrm.Page.getAttribute("tss_totalprice").fireOnChange();

//    }
//    else
//    {
//        alert('Discount percent cannot be greater equal than 100 %.');

//        if (qty == null)
//        {
//            qty = Xrm.Page.getAttribute('tss_qtyrequest').getValue();
//        }

//        Xrm.Page.getAttribute('tss_discountamount').setValue(0);
//        Xrm.Page.getAttribute('tss_discountpercent').setValue(0);
//        Xrm.Page.getAttribute('tss_totalprice').setValue(parseInt(finalprice * qty));
//        Xrm.Page.getAttribute('tss_priceafterdiscount').setValue(parseInt(0));
//    }

//    Xrm.Page.getAttribute("tss_discountamount").setSubmitMode("always");
//    Xrm.Page.getAttribute("tss_discountpercent").setSubmitMode("always");
//    Xrm.Page.getAttribute("tss_finalprice").setSubmitMode("always");
//    Xrm.Page.getAttribute("tss_priceafterdiscount").setSubmitMode("always");
//    Xrm.Page.getAttribute("tss_totalprice").setSubmitMode("always");
//}