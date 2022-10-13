namespace TrakNusSparepartSystem.Plugins
{
    using System;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using TrakNusSparepartSystem.Plugins.BusinessLayer;

    /// <summary>
    /// PostQuotationPartIndicatorUpdate Plugin.
    /// Fires when the following attributes are updated:
    /// All Attributes
    /// </summary>    
    public class PostQuotationPartIndicatorUpdate: Plugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostQuotationPartIndicatorUpdate"/> class.
        /// </summary>
        public PostQuotationPartIndicatorUpdate()
            : base(typeof(PostQuotationPartIndicatorUpdate))
        {
            base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, "Update", "tss_quotationpartindicator", new Action<LocalPluginContext>(ExecutePostQuotationPartIndicatorUpdate)));

            // Note : you can register for more events here if this plugin is not specific to an individual entity and message combination.
            // You may also need to update your RegisterFile.crmregister plug-in registration file to reflect any change.
        }
        /// </remarks>
        protected void ExecutePostQuotationPartIndicatorUpdate(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new ArgumentNullException("localContext");
            }

            ITracingService tracer = localContext.TracingService;
            // TODO: Implement your custom Plug-in business logic.
            BL_tss_quotationpartindicator bl = new BL_tss_quotationpartindicator();
            bl.Form_OnUpdate_TotalConfidenceLevel_PostOperation(localContext.OrganizationService, localContext.PluginExecutionContext, tracer);
        }
    }
}
