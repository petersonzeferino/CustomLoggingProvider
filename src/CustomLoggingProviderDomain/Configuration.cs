using CustomLoggingProviderLibrary;
using Microsoft.Extensions.Logging;

namespace CustomLoggingProviderDomain
{
    public class Configuration
    {
        public Configuration()
        {
            var logger = new LoggerEventProvider("LoggingProvider", "LoggingProvider", LogLevel.Debug);
            LogStartupMessage(logger);
        }

        private void LogStartupMessage(LoggerEventProvider logger)
        {
            logger.LogInfo("Test message for log info from Configuration");
        }
    }

}
