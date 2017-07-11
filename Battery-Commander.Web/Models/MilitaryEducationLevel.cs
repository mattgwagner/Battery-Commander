using System.ComponentModel.DataAnnotations;

namespace BatteryCommander.Web.Models
{
    public enum MilitaryEducationLevel : byte
    {
        Unknown,

        [Display(Name = "(AIT) Advanced Individual Training", ShortName = "AIT")]
        AIT = 1,

        [Display(Name = "(BLC) Basic Leader Course", ShortName = "BLC")]
        BLC = 2,

        [Display(Name = "(ALC) Advanced Leader Course", ShortName = "ALC")]
        ALC = 3,

        [Display(Name = "(SLC) Senior Leader Course", ShortName = "SLC")]
        SLC = 4,

        [Display(Name = "(BOLC) Basic Officer Leaders Course", ShortName = "BOLC")]
        BOLC = 10,

        [Display(Name = "(CCC) Captains Career Course", ShortName = "CCC")]
        CCC = 11
    }
}