using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using TrakNusRapidService.DataLayer;

namespace TrakNusRapidService.Plugins.BusinessLayer
{
    class BL_trs_toolstransfer
    {
        #region Constants
        private const string _classname = "BL_trs_toolstransfer";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_trs_toolstransfer _DL_trs_toolstransfer = new DL_trs_toolstransfer();
        private DL_trs_toolsmaster _DL_trs_toolsmaster = new DL_trs_toolsmaster();
        #endregion

        #region Events
        public void transferTools(IOrganizationService service, Entity postEntityImage)
        {
            try
            {
                //Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                if (postEntityImage.LogicalName == _DL_trs_toolstransfer.EntityName)
                {
                    // Update Tools Master Location
                    Guid toolsMasterId = ((EntityReference)postEntityImage["trs_tools"]).Id;
                    EntityReference trs_plant = new EntityReference
                    {
                        Id = ((EntityReference)postEntityImage["trs_branch"]).Id,
                        LogicalName = ((EntityReference)postEntityImage["trs_branch"]).LogicalName
                    };
                    _DL_trs_toolsmaster.trs_branch = trs_plant;
                    _DL_trs_toolsmaster.Update(service, toolsMasterId);                    
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".transferTools : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}
