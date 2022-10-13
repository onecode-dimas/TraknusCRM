using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_partpricemaster
    {
        #region Dependencies
        private DL_trs_masterpart _DL_trs_masterpart = new DL_trs_masterpart();
        #endregion

        #region Properties
        private string _classname = "DL_trs_partpricemaster";

        private string _entityname = "trs_partpricemaster";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Part Price Master";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _trs_partmaster = false;
        private EntityReference _trs_partmaster_value;
        public Guid trs_partmaster
        {
            get { return _trs_partmaster ? _trs_partmaster_value.Id : Guid.Empty; }
            set { _trs_partmaster = true; _trs_partmaster_value = new EntityReference(_DL_trs_masterpart.EntityName, value); }
        }

        private Money _trs_price1;
        public Money trs_price1
        {
            get { return _trs_price1; }
            set { _trs_price1 = value; }
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
                throw new Exception(_classname + ".Select : " + ex.Message.ToString());
            }
        }
    }
}
