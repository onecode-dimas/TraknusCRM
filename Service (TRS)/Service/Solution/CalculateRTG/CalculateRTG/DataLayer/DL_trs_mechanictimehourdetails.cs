using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Client.Services;

namespace CalculateRTG.DataLayer
{
    public class DL_trs_mechanictimehourdetails
    {
        #region Properties
        private string _classname = "DL_trs_mechanictimehourdetails";

        private string _entityname = "trs_mechanictimehourdetails";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Mechanic Time And Hour Details";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private string _trs_name_value;
        public string trs_name
        {
            get { return _trs_name_value; }
            set { _trs_name_value = value; }
        }

        private decimal _trs_actualtimehour_value;
        public decimal trs_actualtimehour
        {
            get { return _trs_actualtimehour_value; }
            set { _trs_actualtimehour_value = value; }
        }

        private decimal _trs_repairtimeguide_value;
        public decimal trs_repairtimeguide
        {
            get { return _trs_repairtimeguide_value; }
            set { _trs_repairtimeguide_value = value; }
        }

        private Guid _trs_mechanicid_value;
        public Guid trs_mechanicid
        {
            get { return _trs_mechanicid_value; }
            set { _trs_mechanicid_value = value; }
        }

        private Guid _trs_workordernumber_value;
        public Guid trs_workordernumber
        {
            get { return _trs_workordernumber_value; }
            set { _trs_workordernumber_value = value; }
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
                throw new Exception(_classname + ".Select  : " + ex.Message.ToString());
            }
        }

        public void Insert(IOrganizationService organizationService)
        {
            try
            {
               
                Entity entity = new Entity(_entityname);
                entity.Attributes["trs_mechanicid"] = new EntityReference("equipment", _trs_mechanicid_value);
                entity.Attributes["trs_workordernumber"] = new EntityReference("serviceappointment", _trs_workordernumber_value);
                entity.Attributes["trs_actualtimehour"] = _trs_actualtimehour_value;
                entity.Attributes["trs_repairtimeguide"] = _trs_repairtimeguide_value;
                entity.Attributes["trs_name"] = _trs_name_value;
                organizationService.Create(entity);
           
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Insert : " + ex.Message.ToString());
            }
        }
    }
}
