using System;
using System.Diagnostics;
using System.IO;
using System.Security;

namespace CustomLoggingProviderLibrary.Helpers
{
    /// <summary>
    /// Helper class for managing Windows Event Log sources and writing logs to both the Event Log and files.
    /// </summary>
    internal class EventLogHelper
    {
        private const string DefaultAppName = "CustomLoggingProvider";
        private const string AdminPrivilegesMessage =
            "Administrator privileges are required to create or modify Event Log sources.\n" +
            "Please run the application as Administrator and try again.";

        /// <summary>
        /// Formats a log message, optionally prefixing it with "[TEST]".
        /// </summary>
        /// <param name="message">The original message.</param>
        /// <param name="isTest">Whether the message is for testing purposes.</param>
        /// <returns>The formatted log message.</returns>
        private static string FormatLogMessage(string message, bool isTest = false)
        {
            return isTest ? $"[TEST] {message}" : message;
        }

        /// <summary>
        /// Gets a safe default folder path for storing application log files.
        /// By default, it uses: %LOCALAPPDATA%\CustomLoggingProvider.
        /// </summary>
        /// <param name="applicationName">Optional application name for creating a subfolder.</param>
        /// <returns>The full folder path for storing log files.</returns>
        private static string GetSafeLogFolder(string applicationName = DefaultAppName)
        {
            string folderPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                applicationName
            );
            Directory.CreateDirectory(folderPath);
            return folderPath;
        }

        /// <summary>
        /// Writes a message to a log file using the LoggerFileProvider.
        /// </summary>
        /// <param name="message">The log message to write.</param>
        /// <param name="logName">The event log name associated with the message.</param>
        /// <param name="logFolderPath">The folder path where the log file should be saved.</param>
        private static void LogToFile(string message, string logName, string logFolderPath)
        {
            var logEntry = new LoggerFileModel(message, logName, Environment.MachineName, logFolderPath);
            LoggerFileProvider.WriteToLog(logEntry);
        }

        /// <summary>
        /// Ensures that the specified Windows Event Log source is registered.
        /// Creates it if missing, and optionally writes a test entry.
        /// </summary>
        /// <param name="sourceName">The name of the event source to register.</param>
        /// <param name="logName">The log name to associate with the source (default is "Application").</param>
        /// <param name="writeTestEntry">If true, writes a test message after creation.</param>
        /// <param name="logFolderPath">Optional path to save log files. Defaults to %LOCALAPPDATA%\CustomLoggingProvider.</param>
        internal void EnsureEventLogSource(
            string sourceName,
            string logName = "Application",
            bool writeTestEntry = true,
            string logFolderPath = "")
        {
            logFolderPath = string.IsNullOrWhiteSpace(logFolderPath)
                ? GetSafeLogFolder()
                : logFolderPath;
            try
            {
                if (EventLog.SourceExists(sourceName))
                {
                    HandleExistingSource(sourceName, logName, logFolderPath, writeTestEntry);
                }
                else
                {
                    CreateEventSource(sourceName, logName, writeTestEntry, logFolderPath);
                }
            }
            catch (SecurityException)
            {
                LogToFile(AdminPrivilegesMessage, logName, logFolderPath);
            }
            catch (Exception ex)
            {
                LogToFile(FormatLogMessage($"Unexpected error: {ex.Message}", writeTestEntry), logName, logFolderPath);
            }
        }

        /// <summary>
        /// Handles the case when an event source already exists.
        /// Logs a warning if it's linked to a different log name.
        /// </summary>
        /// <param name="sourceName">The existing event source name.</param>
        /// <param name="expectedLogName">The expected log name for the source.</param>
        /// <param name="logFolderPath">The folder path for file logs.</param>
        /// <param name="isTest">Whether to prefix the message with "[TEST]".</param>
        private void HandleExistingSource(string sourceName, string expectedLogName, string logFolderPath, bool isTest)
        {
            string currentLogName = EventLog.LogNameFromSourceName(sourceName, ".");
            string message = currentLogName.Equals(expectedLogName, StringComparison.OrdinalIgnoreCase)
                ? $"Source '{sourceName}' is already correctly registered in log '{expectedLogName}'."
                : $"Source '{sourceName}' exists in log '{currentLogName}', not '{expectedLogName}'. " +
                  "Delete the existing source or use a different name.";

            LogToFile(FormatLogMessage(message, isTest), expectedLogName, logFolderPath);
        }

        /// <summary>
        /// Creates a new event source and optionally writes a test log entry.
        /// </summary>
        /// <param name="sourceName">The name of the new event source.</param>
        /// <param name="logName">The log name to associate with the source.</param>
        /// <param name="writeTestEntry">If true, writes a test entry after creation.</param>
        /// <param name="logFolderPath">The folder path for file logs.</param>
        private void CreateEventSource(string sourceName, string logName, bool writeTestEntry, string logFolderPath)
        {
            EventLog.CreateEventSource(new EventSourceCreationData(sourceName, logName));

            string creationMessage = $"Source '{sourceName}' created and linked to log '{logName}'.";
            LogToFile(FormatLogMessage(creationMessage, writeTestEntry), logName, logFolderPath);

            if (writeTestEntry)
            {
                EventLog.WriteEntry(sourceName, "Event source created successfully.", EventLogEntryType.Information);
                Console.WriteLine(FormatLogMessage(
                    $"Test log entry written to '{logName}' with source '{sourceName}'.", true
                ));
            }
        }
    }
}
