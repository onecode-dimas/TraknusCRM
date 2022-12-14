using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Win32;

namespace EnhancementCRM.HelperUnit
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
        private string LogTable = "LogSAPUnitTableName"; //Table Name : Log_SAP_Unit
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

        private string _columnsSeparator = "; ";
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
                Mode = "Debug";
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
                RegistryKey regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\MSCRM", false);

                if (regkey != null)
                {
                    SqlCredential = (string)regkey.GetValue(LogCredential).ToString();
                    TableName = (string)regkey.GetValue(LogTable).ToString();
                }
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
