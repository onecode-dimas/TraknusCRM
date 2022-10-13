function calculateTotal() {
    var labour = Xrm.Page.getAttribute("trs_serviceprice").getValue();
    var accomodation = Xrm.Page.getAttribute("trs_accommodation").getValue();
    var transportation = Xrm.Page.getAttribute("trs_transportation").getValue();
    //var total = labour + accomodation + transportation;
    Xrm.Page.getAttribute("price").setValue(labour + accomodation + transportation);
    Xrm.Page.getAttribute("price").setSubmitMode("always");
}

function preFilterLookup() {
    Xrm.Page.getControl("trs_contact").addPreSearch(function () {
        addLookupFilter();
    });
}
function addLookupFilter() {
    var customer = Xrm.Page.getAttribute("trs_customer").getValue();
    var fetchXml;
    if (customer != null) {
        fetchXml = "<filter type='and'><condition attribute='parentcustomerid' operator='eq' uitype='account' value='" + customer[0].id + "' /></filter>";
        Xrm.Page.getControl("trs_contact").addCustomFilter(fetchXml);
    }
}