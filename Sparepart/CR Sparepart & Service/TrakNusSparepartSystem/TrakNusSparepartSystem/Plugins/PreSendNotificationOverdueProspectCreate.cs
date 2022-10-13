namespace TrakNusSparepartSystem.Plugins
{
    using System;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using TrakNusSparepartSystem.Plugins.BusinessLayer;

    /// <summary>
    /// PreSendNotificationOverdueProspectCreate Plugin.
    /// </summary>    
    public class PreSendNotificationOverdueProspectCreate: Plugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreSendNotificationOverdueProspectCreate"/> class.
        /// </summary>
        public PreSendNotificationOverdueProspectCreate()
            : base(typeof(PreSendNotificationOverdueProspectCreate))
        {
            base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(20, "Create", "tss_sendnotifoverdueprospect", new Action<LocalPluginContext>(ExecutePreSendNotificationOverdueProspectCreate)));

            // Note : you can register for more events here if this plugin is not specific to an individual entity and message combination.
            // You may also need to update your RegisterFile.crmregister plug-in registration file to reflect any change.
        }

        /// </remarks>
        protected void ExecutePreSendNotificationOverdueProspectCreate(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new ArgumentNullException("localContext");
            }

            // TODO: Implement your custom Plug-in business logic.
            IPluginExecutionContext context = localContext.PluginExecutionContext;
            ITracingService tracer = localContext.TracingService;
            Entity entity = (Entity)context.InputParameters["Target"];

            BL_tss_sendnotifoverdueprospect _BL_tss_sendnotifoverdueprospect = new BL_tss_sendnotifoverdueprospect();
            _BL_tss_sendnotifoverdueprospect.Form_OnCreate_PreOperation(localContext.OrganizationService, localContext.PluginExecutionContext, entity, tracer);
        }
    }
}
