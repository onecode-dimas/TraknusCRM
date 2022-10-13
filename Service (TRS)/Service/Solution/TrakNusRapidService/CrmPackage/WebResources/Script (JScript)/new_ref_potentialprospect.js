function ref_potential_prospect()
{
	var cek = Xrm.Page.getAttribute("new_ref").getValue();
	if (cek == null)
	{
		var ref = Xrm.Page.getAttribute("ownerid").getValue();
		if (ref != null)
		{
			var lookupItem = new Array;
			lookupItem = Xrm.Page.getAttribute("ownerid").getValue();
			if (lookupItem[0] != null)
			{
		Xrm.Page.getAttribute("new_ref").setValue([{id: lookupItem[0].id, name: lookupItem[0].name, entityType: lookupItem[0].entityType}]);
			}
		}
	}
}

function cek_prospect_product()
{
	Xrm.Page.getControl("transactioncurrencyid").setDisabled(false);
	Xrm.Page.getControl("pricelevelid").setDisabled(false);
	Xrm.Page.getControl("new_unitgroup").setDisabled(false);
	 
	var opportunityid = Xrm.Page.data.entity.getId();
	var entityName = 'opportunityproduct';
	var outputColumns = [new CRMField('opportunityproductid')];
	var filters = [new FilterBy('opportunityid', LogicalOperator.Equal, opportunityid)];
	var items = RetrieveRecords(entityName, outputColumns, filters);

	if(items == null)
	{
		Xrm.Page.getControl("transactioncurrencyid").setDisabled(false);
		Xrm.Page.getControl("pricelevelid").setDisabled(false);
		Xrm.Page.getControl("new_unitgroup").setDisabled(false);
	}
	else if(items != null)
	{
		if(items.Rows.length<=0)
		{
			Xrm.Page.getControl("transactioncurrencyid").setDisabled(false);
			Xrm.Page.getControl("pricelevelid").setDisabled(false);
			Xrm.Page.getControl("new_unitgroup").setDisabled(false);
		}
		else if(items.Rows.length>0)
		{
			Xrm.Page.getControl("transactioncurrencyid").setDisabled(true);
			Xrm.Page.getControl("pricelevelid").setDisabled(true);
			Xrm.Page.getControl("new_unitgroup").setDisabled(true);
		}
	}
}