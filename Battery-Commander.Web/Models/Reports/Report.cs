using System;

namespace BatteryCommander.Web.Models.Reports
{
    public abstract class Report
    {
        // Format - Text, HTML, PDF, Excel

        // FROM Unit

        // TO Recipient(s), CC/BCC

        // Schedule(s)

        public abstract ReportType Type { get; }

        public virtual String DateTimeGroup => DateTime.UtcNow.ToEst().ToDateTimeGroup();

        public enum ReportType : byte
        {
            // Standard Types of Reports - PERSTAT, Green 3 Sensitive Items, Yellow 1 LOGSTAT, TAN 1 COMSTAT

            Sensitive_Items,

            Perstat,

            Logstat,

            Comstat
        }
    }
}