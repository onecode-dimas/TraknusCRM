// <copyright file="PostServiceRequisitionCreate.cs" company="">
// Copyright (c) 2014 All Rights Reserved
// </copyright>
// <author></author>
// <date>10/14/2014 3:44:35 PM</date>
// <summary>Implements the PostServiceRequisitionCreate Plugin.</summary>
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
    /// PostServiceRequisitionCreate Plugin.
    /// </summary>    
    public class PostServiceRequisitionCreate: Plugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostServiceRequisitionCreate"/> class.
        /// </summary>
        public PostServiceRequisitionCreate()
            : base(typeof(PostServiceRequisitionCreate))
        {
            base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, "Create", "incident", new Action<LocalPluginContext>(ExecutePostServiceRequisitionCreate)));

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
        protected void ExecutePostServiceRequisitionCreate(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new ArgumentNullException("localContext");
            }

            // TODO: Implement your custom Plug-in business logic.

            BL_incident _BL_incident = new BL_incident();
            _BL_incident.Form_OnCreate(localContext.OrganizationService, localContext.PluginExecutionContext);
        }
    }
}
