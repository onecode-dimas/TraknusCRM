using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Xrm.Sdk;

using TrakNusRapidService.DataLayer;

namespace TrakNusRapidServiceWcfService.BusinessLayer
{
    public class BL_trs_workorderpartssummary
    {
        #region Constants
        private const string _classname = "BL_trs_workorderpartssummary";
        #endregion

        #region Dependencies
        DL_trs_workorderpartssummary _DL_trs_workorderpartssummary = new DL_trs_workorderpartssummary();
        BL_serviceappointment _BL_serviceappointment = new BL_serviceappointment();
        #endregion

        #region Privates
        #endregion

        #region Publics
        public void UpdateReturnedQuantity(IOrganizationService organizationService, Guid id
            , int quantity, DateTime modifiedOn
            , DateTime syncTime)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new Exception("Primary Key can not empty !");
                }
                else
                {
                    Entity entity = _DL_trs_workorderpartssummary.Select(organizationService, id);
                    if (_BL_serviceappointment.IsAlreadySubmitTECObyMechanic(organizationService, entity.GetAttributeValue<EntityReference>("trs_workorder").Id))
                    {
                        throw new Exception("Can not update WO with status 'Closed'.");
                    }
                    else
                    {
                        _DL_trs_workorderpartssummary = new DL_trs_workorderpartssummary();
                        _DL_trs_workorderpartssummary.trs_returnedquantity = quantity;
                        _DL_trs_workorderpartssummary.trs_synctime = syncTime;
                        _DL_trs_workorderpartssummary.Update(organizationService, id);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".UpdateReturnedQuantity : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}