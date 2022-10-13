function getLocPopulation()
{
	var sn = Xrm.Page.getAttribute("new_serialnumber").getValue();
	var loc_ass = Xrm.Page.getAttribute("location").getValue();
	if (sn != null)
	{
		var entityName = 'new_population';
		var outputColumns = [new CRMField('new_unitlocation')];
		var filters = [new FilterBy('new_serialnumber', LogicalOperator.Equal, sn)];
		
		var items = RetrieveRecords(entityName, outputColumns, filters);
		if(items != null)
		{
			var loc_populasi = items.Rows[0].Columns[1].Value;
			if (loc_ass == null)
			{
				Xrm.Page.getAttribute("location").setValue(loc_populasi.toString());
			}
		}
	}
}