using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TrakNusSparepartSystem.DataLayer;
using TrakNusSparepartSystem.Helper;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Web;
using System.Collections;

namespace TrakNusSparepartSystem.Plugins.BusinessLayer
{
    public class BL_tss_marketsizeconsump
    {
        #region Constants
        private const string _classname = "BL_tss_marketsizepartconsump";
        private const string _entityname = "tss_marketsizepartconsump";
        #endregion

        #region Depedencies
        private BL_trs_runningnumber _BL_trs_runningnumber = new BL_trs_runningnumber();
        #endregion

        public bool IsNumeric(string value)
        {
            return value.All(char.IsNumber);
        }

        #region Form Event

        public void Form_OnCreate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];

                if (entity.LogicalName == _entityname)
                {
                    string _typepm = entity.GetAttributeValue<string>("tss_typepm");

                    if (IsNumeric(_typepm))
                    {
                        QueryExpression qExpression = new QueryExpression(_entityname)
                        {
                            ColumnSet = new ColumnSet(true),
                            Criteria =
                        {
                            Filters =
                            {
                                new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("tss_partnumber", ConditionOperator.Equal, ((EntityReference)entity.Attributes["tss_partnumber"]).Id),
                                        new ConditionExpression("tss_typepm", ConditionOperator.Equal, entity.Attributes["tss_typepm"]),
                                       new ConditionExpression("tss_model", ConditionOperator.Equal, ((EntityReference)entity.Attributes["tss_model"]).Id)
                                    }
                                },
                                //new FilterExpression
                                //{
                                //    FilterOperator = LogicalOperator.Or,
                                //    Conditions =
                                //    {
                                //        new ConditionExpression("tss_status", ConditionOperator.Equal, ((EntityReference)entity.Attributes["tss_customer"]).Id),
                                //    }
                                //}
                            }
                        }
                        };

                        EntityCollection ENExpression = organizationService.RetrieveMultiple(qExpression);

                        if (ENExpression.Entities.Count != 0)
                        {
                            throw new InvalidPluginExecutionException("There is same Model, Part Number, & Type PM for any Market Size Part Consump Data.");
                        }
                    }
                    else
                        throw new InvalidPluginExecutionException("Type PM Part Consump value must NUMERIC !");

                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PreOperation : " + ex.Message.ToString());
            }
        }

        public void Form_OnUpdate_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                Entity entityToUpdate = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));

                if (entity.LogicalName == _entityname)
                {
                    string _typepm = entity.Attributes.Contains("tss_typepm") ? entity.GetAttributeValue<string>("tss_typepm") : entityToUpdate.GetAttributeValue<string>("tss_typepm");

                    if (IsNumeric(_typepm))
                    {
                        QueryExpression qExpression = new QueryExpression(_entityname)
                        {

                            ColumnSet = new ColumnSet(true),
                            Criteria =
                        {
                            Filters =
                            {
                                new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("tss_partnumber", ConditionOperator.Equal, entity.Attributes.Contains("tss_partnumber")? ((EntityReference)entity.Attributes["tss_partnumber"]).Id:((EntityReference)entityToUpdate.Attributes["tss_partnumber"]).Id),
                                        new ConditionExpression("tss_typepm", ConditionOperator.Equal,entity.Attributes.Contains("tss_typepm")? entity.Attributes["tss_typepm"]: entityToUpdate.Attributes["tss_typepm"]),
                                        new ConditionExpression("tss_model", ConditionOperator.Equal,entity.Attributes.Contains("tss_model")? ((EntityReference)entity.Attributes["tss_model"]).Id: ((EntityReference)entityToUpdate.Attributes["tss_model"]).Id)
                                    }
                                },
                                    //new FilterExpression
                                    //{
                                    //    FilterOperator = LogicalOperator.Or,
                                    //    Conditions =
                                    //    {
                                    //        new ConditionExpression("tss_status", ConditionOperator.Equal, ((EntityReference)entity.Attributes["tss_customer"]).Id),
                                    //    }
                                    //}
                                }
                            }
                        };

                        EntityCollection ENExpression = organizationService.RetrieveMultiple(qExpression);

                        if (ENExpression.Entities.Count != 0)
                        {
                            throw new InvalidPluginExecutionException("There is same Model, Part Number, & Type PM for any Market Size Part Consump Data.");
                        }
                    }
                    else
                        throw new InvalidPluginExecutionException("Type PM Part Consump value must NUMERIC !");
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate_PostOperation: " + ex.Message.ToString());
            }
        }

        #endregion
    }
}
