using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_quotationpartdetail
    {
        #region Dependencies
        private DL_trs_quotation _DL_trs_quotation = new DL_trs_quotation();
        private DL_trs_quotationcommercialdetail _DL_trs_quotationcommercialdetail = new DL_trs_quotationcommercialdetail();
        private DL_trs_masterpart _DL_trs_masterpart = new DL_trs_masterpart();
        #endregion

        #region Properties
        private string _classname = "DL_trs_quotationpartdetail";

        private string _entityname = "trs_quotationpartdetail";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Quotation Part Detail";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _trs_partdescription;
        private string _trs_partdescription_value;
        public string trs_partdescription
        {
            get { return _trs_partdescription ? _trs_partdescription_value : string.Empty; }
            set { _trs_partdescription = true;  _trs_partdescription_value = value; }
        }

        private bool _trs_partnumber = false;
        private EntityReference _trs_partnumber_value;
        public Guid trs_partnumber
        {
            get { return _trs_partnumber ? _trs_partnumber_value.Id : Guid.Empty; }
            set { _trs_partnumber = true; _trs_partnumber_value = new EntityReference(_DL_trs_masterpart.EntityName, value); }
        }

        private bool _trs_quotation = false;
        private EntityReference _trs_quotation_value;
        public Guid trs_quotation
        {
            get { return _trs_quotation ? _trs_quotation_value.Id : Guid.Empty; }
            set { _trs_quotation = true; _trs_quotation_value = new EntityReference(_DL_trs_quotation.EntityName, value); }
        }

        private bool _trs_commercialdetailid = false;
        private EntityReference _trs_commercialdetailid_value;
        public Guid trs_commercialdetailid
        {
            get { return _trs_commercialdetailid ? _trs_commercialdetailid_value.Id : Guid.Empty; }
            set { _trs_commercialdetailid = true; _trs_commercialdetailid_value = new EntityReference(_DL_trs_quotationcommercialdetail.EntityName, value); }
        }

        private bool _trs_quantity = false;
        private int _trs_quantity_value;
        public int trs_quantity
        {
            get { return _trs_quantity ? _trs_quantity_value : 0; }
            set { _trs_quantity = true; _trs_quantity_value = value; }
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

        public void Insert(IOrganizationService organizationService)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                if (_trs_quotation) { entity.Attributes["trs_quotation"] = _trs_quotation_value; }
                if (_trs_commercialdetailid) { entity.Attributes["trs_commercialdetailid"] = _trs_commercialdetailid_value; }
                if (_trs_partnumber) { entity.Attributes["trs_partnumber"] = _trs_partnumber_value; }
                if (_trs_partdescription) { entity.Attributes["trs_partdescription"] = _trs_partdescription_value; }
                if (_trs_quantity) { entity.Attributes["trs_quantity"] = _trs_quantity_value; }
                organizationService.Create(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Insert : " + ex.Message, ex.InnerException);
            }
        }

        public void Update(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                entity.Id = id;
                if (_trs_quotation) { entity.Attributes["trs_quotation"] = _trs_quotation_value; }
                if (_trs_commercialdetailid) { entity.Attributes["trs_commercialdetailid"] = _trs_commercialdetailid_value; }
                if (_trs_partnumber) { entity.Attributes["trs_partnumber"] = _trs_partnumber_value; }
                if (_trs_partdescription) { entity.Attributes["trs_partdescription"] = _trs_partdescription_value; }
                if (_trs_quantity) { entity.Attributes["trs_quantity"] = _trs_quantity_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message, ex.InnerException);
            }
        }
    }
}
