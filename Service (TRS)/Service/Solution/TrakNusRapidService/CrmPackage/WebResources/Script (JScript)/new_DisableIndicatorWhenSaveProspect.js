function disable_indicator()
{
	var formtype = Xrm.Page.ui.getFormType();
	if (formtype != 1)
	{
		var pricelist = Xrm.Page.getAttribute("pricelevelid").getValue();
		if (pricelist != null)
		{
			Xrm.Page.getAttribute("new_indicator").setRequiredLevel("required");
			Xrm.Page.getControl("new_indicator").setDisabled(false);
		}
		else
		{
			Xrm.Page.getAttribute("new_indicator").setRequiredLevel("none");
			Xrm.Page.getControl("new_indicator").setDisabled(true);
			Xrm.Page.getControl("new_indicatordescription").setVisible(false);
		}
	}
	else if (formtype == 1)
	{
		Xrm.Page.getAttribute("new_indicator").setRequiredLevel("none");
		Xrm.Page.getControl("new_indicator").setDisabled(true);
		Xrm.Page.getControl("new_indicatordescription").setVisible(false);
	}
}

function disableNotification()
{
      $(".Notifications").empty();
      $(".Notifications").css("background-color", "transparent");
}