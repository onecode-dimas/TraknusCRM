using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.Helper
{
    public static class DataRowExtensions
    {
        public static T FieldOrDefault<T>(this DataRow _datarow, string _columnname)
        {
            return _datarow.IsNull(_columnname) ? default(T) : _datarow.Field<T>(_columnname);
        }
    }
}
