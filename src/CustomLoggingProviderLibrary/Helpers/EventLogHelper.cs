using System;
using System.Diagnostics;
using System.IO;
using System.Security;

namespace CustomLoggingProviderLibrary.Helpers
{
    internal class EventLogHelper
    {
        private const string DefaultAppName = "CustomLoggingProvider";
        private const string AdminPrivilegesMessage =
            "Administrator privileges are required to create or modify Event Log sources.\n" +
            "Please run the application as Administrator and try again.";

        /// <summary>
        /// Gets a safe default folder path for storing application log files.
        /// By default, it uses: %LOCALAPPDATA%\CustomLoggingProvider
        /// </summary>
        /// <param name="applicationName">The application name, used to create a subfolder under AppData\Local.</param>
        /// <returns>The full folder path for storing log files.</returns>
        /// <remarks>
        /// Using this path minimizes the risk of permission issues when writing log files,
        /// as it is within the current user's profile directory.
        /// </remarks>
        private static string GetSafeLogFolder(string applicationName)
        {
            string baseFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                applicationName
            );

            Directory.CreateDirectory(baseFolder);
            return baseFolder;
        }

        /// <summary>
        /// Writes a message to a log file using the LoggerFileProvider.
        /// </summary>
        /// <param name="message">The log message to write.</param>
        /// <param name="logName">The event log name associated with the message.</param>
        /// <param name="logFolderPath">The folder path where the log file should be saved.</param>
        private static void LogToFile(string message, string logName, string logFolderPath)
        {
            var loggerFileModel = new LoggerFileModel(message, logName, Environment.MachineName, logFolderPath);
            LoggerFileProvider.WriteToLog(loggerFileModel);
        }

        /// <summary>
        /// Ensures that the specified Windows Event Log source is registered in the given log.
        /// If it doesn't exist, creates it and optionally writes a test log entry.
        /// </summary>
        /// <param name="sourceName">The name of the event source to register.</param>
        /// <param name="logName">The log name to associate with the source (default is "Application").</param>
        /// <param name="writeTestEntry">If true, writes a test message to the event log after creating it.</param>
        /// <param name="writeLogToFileFolderPath">
        /// Optional. Path to save log files.
        /// Defaults to %LOCALAPPDATA%\CustomLoggingProvider if not provided.
        /// </param>
        /// <remarks>
        /// If the source already exists but is linked to a different log,
        /// the method will log a warning to a file instead of overwriting the registration.
        /// </remarks>
        internal void EnsureEventLogSource(
            string sourceName,
            string logName = "Application",
            bool writeTestEntry = true,
            string writeLogToFileFolderPath = ""
        )
        {
            try
            {
                // If no path is provided, default to safe folder in AppData\Local
                if (string.IsNullOrWhiteSpace(writeLogToFileFolderPath))
                {
                    writeLogToFileFolderPath = GetSafeLogFolder(DefaultAppName);
                }

                if (EventLog.SourceExists(sourceName))
                {
                    HandleSourceAlreadyExists(sourceName, logName, writeLogToFileFolderPath);
                    return;
                }

                CreateEventSource(sourceName, logName, writeTestEntry, writeLogToFileFolderPath);
            }
            catch (SecurityException)
            {
                LogToFile(AdminPrivilegesMessage, logName, writeLogToFileFolderPath);
            }
            catch (Exception ex)
            {
                LogToFile($"Unexpected error while checking or creating the event log source: {ex.Message}",
                    logName, writeLogToFileFolderPath);
            }
        }

        /// <summary>
        /// Handles the case where the event source already exists.
        /// If the source is linked to a different log, a warning is logged to a file.
        /// </summary>
        /// <param name="sourceName">The event source name.</param>
        /// <param name="expectedLogName">The log name that the source should be linked to.</param>
        /// <param name="logFolderPath">The folder path where log files should be saved.</param>
        private void HandleSourceAlreadyExists(string sourceName, string expectedLogName, string logFolderPath)
        {
            string currentLog = EventLog.LogNameFromSourceName(sourceName, ".");
            string message;

            if (!string.Equals(currentLog, expectedLogName, StringComparison.OrdinalIgnoreCase))
            {
                message =
                    $"Source '{sourceName}' is already registered in log '{currentLog}', not '{expectedLogName}'.\n" +
                    "You must delete the existing source or use a different source name.";

                LogToFile(message, expectedLogName, logFolderPath);
            }
            else
            {
                message = $"Source '{sourceName}' is already correctly registered in log '{expectedLogName}'.";
            }

            LogToFile(message, expectedLogName, logFolderPath);
        }

        /// <summary>
        /// Creates a new Windows Event Log source and optionally writes a test log entry.
        /// </summary>
        /// <param name="sourceName">The event source name to create.</param>
        /// <param name="logName">The log name to associate with the source.</param>
        /// <param name="writeTestEntry">If true, writes a test message to the event log after creation.</param>
        /// <param name="logFolderPath">The folder path where log files should be saved.</param>
        private void CreateEventSource(string sourceName, string logName, bool writeTestEntry, string logFolderPath)
        {
            EventLog.CreateEventSource(new EventSourceCreationData(sourceName, logName));
            string message;
            message =  $"Source '{sourceName}' created and linked to log '{logName}'.";
            LogToFile(message, logName, logFolderPath);

            if (writeTestEntry)
            {
                EventLog.WriteEntry(sourceName, "Event source created successfully.", EventLogEntryType.Information);
                Console.WriteLine($"Test log entry written to '{logName}' with source '{sourceName}'.");
                LogToFile(message, logName, logFolderPath);
            }
        }
    }
}
