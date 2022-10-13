using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusSparepartSystem.DataLayer
{
    public class DL_tss_prospectpartheader
    {
        #region Dependencies
        #endregion

        #region Properties
        private string _classname = "DL_tss_prospectpartheader";

        private string _entityname = "tss_prospectpartheader";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Prospect Part Header";
        public string DisplayName
        {
            get { return _displayname; }
        }

        //OptionSet
        private bool _tss_statusreason = false;
        private int _tss_statusreason_value;
        public int tss_statusreason
        {
            get { return _tss_statusreason ? _tss_statusreason_value : int.MinValue; }
            set { _tss_statusreason = true; _tss_statusreason_value = value; }
        }

        //OptionSet
        private bool _tss_pipelinephase = false;
        private int _tss_pipelinephase_value;
        public int tss_pipelinephase
        {
            get { return _tss_pipelinephase ? _tss_pipelinephase_value : int.MinValue; }
            set { _tss_pipelinephase = true; _tss_pipelinephase_value = value; }
        }

        private bool _tss_reviser = false;
        private bool _tss_reviser_value;
        public bool tss_reviser
        {
            get { return _tss_reviser ? _tss_reviser_value : false; }
            set { _tss_reviser = true; _tss_reviser_value = value; }
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

        public void UpdateStatusReasonAndPipeline(IOrganizationService orgService, Guid id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                entity.Id = id;
                if (_tss_pipelinephase) { entity.Attributes["tss_pipelinephase"] = new OptionSetValue(_tss_pipelinephase_value); }
                if (_tss_statusreason) { entity.Attributes["tss_statusreason"] = new OptionSetValue(_tss_statusreason_value); }
                //if (_tss_reviser) { entity.Attributes["tss_reviseprospect"] = _tss_reviser_value; }
                orgService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".UpdateStatusReasonAndPipeline : " + ex.Message.ToString());
            }
        }

        public void UpdateReviserProspect(IOrganizationService orgService, Guid id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                entity.Id = id;
                if (_tss_reviser) { entity.Attributes["tss_reviseprospect"] = _tss_reviser_value; }
                orgService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".UpdateReviserProspect: " + ex.Message.ToString());
            }
        }
    }
}
