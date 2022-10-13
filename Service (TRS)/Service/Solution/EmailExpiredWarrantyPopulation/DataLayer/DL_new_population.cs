using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace SendEmail.DataLayer
{
    public class DL_new_population
    {
        #region Dependencies
        //private DL_trs_functionallocation _DL_trs_functionallocation = new DL_trs_functionallocation();
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

        private bool _trs_functionallocation = false;
        private EntityReference _trs_functionallocation_value;
        //public Guid trs_functionallocation
        //{
        //    get { return _trs_functionallocation ? _trs_functionallocation_value.Id : Guid.Empty; }
        //    set { _trs_functionallocation = true; _trs_functionallocation_value = new EntityReference(_DL_trs_functionallocation.EntityName, value); }
        //}

        private bool _trs_synctime = false;
        private DateTime _trs_synctime_value;
        public DateTime trs_synctime
        {
            get { return _trs_synctime ? _trs_synctime_value.ToLocalTime() : DateTime.MinValue; }
            set { _trs_synctime = true; _trs_synctime_value = value.ToLocalTime(); }
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
                    throw new Exception(_classname + ".Insert : Primary Key is empty.");
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
                if (_trs_functionallocation) { entity.Attributes["trs_functionallocation"] = _trs_functionallocation_value; }
                if (_trs_synctime) { entity.Attributes["trs_synctime"] = _trs_synctime_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
