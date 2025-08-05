using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomLoggingProviderLibrary
{

    internal class LoggerFileProvider
    {
        /// <summary>
        /// Write log to file
        /// </summary>
        /// <param name="loggerFile"></param>
        internal static void WriteToLog(LoggerFileModel loggerFile)
        {
            try
            {
                string logFilePath = $"{loggerFile.WriteLogToFileFolderPath}{loggerFile.ApplicationName}Log.txt";
                BackUpLogFile(logFilePath, loggerFile);

                string content = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss")+ "\t";
                if (!string.IsNullOrEmpty(loggerFile.MachineIdentifier))
                {
                    content += loggerFile.MachineIdentifier + "\t";
                }
                content += loggerFile.Message + "\r\n";
                content += "-----------------------------------------------------------------------------------------------------------------------\r\n";
                File.AppendAllText(logFilePath, content);
            }
            catch (Exception ex)
            {
                string logFilePath = $"{loggerFile.WriteLogToFileFolderPath}{loggerFile.ApplicationName}ErrorLog.txt";

                string content = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss")+ "\t";
                if (!string.IsNullOrEmpty(loggerFile.MachineIdentifier))
                {
                    content += loggerFile.MachineIdentifier + "\t";
                }
                content += loggerFile.Message + "\r\n";
                content += "-----------------------------------------------------------------------------------------------------------------------\r\n";
                content += ex.Message + "\r\n";
                content += "-----------------------------------------------------------------------------------------------------------------------\r\n";
                File.AppendAllText(logFilePath, content);
            }
        }

        private static void BackUpLogFile(string logFilePath, LoggerFileModel loggerFile)
        {
            if (File.Exists(logFilePath))
            {
                FileInfo fileLogInfo = new FileInfo(logFilePath);
                if (fileLogInfo.LastWriteTime.Date < DateTime.Today.Date)
                {
                    string newFileName = fileLogInfo.Directory + "\\" + fileLogInfo.LastWriteTime.Date.ToShortDateString().Replace("/", "") + "_" + Path.GetFileName(logFilePath);
                    File.Copy(logFilePath, newFileName);
                    loggerFile.SetBackUpLogFile();
                    File.Delete(logFilePath);
                }
            }
        }

        private static void ClearFileContent(string logFilePath)
        {
            File.WriteAllText(logFilePath, string.Empty);
        }
    }
}
