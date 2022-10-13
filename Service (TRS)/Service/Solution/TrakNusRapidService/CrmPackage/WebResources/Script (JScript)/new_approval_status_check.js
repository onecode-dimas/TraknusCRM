function approval_status()
{
	var status = Xrm.Page.getAttribute("new_finalquotation").getValue();
	
	if (status == '1')
	{
		Xrm.Page.ui.tabs.get("tab_print_quote").setVisible(true);
		Xrm.Page.ui.tabs.get("tab_contract").setVisible(true);
		Xrm.Page.getControl("new_paymentterm").setDisabled(true);
	}
	else if (status == '0')
	{
		Xrm.Page.ui.tabs.get("tab_print_quote").setVisible(false);
		Xrm.Page.ui.tabs.get("tab_contract").setVisible(false);
		Xrm.Page.getControl("new_paymentterm").setDisabled(false);
	}
}