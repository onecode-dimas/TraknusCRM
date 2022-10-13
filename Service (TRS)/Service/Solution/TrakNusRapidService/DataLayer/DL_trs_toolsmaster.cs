using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_toolsmaster
    {
        #region Dependencies
        private DL_serviceappointment _DL_serviceappointment = new DL_serviceappointment();
        #endregion
        #region Properties
        private string _classname = "DL_trs_toolsmaster";

        private string _entityname = "trs_toolsmaster";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Tool Master";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private string _trs_toolsid;
        public string trs_toolsid
        {
            get { return _trs_toolsid; }
            set { _trs_toolsid = value; }
        }

        private string _trs_toolsname;
        public string trs_toolsname
        {
            get { return _trs_toolsname; }
            set { _trs_toolsname = value; }
        }
        
        private bool _trs_branch = false;
        private EntityReference _trs_branch_value;
        public EntityReference trs_branch
        {
            get { return _trs_branch ? _trs_branch_value : null; }
            set { _trs_branch = true; _trs_branch_value = value; }
        }

        private bool _trs_workorderid = false;
        private EntityReference _trs_workorderid_value;
        public Guid trs_workorderid
        {
            get { return _trs_workorderid ? _trs_workorderid_value.Id : Guid.Empty; }
            set { _trs_workorderid = true; _trs_workorderid_value = new EntityReference(_DL_serviceappointment.EntityName, value); }
        }

        private int _trs_calibrationcycle;
        public int trs_calibrationcycle
        {
            get { return _trs_calibrationcycle; }
            set { _trs_calibrationcycle = value; }
        }

        private DateTime _trs_calibrationdate;
        public DateTime trs_calibrationdate
        {
            get { return _trs_calibrationdate.ToLocalTime(); }
            set { _trs_calibrationdate = value.ToLocalTime(); }
        }

        private bool _trs_status;
        public bool trs_status
        {
            get { return _trs_status; }
            set { _trs_status = value; }
        }

        private int _trs_unitofmeasurements;
        public int trs_unitofmeasurements
        {
            get { return _trs_unitofmeasurements; }
            set { _trs_unitofmeasurements = value; }
        }

        private string _trs_toolsdescription;
        public string trs_toolsdescription
        {
            get { return _trs_toolsdescription; }
            set { _trs_toolsdescription = value; }
        }
        #endregion

        public Entity Select(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = organizationService.Retrieve(_entityname, id, new ColumnSet(new string[] { "trs_toolsid", "trs_toolsname", "trs_branch", "trs_calibrationcycle", "trs_calibrationdate", "trs_status", "trs_toolsdescription" }));
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception("Error DL_trs_toolsmaster.Select : " + ex.Message, ex.InnerException);
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
                throw new Exception("Error DL_trs_toolsmaster.Select : " + ex.Message, ex.InnerException);
            }
        }

        public void Update(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                entity.Id = id;
                entity.Attributes["trs_status"] = _trs_status;
                if (_trs_branch) { entity.Attributes["trs_branch"] = _trs_branch_value; }
                if (_trs_workorderid) { entity.Attributes["trs_workorderid"] = _trs_workorderid_value; }                 
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
