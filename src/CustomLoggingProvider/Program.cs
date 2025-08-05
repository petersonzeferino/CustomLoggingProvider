using CustomLoggingProviderDomain;
using CustomLoggingProviderLibrary;
using Microsoft.Extensions.Logging;
using System;

namespace CustomLoggingProvider
{
    public class Program
    {
        static void Main(string[] args)
        {

            if (!InitializeLogger())
            {
                Console.WriteLine("Logger initialization failed in Configuration.");
                return;
            }

            var logger = new LoggerEventProvider();
            LogTestMessages(logger);

            Configuration.Startup();

            Console.ReadLine();
        }

        private static bool InitializeLogger()
        {
            return LoggerEventProvider.Initialize(
                "LoggingProvider",
                "LoggingProvider",
                 LogLevel.Debug,
                 true
            );
        }

        private static void LogTestMessages(LoggerEventProvider logger)
        {
            logger.LogInfo("Test message for log info");
            logger.LogError("Test message for log error");
            logger.LogWarning("Test message for log warning");
            logger.LogDebug("Test message for log debug");
            logger.LogCritical("Test message for log critical");
        }
    }
}
