using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using TrakNusRapidService.DataLayer;
using TrakNusRapidService.Helper;
using TrakNusRapidService.Helper.MobileWebService;

using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Client;

namespace TrakNusRapidService.Workflow.BusinessLayer
{
    public class BL_serviceappointment
    {
        #region Constants
        private const string _classname = "BL_serviceappointment";
        private const int _depth = 1;
        private const int _participationtypemask_customer = 11;
        private const int _partyobjecttypecode_account = 1;
        private const string trPartHeader = "H";
        private const string trPartDetail = "D";

        private enum TrType
        {
            Create = 1,
            Change = 2,
            Release = 3,
            AssignMechanic = 4,
            Confirmation = 5,
            PartConsume = 6,
            TECO = 9,
            CancelWO = 0
        }

        #endregion

        #region Depedencies
        private DL_serviceappointment _DL_serviceappointment = new DL_serviceappointment();
        private DL_activityparty _DL_activityparty = new DL_activityparty();
        private DL_task _DL_task = new DL_task();
        private DL_trs_commercialdetail _DL_trs_commercialdetail = new DL_trs_commercialdetail();
        private DL_account _DL_account = new DL_account();
        private DL_trs_plant _DL_trs_plant = new DL_trs_plant();
        private DL_trs_workcenter _DL_trs_workcenter = new DL_trs_workcenter();
        private DL_new_population _DL_new_population = new DL_new_population();
        private DL_trs_functionallocation _DL_trs_functionallocation = new DL_trs_functionallocation();
        private DL_trs_profitcenter _DL_trs_profitcenter = new DL_trs_profitcenter();
        private DL_trs_responsiblecostcenter _DL_trs_responsiblecostcenter = new DL_trs_responsiblecostcenter();
        private DL_trs_tasklistgroup _DL_trs_tasklistgroup = new DL_trs_tasklistgroup();
        private DL_trs_tasklistheader _DL_trs_tasklistheader = new DL_trs_tasklistheader();
        private DL_trs_tasklisttype _DL_trs_tasklisttype = new DL_trs_tasklisttype();
        private DL_trs_workorderpartdetail _DL_trs_workorderpartdetail = new DL_trs_workorderpartdetail();
        private DL_trs_workordersupportingmaterial _DL_trs_workordersupportingmaterial = new DL_trs_workordersupportingmaterial();
        private DL_trs_toolsmaster _DL_trs_toolsmaster = new DL_trs_toolsmaster();
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        private DL_trs_mtar _DL_trs_mtar = new DL_trs_mtar();
        private DL_equipment _DL_equipment = new DL_equipment();
        private DL_businessunit _DL_businessunit = new DL_businessunit();
        private DL_trs_acttype _DL_trs_acttype = new DL_trs_acttype();
        private DL_transactioncurrency _DL_transactioncurrency = new DL_transactioncurrency();
        private DL_trs_paymentterm _DL_trs_paymentterm = new DL_trs_paymentterm();
        private DL_trs_quotationcommercialheader _DL_trs_quotationcommercialheader = new DL_trs_quotationcommercialheader();
        private DL_trs_quotationpartdetail _DL_trs_quotationpartdetail = new DL_trs_quotationpartdetail();
        private DL_trs_quotationsupportingmaterial _DL_trs_quotationsupportingmaterial = new DL_trs_quotationsupportingmaterial();
        private DL_trs_toolsusage _DL_trs_toolsusage = new DL_trs_toolsusage();
        private DL_trs_commercialdetailmechanic _DL_trs_commercialdetailmechanic = new DL_trs_commercialdetailmechanic();
        private DL_trs_workordersecondman _DL_trs_workordersecondman = new DL_trs_workordersecondman();
        private DL_trs_masterpart _DL_trs_masterpart = new DL_trs_masterpart();
        private DL_trs_workorderpartssummary _DL_trs_workorderpartssummary = new DL_trs_workorderpartssummary();
        private DL_trs_workordertoolsrecommendation _DL_trs_workordertoolsrecommendation = new DL_trs_workordertoolsrecommendation();

        private FMath _fMath = new FMath();
        private WOGenerator _woGenerator = new WOGenerator();
        private MTARGenerator _mtarGenerator = new MTARGenerator();
        private FSAP _fSAP = new FSAP();
        #endregion

        #region Privates
        /*
        private void CreateMTAR(IOrganizationService organizationService, Guid id, int mtarStatus, bool workshop)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_activityparty.EntityName);
                queryExpression.ColumnSet = new ColumnSet(true);
                FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                filterExpression.AddCondition("activityid", ConditionOperator.Equal, id);
                filterExpression.AddCondition("participationtypemask", ConditionOperator.Equal, 10);
                filterExpression.AddCondition("partyobjecttypecode", ConditionOperator.Equal, 4000);

                EntityCollection entityCollection = _DL_activityparty.Select(organizationService, queryExpression);
                foreach (Entity entity in entityCollection.Entities)
                {
                    _DL_trs_mtar = new DL_trs_mtar();
                    _DL_trs_mtar.trs_name = _fMobile.ConvertMTARtoWords(mtarStatus);
                    _DL_trs_mtar.trs_workorder = id;
                    _DL_trs_mtar.trs_mechanic = entity.GetAttributeValue<EntityReference>("partyid").Id;
                    _DL_trs_mtar.trs_mtarstatus = mtarStatus;
                    switch(mtarStatus)
                    {
                        case Configuration.MTAR_Hold:
                            _DL_trs_mtar.trs_statusremarks = "Hold by SDH";
                            break;
                        case Configuration.MTAR_Resume:
                            _DL_trs_mtar.trs_statusremarks = "Unhold by SDH";
                            break;
                    }
                    _DL_trs_mtar.trs_longitude = Configuration.TNGPSLongitude;
                    _DL_trs_mtar.trs_latitude = Configuration.TNGPSLatitude;
                    _DL_trs_mtar.trs_automatictime = DateTime.Now;
                    _DL_trs_mtar.trs_frommobile = false;
                    _DL_trs_mtar.trs_workshop = workshop;
                    _DL_trs_mtar.trs_updatewostatus = true;
                    _DL_trs_mtar.Insert(organizationService);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".CreateMTAR : " + ex.Message.ToString());
            }
        }
         * */

        public void UpdateMechanictoOpenTask(IOrganizationService organizationService, Guid id, Guid mechanicLeader)
        {
            try
            {
                List<Guid> mechanic = new List<Guid>();
                List<Guid> mechanicSecondman = new List<Guid>();

                //Get Secondman List
                QueryExpression queryExpression = new QueryExpression(_DL_trs_workordersecondman.EntityName);
                queryExpression.ColumnSet = new ColumnSet(true);
                queryExpression.Criteria.AddCondition("trs_activityid", ConditionOperator.Equal, id);

                EntityCollection entityCollection = _DL_trs_workordersecondman.Select(organizationService, queryExpression);
                foreach (Entity entity in entityCollection.Entities)
                {
                    mechanicSecondman.Add(entity.GetAttributeValue<EntityReference>("trs_equipmentid").Id);
                }

                //Get Open Commercial Detail
                queryExpression = new QueryExpression(_DL_serviceappointment.EntityName);
                LinkEntity lCommercialHeader = new LinkEntity();
                lCommercialHeader.LinkFromEntityName = _DL_serviceappointment.EntityName;
                lCommercialHeader.LinkFromAttributeName = "activityid";
                lCommercialHeader.LinkToEntityName = _DL_task.EntityName;
                lCommercialHeader.LinkToAttributeName = "trs_operationid";
                lCommercialHeader.JoinOperator = JoinOperator.Inner;

                LinkEntity lCommercialDetail = new LinkEntity();
                lCommercialDetail.LinkFromEntityName = _DL_task.EntityName;
                lCommercialDetail.LinkFromAttributeName = "activityid";
                lCommercialDetail.LinkToEntityName = _DL_trs_commercialdetail.EntityName;
                lCommercialDetail.LinkToAttributeName = "trs_commercialheaderid";
                lCommercialDetail.JoinOperator = JoinOperator.Inner;
                lCommercialDetail.EntityAlias = "commercialdetail";
                lCommercialDetail.Columns.AddColumns("trs_commercialdetailid", "statuscode");
                lCommercialDetail.LinkCriteria.AddCondition("statuscode", ConditionOperator.NotEqual, 167630000);

                lCommercialHeader.LinkEntities.Add(lCommercialDetail);
                queryExpression.LinkEntities.Add(lCommercialHeader);
                queryExpression.Criteria.AddCondition("activityid", ConditionOperator.Equal, id);

                EntityCollection ecolCommercialDetail = _DL_serviceappointment.Select(organizationService, queryExpression);
                foreach (Entity eCommercialDetail in ecolCommercialDetail.Entities)
                {
                    //Get Existing Mechanic
                    mechanic = new List<Guid>();
                    queryExpression = new QueryExpression(_DL_trs_commercialdetailmechanic.EntityName);
                    queryExpression.ColumnSet = new ColumnSet(true);
                    queryExpression.Criteria.AddCondition("trs_commercialdetailid", ConditionOperator.Equal, (Guid)eCommercialDetail.GetAttributeValue<AliasedValue>("commercialdetail.trs_commercialdetailid").Value);
                    EntityCollection ecolCommercialDetailMechanic = _DL_trs_commercialdetailmechanic.Select(organizationService, queryExpression);
                    foreach (Entity eCommercialDetailMechanic in ecolCommercialDetailMechanic.Entities)
                    {
                        mechanic.Add(eCommercialDetailMechanic.GetAttributeValue<EntityReference>("trs_equipmentid").Id);
                    }

                    //Get New Mechanic
                    queryExpression = new QueryExpression(_DL_activityparty.EntityName);
                    queryExpression.ColumnSet = new ColumnSet(true);

                    LinkEntity lEquipment = new LinkEntity();
                    lEquipment.LinkFromEntityName = _DL_activityparty.EntityName;
                    lEquipment.LinkFromAttributeName = "partyid";
                    lEquipment.LinkToEntityName = _DL_equipment.EntityName;
                    lEquipment.LinkToAttributeName = "equipmentid";
                    lEquipment.JoinOperator = JoinOperator.Inner;
                    lEquipment.EntityAlias = "equipment";
                    lEquipment.Columns = new ColumnSet(true);
                    queryExpression.LinkEntities.Add(lEquipment);

                    FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                    filterExpression.AddCondition("activityid", ConditionOperator.Equal, id);
                    filterExpression.AddCondition("participationtypemask", ConditionOperator.Equal, 10);
                    filterExpression.AddCondition("partyobjecttypecode", ConditionOperator.Equal, 4000);
                    EntityCollection ecolMechanic = _DL_activityparty.Select(organizationService, queryExpression);

                    foreach (Entity eMechanic in ecolMechanic.Entities)
                    {
                        if (mechanic.Exists(x => x == eMechanic.GetAttributeValue<EntityReference>("partyid").Id)) { }
                        else
                        {
                            //Insert into Commercial Detail (Mechanic)
                            _DL_trs_commercialdetailmechanic = new DL_trs_commercialdetailmechanic();
                            _DL_trs_commercialdetailmechanic.trs_commercialdetailid = new Guid(eCommercialDetail.GetAttributeValue<AliasedValue>("commercialdetail.trs_commercialdetailid").Value.ToString());
                            _DL_trs_commercialdetailmechanic.trs_nrp = eMechanic.GetAttributeValue<AliasedValue>("equipment.trs_nrp").Value.ToString();
                            _DL_trs_commercialdetailmechanic.trs_equipmentid = eMechanic.GetAttributeValue<EntityReference>("partyid").Id;
                            if (_DL_trs_commercialdetailmechanic.trs_equipmentid == mechanicLeader)
                                _DL_trs_commercialdetailmechanic.trs_mechanicrole = Configuration.MechanicRole_Leader;
                            else
                            {
                                if (mechanicSecondman.Exists(x => x == _DL_trs_commercialdetailmechanic.trs_equipmentid))
                                    _DL_trs_commercialdetailmechanic.trs_mechanicrole = Configuration.MechanicRole_Secondman;
                                else
                                    _DL_trs_commercialdetailmechanic.trs_mechanicrole = Configuration.MechanicRole_Member;
                            }
                            _DL_trs_commercialdetailmechanic.Insert(organizationService);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(_classname + ".UpdateMechanictoOpenTask : " + ex.Message);
            }
        }
        #endregion

        #region Events
        public EntityCollection getAllServiceAppointment(IOrganizationService organizationService, Guid Id)
        {
            try
            {
                QueryExpression qe = new QueryExpression(_DL_serviceappointment.EntityName);
                qe.ColumnSet = new ColumnSet(true);

                ConditionExpression ce = new ConditionExpression();
                ce.AttributeName = "activityid";
                ce.Operator = ConditionOperator.Equal;
                ce.Values.Add(Id.ToString());

                qe.Criteria.AddCondition(ce);

                return _DL_serviceappointment.Select(organizationService, qe);
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(ex.ToString());
            }
        }

        public void Hold(IOrganizationService organizationService, ITracingService tracingService, Guid id)
        {
            try
            {
                //Update status diganti dari MTAR
                /*_DL_serviceappointment = new DL_serviceappointment();
                _DL_serviceappointment.trs_frommobile = false;
                _DL_serviceappointment.Update(organizationService, id);

                _DL_serviceappointment.Hold(organizationService, id);*/
                Entity entity = _DL_serviceappointment.Select(organizationService, id);
                if (entity.Attributes.Contains("trs_isdispatched") && entity.GetAttributeValue<bool>("trs_isdispatched"))
                {
                    //CreateMTAR(organizationService, id, Configuration.MTAR_Hold, entity.GetAttributeValue<bool>("trs_workshop"));
                    _mtarGenerator.GenerateMTARforAllMechanic(organizationService, id, Configuration.MTAR_Hold, entity.GetAttributeValue<bool>("trs_workshop"), true);
                }
                else
                {
                    //CreateMTAR(organizationService, id, Configuration.MTAR_Hold, true);
                    _mtarGenerator.GenerateMTARforAllMechanic(organizationService, id, Configuration.MTAR_Hold, true, true);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(_classname + ".Hold : " + ex.Message);
            }
        }

        public void Unhold(IOrganizationService organizationService, ITracingService tracingService, Guid id)
        {
            try
            {
                //Update status diganti dari MTAR
                /*_DL_serviceappointment = new DL_serviceappointment();
                _DL_serviceappointment.trs_frommobile = false;
                _DL_serviceappointment.Update(organizationService, id);

                _DL_serviceappointment.Unhold(organizationService, id);*/
                Entity entity = _DL_serviceappointment.Select(organizationService, id);
                if (entity.Attributes.Contains("trs_isdispatched") && entity.GetAttributeValue<bool>("trs_isdispatched"))
                {
                    //CreateMTAR(organizationService, id, Configuration.MTAR_Resume, entity.GetAttributeValue<bool>("trs_workshop"));
                    _mtarGenerator.GenerateMTARforAllMechanic(organizationService, id, Configuration.MTAR_Resume, entity.GetAttributeValue<bool>("trs_workshop"), true);
                }
                else
                {
                    //CreateMTAR(organizationService, id, Configuration.MTAR_Resume, true);
                    _mtarGenerator.GenerateMTARforAllMechanic(organizationService, id, Configuration.MTAR_Resume, true, true);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(_classname + ".Hold : " + ex.Message);
            }
        }

        public void Dispatch(IOrganizationService organizationService, ITracingService tracingService, Guid id)
        {
            try
            {
                Entity entity = _DL_serviceappointment.Select(organizationService, id);
                if (entity.Attributes.Contains("trs_mechanicleader") && entity.GetAttributeValue<EntityReference>("trs_mechanicleader") != null)
                {
                    UpdateMechanictoOpenTask(organizationService, id, entity.GetAttributeValue<EntityReference>("trs_mechanicleader").Id);
                    if (entity.GetAttributeValue<bool>("trs_workshop"))
                    {
                        //CreateMTAR(organizationService, id, Configuration.MTAR_Dispatch, entity.GetAttributeValue<bool>("trs_workshop"));
                        if (!entity.GetAttributeValue<bool>("trs_isdispatched"))
                        {
                            _DL_serviceappointment.Dispatched(organizationService, id);
                            _mtarGenerator.GenerateMTARforAllMechanic(organizationService, id, Configuration.MTAR_Dispatch, entity.GetAttributeValue<bool>("trs_workshop"), true);
                        }
                    }
                    else
                    {
                        if (entity.GetAttributeValue<bool>("trs_isdispatched")
                            || entity.GetAttributeValue<OptionSetValue>("statuscode").Value == Configuration.WOStatus_Dispatched)
                        {
                            FMobile _fMobile = new FMobile(organizationService);
                            _fMobile.SendServiceAppointment(organizationService, id);
                        }
                        else
                            _DL_serviceappointment.Dispatched(organizationService, id);
                    }
                }
                else
                {
                    throw new InvalidWorkflowException("Please fill Mechanic Leader first !");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(_classname + ".Dispatch : " + ex.Message);
            }
        }

        public void CalculateRTG(IOrganizationService organizationService, Guid id)
        {
            try
            {
                QueryExpression qeCommercialHeader = new QueryExpression(_DL_task.EntityName);
                qeCommercialHeader.ColumnSet = new ColumnSet(true);
                ConditionExpression ceCommercialHeader = new ConditionExpression();
                ceCommercialHeader.AttributeName = "trs_operationid";
                ceCommercialHeader.Operator = ConditionOperator.Equal;
                ceCommercialHeader.Values.Add(id);
                qeCommercialHeader.Criteria.AddCondition(ceCommercialHeader);
                EntityCollection ecCommercialHeader = _DL_task.Select(organizationService, qeCommercialHeader);

                foreach (Entity enCommercialHeader in ecCommercialHeader.Entities)
                {
                    decimal totalRTGCommercialDetail = 0;

                    QueryExpression qeCommercialDetail = new QueryExpression(_DL_trs_commercialdetail.EntityName);
                    qeCommercialDetail.ColumnSet = new ColumnSet(true);
                    ConditionExpression ceCommercialDetail = new ConditionExpression();
                    ceCommercialDetail.AttributeName = "trs_CommercialHeaderId";
                    ceCommercialDetail.Operator = ConditionOperator.Equal;
                    ceCommercialDetail.Values.Add(enCommercialHeader.Id);
                    qeCommercialDetail.Criteria.AddCondition(ceCommercialDetail);
                    EntityCollection ecCommercialDetail = _DL_trs_commercialdetail.Select(organizationService, qeCommercialDetail);

                    foreach (Entity enCommercialDetail in ecCommercialDetail.Entities)
                    {
                        if (enCommercialDetail.Contains("trs_RTG") && enCommercialDetail.Attributes["trs_RTG"] != null)
                        {
                            totalRTGCommercialDetail += (decimal)enCommercialDetail.Attributes["trs_RTG"];
                        }
                    }

                    // update commercial header rtg
                    _DL_task = new DL_task();
                    _DL_task.trs_totalrtg = totalRTGCommercialDetail;
                    _DL_task.Update(organizationService, enCommercialHeader.Id);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(_classname + ".CalculateRTG : " + ex.Message);
            }
        }

        public void Release(IOrganizationService organizationService, ITracingService tracingService, Guid id)
        {
            try
            {
                _woGenerator.Release(organizationService, id);
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(_classname + ".Release: " + ex.Message.ToString());
            }
        }
 
        public void CreateQuotationWorkorder(IOrganizationService organizationService, ITracingService tracingService, Guid id)
        {
            try
            {
                Entity entity = _DL_serviceappointment.Select(organizationService, id);
                Guid srID = ((EntityReference) entity.Attributes["regardingobjectid"]).Id;

                QueryExpression qe = new QueryExpression("trs_quotation");
                qe.ColumnSet = new ColumnSet("trs_servicerequisition", "trs_quotationid");
                qe.Criteria.AddCondition("trs_servicerequisition", ConditionOperator.Equal, srID);
                EntityCollection eQuot = organizationService.RetrieveMultiple(qe);
                if (eQuot.Entities.Count > 0)
                {
                    Guid quoID = (Guid)eQuot.Entities[0].Attributes["trs_quotationid"];
                    if (quoID != null)
                    {
                        Guid commercialHeaderId = Guid.Empty;
                        Guid commercialDetailId = Guid.Empty;

                        #region Quotation Commercial Header
                        QueryExpression qeCH = new QueryExpression("trs_quotationcommercialheader");
                        qeCH.ColumnSet = new ColumnSet(true);
                        qeCH.Criteria.AddCondition("trs_quotationid", ConditionOperator.Equal, quoID);
                        EntityCollection eQCH = organizationService.RetrieveMultiple(qeCH);
                        foreach (Entity entityCommercialHeader in eQCH.Entities)
                        {
                            commercialHeaderId = Guid.Empty;

                            _DL_task = new DL_task();
                            _DL_task.trs_operationid = id;
                            _DL_task.trs_tasklistheader = ((EntityReference)entityCommercialHeader.Attributes["trs_taskheader"]).Id;
                            _DL_task.trs_price = entityCommercialHeader.GetAttributeValue<Money>("trs_price").Value;
                            _DL_task.trs_totalrtg = Convert.ToDecimal (entityCommercialHeader.Attributes["trs_totalrtg"]);
                            //_DL_task.trs_totalrtg = entityCommercialHeader.GetAttributeValue<decimal>("trs_totalrtg");
                            _DL_task.trs_discountby = entityCommercialHeader.GetAttributeValue<bool>("trs_discountby");
                            _DL_task.trs_discountamount = entityCommercialHeader.GetAttributeValue<Money>("trs_discountamount").Value;
                            _DL_task.trs_discountpercent = Convert.ToDecimal(entityCommercialHeader.Attributes["trs_discountpercent"]);
                            _DL_task.trs_totalprice = entityCommercialHeader.GetAttributeValue<Money>("trs_totalprice").Value;
                            _DL_task.transactioncurrencyid = ((EntityReference)entityCommercialHeader.Attributes["transactioncurrencyid"]).Id;
                            _DL_task.subject = entityCommercialHeader.GetAttributeValue<string>("trs_commercialheader");
                            _DL_task.regardingobjectid = srID;
                            _DL_task.trs_fromquotation = true;
                            commercialHeaderId = _DL_task.Insert(organizationService);

                            #region Quotation Commerical Detail
                            //NOTE = Quotation commercial detail diremark karena Parental jadi ketika insert Quot. Comm. Header
                            //       maka data terkait akan lansung ikut ke insert ke work order comm. detail 
                            //2015-02-18 : dibuka kembali karena yang dari quotation tidak boleh update lagi dari master

                            QueryExpression qeCoDetail = new QueryExpression("trs_quotationcommercialdetail");
                            qeCoDetail.ColumnSet = new ColumnSet(true);
                            //qeCoDetail.Criteria.AddCondition("trs_quotation", ConditionOperator.Equal, quoID);
                            qeCoDetail.Criteria.AddCondition("trs_commercialheader", ConditionOperator.Equal, entityCommercialHeader.Id);
                            EntityCollection eCoDt = organizationService.RetrieveMultiple(qeCoDetail);
                            foreach (Entity entityCoDt in eCoDt.Entities)
                            {
                                commercialDetailId = Guid.Empty;

                                _DL_trs_commercialdetail = new DL_trs_commercialdetail();
                                _DL_trs_commercialdetail.trs_workorder = id;
                                if (entityCoDt.Contains("trs_taskcode"))
                                    _DL_trs_commercialdetail.TaskCode = entityCoDt.GetAttributeValue<string>("trs_taskcode");
                                _DL_trs_commercialdetail.CommercialHeaderId = commercialHeaderId;
                                if (entityCoDt.Contains("trs_mechanicgrade"))
                                    _DL_trs_commercialdetail.trs_mechanicgrade = ((EntityReference)entityCoDt["trs_mechanicgrade"]).Id;
                                if (entityCoDt.Contains("trs_taskname"))
                                    _DL_trs_commercialdetail.Taskname = ((EntityReference)entityCoDt["trs_taskname"]).Id;
                                _DL_trs_commercialdetail.trs_commercialtask = ((EntityReference)entityCoDt.Attributes["trs_tasklistdetail"]).Id;
                                if (entityCoDt.Contains("trs_rtg"))
                                {
                                    _DL_trs_commercialdetail.trs_rtg = (decimal)entityCoDt["trs_rtg"];
                                }
                                commercialDetailId = _DL_trs_commercialdetail.Insert(organizationService);

                                #region Part Detail / Commercial Detail (Part)
                                QueryExpression qePartDetail = new QueryExpression("trs_quotationpartdetail");
                                qePartDetail.ColumnSet = new ColumnSet("trs_quantity", "trs_partnumber", "trs_partdescription");
                                //qePartDetail.Criteria.AddCondition("trs_quotation", ConditionOperator.Equal, quoID);
                                qePartDetail.Criteria.AddCondition("trs_commercialdetailid", ConditionOperator.Equal, entityCoDt.Id);
                                EntityCollection eQPartDetail = organizationService.RetrieveMultiple(qePartDetail);
                                foreach (Entity entityPartDetail in eQPartDetail.Entities)
                                {
                                    _DL_trs_workorderpartdetail = new DL_trs_workorderpartdetail();
                                    if (entityPartDetail.Contains("trs_workorder")) { _DL_trs_workorderpartdetail.trs_workorder = id; }
                                    _DL_trs_workorderpartdetail.trs_task = commercialDetailId;
                                    if (entityPartDetail.Contains("trs_partnumber")) { _DL_trs_workorderpartdetail.trs_partnumber = ((EntityReference)entityPartDetail.Attributes["trs_partnumber"]).Id; }
                                    if (entityPartDetail.Contains("trs_partdescription")) { _DL_trs_workorderpartdetail.trs_partdescription = entityPartDetail.GetAttributeValue<string>("trs_partdescription"); }
                                    if (entityPartDetail.Contains("trs_quantity")) { _DL_trs_workorderpartdetail.trs_quantity = entityPartDetail.GetAttributeValue<Int32>("trs_quantity"); }
                                    _DL_trs_workorderpartdetail.Insert(organizationService);
                                }
                                #endregion
                            }
                            #endregion
                        }
                        #endregion

                        #region Quotation Part Summary
                        QueryExpression qePartSummary = new QueryExpression("trs_quotationpartssummary");
                        qePartSummary.ColumnSet = new ColumnSet(true);
                        qePartSummary.Criteria.AddCondition("trs_quotationnumber", ConditionOperator.Equal, quoID);
                        EntityCollection eQPS = organizationService.RetrieveMultiple(qePartSummary);
                        foreach (Entity entityPartSummary in eQPS.Entities)
                        {
                            _DL_trs_workorderpartssummary = new DL_trs_workorderpartssummary();
                            _DL_trs_workorderpartssummary.trs_workorder = id;
                            _DL_trs_workorderpartssummary.trs_partnumber = ((EntityReference)entityPartSummary.Attributes["trs_partnumber"]).Id;
                            _DL_trs_workorderpartssummary.trs_tasklistquantity = entityPartSummary.GetAttributeValue<Int32>("trs_tasklistquantity");
                            _DL_trs_workorderpartssummary.trs_manualquantity = entityPartSummary.GetAttributeValue<Int32>("trs_manualquantity");
                            _DL_trs_workorderpartssummary.trs_discountamount = entityPartSummary.GetAttributeValue<Money>("trs_discountamount");
                            _DL_trs_workorderpartssummary.trs_discountpercent = Convert.ToDecimal(entityPartSummary.Attributes["trs_discountpercent"]);
                            _DL_trs_workorderpartssummary.trs_totalprice = entityPartSummary.GetAttributeValue<Money>("trs_totalprice");
                            _DL_trs_workorderpartssummary.trs_price = entityPartSummary.GetAttributeValue<Money>("trs_price");
                            _DL_trs_workorderpartssummary.trs_discountby = entityPartSummary.GetAttributeValue<bool>("trs_discountby");
                            _DL_trs_workorderpartssummary.Insert(organizationService);
                        }
                        #endregion

                        #region Supporting Material
                        QueryExpression qeSupMaterial = new QueryExpression("trs_quotationsupportingmaterial");
                        qeSupMaterial.ColumnSet = new ColumnSet(true);
                        qeSupMaterial.Criteria.AddCondition("trs_quotationid", ConditionOperator.Equal, quoID);
                        EntityCollection eQSM = organizationService.RetrieveMultiple(qeSupMaterial);
                        foreach (Entity entitySuppMaterial in eQSM.Entities)
                        {
                            _DL_trs_workordersupportingmaterial = new DL_trs_workordersupportingmaterial();
                            _DL_trs_workordersupportingmaterial.trs_workorderid = id;
                            _DL_trs_workordersupportingmaterial.trs_quantity = Convert.ToInt32(entitySuppMaterial.GetAttributeValue<String>("trs_quantity"));
                            _DL_trs_workordersupportingmaterial.trs_price = entitySuppMaterial.GetAttributeValue<Money>("trs_price").Value;
                            _DL_trs_workordersupportingmaterial.trs_supportingmaterialname = entitySuppMaterial.GetAttributeValue<string>("trs_supportingmaterial");
                            _DL_trs_workordersupportingmaterial.trs_standardtext = entitySuppMaterial.GetAttributeValue<OptionSetValue>("trs_supportingmaterialoption").Value;
                            _DL_trs_workordersupportingmaterial.trs_totalprice = entitySuppMaterial.GetAttributeValue<Money>("trs_totalprice").Value;
                            _DL_trs_workordersupportingmaterial.Insert(organizationService);
                        }
                        #endregion

                        #region Quotation Tool
                        QueryExpression qeTool = new QueryExpression("trs_quotationtool");
                        qeTool.ColumnSet = new ColumnSet(true);
                        qeTool.Criteria.AddCondition("trs_quotation", ConditionOperator.Equal, quoID);
                        EntityCollection eQueryTool = organizationService.RetrieveMultiple(qeTool);
                        foreach (Entity entityTool in eQueryTool.Entities)
                        {
                            _DL_trs_workordertoolsrecommendation = new DL_trs_workordertoolsrecommendation();
                            _DL_trs_workordertoolsrecommendation.trs_workorder = id;
                            _DL_trs_workordertoolsrecommendation.trs_toolsgroupid = ((EntityReference)entityTool.Attributes["trs_toolsgroup"]).Id;
                            _DL_trs_workordertoolsrecommendation.Insert(organizationService);
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(_classname + ".CreateQuotationWorkorder: " + ex.Message.ToString());
            }
        }

        public void GetPartsTools(IOrganizationService organizationService, ITracingService tracingService, Guid activityId)
        {
            try
            {
                //// Get commercial detail
                //QueryExpression qeCommercialHeader = new QueryExpression(_DL_task.EntityName);
                //qeCommercialHeader.ColumnSet = new ColumnSet(true);
                //qeCommercialHeader.Criteria.AddCondition("trs_operationid", ConditionOperator.Equal, activityId);
                //EntityCollection ecCommercialHeader = _DL_task.Select(organizationService, qeCommercialHeader);
                //foreach (Entity enCommercialHeader in ecCommercialHeader.Entities)
                //{
                //    QueryExpression qeCommercialDetail = new QueryExpression(_DL_trs_commercialdetail.EntityName);
                //    qeCommercialDetail.ColumnSet = new ColumnSet(true);
                //    qeCommercialDetail.Criteria.AddCondition("trs_commercialheaderid", ConditionOperator.Equal, enCommercialHeader.Id);
                //    EntityCollection ecCommercialDetail = _DL_trs_commercialdetail.Select(organizationService, qeCommercialDetail);

                //    QueryExpression qeWorkOrderPartDetail = new QueryExpression(_DL_trs_workorderpartdetail.EntityName);
                //    qeWorkOrderPartDetail.ColumnSet = new ColumnSet(true);
                //    qeWorkOrderPartDetail.Criteria.AddCondition("trs_workorder", ConditionOperator.Equal, activityId);
                //    EntityCollection ecWorkOrderPartDetail = _DL_trs_workorderpartdetail.Select(organizationService, qeWorkOrderPartDetail);

                //    foreach (Entity enCommercialDetail in ecCommercialDetail.Entities)
                //    {
                        // check if part from commercial detail already exist
                        //bool isAlreadyExist = false;
                        //foreach (Entity enWorkOrderPartDetail in ecWorkOrderPartDetail.Entities)
                        //{
                        //    if (enCommercialDetail.Id == ((EntityReference)enWorkOrderPartDetail["trs_task"]).Id)
                        //        isAlreadyExist = true;
                        //}
                        //if (isAlreadyExist == false)
                        //{
                        //    // Create wo part detail
                        //    _DL_trs_workorderpartdetail = new DL_trs_workorderpartdetail();
                        //    _DL_trs_workorderpartdetail.trs_partdescription = enCommercialDetail["trs_taskcode"].ToString();
                        //    _DL_trs_workorderpartdetail.trs_task = enCommercialDetail.Id;
                        //    _DL_trs_workorderpartdetail.trs_workorder = activityId;
                        //    _DL_trs_workorderpartdetail.Insert(organizationService);
                        //    // quantity from quotation part detail
                        //}
                        //// get tools -- Masih salah cara tarik tools-nya
                        //if (enCommercialDetail.Contains("trs_commercialtask"))
                        //{
                        //    QueryExpression qeMasterPart = new QueryExpression(_DL_trs_masterpart.EntityName);
                        //    qeMasterPart.ColumnSet = new ColumnSet(true);
                        //    qeMasterPart.Criteria.AddCondition("trs_tasklistdetails", ConditionOperator.Equal, ((EntityReference)enCommercialDetail["trs_commercialtask"]).Id);
                        //    EntityCollection ecMasterPart = _DL_trs_masterpart.Select(organizationService, qeMasterPart);
                        //    foreach (Entity enMasterPart in ecMasterPart.Entities)
                        //    {
                        //        if (enMasterPart.Contains("trs_tools"))
                        //        {
                        //            Entity enToolsMaster = _DL_trs_toolsmaster.Select(organizationService, ((EntityReference)enMasterPart["trs_tools"]).Id);
                        //            _DL_trs_toolsmaster = new DL_trs_toolsmaster();
                        //            _DL_trs_toolsmaster.trs_workorderid = activityId;
                        //            _DL_trs_toolsmaster.Update(organizationService, enToolsMaster.Id);
                        //        }
                        //    }
                        //}
                //    }
                //}

            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(_classname + ".GetPartsTools : " + ex.Message.ToString());
            }
        }

        public void RequestTools(IOrganizationService organizationService, ITracingService tracingService, Guid id)
        {
            try
            {
                string content = string.Empty;
                ServiceAppointmentBase mServiceAppointment = new ServiceAppointmentBase();
                Entity entity = _DL_serviceappointment.Select(organizationService, id);

                QueryExpression qeToolRecommendation = new QueryExpression(_DL_trs_workordertoolsrecommendation.EntityName);
                qeToolRecommendation.ColumnSet = new ColumnSet(true);
                qeToolRecommendation.Criteria.AddCondition("trs_workorder", ConditionOperator.Equal, id);
                EntityCollection ecToolRecommendation = _DL_trs_workordertoolsrecommendation.Select(organizationService, qeToolRecommendation);
                foreach (Entity enToolsRecommendation in ecToolRecommendation.Entities)
                {
                    content += enToolsRecommendation["trs_tools"].ToString() + " Name: " + enToolsRecommendation["trs_toolsname"].ToString() + "<br />";//" Description: " + enToolsRecommendation["trs_toolsdescription"].ToString() + 
                }
                Guid emailId = Guid.Empty;
                EmailAgent emailAgent = new EmailAgent();
                emailAgent.AddSender(((EntityReference)entity["owninguser"]).Id);
                emailAgent.AddReceiver(_DL_systemuser.EntityName, ((EntityReference)entity["owninguser"]).Id);
                //emailAgent.subject = "Request tools untuk Work Order no" + entity["subject"].ToString();
                emailAgent.subject = "Request tools untuk Work Order no" + entity["trs_crmwonumber"].ToString();
                emailAgent.description = content;
                emailAgent.Create(organizationService, out emailId);
                //if (emailId != Guid.Empty)
                //    emailAgent.Send(organizationService, emailId);

            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(_classname + ".RequestTools : " + ex.Message);
            }
        }

        public void PartReturn(IOrganizationService organizationService, ITracingService tracingService, Guid id)
        {
            try
            {
                if (_fSAP.SynchronizetoSAP(organizationService))
                {
                    Entity enServiceAppointment = _DL_serviceappointment.Select(organizationService, id);

                    QueryExpression qeWorkOrderPartsSummary = new QueryExpression(_DL_trs_workorderpartssummary.EntityName);
                    qeWorkOrderPartsSummary.ColumnSet = new ColumnSet(true);
                    qeWorkOrderPartsSummary.Criteria.AddCondition("trs_workorder", ConditionOperator.Equal, id);
                    EntityCollection ecWorkOrderPartsSummary = _DL_trs_workorderpartssummary.Select(organizationService, qeWorkOrderPartsSummary);

                    string content = string.Empty;
                    string trType = "A";
                    string trSta = string.Empty;
                    string trPart = "D";
                    //string woNumber = enServiceAppointment["subject"].ToString();
                    string woNumber = enServiceAppointment["trs_crmwonumber"].ToString();

                    foreach (Entity enWorkOrderPartsSummary in ecWorkOrderPartsSummary.Entities)
                    {
                        if (content != string.Empty)
                            content += Environment.NewLine;
                        string materialNumber = string.Empty;
                        string quantity = string.Empty;

                        if (enWorkOrderPartsSummary.Contains("trs_returnedquantity"))
                            quantity = ((int)enWorkOrderPartsSummary["trs_returnedquantity"]).ToString();
                        if (enWorkOrderPartsSummary.Contains("trs_partnumber"))
                        {
                            Entity enMasterPart = _DL_trs_masterpart.Select(organizationService, ((EntityReference)enWorkOrderPartsSummary["trs_partnumber"]).Id);
                            materialNumber = enMasterPart["trs_name"].ToString();
                        }
                        content += trType + "|" + trSta + "|" + trPart + "|" + woNumber + "|" + materialNumber + "|" + quantity;
                    }

                    string path = @"\\" + _fSAP.GetSAPSharingPath(organizationService) + @"\02_TRSTOSAP\";
                    //string path = @"D:\Shared Folder\";
                    if (System.IO.Directory.Exists(path))
                    {
                        string filename = "WR_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + woNumber + ".txt";
                        System.IO.File.WriteAllText(path + filename, content);
                    }

                    else
                    {
                        throw new Exception("Directory not found: " + path);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(_classname + ".ReturnParts : " + ex.Message);
            }
        }

        public void ConfirmTECO(IOrganizationService organizationService, ITracingService tracingService, Guid id)
        {
            try
            {
                if (_fSAP.SynchronizetoSAP(organizationService))
                {
                    Entity enServiceAppointment = _DL_serviceappointment.Select(organizationService, id);

                    QueryExpression qeWorkOrderPartsSummary = new QueryExpression(_DL_trs_workorderpartssummary.EntityName);
                    qeWorkOrderPartsSummary.ColumnSet = new ColumnSet(true);
                    qeWorkOrderPartsSummary.Criteria.AddCondition("trs_workorder", ConditionOperator.Equal, id);
                    EntityCollection ecWorkOrderPartsSummary = _DL_trs_workorderpartssummary.Select(organizationService, qeWorkOrderPartsSummary);

                    string partconsumed = string.Empty;
                    string partreturned = string.Empty;
                    string partreturnedline = string.Empty;
                    string confirm = string.Empty;
                    string teco = string.Empty;
                    string trSta = string.Empty;
                    //string woNumber = enServiceAppointment["subject"].ToString();
                    string woNumber = enServiceAppointment["trs_crmwonumber"].ToString();
                    string detType = "C";

                    if (ecWorkOrderPartsSummary.Entities.Count > 0)
                    {
                        foreach (Entity enWorkOrderPartsSummary in ecWorkOrderPartsSummary.Entities)
                        {
                            if (partconsumed != string.Empty)
                                partconsumed += Environment.NewLine;
                            if (partreturned != string.Empty)
                                partreturned += Environment.NewLine;

                            string itemNo = string.Empty;
                            decimal manualQuantity = 0;
                            decimal tasklistQuantity = 0;
                            decimal returnedQuantity = 0;
                            decimal consumedQuantity = 0;
                            string materialnumber = string.Empty;

                            if (enWorkOrderPartsSummary.Contains("trs_manualquantity"))
                                manualQuantity = (int)enWorkOrderPartsSummary["trs_manualquantity"];
                            if (enWorkOrderPartsSummary.Contains("trs_tasklistquantity"))
                                tasklistQuantity = (int)enWorkOrderPartsSummary["trs_tasklistquantity"];
                            if (enWorkOrderPartsSummary.Contains("trs_returnedquantity"))
                                returnedQuantity = (int)enWorkOrderPartsSummary["trs_returnedquantity"];
                            consumedQuantity = manualQuantity + tasklistQuantity - returnedQuantity;

                            materialnumber = ((EntityReference)enWorkOrderPartsSummary["trs_partnumber"]).Name;

                            if (enWorkOrderPartsSummary.Contains("trs_itemnumber"))
                                itemNo = enWorkOrderPartsSummary["trs_itemnumber"].ToString();

                            partconsumed += ((int)TrType.PartConsume).ToString() + "|" + trSta + "|" + trPartDetail + "|" + woNumber +
                                "|" + detType + "|" + itemNo + "|" + consumedQuantity.ToString();

                            if (returnedQuantity > 0)
                            {
                                partreturned += "A|" + trSta + "|" + trPartDetail + "|" + woNumber + "|" + materialnumber + "|" + returnedQuantity.ToString();
                            }
                        }
                    }

                    confirm = Environment.NewLine + ((int)TrType.Confirmation).ToString() + "|" + trSta + "|" + trPartHeader + "|" + woNumber;
                    teco = Environment.NewLine + ((int)TrType.TECO).ToString() + "|" + trSta + "|" + trPartHeader + "|" + woNumber;
                    partreturnedline = Environment.NewLine + partreturned;

                    //SetStateRequest state = new SetStateRequest();
                    //state.State = new OptionSetValue((int)

                    _DL_serviceappointment = new DL_serviceappointment();
                    _DL_serviceappointment.SubmitTECObySDH(organizationService, id);

                    string path = @"\\" + _fSAP.GetSAPSharingPath(organizationService) + @"\02_TRSTOSAP\";
                    //string path = @"D:\Shared Folder\";
                    if (System.IO.Directory.Exists(path))
                    {
                        string filename = "WR_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + woNumber + ".txt";
                        System.IO.File.WriteAllText(path + filename, partconsumed + confirm + teco + partreturnedline);
                    }

                    else
                    {
                        throw new Exception("Directory not found: " + path);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(_classname + ".ConfirmTECO : " + ex.Message);
            }
        }

        //Added by Santony - 4/9/2015 (Mengirimkan Text File ke SAP untuk Parts di WO yang di update)
        public void UpdateWO_to_SAP(IOrganizationService organizationService, ITracingService tracingService, Guid id)
        {
            try
            {
                if (_fSAP.SynchronizetoSAP(organizationService))
                {
                    #region variable
                    string header = string.Empty;
                    string operationdetail = string.Empty;
                    string component = string.Empty;
                    string assignMechanic = string.Empty;

                    string currency = string.Empty;
                    string termOfPayment = string.Empty;
                    string actType = string.Empty;
                    string pricePerUnit = string.Empty;
                    string discountPercent = string.Empty;
                    string materialNo = string.Empty;
                    #endregion

                    #region header
                    Entity enServiceAppointment = _DL_serviceappointment.Select(organizationService, id);
                    string trType = ((int)TrType.Create).ToString();
                    string trSta = string.Empty;
                    string woNumber = string.Empty;
                    string plant = string.Empty;
                    string businessArea = string.Empty;
                    string shorttext = string.Empty;
                    string purchaseorder = string.Empty;
                    string accindic = string.Empty;
                    string workcenter = string.Empty;
                    string pmacctype = string.Empty;
                    string bscstart = string.Empty;
                    string bscend = string.Empty;
                    string equipment = string.Empty;
                    string functionallocation = string.Empty;
                    string customer = string.Empty;
                    string responsiblecostcenter = string.Empty;
                    string profitcenter = string.Empty;

                    //if (enServiceAppointment.Contains("subject"))
                    //woNumber = enServiceAppointment.Attributes["subject"].ToString();

                    if (enServiceAppointment.Contains("trs_crmwonumber"))
                        woNumber = enServiceAppointment.Attributes["trs_crmwonumber"].ToString();

                    if (enServiceAppointment.Contains("trs_plant"))
                    {
                        //Entity enBusinessUnit = _DL_businessunit.Select(organizationService, ((EntityReference)enServiceAppointment.Attributes["trs_plant"]).Id);
                        //plant = enBusinessUnit.Attributes["name"].ToString();
                        // EHN -- field trs_plant tidak terbaca 16/12/2014
                        plant = ((EntityReference)enServiceAppointment.Attributes["trs_plant"]).Name;
                    }
                    if (enServiceAppointment.Contains("trs_branch"))
                        businessArea = ((EntityReference)enServiceAppointment.Attributes["trs_branch"]).Name;
                    if (enServiceAppointment.Contains("description"))
                        shorttext = enServiceAppointment.Attributes["description"].ToString();
                    if (enServiceAppointment.Contains("trs_ponumber"))
                        purchaseorder = enServiceAppointment.Attributes["trs_ponumber"].ToString();
                    if (enServiceAppointment.Contains("trs_accind"))
                    {
                        accindic = ((OptionSetValue)enServiceAppointment.Attributes["trs_accind"]).Value.ToString("00");
                    }

                    if (enServiceAppointment.Contains("trs_workcenter"))
                    {
                        EntityReference erWorkcenter = (EntityReference)enServiceAppointment.Attributes["trs_workcenter"];
                        Entity enWorkCenter = _DL_trs_workcenter.Select(organizationService, erWorkcenter.Id);
                        workcenter = enWorkCenter.Contains("trs_workcenter") ? enWorkCenter.Attributes["trs_workcenter"].ToString() : string.Empty;
                    }
                    if (enServiceAppointment.Contains("trs_pmacttype"))
                    {
                        EntityReference erTaskListGroup = (EntityReference)enServiceAppointment.Attributes["trs_pmacttype"];
                        Entity enTaskListGroup = _DL_trs_tasklistgroup.Select(organizationService, erTaskListGroup.Id);
                        pmacctype = enTaskListGroup.Contains("trs_pmacttype") ? enTaskListGroup.Attributes["trs_pmacttype"].ToString() : string.Empty;
                    }
                    if (enServiceAppointment.Contains("scheduledstart"))
                        bscstart = ((DateTime)enServiceAppointment.Attributes["scheduledstart"]).ToLocalTime().ToString("yyyyMMdd");
                    if (enServiceAppointment.Contains("scheduledend"))
                        bscend = ((DateTime)enServiceAppointment.Attributes["scheduledend"]).ToLocalTime().ToString("yyyyMMdd");
                    if (enServiceAppointment.Contains("trs_equipment"))
                    {
                        EntityReference erEquipment = (EntityReference)enServiceAppointment.Attributes["trs_equipment"];
                        Entity enEquipment = _DL_new_population.Select(organizationService, erEquipment.Id);
                        equipment = enEquipment.Contains("new_serialnumber") ? enEquipment.Attributes["new_serialnumber"].ToString() : string.Empty;
                    }
                    if (enServiceAppointment.Contains("trs_functionallocation"))
                    {
                        EntityReference erFunctionallocation = (EntityReference)enServiceAppointment.Attributes["trs_functionallocation"];
                        Entity enFunctionallocation = _DL_trs_functionallocation.Select(organizationService, erFunctionallocation.Id);
                        functionallocation = enFunctionallocation.Contains("trs_functionalcode") ? enFunctionallocation.Attributes["trs_functionalcode"].ToString() : string.Empty;
                    }

                    // customer from party list, must have only one customer
                    if (enServiceAppointment.Contains("customers"))
                    {
                        QueryExpression queryExpression = new QueryExpression(_DL_activityparty.EntityName);
                        queryExpression.ColumnSet = new ColumnSet(true);
                        FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                        filterExpression.AddCondition("activityid", ConditionOperator.Equal, id);
                        filterExpression.AddCondition("participationtypemask", ConditionOperator.Equal, _participationtypemask_customer);
                        filterExpression.AddCondition("partyobjecttypecode", ConditionOperator.Equal, _partyobjecttypecode_account);
                        EntityCollection ecActivityParty = _DL_activityparty.Select(organizationService, queryExpression);

                        if (ecActivityParty.Entities.Count > 0)
                        {
                            Entity enActivityParty = ecActivityParty.Entities[0];
                            if (enActivityParty.Contains("partyid") && enActivityParty.Attributes["partyid"] != null)
                            {
                                Entity enAccount = _DL_account.Select(organizationService, ((EntityReference)enActivityParty.Attributes["partyid"]).Id);
                                customer = enAccount.Contains("accountnumber") ? enAccount.Attributes["accountnumber"].ToString() : string.Empty;
                            }
                        }
                    }

                    if (enServiceAppointment.Contains("trs_responsiblecctr"))
                    {
                        EntityReference erResponsiblecostcenter = (EntityReference)enServiceAppointment.Attributes["trs_responsiblecctr"];
                        Entity enResponsiblecostcenter = _DL_trs_responsiblecostcenter.Select(organizationService, erResponsiblecostcenter.Id);
                        responsiblecostcenter = enResponsiblecostcenter.Contains("trs_costcenter") ? enResponsiblecostcenter.Attributes["trs_costcenter"].ToString() : string.Empty;
                    }
                    if (enServiceAppointment.Contains("trs_profitcenter"))
                    {
                        EntityReference erProfitcenter = (EntityReference)enServiceAppointment.Attributes["trs_profitcenter"];
                        Entity enProfitcenter = _DL_trs_profitcenter.Select(organizationService, erProfitcenter.Id);
                        profitcenter = enProfitcenter.Contains("trs_profitcenter") ? enProfitcenter.Attributes["trs_profitcenter"].ToString() : string.Empty;
                    }

                    if (enServiceAppointment.Contains("trs_acttype"))
                    {
                        Entity enActType = _DL_trs_acttype.Select(organizationService, ((EntityReference)enServiceAppointment["trs_acttype"]).Id);
                        actType = enActType["trs_name"].ToString();
                    }

                    if (enServiceAppointment.Contains("transactioncurrencyid"))
                    {

                        QueryExpression qeCurrency = new QueryExpression("transactioncurrency");
                        qeCurrency.ColumnSet = new ColumnSet(true);
                        FilterExpression feCurrency = qeCurrency.Criteria.AddFilter(LogicalOperator.And);
                        feCurrency.AddCondition("transactioncurrencyid", ConditionOperator.Equal, ((EntityReference)enServiceAppointment["transactioncurrencyid"]).Id);
                        EntityCollection ecCurrency = _DL_transactioncurrency.Select(organizationService, qeCurrency);
                        if (ecCurrency.Entities.Count > 0)
                        {
                            currency = ecCurrency.Entities[0]["currencyname"].ToString();
                        }
                    }
                    if (enServiceAppointment.Contains("trs_paymentterm"))
                    {
                        Entity enPaymentTerm = _DL_trs_paymentterm.Select(organizationService, ((EntityReference)enServiceAppointment["trs_paymentterm"]).Id);
                        termOfPayment = enPaymentTerm.Contains("trs_name") ? enPaymentTerm["trs_name"].ToString() : string.Empty;
                    }

                    header = ((int)TrType.Change).ToString() + "|" + trSta + "|" + trPartHeader + "|" + woNumber + "|" + plant + "|" + businessArea + "|" + shorttext + "|" + purchaseorder
                            + "|" + accindic + "|" + workcenter + "|" + plant + "|" + pmacctype + "|" + bscstart + "|" + bscend
                            + "|" + equipment + "|" + functionallocation + "|" + customer + "|" + responsiblecostcenter + "|" + profitcenter;

                    #endregion

                    #region operation/commercial header
                    int itemCountOperation = 10;
                    string opDetType = "O";
                    string opItemNo = string.Empty;
                    string opShortText = string.Empty;
                    string stdTextKey = string.Empty;
                    string work = string.Empty;
                    string unit = "H";
                    string duration = string.Empty;
                    #endregion

                    #region commercial header
                    QueryExpression qeCommercialHeader = new QueryExpression(_DL_task.EntityName);
                    qeCommercialHeader.ColumnSet = new ColumnSet(true);
                    FilterExpression feCommercialHeader = qeCommercialHeader.Criteria.AddFilter(LogicalOperator.And);
                    feCommercialHeader.AddCondition("trs_operationid", ConditionOperator.Equal, id);
                    EntityCollection ecCommercialHeader = _DL_task.Select(organizationService, qeCommercialHeader);

                    foreach (Entity enCommercialHeader in ecCommercialHeader.Entities)
                    {
                        pricePerUnit = string.Empty;
                        discountPercent = string.Empty;
                        materialNo = string.Empty;
                        opItemNo = itemCountOperation.ToString();
                        if (enCommercialHeader.Contains("subject"))
                            opShortText = enCommercialHeader["subject"].ToString();
                        //if (enCommercialHeader.Contains("trs_totalrtg"))
                        //    work = ((decimal)enCommercialHeader["trs_totalrtg"]).ToString("0.##");
                        //operation quantity hardcode 1
                        work = "1";
                        if (enCommercialHeader.Contains("trs_totalprice"))
                            pricePerUnit = ((Money)enCommercialHeader["trs_totalprice"]).Value.ToString("0.##");
                        //if (enCommercialHeader.Contains("trs_discountpercent"))
                        //    discountPercent = ((decimal)enCommercialHeader["trs_discountpercent"]).ToString("0.##");

                        operationdetail += Environment.NewLine + ((int)TrType.Create).ToString() + "|" + trSta + "|" + trPartDetail + "|" + woNumber
                            + "|" + opDetType + "|" + opItemNo + "|" + opShortText + "|" + stdTextKey + "|" + work + "|" + unit
                            + "|" + duration + "|" + unit + "|" + actType;
                    }
                    #endregion

                    #region supporting material
                    QueryExpression qeWOSupportingMaterialOtherService = new QueryExpression(_DL_trs_workordersupportingmaterial.EntityName);
                    qeWOSupportingMaterialOtherService.ColumnSet = new ColumnSet(true);
                    FilterExpression feWOSupportingMaterialOtherService = qeWOSupportingMaterialOtherService.Criteria.AddFilter(LogicalOperator.And);
                    feWOSupportingMaterialOtherService.AddCondition("trs_workorderid", ConditionOperator.Equal, id);
                    feWOSupportingMaterialOtherService.AddCondition("trs_supportingmaterialtype", ConditionOperator.Equal, false);
                    EntityCollection ecWOSupportingMaterialOtherService = _DL_trs_workordersupportingmaterial.Select(organizationService, qeWOSupportingMaterialOtherService);

                    foreach (Entity enWOSupportingMaterial in ecWOSupportingMaterialOtherService.Entities)
                    {
                        pricePerUnit = string.Empty;
                        discountPercent = string.Empty;
                        materialNo = string.Empty;
                        opItemNo = itemCountOperation.ToString();
                        if (enWOSupportingMaterial.Contains("trs_supportingmaterialname"))
                            opShortText = enWOSupportingMaterial["trs_supportingmaterialname"].ToString();
                        if (enWOSupportingMaterial.Contains("trs_standardtext"))
                            stdTextKey = OptionSetExtractor.GetOptionSetText(organizationService, enWOSupportingMaterial.LogicalName,
                                "trs_standardtext", ((OptionSetValue)enWOSupportingMaterial["trs_standardtext"]).Value);
                        //if (enWOSupportingMaterial.Contains("trs_quantity"))
                        //    work = enWOSupportingMaterial["trs_quantity"].ToString();
                        //operation quantity hardcode 1
                        work = "1";
                        if (enWOSupportingMaterial.Contains("trs_totalprice"))
                            pricePerUnit = ((Money)enWOSupportingMaterial["trs_totalprice"]).Value.ToString("0.##");

                        operationdetail += Environment.NewLine + ((int)TrType.Create).ToString() + "|" + trSta + "|" + trPartDetail + "|" + woNumber
                            + "|" + opDetType + "|" + opItemNo + "|" + opShortText + "|" + stdTextKey + "|" + work + "|" + unit
                            + "|" + duration + "|" + unit + "|" + actType;
                    }
                    #endregion

                    #region component detail
                    int compTaskListQuantity = 0;
                    int compManualQuantity = 0;
                    int compTotalQuantity = 0;
                    string compDetType = "C";
                    string compItemNo_PartSummary = string.Empty;
                    string compItemNo_SupportingMaterial = string.Empty;
                    string compMaterialDesc = string.Empty;
                    string compReqQty = string.Empty;
                    string compUM = string.Empty;
                    string compPrice = string.Empty;
                    string compSloc = string.Empty;
                    string compOpertnNo = "10";
                    #endregion

                    #region part summary
                    unit = "PC";
                    QueryExpression qeWOPartSummary = new QueryExpression(_DL_trs_workorderpartssummary.EntityName);
                    qeWOPartSummary.ColumnSet = new ColumnSet(true);
                    FilterExpression feWOPartSummary = qeWOPartSummary.Criteria.AddFilter(LogicalOperator.And);
                    feWOPartSummary.AddCondition("trs_workorder", ConditionOperator.Equal, id);
                    EntityCollection ecWOPartSummary = _DL_trs_workorderpartssummary.Select(organizationService, qeWOPartSummary);

                    foreach (Entity enWOPartSummary in ecWOPartSummary.Entities)
                    {
                        pricePerUnit = string.Empty;
                        discountPercent = string.Empty;
                        materialNo = string.Empty;

                        if (enWOPartSummary.Contains("trs_partnumber"))
                        {
                            EntityReference erMasterPart = (EntityReference)enWOPartSummary["trs_partnumber"];
                            Entity enMasterPart = _DL_trs_masterpart.Select(organizationService, erMasterPart.Id);
                            materialNo = enMasterPart.Contains("trs_name") ? enMasterPart.Attributes["trs_name"].ToString() : string.Empty;
                        }
                        if (enWOPartSummary.Contains("trs_tasklistquantity"))
                            compTaskListQuantity = Convert.ToInt32(enWOPartSummary["trs_tasklistquantity"]);

                        if (enWOPartSummary.Contains("trs_manualquantity"))
                            compManualQuantity = Convert.ToInt32(enWOPartSummary["trs_manualquantity"]);

                        compTotalQuantity = compTaskListQuantity + compManualQuantity;
                        compReqQty = compTotalQuantity.ToString();

                        if (enWOPartSummary.Contains("trs_price"))
                            pricePerUnit = ((Money)enWOPartSummary["trs_price"]).Value.ToString("0.##");
                        if (enWOPartSummary.Contains("trs_discountpercent"))
                            discountPercent = ((decimal)enWOPartSummary["trs_discountpercent"]).ToString("0.##");

                        if (enWOPartSummary.Contains("trs_itemnumber"))
                            compItemNo_PartSummary = (Convert.ToInt32(enWOPartSummary["trs_itemnumber"])).ToString();

                        component += Environment.NewLine + ((int)TrType.Create).ToString() + "|" + trSta + "|" + trPartDetail + "|" + woNumber +
                            "|" + compDetType + "|" + compItemNo_PartSummary + "|" + materialNo + "|" + compMaterialDesc + "|" + compReqQty + "|" + compUM +
                            "|" + compPrice + "|" + plant + "|" + compSloc + "|" + compOpertnNo;
                    }
                    #endregion

                    #region supporting material
                    unit = "EA";
                    QueryExpression qeWOSupportingMaterialExternalMaterial = new QueryExpression(_DL_trs_workordersupportingmaterial.EntityName);
                    qeWOSupportingMaterialExternalMaterial.ColumnSet = new ColumnSet(true);
                    FilterExpression feWOSupportingMaterialExternalMaterial = qeWOSupportingMaterialExternalMaterial.Criteria.AddFilter(LogicalOperator.And);
                    feWOSupportingMaterialExternalMaterial.AddCondition("trs_workorderid", ConditionOperator.Equal, id);
                    feWOSupportingMaterialExternalMaterial.AddCondition("trs_supportingmaterialtype", ConditionOperator.Equal, true);
                    EntityCollection ecWOSupportingMaterialExternalMaterial = _DL_trs_workordersupportingmaterial.Select(organizationService, qeWOSupportingMaterialExternalMaterial);

                    foreach (Entity enWOSupportingMaterial in ecWOSupportingMaterialExternalMaterial.Entities)
                    {
                        pricePerUnit = string.Empty;
                        discountPercent = string.Empty;
                        materialNo = string.Empty;

                        if (enWOSupportingMaterial.Contains("trs_supportingmaterialname"))
                            compMaterialDesc = enWOSupportingMaterial["trs_supportingmaterialname"].ToString();
                        if (enWOSupportingMaterial.Contains("trs_quantity"))
                            compReqQty = enWOSupportingMaterial["trs_quantity"].ToString();
                        if (enWOSupportingMaterial.Contains("trs_price"))
                            compPrice = ((Money)enWOSupportingMaterial["trs_price"]).Value.ToString("0.##");

                        if (enWOSupportingMaterial.Contains("trs_quantity"))
                            work = enWOSupportingMaterial["trs_quantity"].ToString();
                        if (enWOSupportingMaterial.Contains("trs_totalprice"))
                            pricePerUnit = ((Money)enWOSupportingMaterial["trs_totalprice"]).Value.ToString("0.##");

                        if (enWOSupportingMaterial.Contains("trs_itemnumber"))
                            compItemNo_SupportingMaterial = (Convert.ToInt32(enWOSupportingMaterial["trs_itemnumber"])).ToString();

                        component += Environment.NewLine + ((int)TrType.Create).ToString() + "|" + trSta + "|" + trPartDetail + "|" + woNumber
                            + "|" + compDetType + "|" + compItemNo_SupportingMaterial + "|" + materialNo + "|" + compMaterialDesc + "|" + compReqQty + "|" + unit
                            + "|" + compPrice + "|" + plant + "|" + compSloc + "|" + compOpertnNo;
                    }
                    #endregion

                    #region assignmechanic
                    int itemNo = 10;
                    string personNo = string.Empty;
                    work = "1";

                    foreach (Entity enCommercialHeader in ecCommercialHeader.Entities)
                    {
                        //if (enCommercialHeader.Contains("trs_itemnumber"))
                        //    itemNo = ((int)enCommercialHeader["trs_itemnumber"]).ToString();
                        //if (enCommercialHeader.Contains("trs_totalrtg"))
                        //    work = ((decimal)enCommercialHeader["trs_totalrtg"]).ToString("##.##");
                        //operation quantity hardcode 1

                        if (enServiceAppointment.Contains("trs_mechanicleader"))
                        {
                            Entity enMechanic = _DL_equipment.Select(organizationService, ((EntityReference)enServiceAppointment["trs_mechanicleader"]).Id);
                            personNo = enMechanic.Contains("trs_nrp") ? enMechanic["trs_nrp"].ToString() : string.Empty;
                        }

                        assignMechanic += Environment.NewLine + ((int)TrType.AssignMechanic).ToString() + "|" + trSta + "|" +
                            trPartDetail + "|" + woNumber + "|" + opDetType + "|" + itemNo.ToString() + "|" + personNo + "|" + work;

                        itemNo += 10;
                    }

                    foreach (Entity enWOSupportingMaterial in ecWOSupportingMaterialOtherService.Entities)
                    {
                        assignMechanic += Environment.NewLine + ((int)TrType.AssignMechanic).ToString() + "|" + trSta + "|" +
                            trPartDetail + "|" + woNumber + "|" + opDetType + "|" + itemNo.ToString() + "|" + personNo + "|" + work;

                        itemNo += 10;
                    }
                    #endregion

                    #region write
                    string path = @"\\" + _fSAP.GetSAPSharingPath(organizationService) + @"\02_TRSTOSAP\";
                    //string path = @"D:\Shared Folder\";
                    if (System.IO.Directory.Exists(path))
                    {
                        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                        string content = header + operationdetail + component + assignMechanic;
                        string filename = "WR_" + timestamp + "_" + woNumber + ".txt";

                        System.IO.File.WriteAllText(path + filename, content);

                        _DL_serviceappointment.trs_lasterror = string.Empty;
                        _DL_serviceappointment.trs_lastfilename = filename;
                        _DL_serviceappointment.Update(organizationService, id);
                    }
                    else
                    {
                        throw new Exception("Directory not found: " + path);
                    }
                    #endregion

                }
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(_classname + ".UpdateWO_to_SAP : " + ex.Message);
            }
        }

        public void CalculateRTG(IOrganizationService organizationService, ITracingService tracingService, Guid id)
        {
            try
            {
                _woGenerator.CalculateRTG(organizationService, id);
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(_classname + ".CalculateRTG : " + ex.Message);
            }
        }

        public void SummarizeParts(IOrganizationService organizationService, ITracingService tracingService, Guid id)
        {
            try
            {
                _woGenerator.SummarizeParts(organizationService, id, false);
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(_classname + ".SummarizeParts : " + ex.Message);
            }
        }

        public void SummarizeToolGroups(IOrganizationService organizationService, ITracingService tracingService, Guid id)
        {
            try
            {
                _woGenerator.SummarizeToolGroups(organizationService, id);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SummarizeToolGroups : " + ex.Message);
            }
        }
        #endregion
    }
}
