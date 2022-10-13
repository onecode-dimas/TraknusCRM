using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TrakNusSparepartSystem.DataLayer;
using TrakNusSparepartSystem.Helper;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusSparepartSystem.Plugins.BusinessLayer
{
    public class BL_tss_quotationpartlines
    {
        #region Constants
        private const string _classname = "BL_tss_quotationpartlines";
        private const string _entityname = "tss_quotationpartlines";
        private const int _depth = 1;
        #endregion

        #region Dependencies
        #endregion

        #region Publics
        public void Form_OnCreate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {

                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                String quotationNo = String.Empty;
                String noSequence = String.Empty;
                if (entity.LogicalName == _entityname)
                {
                    if (entity.Attributes.Contains("tss_quotationpartheader") && entity.Attributes["tss_quotationpartheader"] != null)
                    {
                        Guid quotationId = entity.GetAttributeValue<EntityReference>("tss_quotationpartheader").Id;
                        Entity quotationEntity = organizationService.Retrieve("tss_quotationpartheader", quotationId, new ColumnSet(true));
                        quotationNo = quotationEntity.GetAttributeValue<String>("tss_quotationnumber");
                    }

                        

                    if (entity.Attributes.Contains("tss_itemnumber")) noSequence = entity.GetAttributeValue<int>("tss_itemnumber").ToString();

                    if (entity.Attributes.Contains("tss_name"))
                        entity.Attributes["tss_name"] = quotationNo + " - " + noSequence;
                    else
                        entity.Attributes.Add("tss_name", quotationNo + " - " + noSequence);

                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PreOperation: " + ex.Message.ToString());
            }
        }
        #endregion
    }
}
