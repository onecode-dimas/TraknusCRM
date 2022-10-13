using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhancementCRM.HelperUnit.Data_Layer
{
    public class DL_categorycode
    {
        #region Dependencies
        private DL_runningnumber _runningnumber = new DL_runningnumber();
        private DL_systemuser _systemuser = new DL_systemuser();
        private DL_businessunit _businessunit = new DL_businessunit();
        #endregion
        #region Properties
        private string _classname = "DL_categorycode";

        private string _entityname = "tss_categorycode";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Category Code Sparepart";
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
