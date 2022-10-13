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
    public class BL_ittn_salesorder
    {
        #region Depedencies
        Generator _generator = new Generator();
        MWSLog _mwslog = new MWSLog();
        #endregion ---

        private string _entityname_approvallistcposaleseffort = "ittn_approvallistcposaleseffort";
        private string _entityname_cposaleseffort = "ittn_cposaleseffort";
        private string _entityname_matrixapprovalsaleseffort = "ittn_matrixapprovalsaleseffort";
        private string _entityname_salesorder = "salesorder";
        private string _entityname_systemuser = "systemuser";
        private string _entityname_unitgroup = "uomschedule";

        public void RequestApproveSalesEffort(IOrganizationService _organizationservice, IWorkflowContext _context, string[] _recordids)
        {
            Entity _salesorder = _organizationservice.Retrieve(_entityname_salesorder, new Guid(_recordids[0]), new ColumnSet(true));
            Entity _unitgroup = _organizationservice.Retrieve(_entityname_unitgroup, _salesorder.GetAttributeValue<EntityReference>("new_unitgroup").Id, new ColumnSet(true));
            
            QueryExpression _queryexpression = new QueryExpression(_entityname_cposaleseffort);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition("ittn_cpo", ConditionOperator.Equal, _salesorder.Id);
            _queryexpression.Criteria.AddCondition("ittn_reqapprovesaleseffortdate", ConditionOperator.Null);
            EntityCollection _cposalesefforts = _organizationservice.RetrieveMultiple(_queryexpression);

            _queryexpression = new QueryExpression(_entityname_matrixapprovalsaleseffort);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Orders.Add(new OrderExpression("ittn_priorityno", OrderType.Ascending));
            _queryexpression.Criteria.AddCondition("ittn_unitgroup", ConditionOperator.Equal, _unitgroup.Id);
            EntityCollection _matrixapprovalsalesefforts = _organizationservice.RetrieveMultiple(_queryexpression);

            if (_matrixapprovalsalesefforts.Entities.Count() == 0)
            {
                throw new InvalidWorkflowException("Approval List Sales Effort for Unit Group '" + _unitgroup.GetAttributeValue<string>("name") + "' is NOT found !");
            }

            foreach (var _cposaleseffort in _cposalesefforts.Entities)
            {
                int _index = 0;
                Entity _firstmatrix = new Entity(_entityname_matrixapprovalsaleseffort);
                Guid _firstapproverid = Guid.Empty;

                foreach (var _matrixapprovalsaleseffort in _matrixapprovalsalesefforts.Entities)
                {
                    Entity _systemuser = _organizationservice.Retrieve(_entityname_systemuser, _matrixapprovalsaleseffort.GetAttributeValue<EntityReference>("ittn_approver").Id, new ColumnSet(true));
                    Entity _new = new Entity(_entityname_approvallistcposaleseffort);

                    _new["ittn_approver"] = new EntityReference(_entityname_systemuser, _systemuser.Id);
                    _new["ittn_cposaleseffort"] = new EntityReference(_entityname_cposaleseffort, _cposaleseffort.Id);
                    _new["ittn_name"] = _systemuser.GetAttributeValue<string>("fullname");

                    _organizationservice.Create(_new);

                    #region Share Record
                    ShareRecords _sharerecords = new ShareRecords();

                    _sharerecords.ShareRecord(_organizationservice, _salesorder, _systemuser);
                    _sharerecords.ShareRecord(_organizationservice, _cposaleseffort, _systemuser);
                    #endregion ---

                    if (_index == 0)
                    {
                        _firstmatrix = _matrixapprovalsaleseffort;
                        _firstapproverid = _systemuser.Id;
                    }

                    _index += 1;
                }

                #region UPDATE
                Entity _update = new Entity(_entityname_cposaleseffort);
                _update.Id = _cposaleseffort.Id;

                _update["ittn_needapprovesaleseffort"] = true;
                _update["ittn_reqapprovesaleseffortdate"] = DateTime.Now;
                _update["ittn_saleseffortcurrentapprover"] = new EntityReference(_entityname_matrixapprovalsaleseffort, _firstmatrix.Id);
                _update["ittn_statusreason"] = new OptionSetValue(841150000); 

                _organizationservice.Update(_update);
                #endregion

                #region Send Email
                string _emailsubject = _cposaleseffort.GetAttributeValue<string>("ittn_name") + " is waiting for approval !";
                string _emailcontent = "Please review and approve.";

                EmailAgent _emailagent = new EmailAgent();
                _emailagent.SendNotification(_context.UserId, _firstapproverid, Guid.Empty, _organizationservice, _emailsubject, _emailcontent);
                #endregion ---
            }
        }
    }
}
