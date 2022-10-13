using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrakNusSparepartSystemScheduler.Helper;

namespace TrakNusSparepartSystemScheduler.Business_Layer
{
    public class BL_RecordHelper
    {
        public void PerformBulkDelete()
        {
            string _connectionstring = string.Empty;

            if (ConnString.IsValidConnectionString(ConfigurationManager.ConnectionStrings["CRMCS"].ConnectionString))
                _connectionstring = ConfigurationManager.ConnectionStrings["CRMCS"].ConnectionString;
            else
                _connectionstring = "Url=http://10.0.10.43/traktornusantara; Domain=traknus; Username=admin.crm; Password=pass@word2;";

            //string CRM_URL = _connectionstring.Split(';')[0].Remove(0, 4);

            Console.WriteLine("Bulk Delete From Console");
            Console.WriteLine("==============================");
            Console.WriteLine("Getting CrmServiceClient..");

            CrmServiceClient conn = new CrmServiceClient(_connectionstring);

            try
            {
                IOrganizationService _organizationservice = conn.OrganizationServiceProxy;

                BulkDeleteRequest _bulkdeleterequest = new BulkDeleteRequest
                {
                    JobName = "Bulk Delete From Console " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                    ToRecipients = new Guid[] { },
                    CCRecipients = new Guid[] { },
                    RecurrencePattern = string.Empty,
                    QuerySet = new QueryExpression[]
                        {
                            new QueryExpression { EntityName = "tss_mastermarketsize" },
                            new QueryExpression { EntityName = "tss_mastermarketsizesublines" }
                        }
                };

                BulkDeleteResponse _bulkdeleteresponse = (BulkDeleteResponse)_organizationservice.Execute(_bulkdeleterequest);
                Guid _jobid = _bulkdeleteresponse.JobId;

                bool _deleting = true;
                Console.WriteLine("Deleting..");

                while (_deleting)
                {
                    Thread.Sleep(20000);

                    QueryExpression _queryexpression = new QueryExpression { EntityName = "bulkdeleteoperation" };
                    _queryexpression.Criteria.AddCondition("asyncoperationid", ConditionOperator.Equal, _jobid);
                    _queryexpression.Criteria.AddCondition("statecode", ConditionOperator.Equal, 3);
                    _queryexpression.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 30);

                    EntityCollection _resultcollection = _organizationservice.RetrieveMultiple(_queryexpression);

                    if (_resultcollection.Entities.Count > 0)
                    {
                        Console.WriteLine("Finished Deleting !");
                        _deleting = false;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("FAILED ! : " + e.Message);
                throw;
            }
        }
    }
}
