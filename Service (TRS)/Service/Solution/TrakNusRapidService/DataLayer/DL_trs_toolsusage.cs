using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_toolsusage
    {
        #region Dependencies
        private DL_trs_toolsmaster _DL_trs_toolsmaster = new DL_trs_toolsmaster();
        private DL_serviceappointment _DL_serviceappointment = new DL_serviceappointment();
        private DL_trs_toolsgroup _DL_trs_toolsgroup = new DL_trs_toolsgroup();
        #endregion

        #region Properties
        private string _classname = "DL_trs_toolsusage";

        private string _entityname = "trs_toolsusage";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Tools Usage";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private string _trs_toolsname;
        public string trs_toolsname
        {
            get { return _trs_toolsname; }
            set { _trs_toolsname = value; }
        }

        private DateTime _trs_dateborrowed;
        public DateTime trs_dateborrowed
        {
            get { return _trs_dateborrowed.ToLocalTime(); }
            set { _trs_dateborrowed = value.ToLocalTime(); }
        }

        private DateTime _trs_datereturned;
        public DateTime trs_datereturned
        {
            get { return _trs_datereturned.ToLocalTime(); }
            set { _trs_datereturned = value.ToLocalTime(); }
        }

        private bool _trs_conditionborrowed = false;
        private int _trs_conditionborrowed_value;
        public int trs_conditionborrowed
        {
            get { return _trs_conditionborrowed ? _trs_conditionborrowed_value : 0; }
            set { _trs_conditionborrowed = true; _trs_conditionborrowed_value = value; }
        }

        private bool _trs_conditionreturned = false;
        private int _trs_conditionreturned_value;
        public int trs_conditionreturned
        {
            get { return _trs_conditionreturned ? _trs_conditionreturned_value : 0; }
            set { _trs_conditionreturned = true; _trs_conditionreturned_value = value; }
        }

        private bool _trs_toolsmaster = false;
        private EntityReference _trs_toolsmaster_value;
        public Guid trs_toolsmaster
        {
            get { return _trs_toolsmaster ? _trs_toolsmaster_value.Id : Guid.Empty; }
            set { _trs_toolsmaster = true; _trs_toolsmaster_value = new EntityReference(_DL_trs_toolsmaster.EntityName, value); }
        }

        private bool _trs_price = false;
        private Money _trs_price_value;
        public decimal trs_price
        {
            get { return _trs_price ? _trs_price_value.Value : 0; }
            set { _trs_price = true; _trs_price_value = new Money(value); }
        }

        private bool _trs_toolsusage = false;
        private EntityReference _trs_toolsusage_value;
        public Guid trs_toolsusage
        {
            get { return _trs_toolsusage ? _trs_toolsusage_value.Id : Guid.Empty; }
            set { _trs_toolsusage = true; _trs_toolsusage_value = new EntityReference(_DL_serviceappointment.EntityName, value); }
        }
        #endregion

        public Entity Select(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = organizationService.Retrieve(_entityname, id, new ColumnSet(new string[] { "trs_toolsname", "trs_dateborrowed", "trs_datereturned", "trs_conditionborrowed", "trs_conditionreturned", "trs_toolsmaster" }));
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception("Error DL_trs_toolsusage.Select : " + ex.Message, ex.InnerException);
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
                throw new Exception("Error DL_trs_toolsusage.Select : " + ex.Message, ex.InnerException);
            }
        }

        public Guid Insert(IOrganizationService organizationService)
        {
            try
            {
                if (_trs_toolsmaster)
                {
                    Entity entity = new Entity(_entityname);
                    //entity.Attributes["trs_toolsname"] = _trs_toolsname;
                    //entity.Attributes["trs_dateborrowed"] = _trs_dateborrowed; 
                    //entity.Attributes["trs_datereturned"] = _trs_datereturned; 
                    if (_trs_conditionborrowed) { entity.Attributes["trs_conditionborrowed"] = _trs_conditionborrowed_value; }
                    if (_trs_conditionreturned) { entity.Attributes["trs_conditionreturned"] = _trs_conditionreturned_value; }
                    if (_trs_toolsusage) { entity.Attributes["trs_toolsusage"] = _trs_toolsusage_value; }
                    if (_trs_toolsmaster) { entity.Attributes["trs_toolsmaster"] = _trs_toolsmaster_value; }
                    if (_trs_price) { entity.Attributes["trs_price"] = _trs_price_value; }
                    return organizationService.Create(entity);
                }
                else
                {
                    throw new Exception(_classname + ".Insert : Tools Master is empty.");
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
                entity.Attributes["trs_toolsname"] = _trs_toolsname;
                entity.Attributes["trs_dateborrowed"] = _trs_dateborrowed;
                entity.Attributes["trs_datereturned"] = _trs_datereturned;
                entity.Attributes["trs_conditionborrowed"] = _trs_conditionborrowed;
                entity.Attributes["trs_conditionreturned"] = _trs_conditionreturned;
                entity.Attributes["trs_toolsmaster"] = _trs_toolsmaster;
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
