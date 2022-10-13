/****************************************************************************************************
 * Created By   : Thomas Williem Effendi
 * Created Date : 6 May 2014
 * Description  : Class for create log file
 * Compatibility: All Windows Operating System
 *****************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;

namespace TrakNusRapidService.Helper
{
    public class LogCreator
    {
        #region Constants
        private const string _classname = "LogCreator";
        #endregion

        #region Properties
        private StreamWriter streamWriter;

        private string _fileLocation = @"C:\Tectura\Log";
        public string FileLocation
        {
            get { return _fileLocation; }
            set { _fileLocation = value; }
        }

        private string _filePrefix = "log";
        public string FilePrefix
        {
            get { return _filePrefix; }
            set { _filePrefix = value; }
        }

        private string _columnsSeparator = ";";
        public string ColumnsSeparator
        {
            get { return _columnsSeparator; }
            set { _columnsSeparator = value; }
        }

        private string _organization = string.Empty;
        public string Organization
        {
            get { return _organization; }
            set { _organization = value; }
        }
        #endregion

        #region Privates
        private bool IsFileExist(string fileName)
        {
            return File.Exists(_fileLocation + @"\" + fileName);
        }

        private string GenerateFileName(DateTime timeStamp)
        {
            return _filePrefix + "_" + timeStamp.ToString("yyyyMMdd") + ".txt";
        }

        private void GetLogConfiguration()
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
                            case "organization":
                                Organization = appSettings[key];
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
        public LogCreator(string organization = "")
        {
            if (organization != string.Empty)
            {
                Organization = organization;
            }
            else
            {
                GetLogConfiguration();
            }
            FileLocation += @"\" + _organization;
        }

        public void Write(string message)
        {
            DateTime timeStamp = DateTime.Now;
            string fileName = GenerateFileName(timeStamp);

            if (IsFileExist(fileName))
            {
                streamWriter = File.AppendText(_fileLocation + @"\" + fileName);
            }
            else
            {
                Directory.CreateDirectory(_fileLocation);
                streamWriter = new StreamWriter(_fileLocation + @"\" + fileName);
            }

            streamWriter.Write(timeStamp.ToString("yyyy/MM/dd HH:mm:ss.FFFFFFF"));
            streamWriter.Write(_columnsSeparator);
            streamWriter.WriteLine(message);
            streamWriter.Close();
            streamWriter.Dispose();
        }
        #endregion
    }
}
