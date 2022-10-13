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
    SetDefaultForm();
    HideFormSelector();
}

function SetDefaultForm() {
    debugger;

    try {
        var _formname_default = "Information";
        var _formname;
        var _formtype;
        var _contracttemplateid = GetAttributeValue(Xrm.Page.getAttribute("contracttemplateid"));

        if (_contracttemplateid != null) {
            _formname = _contracttemplateid[0].name;
            _formtype = _formname;

            //switch (_formname) {
            //    case "Blanket Order Program":
            //        _formtype = "Blanket Order Program";
            //        break;
            //    case "Service Level Agreement":
            //        _formtype = "Service Level Agreement";
            //        break;
            //    default:
            //        _formtype = "Information";
            //}

            if (Xrm.Page.ui.formSelector.getCurrentItem().getLabel() != _formtype) {
                var _items = Xrm.Page.ui.formSelector.items.get();
                var _items_count = _items.length;
                var _counter = 0;
                var _defaultitem;

                for (var i in _items) {
                    var _item = _items[i];
                    var _itemid = _item.getId();
                    var _itemlabel = _item.getLabel();

                    if (_itemlabel == _formname_default) {
                        _defaultitem = _item;
                    }

                    if (_itemlabel == _formtype) {
                        _item.navigate();
                        location.reload();

                        break;
                    }

                    _counter += 1;
                }

                if (_counter == _items_count && Xrm.Page.ui.formSelector.getCurrentItem().getLabel() != _formname_default) {
                    _defaultitem.navigate();
                    location.reload();
                }
            }
        }
        else {
            SetContractTemplateID(Xrm.Page.ui.formSelector.getCurrentItem().getLabel());
        }
    } catch (e) {
        alert(e.message);
    }
}

function HideFormSelector() {
    var _formselectorcontainer = document.getElementById("formselectorcontainer");

    if (_formselectorcontainer != null) {
        _formselectorcontainer.style.display = "none";
    }
}

function SetContractTemplateID(_itemlabel) {
    var _contracttemplateid = Xrm.Page.getAttribute("contracttemplateid");

    if (_contracttemplateid != null && _itemlabel != null) {
        var _itemid;

        XrmServiceToolkit.Rest.RetrieveMultiple("ContractTemplateSet", "?$select=ContractTemplateId,Name&$filter=Name eq '" + _itemlabel + "'", function (results) {
            for (var i = 0; i < results.length; i++) {
                var contractTemplateId = results[i].ContractTemplateId;
                var name = results[i].Name;

                _contracttemplateid.setValue([{ id: contractTemplateId, name: name, entityType: "contracttemplate" }]);
            }
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, function () {
            //On Complete - Do Something
        }, false);
    }
}

function CustomRule_Activate() {

}

function CustomRule_Hold() {

}

function CustomRule_UnHold() {

}

function CustomRule_Renewal() {

}

function Activate() {

}

function Hold() {

}

function UnHold() {

}

function Renewal() {

}