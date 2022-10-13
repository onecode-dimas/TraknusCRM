using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Check_Aging_Overdue_Prospect.DataLayer
{
    class DL_email
    {
        #region Dependencies
        private DL_activityparty _DL_activityparty = new DL_activityparty();
        #endregion

        #region Properties
        private string _classname = "DL_email";

        private string _entityname = "email";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Email";
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

        private bool _from = false;
        private EntityCollection _from_value;
        public EntityCollection from
        {
            get { return _from ? _from_value : null; }
            set { _from = true; _from_value = value; }
        }

        private bool _to = false;
        private EntityCollection _to_value;
        public EntityCollection to
        {
            get { return _to ? _to_value : null; }
            set { _to = true; _to_value = value; }
        }

        private bool _cc = false;
        private EntityCollection _cc_value;
        public EntityCollection cc
        {
            get { return _cc ? _cc_value : null; }
            set { _cc = true; _cc_value = value; }
        }

        private bool _bcc = false;
        private EntityCollection _bcc_value;
        public EntityCollection bcc
        {
            get { return _bcc ? _bcc_value : null; }
            set { _bcc = true; _bcc_value = value; }
        }

        private bool _description = false;
        private string _description_value;
        public string description
        {
            get { return _description ? _description_value : null; }
            set { _description = true; _description_value = value; }
        }

        private bool _prioritycode = false;
        private OptionSetValue _prioritycode_value;
        public int prioritycode
        {
            get { return _prioritycode ? _prioritycode_value.Value : 1; }
            set { _prioritycode = true; _prioritycode_value = new OptionSetValue(value); }
        }

        private bool _trs_autosend = false;
        private bool _trs_autosend_value;
        public bool trs_autosend
        {
            get { return _trs_autosend ? _trs_autosend_value : false; }
            set { _trs_autosend = true; _trs_autosend_value = value; }
        }

        private bool _trs_autosendcategory = false;
        private decimal _trs_autosendcategory_value;
        public decimal trs_autosendcategory
        {
            get { return _trs_autosendcategory ? _trs_autosendcategory_value : 0; }
            set { _trs_autosendcategory = true; _trs_autosendcategory_value = value; }
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

        public void Insert(IOrganizationService organizationService, out Guid id)
        {
            try
            {
               
                if (_subject)
                {
                    Entity entity = new Entity(_entityname);
                    entity.Attributes["subject"] = _subject_value;
                    if (_from) { entity.Attributes["from"] = _from_value; }
                    if (_to) { entity.Attributes["to"] = _to_value; }
                    if (_cc) { entity.Attributes["cc"] = _cc_value; }
                    if (_bcc) { entity.Attributes["bcc"] = _bcc_value; }
                    if (_description) { entity.Attributes["description"] = _description_value; }
                    if (_prioritycode) { entity.Attributes["prioritycode"] = _prioritycode_value; }
                    if (_trs_autosend) { entity.Attributes["trs_autosend"] = _trs_autosend_value; }
                    if (_trs_autosendcategory) { entity.Attributes["trs_autosendcategory"] = _trs_autosendcategory_value; }
                    id = organizationService.Create(entity);
                }
                else
                {
                    throw new Exception("Primary Key is empty.");
                }
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
                if (_subject) { entity.Attributes["subject"] = _subject_value; }
                if (_from) { entity.Attributes["from"] = _from_value; }
                if (_to) { entity.Attributes["to"] = _to_value; }
                if (_cc) { entity.Attributes["cc"] = _cc_value; }
                if (_bcc) { entity.Attributes["bcc"] = _bcc_value; }
                if (_description) { entity.Attributes["description"] = _description_value; }
                if (_prioritycode) { entity.Attributes["prioritycode"] = _prioritycode_value; }
                if (_trs_autosend) { entity.Attributes["trs_autosend"] = _trs_autosend_value; }
                if (_trs_autosendcategory) { entity.Attributes["trs_autosendcategory"] = _trs_autosendcategory_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
