
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

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
        public static void Initialize(int minimumLevel)
        {
            _applicationName = GetCallerClassName();
            _logLevel = ParseLogLevel(minimumLevel);
            _logger = LoggerProviderFactory.GetLogger<LoggerEventProvider>(_applicationName, _logLevel);
        }

        private static string GetCallerClassName()
        {
            try
            {
                StackTrace stackTrace = new StackTrace();
                StackFrame frame = stackTrace.GetFrame(2);
                return frame.GetMethod().DeclaringType.FullName;
            }
            catch (Exception)
            {
                return string.Empty;
            }
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

        /// <summary>
        /// Write Info message
        /// </summary>
        /// <param name="message">Message for log</param>
        /// <param name="actioName">Name of the method called</param>
        public void LogInfo(string message) 
        {
            _logger?.LogInformation(FormatLogMessage(message));
        }

        /// <summary>
        /// Write Error message
        /// </summary>
        /// <param name="message">Message for log</param>
        /// <param name="actioName">Name of the method called</param>
        public void LogError(string message, string actioName = null)
        {
            _logger?.LogError(FormatLogMessage(message));
        }

        /// <summary>
        /// Write Warning message
        /// </summary>
        /// <param name="message">Message for log</param>
        /// <param name="actioName">Name of the method called</param>
        public void LogWarning(string message)
        {
            _logger?.LogWarning(FormatLogMessage(message));
        }

        /// <summary>
        /// Write Debug message
        /// </summary>
        /// <param name="message">Message for log</param>
        /// <param name="actioName">Name of the method called</param>
        public void LogDebug(string message)
        {
            _logger?.LogDebug(FormatLogMessage(message));
        }

        /// <summary>
        /// Write Critical message
        /// </summary>
        /// <param name="message">Message for log</param>
        /// <param name="actioName">Name of the method called</param>
        public void LogCritical(string message)
        {   
            _logger?.LogCritical(FormatLogMessage(message));
        }
    }
}


