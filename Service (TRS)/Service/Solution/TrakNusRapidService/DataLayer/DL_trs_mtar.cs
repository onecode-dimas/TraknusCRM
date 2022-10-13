using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_mtar
    {
        #region Dependencies
        private DL_equipment _DL_equipment = new DL_equipment();
        private DL_serviceappointment _DL_serviceappointment = new DL_serviceappointment();
        #endregion

        #region Properties
        private string _classname = "DL_trs_mtar";

        private string _entityname = "trs_mtar";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "MTAR";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _trs_name = false;
        private string _trs_name_value;
        public string trs_name
        {
            get { return _trs_name ? _trs_name_value : null; }
            set { _trs_name = true; _trs_name_value = value; }
        }

        private bool _trs_mechanic = false;
        private EntityReference _trs_mechanic_value;
        public Guid trs_mechanic
        {
            get { return _trs_mechanic ? _trs_mechanic_value.Id : Guid.Empty; }
            set { _trs_mechanic = true; _trs_mechanic_value = new EntityReference(_DL_equipment.EntityName, value); }
        }

        private bool _trs_workorder = false;
        private EntityReference _trs_workorder_value;
        public Guid trs_workorder
        {
            get { return _trs_workorder ? _trs_workorder_value.Id : Guid.Empty; }
            set { _trs_workorder = true; _trs_workorder_value = new EntityReference(_DL_serviceappointment.EntityName, value); }
        }

        private bool _trs_mtarstatus = false;
        private OptionSetValue _trs_mtarstatus_value;
        public int trs_mtarstatus
        {
            get { return _trs_mtarstatus ? _trs_mtarstatus_value.Value : int.MinValue; }
            set { _trs_mtarstatus = true; _trs_mtarstatus_value = new OptionSetValue(value); }
        }

        private bool _trs_statusremarks = false;
        private string _trs_statusremarks_value;
        public string trs_statusremarks
        {
            get { return _trs_statusremarks ? _trs_statusremarks_value : string.Empty; }
            set { _trs_statusremarks = true; _trs_statusremarks_value = value; }
        }

        private bool _trs_longitude = false;
        private decimal _trs_longitude_value;
        public decimal trs_longitude
        {
            get { return _trs_longitude_value; }
            set { _trs_longitude = true; _trs_longitude_value = value; }
        }


        private bool _trs_latitude = false;
        private decimal _trs_latitude_value;
        public decimal trs_latitude
        {
            get { return _trs_latitude ? _trs_latitude_value : decimal.MinValue; }
            set { _trs_latitude = true; _trs_latitude_value = value; }
        }

        private bool _trs_automatictime = false;
        private DateTime _trs_automatictime_value;
        public DateTime trs_automatictime
        {
            get { return _trs_automatictime ? _trs_automatictime_value.ToLocalTime() : DateTime.MinValue; }
            set { _trs_automatictime = true; _trs_automatictime_value = value.ToLocalTime(); }
        }

        private bool _trs_manualtime = false;
        private DateTime _trs_manualtime_value;
        public DateTime trs_manualtime
        {
            get { return _trs_manualtime ? _trs_manualtime_value.ToLocalTime() : DateTime.MinValue; }
            set { _trs_manualtime = true; _trs_manualtime_value = value.ToLocalTime(); }
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

        private bool _trs_updatewostatus = false;
        private bool _trs_updatewostatus_value;
        public bool trs_updatewostatus
        {
            get { return _trs_updatewostatus ? _trs_updatewostatus_value : false; }
            set { _trs_updatewostatus = true; _trs_updatewostatus_value = value; }
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
                if (_trs_name)
                {
                    Entity entity = new Entity(_entityname);
                    if (_trs_name) { entity.Attributes["trs_name"] = _trs_name_value; }
                    if (_trs_workorder) { entity.Attributes["trs_workorder"] = _trs_workorder_value; }
                    if (_trs_mechanic) { entity.Attributes["trs_mechanic"] = _trs_mechanic_value; }
                    if (_trs_mtarstatus) { entity.Attributes["trs_mtarstatus"] = _trs_mtarstatus_value; }
                    if (_trs_statusremarks) { entity.Attributes["trs_statusremarks"] = _trs_statusremarks_value; }
                    if (_trs_longitude) { entity.Attributes["trs_longitude"] = _trs_longitude_value; }
                    if (_trs_latitude) { entity.Attributes["trs_latitude"] = _trs_latitude_value; }
                    if (_trs_automatictime) { entity.Attributes["trs_automatictime"] = _trs_automatictime_value; }
                    if (_trs_manualtime) { entity.Attributes["trs_manualtime"] = _trs_manualtime_value; }
                    if (_trs_synctime) { entity.Attributes["trs_synctime"] = _trs_synctime_value; }
                    if (_trs_frommobile) { entity.Attributes["trs_frommobile"] = _trs_frommobile_value; }
                    if (_trs_workshop) { entity.Attributes["trs_workshop"] = _trs_workshop_value; }
                    if (_trs_updatewostatus) { entity.Attributes["trs_updatewostatus"] = _trs_updatewostatus_value; }
                    if (_trs_mobileguid) { entity.Attributes["trs_mobileguid"] = _trs_mobileguid_value; }
                    return organizationService.Create(entity);
                }
                else
                {
                    throw new Exception("Primary Key is empty.");
                }
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
                if (_trs_name) { entity.Attributes["trs_name"] = _trs_name_value; }
                if (_trs_workorder) { entity.Attributes["trs_workorder"] = _trs_workorder_value; }
                if (_trs_mechanic) { entity.Attributes["trs_mechanic"] = _trs_mechanic_value; }
                if (_trs_mtarstatus) { entity.Attributes["trs_mtarstatus"] = _trs_mtarstatus_value; }
                if (_trs_statusremarks) { entity.Attributes["trs_statusremarks"] = _trs_statusremarks_value; }
                if (_trs_longitude) { entity.Attributes["trs_longitude"] = _trs_longitude_value; }
                if (_trs_latitude) { entity.Attributes["trs_latitude"] = _trs_latitude_value; }
                if (_trs_automatictime) { entity.Attributes["trs_automatictime"] = _trs_automatictime_value; }
                if (_trs_manualtime) { entity.Attributes["trs_manualtime"] = _trs_manualtime_value; }
                if (_trs_synctime) { entity.Attributes["trs_synctime"] = _trs_synctime_value; }
                if (_trs_frommobile) { entity.Attributes["trs_frommobile"] = _trs_frommobile_value; }
                if (_trs_updatewostatus) { entity.Attributes["trs_updatewostatus"] = _trs_updatewostatus_value; }
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
