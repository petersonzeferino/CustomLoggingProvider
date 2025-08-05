
using Microsoft.Extensions.Logging;

namespace CustomLoggingProviderLibrary
{
    internal class LoggerProviderFactory
    {
        private static ILoggerFactory _loggerFactory;

        /// <summary>
        /// Configures and returns a typed logger instance for the specified class.
        /// Initializes the logger factory on first use and configures logging to Console and Windows Event Log.
        /// </summary>
        /// <typeparam name="T">The type (usually the calling class) associated with the logger instance.</typeparam>
        /// <param name="applicationName">The application name used as the source in the Windows Event Viewer.</param>
        /// <param name="logName">The name of the event log (e.g., "Application") to which logs will be written.</param>
        /// <param name="logLevel">The minimum log level that should be captured (e.g., Information, Warning, Error).</param>
        /// <returns>A configured <see cref="ILogger{T}"/> instance for logging.</returns>
        internal static ILogger<T> GetLogger<T>(string applicationName, string logName, LogLevel logLevel)
        {
            if (_loggerFactory == null)
            {
                _loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder
                        .AddConsole()                                   // Adds logging to the console
                        .AddEventLog(settings =>                        // Logs to the Event Viewer
                        {
                            settings.SourceName = applicationName;     // Source name displayed in the Event Viewer
                            settings.LogName = logName;                // Log name displayed in the Event Viewer
                        })
                        .SetMinimumLevel(logLevel);                    // Sets the minimum level
                });
            }
            return _loggerFactory.CreateLogger<T>();
        }

    }
}