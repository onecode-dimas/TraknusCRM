using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_new_population
    {
        #region Dependencies
        private DL_trs_functionallocation _DL_trs_functionallocation = new DL_trs_functionallocation();
        #endregion

        #region Properties
        private string _classname = "DL_new_population";

        private string _entityname = "new_population";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Population";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _trs_serialnumber = false;
        private string _trs_serialnumber_value;
        public string trs_serialnumber
        {
            get { return _trs_serialnumber ? _trs_serialnumber_value : null; }
            set { _trs_serialnumber = true; _trs_serialnumber_value = value; }
        }

        private bool _trs_bastsigndate = false;
        private DateTime _trs_bastsigndate_value;
        public DateTime trs_bastsigndate
        {
            get { return _trs_bastsigndate ? _trs_bastsigndate_value.ToLocalTime() : DateTime.MinValue; }
            set { _trs_bastsigndate = true; _trs_bastsigndate_value = value.ToLocalTime(); }
        }

        private bool _trs_warrantystartdate = false;
        private DateTime _trs_warrantystartdate_value;
        public DateTime trs_warrantystartdate
        {
            get { return _trs_warrantystartdate ? _trs_warrantystartdate_value.ToLocalTime() : DateTime.MinValue; }
            set { _trs_warrantystartdate = true; _trs_warrantystartdate_value = value.ToLocalTime(); }
        }

        private bool _trs_warrantyenddate = false;
        private DateTime _trs_warrantyenddate_value;
        public DateTime trs_warrantyenddate
        {
            get { return _trs_warrantyenddate ? _trs_warrantyenddate_value.ToLocalTime() : DateTime.MinValue; }
            set { _trs_warrantyenddate = true; _trs_warrantyenddate_value = value.ToLocalTime(); }
        }

        private bool _trs_functionallocation = false;
        private EntityReference _trs_functionallocation_value;
        public Guid trs_functionallocation
        {
            get { return _trs_functionallocation ? _trs_functionallocation_value.Id : Guid.Empty; }
            set { _trs_functionallocation = true; _trs_functionallocation_value = new EntityReference(_DL_trs_functionallocation.EntityName, value); }
        }

        private bool _trs_synctime = false;
        private DateTime _trs_synctime_value;
        public DateTime trs_synctime
        {
            get { return _trs_synctime ? _trs_synctime_value.ToLocalTime() : DateTime.MinValue; }
            set { _trs_synctime = true; _trs_synctime_value = value.ToLocalTime(); }
        }

        private bool _trs_unitstatus = false;
        private bool _trs_unitstatus_value;
        public bool trs_unitstatus
        {
            get { return _trs_unitstatus ? _trs_unitstatus_value : false; }
            set { _trs_unitstatus = true; _trs_unitstatus_value = value; }
        }

        private bool _trs_lock = false;
        private bool _trs_lock_value;
        public bool trs_lock
        {
            get { return _trs_lock ? _trs_lock_value : false; }
            set { _trs_lock = true; _trs_lock_value = value; }
        }

        private bool _trs_functionallongitude = false;
        private decimal _trs_functionallongitude_value;
        public decimal trs_functionallongitude
        {
            get { return _trs_functionallongitude ? _trs_functionallongitude_value : decimal.MinValue; }
            set { _trs_functionallongitude = true; _trs_functionallongitude_value = value; }
        }

        private bool _trs_functionallatitude = false;
        private decimal _trs_functionallatitude_value;
        public decimal trs_functionallatitude
        {
            get { return _trs_functionallatitude ? _trs_functionallatitude_value : decimal.MinValue; }
            set { _trs_functionallatitude = true; _trs_functionallatitude_value = value; }
        }

        private bool _trs_hourmeteronvisit = false;
        private decimal _trs_hourmeteronvisit_value;
        public decimal trs_hourmeteronvisit
        {
            get { return _trs_hourmeteronvisit ? _trs_hourmeteronvisit_value : decimal.MinValue; }
            set { _trs_hourmeteronvisit = true; _trs_hourmeteronvisit_value = value; }
        }

        private bool _trs_datevisit = false;
        private DateTime _trs_datevisit_value;
        public DateTime trs_datevisit
        {
            get { return _trs_datevisit ? _trs_datevisit_value.ToLocalTime() : DateTime.MinValue; }
            set { _trs_datevisit = true; _trs_datevisit_value = value.ToLocalTime(); }
        }

        private bool _new_statusinoperation = false;
        private OptionSetValue _new_statusinoperation_value;
        public int new_statusinoperation
        {
            get { return _new_statusinoperation ? _new_statusinoperation_value.Value : int.MinValue; }
            set { _new_statusinoperation = true; _new_statusinoperation_value = new OptionSetValue(value); }
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
                if (_trs_serialnumber)
                {
                    Entity entity = new Entity(_entityname);
                    entity.Attributes["trs_serialnumber"] = _trs_serialnumber_value;
                    if (_trs_functionallocation) { entity.Attributes["trs_functionallocation"] = _trs_functionallocation_value; }
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
                if (_trs_serialnumber) { entity.Attributes["trs_serialnumber"] = _trs_serialnumber_value; }
                if (_trs_bastsigndate) { entity.Attributes["trs_bastsigndate"] = _trs_bastsigndate_value; }
                if (_trs_warrantystartdate) { entity.Attributes["trs_warrantystartdate"] = _trs_warrantystartdate_value; }
                if (_trs_warrantyenddate) { entity.Attributes["trs_warrantyenddate"] = _trs_warrantyenddate_value; }
                if (_trs_functionallocation) { entity.Attributes["trs_functionallocation"] = _trs_functionallocation_value; }
                if (_trs_unitstatus) { entity.Attributes["trs_unitstatus"] = _trs_unitstatus_value; }
                if (_trs_lock) { entity.Attributes["trs_lock"] = _trs_lock_value; }
                if (_trs_synctime) { entity.Attributes["trs_synctime"] = _trs_synctime_value; }
                if (_trs_functionallatitude) { entity.Attributes["trs_functionallatitude"] = _trs_functionallatitude_value; }
                if (_trs_functionallongitude) { entity.Attributes["trs_functionallongitude"] = _trs_functionallongitude_value; }
                if (_trs_hourmeteronvisit) { entity.Attributes["trs_hourmeteronvisit"] = _trs_hourmeteronvisit_value; }
                if (_trs_datevisit) { entity.Attributes["trs_datevisit"] = _trs_datevisit_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
