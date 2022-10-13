function checkaccno_npwp()
{
	//var reqbtn = "account|NoRelationship|Form|Traknus.Form.account.MainTab.ExportData.RequestAccountNo-Large";
	//var btn = window.top.document.getElementById(reqbtn);
	var acc_no = Xrm.Page.getAttribute("accountnumber").getValue();
	var npwp = Xrm.Page.getAttribute("new_npwp").getValue();
	var cat = Xrm.Page.getAttribute("new_category").getValue();
	
	if (acc_no == null || npwp == null)
	{
		if(cat != null){
			if (cat != 2){
				Xrm.Page.getAttribute("new_category").setValue(3);
			}
		}
		else
		{
			Xrm.Page.getAttribute("new_category").setValue(3);
		}
		/*if(btn)
		{
			btn.disabled = false;
		}*/
	}
	else if (acc_no != null && npwp != null)
	{
		if(cat != null){
			if (cat != 2){
				Xrm.Page.getAttribute("new_category").setValue(1);
			}
		}
		else
		{
			Xrm.Page.getAttribute("new_category").setValue(1);
		}
		/*if(btn)
		{
			btn.disabled = true;
		}*/
	}
}