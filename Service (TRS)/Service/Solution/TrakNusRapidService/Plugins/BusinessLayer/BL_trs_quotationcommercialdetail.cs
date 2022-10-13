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
    class BL_trs_quotationcommercialdetail
    {
        #region Constants
        private const string _classname = "BL_trs_quotationcommercialdetail";
        #endregion

        #region Depedencies
        private DL_trs_quotationcommercialdetail _DL_trs_quotationcommercialdetail = new DL_trs_quotationcommercialdetail();
        private DL_trs_quotationpartdetail _DL_trs_quotationpartdetail = new DL_trs_quotationpartdetail();
        private DL_trs_tasklistdetailpart _DL_trs_tasklistdetailpart = new DL_trs_tasklistdetailpart();
        private DL_trs_masterpart _DL_trs_masterpart = new DL_trs_masterpart();
        #endregion

        #region Privates
        private void FillCommercialDetailParts(IOrganizationService organizationService, Guid id, Guid quotationId, Guid taskListDetailId)
        {
            try
            {
                Entity ePart = new Entity();

                QueryExpression queryExpression = new QueryExpression(_DL_trs_tasklistdetailpart.EntityName);
                queryExpression.ColumnSet = new ColumnSet(true);
                queryExpression.Criteria.AddCondition("trs_tasklistdetailid", ConditionOperator.Equal, taskListDetailId);
                EntityCollection ecolTaskListDetailParts = _DL_trs_tasklistdetailpart.Select(organizationService, queryExpression);

                foreach (Entity eTaskListDetailParts in ecolTaskListDetailParts.Entities)
                {
                    ePart = _DL_trs_masterpart.Select(organizationService, eTaskListDetailParts.GetAttributeValue<EntityReference>("trs_masterpartid").Id);

                    _DL_trs_quotationpartdetail = new DL_trs_quotationpartdetail();
                    _DL_trs_quotationpartdetail.trs_quotation = quotationId;
                    _DL_trs_quotationpartdetail.trs_commercialdetailid = id;
                    _DL_trs_quotationpartdetail.trs_partnumber = ePart.Id;
                    _DL_trs_quotationpartdetail.trs_partdescription = ePart.GetAttributeValue<string>("trs_partdescription");
                    _DL_trs_quotationpartdetail.trs_quantity = eTaskListDetailParts.GetAttributeValue<int>("trs_quantity");
                    _DL_trs_quotationpartdetail.Insert(organizationService);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".FillCommercialDetailParts : " + ex.Message);
            }
        }

        private void FillCommercialDetailTools(IOrganizationService organizationService, Guid id, Guid taskListDetailId)
        {
            try
            {
                List<Guid> toolGroupsId = new List<Guid>();
                FetchExpression fetchExpression = new FetchExpression(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' aggregate='true'>
                                                                            <entity name='trs_trs_commercialtask_trs_toolsgroup'>
                                                                            <attribute name='trs_toolsgroupid' alias='trs_toolsgroupid' groupby='true' />
                                                                            <filter type='and'>
                                                                                <condition attribute='trs_commercialtaskid' operator='eq' value='{" + taskListDetailId.ToString() + @"}' />
                                                                            </filter>
                                                                            </entity>
                                                                        </fetch>");
                EntityCollection entityCollection = organizationService.RetrieveMultiple(fetchExpression);
                foreach (Entity entity in entityCollection.Entities)
                {
                    toolGroupsId.Add((Guid)entity.GetAttributeValue<AliasedValue>("trs_toolsgroupid").Value);
                }
                _DL_trs_quotationcommercialdetail.AssociateToolGroups(organizationService, id, toolGroupsId);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".FillCommercialDetailTools : " + ex.Message);
            }
        }
        #endregion

        #region Events
        public void Form_OnCreate(IOrganizationService organizationService, IPluginExecutionContext pluginExecutionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExecutionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_trs_quotationcommercialdetail.EntityName)
                {
                    FillCommercialDetailParts(organizationService
                        , entity.Id
                        , entity.GetAttributeValue<EntityReference>("trs_quotation").Id
                        , entity.GetAttributeValue<EntityReference>("trs_tasklistdetail").Id);
                    FillCommercialDetailTools(organizationService
                        , entity.Id
                        , entity.GetAttributeValue<EntityReference>("trs_tasklistdetail").Id);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}
