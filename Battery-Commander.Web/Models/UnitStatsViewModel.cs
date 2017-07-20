using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static BatteryCommander.Web.Models.Soldier;

namespace BatteryCommander.Web.Models
{
    public class UnitStatsViewModel
    {
        public Unit Unit { get; set; }

        // Assigned	Passed	Failed	Not Tested	% Pass/Assigned

        public Stat ABCP { get; set; } = new Stat { };

        public Stat APFT { get; set; } = new Stat { };

        public ICollection<SSDStat> SSD { get; set; } = new List<SSDStat>();

        public class SSDStat
        {
            public Rank Rank { get; set; }

            public int Assigned { get; set; }

            public int Completed { get; set; }

            [DisplayFormat(DataFormatString = SSDStatusModel.Format)]
            public Decimal Percentage => Assigned > 0 ? (Decimal)Completed / Assigned * 100 : Decimal.Zero;
        }

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