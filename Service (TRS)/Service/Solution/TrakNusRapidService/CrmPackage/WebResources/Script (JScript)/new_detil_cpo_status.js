function detil_status_cpo()
{
	var status_cpo = Xrm.Page.getAttribute("new_statuscpo").getValue();
	
	if (status_cpo >= 3)
	{
		Xrm.Page.ui.tabs.get("tab_7").setVisible(true);
		
		if (status_cpo == 3)
		{
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_unit_allocation").setVisible(true);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_sr").setVisible(false);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_do").setVisible(false);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_bast").setVisible(false);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_invoice").setVisible(false);
		}
		
		if (status_cpo == 4)
		{
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_unit_allocation").setVisible(true);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_do").setVisible(true);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_sr").setVisible(false);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_bast").setVisible(false);		
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_invoice").setVisible(false);
		}
		
		if (status_cpo == 5)
		{
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_unit_allocation").setVisible(true);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_sr").setVisible(true);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_do").setVisible(true);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_bast").setVisible(false);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_invoice").setVisible(false);
		}
		
		if (status_cpo == 6)
		{
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_unit_allocation").setVisible(true);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_sr").setVisible(true);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_do").setVisible(true);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_bast").setVisible(true);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_invoice").setVisible(false);
		}
		
		if (status_cpo >= 7)
		{
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_unit_allocation").setVisible(true);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_sr").setVisible(true);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_do").setVisible(true);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_bast").setVisible(true);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_invoice").setVisible(true);
		}
	}
	else if (status_cpo <= 2)
	{
		Xrm.Page.ui.tabs.get("tab_7").setVisible(false);
	}
}

function detil_status_cpo_products()
{
	var status_cpo = Xrm.Page.getAttribute("new_statuscpo").getValue();
	
	if (status_cpo >= 3)
	{
		Xrm.Page.ui.tabs.get("tab_7").setVisible(true);
		
		if (status_cpo == 3)
		{
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_unit_allocation").setVisible(true);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_sr").setVisible(false);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_do").setVisible(false);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_bast").setVisible(false);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_invoice").setVisible(false);
		}
		
		if (status_cpo == 4)
		{
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_unit_allocation").setVisible(true);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_do").setVisible(true);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_sr").setVisible(false);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_bast").setVisible(false);		
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_invoice").setVisible(false);
		}
		
		if (status_cpo == 5)
		{
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_unit_allocation").setVisible(true);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_sr").setVisible(true);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_do").setVisible(true);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_bast").setVisible(false);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_invoice").setVisible(false);
		}
		
		if (status_cpo == 6)
		{
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_unit_allocation").setVisible(true);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_sr").setVisible(true);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_do").setVisible(true);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_bast").setVisible(true);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_invoice").setVisible(false);
		}
		
		if (status_cpo >= 7)
		{
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_unit_allocation").setVisible(true);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_sr").setVisible(true);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_do").setVisible(true);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_bast").setVisible(true);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_invoice").setVisible(true);
		}
	}
	else
	{
		Xrm.Page.ui.tabs.get("tab_7").sections.get("section_unit_allocation").setVisible(false);
		Xrm.Page.ui.tabs.get("tab_7").sections.get("section_sr").setVisible(false);
		Xrm.Page.ui.tabs.get("tab_7").sections.get("section_do").setVisible(false);
		Xrm.Page.ui.tabs.get("tab_7").sections.get("section_bast").setVisible(false);
		Xrm.Page.ui.tabs.get("tab_7").sections.get("section_invoice").setVisible(false);
	}
}

function DefaultIsUserCreated()
{
	Xrm.Page.getAttribute("new_isusercreated").setValue(true);
	Xrm.Page.getAttribute("new_isplugin").setValue(false);
	Xrm.Page.getAttribute("quantity").setValue(1);
}

function EnabledDiscount()
{
	var effected_retention = Xrm.Page.getAttribute("new_iseffectedretention").getValue();
	if (effected_retention == null)
	{
		Xrm.Page.getAttribute("new_iseffectedretention").setValue(false);
	}
	
	effected_retention = Xrm.Page.getAttribute("new_iseffectedretention").getValue();
	if (effected_retention == true)
	{
		Xrm.Page.getControl("manualdiscountamount").setDisabled(false);
	}
	if (effected_retention == false)
	{
		Xrm.Page.getControl("manualdiscountamount").setDisabled(true);
	}
}

function GetProductType(ProductGUID)
{
	if(ProductGUID!= null)
	{
		var entityName = 'product';
		var outputColumns = [new CRMField('producttypecode')];
		var filters = [new FilterBy('productid', LogicalOperator.Equal, ProductGUID)];

		var items = RetrieveRecords(entityName, outputColumns, filters);
		if(items != null)
		{
			Xrm.Page.getControl("new_iseffectedretention").setDisabled(false);
			var ProductType = items.Rows[0].Columns[1].Value;
			//apabila typenya retention
			//disable is effected retention dan ganti defaultnya menjadi false
			if (ProductType == 3)
			{
				Xrm.Page.getControl("new_retentionamount").setDisabled(false);
				Xrm.Page.getAttribute("new_iseffectedretention").setValue(false);
				Xrm.Page.getControl("new_iseffectedretention").setDisabled(true);
			}
			//apabila typenya retention
			//disable is effected retention dan ganti defaultnya menjadi false
			else
			{
				Xrm.Page.getControl("new_retentionamount").setDisabled(false);
				Xrm.Page.getControl("new_iseffectedretention").setDisabled(false);
			}
			return items;
		}
	}
	return null;
}
function GetProductIdOnChange()
{
	var ceknull = Xrm.Page.getAttribute("productid").getValue();
	if (ceknull!=null)
	{
		var lookupItem = new Array;
		lookupItem = Xrm.Page.getAttribute("productid").getValue();
		if (lookupItem[0] != null)
		{	
			GetProductType(lookupItem[0].id);
		}
		else
		{
			Xrm.Page.getControl("new_iseffectedretention").setDisabled(true);		
			Xrm.Page.getControl("new_retentionamount").setDisabled(true);		
		}
	}
	else
	{
		Xrm.Page.getControl("new_iseffectedretention").setDisabled(true);		
		Xrm.Page.getControl("new_retentionamount").setDisabled(true);		
	}
}
function GetProductOnSave(ProductGUID)
{
	if(ProductGUID!= null)
	{
		var entityName = 'product';
		var outputColumns = [new CRMField('producttypecode')];
		var filters = [new FilterBy('productid', LogicalOperator.Equal, ProductGUID)];

		var items = RetrieveRecords(entityName, outputColumns, filters);
		if(items != null)
		{
			Xrm.Page.getControl("new_iseffectedretention").setDisabled(false);
			Xrm.Page.getControl("new_retentionamount").setDisabled(false);
			
			var ProductType = items.Rows[0].Columns[1].Value;
			//apabila typenya retention
			//disable is effected retention dan ganti defaultnya menjadi false
			if (ProductType == 3)
			{
				Xrm.Page.getAttribute("new_iseffectedretention").setValue(false);
			}
			Xrm.Page.getControl("new_retentionamount").setDisabled(false);
			Xrm.Page.getControl("new_iseffectedretention").setDisabled(false);
			
			return items;
		}
	}
	return null;
}
function GetProductIdOnSave()
{
	Xrm.Page.getControl("new_iseffectedretention").setDisabled(false);		
	Xrm.Page.getControl("new_retentionamount").setDisabled(false);		
	
	var ceknull = Xrm.Page.getAttribute("productid").getValue();
	if (ceknull!=null)
	{
		var lookupItem = new Array;
		lookupItem = Xrm.Page.getAttribute("productid").getValue();
		if (lookupItem[0] != null)
		{	
			GetProductOnSave(lookupItem[0].id);
		}
	}
}