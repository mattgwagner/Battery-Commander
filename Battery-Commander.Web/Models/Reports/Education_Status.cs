using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static BatteryCommander.Web.Models.Soldier;

namespace BatteryCommander.Web.Models.Reports
{
    public class Education_Status
    {
        public IEnumerable<Soldier> Soldiers { get; set; } = Enumerable.Empty<Soldier>();

        public IEnumerable<Row> Rows
        {
            get
            {
                return
                    RankExtensions
                    .All()
                    .Where(rank => Rank.Cadet != rank)
                    .Select(rank => new Row
                    {
                        Rank = rank,
                        Assigned = Soldiers.Where(soldier => soldier.Rank == rank).Count(),
                        Completed = Soldiers.Where(soldier => soldier.Rank == rank).Where(soldier => soldier.IsEducationComplete).Count()
                    })
                    .Where(stat => stat.Assigned > 0)
                    .ToList();
            }
        }

        public class Row
        {
            public Rank Rank { get; set; }

            public int Assigned { get; set; }

            public int Incomplete => Assigned - Completed;

            public int Completed { get; set; }

            [DisplayFormat(DataFormatString = SSDStatusModel.Format)]
            public Decimal Percentage => Assigned > 0 ? (Decimal)Completed / Assigned : Decimal.Zero;
        }
    }
}