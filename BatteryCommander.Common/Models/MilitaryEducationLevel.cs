using System.ComponentModel.DataAnnotations;

namespace BatteryCommander.Common.Models
{
    public enum MilitaryEducationLevel
    {
        None,

        [Display(Name = "(AIT) Advanced Individual Training")]
        AIT,

        [Display(Name = "(WLC) Warrior Leader Course")]
        WLC,

        [Display(Name = "Advanced Leader Course")]
        ALC,

        [Display(Name = "Senior Leader Course")]
        SLC,

        [Display(Name = "Basic Officer Leaders Course")]
        BOLC,

        [Display(Name = "Captains Career Course")]
        CCC
    }
}