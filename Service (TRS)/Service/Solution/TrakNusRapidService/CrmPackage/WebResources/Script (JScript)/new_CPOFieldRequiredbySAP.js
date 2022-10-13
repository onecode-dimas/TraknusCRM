function CPOFieldRequiredbySAP()
{
	if (Xrm.Page.getAttribute("new_ordertype").getValue()==null)
	{
		Xrm.Page.getAttribute("new_ordertype").setValue( [{id: "B28DECCA-F523-E211-92CA-005056924533", name: "CPO  Direct Sales", entityType: "new_ordertype"}]);
	}
	if (Xrm.Page.getAttribute("new_distchannel").getValue()==null)
	{
		Xrm.Page.getAttribute("new_distchannel").setValue( [{id: "870C45DA-F623-E211-92CA-005056924533", name: "Direct", entityType: "new_distchannel"}]);
	}
	if (Xrm.Page.getAttribute("new_pricelistcpo").getValue()==null)
	{
		Xrm.Page.getAttribute("new_pricelistcpo").setValue( [{id: "13D6398E-F823-E211-92CA-005056924533", name: "Price List 1", entityType: "new_pricelistcpo"}]);
	}
	if (Xrm.Page.getAttribute("new_reservation").getValue()==null)
	{
		Xrm.Page.getAttribute("new_reservation").setValue( [{id: "ACE9FF07-F923-E211-92CA-005056924533", name: "Ex Stock", entityType: "new_reservation"}]);
	}
	if (Xrm.Page.getAttribute("new_documentrequired").getValue()==null)
	{
		Xrm.Page.getAttribute("new_documentrequired").setValue( [{id: "D213CE40-0F4F-E211-90D6-005056924533", name: "No", entityType: "new_documentrequired"}]);
	}
	if (Xrm.Page.getAttribute("new_salesorganization").getValue()==null)
	{
		Xrm.Page.getAttribute("new_salesorganization").setValue( [{id: "A6534B68-F623-E211-92CA-005056924533", name: "Traktor Nusantara", entityType: "new_salesorganization"}]);
	}
	if (Xrm.Page.getAttribute("new_pricegroup").getValue()==null)
	{
		Xrm.Page.getAttribute("new_pricegroup").setValue( [{id: "31D38136-F823-E211-92CA-005056924533", name: "Unit LANDED", entityType: "new_pricegroup"}]);
	}
	if (Xrm.Page.getAttribute("new_handlingimport").getValue()==null)
	{
		Xrm.Page.getAttribute("new_handlingimport").setValue( [{id: "464C01EC-0C4F-E211-90D6-005056924533", name: "Not To Be Prepared", entityType: "new_handlingimport"}]);
	}
	if (Xrm.Page.getAttribute("new_salesmanagerremarks").getValue()==null)
	{
		Xrm.Page.getAttribute("new_salesmanagerremarks").setValue("Mohon buat SR & DO unit diatas");
	}
}

function printPreview()
{
	var idsap = Xrm.Page.getAttribute("new_cpoidsap").getValue();
	if (idsap != null)
	{
		Xrm.Page.ui.tabs.get("tab_print_cpo").setVisible(true);
	}
	else
	{
		Xrm.Page.ui.tabs.get("tab_print_cpo").setVisible(false);
	}
}

function po_required()
{
	var po_number = Xrm.Page.getAttribute("new_ponumber").getValue();
	var po_leasing = Xrm.Page.getAttribute("new_poleasingnumber").getValue();
	
	if (po_number != null || po_leasing != null)
	{
		Xrm.Page.getAttribute("new_ponumber").setRequiredLevel("required");
		//Xrm.Page.getAttribute("new_poleasingnumber").setRequiredLevel("required");
	}
	else if (po_number == null && po_leasing == null)
	{
		Xrm.Page.getAttribute("new_ponumber").setRequiredLevel("none");
		Xrm.Page.getAttribute("new_poleasingnumber").setRequiredLevel("none");
	}
}