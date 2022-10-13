function GetAttributeValue(attribute) {
    var result = null;

    if (attribute != null) {
        if (attribute.getValue() != null) {
            result = attribute.getValue();
        }
    }

    return result;
}

function OnLoad() {
    SetFormSLABOP();
}

function SetFormSLABOP() {
    debugger;

    var _contractid = GetAttributeValue(Xrm.Page.getAttribute("contractid"));

    XrmServiceToolkit.Rest.Retrieve(_contractid[0].id, "ContractSet", "ContractTemplateId,contract_template_contracts/Name", 'contract_template_contracts', function (result) {
        var contractTemplateId = result.ContractTemplateId;
        var contract_template_contracts_Name = result.contract_template_contracts.Name;

        if (contract_template_contracts_Name == "Blanket Order Program" || contract_template_contracts_Name == "Service Level Agreement") {
            SetFormSLABOP_SetMandatory();
            SetFormSLABOP_SetPrice();
        }
    }, function (error) {
        Xrm.Utility.alertDialog(error.message);
    }, false);
}

function SetFormSLABOP_SetMandatory() {
    Xrm.Page.getAttribute("totalallotments").setRequiredLevel("none");
}

function SetFormSLABOP_SetPrice() {

}