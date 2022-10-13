function auto_unit_cpo_product()
{
	var unit_group = Xrm.Page.getAttribute("new_unitgroup").getValue();
	if (unit_group != null)
	{
		setUnit(unit_group[0].name);
	}
}

function setUnit(nama)
{
	var entityName = 'uom';
	var outputColumns = [new CRMField('uomid'), new CRMField('name')];
	var filters = [new FilterBy('name', LogicalOperator.Equal, nama)];
		
	var items = RetrieveRecords(entityName, outputColumns, filters);
	if(items != null)
	{
		var id = items.Rows[0].Columns[1].Value;
		var nama = items.Rows[0].Columns[2].Value;
		Xrm.Page.getAttribute("uomid").setValue([{id: id.toString(), name: nama.toString(), entityType: 'uom'}]);
	}
}