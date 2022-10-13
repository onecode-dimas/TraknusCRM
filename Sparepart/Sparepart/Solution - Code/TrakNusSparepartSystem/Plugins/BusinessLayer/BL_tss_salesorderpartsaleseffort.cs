using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrakNusSparepartSystem.DataLayer;

namespace TrakNusSparepartSystem.Plugins.BusinessLayer
{
    public class BL_tss_salesorderpartsaleseffort
    {
        #region Constants
        private const string _classname = "BL_tss_salesorderpartsaleseffort";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_tss_salesorderpartheader _DL_tss_salesorderpartheader = new DL_tss_salesorderpartheader();
        #endregion

        public void Form_OnUpdate_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExecutionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExecutionContext.InputParameters["Target"];
                if (entity.LogicalName == "tss_sopartsaleseffort")
                {
                    var context = new OrganizationServiceContext(organizationService);
                    var now = (from c in context.CreateQuery("tss_sopartsaleseffort")
                               where c.GetAttributeValue<Guid>("tss_sopartsaleseffortid") == entity.Id
                               select c).ToList();

                    if (now.Count > 0)
                    {
                        if (now[0].Attributes.Contains("tss_sopartheaderid") && now[0].Attributes["tss_sopartheaderid"] != null)
                        {
                            var saleseffortlist = (from c in context.CreateQuery("tss_sopartsaleseffort")
                                                   where c.GetAttributeValue<EntityReference>("tss_sopartheaderid").Id == now[0].GetAttributeValue<EntityReference>("tss_sopartheaderid").Id
                                                   where c.GetAttributeValue<bool>("tss_approvalstatus") == false
                                                   select c).ToList();

                            if (saleseffortlist.Count <= 0)
                            {
                                //update so part header field status sales effort to approved
                                Entity ent = new Entity(_DL_tss_salesorderpartheader.EntityName);
                                ent.Id = now[0].GetAttributeValue<EntityReference>("tss_sopartheaderid").Id;
                                ent.Attributes["tss_approveallsaleseffort"] = true;
                                ent.Attributes["tss_statussaleseffort"] = new OptionSetValue(865920001);
                                organizationService.Update(ent);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate_PostOperation : " + ex.Message.ToString());
            }
        }
    }
}
