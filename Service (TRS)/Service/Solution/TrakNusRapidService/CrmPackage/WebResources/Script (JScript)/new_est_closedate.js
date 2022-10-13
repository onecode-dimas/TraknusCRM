function est_close_date()
{
	var phase = Xrm.Page.getAttribute("new_pipelinephase").getValue();
	
	if (phase != null)
	{
		if (phase == 1)
		{
			Xrm.Page.getAttribute("estimatedclosedate").setValue(null);
			Xrm.Page.getAttribute("estimatedclosedate").setRequiredLevel("none");
			Xrm.Page.getControl("estimatedclosedate").setVisible(false);
		}
		else
		{
			Xrm.Page.getControl("estimatedclosedate").setVisible(true);
			Xrm.Page.getAttribute("estimatedclosedate").setRequiredLevel("required");
		}
	}
}

function auto_save_estCloseDate()
{
	Xrm.Page.getAttribute("pricelevelid").setRequiredLevel("none");
	Xrm.Page.getAttribute("new_unitgroup").setRequiredLevel("none");
	Xrm.Page.getControl("new_indicator").setDisabled(true);

	Xrm.Page.data.entity.save();
}