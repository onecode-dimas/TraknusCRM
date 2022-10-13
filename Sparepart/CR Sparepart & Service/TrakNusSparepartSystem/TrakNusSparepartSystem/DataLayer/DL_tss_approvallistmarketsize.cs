using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.DataLayer
{
    public class DL_tss_approvallistmarketsize
    {
        #region Dependencies
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        private DL_tss_marketsizeresultpss _DL_tss_marketsizeresultpss = new DL_tss_marketsizeresultpss();
        private DL_tss_marketsizeresultbranch _DL_tss_marketsizeresultbranch = new DL_tss_marketsizeresultbranch();
        private DL_tss_marketsizeresultnational _DL_tss_marketsizeresultnational = new DL_tss_marketsizeresultnational();
        #endregion

        #region Properties
        private string _classname = "DL_tss_approvallistmarketsize";

        private string _entityname = "tss_approvallistmarketsize";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Approval List Market Size";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _tss_approver = false;
        private EntityReference _tss_approver_value;
        public Guid tss_approver
        {
            get { return _tss_approver ? _tss_approver_value.Id : Guid.Empty; }
            set { _tss_approver = true; _tss_approver_value = new EntityReference(_DL_systemuser.EntityName, value); }
        }

        private bool _tss_marketsizeresultpss = false;
        private EntityReference _tss_marketsizeresultpss_value;
        public Guid tss_marketsizeresultpss
        {
            get { return _tss_marketsizeresultpss ? _tss_marketsizeresultpss_value.Id : Guid.Empty; }
            set { _tss_marketsizeresultpss = true; _tss_marketsizeresultpss_value = new EntityReference(_DL_tss_marketsizeresultpss.EntityName, value); }
        }

        private bool _tss_marketsizeresultbranch = false;
        private EntityReference _tss_marketsizeresultbranch_value;
        public Guid tss_marketsizeresultbranch
        {
            get { return _tss_marketsizeresultbranch ? _tss_marketsizeresultbranch_value.Id : Guid.Empty; }
            set { _tss_marketsizeresultbranch = true; _tss_marketsizeresultbranch_value = new EntityReference(_DL_tss_marketsizeresultbranch.EntityName, value); }
        }

        private bool _tss_marketsizeresultnational = false;
        private EntityReference _tss_marketsizeresultnational_value;
        public Guid tss_marketsizeresultnational
        {
            get { return _tss_marketsizeresultnational ? _tss_marketsizeresultnational_value.Id : Guid.Empty; }
            set { _tss_marketsizeresultnational = true; _tss_marketsizeresultnational_value = new EntityReference(_DL_tss_marketsizeresultnational.EntityName, value); }
        }

        //Option Set
        private bool _tss_type = false;
        private int _tss_type_value;
        public int tss_type
        {
            get { return _tss_type ? _tss_type_value : int.MinValue; }
            set { _tss_type = true; _tss_type_value = value; }
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
                if (_tss_approver) { entity.Attributes["tss_approver"] = _tss_approver_value; }
                if (_tss_marketsizeresultpss) { entity.Attributes["tss_marketsizeresultpss"] = _tss_marketsizeresultpss_value; }
                if (_tss_marketsizeresultbranch) { entity.Attributes["tss_marketsizeresultbranch"] = _tss_marketsizeresultbranch_value; }
                if (_tss_marketsizeresultnational) { entity.Attributes["tss_marketsizeresultnational"] = _tss_marketsizeresultnational_value; }
                if (_tss_type) { entity.Attributes["tss_type"] = new OptionSetValue(_tss_type_value); }
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
                if (_tss_approver) { entity.Attributes["tss_approver"] = _tss_approver_value; }
                if (_tss_marketsizeresultpss) { entity.Attributes["tss_marketsizeresultpss"] = _tss_marketsizeresultpss_value; }
                if (_tss_marketsizeresultbranch) { entity.Attributes["tss_marketsizeresultbranch"] = _tss_marketsizeresultbranch_value; }
                if (_tss_marketsizeresultnational) { entity.Attributes["tss_marketsizeresultnational"] = _tss_marketsizeresultnational_value; }
                if (_tss_type) { entity.Attributes["tss_type"] = new OptionSetValue(_tss_type_value); }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
