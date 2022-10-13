// <copyright file="PostTechnicalServiceReportUpdate.cs" company="Microsoft">
// Copyright (c) 2014 All Rights Reserved
// </copyright>
// <author>Microsoft</author>
// <date>12/5/2014 2:20:29 PM</date>
// <summary>Implements the PostTechnicalServiceReportUpdate Plugin.</summary>
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
    /// PostTechnicalServiceReportUpdate Plugin.
    /// Fires when the following attributes are updated:
    /// All Attributes
    /// </summary>    
    public class PostTechnicalServiceReportUpdate: Plugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostTechnicalServiceReportUpdate"/> class.
        /// </summary>
        public PostTechnicalServiceReportUpdate()
            : base(typeof(PostTechnicalServiceReportUpdate))
        {
            base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, "Update", "trs_technicalservicereport", new Action<LocalPluginContext>(ExecutePostTechnicalServiceReportUpdate)));

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
        protected void ExecutePostTechnicalServiceReportUpdate(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new ArgumentNullException("localContext");
            }

            // TODO: Implement your custom Plug-in business logic.
            BL_trs_technicalservicereport _BL_trs_technicalservicereport = new BL_trs_technicalservicereport();
            _BL_trs_technicalservicereport.Form_OnUpdate(localContext.OrganizationService, localContext.PluginExecutionContext);
        }
    }
}
