function GetProductDescriptions(ProductGUID)
{
	try {
		if(ProductGUID!= null)
		{
			var entityName = 'product';
			var outputColumns = [new CRMField('new_minimumprice')];
			var filters = [new FilterBy('productid', LogicalOperator.Equal, ProductGUID)];
 
			var items = RetrieveRecords(entityName, outputColumns, filters);
 
			if(items != null){
																					var ProductDesc = items.Rows[0].Columns[1].Value;
//alert(ProductDesc );
																					Xrm.Page.getAttribute("amount").setValue(parseInt(ProductDesc));
																					return items;
				}
		}
	}
	catch(err) {
	}
 
	return null;
}


function GetProductIds()
{
	var ceknull = Xrm.Page.getAttribute("productid").getValue();
var minprice= Xrm.Page.getAttribute("amount").getValue();
	if (ceknull!=null)
	{
		var lookupItem = new Array;
		lookupItem = Xrm.Page.getAttribute("productid").getValue();
		if (lookupItem[0] != null)
		{	if (minprice==null)
			{
			GetProductDescriptions(lookupItem[0].id);
			}	
			else
			{

			}
		}
	}
}