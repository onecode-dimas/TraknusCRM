using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_quotationtool
    {
        #region Dependencies
        DL_trs_toolsmaster _DL_trs_toolsmaster = new DL_trs_toolsmaster();
        DL_trs_quotation _DL_trs_quotation = new DL_trs_quotation();
        DL_trs_toolsgroup _DL_trs_toolsgroup = new DL_trs_toolsgroup();
        #endregion

        #region Properties
        private string _classname = "DL_trs_quotationtool";

        private string _entityname = "trs_quotationtool";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Quotation Tool";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _trs_toolsmaster = false;
        private EntityReference _trs_toolsmaster_value;
        public Guid trs_toolsmaster
        {
            get { return _trs_toolsmaster ? _trs_toolsmaster_value.Id : Guid.Empty; }
            set { _trs_toolsmaster = true; _trs_toolsmaster_value = new EntityReference(_DL_trs_toolsmaster.EntityName, value); }
        }

        private bool _trs_quotation = false;
        private EntityReference _trs_quotation_value;
        public Guid trs_quotation
        {
            get { return _trs_quotation ? _trs_quotation_value.Id : Guid.Empty; }
            set { _trs_quotation = true; _trs_quotation_value = new EntityReference(_DL_trs_quotation.EntityName, value); }
        }

        private bool _trs_toolsgroup = false;
        private EntityReference _trs_toolsgroup_value;
        public Guid trs_toolsgroup
        {
            get { return _trs_toolsgroup ? _trs_toolsgroup_value.Id : Guid.Empty; }
            set { _trs_toolsgroup = true; _trs_toolsgroup_value = new EntityReference(_DL_trs_toolsgroup.EntityName, value); }
        }

        private bool _trs_toolsname = false;
        private string _trs_toolsname_value;
        public string trs_toolsname
        {
            get { return _trs_toolsname ? _trs_toolsname_value : string.Empty; }
            set { _trs_toolsname = true; _trs_toolsname_value = value; }
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
                    Entity entity = new Entity(_entityname);
                    if (_trs_toolsmaster) { entity.Attributes["trs_toolsmaster"] = _trs_toolsmaster_value; }
                    if (_trs_quotation) { entity.Attributes["trs_quotation"] = _trs_quotation_value; }
                    if (_trs_toolsgroup) { entity.Attributes["trs_toolsgroup"] = _trs_toolsgroup_value; }
                    if (_trs_toolsname) { entity.Attributes["trs_toolsname"] = _trs_toolsname_value; }
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
                if (_trs_toolsmaster) { entity.Attributes["trs_toolsmaster"] = _trs_toolsmaster_value; }
                if (_trs_quotation) { entity.Attributes["trs_quotation"] = _trs_quotation_value; }
                if (_trs_toolsgroup) { entity.Attributes["trs_toolsgroup"] = _trs_toolsgroup_value; }
                if (_trs_toolsname) { entity.Attributes["trs_toolsname"] = _trs_toolsname_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
