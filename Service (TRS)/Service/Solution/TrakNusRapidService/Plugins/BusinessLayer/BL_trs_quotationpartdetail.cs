using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;

using TrakNusRapidService.DataLayer;

namespace TrakNusRapidService.Plugins.BusinessLayer
{
    class BL_trs_quotationpartdetail
    {
        #region Constants
        private const string _classname = "BL_trs_quotationpartdetail";
        #endregion

        #region Depedencies
        private DL_trs_quotationpartdetail _DL_trs_quotationpartdetail = new DL_trs_quotationpartdetail();
        #endregion

        #region Privates
        #endregion

        #region Events
        #region Forms
        public void Form_OnCreate(IOrganizationService organizationService, IPluginExecutionContext pluginExecutionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExecutionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_trs_quotationpartdetail.EntityName)
                {
                    decimal partsPrice = 0;
                    decimal quantity = 0;
                    bool discountBy = true;
                    decimal discountAmount = 0;
                    decimal discountPercent = 0;
                    decimal totalPrice = 0;

                    if (entity.Contains("trs_price") && entity.Attributes["trs_price"] != null)
                        partsPrice = ((Money)entity.Attributes["trs_price"]).Value;
                    if (entity.Contains("trs_quantity") && entity.Attributes["trs_quantity"] != null)
                        quantity = (int)entity.Attributes["trs_quantity"];

                    if (entity.Contains("trs_discountby") && entity.Attributes["trs_discountby"] != null)
                    {
                        discountBy = (bool)entity.Attributes["trs_discountby"];

                        if (discountBy == true && entity.Contains("trs_discountamount") && entity.Attributes["trs_discountamount"] != null)
                        {
                            discountAmount = ((Money)entity.Attributes["trs_discountamount"]).Value;
                            totalPrice = partsPrice * quantity - discountAmount;
                        }
                        if (discountBy == false && entity.Contains("trs_discountpercent") && entity.Attributes["trs_discountpercent"] != null)
                        {
                            discountPercent = (decimal)entity.Attributes["trs_discountpercent"];
                            totalPrice = partsPrice * quantity - (partsPrice * quantity * (discountPercent / 100));
                        }
                    }

                    entity.Attributes["trs_totalprice"] = new Money(totalPrice);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate : " + ex.Message.ToString());
            }
        }

        public void Form_OnUpdate_CalculateTotal(IOrganizationService organizationService, IPluginExecutionContext pluginExecutionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExecutionContext.InputParameters["Target"];
                Entity previous = (Entity)pluginExecutionContext.PreEntityImages["PreImage"];
                if (entity.LogicalName == _DL_trs_quotationpartdetail.EntityName)
                {
                    decimal partsPrice = 0;
                    decimal quantity = 0;
                    bool discountBy = true;
                    decimal discountAmount = 0;
                    decimal discountPercent = 0;
                    decimal totalPrice = 0;

                    if (entity.Contains("trs_price") && entity.Attributes["trs_price"] != null)
                        partsPrice = ((Money)entity.Attributes["trs_price"]).Value;
                    else if (previous.Contains("trs_price") && previous.Attributes["trs_price"] != null)
                        partsPrice = ((Money)previous.Attributes["trs_price"]).Value;

                    if (entity.Contains("trs_quantity") && entity.Attributes["trs_quantity"] != null)
                        quantity = (int)entity.Attributes["trs_quantity"];
                    else if (previous.Contains("trs_quantity") && previous.Attributes["trs_quantity"] != null)
                        quantity = (int)previous.Attributes["trs_quantity"];

                    if (entity.Contains("trs_discountby") && entity.Attributes["trs_discountby"] != null)
                        discountBy = (bool)entity.Attributes["trs_discountby"];
                    else if (previous.Contains("trs_discountby") && previous.Attributes["trs_discountby"] != null)
                        discountBy = (bool)previous.Attributes["trs_discountby"];


                    if (discountBy == true)
                    {
                        if (entity.Contains("trs_discountamount") && entity.Attributes["trs_discountamount"] != null)
                            discountAmount = ((Money)entity.Attributes["trs_discountamount"]).Value;
                        else if (previous.Contains("trs_discountamount") && previous.Attributes["trs_discountamount"] != null)
                            discountAmount = ((Money)previous.Attributes["trs_discountamount"]).Value;
                        totalPrice = partsPrice * quantity - discountAmount;
                    }
                    else
                    {
                        if (entity.Contains("trs_discountpercent") && entity.Attributes["trs_discountpercent"] != null)
                            discountPercent = (decimal)entity.Attributes["trs_discountpercent"];
                        else if (previous.Contains("trs_discountpercent") && previous.Attributes["trs_discountpercent"] != null)
                            discountPercent = (decimal)previous.Attributes["trs_discountpercent"];
                        totalPrice = partsPrice * quantity - (partsPrice * quantity * (discountPercent / 100));
                    }

                    entity.Attributes["trs_totalprice"] = new Money(totalPrice);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate_CalculateTotal : " + ex.Message.ToString());
            }
        }
        #endregion

        #region Fields
        #endregion
        #endregion
    }
}
