using EnhancementCRM.HelperUnit;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhancementCRM.WorkflowActivity.Business_Layer
{
    public class BL_ittn_incentive
    {
        #region Depedencies
        Generator _generator = new Generator();
        MWSLog _mwslog = new MWSLog();
        #endregion ---

        private string _entityname_businessunit = "businessunit";
        private string _entityname_incentive = "new_incentive";
        private string _entityname_product = "product";
        private string _entityname_quote = "quote";
        private string _entityname_quotedetail = "quotedetail";
        private string _entityname_quoteconditiontype = "ittn_quoteconditiontype";
        private string _entityname_transactioncurrency = "transactioncurrency";
        private string _entityname_transactioncurrency_code = "IDR";
        private string _entityname_systemuser = "systemuser";
        private string _entityname_workflowconfiguration = "trs_workflowconfiguration";
        private string _entityname_workflowconfiguration_name = "TRS";
        private string _entityname_workflowconfiguration_primaryfieldname = "trs_generalconfig";

        public void ApproveIncentive(IOrganizationService _organizationservice, IWorkflowContext _context, string[] _recordids)
        {
            Guid _incentiveid = new Guid(_recordids[0]);

            Entity _update = new Entity(_entityname_incentive);
            _update.Id = _incentiveid;

            _update["ittn_approveincentiveby"] = new EntityReference(_entityname_systemuser, _context.UserId);
            _update["ittn_approveincentivedate"] = DateTime.Now;
            _update["ittn_statusreason"] = new OptionSetValue(841150001);

            _organizationservice.Update(_update);
        }
    }
}
