using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatteryCommander.Common.Models
{
    public class Qualification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public String Name { get; set; }

        public String Description { get; set; }

        // TODO Length of time qual is valid?

        [Display(Name = "Parent Task")]
        public virtual Qualification ParentTask { get; set; }

        public int? ParentTaskId { get; set; }

        [Display(Name = "Soldier Qualifications")]
        public virtual ICollection<SoldierQualification> SoldierQualifications { get; set; }

        [Display(Name = "Qualification Tasks")]
        public virtual ICollection<Qualification> Tasks { get; set; }

        public Qualification()
        {
            this.SoldierQualifications = new List<SoldierQualification>();
            this.Tasks = new List<Qualification>();
        }
    }
}