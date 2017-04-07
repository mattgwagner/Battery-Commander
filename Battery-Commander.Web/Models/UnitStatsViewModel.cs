using System;

namespace BatteryCommander.Web.Models
{
    public class UnitStatsViewModel
    {
        public Unit Unit { get; set; }

        // Assigned	Passed	Failed	Not Tested	% Pass/Assigned

        public Stat ABCP { get; set; } = new Stat { };

        public Stat APFT { get; set; } = new Stat { };

        public class Stat
        {
            public int Assigned { get; set; }

            public int Passed { get; set; }

            public int Failed { get; set; }

            public int NotTested { get; set; }

            public Decimal PercentPass => (Decimal)Passed / Assigned;
        }
    }
}