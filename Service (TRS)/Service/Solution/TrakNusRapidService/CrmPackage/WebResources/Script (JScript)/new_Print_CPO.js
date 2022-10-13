function print_cpo()
{
	var cpo_id_sap = Xrm.Page.getAttribute("new_cpoidsap").getValue();
	if (cpo_id_sap == null)
	{
		Xrm.Page.ui.tabs.get("tab_print_cpo").sections.get("tab_print_cpo_section_cpo").setVisible(false);
	}
	else
	{
		if (cpo_id_sap == "")
		{
			Xrm.Page.ui.tabs.get("tab_print_cpo").sections.get("tab_print_cpo_section_cpo").setVisible(false);
		}
		else
		{
			Xrm.Page.ui.tabs.get("tab_print_cpo").sections.get("tab_print_cpo_section_cpo").setVisible(true);
		}
	}
}