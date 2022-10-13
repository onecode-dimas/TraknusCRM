/****************************************************************************************************
 * Created By   : Thomas Williem Effendi
 * Created Date : 7 May 2014
 * Description  : Class for connect to ODBC
 * Notes        : Default Connection Name "SqlSrvCS"
 *                Default Connection Type = "Sql Server 2012"
 * Compatibility: CRM 2011
 *****************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.Odbc;

namespace SendEmail.Helper
{
    class ODBCConnector
    {
        #region Property
        private bool _connected = false;

        private OdbcConnection _connection = null;
        public OdbcConnection Connection
        {
            get { return _connection; }
        }

        private string _connectionName = "SqlSrvCS";
        public string ConnectionName
        {
            get { return _connectionName; }
            set { _connectionName = value; }
        }

        private string _connectionType = "SqlServer2012";
        public string ConnectionType
        {
            get { return _connectionType; }
            set { _connectionType = value; }
        }

        private string _connectionString = null;
        public string ConnectionString
        {
            get { return _connectionString; }
        }

        private string _query = null;
        public string Query
        {
            get { return _query; }
        }
        #endregion

        #region Connection String
        public String GetConnectionString()
        {
            string connectionString = string.Empty;

            for (int i = 0; i < ConfigurationManager.ConnectionStrings.Count; i++)
            {
                if (ConfigurationManager.ConnectionStrings[i].Name == _connectionName)
                {
                    connectionString = ConfigurationManager.ConnectionStrings[i].ConnectionString;
                    break;
                }
            }

            if (connectionString == string.Empty)
                throw new Exception("Can not found Connection String.");
            else
                if (IsValidConnectionString(connectionString))
                    return connectionString;
                else
                    throw new Exception("Invalid Connection String. Please change it to : " + ConnectionStringExample(_connectionType));
        }

        private static Boolean IsValidConnectionString(string connectionString)
        {
            if (connectionString.ToLower().Contains("server=") &&
                connectionString.ToLower().Contains("database=") &&
                (connectionString.ToLower().Contains("trusted_connection=") ||
                    (connectionString.ToLower().Contains("uid=") &&
                        connectionString.ToLower().Contains("pwd="))
                )
            )
                return true;

            return false;
        }
        #endregion

        public void Connect()
        {
            _connectionString = GetConnectionString();
            try
            {
                _connection = new OdbcConnection(_connectionString);
                _connection.Open();
                _connected = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to connect to data source.");
            }
        }

        public void Disconnect()
        {
            _connection.Close();
            _connection.Dispose();
            _connected = false;
        }

        public DataTable Select(OdbcCommand odbcCommand)
        {
            if (!_connected)
                Connect();
            odbcCommand.Connection = _connection;

            int i, j;
            DataTable dt = new DataTable();
            OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader();
            if (odbcDataReader.HasRows)
            {
                for (i = 0; i < odbcDataReader.FieldCount; i++)
                {
                    dt.Columns.Add(odbcDataReader.GetName(i)); //, odbcDataReader[i].GetType());
                }

                while (odbcDataReader.Read())
                {
                    DataRow dr = dt.Rows.Add();
                    for (i = 0; i < odbcDataReader.FieldCount; i++)
                    {
                        dr[i] = odbcDataReader[i];
                    }
                }
            }

            Disconnect();
            return dt;
        }

        #region On Progress
        //public void Insert(string query)
        //{
        //    Connect();
        //    OdbcCommand command = new OdbcCommand(query, _connection);
        //    try
        //    {
        //        command.ExecuteNonQuery();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(string.Format("Failed to Insert : {0}", query));
        //    }
        //    Disconnect();
        //}

        //public void Delete(string query)
        //{
        //    Connect();
        //    OdbcCommand command = new OdbcCommand(query, _connection);
        //    try
        //    {
        //        command.ExecuteNonQuery();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(string.Format("Failed to Delete : {0}", query));
        //    }
        //    Disconnect();
        //}

        //public void Update(string query)
        //{
        //    Connect();
        //    OdbcCommand command = new OdbcCommand(query, _connection);
        //    try
        //    {
        //        command.ExecuteNonQuery();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(string.Format("Failed to Delete : {0}", query));
        //    }
        //    Disconnect();
        //}
        #endregion

        public string ConnectionStringExample(string connectionstringType)
        {
            switch (connectionstringType)   //default = 'SQL Server 2012'
            {
                case "MySQL":
                    return "";
                case "SqlServer2008":
                    return "Driver={SQL Server Native Client 10.0};Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;";
                default:
                    return "Driver={SQL Server Native Client 11.0};Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;";
            }
        }
    }
}
