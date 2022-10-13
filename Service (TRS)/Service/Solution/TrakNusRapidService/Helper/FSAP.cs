using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using TrakNusRapidService.DataLayer;

namespace TrakNusRapidService.Helper
{
    public class FSAP
    {
        #region Constants
        private const string _classname = "FSAP";
        #endregion

        #region Depedencies
        private DL_trs_workflowconfiguration _DL_trs_workflowconfiguration = new DL_trs_workflowconfiguration();
        #endregion

        #region Publics
        public bool SynchronizetoSAP(IOrganizationService organizationService)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_trs_workflowconfiguration.EntityName);
                queryExpression.ColumnSet = new ColumnSet(true);

                FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                filterExpression.AddCondition("trs_generalconfig", ConditionOperator.Equal, Configuration.ConfigurationName);
                EntityCollection entityCollection = _DL_trs_workflowconfiguration.Select(organizationService, queryExpression);
                if (entityCollection.Entities.Count > 0)
                {
                    Entity entity = entityCollection.Entities[0];
                    if (entity.Attributes.Contains("trs_synctosap"))
                    {
                        return entity.GetAttributeValue<bool>("trs_synctosap");
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    throw new Exception("Can not found configuration with name 'TRS'.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SynchronizetoSAP : " + ex.Message);
            }
        }

        public string GetSAPSharingPath(IOrganizationService organizationService)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_trs_workflowconfiguration.EntityName);
                queryExpression.ColumnSet = new ColumnSet(true);

                FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                filterExpression.AddCondition("trs_generalconfig", ConditionOperator.Equal, Configuration.ConfigurationName);

                EntityCollection entityCollection = _DL_trs_workflowconfiguration.Select(organizationService, queryExpression);
                if (entityCollection.Entities.Count > 0)
                {
                    Entity entity = entityCollection.Entities[0];
                    if (entity.Attributes.Contains("trs_folderpath"))
                    {
                        return entity.GetAttributeValue<string>("trs_folderpath");
                    }
                    else
                    {
                        throw new Exception("Please fill Sharing Path for SAP first !");
                    }
                }
                else
                {
                    throw new Exception("Can not found configuration with name 'TRS'.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".GetSAPSharingPath : " + ex.Message);
            }
        }
        #endregion
    }
}
