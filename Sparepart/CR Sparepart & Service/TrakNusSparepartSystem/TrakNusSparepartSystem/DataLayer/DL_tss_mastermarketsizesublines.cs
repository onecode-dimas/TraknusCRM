using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.DataLayer
{
    public class DL_tss_mastermarketsizesublines
    {
        #region Dependencies
        DL_tss_mastermarketsize _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        DL_tss_mastermarketsizelines _DL_tss_mastermarketsizelines = new DL_tss_mastermarketsizelines();
        DL_trs_masterpart _DL_trs_masterpart = new DL_trs_masterpart();
        #endregion

        #region Properties
        private string _classname = "DL_tss_mastermarketsizesublines";

        private string _entityname = "tss_mastermarketsizesublines";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Master Market Size Sub Lines";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _tss_mastermslinesref = false;
        private EntityReference _tss_mastermslinesref_value;
        public Guid tss_mastermslinesref
        {
            get { return _tss_mastermslinesref ? _tss_mastermslinesref_value.Id : Guid.Empty; }
            set { _tss_mastermslinesref = true; _tss_mastermslinesref_value = new EntityReference(_DL_tss_mastermarketsizelines.EntityName, value); }
        }

        private bool _tss_mastermarketsizeid = false;
        private EntityReference _tss_mastermarketsizeid_value;
        public Guid tss_mastermarketsizeid
        {
            get { return _tss_mastermarketsizeid ? _tss_mastermarketsizeid_value.Id : Guid.Empty; }
            set { _tss_mastermarketsizeid = true; _tss_mastermarketsizeid_value = new EntityReference(_DL_tss_mastermarketsize.EntityName, value); }
        }

        public bool _tss_partnumber = false;
        private EntityReference _tss_partnumber_value;
        public Guid tss_partnumber
        {
            get { return _tss_partnumber ? _tss_partnumber_value.Id : Guid.Empty; }
            set { _tss_partnumber = true; _tss_partnumber_value = new EntityReference(_DL_trs_masterpart.EntityName, value); }
        }

        private bool _tss_description = false;
        private string _tss_description_value;
        public string tss_description
        {
            get { return _tss_description ? _tss_description_value : null; }
            set { _tss_description = true; _tss_description_value = value; }
        }

        private bool _tss_partdescription = false;
        private string _tss_partdescription_value;
        public string tss_partdescription
        {
            get { return _tss_partdescription ? _tss_partdescription_value : null; }
            set { _tss_partdescription = true; _tss_partdescription_value = value; }
        }

        private bool _tss_qty = false;
        private int _tss_qty_value;
        public int tss_qty
        {
            get { return _tss_qty ? _tss_qty_value : int.MinValue; }
            set { _tss_qty = true; _tss_qty_value = value; }
        }

        private bool _tss_originalqty = false;
        private int _tss_originalqty_value;
        public int tss_originalqty
        {
            get { return _tss_originalqty ? _tss_originalqty_value : int.MinValue; }
            set { _tss_originalqty = true; _tss_originalqty_value = value; }
        }

        private bool _tss_price = false;
        private Money _tss_price_value;
        public decimal tss_price
        {
            get { return _tss_price ? _tss_price_value.Value : 0; }
            set { _tss_price = true; _tss_price_value = new Money(value); }
        }

        private bool _tss_minimumprice = false;
        private Money _tss_minimumprice_value;
        public decimal tss_minimumprice
        {
            get { return _tss_minimumprice ? _tss_minimumprice_value.Value : 0; }
            set { _tss_minimumprice = true; _tss_minimumprice_value = new Money(value); }
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
                throw new Exception(_classname + ".Select : " + ex.Message.ToString());
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
                throw new Exception(_classname + ".Select : " + ex.Message.ToString());
            }
        }
        public Guid Insert(IOrganizationService organizationService)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                if (_tss_mastermslinesref) { entity.Attributes["tss_mastermslinesref"] = _tss_mastermslinesref_value; }
                if (_tss_mastermarketsizeid) { entity.Attributes["tss_mastermarketsizeid"] = _tss_mastermarketsizeid_value; }
                if (_tss_partnumber) { entity.Attributes["tss_partnumber"] = _tss_partnumber_value; }
                if (_tss_description) { entity.Attributes["tss_description"] = _tss_description_value; }
                if (_tss_partdescription) { entity.Attributes["tss_partdescription"] = _tss_partdescription_value; }
                if (_tss_qty) { entity.Attributes["tss_qty"] = _tss_qty_value; }
                if (_tss_originalqty) { entity.Attributes["tss_originalqty"] = _tss_originalqty_value; }
                if (_tss_price) { entity.Attributes["tss_price"] = _tss_price_value; }
                if (_tss_minimumprice) { entity.Attributes["tss_minimumprice"] = _tss_minimumprice_value; }
                return organizationService.Create(entity);
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
                if (_tss_mastermslinesref) { entity.Attributes["tss_mastermslinesref"] = _tss_mastermslinesref_value; }
                if (_tss_mastermarketsizeid) { entity.Attributes["tss_mastermarketsizeid"] = _tss_mastermarketsizeid_value; }
                if (_tss_partnumber) { entity.Attributes["tss_partnumber"] = _tss_partnumber_value; }
                if (_tss_description) { entity.Attributes["tss_description"] = _tss_description_value; }
                if (_tss_partdescription) { entity.Attributes["tss_partdescription"] = _tss_partdescription_value; }
                if (_tss_qty) { entity.Attributes["tss_qty"] = _tss_qty_value; }
                if (_tss_originalqty) { entity.Attributes["tss_originalqty"] = _tss_originalqty_value; }
                if (_tss_price) { entity.Attributes["tss_price"] = _tss_price_value; }
                if (_tss_minimumprice) { entity.Attributes["tss_minimumprice"] = _tss_minimumprice_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
