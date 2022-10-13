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
    public class DL_tss_salesorderpartlinesservice
    {
        #region Dependencies
        private DL_currency _DL_currency = new DL_currency();
        #endregion

        #region Contants
        private const string _soPart = "tss_sopartheader";
        #endregion

        #region Properties
        private string _classname = "DL_tss_sopartlinesservice";

        private string _entityname = "tss_sopartlinesservice";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Sales Order Part Lines - Service";
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

        private bool _tss_commercialheader = false;
        private string _tss_commercialheader_value;
        public string tss_commercialheader
        {
            get { return _tss_commercialheader ? _tss_commercialheader_value : string.Empty; }
            set { _tss_commercialheader = true; _tss_commercialheader_value = value; }
        }

        private bool _tss_price = false;
        private Money _tss_price_value;
        public decimal tss_price
        {
            get { return _tss_price ? _tss_price_value.Value : 0; }
            set { _tss_price = true; _tss_price_value = new Money(value); }
        }

        private bool _tss_discamount = false;
        private Money _tss_discamount_value;
        public decimal tss_discamount
        {
            get { return _tss_discamount ? _tss_discamount_value.Value : 0; }
            set { _tss_discamount = true; _tss_discamount_value = new Money(value); }
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

        public void CreateSOLinesService(IOrganizationService organizationService)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                if (_tss_sopartheader) { entity.Attributes["tss_sopartheaderid"] = _tss_sopartheader_value; }
                if (_tss_commercialheader) { entity.Attributes["tss_commercialheader"] = _tss_commercialheader_value; }
                if (_tss_price) { entity.Attributes["tss_price"] = _tss_price_value; }
                if (_tss_discamount) { entity.Attributes["tss_discountamount"] = _tss_discamount_value; }
                if (_tss_totalprice) { entity.Attributes["tss_totalprice"] = _tss_totalprice_value; }
                organizationService.Create(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".CreateSOLinesService : " + ex.Message.ToString());
            }
        }

    }
}
