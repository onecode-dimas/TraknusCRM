using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;

using TrakNusRapidService.DataLayer;
using TrakNusRapidService.Helper;

namespace TrakNusRapidService.Plugins.BusinessLayer
{
    class BL_trs_productsection
    {
        #region Constants
        private const string _classname = "BL_trs_productsection";
        #endregion

        #region Depedencies
        private DL_trs_productsection _DL_trs_productsection = new DL_trs_productsection();
        #endregion

        #region Privates
        private void SendtoMobile(IOrganizationService organizationService, Guid id)
        {
            try
            {
                FMobile _fmobile = new FMobile(organizationService);
                _fmobile.SendProductSection(organizationService, id);
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
                if (entity.LogicalName == _DL_trs_productsection.EntityName)
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
                if (entity.LogicalName == _DL_trs_productsection.EntityName)
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
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate : " + ex.Message.ToString());
            }
        }
        #endregion
        #endregion
    }
}
