using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Reflection;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using TrakNusRapidService.DataLayer;
using TrakNusRapidService.Helper.MobileWebService;

namespace TrakNusRapidService.Helper
{
    public class FMobile
    {
        #region Constants
        private const string _classname = "FMobile";
        #endregion
        
        #region Depedencies
        private DL_account _DL_account = new DL_account();
        private DL_contact _DL_contact = new DL_contact();
        private DL_customeraddress _DL_customeraddress = new DL_customeraddress();
        private DL_equipment _DL_equipment = new DL_equipment();
        private DL_new_population _DL_new_population = new DL_new_population();
        private DL_trs_mtar _DL_trs_mtar = new DL_trs_mtar();
        private DL_trs_ppmreport _DL_trs_ppmreport = new DL_trs_ppmreport();
        private DL_trs_technicalservicereport _DL_trs_technicalservicereport = new DL_trs_technicalservicereport();
        private DL_activityparty _DL_activityparty = new DL_activityparty();
        private DL_serviceappointment _DL_serviceappointment = new DL_serviceappointment();
        private DL_trs_workorderpartssummary _DL_trs_workorderpartssummary = new DL_trs_workorderpartssummary();
        private DL_trs_workordersupportingmaterial _DL_trs_workordersupportingmaterial = new DL_trs_workordersupportingmaterial();
        private DL_task _DL_task = new DL_task();
        private DL_trs_commercialdetail _DL_trs_commercialdetail = new DL_trs_commercialdetail();
        private DL_trs_commercialdetailmechanic _DL_trs_commercialdetailmechanic = new DL_trs_commercialdetailmechanic();
        private DL_activitypointer _DL_activitypointer = new DL_activitypointer();
        private DL_trs_workordersecondman _DL_trs_workordersecondman = new DL_trs_workordersecondman();
        private DL_trs_functionallocation _DL_trs_functionallocation = new DL_trs_functionallocation();
        private DL_trs_producttype _DL_trs_producttype = new DL_trs_producttype();
        private DL_trs_productsection _DL_trs_productsection = new DL_trs_productsection();
        private DL_trs_tsrpartsdamageddetail _DL_trs_tsrpartsdamageddetail = new DL_trs_tsrpartsdamageddetail();
        private DL_trs_tsrpartdetails _DL_trs_tsrpartdetails = new DL_trs_tsrpartdetails();
        private DL_product _DL_product = new DL_product();
        private DL_trs_masterpart _DL_trs_masterpart = new DL_trs_masterpart();
        private DL_trs_tasklist _DL_trs_tasklist = new DL_trs_tasklist();
        private DL_trs_tasklistgroup _DL_trs_tasklistgroup = new DL_trs_tasklistgroup();
        private DL_trs_tasklistheader _DL_trs_tasklistheader = new DL_trs_tasklistheader();
        private DL_trs_commercialtask _DL_trs_commercialtask = new DL_trs_commercialtask();
        private DL_trs_tasklistdetailpart _DL_trs_tasklistdetailpart = new DL_trs_tasklistdetailpart();
        private DL_incident _DL_incident = new DL_incident();
        private DL_trs_mechanicgrade _DL_trs_mechanicgrade = new DL_trs_mechanicgrade();
        private DL_trs_workorderpartrecommendation _DL_trs_workorderpartrecommendation = new DL_trs_workorderpartrecommendation();
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        private DL_trs_workflowconfiguration _DL_trs_workflowconfiguration = new DL_trs_workflowconfiguration();
        private DL_site _DL_site = new DL_site();
        private DL_trs_technicalservicereportdocumentation _DL_trs_technicalservicereportdocumentation = new DL_trs_technicalservicereportdocumentation();
        private DL_trs_sectionrecommendation _DL_trs_sectionrecommendation = new DL_trs_sectionrecommendation();
        private DL_businessunit _DL_businessunit = new DL_businessunit();
        private DL_trs_unittypeofwork _DL_trs_unittypeofwork = new DL_trs_unittypeofwork();
        private DL_trs_workorderdocumentation _DL_trs_workorderdocumentation = new DL_trs_workorderdocumentation();
        private DL_trs_mechanicmonthlypoint _DL_trs_mechanicmonthlypoint = new DL_trs_mechanicmonthlypoint();

        private LogCreator _logCreator;
        #endregion

        #region Properties
        private MobileServiceSoapClient _client = null;

        private Guid? _customerId;
        private Guid? _mechanicLeaderId;
        private List<Guid> _mechanicSecondmanId;
        private string _webserviceurl = null;
        private string _organization = null;
        #endregion

        #region Privates
        private string GetMobileWebServiceUri(IOrganizationService organizationService)
        {
            try
            {
                if (_webserviceurl == null)
                {
                    QueryExpression queryExpression = new QueryExpression(_DL_trs_workflowconfiguration.EntityName);
                    queryExpression.ColumnSet = new ColumnSet(true);
                    queryExpression.Criteria.AddCondition("trs_generalconfig", ConditionOperator.Equal, Configuration.ConfigurationName);

                    EntityCollection entityCollection = _DL_trs_workflowconfiguration.Select(organizationService, queryExpression);
                    if (entityCollection.Entities.Count > 0)
                    {
                        Entity entity = entityCollection.Entities[0];
                        if (entity.Attributes.Contains("trs_mobilewebservice"))
                            _webserviceurl = entity.GetAttributeValue<string>("trs_mobilewebservice");
                        else
                            throw new Exception("Please fill uri for mobile web service !");
                        if (entity.Attributes.Contains("trs_logfoldername"))
                            _organization = entity.GetAttributeValue<string>("trs_logfoldername");
                        else
                            throw new Exception("Please fill log folder name !");
                    }
                    else
                    {
                        throw new Exception("Can not found configuration with name 'TRS'.");
                    }
                }
                return _webserviceurl;
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".GetMobileWebServiceUri : " + ex.Message);
            }
        }

        private void Connect(IOrganizationService organizationService, string caller, Guid id)
        {
            try
            {
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + caller + _logCreator.ColumnsSeparator
                    + id.ToString() + _logCreator.ColumnsSeparator
                    + "Configuring Connection ...");

                EndpointAddress endpointAddress = new EndpointAddress(_webserviceurl);
                BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
                _client = new MobileServiceSoapClient(basicHttpBinding, endpointAddress);
                _client.InnerChannel.OperationTimeout = new TimeSpan(1, 0, 0);

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + caller + _logCreator.ColumnsSeparator
                    + id.ToString() + _logCreator.ColumnsSeparator
                    + "Configuring Connection Done.");
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Connect : " + ex.Message.ToString());
            }
        }

        private void ErrorHandler(SynchronizationResult synchronizeResult, string entityName, string displayName, Guid id)
        {
            string errCode = DateTime.Now.ToString("yyyyMMddHHmmssFFFFFFF");
            throw new Exception("Failed to Synchronize " + displayName + " with id : " + id.ToString() + ". (" + errCode + ")" + " | " + synchronizeResult.StackTrace);
        }

        private void ErrorMessage_NotFound(string fieldName)
        {
            throw new Exception("Can not found '" + fieldName + "'");
        }

        private void GetSecondmanList(IOrganizationService organizationService, Guid activityId)
        {
            _mechanicSecondmanId = new List<Guid>();

            QueryExpression queryExpression = new QueryExpression(_DL_trs_workordersecondman.EntityName);
            queryExpression.ColumnSet = new ColumnSet(true);
            queryExpression.Criteria.AddCondition("trs_activityid", ConditionOperator.Equal, activityId);

            EntityCollection entityCollection = _DL_trs_workordersecondman.Select(organizationService, queryExpression);
            foreach (Entity entity in entityCollection.Entities)
            {
                _mechanicSecondmanId.Add(entity.GetAttributeValue<EntityReference>("trs_equipmentid").Id);
            }
        }

        private int GetServiceAppointment_MechanicRole(IOrganizationService organizationService, Guid activityId, Guid equipmentId)
        {
            if (equipmentId == _mechanicLeaderId)
                return Configuration.MechanicRole_Leader;
            else
            {
                if (_mechanicSecondmanId.Exists(x => x == equipmentId))
                    return Configuration.MechanicRole_Secondman;
                else
                    return Configuration.MechanicRole_Member;
            }
        }
        #endregion

        #region Publics
        public FMobile(IOrganizationService organizationService)
        {
            try
            {
                GetMobileWebServiceUri(organizationService);
                _logCreator = new LogCreator(_organization);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + " : " + ex.Message);
            }
        }

        public void SendAllMaster(IOrganizationService organizationService)
        {
            SendAllAccount(organizationService);
            SendAllContact(organizationService);
            SendAllMechanic(organizationService);
            SendAllParts(organizationService);

            SendAllProduct(organizationService);
            SendAllPopulation(organizationService);
        }

        #region Account
        public void SendAllAccount(IOrganizationService organizationService)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_account.EntityName);
                EntityCollection eCollection = _DL_account.Select(organizationService, queryExpression);
                foreach (Entity entity in eCollection.Entities)
                {
                    SendAccount(organizationService, entity.Id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendAllAccount : " + ex.Message);
            }
        }

        public void SendAccount(IOrganizationService organizationService, Guid id)
        {
            try
            {
                SynchronizationResult synchronizationResult = new SynchronizationResult();
                Entity entity = _DL_account.Select(organizationService, id);
                AccountBase mAccount = new AccountBase();
                if (entity.Attributes.Contains("accountid"))
                    mAccount.AccountId = entity.Id;
                else
                    ErrorMessage_NotFound("accountid");
                if (entity.Attributes.Contains("name"))
                    mAccount.Name = entity.GetAttributeValue<string>("name");
                else
                    ErrorMessage_NotFound("name");
                if (entity.Attributes.Contains("modifiedon"))
                    mAccount.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                else
                    ErrorMessage_NotFound("modifiedon");
                if (entity.Attributes.Contains("statecode"))
                    mAccount.StateCode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                else
                    ErrorMessage_NotFound("statecode");
                if (entity.Attributes.Contains("statuscode"))
                    mAccount.StatusCode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                else
                    ErrorMessage_NotFound("statuscode");

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                synchronizationResult = _client.SaveEntity(mAccount);
                _client.Close();

                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_account.EntityName, _DL_account.DisplayName, id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendAccount : " + ex.Message);
            }
        }
        #endregion

        #region Contact
        public void SendAllContact(IOrganizationService organizationService)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_contact.EntityName);
                EntityCollection eCollection = _DL_contact.Select(organizationService, queryExpression);
                foreach (Entity entity in eCollection.Entities)
                {
                    SendContact(organizationService, entity.Id);
                    SendContactAddress(organizationService, entity.Id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendAllContact : " + ex.Message);
            }
        }

        public void SendContact(IOrganizationService organizationService, Guid id)
        {
            try
            {
                SynchronizationResult synchronizationResult = new SynchronizationResult();
                Entity entity = _DL_contact.Select(organizationService, id);
                ContactBase mContact = new ContactBase();
                if (entity.Attributes.Contains("contactid"))
                    mContact.ContactId = entity.Id;
                else
                    ErrorMessage_NotFound("contactid");
                if (entity.Attributes.Contains("fullname"))
                    mContact.FullName = entity.GetAttributeValue<string>("fullname");
                else
                    ErrorMessage_NotFound("fullname");
                if (entity.Attributes.Contains("modifiedon"))
                    mContact.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                else
                    ErrorMessage_NotFound("modifiedon");
                if (entity.Attributes.Contains("statecode"))
                    mContact.StateCode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                else
                    ErrorMessage_NotFound("statecode");
                if (entity.Attributes.Contains("statuscode"))
                    mContact.StatusCode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                else
                    ErrorMessage_NotFound("statuscode");

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                synchronizationResult = _client.SaveEntity(mContact);
                _client.Close();

                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_contact.EntityName, _DL_contact.DisplayName, id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendContact : " + ex.Message);
            }
        }

        public void SendContactAddress(IOrganizationService organizationService, Guid contactId)
        {
            try
            {
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                QueryExpression queryExpression = new QueryExpression(_DL_customeraddress.EntityName);
                queryExpression.ColumnSet = new ColumnSet(true);
                FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                filterExpression.AddCondition("parentid", ConditionOperator.Equal, contactId);
                filterExpression.AddCondition("addressnumber", ConditionOperator.Equal, 1);
                filterExpression.AddCondition("objecttypecode", ConditionOperator.Equal, 2);
                EntityCollection entityCollection = _DL_customeraddress.Select(organizationService, queryExpression);
                Entity entity = entityCollection.Entities[0];

                CustomerAddressBase mCustomerAddress = new CustomerAddressBase();
                mCustomerAddress.CustomerAddressId = entity.GetAttributeValue<Guid>("customeraddressid");
                if (entity.Attributes.Contains("parentid"))                 
                    mCustomerAddress.ParentId = entity.GetAttributeValue<EntityReference>("parentid").Id;
                    mCustomerAddress.AddressNumber = 1;
                    mCustomerAddress.ObjectTypeCode = 2;
                if (entity.Attributes.Contains("line1"))
                    mCustomerAddress.Line1 = entity.GetAttributeValue<string>("line1");
                if (entity.Attributes.Contains("modifiedon"))
                    mCustomerAddress.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                else
                    ErrorMessage_NotFound("modifiedon");

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, contactId);
                synchronizationResult = _client.SaveEntity(mCustomerAddress);
                _client.Close();
                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_customeraddress.EntityName, _DL_customeraddress.DisplayName, mCustomerAddress.CustomerAddressId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendContactAddress : " + ex.Message);
            }
        }
        #endregion

        #region Equipment
        public void RequestNewPassword(IOrganizationService organizationService, Guid admin, Guid mechanicId)
        {
            try
            {
                SynchronizationResult synchronizationResult = new SynchronizationResult();
                string nrp = string.Empty;

                Entity entity = _DL_equipment.Select(organizationService, mechanicId);
                if (entity.Attributes.Contains("trs_nrp"))
                    nrp = entity.GetAttributeValue<string>("trs_nrp");

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, mechanicId);
                synchronizationResult = _client.ResetAndGenerateNewPassword(mechanicId);
                if (synchronizationResult.Result == Result.Success)
                    SendEmail_PasswordMechanic(organizationService, admin, nrp, synchronizationResult.Data.ToString());
                else
                    ErrorHandler(synchronizationResult, "Reset Password Mechanic", "Reset Password Mechanic", mechanicId);

                _client.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".RequestNewPassword : " + ex.Message.ToString());
            }
        }

        public void RemoteWipe(IOrganizationService organizationService, Guid mechanicId)
        {
            try
            {
                SynchronizationResult synchronizationResult = new SynchronizationResult();
                Connect(organizationService, MethodBase.GetCurrentMethod().Name, mechanicId);
                synchronizationResult = _client.RemoteWipe(mechanicId);
                if (synchronizationResult.Result == Result.Failed)
                    ErrorHandler(synchronizationResult, "Remote Wipe", "Remote Wipe", mechanicId);
                _client.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".RequestNewPassword : " + ex.Message.ToString());
            }
        }

        public void SendEmail_PasswordMechanic(IOrganizationService organizationService, Guid sender, string nrp, string password)
        {
            try
            {
                Guid emailId = Guid.Empty;
                Guid receiver = Guid.Empty;
                
                QueryExpression queryExpression = new QueryExpression(_DL_trs_workflowconfiguration.EntityName);
                queryExpression.ColumnSet = new ColumnSet(true);
                queryExpression.Criteria.AddCondition("trs_generalconfig", ConditionOperator.Equal, Configuration.ConfigurationName);
                EntityCollection entityCollection = _DL_trs_workflowconfiguration.Select(organizationService, queryExpression);
                if (entityCollection.Entities.Count > 0)
                {
                    Entity eConfiguration = entityCollection.Entities[0];   
                    receiver = eConfiguration.GetAttributeValue<EntityReference>("trs_gadgetadministrator").Id;
                }
                else
                    throw new Exception("Please setup Gadget Administrator first !");
                
                EmailAgent emailAgent = new EmailAgent();
                emailAgent.AddSender(sender);
                emailAgent.AddReceiver(_DL_systemuser.EntityName, receiver);
                emailAgent.subject = "New Password for Mechanic with NRP : " + nrp;
                emailAgent.description = password;
                emailAgent.Create(organizationService, out emailId);

                if (emailId != Guid.Empty)
                    emailAgent.Send(organizationService, emailId);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendEmail : " + ex.Message.ToString());
            }
        }

        public void SendAllMechanicGrade(IOrganizationService organizationService)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_trs_mechanicgrade.EntityName);
                EntityCollection eCollection = _DL_trs_mechanicgrade.Select(organizationService, queryExpression);
                foreach (Entity entity in eCollection.Entities)
                {
                    SendMechanicGrade(organizationService, entity.Id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendAllMechanicGrade : " + ex.Message);
            }
        }

        public void SendMechanicGrade(IOrganizationService organizationService, Guid id)
        {
            try
            {
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                Entity entity = _DL_trs_mechanicgrade.Select(organizationService, id);
                trs_mechanicgradeBase mMechanicGrade = new trs_mechanicgradeBase();

                #region Required
                if (entity.Attributes.Contains("trs_mechanicgradeid"))
                    mMechanicGrade.trs_mechanicgradeId = entity.GetAttributeValue<Guid>("trs_mechanicgradeid");
                else
                    ErrorMessage_NotFound("trs_mechanicgradeid");
                if (entity.Attributes.Contains("trs_grade"))
                    mMechanicGrade.trs_grade = entity.GetAttributeValue<string>("trs_grade");
                else
                    ErrorMessage_NotFound("trs_grade");
                if (entity.Attributes.Contains("modifiedon"))
                    mMechanicGrade.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                else
                    ErrorMessage_NotFound("modifiedon");
                if (entity.Attributes.Contains("statecode"))
                    mMechanicGrade.StateCode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                else
                    ErrorMessage_NotFound("statecode");
                if (entity.Attributes.Contains("statuscode"))
                    mMechanicGrade.StatusCode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                else
                    ErrorMessage_NotFound("statuscode");
                #endregion

                #region Optional
                if (entity.Attributes.Contains("trs_gradedesciption"))
                    mMechanicGrade.trs_GradeDesciption = entity.GetAttributeValue<string>("trs_gradedesciption");
                if (entity.Attributes.Contains("trs_tresholdhour"))
                    mMechanicGrade.trs_TresholdHour = entity.GetAttributeValue<Int32>("trs_tresholdhour");
                #endregion

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                synchronizationResult = _client.SaveEntity(mMechanicGrade);
                _client.Close();
                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_trs_mechanicgrade.EntityName, _DL_trs_mechanicgrade.DisplayName, id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendMechanicGrade : " + ex.Message);
            }
        }

        public void SendAllMechanic(IOrganizationService organizationService)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_equipment.EntityName);
                EntityCollection eCollection = _DL_equipment.Select(organizationService, queryExpression);
                foreach (Entity entity in eCollection.Entities)
                {
                    SendMechanic(organizationService, entity.Id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendAllMechanic : " + ex.Message);
            }
        }

        public void SendMechanic(IOrganizationService organizationService, Guid id)
        {
            try
            {
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                Entity entity = _DL_equipment.Select(organizationService, id);
                EquipmentBase mEquipment = new EquipmentBase();
                #region Required
                if (entity.Attributes.Contains("equipmentid"))
                    mEquipment.EquipmentId = entity.GetAttributeValue<Guid>("equipmentid");
                else
                    ErrorMessage_NotFound("equipmentid");
                if (entity.Attributes.Contains("trs_nrp"))
                    mEquipment.trs_NRP = entity.GetAttributeValue<string>("trs_nrp");
                else
                    ErrorMessage_NotFound("trs_nrp");
                if (entity.Attributes.Contains("name"))
                    mEquipment.Name = entity.GetAttributeValue<string>("name");
                else
                    ErrorMessage_NotFound("name");
                if (entity.Attributes.Contains("modifiedon"))
                    mEquipment.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                else
                    ErrorMessage_NotFound("modifiedon");
                #endregion

                #region Optional
                if (entity.Contains("trs_mechanicgrade"))
                    mEquipment.trs_MechanicGrade = ((EntityReference)entity.Attributes["trs_mechanicgrade"]).Id;
                if (entity.Attributes.Contains("trs_monthlypoint"))
                    mEquipment.trs_MonthlyPoint = entity.GetAttributeValue<decimal>("trs_monthlypoint");
                if (entity.Attributes.Contains("trs_repairtimehour"))
                    mEquipment.trs_RepairTimeHour = entity.GetAttributeValue<int>("trs_repairtimehour");
                if (entity.Attributes.Contains("trs_mechanicstatus"))
                    mEquipment.trs_MechanicStatus = entity.GetAttributeValue<bool>("trs_mechanicstatus");
                if (entity.Contains("isdisabled"))
                    mEquipment.IsDisabled = entity.GetAttributeValue<bool>("isdisabled");
                #endregion

                #region Unrequired
                //if (entity.Attributes.Contains("trs_repairtimehourgrade"))
                //    mEquipment.trs_RepairTimeHourGrade = entity.GetAttributeValue<decimal>("trs_repairtimehourgrade");
                //if (entity.Attributes.Contains("siteid"))
                //    mEquipment.SiteId = entity.GetAttributeValue<EntityReference>("siteid").Id;
                //if (entity.Contains("organizationid"))
                //    mEquipment.OrganizationId = ((EntityReference)entity.Attributes["organizationid"]).Id;
                //if (entity.Contains("description"))
                //    mEquipment.Description = entity.GetAttributeValue<string>("description");
                //if (entity.Attributes.Contains("emailaddress"))
                //    mEquipment.EMailAddress = entity.GetAttributeValue<string>("emailaddress");
                //if (entity.Attributes.Contains("trs_deviceid"))
                //    mEquipment.trs_DeviceID = entity.GetAttributeValue<string>("trs_deviceid");
                //if (entity.Attributes.Contains("trs_facilitytype"))
                //    mEquipment.trs_FacilityType = entity.GetAttributeValue<int>("trs_facilitytype");
                //if (entity.Attributes.Contains("trs_mainphone"))
                //    mEquipment.trs_MainPhone = entity.GetAttributeValue<string>("trs_mainphone");
                //if (entity.Attributes.Contains("trs_mainphone"))
                //    mEquipment.trs_PermanentMechanicID = entity.GetAttributeValue<string>("trs_permanentmechanicid");
                //if (entity.Attributes.Contains("trs_mainphone"))
                //    mEquipment.trs_SIM1 = entity.GetAttributeValue<string>("trs_sim1");
                //if (entity.Attributes.Contains("trs_mainphone"))
                //    mEquipment.trs_SIM2 = entity.GetAttributeValue<string>("trs_sim2");
                //if (entity.Contains("trs_mechanicsid"))
                //    mEquipment.trs_MechanicsId = ((EntityReference)entity.Attributes["trs_mechanicsid"]).Id;
                //if (entity.Contains("trs_level"))
                //    mEquipment.trs_Level = entity.GetAttributeValue<string>("trs_level");
                //if (entity.Contains("trs_birthdate"))
                //    mEquipment.trs_BirthDate = entity.GetAttributeValue<DateTime>("trs_birthdate").ToUniversalTime();
                //if (entity.Contains("trs_familystatus"))
                //    mEquipment.trs_FamilyStatus = entity.GetAttributeValue<int>("trs_familystatus");
                //if (entity.Contains("trs_hireddate"))
                //    mEquipment.trs_HiredDate = entity.GetAttributeValue<DateTime>("trs_hireddate").ToUniversalTime();
                #endregion

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                synchronizationResult = _client.SaveEntity(mEquipment);
                _client.Close();
                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_equipment.EntityName, _DL_equipment.DisplayName, id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendMechanic : " + ex.Message);
            }
        }

        public void SendMechanicMonthlyPoint(IOrganizationService organizationService, Guid id)
        {
            try
            {
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                Entity entity = _DL_trs_mechanicmonthlypoint.Select(organizationService, id);
                trs_mechanicmonthlypointBase mMechanicMonthlyPoint = new trs_mechanicmonthlypointBase();

                #region Required
                if (entity.Attributes.Contains("trs_mechanicmonthlypointid"))
                    mMechanicMonthlyPoint.trs_mechanicmonthlypointId = entity.GetAttributeValue<Guid>("trs_mechanicmonthlypointid");
                else
                    ErrorMessage_NotFound("trs_mechanicmonthlypointid");
                if (entity.Attributes.Contains("trs_pointdate"))
                    mMechanicMonthlyPoint.trs_PointDate = entity.GetAttributeValue<DateTime>("trs_pointdate").ToUniversalTime();
                else
                    ErrorMessage_NotFound("trs_pointdate");
                if (entity.Attributes.Contains("trs_mechanicid"))
                    mMechanicMonthlyPoint.trs_MechanicId = entity.GetAttributeValue<EntityReference>("trs_mechanicid").Id;
                else
                    ErrorMessage_NotFound("trs_mechanicid");
                if (entity.Attributes.Contains("trs_point"))
                    mMechanicMonthlyPoint.trs_Point = entity.GetAttributeValue<decimal>("trs_point");
                else
                    ErrorMessage_NotFound("trs_point");
                if (entity.Attributes.Contains("modifiedon"))
                    mMechanicMonthlyPoint.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                else
                    ErrorMessage_NotFound("modifiedon");
                #endregion

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                synchronizationResult = _client.SaveEntity(mMechanicMonthlyPoint);
                _client.Close();
                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_equipment.EntityName, _DL_equipment.DisplayName, id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendMechanic : " + ex.Message);
            }
        }
        #endregion

        #region Parts
        public void SendAllParts(IOrganizationService organizationService)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_trs_masterpart.EntityName);
                EntityCollection eCollection = _DL_trs_masterpart.Select(organizationService, queryExpression);
                foreach (Entity entity in eCollection.Entities)
                {
                    SendPart(organizationService, entity.Id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendAllParts : " + ex.Message);
            }
        }

        public void SendPart(IOrganizationService organizationService, Guid id)
        {
            try
            {
                SynchronizationResult synchronizationResult = new SynchronizationResult();
                Entity entity = _DL_trs_masterpart.Select(organizationService, id);
                trs_masterpartBase mMasterPart = new trs_masterpartBase();
                if (entity.Attributes.Contains("trs_masterpartid"))
                    mMasterPart.trs_masterpartId = entity.GetAttributeValue<Guid>("trs_masterpartid");
                else
                    ErrorMessage_NotFound("trs_masterpartid");
                if (entity.Attributes.Contains("trs_name"))
                    mMasterPart.trs_name = entity.GetAttributeValue<string>("trs_name");
                else
                    ErrorMessage_NotFound("trs_name");
                if (entity.Attributes.Contains("trs_partdescription"))
                    mMasterPart.trs_name = entity.GetAttributeValue<string>("trs_partdescription");
                else
                    ErrorMessage_NotFound("trs_partdescription");
                if (entity.Attributes.Contains("statecode"))
                    mMasterPart.StateCode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                else
                    ErrorMessage_NotFound("statecode");
                if (entity.Attributes.Contains("statuscode"))
                    mMasterPart.StatusCode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                else
                    ErrorMessage_NotFound("statuscode");
                if (entity.Attributes.Contains("modifiedon"))
                    mMasterPart.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                else
                    ErrorMessage_NotFound("modifiedon");

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                synchronizationResult = _client.SaveEntity(mMasterPart);
                _client.Close();
                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_trs_masterpart.EntityName, _DL_trs_masterpart.DisplayName, id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendPart : " + ex.Message);
            }
        }
        #endregion

        #region Product Type
        public void SendAllProductType(IOrganizationService organizationService)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_trs_producttype.EntityName);
                EntityCollection eCollection = _DL_trs_producttype.Select(organizationService, queryExpression);
                foreach (Entity entity in eCollection.Entities)
                {
                    SendProductType(organizationService, entity.Id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendAllProductType : " + ex.Message);
            }
        }

        public void SendProductType(IOrganizationService organizationService, Guid id)
        {
            try
            {
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                Entity entity = _DL_trs_producttype.Select(organizationService, id);
                trs_producttypeBase mProductType = new trs_producttypeBase();
                if (entity.Attributes.Contains("trs_producttypeid"))
                    mProductType.trs_producttypeId = entity.GetAttributeValue<Guid>("trs_producttypeid");
                else
                    ErrorMessage_NotFound("trs_producttypeid");
                mProductType.trs_producttype = entity.GetAttributeValue<string>("trs_producttype");
                if (entity.Attributes.Contains("modifiedon"))
                    mProductType.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                else
                    ErrorMessage_NotFound("modifiedon");
                if (entity.Attributes.Contains("statecode"))
                    mProductType.StateCode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                else
                    ErrorMessage_NotFound("statecode");
                if (entity.Attributes.Contains("statuscode"))
                    mProductType.StatusCode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                else
                    ErrorMessage_NotFound("statuscode");

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                synchronizationResult = _client.SaveEntity(mProductType);
                _client.Close();
                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_trs_producttype.EntityName, _DL_trs_producttype.DisplayName, id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendProductType : " + ex.Message);
            }
        }

        public void SendUnitTypeofWork(IOrganizationService organizationService, Guid id)
        {
            try
            {
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                Entity entity = _DL_trs_unittypeofwork.Select(organizationService, id);
                trs_unittypeofworkBase mUnitTypeofWork = new trs_unittypeofworkBase();
                if (entity.Attributes.Contains("trs_unittypeofworkid"))
                {
                    mUnitTypeofWork.trs_unittypeofworkId = entity.GetAttributeValue<Guid>("trs_unittypeofworkid");
                }
                else
                {
                    ErrorMessage_NotFound("trs_unittypeofworkid");
                }

                if (entity.Attributes.Contains("trs_producttype"))
                {
                    mUnitTypeofWork.trs_ProductType = entity.GetAttributeValue<EntityReference>("trs_producttype").Id;
                }
                else {
                    ErrorMessage_NotFound("trs_unittypeofworkid");    
                }

                mUnitTypeofWork.trs_typeofwork = entity.GetAttributeValue<string>("trs_typeofwork");
                
                if (entity.Attributes.Contains("modifiedon"))
                {
                    mUnitTypeofWork.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                }
                else
                {
                    ErrorMessage_NotFound("modifiedon");
                }

                if (entity.Attributes.Contains("statecode"))
                {
                    mUnitTypeofWork.statecode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                }
                else
                {
                    ErrorMessage_NotFound("statecode");
                }

                if (entity.Attributes.Contains("statuscode"))
                {
                    mUnitTypeofWork.statuscode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                }
                else
                {
                    ErrorMessage_NotFound("statuscode");
                }

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                synchronizationResult = _client.SaveEntity(mUnitTypeofWork);
                _client.Close();
                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_trs_producttype.EntityName, _DL_trs_producttype.DisplayName, id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendUnitTypeofWork : " + ex.Message);
            }
        }
        #endregion

        #region Product Section
        public void SendAllProductSection(IOrganizationService organizationService)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_trs_productsection.EntityName);
                EntityCollection eCollection = _DL_trs_productsection.Select(organizationService, queryExpression);
                foreach (Entity entity in eCollection.Entities)
                {
                    SendProductSection(organizationService, entity.Id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendAllProductSection : " + ex.Message);
            }
        }

        public void SendProductSection(IOrganizationService organizationService, Guid id)
        {
            try
            {
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                Entity entity = _DL_trs_productsection.Select(organizationService, id);
                trs_productsectionBase mProductSection = new trs_productsectionBase();

                if (entity.Attributes.Contains("trs_productsectionid"))
                    mProductSection.trs_productsectionId = entity.GetAttributeValue<Guid>("trs_productsectionid");
                else
                    ErrorMessage_NotFound("trs_productsectionid");
                if (entity.Attributes.Contains("trs_productsectioncode"))
                    mProductSection.trs_productsectioncode = entity.GetAttributeValue<string>("trs_productsectioncode");
                else
                    ErrorMessage_NotFound("trs_productsectioncode");
                if (entity.Attributes.Contains("trs_productsectionname"))
                    mProductSection.trs_productsectionname = entity.GetAttributeValue<string>("trs_productsectionname");
                else
                    ErrorMessage_NotFound("trs_productsectionname");
                if (entity.Attributes.Contains("modifiedon"))
                    mProductSection.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                else
                    ErrorMessage_NotFound("modifiedon");
                if (entity.Attributes.Contains("statecode"))
                    mProductSection.StateCode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                else
                    ErrorMessage_NotFound("statecode");
                if (entity.Attributes.Contains("statuscode"))
                    mProductSection.StatusCode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                else
                    ErrorMessage_NotFound("statuscode");

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                synchronizationResult = _client.SaveEntity(mProductSection);
                _client.Close();
                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_trs_productsection.EntityName, _DL_trs_productsection.DisplayName, id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendProductSection : " + ex.Message);
            }
        }
        #endregion

        #region Product
        public void SendAllProduct(IOrganizationService organizationService)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_product.EntityName);
                queryExpression.Criteria.AddCondition("producttypecode", ConditionOperator.Equal, 1);
                EntityCollection eCollection = _DL_product.Select(organizationService, queryExpression);
                foreach (Entity entity in eCollection.Entities)
                {
                    SendProduct(organizationService, entity.Id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendAllProduct : " + ex.Message);
            }
        }

        public void SendProduct(IOrganizationService organizationService, Guid id)
        {
            try
            {
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                Entity entity = _DL_product.Select(organizationService, id);
                ProductBase mProduct = new ProductBase();

                #region Required
                if (entity.Attributes.Contains("productid"))
                    mProduct.ProductId = entity.GetAttributeValue<Guid>("productid");
                else
                    ErrorMessage_NotFound("productid");
                if (entity.Attributes.Contains("productnumber"))
                    mProduct.ProductNumber = entity.GetAttributeValue<string>("productnumber");
                else
                    ErrorMessage_NotFound("productnumber");
                if (entity.Attributes.Contains("name"))
                    mProduct.Name = entity.GetAttributeValue<string>("name");
                else
                    ErrorMessage_NotFound("name");
                if (entity.Attributes.Contains("trs_producttypeid"))
                    mProduct.trs_producttypeid = entity.Contains("trs_producttypeid") ? entity.GetAttributeValue<EntityReference>("trs_producttypeid").Id : (Guid?)null;
                //else
                    //ErrorMessage_NotFound("trs_producttypeid");
                if (entity.Attributes.Contains("modifiedon"))
                    mProduct.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                else
                    ErrorMessage_NotFound("modifiedon");
                if (entity.Attributes.Contains("statecode"))
                    mProduct.StateCode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                else
                    ErrorMessage_NotFound("statecode");
                if (entity.Attributes.Contains("statuscode"))
                    mProduct.StatusCode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                else
                    ErrorMessage_NotFound("statuscode");
                #endregion

                #region Not Needed Yet
                //if (entity.Attributes.Contains("transactioncurrencyid"))
                //    mProduct.TransactionCurrencyId = entity.Contains("transactioncurrencyid") ? entity.GetAttributeValue<EntityReference>("transactioncurrencyid").Id : (Guid?)null;
                //if (entity.Attributes.Contains("new_isstandard"))
                //    mProduct.new_IsStandard = entity.GetAttributeValue<bool>("new_isstandard");
                //if (entity.Attributes.Contains("new_unitgroup"))
                //    mProduct.new_UnitGroup = entity.Contains("new_unitgroup") ? entity.GetAttributeValue<EntityReference>("new_unitgroup").Id : (Guid?)null;
                //if (entity.Attributes.Contains("defaultuomscheduleid"))
                //    mProduct.DefaultUoMScheduleId = entity.Contains("defaultuomscheduleid") ? entity.GetAttributeValue<EntityReference>("defaultuomscheduleid").Id : (Guid?)null;
                //if (entity.Attributes.Contains("defaultuomid"))
                //    mProduct.DefaultUoMId = entity.Contains("defaultuomid") ? entity.GetAttributeValue<EntityReference>("defaultuomid").Id : (Guid?)null;
                //if (entity.Attributes.Contains("pricelevelid"))
                //    mProduct.PriceLevelId = entity.Contains("pricelevelid") ? entity.GetAttributeValue<EntityReference>("pricelevelid").Id : (Guid?)null;
                //if (entity.Attributes.Contains("new_division"))
                //    mProduct.new_Division = entity.Contains("new_division") ? entity.GetAttributeValue<EntityReference>("new_division").Id : (Guid?)null;
                //if (entity.Attributes.Contains("new_materialgroup"))
                //    mProduct.new_MaterialGroup = entity.Contains("new_materialgroup") ? entity.GetAttributeValue<EntityReference>("new_materialgroup").Id : (Guid?)null;
                //if (entity.Attributes.Contains("new_category"))
                //    mProduct.new_Category = entity.Contains("new_category") ? entity.GetAttributeValue<EntityReference>("new_category").Id : (Guid?)null;
                //if (entity.Attributes.Contains("new_salesorganization"))
                //    mProduct.new_SalesOrganization = entity.Contains("new_salesorganization") ? entity.GetAttributeValue<EntityReference>("new_salesorganization").Id : (Guid?)null;
                //if (entity.Attributes.Contains("producttypecode"))
                //    mProduct.ProductTypeCode = entity.GetAttributeValue<OptionSetValue>("producttypecode").Value;
                //if (entity.Attributes.Contains("new_salesgroup"))
                //    mProduct.new_SalesGroup = entity.Contains("new_salesgroup") ? entity.GetAttributeValue<EntityReference>("new_salesgroup").Id : (Guid?)null;
                //if (entity.Attributes.Contains("new_externalmaterialgroup"))
                //    mProduct.new_ExternalMaterialGroup = entity.Contains("new_externalmaterialgroup") ? entity.GetAttributeValue<EntityReference>("new_externalmaterialgroup").Id : (Guid?)null;
                //if (entity.Attributes.Contains("description"))
                //    mProduct.Description = entity.GetAttributeValue<string>("description");
                #endregion

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                synchronizationResult = _client.SaveEntity(mProduct);
                _client.Close();
                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_product.EntityName, _DL_product.DisplayName, id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendProduct : " + ex.Message);
            }
        }
        #endregion

        #region Population
        public void SendAllPopulation(IOrganizationService organizationService)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_new_population.EntityName);
                EntityCollection eCollection = _DL_new_population.Select(organizationService, queryExpression);
                foreach (Entity entity in eCollection.Entities)
                {
                    SendPopulation(organizationService, entity.Id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendAllPopulation : " + ex.Message);
            }
        }

        public void SendPopulation(IOrganizationService organizationService, Guid id)
        {
            try
            {
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                Entity entity = _DL_new_population.Select(organizationService, id);
                
                new_populationBase mPopulation = new new_populationBase();
                if (entity.Attributes.Contains("new_populationid"))
                    mPopulation.new_populationId = entity.GetAttributeValue<Guid>("new_populationid");
                else
                    ErrorMessage_NotFound("new_populationid");
                if (entity.Attributes.Contains("new_enginenumber"))
                    mPopulation.new_enginenumber = entity.GetAttributeValue<string>("new_enginenumber");
                if (entity.Attributes.Contains("new_serialnumber"))
                    mPopulation.new_SerialNumber = entity.GetAttributeValue<string>("new_serialnumber");
                if (entity.Attributes.Contains("trs_productmaster"))
                    mPopulation.trs_ProductMaster = entity.GetAttributeValue<EntityReference>("trs_productmaster").Id;
                if (entity.Attributes.Contains("trs_equipmentnumber"))
                    mPopulation.trs_EquipmentNumber = entity.GetAttributeValue<string>("trs_equipmentnumber");
                if (entity.Attributes.Contains("new_deliverydate"))
                    mPopulation.new_DeliveryDate = entity.GetAttributeValue<DateTime>("new_deliverydate");
                if (entity.Attributes.Contains("trs_oldhm"))
                    mPopulation.trs_OldHM = entity.GetAttributeValue<int>("trs_oldhm");
                if (entity.Attributes.Contains("modifiedon"))
                    mPopulation.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                else
                    ErrorMessage_NotFound("modifiedon");
                if (entity.Attributes.Contains("statecode"))
                    mPopulation.statecode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                else
                    ErrorMessage_NotFound("statecode");
                if (entity.Attributes.Contains("statuscode"))
                    mPopulation.statuscode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                else
                    ErrorMessage_NotFound("statuscode");

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                synchronizationResult = _client.SaveEntity(mPopulation);
                _client.Close();
                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_new_population.EntityName, _DL_new_population.DisplayName, id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendPopulation : " + ex.Message);
            }
        }
        #endregion

        #region Task
        public void SendTaskList(IOrganizationService organizationService, Guid id)
        {
            try
            {
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                Entity entity = _DL_trs_tasklist.Select(organizationService, id);

                trs_tasklistBase mTaskList = new trs_tasklistBase();
                if (entity.Attributes.Contains("trs_tasklistid"))
                    mTaskList.trs_tasklistId = entity.GetAttributeValue<Guid>("trs_tasklistid");
                else
                    ErrorMessage_NotFound("trs_tasklistid");
                if (entity.Attributes.Contains("trs_name"))
                    mTaskList.trs_name = entity.GetAttributeValue<string>("trs_name");
                else
                    ErrorMessage_NotFound("trs_name");
                if (entity.Attributes.Contains("trs_productsection"))
                    mTaskList.trs_ProductSection = entity.GetAttributeValue<EntityReference>("trs_productsection").Id;
                else
                    ErrorMessage_NotFound("trs_productsection");
                if (entity.Attributes.Contains("statecode"))
                    mTaskList.StateCode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                else
                    ErrorMessage_NotFound("statecode");
                if (entity.Attributes.Contains("statuscode"))
                    mTaskList.StatusCode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                else
                    ErrorMessage_NotFound("statuscode");
                if (entity.Attributes.Contains("modifiedon"))
                    mTaskList.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                else
                    ErrorMessage_NotFound("modifiedon");

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                synchronizationResult = _client.SaveEntity(mTaskList);
                _client.Close();
                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_trs_tasklist.EntityName, _DL_trs_tasklist.DisplayName, id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendTaskList : " + ex.Message);
            }
        }

        public void SendAllTaskList_Group(IOrganizationService organizationService)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_trs_tasklistgroup.EntityName);
                EntityCollection eCollection = _DL_trs_tasklistgroup.Select(organizationService, queryExpression);
                foreach (Entity entity in eCollection.Entities)
                {
                    SendTaskList_Group(organizationService, entity.Id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendAllTaskList_Group : " + ex.Message);
            }
        }

        public void SendTaskList_Group(IOrganizationService organizationService, Guid id)
        {
            try
            {
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                Entity entity = _DL_trs_tasklistgroup.Select(organizationService, id);

                trs_tasklistgroupBase mTaskListGroup = new trs_tasklistgroupBase();
                if (entity.Attributes.Contains("trs_tasklistgroupid"))
                    mTaskListGroup.trs_tasklistgroupId = entity.GetAttributeValue<Guid>("trs_tasklistgroupid");
                else
                    ErrorMessage_NotFound("trs_tasklistgroupid");
                if (entity.Attributes.Contains("trs_tasklistgroupname"))
                    mTaskListGroup.trs_tasklistgroupname = entity.GetAttributeValue<string>("trs_tasklistgroupname");
                else
                    ErrorMessage_NotFound("trs_tasklistgroupname");
                if (entity.Attributes.Contains("statecode"))
                    mTaskListGroup.statecode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                else
                    ErrorMessage_NotFound("statecode");
                if (entity.Attributes.Contains("statuscode"))
                    mTaskListGroup.statuscode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                else
                    ErrorMessage_NotFound("statuscode");
                if (entity.Attributes.Contains("modifiedon"))
                    mTaskListGroup.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                else
                    ErrorMessage_NotFound("modifiedon");

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                synchronizationResult = _client.SaveEntity(mTaskListGroup);
                _client.Close();
                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_trs_tasklistgroup.EntityName, _DL_trs_tasklistgroup.DisplayName, id);
                }
                //throw new Exception("test exception sync");
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendTaskList_Group : " + ex.Message);
            }
        }
        
        public void SendTaskList_Header(IOrganizationService organizationService, Guid id)
        {
            try
            {
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                Entity entity = _DL_trs_tasklistheader.Select(organizationService, id);

                trs_tasklistheaderBase mTaskListHeader = new trs_tasklistheaderBase();
                if (entity.Attributes.Contains("trs_tasklistheaderid"))
                    mTaskListHeader.trs_tasklistheaderId = entity.GetAttributeValue<Guid>("trs_tasklistheaderid");
                else
                    ErrorMessage_NotFound("trs_tasklistheaderid");
                if (entity.Attributes.Contains("trs_header"))
                    mTaskListHeader.trs_Header = entity.GetAttributeValue<string>("trs_header");
                else
                    ErrorMessage_NotFound("trs_header");
                if (entity.Attributes.Contains("trs_tasklistgroup"))
                    mTaskListHeader.trs_TaskListGroup = entity.GetAttributeValue<EntityReference>("trs_tasklistgroup").Id;
                else
                    ErrorMessage_NotFound("trs_tasklistgroup");
                if (entity.Attributes.Contains("trs_product"))
                    mTaskListHeader.trs_Product = entity.GetAttributeValue<EntityReference>("trs_product").Id;
                else
                    ErrorMessage_NotFound("trs_product");
                if (entity.Attributes.Contains("trs_productsection"))
                    mTaskListHeader.trs_ProductSection = entity.GetAttributeValue<EntityReference>("trs_productsection").Id;
                else
                    ErrorMessage_NotFound("trs_productsection");
                if (entity.Attributes.Contains("statecode"))
                    mTaskListHeader.statecode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                else
                    ErrorMessage_NotFound("statecode");
                if (entity.Attributes.Contains("statuscode"))
                    mTaskListHeader.statuscode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                else
                    ErrorMessage_NotFound("statuscode");
                if (entity.Attributes.Contains("modifiedon"))
                    mTaskListHeader.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                else
                    ErrorMessage_NotFound("modifiedon");

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                synchronizationResult = _client.SaveEntity(mTaskListHeader);
                _client.Close();
                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_trs_tasklistheader.EntityName, _DL_trs_tasklistheader.DisplayName, id);
                }
                else
                {
                    SendAllTaskList_DetailbyHeader(organizationService, id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendTaskList_Header : " + ex.Message);
            }

        }

        public void SendAllTaskList_DetailbyHeader(IOrganizationService organizationService, Guid taskListHeaderId)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_trs_commercialtask.EntityName);
                queryExpression.Criteria.AddCondition("trs_tasklistheaderid", ConditionOperator.Equal, taskListHeaderId);
                EntityCollection eCollection = _DL_trs_commercialdetail.Select(organizationService, queryExpression);
                foreach (Entity entity in eCollection.Entities)
                {
                    SendTaskList_Detail(organizationService, entity.Id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendTaskList_DetailbyHeader : " + ex.Message);
            }
        }

        public void SendTaskList_Detail(IOrganizationService organizationService, Guid id)
        {
            try
            {
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Start ...");
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Get Data ...");

                Entity entity = _DL_trs_commercialtask.Select(organizationService, id);

                trs_commercialtaskBase mCommercialTask = new trs_commercialtaskBase();
                if (entity.Attributes.Contains("trs_commercialtaskid"))
                    mCommercialTask.trs_commercialtaskId = entity.GetAttributeValue<Guid>("trs_commercialtaskid");
                else
                    ErrorMessage_NotFound("trs_commercialtaskid");
                if (entity.Attributes.Contains("trs_taskcode"))
                    mCommercialTask.trs_taskcode = entity.GetAttributeValue<string>("trs_taskcode");
                else
                    ErrorMessage_NotFound("trs_taskcode");
                if (entity.Attributes.Contains("trs_tasklistheaderid"))
                    mCommercialTask.trs_TasklistHeaderId = entity.GetAttributeValue<EntityReference>("trs_tasklistheaderid").Id;
                else
                    ErrorMessage_NotFound("trs_tasklistheaderid");
                if (entity.Attributes.Contains("trs_taskname"))
                    mCommercialTask.trs_TaskName = entity.GetAttributeValue<EntityReference>("trs_taskname").Id;
                else
                    ErrorMessage_NotFound("trs_taskname");
                if (entity.Attributes.Contains("trs_mechanicgrade"))
                    mCommercialTask.trs_MechanicGrade = entity.GetAttributeValue<EntityReference>("trs_mechanicgrade").Id;
                else
                    ErrorMessage_NotFound("trs_mechanicgrade");
                if (entity.Attributes.Contains("trs_rtg"))
                    mCommercialTask.trs_RTG = entity.GetAttributeValue<Decimal>("trs_rtg");
                else
                    ErrorMessage_NotFound("trs_rtg");
                if (entity.Attributes.Contains("statecode"))
                    mCommercialTask.StateCode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                else
                    ErrorMessage_NotFound("statecode");
                if (entity.Attributes.Contains("statuscode"))
                    mCommercialTask.StatusCode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                else
                    ErrorMessage_NotFound("statuscode");
                if (entity.Attributes.Contains("modifiedon"))
                    mCommercialTask.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                else
                    ErrorMessage_NotFound("modifiedon");

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + mCommercialTask.trs_commercialtaskId + _logCreator.ColumnsSeparator
                    + mCommercialTask.trs_taskcode + _logCreator.ColumnsSeparator
                    + mCommercialTask.trs_TasklistHeaderId + _logCreator.ColumnsSeparator
                    + mCommercialTask.trs_TaskName + _logCreator.ColumnsSeparator
                    + mCommercialTask.trs_RTG.ToString());
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Get Data Done.");

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Sending Data ...");
                synchronizationResult = _client.SaveEntity(mCommercialTask);
                _client.Close();
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Sending Data Done ...");

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Checking Result ...");
                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_trs_commercialtask.EntityName, _DL_trs_commercialtask.DisplayName, id);
                }
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Checking Result Done.");

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Complete ...");
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendTaskList_Detail : " + ex.Message);
            }
        }

        public void SendAllTaskList_DetailPartsbyDetail(IOrganizationService organizationService, Guid taskListDetailId)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_trs_commercialdetailmechanic.EntityName);
                queryExpression.Criteria.AddCondition("trs_tasklistdetailid", ConditionOperator.Equal, taskListDetailId);
                EntityCollection eCollection = _DL_trs_commercialdetailmechanic.Select(organizationService, queryExpression);
                foreach (Entity entity in eCollection.Entities)
                {
                    SendAllTaskList_DetailPartsbyDetail(organizationService, entity.Id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendAllTaskList_DetailPartsbyDetail : " + ex.Message);
            }
        }

        public void SendTasKList_DetailParts(IOrganizationService organizationService, Guid id)
        {
            try
            {
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                Entity entity = _DL_trs_tasklistdetailpart.Select(organizationService, id);

                trs_tasklistdetailpartBase mTaskListDetailPart = new trs_tasklistdetailpartBase();
                if (entity.Attributes.Contains("trs_tasklistdetailpartid"))
                    mTaskListDetailPart.trs_tasklistdetailpartId = entity.GetAttributeValue<Guid>("trs_tasklistdetailpartid");
                else
                    ErrorMessage_NotFound("trs_tasklistdetailpartid");
                if (entity.Attributes.Contains("trs_tasklistdetailid"))
                    mTaskListDetailPart.trs_tasklistdetailid = entity.GetAttributeValue<EntityReference>("trs_tasklistdetailid").Id;
                else
                    ErrorMessage_NotFound("trs_tasklistdetailid");
                if (entity.Attributes.Contains("trs_masterpartid"))
                    mTaskListDetailPart.trs_masterpartid = entity.GetAttributeValue<EntityReference>("trs_masterpartid").Id;
                else
                    ErrorMessage_NotFound("trs_masterpartid");
                if (entity.Attributes.Contains("trs_quantity"))
                    mTaskListDetailPart.trs_quantity = entity.GetAttributeValue<int>("trs_quantity");
                else
                    ErrorMessage_NotFound("trs_quantity");
                if (entity.Attributes.Contains("statecode"))
                    mTaskListDetailPart.statecode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                else
                    ErrorMessage_NotFound("statecode");
                if (entity.Attributes.Contains("statuscode"))
                    mTaskListDetailPart.statuscode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                else
                    ErrorMessage_NotFound("statuscode");
                if (entity.Attributes.Contains("modifiedon"))
                    mTaskListDetailPart.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                else
                    ErrorMessage_NotFound("modifiedon");

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                synchronizationResult = _client.SaveEntity(mTaskListDetailPart);
                _client.Close();
                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_trs_tasklistdetailpart.EntityName, _DL_trs_tasklistdetailpart.DisplayName, id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendTasKList_DetailParts : " + ex.Message);
            }
        }
        #endregion

        #region Service Appointment
        //Role :
        //  0   : Member
        //  1   : Leader
        //  2   : Second Man
        public int ConvertMechanicRole_MobiletoCRM(int role)
        {
            //return (167630000 + role);
            return role;
        }
        public int ConvertMechanicRole_CRMtoMobile(int role)
        {
            //return (role - 167630000);
            return role;
        }

        public void SendActivityPointer(IOrganizationService organizationService, Guid activityId)
        {
            try
            {
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Start ...");
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Get Data for Guid '" + activityId + "' ...");
                Entity entity = _DL_activitypointer.Select(organizationService, activityId);

                ActivityPointerBase mActivityPointer = new ActivityPointerBase();
                mActivityPointer.ActivityId = activityId;
                if (entity.Attributes.Contains("subject"))
                    mActivityPointer.Subject = entity.GetAttributeValue<string>("subject");
                else
                    ErrorMessage_NotFound("subject");
                if (entity.Attributes.Contains("actualstart"))
                    mActivityPointer.ActualStart = entity.GetAttributeValue<DateTime>("actualstart").ToUniversalTime();
                if (entity.Attributes.Contains("actualend"))
                    mActivityPointer.ActualEnd = entity.GetAttributeValue<DateTime>("actualend").ToUniversalTime();
                if (entity.Attributes.Contains("regardingobjectid"))
                    mActivityPointer.RegardingObjectId = entity.GetAttributeValue<EntityReference>("regardingobjectid").Id;
                else
                    ErrorMessage_NotFound("regardingobjectid");
                if (entity.Attributes.Contains("statecode"))
                    mActivityPointer.StateCode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                else
                    ErrorMessage_NotFound("statecode");
                if (entity.Attributes.Contains("statuscode"))
                    mActivityPointer.StatusCode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                else
                    ErrorMessage_NotFound("statuscode");
                if (entity.Attributes.Contains("modifiedon"))
                    mActivityPointer.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                else
                    ErrorMessage_NotFound("modifiedon");
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Get Data Done.");

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, activityId);
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Sending Data ...");
                synchronizationResult = _client.SaveEntity(mActivityPointer);
                _client.Close();
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Sending Data Done ...");

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Checking Result ...");
                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_activitypointer.EntityName, _DL_activitypointer.DisplayName, activityId);
                }
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Checking Result Done.");

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Complete ...");
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendActivityPointer : " + ex.Message);
            }
        }

        public void SendServiceAppointment(IOrganizationService organizationService, Guid id)
        {
            try
            {
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + id.ToString() + _logCreator.ColumnsSeparator
                    + "Start ...");
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + id.ToString() + _logCreator.ColumnsSeparator
                    + "Sending Component ...");
                SendServiceAppointment_Customer(organizationService, id);
                SendServiceAppointment_Header(organizationService, id);
                
                SendServiceAppointment_Mechanic(organizationService, id);
                SendServiceAppointment_PartsSummary(organizationService, id);
                SendServiceAppointment_SupportingMaterial(organizationService, id);
                SendServiceAppointment_CommercialHeader(organizationService, id, true, true);
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + id.ToString() + _logCreator.ColumnsSeparator
                    + "Sending Component Done.");

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + id.ToString() + _logCreator.ColumnsSeparator
                    + "Triggering Dispatch ...");
                synchronizationResult = _client.DispatchWorkOrder(id);
                _client.Close();
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + id.ToString() + _logCreator.ColumnsSeparator
                    + "Triggering Dispatch Done ...");

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + id.ToString() + _logCreator.ColumnsSeparator
                    + "Checking Result ...");
                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, "Dispatch Notification", "Dispatch Notification", id);
                }
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + id.ToString() + _logCreator.ColumnsSeparator
                    + "Checking Result Done.");

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + id.ToString() + _logCreator.ColumnsSeparator
                    + "Complete ...");
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendServiceAppointment : " + ex.Message);
            }
        }

        public void SendServiceAppointment_Header(IOrganizationService organizationService, Guid id)
        {
            try
            {
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Start ...");
                SendActivityPointer(organizationService, id);
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Get Data ...");
                Entity ePlant = new Entity();
                Entity entity = _DL_serviceappointment.Select(organizationService, id);

                if (entity.Attributes.Contains("trs_mechanicleader"))
                    _mechanicLeaderId = entity.GetAttributeValue<EntityReference>("trs_mechanicleader").Id;
                if (entity.Attributes.Contains("trs_mechanicsubleader"))
                    _mechanicLeaderId = entity.GetAttributeValue<EntityReference>("trs_mechanicsubleader").Id;

                ServiceAppointmentBase mServiceAppointment = new ServiceAppointmentBase();
                #region Required
                if (entity.Attributes.Contains("activityid"))
                    mServiceAppointment.ActivityId = entity.GetAttributeValue<Guid>("activityid");
                else
                    ErrorMessage_NotFound("activityid");
                if (entity.Attributes.Contains("trs_crmwonumber"))
                    mServiceAppointment.trs_CRMWONumber = entity.GetAttributeValue<string>("trs_crmwonumber");
                else
                    ErrorMessage_NotFound("trs_crmwonumber");
                if (entity.Attributes.Contains("trs_branch"))
                {
                    Entity eBusinessUnit = _DL_businessunit.Select(organizationService, entity.GetAttributeValue<EntityReference>("trs_branch").Id);
                    mServiceAppointment.trs_Branch = eBusinessUnit.GetAttributeValue<string>("trs_branchcode");
                }
                else
                {
                    ErrorMessage_NotFound("trs_branch");
                }
                if (entity.Attributes.Contains("trs_contactperson"))
                    mServiceAppointment.trs_ContactPerson = entity.GetAttributeValue<EntityReference>("trs_contactperson").Id;
                else
                    ErrorMessage_NotFound("trs_contactperson");
                if (entity.Attributes.Contains("trs_cpphone"))
                    mServiceAppointment.trs_CPPhone = entity.GetAttributeValue<string>("trs_cpphone");
                else
                    ErrorMessage_NotFound("trs_cpphone");
                if (entity.Attributes.Contains("trs_cponsite"))
                    mServiceAppointment.trs_CPOnSite = entity.GetAttributeValue<EntityReference>("trs_cponsite").Id;
                else
                    ErrorMessage_NotFound("trs_cponsite");
                if (entity.Attributes.Contains("trs_phoneonsite"))
                    mServiceAppointment.trs_PhoneonSite = entity.GetAttributeValue<string>("trs_phoneonsite");
                else
                    ErrorMessage_NotFound("trs_phoneonsite");
                if (entity.Attributes.Contains("trs_equipment"))
                    mServiceAppointment.trs_equipment = entity.GetAttributeValue<EntityReference>("trs_equipment").Id;
                else
                    ErrorMessage_NotFound("trs_equipment");
                if (entity.Attributes.Contains("trs_plant"))
                {
                    ePlant = _DL_site.Select(organizationService, entity.GetAttributeValue<EntityReference>("trs_plant").Id);
                    mServiceAppointment.new_Plant = ePlant.GetAttributeValue<string>("name");
                }
                else
                    ErrorMessage_NotFound("trs_plant");
                if (entity.Attributes.Contains("scheduledstart"))
                    mServiceAppointment.trs_ScheduledStart = entity.GetAttributeValue<DateTime>("scheduledstart").ToUniversalTime();
                else
                    ErrorMessage_NotFound("scheduledstart");
                if (entity.Attributes.Contains("scheduledend"))
                    mServiceAppointment.trs_ScheduledEnd = entity.GetAttributeValue<DateTime>("scheduledend").ToUniversalTime();
                else
                    ErrorMessage_NotFound("scheduledend");
                if (entity.Attributes.Contains("trs_estimationhour"))
                    mServiceAppointment.trs_EstimationHour = entity.GetAttributeValue<decimal>("trs_estimationhour");
                else
                    ErrorMessage_NotFound("trs_estimationhour");
                if (entity.Attributes.Contains("trs_pmacttype"))
                    mServiceAppointment.trs_PMActType = entity.GetAttributeValue<EntityReference>("trs_pmacttype").Id;
                else
                    ErrorMessage_NotFound("trs_pmacttype");
                if (entity.Attributes.Contains("modifiedon"))
                    mServiceAppointment.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                else
                    ErrorMessage_NotFound("modifiedon");
                #endregion

                #region Optional
                if (entity.Attributes.Contains("trs_product"))
                    mServiceAppointment.trs_Product = entity.GetAttributeValue<string>("trs_product");
                if (entity.Attributes.Contains("trs_productmodel"))
                    mServiceAppointment.trs_ProductModel = entity.GetAttributeValue<string>("trs_productmodel");
                if (entity.Attributes.Contains("trs_customernote"))
                    mServiceAppointment.trs_CustomerNote = entity.GetAttributeValue<string>("trs_customernote");
                if (entity.Attributes.Contains("new_deliverydate"))
                    mServiceAppointment.new_DeliveryDate = entity.GetAttributeValue<DateTime>("new_deliverydate").ToUniversalTime();
                if (entity.Attributes.Contains("trs_lasthourmeter"))
                    mServiceAppointment.trs_LastHourMeter = entity.GetAttributeValue<decimal>("trs_lasthourmeter");
                if (entity.Attributes.Contains("trs_hourmeter"))
                    mServiceAppointment.trs_HourMeter = entity.GetAttributeValue<decimal>("trs_hourmeter");
                if (entity.Attributes.Contains("trs_inspectorcomments"))
                    mServiceAppointment.trs_InspectorComments = entity.GetAttributeValue<string>("trs_inspectorcomments");
                if (entity.Attributes.Contains("trs_inspectorsuggestion"))
                    mServiceAppointment.trs_PMActType = entity.GetAttributeValue<EntityReference>("trs_inspectorsuggestion").Id;
                if (entity.Attributes.Contains("trs_customersatisfaction"))
                    mServiceAppointment.trs_CustomerSatisfaction = entity.GetAttributeValue<string>("trs_customersatisfaction");
                if (entity.Attributes.Contains("trs_customercomments"))
                    mServiceAppointment.trs_CustomerComments = entity.GetAttributeValue<string>("trs_customercomments");
                if (entity.Attributes.Contains("trs_functionallocation"))
                    mServiceAppointment.trs_FunctionalLocation = entity.GetAttributeValue<EntityReference>("trs_functionallocation").Id;
                if (entity.Attributes.Contains("trs_oldhm"))
                    mServiceAppointment.trs_OldHM = entity.GetAttributeValue<int>("trs_oldhm");
                mServiceAppointment.new_DealerName = _customerId;
                #endregion
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Get Data Done.");

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Sending Data ...");
                synchronizationResult = _client.SaveEntity(mServiceAppointment);
                _client.Close();
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Sending Data Done ...");

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Checking Result ...");
                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_serviceappointment.EntityName, _DL_serviceappointment.DisplayName, id);
                }
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Checking Result Done.");

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Complete ...");
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendServiceAppointment_Header : " + ex.Message);
            }
        }
        
        public void SendServiceAppointment_Customer(IOrganizationService organizationService, Guid activityId)
        {
            try
            {
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Start ...");
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Get Data ...");
                ActivityPartyBase mActivityParty = new ActivityPartyBase();
                int participationtypemask = 11;
                int partyobjecttypecode = 1;

                QueryExpression queryExpression = new QueryExpression(_DL_activityparty.EntityName);
                queryExpression.ColumnSet = new ColumnSet(true);
                FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                filterExpression.AddCondition("activityid", ConditionOperator.Equal, activityId);
                filterExpression.AddCondition("participationtypemask", ConditionOperator.Equal, participationtypemask);
                filterExpression.AddCondition("partyobjecttypecode", ConditionOperator.Equal, partyobjecttypecode);

                EntityCollection entityCollection = _DL_activityparty.Select(organizationService, queryExpression);
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Get Data Done.");

                if (entityCollection.Entities.Count > 0)
                {
                    Entity entity = entityCollection.Entities[0];
                    //mActivityParty.ParticipationTypeMask = participationtypemask;
                    //mActivityParty.PartyObjectTypeCode = partyobjecttypecode;
                    //mActivityParty.ActivityId = activityId;
                    //if (entity.Attributes.Contains("activitypartyid"))
                    //    mActivityParty.ActivityPartyId = entity.GetAttributeValue<Guid>("activitypartyid");
                    //else
                    //    ErrorMessage_NotFound("activitypartyid");
                    if (entity.Attributes.Contains("partyid"))
                    {
                        //mActivityParty.PartyId = entity.GetAttributeValue<EntityReference>("partyid").Id;
                        _customerId = entity.GetAttributeValue<EntityReference>("partyid").Id;
                    }
                    else
                        ErrorMessage_NotFound("partyid");

                    //Connect();
                    //synchronizationResult = _client.SaveEntity(mActivityParty);
                    //_client.Close();
                    //if (synchronizationResult.Result == Result.Failed)
                    //{
                    //    ErrorHandler(synchronizationResult, _DL_activityparty.EntityName, _DL_activityparty.DisplayName, mActivityParty.ActivityPartyId);
                    //}
                }
                else
                    throw new Exception("Can not found Customer.");

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Complete ...");
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendServiceAppointment_Customer : " + ex.Message);
            }
        }

        public void SendServiceAppointment_Mechanic(IOrganizationService organizationService, Guid activityId)
        {
            try
            {
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Start ...");
                GetSecondmanList(organizationService, activityId);
                
                SynchronizationResult synchronizationResult = new SynchronizationResult();
                ServiceAppointmentBaseMechanic mServiceAppointmentMechanic = new ServiceAppointmentBaseMechanic();

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Get Data ...");
                QueryExpression queryExpression = new QueryExpression(_DL_activityparty.EntityName);
                queryExpression.ColumnSet = new ColumnSet(true);

                FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                filterExpression.AddCondition("activityid", ConditionOperator.Equal, activityId);
                filterExpression.AddCondition("participationtypemask", ConditionOperator.Equal, 10);
                filterExpression.AddCondition("partyobjecttypecode", ConditionOperator.Equal, 4000);

                EntityCollection entityCollection = _DL_activityparty.Select(organizationService, queryExpression);
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Get Data Done.");

                foreach (Entity entity in entityCollection.Entities)
                {
                    synchronizationResult = new SynchronizationResult();
                    mServiceAppointmentMechanic = new ServiceAppointmentBaseMechanic();
                    mServiceAppointmentMechanic.Id = entity.GetAttributeValue<Guid>("activitypartyid");
                    mServiceAppointmentMechanic.WorkOrderId = activityId;
                    mServiceAppointmentMechanic.EquipmentId = entity.GetAttributeValue<EntityReference>("partyid").Id;
                    mServiceAppointmentMechanic.Role = GetServiceAppointment_MechanicRole(organizationService, activityId, mServiceAppointmentMechanic.EquipmentId);

                    Connect(organizationService, MethodBase.GetCurrentMethod().Name, activityId);
                    _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                        + "Sending Data ...");
                    synchronizationResult = _client.SaveEntity(mServiceAppointmentMechanic);
                    _client.Close();
                    _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                        + "Sending Data Done ...");

                    _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                        + "Checking Result ...");
                    if (synchronizationResult.Result == Result.Failed)
                    {
                        ErrorHandler(synchronizationResult, _DL_activityparty.EntityName, "Service Appointment Mechanic", mServiceAppointmentMechanic.Id);
                    }
                    _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                        + "Checking Result Done.");
                }

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Complete ...");
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendServiceAppointment_Mechanic : " + ex.Message);
            }
        }

        public void SendServiceAppointment_PartsSummary(IOrganizationService organizationService, Guid activityId)
        {
            try
            {
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Start ...");
                SynchronizationResult synchronizationResult = new SynchronizationResult();
                trs_workorderpartssummaryBase mServiceAppointmentPartsSummary = new trs_workorderpartssummaryBase();

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Get Data ...");
                QueryExpression queryExpression = new QueryExpression(_DL_trs_workorderpartssummary.EntityName);
                queryExpression.ColumnSet = new ColumnSet(true);
                queryExpression.Criteria.AddCondition("trs_workorder", ConditionOperator.Equal, activityId);

                EntityCollection entityCollection = _DL_trs_workorderpartssummary.Select(organizationService, queryExpression);
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Get Data Done.");

                foreach (Entity entity in entityCollection.Entities)
                {
                    synchronizationResult = new SynchronizationResult();
                    mServiceAppointmentPartsSummary = new trs_workorderpartssummaryBase();
                    mServiceAppointmentPartsSummary.trs_workorderpartssummaryId = entity.GetAttributeValue<Guid>("trs_workorderpartssummaryid");
                    mServiceAppointmentPartsSummary.trs_workorder = entity.GetAttributeValue<EntityReference>("trs_workorder").Id;
                    mServiceAppointmentPartsSummary.trs_partnumber = entity.GetAttributeValue<EntityReference>("trs_partnumber").Id;
                    mServiceAppointmentPartsSummary.trs_partname = entity.GetAttributeValue<string>("trs_partname");
                    //mServiceAppointmentPartsSummary.trs_partdescription = entity.GetAttributeValue<string>("trs_partdescription");
                    mServiceAppointmentPartsSummary.trs_tasklistquantity = entity.GetAttributeValue<int>("trs_tasklistquantity");
                    mServiceAppointmentPartsSummary.trs_manualquantity = entity.GetAttributeValue<int>("trs_manualquantity");
                    mServiceAppointmentPartsSummary.trs_acceptedquantity = entity.GetAttributeValue<int>("trs_acceptedquantity");
                    mServiceAppointmentPartsSummary.trs_returnedquantity = entity.GetAttributeValue<int>("trs_returnedquantity");
                    if (entity.Attributes.Contains("modifiedon"))
                        mServiceAppointmentPartsSummary.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                    else
                        ErrorMessage_NotFound("modifiedon");
                    if (entity.Attributes.Contains("statecode"))
                        mServiceAppointmentPartsSummary.statecode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                    else
                        ErrorMessage_NotFound("statecode");
                    if (entity.Attributes.Contains("statuscode"))
                        mServiceAppointmentPartsSummary.statuscode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                    else
                        ErrorMessage_NotFound("statuscode");

                    Connect(organizationService, MethodBase.GetCurrentMethod().Name, activityId);
                    _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                        + "Sending Data ...");
                    synchronizationResult = _client.SaveEntity(mServiceAppointmentPartsSummary);
                    _client.Close();
                    _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                        + "Sending Data Done ...");

                    _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                        + "Checking Result ...");
                    if (synchronizationResult.Result == Result.Failed)
                    {
                        ErrorHandler(synchronizationResult, _DL_trs_workorderpartssummary.EntityName, _DL_trs_workorderpartssummary.DisplayName, mServiceAppointmentPartsSummary.trs_workorderpartssummaryId);
                    }
                    _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                        + "Checking Result Done.");
                }
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Complete ...");
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendServiceAppointment_PartsSummary : " + ex.Message);
            }
        }

        public void SendServiceAppointment_SupportingMaterial(IOrganizationService organizationService, Guid activityId)
        {
            try
            {
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Start ...");
                SynchronizationResult synchronizationResult = new SynchronizationResult();
                trs_workordersupportingmaterialBase mServiceAppointmentSupportingMaterial = new trs_workordersupportingmaterialBase();

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Get Data ...");
                QueryExpression queryExpression = new QueryExpression(_DL_trs_workordersupportingmaterial.EntityName);
                queryExpression.ColumnSet = new ColumnSet(true);
                FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                filterExpression.AddCondition("trs_workorderid", ConditionOperator.Equal, activityId);
                filterExpression.AddCondition("trs_supportingmaterialtype", ConditionOperator.Equal, false);

                EntityCollection entityCollection = _DL_trs_workordersupportingmaterial.Select(organizationService, queryExpression);
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Get Data Done.");

                foreach (Entity entity in entityCollection.Entities)
                {
                    synchronizationResult = new SynchronizationResult();
                    mServiceAppointmentSupportingMaterial = new trs_workordersupportingmaterialBase();
                    if (entity.Attributes.Contains("trs_workordersupportingmaterialid"))
                        mServiceAppointmentSupportingMaterial.trs_workordersupportingmaterialId = entity.GetAttributeValue<Guid>("trs_workordersupportingmaterialid");
                    else
                        ErrorMessage_NotFound("trs_workordersupportingmaterialid");
                    if (entity.Attributes.Contains("trs_workorderid"))
                        mServiceAppointmentSupportingMaterial.trs_workorderid = entity.GetAttributeValue<EntityReference>("trs_workorderid").Id;
                    else
                        ErrorMessage_NotFound("trs_workorderid");
                    if (entity.Attributes.Contains("trs_supportingmaterialname"))
                        mServiceAppointmentSupportingMaterial.trs_supportingmaterialname = entity.GetAttributeValue<string>("trs_supportingmaterialname");
                    else
                        ErrorMessage_NotFound("trs_supportingmaterialname");
                    if (entity.Attributes.Contains("trs_quantity"))
                        mServiceAppointmentSupportingMaterial.trs_quantity = entity.GetAttributeValue<int>("trs_quantity");
                    else
                        ErrorMessage_NotFound("trs_quantity");
                    if (entity.Attributes.Contains("modifiedon"))
                        mServiceAppointmentSupportingMaterial.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                    else
                        ErrorMessage_NotFound("modifiedon");
                    if (entity.Attributes.Contains("statecode"))
                        mServiceAppointmentSupportingMaterial.statecode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                    else
                        ErrorMessage_NotFound("statecode");
                    if (entity.Attributes.Contains("statuscode"))
                        mServiceAppointmentSupportingMaterial.statuscode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                    else
                        ErrorMessage_NotFound("statuscode");

                    Connect(organizationService, MethodBase.GetCurrentMethod().Name, activityId);
                    _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                        + "Sending Data ...");
                    synchronizationResult = _client.SaveEntity(mServiceAppointmentSupportingMaterial);
                    _client.Close();
                    _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                        + "Sending Data Done ...");

                    _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                        + "Checking Result ...");
                    if (synchronizationResult.Result == Result.Failed)
                    {
                        ErrorHandler(synchronizationResult, _DL_trs_workordersupportingmaterial.EntityName, _DL_trs_workordersupportingmaterial.DisplayName, mServiceAppointmentSupportingMaterial.trs_workordersupportingmaterialId);
                    }
                    _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                        + "Checking Result Done.");
                }
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Complete ...");
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendServiceAppointment_SupportingMaterial : " + ex.Message);
            }
        }

        public void SendServiceAppointment_CommercialHeader(IOrganizationService organizationService, Guid activityId
            , bool withDetail, bool withMechanic)
        {
            try
            {
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Start ...");
                SynchronizationResult synchronizationResult = new SynchronizationResult();
                TaskBase mServiceAppointmentCommercialHeader = new TaskBase();

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Get Data ...");
                QueryExpression queryExpression = new QueryExpression(_DL_task.EntityName);
                queryExpression.ColumnSet = new ColumnSet(true);
                queryExpression.Criteria.AddCondition("trs_operationid", ConditionOperator.Equal, activityId);

                EntityCollection entityCollection = _DL_task.Select(organizationService, queryExpression);
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Get Data Done.");

                foreach (Entity entity in entityCollection.Entities)
                {
                    synchronizationResult = new SynchronizationResult();
                    mServiceAppointmentCommercialHeader = new TaskBase();

                    #region Required
                    if (entity.Attributes.Contains("activityid"))
                        mServiceAppointmentCommercialHeader.ActivityId = entity.GetAttributeValue<Guid>("activityid");
                    else
                        ErrorMessage_NotFound("activityid");
                    if (entity.Attributes.Contains("trs_operationid"))
                        mServiceAppointmentCommercialHeader.trs_OperationId = entity.GetAttributeValue<EntityReference>("trs_operationid").Id;
                    else
                        ErrorMessage_NotFound("trs_operationid");
                    if (entity.Attributes.Contains("trs_tasklistheader"))
                        mServiceAppointmentCommercialHeader.trs_tasklistheader = entity.GetAttributeValue<EntityReference>("trs_tasklistheader").Id;
                    else
                        ErrorMessage_NotFound("trs_tasklistheader");
                    if (entity.Attributes.Contains("trs_totalrtg"))
                        mServiceAppointmentCommercialHeader.trs_TotalRTG = entity.GetAttributeValue<decimal>("trs_totalrtg");
                    else
                        ErrorMessage_NotFound("trs_totalrtg");
                    if (entity.Attributes.Contains("modifiedon"))
                        mServiceAppointmentCommercialHeader.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                    else
                        ErrorMessage_NotFound("modifiedon");
                    #endregion

                    #region Optional
                    if (entity.Attributes.Contains("trs_itemnumber"))
                        mServiceAppointmentCommercialHeader.trs_ItemNumber = entity.GetAttributeValue<int>("trs_itemnumber");
                    #endregion

                    SendActivityPointer(organizationService, mServiceAppointmentCommercialHeader.ActivityId);
                    Connect(organizationService, MethodBase.GetCurrentMethod().Name, activityId);
                    _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                        + "Sending Data ...");
                    synchronizationResult = _client.SaveEntity(mServiceAppointmentCommercialHeader);
                    _client.Close();
                    _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                        + "Sending Data Done ...");

                    _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                        + "Checking Result ...");
                    if (synchronizationResult.Result == Result.Failed)
                    {
                        ErrorHandler(synchronizationResult, _DL_task.EntityName, _DL_task.DisplayName, mServiceAppointmentCommercialHeader.ActivityId);
                    }
                    else
                    {
                        if (withDetail)
                            SendServiceAppointment_CommercialDetail(organizationService, entity.GetAttributeValue<Guid>("activityid"), withMechanic);
                    }
                    _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                        + "Checking Result Done.");
                }
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Complete ...");
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendServiceAppointment_CommercialHeader : " + ex.Message);
            }
        }

        public void SendServiceAppointment_CommercialDetail(IOrganizationService organizationService, Guid taskId
            , bool withMechanic)
        {
            try
            {
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Start ...");
                SynchronizationResult synchronizationResult = new SynchronizationResult();
                trs_commercialdetailBase mServiceAppointmentCommercialDetail = new trs_commercialdetailBase();

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Get Data ...");
                QueryExpression queryExpression = new QueryExpression(_DL_trs_commercialdetail.EntityName);
                queryExpression.ColumnSet = new ColumnSet(true);
                queryExpression.Criteria.AddCondition("trs_commercialheaderid", ConditionOperator.Equal, taskId);

                EntityCollection entityCollection = _DL_trs_commercialdetail.Select(organizationService, queryExpression);
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Get Data Done.");

                foreach (Entity entity in entityCollection.Entities)
                {
                    synchronizationResult = new SynchronizationResult();
                    mServiceAppointmentCommercialDetail = new trs_commercialdetailBase();

                    #region Required
                    if (entity.Attributes.Contains("trs_commercialdetailid"))
                        mServiceAppointmentCommercialDetail.trs_commercialdetailId = entity.GetAttributeValue<Guid>("trs_commercialdetailid");
                    else
                        ErrorMessage_NotFound("trs_commercialdetailid");
                    if (entity.Attributes.Contains("trs_workorder"))
                        mServiceAppointmentCommercialDetail.trs_workorder = entity.GetAttributeValue<EntityReference>("trs_workorder").Id;
                    else
                        ErrorMessage_NotFound("trs_workorder");
                    if (entity.Attributes.Contains("trs_commercialheaderid"))
                        mServiceAppointmentCommercialDetail.trs_commercialheaderid = entity.GetAttributeValue<EntityReference>("trs_commercialheaderid").Id;
                    else
                        ErrorMessage_NotFound("trs_commercialheaderid");
                    if (entity.Attributes.Contains("trs_commercialtask"))
                        mServiceAppointmentCommercialDetail.trs_commercialtask = entity.GetAttributeValue<EntityReference>("trs_commercialtask").Id;
                    else
                        ErrorMessage_NotFound("trs_commercialtask");
                    if (entity.Attributes.Contains("trs_taskname"))
                        mServiceAppointmentCommercialDetail.trs_taskname = entity.GetAttributeValue<EntityReference>("trs_taskname").Id;
                    else
                        ErrorMessage_NotFound("trs_taskname");
                    if (entity.Attributes.Contains("trs_taskcode"))
                        mServiceAppointmentCommercialDetail.trs_taskcode = entity.GetAttributeValue<string>("trs_taskcode");
                    else
                        ErrorMessage_NotFound("trs_taskcode");
                    if (entity.Attributes.Contains("trs_mechanicgrade"))
                        mServiceAppointmentCommercialDetail.trs_mechanicgrade = entity.GetAttributeValue<EntityReference>("trs_mechanicgrade").Id;
                    else
                        ErrorMessage_NotFound("trs_mechanicgrade");
                    if (entity.Attributes.Contains("trs_rtg"))
                        mServiceAppointmentCommercialDetail.trs_rtg = entity.GetAttributeValue<decimal>("trs_rtg");
                    else
                        ErrorMessage_NotFound("trs_rtg");
                    if (entity.Attributes.Contains("modifiedon"))
                        mServiceAppointmentCommercialDetail.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                    else
                        ErrorMessage_NotFound("modifiedon");
                    if (entity.Attributes.Contains("statecode"))
                        mServiceAppointmentCommercialDetail.StateCode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                    else
                        ErrorMessage_NotFound("statecode");
                    if (entity.Attributes.Contains("statuscode"))
                        mServiceAppointmentCommercialDetail.StatusCode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                    else
                        ErrorMessage_NotFound("statuscode");
                    #endregion

                    #region Optional
                    if (entity.Attributes.Contains("trs_automatictime"))
                        mServiceAppointmentCommercialDetail.trs_automatictime = entity.GetAttributeValue<DateTime>("trs_automatictime");
                    if (entity.Attributes.Contains("trs_manualtime"))
                        mServiceAppointmentCommercialDetail.trs_manualtime = entity.GetAttributeValue<DateTime>("trs_manualtime");
                    #endregion

                    Connect(organizationService, MethodBase.GetCurrentMethod().Name, taskId);
                    _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                        + "Sending Data ...");
                    synchronizationResult = _client.SaveEntity(mServiceAppointmentCommercialDetail);
                    _client.Close();
                    _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                        + "Sending Data Done ...");

                    _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                        + "Checking Result ...");
                    if (synchronizationResult.Result == Result.Failed)
                    {
                        ErrorHandler(synchronizationResult, _DL_trs_commercialdetail.EntityName, _DL_trs_commercialdetail.DisplayName, mServiceAppointmentCommercialDetail.trs_commercialdetailId);
                    }
                    else
                    {
                        if (withMechanic)
                            SendServiceAppointment_CommercialDetailMechanic(organizationService, entity.GetAttributeValue<Guid>("trs_commercialdetailid"));
                    }
                    _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                        + "Checking Result Done.");
                }
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Complete ...");
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendServiceAppointment_CommercialDetail : " + ex.Message);
            }
        }

        public void SendServiceAppointment_CommercialDetailMechanic(IOrganizationService organizationService, Guid taskId)
        {
            try
            {
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Start ...");
                SynchronizationResult synchronizationResult = new SynchronizationResult();
                trs_commercialdetailmechanicBase mServiceAppointmentCommercialDetailMechanic = new trs_commercialdetailmechanicBase();

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Get Data ...");
                QueryExpression queryExpression = new QueryExpression(_DL_trs_commercialdetailmechanic.EntityName);
                queryExpression.ColumnSet = new ColumnSet(true);
                queryExpression.Criteria.AddCondition("trs_commercialdetailid", ConditionOperator.Equal, taskId);

                EntityCollection entityCollection = _DL_trs_commercialdetailmechanic.Select(organizationService, queryExpression);
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Get Data Done.");

                foreach (Entity entity in entityCollection.Entities)
                {
                    synchronizationResult = new SynchronizationResult();
                    mServiceAppointmentCommercialDetailMechanic = new trs_commercialdetailmechanicBase();
                    if (entity.Attributes.Contains("trs_commercialdetailmechanicid"))
                        mServiceAppointmentCommercialDetailMechanic.trs_commercialdetailmechanicId = entity.GetAttributeValue<Guid>("trs_commercialdetailmechanicid");
                    else
                        ErrorMessage_NotFound("trs_commercialdetailmechanicid");
                    if (entity.Attributes.Contains("trs_commercialdetailid"))
                        mServiceAppointmentCommercialDetailMechanic.trs_commercialdetailid = entity.GetAttributeValue<EntityReference>("trs_commercialdetailid").Id;
                    else
                        ErrorMessage_NotFound("trs_commercialdetailid");
                    if (entity.Attributes.Contains("trs_equipmentid"))
                        mServiceAppointmentCommercialDetailMechanic.trs_equipmentid = entity.GetAttributeValue<EntityReference>("trs_equipmentid").Id;
                    else
                        ErrorMessage_NotFound("trs_equipmentid");
                    if (entity.Attributes.Contains("trs_nrp"))
                        mServiceAppointmentCommercialDetailMechanic.trs_nrp = entity.GetAttributeValue<string>("trs_nrp");
                    else
                        ErrorMessage_NotFound("trs_nrp");
                    if (entity.Attributes.Contains("trs_mechanicrole"))
                        mServiceAppointmentCommercialDetailMechanic.trs_mechanicrole = entity.GetAttributeValue<OptionSetValue>("trs_mechanicrole").Value;
                    else
                        ErrorMessage_NotFound("trs_mechanicrole");
                    if (entity.Attributes.Contains("modifiedon"))
                        mServiceAppointmentCommercialDetailMechanic.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                    else
                        ErrorMessage_NotFound("modifiedon");
                    if (entity.Attributes.Contains("statecode"))
                        mServiceAppointmentCommercialDetailMechanic.StateCode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                    else
                        ErrorMessage_NotFound("statecode");
                    if (entity.Attributes.Contains("statuscode"))
                        mServiceAppointmentCommercialDetailMechanic.StatusCode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                    else
                        ErrorMessage_NotFound("statuscode");

                    Connect(organizationService, MethodBase.GetCurrentMethod().Name, taskId);
                    _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                        + "Sending Data ...");
                    synchronizationResult = _client.SaveEntity(mServiceAppointmentCommercialDetailMechanic);
                    _client.Close();
                    _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                        + "Sending Data Done ...");

                    _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                        + "Checking Result ...");
                    if (synchronizationResult.Result == Result.Failed)
                    {
                        ErrorHandler(synchronizationResult, _DL_trs_commercialdetailmechanic.EntityName, _DL_trs_commercialdetailmechanic.DisplayName, mServiceAppointmentCommercialDetailMechanic.trs_commercialdetailmechanicId);
                    }
                    _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                        + "Checking Result Done.");
                }
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Complete ...");
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendServiceAppointment_CommercialDetailMechanic : " + ex.Message);
            }
        }

        public void SendServiceAppointment_AllRecommedationbyWO(IOrganizationService organizationService, Guid activityId)
        {
            try
            {
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Start ...");
                QueryExpression queryExpression = new QueryExpression(_DL_trs_workorderpartrecommendation.EntityName);
                EntityCollection eCollection = _DL_trs_workorderpartrecommendation.Select(organizationService, queryExpression);
                foreach (Entity entity in eCollection.Entities)
                {
                    SendServiceAppointment_Recommendation(organizationService, entity.Id);
                }
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Complete ...");
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendServiceAppointment_AllRecommedationbyWO : " + ex.Message);
            }
        }

        public void SendServiceAppointment_Recommendation(IOrganizationService organizationService, Guid id)
        {
            try
            {
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Start ...");
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Get Data ...");
                Entity entity = _DL_trs_workorderpartrecommendation.Select(organizationService, id);
                trs_workorderpartrecommendationBase mServiceAppointmentRecommendation = new trs_workorderpartrecommendationBase();

                if (entity.Attributes.Contains("trs_workorderpartrecommendationid"))
                    mServiceAppointmentRecommendation.trs_workorderpartrecommendationId = new Guid(entity.GetAttributeValue<string>("trs_mobileguid"));
                else
                    ErrorMessage_NotFound("trs_workorderpartrecommendationid");
                if (entity.Attributes.Contains("trs_workorder"))
                    mServiceAppointmentRecommendation.trs_workorder = entity.GetAttributeValue<EntityReference>("trs_workorder").Id;
                else
                    ErrorMessage_NotFound("trs_workorder");
                if (entity.Attributes.Contains("trs_section"))
                    mServiceAppointmentRecommendation.trs_section = entity.GetAttributeValue<EntityReference>("trs_section").Id;
                else
                    ErrorMessage_NotFound("trs_section");
                if (entity.Attributes.Contains("modifiedon"))
                    mServiceAppointmentRecommendation.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                else
                    ErrorMessage_NotFound("modifiedon");
                if (entity.Attributes.Contains("statecode"))
                    mServiceAppointmentRecommendation.statecode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                else
                    ErrorMessage_NotFound("statecode");
                if (entity.Attributes.Contains("statuscode"))
                    mServiceAppointmentRecommendation.statuscode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                else
                    ErrorMessage_NotFound("statuscode");

                #region Optional
                if (entity.Attributes.Contains("trs_tasklistdetailid"))
                    mServiceAppointmentRecommendation.trs_tasklistdetailid = entity.GetAttributeValue<EntityReference>("trs_tasklistdetailid").Id;
                if (entity.Attributes.Contains("trs_partnumber"))
                    mServiceAppointmentRecommendation.trs_partnumber = entity.GetAttributeValue<EntityReference>("trs_partnumber").Id;
                if (entity.Attributes.Contains("trs_quantity"))
                    mServiceAppointmentRecommendation.trs_quantity = entity.GetAttributeValue<int>("trs_quantity");
                #endregion
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Get Data Done.");

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Sending Data ...");
                synchronizationResult = _client.SaveEntity(mServiceAppointmentRecommendation);
                _client.Close();
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Sending Data Done ...");

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Checking Result ...");
                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_trs_workorderpartrecommendation.EntityName, _DL_trs_workorderpartrecommendation.DisplayName, mServiceAppointmentRecommendation.trs_workorderpartrecommendationId);
                }
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Checking Result Done.");

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Complete ...");
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendServiceAppointment_Recommendation : " + ex.Message);
            }
        }

        public void SendServiceAppointment_Documentation(IOrganizationService organizationService, Guid id)
        {
            try
            {
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Start ...");
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Get Data ...");
                Entity entity = _DL_trs_workorderdocumentation.Select(organizationService, id);
                trs_workorderdocumentationBase mServiceAppointmentDocumentation = new trs_workorderdocumentationBase();

                if (entity.Attributes.Contains("trs_workorderid"))
                {
                    mServiceAppointmentDocumentation.trs_WorkOrderId = entity.GetAttributeValue<EntityReference>("trs_workorderid").Id;
                }
                else
                {
                    ErrorMessage_NotFound("trs_workorderid");
                }
                if (entity.Attributes.Contains("trs_mobileguid"))
                {
                    mServiceAppointmentDocumentation.trs_workorderdocumentationId = new Guid(entity.GetAttributeValue<string>("trs_mobileguid"));
                }
                else
                {
                    ErrorMessage_NotFound("trs_mobileguid");
                }
                if (entity.Attributes.Contains("trs_url"))
                {
                    mServiceAppointmentDocumentation.trs_url = entity.GetAttributeValue<String>("trs_url");
                }
                else
                {
                    ErrorMessage_NotFound("trs_url");
                }
                if (entity.Attributes.Contains("trs_description"))
                {
                    mServiceAppointmentDocumentation.trs_Description = entity.GetAttributeValue<string>("trs_description");
                }
                if (entity.Attributes.Contains("modifiedon"))
                {
                    mServiceAppointmentDocumentation.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                }
                else
                {
                    ErrorMessage_NotFound("modifiedon");
                }
                if (entity.Attributes.Contains("statecode"))
                {
                    mServiceAppointmentDocumentation.statecode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                }
                else
                {
                    ErrorMessage_NotFound("statecode");
                }
                if (entity.Attributes.Contains("statuscode"))
                {
                    mServiceAppointmentDocumentation.statuscode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                }
                else
                {
                    ErrorMessage_NotFound("statuscode");
                }
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Get Data Done.");

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Sending Data ...");
                synchronizationResult = _client.SaveEntity(mServiceAppointmentDocumentation);
                _client.Close();
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Sending Data Done ...");

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Checking Result ...");
                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_trs_tsrpartdetails.EntityName, _DL_trs_tsrpartdetails.DisplayName, id);
                }
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Checking Result Done.");

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Complete ...");
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendTSRDocumentation : " + ex.Message);
            }
        }
        #endregion

        #region MTAR
        public string ConvertMTARtoWords(int status)
        {
            switch (status % 167630000)
            {
                case 1:
                    return "Ready to Go";
                case 2:
                    return "Arrived";
                case 3:
                    return "Standby";
                case 4:
                    return "Ready to Repair";
                case 5:
                    return "Hold";
                case 6:
                    return "Done for Today";
                case 7:
                    return "Resume";
                case 8:
                    return "Repair Done";
                case 9:
                    return "Finish WO";
                case 10:
                    return "Arrived at Office";
                case 11:
                    return "Submit Teco";
                default:
                    return "Prepare at Office";
            }
        }

        public int ConvertMTAR_MobiletoCRM(int status)
        {
            try
            {
                //return (167630000 + status);
                return status;
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".ConvertMTAR_MobiletoCRM : " + ex.Message);
            }
        }
        public int ConvertMTAR_CRMtoMobile(int status)
        {
            try
            {
                //return (status - 167630000);
                return status;
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".ConvertMTAR_CRMtoMobile : " + ex.Message);
            }
        }

        public void SendMTAR(IOrganizationService organizationService, Guid id)
        {
            try
            {
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Start ...");
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Get Data ...");
                Entity entity = _DL_trs_mtar.Select(organizationService, id);
                trs_mtarBase mMTAR = new trs_mtarBase();
                mMTAR.trs_mtarId = new Guid(entity.GetAttributeValue<string>("trs_mobileguid"));
                if (entity.Attributes.Contains("trs_workorder"))
                {
                    mMTAR.trs_WorkOrder = entity.GetAttributeValue<EntityReference>("trs_workorder").Id;
                }
                if (entity.Attributes.Contains("trs_latitude"))
                {
                    mMTAR.trs_latitude = entity.GetAttributeValue<decimal>("trs_latitude");
                }
                if (entity.Attributes.Contains("trs_longitude"))
                {
                    mMTAR.trs_longitude = entity.GetAttributeValue<decimal>("trs_longitude");
                }
                if (entity.Attributes.Contains("trs_mechanic"))
                {
                    mMTAR.trs_Mechanic = entity.GetAttributeValue<EntityReference>("trs_mechanic").Id;
                }
                if (entity.Attributes.Contains("trs_mtarstatus"))
                {
                    mMTAR.trs_mtarstatus = entity.GetAttributeValue<OptionSetValue>("trs_mtarstatus").Value;
                }
                if (entity.Attributes.Contains("trs_automatictime"))
                {
                    mMTAR.trs_AutomaticTime = entity.GetAttributeValue<DateTime>("trs_automatictime").ToUniversalTime();
                }
                if (entity.Attributes.Contains("trs_manualtime"))
                {
                    mMTAR.trs_ManualTime = entity.GetAttributeValue<DateTime>("trs_manualtime").ToUniversalTime();
                }
                if (entity.Attributes.Contains("modifiedon"))
                {
                    mMTAR.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                }
                else 
                {
                    ErrorMessage_NotFound("modifiedon");
                }
                if (entity.Attributes.Contains("statecode"))
                {
                    mMTAR.StateCode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                }
                else
                {
                    ErrorMessage_NotFound("statecode");
                }
                if (entity.Attributes.Contains("statuscode"))
                {
                    mMTAR.StatusCode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                }
                else
                {
                    ErrorMessage_NotFound("statuscode");
                }
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Get Data Done.");

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Sending Data ...");
                synchronizationResult = _client.SaveEntity(mMTAR);
                _client.Close();
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Sending Data Done ...");

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Checking Result ...");
                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_trs_mtar.EntityName, _DL_trs_mtar.DisplayName, id);
                }
                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Checking Result Done.");

                _logCreator.Write(MethodBase.GetCurrentMethod().Name + _logCreator.ColumnsSeparator
                    + "Complete ...");
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendMTAR : " + ex.Message);
            }
        }

        public void SendFunctionalLocation(IOrganizationService organizationService, Guid id)
        {
            try
            {
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                Entity entity = _DL_trs_functionallocation.Select(organizationService, id);
                trs_functionallocationBase mFunctionallocation = new trs_functionallocationBase();
                if (entity.Attributes.Contains("trs_functionallocationid"))
                    mFunctionallocation.trs_functionallocationId = entity.GetAttributeValue<Guid>("trs_functionallocationid");
                else
                    ErrorMessage_NotFound("trs_functionallocationid");
                mFunctionallocation.trs_FunctionalCode = entity.GetAttributeValue<string>("trs_functionalcode");
                mFunctionallocation.trs_name = entity.GetAttributeValue<string>("trs_name");
                mFunctionallocation.trs_Customer = entity.GetAttributeValue<EntityReference>("trs_customer").Id;
                mFunctionallocation.trs_FunctionalAddress = entity.GetAttributeValue<string>("trs_functionaladdress");
                mFunctionallocation.trs_Area = entity.GetAttributeValue<string>("trs_area");
                if (entity.Attributes.Contains("trs_functionallatitude"))
                    mFunctionallocation.trs_functionallatitude = entity.GetAttributeValue<decimal>("trs_functionallatitude");
                if (entity.Attributes.Contains("trs_functionallongitude"))
                    mFunctionallocation.trs_functionallongitude = entity.GetAttributeValue<decimal>("trs_functionallongitude");
                if (entity.Attributes.Contains("modifiedon"))
                    mFunctionallocation.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                else
                    ErrorMessage_NotFound("modifiedon");
                if (entity.Attributes.Contains("statecode"))
                    mFunctionallocation.StateCode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                else
                    ErrorMessage_NotFound("statecode");
                if (entity.Attributes.Contains("statuscode"))
                    mFunctionallocation.StatusCode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                else
                    ErrorMessage_NotFound("statuscode");

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                synchronizationResult = _client.SaveEntity(mFunctionallocation);
                _client.Close();
                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_trs_functionallocation.EntityName, _DL_trs_functionallocation.DisplayName, id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendFunctionalLocation : " + ex.Message);
            }
        }
        #endregion

        #region Technical Service Report
        public void SendTSR(IOrganizationService organizationService, Guid id)
        {
            try
            {
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                Entity entity = _DL_trs_technicalservicereport.Select(organizationService, id);

                trs_technicalservicereportBase tsr = new trs_technicalservicereportBase();
                if (entity.Attributes.Contains("trs_workorder"))
                {
                    tsr.trs_WorkOrder = entity.GetAttributeValue<EntityReference>("trs_workorder").Id;
                }
                else {
                    ErrorMessage_NotFound("trs_workorder");
                }
                if (entity.Attributes.Contains("trs_technicalservicereportId"))
                    tsr.trs_technicalservicereportId = new Guid(entity.GetAttributeValue<string>("trs_mobileguid"));
                if (entity.Attributes.Contains("trs_tsrnumber"))
                    tsr.trs_TSRNumber = entity.GetAttributeValue<string>("trs_tsrnumber");
                if (entity.Attributes.Contains("trs_conditiondescription"))
                    tsr.trs_ConditionDescription = entity.GetAttributeValue<string>("trs_conditiondescription");
                if (entity.Attributes.Contains("trs_application"))
                    tsr.trs_Application = entity.GetAttributeValue<OptionSetValue>("trs_application").Value;
                if (entity.Attributes.Contains("trs_correctiontaken"))
                    tsr.trs_CorrectionTaken = entity.GetAttributeValue<string>("trs_correctiontaken");
                if (entity.Attributes.Contains("trs_jobstatus"))
                    tsr.trs_JobStatus = entity.GetAttributeValue<bool>("trs_jobstatus");
                if (entity.Attributes.Contains("trs_repairdate"))
                    tsr.trs_RepairDate = entity.GetAttributeValue<DateTime>("trs_repairdate");
                if (entity.Attributes.Contains("trs_troubledate"))
                    tsr.trs_TroubleDate = entity.GetAttributeValue<DateTime>("trs_troubledate");
                if (entity.Attributes.Contains("trs_typeofsoil"))
                    tsr.trs_TypeofSoil = entity.GetAttributeValue<OptionSetValue>("trs_typeofsoil").Value;
                if (entity.Attributes.Contains("trs_notfinishreason"))
                    tsr.trs_NotFinishReason = entity.GetAttributeValue<OptionSetValue>("trs_notfinishreason").Value;
                if (entity.Attributes.Contains("trs_operatingcondition"))
                    tsr.trs_OperatingCondition = entity.GetAttributeValue<bool>("trs_operatingcondition");
                if (entity.Attributes.Contains("trs_equipment"))
                    tsr.trs_Equipment = entity.GetAttributeValue<EntityReference>("trs_equipment").Id;
                if (entity.Attributes.Contains("trs_warrantystatus"))
                    tsr.trs_WarrantyStatus = entity.GetAttributeValue<bool>("trs_warrantystatus");
                if (entity.Attributes.Contains("trs_producttype"))
                    tsr.trs_ProductType = entity.GetAttributeValue<EntityReference>("trs_producttype").Id;
                if (entity.Attributes.Contains("trs_symptom"))
                    tsr.trs_Symptom = entity.GetAttributeValue<string>("trs_symptom");
                if (entity.Attributes.Contains("modifiedon"))
                    tsr.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                else
                    ErrorMessage_NotFound("modifiedon");
                if (entity.Attributes.Contains("statecode"))
                    tsr.statecode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                else
                    ErrorMessage_NotFound("statecode");
                if (entity.Attributes.Contains("statuscode"))
                    tsr.statuscode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                else
                    ErrorMessage_NotFound("statuscode");
                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                synchronizationResult = _client.SaveEntity(tsr);
                _client.Close();

                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_trs_technicalservicereport.EntityName, _DL_trs_technicalservicereport.DisplayName, id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendTSR : " + ex.Message);
            }
        }

        public void SendTSRPartDetails(IOrganizationService organizationService, Guid id)
        {
            try
            {
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                Entity entity = _DL_trs_tsrpartdetails.Select(organizationService, id);
                trs_tsrpartdetailsBase mTSRPartDetails = new trs_tsrpartdetailsBase();

                if (entity.Attributes.Contains("trs_wonumber"))
                {
                    mTSRPartDetails.trs_WONumber = entity.GetAttributeValue<EntityReference>("trs_wonumber").Id;
                }
                else
                {
                    ErrorMessage_NotFound("trs_wonumber");
                }

                if (entity.Attributes.Contains("trs_mobileguid"))
                {
                    mTSRPartDetails.trs_tsrpartdetailsId = new Guid(entity.GetAttributeValue<string>("trs_mobileguid"));
                }
                else
                {
                    ErrorMessage_NotFound("trs_mobileguid");
                }

                if (entity.Attributes.Contains("trs_partnumber"))
                {
                    mTSRPartDetails.trs_PartNumber = entity.GetAttributeValue<EntityReference>("trs_partnumber").Id;
                }
                else
                {
                    ErrorMessage_NotFound("trs_partnumber");
                }
                if (entity.Attributes.Contains("trs_quantity"))
                {
                    mTSRPartDetails.trs_Quantity = entity.GetAttributeValue<int>("trs_quantity");
                }
                if (entity.Attributes.Contains("modifiedon"))
                    mTSRPartDetails.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                else
                    ErrorMessage_NotFound("modifiedon");
                if (entity.Attributes.Contains("statecode"))
                    mTSRPartDetails.statecode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                else
                    ErrorMessage_NotFound("statecode");
                if (entity.Attributes.Contains("statuscode"))
                    mTSRPartDetails.statuscode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                else
                    ErrorMessage_NotFound("statuscode");

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                synchronizationResult = _client.SaveEntity(mTSRPartDetails);
                _client.Close();
                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_trs_tsrpartdetails.EntityName, _DL_trs_tsrpartdetails.DisplayName, id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendTSRPartDetails : " + ex.Message);
            }
        }
        
        public void SendTSRPartsDamagedDetail(IOrganizationService organizationService, Guid id)
        {
            try
            {
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                Entity entity = _DL_trs_tsrpartsdamageddetail.Select(organizationService, id);
                trs_tsrpartsdamageddetailBase mTSRPartsDamagedDetail = new trs_tsrpartsdamageddetailBase();
                if (entity.Attributes.Contains("trs_wonumber"))
                {
                    mTSRPartsDamagedDetail.trs_WONumber = entity.GetAttributeValue<EntityReference>("trs_wonumber").Id;
                }
                else
                {
                    ErrorMessage_NotFound("trs_wonumber");
                }

                if (entity.Attributes.Contains("trs_partnumber"))
                {
                    mTSRPartsDamagedDetail.trs_PartNumber = entity.GetAttributeValue<EntityReference>("trs_partnumber").Id;
                }
                else
                {
                    ErrorMessage_NotFound("trs_partnumber");
                }

                if (entity.Attributes.Contains("trs_mobileguid"))
                {
                    mTSRPartsDamagedDetail.trs_tsrpartsdamageddetailId = new Guid(entity.GetAttributeValue<string>("trs_mobileguid"));
                }
                else
                {
                    ErrorMessage_NotFound("trs_mobileguid");
                }

                mTSRPartsDamagedDetail.trs_Quantity = entity.GetAttributeValue<int>("trs_quantity");

                if (entity.Attributes.Contains("modifiedon"))
                    mTSRPartsDamagedDetail.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                else
                    ErrorMessage_NotFound("modifiedon");
                if (entity.Attributes.Contains("statecode"))
                    mTSRPartsDamagedDetail.statecode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                else
                    ErrorMessage_NotFound("statecode");
                if (entity.Attributes.Contains("statuscode"))
                    mTSRPartsDamagedDetail.statuscode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                else
                    ErrorMessage_NotFound("statuscode");

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                synchronizationResult = _client.SaveEntity(mTSRPartsDamagedDetail);
                _client.Close();
                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_trs_tsrpartsdamageddetail.EntityName, _DL_trs_tsrpartsdamageddetail.DisplayName, id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendTSRPartsDamagedDetail : " + ex.Message);
            }
        }

        public void SendTSRDocumentation(IOrganizationService organizationService, Guid id)
        {
            try
            {
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                Entity entity = _DL_trs_technicalservicereportdocumentation.Select(organizationService, id);
                trs_technicalservicereportdocumentationBase mTSRDocumentation = new trs_technicalservicereportdocumentationBase();

                if (entity.Attributes.Contains("trs_workorder"))
                {
                    mTSRDocumentation.trs_WorkOrder = entity.GetAttributeValue<EntityReference>("trs_workorder").Id;
                }
                else
                {
                    ErrorMessage_NotFound("trs_workorder");
                }
                if (entity.Attributes.Contains("trs_mobileguid"))
                {
                    mTSRDocumentation.trs_technicalservicereportdocumentationId = new Guid(entity.GetAttributeValue<string>("trs_mobileguid"));
                }
                else
                {
                    ErrorMessage_NotFound("trs_mobileguid");
                }
                if (entity.Attributes.Contains("trs_url"))
                {
                    mTSRDocumentation.trs_url = entity.GetAttributeValue<String>("trs_url");
                }
                else
                {
                    ErrorMessage_NotFound("trs_url");
                }
                if (entity.Attributes.Contains("trs_description"))
                {
                    mTSRDocumentation.trs_Description = entity.GetAttributeValue<string>("trs_description");
                }
                if (entity.Attributes.Contains("modifiedon"))
                {
                    mTSRDocumentation.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                }
                else
                {
                    ErrorMessage_NotFound("modifiedon");
                }
                if (entity.Attributes.Contains("statecode"))
                {
                    mTSRDocumentation.statecode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                }
                else
                {
                    ErrorMessage_NotFound("statecode");
                }
                if (entity.Attributes.Contains("statuscode"))
                {
                    mTSRDocumentation.statuscode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                }
                else
                {
                    ErrorMessage_NotFound("statuscode");
                }

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                synchronizationResult = _client.SaveEntity(mTSRDocumentation);
                _client.Close();
                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_trs_tsrpartdetails.EntityName, _DL_trs_tsrpartdetails.DisplayName, id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendTSRDocumentation : " + ex.Message);
            }
        }
        #endregion

        #region PPM
        public void SendPPMInspectionReport(IOrganizationService organizationService, Guid id)
        {
            try
            {
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                Entity entity = _DL_trs_ppmreport.Select(organizationService, id);
                trs_ppmreportBase ppm = new trs_ppmreportBase();

                if (entity.Attributes.Contains("trs_workorder"))
                {
                    ppm.trs_WorkOrder = entity.GetAttributeValue<EntityReference>("trs_workorder").Id;
                }
                else
                {
                    ErrorMessage_NotFound("trs_workorder");
                }
                if (entity.Attributes.Contains("trs_mobileguid"))
                {
                    ppm.trs_ppmreportId = new Guid(entity.GetAttributeValue<string>("trs_mobileguid"));
                }
                else
                {
                    ErrorMessage_NotFound("trs_mobileguid");
                }

                if (entity.Attributes.Contains("trs_reportnumber"))
                {
                    ppm.trs_reportnumber = entity.GetAttributeValue<string>("trs_reportnumber");
                }

                if (entity.Attributes.Contains("trs_analysis"))
                {
                    ppm.trs_Analysis = entity.GetAttributeValue<string>("trs_analysis");
                }
                if (entity.Attributes.Contains("trs_application"))
                {
                    ppm.trs_Application = entity.GetAttributeValue<OptionSetValue>("trs_application").Value;
                }
                if (entity.Attributes.Contains("trs_comments"))
                {
                    ppm.trs_Comments = entity.GetAttributeValue<OptionSetValue>("trs_comments").Value;
                }
                if (entity.Attributes.Contains("trs_machinecondition"))
                {
                    ppm.trs_MachineCondition = entity.GetAttributeValue<OptionSetValue>("trs_machinecondition").Value;
                }
                if (entity.Attributes.Contains("trs_repairdate"))
                {
                    ppm.trs_RepairDate = entity.GetAttributeValue<DateTime>("trs_repairdate").ToUniversalTime();
                }
                if (entity.Attributes.Contains("trs_repairdate"))
                {
                    ppm.trs_TroubleDate = entity.GetAttributeValue<DateTime>("trs_troubledate").ToUniversalTime();
                }
                if (entity.Attributes.Contains("trs_typeofsoil"))
                {
                    ppm.trs_TypeofSoil = entity.GetAttributeValue<OptionSetValue>("trs_typeofsoil").Value;
                }
                if (entity.Attributes.Contains("trs_typeofwork"))
                {
                    ppm.trs_TypeofWork = entity.GetAttributeValue<EntityReference>("trs_typeofwork").Id;
                }
                if (entity.Attributes.Contains("trs_equipment"))
                {
                    ppm.trs_Equipment = entity.GetAttributeValue<EntityReference>("trs_equipment").Id;
                }

                if (entity.Attributes.Contains("modifiedon"))
                    ppm.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                else
                    ErrorMessage_NotFound("modifiedon");
                if (entity.Attributes.Contains("statecode"))
                    ppm.StateCode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                else
                    ErrorMessage_NotFound("statecode");
                if (entity.Attributes.Contains("statuscode"))
                    ppm.StatusCode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                else
                    ErrorMessage_NotFound("statuscode");

                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                synchronizationResult = _client.SaveEntity(ppm);
                _client.Close();
                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_trs_ppmreport.EntityName, _DL_trs_ppmreport.DisplayName, id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendPpmInspectionReport : " + ex.Message);
            }
        }

        public void SendPPMRecommendation(IOrganizationService organizationService, Guid id)
        {
            try
            {
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                Entity entity = _DL_trs_sectionrecommendation.Select(organizationService, id);
                trs_sectionrecommendationBase mPPMRecommendation = new trs_sectionrecommendationBase();

                if (entity.Attributes.Contains("trs_wonumber"))
                {
                    mPPMRecommendation.trs_WONumber = entity.GetAttributeValue<EntityReference>("trs_wonumber").Id;
                }
                else
                {
                    ErrorMessage_NotFound("trs_wonumber");
                }

                if (entity.Attributes.Contains("trs_mobileguid"))
                {
                    mPPMRecommendation.trs_sectionrecommendationId = new Guid(entity.GetAttributeValue<string>("trs_mobileguid"));
                }
                else
                {
                    ErrorMessage_NotFound("trs_sectionrecommendationid");
                }

                if (entity.Attributes.Contains("trs_name"))
                {
                    mPPMRecommendation.trs_name = entity.GetAttributeValue<string>("trs_name");
                }

                if (entity.Attributes.Contains("modifiedon"))
                {
                    mPPMRecommendation.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                }
                else
                {
                    ErrorMessage_NotFound("modifiedon");
                }
                if (entity.Attributes.Contains("statecode"))
                {
                    mPPMRecommendation.StateCode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                }
                else
                {
                    ErrorMessage_NotFound("statecode");
                }
                if (entity.Attributes.Contains("statuscode"))
                {
                    mPPMRecommendation.StatusCode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                }
                else
                {
                    ErrorMessage_NotFound("statuscode");
                }
                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                synchronizationResult = _client.SaveEntity(mPPMRecommendation);
                _client.Close();
                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_trs_ppmreport.EntityName, _DL_trs_ppmreport.DisplayName, id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendPPMRecommendation : " + ex.Message);
            }
        }
        #endregion

        #region Case / Service Requisition / Incident
        public void SendIncident(IOrganizationService organizationService, Guid id)
        {
            try
            {
                SynchronizationResult synchronizationResult = new SynchronizationResult();

                Entity entity = _DL_incident.Select(organizationService, id);
                IncidentBase incident = new IncidentBase();
                if (entity.Attributes.Contains("incidentid"))
                    incident.IncidentId = entity.GetAttributeValue<Guid>("incidentid");
                else
                    ErrorMessage_NotFound("incidentid");
                if (entity.Attributes.Contains("customerid"))
                    incident.trs_Customer = ((EntityReference)entity.Attributes["customerid"]).Id;
                else
                    ErrorMessage_NotFound("customerid");
                if (entity.Attributes.Contains("ticketnumber"))
                    incident.TicketNumber = entity.GetAttributeValue<string>("ticketnumber");
                if (entity.Attributes.Contains("trs_parentsrid"))
                    incident.trs_parentsrid = ((EntityReference)entity.Attributes["trs_parentsrid"]).Id;
                if (entity.Attributes.Contains("modifiedon"))
                    incident.ModifiedOn = entity.GetAttributeValue<DateTime>("modifiedon").ToUniversalTime();
                else
                    ErrorMessage_NotFound("modifiedon");
                if (entity.Attributes.Contains("statecode"))
                    incident.StateCode = entity.GetAttributeValue<OptionSetValue>("statecode").Value;
                else
                    ErrorMessage_NotFound("statecode");
                if (entity.Attributes.Contains("statuscode"))
                    incident.StatusCode = entity.GetAttributeValue<OptionSetValue>("statuscode").Value;
                else
                    ErrorMessage_NotFound("statuscode");
                if (entity.Attributes.Contains("createdon"))
                    incident.CreatedOn = entity.GetAttributeValue<DateTime>("createdon").ToUniversalTime();
                else
                    ErrorMessage_NotFound("createdon");

                if (entity.Attributes.Contains("description"))
                    incident.Description = entity.GetAttributeValue<string>("description");
                
                Connect(organizationService, MethodBase.GetCurrentMethod().Name, id);
                synchronizationResult = _client.SaveEntity(incident);
                _client.Close();
                if (synchronizationResult.Result == Result.Failed)
                {
                    ErrorHandler(synchronizationResult, _DL_incident.EntityName, _DL_incident.DisplayName, id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendIncident : " + ex.Message);
            }
        }
        #endregion

        #endregion
    }
}
