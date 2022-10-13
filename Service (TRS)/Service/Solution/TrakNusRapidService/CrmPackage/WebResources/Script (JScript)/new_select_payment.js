function selectpayment() {

	Xrm.Page.ui.tabs.get("tab_7").sections.get("section_payment_percent").setVisible(false);
	Xrm.Page.ui.tabs.get("tab_7").sections.get("section_payment_writein").setVisible(false);
	var option = Xrm.Page.getAttribute("new_selectpayment").getValue();
	if (option != null)
	{
		//If Select Payment = Writein
		if (option == 1)
		{
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_payment_percent").setVisible(false);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_payment_writein").setVisible(true);

			//Put Required Field Writein
			Xrm.Page.getAttribute("new_downpaymentwritein").setRequiredLevel("required");
			Xrm.Page.getAttribute("new_paymentiiwritein").setRequiredLevel("required");
			Xrm.Page.getAttribute("new_paymentiiiwritein").setRequiredLevel("required");
			Xrm.Page.getAttribute("new_daytierii").setRequiredLevel("required");
			Xrm.Page.getAttribute("new_daytieriii").setRequiredLevel("required");

			//Remove Required Field Percentage
			Xrm.Page.getAttribute("new_downpayment").setRequiredLevel("none");
			Xrm.Page.getAttribute("new_payment2").setRequiredLevel("none");
			Xrm.Page.getAttribute("new_payment3").setRequiredLevel("none");

			//Remove Value Percentage
			Xrm.Page.getAttribute("new_downpayment").setValue(null);
			Xrm.Page.getAttribute("new_payment2").setValue(null);
			Xrm.Page.getAttribute("new_payment3").setValue(null);
		}
		else if(option == 0)
		{
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_payment_writein").setVisible(false);
			Xrm.Page.ui.tabs.get("tab_7").sections.get("section_payment_percent").setVisible(true);

			//Remove Required Field Writein
			Xrm.Page.getAttribute("new_downpaymentwritein").setRequiredLevel("none");
			Xrm.Page.getAttribute("new_paymentiiwritein").setRequiredLevel("none");
			Xrm.Page.getAttribute("new_paymentiiiwritein").setRequiredLevel("none");

			//Put Required Field Percentage
			Xrm.Page.getAttribute("new_downpayment").setRequiredLevel("required");
			Xrm.Page.getAttribute("new_payment2").setRequiredLevel("required");
			Xrm.Page.getAttribute("new_payment3").setRequiredLevel("required");
			Xrm.Page.getAttribute("new_daytierii").setRequiredLevel("required");
			Xrm.Page.getAttribute("new_daytieriii").setRequiredLevel("required");

			//Remove Value Writein
			Xrm.Page.getAttribute("new_downpaymentwritein").setValue(null);
			Xrm.Page.getAttribute("new_paymentiiwritein").setValue(null);
			Xrm.Page.getAttribute("new_paymentiiiwritein").setValue(null);
		}
	}
	else
	{
		Xrm.Page.ui.tabs.get("tab_7").sections.get("section_payment_percent").setVisible(false);

		//Remove Value Percentage
		Xrm.Page.getAttribute("new_downpayment").setValue(null);
		Xrm.Page.getAttribute("new_payment2").setValue(null);
		Xrm.Page.getAttribute("new_payment3").setValue(null);

		Xrm.Page.ui.tabs.get("tab_7").sections.get("section_payment_writein").setVisible(false);

		//Remove Value Writein
		Xrm.Page.getAttribute("new_downpaymentwritein").setValue(null);
		Xrm.Page.getAttribute("new_paymentiiwritein").setValue(null);
		Xrm.Page.getAttribute("new_paymentiiiwritein").setValue(null);
	}
}