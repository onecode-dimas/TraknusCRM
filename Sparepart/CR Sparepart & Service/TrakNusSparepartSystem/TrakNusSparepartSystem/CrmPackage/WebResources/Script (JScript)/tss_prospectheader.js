//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// ON LOAD Additional Refresh Grid
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function CheckProspectTotalAmountOnRefreshGrid(gridElementId) {
    try {
        AddEventOnRefreshGrid(gridElementId, function () {
            GetProspectTotalAmountRelatedData(Xrm.Page.data.entity.getId(), function (totalAmount, underMinPrice, pipeline) {
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
                compareAttribute("tss_pipelinephase", pipeline);
                compareAttribute("tss_totalamount", Number(totalAmount));
                compareAttribute("tss_underminimumprice", underMinPrice);
            });
        });
    } catch (e) {
        alert("CheckProspectTotalAmountOnRefreshGrid: " + e.message);
    }
}
function AddEventOnRefreshGrid(gridElementId, onRefreshFunction) {
    try {
        var elem = document.getElementById(gridElementId);
        if (elem !== null) {
            var ctrl = document.getElementById(gridElementId).control;
            ctrl.add_onRefresh(function () {
                onRefreshFunction();
            });
        } else {
            setTimeout(function () {
                AddEventOnRefreshGrid(gridElementId, onRefreshFunction);
            }, 1000);
        }
    } catch (e) {
        alert("AddEventOnRefreshGrid: " + e.message);
    }
}
function GetProspectTotalAmountRelatedData(prospectId, successCallback) {
    XrmServiceToolkit.Rest.Retrieve(
        prospectId,
        "tss_prospectpartheaderSet",
        null, null,
        function (result) {
            var tss_TotalAmount = result.tss_Totalamount.Value;
            var tss_UnderMinimumPrice = result.tss_Underminimumprice;
            var tss_pipelinephase = result.tss_Pipelinephase.Value;
            successCallback(tss_TotalAmount, tss_UnderMinimumPrice, tss_pipelinephase);
        },
        function (error) {
            console.log(error.message);
        },
        true
    );
}
function GetQuotationRelatedData() {
    var prosID = Xrm.Page.data.entity.getId();
    if (prosID != null && prosID != undefined && prosID != '') {
        var quotationpartGrid = 'QuotationPart';
        var tab_quotationGrid = 'tab_3';
        var section_quotationGrid = 'tab_3_section_1';
        var OrderReason = Xrm.Page.getAttribute("tss_orderreason");
        //Disable Quotation Subgrid
        $('#' + quotationpartGrid + '_addImageButton').css('display', 'none');
        $('#' + quotationpartGrid + '_openAssociatedGridViewImageButton').css('display', 'none');
        XrmServiceToolkit.Rest.RetrieveMultiple(
            "tss_quotationpartheaderSet",
            "$select=*&$orderby=CreatedOn desc&$filter=tss_Prospectlink/Id eq (guid'" + prosID + "')",
            function (result) {
                if (result.length > 0) {
                    Xrm.Page.ui.tabs.get(tab_quotationGrid).setVisible(true);
                    //Xrm.Page.ui.tabs.get(tab_quotationGrid).sections.get(section_quotationGrid).setVisible(true);
                } else {
                    Xrm.Page.ui.tabs.get(tab_quotationGrid).setVisible(false);
                }
            }, function (error) {
                alert(error.message);
            }, function onComplete() {
                //alert("DONE.");
            }, false
        );
    }
}
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// Helper
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//Disable form
var pipelinephase_inquiry = 865920000;
var statusreason_lost = 865920002;
var userID = Xrm.Page.context.getUserId();

function retrieveProspectLines() {
    var prosid = Xrm.Page.data.entity.getId();
    var id;
    var elem;
    XrmServiceToolkit.Rest.RetrieveMultiple(
        "tss_prospectpartlinesSet",
        "$select=*&$filter=tss_ProspectPartHeader/Id eq (guid'" + prosid + "')",
    function (result) {
        for (var i = 0; i < result.length; i++) {
            id = "gridBodyTable_gridDelBtn_" + i;
            elem = document.getElementById(id);
            elem.parentNode.removeChild(elem);
        }

    }, function (error) {
        alert(error.message);
    }, function onComplete() {
        //alert(" records should have been retrieved.");
    }, false);
}

function doesControlHaveAttribute(control) {
    var controlType = control.getControlType();
    return controlType != "iframe" && controlType != "webresource" && controlType != "subgrid";
}
function disableFormFields(onOff) {
    Xrm.Page.ui.controls.forEach(function (control, index) {
        if (doesControlHaveAttribute(control)) {
            control.setDisabled(onOff);
        }
    });
}
function setupForm() {
    var flagPSS = false;
    XrmServiceToolkit.Rest.Retrieve(
        userID,
        "SystemUserSet",
        null, null,
        function (result) {
            if (result.Title == 'PSS') flagPSS = true;
        }, function (error) {
            alert("Failed retrieve user on setupForm");
        }, false
    );
    if (Xrm.Page.getAttribute("tss_pipelinephase").getValue() != pipelinephase_inquiry) {
        makeReadOnly();
        disableFormFields(true);
    }
    if (Xrm.Page.getAttribute("tss_statusreason").getValue() == statusreason_lost) {
        makeReadOnly();
        disableFormFields(true);
    }
}

function makeReadOnly() {
    var prospectlinesGrid = 'ProspectPartLines';
    var quotationpartGrid = 'QuotationPart';
    try {
        var subgridsLoaded = false;
        Xrm.Page.ui.controls.get().forEach(function (control, index) {
            if (control.setDisabled && Xrm.Page.ui.getFormType() != 3) {
                control.setDisabled(true);
            }
            else {
                removeButtonsFromSubGrid(control);
                subgridsLoaded = true;
            }
        });
        if ($("div[id$='" + prospectlinesGrid + "']").length > 0 && !subgridsLoaded) {
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
        $('#' + subgridControl.getName() + '_openAssociatedGridViewImageButton').css('display', 'none');
        retrieveProspectLines();
        //gridBodyTable_gridDelBtn_0
        //var elem = document.getElementById("deleteAction");
        //elem.parentNode.removeChild(elem);
        //$('#deleteAction').remove();
        clearInterval(intervalId);
    }
}

var OrderReason = {
    Overhaul: 865920000,
    Others: 865920001
};

if (!window.showModalDialog) {
    window.showModalDialog = function (arg1, arg2, arg3) {

        var w;
        var h;
        var resizable = "no";
        var scroll = "no";
        var status = "no";

        // get the modal specs
        var mdattrs = arg3.split(";");
        for (i = 0; i < mdattrs.length; i++) {
            var mdattr = mdattrs[i].split(":");

            var n = mdattr[0];
            var v = mdattr[1];
            if (n) { n = n.trim().toLowerCase(); }
            if (v) { v = v.trim().toLowerCase(); }

            if (n == "dialogheight") {
                h = v.replace("px", "");
            } else if (n == "dialogwidth") {
                w = v.replace("px", "");
            } else if (n == "resizable") {
                resizable = v;
            } else if (n == "scroll") {
                scroll = v;
            } else if (n == "status") {
                status = v;
            }
        }

        var left = window.screenX + (window.outerWidth / 2) - (w / 2);
        var top = window.screenY + (window.outerHeight / 2) - (h / 2);
        var targetWin = window.open(arg1, arg1, 'toolbar=no, location=no, directories=no, status=' + status + ', menubar=no, scrollbars=' + scroll + ', resizable=' + resizable + ', copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
        targetWin.focus();
    };
}

function SetDefaultValue_TwoOptionField(fieldname) {
    var attr = Xrm.Page.getAttribute(fieldname);
    if (attr != null) {
        attr.setValue(1);
        attr.setValue(0);
        attr.setSubmitMode("always");
    }

}

function checkisDirty(fieldname) {
    var oppAttributes = Xrm.Page.getAttribute(fieldname);
    var listofDirtyAttri;
    if (oppAttributes != null) {
        for (var i in oppAttributes) {
            if (oppAttributes[i].getIsDirty()) {
                listofDirtyAttri += oppAttributes[i].getName() + "\n";
            }
        }
    }
    alert(listofDirtyAttri);
}

function forceSubmitDirtyFields() {
    //var message = "The following fields are dirty: \n";
    Xrm.Page.data.entity.attributes.forEach(function (attribute, index) {
        if (attribute.getIsDirty() == true) {
            attribute.setSubmitMode('always');
            //message += "\u2219 " + attribute.getName() + "\n";
        }
    });
    //alert(message);

}

function SetLookupNull(lookupAttribute) {
    var lookupObject = Xrm.Page.getAttribute(lookupAttribute);

    if (lookupObject != null) {
        Xrm.Page.getAttribute(lookupAttribute).setValue(null);
        Xrm.Page.getAttribute(lookupAttribute).setSubmitMode("always");
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
            if (workflowName != 'Create Quotation') {
                alert('Successfully executed ' + workflowName + '.');
            }
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

function SetVisibility(ControlName, Visible) {
    try {
        Xrm.Page.getControl(ControlName).setVisible(Visible);
    }
    catch (e) {
        throw new Error("SetVisibility : " + e.message);
    }
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FORM ON LOAD AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function Refresh() {
    Xrm.Page.getControl("QuotationPart").addOnLoad(GridOnloadFunction);
}
function GridOnloadFunction() {
    var save = true;
    Xrm.Page.data.refresh(save);
};
function onLoad() {

    var subgrid = Xrm.Page.ui.controls.get("QuotationPart");
    //var quotationpartGrid = 'QuotationPart';
    var OrderReason = Xrm.Page.getAttribute("tss_orderreason");
    //Disable Quotation Subgrid
    //$('#' + quotationpartGrid + '_addImageButton').css('display', 'none');
    //$('#' + quotationpartGrid + '_openAssociatedGridViewImageButton').css('display', 'none');
    GetQuotationRelatedData();
    CheckProspectTotalAmountOnRefreshGrid("ProspectPartLines");
    CheckProspectTotalAmountOnRefreshGrid("QuotationPart");

    setupForm();
    setOnLoad();
    preFilterLookupContact();

    if (OrderReason != null) {
        if (OrderReason.getValue() == OrderReason.Overhaul) {
            OrderReason.setSubmitMode("always");
            preFilterLookupUnit();
        }
    }
    //GetListOfDirtyFields();
    forceSubmitDirtyFields();
    hideUnit();
}

function hideUnit() {
    var unit = Xrm.Page.getAttribute("tss_unit").getValue();
    if (unit == null) {
        Xrm.Page.ui.controls.get("tss_unit").setVisible(false);
    }
    else {
        Xrm.Page.ui.controls.get("tss_unit").setVisible(true);
    }
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// RIBBON AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function btn_CancelProspect() {
    var approve = confirm("Are you sure to Cancel Prospect ?");
    if (approve) {
        var guid = Xrm.Page.data.entity.getId();
        var entityName = Xrm.Page.data.entity.getEntityName();
        if (guid == null) {
            alert('Record not found!');
        }
        else {
            //alert(guid);
            var DialogGUID = "270BDC04-BDE2-467E-A784-2D12D9049E79";

            var dynamicServerUrl;
            if (Xrm.Page.context.getClientUrl !== undefined) {
                dynamicServerUrl = Xrm.Page.context.getClientUrl();
            } else {
                dynamicServerUrl = Xrm.Page.context.getServerUrl();
            }

            var serverUrl = dynamicServerUrl+"/cs/dialog/rundialog.aspx?DialogId=" + "{" + DialogGUID + "}" + "&EntityName=" + entityName + "&ObjectId=" + guid;
            //alert( serverUrl);
            window.showModalDialog(serverUrl, null, "dialogHeight:540px;dialogWidth:720px;center:yes;resizable:1;maximize:1;minimize:1;status:no;scroll:no");

            if (parent.opener != undefined) window.parent.close();
            else Xrm.Page.ui.close();
            //window.location.reload(true);
        }
    }
    //ACF1EE71-69B4-49E2-AD6C-2EF5DB36A549 <-- Cancel Reason
    //FEBF935C-CF53-4414-A789-C2E055648B2C  <-- Dialog Competitor
}

function disableButtonQuotation() {
    var statusQuotation = Xrm.Page.getAttribute("statuscode").getValue();
    if (statusQuotation == 167630001) {
        //Revise status
        //A value of true will result in the Enabling of the button, false will Disable it.
        return true;
    }
}

function btn_CreateQuotation() {
    try {
        var workflowId = '92A906F7-8827-4057-873B-22DFD3990ED1';
        var workflowName = 'Create Quotation';
        var prospectID = Xrm.Page.data.entity.getId();
        var active = 865920005;
        var finalapprove = 865920006;
        var lost = 865920007;
        var won = 865920008;
        var check = true;
        var status
        XrmServiceToolkit.Rest.RetrieveMultiple(
            "tss_prospectpartlinesSet",
            "$select=*&$filter=tss_ProspectPartHeader/Id eq (guid'" + prospectID + "')",
            function (result) {
                if (result.length > 0) {
                    //Checking quotation status active
                    XrmServiceToolkit.Rest.RetrieveMultiple(
                        "tss_quotationpartheaderSet",
                        "$select=tss_statusreason,tss_quotationpartheaderId&$filter=tss_Prospectlink/Id eq (guid'" + prospectID + "')",
                        function (result) {
                            if (result.length > 0) {
                                for (var i = 0; i < result.length; i++) {
                                    status = result[i].tss_statusreason.Value;
                                    if (status == active || status == finalapprove) {
                                        alert('Cannot create Quotation Part, because there is still available Quotation with status reason active/final approve/lost/won.');
                                        check = true;
                                        break;
                                    } else {
                                        check = false;
                                    }
                                }
                                if (!check) {
                                    ExecuteWorkflow(workflowId, workflowName, function () { OpenNewQuotationPartHeader(); });// Refresh(); //OpenNewQuotationPartHeader();

                                }
                            } else {
                                ExecuteWorkflow(workflowId, workflowName, function () { OpenNewQuotationPartHeader(); });// Refresh(); //OpenNewQuotationPartHeader();

                            }
                        }, function (error) {
                            alert(error.message);
                        }, function onComplete() {
                            //alert("DONE.");
                        }, false
                    );
                } else {
                    alert('Cannot create Quotation Part, please fill Prospect Part Lines first.');
                }
            },

            function (error) {
                alert(error.message);
            },

            function onComplete() {
                //alert("DONE.");
            }, false
        );

    } catch (e) {
        alert('Exception_btn_CreateQuotation: ' + e.message);
    }

}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// FIELD EVENT AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function onchangePSS() {
    var prosID = Xrm.Page.data.entity.getId();
    if (Xrm.Page.data.entity.attributes.get("tss_pss").getValue()[0] != null) {
        var userID = Xrm.Page.data.entity.attributes.get("tss_pss").getValue()[0].id;
        var entity = {};
        XrmServiceToolkit.Rest.Retrieve(
		userID,
		"SystemUserSet",
		null, null,
		function (result) {
		    entity.tss_Branch = { Id: result.BusinessUnitId.Id, LogicalName: "businessunit" };

		    XrmServiceToolkit.Rest.Update(
				prosID,
				entity,
				"tss_prospectpartheaderSet",
				function (result) {
				    alert("Successfully Updated Prospect Part Header onchange PSS");
				},
				function (error) {
				    alert("Failed to Update Prospect Part Header: " + error.message);
				},
				false
			);
		},
		function (error) {
		    alert("failed");
		},
		false
		);
        //RefreshForm();
    }
}

function onchangeCustomer() {
    var custlookup = Xrm.Page.getAttribute("tss_customer").getValue();
    var entity = {};
    var today = new Date();

    if (custlookup != null) {
        var custID = custlookup[0].id;

        XrmServiceToolkit.Rest.Retrieve(
            custID,
            "AccountSet",
            null, null,
            function (result) {
                if (result.ParentAccountId.Id != null) {
                    var parentid = result.ParentAccountId.Id;
                    var parentname = result.ParentAccountId.Name;
                    entity.tss_CustomerGroup = { Id: parentid, LogicalName: "account" };
                    var parent = new Array();
                    parent[0] = new Object();
                    parent[0].id = parentid;
                    parent[0].name = parentname;
                    parent[0].entityType = "account";
                    Xrm.Page.getAttribute("tss_customergroup").setValue(parent);
                    Xrm.Page.getAttribute("tss_customergroup").setSubmitMode("always");
                    Xrm.Page.getAttribute('tss_customergroup').controls.get(0).setVisible(true);
                }
                else {
                    Xrm.Page.getAttribute("tss_customergroup").setValue(null);
                    Xrm.Page.getAttribute("tss_customergroup").setSubmitMode("always");
                    //Set Visible Customer Group to false / hidden
                    Xrm.Page.getAttribute('tss_customergroup').controls.get(0).setVisible(false);
                }

                if (result.PrimaryContactId.Id != null) {
                    var contactid = result.PrimaryContactId.Id;
                    var contactname = result.PrimaryContactId.Name;
                    entity.tss_Contact = { Id: contactid, LogicalName: "contact" };
                    var contact = new Array();
                    contact[0] = new Object();
                    contact[0].id = contactid;
                    contact[0].name = contactname;
                    contact[0].entityType = "contact";
                    Xrm.Page.getAttribute("tss_contact").setValue(contact);
                    Xrm.Page.getAttribute("tss_contact").setSubmitMode("always");
                }
                else {
                    Xrm.Page.getAttribute("tss_contact").setValue(null);
                    Xrm.Page.getAttribute("tss_contact").setSubmitMode("always");
                }
                XrmServiceToolkit.Rest.RetrieveMultiple(
                    "ContractSet",
                    "$select=*&$orderby=CreatedOn desc&$filter=StateCode/Value eq 2 and trs_Customer/Id eq (guid'" + custID + "') and ActiveOn le datetime'" + today.toISOString() + "' and ExpiresOn ge datetime'" + today.toISOString() + "'",
                    function (result) {
                        if (result.length > 0) {
                            entity.tss_HaveContract = true;
                        }
                        else {
                            entity.tss_HaveContract = false;
                        }
                        Xrm.Page.getAttribute("tss_havecontract").setValue(entity.tss_HaveContract);
                        Xrm.Page.getAttribute("tss_havecontract").setSubmitMode("always");
                    }, function (error) {
                        alert('Have Contract ' + error.message);
                    }, function onComplete() {
                        //alert("DONE.");
                    }, false
                );
            },
            function (error) {
                alert("failed");
            }, false
        );
    }
    else {
        SetLookupNull("tss_customergroup");
        SetVisibility("tss_customergroup", false);
        SetLookupNull("tss_contact");
        SetLookupNull("tss_unit");
    }
}

function preFilterLookupPSS() {
    Xrm.Page.getControl("tss_pss")._control && Xrm.Page.getControl("tss_pss")._control.tryCompleteOnDemandInitialization && Xrm.Page.getControl("tss_pss")._control.tryCompleteOnDemandInitialization();
    Xrm.Page.getControl("tss_pss").addPreSearch(function () {
        addLookupFilterPSS();
    });
}

function addLookupFilterPSS() {
    var cust = Xrm.Page.getAttribute("tss_branch");
    var custObj = cust.getValue();
    if (custObj != null) { 	//<condition attribute='title' operator='eq' value='PSS'/>
        //"<filter type='and'><condition attribute='businessunitid' uitype='" + custObj[0].entityType + "' operator='eq' value='" + custObj[0].id + "'/><condition attribute='title' operator='eq' value='PSS'/></filter>";
        var fetchFilter = "<filter><condition attribute='title' operator='eq' value='PSS'/></filter>";
        Xrm.Page.getControl("tss_pss").addCustomFilter(fetchFilter);
    }
}

function preFilterLookupContact() {
    Xrm.Page.getControl("tss_contact")._control && Xrm.Page.getControl("tss_contact")._control.tryCompleteOnDemandInitialization && Xrm.Page.getControl("tss_contact")._control.tryCompleteOnDemandInitialization();
    Xrm.Page.getControl("tss_contact").addPreSearch(function () {
        addLookupFilterContact();
    });
}

function addLookupFilterContact() {
    var cust = Xrm.Page.getAttribute("tss_customer");
    var custObj = cust.getValue();

    if (custObj != null) {
        var fetchFilters = "<filter type='and'><condition attribute='parentcustomerid' uitype='" + custObj[0].entityType + "' operator='eq' value='" + custObj[0].id + "'/></filter>";
        Xrm.Page.getControl("tss_contact").addCustomFilter(fetchFilters);
    }
}

function preFilterLookupUnit() {
    Xrm.Page.getControl("tss_unit")._control && Xrm.Page.getControl("tss_unit")._control.tryCompleteOnDemandInitialization && Xrm.Page.getControl("tss_unit")._control.tryCompleteOnDemandInitialization();
    Xrm.Page.getControl("tss_unit").addPreSearch(function () {
        addLookupFilterUnit();
    });
}

function addLookupFilterUnit() {
    var cust = Xrm.Page.getAttribute("tss_customer");
    var custObj = cust.getValue();

    if (custObj != null) {
        var fetchFilters = "<filter type='and'><condition attribute='new_customercode' uitype='" + custObj[0].entityType + "' operator='eq' value='" + custObj[0].id + "'/></filter>";
        Xrm.Page.getControl("tss_unit").addCustomFilter(fetchFilters);
    }
}

function OnChange_OrderReason() {
    var OrderReason = Xrm.Page.getAttribute("tss_orderreason");

    if (OrderReason != null) {
        if (OrderReason.getValue() == OrderReason.Overhaul) {
            preFilterLookupUnit();
        }
    }
}
function totalAmount_onChange() {
    alert('YEAY UPDATE');
    Xrm.Page.data.refresh();
}
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// PUBLICS AREA
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function OpenNewQuotationPartHeader() {
    try {
        var qphID;
        var prosID = Xrm.Page.data.entity.getId();
        var windowOptions = {
            openInNewWindow: true
        };
        XrmServiceToolkit.Rest.RetrieveMultiple(
            "tss_quotationpartheaderSet",
            "$select=tss_quotationpartheaderId&$top=1&$orderby=CreatedOn desc&$filter=tss_Prospectlink/Id eq (guid'" + prosID + "')",
            function (result) {
                qphID = result[0].tss_quotationpartheaderId;
            }, function (error) {
                alert(error.message);
            }, function onComplete() {
                //alert("DONE.");
            }, false
        );
        if (qphID != null && qphID != 'undefined') Xrm.Utility.openEntityForm("tss_quotationpartheader", qphID, null, windowOptions);
    } catch (e) {
        alert('Failed try to open Quotation Part Header because ' + e.message);
    }
}

function createQuotationPartHeader() {
    try {
        var prosIDs = Xrm.Page.data.entity.getId();
        var custID = Xrm.Page.data.entity.attributes.get("tss_customer").getValue()[0];
        var estclosedate = Xrm.Page.data.entity.attributes.get("tss_estimationclosedate").getValue();
        var date = new Date(estclosedate);
        var contactID = Xrm.Page.data.entity.attributes.get("tss_contact").getValue()[0];
        var currency = Xrm.Page.data.entity.attributes.get("tss_currency").getValue()[0];
        var totamount = Xrm.Page.data.entity.attributes.get("tss_totalamount").getValue().toString();
        var branch = Xrm.Page.data.entity.attributes.get("tss_branch").getValue()[0];
        var pss = Xrm.Page.data.entity.attributes.get("tss_pss").getValue()[0];
        var sourcetype = Xrm.Page.data.entity.attributes.get("tss_sourcetype").getValue();
        var entity = {};
        var fl;
        var userID;

        entity.tss_customer = { Id: custID.id, LogicalName: "account" };
        entity.tss_ProspectLink = { Id: prosIDs, LogicalName: "tss_prospectpartheader" };
        entity.tss_branch = { Id: branch.id, LogicalName: "businessunit" };
        entity.tss_PSS = { Id: pss.id, LogicalName: "systemuser" };
        entity.tss_estimationclosedate = estclosedate;
        entity.tss_contact = { Id: contactID.id, LogicalName: "contact" };
        entity.tss_currency = { Id: currency.id, LogicalName: "transactioncurrency" };
        entity.tss_totalprice = { Value: totamount };
        entity.tss_sourcetype = { Value: sourcetype };

        XrmServiceToolkit.Rest.Create(entity, "tss_quotationpartheaderSet",
            function (result) {
                var newEntityId = result.tss_quotationpartheaderId;
                var prospect = {};
                prospect.tss_Pipelinephase = { Value: 865920001 };

                XrmServiceToolkit.Rest.Update(
                    prosIDs,
                    prospect,
                    "tss_prospectpartheaderSet",
                    function () {
                        alert("Successfully Updated Prospect Pipeline to Quotation");
                    },
                    function (error) {
                        alert("Failed to Update Prospect Pipeline: " + error.message);
                    },
                    false
                );
                OpenNewQuotationPartHeader(newEntityId);
            }, function (error) {
                alert(error.message);
            }, true
        );
    } catch (e) {
        alert('createQuotationPartHeader: ' + e.message);
    }
}

function setOnLoad() {
    var formType = Xrm.Page.ui.getFormType();

    if (formType < 2) {
        //Set Hidden Customer Group --> Handle by Business Unit
        //Xrm.Page.getAttribute('tss_customergroup').controls.get(0).setVisible(false);

        //Set Source Type to Direct Sales
        var topic = Xrm.Page.getAttribute("tss_topic").getValue();
        if (topic == null) {
            var optionText = 865920000;
            var options = Xrm.Page.getAttribute("tss_sourcetype").getOptions();
            for (i = 0; i < options.length; i++) {
                if (options[i].value == optionText)
                    Xrm.Page.getAttribute("tss_sourcetype").setValue(options[i].value);
                Xrm.Page.getAttribute("tss_sourcetype").setSubmitMode("always");
            }
        }

        //Set Currency to IDR
        var currency = Xrm.Page.getAttribute("tss_currency").getValue();
        if (currency == null) {
            var idr = new Array();
            idr[0] = new Object();
            idr[0].id = "7459618D-4EE9-E111-9AA2-544249894792";
            idr[0].name = "IDR";
            idr[0].entityType = "transactioncurrency";
            Xrm.Page.getAttribute("tss_currency").setValue(idr);
            Xrm.Page.getAttribute("tss_currency").setSubmitMode("always");
        }

        //Set User & Branch
        var user = Xrm.Page.getAttribute("tss_pss").getValue();
        var branch = Xrm.Page.getAttribute("tss_branch").getValue();
        if (user == null && branch == null || user == 'undefined' && branch == 'undefined') {
            var setUservalue = new Array();
            setUservalue[0] = new Object();
            setUservalue[0].id = Xrm.Page.context.getUserId();
            setUservalue[0].entityType = 'systemuser';
            setUservalue[0].name = Xrm.Page.context.getUserName();
            Xrm.Page.getAttribute("tss_pss").setValue(setUservalue);
            Xrm.Page.getAttribute("tss_pss").setSubmitMode("always");
            var userID = Xrm.Page.context.getUserId();
            XrmServiceToolkit.Rest.Retrieve(
                userID,
                "SystemUserSet",
                null, null,
                function (result) {
                    //entity.tss_Branch = { Id: result.BusinessUnitId.Id, LogicalName: "businessunit" };
                    var setBranch = new Array();
                    setBranch[0] = new Object();
                    setBranch[0].id = result.BusinessUnitId.Id;
                    setBranch[0].entityType = 'businessunit';
                    setBranch[0].name = result.BusinessUnitId.Name;
                    Xrm.Page.getAttribute("tss_branch").setValue(setBranch);
                    Xrm.Page.getAttribute("tss_branch").setSubmitMode("always");
                },
                function (error) {
                    alert("failed");
                },
                false
            );
        }

        //Set Default Value for Two Option Field
        SetDefaultValue_TwoOptionField("tss_havecontract");
        SetDefaultValue_TwoOptionField("tss_underminimumprice");
        SetDefaultValue_TwoOptionField("tss_reviseprospect");
        Xrm.Page.getAttribute("tss_totalamount").setValue(0);
        Xrm.Page.getAttribute("tss_totalamount").setSubmitMode("always");
    }
}

function GetListOfDirtyFields() {
    var attributes = Xrm.Page.data.entity.attributes.get();

    for (var i in attributes) {
        var attribute = attributes[i];
        if (attribute.getIsDirty())
            alert("attribute dirty: " + attribute.getName());
    }
}

function Form_OnSave() {
    forceSubmitDirtyFields();
}

function Ribbon_EnableRules_CancelProspect() {
    var returnResult = false;

    if (Xrm.Page.context.getUserId().replace(/[{}]/g, "").toLowerCase() == Xrm.Page.getAttribute("ownerid").getValue()[0].id.replace(/[{}]/g, "").toLowerCase()
        && Xrm.Page.getAttribute("tss_statusreason").getValue() == 865920000
        && Xrm.Page.ui.getFormType() > 1) {
        returnResult = true;
    }
    return returnResult;
}