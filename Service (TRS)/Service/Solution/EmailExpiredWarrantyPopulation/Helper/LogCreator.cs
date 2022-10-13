
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SendEmail.Helper
{
    class LogCreator
    {
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

        private Guid _userId = new Guid("00000000-0000-0000-0000-000000000000");
        public Guid UserID
        {
            get { return _userId; }
            set { _userId = value; }
        }

        private bool IsFileExist(string fileName)
        {
            return File.Exists(_fileLocation + @"\" + fileName);
        }

        private string GenerateFileName(DateTime timeStamp)
        {
            return _filePrefix + "_" + timeStamp.ToString("yyyyMMdd") + ".txt";
        }

        public void Write(string message)
        {
            StreamWriter streamWriter;
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
            streamWriter.Write("\t");
            streamWriter.Write(_userId.ToString());
            streamWriter.Write("\t");
            streamWriter.WriteLine(message);
            streamWriter.Close();
            streamWriter.Dispose();
        }
    }
}
