function contract_required()
{
	var stat_approval = Xrm.Page.getAttribute("new_finalquotation").getValue();
	Xrm.Page.ui.tabs.get("tab_contract").setVisible(false);
	Xrm.Page.ui.tabs.get("tab_print_quote").sections.get("general_section_3").setVisible(false);

	if (stat_approval != null)
	{
		if (stat_approval == true)
		{
			Xrm.Page.ui.tabs.get("tab_contract").setVisible(true);
			Xrm.Page.getControl("new_finalquotation").setDisabled(true);
			Xrm.Page.getAttribute("new_activeon").setRequiredLevel("required");
			Xrm.Page.getAttribute("new_contactname").setRequiredLevel("required");
			Xrm.Page.getAttribute("new_deliveryterms").setRequiredLevel("required");
			Xrm.Page.ui.tabs.get("tab_print_quote").sections.get("general_section_3").setVisible(true);
		}
		else
		{
			Xrm.Page.ui.tabs.get("tab_contract").setVisible(false);
			Xrm.Page.getAttribute("new_activeon").setRequiredLevel("none");
			Xrm.Page.getAttribute("new_contactname").setRequiredLevel("none");
			Xrm.Page.getAttribute("new_deliveryterms").setRequiredLevel("none");
			Xrm.Page.ui.tabs.get("tab_print_quote").sections.get("general_section_3").setVisible(false);
			//Xrm.Page.getControl("new_finalquotation").setDisabled(false);
		}
	}
}