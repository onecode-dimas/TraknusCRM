function disable_SN()
{
	var type = Xrm.Page.getAttribute("new_serviceactivitytype").getValue();
	if (type != null)
	{
		if (type == 1)
		{
			Xrm.Page.getControl("new_serialnumber").setDisabled(false);
			Xrm.Page.getControl("new_deliverydate").setDisabled(false);
			Xrm.Page.getControl("new_expireddate").setDisabled(false);
		}
		else
		{
			Xrm.Page.getControl("new_serialnumber").setDisabled(true);
			Xrm.Page.getControl("new_deliverydate").setDisabled(true);
			Xrm.Page.getControl("new_expireddate").setDisabled(true);
		}
	}
}