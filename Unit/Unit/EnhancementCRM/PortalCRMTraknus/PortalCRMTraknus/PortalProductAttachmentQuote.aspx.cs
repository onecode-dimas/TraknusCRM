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
    public partial class PortalProductAttachmentQuote : System.Web.UI.Page
    {
        public int countAttach = 0;
        private List<string> listProductNumber = new List<string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                if (DataBindDataGrid() == 0)
                {
                    submit1.Text = "Close";
                    countAttach = 0;
                }
            }
        }

        protected void submit_Click(object sender, EventArgs e)
        {
            List<string> l = new List<string>();
            foreach (GridViewRow row in GridView1.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox c = row.FindControl("CheckBox1") as CheckBox;
                    if (c != null && c.Checked == true)
                    {
                        l.Add(c.Text);
                    }
                }
            }
            countAttach = l.Count;
            if (l.Count > 0)
            {
                string strConnectionCRM = ConfigurationManager.AppSettings["ConnectionCRM"];
                Microsoft.Xrm.Client.CrmConnection connection = Microsoft.Xrm.Client.CrmConnection.Parse(strConnectionCRM);
                XrmServiceContext crm = new XrmServiceContext(connection);

                Guid parent_product_id = new Guid(Request.QueryString["ProductGuid"]);
                var etyQuoteProduct = crm.QuoteDetailSet.Where(op => op.Id == parent_product_id).FirstOrDefault();
                if (etyQuoteProduct == null)
                {
                    throw new Exception("Not found Parent Product!");
                }
                
                Guid product_id;
                Guid unit;
                string desc;

                foreach (string s in l)
                {
                    var etyProduct = crm.ProductSet.Where(p => p.ProductNumber == s).FirstOrDefault();
                    if (etyProduct != null)
                    {
                        product_id = etyProduct.Id;
                        unit = etyProduct.DefaultUoMId.Id;
                        desc = etyProduct.Description;

                        var etyUnit = crm.UoMSet.Where(u => u.Id == unit).FirstOrDefault();
                        var etyQuote = crm.QuoteSet.Where(op => op.Id == etyQuoteProduct.QuoteId.Id).FirstOrDefault();
                        var etyPricelist = crm.UoMScheduleSet.Where(pl => pl.Id == etyProduct.DefaultUoMScheduleId.Id).FirstOrDefault();
                        //var etyPriceListItem = crm.ProductPriceLevelSet.Where(ppl => ppl.PriceLevelId.Id.Equals(etyProduct.PriceLevelId.Id)).FirstOrDefault();

                        crm.ClearChanges();
                        var etyPAttach = new Xrm.QuoteDetail
                        {
                            IsProductOverridden = false,
                            ProductId = etyProduct.ToEntityReference(),
                            UoMId = etyUnit.ToEntityReference(),
                            new_UnitGroup = etyPricelist.ToEntityReference(),
                            //new_Pricelist = etyPricelist.ToEntityReference(),
                            new_ParentNumber = etyQuoteProduct.new_ItemNumber,
                            //PricePerUnit = etyPriceListItem.Amount != null ? etyPriceListItem.Amount : 0,

                            IsPriceOverridden = false,
                            new_TypeDiscount = false,
                            new_PercentageDiscount = 0,
                            new_Quantity = etyQuoteProduct.new_Quantity,
                            Quantity = etyQuoteProduct.new_Quantity,
                            ManualDiscountAmount = 0,
                            Description = desc,

                            QuoteId = etyQuote.ToEntityReference(),
                            new_IsUserCreated = true
                        };
                        crm.AddObject(etyPAttach);
                        crm.SaveChanges();
                        crm.ClearChanges();
                    }
                }

                container1.Visible = false;
                Response.Write("<script type='text/javascript'>alert('Product Attachment has been Added'); this.close();</script>");
            }
            else
            {
                Response.Write("<script type='text/javascript'>this.close();</script>");
            }
        }

        private int DataBindDataGrid()
        {
            //clear grid view
            this.GridView1.DataSource = null;
            this.GridView1.DataBind();

            #region Connection String

            string strConnectionCRM = ConfigurationManager.AppSettings["ConnectionCRM"];

            Microsoft.Xrm.Client.CrmConnection connection = Microsoft.Xrm.Client.CrmConnection.Parse(strConnectionCRM);
            XrmServiceContext crm = new XrmServiceContext(connection);

            #endregion

            string quote_product_guid = Request.QueryString["ProductGuid"];
            var etyPQuoteProducts = crm.QuoteDetailSet.Where(op => op.Id.Equals(new Guid(quote_product_guid)));

            string[] qp_guid = quote_product_guid.Split(',');
            Guid quote_product_id;
            Guid product_id;
            Guid attach_id;

            int counter = 0;

            #region Looping Ambil Product Attachment

            ProductAttachmentList l = ProductAttachmentList.GetProducts();
            for (int i = 0; i < qp_guid.Count(); i++)
            {
                quote_product_id = new Guid(qp_guid[i]);
                var etyQuoteProduct = crm.QuoteDetailSet.Where(op => op.Id == quote_product_id).FirstOrDefault();

                if (etyQuoteProduct != null)
                {
                    product_id = etyQuoteProduct.ProductId.Id;

                    var etyProduct = crm.ProductSet.Where(p => p.Id == product_id).FirstOrDefault();
                    if (etyProduct != null)
                    {
                        var etyProductAttachment = crm.new_productattachmentSet.Where(pa => pa.new_UnitProduct.Id == product_id).FirstOrDefault();
                        if (etyProductAttachment != null)
                        {
                            attach_id = etyProductAttachment.Id;
                            var etyPAProduct = crm.new_productattachment_productSet.Where(
                                pap => pap.new_productattachmentid == attach_id);

                            if (etyPAProduct != null)
                            {
                                int x = 1;
                                foreach (var j in etyPAProduct)
                                {
                                    x += 1;
                                    counter += 1;

                                    var etyProduct2 = crm.ProductSet.Where(p2 => p2.Id == j.productid).FirstOrDefault();
                                    if (etyProduct2 != null)
                                    {
                                        decimal prc = 0;
                                        var etyPricelistItem = crm.ProductPriceLevelSet.Where(pl => pl.PriceLevelId.Id == etyProduct2.PriceLevelId.Id && pl.ProductId.Id.Equals(etyProduct2.Id)).FirstOrDefault();
                                        if (etyPricelistItem != null)
                                        {
                                            if (etyPricelistItem.Amount != null)
                                            {
                                                prc = decimal.Round((decimal)etyPricelistItem.Amount, 2);
                                            }
                                        }
                                        if (DropDownList1.SelectedValue == "1")
                                        {
                                            if (etyProduct2.new_IsStandard == true)
                                            {
                                                l.AddList(new ProductAttachment(x, etyProduct2.ProductNumber, etyProduct2.Name, etyProduct.Name, prc, etyProduct2.Description));
                                            }
                                        }
                                        else if (DropDownList1.SelectedValue == "0")
                                        {
                                            if (etyProduct2.new_IsStandard != true)
                                            {
                                                l.AddList(new ProductAttachment(x, etyProduct2.ProductNumber, etyProduct2.Name, etyProduct.Name, prc, etyProduct2.Description));
                                            }
                                        }
                                    }
                                }
                                countAttach = x;
                            }
                        }
                    }
                }
            }

            #endregion

            l.Sort();
            this.GridView1.DataSource = l;
            this.GridView1.DataBind();

            return counter;
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DataBindDataGrid() == 0)
            {
                submit1.Text = "Close";
                countAttach = 0;
            }
        }
    }
}