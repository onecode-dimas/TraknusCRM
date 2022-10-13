using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using TrakNusRapidService.DataLayer;

namespace TrakNusRapidService.Workflow.BusinessLayer
{
    class BL_trs_toolsmaster
    {
        #region Constants
        private const string _classname = "BL_trs_toolsmaster";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_trs_toolsmaster _DL_trs_toolsmaster = new DL_trs_toolsmaster();
        #endregion

        #region Events
        public void updateToolsStatusBranch(IOrganizationService service, Guid toolsId, bool status, Guid branch)
        {
            EntityReference trs_plant = new EntityReference
            {
                Id = branch,
                LogicalName = "businessunit"
            };
            _DL_trs_toolsmaster.trs_branch = trs_plant;
            _DL_trs_toolsmaster.trs_status = status;
            _DL_trs_toolsmaster.Update(service, toolsId);            
        }
        #endregion
    }
}
