﻿using Humanizer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BatteryCommander.Web.Models
{
    public class Evaluation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "EES ID")]
        public int? EvaluationId { get; set; }

        [DataType(DataType.Url)]
        public String EvaluationLink
        {
            get
            {
                const string BaseUri = "https://evaluations.hrc.army.mil";

                if (Ratee?.IsOfficer == true)
                {
                    return $"{BaseUri}/signatureController.html?eid={EvaluationId}";
                }
                else
                {
                    return $"{BaseUri}/ncoerSignatureController.html?eid={EvaluationId}";
                }
            }
        }

        [Required]
        public int RateeId { get; set; }

        public virtual Soldier Ratee { get; set; }

        [Required]
        public int RaterId { get; set; }

        public virtual Soldier Rater { get; set; }

        [Required]
        public int SeniorRaterId { get; set; }

        public virtual Soldier SeniorRater { get; set; }

        [Required, DataType(DataType.Date), Column(TypeName = "date")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required, DataType(DataType.Date), Column(TypeName = "date")]
        public DateTime ThruDate { get; set; }

        [Required]
        public EvaluationStatus Status { get; set; } = EvaluationStatus.Unknown;

        [Required]
        public EvaluationType Type { get; set; } = EvaluationType.Annual;

        [NotMapped]
        public Boolean IsCompleted => EvaluationStatus.Completed == Status;

        [NotMapped, DisplayFormat(DataFormatString = "{0:%d}d")]
        public TimeSpan Delinquency => (ThruDate - DateTime.Today);

        [NotMapped, Display(Name = "Delinquency")]
        public String DelinquencyHumanized => Delinquency.Humanize();

        [NotMapped, DataType(DataType.Date)]
        public DateTimeOffset? LastUpdated => Events.OrderByDescending(e => e.Timestamp).Select(e => (DateTimeOffset?)e.Timestamp).FirstOrDefault();

        public virtual ICollection<Event> Events { get; set; } = new List<Event>();

        public class Event
        {
            // Could represent a state transition or a manual comment added, perhaps we need to flag that?

            [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }

            public int EvaluationId { get; set; }

            public virtual Evaluation Evaluation { get; set; }

            [DataType(DataType.DateTime)]
            public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

            // Username? Printed name?

            public string Author { get; set; }

            public string Message { get; set; }
        }
    }

    public enum EvaluationType : byte
    {
        Annual,

        [Display(Name = "Change of Rater")]
        Change_of_Rater,

        [Display(Name = "Complete the Record")]
        Complete_the_Record
    }

    public enum EvaluationStatus : byte
    {
        Unknown,

        Completed = 100
    }
}