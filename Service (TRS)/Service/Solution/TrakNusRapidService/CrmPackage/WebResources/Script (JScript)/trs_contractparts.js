///
/// Price Discount Calculation
///
function CalculateTotal() {

    var priceValue = Xrm.Page.getAttribute("trs_price").getValue();
    var discountValue = Xrm.Page.getAttribute("trs_partdiscount").getValue();
    var discountedPrice = Xrm.Page.getAttribute("trs_discountedprice");
    //trs_discountedprice
    //trs_partdiscount
    //trs_price
    var discountAmount = Xrm.Page.getAttribute("trs_discountamount").getValue();
    var totalPrice = 0;
    totalPrice = price - (price * discountValue / 100);
    discountedPrice.setValue(totalPrice);
    discountedPrice.setSubmitMode("always");
}
