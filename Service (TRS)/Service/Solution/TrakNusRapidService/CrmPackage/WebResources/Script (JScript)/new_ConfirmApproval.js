function confirm_approval()
{
	var approve = Xrm.Page.getAttribute("new_managerapproval").getValue();
	if (approve != null)
	{
		if (approve == 1)
		{
			var cek = confirm('Are you sure?');
			if (!cek)
			{
				Xrm.Page.getAttribute("new_managerapproval").setValue(0);
			}
		}
	}
}