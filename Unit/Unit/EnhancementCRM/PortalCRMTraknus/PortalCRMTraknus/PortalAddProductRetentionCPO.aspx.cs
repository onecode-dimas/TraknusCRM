using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Client;
using Xrm;
using System.Configuration;

namespace PortalCRMTraknus
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        public int countAttach = 0;
        private List<string> listProductNumber = new List<string>();
        CPOProductList l = CPOProductList.GetProducts();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                container1.Visible = true;
                container2.Visible = false;
                if (IsPostBack == false)
                {
                    if (DataBindDataGrid() == 0)
                    {
                        submit1.Text = "Close";
                        countAttach = 0;
                    }
                    else
                    {
                        
                    }
                }
            }
            catch (Exception ex)
            {
                container2.Visible = true;
                container1.Visible = false;
                Response.Write("<div style='color:red'>Business Object Error</div><div>" + ex.Message + "</div>");
            }
        }

        private void ValidationRetention(XrmServiceContext crm, SalesOrder cpo)
        {
            try
            {
                using (crm)
                {
                    var prod = crm.ProductSet.Where(p => p.Name.Equals("Retention") &&
                        p.TransactionCurrencyId.Id.Equals(cpo.TransactionCurrencyId.Id)).FirstOrDefault();
                    if (prod != null)
                    {
                        var prodList = crm.ProductPriceLevelSet.Where(pl => pl.ProductId.Id.Equals(prod.Id) && pl.TransactionCurrencyId.Id.Equals(cpo.TransactionCurrencyId.Id)).FirstOrDefault();
                        if (prodList == null)
                        {
                            throw new Exception("Not Found Product Retention in List Unit Group " + cpo.new_UnitGroup.Name);   
                        }
                    }
                    else
                    {
                        throw new Exception("Not Found Product Retention in Unit Group " + cpo.new_UnitGroup.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int DataBindDataGrid()
        {
            try
            {
                //clear grid view
                this.GridView1.DataSource = null;
                this.GridView1.DataBind();

                #region Connection String

                string strConnectionCRM = ConfigurationManager.AppSettings["ConnectionCRM"];

                Microsoft.Xrm.Client.CrmConnection connection = Microsoft.Xrm.Client.CrmConnection.Parse(strConnectionCRM);
                XrmServiceContext crm = new XrmServiceContext(connection);

                #endregion

                Guid cpo_id = new Guid(Request.QueryString["id"]);
                //Guid cpo_id = new Guid("78D7363D-FB86-E211-BDC4-005056921023");

                SalesOrder etyCPO = crm.SalesOrderSet.Where(c => c.Id == cpo_id).FirstOrDefault();
                if (etyCPO == null)
                {
                    throw new Exception("Not Found CPOrospect");
                }
                ValidationRetention(crm, etyCPO);

                int counter = 0;

                #region Looping Ambil CPO Product

                var etyCPOProducts = crm.SalesOrderDetailSet.Where(
                    op => op.SalesOrderId.Id.Equals(cpo_id));

                foreach (var cpoProd in etyCPOProducts)
                {
                    if (cpoProd.ProductId.Name.Equals("Retention") == false && (cpoProd.new_ItemNumber % 1000 ==0))
                    {
                        var prod = crm.ProductSet.Where(p => p.Id.Equals(cpoProd.ProductId.Id)).FirstOrDefault();

                        counter += 1;
                        l.Add(new CPOProduct(
                            counter,
                            (cpoProd.new_ItemNumber != null ? cpoProd.new_ItemNumber.Value : 0),
                            prod.ProductNumber,
                            prod.Name,
                            (cpoProd.new_PricePerUnit != null ? cpoProd.new_PricePerUnit.Value : 0),
                            (cpoProd.new_ManualDiscount != null ? cpoProd.new_ManualDiscount.Value : 0),
                            (cpoProd.new_RetentionAmount != null ? cpoProd.new_RetentionAmount.Value : 0),
                            (cpoProd.new_ExtendedAmount != null ? cpoProd.new_ExtendedAmount.Value : 0),
                            cpoProd.Description));
                    }
                }

                #endregion

                if (counter > 0)
                {
                    //submit1.Visible = false;
                }
                l.Sort();
                this.GridView1.DataSource = l;
                this.GridView1.DataBind();

                return counter;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        private void AddProductRetentionAndCalculate(XrmServiceContext crm, SalesOrder cpo)
        {
            try
            {
                using (crm)
                {
                    decimal retentionAmount = 0;
                    decimal taxPercentage = 0;

                    if (cpo.new_PabeanLK != null)
                    {
                        var Pabean = crm.new_pabeanSet.Where(p => p.Id.Equals(cpo.new_PabeanLK.Id)).FirstOrDefault();
                        if (Pabean != null && Pabean.new_AddTax == true)
                        {
                            var acc = crm.AccountSet.Where(a => a.Id.Equals(cpo.CustomerId.Id)).FirstOrDefault();
                            if (acc != null && acc.new_BusinessGroup != null)
                            {
                                var busGroup = crm.new_businessgroupSet.Where(b => b.Id.Equals(acc.new_BusinessGroup.Id)).FirstOrDefault();
                                if (busGroup != null && busGroup.new_PercentageTax.HasValue)
                                {
                                    taxPercentage = busGroup.new_PercentageTax.Value;
                                }
                            }
                        }
                    }

                    #region Update Non Retention

                    foreach (GridViewRow row in GridView1.Rows)
                    {
                        CheckBox c = row.FindControl("CheckBox1") as CheckBox;
                        SalesOrderDetail cpoProd = crm.SalesOrderDetailSet.Where(s => s.SalesOrderId.Id.Equals(cpo.Id) && 
                            s.new_ItemNumber.Value.Equals(decimal.Parse(c.Text))).FirstOrDefault();
                        if (cpoProd.ProductId.Name != "Retention")
                        {
                            TextBox tRen = row.FindControl("TextBoxRetention") as TextBox;
                            TextBox_Convert(tRen, null);

                            decimal ret = decimal.Parse(tRen.Text);
                            decimal tax = 0;

                            tax = ((cpoProd.new_PricePerUnit != null ? cpoProd.new_PricePerUnit.Value : 0) -
                                ret -
                                (cpoProd.new_ManualDiscount != null ? cpoProd.new_ManualDiscount.Value : 0)) * 
                                ((decimal)taxPercentage / 100);

                            retentionAmount = retentionAmount + ret;

                            if (ret > 0)
                            {
                                crm.ClearChanges();
                                var update = new Xrm.SalesOrderDetail
                                {
                                    Id = cpoProd.Id,

                                    new_RetentionAmount = ret,

                                    new_Tax = tax,

                                    new_ExtendedAmount = (cpoProd.new_PricePerUnit != null ? cpoProd.new_PricePerUnit.Value : 0) -
                                    ret -
                                    (cpoProd.new_ManualDiscount != null ? cpoProd.new_ManualDiscount.Value : 0) +
                                    tax,

                                    new_IsEffectedRetention = true,

                                    new_IsUserCreated = true,
                                    new_IsMaintenanceInSAP = true,
                                    new_IsPlugin = true,
                                };
                                crm.Attach(update);
                                crm.UpdateObject(update);
                                crm.SaveChanges();
                                crm.ClearChanges();
                            }
                            else
                            {
                                crm.ClearChanges();
                                var update = new Xrm.SalesOrderDetail
                                {
                                    Id = cpoProd.Id,

                                    new_RetentionAmount = ret,

                                    new_Tax = tax,

                                    new_ExtendedAmount = (cpoProd.new_PricePerUnit != null ? cpoProd.new_PricePerUnit.Value : 0) -
                                    ret -
                                    (cpoProd.new_ManualDiscount != null ? cpoProd.new_ManualDiscount.Value : 0) +
                                    tax,

                                    new_IsEffectedRetention = false,

                                    new_IsUserCreated = true,
                                    new_IsMaintenanceInSAP = true,
                                    new_IsPlugin = true,
                                };
                                crm.Attach(update);
                                crm.UpdateObject(update);
                                crm.SaveChanges();
                                crm.ClearChanges();
                            }
                        }
                    }

                    #endregion

                    var prod = crm.ProductSet.Where(p => p.Name.Equals("Retention") && p.TransactionCurrencyId.Id.Equals(cpo.TransactionCurrencyId.Id)).FirstOrDefault();
                    var cpoProdRet = crm.SalesOrderDetailSet.Where(s => s.SalesOrderId.Id.Equals(cpo.Id) && s.ProductId.Id.Equals(prod.Id)).FirstOrDefault();

                    decimal taxRet = 0;
                    taxRet = retentionAmount * ((decimal)taxPercentage / 100);

                    #region Update ReTention

                    if (cpoProdRet != null)
                    {
                        crm.ClearChanges();
                        var update = new Xrm.SalesOrderDetail
                        {
                            Id = cpoProdRet.Id,
                            new_PricePerUnit = retentionAmount,
                            new_RetentionAmount = 0,

                            new_ManualDiscount = 0,

                            new_Tax = taxRet,
                            new_ExtendedAmount = retentionAmount + taxRet,

                            new_IsUserCreated = true,
                            new_IsMaintenanceInSAP = true,
                            new_IsPlugin = true,
                        };
                        crm.Attach(update);
                        crm.UpdateObject(update);
                        crm.SaveChanges();
                        crm.ClearChanges();
                    }

                    #endregion
                    #region Insert Retention

                    else
                    {
                        decimal counterParent = 0;
                        var CDHistorys = crm.SalesOrderDetailSet.Where(
                                    qp => qp.SalesOrderId.Id.Equals(cpo.Id));
                        foreach (SalesOrderDetail cdh in CDHistorys)
                        {
                            if (cdh.new_ItemNumber != null)
                            {
                                if (cdh.new_ItemNumber.Value > counterParent)
                                {
                                    counterParent = cdh.new_ItemNumber.Value;
                                }
                            }
                        }
                        counterParent = counterParent - (counterParent % 1000);

                        var insertDetail = new SalesOrderDetail
                        {
                            IsProductOverridden = true,
                            ProductId = prod.ToEntityReference(),
                            UoMId = prod.DefaultUoMId,

                            IsPriceOverridden = true,//qp.IsPriceOverridden,
                            Quantity = 1,
                            new_TypeDiscount = true,
                            new_PercentageDiscount = 0,

                            new_ItemNumber = counterParent + 1000,

                            new_PricePerUnit = retentionAmount,
                            new_RetentionAmount = 0,
                            new_ManualDiscount = 0,
                            new_ManualDiscountOrigin = 0,

                            new_Tax = taxRet,
                            new_ExtendedAmount = retentionAmount + taxRet,

                            Description = prod.Description,
                            SalesOrderId = cpo.ToEntityReference(),

                            new_IsUserCreated = true,
                            new_IsMaintenanceInSAP = true,
                            new_IsPlugin = true,
                        };
                        crm.AddObject(insertDetail);
                        crm.SaveChanges();
                        crm.ClearChanges();
                    }

                    #endregion
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        private void UpdateSalesOrder(IOrganizationService service, Guid id)
        {
            using (var crm = new XrmServiceContext(service))
            {
                var cpoDetail = crm.SalesOrderDetailSet.Where(sd => sd.Id.Equals(id)).FirstOrDefault();
                if (cpoDetail != null)
                {
                    if (cpoDetail.SalesOrderId != null)
                    {
                        var SO = crm.SalesOrderSet.Where(s => s.Id.Equals(cpoDetail.SalesOrderId.Id)).FirstOrDefault();

                        var CPOProds = crm.SalesOrderDetailSet.Where(cp => cp.SalesOrderId.Id.Equals(cpoDetail.SalesOrderId.Id) && cp.new_ItemNumber != null);

                        decimal detailAmount = 0;
                        decimal tax = 0;
                        decimal amount = 0;

                        foreach (SalesOrderDetail CPOProd in CPOProds)
                        {
                            detailAmount = detailAmount + 
                                ((CPOProd.new_PricePerUnit != null ? CPOProd.new_PricePerUnit.Value : 0) - 
                                (CPOProd.new_RetentionAmount != null ? CPOProd.new_RetentionAmount.Value : 0) - 
                                (CPOProd.new_ManualDiscount != null ? CPOProd.new_ManualDiscount.Value : 0));
                            tax = tax + (CPOProd.new_Tax != null ? CPOProd.new_Tax.Value : 0);

                            amount = amount +
                                ((CPOProd.new_PricePerUnit != null ? CPOProd.new_PricePerUnit.Value : 0) -
                                (CPOProd.new_RetentionAmount != null ? CPOProd.new_RetentionAmount.Value : 0) -
                                (CPOProd.new_ManualDiscount != null ? CPOProd.new_ManualDiscount.Value : 0) +
                                (CPOProd.new_Tax != null ? CPOProd.new_Tax.Value : 0));
                        }

                        crm.ClearChanges();
                        var updateCPO = new SalesOrder
                        {
                            Id = cpoDetail.SalesOrderId.Id,
                            new_DetailAmount = detailAmount,
                            new_TotalTax = tax,
                            new_TotalAmount = amount,
                        };
                        crm.Attach(updateCPO);
                        crm.UpdateObject(updateCPO);
                        crm.SaveChanges();
                        crm.ClearChanges();
                    }
                }
            }
        }

        protected void CheckBox1_Click(object sender, EventArgs e)
        {
            try
            {
                int count = 0;
                List<string> l = new List<string>();
                foreach (GridViewRow row in GridView1.Rows)
                {
                    count++;
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        CheckBox c = row.FindControl("CheckBox1") as CheckBox;
                        if (c != null && c.Checked == true)
                        {
                            l.Add(c.Text);
                        }
                    }
                }
                if (count > 0)
                {
                    if (l.Count > 0)
                    {
                        //submit1.Visible = true;
                    }
                    else
                    {
                        //submit1.Visible = false;
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        protected void submit_Click(object sender, EventArgs e)
        {
            try
            {
                int count = 0;
                List<string> l = new List<string>();
                foreach (GridViewRow row in GridView1.Rows)
                {
                    count++;
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        CheckBox c = row.FindControl("CheckBox1") as CheckBox;
                        if (c != null && c.Checked == true)
                        {
                            l.Add(c.Text);
                        }
                    }
                }
                if (count > 0)
                {
                    countAttach = l.Count;

                    #region Connection String

                    string strConnectionCRM = ConfigurationManager.AppSettings["ConnectionCRM"];

                    Microsoft.Xrm.Client.CrmConnection connection = Microsoft.Xrm.Client.CrmConnection.Parse(strConnectionCRM);
                    XrmServiceContext crm = new XrmServiceContext(connection);

                    #endregion

                    Guid cpo_id = new Guid(Request.QueryString["id"]);
                    //Guid cpo_id = new Guid("78D7363D-FB86-E211-BDC4-005056921023");

                    SalesOrder etyCPO = crm.SalesOrderSet.Where(c => c.Id == cpo_id).FirstOrDefault();
                    if (etyCPO == null)
                    {
                        throw new Exception("Not Found CPOrospect");
                    }
                    ValidationRetention(crm, etyCPO);
                    AddProductRetentionAndCalculate(crm, etyCPO);

                    Response.Write("<script type='text/javascript'>alert('Product Retention has been Added'); this.close();</script>");
                }
                else
                {
                    Response.Write("<script type='text/javascript'>this.close();</script>");
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        protected void RecalculateLinkButton_Click(object sender, EventArgs e)
        {
            try
            {
                TextBox_Convert(this.PercentageRetentionTextBox, e);
                foreach (GridViewRow row in GridView1.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        System.Web.UI.WebControls.Label pricePerUnit = row.FindControl("PricePerUnitLabel") as System.Web.UI.WebControls.Label;
                        System.Web.UI.WebControls.Label discount = row.FindControl("DiscountLabel") as System.Web.UI.WebControls.Label;
                        TextBox tRen = row.FindControl("TextBoxRetention") as TextBox;
                        TextBox_Convert(tRen, e);
                        CheckBox c = row.FindControl("CheckBox1") as CheckBox;
                        if (c != null)
                        {
                            if (c.Checked == true)
                            {
                                tRen.Text = ((decimal.Parse(pricePerUnit.Text) - decimal.Parse(discount.Text)) * decimal.Parse(this.PercentageRetentionTextBox.Text) / 100).ToString();
                            }
                            else
                            {
                                tRen.Text = "0"; 
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void submitError_Click(object sender, EventArgs e)
        {
            container2.Visible = false;
            container1.Visible = true;
        }
        protected void TextBox_Convert(object sender, EventArgs e)
        {
            TextBox tb = (TextBox) sender;
            decimal d = 0;
            decimal.TryParse(tb.Text, out d);
            if (d < 0)
            {
                d = 0;
            }
            tb.Text = decimal.Round(d,2).ToString();
        }
    }
}