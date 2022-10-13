function visit_close_date()
{
	var purpose = Xrm.Page.getAttribute("new_purposeofvisit").getValue();
	
	if (purpose != null)
	{
		if (purpose == 5)
		{
			Xrm.Page.getControl("new_estclosedate").setVisible(true);
		}
		else
		{
			Xrm.Page.getControl("new_estclosedate").setVisible(false);
			Xrm.Page.getAttribute("new_estclosedate").setValue(null);
		}
	}
}