using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_equipment
    {
        #region Dependencies
        #endregion

        #region Properties
        private string _classname = "DL_equipment";

        private string _entityname = "equipment";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Mechanic";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _name = false;
        private string _name_value;
        public string name
        {
            get { return _name ? _name_value : null; }
            set { _name = true; _name_value = value; }
        }

        private bool _trs_longitude = false;
        private decimal _trs_longitude_value;
        public decimal trs_longitude
        {
            get { return _trs_longitude ? _trs_longitude_value : decimal.MinValue; }
            set { _trs_longitude = true; _trs_longitude_value = value; }
        }

        private bool _trs_latitude = false;
        private decimal _trs_latitude_value;
        public decimal trs_latitude
        {
            get { return _trs_latitude ? _trs_latitude_value : decimal.MinValue; }
            set { _trs_latitude = true; _trs_latitude_value = value; }
        }

        private bool _trs_repairtimehour = false;
        private decimal _trs_repairtimehour_value;
        public decimal trs_repairtimehour
        {
            get { return _trs_repairtimehour ? _trs_repairtimehour_value : decimal.MinValue; }
            set { _trs_repairtimehour = true; _trs_repairtimehour_value = value; }
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
                if (_name)
                {
                    Entity entity = new Entity(_entityname);
                    entity.Attributes["name"] = _name_value;
                    if (_trs_longitude) { entity.Attributes["trs_longitude"] = _trs_longitude_value; }
                    if (_trs_latitude) { entity.Attributes["trs_latitude"] = _trs_latitude_value; }
                    if (_trs_repairtimehour) { entity.Attributes["trs_repairtimehour"] = _trs_repairtimehour_value; }
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
                if (_name) { entity.Attributes["name"] = _name_value; }
                if (_trs_longitude) { entity.Attributes["trs_longitude"] = _trs_longitude_value; }
                if (_trs_latitude) { entity.Attributes["trs_latitude"] = _trs_latitude_value; }
                if (_trs_repairtimehour) { entity.Attributes["trs_repairtimehour"] = _trs_repairtimehour_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
