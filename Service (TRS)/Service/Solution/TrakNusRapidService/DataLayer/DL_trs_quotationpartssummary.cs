using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_quotationpartssummary
    {
        #region Dependencies
        private DL_trs_quotation _DL_trs_quotation = new DL_trs_quotation();
        private DL_trs_masterpart _DL_trs_masterpart = new DL_trs_masterpart();
        private DL_transactioncurrency _DL_transactioncurrency = new DL_transactioncurrency();
        #endregion

        #region Properties
        private string _classname = "DL_trs_quotationpartssummary";

        private string _entityname = "trs_quotationpartssummary";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Quotation Parts Summary";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _trs_quotationnumber = false;
        private EntityReference _trs_quotationnumber_value;
        public Guid trs_quotationnumber
        {
            get { return _trs_quotationnumber ? _trs_quotationnumber_value.Id : Guid.Empty; }
            set { _trs_quotationnumber = true; _trs_quotationnumber_value = new EntityReference(_DL_trs_quotation.EntityName, value); }
        }

        private bool _trs_partnumber = false;
        private EntityReference _trs_partnumber_value;
        public Guid trs_partnumber
        {
            get { return _trs_partnumber ? _trs_partnumber_value.Id : Guid.Empty; }
            set { _trs_partnumber = true; _trs_partnumber_value = new EntityReference(_DL_trs_masterpart.EntityName, value); }
        }

        private bool _trs_manualquantity = false;
        private int _trs_manualquantity_value;
        public int trs_manualquantity
        {
            get { return _trs_manualquantity ? _trs_manualquantity_value :  int.MinValue; }
            set { _trs_manualquantity = true; _trs_manualquantity_value = value; }
        }

        private bool _trs_tasklistquantity;
        private int _trs_tasklistquantity_value;
        public int trs_tasklistquantity
        {
            get { return _trs_tasklistquantity ? _trs_manualquantity_value :  int.MinValue; }
            set { _trs_tasklistquantity = true; _trs_tasklistquantity_value = value; }
        }

        private bool _trs_itemnumber = false;
        private int _trs_itemnumber_value;
        public int trs_itemnumber
        {
            get { return _trs_itemnumber ? _trs_itemnumber_value : 1; }
            set { _trs_itemnumber = true; _trs_itemnumber_value = value; }
        }

        private bool _trs_price = false;
        private Money _trs_price_value;
        public Money trs_price
        {
            get { return _trs_price ? _trs_price_value : new Money(0); }
            set { _trs_price = true; _trs_price_value = value; }
        }

        private bool _trs_discountpercent = false;
        private decimal _trs_discountpercent_value;
        public decimal trs_discountpercent
        {
            get { return _trs_discountpercent ? _trs_discountpercent_value : 0; }
            set { _trs_discountpercent = true; _trs_discountpercent_value = value; }
        }

        private bool _trs_discountamount = false;
        private Money _trs_discountamount_value;
        public Money trs_discountamount
        {
            get { return _trs_discountamount ? _trs_discountamount_value : new Money(0); }
            set { _trs_discountamount = true; _trs_discountamount_value = value; }
        }

        private bool _trs_totalprice = false;
        private Money _trs_totalprice_value;
        public Money trs_totalprice
        {
            get { return _trs_totalprice ? _trs_totalprice_value : new Money(0); }
            set { _trs_totalprice = true; _trs_totalprice_value = value; }
        }

        private bool _transactioncurrencyid = false;
        private EntityReference _transactioncurrencyid_value;
        public Guid transactioncurrencyid
        {
            get { return _transactioncurrencyid ? _transactioncurrencyid_value.Id : Guid.Empty; }
            set { _transactioncurrencyid = true; _transactioncurrencyid_value = new EntityReference(_DL_transactioncurrency.EntityName, value); }
        }

        private bool _trs_partname = false;
        private string _trs_partname_value;
        public string trs_partname
        {
            get { return _trs_partname ? _trs_partname_value : null; }
            set { _trs_partname = true; _trs_partname_value = value; }
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
                if (_trs_quotationnumber)
                {
                    Entity entity = new Entity(_entityname);
                    if (_trs_quotationnumber) { entity.Attributes["trs_quotationnumber"] = _trs_quotationnumber_value; }
                    if (_trs_partnumber) { entity.Attributes["trs_partnumber"] = _trs_partnumber_value; }
                    if (_trs_manualquantity) { entity.Attributes["trs_manualquantity"] = _trs_manualquantity_value; }
                    if (_trs_tasklistquantity) { entity.Attributes["trs_tasklistquantity"] = _trs_tasklistquantity_value; }
                    if (_trs_itemnumber) { entity.Attributes["trs_itemnumber"] = _trs_itemnumber_value; }
                    if (_trs_price) { entity.Attributes["trs_price"] = _trs_price_value; }
                    if (_trs_discountpercent) { entity.Attributes["trs_discountpercent"] = _trs_discountpercent_value; }
                    if (_trs_discountamount) { entity.Attributes["trs_discountamount"] = _trs_discountamount_value; }
                    if (_trs_totalprice) { entity.Attributes["trs_totalprice"] = _trs_totalprice_value; }
                    if (_transactioncurrencyid) { entity.Attributes["transactioncurrencyid"] = _transactioncurrencyid_value; }
                    if (_trs_partname) { entity.Attributes["trs_partname"] = _trs_partname_value; }
                    organizationService.Create(entity);
                }
                else
                {
                    throw new Exception(_classname + ".Insert : Quotation Number is empty.");
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
                if (_trs_quotationnumber) { entity.Attributes["trs_quotationnumber"] = _trs_quotationnumber_value; }
                if (_trs_partnumber) { entity.Attributes["trs_partnumber"] = _trs_partnumber_value; }
                if (_trs_manualquantity) { entity.Attributes["trs_manualquantity"] = _trs_manualquantity_value; }
                if (_trs_tasklistquantity) { entity.Attributes["trs_tasklistquantity"] = _trs_tasklistquantity_value; }
                if (_trs_itemnumber) { entity.Attributes["trs_itemnumber"] = _trs_itemnumber_value; }
                if (_trs_price) { entity.Attributes["trs_price"] = _trs_price_value; }
                if (_trs_discountpercent) { entity.Attributes["trs_discountpercent"] = _trs_discountpercent_value; }
                if (_trs_discountamount) { entity.Attributes["trs_discountamount"] = _trs_discountamount_value; }
                if (_trs_totalprice) { entity.Attributes["trs_totalprice"] = _trs_totalprice_value; }
                if (_transactioncurrencyid) { entity.Attributes["transactioncurrencyid"] = _transactioncurrencyid_value; }
                if (_trs_partname) { entity.Attributes["trs_partname"] = _trs_partname_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
