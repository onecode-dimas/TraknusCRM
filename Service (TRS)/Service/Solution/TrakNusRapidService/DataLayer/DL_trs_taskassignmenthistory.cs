using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_taskassignmenthistory
    {
        #region Dependencies
        private DL_trs_taskassignmenthistory _DL_trs_taskassignmenthistory = new DL_trs_taskassignmenthistory();       
        #endregion

        #region Properties
        private string _classname = "DL_trs_taskassignmenthistory";

        private string _entityname = "trs_taskassignmenthistory";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Task Assignment History";
        public string DisplayName
        {
            get { return _displayname; }
        }

        #endregion


        public Entity Select(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = organizationService.Retrieve(_entityname, id, new ColumnSet(new string[] { "trs_taskcode", "trs_taskname" }));
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Select : " + ex.Message, ex.InnerException);
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
                throw new Exception(_classname + ".Select : " + ex.Message, ex.InnerException);
            }
        }
    }
}
