using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_contractrealization
    {
        #region Dependencies
        private DL_incident _DL_incident = new DL_incident();
        private DL_serviceappointment _DL_serviceappointment = new DL_serviceappointment();
        #endregion

        #region Properties
        private string _classname = "DL_trs_contractrealization";

        private string _entityname = "trs_contractrealization";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Contract (Realization)";
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

        private bool _trs_failedreason = false;
        private string _trs_failedreason_value;
        public string trs_failedreason
        {
            get { return _trs_failedreason ? _trs_failedreason_value : null; }
            set { _trs_failedreason = true; _trs_failedreason_value = value; }
        }

        private bool _trs_incidentid = false;
        private EntityReference _trs_incidentid_value;
        public Guid trs_incidentid
        {
            get { return _trs_incidentid ? _trs_incidentid_value.Id : Guid.Empty; }
            set { _trs_incidentid = true; _trs_incidentid_value = new EntityReference(_DL_incident.EntityName, value); }
        }

        private bool _trs_workorderid = false;
        private EntityReference _trs_workorderid_value;
        public Guid trs_workorderid
        {
            get { return _trs_workorderid ? _trs_workorderid_value.Id : Guid.Empty; }
            set { _trs_workorderid = true; _trs_workorderid_value = new EntityReference(_DL_serviceappointment.EntityName, value); }
        }

        private bool _trs_success = false;
        private bool _trs_success_value;
        public bool trs_success
        {
            get { return _trs_success ? _trs_success_value : false; }
            set { _trs_success = true; _trs_success_value = value; }
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

        public Guid Insert(IOrganizationService organizationService)//, out Guid id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                if (_trs_name) { entity.Attributes["trs_name"] = _trs_name_value; }
                if (_trs_failedreason) { entity.Attributes["trs_failedreason"] = _trs_failedreason_value; }
                if (_trs_incidentid) { entity.Attributes["trs_incidentid"] = _trs_incidentid_value; }
                if (_trs_workorderid) { entity.Attributes["trs_workorderid"] = _trs_workorderid_value; }
                if (_trs_success) { entity.Attributes["trs_success"] = _trs_success_value; }
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
                if (_trs_failedreason) { entity.Attributes["trs_failedreason"] = _trs_failedreason_value; }
                if (_trs_incidentid) { entity.Attributes["trs_incidentid"] = _trs_incidentid_value; }
                if (_trs_workorderid) { entity.Attributes["trs_workorderid"] = _trs_workorderid_value; }
                if (_trs_success) { entity.Attributes["trs_success"] = _trs_success_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
