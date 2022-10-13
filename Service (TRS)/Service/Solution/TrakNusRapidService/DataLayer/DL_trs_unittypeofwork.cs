using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_unittypeofwork
    {
        #region Dependencies
        DL_trs_producttype _DL_trs_producttype = new DL_trs_producttype();
        #endregion

        #region Properties
        private string _classname = "DL_trs_unittypeofwork";

        private string _entityname = "trs_unittypeofwork";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Product Type of Work";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _trs_producttype = false;
        private EntityReference _trs_producttype_value;
        public Guid trs_producttype
        {
            get { return _trs_producttype ? _trs_producttype_value.Id : Guid.Empty; }
            set { _trs_producttype = true; _trs_producttype_value = new EntityReference(_DL_trs_producttype.EntityName, value); }
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
