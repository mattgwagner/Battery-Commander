using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static BatteryCommander.Web.Models.Soldier;

namespace BatteryCommander.Web.Models
{
    public class UnitStatsViewModel
    {
        public Unit Unit { get; set; }

        // Assigned	Passed	Failed	Not Tested	% Pass/Assigned

        public Stat ABCP { get; set; } = new Stat { };

        public Stat APFT { get; set; } = new Stat { };

        public Stat DSCA { get; set; } = new Stat { };

        public Stat IWQ { get; set; } = new Stat { };

        public ICollection<SSDStat> SSD { get; set; } = new List<SSDStat>();

        public ICollection<SSDStat> Education { get; set; } = new List<SSDStat>();

        public SSDStat SSDTotal => new SSDStat
        {
            Assigned = SSD.Select(s => s.Assigned).Sum(),
            Completed = SSD.Select(s => s.Completed).Sum()
        };

        public class SSDStat
        {
            public Rank Rank { get; set; }

            public int Assigned { get; set; }

            public int Incomplete => Assigned - Completed;

            public int Completed { get; set; }

            [DisplayFormat(DataFormatString = SSDStatusModel.Format)]
            public Decimal Percentage => Assigned > 0 ? (Decimal)Completed / Assigned : Decimal.Zero;
        }

        public class Stat
        {
            public int Assigned { get; set; }

            public int Passed { get; set; }

            public int Failed { get; set; }

            [Display(Name = "Not Tested")]
            public int NotTested { get; set; }

            [Display(Name = "Pass %"), DisplayFormat(DataFormatString = SSDStatusModel.Format)]
            public Decimal PercentPass => (Decimal)Passed / Assigned;
        }
    }
}