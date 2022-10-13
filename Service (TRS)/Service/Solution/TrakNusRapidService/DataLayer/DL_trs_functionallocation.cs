using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_functionallocation
    {
        #region Dependencies
        private DL_account _DL_account = new DL_account();
        #endregion

        #region Properties
        private string _classname = "DL_trs_functionallocation";

        private string _entityname = "trs_functionallocation";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Functional Location";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _trs_name = false;
        private string _trs_name_value;
        public string trs_name
        {
            get { return _trs_name ? _trs_name_value : null; }
            set { _trs_name = true; _trs_name_value = value; }
        }

        private bool _trs_customer = false;
        private EntityReference _trs_customer_value;
        public Guid trs_customer
        {
            get { return _trs_customer ? _trs_customer_value.Id : Guid.Empty; }
            set { _trs_customer = true; _trs_customer_value = new EntityReference(_DL_account.EntityName, value); }
        }

        private bool _trs_functionallongitude = false;
        private decimal _trs_functionallongitude_value;
        public decimal trs_functionallongitude
        {
            get { return _trs_functionallongitude ? _trs_functionallongitude_value : decimal.MinValue; }
            set { _trs_functionallongitude = true; _trs_functionallongitude_value = value; }
        }

        private bool _trs_functionallatitude = false;
        private decimal _trs_functionallatitude_value;
        public decimal trs_functionallatitude
        {
            get { return _trs_functionallatitude ? _trs_functionallatitude_value : decimal.MinValue; }
            set { _trs_functionallatitude = true; _trs_functionallatitude_value = value; }
        }

        private bool _trs_frommobile = false;
        private bool _trs_frommobile_value;
        public bool trs_frommobile
        {
            get { return _trs_frommobile ? _trs_frommobile_value : false; }
            set { _trs_frommobile = true; _trs_frommobile_value = value; }
        }

        private bool _trs_functionaladdress = false;
        private string _trs_functionaladdress_value;
        public string trs_functionaladdress
        {
            get { return _trs_functionaladdress ? _trs_functionaladdress_value : null; }
            set { _trs_functionaladdress = true; _trs_functionaladdress_value = value; }
        }

        private bool _trs_functionalcode = false;
        private string _trs_functionalcode_value;
        public string trs_functionalcode
        {
            get { return _trs_functionalcode ? _trs_functionalcode_value : null; }
            set { _trs_functionalcode = true; _trs_functionalcode_value = value; }
        }

        private bool _trs_area = false;
        private string _trs_area_value;
        public string trs_area
        {
            get { return _trs_area ? _trs_area_value : null; }
            set { _trs_area = true; _trs_area_value = value; }
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

        public Guid Insert(IOrganizationService organizationService)//, out Guid id)
        {
            try
            {
                if (_trs_name)
                {
                    Entity entity = new Entity(_entityname);
                    entity.Attributes["trs_name"] = _trs_name_value;
                    if (_trs_customer) { entity.Attributes["trs_customer"] = _trs_customer_value; }
                    if (_trs_functionallongitude) { entity.Attributes["trs_functionallongitude"] = _trs_functionallongitude_value; }
                    if (_trs_functionallatitude) { entity.Attributes["trs_functionallatitude"] = _trs_functionallatitude_value; }
                    if (_trs_functionallatitude) { entity.Attributes["trs_frommobile"] = _trs_frommobile_value; }
                    if (_trs_functionalcode) { entity.Attributes["trs_functionalcode"] = _trs_functionalcode_value; }
                    if (_trs_functionaladdress) { entity.Attributes["trs_functionaladdress"] = _trs_functionaladdress_value; }
                    if (_trs_area) { entity.Attributes["trs_area"] = _trs_area_value; }
                    return organizationService.Create(entity);
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
                if (_trs_name) { entity.Attributes["trs_name"] = _trs_name_value; }
                if (_trs_customer) { entity.Attributes["trs_customer"] = _trs_customer_value; }
                if (_trs_functionallongitude) { entity.Attributes["trs_functionallongitude"] = _trs_functionallongitude_value; }
                if (_trs_functionallatitude) { entity.Attributes["trs_functionallatitude"] = _trs_functionallatitude_value; }
                if (_trs_functionallatitude) { entity.Attributes["trs_frommobile"] = _trs_frommobile_value; }
                if (_trs_functionalcode) { entity.Attributes["trs_functionalcode"] = _trs_functionalcode_value; }
                if (_trs_functionaladdress) { entity.Attributes["trs_functionaladdress"] = _trs_functionaladdress_value; }
                if (_trs_area) { entity.Attributes["trs_area"] = _trs_area_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
