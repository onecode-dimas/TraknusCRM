function onLoadEvent() {
    Xrm.Page.getControl("trs_equipment").setDisabled(true);
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