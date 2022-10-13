function addquote_onlyone()
{
	var prospectid = Xrm.Page.data.entity.getId();
	
	var total = getTotalQuote(prospectid.toString());
	
	if (total == 0)
	{
		return true;
	}
	else if (total > 0)
	{
		return false;
	}
}

function getTotalQuote(pid)
{
	var entityName = 'quote';
	var outputColumns = [new CRMField('statecode')];
	var filters = [new FilterBy('opportunityid', LogicalOperator.Equal, pid.toString())];
	
	var items = RetrieveRecords(entityName, outputColumns, filters);
	var total = 0;
	if(items != null)
	{
		for (var i = 0; i < items.Rows.length; i++)
		{
			if (items.Rows[0].Columns[j].Value == 0 || items.Rows[0].Columns[j].Value == 1)
			{
				total = total + 1;
			}
		}
		return total;
	}
	return total;
}