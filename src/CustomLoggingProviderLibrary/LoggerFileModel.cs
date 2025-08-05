using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomLoggingProviderLibrary
{
    public class LoggerFileModel
    {
        public string Message { get; set; }
        public string ApplicationName { get; set; } 
        public string MachineIdentifier { get; private set; }
        public string WriteLogToFileFolderPath { get; private set; }
        public bool EnableClearFileContent { get; private set; } = false;
        public DateTime BackupLogTime { get; private set; }

        public LoggerFileModel(string message, string applicationName, string machineIdentifier, string writeLogToFileFolderPath)
        {
            Message = message;
            ApplicationName = applicationName;
            MachineIdentifier = machineIdentifier;
            WriteLogToFileFolderPath = writeLogToFileFolderPath;
        }

        public void SetClearFileContent(bool isEnable)
        {
            EnableClearFileContent = isEnable;
        }

        public void SetBackUpLogFile() 
        {
            BackupLogTime = DateTime.Now.Date;
        }
    }
}