namespace TrakNusSparepartSystem.Workflow
{
    using System;
    using System.Activities;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;
    using TrakNusSparepartSystem.Workflow.BusinessLayer;

    public sealed class AssignToPdh : CodeActivity
    {
        /// <summary>
        /// Executes the workflow activity.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        protected override void Execute(CodeActivityContext executionContext)
        {
            // Create the tracing service
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();
            
            if (tracingService == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve tracing service.");
            }

            tracingService.Trace("Entered AssignToPdh.Execute(), Activity Instance Id: {0}, Workflow Instance Id: {1}",
                executionContext.ActivityInstanceId,
                executionContext.WorkflowInstanceId);

            // Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve workflow context.");
            }

            tracingService.Trace("AssignToPdh.Execute(), Correlation Id: {0}, Initiating User: {1}",
                context.CorrelationId,
                context.InitiatingUserId);

            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
           
            try
            {
                // TODO: Implement your custom Workflow business logic.
                

                if (context.PrimaryEntityName == "trs_quotation")
                {
                    BL_tss_quotation _BL_tss_quotation = new BL_tss_quotation();

                    //copy entity to quotation part 
                    _BL_tss_quotation.CreateQuotation_CloneEntity(service, context.PrimaryEntityId, context.PrimaryEntityName, tracingService);

                    //update Status Assign Quo to Assigned 
                    _BL_tss_quotation.AssignToPdh_OnClick(service, context.PrimaryEntityId, context.PrimaryEntityName, tracingService);

                    
                }
                else
                {
                    throw new Exception("This workflow primary entity must be Quotation Part Header. Current Primary Entity : " + context.PrimaryEntityName);
                }
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                tracingService.Trace("Exception: {0}", e.ToString());

                // Handle the exception.
                throw;
            }

            tracingService.Trace("Exiting AssignToPdh.Execute(), Correlation Id: {0}", context.CorrelationId);
        }
    }
}