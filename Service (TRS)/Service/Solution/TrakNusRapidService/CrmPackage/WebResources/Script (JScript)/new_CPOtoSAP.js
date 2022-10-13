function CPOtoSAP()
{
	var cpo_id = Xrm.Page.data.entity.getId();
	var url = "http://192.168.0.165:83/CPOToSAP.aspx?id="+cpo_id[0].id.toString();
	var win = window.open(url, "CPOtoSAP", "toolbar=0, menubar=0, status=0, resizeable=0, width=300, height=250");

	Xrm.Page.getAttribute("new_statuscpo").setValue(1);
	Xrm.Page.data.entity.save();
	window.location.reload(true);
}