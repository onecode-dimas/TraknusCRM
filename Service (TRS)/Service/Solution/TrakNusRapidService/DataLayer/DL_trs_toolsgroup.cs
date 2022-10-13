using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrakNusRapidService.DataLayer
{
    public class DL_trs_toolsgroup
    {
        #region Dependencies
        #endregion

        #region Properties
        private string _classname = "DL_trs_toolsgroup";

        private string _entityname = "trs_toolsgroup";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Tools Group";
        public string DisplayName
        {
            get { return _displayname; }
        }
        #endregion
    }
}
