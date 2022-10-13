using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using TrakNusRapidService.DataLayer;
using TrakNusRapidService.Helper;

namespace TrakNusRapidServiceWcfService.BusinessLayer
{
    public class BL_serviceappointment
    {
        #region Constants
        private const string _classname = "BL_serviceappointment";
        #endregion

        #region Dependencies
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        private DL_serviceappointment _DL_serviceappointment = new DL_serviceappointment();

        private BL_trs_mtar _BL_trs_mtar = new BL_trs_mtar();
        private WOGenerator _woGenerator = new WOGenerator();
        #endregion

        #region Privates
        private Guid GetWOSDH(IOrganizationService organizationService, Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new Exception("Primary Key can not empty !");
                }
                else
                {
                    Entity entity = _DL_serviceappointment.Select(organizationService, id);
                    if (entity.Attributes.Contains("owninguser"))
                    {
                        QueryExpression queryExpression = new QueryExpression("systemuser");
                        queryExpression.ColumnSet = new ColumnSet("businessunitid");

                        LinkEntity systemuser = new LinkEntity();
                        systemuser.LinkFromEntityName = _DL_systemuser.EntityName;
                        systemuser.LinkFromAttributeName = "businessunitid";
                        systemuser.LinkToEntityName = _DL_systemuser.EntityName;
                        systemuser.LinkToAttributeName = "businessunitid";
                        systemuser.JoinOperator = JoinOperator.Inner;
                        systemuser.Columns.AddColumns("systemuserid");
                        systemuser.EntityAlias = "systemuser1";
                        systemuser.LinkCriteria.AddCondition("title", ConditionOperator.Equal, "SDH");

                        queryExpression.LinkEntities.Add(systemuser);
                        queryExpression.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, entity.GetAttributeValue<EntityReference>("owninguser").Id);

                        EntityCollection entityCollection = organizationService.RetrieveMultiple(queryExpression);
                        if (entityCollection.Entities.Count > 0)
                        {
                            return new Guid(((AliasedValue)entityCollection.Entities[0].Attributes["systemuser1.systemuserid"]).Value.ToString());
                        }
                        else
                        {
                            throw new Exception("Can not found SDH for this WO.");
                        }
                    }
                    if (entity.Attributes.Contains("owningteam"))
                    {
                        QueryExpression queryExpression = new QueryExpression("teammembership");
                        queryExpression.ColumnSet = new ColumnSet("teamid");

                        LinkEntity systemuser = new LinkEntity();
                        systemuser.LinkFromEntityName = "teammembership";
                        systemuser.LinkFromAttributeName = "systemuserid";
                        systemuser.LinkToEntityName = _DL_systemuser.EntityName;
                        systemuser.LinkToAttributeName = "systemuserid";
                        systemuser.JoinOperator = JoinOperator.Inner;
                        systemuser.Columns.AddColumns("systemuserid");
                        systemuser.EntityAlias = "systemuser";

                        LinkEntity systemuserroles = new LinkEntity();
                        systemuserroles.LinkFromEntityName = _DL_systemuser.EntityName;
                        systemuserroles.LinkFromAttributeName = "systemuserid";
                        systemuserroles.LinkToEntityName = "systemuserroles";
                        systemuserroles.LinkToAttributeName = "systemuserid";
                        systemuserroles.JoinOperator = JoinOperator.Inner;
                        systemuserroles.Columns.AddColumns("roleid");
                        systemuserroles.EntityAlias = "systemuserroles";

                        LinkEntity role = new LinkEntity();
                        role.LinkFromEntityName = "systemuserroles";
                        role.LinkFromAttributeName = "roleid";
                        role.LinkToEntityName = "role";
                        role.LinkToAttributeName = "roleid";
                        role.JoinOperator = JoinOperator.Inner;
                        role.Columns.AddColumns("name");
                        role.EntityAlias = "role";
                        role.LinkCriteria.AddCondition("name", ConditionOperator.Equal, Configuration.SDHRoleName);

                        systemuserroles.LinkEntities.Add(role);
                        systemuser.LinkEntities.Add(systemuserroles);
                        queryExpression.LinkEntities.Add(systemuser);
                        queryExpression.Criteria.AddCondition("teamid", ConditionOperator.Equal, entity.GetAttributeValue<EntityReference>("owningteam").Id);

                        EntityCollection entityCollection = organizationService.RetrieveMultiple(queryExpression);
                        if (entityCollection.Entities.Count > 0)
                        {
                            return new Guid(((AliasedValue)entityCollection.Entities[0].Attributes["systemuser.systemuserid"]).Value.ToString());
                        }
                        else
                        {
                            throw new Exception("Can not found SDH for this WO.");
                        }
                    }
                    else
                    {
                        throw new Exception("Can not found team for this WO.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".GetWOSDH : " + ex.Message.ToString());
            }
        }

        private void SendEmail(IOrganizationService organizationService, Guid sender, Guid receiver, string subject, string description)
        {
            try
            {
                Guid emailId = Guid.Empty;

                EmailAgent emailAgent = new EmailAgent();
                emailAgent.AddSender(sender);
                emailAgent.AddReceiver(_DL_systemuser.EntityName, receiver);
                emailAgent.subject = subject;
                emailAgent.description = description;
                emailAgent.Create(organizationService, out emailId);

                if (emailId != Guid.Empty)
                    emailAgent.Send(organizationService, emailId);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendEmail : " + ex.Message.ToString());
            }
        }

        private void ChangeServiceAppointmentStatustoRelease(IOrganizationService organizationService, Guid id
            , DateTime syncTime)
        {
            _DL_serviceappointment = new DL_serviceappointment();
            _DL_serviceappointment.Released(organizationService, id);
            _DL_serviceappointment.trs_synctime = syncTime;
            _DL_serviceappointment.Update(organizationService, id);
        }
        #endregion

        #region Publics
        public void RejectDispathedServiceAppointment(IOrganizationService organizationService, Guid userId, Guid id
            , string woNumber, DateTime syncTime)
        {
            try
            {
                Guid sdhId = GetWOSDH(organizationService, id);
                ChangeServiceAppointmentStatustoRelease(organizationService, id, syncTime);
                SendEmail(organizationService, userId, sdhId
                    , "WO No. " + woNumber + " : " + "Not Confirmed"
                    , "Reason : Idle for " + Configuration.IdleMinutesinWords);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".RejectDispathedServiceAppointment : " + ex.Message.ToString());
            }
        }

        public void CancelDispathedServiceAppointment(IOrganizationService organizationService, Guid userId, Guid id
            , string woNumber, string reason, DateTime syncTime)
        {
            try
            {
                Guid sdhId = GetWOSDH(organizationService, id);
                ChangeServiceAppointmentStatustoRelease(organizationService, id, syncTime);
                SendEmail(organizationService, userId, sdhId
                    , "WO No. " + woNumber + " : " + "Canceled"
                    , "Reason : " + reason);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".CancelDispathedServiceAppointment : " + ex.Message.ToString());
            }
        }

        public void UpdateStatus_Hold(IOrganizationService organizationService, Guid id
            , Guid mechanicId, Guid mobileGuid, string remarks
            , decimal longitude, decimal latitude
            , DateTime automaticTime, DateTime? manualTime
            , DateTime modifiedOn, DateTime syncTime)
        {
            try
            {
                _BL_trs_mtar.RecordMTAR(organizationService
                    , id
                    , mechanicId
                    , mobileGuid
                    , Configuration.MTAR_Hold
                    , remarks
                    , longitude
                    , latitude
                    , automaticTime
                    , manualTime
                    , modifiedOn
                    , syncTime
                    , true);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".UpdateStatus_Hold : " + ex.Message);
            }
        }

        public void UpdateStatus_Unhold(IOrganizationService organizationService, Guid id
            , Guid mechanicId, Guid mobileGuid, string remarks
            , decimal longitude, decimal latitude
            , DateTime automaticTime, DateTime? manualTime
           , DateTime modifiedOn, DateTime syncTime)
        {
            try
            {
                _BL_trs_mtar.RecordMTAR(organizationService
                    , id
                    , mechanicId
                    , mobileGuid
                    , Configuration.MTAR_Resume
                    , remarks
                    , longitude
                    , latitude
                    , automaticTime
                    , manualTime
                    , modifiedOn
                    , syncTime
                    , true);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".UpdateStatus_Unhold : " + ex.Message);
            }
        }

        public void UpdateStatus_SubmitTECO(IOrganizationService organizationService, Guid id, Guid mechanicId, Guid mobileGuid
            , string inspectorComments
            , Guid? inspectorSuggestion
            , string customerComments
            , string customerSatisfaction
            , DateTime documentDate
            , decimal? hourMeter
            , int? equipmentStatus
            , string remarks
            , decimal longitude
            , decimal latitude
            , DateTime automaticTime
            , DateTime? manualTime
            , DateTime modifiedOn, DateTime syncTime)
        {
            try
            {
                if (id == Guid.Empty)
                    throw new Exception("Primary Key can not empty !");

                if (_woGenerator.GetMechanicLeader(organizationService, id) == mechanicId)
                {
                    _DL_serviceappointment = new DL_serviceappointment();
                    _DL_serviceappointment.trs_inspectorcomments = inspectorComments;
                    if (inspectorSuggestion != null)
                        _DL_serviceappointment.trs_inspectorsuggestion = (Guid)inspectorSuggestion;
                    _DL_serviceappointment.trs_customercomments = customerComments;
                    _DL_serviceappointment.trs_customersatisfaction = customerSatisfaction;
                    _DL_serviceappointment.trs_documentdate = documentDate;
                    //_DL_serviceappointment.trs_documentlink = documentLink;
                    if (hourMeter != null)
                        _DL_serviceappointment.trs_lasthourmeter = (decimal)hourMeter;
                    if (equipmentStatus != null)
                        _DL_serviceappointment.trs_statusinoperation = (int)equipmentStatus;
                    _DL_serviceappointment.trs_frommobile = true;
                    _DL_serviceappointment.trs_synctime = syncTime;
                    _DL_serviceappointment.Update(organizationService, id);
                }

                _BL_trs_mtar.RecordMTAR(organizationService
                    , id
                    , mechanicId
                    , mobileGuid
                    , Configuration.MTAR_SubmitTeco
                    , remarks
                    , longitude
                    , latitude
                    , automaticTime
                    , manualTime
                    , modifiedOn
                    , syncTime
                    , true);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".UpdateStatus_SubmitTECO : " + ex.Message);
            }
        }

        public void RequestParts(IOrganizationService organizationService, Guid userId, Guid id, string description)
        {
            try
            {
                Guid sdhId = GetWOSDH(organizationService, id);
                SendEmail(organizationService, userId, sdhId
                    , "Request Part from Mechanic"
                    , description);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".RequestParts : " + ex.Message.ToString());
            }
        }

        public bool IsAlreadySubmitTECObyMechanic(IOrganizationService organizationService, Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    throw new Exception("Primary Key can not empty !");
                Entity entity = _DL_serviceappointment.Select(organizationService, id);
                if (entity.GetAttributeValue<OptionSetValue>("statecode").Value == 3)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".IsAlreadySubmitTECObyMechanic : " + ex.Message);
            }
        }
        #endregion
    }
}