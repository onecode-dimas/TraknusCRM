using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;

using TrakNusRapidService.DataLayer;
using TrakNusRapidService.Helper;

namespace TrakNusRapidService.Plugins.BusinessLayer
{
    class BL_email
    {
        #region Constants
        private const string _classname = "BL_email";
        #endregion

        #region Depedencies
        private DL_email _DL_email = new DL_email();

        private FAutoEmailConfiguration _fAutoEmailConfiguration = new FAutoEmailConfiguration();
        #endregion

        #region Privates
        #endregion

        #region Events
        #region Forms
        public void Form_OnCreate_PreValidate(IOrganizationService organizationService, IPluginExecutionContext pluginExecutionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExecutionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_email.EntityName)
                {
                    if (entity.Attributes.Contains("trs_autosendcategory"))
                    {
                        _fAutoEmailConfiguration.Configure(organizationService
                            , entity.Id
                            , pluginExecutionContext.UserId
                            , (int)entity.GetAttributeValue<decimal>("trs_autosendcategory"));
                    }
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
        #endregion

        #region Fields
        #endregion
        #endregion
    }
}
