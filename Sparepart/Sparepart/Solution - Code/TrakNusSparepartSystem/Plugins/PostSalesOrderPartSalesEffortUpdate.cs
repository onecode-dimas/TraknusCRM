// <copyright file="PostSalesOrderPartSalesEffortUpdate.cs" company="">
// Copyright (c) 2018 All Rights Reserved
// </copyright>
// <author></author>
// <date>1/29/2018 1:54:53 PM</date>
// <summary>Implements the PostSalesOrderPartSalesEffortUpdate Plugin.</summary>
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
    /// PostSalesOrderPartSalesEffortUpdate Plugin.
    /// Fires when the following attributes are updated:
    /// tss_approvalstatus
    /// </summary>    
    public class PostSalesOrderPartSalesEffortUpdate: Plugin
    {
        /// <summary>
        /// Alias of the image registered for the snapshot of the 
        /// primary entity's attributes after the core platform operation executes.
        /// The image contains the following attributes:
        /// tss_approvalstatus
        /// 
        /// Note: Only synchronous post-event and asynchronous registered plug-ins 
        /// have PostEntityImages populated.
        /// </summary>
        private readonly string postImageAlias = "PostImage";

        /// <summary>
        /// Initializes a new instance of the <see cref="PostSalesOrderPartSalesEffortUpdate"/> class.
        /// </summary>
        public PostSalesOrderPartSalesEffortUpdate()
            : base(typeof(PostSalesOrderPartSalesEffortUpdate))
        {
            base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, "Update", "tss_sopartsaleseffort", new Action<LocalPluginContext>(ExecutePostSalesOrderPartSalesEffortUpdate)));

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
        protected void ExecutePostSalesOrderPartSalesEffortUpdate(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new ArgumentNullException("localContext");
            }

            IPluginExecutionContext context = localContext.PluginExecutionContext;

            Entity postImageEntity = (context.PostEntityImages != null && context.PostEntityImages.Contains(this.postImageAlias)) ? context.PostEntityImages[this.postImageAlias] : null;
            BL_tss_salesorderpartsaleseffort _BL_tss_salesorderpartsaleseffort = new BL_tss_salesorderpartsaleseffort();
            _BL_tss_salesorderpartsaleseffort.Form_OnUpdate_PostOperation(localContext.OrganizationService, localContext.PluginExecutionContext);
            // TODO: Implement your custom Plug-in business logic.
        }
    }
}
