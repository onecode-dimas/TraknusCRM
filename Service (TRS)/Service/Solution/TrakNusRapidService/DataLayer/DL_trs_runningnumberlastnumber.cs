using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_runningnumberlastnumber
    {
        #region Dependencies
        private DL_trs_runningnumber _DL_trs_runningnumber = new DL_trs_runningnumber();
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        #endregion

        #region Properties
        private string _classname = "DL_trs_runningnumberlastnumber";

        private string _entityname = "trs_runningnumberlastnumber";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Running Number - Last Number";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _trs_year = false;
        private string _trs_year_value;
        public string trs_year
        {
            get { return _trs_year ? _trs_year_value : null; }
            set { _trs_year = true; _trs_year_value = value; }
        }

        private bool _trs_runningnumberid = false;
        private EntityReference _trs_runningnumberid_value;
        public Guid trs_runningnumberid
        {
            get { return _trs_runningnumberid ? _trs_runningnumberid_value.Id : Guid.Empty; }
            set { _trs_runningnumberid = true; _trs_runningnumberid_value = new EntityReference (_DL_trs_runningnumber.EntityName, value); }
        }

        private bool _trs_lastnumber = false;
        private decimal _trs_lastnumber_value;
        public decimal trs_lastnumber
        {
            get { return _trs_lastnumber ? _trs_lastnumber_value : decimal.MinValue; }
            set { _trs_lastnumber = true; _trs_lastnumber_value = value; }
        }

        public bool _trs_lockingby = false;
        private EntityReference _trs_lockingby_value;
        public Guid trs_lockingby
        {
            get { return _trs_lockingby ? _trs_lockingby_value.Id : Guid.Empty; }
            set { _trs_lockingby = true; _trs_lockingby_value = new EntityReference(_DL_systemuser.EntityName, value); }
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
                if (_trs_year && _trs_runningnumberid)
                {
                    Entity entity = new Entity(_entityname);
                    entity.Attributes["trs_year"] = _trs_year_value;
                    entity.Attributes["trs_runningnumberid"] = _trs_runningnumberid_value;
                    if (_trs_lastnumber) { entity.Attributes["trs_lastnumber"] = _trs_lastnumber_value; }
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
                if (_trs_year) { entity.Attributes["trs_year"] = _trs_year_value; }
                if (_trs_runningnumberid) { entity.Attributes["trs_runningnumberid"] = _trs_runningnumberid_value; }
                if (_trs_lastnumber) { entity.Attributes["trs_lastnumber"] = _trs_lastnumber_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
