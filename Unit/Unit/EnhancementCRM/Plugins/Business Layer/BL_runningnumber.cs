using EnhancementCRM.HelperUnit.Data_Layer;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhancementCRM.Plugins.Business_Layer
{
    public class BL_runningnumber
    {
        #region Constants
        private const string _classname = "BL_runningnumber";
        #endregion

        #region Dependencies
        private DL_runningnumber _runningnumber = new DL_runningnumber();
        private DL_runningnumber_lastnumber _runningnumber_lastnumber = new DL_runningnumber_lastnumber();
        private DL_businessunit _businessunit = new DL_businessunit();
        private DL_categorycode _categorycode = new DL_categorycode();
        private DL_systemuser _systemuser = new DL_systemuser();
        #endregion

        #region Privates
        private void CreateNewLastNumber(IOrganizationService organizationService
            , IPluginExecutionContext pluginExecutionContext, int year, Guid runningNumberId)
        {
            try
            {
                _runningnumber_lastnumber = new DL_runningnumber_lastnumber();
                _runningnumber_lastnumber.trs_year = year.ToString();
                _runningnumber_lastnumber.trs_runningnumberid = runningNumberId;
                _runningnumber_lastnumber.trs_lastnumber = 0;
                _runningnumber_lastnumber.Insert(organizationService);
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
                QueryExpression queryExpression = new QueryExpression(_runningnumber_lastnumber.EntityName);
                queryExpression.ColumnSet.AddColumns("trs_lastnumber", "trs_lockingby");

                FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                filterExpression.AddCondition("trs_year", ConditionOperator.Equal, year.ToString());
                filterExpression.AddCondition("trs_runningnumberid", ConditionOperator.Equal, runningNumberId);

                EntityCollection entityCollection = _runningnumber_lastnumber.Select(organizationService, queryExpression);
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

        private void UpdateLastNumber(IOrganizationService organizationService, Guid id, decimal lastNumber)
        {
            try
            {
                _runningnumber_lastnumber = new DL_runningnumber_lastnumber();
                _runningnumber_lastnumber.trs_lastnumber = lastNumber;
                _runningnumber_lastnumber.Update(organizationService, id);
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
                Entity eBusinessUnit = _businessunit.Select(organizationService, pluginExecutionContext.BusinessUnitId);
                //throw new Exception("Test : " + eBusinessUnit.Attributes["trs_branchcode"].ToString());
                if (eBusinessUnit.Attributes.Contains("trs_branchcode"))
                {
                    branchCode = eBusinessUnit.Attributes["trs_branchcode"].ToString();
                }
                else
                {
                    throw new InvalidPluginExecutionException("Please setup Branch Code for Business Unit '" + eBusinessUnit.Attributes["name"].ToString() + "' first !");
                }
                Entity eSystemUser = _systemuser.Select(organizationService, userId);
                string initUser = string.Empty;
                if (eSystemUser.Attributes.Contains("new_initial"))
                {
                    initUser = eSystemUser.Attributes["new_initial"].ToString();
                }
                else
                {
                    throw new InvalidPluginExecutionException("Please setup Initial User for Last Number '" + eBusinessUnit.Attributes["name"].ToString() + "' first!");
                }

                QueryExpression queryExpression = new QueryExpression(_runningnumber.EntityName);
                queryExpression.ColumnSet.AddColumns("trs_prefix");

                FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                filterExpression.AddCondition("trs_entityname", ConditionOperator.Equal, entityName);

                EntityCollection entityCollection = _runningnumber.Select(organizationService, queryExpression);
                if (entityCollection.Entities.Count > 0)
                {
                    Entity entity = entityCollection.Entities[0];

                    Guid lastNumberId = new Guid();
                    decimal lastNumber = 0;
                    Guid lockingBy = new Guid();

                    GetLastNumber(organizationService, pluginExecutionContext, year, entity.Id, out lastNumberId, out lastNumber, out lockingBy);

                    lastNumber++;
                    string newRunningNumber = entity.Attributes["trs_prefix"].ToString();
                    newRunningNumber += "." + year.ToString().Substring(2, 2) + month;
                    newRunningNumber += "-" + "03";
                    newRunningNumber += "." + branchCode;
                    newRunningNumber += lastNumber.ToString("00000");
                    newRunningNumber += "-" + initUser;

                    UpdateLastNumber(organizationService, lastNumberId, lastNumber);

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
        public string GenerateNewRunningNumberSalesOrder(IOrganizationService organizationService
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
                Entity eBusinessUnit = _businessunit.Select(organizationService, pluginExecutionContext.BusinessUnitId);
                //throw new Exception("Test : " + eBusinessUnit.Attributes["trs_branchcode"].ToString());
                if (eBusinessUnit.Attributes.Contains("trs_branchcode"))
                {
                    branchCode = eBusinessUnit.Attributes["trs_branchcode"].ToString();
                }
                else
                {
                    throw new InvalidPluginExecutionException("Please setup Branch Code for Business Unit '" + eBusinessUnit.Attributes["name"].ToString() + "' first !");
                }

                QueryExpression queryExpression = new QueryExpression(_runningnumber.EntityName);
                queryExpression.ColumnSet.AddColumns("trs_prefix");

                FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                filterExpression.AddCondition("trs_entityname", ConditionOperator.Equal, entityName);

                EntityCollection entityCollection = _runningnumber.Select(organizationService, queryExpression);
                if (entityCollection.Entities.Count > 0)
                {
                    Entity entity = entityCollection.Entities[0];

                    Guid lastNumberId = new Guid();
                    decimal lastNumber = 0;
                    Guid lockingBy = new Guid();

                    GetLastNumber(organizationService, pluginExecutionContext, year, entity.Id, out lastNumberId, out lastNumber, out lockingBy);

                    lastNumber++;
                    string newRunningNumber = entity.Attributes["trs_prefix"].ToString();
                    newRunningNumber += "." + year.ToString().Substring(2, 2) + month;
                    newRunningNumber += "-" + "03";
                    newRunningNumber += "-" + branchCode;
                    newRunningNumber += lastNumber.ToString("00000");

                    UpdateLastNumber(organizationService, lastNumberId, lastNumber);

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

        public string GenerateNewRunningNumberModulMarketSize(IOrganizationService organizationService
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
                Entity eBusinessUnit = _businessunit.Select(organizationService, pluginExecutionContext.BusinessUnitId);
                //throw new Exception("Test : " + eBusinessUnit.Attributes["trs_branchcode"].ToString());
                if (eBusinessUnit.Attributes.Contains("trs_branchcode"))
                {
                    branchCode = eBusinessUnit.Attributes["trs_branchcode"].ToString();
                }
                else
                {
                    throw new InvalidPluginExecutionException("Please setup Branch Code for Business Unit '" + eBusinessUnit.Attributes["name"].ToString() + "' first !");
                }
                Entity eSystemUser = _systemuser.Select(organizationService, userId);


                QueryExpression queryExpression = new QueryExpression(_runningnumber.EntityName);
                queryExpression.ColumnSet.AddColumns("trs_prefix");

                FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                filterExpression.AddCondition("trs_entityname", ConditionOperator.Equal, entityName);

                EntityCollection entityCollection = _runningnumber.Select(organizationService, queryExpression);
                if (entityCollection.Entities.Count > 0)
                {
                    Entity entity = entityCollection.Entities[0];

                    Guid lastNumberId = new Guid();
                    decimal lastNumber = 0;
                    Guid lockingBy = new Guid();

                    GetLastNumber(organizationService, pluginExecutionContext, year, entity.Id, out lastNumberId, out lastNumber, out lockingBy);

                    lastNumber++;
                    string newRunningNumber = entity.Attributes["trs_prefix"].ToString();
                    newRunningNumber += "." + year.ToString().Substring(2, 2) + month;
                    //newRunningNumber += "-" + "03";
                    newRunningNumber += "-" + branchCode;
                    newRunningNumber += lastNumber.ToString("00000");

                    UpdateLastNumber(organizationService, lastNumberId, lastNumber);

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
