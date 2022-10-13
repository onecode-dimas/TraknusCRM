using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.DataLayer
{
    public class DL_tss_marketsizesummarybyparttype
    {
        #region Dependencies
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        private DL_businessunit _DL_businessunit = new DL_businessunit();
        private DL_tss_marketsizeresultpss _DL_tss_marketsizeresultpss = new DL_tss_marketsizeresultpss();
        #endregion

        #region Properties
        private string _classname = "DL_tss_marketsizesummarybyparttype";

        private string _entityname = "tss_marketsizesummarybyparttype";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Market Size Summary by Part Type";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _tss_mssummaryid = false;
        private string _tss_mssummaryid_value;
        public string tss_mssummaryid
        {
            get { return _tss_mssummaryid ? _tss_mssummaryid_value : null; }
            set { _tss_mssummaryid = true; _tss_mssummaryid_value = value; }
        }

        private bool _tss_marketsizeid = false;
        private EntityReference _tss_marketsizeid_value;
        public Guid tss_marketsizeid
        {
            get { return _tss_marketsizeid ? _tss_marketsizeid_value.Id : Guid.Empty; }
            set { _tss_marketsizeid = true; _tss_marketsizeid_value = new EntityReference(_DL_tss_marketsizeresultpss.EntityName, value); }
        }

        private bool _tss_pss = false;
        private EntityReference _tss_pss_value;
        public Guid tss_pss
        {
            get { return _tss_pss ? _tss_pss_value.Id : Guid.Empty; }
            set { _tss_pss = true; _tss_pss_value = new EntityReference(_DL_systemuser.EntityName, value); }
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

        private bool _tss_activeperiodsend = false;
        private DateTime _tss_activeperiodsend_value;
        public DateTime tss_activeperiodsend
        {
            get { return _tss_activeperiodsend ? _tss_activeperiodsend_value.ToLocalTime() : DateTime.MinValue; }
            set { _tss_activeperiodsend = true; _tss_activeperiodsend_value = value.ToLocalTime(); }
        }

        private bool _tss_mainparts = false;
        private Money _tss_mainparts_value;
        public decimal tss_mainparts
        {
            get { return _tss_mainparts ? _tss_mainparts_value.Value : 0; }
            set { _tss_mainparts = true; _tss_mainparts_value = new Money(value); }
        }

        private bool _tss_typeagro = false;
        private Money _tss_typeagro_value;
        public decimal tss_typeagro
        {
            get { return _tss_typeagro ? _tss_typeagro_value.Value : 0; }
            set { _tss_typeagro = true; _tss_typeagro_value = new Money(value); }
        }

        private bool _tss_typeindustry = false;
        private Money _tss_typeindustry_value;
        public decimal tss_typeindustry
        {
            get { return _tss_typeindustry ? _tss_typeindustry_value.Value : 0; }
            set { _tss_typeindustry = true; _tss_typeindustry_value = new Money(value); }
        }

        private bool _tss_oil = false;
        private Money _tss_oil_value;
        public decimal tss_oil
        {
            get { return _tss_oil ? _tss_oil_value.Value : 0; }
            set { _tss_oil = true; _tss_oil_value = new Money(value); }
        }

        private bool _tss_batterytraction = false;
        private Money _tss_batterytraction_value;
        public decimal tss_batterytraction
        {
            get { return _tss_batterytraction ? _tss_batterytraction_value.Value : 0; }
            set { _tss_batterytraction = true; _tss_batterytraction_value = new Money(value); }
        }

        private bool _tss_batterycranking = false;
        private Money _tss_batterycranking_value;
        public decimal tss_batterycranking
        {
            get { return _tss_batterycranking ? _tss_batterycranking_value.Value : 0; }
            set { _tss_batterycranking = true; _tss_batterycranking_value = new Money(value); }
        }

        private bool _tss_totalamountms = false;
        private Money _tss_totalamountms_value;
        public decimal tss_totalamountms
        {
            get { return _tss_totalamountms ? _tss_totalamountms_value.Value : 0; }
            set { _tss_totalamountms = true; _tss_totalamountms_value = new Money(value); }
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
                if (_tss_mssummaryid) { entity.Attributes["tss_mssummaryid"] = _tss_mssummaryid_value; }
                if (_tss_marketsizeid) { entity.Attributes["tss_marketsizeid"] = _tss_marketsizeid_value; }
                if (_tss_pss) { entity.Attributes["tss_pss"] = _tss_pss_value; }
                if (_tss_msperiodstart) { entity.Attributes["tss_msperiodstart"] = _tss_msperiodstart_value; }
                if (_tss_msperiodend) { entity.Attributes["tss_msperiodend"] = _tss_msperiodend_value; }
                if (_tss_activeperiodstart) { entity.Attributes["tss_activeperiodstart"] = _tss_activeperiodstart_value; }
                if (_tss_activeperiodsend) { entity.Attributes["tss_activeperiodsend"] = _tss_activeperiodsend_value; }
                if (_tss_mainparts) { entity.Attributes["tss_mainparts"] = _tss_mainparts_value; }
                if (_tss_typeagro) { entity.Attributes["tss_typeagro"] = _tss_typeagro_value; }
                if (_tss_typeindustry) { entity.Attributes["tss_typeindustry"] = _tss_typeindustry_value; }
                if (_tss_oil) { entity.Attributes["tss_oil"] = _tss_oil_value; }
                if (_tss_batterycranking) { entity.Attributes["tss_batterycranking"] = _tss_batterycranking_value; }
                if (_tss_batterytraction) { entity.Attributes["tss_batterytraction"] = _tss_batterytraction_value; }
                if (_tss_totalamountms) { entity.Attributes["tss_totalamountms"] = _tss_totalamountms_value; }
                return organizationService.Create(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Insert : " + ex.Message.ToString());
            }
        }
        public void Update(IOrganizationService organizationService, Guid Id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                entity.Id = Id;
                if (_tss_mssummaryid) { entity.Attributes["tss_mssummaryid"] = _tss_mssummaryid_value; }
                if (_tss_marketsizeid) { entity.Attributes["tss_marketsizeid"] = _tss_marketsizeid_value; }
                if (_tss_pss) { entity.Attributes["_tss_pss"] = _tss_pss_value; }
                if (_tss_msperiodstart) { entity.Attributes["tss_msperiodstart"] = _tss_msperiodstart_value; }
                if (_tss_msperiodend) { entity.Attributes["tss_msperiodend"] = _tss_msperiodend_value; }
                if (_tss_activeperiodstart) { entity.Attributes["tss_activeperiodstart"] = _tss_activeperiodstart_value; }
                if (_tss_activeperiodsend) { entity.Attributes["tss_activeperiodsend"] = _tss_activeperiodsend_value; }
                if (_tss_mainparts) { entity.Attributes["tss_mainparts"] = _tss_mainparts_value; }
                if (_tss_typeagro) { entity.Attributes["tss_typeagro"] = _tss_typeagro_value; }
                if (_tss_typeindustry) { entity.Attributes["tss_typeindustry"] = _tss_typeindustry_value; }
                if (_tss_oil) { entity.Attributes["tss_oil"] = _tss_oil_value; }
                if (_tss_batterycranking) { entity.Attributes["tss_batterycranking"] = _tss_batterycranking_value; }
                if (_tss_batterytraction) { entity.Attributes["tss_batterytraction"] = _tss_batterytraction_value; }
                if (_tss_totalamountms) { entity.Attributes["tss_totalamountms"] = _tss_totalamountms_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
