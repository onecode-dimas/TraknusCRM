function setPaymentTerm()
{
	var t = Xrm.Page.getAttribute("new_paymentterm").getValue();
	if (t==null)
	{
//dihilangkan
//		Xrm.Page.getAttribute("new_paymentterm").setValue( [{id: 'B685F912-FA23-E211-92CA-005056924533', name: 'Cash on Delivery', entityType: 'new_paymentterm'}]);
//
	Xrm.Page.getAttribute("new_deliveryterms").setValue("Loco..../Franco Site....");
	}
}