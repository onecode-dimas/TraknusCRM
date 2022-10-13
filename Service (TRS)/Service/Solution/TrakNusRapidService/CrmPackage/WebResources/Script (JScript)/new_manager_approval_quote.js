function tab_manager()
{
	Xrm.Page.ui.tabs.get("manager_approval").setVisible(false);
	var final_quote = Xrm.Page.getAttribute("new_finalquotation").getValue();
	if (final_quote != null)
	{
		if (final_quote == true)
		{
			disableFields();
			var lock = Xrm.Page.getAttribute("new_activatelock").getValue();
			if (lock == null)
			{
				Xrm.Page.ui.tabs.get("manager_approval").setVisible(false);
				Xrm.Page.getControl("new_managerapproval").setDisabled(true);
			}
			else if (lock == 1)
			{
				Xrm.Page.ui.tabs.get("manager_approval").setVisible(false);
				Xrm.Page.getControl("new_managerapproval").setDisabled(true);
			}
			else if (lock == 2)
			{
				var quoteid = Xrm.Page.data.entity.getId();
				var ownerid = getOwner(quoteid.toString());
				var userid = Xrm.Page.context.getUserId();
				var roleUser = Xrm.Page.context.getUserRoles().toString();
				var userLogHasAdmin = hasRoleAdmin(roleUser);
				var manager = Xrm.Page.getAttribute("new_managerapproval").getValue();
				var isManagerUser = Xrm.Page.getAttribute("new_ismanageruser").getValue();
				var managerownerid = "";
				
				if (isManagerUser == true)
				{
					managerownerid = getManagerOwner(ownerid.toString());
				}
				else
				{
					//var pricelevel = getPriceLevel(quoteid.toString());
					managerownerid = getManagerProduct(quoteid.toString());
				}
				
				if (manager == null)
				{
					//Cek Role User apakah Manager atau Administrator
					if (userLogHasAdmin)
					{
						Xrm.Page.ui.tabs.get("manager_approval").setVisible(true);
						Xrm.Page.getControl("new_managerapproval").setDisabled(false);
					}
					else
					{
						if (userid == managerownerid)
						{
							Xrm.Page.ui.tabs.get("manager_approval").setVisible(true);
							Xrm.Page.getControl("new_managerapproval").setDisabled(false);
						}
						else
						{
							Xrm.Page.ui.tabs.get("manager_approval").setVisible(false);
							Xrm.Page.getControl("new_managerapproval").setDisabled(true);
						}
					}
				}
				else
				{
					Xrm.Page.ui.tabs.get("manager_approval").setVisible(false);
					Xrm.Page.getControl("new_managerapproval").setDisabled(true);
				}
			}
			else if (lock == 3)
			{
				Xrm.Page.ui.tabs.get("manager_approval").setVisible(false);
				Xrm.Page.getControl("new_managerapproval").setDisabled(true);
			}
		}
		else
		{
			Xrm.Page.ui.tabs.get("manager_approval").setVisible(false);
			Xrm.Page.getControl("new_managerapproval").setDisabled(true);
		}
	}
	else
	{
		Xrm.Page.ui.tabs.get("manager_approval").setVisible(false);
		Xrm.Page.getControl("new_managerapproval").setDisabled(true);
	}
}

function checkRoles(roleid)
{
	var entityName = 'role';
	var outputColumns = [new CRMField('name')];
	var filters = [new FilterBy('roleid', LogicalOperator.Equal, roleid)];
		
	var items = RetrieveRecords(entityName, outputColumns, filters);
	if(items != null)
	{
		var nama = items.Rows[0].Columns[1].Value;
		if (nama == 'System Administrator')
		{
			return true;
		}
	}
	return false;
}

function hasRoleAdmin(roleid)
{
	var listRole = roleid.split(",");
							
	for (var i = 0; i < listRole.length; i++)
	{
		var flag_role = checkRoles(listRole[i].toString());
		if (flag_role)
		{
			return true;
		}
	}
	return false;
}

function getOwner(quoteid)
{
	var entityName = 'quote';
	var outputColumns = [new CRMField('ownerid')];
	var filters = [new FilterBy('quoteid', LogicalOperator.Equal, quoteid)];
		
	var items = RetrieveRecords(entityName, outputColumns, filters);
	if(items != null)
	{
		return items.Rows[0].Columns[1].Value;
	}
	return "";
}

function getManagerProduct(quoteid)
{
	var entityName = 'quote';
	var outputColumns = [new CRMField('new_managerdept')];
	var filters = [new FilterBy('quoteid', LogicalOperator.Equal, quoteid)];
		
	var items = RetrieveRecords(entityName, outputColumns, filters);
	if(items != null)
	{
		return items.Rows[0].Columns[1].Value;
	}
	return "";
}

function getManagerOwner(managerowner)
{
	var entityName = 'systemuser';
	var outputColumns = [new CRMField('parentsystemuserid')];
	var filters = [new FilterBy('systemuserid', LogicalOperator.Equal, managerowner)];
		
	var items = RetrieveRecords(entityName, outputColumns, filters);
	if(items != null)
	{
		return items.Rows[0].Columns[1].Value;
	}
	return "";
}

/*
function getProductManager(plId)
{
	var entityName = 'new_mappingproductmanager';
	var outputColumns = [new CRMField('new_manager')];
	var filters = [new FilterBy('new_product', LogicalOperator.Equal, plId)];
				   
	var items = RetrieveRecords(entityName, outputColumns, filters);
	if(items != null)
	{
		return items.Rows[0].Columns[1].Value.toString();
	}
	return "";
}*/