function GetContactName(ContactGUID)
{
	try {
		if(ContactGUID!= null)
		{
			var entityName = 'contact';
			var outputColumns = [new CRMField('firstname'),new CRMField('lastname')];
			var filters = [new FilterBy('contactid', LogicalOperator.Equal, ContactGUID)];
 
			var items = RetrieveRecords(entityName, outputColumns, filters);
 
			if(items != null){
																					var Firstname = items.Rows[0].Columns[1].Value;
																					var Lastname =  items.Rows[0].Columns[2].Value;
																					Xrm.Page.getAttribute("firstname").setValue(Firstname);
																					Xrm.Page.getAttribute("lastname").setValue(Lastname);
																					return items;
				}
		}
	}
	catch(err) {
	}
 
	return null;
} 

function GetAccountName(AccountGUID)
{
	try {
		if(AccountGUID!= null)
		{
			var entityName = 'account';
			var outputColumns = [new CRMField('name'),new CRMField('primarycontactid')];
			var filters = [new FilterBy('accountid', LogicalOperator.Equal, AccountGUID)];
 
			var items = RetrieveRecords(entityName, outputColumns, filters);
 
			if(items != null){
																					var accountname = items.Rows[0].Columns[1].Value;
																					Xrm.Page.getAttribute("companyname").setValue(accountname);
																					GetContactName(items.Rows[0].Columns[2].Value);
																					return items;
				}
		}
	}
	catch(err) {
	}
 
	return null;
}


function GetAccountId()
{
	var ceknull = Xrm.Page.getAttribute("new_accountname").getValue();
	if (ceknull!=null)
	{
		var lookupItem = new Array;
		lookupItem = Xrm.Page.getAttribute("new_accountname").getValue();
		if (lookupItem[0] != null)
		{	
			GetAccountName(lookupItem[0].id);
		}
	}
}