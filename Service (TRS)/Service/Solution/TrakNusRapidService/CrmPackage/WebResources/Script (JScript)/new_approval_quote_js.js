function approval(id)
{
	var approve = confirm("Approve this quote?");
	if (approve)
	{
		Xrm.Page.ui.tabs.get("tab_print_quote").setVisible(true);
		Xrm.Page.ui.tabs.get("tab_contract").setVisible(true);
		Xrm.Page.getAttribute("new_approvalstatus").setValue(1);
		Xrm.Page.getAttribute("new_approvalstate").setValue(1);
		
		var acc_id = Xrm.Page.getAttribute("customerid").getValue();
		var url = "http://192.168.0.165:83/RequestAccountNumberNPWP.aspx?id="+acc_id[0].id.toString();
		var win = window.open(url, "reqaccnumnpwp", "toolbar=0, menubar=0, status=0, resizeable=0, width=300, height=250");
		
		/*win.close();*/
		Xrm.Page.data.entity.save();
		
		/*var approvebtn = "quote|NoRelationship|Form|Traknus.Form.quote.MainTab.Actions.ApproveQuote-Large"; // id of revise button
		var btn = window.top.document.getElementById(approvebtn);
		if(btn)
		{
		   btn.disabled = true;
		}
		disableFormFields(true);*/
	}
	else
	{
		/*var createorderbtn = "quote|NoRelationship|Form|Mscrm.Form.quote.CreateOrder-Large"; // id of create order button
		var btn = window.top.document.getElementById(createorderbtn);
		if(btn)
		{
		   btn.disabled = true;
		}
		disableFormFields(true);
		Xrm.Page.ui.tabs.get("tab_6").setVisible(false);
		Xrm.Page.getAttribute("new_approvalstatus").setValue(0);
		Xrm.Page.data.entity.save();*/
	}
}