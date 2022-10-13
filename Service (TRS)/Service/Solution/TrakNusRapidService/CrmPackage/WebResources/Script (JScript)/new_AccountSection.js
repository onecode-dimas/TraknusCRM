function section_hide() {
    var roleUser = Xrm.Page.context.getUserRoles().toString();
    var listRole = roleUser.split(",");
    //debugger;			
    init(false);
    for (var i = 0; i < listRole.length; i++) {
        var nama = checkRoles(listRole[i].toString());

        //Jika Role = Sales Manager atau Role = System Administrator
        if (nama == 'System Administrator' || nama == '#BC Manager') {
            init(true);
            break;
        }
        else if (nama == '##BC BARU') {
            var userlogid = Xrm.Page.context.getUserId();
            checkUnit(userlogid.toString());
        }
    }
}

function init(flag) {
    Xrm.Page.ui.tabs.get("tab_7").sections.get("general_section_8").setVisible(flag); //MF
    Xrm.Page.ui.tabs.get("tab_7").sections.get("general_section_9").setVisible(flag); //PER
    Xrm.Page.ui.tabs.get("tab_7").sections.get("general_section_10").setVisible(flag); //TYT
    Xrm.Page.ui.tabs.get("tab_7").sections.get("general_section_12").setVisible(flag); //SMV
    Xrm.Page.ui.tabs.get("tab_7").sections.get("general_section_11").setVisible(flag); //SAK
    Xrm.Page.ui.tabs.get("tab_7").sections.get("general_section_13").setVisible(flag); //GDN
    Xrm.Page.ui.tabs.get("tab_7").sections.get("general_section_7").setVisible(flag); //FGW
    Xrm.Page.ui.tabs.get("tab_7").sections.get("general_section_6").setVisible(flag); //BTF
}

function BTF(flag) {
    Xrm.Page.ui.tabs.get("tab_7").sections.get("general_section_6").setVisible(flag); //BTF
}

function FGW(flag) {
    Xrm.Page.ui.tabs.get("tab_7").sections.get("general_section_7").setVisible(flag); //FGW
}

function MF(flag) {
    Xrm.Page.ui.tabs.get("tab_7").sections.get("general_section_8").setVisible(flag); //MF
}

function PER(flag) {
    Xrm.Page.ui.tabs.get("tab_7").sections.get("general_section_9").setVisible(flag); //PER
}

function TYT(flag) {
    Xrm.Page.ui.tabs.get("tab_7").sections.get("general_section_10").setVisible(flag); //TYT
}

function SAK(flag) {
    Xrm.Page.ui.tabs.get("tab_7").sections.get("general_section_11").setVisible(flag); //SAK
}

function STM(flag) {
    Xrm.Page.ui.tabs.get("tab_7").sections.get("general_section_12").setVisible(flag); //SMV
}

function GDN(flag) {
    Xrm.Page.ui.tabs.get("tab_7").sections.get("general_section_13").setVisible(flag); //GDN
}

function checkRoles(roleid) {
    var entityName = 'role';
    var outputColumns = [new CRMField('name')];
    var filters = [new FilterBy('roleid', LogicalOperator.Equal, roleid.toString())];

    var items = RetrieveRecords(entityName, outputColumns, filters);
    if (items != null) {
        return items.Rows[0].Columns[1].Value;
    }
    return "";
}

function checkUnit(uid) {
    var entityName = 'new_systemuser_pricelevel';
    var outputColumns = [new CRMField('pricelevelid')];
    var filters = [new FilterBy('systemuserid', LogicalOperator.Equal, uid.toString())];

    var items = RetrieveRecords(entityName, outputColumns, filters);
    if (items != null) {
        for (var i = 0; i < items.Rows.length; i++) {
            setViewSection(items.Rows[0].Columns[i].Value);
        }
    }
}

function setViewSection(plid) {
    var entityName = 'pricelevel';
    var outputColumns = [new CRMField('name')];
    var filters = [new FilterBy('pricelevelid', LogicalOperator.Equal, plid.toString())];

    var items = RetrieveRecords(entityName, outputColumns, filters);
    if (items != null) {
        for (var i = 0; i < items.Rows.length; i++) {
            if (items.Rows[0].Columns[i].Value == "BTF") {
                BTF(true);
            }
            else if (items.Rows[0].Columns[i].Value == "FGW") {
                FGW(true);
            }
            else if (items.Rows[0].Columns[i].Value == "MF") {
                MF(true);
            }
            else if (items.Rows[0].Columns[i].Value == "PER") {
                PER(true);
            }
            else if (items.Rows[0].Columns[i].Value == "TYT") {
                TYT(true);
            }
            else if (items.Rows[0].Columns[i].Value == "SAK") {
                SAK(true);
            }
            else if (items.Rows[0].Columns[i].Value == "STM") {
                STM(true);
            }
            else if (items.Rows[0].Columns[i].Value == "GDN") {
                GDN(true);
            }
        }
    }
}