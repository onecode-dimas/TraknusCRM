function product_status()
{
	Xrm.Page.getControl("new_indentmonth").setVisible(false);
	var stat = Xrm.Page.getAttribute("new_productstatus").getValue();
	if (stat != null)
	{
		if (stat == 1)
		{
			Xrm.Page.getControl("new_indentmonth").setVisible(true);
		}
		else
		{
			Xrm.Page.getControl("new_indentmonth").setVisible(false);
			Xrm.Page.getAttribute("new_indentmonth").setValue(null);
		}
	}
	else
	{
		Xrm.Page.getAttribute("new_indentmonth").setValue(null);
	}
}
function DefaultIsUserCreated()
{
	Xrm.Page.getAttribute("new_isusercreated").setValue(true);
}