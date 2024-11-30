
using Microsoft.Extensions.Logging;
using System;

namespace CustomLoggingProviderLibrary
{
    public class LoggerEventProvider
    {
        private static ILogger _logger;
        private static LogLevel _logLevel;
        private static string _applicationName;

        /// <summary>
        /// Initializes the global logger for the application
        /// Should be called only once at the start of the application
        /// </summary>
        /// <param name="applicationName">Name of the application to identify the log</param>
        /// <param name="minimumLevel">Minimum log level</param>
        public static void Initialize(string applicationName, int minimumLevel)
        {
            _applicationName = applicationName;
            _logLevel = ParseLogLevel(minimumLevel);
            _logger = LoggerProviderFactory.GetLogger<LoggerEventProvider>(applicationName, _logLevel);
        }

        private static LogLevel ParseLogLevel(int value)
        {
            if (Enum.IsDefined(typeof(LogLevel), value))
            {
                return (LogLevel)value;
            }

            return LogLevel.Trace;
        }

        private string FormatLogMessage(string message) =>  $"[{_logLevel}] {_applicationName}: {message}";

        private string FormatLogMessage(string message, string actionName) => $"[{_logLevel}] {_applicationName} - {actionName}: {message}";

        /// <summary>
        /// Write Info message
        /// </summary>
        /// <param name="message">Message for log</param>
        /// <param name="actioName">Name of the method called</param>
        public void LogInfo(string message, string actioName = null) 
        {
            _logger?.LogInformation(string.IsNullOrEmpty(actioName) ? FormatLogMessage(message) : FormatLogMessage(message, actioName));
        }

        /// <summary>
        /// Write Error message
        /// </summary>
        /// <param name="message">Message for log</param>
        /// <param name="actioName">Name of the method called</param>
        public void LogError(string message, string actioName = null)
        {
            _logger?.LogError(string.IsNullOrEmpty(actioName) ? FormatLogMessage(message) : FormatLogMessage(message, actioName));
        }

        /// <summary>
        /// Write Warning message
        /// </summary>
        /// <param name="message">Message for log</param>
        /// <param name="actioName">Name of the method called</param>
        public void LogWarning(string message, string actioName = null)
        {
            _logger?.LogWarning(string.IsNullOrEmpty(actioName) ? FormatLogMessage(message) : FormatLogMessage(message, actioName));
        }

        /// <summary>
        /// Write Debug message
        /// </summary>
        /// <param name="message">Message for log</param>
        /// <param name="actioName">Name of the method called</param>
        public void LogDebug(string message, string actioName = null)
        {
            _logger?.LogDebug(string.IsNullOrEmpty(actioName) ? FormatLogMessage(message) : FormatLogMessage(message, actioName));
        }

        /// <summary>
        /// Write Critical message
        /// </summary>
        /// <param name="message">Message for log</param>
        /// <param name="actioName">Name of the method called</param>
        public void LogCritical(string message, string actioName = null)
        {   
            _logger?.LogCritical(string.IsNullOrEmpty(actioName) ? FormatLogMessage(message) : FormatLogMessage(message, actioName));
        }
    }
}


