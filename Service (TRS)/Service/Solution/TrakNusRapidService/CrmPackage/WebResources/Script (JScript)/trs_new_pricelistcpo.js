///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Helper
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Publics
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function GetDefaultPriceListCPOCurrency(pricelistId) {
    try {
        var currency = null;
        var path = "/new_pricelistcpoSet?$select=trs_Currency&$filter=new_pricelistcpoId eq guid'" + pricelistId + "'";
        var retrieveReq = RetrieveOData(path);
        if (retrieveReq.readyState == 4 /* complete */) {
            var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
            if (retrieved.results != null && retrieved.results.length > 0) {
                if (retrieved.results[0].trs_Currency != null) {
                    currency = retrieved.results[0].trs_Currency;
                }
            }
        }
        if (currency == null)
            throw new Error("Can not found master data for this Price List !");
        else
            return currency;
    }
    catch (e) {
        throw new Error("GetDefaultCurrency : " + e.message);
    }
}

function GetDefaultPriceType() {
    try {
        var priceType = null;
        var path = "/new_pricelistcpoSet?$select=new_pricelistcpoId,new_name&$filter=new_Code eq 'P1'";
        var retrieveReq = RetrieveOData(path);
        if (retrieveReq.readyState == 4 /* complete */) {
            var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
            if (retrieved.results != null && retrieved.results.length > 0) {
                priceType = {Id : retrieved.results[0].new_pricelistcpoId, Name : retrieved.results[0].new_name, LogicalName : "new_pricelistcpo" };
            }
        }
        if (priceType == null)
            throw new Error("Can not found master data for this Price List !");
        else
            return priceType;
    }
    catch (e) {
        throw new Error("GetDefaultPriceType : " + e.message);
    }
}