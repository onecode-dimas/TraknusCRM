function ValidatePopulation(populationID) {
    try {

        if (populationID != null) {
            var path = "/new_populationSet?$select=new_SerialNumber,trs_WorkCenter,trs_businessunit_new_population_WorkCenter/Name&$expand=trs_businessunit_new_population_WorkCenter&$filter=new_populationId eq guid'" + populationID + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                //alert (retrieved.results);
                //alert (retrieved.results.length);
                if (retrieved.results != null && retrieved.results.length > 0) {
                    var workCentre = retrieved.results[0].trs_WorkCenter;
                    //alert ("workCentre: " + workCentre );                 
                }
            }
        }
        return workCentre;
    }
    catch (e) {
        alert("ValidatePopulation : " + e.Message);
    }
}

function onLoad() {
    hiding_subGrid();
}

function hiding_subGrid() {
    Xrm.Page.ui.tabs.get("tab_2").sections.get("tab_2_section_2").setVisible(false);
}
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// RIBBON AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function checkDirty() {
    var mod = Xrm.Page.data.entity.getIsDirty();
    if (mod) {
        alert("You have to save the form first!");
        return true;
    }
    else {
        return false;
    }
}

function trs_cmdReceiveReturn_OnClick() {
    //alert("Receive Returned!"); 43307304-5FDA-E711-9EAB-C4346BAC57E3
    var workflowId = '43307304-5FDA-E711-9EAB-C4346BAC57E3';
    var workflowName = "Confirm Receive Returned";
    ExecuteWorkflow(workflowId, workflowName, setTimeout(function () { RefreshForm(); }, 3000));
}

function trs_cmdConfirmReturn_OnClick() {
    //alert("Hello Confirm Returned");
    try {
        debugger;
        var check;
        var tbID = Xrm.Page.data.entity.getId();
        XrmServiceToolkit.Rest.RetrieveMultiple(
        "trs_toolsborrowdetailSet",
        "$select=trs_DateReturned,trs_ConditionReturned,trs_StatusTools&$filter=trs_ToolsBorrowDetailsId/Id eq (guid'" + tbID + "')",

        function (result) {
            re = result;
            if (result.length > 0) {
                for (var i = 0; i < re.length; i++) {
                    //alert("Status : " + re[i].trs_StatusTools.Value);
                    if (re[i].trs_StatusTools.Value == 167630003) {
                        if (re[i].trs_DateReturned == null && re[i].trs_ConditionReturned.Value == null) {
                            check = true;
                        } else { check = false; }
                    } else { break; }
                }
                if (!check) {
                    //alert("Confirm returned!");
                    var workflowId = '69F476D9-93D9-E711-9EAB-C4346BAC57E3';
                    var workflowName = "Confirm Return";
                    ExecuteWorkflow(workflowId, workflowName, setTimeout(function () { RefreshForm(); }, 3000));
                } else {
                    alert("Cannot Confirm Transfer : Please check your tools borrow details before confirm return.");
                }
            }
            else {
                alert("WARNING: There is no to retrieve.");
            }
        },

        function error(error) {
            alert("WARNING: Please fill the Date Returned & Condition Returned on Tools Borrow Detail.");
        },

        function onComplete() {
            //return;
        },
        false);
    } catch (e) {
        alert(e.message);
        alert("WARNING: Please fill the Date Returned & Condition Returned on Tools Borrow Detail.");
    }
}

function trs_cmdConfirmReceive_OnClick() {
    var workflowId = 'e89fcd6e-4d18-4c81-8890-39ec14d39d91';
    var workflowName = "Confirm Receive";
    ExecuteWorkflow(workflowId, workflowName, setTimeout(function () { RefreshForm(); }, 2000));
}

function trs_cmdTransferTools_OnClick() {

    var isdirty = checkDirty();
    if (isdirty == false) {
        // For hiding section 1 grid 1
        Xrm.Page.ui.tabs.get("tab_2").sections.get("tab_2_section_1").setVisible(false);
        // For visible section 2 grid 2
        Xrm.Page.ui.tabs.get("tab_2").sections.get("tab_2_section_2").setVisible(true);
    }
}

function trs_cmdConfirmTransfer_OnClick() {

    try {
        debugger;
        var check;
        var tbID = Xrm.Page.data.entity.getId();
        XrmServiceToolkit.Rest.RetrieveMultiple(
        "trs_toolsborrowdetailSet",
        "$select=trs_DateTransferred,trs_ConditionTransferred,trs_StatusTools&$filter=trs_ToolsBorrowDetailsId/Id eq (guid'" + tbID + "')",

        function (result) {
            re = result;
            if (result.length > 0) {
                for (var i = 0; i < re.length; i++) {
                    //alert("Status : " + re[i].trs_StatusTools.Value);
                    if (re[i].trs_StatusTools.Value == 167630001) {
                        if (re[i].trs_DateTransferred == null && re[i].trs_ConditionTransferred.Value == null) {
                            check = true;
                        } else { check = false; }
                    } else { check = true; }
                }
                if (!check) {
                    //alert("OK Button Confirm!");
                    var workflowId = '5039CA67-8773-4ACD-BF3F-4C0A22D824E9';
                    var workflowName = "Confirm Transfer";
                    ExecuteWorkflow(workflowId, workflowName, setTimeout(function () { alert("Warning: This page will automatically refresh."); RefreshForm(); }, 7000));
                    //, function () { RefreshForm(); });
                } else {
                    alert("Cannot Confirm Transfer : Please check your tools borrow details before confirm transfer.");
                }
            }
            else {
                alert("WARNING: There is no to retrieve.");
            }
        },

        function error(error) {
            alert("WARNING: Please fill the Date Transferred & Condition Transferred on Tools Borrow Detail.");
        },

        function onComplete() {
            //return;
        },
        false);
    } catch (e) {
        alert(e.message);
        alert("WARNING: Please fill the Date Transferred & Condition Transferred on Tools Borrow Detail.");
    }
}

function trs_cmdHold_OnClick() {
    var isdirty = checkDirty();
    if (isdirty == false) {
        var workflowId = '6CB329F9-4DAA-4E4D-A944-C45522FEB9E4';
        var workflowName = "Hold WO";
        ExecuteWorkflow(workflowId, workflowName);
    }
}

function trs_cmdUnhold_OnClick() {
    var isdirty = checkDirty();
    if (isdirty == false) {
        var workflowId = '8764E271-78BC-410B-A85C-8AACBCFC980B';
        var workflowName = "Unhold WO";
        ExecuteWorkflow(workflowId, workflowName);
    }
}

function trs_cmdDispatch_OnClick() {
    var isdirty = checkDirty();
    var Id = Xrm.Page.data.entity.getId();
    var workflowId = '3A9C28EC-106E-44F2-8A28-3A7B8A7C3DD3';
    var workflowName = "Dispatch WO";

    if (isdirty == false) {
        var IsFMC = Xrm.Page.getAttribute("trs_fullmaintenancecontract").getValue();
        var IsWorkshop = Xrm.Page.getAttribute("trs_workshop").getValue();

        if (IsFMC != null && IsFMC == true && IsWorkshop != null && IsWorkshop == true) {
            var AvailableBay = Xrm.Page.getAttribute("trs_availablebay").getValue();

            if (AvailableBay != null) {
                ExecuteWorkflow(workflowId, workflowName);
                Xrm.Utility.openEntityForm("serviceappointment", Id);
            }
            else {
                alert("Please choose Bay from Available Bay.");
                Xrm.Page.getControl("trs_availablebay").setVisible(true);
                Xrm.Page.ui.controls.get("trs_availablebay").setDisabled(false);
                Xrm.Page.getControl("trs_availablebay").setFocus();
            }
        }
        else {
            ExecuteWorkflow(workflowId, workflowName);
            Xrm.Utility.openEntityForm("serviceappointment", Id);
        }
    }
}

function WO_PartsSummary() {
    var isdirty = checkDirty();
    if (isdirty == false) {
        //Precheck
        var precheck = true;
        XrmServiceToolkit.Rest.RetrieveMultiple(
            "trs_workorderpartssummarySet",
            "?$filter=trs_ConfirmReturn eq false and  trs_returnedquantity gt 0",
            function (results) {
                if (results.length > 0)
                    precheck = false;
            },
            function (error) {
                alert(error.message);
            },
            function () {
                //On Complete - Do Something
            },
            false
        );
        if (!precheck) {
            alert("There are part summary that have part returned but is not confirmed by supervisor. Unconfirmed part will not be processed in this part return.");
        }
        var workflowId = 'F4ECC0F7-A6E3-4555-B2B4-A0FEAA7636E5';
        var workflowName = "Summarize Parts WO";
        ExecuteWorkflow(workflowId, workflowName);
    }
}

function WO_Release() {
    try {
        var equipment = Xrm.Page.getAttribute("trs_equipment").getValue();
        var equipmentId = equipment[0].id;
        //alert (equipmentId);
        var workCenter = ValidatePopulation(equipmentId);
        //alert ("workCenter: " + workCenter);
        var PartCostWO = Xrm.Page.getAttribute("new_partcost").getValue();
        var ApprovedCostWO = Xrm.Page.getAttribute("new_approvecostwo").getValue();
        var TotalPart = Xrm.Page.getAttribute("trs_totalpart").getValue();
        var ApproveFullSupplyWO = Xrm.Page.getAttribute("ittn_approvefullsupplywo").getValue();
        var NeedApproveFullSupplyWO = Xrm.Page.getAttribute("ittn_needapprovefullsupplywo").getValue();

        if (workCenter == null) {
            alert("Please fill Unit's Work Center to release");
        } else {
            var isdirty = checkDirty();

            if (isdirty == false) {
                if (PartCostWO != null && PartCostWO == 1 && ApprovedCostWO != null && ApprovedCostWO == false) {
                    alert("This Work Order Part Cost needs to approve before Release to SAP.");
                }
                else if (TotalPart != null && TotalPart > 0 && PartCostWO == null) {
                    alert("This Work Order Part Cost needs to fulfill before Release to SAP.");
                }
                else {
                    //23-05-2019 - Pengecekan Maksimum Full Supply WO
                    if (NeedApproveFullSupplyWO != null && NeedApproveFullSupplyWO == true && (ApproveFullSupplyWO == null || ApproveFullSupplyWO != null && ApproveFullSupplyWO == false)) {
                        alert("This Work Order Full Supply WO needs to approve before Release to SAP.");
                    }
                    else {
                        var workflowId = '3BE8F213-1CC1-42E1-9134-B7F603D61817';
                        var workflowName = 'Release WO';
                        ExecuteWorkflow(workflowId, workflowName);
                    }
                }
            }
        }
    }
    catch (e) {
        alert("WO_Release : " + e.message);
    }
}

function WO_GetPartsTools() {
    var isdirty = checkDirty();
    if (isdirty == false) {
        var workflowId = '81BDD1E6-DB1E-4315-BD17-CAC66E0ED52A';
        var workflowName = 'Get Parts and Tools';
        ExecuteWorkflow(workflowId, workflowName);
    }
}

function WO_PartReturn() {
    var isdirty = checkDirty();
    if (isdirty == false) {
        var workflowId = '314a5e16-407f-47eb-98dc-025f486854e1';
        var workflowName = 'Return Part';
        ExecuteWorkflow(workflowId, workflowName);
    }
}

function WO_RequestTools() {
    var isdirty = checkDirty();
    if (isdirty == false) {
        var workflowId = '06ffd51b-f2fa-4db7-9027-047115dcf17b';
        var workflowName = 'Request Tools';
        ExecuteWorkflow(workflowId, workflowName);
    }
}

function WO_CalculateRTG() {
    var isdirty = checkDirty();
    if (isdirty == false) {
        var workflowId = 'DC9ABAFE-C85C-4F12-AF51-BD39ECB18624';
        var workflowName = 'Calculate RTG';
        ExecuteWorkflow(workflowId, workflowName);
    }
}

function UpdateWOtoSAP() {
    var isdirty = checkDirty();
    var ApprovePartChanges = Xrm.Page.getAttribute("ittn_approvepartchanges").getValue();
    var NeedApprovePartChanges = Xrm.Page.getAttribute("ittn_needapprovepartchanges").getValue();

    if (isdirty == false) {
        if (NeedApprovePartChanges != null && NeedApprovePartChanges == true && (ApprovePartChanges == null || ApprovePartChanges != null && ApprovePartChanges == false)) {
            alert("This Work Order Part Changes needs to approve before Update to SAP.");
        }
        else {
            var workflowId = 'A73DD083-81AF-453F-B032-0B3B6A2696CD';
            var workflowName = 'WO Update to SAP';
            ExecuteWorkflow(workflowId, workflowName);

            UpdateFieldValue("ittn_needapprovepartchanges", false);
            UpdateFieldValue("ittn_approvepartchanges", false);
            UpdateFieldValue("ittn_approvepartchangesby", null);
            UpdateFieldValue("ittn_partchangescurrentapprover", null);
            UpdateFieldValue("ittn_reqapprovepartchangesdate", null);
            UpdateFieldValue("ittn_approvepartchangesdate", null);
            UpdateFieldValue("ittn_escalatepartchangesdate", null);
        }
    }
}

function UpdateFieldValue(fieldname, fieldvalue) {
    Xrm.Page.getAttribute(fieldname).setValue(fieldvalue);
    Xrm.Page.getAttribute(fieldname).setSubmitMode("always");
    Xrm.Page.getAttribute(fieldname).fireOnChange();
}

function trs_CopyQuotation_OnClick() {
    var isdirty = checkDirty();
    if (isdirty == false) {
        var workflowId = '4E961079-4B9C-4A26-B099-E9F65F10D7E6';
        var workflowName = 'WO Copy data Quotation Workorder';
        ExecuteWorkflow(workflowId, workflowName, function () { RefreshForm(); });
    }
}

function CheckWarrantyService() {
    ///<summary>Check Warranty Service</summary>
    ///<returns type="bool">In Warranty / Not</returns>
    return CheckWarrantyService_ByMonth() || CheckWarrantyService_ByHourMeter();
}

function CheckWarrantyService_ByMonth() {
    ///<summary>Check Warranty Service By Month</summary>
    ///<returns type="bool">In Warranty / Not</returns>
    try {
        var createdOn = Xrm.Page.getAttribute("createdon").getValue();
        var serialNumber = Xrm.Page.getAttribute("new_serialnumber").getValue();
        var warrantyServiceBasedOn = GetWorkflowIntegrationConfiguration_WarrantyServiceBasedOn();

        //
        var returnResult = false;
        var resultActivityId, resultDocumentDate;

        var warrantyServiceBasedOnConst = {
            TaskListHeader: 167630000,
            TaskListGroup: 167630001
        };

        var countMonthDiff = function monthDiff(d1, d2) {
            var months;
            months = (d2.getFullYear() - d1.getFullYear()) * 12;
            months -= d1.getMonth() + 1;
            months += d2.getMonth();
            return months <= 0 ? 0 : months;
        };

        //Changes for service appointment query logic.

        // Look for current SR WO.
        // Check is there any Parent SR.
        // If no, return false
        // If yes, then
        // Look for Service Appointment for Parent SR
        // Then use the document date from there.

        var currentRegarding = Xrm.Page.getAttribute("regardingobjectid").getValue()[0].id;

        var parentCaseIdResult = null;

        XrmServiceToolkit.Rest.Retrieve(
            currentRegarding,
            "IncidentSet",
            "trs_SRRegarding",
            null,
            function (result) {
                var parentCaseId = result.trs_SRRegarding;
                parentCaseIdResult = parentCaseId.Id;
            },
            function (error) {
                alert(error.message);
            },
            false
        );

        if (parentCaseIdResult != null) {
            XrmServiceToolkit.Rest.RetrieveMultiple(
                "ServiceAppointmentSet",
                "?$select=ActivityId,trs_DocumentDate&$filter=RegardingObjectId/Id eq (guid'" + parentCaseIdResult + "')",
                function (results) {
                    for (var i = 0; i < results.length; i++) {
                        var activityId = results[i].ActivityId;
                        var trs_DocumentDate = results[i].trs_DocumentDate;
                        resultActivityId = activityId;
                        resultDocumentDate = trs_DocumentDate;
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
        } else {
            return returnResult;
        }

        var monthDiff = countMonthDiff(resultDocumentDate, createdOn);
        console.log("Month Difference : " + monthDiff);

        if (typeof (warrantyServiceBasedOn) != "undefined" && warrantyServiceBasedOn != null) {
            console.log("Entering Warranty Checking.");
            if (warrantyServiceBasedOn === warrantyServiceBasedOnConst.TaskListGroup) {
                console.log("Warranty Service Based On Task List Group");
                console.dir(resultActivityId);

                XrmServiceToolkit.Rest.RetrieveMultiple(
                    "TaskSet",
                    "?$select=trs_trs_tasklistgroup_task_TaskListGroup/trs_WarrantyHourMeter,trs_trs_tasklistgroup_task_TaskListGroup/trs_WarrantyMonth&$expand=trs_trs_tasklistgroup_task_TaskListGroup&$filter=trs_OperationId/Id eq (guid'" + resultActivityId + "')",
                    function (results) {
                        console.log("Task Query Result Obtained.");
                        console.dir(results);
                        var maxHourMeter = 0, maxMonth = 0;

                        for (var i = 0; i < results.length; i++) {
                            var trs_trs_tasklistgroup_task_TaskListGroup_trs_WarrantyHourMeter = results[i].trs_trs_tasklistgroup_task_TaskListGroup.trs_WarrantyHourMeter;
                            var trs_trs_tasklistgroup_task_TaskListGroup_trs_WarrantyMonth = results[i].trs_trs_tasklistgroup_task_TaskListGroup.trs_WarrantyMonth;
                            if (maxHourMeter < trs_trs_tasklistgroup_task_TaskListGroup_trs_WarrantyHourMeter)
                                maxHourMeter = trs_trs_tasklistgroup_task_TaskListGroup_trs_WarrantyHourMeter;
                            if (maxMonth < trs_trs_tasklistgroup_task_TaskListGroup_trs_WarrantyMonth)
                                maxMonth = trs_trs_tasklistgroup_task_TaskListGroup_trs_WarrantyMonth;
                        }

                        alert("max month : " + maxMonth);
                        console.log("Maximum Month : " + maxMonth);
                        if (monthDiff >= 0 && monthDiff <= maxMonth) returnResult = true;

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
            if (warrantyServiceBasedOn === warrantyServiceBasedOnConst.TaskListHeader) {
                console.log("Warranty Service Based On Task List Header");
                XrmServiceToolkit.Rest.RetrieveMultiple(
                    "TaskSet",
                    "?$select=trs_trs_tasklistheader_task_Task/trs_WarrantyHourMeter,trs_trs_tasklistheader_task_Task/trs_WarrantyMonth&$expand=trs_trs_tasklistheader_task_Task&$filter=trs_OperationId/Id eq (guid'" + resultActivityId + "')",
                    function (results) {
                        console.log("Task Query Result Obtained.");
                        console.dir(results);

                        var maxHourMeter = 0, maxMonth = 0;
                        for (var i = 0; i < results.length; i++) {
                            var trs_trs_tasklistheader_task_Task_trs_WarrantyHourMeter = results[i].trs_trs_tasklistheader_task_Task.trs_WarrantyHourMeter;
                            var trs_trs_tasklistheader_task_Task_trs_WarrantyMonth = results[i].trs_trs_tasklistheader_task_Task.trs_WarrantyMonth;
                            if (maxHourMeter < trs_trs_tasklistheader_task_Task_trs_WarrantyHourMeter)
                                maxHourMeter = trs_trs_tasklistheader_task_Task_trs_WarrantyHourMeter;
                            if (maxMonth < trs_trs_tasklistheader_task_Task_trs_WarrantyMonth)
                                maxMonth = trs_trs_tasklistheader_task_Task_trs_WarrantyMonth;
                        }

                        console.log("Maximum Month : " + maxMonth);
                        if (monthDiff > 0 && monthDiff <= maxMonth) returnResult = true;
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
        }
        console.log("Return Result : " + returnResult);
        return returnResult;
    } catch (e) {
        console.log("Error occured, message:");
        console.log(e.message);
        console.log("Returning false result.");
        return false;
    }

}

function CheckWarrantyService_ByHourMeter() {
    ///<summary>Check Warranty Service By Hour Meter</summary>
    ///<returns type="bool">In Warranty / Not</returns>
    try {
        var currentHourMeter = Xrm.Page.getAttribute("trs_lasthourmeter").getValue();
        var warrantyServiceBasedOn = GetWorkflowIntegrationConfiguration_WarrantyServiceBasedOn();
        var serialNumber = Xrm.Page.getAttribute("new_serialnumber").getValue();

        console.log("Checking Warranty Service By Hour Meter");
        console.log("Current Hour Meter :" + currentHourMeter);
        console.log("Warranty Service Based On :" + warrantyServiceBasedOn);
        console.log("Serial Number: " + serialNumber);

        var warrantyServiceBasedOnConst = {
            TaskListHeader: 167630000,
            TaskListGroup: 167630001
        };
        var returnResult = false;

        var resultLastHourMeter;
        var resultCurrentHourMeter;

        //Changes for service appointment query logic.

        // Look for current SR WO.
        // Check is there any Parent SR.
        // If no, return false
        // If yes, then
        // Look for Service Appointment for Parent SR

        var currentRegarding = Xrm.Page.getAttribute("regardingobjectid").getValue()[0].id;

        var parentCaseIdResult = null;

        XrmServiceToolkit.Rest.Retrieve(
            currentRegarding,
            "IncidentSet",
            "trs_SRRegarding",
            null,
            function (result) {
                var parentCaseId = result.trs_SRRegarding;
                parentCaseIdResult = parentCaseId.Id;
            },
            function (error) {
                alert(error.message);
            },
            false
        );
        var resultActivityId;

        if (parentCaseIdResult != null) {
            XrmServiceToolkit.Rest.RetrieveMultiple(
                "ServiceAppointmentSet",
                "?$select=ActivityId,trs_HourMeter,trs_LastHourMeter&$filter=RegardingObjectId/Id eq (guid'" + parentCaseIdResult + "')",
                function (results) {
                    console.log("Service Appointment Query Result Obtained.");
                    console.dir(results);
                    for (var i = 0; i < results.length; i++) {
                        var activityId = results[i].ActivityId;
                        var trs_HourMeter = results[i].trs_HourMeter;
                        var trs_LastHourMeter = results[i].trs_LastHourMeter;
                        resultLastHourMeter = trs_HourMeter;
                        resultCurrentHourMeter = trs_LastHourMeter;
                        resultActivityId = activityId;
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
        } else {
            return returnResult;
        }


        var diffHourMeter = resultCurrentHourMeter - resultLastHourMeter;
        console.log("Hour Meter Difference : " + diffHourMeter);

        if (typeof (warrantyServiceBasedOn) != "undefined" && warrantyServiceBasedOn != null) {
            console.log("Entering Warranty Checking.");
            if (warrantyServiceBasedOn === warrantyServiceBasedOnConst.TaskListGroup) {
                console.log("Warranty Service Based On Task List Group");
                console.dir(resultActivityId);
                XrmServiceToolkit.Rest.RetrieveMultiple(
                    "TaskSet",
                    "?$select=trs_trs_tasklistgroup_task_TaskListGroup/trs_WarrantyHourMeter,trs_trs_tasklistgroup_task_TaskListGroup/trs_WarrantyMonth&$expand=trs_trs_tasklistgroup_task_TaskListGroup&$filter=trs_OperationId/Id eq (guid'" + resultActivityId + "')",
                    function (results) {
                        console.log("Task Query Result Obtained.");
                        console.dir(results);

                        var maxHourMeter = 0, maxMonth = 0;

                        for (var i = 0; i < results.length; i++) {
                            var trs_trs_tasklistgroup_task_TaskListGroup_trs_WarrantyHourMeter = results[i].trs_trs_tasklistgroup_task_TaskListGroup.trs_WarrantyHourMeter;
                            var trs_trs_tasklistgroup_task_TaskListGroup_trs_WarrantyMonth = results[i].trs_trs_tasklistgroup_task_TaskListGroup.trs_WarrantyMonth;
                            if (maxHourMeter < trs_trs_tasklistgroup_task_TaskListGroup_trs_WarrantyHourMeter)
                                maxHourMeter = trs_trs_tasklistgroup_task_TaskListGroup_trs_WarrantyHourMeter;
                            if (maxMonth < trs_trs_tasklistgroup_task_TaskListGroup_trs_WarrantyMonth)
                                maxMonth = trs_trs_tasklistgroup_task_TaskListGroup_trs_WarrantyMonth;
                        }
                        console.log("Maximum Hour Meter : " + maxHourMeter);
                        if (diffHourMeter >= 0 && diffHourMeter <= maxHourMeter) returnResult = true;

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
            if (warrantyServiceBasedOn === warrantyServiceBasedOnConst.TaskListHeader) {
                console.log("Warranty Service Based On Task List Group");
                XrmServiceToolkit.Rest.RetrieveMultiple(
                    "TaskSet",
                    "?$select=trs_trs_tasklistheader_task_Task/trs_WarrantyHourMeter,trs_trs_tasklistheader_task_Task/trs_WarrantyMonth&$expand=trs_trs_tasklistheader_task_Task&$filter=trs_OperationId/Id eq (guid'" + resultActivityId + "')",
                    function (results) {
                        console.log("Task Query Result Obtained.");
                        console.dir(results);

                        var maxHourMeter = 0, maxMonth = 0;
                        for (var i = 0; i < results.length; i++) {
                            var trs_trs_tasklistheader_task_Task_trs_WarrantyHourMeter = results[i].trs_trs_tasklistheader_task_Task.trs_WarrantyHourMeter;
                            var trs_trs_tasklistheader_task_Task_trs_WarrantyMonth = results[i].trs_trs_tasklistheader_task_Task.trs_WarrantyMonth;
                            if (maxHourMeter < trs_trs_tasklistheader_task_Task_trs_WarrantyHourMeter)
                                maxHourMeter = trs_trs_tasklistheader_task_Task_trs_WarrantyHourMeter;
                            if (maxMonth < trs_trs_tasklistheader_task_Task_trs_WarrantyMonth)
                                maxMonth = trs_trs_tasklistheader_task_Task_trs_WarrantyMonth;
                        }

                        console.log("Maximum Hour Meter : " + maxHourMeter);
                        if (diffHourMeter > 0 && diffHourMeter <= maxHourMeter) returnResult = true;
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
        }
        console.log("Return Result : " + returnResult);
        return returnResult;
    } catch (e) {
        console.log("Error occured, message:");
        console.log(e.message);
        console.log("Returning false result.");
        return false;
    }
}

function CheckWarrantySales() {
    ///<summary>Check Warranty Sales</summary>
    ///<returns type="bool">In Warranty / Not</returns>
    try {
        var inWarranty = false;
        var createdOn = Xrm.Page.getAttribute("createdon").getValue();

        console.log("Check Warranty Sales");
        console.log("Wo Created On : ");
        console.dir(createdOn);
        if (Xrm.Page.getAttribute("trs_equipment")) {
            var equipmentId = Xrm.Page.getAttribute("trs_equipment").getValue()[0].id;
            //get the warranty date here.
            console.log("Querying population");
            XrmServiceToolkit.Rest.Retrieve(
                equipmentId,
                "new_populationSet",
                "trs_WarrantyEndDate,trs_WarrantyStartdate",
                null,
                function (result) {
                    console.log("Population Query Result Obtained");
                    console.dir(result);
                    var trs_WarrantyEndDate = result.trs_WarrantyEndDate;
                    var trs_WarrantyStartdate = result.trs_WarrantyStartdate;

                    inWarranty = (createdOn <= trs_WarrantyEndDate) && (createdOn >= trs_WarrantyStartdate);
                },
                function (error) {
                    alert(error.message);
                },
                false
            );
        }
        console.log("Return Result : " + inWarranty);
        return inWarranty;
    } catch (e) {
        console.log("Error occured, message:");
        console.log(e.message);
        console.log("Returning false result.");
        return false;
    }

}


function GetWorkflowIntegrationConfiguration_WarrantyServiceBasedOn() {
    var resultValue;

    console.log("Calling GetWorkflowIntegrationConfiguration_WarrantyServiceBasedOn.");

    XrmServiceToolkit.Rest.RetrieveMultiple(
        "trs_workflowconfigurationSet",
        "?$select=trs_WarrantyServiceBasedOn",
        function (results) {
            console.log("Query Workflow Configuration Set Result Obtained.");
            console.dir(results);

            for (var i = 0; i < results.length; i++) {
                resultValue = results[i].trs_WarrantyServiceBasedOn.Value;
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

function WO_TECO_CheckRedo() {
    var workOrderId = Xrm.Page.data.entity.getId();
    var pmacttypelookup = Xrm.Page.getAttribute("trs_pmacttype").getValue();
    var pmacttype = pmacttypelookup[0].name
    var IsCalculateToRedo = Xrm.Page.getAttribute("trs_iscalculatetoredo").getValue();

    var max = 0;
    if (typeof (XrmServiceToolkit) === "undefined") {
        alert("Xrm Service Toolkit is not loaded.");
        return;
    }
    XrmServiceToolkit.Rest.RetrieveMultiple(
        "trs_workorderpartssummarySet",
        "?$select=trs_DispatchOrder&$filter=trs_workorder/Id eq (guid'" + workOrderId + "')",
        function (results) {
            for (var i = 0; i < results.length; i++) {
                var trs_DispatchOrder = results[i].trs_DispatchOrder;
                if (trs_DispatchOrder > max) max = trs_DispatchOrder;
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


    var accIndicConst = {
        Customer: 1,
        Internal: 2,
        Warranty: 3,
        Contract: 4
    };

    var accInd = Xrm.Page.getAttribute("trs_accind").getValue();

    //If this is the second dispatch or in warranty
    if (max > 1 || (pmacttype == 'REDO' && accInd === accIndicConst.Internal)) { //CHECK WARRANTY SERVICE CHANGE TO PMACTTYPE REDO
        //then
        if (IsCalculateToRedo == null) {
            alert("This Work Order indicate will be cut point of mechanic.");

            Xrm.Page.getControl("trs_iscalculatetoredo").setVisible(true);
            Xrm.Page.ui.controls.get("trs_iscalculatetoredo").setDisabled(false);
            Xrm.Page.getAttribute("trs_iscalculatetoredo").setRequiredLevel("required");
            Xrm.Page.getControl("trs_iscalculatetoredo").setFocus();
        }
        else {
            Xrm.Page.getControl("trs_iscalculatetoredo").setVisible(true);
            Xrm.Page.ui.controls.get("trs_iscalculatetoredo").setDisabled(true);
        }
    }
}

function WO_TECO() {
    try {
        var docdate = Xrm.Page.getAttribute("trs_documentdate").getValue();
        var statusinOperation = Xrm.Page.getAttribute("trs_statusinoperation").getValue();
        var FullSupplyWOParts = Xrm.Page.getAttribute("ittn_fullsupplywoparts").getValue();

        if (statusinOperation == null) {
            Xrm.Page.getAttribute("trs_statusinoperation").setRequiredLevel("required");
            alert('Please fill Status in Operation');
            Xrm.Page.getControl("trs_statusinoperation").setFocus();
        } else if (docdate == null) {
            Xrm.Page.getAttribute("trs_documentdate").setRequiredLevel("required");
            alert('Please fill BAST/BAPP Sign Date');
            Xrm.Page.getControl("trs_documentdate").setFocus();
        } else {
            var isdirty = checkDirty();
            if (isdirty == false) {
                var type = 0;
                var pmacttypelookup = Xrm.Page.getAttribute("trs_pmacttype").getValue();

                if (FullSupplyWOParts != null && FullSupplyWOParts == true) {
                    if (pmacttypelookup != null) {
                        var pmacttype = pmacttypelookup[0].name;
                        if (pmacttype == 'INSPECTION' || pmacttype == 'PPM')
                            type = 1;
                        else if (pmacttype == 'WARRANTY REPAIR' || pmacttype == 'REPAIR')
                            type = 2;

                        var status = IsAllowtoSubmitTeco(type);

                        if (status) {
                            var isAllowTeco = checkPartReturn();

                            if (isAllowTeco) {
                                var workflowId = 'E8614282-8D1B-4BC6-A7BA-5BAFB4D9A860';
                                var workflowName = 'Submit TECO';

                                var IsCalculateToRedo = Xrm.Page.getAttribute("trs_iscalculatetoredo").getValue();
                                var accInd = Xrm.Page.getAttribute("trs_accind").getValue();
                                var regarding = Xrm.Page.getAttribute("regardingobjectid");
                                var serviceRequisitionId = regarding.getValue()[0].id;

                                XrmServiceToolkit.Rest.Retrieve(
                                    serviceRequisitionId,
                                    "IncidentSet",
                                    "trs_RedoSR,trs_SRRegarding", null,
                                    function (result) {
                                        var RedoSR = result.trs_RedoSR;
                                        var SRRegarding = result.trs_SRRegarding;

                                        if (SRRegarding.Id != null || RedoSR.Id != null) {
                                            if ((pmacttype == 'REDO' || pmacttype == 'REPAIR') && IsCalculateToRedo == null && accInd == 2) //ACCIND : 2 (INTERNAL)
                                                WO_TECO_CheckRedo();
                                            else
                                                ExecuteWorkflow(workflowId, workflowName);
                                        }
                                        else
                                            ExecuteWorkflow(workflowId, workflowName);
                                    }, function (error) {
                                        Xrm.Utility.alertDialog(error.message);
                                    },
                                    false
                                );
                            }
                            else {
                                alert('Please confirm remaining parts first!');
                            }

                            if (type == 2) {
                                var workflowId = 'AF239336-2056-4682-B0DE-D5312D5A990C';
                                var workflowName = 'TSR_SendEmailNotification';
                                ExecuteWorkflow(workflowId, workflowName);
                            }
                        }
                        else {
                            switch (type) {
                                case 1:
                                    alert('Please insert PPM Report');
                                    break;
                                case 2:
                                    alert('Please insert TSR');
                                    break;
                                default:
                                    alert('Failed to Submit TECO');
                            }
                        }
                    }
                }
                else {
                    alert("This Work Order Part need to confirm supply before TECO to SAP.");
                }
            }
        }
    }
    catch (e) {
        alert("WO_TECO : " + e.message);
    }
}

function checkPartReturn() {
    try {
        var returnStatus = false;
        var recordId = Xrm.Page.data.entity.getId().replace('{', '').replace('}', '');

        var pathPart = "/trs_workorderpartssummarySet?$select=trs_acceptedquantity,trs_returnedquantity,trs_ReturnedtoPartsDept&$filter=trs_workorder/Id eq guid'" + recordId + "'";
        var retrieveReqPart = RetrieveOData(pathPart);
        //alert ('in: ' + returnStatus);
        if (retrieveReqPart.readyState == 4 /* complete */) {
            var retrievedPart = this.parent.JSON.parse(retrieveReqPart.responseText).d;
            // Cek apakah ada parts
            if (retrievedPart.results != null && retrievedPart.results.length > 0) {
                //alert ('retrievedPart: ' + retrievedPart.results.length);
                var i = 0;
                var acceptedQty = 0;
                var returnedQty = 0;
                var confirmReturnedQty = 0;

                while (i < retrievedPart.results.length) {
                    acceptedQty = acceptedQty + retrievedPart.results[i].trs_acceptedquantity;
                    returnedQty = returnedQty + retrievedPart.results[i].trs_returnedquantity;
                    confirmReturnedQty = confirmReturnedQty + retrievedPart.results[i].trs_ReturnedtoPartsDept;
                    i++;
                }
                //alert ('returnedQty: ' + returnedQty);

                // If return qty bigger than 0 
                if (returnedQty >= 1) {
                    // If return qty has been confirmed
                    if (confirmReturnedQty >= 1) {
                        returnStatus = true;
                    }
                }
                else {
                    // Tidak ada part yang tersisa
                    returnStatus = true;
                }
            }
            else {
                // Jika tidak ada part maka balikan harus true
                returnStatus = true;
            }
        }
        //alert ('end: ' + returnStatus);
        return returnStatus;
    }
    catch (e) {
        throw ("checkPartReturn : " + e.message);
    }
}

function IsAllowtoSubmitTeco(type) {   // If Type equals warranty repair, must have tsr
    try {
        var status = false;
        var recordId = Xrm.Page.data.entity.getId().replace('{', '').replace('}', '');

        if (type == 1) {
            var path = "/trs_ppmreportSet?$select=trs_ppmreportId&$filter=trs_WorkOrder/Id eq guid'" + recordId + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved.results != null && retrieved.results.length > 0) {
                    status = true;
                }
            }
        }
        else if (type == 2) {
            var path = "/trs_technicalservicereportSet?$select=trs_TSRNumber&$filter=trs_WorkOrder/Id eq guid'" + recordId + "'";
            var retrieveReq = RetrieveOData(path);
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved.results.length > 0) {
                    status = true;
                }
            }
        }
        else {
            status = true;
        }

        return status;
    }
    catch (e) {
        throw ("IsAllowtoSubmitTeco : " + e.message);
    }
}

function ExecuteWorkflow(workflowId, workflowName, successCallback, failedCallback) {
    var _return = window.confirm('Do you want to ' + workflowName + ' ?');
    if (_return) {
        //var url = Xrm.Page.context.getServerUrl();
        var url = document.location.protocol + "//" + document.location.host + "/" + Xrm.Page.context.getOrgUniqueName();
        var entityId = Xrm.Page.data.entity.getId();
        var OrgServicePath = "/XRMServices/2011/Organization.svc/web";
        url = url + OrgServicePath;
        var request;
        request = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
                      "<s:Body>" +
                        "<Execute xmlns=\"http://schemas.microsoft.com/xrm/2011/Contracts/Services\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">" +
                          "<request i:type=\"b:ExecuteWorkflowRequest\" xmlns:a=\"http://schemas.microsoft.com/xrm/2011/Contracts\" xmlns:b=\"http://schemas.microsoft.com/crm/2011/Contracts\">" +
                            "<a:Parameters xmlns:c=\"http://schemas.datacontract.org/2004/07/System.Collections.Generic\">" +
                              "<a:KeyValuePairOfstringanyType>" +
                                "<c:key>EntityId</c:key>" +
                                "<c:value i:type=\"d:guid\" xmlns:d=\"http://schemas.microsoft.com/2003/10/Serialization/\">" + entityId + "</c:value>" +
                              "</a:KeyValuePairOfstringanyType>" +
                              "<a:KeyValuePairOfstringanyType>" +
                                "<c:key>WorkflowId</c:key>" +
                                "<c:value i:type=\"d:guid\" xmlns:d=\"http://schemas.microsoft.com/2003/10/Serialization/\">" + workflowId + "</c:value>" +
                              "</a:KeyValuePairOfstringanyType>" +
                            "</a:Parameters>" +
                            "<a:RequestId i:nil=\"true\" />" +
                            "<a:RequestName>ExecuteWorkflow</a:RequestName>" +
                          "</request>" +
                        "</Execute>" +
                      "</s:Body>" +
                    "</s:Envelope>";

        var req = new XMLHttpRequest();
        req.open("POST", url, true)
        // Responses will return XML. It isn't possible to return JSON.
        req.setRequestHeader("Accept", "application/xml, text/xml, */*");
        req.setRequestHeader("Content-Type", "text/xml; charset=utf-8");
        req.setRequestHeader("SOAPAction", "http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Execute");
        req.onreadystatechange = function () { AssignResponse(req, workflowName, successCallback, failedCallback); };
        req.send(request);
    }
}


function AssignResponse(req, workflowName, successCallback, failedCallback) {
    if (req.readyState == 4) {
        if (req.status == 200) {
            alert('Successfully executed ' + workflowName + '.');
            if (successCallback !== undefined && typeof successCallback === "function") {
                successCallback();
            }
        }
        else {
            var faultstring = req.responseXML.getElementsByTagName("faultstring")[0].textContent;
            alert('Fail to execute ' + workflowName + '.\r\n Response Code : (' + req.status + ')' + "\r\nPlease contact your IT support.\r\nTechnical Details :" + faultstring);
            if (failedCallback !== undefined && typeof failedCallback === "function") {
                failedCallback(new Error(faultstring));
            }
        }
    }
}

function RibbonRefreshDisplay() {
    Xrm.Page.ui.refreshRibbon();
}

function setWorkshop() {
    var work = Xrm.Page.getAttribute("trs_workshop").getValue();
    if (work == 1) {
        //    Xrm.Page.getAttribute("trs_contactperson").setRequiredLevel("recommended");
        //    Xrm.Page.getAttribute("trs_cpphone").setRequiredLevel("recommended");
        //    Xrm.Page.getAttribute("trs_cponsite").setRequiredLevel("recommended");
        //    Xrm.Page.getAttribute("trs_phoneonsite").setRequiredLevel("recommended");
        return false;
    }
    else {
        //    Xrm.Page.getAttribute("trs_contactperson").setRequiredLevel("required");
        //    Xrm.Page.getAttribute("trs_cpphone").setRequiredLevel("required");
        //    Xrm.Page.getAttribute("trs_cponsite").setRequiredLevel("required");
        //    Xrm.Page.getAttribute("trs_phoneonsite").setRequiredLevel("required");
        return true;
    }
    RibbonRefreshDisplay();
}


function RefreshForm() {
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

    var check = [];
    check.push(function () {
        return formType !== formStatus.Create;
    });

    var boolArrayCheck = function (predicateArray) {
        //Assume predicateArray is right
        var boolCheck = true;
        for (var index = 0; index < predicateArray.length; index++) {
            boolCheck = boolCheck && predicateArray[index]();
        }
        return boolCheck;
    }

    if (boolArrayCheck(check)) {
        Xrm.Utility.openEntityForm(Xrm.Page.data.entity.getEntityName(), Xrm.Page.data.entity.getId());
    }
}

function ConvertWORecommendationToSRAndQuotation_Ribbon_EnableRules() {
    try {
        var check = [];
        check.push(function () {
            //status related.
            var statusReason = Xrm.Page.getAttribute("statuscode");

            var statusCompleted = 8;
            var statusTecoToSap = 167630002;
            var statusSubTecoByMechanic = 167630003;
            var WORecommendationExist = false;
            var workOrderId = Xrm.Page.data.entity.getId();

            XrmServiceToolkit.Rest.RetrieveMultiple(
                "trs_workorderpartrecommendationSet",
                "?$filter=trs_workorder/Id eq (guid'" + workOrderId + "')",
                function (results) {
                    if (results.length > 0)
                        WORecommendationExist = true;
                },
                function (error) {
                    alert(error.message);
                },
                function () {
                    //On Complete - Do Something
                },
                false
            );

            if (statusReason && WORecommendationExist === true) {
                var statusReasonValue = statusReason.getValue();
                return statusReasonValue === statusSubTecoByMechanic || statusReasonValue === statusTecoToSap || statusReasonValue === statusCompleted;
            }
            return false;
        });

        check.push(function () {
            //warranty related.
            var accIndicConst = {
                Customer: 1,
                Internal: 2,
                Warranty: 3,
                Contract: 4
            };

            var accInd = Xrm.Page.getAttribute("trs_accind").getValue();

            if ((CheckWarrantySales() == false) || (CheckWarrantyService() == false && accInd === accIndicConst.Customer))
                return true;
            else
                return (!CheckWarrantySales()) && (CheckWarrantyService() && (accInd === accIndicConst.Customer));
        });

        var boolArrayCheck = function (predicateArray) {
            //Assume predicateArray is right
            var boolCheck = true;
            for (var index = 0; index < predicateArray.length; index++) {
                boolCheck = boolCheck && predicateArray[index]();
            }
            return boolCheck;
        }

        return boolArrayCheck(check);
    } catch (e) {
        if (console) {
            console.log(e.message);
        }
        return false;
    }
}

function CopyQuotationData_Ribbon_EnableRules() {
    try {
        var check = [];

        check.push(function () {
            var regarding = Xrm.Page.getAttribute("regardingobjectid");
            var result = false;
            if (regarding) {
                var serviceRequisitionId = regarding.getValue()[0].id;
                XrmServiceToolkit.Rest.RetrieveMultiple(
                    "trs_quotationSet",
                    "?$select=trs_quotationId&$filter=statuscode/Value eq 167630002 and trs_QuotationDeal eq true and trs_PODate ne null and trs_PONumber ne null and trs_ServiceRequisition/Id eq (guid'" + serviceRequisitionId + "')",
                    function (results) {
                        if (results.length > 0) {
                            result = true;
                        }
                    },
                    function (error) {
                        if (console) {
                            console.log(error.message);
                        }
                    },
                    function () {
                        //On Complete - Do Something
                    },
                    false
                );
            }
            return result;
        });

        var boolArrayCheck = function (predicateArray) {
            //Assume predicateArray is right
            var boolCheck = true;
            for (var index = 0; index < predicateArray.length; index++) {
                boolCheck = boolCheck && predicateArray[index]();
            }
            return boolCheck;
        }

        return boolArrayCheck(check);

    } catch (e) {
        if (console) {
            console.log(e.message);
        }
        return false;
    }
}

function ReturnParts_Ribbon_EnableRules() {
    try {
        var returnStatus = false;
        var UnconfirmedPart = 0;

        var recordId = Xrm.Page.data.entity.getId();

        XrmServiceToolkit.Rest.RetrieveMultiple(
            "trs_workorderpartssummarySet",
            "?$select=trs_ConfirmReturn&$filter=trs_returnedquantity gt 0 and trs_workorder/Id eq (guid'" + recordId + "')",
            function (results) {
                for (var i = 0; i < results.length; i++) {
                    var trs_ConfirmReturn = results[i].trs_ConfirmReturn;

                    if (trs_ConfirmReturn == null || trs_ConfirmReturn == false) {
                        UnconfirmedPart++;
                    }
                }
            },
            function (error) {
                if (console) {
                    console.log(error.message);
                }
            },
            function () {
                //On Complete - Do Something
            },
            false
        );

        if (UnconfirmedPart == 0) {
            returnStatus = true;
        }

        return returnStatus;
    }
    catch (e) {
        if (console) {
            console.log(e.message);
        }
    }
}

function EnableRule_ConvertWOToRepairWoAndSR() {
    var accIndicConst = {
        Customer: 1,
        Internal: 2,
        Warranty: 3,
        Contract: 4
    };

    var returnStatus = false;
    var statusReason = Xrm.Page.getAttribute("statuscode");
    var statusCompleted = 8;
    var statusTecoToSap = 167630002;
    var statusSubTecoByMechanic = 167630003;
    var accInd = Xrm.Page.getAttribute("trs_accind").getValue();
    var WORecommendationExist = false;
    var workOrderId = Xrm.Page.data.entity.getId();

    XrmServiceToolkit.Rest.RetrieveMultiple(
        "trs_workorderpartrecommendationSet",
        "?$filter=trs_workorder/Id eq (guid'" + workOrderId + "')",
        function (results) {
            if (results.length > 0)
                WORecommendationExist = true;
        },
        function (error) {
            alert(error.message);
        },
        function () {
            //On Complete - Do Something
        },
        false
    );

    if (statusReason && WORecommendationExist === true) {
        var statusReasonValue = statusReason.getValue();
        if (statusReasonValue === statusSubTecoByMechanic || statusReasonValue === statusTecoToSap || statusReasonValue === statusCompleted) {
            if (accInd === accIndicConst.Internal || accInd === accIndicConst.Warranty) {
                returnStatus = true;
            }
        }
    }

    return returnStatus;
}

function RunConvertWoToRepairWoAndSR() {
    var isdirty = checkDirty();
    if (isdirty == false) {

        var runThis = confirm("Do you want to Convert this WO into WO Repair & SR?");
        if (runThis) {
            //* Comment by Santony [12/09/2017] -> Change Dialog to Workflow
            //Process.callAction("trs_ConvertWOtoRepairWOSR",
            //[
            //    {
            //        key: "Target",
            //        type: Process.Type.EntityReference,
            //        value: new Process.EntityReference("serviceappointment", Xrm.Page.data.entity.getId())
            //    },
            //    {
            //        key: "WarrantySales",
            //        type: Process.Type.Bool,
            //        value : CheckWarrantySales()
            //    },
            //    {
            //        key: "WarrantyService",
            //        type: Process.Type.Bool,
            //        value: CheckWarrantyService()
            //    }
            //],
            //function (params) {
            //    var clonedWO = params["ClonedWO"].id;
            //    Xrm.Utility.openEntityForm(params["ClonedWO"].entityType, params["ClonedWO"].id);
            //},
            //function (e, t) {
            //    // Error
            //    alert(e);
            //    // Write the trace log to the dev console
            //    if (window.console && console.error) {
            //        console.error(e + "\n" + t);
            //    }
            //});
            //alert("Action is called, please wait.");

            var workflowName = "Convert WO to WO Repair & SR";
            var workflowId = "86173943-B0B1-46D9-9F33-09652C3610ED";
            var recordId = Xrm.Page.data.entity.getId();
            Process.ExecuteWorkflow(workflowId, recordId, function () {
                //sukses ngapain
                alert("Convert WO to WO Repair & SR Success");
                //get SR from WO
                var SR = Xrm.Page.getAttribute("regardingobjectid").getValue();
                var SRId = SR[0].id;

                //get child SR from SR that last created
                var incidentId;
                XrmServiceToolkit.Rest.RetrieveMultiple(
        "IncidentSet",
        "?$select=IncidentId&$filter=trs_SRRegarding/Id eq (guid'" + SRId + "')&$top=1&$orderby=CreatedOn desc",
                    function (results) {
                        for (var i = 0; i < results.length; i++) {
                            incidentId = results[i].IncidentId;
                        }
                    },
                    function (error) {
                        alert(error.message);
                    },
                    function () {
                        if (incidentId != null) {
                            XrmServiceToolkit.Rest.RetrieveMultiple(
                                "ServiceAppointmentSet",
                                "?$select=ActivityId&$filter=RegardingObjectId/Id eq (guid'" + incidentId + "')",
                                function (results) {
                                    for (var i = 0; i < results.length; i++) {
                                        var activityId = results[i].ActivityId;
                                        resultActivityId = activityId;
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
                        Xrm.Utility.openEntityForm("serviceappointment", resultActivityId);
                    },
                    true
                );
            }, function (err) {
                //error ngapain
                alert("Failed to run workflow. Technical details :\r\n" + err.message);
            });
        }
    }
}


var Process = Process || {};

// Supported Action input parameter types
Process.Type = {
    Bool: "c:boolean",
    Float: "c:double", // Not a typo
    Decimal: "c:decimal",
    Int: "c:int",
    String: "c:string",
    DateTime: "c:dateTime",
    Guid: "c:guid",
    EntityReference: "a:EntityReference",
    OptionSet: "a:OptionSetValue",
    Money: "a:Money",
    Entity: "a:Entity",
    EntityCollection: "a:EntityCollection"
}

// inputParams: Array of parameters to pass to the Action. Each param object should contain key, value, and type.
// successCallback: Function accepting 1 argument, which is an array of output params. Access values like: params["key"]
// errorCallback: Function accepting 1 argument, which is the string error message. Can be null.
// Unless the Action is global, you must specify a 'Target' input parameter as EntityReference
// actionName is required
Process.callAction = function (actionName, inputParams, successCallback, errorCallback, url) {
    var ns = {
        "": "http://schemas.microsoft.com/xrm/2011/Contracts/Services",
        ":s": "http://schemas.xmlsoap.org/soap/envelope/",
        ":a": "http://schemas.microsoft.com/xrm/2011/Contracts",
        ":i": "http://www.w3.org/2001/XMLSchema-instance",
        ":b": "http://schemas.datacontract.org/2004/07/System.Collections.Generic",
        ":c": "http://www.w3.org/2001/XMLSchema",
        ":d": "http://schemas.microsoft.com/xrm/2011/Contracts/Services",
        ":e": "http://schemas.microsoft.com/2003/10/Serialization/",
        ":f": "http://schemas.microsoft.com/2003/10/Serialization/Arrays",
        ":g": "http://schemas.microsoft.com/crm/2011/Contracts",
        ":h": "http://schemas.microsoft.com/xrm/2011/Metadata",
        ":j": "http://schemas.microsoft.com/xrm/2011/Metadata/Query",
        ":k": "http://schemas.microsoft.com/xrm/2013/Metadata",
        ":l": "http://schemas.microsoft.com/xrm/2012/Contracts",
        //":c": "http://schemas.microsoft.com/2003/10/Serialization/" // Conflicting namespace for guid... hardcoding in the _getXmlValue bit
    };

    var requestXml = "<s:Envelope";

    // Add all the namespaces
    for (var i in ns) {
        requestXml += " xmlns" + i + "='" + ns[i] + "'";
    }

    requestXml += ">" +
          "<s:Body>" +
            "<Execute>" +
              "<request>";

    if (inputParams != null && inputParams.length > 0) {
        requestXml += "<a:Parameters>";

        // Add each input param
        for (var i = 0; i < inputParams.length; i++) {
            var param = inputParams[i];

            var value = Process._getXmlValue(param.key, param.type, param.value);

            requestXml += value;
        }

        requestXml += "</a:Parameters>";
    }
    else {
        requestXml += "<a:Parameters />";
    }

    requestXml += "<a:RequestId i:nil='true' />" +
                "<a:RequestName>" + actionName + "</a:RequestName>" +
              "</request>" +
            "</Execute>" +
          "</s:Body>" +
        "</s:Envelope>";

    Process._callActionBase(requestXml, successCallback, errorCallback, url);
}

// Runs the specified workflow for a particular record
// successCallback and errorCallback accept no arguments
// workflowId, and recordId are required
Process.callWorkflow = function (workflowId, recordId, successCallback, errorCallback, url) {
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
        if (req.readyState == 4) {
            if (req.status == 200) {
                if (successCallback) {
                    successCallback();
                }
            }
            else {
                if (errorCallback) {
                    errorCallback();
                }
            }
        }
    };

    req.send(request);
}

// Pops open the specified dialog process for a particular record
// dialogId, entityName, and recordId are required
// callback fires even if the dialog is closed/cancelled
Process.callDialog = function (dialogId, entityName, recordId, callback, url) {
    tryShowDialog("/cs/dialog/rundialog.aspx?DialogId=%7b" + dialogId + "%7d&EntityName=" + entityName + "&ObjectId=" + recordId, 600, 400, callback, url);

    // Function copied from Alert.js v1.0 https://alertjs.codeplex.com
    function tryShowDialog(url, width, height, callback, baseUrl) {
        width = width || Alert._dialogWidth;
        height = height || Alert._dialogHeight;

        var isOpened = false;

        try {
            // Web (IE, Chrome, FireFox)
            var Mscrm = Mscrm && Mscrm.CrmDialog && Mscrm.CrmUri && Mscrm.CrmUri.create ? Mscrm : parent.Mscrm;
            if (Mscrm && Mscrm.CrmDialog && Mscrm.CrmUri && Mscrm.CrmUri.create) {
                // Use CRM light-box (unsupported)
                var crmUrl = Mscrm.CrmUri.create(url);
                var dialogwindow = new Mscrm.CrmDialog(crmUrl, window, width, height);

                // Allows for opening non-webresources (e.g. dialog processes) - CRM messes up when it's not a web resource (unsupported)
                if (!crmUrl.get_isWebResource()) {
                    crmUrl.get_isWebResource = function () { return true; }
                }

                // Open the lightbox
                dialogwindow.show();
                isOpened = true;

                // Make sure when the dialog is closed, the callback is fired
                // This part's all pretty unsupported, hence the try-catches
                // If you can avoid using a callback, best not to use one
                if (callback) {
                    try {
                        // Get the lightbox iframe (unsupported)
                        var $frame = parent.$("#InlineDialog_Iframe");
                        if ($frame.length == 0) { $frame = parent.parent.$("#InlineDialog_Iframe"); }
                        $frame.load(function () {
                            try {
                                // Override the CRM closeWindow function (unsupported)
                                var frameDoc = $frame[0].contentWindow;
                                var closeEvt = frameDoc.closeWindow; // OOTB close function
                                frameDoc.closeWindow = function () {
                                    // Bypasses onunload event on dialogs to prevent "are you sure..." (unsupported - doesn't work with 2015 SP1)
                                    try { frameDoc.Mscrm.GlobalVars.$B = false; } catch (e) { }

                                    // Fire the callback and close
                                    callback();
                                    try { closeEvt(); } catch (e) { }
                                }
                            } catch (e) { }
                        });
                    } catch (e) { }
                }
            }
        } catch (e) { }

        try {
            // Outlook
            if (!isOpened && window.showModalDialog) {
                // If lightbox fails, use window.showModalDialog
                baseUrl = baseUrl || Xrm.Page.context.getClientUrl();
                var params = "dialogWidth:" + width + "px; dialogHeight:" + height + "px; status:no; scroll:no; help:no; resizable:yes";

                window.showModalDialog(baseUrl + url, window, params);
                if (callback) {
                    callback();
                }

                isOpened = true;
            }
        } catch (e) { }

        return isOpened;
    }
}

Process._emptyGuid = "00000000-0000-0000-0000-000000000000";

// This can be used to execute custom requests if needed - useful for me testing the SOAP :)
Process._callActionBase = function (requestXml, successCallback, errorCallback, url) {
    if (url == null) {
        url = Xrm.Page.context.getClientUrl();
    }

    var req = new XMLHttpRequest();
    req.open("POST", url + "/XRMServices/2011/Organization.svc/web", true);
    req.setRequestHeader("Accept", "application/xml, text/xml, */*");
    req.setRequestHeader("Content-Type", "text/xml; charset=utf-8");
    req.setRequestHeader("SOAPAction", "http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Execute");

    req.onreadystatechange = function () {
        if (req.readyState == 4) {
            if (req.status == 200) {
                // If there's no successCallback we don't need to check the outputParams
                if (successCallback) {
                    // Yucky but don't want to risk there being multiple 'Results' nodes or something
                    var resultsNode = req.responseXML.childNodes[0].childNodes[0].childNodes[0].childNodes[0].childNodes[1]; // <a:Results>

                    // Action completed successfully - get output params
                    var responseParams = Process._getChildNodes(resultsNode, "a:KeyValuePairOfstringanyType");

                    var outputParams = {};
                    for (i = 0; i < responseParams.length; i++) {
                        var attrNameNode = Process._getChildNode(responseParams[i], "b:key");
                        var attrValueNode = Process._getChildNode(responseParams[i], "b:value");

                        var attributeName = Process._getNodeTextValue(attrNameNode);
                        var attributeValue = Process._getValue(attrValueNode);

                        // v1.0 - Deprecated method using key/value pair and standard array
                        //outputParams.push({ key: attributeName, value: attributeValue.value });

                        // v2.0 - Allows accessing output params directly: outputParams["Target"].attributes["new_fieldname"];
                        outputParams[attributeName] = attributeValue.value;

                        /*
                        RETURN TYPES:
                            DateTime = Users local time (JavaScript date)
                            bool = true or false (JavaScript boolean)
                            OptionSet, int, decimal, float, etc = 1 (JavaScript number)
                            guid = string
                            EntityReference = { id: "guid", name: "name", entityType: "account" }
                            Entity = { logicalName: "account", id: "guid", attributes: {}, formattedValues: {} }
                            EntityCollection = [{ logicalName: "account", id: "guid", attributes: {}, formattedValues: {} }]
    
                        Attributes for entity accessed like: entity.attributes["new_fieldname"].value
                        For entityreference: entity.attributes["new_fieldname"].value.id
                        Make sure attributes["new_fieldname"] is not null before using .value
                        Or use the extension method entity.get("new_fieldname") to get the .value
                        Also use entity.formattedValues["new_fieldname"] to get the string value of optionsetvalues, bools, moneys, etc
                        */
                    }

                    // Make sure the callback accepts exactly 1 argument - use dynamic function if you want more
                    successCallback(outputParams);
                }
            }
            else {
                // Error has occured, action failed
                if (errorCallback) {
                    var message = null;
                    var traceText = null;
                    try {
                        message = Process._getNodeTextValueNotNull(req.responseXML.getElementsByTagName("Message"));
                        traceText = Process._getNodeTextValueNotNull(req.responseXML.getElementsByTagName("TraceText"));
                    } catch (e) { }
                    if (message == null) { message = "Error executing Action. Check input parameters or contact your CRM Administrator"; }
                    errorCallback(message, traceText);
                }
            }
        }
    };

    req.send(requestXml);
}

// Get only the immediate child nodes for a specific tag, otherwise entitycollections etc mess it up
Process._getChildNodes = function (node, childNodesName) {
    var childNodes = [];

    for (var i = 0; i < node.childNodes.length; i++) {
        if (node.childNodes[i].tagName == childNodesName) {
            childNodes.push(node.childNodes[i]);
        }
    }

    // Chrome uses just 'Results' instead of 'a:Results' etc
    if (childNodes.length == 0 && childNodesName.indexOf(":") !== -1) {
        childNodes = Process._getChildNodes(node, childNodesName.substring(childNodesName.indexOf(":") + 1));
    }

    return childNodes;
}

// Get a single child node for a specific tag
Process._getChildNode = function (node, childNodeName) {
    var nodes = Process._getChildNodes(node, childNodeName);

    if (nodes != null && nodes.length > 0) { return nodes[0]; }
    else { return null; }
}

// Gets the first not null value from a collection of nodes
Process._getNodeTextValueNotNull = function (nodes) {
    var value = "";

    for (var i = 0; i < nodes.length; i++) {
        if (value === "") {
            value = Process._getNodeTextValue(nodes[i]);
        }
    }

    return value;
}

// Gets the string value of the XML node
Process._getNodeTextValue = function (node) {
    if (node != null) {
        var textNode = node.firstChild;
        if (textNode != null) {
            return textNode.textContent || textNode.nodeValue || textNode.data || textNode.text;
        }
    }

    return "";
}

// Gets the value of a parameter based on its type, can be recursive for entities
Process._getValue = function (node) {
    var value = null;
    var type = null;

    if (node != null) {
        type = node.getAttribute("i:type") || node.getAttribute("type");

        // If the parameter/attribute is null, there won't be a type either
        if (type != null) {
            // Get the part after the ':' (since Chrome doesn't have the ':')
            var valueType = type.substring(type.indexOf(":") + 1).toLowerCase();

            if (valueType == "entityreference") {
                // Gets the lookup object
                var attrValueIdNode = Process._getChildNode(node, "a:Id");
                var attrValueEntityNode = Process._getChildNode(node, "a:LogicalName");
                var attrValueNameNode = Process._getChildNode(node, "a:Name");

                var lookupId = Process._getNodeTextValue(attrValueIdNode);
                var lookupName = Process._getNodeTextValue(attrValueNameNode);
                var lookupEntity = Process._getNodeTextValue(attrValueEntityNode);

                value = new Process.EntityReference(lookupEntity, lookupId, lookupName);
            }
            else if (valueType == "entity") {
                // Gets the entity data, and all attributes
                value = Process._getEntityData(node);
            }
            else if (valueType == "entitycollection") {
                // Loop through each entity, returns each entity, and all attributes
                var entitiesNode = Process._getChildNode(node, "a:Entities");
                var entityNodes = Process._getChildNodes(entitiesNode, "a:Entity");

                value = [];
                if (entityNodes != null && entityNodes.length > 0) {
                    for (var i = 0; i < entityNodes.length; i++) {
                        value.push(Process._getEntityData(entityNodes[i]));
                    }
                }
            }
            else if (valueType == "aliasedvalue") {
                // Gets the actual data type of the aliased value
                // Key for these is "alias.fieldname"
                var aliasedValue = Process._getValue(Process._getChildNode(node, "a:Value"));
                if (aliasedValue != null) {
                    value = aliasedValue.value;
                    type = aliasedValue.type;
                }
            }
            else {
                // Standard fields like string, int, date, money, optionset, float, bool, decimal
                // Output will be string, even for number fields etc
                var stringValue = Process._getNodeTextValue(node);

                if (stringValue != null) {
                    switch (valueType) {
                        case "datetime":
                            value = new Date(stringValue);
                            break;
                        case "int":
                        case "money":
                        case "optionsetvalue":
                        case "double": // float
                        case "decimal":
                            value = Number(stringValue);
                            break;
                        case "boolean":
                            value = stringValue.toLowerCase() === "true";
                            break;
                        default:
                            value = stringValue;
                    }
                }
            }
        }
    }

    return new Process.Attribute(value, type);
}

Process._getEntityData = function (entityNode) {
    var value = null;

    var entityAttrsNode = Process._getChildNode(entityNode, "a:Attributes");
    var entityIdNode = Process._getChildNode(entityNode, "a:Id");
    var entityLogicalNameNode = Process._getChildNode(entityNode, "a:LogicalName");
    var entityFormattedValuesNode = Process._getChildNode(entityNode, "a:FormattedValues");

    var entityLogicalName = Process._getNodeTextValue(entityLogicalNameNode);
    var entityId = Process._getNodeTextValue(entityIdNode);
    var entityAttrs = Process._getChildNodes(entityAttrsNode, "a:KeyValuePairOfstringanyType");

    value = new Process.Entity(entityLogicalName, entityId);

    // Attribute values accessed via entity.attributes["new_fieldname"]
    if (entityAttrs != null && entityAttrs.length > 0) {
        for (var i = 0; i < entityAttrs.length; i++) {

            var attrNameNode = Process._getChildNode(entityAttrs[i], "b:key")
            var attrValueNode = Process._getChildNode(entityAttrs[i], "b:value");

            var attributeName = Process._getNodeTextValue(attrNameNode);
            var attributeValue = Process._getValue(attrValueNode);

            value.attributes[attributeName] = attributeValue;
        }
    }

    // Formatted values accessed via entity.formattedValues["new_fieldname"]
    for (var j = 0; j < entityFormattedValuesNode.childNodes.length; j++) {
        var foNode = entityFormattedValuesNode.childNodes[j];

        var fNameNode = Process._getChildNode(foNode, "b:key")
        var fValueNode = Process._getChildNode(foNode, "b:value");

        var fName = Process._getNodeTextValue(fNameNode);
        var fValue = Process._getNodeTextValue(fValueNode);

        value.formattedValues[fName] = fValue;
    }

    return value;
}

Process._getXmlValue = function (key, dataType, value) {
    var xml = "";
    var xmlValue = "";

    var extraNamespace = "";

    // Check the param type to determine how the value is formed
    switch (dataType) {
        case Process.Type.String:
            xmlValue = Process._htmlEncode(value) || ""; // Allows fetchXml strings etc
            break;
        case Process.Type.DateTime:
            xmlValue = value.toISOString() || "";
            break;
        case Process.Type.EntityReference:
            xmlValue = "<a:Id>" + (value.id || "") + "</a:Id>" +
                  "<a:LogicalName>" + (value.entityType || "") + "</a:LogicalName>" +
                  "<a:Name i:nil='true' />";
            break;
        case Process.Type.OptionSet:
        case Process.Type.Money:
            xmlValue = "<a:Value>" + (value || 0) + "</a:Value>";
            break;
        case Process.Type.Entity:
            xmlValue = Process._getXmlEntityData(value);
            break;
        case Process.Type.EntityCollection:
            if (value != null && value.length > 0) {
                var entityCollection = "";
                for (var i = 0; i < value.length; i++) {
                    var entityData = Process._getXmlEntityData(value[i]);
                    if (entityData !== null) {
                        entityCollection += "<a:Entity>" + entityData + "</a:Entity>";
                    }
                }
                if (entityCollection !== null && entityCollection !== "") {
                    xmlValue = "<a:Entities>" + entityCollection + "</a:Entities>" +
                        "<a:EntityName i:nil='true' />" +
                        "<a:MinActiveRowVersion i:nil='true' />" +
                        "<a:MoreRecords>false</a:MoreRecords>" +
                        "<a:PagingCookie i:nil='true' />" +
                        "<a:TotalRecordCount>0</a:TotalRecordCount>" +
                        "<a:TotalRecordCountLimitExceeded>false</a:TotalRecordCountLimitExceeded>";
                }
            }
            break;
        case Process.Type.Guid:
            // I don't think guid fields can even be null?
            xmlValue = value || Process._emptyGuid;

            // This is a hacky fix to get guids working since they have a conflicting namespace :(
            extraNamespace = " xmlns:c='http://schemas.microsoft.com/2003/10/Serialization/'";
            break;
        default: // bool, int, double, decimal
            xmlValue = value != undefined ? value : null;
            break;
    }

    xml = "<a:KeyValuePairOfstringanyType>" +
            "<b:key>" + key + "</b:key>" +
            "<b:value i:type='" + dataType + "'" + extraNamespace;

    // nulls crash if you have a non-self-closing tag
    if (xmlValue === null || xmlValue === "") {
        xml += " i:nil='true' />";
    }
    else {
        xml += ">" + xmlValue + "</b:value>";
    }

    xml += "</a:KeyValuePairOfstringanyType>";

    return xml;
}

Process._getXmlEntityData = function (entity) {
    var xml = null;

    if (entity != null) {
        var attrXml = "";

        for (field in entity.attributes) {
            var a = entity.attributes[field];
            var aXml = Process._getXmlValue(field, a.type, a.value);

            attrXml += aXml;
        }

        if (attrXml !== "") {
            xml = "<a:Attributes>" + attrXml + "</a:Attributes>";
        }
        else {
            xml = "<a:Attributes />";
        }

        xml += "<a:EntityState i:nil='true' />" +
            "<a:FormattedValues />" +
            "<a:Id>" + entity.id + "</a:Id>" +
            "<a:KeyAttributes />" +
            "<a:LogicalName>" + entity.logicalName + "</a:LogicalName>" +
            "<a:RelatedEntities />" +
            "<a:RowVersion i:nil='true' />";
    }

    return xml;
}

Process._htmlEncode = function (s) {
    if (typeof s !== "string") { return s; }

    return s.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
}

Process.Entity = function (logicalName, id, attributes) {
    this.logicalName = logicalName || "";
    this.attributes = attributes || {};
    this.formattedValues = {};
    this.id = id || Process._emptyGuid;
}

// Gets the value of the attribute without having to check null
Process.Entity.prototype.get = function (key) {
    var a = this.attributes[key];
    if (a != null) {
        return a.value;
    }

    return null;
}

Process.EntityReference = function (entityType, id, name) {
    this.id = id || Process._emptyGuid;
    this.name = name || "";
    this.entityType = entityType || "";
}

Process.Attribute = function (value, type) {
    this.value = value != undefined ? value : null;
    this.type = type || "";
}