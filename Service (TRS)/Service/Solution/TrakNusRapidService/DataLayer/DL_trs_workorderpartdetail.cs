using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_workorderpartdetail
    {
        #region Dependencies        
        private DL_serviceappointment _DL_serviceappointment = new DL_serviceappointment();
        private DL_trs_commercialdetail _DL_trs_commercialdetail = new DL_trs_commercialdetail();
        private DL_trs_masterpart _DL_trs_masterpart = new DL_trs_masterpart(); 
        #endregion

        #region Properties
        private string _classname = "DL_trs_workorderpartdetail";

        private string _entityname = "trs_workorderpartdetail";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Work Order Part Detail";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private string _trs_partdescription;
        public string trs_partdescription
        {
            get { return _trs_partdescription; }
            set { _trs_partdescription = value; }
        }
        
        private bool _trs_partnumber = false;
        private EntityReference _trs_partnumber_value;
        public Guid trs_partnumber
        {
            get { return _trs_partnumber ? _trs_partnumber_value.Id : Guid.Empty; }
            set { _trs_partnumber = true; _trs_partnumber_value = new EntityReference(_DL_trs_masterpart.EntityName, value); }
        }
        
        private bool _trs_task = false;
        private EntityReference _trs_task_value;
        public Guid trs_task
        {
            get { return _trs_task ? _trs_task_value.Id : Guid.Empty; }
            set { _trs_task = true; _trs_task_value = new EntityReference(_DL_trs_commercialdetail.EntityName, value); }
        }
        
        private bool _trs_workorder = false;
        private EntityReference _trs_workorder_value;
        public Guid trs_workorder
        {
            get { return _trs_workorder ? _trs_workorder_value.Id : Guid.Empty; }
            set { _trs_workorder = true; _trs_workorder_value = new EntityReference(_DL_serviceappointment.EntityName, value); }
        }

        private bool _trs_quantity = false;
        private int _trs_quantity_value;
        public int trs_quantity
        {
            get { return _trs_quantity ? _trs_quantity_value : 0; }
            set { _trs_quantity = true;  _trs_quantity_value = value; }
        }
        #endregion

        public Entity Select(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = organizationService.Retrieve(_entityname, id, new ColumnSet(new string[] { "trs_taskcode", "trs_taskname" }));
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception("Error DL_trs_commercialdetail.Select : " + ex.Message, ex.InnerException);
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
                throw new Exception("Error DL_trs_commercialdetail.Select : " + ex.Message, ex.InnerException);
            }
        }

        public void Insert(IOrganizationService organizationService)
        {
            try
            {
                if (!string.IsNullOrEmpty(_trs_partdescription))
                {
                    Entity entity = new Entity(_entityname);
                    entity.Attributes["trs_partdescription"] = _trs_partdescription;
                    if (_trs_partnumber) { entity.Attributes["trs_partnumber"] = _trs_partnumber_value; }
                    if (_trs_task) { entity.Attributes["trs_task"] = _trs_task_value; }
                    if (_trs_workorder) { entity.Attributes["trs_workorder"] = _trs_workorder_value; }
                    if (_trs_quantity) { entity.Attributes["trs_quantity"] = _trs_quantity_value; }
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
    }
}
