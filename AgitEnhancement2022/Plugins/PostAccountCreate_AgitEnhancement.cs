using System;
using Microsoft.Xrm.Sdk;
using AgitEnhancement2022.Plugins.BusinessLayer;

namespace AgitEnhancement2022.Plugins
{
    public class PostAccountCreate_AgitEnhancement : Plugin
    {
        public PostAccountCreate_AgitEnhancement()
            : base(typeof(PostAccountCreate_AgitEnhancement))
        {
            base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, "Create", "account", 
                new Action<LocalPluginContext>(ExecutePostAccountCreate_AgitEnhancement)));
            
        }

        protected void ExecutePostAccountCreate_AgitEnhancement(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new ArgumentNullException("localContext");
            }

            ITracingService tracer = localContext.TracingService;
            BL_account _BL_account = new BL_account();
            _BL_account.PostCreate_Account(localContext.OrganizationService, localContext.PluginExecutionContext, tracer);
        }
    }
}
