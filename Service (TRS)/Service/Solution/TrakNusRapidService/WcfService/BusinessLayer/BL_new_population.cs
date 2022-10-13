using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Xrm.Sdk;

using TrakNusRapidService.DataLayer;
using TrakNusRapidService.Helper;

namespace TrakNusRapidServiceWcfService.BusinessLayer
{
    public class BL_new_population
    {
        #region Constants
        private const string _classname = "BL_new_population";
        #endregion

        #region Dependencies
        private DL_new_population _DL_new_population = new DL_new_population();
        private DL_serviceappointment _DL_serviceappointment = new DL_serviceappointment();
        private BL_trs_functionallocation _BL_trs_functionallocation = new BL_trs_functionallocation();
        #endregion

        #region Privates
        #endregion

        #region Publics
        public void UpdatePopulationLocation(IOrganizationService organizationService, Guid woId
            , Guid populationId, Guid accountId
            , decimal longitude, decimal latitude, string area
            , DateTime syncTime)
        {
            try
            {
                if (populationId == Guid.Empty)
                {
                    throw new Exception("Population Id can not empty !");
                }
                else
                {
                    Guid functionalLocationId = _BL_trs_functionallocation.GetFunctionalLocationIdbyLongitudeLatitude(organizationService, accountId, longitude, latitude);
                    if (functionalLocationId == Guid.Empty)
                        functionalLocationId = _BL_trs_functionallocation.CreateFunctionalLocation(organizationService, accountId, longitude, latitude, area);
                    
                    if (functionalLocationId == Guid.Empty)
                        throw new Exception("Can not found Functional Location.");
                    else
                    {
                        _DL_new_population = new DL_new_population();
                        _DL_new_population.trs_functionallocation = functionalLocationId;
                        _DL_new_population.trs_synctime = syncTime;
                        _DL_new_population.Update(organizationService, populationId);

                        /* Close by Thomas - 19 March 2015 (karena branch ama plant-nya masih kosong)
                        _DL_serviceappointment = new DL_serviceappointment();
                        _DL_serviceappointment.trs_functionallocation = functionalLocationId;
                        _DL_serviceappointment.trs_synctime = syncTime;
                        _DL_serviceappointment.Update(organizationService, woId);

                        _fMobile.SendServiceAppointment_Header(organizationService, woId);
                         * */
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".UpdatePopulationLocation : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}