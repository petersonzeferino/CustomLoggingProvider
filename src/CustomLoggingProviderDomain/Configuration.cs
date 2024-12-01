﻿using CustomLoggingProviderLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomLoggingProviderDomain
{
    public static class Configuration
    {
        public static void Startup()
        {
            LoggerEventProvider.Initialize(9);
            var _logger = new LoggerEventProvider();

            _logger.LogInfo("Test message for log info from Configuration");
        }
    }
}
