using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using TrakNusRapidService.DataLayer;

namespace TrakNusRapidService.Helper
{
    public class FAutoEmailConfiguration
    {
        #region Constants
        private const string _classname = "FAutoEmailConfiguration";
        #endregion

        #region Depedencies
        private DL_trs_workflowconfiguration _DL_trs_workflowconfiguration = new DL_trs_workflowconfiguration();
        private DL_systemuser _DL_systemuser = new DL_systemuser();

        private EmailAgent _emailAgent = new EmailAgent();
        #endregion

        #region Privates
        #endregion

        #region Publics
        public void Configure(IOrganizationService organizationService, Guid id, Guid userId, int category)
        {
            try
            {
                if (category > 0)
                {
                    _emailAgent = new EmailAgent();
                    switch (category)
                    {
                        case 1: //SSIS Mechaic Monthly Point
                            QueryExpression queryExpression = new QueryExpression(_DL_trs_workflowconfiguration.EntityName);
                            queryExpression.ColumnSet = new ColumnSet(true);
                            queryExpression.Criteria.AddCondition("trs_generalconfig", ConditionOperator.Equal, Configuration.ConfigurationName);
                            EntityCollection entityCollection = _DL_trs_workflowconfiguration.Select(organizationService, queryExpression);
                            if (entityCollection.Entities.Count > 0)
                            {
                                Entity entity = entityCollection.Entities[0];
                                if (entity.Attributes.Contains("trs_gadgetadministrator"))
                                {
                                    _emailAgent.AddReceiver(_DL_systemuser.EntityName, entity.GetAttributeValue<EntityReference>("trs_gadgetadministrator").Id);
                                    _emailAgent.AddSender(userId);
                                    _emailAgent.subject = "Error when Calculate Monthly Point.";
                                    _emailAgent.Update(organizationService, id);
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Configure : " + ex.Message);
            }
        }
        #endregion
    }
}
