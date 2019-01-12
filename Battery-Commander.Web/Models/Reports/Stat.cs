using System;
using System.ComponentModel.DataAnnotations;
using static BatteryCommander.Web.Models.Soldier;

namespace BatteryCommander.Web.Models.Reports
{
    public class Stat
    {
        public int Assigned { get; set; }

        public int Passed { get; set; }

        public int Failed { get; set; }

        [Display(Name = "Not Tested")]
        public int NotTested { get; set; }

        [Display(Name = "Pass %"), DisplayFormat(DataFormatString = SSDStatusModel.PercentFormat)]
        public Decimal PercentPass => (Assigned > 0 ? (Decimal)Passed / Assigned : Decimal.Zero;
    }

    public class Row
    {
        public Rank Rank { get; set; }

        public int Assigned { get; set; }

        public int Incomplete => Assigned - Completed;

        public int Completed { get; set; }

        [DisplayFormat(DataFormatString = SSDStatusModel.PercentFormat)]
        public Decimal Percentage => Assigned > 0 ? (Decimal)Completed / Assigned : Decimal.Zero;
    }
}