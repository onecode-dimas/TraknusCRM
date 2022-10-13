using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using TrakNusRapidService.DataLayer;
using TrakNusRapidService.Helper;

namespace TrakNusRapidService.Plugins.BusinessLayer
{
    class BL_trs_mechanicmonthlypoint
    {
        #region Constants
        private const string _classname = "BL_trs_mechanicmonthlypoint";
        #endregion

        #region Depedencies
        private DL_trs_mechanicmonthlypoint _DL_trs_mechanicmonthlypoint = new DL_trs_mechanicmonthlypoint();
        private DL_equipment _DL_equipment = new DL_equipment();
        #endregion

        #region Privates
        private void SendtoMobile(IOrganizationService organizationService, Guid id)
        {
            try
            {
                FMobile _fmobile = new FMobile(organizationService);
                _fmobile.SendMechanicMonthlyPoint(organizationService, id);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".SendtoMobile : " + ex.Message);
            }
        }

        private void SummarizeMechanicPoint(IOrganizationService organizationService, Guid mechanicId)
        {
            try
            {
                FetchExpression fetchExpression = new FetchExpression(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' aggregate='true'>
                                                                        <entity name='trs_mechanicmonthlypoint'>
                                                                        <attribute name='trs_point' alias='trs_point_sum' aggregate='sum' />
                                                                        <attribute name='trs_mechanicid' alias='trs_mechanicid' groupby='true' />
                                                                        <filter type='and'>
                                                                            <condition attribute='trs_mechanicid' operator='eq' value='{" + mechanicId.ToString() + @"}' />
                                                                            <condition attribute='statecode' operator='eq' value='0' />
                                                                            <condition attribute='statuscode' operator='eq' value='1' />
                                                                        </filter>
                                                                        </entity>
                                                                    </fetch>");
                EntityCollection entityCollection = organizationService.RetrieveMultiple(fetchExpression);
                if (entityCollection.Entities.Count > 0)
                {
                    _DL_equipment = new DL_equipment();
                    _DL_equipment.trs_repairtimehour = (decimal)entityCollection.Entities[0].GetAttributeValue<AliasedValue>("trs_point").Value;
                    _DL_equipment.Update(organizationService, mechanicId);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".SummarizeMechanicPoint : " + ex.Message);
            }
        }
        #endregion

        #region Events
        #region Forms
        public void Form_OnCreate(IOrganizationService organizationService, IPluginExecutionContext pluginExcecutionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExcecutionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_trs_mechanicmonthlypoint.EntityName)
                {
                    SummarizeMechanicPoint(organizationService, entity.Id);
                    SendtoMobile(organizationService, entity.Id);
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

        public void Form_OnUpdate(IOrganizationService organizationService, IPluginExecutionContext pluginExcecutionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExcecutionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_trs_mechanicmonthlypoint.EntityName)
                {
                    SummarizeMechanicPoint(organizationService, entity.Id);
                    SendtoMobile(organizationService, entity.Id);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate : " + ex.Message.ToString());
            }
        }
        #endregion
        #endregion
    }
}
