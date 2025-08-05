using System;
using System.Diagnostics;
using System.Security;

namespace CustomLoggingProviderLibrary.Helpers
{
    internal class EventLogHelper
    {

        /// <summary>
        /// Ensures that the specified Event Log source is registered in the given log.
        /// If it doesn't exist, creates it and optionally writes a test log entry.
        /// </summary>
        /// <param name="sourceName">The name of the source to use.</param>
        /// <param name="logName">The log name to associate with the source (default is "Application").</param>
        /// <param name="writeTestEntry">If true, writes a test message to the event log after creating it.</param>
        internal void EnsureEventLogSource(string sourceName, string logName = "Application", bool writeTestEntry = true)
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
    }
}
