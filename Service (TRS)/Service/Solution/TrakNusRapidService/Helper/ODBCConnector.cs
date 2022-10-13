/****************************************************************************************************
 * Created By   : Thomas Williem Effendi
 * Created Date : 7 May 2014
 * Description  : Class for connect to ODBC
 * Notes        : Default Connection Name "SqlSrvCS"
 *                Default Connection Type = "Sql Server 2012"
 * Compatibility: CRM 2011, CRM 2013
 *****************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.Odbc;

namespace TrakNusRapidService.Helper
{
    public class ODBCConnector
    {
        #region Constants
        private const string _classname = "ODBCConnector";
        #endregion

        #region Properties
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

        private string _connectionType = ODBCConnectorType.SqlServer2012;
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

        #region Privates
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

        #region Connectors
        public String GetConnectionString()
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception(_classname + ".GetConnectionString : " + ex.Message.ToString());
            }
        }

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
                throw new Exception(_classname + ".Connect : " + ex.Message.ToString());
            }
        }

        public void Disconnect()
        {
            try
            {
                _connection.Close();
                _connection.Dispose();
                _connected = false;
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Disconnect : " + ex.Message.ToString());
            }
        }

        public string ConnectionStringExample(string connectionstringType)
        {
            switch (connectionstringType)   //default = 'SQL Server 2012'
            {
                case ODBCConnectorType.MySQL:
                    return "";
                case ODBCConnectorType.SqlServer2008:
                    return "Driver={SQL Server Native Client 10.0};Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;";
                default:
                    return "Driver={SQL Server Native Client 11.0};Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;";
            }
        }
        #endregion

        #region CRUD
        public DataTable Select(OdbcCommand odbcCommand)
        {
            try
            {
                if (!_connected)
                    Connect();
                odbcCommand.Connection = _connection;

                int i;
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
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Select : " + ex.Message.ToString());
            }
        }

        public void Update(string query)
        {
            try
            {
                if (!_connected)
                    Connect();
                OdbcCommand command = new OdbcCommand(query, _connection);
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Failed to Update : {0}.\nError : {1}", query, ex.Message.ToString()));
                }
                Disconnect();
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
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
        #endregion
        #endregion
    }

    public class ODBCConnectorType
    {
        #region Constants
        public const string MySQL = "MySQL";
        public const string SqlServer2008 = "SqlServer2008";
        public const string SqlServer2012 = "SqlServer2012";
        #endregion
    }
}
