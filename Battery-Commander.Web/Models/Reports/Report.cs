using FluentEmail.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BatteryCommander.Web.Models.Reports
{
    public abstract class Report
    {
        public Unit Unit { get; }

        public Report(Unit unit)
        {
            Unit = unit;
        }

        // Format - Text, HTML, PDF, Excel

        public Boolean Enabled
        {
            get
            {
                return
                    Unit
                    .ReportSettings
                    .Where(report => report.Type == Type)
                    .Select(report => report.Enabled)
                    .SingleOrDefault();
            }
        }

        public Address From
        {
            get
            {
                return
                    Unit
                    .ReportSettings
                    .Where(report => report.Type == Type)
                    .Select(report => report.From)
                    .SingleOrDefault();
            }
        }

        public IList<Address> Recipients
        {
            get
            {
                return
                    Unit
                    .ReportSettings
                    .Where(report => report.Type == Type)
                    .SelectMany(report => report.Recipients)
                    .ToList();
            }
        }

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