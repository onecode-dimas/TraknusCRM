using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_contract
    {
        #region Dependencies
        #endregion

        #region Properties
        private string _classname = "DL_contract";

        private string _entityname = "contract";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Contract";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _title = false;
        private string _title_value;
        public string title
        {
            get { return _title ? _title_value : null; }
            set { _title = true; _title_value = value; }
        }

        private bool _trs_maintenanceperiod = false;
        private bool _trs_maintenanceperiod_value;
        public bool trs_maintenanceperiod
        {
            get { return _trs_maintenanceperiod ? _trs_maintenanceperiod_value : false; }
            set { _trs_maintenanceperiod = true; _trs_maintenanceperiod_value = value; }
        }

        private bool _trs_otos = false;
        private int _trs_otos_value;
        public int trs_otos
        {
            get { return _trs_otos ? _trs_otos_value : int.MinValue; }
            set { _trs_otos = true; _trs_otos_value = value; }
        }

        private bool _trs_ots = false;
        private int _trs_ots_value;
        public int trs_ots
        {
            get { return _trs_ots ? _trs_ots_value : int.MinValue; }
            set { _trs_ots = true; _trs_ots_value = value; }
        }

        private bool _trs_servicelevel = false;
        private string _trs_servicelevel_value;
        public string trs_servicelevel
        {
            get { return _trs_servicelevel ? _trs_servicelevel_value : null; }
            set { _trs_servicelevel = true; _trs_servicelevel_value = value; }
        }

        private bool _trs_type = false;
        private string _trs_type_value;
        public string trs_type
        {
            get { return _trs_type ? _trs_type_value : null; }
            set { _trs_type = true; _trs_type_value = value; }
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
                if (_title)
                {
                    Entity entity = new Entity(_entityname);
                    entity.Attributes["title"] = _title_value;
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
                if (_title) { entity.Attributes["title"] = _title_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
