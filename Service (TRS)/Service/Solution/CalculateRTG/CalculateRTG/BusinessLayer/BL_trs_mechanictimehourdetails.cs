using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Client.Services;
using CalculateRTG.DataLayer;

namespace CalculateRTG.BusinessLayer
{
    public class BL_trs_mechanictimehourdetails
    {
        #region Constants
        private const string _classname = "BL_trs_mechanictimehourdetails";
        private const int _depth = 1;
        #endregion


        #region Depedencies
        private DL_trs_mechanictimehourdetails _DL_trs_mechanictimehourdetails = new DL_trs_mechanictimehourdetails();
        #endregion

        public void Form_OnCreate(IOrganizationService organizationService, Guid _mId, Guid _woId, decimal _actual, decimal _repair, string _name)
        {
            try
            {
                _DL_trs_mechanictimehourdetails.trs_mechanicid = _mId;
                _DL_trs_mechanictimehourdetails.trs_workordernumber = _woId;
                _DL_trs_mechanictimehourdetails.trs_actualtimehour = _actual;
                _DL_trs_mechanictimehourdetails.trs_repairtimeguide = _repair;
                _DL_trs_mechanictimehourdetails.trs_name = _name;
                _DL_trs_mechanictimehourdetails.Insert(organizationService);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Form_OnCreate : " + ex.Message.ToString());
            }
        }
    }
}
