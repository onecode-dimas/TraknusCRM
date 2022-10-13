using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystemScheduler.Helper
{
    public static class Common
    {
        public static int DiffDays(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                return 0;
            
            TimeSpan span = endDate.Date - startDate.Date;
            return (int)Math.Round(span.TotalDays);
        }
    }
}
