function CalculateDiscount()
{
	var typeDiscount = Xrm.Page.getAttribute("new_typediscount").getValue();
	if (typeDiscount == null)
	{
		Xrm.Page.getAttribute("new_typediscount").setValue(false);
		typeDiscount = Xrm.Page.getAttribute("new_typediscount").getValue();
	}
	// percentage 
	if (typeDiscount == false)
	{
		Xrm.Page.getControl("new_percentagediscount").setVisible(true);
		Xrm.Page.getControl("manualdiscountamount").setVisible(false);
	}
	// write - in
	else
	{
		Xrm.Page.getControl("new_percentagediscount").setVisible(false);
		Xrm.Page.getControl("manualdiscountamount").setVisible(true);
	}
}