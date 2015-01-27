using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatteryCommander.Common.Models
{
    public class Position
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, StringLength(30)]
        public String Name { get; set; }

        [Required, StringLength(5)]
        public String Abbreviation { get; set; }
    }

    //public enum Position
    //{
    //    Unassigned,

    //    Signal_NCO,

    //    Supply_Specialist,

    //    Supply_NCO,

    //    Ammo_Section_Member,

    //    Ammo_Section_Sergeant,

    //    Ammo_Section_Chief,

    //    Ammo_Team_Chief,

    //    Cannoneer_1,

    //    Cannoneer_2,

    //    Driver,

    //    Assistant_Gunner,

    //    Gunner,

    //    Gun__Section_Chief,

    //    Gunnery_Sergeant,

    //    Platoon_Sergeant,

    //    Fire_Direction_Officer,

    //    Platoon_Leader,

    //    First_Sergeant,

    //    Commander
    //}
}