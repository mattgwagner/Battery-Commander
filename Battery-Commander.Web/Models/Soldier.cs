﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatteryCommander.Web.Models
{
    public class Soldier
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

        public TimeSpan? TimeTillETS => ETSDate - DateTime.Today;

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

        public virtual ICollection<ABCP> ABCPs { get; set; }

        public override string ToString() => $"{Rank.ShortName()} {LastName} {FirstName} {MiddleName}".ToUpper();
    }
}