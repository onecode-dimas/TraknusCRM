using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.DataLayer
{
    public class DL_user
    {
        #region Properties
        private string _classname = "DL_user";

        private string _entityname = "systemuser";

        public string EntityName
        {
            get { return _entityname; }
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
                Entity entity = organizationService.Retrieve(_entityname, id, new ColumnSet(true));
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception("Error DL_user.Select : " + ex.Message, ex.InnerException);
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
                throw new Exception("Error DL_user.Select : " + ex.Message, ex.InnerException);
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
