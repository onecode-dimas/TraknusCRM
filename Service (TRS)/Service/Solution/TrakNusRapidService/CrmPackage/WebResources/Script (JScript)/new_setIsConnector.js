function setIsConnector() {
var totAmount = Xrm.Page.getAttribute("new_isconnector").setValue("0");
}

function manualCPOSAP(){
var today =new Date();
var salesoffice = Xrm.Page.getAttribute("new_salesoffice").getValue();
var deliveryplant = Xrm.Page.getAttribute("new_deliveryplant").getValue();
var documentdate = Xrm.Page.getAttribute("new_documentdate").getValue();
	if (salesoffice == null ||deliveryplant == null ||documentdate == null)
	{
		Xrm.Page.getAttribute("new_salesoffice").setValue( [{id: "0B873189-F723-E211-92CA-005056924533", name: "A001", entityType: "new_deliveryplant"}]);
		Xrm.Page.getAttribute("new_deliveryplant").setValue( [{id: "0B873189-F723-E211-92CA-005056924533", name: "A001", entityType: "new_deliveryplant"}]);
	//	Xrm.Page.getAttribute("new_unittobeoperatedin").setValue("Area/Kota/Propinsi");
	//	Xrm.Page.getAttribute("new_adressmap").setValue("Alamat Pengiriman");
	//	Xrm.Page.getAttribute("new_otherremarks").setValue("Customer Group");
	//	Xrm.Page.getAttribute("new_poleasingnumber").setValue("PO Leasing {tanggal} (jika menggunakan leasing)");
	//	Xrm.Page.getAttribute("new_pictitle").setValue("Nama (no hp) Penerima Barang");
		Xrm.Page.getAttribute("new_documentdate").setValue(today);
	//	Xrm.Page.getAttribute("new_deliverytime").setValue(today);
	}
}