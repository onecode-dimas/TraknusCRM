function showtabpopulationcompetitorunit() {
	var option = Xrm.Page.getAttribute("new_competitorunit").getValue();
    if (option== '0') {
        Xrm.Page.ui.tabs.get("general").sections.get("sectioncompetitorunit").setVisible(false);
		Xrm.Page.getAttribute("new_competitor").setRequiredLevel("none");
    }
	else if(option='1'){
        Xrm.Page.ui.tabs.get("general").sections.get("sectioncompetitorunit").setVisible(true);
		Xrm.Page.getAttribute("new_competitor").setRequiredLevel("required");
	}
}