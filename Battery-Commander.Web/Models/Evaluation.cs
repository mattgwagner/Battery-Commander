using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatteryCommander.Web.Models
{
    public class Evaluation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

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
        public DateTime ThruDate { get; set; }

        public EvaluationStatus Status { get; set; } = EvaluationStatus.Unknown;

        [NotMapped]
        public Boolean IsCompleted => EvaluationStatus.Completed == Status;
    }

    public enum EvaluationStatus : byte
    {
        Unknown,

        Completed = 100
    }
}