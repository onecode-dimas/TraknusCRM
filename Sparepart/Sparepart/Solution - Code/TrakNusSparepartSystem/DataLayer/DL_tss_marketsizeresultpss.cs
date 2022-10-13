using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.DataLayer
{
    public class DL_tss_marketsizeresultpss
    {
        #region Dependencies
        DL_businessunit _DL_businessunit = new DL_businessunit();
        DL_systemuser _DL_systemuser = new DL_systemuser();
        DL_account _DL_account = new DL_account();
        #endregion
        #region Properties
        private string _classname = "DL_tss_marketsizeresultpss";

        private string _entityname = "tss_marketsizeresultpss";

        public string EntityName
        {
            get { return _entityname; }
        }

        private bool _tss_marketsizeid = false;
        private string _tss_marketsizeid_value;
        public string tss_marketsizeid
        {
            get { return _tss_marketsizeid ? _tss_marketsizeid_value : null; }
            set { _tss_marketsizeid = true; _tss_marketsizeid_value = value; }
        }

        private bool _tss_branch = false;
        private EntityReference _tss_branch_value;
        public Guid tss_branch
        {
            get { return _tss_branch ? _tss_branch_value.Id : Guid.Empty; }
            set { _tss_branch = true; _tss_branch_value = new EntityReference(_DL_businessunit.EntityName, value); }
        }

        private bool _tss_pss = false;
        private EntityReference _tss_pss_value;
        public Guid tss_pss
        {
            get { return _tss_pss ? _tss_pss_value.Id : Guid.Empty; }
            set { _tss_pss = true; _tss_pss_value = new EntityReference(_DL_systemuser.EntityName, value); }
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

        private bool _tss_activeperiodsend = false;
        private DateTime _tss_activeperiodsend_value;
        public DateTime tss_activeperiodsend
        {
            get { return _tss_activeperiodsend ? _tss_activeperiodsend_value.ToLocalTime() : DateTime.MinValue; }
            set { _tss_activeperiodsend = true; _tss_activeperiodsend_value = value.ToLocalTime(); }
        }

        private bool _tss_approvedate = false;
        private DateTime _tss_approvedate_value;
        public DateTime tss_approvedate
        {
            get { return _tss_approvedate ? _tss_approvedate_value.ToLocalTime() : DateTime.MinValue; }
            set { _tss_approvedate = true; _tss_approvedate_value = value.ToLocalTime(); }
        }

        private bool _tss_confirmdate = false;
        private DateTime _tss_confirmdate_value;
        public DateTime tss_confirmdate
        {
            get { return _tss_confirmdate ? _tss_confirmdate_value.ToLocalTime() : DateTime.MinValue; }
            set { _tss_confirmdate = true; _tss_confirmdate_value = value.ToLocalTime(); }
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
                if (_tss_marketsizeid) { entity.Attributes["tss_marketsizeid"] = _tss_marketsizeid_value; }
                if (_tss_pss) { entity.Attributes["tss_pss"] = _tss_pss_value; }
                if (_tss_branch) { entity.Attributes["tss_branch"] = _tss_branch_value; }
                if (_tss_customer) { entity.Attributes["tss_customer"] = _tss_customer_value; }
                if (_tss_customergroup) { entity.Attributes["tss_customergroup"] = _tss_customergroup_value; }
                if (_tss_totaluio) { entity.Attributes["tss_totaluio"] = _tss_totaluio_value; }
                if (_tss_totalnonuio) { entity.Attributes["tss_totalnonuio"] = _tss_totalnonuio_value; }
                if (_tss_totalgroupuiocommodity) { entity.Attributes["tss_totalgroupuiocommodity"] = _tss_totalgroupuiocommodity_value; }
                if (_tss_totalalluio) { entity.Attributes["tss_totalalluio"] = _tss_totalalluio_value; }
                if (_tss_totalmscommodity) { entity.Attributes["tss_totalmscommodity"] = _tss_totalmscommodity_value; }
                if (_tss_totalmsstandardpart) { entity.Attributes["tss_totalmsstandardpart"] = _tss_totalmsstandardpart_value; }
                if (_tss_totalamountms) { entity.Attributes["tss_totalamountms"] = _tss_totalamountms_value; }
                if (_tss_msperiodstart) { entity.Attributes["tss_msperiodstart"] = _tss_msperiodstart_value; }
                if (_tss_msperiodend) { entity.Attributes["tss_msperiodend"] = _tss_msperiodend_value; }
                if (_tss_activeperiodstart) { entity.Attributes["tss_activeperiodstart"] = _tss_activeperiodstart_value; }
                if (_tss_activeperiodsend) { entity.Attributes["tss_activeperiodsend"] = _tss_activeperiodsend_value; }
                if (_tss_status) { entity.Attributes["tss_status"] = new OptionSetValue(_tss_status_value); }
                if (_tss_statusreason) { entity.Attributes["tss_statusreason"] = new OptionSetValue(_tss_statusreason_value); }
                if (_tss_approvedate) { entity.Attributes["tss_approvedate"] = _tss_approvedate_value; }
                if (_tss_confirmdate) { entity.Attributes["tss_confirmdate"] = _tss_confirmdate_value; }
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
                if (_tss_marketsizeid) { entity.Attributes["tss_marketsizeid"] = _tss_marketsizeid_value; }
                if (_tss_pss) { entity.Attributes["tss_pss"] = _tss_pss_value; }
                if (_tss_branch) { entity.Attributes["tss_branch"] = _tss_branch_value; }
                if (_tss_customer) { entity.Attributes["tss_customer"] = _tss_customer_value; }
                if (_tss_customergroup) { entity.Attributes["tss_customergroup"] = _tss_customergroup_value; }
                if (_tss_totaluio) { entity.Attributes["tss_totaluio"] = _tss_totaluio_value; }
                if (_tss_totalnonuio) { entity.Attributes["tss_totalnonuio"] = _tss_totalnonuio_value; }
                if (_tss_totalgroupuiocommodity) { entity.Attributes["tss_totalgroupuiocommodity"] = _tss_totalgroupuiocommodity_value; }
                if (_tss_totalalluio) { entity.Attributes["tss_totalalluio"] = _tss_totalalluio_value; }
                if (_tss_totalmscommodity) { entity.Attributes["tss_totalmscommodity"] = _tss_totalmscommodity_value; }
                if (_tss_totalmsstandardpart) { entity.Attributes["tss_totalmsstandardpart"] = _tss_totalmsstandardpart_value; }
                if (_tss_totalamountms) { entity.Attributes["tss_totalamountms"] = _tss_totalamountms_value; }
                if (_tss_msperiodstart) { entity.Attributes["tss_msperiodstart"] = _tss_msperiodstart_value; }
                if (_tss_msperiodend) { entity.Attributes["tss_msperiodend"] = _tss_msperiodend_value; }
                if (_tss_activeperiodstart) { entity.Attributes["tss_activeperiodstart"] = _tss_activeperiodstart_value; }
                if (_tss_activeperiodsend) { entity.Attributes["tss_activeperiodsend"] = _tss_activeperiodsend_value; }
                if (_tss_status) { entity.Attributes["tss_status"] = new OptionSetValue(_tss_status_value); }
                if (_tss_statusreason) { entity.Attributes["tss_statusreason"] = new OptionSetValue(_tss_statusreason_value); }
                if (_tss_approvedate) { entity.Attributes["tss_approvedate"] = _tss_approvedate_value; }
                if (_tss_confirmdate) { entity.Attributes["tss_confirmdate"] = _tss_confirmdate_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
