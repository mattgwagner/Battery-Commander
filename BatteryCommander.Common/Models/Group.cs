using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatteryCommander.Common.Models
{
    public class Group
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, StringLength(50)]
        public String Name { get; set; }

        public virtual ICollection<Soldier> Soldiers { get; set; }

        public Group()
        {
            this.Soldiers = new List<Soldier>();
        }
    }
}