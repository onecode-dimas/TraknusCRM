using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_sectionrecommendation
    {
        #region Dependencies
        private DL_trs_ppmreport _DL_trs_ppmreport = new DL_trs_ppmreport();
        private DL_serviceappointment _DL_serviceappointment = new DL_serviceappointment();
        private DL_trs_productsection _DL_trs_productsection = new DL_trs_productsection();
        private DL_trs_tasklistgroup _DL_trs_tasklistgroup = new DL_trs_tasklistgroup();
        #endregion

        #region Properties
        private string _classname = "DL_trs_sectionrecommendation";

        private string _entityname = "trs_sectionrecommendation";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "PPM Report (Section Recommendation)";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _trs_reportnumber = false;
        private EntityReference _trs_reportnumber_value;
        public Guid trs_reportnumber
        {
            get { return _trs_reportnumber ? _trs_reportnumber_value.Id : Guid.Empty; }
            set { _trs_reportnumber = true; _trs_reportnumber_value = new EntityReference(_DL_trs_ppmreport.EntityName, value); }
        }

        private bool _trs_name = false;
        private string _trs_name_value;
        public string trs_name
        {
            get { return _trs_name ? _trs_name_value : string.Empty; }
            set { _trs_name = true; _trs_name_value = value; }
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

        private bool _trs_wonumber = false;
        private EntityReference _trs_wonumber_value;
        public Guid trs_wonumber
        {
            get { return _trs_wonumber ? _trs_wonumber_value.Id : Guid.Empty; }
            set { _trs_wonumber = true; _trs_wonumber_value = new EntityReference(_DL_serviceappointment.EntityName, value); }
        }

        private bool _trs_mobileguid = false;
        private string _trs_mobileguid_value;
        public string trs_mobileguid
        {
            get { return _trs_mobileguid ? _trs_mobileguid_value : string.Empty; }
            set { _trs_mobileguid = true; _trs_mobileguid_value = value; }
        }

        private bool _trs_section = false;
        private EntityReference _trs_section_value;
        public Guid trs_section
        {
            get { return _trs_section ? _trs_section_value.Id : Guid.Empty; }
            set { _trs_section = true; _trs_section_value = new EntityReference(_DL_trs_productsection.EntityName, value); }
        }

        private bool _trs_recommendation = false;
        private EntityReference _trs_recommendation_value;
        public Guid trs_recommendation
        {
            get { return _trs_recommendation ? _trs_recommendation_value.Id : Guid.Empty; }
            set { _trs_recommendation = true; _trs_recommendation_value = new EntityReference(_DL_trs_tasklistgroup.EntityName, value); }
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
                if (_trs_name) { entity.Attributes["trs_name"] = _trs_name_value; }
                if (_trs_synctime) { entity.Attributes["trs_synctime"] = _trs_synctime_value; }
                if (_trs_frommobile) { entity.Attributes["trs_frommobile"] = _trs_frommobile_value; }
                if (_trs_workshop) { entity.Attributes["trs_workshop"] = _trs_workshop_value; }
                if (_trs_wonumber) { entity.Attributes["trs_wonumber"] = _trs_wonumber_value; }
                if (_trs_mobileguid) { entity.Attributes["trs_mobileguid"] = _trs_mobileguid_value; }
                if (_trs_section) { entity.Attributes["trs_section"] = _trs_section_value; }
                if (_trs_recommendation) { entity.Attributes["trs_recommendation"] = _trs_recommendation_value; }
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
                if (_trs_name) { entity.Attributes["trs_name"] = _trs_name_value; }
                if (_trs_synctime) { entity.Attributes["trs_synctime"] = _trs_synctime_value; }
                if (_trs_frommobile) { entity.Attributes["trs_frommobile"] = _trs_frommobile_value; }
                if (_trs_workshop) { entity.Attributes["trs_workshop"] = _trs_workshop_value; }
                if (_trs_wonumber) { entity.Attributes["trs_wonumber"] = _trs_wonumber_value; }
                if (_trs_mobileguid) { entity.Attributes["trs_mobileguid"] = _trs_mobileguid_value; }
                if (_trs_section) { entity.Attributes["trs_section"] = _trs_section_value; }
                if (_trs_recommendation) { entity.Attributes["trs_recommendation"] = _trs_recommendation_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
