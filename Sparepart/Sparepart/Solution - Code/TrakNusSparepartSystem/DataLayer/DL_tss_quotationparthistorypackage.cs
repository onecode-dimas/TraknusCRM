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
    public class DL_tss_quotationparthistorypackage
    {
        #region Dependencies
        private DL_account _DL_account = new DL_account();
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        private DL_businessunit _DL_businessunit = new DL_businessunit();
        private DL_tss_prospectpartheader _DL_tss_prospectpartheader = new DL_tss_prospectpartheader();
        private DL_trs_quotation _DL_trs_quotation = new DL_trs_quotation();
        private DL_contact _DL_contact = new DL_contact();
        private DL_currency _DL_currency = new DL_currency();
        #endregion

        #region Properties
        private string _classname = "DL_tss_quotationparthistorypackage";

        private string _entityname = "tss_quotationparthistorypackage";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Quotation Part - History Package";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _tss_packageno = false;
        private string _tss_packageno_value;
        public string tss_packageno
        {
            get { return _tss_packageno ? _tss_packageno_value : null; }
            set { _tss_packageno = true; _tss_packageno_value = value; }
        }

        private bool _tss_packagesname = false;
        private string _tss_packagesname_value;
        public string tss_packagesname
        {
            get { return _tss_packagesname ? _tss_packagesname_value : null; }
            set { _tss_packagesname = true; _tss_packagesname_value = value; }
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

        public Guid CreateQuotationPartHistoryPackage(IOrganizationService organizationService)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                if (_tss_packageno) { entity.Attributes["tss_packageno"] = _tss_packageno_value; }
                if (_tss_packagesname) { entity.Attributes["tss_packagename"] = _tss_packagesname_value; }
                return organizationService.Create(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".CreateQuotationPartHistoryPackage : " + ex.Message.ToString());
            }
        }
    }
}
