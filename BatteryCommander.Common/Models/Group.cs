using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatteryCommander.Common.Models
{
    //public class Group
    //{
    //    [Key]
    //    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //    public int Id { get; set; }

    //    [Required, StringLength(50)]
    //    public String Name { get; set; }

    //    // TODO Group hierarchy? i.e. Unit <- 1 PLT <- 1st SQUAD

    //    public int? ParentId { get; set; }

    //    public virtual Group Parent { get; set; }

    //    public int? LeaderId { get; set; }

    //    public virtual Soldier Leader { get; set; }

    //    public virtual ICollection<Soldier> Soldiers { get; set; }

    //    public Group()
    //    {
    //        this.Soldiers = new List<Soldier>();
    //    }
    //}

    public enum Group
    {
        Headquarters,

        Headquarters_FirstPlatoon,

        Headquarters_SecondPlatoon,

        Gun1,

        Gun2,

        Gun3,

        Gun4,

        Gun5,

        Gun6,

        Gun7,

        Gun8,

        Ammo1,

        Ammo2,

        GhostGuns
    }
}