using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_quotationcommercialheader
    {
        #region Properties
        private string _classname = "DL_trs_quotationcommercialheader";

        private string _entityname = "trs_quotationcommercialheader";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Quotation Commercial Header";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private string _trs_commercialheader = null;
        public string trs_commercialheader
        {
            get { return _trs_commercialheader; }
            set { _trs_commercialheader = value; }
        }

        private string _trs_quotationcommercialheaderid = null;
        public string trs_quotationcommercialheaderid
        {
            get { return _trs_quotationcommercialheaderid; }
            set { _trs_quotationcommercialheaderid = value; }
        }

        private bool _trs_totalrtg = false;
        private decimal _trs_totalrtg_value;
        public decimal trs_totalrtg
        {
            get { return _trs_totalrtg ? _trs_totalrtg_value : 0; }
            set { _trs_totalrtg = true; _trs_totalrtg_value = value; }
        }


        private bool _trs_price = false;
        private decimal _trs_price_value;
        public decimal trs_price
        {
            get { return _trs_price ? _trs_price_value : 0; }
            set { _trs_price = true; _trs_price_value = value; }
        }

        private bool _trs_totalprice = false;
        private decimal _trs_totalprice_value;
        public decimal trs_totalprice
        {
            get { return _trs_totalprice ? _trs_totalprice_value : 0; }
            set { _trs_totalprice = true; _trs_totalprice_value = value; }
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
                EntityCollection entityCollection = organizationService.RetrieveMultiple(queryExpression);
                return entityCollection;

            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Select  : " + ex.Message.ToString());
            }
        }

        #region insert and update
        //public void Insert(IOrganizationService organizationService)
        //{
        //    try
        //    {
        //        if (_trs_quotationnumber)
        //        {
        //            Entity entity = new Entity(_entityname);
        //            entity.Attributes["trs_quotationnumber"] = _trs_quotationnumber_value;
        //            organizationService.Create(entity);
        //        }
        //        else
        //        {
        //            throw new Exception("Primary Key is empty.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(_classname + ".Insert : " + ex.Message.ToString());
        //    }
        //}

        public void Update(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                entity.Id = id;
                if (_trs_totalrtg) { entity.Attributes["trs_totalrtg"] = _trs_totalrtg_value; };
                if (_trs_price) { entity.Attributes["trs_price"] = new Money(_trs_price_value); };
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}
