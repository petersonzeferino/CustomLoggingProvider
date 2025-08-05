using System;
using System.IO;

namespace CustomLoggingProviderLibrary
{

    internal class LoggerFileProvider
    {
        /// <summary>
        /// Writes a log message to a text file. If the file already exists and was last written on a previous day,
        /// it creates a backup of the file before writing. In case of an error, logs the exception to a separate error file.
        /// </summary>
        /// <param name="loggerFile">An instance of <see cref="LoggerFileModel"/> containing the log message and configuration.</param>
        internal static void WriteToLog(LoggerFileModel loggerFile)
        {
            try
            {
                string logFilePath = Path.Combine(loggerFile.WriteLogToFileFolderPath, $"{loggerFile.ApplicationName}Log.txt");
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
                string logFilePath = Path.Combine(loggerFile.WriteLogToFileFolderPath, $"{loggerFile.ApplicationName}ErrorLog.txt");

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

        /// <summary>
        /// Creates a backup of the log file if it was last written on a different day.
        /// The backup file is named with the date as a prefix and the original file is deleted after backup.
        /// </summary>
        /// <param name="logFilePath">The full path to the current log file.</param>
        /// <param name="loggerFile">An instance of <see cref="LoggerFileModel"/> used to update the backup status.</param>
        private static void BackUpLogFile(string logFilePath, LoggerFileModel loggerFile)
        {
            if (File.Exists(logFilePath))
            {
                FileInfo fileLogInfo = new FileInfo(logFilePath);
                if (fileLogInfo.LastWriteTime.Date < DateTime.Today.Date)
                {
                    string newFileName = Path.Combine(fileLogInfo.DirectoryName, $"{fileLogInfo.LastWriteTime:yyyyMMdd}_{Path.GetFileName(logFilePath)}");
                    File.Copy(logFilePath, newFileName);
                    loggerFile.SetBackUpLogFile();
                    File.Delete(logFilePath);
                }
            }
        }

        /// <summary>
        /// Clears the content of the specified log file by overwriting it with an empty string.
        /// </summary>
        /// <param name="logFilePath">The full path to the log file to be cleared.</param>
        private static void ClearFileContent(string logFilePath)
        {
            File.WriteAllText(logFilePath, string.Empty);
        }
    }
}
