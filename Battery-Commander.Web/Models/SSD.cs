using Humanizer;
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

        [Display(Name = "SSD")]
        public virtual SSDStatusModel SSDStatus
        {
            get
            {
                return new SSDStatusModel
                {
                    Rank = Rank,

                    AsOf =
                        SSDSnapshots?
                        .Select(snapshot => (DateTimeOffset?)snapshot.AsOf)
                        .OrderByDescending(d => d)
                        .FirstOrDefault(),

                    SSD_1 =
                        SSDSnapshots?
                        .OrderByDescending(snapshot => snapshot.AsOf)
                        .Where(snapshot => snapshot.SSD == SSD.SSD_1)
                        .Select(snapshot => snapshot.PerecentComplete ?? 0)
                        .FirstOrDefault(),

                    SSD_2 =
                        SSDSnapshots?
                        .OrderByDescending(snapshot => snapshot.AsOf)
                        .Where(snapshot => snapshot.SSD == SSD.SSD_2)
                        .Select(snapshot => snapshot.PerecentComplete ?? 0)
                        .FirstOrDefault(),

                    SSD_3 =
                        SSDSnapshots?
                        .OrderByDescending(snapshot => snapshot.AsOf)
                        .Where(snapshot => snapshot.SSD == SSD.SSD_3)
                        .Select(snapshot => snapshot.PerecentComplete ?? 0)
                        .FirstOrDefault(),

                    SSD_4 =
                        SSDSnapshots?
                        .OrderByDescending(snapshot => snapshot.AsOf)
                        .Where(snapshot => snapshot.SSD == SSD.SSD_4)
                        .Select(snapshot => snapshot.PerecentComplete ?? 0)
                        .FirstOrDefault(),

                    SSD_5 =
                        SSDSnapshots?
                        .OrderByDescending(snapshot => snapshot.AsOf)
                        .Where(snapshot => snapshot.SSD == SSD.SSD_5)
                        .Select(snapshot => snapshot.PerecentComplete ?? 0)
                        .FirstOrDefault()
                };
            }
        }

        public class SSDSnapshot
        {
            [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }

            [Required]
            public int SoldierId { get; set; }

            public virtual Soldier Soldier { get; set; }

            public DateTimeOffset AsOf { get; set; } = DateTimeOffset.UtcNow;

            public SSD SSD { get; set; } = SSD.SSD_1;

            public decimal? PerecentComplete { get; set; }
        }

        public class SSDStatusModel
        {
            internal const String PercentFormat = "{0:P0}";

            public Rank Rank { get; set; }

            public String CssClass
            {
                get
                {
                    if (CurrentProgress >= Decimal.One)
                    {
                        return "label label-success";
                    }

                    return "label label-danger";
                }
            }

            public Decimal? CurrentProgress
            {
                get
                {
                    switch (Rank)
                    {
                        case Rank.E1:
                        case Rank.E2:
                        case Rank.E3:
                        case Rank.E4:
                        case Rank.E4_CPL:
                            return SSD_1;

                        case Rank.E5:
                            return SSD_2;

                        case Rank.E6:
                            return SSD_3;

                        case Rank.E7:
                        case Rank.E8:
                        case Rank.E8_MSG:
                            return SSD_4;

                        case Rank.E9:
                            return SSD_5;

                        default:
                            return null;
                    }
                }
            }

            [Display(Name = "Updated")]
            public DateTimeOffset? AsOf { get; set; }

            public TimeSpan? Updated => (DateTimeOffset.UtcNow - AsOf);

            [Display(Name = "Updated")]
            public String UpdatedHumanized => Updated?.Humanize(minUnit: Humanizer.Localisation.TimeUnit.Minute);

            [Display(Name = "SSD 1"), DisplayFormat(DataFormatString = PercentFormat)]
            public decimal? SSD_1 { get; set; }

            [Display(Name = "SSD 2"), DisplayFormat(DataFormatString = PercentFormat)]
            public decimal? SSD_2 { get; set; }

            [Display(Name = "SSD 3"), DisplayFormat(DataFormatString = PercentFormat)]
            public decimal? SSD_3 { get; set; }

            [Display(Name = "SSD 4"), DisplayFormat(DataFormatString = PercentFormat)]
            public decimal? SSD_4 { get; set; }

            [Display(Name = "SSD 5"), DisplayFormat(DataFormatString = PercentFormat)]
            public decimal? SSD_5 { get; set; }
        }
    }

    public static class SSDExtensions
    {
        public static SSD? RequiredSSD(this Rank rank)
        {
            switch (rank)
            {
                case Rank.E1:
                case Rank.E2:
                case Rank.E3:
                case Rank.E4:
                case Rank.E4_CPL:
                    return SSD.SSD_1;

                case Rank.E5:
                    return SSD.SSD_2;

                case Rank.E6:
                    return SSD.SSD_3;

                case Rank.E7:
                case Rank.E8:
                case Rank.E8_MSG:
                    return SSD.SSD_4;

                case Rank.E9:
                    return SSD.SSD_5;

                // SSD_6 is in Development for E9's

                default:
                    return null;
            }
        }
    }
}