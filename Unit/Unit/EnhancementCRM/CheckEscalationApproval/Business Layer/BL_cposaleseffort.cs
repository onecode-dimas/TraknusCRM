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
    public class BL_cposaleseffort : BL_manager
    {
        private string _entityname_matrixapprovalsaleseffort = "ittn_matrixapprovalsaleseffort";
        private string _entityname_cposaleseffort = "ittn_cposaleseffort";
        private string _entityname_salesorder = "salesorder";
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

                    QueryExpression _queryexpression = new QueryExpression(_entityname_cposaleseffort);
                    _queryexpression.ColumnSet = new ColumnSet(true);
                    _queryexpression.Criteria.AddFilter(_filterexpression);
                    EntityCollection _cposalesefforts = _organizationservice.RetrieveMultiple(_queryexpression);

                    foreach (var _cposaleseffort in _cposalesefforts.Entities)
                    {
                        Guid _cposaleseffortid = _cposaleseffort.Id;
                        Guid _currentapproverid = _cposaleseffort.GetAttributeValue<EntityReference>("ittn_saleseffortcurrentapprover").Id;
                        Guid _unitgroupid = new Guid();
                        DateTime _reqapprovesaleseffortdate = _cposaleseffort.GetAttributeValue<DateTime>("ittn_reqapprovesaleseffortdate");
                        DateTime _saleseffortescalationdate = _cposaleseffort.GetAttributeValue<DateTime>("ittn_saleseffortescalationdate");
                        DateTime _requestorescalationdate = _saleseffortescalationdate != null && _saleseffortescalationdate != DateTime.MinValue ? _saleseffortescalationdate : _reqapprovesaleseffortdate;

                        // GET CPO
                        Entity _cpo = _organizationservice.Retrieve(_entityname_salesorder, _cposaleseffort.GetAttributeValue<EntityReference>("ittn_cpo").Id, new ColumnSet(true));
                        _unitgroupid = _cpo.GetAttributeValue<EntityReference>("new_unitgroup").Id;

                        Entity _matrixapprovalsales = _organizationservice.Retrieve(_entityname_matrixapprovalsaleseffort, _currentapproverid, new ColumnSet(true));

                        if (_matrixapprovalsales != null)
                        {
                            Entity _currentmatrixapprovalsales = _matrixapprovalsales;
                            int _priorityno = _currentmatrixapprovalsales.GetAttributeValue<int>("ittn_priorityno");
                            int _escalationhour = _currentmatrixapprovalsales.GetAttributeValue<int>("ittn_escalationhour");
                            int _hourdifference = Convert.ToInt32((DateTime.Now - _requestorescalationdate).TotalHours);

                            if (_hourdifference > _escalationhour)
                            {
                                // GET MATRIX APPROVAL ( NEXT ESCALATION )
                                _queryexpression = new QueryExpression(_entityname_matrixapprovalsaleseffort);
                                _queryexpression.ColumnSet = new ColumnSet(true);
                                _queryexpression.Orders.Add(new OrderExpression("ittn_priorityno", OrderType.Ascending));
                                _queryexpression.Criteria.AddCondition("ittn_unitgroup", ConditionOperator.Equal, _unitgroupid);
                                _queryexpression.Criteria.AddCondition("ittn_priorityno", ConditionOperator.Equal, _priorityno + 1);

                                EntityCollection _matrixapprovalsales_nextescalation = _organizationservice.RetrieveMultiple(_queryexpression);

                                if (_matrixapprovalsales_nextescalation.Entities.Count() > 0)
                                {
                                    Entity _nextapprovermatrixapprovalsaleseffort = _matrixapprovalsales_nextescalation.Entities.FirstOrDefault();

                                    #region Update
                                    Entity _update = new Entity(_entityname_cposaleseffort);
                                    _update.Id = _cposaleseffortid;

                                    _update["ittn_saleseffortcurrentapprover"] = new EntityReference(_entityname_matrixapprovalsaleseffort, _nextapprovermatrixapprovalsaleseffort.Id);
                                    _update["ittn_saleseffortescalationdate"] = DateTime.Now;

                                    _organizationservice.Update(_update);
                                    #endregion ---

                                    #region Send Email
                                    string _emailsubject = _cposaleseffort.GetAttributeValue<string>("ittn_name") + " is waiting for approval !";
                                    string _emailcontent = "Please review and approve.";

                                    EmailAgent _emailagent = new EmailAgent();
                                    _emailagent.SendNotification(Get_AdminCRM().Id, _nextapprovermatrixapprovalsaleseffort.Id, Guid.Empty, _organizationservice, _emailsubject, _emailcontent);
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
