using Microsoft.Win32;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.WorkflowActivity.Business_Layer
{
    public class GetStoredProcedure
    {
        private string _dbsource = "DBSource";

        public DataTable Connect(string _storedprocedurename, List<SqlParameter> _sqlparameters, bool _returnvalue)
        {
            DataTable _datatable = new DataTable();

            //string _connectionstring = "Data Source=10.0.10.43;User Id=sa; Password=pass@word2; Initial Catalog=CRMTrakNus_MSCRM";
            string _connectionstring = string.Empty;

            RegistryKey _regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\MSCRM", false);

            if (_regkey != null)
                _connectionstring = (string)_regkey.GetValue(_dbsource).ToString();
            else
                throw new InvalidWorkflowException("SqlConnection FAILED ! Connection String NOT Found !");

            using (SqlConnection _sqlconnection = new SqlConnection(_connectionstring))
            {
                if (_sqlconnection != null)
                {
                    using (SqlCommand _sqlcommand = new SqlCommand(_storedprocedurename, _sqlconnection))
                    {
                        _sqlcommand.CommandType = CommandType.StoredProcedure;
                        _sqlcommand.CommandTimeout = 36000;
                        _sqlcommand.Parameters.AddRange(_sqlparameters.ToArray());

                        if (_returnvalue)
                        {
                            SqlDataAdapter _sqldataadapter = new SqlDataAdapter(_sqlcommand);
                            _sqldataadapter.Fill(_datatable);
                        }
                        else
                        {
                            _sqlconnection.Open();
                            _sqlcommand.ExecuteNonQuery();
                        }   
                    }
                }
                else
                    throw new InvalidWorkflowException("SqlConnection FAILED !");
            }

            if (_datatable.Rows.Count > 0)
                return _datatable;
            else
                return null;
        }
    }
}
