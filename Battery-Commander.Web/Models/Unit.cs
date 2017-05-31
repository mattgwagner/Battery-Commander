using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatteryCommander.Web.Models
{
    public class Unit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public String Name { get; set; }

        public String UIC { get; set; }

        public Boolean IgnoreForReports { get; set; }

        public virtual ICollection<Soldier> Soldiers { get; set; }
    }
}