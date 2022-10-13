function prospect_rating(arrItem)
{
	var prospectbtn = "opportunity|NoRelationship|HomePageGrid|Traknus.HomepageGrid.opportunity.MainTab.ExportData.ProspectRating-Large";
	var btn = window.top.document.getElementById(prospectbtn);
	if(btn)
	{
	   btn.style.display = 'none';
	}

	try
	{
		var grid = document.getElementById("gridBodyTable").lastChild;
		var gridTH = document.getElementById("gridBodyTable");
		var headers = gridTH.getElementsByTagName("TH");

		for (var n = 0; n < headers.length; n++)
		{
			var header = headers[n];     

			if (header.innerText == "Est. Close Date")
			{
				for (var i = 0; i < grid.childNodes.length; i++)
				{
					var close_date = new Date(grid.childNodes[i].childNodes[n].innerText);
					if (close_date != null || close_date.toString() != "")
					{
						var today_date = new Date();
						today_date.setHours(0,0,0,0);
						
						var selisih = (today_date - close_date) / (1000*60*60*24);
						
						if(selisih >= 30)
						{
							//grid.childNodes[i].style.backgroundColor = "FF0000";
							grid.childNodes[i].style.backgroundColor = "DADADA";
						}
						/*else if(selisih > 90 && selisih <= 180)
						{
							grid.childNodes[i].style.backgroundColor = "FFFF00";
						}
						else if(selisih > 180)
						{
							grid.childNodes[i].style.backgroundColor = "00FFFF";
						}*/
					}
				}
			}
		}
    }
    catch(e)
    {
		alert(e.description);
    }
}