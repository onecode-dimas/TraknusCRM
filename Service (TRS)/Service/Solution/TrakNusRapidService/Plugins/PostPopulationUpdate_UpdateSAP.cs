// <copyright file="PostPopulationUpdate_UpdateSAP.cs" company="">
// Copyright (c) 2014 All Rights Reserved
// </copyright>
// <author></author>
// <date>11/10/2014 4:57:05 PM</date>
// <summary>Implements the PostPopulationUpdate_UpdateSAP Plugin.</summary>
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
// </auto-generated>
namespace TrakNusRapidService.Plugins
{
    using System;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using TrakNusRapidService.Plugins.BusinessLayer;

    /// <summary>
    /// PostPopulationUpdate_UpdateSAP Plugin.
    /// Fires when the following attributes are updated:
    /// trs_functionallocation,trs_warrantyenddate,trs_warrantystartdate
    /// </summary>    
    public class PostPopulationUpdate_UpdateSAP: Plugin
    {
        /// <summary>
        /// Alias of the image registered for the snapshot of the 
        /// primary entity's attributes before the core platform operation executes.
        /// The image contains the following attributes:
        /// No Attributes
        /// </summary>
        private readonly string preImageAlias = "PreImage";

        /// <summary>
        /// Initializes a new instance of the <see cref="PostPopulationUpdate_UpdateSAP"/> class.
        /// </summary>
        public PostPopulationUpdate_UpdateSAP()
            : base(typeof(PostPopulationUpdate_UpdateSAP))
        {
            base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, "Update", "new_population", new Action<LocalPluginContext>(ExecutePostPopulationUpdate_UpdateSAP)));

            // Note : you can register for more events here if this plugin is not specific to an individual entity and message combination.
            // You may also need to update your RegisterFile.crmregister plug-in registration file to reflect any change.
        }

        /// <summary>
        /// Executes the plug-in.
        /// </summary>
        /// <param name="localContext">The <see cref="LocalPluginContext"/> which contains the
        /// <see cref="IPluginExecutionContext"/>,
        /// <see cref="IOrganizationService"/>
        /// and <see cref="ITracingService"/>
        /// </param>
        /// <remarks>
        /// For improved performance, Microsoft Dynamics CRM caches plug-in instances.
        /// The plug-in's Execute method should be written to be stateless as the constructor
        /// is not called for every invocation of the plug-in. Also, multiple system threads
        /// could execute the plug-in at the same time. All per invocation state information
        /// is stored in the context. This means that you should not use global variables in plug-ins.
        /// </remarks>
        protected void ExecutePostPopulationUpdate_UpdateSAP(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new ArgumentNullException("localContext");
            }

            IPluginExecutionContext context = localContext.PluginExecutionContext;

            Entity preImageEntity = (context.PreEntityImages != null && context.PreEntityImages.Contains(this.preImageAlias)) ? context.PreEntityImages[this.preImageAlias] : null;

            BL_new_population _BL_new_population = new BL_new_population();
            _BL_new_population.Form_OnUpdate_UpdateSAP(localContext.OrganizationService, localContext.PluginExecutionContext);
        }
    }
}
