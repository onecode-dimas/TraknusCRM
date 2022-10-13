using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using TrakNusRapidService.DataLayer;

namespace TrakNusRapidService.Helper
{
    public class SRGenerator
    {
        #region Constants
        private const string _classname = "SRGenerator";
        #endregion

        #region Depedencies
        private DL_incident _DL_incident = new DL_incident();
        private DL_trs_tasklistgroup _DL_trs_tasklistgroup = new DL_trs_tasklistgroup();
        #endregion

        #region Properties
        #endregion

        #region Privates
        private Guid GetPMActTypeId(IOrganizationService organizationService, string keyword)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_trs_tasklistgroup.EntityName);
                queryExpression.Criteria.AddCondition("trs_pmacttype", ConditionOperator.Equal, keyword);
                EntityCollection entityCollection = _DL_trs_tasklistgroup.Select(organizationService, queryExpression);
                if (entityCollection.Entities.Count > 0)
                {
                    return entityCollection.Entities[0].Id;
                }
                else
                {
                    throw new InvalidPluginExecutionException("Can not found PMActType with code '" + keyword + "'.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".GetPMActTypeId : " + ex.Message);
            }
        }
        #endregion

        #region Publics
        public void AssignCase(IOrganizationService organizationService, Guid id, Guid assigneeId, bool team = false)
        {
            if (team)
                _DL_incident.AssigntoTeam(organizationService, id, assigneeId);
            else
                _DL_incident.AssigntoUser(organizationService, id, assigneeId);
        }

        public Guid GenerateCase(IOrganizationService organizationService
            , Guid PopulationId
            , Guid CustomerId
            , Guid PrimaryContactId
            , string Topic
            , string keyword_PMActType
            , Guid? ContractId = null
            , Guid? ContractDetailId = null
        )
        {
            try
            {
                _DL_incident = new DL_incident();
                _DL_incident.trs_unit = (Guid)PopulationId;
                _DL_incident.customerid = (Guid)CustomerId;
                _DL_incident.primarycontactid = (Guid)PrimaryContactId;
                _DL_incident.title = Topic.ToString();
                _DL_incident.trs_pmacttype = GetPMActTypeId(organizationService, keyword_PMActType);
                if (ContractId != null)
                    _DL_incident.contractid = (Guid)ContractId;
                if (ContractDetailId != null)
                    _DL_incident.contractdetailid = (Guid)ContractDetailId;

                _DL_incident.trs_automaticsr = true;
                return _DL_incident.Insert(organizationService);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".GenerateCase : " + ex.Message);
            }
        }
        #endregion
    }
}
