using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using TrakNusRapidService.DataLayer;

namespace TrakNusRapidService.Workflow.BusinessLayer
{
    class BL_trs_toolsusage
    {
        #region Constants
        private const string _classname = "BL_trs_toolsusage";
        private const int _depth = 1;
        #endregion

        #region Depedencies        
        private DL_trs_toolsusage _DL_trs_toolsusage = new DL_trs_toolsusage();
        #endregion

        #region Events
        public void updateToolsUsage(IOrganizationService service, Guid usageId, string toolsName, DateTime dateReturned, int conditionReturned)
        {
            _DL_trs_toolsusage.trs_toolsname = toolsName;
            _DL_trs_toolsusage.trs_datereturned = dateReturned;
            _DL_trs_toolsusage.trs_conditionreturned = conditionReturned;
            _DL_trs_toolsusage.Update(service, usageId);
        }

        public void createToolsUsage(IOrganizationService service, string toolsName, DateTime dateBorrowed, int conditionBorrowed, Guid toolsMaster)
        {
            _DL_trs_toolsusage.trs_toolsname = toolsName;
            _DL_trs_toolsusage.trs_datereturned = dateBorrowed;
            _DL_trs_toolsusage.trs_conditionreturned = conditionBorrowed;
            //_DL_trs_toolsusage.trs_toolsmaster = toolsMaster;
            _DL_trs_toolsusage.Insert(service);
        }
        #endregion
    }
}
