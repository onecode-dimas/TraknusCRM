using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

using System.IO;
using System.Web.Configuration;
using Microsoft.Win32;
using System.Data.SqlClient;
using System.Data;

namespace TrakNusSparepartSystem.WcfService.Helper
{
    public class MWSLog
    {
        #region Constants
        private const string _classname = "MWSLog";
        #endregion

        #region Properties
        private string SqlCredential = string.Empty;
        private string TableName = string.Empty;
        private string LogCredential = "LogCredential";
        private string LogTable = "LogTableName";
        public enum LogType
        {
            Error,
            Information
        };
        public enum Source
        {
            Inbound,
            Outbound
        };

        private string _columnsSeparator = ";";
        public string ColumnsSeparator
        {
            get { return _columnsSeparator; }
            set { _columnsSeparator = value; }
        }

        private bool _verbose;
        public bool Verbose
        {
            get { return _verbose; }
            set { _verbose = value; }
        }

        private enum LogMode : int { Off, Debug, Verbose }
        private int _mode;
        public string Mode
        {
            get
            {
                switch (_mode)
                {
                    case 1:
                        return "Debug";
                        break;
                    case 2:
                        return "Verbose";
                        break;
                    default:
                        return "Off";
                        break;
                }
            }
            set
            {
                switch (value.ToLower())
                {
                    case "debug":
                        _mode = 1;
                        break;
                    case "verbose":
                        _mode = 2;
                        _verbose = true;
                        break;
                    default:
                        _mode = 0;
                        break;
                }
            }
        }

        private string _organization = string.Empty;
        public string Organization
        {
            get { return _organization; }
            set { _organization = value; }
        }
        #endregion

        #region Privates
        private void GetConfiguration()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                if (appSettings.Count > 0)
                {
                    foreach (var key in appSettings.AllKeys)
                    {
                        switch (key.ToLower())
                        {
                            case "log mode":
                                Mode = appSettings[key];
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".GetConfiguration : " + ex.Message.ToString());
            }
        }
        #endregion

        #region Publics
        public void GetSqlInformation()
        {
            try
            {
                SqlCredential = ConfigurationManager.AppSettings[LogCredential].ToString();
                TableName = ConfigurationManager.AppSettings[LogTable].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".GetSqlInformation : " + ex.Message.ToString());
            }
        }

        public MWSLog()
        {
            GetConfiguration();
            GetSqlInformation();
        }

        public void Write(string methodName, string description, LogType logType, Source source)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                GetSqlInformation();
                conn.ConnectionString = SqlCredential;
                conn.Open();

                string smt = "INSERT INTO dbo." + TableName + "(MethodName,CreatedOn,Description,LogType,Source) VALUES(@MethodName,@CreatedOn,@Description,@LogType,@Source)";

                SqlCommand cmd = new SqlCommand(smt, conn);
                cmd.Parameters.Add("@MethodName", SqlDbType.VarChar, 50);
                cmd.Parameters.Add("@CreatedOn", SqlDbType.DateTime);
                cmd.Parameters.Add("@Description", SqlDbType.VarChar);
                cmd.Parameters.Add("@LogType", SqlDbType.VarChar, 50);
                cmd.Parameters.Add("@Source", SqlDbType.VarChar, 50);

                cmd.Parameters["@MethodName"].Value = methodName;
                cmd.Parameters["@CreatedOn"].Value = DateTime.Now;
                cmd.Parameters["@Description"].Value = description;
                if (logType == LogType.Error)
                {
                    cmd.Parameters["@LogType"].Value = "Error";
                }
                else if (logType == LogType.Information)
                {
                    cmd.Parameters["@LogType"].Value = "Information";
                }
                if (source == Source.Inbound)
                {
                    cmd.Parameters["@Source"].Value = "Inbound";
                }
                else if (source == Source.Outbound)
                {
                    cmd.Parameters["@Source"].Value = "Outbound";
                }

                cmd.ExecuteNonQuery();

                conn.Close();
                conn.Dispose();
            };
        }
        #endregion
    }
}