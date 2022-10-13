using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.DataLayer
{
    public class DL_tss_kauio
    {
        #region Dependencies
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        private DL_account _DL_account = new DL_account();
        private DL_population _DL_population = new DL_population();
        #endregion

        #region Properties
        private string _classname = "DL_tss_kauio";

        private string _entityname = "tss_kauio";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "KA UIO";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _tss_errordescription = false;
        private string _tss_errordescription_value;
        public string tss_errordescription
        {
            get { return _tss_errordescription ? _tss_errordescription_value : null; }
            set { _tss_errordescription = true; _tss_errordescription_value = value; }
        }

        private bool _tss_pss = false;
        private EntityReference _tss_pss_value;
        public Guid tss_pss
        {
            get { return _tss_pss ? _tss_pss_value.Id : Guid.Empty; }
            set { _tss_pss = true; _tss_pss_value = new EntityReference(_DL_systemuser.EntityName, value); }
        }

        private bool _tss_customer = false;
        private EntityReference _tss_customer_value;
        public Guid tss_customer
        {
            get { return _tss_customer ? _tss_customer_value.Id : Guid.Empty; }
            set { _tss_customer = true; _tss_customer_value = new EntityReference(_DL_account.EntityName, value); }
        }

        private bool _tss_serialnumber = false;
        private EntityReference _tss_serialnumber_value;
        public Guid tss_serialnumber
        {
            get { return _tss_serialnumber ? _tss_serialnumber_value.Id : Guid.Empty; }
            set { _tss_serialnumber = true; _tss_serialnumber_value = new EntityReference(_DL_population.EntityName, value); }
        }

        //Two Option
        private bool _tss_calculatetoms = false;
        private bool _tss_calculatetoms_value;
        public bool tss_calculatetoms
        {
            get { return _tss_calculatetoms ? _tss_calculatetoms_value : false; }
            set { _tss_calculatetoms = true; _tss_calculatetoms_value = value; }
        }

        //Two Option
        private bool _tss_calculatestatus = false;
        private bool _tss_calculatestatus_value;
        public bool tss_calculatestatus
        {
            get { return _tss_calculatestatus ? _tss_calculatestatus_value : false; }
            set { _tss_calculatestatus = true; _tss_calculatestatus_value = value; }
        }

        //Option Set
        private bool _tss_reason = false;
        private int _tss_reason_value;
        public int tss_reason
        {
            get { return _tss_reason ? _tss_reason_value : int.MinValue; }
            set { _tss_reason = true; _tss_reason_value = value; }
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

        public void UpdateStatus(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                entity.Id = id;
                if (_tss_calculatestatus) { entity.Attributes["tss_calculatestatus"] = _tss_calculatestatus_value; }
                if (_tss_errordescription) { entity.Attributes["tss_errordescription"] = _tss_errordescription_value; }

                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
