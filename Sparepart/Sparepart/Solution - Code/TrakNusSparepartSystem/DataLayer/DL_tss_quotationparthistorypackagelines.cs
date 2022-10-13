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
    public class DL_tss_quotationparthistorypackagelines
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
        private const string _unitGroup = "trs_unitofmeasurement";
        private const string _priceLevel = "pricelevel";
        private const string _priceGroup = "new_pricegroup";
        private const string _priceList = "tss_pricelistpart";
        #endregion

        #region Properties
        private string _classname = "DL_tss_quotationparthistorypackagelines";

        private string _entityname = "tss_quotationparthistorypackagelines";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Quotation Part - History Package Lines";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _tss_partnumber = false;
        private EntityReference _tss_partnumber_value;
        public Guid tss_partnumber
        {
            get { return _tss_partnumber ? _tss_partnumber_value.Id : Guid.Empty; }
            set { _tss_partnumber = true; _tss_partnumber_value = new EntityReference(_masterPart, value); }
        }

        private bool _tss_quantity = false;
        private int _tss_quantity_value;
        public int tss_quantity
        {
            get { return _tss_quantity ? _tss_quantity_value : int.MinValue; }
            set { _tss_quantity = true; _tss_quantity_value = value; }
        }

        private bool _tss_price = false;
        private Money _tss_price_value;
        public decimal tss_price
        {
            get { return _tss_price ? _tss_price_value.Value : 0; }
            set { _tss_price = true; _tss_price_value = new Money(value); }
        }

        private bool _tss_finalprice = false;
        private Money _tss_finalprice_value;
        public decimal tss_finalprice
        {
            get { return _tss_finalprice ? _tss_finalprice_value.Value : 0; }
            set { _tss_finalprice = true; _tss_finalprice_value = new Money(value); }
        }

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

        private bool _tss_totalprice = false;
        private Money _tss_totalprice_value;
        public decimal tss_totalprice
        {
            get { return _tss_totalprice ? _tss_totalprice_value.Value : 0; }
            set { _tss_totalprice = true; _tss_totalprice_value = new Money(value); }
        }

        private bool _tss_historypackage = false;
        private EntityReference _tss_historypackage_value;
        public Guid tss_historypackage
        {
            get { return _tss_historypackage ? _tss_historypackage_value.Id : Guid.Empty; }
            set { _tss_historypackage = true; _tss_historypackage_value = new EntityReference("tss_quotationparthistorypackage", value); }
        }

        private bool _tss_partnumberinterchange = false;
        private EntityReference _tss_partnumberinterchange_value;
        public Guid tss_partnumberinterchange
        {
            get { return _tss_partnumberinterchange ? _tss_partnumberinterchange_value.Id : Guid.Empty; }
            set { _tss_partnumberinterchange = true; _tss_partnumberinterchange_value = new EntityReference("tss_partmasterlinesinterchange", value); }
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

        public void CreateQuotationPartHistoryPackageLines(IOrganizationService organizationService)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                if (_tss_historypackage) { entity.Attributes["tss_historypackage"] = _tss_historypackage_value; }
                if (_tss_partnumber) { entity.Attributes["tss_partnumber"] = _tss_partnumber_value; }
                if (_tss_quantity) { entity.Attributes["tss_quantity"] = _tss_quantity_value; }
                if (_tss_price) { entity.Attributes["tss_price"] = _tss_price_value; }
                if (_tss_finalprice) { entity.Attributes["tss_finalprice"] = _tss_finalprice_value; }
                if (_tss_discountby) { entity.Attributes["tss_discountby"] = _tss_discountby_value; }
                if (_tss_discamount) { entity.Attributes["tss_discountamount"] = _tss_discamount_value; }
                if (_tss_discpercent) { entity.Attributes["tss_discountpercent"] = _tss_discpercent_value; }
                if (_tss_totalprice) { entity.Attributes["tss_totalprice"] = _tss_totalprice_value; }
                if (_tss_partnumberinterchange) { entity.Attributes["tss_partnumberinterchange"] = _tss_partnumberinterchange_value; }
                organizationService.Create(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".CreateQuotationPartHistoryPackageLines : " + ex.Message.ToString());
            }
        }
    }
}
