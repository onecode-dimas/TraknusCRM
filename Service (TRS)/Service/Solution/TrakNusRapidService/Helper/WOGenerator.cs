using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;

using TrakNusRapidService.DataLayer;

namespace TrakNusRapidService.Helper
{
    public class WOGenerator
    {
        #region Constants
        private const string _classname = "WOGenerator";
        #endregion

        #region Depedencies
        private DL_account _DL_account = new DL_account();
        private DL_activityparty _DL_activityparty = new DL_activityparty();
        private DL_contact _DL_contact = new DL_contact();
        private DL_incident _DL_incident = new DL_incident();
        private DL_new_population _DL_new_population = new DL_new_population();
        private DL_serviceappointment _DL_serviceappointment = new DL_serviceappointment();
        private DL_equipment _DL_equipment = new DL_equipment();
        private DL_task _DL_task = new DL_task();
        private DL_trs_masterpart _DL_trs_masterpart = new DL_trs_masterpart();
        private DL_new_pricelistcpo _DL_new_pricelistcpo = new DL_new_pricelistcpo();
        private DL_trs_partpricemaster _DL_trs_partpricemaster = new DL_trs_partpricemaster();
        private DL_trs_workorderpartssummary _DL_trs_workorderpartssummary = new DL_trs_workorderpartssummary();
        private DL_trs_workordertoolsrecommendation _DL_trs_workordertoolsrecommendation = new DL_trs_workordertoolsrecommendation();
        private DL_trs_contractpart _DL_trs_contractpart = new DL_trs_contractpart();
        private DL_trs_functionallocation _DL_trs_functionallocation = new DL_trs_functionallocation();
        private DL_businessunit _DL_businessunit = new DL_businessunit();
        private DL_contractdetail _DL_contractdetail = new DL_contractdetail();

        private DL_service _DL_service = new DL_service();
        private DL_trs_tasklistgroup _DL_trs_tasklistgroup = new DL_trs_tasklistgroup();
        private DL_trs_acttype _DL_trs_acttype = new DL_trs_acttype();
        private DL_trs_workcenter _DL_trs_workcenter = new DL_trs_workcenter();
        private DL_trs_profitcenter _DL_trs_profitcenter = new DL_trs_profitcenter();
        private DL_trs_responsiblecostcenter _DL_trs_responsiblecostcenter = new DL_trs_responsiblecostcenter();
        private DL_trs_commercialdetail _DL_trs_commercialdetail = new DL_trs_commercialdetail();

        private FMath _fMath = new FMath();
        #endregion

        #region Properties
        private EntityCollection _mechanicList;
        #endregion

        #region Privates
        private Guid GetServiceId(IOrganizationService organizationService, string keyword)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_service.EntityName);
                queryExpression.Criteria.AddCondition("name", ConditionOperator.Equal, keyword);
                EntityCollection entityCollection = _DL_service.Select(organizationService, queryExpression);
                if (entityCollection.Entities.Count > 0)
                {
                    return entityCollection.Entities[0].Id;
                }
                else
                {
                    throw new InvalidPluginExecutionException("Can not found Service with name '" + keyword + "'.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".GetServiceId : " + ex.Message);
            }
        }

        private Guid GetPMActTypeId(IOrganizationService organizationService, string keyword)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_trs_tasklistgroup.EntityName);
                queryExpression.Criteria.AddCondition("trs_pmacttype", ConditionOperator.Equal, keyword);
                EntityCollection entityCollection = _DL_trs_tasklistgroup.Select(organizationService, queryExpression);
                if (entityCollection.Entities.Count > 0)
                {
                    return entityCollection.Entities[0].Id;
                }
                else
                {
                    throw new InvalidPluginExecutionException("Can not found PMActType with code '" + keyword + "'.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".GetPMActTypeId : " + ex.Message);
            }
        }

        private Guid GetActivityTypeId(IOrganizationService organizationService, string keyword)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_trs_acttype.EntityName);
                queryExpression.Criteria.AddCondition("trs_name", ConditionOperator.Equal, keyword);
                EntityCollection entityCollection = _DL_trs_acttype.Select(organizationService, queryExpression);
                if (entityCollection.Entities.Count > 0)
                {
                    return entityCollection.Entities[0].Id;
                }
                else
                {
                    throw new InvalidPluginExecutionException("Can not found Activity Type with name '" + keyword + "'.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".GetAccountTypeId : " + ex.Message);
            }
        }

        private Guid GetWorkCenterId(IOrganizationService organizationService, Guid branchId, string keyword)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_trs_workcenter.EntityName);
                FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                filterExpression.AddCondition("trs_plant", ConditionOperator.Equal, branchId);
                filterExpression.AddCondition("trs_workcenter", ConditionOperator.Equal, keyword);
                EntityCollection entityCollection = _DL_trs_workcenter.Select(organizationService, queryExpression);
                if (entityCollection.Entities.Count > 0)
                {
                    return entityCollection.Entities[0].Id;
                }
                else
                {
                    throw new InvalidPluginExecutionException("Can not found Work Center for Branch Id '" + branchId.ToString() + "' with name '" + keyword + "'.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".GetWorkCenterId : " + ex.Message);
            }
        }

        private Guid GetProfitCenter(IOrganizationService organizationService, Guid branchId, string keyword)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_trs_profitcenter.EntityName);
                FilterExpression filterExpresison = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                filterExpresison.AddCondition("trs_businessarea", ConditionOperator.Equal, branchId);
                filterExpresison.AddCondition("trs_name", ConditionOperator.Equal, keyword);
                
                EntityCollection entityCollection = _DL_trs_profitcenter.Select(organizationService, queryExpression);
                if (entityCollection.Entities.Count > 0)
                {
                    return entityCollection.Entities[0].Id;
                }
                else
                {
                    throw new Exception("Can not found Profit Center for Branch Id '" + branchId.ToString() + "' with name '" + keyword + "'.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".GetProfitCenter : " + ex.Message);
            }
        }

        private Guid GetResposibleCostCenter(IOrganizationService organizationService, Guid profitCenterId)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_trs_responsiblecostcenter.EntityName);
                queryExpression.Criteria.AddCondition("trs_profitcenter", ConditionOperator.Equal, profitCenterId);
                EntityCollection entityCollection = _DL_trs_responsiblecostcenter.Select(organizationService, queryExpression);
                if (entityCollection.Entities.Count > 0)
                {
                    return entityCollection.Entities[0].Id;
                }
                else
                {
                    throw new Exception("Can not found Cost Center for Profit Center Id '" + profitCenterId.ToString() + "'.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".GetResposibleCostCenter : " + ex.Message);
            }
        }
        #endregion

        #region Publics
        public void AssignWO(IOrganizationService organizationService, Guid id, Guid assigneeId, bool team = false)
        {
            if (team)
                _DL_serviceappointment.AssigntoTeam(organizationService, id, assigneeId);
            else
                _DL_serviceappointment.AssigntoUser(organizationService, id, assigneeId);
        }

        public void AssignCommercialHeader(IOrganizationService organizationService, Guid id, Guid assigneeId, bool team = false)
        {
            if (team)
                _DL_task.AssigntoTeam(organizationService, id, assigneeId);
            else
                _DL_task.AssigntoUser(organizationService, id, assigneeId);
        }

        public void AddMechanic(Guid equipmentId)
        {
            try
            {
                _DL_activityparty = new DL_activityparty();
                _DL_activityparty.partyid = new EntityReference(_DL_equipment.EntityName, equipmentId);
                _mechanicList.Entities.Add(_DL_activityparty.GetEntity());
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".AddMechanic : " + ex.Message);
            }
        }

        public Guid AddCommercialHeader(IOrganizationService organizationService
            , Guid id
            , Guid taskHeaderId
            , decimal price)
        {
            try
            {
                _DL_task = new DL_task();
                _DL_task.trs_operationid = id;
                _DL_task.trs_tasklistheader = taskHeaderId;
                _DL_task.trs_price = price;
                _DL_task.trs_totalprice = price;
                return _DL_task.Insert(organizationService);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".AddCommercialHeader : " + ex.Message);
            }
        }

        public Guid GenerateWO(IOrganizationService organizationService
            , out Guid? branchId
            , Guid SRId
            , Guid PopulationId
            , Guid CustomerId
            , string Description
            , string keyword_Service
            , int AccountIndicator
            , string keyword_PMActType
            , string keyword_ActivityType
            , string keyword_WorkCenter
            , string keyword_ProfitCenter
            , Guid? ContactPersonId = null
            , Guid? ContactPersononSiteId = null
            , DateTime? ScheduledStartTime = null
            , DateTime? ScheduledEndTime = null
        )
        {
            try
            {
                branchId = Guid.Empty;

                _DL_serviceappointment = new DL_serviceappointment();
                _DL_serviceappointment.regardingobjectid = new EntityReference(_DL_incident.EntityName, SRId);

                #region Customer
                _DL_activityparty = new DL_activityparty();
                _DL_activityparty.partyid = new EntityReference(_DL_account.EntityName, (Guid)CustomerId);
                EntityCollection ecolCustomer = new EntityCollection();
                ecolCustomer.Entities.Add(_DL_activityparty.GetEntity());
                _DL_serviceappointment.customers = ecolCustomer;
                Entity eAccount = _DL_account.Select(organizationService, (Guid)CustomerId);
                if (eAccount.Attributes.Contains("telephone1"))
                    _DL_serviceappointment.trs_phone = eAccount.GetAttributeValue<string>("telephone1");
                if (eAccount.Attributes.Contains("new_npwp"))
                    _DL_serviceappointment.trs_npwp = eAccount.GetAttributeValue<string>("new_npwp");
                if (eAccount.Attributes.Contains("address1_name"))
                    _DL_serviceappointment.trs_address = eAccount.GetAttributeValue<string>("address1_name");

                #region Contact Person
                if (ContactPersonId != null)
                {
                    _DL_serviceappointment.trs_contactperson = (Guid)ContactPersonId;
                    Entity ePrimaryContact = _DL_contact.Select(organizationService, (Guid)ContactPersonId);
                    if (ePrimaryContact.Attributes.Contains("telephone1"))
                        _DL_serviceappointment.trs_cpphone = ePrimaryContact.GetAttributeValue<string>("telephone1");
                }
                #endregion

                #region Contact Person on Site
                if (ContactPersononSiteId != null)
                {
                    _DL_serviceappointment.trs_cponsite = (Guid)ContactPersononSiteId;
                    Entity eContactPersononSite = _DL_contact.Select(organizationService, (Guid)ContactPersononSiteId);
                    if (eContactPersononSite.Attributes.Contains("telephone1"))
                        _DL_serviceappointment.trs_phoneonsite = eContactPersononSite.GetAttributeValue<string>("telephone1");
                    else
                        throw new Exception("Can not found telephone number for Contact Person on Site.");
                }
                #endregion
                #endregion

                #region Population
                _DL_serviceappointment.trs_equipment = (Guid)PopulationId;
                Entity ePopulation = _DL_new_population.Select(organizationService, (Guid)PopulationId);
                if (ePopulation.Attributes.Contains("new_serialnumber"))
                    _DL_serviceappointment.new_serialnumber = ePopulation.GetAttributeValue<string>("new_serialnumber");
                if (ePopulation.Attributes.Contains("new_productitem"))
                    _DL_serviceappointment.trs_product = ePopulation.GetAttributeValue<string>("new_productitem");
                if (ePopulation.Attributes.Contains("new_model"))
                    _DL_serviceappointment.trs_productmodel = ePopulation.GetAttributeValue<string>("new_model");
                if (ePopulation.Attributes.Contains("new_deliverydate"))
                    _DL_serviceappointment.new_deliverydate = ePopulation.GetAttributeValue<DateTime>("new_deliverydate").ToUniversalTime();
                if (ePopulation.Attributes.Contains("new_enginenumber"))
                    _DL_serviceappointment.trs_enginenumber = ePopulation.GetAttributeValue<string>("new_enginenumber");
                if (ePopulation.Attributes.Contains("trs_chasisnumber"))
                    _DL_serviceappointment.trs_chasisnumber = ePopulation.GetAttributeValue<string>("trs_chasisnumber");
                if (ePopulation.Attributes.Contains("trs_hourmeter"))
                    _DL_serviceappointment.trs_hourmeter = ePopulation.GetAttributeValue<decimal>("trs_hourmeter");

                #region Functional Location
                Entity eFunctionalLocation = null;
                if (ePopulation.Attributes.Contains("trs_functionallocation"))
                {
                    _DL_serviceappointment.trs_functionallocation = ePopulation.GetAttributeValue<EntityReference>("trs_functionallocation").Id;
                    eFunctionalLocation = _DL_trs_functionallocation.Select(organizationService, _DL_serviceappointment.trs_functionallocation);
                    if (eFunctionalLocation.Attributes.Contains("trs_branch"))
                    {
                        branchId = eFunctionalLocation.GetAttributeValue<EntityReference>("trs_branch").Id;
                        if (branchId != null)
                        {
                            _DL_serviceappointment.trs_branch = (Guid)branchId;
                            _DL_serviceappointment.trs_workcenter = GetWorkCenterId(organizationService, (Guid)branchId, keyword_WorkCenter);
                            _DL_serviceappointment.trs_profitcenter = GetProfitCenter(organizationService, (Guid)branchId, keyword_ProfitCenter);
                            if (_DL_serviceappointment.trs_profitcenter != Guid.Empty)
                                _DL_serviceappointment.trs_responsiblecctr = GetResposibleCostCenter(organizationService, _DL_serviceappointment.trs_profitcenter);
                        }
                    }
                    else
                    {
                        throw new Exception("Can not found branch in functional location for this population.");
                    }
                    if (eFunctionalLocation.Attributes.Contains("trs_plant"))
                        _DL_serviceappointment.trs_plant = eFunctionalLocation.GetAttributeValue<EntityReference>("trs_plant").Id;
                    else
                        throw new Exception("Can not found plant in functional location for this population.");
                }
                #endregion
                #endregion

                #region Work Order
                _DL_serviceappointment.description = Description;
                _DL_serviceappointment.trs_accind = new OptionSetValue(AccountIndicator);
                _DL_serviceappointment.serviceid = GetServiceId(organizationService, keyword_Service);
                _DL_serviceappointment.trs_pmacttype = GetPMActTypeId(organizationService, keyword_PMActType);
                _DL_serviceappointment.trs_acttype = GetActivityTypeId(organizationService, keyword_ActivityType);
                _DL_serviceappointment.resources = _mechanicList;
                if (ScheduledStartTime != null)
                    _DL_serviceappointment.scheduledstart = ((DateTime)ScheduledStartTime).ToLocalTime();
                if (ScheduledEndTime != null)
                    _DL_serviceappointment.scheduledend = ((DateTime)ScheduledEndTime).ToLocalTime();
                #endregion

                return _DL_serviceappointment.Insert(organizationService);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".GenerateWO : " + ex.Message);
            }
        }

        public void CalculateRTG(IOrganizationService organizationService, Guid id)
        {
            try
            {
                DateTime startDate = new DateTime();
                Entity enServiceAppointment = _DL_serviceappointment.Select(organizationService, id);
                if (enServiceAppointment.Contains("scheduledstart"))
                {
                    //Added by Santony - 4/1/2015 (Menghitung Total RTG  di Commercial Detail)
                    CalculateRTG_CommercialDetail(organizationService, id);

                    startDate = enServiceAppointment.GetAttributeValue<DateTime>("scheduledstart");

                    QueryExpression qeCommercialHeader = new QueryExpression(_DL_task.EntityName);
                    qeCommercialHeader.ColumnSet = new ColumnSet(true);
                    FilterExpression feCommercialHeader = qeCommercialHeader.Criteria.AddFilter(LogicalOperator.And);
                    feCommercialHeader.AddCondition("trs_operationid", ConditionOperator.Equal, id);
                    EntityCollection ecCommercialHeader = _DL_task.Select(organizationService, qeCommercialHeader);

                    decimal totalRTG = 0;
                    if (ecCommercialHeader.Entities.Count > 0)
                    {
                        foreach (Entity enCommercialHeader in ecCommercialHeader.Entities)
                        {
                            if (enCommercialHeader.Contains("trs_totalrtg"))
                            {
                                totalRTG += (decimal)enCommercialHeader["trs_totalrtg"];
                            }

                        }
                        startDate = startDate.AddHours(Convert.ToDouble(totalRTG));
                        _DL_serviceappointment = new DL_serviceappointment();
                        //_DL_serviceappointment.trs_estimationhour = totalRTG;
                        _DL_serviceappointment.trs_estimationhour = _fMath.RoundModeUp(totalRTG, 1);
                        _DL_serviceappointment.scheduledend = startDate;
                        _DL_serviceappointment.Update(organizationService, id);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".CalculateRTG : " + ex.Message);
            }
        }

        //Added by Santony - 4/1/2015 (Menghitung Total RTG  di Commercial Detail)
        public void CalculateRTG_CommercialDetail(IOrganizationService organizationService, Guid WOId)
        {
            try
            {
                Guid ActivityId = Guid.Empty;
                decimal TotalRTGDetail = 0;

                QueryExpression qeTask = new QueryExpression(_DL_task.EntityName);
                qeTask.ColumnSet = new ColumnSet(true);
                FilterExpression feTask = qeTask.Criteria.AddFilter(LogicalOperator.And);
                feTask.AddCondition("trs_operationid", ConditionOperator.Equal, WOId);
                EntityCollection ecTask = _DL_task.Select(organizationService, qeTask);

                if (ecTask.Entities.Count > 0)
                {
                    foreach (var itemTask in ecTask.Entities)
                    {
                        if (itemTask.Attributes.Contains("activityid") && itemTask.Attributes["activityid"] != null)
                            ActivityId = ((Guid)itemTask.Attributes["activityid"]);
                    }
                }

                QueryExpression qeCommercialDetail = new QueryExpression(_DL_trs_commercialdetail.EntityName);
                qeCommercialDetail.ColumnSet = new ColumnSet(true);
                FilterExpression feCommercialDetail = qeCommercialDetail.Criteria.AddFilter(LogicalOperator.And);
                feCommercialDetail.AddCondition("trs_workorder", ConditionOperator.Equal, WOId);
                feCommercialDetail.AddCondition("trs_commercialheaderid", ConditionOperator.Equal, ActivityId);
                EntityCollection ecCommercialDetail = _DL_trs_commercialdetail.Select(organizationService, qeCommercialDetail);

                if (ecCommercialDetail.Entities.Count > 0)
                {
                    foreach (var itemCommercialDetail in ecCommercialDetail.Entities)
                    {
                        if (itemCommercialDetail.Attributes.Contains("trs_rtg") && itemCommercialDetail.Attributes["trs_rtg"] != null)
                            TotalRTGDetail += (decimal)itemCommercialDetail["trs_rtg"];
                    }
                }

                _DL_task = new DL_task();
                _DL_task.trs_totalrtg = TotalRTGDetail;
                _DL_task.Update(organizationService, ActivityId);
            }
            catch (Exception ex)
            {
                
                throw new Exception(_classname + ".CalculateRTG_CommercialDetail : " + ex.Message);
            }
        }

        public void SummarizeParts(IOrganizationService organizationService
            , Guid id
            , bool isContract
            , Guid? contractLineId = null
            , decimal discountPercent = 0)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression();
                Entity ePart = new Entity();
                EntityCollection ecolContractPart = new EntityCollection();
                Entity eContractPart = new Entity();
                EntityCollection ecolPartPrice = new EntityCollection();
                Entity ePartPrice = new Entity();
                EntityCollection ecolPartsSummary = new EntityCollection();
                Entity ePartsSummary = new Entity();
                EntityCollection ecolMaxItemNumber = new EntityCollection();
                Entity eMaxItemNumber = new Entity();
                int itemNumber = 0;
                int quantity = 0;
                Guid? currency = null;
                Money price = new Money(0);
                FilterExpression filterExpression = new FilterExpression();

                if (!isContract)
                {
                    queryExpression = new QueryExpression(_DL_incident.EntityName);
                    queryExpression.ColumnSet = new ColumnSet(true);

                    LinkEntity leServiceAppointment = new LinkEntity();
                    leServiceAppointment.LinkFromEntityName = _DL_incident.EntityName;
                    leServiceAppointment.LinkFromAttributeName = "incidentid";
                    leServiceAppointment.LinkToEntityName = _DL_serviceappointment.EntityName;
                    leServiceAppointment.LinkToAttributeName = "regardingobjectid";
                    leServiceAppointment.JoinOperator = JoinOperator.Inner;
                    leServiceAppointment.EntityAlias = "serviceappointment";
                    leServiceAppointment.Columns = new ColumnSet(true);
                    leServiceAppointment.LinkCriteria.AddCondition("activityid", ConditionOperator.Equal, id);

                    LinkEntity leContractDetail = new LinkEntity();
                    leContractDetail.LinkFromEntityName = _DL_serviceappointment.EntityName;
                    leContractDetail.LinkFromAttributeName = "activityid";
                    leContractDetail.LinkToEntityName = _DL_contractdetail.EntityName;
                    leContractDetail.LinkToAttributeName = "contractdetailid";
                    leContractDetail.JoinOperator = JoinOperator.Inner;
                    leContractDetail.EntityAlias = "contractdetail";
                    leContractDetail.Columns = new ColumnSet(true);

                    leServiceAppointment.LinkEntities.Add(leContractDetail);
                    queryExpression.LinkEntities.Add(leServiceAppointment);

                    EntityCollection ecolIncident = _DL_incident.Select(organizationService, queryExpression);
                    if (ecolIncident.Entities.Count > 0)
                    {
                        Entity eIncident = ecolIncident.Entities[0];
                        if (eIncident.Attributes.Contains("serviceappointment.contractdetailid"))
                        {
                            isContract = true;
                            contractLineId = (Guid)(eIncident.GetAttributeValue<AliasedValue>("serviceappointment.contractdetailid")).Value;
                            if (eIncident.Attributes.Contains("contractdetail.trs_partsdiscount"))
                            {
                                discountPercent = (decimal)(eIncident.GetAttributeValue<AliasedValue>("contractdetail.trs_partsdiscount")).Value;
                            }
                        }
                    }
                }

                FetchExpression fetchExpression = new FetchExpression(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' aggregate='true'>
                                                                              <entity name='trs_workorderpartdetail'>
                                                                                <attribute name='trs_quantity' alias='trs_quantity_sum' aggregate='sum' />
                                                                                <attribute name='trs_partnumber' alias='trs_partnumber' groupby='true' />
                                                                                <filter type='and'>
                                                                                  <condition attribute='trs_workorder' operator='eq' uitype='serviceappointment' value='{" + id.ToString() + @"}' />
                                                                                  <condition attribute='statecode' operator='eq' value='0' />
                                                                                  <condition attribute='statuscode' operator='eq' value='1' />
                                                                                </filter>
                                                                              </entity>
                                                                            </fetch>");
                EntityCollection entityCollection = organizationService.RetrieveMultiple(fetchExpression);
                foreach (Entity entity in entityCollection.Entities)
                {
                    ePart = _DL_trs_masterpart.Select(organizationService, ((EntityReference)entity.GetAttributeValue<AliasedValue>("trs_partnumber").Value).Id);
                    quantity = (int)entity.GetAttributeValue<AliasedValue>("trs_quantity_sum").Value;

                    //Get Price
                    currency = null;
                    price = new Money(0);

                    if (isContract)
                    {
                        queryExpression = new QueryExpression(_DL_trs_contractpart.EntityName);
                        queryExpression.ColumnSet = new ColumnSet(true);
                        filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                        filterExpression.AddCondition("trs_contractlineid", ConditionOperator.Equal, contractLineId);
                        filterExpression.AddCondition("trs_partmaster", ConditionOperator.Equal, ePart.Id);
                        ecolContractPart = _DL_trs_contractpart.Select(organizationService, queryExpression);
                        if (ecolContractPart.Entities.Count > 0)
                        {
                            eContractPart = ecolContractPart.Entities[0];
                            currency = eContractPart.GetAttributeValue<EntityReference>("transactioncurrencyid").Id;
                            price = eContractPart.GetAttributeValue<Money>("trs_price");
                        }
                    }

                    if (currency == null)
                    {
                        queryExpression = new QueryExpression(_DL_trs_partpricemaster.EntityName);
                        queryExpression.ColumnSet = new ColumnSet(true);

                        LinkEntity lePriceListCPO = new LinkEntity();
                        lePriceListCPO.LinkFromEntityName = _DL_trs_partpricemaster.EntityName;
                        lePriceListCPO.LinkFromAttributeName = "trs_pricelist";
                        lePriceListCPO.LinkToEntityName = _DL_new_pricelistcpo.EntityName;
                        lePriceListCPO.LinkToAttributeName = "new_pricelistcpoid";
                        lePriceListCPO.JoinOperator = JoinOperator.Inner;
                        lePriceListCPO.LinkCriteria.AddCondition("new_code", ConditionOperator.Equal, "P1");

                        queryExpression.LinkEntities.Add(lePriceListCPO);
                        queryExpression.Criteria.AddCondition("trs_partmaster", ConditionOperator.Equal, ePart.Id);
                        ecolPartPrice = _DL_trs_partpricemaster.Select(organizationService, queryExpression);
                        if (ecolPartPrice.Entities.Count > 0)
                        {
                            ePartPrice = ecolPartPrice.Entities[0];
                            currency = ePartPrice.GetAttributeValue<EntityReference>("transactioncurrencyid").Id;
                            price = ePartPrice.GetAttributeValue<Money>("trs_price");
                        }
                    }

                    queryExpression = new QueryExpression(_DL_trs_workorderpartssummary.EntityName);
                    queryExpression.ColumnSet = new ColumnSet(true);
                    filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                    filterExpression.AddCondition("trs_workorder", ConditionOperator.Equal, id);
                    filterExpression.AddCondition("trs_partnumber", ConditionOperator.Equal, ePart.Id);
                    ecolPartsSummary = _DL_trs_workorderpartssummary.Select(organizationService, queryExpression);
                    if (ecolPartsSummary.Entities.Count > 0)
                    {
                        ePartsSummary = ecolPartsSummary.Entities[0];
                        _DL_trs_workorderpartssummary = new DL_trs_workorderpartssummary();
                        _DL_trs_workorderpartssummary.trs_tasklistquantity = quantity;
                        if (currency != null)
                        {
                            _DL_trs_workorderpartssummary.transactioncurrencyid = (Guid)currency;
                            _DL_trs_workorderpartssummary.trs_price = price;
                        }
                        _DL_trs_workorderpartssummary.Update(organizationService, ePartsSummary.Id);
                    }
                    else
                    {
                        fetchExpression = new FetchExpression(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' aggregate='true'>
                                                                      <entity name='trs_workorderpartssummary'>
                                                                        <attribute name='trs_itemnumber' alias='trs_itemnumber' aggregate='max' />
                                                                        <filter type='and'>
                                                                          <condition attribute='trs_workorder' operator='eq' uitype='serviceappointment' value='{" + id.ToString() + @"}' />
                                                                        </filter>
                                                                      </entity>
                                                                    </fetch>");
                        ecolMaxItemNumber = organizationService.RetrieveMultiple(fetchExpression);
                        if (ecolMaxItemNumber.Entities.Count > 0)
                        {
                            eMaxItemNumber = ecolMaxItemNumber.Entities[0];
                            if (eMaxItemNumber.Attributes.Contains("trs_itemnumber"))
                                itemNumber = (int)eMaxItemNumber.GetAttributeValue<AliasedValue>("trs_itemnumber").Value + 1;
                            else
                                itemNumber = 1;
                        }
                        else
                        {
                            itemNumber = 1;
                        }

                        _DL_trs_workorderpartssummary = new DL_trs_workorderpartssummary();
                        _DL_trs_workorderpartssummary.trs_workorder = id;
                        _DL_trs_workorderpartssummary.trs_itemnumber = itemNumber;
                        _DL_trs_workorderpartssummary.trs_partnumber = ePart.Id;
                        _DL_trs_workorderpartssummary.trs_partname = ePart.GetAttributeValue<string>("trs_partdescription");
                        _DL_trs_workorderpartssummary.trs_tasklistquantity = quantity;
                        if (currency != null)
                        {
                            _DL_trs_workorderpartssummary.transactioncurrencyid = (Guid)currency;
                            _DL_trs_workorderpartssummary.trs_price = price;
                            _DL_trs_workorderpartssummary.trs_totalprice = new Money(quantity * price.Value);
                            if (isContract)
                            {
                                _DL_trs_workorderpartssummary.trs_discountpercent = discountPercent;
                                _DL_trs_workorderpartssummary.trs_discountamount = new Money(Math.Round(_DL_trs_workorderpartssummary.trs_totalprice.Value * discountPercent / 100, 2));
                                _DL_trs_workorderpartssummary.trs_totalprice = new Money(_DL_trs_workorderpartssummary.trs_totalprice.Value - _DL_trs_workorderpartssummary.trs_discountamount.Value);
                            }
                        }
                        _DL_trs_workorderpartssummary.Insert(organizationService);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SummarizeParts : " + ex.Message);
            }
        }

        public void SummarizeToolGroups(IOrganizationService organizationService, Guid id)
        {
            try
            {
                List<Guid> WOToolGroups = new List<Guid>();
                Guid WOToolGroup;

                QueryExpression queryExpression = new QueryExpression(_DL_trs_workordertoolsrecommendation.EntityName);
                queryExpression.ColumnSet = new ColumnSet(true);
                queryExpression.Criteria.AddCondition("trs_workorder", ConditionOperator.Equal, id);
                EntityCollection ecolWOTools = _DL_trs_workordertoolsrecommendation.Select(organizationService, queryExpression);
                foreach (Entity eWOTools in ecolWOTools.Entities)
                {
                    WOToolGroups.Add(eWOTools.GetAttributeValue<EntityReference>("trs_toolsgroupid").Id);
                }

                FetchExpression fetchExpression = new FetchExpression(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' aggregate='true'>
                                                                          <entity name='trs_commercialdetail'>
                                                                            <filter type='and'>
                                                                              <condition attribute='trs_workorder' operator='eq' uitype='serviceappointment' value='{" + id.ToString() + @"}' />
                                                                            </filter>
                                                                            <link-entity name='trs_trs_commercialdetail_trs_toolsgroup' from='trs_commercialdetailid' to='trs_commercialdetailid' alias='trs_commercialdetailtools'>
                                                                              <attribute name='trs_toolsgroupid' alias='trs_toolsgroupid' groupby='true' />
                                                                            </link-entity>
                                                                          </entity>
                                                                        </fetch>");
                EntityCollection entityCollection = organizationService.RetrieveMultiple(fetchExpression);
                foreach (Entity entity in entityCollection.Entities)
                {
                    WOToolGroup = (Guid)entity.GetAttributeValue<AliasedValue>("trs_toolsgroupid").Value;
                    if (WOToolGroups.Exists(x => x == WOToolGroup)) { }
                    else
                    {
                        _DL_trs_workordertoolsrecommendation = new DL_trs_workordertoolsrecommendation();
                        _DL_trs_workordertoolsrecommendation.trs_workorder = id;
                        _DL_trs_workordertoolsrecommendation.trs_toolsgroupid = WOToolGroup;
                        _DL_trs_workordertoolsrecommendation.Insert(organizationService);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SummarizeToolGroups : " + ex.Message);
            }
        }

        public void Release(IOrganizationService organizationService, Guid id)
        {
            try
            {
                _DL_serviceappointment.Released(organizationService, id);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Release : " + ex.Message);
            }
        }

        public Guid GetMechanicLeader(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = _DL_serviceappointment.Select(organizationService, id);
                if (entity.Attributes.Contains("trs_mechanicleader"))
                    return entity.GetAttributeValue<EntityReference>("trs_mechanicleader").Id;
                else
                    throw new Exception("Can not found Mechanic Leader.");
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".GetMechanicLeader : " + ex.Message);
            }
        }
        #endregion
    }
}