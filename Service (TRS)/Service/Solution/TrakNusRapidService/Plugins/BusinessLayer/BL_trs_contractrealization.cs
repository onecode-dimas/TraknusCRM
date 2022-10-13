using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;

using TrakNusRapidService.DataLayer;

namespace TrakNusRapidService.Plugins.BusinessLayer
{
    class BL_trs_contractrealization
    {
        #region Constants
        private const string _classname = "BL_trs_contractrealization";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_trs_contractrealization _DL_trs_contractrealization = new DL_trs_contractrealization();
        #endregion

        #region Privates
        #endregion

        #region Events
        #region Forms
        public void Form_OnCreate(IOrganizationService organizationService, IPluginExecutionContext pluginExecutionContext)
        {
            //try
            //{
            //    Entity entity = (Entity)pluginExecutionContext.InputParameters["Target"];
            //    if (entity.LogicalName == _DL_trs_contractrealization.EntityName)
            //    {
            //        ExecuteWorkflowRequest req = new ExecuteWorkflowRequest();
            //        req.WorkflowId = new Guid("D0B126F8-84A1-4E1A-8A10-42CC6B7B68E0");
            //        req.EntityId = new Guid("239CB2DC-C27E-E411-B4E3-C4346BAC57E3");
            //        organizationService.Execute(req);
            //    }
            //    else
            //    {
            //        return;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate : " + ex.Message);
            //}
        }
        #endregion
        #endregion
    }
}
