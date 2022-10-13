function AlertText() { 
    var primaryContactName = Xrm.Page.data.entity.attributes.get("new_unitproduct").getValue()[0].name; 
var Name = Xrm.Page.data.entity.attributes.get("new_name"); 
Name.setValue(primaryContactName); 
}