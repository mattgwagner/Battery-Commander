using System.ComponentModel.DataAnnotations;

namespace BatteryCommander.Web.Models
{
    public enum MilitaryEducationLevel : byte
    {
        None,

        [Display(Name = "(AIT) Advanced Individual Training", ShortName = "AIT")]
        AIT,

        [Display(Name = "(WLC) Warrior Leader Course", ShortName = "WLC")]
        WLC,

        [Display(Name = "(ALC) Advanced Leader Course", ShortName = "ALC")]
        ALC,

        [Display(Name = "(SLC) Senior Leader Course", ShortName = "SLC")]
        SLC,

        [Display(Name = "(BOLC) Basic Officer Leaders Course", ShortName = "BOLC")]
        BOLC,

        [Display(Name = "(CCC) Captains Career Course", ShortName = "CCC")]
        CCC
    }
}