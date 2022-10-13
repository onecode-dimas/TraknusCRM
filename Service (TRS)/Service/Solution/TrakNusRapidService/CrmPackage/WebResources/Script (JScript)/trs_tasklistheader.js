var defaultPriceType = 167630001;

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
function GetTaskListPrice(priceType, tasklistheaderId) {
    try {
        var price = null;
        if (priceType == defaultPriceType) {    //P2
            var path = "/trs_tasklistheaderSet?$select=trs_Price&$filter=trs_tasklistheaderId eq guid'" + tasklistheaderId + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved.results != null && retrieved.results.length > 0) {
                    price = retrieved.results[0].trs_Price.Value;
                }
            }
        }
        else if (priceType == 167630000) {      //P1
            throw new Error("Can not use Price Type 'P1' use in this version yet !");
        }
        else {
            throw new Error("Can not found Price Type !");
        }
        if (price == null) {
            throw new Error("Can not found Price !");
        }
        else {
            return price;
        }
    }
    catch (e) {
        throw new Error("GetTaskListPrice : " + e.message);
    }
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FORM ON LOAD AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FORM ON SAVE AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FIELD EVENT AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
