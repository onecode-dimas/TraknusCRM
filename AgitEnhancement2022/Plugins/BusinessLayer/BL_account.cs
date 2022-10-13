using System;
using Microsoft.Xrm.Sdk;

namespace AgitEnhancement2022.Plugins.BusinessLayer
{
    class BL_account
    {
        #region Constants
        private const string _classname = "BL_account";

        private string _entityname_account = "account";
        private string _entityname_contact = "contact";

        private string _attrname_primarycontactid = "primarycontactid";
        private string _attrname_email_pic = "emailaddress1";
        private string _attrname_address_name = "address1_name";
        private string _attrname_street1 = "address1_line1";
        private string _attrname_city = "address1_city";
        private string _attrname_state_province = "address1_stateorprovince";
        private string _attrname_zip_postalcode = "address1_postalcode";
        private string _attrname_country_region = "address1_country";
        #endregion
        
        public void PostCreate_Account(IOrganizationService organizationservice, IPluginExecutionContext pluginExecutionContext, ITracingService tracer)
        {
            try
            {
                Entity entity = (Entity)pluginExecutionContext.InputParameters["Target"];
                Guid primaryContactId = entity.Attributes.Contains(_attrname_primarycontactid) ? entity.GetAttributeValue<EntityReference>(_attrname_primarycontactid).Id : new Guid();

                tracer.Trace("primaryContactId == new Guid(): {0}", primaryContactId == new Guid());
                if (primaryContactId == new Guid()) return;

                var emailPic = entity.GetAttributeValue<string>(_attrname_email_pic);
                var addressName = entity.GetAttributeValue<string>(_attrname_address_name);
                var street1 = entity.GetAttributeValue<string>(_attrname_street1);
                var city = entity.GetAttributeValue<string>(_attrname_city);
                var stateProvince = entity.GetAttributeValue<string>(_attrname_state_province);
                var postalCode = entity.GetAttributeValue<string>(_attrname_zip_postalcode);
                var countryRegion = entity.GetAttributeValue<string>(_attrname_country_region);

                Entity updateContact = new Entity(_entityname_contact);
                updateContact.Id = primaryContactId;
                updateContact["emailaddress1"] = emailPic;
                updateContact["address1_name"] = addressName;
                updateContact["address1_line1"] = street1;
                updateContact["address1_city"] = city;
                updateContact["address1_stateorprovince"] = stateProvince;
                updateContact["address1_postalcode"] = postalCode;
                updateContact["address1_country"] = countryRegion;
                organizationservice.Update(updateContact);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".PostCreate_Account: " + ex.Message.ToString());
            }
        }
    }
}
