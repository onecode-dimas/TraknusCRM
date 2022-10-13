using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhancementCRM.HelperUnit
{
    public class ConnectionString
    {
        public static Boolean IsValidConnectionString(String _connectionstring)
        {
            if (_connectionstring.Replace(" ", "").ToLower().Contains("url=") ||
                _connectionstring.Replace(" ", "").ToLower().Contains("server=") ||
                _connectionstring.Replace(" ", "").ToLower().Contains("serviceuri="))
                return true;

            return false;
        }
    }
}
