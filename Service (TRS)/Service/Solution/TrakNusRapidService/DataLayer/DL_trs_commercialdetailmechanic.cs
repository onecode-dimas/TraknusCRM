using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_commercialdetailmechanic
    {
        #region Dependencies
        private DL_equipment _DL_equipment = new DL_equipment();
        private DL_trs_commercialdetail _DL_trs_commercialdetail = new DL_trs_commercialdetail();
        #endregion

        #region Properties
        private string _classname = "DL_trs_commercialdetailmechanic";

        private string _entityname = "trs_commercialdetailmechanic";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Commercial Detail Mechanic";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _trs_nrp = false;
        private string _trs_nrp_value;
        public string trs_nrp
        {
            get { return _trs_nrp ? _trs_nrp_value : string.Empty; }
            set { _trs_nrp = true; _trs_nrp_value = value; }
        }

        private bool _trs_commercialdetailid = false;
        private EntityReference _trs_commercialdetailid_value;
        public Guid trs_commercialdetailid
        {
            get { return _trs_commercialdetailid ? _trs_commercialdetailid_value.Id : Guid.Empty; }
            set { _trs_commercialdetailid = true; _trs_commercialdetailid_value = new EntityReference(_DL_trs_commercialdetail.EntityName, value); }
        }

        private bool _trs_equipmentid = false;
        private EntityReference _trs_equipmentid_value;
        public Guid trs_equipmentid
        {
            get { return _trs_equipmentid ? _trs_equipmentid_value.Id : Guid.Empty; }
            set { _trs_equipmentid = true; _trs_equipmentid_value = new EntityReference(_DL_equipment.EntityName, value); }
        }

        private bool _trs_mechanicrole = false;
        private OptionSetValue _trs_mechanicrole_value;
        public int trs_mechanicrole
        {
            get { return _trs_mechanicrole ? _trs_mechanicrole_value.Value : int.MinValue; }
            set { _trs_mechanicrole = true; _trs_mechanicrole_value = new OptionSetValue(value); }
        }

        private bool _trs_synctime = false;
        private DateTime _trs_synctime_value;
        public DateTime trs_synctime
        {
            get { return _trs_synctime ? _trs_synctime_value.ToLocalTime() : DateTime.MinValue; }
            set { _trs_synctime = true; _trs_synctime_value = value.ToLocalTime(); }
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
                throw new Exception(_classname + ".Select : " + ex.Message);
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
                throw new Exception(_classname + ".Select : " + ex.Message);
            }
        }

        public void Insert(IOrganizationService organizationService)
        {
            try
            {
                if (_trs_nrp)
                {
                    Entity entity = new Entity(_entityname);
                    entity.Attributes["trs_nrp"] = _trs_nrp_value;
                    if (_trs_commercialdetailid) { entity.Attributes["trs_commercialdetailid"] = _trs_commercialdetailid_value; }
                    if (_trs_equipmentid) { entity.Attributes["trs_equipmentid"] = _trs_equipmentid_value; }
                    if (_trs_mechanicrole) { entity.Attributes["trs_mechanicrole"] = _trs_mechanicrole_value; }
                    if (_trs_synctime) { entity.Attributes["trs_synctime"] = _trs_synctime_value; }
                    organizationService.Create(entity);
                }
                else
                {
                    throw new Exception("Primary Key is empty.");
                }
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
                if (_trs_nrp) { entity.Attributes["trs_nrp"] = _trs_nrp_value; }
                if (_trs_commercialdetailid) { entity.Attributes["trs_commercialdetailid"] = _trs_commercialdetailid_value; }
                if (_trs_equipmentid) { entity.Attributes["trs_equipmentid"] = _trs_equipmentid_value; }
                if (_trs_mechanicrole) { entity.Attributes["trs_mechanicrole"] = _trs_mechanicrole_value; }
                if (_trs_synctime) { entity.Attributes["trs_synctime"] = _trs_synctime_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }
    }
}
