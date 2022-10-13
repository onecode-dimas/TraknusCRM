function showtab() {
	var option = Xrm.Page.getAttribute("new_existingcustomer").getValue();
    if (option== '0') {
        Xrm.Page.ui.tabs.get("general").sections.get("Section").setVisible(false);
//		Xrm.Page.ui.tabs.get("general").sections.get("name").setVisible(true);
		Xrm.Page.getAttribute("new_accountname").setRequiredLevel("none");
    }
	else if(option='1'){
		Xrm.Page.ui.tabs.get("general").sections.get("Section").setVisible(true);
//		Xrm.Page.ui.tabs.get("general").sections.get("name").setVisible(false);
		Xrm.Page.getAttribute("new_accountname").setRequiredLevel("required");
	}
}