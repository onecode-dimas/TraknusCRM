// <copyright file="PostUpdate_CPO_by_CPOIDSAP.cs" company="">
// Copyright (c) 2019 All Rights Reserved
// </copyright>
// <author></author>
// <date>7/29/2019 7:47:11 PM</date>
// <summary>Implements the PostUpdate_CPO_by_CPOIDSAP Plugin.</summary>
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
// </auto-generated>

using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using EnhancementCRM.Plugins.Business_Layer;

namespace EnhancementCRM.Plugins
{

    /// <summary>
    /// PostUpdate_CPO_by_CPOIDSAP Plugin.
    /// Fires when the following attributes are updated:
    /// new_cpoidsap
    /// </summary>    
    public class PostUpdate_CPO_by_CPOIDSAP : PluginBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostUpdate_CPO_by_CPOIDSAP"/> class.
        /// </summary>
        /// <param name="unsecure">Contains public (unsecured) configuration information.</param>
        /// <param name="secure">Contains non-public (secured) configuration information. 
        /// When using Microsoft Dynamics 365 for Outlook with Offline Access, 
        /// the secure string is not passed to a plug-in that executes while the client is offline.</param>
        public PostUpdate_CPO_by_CPOIDSAP(string unsecure, string secure)
            : base(typeof(PostUpdate_CPO_by_CPOIDSAP))
        {

            // TODO: Implement your custom configuration handling.
        }


        /// <summary>
        /// Main entry point for he business logic that the plug-in is to execute.
        /// </summary>
        /// <param name="localContext">The <see cref="LocalPluginContext"/> which contains the
        /// <see cref="IPluginExecutionContext"/>,
        /// <see cref="IOrganizationService"/>
        /// and <see cref="ITracingService"/>
        /// </param>
        /// <remarks>
        /// For improved performance, Microsoft Dynamics 365 caches plug-in instances.
        /// The plug-in's Execute method should be written to be stateless as the constructor
        /// is not called for every invocation of the plug-in. Also, multiple system threads
        /// could execute the plug-in at the same time. All per invocation state information
        /// is stored in the context. This means that you should not use global variables in plug-ins.
        /// </remarks>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new InvalidPluginExecutionException("localContext");
            }

            // TODO: Implement your custom Plug-in business logic.
            IPluginExecutionContext context = localContext.PluginExecutionContext;
            ITracingService tracer = localContext.TracingService;
            Entity entity = (Entity)context.InputParameters["Target"];

            //prevent insert overdue yess
            BL_CPO _BL_CPO = new BL_CPO();
            //_BL_CPO.PostUpdate_CPO_CPOIDSAP(localContext.OrganizationService, localContext.PluginExecutionContext, entity, tracer);
        }
    }
}
