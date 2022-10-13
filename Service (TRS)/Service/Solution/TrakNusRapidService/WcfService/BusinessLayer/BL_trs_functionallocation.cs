using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Client.Services;

using TrakNusRapidService.DataLayer;
using TrakNusRapidService.Helper;

namespace TrakNusRapidServiceWcfService.BusinessLayer
{
    class BL_trs_functionallocation
    {
        #region Constants
        private const string _classname = "BL_trs_functionallocation";
        #endregion

        #region Dependencies
        private DL_trs_functionallocation _DL_trs_functionallocation = new DL_trs_functionallocation();
        private FMath _FMath = new FMath();
        #endregion

        #region Privates
        #endregion

        #region Publics
        public Guid CreateFunctionalLocation(IOrganizationService organizationService, Guid accountId
            , decimal longitude, decimal latitude, string area)
        {
            try
            {
                Guid id = Guid.Empty;
                _DL_trs_functionallocation = new DL_trs_functionallocation();
                _DL_trs_functionallocation.trs_customer = accountId;
                _DL_trs_functionallocation.trs_functionallongitude = _FMath.Truncate(longitude, Configuration.GPSMaxDigit);
                _DL_trs_functionallocation.trs_functionallatitude = _FMath.Truncate(latitude, Configuration.GPSMaxDigit);
                _DL_trs_functionallocation.trs_name = latitude.ToString() + "," + longitude.ToString();
                _DL_trs_functionallocation.trs_area = area;
                id = _DL_trs_functionallocation.Insert(organizationService);
                return id;
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".CreateFunctionalLocation : " + ex.Message.ToString());
            }
        }

        public Guid GetFunctionalLocationIdbyLongitudeLatitude(IOrganizationService organizationService, Guid accountId, decimal longitude, decimal latitude)
        {
            try
            {
                longitude = _FMath.Truncate(longitude, Configuration.GPSToleranceDigit);
                latitude = _FMath.Truncate(latitude, Configuration.GPSToleranceDigit);

                QueryExpression queryExpression = new QueryExpression(_DL_trs_functionallocation.EntityName);

                FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                filterExpression.AddCondition("trs_customer", ConditionOperator.Equal, accountId);
                if (longitude < 0)
                {
                    filterExpression.AddCondition("trs_functionallongitude", ConditionOperator.GreaterEqual, longitude - Configuration.GPSTolerance);
                    filterExpression.AddCondition("trs_functionallongitude", ConditionOperator.LessThan, longitude);
                }
                else
                {
                    filterExpression.AddCondition("trs_functionallongitude", ConditionOperator.GreaterEqual, longitude);
                    filterExpression.AddCondition("trs_functionallongitude", ConditionOperator.LessThan, longitude + Configuration.GPSTolerance);
                }
                if (latitude < 0)
                {
                    filterExpression.AddCondition("trs_functionallatitude", ConditionOperator.GreaterEqual, latitude - Configuration.GPSTolerance);
                    filterExpression.AddCondition("trs_functionallatitude", ConditionOperator.LessThan, latitude);
                }
                else
                {
                    filterExpression.AddCondition("trs_functionallatitude", ConditionOperator.GreaterEqual, latitude);
                    filterExpression.AddCondition("trs_functionallatitude", ConditionOperator.LessThan, latitude + Configuration.GPSTolerance);
                }

                EntityCollection eFunctionalLocation = _DL_trs_functionallocation.Select(organizationService, queryExpression);
                if (eFunctionalLocation.Entities.Count > 0)
                    return eFunctionalLocation.Entities[0].Id;
                else
                    return Guid.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".GetFunctionalLocationbyLongitudeLatitude : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}