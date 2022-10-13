using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Xrm.Sdk;

using TrakNusRapidService.DataLayer;

namespace TrakNusRapidServiceWcfService.BusinessLayer
{
    public class BL_trs_commercialdetail
    {
        #region Constants
        private const string _classname = "BL_trs_commercialdetail";
        #endregion

        #region Dependencies
        private DL_trs_commercialdetail _DL_trs_commercialdetail = new DL_trs_commercialdetail();
        #endregion

        #region Privates
        #endregion

        #region Publics
        public void UpdateFinishTime(IOrganizationService organizationService, Guid id
            , DateTime automaticTime, DateTime manualTime
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
                    Entity entity = _DL_trs_commercialdetail.Select(organizationService, id);
                    if ((DateTime)entity.Attributes["modifiedon"] < modifiedOn)
                    {
                        _DL_trs_commercialdetail = new DL_trs_commercialdetail();
                        if (automaticTime != DateTime.MinValue)
                            _DL_trs_commercialdetail.trs_automatictime = automaticTime;
                        if (manualTime != DateTime.MinValue)
                            _DL_trs_commercialdetail.trs_manualtime = manualTime;
                        _DL_trs_commercialdetail.trs_synctime = syncTime;
                        _DL_trs_commercialdetail.Update(organizationService, id);

                        _DL_trs_commercialdetail.Completed(organizationService, id);
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