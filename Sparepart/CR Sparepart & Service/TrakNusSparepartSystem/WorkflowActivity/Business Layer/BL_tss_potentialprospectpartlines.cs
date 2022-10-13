using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using TrakNusSparepartSystem.DataLayer;
using TrakNusSparepartSystem.Helper;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
namespace TrakNusSparepartSystem.WorkflowActivity.Business_Layer
{
    public class BL_tss_potentialprospectpartlines
    {
        DL_tss_potentialprospectpart _DL_tss_potentialprospectpart = new DL_tss_potentialprospectpart();
        DL_tss_salestargetnational _DL_tss_salestargetnational = new DL_tss_salestargetnational();
        DL_tss_salestargetpss _DL_tss_salestargetpss = new DL_tss_salestargetpss();
        DL_tss_marketsizeresultmapping _DL_tss_marketsizeresultmapping = new DL_tss_marketsizeresultmapping();
        DL_tss_mastermarketsize _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        DL_tss_sparepartsetup _DL_tss_sparepartsetup = new DL_tss_sparepartsetup();
        DL_tss_keyaccount _DL_tss_keyaccount = new DL_tss_keyaccount();
        DL_tss_mastermarketsizelines _DL_tss_mastermarketsizelines = new DL_tss_mastermarketsizelines();
        DL_tss_mastermarketsizesublines _DL_tss_mastermarketsizesublines = new DL_tss_mastermarketsizesublines();
        public void GeneratePotentialProspectPartLines(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context,Entity enPotentialPart) {
            
        }
    }
}
