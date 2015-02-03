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

        [Display(Name = "(ALC) Advanced Leader Course")]
        ALC,

        [Display(Name = "(SLC) Senior Leader Course")]
        SLC,

        [Display(Name = "(BOLC) Basic Officer Leaders Course")]
        BOLC,

        [Display(Name = "(CCC) Captains Career Course")]
        CCC
    }
}