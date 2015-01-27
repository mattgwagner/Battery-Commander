using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatteryCommander.Common.Models
{
    public class Soldier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, StringLength(50)]
        public String LastName { get; set; }

        [Required, StringLength(50)]
        public String FirstName { get; set; }

        [Required]
        public Rank Rank { get; set; }

        [Required]
        public SoldierStatus Status { get; set; }

        // TODO Position - PL, FDO, Section Chief, etc.

        [Required]
        public MOS MOS { get; set; }

        [Required]
        public Boolean IsDutyMOSQualified { get; set; }

        [Required]
        public MilitaryEducationLevel EducationLevelCompleted { get; set; }

        [Required]
        public Group Group { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ETSDate { get; set; }

        public virtual ICollection<SoldierQualification> Qualifications { get; set; }

        [StringLength(300)]
        public String Notes { get; set; }

        public Soldier()
        {
            this.Rank = Models.Rank.E1;
            this.Status = Models.SoldierStatus.Active;
            this.MOS = Models.MOS.Unknown;
            this.EducationLevelCompleted = MilitaryEducationLevel.None;
            this.Group = Models.Group.GhostGuns;

            this.Qualifications = new List<SoldierQualification>();
        }
    }
}