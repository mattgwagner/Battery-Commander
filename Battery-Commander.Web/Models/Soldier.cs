using Humanizer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BatteryCommander.Web.Models
{
    public class Soldier : IValidatableObject
    {
        private const double DaysPerYear = 365.2425;

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

        [Required]
        public Rank Rank { get; set; } = Rank.E1;

        [NotMapped]
        public Boolean IsNCO => Rank.IsNCO();

        [NotMapped]
        public Boolean IsOfficer => Rank.IsOfficer();

        [DataType(DataType.Date), Column(TypeName = "date"), Display(Name = "Date of Rank")]
        public DateTime? DateOfRank { get; set; }

        [NotMapped, Display(Name = "Time in Grade")]
        public TimeSpan? TimeInGrade => (DateTime.Today - DateOfRank);

        [NotMapped, Display(Name = "Time in Grade")]
        public String TimeInGradeHumanized => TimeInGrade?.Humanize(precision: 2);

        [StringLength(12)]
        public String DoDId { get; set; }

        [DataType(DataType.EmailAddress), StringLength(50)]
        public String MilitaryEmail { get; set; }

        [DataType(DataType.EmailAddress), StringLength(50)]
        public String CivilianEmail { get; set; }

        [DataType(DataType.Date), Column(TypeName = "date"), Display(Name = "DOB")]
        public DateTime DateOfBirth { get; set; }

        public int Age => AgeAsOf(DateTime.Today);

        public int AgeAsOf(DateTime date)
        {
            // They may not have reached their birthday for this year

            return (int)((date - DateOfBirth).TotalDays / DaysPerYear);
        }

        [DataType(DataType.Date), Column(TypeName = "date"), Display(Name = "ETS Date")]
        public DateTime? ETSDate { get; set; }

        [NotMapped, DisplayFormat(DataFormatString = "{0:%d}d"), Display(Name = "Till ETS")]
        public TimeSpan? TimeTillETS => ETSDate - DateTime.Today;

        [NotMapped, Display(Name = "Till ETS")]
        public String TimeTillETSHumanized => TimeTillETS?.Humanize(precision: 2);

        [Required]
        public Gender Gender { get; set; } = Gender.Male;

        // public MilitaryEducationLevel EducationLevel { get; set; } = MilitaryEducationLevel.None;

        // Status - Active, Inactive

        [Required]
        public int UnitId { get; set; }

        public virtual Unit Unit { get; set; }

        // Position

        // Security Clearance

        // MOS - Duty MOSQ'd?

        // ETS Date & Time till ETS

        // PEBD

        // Date of Rank

        public virtual ICollection<APFT> APFTs { get; set; }

        public virtual APFT LastApft => APFTs.OrderByDescending(apft => apft.Date).FirstOrDefault();

        public virtual EventStatus ApftStatus
        {
            get
            {
                if (LastApft?.IsPassing == true) return EventStatus.Passed;

                if (LastApft?.IsPassing == false) return EventStatus.Failed;

                return EventStatus.NotTested;
            }
        }

        public virtual ICollection<ABCP> ABCPs { get; set; }

        public virtual ABCP LastAbcp => ABCPs.OrderByDescending(abcp => abcp.Date).FirstOrDefault();

        public virtual EventStatus AbcpStatus
        {
            get
            {
                // TODO This doesn't take into account soldiers who passed tape but are still on the ABCP program

                if (LastAbcp?.IsPassing == true) return EventStatus.Passed;

                if (LastAbcp?.IsPassing == false) return EventStatus.Failed;

                return EventStatus.NotTested;
            }
        }

        public override string ToString() => $"{Rank.ShortName()} {LastName} {FirstName} {MiddleName}".ToUpper();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DateOfBirth > DateTime.Today.AddYears(-17)) yield return new ValidationResult("DateOfBirth doesn't seem right", new[] { nameof(DateOfBirth) });
        }

        public enum EventStatus
        {
            NotTested,

            Passed,

            Failed
        }
    }
}