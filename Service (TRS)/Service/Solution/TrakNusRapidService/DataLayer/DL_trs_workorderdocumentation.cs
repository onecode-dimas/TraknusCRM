using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_workorderdocumentation
    {
        #region Dependencies
        private DL_serviceappointment _DL_serviceappointment = new DL_serviceappointment();
        #endregion

        #region Properties
        private string _classname = "DL_trs_workorderdocumentation";

        private string _entityname = "trs_workorderdocumentation";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Work Order Documentation";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _trs_workorderid = false;
        private EntityReference _trs_workorderid_value;
        public Guid trs_workorderid
        {
            get { return _trs_workorderid ? _trs_workorderid_value.Id : Guid.Empty; }
            set { _trs_workorderid = true; _trs_workorderid_value = new EntityReference(_DL_serviceappointment.EntityName, value); }
        }

        private bool _trs_mobileguid = false;
        private string _trs_mobileguid_value;
        public string trs_mobileguid
        {
            get { return _trs_mobileguid ? _trs_mobileguid_value : string.Empty; }
            set { _trs_mobileguid = true; _trs_mobileguid_value = value; }
        }

        private bool _trs_url = false;
        private string _trs_url_value;
        public Uri trs_url
        {
            get { return _trs_url ? new Uri(_trs_url_value) : null; }
            set { _trs_url = true; _trs_url_value = value.AbsoluteUri; }
        }

        private bool _trs_description = false;
        private string _trs_description_value;
        public string trs_description
        {
            get { return _trs_description ? _trs_description_value : string.Empty; }
            set { _trs_description = true; _trs_description_value = value; }
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
                EntityCollection entityCollection = organizationService.RetrieveMultiple(queryExpression);
                return entityCollection;

            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Select  : " + ex.Message.ToString());
            }
        }

        public Guid Insert(IOrganizationService organizationService)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                if (_trs_workorderid) { entity.Attributes["trs_workorderid"] = _trs_workorderid_value; };
                if (_trs_url) { entity.Attributes["trs_url"] = _trs_url_value; };
                if (_trs_description) { entity.Attributes["trs_description"] = _trs_description_value; };
                if (_trs_synctime) { entity.Attributes["trs_synctime"] = _trs_synctime_value; }
                if (_trs_frommobile) { entity.Attributes["trs_frommobile"] = _trs_frommobile_value; }
                if (_trs_workshop) { entity.Attributes["trs_workshop"] = _trs_workshop_value; }
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
                if (_trs_workorderid) { entity.Attributes["trs_workorderid"] = _trs_workorderid_value; };
                if (_trs_url) { entity.Attributes["trs_url"] = _trs_url_value; };
                if (_trs_description) { entity.Attributes["trs_description"] = _trs_description_value; };
                if (_trs_synctime) { entity.Attributes["trs_synctime"] = _trs_synctime_value; }
                if (_trs_frommobile) { entity.Attributes["trs_frommobile"] = _trs_frommobile_value; }
                if (_trs_workshop) { entity.Attributes["trs_workshop"] = _trs_workshop_value; }
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
