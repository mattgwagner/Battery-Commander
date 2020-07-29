using BatteryCommander.Web.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BatteryCommander.Web.Models
{
    /// <summary>
    /// Army Combat Fitness Test, replaces the APFT
    /// </summary>
    public class ACFT : IValidatableObject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int SoldierId { get; set; }

        public virtual Soldier Soldier { get; set; }

        // Grader?

        [Required, DataType(DataType.Date), Column(TypeName = "date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; } = DateTime.Today;

        [Display(Name = "For Record")]
        public Boolean ForRecord { get; set; }

        [Display(Name = "Three Rep Maximum Deadlifts (pounds)"), Range(0, 500)]
        public int ThreeRepMaximumDeadlifts { get; set; }

        [Display(Name = "Standing Power Throw (meters)")]
        public decimal StandingPowerThrow { get; set; }

        [Display(Name = "Hand-Release Push-Ups (reps)"), Range(0, 150)]
        public int HandReleasePushups { get; set; }

        [Range(0, double.MaxValue)]
        public int SprintDragCarrySeconds { get; set; }

        [NotMapped, Display(Name = "Sprint-Drag-Carry (time)")]
        public TimeSpan SprintDragCarry
        {
            get { return TimeSpan.FromSeconds(SprintDragCarrySeconds); }
            set { SprintDragCarrySeconds = (int)value.TotalSeconds; }
        }

        [Display(Name = "Leg Tucks (reps)"), Range(0, 50)]
        public int LegTucks { get; set; }

        [Range(0, double.MaxValue)]
        public int TwoMileRunSeconds { get; set; }

        [NotMapped, Display(Name = "Two-Mile Run (time)")]
        public TimeSpan TwoMileRun
        {
            get { return TimeSpan.FromSeconds(TwoMileRunSeconds); }
            set { TwoMileRunSeconds = (int)value.TotalSeconds; }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Date > DateTime.Today) yield return new ValidationResult("Cannot select a date after today", new[] { nameof(Date) });
        }

        [NotMapped, Display(Name = "Total Score")]
        public int TotalScore => Scores.Sum();

        private IEnumerable<int> Scores => new[]
        {
            ACFTScoreTables.MaximumDeadLift(ThreeRepMaximumDeadlifts),
            ACFTScoreTables.StandingPowerThrow(StandingPowerThrow),
            ACFTScoreTables.HandReleasePushUps(HandReleasePushups),
            ACFTScoreTables.SprintDragCarry(SprintDragCarry),
            ACFTScoreTables.LegTuck(LegTucks),
            ACFTScoreTables.TwoMileRun(TwoMileRun),
        };

        // TODO Consider denormalizing this to store on the test itself so when it changes

        [NotMapped]
        public ACFTGradingStandard GradingStandard => Soldier?.ACFT_Grading_Standard ?? ACFTGradingStandard.Gold;

        // Summary

        [DataType(DataType.Date)]
        public DateTime ValidThru => Date.AddDays(Soldier.DaysPerYear);

        public Boolean IsValid => ValidThru > DateTime.Today;

        public Boolean IsPassing
        {
            get
            {
                switch (GradingStandard)
                {
                    case ACFTGradingStandard.Black: return Scores.All(score => score >= 70);
                    case ACFTGradingStandard.Grey: return Scores.All(score => score >= 65);
                    case ACFTGradingStandard.Gold:
                    default:
                        return Scores.All(score => score >= 60);
                }
            }
        }

        // Generate Counseling?
    }

    public enum ACFTGradingStandard : byte
    {
        Gold,
        Grey,
        Black
    }

    public partial class Soldier
    {
        /// <summary>
        /// The Soldier's physical demand standards based on MOS + Rank
        /// </summary>
        [Display(Name = "ACFT Physical Demand Category")]
        public ACFTGradingStandard ACFT_Grading_Standard { get; set; } = ACFTGradingStandard.Gold;

        public virtual ICollection<ACFT> ACFTs { get; set; }

        public virtual ACFT LastAcft => ACFTs?.OrderByDescending(test => test.Date).FirstOrDefault();
    }
}