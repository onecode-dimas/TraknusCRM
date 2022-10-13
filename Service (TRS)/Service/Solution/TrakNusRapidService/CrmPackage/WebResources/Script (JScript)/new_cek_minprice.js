function min_price()
{
      var ext_amount = Xrm.Page.getAttribute("extendedamount").getValue() / Xrm.Page.getAttribute("quantity").getValue();
      var min_price = Xrm.Page.getAttribute("new_minimumprice").getValue();
     if (ext_amount < min_price)
     {
            alert('Please Remind that the Price is below the Min Price and will need Approval from the Manager!');
     }
}