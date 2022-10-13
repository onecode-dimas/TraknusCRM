using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_responsiblecostcenter
    {
        #region Dependencies
        #endregion

        #region Properties
        private string _classname = "DL_trs_responsiblecostcenter";

        private string _entityname = "trs_responsiblecostcenter";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Responsible Cost Center";
        public string DisplayName
        {
            get { return _displayname; }
        }
        #endregion

        public Entity Select(IOrganizationService organizationService, Guid id)
        {
            try
            {
                return organizationService.Retrieve(_entityname, id, new ColumnSet(true));
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Select : " + ex.Message.ToString());
            }
        }

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
