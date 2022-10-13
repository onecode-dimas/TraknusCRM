using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TrakNusSparepartSystem.DataLayer;
using TrakNusSparepartSystem.Helper;

namespace TrakNusSparepartSystem.Plugins.BusinessLayer
{
    public class BL_tss_quotationpartreasondiscountpackage
    {
        #region Constants
        private const string _classname = "BL_tss_quotationpartreasondiscountpackage";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        BL_tss_quotationpartheader _BL_tss_quotationpartheader = new BL_tss_quotationpartheader();
        DL_tss_quotationpartheader _DL_tss_quotationpartheader = new DL_tss_quotationpartheader();
        DL_systemuser _DL_systemuser = new DL_systemuser();
        #endregion

        #region Properties
        private string _entityname = "tss_quotationpartreasondiscountpackage";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Quotation Part Lines - Reason Discount / Package";
        public string DisplayName
        {
            get { return _displayname; }
        }
        #endregion

        public void Form_OnUpdate_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExcecutionContext, Entity postImg, ITracingService tracingService)
        {
            try
            {
                bool flagResult = true;
                Entity entity = organizationService.Retrieve(pluginExcecutionContext.PrimaryEntityName, pluginExcecutionContext.PrimaryEntityId, new ColumnSet(true));
                if (entity.LogicalName == pluginExcecutionContext.PrimaryEntityName)
                {
                    if (entity.Attributes.Contains("tss_result"))
                    {
                        Guid quotationID = new Guid();

                        if (entity.Attributes.Contains("tss_quotationpartheader"))
                        {
                            //Guid quotationID = postImg.GetAttributeValue<EntityReference>("tss_quotationpartheader").Id;
                            quotationID = entity.GetAttributeValue<EntityReference>("tss_quotationpartheader").Id;

                            //throw new Exception(quotationID.ToString());
                        }
                        else
                        {
                            entity.Attributes.Add("tss_quotationpartheader", postImg.GetAttributeValue<EntityReference>("tss_quotationpartheader").Id);
                            quotationID = entity.GetAttributeValue<EntityReference>("tss_quotationpartheader").Id;
                        }

                        QueryExpression reasonDiscount = new QueryExpression(_entityname)
                        {
                            ColumnSet = new ColumnSet(true),
                            Criteria = new FilterExpression()
                            {
                                Conditions =
                                {
                                    new ConditionExpression("tss_quotationpartheader",ConditionOperator.Equal, quotationID)
                                }
                            }
                        };
                        var ecReasonDiscount = organizationService.RetrieveMultiple(reasonDiscount);
                        foreach (var item in ecReasonDiscount.Entities)
                        {
                            if (!item.Attributes.Contains("tss_result"))
                            {
                                flagResult = false;
                                break;
                            }
                        }

                        //var quotationHeaderEntityReference =
                        //    postImg.GetAttributeValue<EntityReference>("tss_quotationpartheader");
                        var quotationHeaderEntityReference =
                            entity.GetAttributeValue<EntityReference>("tss_quotationpartheader");
                        var quotationEntity = organizationService.Retrieve(quotationHeaderEntityReference.LogicalName,
                            quotationHeaderEntityReference.Id, new ColumnSet(true));

                        if (!quotationEntity.Contains("tss_requestdeliverydate"))throw new InvalidPluginExecutionException("Request Delivery Date is required before filling Discount Reason.");
                        
                        var isDiscount = quotationEntity.Contains("tss_underminimumprice") 
                                         && quotationEntity.GetAttributeValue<bool>("tss_underminimumprice") 
                                         && quotationEntity.Contains("tss_approvediscount") 
                                         && !quotationEntity.GetAttributeValue<bool>("tss_approvediscount");

                        //Competitor Check
                        var thisEntity = pluginExcecutionContext.InputParameters["Target"] as Entity;
                        if (thisEntity != null)
                        {

                            var competitorDiscountFilled = ecReasonDiscount.Entities.Where(ent =>
                                ent.GetAttributeValue<bool>("tss_iscompetitor") &&
                                ent.GetAttributeValue<bool>("tss_result"));

                            var competitorDiscountReason = ecReasonDiscount.Entities.Where(ent =>
                                ent.GetAttributeValue<bool>("tss_iscompetitor")
                                );
                            //First check, if this entity is not competitor discount, check competitor mark required to be filled first.
                            if (competitorDiscountReason.Any())
                            {
                                var reason = competitorDiscountReason.First();
                                if (reason.Id != thisEntity.Id)
                                {
                                    if (!reason.Contains("tss_result"))
                                    {
                                        throw new InvalidPluginExecutionException("You need to fill the competitor discount reason result first.");
                                    }
                                }
                            }

                            //if already filled and competitor is yes, check for detail lines.
                            if (competitorDiscountFilled.Any())
                            {
                                var reason = competitorDiscountFilled.First();
                                if (reason.Id != thisEntity.Id)
                                {
                                    QueryExpression competitorQuery = new QueryExpression("tss_quotationpartreasoncompetitor")
                                    {
                                        ColumnSet = new ColumnSet(true),
                                        Criteria =
                                        {
                                            Conditions =
                                            {
                                                new ConditionExpression("tss_reasondiscountpackage",ConditionOperator.Equal,reason.Id)
                                            }
                                        }
                                    };

                                    var competitorQueryResult = organizationService.RetrieveMultiple(competitorQuery);
                                    if (competitorQueryResult.Entities.Count == 0)throw new InvalidPluginExecutionException("Reason Discount for Competitor detected but no Competitor is filled. Please create competitor record in Competitor Discount Reason first.");
                                }
                            }
                        }

                        tracingService.Trace("Tracing Flag Result = " + flagResult.ToString() + " , isDiscount = " + isDiscount.ToString());

                        if (flagResult && isDiscount)
                        {
                            _BL_tss_quotationpartheader = new BL_tss_quotationpartheader();
                            _BL_tss_quotationpartheader.UpdateStatusReason(organizationService, quotationID,tracingService);
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate_PostOperation : " + ex.Message.ToString());
            }
        }

        public void Form_OnCreate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {

                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                String quotationNo = String.Empty;
                String noSequence = String.Empty;
                Guid quotationId = Guid.Empty;
                if (entity.LogicalName == _entityname)
                {
                    if (entity.Attributes.Contains("tss_quotationpartheader"))
                    {
                        quotationId = entity.GetAttributeValue<EntityReference>("tss_quotationpartheader").Id;
                        Entity quotationEntity = organizationService.Retrieve("tss_quotationpartheader", quotationId, new ColumnSet(true));
                        quotationNo = quotationEntity.GetAttributeValue<String>("tss_quotationnumber");
                    }

                    QueryExpression qe = new QueryExpression(_entityname)
                    {
                        ColumnSet = new ColumnSet(true),
                        Criteria = new FilterExpression()
                            {
                                Conditions =
                                {
                                    new ConditionExpression("tss_quotationpartheader",ConditionOperator.Equal,quotationId)
                                }
                            }
                    };
                    EntityCollection ec = organizationService.RetrieveMultiple(qe);

                    noSequence = (ec.Entities.Count + 1).ToString();


                    if (entity.Attributes.Contains("tss_name"))
                        entity.Attributes["tss_name"] = quotationNo + " - " + noSequence;
                    else
                        entity.Attributes.Add("tss_name", quotationNo + " - " + noSequence);

                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PreOperation: " + ex.Message.ToString());
            }
        }
    }
}
