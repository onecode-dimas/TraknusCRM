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
  public  class BL_tss_marketsizeconsumpAging
    {
        #region Constants
        private const string _classname = "BL_tss_marketsizepartconsumpaging";
        private const string _entityname = "tss_marketsizepartconsumpaging";
        #endregion

        #region Depedencies
        private BL_trs_runningnumber _BL_trs_runningnumber = new BL_trs_runningnumber();
        #endregion


        #region Form Event

        public void Form_OnCreate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                if (entity.LogicalName == _entityname)
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
                                        new ConditionExpression("tss_aging", ConditionOperator.Equal, entity.Attributes["tss_aging"]),
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
                        throw new InvalidPluginExecutionException("There is same Model, Part Number & Aging for any Market Size Part Consumpt Data.");
                    }
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
                                        new ConditionExpression("tss_partnumber", ConditionOperator.Equal,entity.Attributes.Contains("tss_partnumber")? ((EntityReference)entity.Attributes["tss_partnumber"]).Id:((EntityReference)entityToUpdate.Attributes["tss_partnumber"]).Id),
                                        new ConditionExpression("tss_aging", ConditionOperator.Equal,entity.Attributes.Contains("tss_aging")? entity.Attributes["tss_aging"]:entityToUpdate.Attributes["tss_aging"]),
                                       new ConditionExpression("tss_model", ConditionOperator.Equal,entity.Attributes.Contains("tss_model")? ((EntityReference)entity.Attributes["tss_model"]).Id:((EntityReference)entityToUpdate.Attributes["tss_model"]).Id)
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
                        throw new InvalidPluginExecutionException("There is same Model, Part Number & Aging for any Market Size Part Consumpt Data.");
                    }
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
