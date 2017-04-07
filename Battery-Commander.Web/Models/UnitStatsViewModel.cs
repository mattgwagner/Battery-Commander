using System;
using System.ComponentModel.DataAnnotations;

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

            [Display(Name = "Not Tested")]
            public int NotTested { get; set; }

            [Display(Name = "Pass %")]
            public Decimal PercentPass => (Decimal)Passed / Assigned * 100;
        }
    }
}