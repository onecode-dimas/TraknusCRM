using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace DataLayer
{
    public class DL_systemuser
    {
        #region Dependencies
        #endregion

        #region Properties
        private string _classname = "DL_systemuser";

        private string _entityname = "systemuser";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "User";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _fullname = false;
        private string _fullname_value;
        public string fullname
        {
            get { return _fullname ? _fullname_value : null; }
            set { _fullname = true; _fullname_value = value; }
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
                if (_fullname)
                {
                    Entity entity = new Entity(_entityname);
                    entity.Attributes["fullname"] = _fullname_value;
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
                if (_fullname) { entity.Attributes["fullname"] = _fullname_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
