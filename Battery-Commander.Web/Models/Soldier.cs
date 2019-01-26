using Humanizer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatteryCommander.Web.Models
{
    public partial class Soldier : IValidatableObject
    {
        public const double DaysPerYear = 365.2425;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "Last Name")]
        public String LastName { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "First Name")]
        public String FirstName { get; set; }

        [StringLength(50), Display(Name = "Middle Name")]
        public String MiddleName { get; set; }

        public int? SupervisorId { get; set; }

        public Soldier Supervisor { get; set; }

        [DataType(DataType.Date), Column(TypeName = "date"), Display(Name = "Date of Rank"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DateOfRank { get; set; }

        [NotMapped, Display(Name = "Time in Grade")]
        public TimeSpan? TimeInGrade => (DateTime.Today - DateOfRank);

        [NotMapped, Display(Name = "Time in Grade")]
        public String TimeInGradeHumanized => TimeInGrade?.Humanize(maxUnit: Humanizer.Localisation.TimeUnit.Year, minUnit: Humanizer.Localisation.TimeUnit.Day, precision: 2);

        [StringLength(12), Display(Name = "DOD ID")]
        public String DoDId { get; set; }

        [DataType(DataType.EmailAddress), StringLength(50)]
        public String MilitaryEmail { get; set; }

        [DataType(DataType.EmailAddress), StringLength(50)]
        public String CivilianEmail { get; set; }

        [DataType(DataType.Date), Column(TypeName = "date"), Display(Name = "DOB"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateOfBirth { get; set; }

        public Boolean HasDoB => DateOfBirth > DateTime.MinValue;

        public int Age => AgeAsOf(DateTime.Today);

        public int AgeAsOf(DateTime date)
        {
            // They may not have reached their birthday for this year

            return (int)((date - DateOfBirth).TotalDays / DaysPerYear);
        }

        [DataType(DataType.Date), Column(TypeName = "date"), Display(Name = "ETS Date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? ETSDate { get; set; }

        [NotMapped, DisplayFormat(DataFormatString = "{0:%d}d"), Display(Name = "Till ETS")]
        public TimeSpan? TimeTillETS => ETSDate - DateTime.Today;

        [NotMapped, Display(Name = "Till ETS")]
        public String TimeTillETSHumanized => TimeTillETS?.Humanize(maxUnit: Humanizer.Localisation.TimeUnit.Year, minUnit: Humanizer.Localisation.TimeUnit.Day);

        [Required]
        public Gender Gender { get; set; } = Gender.Male;

        [Required]
        public int UnitId { get; set; }

        public virtual Unit Unit { get; set; }

        public SoldierStatus Status { get; set; } = SoldierStatus.Unknown;

        // Position

        // Security Clearance

        // MOS - Duty MOSQ'd?

        // PEBD

        /// <summary>
        /// If true, the user has been granted access to log in to the application
        /// </summary>
        [Display(Name = "Login Enabled")]
        public Boolean CanLogin { get; set; } = false;

        public override string ToString() => $"{Rank.ShortName()} {LastName} {FirstName} {MiddleName}".ToUpper();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DateOfBirth > DateTime.Today.AddYears(-17)) yield return new ValidationResult("DateOfBirth doesn't seem right", new[] { nameof(DateOfBirth) });
        }

        public enum SoldierStatus : byte
        {
            // Where do RSP, IET, and OCS fall in here?

            Unknown = 0,

            [Display(Name = "Present")]
            PresentForDuty = 1,

            Detached = 2,

            [Display(Name = "Pass")]
            OnPass = 3,

            TDY = 4,

            AWOL = 5,

            [Display(Name = "Rear Det")]
            RearDetachment = 6
        }
    }
}