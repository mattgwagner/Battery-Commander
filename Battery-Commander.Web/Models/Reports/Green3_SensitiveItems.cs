using System;
using System.Linq;

namespace BatteryCommander.Web.Models.Reports
{
    public class Green3_SensitiveItems : Report
    {
        public Green3_SensitiveItems(Unit unit) : base(unit)
        {
            // Nothing to do here
        }

        // HACK: Make this configurable for a specific user

        public String Authentication => Unit.ReportSettings.SingleOrDefault(settings => settings.Type == Type)?.From.Name;

        // You damn well better have a Green status

        public String Status => "GREEN";

        public override ReportType Type => ReportType.Sensitive_Items;
    }
}