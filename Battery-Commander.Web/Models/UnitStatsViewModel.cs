using BatteryCommander.Web.Models.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static BatteryCommander.Web.Models.Soldier;

namespace BatteryCommander.Web.Models
{
    public class UnitStatsViewModel
    {
        // Vehicle Breakdown - FMC/On-Hand, HMMWV, LMTV

        public Unit Unit { get; set; }

        // Assigned	Passed	Failed	Not Tested	% Pass/Assigned

        public Stat ABCP { get; set; } = new Stat { };

        public Stat APFT { get; set; } = new Stat { };

        public Stat DSCA { get; set; } = new Stat { };

        public Stat IWQ { get; set; } = new Stat { };
    }
}