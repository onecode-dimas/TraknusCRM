using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Crm.Sdk.Messages;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_incident
    {
        #region Dependencies
        private DL_account _DL_account = new DL_account();
        private DL_contact _DL_contact = new DL_contact();
        private DL_new_population _DL_new_population = new DL_new_population();
        private DL_contract _DL_contract = new DL_contract();
        private DL_contractdetail _DL_contractdetail = new DL_contractdetail();
        private DL_team _DL_team = new DL_team();
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        private DL_trs_tasklistgroup _DL_trs_tasklistgroup = new DL_trs_tasklistgroup();
        #endregion

        #region Properties
        private string _classname = "DL_incident";

        private string _entityname = "incident";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Service Requisition";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _title = false;
        private string _title_value;
        public string title
        {
            get { return _title ? _title_value : null; }
            set { _title = true; _title_value = value; }
        }

        private bool _description = false;
        private string _description_value;
        public string description
        {
            get { return _description ? _description_value : null; }
            set { _description = true; _description_value = value; }
        }

        private bool _ticketnumber = false;
        private string _ticketnumber_value;
        public string ticketnumber
        {
            get { return _ticketnumber ? _ticketnumber_value : null; }
            set { _ticketnumber = true; _ticketnumber_value = value; }
        }

        private bool _responseby = false;
        private DateTime _responseby_value;
        public DateTime responseby
        {
            get { return _responseby ? _responseby_value.ToLocalTime() : DateTime.MinValue; }
            set { _responseby = true; _responseby_value = value.ToLocalTime(); }
        }

        private bool _resolveby = false;
        private DateTime _resolveby_value;
        public DateTime resolveby
        {
            get { return _resolveby ? _resolveby_value.ToLocalTime() : DateTime.MinValue; }
            set { _resolveby = true; _resolveby_value = value.ToLocalTime(); }
        }

        private bool _customerid = false;
        private EntityReference _customerid_value;
        public Guid customerid
        {
            get { return _customerid ? _customerid_value.Id : Guid.Empty; }
            set { _customerid = true; _customerid_value = new EntityReference(_DL_account.EntityName, value); }
        }

        private bool _primarycontactid = false;
        private EntityReference _primarycontactid_value;
        public Guid primarycontactid
        {
            get { return _primarycontactid ? _primarycontactid_value.Id : Guid.Empty; }
            set { _primarycontactid = true; _primarycontactid_value = new EntityReference(_DL_contact.EntityName, value); }
        }

        private bool _trs_unit = false;
        private EntityReference _trs_unit_value;
        public Guid trs_unit
        {
            get { return _trs_unit ? _trs_unit_value.Id : Guid.Empty; }
            set { _trs_unit = true; _trs_unit_value = new EntityReference(_DL_new_population.EntityName, value); }
        }

        private bool _trs_automaticsr = false;
        private bool _trs_automaticsr_value;
        public bool trs_automaticsr
        {
            get { return _trs_automaticsr ? _trs_automaticsr : false; }
            set { _trs_automaticsr = true; _trs_automaticsr_value = value; }
        }

        private bool _trs_servicefee = false;
        private bool _trs_servicefee_value;
        public bool trs_servicefee
        {
            get { return _trs_servicefee ? _trs_servicefee_value : false; }
            set { _trs_servicefee = true; _trs_servicefee_value = value; } 
        }

        private bool _contractid = false;
        private EntityReference _contractid_value;
        public Guid contractid
        {
            get { return _contractid ? _contractid_value.Id : Guid.Empty; }
            set { _contractid = true; _contractid_value = new EntityReference(_DL_contract.EntityName, value); }
        }

        private bool _contractdetailid = false;
        private EntityReference _contractdetailid_value;
        public Guid contractdetailid
        {
            get { return _contractdetailid ? _contractdetailid_value.Id : Guid.Empty; }
            set { _contractdetailid = true; _contractdetailid_value = new EntityReference(_DL_contractdetail.EntityName, value); }
        }

        private bool _parentcaseid = false;
        private EntityReference _parentcaseid_value;
        public Guid parentcaseid
        {
            get { return _parentcaseid ? _parentcaseid_value.Id : Guid.Empty; }
            set { _parentcaseid = true; _parentcaseid_value = new EntityReference(_entityname, value); }
        }

        private bool _trs_pmacttype = false;
        private EntityReference _trs_pmacttype_value;
        public Guid trs_pmacttype
        {
            get { return _trs_pmacttype ? _trs_pmacttype_value.Id : Guid.Empty; }
            set { _trs_pmacttype = true; _trs_pmacttype_value = new EntityReference(_DL_trs_tasklistgroup.EntityName, value); }
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
                if (_title)
                {
                    Entity entity = new Entity(_entityname);
                    entity.Attributes["title"] = _title_value;
                    if (_ticketnumber) { entity.Attributes["ticketnumber"] = _ticketnumber_value; }
                    if (_customerid) { entity.Attributes["customerid"] = _customerid_value; }
                    if (_primarycontactid) { entity.Attributes["primarycontactid"] = _primarycontactid_value; }
                    if (_trs_unit) { entity.Attributes["trs_unit"] = _trs_unit_value; }
                    if (_trs_automaticsr) { entity.Attributes["trs_automaticsr"] = _trs_automaticsr_value; }
                    if (_trs_servicefee) { entity.Attributes["trs_servicefee"] = _trs_servicefee_value; }
                    if (_contractid) { entity.Attributes["contractid"] = _contractid_value; }
                    if (_contractdetailid) { entity.Attributes["contractdetailid"] = _contractdetailid_value; }
                    if (_resolveby) { entity.Attributes["resolveby"] = _resolveby_value; }
                    if (_responseby) { entity.Attributes["responseby"] = _responseby_value; }
                    if (_parentcaseid) { entity.Attributes["parentcaseid"] = _parentcaseid_value; }
                    if (_description) { entity.Attributes["description"] = _description_value; }
                    if (_trs_pmacttype) { entity.Attributes["trs_pmacttype"] = _trs_pmacttype_value; }
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
                if (_title) { entity.Attributes["title"] = _title_value; }
                if (_ticketnumber) { entity.Attributes["ticketnumber"] = _ticketnumber_value; }
                if (_customerid) { entity.Attributes["customerid"] = _customerid_value; }
                if (_primarycontactid) { entity.Attributes["primarycontactid"] = _primarycontactid_value; }
                if (_trs_unit) { entity.Attributes["trs_unit"] = _trs_unit_value; }
                if (_trs_automaticsr) { entity.Attributes["trs_automaticsr"] = _trs_automaticsr_value; }
                if (_trs_servicefee) { entity.Attributes["trs_servicefee"] = _trs_servicefee_value; }
                if (_contractid) { entity.Attributes["contractid"] = _contractid_value; }
                if (_contractdetailid) { entity.Attributes["contractdetailid"] = _contractdetailid_value; }
                if (_resolveby) { entity.Attributes["resolveby"] = _resolveby_value; }
                if (_responseby) { entity.Attributes["responseby"] = _responseby_value; }
                if (_parentcaseid) { entity.Attributes["parentcaseid"] = _parentcaseid_value; }
                if (_description) { entity.Attributes["description"] = _description_value; }
                if (_trs_pmacttype) { entity.Attributes["trs_pmacttype"] = _trs_pmacttype_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
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
