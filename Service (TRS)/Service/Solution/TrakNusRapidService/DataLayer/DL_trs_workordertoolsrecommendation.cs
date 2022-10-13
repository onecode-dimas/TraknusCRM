using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_workordertoolsrecommendation
    {
        #region Dependencies
        private DL_serviceappointment _DL_serviceappointment = new DL_serviceappointment();
        private DL_trs_toolsgroup _DL_trs_toolsgroup = new DL_trs_toolsgroup();
        #endregion

        #region Properties
        private string _classname = "DL_trs_workordertoolsrecommendation";

        private string _entityname = "trs_workordertoolsrecommendation";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Work Order (Tools Recommendation)";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _trs_workorder = false;
        private EntityReference _trs_workorder_value;
        public Guid trs_workorder
        {
            get { return _trs_workorder ? _trs_workorder_value.Id : Guid.Empty; }
            set { _trs_workorder = true; _trs_workorder_value = new EntityReference(_DL_serviceappointment.EntityName, value); }
        }

        private bool _trs_toolsgroupid = false;
        private EntityReference _trs_toolsgroupid_value;
        public Guid trs_toolsgroupid
        {
            get { return _trs_toolsgroupid ? _trs_toolsgroupid_value.Id : Guid.Empty; }
            set { _trs_toolsgroupid = true; _trs_toolsgroupid_value = new EntityReference(_DL_trs_toolsgroup.EntityName, value); }
        }
        #endregion

        public Entity Select(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = organizationService.Retrieve(_entityname, id, new ColumnSet(true));
                return entity;
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
                EntityCollection entityCollection = organizationService.RetrieveMultiple(queryExpression);
                return entityCollection;
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Select : " + ex.Message, ex.InnerException);
            }
        }

        public void Insert(IOrganizationService organizationService)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                if (_trs_workorder) { entity.Attributes["trs_workorder"] = _trs_workorder_value; }
                if (_trs_toolsgroupid) { entity.Attributes["trs_toolsgroupid"] = _trs_toolsgroupid_value; }
                organizationService.Create(entity);
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
                if (_trs_workorder) { entity.Attributes["trs_workorder"] = _trs_workorder_value; };
                if (_trs_toolsgroupid) { entity.Attributes["trs_toolsgroupid"] = _trs_toolsgroupid; };
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
