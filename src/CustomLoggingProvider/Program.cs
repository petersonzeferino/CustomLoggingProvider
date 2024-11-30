using CustomLoggingProviderLibrary;
using System;

namespace CustomLoggingProvider
{
    public  class Program
    {
        static void Main(string[] args)
        {
            LoggerEventProvider.Initialize("CustomLoggingProvider", 9);
            var _logger = new LoggerEventProvider();

            _logger.LogInfo("Test message for logger");

            Console.WriteLine("Hello World");
            Console.ReadLine();           
        }
    }
}
