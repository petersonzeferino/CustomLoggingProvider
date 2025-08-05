using CustomLoggingProviderLibrary.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Security;

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
        /// Initializes the global logger for the application
        /// Should be called only once at the start of the application
        /// </summary>
        /// <param name="applicationName">Name of the application to identify the log in the Event Viewer</param>
        /// <param name="logName">Minimum log level</param>
        /// <param name="logMinimumLevel">     </param>
        /// <param name="enableWriteLogToFile">Enable file write logging to identify if it is enabled</param>
        /// <param name="writeLogToFileFolderPath">      </param>
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

        private string FormatLogMessage(string message) => $"[{_logLevel}] {_callerName}: {message}";

        private void WriteLogToFile(string message)
        {
            if (_enableWriteLogToFile)
            {
                var loggerFileModel = new LoggerFileModel(message, _applicationName, "", _writeLogToFileFolderPath);
                LoggerFileProvider.WriteToLog(loggerFileModel);
            }
        }

        /// <summary>
        /// Write Info message
        /// </summary>
        /// <param name="message">Message for log</param>
        /// <param name="actioName">Name of the method called</param>
        public void LogInfo(string message)
        {
            WriteLogToFile(message);
            _logger?.LogInformation(FormatLogMessage(message));
        }

        /// <summary>
        /// Write Error message
        /// </summary>
        /// <param name="message">Message for log</param>
        /// <param name="actioName">Name of the method called</param>
        public void LogError(string message)
        {
            WriteLogToFile(message);
            _logger?.LogError(FormatLogMessage(message));
        }


        /// <summary>
        /// Write Warning message
        /// </summary>
        /// <param name="message">Message for log</param>
        /// <param name="actioName">Name of the method called</param>
        public void LogWarning(string message)
        {
            WriteLogToFile(message);
            _logger?.LogWarning(FormatLogMessage(message));
        }

        /// <summary>
        /// Write Debug message
        /// </summary>
        /// <param name="message">Message for log</param>
        /// <param name="actioName">Name of the method called</param>
        public void LogDebug(string message)
        {
            WriteLogToFile(message);
            _logger?.LogDebug(FormatLogMessage(message));
        }


        /// <summary>
        /// Write Critical message
        /// </summary>
        /// <param name="message">Message for log</param>
        /// <param name="actioName">Name of the method called</param>
        public void LogCritical(string message)
        {
            WriteLogToFile(message);
            _logger?.LogCritical(FormatLogMessage(message));
        }
    }
}
