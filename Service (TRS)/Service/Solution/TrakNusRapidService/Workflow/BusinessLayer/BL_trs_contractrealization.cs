using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using TrakNusRapidService.DataLayer;
using TrakNusRapidService.Helper;

namespace TrakNusRapidService.Workflow.BusinessLayer
{
    public class BL_trs_contractrealization
    {
        #region Constants
        private const string _classname = "BL_trs_contractrealization";
        private const int _accountIndicator = 4;
        private const string _service = "Contract Periodical";
        private const string _pmActType = "013";
        private const string _actType = "SERV";
        private const string _workCenter = "SERVICE";
        private const string _profitCenter = "SERVICE";
        #endregion

        #region Depedencies
        private DL_trs_contractrealization _DL_trs_contractrealization = new DL_trs_contractrealization();
        private DL_trs_contractservice _DL_trs_contractservice = new DL_trs_contractservice();
        private DL_contractdetail _DL_contractdetail = new DL_contractdetail();
        private DL_trs_contractmechanic _DL_trs_contractmechanic = new DL_trs_contractmechanic();
        private DL_new_population _DL_new_population = new DL_new_population();
        private DL_account _DL_account = new DL_account();
        private DL_team _DL_team = new DL_team();
        private DL_serviceappointment _DL_serviceappointment = new DL_serviceappointment();

        private SRGenerator _srGenerator = new SRGenerator();
        private WOGenerator _woGenerator = new WOGenerator();
        #endregion

        #region Privates
        private Guid GetTeamId(IOrganizationService organizationService, Guid branchId)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_team.EntityName);
                queryExpression.Criteria.AddCondition("businessunitid", ConditionOperator.Equal, branchId);
                EntityCollection entityCollection = _DL_team.Select(organizationService, queryExpression);
                if (entityCollection.Entities.Count > 0)
                {
                    return entityCollection.Entities[0].Id;
                }
                else
                {
                    throw new InvalidWorkflowException("Can not found Team for Branch Id : '" + branchId.ToString() + "'");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(_classname + ".GetTeamId : " + ex.Message);
            }
        }
        #endregion

        #region Publics
        public void GenerateActivity(IOrganizationService organizationService, ITracingService tracingService, Guid id)
        {
            Guid? incidentId = null;
            Guid? woId = null;
            bool success = false;
            Guid? branchId = null;
            Guid? teamId = null;
            try
            {
                Entity entity = _DL_trs_contractrealization.Select(organizationService, id);
                if (entity.Attributes.Contains("trs_success"))
                    success = entity.GetAttributeValue<bool>("trs_success");
                if (!success)
                {
                    if (entity.Attributes.Contains("trs_incidentid"))
                        incidentId = entity.GetAttributeValue<EntityReference>("trs_incidentid").Id;
                    if (entity.Attributes.Contains("trs_workorderid"))
                        woId = entity.GetAttributeValue<EntityReference>("trs_workorderid").Id;

                    string topic = string.Empty;
                    if (entity.Attributes.Contains("trs_description"))
                        topic = entity.GetAttributeValue<string>("trs_description");
                    Guid populationId;
                    if (entity.Attributes.Contains("trs_populationid"))
                        populationId = entity.GetAttributeValue<EntityReference>("trs_populationid").Id;
                    else
                        throw new InvalidWorkflowException("Population can not empty !");
                    Guid contractId;
                    if (entity.Attributes.Contains("trs_contractid"))
                        contractId = entity.GetAttributeValue<EntityReference>("trs_contractid").Id;
                    else
                        throw new InvalidWorkflowException("Contract can not empty !");
                    DateTime maintenanceTime;
                    if (entity.Attributes.Contains("trs_maintenancetime"))
                        maintenanceTime = entity.GetAttributeValue<DateTime>("trs_maintenancetime").ToLocalTime();
                    else
                        throw new InvalidWorkflowException("Maintenance Time can not empty !");

                    Entity eContractDetail = _DL_contractdetail.Select(organizationService, entity.GetAttributeValue<EntityReference>("trs_contractdetailid").Id);
                    Guid customerId;
                    if (eContractDetail.Attributes.Contains("trs_customer"))
                        customerId = eContractDetail.GetAttributeValue<EntityReference>("trs_customer").Id;
                    else
                        throw new InvalidWorkflowException("Please fill Customer in Contract Detail !");
                    Guid contactId;
                    if (eContractDetail.Attributes.Contains("trs_contact"))
                        contactId = eContractDetail.GetAttributeValue<EntityReference>("trs_contact").Id;
                    else
                        throw new InvalidWorkflowException("Please fill Contact in Contract Detail !");
                    Guid cponsiteId;
                    if (eContractDetail.Attributes.Contains("trs_contactpersononsite"))
                        cponsiteId = eContractDetail.GetAttributeValue<EntityReference>("trs_contactpersononsite").Id;
                    else
                        throw new InvalidWorkflowException("Please fill Contact Person on Site in Contract Detail !");
                    decimal partDiscount = 0;
                    if (eContractDetail.Attributes.Contains("trs_partsdiscount"))
                        partDiscount = eContractDetail.GetAttributeValue<decimal>("trs_partsdiscount");

                    #region SR
                    if (incidentId == null)
                    {
                        _srGenerator = new SRGenerator();
                        incidentId = _srGenerator.GenerateCase(organizationService
                                            , populationId
                                            , customerId
                                            , contactId
                                            , topic
                                            , _pmActType
                                            , contractId
                                            , eContractDetail.Id);
                    }
                    #endregion

                    #region WO
                    if (incidentId != null)
                    {
                        _woGenerator = new WOGenerator();
                        //Add Mechanic
                        QueryExpression queryExpression = new QueryExpression(_DL_trs_contractmechanic.EntityName);
                        queryExpression.ColumnSet = new ColumnSet(true);
                        FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                        filterExpression.AddCondition("trs_contractdetail", ConditionOperator.Equal, eContractDetail.Id);
                        EntityCollection ecolContractMechanic = new EntityCollection(); ;
                        foreach (Entity eContractMechanic in ecolContractMechanic.Entities)
                        {
                            _woGenerator.AddMechanic(eContractMechanic.GetAttributeValue<EntityReference>("trs_equipment").Id);
                        }

                        //Generate WO
                        int statuscode = Configuration.WOStatus_New;
                        if (woId == null)
                        {
                            woId = _woGenerator.GenerateWO(organizationService
                                            , out branchId
                                            , (Guid)incidentId
                                            , populationId
                                            , customerId
                                            , topic
                                            , _service
                                            , _accountIndicator
                                            , _pmActType
                                            , _actType
                                            , _workCenter
                                            , _profitCenter
                                            , contactId
                                            , cponsiteId
                                            , ScheduledStartTime: maintenanceTime
                                            , ScheduledEndTime: maintenanceTime.AddMinutes(1));
                        }
                        else
                        {
                            Entity eWo = _DL_serviceappointment.Select(organizationService, (Guid)woId);
                            statuscode = eWo.GetAttributeValue<int>("statuscode");
                        }

                        if (woId != null && !success && statuscode == Configuration.WOStatus_New)
                        {
                            if (branchId != null)
                            {
                                teamId = GetTeamId(organizationService, (Guid)branchId);
                                _srGenerator.AssignCase(organizationService, (Guid)incidentId, (Guid)teamId, true);
                                _woGenerator.AssignWO(organizationService, (Guid)woId, (Guid)teamId, true);
                            }

                            if (entity.Attributes.Contains("trs_contractserviceid"))
                            {
                                //Add Commercial Task
                                Entity eContractService = _DL_trs_contractservice.Select(organizationService, entity.GetAttributeValue<EntityReference>("trs_contractserviceid").Id);
                                if (eContractService.Attributes.Contains("trs_header"))
                                {
                                    Guid? commercialHeaderId = _woGenerator.AddCommercialHeader(organizationService
                                                                                                    , (Guid)woId
                                                                                                    , eContractService.GetAttributeValue<EntityReference>("trs_header").Id
                                                                                                    , eContractDetail.Attributes.Contains("trs_serviceprice") ? eContractDetail.GetAttributeValue<Money>("trs_serviceprice").Value : 0);
                                    if (commercialHeaderId != null)
                                        _woGenerator.AssignCommercialHeader(organizationService, (Guid)commercialHeaderId, (Guid)teamId, true);
                                }
                                else
                                    throw new InvalidWorkflowException("Can not found the task.");

                                //Calculate RTG
                                _woGenerator.CalculateRTG(organizationService, (Guid)woId);

                                //Summarize Parts and Tools
                                _woGenerator.SummarizeParts(organizationService, (Guid)woId, true, eContractDetail.Id, partDiscount);
                                _woGenerator.SummarizeToolGroups(organizationService, (Guid)woId);

                                //Release WO
                                _woGenerator.Release(organizationService, (Guid)woId);
                            }
                            else
                            {
                                throw new InvalidWorkflowException("Failed to get Task Id.");
                            }

                            _DL_trs_contractrealization = new DL_trs_contractrealization();
                            _DL_trs_contractrealization.trs_incidentid = (Guid)incidentId;
                            _DL_trs_contractrealization.trs_workorderid = (Guid)woId;
                            _DL_trs_contractrealization.trs_failedreason = string.Empty;
                            _DL_trs_contractrealization.trs_success = true;
                            _DL_trs_contractrealization.Update(organizationService, id);
                        }
                        else
                        {
                            throw new InvalidWorkflowException("Failed to generate WO (Header).");
                        }
                    }
                    else
                    {
                        throw new InvalidWorkflowException("Failed to generate SR.");
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                _DL_trs_contractrealization = new DL_trs_contractrealization();
                if (incidentId != null)
                    _DL_trs_contractrealization.trs_incidentid = (Guid)incidentId;
                if (woId != null)
                    _DL_trs_contractrealization.trs_workorderid = (Guid)woId;
                _DL_trs_contractrealization.trs_failedreason = _classname + ".GenerateActivity : " + ex.Message;
                _DL_trs_contractrealization.Update(organizationService, id);
            }
        }
        #endregion
    }
}
