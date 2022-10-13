using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using TrakNusRapidService.DataLayer;

namespace TrakNusRapidService.Plugins.BusinessLayer
{
    class BL_task
    {
        #region Constants
        private const string _classname = "BL_task";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_task _DL_task = new DL_task();
        private DL_trs_commercialdetail _DL_trs_commercialdetail = new DL_trs_commercialdetail();
        #endregion

        #region Privates
        #endregion

        #region Events
        #region Forms
        public void Form_OnCreate(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_task.EntityName)
                {
                    if (entity.Attributes.Contains("trs_fromquotation") && entity.GetAttributeValue<bool>("trs_fromquotation"))
                    {
                        return;
                    }
                    else
                    {
                        if (entity.Attributes.Contains("trs_tasklistheader") && entity.Attributes["trs_tasklistheader"] != null)
                        {
                            if (!entity.Attributes.Contains("trs_operationid"))
                                throw new InvalidPluginExecutionException("Can not found WO Id.");

                            EntityReference erTaskListHeader = (EntityReference)entity.Attributes["trs_tasklistheader"];
                            decimal totalRTG = 0;

                            QueryExpression qe = new QueryExpression();
                            qe.EntityName = "trs_commercialtask";
                            qe.ColumnSet.AllColumns = true;

                            ConditionExpression con1 = new ConditionExpression();
                            con1.AttributeName = "trs_tasklistheaderid";
                            con1.Operator = ConditionOperator.Equal;
                            con1.Values.Add(erTaskListHeader.Id);

                            FilterExpression Mainfilter = new FilterExpression();
                            Mainfilter.FilterOperator = LogicalOperator.And;
                            Mainfilter.Conditions.Add(con1);

                            qe.Criteria.AddFilter(Mainfilter);

                            EntityCollection ecCommercialTask = new DL_trs_commercialtask().Select(organizationService, qe);

                            foreach (Entity enCommercialTask in ecCommercialTask.Entities)
                            {
                                decimal rtg = 0;
                                _DL_trs_commercialdetail = new DL_trs_commercialdetail();
                                _DL_trs_commercialdetail.CommercialHeaderId = entity.Id;
                                _DL_trs_commercialdetail.trs_workorder = entity.GetAttributeValue<EntityReference>("trs_operationid").Id;
                                _DL_trs_commercialdetail.trs_commercialtask = enCommercialTask.Id;
                                if (enCommercialTask.Contains("trs_taskcode"))
                                    _DL_trs_commercialdetail.TaskCode = enCommercialTask["trs_taskcode"].ToString();
                                if (enCommercialTask.Contains("trs_mechanicgrade"))
                                    _DL_trs_commercialdetail.trs_mechanicgrade = ((EntityReference)enCommercialTask["trs_mechanicgrade"]).Id;
                                if (enCommercialTask.Contains("trs_taskname"))
                                    _DL_trs_commercialdetail.Taskname = ((EntityReference)enCommercialTask["trs_taskname"]).Id;
                                if (enCommercialTask.Contains("trs_rtg"))
                                {
                                    rtg = (decimal)enCommercialTask["trs_rtg"];
                                    _DL_trs_commercialdetail.trs_rtg = rtg;
                                }
                                totalRTG += rtg;

                                _DL_trs_commercialdetail.trs_fromquotation = entity.GetAttributeValue<bool>("trs_fromquotation");
                                _DL_trs_commercialdetail.Insert(organizationService);
                            }
                            _DL_task.trs_totalrtg = totalRTG;
                            _DL_task.Update(organizationService, entity.Id);
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
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate : " + ex.Message.ToString());
            }
        }
        #endregion

        #region Fields
        #endregion
        #endregion
    }
}
