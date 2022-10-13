using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_technicalservicereport
    {
        #region Dependencies
        private DL_serviceappointment _DL_serviceappointment = new DL_serviceappointment();
        private DL_new_population _DL_new_population = new DL_new_population();
        private DL_trs_producttype _DL_trs_producttype = new DL_trs_producttype();
        private DL_trs_masterpart _DL_trs_masterpart = new DL_trs_masterpart();
        #endregion

        #region Properties
        private string _classname = "DL_trs_technicalservicereport";

        private string _entityname = "trs_technicalservicereport";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Technical Service Report";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _trs_tsrnumber = false;
        private string _trs_tsrnumber_value;
        public string trs_tsrnumber
        {
            get { return _trs_tsrnumber ? _trs_tsrnumber_value : string.Empty; }
            set { _trs_tsrnumber = true; _trs_tsrnumber_value = value; }
        }

        private bool _trs_workorder = false;
        private EntityReference _trs_workorder_value;
        public Guid trs_workorder
        {
            get { return _trs_workorder ? _trs_workorder_value.Id : Guid.Empty; }
            set { _trs_workorder = true; _trs_workorder_value = new EntityReference(_DL_serviceappointment.EntityName, value); }
        }

        private bool _trs_equipment = false;
        private EntityReference _trs_equipment_value;
        public Guid trs_equipment
        {
            get { return _trs_equipment ? _trs_equipment_value.Id : Guid.Empty; }
            set { _trs_equipment = true; _trs_equipment_value = new EntityReference(_DL_new_population.EntityName, value); }
        }

        private bool _trs_symptom = false;
        private string _trs_symptom_value;
        public string trs_symptom
        {
            get { return _trs_symptom ? _trs_symptom_value : string.Empty; }
            set { _trs_symptom = true; _trs_symptom_value = value; }
        }

        private bool _trs_operatingcondition = false;
        private bool _trs_operatingcondition_value;
        public bool trs_operatingcondition
        {
            get { return _trs_operatingcondition ? _trs_operatingcondition_value : false; }
            set { _trs_operatingcondition = true; _trs_operatingcondition_value = value; }
        }

        private bool _trs_conditiondescription = false;
        private string _trs_conditiondescription_value;
        public string trs_conditiondescription
        {
            get { return _trs_conditiondescription ? _trs_conditiondescription_value : string.Empty; }
            set { _trs_conditiondescription = true; _trs_conditiondescription_value = value; }
        }

        private bool _trs_producttype = false;
        private EntityReference _trs_producttype_value;
        public Guid trs_producttype
        {
            get { return _trs_producttype ? _trs_producttype_value.Id : Guid.Empty; }
            set { _trs_producttype = true; _trs_producttype_value = new EntityReference(_DL_trs_producttype.EntityName, value); }
        }

        private bool _trs_application = false;
        private OptionSetValue _trs_application_value;
        public int trs_application
        {
            get { return _trs_application ? _trs_application_value.Value : int.MinValue; }
            set { _trs_application = true; _trs_application_value = new OptionSetValue(value); }
        }

        private bool _trs_typeofsoil = false;
        private OptionSetValue _trs_typeofsoil_value;
        public int trs_typeofsoil
        {
            get { return _trs_typeofsoil ? _trs_typeofsoil_value.Value : int.MinValue; }
            set { _trs_typeofsoil = true; _trs_typeofsoil_value = new OptionSetValue(value); }
        }

        private bool _trs_gensettype = false;
        private OptionSetValue _trs_gensettype_value;
        public int trs_gensettype
        {
            get { return _trs_gensettype ? _trs_gensettype_value.Value : int.MinValue; }
            set { _trs_gensettype = true; _trs_gensettype_value = new OptionSetValue(value); }
        }

        private bool _trs_sector = false;
        private OptionSetValue _trs_sector_value;
        public int trs_sector
        {
            get { return _trs_sector ? _trs_sector_value.Value : int.MinValue; }
            set { _trs_sector = true; _trs_sector_value = new OptionSetValue(value); }
        }
        
        private bool _trs_repairdate = false;
        private DateTime _trs_repairdate_value;
        public DateTime trs_repairdate
        {
            get { return _trs_repairdate ? _trs_repairdate_value.ToLocalTime() : DateTime.MinValue; }
            set { _trs_repairdate = true; _trs_repairdate_value = value.ToLocalTime(); }
        }
        
        private bool _trs_troubledate = false;
        private DateTime _trs_troubledate_value;
        public DateTime trs_troubledate
        {
            get { return _trs_troubledate ? _trs_troubledate_value.ToLocalTime() : DateTime.MinValue; }
            set { _trs_troubledate = true; _trs_troubledate_value = value.ToLocalTime(); }
        }

        private bool _trs_partscaused = false;
        private EntityReference _trs_partscaused_value;
        public Guid trs_partscaused
        {
            get { return _trs_partscaused ? _trs_partscaused_value.Id : Guid.Empty; }
            set { _trs_partscaused = true; _trs_partscaused_value = new EntityReference(_DL_trs_masterpart.EntityName, value); }
        }

        private bool _trs_technicalanalysis = false;
        private string _trs_technicalanalysis_value;
        public string trs_technicalanalysis
        {
            get { return _trs_technicalanalysis ? _trs_technicalanalysis_value : string.Empty; }
            set { _trs_technicalanalysis = true; _trs_technicalanalysis_value = value; }
        }

        private bool _trs_correctiontaken = false;
        private string _trs_correctiontaken_value;
        public string trs_correctiontaken
        {
            get { return _trs_correctiontaken ? _trs_correctiontaken_value : string.Empty; }
            set { _trs_correctiontaken = true; _trs_correctiontaken_value = value; }
        }
        
        private bool _trs_warrantystatus = false;
        private bool _trs_warrantystatus_value;
        public bool trs_warrantystatus
        {
            get { return _trs_warrantystatus ? _trs_warrantystatus_value : false; }
            set { _trs_warrantystatus = true; _trs_warrantystatus_value = value; }
        }

        private bool _trs_synctime = false;
        private DateTime _trs_synctime_value;
        public DateTime trs_synctime
        {
            get { return _trs_synctime ? _trs_synctime_value.ToLocalTime() : DateTime.MinValue; }
            set { _trs_synctime = true; _trs_synctime_value = value.ToLocalTime(); }
        }

        private bool _trs_frommobile = false;
        private bool _trs_frommobile_value;
        public bool trs_frommobile
        {
            get { return _trs_frommobile ? _trs_frommobile_value : false; }
            set { _trs_frommobile = true; _trs_frommobile_value = value; }
        }

        private bool _trs_workshop = false;
        private bool _trs_workshop_value;
        public bool trs_workshop
        {
            get { return _trs_workshop ? _trs_workshop_value : false; }
            set { _trs_workshop = true; _trs_workshop_value = value; }
        }

        private bool _trs_mobileguid = false;
        private string _trs_mobileguid_value;
        public string trs_mobileguid
        {
            get { return _trs_mobileguid ? _trs_mobileguid_value : string.Empty; }
            set { _trs_mobileguid = true; _trs_mobileguid_value = value; }
        }

        private bool _trs_oldhm = false;
        private int _trs_oldhm_value;
        public int trs_oldhm
        {
            get { return _trs_oldhm ? _trs_oldhm_value : 0; }
            set { _trs_oldhm = true; _trs_oldhm_value = value; }
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
                //if (_trs_tsrnumber)
                //{
                    Entity entity = new Entity(_entityname);
                    if (_trs_tsrnumber) { entity.Attributes["trs_tsrnumber"] = _trs_tsrnumber_value; }
                    if (_trs_workorder) { entity.Attributes["trs_workorder"] = _trs_workorder_value; }
                    if (_trs_equipment) { entity.Attributes["trs_equipment"] = _trs_equipment_value; }
                    if (_trs_symptom) { entity.Attributes["trs_symptom"] = _trs_symptom_value; }
                    if (_trs_operatingcondition) { entity.Attributes["trs_operatingcondition"] = _trs_operatingcondition_value; }
                    if (_trs_conditiondescription) { entity.Attributes["trs_conditiondescription"] = _trs_conditiondescription_value; }
                    if (_trs_producttype) { entity.Attributes["trs_producttype"] = _trs_producttype_value; }
                    if (_trs_application) { entity.Attributes["trs_application"] = _trs_application_value; }
                    if (_trs_typeofsoil) { entity.Attributes["trs_typeofsoil"] = _trs_typeofsoil_value; }
                    if (_trs_gensettype) { entity.Attributes["trs_gensettype"] = _trs_gensettype_value; }
                    if (_trs_sector) { entity.Attributes["trs_sector"] = _trs_sector_value; }
                    if (_trs_repairdate) { entity.Attributes["trs_repairdate"] = _trs_repairdate_value; }
                    if (_trs_troubledate) { entity.Attributes["trs_troubledate"] = _trs_troubledate_value; }
                    if (_trs_partscaused) { entity.Attributes["trs_partscaused"] = _trs_partscaused_value; }
                    if (_trs_technicalanalysis) { entity.Attributes["trs_technicalanalyis"] = _trs_technicalanalysis_value; }
                    if (_trs_correctiontaken) { entity.Attributes["trs_correctiontaken"] = _trs_correctiontaken_value; }
                    if (_trs_warrantystatus) { entity.Attributes["trs_warrantystatus"] = _trs_warrantystatus_value; }
                    if (_trs_synctime) { entity.Attributes["trs_synctime"] = _trs_synctime_value; }
                    if (_trs_frommobile) { entity.Attributes["trs_frommobile"] = _trs_frommobile_value; }
                    if (_trs_workshop) { entity.Attributes["trs_workshop"] = _trs_workshop_value; }
                    if (_trs_mobileguid) { entity.Attributes["trs_mobileguid"] = _trs_mobileguid_value; }
                    if (_trs_oldhm) { entity.Attributes["trs_oldhm"] = _trs_oldhm_value; }
                    return organizationService.Create(entity);
                //}
                //else
                //{
                //    throw new Exception("Primary Key is empty.");
                //}
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
                if (_trs_tsrnumber) { entity.Attributes["trs_tsrnumber"] = _trs_tsrnumber_value; }
                if (_trs_workorder) { entity.Attributes["trs_workorder"] = _trs_workorder_value; }
                if (_trs_equipment) { entity.Attributes["trs_equipment"] = _trs_equipment_value; }
                if (_trs_symptom) { entity.Attributes["trs_symptom"] = _trs_symptom_value; }
                if (_trs_operatingcondition) { entity.Attributes["trs_operatingcondition"] = _trs_operatingcondition_value; }
                if (_trs_conditiondescription) { entity.Attributes["trs_conditiondescription"] = _trs_conditiondescription_value; }
                if (_trs_producttype) { entity.Attributes["trs_producttype"] = _trs_producttype_value; }
                if (_trs_application) { entity.Attributes["trs_application"] = _trs_application_value; }
                if (_trs_typeofsoil) { entity.Attributes["trs_typeofsoil"] = _trs_typeofsoil_value; }
                if (_trs_gensettype) { entity.Attributes["trs_gensettype"] = _trs_gensettype_value; }
                if (_trs_sector) { entity.Attributes["trs_sector"] = _trs_sector_value; }
                if (_trs_repairdate) { entity.Attributes["trs_repairdate"] = _trs_repairdate_value; }
                if (_trs_troubledate) { entity.Attributes["trs_troubledate"] = _trs_troubledate_value; }
                if (_trs_partscaused) { entity.Attributes["trs_partscaused"] = _trs_partscaused_value; }
                if (_trs_technicalanalysis) { entity.Attributes["trs_technicalanalyis"] = _trs_technicalanalysis_value; }
                if (_trs_correctiontaken) { entity.Attributes["trs_correctiontaken"] = _trs_correctiontaken_value; }
                if (_trs_warrantystatus) { entity.Attributes["trs_warrantystatus"] = _trs_warrantystatus_value; }
                if (_trs_synctime) { entity.Attributes["trs_synctime"] = _trs_synctime_value; }
                if (_trs_frommobile) { entity.Attributes["trs_frommobile"] = _trs_frommobile_value; }
                if (_trs_workshop) { entity.Attributes["trs_workshop"] = _trs_workshop_value; }
                if (_trs_mobileguid) { entity.Attributes["trs_mobileguid"] = _trs_mobileguid_value; }
                if (_trs_oldhm) { entity.Attributes["trs_oldhm"] = _trs_oldhm_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
