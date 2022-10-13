function ribbon_requestaccount()
{
	var acc_no = Xrm.Page.getAttribute("accountnumber").getValue();
	var npwp = Xrm.Page.getAttribute("new_npwp").getValue();
	
	if (acc_no == null || npwp == null)
	{
		return true;
	}
	else if (acc_no != null && npwp != null)
	{
		return false;
	}
}