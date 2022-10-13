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

namespace TrakNusSparepartSystem.Workflow.BusinessLayer
{
    public class BL_tss_quotation
    {
        #region Dependencies
        private DL_team _DL_team = new DL_team();
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        private DL_trs_discountapproval _DL_trs_discountapproval = new DL_trs_discountapproval();
        private DL_trs_quotation _DL_trs_quotation = new DL_trs_quotation();
        #endregion

        private string _classname = "DL_trs_quotation";

        public void AssignToPdh_OnClick(IOrganizationService organizationService, Guid id, string entityName, ITracingService trace) //, IPluginExecutionContext pluginExceptionContext
        {
            try
            {
                var getQuotation= organizationService.Retrieve(entityName, id, new ColumnSet(true));
                if (getQuotation.Attributes.Contains("trs_quotationnumber"))
                {
                    _DL_trs_quotation.tss_statusassignquo = 865920000;//update to be Assigned
                    _DL_trs_quotation.UpdateStatusAssignQuo(organizationService, id);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".AssignToPdh_OnClick : " + ex.Message.ToString());
            }
        }

        public void CreateQuotation_CloneEntity(IOrganizationService organizationService, Guid id, string entityName, ITracingService trace) //, IPluginExecutionContext pluginExceptionContext
        {
            try
            {
                var getQuotation = organizationService.Retrieve(entityName, id, new ColumnSet(true));  //quoation (service)
                if (getQuotation.Attributes.Contains("trs_quotationnumber"))
                {
                    if (getQuotation.GetAttributeValue<string>("trs_quotationnumber") != string.Empty)
                    {
                        //if (getQuotation.GetAttributeValue<DateTime>("trs_estimationcloseddate") != null)
                        //    _DL_trs_quotation.trs_estimationcloseddate = getQuotation.GetAttributeValue<DateTime>("trs_estimationcloseddate");
                        //else
                        //    _DL_trs_quotation.trs_estimationcloseddate = null;
                        //_DL_trs_quotation.trs_estimationcloseddate = getQuotation.GetAttributeValue<DateTime>("trs_estimationcloseddate");
                        //_DL_trs_quotation.CopyEntityQuotation(organizationService, entityName, id);

                        /*if (IsComponentHasData(organizationService, getQuotation.Id) == true)
                        {
                            CopyEntityQuotation(organizationService, entityName, id);
                        }
                        else
                        {
                            throw new Exception("Cannot Assign to PDH, Component Has No Data");
                        }*/

                        CopyEntityQuotation(organizationService, entityName, id);

                        
                    }
                    else
                    {
                        throw new Exception("Can't Copy Data to Quotation Part");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".CopyEntityQuotation : " + ex.Message.ToString());
            }
        }

        public bool IsComponentHasData(IOrganizationService organizationService, Guid Id)
        {
            QueryExpression QueryComponent = new QueryExpression("trs_quotationpartssummary");  //component from quotation
            QueryComponent.ColumnSet = new ColumnSet(true);
            QueryComponent.Criteria.AddCondition("trs_quotationnumber", ConditionOperator.Equal, Id);

            EntityCollection CompoentnItems = organizationService.RetrieveMultiple(QueryComponent);
            if (CompoentnItems.Entities.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CopyEntityQuotation(IOrganizationService organizationService, string entityName, Guid id)
        {
            try
            {
                var getQuotation = organizationService.Retrieve(entityName, id, new ColumnSet(true));

                Entity target = new Entity(getQuotation.LogicalName);
                if (getQuotation != null && getQuotation.Attributes.Count > 0)
                {
                    //CloneRecord(getQuotation, target);  //clone header

                    #region insert quoation(service) to quoation part
                    Entity entityQuoationPart = new Entity("tss_quotationpartheader");
                    entityQuoationPart.Attributes["tss_quotationserviceno"] = new EntityReference("trs_quotation", getQuotation.Id);
                    entityQuoationPart.Attributes["tss_servicequoteowner"] = getQuotation.GetAttributeValue<EntityReference>("ownerid");
                    entityQuoationPart.Attributes["tss_estimationclosedate"] = getQuotation.GetAttributeValue<DateTime?>("trs_estimationcloseddate"); //allow null to insert

                    /*if (getQuotation.GetAttributeValue<DateTime>("trs_estimationcloseddate") != (DateTime?)null)
                        entityQuoationPart.Attributes["tss_estimationclosedate"] = getQuotation.GetAttributeValue<DateTime>("trs_estimationcloseddate");
                    else
                        entityQuoationPart.Attributes["tss_estimationclosedate"] = (DateTime?)null;  */

                    //entityQuoationPart.Attributes["tss_estimationclosedate"] = (getQuotation.GetAttributeValue<DateTime>("trs_estimationcloseddate")==null ? null : getQuotation.GetAttributeValue<DateTime>("trs_estimationcloseddate"));
                    //if (_trs_estimationcloseddate) { entityQuoationPart.Attributes["tss_estimationclosedate"] = _trs_estimationcloseddate_value; }

                    entityQuoationPart.Attributes["tss_customer"] = getQuotation.GetAttributeValue<EntityReference>("trs_customer");    //lookup
                    entityQuoationPart.Attributes["tss_sourcetype"] = new OptionSetValue(865920002);  //option set to Service
                    entityQuoationPart.Attributes["tss_branch"] = getQuotation.GetAttributeValue<EntityReference>("trs_branch");      //lookup
                    entityQuoationPart.Attributes["tss_contact"] = getQuotation.GetAttributeValue<EntityReference>("trs_contact");  //lookup
                    entityQuoationPart.Attributes["tss_unit"] = getQuotation.GetAttributeValue<EntityReference>("trs_unit");  //lookup
                    entityQuoationPart.Attributes["tss_currency"] = getQuotation.GetAttributeValue<EntityReference>("transactioncurrencyid");  //lookup
                    //entityQuoationPart.Attributes["tss_pss"] = getQuotation.GetAttributeValue<EntityReference>("tss_pss");  //lookup   krn sudah d eksusi oleh plugin PostQuotationPartHeaderCreat
                    entityQuoationPart.Attributes["tss_statuscode"] = new OptionSetValue(865920000); //status reason draft
                    entityQuoationPart.Attributes["tss_statusreason"] = new OptionSetValue(865920000); //status open
                    entityQuoationPart.Attributes["tss_revision"] = null;   //whole number
                    Guid QuoationPartId = organizationService.Create(entityQuoationPart);
                    #endregion

                    #region update tss_quotationpartno in quotation service from quotation part
                    //get tss_quotationpartno from quotatin service header
                    target.Id = id;
                    target.Attributes["tss_quotationpartno"] = new EntityReference("tss_quotationpartheader", QuoationPartId);
                    organizationService.Update(target);
                    #endregion


                    #region Component to Quotation Part Lines
                    decimal totalPrice = 0;
                    QueryExpression QueryComponent = new QueryExpression("trs_quotationpartssummary");  //component from quotation
                    QueryComponent.ColumnSet = new ColumnSet(true);
                    QueryComponent.Criteria.AddCondition("trs_quotationnumber", ConditionOperator.Equal, getQuotation.Id);

                    EntityCollection CompoentnItems = organizationService.RetrieveMultiple(QueryComponent);
                    if (CompoentnItems.Entities.Count > 0)
                    {
                        foreach (Entity sItem in CompoentnItems.Entities)
                        {
                            //Entity tItem = new Entity(sItem.LogicalName);
                            //CloneRecord(sItem, tItem);   //clone line

                            Money diskonSatuan;
                            decimal discountAmount = sItem.GetAttributeValue<Money>("trs_discountamount").Value;
                            int taskListQuantity = sItem.GetAttributeValue<int>("trs_tasklistquantity");
                            int manualQuantity = sItem.GetAttributeValue<int>("trs_manualquantity");
                            int quantity;

                            if (manualQuantity != null)
                                quantity = manualQuantity;
                            else if (taskListQuantity != null && manualQuantity == null)
                                quantity = taskListQuantity;
                            else
                                quantity = 0;

                            //if discount by mount
                            if (sItem.GetAttributeValue<bool>("trs_discountby") == true) //value: amount (1)
                                diskonSatuan = new Money(discountAmount / quantity);
                            else
                                diskonSatuan = sItem.GetAttributeValue<Money>("trs_discountamount");

                            //get tss_partdescription from partnumber
                            var PartMaster = organizationService.Retrieve("trs_masterpart", sItem.GetAttributeValue<EntityReference>("trs_partnumber").Id, new ColumnSet(true));
                            var Description = PartMaster.GetAttributeValue<string>("trs_partdescription");

                            //insert to quotation part lines
                            Entity PartLineEntity = new Entity("tss_quotationpartlines");
                            PartLineEntity.Attributes["tss_quotationpartheader"] = new EntityReference("tss_quotationpartheader", QuoationPartId); //get from new data quotation part header
                            //PartLineEntity.Attributes["tss_partnumber"] = sItem.GetAttributeValue<EntityReference>("trs_masterpart").Id;
                            PartLineEntity.Attributes["tss_partnumber"] = sItem.GetAttributeValue<EntityReference>("trs_partnumber");
                            PartLineEntity.Attributes["tss_partdescription"] = Description;  //get description from part master
                            PartLineEntity.Attributes["tss_quantity"] = sItem.GetAttributeValue<int>("trs_manualquantity");
                            PartLineEntity.Attributes["tss_price"] = sItem.GetAttributeValue<Money>("trs_price");
                            PartLineEntity.Attributes["tss_discountby"] = sItem.GetAttributeValue<bool>("trs_discountby");
                            //PartLineEntity.Attributes["tss_discountamount"] = sItem.GetAttributeValue<Money>("trs_discountamount");
                            PartLineEntity.Attributes["tss_unitgroup"] = PartMaster.GetAttributeValue<EntityReference>("tss_unitgroup");
                            PartLineEntity.Attributes["tss_sourcetype"] = new OptionSetValue(865920005); //quote from service
                            PartLineEntity.Attributes["tss_discountamount"] = diskonSatuan;
                            PartLineEntity.Attributes["tss_discountpercent"] = sItem.GetAttributeValue<decimal>("trs_discountpercent");
                            PartLineEntity.Attributes["tss_totalprice"] = sItem.GetAttributeValue<Money>("trs_totalprice");

                            //set item number
                            int defaultNumber = 10;
                            int max = 0;
                            var context = new OrganizationServiceContext(organizationService);
                            var lines = (from c in context.CreateQuery("tss_quotationpartlines")
                                         where c.GetAttributeValue<EntityReference>("tss_quotationpartheader").Id == QuoationPartId
                                         select c).ToList();

                            for (int i = 0; i < lines.Count; i++)
                            {
                                if (lines[i].Attributes.Contains("tss_itemnumber") && lines[i].Attributes["tss_itemnumber"] != null)
                                {
                                    if (lines[i].GetAttributeValue<int>("tss_itemnumber") > max) max = lines[i].GetAttributeValue<int>("tss_itemnumber");
                                }
                            }
                            if (max >= defaultNumber)
                            {
                                PartLineEntity.Attributes["tss_itemnumber"] = max + 10;
                            }
                            else
                            {
                                PartLineEntity.Attributes["tss_itemnumber"] = 10;
                            }

                            organizationService.Create(PartLineEntity);

                            //sum of total price to quotation part total price
                            totalPrice += sItem.GetAttributeValue<Money>("trs_totalprice").Value;
                            entityQuoationPart.Id = QuoationPartId;
                            entityQuoationPart.Attributes["tss_totalprice"] = new Money(totalPrice);
                            organizationService.Update(entityQuoationPart);
                        }
                    }
                    #endregion


                    #region Service to Quotation Part Lines - Service
                    QueryExpression QueryService = new QueryExpression("trs_quotationcommercialheader");  //component from quotation
                    QueryService.ColumnSet = new ColumnSet(true);
                    QueryService.Criteria.AddCondition("trs_quotationid", ConditionOperator.Equal, getQuotation.Id);

                    EntityCollection ServiceItems = organizationService.RetrieveMultiple(QueryService);
                    if (ServiceItems.Entities.Count > 0)
                    {
                        foreach (Entity sItem in ServiceItems.Entities)
                        {
                            //insert to Quotation Part Lines - Service
                            Entity ServiceEntity = new Entity("tss_quotationpartlinesservice");
                            //CloneRecord(sItem, ServiceEntity);   //clone line service
                            ServiceEntity.Attributes["tss_quotationpartheader"] = new EntityReference("tss_quotationpartheader", QuoationPartId);
                            ServiceEntity.Attributes["tss_taskheader"] = sItem.GetAttributeValue<EntityReference>("trs_taskheader");
                            ServiceEntity.Attributes["tss_pricetype"] = sItem.GetAttributeValue<OptionSetValue>("trs_pricetype");
                            ServiceEntity.Attributes["tss_commercialheader"] = sItem.GetAttributeValue<string>("trs_commercialheader");
                            ServiceEntity.Attributes["tss_price"] = sItem.GetAttributeValue<Money>("trs_price");
                            ServiceEntity.Attributes["tss_discountby"] = sItem.GetAttributeValue<bool>("trs_discountby");
                            ServiceEntity.Attributes["tss_discountpercent"] = sItem.GetAttributeValue<decimal>("trs_discountpercent");
                            ServiceEntity.Attributes["tss_discountamount"] = sItem.GetAttributeValue<Money>("trs_discountamount");
                            ServiceEntity.Attributes["tss_totalprice"] = sItem.GetAttributeValue<Money>("trs_totalprice");
                            organizationService.Create(ServiceEntity);
                        }
                    }
                    #endregion


                    #region Support Material to Quotation Part Lines - Support Material
                    QueryExpression QueryMaterial = new QueryExpression("trs_quotationsupportingmaterial");  //component from quotation
                    QueryMaterial.ColumnSet = new ColumnSet(true);
                    QueryMaterial.Criteria.AddCondition("trs_quotationid", ConditionOperator.Equal, getQuotation.Id);

                    EntityCollection MaterialItems = organizationService.RetrieveMultiple(QueryMaterial);
                    if (MaterialItems.Entities.Count > 0)
                    {
                        foreach (Entity sItem in MaterialItems.Entities)
                        {
                            //insert to Quotation Part Lines - support material 
                            Entity MaterialEntity = new Entity("tss_quotationpartlinessupportingmaterial");
                            MaterialEntity.Attributes["tss_quotationpartheader"] = new EntityReference("tss_quotationpartheader", QuoationPartId);
                            MaterialEntity.Attributes["tss_type"] = sItem.GetAttributeValue<bool>("trs_type");
                            MaterialEntity.Attributes["tss_supportingmaterial"] = sItem.GetAttributeValue<string>("trs_supportingmaterial");
                            MaterialEntity.Attributes["tss_price"] = sItem.GetAttributeValue<Money>("trs_price");
                            MaterialEntity.Attributes["tss_quantity"] = sItem.GetAttributeValue<string>("trs_quantity");
                            MaterialEntity.Attributes["tss_totalprice"] = sItem.GetAttributeValue<Money>("trs_totalprice");
                            organizationService.Create(MaterialEntity);
                        }
                    }
                    #endregion

                    #region Insert ALL Sales Indicator to Quoatation Part Indicator
                    QueryExpression QueryIndicator = new QueryExpression("tss_indicator"); //entity sales indicator
                    QueryIndicator.ColumnSet = new ColumnSet(true);

                    EntityCollection IndicatorItems = organizationService.RetrieveMultiple(QueryIndicator);
                    if (IndicatorItems.Entities.Count > 0)
                    {
                        foreach (Entity indicator in IndicatorItems.Entities)
                        {
                            Entity entity = new Entity("tss_quotationpartindicator");
                            entity.Attributes["tss_quotationpartheader"] = new EntityReference("tss_quotationpartheader", QuoationPartId);
                            entity.Attributes["tss_indicator"] = new EntityReference("tss_indicator", indicator.Id); //lookup sales indicator
                            entity.Attributes["tss_result"] = null;   //two option
                            entity.Attributes["tss_value"] = indicator.GetAttributeValue<int>("tss_value");   //whole number
                            organizationService.Create(entity);
                        }
                    } 
                    #endregion

                    #region Insert all data from master reason to Quotation Part - Reason Discount/ Package (set result Blank)
                    QueryExpression QueryReason = new QueryExpression("tss_reason");
                    QueryReason.ColumnSet = new ColumnSet(true);

                    EntityCollection ReasonItems = organizationService.RetrieveMultiple(QueryReason);
                    if (ReasonItems.Entities.Count > 0)
                    {
                        foreach (Entity reason in ReasonItems.Entities)
                        {
                            Entity entity = new Entity("tss_quotationpartreasondiscountpackage");
                            entity.Attributes["tss_quotationpartheader"] = new EntityReference("tss_quotationpartheader", QuoationPartId);
                            //entity.Attributes["tss_reason"] = reason.GetAttributeValue<EntityReference>("tss_reason");   
                            entity.Attributes["tss_reason"] = new EntityReference("tss_reason", reason.Id);
                            entity.Attributes["tss_result"] = null;   //two option
                            entity.Attributes["tss_iscompetitor"] = reason.GetAttributeValue<bool>("tss_iscompetitor");
                            organizationService.Create(entity);
                        }
                    }  
                    #endregion

                    #region assign pdh
                    if (getQuotation.Attributes.Contains("tss_pdh") && getQuotation.Attributes["tss_pdh"] != null)
                    {
                        var pdhId = getQuotation.GetAttributeValue<EntityReference>("tss_pdh").Id;
                        AssignRequest assign = new AssignRequest
                        {
                            Assignee = new EntityReference("systemuser", pdhId),
                            Target = new EntityReference("tss_quotationpartheader", QuoationPartId)
                        };
                        organizationService.Execute(assign);

                        Entity ent = new Entity("tss_approverlist");
                        ent.Attributes["tss_approver"] = new EntityReference("systemuser", pdhId);
                        ent.Attributes["tss_quotationpartheaderid"] = new EntityReference("tss_quotationpartheader", QuoationPartId);
                        organizationService.Create(ent);
                    }

                    //if (getQuotation.Attributes.Contains("trs_branch") && getQuotation.Attributes["trs_branch"] != null)
                    //{
                    //    var context = new OrganizationServiceContext(organizationService);
                    //    var pdh = (from c in context.CreateQuery("systemuser")
                    //               where c.GetAttributeValue<string>("title") == "PDH"
                    //               where c.GetAttributeValue<EntityReference>("businessunitid").Id == getQuotation.GetAttributeValue<EntityReference>("trs_branch").Id
                    //               select c).ToList();

                    //    for (int i = 0; i < pdh.Count; i++)
                    //    {
                    //        if (i == 0)
                    //        {
                    //            AssignRequest assign = new AssignRequest
                    //            {
                    //                Assignee = new EntityReference("systemuser", pdh[0].Id),
                    //                Target = new EntityReference("tss_quotationpartheader", QuoationPartId)
                    //            };
                    //            organizationService.Execute(assign);
                    //        }
                    //        else
                    //        {
                    //            ShareRecords sh = new ShareRecords();
                    //            sh.ShareRecord(organizationService, entityQuoationPart, new Entity { LogicalName = "systemuser", Id = pdh[i].Id });
                    //        }

                    //        Entity ent = new Entity("tss_approverlist");
                    //        ent.Attributes["tss_approver"] = new EntityReference("systemuser", pdh[i].Id);
                    //        ent.Attributes["tss_quotationpartheaderid"] = new EntityReference("tss_quotationpartheader", QuoationPartId);
                    //        organizationService.Create(ent);
                    //    }
                    //}
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw new Exception(".CopyEntityQuotation : " + ex.Message.ToString());
            }
        }

        private static void CloneRecord(Entity sourcEntity, Entity target)
        {
            /*string[] attributesToExclude = new string[]
            {
                    "modifiedon",
                    "createdon",
                    "statecode",
                    //"tss_quotationid",
                    "tss_statusreason",
                    "tss_statuscode",
                    "tss_revision",
                    "tss_packageno",
                    "tss_package",
                    "tss_packagesname",
                    "tss_packagedescription",
                    "tss_totalexpectedpackageamount",
                    "tss_approvepackage",
                    //"tss_finalprice",
                    "tss_totalfinalprice"
            }; */

            string[] attributesToExcludeService = new string[]
            {
                    "modifiedon",
                    "createdon, trs_totalrtg, transactioncurrencyid,trs_discountheader, trs_supportingmaterialoption"
            };


            foreach (string attrName in sourcEntity.Attributes.Keys)
            {
                if (!attributesToExcludeService.Contains(attrName) && attrName.ToLower() != sourcEntity.LogicalName.ToLower() + "id")
                {
                    target.Attributes.Add(attrName, sourcEntity[attrName]);
                }
            }
        }

    }
}
