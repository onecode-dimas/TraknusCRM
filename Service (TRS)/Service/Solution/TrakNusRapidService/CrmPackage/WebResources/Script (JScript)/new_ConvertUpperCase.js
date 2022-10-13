function convertUpperCase()
{
	var accName = Xrm.Page.getAttribute("name").getValue();
	if (accName != null)
	{
		Xrm.Page.getAttribute("name").setValue(accName.toUpperCase());
	}
}