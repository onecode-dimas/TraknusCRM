// <copyright file="PreMarketSizePartConsumpCreate.cs" company="">
// Copyright (c) 2018 All Rights Reserved
// </copyright>
// <author></author>
// <date>04-Oct-18 6:13:41 PM</date>
// <summary>Implements the PreMarketSizePartConsumpCreate Plugin.</summary>
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
// </auto-generated>

using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using TrakNusSparepartSystem.plugin;
using TrakNusSparepartSystem.Plugins.BusinessLayer;

namespace TrakNusSparepartSystem.Plugins
{

    /// <summary>
    /// PreMarketSizePartConsumpCreate Plugin.
    /// </summary>    
    public class PreMarketSizePartConsumpCreate: PluginBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreMarketSizePartConsumpCreate"/> class.
        /// </summary>
        /// <param name="unsecure">Contains public (unsecured) configuration information.</param>
        /// <param name="secure">Contains non-public (secured) configuration information. 
        /// When using Microsoft Dynamics 365 for Outlook with Offline Access, 
        /// the secure string is not passed to a plug-in that executes while the client is offline.</param>
        public PreMarketSizePartConsumpCreate(string unsecure, string secure)
            : base(typeof(PreMarketSizePartConsumpCreate))
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
            BL_tss_marketsizeconsump _BL_tss_marketsizeconsump = new BL_tss_marketsizeconsump();
            _BL_tss_marketsizeconsump.Form_OnCreate_PreOperation(localContext.OrganizationService, localContext.PluginExecutionContext);
        }
    }
}
