using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Messages;
using System.Activities;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TrakNusSparepartSystem.Helper;
using System.ServiceModel;
using Microsoft.Xrm;
using System.Threading.Tasks;
using CrmEarlyBound;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Win32;

namespace TrakNusSparepartSystem.WorkflowActivity.Business_Layer
{
    public class BL_tss_mastermarketsize_sp
    {
        public const int BATCH_SIZE = 50;
        public const int LIST_BATCH_SIZE = 200;
        private string _dbsource = "DBSource";

        public void GenerateMasterMarketSizeFromKeyAccount_UsingSP_OnClick(IOrganizationService _organizationservice, ITracingService _tracingservice, IWorkflowContext _workflowcontext, string _recordids)
        {
            using (OrganizationServiceContext _context = new OrganizationServiceContext(_organizationservice))
            {
                List<SqlParameter> _sqlparameters = new List<SqlParameter>()
                {
                    new SqlParameter("@tss_keyaccountid", SqlDbType.NVarChar) { Value = _recordids },
                    new SqlParameter("@systemuserid", SqlDbType.NVarChar) { Value = _workflowcontext.UserId.ToString().Replace("{", "").Replace("}", "") },
                };

                DataTable _datatable = new GetStoredProcedure().Connect("sp_ms_GenerateMS_FromKeyAccount", _sqlparameters, false);
            }
        }

        public void GenerateMarketSizeSummaryByPartNumberFromKeyAccount_UsingSP_OnClick(IOrganizationService _organizationservice, ITracingService _tracingservice, IWorkflowContext _workflowcontext, string _recordids)
        {

        }

        public void GenerateMarketSizeSummaryByGroupUIOCommodityFromKeyAccount_UsingSP_OnClick(IOrganizationService _organizationservice, ITracingService _tracingservice, IWorkflowContext _workflowcontext, string _recordids)
        {

        }
    }
}