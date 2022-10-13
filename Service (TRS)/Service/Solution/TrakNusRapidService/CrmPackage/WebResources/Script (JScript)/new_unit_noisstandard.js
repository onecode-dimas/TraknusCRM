function no_isstandard()
{
	var pid = Xrm.Page.getAttribute("productid").getValue();
	if (pid != null)
	{
		var flag = IsStandard(pid[0].id.toString());
		if (flag == true)
		{
			alert('You Can Not Add Product Attachment Standard as Unit');
			Xrm.Page.getAttribute("productid").setValue(null);
		}
	}
}

function IsStandard(pid)
{
	try {
		var entityName = 'product';
		var outputColumns = [new CRMField('new_isstandard')];
		var filters = [new FilterBy('productid', LogicalOperator.Equal, pid)];
 
		var items = RetrieveRecords(entityName, outputColumns, filters);
		if(items != null)
		{
			return items.Rows[0].Columns[1].Value;
		}
	}
	catch(err)
	{
		
	}
}