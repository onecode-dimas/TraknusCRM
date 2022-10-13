using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_systemuserroles
    {
        #region Dependencies
        #endregion

        #region Properties
        private string _classname = "DL_systemuserroles";

        private string _entityname = "systemuserroles";
        public string EntityName
        {
            get { return _entityname; }
        }
        #endregion
        public EntityCollection Select(IOrganizationService organizationService, QueryExpression queryExpression)
        {
            try
            {
                return organizationService.RetrieveMultiple(queryExpression);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Select : " + ex.Message.ToString());
            }
        }
    }
}
