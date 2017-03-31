using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BatteryCommander.Web.Models
{
    public class APFT
    {
        private const int MinimumPerEvent = 60;

        private const int MinimumTotal = MinimumPerEvent * 3;

        private const double DaysPerYear = 365.2425;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int SoldierId { get; set; }

        public virtual Soldier Soldier { get; set; }

        [Required, DataType(DataType.Date), Column(TypeName = "date")]
        public DateTime Date { get; set; } = DateTime.Today;

        public int Age
        {
            get
            {
                // They may not have reached their birthday for this year

                return (int)((Date - Soldier.DateOfBirth).TotalDays / DaysPerYear);
            }
        }

        public int PushUps { get; set; }

        public int SitUps { get; set; }

        public int RunSeconds { get; set; }

        public TimeSpan Run => TimeSpan.FromSeconds(RunSeconds);

        [NotMapped]
        public int PushUpScore;

        [NotMapped]
        public int SitUpScore;

        [NotMapped]
        public int RunScore;

        [NotMapped]
        public int TotalScore => PushUpScore + SitUpScore + RunScore;

        [NotMapped]
        public Boolean IsPassing
        {
            get
            {
                return new[] { PushUpScore, SitUpScore, RunScore }.All(s => s >= MinimumPerEvent) && TotalScore >= MinimumTotal;
            }
        }
    }
}