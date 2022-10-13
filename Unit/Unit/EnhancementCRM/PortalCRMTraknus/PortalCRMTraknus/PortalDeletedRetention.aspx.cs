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
    public partial class PortalDeletedRetention : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region getDataCPO

            if (IsPostBack == false)
            {
                #region Connection

                string strConnectionCRM = ConfigurationManager.AppSettings["ConnectionCRM"];
                Microsoft.Xrm.Client.CrmConnection connection = Microsoft.Xrm.Client.CrmConnection.Parse(strConnectionCRM);
                XrmServiceContext crm = null;
                crm = new XrmServiceContext(connection);

                crm.ClearChanges();
                string CPO_Id = "";
                CPO_Id = Request.QueryString["id"];

                //testing code
                //CPO_Id = "{78D7363D-FB86-E211-BDC4-005056921023}";

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

                #endregion

                try
                {
                    var etyCPO = crm.SalesOrderSet.Where(s => s.Id.Equals(id_req)).FirstOrDefault();
                    if (etyCPO != null)
                    {
                        crm.ClearChanges();

                        var prod = crm.ProductSet.Where(p => p.Name.Equals("Retention") && p.TransactionCurrencyId.Id.Equals(etyCPO.TransactionCurrencyId.Id)).FirstOrDefault();
                        if (prod != null)
                        {
                            var cpoProdRet = crm.SalesOrderDetailSet.Where(s => s.SalesOrderId.Id.Equals(etyCPO.Id) && s.ProductId.Id.Equals(prod.Id)).FirstOrDefault();
                            if (cpoProdRet != null)
                            {
                                crm.ClearChanges();
                                var delete = new Xrm.SalesOrderDetail
                                {
                                    Id = cpoProdRet.Id,
                                };
                                crm.Attach(delete);
                                crm.DeleteObject(delete);
                                crm.SaveChanges();
                                crm.ClearChanges();
                            }
                        }
                        UpdateSalesOrder(crm, id_req);
                    }
                    etyCPO = null;
                    Response.Write("<script type='text/javascript'>alert('Retention has been Deleted'); self.close();</script>");
                }
                catch (Exception ex)
                {
                    Response.Write("<div style='color:red'>Business Object Error</div><div>" + ex.Message + "</div>");
                }
                crm.ClearChanges();
            }

            #endregion
        }
        private void UpdateSalesOrder(XrmServiceContext crm, Guid id)
        {
            using (crm)
            {
                decimal detailAmount = 0;
                decimal taxPercentage = 0;
                decimal tax = 0;
                decimal amount = 0;

                var SO = crm.SalesOrderSet.Where(s => s.Id.Equals(id)).FirstOrDefault();
                if (SO.new_PabeanLK != null)
                {
                    var Pabean = crm.new_pabeanSet.Where(p => p.Id.Equals(SO.new_PabeanLK.Id)).FirstOrDefault();
                    if (Pabean != null && Pabean.new_AddTax == true)
                    {
                        var acc = crm.AccountSet.Where(a => a.Id.Equals(SO.CustomerId.Id)).FirstOrDefault();
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
                var CPOProds = crm.SalesOrderDetailSet.Where(cp => cp.SalesOrderId.Id.Equals(id) && cp.new_ItemNumber != null);

                foreach (SalesOrderDetail CPOProd in CPOProds)
                {
                    detailAmount = detailAmount + ((CPOProd.new_PricePerUnit != null ? CPOProd.new_PricePerUnit.Value : 0) -
                        (CPOProd.new_ManualDiscount != null ? CPOProd.new_ManualDiscount.Value : 0));

                    tax = tax + ((CPOProd.new_PricePerUnit != null ? CPOProd.new_PricePerUnit.Value : 0) -
                        (CPOProd.new_ManualDiscount != null ? CPOProd.new_ManualDiscount.Value : 0)) * 
                        (decimal)(taxPercentage / 100);

                    amount = amount + ((CPOProd.new_PricePerUnit != null ? CPOProd.new_PricePerUnit.Value : 0) -
                        (CPOProd.new_ManualDiscount != null ? CPOProd.new_ManualDiscount.Value : 0)) * 
                        ((decimal)(taxPercentage + 100) / 100);


                    crm.ClearChanges();
                    var updateCPODetail = new SalesOrderDetail
                    {
                        Id = CPOProd.Id,
                        new_RetentionAmount = 0,
                        new_Tax = ((CPOProd.new_PricePerUnit != null ? CPOProd.new_PricePerUnit.Value : 0) -
                                (CPOProd.new_ManualDiscount != null ? CPOProd.new_ManualDiscount.Value : 0)) *
                                (decimal)(taxPercentage / 100),
                        new_ExtendedAmount = ((CPOProd.new_PricePerUnit != null ? CPOProd.new_PricePerUnit.Value : 0) -
                                (CPOProd.new_ManualDiscount != null ? CPOProd.new_ManualDiscount.Value : 0)) *
                                ((decimal)(taxPercentage + 100) / 100),
                    };
                    crm.Attach(updateCPODetail);
                    crm.UpdateObject(updateCPODetail);
                    crm.SaveChanges();
                    crm.ClearChanges();
                }

                crm.ClearChanges();
                var updateCPO = new SalesOrder
                {
                    Id = id,
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