using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.DataLayer
{
    public class DL_tss_keyaccount
    {
        #region constan
        private const int REVISION = 865920001;
        #endregion

        #region Dependencies
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        private DL_account _DL_account = new DL_account();
        private DL_trs_functionallocation _DL_trs_functionallocation = new DL_trs_functionallocation();
        #endregion

        #region Properties
        private string _classname = "DL_tss_keyaccount";

        private string _entityname = "tss_keyaccount";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Key Account";
        public string DisplayName
        {
            get { return _displayname; }
        }


        private bool _tss_kamsid = false;
        private string _tss_kamsid_value;
        public string tss_kamsid
        {
            get { return _tss_kamsid ? _tss_kamsid_value : null; }
            set { _tss_kamsid = true; _tss_kamsid_value = value; }
        }

        //Option Set
        private bool _tss_version = false;
        private int _tss_version_value;
        public int tss_version
        {
            get { return _tss_version ? _tss_version_value : int.MinValue; }
            set { _tss_version = true; _tss_version_value = value; }
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

        private bool _tss_activestartdate = false;
        private DateTime _tss_activestartdate_value;
        public DateTime tss_activestartdate
        {
            get { return _tss_activestartdate ? _tss_activestartdate_value.ToLocalTime() : DateTime.MinValue; }
            set { _tss_activestartdate = true; _tss_activestartdate_value = value.ToLocalTime(); }
        }

        private bool _tss_activeenddate = false;
        private DateTime _tss_activeenddate_value;
        public DateTime tss_activeenddate
        {
            get { return _tss_activeenddate ? _tss_activeenddate_value.ToLocalTime() : DateTime.MinValue; }
            set { _tss_activeenddate = true; _tss_activeenddate_value = value.ToLocalTime(); }
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

        private bool _tss_funclock = false;
        private EntityReference _tss_funclock_value;
        public Guid tss_funclock
        {
            get { return _tss_funclock ? _tss_funclock_value.Id : Guid.Empty; }
            set { _tss_funclock = true; _tss_funclock_value = new EntityReference(_DL_trs_functionallocation.EntityName, value); }
        }

        //Two Option
        private bool _tss_calculatetoms = false;
        private bool _tss_calculatetoms_value;
        public bool tss_calculatetoms
        {
            get { return _tss_calculatetoms ? _tss_calculatetoms_value : false; }
            set { _tss_calculatetoms = true; _tss_calculatetoms_value = value; }
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
        private bool _tss_reason = false;
        private int _tss_reason_value;
        public int tss_reason
        {
            get { return _tss_reason ? _tss_reason_value : int.MinValue; }
            set { _tss_reason = true; _tss_reason_value = value; }
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

        public void UpdateStatus(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                entity.Id = id;
                if (_tss_status) { entity.Attributes["tss_status"] = new OptionSetValue(_tss_status_value); }
                if (_tss_reason) { entity.Attributes["tss_reason"] = new OptionSetValue(_tss_reason_value); }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }

        public Tuple<DateTime, DateTime> CheckReviseStatus(Entity keyAccount, Entity generalSetup, ref DateTime startDtMS, ref DateTime endDtMS)
        {

            if (keyAccount.GetAttributeValue<OptionSetValue>("tss_version").Value == REVISION)
            {
                startDtMS = new DateTime(generalSetup.GetAttributeValue<DateTime>("tss_startdatemarketsize").Year, generalSetup.GetAttributeValue<DateTime>("tss_startdatemarketsize").Month, 1).AddMonths(generalSetup.GetAttributeValue<int>("tss_evaluationmarketsize"));
                endDtMS = generalSetup.GetAttributeValue<DateTime>("tss_enddatems");
            }
            else
            {
                startDtMS = generalSetup.GetAttributeValue<DateTime>("tss_startdatemarketsize");
                endDtMS = generalSetup.GetAttributeValue<DateTime>("tss_startdatemarketsize");
            }
            return new Tuple<DateTime, DateTime>(startDtMS, endDtMS);
        }

        public Tuple<DateTime, DateTime> StatusRevise(Entity _keyaccount, ref DateTime _startdatemarketsize, ref DateTime _enddatemarketsize, int _evaluationmarketsize)
        {
            if (_keyaccount.GetAttributeValue<OptionSetValue>("tss_version").Value == REVISION)
                _startdatemarketsize = new DateTime(_startdatemarketsize.Year, _startdatemarketsize.Month, 1).AddMonths(_evaluationmarketsize);

            return new Tuple<DateTime, DateTime>(_startdatemarketsize, _enddatemarketsize);
        }
    }
}
