using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Xrm;
using System.Configuration;
using System.Data.OleDb;
using System.Reflection;
using System.IO;
using System.Data.SqlClient;

namespace PortalCRMTraknus
{
    public partial class CPOToSAP : System.Web.UI.Page
    {
        private void ValidationProductAmount(XrmServiceContext crm, SalesOrder etyCPO)
        {
            if (etyCPO.TotalAmount == null)
            {
                throw new Exception("Not found Total Amount in CPO ");
            }
            if (etyCPO.TotalAmount.HasValue == false)
            {
                throw new Exception("Not found Total Amount in CPO ");
            }
            if (etyCPO.QuoteId == null)
            {
                throw new Exception("Not found Quote in CPO ");
            }
            var quot = crm.QuoteSet.Where(q => q.Id.Equals(etyCPO.QuoteId.Id)).FirstOrDefault();
            if (quot == null)
            {
                throw new Exception("Not found Quote in CPO ");
            }
            if (quot.TotalAmount == null)
            {
                throw new Exception("Not found Total Amount in Quote ");
            }
            if (quot.TotalAmount.HasValue == false)
            {
                throw new Exception("Not found Total Amount in Quote ");
            }
            decimal tempDec = etyCPO.new_TotalAmount.Value;
            decimal tempDec2 = 0;
            var pabean = crm.new_pabeanSet.Where(p => p.Id.Equals(etyCPO.new_PabeanLK.Id)).FirstOrDefault();
            if (pabean != null)
            {
                if (pabean.new_AddTax != null)
                {
                    if (pabean.new_AddTax == true) 
                    { 
                        tempDec2 = quot.TotalAmount.Value * ((decimal)1.1); 
                    }
                    else 
                    { 
                        tempDec2 = quot.TotalAmount.Value; 
                    }
                }
                else 
                { 
                    tempDec2 = quot.TotalAmount.Value; 
                }
            }
            else 
            { 
                tempDec2 = quot.TotalAmount.Value; 
            }
            if (tempDec != tempDec2)
            {
                decimal selisih = 0;
                if (tempDec > tempDec2)
                {
                    selisih = tempDec - tempDec2;
                }
                else
                {
                    selisih = tempDec2 - tempDec;
                }
                if (selisih >= 1)
                {
                    throw new Exception("Total Amount in CPO must be " + (decimal.Round(tempDec2, 2).ToString()) + ", current total " + (decimal.Round(tempDec, 2).ToString()));
                }
            }
        }

        private void ProsesCPOtoSAP(XrmServiceContext crm, SalesOrder etyCPO, string PathFile, string PathFileAttachment)
        {
            #region Field

            string cmText = "";
            string orderNumber = "";
            string orderType = "";
            string documentDateYYYYMMDD = "";
            string salesOrg = "";
            string distCannel = "";
            string divisi = "";
            string sold_to_party = "";
            string PO_Number = "";
            string PO_Date = "";
            string Delivery_Plant = "";
            string Doc_Currency = "";
            string Price_List = "";
            string Price_Group = "";
            string Sales_Office = "";
            string Sales_group = "";
            string Business_Sector = "";
            string PKP = "";
            string Reservation = "";
            string Handling_Impotir = "";
            string Document_Required = "";
            string Payment_Term = "";
            string Salesman = "";
            string Delivery_Terms = "";
            string Adress_Map = "";
            string PIC_Title = "";
            string Unit_to_be_operated = "";
            string Delivery_Time = "";
            string Other_Remarks = "";
            string Sales_Manager_Remarks = "";
            string PO_Leasing_Number = "";
            string PO_Leasing_Date = "";
            string DP_Remarks = "";
            string Balance_Remarks = "";
            string Material = "";

            string Pabean = "";
            string BillingDateDP = "";
            string DPAmount = "";
            string BillingDateTermin1 = "";
            string Termin1Amount = "";
            string BillingDateTermin2 = "";
            string Termin2Amount = "";
            string CustomerGroup = "";

            int QTY = 0;
            decimal Price = 0;
            string Unit_Specification = "";
            string LeasingNumber = "";
            string ProductType = "";
            string ItemNumber = "";
            string textToTXTTemp = "";
            Xrm.Product prod = null;
            Xrm.new_division div = null;
            string chechingStatus = "INSERT";

            #endregion

            #region function CRM

            string CPO_Id = etyCPO.Id.ToString();

            Quote quote = null;

            if (etyCPO.QuoteId != null)
            {
                quote = crm.QuoteSet.Where(a => a.Id.Equals(etyCPO.QuoteId.Id)).FirstOrDefault();
            }
            else
            {
                throw new Exception("Not Found Quote in CPO");
            }

            string connectionSQL = ConfigurationManager.AppSettings["connectionSQL"];

            if (File.Exists(PathFile))
            {
                File.SetAttributes(PathFile, FileAttributes.Normal);
                File.Delete(PathFile);
            }

            //System.IO.StreamWriter oWriter = null;

            try
            {
                #region Order Number

                if (etyCPO.OrderNumber != null && etyCPO.OrderNumber != "")
                {
                    orderNumber = etyCPO.OrderNumber.Trim();
                }

                #endregion
                #region ORDER TYPE

                if (etyCPO.new_OrderType != null)
                {
                    var ot = crm.new_ordertypeSet.Where(o => o.Id.Equals(etyCPO.new_OrderType.Id)).FirstOrDefault();
                    if (ot != null)
                    {
                        if (ot.new_Code != null && ot.new_Code != "")
                        {
                            if (ot.new_Code.Length <= 4)
                            {
                                orderType = ot.new_Code;
                            }
                            else
                            {
                                throw new Exception("Failed Proses CPO to SAP, Lenght Order Type Code > 4 in CPO!");
                            }
                        }
                        else
                        {
                            throw new Exception("Failed Proses CPO to SAP, Not Found Order Type Lookup Code in CPO!");
                        }
                    }
                    else
                    {
                        throw new Exception("Failed Proses CPO to SAP, Not Found Order Type in CPO!");
                    }
                }
                else
                {
                    throw new Exception("Failed Proses CPO to SAP, Not Found Order Type in CPO!");
                }
                cmText = cmText + "'" + orderType + "',";
                #endregion
                #region Document Date

                if (etyCPO.new_DocumentDate != null && etyCPO.new_DocumentDate.HasValue)
                {
                    documentDateYYYYMMDD = etyCPO.new_DocumentDate.Value.ToLocalTime().ToString("yyyyMMdd");
                }
                else
                {
                    throw new Exception("Failed Proses CPO to SAP, Not Found Document Date in CPO!");
                }
                cmText = cmText + "'" + orderType + "',";
                #endregion
                #region Distribution Cannel

                if (etyCPO.new_DistChannel != null)
                {
                    var dis = crm.new_distchannelSet.Where(a => a.Id.Equals(etyCPO.new_DistChannel.Id)).FirstOrDefault();
                    if (dis != null && dis.new_Code != null && dis.new_Code != "")
                    {
                        if (dis.new_Code.Length <= 2)
                        {
                            distCannel = dis.new_Code;
                        }
                        else
                        {
                            throw new Exception("Failed Proses CPO to SAP, Lenght Dist. Channel > 2 in CPO!");
                        }
                    }
                    else
                    {
                        throw new Exception("Failed Proses CPO to SAP, Not Found Dist Channel in CPO!");
                    }
                }
                else
                {
                    throw new Exception("Failed Proses CPO to SAP, Not Found Dist Channel in CPO!");
                }
                cmText = cmText + "'" + distCannel + "',";

                #endregion
                #region Sales Organization

                if (etyCPO.new_SalesOrganization != null)
                {
                    var etySalesOrg = crm.new_salesorganizationSet.Where(a => a.Id.Equals(etyCPO.new_SalesOrganization.Id)).FirstOrDefault();
                    if (etySalesOrg != null && etySalesOrg.new_Code != null)
                    {
                        salesOrg = etySalesOrg.new_Code;
                    }
                    else
                    {
                        throw new Exception("Failed Proses CPO to SAP, Lenght Sales Organization > 4 in CPO!");
                    }
                }
                else
                {
                    throw new Exception("Failed Proses CPO to SAP, Not Found Sales Organization in CPO!");
                }
                cmText = cmText + "'" + salesOrg + "',";

                #endregion
                //Divisi ada di product
                #region Sales Office

                if (etyCPO.new_SalesOffice != null)
                {
                    var salOff = crm.new_deliveryplantSet.Where(a => a.Id.Equals(etyCPO.new_SalesOffice.Id)).FirstOrDefault();
                    if (salOff != null && salOff.new_name != null && salOff.new_name != "")
                    {
                        if (salOff.new_name.Length <= 4)
                        {
                            Sales_Office = salOff.new_name;
                        }
                        else
                        {
                            throw new Exception("Failed Proses CPO to SAP, Lenght Sales Office > 4 in CPO!");
                        }
                    }
                    else
                    {
                        throw new Exception("Failed Proses CPO to SAP, Not Found Sales Office in CPO!");
                    }
                }
                else
                {
                    throw new Exception("Failed Proses CPO to SAP, Not Found Sales Office in CPO!");
                }
                cmText = cmText + "'" + Sales_Office + "',";

                #endregion
                #region sold to party

                var acc = crm.AccountSet.Where(a => a.Id.Equals(etyCPO.CustomerId.Id)).FirstOrDefault();
                if (acc != null)
                {
                    if (etyCPO.new_SalesOrganization == null)
                    {
                        throw new Exception("Failed Proses CPO to SAP, Not Found Sales Organization in CPO!");
                    }
                    var so = crm.new_salesorganizationSet.Where(a => a.Id.Equals(etyCPO.new_SalesOrganization.Id)).FirstOrDefault();
                    if (so == null)
                    {
                        throw new Exception("Failed Proses CPO to SAP, Not Found Sales Organization in CPO!");
                    }

                    if (so != null)
                    {
                        if (so.new_Code.Trim() == "A000")
                        {
                            if (acc.AccountNumber != null && acc.AccountNumber != "")
                            {
                                if (acc.AccountNumber.Length <= 10)
                                {
                                    sold_to_party = acc.AccountNumber;
                                }
                                else
                                {
                                    throw new Exception("Failed Proses CPO to SAP, Lenght AccountNumber TN > 10 in Customer!");
                                }
                            }
                            else
                            {
                                throw new Exception("Failed Proses CPO to SAP, Not Found Account Number TN in Account!");
                            }
                        }
                        else if (so.new_Code.Trim() == "S000")
                        {
                            if (acc.new_AccountNumberSHN != null && acc.new_AccountNumberSHN != "")
                            {
                                if (acc.new_AccountNumberSHN.Length <= 10)
                                {
                                    sold_to_party = acc.new_AccountNumberSHN;
                                }
                                else
                                {
                                    throw new Exception("Failed Proses CPO to SAP, Lenght AccountNumber SHN > 10 in Customer!");
                                }
                            }
                            else
                            {
                                throw new Exception("Failed Proses CPO to SAP, Not Found Account Number SHN in Account!");
                            }
                        }
                        else
                        {
                            throw new Exception("Failed Proses CPO to SAP, Not Found Code A000 or S000 in Sales Organization CPO!");
                        }
                    }
                    cmText = cmText + "'" + sold_to_party + "',";
                }
                else
                {
                    throw new Exception("Failed Proses CPO to SAP, Not Found Customer in CPO!");
                }

                #endregion
                #region Biling Date DP

                if (etyCPO.CreatedOn != null)
                {
                    BillingDateDP = etyCPO.CreatedOn.Value.ToLocalTime().ToString("yyyyMMdd");
                }
                else
                {
                    throw new Exception("Failed Proses CPO to SAP, Not Found Create On in CPO!");
                }
                cmText = cmText + "'" + BillingDateDP + "',";

                #endregion

                decimal dp = 0;
                decimal t2 = 0;
                decimal t3 = 0;
                decimal t4 = 0;
                decimal tot = 0;

                string bd1 = "00000000";
                string bd2 = "00000000";
                string bd3 = "00000000";
                string bd4 = "00000000";

                if (quote.TotalAmount != null && quote.TotalAmount.HasValue)
                {
                    tot = quote.TotalAmount.Value;
                }
                string s = quote.new_QuotationNumber;
                // 0 ==> percentage
                // 1 ==> write in amount
                if (quote.new_SelectPayment == false)
                {
                    if (quote.new_TermofPayment == true)
                    {
                        #region CAD Percentege

                        #region Down Payment

                        if (quote.new_DownPayment != null && quote.new_DownPayment.HasValue)
                        {
                            dp = quote.new_DownPayment.Value;
                            if (quote.CreatedOn.HasValue)
                            {
                                bd1 = etyCPO.CreatedOn.Value.ToString("yyyyMMdd");
                            }
                            else
                            {
                                throw new Exception("Failed Proses CPO to SAP, Not Found Create On in Quote!");
                            }
                        }

                        #endregion

                        #region Payment 2

                        if (quote.new_Payment2 != null && quote.new_Payment2.HasValue)
                        {
                            t2 = quote.new_Payment2.Value;
                            if (quote.new_daytierII.HasValue)
                            {
                                bd2 = BillingDateTermin1 = (quote.CreatedOn.Value.AddDays((double)quote.new_daytierII.Value)).ToString("yyyyMMdd");
                            }
                            else
                            {
                                throw new Exception("Failed Proses CPO to SAP, Not Found Day Tier II in Quote!");
                            }
                        }

                        #endregion

                        #region Payment 3

                        if (quote.new_Payment3 != null && quote.new_Payment3.HasValue)
                        {
                            t3 = quote.new_Payment3.Value;
                            if (quote.new_daytierIII.HasValue)
                            {
                                bd3 = (quote.CreatedOn.Value.AddDays((double)quote.new_daytierII.Value + (double)quote.new_daytierIII.Value)).ToString("yyyyMMdd");
                            }
                            else
                            {
                                throw new Exception("Failed Proses CPO to SAP, Not Found Day Tier III in Quote!");
                            }
                        }

                        #endregion

                        #region Payment 4

                        if (quote.new_Payment4 != null && quote.new_Payment4.HasValue)
                        {
                            t4 = quote.new_Payment4.Value;
                            if (quote.new_daytierIV.HasValue)
                            {
                                bd4 = (quote.CreatedOn.Value.AddDays((double)quote.new_daytierII.Value + (double)quote.new_daytierIII.Value) + quote.CreatedOn.Value.AddDays((double)quote.new_daytierIV.Value).ToString("yyyyMMdd"));
                            }
                            else
                            {
                                throw new Exception("Failed Proses CPO to SAP, Not Found Day Tier IV in Quote!");
                            }
                        }

                        #endregion

                        if ((dp + t2 + t3 + t4) != 100)
                        {
                            throw new Exception("Total must be 100.00");
                        }
                        else
                        {
                            dp = (tot * (dp / 100));
                            t2 = (tot * (t2 / 100));
                            t3 = (tot * (t3 / 100));
                            t4 = (tot * (t4 / 100));
                        }

                        #endregion
                    }
                    else
                    {
                        #region CBD Percentage OLD

                        //if (quote.new_Payment != 100)
                        //{
                        //    throw new Exception("Total must be 100.00");
                        //}
                        //else
                        //{
                        //    dp = (tot);
                        //    if (quote.CreatedOn.HasValue)
                        //    {
                        //        bd1 = etyCPO.CreatedOn.Value.ToString("yyyyMMdd");
                        //    }
                        //    else
                        //    {
                        //        throw new Exception("Failed Proses CPO to SAP, Not Found Create On in Quote!");
                        //    }
                        //    t2 = 0;
                        //    t3 = 0;
                        //    t4 = 0;

                        //    bd2 = "00000000";
                        //    bd3 = "00000000";
                        //    bd4 = "00000000";
                        //}

                        #endregion

                        #region CBD Percentage

                        #region Down Payment

                        if (quote.new_DownPayment != null && quote.new_DownPayment.HasValue)
                        {
                            dp = quote.new_DownPayment.Value;
                            if (quote.CreatedOn.HasValue)
                            {
                                bd1 = etyCPO.CreatedOn.Value.ToString("yyyyMMdd");
                            }
                            else
                            {
                                throw new Exception("Failed Proses CPO to SAP, Not Found Create On in Quote!");
                            }
                        }

                        #endregion

                        #region Payment 2

                        if (quote.new_Payment2 != null && quote.new_Payment2.HasValue)
                        {
                            t2 = quote.new_Payment2.Value;
                            if (quote.new_daytierII.HasValue)
                            {
                                bd2 = BillingDateTermin1 = (quote.CreatedOn.Value.AddDays((double)quote.new_daytierII.Value)).ToString("yyyyMMdd");
                            }
                            else
                            {
                                throw new Exception("Failed Proses CPO to SAP, Not Found Day Tier II in Quote!");
                            }
                        }

                        #endregion

                        #region Payment 3

                        if (quote.new_Payment3 != null && quote.new_Payment3.HasValue)
                        {
                            t3 = quote.new_Payment3.Value;
                            if (quote.new_daytierIII.HasValue)
                            {
                                bd3 = (quote.CreatedOn.Value.AddDays((double)quote.new_daytierII.Value + (double)quote.new_daytierIII.Value)).ToString("yyyyMMdd");
                            }
                            else
                            {
                                throw new Exception("Failed Proses CPO to SAP, Not Found Day Tier III in Quote!");
                            }
                        }

                        #endregion

                        #region Payment 4

                        if (quote.new_Payment4 != null && quote.new_Payment4.HasValue)
                        {
                            t4 = quote.new_Payment4.Value;
                            if (quote.new_daytierIV.HasValue)
                            {
                                bd4 = (quote.CreatedOn.Value.AddDays((double)quote.new_daytierII.Value + (double)quote.new_daytierIII.Value) + quote.CreatedOn.Value.AddDays((double)quote.new_daytierIV.Value).ToString("yyyyMMdd"));
                            }
                            else
                            {
                                throw new Exception("Failed Proses CPO to SAP, Not Found Day Tier IV in Quote!");
                            }
                        }

                        #endregion

                        if ((dp + t2 + t3 + t4) != 100)
                        {
                            throw new Exception("Total must be 100.00");
                        }
                        else
                        {
                            dp = tot;
                        }

                        t2 = 0;
                        t3 = 0;
                        t4 = 0;

                        bd2 = "00000000";
                        bd3 = "00000000";
                        bd4 = "00000000";

                        #endregion
                    }
                }
                else
                {
                    if (quote.new_TermofPayment == true)
                    {
                        #region CAD WriteIn

                        #region Down Payment

                        if (quote.new_DownPaymentWriteIn != null && quote.new_DownPaymentWriteIn.HasValue)
                        {
                            dp = quote.new_DownPaymentWriteIn.Value;
                            if (quote.CreatedOn.HasValue)
                            {
                                bd1 = etyCPO.CreatedOn.Value.ToString("yyyyMMdd");
                            }
                            else
                            {
                                throw new Exception("Failed Proses CPO to SAP, Not Found Create On in Quote!");
                            }
                        }

                        #endregion

                        #region Payment 2

                        if (quote.new_PaymentIIWritein != null && quote.new_PaymentIIWritein.HasValue)
                        {
                            t2 = quote.new_PaymentIIWritein.Value;
                            if (quote.new_daytierII.HasValue)
                            {
                                bd2 = (quote.CreatedOn.Value.AddDays((double)quote.new_daytierII.Value)).ToString("yyyyMMdd");
                            }
                            else
                            {
                                throw new Exception("Failed Proses CPO to SAP, Not Found Day Tier II in Quote!");
                            }
                        }

                        #endregion

                        #region Payment 3

                        if (quote.new_PaymentIIIWritein != null && quote.new_PaymentIIIWritein.HasValue)
                        {
                            t3 = quote.new_PaymentIIIWritein.Value;
                            if (quote.new_daytierIII.HasValue)
                            {
                                bd3 = (quote.CreatedOn.Value.AddDays((double)quote.new_daytierII.Value + (double)quote.new_daytierIII.Value)).ToString("yyyyMMdd");
                            }
                            else
                            {
                                throw new Exception("Failed Proses CPO to SAP, Not Found Day Tier III in Quote!");
                            }
                        }

                        #endregion

                        #region Payment 4

                        if (quote.new_PaymentIVWritein != null && quote.new_PaymentIVWritein.HasValue)
                        {
                            t4 = quote.new_PaymentIVWritein.Value;
                            if (quote.new_daytierIV.HasValue)
                            {
                                bd4 = (quote.CreatedOn.Value.AddDays((double)quote.new_daytierII.Value + (double)quote.new_daytierIII.Value) + quote.CreatedOn.Value.AddDays((double)quote.new_daytierIV.Value).ToString("yyyyMMdd"));
                            }
                            else
                            {
                                throw new Exception("Failed Proses CPO to SAP, Not Found Day Tier IV in Quote!");
                            }
                        }

                        #endregion

                        if ((dp + t2 + t3 + t4) != quote.TotalAmount.Value)
                        {
                            throw new Exception("Total must be " + decimal.Round(quote.TotalAmount.Value, 0).ToString());
                        }

                        #endregion
                    }
                    else
                    {
                        #region CBD WriteIn OLD

                        //if (quote.new_PaymentWriteIn != null && quote.new_PaymentWriteIn.HasValue)
                        //{
                        //    dp = quote.new_PaymentWriteIn.Value;
                        //    dp = (tot);
                        //    if (quote.CreatedOn.HasValue)
                        //    {
                        //        bd1 = etyCPO.CreatedOn.Value.ToString("yyyyMMdd");
                        //    }
                        //    else
                        //    {
                        //        throw new Exception("Failed Proses CPO to SAP, Not Found Create On in Quote!");
                        //    }
                        //    t2 = 0;
                        //    t3 = 0;
                        //    t4 = 0;

                        //    bd2 = "00000000";
                        //    bd3 = "00000000";
                        //    bd4 = "00000000";
                        //}

                        #endregion

                        #region CAD WriteIn

                        #region Down Payment

                        if (quote.new_DownPaymentWriteIn != null && quote.new_DownPaymentWriteIn.HasValue)
                        {
                            dp = quote.new_DownPaymentWriteIn.Value;
                            if (quote.CreatedOn.HasValue)
                            {
                                bd1 = etyCPO.CreatedOn.Value.ToString("yyyyMMdd");
                            }
                            else
                            {
                                throw new Exception("Failed Proses CPO to SAP, Not Found Create On in Quote!");
                            }
                        }

                        #endregion

                        #region Payment 2

                        if (quote.new_PaymentIIWritein != null && quote.new_PaymentIIWritein.HasValue)
                        {
                            t2 = quote.new_PaymentIIWritein.Value;
                            if (quote.new_daytierII.HasValue)
                            {
                                bd2 = (quote.CreatedOn.Value.AddDays((double)quote.new_daytierII.Value)).ToString("yyyyMMdd");
                            }
                            else
                            {
                                throw new Exception("Failed Proses CPO to SAP, Not Found Day Tier II in Quote!");
                            }
                        }

                        #endregion

                        #region Payment 3

                        if (quote.new_PaymentIIIWritein != null && quote.new_PaymentIIIWritein.HasValue)
                        {
                            t3 = quote.new_PaymentIIIWritein.Value;
                            if (quote.new_daytierIII.HasValue)
                            {
                                bd3 = (quote.CreatedOn.Value.AddDays((double)quote.new_daytierII.Value + (double)quote.new_daytierIII.Value)).ToString("yyyyMMdd");
                            }
                            else
                            {
                                throw new Exception("Failed Proses CPO to SAP, Not Found Day Tier III in Quote!");
                            }
                        }

                        #endregion

                        #region Payment 4

                        if (quote.new_PaymentIVWritein != null && quote.new_PaymentIVWritein.HasValue)
                        {
                            t4 = quote.new_PaymentIVWritein.Value;
                            if (quote.new_daytierIV.HasValue)
                            {
                                bd4 = (quote.CreatedOn.Value.AddDays((double)quote.new_daytierII.Value + (double)quote.new_daytierIII.Value) + quote.CreatedOn.Value.AddDays((double)quote.new_daytierIV.Value).ToString("yyyyMMdd"));
                            }
                            else
                            {
                                throw new Exception("Failed Proses CPO to SAP, Not Found Day Tier IV in Quote!");
                            }
                        }

                        #endregion

                        if ((dp + t2 + t3 + t4) != quote.TotalAmount.Value)
                        {
                            throw new Exception("Total must be " + decimal.Round(quote.TotalAmount.Value, 0).ToString());
                        }
                        else
                        {
                            dp = tot;
                        }

                        t2 = 0;
                        t3 = 0;
                        t4 = 0;

                        bd2 = "00000000";
                        bd3 = "00000000";
                        bd4 = "00000000";

                        #endregion
                    }
                }

                DPAmount = decimal.Round(dp, 2, MidpointRounding.AwayFromZero).ToString();
                BillingDateDP = bd1;
                Termin1Amount = decimal.Round(t2, 2, MidpointRounding.AwayFromZero).ToString();
                BillingDateTermin1 = bd2;
                Termin2Amount = decimal.Round(t3, 2, MidpointRounding.AwayFromZero).ToString();
                BillingDateTermin2 = bd3;

                #region PO NUmber

                if (etyCPO.new_PONumber != null && etyCPO.new_PONumber != "")
                {
                    if (etyCPO.new_PONumber.Length <= 35)
                    {
                        PO_Number = etyCPO.new_PONumber;
                    }
                    else
                    {
                        //throw new Exception("Failed Proses CPO to SAP, Lenght PO Number > 35 in CPO!");
                    }
                }
                else
                {
                    //throw new Exception("Failed Proses CPO to SAP, Not Found PO Number in CPO!");
                }
                cmText = cmText + "'" + PO_Number + "',";

                #endregion
                #region PO Date

                if (etyCPO.new_PODate != null && etyCPO.new_PODate.HasValue)
                {
                    PO_Date = etyCPO.new_PODate.Value.ToLocalTime().ToString("yyyyMMdd");
                }
                else
                {
                    //throw new Exception("Failed Proses CPO to SAP, Not Found PO Date in CPO!");
                }
                cmText = cmText + "'" + PO_Date + "',";

                #endregion
                #region Doc Currency

                if (etyCPO.TransactionCurrencyId != null)
                {
                    var transCur = crm.TransactionCurrencySet.Where(a => a.Id.Equals(etyCPO.TransactionCurrencyId.Id)).FirstOrDefault();
                    if (transCur != null)
                    {
                        if (transCur.ISOCurrencyCode.Length <= 5)
                        {
                            Doc_Currency = transCur.ISOCurrencyCode;
                        }
                        else
                        {
                            throw new Exception("Failed Proses CPO to SAP, Lenght Currency > 5 in CPO!");
                        }
                    }
                    else
                    {
                        throw new Exception("Failed Proses CPO to SAP, Not Found Currency in CPO!");
                    }
                }
                else
                {
                    throw new Exception("Failed Proses CPO to SAP, Not Currency in CPO!");
                }
                cmText = cmText + "'" + Doc_Currency + "',";

                #endregion
                #region Payment Term

                if (etyCPO.new_PaymentTerm != null)
                {
                    var payTerm = crm.new_paymenttermSet.Where(p => p.Id.Equals(etyCPO.new_PaymentTerm.Id)).FirstOrDefault();
                    if (payTerm != null)
                    {
                        if (payTerm.new_Code.Length <= 4)
                        {
                            Payment_Term = payTerm.new_Code;
                        }
                        else
                        {
                            throw new Exception("Failed Proses CPO to SAP, Lenght Payment Term > 4 in CPO!");
                        }
                    }
                    else
                    {
                        throw new Exception("Failed Proses CPO to SAP, Not Found Payment Term in CPO!");
                    }
                }
                else
                {
                    throw new Exception("Failed Proses CPO to SAP, Not Found Payment Term in CPO!");
                }
                cmText = cmText + "'" + Payment_Term + "',";

                #endregion
                #region Delivery Plant

                if (etyCPO.new_DeliveryPlant != null)
                {
                    var delPlan = crm.new_deliveryplantSet.Where(a => a.Id.Equals(etyCPO.new_DeliveryPlant.Id)).FirstOrDefault();
                    if (delPlan != null && delPlan.new_name != null)
                    {
                        if (delPlan.new_name.Length <= 4)
                        {
                            Delivery_Plant = delPlan.new_name;
                        }
                        else
                        {
                            throw new Exception("Failed Proses CPO to SAP, Lenght Delivery Plant > 4 in CPO!");
                        }
                    }
                    else
                    {
                        throw new Exception("Failed Proses CPO to SAP, Not Found Delivery Plant in CPO!");
                    }
                }
                else
                {
                    throw new Exception("Failed Proses CPO to SAP, Not Found Delivery Plant in CPO!");
                }
                cmText = cmText + "'" + Delivery_Plant + "',";

                #endregion
                #region NRP

                if (etyCPO.OwnerId != null)
                {
                    var user = crm.SystemUserSet.Where(a => a.Id.Equals(etyCPO.OwnerId.Id)).FirstOrDefault();
                    if (user != null && user.new_NRP != null && user.new_NRP != "")
                    {
                        if (user.new_NRP.Length <= 8)
                        {
                            Salesman = user.new_NRP;
                        }
                        else
                        {
                            throw new Exception("Failed Proses CPO to SAP, Lenght NRP > 4 in BC!");
                        }
                    }
                    else
                    {
                        throw new Exception("Failed Proses CPO to SAP, Not Found NRP in BC!");
                    }
                }
                else
                {
                    throw new Exception("Failed Proses CPO to SAP, Not Found BC in CPO!");
                }
                cmText = cmText + "'" + Salesman + "',";

                #endregion
                #region Paben Non Pabean

                if (etyCPO.new_PabeanLK != null)
                {
                    var pabeanLK = crm.new_pabeanSet.Where(p => p.Id.Equals(etyCPO.new_PabeanLK.Id)).FirstOrDefault();
                    if (pabeanLK != null && pabeanLK.new_Code != null && pabeanLK.new_Code != "" && pabeanLK.new_Code.Trim() != null)
                    {
                        Pabean = pabeanLK.new_Code;
                    }
                    else
                    {
                        throw new Exception("Failed Proses CPO to SAP, Not Found Pabean in CPO!");
                    }
                }
                else
                {
                    throw new Exception("Failed Proses CPO to SAP, Not Found Pabean in CPO!");
                }
                cmText = cmText + "'" + Pabean + "',";

                #endregion
                #region Customer Group
                if (acc != null && acc.new_BusinessGroup != null)
                {
                    crm.ClearChanges();
                    var busGroup = crm.new_businessgroupSet.Where(a => a.Id.Equals(acc.new_BusinessGroup.Id)).FirstOrDefault();
                    if (busGroup != null && busGroup.new_Code != null && busGroup.new_Code != "")
                    {
                        if (busGroup.new_Code.Length <= 2)
                        {
                            CustomerGroup = busGroup.new_Code;
                        }
                        else
                        {
                            throw new Exception("Failed Proses CPO to SAP, Lenght Business Group > 2 in CPO!");
                        }
                    }
                    else
                    {
                        throw new Exception("Failed Proses CPO to SAP, Not Found Business Group in Account!");
                    }
                }
                else
                {
                    throw new Exception("Failed Proses CPO to SAP, Not Found Business Group in Account!");
                }

                #endregion
                #region Price List

                if (etyCPO.new_PriceListCPO != null)
                {
                    var pl = crm.new_pricelistcpoSet.Where(a => a.Id.Equals(etyCPO.new_PriceListCPO.Id)).FirstOrDefault();
                    if (pl != null)
                    {
                        if (pl.new_Code != null && pl.new_Code != "")
                        {
                            if (pl.new_Code.Length <= 5)
                            {
                                Price_List = pl.new_Code;
                            }
                            else
                            {
                                throw new Exception("Failed Proses CPO to SAP, Lenght Price List > 5 in CPO!");
                            }
                        }
                        else
                        {
                            throw new Exception("Failed Proses CPO to SAP, Price List in CPO!");
                        }
                    }
                }
                else
                {
                    throw new Exception("Failed Proses CPO to SAP, Price List in CPO!");
                }
                cmText = cmText + "'" + Price_List + "',";

                #endregion
                #region Price Group

                if (etyCPO.new_PriceGroup != null)
                {
                    var pg = crm.new_pricegroupSet.Where(p => p.Id.Equals(etyCPO.new_PriceGroup.Id)).FirstOrDefault();
                    if (pg != null)
                    {
                        if (pg.new_Code != null && pg.new_Code != "")
                        {
                            if (pg.new_Code.Length <= 2)
                            {
                                Price_Group = pg.new_Code;
                            }
                            else
                            {
                                throw new Exception("Failed Proses CPO to SAP, Lenght Price List > 2 in CPO!");
                            }
                        }
                        else
                        {
                            throw new Exception("Failed Proses CPO to SAP, Not Found Price Group in CPO!");
                        }
                    }
                    else
                    {
                        throw new Exception("Failed Proses CPO to SAP, Not Found Price Group in CPO!");
                    }
                }
                else
                {
                    throw new Exception("Failed Proses CPO to SAP, Not Found Price Group in CPO!");
                }
                cmText = cmText + "'" + Price_Group + "',";

                #endregion
                #region Business Sector

                if (acc != null && acc.new_BusinessSector != null)
                {
                    crm.ClearChanges();
                    var busSec = crm.new_businesssectorSet.Where(a => a.Id.Equals(acc.new_BusinessSector.Id)).FirstOrDefault();
                    if (busSec != null && (busSec.new_Code != null || busSec.new_Code != ""))
                    {
                        if (busSec.new_Code.Length <= 2)
                        {
                            Business_Sector = busSec.new_Code;
                        }
                        else
                        {
                            throw new Exception("Failed Proses CPO to SAP, Length Business Sector > 3 in CPO!");
                        }
                    }
                    else
                    {
                        throw new Exception("Failed Proses CPO to SAP, Not Found Business Sector in Customer!");
                    }
                }
                else
                {
                    throw new Exception("Failed Proses CPO to SAP, Not Found Business Sector in Customer!");
                }
                cmText = cmText + "'" + Business_Sector + "',";

                #endregion
                #region PKP

                if (acc != null && acc.new_TaxStatus != null)
                {
                    var tax = crm.new_taxSet.Where(a => a.Id.Equals(acc.new_TaxStatus.Id)).FirstOrDefault();
                    if (tax != null && tax.new_Code != null && tax.new_Code != "")
                    {
                        if (tax.new_Code.Length <= 3)
                        {
                            PKP = tax.new_Code;
                        }
                        else
                        {
                            throw new Exception("Failed Proses CPO to SAP, Lenght Tax Status > 3 in CPO!");
                        }
                    }
                    else
                    {
                        throw new Exception("Failed Proses CPO to SAP, Not Found Tax Status in Customer!");
                    }
                }
                else
                {
                    throw new Exception("Failed Proses CPO to SAP, Not Found Tax Status in Customer!");
                }
                cmText = cmText + "'" + PKP + "',";

                #endregion
                #region Reservation
                if (etyCPO.new_Reservation != null)
                {
                    var res = crm.new_reservationSet.Where(a => a.Id.Equals(etyCPO.new_Reservation.Id)).FirstOrDefault();
                    if (res != null && res.new_Code != null && res.new_Code != "")
                    {
                        if (res.new_Code.Length <= 3)
                        {
                            Reservation = res.new_Code;
                        }
                        else
                        {
                            throw new Exception("Failed Proses CPO to SAP, Lenght Reservation > 3 in CPO!");
                        }
                    }
                    else
                    {
                        throw new Exception("Failed Proses CPO to SAP, Not Found Reservation in CPO!");
                    }
                }
                else
                {
                    throw new Exception("Failed Proses CPO to SAP, Not Found Reservation in CPO!");
                }
                cmText = cmText + "'" + Reservation + "',";

                #endregion
                #region HandlingImpotir

                if (etyCPO.new_HandlingImport != null)
                {
                    var HanIm = crm.new_handlingimportSet.Where(a => a.Id.Equals(etyCPO.new_HandlingImport.Id)).FirstOrDefault();
                    if (HanIm != null && HanIm.new_Code != null && HanIm.new_Code != "")
                    {
                        if (HanIm.new_Code.Length <= 3)
                        {
                            Handling_Impotir = HanIm.new_Code;
                        }
                        else
                        {
                            throw new Exception("Failed Proses CPO to SAP, Lenght Handling Import > 3 in CPO!");
                        }
                    }
                    else
                    {
                        throw new Exception("Failed Proses CPO to SAP, Not Found Handling Import in CPO!");
                    }
                }
                else
                {
                    throw new Exception("Failed Proses CPO to SAP, Not Found Handling Import in CPO!");
                }
                cmText = cmText + "'" + Handling_Impotir + "',";

                #endregion
                #region Document Required

                if (etyCPO.new_DocumentRequired != null)
                {
                    var DocReq = crm.new_documentrequiredSet.Where(a => a.Id.Equals(etyCPO.new_DocumentRequired.Id)).FirstOrDefault();
                    if (DocReq != null && DocReq.new_Code != null && DocReq.new_Code != "")
                    {
                        if (DocReq.new_Code.Length <= 3)
                        {
                            Document_Required = DocReq.new_Code;
                        }
                        else
                        {
                            throw new Exception("Failed Proses CPO to SAP, Lenght Document Required > 3 in CPO!");
                        }
                    }
                    else
                    {
                        throw new Exception("Failed Proses CPO to SAP, Not Found Document Required in CPO!");
                    }
                }
                else
                {
                    throw new Exception("Failed Proses CPO to SAP, Not Found Document Required in CPO!");
                }
                cmText = cmText + "'" + Document_Required + "',";

                #endregion
                #region DeliveryTerms
                if (etyCPO.new_DeliveryTerms != null && etyCPO.new_DeliveryTerms != null && etyCPO.new_DeliveryTerms != "")
                {
                    if (etyCPO.new_DeliveryTerms.Length <= 30)
                    {
                        Delivery_Terms = etyCPO.new_DeliveryTerms;
                    }
                    else
                    {
                        throw new Exception("Failed Proses CPO to SAP, Lenght Delivery Terms > 30 in BC!");
                    }
                }
                else
                {
                    throw new Exception("Failed Proses CPO to SAP, Not Found Delivery Term in CPO!");
                }
                cmText = cmText + "'" + Delivery_Terms + "',";

                #endregion
                #region Adrees Map
                if (etyCPO.new_AdressMap != null && etyCPO.new_AdressMap != "")
                {
                    if (etyCPO.new_AdressMap.Length <= 255)
                    {
                        Adress_Map = etyCPO.new_AdressMap;
                    }
                    else
                    {
                        throw new Exception("Failed Proses CPO to SAP, Lenght Adress Map > 255 in BC!");
                    }
                }
                else
                {
                    throw new Exception("Failed Proses CPO to SAP, Not Found Adress Map in CPO!");
                }
                cmText = cmText + "'" + Adress_Map + "',";
                #endregion
                #region PIC Title

                if (etyCPO.new_PICTitle != null && etyCPO.new_PICTitle != "")
                {
                    if (etyCPO.new_PICTitle.Length <= 30)
                    {
                        PIC_Title = etyCPO.new_PICTitle;
                    }
                    else
                    {
                        throw new Exception("Failed Proses CPO to SAP, Lenght PIC Title > 30 in CPO!");
                    }
                }
                else
                {
                    throw new Exception("Failed Proses CPO to SAP, Not Found PIC Title in CPO!");
                }
                cmText = cmText + "'" + PIC_Title + "',";

                #endregion
                #region Unit To Be Operated In

                if (etyCPO.new_UnitToBeOperatedIn != null && etyCPO.new_UnitToBeOperatedIn != "")
                {
                    if (etyCPO.new_UnitToBeOperatedIn.Length <= 30)
                    {
                        Unit_to_be_operated = etyCPO.new_UnitToBeOperatedIn;
                    }
                    else
                    {
                        throw new Exception("Failed Proses CPO to SAP, Lenght Unit To Be Operated In > 30 in CPO!");
                    }
                }
                else
                {
                    throw new Exception("Failed Proses CPO to SAP, Not Found Unit To Be Operated In in CPO!");
                }
                cmText = cmText + "'" + Unit_to_be_operated + "',";

                #endregion
                #region Delivery Time

                if (etyCPO.new_DeliveryTime != null && etyCPO.new_DeliveryTime.HasValue)
                {
                    Delivery_Time = etyCPO.new_DeliveryTime.Value.ToLocalTime().ToString("MMMM yyyy");
                }
                else
                {
                    throw new Exception("Failed Proses CPO to SAP, Not Found Delivery Time in CPO!");
                }
                cmText = cmText + "'" + Delivery_Time + "',";

                #endregion
                #region Other Remarks

                if (etyCPO.new_OtherRemarks != null)
                {
                    if (etyCPO.new_OtherRemarks.Length <= 255)
                    {
                        Other_Remarks = etyCPO.new_OtherRemarks;
                    }
                    else
                    {
                        throw new Exception("Failed Proses CPO to SAP, Lenght Other Remarks > 255 in CPO!");
                    }
                }
                else
                {
                    Other_Remarks = "";
                    //throw new Exception("Failed Proses CPO to SAP, Not Found Other Remarks in CPO!");
                }
                cmText = cmText + "'" + Other_Remarks + "',";

                #endregion
                #region Sales Manager Remarks

                if (etyCPO.new_SalesManagerRemarks != null && etyCPO.new_SalesManagerRemarks != "")
                {
                    if (etyCPO.new_SalesManagerRemarks.Length <= 255)
                    {
                        Sales_Manager_Remarks = etyCPO.new_SalesManagerRemarks;
                    }
                    else
                    {
                        throw new Exception("Failed Proses CPO to SAP, Lenght Sales Manager Remarks > 255 in CPO!");
                    }
                }
                else
                {
                    throw new Exception("Failed Proses CPO to SAP, Not Found Sales Manager Remarks in CPO!");
                }
                cmText = cmText + "'" + Sales_Manager_Remarks + "',";

                #endregion
                #region PO Leasing Number

                if (etyCPO.new_POLeasingNumber != null)
                {
                    if (etyCPO.new_POLeasingNumber.Length <= 30)
                    {
                        PO_Leasing_Number = etyCPO.new_POLeasingNumber;
                    }
                    else
                    {
                        throw new Exception("Failed Proses CPO to SAP, Lenght PO Leasing Number > 30 in CPO!");
                    }
                }
                else
                {
                    PO_Leasing_Number = "";
                    //throw new Exception("Failed Proses CPO to SAP, Not Found PO Leasing Number in CPO!");
                }
                cmText = cmText + "'" + PO_Leasing_Number + "',";

                #endregion
                #region PO Leasing Date

                if (etyCPO.new_PODate != null && etyCPO.new_PODate.HasValue)
                {
                    PO_Leasing_Date = etyCPO.new_PODate.Value.ToLocalTime().ToString("yyyyMMdd"); ;
                }
                else
                {
                    PO_Leasing_Date = "";
                }

                #endregion
                #region DP Remarks

                if (etyCPO.new_DPRemarks != null)
                {
                    if (etyCPO.new_DPRemarks.Length <= 255)
                    {
                        DP_Remarks = etyCPO.new_DPRemarks;
                    }
                    else
                    {
                        throw new Exception("Failed Proses CPO to SAP, Lenght DP Remarks > 255 in CPO!");
                    }
                }
                else
                {
                    throw new Exception("Failed Proses CPO to SAP, Not Found DP Remarks in CPO!");
                }
                cmText = cmText + "'" + DP_Remarks + "',";

                #endregion
                #region Balance Remarks

                if (etyCPO.new_BalanceRemarks != null)
                {
                    if (etyCPO.new_BalanceRemarks.Length <= 255)
                    {
                        Balance_Remarks = etyCPO.new_BalanceRemarks;
                    }
                    else
                    {
                        throw new Exception("Failed Proses CPO to SAP, Lenght Balance Remarks > 255 in CPO!");
                    }
                }
                else
                {
                    throw new Exception("Failed Proses CPO to SAP, Not Found Balance Remarks in CPO!");
                }
                cmText = cmText + "'" + Balance_Remarks + "',";

                #endregion
                #region LeasingNumber

                if (etyCPO.new_Leasing != null)
                {
                    crm.ClearChanges();
                    var Leasing = crm.new_leasingSet.Where(a => a.Id.Equals(etyCPO.new_Leasing.Id)).FirstOrDefault();
                    if (Leasing != null)
                    {
                        LeasingNumber = Leasing.new_Code;
                    }
                    else
                    {
                        LeasingNumber = "";
                    }
                }
                else
                {
                    LeasingNumber = "";
                }

                #endregion
                //Material di Product
                //QTY di Product
                //Price di Product
                //Unit Specification di Product
                #region ety Product CPO

                var etyProdCPOs = crm.SalesOrderDetailSet.Where(sd => sd.SalesOrderId.Id.Equals(CPO_Id));
                if (etyProdCPOs == null)
                {
                    throw new Exception("Failed Proses CPO to SAP, Not Found CPO Product in CPO!");
                }

                //validasi untuk cek product
                //product hanya boleh 1 saja
                Guid idProduct = Guid.Empty;

                #region Read Data Product

                Xrm.new_materialgroup matGroup = null;
                foreach (SalesOrderDetail itmProd in etyProdCPOs)
                {
                    if (itmProd != null)
                    {
                        prod = null;
                        div = null;
                        matGroup = null;
                        Material = "";
                        divisi = "";
                        Unit_Specification = "";
                        QTY = 0;
                        Price = 0;


                        if (itmProd.ProductId != null)
                        {
                            prod = crm.ProductSet.Where(p => p.Id.Equals(itmProd.ProductId.Id)).FirstOrDefault();
                            if (prod != null)
                            {
                                #region Divisi

                                if (prod.new_Division != null)
                                {
                                    div = crm.new_divisionSet.Where(d => d.Id.Equals(prod.new_Division.Id)).FirstOrDefault();
                                    if (div != null)
                                    {
                                        if (div.new_Code.Length <= 2)
                                        {
                                            divisi = div.new_Code;
                                        }
                                        else
                                        {
                                            throw new Exception("Failed Proses CPO to SAP, Lenght Divisi > 2 in Product!");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Failed Proses CPO to SAP, Not Found Divisi in Product!");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Failed Proses CPO to SAP, Not Found Divisi in Product!");
                                }

                                #endregion

                                #region Material

                                if (prod.ProductNumber != null)
                                {
                                    Material = prod.ProductNumber;
                                }
                                else
                                {
                                    throw new Exception("Failed Proses CPO to SAP, Not Found Product Number in Product!");
                                }

                                #endregion

                                #region Sales Group

                                if (prod.new_SalesGroup != null && prod.new_SalesGroup.Name != null && prod.new_SalesGroup.Name != "")
                                {
                                    if (prod.new_SalesGroup.Name.Length <= 3)
                                    {
                                        Sales_group = prod.new_SalesGroup.Name;
                                    }
                                    else
                                    {
                                        throw new Exception("Failed Proses CPO to SAP, Lenght Sales Group > 3 in Product!");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Failed Proses CPO to SAP, Not Found Sales Group in Product!");
                                }
                                cmText = cmText + "'" + Sales_group + "',";

                                #endregion

                                #region Product Type

                                if (prod.new_IsStandard.HasValue)
                                {
                                    if (prod.new_IsStandard.HasValue)
                                    {
                                        ProductType = prod.new_IsStandard.Value.ToString().ToUpper();
                                    }
                                    else
                                    {
                                        ProductType = "FALSE";
                                    }
                                }
                                else
                                {
                                    ProductType = "FALSE";
                                }

                                #endregion
                            }
                            else
                            {
                                throw new Exception("Failed Proses CPO to SAP, Not Found Product in CPO Product!");
                            }
                        }
                        else
                        {
                            //untuk yang productnya tidak ada ini adalah product custome
                            //ini akan bermasalah untuk isi data divisi,material,unit_specefication
                            throw new Exception("Failed Proses CPO to SAP, Not Found Product in CPO Product!");
                        }

                        #region QTY

                        if (itmProd.Quantity != null && itmProd.Quantity.HasValue)
                        {
                            QTY = (int)itmProd.Quantity.Value;
                        }
                        else
                        {
                            throw new Exception("Failed Proses CPO to SAP, Not Found QTY in CPO Product!");
                        }

                        #endregion

                        #region Price

                        if (itmProd.new_ExtendedAmount != null && itmProd.new_ExtendedAmount.HasValue)
                        {
                            Price = itmProd.new_ExtendedAmount.Value - (itmProd.new_Tax != null ? itmProd.new_Tax.Value : 0);
                        }
                        else
                        {
                            throw new Exception("Failed Proses CPO to SAP, Not Found Extended Amount in CPO Product!");
                        }

                        #endregion

                        #region Unit Specification

                        if (itmProd.Description != null)
                        {
                            Unit_Specification = itmProd.Description;
                        }
                        else
                        {
                            Unit_Specification = "";
                            //throw new Exception("Failed Proses CPO to SAP, Not Found Description in CPO Product!");
                        }

                        #endregion

                        #region Item Number

                        if (itmProd.new_ItemNumber != null && itmProd.new_ItemNumber.HasValue)
                        {
                            ItemNumber = (decimal.Round(itmProd.new_ItemNumber.Value, 0)).ToString();
                        }
                        else
                        {
                            throw new Exception("Not Found Item Number in CPO Product");
                        }

                        #endregion


                        #region ety Product CPO SQL

                        using (SqlConnection cn = new SqlConnection(connectionSQL))
                        {
                            cn.Open();
                            chechingStatus = "INSERT";

                            #region Checking Existing Data

                            using (SqlCommand cmCecking = cn.CreateCommand())
                            {
                                cmCecking.CommandType = CommandType.Text;
                                cmCecking.CommandText = "SELECT * FROM CPO WHERE ID_UNIQ_CPO_CRM='" + etyCPO.Id.ToString() + "' AND ID_UNIQ_CPO_DETAIL_CRM='" + itmProd.Id.ToString() + "' AND ItemNumber='" + decimal.Round(itmProd.new_ItemNumber.Value, 0, MidpointRounding.AwayFromZero).ToString() + "'";
                                using (SqlDataReader dr = cmCecking.ExecuteReader())
                                {
                                    if (dr.Read()) { chechingStatus = "UPDATE"; }
                                }
                            }

                            #endregion

                            SqlTransaction tr = cn.BeginTransaction();
                            try
                            {
                                using (SqlCommand cm = tr.Connection.CreateCommand())
                                {
                                    cm.Transaction = tr;
                                    cm.CommandType = CommandType.Text;
                                    if (chechingStatus == "INSERT")
                                    {
                                        cm.CommandText = "INSERT INTO CPO " +
                                            "(orderNumber,orderType,salesOrg,distCannel,sold_to_party,PO_Number," +
                                            "PO_Date,Delivery_Plant,Doc_Currency," +
                                            "Price_List,Price_Group,Sales_Office,Sales_group," +
                                            "Business_Sector,PKP,Reservation,Handling_Impotir," +
                                            "Document_Required,Payment_Term,Salesman,Delivery_Terms," +
                                            "Adress_Map,PIC_Title,Unit_to_be_operated,Delivery_Time," +
                                            "Other_Remarks,Sales_Manager_Remarks,Leasing_Code,PO_Leasing_Number,PO_Leasing_Date," +
                                            "DP_Remarks,Balance_Remarks,divisi,Material," +
                                            "Unit_Specification,QTY,Price," +
                                            "Paben_Non_Pabean,Billing_Date_DP,DP_Amount,Billing_Date_Termin_1,Termin_1_Amount,Billing_Date_Termin_2,Termin_2_Amount," +
                                            "Customer_Group,Product_Type," +
                                            "ID_UNIQ_CPO_CRM,ID_UNIQ_CPO_DETAIL_CRM,FLAG,ItemNumber,ParentNumber,StatusCPOProduct,modifOn) " +
                                            "VALUES('" + orderNumber + "','" + orderType + "','" + salesOrg + "','" + distCannel + "','" + sold_to_party + "','" +
                                            PO_Number + "','" + PO_Date + "','" + Delivery_Plant + "','" + Doc_Currency + "','" +
                                            Price_List + "','" + Price_Group + "','" + Sales_Office + "','" + Sales_group + "','" +
                                            Business_Sector + "','" + PKP + "','" + Reservation + "','" + Handling_Impotir + "','" +
                                            Document_Required + "','" + Payment_Term + "','" + Salesman + "','" + Delivery_Terms + "','" +
                                            Adress_Map + "','" + PIC_Title + "','" + Unit_to_be_operated + "','" + Delivery_Time + "','" +
                                            Other_Remarks + "','" + Sales_Manager_Remarks + "','" + LeasingNumber + "','" + PO_Leasing_Number + "','" + PO_Leasing_Date + "','" +
                                            DP_Remarks + "','" + Balance_Remarks + "','" + divisi + "','" + Material + "','" +
                                            Unit_Specification + "','" + QTY + "','" + Price + "','" +
                                            Pabean + "','" + BillingDateDP + "','" + DPAmount + "','" +
                                            BillingDateTermin1 + "','" + Termin1Amount + "','" + BillingDateTermin2 + "','" +
                                            Termin2Amount + "','" + CustomerGroup + "','" + ProductType + "','" +
                                            etyCPO.Id.ToString() + "','" + itmProd.Id.ToString() + "','0','" + decimal.Round(itmProd.new_ItemNumber.Value, 0, MidpointRounding.AwayFromZero).ToString() + "'," + (itmProd.new_ParentNumber != null ? "'" + decimal.Round(itmProd.new_ParentNumber.Value, 0, MidpointRounding.AwayFromZero).ToString() + "'" : "NULL") + ",'1','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                                    }
                                    else
                                    {
                                        cm.CommandText = "UPDATE CPO SET " +
                                            "orderNumber='" + orderNumber + "', " +
                                            "orderType='" + orderType + "', " +
                                            "salesOrg='" + salesOrg + "', " +
                                            "distCannel='" + distCannel + "', " +
                                            "sold_to_party='" + sold_to_party + "', " +
                                            "PO_Number='" + PO_Number + "', " +
                                            "PO_Date='" + PO_Date + "', " +
                                            "Delivery_Plant='" + Delivery_Plant + "', " +
                                            "Doc_Currency='" + Doc_Currency + "', " +
                                            "Price_List='" + Price_List + "', " +
                                            "Price_Group='" + Price_Group + "', " +
                                            "Sales_Office='" + Sales_Office + "', " +
                                            "Sales_group='" + Sales_group + "', " +
                                            "Business_Sector='" + Business_Sector + "', " +
                                            "PKP='" + PKP + "', " +
                                            "Reservation='" + Reservation + "', " +
                                            "Handling_Impotir='" + Handling_Impotir + "', " +
                                            "Document_Required='" + Document_Required + "', " +
                                            "Payment_Term='" + Payment_Term + "', " +
                                            "Salesman='" + Salesman + "', " +
                                            "Delivery_Terms='" + Delivery_Terms + "', " +
                                            "Adress_Map='" + Adress_Map + "', " +
                                            "PIC_Title='" + PIC_Title + "', " +
                                            "Unit_to_be_operated='" + Unit_to_be_operated + "', " +
                                            "Delivery_Time='" + Delivery_Time + "', " +
                                            "Other_Remarks='" + Other_Remarks + "', " +
                                            "Sales_Manager_Remarks='" + Sales_Manager_Remarks + "', " +
                                            "Leasing_Code='" + LeasingNumber + "', " +
                                            "PO_Leasing_Number='" + PO_Leasing_Number + "', " +
                                            "PO_Leasing_Date='" + PO_Leasing_Date + "', " +
                                            "DP_Remarks='" + DP_Remarks + "', " +
                                            "Balance_Remarks='" + Balance_Remarks + "', " +
                                            "divisi='" + divisi + "', " +
                                            "Material='" + Material + "', " +
                                            "Unit_Specification='" + Unit_Specification + "', " +
                                            "QTY='" + QTY + "', " +
                                            "Price='" + Price + "', " +
                                            "Paben_Non_Pabean ='" + Pabean + "', " +
                                            "Billing_Date_DP ='" + BillingDateDP + "', " +
                                            "DP_Amount='" + DPAmount + "', " +
                                            "Billing_Date_Termin_1='" + BillingDateTermin1 + "', " +
                                            "Termin_1_Amount='" + Termin1Amount + "', " +
                                            "Billing_Date_Termin_2='" + BillingDateTermin2 + "', " +
                                            "Termin_2_Amount='" + Termin2Amount + "', " +
                                            "Customer_Group='" + CustomerGroup + "', " +
                                            "Product_Type='" + ProductType + "', " +
                                            "FLAG='1', " +
                                            "ParentNumber =" + (itmProd.new_ParentNumber != null ? "'" + decimal.Round(itmProd.new_ParentNumber.Value, 0, MidpointRounding.AwayFromZero).ToString() + "'" : "NULL") +
                                            ",StatusCPOProduct='1',modifOn='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                            "WHERE ID_UNIQ_CPO_CRM='" + etyCPO.Id.ToString() +
                                            "' AND ID_UNIQ_CPO_DETAIL_CRM='" + idProduct.ToString() + "'" +
                                            " AND ItemNumber='" + decimal.Round(itmProd.new_ItemNumber.Value, 0, MidpointRounding.AwayFromZero).ToString() + "'";
                                    }
                                    cm.ExecuteNonQuery();
                                };
                                tr.Commit();
                            }
                            catch (SqlException ex)
                            {
                                throw new Exception(ex.ErrorCode + " => " + ex.Message, ex);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message, ex);
                            }
                        }

                        #endregion

                        string textCreateFile = "";
                        #region Replace ASCII 13 dan 10

                        textCreateFile =
                            orderNumber + "|" +
                            orderType + "|" +
                            documentDateYYYYMMDD + "|" +
                            distCannel + "|" +
                            salesOrg + "|" +
                            divisi + "|" +
                            Sales_Office + "|" +
                            sold_to_party + "|" +
                            BillingDateDP + "|" +
                            DPAmount + "|" +
                            BillingDateTermin1 + "|" +
                            Termin1Amount + "|" +
                            BillingDateTermin2 + "|" +
                            Termin2Amount + "|" +
                            PO_Number + "|" +
                            PO_Date + "|" +
                            Doc_Currency + "|" +
                            Payment_Term + "|" +
                            Delivery_Plant + "|" +
                            Salesman + "|" +
                            Pabean + "|" +
                            CustomerGroup + "|" +
                            Price_List + "|" +
                            Price_Group + "|" +
                            Sales_group + "|" +
                            Business_Sector + "|" +
                            PKP + "|" +
                            Reservation + "|" +
                            Handling_Impotir + "|" +
                            Document_Required + "|" +
                            Delivery_Terms + "|" +
                            Adress_Map + "|" +
                            PIC_Title + "|" +
                            Unit_to_be_operated + "|" +
                            Delivery_Time + "|" +
                            Other_Remarks + "|" +
                            Sales_Manager_Remarks + "|" +
                            PO_Leasing_Number + "|" +
                            DP_Remarks + "|" +
                            Balance_Remarks + "|" +
                            decimal.Round(itmProd.new_ItemNumber.Value, 0, MidpointRounding.AwayFromZero).ToString() + "|" +
                            Material + "|" +
                            QTY + "|" +
                            decimal.Round(Price, 2, MidpointRounding.AwayFromZero).ToString() + "|" +
                            Unit_Specification + "|" +
                            LeasingNumber + "|" +
                            ProductType;

                        //textCreateFile = textCreateFile.Replace(Convert.ToChar(13), Convert.ToChar(32));
                        //textCreateFile = textCreateFile.Replace(Convert.ToChar(10), Convert.ToChar(32));

                        #endregion

                        #region Create File

                        using(System.IO.StreamWriter oWriter = new System.IO.StreamWriter(PathFile, true))
                        {
                            oWriter.WriteLine
                            (
                                textCreateFile
                            ); 
                        }

                        #endregion
                    }
                    else
                    {
                        throw new Exception("Failed Proses CPO to SAP, Not Found CPO Product in CPO!");
                    }
                }

                #endregion

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //oWriter.Close();
                //oWriter.Dispose();
            }
            #endregion
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            #region getDataCPO

            if (IsPostBack == false)
            {
                string strConnectionCRM = ConfigurationManager.AppSettings["ConnectionCRM"];
                string fileName = ConfigurationManager.AppSettings["fileName"];
                string fileNameAttachment = ConfigurationManager.AppSettings["fileNameAttachment"];
                string pathOnly = ConfigurationManager.AppSettings["pathFile"];

                Microsoft.Xrm.Client.CrmConnection connection = Microsoft.Xrm.Client.CrmConnection.Parse(strConnectionCRM);
                XrmServiceContext crm = null;
                crm = new XrmServiceContext(connection);

                //otomatFillingNo(crm);

                crm.ClearChanges();
                string CPO_Id = Request.QueryString["id"];

                //testing code
                //CPO_Id = "{5296BCEE-8100-E211-92C9-005056924533}";
                //CPO_Id = "{94C251A6-472A-E211-92CA-005056924533}";
                //CPO_Id = "{0284B9F3-9E5B-E211-90D6-005056924533}";
                //CPO_Id = "{1A45349E-C25B-E211-90D6-005056924533}";
                //CPO_Id = "{D3F66D89-0986-E211-B27D-005056921023}";
                

                HttpContext.Current.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
                HttpContext.Current.Response.Cache.SetValidUntilExpires(false);
                HttpContext.Current.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                HttpContext.Current.Response.Cache.SetNoStore();

                
                var serviceContainer = crm as Microsoft.Xrm.Client.IOrganizationServiceContainer;
                var cachedOrgService = serviceContainer.Service as Microsoft.Xrm.Client.Services.CachedOrganizationService;
                var orgServiceCache = cachedOrgService.Cache as Microsoft.Xrm.Client.Services.IOrganizationServiceCache;
                
                Guid id_req = new Guid(CPO_Id);
                orgServiceCache.Remove("SalesOrder", id_req);
                orgServiceCache.Remove("salesorder", id_req);
                orgServiceCache.Mode = Microsoft.Xrm.Client.Services.OrganizationServiceCacheMode.Disabled;

                try
                {
                    var etyCPO = crm.SalesOrderSet.Where(s => s.Id.Equals(id_req)).FirstOrDefault();
                    if (etyCPO != null)
                    {
                        //ValidationProductAttachment(crm, etyCPO);
                        ValidationProductAmount(crm, etyCPO);

                        crm.ClearChanges();

                        etyCPO = crm.SalesOrderSet.Where(s => s.Id.Equals(id_req)).FirstOrDefault();

                        fileName = fileName.Replace("[custome]", etyCPO.OrderNumber);
                        fileNameAttachment = fileNameAttachment.Replace("[custome]", etyCPO.OrderNumber);
                        ProsesCPOtoSAP(crm, etyCPO, pathOnly + fileName, pathOnly + fileNameAttachment);

                        crm.ClearChanges();
                        var etyProdCPOs = crm.SalesOrderDetailSet.Where(sd => sd.SalesOrderId.Id.Equals(CPO_Id));
                        foreach (SalesOrderDetail itmProd in etyProdCPOs)
                        {
                            crm.ClearChanges();
                            var updateItm = new SalesOrderDetail
                            {
                                Id = itmProd.Id,

                                new_StatusCPO = 1,
                            };
                            crm.Attach(updateItm);
                            crm.UpdateObject(updateItm);
                            crm.SaveChanges();
                            crm.ClearChanges();
                        }
                        crm.ClearChanges();
                        var update = new SalesOrder
                        {
                            Id = id_req,

                            new_StatusCPO = 1,
                            new_ForChartStatusCPO = 1
                        };
                        crm.Attach(update);
                        crm.UpdateObject(update);
                        crm.SaveChanges();
                        crm.ClearChanges();
                    }
                    etyCPO = null;
                    Response.Write("<script type='text/javascript'>alert('CPO has been send to SAP'); self.close();</script>");
                }
                catch(Exception ex)
                {
                    Response.Write("<div style='color:red'>Business Object Error</div><div>" + ex.Message + "</div>");
                }
                crm.ClearChanges();
            }

            #endregion
        }
    }
}