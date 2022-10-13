function SetContract() {
    var serverUrl = "http://" + window.location.host + "/" + Xrm.Page.context.getOrgUniqueName();
    var unitlookup = Xrm.Page.getAttribute("trs_unit").getValue();
    var customerlookup = Xrm.Page.getAttribute("customerid").getValue();
    var createdon;
    if (Xrm.Page.getAttribute("createdon").getValue() != null) {
        createdon = getODataUTCDateFilter(Xrm.Page.getAttribute("createdon").getValue());
    }

    if (unitlookup != null && customerlookup != null && createdon != null) {
        var unitid = unitlookup[0].id;
        var customerid = customerlookup[0].id;
        var haveContract = false;

        // Creating the Odata Endpoint
        var oDataPath = serverUrl + "/XRMServices/2011/OrganizationData.svc";
        var retrieveReq = new XMLHttpRequest();

        var OdataPopulationInContract = oDataPath + "/trs_populationincontractSet?$select=trs_ContractLine&$filter=trs_Equipment/Id eq guid'" + unitid + "'";
        retrieveReq.open("GET", OdataPopulationInContract, false);
        retrieveReq.setRequestHeader("Accept", "application/json");
        retrieveReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        retrieveReq.onreadystatechange = function () { retrieveReqCallBack(this); };
        retrieveReq.send();
        if (retrieveReq.readyState == 4 /* complete */) {
            var retrievedpopulation = this.parent.JSON.parse(retrieveReq.responseText).d;
            if (retrievedpopulation.results != null && retrievedpopulation.results.length > 0) {
                for (var i = 0; i < retrievedpopulation.results.length; i++) {
                    var contractlinelookup = retrievedpopulation.results[i].trs_ContractLine;
                    var contractlineid = contractlinelookup.Id;
                    var OdataContractLine = oDataPath + "/ContractDetailSet?$select=ContractDetailId,ContractId,Title,contract_line_items/Title&$expand=contract_line_items&$filter=CustomerId/Id eq guid'" + customerid + "' and ActiveOn le " + createdon + " and ExpiresOn ge " + createdon + " and ContractDetailId eq guid'" + contractlineid + "'";

                    var retrieveReqContract = new XMLHttpRequest();
                    retrieveReqContract.open("GET", OdataContractLine, false);
                    retrieveReqContract.setRequestHeader("Accept", "application/json");
                    retrieveReqContract.setRequestHeader("Content-Type", "application/json; charset=utf-8");
                    retrieveReqContract.onreadystatechange = function () { retrieveReqCallBack(this); };
                    retrieveReqContract.send();

                    if (retrieveReqContract.readyState == 4 /* complete */) {
                        var retrievedcontractdetail = this.parent.JSON.parse(retrieveReqContract.responseText).d;
                        if (retrievedcontractdetail != null && retrievedcontractdetail.results.length > 0) {
                            haveContract = true;
                            var result = retrievedcontractdetail.results[0];
                            if (result != null) {
                                if (result.ContractId != null && result.contract_line_items.Title != null) {
                                    var contractheader = new Array();
                                    contractheader[0] = new Object();
                                    contractheader[0].id = result.ContractId.Id;
                                    contractheader[0].name = result.contract_line_items.Title;
                                    contractheader[0].entityType = "contract";
                                    Xrm.Page.getAttribute("contractid").setValue(contractheader);
                                    Xrm.Page.getAttribute("contractid").setSubmitMode("always");
                                    haveContract = true;
                                }
                                if (result.ContractDetailId != null && result.Title != null) {
                                    var contractdetail = new Array();
                                    contractdetail[0] = new Object();
                                    contractdetail[0].id = result.ContractDetailId;
                                    contractdetail[0].name = result.Title;
                                    contractdetail[0].entityType = "contractdetail";
                                    Xrm.Page.getAttribute("contractdetailid").setValue(contractdetail);
                                    Xrm.Page.getAttribute("contractdetailid").setSubmitMode("always");
                                }
                            }
                        }
                    }
                }
            }
            else
                haveContract == false;
        }
    }
    if (haveContract == false) {
        Xrm.Page.getAttribute("contractid").setValue(null);
        Xrm.Page.getAttribute("contractid").setSubmitMode("always");

        Xrm.Page.getAttribute("contractdetailid").setValue(null);
        Xrm.Page.getAttribute("contractdetailid").setSubmitMode("always");
    }
}

//    if (retrievedpopulation.results[0].trs_EquipmentsId != null) {
//        var OdataContractLine = oDataPath + "/ContractDetailSet?$select=ContractDetailId,ContractId,Title,contract_line_items/Title&$expand=contract_line_items&$filter=trs_Customer/Id eq guid'" + customerid + "' and ContractDetailId eq guid'" + retrievedpopulation.results[0].trs_EquipmentsId.Id + "'";
//    }
function getODataUTCDateFilter(date) {
    var monthString;
    var rawMonth = (date.getUTCMonth() + 1).toString();
    if (rawMonth.length == 1) {
        monthString = "0" + rawMonth;
    }
    else { monthString = rawMonth; }

    var dateString;
    var rawDate = date.getUTCDate().toString();
    if (rawDate.length == 1) {
        dateString = "0" + rawDate;
    }
    else { dateString = rawDate; }

    var hourString = date.getUTCHours().toString();
    if (hourString.length == 1)
        hourString = "0" + hourString;

    var minuteString = date.getUTCMinutes().toString();
    if (minuteString.length == 1)
        minuteString = "0" + minuteString;

    var secondString = date.getUTCSeconds().toString();
    if (secondString.length == 1)
        secondString = "0" + secondString;

    var DateFilter = "datetime'";
    DateFilter += date.getUTCFullYear() + "-";
    DateFilter += monthString + "-";
    DateFilter += dateString;
    DateFilter += "T" + hourString + ":";
    DateFilter += minuteString + ":";
    DateFilter += secondString + "Z'";
    return DateFilter;
}

function preFilterLookupCustomer() {
    Xrm.Page.getControl("trs_unit").addPreSearch(function () {
        addLookupFilter();
    });
}

function addLookupFilterCustomer() {
    var cust = Xrm.Page.getAttribute("customerid").getValue();

    if (cust != null) {
        fetchXml = "<filter type='and'><condition attribute='new_customercode' operator='eq' value='" + cust[0].id + "' /></filter>";
        Xrm.Page.getControl("trs_unit").addCustomFilter(fetchXml);
    }
}


function preFilterLookupUnit() {
    
    Xrm.Page.getControl("customerid").addPreSearch(function () {
        addLookupFilterUnit();
    });
}

function addLookupFilterUnit() {
    var serverUrl = "http://" + window.location.host + "/" + Xrm.Page.context.getOrgUniqueName();
    var oDataPath = serverUrl + "/XRMServices/2011/OrganizationData.svc";
    var HeaderRetrieveReq = new XMLHttpRequest();
    var unit = Xrm.Page.getAttribute("trs_unit").getValue();
    var Odata = oDataPath + "/new_populationSet?$select=new_customercode&$filter=new_populationId eq guid'" + unit[0].id + "'";

    HeaderRetrieveReq.open("GET", Odata, false);
    HeaderRetrieveReq.setRequestHeader("Accept", "application/json");
    HeaderRetrieveReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    HeaderRetrieveReq.onreadystatechange = function () { RetrieveAccount(this); };
    HeaderRetrieveReq.send();

    if (HeaderRetrieveReq.readyState == 4 /* complete */) {
        var FieldRetrieved = this.parent.JSON.parse(HeaderRetrieveReq.responseText).d;

        if (FieldRetrieved != null && FieldRetrieved.results.length > 0) {
            if (FieldRetrieved.results[0].new_customercode != null) {

                //alert(FieldRetrieved.results[0].new_customercode.Id + " = " +  FieldRetrieved.results[0].new_customercode.Title);

                var accountID = FieldRetrieved.results[0].new_customercode.Id;

                if (accountID != null) {
                    fetchXml = "<filter type='and'><condition attribute='accountid' operator='eq' value='" + accountID + "' /></filter>";
                    Xrm.Page.getControl("customerid").addCustomFilter(fetchXml);
                }


                //var acc = new Array();
                //acc[0] = new Object();
                //acc[0].id = FieldRetrieved.results[0].new_customercode.Id;
                //acc[0].name = "name";
                //acc[0].entityType = "account";
                //Xrm.Page.getAttribute("customerid").setValue(acc);
                //Xrm.Page.getAttribute("customerid").setSubmitMode("always");

            }

        }
    }
}

function SetCustomerByUnit() {
    var serverUrl = "http://" + window.location.host + "/" + Xrm.Page.context.getOrgUniqueName();
    var oDataPath = serverUrl + "/XRMServices/2011/OrganizationData.svc";
    var HeaderRetrieveReq = new XMLHttpRequest();
    var unit = Xrm.Page.getAttribute("trs_unit").getValue();
    var Odata = oDataPath + "/new_populationSet?$select=new_customercode&$filter=new_populationId eq guid'" + unit[0].id + "'";

    HeaderRetrieveReq.open("GET", Odata, false);
    HeaderRetrieveReq.setRequestHeader("Accept", "application/json");
    HeaderRetrieveReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    HeaderRetrieveReq.onreadystatechange = function () { RetrieveAccount(this); };
    HeaderRetrieveReq.send();

    if (HeaderRetrieveReq.readyState == 4) {
        var FieldRetrieved = this.parent.JSON.parse(HeaderRetrieveReq.responseText).d;

        if (FieldRetrieved != null && FieldRetrieved.results.length > 0) {
            if (FieldRetrieved.results[0].new_customercode != null) {

                var acc = new Array();
                acc[0] = new Object();
                acc[0].id = FieldRetrieved.results[0].new_customercode.Id;
                acc[0].name = FieldRetrieved.results[0].new_customercode.Name;
                acc[0].entityType = "account";
                Xrm.Page.getAttribute("customerid").setValue(acc);
                Xrm.Page.getAttribute("customerid").setSubmitMode("always");
            }
        }
    }
}

function FillTopicbyPMActType() {
    var pmacttypelookup = Xrm.Page.getAttribute("trs_pmacttype").getValue();
    if (pmacttypelookup != null) {
        Xrm.Page.getAttribute("title").setValue(pmacttypelookup[0].name);
    }
    else {
        Xrm.Page.getAttribute("title").setValue(null);
    }
}