function onLoad() {
    makeReadOnly();
    ShowAndHideGrid();
}

function ShowAndHideGrid() {
    var isCompt = Xrm.Page.getAttribute("tss_iscompetitor").getValue();
    var result = Xrm.Page.getAttribute("tss_result").getValue();

    var tab_competitorGrid = 'tab_2';
    if (isCompt != null && result != null && isCompt && result) {
        Xrm.Page.ui.tabs.get(tab_competitorGrid).setVisible(true);
    } else {
        Xrm.Page.ui.tabs.get(tab_competitorGrid).setVisible(false);
    }
}
function showAddButton() {
    makeReadOnly();
}
var intervalId = true;
function makeReadOnly() {
    try {
        var result = Xrm.Page.getAttribute("tss_result").getValue();
        var subgridsLoaded = false;
        if (!result && !subgridsLoaded) {
            intervalId = setInterval(function () {
                var subgridsArr = Xrm.Page.getControl(function (control, index) {
                    return control.getControlType() == 'subgrid';
                });
                subgridsArr.forEach(function (control, index) {
                    removeButtonsFromSubGrid(control);
                });
            }, 500);
        }
        else if (result) {
            intervalId = setInterval(function () {
                var subgridsArr = Xrm.Page.getControl(function (control, index) {
                    return control.getControlType() == 'subgrid';
                });
                subgridsArr.forEach(function (control, index) {
                    enableButtonsFromSubGrid(control);
                });
            }, 500);
        }
    }
    catch (e) {
        alert("makeReadOnly() Error: " + e.message);
    }
}

function removeButtonsFromSubGrid(subgridControl) {
    if (intervalId) {
        $('#' + subgridControl.getName() + '_addImageButton').css('display', 'none');
        $('#' + subgridControl.getName() + '_openAssociatedGridViewImageButton').css('display', 'none');
        clearInterval(intervalId);
    }
}

function enableButtonsFromSubGrid(subgridControl) {
    $('#' + subgridControl.getName() + '_addImageButton').css('display', 'inline');
    $('#' + subgridControl.getName() + '_openAssociatedGridViewImageButton').css('display', 'inline');
}

function preventSave(econtext) {

    var eventArgs = econtext.getEventArgs();

    var reqDeliveryDate;
    var re;

    var lookupObject = Xrm.Page.getAttribute("tss_quotationpartheader");
    if (lookupObject != null) {
        var lookUpObjectValue = lookupObject.getValue();
        if ((lookUpObjectValue != null)) {
            var lookupid = lookUpObjectValue[0].id;
        }

        XrmServiceToolkit.Rest.Retrieve(
        lookupid,
            "tss_quotationpartheaderSet",
            null, null,

            function (result) {
                re = result;
            },
            function (error) {
                alert(error.message);
            },
            false);

        reqDeliveryDate = re.tss_requestdeliverydate;
        if (reqDeliveryDate == null) {
            if (eventArgs.getSaveMode() == 1 || eventArgs.getSaveMode() == 2 || eventArgs.getSaveMode() == 59 || eventArgs.getSaveMode() == 80) {
                eventArgs.preventDefault();
                alert("Cannot save the record, You must be fill Request Delivery Date in Quotation Header First!");
            }
        }
    }
}