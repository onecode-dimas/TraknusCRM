function validationPayment() {
    //debugger;
    var option = Xrm.Page.getAttribute("new_selectpayment").getValue();

    if (option != null) {
        if (option == 0) {
            // OPTION PAYMENT == PERCENTAGE

            var tot = 0;
            var _maxpercentage = 100;
            var att_dp = Xrm.Page.getAttribute("new_downpayment");
            var att_t2 = Xrm.Page.getAttribute("new_payment2");
            var att_t3 = Xrm.Page.getAttribute("new_payment3");
            var att_t4 = Xrm.Page.getAttribute("ittn_payment4");

            var dp = att_dp.getValue();
            var t2 = att_t2.getValue();
            var t3 = att_t3.getValue();
            var t4 = att_t4.getValue();
            var top = Xrm.Page.getAttribute("new_termofpayment").getValue();

            if (dp == null) {
                dp = 0;
                att_dp.setValue(0);
            }

            if (t2 == null) {
                t2 = 0;
                att_t2.setValue(0);
            }

            if (t3 == null) {
                t3 = 0;
                att_t3.setValue(0);
            }

            if (t4 == null) {
                t4 = 0;
                att_t4.setValue(0);
            }

            var att_retention = Xrm.Page.getAttribute("ittn_retention");

            if (att_retention != null) {
                var _retention = att_retention.getValue();

                if (_retention) {
                    var att_retentionpcg = Xrm.Page.getAttribute("ittn_retentionpercentage");

                    if (att_retentionpcg != null) {
                        var _retentionpercentage = att_retentionpcg.getValue();

                        if (_retentionpercentage != null && _retentionpercentage > 0) {
                            _maxpercentage = _maxpercentage - _retentionpercentage;

                            if ((dp + t2 + t3 + t4) > _maxpercentage) {
                                dp = _maxpercentage;
                                t2 = 0;
                                t3 = 0;
                                t4 = 0;

                                att_dp.setValue(dp);
                                att_t2.setValue(t2);
                                att_t3.setValue(t3);
                                att_t4.setValue(t4);
                            }

                        }
                    }

                }
            }

            if (top != null) {
                if (top == 1) {
                    if (dp == _maxpercentage) {
                        alert('For CAD, Down Payment can not ' + _maxpercentage + ' percent !');

                        //if (t2 > 0 && t3 > 0 && t4 > 0) {
                        //    att_dp.setValue(_maxpercentage - (t2 + t3 + t4));
                        //}
                        //else if 
                        //    (
                        //    (t2 == 0 && t3 == 0 && t4 == 0) ||
                        //    (t2 > 0 && t3 == 0 && t4 == 0) ||
                        //    (t2 == 0 && t3 > 0 && t4 == 0) ||
                        //    (t2 == 0 && t3 == 0 && t4 > 0) ||
                        //    (t2 > 0 && t3 > 0 && t4 == 0) ||
                        //    (t2 > 0 && t3 == 0 && t4 > 0) ||
                        //    (t2 == 0 && t3 > 0 && t4 > 0)
                        //) {
                        //    att_dp.setValue(0);
                        //}

                        dp = 0;
                        t2 = 0;
                        t3 = 0;
                        t4 = 0;

                        att_dp.setValue(dp);
                        att_t2.setValue(t2);
                        att_t3.setValue(t3);
                        att_t4.setValue(t4);
                    }
                }
            }

            // AUTOMATIC CALCULATION
            // 1 COMBINATION
            if (dp > 0 && t2 == 0 && t3 == 0 && t4 == 0) {
                if (dp > _maxpercentage) {
                    dp = _maxpercentage;
                }

                var _newvalue = _maxpercentage - dp;

                att_dp.setValue(dp);
                att_t2.setValue(_newvalue);
                att_t3.setValue(0);
                att_t4.setValue(0);
            }
            else if (dp == 0 && t2 > 0 && t3 == 0 && t4 == 0) {
                if (t2 > _maxpercentage) {
                    t2 = _maxpercentage;
                }

                var _newvalue = _maxpercentage - t2;

                att_dp.setValue(_newvalue);
                att_t2.setValue(t2);
                att_t3.setValue(0);
                att_t4.setValue(0);
            }
            else if (dp == 0 && t2 == 0 && t3 > 0 && t4 == 0) {
                if (t3 > _maxpercentage) {
                    t3 = _maxpercentage;
                }

                var _newvalue = _maxpercentage - t3;

                att_dp.setValue(_newvalue);
                att_t2.setValue(0);
                att_t3.setValue(t3);
                att_t4.setValue(0);
            }
            else if (dp == 0 && t2 == 0 && t3 == 0 && t4 > 0) {
                if (t4 > _maxpercentage) {
                    t4 = _maxpercentage;
                }

                var _newvalue = _maxpercentage - t4;

                att_dp.setValue(_newvalue);
                att_t2.setValue(0);
                att_t3.setValue(0);
                att_t4.setValue(t4);
            }

            // 2 COMBINATIONS
            else if (dp > 0 && t2 > 0 && t3 == 0 && t4 == 0) {
                if ((dp + t2) > _maxpercentage) {
                    var _newvalue = _maxpercentage - dp;

                    att_dp.setValue(dp);
                    att_t2.setValue(_newvalue);
                    att_t3.setValue(0);
                    att_t4.setValue(0);
                }
                else {
                    var _newvalue = _maxpercentage - (dp + t2);

                    att_dp.setValue(dp);
                    att_t2.setValue(t2);
                    att_t3.setValue(_newvalue);
                    att_t4.setValue(0);
                }
            }
            else if (dp > 0 && t2 == 0 && t3 > 0 && t4 == 0) {
                if ((dp + t3) > _maxpercentage) {
                    var _newvalue = _maxpercentage - dp;

                    att_dp.setValue(dp);
                    att_t2.setValue(0);
                    att_t3.setValue(_newvalue);
                    att_t4.setValue(0);
                }
                else {
                    var _newvalue = _maxpercentage - (dp + t3);

                    att_dp.setValue(dp);
                    att_t2.setValue(_newvalue);
                    att_t3.setValue(t3);
                    att_t4.setValue(0);
                }
            }
            else if (dp > 0 && t2 == 0 && t3 == 0 && t4 > 0) {
                if ((dp + t4) > _maxpercentage) {
                    var _newvalue = _maxpercentage - dp;

                    att_dp.setValue(dp);
                    att_t2.setValue(0);
                    att_t3.setValue(0);
                    att_t4.setValue(_newvalue);
                }
                else {
                    var _newvalue = _maxpercentage - (dp + t4);

                    att_dp.setValue(dp);
                    att_t2.setValue(_newvalue);
                    att_t3.setValue(0);
                    att_t4.setValue(t4);
                }
            }
            else if (dp == 0 && t2 > 0 && t3 > 0 && t4 == 0) {
                if ((t2 + t3) > _maxpercentage) {
                    var _newvalue = _maxpercentage - t2;

                    att_dp.setValue(0);
                    att_t2.setValue(t2);
                    att_t3.setValue(_newvalue);
                    att_t4.setValue(0);
                }
                else {
                    var _newvalue = _maxpercentage - (t2 + t3);

                    att_dp.setValue(_newvalue);
                    att_t2.setValue(t2);
                    att_t3.setValue(t3);
                    att_t4.setValue(0);
                }
            }
            else if (dp == 0 && t2 > 0 && t3 == 0 && t4 > 0) {
                if ((t2 + t4) > _maxpercentage) {
                    var _newvalue = _maxpercentage - t2;

                    att_dp.setValue(0);
                    att_t2.setValue(t2);
                    att_t3.setValue(0);
                    att_t4.setValue(_newvalue);
                }
                else {
                    var _newvalue = _maxpercentage - (t2 + t4);

                    att_dp.setValue(_newvalue);
                    att_t2.setValue(t2);
                    att_t3.setValue(0);
                    att_t4.setValue(t4);
                }
            }
            else if (dp == 0 && t2 == 0 && t3 > 0 && t4 > 0) {
                if ((t3 + t4) > _maxpercentage) {
                    var _newvalue = _maxpercentage - t3;

                    att_dp.setValue(0);
                    att_t2.setValue(0);
                    att_t3.setValue(t3);
                    att_t4.setValue(_newvalue);
                }
                else {
                    var _newvalue = _maxpercentage - (t3 + t4);

                    att_dp.setValue(_newvalue);
                    att_t2.setValue(0);
                    att_t3.setValue(t3);
                    att_t4.setValue(t4);
                }
            }

            // 3 COMBINATIONS
            else if (dp > 0 && t2 > 0 && t3 > 0 && t4 == 0) {
                if ((dp + t2 + t3) > _maxpercentage) {
                    var _newvalue = _maxpercentage - (dp + t2);

                    att_dp.setValue(dp);
                    att_t2.setValue(t2);
                    att_t3.setValue(_newvalue);
                    att_t4.setValue(0);
                }
                else {
                    var _newvalue = _maxpercentage - (dp + t2 + t3);

                    att_dp.setValue(dp);
                    att_t2.setValue(t2);
                    att_t3.setValue(t3);
                    att_t4.setValue(_newvalue);
                }
            }
            else if (dp > 0 && t2 > 0 && t3 == 0 && t4 > 0) {
                if ((dp + t2 + t4) > _maxpercentage) {
                    var _newvalue = _maxpercentage - (dp + t2);

                    att_dp.setValue(dp);
                    att_t2.setValue(t2);
                    att_t3.setValue(0);
                    att_t4.setValue(_newvalue);
                }
                else {
                    var _newvalue = _maxpercentage - (dp + t2 + t4);

                    att_dp.setValue(dp);
                    att_t2.setValue(t2);
                    att_t3.setValue(_newvalue);
                    att_t4.setValue(t4);
                }
            }
            else if (dp > 0 && t2 == 0 && t3 > 0 && t4 > 0) {
                if ((dp + t3 + t4) > _maxpercentage) {
                    var _newvalue = _maxpercentage - (dp + t3);

                    att_dp.setValue(dp);
                    att_t2.setValue(0);
                    att_t3.setValue(t3);
                    att_t4.setValue(_newvalue);
                }
                else {
                    var _newvalue = _maxpercentage - (dp + t3 + t4);

                    att_dp.setValue(dp);
                    att_t2.setValue(_newvalue);
                    att_t3.setValue(t3);
                    att_t4.setValue(t4);
                }
            }
            else if (dp == 0 && t2 > 0 && t3 > 0 && t4 > 0) {
                if ((t2 + t3 + t4) > _maxpercentage) {
                    var _newvalue = _maxpercentage - (t2 + t3);

                    att_dp.setValue(0);
                    att_t2.setValue(t2);
                    att_t3.setValue(t3);
                    att_t4.setValue(_newvalue);
                }
                else {
                    var _newvalue = _maxpercentage - (t2 + t3 + t4);

                    att_dp.setValue(_newvalue);
                    att_t2.setValue(t2);
                    att_t3.setValue(t3);
                    att_t4.setValue(t4);
                }
            }

            // 4 COMBINATIONS
            else if (dp > 0 && t2 > 0 && t3 > 0 && t4 > 0) {
                if ((dp + t2 + t3 + t4) > _maxpercentage) {
                    att_dp.setValue(_maxpercentage);
                    att_t2.setValue(0);
                    att_t3.setValue(0);
                    att_t4.setValue(0);
                }
                else {
                    var _newvalue = _maxpercentage - (dp + t2 + t3);

                    att_dp.setValue(dp);
                    att_t2.setValue(t2);
                    att_t3.setValue(t3);
                    att_t4.setValue(_newvalue);
                }
            }

            if (dp > 0 && t2 > 0 && t3 > 0 && t4 > 0) {
                tot = dp + t2 + t3 + t4;

                if (tot != _maxpercentage) {
                    if (top == 1 && dp == _maxpercentage) {

                    }
                    else {
                        alert("Total must be " + _maxpercentage + " !");
                    }
                }
            }
        }

        else

        {
            // OPTION PAYMENT == WRITEIN

            var totAmount = Xrm.Page.getAttribute("totalamount").getValue();
            var tot = 0;

            if (totAmount == null) {
                totAmount = 0;
            }

            var att_dp = Xrm.Page.getAttribute("new_downpaymentwritein");
            var att_t2 = Xrm.Page.getAttribute("new_paymentiiwritein");
            var att_t3 = Xrm.Page.getAttribute("new_paymentiiiwritein");
            var att_t4 = Xrm.Page.getAttribute("ittn_paymentivwritein");

            var dp = att_dp.getValue();
            var t2 = att_t2.getValue();
            var t3 = att_t3.getValue();
            var t4 = att_t4.getValue();
            var top = Xrm.Page.getAttribute("new_termofpayment").getValue();

            if (dp == null) {
                dp = 0;
                att_dp.setValue(0);
            }

            if (t2 == null) {
                t2 = 0;
                att_t2.setValue(0);
            }

            if (t3 == null) {
                t3 = 0;
                att_t3.setValue(0);
            }

            if (t4 == null) {
                t4 = 0;
                att_t4.setValue(0);
            }

            var att_retention = Xrm.Page.getAttribute("ittn_retention");

            if (att_retention != null) {
                var _retention = att_retention.getValue();

                if (_retention) {
                    var att_retentionpcg = Xrm.Page.getAttribute("ittn_retentionpercentage");

                    if (att_retentionpcg != null) {
                        var _retentionpercentage = att_retentionpcg.getValue();

                        if (_retentionpercentage != null && _retentionpercentage > 0) {
                            totAmount = totAmount - (_retentionpercentage * totAmount / 100);

                            if ((dp + t2 + t3 + t4) > totAmount) {
                                dp = totAmount;
                                t2 = 0;
                                t3 = 0;
                                t4 = 0;

                                att_dp.setValue(dp);
                                att_t2.setValue(t2);
                                att_t3.setValue(t3);
                                att_t4.setValue(t4);
                            }

                        }
                    }

                }
            }

            if (top != null) {
                if (top == 1) {
                    if (dp == totAmount) {
                        alert('For CAD, Down Payment can not have the same Amount with ' + totAmount);

                        //if (t2 > 0 && t3 > 0 && t4 > 0) {
                        //    att_dp.setValue(totAmount - (t2 + t3 + t4));
                        //}
                        //else if
                        //    (
                        //    (t2 == 0 && t3 == 0 && t4 == 0) ||
                        //    (t2 > 0 && t3 == 0 && t4 == 0) ||
                        //    (t2 == 0 && t3 > 0 && t4 == 0) ||
                        //    (t2 == 0 && t3 == 0 && t4 > 0) ||
                        //    (t2 > 0 && t3 > 0 && t4 == 0) ||
                        //    (t2 > 0 && t3 == 0 && t4 > 0) ||
                        //    (t2 == 0 && t3 > 0 && t4 > 0)
                        //) {
                        //    att_dp.setValue(0);
                        //}

                        dp = 0;
                        t2 = 0;
                        t3 = 0;
                        t4 = 0;

                        att_dp.setValue(dp);
                        att_t2.setValue(t2);
                        att_t3.setValue(t3);
                        att_t4.setValue(t4);
                    }
                }
            }

            // AUTOMATIC CALCULATION
            // 1 COMBINATION
            if (dp > 0 && t2 == 0 && t3 == 0 && t4 == 0) {
                if (dp > totAmount) {
                    dp = totAmount;
                }

                var _newvalue = totAmount - dp;

                att_dp.setValue(dp);
                att_t2.setValue(_newvalue);
                att_t3.setValue(0);
                att_t4.setValue(0);
            }
            else if (dp == 0 && t2 > 0 && t3 == 0 && t4 == 0) {
                if (t2 > totAmount) {
                    t2 = totAmount;
                }

                var _newvalue = totAmount - t2;

                att_dp.setValue(_newvalue);
                att_t2.setValue(t2);
                att_t3.setValue(0);
                att_t4.setValue(0);
            }
            else if (dp == 0 && t2 == 0 && t3 > 0 && t4 == 0) {
                if (t3 > totAmount) {
                    t3 = totAmount;
                }

                var _newvalue = totAmount - t3;

                att_dp.setValue(_newvalue);
                att_t2.setValue(0);
                att_t3.setValue(t3);
                att_t4.setValue(0);
            }
            else if (dp == 0 && t2 == 0 && t3 == 0 && t4 > 0) {
                if (t4 > totAmount) {
                    t4 = totAmount;
                }

                var _newvalue = totAmount - t4;

                att_dp.setValue(_newvalue);
                att_t2.setValue(0);
                att_t3.setValue(0);
                att_t4.setValue(t4);
            }

                // 2 COMBINATIONS
            else if (dp > 0 && t2 > 0 && t3 == 0 && t4 == 0) {
                if ((dp + t2) > totAmount) {
                    var _newvalue = totAmount - dp;

                    att_dp.setValue(dp);
                    att_t2.setValue(_newvalue);
                    att_t3.setValue(0);
                    att_t4.setValue(0);
                }
                else {
                    var _newvalue = totAmount - (dp + t2);

                    att_dp.setValue(dp);
                    att_t2.setValue(t2);
                    att_t3.setValue(_newvalue);
                    att_t4.setValue(0);
                }
            }
            else if (dp > 0 && t2 == 0 && t3 > 0 && t4 == 0) {
                if ((dp + t3) > totAmount) {
                    var _newvalue = totAmount - dp;

                    att_dp.setValue(dp);
                    att_t2.setValue(0);
                    att_t3.setValue(_newvalue);
                    att_t4.setValue(0);
                }
                else {
                    var _newvalue = totAmount - (dp + t3);

                    att_dp.setValue(dp);
                    att_t2.setValue(_newvalue);
                    att_t3.setValue(t3);
                    att_t4.setValue(0);
                }
            }
            else if (dp > 0 && t2 == 0 && t3 == 0 && t4 > 0) {
                if ((dp + t4) > totAmount) {
                    var _newvalue = totAmount - dp;

                    att_dp.setValue(dp);
                    att_t2.setValue(0);
                    att_t3.setValue(0);
                    att_t4.setValue(_newvalue);
                }
                else {
                    var _newvalue = totAmount - (dp + t4);

                    att_dp.setValue(dp);
                    att_t2.setValue(_newvalue);
                    att_t3.setValue(0);
                    att_t4.setValue(t4);
                }
            }
            else if (dp == 0 && t2 > 0 && t3 > 0 && t4 == 0) {
                if ((t2 + t3) > totAmount) {
                    var _newvalue = totAmount - t2;

                    att_dp.setValue(0);
                    att_t2.setValue(t2);
                    att_t3.setValue(_newvalue);
                    att_t4.setValue(0);
                }
                else {
                    var _newvalue = totAmount - (t2 + t3);

                    att_dp.setValue(_newvalue);
                    att_t2.setValue(t2);
                    att_t3.setValue(t3);
                    att_t4.setValue(0);
                }
            }
            else if (dp == 0 && t2 > 0 && t3 == 0 && t4 > 0) {
                if ((t2 + t4) > totAmount) {
                    var _newvalue = totAmount - t2;

                    att_dp.setValue(0);
                    att_t2.setValue(t2);
                    att_t3.setValue(0);
                    att_t4.setValue(_newvalue);
                }
                else {
                    var _newvalue = totAmount - (t2 + t4);

                    att_dp.setValue(_newvalue);
                    att_t2.setValue(t2);
                    att_t3.setValue(0);
                    att_t4.setValue(t4);
                }
            }
            else if (dp == 0 && t2 == 0 && t3 > 0 && t4 > 0) {
                if ((t3 + t4) > totAmount) {
                    var _newvalue = totAmount - t3;

                    att_dp.setValue(0);
                    att_t2.setValue(0);
                    att_t3.setValue(t3);
                    att_t4.setValue(_newvalue);
                }
                else {
                    var _newvalue = totAmount - (t3 + t4);

                    att_dp.setValue(_newvalue);
                    att_t2.setValue(0);
                    att_t3.setValue(t3);
                    att_t4.setValue(t4);
                }
            }

                // 3 COMBINATIONS
            else if (dp > 0 && t2 > 0 && t3 > 0 && t4 == 0) {
                if ((dp + t2 + t3) > totAmount) {
                    var _newvalue = totAmount - (dp + t2);

                    att_dp.setValue(dp);
                    att_t2.setValue(t2);
                    att_t3.setValue(_newvalue);
                    att_t4.setValue(0);
                }
                else {
                    var _newvalue = totAmount - (dp + t2 + t3);

                    att_dp.setValue(dp);
                    att_t2.setValue(t2);
                    att_t3.setValue(t3);
                    att_t4.setValue(_newvalue);
                }
            }
            else if (dp > 0 && t2 > 0 && t3 == 0 && t4 > 0) {
                if ((dp + t2 + t4) > totAmount) {
                    var _newvalue = totAmount - (dp + t2);

                    att_dp.setValue(dp);
                    att_t2.setValue(t2);
                    att_t3.setValue(0);
                    att_t4.setValue(_newvalue);
                }
                else {
                    var _newvalue = totAmount - (dp + t2 + t4);

                    att_dp.setValue(dp);
                    att_t2.setValue(t2);
                    att_t3.setValue(_newvalue);
                    att_t4.setValue(t4);
                }
            }
            else if (dp > 0 && t2 == 0 && t3 > 0 && t4 > 0) {
                if ((dp + t3 + t4) > totAmount) {
                    var _newvalue = totAmount - (dp + t3);

                    att_dp.setValue(dp);
                    att_t2.setValue(0);
                    att_t3.setValue(t3);
                    att_t4.setValue(_newvalue);
                }
                else {
                    var _newvalue = totAmount - (dp + t3 + t4);

                    att_dp.setValue(dp);
                    att_t2.setValue(_newvalue);
                    att_t3.setValue(t3);
                    att_t4.setValue(t4);
                }
            }
            else if (dp == 0 && t2 > 0 && t3 > 0 && t4 > 0) {
                if ((t2 + t3 + t4) > totAmount) {
                    var _newvalue = totAmount - (t2 + t3);

                    att_dp.setValue(0);
                    att_t2.setValue(t2);
                    att_t3.setValue(t3);
                    att_t4.setValue(_newvalue);
                }
                else {
                    var _newvalue = totAmount - (t2 + t3 + t4);

                    att_dp.setValue(_newvalue);
                    att_t2.setValue(t2);
                    att_t3.setValue(t3);
                    att_t4.setValue(t4);
                }
            }

                // 4 COMBINATIONS
            else if (dp > 0 && t2 > 0 && t3 > 0 && t4 > 0) {
                if ((dp + t2 + t3 + t4) > totAmount) {
                    att_dp.setValue(totAmount);
                    att_t2.setValue(0);
                    att_t3.setValue(0);
                    att_t4.setValue(0);
                }
                else {
                    var _newvalue = totAmount - (dp + t2 + t3);

                    att_dp.setValue(dp);
                    att_t2.setValue(t2);
                    att_t3.setValue(t3);
                    att_t4.setValue(_newvalue);
                }

            }

            if (dp > 0 && t2 > 0 && t3 > 0 && t4 > 0) {
                tot = dp + t2 + t3 + t4;

                if (tot != totAmount) {
                    if (top == 1 && dp == totAmount) {

                    }
                    else {
                        alert("Total must be " + totAmount);
                    }
                }
            }

        }
    }
}