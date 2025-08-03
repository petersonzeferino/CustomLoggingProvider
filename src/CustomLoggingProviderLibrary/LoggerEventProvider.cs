
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Security;

namespace CustomLoggingProviderLibrary
{
    public class LoggerEventProvider
    {
        private static ILogger _logger;
        private static LogLevel _logLevel;
        private static string _callerName;
        private static string _applicationName;
        private static string _logName;
        private static string _enableWriteLogToFile;

        /// <summary>
        /// Initializes the global logger for the application
        /// Should be called only once at the start of the application
        /// </summary>
        /// <param name="applicationName">Name of the application to identify the log in the Event Viewer</param>
        /// <param name="minimumLevel">Minimum log level</param>
        /// <param name="enableWriteLogToFile">Enable file write logging and use the values ​​Y, N, 0, 1 to identify if it is enabled</param>
        public static void Initialize(string applicationName,
                                      string logName,
                                      int minimumLevel,
                                      string enableWriteLogToFile = "")
        {
            _applicationName = applicationName;
            _logName = logName;
            _logLevel = ParseLogLevel(minimumLevel);
            _callerName = GetCallerClassName();
            _enableWriteLogToFile = enableWriteLogToFile;           
            _logger = LoggerProviderFactory.GetLogger<LoggerEventProvider>(_applicationName, _logName, _logLevel);

            EnsureEventLogSource(_applicationName, _logName, true);
        }

        /// <summary>
        /// Ensures that the specified Event Log source exists. 
        /// If it does not, attempts to create it in the "Application" log.
        /// This operation requires administrative privileges.
        /// </summary>
        /// <param name="sourceName">The name of the source to register in the Event Log.</param>
        /// <summary>
        /// Ensures that the specified Event Log source exists.
        /// If it does not, attempts to create it in the "Application" log.
        /// This operation requires administrative privileges.
        /// </summary>
        /// <param name="sourceName">The name of the source to register in the Event Log.</param>
        /// <summary>
        /// Ensures that the specified Event Log source exists.
        /// If it does not, attempts to create it in the "Application" log.
        /// This operation requires administrative privileges.
        /// </summary>
        /// <param name="sourceName">The name of the source to register in the Event Log.</param>
        //public static void EnsureEventLogSourceExists(string sourceName)
        //{
        //    try
        //    {
        //        // Check if the source exists, and create it if necessary
        //        if (!EventLog.SourceExists(sourceName))
        //        {
        //            EventLog.CreateEventSource(new EventSourceCreationData(sourceName, "Application"));

        //            // Optional: Write an initial log entry
        //            EventLog.WriteEntry(sourceName, "Event source created successfully.", EventLogEntryType.Information);
        //        }
        //    }
        //    catch (SecurityException)
        //    {
        //        Console.ForegroundColor = ConsoleColor.Red;
        //        Console.WriteLine("❌ Administrator privileges are required to create an Event Log source.");
        //        Console.WriteLine("➡️  Please run the application as Administrator and try again.");
        //        Console.ResetColor();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.ForegroundColor = ConsoleColor.Red;
        //        Console.WriteLine("❌ An unexpected error occurred while checking or creating the event log source:");
        //        Console.WriteLine(ex.Message);
        //        Console.ResetColor();
        //    }
        //}




        //public static void EnsureCustomEventLogExists(string sourceName, string logName)
        //{
        //    try
        //    {
        //        // Check if source exists and is correctly mapped
        //        if (EventLog.SourceExists(sourceName))
        //        {
        //            var currentLogName = EventLog.LogNameFromSourceName(sourceName, ".");
        //            if (!string.Equals(currentLogName, logName, StringComparison.OrdinalIgnoreCase))
        //            {
        //                Console.WriteLine($"❌ Source '{sourceName}' is already registered in log '{currentLogName}', not '{logName}'.");
        //                return;
        //            }
        //        }
        //        else
        //        {
        //            // Register the new source with the custom log name
        //            EventLog.CreateEventSource(new EventSourceCreationData(sourceName, logName));
        //            Console.WriteLine($"✅ Source '{sourceName}' created in log '{logName}'.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"❌ Failed to create event log source: {ex.Message}");
        //    }
        //}



        /// <summary>
        /// Ensures that the specified Event Log source is registered in the given log.
        /// If it doesn't exist, creates it and optionally writes a test log entry.
        /// </summary>
        /// <param name="sourceName">The name of the source to use.</param>
        /// <param name="logName">The log name to associate with the source (default is "Application").</param>
        /// <param name="writeTestEntry">If true, writes a test message to the event log after creating it.</param>
        public static void EnsureEventLogSource(string sourceName, string logName = "Application", bool writeTestEntry = true)
        {
            try
            {
                if (EventLog.SourceExists(sourceName))
                {
                    string currentLog = EventLog.LogNameFromSourceName(sourceName, ".");

                    if (!string.Equals(currentLog, logName, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"❌ Source '{sourceName}' is already registered in log '{currentLog}', not '{logName}'.");
                        Console.WriteLine("➡️  You must delete the existing source or use a different source name.");
                        Console.ResetColor();
                        return;
                    }

                    Console.WriteLine($"✅ Source '{sourceName}' is already correctly registered in log '{logName}'.");
                }
                else
                {
                    EventLog.CreateEventSource(new EventSourceCreationData(sourceName, logName));
                    Console.WriteLine($"✅ Source '{sourceName}' created and linked to log '{logName}'.");

                    if (writeTestEntry)
                    {
                        EventLog.WriteEntry(sourceName, "Event source created successfully.", EventLogEntryType.Information);
                        Console.WriteLine($"ℹ️  Test log entry written to '{logName}' with source '{sourceName}'.");
                    }
                }
            }
            catch (SecurityException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Administrator privileges are required to create or modify Event Log sources.");
                Console.WriteLine("➡️  Please run the application as Administrator and try again.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Unexpected error while checking or creating the event log source:");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
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

        private string FormatLogMessage(string message) => $"[{_logLevel}] {_callerName}: {message}";

        private void WriteLogToFile(string message)
        {
            if (ConvertToBool(_enableWriteLogToFile))
            {
                var loggerFileModel = new LoggerFileModel(message,
                                                          _applicationName,
                                           "",
                                       "F:\\Temp\\");

                LoggerFileProvider.WriteToLog(loggerFileModel);
            }
        }

        private bool ConvertToBool(string value)
        {
            if (value == "1" || value.Equals("Y", StringComparison.OrdinalIgnoreCase) || value.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else if (value == "0" || value.Equals("N", StringComparison.OrdinalIgnoreCase) || value.Equals("false", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            else
            {
                return false;
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
        public void LogError(string message, string actioName = null)
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


