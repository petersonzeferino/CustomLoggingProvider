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
            var program = new Program();

            var logger = new LoggerEventProvider("LoggingProvider", "LoggingProvider", LogLevel.Debug, true, "F:\\Temp");

            program.LogTestMessages(logger);

            new Configuration();

            Console.ReadLine();
        }

        public void LogTestMessages(LoggerEventProvider logger)
        {
            logger.LogInfo("Test message for log info");
            logger.LogError("Test message for log error");
            logger.LogWarning("Test message for log warning");
            logger.LogDebug("Test message for log debug");
            logger.LogCritical("Test message for log critical");
        }
    }
}
