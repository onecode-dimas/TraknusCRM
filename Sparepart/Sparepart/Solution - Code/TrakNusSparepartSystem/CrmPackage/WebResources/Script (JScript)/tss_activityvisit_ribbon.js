///<reference path="MSXRMTOOLS.Xrm.Page.2016.js"/>

var WebResource = {
    PreSalesActivity: "WebResource_PreSalesActivityGrid",
    QuotationPart: "WebResource_QuotationPart",
    SalesOrder: "WebResource_SalesOrderPart",
    WorkOrder: "WebResource_WorkOrder",
    OutstandingAR: "WebResource_OutstandingAR",
    Delivery: "WebResource_Delivery",
    Invoice: "WebResource_Invoice",
    VoiceOfCustomer: "WebResource_VoiceOfCustomer",
    Population: "WebResource_Population",
    PotentialProspectPart: "WebResource_PotentialProspectPart" //20180907
};

var ActivatedWebResource = [
    WebResource.PreSalesActivity,
    WebResource.QuotationPart,
    WebResource.SalesOrder,
    WebResource.WorkOrder,
    WebResource.OutstandingAR,
    WebResource.Delivery,
    WebResource.Invoice,
    WebResource.VoiceOfCustomer,
    WebResource.Population,
    WebResource.PotentialProspectPart //20180907
];

var ActiveTab = [
    "PreSalesTab",
    "SalesTab",
    "PostSalesTab",
    "PopulationTab"
]

var addNewVisitVisibility = false;
var addNewActivitiesVisibility = true;

function DisableSection() {
    
}

function SetFocus(controlName) {
    Xrm.Page.getControl(controlName).setFocus();
}

function Ribbon_AddNewActivities() {
    SetVisibilityTab(ActiveTab, true);
    SetVisibilityWebResource(ActivatedWebResource, true);
    StartLoading(ActivatedWebResource);
    addNewVisitVisibility = true;
    addNewActivitiesVisibility = false;
    Xrm.Page.ui.refreshRibbon();
    Xrm.Page.ui.setFormNotification("Please scroll down to find the template grid", "INFO", "SCROLLNOTIFICATION");
}

function Ribbon_AddNewVisit() {
    Appointment.CreateVisit();
}

function RibbonVisibility_AddNewActivities() {
    var isNotComplete = Xrm.Page.getAttribute("statecode").getValue() !== null && Xrm.Page.getAttribute("statecode").getValue() !== 1;
    return addNewActivitiesVisibility && isNotComplete;
}

function RibbonVisibility_AddNewVisit() {
    return addNewVisitVisibility;
}

function Ribbon_MarkRecord() {
   debugger;
   alert("marking");
}

function RetrieveWebResourceObject(webResourceName) {
    console.log("Trying to access " + webResourceName);
    return Xrm.Page.getControl(webResourceName).getObject().contentWindow;
}

function SetVisibilityTab(tabName, visibility)
{
    try {
        if (isArray(ActiveTab)) {
            for (var index = 0; index < tabName.length; index++)
            {
                Xrm.Page.ui.tabs.get(tabName[index]).setVisible(visibility);
            }
        }
    } catch (e) {
        console.log(e.message);
    }
}

function SetVisibilityWebResource(webResources, visibility) {
    try {
        var currentControl;
        if (isArray(webResources)) {
            //if array
            for (var index = 0; index < webResources.length; index++) {
                currentControl = Xrm.Page.getControl(webResources[index]);
                if (currentControl !== "undefined") {
                    currentControl.setVisible(visibility);
                }
            }
        } else {
            //treat as string (jz one)
            currentControl = Xrm.Page.getControl(webResources);
            if (currentControl !== "undefined") {
                currentControl.setVisible(visibility);
            }
        }
    } catch (e) {
        console.log(e.message);
    }
} 

function StartLoading(webResources) {
    try {
        if (isArray(webResources)) {
            //if array
            for (var index = 0; index < webResources.length; index++) {
                if (RetrieveWebResourceObject(webResources[index]).Start !== "undefined")
                    RetrieveWebResourceObject(webResources[index]).Start();
                else
                    setTimeout(function() { StartLoading(webResources) },1000); // restart
            }

        } else {
            //treat as string (jz one)
            RetrieveWebResourceObject(webResources).Start();
        }
    } catch (e) {
        console.log(e.message);
    }
}

//Namespace declaration.
var Appointment = {} || Appointment;
Appointment.CreateVisitHeader = function(headerData) {
    var newEntityId = "";
    try {
        XrmServiceToolkit.Rest.Create(headerData, "tss_partactivityheaderSet", function (result) {
            newEntityId = result.tss_partactivityheaderId;
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, false);
        return newEntityId;
    } catch (e) {
        console.log(e);
        return newEntityId;
    }
}

Appointment.CreateVisitLines = function (lines) {
    var newEntityId = "";
    try {
        XrmServiceToolkit.Rest.Create(lines, "tss_partactivitylinesSet", function (result) {
            newEntityId = result.tss_partactivitylinesId;
        }, function (error) {
            Xrm.Utility.alertDialog(error.message);
        }, false);
        return newEntityId;
    } catch (e) {
        console.log(e);
        return newEntityId;
    }
}

Appointment.CreateVisit = function () {
    if (Xrm.Page.getAttribute("tss_customer").getValue() == null) {
        Xrm.Utility.alertDialog("Customer is required before creating visit.");
        return;
    }

    var baseActivityType = 865920000;
    var activityType = {
        MarketSize: baseActivityType + 0,
        Campaign: baseActivityType + 1,
        QuotationPart: baseActivityType + 2,
        PreSalesActivity: baseActivityType + 6,
        SalesOrder: baseActivityType + 4,
        WorkOrder: baseActivityType + 3,
        OutstandingAR: baseActivityType + 7,
        Delivery: baseActivityType + 9,
        Invoice: baseActivityType + 8,
        VoiceOfCustomer: baseActivityType + 10,
        Population: baseActivityType + 5,
        PotentialProspectPart: baseActivityType + 11 //20180907
    }
    var duplicateHeaderResult = [];
    debugger;
    XrmServiceToolkit.Rest.RetrieveMultiple("tss_partactivityheaderSet", "?$select=tss_partactivityheaderId&$filter=tss_Appointment/Id eq (guid'"+Xrm.Page.data.entity.getId()+"')", function (results) {
       for (var i = 0; i < results.length; i++) {
            var tss_partactivityheaderId = results[i].tss_partactivityheaderId;
            duplicateHeaderResult.push(tss_partactivityheaderId);
            
        }
    }, function (error) {
        Xrm.Utility.alertDialog(error.message);
    }, function () {
        //On Complete - Do Something
    }, false);

    if (duplicateHeaderResult.length > 0) {
        var deleteAllRelatedActivityHeader = confirm(duplicateHeaderResult.length +
            " Visit detected that already linked to this Visit Plan.\r\nDo you want to delete all related plan before continuing?");

        if (deleteAllRelatedActivityHeader) {
            for (var idx = 0; idx < duplicateHeaderResult.length; idx++) {
                XrmServiceToolkit.Rest.Delete(duplicateHeaderResult[idx], "tss_partactivityheaderSet", function () {
                    //Success - No Return Data - Do Something
                }, function (error) {
                    Xrm.Utility.alertDialog(error.message);
                }, false);
            }
            
        }
    }
    
    var activitesArray = [
        {
            WebResourceName: WebResource.PreSalesActivity,
            TransformationFunction: function (data) {
                return {
                    tss_ActivitiesType: { Value: activityType.PreSalesActivity },
                    tss_activities: data.tss_activity
                };
            }
        },
        {
            WebResourceName: WebResource.QuotationPart,
            TransformationFunction: function (data) {
                return {
                    tss_ActivitiesType: { Value: activityType.QuotationPart },
                    tss_activities: data.tss_quotationnumber
                };
            }
        },
        {
            WebResourceName: WebResource.WorkOrder,
            TransformationFunction: function (data) {
                return {
                    tss_ActivitiesType: { Value: activityType.WorkOrder },
                    tss_activities: data.trs_crmwonumber
                };
            }
        },
        {
            WebResourceName: WebResource.OutstandingAR,
            TransformationFunction: function (data) {
                return {
                    tss_ActivitiesType: { Value: activityType.OutstandingAR },
                    tss_activities: data.tss_sonumber
                };
            }
        },
        {
            WebResourceName: WebResource.Delivery,
            TransformationFunction: function (data) {
                return {
                    tss_ActivitiesType: { Value: activityType.Delivery },
                    tss_activities: data.tss_DeliveryNo
                };
            }
        },
        {
            WebResourceName: WebResource.Invoice,
            TransformationFunction: function (data) {
                return {
                    tss_ActivitiesType: { Value: activityType.Invoice },
                    tss_activities: data.tss_InvoiceNo
                };
            }
        },
        {
            WebResourceName: WebResource.VoiceOfCustomer,
            TransformationFunction: function (data) {
                return {
                    tss_ActivitiesType: { Value: activityType.VoiceOfCustomer },
                    tss_activities: data.new_CaseNumber
                };
            }
        },
        {
            WebResourceName: WebResource.Population,
            TransformationFunction: function (data) {
                return {
                    tss_ActivitiesType: { Value: activityType.Population },
                    tss_activities: data.new_SerialNumber
                };
            }
        },
        {
            WebResourceName: WebResource.SalesOrder,
            TransformationFunction: function (data) {
                return {
                    tss_ActivitiesType: { Value: activityType.SalesOrder },
                    tss_activities: data.tss_sonumber
                };
            }
        },
        { //20180907
            WebResourceName: WebResource.PotentialProspectPart,
            TransformationFunction: function (data) {
                return {
                    //tss_ActivitiesType: { Value: activityType.PotentialProspectPart },
                    //tss_activities: data.tss_potentialprospectpartid
                    tss_ActivitiesType: { Value: activityType.PotentialProspectPart },
                    tss_activities: data.tss_PotentialProspectId
                };
            }
        }
    ];

    //Check Checked Items
    var itemsString = "Selected Activities:";
    var flagCheckAll = false;

    for (var idx = 0; idx < activitesArray.length; idx++) {
        var webResourceName = activitesArray[idx].WebResourceName;
        var items = RetrieveWebResourceObject(webResourceName).App.checkedRows;
        console.log(items);

        if (items !== "undefined" && items.length > 0) {
            flagCheckAll = true;
            for (var index = 0; index < items.length; index++) {
                var currentItem = items[index];
                var line = activitesArray[idx].TransformationFunction(currentItem);
                itemsString += "\r\n- " + line.tss_activities;
            }
        }
    }

    if (flagCheckAll) {

        var areYouSure = confirm("Are you sure you want to create visit?\r\n" + itemsString);

        if (!areYouSure) return;
        debugger;
        var headerData = {
            tss_plansubject: Xrm.Page.getAttribute("tss_customer").getValue()[0].name + " - " + (new Date()).format("MM/dd/yyyy"),
            tss_Customer: {
                Id: Xrm.Page.getAttribute("tss_customer").getValue()[0].id,
                LogicalName: "account"
            },
            tss_PSS: {
                Id: Xrm.Page.context.getUserId(), //assume that is the current user logged in. Since this is only created from the user perspective.
                LogicalName: "systemuser"
            },
            tss_Appointment: {
                Id: Xrm.Page.data.entity.getId(),
                LogicalName: "appointment"
            },
            tss_Status: {
                Value: 865920000 //Active
            }
        };
        console.log(headerData);
        var headerId = Appointment.CreateVisitHeader(headerData);
        console.log(headerId);
        if (headerId === "") return;

        var createActivity = function (webResourceName, transformationFunction) {
            var selectedActivities = RetrieveWebResourceObject(webResourceName).App.checkedRows;

            if (selectedActivities !== "undefined" && selectedActivities.length > 0) {
                for (var index = 0; index < selectedActivities.length; index++) {
                    var currentItem = selectedActivities[index];
                    var line = transformationFunction(currentItem);
                    line.tss_PartActivityHeader = {
                        Id: headerId,
                        LogicalName: "tss_partactivityheader"
                    };
                    Appointment.CreateVisitLines(line);



                }
            }
        }



        for (var idx = 0; idx < activitesArray.length; idx++) {
            createActivity(activitesArray[idx].WebResourceName, activitesArray[idx].TransformationFunction);
        }

        Xrm.Utility.openEntityForm("tss_partactivityheader", headerId, null,
            {
                openInNewWindow: true
            });
    }
    else {
        alert("Please select activity first!");
    }
}