using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.DataLayer
{
    public class DL_tss_marketsizeresultnational
    {
        #region Dependencies
        #endregion

        #region Properties
        private string _classname = "DL_tss_marketsizeresultnational";

        private string _entityname = "tss_marketsizeresultnational";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Market Size Result National";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _tss_totaluio = false;
        private int _tss_totaluio_value;
        public int tss_totaluio
        {
            get { return _tss_totaluio ? _tss_totaluio_value : int.MinValue; }
            set { _tss_totaluio = true; _tss_totaluio_value = value; }
        }

        private bool _tss_totalnonuio = false;
        private int _tss_totalnonuio_value;
        public int tss_totalnonuio
        {
            get { return _tss_totalnonuio ? _tss_totalnonuio_value : int.MinValue; }
            set { _tss_totalnonuio = true; _tss_totalnonuio_value = value; }
        }

        private bool _tss_totalgroupuiocommodity = false;
        private int _tss_totalgroupuiocommodity_value;
        public int tss_totalgroupuiocommodity
        {
            get { return _tss_totalgroupuiocommodity ? _tss_totalgroupuiocommodity_value : int.MinValue; }
            set { _tss_totalgroupuiocommodity = true; _tss_totalgroupuiocommodity_value = value; }
        }

        private bool _tss_totalalluio = false;
        private int _tss_totalalluio_value;
        public int tss_totalalluio
        {
            get { return _tss_totalalluio ? _tss_totalalluio_value : int.MinValue; }
            set { _tss_totalalluio = true; _tss_totalalluio_value = value; }
        }

        private bool _tss_totalmscommodity = false;
        private Money _tss_totalmscommodity_value;
        public decimal tss_totalmscommodity
        {
            get { return _tss_totalmscommodity ? _tss_totalmscommodity_value.Value : 0; }
            set { _tss_totalmscommodity = true; _tss_totalmscommodity_value = new Money(value); }
        }

        private bool _tss_totalmsstandardpart = false;
        private Money _tss_totalmsstandardpart_value;
        public decimal tss_totalmsstandardpart
        {
            get { return _tss_totalmsstandardpart ? _tss_totalmsstandardpart_value.Value : 0; }
            set { _tss_totalmsstandardpart = true; _tss_totalmsstandardpart_value = new Money(value); }
        }

        private bool _tss_totalamountms = false;
        private Money _tss_totalamountms_value;
        public decimal tss_totalamountms
        {
            get { return _tss_totalamountms ? _tss_totalamountms_value.Value : 0; }
            set { _tss_totalamountms = true; _tss_totalamountms_value = new Money(value); }
        }

        private bool _tss_msperiodstart = false;
        private DateTime _tss_msperiodstart_value;
        public DateTime tss_msperiodstart
        {
            get { return _tss_msperiodstart ? _tss_msperiodstart_value.ToLocalTime() : DateTime.MinValue; }
            set { _tss_msperiodstart = true; _tss_msperiodstart_value = value.ToLocalTime(); }
        }

        private bool _tss_msperiodend = false;
        private DateTime _tss_msperiodend_value;
        public DateTime tss_msperiodend
        {
            get { return _tss_msperiodend ? _tss_msperiodend_value.ToLocalTime() : DateTime.MinValue; }
            set { _tss_msperiodend = true; _tss_msperiodend_value = value.ToLocalTime(); }
        }

        private bool _tss_activeperiodstart = false;
        private DateTime _tss_activeperiodstart_value;
        public DateTime tss_activeperiodstart
        {
            get { return _tss_activeperiodstart ? _tss_activeperiodstart_value.ToLocalTime() : DateTime.MinValue; }
            set { _tss_activeperiodstart = true; _tss_activeperiodstart_value = value.ToLocalTime(); }
        }

        private bool _tss_activeperiodend = false;
        private DateTime _tss_activeperiodend_value;
        public DateTime tss_activeperiodend
        {
            get { return _tss_activeperiodend ? _tss_activeperiodend_value.ToLocalTime() : DateTime.MinValue; }
            set { _tss_activeperiodend = true; _tss_activeperiodend_value = value.ToLocalTime(); }
        }

        //Option Set
        private bool _tss_status = false;
        private int _tss_status_value;
        public int tss_status
        {
            get { return _tss_status ? _tss_status_value : int.MinValue; }
            set { _tss_status = true; _tss_status_value = value; }
        }

        //Option Set
        private bool _tss_statusreason = false;
        private int _tss_statusreason_value;
        public int tss_statusreason
        {
            get { return _tss_statusreason ? _tss_statusreason_value : int.MinValue; }
            set { _tss_statusreason = true; _tss_statusreason_value = value; }
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
                if (_tss_totalalluio) { entity.Attributes["tss_totalalluio"] = _tss_totalalluio_value; }
                if (_tss_totalnonuio) { entity.Attributes["tss_totalnonuio"] = _tss_totalnonuio_value; }
                if (_tss_totalgroupuiocommodity) { entity.Attributes["tss_totalgroupuiocommodity"] = _tss_totalgroupuiocommodity_value; }
                if (_tss_totalalluio) { entity.Attributes["tss_totalalluio"] = _tss_totalalluio_value; }
                if (_tss_totalmscommodity) { entity.Attributes["tss_totalmscommodity"] = _tss_totalmscommodity_value; }
                if (_tss_totalmsstandardpart) { entity.Attributes["tss_totalmsstandardpart"] = _tss_totalmsstandardpart_value; }
                if (_tss_totalamountms) { entity.Attributes["tss_totalamountms"] = _tss_totalamountms_value; }
                if (_tss_msperiodstart) { entity.Attributes["tss_msperiodstart"] = _tss_msperiodstart_value; }
                if (_tss_msperiodend) { entity.Attributes["tss_msperiodend"] = _tss_msperiodend_value; }
                if (_tss_activeperiodstart) { entity.Attributes["tss_activeperiodstart"] = _tss_activeperiodstart_value; }
                if (_tss_activeperiodend) { entity.Attributes["tss_activeperiodend"] = _tss_activeperiodend_value; }
                if (_tss_status) { entity.Attributes["tss_status"] = new OptionSetValue(_tss_status_value); }
                if (_tss_statusreason) { entity.Attributes["tss_statusreason"] = new OptionSetValue(_tss_statusreason_value); }
                return organizationService.Create(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Insert : " + ex.Message.ToString());
            }
        }
    }
}
