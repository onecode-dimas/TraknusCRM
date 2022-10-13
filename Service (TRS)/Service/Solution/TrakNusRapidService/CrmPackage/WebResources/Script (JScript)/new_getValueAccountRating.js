function value_rating()
{
	var cekRating = Xrm.Page.getAttribute("new_rating").getValue();
	if (cekRating != null)
	{
		var rating = Xrm.Page.getAttribute("new_rating").getSelectedOption().text;
		Xrm.Page.getAttribute("new_name").setValue(rating);
	}
}