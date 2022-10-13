using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Xrm.Sdk;

using TrakNusRapidService.DataLayer;
using TrakNusRapidService.Helper;

namespace TrakNusRapidServiceWcfService.BusinessLayer
{
    public class BL_equipment
    {
        #region Constants
        private const string _classname = "BL_equipment";
        #endregion

        #region Dependencies
        private DL_equipment _DL_equipment = new DL_equipment();
        private FMath _FMath = new FMath();
        #endregion

        #region Privates
        #endregion

        #region Publics
        public void UpdateMechanicLocation(IOrganizationService organizationService, Guid id
            , decimal longitude, decimal latitude
            , DateTime syncTime)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new Exception("Mechanic Id can not empty !");
                }
                else
                {
                    _DL_equipment = new DL_equipment();
                    _DL_equipment.trs_longitude = _FMath.Truncate(longitude, Configuration.GPSMaxDigit);
                    _DL_equipment.trs_latitude = _FMath.Truncate(latitude, Configuration.GPSMaxDigit);
                    _DL_equipment.Update(organizationService, id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".UpdateMechanicLocation : " + ex.Message.ToString());
            }
        }

        public void SendPasswordMechanic(IOrganizationService organizationService, Guid userId, Guid id, string password)
        {
            try
            {
                FMobile _fMobile = new FMobile(organizationService);
                Entity entity = _DL_equipment.Select(organizationService, id);
                if (entity.Attributes.Contains("trs_nrp"))
                    _fMobile.SendEmail_PasswordMechanic(organizationService, userId, entity.GetAttributeValue<string>("trs_nrp"), password);
                else
                    throw new Exception("Can not found Mechanic with Id : " + id.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendPasswordMechanic : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}