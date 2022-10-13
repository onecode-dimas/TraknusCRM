function showtab() {
	Xrm.Page.getControl("new_indicator").setDisabled(false);
		Xrm.Page.getAttribute("new_indicatordescription").setRequiredLevel("none");
        	Xrm.Page.ui.tabs.get("Product Line Item Details").sections.get("general_section_4").setVisible(false);

	var option = Xrm.Page.getAttribute("new_indicator").getValue();
	if (option=='1')
	{
		Xrm.Page.getAttribute("new_indicatordescription").setRequiredLevel("required");
        	Xrm.Page.ui.tabs.get("Product Line Item Details").sections.get("general_section_4").setVisible(true);
		Xrm.Page.getControl("new_indicator").setDisabled(true);
    	}
	else if(option=='2')
	{
		Xrm.Page.getAttribute("new_indicatordescription").setRequiredLevel("required");
        	Xrm.Page.ui.tabs.get("Product Line Item Details").sections.get("general_section_4").setVisible(true);
		Xrm.Page.getControl("new_indicator").setDisabled(true);
	}
	else if(option=='3')
	{
		Xrm.Page.getAttribute("new_indicatordescription").setRequiredLevel("none");
        	Xrm.Page.ui.tabs.get("Product Line Item Details").sections.get("general_section_4").setVisible(false);
	}
	else if(option=='0')
	{
		Xrm.Page.getAttribute("new_indicatordescription").setRequiredLevel("none");
        	Xrm.Page.ui.tabs.get("Product Line Item Details").sections.get("general_section_4").setVisible(false);
	}
}

function SaveAndClose() { 
	var pricelist = Xrm.Page.getAttribute("pricelevelid").getValue();
	var pricelistname = '';
	if (pricelist != null)
	{
		var lookupItem = new Array;
		lookupItem = Xrm.Page.getAttribute("pricelevelid").getValue();
		if (lookupItem[0] != null)
		{	
			pricelistname = lookupItem[0].name;
		}
	}
	var entityName = 'uomschedule';
	var outputColumns = [new CRMField('uomscheduleid'), new CRMField('name')];
	var filters = [new FilterBy('name', LogicalOperator.Equal, pricelistname)];
		
	var items = RetrieveRecords(entityName, outputColumns, filters);
	if(items != null)
	{
		var id = items.Rows[0].Columns[1].Value;
		var nama = items.Rows[0].Columns[2].Value;
		Xrm.Page.getAttribute("new_unitgroup").setValue([{id: id.toString(), name: nama.toString(), entityType: 'uomschedule'}]);
	}
/// ABH
		Xrm.Page.getControl("new_indicator").setDisabled(true);
/// ABH

    Xrm.Page.data.entity.save(); 
}