function onLoad() {
    checkActivityNo();
    SetPurposeLookup();
    DisableForm();
}

function setDisabledField(field, value) {
    Xrm.Page.getControl(field).setDisabled(value);
}

function DisableForm() {
    if (Xrm.Page.getAttribute("tss_partactivityheader").getValue() != null) {
        var headerid = Xrm.Page.getAttribute("tss_partactivityheader").getValue()[0].id;

        XrmServiceToolkit.Rest.Retrieve(headerid, "tss_partactivityheaderSet", "tss_Status", null, function (result) {
            var tss_Status = result.tss_Status;
            if (tss_Status != null) {
                if (tss_Status.Value == 865920001) {
                    Xrm.Page.ui.controls.forEach(function (control, index) {
                        control.setDisabled(true);
                    });
                }
            }
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, false);
    }
}

function SetPurposeLookup() {
    var type = Xrm.Page.getAttribute("tss_activitiestype").getValue();

    if (type != null) {
        addPurposeFilter(type);
    }
}

function addPurposeFilter(type) {
    var viewId = "{6fd72744-3676-41d4-8003-ae4cde9ac282}";
    var entityName = "tss_purposeactivity";
    var viewDisplayName = "Purpose Activity";

    var fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>" +
                        "<entity name='tss_purposeactivity'>" +
                            "<attribute name='tss_purpose' />" +
                            "<attribute name='tss_purposeactivityid' />" +
                            " <order attribute='tss_purpose' descending='false' />" +
                            "<filter type='and'>" +
                                "<condition attribute='statecode' operator='eq' value='0'  />" +
                                "<condition attribute='tss_visitactivitytype' operator='eq' value='" + type + "'  />" +
                            "</filter>" +
                          "</entity>" +
                    "</fetch>";

    var layoutXml = "<grid name='resultset' " +
                    "object='1' " +
                    "jump='tss_purposeactivityid' " +
                    "select='1' " +
                    "icon='1' " +
                    "preview='1'>" +
                    "<row name='result' " +
                    "id='tss_purposeactivityid'>" +
                    "<cell name='tss_purpose' " +
                    "width='200' />" +
                    "</row>" +
                    "</grid>";

    Xrm.Page.getControl("tss_purpose").addCustomView(viewId, entityName, viewDisplayName, fetchXml, layoutXml, true);
}

function checkActivityNo() {
    if (Xrm.Page.getAttribute("tss_activities").getValue() != null) {

        //Xrm.Page.getControl("tss_presalesactivity").setVisible(false);
        Xrm.Page.getControl("tss_population").setVisible(false);
        Xrm.Page.getControl("tss_salesorder").setVisible(false);
        Xrm.Page.getControl("tss_quotation").setVisible(false);
        Xrm.Page.getControl("tss_voiceofcustomer").setVisible(false);
        Xrm.Page.getControl("tss_workorder").setVisible(false);
        Xrm.Page.getControl("tss_potentialprospectpart").setVisible(false);

        var activityNo = Xrm.Page.getAttribute("tss_activities").getValue();

        XrmServiceToolkit.Rest.RetrieveMultiple("tss_sopartheaderSet", "?$select=tss_sopartheaderId&$filter=tss_sonumber eq '" + activityNo + "'", function (results) {
            if (results.length > 0) {
                Xrm.Page.getControl("tss_salesorder").setVisible(true);
                Xrm.Page.getAttribute("tss_salesorder").setValue([{ id: results[0].tss_sopartheaderId, name: activityNo, entityType: "tss_sopartheader" }]);
                Xrm.Page.getAttribute("tss_salesorder").setSubmitMode("always");
            }
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, function () {
        }, false);

        var soLines;
        XrmServiceToolkit.Rest.RetrieveMultiple("tss_salesorderpartsublinesSet", "?$select=tss_SalesOrderPartLines&$filter=tss_InvoiceNo eq '" + activityNo + "' or tss_DeliveryNo eq '" + activityNo + "'", function (results) {
            if (results.length > 0) {
                soLines = results[0].tss_SalesOrderPartLines.Id;
            }
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, function () {
        }, false);

        if (soLines != null) {
            XrmServiceToolkit.Rest.RetrieveMultiple("tss_sopartlinesSet", "?$select=tss_SOPartHeaderId&$filter=tss_sopartlinesId eq (guid'" + soLines + "')", function (results) {
                if (results.length > 0) {
                    Xrm.Page.getControl("tss_salesorder").setVisible(true);
                    Xrm.Page.getAttribute("tss_salesorder").setValue([{ id: results[0].tss_SOPartHeaderId.Id, name: results[0].tss_SOPartHeaderId.Name, entityType: "tss_sopartheader" }]);
                    Xrm.Page.getAttribute("tss_salesorder").setSubmitMode("always");
                }
            }, function (error) {
                Xrm.Utility.alertDialog(error.message);
            }, function () {
            }, false);
        }

        XrmServiceToolkit.Rest.RetrieveMultiple("tss_quotationpartheaderSet", "?$select=tss_quotationpartheaderId&$filter=tss_quotationnumber eq '" + activityNo + "' and tss_statusreason/Value ne 865920007 and tss_statusreason/Value ne 865920008", function (results) {
            if (results.length > 0) {
                Xrm.Page.getControl("tss_quotation").setVisible(true);
                Xrm.Page.getAttribute("tss_quotation").setValue([{ id: results[0].tss_quotationpartheaderId, name: activityNo, entityType: "tss_quotationpartheader" }]);
                Xrm.Page.getAttribute("tss_quotation").setSubmitMode("always");
            }
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, function () {
        }, false);

        XrmServiceToolkit.Rest.RetrieveMultiple("ServiceAppointmentSet", "?$select=ActivityId&$filter=trs_crmwonumber eq '" + activityNo + "'", function (results) {
            if (results.length > 0) {
                Xrm.Page.getControl("tss_workorder").setVisible(true);
                Xrm.Page.getAttribute("tss_workorder").setValue([{ id: results[0].ActivityId, name: activityNo, entityType: "serviceappointment" }]);
                Xrm.Page.getAttribute("tss_workorder").setSubmitMode("always");
            }
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, function () {
        }, false);

        XrmServiceToolkit.Rest.RetrieveMultiple("new_populationSet", "?$select=new_populationId&$filter=new_SerialNumber eq '" + activityNo + "'", function (results) {
            if (results.length > 0) {
                Xrm.Page.getControl("tss_population").setVisible(true);
                Xrm.Page.getAttribute("tss_population").setValue([{ id: results[0].new_populationId, name: activityNo, entityType: "new_population" }]);
                Xrm.Page.getAttribute("tss_population").setSubmitMode("always");
            }
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, function () {
        }, false);

        XrmServiceToolkit.Rest.RetrieveMultiple("trs_vocSet", "?$select=trs_vocId&$filter=new_CaseNumber eq '" + activityNo + "'", function (results) {
            if (results.length > 0) {
                Xrm.Page.getControl("tss_voiceofcustomer").setVisible(true);
                Xrm.Page.getAttribute("tss_voiceofcustomer").setValue([{ id: results[0].trs_vocId, name: activityNo, entityType: "trs_voc" }]);
                Xrm.Page.getAttribute("tss_voiceofcustomer").setSubmitMode("always");
            }
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, function () {
        }, false);

        XrmServiceToolkit.Rest.RetrieveMultiple("tss_PotentialProspectPartSet", "?$select=tss_PotentialProspectPartId&$filter=tss_PotentialProspectId eq '" + activityNo + "'", function (results) {
            if (results.length > 0) {
                Xrm.Page.getControl("tss_potentialprospectpart").setVisible(true);
                Xrm.Page.getAttribute("tss_potentialprospectpart").setValue([{ id: results[0].tss_PotentialProspectPartId, name: activityNo, entityType: "tss_potentialprospectpart" }]);
                Xrm.Page.getAttribute("tss_potentialprospectpart").setSubmitMode("always");
            }
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, function () {
        }, false);

        //XrmServiceToolkit.Rest.RetrieveMultiple("tss_presalesactivitySet", "?$select=tss_presalesactivityId&$filter=tss_activity eq '" + activityNo + "'", function (results) {
        //    if (results.length > 0) {
        //        Xrm.Page.getControl("tss_presalesactivity").setVisible(true);
        //        Xrm.Page.getAttribute("tss_presalesactivity").setValue([{ id: results[0].tss_presalesactivityId, name: activityNo, entityType: "tss_presalesactivity" }]);
        //        Xrm.Page.getAttribute("tss_presalesactivity").setSubmitMode("always");
        //    }
        //}, function (error) {
        //    Xrm.Utility.alertDialog(error.message);
        //}, function () {
        //}, false);
    }
}