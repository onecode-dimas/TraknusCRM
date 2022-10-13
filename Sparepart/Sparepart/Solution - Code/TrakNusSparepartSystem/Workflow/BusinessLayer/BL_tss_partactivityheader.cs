using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrakNusSparepartSystem.DataLayer;

namespace TrakNusSparepartSystem.Workflow.BusinessLayer
{
    public class BL_tss_partactivityheader
    {
        #region Dependencies
        private DL_team _DL_team = new DL_team();
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        private DL_trs_discountapproval _DL_trs_discountapproval = new DL_trs_discountapproval();
        private DL_tss_partactivityheader _DL_DL_tss_partactivityheader = new DL_tss_partactivityheader();
        private DL_tss_potentialprospectpart _DL_tss_potentialprospectpart = new DL_tss_potentialprospectpart();
        #endregion

        private string _classname = "DL_tss_partactivityheader";

        public void AddNewAcitivities_OnClick(IOrganizationService organizationService, Guid id, string entityName, ITracingService trace) //, IPluginExecutionContext pluginExceptionContext
        {
            try
            {
                var getQuotation = organizationService.Retrieve(entityName, id, new ColumnSet(true));
                if (getQuotation.Attributes.Contains("trs_plansubject"))
                {
                    //PSS only allow to input max activity per day according Max Visit per day on Setup
                    _DL_DL_tss_partactivityheader.CheckMaxActivityPerDays(organizationService,entityName,id,trace);

                    //PSS only allow to input Visit Date backdated for previous month according Max Backdated on Setup
                    _DL_DL_tss_partactivityheader.CheckVisitDateBackDate(organizationService, entityName, id,trace);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".AddNewAcitivities_OnClick : " + ex.Message.ToString());
            }
        }

        public void MarkComplete_OnClick(IOrganizationService organizationService, Guid id, string entityName, ITracingService trace)
        {
            try
            {
                var getQuotation = organizationService.Retrieve(entityName, id, new ColumnSet(true));

                #region Potential Prospect Part

                QueryExpression QueryService = new QueryExpression("tss_partactivitylines");
                QueryService.ColumnSet = new ColumnSet(true);
                QueryService.Criteria.AddCondition("tss_partactivityheader", ConditionOperator.Equal, getQuotation.Id);

                EntityCollection ServiceItems = organizationService.RetrieveMultiple(QueryService);

                if (ServiceItems.Entities.Count > 0)
                {
                    foreach (Entity sItem in ServiceItems.Entities)
                    {
                        _DL_tss_potentialprospectpart.MarkComplete(organizationService, sItem.GetAttributeValue<Guid>("trs_activities"));
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".MarkComplete_OnClick : " + ex.Message.ToString());
            }
        }
    }
}
