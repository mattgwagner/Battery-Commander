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

        [Display(Name = "Three Rep Maximum Deadlifts")]
        public int ThreeRepMaximumDeadlifts { get; set; }

        [Display(Name = "Standing Power Throw")]
        public int StandingPowerThrow { get; set; }

        [Display(Name = "Hand-Release Push-Ups")]
        public int HandReleasePushups { get; set; }

        public int SprintDragCarrySeconds { get; set; }

        [NotMapped]
        public TimeSpan SprintDragCarry
        {
            get { return TimeSpan.FromSeconds(SprintDragCarrySeconds); }
            set { SprintDragCarrySeconds = (int)value.TotalSeconds; }
        }

        [Display(Name = "Leg Tucks")]
        public int LegTucks { get; set; }

        public int TwoMileRunSeconds { get; set; }

        [NotMapped, Display(Name = "Two-Mile Run")]
        public TimeSpan TwoMileRun
        {
            get { return TimeSpan.FromSeconds(TwoMileRunSeconds); }
            set { TwoMileRunSeconds = (int)value.TotalSeconds; }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Date > DateTime.Today) yield return new ValidationResult("Cannot select a date after today", new[] { nameof(Date) });
        }

        // Raw Data by Event

        // Event Scores

        public int TotalScore => 0;

        // Soldier Info

        // TODO Phsyical Standards Category read off of the Soldier

        // Summary

        [DataType(DataType.Date)]
        public DateTime ValidThru => Date.AddDays(Soldier.DaysPerYear);

        public Boolean IsValid => ValidThru > DateTime.Today;

        public Boolean IsPassing
        {
            get
            {
                // Look at the Soldier's physical standards category

                // Compare that to the total score and min per event

                // Return true or false

                return false;
            }
        }

        // Generate Counseling?
    }

    public partial class Soldier
    {
        // TODO Phsyical Standards Category? By MOS or unit?

        public virtual ICollection<ACFT> ACFTs { get; set; }

        public virtual ACFT LastAcft => ACFTs?.OrderByDescending(test => test.Date).FirstOrDefault();
    }
}