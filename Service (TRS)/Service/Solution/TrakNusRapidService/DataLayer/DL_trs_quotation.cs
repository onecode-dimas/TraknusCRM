using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_quotation
    {
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

        public void Insert(IOrganizationService organizationService)
        {
            try
            {
                if (_trs_quotationnumber)
                {
                    Entity entity = new Entity(_entityname);
                    entity.Attributes["trs_quotationnumber"] = _trs_quotationnumber_value;
                    organizationService.Create(entity);
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
    }
}
