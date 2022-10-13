using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.WorkflowActivity.Helper
{
    public static class Common
    {
        public static int DiffDays(DateTime? d1, DateTime? d2)
        {
            if (d1 == DateTime.MinValue || d2 == DateTime.MinValue)
            {
                return 0;
            }
            {
                DateTime vD2 = (DateTime)d2;
                DateTime vD1 = (DateTime)d1;
                TimeSpan span = vD2.Subtract(vD1);
                return (int)span.TotalDays;
            }

        }
    }
}
