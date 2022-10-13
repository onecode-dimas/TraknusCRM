function UpdateClaimIncentive()
{
	var claim = Xrm.Page.getAttribute("new_claimincentive").getValue();
	if (claim ==null )
	{
		var cek = confirm('Are you sure?');
		if (cek)
		{
			Xrm.Page.getAttribute("new_claimincentive").setValue(1);
			Xrm.Page.data.entity.save();
		}
	}
	else if (claim !=null )
	{	
		if (claim !=1)
		{
			var cek = confirm('Are you sure?');
			if (cek)
			{
				Xrm.Page.getAttribute("new_claimincentive").setValue(1);
				Xrm.Page.data.entity.save();
			}
		}
	}
}
function UpdatePaidIncentive()
{
	var paid = Xrm.Page.getAttribute("new_paidincentive").getValue();
	if (paid ==null )
	{
		var cek = confirm('Are you sure?');
		if (cek)
		{
			Xrm.Page.getAttribute("new_paidincentive").setValue(1);
			Xrm.Page.data.entity.save();
		}
	}
	else if (paid !=null )
	{	
		if (paid !=1)
		{
			var cek = confirm('Are you sure?');
			if (cek)
			{
				Xrm.Page.getAttribute("new_paidincentive").setValue(1);
				Xrm.Page.data.entity.save();
			}
		}
	}
}

function CekClaimIncentive()
{
	var claim = Xrm.Page.getAttribute("new_claimincentive").getValue();
	var paid = Xrm.Page.getAttribute("new_paidincentive").getValue();
	
	if (paid !=null )
	{
		if (paid == true)
		{
			Xrm.Page.getControl("new_reason").setDisabled(true);
			Xrm.Page.getControl("new_optionattachmentifany").setDisabled(true);
			Xrm.Page.getControl("new_koefisienf6").setDisabled(true);
			Xrm.Page.getControl("new_koefisienf7").setDisabled(true);
			Xrm.Page.getControl("new_ecbastrmusr").setDisabled(true);		
		}
		else
		{
			if (claim !=null )
			{
				if (claim == true)
				{
					Xrm.Page.getControl("new_reason").setDisabled(true);
					Xrm.Page.getControl("new_optionattachmentifany").setDisabled(true);
					Xrm.Page.getControl("new_koefisienf6").setDisabled(true);
					Xrm.Page.getControl("new_koefisienf7").setDisabled(true);
					Xrm.Page.getControl("new_ecbastrmusr").setDisabled(true);		
				}
				else
				{
					Xrm.Page.getControl("new_reason").setDisabled(false);
					Xrm.Page.getControl("new_optionattachmentifany").setDisabled(false);
					Xrm.Page.getControl("new_koefisienf6").setDisabled(false);
					Xrm.Page.getControl("new_koefisienf7").setDisabled(false);
					Xrm.Page.getControl("new_ecbastrmusr").setDisabled(false);
				}
			}
		}
	}
}