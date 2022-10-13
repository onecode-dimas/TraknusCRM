function approval_status()
{
	var status = Xrm.Page.getAttribute("new_finalquotation").getValue();

var btnRunWorklfow=top.document.getElementById("quote|NoRelationship|Form|Mscrm.Form.quote.ActivateQuote-Large");


	if (status == '1')
	{
//btnRunWorklfow.style.display='inherit';
		Xrm.Page.ui.tabs.get("tab_print_quote").setVisible(true);
		Xrm.Page.ui.tabs.get("tab_contract").setVisible(true);
		Xrm.Page.getControl("new_paymentterm").setDisabled(true);
	}
	else if (status == '0')
	{
//btnRunWorklfow.style.display='none';
		Xrm.Page.ui.tabs.get("tab_print_quote").setVisible(false);
		Xrm.Page.ui.tabs.get("tab_contract").setVisible(false);
		Xrm.Page.getControl("new_paymentterm").setDisabled(false);
	}
}