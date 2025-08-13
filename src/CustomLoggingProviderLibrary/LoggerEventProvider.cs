using CustomLoggingProviderLibrary.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;

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
        private readonly bool _redactSensitiveData;

        /// <summary>
        /// Initializes the logger provider for the application.
        /// Should be called once at the start of the application to configure logging output to:
        /// - Console
        /// - Event Viewer
        /// - Optionally, a local file
        /// 
        /// Sensitive data redaction can also be enabled to mask emails, passwords, and API keys in log messages.
        /// </summary>
        /// <param name="applicationName">Name of the application used as the source identifier in the Event Viewer.</param>
        /// <param name="logName">Name of the log in the Event Viewer (e.g., "Application").</param>
        /// <param name="logMinimumLevel">Minimum log level to be captured (e.g., Information, Warning, Error).</param>
        /// <param name="enableWriteLogToFile">
        /// Indicates whether log messages should also be written to a local file. 
        /// If true, log files will be saved to <paramref name="writeLogToFileFolderPath"/> or a default folder if not specified.
        /// </param>
        /// <param name="writeLogToFileFolderPath">
        /// Optional folder path where log files will be saved if file logging is enabled. 
        /// Defaults to %LOCALAPPDATA%\CustomLoggingProvider if not provided.
        /// </param>
        /// <param name="redactSensitiveData">
        /// If true, sensitive data (emails, passwords, API keys) will be automatically masked in all log messages.
        /// </param>
        public LoggerEventProvider(
            string applicationName,
            string logName,
            LogLevel logMinimumLevel,
            bool enableWriteLogToFile = false,
            string writeLogToFileFolderPath = "",
            bool redactSensitiveData = false)
        {
            _applicationName = applicationName;
            _logName = logName;
            _logLevel = logMinimumLevel;
            _enableWriteLogToFile = enableWriteLogToFile;
            _callerName = GetCallerClassName();
            _writeLogToFileFolderPath = writeLogToFileFolderPath;
            _redactSensitiveData = redactSensitiveData;

            _logger = LoggerProviderFactory.GetLogger<LoggerEventProvider>(_applicationName, _logName, _logLevel);

            _eventLogHelper = new EventLogHelper();
            _eventLogHelper.EnsureEventLogSource(_applicationName, _logName, writeTestEntry: true, _writeLogToFileFolderPath);
        }

        /// <summary>
        /// Gets the local machine's hostname along with available IPv4 and IPv6 addresses.
        /// Example: "MYPC - IPv4: 192.168.1.10 - IPv6: fe80::1%4"
        /// If an IP type is not found, it is omitted from the result.
        /// </summary>
        /// <returns>A string containing the hostname and available IP addresses.</returns>
        private string GetHostNameAndIps()
        {
            string hostName = Dns.GetHostName();
            var hostEntry = Dns.GetHostEntry(hostName);

            string ipv4 = hostEntry.AddressList
                                   .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?
                                   .ToString();

            string ipv6 = hostEntry.AddressList
                                   .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetworkV6)?
                                   .ToString();

            // Build parts dynamically so we don't get dangling separators
            var parts = new[] { hostName,
                            ipv4 != null ? $"IPv4: {ipv4}" : null,
                            ipv6 != null ? $"IPv6: {ipv6}" : null }
                        .Where(p => !string.IsNullOrEmpty(p));

            return string.Join(" - ", parts);
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
        /// Redacts the message if redaction is enabled.
        /// </summary>
        /// <param name="message">The original log message.</param>
        /// <returns>The sanitized message if redaction is enabled; otherwise, the original message.</returns>
        private string MakeSafe(string message)
        {
            return _redactSensitiveData ? SensitiveDataRedactorHelper.Redact(message) : message;
        }

        /// <summary>
        /// Formats the log message by including the log level and caller class name.
        /// </summary>
        /// <param name="message">The original log message to be formatted.</param>
        /// <returns>A formatted log message string including the log level and caller information.</returns>
        private string FormatLogMessage(string message) => $"{_callerName}: {message}";

        /// <summary>
        /// Writes the specified log message to a file if file logging is enabled.
        /// </summary>
        /// <param name="message">The log message to be written to the file.</param>
        private void WriteLogToFile(string message)
        {
            if (_enableWriteLogToFile && !string.IsNullOrEmpty(_writeLogToFileFolderPath))
            {
                var loggerFileModel = new LoggerFileModel(message, _applicationName, Environment.MachineName, _writeLogToFileFolderPath);
                LoggerFileProvider.WriteToLog(loggerFileModel);
            }
        }

        /// <summary>
        /// Logs messages at various severity levels (Info, Error, Warning, Debug, Critical) 
        /// to both the configured file output and the optional ILogger instance. 
        /// Applies sensitive data redaction via <see cref="MakeSafe"/> before logging, 
        /// and formats messages for structured logging via <see cref="FormatLogMessage"/>.
        /// </summary>
        /// <param name="level">The severity level of the log (e.g., INFO, ERROR).</param>
        /// <param name="message">The message to log.</param>
        /// <param name="logAction">The action to invoke on the ILogger instance for the specific log level.</param>
        private void Log(string level, string message, Action<string> logAction)
        {
            string prefix = $"[{level}] ";
            string safeMessage = MakeSafe(message);

            WriteLogToFile($"{prefix}{safeMessage}");
            logAction?.Invoke($"{prefix}{FormatLogMessage(safeMessage)}");
        }

        /// <summary>Logs an informational message.</summary>
        /// <param name="message">The message to log.</param>
        public void LogInfo(string message) => Log("INFO", message, msg => _logger?.LogInformation(msg));

        /// <summary>Logs an error message.</summary>
        /// <param name="message">The message to log.</param>
        public void LogError(string message) => Log("ERROR", message, msg => _logger?.LogError(msg));

        /// <summary>Logs a warning message.</summary>
        /// <param name="message">The message to log.</param>
        public void LogWarning(string message) => Log("WARNING", message, msg => _logger?.LogWarning(msg));

        /// <summary>Logs a debug message.</summary>
        /// <param name="message">The message to log.</param>
        public void LogDebug(string message) => Log("DEBUG", message, msg => _logger?.LogDebug(msg));

        /// <summary>Logs a critical message.</summary>
        /// <param name="message">The message to log.</param>
        public void LogCritical(string message) => Log("CRITICAL", message, msg => _logger?.LogCritical(msg));

    }
}
