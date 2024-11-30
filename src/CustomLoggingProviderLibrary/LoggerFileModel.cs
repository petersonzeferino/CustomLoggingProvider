using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomLoggingProviderLibrary
{
    public class LoggerFileModel
    {
        public DateTime BackupLogTime { get; set; }
        public string MachineIdentifier { get; set; }
        public string ProcessEventsLogFolder { get; set; }
        public int Logging { get; set; }
    }
}