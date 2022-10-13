using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhancementCRM.Plugins.Business_Layer
{
    public class BL_serviceappointment
    {
        #region Properties
        private string _classname = "BL_serviceappointment";
        private string _entityname_serviceappointment = "serviceappointment";

        private string _entityname_cpoconditiontype = "ittn_cpoconditiontype";

        private string _entityname_salesorder = "salesorder";
        private string _entityname_salesorderdetail = "salesorderdetail";
        private string _entityname_systemuser = "systemuser";
        #endregion

        public void PreCreate_serviceappointment_checkconditiontype(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        {
            try
            {
                //if (_entity != null)
                //{
                //    string _serialnumber = _entity.GetAttributeValue<string>("new_serialnumber");

                //    QueryExpression _queryexpression = new QueryExpression(_entityname_salesorderdetail);
                //    _queryexpression.ColumnSet = new ColumnSet(true);
                //    _queryexpression.Criteria.AddCondition("new_serialnumber", ConditionOperator.Equal, _serialnumber);
                //    EntityCollection _salesorderdetails = _organizationservice.RetrieveMultiple(_queryexpression);

                //    if (_salesorderdetails.Entities.Count() > 0)
                //    {
                //        Entity _salesorderdetail = _salesorderdetails.Entities.FirstOrDefault();
                //        Guid _salesorderid = _salesorderdetail.GetAttributeValue<EntityReference>("salesorderid").Id;
                //        //Entity _salesorder = _organizationservice.Retrieve(_entityname_salesorder, _salesorderid, new ColumnSet(true));

                //        _queryexpression = new QueryExpression(_entityname_cpoconditiontype);
                //        _queryexpression.ColumnSet = new ColumnSet(true);
                //        _queryexpression.Criteria.AddCondition("ittn_cpo", ConditionOperator.Equal, _salesorderid);
                //        _queryexpression.Criteria.AddCondition("ittn_amount", ConditionOperator.Equal, 0);
                //        EntityCollection _cpoconditiontypes = _organizationservice.RetrieveMultiple(_queryexpression);

                //        if (_cpoconditiontypes.Entities.Count() > 0)
                //        {
                //            throw new InvalidPluginExecutionException("Condition Type for CPO (Sales Order) with amount 0 is found !");
                //        }
                //    }
                //    else
                //    {
                //        throw new InvalidPluginExecutionException("CPO (Sales Order) Detail with Serial Number '" + _serialnumber + "' !");
                //    }
                //}
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".PreCreate_serviceappointment_checkconditiontype: " + ex.Message.ToString());
            }
        }
    }
}
