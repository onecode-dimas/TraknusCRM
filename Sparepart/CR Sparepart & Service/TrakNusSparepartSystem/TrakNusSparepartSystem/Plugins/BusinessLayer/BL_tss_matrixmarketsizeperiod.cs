using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrakNusSparepartSystem.DataLayer;
using TrakNusSparepartSystem.Helper;

namespace TrakNusSparepartSystem.Plugins.BusinessLayer
{
    public class BL_tss_matrixmarketsizeperiod
    {
        #region Constants
        private const string _classname = "BL_tss_matrixmarketsizeperiod";
        private const string _entityname = "tss_matrixmarketsizeperiod";
        #endregion

        #region CREATE
        public void Form_OnCreate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];

                if (entity.LogicalName == _entityname)
                {
                    bool _isactive = entity.GetAttributeValue<bool>("tss_isactive");

                    if (_isactive)
                    {
                        FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
                        _filterexpression.AddCondition("tss_isactive", ConditionOperator.Equal, true);
                        _filterexpression.AddCondition("tss_matrixmarketsizeperiodid", ConditionOperator.NotEqual, entity.Id);

                        QueryExpression _queryexpression = new QueryExpression("tss_matrixmarketsizeperiod");
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria.AddFilter(_filterexpression);

                        EntityCollection _matrixmarketsizeperiodcollection = organizationService.RetrieveMultiple(_queryexpression);

                        if (_matrixmarketsizeperiodcollection.Entities.Count() > 0)
                        {
                            throw new InvalidPluginExecutionException("Matrix Market Size Period only allow 1 Period activated !");
                        }
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

        #endregion

        #region UPDATE
        public void Form_OnUpdate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];

                if (entity.LogicalName == _entityname)
                {
                    bool _isactive = entity.GetAttributeValue<bool>("tss_isactive");

                    if (_isactive)
                    {
                        FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
                        _filterexpression.AddCondition("tss_isactive", ConditionOperator.Equal, true);
                        _filterexpression.AddCondition("tss_matrixmarketsizeperiodid", ConditionOperator.NotEqual, entity.Id);

                        QueryExpression _queryexpression = new QueryExpression("tss_matrixmarketsizeperiod");
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria.AddFilter(_filterexpression);

                        EntityCollection _matrixmarketsizeperiodcollection = organizationService.RetrieveMultiple(_queryexpression);

                        if (_matrixmarketsizeperiodcollection.Entities.Count() > 0)
                        {
                            throw new InvalidPluginExecutionException("Matrix Market Size Period only allow 1 Period activated !");
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate_PreOperation : " + ex.Message.ToString());
            }
        }

        #endregion
    }
}
