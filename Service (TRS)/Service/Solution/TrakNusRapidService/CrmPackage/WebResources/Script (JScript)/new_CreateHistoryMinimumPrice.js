function CreateIncident() {
    //get Account GUID and Name
    var AccountGUID = Xrm.Page.data.entity.getId();
    var AccountName = Xrm.Page.data.entity.attributes.get("name").getValue();

    //define default values for new Incident record
    var parameters = {};
    parameters["new_name"] = AccountName;

    //pop incident form with default values
    Xrm.Utility.openEntityForm("new_historyminimumprice", null, parameters);
}