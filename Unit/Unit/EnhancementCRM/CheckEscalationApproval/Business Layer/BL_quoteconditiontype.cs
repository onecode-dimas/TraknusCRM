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
    public class BL_quoteconditiontype: BL_manager
    {
        private string _entityname_matrixapprovalconditiontype = "ittn_matrixapprovalconditiontype";
        private string _entityname_quote = "quote";
        private string _entityname_quoteconditiontype = "ittn_quoteconditiontype";
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
                    _filterexpression.AddCondition("ittn_needapproveconditiontype", ConditionOperator.Equal, true);
                    _filterexpression.AddCondition("ittn_approveconditiontypedate", ConditionOperator.Null);
                    _filterexpression.AddCondition("ittn_conditiontypecurrentapprover", ConditionOperator.NotNull);

                    QueryExpression _queryexpression = new QueryExpression(_entityname_quoteconditiontype);
                    _queryexpression.ColumnSet = new ColumnSet(true);
                    _queryexpression.Criteria.AddFilter(_filterexpression);
                    EntityCollection _quoteconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                    foreach (var _quoteconditiontype in _quoteconditiontypes.Entities)
                    {
                        Guid _quoteid = _quoteconditiontype.GetAttributeValue<EntityReference>("ittn_quote").Id;
                        Guid _conditiontypeid = _quoteconditiontype.GetAttributeValue<EntityReference>("ittn_conditiontype").Id;
                        Guid _currentapproverid = _quoteconditiontype.GetAttributeValue<EntityReference>("ittn_conditiontypecurrentapprover").Id;
                        Guid _unitgroupid = new Guid();
                        DateTime _reqapproveconditiontypedate = _quoteconditiontype.GetAttributeValue<DateTime>("ittn_reqapproveconditiontypedate");
                        DateTime _escalateconditiontypedate = _quoteconditiontype.GetAttributeValue<DateTime>("ittn_escalateconditiontypedate");
                        DateTime _requestorescalationdate = _escalateconditiontypedate != null && _escalateconditiontypedate != DateTime.MinValue ? _escalateconditiontypedate : _reqapproveconditiontypedate;

                        // GET QUOTE
                        Entity _quote = _organizationservice.Retrieve(_entityname_quote, _quoteid, new ColumnSet(true));
                        _unitgroupid = _quote.GetAttributeValue<EntityReference>("new_unitgroup").Id;

                        Entity _matrixapprovalconditiontype = _organizationservice.Retrieve(_entityname_matrixapprovalconditiontype, _currentapproverid, new ColumnSet(true));

                        if (_matrixapprovalconditiontype != null)
                        {
                            Entity _currentmatrixapprovalconditiontype = _matrixapprovalconditiontype;
                            int _priorityno = _currentmatrixapprovalconditiontype.GetAttributeValue<int>("ittn_priorityno");
                            int _escalationhour = _currentmatrixapprovalconditiontype.GetAttributeValue<int>("ittn_escalationhour");
                            int _hourdifference = Convert.ToInt32((DateTime.Now - _requestorescalationdate).TotalHours);

                            if (_hourdifference > _escalationhour)
                            {
                                // GET MATRIX APPROVAL ( NEXT ESCALATION )
                                _queryexpression = new QueryExpression(_entityname_matrixapprovalconditiontype);
                                _queryexpression.ColumnSet = new ColumnSet(true);
                                _queryexpression.Orders.Add(new OrderExpression("ittn_priorityno", OrderType.Ascending));
                                _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _conditiontypeid);
                                _queryexpression.Criteria.AddCondition("ittn_unitgroup", ConditionOperator.Equal, _unitgroupid);
                                _queryexpression.Criteria.AddCondition("ittn_priorityno", ConditionOperator.Equal, _priorityno + 1);

                                EntityCollection _matrixapprovalconditiontypes_nextescalation = _organizationservice.RetrieveMultiple(_queryexpression);

                                if (_matrixapprovalconditiontypes_nextescalation.Entities.Count() > 0)
                                {
                                    Entity _nextapprovermatrixapprovalconditiontype = _matrixapprovalconditiontypes_nextescalation.Entities.FirstOrDefault();

                                    #region Update
                                    Entity _quoteconditiontype_update = new Entity(_entityname_quoteconditiontype);
                                    _quoteconditiontype_update.Id = _quoteconditiontype.Id;

                                    _quoteconditiontype_update["ittn_conditiontypecurrentapprover"] = new EntityReference(_entityname_matrixapprovalconditiontype, _nextapprovermatrixapprovalconditiontype.Id);
                                    _quoteconditiontype_update["ittn_escalateconditiontypedate"] = DateTime.Now;

                                    _organizationservice.Update(_quoteconditiontype_update);
                                    #endregion ---

                                    #region Send Email
                                    string _emailsubject = _quote.GetAttributeValue<string>("name") + " is waiting for approval !";
                                    string _emailcontent = "Please review and approve.";

                                    EmailAgent _emailagent = new EmailAgent();
                                    _emailagent.SendNotification(Get_AdminCRM().Id, _nextapprovermatrixapprovalconditiontype.Id, Guid.Empty, _organizationservice, _emailsubject, _emailcontent);
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
