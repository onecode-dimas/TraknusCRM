using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_voc
    {
        #region Dependencies
        #endregion

        #region Properties
        private string _classname = "DL_trs_voc";

        private string _entityname = "trs_voc";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Voice of Customer";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _trs_topic = false;
        private string _trs_topic_value;
        public string trs_topic
        {
            get { return _trs_topic ? _trs_topic_value : null; }
            set { _trs_topic = true; _trs_topic_value = value; }
        }

        private bool _trs_customername = false;
        private string _trs_customername_value;
        public string trs_customername
        {
            get { return _trs_customername ? _trs_customername_value : null; }
            set { _trs_customername = true; _trs_customername_value = value; }
        }

        private bool _trs_contact = false;
        private string _trs_contact_value;
        public string trs_contact
        {
            get { return _trs_contact ? _trs_contact_value : null; }
            set { _trs_contact = true; _trs_contact_value = value; }
        }

        private bool _trs_voctype = false;
        private string _trs_voctype_value;
        public string trs_voctype
        {
            get { return _trs_voctype ? _trs_voctype_value : null; }
            set { _trs_voctype = true; _trs_voctype_value = value; }
        }

        private bool _trs_comment = false;
        private string _trs_comment_value;
        public string trs_comment
        {
            get { return _trs_comment ? _trs_comment_value : null; }
            set { _trs_comment = true; _trs_comment_value = value; }
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
                if (_trs_topic)
                {
                    Entity entity = new Entity(_entityname);
                    entity.Attributes["trs_topic"] = _trs_topic_value;
                    if (_trs_customername) { entity.Attributes["trs_customername"] = _trs_customername_value; }
                    if (_trs_contact) { entity.Attributes["trs_contact"] = _trs_contact_value; }
                    if (_trs_voctype) { entity.Attributes["trs_voctype"] = _trs_voctype_value; }
                    if (_trs_comment) { entity.Attributes["trs_comment"] = _trs_comment_value; }
                    return organizationService.Create(entity);
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
                if (_trs_topic) { entity.Attributes["trs_topic"] = _trs_topic_value; }
                if (_trs_customername) { entity.Attributes["trs_customername"] = _trs_customername_value; }
                if (_trs_contact) { entity.Attributes["trs_contact"] = _trs_contact_value; }
                if (_trs_voctype) { entity.Attributes["trs_voctype"] = _trs_voctype_value; }
                if (_trs_comment) { entity.Attributes["trs_comment"] = _trs_comment_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
