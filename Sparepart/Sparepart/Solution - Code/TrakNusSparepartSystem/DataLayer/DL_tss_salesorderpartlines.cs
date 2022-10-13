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
    public class DL_tss_salesorderpartlines
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
        private const string _soPart = "tss_sopartheader";
        private const string _masterPart = "trs_masterpart";
        private const string _priceLevel = "pricelevel";
        private const string _priceGroup = "new_pricegroup";
        private const string _priceList = "tss_pricelistpart";
        #endregion

        #region Properties
        private string _classname = "DL_tss_sopartlines";

        private string _entityname = "tss_sopartlines";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Sales Order Part Lines";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _tss_sopartheader = false;
        private EntityReference _tss_sopartheader_value;
        public Guid tss_sopartheader
        {
            get { return _tss_sopartheader ? _tss_sopartheader_value.Id : Guid.Empty; }
            set { _tss_sopartheader = true; _tss_sopartheader_value = new EntityReference(_soPart, value); }
        }

        private bool _tss_partnumber = false;
        private EntityReference _tss_partnumber_value;
        public Guid tss_partnumber
        {
            get { return _tss_partnumber ? _tss_partnumber_value.Id : Guid.Empty; }
            set { _tss_partnumber = true; _tss_partnumber_value = new EntityReference(_masterPart, value); }
        }

        private bool _tss_itemnumber = false;
        private int _tss_itemnumber_value;
        public int tss_itemnumber
        {
            get { return _tss_itemnumber ? _tss_itemnumber_value : int.MinValue; }
            set { _tss_itemnumber = true; _tss_itemnumber_value = value; }
        }

        private bool _tss_isinterchange = false;
        private bool _tss_isinterchange_value;
        public bool tss_isinterchange
        {
            get { return _tss_isinterchange ? _tss_isinterchange_value : false; }
            set { _tss_isinterchange = true; _tss_isinterchange_value = value; }
        }

        private bool _tss_pninterchange = false;
        private EntityReference _tss_pninterchange_value;
        public Guid tss_pninterchange
        {
            get { return _tss_pninterchange ? _tss_pninterchange_value.Id : Guid.Empty; }
            set { _tss_pninterchange = true; _tss_pninterchange_value = new EntityReference(_masterPart, value); }
        }

        private bool _tss_requestdeliverydate = false;
        private DateTime _tss_requestdeliverydate_value;
        public DateTime tss_requestdeliverydate
        {
            get { return _tss_requestdeliverydate ? _tss_requestdeliverydate_value.ToLocalTime() : DateTime.MinValue; }
            set { _tss_requestdeliverydate = true; _tss_requestdeliverydate_value = value.ToLocalTime(); }
        }

        //OptionSet
        private bool _tss_sourcetype = false;
        private int _tss_sourcetype_value;
        public int tss_sourcetype
        {
            get { return _tss_sourcetype ? _tss_sourcetype_value : int.MinValue; }
            set { _tss_sourcetype = true; _tss_sourcetype_value = value; }
        }

        //OptionSet
        private bool _tss_itemcategory = false;
        private int _tss_itemcategory_value;
        public int tss_itemcategory
        {
            get { return _tss_itemcategory ? _tss_itemcategory_value : int.MinValue; }
            set { _tss_itemcategory = true; _tss_itemcategory_value = value; }
        }

        private bool _tss_salesman = false;
        private EntityReference _tss_salesman_value;
        public Guid tss_salesman
        {
            get { return _tss_salesman ? _tss_salesman_value.Id : Guid.Empty; }
            set { _tss_salesman = true; _tss_salesman_value = new EntityReference(_DL_account.EntityName, value); }
        }

        //OptionSet
        private bool _tss_status = false;
        private int _tss_status_value;
        public int tss_status
        {
            get { return _tss_status ? _tss_status_value : int.MinValue; }
            set { _tss_status = true; _tss_status_value = value; }
        }

        private bool _tss_pricetype = false;
        private EntityReference _tss_pricetype_value;
        public Guid tss_pricetype
        {
            get { return _tss_pricetype ? _tss_pricetype_value.Id : Guid.Empty; }
            set { _tss_pricetype = true; _tss_pricetype_value = new EntityReference(_priceList, value); }
        }

        private bool _tss_pricegroup = false;
        private EntityReference _tss_pricegroup_value;
        public Guid tss_pricegroup
        {
            get { return _tss_pricegroup ? _tss_pricegroup_value.Id : Guid.Empty; }
            set { _tss_pricegroup = true; _tss_pricegroup_value = new EntityReference(_priceGroup, value); }
        }

        private bool _tss_unit = false;
        private EntityReference _tss_unit_value;
        public Guid tss_unit
        {
            get { return _tss_unit ? _tss_unit_value.Id : Guid.Empty; }
            set { _tss_unit = true; _tss_unit_value = new EntityReference("uom", value); }
        }

        private bool _tss_unitgroup = false;
        private EntityReference _tss_unitgroup_value;
        public Guid tss_unitgroup
        {
            get { return _tss_unitgroup ? _tss_unitgroup_value.Id : Guid.Empty; }
            set { _tss_unitgroup = true; _tss_unitgroup_value = new EntityReference("uomschedule", value); }
        }

        private bool _tss_currency = false;
        private EntityReference _tss_currency_value;
        public Guid tss_currency
        {
            get { return _tss_currency ? _tss_currency_value.Id : Guid.Empty; }
            set { _tss_currency = true; _tss_currency_value = new EntityReference(_DL_currency.EntityName, value); }
        }


        //Quantity Request
        private bool _tss_qtyrequest = false;
        private int _tss_qtyrequest_value;
        public int tss_qtyrequest
        {
            get { return _tss_qtyrequest ? _tss_qtyrequest_value : int.MinValue; }
            set { _tss_qtyrequest = true; _tss_qtyrequest_value = value; }
        }

        private bool _tss_finalprice = false;
        private Money _tss_finalprice_value;
        public decimal tss_finalprice
        {
            get { return _tss_finalprice ? _tss_finalprice_value.Value : 0; }
            set { _tss_finalprice = true; _tss_finalprice_value = new Money(value); }
        }

        private bool _tss_totalprice = false;
        private Money _tss_totalprice_value;
        public decimal tss_totalprice
        {
            get { return _tss_totalprice ? _tss_totalprice_value.Value : 0; }
            set { _tss_totalprice = true; _tss_totalprice_value = new Money(value); }
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

        public void CreateSOLinesFromQuotationLines(IOrganizationService organizationService, ITracingService trace)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                if (_tss_sopartheader) { entity.Attributes["tss_sopartheaderid"] = _tss_sopartheader_value; }
                trace.Trace("1: " + _tss_sopartheader_value);
                if (_tss_partnumber) { entity.Attributes["tss_partnumber"] = _tss_partnumber_value; }
                trace.Trace("2: " + _tss_partnumber_value);
                if (_tss_itemnumber) { entity.Attributes["tss_itemnumber"] = _tss_itemnumber_value; }
                trace.Trace("3: " + _tss_itemnumber_value);
                if (_tss_unit) { entity.Attributes["tss_unit"] = _tss_unit_value; }
                if (_tss_unitgroup) { entity.Attributes["tss_unitgroup"] = _tss_unitgroup_value; }
                if (_tss_pricegroup) { entity.Attributes["tss_pricegroup"] = _tss_pricegroup_value; }
                if (_tss_pricetype) { entity.Attributes["tss_pricetype"] = _tss_pricetype_value; }
                trace.Trace("4: " + _tss_unitgroup_value);
                if (_tss_sourcetype) { entity.Attributes["tss_sourcetype"] = new OptionSetValue(_tss_sourcetype_value); }
                trace.Trace("5: " + _tss_sourcetype_value);
                if (_tss_requestdeliverydate) { entity.Attributes["tss_requestdeliverydate"] = _tss_requestdeliverydate_value; }
                trace.Trace("6: " + _tss_requestdeliverydate_value);
                if (_tss_isinterchange) { entity.Attributes["tss_isinterchange"] = _tss_isinterchange_value; }
                trace.Trace("7: " + _tss_isinterchange_value);
                if (_tss_pninterchange) { entity.Attributes["tss_partnumberinterchange"] = _tss_pninterchange_value; }
                trace.Trace("8: " + _tss_pninterchange_value);
                if (_tss_status) { entity.Attributes["tss_status"] = new OptionSetValue(_tss_status_value); }
                trace.Trace("9: " + _tss_status_value);
                if (_tss_qtyrequest) { entity.Attributes["tss_qtyrequest"] = _tss_qtyrequest_value; }
                trace.Trace("10: " + _tss_qtyrequest_value);
                if (_tss_totalprice) { entity.Attributes["tss_totalprice"] = _tss_totalprice_value; }
                trace.Trace("11: " + _tss_totalprice_value);
                if (_tss_finalprice) { entity.Attributes["tss_finalprice"] = _tss_finalprice_value; }
                organizationService.Create(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".CreateSOLinesFromQuotationLines : " + ex.Message.ToString());
            }
        }
    }
}
