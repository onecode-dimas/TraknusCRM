function validate_npwp()
{
	var format_npwp = /\b[0-9]{2}\.[0-9]{3}\.[0-9]{3}\.[0-9]{1}-[0-9]{3}\.[0-9]{3}\b/;
	var npwp = Xrm.Page.getAttribute("new_npwp").getValue();
	
	if (npwp != null)
	{
		if (npwp.match(format_npwp) == null)
		{
			alert("NPWP Format is Wrong. Correct Format: 00.000.000.0-000.000");
		}
	}
}

function masking_npwp()
{

}