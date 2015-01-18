using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatteryCommander.Common.Models
{
    public class SoldierQualification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int SoldierId { get; set; }

        public virtual Soldier Soldier { get; set; }

        [Required]
        public QualificationStatus Status { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime QualificationDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExpirationDate { get; set; }

        public SoldierQualification()
        {
            this.Status = QualificationStatus.Unknown;
            this.QualificationDate = DateTime.Today;
        }
    }
}