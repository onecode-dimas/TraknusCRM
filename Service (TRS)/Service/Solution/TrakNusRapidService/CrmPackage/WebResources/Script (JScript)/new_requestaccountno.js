function reqAccNo()
{
	var setuju = confirm('Send Request?');
	if (setuju)
	{
		var val = Xrm.Page.getAttribute("new_sendrequestaccount").getValue();
		if ((val == null) || (val == false))
		{
			var cek = '1';
			try
			{
				cek = '1';
				Xrm.Page.getAttribute("new_sendrequestaccount").setValue(true);
				Xrm.Page.data.entity.save();
			}
			catch(err)
			{
				cek = '0';
				alert(err);
			}
			if (cek == '1')
			{
				var name = Xrm.Page.getAttribute("name").getValue();
				var taxStatus = Xrm.Page.getAttribute("new_taxstatus").getValue();
				var businessGroup = Xrm.Page.getAttribute("new_businessgroup").getValue();
				var businessSector = Xrm.Page.getAttribute("new_businesssector").getValue();
				
				//alert('asdadssdas');
				if (name !=null && taxStatus !=null && businessGroup !=null && businessSector !=null)
				{
					alert ('Request has been sent!');
					// window.location.reload(true);
					Xrm.Page.getAttribute("new_sendrequestaccount").setValue(false);
				}			
			}
		}
	}
}