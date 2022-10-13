using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_workorderpartrecommendation
    {
        #region Dependencies
        private DL_serviceappointment _DL_serviceappointment = new DL_serviceappointment();
        private DL_trs_productsection _DL_trs_productsection = new DL_trs_productsection();
        private DL_trs_commercialtask _DL_trs_commercialtask = new DL_trs_commercialtask();
        private DL_trs_masterpart _DL_trs_masterpart = new DL_trs_masterpart();
        private DL_trs_tasklistheader _DL_trs_tasklistheader = new DL_trs_tasklistheader();
        #endregion

        #region Properties
        private string _classname = "DL_trs_workorderpartrecommendation";

        private string _entityname = "trs_workorderpartrecommendation";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Work Order Recommendation";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _trs_partdescription = false;
        private string _trs_partdescription_value;
        public string trs_partdescription
        {
            get { return _trs_partdescription ? _trs_partdescription_value : string.Empty; }
            set { _trs_partdescription = true; _trs_partdescription_value = value; }
        }

        private bool _trs_workorder = false;
        private EntityReference _trs_workorder_value;
        public Guid trs_workorder
        {
            get { return _trs_workorder ? _trs_workorder_value.Id : Guid.Empty; }
            set { _trs_workorder = true; _trs_workorder_value = new EntityReference(_DL_serviceappointment.EntityName, value); }
        }

        private bool _trs_section = false;
        private EntityReference _trs_section_value;
        public Guid trs_section
        {
            get { return _trs_section ? _trs_section_value.Id : Guid.Empty; }
            set { _trs_section = true; _trs_section_value = new EntityReference(_DL_trs_productsection.EntityName, value); }
        }

        private bool _trs_tasklistdetailid = false;
        private EntityReference _trs_tasklistdetailid_value;
        public Guid trs_tasklistdetailid
        {
            get { return _trs_tasklistdetailid ? _trs_tasklistdetailid_value.Id : Guid.Empty; }
            set { _trs_tasklistdetailid = true; _trs_tasklistdetailid_value = new EntityReference(_DL_trs_commercialtask.EntityName, value); }
        }

        private bool _trs_partnumber = false;
        private EntityReference _trs_partnumber_value;
        public Guid trs_partnumber
        {
            get { return _trs_partnumber ? _trs_partnumber_value.Id : Guid.Empty; }
            set { _trs_partnumber = true; _trs_partnumber_value = new EntityReference(_DL_trs_masterpart.EntityName, value); }
        }

        private bool _trs_quantity = false;
        private int _trs_quantity_value;
        public int trs_quantity
        {
            get { return _trs_quantity ? _trs_quantity_value : int.MinValue; }
            set { _trs_quantity = true; _trs_quantity_value = value; }
        }

        private bool _trs_synctime = false;
        private DateTime _trs_synctime_value;
        public DateTime trs_synctime
        {
            get { return _trs_synctime ? _trs_synctime_value.ToLocalTime() : DateTime.MinValue; }
            set { _trs_synctime = true; _trs_synctime_value = value.ToLocalTime(); }
        }

        private bool _trs_frommobile = false;
        private bool _trs_frommobile_value;
        public bool trs_frommobile
        {
            get { return _trs_frommobile ? _trs_frommobile_value : false; }
            set { _trs_frommobile = true; _trs_frommobile_value = value; }
        }

        private bool _trs_mobileguid = false;
        private string _trs_mobileguid_value;
        public string trs_mobileguid
        {
            get { return _trs_mobileguid ? _trs_mobileguid_value : string.Empty; }
            set { _trs_mobileguid = true; _trs_mobileguid_value = value; }
        }

        private bool _trs_partnumbercatalog = false;
        private string _trs_partnumbercatalog_value;
        public string trs_partnumbercatalog
        {
            get { return _trs_partnumbercatalog ? _trs_partnumbercatalog_value : string.Empty; }
            set { _trs_partnumbercatalog = true; _trs_partnumbercatalog_value = value; }
        }

        private bool _trs_task = false;
        private EntityReference _trs_task_value;
        public Guid trs_task
        {
            get { return _trs_task ? _trs_task_value.Id : Guid.Empty; }
            set { _trs_task = true; _trs_task_value = new EntityReference(_DL_trs_tasklistheader.EntityName, value); }
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

        public Guid Insert(IOrganizationService organizationService)
        {
            try
            {
                if (_trs_workorder)
                {
                    Entity entity = new Entity(_entityname);
                    if (_trs_workorder) { entity.Attributes["trs_workorder"] = _trs_workorder_value; }
                    if (_trs_partdescription) { entity.Attributes["trs_partdescription"] = _trs_partdescription_value; }
                    if (_trs_section) { entity.Attributes["trs_section"] = _trs_section_value; }
                    if (_trs_tasklistdetailid) { entity.Attributes["trs_tasklistdetailid"] = _trs_tasklistdetailid_value; }
                    if (_trs_partnumber) { entity.Attributes["trs_partnumber"] = _trs_partnumber_value; }
                    if (_trs_quantity) { entity.Attributes["trs_quantity"] = _trs_quantity_value; }
                    if (_trs_synctime) { entity.Attributes["trs_synctime"] = _trs_synctime_value; }
                    if (_trs_mobileguid) { entity.Attributes["trs_mobileguid"] = _trs_mobileguid_value; }
                    if (_trs_partnumbercatalog) { entity.Attributes["trs_partnumbercatalog"] = _trs_partnumbercatalog_value; }
                    if (_trs_task) { entity.Attributes["trs_task"] = _trs_task_value; }
                    return organizationService.Create(entity);
                }
                else
                {
                    throw new Exception(_classname + ".Insert : Quotation Number is empty.");
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
                if (_trs_workorder) { entity.Attributes["trs_workorder"] = _trs_workorder_value; }
                if (_trs_partdescription) { entity.Attributes["trs_partdescription"] = _trs_partdescription_value; }
                if (_trs_section) { entity.Attributes["trs_section"] = _trs_section_value; }
                if (_trs_tasklistdetailid) { entity.Attributes["trs_tasklistdetailid"] = _trs_tasklistdetailid_value; }
                if (_trs_partnumber) { entity.Attributes["trs_partnumber"] = _trs_partnumber_value; }
                if (_trs_quantity) { entity.Attributes["trs_quantity"] = _trs_quantity_value; }
                if (_trs_synctime) { entity.Attributes["trs_synctime"] = _trs_synctime_value; }
                if (_trs_mobileguid) { entity.Attributes["trs_mobileguid"] = _trs_mobileguid_value; }
                if (_trs_partnumbercatalog) { entity.Attributes["trs_partnumbercatalog"] = _trs_partnumbercatalog_value; }
                if (_trs_task) { entity.Attributes["trs_task"] = _trs_task_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
