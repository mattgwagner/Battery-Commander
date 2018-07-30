using System;

namespace BatteryCommander.Web.Models.Reports
{
    public class Green3_SensitiveItems : Report
    {
        // You damn well better have a Green status

        public String Status => "GREEN";

        // HACK: Make this configurable for a specific user

        public String Authentication => "1LT MW";
    }
}