using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Client.Services;

namespace CalculateRTG.DataLayer
{
    public class DL_trs_commercialdetail
    {
        #region Dependencies
        #endregion

        #region Properties
        private string _classname = "DL_serviceappointment";

        private string _entityname = "serviceappointment";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Work Order";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _subject = false;
        private string _subject_value;
        public string subject
        {
            get { return _subject ? _subject_value : null; }
            set { _subject = true; _subject_value = value; }
        }

        private int _statuscode;
        public int StatusCode
        {
            get { return _statuscode; }
            set { _statuscode = value; }
        }

        private bool _actualstart = false;
        private DateTime _actualstart_value;
        public DateTime actualstart
        {
            get { return _actualstart ? _actualstart_value.ToUniversalTime() : DateTime.MinValue; }
            set { _actualstart = true; _actualstart_value = value.ToLocalTime(); }
        }

        private bool _trs_documentdate = false;
        private DateTime _trs_documentdate_value;
        public DateTime trs_documentdate
        {
            get { return _trs_documentdate ? _trs_documentdate_value.ToUniversalTime() : DateTime.MinValue; }
            set { _trs_documentdate = true; _trs_documentdate_value = value.ToLocalTime(); }
        }

        private bool _trs_synctime = false;
        private DateTime _trs_synctime_value;
        public DateTime trs_synctime
        {
            get { return _trs_synctime ? _trs_synctime_value.ToUniversalTime() : DateTime.MinValue; }
            set { _trs_synctime = true; _trs_synctime_value = value.ToLocalTime(); }
        }

        private bool _trs_documentlink = false;
        private Uri _trs_documentlink_value;
        public Uri trs_documentlink
        {
            get { return _trs_documentlink ? _trs_documentlink_value : null; }
            set { _trs_documentlink = true; _trs_documentlink_value = value; }
        }

        private bool _trs_frommobile = false;
        private bool _trs_frommobile_value;
        public bool trs_frommobile
        {
            get { return _trs_frommobile ? _trs_frommobile_value : false; }
            set { _trs_frommobile = true; _trs_frommobile_value = value; }
        }

        private bool _trs_isarrived = false;
        private bool _trs_isarrived_value;
        public bool trs_isarrived
        {
            get { return _trs_isarrived ? _trs_isarrived_value : false; }
            set { _trs_isarrived = true; _trs_isarrived_value = value; }
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
    }
}
