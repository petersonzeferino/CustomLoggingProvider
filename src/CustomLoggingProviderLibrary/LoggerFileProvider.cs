using System;
using System.IO;

namespace CustomLoggingProviderLibrary
{

    internal class LoggerFileProvider
    {
        /// <summary>
        /// Writes a log entry or, in case of an error, writes to the error log file.
        /// </summary>
        /// <param name="loggerFile">Object containing the information required to write the log.</param>
        internal static void WriteToLog(LoggerFileModel loggerFile)
        {
            try
            {
                string logFilePath = GetLogFilePath(loggerFile, "Log");
                BackUpLogFile(logFilePath, loggerFile);

                AppendLogEntry(logFilePath, loggerFile.Message, loggerFile.MachineIdentifier);
            }
            catch (Exception ex)
            {
                string errorLogFilePath = GetLogFilePath(loggerFile, "ErrorLog");

                string errorMessage = $"{loggerFile.Message}\r\n{new string('-', 119)}\r\n{ex.Message}";
                AppendLogEntry(errorLogFilePath, errorMessage, loggerFile.MachineIdentifier);
            }
        }

        /// <summary>
        /// Builds the complete file path for the log file based on the application name and the provided suffix.
        /// </summary>
        /// <param name="loggerFile">Object containing log information.</param>
        /// <param name="suffix">Suffix to be added to the file name (e.g., "Log" or "ErrorLog").</param>
        /// <returns>Complete file path for the log file.</returns>
        private static string GetLogFilePath(LoggerFileModel loggerFile, string suffix)
        {
            return Path.Combine(loggerFile.WriteLogToFileFolderPath, $"{loggerFile.ApplicationName}{suffix}.txt");
        }

        /// <summary>
        /// Appends a log entry to the specified file, including timestamp, machine identifier (optional), and the message.
        /// </summary>
        /// <param name="filePath">Path to the log file.</param>
        /// <param name="message">Log message to be recorded.</param>
        /// <param name="machineId">Optional machine identifier.</param>
        private static void AppendLogEntry(string filePath, string message, string machineId = null)
        {
            var content = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss}\t";

            if (!string.IsNullOrEmpty(machineId))
                content += $"{machineId}\t";

            content += $"{message}\r\n";
            content += $"{new string('-', 119)}\r\n";

            File.AppendAllText(filePath, content);
        }


        /// <summary>
        /// Creates a backup of the log file if it was last written on a different day.
        /// The backup file is named with the last write date as a prefix, 
        /// and the original file is deleted after backup.
        /// </summary>
        /// <param name="logFilePath">The full path to the current log file.</param>
        /// <param name="loggerFile">An instance of <see cref="LoggerFileModel"/> used to update the backup status.</param>
        private static void BackUpLogFile(string logFilePath, LoggerFileModel loggerFile)
        {
            if (!File.Exists(logFilePath))
                return;

            var fileInfo = new FileInfo(logFilePath);
            if (fileInfo.LastWriteTime.Date >= DateTime.Today)
                return;

            string backupFileName = $"{fileInfo.LastWriteTime:yyyyMMdd}_{fileInfo.Name}";
            string backupFilePath = Path.Combine(fileInfo.DirectoryName ?? string.Empty, backupFileName);

            File.Copy(logFilePath, backupFilePath);
            loggerFile.SetBackUpLogFile();
            File.Delete(logFilePath);
        }

        /// <summary>
        /// Clears all content from the specified log file by overwriting it with an empty string.
        /// </summary>
        /// <param name="logFilePath">The full path to the log file to be cleared.</param>
        private static void ClearFileContent(string logFilePath)
        {
            File.WriteAllText(logFilePath, string.Empty);
        }
    }
}
