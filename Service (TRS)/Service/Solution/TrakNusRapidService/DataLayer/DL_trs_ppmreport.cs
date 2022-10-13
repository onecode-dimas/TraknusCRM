using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_ppmreport
    {
        #region Dependencies
        private DL_serviceappointment _DL_serviceappointment = new DL_serviceappointment();
        private DL_trs_producttype _DL_trs_producttype = new DL_trs_producttype();
        private DL_new_population _DL_new_population = new DL_new_population();
        private DL_trs_unittypeofwork _DL_trs_unittypeofwork = new DL_trs_unittypeofwork();
        #endregion

        #region Properties
        private string _classname = "DL_trs_ppmreport";

        private string _entityname = "trs_ppmreport";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "PPM Report";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _trs_reportnumber = false;
        private string _trs_reportnumber_value;
        public string trs_reportnumber
        {
            get { return _trs_reportnumber ? _trs_reportnumber_value : null; }
            set { _trs_reportnumber = true; _trs_reportnumber_value = value; }
        }

        private bool _trs_workorder = false;
        private EntityReference _trs_workorder_value;
        public Guid trs_workorder
        {
            get { return _trs_workorder ? _trs_workorder_value.Id : Guid.Empty; }
            set { _trs_workorder = true; _trs_workorder_value = new EntityReference(_DL_serviceappointment.EntityName, value); }
        }

        private bool _trs_producttype = false;
        private EntityReference _trs_producttype_value;
        public Guid trs_producttype
        {
            get { return _trs_producttype ? _trs_producttype_value.Id : Guid.Empty; }
            set { _trs_producttype = true; _trs_producttype_value = new EntityReference(_DL_trs_producttype.EntityName, value); }
        }

        private bool _trs_equipment = false;
        private EntityReference _trs_equipment_value;
        public Guid trs_new_population
        {
            get { return _trs_equipment ? _trs_equipment_value.Id : Guid.Empty; }
            set { _trs_equipment = true; _trs_equipment_value = new EntityReference(_DL_new_population.EntityName, value); }
        }

        private bool _trs_typeofwork = false;
        private EntityReference _trs_typeofwork_value;
        public Guid trs_typeofwork
        {
            get { return _trs_typeofwork ? _trs_typeofwork_value.Id : Guid.Empty; }
            set { _trs_typeofwork = true; _trs_typeofwork_value = new EntityReference(_DL_trs_unittypeofwork.EntityName, value); }
        }

        private bool _trs_comments = false;
        private OptionSetValue _trs_comments_value;
        public int trs_comments
        {
            get { return _trs_comments ? _trs_comments_value.Value : int.MinValue; }
            set { _trs_comments = true; _trs_comments_value = new OptionSetValue(value); }
        }

        private bool _trs_machinecondition = false;
        private OptionSetValue _trs_machinecondition_value;
        public int trs_machinecondition
        {
            get { return _trs_machinecondition ? _trs_machinecondition_value.Value : int.MinValue; }
            set { _trs_machinecondition = true; _trs_machinecondition_value = new OptionSetValue(value); }
        }

        private bool _trs_typeofsoil = false;
        private OptionSetValue _trs_typeofsoil_value;
        public int trs_typeofsoil
        {
            get { return _trs_typeofsoil ? _trs_typeofsoil_value.Value : int.MinValue; }
            set { _trs_typeofsoil = true; _trs_typeofsoil_value = new OptionSetValue(value); }
        }

        private bool _trs_application = false;
        private OptionSetValue _trs_application_value;
        public int trs_application
        {
            get { return _trs_application ? _trs_application_value.Value : int.MinValue; }
            set { _trs_application = true; _trs_application_value = new OptionSetValue(value); }
        }

        private bool _trs_analysis = false;
        private string _trs_analysis_value;
        public string trs_analysis
        {
            get { return _trs_analysis ? _trs_analysis_value : string.Empty; }
            set { _trs_analysis = true; _trs_analysis_value = value; }
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

        private bool _trs_type = false;
        private OptionSetValue _trs_type_value;
        public int trs_type
        {
            get { return _trs_type ? _trs_type_value.Value : int.MinValue; }
            set { _trs_type = true; _trs_type_value = new OptionSetValue(value); }
        }

        private bool _trs_mobileguid = false;
        private string _trs_mobileguid_value;
        public string trs_mobileguid
        {
            get { return _trs_mobileguid ? _trs_mobileguid_value : string.Empty; }
            set { _trs_mobileguid = true; _trs_mobileguid_value = value; }
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
                if (_trs_reportnumber) { entity.Attributes["trs_reportnumber"] = _trs_reportnumber_value; }
                if (_trs_workorder) { entity.Attributes["trs_workorder"] = _trs_workorder_value; }
                if (_trs_producttype) { entity.Attributes["trs_producttype"] = _trs_producttype_value; }
                if (_trs_equipment) { entity.Attributes["trs_equipment"] = _trs_equipment_value; }
                if (_trs_comments) { entity.Attributes["trs_comments"] = _trs_comments_value; }
                if (_trs_machinecondition) { entity.Attributes["trs_machinecondition"] = _trs_machinecondition_value; }
                if (_trs_typeofwork) { entity.Attributes["trs_typeofwork"] = _trs_typeofwork_value; }
                if (_trs_typeofsoil) { entity.Attributes["trs_typeofsoil"] = _trs_typeofsoil_value; }
                if (_trs_application) { entity.Attributes["trs_application"] = _trs_application_value; }
                if (_trs_analysis) { entity.Attributes["trs_analysis"] = _trs_analysis_value; }
                if (_trs_repairdate) { entity.Attributes["trs_repairdate"] = _trs_repairdate_value; }
                if (_trs_troubledate) { entity.Attributes["trs_troubledate"] = _trs_troubledate_value; }
                if (_trs_synctime) { entity.Attributes["trs_synctime"] = _trs_synctime_value; }
                if (_trs_frommobile) { entity.Attributes["trs_frommobile"] = _trs_frommobile_value; }
                if (_trs_workshop) { entity.Attributes["trs_workshop"] = _trs_workshop_value; }
                if (_trs_type) { entity.Attributes["trs_type"] = _trs_type_value; }
                if (_trs_mobileguid) { entity.Attributes["trs_mobileguid"] = _trs_mobileguid_value; }
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
                if (_trs_reportnumber) { entity.Attributes["trs_reportnumber"] = _trs_reportnumber_value; }
                if (_trs_workorder) { entity.Attributes["trs_workorder"] = _trs_workorder_value; }
                if (_trs_producttype) { entity.Attributes["trs_producttype"] = _trs_producttype_value; }
                if (_trs_equipment) { entity.Attributes["trs_equipment"] = _trs_equipment_value; }
                if (_trs_comments) { entity.Attributes["trs_comments"] = _trs_comments_value; }
                if (_trs_machinecondition) { entity.Attributes["trs_machinecondition"] = _trs_machinecondition_value; }
                if (_trs_typeofwork) { entity.Attributes["trs_typeofwork"] = _trs_typeofwork_value; }
                if (_trs_typeofsoil) { entity.Attributes["trs_typeofsoil"] = _trs_typeofsoil_value; }
                if (_trs_application) { entity.Attributes["trs_application"] = _trs_application_value; }
                if (_trs_analysis) { entity.Attributes["trs_analysis"] = _trs_analysis_value; }
                if (_trs_repairdate) { entity.Attributes["trs_repairdate"] = _trs_repairdate_value; }
                if (_trs_troubledate) { entity.Attributes["trs_troubledate"] = _trs_troubledate_value; }
                if (_trs_synctime) { entity.Attributes["trs_synctime"] = _trs_synctime_value; }
                if (_trs_frommobile) { entity.Attributes["trs_frommobile"] = _trs_frommobile_value; }
                if (_trs_workshop) { entity.Attributes["trs_workshop"] = _trs_workshop_value; }
                if (_trs_type) { entity.Attributes["trs_type"] = _trs_type_value; }
                if (_trs_mobileguid) { entity.Attributes["trs_mobileguid"] = _trs_mobileguid_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
