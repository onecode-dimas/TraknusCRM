using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusSparepartSystem.Plugins.BusinessLayer
{
    public class BL_tss_salesorderpartsublines
    {
        private const int Status_BaseValue = 865920000;
        private const int Status_New = Status_BaseValue + 0;
        private const int Status_Cancel = Status_BaseValue + 1;
        private const int Status_Delivered = Status_BaseValue + 2;
        private const int Status_Invoiced = Status_BaseValue + 3;
        private const int Status_Paid = Status_BaseValue + 4;
        private const int Status_FullReserve = Status_BaseValue + 5;
        private const int Status_PartialReserve = Status_BaseValue + 6;
        private const int Status_FullSupply = Status_BaseValue + 7;
        private const int Status_PartialSupply = Status_BaseValue + 8;

        private const int HeaderStatus_BaseValue = 865920000;
        private const int HeaderStatus_New = HeaderStatus_BaseValue + 0;
        private const int HeaderStatus_SoProcessedToSap = HeaderStatus_BaseValue + 1;
        private const int HeaderStatus_Cancel = HeaderStatus_BaseValue + 2;
        private const int HeaderStatus_Delivered = HeaderStatus_BaseValue + 3;
        private const int HeaderStatus_Invoiced = HeaderStatus_BaseValue + 4;
        private const int HeaderStatus_Paid = HeaderStatus_BaseValue + 5;
        private const int HeaderStatus_FullReserve = HeaderStatus_BaseValue + 6;
        private const int HeaderStatus_PartialReserve = HeaderStatus_BaseValue + 7;
        private const int HeaderStatus_FullSupply = HeaderStatus_BaseValue + 8;
        private const int HeaderStatus_PartialSupply = HeaderStatus_BaseValue + 9;
        private const int HeaderStatus_Error = HeaderStatus_BaseValue + 10;

        public void CheckReservation(IOrganizationService organizationService,
            IPluginExecutionContext pluginExecutionContext, ITracingService tracing)
        {
            CheckReservation_LinesLevel(organizationService,pluginExecutionContext,tracing);
            CheckReservation_HeaderLevel(organizationService,pluginExecutionContext,tracing);
        }

        public void CheckSupply(IOrganizationService organizationService,
            IPluginExecutionContext pluginExecutionContext, ITracingService tracing)
        {
            CheckSupply_LinesLevel(organizationService, pluginExecutionContext, tracing);
            CheckSupply_HeaderLevel(organizationService, pluginExecutionContext, tracing);
        }

        private void CheckReservation_HeaderLevel(IOrganizationService organizationService,
            IPluginExecutionContext pluginExecutionContext, ITracingService tracing)
        {
            try
            {
                tracing.Trace("Entering Check Reservation : Header Level");

                var target = pluginExecutionContext.InputParameters["Target"] as Entity;

                if (target == null) return;

                var requeryEntity = organizationService.Retrieve(target.LogicalName, target.Id, new ColumnSet(true));

                if (!requeryEntity.Contains("tss_salesorderpartlines")) return;

                var salesOrderPartLinesEntityReference =
                    requeryEntity.GetAttributeValue<EntityReference>("tss_salesorderpartlines");

                var salesOrderPartLinesEntity = organizationService.Retrieve(
                    salesOrderPartLinesEntityReference.LogicalName, salesOrderPartLinesEntityReference.Id,
                    new ColumnSet(true));

                if (!salesOrderPartLinesEntity.Contains("tss_sopartheaderid")) return;

                var salesOrderPartHeaderEntityReference =
                    salesOrderPartLinesEntity.GetAttributeValue<EntityReference>("tss_sopartheaderid");

                //try to retrieve all lines from sales order header
                QueryExpression salesOrderPartLinesQueryExpression = new QueryExpression(salesOrderPartLinesEntityReference.LogicalName)
                {
                    ColumnSet = new ColumnSet(true),
                    Criteria =
                    {
                        Conditions =
                        {
                            new ConditionExpression("tss_sopartheaderid",ConditionOperator.Equal,salesOrderPartHeaderEntityReference.Id)
                        }
                    }
                };

                var queryResult = organizationService.RetrieveMultiple(salesOrderPartLinesQueryExpression);

                if (queryResult.Entities.Count > 0)
                {
                    OptionSetValue statusUpdate = null;

                    var isAllLinesFullReserved = queryResult.Entities.ToList().All(entity =>
                        entity.Contains("tss_status") && entity.GetAttributeValue<OptionSetValue>("tss_status").Value ==
                        Status_FullReserve);

                    var isAllLinesPartialReserved = queryResult.Entities.ToList().All(entity =>
                        entity.Contains("tss_status") && entity.GetAttributeValue<OptionSetValue>("tss_status").Value ==
                        Status_PartialReserve);

                    if (isAllLinesFullReserved)
                    {
                        statusUpdate = new OptionSetValue(HeaderStatus_FullReserve);
                    }

                    if (isAllLinesPartialReserved)
                    {
                        statusUpdate = new OptionSetValue(HeaderStatus_PartialReserve);
                    }

                    if (statusUpdate != null)
                    {
                        var updateEntity = new Entity(salesOrderPartHeaderEntityReference.LogicalName, salesOrderPartHeaderEntityReference.Id);
                        updateEntity["tss_statecode"] = statusUpdate;
                        organizationService.Update(updateEntity);
                    }
                }

                tracing.Trace("Exiting Check Reservation : Header Level");
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Error Occured in Check Reservation On Header Level. Technical Details:\r\n" + ex.ToString());
            }
        }

        private void CheckSupply_HeaderLevel(IOrganizationService organizationService,
            IPluginExecutionContext pluginExecutionContext, ITracingService tracing)
        {
            try
            {
                tracing.Trace("Entering Check Supply : Header Level");

                var target = pluginExecutionContext.InputParameters["Target"] as Entity;

                if (target == null) return;

                var requeryEntity = organizationService.Retrieve(target.LogicalName, target.Id, new ColumnSet(true));

                if (!requeryEntity.Contains("tss_salesorderpartlines")) return;

                var salesOrderPartLinesEntityReference =
                    requeryEntity.GetAttributeValue<EntityReference>("tss_salesorderpartlines");

                var salesOrderPartLinesEntity = organizationService.Retrieve(
                    salesOrderPartLinesEntityReference.LogicalName, salesOrderPartLinesEntityReference.Id,
                    new ColumnSet(true));

                if (!salesOrderPartLinesEntity.Contains("tss_sopartheaderid")) return;

                var salesOrderPartHeaderEntityReference =
                    salesOrderPartLinesEntity.GetAttributeValue<EntityReference>("tss_sopartheaderid");

                //try to retrieve all lines from sales order header
                QueryExpression salesOrderPartLinesQueryExpression = new QueryExpression(salesOrderPartLinesEntityReference.LogicalName)
                {
                    ColumnSet = new ColumnSet(true),
                    Criteria =
                    {
                        Conditions =
                        {
                            new ConditionExpression("tss_sopartheaderid",ConditionOperator.Equal,salesOrderPartHeaderEntityReference.Id)
                        }
                    }
                };

                var queryResult = organizationService.RetrieveMultiple(salesOrderPartLinesQueryExpression);

                if (queryResult.Entities.Count > 0)
                {
                    OptionSetValue statusUpdate = null;

                    var isAllLinesFullSupply = queryResult.Entities.ToList().All(entity =>
                        entity.Contains("tss_status") && entity.GetAttributeValue<OptionSetValue>("tss_status").Value ==
                        Status_FullSupply);

                    var isAllLinesPartialSupply = queryResult.Entities.ToList().All(entity =>
                        entity.Contains("tss_status") && entity.GetAttributeValue<OptionSetValue>("tss_status").Value ==
                        Status_PartialSupply);

                    if (isAllLinesFullSupply)
                    {
                        statusUpdate = new OptionSetValue(HeaderStatus_FullSupply);
                    }

                    if (isAllLinesPartialSupply)
                    {
                        statusUpdate = new OptionSetValue(HeaderStatus_PartialSupply);
                    }

                    if (statusUpdate != null)
                    {
                        var updateEntity = new Entity(salesOrderPartHeaderEntityReference.LogicalName, salesOrderPartHeaderEntityReference.Id);
                        updateEntity["tss_statecode"] = statusUpdate;
                        organizationService.Update(updateEntity);
                    }
                }

                tracing.Trace("Exiting Check Supply : Header Level");
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Error Occured in Check Supply On Header Level. Technical Details:\r\n" + ex.ToString());
            }
        }

        private void CheckReservation_LinesLevel(IOrganizationService organizationService,
            IPluginExecutionContext pluginExecutionContext, ITracingService tracing)
        {
            try
            {
                tracing.Trace("Entering Check Reservation : Lines Level");

                var target = pluginExecutionContext.InputParameters["Target"] as Entity;
                
                if (target == null) return;

                var requeryEntity = organizationService.Retrieve(target.LogicalName, target.Id, new ColumnSet(true));

                if (!requeryEntity.Contains("tss_salesorderpartlines")) return;

                var linesEntityReference = requeryEntity.GetAttributeValue<EntityReference>("tss_salesorderpartlines");
                var linesEntity = organizationService.Retrieve(linesEntityReference.LogicalName,
                    linesEntityReference.Id, new ColumnSet(true));


                var reservationQtySubLines = requeryEntity.GetAttributeValue<int>("tss_reservationqty");
                var quantityAvailableSubLines = requeryEntity.GetAttributeValue<int>("tss_qtyavailable");
                var quantityRequestLines = linesEntity.GetAttributeValue<int>("tss_qtyrequest");

                if (quantityRequestLines == reservationQtySubLines)
                {
                    //set full reserve
                    Entity updateEntity = new Entity(target.LogicalName,target.Id);
                    updateEntity["tss_status"] = new OptionSetValue(Status_FullReserve);
                    organizationService.Update(updateEntity);
                }
                else 
                if (reservationQtySubLines < quantityRequestLines && reservationQtySubLines > 0)
                {
                    //set partial reserve
                    Entity updateEntity = new Entity(target.LogicalName, target.Id);
                    updateEntity["tss_status"] = new OptionSetValue(Status_PartialReserve);
                    organizationService.Update(updateEntity);
                }
                tracing.Trace("Exiting Check Reservation : Lines Level");

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Error Occured in Check Reservation On Lines Level. Technical Details:\r\n" + ex.ToString());
            }
        }


        private void CheckSupply_LinesLevel(IOrganizationService organizationService,
            IPluginExecutionContext pluginExecutionContext, ITracingService tracing)
        {
            try
            {
                tracing.Trace("Entering Check Supply : Lines Level");

                var target = pluginExecutionContext.InputParameters["Target"] as Entity;

                if (target == null) return;

                var requeryEntity = organizationService.Retrieve(target.LogicalName, target.Id, new ColumnSet(true));

                if (!requeryEntity.Contains("tss_salesorderpartlines")) return;

                var linesEntityReference = requeryEntity.GetAttributeValue<EntityReference>("tss_salesorderpartlines");
                var linesEntity = organizationService.Retrieve(linesEntityReference.LogicalName,
                    linesEntityReference.Id, new ColumnSet(true));


                var reservationQtySubLines = requeryEntity.GetAttributeValue<int>("tss_reservationqty");
                var quantityAvailableSubLines = requeryEntity.GetAttributeValue<int>("tss_qtyavailable");
                var quantityRequestLines = linesEntity.GetAttributeValue<int>("tss_qtyrequest");

                if (quantityRequestLines == quantityAvailableSubLines)
                {
                    //set full supply
                    Entity updateEntity = new Entity(target.LogicalName, target.Id);
                    updateEntity["tss_status"] = new OptionSetValue(Status_FullSupply);
                    organizationService.Update(updateEntity);
                }
                else
                    if (quantityAvailableSubLines < quantityRequestLines && quantityAvailableSubLines > 0)
                {
                    //set partial supply
                    Entity updateEntity = new Entity(target.LogicalName, target.Id);
                    updateEntity["tss_status"] = new OptionSetValue(Status_PartialSupply);
                    organizationService.Update(updateEntity);
                }
                tracing.Trace("Exiting Check Supply : Lines Level");

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Error Occured in Check Supply On Lines Level. Technical Details:\r\n" + ex.ToString());
            }
        }
    }
}
