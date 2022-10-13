using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TrakNusSparepartSystem.DataLayer;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusSparepartSystem.DataLayer
{
    public class DL_tss_quotationpartlines
    {
        #region Dependencies
        private DL_account _DL_account = new DL_account();
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        private DL_businessunit _DL_businessunit = new DL_businessunit();
        private DL_tss_prospectpartheader _DL_tss_prospectpartheader = new DL_tss_prospectpartheader();
        private DL_trs_quotation _DL_trs_quotation = new DL_trs_quotation();
        private DL_contact _DL_contact = new DL_contact();
        private DL_currency _DL_currency = new DL_currency();
        #endregion

        #region Contants
        private const string _quotationPart = "tss_quotationpartheader";
        private const string _masterPart = "trs_masterpart";
        private const string _unitGroup = "uomschedule";
        private const string _priceListPart = "tss_pricelistpart";
        private const string _partinterchange = "tss_partmasterlinesinterchange";
        #endregion

        #region Properties
        private string _classname = "DL_tss_quotationpartlines";

        private string _entityname = "tss_quotationpartlines";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Quotation Part Lines";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _tss_quopartheader = false;
        private EntityReference _tss_quopartheader_value;
        public Guid tss_quopartheader
        {
            get { return _tss_quopartheader ? _tss_quopartheader_value.Id : Guid.Empty; }
            set { _tss_quopartheader = true; _tss_quopartheader_value = new EntityReference(_quotationPart, value); }
        }

        //OptionSet
        private bool _tss_sourcetype = false;
        private int _tss_sourcetype_value;
        public int tss_sourcetype
        {
            get { return _tss_sourcetype ? _tss_sourcetype_value : int.MinValue; }
            set { _tss_sourcetype = true; _tss_sourcetype_value = value; }
        }

        private bool _tss_partnumber = false;
        private EntityReference _tss_partnumber_value;
        public Guid tss_partnumber
        {
            get { return _tss_partnumber ? _tss_partnumber_value.Id : Guid.Empty; }
            set { _tss_partnumber = true; _tss_partnumber_value = new EntityReference(_masterPart, value); }
        }

        private bool _tss_isinterchange = false;
        private bool _tss_isinterchange_value;
        public bool tss_isinterchange
        {
            get { return _tss_isinterchange ? _tss_isinterchange_value : false; }
            set { _tss_isinterchange = true; _tss_isinterchange_value = value; }
        }

        private bool _tss_partnumberinterchange = false;
        private EntityReference _tss_partnumberinterchange_value;
        public Guid tss_partnumberinterchange
        {
            get { return _tss_partnumberinterchange ? _tss_partnumberinterchange_value.Id : Guid.Empty; }
            set { _tss_partnumberinterchange = true; _tss_partnumberinterchange_value = new EntityReference(_partinterchange, value); }
        }

        private bool _tss_itemnumber = false;
        private int _tss_itemnumber_value;
        public int tss_itemnumber
        {
            get { return _tss_itemnumber ? _tss_itemnumber_value : int.MinValue; }
            set { _tss_itemnumber = true; _tss_itemnumber_value = value; }
        }

        private bool _tss_partdescription = false;
        private string _tss_partdescription_value;
        public string tss_partdescription
        {
            get { return _tss_partdescription ? _tss_partdescription_value : null; }
            set { _tss_partdescription = true; _tss_partdescription_value = value; }
        }

        private bool _tss_unitgroup = false;
        private EntityReference _tss_unitgroup_value;
        public Guid tss_unitgroup
        {
            get { return _tss_unitgroup ? _tss_unitgroup_value.Id : Guid.Empty; }
            set { _tss_unitgroup = true; _tss_unitgroup_value = new EntityReference(_unitGroup, value); }
        }

        private bool _tss_pricetype = false;
        private EntityReference _tss_pricetype_value;
        public Guid tss_pricetype
        {
            get { return _tss_pricetype ? _tss_pricetype_value.Id : Guid.Empty; }
            set { _tss_pricetype = true; _tss_pricetype_value = new EntityReference(_priceListPart, value); }
        }

        private bool _tss_qty = false;
        private int _tss_qty_value;
        public int tss_qty
        {
            get { return _tss_qty ? _tss_qty_value : int.MinValue; }
            set { _tss_qty = true; _tss_qty_value = value; }
        }

        private bool _tss_price = false;
        private Money _tss_price_value;
        public decimal tss_price
        {
            get { return _tss_price ? _tss_price_value.Value : 0; }
            set { _tss_price = true; _tss_price_value = new Money(value); }
        }

        private bool _tss_priceafterdisc = false;
        private Money _tss_priceafterdisc_value;
        public decimal tss_priceafterdisc
        {
            get { return _tss_priceafterdisc ? _tss_priceafterdisc_value.Value : 0; }
            set { _tss_priceafterdisc = true; _tss_priceafterdisc_value = new Money(value); }
        }

        private bool _tss_finalprice = false;
        private Money _tss_finalprice_value;
        public decimal tss_finalprice
        {
            get { return _tss_finalprice ? _tss_finalprice_value.Value : 0; }
            set { _tss_finalprice = true; _tss_finalprice_value = new Money(value); }
        }

        private bool _tss_underminprice = false;
        private bool _tss_underminprice_value;
        public bool tss_underminprice
        {
            get { return _tss_underminprice ? _tss_underminprice_value : false; }
            set { _tss_underminprice = true; _tss_underminprice_value = value; }
        }

        private bool _tss_minprice = false;
        private Money _tss_minprice_value;
        public decimal tss_minprice
        {
            get { return _tss_minprice ? _tss_minprice_value.Value : 0; }
            set { _tss_minprice = true; _tss_minprice_value = new Money(value); }
        }

        private bool _tss_totalprice = false;
        private Money _tss_totalprice_value;
        public decimal tss_totalprice
        {
            get { return _tss_totalprice ? _tss_totalprice_value.Value : 0; }
            set { _tss_totalprice = true; _tss_totalprice_value = new Money(value); }
        }

        private bool _tss_totalfinalprice = false;
        private Money _tss_totalfinalprice_value;
        public decimal tss_totalfinalprice
        {
            get { return _tss_totalfinalprice ? _tss_totalfinalprice_value.Value : 0; }
            set { _tss_totalfinalprice = true; _tss_totalfinalprice_value = new Money(value); }
        }

        //updated by indra - field optionset tss_top
        private bool _tss_discountby = false;
        private bool _tss_discountby_value;
        public bool tss_discountby
        {
            get { return _tss_discountby ? _tss_discountby_value : false; }
            set { _tss_discountby = true; _tss_discountby_value = value; }
        }

        private bool _tss_discamount = false;
        private Money _tss_discamount_value;
        public decimal tss_discamount
        {
            get { return _tss_discamount ? _tss_discamount_value.Value : 0; }
            set { _tss_discamount = true; _tss_discamount_value = new Money(value); }
        }

        private bool _tss_discpercent = false;
        private decimal _tss_discpercent_value;
        public decimal tss_discpercent
        {
            get { return _tss_discpercent ? _tss_discpercent_value : 0; }
            set { _tss_discpercent = true; _tss_discpercent_value = value; }
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
                throw new Exception("Error DL_tss_quotationpartheader.Select : " + ex.Message, ex.InnerException);
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
                throw new Exception("Error DL_tss_quotationpartheader.Select : " + ex.Message, ex.InnerException);
            }
        }

        public void CreateQuotationLinesFromProspectLines(IOrganizationService organizationService)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                if (_tss_quopartheader) { entity.Attributes["tss_quotationpartheader"] = _tss_quopartheader_value; }
                if (_tss_sourcetype) { entity.Attributes["tss_sourcetype"] = new OptionSetValue(_tss_sourcetype_value); }
                if (_tss_partnumber) { entity.Attributes["tss_partnumber"] = _tss_partnumber_value; }
                if (_tss_isinterchange) { entity.Attributes["tss_isinterchange"] = _tss_isinterchange_value; }
                if (_tss_partnumberinterchange) { entity.Attributes["tss_partnumberinterchange"] = _tss_partnumberinterchange_value; }
                if (_tss_itemnumber) { entity.Attributes["tss_itemnumber"] = _tss_itemnumber_value; }
                if (_tss_unitgroup) { entity.Attributes["tss_unitgroup"] = _tss_unitgroup_value; }
                if (_tss_pricetype) { entity.Attributes["tss_pricetype"] = _tss_pricetype_value; }
                if (_tss_qty) { entity.Attributes["tss_quantity"] = _tss_qty_value; }
                if (_tss_price) { entity.Attributes["tss_price"] = _tss_price_value; }
                if (_tss_priceafterdisc) { entity.Attributes["tss_priceafterdiscount"] = _tss_priceafterdisc_value; }
                if (_tss_discountby) { entity.Attributes["tss_discountby"] = _tss_discountby_value; }
                if (_tss_discamount) { entity.Attributes["tss_discountamount"] = _tss_discamount_value; }
                if (_tss_discpercent) { entity.Attributes["tss_discountpercent"] = _tss_discpercent_value; }
                if (_tss_totalprice) { entity.Attributes["tss_totalprice"] = _tss_totalprice_value; }
                if (_tss_minprice) { entity.Attributes["tss_minimumprice"] = _tss_minprice_value; }
                if (tss_underminprice) { entity.Attributes["tss_underminimumprice"] = _tss_underminprice_value; }
                //if (_tss_totalfinalprice) { entity.Attributes["tss_totalfinalprice"] = _tss_totalfinalprice_value; }
                organizationService.Create(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".CreateQuotationLinesFromProspectLines : " + ex.Message.ToString());
            }
        }

    }
}
