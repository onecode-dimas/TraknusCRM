using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusSparepartSystem.DataLayer
{
    public class DL_UOM
    {
        #region Properties
        private string _classname = "DL_UOM";

        private string _entityname = "trs_unitofmeasurement";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Unit of Measurement";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _name = false;
        private string _name_value;
        public string name
        {
            get { return _name ? _name_value : null; }
            set { _name = true; _name_value = value; }
        }
        #endregion
    }
}
