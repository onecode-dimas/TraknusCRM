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
        //retrieveReq.onreadystatechange = function () { retrieveReqCallBack(this); };
        retrieveReq.send();
        return retrieveReq;
    }
    catch (e) {
        throw new Error("RetrieveOData : Failed to retrieve OData !");
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

function OnLoad() {
    debugger;
    var _mandatory = GetAttributeValue(Xrm.Page.getAttribute("ittn_mandatory"));
    var _parentnumber = GetAttributeValue(Xrm.Page.getAttribute("new_parentnumber"));
    var _itemnumber = GetAttributeValue(Xrm.Page.getAttribute("new_itemnumber"));
    var _formType = Xrm.Page.ui.getFormType();

    //if (_mandatory == true && _parentnumber != null) {
    //    PreFilter_ExistingProduct();
    //}

    if (_parentnumber != null && _itemnumber != null) {
        var _formname = Xrm.Page.ui.formSelector.getCurrentItem().getLabel();
        if (_formname == 'Information') Xrm.Page.ui.tabs.get("tab_salesbomdetails").setVisible(false);

        PreFilter_ExistingProduct();
    }

    if (_parentnumber == null && _itemnumber != null) {
        Xrm.Page.getControl("ittn_alternativebom").setVisible(true);
        PreFilter_AlternativeBOM();
    }
    else {
        Xrm.Page.getControl("ittn_alternativebom").setVisible(false);
    }

    if (_formType < 2) {
        Xrm.Page.getAttribute("ittn_mandatory").setValue(1);
        Xrm.Page.getAttribute("ittn_mandatory").setValue(0);
        Xrm.Page.getAttribute("ittn_mandatory").setSubmitMode("always");
        Xrm.Page.getAttribute("ittn_mandatory").fireOnChange();
        Xrm.Page.getControl("ittn_mandatory").setVisible(false);
    }
    else {
        Xrm.Page.getControl("ittn_mandatory").setVisible(true);
    }
    //CheckDirtyAttributes();
}

function OnChange_ExistingProduct() {
    debugger;
    var _product = GetAttributeValue(Xrm.Page.getAttribute("new_product"));

    if (_product != null) {
        XrmServiceToolkit.Rest.RetrieveMultiple("ittn_salesbomSet", "?$select=ittn_name,ittn_salesbomId&$filter=ittn_Product/Id eq (guid'" + _product[0].id + "') and ittn_AlternativeBOM eq 1", function (results) {
            if (results.length > 0) {
                Xrm.Page.getControl("ittn_alternativebom").setVisible(true);

                Xrm.Page.getAttribute("ittn_alternativebom").setValue([{ id: results[0].ittn_salesbomId, name: results[0].ittn_name, entityType: "ittn_salesbom" }]);
                Xrm.Page.getAttribute("ittn_alternativebom").setSubmitMode("always");
                Xrm.Page.getAttribute("ittn_alternativebom").fireOnChange();
            }
            else {
                Xrm.Page.getControl("ittn_alternativebom").setVisible(false);
                Xrm.Page.getAttribute("ittn_alternativebom").setValue(null);
                Xrm.Page.getAttribute("ittn_alternativebom").setSubmitMode("always");
                Xrm.Page.getAttribute("ittn_alternativebom").fireOnChange();
            }

        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, function () {
            //On Complete - Do Something
        }, false);
    }
    else {
        Xrm.Page.getControl("ittn_alternativebom").setVisible(false);
        Xrm.Page.getAttribute("ittn_alternativebom").setValue(null);
        Xrm.Page.getAttribute("ittn_alternativebom").setSubmitMode("always");
        Xrm.Page.getAttribute("ittn_alternativebom").fireOnChange();
    }
}

function PreFilter_ExistingProduct() {
    debugger;

    var _opportunity = GetAttributeValue(Xrm.Page.getAttribute("opportunityid"));
    var _itemnumber = GetAttributeValue(Xrm.Page.getAttribute("new_itemnumber"));
    var _parentnumber = GetAttributeValue(Xrm.Page.getAttribute("new_parentnumber"));
    var _materialgroup;
    var _unitgroup;
    var _productname;

    var path = "/OpportunityProductSet?$select=new_Product&$filter=OpportunityId/Id eq (guid'" + _opportunity[0].id + "') and new_ItemNumber eq " + _parentnumber;
    var retrieveReq = RetrieveOData(path);
    if (retrieveReq.readyState == 4 /* complete */) {
        var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
        if (retrieved != null && retrieved.results.length > 0) {
            var _product = retrieved.results[0].new_Product.Id;
            
            path = "/ProductSet?$select=Name,new_MaterialGroup,new_UnitGroup&$filter=ProductId eq (guid'" + _product + "')";
            retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;

                if (retrieved != null && retrieved.results.length > 0) {
                    //_materialgroup = retrieved.results[0].new_MaterialGroup.Id;
                    _unitgroup = retrieved.results[0].new_UnitGroup.Id;
                    _productname = retrieved.results[0].Name;
                }
            }
        }
    }

    path = "/OpportunityProductSet?$select=new_Product&$filter=OpportunityId/Id eq (guid'" + _opportunity[0].id + "') and new_ItemNumber eq " + _itemnumber;
    retrieveReq = RetrieveOData(path);
    if (retrieveReq.readyState == 4 /* complete */) {
        var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
        if (retrieved != null && retrieved.results.length > 0) {
            var _product = retrieved.results[0].new_Product.Id;

            path = "/ProductSet?$select=Name,new_MaterialGroup,new_UnitGroup&$filter=ProductId eq (guid'" + _product + "')";
            retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;

                if (retrieved != null && retrieved.results.length > 0) {
                    _materialgroup = retrieved.results[0].new_MaterialGroup.Id;
                    //_unitgroup = retrieved.results[0].new_UnitGroup.Id;
                }
            }
        }
    }

    if (_materialgroup != null && _unitgroup != null && _productname != null) {
        var _fieldname = "new_product";
        Xrm.Page.getControl(_fieldname).setDisabled(false);
        Xrm.Page.getControl(_fieldname)._control && Xrm.Page.getControl(_fieldname)._control.tryCompleteOnDemandInitialization && Xrm.Page.getControl(_fieldname)._control.tryCompleteOnDemandInitialization();
        Xrm.Page.getControl(_fieldname).addPreSearch(function () {

            var fetchFilters = "<filter type='and'>"
                + "<condition attribute='new_materialgroup'  operator='eq' value='" + _materialgroup + "'/>"
                + "<condition attribute='new_unitgroup'  operator='eq' value='" + _unitgroup + "'/>"
                + "<condition attribute='name' operator='eq' value='" + _productname + "'/>"
                + "</filter>";
            Xrm.Page.getControl(_fieldname).addCustomFilter(fetchFilters);

        });
    }
}

function PreFilter_AlternativeBOM() {
    var _parentnumber = GetAttributeValue(Xrm.Page.getAttribute("new_parentnumber"));

    if (_parentnumber == null) {

        var _product = GetAttributeValue(Xrm.Page.getAttribute("new_product"));
        var _fieldname = "ittn_alternativebom";
        Xrm.Page.getControl(_fieldname).setDisabled(false);
        Xrm.Page.getControl(_fieldname)._control && Xrm.Page.getControl(_fieldname)._control.tryCompleteOnDemandInitialization && Xrm.Page.getControl(_fieldname)._control.tryCompleteOnDemandInitialization();
        Xrm.Page.getControl(_fieldname).addPreSearch(function () {

            var fetchFilters = "<filter type='and'>"
                + "<condition attribute='ittn_product'  operator='eq' value='" + _product[0].id + "'/>"
                + "</filter>";
            Xrm.Page.getControl(_fieldname).addCustomFilter(fetchFilters);

        });
    }
}

function CheckDirtyAttributes() {
    attributes = Xrm.Page.data.entity.attributes.get();

    if (attributes != null) {
        for (var i in attributes) {
            if (attributes[i].getIsDirty()) {
                Xrm.Page.getAttribute(attributes[i].getName()).setValue(attributes[i].getValue());
                Xrm.Page.getAttribute(attributes[i].getName()).setSubmitMode("always");
                Xrm.Page.getAttribute(attributes[i].getName()).fireOnChange();
                alert(attributes[i].getName());
            }
        }
    }
}