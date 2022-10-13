function sap_technical()
{
	var markcompletebtn = "serviceappointment|NoRelationship|Form|Mscrm.Form.serviceappointment.Close-Large";
	var btn = window.top.document.getElementById(markcompletebtn);
	var sap_tech = Xrm.Page.getAttribute("new_saptechnicalcompleted").getValue();
	if (sap_tech == false || sap_tech == null)
	{
		if(btn)
		{
			btn.disabled = true;
		}
	}
	else
	{
		if(btn)
		{
			btn.disabled = false;
		}
	}	
}
function submitsap()
{
	//var cek = '1';
	//try
	//{
	//	cek = '1';
	//	var submitsapbutton = Xrm.Page.getAttribute("new_submitsap").getValue();
	//	if(submitsapbutton==null || submitsapbutton==0)
	//	{
	//		Xrm.Page.getAttribute("new_submitsap").setValue(1);
	//		//alert('SA send to SAP');
	//		Xrm.Page.data.entity.save();
	//		cek='2';
	//	} 
	//}
	//catch(err)
	//{
	//	cek = '0';
	//	alert(err.message);
	//}
	//var toservicemanager = Xrm.Page.getAttribute("new_toservicemanager").getValue();
	//var subject = Xrm.Page.getAttribute("subject").getValue();
	//var service = Xrm.Page.getAttribute("serviceid").getValue();
	//var serialnumber = Xrm.Page.getAttribute("new_serialnumber").getValue();
	//var scheduledstart = Xrm.Page.getAttribute("scheduledstart").getValue();
	//var scheduledend = Xrm.Page.getAttribute("scheduledend").getValue();
	
	//if (toservicemanager !=null && subject !=null && service !=null && serialnumber !=null && scheduledstart!=null && scheduledend!=null)
	//{
	//	var recordid = Xrm.Page.data.entity.getId();
	//	var url = "http://192.168.0.166:83/WOToSAP.aspx?id="+recordid;
	//	var win = window.open(url, "reqaccnumnpwp", "toolbar=0, menubar=0, status=0, resizeable=0, width=300, height=250");
    //}
    var pmacttype = Xrm.Page.getAttribute("trs_pmacttype").getValue();
    if (pmacttype != null) {
        var pmacttypename = pmacttype[0].name;
        if (pmacttypename == 'INSPECTION' || pmacttypename == 'PPM') {
            var serverUrl = "http://" + window.location.host + "/" + Xrm.Page.context.getOrgUniqueName();
            var entityId = Xrm.Page.data.entity.getId();

            // Creating the Odata Endpoint
            var oDataPath = serverUrl + "/XRMServices/2011/OrganizationData.svc";
            var retrieveReq = new XMLHttpRequest();
            var Odata = oDataPath + "/trs_ppmreportSet?$select=trs_ppmreportId&$filter=trs_WorkOrder/Id eq guid'" + entityId + "'";
            retrieveReq.open("GET", Odata, false);
            retrieveReq.setRequestHeader("Accept", "application/json");
            retrieveReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            //retrieveReq.onreadystatechange = function () { retrieveReqCallBack(this); };
            retrieveReq.send();
            if (retrieveReq.readyState == 4 /* complete */) {
                var retrieved = this.parent.JSON.parse(retrieveReq.responseText).d;
                if (retrieved.results != null && retrieved.results.length > 0) {
                    //alert('found');
                }
                else {
                    alert('PPM Report not found');
                }
            }
        }
    }
}

function hidebutton(){
	//var wo_number = Xrm.Page.getAttribute("new_sapwonumber").getValue();
	
	//if (wo_number == null)
	//{
	//   return true;
	//}
	//else
	//{
	//	return false;
	//}
}