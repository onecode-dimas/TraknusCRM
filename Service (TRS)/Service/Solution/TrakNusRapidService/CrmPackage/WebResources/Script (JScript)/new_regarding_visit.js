function visit_regarding_onload()
{
	var regarding_object = Xrm.Page.getAttribute("regardingobjectid").getValue();
	if (regarding_object != null)
	{
		Xrm.Page.getControl("regardingobjectid").setDisabled(true);
	}
}

function visit_regarding_onsave()
{
	var regarding_object = Xrm.Page.getAttribute("regardingobjectid").getValue();
	if (regarding_object != null)
	{
		Xrm.Page.getControl("regardingobjectid").setDisabled(false);
	}
}