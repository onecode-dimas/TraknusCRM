function GetProductDescription(ProductGUID)
{
	try {
		if(ProductGUID!= null)
		{
			var entityName = 'product';
			var outputColumns = [new CRMField('description')];
			var filters = [new FilterBy('productid', LogicalOperator.Equal, ProductGUID)];
 
			var items = RetrieveRecords(entityName, outputColumns, filters);
 
			if(items != null)
			{
				var ProductDesc = items.Rows[0].Columns[1].Value;
				
				//Xrm.Page.getAttribute("description").setValue(ProductDesc);
				
				//GetContactName(items.Rows[0].Columns[1].Value);
				return items;
			}
		}
	}
	catch(err) {
	}
 
	return null;
}

function GetProductId()
{
	Xrm.Page.getControl("new_minimumprice").setDisabled(true);
	
	var ceknull = Xrm.Page.getAttribute("productid").getValue();
	var description= Xrm.Page.getAttribute("description").getValue();
	if (ceknull!=null)
	{
		var lookupItem = new Array;
		lookupItem = Xrm.Page.getAttribute("productid").getValue();
		if (lookupItem[0] != null)
		{	if (description==null)
			{
				Xrm.Page.getControl("new_minimumprice").setDisabled(false);
				GetProductDescription(lookupItem[0].id);
				Xrm.Page.getControl("new_minimumprice").setDisabled(true);
			}
			else
			{

			}
		}
	}
}

function GetProductIdOnChange()
{
	var ceknull = Xrm.Page.getAttribute("productid").getValue();
	var description= Xrm.Page.getAttribute("description").getValue();
	if (ceknull!=null)
	{
		var lookupItem = new Array;
		lookupItem = Xrm.Page.getAttribute("productid").getValue();
		if (lookupItem[0] != null)
		{	
			Xrm.Page.getControl("new_minimumprice").setDisabled(false);
			GetProductDescription(lookupItem[0].id);
			Xrm.Page.getControl("new_minimumprice").setDisabled(true);
		}
	}
}

function GetProductIdOnSave()
{
	Xrm.Page.getControl("new_minimumprice").setDisabled(false);
}