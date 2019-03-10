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
        public static TimeSpan DelinquentAfter => TimeSpan.FromDays(-90);

        /// <summary>
        /// List of statuses that represent completed evaluations
        /// </summary>
        public static IEnumerable<EvaluationStatus> Completed => new[] { EvaluationStatus.Submitted_to_HQDA, EvaluationStatus.Accepted_to_iPerms };

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

        [Display(Name = "Senior Rater")]
        public virtual Soldier SeniorRater { get; set; }

        public int? ReviewerId { get; set; }

        public virtual Soldier Reviewer { get; set; }

        [Required, DataType(DataType.Date), Column(TypeName = "date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required, DataType(DataType.Date), Column(TypeName = "date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}"), Display(Name = "Thru Date")]
        public DateTime ThruDate { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}"), Display(Name = "Sign On/After")]
        public DateTime CanBeSignedAfter => ThruDate.AddDays(-14);

        [Required]
        public EvaluationStatus Status { get; set; } = EvaluationStatus.At_Rater;

        [Required]
        public EvaluationType Type { get; set; } = EvaluationType.Annual;

        [NotMapped]
        public Boolean IsCompleted => Completed.Contains(Status);

        [NotMapped]
        public Boolean IsDelinquent => !IsCompleted && (Delinquency < DelinquentAfter);

        [NotMapped, DisplayFormat(DataFormatString = "{0:%d}d")]
        public TimeSpan Delinquency => (ThruDate - DateTime.Today);

        [NotMapped, Display(Name = "Delinquency")]
        public String DelinquencyHumanized => Delinquency.Humanize(maxUnit: Humanizer.Localisation.TimeUnit.Day);

        [NotMapped]
        public virtual Event LastEvent => Events.OrderByDescending(e => e.Timestamp).FirstOrDefault();

        [NotMapped, DataType(DataType.Date), Display(Name = "Last Updated")]
        public DateTimeOffset? LastUpdated => LastEvent?.Timestamp;

        [NotMapped]
        public String LastUpdatedEst => $"{LastUpdated.ToEst()}";

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

            public override string ToString() => $"{Author}: {Message}";
        }
    }

    public enum EvaluationType : byte
    {
        Annual = 0,

        [Display(Name = "Change of Rater")]
        Change_of_Rater = 1,

        [Display(Name = "Complete the Record")]
        Complete_the_Record = 2,

        [Display(Name = "Change of Duty")]
        Change_of_Duty = 3,

        Retirement = 4,

        Discharge = 5,

        [Display(Name = "Extended Annual")]
        Extended_Annual = 6,

        [Display(Name = "Relief for Cause")]
        Relief_for_Cause = 7
    }

    public enum EvaluationStatus : byte
    {
        [Display(Name = "At Rater")]
        At_Rater = 0,

        [Display(Name = "At Senior Rater")]
        At_Senior_Rater = 10,

        [Display(Name = "At Reviewer")]
        At_Reviewer = 15,

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
