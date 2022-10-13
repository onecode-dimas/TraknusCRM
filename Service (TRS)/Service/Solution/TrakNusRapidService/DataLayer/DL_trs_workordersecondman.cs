using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_workordersecondman
    {
        #region Dependencies
        private DL_serviceappointment _DL_serviceappointment = new DL_serviceappointment();
        private DL_equipment _DL_equipment = new DL_equipment();
        #endregion

        #region Properties
        private string _classname = "DL_trs_workordersecondman";

        private string _entityname = "trs_workordersecondman";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Work Order - Secondman";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _trs_nrp = false;
        private string _trs_nrp_value;
        public string trs_nrp
        {
            get { return _trs_nrp ? _trs_nrp_value : null; }
            set { _trs_nrp = true; _trs_nrp_value = value; }
        }

        private bool _trs_activityid = false;
        private EntityReference _trs_activityid_value;
        public Guid trs_activityid
        {
            get { return _trs_activityid ? _trs_activityid_value.Id : Guid.Empty; }
            set { _trs_activityid = true; _trs_activityid_value = new EntityReference(_DL_serviceappointment.EntityName, value); }
        }
        #endregion

        private bool _trs_equipmentid = false;
        private EntityReference _trs_equipmentid_value;
        public Guid trs_equipmentid
        {
            get { return _trs_equipmentid ? _trs_equipmentid_value.Id : Guid.Empty; }
            set { _trs_equipmentid = true; _trs_equipmentid_value = new EntityReference(_DL_equipment.EntityName, value); }
        }

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

        public void Insert(IOrganizationService organizationService)
        {
            try
            {
                if (_trs_nrp)
                {
                    Entity entity = new Entity(_entityname);
                    entity.Attributes["trs_nrp"] = _trs_nrp_value;
                    if (_trs_activityid) { entity.Attributes["trs_activityid"] = _trs_activityid_value; }
                    if (_trs_equipmentid) { entity.Attributes["trs_equipmentid"] = _trs_equipmentid_value; }
                    organizationService.Create(entity);
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
                if (_trs_nrp) { entity.Attributes["trs_nrp"] = _trs_nrp_value; }
                if (_trs_activityid) { entity.Attributes["trs_activityid"] = _trs_activityid_value; }
                if (_trs_equipmentid) { entity.Attributes["trs_equipmentid"] = _trs_equipmentid_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }

        public void Deactivate(IOrganizationService organizationService, Guid id)
        {
            try
            {
                EntityReference entityReference = new EntityReference(_entityname, id);
                OrganizationRequest organizationRequest = new OrganizationRequest();
                organizationRequest.RequestName = "SetState";
                organizationRequest["EntityMoniker"] = entityReference;
                organizationRequest["State"] = new OptionSetValue(1);
                organizationRequest["Status"] = new OptionSetValue(2);
                organizationService.Execute(organizationRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Deactivate : " + ex.Message);
            }
        }
    }
}
