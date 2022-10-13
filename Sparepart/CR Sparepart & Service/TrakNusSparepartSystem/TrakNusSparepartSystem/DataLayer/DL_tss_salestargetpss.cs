using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusSparepartSystem.DataLayer
{
    public class DL_tss_salestargetpss
    {
        #region Properties

        private string _classname = "DL_tss_salestargetpss";

        private string _entityname = "tss_salestargetpss";
        public string EntityName
        {
            get { return _entityname; }
        }

        private bool _name = false;
        private string _name_value;
        public string name
        {
            get { return _name ? _name_value : null; }
            set { _name = true; _name_value = value; }
        }

        private bool _tss_activeenddate = false;
        private DateTime _tss_activeenddate_value;
        public DateTime tss_activeenddate
        {
            get { return _tss_activeenddate ? _tss_activeenddate_value.ToLocalTime() : DateTime.MinValue; }
            set { _tss_activeenddate = true; _tss_activeenddate_value = value; }
        }

        private bool _tss_activestartdate = false;
        private DateTime _tss_activestartdate_value;
        public DateTime tss_activestartdate
        {
            get { return _tss_activestartdate ? _tss_activestartdate_value.ToLocalTime() : DateTime.MinValue; }
            set { _tss_activeenddate = true; _tss_activestartdate_value = value; }
        }

        private bool _tss_april = false;
        private decimal _tss_april_value;
        public decimal tss_april
        {
            get { return _tss_april ? _tss_april_value : int.MinValue; }
            set { _name = true; _tss_april_value = value; }
        }

        private bool _tss_august = false;
        private decimal _tss_august_value;
        public decimal tss_august
        {
            get { return _tss_august ? _tss_august_value : int.MinValue; }
            set { _tss_august = true; _tss_august_value = value; }
        }

        private bool _tss_branch = false;
        private Guid _tss_branch_value;

        public Guid tss_branch
        {
            get { return _tss_branch_value; }
            set { _tss_branch = true; _tss_branch_value = value; }
        }


        private bool _tss_december = false;
        private decimal _tss_december_value;
        public decimal tss_december
        {
            get { return _tss_december ? _tss_december_value : int.MinValue; }
            set { _tss_december = true; _tss_december_value = value; }
        }

        private bool _tss_february = false;
        private decimal _tss_february_value;
        public decimal tss_february
        {
            get { return _tss_february ? _tss_february_value : int.MinValue; }
            set { _tss_february = true; _tss_february_value = value; }
        }

        private bool _tss_january = false;
        private decimal _tss_january_value;
        public decimal tss_january
        {
            get { return _tss_january ? _tss_january_value : int.MinValue; }
            set { _tss_january = true; _tss_january_value = value; }
        }

        private bool _tss_july = false;
        private decimal _tss_july_value;
        public decimal tss_july
        {
            get { return _tss_july ? _tss_july_value : int.MinValue; }
            set { _tss_july = true; _tss_july_value = value; }
        }

        private bool _tss_june = false;
        private decimal _tss_june_value;
        public decimal tss_june
        {
            get { return _tss_june ? _tss_june_value : int.MinValue; }
            set { _tss_june = true; _tss_june_value = value; }
        }

        private bool _tss_march = false;
        private decimal _tss_march_value;
        public decimal tss_march
        {
            get { return _tss_march ? _tss_march_value : int.MinValue; }
            set { _tss_march = true; _tss_march_value = value; }
        }

        //SetOption
        private bool _tss_marketsizeid = false;
        private Guid _tss_marketsizeid_value;
        public Guid tss_marketsizeid
        {
            get { return _tss_marketsizeid ? _tss_marketsizeid_value : Guid.Empty; }
            set { _tss_marketsizeid = true; _tss_marketsizeid_value = value; }
        }
        private bool _tss_may = false;
        private decimal _tss_may_value;
        public decimal tss_may
        {
            get { return _tss_may ? _tss_may_value : int.MinValue; }
            set { _tss_may = true; _tss_may_value = value; }
        }


        private bool _tss_msenddate = false;
        private DateTime _tss_msenddate_value;
        public DateTime tss_msenddate
        {
            get { return _tss_msenddate ? _tss_msenddate_value.ToLocalTime() : DateTime.MinValue; }
            set { _tss_msenddate = true; _tss_msenddate_value = value; }
        }
        private bool _tss_msperiodstart = false;
        private DateTime _tss_msperiodstart_value;
        public DateTime tss_msperiodstart
        {
            get { return _tss_msperiodstart ? _tss_msperiodstart_value.ToLocalTime() : DateTime.MinValue; }
            set { _tss_msperiodstart = true; _tss_msperiodstart_value = value; }
        }

        private bool _tss_name = false;
        private string _tss_name_value;
        public string tss_name
        {
            get { return _tss_name ? _tss_name_value : null; }
            set { _tss_name = true; _tss_name_value = value; }
        }

        private bool _tss_november = false;
        private decimal _tss_november_value;
        public decimal tss_november
        {
            get { return _tss_november ? _tss_november_value : int.MinValue; }
            set { _tss_november = true; _tss_november_value = value; }
        }

        private bool _tss_october = false;
        private decimal _tss_october_value;
        public decimal tss_october
        {
            get { return _tss_october ? _tss_october_value : int.MinValue; }
            set { _tss_october = true; _tss_october_value = value; }
        }

        private bool _tss_pss = false;
        private Guid _tss_pss_value;

        public Guid tss_pss
        {
            get { return _tss_pss ? _tss_pss_value : Guid.Empty; }
            set { _tss_pss = true; _tss_pss_value = value; }
        }

        private bool _tss_salestargetpssid = false;
        private Guid _tss_salestargetpssid_value;
        public Guid tss_salestargetpssid
        {
            get { return _tss_salestargetpssid ? _tss_salestargetpssid_value : Guid.Empty; }
            set { _tss_salestargetpssid = true; _tss_salestargetpssid_value = value; }
        }

        private bool _tss_september = false;
        private decimal _tss_september_value;
        public decimal tss_september
        {
            get { return _tss_september ? _tss_september_value : int.MinValue; }
            set { _tss_september = true; _tss_september_value = value; }
        }
        //SetOption
        private bool _tss_status = false;
        private int _tss_status_value;
        public int tss_status
        {
            get { return _tss_status ? _tss_status_value : int.MinValue; }
            set { _tss_status = true; _tss_status_value = value; }
        }

        //SetOption
        private bool _tss_statusreason = false;
        private int _tss_statusreason_value;
        public int tss_statusreason
        {
            get { return _tss_statusreason ? _tss_statusreason_value : int.MinValue; }
            set { _tss_statusreason = true; _tss_statusreason_value = value; }
        }

        private bool _tss_totalallsalesyearly = false;
        private decimal _tss_totalallsalesyearly_value;
        public decimal tss_totalallsalesyearly
        {
            get { return _tss_totalallsalesyearly ? _tss_totalallsalesyearly_value : int.MinValue; }
            set { _tss_totalallsalesyearly = true; _tss_totalallsalesyearly_value = value; }
        }

        private bool _tss_totalamountmarketsizeyearly = false;
        private decimal _tss_totalamountmarketsizeyearly_value;
        public decimal tss_totalamountmarketsizeyearly
        {
            get { return _tss_totalamountmarketsizeyearly ? _tss_totalamountmarketsizeyearly_value : 0m; }
            set { _tss_totalamountmarketsizeyearly = true; _tss_totalamountmarketsizeyearly_value = value; }
        }

        private bool _tss_totalpctgmarketsizeyearly = false;
        private decimal _tss_totalpctgmarketsizeyearly_value;
        public decimal tss_totalpctgmarketsizeyearly
        {
            get { return _tss_totalpctgmarketsizeyearly ? _tss_totalpctgmarketsizeyearly_value : int.MinValue; }
            set { _tss_totalpctgmarketsizeyearly = true; _tss_totalpctgmarketsizeyearly_value = value; }
        }

        private bool _tss_totalyearly = false;
        private decimal _tss_totalyearly_value=0m;
        public decimal tss_totalyearly
        {
            get { return _tss_totalyearly ? _tss_totalyearly_value : 0m; }
            set { _tss_totalyearly = true; _tss_totalyearly_value = value; }
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
                return organizationService.RetrieveMultiple(queryExpression);
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
                if (_tss_activeenddate) { entity.Attributes["tss_activeenddate"] = _tss_activeenddate_value; }
                if (_tss_activestartdate) { entity.Attributes["tss_activestartdate"] = _tss_activestartdate_value; }
                if (_tss_april) { entity.Attributes["tss_april"] = _tss_april_value; }
                if (_tss_august) { entity.Attributes["tss_august"] = _tss_august_value; }
                if (_tss_branch) { entity.Attributes["tss_branch"] = _tss_branch_value; }
                if (_tss_december) { entity.Attributes["tss_december"] = _tss_december_value; }
                if (_tss_february) { entity.Attributes["tss_february"] = _tss_february_value; }
                if (_tss_january) { entity.Attributes["tss_january"] = _tss_january_value; }
                if (_tss_july) { entity.Attributes["tss_july"] = _tss_july_value; }
                if (_tss_june) { entity.Attributes["tss_june"] = _tss_june_value; }
                if (_tss_march) { entity.Attributes["tss_march"] = _tss_march_value; }
                if (_tss_marketsizeid) { entity.Attributes["tss_marketsizeid"] = _tss_marketsizeid_value; }
                if (_tss_may) { entity.Attributes["tss_may"] = _tss_may_value; }
                if (_tss_msenddate) { entity.Attributes["tss_msenddate"] = _tss_msenddate_value; }
                if (_tss_msperiodstart) { entity.Attributes["tss_msperiodstart"] = _tss_msperiodstart_value; }
                if (_tss_name) { entity.Attributes["tss_name"] = _tss_name_value; }
                if (_tss_november) { entity.Attributes["tss_november"] = _tss_november_value; }
                if (_tss_october) { entity.Attributes["tss_october"] = _tss_october_value; }
                if (_tss_pss) { entity.Attributes["tss_pss"] = _tss_pss_value; }
                if (_tss_salestargetpssid) { entity.Attributes["tss_salestargetpssid"] = _tss_salestargetpssid_value; }
                if (_tss_september) { entity.Attributes["tss_september"] = _tss_september_value; }
                if (_tss_status) { entity.Attributes["tss_status"] = new OptionSetValue(_tss_status_value); }
                if (_tss_statusreason) { entity.Attributes["tss_statusreason"] = new OptionSetValue(_tss_statusreason_value); }
                if (_tss_totalallsalesyearly) { entity.Attributes["tss_totalallsalesyearly"] = _tss_totalallsalesyearly_value; }
                if (_tss_totalamountmarketsizeyearly) { entity.Attributes["tss_totalamountmarketsizeyearly"] = _tss_totalamountmarketsizeyearly_value; }
                if (_tss_totalpctgmarketsizeyearly) { entity.Attributes["tss_totalpctgmarketsizeyearly"] = _tss_totalpctgmarketsizeyearly_value; }
                if (_tss_totalyearly) { entity.Attributes["tss_totalyearly"] = _tss_totalyearly_value; }
                return organizationService.Create(entity);

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
                if (_name) { entity.Attributes["name"] = _name_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
