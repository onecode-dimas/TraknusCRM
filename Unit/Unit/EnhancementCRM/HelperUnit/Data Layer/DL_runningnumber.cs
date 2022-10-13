using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhancementCRM.HelperUnit.Data_Layer
{
    public class DL_runningnumber
    {
        #region Properties
        private string _classname = "DL_runningnumber";

        private string _entityname = "trs_runningnumber";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Running Number";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _trs_entityname = false;
        private string _trs_entityname_value;
        public string trs_entityname
        {
            get { return _trs_entityname ? _trs_entityname_value : null; }
            set { _trs_entityname = true; _trs_entityname_value = value; }
        }

        private bool _trs_prefix = false;
        private string _trs_prefix_value;
        public string trs_prefix
        {
            get { return _trs_prefix ? _trs_prefix_value : null; }
            set { _trs_prefix = true; _trs_prefix_value = value; }
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
                if (_trs_entityname)
                {
                    Entity entity = new Entity(_entityname);
                    entity.Attributes["trs_entityname"] = _trs_entityname_value;
                    if (_trs_prefix) { entity.Attributes["trs_prefix"] = _trs_prefix_value; }
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
                if (_trs_entityname) { entity.Attributes["trs_entityname"] = _trs_entityname_value; }
                if (_trs_prefix) { entity.Attributes["trs_prefix"] = _trs_prefix_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
