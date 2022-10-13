/****************************************************************************************************
 * Created By   : Thomas Williem Effendi
 * Created Date : 25 February 2015
 * Description  : Class for read log file
 * Compatibility: All Windows Operating System
 *****************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace TrakNusRapidService.Helper
{
    public class LogReader
    {
        #region Constants
        private const string _classname = "LogReader";
        #endregion

        #region Properties
        private StreamWriter streamWriter;

        private string _fileLocation = @"C:\Tectura\Log\MWSLog\";
        public string FileLocation
        {
            get { return _fileLocation; }
            set { _fileLocation = value; }
        }

        private string _filePrefix = "MWSLog";
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
        #endregion

        #region Privates
        private bool IsFileExist(string fileName)
        {
            return File.Exists(_fileLocation + @"\" + fileName);
        }
        #endregion

        #region Publics
        public DataTable GetListFile()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("fileName");
            dt.Columns.Add("name");
            string fileName = string.Empty;

            string[] filePaths = Directory.GetFiles(_fileLocation);
            foreach (string filePath in filePaths)
            {
                fileName = filePath.Replace(_fileLocation, "");
                dt.Rows.Add(fileName, fileName.Replace(_filePrefix + "_", "").Replace(".txt", ""));
            }
            return dt;
        }

        public string Read(string fileName, string functionName)
        {
            string result = string.Empty;
            string content = string.Empty;
            string[] data;

            using (StreamReader streamReader = new StreamReader(_fileLocation + fileName))
            {
                result = streamReader.ReadToEnd();
            }

            /*
            if (result != string.Empty)
            {
                content = "<table style=\"border:1px solid\">"; 
                string[] lines = result.Replace(System.Environment.NewLine, "~").Split('~');
                foreach (string line in lines)
                {
                    data = line.Split(_columnsSeparator.ToCharArray()[0]);
                    if (data.Count() > 1 && data[1] == functionName)
                    {
                        content += "<tr style=\"border:1px solid\">";
                        for (int i = 0; i < 10; i++)
                        {
                            if (i < data.Count())
                            {
                                content += "<td style=\"border:1px solid\">" + data[i] + "</td>";
                            }
                            else
                            {
                                content += "<td style=\"border:1px solid\">&nbsp;</td>";
                            }
                        }
                        content += "</tr>";
                    }
                }
                content += "</table>";
            }*/

            content = result.Replace(System.Environment.NewLine, "<br />");
            return content;
        }
        #endregion
    }
}
