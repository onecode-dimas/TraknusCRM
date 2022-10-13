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
    public class DL_tss_approverlist
    {
        #region Dependencies
        private DL_account _DL_account = new DL_account();
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        private DL_businessunit _DL_businessunit = new DL_businessunit();
        private DL_tss_prospectpartheader _DL_tss_prospectpartheader = new DL_tss_prospectpartheader();
        private DL_tss_quotationpartheader _DL_tss_quotationpartheader = new DL_tss_quotationpartheader();
        private DL_tss_salesorderpartheader _DL_tss_salesorderpartheader = new DL_tss_salesorderpartheader();
        #endregion

        #region Contants
        private const string _matrixapproverdiscount = "tss_matrixapprovaldiscount";
        #endregion

        #region Properties
        private string _classname = "DL_tss_approverlist";

        private string _entityname = "tss_approverlist";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Approver List";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _tss_quotationpart = false;
        private EntityReference _tss_quotationpart_value;
        public Guid tss_quotationpart
        {
            get { return _tss_quotationpart ? _tss_quotationpart_value.Id : Guid.Empty; }
            set { _tss_quotationpart = true; _tss_quotationpart_value = new EntityReference(_DL_tss_quotationpartheader.EntityName, value); }
        }

        private bool _tss_sopart = false;
        private EntityReference _tss_sopart_value;
        public Guid tss_sopart
        {
            get { return _tss_sopart ? _tss_sopart_value.Id : Guid.Empty; }
            set { _tss_sopart = true; _tss_sopart_value = new EntityReference(_DL_tss_salesorderpartheader.EntityName, value); }
        }

        private bool _tss_approver = false;
        private EntityReference _tss_approver_value;
        public Guid tss_approver
        {
            get { return _tss_approver ? _tss_approver_value.Id : Guid.Empty; }
            set { _tss_approver = true; _tss_approver_value = new EntityReference(_matrixapproverdiscount, value); }
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

        public void CreateApprover(IOrganizationService organizationService)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                if (_tss_quotationpart) { entity.Attributes["tss_quotationpartheaderid"] = _tss_quotationpart_value; }
                if (_tss_sopart) { entity.Attributes["tss_salesorderpartheaderid"] = _tss_sopart_value; }
                if (_tss_approver) { entity.Attributes["tss_approver"] = _tss_approver_value; }
                organizationService.Create(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".CreateApprover : " + ex.Message.ToString());
            }
        }


    }
}
