// <copyright file="PreQuotationPartLinesDelete.cs" company="">
// Copyright (c) 2018 All Rights Reserved
// </copyright>
// <author></author>
// <date>1/15/2018 9:22:59 PM</date>
// <summary>Implements the PreQuotationPartLinesDelete Plugin.</summary>
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
    /// PreQuotationPartLinesDelete Plugin.
    /// </summary>    
    public class PreQuotationPartLinesDelete: Plugin
    {
        /// <summary>
        /// Alias of the image registered for the snapshot of the 
        /// primary entity's attributes before the core platform operation executes.
        /// The image contains the following attributes:
        /// tss_totalfinalprice,tss_totalprice,tss_underminimumprice
        /// </summary>
        private readonly string preImageAlias = "PreImage";

        /// <summary>
        /// Initializes a new instance of the <see cref="PreQuotationPartLinesDelete"/> class.
        /// </summary>
        public PreQuotationPartLinesDelete()
            : base(typeof(PreQuotationPartLinesDelete))
        {
            base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(20, "Delete", "tss_quotationpartlines", new Action<LocalPluginContext>(ExecutePreQuotationPartLinesDelete)));

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
        protected void ExecutePreQuotationPartLinesDelete(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new ArgumentNullException("localContext");
            }

            IPluginExecutionContext context = localContext.PluginExecutionContext;

            Entity preImageEntity = (context.PreEntityImages != null && context.PreEntityImages.Contains(this.preImageAlias)) ? context.PreEntityImages[this.preImageAlias] : null;

            // TODO: Implement your custom Plug-in business logic.
            BL_tss_quotationpartheader _BL_tss_quotationpartheader = new BL_tss_quotationpartheader();
            _BL_tss_quotationpartheader.Form_OnDelete_PreOperation(localContext.OrganizationService, localContext.PluginExecutionContext, preImageEntity);
        }
    }
}
