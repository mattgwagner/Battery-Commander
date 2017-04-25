using Humanizer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BatteryCommander.Web.Models
{
    public partial class Evaluation
    {
        /// <summary>
        /// After this period of time, incomplete evaluations are considered delinquent
        /// </summary>
        public static TimeSpan DelinquentAfter => TimeSpan.FromDays(60);

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
        public EvaluationStatus Status { get; set; } = EvaluationStatus.At_Rater;

        [Required]
        public EvaluationType Type { get; set; } = EvaluationType.Annual;

        [NotMapped]
        public Boolean IsCompleted => new[] { EvaluationStatus.Submitted_to_HQDA, EvaluationStatus.Accepted_to_iPerms }.Contains(Status);

        [NotMapped]
        public Boolean IsDelinquent => !IsCompleted && (Delinquency < TimeSpan.Zero && Delinquency > DelinquentAfter);

        [NotMapped, DisplayFormat(DataFormatString = "{0:%d}d")]
        public TimeSpan Delinquency => (ThruDate - DateTime.Today);

        [NotMapped, Display(Name = "Delinquency")]
        public String DelinquencyHumanized => Delinquency.Humanize();

        [NotMapped, DataType(DataType.Date)]
        public DateTimeOffset? LastUpdated => Events.OrderByDescending(e => e.Timestamp).Select(e => (DateTimeOffset?)e.Timestamp).FirstOrDefault();

        [NotMapped, Display(Name = "Last Updated")]
        public String LastUpdatedHumanized => LastUpdated?.Humanize();

        [Display(Name = "History")]
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

            public String TimestampHumanized => Timestamp.Humanize();

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
        [Display(Name = "At Rater")]
        At_Rater = 0,

        [Display(Name = "At Senior Rater")]
        At_Senior_Rater = 10,

        [Display(Name = "Pending 1SG/Admin Review")]
        Pending_Internal_Review = 20,

        [Display(Name = "Ready for Signatures")]
        Ready_for_Signatures = 30,

        [Display(Name = "Pending S1 Review")]
        Pending_S1_Review = 80,

        [Display(Name = "Pending HQDA Submission")]
        Pending_HQDA_Submission = 90,

        [Display(Name = "Submitted to HQDA")]
        Submitted_to_HQDA = 100,

        [Display(Name = "Accepted to iPerms")]
        Accepted_to_iPerms = byte.MaxValue
    }
}