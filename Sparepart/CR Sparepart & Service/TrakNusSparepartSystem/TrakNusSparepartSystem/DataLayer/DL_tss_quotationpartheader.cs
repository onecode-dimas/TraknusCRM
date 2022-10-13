using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TrakNusSparepartSystem.DataLayer;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusSparepartSystem.DataLayer
{
    public class DL_tss_quotationpartheader
    {
        #region Dependencies
        private DL_account _DL_account = new DL_account();
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        private DL_businessunit _DL_businessunit = new DL_businessunit();
        private DL_tss_prospectpartheader _DL_tss_prospectpartheader = new DL_tss_prospectpartheader();
        private DL_trs_quotation _DL_trs_quotation = new DL_trs_quotation();
        private DL_contact _DL_contact = new DL_contact();
        private DL_currency _DL_currency = new DL_currency();
        #endregion

        #region Contants
        private const string _populationName = "new_population";
        #endregion

        #region Properties
        private string _classname = "DL_tss_quotationpartheader";

        private string _entityname = "tss_quotationpartheader";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Quotation Part Header";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _tss_quonumber = false;
        private string _tss_quonumber_value;
        public string tss_quonumber
        {
            get { return _tss_quonumber ? _tss_quonumber_value : null; }
            set { _tss_quonumber = true; _tss_quonumber_value = value; }
        }

        private bool _tss_quotationid = false;
        private string _tss_quotationid_value;
        public string tss_quotationid
        {
            get { return _tss_quotationid ? _tss_quotationid_value : null; }
            set { _tss_quotationid = true; _tss_quotationid_value = value; }
        }

        private bool _tss_customer = false;
        private EntityReference _tss_customer_value;
        public Guid tss_customer
        {
            get { return _tss_customer ? _tss_customer_value.Id : Guid.Empty; }
            set { _tss_customer = true; _tss_customer_value = new EntityReference(_DL_account.EntityName, value); }
        }

        private bool _tss_branch = false;
        private EntityReference _tss_branch_value;
        public Guid tss_branch
        {
            get { return _tss_branch ? _tss_branch_value.Id : Guid.Empty; }
            set { _tss_branch = true; _tss_branch_value = new EntityReference(_DL_businessunit.EntityName, value); }
        }

        private bool _tss_prospectlink = false;
        private EntityReference _tss_prospectlink_value;
        public Guid tss_prospectlink
        {
            get { return _tss_prospectlink ? _tss_prospectlink_value.Id : Guid.Empty; }
            set { _tss_prospectlink = true; _tss_prospectlink_value = new EntityReference(_DL_tss_prospectpartheader.EntityName, value); }
        }

        private bool _tss_quotserviceno = false;
        private EntityReference _tss_quotserviceno_value;
        public Guid tss_quotserviceno
        {
            get { return _tss_quotserviceno ? _tss_quotserviceno_value.Id : Guid.Empty; }
            set { _tss_quotserviceno = true; _tss_quotserviceno_value = new EntityReference(_DL_trs_quotation.EntityName, value); }
        }

        private bool _tss_estimationclosedate = false;
        private DateTime _tss_estimationclosedate_value;
        public DateTime tss_estimationclosedate
        {
            get { return _tss_estimationclosedate ? _tss_estimationclosedate_value.ToLocalTime() : DateTime.MinValue; }
            set { _tss_estimationclosedate = true; _tss_estimationclosedate_value = value.ToLocalTime(); }
        }

        private bool _tss_requestdeliverydate = false;
        private DateTime _tss_requestdeliverydate_value;
        public DateTime tss_requestdeliverydate
        {
            get { return _tss_requestdeliverydate ? _tss_requestdeliverydate_value.ToLocalTime() : DateTime.MinValue; }
            set { _tss_requestdeliverydate = true; _tss_requestdeliverydate_value = value.ToLocalTime(); }
        }

        private bool _tss_pss = false;
        private EntityReference _tss_pss_value;
        public Guid tss_pss
        {
            get { return _tss_pss ? _tss_pss_value.Id : Guid.Empty; }
            set { _tss_pss = true; _tss_pss_value = new EntityReference(_DL_systemuser.EntityName, value); }
        }

        //OptionSet
        private bool _tss_sourcetype= false;
        private int _tss_sourcetype_value;
        public int tss_sourcetype
        {
            get { return _tss_sourcetype ? _tss_sourcetype_value : int.MinValue; }
            set { _tss_sourcetype = true; _tss_sourcetype_value = value; }
        }

        //OptionSet
        private bool _tss_quotationstatus = false;
        private int _tss_quotationstatus_value;
        public int tss_quotationstatus
        {
            get { return _tss_quotationstatus ? _tss_quotationstatus_value : int.MinValue; }
            set { _tss_quotationstatus = true; _tss_quotationstatus_value = value; }
        }

        //OptionSet
        private bool _tss_statusreason = false;
        private int _tss_statusreason_value;
        public int tss_statusreason
        {
            get { return _tss_statusreason ? _tss_statusreason_value : int.MinValue; }
            set { _tss_statusreason = true; _tss_statusreason_value = value; }
        }

        private bool _tss_revision = false;
        private int _tss_revision_value;
        public int tss_revision
        {
            get { return _tss_revision ? _tss_revision_value : int.MinValue; }
            set { _tss_revision = true; _tss_revision_value = value; }
        }

        private bool _tss_contact = false;
        private EntityReference _tss_contact_value;
        public Guid tss_contact
        {
            get { return _tss_contact ? _tss_contact_value.Id : Guid.Empty; }
            set { _tss_contact = true; _tss_contact_value = new EntityReference(_DL_contact.EntityName, value); }
        }

        private bool _tss_currency = false;
        private EntityReference _tss_currency_value;
        public Guid tss_currency
        {
            get { return _tss_currency ? _tss_currency_value.Id : Guid.Empty; }
            set { _tss_currency = true; _tss_currency_value = new EntityReference(_DL_currency.EntityName, value); }
        }

        private bool _tss_totalprice = false;
        private Money _tss_totalprice_value;
        public decimal tss_totalprice
        {
            get { return _tss_totalprice ? _tss_totalprice_value.Value : 0; }
            set { _tss_totalprice = true; _tss_totalprice_value = new Money(value); }
        }

        private bool _tss_totalfinalprice = false;
        private Money _tss_totalfinalprice_value;
        public decimal tss_totalfinalprice
        {
            get { return _tss_totalfinalprice ? _tss_totalfinalprice_value.Value : 0; }
            set { _tss_totalfinalprice = true; _tss_totalfinalprice_value = new Money(value); }
        }

        private bool _tss_totalconfidencelevel = false;
        private int _tss_totalconfidencelevel_value;
        public int tss_totalconfidencelevel
        {
            get { return _tss_totalconfidencelevel ? _tss_totalconfidencelevel_value : int.MinValue; }
            set { _tss_totalconfidencelevel = true; _tss_totalconfidencelevel_value = value; }
        }

        private bool _tss_activedate = false;
        private DateTime _tss_activedate_value;
        public DateTime tss_activedate
        {
            get { return _tss_activedate ? _tss_activedate_value.ToLocalTime() : DateTime.MinValue; }
            set { _tss_activedate = true; _tss_activedate_value = value.ToLocalTime(); }
        }

        private bool _tss_packageno = false;
        private string _tss_packageno_value;
        public string tss_packageno
        {
            get { return _tss_packageno ? _tss_packageno_value : null; }
            set { _tss_packageno = true; _tss_packageno_value = value; }
        }

        //two option
        //private bool _tss_package = false;
        //private string _tss_package_value;
        //public string tss_package
        //{
        //    get { return _tss_package ? _tss_package_value : null; }
        //    set { _tss_package = true; _tss_package_value = value; }
        //}

        private bool _tss_packagedescription = false;
        private string _tss_packagedescription_value;
        public string tss_packagedescription
        {
            get { return _tss_packagedescription ? _tss_packagedescription_value : null; }
            set { _tss_packagedescription = true; _tss_packagedescription_value = value; }
        }

        //updated by indra - field optionset tss_top
        private bool _tss_top = false;
        private int _tss_top_value;
        public int tss_top
        {
            get { return _tss_top ? _tss_top_value : int.MinValue; }
            set { _tss_top = true; _tss_top_value = value; }
        }

        //OptionSet
        private bool _tss_requestnewtop = false;
        private int? _tss_requestnewtop_value;
        public int? tss_requestnewtop
        {
            get { return _tss_requestnewtop ? _tss_requestnewtop_value : int.MinValue; }
            set { _tss_requestnewtop = true; _tss_requestnewtop_value = value; }
        }
                    
        //two option
        private bool _tss_approvenewtop = false;
        private bool _tss_approvenewtop_value;
        public bool tss_approvenewtop
        {
            get { return _tss_approvenewtop ? _tss_approvenewtop_value : false; }
            set { _tss_approvenewtop = true; _tss_approvenewtop_value = value; }
        }

        private bool _tss_approvetopby = false;
        private EntityReference _tss_approvetopby_value;
        public Guid tss_approvetopby
        {
            get { return _tss_approvetopby ? _tss_approvetopby_value.Id : Guid.Empty; }
            set { _tss_approvetopby = true; _tss_approvetopby_value = new EntityReference(_DL_systemuser.EntityName, value); }
        }

        private bool _tss_unit = false;
        private EntityReference _tss_unit_value;
        public Guid tss_unit
        {
            get { return _tss_unit ? _tss_unit_value.Id : Guid.Empty; }
            set { _tss_unit = true; _tss_unit_value = new EntityReference(_populationName, value); }
        }

        //two option
        private bool _tss_approvediscount = false;
        private bool _tss_approvediscount_value;
        public bool tss_approvediscount
        {
            get { return _tss_approvediscount ? _tss_approvediscount_value : false; }
            set { _tss_approvediscount = true; _tss_approvediscount_value = value; }
        }

        private bool _tss_isnewcustomer = false;
        private bool _tss_isnewcustomer_value;
        public bool tss_isnewcustomer
        {
            get { return _tss_isnewcustomer ? _tss_isnewcustomer_value : false; }
            set { _tss_isnewcustomer = true; _tss_isnewcustomer_value = value; }
        }

        private bool _tss_approvediscountby = false;
        private EntityReference _tss_approvediscountby_value;
        public Guid tss_approvediscountby
        {
            get { return _tss_approvediscountby ? _tss_approvediscountby_value.Id : Guid.Empty; }
            set { _tss_approvediscountby = true; _tss_approvediscountby_value = new EntityReference(_DL_systemuser.EntityName, value); }
        }

        private bool _tss_underminprice = false;
        private bool _tss_underminprice_value;
        public bool tss_underminprice
        {
            get { return _tss_underminprice ? _tss_underminprice_value : false; }
            set { _tss_underminprice = true; _tss_underminprice_value = value; }
        }

        private bool _tss_discountdatetime = false;
        private DateTime _tss_discountdatetime_value;
        public DateTime tss_discountdatetime
        {
            get { return _tss_discountdatetime ? _tss_discountdatetime_value.ToLocalTime() : DateTime.MinValue; }
            set { _tss_discountdatetime = true; _tss_discountdatetime_value = value.ToLocalTime(); }
        }

        private bool _tss_discountcurrentapprover = false;
        private EntityReference _tss_discountcurrentapprover_value;
        public Guid tss_discountcurrentapprover
        {
            get { return _tss_discountcurrentapprover ? _tss_discountcurrentapprover_value.Id : Guid.Empty; }
            set { _tss_discountcurrentapprover = true; _tss_discountcurrentapprover_value = new EntityReference(_DL_systemuser.EntityName, value); }
        }

        private bool _tss_salesorderno = false;
        private EntityReference _tss_salesorderno_value;
        public Guid tss_salesorderno
        {
            get { return _tss_salesorderno ? _tss_salesorderno_value.Id : Guid.Empty; }
            set { _tss_salesorderno = true; _tss_salesorderno_value = new EntityReference("tss_sopartheader", value); }
        }

        private bool _tss_salesorderreference = false;
        private EntityReference _tss_salesorderreference_value;
        public Guid tss_salesorderreference
        {
            get { return _tss_salesorderreference ? _tss_salesorderreference_value.Id : Guid.Empty; }
            set { _tss_salesorderreference = true; _tss_salesorderreference_value = new EntityReference("tss_sopartheader", value); }
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
                throw new Exception("Error DL_tss_quotationpartheader.Select : " + ex.Message, ex.InnerException);
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
                throw new Exception("Error DL_tss_quotationpartheader.Select : " + ex.Message, ex.InnerException);
            }
        }

        public void UpdateQuotationFromProspect(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = Select(organizationService, id);
                if (_tss_statusreason) { entity.Attributes["tss_statusreason"] = new OptionSetValue(_tss_statusreason_value); }
                if (_tss_quotationstatus) { entity.Attributes["tss_statuscode"] = new OptionSetValue(_tss_quotationstatus_value); }
                if (_tss_customer) { entity.Attributes["tss_customer"] = _tss_customer_value; }
                if (_tss_prospectlink) { entity.Attributes["tss_prospectlink"] = _tss_prospectlink_value; }
                if (_tss_estimationclosedate) { entity.Attributes["tss_estimationclosedate"] = _tss_estimationclosedate_value; }
                if (_tss_requestdeliverydate) { entity.Attributes["tss_requestdeliverydate"] = _tss_requestdeliverydate_value; }
                if (_tss_contact) { entity.Attributes["tss_contact"] = _tss_contact_value; }
                if (_tss_currency) { entity.Attributes["tss_currency"] = _tss_currency_value; }
                if (_tss_totalprice) { entity.Attributes["tss_totalprice"] = _tss_totalprice_value; }
                if (_tss_branch) { entity.Attributes["tss_branch"] = _tss_branch_value; }
                if (_tss_pss) { entity.Attributes["tss_pss"] = _tss_pss_value; }
                if (_tss_top) { entity.Attributes["tss_top"] = new OptionSetValue(_tss_top_value); }
                if (_tss_sourcetype) { entity.Attributes["tss_sourcetype"] = new OptionSetValue(_tss_sourcetype_value); }
                if (_tss_isnewcustomer) { entity.Attributes["tss_isnewcustomer"] = _tss_isnewcustomer_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".CreateQuotationFromProspect : " + ex.Message.ToString());
            }
        }

        public Guid CreateQuotationFromProspect(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                if (_tss_statusreason) { entity.Attributes["tss_statusreason"] = new OptionSetValue(_tss_statusreason_value); }
                if (_tss_quotationstatus) { entity.Attributes["tss_statuscode"] = new OptionSetValue(_tss_quotationstatus_value); }
                if (_tss_customer) { entity.Attributes["tss_customer"] = _tss_customer_value; }
                if (_tss_prospectlink) { entity.Attributes["tss_prospectlink"] = _tss_prospectlink_value; }
                if (_tss_estimationclosedate) { entity.Attributes["tss_estimationclosedate"] = _tss_estimationclosedate_value; }
                if (_tss_contact) { entity.Attributes["tss_contact"] = _tss_contact_value; }
                if (_tss_currency) { entity.Attributes["tss_currency"] = _tss_currency_value; }
                if (_tss_totalprice) { entity.Attributes["tss_totalprice"] = _tss_totalprice_value; }
                if (_tss_branch) { entity.Attributes["tss_branch"] = _tss_branch_value; }
                if (_tss_pss) { entity.Attributes["tss_pss"] = _tss_pss_value; }
                if (_tss_unit) { entity.Attributes["tss_unit"] = _tss_unit_value; }
                //updated by indra - set TOP value
                if (_tss_top) { entity.Attributes["tss_top"] = new OptionSetValue(_tss_top_value); }
                if (_tss_underminprice) { entity.Attributes["tss_underminimumprice"] = _tss_underminprice_value; }
                if (_tss_sourcetype) { entity.Attributes["tss_sourcetype"] = new OptionSetValue(_tss_sourcetype_value); }
                if (_tss_isnewcustomer) { entity.Attributes["tss_isnewcustomer"] = _tss_isnewcustomer_value; }
                return organizationService.Create(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".CreateQuotationFromProspect : " + ex.Message.ToString());
            }
        }

        public void FinalQuotation(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                entity.Id = id;
                if (_tss_quotationstatus) { entity.Attributes["tss_statuscode"] = new OptionSetValue(_tss_quotationstatus_value); }
                if (_tss_statusreason) { entity.Attributes["tss_statusreason"] = new OptionSetValue(_tss_statusreason_value); }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".FinalQuotation : " + ex.Message.ToString());
            }
        }

        public void ReviseProspect(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                entity.Id = id;
                if (_tss_quotationstatus) { entity.Attributes["tss_statuscode"] = new OptionSetValue(_tss_quotationstatus_value); }
                if (_tss_statusreason) { entity.Attributes["tss_statusreason"] = new OptionSetValue(_tss_statusreason_value); }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".ReviseProspect : " + ex.Message.ToString());
            }
        }

        public void RevisedQuotation(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                entity.Id = id;
                if (_tss_statusreason) { entity.Attributes["tss_statusreason"] = new OptionSetValue(_tss_statusreason_value); }
                if (_tss_quotationstatus) { entity.Attributes["tss_statuscode"] = new OptionSetValue(_tss_quotationstatus_value); }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".UpdateQuotation : " + ex.Message.ToString());
            }
        }

        public void CreateQuotationRevise(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                entity.Attributes["tss_quotationnumber"] = _tss_quonumber_value;
                entity.Attributes["tss_quotationid"] = _tss_quonumber_value;
                if (_tss_quotationstatus) { entity.Attributes["tss_statusreason"] = new OptionSetValue(_tss_statusreason_value); }
                if (_tss_statusreason) { entity.Attributes["tss_statuscode"] = new OptionSetValue(_tss_statusreason_value); }
                entity.Attributes["tss_revision"] = _tss_revision_value;
                entity.Attributes["tss_packageno"] = _tss_packageno_value;
                entity.Attributes["tss_package"] = false;  //set No from two option
                entity.Attributes["tss_packagedescription"] = _tss_packagedescription_value;
                entity.Attributes["tss_totalprice"] = _tss_totalprice_value;
                entity.Attributes["tss_totalfinalprice"] = _tss_totalfinalprice_value;
                
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".UpdateQuotation : " + ex.Message.ToString());
            }
        }

        //public void CopyEntityQuotation(IOrganizationService organizationService, string entityName, Guid id)
        //{
        //    try
        //    {
        //        var QuotationPartHeader = organizationService.Retrieve(entityName, id, new ColumnSet(true));

        //        Entity target = new Entity(QuotationPartHeader.LogicalName);
        //        if (QuotationPartHeader != null && QuotationPartHeader.Attributes.Count > 0)
        //        {
        //            CloneRecord(QuotationPartHeader, target);  //clone header

        //            Guid clonedQuoId = organizationService.Create(target);
        //            //ClonedQuotation.Set(executionContext, new EntityReference("tss_quotationpartheader", clonedOppId));

        //            //get quotation part lines
        //            QueryExpression QueryLines = new QueryExpression("tss_quotationpartlines");
        //            QueryLines.ColumnSet = new ColumnSet(true);
        //            QueryLines.Criteria.AddCondition("tss_quotationpartheader", ConditionOperator.Equal, QuotationPartHeader.Id);

        //            EntityCollection lineItems = organizationService.RetrieveMultiple(QueryLines);
        //            if (lineItems.Entities.Count > 0)
        //            {
        //                foreach (Entity sItem in lineItems.Entities)
        //                {
        //                    Entity tItem = new Entity(sItem.LogicalName);
        //                    CloneRecord(sItem, tItem);   //clone line
        //                    tItem.Attributes["tss_quotationpartheader"] = new EntityReference("tss_quotationpartheader", clonedQuoId);

        //                    Guid CloneLinesGuid = organizationService.Create(tItem);

        //                    #region Update TotalPrice and TotalFinalPrice in Quotation Part Lines

        //                    //Guid CloneLinesGuid = organizationService.Create(tItem);  //get guid clone record Lines
        //                    Entity ChildEntity = organizationService.Retrieve("tss_quotationpartlines", CloneLinesGuid, new ColumnSet(true));
        //                    ChildEntity.Attributes["tss_finalprice"] = null;
        //                    ChildEntity.Attributes["tss_totalfinalprice"] = null;
        //                    ChildEntity.Attributes["tss_approvebypa"] = false;
        //                    ChildEntity.Attributes["tss_approvefinalpriceby"] = null;

        //                    organizationService.Update(ChildEntity);

        //                    #endregion
        //                }
        //            }

        //            #region Update Quotation Part Header

        //            var revision = QuotationPartHeader.GetAttributeValue<int>("tss_revision");
        //                if (revision == null)
        //                    revision = 0;
        //                revision += 1;

        //            Entity entity = organizationService.Retrieve(entityName, clonedQuoId, new ColumnSet(true));  //using new record  header
        //            //Entity entity = new Entity(_entityname);
        //            entity.Attributes["tss_quotationnumber"] = QuotationPartHeader.GetAttributeValue<string>("tss_quotationnumber"); //update using old record
        //            //entity.Attributes["tss_quotationid"] = QuotationPartHeader.GetAttributeValue<string>("tss_quotationid"); //increament +1
        //            entity.Attributes["tss_statusreason"] = new OptionSetValue(865920000); //open
        //            entity.Attributes["tss_statuscode"] = new OptionSetValue(865920000); //  draft
        //            entity.Attributes["tss_revision"] = revision;
        //            entity.Attributes["tss_package"] = false;  //set No from two option
        //            entity.Attributes["tss_packageno"] = string.Empty;
        //            entity.Attributes["tss_packagesname"] = string.Empty;
        //            entity.Attributes["tss_packagedescription"] = string.Empty;
        //            entity.Attributes["tss_totalexpectedpackageamount"] = null;
        //            entity.Attributes["tss_approvepackage"] = false;
        //            //entity.Attributes["tss_finalprice"] = null;
        //            entity.Attributes["tss_totalfinalprice"] = null;   

        //            organizationService.Update(entity);

        //            #endregion 
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(_classname + ".UpdateQuotation : " + ex.Message.ToString());
        //    }
        //}

        //private static void CloneRecord(Entity sourcEntity, Entity target)
        //{
        //    string[] attributesToExclude = new string[]
        //    {
        //            "modifiedon",
        //            "createdon",
        //            "statecode",
        //            //"tss_quotationid",
        //            "tss_statusreason",
        //            "tss_statuscode",
        //            "tss_revision",
        //            "tss_packageno",
        //            "tss_package",
        //            "tss_packagesname",
        //            "tss_packagedescription",
        //            "tss_totalexpectedpackageamount",
        //            "tss_approvepackage",
        //            //"tss_finalprice",
        //            "tss_totalfinalprice"
        //    };

        //    //string[] attributesToExclude = new string[]
        //    //{
        //    //        "modifiedon",
        //    //        "createdon"
        //    //};


        //    foreach (string attrName in sourcEntity.Attributes.Keys)
        //    {
        //        if (!attributesToExclude.Contains(attrName) && attrName.ToLower() != sourcEntity.LogicalName.ToLower() + "id")
        //        {
        //            target.Attributes.Add(attrName, sourcEntity[attrName]);
        //        }
        //    }
        //}

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

        public void UpdateStatusAfterCreateSO(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                entity.Id = id;
                if (_tss_statusreason) { entity.Attributes["tss_statusreason"] = new OptionSetValue(_tss_statusreason_value); }
                if (_tss_quotationstatus) { entity.Attributes["tss_statuscode"] = new OptionSetValue(_tss_quotationstatus_value); }
                if (_tss_salesorderreference) { entity.Attributes["tss_salesorderreference"] = _tss_salesorderreference_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".UpdateStatusAfterCreateSO : " + ex.Message.ToString());
            }
        }

        public void UpdateApprovalReason(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                entity.Id = id;
                if (_tss_discountdatetime) { entity.Attributes["tss_discountdatetime"] = _tss_discountdatetime_value; }
                if (_tss_discountcurrentapprover) { entity.Attributes["tss_discountcurrentapprover"] = _tss_discountcurrentapprover_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".UpdateApprovalReason : " + ex.Message.ToString());
            }
        }

        public void ApproveTopQuotation(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                entity.Id = id;
                if (_tss_top) { entity.Attributes["tss_top"] = new OptionSetValue(_tss_top_value); }
                if (_tss_approvenewtop) { entity.Attributes["tss_approvenewtop"] = _tss_approvenewtop_value; }
                if (_tss_approvetopby) { entity.Attributes["tss_approvetopby"] = _tss_approvetopby_value; }
                if (_tss_quotationstatus) { entity.Attributes["tss_statuscode"] = new OptionSetValue(_tss_quotationstatus_value); }
                if (_tss_statusreason) { entity.Attributes["tss_statusreason"] = new OptionSetValue(_tss_statusreason_value); }

                if (_tss_requestnewtop)
                {
                    if (_tss_requestnewtop_value != null)
                        entity.Attributes["tss_requestnewtop"] = new OptionSetValue(_tss_requestnewtop_value.Value);
                    else
                        entity.Attributes["tss_requestnewtop"] = null;
                }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".UpdateQuotation : " + ex.Message.ToString());
            }
        }

        public void ApproveDiscountQuotation(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                entity.Id = id;
                if (_tss_approvediscount) { entity.Attributes["tss_approvediscount"] = _tss_approvediscount_value; }
                if (_tss_approvediscountby) { entity.Attributes["tss_approvediscountby"] = _tss_approvediscountby_value; }
                if (_tss_quotationstatus) { entity.Attributes["tss_statuscode"] = new OptionSetValue(_tss_quotationstatus_value); }
                if (_tss_statusreason) { entity.Attributes["tss_statusreason"] = new OptionSetValue(_tss_statusreason_value); }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".ApproveDiscountQuotation : " + ex.Message.ToString());
            }
        }
    }
}
