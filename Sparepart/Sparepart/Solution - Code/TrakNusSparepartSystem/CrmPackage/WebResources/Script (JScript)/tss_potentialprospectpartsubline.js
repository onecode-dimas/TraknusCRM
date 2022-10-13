
function btn_GenerateProspectPart(selectedIds) {
    try {
        var actionName = 'tss_AGITProspectPartActionGenerateProspectPartFromPotentialProspectSubLine';
        ExecuteAction(actionName, selectedIds, function () { OpenNewProspectPart(selectedIds[0].replace('{', '').replace('}', '')); });
    } catch (e) {
        Xrm.Utility.alertDialog('Fail to Generate Prospect Part Header : ' + e.message);
    }
}

function OpenNewProspectPart(selectdId) {
    try {


        var windowOptions = {
            openInNewWindow: true
        };

        var potentialProspectPartSubLinesId = selectdId;
        var partNumberId = null;
        var parttype = null;
        var potentialProspectPartLinesId = null
        var customerId = null;
        var pssId = null;
        var potentialProspectPartId = null;
        var prospectpartheaderId;

        debugger;
        var req = new XMLHttpRequest();
        req.open("GET", Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/tss_PotentialProspectPartSubLinesSet(guid'" + potentialProspectPartSubLinesId + "')?$select=tss_PotentialProspectPartLinesRef,tss_potentialprospectpartref,tss_PartNumber", false);
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                this.onreadystatechange = null;
                if (this.status === 200) {
                    var result = JSON.parse(this.responseText).d;
                    potentialProspectPartLinesId = result.tss_PotentialProspectPartLinesRef.Id;
                    potentialProspectPartId = result.tss_potentialprospectpartref.Id;
                    partNumberId = result.tss_PartNumber.Id;
                } else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send();


        /*
          Cek Part Number
        */

        req = new XMLHttpRequest();
        req.open("GET", Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/trs_masterpartSet(guid'" + partNumberId + "')?$select=tss_parttype", false);
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                this.onreadystatechange = null;
                if (this.status === 200) {
                    var result = JSON.parse(this.responseText).d;
                    parttype = result.tss_parttype;
                } else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send();

        if (parttype == 865920001)/*Commodity*/ {

            req = new XMLHttpRequest();
            req.open("GET", Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/tss_PotentialProspectPartSet(guid'" + potentialProspectPartId + "')?$select=tss_Customer,tss_PSS", false);
            req.setRequestHeader("Accept", "application/json");
            req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            req.onreadystatechange = function () {
                if (this.readyState === 4) {
                    this.onreadystatechange = null;
                    if (this.status === 200) {
                        var result = JSON.parse(this.responseText).d;
                        customerId = result.tss_Customer.Id;
                        pssId = result.tss_PSS.Id;
                    } else {
                        Xrm.Utility.alertDialog(this.statusText);
                    }
                }
            };
            req.send();



        } else {

            req = new XMLHttpRequest();
            req.open("GET", Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/tss_PotentialProspectPartLinesSet(guid'" + potentialProspectPartLinesId + "')?$select=tss_PotentialProspectPart", false);
            req.setRequestHeader("Accept", "application/json");
            req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            req.onreadystatechange = function () {
                if (this.readyState === 4) {
                    this.onreadystatechange = null;
                    if (this.status === 200) {
                        var result = JSON.parse(this.responseText).d;
                        potentialProspectPartId = result.tss_PotentialProspectPart.Id;
                    } else {
                        Xrm.Utility.alertDialog(this.statusText);
                    }
                }
            };
            req.send();

            req = new XMLHttpRequest();
            req.open("GET", Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/tss_PotentialProspectPartSet(guid'" + potentialProspectPartId + "')?$select=tss_Customer,tss_PSS", false);
            req.setRequestHeader("Accept", "application/json");
            req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            req.onreadystatechange = function () {
                if (this.readyState === 4) {
                    this.onreadystatechange = null;
                    if (this.status === 200) {
                        var result = JSON.parse(this.responseText).d;
                        customerId = result.tss_Customer.Id;
                        pssId = result.tss_PSS.Id;
                    } else {
                        Xrm.Utility.alertDialog(this.statusText);
                    }
                }
            };
            req.send();
        }

        req = new XMLHttpRequest();
        req.open("GET", Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/tss_prospectpartheaderSet?$select=tss_prospectpartheaderId&$filter=tss_PSS/Id eq (guid'" + pssId + "') and tss_Customer/Id eq (guid'" + customerId + "')&$top=1&$orderby=CreatedOn desc", false);
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                this.onreadystatechange = null;
                if (this.status === 200) {
                    var returned = JSON.parse(this.responseText).d;
                    var results = returned.results;
                    if (results.length > 0)
                        prospectpartheaderId = results[0].tss_prospectpartheaderId;
                } else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send();

        if (prospectpartheaderId)
            Xrm.Utility.openEntityForm("tss_prospectpartheader", prospectpartheaderId, null, windowOptions);

    } catch (e) {
        alert('Failed try to open Prospect Part Header because ' + e.message);
    }
}

function ExecuteAction(actionName, selectedIds, successCallback, failedCallback) {
    var _return = window.confirm('Do you want to Generate Prospect Part Header?');
    if (_return) {
        var result = null;
        var recordID = "";

        for (var i = 0; i < selectedIds.length; i++) {
            if (recordID == "")
                recordID = selectedIds[i];
            else
                recordID = recordID + "," + selectedIds[i];
        }

        var parameters = {
            "RecordID": recordID
        };

        // Creating the Odata Endpoint
        var oDataPath = Xrm.Page.context.getClientUrl() + "/api/data/v8.0/";
        var req = new XMLHttpRequest();
        req.open("POST", oDataPath + actionName, false);
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState == 4) {
                req.onreadystatechange = null;
                if (this.status == 200 || this.status == 204) {
                    Xrm.Utility.alertDialog('Successfully executed Generate Prospect Part Header.');
                    //location.reload();
                    //AssignResponse(req, 'Successfully executed Generate Prospect Part Header.', successCallback, failedCallback);
                    if (successCallback !== undefined && typeof successCallback === "function") {
                        successCallback();
                    }
                }
                else {
                    var error = "";
                    if (this.response != null) {
                        error = JSON.parse(this.response).error.message;
                    }
                    Xrm.Utility.alertDialog('Fail to Convert to Prospect Part .\r\nResponse Code : (' + req.status + ')' + "\r\nPlease contact your IT support.\r\nTechnical Details : " + error);
                }
            }
        };


        req.send(JSON.stringify(parameters));
    }
}

function AssignResponse(req, workflowName, successCallback, failedCallback) {
    debugger;
    if (req.readyState == 4) {
        if (req.status == 200 || req.status == 204) {
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