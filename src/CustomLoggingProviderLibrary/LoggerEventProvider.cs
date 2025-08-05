using CustomLoggingProviderLibrary.Helpers;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CustomLoggingProviderLibrary
{
    public class LoggerEventProvider
    {
        private readonly ILogger _logger;
        private readonly LogLevel _logLevel;
        private readonly string _applicationName;
        private readonly string _logName;
        private readonly bool _enableWriteLogToFile;
        private readonly string _callerName;
        private readonly EventLogHelper _eventLogHelper;
        private readonly string _writeLogToFileFolderPath;

        /// <summary>
        /// Initializes the logger provider for the application.
        /// Should be called once at the start of the application to configure logging output to Console, Event Viewer, and optionally to a file.
        /// </summary>
        /// <param name="applicationName">Name of the application used as the source identifier in the Event Viewer.</param>
        /// <param name="logName">Name of the log in the Event Viewer (e.g., "Application").</param>
        /// <param name="logMinimumLevel">Minimum log level to be captured (e.g., Information, Warning, Error).</param>
        /// <param name="enableWriteLogToFile">Indicates whether log messages should also be written to a local file.</param>
        /// <param name="writeLogToFileFolderPath">Optional folder path where log files will be saved if file logging is enabled.</param>
        public LoggerEventProvider(string applicationName, string logName, LogLevel logMinimumLevel, bool enableWriteLogToFile = false, string writeLogToFileFolderPath = "")
        {
            _applicationName = applicationName;
            _logName = logName;
            _logLevel = logMinimumLevel;
            _enableWriteLogToFile = enableWriteLogToFile;
            _callerName = GetCallerClassName();
            _writeLogToFileFolderPath = writeLogToFileFolderPath;

            _logger = LoggerProviderFactory.GetLogger<LoggerEventProvider>(_applicationName, _logName, _logLevel);

            _eventLogHelper = new EventLogHelper();
            _eventLogHelper.EnsureEventLogSource(_applicationName, _logName, writeTestEntry: true);
        }

        /// <summary>
        /// Retrieves the fully qualified name of the class that called the logger.
        /// </summary>
        /// <returns>
        /// The full name of the caller class, or "UnknownCaller" if the information cannot be retrieved.
        /// </returns>
        private string GetCallerClassName()
        {
            try
            {
                var frame = new StackTrace().GetFrame(2);
                return frame?.GetMethod()?.DeclaringType?.FullName ?? "UnknownCaller";
            }
            catch
            {
                return "UnknownCaller";
            }
        }

        /// <summary>
        /// Formats the log message by including the log level and caller class name.
        /// </summary>
        /// <param name="message">The original log message to be formatted.</param>
        /// <returns>A formatted log message string including the log level and caller information.</returns>
        private string FormatLogMessage(string message) => $"[{_logLevel}] {_callerName}: {message}";

        /// <summary>
        /// Writes the specified log message to a file if file logging is enabled.
        /// </summary>
        /// <param name="message">The log message to be written to the file.</param>
        private void WriteLogToFile(string message)
        {
            if (_enableWriteLogToFile)
            {
                var loggerFileModel = new LoggerFileModel(message, _applicationName, "", _writeLogToFileFolderPath);
                LoggerFileProvider.WriteToLog(loggerFileModel);
            }
        }

        /// <summary>
        /// Writes an informational message to the configured logging outputs (Console, Event Viewer, and optionally file).
        /// </summary>
        /// <param name="message">The informational message to log.</param>
        public void LogInfo(string message)
        {
            WriteLogToFile(message);
            _logger?.LogInformation(FormatLogMessage(message));
        }

        /// <summary>
        /// Writes an error message to the configured logging outputs (Console, Event Viewer, and optionally file).
        /// </summary>
        /// <param name="message">The error message to log.</param>
        public void LogError(string message)
        {
            WriteLogToFile(message);
            _logger?.LogError(FormatLogMessage(message));
        }


        /// <summary>
        /// Writes an warning message to the configured logging outputs (Console, Event Viewer, and optionally file).
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        public void LogWarning(string message)
        {
            WriteLogToFile(message);
            _logger?.LogWarning(FormatLogMessage(message));
        }

        /// <summary>
        /// Writes an debug message to the configured logging outputs (Console, Event Viewer, and optionally file).
        /// </summary>
        /// <param name="message">The debug message to log.</param>
        public void LogDebug(string message)
        {
            WriteLogToFile(message);
            _logger?.LogDebug(FormatLogMessage(message));
        }


        /// <summary>
        /// Writes an critcal message to the configured logging outputs (Console, Event Viewer, and optionally file).
        /// </summary>
        /// <param name="message">The critcal message to log.</param>
        public void LogCritical(string message)
        {
            WriteLogToFile(message);
            _logger?.LogCritical(FormatLogMessage(message));
        }
    }
}
