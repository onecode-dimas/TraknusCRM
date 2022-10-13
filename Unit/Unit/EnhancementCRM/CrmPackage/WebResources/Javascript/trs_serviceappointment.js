///<reference path="../Script (JScript)/XrmPage-vsdoc.js"/>
///<reference path="../Script (JScript)/XrmServiceToolkit.js"/>
var newform = false;

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

function SetAttributeRequirement(fieldname, requirement) {
    ///<param name="fieldname">String : fieldname that will be set mandatory</param>
    ///<param name="requirement">Int : requirement level of the attribute. 0 : none, 1 : recommended, 2 : required.</param>
    ///<summary>Set field to be mandatory in form</summary>
    try {
        var requirementString = "";
        switch (requirement) {
            case 0:
                requirementString = "none";
                break;
            case 1:
                requirementString = "recommended";
                break;
            case 2:
                requirementString = "required";
                break;
            default:
                requirementString = "none";
                break;
        }
        Xrm.Page.getAttribute(fieldname).setRequiredLevel(requirementString);
    } catch (e) {
        throw new Error("SetAttributeRequirement : " + e.message);
    }
}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Publics
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function SetActType(type) {
    try {
        var path = "/trs_acttypeSet?$select=trs_acttypeId&$filter=trs_name eq '" + type + "'";
        var retrieveReq = RetrieveOData(path);
        if (retrieveReq.readyState == 4 /* complete */) {
            var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
            if (retrieved != null && retrieved.results.length > 0) {
                if (retrieved.results[0].trs_acttypeId != null) {
                    var arr = new Array();
                    arr[0] = new Object();
                    arr[0].id = retrieved.results[0].trs_acttypeId;
                    arr[0].name = type;
                    arr[0].entityType = "trs_acttype";
                    Xrm.Page.getAttribute("trs_acttype").setValue(arr);
                    Xrm.Page.getAttribute("trs_acttype").setSubmitMode("always");
                    Xrm.Page.getAttribute("trs_acttype").fireOnChange();
                }
            }
        }
    }
    catch (e) {
        throw new Error("SetActType : " + e.Message);
    }
}

function SetWorkCenter(name, branchGuid) {
    ///trs_workcenterSet?$select=trs_Plant,trs_workcenter,trs_workcenterId&$filter=trs_Plant/Id eq guid'' and trs_workcenter eq ''
    try {
        var path = "/trs_workcenterSet?$select=trs_workcenter,trs_workcenterId&$filter=trs_Plant/Id eq guid'" + branchGuid + "' and trs_workcenter eq '" + name + "'";
        var retrieveReq = RetrieveOData(path);
        if (retrieveReq.readyState == 4 /* complete */) {
            var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
            if (retrieved != null && retrieved.results.length > 0) {
                if (retrieved.results[0].trs_workcenterId != null) {
                    var arr = new Array();
                    arr[0] = new Object();
                    arr[0].id = retrieved.results[0].trs_workcenterId;
                    arr[0].name = retrieved.results[0].trs_workcenter;
                    arr[0].entityType = "trs_workcenter";
                    Xrm.Page.getAttribute("trs_workcenter").setValue(arr);
                    Xrm.Page.getAttribute("trs_workcenter").setSubmitMode("always");
                    Xrm.Page.getAttribute("trs_workcenter").fireOnChange();
                }
            }
        }
    }
    catch (e) {
        throw new Error("SetWorkCenter : " + e.Message);
    }
}

function SetProfitCenter(branch) {
    try {
        var path = "/trs_profitcenterSet?$select=trs_Name,trs_profitcenter,trs_profitcenterId&$filter=trs_profitcenter eq '" + branch + "'";
        var retrieveReq = RetrieveOData(path);
        if (retrieveReq.readyState == 4 /* complete */) {
            var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
            if (retrieved != null && retrieved.results.length > 0) {
                if (retrieved.results[0].trs_profitcenterId != null) {
                    var arr = new Array();
                    arr[0] = new Object();
                    arr[0].id = retrieved.results[0].trs_profitcenterId;

                    arr[0].name = retrieved.results[0].trs_profitcenter;
                    arr[0].entityType = "trs_profitcenter";
                    Xrm.Page.getAttribute("trs_profitcenter").setValue(arr);
                    Xrm.Page.getAttribute("trs_profitcenter").setSubmitMode("always");
                    Xrm.Page.getAttribute("trs_profitcenter").fireOnChange();
                }
            }
        }
    }
    catch (e) {
        throw new Error("SetProfitCenter : " + e.Message);
    }
}


function SetRCCtr(profitcenter) {
    ///trs_responsiblecostcenterSet?$select=trs_costcenter,trs_responsiblecostcenterId&$filter=trs_ProfitCenter/Id eq guid''
    try {
        var path = "/trs_responsiblecostcenterSet?$select=trs_costcenter,trs_responsiblecostcenterId&$filter=trs_ProfitCenter/Id eq guid'" + profitcenter + "'";
        var retrieveReq = RetrieveOData(path);
        if (retrieveReq.readyState == 4 /* complete */) {
            var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
            if (retrieved != null && retrieved.results.length > 0) {
                if (retrieved.results[0].trs_responsiblecostcenterId != null) {
                    var arr = new Array();
                    arr[0] = new Object();
                    arr[0].id = retrieved.results[0].trs_responsiblecostcenterId;
                    arr[0].name = retrieved.results[0].trs_costcenter;
                    arr[0].entityType = "trs_responsiblecostcenter";
                    Xrm.Page.getAttribute("trs_responsiblecctr").setValue(arr);
                    Xrm.Page.getAttribute("trs_responsiblecctr").setSubmitMode("always");
                    Xrm.Page.getAttribute("trs_responsiblecctr").fireOnChange();
                }
            }
        }
    }
    catch (e) {
        throw new Error("SetProfitCenter : " + e.Message);
    }
}
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FORM ON LOAD AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function Form_OnLoad() {
    try {
        var IsCalculateToRedo = Xrm.Page.getAttribute("trs_iscalculatetoredo").getValue();
        validatingRoleAdmin();

        if (Xrm.Page.ui.getFormType() < 2)
            newform = true;

        if (newform) {
            GetDefaultService();
            RetrieveSRData();
            setEmptyCPOnSite();
            setEmptyFuncloc();

            TrsServiceAppointment.ServiceCall.RetrieveBranchAndPlant(Xrm.Page.getAttribute("ownerid").getValue()[0].id,
                function (branchId,
                    siteId) {
                    SetLookupValue("trs_branch", branchId);
                    SetLookupValue("trs_plant", siteId);
                }, function (e) {
                    alert(e.message);
                });


            // Aded by Erry 16/10/2015, to prevent the making of WO PDI and Delivery for customer name TN
            CheckPDIDeliverytoTraknus();
            CheckUnitStatusforASS();
        }
        else {
            Xrm.Page.ui.controls.get("trs_accind").setDisabled(false);
            Xrm.Page.ui.controls.get("serviceid").setDisabled(true);
            Xrm.Page.ui.controls.get("trs_acttype").setDisabled(false);
            Xrm.Page.ui.controls.get("trs_pmacttype").setDisabled(true);
            Xrm.Page.ui.controls.get("trs_branch").setDisabled(true);
            Xrm.Page.ui.controls.get("trs_workcenter").setDisabled(true);
            Xrm.Page.ui.controls.get("trs_plant").setDisabled(true);
            Xrm.Page.ui.controls.get("trs_profitcenter").setDisabled(true);
            Xrm.Page.ui.controls.get("trs_responsiblecctr").setDisabled(true);
            Xrm.Page.ui.controls.get("trs_workshop").setDisabled(true);
            Xrm.Page.ui.controls.get("resources").setDisabled(false);
            Xrm.Page.ui.controls.get("trs_equipment").setDisabled(true);

            var contactLookup = Xrm.Page.getAttribute("trs_cponsite").getValue();
            if (contactLookup != null) {
                Xrm.Page.ui.controls.get("trs_cponsite").setDisabled(true);
            }

            var statusCode = Xrm.Page.getAttribute("statuscode").getValue();
            var UserTitle_On_WorkflowConfiguration = GetWorkflowIntegrationConfiguration_UserTitle();

            CheckWOIsDispatched();
            FilterLookup();
            PMACTTYPE_OnChange();
            EnableDisable_PartCost();
            EnableDisableFieldPartCost_BasedOn_StatusCode(statusCode);
            DisableFields_For_User_PDH(UserTitle_On_WorkflowConfiguration);
            //Required_PartCost();
            CheckNeedApprovalFullSupplyWO();
            EnableDisable_FullSupplyWO();
            EnableDisable_PartChanges();
            CheckNeedApprovalPartChanges("PartSummary");
        }

        // Added by Erry 28/09/2015, to check open WO for the same equipment
        RetrieveOpenWO();

        var result = TrsServiceAppointment.LocalForm.SetVisibility("trs_redooption", function () {
            var pmacttype = Xrm.Page.getAttribute("trs_pmacttype");
            if ("getValue" in pmacttype) {
                var valueArray = Xrm.Page.getAttribute("trs_pmacttype").getValue();
                if (valueArray === null || valueArray === undefined) return false;
                return valueArray[0].name === "REDO";
            } else {
                return false;
            }
        }, ["trs_pmacttype"]);
        if (result.Error !== null) {
            alert(result.Error.message);
        }
        IsMechanicGradeGreaterThanTaskList_OnChange();

        TrsServiceAppointment.LocalForm.CheckWoRevenueRelatedOnRefreshGrid("Operations");
        TrsServiceAppointment.LocalForm.CheckWoRevenueRelatedOnRefreshGrid("PartSummary");
        TrsServiceAppointment.LocalForm.CheckWoRevenueRelatedOnRefreshGrid("SupportingMaterials");
        TrsServiceAppointment.LocalForm.CheckMechanicGradeCheckingOnRefreshGrid("Operations");

        if (IsCalculateToRedo != null) {
            Xrm.Page.getControl("trs_iscalculatetoredo").setVisible(true);
            Xrm.Page.ui.controls.get("trs_iscalculatetoredo").setDisabled(false);
        }
        else {
            Xrm.Page.getControl("trs_iscalculatetoredo").setVisible(false);
            Xrm.Page.ui.controls.get("trs_iscalculatetoredo").setDisabled(true);
        }

        IsFMC();
        EnableDisable_FunctionalLocation();
    }
    catch (e) {
        alert("Form_OnLoad : " + e.message);
    }
}

function CheckNeedApprovalFullSupplyWO() {
    var MechanicLeader = Xrm.Page.getAttribute("trs_mechanicleader").getValue();

    if (MechanicLeader != null) {
        if (CheckMaxFullSupplyWO(MechanicLeader[0].id) == true)
        {
            Xrm.Page.getAttribute("ittn_needapprovefullsupplywo").setValue(true);
            Xrm.Page.getAttribute("ittn_needapprovefullsupplywo").setSubmitMode("always");
            Xrm.Page.getAttribute("ittn_needapprovefullsupplywo").fireOnChange();
        }
        else {
            Xrm.Page.getAttribute("ittn_needapprovefullsupplywo").setValue(false);
            Xrm.Page.getAttribute("ittn_needapprovefullsupplywo").setSubmitMode("always");
            Xrm.Page.getAttribute("ittn_needapprovefullsupplywo").fireOnChange();
        }
    }
}

function validatingRoleAdmin() {
    try {
        CheckUserRole("#Admin SVC");
    }
    catch (e) {
        alert("validatingRoleAdmin : " + e.message);
    }
}

// This method takes SecurityRole Name and context as parameters
function CheckUserRole(roleName) {
    var userHasRole = false;

    var autoField = Xrm.Page.getAttribute("trs_automatic").getValue();
    if (autoField != null) {
        if (autoField == true) {
            //get the current roles for the user
            var userRoles = Xrm.Page.context.getUserRoles();
            if (userRoles != null) {
                var path = "/RoleSet?$select=Name&$filter=RoleId eq guid'" + userRoles[0] + "'";
                var retrieveReq = RetrieveOData(path);
                if (retrieveReq.readyState == 4 /* complete */) {
                    var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                    if (retrieved != null && retrieved.results.length > 0) {
                        if (retrieved.results[0].Name == roleName) {
                            Xrm.Page.getControl("scheduledstart").setDisabled(true);
                            Xrm.Page.getControl("scheduledend").setDisabled(true);
                        }
                    }
                }
            }
        }
    }
}


function RetrieveSRData() {
    try {
        var SRLookup = Xrm.Page.getAttribute("regardingobjectid").getValue();
        if (SRLookup != null) {
            var path = "/IncidentSet?$select=PrimaryContactId,CustomerId,trs_Unit,Description,trs_PMActType&$filter=IncidentId eq guid'" + SRLookup[0].id + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved.results != null && retrieved.results.length > 0) {
                    var customerid = retrieved.results[0].CustomerId;
                    if (customerid.Id != null) {
                        SetLookupValue("customers", customerid);
                        SetLookupValue("trs_customer", customerid);
                    }
                    var primarycontact = retrieved.results[0].PrimaryContactId;
                    if (primarycontact.Id != null)
                        SetLookupValue("trs_contactperson", primarycontact);
                    var unit = retrieved.results[0].trs_Unit;
                    if (unit.Id != null)
                        SetLookupValue("trs_equipment", unit);
                    ValidatePopulation(unit.Id);
                    var pmacttype = retrieved.results[0].trs_PMActType;
                    if (pmacttype.Id != null) {
                        SetLookupValue("trs_pmacttype", pmacttype);

                        var pmacttypevalue = Xrm.Page.getAttribute("trs_pmacttype").getValue();
                        if (pmacttypevalue[0].name == "DELIVERY") {
                            alert("Please change Functional Location to Customer Functional Location.");
                            Xrm.Page.ui.controls.get("trs_functionallocation").setDisabled(false);
                            Xrm.Page.ui.controls.get("trs_functionallocation").setFocus();
                        }
                    }
                    Xrm.Page.getAttribute("trs_customernote").setValue(retrieved.results[0].Description);
                }
            }
        }
        else {
            throw new Error("undefined");
        }
    }
    catch (e) {
        if (e.message === "undefined")
            alert("RetrieveSRData : Please open this window from SR window");
        else
            alert("RetrieveSRData : " + e.message);
    }
}

function ValidatePopulation(populationID) {
    try {
        if (populationID != null) {
            var path = "/new_populationSet?$select=new_description,new_Model,trs_WorkCenter&$filter=new_populationId eq guid'" + populationID + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved.results != null && retrieved.results.length > 0) {
                    var workCentre = retrieved.results[0].trs_WorkCenter;
                    if (workCentre.Id == null) {
                        alert("Population Work Center can not be Empty, Please Fill !");

                        //Added Wendy 8 Sep 16
                        var attributes = Xrm.Page.data.entity.attributes.get();
                        for (var i in attributes)
                        { attributes[i].setSubmitMode("never"); }
                        Xrm.Page.ui.close();
                    }
                }
            }
        }
        else {
            throw new Error("undefined");
        }
    }
    catch (e) {
        alert("ValidatePopulation : " + e.Message);
    }
}

function CheckWOIsDispatched() {
    try {
        var WOIsDispatched = Xrm.Page.getAttribute("trs_isdispatched").getValue();

        if (WOIsDispatched != null) {
            if (WOIsDispatched == true) {
                Xrm.Page.ui.controls.get("trs_mechanicleader").setDisabled(true);
            }
            else {
                Xrm.Page.ui.controls.get("trs_mechanicleader").setDisabled(false);
            }
        }
    }
    catch (e) {
        alert("CheckWOIsDispatched : " + e.message);
    }
}

function GetDefaultService() {
    try {
        var path = "/ServiceSet?$filter=Name eq 'Service Activity'";
        var retrieveReq = RetrieveOData(path);
        if (retrieveReq.readyState == 4 /* complete */) {
            var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
            if (retrieved.results != null && retrieved.results.length > 0) {
                if (retrieved.results[0].ServiceId != null) {
                    var arr = new Array();
                    arr[0] = new Object();
                    arr[0].id = retrieved.results[0].ServiceId;
                    arr[0].name = retrieved.results[0].Name;
                    arr[0].entityType = "service";
                    Xrm.Page.getAttribute("serviceid").setValue(arr);
                    Xrm.Page.getAttribute("serviceid").setSubmitMode("always");
                    Xrm.Page.getAttribute("serviceid").fireOnChange();
                }
            }
        }
    }
    catch (e) {
        alert("GetDefaultService : " + e.message);
    }
}

function setEmptyCPOnSite() {
    try {
        var CPOnSite = Xrm.Page.getAttribute("trs_cponsite").getValue();
        if (CPOnSite == null) {
            var path = "/ContactSet?$select=Telephone1,ContactId,FullName&$filter=FullName eq 'N/A'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved.results != null && retrieved.results.length > 0) {
                    if (retrieved.results[0].ContactId != null) {
                        var arr = new Array();
                        arr[0] = new Object();
                        arr[0].id = retrieved.results[0].ContactId;
                        arr[0].name = retrieved.results[0].FullName;
                        arr[0].entityType = "contact";
                        Xrm.Page.getAttribute("trs_cponsite").setValue(arr);
                        Xrm.Page.getAttribute("trs_cponsite").setSubmitMode("always");
                        var phone = retrieved.results[0].Telephone1;
                        Xrm.Page.getAttribute("trs_phoneonsite").setValue(phone);
                    }
                }
            }
        }
    }
    catch (e) {
        if (e.Message == "undefined")
            alert("setEmptyCPOnSite : Cannot find Contact with name N/A");
        else
            alert("setEmptyCPOnSite : " + e.Message);
    }
}

function setEmptyFuncloc() {
    try {
        var FuncLoc = Xrm.Page.getAttribute("trs_functionallocation").getValue();
        if (FuncLoc == null) {
            var path = "/trs_functionallocationSet?$select=trs_FunctionalLocation,trs_functionallocationId,trs_name&$filter=trs_name eq 'N/A'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved.results != null && retrieved.results.length > 0) {
                    var arr = new Array();
                    arr[0] = new Object();
                    arr[0].id = retrieved.results[0].trs_functionallocationId;
                    arr[0].name = retrieved.results[0].trs_name;
                    arr[0].entityType = "trs_functionallocation";
                    Xrm.Page.getAttribute("trs_functionallocation").setValue(arr);
                    Xrm.Page.getAttribute("trs_functionallocation").setSubmitMode("always");
                    Xrm.Page.getAttribute("trs_functionallocation").fireOnChange();
                    window.alert("This equipment doesn't have Func. Loc. Please update data equipment.");
                }
            }
        }
    }
    catch (e) {
        if (e.Message == "undefined")
            alert("setEmptyFuncloc : Cannot find Functional Location with name N/A");
        else
            alert("setEmptyFuncloc : " + e.Message);
    }
}

function RetrieveOpenWO() {
    try {
        var UnitLookup = Xrm.Page.getAttribute("trs_equipment").getValue();
        var WONumber = Xrm.Page.getAttribute("trs_crmwonumber").getValue();
        //alert ("WONumber: " + WONumber);
        if (UnitLookup != null) {
            var path = "/ServiceAppointmentSet?$select=trs_crmwonumber&$filter=trs_equipment/Id eq guid'" + UnitLookup[0].id + "' and (StatusCode/Value ne 8 or StatusCode/Value ne 9)";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved.results != null && retrieved.results.length > 0) {
                    var crmWONumber = retrieved.results[0].trs_crmwonumber;
                    //alert ("crmWONumber: " + crmWONumber);
                    if (crmWONumber != WONumber && crmWONumber != null) {
                        window.alert("There's still open WO for the same equipment. Please complete this WO first: " + crmWONumber);
                    }
                }
            }
        }
    }
    catch (e) {
        if (e.Message == "undefined")
            alert("RetrieveOpenWO : undefined");
        else
            alert("RetrieveOpenWO : " + e.Message);
    }
}

function CheckPDIDeliverytoTraknus() {
    try {
        var customersParty = Xrm.Page.getAttribute("customers").getValue();
        var PMActType = Xrm.Page.getAttribute("trs_pmacttype").getValue();
        //alert("customersParty: " + customersParty);
        //alert("PMActType: " + PMActType);
        if (customersParty != null && PMActType != null) {
            var customerName = customersParty[0].name;
            var PMActTypeName = PMActType[0].name;
            //alert("customersParty: " + customerName);
            //alert("PMActType: " + PMActTypeName);
            if (customerName == "TRAKTOR NUSANTARA, PT" && (PMActTypeName == "DELIVERY" || PMActType == "PDI")) {
                window.alert("You can not make PDI/Delivery Work Order for Customer TRAKTOR NUSANTARA");
                Xrm.Page.data.setFormDirty(false);
                Xrm.Page.ui.close();
            }
        }
    }
    catch (e) {
        if (e.Message == "undefined")
            alert("CheckPDIDeliverytoTraknus : undefined");
        else
            alert("CheckPDIDeliverytoTraknus : " + e.Message);
    }
}

function CheckUnitStatusforASS() {
    try {
        var PMActType = Xrm.Page.getAttribute("trs_pmacttype").getValue();
        var unitStatus = null;
        var equipmentLookup = Xrm.Page.getAttribute("trs_equipment").getValue();

        if (equipmentLookup != null) {
            var path = "/new_populationSet?$select=trs_UnitStatus,trs_WorkCenter&$filter=new_populationId eq guid'" + equipmentLookup[0].id + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved != null && retrieved.results.length > 0) {
                    if (retrieved.results[0].trs_UnitStatus != null) {
                        unitStatus = retrieved.results[0].trs_UnitStatus;
                    }
                }
            }
        }

        if (unitStatus != null && PMActType != null) {
            var PMActTypeName = PMActType[0].name;
            //alert("unitStatus " + unitStatus);
            //alert("PMActType: " + PMActTypeName);
            if ((!unitStatus) && (PMActTypeName == "ASS 1" || PMActType == "ASS 2" || PMActType == "ASS 3")) {
                window.alert("You cannot make ASS Work Order for Inventory Unit");
                Xrm.Page.data.setFormDirty(false);
                Xrm.Page.ui.close();
            }
        }
    }
    catch (e) {
        if (e.Message == "undefined")
            alert("CheckUnitStatusforASS : undefined");
        else
            alert("CheckUnitStatusforASS : " + e.Message);
    }
}
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FORM ON SAVE AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function Form_OnSave() {
    try {
        OnSave_AlwaysSubmit();
        OnSave_ClearActualStartDate();
    }
    catch (e) {
        alert("Form_OnSave : " + e.message);
    }
}

function OnSave_ClearActualStartDate() {
    //On Create , Check actualstart and make it's value null
    try {
        if (newform)
            Xrm.Page.getAttribute("actualstart").setValue("");
    }
    catch (e) {
        alert("OnSave_ClearActualStartDate : " + e.message);
    }
}

function OnSave_AlwaysSubmit() {
    try {
        //Customer detail
        Xrm.Page.getAttribute("trs_phone").setSubmitMode("always");
        Xrm.Page.getAttribute("trs_npwp").setSubmitMode("always");
        Xrm.Page.getAttribute("trs_address").setSubmitMode("always");
        //Contact person detail
        Xrm.Page.getAttribute("trs_cpphone").setSubmitMode("always");
        //Contact person onsite detail
        Xrm.Page.getAttribute("trs_phoneonsite").setSubmitMode("always");
        //Equipment detail
        Xrm.Page.getAttribute("new_serialnumber").setSubmitMode("always");
        Xrm.Page.getAttribute("trs_product").setSubmitMode("always");
        Xrm.Page.getAttribute("trs_productmodel").setSubmitMode("always");
        Xrm.Page.getAttribute("new_deliverydate").setSubmitMode("always");
        Xrm.Page.getAttribute("trs_enginenumber").setSubmitMode("always");
        Xrm.Page.getAttribute("trs_chasisnumber").setSubmitMode("always");
        Xrm.Page.getAttribute("trs_hourmeter").setSubmitMode("always");
        //Functional location detail
        Xrm.Page.getAttribute("location").setSubmitMode("always");
        Xrm.Page.getAttribute("trs_customer").setSubmitMode("always");
        Xrm.Page.getAttribute("trs_resources").setSubmitMode("always");
    }
    catch (e) {
        alert("OnSave_AlwaysSubmit : " + e.message);
    }
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FIELD EVENT AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function Customers_OnChange() {
    try {
        Xrm.Page.getAttribute("trs_phone").setValue(null);
        Xrm.Page.getAttribute("trs_npwp").setValue(null);
        Xrm.Page.getAttribute("trs_address").setValue(null);

        var customerLookup = Xrm.Page.getAttribute("customers").getValue();
        if (customerLookup != null) {
            var customerid = customerLookup[0].id;
            var path = "/AccountSet?$select=Address1_Name,new_NPWP,Telephone1,PrimaryContactId&$filter=AccountId eq guid'" + customerid + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                FilterContactLookup();
                FilterContactonSiteLookup();

                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved.results != null && retrieved.results.length > 0) {
                    if (retrieved.results[0].Telephone1 != null)
                        Xrm.Page.getAttribute("trs_phone").setValue(retrieved.results[0].Telephone1);
                    if (retrieved.results[0].new_NPWP != null)
                        Xrm.Page.getAttribute("trs_npwp").setValue(retrieved.results[0].new_NPWP);
                    if (retrieved.results[0].Address1_Name)
                        Xrm.Page.getAttribute("trs_address").setValue(retrieved.results[0].Address1_Name);
                    var primaryContactId = retrieved.results[0].PrimaryContactId;
                    if (primaryContactId.Id != null)
                        SetLookupValue("trs_contactperson", primaryContactId);
                }
            }
        }
    }
    catch (e) {
        alert("Customer_OnChange : " + e.message);
    }
}

function Mechanic_OnChange() {
    try {
        //Added by Santony [9/11/2017] - Field hidden field Resources (enhance advanced find issue)
        var mechanic = Xrm.Page.getAttribute("resources").getValue();
        if (mechanic != null) {
            var concatString = "";
            for (var i = 0; i < mechanic.length; i++) {
                concatString += mechanic[i].name;

                if (i < mechanic.length - 1) {
                    concatString += "; ";
                }
            }

            Xrm.Page.getAttribute("trs_resources").setValue(concatString);
        }
        else {
            Xrm.Page.getAttribute("trs_resources").setValue(null);
        }
    }
    catch (e) {
        alert("Mechanic_OnChange : " + e.message);
    }
}

function ContactPerson_OnChange() {
    try {
        Xrm.Page.getAttribute("trs_cpphone").setValue(null);

        var contactLookup = Xrm.Page.getAttribute("trs_contactperson").getValue();
        if (contactLookup != null) {
            var contactid = contactLookup[0].id;
            var path = "/ContactSet?$select=Telephone1&$filter=ContactId eq guid'" + contactid + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved.results != null && retrieved.results.length > 0) {
                    if (retrieved.results[0].Telephone1 != null)
                        Xrm.Page.getAttribute("trs_cpphone").setValue(retrieved.results[0].Telephone1);
                }
            }
        }
    }
    catch (e) {
        alert("ContactPerson_OnChange : " + e.message);
    }
}

function SetContactPersonOnSite_OnChange() {
    try {
        Xrm.Page.getAttribute("trs_phoneonsite").setValue(null);

        var contactLookup = Xrm.Page.getAttribute("trs_cponsite").getValue();
        if (contactLookup != null) {
            var contactid = contactLookup[0].id;
            var path = "/ContactSet?$select=Telephone1&$filter=ContactId eq guid'" + contactid + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved.results != null && retrieved.results.length > 0) {
                    if (retrieved.results[0].Telephone1 != null)
                        Xrm.Page.getAttribute("trs_phoneonsite").setValue(retrieved.results[0].Telephone1);
                }
            }
        }
    }
    catch (e) {
        alert("SetContactPersonOnSite_OnChange : " + e.message);
    }
}

function Equipment_OnChange() {
    try {


        var UnitStatus;

        Xrm.Page.getAttribute("new_serialnumber").setValue(null);
        Xrm.Page.getAttribute("trs_product").setValue(null);
        Xrm.Page.getAttribute("trs_productmodel").setValue(null);
        Xrm.Page.getAttribute("new_deliverydate").setValue(null);
        Xrm.Page.getAttribute("trs_enginenumber").setValue(null);
        Xrm.Page.getAttribute("trs_chasisnumber").setValue(null);
        Xrm.Page.getAttribute("trs_hourmeter").setValue(null);
        Xrm.Page.getAttribute("trs_oldhm").setValue(null);

        var equipmentLookup = Xrm.Page.getAttribute("trs_equipment").getValue();
        if (equipmentLookup != null) {
            var path = "/new_populationSet?$select=new_DeliveryDate,new_enginenumber,new_Model,new_ProductName,new_SerialNumber,trs_ChasisNumber,trs_FunctionalLocation,new_LatestHourMeter,trs_OldHM,new_StatusInOperation,trs_UnitStatus,trs_WorkCenter&$filter=new_populationId eq guid'" + equipmentLookup[0].id + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved != null && retrieved.results.length > 0) {
                    if (retrieved.results[0].new_SerialNumber != null)
                        Xrm.Page.getAttribute("new_serialnumber").setValue(retrieved.results[0].new_SerialNumber);
                    if (retrieved.results[0].new_ProductName != null)
                        Xrm.Page.getAttribute("trs_product").setValue(retrieved.results[0].new_ProductName);
                    if (retrieved.results[0].new_Model != null)
                        Xrm.Page.getAttribute("trs_productmodel").setValue(retrieved.results[0].new_Model);
                    if (retrieved.results[0].new_DeliveryDate != null)
                        Xrm.Page.getAttribute("new_deliverydate").setValue(new Date(parseInt(retrieved.results[0].new_DeliveryDate.replace("/Date(", "").replace(")/", ""), 10)));
                    if (retrieved.results[0].new_enginenumber != null)
                        Xrm.Page.getAttribute("trs_enginenumber").setValue(retrieved.results[0].new_enginenumber);
                    if (retrieved.results[0].trs_ChasisNumber != null)
                        Xrm.Page.getAttribute("trs_chasisnumber").setValue(retrieved.results[0].trs_ChasisNumber);
                    if (retrieved.results[0].new_LatestHourMeter != null)
                        Xrm.Page.getAttribute("trs_hourmeter").setValue(Number(retrieved.results[0].new_LatestHourMeter));
                    if (retrieved.results[0].trs_OldHM != null)
                        Xrm.Page.getAttribute("trs_oldhm").setValue(Number(retrieved.results[0].trs_OldHM));
                    if (retrieved.results[0].new_StatusInOperation != null)
                        Xrm.Page.getAttribute("trs_statusinoperation").setValue(eval(retrieved.results[0].new_StatusInOperation.Value));
                    if (retrieved.results[0].trs_UnitStatus != null) {
                        if (retrieved.results[0].trs_UnitStatus == false)
                            Xrm.Page.getAttribute("trs_workshop").setValue(true);
                        else
                            Xrm.Page.getAttribute("trs_workshop").setValue(false);
                    }

                    var functionalLocation = retrieved.results[0].trs_FunctionalLocation;
                    if (functionalLocation.Id != null)
                        SetLookupValue("trs_functionallocation", functionalLocation);

                    var formType = Xrm.Page.ui.getFormType();

                    //Added Wendy 8 Sep 16
                    if (formType == 1) {  //CREATE
                        var subWorkCenter = retrieved.results[0].trs_WorkCenter;
                        if (subWorkCenter.Id != null) {
                            SetLookupValue("trs_subworkcenter", subWorkCenter);
                        } else {
                            Xrm.Page.ui.controls.get("trs_subworkcenter").setDisabled(false);
                            Xrm.Page.getAttribute("trs_subworkcenter").setRequiredLevel("required");

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

function Mechanics_OnChange() {
    try {
        FilterMechanicLeaderandSubLeader();
        Mechanic_OnChange();
    }
    catch (e) {
        alert("Mechanics_OnChange : " + e.message);
    }
}

function PMACTTYPE_OnChange() {
    try {
        Xrm.Page.getAttribute("trs_assemblingtype").setRequiredLevel("none");
        Xrm.Page.getControl("trs_assemblingtype").setVisible(false);

        var pma = Xrm.Page.getAttribute("trs_pmacttype").getValue();
        if (pma != null) {
            var path = "/trs_tasklistgroupSet?$select=trs_pmacttype&$filter=trs_tasklistgroupId eq guid'" + pma[0].id + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved != null && retrieved.results.length > 0) {
                    if (retrieved.results[0].trs_pmacttype == '003') {
                        Xrm.Page.getControl("trs_assemblingtype").setVisible(true);
                        Xrm.Page.getAttribute("trs_assemblingtype").setRequiredLevel("required");
                    }
                }
            }
        }
    }
    catch (e) {
        alert("PMACTTYPE_OnChange : " + e.message);
    }
}

function AccIndic_OnChange() {
    try {
        var acc = Xrm.Page.getAttribute("trs_accind").getValue();
        if (acc != null && acc != "undefined") {
            if (acc == 1) {
                SetActType("GENRP1");
                var branch = (Xrm.Page.getAttribute("trs_branch").getValue())[0];
                SetWorkCenter("SERVICE", branch.id);
                SetProfitCenter("SRV-" + branch.name);
                SetRCCtr((Xrm.Page.getAttribute("trs_profitcenter").getValue())[0].id);
                SetAttributeRequirement("trs_estimatedbillingdate", 2);
            }
            else if (acc == 2) {
                SetActType("SERV");
                SetAttributeRequirement("trs_estimatedbillingdate", 0);
            }
            else if (acc == 3) {
                SetActType("WARNTY");
                SetAttributeRequirement("trs_estimatedbillingdate", 0);
            }

            else if (acc == 4) {
                SetActType("SERV");
                SetAttributeRequirement("trs_estimatedbillingdate", 0);
            }
        }
    }
    catch (e) {
        alert("AccIndic_OnChange : " + e.message);
    }
}

function FunctionalLocation_OnChange() {
    try {
        Xrm.Page.getAttribute("location").setValue(null);
        Xrm.Page.getAttribute("trs_branch").setValue(null);
        Xrm.Page.getAttribute("trs_plant").setValue(null);

        var functionallocationLookup = Xrm.Page.getAttribute("trs_functionallocation").getValue();
        if (functionallocationLookup != null && functionallocationLookup.Name != "N/A") {
            var functionallocationid = functionallocationLookup[0].id;
            var path = "/trs_functionallocationSet?$select=trs_Branch,trs_FunctionalAddress,trs_Plant&$filter=trs_functionallocationId eq guid'" + functionallocationid + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved.results != null && retrieved.results.length > 0) {
                    var branch = retrieved.results[0].trs_Branch;
                    if (branch != null)
                        SetLookupValue("trs_branch", branch);
                    var plant = retrieved.results[0].trs_Plant;
                    if (plant != null)
                        SetLookupValue("trs_plant", plant);
                    if (retrieved.results[0].trs_FunctionalAddress != null)
                        Xrm.Page.getAttribute("location").setValue(retrieved.results[0].trs_FunctionalAddress);
                }
            }
        }
        //else {
        //    var userId = Xrm.Page.context.getUserId();
        //    var path = "/SystemUserSet?$select=BusinessUnitId&$filter=SystemUserId eq guid'" + userId + "'";
        //    var retrieveReq = RetrieveOData(path);
        //    if (retrieveReq.readyState == 4 /* complete */) {
        //        var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
        //        if (retrieved.results != null && retrieved.results.length > 0) {
        //            var branch = retrieved.results[0].trs_Branch;
        //            if (branch != null)
        //                SetLookupValue("trs_branch", branch);
        //            var plant = retrieved.results[0].trs_Plant;
        //            if (plant != null)
        //                SetLookupValue("trs_plant", plant);
        //            if (retrieved.results[0].trs_FunctionalAddress != null)
        //                Xrm.Page.getAttribute("location").setValue(retrieved.results[0].trs_FunctionalAddress);
        //        }
        //    }
        //}
    }
    catch (e) {
        alert("FunctionalLocation_OnChange : " + e.message);
    }
}

function Plant_OnChange() {
    try {
        FilterResposibleCostCenter();
    }
    catch (e) {
        alert("Plant_OnChange : " + e.message);
    }
}

function IsMechanicGradeGreaterThanTaskList_OnChange() {
    try {
        var IsMechanicGradeGreaterThanTaskList = Xrm.Page.getAttribute("trs_ismechanicgradegreaterthantasklist").getValue();
        var mechanicName = Xrm.Page.getAttribute("trs_mechanicnamegradebelowtasklist").getValue();


        if (IsMechanicGradeGreaterThanTaskList != null) {
            Xrm.Page.ui.clearFormNotification("MechanicError");
            if (IsMechanicGradeGreaterThanTaskList == false) {
                Xrm.Page.ui.setFormNotification("Grade of mechanic that you are assigned are lower than tasklist grade. Mechanic Related : " + mechanicName, "ERROR", "MechanicError");
            }
        }
    } catch (e) {
        alert("IsMechanicGradeGreaterThanTaskList_OnChange : " + e.message);
    }
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FILTERING LOOKUP AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function FilterLookup() {
    try {
        FilterContactLookup();
        FilterContactonSiteLookup();
        FilterMechanicLeaderandSubLeader();
        FilterResposibleCostCenter();
    }
    catch (e) {
        alert("FilterLookup : " + e.message);
    }
}

function FilterContactLookup() {
    try {
        Xrm.Page.getControl("trs_contactperson").removePreSearch();
        var cust = Xrm.Page.getAttribute("customers").getValue();
        if (cust != null) {
            var cond = "";
            for (var i = 0; i < cust.length; i++) {
                cond += "<condition attribute='parentcustomerid' operator='eq' value='" + cust[i].id + "' />";
            }
            var fetchXml = "<filter type='and'><filter type='or'>" + cond + "</filter></filter>";

            Xrm.Page.getControl("trs_contactperson").addPreSearch(function () {
                Xrm.Page.getControl("trs_contactperson").addCustomFilter(fetchXml);
            });
        }
    }
    catch (e) {
        throw new Error("FilterContactLookup : " + e.message);
    }
}

function FilterContactonSiteLookup() {
    try {
        Xrm.Page.getControl("trs_cponsite").removePreSearch();
        var cust = Xrm.Page.getAttribute("customers").getValue();
        if (cust != null) {
            var cond = "";
            for (var i = 0; i < cust.length; i++) {
                cond += "<condition attribute='parentcustomerid' operator='eq' value='" + cust[i].id + "' />";
            }
            var fetchXml = "<filter type='and'><filter type='and'><condition attribute='trs_customerreference' operator='eq' value='2' /><filter type='or'>" + cond + "</filter></filter></filter>";

            Xrm.Page.getControl("trs_cponsite").addPreSearch(function () {
                Xrm.Page.getControl("trs_cponsite").addCustomFilter(fetchXml);
            });
        }
    }
    catch (e) {
        throw new Error("FilterContactonSiteLookup : " + e.message);
    }
}

function FilterMechanicLeaderandSubLeader() {
    try {
        Xrm.Page.getControl("trs_mechanicleader").removePreSearch();
        Xrm.Page.getControl("trs_mechanicsubleader").removePreSearch();
        var mechanic = Xrm.Page.getAttribute("resources").getValue();
        if (mechanic != null) {
            var cond = "";
            for (var i = 0; i < mechanic.length; i++) {

                cond += "<condition attribute='equipmentid' operator='eq' value='" + mechanic[i].id + "' />";
            }
            fetchXml = "<filter type='and'>" + "<filter type='or'>" + cond + "</filter>" + "</filter>";

            Xrm.Page.getControl("trs_mechanicleader").addPreSearch(function () {
                Xrm.Page.getControl("trs_mechanicleader").addCustomFilter(fetchXml);
            });
            Xrm.Page.getControl("trs_mechanicsubleader").addPreSearch(function () {
                Xrm.Page.getControl("trs_mechanicsubleader").addCustomFilter(fetchXml);
            });
        }
    }
    catch (e) {
        throw new Error("FilterMechanicLeaderandSubLeader : " + e.message);
    }
}

function FilterResposibleCostCenter() {
    try {
        var plant = Xrm.Page.getAttribute("trs_plant").getValue();
        if (plant != null) {
            var path = "/SiteSet?$select=Name&$filter=SiteId eq guid'" + plant[0].id + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved != null && retrieved.results.length > 0) {
                    str = retrieved.results[0].Name;
                    Xrm.Page.getControl("trs_responsiblecctr").addPreSearch(function () {
                        fetchXml = "<filter type='and'><condition attribute='trs_costcenter' operator='like' value='%" + str + "%' /></filter>";
                        Xrm.Page.getControl("trs_responsiblecctr").addCustomFilter(fetchXml);
                    });
                }
            }
        }
    }
    catch (e) {
        throw new Error("FilterResposibleCostCenter : " + e.message);
    }
}

//Namespace Creation
var TrsServiceAppointment = {};
var Fields = Fields || {};
var Config = Config || {};
Config.MaxErrorLength = 200;
Fields.LastError = "trs_lasterror";

//Fallback if doesnt have console
var fallbackConsole = function () {
    if (!('console' in self)) {
        console = {
            log: function (message) { },
            dir: function (obj) { }
        }
    }
}

fallbackConsole();

TrsServiceAppointment.Utility = {
    CalculateDistanceInKM: function distance(lat1, lon1, lat2, lon2) {
        //:::  Passed to function:                                                    :::
        //:::    lat1, lon1 = Latitude and Longitude of point 1 (in decimal degrees)  :::
        //:::    lat2, lon2 = Latitude and Longitude of point 2 (in decimal degrees)  :::
        var radlat1 = Math.PI * lat1 / 180;
        var radlat2 = Math.PI * lat2 / 180;
        var theta = lon1 - lon2;
        var radtheta = Math.PI * theta / 180;
        var dist = Math.sin(radlat1) * Math.sin(radlat2) + Math.cos(radlat1) * Math.cos(radlat2) * Math.cos(radtheta);
        dist = Math.acos(dist);
        dist = dist * 180 / Math.PI;
        dist = dist * 60 * 1.1515;
        dist = dist * 1.609344;//KM
        return dist;
    },
    CutString: function cutString(input, length) {
        if (typeof input !== "string") {
            return {
                Error: new Error("Input should be string"),
                Value: null
            };
        }
        if (typeof length !== "number") {
            return {
                Error: new Error("Length is expected to be number."),
                Value: null
            }
        }
        if (input.length <= length) {
            return {
                Error: null,
                Value: input
            }
        }
        return {
            Error: null,
            Value: input.substr(0, length)
        };
    }
};

//Localform Namespace Creation
TrsServiceAppointment.LocalForm = {
    Watch: function (valueFunction, onChangeFunction, periodMs) {
        var currValue = null;
        setInterval(function () {
            var value = valueFunction();
            if (value === undefined || value === null) return;
            if (currValue !== null && value !== currValue) {
                onChangeFunction();
            }
            currValue = value;
        }, periodMs);
    },
    CheckScriptLoaded: function () {
        if (typeof XrmServiceToolkit === "undefined") {
            return false;
        } else {
            return true;
        }
    },
    OpenWebResourceWithParam: function (webresourceName, parameters) {
        /// <summary>
        /// Set Web Resource Url with parameter
        /// Parameters usage example:
        /// var param = new Array();
        /// param.push({ "attribute": "btpn_accountnumber", "value": accountNumber });
        /// param.push({ "attribute": "btpn_cif", "value": cif });
        /// param.push({ "attribute": "btpn_casenumber", "value": caseNumber });
        /// </summary>
        /// <param name="webresourceName" type="string">Webresource Name</param>
        /// <param name="fieldName" type="string">
        /// A string represents field name
        /// </param>
        /// <param name="parameters" type="Array">
        /// An array that contains an object with attribute and value
        /// e.g.{ "attribute": 'attributename', "value": 'attributevalue' }
        /// </param>
        if (typeof Xrm === "undefined") {
            alert("Xrm is not defined.");
            return;
        }
        var baseUrl = Xrm.Page.context.getClientUrl();

        var paramUri = "";
        var i;
        for (i = 0; i < parameters.length; i++) {
            if (i === 0) {
                paramUri += parameters[i].attribute + "=" + parameters[i].value;
            } else {
                paramUri += "&" + parameters[i].attribute + "=" + parameters[i].value;
            }
        }

        var features = [
        					'height=' + screen.height,
        					'width=' + screen.width,
                            'location=no',
                            'menubar=no',
                            'toolbar=no',
                            'resizeable=1',
                            'left=0',
                            'top=0',
                            'scrollbars=1'
        ].join(',');

        var webresourceurl = baseUrl + "/Webresources/" + webresourceName + "?Data=" + encodeURIComponent(paramUri);
        window.open(webresourceurl, 'windowOpenTab', features);
    },
    OpenDialogWebResourceWithParam: function OpenDialogWebResourceWithParam(webresourceName, parameters, dialogWidth, dialogHeight, callback) {
        /// <summary>
        /// Open Dialog of Web Resource with parameter
        /// Parameters usage example:
        /// var param = [{ "attribute": "btpn_accountnumber", "value": accountNumber },
        ///              { "attribute": "btpn_cif", "value": cif },
        ///              { "attribute": "btpn_casenumber", "value": caseNumber }];
        /// </summary>
        /// <param name="webresourceName" type="string">Webresource Name</param>
        /// <param name="fieldName" type="string">
        /// A string represents field name
        /// </param>
        /// <param name="parameters" type="Array">
        /// An array that contains an object with attribute and value
        /// e.g.{ "attribute": 'attributename', "value": 'attributevalue' }
        /// </param>
        if (typeof Xrm === "undefined") {
            alert("Xrm is not defined.");
            return;
        }
        var baseUrl = Xrm.Page.context.getClientUrl();
        var dialogOption = new Xrm.DialogOption;
        dialogOption.width = dialogWidth;
        dialogOption.height = dialogHeight;

        var paramUri = "";
        var i;
        for (i = 0; i < parameters.length; i++) {
            if (i === 0) {
                paramUri += parameters[i].attribute + "=" + parameters[i].value;
            } else {
                paramUri += "&" + parameters[i].attribute + "=" + parameters[i].value;
            }
        }

        var webresourceurl = baseUrl + "/Webresources/" + webresourceName + "?Data=" + encodeURIComponent(paramUri);
        Xrm.Internal.openDialog(webresourceurl, dialogOption, null, null, callback);
    },
    CheckWoRevenueRelatedOnRefreshGrid: function (gridElementId) {
        try {
            TrsServiceAppointment.LocalForm.AddEventOnRefreshGrid(gridElementId, function () {
                TrsServiceAppointment.ServiceCall.GetWorkOrderRevenueRelatedData(Xrm.Page.data.entity.getId(), function (totalPart, totalSuppMaterial, totalTask, woRevenue) {
                    var compareAttribute = function (attributeName, valueCheck) {
                        var attr = Xrm.Page.getAttribute(attributeName);
                        if (attr !== undefined && attr !== null) {
                            var value = attr.getValue();
                            if (value !== valueCheck) {
                                attr.setSubmitMode("always");
                                attr.setValue(valueCheck);
                                attr.fireOnChange();
                                Xrm.Page.data.setFormDirty(false);
                            }
                        }
                    };
                    compareAttribute("trs_totalpart", Number(totalPart));
                    compareAttribute("trs_totalsupportingmaterial", Number(totalSuppMaterial));
                    compareAttribute("trs_totaltask", Number(totalTask));
                    compareAttribute("trs_worevenue", Number(woRevenue));
                });

                //Required_PartCost();
            });
        } catch (e) {
            alert("TrsServiceAppointment.LocalForm.CheckWoRevenueRelatedOnRefreshGrid: " + e.message);
        }
    },
    AddEventOnRefreshGrid: function (gridElementId, onRefreshFunction) {
        try {
            var elem = document.getElementById(gridElementId);
            if (elem !== null) {
                var ctrl = document.getElementById(gridElementId).control;
                ctrl.add_onRefresh(function () {
                    onRefreshFunction();
                });
            } else {
                setTimeout(function () {
                    TrsServiceAppointment.LocalForm.AddEventOnRefreshGrid(gridElementId, onRefreshFunction);
                }, 1000);
            }
        } catch (e) {
            alert("TrsServiceAppointment.LocalForm.AddEventOnRefreshGrid: " + e.message);
        }
    },
    CheckMechanicGradeCheckingOnRefreshGrid: function (gridElementId) {
        try {
            TrsServiceAppointment.LocalForm.AddEventOnRefreshGrid(gridElementId, function () {
                TrsServiceAppointment.ServiceCall.GetMechanicGradeCheckingRelatedData(Xrm.Page.data.entity.getId(),
                function (trs_IsMechanicGradeGreaterThanTaskList, trs_MechanicNameGradeBelowTaskList) {
                    var compareAttribute = function (attributeName, valueCheck) {
                        var attr = Xrm.Page.getAttribute(attributeName);
                        if (attr !== undefined && attr !== null) {
                            //force set
                            attr.setSubmitMode("never");
                            attr.setValue(valueCheck);
                        }
                    };
                    compareAttribute("trs_ismechanicgradegreaterthantasklist", trs_IsMechanicGradeGreaterThanTaskList);
                    compareAttribute("trs_mechanicnamegradebelowtasklist", trs_MechanicNameGradeBelowTaskList);
                    IsMechanicGradeGreaterThanTaskList_OnChange();
                });
            });
        } catch (e) {
            alert("TrsServiceAppointment.LocalForm.CheckMechanicGradeCheckingOnRefreshGrid: " + e.message);
        }
    }
};

TrsServiceAppointment.LocalForm.SetVisibility = function setVisible(attribute, validation, checkOnChange) {
    ///<summary>Function to set attribute is visible or not
    /// Also can check on attribute changed and reevaluate.
    ///</summary>
    var isNullOrUndefined = function (input) { return input === undefined || input === null; };

    if (typeof attribute !== "string") {
        return {
            Error: new Error("Attribute is required to be string."),
            Value: null
        };
    }
    if (typeof validation !== "function") {
        return {
            Error: new Error("Validation is required to be predicate"),
            Value: null
        };
    }
    if (typeof (validation()) !== "boolean") {
        return {
            Error: new Error("Validation is required to be predicate"),
            Value: null
        };
    }

    if (isNullOrUndefined(checkOnChange)) {
        return {
            Error: new Error("Check On Change is need to be filled. If not required just pass an empty array."),
            Value: null
        };
    }
    //Assume array
    Xrm.Page.getControl(attribute).setVisible(validation());
    var isArray = function (input) { return "length" in input; }

    if (isArray(checkOnChange)) {
        //If specified attribute change, check for this again.
        for (var idx = 0; idx < checkOnChange.length; idx++) {
            Xrm.Page.getAttribute(checkOnChange[idx]).addOnChange(function () { setVisible(attribute, validation, []); });
        }
    }

    return {
        Error: null,
        Value: null
    };
};

TrsServiceAppointment.LocalForm.Events = {
    OnLoad: {
        UpdateUnitLocation: function () {
            if (!TrsServiceAppointment.LocalForm.CheckScriptLoaded()) {
                setTimeout(TrsServiceAppointment.LocalForm.Events.OnLoad.UpdateUnitLocation, 1000);
                return;
            }
            var WoStatusCode = {
                SubTecoByMechanic: 167630003
            };

            //check only trigger this when update.
            var isCreateForm = Xrm.Page.ui.getFormType() === 1;
            if (isCreateForm) return;
            console.log("Start update unit location.");

            var woId = Xrm.Page.data.entity.getId();
            var populationId = Xrm.Page.getAttribute("trs_equipment").getValue()[0].id;
            var branchId = Xrm.Page.getAttribute("trs_branch").getValue()[0].id;
            var plantId = Xrm.Page.getAttribute("trs_plant").getValue()[0].id;
            var customerId = Xrm.Page.getAttribute("customers").getValue()[0].id;
            var submitTecoMtar = TrsServiceAppointment.ServiceCall.ObtainMtar(woId);
            var statusCode = Xrm.Page.getAttribute("statuscode").getValue();
            var isSubTecoByMechanic = statusCode === WoStatusCode.SubTecoByMechanic;


            //logging purposes here.
            var activeObject = {
                WoId: woId,
                PopulationId: populationId,
                BranchId: branchId,
                PlantId: plantId,
                CustomerId: customerId,
                SubmitTecoMtar: submitTecoMtar,
                StatusCode: statusCode,
                ContainMtar: submitTecoMtar.ContainMtar,
                IsSubTecoByMechanic: isSubTecoByMechanic
            };
            console.log("Dumping active object :");
            console.dir(activeObject);

            if (submitTecoMtar.ContainMtar === true && isSubTecoByMechanic === true) {
                //start process
                var toleranceInKm = TrsServiceAppointment.ServiceCall.GetLocationTolerance().LocationTolerance;

                var populationLocation = TrsServiceAppointment.ServiceCall.GetPopulationFunctionalLocation(populationId);

                console.log("Dumping active population location:");
                console.dir(populationLocation);

                var distance = TrsServiceAppointment.Utility.CalculateDistanceInKM(populationLocation.Latitude, populationLocation.Longitude, submitTecoMtar.Latitude, submitTecoMtar.Longitude);
                console.log("Distance from population with longlat from mtar is " + distance + " km. Tolerance on setting is " + toleranceInKm + " KM.");

                if (distance > toleranceInKm) {
                    //ask want update location
                    var confirmationText = "Work Order Mtar Submit Teco detected and outside of specified tolerance (" + toleranceInKm + " KM). Do you want update population functional location?";
                    var yesNo = confirm(confirmationText);
                    if (yesNo === true) {
                        var param = [
                            { "attribute": "crmwonumber", "value": Xrm.Page.getAttribute("trs_crmwonumber").getValue() },
                            { "attribute": "woid", "value": woId },
                            { "attribute": "customername", "value": Xrm.Page.getAttribute("customers").getValue()[0].name },
                            { "attribute": "populationid", "value": populationId },
                            { "attribute": "branchid", "value": branchId },
                            { "attribute": "plantid", "value": plantId },
                            { "attribute": "customerid", "value": customerId }
                        ];
                        TrsServiceAppointment.LocalForm.OpenWebResourceWithParam("trs_wofunclocupdate.htm", param);
                        //console.log("Distance is " + distance + " . Tolerance only " + toleranceInKm + " KM. Updating Functional Location.");
                        //var newFunctionalLocationId = TrsServiceAppointment.ServiceCall.CreateFunctionalLocation(submitTecoMtar.Latitude, submitTecoMtar.Longitude, customerId, branchId, plantId);
                        //if (newFunctionalLocationId === "") {
                        //    alert("Functional Location Creation Failed.");
                        //    return;
                        //}
                        //TrsServiceAppointment.ServiceCall.UpdatePopulationFunctionalLocation(newFunctionalLocationId, populationId);

                        //console.log("Unit functional location updated.");
                        //alert("Unit functional location updated.");
                    }
                }
            }
        },
        CutErrorMessage: function CutErrorMessage() {
            var isNullOrUndefined = function (value) { return value === undefined || value === null; };
            var lastError = Xrm.Page.getAttribute(Fields.LastError);


            //footer_trs_lasterror1_d
            var footerLastError = $("#footer_trs_lasterror1_d");

            if (!isNullOrUndefined(footerLastError)) {
                var lastErrorValue = footerLastError.text();
                var trimmedValue = TrsServiceAppointment.Utility.CutString(lastErrorValue, Config.MaxErrorLength);

                if (trimmedValue.Error === null) {
                    //Success
                    footerLastError.text(trimmedValue.Value);
                }
                else {
                    var errorMessage = "Error occuring in CutErrorOnLoad : Please contact your support.\n";
                    errorMessage += "Technical Info :\n";
                    errorMessage += trimmedValue.Error.message;
                    alert(errorMessage);
                }
            }

        }

    }
}



TrsServiceAppointment.ServiceCall = {
    RetrieveBranchAndPlant: function (userId, successCallback, errorCallback) {
        try {
            var businessUnitId, siteId, branchCode, branchId;

            XrmServiceToolkit.Rest.Retrieve(
                userId,
                "SystemUserSet",
                "BusinessUnitId,SiteId",
                null,
                function (result) {
                    businessUnitId = result.BusinessUnitId;
                    siteId = result.SiteId;
                },
                function (error) {
                    alert(error.message);
                },
                false
            );


            XrmServiceToolkit.Rest.Retrieve(
                businessUnitId.Id,
                "BusinessUnitSet",
                "trs_BranchCode",
                null,
                function (result) {
                    branchCode = result.trs_BranchCode;
                },
                function (error) {
                    alert(error.message);
                },
                false
            );

            //Exception for A000
            if (branchCode === "A000") branchCode = "A001";

            XrmServiceToolkit.Rest.RetrieveMultiple(
                "BusinessUnitSet",
                "?$select=BusinessUnitId,Name&$filter=trs_BranchCode eq '" + branchCode + "'",
                function (results) {
                    for (var i = 0; i < results.length; i++) {
                        var businessUnitId = results[i].BusinessUnitId;
                        var name = results[i].Name;
                        if (name === branchCode) {
                            branchId = {
                                Id: businessUnitId,
                                Name: name,
                                LogicalName: "businessunit"
                            }
                        }
                    }
                },
                function (error) {
                    alert(error.message);
                },
                function () {
                    //On Complete - Do Something
                },
                false
            );

            successCallback(branchId, siteId);

        } catch (e) {
            e.message = "Error has occured within TrsServiceAppointment.ServiceCall.RetrievePlantAndPlant.Technical Details:\r\n" + e.message;
            errorCallback(e);
        }
    },
    GetMechanicGradeCheckingRelatedData: function (workOrderId, successCallback) {
        XrmServiceToolkit.Rest.Retrieve(
            workOrderId,
            "ServiceAppointmentSet",
            "trs_IsMechanicGradeGreaterThanTaskList,trs_MechanicNameGradeBelowTaskList",
            null,
            function (result) {
                var trs_IsMechanicGradeGreaterThanTaskList = result.trs_IsMechanicGradeGreaterThanTaskList;
                var trs_MechanicNameGradeBelowTaskList = result.trs_MechanicNameGradeBelowTaskList;
                successCallback(trs_IsMechanicGradeGreaterThanTaskList, trs_MechanicNameGradeBelowTaskList);
            },
            function (error) {
                console.log(error.message);
            },
            true
        );
    },
    GetWorkOrderRevenueRelatedData: function (workOrderId, successCallback) {
        XrmServiceToolkit.Rest.Retrieve(
            workOrderId,
            "ServiceAppointmentSet",
            "trs_TotalPart,trs_TotalSupportingMaterial,trs_TotalTask,trs_WORevenue",
            null,
            function (result) {
                var trs_TotalPart = result.trs_TotalPart.Value;
                var trs_TotalSupportingMaterial = result.trs_TotalSupportingMaterial.Value;
                var trs_TotalTask = result.trs_TotalTask.Value;
                var trs_WORevenue = result.trs_WORevenue.Value;
                successCallback(trs_TotalPart, trs_TotalSupportingMaterial, trs_TotalTask, trs_WORevenue);
            },
            function (error) {
                console.log(error.message);
            },
            true
        );
    },
    GetPopulationFunctionalLocation: function getPopulationFunctionalLocation(populationId) {
        ///<summary>Get Population Functional Location</summary>
        ///<param name="populationId">Population Id</param>
        var defaultReturn = {
            Name: "",
            Latitude: 0,
            Longitude: 0,
            FunctionalLocationId: ""
        }

        var functionalLocationId = "";
        XrmServiceToolkit.Rest.Retrieve(
            populationId,
            "new_populationSet",
            "trs_FunctionalLocation",
            null,
            function (result) {
                var trs_FunctionalLocation = result.trs_FunctionalLocation;
                functionalLocationId = trs_FunctionalLocation.Id;
            },
            function (error) {
                alert(error.message);
            },
            false
        );

        XrmServiceToolkit.Rest.Retrieve(
            functionalLocationId,
            "trs_functionallocationSet",
            "trs_functionallatitude,trs_functionallocationId,trs_functionallongitude,trs_name",
            null,
            function (result) {
                var trs_functionallatitude = result.trs_functionallatitude;
                var trs_functionallocationId = result.trs_functionallocationId;
                var trs_functionallongitude = result.trs_functionallongitude;
                var trs_name = result.trs_name;
                defaultReturn.FunctionalLocationId = trs_functionallocationId;
                defaultReturn.Latitude = trs_functionallatitude;
                defaultReturn.Longitude = trs_functionallongitude;
                defaultReturn.Name = trs_name;
            },
            function (error) {
                alert(error.message);
            },
            false
        );

        return defaultReturn;
    },
    ObtainMtar: function obtainMtar(woId) {
        ///<summary>Obtain Mtar Data only for Submit Teco</summary>
        ///<param name="woId">Wo Id</param>
        var defaultReturn = {
            ContainMtar: false,
            Latitude: 0,
            Longitude: 0,
            MtarId: null,
            WorkOrderId: null
        };
        XrmServiceToolkit.Rest.RetrieveMultiple(
            "trs_mtarSet",
            "?$select=trs_latitude,trs_longitude,trs_mtarId,trs_mtarstatus,trs_WorkOrder&$filter=trs_mtarstatus/Value eq 167630011 and trs_WorkOrder/Id eq (guid'" + woId + "')&$top=1&$orderby=CreatedOn desc",
            function (results) {
                for (var i = 0; i < results.length; i++) {
                    var trs_latitude = results[i].trs_latitude;
                    var trs_longitude = results[i].trs_longitude;
                    var trs_mtarId = results[i].trs_mtarId;
                    var trs_mtarstatus = results[i].trs_mtarstatus;
                    var trs_WorkOrder = results[i].trs_WorkOrder;
                    defaultReturn.ContainMtar = true;
                    defaultReturn.Latitude = trs_latitude;
                    defaultReturn.Longitude = trs_longitude;
                    defaultReturn.WorkOrderId = trs_WorkOrder.Id;
                    defaultReturn.MtarId = trs_mtarId;
                }
            },
            function (error) {
                console.log(error.message);
                alert(error.message);
            },
            function () {
                //On Complete - Do Something
            },
            false
        );
        return defaultReturn;
    },
    CreateFunctionalLocation: function createFunctionalLocation(latitude, longitude, customerId, branchId, plantId) {
        ///<summary>Create new functional location</summary>
        ///<param name="latitude">Latitude</param>
        ///<param name="longitude">Longitude</param>
        ///<param name="customerId">Wo Customer Id</param>
        ///<param name="branchId">Wo Branch Id (SAP)</param>
        ///<param name="plantId">Wo Plant Id (SAP)</param>

        var entity = {};
        entity.trs_Customer = {
            Id: customerId,
            LogicalName: "account"
        };
        entity.trs_Branch = {
            Id: branchId,
            LogicalName: "businessunit"
        };
        entity.trs_Plant = {
            Id: plantId,
            LogicalName: "site"
        };
        entity.trs_functionallatitude = latitude;
        entity.trs_functionallongitude = longitude;
        var entityId = "";
        XrmServiceToolkit.Rest.Create(
            entity,
            "trs_functionallocationSet",
            function (result) {
                var newEntityId = result.trs_functionallocationId;
                entityId = newEntityId;
            },
            function (error) {
                alert(error.message);
            },
            false
        );
        return entityId;
    },
    UpdatePopulationFunctionalLocation: function UpdatePopulationFunctionalLocation(functionalLocationId, populationId) {
        ///<summary>Update Population id with new Functional Location</summary>
        ///<param name="functionalLocationId">Functional Location Id that will be set to Population Location</param>
        ///<param name="populationid">Population id</param>
        var entity = {};
        entity.trs_FunctionalLocation = {
            Id: functionalLocationId,
            LogicalName: "trs_functionallocation"
        };

        XrmServiceToolkit.Rest.Update(
            populationId,
            entity,
            "new_populationSet",
            function () {
                //Success - No Return Data - Do Something
            },
            function (error) {
                alert(error.message);
            },
            false
        );
    },
    GetLocationTolerance: function getLocationTolerance() {
        ///<summary>Get Location Tolerance from Workflow Integration Configuration, only take from TRS Configuration.</summary>
        var returnObject = {
            LocationTolerance: 999,
            WorkflowConfigurationId: ""
        }

        XrmServiceToolkit.Rest.RetrieveMultiple(
            "trs_workflowconfigurationSet",
            "?$select=trs_LocationTolerance,trs_workflowconfigurationId&$filter=trs_GeneralConfig eq 'TRS'",
            function (results) {
                for (var i = 0; i < results.length; i++) {
                    var trs_LocationTolerance = results[i].trs_LocationTolerance;
                    var trs_workflowconfigurationId = results[i].trs_workflowconfigurationId;
                    returnObject.LocationTolerance = trs_LocationTolerance;
                    returnObject.WorkflowConfigurationId = trs_workflowconfigurationId.Id;
                    break;
                }
            },
            function (error) {
                alert(error.message);
            },
            function () {
                //On Complete - Do Something
            },
            false
        );

        return returnObject;
    }
};

//Namespace setup
var Process = Process || {};
(function () {
    function GetWorkflowId(name) {
        var workflowId;

        XrmServiceToolkit.Rest.RetrieveMultiple(
            "WorkflowSet",
            "?$select=WorkflowId&$filter=Name eq '" + encodeURI(name) + "' and StateCode/Value eq 1 and  ParentWorkflowId eq null",
            function (results) {
                for (var i = 0; i < results.length; i++) {
                    workflowId = results[i].WorkflowId;
                }
            },
            function (error) {
                alert(error.message);
            },
            function () {
                //On Complete - Do Something
            },
            false
        );
        return workflowId;
    }


    function CallWorkflowfunction(workflowId, recordId, successCallback, errorCallback, url) {
        if (url == null) {
            url = Xrm.Page.context.getClientUrl();
        }

        var request = "<s:Envelope xmlns:s='http://schemas.xmlsoap.org/soap/envelope/'>" +
              "<s:Body>" +
                "<Execute xmlns='http://schemas.microsoft.com/xrm/2011/Contracts/Services' xmlns:i='http://www.w3.org/2001/XMLSchema-instance'>" +
                  "<request i:type='b:ExecuteWorkflowRequest' xmlns:a='http://schemas.microsoft.com/xrm/2011/Contracts' xmlns:b='http://schemas.microsoft.com/crm/2011/Contracts'>" +
                    "<a:Parameters xmlns:c='http://schemas.datacontract.org/2004/07/System.Collections.Generic'>" +
                      "<a:KeyValuePairOfstringanyType>" +
                        "<c:key>EntityId</c:key>" +
                        "<c:value i:type='d:guid' xmlns:d='http://schemas.microsoft.com/2003/10/Serialization/'>" + recordId + "</c:value>" +
                      "</a:KeyValuePairOfstringanyType>" +
                      "<a:KeyValuePairOfstringanyType>" +
                        "<c:key>WorkflowId</c:key>" +
                        "<c:value i:type='d:guid' xmlns:d='http://schemas.microsoft.com/2003/10/Serialization/'>" + workflowId + "</c:value>" +
                      "</a:KeyValuePairOfstringanyType>" +
                    "</a:Parameters>" +
                    "<a:RequestId i:nil='true' />" +
                    "<a:RequestName>ExecuteWorkflow</a:RequestName>" +
                  "</request>" +
                "</Execute>" +
              "</s:Body>" +
            "</s:Envelope>";

        var req = new XMLHttpRequest();
        req.open("POST", url + "/XRMServices/2011/Organization.svc/web", true);

        req.setRequestHeader("Accept", "application/xml, text/xml, */*");
        req.setRequestHeader("Content-Type", "text/xml; charset=utf-8");
        req.setRequestHeader("SOAPAction", "http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Execute");
        req.onreadystatechange = function () {
            if (req.readyState === 4) {
                console.log(req);
                console.log($.parseXML(req.response));
                if (req.status === 200) {
                    if (successCallback) {
                        successCallback();
                    }
                }
                else {
                    var faultstring = req.responseXML.getElementsByTagName("faultstring")[0].textContent;
                    if (errorCallback) {
                        errorCallback(new Error(faultstring));
                    }
                }
            }
        };

        req.send(request);
    }

    Process.GetWorkflowId = GetWorkflowId;
    Process.ExecuteWorkflow = CallWorkflowfunction;
})();


function RunWorkflowWORecommendation() {
    var _return = window.confirm('Do you want convert this to Service Requisitions and Quotation ?');
    debugger;

    if (_return) {
        var workflowName = "Create SR from WO Recommendation";
        //var workflowId = Process.GetWorkflowId(workflowName);
        var workflowId = "b32b719c-3118-4bd6-90ed-4db93cb7fcb5";

        var recordId = Xrm.Page.data.entity.getId();
        Process.ExecuteWorkflow(workflowId, recordId, function () {
            //sukses ngapain
            alert("Convert SR and Quotation Success");
            //get SR from WO
            var SR = Xrm.Page.getAttribute("regardingobjectid").getValue();
            var SRId = SR[0].id;


            //get child SR from SR that last created
            var incidentId = "";
            XrmServiceToolkit.Rest.RetrieveMultiple(
    "IncidentSet",
    "?$select=IncidentId&$filter=ParentCaseId/Id eq (guid'" + SRId + "')&$top=1&$orderby=CreatedOn desc",
                function (results) {
                    for (var i = 0; i < results.length; i++) {
                        incidentId = results[i].IncidentId;
                    }
                },
                function (error) {
                    alert(error.message);
                },
                function () {
                    Xrm.Utility.openEntityForm("incident", incidentId);
                },
                true
            );

        }, function (err) {
            //error ngapain
            alert("Failed to run workflow. Technical details :\r\n" + err.message);
        });
    }
}

function IsFMC() {
    var IsFMC = Xrm.Page.getAttribute("trs_fullmaintenancecontract").getValue();
    var AvailableBay = Xrm.Page.getAttribute("trs_availablebay").getValue();
    var IsWorkshop = Xrm.Page.getAttribute("trs_workshop").getValue();

    if (IsFMC != null && IsFMC == true && IsWorkshop != null && IsWorkshop == true) {
        if (AvailableBay == null) {
            //Xrm.Page.getControl("trs_availablebay").setVisible(true);
            //Xrm.Page.ui.controls.get("trs_availablebay").setDisabled(false);

            FilterMatrixBay();
        }
        else {
            Xrm.Page.getControl("trs_availablebay").setVisible(true);
            Xrm.Page.ui.controls.get("trs_availablebay").setDisabled(true);
        }
    }
    else {
        DisableAvailableBay();
    }
}

function DisableAvailableBay() {
    Xrm.Page.getControl("trs_availablebay").setVisible(false);
    Xrm.Page.ui.controls.get("trs_availablebay").setDisabled(true);
}

function FilterMatrixBay() {
    var customerLookup = Xrm.Page.getAttribute("customers").getValue();
    var functionallocationLookup = Xrm.Page.getAttribute("trs_functionallocation").getValue();
    var ContractId, MatrixBayId;
    var IsContractActive = false;
    var Today = new Date();

    if (customerLookup != null && functionallocationLookup != null) {
        var customerid = customerLookup[0].id;
        var functionallocationid = functionallocationLookup[0].id;

        XrmServiceToolkit.Rest.RetrieveMultiple(
            "trs_matrixbaySet",
            "?$select=trs_Contract,trs_matrixbayId&$filter=trs_Customer/Id eq (guid'" + customerid + "') and trs_FunctionalLocation/Id eq (guid'" + functionallocationid + "')",
            function (results) {
                if (results.length > 0) {
                    for (var i = 0; i < results.length; i++) {
                        var trs_matrixbayId = results[i].trs_matrixbayId;
                        var trs_Contract = results[i].trs_Contract;
                        MatrixBayId = trs_matrixbayId;
                        ContractId = trs_Contract.Id;

                        if (MatrixBayId != null && ContractId != null) {
                            XrmServiceToolkit.Rest.Retrieve(
                                ContractId,
                                "ContractSet",
                                "ActiveOn,ExpiresOn,StatusCode",
                                null,
                                function (result) {
                                    var activeOn = result.ActiveOn;
                                    var expiresOn = result.ExpiresOn;
                                    var statusCode = result.StatusCode;

                                    if (activeOn <= Today && expiresOn >= Today && statusCode.Value == 3)
                                        IsContractActive = true;

                                    if (IsContractActive == true) {
                                        XrmServiceToolkit.Rest.RetrieveMultiple(
                                            "trs_matrixbaylineSet",
                                            "?$select=trs_matrixbaylineId,trs_name&$expand=trs_trs_matrixbay_trs_matrixbayline&$filter=trs_trs_matrixbay_trs_matrixbayline/trs_matrixbayId eq (guid'" + MatrixBayId + "')",
                                            function (results) {
                                                for (var i = 0; i < results.length; i++) {
                                                    var trs_matrixbaylineId = results[i].trs_matrixbaylineId;
                                                    var trs_name = results[i].trs_name;

                                                    FilterMatrixBayLookup(MatrixBayId, results.length);
                                                }
                                            },
                                            function (error) {
                                                Xrm.Utility.alertDialog(error.message);
                                            },
                                            function () {
                                                //On Complete - Do Something
                                            },
                                            false
                                        );
                                    }
                                    else {
                                        DisableAvailableBay();
                                    }
                                },
                                function (error) {
                                    Xrm.Utility.alertDialog(error.message);
                                },
                                false
                            );
                        }
                        else {
                            DisableAvailableBay();
                        }
                    }
                }
                else {
                    DisableAvailableBay();
                }
            },
            function (error) {
                Xrm.Utility.alertDialog(error.message);
            },
            function () {
                //On Complete - Do Something
            },
            false
        );
    }
}

function FilterMatrixBayLookup(MatrixBayId, count) {
    try {
        Xrm.Page.getControl("trs_availablebay").removePreSearch();
        if (MatrixBayId != null) {
            var cond = "";
            for (var i = 0; i < count; i++) {
                cond += "<condition attribute='trs_matrixbaylinesid' operator='eq' value='" + MatrixBayId + "' />";
            }
            var fetchXml = "<filter type='and'><filter type='or'>" + cond + "</filter></filter>";

            Xrm.Page.getControl("trs_availablebay").addPreSearch(function () {
                Xrm.Page.getControl("trs_availablebay").addCustomFilter(fetchXml);
            });
        }
    }
    catch (e) {
        throw new Error("FilterMatrixBayLookup : " + e.message);
    }
}

function EnableDisable_FunctionalLocation() {
    var statusCode = Xrm.Page.getAttribute("statuscode").getValue();

    if (statusCode == 1 || statusCode == 2) {
        Xrm.Page.ui.controls.get("trs_functionallocation").setDisabled(false);
    }
    else {
        Xrm.Page.ui.controls.get("trs_functionallocation").setDisabled(true);
    }
}

function EnableDisable_PartCost() {
    var flag = false;

    var WOId = Xrm.Page.data.entity.getId();
    var UserID = Xrm.Page.context.getUserId();

    XrmServiceToolkit.Rest.RetrieveMultiple(
    "new_workorderapprovallistSet",
    "$select=*&$filter=new_WorkOrder/Id eq (guid'" + WOId + "') and new_Approver/Id eq (guid'" + UserID + "')",
    function (results) {
        if (results.length > 0) {
            flag = true;
        }
    },
    function (error) {
        alert('Retrieve WO Approval List: ' + error.message);
    },
    function onComplete() {
        //alert(" records should have been retrieved.");
    },
    false
    );

    if (flag) {
        Xrm.Page.getControl("new_approvecostwo").setVisible(true);
        EnableDisableField("new_approvecostwo", false);
    }
    else {
        Xrm.Page.getControl("new_approvecostwo").setVisible(false);
        EnableDisableField("new_approvecostwo", true);
    }
}

function EnableDisable_PartChanges() {
    var flag = false;

    var WOId = Xrm.Page.data.entity.getId();
    var UserID = Xrm.Page.context.getUserId();
    var NeedApprovePartChanges = Xrm.Page.getAttribute("ittn_needapprovepartchanges").getValue();

    XrmServiceToolkit.Rest.RetrieveMultiple(
    "ittn_workorderapprovallistpartchangesSet",
    "$select=*&$filter=ittn_WorkOrder/Id eq (guid'" + WOId + "') and ittn_Approver/Id eq (guid'" + UserID + "')",
    function (results) {
        if (results.length > 0) {
            flag = true;
        }
    },
    function (error) {
        alert('Retrieve WO Approval List Part Changes: ' + error.message);
    },
    function onComplete() {
        //alert(" records should have been retrieved.");
    },
    false
    );

    if (flag && NeedApprovePartChanges != null && NeedApprovePartChanges == true) {
        Xrm.Page.getControl("ittn_approvepartchanges").setVisible(true);
        EnableDisableField("ittn_approvepartchanges", false);
    }
    else {
        Xrm.Page.getControl("ittn_approvepartchanges").setVisible(false);
        EnableDisableField("ittn_approvepartchanges", true);
    }
}

function EnableDisable_FullSupplyWO() {
    var flag = false;

    var WOId = Xrm.Page.data.entity.getId();
    var UserID = Xrm.Page.context.getUserId();
    var NeedApproveFullSupplyWO = Xrm.Page.getAttribute("ittn_needapprovefullsupplywo").getValue();

    XrmServiceToolkit.Rest.RetrieveMultiple(
    "ittn_workorderapprovallistfullsupplywoSet",
    "$select=*&$filter=ittn_WorkOrderId/Id eq (guid'" + WOId + "') and ittn_Approver/Id eq (guid'" + UserID + "')",
    function (results) {
        if (results.length > 0) {
            flag = true;
        }
    },
    function (error) {
        alert('Retrieve WO Approval List Full Supply WO: ' + error.message);
    },
    function onComplete() {
        //alert(" records should have been retrieved.");
    },
    false
    );

    if (flag && NeedApproveFullSupplyWO != null && NeedApproveFullSupplyWO == true) {
        Xrm.Page.getControl("ittn_approvefullsupplywo").setVisible(true);
        EnableDisableField("ittn_approvefullsupplywo", false);
    }
    else {
        Xrm.Page.getControl("ittn_approvefullsupplywo").setVisible(false);
        EnableDisableField("ittn_approvefullsupplywo", true);
    }
}

function EnableDisableFieldPartCost_BasedOn_StatusCode(statusCode) {
    if (statusCode >= 2) { //WO Released keatas
        EnableDisableField("new_partcost", true);
    }
    else {
        EnableDisableField("new_partcost", false);
    }
}

function DisableFields_For_User_PDH(usertitleconfig) {
    var UserID = Xrm.Page.context.getUserId();
    var UserTitle;

    XrmServiceToolkit.Rest.Retrieve(
        UserID,
        "SystemUserSet",
        "Title",
        null,
        function (result) {
            UserTitle = result.Title;
        },
        function (error) {
            alert(error.message);
        },
        false
    );

    if (UserTitle == usertitleconfig) {
        EnableDisableField("trs_cpphone", true)
        EnableDisableField("trs_lasthourmeter", true)
        EnableDisableField("trs_statusinoperation", true)
        EnableDisableField("trs_subworkcenter", true)
        EnableDisableField("trs_functionallocation", true)
        EnableDisableField("description", true)
        EnableDisableField("trs_ponumber", true)
        EnableDisableField("trs_accind", true)
        EnableDisableField("trs_acttype", true)
        EnableDisableField("resources", true)
        EnableDisableField("trs_mechanicleader", true)
        EnableDisableField("trs_mechanicsubleader", true)
        EnableDisableField("scheduledstart", true)
        EnableDisableField("scheduledend", true)
        EnableDisableField("trs_estimatedbillingdate", true)
        EnableDisableField("trs_customernote", true)
        EnableDisableField("trs_inspectorcomments", true)
        EnableDisableField("trs_inspectorsuggestion", true)
        EnableDisableField("trs_customersatisfaction", true)
        EnableDisableField("trs_customercomments", true)
        EnableDisableField("regardingobjectid", true)
        EnableDisableField("trs_cancelreason", true)
        EnableDisableField("trs_documentdate", true)
        EnableDisableField("trs_bastdate", true)
    }
}

function EnableDisableField(fieldname, status) {
    Xrm.Page.ui.controls.get(fieldname).setDisabled(status);
}

function GetWorkflowIntegrationConfiguration_UserTitle() {
    var resultValue;

    console.log("Calling GetWorkflowIntegrationConfiguration_UserTitle.");

    XrmServiceToolkit.Rest.RetrieveMultiple(
        "trs_workflowconfigurationSet",
        "?$select=trs_UserTitle&$filter=trs_GeneralConfig eq 'TRS'",
        function (results) {
            console.log("Query Workflow Configuration Set Result Obtained.");
            console.dir(results);

            for (var i = 0; i < results.length; i++) {
                resultValue = results[i].trs_UserTitle;
                break;
            }
        },
        function (error) {
            alert(error.message);
        },
        function () {
            //On Complete - Do Something
        },
        false
    );
    return resultValue;
}

function TotalPart_OnChange() {
    try {
        var TotalPart = Xrm.Page.data.entity.attributes.get("trs_totalpart").getValue();
        if (TotalPart != null || TotalPart > 0) {
            SetAttributeRequirement("new_partcost", 2);
        }
        else {
            SetAttributeRequirement("new_partcost", 0);
        }
    }
    catch (e) {
        throw new Error("TotalPart_OnChange : " + e.message);
    }
}

function GetTotalPartsSummaryRecords(WorkOrderId) {
    var total = 0;

    XrmServiceToolkit.Rest.RetrieveMultiple(
        "trs_workorderpartssummarySet",
        "?$select=trs_workorderpartssummaryId&$filter=trs_workorder/Id eq (guid'" + WorkOrderId + "')",
        function (results) {
            total = results.length;
        },
        function (error) {
            Xrm.Utility.alertDialog(error.message);
        },
        function () {
            //On Complete - Do Something
        },
        false
    );
    return total;
}

function Required_PartCost() {
    if (GetTotalPartsSummaryRecords(Xrm.Page.data.entity.getId()) > 0) {
        SetAttributeRequirement("new_partcost", 2);
    }
    else {
        SetAttributeRequirement("new_partcost", 0);
    }
}

function CheckMaxFullSupplyWO(MechanicLeader) {
    var IsMaxFullSupplyWO = false;
    var TotalMaxFullSupplyWO = GetWorkflowIntegrationConfiguration_MaxFullSupplyWO();
    var count = 0;
    var recordId = Xrm.Page.data.entity.getId();

    XrmServiceToolkit.Rest.RetrieveMultiple(
        "ServiceAppointmentSet",
        "?$select=ActivityId&$filter=StatusCode/Value ne 8 and trs_MechanicLeader/Id eq (guid'" + MechanicLeader + "')",// and ActivityId ne (guid'" + recordId + "')",
        function (results) {
            for (var i = 0; i < results.length; i++) {
                var activityId = results[i].ActivityId;

                XrmServiceToolkit.Rest.RetrieveMultiple(
                    "trs_workorderpartssummarySet",
                    "?$select=trs_workorderpartssummaryId&$filter=trs_workorder/Id eq (guid'" + activityId + "') and trs_ConfirmSupply eq true",
                    function (results) {
                        for (var i = 0; i < results.length; i++) {
                            var trs_workorderpartssummaryId = results[i].trs_workorderpartssummaryId;
                            count += 1;
                        }
                    },
                    function (error) {
                        alert(error.message);
                    },
                    function () {
                        //On Complete - Do Something
                    },
                    false
                );
            }
        },
        function (error) {
            alert(error.message);
        },
        function () {
            //On Complete - Do Something
        },
        false
    );

    if (count > TotalMaxFullSupplyWO) {
        IsMaxFullSupplyWO = true;
    }

    return IsMaxFullSupplyWO;
}

function GetWorkflowIntegrationConfiguration_MaxFullSupplyWO() {
    var resultValue;

    console.log("Calling GetWorkflowIntegrationConfiguration_MaxFullSupplyWO.");

    XrmServiceToolkit.Rest.RetrieveMultiple(
        "trs_workflowconfigurationSet",
        "?$select=ittn_MaxFullSupplyWO&$filter=trs_GeneralConfig eq 'TRS'",
        function (results) {
            for (var i = 0; i < results.length; i++) {
                resultValue = results[i].ittn_MaxFullSupplyWO;
            }
        },
        function (error) {
            alert(error.message);
        },
        function () {
            //On Complete - Do Something
        },
        false
    );

    return resultValue;
}

function CheckNeedApprovalPartChanges(gridElementId) {
    try {
        TrsServiceAppointment.LocalForm.AddEventOnRefreshGrid(gridElementId, function () {
            GetNeedApprovalPartChangesRelatedData(Xrm.Page.data.entity.getId(), function (NeedApprovePartChanges) {
                var compareAttribute = function (attributeName, valueCheck) {
                    var attr = Xrm.Page.getAttribute(attributeName);
                    if (attr !== undefined && attr !== null) {
                        var value = attr.getValue();
                        if (value !== valueCheck) {
                            attr.setValue(valueCheck);
                            attr.setSubmitMode("always");
                            attr.fireOnChange();
                            Xrm.Page.data.setFormDirty(false);
                            Xrm.Page.data.entity.save();
                        }
                    }
                };
                compareAttribute("ittn_needapprovepartchanges", NeedApprovePartChanges);
            });
        });
    } catch (e) {
        alert("CheckNeedApprovalPartChanges: " + e.message);
    }
}

function GetNeedApprovalPartChangesRelatedData(workOrderId, successCallback) {
    XrmServiceToolkit.Rest.Retrieve(
           workOrderId,
           "ServiceAppointmentSet",
           "ittn_NeedApprovePartChanges",
           null,
           function (result) {
               var ittn_NeedApprovePartChanges = result.ittn_NeedApprovePartChanges;
               successCallback(ittn_NeedApprovePartChanges);
           },
           function (error) {
               console.log(error.message);
           },
           true
       );
}