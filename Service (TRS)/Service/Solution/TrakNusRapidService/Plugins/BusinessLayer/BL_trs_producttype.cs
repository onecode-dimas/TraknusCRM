using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using TrakNusRapidService.DataLayer;
using TrakNusRapidService.Helper;

namespace TrakNusRapidService.Plugins.BusinessLayer
{
    class BL_trs_producttype
    {
        #region Constants
        private const string _classname = "BL_trs_producttype";
        #endregion

        #region Depedencies
        private DL_trs_producttype _DL_trs_producttype = new DL_trs_producttype();
        #endregion

        #region Privates
        private void SendtoMobile(IOrganizationService organizationService, Guid id)
        {
            try
            {
                FMobile _fmobile = new FMobile(organizationService);
                _fmobile.SendProductType(organizationService, id);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".SendtoMobile : " + ex.Message);
            }
        }
        #endregion

        #region Events
        #region Forms
        public void Form_OnCreate(IOrganizationService organizationService, IPluginExecutionContext pluginExcecutionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExcecutionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_trs_producttype.EntityName)
                {
                    SendtoMobile(organizationService, entity.Id);
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

        public void Form_OnUpdate(IOrganizationService organizationService, IPluginExecutionContext pluginExcecutionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExcecutionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_trs_producttype.EntityName)
                {

                    //if (entity.Attributes.Contains("producttypecode") && entity.Attributes["producttypecode"] != null)
                    //{
                        //OptionSetValue opProdType = new OptionSetValue();
                        //opProdType.Value = ((OptionSetValue)entity.Attributes["producttypecode"]).Value;

                        //if (opProdType.Value == 1)
                        //{
                                SendtoMobile(organizationService, entity.Id);
                    //    }
                    //}
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate : " + ex.Message.ToString());
            }
        }
        
        #endregion

        #region Fields
        #endregion
        #endregion
    }
}
