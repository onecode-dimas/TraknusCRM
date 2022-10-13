function DisableRibon() 
{
	var elem = window.top.document.getElementById("EntityTemplateTab.salesorder.NoRelationship.Form.Mscrm.Form.salesorder.MainTab");
	if (elem!=null)
	{
		elem.style.visibility = 'hidden';
	}
	elem = window.top.document.getElementById("EntityTemplateTab.salesorderdetail.OneToMany.SubGridAssociated.Mscrm.SubGrid.salesorderdetail.MainTab");
	if (elem!=null)
	{
		elem.style.visibility = 'hidden';
	}
	elem = window.top.document.getElementById("EntityTemplateTab.incident.OneToMany.SubGridAssociated.Mscrm.SubGrid.incident.MainTab");
	if (elem!=null)
	{
		elem.style.visibility = 'hidden';
	}
	elem = window.top.document.getElementById("EntityTemplateTab.new_incentive.OneToMany.SubGridAssociated.Mscrm.SubGrid.new_incentive.MainTab");
	if (elem!=null)
	{
		elem.style.visibility = 'hidden';
	}
}

function disabledSAPButton()
{
	var status = Xrm.Page.getAttribute("new_cpoidsap").getValue();
	if (status != null)
	{
		disableFormFields(true);
		return false;
	}
	disableFormFields(false);
	return true;
}
function disabledRetentionButton()
{
	var status = Xrm.Page.getAttribute("new_cpoidsap").getValue();
	if (status != null)
	{
		disableFormFields(true);
		return false;
	}
	var status2 = Xrm.Page.getAttribute("new_statuscpo").getValue();
	if (status2 != null)
	{
		disableFormFields(true);
		return false;
	}
	disableFormFields(false);
	return true;
}
function doesControlHaveAttribute(control) 
{
    var controlType = control.getControlType();
    return controlType != "iframe" && controlType != "webresource" && controlType != "subgrid";
}

function disableFormFields(onOff) 
{
	Xrm.Page.getControl("new_cpoindicator").setDisabled(onOff);
	// Xrm.Page.getControl("new_ponumber").setDisabled(onOff);
	// Xrm.Page.getControl("new_podate").setDisabled(onOff);
	// Xrm.Page.getControl("new_leasing").setDisabled(onOff);
	// Xrm.Page.getControl("new_salesorganization").setDisabled(onOff);
	// Xrm.Page.getControl("new_salesoffice").setDisabled(onOff);
	// Xrm.Page.getControl("new_deliveryplant").setDisabled(onOff);
	// Xrm.Page.getControl("new_pictitle").setDisabled(onOff);
	// Xrm.Page.getControl("new_dpremarks").setDisabled(onOff);
	// Xrm.Page.getControl("new_salesmanagerremarks").setDisabled(onOff);
	// Xrm.Page.getControl("new_deliveryterms").setDisabled(onOff);
	// Xrm.Page.getControl("new_pabeanlk").setDisabled(onOff);
	// Xrm.Page.getControl("new_addressmap").setDisabled(onOff);
	// Xrm.Page.getControl("new_unittobeoperatedin").setDisabled(onOff);
	// Xrm.Page.getControl("new_balanceremarks").setDisabled(onOff);
	// Xrm.Page.getControl("new_otherremarks").setDisabled(onOff);
	
    // Xrm.Page.ui.controls.forEach(function (control, index) 
	// {
		// if (doesControlHaveAttribute(control)) 
		// {
			// ctrl = control;
			// tp = control.getControlType();
			// //id = control.getControlId();
			// control.setDisabled(onOff);
		// }
    // });
}

function disablePreview()
{
	Xrm.Page.ui.tabs.get("tab_print_cpo").setVisible(false);
	var status = Xrm.Page.getAttribute("new_cpoidsap").getValue();
	if (status != null)
	{
		Xrm.Page.ui.tabs.get("tab_print_cpo").setVisible(true);
	}
}