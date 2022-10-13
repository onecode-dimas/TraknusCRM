using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TrakNusSparepartSystem.DataLayer;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusSparepartSystem.DataLayer
{
    public class DL_tss_salesindicator
    {
        #region Dependencies
        #endregion

        #region Contants
        #endregion

        #region Properties
        private string _classname = "DL_tss_salesindicator";
        private string _entityname = "tss_indicator";
        public string EntityName
        {
            get { return _entityname; }
        }
        private string _displayname = "Sales Indicator";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _tss_indicator = false;
        private string _tss_indicator_value;
        public string tss_indicator
        {
            get { return _tss_indicator ? _tss_indicator_value : null; }
            set { _tss_indicator = true; _tss_indicator_value = value; }
        }

        private bool _tss_indicatorvalue = false;
        private int _tss_indicatorvalue_value;
        public int tss_indicatorvalue
        {
            get { return _tss_indicatorvalue ? _tss_indicatorvalue_value : int.MinValue; }
            set { _tss_indicatorvalue = true; _tss_indicatorvalue_value = value; }
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
                throw new Exception(".Select : " + ex.Message, ex.InnerException);
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

        public void CreateSalesIndicator(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                entity.Id = id;
                if (_tss_indicator) { entity.Attributes["tss_indicator"] = _tss_indicator_value; }
                if (_tss_indicatorvalue) { entity.Attributes["tss_value"] = _tss_indicatorvalue_value; }
                organizationService.Create(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".CreateSalesIndicator : " + ex.Message.ToString());
            }
        }

        public void UpdateSalesIndicator(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                entity.Id = id;
                if (_tss_indicator) { entity.Attributes["tss_indicator"] = _tss_indicator_value; }
                if (_tss_indicatorvalue) { entity.Attributes["tss_value"] = _tss_indicatorvalue_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".UpdateSalesIndicator : " + ex.Message.ToString());
            }
        }

    }
}
