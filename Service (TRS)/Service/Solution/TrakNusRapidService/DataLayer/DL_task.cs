using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Crm.Sdk.Messages;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;

namespace TrakNusRapidService.DataLayer
{
    public class DL_task
    {
        #region Dependencies
        private DL_trs_tasklistheader _DL_trs_tasklistheader = new DL_trs_tasklistheader();
        private DL_serviceappointment _DL_serviceappointment = new DL_serviceappointment();
        private DL_team _DL_team = new DL_team();
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        private DL_incident _DL_incident = new DL_incident();
        private DL_transactioncurrency _DL_transactioncurrency = new DL_transactioncurrency();
        #endregion

        #region Properties
        private string _classname = "DL_task";

        private string _entityname = "task";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Commercial Header";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private string _activityid = null;

        public string activityid
        {
            get { return _activityid; }
            set { _activityid = value; }
        }

        private bool _trs_discountby = false;
        private bool _trs_discountby_value;
        public bool trs_discountby
        {
            get { return _trs_discountby ? _trs_discountby_value : false; }
            set { _trs_discountby = true; _trs_discountby_value = value; }
        }

        private bool _trs_totalrtg = false;
        private decimal _trs_totalrtg_value;
        public decimal trs_totalrtg
        {
            get { return _trs_totalrtg ? _trs_totalrtg_value : 0; }
            set { _trs_totalrtg = true; _trs_totalrtg_value = value; }
        }

        private bool _trs_itemnumber = false;
        private int _trs_itemnumber_value;
        public int trs_itemnumber
        {
            get { return _trs_itemnumber ? _trs_itemnumber_value : 10; }
            set { _trs_itemnumber = true; _trs_itemnumber_value = value; }
        }

        private bool _trs_tasklistheader = false;
        private EntityReference _trs_tasklistheader_value;
        public Guid trs_tasklistheader
        {
            get { return _trs_tasklistheader ? _trs_tasklistheader_value.Id : Guid.Empty; }
            set { _trs_tasklistheader = true; _trs_tasklistheader_value = new EntityReference(_DL_trs_tasklistheader.EntityName, value); }
        }

        private bool _transactioncurrencyid = false;
        private EntityReference _transactioncurrencyid_value;
        public Guid transactioncurrencyid
        {
            get { return _transactioncurrencyid ? _transactioncurrencyid_value.Id : Guid.Empty; }
            set { _transactioncurrencyid = true; _transactioncurrencyid_value = new EntityReference(_DL_transactioncurrency.EntityName, value); }
        }

        private bool _trs_price = false;
        private Money _trs_price_value;
        public decimal trs_price
        {
            get { return _trs_price ? _trs_price_value.Value : 0; }
            set { _trs_price = true; _trs_price_value = new Money(value); }
        }

        private bool _trs_operationid = false;
        private EntityReference _trs_operationid_value;
        public Guid trs_operationid
        {
            get { return _trs_operationid ? _trs_operationid_value.Id : Guid.Empty; }
            set { _trs_operationid = true; _trs_operationid_value = new EntityReference(_DL_serviceappointment.EntityName, value); }
        }

        private bool _regardingobjectid = false;
        private EntityReference _regardingobjectid_value;
        public Guid regardingobjectid
        {
            get { return _regardingobjectid ? _regardingobjectid_value.Id : Guid.Empty; }
            set { _regardingobjectid = true; _regardingobjectid_value = new EntityReference(_DL_incident.EntityName, value); }
        }

        private bool _trs_discountamount = false;
        private decimal _trs_discountamount_value;
        public decimal trs_discountamount
        {
            get { return _trs_discountamount ? _trs_discountamount_value : 0; }
            set { _trs_discountamount = true; _trs_discountamount_value = value; }
        }

        private bool _trs_totalprice = false;
        private Money _trs_totalprice_value;
        public decimal trs_totalprice
        {
            get { return _trs_totalprice ? _trs_totalprice_value.Value : 0; }
            set { _trs_totalprice = true; _trs_totalprice_value = new Money(value); }
        }

        private bool _subject = false;
        private string _subject_value;
        public string subject
        {
            get { return _subject ? _subject_value : null; }
            set { _subject = true; _subject_value = value; }
        }

        private bool _trs_discountpercent = false;
        private decimal _trs_discountpercent_value;
        public decimal trs_discountpercent
        {
            get { return _trs_discountpercent ? _trs_discountpercent_value : 0; }
            set { _trs_discountpercent = true; _trs_discountpercent_value = value; }
        }

        private bool _trs_fromquotation = false;
        private bool _trs_fromquotation_value;
        public bool trs_fromquotation
        {
            get { return _trs_fromquotation ? _trs_fromquotation_value : false; }
            set { _trs_fromquotation = true; _trs_fromquotation_value = value; }
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
                EntityCollection entityCollection = organizationService.RetrieveMultiple(queryExpression);
                return entityCollection;
                
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Select  : " + ex.Message.ToString());
            }
        }

        public Guid Insert(IOrganizationService organizationService)
        {
            try
            {
                if (_trs_tasklistheader)
                {
                    Entity entity = new Entity(_entityname);
                    if (_trs_operationid) { entity.Attributes["trs_operationid"] = _trs_operationid_value; };
                    if (_trs_tasklistheader) { entity.Attributes["trs_tasklistheader"] = _trs_tasklistheader_value; };
                    if (_trs_price) { entity.Attributes["trs_price"] = _trs_price_value; };
                    if (_trs_totalrtg) { entity.Attributes["trs_totalrtg"] = _trs_totalrtg_value; };
                    if (_regardingobjectid) { entity.Attributes["regardingobjectid"] = _regardingobjectid_value; }
                    if (_trs_discountamount) { entity.Attributes["trs_discountamount"] = new Money(_trs_discountamount_value); }
                    if (_trs_totalprice) { entity.Attributes["trs_totalprice"] = _trs_totalprice_value; }
                    if (_subject) { entity.Attributes["subject"] = _subject_value; }
                    if (_trs_discountpercent) { entity.Attributes["trs_discountpercent"] = _trs_discountpercent_value; }
                    if (_trs_discountby) { entity.Attributes["trs_discountby"] = _trs_discountby_value; };
                    if (_trs_fromquotation) { entity.Attributes["trs_fromquotation"] = _trs_fromquotation_value; };
                    if (_transactioncurrencyid) { entity.Attributes["transactioncurrencyid"] = _transactioncurrencyid_value; };
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
                if (_trs_totalrtg) { entity.Attributes["trs_totalrtg"] = _trs_totalrtg_value; };
                if (_trs_itemnumber) { entity.Attributes["trs_itemnumber"] = _trs_itemnumber_value; };
                if (_trs_discountby) { entity.Attributes["trs_discountby"] = _trs_discountby_value; };
                if (_trs_fromquotation) { entity.Attributes["trs_fromquotation"] = _trs_fromquotation_value; };
                if (_transactioncurrencyid) { entity.Attributes["transactioncurrencyid"] = _transactioncurrencyid_value; };
                //if (_trs_quotationnumber) { entity.Attributes["trs_quotationnumber"] = _trs_quotationnumber_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }

        public void AssociatetoWO(IOrganizationService organizationService, Guid id, Guid serviceAppointmentId)
        {
            try
            {
                AssociateRequest associate = new AssociateRequest
                {
                    Target = new EntityReference(_DL_serviceappointment.EntityName, serviceAppointmentId),
                    RelatedEntities = new EntityReferenceCollection
                    {
                        new EntityReference(_entityname, id)
                    },
                    Relationship = new Relationship("trs_serviceappointment_task")
                };
                organizationService.Execute(associate);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".AssociatetoWO : " + ex.Message);
            }
        }

        public void AssigntoTeam(IOrganizationService organizationService, Guid id, Guid teamId)
        {
            try
            {
                AssignRequest assign = new AssignRequest
                {
                    Assignee = new EntityReference(_DL_team.EntityName, teamId),
                    Target = new EntityReference(_entityname, id)
                };
                organizationService.Execute(assign);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".AssigntoTeam : " + ex.Message);
            }
        }

        public void AssigntoUser(IOrganizationService organizationService, Guid id, Guid userId)
        {
            try
            {
                AssignRequest assign = new AssignRequest
                {
                    Assignee = new EntityReference(_DL_systemuser.EntityName, userId),
                    Target = new EntityReference(_entityname, id)
                };
                organizationService.Execute(assign);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".AssigntoUser : " + ex.Message);
            }
        }
    }
}
