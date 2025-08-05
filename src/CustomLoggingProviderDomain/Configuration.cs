using CustomLoggingProviderLibrary;
using Microsoft.Extensions.Logging;
using System;

namespace CustomLoggingProviderDomain
{
    public static class Configuration
    {
        public static void Startup()
        {
            if (!InitializeLogger())
            {
                Console.WriteLine("Logger initialization failed in Configuration.");
                return;
            }

            var logger = new LoggerEventProvider();
            LogStartupMessage(logger);
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

        private static void LogStartupMessage(LoggerEventProvider logger)
        {
            logger.LogInfo("Test message for log info from Configuration");
        }
    }
}
