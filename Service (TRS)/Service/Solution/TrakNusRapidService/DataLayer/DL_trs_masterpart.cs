using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_masterpart
    {
        #region Properties
        private string _classname = "DL_trs_masterpart";

        private string _entityname = "trs_masterpart";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Part Master";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private string _trs_Task;
        public string trs_Task
        {
            get { return _trs_Task; }
            set { _trs_Task = value; }
        }

        private string _trs_name;
        public string trs_name
        {
            get { return _trs_name; }
            set { _trs_name = value; }
        }

        private string _trs_PartDescription;
        public string trs_PartDescription
        {
            get { return _trs_PartDescription; }
            set { _trs_PartDescription = value; }
        }
        #endregion

        public Entity Select(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = organizationService.Retrieve(_entityname, id, new ColumnSet(true));
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
