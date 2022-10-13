using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.DataLayer
{
    public class DL_tss_marketsizepartconsump
    {
        #region Dependencies
        #endregion

        #region Properties
        private string _classname = "DL_tss_marketsizepartconsump";

        private string _entityname = "tss_marketsizepartconsump";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Market Size Part Consump";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _name = false;
        private string _name_value;
        public string name
        {
            get { return _name ? _name_value : null; }
            set { _name = true; _name_value = value; }
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
                if (_name)
                {
                    Entity entity = new Entity(_entityname);
                    entity.Attributes["name"] = _name_value;
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
                if (_name) { entity.Attributes["name"] = _name_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
