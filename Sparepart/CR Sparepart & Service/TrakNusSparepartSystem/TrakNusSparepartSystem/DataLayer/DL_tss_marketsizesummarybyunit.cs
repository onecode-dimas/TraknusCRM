using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.DataLayer
{
    public class DL_tss_marketsizesummarybyunit
    {
        #region Dependencies
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        private DL_businessunit _DL_businessunit = new DL_businessunit();
        private DL_tss_marketsizeresultpss _DL_tss_marketsizeresultpss = new DL_tss_marketsizeresultpss();
        private DL_population _DL_population = new DL_population();
        private DL_tss_groupuiocommodity _DL_tss_groupuiocommodity = new DL_tss_groupuiocommodity();
        #endregion

        #region Properties
        private string _classname = "DL_tss_marketsizesummarybyunit";

        private string _entityname = "tss_marketsizesummarybyunit";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Market Size Summary by Unit";
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

        private bool _tss_serialnumber = false;
        private EntityReference _tss_serialnumber_value;
        public Guid tss_serialnumber
        {
            get { return _tss_serialnumber ? _tss_serialnumber_value.Id : Guid.Empty; }
            set { _tss_serialnumber = true; _tss_serialnumber_value = new EntityReference(_DL_population.EntityName, value); }
        }

        private bool _tss_groupuiocommodityid = false;
        private EntityReference _tss_groupuiocommodityid_value;
        public Guid tss_groupuiocommodityid
        {
            get { return _tss_groupuiocommodityid ? _tss_groupuiocommodityid_value.Id : Guid.Empty; }
            set { _tss_groupuiocommodityid = true; _tss_groupuiocommodityid_value = new EntityReference(_DL_tss_groupuiocommodity.EntityName, value); }
        }

        private bool _tss_totalamountmsmainpart = false;
        private Money _tss_totalamountmsmainpart_value;
        public decimal tss_totalamountmsmainpart
        {
            get { return _tss_totalamountmsmainpart ? _tss_totalamountmsmainpart_value.Value : 0; }
            set { _tss_totalamountmsmainpart = true; _tss_totalamountmsmainpart_value = new Money(value); }
        }

        private bool _tss_totalamountmscommodity = false;
        private Money _tss_totalamountmscommodity_value;
        public decimal tss_totalamountmscommodity
        {
            get { return _tss_totalamountmscommodity ? _tss_totalamountmscommodity_value.Value : 0; }
            set { _tss_totalamountmscommodity = true; _tss_totalamountmscommodity_value = new Money(value); }
        }

        private bool _tss_totalamountms = false;
        private Money _tss_totalamountms_value;
        public decimal tss_totalamountms
        {
            get { return _tss_totalamountms ? _tss_totalamountms_value.Value : 0; }
            set { _tss_totalamountms = true; _tss_totalamountms_value = new Money(value); }
        }
        #endregion
    }
}
