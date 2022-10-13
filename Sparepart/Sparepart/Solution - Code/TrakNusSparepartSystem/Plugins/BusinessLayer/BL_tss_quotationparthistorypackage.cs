using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using TrakNusSparepartSystem.DataLayer;

namespace TrakNusSparepartSystem.Plugins.BusinessLayer
{
    public class BL_tss_quotationparthistorypackage
    {
        #region Constant
        private string _classname = "BL_tss_quotationparthistorypackage";
        private string _entityname = "tss_quotationparthistorypackage";

        public string EntityName
        {
            get { return _entityname; }
        }
        #endregion

        #region Plugin
        public void Form_OnCreate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {

                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                String packageNo = String.Empty;
                String packageName = String.Empty;
                if (entity.LogicalName == _entityname)
                {
                    if (entity.Attributes.Contains("tss_packageno")) packageNo = entity.GetAttributeValue<String>("tss_packageno");
                    if (entity.Attributes.Contains("tss_packagename")) packageName = entity.GetAttributeValue<String>("tss_packagename");

                    if (entity.Attributes.Contains("tss_name"))
                        entity.Attributes["tss_name"] = packageNo + " - "+ packageName;
                    else
                        entity.Attributes.Add("tss_name", packageNo + " - "+ packageName);
                   
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PreOperation: " + ex.Message.ToString());
            }
        }
        #endregion
    }
}
