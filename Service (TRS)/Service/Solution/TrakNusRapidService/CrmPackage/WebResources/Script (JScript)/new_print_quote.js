function show_print_priview()
{
	var quoteid = Xrm.Page.data.entity.getId();
	var entityName = 'quote';
	var outputColumns = [new CRMField('statecode'), new CRMField('statuscode')];
	var filters = [new FilterBy('quoteid', LogicalOperator.Equal, quoteid)];
	var state_code;
	var status_code;
			
	var items = RetrieveRecords(entityName, outputColumns, filters);
	if(items != null)
	{
		state_code = items.Rows[0].Columns[1].Value;
		status_code = items.Rows[0].Columns[2].Value;
	}
	
	if (state_code != null)
	{
		if (state_code == "Active")
		{
			Xrm.Page.ui.tabs.get("tab_print_quote").setVisible(true);
		}
		else if (state_code == "Won")
		{
			Xrm.Page.ui.tabs.get("tab_print_quote").setVisible(true);
		}
		else
		{
			Xrm.Page.ui.tabs.get("tab_print_quote").setVisible(false);
		}
	}
	else
	{
		Xrm.Page.ui.tabs.get("tab_print_quote").setVisible(false);
	}
}