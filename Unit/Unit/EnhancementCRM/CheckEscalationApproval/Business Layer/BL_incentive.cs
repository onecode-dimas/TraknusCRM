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
    public class BL_incentive : BL_manager
    {
        private string _entityname_matrixapprovalincentive = "ittn_matrixapprovalincentive";
        private string _entityname_incentive = "new_incentive";
        private string _entityname_systemuser = "systemuser";

        public void CheckEscalationApproval()
        {
            try
            {
                string _connectionString = string.Empty;

                if (ConnectionString.IsValidConnectionString(ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString))
                    _connectionString = ConfigurationManager.ConnectionStrings["TRSCRM"].ConnectionString;

                using (CrmServiceClient conn = new CrmServiceClient(_connectionString))
                {
                    IOrganizationService _organizationservice = (IOrganizationService)conn.OrganizationServiceProxy;

                    FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
                    _filterexpression.AddCondition("ittn_statusreason", ConditionOperator.Equal, 841150000);

                    QueryExpression _queryexpression = new QueryExpression(_entityname_incentive);
                    _queryexpression.ColumnSet = new ColumnSet(true);
                    _queryexpression.Criteria.AddFilter(_filterexpression);
                    EntityCollection _incentives = _organizationservice.RetrieveMultiple(_queryexpression);

                    foreach (var _incentive in _incentives.Entities)
                    {
                        Guid _incentiveid = _incentive.Id;
                        Guid _currentapproverid = _incentive.GetAttributeValue<EntityReference>("ittn_currentincentiveapprover").Id;
                        Guid _unitgroupid = new Guid();
                        DateTime _reqapproveminpricedate = _incentive.GetAttributeValue<DateTime>("ittn_reqapprovalincentivedate");
                        DateTime _escalateminpricedate = _incentive.GetAttributeValue<DateTime>("ittn_escalateincentivedate");
                        DateTime _requestorescalationdate = _escalateminpricedate != null && _escalateminpricedate != DateTime.MinValue ? _escalateminpricedate : _reqapproveminpricedate;

                        // GET QUOTE
                        _unitgroupid = _incentive.GetAttributeValue<EntityReference>("new_unitgroup").Id;

                        Entity _matrixapprovalincentive = _organizationservice.Retrieve(_entityname_matrixapprovalincentive, _currentapproverid, new ColumnSet(true));

                        if (_matrixapprovalincentive != null)
                        {
                            Entity _currentmatrixapprovalincentive = _matrixapprovalincentive;
                            int _priorityno = _currentmatrixapprovalincentive.GetAttributeValue<int>("ittn_priorityno");
                            int _escalationhour = _currentmatrixapprovalincentive.GetAttributeValue<int>("ittn_escalationhour");
                            int _hourdifference = Convert.ToInt32((DateTime.Now - _requestorescalationdate).TotalHours);

                            if (_hourdifference > _escalationhour)
                            {
                                // GET MATRIX APPROVAL ( NEXT ESCALATION )
                                _queryexpression = new QueryExpression(_entityname_matrixapprovalincentive);
                                _queryexpression.ColumnSet = new ColumnSet(true);
                                _queryexpression.Orders.Add(new OrderExpression("ittn_priorityno", OrderType.Ascending));
                                _queryexpression.Criteria.AddCondition("ittn_unitgroup", ConditionOperator.Equal, _unitgroupid);
                                _queryexpression.Criteria.AddCondition("ittn_priorityno", ConditionOperator.Equal, _priorityno + 1);

                                EntityCollection _matrixapprovalincentives_nextescalation = _organizationservice.RetrieveMultiple(_queryexpression);

                                if (_matrixapprovalincentives_nextescalation.Entities.Count() > 0)
                                {
                                    Entity _nextapprovermatrixapprovalincentive = _matrixapprovalincentives_nextescalation.Entities.FirstOrDefault();

                                    #region Update
                                    Entity _update = new Entity(_entityname_incentive);
                                    _update.Id = _incentiveid;

                                    _update["ittn_currentincentiveapprover"] = new EntityReference(_entityname_matrixapprovalincentive, _nextapprovermatrixapprovalincentive.Id);
                                    _update["ittn_escalateincentivedate"] = DateTime.Now;

                                    _organizationservice.Update(_update);
                                    #endregion ---

                                    #region Send Email
                                    string _emailsubject = _incentive.GetAttributeValue<string>("new_name") + " is waiting for approval !";
                                    string _emailcontent = "Please review and approve.";

                                    EmailAgent _emailagent = new EmailAgent();
                                    _emailagent.SendNotification(Get_AdminCRM().Id, _nextapprovermatrixapprovalincentive.Id, Guid.Empty, _organizationservice, _emailsubject, _emailcontent);
                                    #endregion ---
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
