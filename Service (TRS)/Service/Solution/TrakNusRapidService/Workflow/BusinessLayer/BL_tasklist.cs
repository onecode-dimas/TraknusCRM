using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using TrakNusRapidService.DataLayer;


namespace TrakNusRapidService.Workflow.BusinessLayer
{
   class BL_tasklist
   {
       #region Constants
       private const string _classname = "BL_tasklist";
       private const int _depth = 1;
       #endregion

       #region Depedencies
       private DL_trs_tasklist _DL_trs_tasklist = new DL_trs_tasklist();      
       #endregion

       #region Privates
       #endregion

       #region Events
       public EntityCollection getAllTaskList(IOrganizationService organizationService)
       {
           try
           {
               QueryExpression qe = new QueryExpression(_DL_trs_tasklist.EntityName);
               qe.ColumnSet = new ColumnSet(new string[] { "trs_productsection", "trs_name", "trs_tasklistid" }); 
               return _DL_trs_tasklist.Select(organizationService, qe);
           }
           catch (Exception ex)
           {
               throw new InvalidWorkflowException(ex.ToString());
           }
       }

       public EntityCollection getAllTaskListByProductSection(IOrganizationService organizationService, Guid Id)
       {
           try
           {
               QueryExpression qetl = new QueryExpression(_DL_trs_tasklist.EntityName);
               qetl.ColumnSet = new ColumnSet(new string[] { "trs_productsection", "trs_name", "trs_tasklistid" });

               ConditionExpression cetl = new ConditionExpression();
               cetl.AttributeName = "trs_productsection";
               cetl.Operator = ConditionOperator.Equal;
               cetl.Values.Add(Id.ToString());

               qetl.Criteria.AddCondition(cetl);

               return _DL_trs_tasklist.Select(organizationService, qetl);
           }
           catch (Exception ex)
           {
               throw new InvalidWorkflowException(ex.ToString());
           }
       }
       #endregion

   }
}
