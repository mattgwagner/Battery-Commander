using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BatteryCommander.Web.Models
{
    public enum SSD : byte
    {
        [Display(Name = "SSD 1")]
        SSD_1,

        [Display(Name = "SSD 2")]
        SSD_2,

        [Display(Name = "SSD 3")]
        SSD_3,

        [Display(Name = "SSD 4")]
        SSD_4,

        [Display(Name = "SSD 5")]
        SSD_5
    }

    public partial class Soldier
    {
        public virtual ICollection<SSDSnapshot> SSDSnapshots { get; set; }

        public virtual SSDStatusModel SSDStatus
        {
            get
            {
                return new SSDStatusModel
                {
                    AsOf =
                        SSDSnapshots
                        .Select(snapshot => snapshot.AsOf)
                        .OrderByDescending(d => d)
                        .FirstOrDefault(),

                    SSD_1 =
                        SSDSnapshots
                        .OrderByDescending(snapshot => snapshot.AsOf)
                        .Where(snapshot => snapshot.SSD == SSD.SSD_1)
                        .Select(snapshot => snapshot.PerecentComplete)
                        .FirstOrDefault(),

                    SSD_2 =
                        SSDSnapshots
                        .OrderByDescending(snapshot => snapshot.AsOf)
                        .Where(snapshot => snapshot.SSD == SSD.SSD_2)
                        .Select(snapshot => snapshot.PerecentComplete)
                        .FirstOrDefault(),

                    SSD_3 =
                        SSDSnapshots
                        .OrderByDescending(snapshot => snapshot.AsOf)
                        .Where(snapshot => snapshot.SSD == SSD.SSD_3)
                        .Select(snapshot => snapshot.PerecentComplete)
                        .FirstOrDefault(),

                    SSD_4 =
                        SSDSnapshots
                        .OrderByDescending(snapshot => snapshot.AsOf)
                        .Where(snapshot => snapshot.SSD == SSD.SSD_4)
                        .Select(snapshot => snapshot.PerecentComplete)
                        .FirstOrDefault(),

                    SSD_5 =
                        SSDSnapshots
                        .OrderByDescending(snapshot => snapshot.AsOf)
                        .Where(snapshot => snapshot.SSD == SSD.SSD_5)
                        .Select(snapshot => snapshot.PerecentComplete)
                        .FirstOrDefault()
                };
            }
        }

        public class SSDSnapshot
        {
            [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }

            public DateTimeOffset AsOf { get; set; } = DateTimeOffset.UtcNow;

            public SSD SSD { get; set; } = SSD.SSD_1;

            public decimal? PerecentComplete { get; set; }
        }

        public class SSDStatusModel
        {
            public DateTimeOffset AsOf { get; set; }

            // Humanized time since

            public decimal? SSD_1 { get; set; }

            public decimal? SSD_2 { get; set; }

            public decimal? SSD_3 { get; set; }

            public decimal? SSD_4 { get; set; }

            public decimal? SSD_5 { get; set; }
        }
    }
}