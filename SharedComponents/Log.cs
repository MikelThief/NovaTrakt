using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace NovaTrakt.SharedComponents
{
    public class Log
    {
        public Log ()
        {
            filePath = @"C:\Temp\";
            fileName = "NovaTrakt.log";
            logFileWithPath = filePath + fileName;
        }

        private string _log = "";
        private string filePath;
        private string fileName;
        private string logFileWithPath;

        public string LogPath()
        {
            return logFileWithPath;
        }

        public long FileSizeInBytes
        {
            get
            {
                if (File.Exists(logFileWithPath))
                    return new FileInfo(logFileWithPath).Length;
                else
                    return 0;
            }
        }
        private double fileSizeInMegaBytes
        {
            get { return (FileSizeInBytes / 1024f) / 1024f; }
        }

        public bool LargeFile()
        {
            if (fileSizeInMegaBytes >= 5)
                return true;

            return false;
        }

        public void TidyFile()
        {
            if (fileSizeInMegaBytes >= 5)
            {
                // Calculate difference between fileSizeInBytes and 5MB
                double diff = (FileSizeInBytes - ((5 * 1024) * 1024));

                // Read the first lines to a string, keep going until the string size in bytes is greater than the difference above
                StreamReader sr = new StreamReader(logFileWithPath);
                string dmp = "";
                // Build in a 2MB buffer zone
                double bufferBytes = (2 * 1024f) * 1024f;
                while (ASCIIEncoding.Unicode.GetByteCount(dmp) < (diff + bufferBytes))
                {
                    dmp += sr.ReadLine();
                }

                // Read the rest of the file
                string remainingContents = sr.ReadToEnd();
                sr.Close();
                // Write remainingContents back to the file
                using (StreamWriter sw = new StreamWriter(logFileWithPath, false))
                {
                    sw.Write(remainingContents);
                }
            }
        }

        public void WriteLine (string _type, string _message)
        {
            _log += DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",
                                            CultureInfo.InvariantCulture) + " \t" + DateTimeOffset.Now.Offset.Hours;
            _log += " \t " + _type + ": \t";
            _log += _message + Environment.NewLine;

            WriteToFile();
        }

        public void WriteVerboseLine(string _message)
        {
            if (AppSettings.getBool("VerboseLogging", false))
            {
                _log += DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",
                                                CultureInfo.InvariantCulture) + " \t" + DateTimeOffset.Now.Offset.Hours;
                _log += " \tVerbose: \t";
                _log += _message + Environment.NewLine;

                WriteToFile();
            }
        }

        public void WriteGPSLine(string _message)
        {
            if (AppSettings.getBool("VerboseLogging", false))
            {
                _log += DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",
                                               CultureInfo.InvariantCulture) + " \t" + DateTimeOffset.Now.Offset.Hours;
                _log += " \tGPS Data: \t";
                _log += _message + Environment.NewLine;

                WriteToFile();
            }
        }

        public void WriteToFile()
        {
            // Check the file exists, and create it if not
            if (!File.Exists(logFileWithPath))
                ClearLog();

            try
            {
                File.AppendAllText(logFileWithPath, _log);
                _log = "";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void WriteEmptyLine()
        {
            // Check the file exists, and create it if not
            if (!File.Exists(logFileWithPath))
                ClearLog();

            string logLine = Environment.NewLine;

            try
            {
                File.AppendAllText(logFileWithPath, logLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void ClearLog()
        {
            // Delete the file if it exists.
            if (File.Exists(logFileWithPath))
                File.Delete(logFileWithPath);

            // Check the %APPDATA%\NovaTrakt Folder Exists
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            // Create the file.
            using (FileStream fs = File.Create(logFileWithPath))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes("");
                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }
        }
    }
}
