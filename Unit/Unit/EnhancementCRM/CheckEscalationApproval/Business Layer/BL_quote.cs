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
    public class BL_quote: BL_manager
    {
        private string _entityname_approvallistquoteminprice = "ittn_approvallistquoteminprice";
        private string _entityname_matrixapprovalquoteminprice = "ittn_matrixapprovalquoteminprice";
        private string _entityname_opportunityproduct = "opportunityproduct";
        private string _entityname_quote = "quote";
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

                    FilterExpression _filterexpression = new FilterExpression(LogicalOperator.Or);
                    _filterexpression.AddCondition("ittn_statusreason", ConditionOperator.Equal, 841150001);
                    _filterexpression.AddCondition("ittn_statusreason", ConditionOperator.Equal, 841150006);

                    QueryExpression _queryexpression = new QueryExpression(_entityname_quote);
                    _queryexpression.ColumnSet = new ColumnSet(true);
                    _queryexpression.Criteria.AddFilter(_filterexpression);
                    EntityCollection _quotes = _organizationservice.RetrieveMultiple(_queryexpression);

                    foreach (var _quote in _quotes.Entities)
                    {
                        Guid _quoteid = _quote.Id;
                        Guid _currentapproverid = _quote.GetAttributeValue<EntityReference>("ittn_minpricecurrentapprover").Id;
                        Guid _unitgroupid = new Guid();
                        DateTime _reqapproveminpricedate = _quote.GetAttributeValue<DateTime>("ittn_reqapproveminpricedate");
                        DateTime _escalateminpricedate = _quote.GetAttributeValue<DateTime>("ittn_escalateminpricedate");
                        DateTime _requestorescalationdate = _escalateminpricedate != null && _escalateminpricedate != DateTime.MinValue ? _escalateminpricedate : _reqapproveminpricedate;

                        // GET QUOTE
                        _unitgroupid = _quote.GetAttributeValue<EntityReference>("new_unitgroup").Id;

                        Entity _matrixapprovalquoteminprice = _organizationservice.Retrieve(_entityname_matrixapprovalquoteminprice, _currentapproverid, new ColumnSet(true));

                        if (_matrixapprovalquoteminprice != null)
                        {
                            Entity _currentmatrixapprovalquoteminprice = _matrixapprovalquoteminprice;
                            int _priorityno = _currentmatrixapprovalquoteminprice.GetAttributeValue<int>("ittn_priorityno");
                            int _escalationhour = _currentmatrixapprovalquoteminprice.GetAttributeValue<int>("ittn_escalationhour");
                            int _hourdifference = Convert.ToInt32((DateTime.Now - _requestorescalationdate).TotalHours);

                            if (_hourdifference > _escalationhour)
                            {
                                Guid _opportunityid = _quote.GetAttributeValue<EntityReference>("opportunityid").Id;

                                _queryexpression = new QueryExpression(_entityname_opportunityproduct);
                                _queryexpression.ColumnSet = new ColumnSet(true);
                                _queryexpression.Criteria.AddCondition("opportunityid", ConditionOperator.Equal, _opportunityid);
                                EntityCollection _opportunityproducts = _organizationservice.RetrieveMultiple(_queryexpression);

                                decimal _totalbaseamount = _opportunityproducts.Entities.AsEnumerable().Where(x => x.GetAttributeValue<Money>("baseamount").Value > 0).Sum(x => x.GetAttributeValue<Money>("baseamount").Value);
                                decimal _totalextendedamount = _opportunityproducts.Entities.AsEnumerable().Where(x => x.GetAttributeValue<Money>("ittn_totalextendedamount").Value > 0).Sum(x => x.GetAttributeValue<Money>("ittn_totalextendedamount").Value);
                                decimal _totalamountpercentage = _totalextendedamount / _totalbaseamount * 100;

                                // GET MATRIX APPROVAL ( NEXT ESCALATION )
                                _queryexpression = new QueryExpression(_entityname_matrixapprovalquoteminprice);
                                _queryexpression.ColumnSet = new ColumnSet(true);
                                _queryexpression.Orders.Add(new OrderExpression("ittn_priorityno", OrderType.Ascending));
                                _queryexpression.Criteria.AddCondition("ittn_startpercentage", ConditionOperator.LessEqual, _totalamountpercentage);
                                _queryexpression.Criteria.AddCondition("ittn_endpercentage", ConditionOperator.GreaterThan, _totalamountpercentage);
                                _queryexpression.Criteria.AddCondition("ittn_unitgroup", ConditionOperator.Equal, _unitgroupid);
                                _queryexpression.Criteria.AddCondition("ittn_priorityno", ConditionOperator.Equal, _priorityno + 1);
                                _queryexpression.Criteria.AddCondition("ittn_needapproval", ConditionOperator.Equal, true);

                                EntityCollection _matrixapprovalquoteminprice_nextescalation = _organizationservice.RetrieveMultiple(_queryexpression);

                                if (_matrixapprovalquoteminprice_nextescalation.Entities.Count() > 0)
                                {
                                    Entity _nextapprovermatrixapprovalquoteminprice = _matrixapprovalquoteminprice_nextescalation.Entities.FirstOrDefault();

                                    #region Update
                                    Entity _quote_update = new Entity(_entityname_quote);
                                    _quote_update.Id = _quote.Id;

                                    _quote_update["ittn_minpricecurrentapprover"] = new EntityReference(_entityname_matrixapprovalquoteminprice, _nextapprovermatrixapprovalquoteminprice.Id);
                                    _quote_update["ittn_escalateminpricedate"] = DateTime.Now;

                                    _organizationservice.Update(_quote_update);
                                    #endregion ---

                                    #region Send Email
                                    string _emailsubject = _quote.GetAttributeValue<string>("name") + " is waiting for approval !";
                                    string _emailcontent = "Please review and approve.";

                                    EmailAgent _emailagent = new EmailAgent();
                                    _emailagent.SendNotification(Get_AdminCRM().Id, _nextapprovermatrixapprovalquoteminprice.Id, Guid.Empty, _organizationservice, _emailsubject, _emailcontent);
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
