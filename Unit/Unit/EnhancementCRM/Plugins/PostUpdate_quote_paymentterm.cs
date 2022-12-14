// <copyright file="PostUpdate_quote_paymentterm.cs" company="">
// Copyright (c) 2019 All Rights Reserved
// </copyright>
// <author></author>
// <date>13-Aug-19 6:12:49 PM</date>
// <summary>Implements the PostUpdate_quote_paymentterm Plugin.</summary>
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
    /// PostUpdate_quote_paymentterm Plugin.
    /// Fires when the following attributes are updated:
    /// new_paymentterm
    /// </summary>    
    public class PostUpdate_quote_paymentterm : PluginBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostUpdate_quote_paymentterm"/> class.
        /// </summary>
        /// <param name="unsecure">Contains public (unsecured) configuration information.</param>
        /// <param name="secure">Contains non-public (secured) configuration information. 
        /// When using Microsoft Dynamics 365 for Outlook with Offline Access, 
        /// the secure string is not passed to a plug-in that executes while the client is offline.</param>
        public PostUpdate_quote_paymentterm(string unsecure, string secure)
            : base(typeof(PostUpdate_quote_paymentterm))
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
            BL_quote _BL_quote = new BL_quote();
            _BL_quote.PostUpdate_quote_paymentterm(localContext.OrganizationService, localContext.PluginExecutionContext, entity, tracer);

        }
    }
}
