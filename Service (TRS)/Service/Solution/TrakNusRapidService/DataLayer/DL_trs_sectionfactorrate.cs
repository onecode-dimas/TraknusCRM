using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_sectionfactorrate
    {
        #region Properties
        private string _classname = "DL_trs_sectionfactorrate";

        private string _entityname = "trs_sectionfactorrate";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Section Factor Rate";
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
                EntityCollection entityCollection = organizationService.RetrieveMultiple(queryExpression);
                return entityCollection;

            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Select  : " + ex.Message.ToString());
            }
        }
    }
}
