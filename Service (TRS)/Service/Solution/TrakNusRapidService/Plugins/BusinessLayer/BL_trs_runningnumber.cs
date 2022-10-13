using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using TrakNusRapidService.Helper;
using TrakNusRapidService.DataLayer;

namespace TrakNusRapidService.Plugins.BusinessLayer
{
    class BL_trs_runningnumber
    {
        #region Constants
        private const string _classname = "BL_trs_runningnumber";
        private const int _depth = 1;
        #endregion

        #region Dependencies
        private DL_trs_runningnumber _DL_trs_runningnumber = new DL_trs_runningnumber();
        private DL_trs_runningnumberlastnumber _DL_trs_runningnumberlastnumber = new DL_trs_runningnumberlastnumber();
        private DL_businessunit _DL_businessunit = new DL_businessunit();
        #endregion

        #region Privates
        private void CreateNewLastNumber(IOrganizationService organizationService
            , IPluginExecutionContext pluginExecutionContext, int year, Guid runningNumberId)
        {
            try
            {
                _DL_trs_runningnumberlastnumber = new DL_trs_runningnumberlastnumber();
                _DL_trs_runningnumberlastnumber.trs_year = year.ToString();
                _DL_trs_runningnumberlastnumber.trs_runningnumberid = runningNumberId;
                _DL_trs_runningnumberlastnumber.trs_lastnumber = 0;
                _DL_trs_runningnumberlastnumber.Insert(organizationService);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".CreateNewLastNumber : " + ex.Message.ToString());
            }
        }

        private void GetLastNumber(IOrganizationService organizationService
            , IPluginExecutionContext pluginExecutionContext, int year, Guid runningNumberId
            , out Guid id, out decimal lastNumber, out Guid lockingBy)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_trs_runningnumberlastnumber.EntityName);
                queryExpression.ColumnSet.AddColumns("trs_lastnumber", "trs_lockingby");

                FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                filterExpression.AddCondition("trs_year", ConditionOperator.Equal, year.ToString());
                filterExpression.AddCondition("trs_runningnumberid", ConditionOperator.Equal, runningNumberId);

                EntityCollection entityCollection = _DL_trs_runningnumberlastnumber.Select(organizationService, queryExpression);
                if (entityCollection.Entities.Count > 0)
                {
                    Entity entity = entityCollection.Entities[0];
                    id = entity.Id;
                    lastNumber = Convert.ToDecimal(entity.Attributes["trs_lastnumber"].ToString());
                    if (entity.Attributes.Contains("trs_lockingby"))
                        lockingBy = ((EntityReference)entity.Attributes["trs_lockingby"]).Id;
                    else
                        lockingBy = new Guid();
                }
                else
                {
                    CreateNewLastNumber(organizationService, pluginExecutionContext, year, runningNumberId);
                    GetLastNumber(organizationService, pluginExecutionContext, year, runningNumberId, out id, out lastNumber, out lockingBy);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".GetLastNumber : " + ex.Message.ToString());
            }
        }

        private void LockLastNumber(Guid id, Guid user)
        {
            try
            {
                string query = "UPDATE " + _DL_trs_runningnumberlastnumber.EntityName + "Base" + Convert.ToChar(13);
                query += "SET trs_lockingby = '" + user.ToString() + "'" + Convert.ToChar(13);
                query += "WHERE trs_runningnumberlastnumberId = '" + id.ToString() + "'" + Convert.ToChar(13);
                query += " AND trs_lockingby IS NULL OR trs_lockingby = '" + Guid.Empty.ToString() + "'";

                ODBCConnector odbcConnector = new ODBCConnector();
                odbcConnector.ConnectionName = Configuration.ConnectionNameODBC;
                odbcConnector.ConnectionType = ODBCConnectorType.SqlServer2008;
                odbcConnector.Update(query);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".LockLastNumber : " + ex.Message.ToString());
            }
        }

        private void UnlockLastNumber(Guid id, Guid user)
        {
            try
            {
                string query = "UPDATE " + _DL_trs_runningnumberlastnumber.EntityName + "Base" + Convert.ToChar(13);
                query += "SET trs_lockingby = '" + Guid.Empty.ToString() + "'" + Convert.ToChar(13);
                query += "WHERE trs_runningnumberlastnumberId = '" + id.ToString() + "'" + Convert.ToChar(13);
                query += " AND trs_lockingby = '" + user.ToString() + "'";
                
                ODBCConnector odbcConnector = new ODBCConnector();
                odbcConnector.ConnectionName = Configuration.ConnectionNameODBC;
                odbcConnector.ConnectionType = ODBCConnectorType.SqlServer2008;
                odbcConnector.Update(query);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".LockLastNumber : " + ex.Message.ToString());
            }
        }

        private void UpdateLastNumber(IOrganizationService organizationService, Guid id, decimal lastNumber)
        {
            try
            {
                _DL_trs_runningnumberlastnumber = new DL_trs_runningnumberlastnumber();
                _DL_trs_runningnumberlastnumber.trs_lastnumber = lastNumber;
                _DL_trs_runningnumberlastnumber.Update(organizationService, id);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".UpdateLastNumber : " + ex.Message.ToString());
            }
        }
        #endregion

        #region Publics
        public string GenerateNewRunningNumber(IOrganizationService organizationService
            , IPluginExecutionContext pluginExecutionContext, string entityName, DateTime createdDate)
        {
            try
            {
                int year = createdDate.Year;
                string month = createdDate.Month.ToString();
                if (month.Length < 2)
                    month = "0" + month;

                Guid userId = pluginExecutionContext.UserId;
                string branchCode = string.Empty;                
                Entity eBusinessUnit = _DL_businessunit.Select(organizationService, pluginExecutionContext.BusinessUnitId);
                //throw new Exception("Test : " + eBusinessUnit.Attributes["trs_branchcode"].ToString());
                if (eBusinessUnit.Attributes.Contains("trs_branchcode"))
                {
                    branchCode = eBusinessUnit.Attributes["trs_branchcode"].ToString();
                }
                else
                {
                    throw new InvalidPluginExecutionException("Please setup Branch Code for Business Unit '" + eBusinessUnit.Attributes["name"].ToString() + "' first !");
                }
                
                QueryExpression queryExpression = new QueryExpression(_DL_trs_runningnumber.EntityName);
                queryExpression.ColumnSet.AddColumns("trs_prefix");

                FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                filterExpression.AddCondition("trs_entityname", ConditionOperator.Equal, entityName);

                EntityCollection entityCollection = _DL_trs_runningnumber.Select(organizationService, queryExpression);                
                if (entityCollection.Entities.Count > 0)
                {
                    Entity entity = entityCollection.Entities[0];

                    Guid lastNumberId = new Guid();
                    decimal lastNumber = 0;
                    Guid lockingBy = new Guid();
                    
                    GetLastNumber(organizationService, pluginExecutionContext, year, entity.Id, out lastNumberId, out lastNumber, out lockingBy);
                    //if (lockingBy != userId)
                    //{
                    //    LockLastNumber(lastNumberId, userId);
                    //    GetLastNumber(organizationService, pluginExecutionContext, year, entity.Id, out lastNumberId, out lastNumber, out lockingBy);
                    //    if (lockingBy != userId)
                    //        throw new InvalidPluginExecutionException("Failed to lock last number. Please try again later !");
                    //}

                    lastNumber++;
                    string newRunningNumber = entity.Attributes["trs_prefix"].ToString();
                    newRunningNumber += "." + year.ToString().Substring(2, 2) + month;
                    newRunningNumber += "-" + branchCode;
                    newRunningNumber += lastNumber.ToString("000000");

                    UpdateLastNumber(organizationService, lastNumberId, lastNumber);
                    //UnlockLastNumber(lastNumberId, userId);

                    return newRunningNumber.ToUpper();
                }
                else
                {
                    throw new InvalidPluginExecutionException("Please setup running number first for Entity Name '" + entityName + "'.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".GenerateNewRunningNumber : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}
