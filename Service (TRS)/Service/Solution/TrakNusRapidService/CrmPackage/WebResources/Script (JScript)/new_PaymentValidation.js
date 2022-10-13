function validationPayment()
{
	var option = Xrm.Page.getAttribute("new_selectpayment").getValue();
	if (option != null)
	{
		//If Select Payment = Percentage
		if(option == 0)
		{
			var tot = 0;
			
			var dp = Xrm.Page.getAttribute("new_downpayment").getValue();
			var t2 = Xrm.Page.getAttribute("new_payment2").getValue();
			var t3 = Xrm.Page.getAttribute("new_payment3").getValue();
			//var t4 = Xrm.Page.getAttribute("new_payment4").getValue();
			
			var top = Xrm.Page.getAttribute("new_termofpayment").getValue();
			if (top != null)
			{
				if (top == 1)
				{
					if (dp != null)
					{
						if (dp == 100)
						{
							alert('For CAD, Down Payment can not 100 percent');
							if (t2 != null && t3 != null)
							{
								Xrm.Page.getAttribute("new_downpayment").setValue(100 - (t2 + t3));
							}
							else if ((t2 != null && t3 == null) || (t2 == null && t3 != null))
							{
								Xrm.Page.getAttribute("new_downpayment").setValue(0);
							}
						}
					}
				}
			}
			
			//Automatic Calculation
			if(dp != null && t2 != null)
			{
				var new_value = 100 - (dp + t2);
				if (new_value < 0)
				{
					new_value = 0;
				}
				else if (new_value > 100)
				{
					new_value = 100;
				}
				Xrm.Page.getAttribute("new_payment3").setValue(new_value);
			}
			else if(dp != null && t3 != null)
			{
				var new_value = 100 - (dp + t3);
				if (new_value < 0)
				{
					new_value = 0;
				}
				else if (new_value > 100)
				{
					new_value = 100;
				}
				Xrm.Page.getAttribute("new_payment2").setValue(new_value);
			}
			else if(t2 != null && t3 != null)
			{
				var new_value = 100 - (t2 + t3);
				if (new_value < 0)
				{
					new_value = 0;
				}
				else if (new_value > 100)
				{
					new_value = 99;
				}
				Xrm.Page.getAttribute("new_downpayment").setValue(100 - (t2 + t3));
			}
			
			if (dp != null && t2 != null && t3 != null)
			{
				tot = dp + t2 + t3;
				if (tot != 100)
				{
					if (top == 1 && dp == 100)
					{
					}
					else
					{
						alert("Total must be 100");
					}
				}
			}
		}
		else
		{
			var totAmount = Xrm.Page.getAttribute("totalamount").getValue();
			var tot = 0;
			if (totAmount == null)
			{
				totAmount = 0;
			}
			
			var dp = Xrm.Page.getAttribute("new_downpaymentwritein").getValue();
			var t2 = Xrm.Page.getAttribute("new_paymentiiwritein").getValue();
			var t3 = Xrm.Page.getAttribute("new_paymentiiiwritein").getValue();
			//var t4 = Xrm.Page.getAttribute("new_paymentivwritein").getValue();			

			var top = Xrm.Page.getAttribute("new_termofpayment").getValue();
			if (top != null)
			{
				if (top == 1)
				{
					if (dp != null && totAmount != 0)
					{
						if (dp == totAmount)
						{
							alert('For CAD, Down Payment can not have the same Amount with Total Amount');
							if (t2 != null && t3 != null)
							{
								Xrm.Page.getAttribute("new_downpaymentwritein").setValue(totAmount - (t2 + t3));
							}
							else if ((t2 != null && t3 == null) || (t2 == null && t3 != null))
							{
								Xrm.Page.getAttribute("new_downpaymentwritein").setValue(0);
							}
						}
					}
				}
			}
		
			//Automatic Calculation
			if(dp != null && t2 != null)
			{
				var new_value = totAmount - (dp + t2);
				if (new_value < 0)
				{
					new_value = 0;
				}
				else if (new_value > totAmount)
				{
					new_value = totAmount;
				}
				Xrm.Page.getAttribute("new_paymentiiiwritein").setValue(new_value);
			}
			else if(dp != null && t3 != null)
			{
				var new_value = totAmount - (dp + t3);
				if (new_value < 0)
				{
					new_value = 0;
				}
				else if (new_value > totAmount)
				{
					new_value = totAmount;
				}
				Xrm.Page.getAttribute("new_paymentiiwritein").setValue(new_value);
			}
			else if(t2 != null && t3 != null)
			{
				var new_value = totAmount - (t2 + t3);
				if (new_value < 0)
				{
					new_value = 0;
				}
				else if (new_value > 100)
				{
					new_value = totAmount - 1;
				}
				Xrm.Page.getAttribute("new_downpaymentwritein").setValue(new_value);
			}
					
			if (dp != null && t2 != null && t3 != null)
			{
				tot = dp + t2 + t3;
				if (tot != totAmount)
				{
					alert("Total must be "+ totAmount);
				}
			}
		}
	}
}