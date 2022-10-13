using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace DataLayer
{
    public class DL_tss_quotationpartheader
    {
        #region Dependencies
        #endregion

        #region Properties
        private string _classname = "DL_tss_quotationpartheader";

        private string _entityname = "tss_quotationpartheader";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Quotation Part Header";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _trs_spbnumber = false;
        private string _trs_spbnumber_value;
        public string trs_spbnumber
        {
            get { return _trs_spbnumber ? _trs_spbnumber_value : null; }
            set { _trs_spbnumber = true; _trs_spbnumber_value = value; }
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
                throw new Exception("Error DL_tss_quotationpartheader.Select : " + ex.Message, ex.InnerException);
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
                throw new Exception("Error DL_tss_quotationpartheader.Select : " + ex.Message, ex.InnerException);
            }
        }
    }
}
