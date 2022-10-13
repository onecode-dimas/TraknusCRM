using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_toolstransfer
    {
        #region Dependencies
        private DL_trs_toolsmaster _DL_trs_toolsmaster = new DL_trs_toolsmaster();
        #endregion

        #region Properties
        private string _classname = "DL_trs_toolstransfer";

        private string _entityname = "trs_toolstransfer";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Tools Transfer";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private string _trs_transferreason;
        public string trs_transferreason
        {
            get { return _trs_transferreason; }
            set { _trs_transferreason = value; }
        }

        private DateTime _trs_transferdate;
        public DateTime trs_transferdate
        {
            get { return _trs_transferdate.ToLocalTime(); }
            set { _trs_transferdate = value.ToLocalTime(); }
        }

        private bool _trs_branch = false;
        private EntityReference _trs_branch_value;
        public Guid trs_branch
        {
            get { return _trs_branch ? _trs_branch_value.Id : Guid.Empty; }
            set { _trs_branch = true; _trs_branch_value = new EntityReference("businessunit", value); }
        }

        private bool _trs_tools = false;
        private EntityReference _trs_tools_value;
        public Guid trs_tools
        {
            get { return _trs_tools ? _trs_tools_value.Id : Guid.Empty; }
            set { _trs_tools = true; _trs_tools_value = new EntityReference(_DL_trs_toolsmaster.EntityName, value); }
        }
        #endregion

        public Entity Select(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = organizationService.Retrieve(_entityname, id, new ColumnSet(new string[] { "trs_transferreason", "trs_transferdate", "trs_branch", "trs_tools" }));
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception("Error DL_trs_toolstransfer.Select : " + ex.Message, ex.InnerException);
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
                throw new Exception("Error DL_trs_toolstransfer.Select : " + ex.Message, ex.InnerException);
            }
        }

        public Guid Insert(IOrganizationService organizationService)
        {
            try
            {
                if (!string.IsNullOrEmpty(_trs_transferreason))
                {
                    Entity entity = new Entity(_entityname);
                    entity.Attributes["trs_transferreason"] = _trs_transferreason;
                    entity.Attributes["trs_transferdate"] = _trs_transferdate;
                    entity.Attributes["trs_tools"] = _trs_tools;
                    entity.Attributes["trs_branch"] = _trs_branch;
                    return organizationService.Create(entity);
                }
                else
                {
                    throw new Exception(_classname + ".Insert : Tools Master is empty.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Insert : " + ex.Message.ToString());
            }
        }        
    }
}
