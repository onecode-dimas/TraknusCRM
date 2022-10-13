using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusSparepartSystem.DataLayer
{
    class DL_activityparty
    {
        #region Dependencies
        #endregion

        #region Properties
        private string _classname = "DL_activityparty";

        private string _entityname = "activityparty";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Activity Party";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _activityid = false;
        private EntityReference _activityid_value;
        public EntityReference activityid
        {
            get { return _activityid ? _activityid_value : null; }
            set { _activityid = true; _activityid_value = value; }
        }

        private bool _partyid = false;
        private EntityReference _partyid_value;
        public EntityReference partyid
        {
            get { return _partyid ? _partyid_value : null; }
            set { _partyid = true; _partyid_value = value; }
        }

        private bool _partyobjecttypecode = false;
        private OptionSetValue _partyobjecttypecode_value;
        public int partyobjecttypecode
        {
            get { return _partyobjecttypecode ? _partyobjecttypecode_value.Value : int.MinValue; }
            set { _partyobjecttypecode = true; _partyobjecttypecode_value = new OptionSetValue(value); }
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

        public void Insert(IOrganizationService organizationService)
        {
            try
            {
                if (_activityid)
                {
                    Entity entity = new Entity(_entityname);
                    entity.Attributes["activityid"] = _activityid_value;

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
                if (_activityid) { entity.Attributes["activityid"] = _activityid_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }

        public Entity GetEntity()
        {
            Entity entity = new Entity(_entityname);
            if (_activityid) { entity.Attributes["activityid"] = _activityid_value; }
            if (_partyid) { entity.Attributes["partyid"] = _partyid_value; }
            if (_partyobjecttypecode) { entity.Attributes["partyobjecttypecode"] = _partyobjecttypecode_value; }
            return entity;
        }
    }
}