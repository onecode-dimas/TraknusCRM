var formstatus = 1;
var statuscode_draftnew = 1;
var statuscode_finalquote = 167630000;
var statuscode_waitingapproval = 167630001;
var statuscode_finalapproved = 167630002;
var statuscode_draftrevision = 167630003;
var intervalId;

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

function DisableField(ControlName) {
    try {
        Xrm.Page.getControl(ControlName).setDisabled(true);
    }
    catch (e) {
        throw new Error("DisableField : " + e.message);
    }
}

function EnableField(ControlName) {
    try {
        Xrm.Page.getControl(ControlName).setDisabled(false);
    }
    catch (e) {
        throw new Error("EnableField : " + e.message);
    }
}

function RequiredControl(ControlName) {
    try {
        Xrm.Page.getAttribute(ControlName).setRequiredLevel("required");
    }
    catch (e) {
        throw new Error("RequiredControl : " + e.message);
    }
}

function UnrequiredControl(ControlName) {
    try {
        Xrm.Page.getAttribute(ControlName).setRequiredLevel("none");
    }
    catch (e) {
        throw new Error("UnrequiredControl : " + e.message);
    }
}

function SetLookupValue(fieldname, value) {
    try {
        var lookup = new Array();
        lookup[0] = new Object();
        lookup[0].id = value.Id;
        lookup[0].name = value.Name;
        lookup[0].entityType = value.LogicalName;

        Xrm.Page.getAttribute(fieldname).setValue(lookup);
        Xrm.Page.getAttribute(fieldname).setSubmitMode("always");
        Xrm.Page.getAttribute(fieldname).fireOnChange();
    }
    catch (e) {
        throw new Error("SetLookupValue : " + e.message);
    }
}

function DisableSection(sectionName, status) {
    try {
        var controlName = Xrm.Page.ui.controls.get();
        for (var i in controlName) {
            var c = controlName[i];
            var cSection = c.getParent().getName();
            if (cSection == sectionName) {
                c.setDisabled(status);
            }
        }
    }
    catch (e) {
        throw new Error("DisableSection : " + e.message);
    }
}

function RemoveButtonsFromSubGrid(subgridControlName) {
    try {
        $('#' + subgridControlName + '_addImageButton').css('display', 'none');
        $('#' + subgridControlName + '_openAssociatedGridViewImageButton').css('display', 'none');
    }
    catch (e) {
        throw new Error("RemoveButtonsFromSubGrid : " + e.message);
    }
}

function ReadOnlyForm() {
    try {
        var subgridsLoaded = false;
        Xrm.Page.ui.controls.get().forEach(function (control, index) {
            if (control.getDisabled && formstatus != 3) {
                control.setDisabled(true);
            }
            else {
                RemoveButtonsFromSubGrid(control);
                subgridsLoaded = true;
            }
        });
        if ($("div[id$='_crmGridTD']").length > 0 && !subgridsLoaded) {
            intervalId = setInterval(function () {
                var subgridsArr = Xrm.Page.getControl(function (control, index) {
                    return control.getControlType() == 'subgrid';
                });
                subgridsArr.forEach(function (control, index) {
                    RemoveButtonsFromSubGrid(control);
                });
            }, 500);
        }
    }
    catch (e) {
        throw new Error("ReadOnlyForm : " + e.message);
    }
}

function DisableAllFields(status) {
    try {
        var ctrlName = Xrm.Page.getControl()
        for (var i in ctrlName) {
            var ctrl = ctrlName[i];
            ctrl.setDisabled(status);
        }
    }
    catch (e) {
        throw new Error("DisableAllFields : " + e.message);
    }
}

function DisableContent() {
    try {
        Xrm.Page.ui.controls.forEach(function (control, index) {
            if (control.getControlType() != "subgrid") {
                if (control.getControlType() == "standard") {
                    if (control._control
                        && control._control.get_innerControl()
                        && control._control.get_innerControl()._element
                        && control._control.get_innerControl()._element.tagName
                        && control._control.get_innerControl()._element.tagName.toLowerCase() === "textarea") {
                        //There's a bug on the setDisabled on IE9 by which when the textarea is disabled using setDisabled, the scroll bar doesn't works, and you can't copy the text either.
                        //So, in this case, we are setting the textarea editable with the submit mode = none
                        control.getAttribute().setSubmitMode("never");
                    }
                    else {
                        control.setDisabled(true);  //disable any other controls normally
                    }
                }
                else
                    control.setDisabled(true);  //disable any other controls normally
            }
        });
    }
    catch (e) {
        throw new Error("DisableContent : " + control.getName() + " - " + control.getControlType() + " - " + e.message);
    }
}

function SetFormDisableByStatusCode() {
    var statuscode = Xrm.Page.getAttribute("statuscode").getValue();
    //if (statuscode == statuscode_finalquote) {
    if (crmForm.FormType == 2) {
        $(document).ready(function () { ReadOnlyForm(); });
    }
}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Publics
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FORM ON LOAD AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function Form_OnLoad() {
    try {
        formstatus = Xrm.Page.ui.getFormType();

        if (formstatus < 2) {
            EnableField("transactioncurrencyid");
            EnableField("trs_discountheader");
            EnableField("trs_discountheaderamount");
            Equipment_OnChange();
        }
        else {
            DisableField("transactioncurrencyid");
            DisableField("trs_discountheader");
            DisableField("trs_discountheaderamount");

            var statuscode = Xrm.Page.getAttribute("statuscode").getValue();
            if (statuscode == statuscode_waitingapproval
                || statuscode == statuscode_finalquote
                || statuscode == statuscode_finalapproved) {
                //RemoveButtonsFromSubGrid("Services");
                //RemoveButtonsFromSubGrid("QuotationCommercialDetail");
                //RemoveButtonsFromSubGrid("SupportingMaterials");
                //RemoveButtonsFromSubGrid("Tools");
            }
        }
    }
    catch (e) {
        alert("Form_OnLoad : " + e.message);
    }
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FORM ON SAVE AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function Form_OnSave() {
    try {
    }
    catch (e) {
        alert("Form_OnSave : " + e.message);
    }
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FIELD EVENT AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function Equipment_OnChange() {
    try {
        Xrm.Page.getAttribute("trs_branch").setValue(null);
        Xrm.Page.getAttribute("trs_site").setValue(null);

        var unit = Xrm.Page.getAttribute("trs_unit").getValue();
        if (unit != null) {
            var unitid = unit[0].id;
            var path = "/new_populationSet?$select=trs_FunctionalLocation&$filter=new_populationId eq guid'" + unitid + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved.results != null && retrieved.results.length > 0) {
                    if (retrieved.results[0].trs_FunctionalLocation != null) {
                        var FuncLocId = retrieved.results[0].trs_FunctionalLocation.Id;
                        path = "/trs_functionallocationSet?$select=trs_Branch,trs_Plant&$filter=trs_functionallocationId eq guid'" + FuncLocId + "'";
                        retrieveReq = RetrieveOData(path);
                        if (retrieveReq.readyState == 4 /* complete */) {
                            retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                            if (retrieved.results != null && retrieved.results.length > 0) {
                                var branch = retrieved.results[0].trs_Branch;
                                if (branch != null)
                                    SetLookupValue("trs_branch", branch);
                                var plant = retrieved.results[0].trs_Plant;
                                if (plant != null)
                                    SetLookupValue("trs_site", plant);
                            }
                        }
                    }
                }
            }
        }
    }
    catch (e) {
        alert("Equipment_OnChange : " + e.message);
    }
}