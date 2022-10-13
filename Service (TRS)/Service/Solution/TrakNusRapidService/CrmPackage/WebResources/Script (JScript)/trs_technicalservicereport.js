function SetWarrantyStatus() {
    var serverUrl = "http://" + window.location.host + "/" + Xrm.Page.context.getOrgUniqueName();
    var unitlookup = Xrm.Page.getAttribute("trs_equipment").getValue();
    Xrm.Page.getAttribute("trs_warrantystatus").setValue(false);
    if (unitlookup != null) {
        var unitid = unitlookup[0].id;
        // Creating the Odata Endpoint
        var oDataPath = serverUrl + "/XRMServices/2011/OrganizationData.svc";
        var retrieveReq = new XMLHttpRequest();

        var OdataPopulation = oDataPath + "/new_populationSet?$select=trs_WarrantyEndDate,trs_WarrantyStartdate&$filter=new_populationId eq guid'" + unitid + "'";
        retrieveReq.open("GET", OdataPopulation, false);
        retrieveReq.setRequestHeader("Accept", "application/json");
        retrieveReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        retrieveReq.onreadystatechange = function () { retrieveReqCallBack(this); };
        retrieveReq.send();
        if (retrieveReq.readyState == 4 /* complete */) {
            var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
            if (retrieved != null && retrieved.results.length > 0) {
                if (retrieved.results[0].trs_WarrantyStartdate != null && retrieved.results[0].trs_WarrantyEndDate != null) {
                    var warrantystart = new Date(parseInt(retrieved.results[0].trs_WarrantyStartdate.replace("/Date(", "").replace(")/", ""), 10));
                    var warrantyend = new Date(parseInt(retrieved.results[0].trs_WarrantyEndDate.replace("/Date(", "").replace(")/", ""), 10));
                    var currentdate = new Date();

                    if (currentdate >= warrantystart && currentdate <= warrantyend) {
                        Xrm.Page.getAttribute("trs_warrantystatus").setValue(true);
                    }
                }
            }
        }
        SetFormByWarrantyStatus();
    }
}

function SetFormByWarrantyStatus() {
    var warrantystatus = Xrm.Page.getAttribute("trs_warrantystatus").getValue();
    if (warrantystatus == false) {
        Xrm.Page.getControl('trs_claimable').setDisabled(true);
        Xrm.Page.getControl('trs_claimnumber').setDisabled(true);
        Xrm.Page.getControl('trs_claimdate').setDisabled(true);
        Xrm.Page.getControl('trs_claimvalue').setDisabled(true);
        Xrm.Page.getControl('trs_notclaimablereason').setDisabled(true);

        Xrm.Page.getAttribute('trs_claimable').setValue(false);
        Xrm.Page.getAttribute("trs_claimable").setRequiredLevel("none");
        Xrm.Page.getAttribute("trs_claimnumber").setRequiredLevel("none");
        Xrm.Page.getAttribute("trs_claimdate").setRequiredLevel("none");
        Xrm.Page.getAttribute("trs_claimvalue").setRequiredLevel("none");
        Xrm.Page.getAttribute("trs_notclaimablereason").setRequiredLevel("none");
    }
    else {
        Xrm.Page.getControl('trs_claimable').setDisabled(false);
        Xrm.Page.getControl('trs_claimnumber').setDisabled(false);
        Xrm.Page.getControl('trs_claimdate').setDisabled(false);
        Xrm.Page.getControl('trs_claimvalue').setDisabled(true);
        Xrm.Page.getControl('trs_notclaimablereason').setDisabled(false);

        Xrm.Page.getAttribute('trs_claimable').setValue(false);
        Xrm.Page.getAttribute("trs_claimable").setRequiredLevel("required");
        Xrm.Page.getAttribute("trs_claimnumber").setRequiredLevel("required");
        Xrm.Page.getAttribute("trs_claimdate").setRequiredLevel("required");
        Xrm.Page.getAttribute("trs_claimvalue").setRequiredLevel("none");
        Xrm.Page.getAttribute("trs_notclaimablereason").setRequiredLevel("required");
    }
}

function SetFormByClaimable() {
    var claimable = Xrm.Page.getAttribute("trs_claimable").getValue();
    if (claimable == true) {
        Xrm.Page.getControl('trs_claimvalue').setDisabled(false);
        Xrm.Page.getControl('trs_notclaimablereason').setDisabled(true);
        Xrm.Page.getAttribute("trs_claimvalue").setRequiredLevel("required");
        Xrm.Page.getAttribute("trs_notclaimablereason").setRequiredLevel("none");
    }
    else {
        Xrm.Page.getControl('trs_claimvalue').setDisabled(true);
        Xrm.Page.getControl('trs_notclaimablereason').setDisabled(false);
        Xrm.Page.getAttribute("trs_claimvalue").setRequiredLevel("none");
        Xrm.Page.getAttribute("trs_notclaimablereason").setRequiredLevel("required");
    }
}

function getProductId(equipmentId) {
    var serverUrl = "http://" + window.location.host + "/" + Xrm.Page.context.getOrgUniqueName();
    var sReturn = "";

    if (equipmentId != null) {
        // Creating the Odata Endpoint
        var oDataPath = serverUrl + "/XRMServices/2011/OrganizationData.svc";
        var retrieveReq = new XMLHttpRequest();
        var Odata = oDataPath + "/new_populationSet?$select=trs_ProductMaster&$filter=new_populationId eq guid'" + equipmentId + "'";
        retrieveReq.open("GET", Odata, false);
        retrieveReq.setRequestHeader("Accept", "application/json");
        retrieveReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        retrieveReq.onreadystatechange = function () { retrieveReqCallBack(this); };
        retrieveReq.send();
        if (retrieveReq.readyState == 4) {
            var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
            if (retrieved.results != null && retrieved.results.length > 0) {
                if (retrieved.results[0].trs_ProductMaster != null) {
                    sReturn = retrieved.results[0].trs_ProductMaster.Id;
                }
            }
        }
    }
    return sReturn;
}

function setProductType() {
    debugger;

    var serverUrl = "http://" + window.location.host + "/" + Xrm.Page.context.getOrgUniqueName();
    var EquipmentLookup = Xrm.Page.getAttribute("trs_equipment").getValue();

    if (EquipmentLookup != null) {
        var EquipmentLookupID = EquipmentLookup[0].id;
        var ProductID = getProductId(EquipmentLookupID);

        // Creating the Odata Endpoint
        var oDataPath = serverUrl + "/XRMServices/2011/OrganizationData.svc";
        var retrieveReq = new XMLHttpRequest();
        var Odata = oDataPath + "/ProductSet?$select=trs_producttypeid&$filter=ProductId eq guid'" + ProductID + "'";
        retrieveReq.open("GET", Odata, false);
        retrieveReq.setRequestHeader("Accept", "application/json");
        retrieveReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        retrieveReq.onreadystatechange = function () { retrieveReqCallBack(this); };
        retrieveReq.send();
        try {
            if (retrieveReq.readyState == 4) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved.results != null && retrieved.results.length > 0) {
                    if (retrieved.results[0].trs_producttypeid != null) {
                        var arr = new Array();
                        arr[0] = new Object();
                        arr[0].id = retrieved.results[0].trs_producttypeid.Id;
                        arr[0].name = retrieved.results[0].trs_producttypeid.Name;
                        arr[0].entityType = retrieved.results[0].trs_producttypeid.LogicalName;
                        Xrm.Page.getAttribute("trs_producttype").setValue(arr);
                        Xrm.Page.getAttribute("trs_producttype").setSubmitMode("always");
                        Xrm.Page.getAttribute("trs_producttype").fireOnChange();
                    }
                }
            }
        }
        catch (e) {
            alert(e.message);
        }
    }
}