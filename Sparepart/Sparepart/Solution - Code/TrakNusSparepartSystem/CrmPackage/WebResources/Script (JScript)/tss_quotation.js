function ribbon_AssignToPdh() {
    Xrm.Page.getAttribute("tss_pdh").setRequiredLevel("required");
    Xrm.Page.ui.controls.get("tss_pdh").setFocus();

    //debugger;
    //CheckQuotationExistInPartHeader();
    //var workflowId = '36082789-FCEA-E711-940C-0050569355F1';
    //var workflowName = 'Assign to PDH';
    //ExecuteWorkflow(workflowId, workflowName, function () {
    //    RefreshForm();
    //});
}

function visibleAssigntoPDH() {
    var check = true;
    if (Xrm.Page.data.entity.attributes.get("trs_quotationnumber") != null) {
        var statuscode = Xrm.Page.data.entity.attributes.get("statuscode").getValue();
        var quotationpartno = Xrm.Page.data.entity.attributes.get("tss_quotationpartno").getValue();
        var pdh = Xrm.Page.getAttribute("tss_pdh").getValue();
        if (statuscode == 167630000 && quotationpartno == null && pdh == null) {
            check = true;
        }
        else {
            check = false;
        }

        /*var QuoId = Xrm.Page.data.entity.getId();
		//var quotationNumber = Xrm.Page.data.entity.attributes.get("trs_quotationnumber").getValue();

        //check quotation number (service) not exist in quotation part
		var statuscode = Xrm.Page.data.entity.attributes.get("statuscode").getValue();
		XrmServiceToolkit.Rest.RetrieveMultiple(
			"tss_quotationpartheaderSet",
			"$select=* &$filter=tss_quotationserviceno/Id eq (guid'" + QuoId + "')",

		function (result)
		{
			if (result.length > 0)
			{
				//alert("data ditemukan result");
				check = false;
                //alert('Assign to PDH sudah pernah dibuat');
			}
			else
			{
				//alert("data tidak ditemukan");
				if (statuscode == 167630000)
				{
					check = true;
				}
				else
				{
					check = false;
				}
			}
		}, function (error)
		{
			alert(error.message);
		}, function onComplete()
		{
			//alert("DONE.");
		}, false);*/
    }
    return check;
}

function On_Load() {
    try {
        //var elem = top.document.getElementById("trs_quotation|NoRelationship|Form|tss.trs_quotation.AssignToPdh.Button");
        //elem.style.display = "none";
        //CheckQuotationExistInPartHeader();
    }
    catch (e) {
        alert('Failed because ' + e.message);
    }
}

function CheckQuotationExistInPartHeader() {
    try {
        var lookupid;
        var lookupObject = Xrm.Page.getAttribute("trs_quotationid");
        if (lookupObject != null) {
            var lookUpObjectValue = lookupObject.getValue();
            if ((lookUpObjectValue != null)) {
                lookupid = lookUpObjectValue[0].id;
            }
        }
        var QuoId = Xrm.Page.data.entity.getId();
        //var QuotationId= Xrm.Page.getAttribute("trs_quotationid").getValue().id;
        var re;
        XrmServiceToolkit.Rest.RetrieveMultiple(
			"tss_quotationpartheaderSet",
			"$select=* &$filter=tss_quotationserviceno/Id eq (guid'" + QuoId + "')",

		function (result) {
		    //re = result[0].tss_quotationserviceno;
		    re = result;
		    //if(result.lengh )
		    if (result.length > 0) {
		        alert("data ditemukan result");
		    }
		    else {
		        alert("data tidak ditemukan");
		    }
		}, function (error) {
		    alert(error.message);
		}, function onComplete() {
		    //alert("DONE.");
		}, false);
        //debugger;
        //		if (re!=null)
        //		{
        //			alert("data ditemukan");
        //		}
        //		else
        //		{
        //			alert("data tidak ditemukan");
        //		}
    }
    catch (e) {
        alert('Failed because ' + e.message);
    }
}

function disableButtonAssignToPdh() {
    var statusQuotation = Xrm.Page.getAttribute("statuscode").getValue();
    if (statusQuotation == 167630000) {
        //Approved status
        //A value of true will result in the Enabling of the button, false will Disable it.
        return true;
    }
    else {
        return false;
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
        req.onreadystatechange = function () {
            AssignResponse(req, workflowName, successCallback, failedCallback);
        };
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

function filterPDH() {
    var pdh = Xrm.Page.getAttribute("tss_pdh").getValue();
    Xrm.Page.getControl("tss_pdh").setDisabled(false);
    Xrm.Page.getControl("tss_pdh").setVisible(false);
    if (pdh != null) {
        Xrm.Page.getControl("tss_pdh").setVisible(true);
        Xrm.Page.getControl("tss_pdh").setDisabled(true);
    }

    var viewId = "{6fd72744-3676-41d4-8003-ae4cde9ac282}";
    var entityName = "systemuser";
    var viewDisplayName = "PDH";

    var fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>" +
                        "<entity name='systemuser'>" +
                            "<attribute name='fullname' />" +
                            "<attribute name='title' />" +
                            "<attribute name='systemuserid' />" +
                            " <order attribute='fullname' descending='false' />" +
                            "<filter type='and'>" +
                                "<condition attribute='isdisabled' operator='eq' value='0'  />" +
                                "<condition attribute='title' operator='eq' value='PDH'  />" +
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

    Xrm.Page.getControl("tss_pdh").addCustomView(viewId, entityName, viewDisplayName, fetchXml, layoutXml, true);
}

function onSave() {
    if (Xrm.Page.getAttribute("tss_pdh").getValue() != null && Xrm.Page.getAttribute("tss_statusassignquo").getValue() == null) {
        //var workflowId = '36082789-FCEA-E711-940C-0050569355F1';
        var workflowId = 'CDC46D0E-E0EB-403D-A71A-237CEE3FECF6';
        var workflowName = 'Assign to PDH';
        ExecuteWorkflow(workflowId, workflowName, function () {
            Xrm.Utility.openEntityForm(Xrm.Page.data.entity.getEntityName(), Xrm.Page.data.entity.getId());
        });
    }
}