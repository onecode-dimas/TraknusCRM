namespace TrakNusSparepartSystem.Plugins
{
    using System;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using TrakNusSparepartSystem.Plugins.BusinessLayer;

    /// <summary>
    /// PreRatingUpdate Plugin.
    /// Fires when the following attributes are updated:
    /// All Attributes
    /// </summary>    
    public class PreRatingUpdate: Plugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreRatingUpdate"/> class.
        /// </summary>
        public PreRatingUpdate()
            : base(typeof(PreRatingUpdate))
        {
            base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(20, "Update", "tss_rating", new Action<LocalPluginContext>(ExecutePreRatingUpdate)));

            // Note : you can register for more events here if this plugin is not specific to an individual entity and message combination.
            // You may also need to update your RegisterFile.crmregister plug-in registration file to reflect any change.
        }

        protected void ExecutePreRatingUpdate(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new ArgumentNullException("localContext");
            }

            // TODO: Implement your custom Plug-in business logic.

            IPluginExecutionContext context = localContext.PluginExecutionContext;
            ITracingService tracer = localContext.TracingService;
            Entity entity = (Entity)context.InputParameters["Target"];

            //prevent insert overdue yess
            BL_tss_rating _BL_tss_rating = new BL_tss_rating();
            _BL_tss_rating.Form_OnUpdate_PreOperation(localContext.OrganizationService, localContext.PluginExecutionContext, entity, tracer);

            //insert data periode must same with existing
            _BL_tss_rating.Form_OnUpdate_PreOperation_Period(localContext.OrganizationService, localContext.PluginExecutionContext, entity, tracer);
        }
    }
}
