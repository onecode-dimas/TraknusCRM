using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using TrakNusRapidService.DataLayer;

namespace TrakNusRapidService.Workflow.BusinessLayer
{
    class BL_trs_toolstransfer
    {
        #region Constants
        private const string _classname = "BL_trs_toolstransfer";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_trs_toolstransfer _DL_trs_toolstransfer = new DL_trs_toolstransfer();        
        #endregion

        #region Events
        public void transferTools(IOrganizationService service, Guid toolsMaster, DateTime dateTransfer, string transferReason, Guid toBranch)
        {
            _DL_trs_toolstransfer.trs_tools = toolsMaster;
            _DL_trs_toolstransfer.trs_branch = toBranch;
            _DL_trs_toolstransfer.trs_transferreason = transferReason;
            _DL_trs_toolstransfer.trs_transferdate = dateTransfer;            
            _DL_trs_toolstransfer.Insert(service);
        }
        #endregion
    }
}
