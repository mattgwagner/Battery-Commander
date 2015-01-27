using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "BTRY HQ")]
        Headquarters,

        [Display(Name = "1st PLT HQ")]
        Headquarters_FirstPlatoon,

        [Display(Name = "1st PLT Gun 1")]
        Gun1,

        [Display(Name = "1st PLT Gun 2")]
        Gun2,

        [Display(Name = "1st PLT Gun 3")]
        Gun3,

        [Display(Name = "1st PLT Gun 4")]
        Gun4,

        [Display(Name = "1st PLT Ammo")]
        Ammo1,

        [Display(Name = "2nd PLT HQ")]
        Headquarters_SecondPlatoon,

        [Display(Name = "2nd PLT Gun 5")]
        Gun5,

        [Display(Name = "2nd PLT Gun 6")]
        Gun6,

        [Display(Name = "2nd PLT Gun 7")]
        Gun7,

        [Display(Name = "2nd PLT Gun 8")]
        Gun8,

        [Display(Name = "2nd PLT Ammo")]
        Ammo2,

        [Display(Name = "Ghost Guns")]
        GhostGuns
    }
}