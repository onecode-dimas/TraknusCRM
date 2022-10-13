function rejected(id)
{
	var reject = confirm("Reject this quote?");
	if (reject)
	{
		Xrm.Page.ui.tabs.get("tab_print_quote").setVisible(false);
		Xrm.Page.ui.tabs.get("tab_contract").setVisible(false);
		Xrm.Page.getAttribute("new_approvalstatus").setValue(0);
		Xrm.Page.getAttribute("new_approvalstate").setValue(1);
		Xrm.Page.data.entity.save();
		
		/*var rejectbtn = "quote|NoRelationship|Form|Traknus.Form.quote.MainTab.Actions.RejectQuote-Large"; // id of revise button
		var btn = window.top.document.getElementById(rejectbtn);
		if(btn)
		{
		   btn.disabled = true;
		}
		disableFormFields(true);*/
	}
}