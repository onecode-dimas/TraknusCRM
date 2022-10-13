using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

using TrakNusRapidService.Helper;

namespace TrakNusRapidServiceWcfService.Helper
{
    public class MWSLog : LogCreator
    {
        #region Constants
        private const string _classname = "MWSLog";
        #endregion

        #region Properties
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
        public string Mode {
            get
            {
                switch(_mode)
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
        public MWSLog()
        {
            GetConfiguration();
            FileLocation += @"\" + _classname;
            FilePrefix = _classname;
        }

        public void Write(string message)
        {
            base.Write(message);
        }
        #endregion
    }
}