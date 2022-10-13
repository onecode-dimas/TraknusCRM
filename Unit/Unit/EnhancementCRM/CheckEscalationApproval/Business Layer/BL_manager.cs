using EnhancementCRM.HelperUnit;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckEscalationApproval.Business_Layer
{
    public class BL_manager
    {
        public Entity Get_AdminCRM()
        {
            try
            {
                string _connectionString = string.Empty;

                if (ConnectionString.IsValidConnectionString(ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString))
                    _connectionString = ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString;

                using (CrmServiceClient conn = new CrmServiceClient(_connectionString))
                {
                    IOrganizationService _organizationservice = (IOrganizationService)conn.OrganizationServiceProxy;

                    QueryExpression _queryexpression = new QueryExpression("systemuser");
                    _queryexpression.ColumnSet.AddColumns("systemuserid", "domainname");
                    _queryexpression.Criteria.AddCondition("domainname", ConditionOperator.Equal, "TRAKNUS\\admin.crm");

                    return _organizationservice.RetrieveMultiple(_queryexpression).Entities.SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
