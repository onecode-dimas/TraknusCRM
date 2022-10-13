function activate_lock_stat()
{
	var lock = Xrm.Page.getAttribute("new_finalquotation").getValue();
	if (lock != null)
	{
		Xrm.Page.getControl("new_finalquotation").setDisabled(false);
	}
}