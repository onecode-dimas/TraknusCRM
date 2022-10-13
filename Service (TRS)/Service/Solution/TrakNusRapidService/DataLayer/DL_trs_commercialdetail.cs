using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_commercialdetail
    {
        #region Dependencies
        private DL_trs_mechanicgrade _DL_trs_mechanicgrade = new DL_trs_mechanicgrade();
        private DL_trs_commercialtask _DL_trs_commercialtask = new DL_trs_commercialtask();
        private DL_task _DL_task = new DL_task();
        private DL_trs_tasklist _DL_trs_tasklist = new DL_trs_tasklist();
        private DL_serviceappointment _DL_serviceappoinment = new DL_serviceappointment();
        private DL_trs_toolsgroup _DL_trs_toolsgroup = new DL_trs_toolsgroup();
        private DL_trs_trs_commercialdetail_trs_toolsgroup _DL_trs_trs_commercialdetail_trs_toolsgroup = new DL_trs_trs_commercialdetail_trs_toolsgroup();
        #endregion

        #region Properties
        private string _classname = "DL_trs_commercialdetail";

        private string _entityname = "trs_commercialdetail";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Commercial Detail";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _taskcode = false;
        private string _taskcode_value;
        public string TaskCode
        {
            get { return _taskcode ? _taskcode_value: string.Empty; }
            set { _taskcode = true; _taskcode_value = value; }
        }

 

        private bool _taskname = false;
        private EntityReference _taskname_value;
        public Guid Taskname
        {
            get { return _taskname ? _taskname_value.Id : Guid.Empty; }
            set { _taskname = true; _taskname_value = new EntityReference(_DL_trs_tasklist.EntityName, value); }
        }

        private bool _trs_commercialheaderid = false;
        private EntityReference _trs_commercialheaderid_value;
        public Guid CommercialHeaderId
        {
            get { return _trs_commercialheaderid ? _trs_commercialheaderid_value.Id : Guid.Empty; }
            set { _trs_commercialheaderid = true; _trs_commercialheaderid_value = new EntityReference(_DL_task.EntityName, value); }
        }

        private bool _trs_workorder = false;
        private EntityReference _trs_workorder_value;
        public Guid trs_workorder
        {
            get { return _trs_workorder ? _trs_workorder_value.Id : Guid.Empty; }
            set { _trs_workorder = true; _trs_workorder_value = new EntityReference(_DL_serviceappoinment.EntityName, value); }
        }

        private bool _trs_automatictime = false;
        private DateTime _trs_automatictime_value;
        public DateTime trs_automatictime
        {
            get { return _trs_automatictime ? _trs_automatictime_value.ToLocalTime() : DateTime.MinValue; }
            set { _trs_automatictime = true; _trs_automatictime_value = value.ToLocalTime(); }
        }

        private bool _trs_manualtime = false;
        private DateTime _trs_manualtime_value;
        public DateTime trs_manualtime
        {
            get { return _trs_manualtime ? _trs_manualtime_value.ToLocalTime() : DateTime.MinValue; }
            set { _trs_manualtime = true; _trs_manualtime_value = value.ToLocalTime(); }
        }

        private bool _trs_synctime = false;
        private DateTime _trs_synctime_value;
        public DateTime trs_synctime
        {
            get { return _trs_synctime ? _trs_synctime_value.ToLocalTime() : DateTime.MinValue; }
            set { _trs_synctime = true; _trs_synctime_value = value.ToLocalTime(); }
        }

        private bool _trs_mechanicgrade = false;
        private EntityReference _trs_mechanicgrade_value;
        public Guid trs_mechanicgrade
        {
            get { return _trs_mechanicgrade ? _trs_mechanicgrade_value.Id : Guid.Empty; }
            set { _trs_mechanicgrade = true; _trs_mechanicgrade_value = new EntityReference(_DL_trs_mechanicgrade.EntityName, value); }
        }


        private bool _trs_commercialtask = false;
        private EntityReference _trs_commercialtask_value;
        public Guid trs_commercialtask
        {
            get { return _trs_commercialtask ? _trs_commercialtask_value.Id : Guid.Empty; }
            set { _trs_commercialtask = true; _trs_commercialtask_value = new EntityReference(_DL_trs_commercialtask.EntityName, value); }
        }
        private bool _trs_rtg = false;
        private decimal _trs_rtg_value;
        public decimal trs_rtg
        {
            get { return _trs_rtg_value; }
            set { _trs_rtg = true; _trs_rtg_value = value; }
        }

        private bool _statuscode = false;
        private int _statuscode_value;
        public int statuscode
        {
            get { return _statuscode ? _statuscode_value : int.MinValue; }
            set { _statuscode = true; _statuscode_value = value; }
        }

        private bool _trs_fromquotation = false;
        private bool _trs_fromquotation_value;
        public bool trs_fromquotation
        {
            get { return _trs_fromquotation ? _trs_fromquotation_value : false; }
            set { _trs_fromquotation = true; _trs_fromquotation_value = value; }
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
                throw new Exception(_classname + ".Select : " + ex.Message, ex.InnerException);
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
                throw new Exception(_classname + ".Select : " + ex.Message, ex.InnerException);
            }
        }

        public Guid Insert(IOrganizationService organizationService)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                if (_trs_commercialheaderid) { entity.Attributes["trs_commercialheaderid"] = _trs_commercialheaderid_value; }
                if (_taskname) { entity.Attributes["trs_taskname"] = _taskname_value; }
                if (_taskcode) { entity.Attributes["trs_taskcode"] = _taskcode_value; }
                if (_trs_workorder) { entity.Attributes["trs_workorder"] = _trs_workorder_value; }
                if (_trs_commercialtask) { entity.Attributes["trs_commercialtask"] = _trs_commercialtask_value; }
                if (_trs_automatictime) { entity.Attributes["trs_automatictime"] = _trs_automatictime_value; }
                if (_trs_manualtime) { entity.Attributes["trs_manualtime"] = _trs_manualtime_value; }
                if (_trs_synctime) { entity.Attributes["trs_synctime"] = _trs_synctime_value; }
                if (_trs_mechanicgrade) { entity.Attributes["trs_mechanicgrade"] = _trs_mechanicgrade_value; }
                if (_trs_rtg) { entity.Attributes["trs_rtg"] = _trs_rtg_value; }
                if (_statuscode) { entity.Attributes["statuscode"] = new OptionSetValue(_statuscode_value); }
                if (_trs_fromquotation) { entity.Attributes["trs_fromquotation"] = _trs_fromquotation_value; };
                return organizationService.Create(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Insert : " + ex.Message, ex.InnerException);
            }
        }

        public void Update(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                entity.Id = id;
                //if (_trs_name) { entity.Attributes["trs_name"] = _trs_name_value; }
                //if (_trs_customer) { entity.Attributes["trs_customer"] = _trs_customer_value; }
                //if (_trs_functionallongitude) { entity.Attributes["trs_functionallongitude"] = _trs_functionallongitude_value; }
                //if (_trs_functionallatitude) { entity.Attributes["trs_functionallatitude"] = _trs_functionallatitude_value; }
                if (_trs_automatictime) { entity.Attributes["trs_automatictime"] = _trs_automatictime_value; }
                if (_trs_manualtime) { entity.Attributes["trs_manualtime"] = _trs_manualtime_value; }
                if (_trs_synctime) { entity.Attributes["trs_synctime"] = _trs_synctime_value; }
                if (_statuscode) { entity.Attributes["statuscode"] = new OptionSetValue(_statuscode_value); }
                if (_trs_fromquotation) { entity.Attributes["trs_fromquotation"] = _trs_fromquotation_value; };
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }

        public void Open(IOrganizationService organizationService, Guid id)
        {
            try
            {
                EntityReference entityReference = new EntityReference(_entityname, id);
                OrganizationRequest organizationRequest = new OrganizationRequest();
                organizationRequest.RequestName = "SetState";
                organizationRequest["EntityMoniker"] = entityReference;
                organizationRequest["State"] = new OptionSetValue(0);
                organizationRequest["Status"] = new OptionSetValue(1);
                organizationService.Execute(organizationRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Completed : " + ex.Message);
            }
        }

        public void Completed(IOrganizationService organizationService, Guid id)
        {
            try
            {
                EntityReference entityReference = new EntityReference(_entityname, id);
                OrganizationRequest organizationRequest = new OrganizationRequest();
                organizationRequest.RequestName = "SetState";
                organizationRequest["EntityMoniker"] = entityReference;
                organizationRequest["State"] = new OptionSetValue(0);
                organizationRequest["Status"] = new OptionSetValue(167630000);
                organizationService.Execute(organizationRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Completed : " + ex.Message);
            }
        }

        public void AssociateToolGroups(IOrganizationService organizationService, Guid id, List<Guid> toolGroupsId)
        {
            try
            {
                EntityReferenceCollection relatedEntites = new EntityReferenceCollection();
                foreach (Guid toolGroupId in toolGroupsId)
                {
                    relatedEntites.Add(new EntityReference(_DL_trs_toolsgroup.EntityName, toolGroupId));
                }

                Relationship relationship = new Relationship(_DL_trs_trs_commercialdetail_trs_toolsgroup.EntityName);
                organizationService.Associate(_entityname, id, relationship, relatedEntites);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".AssociateToolsGroup : " + ex.Message);
            }
        }
    }
}
