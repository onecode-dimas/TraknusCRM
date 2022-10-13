function ContractDate() {
var today =new Date();

	Xrm.Page.getAttribute("new_activeon").setValue(today);
}

function AutoPabean()
{
	var pabean = Xrm.Page.getAttribute("new_pabeanlk").getValue();
	if (pabean == null)
	{
		setPabean();
	}
}

function setPabean()
{
	var entityName = 'new_pabean';
	var outputColumns = [new CRMField('new_pabeanid'), new CRMField('new_name')];
	var filters = [new FilterBy('new_name', LogicalOperator.Equal, "Dalam Pabean")];
		
	var items = RetrieveRecords(entityName, outputColumns, filters);
	if(items != null)
	{
		var id = items.Rows[0].Columns[1].Value;
		var nama = items.Rows[0].Columns[2].Value;
		Xrm.Page.getAttribute("new_pabeanlk").setValue([{id: id.toString(), name: nama.toString(), entityType: 'new_pabean'}]);
	}
}