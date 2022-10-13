using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Messages;

namespace TrakNusSparepartSystem.Workflow.Helper
{
    public class HelperFunction
    {
        #region Publics
        public static int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }

        public void GetObjectTypeCode(IOrganizationService organizationService, string logicalName, out string ObjectTypeCode)
        {
            try
            {
                Entity entity = new Entity(logicalName);
                RetrieveEntityRequest EntityRequest = new RetrieveEntityRequest();
                EntityRequest.LogicalName = entity.LogicalName;
                EntityRequest.EntityFilters = EntityFilters.All;
                RetrieveEntityResponse responseent = (RetrieveEntityResponse)organizationService.Execute(EntityRequest);
                EntityMetadata ent = responseent.EntityMetadata;
                ObjectTypeCode = ent.ObjectTypeCode.ToString();
            }
            catch
            {
                throw new Exception("Entity with logical name " + logicalName + " not found!");
            }
        }
        #endregion
    }
}
