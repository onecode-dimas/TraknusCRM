function ribbon_AddNewActivities(){
    //alert("ribbon add new activities");
    var workflowId = 'D8D5740E-ED18-451C-B66C-4400395EF545';
    var workflowName = 'Add New Activities';
	ExecuteWorkflow(workflowId, workflowName, function ()
	{
		RefreshForm();
	});
}

function ribbon_MarkComplete(){
    //alert("ribbon mark Complete");
    /*var workflowId = '36082789-FCEA-E711-940C-0050569355F1';
	var workflowName = 'Assign to PDH';
	ExecuteWorkflow(workflowId, workflowName, function ()
	{
		RefreshForm();
	});*/

    //update status to complete
    Xrm.Page.getAttribute("tss_status").setValue(865920001);
    Xrm.Page.getAttribute("tss_status").setSubmitMode("always");
    MarkComplete_VisitPart();
    RefreshForm();
}

function MarkComplete_VisitPart()
{
    var headerId = Xrm.Page.data.entity.getId();
    
    if (headerId != null) {
        XrmServiceToolkit.Rest.RetrieveMultiple("tss_partactivitylinesSet", "?$select=tss_activities,tss_partactivitylinesId&$filter=tss_PartActivityHeader/Id eq (guid'" + headerId + "') and tss_ActivitiesType/Value eq 865920011", function (results) {
            for (var i = 0; i < results.length; i++) {
                var tss_activities = results[i].tss_activities;
                var tss_partactivitylinesId = results[i].tss_partactivitylinesId;

                XrmServiceToolkit.Rest.RetrieveMultiple("tss_PotentialProspectPartSet", "?$select=tss_PotentialProspectPartId&$filter=tss_PotentialProspectId eq '" + tss_activities + "'", function (results) {
                    for (var i = 0; i < results.length; i++) {
                        var tss_PotentialProspectPartId = results[i].tss_PotentialProspectPartId;

                        var entity = {};
                        entity.tss_FollowUp = true;

                        XrmServiceToolkit.Rest.Update(tss_PotentialProspectPartId, entity, "tss_PotentialProspectPartSet", function () {
                            //Success - No Return Data - Do Something
                        }, function (error) {
                            Xrm.Utility.alertDialog(error.message);
                        }, false);
                    }
                }, function (error) {
                    Xrm.Utility.alertDialog(error.message);
                }, function () {
                    //On Complete - Do Something
                }, false);
            }
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, function () {
            //On Complete - Do Something
        }, false);
    }
}

function ActualDate_onChange(){
    try{
        var currentDateTime = new Date();
        var ActualDate = Xrm.Page.getAttribute("tss_actualdate").getValue();
        if(ActualDate !=null){
            if (ActualDate > currentDateTime) {
                alert("Actual Date must be Less than Today");
                Xrm.Page.getAttribute("tss_actualdate").setValue(currentDateTime);
                Xrm.Page.getAttribute("tss_actualdate").setSubmitMode("always");
            }
            else {
                var tss_MaxBackdated;
                XrmServiceToolkit.Rest.RetrieveMultiple("tss_sparepartsetupSet", "?$select=tss_MaxBackdated", function (results) {
                    if(results.length > 0) {
                        var tss_MaxBackdated = results[0].tss_MaxBackdated;

                        if ((currentDateTime.getDate() > tss_MaxBackdated && currentDateTime.getMonth() == ActualDate.getMonth() && currentDateTime.getDate() > tss_MaxBackdated) || ((currentDateTime.getDate() <= tss_MaxBackdated) && ((currentDateTime.getMonth() == 0 && ActualDate.getMonth() == 11) || (currentDateTime.getMonth() - ActualDate.getMonth() <= 1)))) {
                        }
                        else {
                            alert("Can't insert back date!");
                            Xrm.Page.getAttribute("tss_actualdate").setValue(currentDateTime);
                            Xrm.Page.getAttribute("tss_actualdate").setSubmitMode("always");
                        }
                    }
                }, function (error) {
                    Xrm.Utility.alertDialog(error.message);
                }, function () {
                    //On Complete - Do Something
                }, false);
            }
        } 
    }catch(e){
        alert('Failed to set actual date: ' + e.message)
    }
}

function ExecuteWorkflow(workflowId, workflowName,successCallback,failedCallback) {
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
        req.onreadystatechange = function () { AssignResponse(req, workflowName,successCallback,failedCallback); };
        req.send(request);
    }
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
    check.push(function() {
        return formType !== formStatus.Create;
    });

    var boolArrayCheck = function(predicateArray) {
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

function setDisabledField(field, value) {
    Xrm.Page.getControl(field).setDisabled(value);
}

function DisableForm() {
    if (Xrm.Page.ui.getFormType() > 1 && Xrm.Page.getAttribute("tss_status").getValue() == 865920001) {

        setDisabledField("tss_location", true);
        setDisabledField("tss_actualdate", true);
        setDisabledField("tss_actualstarttime", true);
        setDisabledField("tss_actualendtime", true);
        setDisabledField("tss_meetwith", true);
        setDisabledField("tss_description", true);

        //Xrm.Page.ui.controls.forEach(function (control, index) {
        //    control.setDisabled(true);
        //});

        var sopartlinesGrid = 'PartAcitivtyLines';
        var subgridsLoaded = false;
        if ($("div[id$='" + sopartlinesGrid + "']").length > 0 && !subgridsLoaded) {
            intervalId = setInterval(function () {
                var subgridsArr = Xrm.Page.getControl(function (control, index) {
                    return control.getControlType() == 'subgrid';
                });
                subgridsArr.forEach(function (control, index) {
                    removeButtonsFromSubGrid(control);
                });
            }, 500);
        }
    }
}

function removeButtonsFromSubGrid(subgridControl) {
    if (intervalId) {
        $('#' + subgridControl.getName() + '_addImageButton').css('display', 'none');
        $('#' + subgridControl.getName() + '_openAssociatedGridViewImageButton').css('display', 'none');
    }
}


function SetPSSLookup() {
    addPSSFilter();
}

function addPSSFilter() {
    var viewId = "{6fd72744-3676-41d4-8003-ae4cde9ac282}";
    var entityName = "systemuser";
    var viewDisplayName = "PSS";

    var fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>" +
                        "<entity name='systemuser'>" +
                            "<attribute name='fullname' />" +
                            "<attribute name='title' />" +
                            "<attribute name='systemuserid' />" +
                            " <order attribute='fullname' descending='false' />" +
                            "<filter type='and'>" +
                                "<condition attribute='isdisabled' operator='eq' value='0'  />" +
                                "<condition attribute='title' operator='eq' value='PSS'  />" +
                            "</filter>" +
                          "</entity>" +
                    "</fetch>";

    var layoutXml = "<grid name='resultset' " +
                    "object='1' " +
                    "jump='systemuserid' " +
                    "select='1' " +
                    "icon='1' " +
                    "preview='1'>" +
                    "<row name='result' " +
                    "id='systemuserid'>" +
                    "<cell name='fullname' " +
                    "width='200' />" +
                    "<cell name='title' " +
                    "width='200' />" +
                    "</row>" +
                    "</grid>";

    Xrm.Page.getControl("tss_pss").addCustomView(viewId, entityName, viewDisplayName, fetchXml, layoutXml, true);
}

function setActualTime() {
    if (Xrm.Page.getAttribute("tss_actualdate").getValue() != null) {
        Xrm.Page.getAttribute("tss_actualstarttime").setValue(Xrm.Page.getAttribute("tss_actualdate").getValue());
        Xrm.Page.getAttribute("tss_actualstarttime").setSubmitMode("always");
        Xrm.Page.getAttribute("tss_actualendtime").setValue(Xrm.Page.getAttribute("tss_actualdate").getValue());
        Xrm.Page.getAttribute("tss_actualendtime").setSubmitMode("always");
    }
}

var intervalId = true;
function makeReadOnly() {
    var sopartlinesGrid = 'PartAcitivtyLines';
    try {
        var subgridsLoaded = false;
        if ($("div[id$='" + sopartlinesGrid + "']").length > 0 && !subgridsLoaded) {
            intervalId = setInterval(function () {
                var subgridsArr = Xrm.Page.getControl(function (control, index) {
                    return control.getControlType() == 'subgrid';
                });
                subgridsArr.forEach(function (control, index) {
                    removeButtonsFromSubGrid(control);
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
        clearInterval(intervalId);
    }
}