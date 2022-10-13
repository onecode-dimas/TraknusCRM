function cek_min_price_approval()
{
	var formtype = Xrm.Page.ui.getFormType();
	if (formtype != 1)
	{
		var final_quote = Xrm.Page.getAttribute("new_finalquotation").getValue();
		var quoteid = Xrm.Page.data.entity.getId();
		
		if (final_quote == null)
		{
			Xrm.Page.getAttribute("new_activatelock").setValue(1);
			Xrm.Page.getAttribute("new_managerapproval").setValue(null);
		}
		else if (final_quote == false)
		{
			Xrm.Page.getAttribute("new_activatelock").setValue(1);
			Xrm.Page.getAttribute("new_managerapproval").setValue(null);
		}
		else if(final_quote == true)
		{
			Xrm.Page.ui.refreshRibbon();
			
			entityName = 'quotedetail';
			outputColumns = [new CRMField('new_minimumprice'), new CRMField('extendedamount'), new CRMField('quantity')];
			filters = [new FilterBy('quoteid', LogicalOperator.Equal, quoteid)];
			var type;
			
			items = RetrieveRecords(entityName, outputColumns, filters);
			if(items != null)
			{
				for(var j = 0; j < items.Rows.length; j++)
				{
					var min_price = items.Rows[j].Columns[1].Value;
					if (min_price == null)
					{
						min_price = 0;
					}
					var extend_amount = items.Rows[j].Columns[2].Value;
					if (extend_amount == null)
					{
						extend_amount = 0;
					}
					var qty = items.Rows[j].Columns[3].Value;
					if (qty == null)
					{
						qty = 1;
					}
					
					//Jika harga >= min price maka bisa langsung di Activate
					if ((extend_amount / qty) >= min_price)
					{
						//Xrm.Page.getAttribute("new_activatelock").setValue(3);
					}
					else if((extend_amount / qty) < min_price)
					{
						//Xrm.Page.getAttribute("new_activatelock").setValue(2);
						var manager = Xrm.Page.getAttribute("new_managerapproval").getValue();
						if (manager == null)
						{
							alert('To Continue this Quote, it must be Approved by Sales Manager or System Administrator');
						}
						
						break;
					}
				}
			}
		}
	}
}