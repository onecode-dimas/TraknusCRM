//Get Form Type 
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


//Event OnChange Customer

function Customer_onChange() {
    var customerID = Xrm.Page.getAttribute("tss_customer").getValue()[0].id;

    //XrmServiceToolkit.Rest.Retrieve(customerID, "AccountSet", "ParentAccountId", null, function (result) {
    //    Xrm.Page.getAttribute("tss_customergroup").setValue([{ id: result.ParentAccountId.Id, name: result.ParentAccountId.Name, entityType: result.ParentAccountId.LogicalName }]);
    //}, function (error) {
    //    Xrm.Utility.alertDialog(error.message);
    //}, true);

    XrmServiceToolkit.Rest.Retrieve(customerID, "AccountSet", "tss_CustomerParent", null, function (result) {
        var tss_CustomerParent = result.tss_CustomerParent;

        Xrm.Page.getAttribute("tss_customergroup").setValue([{ id: result.tss_CustomerParent.Id, name: result.tss_CustomerParent.Name, entityType: result.tss_CustomerParent.LogicalName }]);
    }, function (error) {
        Xrm.Utility.alertDialog(error.message);
    }, false);
}