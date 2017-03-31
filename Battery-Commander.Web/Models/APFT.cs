using BatteryCommander.Web.Models.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BatteryCommander.Web.Models
{
    public class APFT
    {
        private const int MinimumPerEvent = 60;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int SoldierId { get; set; }

        public virtual Soldier Soldier { get; set; }

        [Required, DataType(DataType.Date), Column(TypeName = "date")]
        public DateTime Date { get; set; } = DateTime.Today;

        public AgeGroup AgeGroup
        {
            get
            {
                int Age = Soldier.AgeAsOf(Date);

                if (Age <= 21) return AgeGroup.Group_17_to_21;
                if (Age <= 26) return AgeGroup.Group_22_to_26;
                if (Age <= 31) return AgeGroup.Group_27_to_31;
                if (Age <= 36) return AgeGroup.Group_32_to_36;
                if (Age <= 41) return AgeGroup.Group_37_to_41;
                if (Age <= 46) return AgeGroup.Group_42_to_46;
                if (Age <= 51) return AgeGroup.Group_47_to_51;
                if (Age <= 56) return AgeGroup.Group_52_to_56;
                if (Age <= 61) return AgeGroup.Group_57_to_61;

                return AgeGroup.Group_62_Plus;
            }
        }

        [Range(0, int.MaxValue)]
        public int PushUps { get; set; }

        [Range(0, int.MaxValue)]
        public int SitUps { get; set; }

        [Range(0, int.MaxValue)]
        public int RunSeconds { get; set; }

        public TimeSpan Run => TimeSpan.FromSeconds(RunSeconds);

        public int PushUpScore
        {
            get
            {
                return
                    APFTScoreTables
                    .PushUps
                    .OrderByDescending(row => row.Reps)
                    .Where(row => row.AgeGroup == AgeGroup)
                    .Where(row => row.Gender == Soldier.Gender)
                    .Where(row => row.Reps <= PushUps)
                    .Select(row => row.Score)
                    .FirstOrDefault();
            }
        }

        public int SitUpScore
        {
            get
            {
                return
                    APFTScoreTables
                    .SitUps
                    .OrderByDescending(row => row.Reps)
                    .Where(row => row.AgeGroup == AgeGroup)
                    .Where(row => row.Gender == Soldier.Gender)
                    .Where(row => row.Reps <= SitUps)
                    .Select(row => row.Score)
                    .FirstOrDefault();
            }
        }

        public int RunScore
        {
            get
            {
                return
                    APFTScoreTables
                    .Run
                    .OrderBy(row => row.Reps)
                    .Where(row => row.AgeGroup == AgeGroup)
                    .Where(row => row.Gender == Soldier.Gender)
                    .Where(row => row.Reps >= Run.TotalSeconds)
                    .Select(row => row.Score)
                    .FirstOrDefault();
            }
        }

        public int TotalScore => PushUpScore + SitUpScore + RunScore;

        public Boolean IsPassing => new[] { PushUpScore, SitUpScore, RunScore }.All(s => s >= MinimumPerEvent);
    }

    public enum Event : byte
    {
        PushUp,

        SitUp,

        Run,

        // We currently don't grade alternate events

        Swim,

        Walk,

        Bike_Moving,

        Bike_Stationary
    }

    public enum AgeGroup : byte
    {
        [Display(Name = "17-21")]
        Group_17_to_21,

        [Display(Name = "22-26")]
        Group_22_to_26,

        [Display(Name = "27-31")]
        Group_27_to_31,

        [Display(Name = "32-36")]
        Group_32_to_36,

        [Display(Name = "37-41")]
        Group_37_to_41,

        [Display(Name = "42-46")]
        Group_42_to_46,

        [Display(Name = "47-51")]
        Group_47_to_51,

        [Display(Name = "52-56")]
        Group_52_to_56,

        [Display(Name = "57-61")]
        Group_57_to_61,

        [Display(Name = "62+")]
        Group_62_Plus
    }
}