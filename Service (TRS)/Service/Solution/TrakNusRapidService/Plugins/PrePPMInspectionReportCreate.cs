// <copyright file="PrePPMInspectionReportCreate.cs" company="">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author></author>
// <date>2/19/2015 11:20:12 AM</date>
// <summary>Implements the PrePPMInspectionReportCreate Plugin.</summary>
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
    /// PrePPMInspectionReportCreate Plugin.
    /// </summary>    
    public class PrePPMInspectionReportCreate: Plugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrePPMInspectionReportCreate"/> class.
        /// </summary>
        public PrePPMInspectionReportCreate()
            : base(typeof(PrePPMInspectionReportCreate))
        {
            base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(20, "Create", "trs_ppmreport", new Action<LocalPluginContext>(ExecutePrePPMInspectionReportCreate)));

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
        protected void ExecutePrePPMInspectionReportCreate(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new ArgumentNullException("localContext");
            }

            // TODO: Implement your custom Plug-in business logic.
            BL_trs_ppmreport _BL_trs_ppmreport = new BL_trs_ppmreport();
            _BL_trs_ppmreport.Form_OnCreate_PreOperation(localContext.OrganizationService, localContext.PluginExecutionContext);
        }
    }
}
