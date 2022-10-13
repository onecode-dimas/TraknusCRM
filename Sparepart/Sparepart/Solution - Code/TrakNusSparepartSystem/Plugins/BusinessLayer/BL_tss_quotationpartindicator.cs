using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.Plugins.BusinessLayer
{
    public class BL_tss_quotationpartindicator
    {
        private const string _classname = "BL_tss_quotationpartindicator";

        public void Form_OnUpdate_TotalConfidenceLevel_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext, ITracingService tracer)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                if (entity.LogicalName == pluginExceptionContext.PrimaryEntityName)
                {
                    //get value of indicator when result =yess
                    var indicator = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));
                    if (indicator.Attributes.Contains("tss_quotationpartheader"))
                    {
                        int totalConfidenceLevel = 0;

                        QueryExpression Query = new QueryExpression(pluginExceptionContext.PrimaryEntityName);
                        Query.ColumnSet = new ColumnSet(true);
                        Query.Criteria.AddCondition("tss_quotationpartheader", ConditionOperator.Equal, indicator.GetAttributeValue<EntityReference>("tss_quotationpartheader").Id);
                        Query.Criteria.AddCondition("tss_result", ConditionOperator.Equal, true);
                        EntityCollection Items = organizationService.RetrieveMultiple(Query);
                        if (Items.Entities.Count > 0)
                        {
                            foreach (Entity item in Items.Entities)
                            {
                                //if (item.Attributes.Contains("tss_result"))
                                //{
                                var indicatorValue = item.GetAttributeValue<int>("tss_value");

                                /*if (item.GetAttributeValue<bool>("tss_result") == true)
                                {
                                    totalConfidenceLevel += indicatorValue;
                                }
                                else
                                {
                                    totalConfidenceLevel -= indicatorValue;
                                }*/

                                totalConfidenceLevel += indicatorValue;

                                //get value of header
                                //var quotationHeader = organizationService.Retrieve("tss_quotationpartheader", indicator.GetAttributeValue<EntityReference>("tss_quotationpartheader").Id, new ColumnSet(true));
                                //if (quotationHeader != null && quotationHeader.Attributes.Count > 0)
                                //{
                                //    //update quotation part header where id =  idicator 
                                //    Entity entityHeader = new Entity("tss_quotationpartheader");
                                //    entityHeader.Id = quotationHeader.Id;
                                //    entityHeader.Attributes["tss_totalconfidencelevel"] = totalConfidenceLevel;
                                //    organizationService.Update(entityHeader);
                                //}
                                //}
                            }
                        }
                        else
                        {
                            totalConfidenceLevel = 0;
                        }

                        //get value of header
                        var quotationHeader = organizationService.Retrieve("tss_quotationpartheader", indicator.GetAttributeValue<EntityReference>("tss_quotationpartheader").Id, new ColumnSet(true));
                        if (quotationHeader != null && quotationHeader.Attributes.Count > 0)
                        {
                            //update quotation part header where id =  idicator 
                            Entity entityHeader = new Entity("tss_quotationpartheader");
                            entityHeader.Id = quotationHeader.Id;
                            entityHeader.Attributes["tss_totalconfidencelevel"] = totalConfidenceLevel;
                            organizationService.Update(entityHeader);
                        }

                        /*var indicatorValue = indicator.GetAttributeValue<int>("tss_value");

                        //get total confidence level from quotation part header
                        var quotationHeader = organizationService.Retrieve("tss_quotationpartheader", indicator.GetAttributeValue<EntityReference>("tss_quotationpartheader").Id, new ColumnSet(true));
                        totalConfidenceLevel = quotationHeader.GetAttributeValue<int>("tss_totalconfidencelevel");

                        if (indicator.GetAttributeValue<bool>("tss_result") == true)  //if result is true
                        {
                            totalConfidenceLevel += indicatorValue;
                        }
                        else
                        {
                            totalConfidenceLevel -= indicatorValue;
                        }

                        //update quotation part header where id =  idicator 
                        if (quotationHeader != null && quotationHeader.Attributes.Count > 0)
                        {
                            Entity entityHeader = new Entity("tss_quotationpartheader");
                            entityHeader.Id = quotationHeader.Id;
                            entityHeader.Attributes["tss_totalconfidencelevel"] = totalConfidenceLevel;
                            organizationService.Update(entityHeader);
                        }*/
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate_TotalConfidenceLevel_PostOperation : " + ex.Message.ToString());
            }
        }
    }
}
