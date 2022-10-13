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
   public  class DL_task
    {
        #region Properties
        private string _classname = "DL_task";

        private string _entityname = "task";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Commercial Header";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private string _regardingobjectid = null;
        public string regardingobjectid
        {
            get { return _regardingobjectid; }
            set { _regardingobjectid = value; }
        }

        private string _activityid = null;

        public string activityid
        {
            get { return _activityid; }
            set { _activityid = value; }
        }


        private bool _trs_totalrtg = false;
        private decimal _trs_totalrtg_value;
        public decimal trs_totalrtg
        {
            get { return _trs_totalrtg ? _trs_totalrtg_value : 0; }
            set { _trs_totalrtg = true; _trs_totalrtg_value = value; }
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
    }
}
