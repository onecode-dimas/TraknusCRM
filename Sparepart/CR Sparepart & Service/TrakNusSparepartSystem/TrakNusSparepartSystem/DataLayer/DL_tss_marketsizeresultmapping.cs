using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.DataLayer
{
    public class DL_tss_marketsizeresultmapping
    {
        #region Dependencies
        DL_tss_keyaccount _DL_tss_keyaccount = new DL_tss_keyaccount();
        DL_tss_marketsizeresultpss _DL_tss_marketsizeresultpss = new DL_tss_marketsizeresultpss();
        DL_tss_marketsizeresultbranch _DL_tss_marketsizeresultbranch = new DL_tss_marketsizeresultbranch();
        DL_tss_marketsizeresultnational _DL_tss_marketsizeresultnational = new DL_tss_marketsizeresultnational();
        #endregion
        #region Properties
        private string _classname = "DL_tss_marketsizeresultmapping";

        private string _entityname = "tss_marketsizeresultmapping";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Market Size Result Mapping";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _tss_keyaccount;
        private EntityReference _tss_keyaccount_value;
        public Guid tss_keyaccount
        {
            get { return _tss_keyaccount ? _tss_keyaccount_value.Id : Guid.Empty; }
            set { _tss_keyaccount = true; _tss_keyaccount_value = new EntityReference(_DL_tss_keyaccount.EntityName, value); }
        }

        private bool _tss_marketsizeresultpss;
        private EntityReference _tss_marketsizeresultpss_value;
        public Guid tss_marketsizeresultpss
        {
            get { return _tss_marketsizeresultpss ? _tss_marketsizeresultpss_value.Id : Guid.Empty; }
            set { _tss_marketsizeresultpss = true; _tss_marketsizeresultpss_value = new EntityReference(_DL_tss_marketsizeresultpss.EntityName, value); }
        }

        private bool _tss_marketsizeresultbranch;
        private EntityReference _tss_marketsizeresultbranch_value;
        public Guid tss_marketsizeresultbranch
        {
            get { return _tss_marketsizeresultbranch ? _tss_marketsizeresultbranch_value.Id : Guid.Empty; }
            set { _tss_marketsizeresultbranch = true; _tss_marketsizeresultbranch_value = new EntityReference(_DL_tss_marketsizeresultbranch.EntityName, value); }
        }

        private bool _tss_marketsizeresultnational;
        private EntityReference _tss_marketsizeresultnational_value;
        public Guid tss_marketsizeresultnational
        {
            get { return _tss_marketsizeresultnational ? _tss_marketsizeresultnational_value.Id : Guid.Empty; }
            set { _tss_marketsizeresultnational = true; _tss_marketsizeresultnational_value = new EntityReference(_DL_tss_marketsizeresultnational.EntityName, value); }
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

        public Guid Insert(IOrganizationService organizationService)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                if (_tss_keyaccount) { entity.Attributes["tss_keyaccount"] = _tss_keyaccount_value; }
                if (_tss_marketsizeresultpss) { entity.Attributes["tss_marketsizeresultpss"] = _tss_marketsizeresultpss_value; }
                if (_tss_marketsizeresultbranch) { entity.Attributes["tss_marketsizeresultbranch"] = _tss_marketsizeresultbranch_value; }
                if (_tss_marketsizeresultnational) { entity.Attributes["tss_marketsizeresultnational"] = _tss_marketsizeresultnational_value; }
                return organizationService.Create(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Insert : " + ex.Message.ToString());
            }
        }
        public void Update(IOrganizationService organizationService, Guid Id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                entity.Id = Id;
                if (_tss_keyaccount) { entity.Attributes["tss_keyaccount"] = _tss_keyaccount_value; }
                if (_tss_marketsizeresultpss) { entity.Attributes["tss_marketsizeresultpss"] = _tss_marketsizeresultpss_value; }
                if (_tss_marketsizeresultbranch) { entity.Attributes["tss_marketsizeresultbranch"] = _tss_marketsizeresultbranch_value; }
                if (_tss_marketsizeresultnational) { entity.Attributes["tss_marketsizeresultnational"] = _tss_marketsizeresultnational_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
