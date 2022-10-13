using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;

namespace TrakNusSparepartSystem.DataLayer
{
    public class DL_trs_quotation
    {
        #region Dependencies
        private DL_team _DL_team = new DL_team();
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        private DL_trs_discountapproval _DL_trs_discountapproval = new DL_trs_discountapproval();
        #endregion

        #region Properties
        private string _classname = "DL_trs_quotation";

        private string _entityname = "trs_quotation";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Quotation";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _trs_quotationnumber = false;
        private string _trs_quotationnumber_value;
        public string trs_quotationnumber
        {
            get { return _trs_quotationnumber ? _trs_quotationnumber_value : null; }
            set { _trs_quotationnumber = true; _trs_quotationnumber_value = value; }
        }


        private bool _trs_subtotalservices = false;
        private Money _trs_subtotalservices_value;
        public decimal trs_subtotalservices
        {
            get { return _trs_subtotalservices ? _trs_subtotalservices_value.Value : 0; }
            set { _trs_subtotalservices = true; _trs_subtotalservices_value = new Money(value); }
        }

        private bool _trs_discountservices = false;
        private Money _trs_discountservices_value;
        public decimal trs_discountservices
        {
            get { return _trs_discountservices ? _trs_discountservices_value.Value : 0; }
            set { _trs_discountservices = true; _trs_discountservices_value = new Money(value); }
        }

        private bool _trs_totalservices = false;
        private Money _trs_totalservices_value;
        public decimal trs_totalservices
        {
            get { return _trs_totalservices ? _trs_totalservices_value.Value : 0; }
            set { _trs_totalservices = true; _trs_totalservices_value = new Money(value); }
        }

        private bool _trs_subtotalparts = false;
        private Money _trs_subtotalparts_value;
        public decimal trs_subtotalparts
        {
            get { return _trs_subtotalparts ? _trs_subtotalparts_value.Value : 0; }
            set { _trs_subtotalparts = true; _trs_subtotalparts_value = new Money(value); }
        }

        private bool _trs_discountparts = false;
        private Money _trs_discountparts_value;
        public decimal trs_discountparts
        {
            get { return _trs_discountparts ? _trs_discountparts_value.Value : 0; }
            set { _trs_discountparts = true; _trs_discountparts_value = new Money(value); }
        }

        private bool _trs_totalparts = false;
        private Money _trs_totalparts_value;
        public decimal trs_totalparts
        {
            get { return _trs_totalparts ? _trs_totalparts_value.Value : 0; }
            set { _trs_totalparts = true; _trs_totalparts_value = new Money(value); }
        }

        private bool _trs_totalsupportingmaterials = false;
        private Money _trs_totalsupportingmaterials_value;
        public decimal trs_totalsupportingmaterials
        {
            get { return _trs_totalsupportingmaterials ? _trs_totalsupportingmaterials_value.Value : 0; }
            set { _trs_totalsupportingmaterials = true; _trs_totalsupportingmaterials_value = new Money(value); }
        }

        private bool _trs_subtotalamount = false;
        private Money _trs_subtotalamount_value;
        public decimal trs_subtotalamount
        {
            get { return _trs_subtotalamount ? _trs_subtotalamount_value.Value : 0; }
            set { _trs_subtotalamount = true; _trs_subtotalamount_value = new Money(value); }
        }

        private bool _trs_discountamount = false;
        private Money _trs_discountamount_value;
        public decimal trs_discountamount
        {
            get { return _trs_discountamount ? _trs_discountamount_value.Value : 0; }
            set { _trs_discountamount = true; _trs_discountamount_value = new Money(value); }
        }

        private bool _trs_totalamount = false;
        private Money _trs_totalamount_value;
        public decimal trs_totalamount
        {
            get { return _trs_totalamount ? _trs_totalamount_value.Value : 0; }
            set { _trs_totalamount = true; _trs_totalamount_value = new Money(value); }
        }

        private bool _trs_revision = false;
        private int _trs_revision_value;
        public int trs_revision
        {
            get { return _trs_revision ? _trs_revision_value : int.MinValue; }
            set { _trs_revision = true; _trs_revision_value = value; }
        }

        private bool _statuscode = false;
        private int _statuscode_value;
        public int statuscode
        {
            get { return _statuscode ? _statuscode_value : int.MinValue; }
            set { _statuscode = true; _statuscode_value = value; }
        }



        private bool _trs_printingsequence = false;
        private double _trs_printingsequence_value;
        public double trs_printingsequence
        {
            get { return _trs_printingsequence ? _trs_printingsequence_value : 0; }
            set { _trs_printingsequence = true; _trs_printingsequence_value = value; }
        }

        private bool _trs_fillingnumber = false;
        private string _trs_fillingnumber_value;
        public string trs_fillingnumber
        {
            get { return _trs_fillingnumber ? _trs_fillingnumber_value : string.Empty; }
            set { _trs_fillingnumber = true; _trs_fillingnumber_value = value; }
        }

        private bool _trs_submitdate = false;
        private DateTime _trs_submitdate_value;
        public DateTime trs_submitdate
        {
            get { return _trs_submitdate ? _trs_submitdate_value.ToLocalTime() : DateTime.MinValue; }
            set { _trs_submitdate = true; _trs_submitdate_value = value.ToLocalTime(); }
        }

        private bool _trs_approveddate = false;
        private DateTime _trs_approveddate_value;
        public DateTime trs_approveddate
        {
            get { return _trs_approveddate ? _trs_approveddate_value.ToLocalTime() : DateTime.MinValue; }
            set { _trs_approveddate = true; _trs_approveddate_value = value.ToLocalTime(); }
        }

        private bool _trs_discountapprovalid = false;
        private EntityReference _trs_discountapprovalid_value;
        public Guid trs_discountapprovalid
        {
            get { return _trs_discountapprovalid ? _trs_discountapprovalid_value.Id : Guid.Empty; }
            set { _trs_discountapprovalid = true; _trs_discountapprovalid_value = new EntityReference(_DL_trs_discountapproval.EntityName, value); }
        }

        private bool _trs_crmwonumber = false;
        private string _trs_crmwonumber_value;
        public string trs_crmwonumber
        {
            get { return _trs_crmwonumber ? _trs_crmwonumber_value : string.Empty; }
            set { _trs_crmwonumber = true; _trs_crmwonumber_value = value; }
        }


        private bool _trs_customer;
        private EntityReference _trs_customer_value;

        public EntityReference trs_customer
        {
            get { return _trs_customer_value; }
            set
            {
                _trs_customer_value = value;
                _trs_customer = true;
            }
        }

        private bool _trs_contact;
        private EntityReference _trs_contact_value;

        public EntityReference trs_contact
        {
            get { return _trs_customer_value; }
            set
            {
                _trs_customer_value = value;
                _trs_contact = true;
            }
        }

        private bool _trs_pmactytype;
        private EntityReference _trs_pmactytype_value;

        public EntityReference trs_pmactytype
        {
            get { return _trs_pmactytype_value; }
            set
            {
                _trs_pmactytype_value = value;
                _trs_pmactytype = true;
            }
        }


        private bool _trs_unit;
        private EntityReference _trs_unit_value;

        public EntityReference trs_unit
        {
            get { return _trs_unit_value; }
            set
            {
                _trs_unit_value = value;
                _trs_unit = true;
            }
        }

        private bool _trs_branch;
        private EntityReference _trs_branch_value;

        public EntityReference trs_branch
        {
            get { return _trs_branch_value; }
            set
            {
                _trs_branch_value = value;
                _trs_branch = true;
            }
        }

        private bool _trs_site;
        private EntityReference _trs_site_value;

        public EntityReference trs_site
        {
            get { return _trs_site_value; }
            set
            {
                _trs_site_value = value;
                _trs_site = true;
            }
        }

        private bool _trs_discountheaderamount;
        private decimal _trs_discountheaderamount_value;

        public decimal trs_discountheaderamount
        {
            get { return _trs_discountheaderamount_value; }
            set
            {
                _trs_discountheaderamount_value = value;
                _trs_discountheaderamount = true;
            }
        }


        private bool _trs_paymentterm;
        private EntityReference _trs_paymentterm_value;

        public EntityReference trs_paymentterm
        {
            get { return _trs_paymentterm_value; }
            set
            {
                _trs_paymentterm_value = value;
                _trs_paymentterm = true;
            }
        }

        private bool _transactioncurrencyid;
        private EntityReference _transactioncurrencyid_value;

        public EntityReference transactioncurrencyid
        {
            get { return _transactioncurrencyid_value; }
            set
            {
                _transactioncurrencyid_value = value;
                _transactioncurrencyid = true;
            }
        }

        private bool _trs_servicerequisition;
        private EntityReference _trs_servicerequisition_value;

        public EntityReference trs_servicerequisition
        {
            get { return _trs_servicerequisition_value; }
            set
            {
                _trs_servicerequisition_value = value;
                _trs_servicerequisition = true;
            }
        }

        private bool _ownerid;
        private EntityReference _ownerid_value;

        public EntityReference ownerid
        {
            get { return _ownerid_value; }
            set
            {
                _ownerid_value = value;
                _ownerid = true;
            }
        }

        private bool _trs_quotationdeal = false;
        private bool _trs_quotationdeal_value;
        public bool trs_quotationdeal
        {
            get { return _trs_quotationdeal ? _trs_quotationdeal_value : false; }
            set { _trs_quotationdeal = true; _trs_quotationdeal_value = value; }
        }

        //OptionSet
        private bool _tss_statusassignquo = false;
        private int _tss_statusassignquo_value;
        public int tss_statusassignquo
        {
            get { return _tss_statusassignquo ? _tss_statusassignquo_value : int.MinValue; }
            set { _tss_statusassignquo = true; _tss_statusassignquo_value = value; }
        }

        private bool _trs_estimationcloseddate = false;
        private DateTime _trs_estimationcloseddate_value;
        public DateTime trs_estimationcloseddate
        {
            get { return _trs_estimationcloseddate ? _trs_estimationcloseddate_value.ToLocalTime() : DateTime.MinValue; }
            set { _trs_estimationcloseddate = true; _trs_estimationcloseddate_value = value.ToLocalTime(); }
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
                if (_trs_quotationnumber) { entity.Attributes["trs_quotationnumber"] = _trs_quotationnumber_value; }
                if (_trs_subtotalservices) { entity.Attributes["trs_subtotalservices"] = _trs_subtotalservices_value; }
                if (_trs_discountservices) { entity.Attributes["trs_discountservices"] = _trs_discountservices_value; }
                if (_trs_totalservices) { entity.Attributes["trs_totalservices"] = _trs_totalservices_value; }
                if (_trs_subtotalparts) { entity.Attributes["trs_subtotalparts"] = _trs_subtotalparts_value; }
                if (_trs_discountparts) { entity.Attributes["trs_discountparts"] = _trs_discountparts_value; }
                if (_trs_totalparts) { entity.Attributes["trs_totalparts"] = _trs_totalparts_value; }
                if (_trs_totalsupportingmaterials) { entity.Attributes["trs_totalsupportingmaterials"] = _trs_totalsupportingmaterials_value; }
                if (_trs_subtotalamount) { entity.Attributes["trs_subtotalamount"] = _trs_subtotalamount_value; }
                if (_trs_discountamount) { entity.Attributes["trs_discountamount"] = _trs_discountamount_value; }
                if (_trs_totalamount) { entity.Attributes["trs_totalamount"] = _trs_totalamount_value; }
                if (_trs_revision) { entity.Attributes["trs_revision"] = _trs_revision_value; }
                if (_statuscode) { entity.Attributes["statuscode"] = new OptionSetValue(_statuscode_value); }
                if (_trs_printingsequence) { entity.Attributes["trs_printingsequence"] = _trs_printingsequence_value; }
                if (_trs_fillingnumber) { entity.Attributes["trs_fillingnumber"] = _trs_fillingnumber_value; }
                if (_trs_submitdate) { entity.Attributes["trs_submitdate"] = _trs_submitdate_value; }
                if (_trs_approveddate) { entity.Attributes["trs_approveddate"] = _trs_approveddate_value; }
                if (_trs_discountapprovalid) { entity.Attributes["trs_discountapprovalid"] = _trs_discountapprovalid_value; }
                if (_trs_crmwonumber) { entity.Attributes["trs_crmwonumber"] = _trs_crmwonumber_value; }
                if (_trs_customer) { entity["trs_customer"] = _trs_customer_value; }
                if (_trs_contact) { entity["trs_contact"] = _trs_contact_value; }
                if (_trs_pmactytype) { entity["trs_pmactytype"] = _trs_pmactytype_value; }
                if (_trs_unit) { entity["trs_unit"] = _trs_unit_value; }
                if (_trs_branch) { entity["trs_branch"] = _trs_branch_value; }
                if (_trs_site) { entity["trs_site"] = _trs_site_value; }
                if (_trs_discountheaderamount) { entity["trs_discountheaderamount"] = _trs_discountheaderamount_value; }
                if (_trs_paymentterm) { entity["trs_paymentterm"] = _trs_paymentterm_value; }
                if (_transactioncurrencyid) { entity["transactioncurrencyid"] = _transactioncurrencyid_value; }
                if (_trs_servicerequisition) { entity["trs_servicerequisition"] = _trs_servicerequisition_value; }
                if (_ownerid) { entity["ownerid"] = _ownerid_value; }
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
                if (_trs_quotationnumber) { entity.Attributes["trs_quotationnumber"] = _trs_quotationnumber_value; }
                if (_trs_subtotalservices) { entity.Attributes["trs_subtotalservices"] = _trs_subtotalservices_value; }
                if (_trs_discountservices) { entity.Attributes["trs_discountservices"] = _trs_discountservices_value; }
                if (_trs_totalservices) { entity.Attributes["trs_totalservices"] = _trs_totalservices_value; }
                if (_trs_subtotalparts) { entity.Attributes["trs_subtotalparts"] = _trs_subtotalparts_value; }
                if (_trs_discountparts) { entity.Attributes["trs_discountparts"] = _trs_discountparts_value; }
                if (_trs_totalparts) { entity.Attributes["trs_totalparts"] = _trs_totalparts_value; }
                if (_trs_totalsupportingmaterials) { entity.Attributes["trs_totalsupportingmaterials"] = _trs_totalsupportingmaterials_value; }
                if (_trs_subtotalamount) { entity.Attributes["trs_subtotalamount"] = _trs_subtotalamount_value; }
                if (_trs_discountamount) { entity.Attributes["trs_discountamount"] = _trs_discountamount_value; }
                if (_trs_totalamount) { entity.Attributes["trs_totalamount"] = _trs_totalamount_value; }
                if (_trs_revision) { entity.Attributes["trs_revision"] = _trs_revision_value; }
                if (_statuscode) { entity.Attributes["statuscode"] = new OptionSetValue(_statuscode_value); }
                if (_trs_printingsequence) { entity.Attributes["trs_printingsequence"] = _trs_printingsequence_value; }
                if (_trs_fillingnumber) { entity.Attributes["trs_fillingnumber"] = _trs_fillingnumber_value; }
                if (_trs_submitdate) { entity.Attributes["trs_submitdate"] = _trs_submitdate_value; }
                if (_trs_approveddate) { entity.Attributes["trs_approveddate"] = _trs_approveddate_value; }
                if (_trs_discountapprovalid) { entity.Attributes["trs_discountapprovalid"] = _trs_discountapprovalid_value; }
                if (_trs_crmwonumber) { entity.Attributes["trs_crmwonumber"] = _trs_crmwonumber_value; }
                if (_trs_customer) { entity["trs_customer"] = _trs_customer_value; }
                if (_trs_contact) { entity["trs_contact"] = _trs_contact_value; }
                if (_trs_pmactytype) { entity["trs_pmactytype"] = _trs_pmactytype_value; }
                if (_trs_unit) { entity["trs_unit"] = _trs_unit_value; }
                if (_trs_branch) { entity["trs_branch"] = _trs_branch_value; }
                if (_trs_site) { entity["trs_site"] = _trs_site_value; }
                if (_trs_discountheaderamount) { entity["trs_discountheaderamount"] = _trs_discountheaderamount_value; }
                if (_trs_paymentterm) { entity["trs_paymentterm"] = _trs_paymentterm_value; }
                if (_transactioncurrencyid) { entity["transactioncurrencyid"] = _transactioncurrencyid_value; }
                if (_trs_servicerequisition) { entity["trs_servicerequisition"] = _trs_servicerequisition_value; }
                if (_trs_quotationdeal) { entity.Attributes["trs_quotationdeal"] = _trs_quotationdeal_value; }
                if (_ownerid) { entity["ownerid"] = _ownerid_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }

        public void Revise(IOrganizationService organizationService, Guid id)
        {
            try
            {
                EntityReference entityReference = new EntityReference(_entityname, id);
                OrganizationRequest organizationRequest = new OrganizationRequest();
                organizationRequest.RequestName = "SetState";
                organizationRequest["EntityMoniker"] = entityReference;
                organizationRequest["State"] = new OptionSetValue(0);
                organizationRequest["Status"] = new OptionSetValue(167630003);
                organizationService.Execute(organizationRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Revise : " + ex.Message);
            }
        }

        public void WaitingApproval(IOrganizationService organizationService, Guid id)
        {
            try
            {
                EntityReference entityReference = new EntityReference(_entityname, id);
                OrganizationRequest organizationRequest = new OrganizationRequest();
                organizationRequest.RequestName = "SetState";
                organizationRequest["EntityMoniker"] = entityReference;
                organizationRequest["State"] = new OptionSetValue(0);
                organizationRequest["Status"] = new OptionSetValue(167630001);
                organizationService.Execute(organizationRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".WaitingApproval : " + ex.Message);
            }
        }

        public void Approve(IOrganizationService organizationService, Guid id)
        {
            try
            {
                EntityReference entityReference = new EntityReference(_entityname, id);
                OrganizationRequest organizationRequest = new OrganizationRequest();
                organizationRequest.RequestName = "SetState";
                organizationRequest["EntityMoniker"] = entityReference;
                organizationRequest["State"] = new OptionSetValue(0);
                organizationRequest["Status"] = new OptionSetValue(167630000);
                organizationService.Execute(organizationRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Approve : " + ex.Message);
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

        public void UpdateStatusAssignQuo(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                entity.Id = id;
                if (_tss_statusassignquo) { entity.Attributes["tss_statusassignquo"] = new OptionSetValue(_tss_statusassignquo_value); }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".UpdateStatusAssignQuo : " + ex.Message.ToString());
            }
        }

        /*
        public void CopyEntityQuotation(IOrganizationService organizationService, string entityName, Guid id)
        {
            try
            {
                var getQuotation = organizationService.Retrieve(entityName, id, new ColumnSet(true));

                Entity target = new Entity(getQuotation.LogicalName);
                if (getQuotation != null && getQuotation.Attributes.Count > 0)
                {
                    //CloneRecord(getQuotation, target);  //clone header

                    #region insert quoation(service) to quoation part
                    Entity entityQuoationPart = new Entity("tss_quotationpartheader");
                    entityQuoationPart.Attributes["tss_quotationserviceno"] = new EntityReference("trs_quotation", getQuotation.Id);
                    entityQuoationPart.Attributes["tss_servicequoteowner"] = getQuotation.GetAttributeValue<EntityReference>("ownerid").Id;

                    //DateTime estimatedCloseDate = getQuotation.GetAttributeValue<DateTime>("trs_estimationcloseddate");

                    if (getQuotation.GetAttributeValue<DateTime>("trs_estimationcloseddate") != (DateTime?)null)
                        entityQuoationPart.Attributes["tss_estimationclosedate"] = getQuotation.GetAttributeValue<DateTime>("trs_estimationcloseddate");
                    else
                        entityQuoationPart.Attributes["tss_estimationclosedate"] = (DateTime?)null;

                    //entityQuoationPart.Attributes["tss_estimationclosedate"] = (getQuotation.GetAttributeValue<DateTime>("trs_estimationcloseddate")==null ? null : getQuotation.GetAttributeValue<DateTime>("trs_estimationcloseddate"));
                    //if (_trs_estimationcloseddate) { entityQuoationPart.Attributes["tss_estimationclosedate"] = _trs_estimationcloseddate_value; }

                    entityQuoationPart.Attributes["tss_customer"] = getQuotation.GetAttributeValue<EntityReference>("trs_customer");    //lookup
                    entityQuoationPart.Attributes["tss_branch"] = getQuotation.GetAttributeValue<EntityReference>("trs_branch");      //lookup
                    entityQuoationPart.Attributes["tss_contact"] = getQuotation.GetAttributeValue<EntityReference>("trs_contact");  //lookup
                    entityQuoationPart.Attributes["tss_currency"] = getQuotation.GetAttributeValue<EntityReference>("transactioncurrencyid");  //lookup
                    entityQuoationPart.Attributes["tss_pss"] = getQuotation.GetAttributeValue<EntityReference>("tss_pss");  //lookup
                    entityQuoationPart.Attributes["tss_statuscode"] = 865920002; //apporved
                    entityQuoationPart.Attributes["tss_revision"] = getQuotation.GetAttributeValue<int>("trs_revision");   //whole number
                    Guid QuoationPartId = organizationService.Create(entityQuoationPart);
                    #endregion


                    #region Component to Quotation Part Lines
                    QueryExpression QueryComponent = new QueryExpression("trs_quotationpartssummary");  //component from quotation
                    QueryComponent.ColumnSet = new ColumnSet(true);
                    QueryComponent.Criteria.AddCondition("trs_quotationnumber", ConditionOperator.Equal, getQuotation.Id);

                    EntityCollection CompoentnItems = organizationService.RetrieveMultiple(QueryComponent);
                    if (CompoentnItems.Entities.Count > 0)
                    {
                        foreach (Entity sItem in CompoentnItems.Entities)
                        {
                            //Entity tItem = new Entity(sItem.LogicalName);
                            //CloneRecord(sItem, tItem);   //clone line

                            //insert to quotation part lines
                            Entity PartLineEntity = new Entity("tss_quotationpartlines");
                            PartLineEntity.Attributes["tss_quotationpartheader"] = new EntityReference("tss_quotationpartheader", QuoationPartId); //get from new data quotation part header
                            //PartLineEntity.Attributes["tss_partnumber"] = sItem.GetAttributeValue<EntityReference>("trs_masterpart").Id;
                            PartLineEntity.Attributes["tss_partnumber"] = sItem.GetAttributeValue<EntityReference>("trs_partnumber");
                            PartLineEntity.Attributes["tss_quantity"] = sItem.GetAttributeValue<int>("trs_manualquantity");
                            PartLineEntity.Attributes["tss_price"] = sItem.GetAttributeValue<Money>("trs_price");
                            PartLineEntity.Attributes["tss_discountby"] = sItem.GetAttributeValue<bool>("trs_discountby");
                            PartLineEntity.Attributes["tss_discountamount"] = sItem.GetAttributeValue<Money>("trs_discountamount");
                            PartLineEntity.Attributes["tss_discountpercent"] = sItem.GetAttributeValue<decimal>("trs_discountpercent");
                            PartLineEntity.Attributes["tss_totalprice"] = sItem.GetAttributeValue<Money>("trs_totalprice");
                            organizationService.Create(PartLineEntity);
                        }
                    }
                    #endregion


                    #region Service to Quotation Part Lines - Service
                    QueryExpression QueryService = new QueryExpression("trs_quotationcommercialheader");  //component from quotation
                    QueryService.ColumnSet = new ColumnSet(true);
                    QueryService.Criteria.AddCondition("trs_quotationid", ConditionOperator.Equal, getQuotation.Id);

                    EntityCollection ServiceItems = organizationService.RetrieveMultiple(QueryService);
                    if (ServiceItems.Entities.Count > 0)
                    {
                        foreach (Entity sItem in ServiceItems.Entities)
                        {
                            decimal diskonSatuan;
                            decimal discountAmount = sItem.GetAttributeValue<Money>("trs_discountamount").Value;
                            int taskListQuantity = sItem.GetAttributeValue<int>("trs_pricetype");
                            int manualQuantity = sItem.GetAttributeValue<int>("trs_pricetype");
                            int quantity;

                            if (manualQuantity != null)
                                quantity = manualQuantity;
                            else if (taskListQuantity != null && manualQuantity == null)
                                quantity = taskListQuantity;
                            else
                                quantity = 0;

                            //if discount by mount
                            if (sItem.GetAttributeValue<bool>("trs_discountby") == true) //value: amount (1)
                                diskonSatuan = discountAmount / quantity;
                            else
                                diskonSatuan = sItem.GetAttributeValue<Money>("trs_discountamount").Value;

                            //insert to Quotation Part Lines - Service
                            Entity ServiceEntity = new Entity("tss_quotationpartlinesservice");
                            //CloneRecord(sItem, ServiceEntity);   //clone line service
                            ServiceEntity.Attributes["tss_quotationpartheader"] = new EntityReference("tss_quotationpartheader", QuoationPartId);
                            ServiceEntity.Attributes["tss_taskheader"] = sItem.GetAttributeValue<EntityReference>("trs_taskheader");
                            ServiceEntity.Attributes["tss_pricetype"] = sItem.GetAttributeValue<OptionSetValue>("trs_pricetype");
                            ServiceEntity.Attributes["tss_commercialheader"] = sItem.GetAttributeValue<string>("trs_commercialheader");
                            ServiceEntity.Attributes["tss_price"] = sItem.GetAttributeValue<Money>("trs_price");
                            ServiceEntity.Attributes["tss_discountby"] = sItem.GetAttributeValue<bool>("trs_discountby");
                            ServiceEntity.Attributes["tss_discountpercent"] = sItem.GetAttributeValue<decimal>("trs_discountpercent");
                            ServiceEntity.Attributes["tss_discountamount"] = diskonSatuan;
                            ServiceEntity.Attributes["tss_totalprice"] = sItem.GetAttributeValue<Money>("trs_totalprice");
                            //get tss_quotationpartno from quotatin service header
                            ServiceEntity.Attributes["tss_quotationpartno"] = getQuotation.GetAttributeValue<EntityReference>("tss_quotationpartno");
                            organizationService.Create(ServiceEntity);
                        }
                    }
                    #endregion


                    #region Support Material to Quotation Part Lines - Support Material
                    QueryExpression QueryMaterial = new QueryExpression("trs_quotationsupportingmaterial");  //component from quotation
                    QueryMaterial.ColumnSet = new ColumnSet(true);
                    QueryMaterial.Criteria.AddCondition("trs_quotationid", ConditionOperator.Equal, getQuotation.Id);

                    EntityCollection MaterialItems = organizationService.RetrieveMultiple(QueryMaterial);
                    if (MaterialItems.Entities.Count > 0)
                    {
                        foreach (Entity sItem in MaterialItems.Entities)
                        {
                            //insert to Quotation Part Lines - support material 
                            Entity MaterialEntity = new Entity("tss_quotationpartlinessupportingmaterial");
                            MaterialEntity.Attributes["tss_quotationpartheader"] = new EntityReference("tss_quotationpartheader", QuoationPartId);
                            MaterialEntity.Attributes["tss_type"] = sItem.GetAttributeValue<bool>("trs_type");
                            MaterialEntity.Attributes["tss_supportingmaterial"] = sItem.GetAttributeValue<string>("trs_supportingmaterial");
                            MaterialEntity.Attributes["tss_price"] = sItem.GetAttributeValue<Money>("trs_price");
                            MaterialEntity.Attributes["tss_quantity"] = sItem.GetAttributeValue<string>("trs_quantity");
                            MaterialEntity.Attributes["tss_totalprice"] = sItem.GetAttributeValue<Money>("trs_totalprice");
                            organizationService.Create(MaterialEntity);
                        }
                    }
                    #endregion

                    #region Insert ALL Sales Indicator to Quoatation Part Indicator
                    QueryExpression QueryIndicator = new QueryExpression("tss_indicator");
                    QueryIndicator.ColumnSet = new ColumnSet(true);

                    EntityCollection IndicatorItems = organizationService.RetrieveMultiple(QueryIndicator);
                    if (IndicatorItems.Entities.Count > 0)
                    {
                        foreach (Entity indicator in IndicatorItems.Entities)
                        {
                            Entity entity = new Entity("tss_quotationpartindicator");
                            entity.Attributes["tss_quotationpartheader"] = new EntityReference("tss_quotationpartheader", QuoationPartId);
                            entity.Attributes["tss_indicator"] = new EntityReference("tss_indicator", indicator.Id); //lookup sales indicator
                            entity.Attributes["tss_result"] = null;   //two option
                            entity.Attributes["tss_value"] = indicator.GetAttributeValue<int>("tss_value");   //whole number
                            organizationService.Create(entity);
                        }
                    }
                    #endregion

                    #region Insert all data from master reason to Quotation Part - Reason Discount/ Package (set result Blank)
                    QueryExpression QueryReason = new QueryExpression("tss_reason");
                    QueryReason.ColumnSet = new ColumnSet(true);

                    EntityCollection ReasonItems = organizationService.RetrieveMultiple(QueryReason);
                    if (ReasonItems.Entities.Count > 0)
                    {
                        foreach (Entity reason in ReasonItems.Entities)
                        {
                            Entity entity = new Entity("tss_quotationpartreasondiscountpackage");
                            entity.Attributes["tss_quotationpartheader"] = new EntityReference("tss_quotationpartheader", QuoationPartId);
                            //entity.Attributes["tss_reason"] = reason.GetAttributeValue<EntityReference>("tss_reason");   
                            entity.Attributes["tss_reason"] = new EntityReference("tss_reason", reason.Id);
                            entity.Attributes["tss_result"] = null;   //two option
                            entity.Attributes["tss_iscompetitor"] = reason.GetAttributeValue<bool>("tss_iscompetitor");
                            organizationService.Create(entity);
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".CopyEntityQuotation : " + ex.Message.ToString());
            }
        }

        */

        /*
        private static void CloneRecord(Entity sourcEntity, Entity target)
        {
            //string[] attributesToExclude = new string[]
            //{
            //        "modifiedon",
            //        "createdon",
            //        "statecode",
            //        //"tss_quotationid",
            //        "tss_statusreason",
            //        "tss_statuscode",
            //        "tss_revision",
            //        "tss_packageno",
            //        "tss_package",
            //        "tss_packagesname",
            //        "tss_packagedescription",
            //        "tss_totalexpectedpackageamount",
            //        "tss_approvepackage",
            //        //"tss_finalprice",
            //        "tss_totalfinalprice"
            //}; 

            string[] attributesToExcludeService = new string[]
            {
                    "modifiedon",
                    "createdon, trs_totalrtg, transactioncurrencyid,trs_discountheader, trs_supportingmaterialoption"
            };


            foreach (string attrName in sourcEntity.Attributes.Keys)
            {
                if (!attributesToExcludeService.Contains(attrName) && attrName.ToLower() != sourcEntity.LogicalName.ToLower() + "id")
                {
                    target.Attributes.Add(attrName, sourcEntity[attrName]);
                }
            }
        }
         * */
    }
}
