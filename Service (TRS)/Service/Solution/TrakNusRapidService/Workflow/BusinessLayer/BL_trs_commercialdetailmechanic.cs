using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using TrakNusRapidService.DataLayer;
using TrakNusRapidService.Helper;
using TrakNusRapidService.Helper.MobileWebService;

namespace TrakNusRapidService.Workflow.BusinessLayer
{
    public class BL_trs_commercialdetailmechanic
    {
        #region Constants
        private const string _classname = "BL_trs_commercialdetailmechanic";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_trs_commercialdetailmechanic _DL_trs_commercialdetailmechanic = new DL_trs_commercialdetailmechanic();
        #endregion

        #region Events
        public void Form_OnCreate(Guid id, string _nrp, Int32 mRole, IOrganizationService service)
        {             
            try
            {
                _DL_trs_commercialdetailmechanic.trs_commercialdetailid = id;
                _DL_trs_commercialdetailmechanic.trs_nrp = _nrp;
                _DL_trs_commercialdetailmechanic.trs_mechanicrole = mRole;
                _DL_trs_commercialdetailmechanic.Insert(service);
            }
             
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(_classname + ".Form_OnCreate : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}
