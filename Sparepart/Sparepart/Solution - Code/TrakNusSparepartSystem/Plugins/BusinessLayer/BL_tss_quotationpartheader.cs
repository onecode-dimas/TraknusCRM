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
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Crm.Sdk.Messages;

namespace TrakNusSparepartSystem.Plugins.BusinessLayer
{
    public class BL_tss_quotationpartheader
    {
        #region Constants
        private const string _classname = "BL_tss_quotationpartheader";
        private const int STATUSCODE_INPROGRESS = 865920001;
        private const int STATUSREASON_WAITINGAPPROVALDISCOUNT = 865920002;
        private const int STATUSCODE_DRAFT = 865920000;
        private const int STATUSREASON_OPEN = 865920000;
        private const int STATUSCODE_APPROVE = 865920002;
        private const int STATUSREASON_FINALAPPROVE = 865920006;
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_tss_quotationpartheader _DL_tss_quotationpartheader = new DL_tss_quotationpartheader();
        private DL_tss_prospectpartheader _DL_tss_prospectpartheader = new DL_tss_prospectpartheader();
        private DL_tss_rating _DL_tss_rating = new DL_tss_rating();
        private BL_trs_runningnumber _BL_trs_runningnumber = new BL_trs_runningnumber();
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        private DL_tss_approverlist _DL_tss_approverlist = new DL_tss_approverlist();
        private ShareRecords _ShareRecords = new ShareRecords();
        #endregion

        #region Properties
        private string _entityname = "tss_quotationpartheader";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Quotation Part";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _tss_priceamount = false;
        private Money _tss_priceamount_value;
        public decimal tss_priceamount
        {
            get { return _tss_priceamount ? _tss_priceamount_value.Value : 0; }
            set { _tss_priceamount = true; _tss_priceamount_value = new Money(value); }
        }

        private bool _tss_totalprice = false;
        private Money _tss_totalprice_value;
        public decimal tss_totalprice
        {
            get { return _tss_totalprice ? _tss_totalprice_value.Value : 0; }
            set { _tss_totalprice = true; _tss_totalprice_value = new Money(value); }
        }

        private bool _tss_totalfinalprice = false;
        private Money _tss_totalfinalprice_value;
        public decimal tss_totalfinalprice
        {
            get { return _tss_totalfinalprice ? _tss_totalfinalprice_value.Value : 0; }
            set { _tss_totalfinalprice = true; _tss_totalfinalprice_value = new Money(value); }
        }

        //two option
        private bool _tss_underminimumprice = false;
        private bool _tss_underminimumprice_value;
        public bool tss_underminimumprice
        {
            get { return _tss_underminimumprice ? _tss_underminimumprice_value : false; }
            set { _tss_underminimumprice = true; _tss_underminimumprice_value = value; }
        }
        #endregion

        #region Forms Event
        public void Form_OnCreate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_tss_quotationpartheader.EntityName)
                {

                    DateTime createdOn = DateTime.Now.ToLocalTime();
                    //string categoryCode = "03";
                    //Generate New Running Number
                    string newRunningNumber = _BL_trs_runningnumber.GenerateNewRunningNumber(
                        organizationService, pluginExceptionContext, _DL_tss_quotationpartheader.EntityName, createdOn);
                    if (entity.Attributes.Contains("tss_quotationnumber"))
                        entity.Attributes["tss_quotationnumber"] = newRunningNumber;
                    else
                        entity.Attributes.Add("tss_quotationnumber", newRunningNumber);
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

        public void Form_OnUpdate_QuotationNumber_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_tss_quotationpartheader.EntityName)
                {
                    DateTime createdOn = DateTime.Now.ToLocalTime();
                    //string categoryCode = "03";
                    //Generate New Running Number
                    string newRunningNumber = _BL_trs_runningnumber.GenerateNewRunningNumber(
                        organizationService, pluginExceptionContext, _DL_tss_quotationpartheader.EntityName, createdOn);
                    if (entity.Attributes.Contains("tss_quotationnumber"))
                        entity.Attributes["tss_quotationnumber"] = newRunningNumber;
                    else
                        entity.Attributes.Add("tss_quotationnumber", newRunningNumber);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate_QuotationNumber_PostOperation : " + ex.Message.ToString());
            }
        }
        #endregion

        #region Generate Running Number Of QuotationId
        public void Form_OnCreate_GenerateQuotationId_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext, ITracingService tracer)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                if (entity.LogicalName == "tss_quotationpartheader")
                {
                    DL_tss_runningnumberid number = new DL_tss_runningnumberid();
                    string newRunningIdString = number.newRunningIdString(organizationService, _DL_tss_quotationpartheader.EntityName, entity.Id, tracer);
                    entity.Attributes["tss_quotationid"] = newRunningIdString;
                    organizationService.Update(entity);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_GenerateQuotationId_PostOperation : " + ex.Message.ToString());
            }
        }
        #endregion

        #region Public
        //Update Total Price (Prospect Part) from Prospect Part Lines
        public void Form_OnCreate_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                bool flagMinimumPrice = false;
                Entity entity = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));
                if (entity.LogicalName == pluginExceptionContext.PrimaryEntityName)
                {
                    if (entity.Attributes.Contains("tss_totalprice"))
                    {
                        //tss_totalamount = entity.GetAttributeValue<Money>("tss_priceamount").Value;
                        Guid quotationID = entity.GetAttributeValue<EntityReference>("tss_quotationpartheader").Id;
                        string prospectName = entity.GetAttributeValue<EntityReference>("tss_quotationpartheader").LogicalName;


                        QueryExpression qQuotationPartLines = new QueryExpression("tss_quotationpartlines")
                        {
                            ColumnSet = new ColumnSet(true),
                            Criteria = new FilterExpression()
                            {
                                Conditions =
                                {
                                    new ConditionExpression("tss_quotationpartheader",ConditionOperator.Equal,quotationID)
                                }
                            }
                        };
                        var ecQuotationPartLines = organizationService.RetrieveMultiple(qQuotationPartLines);
                        foreach (var item in ecQuotationPartLines.Entities)
                        {
                            if (item.GetAttributeValue<bool>("tss_underminimumprice")) flagMinimumPrice = true;
                            tss_priceamount = item.GetAttributeValue<Money>("tss_totalprice").Value;
                            tss_totalprice += tss_priceamount;
                        }
                        var totalFinalPrice = ecQuotationPartLines.Entities
                            .Where(ent => ent.Contains("tss_totalfinalprice")).Select(ent =>
                                ent.GetAttributeValue<Money>("tss_totalfinalprice").Value).Sum();

                        Entity pros = new Entity(prospectName);
                        pros.Id = quotationID;
                        tss_underminimumprice = flagMinimumPrice;
                        if (_tss_underminimumprice) pros.Attributes["tss_underminimumprice"] = _tss_underminimumprice_value;
                        pros.Attributes["tss_totalprice"] = _tss_totalprice_value;
                        pros.Attributes["tss_totalfinalprice"] = new Money(totalFinalPrice);
                        organizationService.Update(pros);
                    }
                }
                else
                {
                    throw new Exception("Wrong entity " + entity.LogicalName + " on Quotation Part Lines !");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PostOperation: " + ex.Message.ToString());
            }
        }

        public void UpdateCloseAsWon(IOrganizationService organizationService, IPluginExecutionContext pluginExcecutionContext, Entity postImg)
        {
            try
            {
                Entity beforeUpdateEntity = organizationService.Retrieve("tss_quotationpartheader", postImg.Id, new ColumnSet(true));

                if (postImg.Attributes.Contains("tss_salesorderno") && postImg.Attributes["tss_salesorderno"] != null
                    && beforeUpdateEntity.GetAttributeValue<OptionSetValue>("tss_statuscode").Value == STATUSCODE_APPROVE
                    && beforeUpdateEntity.GetAttributeValue<OptionSetValue>("tss_statusreason").Value == STATUSREASON_FINALAPPROVE)
                {
                    //update status to won
                    Entity ent = new Entity("tss_quotationpartheader");
                    ent.Id = postImg.Id;
                    ent.Attributes["tss_statuscode"] = new OptionSetValue(865920005);
                    ent.Attributes["tss_statusreason"] = new OptionSetValue(865920008);
                    organizationService.Update(ent);

                    //update all quotation di prospect yang sama, apabila ada jadi close & lost
                    var currEntity = pluginExcecutionContext.InputParameters["Target"] as Entity;
                    if (currEntity == null) throw new Exception("Current Entity is null");

                    if (currEntity.Contains("tss_prospectlink"))
                    {
                        QueryExpression queryQuotationInSameProspect = new QueryExpression("tss_quotationpartheader")
                        {
                            ColumnSet = new ColumnSet(true),
                            Criteria =
                            {
                                Conditions =
                                {
                                    new ConditionExpression("tss_prospectlink",ConditionOperator.Equal,currEntity.GetAttributeValue<EntityReference>("tss_prospectlink").Id)
                                }
                            }
                        };

                        var resultQuotationInSameProspect =
                            organizationService.RetrieveMultiple(queryQuotationInSameProspect);

                        foreach (var quotationEntity in resultQuotationInSameProspect.Entities)
                        {
                            if (quotationEntity.Id != currEntity.Id)
                            {
                                //Update as closed & lost if not this quotation entity.
                                Entity updateEntity = new Entity(quotationEntity.LogicalName, quotationEntity.Id);
                                updateEntity["tss_statuscode"] = new OptionSetValue(865920004);
                                updateEntity["tss_statusreason"] = new OptionSetValue(865920007);
                                organizationService.Update(updateEntity);
                            }
                        }
                    }

                    //update quotation in so
                    var context = new OrganizationServiceContext(organizationService);
                    var salesorder = (from c in context.CreateQuery("tss_sopartheader")
                                      where c.GetAttributeValue<Guid>("tss_sopartheaderid") == postImg.GetAttributeValue<EntityReference>("tss_salesorderno").Id
                                      select c).ToList();

                    if (salesorder.Count > 0)
                    {
                        Entity entSO = new Entity("tss_sopartheader");
                        entSO.Id = salesorder[0].Id;
                        if (postImg.Attributes.Contains("tss_pss") && postImg.Attributes["tss_pss"] != null)
                        {
                            entSO.Attributes["tss_pss"] = postImg.Attributes["tss_pss"];
                        }
                        if (postImg.Attributes.Contains("tss_requestdeliverydate") && postImg.Attributes["tss_requestdeliverydate"] != null)
                        {
                            entSO.Attributes["tss_requestdeliverydate"] = postImg.Attributes["tss_requestdeliverydate"];
                        }
                        entSO.Attributes["tss_quotationlink"] = new EntityReference("tss_quotationpartheader", postImg.Id);
                        organizationService.Update(entSO);
                    }


                    var salesorderlines = (from c in context.CreateQuery("tss_sopartlines")
                                           where c.GetAttributeValue<EntityReference>("tss_sopartheaderid").Id == postImg.GetAttributeValue<EntityReference>("tss_salesorderno").Id
                                           select c).ToList();

                    foreach (var x in salesorderlines)
                    {
                        if (x.Attributes.Contains("tss_itemnumber") && x.Attributes["tss_itemnumber"] != null)
                        {
                            var quotationLines = (from c in context.CreateQuery("tss_quotationpartlines")
                                                  where c.GetAttributeValue<EntityReference>("tss_quotationpartheader").Id == postImg.Id
                                                  where c.GetAttributeValue<int>("tss_itemnumber") == x.GetAttributeValue<int>("tss_itemnumber")
                                                  select c).ToList();

                            if (quotationLines.Count > 0)
                            {
                                if (quotationLines[0].Attributes.Contains("tss_sourcetype") && quotationLines[0].Attributes["tss_sourcetype"] != null)
                                {
                                    int type = quotationLines[0].GetAttributeValue<OptionSetValue>("tss_sourcetype").Value;
                                    int solType = 0;
                                    if (type == 865920000) solType = 865920003;
                                    else if (type == 865920001) solType = 865920000;
                                    else if (type == 865920002) solType = 865920001;
                                    else if (type == 865920003) solType = 865920002;
                                    else if (type == 865920005) solType = 865920005;
                                    if (solType != 0)
                                    {
                                        Entity entSOL = new Entity("tss_sopartlines");
                                        entSOL.Id = x.Id;
                                        entSOL.Attributes["tss_sourcetype"] = new OptionSetValue(solType);
                                        organizationService.Update(entSOL);
                                    }
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".UpdateCloseAsWon: " + ex.Message.ToString());
            }
        }

        public void AssignPSS(IOrganizationService organizationService, IPluginExecutionContext pluginExcecutionContext, Entity postImg)
        {
            try
            {
                if (postImg.Attributes.Contains("tss_pss") && postImg.Attributes["tss_pss"] != null)
                {
                    //delete approver list
                    var context = new OrganizationServiceContext(organizationService);
                    //var approverlist = (from c in context.CreateQuery("tss_approverlist")
                    //                    where c.GetAttributeValue<EntityReference>("tss_quotationpartheaderid").Id == postImg.Id
                    //                    select c).ToList();
                    //for (int i = 0; i < approverlist.Count; i++)
                    //{
                    //    organizationService.Delete("tss_approverlist", approverlist[i].Id);
                    //}

                    //assign to pss
                    AssignRequest assign = new AssignRequest
                    {
                        Assignee = new EntityReference("systemuser", postImg.GetAttributeValue<EntityReference>("tss_pss").Id),
                        Target = new EntityReference("tss_quotationpartheader", postImg.Id)
                    };
                    organizationService.Execute(assign);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".AssignPSS: " + ex.Message.ToString());
            }
        }

        public void Form_OnUpdate_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext, Entity postImg)
        {
            try
            {
                bool flagMinimumPrice = false;
                Entity entity = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));
                if (entity.LogicalName == pluginExceptionContext.PrimaryEntityName)
                {
                    if (entity.Attributes.Contains("tss_totalprice"))
                    {
                        //tss_totalamount = entity.GetAttributeValue<Money>("tss_priceamount").Value;
                        Guid quotationID = entity.GetAttributeValue<EntityReference>("tss_quotationpartheader").Id;
                        string prospectName = entity.GetAttributeValue<EntityReference>("tss_quotationpartheader").LogicalName;
                        bool flagCheckFinalPrice = true;

                        QueryExpression qQuotationPartLines = new QueryExpression("tss_quotationpartlines")
                        {
                            ColumnSet = new ColumnSet(true),
                            Criteria = new FilterExpression()
                            {
                                Conditions =
                                {
                                    new ConditionExpression("tss_quotationpartheader",ConditionOperator.Equal,quotationID)
                                }
                            }
                        };
                        var ecQuotationPartLines = organizationService.RetrieveMultiple(qQuotationPartLines);
                        flagMinimumPrice = postImg.GetAttributeValue<bool>("tss_underminimumprice");
                        foreach (var item in ecQuotationPartLines.Entities)
                        {
                            if (item.GetAttributeValue<bool>("tss_underminimumprice") && !flagMinimumPrice) flagMinimumPrice = true;
                            tss_priceamount = item.GetAttributeValue<Money>("tss_totalprice").Value;
                            tss_totalprice += tss_priceamount;
                        }
                        foreach (var item in ecQuotationPartLines.Entities)
                        {
                            if (!item.Contains("tss_totalfinalprice"))
                            {
                                flagCheckFinalPrice = false;
                                break;
                            }
                            if (item.Contains("tss_totalfinalprice"))
                            {
                                if (item.GetAttributeValue<Money>("tss_totalfinalprice").Value == 0)
                                {
                                    flagCheckFinalPrice = false;
                                    break;
                                }
                            }
                        }

                        var totalFinalPrice = ecQuotationPartLines.Entities
                            .Where(ent => ent.Contains("tss_totalfinalprice")).Select(ent =>
                                ent.GetAttributeValue<Money>("tss_totalfinalprice").Value).Sum();

                        Entity pros = new Entity(prospectName);
                        pros.Id = quotationID;
                        tss_underminimumprice = flagMinimumPrice;
                        //throw new InvalidPluginExecutionException("UMP: " + tss_underminimumprice);
                        if (_tss_underminimumprice) pros.Attributes["tss_underminimumprice"] = _tss_underminimumprice_value;
                        if (flagCheckFinalPrice)
                        {
                            pros.Attributes["tss_totalprice"] = _tss_totalprice_value;
                            pros["tss_totalfinalprice"] = new Money(totalFinalPrice);
                        }
                        organizationService.Update(pros);

                    }
                }
                else
                {
                    throw new Exception("Wrong entity " + entity.LogicalName + " on Quotation Part Lines !");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate_PostOperation: " + ex.Message.ToString());
            }
        }

        public void Form_OnDelete_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext, Entity preImg)
        {
            try
            {
                int countIMP = 0;
                bool flagMinimumPrice = true;
                Entity entity = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));
                if (entity.LogicalName == pluginExceptionContext.PrimaryEntityName)
                {
                    if (entity.Attributes.Contains("tss_totalprice"))
                    {
                        //tss_totalamount = entity.GetAttributeValue<Money>("tss_priceamount").Value;
                        Guid quotationID = entity.GetAttributeValue<EntityReference>("tss_quotationpartheader").Id;
                        string quotationName = entity.GetAttributeValue<EntityReference>("tss_quotationpartheader").LogicalName;
                        Entity QuotationHeader = _DL_tss_quotationpartheader.Select(organizationService, quotationID);

                        if (QuotationHeader.GetAttributeValue<OptionSetValue>("tss_statuscode").Value != STATUSCODE_DRAFT && QuotationHeader.GetAttributeValue<OptionSetValue>("tss_statusreason").Value != STATUSREASON_OPEN)
                        {
                            bool UMP = preImg.GetAttributeValue<bool>("tss_underminimumprice");
                            //throw new InvalidPluginExecutionException("UMP: " + UMP);
                            QueryExpression qQuotationPartLines = new QueryExpression("tss_quotationpartlines")
                            {
                                ColumnSet = new ColumnSet(true),
                                Criteria = new FilterExpression()
                                {
                                    Conditions =
                                {
                                    new ConditionExpression("tss_quotationpartheader",ConditionOperator.Equal,quotationID)
                                }
                                }
                            };
                            var ecQuotationPartLines = organizationService.RetrieveMultiple(qQuotationPartLines);
                            foreach (var item in ecQuotationPartLines.Entities)
                            {
                                if (item.GetAttributeValue<bool>("tss_underminimumprice")) countIMP++;
                                tss_priceamount = item.GetAttributeValue<Money>("tss_totalprice").Value;
                                tss_totalprice += tss_priceamount;
                            }
                            tss_totalprice -= entity.GetAttributeValue<Money>("tss_totalprice").Value;

                            var totalFinalPrice = ecQuotationPartLines.Entities
                                .Where(ent => ent.Contains("tss_totalfinalprice")).Select(ent =>
                                    ent.GetAttributeValue<Money>("tss_totalfinalprice").Value).Sum() - entity.GetAttributeValue<Money>("tss_totalfinalprice").Value;

                            if (countIMP == 1 && UMP) flagMinimumPrice = false;
                            if (countIMP < 1) flagMinimumPrice = false;
                            Entity pros = new Entity(quotationName);
                            pros.Id = quotationID;
                            tss_underminimumprice = flagMinimumPrice;
                            pros.Attributes["tss_underminimumprice"] = _tss_underminimumprice_value;
                            pros.Attributes["tss_totalprice"] = _tss_totalprice_value;
                            pros.Attributes["tss_totalfinalprice"] = new Money(totalFinalPrice);
                            organizationService.Update(pros);
                        }
                    }
                }
                else
                {
                    throw new Exception("Wrong entity " + entity.LogicalName + " on Quotation Part Lines !");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnDelete_PreOperation: " + ex.Message.ToString());
            }
        }

        //public void Form_OnUpdateReqDelivery_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext, bool underPrice)
        public void Form_OnUpdateReqDelivery_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity QuotationPartHeader = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));
                if (QuotationPartHeader.LogicalName == pluginExceptionContext.PrimaryEntityName)
                {
                    if (QuotationPartHeader.Attributes.Contains("tss_requestdeliverydate"))
                    {
                        QueryExpression Query = new QueryExpression("tss_quotationpartlines");
                        Query.ColumnSet = new ColumnSet(true);
                        Query.Criteria.AddCondition("tss_quotationpartheader", ConditionOperator.Equal, QuotationPartHeader.Id);
                        EntityCollection Items = organizationService.RetrieveMultiple(Query);
                        if (Items.Entities.Count > 0)
                        {
                            foreach (var item in Items.Entities)
                            {
                                Entity entity = new Entity("tss_quotationpartlines");
                                entity.Id = item.Id;
                                entity.Attributes["tss_reqdeliverydate"] = QuotationPartHeader.GetAttributeValue<DateTime>("tss_requestdeliverydate");
                                organizationService.Update(entity);
                            }
                        }

                    }
                    //else if (QuotationPartHeader.Attributes.Contains("tss_underminimumprice"))
                    //{
                    //    if (underPrice)
                    //    {
                    //        _DL_tss_quotationpartheader = new DL_tss_quotationpartheader();
                    //        _DL_tss_quotationpartheader.tss_statusreason = STATUSREASON_WAITINGAPPROVALDISCOUNT;
                    //        _DL_tss_quotationpartheader.tss_quotationstatus = STATUSCODE_INPROGRESS;
                    //        _DL_tss_quotationpartheader.UpdateQuotationFromProspect(organizationService, QuotationPartHeader.Id);
                    //    }                        
                    //}
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdateReqDelivery_PostOperation: " + ex.Message.ToString());
            }
        }

        public void UpdatePSS(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext, Entity postImg)
        {
            try
            {
                EntityReference pss = postImg.GetAttributeValue<EntityReference>("tss_pss");
                EntityReference serviceRef = postImg.GetAttributeValue<EntityReference>("tss_quotationserviceno");
                var getQuotPartHeader = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));
                if (getQuotPartHeader.Contains("tss_pss") && getQuotPartHeader.Contains("tss_quotationserviceno"))
                {
                    if (postImg.GetAttributeValue<EntityReference>("tss_pss") != null && postImg.GetAttributeValue<EntityReference>("tss_quotationserviceno") != null)
                    {
                        Guid user = getQuotPartHeader.GetAttributeValue<EntityReference>("ownerid").Id;
                        string quotSender = getQuotPartHeader.GetAttributeValue<EntityReference>("ownerid").Name;
                        string quotNumber = getQuotPartHeader.GetAttributeValue<string>("tss_quotationnumber");
                        if (getQuotPartHeader.GetAttributeValue<int>("tss_revision") == 0)
                        {
                            var service = organizationService.Retrieve("trs_quotation", serviceRef.Id, new ColumnSet(true));
                            Entity quotationService = new Entity("trs_quotation");
                            quotationService.Id = service.Id;
                            if (pss.Id != Guid.Empty) quotationService["tss_pss"] = pss;
                            organizationService.Update(quotationService);
                        }
                        else
                        {
                            if (pss.Id != Guid.Empty && pss != null)
                            {
                                Guid receiver = pss.Id;
                                String receivername = pss.Name;
                                Guid sender = GetSystemUserByFullname(organizationService, "Admin CRM").Id;

                                //HELPER EMAIL
                                HelperFunction helper = new HelperFunction();
                                String strPathAndQuery = HttpContext.Current.Request.Url.PathAndQuery;
                                String strUrl = HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");
                                string objecttypecode = string.Empty;
                                helper.GetObjectTypeCode(organizationService, "tss_quotationpartheader", out objecttypecode);
                                strUrl += "TraktorNusantara";
                                strUrl += "/main.aspx?etc=" + objecttypecode;
                                strUrl += "?id=%7b" + getQuotPartHeader.Id + "%7d&pagetype=entityrecord";
                                strUrl = "<a href='" + strUrl + "'>" + quotNumber + "</a>";

                                var subject = @"PSS has been assign in Quotation Part No. : " + quotNumber;
                                var bodyTemplate = @"Dear Mr/Ms " + receivername + @",<br/><br/>
                                You have been assigned as PSS on Quotation Part No. : " + quotNumber + @".<br/><br/>
                                Please feel free to check Quotation Part from " + quotSender + @".<br/><br/>
                                If you want to check, please click this Quotation Part Number: " + strUrl + @".<br/>
                                <br/><br/>
                                Thank you,<br/>
                                Admin CRM";
                                var email = SendEmailToPSS(sender, receiver, user, organizationService, subject, bodyTemplate);
                                var emailGuid = organizationService.Create(email);
                                //var emailAgent = new Helper.EmailAgent();
                                //emailAgent.SendEmail(organizationService, emailGuid);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".UpdatePSS : " + ex.Message.ToString());
            }
        }

        public void sendEmailAfterFillTOP(IOrganizationService _orgService, Guid id)
        {
            Entity entity = _orgService.Retrieve(_entityname, id, new ColumnSet(true));

            //Send Email to Approval Discount User.
            if (entity.Attributes.Contains("tss_pss") && entity.Attributes.Contains("tss_quotationnumber"))
            {
                List<Guid> listApprover = new List<Guid>();
                string quo_Number = entity.GetAttributeValue<string>("tss_quotationnumber");
                int prioriyNo = int.MinValue;
                DL_systemuser _DL_systemuser = new DL_systemuser();
                Entity pss = _DL_systemuser.Select(_orgService, entity.GetAttributeValue<EntityReference>("tss_pss").Id);
                EntityReference branch = pss.GetAttributeValue<EntityReference>("businessunitid");
                DateTime currentdatetime = DateTime.Now;
                DateTime topdatetime = entity.GetAttributeValue<DateTime>("tss_topdatetime").ToLocalTime();
                EntityReference currentApproval = entity.GetAttributeValue<EntityReference>("tss_topcurrentapprover");

                QueryExpression queryMatrixapprovaltop = new QueryExpression("tss_matrixapprovaltop")
                {
                    ColumnSet = new ColumnSet(true),
                    Criteria =
                    {
                        Conditions =
                        {
                            new ConditionExpression("tss_branch", ConditionOperator.Equal, branch.Id),
                        }
                    },
                    Orders =
                        {
                            new OrderExpression("tss_priorityno", OrderType.Ascending)
                        }
                };

                EntityCollection MatrixApprovals = _orgService.RetrieveMultiple(queryMatrixapprovaltop);
                Entity currMatrixApproval = MatrixApprovals.Entities.Where(o => o.Id == currentApproval.Id).First();
                listApprover.Add(currMatrixApproval.Id);
                prioriyNo = currMatrixApproval.GetAttributeValue<int>("tss_priorityno");
                String strPathAndQuery = HttpContext.Current.Request.Url.PathAndQuery;
                String CRM_URL = HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");
                SendEmailToApproval(entity.Id, listApprover.ToArray(), _orgService, CRM_URL, 2);
                //Add to approver list
                #region Approver List
                QueryExpression qeApproverList = new QueryExpression("tss_matrixapprovaltop")
                {
                    ColumnSet = new ColumnSet(true),
                    Criteria =
                    {
                        Conditions =
                        {
                            new ConditionExpression("tss_branch", ConditionOperator.Equal, branch.Id),
                            new ConditionExpression("tss_priorityno", ConditionOperator.GreaterEqual, prioriyNo)
                        }
                    }
                };
                EntityCollection ecApproverList = _orgService.RetrieveMultiple(qeApproverList);
                foreach (var lstApprover in ecApproverList.Entities)
                {
                    _DL_tss_approverlist = new DL_tss_approverlist();
                    _DL_tss_approverlist.tss_quotationpart = id;
                    _DL_tss_approverlist.tss_approver = lstApprover.GetAttributeValue<EntityReference>("tss_approver").Id;
                    _DL_tss_approverlist.CreateApprover(_orgService);

                    //Retrieve SystemUser
                    Entity systemUser = _DL_systemuser.Select(_orgService, _DL_tss_approverlist.tss_approver);
                    //Grant Access Request / Share Record Form
                    _ShareRecords = new ShareRecords();
                    _ShareRecords.ShareRecord(_orgService, entity, systemUser);
                }
                #endregion
            }
        }

        public void UpdateStatusReason(IOrganizationService organizationService, Guid id, ITracingService tracingService)
        {
            try
            {
                List<Guid> listApprover = new List<Guid>();
                var context = organizationService;
                _DL_tss_quotationpartheader = new DL_tss_quotationpartheader();
                _DL_tss_quotationpartheader.tss_statusreason = STATUSREASON_WAITINGAPPROVALDISCOUNT;
                _DL_tss_quotationpartheader.tss_quotationstatus = STATUSCODE_INPROGRESS;
                //Bukan create SO, tapi function nya bisa di pakai.
                _DL_tss_quotationpartheader.UpdateStatusAfterCreateSO(organizationService, id);

                Entity entity = organizationService.Retrieve(_entityname, id, new ColumnSet(true));

                //Send Email to Approval Discount User.
                if (entity.Attributes.Contains("tss_pss") && entity.Attributes.Contains("tss_quotationnumber"))
                {
                    string quo_Number = entity.GetAttributeValue<string>("tss_quotationnumber");

                    Entity pss = _DL_systemuser.Select(organizationService, entity.GetAttributeValue<EntityReference>("tss_pss").Id);
                    EntityReference branch = pss.GetAttributeValue<EntityReference>("businessunitid");
                    QueryExpression queryGetQP_Lines = new QueryExpression("tss_quotationpartlines");
                    queryGetQP_Lines.Orders.Add(new OrderExpression("tss_discountpercent", OrderType.Descending));
                    queryGetQP_Lines.ColumnSet = new ColumnSet(true);
                    queryGetQP_Lines.Criteria = new FilterExpression
                    {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                        {
                            new ConditionExpression("tss_quotationpartheader", ConditionOperator.Equal, entity.Id),
                            new ConditionExpression("tss_approvebypa", ConditionOperator.NotEqual, true),
                            //new ConditionExpression("tss_unitgroup", ConditionOperator.NotNull)
                        }
                    };
                    EntityCollection QP_Lines = organizationService.RetrieveMultiple(queryGetQP_Lines);
                    tracingService.Trace("QP_Lines = " + QP_Lines.Entities.Count.ToString());
                    if (QP_Lines.Entities.Count > 0)
                    {
                        Entity QP_Line = QP_Lines.Entities.First();
                        DateTime currentdatetime = DateTime.Now;
                        DateTime discountdatetime = DateTime.Now.ToLocalTime();
                        decimal percentageindecimal = QP_Line.GetAttributeValue<decimal>("tss_discountpercent");
                        int percentage = Convert.ToInt32(Math.Round(percentageindecimal, 0, MidpointRounding.AwayFromZero));

                        var orgContext = new OrganizationServiceContext(organizationService);
                        
                        //get minimum discount
                        var allApprovalDiscount = (from c in orgContext.CreateQuery("tss_matrixapprovaldiscount")
                                                   where c.GetAttributeValue<EntityReference>("tss_branch").Id == branch.Id
                                                   where c.GetAttributeValue<int>("tss_startpercentage") <= percentage //20180827
                                                   where c.GetAttributeValue<int>("tss_endpercentage") >= percentage //20180827
                                                   select c).ToList();
                        tracingService.Trace("allApprovalDiscount = " + allApprovalDiscount.Count.ToString());

                        int minStart = int.MaxValue;
                        int minEnd = int.MaxValue;
                        foreach (var x in allApprovalDiscount)
                        {
                            if (x.Attributes.Contains("tss_startpercentage"))
                            {
                                if (x.GetAttributeValue<int>("tss_startpercentage") < minStart)
                                {
                                    minStart = x.GetAttributeValue<int>("tss_startpercentage");
                                    minEnd = x.GetAttributeValue<int>("tss_endpercentage");
                                }
                            }
                        }

                        if (percentage >= minStart && percentage <= minEnd)
                        {
                            var minApprovalDiscount = (from c in orgContext.CreateQuery("tss_matrixapprovaldiscount")
                                                       where c.GetAttributeValue<EntityReference>("tss_branch").Id == branch.Id
                                                       where c.GetAttributeValue<int>("tss_startpercentage") >= minStart
                                                       where c.GetAttributeValue<EntityReference>("tss_approver").Id == entity.GetAttributeValue<EntityReference>("tss_pss").Id
                                                       select c).ToList();
                            if (minApprovalDiscount.Count > 0)
                            {
                                //approve pss
                                _DL_tss_approverlist = new DL_tss_approverlist();
                                _DL_tss_approverlist.tss_quotationpart = id;
                                _DL_tss_approverlist.tss_approver = entity.GetAttributeValue<EntityReference>("tss_pss").Id;
                                _DL_tss_approverlist.CreateApprover(organizationService);

                                //approve other with higher percentage
                                var allApprovalDiscountWithoutMin = (from c in orgContext.CreateQuery("tss_matrixapprovaldiscount")
                                                                     where c.GetAttributeValue<EntityReference>("tss_branch").Id == branch.Id
                                                                     where c.GetAttributeValue<int>("tss_startpercentage") >= minStart
                                                                     select c).ToList();
                                foreach (var x in allApprovalDiscountWithoutMin)
                                {
                                    _DL_tss_approverlist = new DL_tss_approverlist();
                                    _DL_tss_approverlist.tss_quotationpart = id;
                                    _DL_tss_approverlist.tss_approver = x.GetAttributeValue<EntityReference>("tss_approver").Id;
                                    _DL_tss_approverlist.CreateApprover(organizationService);

                                    //Grant Access Request / Share Record Form
                                    Entity systemUser = _DL_systemuser.Select(organizationService, _DL_tss_approverlist.tss_approver);
                                    _ShareRecords = new ShareRecords();
                                    _ShareRecords.ShareRecord(organizationService, entity, systemUser);
                                }
                            }
                            else
                            {
                                QueryExpression queryMatrixapprovaldiscount = new QueryExpression("tss_matrixapprovaldiscount")
                                {
                                    ColumnSet = new ColumnSet(true),
                                    Criteria =
                                {
                                    Conditions =
                                {
                                    new ConditionExpression("tss_branch", ConditionOperator.Equal, branch.Id),
                                    new ConditionExpression("tss_endpercentage", ConditionOperator.GreaterEqual, percentage),
                                    new ConditionExpression("tss_startpercentage", ConditionOperator.LessThan, percentage)
                                }
                                }
                                };
                                EntityCollection MatrixApprovals = organizationService.RetrieveMultiple(queryMatrixapprovaldiscount);

                                foreach (var item in MatrixApprovals.Entities)
                                {
                                    EntityReference nextApproval = item.GetAttributeValue<EntityReference>("tss_approver");
                                    listApprover.Add(nextApproval.Id);
                                }
                                String strPathAndQuery = HttpContext.Current.Request.Url.PathAndQuery;
                                String CRM_URL = HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");
                                SendEmailToApproval(entity.Id, listApprover.ToArray(), organizationService, CRM_URL, 1);

                                //Add to approver list
                                #region Approver List
                                QueryExpression qeApproverList = new QueryExpression("tss_matrixapprovaldiscount")
                                {
                                    ColumnSet = new ColumnSet(true),
                                    Criteria =
                                {
                                    Conditions =
                                {
                                    new ConditionExpression("tss_branch", ConditionOperator.Equal, branch.Id),
                                    new ConditionExpression("tss_endpercentage", ConditionOperator.GreaterEqual, percentage)
                                }
                                }
                                };
                                EntityCollection ecApproverList = organizationService.RetrieveMultiple(qeApproverList);
                                foreach (var lstApprover in ecApproverList.Entities)
                                {
                                    _DL_tss_approverlist = new DL_tss_approverlist();
                                    _DL_tss_approverlist.tss_quotationpart = id;
                                    _DL_tss_approverlist.tss_approver = lstApprover.GetAttributeValue<EntityReference>("tss_approver").Id;
                                    _DL_tss_approverlist.CreateApprover(organizationService);

                                    //Retrieve SystemUser
                                    Entity systemUser = _DL_systemuser.Select(organizationService, _DL_tss_approverlist.tss_approver);
                                    //Grant Access Request / Share Record Form
                                    _ShareRecords = new ShareRecords();
                                    _ShareRecords.ShareRecord(organizationService, entity, systemUser);
                                }
                                #endregion
                            }
                        }
                        //else
                        //{
                        //    QueryExpression queryMatrixapprovaldiscount = new QueryExpression("tss_matrixapprovaldiscount")
                        //    {
                        //        ColumnSet = new ColumnSet(true),
                        //        Criteria =
                        //        {
                        //            Conditions =
                        //        {
                        //            new ConditionExpression("tss_branch", ConditionOperator.Equal, branch.Id),
                        //            new ConditionExpression("tss_endpercentage", ConditionOperator.GreaterEqual, percentage),
                        //            new ConditionExpression("tss_startpercentage", ConditionOperator.LessThan, percentage)
                        //        }
                        //        }
                        //    };
                        //    EntityCollection MatrixApprovals = organizationService.RetrieveMultiple(queryMatrixapprovaldiscount);

                        //    foreach (var item in MatrixApprovals.Entities)
                        //    {
                        //        EntityReference nextApproval = item.GetAttributeValue<EntityReference>("tss_approver");
                        //        listApprover.Add(nextApproval.Id);
                        //    }
                        //    String strPathAndQuery = HttpContext.Current.Request.Url.PathAndQuery;
                        //    String CRM_URL = HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");
                        //    SendEmailToApproval(entity.Id, listApprover.ToArray(), organizationService, CRM_URL, 1);

                        //    //Add to approver list
                        //    #region Approver List
                        //    QueryExpression qeApproverList = new QueryExpression("tss_matrixapprovaldiscount")
                        //    {
                        //        ColumnSet = new ColumnSet(true),
                        //        Criteria =
                        //        {
                        //            Conditions =
                        //        {
                        //            new ConditionExpression("tss_branch", ConditionOperator.Equal, branch.Id),
                        //            new ConditionExpression("tss_endpercentage", ConditionOperator.GreaterEqual, percentage)
                        //        }
                        //        }
                        //    };
                        //    EntityCollection ecApproverList = organizationService.RetrieveMultiple(qeApproverList);
                        //    foreach (var lstApprover in ecApproverList.Entities)
                        //    {
                        //        _DL_tss_approverlist = new DL_tss_approverlist();
                        //        _DL_tss_approverlist.tss_quotationpart = id;
                        //        _DL_tss_approverlist.tss_approver = lstApprover.GetAttributeValue<EntityReference>("tss_approver").Id;
                        //        _DL_tss_approverlist.CreateApprover(organizationService);

                        //        //Retrieve SystemUser
                        //        Entity systemUser = _DL_systemuser.Select(organizationService, _DL_tss_approverlist.tss_approver);
                        //        //Grant Access Request / Share Record Form
                        //        _ShareRecords = new ShareRecords();
                        //        _ShareRecords.ShareRecord(organizationService, entity, systemUser);
                        //    }
                        //    #endregion
                        //}

                        //Update Date & Approver in Quot. Part Header.
                        _DL_tss_quotationpartheader = new DL_tss_quotationpartheader();
                        _DL_tss_quotationpartheader.tss_discountdatetime = discountdatetime;
                        //_DL_tss_quotationpartheader.tss_discountcurrentapprover = nextApproval.Id;
                        _DL_tss_quotationpartheader.UpdateApprovalReason(context, entity.Id);

                        //QueryExpression queryMatrixapprovaldiscount = new QueryExpression("tss_matrixapprovaldiscount")
                        //{
                        //    ColumnSet = new ColumnSet(true),
                        //    Criteria =
                        //    {
                        //        Conditions =
                        //        {
                        //            new ConditionExpression("tss_branch", ConditionOperator.Equal, branch.Id),
                        //            new ConditionExpression("tss_endpercentage", ConditionOperator.GreaterEqual, percentage),
                        //            new ConditionExpression("tss_startpercentage", ConditionOperator.LessThan, percentage)
                        //        }
                        //    }
                        //};
                        //EntityCollection MatrixApprovals = organizationService.RetrieveMultiple(queryMatrixapprovaldiscount);

                        //foreach (var item in MatrixApprovals.Entities)
                        //{
                        //    EntityReference nextApproval = item.GetAttributeValue<EntityReference>("tss_approver");
                        //    listApprover.Add(nextApproval.Id);
                        //}
                        //String strPathAndQuery = HttpContext.Current.Request.Url.PathAndQuery;
                        //String CRM_URL = HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");
                        //SendEmailToApproval(entity.Id, listApprover.ToArray(), organizationService, CRM_URL, 1);
                        ////Update Date & Approver in Quot. Part Header.
                        //_DL_tss_quotationpartheader = new DL_tss_quotationpartheader();
                        //_DL_tss_quotationpartheader.tss_discountdatetime = discountdatetime;
                        ////_DL_tss_quotationpartheader.tss_discountcurrentapprover = nextApproval.Id;
                        //_DL_tss_quotationpartheader.UpdateApprovalReason(context, entity.Id);


                        ////Add to approver list
                        //#region Approver List
                        //QueryExpression qeApproverList = new QueryExpression("tss_matrixapprovaldiscount")
                        //{
                        //    ColumnSet = new ColumnSet(true),
                        //    Criteria =
                        //    {
                        //        Conditions =
                        //        {
                        //            new ConditionExpression("tss_branch", ConditionOperator.Equal, branch.Id),
                        //            new ConditionExpression("tss_endpercentage", ConditionOperator.GreaterEqual, percentage)
                        //        }
                        //    }
                        //};
                        //EntityCollection ecApproverList = organizationService.RetrieveMultiple(qeApproverList);
                        //foreach (var lstApprover in ecApproverList.Entities)
                        //{
                        //    _DL_tss_approverlist = new DL_tss_approverlist();
                        //    _DL_tss_approverlist.tss_quotationpart = id;
                        //    _DL_tss_approverlist.tss_approver = lstApprover.GetAttributeValue<EntityReference>("tss_approver").Id;
                        //    _DL_tss_approverlist.CreateApprover(organizationService);

                        //    //Retrieve SystemUser
                        //    Entity systemUser = _DL_systemuser.Select(organizationService, _DL_tss_approverlist.tss_approver);
                        //    //Grant Access Request / Share Record Form
                        //    _ShareRecords = new ShareRecords();
                        //    _ShareRecords.ShareRecord(organizationService, entity, systemUser);
                        //}
                        //#endregion
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".UpdateStatusReason : " + ex.Message.ToString());
            }
        }

        private Entity SendEmailNotif(Guid senderGuid, Guid receiverGuid, Guid ccGuid, IOrganizationService organizationService, string subject, string bodyTemplate)
        {
            try
            {
                var emailAgent = new Helper.EmailAgent();
                var emailDescription = bodyTemplate;
                var emailFactory = new Helper.EmailFactory();

                emailFactory.SetFrom(emailAgent.CreateActivityParty(new List<Tuple<Guid, string>>
                {
                    new Tuple<Guid, string>(senderGuid,"systemuser")
                }));
                emailFactory.SetTo(emailAgent.CreateActivityParty(new List<Tuple<Guid, string>>
                {
                    new Tuple<Guid, string>(receiverGuid,"systemuser")
                }));
                emailFactory.SetCC(emailAgent.CreateActivityParty(new List<Tuple<Guid, string>>
                {
                    new Tuple<Guid, string>(ccGuid,"systemuser")
                }));
                emailFactory.SetSubject(subject);
                emailFactory.SetContent(emailDescription);
                return emailFactory.Create();
            }
            catch (Exception ex)
            {
                throw new Exception("Send Email Failed. Technical Details :\r\n" + ex.ToString());
            }
        }

        public void sendEmailtoTOPApproval(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext, Entity postImg, Entity preImg)
        {
            try
            {
                if (preImg.Contains("tss_statusreason") && postImg.Contains("tss_statusreason"))
                {
                    var entity = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));

                    if (entity.Contains("tss_quotationnumber") && entity.Contains("tss_pss")
                        && preImg.GetAttributeValue<OptionSetValue>("tss_statusreason").Value != 865920001 && postImg.GetAttributeValue<OptionSetValue>("tss_statusreason").Value == 865920001)
                    {
                        DateTime currentdatetime = DateTime.Now;
                        DL_systemuser _DL_systemuser = new DL_systemuser();
                        Entity sender = GetFromAdminCRM(organizationService).Entities.First();
                        Entity pss = _DL_systemuser.Select(organizationService, entity.GetAttributeValue<EntityReference>("tss_pss").Id);
                        if (!pss.Contains("businessunitid")) throw new Exception("Branch on PSS not found!");
                        EntityReference branch = pss.GetAttributeValue<EntityReference>("businessunitid");
                        string quo_Number = entity.GetAttributeValue<string>("tss_quotationnumber");

                        QueryExpression queryMatrixapprovaltop = new QueryExpression("tss_matrixapprovaltop")
                        {
                            ColumnSet = new ColumnSet(true),
                            Criteria =
                            {
                                Conditions =
                                {
                                    new ConditionExpression("tss_branch", ConditionOperator.Equal, branch.Id),
                                }
                            },
                            Orders =
                            {
                                new OrderExpression("tss_priorityno", OrderType.Ascending)
                            }
                        };
                        EntityCollection MatrixApprovals = organizationService.RetrieveMultiple(queryMatrixapprovaltop);
                        Entity currentMatrixApproval = MatrixApprovals.Entities.FirstOrDefault();

                        if (currentMatrixApproval != null)
                        {
                            String strPathAndQuery = HttpContext.Current.Request.Url.PathAndQuery;
                            string CRM_URL = HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");
                            CRM_URL += "TraktorNusantara";

                            HelperFunction help = new HelperFunction();
                            string objecttypecode = string.Empty;
                            string targetCustomer = string.Empty;
                            string targetUnit = string.Empty;
                            string createdby = string.Empty;
                            help.GetObjectTypeCode(organizationService, "tss_quotationpartheader", out objecttypecode);

                            if (!currentMatrixApproval.Contains("tss_approver")) throw new Exception("Approver not found on Matrix Approval TOP");
                            EntityReference currApproval = currentMatrixApproval.GetAttributeValue<EntityReference>("tss_approver");
                            Entity Approvaldetail = organizationService.Retrieve("systemuser", currApproval.Id, new ColumnSet(true));
                            string receivername = Approvaldetail.GetAttributeValue<string>("fullname");
                            if (!entity.Contains("ownerid")) throw new Exception("Owner not found!");
                            EntityReference ccRef = entity.GetAttributeValue<EntityReference>("ownerid");

                            var context = new OrganizationServiceContext(organizationService);
                            var quot = (from c in context.CreateQuery("tss_quotationpartheader")
                                        where c.GetAttributeValue<Guid>("tss_quotationpartheaderid") == entity.Id
                                        select c).ToList();
                            if (quot.Count > 0)
                            {
                                if (quot[0].Attributes.Contains("tss_customer") && quot[0].GetAttributeValue<EntityReference>("tss_customer").Name != null)
                                {
                                    targetCustomer = quot[0].GetAttributeValue<EntityReference>("tss_customer").Name;
                                }
                                if (quot[0].Attributes.Contains("tss_unit") && quot[0].GetAttributeValue<EntityReference>("tss_unit").Name != null)
                                {
                                    targetUnit = quot[0].GetAttributeValue<EntityReference>("tss_unit").Name;
                                }
                                if (quot[0].Attributes.Contains("createdby") && quot[0].GetAttributeValue<EntityReference>("createdby").Name != null)
                                {
                                    createdby = quot[0].GetAttributeValue<EntityReference>("createdby").Name;
                                }
                            }

                            var subject = @"Waiting Approval Change TOP on Quotation Part with Quotation Number " + entity.GetAttributeValue<string>("tss_quotationnumber");

                            var bodyTemplate = "Dear Mr/Ms" + receivername + ",<br/><br/>";
                            bodyTemplate += "Need Your Approval Change TOP for Quotation below:<br/><br/>";
                            if (targetCustomer != string.Empty) bodyTemplate += "Target customer : " + targetCustomer + "<br/>";
                            if (targetUnit != string.Empty) bodyTemplate += "Target unit : " + targetUnit + "<br/>";
                            bodyTemplate += "Quotation : " + "<a href='" + CRM_URL + "/main.aspx?etc=" + objecttypecode + "&pagetype=entityrecord&id=%7b" + entity.Id + "%7d'>Click here</a><br/><br/>";
                            bodyTemplate += "Thanks,<br/><br/>";
                            bodyTemplate += "Regards,<br/><br/>";
                            bodyTemplate += createdby;

                            if (sender.Id == Guid.Empty || currApproval.Id == Guid.Empty || ccRef.Id == Guid.Empty) throw new Exception("Not enough data on create email notification");
                            Entity email = SendEmailNotif(sender.Id, currApproval.Id, ccRef.Id, organizationService, subject, bodyTemplate);
                            Guid EmailId = organizationService.Create(email);
                            EmailFactory emailfactory = new EmailFactory();
                            emailfactory.SendEmail(organizationService, EmailId);

                            if (currentMatrixApproval == null) throw new Exception("Current Approver not found");
                            entity["tss_topcurrentapprover"] = currentMatrixApproval.ToEntityReference();
                            entity["tss_topdatetime"] = currentdatetime;
                            organizationService.Update(entity);

                            //add records to approval list
                            foreach (var MatrixApproval in MatrixApprovals.Entities)
                            {
                                EntityReference Approval = MatrixApproval.GetAttributeValue<EntityReference>("tss_approver");
                                Entity Approvalrecord = organizationService.Retrieve("systemuser", Approval.Id, new ColumnSet(true));

                                Entity addApprover = new Entity("tss_approverlist");
                                if (Approval == null) throw new Exception("Approver not found!");
                                addApprover["tss_approver"] = Approval;
                                if (entity == null) throw new Exception("Quotation Part Header not found on create Approval List");
                                addApprover["tss_quotationpartheaderid"] = entity.ToEntityReference();

                                if (Approvalrecord == null) throw new Exception("Approver not found on share record!");
                                _ShareRecords = new ShareRecords();
                                //Share Grant Access - Shared Form to Quotation Part
                                _ShareRecords.ShareRecord(organizationService, entity, Approvalrecord);
                                //Create Records
                                organizationService.Create(addApprover);
                            }
                        }
                        else
                        {
                            throw new Exception("Approval not found!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".sendEmailtoTOPApproval : " + ex.Message.ToString());
            }
        }

        #endregion

        #region Public Email
        private Entity GetSystemUserByFullname(IOrganizationService organizationService, string fullName)
        {
            try
            {
                var systemUserQuery = new QueryExpression("systemuser")
                {
                    ColumnSet = new ColumnSet(true),
                    Criteria =
                    {
                        Conditions =
                        {
                            new ConditionExpression("fullname", ConditionOperator.Equal, fullName)
                        }
                    }
                };
                var systemUserCollection = organizationService.RetrieveMultiple(systemUserQuery);
                if (systemUserCollection.Entities.Count > 0)
                {
                    return systemUserCollection.Entities.First();
                }
                else
                {
                    throw new Exception("System user with fullname " + fullName + " is not found.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured when getting system user by full name.\r\nTechnical Details: " + ex.ToString());
            }
        }

        private Entity SendEmailToPSS(Guid senderGuid, Guid receiverGuid, Guid ccGuid,
            IOrganizationService organizationService, string subject, string bodyTemplate)
        {
            try
            {
                string row = string.Empty;

                var emailAgent = new Helper.EmailAgent();
                var emailDescription = bodyTemplate;
                var emailFactory = new Helper.EmailFactory();

                emailFactory.SetFrom(emailAgent.CreateActivityParty(new List<Tuple<Guid, string>>
                {
                    new Tuple<Guid, string>(senderGuid,"systemuser")
                }));
                emailFactory.SetTo(emailAgent.CreateActivityParty(new List<Tuple<Guid, string>>
                {
                    new Tuple<Guid, string>(receiverGuid,"systemuser")
                }));
                emailFactory.SetCC(emailAgent.CreateActivityParty(new List<Tuple<Guid, string>>
                {
                    new Tuple<Guid, string>(ccGuid,"systemuser")
                }));
                emailFactory.SetSubject(subject);
                emailFactory.SetContent(emailDescription);
                return emailFactory.Create();
            }
            catch (Exception ex)
            {
                throw new Exception("Send Email to PSS Failed. Technical Details :\r\n" + ex.ToString());
            }
        }

        public static void SendEmailToApproval(Guid quotationID, Guid[] listReceiver, IOrganizationService organizationService, string url, int caseNumber)
        {
            try
            {
                Entity quotationheader = organizationService.Retrieve("tss_quotationpartheader", quotationID, new ColumnSet(true));
                Guid createEmail = Guid.Empty;
                EntityCollection admins = GetFromAdminCRM(organizationService);
                Guid crmadmin = admins.Entities[0].Id;
                //new Guid("1CBA90DC-4DE9-E111-9AA2-544249894792");
                string quotationNumber = quotationheader.GetAttributeValue<string>("tss_quotationnumber");
                switch (caseNumber)
                {
                    case 1:
                        createEmail = CreateEmailNotifReasonDiscount(crmadmin, listReceiver, crmadmin,
                    organizationService, quotationNumber, quotationheader.Id.ToString(), url);
                        break;
                    case 2:
                        createEmail = CreateEmailNotifChangeTOP(crmadmin, listReceiver, crmadmin,
                    organizationService, quotationNumber, quotationheader.Id.ToString(), url);
                        break;
                }
                EmailFactory emailfactory = new EmailFactory();
                emailfactory.SendEmail(organizationService, createEmail);

            }
            catch (Exception ex)
            {
                //Process here for needed (Tracing or whatever.)
                throw new InvalidPluginExecutionException("Failed to Send Email. Technical Details : \r\n" + ex.ToString());
            }
        }

        private static Guid CreateEmailNotifReasonDiscount(Guid senderGuid, Guid[] listReceiver, Guid ccGuid, IOrganizationService organizationService, string QuotationNumber, string recordID, string url)
        {
            try
            {
                Guid email = Guid.Empty;
                DL_systemuser _DL_user = new DL_systemuser();
                HelperFunction help = new HelperFunction();

                string objecttypecode = string.Empty;
                string CRMURL = string.Empty;
                string fullNames = string.Empty;
                string targetCustomer = string.Empty;
                string targetUnit = string.Empty;
                string createdby = string.Empty;

                help.GetObjectTypeCode(organizationService, "tss_quotationpartheader", out objecttypecode);
                url += "TraktorNusantara";
                CRMURL += url;
                url += "/main.aspx?etc=" + objecttypecode + "&pagetype=entityrecord&id=" + recordID;
                var subject = @"Waiting Approval Discount on Quotation Part with Quotation Number " + QuotationNumber;
                string row = string.Empty;

                var context = new OrganizationServiceContext(organizationService);
                //get receiver fullname
                for (int i = 0; i < listReceiver.Count(); i++)
                {
                    var user = (from c in context.CreateQuery("systemuser")
                                where c.GetAttributeValue<Guid>("systemuserid") == listReceiver[i]
                                select c).ToList();
                    if (user.Count > 0)
                    {
                        if (user[0].Attributes.Contains("fullname"))
                        {
                            if (i > 0) fullNames += " / ";
                            fullNames += user[0].GetAttributeValue<string>("fullname");
                        }
                    }
                }

                var quot = (from c in context.CreateQuery("tss_quotationpartheader")
                            where c.GetAttributeValue<Guid>("tss_quotationpartheaderid") == new Guid(recordID)
                            select c).ToList();
                if (quot.Count > 0)
                {
                    if (quot[0].Attributes.Contains("tss_customer"))
                    {
                        targetCustomer = quot[0].GetAttributeValue<EntityReference>("tss_customer").Name;
                    }
                    if (quot[0].Attributes.Contains("tss_unit"))
                    {
                        targetUnit = quot[0].GetAttributeValue<EntityReference>("tss_unit").Name;
                    }
                    if (quot[0].Attributes.Contains("createdby"))
                    {
                        createdby = quot[0].GetAttributeValue<EntityReference>("createdby").Name;
                    }
                }

                var bodyTemplate = "Dear " + fullNames + ",<br/><br/>";
                bodyTemplate += "Need Your Approval Discount for Quotation below:<br/><br/>";
                if (targetCustomer != string.Empty) bodyTemplate += "Target customer : " + targetCustomer + "<br/>";
                if (targetUnit != string.Empty) bodyTemplate += "Target unit : " + targetUnit + "<br/>";
                bodyTemplate += "Quotation : " + "<a href='" + CRMURL + "/main.aspx?etc=" + objecttypecode + "&pagetype=entityrecord&id=%7b" + recordID + "%7d'>Click here</a><br/><br/>";
                bodyTemplate += "Thanks,<br/><br/>";
                bodyTemplate += "Regards,<br/><br/>";
                bodyTemplate += createdby;

                var emailAgent = new EmailAgent();
                emailAgent.AddSender(senderGuid);
                foreach (Guid item in listReceiver)
                {
                    emailAgent.AddReceiver("systemuser", item);
                }
                //emailAgent.AddCC("systemuser", ccGuid); //bermasalah owning usernya
                emailAgent.subject = subject;
                emailAgent.description = bodyTemplate;
                emailAgent.priority = EmailAgent.Priority_High;
                emailAgent.trs_autosend = true;//set false dulu, jadi draft.
                emailAgent.Create(organizationService, out email);

                return email;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        private static Guid CreateEmailNotifChangeTOP(Guid senderGuid, Guid[] listReceiver, Guid ccGuid, IOrganizationService organizationService, string QuotationNumber, string recordID, string url)
        {
            try
            {
                Guid email = Guid.Empty;
                DL_systemuser _DL_user = new DL_systemuser();
                HelperFunction help = new HelperFunction();

                //Entity receiverRecord = _DL_user.Select(organizationService, receiverGuid);
                //string Fullname = receiverRecord.GetAttributeValue<string>("fullname");
                string objecttypecode = string.Empty;
                string CRMURL = string.Empty;
                string fullNames = string.Empty;
                string targetCustomer = string.Empty;
                string targetUnit = string.Empty;
                string createdby = string.Empty;

                help.GetObjectTypeCode(organizationService, "tss_quotationpartheader", out objecttypecode);
                url += "TraktorNusantara";
                CRMURL += url;
                url += "/main.aspx?etc=" + objecttypecode + "&pagetype=entityrecord&id=" + recordID;
                var subject = @"Waiting Approval Change Term of Payment (TOP) on Quotation Part with Quotation Number " + QuotationNumber;
                string row = string.Empty;

                var context = new OrganizationServiceContext(organizationService);
                //get receiver fullname
                for (int i = 0; i < listReceiver.Count(); i++)
                {
                    var user = (from c in context.CreateQuery("systemuser")
                                where c.GetAttributeValue<Guid>("systemuserid") == listReceiver[i]
                                select c).ToList();
                    if (user.Count > 0)
                    {
                        if (user[0].Attributes.Contains("fullname"))
                        {
                            if (i > 0) fullNames += " / ";
                            fullNames += user[0].GetAttributeValue<string>("fullname");
                        }
                    }
                }

                var quot = (from c in context.CreateQuery("tss_quotationpartheader")
                            where c.GetAttributeValue<Guid>("tss_quotationpartheaderid") == new Guid(recordID)
                            select c).ToList();
                if (quot.Count > 0)
                {
                    if (quot[0].Attributes.Contains("tss_customer"))
                    {
                        targetCustomer = quot[0].GetAttributeValue<EntityReference>("tss_customer").Name;
                    }
                    if (quot[0].Attributes.Contains("tss_unit"))
                    {
                        targetUnit = quot[0].GetAttributeValue<EntityReference>("tss_unit").Name;
                    }
                    if (quot[0].Attributes.Contains("createdby"))
                    {
                        createdby = quot[0].GetAttributeValue<EntityReference>("createdby").Name;
                    }
                }

                var bodyTemplate = "Dear " + fullNames + ",<br/><br/>";
                bodyTemplate += "Need Your Approval Change TOP for Quotation below:<br/><br/>";
                if (targetCustomer != string.Empty) bodyTemplate += "Target customer : " + targetCustomer + "<br/>";
                if (targetUnit != string.Empty) bodyTemplate += "Target unit : " + targetUnit + "<br/>";
                bodyTemplate += "Quotation : " + "<a href='" + CRMURL + "/main.aspx?etc=" + objecttypecode + "&pagetype=entityrecord&id=%7b" + recordID + "%7d'>Click here</a><br/><br/>";
                bodyTemplate += "Thanks,<br/><br/>";
                bodyTemplate += "Regards,<br/><br/>";
                bodyTemplate += createdby;

                var emailAgent = new EmailAgent();
                emailAgent.AddSender(senderGuid);
                foreach (Guid item in listReceiver)
                {
                    emailAgent.AddReceiver("systemuser", item);
                }
                //emailAgent.AddCC("systemuser", ccGuid); //bermasalah owning usernya
                emailAgent.subject = subject;
                emailAgent.description = bodyTemplate;
                emailAgent.priority = EmailAgent.Priority_High;
                emailAgent.trs_autosend = true;//set false dulu, jadi draft.
                emailAgent.Create(organizationService, out email);

                return email;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private static EntityCollection GetFromAdminCRM(IOrganizationService organizationService)
        {
            DL_systemuser adm = new DL_systemuser();

            QueryExpression queryExpression = new QueryExpression(adm.EntityName);
            queryExpression.ColumnSet.AddColumns("systemuserid", "domainname");
            queryExpression.Criteria.AddCondition("domainname", ConditionOperator.Equal, "TRAKNUS\\admin.crm");

            return adm.Select(organizationService, queryExpression);
        }
        #endregion

        public void CreateSOFromQuotation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));

                if (entity.LogicalName == pluginExceptionContext.PrimaryEntityName)
                {
                    if (entity.Attributes.Contains("tss_salesorderreference"))
                    {
                        var salesOrderId = entity.GetAttributeValue<EntityReference>("tss_totalfinalprice").Id;

                        if (salesOrderId != null)
                        {

                        }
                    }
                }
                else
                {
                    throw new Exception("Wrong entity " + entity.LogicalName + " on Sales Order Part Lines !");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PostOperation: " + ex.Message.ToString());
            }
        }

        public void CreateSOPartLines()
        {

        }
    }
}
