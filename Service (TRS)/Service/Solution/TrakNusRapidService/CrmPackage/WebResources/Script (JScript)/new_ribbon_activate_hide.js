// function ribbon_activate()
// {
	// var final_quote = Xrm.Page.getAttribute("new_finalquotation").getValue();
	// if (final_quote != null)
	// {
		// if (final_quote == true)
		// {
			// var lock = Xrm.Page.getAttribute("new_activatelock").getValue();
			// if (lock != null)
			// {
				// if (lock == 1)
				// {
					// return false;
				// }
				// else if (lock == 2)
				// {
					// var manager = Xrm.Page.getAttribute("new_managerapproval").getValue();
					// if (manager != null)
					// {
						// return true;
					// }
					// else
					// {
						// return false;
					// }
				// }
				// else if(lock == 3)
				// {
					// return true;
				// }
			// }
		// }
		// else
		// {
			// return true;
		// }
	// }
	// else
	// {
		// return false;
	// }
// }

// function cpo_activate()
// {
	// var final_quote = Xrm.Page.getAttribute("new_finalquotation").getValue();
	// if (final_quote != null)
	// {
		// if (final_quote == true)
		// {
			// var lock = Xrm.Page.getAttribute("new_activatelock").getValue();
			// if (lock != null)
			// {
				// if (lock == 1)
				// {
					// return false;
				// }
				// else if (lock == 2)
				// {					
					// var manager = Xrm.Page.getAttribute("new_managerapproval").getValue();
					// if (manager != null)
					// {
						// if (manager == 1)
						// {
							// return true;
						// }
						// else
						// {
							// return false;
						// }
					// }
					// else
					// {
						// return false;
					// }
				// }
				// else if(lock == 3)
				// {
					// return true;
				// }
			// }
		// }
		// else
		// {
			// return false;
		// }
	// }
	// else
	// {
		// return false;
	// }
// }

function ribbon_activate()
{
	var und_min = Xrm.Page.getAttribute("new_underminprice").getValue();
	if (und_min != null )
	{
		if (und_min == true)
		{
			var manager = Xrm.Page.getAttribute("new_managerapproval").getValue();
			if (manager != null )
			{
				if (manager == true)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}
		else
		{
			return true;
		}
	}	
	/*var lock = Xrm.Page.getAttribute("new_activatelock").getValue();
	if (lock != null)
	{
		if (lock == 4)
		{
			return true;
		}
	}
	var final_quote = Xrm.Page.getAttribute("new_finalquotation").getValue();
	if (final_quote != null)
	{
		if (final_quote == true)
		{
			return true;
		}
		else
		{
			return false;
		}
	}*/	
}

function cpo_activate()
{
		var final_quote = Xrm.Page.getAttribute("new_finalquotation").getValue();
		if (final_quote != null)
		{
			if (final_quote == true)
			{
				var lock = Xrm.Page.getAttribute("new_activatelock").getValue();
				if (lock != null)
				{
					if (lock == 1)
					{
						return false;
					}
					else if (lock == 2)
					{					
						var manager = Xrm.Page.getAttribute("new_managerapproval").getValue();
						if (manager != null)
						{
							if (manager == 1)
							{
								return true;
							}
							else
							{
								return false;
							}
						}
						else
						{
							return false;
						}
					}
					else if(lock == 3)
					{
						return true;
					}
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}
		else
		{
			return false;
		}
}

function revise_button()
{
	var stat = Xrm.Page.getAttribute("statuscode").getValue();
	if (stat > 3)
	{
		return false;
	}
}