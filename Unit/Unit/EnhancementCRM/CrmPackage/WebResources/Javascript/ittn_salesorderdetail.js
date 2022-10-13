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
    DynamicallyFilterSubgrid();
}

function DynamicallyFilterSubgrid() {
    var objSubGrid = document.getElementById("CPOConditionType");

    //if (objSubGrid == null || objSubGrid.readyState != "complete") {
    //    setTimeout(DynamicallyFilterSubgrid, 2000);

    //    return;
    //}

    if (objSubGrid == null) {
        setTimeout(DynamicallyFilterSubgrid, 2000);

        return;
    }
    else {
        debugger;

        var _salesorderid = Xrm.Page.getAttribute('salesorderid').getValue();
        var _itemnumber = Xrm.Page.getAttribute('new_itemnumber').getValue();

        var FetchXml = "<fetch mapping='logical' version='1.0' distinct='false' output-format='xml-platform'>" +
            "<entity name='ittn_cpoconditiontype'>" +
            "<attribute name='ittn_cpoconditiontypeid' />" +
            "<attribute name='ittn_name' />" +
            "<attribute name='createdon' />" +
            "<order descending='false' attribute='ittn_name' />" +
            "<filter type='and'>" +
            "<condition value='" + _salesorderid[0].id + "' attribute='ittn_cpo' uitype='salesorder' operator='eq' />" +
            "<condition value='" + _itemnumber + "' attribute='ittn_itemnumber' operator='eq' />" +
            "</filter>" +
            "</entity>" +
            "</fetch>";

        objSubGrid.control.SetParameter("fetchXml", FetchXml);

        objSubGrid.control.Refresh();

    }
}
