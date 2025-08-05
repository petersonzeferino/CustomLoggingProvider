using System;

namespace CustomLoggingProviderLibrary
{
    /// <summary>
    /// Represents the model used for writing log messages to a file.
    /// Contains log metadata such as application name, machine identifier, file path, and options for clearing or backing up the log.
    /// </summary>
    public class LoggerFileModel
    {
        public string Message { get; set; }
        public string ApplicationName { get; set; } 
        public string MachineIdentifier { get; private set; }
        public string WriteLogToFileFolderPath { get; private set; }
        public bool EnableClearFileContent { get; private set; } = false;
        public DateTime BackupLogTime { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerFileModel"/> class with the specified message, application name, machine identifier, and log file folder path.
        /// </summary>
        /// <param name="message">The log message to be written.</param>
        /// <param name="applicationName">The name of the application generating the log.</param>
        /// <param name="machineIdentifier">An optional identifier for the machine where the log is generated.</param>
        /// <param name="writeLogToFileFolderPath">The folder path where the log file should be written.</param>
        public LoggerFileModel(string message, string applicationName, string machineIdentifier, string writeLogToFileFolderPath)
        {
            Message = message;
            ApplicationName = applicationName;
            MachineIdentifier = machineIdentifier;
            WriteLogToFileFolderPath = writeLogToFileFolderPath;
        }

        /// <summary>
        /// Enables or disables clearing the content of the log file before writing.
        /// </summary>
        /// <param name="isEnable">True to enable clearing the file content; false to disable.</param>
        public void SetClearFileContent(bool isEnable)
        {
            EnableClearFileContent = isEnable;
        }

        /// <summary>
        /// Sets the backup timestamp for the log file to the current date.
        /// This can be used to track when the log file was last backed up.
        /// </summary>
        public void SetBackUpLogFile() 
        {
            BackupLogTime = DateTime.Now.Date;
        }
    }
}