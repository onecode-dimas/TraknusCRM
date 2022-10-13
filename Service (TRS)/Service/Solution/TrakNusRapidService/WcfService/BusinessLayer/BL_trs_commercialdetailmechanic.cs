using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Xrm.Sdk;

using TrakNusRapidService.DataLayer;
using TrakNusRapidService.Helper;

namespace TrakNusRapidServiceWcfService.BusinessLayer
{
    public class BL_trs_commercialdetailmechanic
    {
        #region Constants
        private const string _classname = "BL_trs_commercialdetailmechanic";
        #endregion

        #region Dependencies
        private DL_trs_commercialdetailmechanic _DL_trs_commercialdetailmechanic = new DL_trs_commercialdetailmechanic();
        #endregion

        #region Privates
        #endregion

        #region Publics
        public void UpdateMechanicRole(IOrganizationService organizationService, Guid id
            , int mechanicRole
            , DateTime modifiedOn, DateTime syncTime)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new Exception("Primary Key can not empty !");
                }
                else
                {
                    Entity entity = _DL_trs_commercialdetailmechanic.Select(organizationService, id);
                    if ((DateTime)entity.Attributes["modifiedon"] < modifiedOn)
                    {
                        FMobile _fMobile = new FMobile(organizationService);

                        _DL_trs_commercialdetailmechanic = new DL_trs_commercialdetailmechanic();
                        _DL_trs_commercialdetailmechanic.trs_mechanicrole = _fMobile.ConvertMechanicRole_MobiletoCRM(mechanicRole);
                        _DL_trs_commercialdetailmechanic.trs_synctime = syncTime;
                        _DL_trs_commercialdetailmechanic.Update(organizationService, id);
                    }
                    else
                    {
                        throw new Exception("CRM more update.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".UpdateFinishTime : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}