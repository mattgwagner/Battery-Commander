using BatteryCommander.Web.Models.Reports;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BatteryCommander.Web.Models
{
    public class Unit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public String Name { get; set; }

        public String UIC { get; set; }

        public Boolean IgnoreForReports { get; set; }

        public virtual ICollection<Soldier> Soldiers { get; set; }

        public virtual ICollection<Vehicle> Vehicles { get; set; }

        // This feels a bit hacky, but it'll do for now

        [NotMapped]
        public UnitConfiguration Configuration
        {
            get { return JsonConvert.DeserializeObject<UnitConfiguration>(_configuration); }
            set { _configuration = JsonConvert.SerializeObject(value); }
        }

        public String _configuration { get; set; } = "[]";

        public class UnitConfiguration
        {
            public Boolean SendPERSTAT { get; set; }

            public String PERSTAT_From { get; set; } = "Reports@redleg.app";

            public ICollection<String> PERSTAT_Recipients { get; set; } = new List<String>();
        }

        [NotMapped]
        public virtual IEnumerable<Soldier> CLS
        {
            get { return Soldiers.Where(soldier => soldier.ClsQualified).OrderBy(soldier => soldier.LastName).ThenBy(soldier => soldier.FirstName); }
        }

        [NotMapped]
        public virtual IEnumerable<Row> SSD
        {
            get
            {
                return
                    RankExtensions
                    .All()
                    .Where(rank => rank.IsEnlisted() || rank.IsNCO())
                    .Select(rank => new Row
                    {
                        Rank = rank,
                        Assigned = Soldiers.Where(soldier => soldier.Rank == rank).Count(),
                        Completed = Soldiers.Where(soldier => soldier.Rank == rank).Where(soldier => soldier.SSDStatus.CurrentProgress >= Decimal.One).Count()
                    })
                    .Where(row => row.Assigned > 0)
                    .ToList();
            }
        }

        [NotMapped]
        public virtual Row SSDTotal
        {
            get
            {
                return new Row
                {
                    Assigned = SSD.Select(_ => _.Assigned).Sum(),
                    Completed = SSD.Select(_ => _.Completed).Sum()
                };
            }
        }

        [NotMapped]
        public virtual IEnumerable<Row> Education
        {
            get
            {
                return
                    RankExtensions
                    .All()
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

        [NotMapped]
        public virtual Row EducationTotal
        {
            get
            {
                return new Row
                {
                    Assigned = Education.Select(_ => _.Assigned).Sum(),
                    Completed = Education.Select(_ => _.Completed).Sum()
                };
            }
        }

        [NotMapped]
        public virtual Stat ABCP
        {
            get
            {
                return new Stat
                {
                    Assigned = Soldiers.Count(),
                    Passed = Soldiers.Where(soldier => soldier.AbcpStatus == EventStatus.Passed).Count(),
                    Failed = Soldiers.Where(soldier => soldier.AbcpStatus == EventStatus.Failed).Count(),
                    NotTested = Soldiers.Where(soldier => soldier.AbcpStatus == EventStatus.NotTested).Count()
                };
            }
        }

        [NotMapped]
        public virtual Stat APFT
        {
            get
            {
                return new Stat
                {
                    Assigned = Soldiers.Count(),
                    Passed = Soldiers.Where(soldier => soldier.ApftStatus == EventStatus.Passed).Count(),
                    Failed = Soldiers.Where(soldier => soldier.ApftStatus == EventStatus.Failed).Count(),
                    NotTested = Soldiers.Where(soldier => soldier.ApftStatus == EventStatus.NotTested).Count()
                };
            }
        }

        [NotMapped]
        public virtual Stat DSCA
        {
            get
            {
                return new Stat
                {
                    Assigned = Soldiers.Count(),
                    Passed = Soldiers.Where(soldier => soldier.DscaQualified).Count(),
                    Failed = Soldiers.Where(soldier => soldier.DscaQualificationDate.HasValue && !soldier.DscaQualified).Count(),
                    NotTested = Soldiers.Where(soldier => !soldier.DscaQualificationDate.HasValue).Count()
                };
            }
        }

        [NotMapped]
        public virtual Stat IWQ
        {
            get
            {
                return new Stat
                {
                    Assigned = Soldiers.Count(),
                    Passed = Soldiers.Where(soldier => soldier.IwqQualified).Count(),
                    Failed = Soldiers.Where(soldier => soldier.IwqQualificationDate.HasValue && !soldier.IwqQualified).Count(),
                    NotTested = Soldiers.Where(soldier => !soldier.IwqQualificationDate.HasValue).Count()
                };
            }
        }

        [NotMapped]
        public virtual Red1_Perstat PERSTAT
        {
            get { return new Red1_Perstat { Soldiers = Soldiers }; }
        }
    }
}