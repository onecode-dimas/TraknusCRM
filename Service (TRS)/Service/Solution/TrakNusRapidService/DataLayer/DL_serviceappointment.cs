using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Crm.Sdk.Messages;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_serviceappointment
    {
        #region Dependencies
        private DL_trs_functionallocation _DL_trs_functionallocation = new DL_trs_functionallocation();
        private DL_new_population _DL_new_population = new DL_new_population();
        private DL_contact _DL_contact = new DL_contact();
        private DL_trs_tasklistgroup _DL_trs_tasklistgroup = new DL_trs_tasklistgroup();
        private DL_trs_profitcenter _DL_trs_profitcenter = new DL_trs_profitcenter();
        private DL_trs_acttype _DL_trs_acttype = new DL_trs_acttype();
        private DL_trs_workcenter _DL_trs_workcenter = new DL_trs_workcenter();
        private DL_trs_responsiblecostcenter _DL_trs_responsiblecostcenter = new DL_trs_responsiblecostcenter();
        private DL_equipment _DL_equipment = new DL_equipment();
        private DL_account _DL_account = new DL_account();
        private DL_activityparty _DL_activityparty = new DL_activityparty();
        private DL_service _DL_service = new DL_service();
        private DL_site _DL_site = new DL_site();
        private DL_incident _DL_incident = new DL_incident();
        private DL_businessunit _DL_businessunit = new DL_businessunit();
        private DL_team _DL_team = new DL_team();
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        #endregion

        #region Properties
        private string _classname = "DL_serviceappointment";

        private string _entityname = "serviceappointment";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Work Order";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _subject = false;
        private string _subject_value;
        public string subject
        {
            get { return _subject ? _subject_value : null; }
            set { _subject = true; _subject_value = value; }
        }

        private bool _actualstart = false;
        private DateTime _actualstart_value;
        public DateTime actualstart
        {
            get { return _actualstart ? _actualstart_value.ToLocalTime() : DateTime.MinValue; }
            set { _actualstart = true; _actualstart_value = value.ToLocalTime(); }
        }

        private bool _trs_documentdate = false;
        private DateTime _trs_documentdate_value;
        public DateTime trs_documentdate
        {
            get { return _trs_documentdate ? _trs_documentdate_value.ToLocalTime() : DateTime.MinValue; }
            set { _trs_documentdate = true; _trs_documentdate_value = value.ToLocalTime(); }
        }

        private bool _trs_synctime = false;
        private DateTime _trs_synctime_value;
        public DateTime trs_synctime
        {
            get { return _trs_synctime ? _trs_synctime_value.ToLocalTime() : DateTime.MinValue; }
            set { _trs_synctime = true; _trs_synctime_value = value.ToLocalTime(); }
        }

        private bool _trs_documentlink = false;
        private Uri _trs_documentlink_value;
        public Uri trs_documentlink
        {
            get { return _trs_documentlink ? _trs_documentlink_value : null; }
            set { _trs_documentlink = true; _trs_documentlink_value = value; }
        }

        private bool _trs_frommobile = false;
        private bool _trs_frommobile_value;
        public bool trs_frommobile
        {
            get { return _trs_frommobile ? _trs_frommobile_value : false; }
            set { _trs_frommobile = true; _trs_frommobile_value = value; }
        }

        private bool _trs_isarrived = false;
        private bool _trs_isarrived_value;
        public bool trs_isarrived
        {
            get { return _trs_isarrived ? _trs_isarrived_value : false; }
            set { _trs_isarrived = true; _trs_isarrived_value = value; }
        }

        private bool _trs_lasterror = false;
        private string _trs_lasterror_value;
        public string trs_lasterror
        {
            get { return _trs_lasterror ? _trs_lasterror_value : null; }
            set { _trs_lasterror = true; _trs_lasterror_value = value; }
        }

        private bool _trs_lastfilename = false;
        private string _trs_lastfilename_value;
        public string trs_lastfilename
        {
            get { return _trs_lastfilename ? _trs_lastfilename_value : null; }
            set { _trs_lastfilename = true; _trs_lastfilename_value = value; }
        }

        private bool _trs_estimationhour = false;
        private decimal _trs_estimationhour_value;
        public decimal trs_estimationhour
        {
            get { return _trs_estimationhour ? _trs_estimationhour_value : 0; }
            set { _trs_estimationhour = true; _trs_estimationhour_value = value; }
        }

        private bool _trs_isdispatched = false;
        private bool _trs_isdispatched_value;
        public bool trs_isdispatched
        {
            get { return _trs_isdispatched ? _trs_isdispatched_value : false; }
            set { _trs_isdispatched = true; _trs_isdispatched_value = value; }
        }

        private bool _trs_functionallocation = false;
        private EntityReference _trs_functionallocation_value;
        public Guid trs_functionallocation
        {
            get { return _trs_functionallocation ? _trs_functionallocation_value.Id : Guid.Empty; }
            set { _trs_functionallocation = true; _trs_functionallocation_value = new EntityReference(_DL_trs_functionallocation.EntityName, value); }
        }

        private bool _trs_equipment = false;
        private EntityReference _trs_equipment_value;
        public Guid trs_equipment
        {
            get { return _trs_equipment ? _trs_equipment_value.Id : Guid.Empty; }
            set { _trs_equipment = true; _trs_equipment_value = new EntityReference(_DL_new_population.EntityName, value); }
        }

        private bool _new_serialnumber = false;
        private string _new_serialnumber_value;
        public string new_serialnumber
        {
            get { return _new_serialnumber ? _new_serialnumber_value : null; }
            set { _new_serialnumber = true; _new_serialnumber_value = value; }
        }

        private bool _trs_product = false;
        private string _trs_product_value;
        public string trs_product
        {
            get { return _trs_product ? _trs_product_value : null; }
            set { _trs_product = true; _trs_product_value = value; }
        }

        private bool _trs_productmodel = false;
        private string _trs_productmodel_value;
        public string trs_productmodel
        {
            get { return _trs_productmodel ? _trs_productmodel_value : null; }
            set { _trs_productmodel = true; _trs_productmodel_value = value; }
        }

        private bool _new_deliverydate = false;
        private DateTime _new_deliverydate_value;
        public DateTime new_deliverydate
        {
            get { return _new_deliverydate ? _new_deliverydate_value.ToLocalTime() : DateTime.MinValue; }
            set { _new_deliverydate = true; _new_deliverydate_value = value.ToLocalTime(); }
        }

        private bool _trs_enginenumber = false;
        private string _trs_enginenumber_value;
        public string trs_enginenumber
        {
            get { return _trs_enginenumber ? _trs_enginenumber_value : null; }
            set { _trs_enginenumber = true; _trs_enginenumber_value = value; }
        }

        private bool _trs_chasisnumber = false;
        private string _trs_chasisnumber_value;
        public string trs_chasisnumber
        {
            get { return _trs_chasisnumber ? _trs_chasisnumber_value : null; }
            set { _trs_chasisnumber = true; _trs_chasisnumber_value = value; }
        }

        private bool _trs_hourmeter = false;
        private decimal _trs_hourmeter_value;
        public decimal trs_hourmeter
        {
            get { return _trs_hourmeter ? _trs_hourmeter_value : 0; }
            set { _trs_hourmeter = true; _trs_hourmeter_value = value; }
        }

        private bool _trs_lasthourmeter = false;
        private decimal _trs_lasthourmeter_value;
        public decimal trs_lasthourmeter
        {
            get { return _trs_lasthourmeter ? _trs_lasthourmeter_value : 0; }
            set { _trs_lasthourmeter = true; _trs_lasthourmeter_value = value; }
        }

        private bool _trs_phone = false;
        private string _trs_phone_value;
        public string trs_phone
        {
            get { return _trs_phone ? _trs_phone_value : null; }
            set { _trs_phone = true; _trs_phone_value = value; }
        }

        private bool _trs_npwp = false;
        private string _trs_npwp_value;
        public string trs_npwp
        {
            get { return _trs_npwp ? _trs_npwp_value : null; }
            set { _trs_npwp = true; _trs_npwp_value = value; }
        }

        private bool _trs_address = false;
        private string _trs_address_value;
        public string trs_address
        {
            get { return _trs_address ? _trs_address_value : null; }
            set { _trs_address = true; _trs_address_value = value; }
        }

        private bool _trs_contactperson = false;
        private EntityReference _trs_contactperson_value;
        public Guid trs_contactperson
        {
            get { return _trs_contactperson ? _trs_contactperson_value.Id : Guid.Empty; }
            set { _trs_contactperson = true; _trs_contactperson_value = new EntityReference(_DL_contact.EntityName, value); }
        }

        private bool _trs_cponsite = false;
        private EntityReference _trs_cponsite_value;
        public Guid trs_cponsite
        {
            get { return _trs_cponsite ? _trs_cponsite_value.Id : Guid.Empty; }
            set { _trs_cponsite = true; _trs_cponsite_value = new EntityReference(_DL_contact.EntityName, value); }
        }

        private bool _trs_cpphone = false;
        private string _trs_cpphone_value;
        public string trs_cpphone
        {
            get { return _trs_cpphone ? _trs_cpphone_value : null; }
            set { _trs_cpphone = true; _trs_cpphone_value = value; }
        }

        private bool _trs_phoneonsite = false;
        private string _trs_phoneonsite_value;
        public string trs_phoneonsite
        {
            get { return _trs_phoneonsite ? _trs_phoneonsite_value : null; }
            set { _trs_phoneonsite = true; _trs_phoneonsite_value = value; }
        }

        private bool _trs_pmacttype = false;
        private EntityReference _trs_pmacttype_value;
        public Guid trs_pmacttype
        {
            get { return _trs_pmacttype ? _trs_pmacttype_value.Id : Guid.Empty; }
            set { _trs_pmacttype = true; _trs_pmacttype_value = new EntityReference(_DL_trs_tasklistgroup.EntityName, value); }
        }

        private bool _trs_inspectorsuggestion = false;
        private EntityReference _trs_inspectorsuggestion_value;
        public Guid trs_inspectorsuggestion
        {
            get { return _trs_inspectorsuggestion ? _trs_inspectorsuggestion_value.Id : Guid.Empty; }
            set { _trs_inspectorsuggestion = true; _trs_inspectorsuggestion_value = new EntityReference(_DL_trs_tasklistgroup.EntityName, value); }
        }

        private bool _trs_profitcenter = false;
        private EntityReference _trs_profitcenter_value;
        public Guid trs_profitcenter
        {
            get { return _trs_profitcenter ? _trs_profitcenter_value.Id : Guid.Empty; }
            set { _trs_profitcenter = true; _trs_profitcenter_value = new EntityReference(_DL_trs_profitcenter.EntityName, value); }
        }

        private bool _trs_acttype = false;
        private EntityReference _trs_acttype_value;
        public Guid trs_acttype
        {
            get { return _trs_acttype ? _trs_acttype_value.Id : Guid.Empty; }
            set { _trs_acttype = true; _trs_acttype_value = new EntityReference(_DL_trs_acttype.EntityName, value); }
        }

        private bool _trs_workcenter = false;
        private EntityReference _trs_workcenter_value;
        public Guid trs_workcenter
        {
            get { return _trs_workcenter ? _trs_workcenter_value.Id : Guid.Empty; }
            set { _trs_workcenter = true; _trs_workcenter_value = new EntityReference(_DL_trs_workcenter.EntityName, value); }
        }

        private bool _trs_responsiblecctr = false;
        private EntityReference _trs_responsiblecctr_value;
        public Guid trs_responsiblecctr
        {
            get { return _trs_responsiblecctr ? _trs_responsiblecctr_value.Id : Guid.Empty; }
            set { _trs_responsiblecctr = true; _trs_responsiblecctr_value = new EntityReference(_DL_trs_responsiblecostcenter.EntityName, value); }
        }

        private bool _trs_mechanicleader = false;
        private EntityReference _trs_mechanicleader_value;
        public Guid trs_mechanicleader
        {
            get { return _trs_mechanicleader ? _trs_mechanicleader_value.Id : Guid.Empty; }
            set { _trs_mechanicleader = true; _trs_mechanicleader_value = new EntityReference(_DL_equipment.EntityName, value); }
        }

        private bool _scheduledstart = false;
        private DateTime _scheduledstart_value;
        public DateTime scheduledstart
        {
            get { return _scheduledstart ? _scheduledstart_value.ToLocalTime() : DateTime.MinValue; }
            set { _scheduledstart = true; _scheduledstart_value = value.ToLocalTime(); }
        }

        private bool _scheduledend = false;
        private DateTime _scheduledend_value;
        public DateTime scheduledend
        {
            get { return _scheduledend ? _scheduledend_value.ToLocalTime() : DateTime.MinValue; }
            set { _scheduledend = true; _scheduledend_value = value.ToLocalTime(); }
        }

        private bool _customers = false;
        private EntityCollection _customers_value;
        public EntityCollection customers
        {
            get { return _customers ? _customers_value : null; }
            set { _customers = true; _customers_value = value; }
        }

        private bool _trs_accind = false;
        private OptionSetValue _trs_accind_value;
        public OptionSetValue trs_accind
        {
            get { return _trs_accind ? _trs_accind_value : null; }
            set { _trs_accind = true; _trs_accind_value = value; }
        }

        private bool _trs_crmwonumber = false;
        private string _trs_crmwonumber_value;
        public string trs_crmwonumber
        {
            get { return _trs_crmwonumber ? _trs_crmwonumber_value : string.Empty; }
            set { _trs_crmwonumber = true; _trs_crmwonumber_value = value; }
        }

        private bool _trs_branch = false;
        private EntityReference _trs_branch_value;
        public Guid trs_branch
        {
            get { return _trs_branch ? _trs_branch_value.Id : Guid.Empty; }
            set { _trs_branch = true; _trs_branch_value = new EntityReference(_DL_businessunit.EntityName, value); }
        }

        private bool _trs_printingsequence = false;
        private double _trs_printingsequence_value;
        public double trs_printingsequence
        {
            get { return _trs_printingsequence ? _trs_printingsequence_value : 0; }
            set { _trs_printingsequence = true; _trs_printingsequence_value = value; }
        }

        private bool _trs_fillingnumber = false;
        private string _trs_fillingnumber_value;
        public string trs_fillingnumber
        {
            get { return _trs_fillingnumber ? _trs_fillingnumber_value : string.Empty; }
            set { _trs_fillingnumber = true; _trs_fillingnumber_value = value; }
        }

        private bool _description = false;
        private string _description_value;
        public string description
        {
            get { return _description ? _description_value : string.Empty; }
            set { _description = true; _description_value = value; }
        }

        private bool _serviceid = false;
        private EntityReference _serviceid_value;
        public Guid serviceid
        {
            get { return _serviceid ? _serviceid_value.Id : Guid.Empty; }
            set { _serviceid = true; _serviceid_value = new EntityReference(_DL_service.EntityName, value); }
        }

        private bool _trs_plant = false;
        private EntityReference _trs_plant_value;
        public Guid trs_plant
        {
            get { return _trs_plant ? _trs_plant_value.Id : Guid.Empty; }
            set { _trs_plant = true; _trs_plant_value = new EntityReference(_DL_site.EntityName, value); }
        }

        private bool _regardingobjectid = false;
        private EntityReference _regardingobjectid_value;
        public EntityReference regardingobjectid
        {
            get { return _regardingobjectid ? _regardingobjectid_value : null; }
            set { _regardingobjectid = true; _regardingobjectid_value = value; }
        }

        private bool _resources = false;
        private EntityCollection _resources_value;
        public EntityCollection resources
        {
            get { return _resources ? _resources_value : null; }
            set { _resources = true; _resources_value = value; }
        }

        private bool _trs_inspectorcomments = false;
        private string _trs_inspectorcomments_value;
        public string trs_inspectorcomments
        {
            get { return _trs_inspectorcomments ? _trs_inspectorcomments_value : null; }
            set { _trs_inspectorcomments = true; _trs_inspectorcomments_value = value; }
        }

        private bool _trs_customercomments = false;
        private string _trs_customercomments_value;
        public string trs_customercomments
        {
            get { return _trs_customercomments ? _trs_customercomments_value : null; }
            set { _trs_customercomments = true; _trs_customercomments_value = value; }
        }

        private bool _trs_customersatisfaction = false;
        private string _trs_customersatisfaction_value;
        public string trs_customersatisfaction
        {
            get { return _trs_customersatisfaction ? _trs_customersatisfaction_value : null; }
            set { _trs_customersatisfaction = true; _trs_customersatisfaction_value = value; }
        }

        private bool _trs_statusinoperation = false;
        private OptionSetValue _trs_statusinoperation_value;
        public int trs_statusinoperation
        {
            get { return _trs_statusinoperation ? _trs_statusinoperation_value.Value : int.MinValue; }
            set { _trs_statusinoperation = true; _trs_statusinoperation_value = new OptionSetValue(value); }
        }
        #endregion

        public Entity Select(IOrganizationService organizationService, Guid id)
        {
            try
            {
                return organizationService.Retrieve(_entityname, id, new ColumnSet(true));
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Select : " + ex.Message.ToString());
            }
        }

        public EntityCollection Select(IOrganizationService organizationService, QueryExpression queryExpression)
        {
            try
            {
                return organizationService.RetrieveMultiple(queryExpression);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Select : " + ex.Message.ToString());
            }
        }

        public Guid Insert(IOrganizationService organizationService)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                if (_subject) { entity.Attributes["subject"] = _subject_value; }
                if (_actualstart) { entity.Attributes["actualstart"] = _actualstart_value; }
                if (_trs_documentdate) { entity.Attributes["trs_documentdate"] = _trs_documentdate_value; }
                if (_trs_documentlink) { entity.Attributes["trs_documentlink"] = _trs_documentlink_value; }
                if (_trs_synctime) { entity.Attributes["trs_synctime"] = _trs_synctime_value; }
                if (_trs_frommobile) { entity.Attributes["trs_frommobile"] = _trs_frommobile_value; }
                if (_trs_isarrived) { entity.Attributes["trs_isarrived"] = _trs_isarrived_value; }
                if (_trs_lasterror) { entity.Attributes["trs_lasterror"] = _trs_lasterror_value; }
                if (_trs_lastfilename) { entity.Attributes["trs_lastfilename"] = _trs_lastfilename_value; }
                if (_trs_estimationhour) { entity.Attributes["trs_estimationhour"] = _trs_estimationhour_value; }
                if (_trs_isdispatched) { entity.Attributes["trs_isdispatched"] = _trs_isdispatched_value; }
                if (_trs_functionallocation) { entity.Attributes["trs_functionallocation"] = _trs_functionallocation_value; }
                if (_trs_equipment) { entity.Attributes["trs_equipment"] = _trs_equipment_value; }
                if (_new_serialnumber) { entity.Attributes["new_serialnumber"] = _new_serialnumber_value; }
                if (_trs_product) { entity.Attributes["trs_product"] = _trs_product_value; }
                if (_trs_productmodel) { entity.Attributes["trs_productmodel"] = _trs_productmodel_value; }
                if (_new_deliverydate) { entity.Attributes["new_deliverydate"] = _new_deliverydate_value; }
                if (_trs_enginenumber) { entity.Attributes["trs_enginenumber"] = _trs_enginenumber_value; }
                if (_trs_chasisnumber) { entity.Attributes["trs_chasisnumber"] = _trs_chasisnumber_value; }
                if (_trs_hourmeter) { entity.Attributes["trs_hourmeter"] = _trs_hourmeter_value; }
                if (_trs_lasthourmeter) { entity.Attributes["trs_lasthourmeter"] = _trs_lasthourmeter_value; }
                if (_trs_phone) { entity.Attributes["trs_phone"] = _trs_phone_value; }
                if (_trs_npwp) { entity.Attributes["trs_npwp"] = _trs_npwp_value; }
                if (_trs_address) { entity.Attributes["trs_address"] = _trs_address_value; }
                if (_trs_contactperson) { entity.Attributes["trs_contactperson"] = _trs_contactperson_value; }
                if (_trs_cponsite) { entity.Attributes["trs_cponsite"] = _trs_cponsite_value; }
                if (_trs_cpphone) { entity.Attributes["trs_cpphone"] = _trs_cpphone_value; }
                if (_trs_phoneonsite) { entity.Attributes["trs_phoneonsite"] = _trs_phoneonsite_value; }
                if (_trs_pmacttype) { entity.Attributes["trs_pmacttype"] = _trs_pmacttype_value; }
                if (_trs_inspectorsuggestion) { entity.Attributes["trs_inspectorsuggestion"] = _trs_inspectorsuggestion_value; }
                if (_trs_profitcenter) { entity.Attributes["trs_profitcenter"] = _trs_profitcenter_value; }
                if (_trs_acttype) { entity.Attributes["trs_acttype"] = _trs_acttype_value; }
                if (_trs_workcenter) { entity.Attributes["trs_workcenter"] = _trs_workcenter_value; }
                if (_trs_responsiblecctr) { entity.Attributes["trs_responsiblecctr"] = _trs_responsiblecctr_value; }
                if (_trs_mechanicleader) { entity.Attributes["trs_mechanicleader"] = _trs_mechanicleader_value; }
                if (_scheduledstart) { entity.Attributes["scheduledstart"] = _scheduledstart_value; }
                if (_scheduledend) { entity.Attributes["scheduledend"] = _scheduledend_value; }
                if (_customers) { entity.Attributes["customers"] = _customers_value; }
                if (_trs_accind) { entity.Attributes["trs_accind"] = _trs_accind_value; }
                if (_trs_crmwonumber) { entity.Attributes["trs_crmwonumber"] = _trs_crmwonumber_value; }
                if (_trs_branch) { entity.Attributes["trs_branch"] = _trs_branch_value; }
                if (_trs_printingsequence) { entity.Attributes["trs_printingsequence"] = _trs_printingsequence_value; }
                if (_trs_fillingnumber) { entity.Attributes["trs_fillingnumber"] = _trs_fillingnumber_value; }
                if (_description) { entity.Attributes["description"] = _description_value; }
                if (_serviceid) { entity.Attributes["serviceid"] = _serviceid_value; }
                if (_trs_plant) { entity.Attributes["trs_plant"] = _trs_plant_value; }
                if (_regardingobjectid) { entity.Attributes["regardingobjectid"] = _regardingobjectid_value; }
                if (_resources) { entity.Attributes["resources"] = _resources_value; }
                if (_trs_statusinoperation) { entity.Attributes["trs_statusinoperation"] = _trs_statusinoperation_value; }
                return organizationService.Create(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Insert : " + ex.Message.ToString());
            }
        }

        public void Update(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                entity.Id = id;
                if (_subject) { entity.Attributes["subject"] = _subject_value; }
                if (_actualstart) { entity.Attributes["actualstart"] = _actualstart_value; }
                if (_trs_documentdate) { entity.Attributes["trs_documentdate"] = _trs_documentdate_value; }
                if (_trs_documentlink) { entity.Attributes["trs_documentlink"] = _trs_documentlink_value; }
                if (_trs_synctime) { entity.Attributes["trs_synctime"] = _trs_synctime_value; }
                if (_trs_frommobile) { entity.Attributes["trs_frommobile"] = _trs_frommobile_value; }
                if (_trs_isarrived) { entity.Attributes["trs_isarrived"] = _trs_isarrived_value; }
                if (_trs_lasterror) { entity.Attributes["trs_lasterror"] = _trs_lasterror_value; }
                if (_trs_lastfilename) { entity.Attributes["trs_lastfilename"] = _trs_lastfilename_value; }
                if (_trs_estimationhour) { entity.Attributes["trs_estimationhour"] = _trs_estimationhour_value; }
                if (_trs_isdispatched) { entity.Attributes["trs_isdispatched"] = _trs_isdispatched_value; }
                if (_trs_functionallocation) { entity.Attributes["trs_functionallocation"] = _trs_functionallocation_value; }
                if (_trs_equipment) { entity.Attributes["trs_equipment"] = _trs_equipment_value; }
                if (_new_serialnumber) { entity.Attributes["new_serialnumber"] = _new_serialnumber_value; }
                if (_trs_product) { entity.Attributes["trs_product"] = _trs_product_value; }
                if (_trs_productmodel) { entity.Attributes["trs_productmodel"] = _trs_productmodel_value; }
                if (_new_deliverydate) { entity.Attributes["new_deliverydate"] = _new_deliverydate_value; }
                if (_trs_enginenumber) { entity.Attributes["trs_enginenumber"] = _trs_enginenumber_value; }
                if (_trs_chasisnumber) { entity.Attributes["trs_chasisnumber"] = _trs_chasisnumber_value; }
                if (_trs_hourmeter) { entity.Attributes["trs_hourmeter"] = _trs_hourmeter_value; }
                if (_trs_lasthourmeter) { entity.Attributes["trs_lasthourmeter"] = _trs_lasthourmeter_value; }
                if (_trs_phone) { entity.Attributes["trs_phone"] = _trs_phone_value; }
                if (_trs_npwp) { entity.Attributes["trs_npwp"] = _trs_npwp_value; }
                if (_trs_address) { entity.Attributes["trs_address"] = _trs_address_value; }
                if (_trs_contactperson) { entity.Attributes["trs_contactperson"] = _trs_contactperson_value; }
                if (_trs_cponsite) { entity.Attributes["trs_cponsite"] = _trs_cponsite_value; }
                if (_trs_cpphone) { entity.Attributes["trs_cpphone"] = _trs_cpphone_value; }
                if (_trs_phoneonsite) { entity.Attributes["trs_phoneonsite"] = _trs_phoneonsite_value; }
                if (_trs_pmacttype) { entity.Attributes["trs_pmacttype"] = _trs_pmacttype_value; }
                if (_trs_inspectorsuggestion) { entity.Attributes["trs_inspectorsuggestion"] = _trs_inspectorsuggestion_value; }
                if (_trs_profitcenter) { entity.Attributes["trs_profitcenter"] = _trs_profitcenter_value; }
                if (_trs_acttype) { entity.Attributes["trs_acttype"] = _trs_acttype_value; }
                if (_trs_workcenter) { entity.Attributes["trs_workcenter"] = _trs_workcenter_value; }
                if (_trs_responsiblecctr) { entity.Attributes["trs_responsiblecctr"] = _trs_responsiblecctr_value; }
                if (_trs_mechanicleader) { entity.Attributes["trs_mechanicleader"] = _trs_mechanicleader_value; }
                if (_scheduledstart) { entity.Attributes["scheduledstart"] = _scheduledstart_value; }
                if (_scheduledend) { entity.Attributes["scheduledend"] = _scheduledend_value; }
                if (_customers) { entity.Attributes["customers"] = _customers_value; }
                if (_trs_accind) { entity.Attributes["trs_accind"] = _trs_accind_value; }
                if (_trs_crmwonumber) { entity.Attributes["trs_crmwonumber"] = _trs_crmwonumber_value; }
                if (_trs_branch) { entity.Attributes["trs_branch"] = _trs_branch_value; }
                if (_trs_printingsequence) { entity.Attributes["trs_printingsequence"] = _trs_printingsequence_value; }
                if (_trs_fillingnumber) { entity.Attributes["trs_fillingnumber"] = _trs_fillingnumber_value; }
                if (_description) { entity.Attributes["description"] = _description_value; }
                if (_serviceid) { entity.Attributes["serviceid"] = _serviceid_value; }
                if (_trs_plant) { entity.Attributes["trs_plant"] = _trs_plant_value; }
                if (_regardingobjectid) { entity.Attributes["regardingobjectid"] = _regardingobjectid_value; }
                if (_resources) { entity.Attributes["resources"] = _resources_value; }
                if (_trs_statusinoperation) { entity.Attributes["trs_statusinoperation"] = _trs_statusinoperation_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }

        public void New(IOrganizationService organizationService, Guid id)
        {
            try
            {
                EntityReference entityReference = new EntityReference(_entityname, id);
                OrganizationRequest organizationRequest = new OrganizationRequest();
                organizationRequest.RequestName = "SetState";
                organizationRequest["EntityMoniker"] = entityReference;
                organizationRequest["State"] = new OptionSetValue(0);
                organizationRequest["Status"] = new OptionSetValue(1);
                organizationService.Execute(organizationRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Hold : " + ex.Message);
            }
        }

        public void Hold(IOrganizationService organizationService, Guid id)
        {
            try
            {
                EntityReference entityReference = new EntityReference(_entityname, id);
                OrganizationRequest organizationRequest = new OrganizationRequest();
                organizationRequest.RequestName = "SetState";
                organizationRequest["EntityMoniker"] = entityReference;
                organizationRequest["State"] = new OptionSetValue(3);
                organizationRequest["Status"] = new OptionSetValue(3);
                organizationService.Execute(organizationRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Hold : " + ex.Message);
            }
        }

        public void Unhold(IOrganizationService organizationService, Guid id)
        {
            try
            {
                int status;
                Entity entity = Select(organizationService, id);
                if (entity.Attributes.Contains("actualstart"))
                    status = 6;
                else if (entity.Attributes.Contains("trs_isarrived") && entity.GetAttributeValue<bool>("trs_isarrived"))
                    status = 7;
                else
                    status = 4;
                EntityReference entityReference = new EntityReference(_entityname, id);
                OrganizationRequest organizationRequest = new OrganizationRequest();
                organizationRequest.RequestName = "SetState";
                organizationRequest["EntityMoniker"] = entityReference;
                organizationRequest["State"] = new OptionSetValue(3);
                organizationRequest["Status"] = new OptionSetValue(status);
                organizationService.Execute(organizationRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Unhold : " + ex.Message);
            }
        }

        public void Released(IOrganizationService organizationService, Guid id)
        {
            try
            {
                EntityReference entityReference = new EntityReference(_entityname, id);
                OrganizationRequest organizationRequest = new OrganizationRequest();
                organizationRequest.RequestName = "SetState";
                organizationRequest["EntityMoniker"] = entityReference;
                organizationRequest["State"] = new OptionSetValue(0);
                organizationRequest["Status"] = new OptionSetValue(2);
                organizationService.Execute(organizationRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Release : " + ex.Message);
            }
        }

        public void Dispatched(IOrganizationService organizationService, Guid id)
        {
            try
            {
                EntityReference entityReference = new EntityReference(_entityname, id);
                OrganizationRequest organizationRequest = new OrganizationRequest();
                organizationRequest.RequestName = "SetState";
                organizationRequest["EntityMoniker"] = entityReference;
                organizationRequest["State"] = new OptionSetValue(3);
                organizationRequest["Status"] = new OptionSetValue(4);
                organizationService.Execute(organizationRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Dispatched : " + ex.Message);
            }
        }

        public void Arrived(IOrganizationService organizationService, Guid id)
        {
            try
            {
                EntityReference entityReference = new EntityReference(_entityname, id);
                OrganizationRequest organizationRequest = new OrganizationRequest();
                organizationRequest.RequestName = "SetState";
                organizationRequest["EntityMoniker"] = entityReference;
                organizationRequest["State"] = new OptionSetValue(3);
                organizationRequest["Status"] = new OptionSetValue(7);
                organizationService.Execute(organizationRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Arrived : " + ex.Message);
            }
        }

        public void InProgress(IOrganizationService organizationService, Guid id)
        {
            try
            {
                EntityReference entityReference = new EntityReference(_entityname, id);
                OrganizationRequest organizationRequest = new OrganizationRequest();
                organizationRequest.RequestName = "SetState";
                organizationRequest["EntityMoniker"] = entityReference;
                organizationRequest["State"] = new OptionSetValue(3);
                organizationRequest["Status"] = new OptionSetValue(6);
                organizationService.Execute(organizationRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".InProgress : " + ex.Message);
            }
        }

        public void SubmitTECObyMechanic(IOrganizationService organizationService, Guid id)
        {
            try
            {
                EntityReference entityReference = new EntityReference(_entityname, id);
                OrganizationRequest organizationRequest = new OrganizationRequest();
                organizationRequest.RequestName = "SetState";
                organizationRequest["EntityMoniker"] = entityReference;
                organizationRequest["State"] = new OptionSetValue(1);
                organizationRequest["Status"] = new OptionSetValue(167630003);
                organizationService.Execute(organizationRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SubmitTECObyMechanic : " + ex.Message);
            }
        }

        public void SubmitTECObySDH(IOrganizationService organizationService, Guid id)
        {
            try
            {
                EntityReference entityReference = new EntityReference(_entityname, id);
                OrganizationRequest organizationRequest = new OrganizationRequest();
                organizationRequest.RequestName = "SetState";
                organizationRequest["EntityMoniker"] = entityReference;
                organizationRequest["State"] = new OptionSetValue(1);
                organizationRequest["Status"] = new OptionSetValue(167630002);
                organizationService.Execute(organizationRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SubmitTECObySDH : " + ex.Message);
            }
        }

        public void Completed(IOrganizationService organizationService, Guid id)
        {
            try
            {
                EntityReference entityReference = new EntityReference(_entityname, id);
                OrganizationRequest organizationRequest = new OrganizationRequest();
                organizationRequest.RequestName = "SetState";
                organizationRequest["EntityMoniker"] = entityReference;
                organizationRequest["State"] = new OptionSetValue(1);
                organizationRequest["Status"] = new OptionSetValue(8);
                organizationService.Execute(organizationRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SubmitTECObySDH : " + ex.Message);
            }
        }

        public void AssigntoTeam(IOrganizationService organizationService, Guid id, Guid teamId)
        {
            try
            {
                AssignRequest assign = new AssignRequest
                {
                    Assignee = new EntityReference(_DL_team.EntityName, teamId),
                    Target = new EntityReference(_entityname, id)
                };
                organizationService.Execute(assign);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".AssigntoTeam : " + ex.Message);
            }
        }

        public void AssigntoUser(IOrganizationService organizationService, Guid id, Guid userId)
        {
            try
            {
                AssignRequest assign = new AssignRequest
                {
                    Assignee = new EntityReference(_DL_systemuser.EntityName, userId),
                    Target = new EntityReference(_entityname, id)
                };
                organizationService.Execute(assign);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".AssigntoUser : " + ex.Message);
            }
        }
    }
}
