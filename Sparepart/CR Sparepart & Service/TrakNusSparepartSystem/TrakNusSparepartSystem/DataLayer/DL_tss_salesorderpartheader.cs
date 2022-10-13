using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Client;

namespace TrakNusSparepartSystem.DataLayer
{
    public class DL_tss_salesorderpartheader
    {
        #region Dependencies
        private DL_account _DL_account = new DL_account();
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        private DL_businessunit _DL_businessunit = new DL_businessunit();
        private DL_tss_prospectpartheader _DL_tss_prospectpartheader = new DL_tss_prospectpartheader();
        private DL_trs_quotation _DL_trs_quotation = new DL_trs_quotation();
        private DL_paymentterm _DL_paymentterm = new DL_paymentterm();
        private DL_contact _DL_contact = new DL_contact();
        private DL_UOM _DL_UOM = new DL_UOM();
        private DL_currency _DL_currency = new DL_currency();
        private DL_tss_salesorderpartlines _DL_tss_salesorderpartlines = new DL_tss_salesorderpartlines();
        #endregion

        #region Contants
        private const string _quotationPart = "tss_quotationpartheader";
        private const string _prospectPart = "tss_prospectpartheader";
        #endregion

        #region Properties
        private string _classname = "DL_tss_salesorderpartheader";

        private string _entityname = "tss_sopartheader";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Sales Order Part Header";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _tss_sonumber = false;
        private string _tss_sonumber_value;
        public string tss_sonumber
        {
            get { return _tss_sonumber ? _tss_sonumber_value : null; }
            set { _tss_sonumber = true; _tss_sonumber_value = value; }
        }

        private bool _tss_sotationid = false;
        private string _tss_sotationid_value;
        public string tss_sotationid
        {
            get { return _tss_sotationid ? _tss_sotationid_value : null; }
            set { _tss_sotationid = true; _tss_sotationid_value = value; }
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

        private bool _tss_quotationlink = false;
        private EntityReference _tss_quotationlink_value;
        public Guid tss_quotationlink
        {
            get { return _tss_quotationlink ? _tss_quotationlink_value.Id : Guid.Empty; }
            set { _tss_quotationlink = true; _tss_quotationlink_value = new EntityReference(_quotationPart, value); }
        }

        private bool _tss_prospectlink = false;
        private EntityReference _tss_prospectlink_value;
        public Guid tss_prospectlink
        {
            get { return _tss_prospectlink ? _tss_prospectlink_value.Id : Guid.Empty; }
            set { _tss_prospectlink = true; _tss_prospectlink_value = new EntityReference(_prospectPart, value); }
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

        private bool _tss_currency = false;
        private EntityReference _tss_currency_value;
        public Guid tss_currency
        {
            get { return _tss_currency ? _tss_currency_value.Id : Guid.Empty; }
            set { _tss_currency = true; _tss_currency_value = new EntityReference(_DL_currency.EntityName, value); }
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
        private bool _tss_statuscode = false;
        private int _tss_statuscode_value;
        public int tss_statuscode
        {
            get { return _tss_statuscode ? _tss_statuscode_value : int.MinValue; }
            set { _tss_statuscode = true; _tss_statuscode_value = value; }
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

        private bool _tss_totalamount = false;
        private Money _tss_totalamount_value;
        public decimal tss_totalamount
        {
            get { return _tss_totalamount ? _tss_totalamount_value.Value : 0; }
            set { _tss_totalamount = true; _tss_totalamount_value = new Money(value); }
        }

        //OptionSet
        private bool _tss_sourcetype = false;
        private int _tss_sourcetype_value;
        public int tss_sourcetype
        {
            get { return _tss_sourcetype ? _tss_sourcetype_value : int.MinValue; }
            set { _tss_sourcetype = true; _tss_sourcetype_value = value; }
        }

        //OptionSet
        private bool _tss_top = false;
        private int _tss_top_value;
        public int tss_top
        {
            get { return _tss_top ? _tss_top_value : int.MinValue; }
            set { _tss_top = true; _tss_top_value = value; }
        }

        //Two Option
        private bool _tss_package = false;
        private bool _tss_package_value;
        public bool tss_package
        {
            get { return _tss_package ? _tss_package_value : false; }
            set { _tss_package = true; _tss_package_value = value; }
        }

        private bool _tss_packageno = false;
        private string _tss_packageno_value;
        public string tss_packageno
        {
            get { return _tss_packageno ? _tss_packageno_value : string.Empty; }
            set { _tss_packageno = true; _tss_packageno_value = value; }
        }

        private bool _tss_packagename = false;
        private string _tss_packagename_value;
        public string tss_packagename
        {
            get { return _tss_packagename ? _tss_packagename_value : string.Empty; }
            set { _tss_packagename = true; _tss_packagename_value = value; }
        }

        private bool _tss_packagedescription = false;
        private string _tss_packagedescription_value;
        public string tss_packagedescription
        {
            get { return _tss_packagedescription ? _tss_packagedescription_value : string.Empty; }
            set { _tss_packagedescription = true; _tss_packagedescription_value = value; }
        }

        private bool _tss_packageqty = false;
        private int _tss_packageqty_value;
        public int tss_packageqty
        {
            get { return _tss_packageqty ? _tss_packageqty_value : int.MinValue; }
            set { _tss_packageqty = true; _tss_packageqty_value = value; }
        }

        private bool _tss_packageunit = false;
        private EntityReference _tss_packageunit_value;
        public Guid tss_packageunit
        {
            get { return _tss_packageunit ? _tss_packageunit_value.Id : Guid.Empty; }
            set { _tss_packageunit = true; _tss_packageunit_value = new EntityReference(_DL_UOM.EntityName, value); }
        }

        private bool _tss_paymentterm = false;
        private EntityReference _tss_paymentterm_value;
        public Guid tss_paymentterm
        {
            get { return _tss_paymentterm ? _tss_paymentterm_value.Id : Guid.Empty; }
            set { _tss_paymentterm = true; _tss_paymentterm_value = new EntityReference(_DL_paymentterm.EntityName, value); }
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
                throw new Exception("Error DL_tss_sopartheader.Select : " + ex.Message, ex.InnerException);
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
                throw new Exception("Error DL_tss_sopartheader.Select : " + ex.Message, ex.InnerException);
            }
        }

        public Guid CreateSalesOrder(IOrganizationService organizationService, ITracingService trace)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                if (_tss_branch) { entity.Attributes["tss_branch"] = _tss_branch_value; }
                if (_tss_contact) { entity.Attributes["tss_contact"] = _tss_contact_value; }
                if (_tss_customer) { entity.Attributes["tss_customer"] = _tss_customer_value; }
                if (_tss_package) { entity.Attributes["tss_package"] = _tss_package_value; }
                if (_tss_packagedescription) { entity.Attributes["tss_packagedescription"] = _tss_packagedescription_value; }
                if (_tss_packagename) { entity.Attributes["tss_packagesname"] = _tss_packagename_value; }
                if (_tss_packageno) { entity.Attributes["tss_packageno"] = _tss_packageno_value; }
                if (_tss_packageqty) { entity.Attributes["tss_packageqty"] = _tss_packageqty_value; }
                if (_tss_packageunit) { entity.Attributes["tss_packageunit"] = _tss_packageunit_value; }
                if (_tss_paymentterm) { entity.Attributes["tss_paymentterm"] = _tss_paymentterm_value; }
                if (_tss_prospectlink) { entity.Attributes["tss_prospectlink"] = _tss_prospectlink_value; }
                if (_tss_pss) { entity.Attributes["tss_pss"] = _tss_pss_value; }
                if (_tss_quotationlink) { entity.Attributes["tss_quotationlink"] = _tss_quotationlink_value; }
                if (_tss_requestdeliverydate) { entity.Attributes["tss_requestdeliverydate"] = _tss_requestdeliverydate_value; }
                if (_tss_sourcetype) { entity.Attributes["tss_sourcetype"] = new OptionSetValue(_tss_sourcetype_value); }
                if (_tss_statuscode) { entity.Attributes["tss_statecode"] = new OptionSetValue(_tss_statuscode_value); } //tss_statecode
                if (_tss_statusreason) { entity.Attributes["tss_statusreason"] = new OptionSetValue(_tss_statusreason_value); }
                if (_tss_top) { entity.Attributes["tss_top"] = new OptionSetValue(_tss_top_value); }
                if (_tss_totalamount) { entity.Attributes["tss_totalamount"] = _tss_totalamount_value; }
                return organizationService.Create(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".CreateSalesOrder : " + ex.Message.ToString());
            }
        }

        public void UpdateStatusSO(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                entity.Id = id;
                if (_tss_statusreason) { entity.Attributes["tss_statusreason"] = new OptionSetValue(_tss_statusreason_value); }
                if (_tss_statuscode) { entity.Attributes["tss_statecode"] = new OptionSetValue(_tss_statuscode_value); }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".UpdateStatusSO : " + ex.Message.ToString());
            }
        }

        public void UpdateTotalAmount(IOrganizationService organizationService, Guid id)
        {
            try
            {
                decimal totalLines = 0;
                var context = new OrganizationServiceContext(organizationService);
                var sopartlines = (from c in context.CreateQuery(_DL_tss_salesorderpartlines.EntityName)
                                   where c.GetAttributeValue<EntityReference>("tss_sopartheaderid").Id == id
                                   select c).ToList();

                foreach (var x in sopartlines)
                {
                    if (x.Attributes.Contains("tss_totalprice") && x.Attributes["tss_totalprice"] != null) totalLines += x.GetAttributeValue<Money>("tss_totalprice").Value;
                }

                Entity entity = new Entity(_entityname);
                entity.Id = id;
                entity.Attributes["tss_totalamount"] = new Money(totalLines);
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".UpdateTotalAmount : " + ex.Message.ToString());
            }
        }
    }
}
