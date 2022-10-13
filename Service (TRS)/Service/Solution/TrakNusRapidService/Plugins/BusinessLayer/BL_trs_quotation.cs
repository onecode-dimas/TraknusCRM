using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using TrakNusRapidService.DataLayer;
using TrakNusRapidService.Helper;

namespace TrakNusRapidService.Plugins.BusinessLayer
{
    class BL_trs_quotation
    {
        #region Constants
        private const string _classname = "BL_trs_quotation";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_trs_quotation _DL_trs_quotation = new DL_trs_quotation();
        private DL_trs_quotationcommercialheader _DL_trs_quotationcommercialheader = new DL_trs_quotationcommercialheader();
        private DL_trs_quotationpartdetail _DL_trs_quotationpartdetail = new DL_trs_quotationpartdetail();
        private DL_trs_quotationsupportingmaterial _DL_trs_quotationsupportingmaterial = new DL_trs_quotationsupportingmaterial();
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        private DL_systemuserroles _DL_systemuserroles = new DL_systemuserroles();
        private DL_trs_discountapproval _DL_trs_discountapproval = new DL_trs_discountapproval();
        private BL_trs_runningnumber _BL_trs_runningnumber = new BL_trs_runningnumber();
        private DL_trs_runningnumberlastnumber _DL_trs_runningnumberlastnumber = new DL_trs_runningnumberlastnumber();
        private DL_businessunit _DL_businessunit = new DL_businessunit();
        private DL_trs_runningnumber _DL_trs_runningnumber = new DL_trs_runningnumber();
        #endregion

        #region Privates

        private void SendEmail(IOrganizationService organizationService, Guid sender, Guid businessunitid, Guid roleid, string subject, string body)
        {
            try
            {
                QueryExpression qeSystemUserRoles = new QueryExpression("systemuserroles");
                qeSystemUserRoles.ColumnSet = new ColumnSet(true);

                LinkEntity role = new LinkEntity();
                role.LinkFromEntityName = "systemuserroles";
                role.LinkFromAttributeName = "roleid";
                role.LinkToEntityName = "role";
                role.LinkToAttributeName = "roleid";
                role.JoinOperator = JoinOperator.Inner;
                role.Columns.AddColumns("businessunitid");
                role.EntityAlias = "role";
                role.LinkCriteria.AddCondition("businessunitid", ConditionOperator.Equal, businessunitid);

                qeSystemUserRoles.LinkEntities.Add(role);
                qeSystemUserRoles.Criteria.AddCondition("roleid", ConditionOperator.Equal, roleid);
                EntityCollection ecSystemUserRoles = organizationService.RetrieveMultiple(qeSystemUserRoles);

                Guid emailId = Guid.Empty;
                EmailAgent emailAgent = new EmailAgent();
                emailAgent.AddSender(sender);
                foreach (Entity en in ecSystemUserRoles.Entities)
                {
                    if (en.Contains("systemuserid"))
                    {
                        emailAgent.AddReceiver(_DL_systemuser.EntityName, (Guid)en["systemuserid"]);
                    }
                }
                emailAgent.subject = subject;
                emailAgent.description = body;
                emailAgent.Create(organizationService, out emailId);
                //throw new Exception(receiver);
                //if (emailId != Guid.Empty)
                //    emailAgent.Send(organizationService, emailId);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendEmail : " + ex.Message.ToString());
            }
        }

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

        #region Events
        #region Forms
        public void Form_OnCreate(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_trs_quotation.EntityName)
                {
                    string newRunningNumber = _BL_trs_runningnumber.GenerateNewRunningNumber(organizationService, pluginExceptionContext, 
                                                        _DL_trs_quotation.EntityName, (DateTime)entity.Attributes["createdon"]);
                    _DL_trs_quotation = new DL_trs_quotation();
                    _DL_trs_quotation.trs_quotationnumber = newRunningNumber;
                    _DL_trs_quotation.Update(organizationService, entity.Id);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate : " + ex.Message.ToString());
            }
        }

        public void Form_OnUpdate(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                Entity pre = (Entity)pluginExceptionContext.PreEntityImages["Target"];
                Guid id = entity.Id;
                if (entity.LogicalName == _DL_trs_quotation.EntityName)
                {
                    if (entity.Attributes.Contains("statuscode"))
                    {
                        switch (entity.GetAttributeValue<OptionSetValue>("statuscode").Value)
                        {
                            case Configuration.Quo_Approve:
                                if (entity.GetAttributeValue<String>("trs_fillingnumber") == null)
                                {
                                    DateTime? createdon = null;
                                    if (entity.Attributes.Contains("createdon"))
                                        createdon = entity.GetAttributeValue<DateTime>("createdon");
                                    else if (pre.Attributes.Contains("createdon"))
                                        createdon = pre.GetAttributeValue<DateTime>("createdon");
                                    else
                                        throw new InvalidPluginExecutionException("Can not found created on.");

                                    string newRunningNumber = _BL_trs_runningnumber.GenerateNewRunningNumber(organizationService, pluginExceptionContext, _DL_trs_quotation.EntityName, (DateTime)createdon);
                                    _DL_trs_quotation = new DL_trs_quotation();
                                    _DL_trs_quotation.trs_fillingnumber = newRunningNumber;
                                    _DL_trs_quotation.Update(organizationService, entity.Id);
                                }
                                break;
                            case Configuration.Quo_Revised:
                                _DL_trs_quotation = new DL_trs_quotation();
                                if (!pre.Attributes.Contains("trs_revision"))
                                    _DL_trs_quotation.trs_revision = 1;
                                else
                                    _DL_trs_quotation.trs_revision = pre.GetAttributeValue<int>("trs_revision") + 1;
                                _DL_trs_quotation.Update(organizationService, entity.Id);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate : " + ex.Message.ToString());
            }
        }
        #endregion
        #region Fields
        public void Status_OnChange(IOrganizationService organizationService, IPluginExecutionContext pluginExecutionContext)
        {
            try
            {
                Entity ePre = (Entity)pluginExecutionContext.PreEntityImages["Target"];
                Entity entity = (Entity)pluginExecutionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_trs_quotation.EntityName && entity.Attributes.Contains("statuscode"))
                {
                    //Rico's
                    //if (entity.GetAttributeValue<OptionSetValue>("statuscode").Value == Configuration.WOStatus_Released && entity.GetAttributeValue<String>("trs_fillingnumber").Value != null && entity.GetAttributeValue<Double>("trs_printingsequence").Value != null)
                    //{
                    //    _DL_trs_quotation = new DL_trs_quotation();
                    //    //double printSequence = _DL_trs_quotation.trs_printingsequence + 1;

                    //    string branchNumber = string.Empty;
                    //    QueryExpression queryExpression = new QueryExpression(_DL_systemuser.EntityName);
                    //    queryExpression.ColumnSet.AddColumns("businessunitid");

                    //    FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                    //    filterExpression.AddCondition("systemuserid", ConditionOperator.Equal, (Guid)entity.Attributes["ownerid"]);

                    //    EntityCollection entityCollection = _DL_systemuser.Select(organizationService, queryExpression);
                    //    if (entityCollection.Entities.Count > 0)
                    //    {
                    //        QueryExpression queryExpression = new QueryExpression(_DL_businessunit.EntityName);
                    //        queryExpression.ColumnSet.AddColumns("name");

                    //        FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                    //        filterExpression.AddCondition("businessunitid", ConditionOperator.Equal, (Guid)entityCollection.Entities[0].Attributes["businessunitid"]);

                    //        EntityCollection entityCollectionBranch = _DL_businessunit.Select(organizationService, queryExpression);
                    //        if (entityCollectionBranch.Entities.Count > 0)
                    //        {
                    //            branchNumber = entityCollectionBranch.Entities[0].Attributes["name"];
                    //        }
                    //    }

                    //    string newRunningNumber = GenerateNewRunningNumber(organizationService, pluginExecutionContext, branchNumber, (DateTime)entity.Attributes["createdon"], (Double)_DL_trs_quotation.trs_printingsequence);
                    //    //_DL_trs_quotation.trs_printingsequence = printSequence;
                    //    _DL_trs_quotation.trs_fillingnumber = newRunningNumber;
                    //    _DL_trs_quotation.Update(organizationService, entity.Id);
                    //}
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Status_OnChange : " + ex.Message.ToString());
            }
        }
        #endregion
        #endregion

        #region public
        public string GenerateNewRunningNumber(IOrganizationService organizationService, IPluginExecutionContext pluginExecutionContext, string branchCode, DateTime createdDate, Double printSequence)
        {
            try
            {
                int year = createdDate.Year;

                string entityName = _DL_trs_quotation.EntityName + "F";

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

                    lastNumber++;
                    string newRunningNumber = branchCode;
                    newRunningNumber += "-" + year.ToString().Substring(2, 2);
                    newRunningNumber += "-" + lastNumber.ToString("00000000");
                    newRunningNumber += "-" + printSequence.ToString("000");

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

        //Deactivate a record
        public void DeactivateRecord(string entityName, IPluginExecutionContext pluginExecutionContext, IOrganizationService organizationService)
        {
            try
            {
                Entity record = (Entity)pluginExecutionContext.InputParameters["Target"];

                if (record.GetAttributeValue<bool>("trs_quotationdeal"))
                {
                    var cols = new ColumnSet(new[] { "statecode", "statuscode" });
                    Guid recordId = (Guid)((Entity)pluginExecutionContext.InputParameters["Target"]).Id;
                    //Check if it is Active or not
                    var entity = organizationService.Retrieve(entityName, recordId, cols);

                    if (entity != null && entity.GetAttributeValue<OptionSetValue>("statecode").Value == 0)
                    {
                        //StateCode = 1 and StatusCode = 2 for deactivating Account or Contact
                        SetStateRequest setStateRequest = new SetStateRequest()
                        {
                            EntityMoniker = new EntityReference
                            {
                                Id = recordId,
                                LogicalName = entityName,
                            },
                            State = new OptionSetValue(1),
                            Status = new OptionSetValue(167630002)
                        };
                        organizationService.Execute(setStateRequest);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
