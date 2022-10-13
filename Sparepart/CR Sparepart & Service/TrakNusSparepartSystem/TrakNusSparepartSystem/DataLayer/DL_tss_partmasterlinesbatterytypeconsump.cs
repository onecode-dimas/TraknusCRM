using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TrakNusSparepartSystem.DataLayer
{
    public class DL_tss_partmasterlinesbatterytypeconsump
    {
        #region Dependencies
        #endregion

        #region Properties
        private string _classname = "DL_tss_partmasterlinesbatterytypeconsump";

        private string _entityname = "tss_partmasterlinesbatterytypeconsump";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Part Master Lines Battery Type Consump";
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

        private bool _tss_lastconsump = false;
        private DateTime _tss_lastconsump_value;
        public DateTime tss_lastconsump
        {
            get { return _tss_lastconsump ? _tss_lastconsump_value : DateTime.MinValue; }
            set { _tss_lastconsump = true; _tss_lastconsump_value = value.ToLocalTime(); }
        }

        private bool _tss_nextconsump = false;
        private DateTime _tss_nextconsump_value;
        public DateTime tss_nextconsump
        {
            get { return _tss_nextconsump ? _tss_nextconsump_value : DateTime.MinValue; }
            set { _tss_nextconsump = true; _tss_nextconsump_value = value.ToLocalTime(); }
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
                if (_tss_lastconsump) { entity.Attributes["tss_lastconsump"] = _tss_lastconsump_value; }
                if (_tss_nextconsump) { entity.Attributes["tss_nextconsump"] = _tss_nextconsump_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
