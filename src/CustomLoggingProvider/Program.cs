using CustomLoggingProviderDomain;
using CustomLoggingProviderLibrary;
using System;

namespace CustomLoggingProvider
{
    public  class Program
    {
        static void Main(string[] args)
        {
            LoggerEventProvider.Initialize(
                                "LoggingProvider", 
                                  9);

            var _logger = new LoggerEventProvider();

            _logger.LogInfo("This is a test message for Event Viewer.");

            _logger.LogInfo("Test message for log info");
            _logger.LogError("Test message for log error");
            _logger.LogWarning("Test message for log warning");
            _logger.LogDebug("Test message for log debug");
            _logger.LogCritical("Test message for log critical");

            Configuration.Startup();

            Console.ReadLine();           
        }
    }
}
