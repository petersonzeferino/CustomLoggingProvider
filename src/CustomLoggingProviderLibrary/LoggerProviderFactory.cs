
using Microsoft.Extensions.Logging;

namespace CustomLoggingProviderLibrary
{
    public class LoggerProviderFactory
    {
        private static ILoggerFactory _loggerFactory;

        /// <summary>
        /// Configures and returns a logger for the specified type
        /// </summary>
        /// <typeparam name="T">Type associated with the logger</typeparam>
        /// <param name="applicationName">Application name to identify the log</param>
        /// <param name="logLevel">Minimum log level</param>
        /// <returns>Logger configured</returns>
        public static ILogger<T> GetLogger<T>(string applicationName, LogLevel logLevel)
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
                        })
                        .SetMinimumLevel(logLevel);                    // Sets the minimum level
                });
            }
            return _loggerFactory.CreateLogger<T>();
        }

    }
}