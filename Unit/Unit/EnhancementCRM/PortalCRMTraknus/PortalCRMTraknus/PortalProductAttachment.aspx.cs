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
    public partial class PortalProductAttachment : System.Web.UI.Page
    {
        public int countAttach = 0;
        private List<string> listProductNumber = new List<string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                #region Command
                ////clear grid view
                //this.GridView1.DataSource = null;
                //this.GridView1.DataBind();

                //#region Connection String

                //string strConnectionCRM = ConfigurationManager.AppSettings["ConnectionCRM"];

                //Microsoft.Xrm.Client.CrmConnection connection = Microsoft.Xrm.Client.CrmConnection.Parse(strConnectionCRM);
                //XrmServiceContext crm = new XrmServiceContext(connection);

                //#endregion

                //string prospect_product_guid = Request.QueryString["ProductGuid"];

                //Guid prospect_id = new Guid(Request.QueryString["id"]);

                //var etyProspect = crm.OpportunitySet.Where(op => op.Id == prospect_id).FirstOrDefault();
                //if (etyProspect == null)
                //{
                //    throw new Exception("Not Found Prospect");
                //}

                ////prospect product
                //var etyProspectProducts = crm.OpportunityProductSet.Where(
                //    op => op.OpportunityId.Id.Equals(prospect_id));
                //string[] pp_guid = prospect_product_guid.Split(',');
                //Guid prospect_product_id;
                //Guid product_id;
                //Guid attach_id;

                //int counter = 0;

                //#region Looping Ambil Product Attachment

                //ProductAttachmentList l = ProductAttachmentList.GetProducts();
                //for (int i = 0; i < pp_guid.Count(); i++)
                //{
                //    prospect_product_id = new Guid(pp_guid[i]);
                //    var etyProspectProduct = crm.OpportunityProductSet.Where(op => op.Id == prospect_product_id).FirstOrDefault();

                //    if (etyProspectProduct != null)
                //    {
                //        product_id = etyProspectProduct.ProductId.Id;

                //        // cek dengan product yang pernah ada di prospect
                //        // apabila product sudah ada di prospect maka tidak perlu dimunculkan lagi.
                //        bool cek = false;
                        
                //        var etyProduct = crm.ProductSet.Where(p => p.Id == product_id).FirstOrDefault();
                //        if (etyProduct != null)
                //        {
                //            var etyProductAttachment = crm.new_productattachmentSet.Where(pa => pa.new_UnitProduct.Id == product_id).FirstOrDefault();
                //            if (etyProductAttachment != null)
                //            {
                //                attach_id = etyProductAttachment.Id;
                //                var etyPAProduct = crm.new_productattachment_productSet.Where(
                //                    pap => pap.new_productattachmentid == attach_id);

                //                if (etyPAProduct != null)
                //                {
                //                    int x = 1;
                //                    foreach (var j in etyPAProduct)
                //                    {
                //                        x += 1;
                //                        counter += 1;

                //                        var etyProduct2 = crm.ProductSet.Where(p2 => p2.Id == j.productid).FirstOrDefault();
                //                        if (DropDownList1.SelectedValue == "1")
                //                        {
                //                            if (etyProduct2.new_IsStandard == true)
                //                            {
                //                                l.AddList(new ProductAttachment(x, etyProduct2.ProductNumber, etyProduct2.Name, etyProduct.Name, etyProduct2.Description));
                //                            }
                //                        }
                //                        else if (DropDownList1.SelectedValue == "0")
                //                        {
                //                            if (etyProduct2.new_IsStandard == false)
                //                            {
                //                                l.AddList(new ProductAttachment(x, etyProduct2.ProductNumber, etyProduct2.Name, etyProduct.Name, etyProduct2.Description));
                //                            }
                //                        }
                //                    }
                //                    countAttach = x;
                //                }
                //            }
                //        }
                //    }
                //}

                //#endregion

                //l.Sort();
                //this.GridView1.DataSource = l;
                //this.GridView1.DataBind();
                #endregion

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

                string id = Request.QueryString["id"];
                //id = "8AB3085D-1170-E211-BFC5-005056924533";
                Guid parent_id = new Guid(id);

                Guid parent_product_id = new Guid(Request.QueryString["ProductGuid"]);
                var etyProspectProduct = crm.OpportunityProductSet.Where(op => op.Id == parent_product_id).FirstOrDefault();
                if (etyProspectProduct == null)
                {
                    throw new Exception("Not found Parent Product!");
                }
                //testing code
                //prospect_product_guid = "{7B0518D7-D702-E211-AA8F-C86000B59ABA},{D46C9D90-BD03-E211-92C9-005056924533},{73F54DA7-BD03-E211-92C9-005056924533},{C9ED93EE-9B12-E211-92CA-005056924533}";

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
                        var etyProspect = crm.OpportunitySet.Where(op => op.Id == parent_id).FirstOrDefault();
                        //var etyPriceListItem = crm.ProductPriceLevelSet.Where(ppl => ppl.PriceLevelId.Id.Equals(etyProduct.PriceLevelId.Id)).FirstOrDefault();

                        crm.ClearChanges();
                        var etyPAttach = new Xrm.OpportunityProduct
                        {
                            IsProductOverridden = false,
                            ProductId = etyProduct.ToEntityReference(),
                            UoMId = etyUnit.ToEntityReference(),
                            new_UnitGroup = etyProspectProduct.new_UnitGroup,
                            new_ParentNumber = etyProspectProduct.new_ItemNumber,
                            //PricePerUnit = etyPriceListItem.Amount != null ? etyPriceListItem.Amount : 0,

                            IsPriceOverridden = false,
                            new_TypeDiscount = false,
                            new_PercentageDiscount = 0,
                            new_Quantity = etyProspectProduct.new_Quantity,
                            Quantity = etyProspectProduct.new_Quantity,
                            ManualDiscountAmount = 0,
                            Description = desc,
                            
                            OpportunityId = etyProspect.ToEntityReference(),
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

            string prospect_product_guid = Request.QueryString["ProductGuid"];

            Guid prospect_id = new Guid(Request.QueryString["id"]);
            var etyProspect = crm.OpportunitySet.Where(op => op.Id == prospect_id).FirstOrDefault();
            if (etyProspect == null)
            {
                throw new Exception("Not Found Prospect");
            }

            //prospect product
            var etyProspectProducts = crm.OpportunityProductSet.Where(
                op => op.OpportunityId.Id.Equals(prospect_id));
            string[] pp_guid = prospect_product_guid.Split(',');

            Guid prospect_product_id;
            Guid product_id;
            Guid attach_id;

            int counter = 0;

            #region Looping Ambil Product Attachment

            ProductAttachmentList l = ProductAttachmentList.GetProducts();
            for (int i = 0; i < pp_guid.Count(); i++)
            {
                prospect_product_id = new Guid(pp_guid[i]);
                var etyProspectProduct = crm.OpportunityProductSet.Where(op => op.Id == prospect_product_id).FirstOrDefault();

                if (etyProspectProduct != null)
                {
                    product_id = etyProspectProduct.ProductId.Id;

                    // cek dengan product yang pernah ada di prospect
                    // apabila product sudah ada di prospect maka tidak perlu dimunculkan lagi.
                    bool cek = false;

                    var etyProduct = crm.ProductSet.Where(p => p.Id == product_id).FirstOrDefault();
                    if (etyProduct != null)
                    {
                        var etyProductAttachment = crm.new_productattachmentSet.Where(pa => pa.new_UnitProduct.Id == product_id).FirstOrDefault();
                        if (etyProductAttachment != null)
                        {
                            attach_id = etyProductAttachment.Id;
                            //var etyPAProduct = crm.new_productattachment_productSet.Where(
                            //    pap => pap.new_productattachmentid == attach_id);

                            var etyPAProduct = from pa in crm.new_productattachment_productSet
                                               join prod in crm.ProductSet on pa.productid equals prod.ProductId
                                               where pa.new_productattachmentid == attach_id && prod.new_UnitGroup == etyProduct.new_UnitGroup && prod.new_MaterialGroup == etyProduct.new_MaterialGroup
                                               select new { pa.productid };

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
                                                prc = decimal.Round((decimal) etyPricelistItem.Amount,2);
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