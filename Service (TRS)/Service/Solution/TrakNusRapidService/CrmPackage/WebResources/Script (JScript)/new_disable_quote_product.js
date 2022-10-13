function disable_quote_product()
{
	var formType = Xrm.Page.ui.getFormType();
	if (formType == 2)
	{
		var qid = Xrm.Page.data.entity.getId();
		var parentid = getParentQuote(qid.toString());
		var final_quote = getFinalQuote(parentid.toString());
		if (final_quote == true)
		{
			disableFormFields(true);
		}
	}
}

function getParentQuote(qpid)
{
	var entityName = 'quotedetail';
	var outputColumns = [new CRMField('quoteid')];
	var filters = [new FilterBy('quotedetailid', LogicalOperator.Equal, qpid)];
		
	var items = RetrieveRecords(entityName, outputColumns, filters);
	if(items != null)
	{
		return items.Rows[0].Columns[1].Value.toString();
	}
	return "";
}

function getFinalQuote(qid)
{
	var entityName = 'quote';
	var outputColumns = [new CRMField('new_finalquotation')];
	var filters = [new FilterBy('quoteid', LogicalOperator.Equal, qid)];
		
	var items = RetrieveRecords(entityName, outputColumns, filters);
	if(items != null)
	{
		return items.Rows[0].Columns[1].Value;
	}
	return false;
}