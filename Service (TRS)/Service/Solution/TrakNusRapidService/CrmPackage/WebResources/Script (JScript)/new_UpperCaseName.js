function convertUpperCaseName()
{
	var firstName = Xrm.Page.getAttribute("firstname").getValue();
	if (firstName != null)
	{
		Xrm.Page.getAttribute("firstname").setValue(firstName.toUpperCase());
	}
	var middleName = Xrm.Page.getAttribute("middlename").getValue();
	if (middleName != null)
	{
		Xrm.Page.getAttribute("middlename").setValue(middleName.toUpperCase());
	}
	var lastName = Xrm.Page.getAttribute("lastname").getValue();
	if (lastName != null)
	{
		Xrm.Page.getAttribute("lastname").setValue(lastName.toUpperCase());
	}
}