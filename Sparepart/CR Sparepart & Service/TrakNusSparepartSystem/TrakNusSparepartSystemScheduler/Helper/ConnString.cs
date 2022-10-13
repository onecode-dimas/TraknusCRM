using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystemScheduler.Helper
{
    public static class ConnString
    {
        public static Boolean IsValidConnectionString(String connectionString)
        {
            if (connectionString.Replace(" ", "").ToLower().Contains("url=") ||
                connectionString.Replace(" ", "").ToLower().Contains("server=") ||
                connectionString.Replace(" ", "").ToLower().Contains("serviceuri="))
                return true;

            return false;
        }
    }
}
