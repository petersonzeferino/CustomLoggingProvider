
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
        /// Initializes the global logger for the application.
        /// Should be called only once at the start of the application.
        /// </summary>
        /// <param name="applicationName">Name of the application to identify the log.</param>
        /// <param name="minimumLevel">Minimum log level.</param>
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
     
        public void LogInfo(string message) 
        {
            _logger?.LogInformation(FormatLogMessage(message));
        }

        public void LogError(string message)
        {
            _logger?.LogError(FormatLogMessage(message));
        }

        public void LogWarning(string message)
        {
            _logger?.LogWarning(FormatLogMessage(message));
        }

        public void LogDebug(string message)
        {
            _logger?.LogDebug(FormatLogMessage(message));
        }

        public void LogCritical(string message)
        {   
            _logger?.LogCritical(FormatLogMessage(message));
        }
    }
}


