using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TrakNusSparepartSystem.DataLayer;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Crm.Sdk.Messages;
using TrakNusSparepartSystem.Helper;

namespace TrakNusSparepartSystem.Plugins.BusinessLayer
{
    public class BL_tss_sparepartsetup
    {
        #region Constants
        private const string _classname = "BL_tss_sparepartsetup";
        private const string _entityname = "tss_sparepartsetup";
        #endregion

        #region UPDATE
        public void Form_OnUpdate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                //Entity entity = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));

                if (entity.LogicalName == _entityname)
                {
                    if (entity.Attributes.Contains("tss_evaluationdurationms"))
                    {
                        int _evaluationdurationms = entity.GetAttributeValue<int>("tss_evaluationdurationms");

                        if (_evaluationdurationms > 30)
                        {
                            throw new InvalidPluginExecutionException("Evaluation Duration (in days) must less or equal 30 days !");
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate_PreOperation : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}
