using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_quotationcommercialdetail
    {
        #region Dependencies
        private DL_trs_mechanicgrade _DL_trs_mechanicgrade = new DL_trs_mechanicgrade();
        private DL_trs_tasklist _DL_trs_tasklist = new DL_trs_tasklist();
        private DL_trs_commercialtask _DL_trs_commercialtask = new DL_trs_commercialtask();
        private DL_trs_quotation _DL_trs_quotation = new DL_trs_quotation();
        private DL_trs_toolsgroup _DL_trs_toolsgroup = new DL_trs_toolsgroup();
        private DL_trs_trs_quotationcommercialdetail_trs_tools _DL_trs_trs_quotationcommercialdetail_trs_tools = new DL_trs_trs_quotationcommercialdetail_trs_tools();
        #endregion

        #region Properties
        private string _classname = "DL_trs_quotationcommercialdetail";

        private string _entityname = "trs_quotationcommercialdetail";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Quotation Commercial Detail";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private string _trs_taskcode;
        public string trs_taskcode
        {
            get { return _trs_taskcode; }
            set { _trs_taskcode = value; }
        }

        private bool _trs_tasklistdetail = false;
        private EntityReference _trs_tasklistdetail_value;
        public Guid trs_tasklistdetail
        {
            get { return _trs_tasklistdetail ? _trs_tasklistdetail_value.Id : Guid.Empty; }
            set { _trs_tasklistdetail = true; _trs_tasklistdetail_value = new EntityReference(_DL_trs_commercialtask.EntityName, value); }
        }

        private bool _trs_quotation = false;
        private EntityReference _trs_quotation_value;
        public Guid trs_quotation
        {
            get { return _trs_quotation ? _trs_quotation_value.Id : Guid.Empty; }
            set { _trs_quotation = true; _trs_quotation_value = new EntityReference(_DL_trs_quotation.EntityName, value); }
        }

        private Guid _trs_commercialheader;
        public Guid trs_commercialheader
        {
            get { return _trs_commercialheader; }
            set { _trs_commercialheader = value; }
        }

        private bool _trs_mechanicgrade = false;
        private EntityReference _trs_mechanicgrade_value;
        public Guid trs_mechanicgrade
        {
            get { return _trs_mechanicgrade ? _trs_mechanicgrade_value.Id : Guid.Empty; }
            set { _trs_mechanicgrade = true; _trs_mechanicgrade_value = new EntityReference(_DL_trs_mechanicgrade.EntityName, value); }
        }

        private bool _trs_rtg = false;
        private decimal _trs_rtg_value;
        public decimal trs_rtg
        {
            get { return _trs_rtg_value; }
            set { _trs_rtg = true; _trs_rtg_value = value; }
        }

        private bool _taskname = false;
        private EntityReference _taskname_value;
        public Guid Taskname
        {
            get { return _taskname ? _taskname_value.Id : Guid.Empty; }
            set { _taskname = true; _taskname_value = new EntityReference(_DL_trs_tasklist.EntityName, value); }
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
                throw new Exception(_classname + ".Select : " + ex.Message, ex.InnerException);
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
                throw new Exception(_classname + ".Select : " + ex.Message, ex.InnerException);
            }
        }

        public Guid Insert(IOrganizationService organizationService)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                entity.Attributes["trs_commercialheader"] = new EntityReference("trs_quotationcommercialheader", _trs_commercialheader);
                if (_trs_quotation) entity.Attributes["trs_quotation"] = _trs_quotation_value;
                if (_trs_tasklistdetail) entity.Attributes["trs_tasklistdetail"] = _trs_tasklistdetail_value;
                entity.Attributes["trs_taskcode"] = _trs_taskcode;
                if (_trs_mechanicgrade) { entity.Attributes["trs_mechanicgrade"] = _trs_mechanicgrade_value; }
                if (_trs_rtg) { entity.Attributes["trs_rtg"] = _trs_rtg_value; }
                if (_taskname) { entity.Attributes["trs_taskname"] = _taskname_value; }
                return organizationService.Create(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Insert : " + ex.Message, ex.InnerException);
            }
        }

        public void Update(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                entity.Id = id;
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }


        public void AssociateToolGroups(IOrganizationService organizationService, Guid id, List<Guid> toolGroupsId)
        {
            try
            {
                EntityReferenceCollection relatedEntites = new EntityReferenceCollection();
                foreach (Guid toolGroupId in toolGroupsId)
                {
                    relatedEntites.Add(new EntityReference(_DL_trs_toolsgroup.EntityName, toolGroupId));
                }

                Relationship relationship = new Relationship(_DL_trs_trs_quotationcommercialdetail_trs_tools.EntityName);
                organizationService.Associate(_entityname, id, relationship, relatedEntites);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".AssociateToolsGroup : " + ex.Message);
            }
        }
    }
}
