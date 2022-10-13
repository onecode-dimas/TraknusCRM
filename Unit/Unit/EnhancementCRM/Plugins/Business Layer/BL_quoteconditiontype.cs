using EnhancementCRM.HelperUnit;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhancementCRM.Plugins.Business_Layer
{
    public class BL_quoteconditiontype
    {
        #region Properties
        private string _classname = "BL_quoteconditiontype";
        private string _entityname = "ittn_quoteconditiontype";

        private string _entityname_approvallistquoteconditiontype = "ittn_approvallistquoteconditiontype";
        private string _entityname_matrixapprovalconditiontype = "ittn_matrixapprovalconditiontype";
        private string _entityname_quote = "quote";
        private string _entityname_systemuser = "systemuser";
        #endregion

        public void PreUpdate_ittn_quoteconditiontype(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        {
            try
            {
                decimal _amount = _entity.GetAttributeValue<Money>("ittn_amount").Value;

                _entity = _organizationservice.Retrieve(_entityname, _entity.Id, new ColumnSet(true));

                if (_entity != null)
                {
                    bool _needapproveconditiontype = _entity.GetAttributeValue<bool>("ittn_needapproveconditiontype");
                    Guid _quoteid = _entity.GetAttributeValue<EntityReference>("ittn_quote").Id;
                    Guid _conditiontypeid = _entity.GetAttributeValue<EntityReference>("ittn_conditiontype").Id;
                    Guid _unitgroupid = new Guid();

                    if (_amount > 0 && _needapproveconditiontype)
                    {
                        // GET QUOTE
                        Entity _quote = _organizationservice.Retrieve(_entityname_quote, _quoteid, new ColumnSet(true));
                        _unitgroupid = _quote.GetAttributeValue<EntityReference>("new_unitgroup").Id;

                        QueryExpression _queryexpression = new QueryExpression(_entityname_approvallistquoteconditiontype);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria.AddCondition("ittn_quoteconditiontype", ConditionOperator.Equal, _entity.Id);
                        EntityCollection _approvallistquoteconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                        if (_approvallistquoteconditiontypes.Entities.Count() > 0)
                            return;

                        // GET MATRIX APPROVAL
                        _queryexpression = new QueryExpression(_entityname_matrixapprovalconditiontype);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Orders.Add(new OrderExpression("ittn_priorityno", OrderType.Ascending));
                        _queryexpression.Criteria.AddCondition("ittn_conditiontype", ConditionOperator.Equal, _conditiontypeid);
                        _queryexpression.Criteria.AddCondition("ittn_unitgroup", ConditionOperator.Equal, _unitgroupid);
                        EntityCollection _matrixapprovalconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                        if (_matrixapprovalconditiontypes.Entities.Count() > 0)
                        {
                            Entity _firstmatrixapprover = new Entity(_entityname_matrixapprovalconditiontype);
                            Guid _currentapproverid = new Guid();
                            int _index = 0;

                            foreach (var _matrixapprovalconditiontype in _matrixapprovalconditiontypes.Entities)
                            {
                                Guid _approverid = _matrixapprovalconditiontype.GetAttributeValue<EntityReference>("ittn_approver").Id;
                                Entity _target = _organizationservice.Retrieve(_entityname_systemuser, _approverid, new ColumnSet(true));

                                #region Create
                                Entity _approvallistquoteconditiontype = new Entity(_entityname_approvallistquoteconditiontype);
                                _approvallistquoteconditiontype["ittn_approver"] = new EntityReference(_entityname_systemuser, _approverid);
                                _approvallistquoteconditiontype["ittn_name"] = _target.GetAttributeValue<string>("fullname");
                                _approvallistquoteconditiontype["ittn_quoteconditiontype"] = new EntityReference(_entityname, _entity.Id);

                                _organizationservice.Create(_approvallistquoteconditiontype);
                                #endregion ---

                                #region Share Record
                                ShareRecords _sharerecords = new ShareRecords();
                                
                                _sharerecords.ShareRecord(_organizationservice, _quote, _target);
                                _sharerecords.ShareRecord(_organizationservice, _entity, _target);
                                #endregion ---

                                if (_index == 0)
                                {
                                    _firstmatrixapprover = _matrixapprovalconditiontype;
                                    _currentapproverid = _approverid;
                                }

                                _index += 1;
                            }

                            Entity _currentapprover = _organizationservice.Retrieve(_entityname_systemuser, _currentapproverid, new ColumnSet(true));

                            #region Update
                            Entity _quoteconditiontype_update = new Entity(_entityname);
                            _quoteconditiontype_update.Id = _entity.Id;

                            _quoteconditiontype_update["ittn_conditiontypecurrentapprover"] = new EntityReference(_entityname_matrixapprovalconditiontype, _firstmatrixapprover.Id);
                            _quoteconditiontype_update["ittn_reqapproveconditiontypedate"] = DateTime.Now;

                            _organizationservice.Update(_quoteconditiontype_update);
                            #endregion ---

                            #region Send Email
                            string _emailsubject = _entity.GetAttributeValue<string>("tss_name") + " is waiting for approval !";
                            string _emailcontent = "Please review and approve.";

                            EmailAgent _emailagent = new EmailAgent();
                            _emailagent.SendNotification(_context.UserId, _currentapprover.Id, Guid.Empty, _organizationservice, _emailsubject, _emailcontent);
                            #endregion ---
                        }
                        else
                        {
                            throw new InvalidPluginExecutionException("Matrix Approval List for this Condition Type and Unit Group is not found !");
                        }
                    }
                    else if (_amount == 0 && _needapproveconditiontype)
                    {
                        // DELETE APPROVAL
                        QueryExpression _queryexpression = new QueryExpression(_entityname_approvallistquoteconditiontype);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria.AddCondition("ittn_quoteconditiontype", ConditionOperator.Equal, _entity.Id);
                        EntityCollection _approvallistquoteconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                        foreach (var _approvallistquoteconditiontype in _approvallistquoteconditiontypes.Entities)
                        {
                            _organizationservice.Delete(_entityname_approvallistquoteconditiontype, _approvallistquoteconditiontype.Id);
                        }

                        #region Update
                        Entity _quoteconditiontype_update = new Entity(_entityname);
                        _quoteconditiontype_update.Id = _entity.Id;

                        _quoteconditiontype_update["ittn_conditiontypecurrentapprover"] = null;
                        _quoteconditiontype_update["ittn_reqapproveconditiontypedate"] = null;
                        _quoteconditiontype_update["ittn_escalateconditiontypedate"] = null;

                        _organizationservice.Update(_quoteconditiontype_update);
                        #endregion ---
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".PreUpdate_ittn_quoteconditiontype: " + ex.Message.ToString());
            }
        }

        public void PostUpdate_ittn_quoteconditiontype_approve(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        {
            bool _approveconditiontype = _entity.GetAttributeValue<bool>("ittn_approveconditiontype");

            _entity = _organizationservice.Retrieve(_entityname, _entity.Id, new ColumnSet(true));

            if (_approveconditiontype)
            {
                Entity _quoteconditiontype_update = new Entity(_entityname);
                _quoteconditiontype_update.Id = _entity.Id;

                _quoteconditiontype_update["ittn_approveconditiontypedate"] = DateTime.Now;
                _quoteconditiontype_update["ittn_approveconditiontypeby"] = new EntityReference(_entityname_systemuser, _context.UserId);

                _organizationservice.Update(_quoteconditiontype_update);
            }
            else
            {
                Entity _quoteconditiontype_update = new Entity(_entityname);
                _quoteconditiontype_update.Id = _entity.Id;

                _quoteconditiontype_update["ittn_approveconditiontypedate"] = null;
                _quoteconditiontype_update["ittn_approveconditiontypeby"] = null;

                _organizationservice.Update(_quoteconditiontype_update);
            }
        }
    }
}
