using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_tsrpartdetails
    {
        #region Dependencies
        private DL_trs_technicalservicereport _DL_trs_technicalservicereport = new DL_trs_technicalservicereport();
        private DL_trs_masterpart _DL_trs_masterpart = new DL_trs_masterpart();
        private DL_serviceappointment _DL_serviceappointment = new DL_serviceappointment();        
        #endregion

        #region Properties
        private string _classname = "DL_trs_tsrpartdetails";

        private string _entityname = "trs_tsrpartdetails";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "TRS - Part Installed";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _trs_name = false;
        private string _trs_name_value;
        public string trs_name
        {
            get { return _trs_name ? _trs_name_value : string.Empty; }
            set { _trs_name = true; _trs_name_value = value; }
        }

        private bool _trs_tsrnumber = false;
        private EntityReference _trs_tsrnumber_value;
        public Guid trs_tsrnumber
        {
            get { return _trs_tsrnumber ? _trs_tsrnumber_value.Id : Guid.Empty; }
            set { _trs_tsrnumber = true; _trs_tsrnumber_value = new EntityReference(_DL_trs_technicalservicereport.EntityName, value); }
        }

        private bool _trs_partnumber = false;
        private EntityReference _trs_partnumber_value;
        public Guid trs_partnumber
        {
            get { return _trs_partnumber ? _trs_partnumber_value.Id : Guid.Empty; }
            set { _trs_partnumber = true; _trs_partnumber_value = new EntityReference(_DL_trs_masterpart.EntityName, value); }
        }

        private bool _trs_quantity = false;
        private int _trs_quantity_value;
        public int trs_quantity
        {
            get { return _trs_quantity ? _trs_quantity_value : int.MinValue; }
            set { _trs_quantity = true; _trs_quantity_value = value; }
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

        private bool _trs_partnumbercatalog = false;
        private string _trs_partnumbercatalog_value;
        public string trs_partnumbercatalog
        {
            get { return _trs_partnumbercatalog ? _trs_partnumbercatalog_value : string.Empty; }
            set { _trs_partnumbercatalog = true; _trs_partnumbercatalog_value = value; }
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
                if (_trs_name) { entity.Attributes["trs_name"] = _trs_name_value; }
                if (_trs_tsrnumber) { entity.Attributes["trs_tsrnumber"] = _trs_tsrnumber_value; }
                if (_trs_partnumber) { entity.Attributes["trs_partnumber"] = _trs_partnumber_value; }
                if (_trs_quantity) { entity.Attributes["trs_quantity"] = _trs_quantity_value; }
                if (_trs_synctime) { entity.Attributes["trs_synctime"] = _trs_synctime_value; }
                if (_trs_frommobile) { entity.Attributes["trs_frommobile"] = _trs_frommobile_value; }
                if (_trs_workshop) { entity.Attributes["trs_workshop"] = _trs_workshop_value; }
                if (_trs_wonumber) { entity.Attributes["trs_wonumber"] = _trs_wonumber_value; }
                if (_trs_mobileguid) { entity.Attributes["trs_mobileguid"] = _trs_mobileguid_value; }
                if (_trs_partnumbercatalog) { entity.Attributes["trs_partnumbercatalog"] = _trs_partnumbercatalog_value; }
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
                if (_trs_name) { entity.Attributes["trs_name"] = _trs_name_value; }
                if (_trs_tsrnumber) { entity.Attributes["trs_tsrnumber"] = _trs_tsrnumber_value; }
                if (_trs_partnumber) { entity.Attributes["trs_partnumber"] = _trs_partnumber_value; }
                if (_trs_quantity) { entity.Attributes["trs_quantity"] = _trs_quantity_value; }
                if (_trs_synctime) { entity.Attributes["trs_synctime"] = _trs_synctime_value; }
                if (_trs_frommobile) { entity.Attributes["trs_frommobile"] = _trs_frommobile_value; }
                if (_trs_workshop) { entity.Attributes["trs_workshop"] = _trs_workshop_value; }
                if (_trs_wonumber) { entity.Attributes["trs_wonumber"] = _trs_wonumber_value; }
                if (_trs_mobileguid) { entity.Attributes["trs_mobileguid"] = _trs_mobileguid_value; }
                if (_trs_partnumbercatalog) { entity.Attributes["trs_partnumbercatalog"] = _trs_partnumbercatalog_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
