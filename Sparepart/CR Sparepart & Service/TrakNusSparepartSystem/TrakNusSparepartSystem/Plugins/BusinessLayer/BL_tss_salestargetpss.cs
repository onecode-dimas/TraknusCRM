using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TrakNusSparepartSystem.DataLayer;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Crm.Sdk.Messages;
using TrakNusSparepartSystem.Helper;

namespace TrakNusSparepartSystem.Plugins.BusinessLayer
{
    public class BL_tss_salestargetpss
    {
        #region Constants
        private const string _classname = "BL_tss_salestargetpss";
        private const string _entityname = "tss_salestargetpss";
        #endregion

        #region DELETE

        public void Form_OnDelete_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));

                if (entity.LogicalName == _entityname)
                {
                    if (entity.Attributes.Contains("tss_marketsizeid"))
                    {
                        Entity _marketsizeresultpss = organizationService.Retrieve("tss_marketsizeresultpss", entity.GetAttributeValue<EntityReference>("tss_marketsizeid").Id, new ColumnSet(true));

                        if (_marketsizeresultpss.Id != null)
                        {
                            throw new InvalidPluginExecutionException("Sales Target PSS cannot be deleted !");
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnDelete_PreOperation : " + ex.Message.ToString());
            }
        }

        #endregion
    }
}
