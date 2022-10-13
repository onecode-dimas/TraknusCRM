// <copyright file="PostQuotationPartUpdate.cs" company="">
// Copyright (c) 2018 All Rights Reserved
// </copyright>
// <author></author>
// <date>1/18/2018 1:24:57 PM</date>
// <summary>Implements the PostQuotationPartUpdate Plugin.</summary>
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
// </auto-generated>
namespace TrakNusSparepartSystem.Plugins
{
    using System;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using TrakNusSparepartSystem.Plugins.BusinessLayer;

    /// <summary>
    /// PostQuotationPartUpdate Plugin.
    /// Fires when the following attributes are updated:
    /// All Attributes
    /// </summary>    
    public class PostQuotationPartUpdate: Plugin
    {
        /// <summary>
        /// Alias of the image registered for the snapshot of the 
        /// primary entity's attributes after the core platform operation executes.
        /// The image contains the following attributes:
        /// tss_quotationserviceno,tss_requestdeliverydate,tss_underminimumprice
        /// 
        /// Note: Only synchronous post-event and asynchronous registered plug-ins 
        /// have PostEntityImages populated.
        /// </summary>
        private readonly string postImageAlias = "PostImage";
        private readonly string preImageAlias = "PreImage";

        /// <summary>
        /// Initializes a new instance of the <see cref="PostQuotationPartUpdate"/> class.
        /// </summary>
        public PostQuotationPartUpdate()
            : base(typeof(PostQuotationPartUpdate))
        {
            base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, "Update", "tss_quotationpartheader", new Action<LocalPluginContext>(ExecutePostQuotationPartUpdate)));

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
        protected void ExecutePostQuotationPartUpdate(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new ArgumentNullException("localContext");
            }

            IPluginExecutionContext context = localContext.PluginExecutionContext;

            Entity postImageEntity = (context.PostEntityImages != null && context.PostEntityImages.Contains(this.postImageAlias)) ? context.PostEntityImages[this.postImageAlias] : null;
            Entity preImageEntity = (context.PreEntityImages != null && context.PreEntityImages.Contains(this.preImageAlias)) ? context.PreEntityImages[this.preImageAlias] : null;

            // TODO: Implement your custom Plug-in business logic.
            try
            {
                Entity entity = (Entity)context.InputParameters["Target"];
                Entity postImg = (Entity)context.PostEntityImages["PostImage"];
                var SOURCETYPE_SERVICE = 865920002;
                //var STATUSCODE_DRAFT = 865920000;
                var STATUSREASON_WAITINGTOP = 865920001;
                //var entityQuotationPartReasonDiscount = "tss_quotationpartreasondiscountpackage";
                //bool flagReason = true;
                //bool status;
                //update req deivery in quotation part lines
                BL_tss_quotationpartheader BL = new BL_tss_quotationpartheader();

                #region Plugins for Quot. Part Reason Discount
                /*
                if (postImg.Attributes.Contains("tss_pss") && postImg.Attributes.Contains("tss_sourcetype") && postImg.Attributes.Contains("tss_statuscode")
                    && postImg.Attributes.Contains("tss_statusreason") && postImg.Attributes.Contains("tss_underminimumprice"))
                {
                    if (postImg.GetAttributeValue<OptionSetValue>("tss_statuscode").Value == STATUSCODE_DRAFT
                    && postImg.GetAttributeValue<OptionSetValue>("tss_statusreason").Value == STATUSREASON_OPEN
                    && postImg.GetAttributeValue<bool>("tss_underminimumprice") == true
                    && postImg.GetAttributeValue<OptionSetValue>("tss_sourcetype").Value != SOURCETYPE_SERVICE)
	                {
                        Entity quotation = localContext.OrganizationService.Retrieve(localContext.PluginExecutionContext.PrimaryEntityName, localContext.PluginExecutionContext.PrimaryEntityId, new ColumnSet(true));
                        if (quotation.GetAttributeValue<string>("tss_quotationid") != null)
                        {
                            QueryExpression qQuotationPartDiscount = new QueryExpression(entityQuotationPartReasonDiscount)
                            {
                                ColumnSet = new ColumnSet(true),
                                Criteria = new FilterExpression()
                                {
                                    Conditions =
                                    {
                                        new ConditionExpression("tss_quotationpartheader",ConditionOperator.Equal,localContext.PluginExecutionContext.PrimaryEntityId)
                                    }
                                }
                            };

                            var ecQuotationPartDiscount = localContext.OrganizationService.RetrieveMultiple(qQuotationPartDiscount);
                            //throw new InvalidPluginExecutionException("Resultsss: " + ecQuotationPartDiscount.Entities.Count);
                            foreach (var item in ecQuotationPartDiscount.Entities)
                            {
                                if (item.Attributes["tss_result"] != null)
                                {
                                    throw new InvalidPluginExecutionException("contain");
                                }
                                else
                                {
                                    throw new InvalidPluginExecutionException("not");
                                }
                                status = item.GetAttributeValue<bool>("tss_result");
                                if (status == null)
                                {
                                    flagReason = false;
                                    break;
                                }
                            }

                            //if (flagReason)
                            //{
                            //    BL.UpdateStatusReason(localContext.OrganizationService, context, postImg);
                            //}
                        }
	                }
                }
                 */ 
                #endregion

                BL.UpdateCloseAsWon(localContext.OrganizationService, localContext.PluginExecutionContext, postImg);

                if (postImg.Attributes.Contains("tss_statusreason"))
                {
                    //!postImg.Attributes.Contains("tss_revision")
                    if (postImg.Attributes.Contains("tss_pss")
                        && postImg.GetAttributeValue<OptionSetValue>("tss_sourcetype").Value == SOURCETYPE_SERVICE)
                    {
                        BL.UpdatePSS(localContext.OrganizationService, localContext.PluginExecutionContext, postImageEntity);
                        BL.AssignPSS(localContext.OrganizationService, localContext.PluginExecutionContext, postImg);
                    }

                    /*if (postImg.Attributes.Contains("tss_requestdeliverydate"))
                    {
                        if (context.Depth == 1)
                        {
                            bool underMinPrice = postImg.GetAttributeValue<bool>("tss_underminimumprice");
                            BL.Form_OnUpdateReqDelivery_PostOperation(localContext.OrganizationService, localContext.PluginExecutionContext, underMinPrice);
                        }
                        else
                        {
                            return;
                        }

                        
                    }*/

                    //if (postImg.GetAttributeValue<OptionSetValue>("tss_statusreason").Value == STATUSREASON_WAITINGTOP)
                    //{
                    //    //SEND EMAIL, AFTER FILL TERM OF PAYMENT

                    //}
                    //else
                    //{
                        
                    //}
                }

                
                //update req delivery date on lines when header update
                BL.Form_OnUpdateReqDelivery_PostOperation(localContext.OrganizationService, localContext.PluginExecutionContext);
                if (postImageEntity != null && preImageEntity != null && localContext.OrganizationService != null && localContext.PluginExecutionContext != null)
                {
                    BL.sendEmailtoTOPApproval(localContext.OrganizationService, localContext.PluginExecutionContext, postImageEntity, preImageEntity);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("PostQuotationPartHeaderUpdate: " + ex);
            }
        }
    }
}
