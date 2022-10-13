using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_workordersupportingmaterial
    {
        #region Dependencies
        private DL_serviceappointment _DL_serviceappointment = new DL_serviceappointment();
        #endregion

        #region Properties
        private string _classname = "DL_trs_workordersupportingmaterial";

        private string _entityname = "trs_workordersupportingmaterial";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Workorder Supporting Material";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _trs_itemnumber = false;
        private int _trs_itemnumber_value;
        public int trs_itemnumber
        {
            get { return _trs_itemnumber ? _trs_itemnumber_value : 0; }
            set { _trs_itemnumber = true; _trs_itemnumber_value = value; }
        }

        private bool _trs_workorderid = false;
        private EntityReference _trs_workorderid_value;
        public Guid trs_workorderid
        {
            get { return _trs_workorderid ? _trs_workorderid_value.Id : Guid.Empty; }
            set { _trs_workorderid = true; _trs_workorderid_value = new EntityReference(_DL_serviceappointment.EntityName, value); }
        }

        private bool _trs_quantity = false;
        private int _trs_quantity_value;
        public int trs_quantity
        {
            get { return _trs_quantity ? _trs_quantity_value : 0; }
            set { _trs_quantity = true; _trs_quantity_value = value; }
        }

        private bool _trs_price = false;
        private Money _trs_price_value;
        public decimal trs_price
        {
            get { return _trs_price ? _trs_price_value.Value : 0; }
            set { _trs_price = true; _trs_price_value = new Money(value); }
        }

        private string _trs_supportingmaterialname;
        public string trs_supportingmaterialname
        {
            get { return _trs_supportingmaterialname; }
            set { _trs_supportingmaterialname = value; }
        }

        private bool _trs_standardtext = false;
        private OptionSetValue _trs_standardtext_value;
        public int trs_standardtext
        {
            get { return _trs_standardtext ? _trs_standardtext_value.Value : int.MinValue; }
            set { _trs_standardtext = true; _trs_standardtext_value = new OptionSetValue(value); }
        }

        private bool _trs_totalprice = false;
        private Money _trs_totalprice_value;
        public decimal trs_totalprice
        {
            get { return _trs_totalprice ? _trs_totalprice_value.Value : 0; }
            set { _trs_totalprice = true; _trs_totalprice_value = new Money(value); }
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

        public void Update(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                entity.Id = id;
                if (_trs_itemnumber) { entity.Attributes["trs_itemnumber"] = _trs_itemnumber_value; };
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }


        public void Insert(IOrganizationService organizationService)
        {
            try
            {

                Entity entity = new Entity(_entityname);
                if (_trs_workorderid) { entity.Attributes["trs_workorderid"] = _trs_workorderid_value; }
                if (_trs_quantity) { entity.Attributes["trs_quantity"] = _trs_quantity_value; }
                if (_trs_price) { entity.Attributes["trs_price"] = _trs_price_value; }
                if (_trs_standardtext) { entity.Attributes["trs_standardtext"] = _trs_standardtext_value; }
                if (_trs_totalprice) { entity.Attributes["trs_totalprice"] = _trs_totalprice_value; }
                entity.Attributes["trs_supportingmaterialname"] = _trs_supportingmaterialname;
                organizationService.Create(entity);

            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Insert : " + ex.Message.ToString());
            }
        }
    }
}
