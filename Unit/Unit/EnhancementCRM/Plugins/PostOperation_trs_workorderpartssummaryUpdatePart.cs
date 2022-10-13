// <copyright file="PostOperation_trs_workorderpartssummaryUpdatePart.cs" company="">
// Copyright (c) 2019 All Rights Reserved
// </copyright>
// <author></author>
// <date>6/25/2019 4:13:08 PM</date>
// <summary>Implements the PostOperation_trs_workorderpartssummaryUpdatePart Plugin.</summary>
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
    /// PostOperation_trs_workorderpartssummaryUpdatePart Plugin.
    /// Fires when the following attributes are updated:
    /// trs_manualquantity
    /// </summary>    
    public class PostOperation_trs_workorderpartssummaryUpdatePart: PluginBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostOperation_trs_workorderpartssummaryUpdatePart"/> class.
        /// </summary>
        /// <param name="unsecure">Contains public (unsecured) configuration information.</param>
        /// <param name="secure">Contains non-public (secured) configuration information. 
        /// When using Microsoft Dynamics 365 for Outlook with Offline Access, 
        /// the secure string is not passed to a plug-in that executes while the client is offline.</param>
        private readonly string preImageAlias = "PreImage";
        public PostOperation_trs_workorderpartssummaryUpdatePart(string unsecure, string secure)
            : base(typeof(PostOperation_trs_workorderpartssummaryUpdatePart))
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

            Entity preImageEntity = (context.PreEntityImages != null && context.PreEntityImages.Contains(this.preImageAlias)) ? context.PreEntityImages[this.preImageAlias] : null;
            BL_trs_workorderpartsummary _BL_trs_workorderpartsummary = new BL_trs_workorderpartsummary();
            _BL_trs_workorderpartsummary.Form_OnUpdate_ManualQuantity_PostOperation(localContext.OrganizationService, localContext.PluginExecutionContext);
        }
    }
}
