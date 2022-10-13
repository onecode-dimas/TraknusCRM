using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.DataLayer
{
    public class DL_tss_mastermarketsize
    {
        #region Dependencies
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        private DL_account _DL_account = new DL_account();
        private DL_population _DL_population = new DL_population();
        private DL_tss_keyaccount _DL_keyaccount = new DL_tss_keyaccount();
        private DL_tss_groupuiocommodityaccount _DL_tss_groupuiocommodityaccount = new DL_tss_groupuiocommodityaccount();
        #endregion

        #region Properties
        private string _classname = "DL_tss_mastermarketsize";

        private string _entityname = "tss_mastermarketsize";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Master Market Size";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _tss_mastermarketsizeid = false;
        private Guid _tss_mastermarketsizeid_value;
        public Guid tss_mastermarketsizeid
        {
            get { return _tss_mastermarketsizeid ? _tss_mastermarketsizeid_value : Guid.Empty; }
            set { _tss_mastermarketsizeid = true; _tss_mastermarketsizeid_value = value; }
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

        private bool _tss_customer = false;
        private EntityReference _tss_customer_value;
        public Guid tss_customer
        {
            get { return _tss_customer ? _tss_customer_value.Id : Guid.Empty; }
            set { _tss_customer = true; _tss_customer_value = new EntityReference(_DL_account.EntityName, value); }
        }

        private bool _tss_customergroup = false;
        private EntityReference _tss_customergroup_value;
        public Guid tss_customergroup
        {
            get { return _tss_customergroup ? _tss_customergroup_value.Id : Guid.Empty; }
            set { _tss_customergroup = true; _tss_customergroup_value = new EntityReference(_DL_account.EntityName, value); }
        }

        private bool _tss_serialnumber = false;
        private EntityReference _tss_serialnumber_value;
        public Guid tss_serialnumber
        {
            get { return _tss_serialnumber ? _tss_serialnumber_value.Id : Guid.Empty; }
            set { _tss_serialnumber = true; _tss_serialnumber_value = new EntityReference(_DL_population.EntityName, value); }
        }

        private bool _tss_groupuiocommodity = false;
        private EntityReference _tss_groupuiocommodity_value;
        public Guid tss_groupuiocommodity
        {
            get { return _tss_groupuiocommodity ? _tss_groupuiocommodity_value.Id : Guid.Empty; }
            set { _tss_groupuiocommodity = true; _tss_groupuiocommodity_value = new EntityReference(_DL_tss_groupuiocommodityaccount.EntityName, value); }
        }

        private bool _tss_groupuiocommodityheader = false;
        private EntityReference _tss_groupuiocommodityheader_value;
        public Guid tss_groupuiocommodityheader
        {
            get { return _tss_groupuiocommodityheader ? _tss_groupuiocommodityheader_value.Id : Guid.Empty; }
            set { _tss_groupuiocommodityheader = true; _tss_groupuiocommodityheader_value = new EntityReference("tss_groupuiocommodityheader", value); }
        }

        private bool _tss_keyaccountid = false;
        private EntityReference _tss_keyaccountid_value;
        public Guid tss_keyaccountid
        {
            get { return _tss_keyaccountid ? _tss_keyaccountid_value.Id : Guid.Empty; }
            set { _tss_keyaccountid = true; _tss_keyaccountid_value = new EntityReference(_DL_keyaccount.EntityName, value); }
        }

        private bool _tss_totalhmconsump = false;
        private int _tss_totalhmconsump_value;
        public int tss_totalhmconsump
        {
            get { return _tss_totalhmconsump ? _tss_totalhmconsump_value : int.MinValue; }
            set { _tss_totalhmconsump = true; _tss_totalhmconsump_value = value; }
        }

        private bool _tss_msperiod = false;
        private int _tss_msperiod_value;
        public int tss_msperiod
        {
            get { return _tss_msperiod ? _tss_msperiod_value : int.MinValue; }
            set { _tss_msperiod = true; _tss_msperiod_value = value; }
        }

        public bool _tss_avghmmethod1 = false;
        private decimal _tss_avghmmethod1_value;
        public decimal tss_avghmmethod1
        {
            get { return _tss_avghmmethod1 ? _tss_avghmmethod1_value : 0; }
            set { _tss_avghmmethod1 = true; _tss_avghmmethod1_value = Math.Round(value, 2); }
        }

        public bool _tss_avghmmethod2 = false;
        private decimal _tss_avghmmethod2_value;
        public decimal tss_avghmmethod2
        {
            get { return _tss_avghmmethod2 ? _tss_avghmmethod2_value : 0; }
            set { _tss_avghmmethod2 = true; _tss_avghmmethod2_value = Math.Round(value, 2); }
        }

        public bool _tss_avghmmethod3 = false;
        private decimal _tss_avghmmethod3_value;
        public decimal tss_avghmmethod3
        {
            get { return _tss_avghmmethod3 ? _tss_avghmmethod3_value : 0; }
            set { _tss_avghmmethod3 = true; _tss_avghmmethod3_value = Math.Round(value, 2); }
        }

        public bool _tss_periodpmmethod4 = false;
        private decimal _tss_periodpmmethod4_value;
        public decimal tss_periodpmmethod4
        {
            get { return _tss_periodpmmethod4 ? _tss_periodpmmethod4_value : 0; }
            set { _tss_periodpmmethod4 = true; _tss_periodpmmethod4_value = Math.Round(value, 2); }
        }

        public bool _tss_periodpmmethod5 = false;
        private decimal _tss_periodpmmethod5_value;
        public decimal tss_periodpmmethod5
        {
            get { return _tss_periodpmmethod5 ? _tss_periodpmmethod5_value : 0; }
            set { _tss_periodpmmethod5 = true; _tss_periodpmmethod5_value = Math.Round(value, 2); }
        }

        //Option Set
        private bool _tss_unittype = false;
        private int _tss_unittype_value;
        public int tss_unittype
        {
            get { return _tss_unittype ? _tss_unittype_value : int.MinValue; }
            set { _tss_unittype = true; _tss_unittype_value = value; }
        }

        private bool _tss_qty = false;
        private int _tss_qty_value;
        public int tss_qty
        {
            get { return _tss_qty ? _tss_qty_value : int.MinValue; }
            set { _tss_qty = true; _tss_qty_value = value; }
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

        private bool _tss_errormessage = false;
        private string _tss_errormessage_value;
        public string tss_errormessage
        {
            get { return _tss_errormessage ? _tss_errormessage_value : null; }
            set { _tss_errormessage = true; _tss_errormessage_value = value; }
        }

        //Two Option
        private bool _tss_ismsresultpssgenerated = false;
        private bool _tss_ismsresultpssgenerated_value;
        public bool tss_ismsresultpssgenerated
        {
            get { return _tss_ismsresultpssgenerated ? _tss_ismsresultpssgenerated_value : false; }
            set { _tss_ismsresultpssgenerated = true; _tss_ismsresultpssgenerated_value = value; }
        }

        private bool _tss_issublinesgenerated = false;
        private bool _tss_issublinesgenerated_value;
        public bool tss_issublinesgenerated
        {
            get { return _tss_issublinesgenerated ? _tss_issublinesgenerated_value : false; }
            set { _tss_issublinesgenerated = true; _tss_issublinesgenerated_value = value; }
        }

        private bool _tss_usetyre = false;
        private bool _tss_usetyre_value;
        public bool tss_usetyre
        {
            get { return _tss_usetyre ? _tss_usetyre_value : false; }
            set { _tss_usetyre = true; _tss_usetyre_value = value; }
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
                if (_tss_pss) { entity.Attributes["tss_pss"] = _tss_pss_value; }
                if (_tss_msperiodstart) { entity.Attributes["tss_msperiodstart"] = _tss_msperiodstart_value; }
                if (_tss_msperiodend) { entity.Attributes["tss_msperiodend"] = _tss_msperiodend_value; }
                if (_tss_activeperiodstart) { entity.Attributes["tss_activeperiodstart"] = _tss_activeperiodstart_value; }
                if (_tss_activeperiodsend) { entity.Attributes["tss_activeperiodsend"] = _tss_activeperiodsend_value; }
                if (_tss_customer) { entity.Attributes["tss_customer"] = _tss_customer_value; }
                if (_tss_customergroup) { entity.Attributes["tss_customergroup"] = _tss_customergroup_value; }
                if (_tss_serialnumber) { entity.Attributes["tss_serialnumber"] = _tss_serialnumber_value; }
                if (_tss_groupuiocommodity) { entity.Attributes["tss_groupuiocommodity"] = _tss_groupuiocommodity_value; }
                if (_tss_groupuiocommodityheader) { entity.Attributes["tss_groupuiocommodityheader"] = _tss_groupuiocommodityheader_value; }
                if (_tss_totalhmconsump) { entity.Attributes["tss_totalhmconsump"] = _tss_totalhmconsump_value; }
                if (_tss_avghmmethod1) { entity.Attributes["tss_avghmmethod1"] = _tss_avghmmethod1_value; }
                if (_tss_avghmmethod2) { entity.Attributes["tss_avghmmethod2"] = _tss_avghmmethod2_value; }
                if (_tss_avghmmethod3) { entity.Attributes["tss_avghmmethod3"] = _tss_avghmmethod3_value; }
                if (_tss_periodpmmethod4) { entity.Attributes["tss_periodpmmethod4"] = _tss_periodpmmethod4_value; }
                if (_tss_periodpmmethod5) { entity.Attributes["tss_periodpmmethod5"] = _tss_periodpmmethod5_value; }
                if (_tss_unittype) { entity.Attributes["tss_unittype"] = new OptionSetValue(_tss_unittype_value); }
                if (_tss_qty) { entity.Attributes["tss_qty"] = _tss_qty_value; }
                if (_tss_status) { entity.Attributes["tss_status"] = new OptionSetValue(_tss_status_value); }
                if (_tss_statusreason) { entity.Attributes["tss_statusreason"] = new OptionSetValue(_tss_statusreason_value); }
                if (_tss_errormessage) { entity.Attributes["tss_errormessage"] = _tss_errormessage_value; }
                if (_tss_usetyre) { entity.Attributes["tss_usetyre"] = _tss_usetyre_value; }
                if (_tss_keyaccountid) { entity.Attributes["tss_keyaccountid"] = _tss_keyaccountid_value; }
                if (_tss_msperiod) { entity.Attributes["tss_msperiod"] = _tss_msperiod_value; }
                if (_tss_ismsresultpssgenerated) { entity.Attributes["tss_ismsresultpssgenerated"] = _tss_ismsresultpssgenerated_value; }
                if (_tss_issublinesgenerated) { entity.Attributes["tss_issublinesgenerated"] = _tss_issublinesgenerated_value; }

                return tss_mastermarketsizeid = organizationService.Create(entity);
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
                if (_tss_mastermarketsizeid) { entity.Attributes["tss_mastermarketsizeid"] = _tss_mastermarketsizeid_value; }
                if (_tss_pss) { entity.Attributes["tss_pss"] = _tss_pss_value; }
                if (_tss_msperiodstart) { entity.Attributes["tss_msperiodstart"] = _tss_msperiodstart_value; }
                if (_tss_msperiodend) { entity.Attributes["tss_msperiodend"] = _tss_msperiodend_value; }
                if (_tss_activeperiodstart) { entity.Attributes["tss_activeperiodstart"] = _tss_activeperiodstart_value; }
                if (_tss_activeperiodsend) { entity.Attributes["tss_activeperiodend"] = _tss_activeperiodsend_value; }
                if (_tss_customer) { entity.Attributes["tss_customer"] = _tss_customer_value; }
                if (_tss_customergroup) { entity.Attributes["tss_customergroup"] = _tss_customergroup_value; }
                if (_tss_serialnumber) { entity.Attributes["tss_serialnumber"] = _tss_serialnumber_value; }
                if (_tss_groupuiocommodity) { entity.Attributes["tss_groupuiocommodity"] = _tss_groupuiocommodity_value; }
                if (_tss_groupuiocommodityheader) { entity.Attributes["tss_groupuiocommodityheader"] = _tss_groupuiocommodityheader_value; }
                if (_tss_totalhmconsump) { entity.Attributes["tss_totalhmconsump"] = _tss_totalhmconsump_value; }
                if (_tss_avghmmethod1) { entity.Attributes["tss_avghmmethod1"] = _tss_avghmmethod1_value; }
                if (_tss_avghmmethod2) { entity.Attributes["tss_avghmmethod2"] = _tss_avghmmethod2_value; }
                if (_tss_avghmmethod3) { entity.Attributes["tss_avghmmethod3"] = _tss_avghmmethod3_value; }
                if (_tss_periodpmmethod4) { entity.Attributes["tss_periodpmmethod4"] = _tss_periodpmmethod4_value; }
                if (_tss_periodpmmethod5) { entity.Attributes["tss_periodpmmethod5"] = _tss_periodpmmethod5_value; }
                if (_tss_unittype) { entity.Attributes["tss_unittype"] = new OptionSetValue(_tss_unittype_value); }
                if (_tss_qty) { entity.Attributes["tss_qty"] = _tss_qty_value; }
                if (_tss_status) { entity.Attributes["tss_status"] = new OptionSetValue(_tss_status_value); }
                if (_tss_statusreason) { entity.Attributes["tss_statusreason"] = new OptionSetValue(_tss_statusreason_value); }
                if (_tss_errormessage) { entity.Attributes["tss_errormessage"] = _tss_errormessage_value; }
                if (_tss_usetyre) { entity.Attributes["tss_usetyre"] = _tss_usetyre_value; }
                if (_tss_keyaccountid) { entity.Attributes["tss_keyaccountid"] = _tss_keyaccountid_value; }
                if (_tss_msperiod) { entity.Attributes["tss_msperiod"] = _tss_msperiod_value; }
                if (_tss_ismsresultpssgenerated) { entity.Attributes["tss_ismsresultpssgenerated"] = _tss_ismsresultpssgenerated_value; }
                if (_tss_issublinesgenerated) { entity.Attributes["tss_issublinesgenerated"] = _tss_issublinesgenerated_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }


        public Entity GetEntity(IOrganizationService organizationService)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                if (_tss_pss) { entity.Attributes["tss_pss"] = _tss_pss_value; }
                if (_tss_msperiodstart) { entity.Attributes["tss_msperiodstart"] = _tss_msperiodstart_value; }
                if (_tss_msperiodend) { entity.Attributes["tss_msperiodend"] = _tss_msperiodend_value; }
                if (_tss_activeperiodstart) { entity.Attributes["tss_activeperiodstart"] = _tss_activeperiodstart_value; }
                if (_tss_activeperiodsend) { entity.Attributes["tss_activeperiodsend"] = _tss_activeperiodsend_value; }
                if (_tss_customer) { entity.Attributes["tss_customer"] = _tss_customer_value; }
                if (_tss_customergroup) { entity.Attributes["tss_customergroup"] = _tss_customergroup_value; }
                if (_tss_serialnumber) { entity.Attributes["tss_serialnumber"] = _tss_serialnumber_value; }
                if (_tss_groupuiocommodity) { entity.Attributes["tss_groupuiocommodity"] = _tss_groupuiocommodity_value; }
                if (_tss_groupuiocommodityheader) { entity.Attributes["tss_groupuiocommodityheader"] = _tss_groupuiocommodityheader_value; }
                if (_tss_totalhmconsump) { entity.Attributes["tss_totalhmconsump"] = _tss_totalhmconsump_value; }
                if (_tss_avghmmethod1) { entity.Attributes["tss_avghmmethod1"] = _tss_avghmmethod1_value; }
                if (_tss_avghmmethod2) { entity.Attributes["tss_avghmmethod2"] = _tss_avghmmethod2_value; }
                if (_tss_avghmmethod3) { entity.Attributes["tss_avghmmethod3"] = _tss_avghmmethod3_value; }
                if (_tss_periodpmmethod4) { entity.Attributes["tss_periodpmmethod4"] = _tss_periodpmmethod4_value; }
                if (_tss_periodpmmethod5) { entity.Attributes["tss_periodpmmethod5"] = _tss_periodpmmethod5_value; }
                if (_tss_unittype) { entity.Attributes["tss_unittype"] = new OptionSetValue(_tss_unittype_value); }
                if (_tss_qty) { entity.Attributes["tss_qty"] = _tss_qty_value; }
                if (_tss_status) { entity.Attributes["tss_status"] = new OptionSetValue(_tss_status_value); }
                if (_tss_statusreason) { entity.Attributes["tss_statusreason"] = new OptionSetValue(_tss_statusreason_value); }
                if (_tss_errormessage) { entity.Attributes["tss_errormessage"] = _tss_errormessage_value; }
                if (_tss_usetyre) { entity.Attributes["tss_usetyre"] = _tss_usetyre_value; }
                if (_tss_keyaccountid) { entity.Attributes["tss_keyaccountid"] = _tss_keyaccountid_value; }
                if (_tss_msperiod) { entity.Attributes["tss_msperiod"] = _tss_msperiod_value; }
                if (_tss_ismsresultpssgenerated) { entity.Attributes["tss_ismsresultpssgenerated"] = _tss_ismsresultpssgenerated_value; }
                if (_tss_issublinesgenerated) { entity.Attributes["tss_issublinesgenerated"] = _tss_issublinesgenerated_value; }

                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".GetEntity : " + ex.Message.ToString());
            }
        }
    }
}
