using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_commercialtask
    {
        #region Dependencies
        private DL_task _DL_task = new DL_task();
        private DL_serviceappointment _DL_serviceappointment = new DL_serviceappointment();
        private DL_trs_mechanicgrade _DL_trs_mechanicgrade = new DL_trs_mechanicgrade();
        private DL_trs_tasklist _DL_trs_tasklist = new DL_trs_tasklist();
        #endregion

        #region Properties
        private string _classname = "DL_commercialtask";

        private string _entityname = "trs_commercialtask";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Commercial Task";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _trs_taskcode = false;
        private string _trs_taskcode_value;
        public string trs_taskcode
        {
            get { return _trs_taskcode ? _trs_taskcode_value : null; }
            set { _trs_taskcode = true; _trs_taskcode_value = value; }
        }

        private bool _trs_commercialheaderid = false;
        private EntityReference _trs_commercialheaderid_value;
        public Guid trs_commercialheaderid
        {
            get { return _trs_commercialheaderid ? _trs_commercialheaderid_value.Id : Guid.Empty; }
            set { _trs_commercialheaderid = true; _trs_commercialheaderid_value = new EntityReference(_DL_task.EntityName, value); }
        }

        private bool _trs_commercialtask = false;
        private EntityReference _trs_commercialtask_value;
        public Guid trs_commercialtask
        {
            get { return _trs_commercialtask ? _trs_commercialtask_value.Id : Guid.Empty; }
            set { _trs_commercialtask = true; _trs_commercialtask_value = new EntityReference(_DL_serviceappointment.EntityName, value); }
        }

        private bool _trs_mechanicgrade = false;
        private EntityReference _trs_mechanicgrade_value;
        public Guid trs_mechanicgrade
        {
            get { return _trs_mechanicgrade ? _trs_mechanicgrade_value.Id : Guid.Empty; }
            set { _trs_mechanicgrade = true; _trs_mechanicgrade_value = new EntityReference(_DL_trs_mechanicgrade.EntityName, value); }
        }

        private bool _trs_taskname = false;
        private EntityReference _trs_taskname_value;
        public Guid trs_taskname
        {
            get { return _trs_taskname ? _trs_taskname_value.Id : Guid.Empty; }
            set { _trs_taskname = true; _trs_taskname_value = new EntityReference(_DL_trs_tasklist.EntityName, value); }
        }

        private bool _trs_rtg = false;
        private decimal _trs_rtg_value;
        public decimal trs_rtg
        {
            get { return _trs_rtg_value; }
            set { _trs_rtg = true; _trs_rtg_value = value; }
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
                throw new Exception(_classname + ".Select  : " + ex.Message.ToString());
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
                throw new Exception(_classname + ".Select  : " + ex.Message.ToString());
            }
        }

        public Guid Insert(IOrganizationService organizationService)
        {
            try
            {
                if (_trs_taskcode)
                {
                    Entity entity = new Entity(_entityname);
                    entity.Attributes["trs_taskcode"] = _trs_taskcode_value;
                    if (_trs_commercialheaderid) { entity.Attributes["trs_commercialheaderid"] = _trs_commercialheaderid_value; }
                    if (_trs_commercialtask) { entity.Attributes["trs_commercialtask"] = _trs_commercialtask_value; }
                    if (_trs_mechanicgrade) { entity.Attributes["trs_mechanicgrade"] = _trs_mechanicgrade_value; }
                    if (_trs_rtg) { entity.Attributes["trs_rtg"] = _trs_rtg_value; }
                    if (_trs_taskname) { entity.Attributes["trs_taskname"] = _trs_taskname_value; }
                    return organizationService.Create(entity);
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
    }
}
