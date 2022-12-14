// <copyright file="PreRatingCreate.cs" company="">
// Copyright (c) 2018 All Rights Reserved
// </copyright>
// <author></author>
// <date>2/1/2018 1:54:40 PM</date>
// <summary>Implements the PreRatingCreate Plugin.</summary>
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
// </auto-generated>
namespace TrakNusSparepartSystem.Plugins
{
    using System;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using TrakNusSparepartSystem.Plugins.BusinessLayer;

    /// <summary>
    /// PreRatingCreate Plugin.
    /// </summary>    
    public class PreRatingCreate: Plugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreRatingCreate"/> class.
        /// </summary>
        public PreRatingCreate()
            : base(typeof(PreRatingCreate))
        {
            base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(20, "Create", "tss_rating", new Action<LocalPluginContext>(ExecutePreRatingCreate)));

            // Note : you can register for more events here if this plugin is not specific to an individual entity and message combination.
            // You may also need to update your RegisterFile.crmregister plug-in registration file to reflect any change.
        }

        protected void ExecutePreRatingCreate(LocalPluginContext localContext)
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
            _BL_tss_rating.Form_OnCreate_PreOperation(localContext.OrganizationService, localContext.PluginExecutionContext, entity, tracer);

            //insert data periode must same with existing
            _BL_tss_rating.Form_OnCreate_PreOperation_Period(localContext.OrganizationService, localContext.PluginExecutionContext, entity,tracer);

        }
    }
}
