// <copyright file="PreTSRPartsDamagedDetailCreate.cs" company="">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author></author>
// <date>1/13/2015 3:54:38 PM</date>
// <summary>Implements the PreTSRPartsDamagedDetailCreate Plugin.</summary>
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
// </auto-generated>
namespace TrakNusRapidService.Plugins
{
    using System;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using BusinessLayer;

    /// <summary>
    /// PreTSRPartsDamagedDetailCreate Plugin.
    /// </summary>    
    public class PreTSRPartsDamagedDetailCreate: Plugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreTSRPartsDamagedDetailCreate"/> class.
        /// </summary>
        public PreTSRPartsDamagedDetailCreate()
            : base(typeof(PreTSRPartsDamagedDetailCreate))
        {
            base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(20, "Create", "trs_tsrpartsdamageddetail", new Action<LocalPluginContext>(ExecutePreTSRPartsDamagedDetailCreate)));

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
        protected void ExecutePreTSRPartsDamagedDetailCreate(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new ArgumentNullException("localContext");
            }

            // TODO: Implement your custom Plug-in business logic.
            BL_trs_tsrpartsdamageddetail _BL_trs_tsrpartsdamageddetail = new BL_trs_tsrpartsdamageddetail();
            _BL_trs_tsrpartsdamageddetail.Form_OnCreate_PreOperation(localContext.OrganizationService, localContext.PluginExecutionContext);
        }
    }
}
