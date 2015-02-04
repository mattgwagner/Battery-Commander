using System.ComponentModel.DataAnnotations;

namespace BatteryCommander.Common.Models
{
    //public class Position
    //{
    //    [Key]
    //    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //    public int Id { get; set; }

    //    [Required, StringLength(30)]
    //    public String Name { get; set; }

    //    [Required, StringLength(5)]
    //    public String Abbreviation { get; set; }
    //}

    public enum Position
    {
        Unassigned,

        [Display(Name = "Signal NCO", ShortName = "SIG SGT")]
        Signal_NCO,

        [Display(Name = "Supply Specialist", ShortName = "SUP SPC")]
        Supply_Specialist,

        [Display(Name = "Supply NCO", ShortName = "SUP NCO")]
        Supply_NCO,

        [Display(Name = "Ammo Handler", ShortName = "AH")]
        Ammo_Section_Member,

        [Display(Name = "Ammo Section Sergeant", ShortName = "AMMO SGT")]
        Ammo_Section_Sergeant,

        [Display(Name = "Ammo Team Chief", ShortName = "ATC")]
        Ammo_Team_Chief,

        [Display(Name = "Cannoneer", ShortName = "CN")]
        Cannoneer,

        [Display(Name = "Driver", ShortName = "DRV")]
        Driver,

        [Display(Name = "Asst. Gunner", ShortName = "AG")]
        Assistant_Gunner,

        [Display(Name = "Gunner", ShortName = "G")]
        Gunner,

        [Display(Name = "Field Artillery Tactical Data Specialist", ShortName = "FATDS")]
        FATDS,

        [Display(Name = "Section Chief", ShortName = "CHF")]
        Gun_Section_Chief,

        [Display(Name = "Gunnery Sergeant", ShortName = "GSG")]
        Gunnery_Sergeant,

        [Display(Name = "Platoon Sergeant", ShortName = "PSG")]
        Platoon_Sergeant,

        [Display(Name = "Fire Direction Officer", ShortName = "FDO")]
        Fire_Direction_Officer,

        [Display(Name = "Platoon Leader", ShortName = "PL")]
        Platoon_Leader,

        [Display(Name = "First Sergeant", ShortName = "1SG")]
        First_Sergeant,

        [Display(Name = "Commander", ShortName = "CMDR")]
        Commander
    }
}